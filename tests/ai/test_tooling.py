"""Observable behavior tests for the portable adoption tools (stdlib only)."""
import importlib.util
import json
from pathlib import Path
import re
import subprocess
import tempfile
import unittest
from unittest.mock import patch
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]
PLUGIN = ROOT / "plugins/ceataec-dotnet"


def load(name):
    spec = importlib.util.spec_from_file_location(name, PLUGIN / "scripts" / f"{name}.py")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


score = load("score")
attach = load("attach")
scaffold = load("scaffold")
verify = load("verify")


class ScoringTests(unittest.TestCase):
    def document(self, entries):
        return {"standardVersion": "0.2.0", "profile": "http-api-postgres",
                "target": "fixture@abc", "assessments": entries}

    def test_unknowns_do_not_appear_as_full_evidence(self):
        result = score.calculate(self.document([
            {"id": "ARC-001", "status": "pass", "evidence": ["src/Domain.csproj:4"]}
        ]))
        self.assertEqual(result["observedScore"], 100)
        self.assertEqual(result["evidenceCoverage"], 15)
        self.assertEqual(result["conservativeScore"], 15)
        self.assertTrue(result["provisional"])

    def test_no_evidence_and_all_na_have_no_observed_score(self):
        self.assertIsNone(score.calculate(self.document([]))["observedScore"])
        catalog = json.loads(score.CATALOG.read_text())
        entries = [{"id": rule["id"], "status": "not-applicable",
                    "reason": "Fixture with no applicable capabilities", "evidence": ["scope.md:1"]}
                   for rule in catalog["rules"]]
        result = score.calculate(self.document(entries))
        self.assertIsNone(result["observedScore"])
        self.assertIsNone(result["evidenceCoverage"])

    def test_credit_and_na_denominators(self):
        result = score.calculate(self.document([
            {"id": "ARC-001", "status": "partial", "reason": "One boundary violation",
             "evidence": ["Domain.csproj:2"]},
            {"id": "SLI-001", "status": "fail", "reason": "Endpoints own database calls",
             "evidence": ["Endpoint.cs:4"]},
            {"id": "DOM-001", "status": "not-applicable", "reason": "No domain operations",
             "evidence": ["scope.md:2"]},
        ]))
        self.assertEqual(result["observedScore"], 30)
        self.assertEqual(result["applicableWeight"], 90)
        self.assertEqual(result["assessedWeight"], 25)

    def test_assessments_cannot_override_catalog_weights(self):
        result = score.calculate(self.document([
            {"id": "ARC-001", "status": "pass", "evidence": ["Domain.csproj:2"],
             "weight": 900, "kind": "readiness"}
        ]))
        self.assertEqual(result["assessedWeight"], 15)
        self.assertEqual(result["byKind"]["readiness"]["assessedWeight"], 0)

    def test_invalid_inputs_rejected(self):
        good = {"id": "ARC-001", "status": "pass", "evidence": ["Domain.csproj:2"]}
        for entries in ([good, good], [{**good, "id": "BOGUS"}],
                        [{**good, "evidence": []}], [{**good, "status": "not-applicable"}],
                        [{**good, "evidence": "not an array"}]):
            with self.subTest(entries=entries), self.assertRaises(ValueError):
                score.calculate(self.document(entries))
        with self.assertRaises(ValueError):
            score.calculate({**self.document([]), "standardVersion": "2.0.0"})


class AttachmentTests(unittest.TestCase):
    def install_previous(self, root):
        with tempfile.TemporaryDirectory(dir=ROOT.parent) as temp:
            old_plugin = Path(temp)
            import shutil
            shutil.copytree(PLUGIN, old_plugin, dirs_exist_ok=True)
            catalog = old_plugin / "spec/rules.json"
            contents = json.loads(catalog.read_text())
            contents["version"] = "0.1.0"
            catalog.write_text(json.dumps(contents))
            files = attach.plan_attachment(root, "claude", plugin=old_plugin)
            for relative, data in files.items():
                path = root / relative
                path.parent.mkdir(parents=True, exist_ok=True)
                path.write_bytes(data)

    def test_upgrade_preserves_previous_snapshot_and_repoints_wrappers(self):
        with tempfile.TemporaryDirectory(dir=ROOT.parent) as temp:
            root = Path(temp)
            self.install_previous(root)
            previous = (root / ".ceataec/dotnet/0.1.0/spec/rules.json").read_bytes()
            attach.attach(root, "claude", upgrade_from="0.1.0")
            self.assertEqual(previous, (root / ".ceataec/dotnet/0.1.0/spec/rules.json").read_bytes())
            self.assertIn("0.2.0", (root / ".claude/skills/ceataec-refactor/SKILL.md").read_text())

    def test_upgrade_refuses_locally_changed_spec_before_writing(self):
        with tempfile.TemporaryDirectory(dir=ROOT.parent) as temp:
            root = Path(temp)
            self.install_previous(root)
            (root / ".ceataec/dotnet/0.1.0/spec/standard.md").write_text("Local amendments")
            with self.assertRaises(ValueError):
                attach.attach(root, "claude", upgrade_from="0.1.0")
            self.assertFalse((root / ".ceataec/dotnet/0.2.0").exists())

    def test_each_harness_links_to_complete_snapshot_and_is_idempotent(self):
        for harness, directory in attach.HARNESS_DIR.items():
            with self.subTest(harness=harness), tempfile.TemporaryDirectory(dir=ROOT.parent) as temp:
                root = Path(temp)
                self.assertTrue(attach.attach(root, harness, dry_run=True))
                self.assertEqual(list(root.iterdir()), [])
                self.assertTrue(attach.attach(root, harness))
                self.assertEqual(attach.attach(root, harness), [])
                for wrapper in (root / directory / "skills").glob("*/SKILL.md"):
                    link = re.search(r'\]\(([^)]+)\)', wrapper.read_text()).group(1)
                    self.assertTrue((wrapper.parent / link).resolve().is_file())
                self.assertTrue((root / ".ceataec/dotnet/0.2.0/spec/rules.json").is_file())

    def test_conflict_preflight_does_not_partially_install(self):
        with tempfile.TemporaryDirectory(dir=ROOT.parent) as temp:
            root = Path(temp)
            user_file = root / ".claude/skills/ceataec-refactor/SKILL.md"
            user_file.parent.mkdir(parents=True)
            user_file.write_text("user-owned skill")
            with self.assertRaises(ValueError):
                attach.attach(root, "claude")
            self.assertFalse((root / ".ceataec").exists())
            self.assertEqual(user_file.read_text(), "user-owned skill")

    def test_target_symlink_is_not_followed(self):
        with tempfile.TemporaryDirectory(dir=ROOT.parent) as temp:
            root = Path(temp) / "target"
            other = Path(temp) / "other"
            root.mkdir()
            other.mkdir()
            (root / ".ceataec").symlink_to(other, target_is_directory=True)
            with self.assertRaises(ValueError):
                attach.attach(root, "claude")
            self.assertEqual(list(other.iterdir()), [])


class ScaffoldTests(unittest.TestCase):
    def test_both_variants_have_resolvable_project_references(self):
        for aspire in ("yes", "no"):
            with self.subTest(aspire=aspire):
                files, metadata = scaffold.render(ROOT, "Acme.Orders", aspire)
                projects = [path for path in files if path.endswith(".csproj")]
                self.assertEqual(len(projects), 10 if aspire == "yes" else 9)
                for project in projects:
                    xml = ET.fromstring(files[project])
                    for reference in xml.iter("ProjectReference"):
                        path = (Path("/fixture") / Path(project).parent /
                                reference.attrib["Include"].replace("\\", "/")).resolve()
                        self.assertIn(path.relative_to("/fixture").as_posix(), files)
                solution = files["Acme.Orders.sln"].decode()
                sln_paths = re.findall(r'"([^"\n]+\.csproj)"', solution)
                self.assertEqual(len(sln_paths), len(projects))
                for path in sln_paths:
                    self.assertIn(path.replace("\\", "/"), files)
                source = b"\n".join(data for path, data in files.items() if path.endswith((".cs", ".csproj")))
                self.assertNotIn(b"Ceataec.ExampleService", source)
                self.assertNotIn(b"Ceataec_ExampleService", source)
                self.assertFalse(any("Properties/Properties" in path for path in files))
                self.assertFalse(any(path.startswith("tests/ai/") for path in files))
                api_settings = json.loads(files["src/Acme.Orders.Api/appsettings.json"])
                self.assertEqual(api_settings["Database"]["ConnectionString"], "")
                factory = files["tests/Acme.Orders.Api.IntegrationTests/ExampleWebApplicationFactory.cs"]
                self.assertIn(b'["ConnectionStrings:ceataec"] = _postgres.GetConnectionString()', factory)
                self.assertEqual(metadata["aspire"], aspire == "yes")
                if aspire == "no":
                    self.assertNotIn(b"EnrichNpgsqlDbContext", source)
                    self.assertNotIn(b'Include="Aspire.', source)
                    self.assertNotIn(".AppHost", solution)
                    self.assertIn("src/Acme.Orders.ServiceDefaults/Extensions.cs", files)
                else:
                    self.assertIn(b"Projects.Acme_Orders_Api", source)

    def test_dry_run_and_existing_output_safety(self):
        with tempfile.TemporaryDirectory(dir=ROOT.parent) as temp:
            output = Path(temp) / "NewService"
            scaffold.scaffold(ROOT, "Acme.Orders", output, dry_run=True)
            self.assertFalse(output.exists())
            scaffold.scaffold(ROOT, "Acme.Orders", output)
            marker = output / "user.txt"
            marker.write_text("preserve")
            with self.assertRaises(ValueError):
                scaffold.scaffold(ROOT, "Acme.Other", output)
            self.assertEqual(marker.read_text(), "preserve")

    def test_invalid_names_and_nested_output_rejected(self):
        for name in ("../Escape", "a-b", "lowercase", "Acme..Orders", "Acme.Con"):
            with self.subTest(name=name), self.assertRaises(ValueError):
                scaffold.render(ROOT, name, "no")
        with self.assertRaises(ValueError):
            scaffold.scaffold(ROOT, "Acme.Orders", ROOT / "nested-output")

    def test_source_dirty_changes_are_not_copied(self):
        with tempfile.TemporaryDirectory(dir=ROOT.parent) as temp:
            checkout = Path(temp) / "reference"
            subprocess.run(["git", "clone", "--quiet", "--no-hardlinks", str(ROOT), str(checkout)], check=True)
            program = checkout / "src/Ceataec.ExampleService.Api/Program.cs"
            program.write_text("DO NOT COPY WORKTREE CONTENT")
            (checkout / "src/untracked.cs").write_text("UNTRACKED")
            files, _ = scaffold.render(checkout, "Acme.Orders", "yes")
            self.assertNotIn("src/untracked.cs", files)
            self.assertNotIn(b"DO NOT COPY", files["src/Acme.Orders.Api/Program.cs"])

    def test_user_secrets_identity_is_fresh(self):
        first, _ = scaffold.render(ROOT, "Acme.Orders", "yes")
        second, _ = scaffold.render(ROOT, "Acme.Orders", "yes")
        project = "src/Acme.Orders.AppHost/Acme.Orders.AppHost.csproj"
        self.assertNotEqual(ET.fromstring(first[project]).find(".//UserSecretsId").text,
                            ET.fromstring(second[project]).find(".//UserSecretsId").text)


class VerificationTests(unittest.TestCase):
    def run_fixture(self, fail=None, scope="all", docker=True, sdk=True):
        calls = []
        def runner(command, **kwargs):
            calls.append(command)
            return subprocess.CompletedProcess(command, 1 if fail and fail in command else 0)
        result = verify.run_checks(ROOT, "Ceataec.ExampleService.sln", scope,
                                   runner=runner, which=lambda tool: tool if (sdk if tool == "dotnet" else docker) else None)
        return result, calls

    def test_restore_failure_blocks_build_and_tests(self):
        result, calls = self.run_fixture(fail="restore")
        self.assertEqual(len(calls), 1)
        self.assertFalse(result["selectedChecksPassed"])
        self.assertTrue(all(item["status"] == "blocked" for item in result["checks"][1:]))

    def test_missing_sdk_is_blocked_not_passed(self):
        result, calls = self.run_fixture(sdk=False)
        self.assertEqual(calls, [])
        self.assertFalse(result["selectedChecksPassed"])

    def test_missing_docker_does_not_prevent_unit_tests(self):
        result, calls = self.run_fixture(docker=False)
        self.assertEqual(sum("test" in c for c in calls), 4)
        self.assertFalse(result["selectedChecksPassed"])
        self.assertEqual(next(c for c in result["checks"] if c.get("integration"))["status"], "blocked")

    def test_unit_scope_does_not_claim_full_verification(self):
        result, calls = self.run_fixture(scope="unit", docker=False)
        self.assertTrue(result["selectedChecksPassed"])
        self.assertFalse(result["allChecksPassed"])
        self.assertFalse(any("docker" in c for c in calls))

    def test_plan_does_not_execute_target_code(self):
        with patch.object(subprocess, "run", side_effect=AssertionError("must not execute")):
            self.assertEqual(len(verify.commands(ROOT, "Ceataec.ExampleService.sln")), 7)

    def test_outside_solution_rejected(self):
        with self.assertRaises(ValueError):
            verify.commands(ROOT, "../other.sln")


if __name__ == "__main__":
    unittest.main()

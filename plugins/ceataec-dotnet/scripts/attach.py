#!/usr/bin/env python3
"""Attach a versioned plugin snapshot and skill entry points without overwriting files."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import re
import tempfile

PLUGIN = Path(__file__).resolve().parents[1]
HARNESS_DIR = {"claude": ".claude", "cursor": ".cursor", "codex": ".agents"}


def plan_attachment(target, harness, plugin=PLUGIN, upgrade_from=None):
    target = Path(target).resolve()
    if not target.is_dir():
        raise ValueError("target must be an existing repository directory")
    catalog = json.loads((plugin / "spec/rules.json").read_text(encoding="utf-8"))
    version = catalog["version"]
    vendor = Path(".ceataec") / "dotnet" / version
    previous = {}
    if upgrade_from is not None:
        if not re.fullmatch(r"\d+\.\d+\.\d+", upgrade_from) or upgrade_from == version:
            raise ValueError("upgrade-from must identify a different installed semantic version")
        previous_vendor = Path(".ceataec") / "dotnet" / upgrade_from
        manifest_path = target / previous_vendor / "attachment.json"
        if any(p.is_symlink() for p in [manifest_path, *manifest_path.parents] if p != target):
            raise ValueError("previous attachment path must not use symlinks")
        installed = json.loads(manifest_path.read_text(encoding="utf-8"))
        if installed.get("harness") != harness or installed.get("standardVersion") != upgrade_from:
            raise ValueError("previous attachment does not match version/harness")
        previous = installed["files"]
        for relative, expected in previous.items():
            path = Path(relative)
            wrapper_root = Path(HARNESS_DIR[harness]) / "skills"
            if path.is_absolute() or ".." in path.parts or not (
                path.is_relative_to(previous_vendor) or
                (path.is_relative_to(wrapper_root) and path.name == "SKILL.md"
                 and path.parent.name.startswith("ceataec-"))
            ):
                raise ValueError("invalid path in previous attachment manifest")
            actual = target / path
            if any(p.is_symlink() for p in [actual, *actual.parents] if p != target):
                raise ValueError(f"previous attachment contains a symlink: {relative}")
            if not actual.is_file() or hashlib.sha256(actual.read_bytes()).hexdigest() != expected:
                raise ValueError(f"locally modified or missing attachment file: {relative}")
    files = {}
    for source in sorted(plugin.rglob("*")):
        if "__pycache__" in source.parts or source.suffix not in {".md", ".json", ".py"}:
            continue
        if source.is_symlink():
            raise ValueError(f"plugin symlink not supported: {source}")
        if source.is_file():
            files[vendor / source.relative_to(plugin)] = source.read_bytes()
    for skill in sorted((plugin / "skills").iterdir()):
        if not (skill / "SKILL.md").is_file():
            continue
        name = f"ceataec-{skill.name}"
        # Cursor also discovers Claude and .agents skills. Avoid duplicate workflows.
        for directory in HARNESS_DIR.values():
            if directory != HARNESS_DIR[harness] and (target / directory / "skills" / name).exists():
                raise ValueError(f"{name} already exists in {directory}; use one attachment mode")
        destination = Path(HARNESS_DIR[harness]) / "skills" / name / "SKILL.md"
        description = (skill / "SKILL.md").read_text(encoding="utf-8").split("description: ", 1)[1].splitlines()[0]
        reference = (Path("../../..") / vendor / "skills" / skill.name / "SKILL.md").as_posix()
        files[destination] = (
            f"---\nname: {name}\ndescription: {description}\n---\n\n"
            f"Read and follow [the bundled {skill.name} workflow]({reference}).\n"
            "Resolve its links relative to that file. The current repository is the target; "
            "the bundled reference version is the baseline. Installation does not authorize "
            "refactoring. REVIEW-SCORE remains feedback only.\n"
        ).encode()
    manifest = {"standardVersion": version, "harness": harness,
                "referenceCommit": catalog["referenceCommit"],
                "files": {p.as_posix(): hashlib.sha256(data).hexdigest() for p, data in files.items()}}
    files[vendor / "attachment.json"] = (json.dumps(manifest, indent=2) + "\n").encode()
    # Preflight the complete set before writing anything. Never follow target symlinks.
    for relative, data in files.items():
        path = target / relative
        for candidate in [path, *path.parents]:
            if candidate == target:
                break
            if candidate.is_symlink():
                raise ValueError(f"refusing symlink: {candidate}")
            if candidate != path and candidate.exists() and not candidate.is_dir():
                raise ValueError(f"parent is not a directory: {candidate}")
        if path.exists() and (not path.is_file() or path.read_bytes() != data):
            expected = previous.get(relative.as_posix())
            if not path.is_file() or expected != hashlib.sha256(path.read_bytes()).hexdigest():
                raise ValueError(f"would overwrite existing content: {path}")
    return files


def attach(target, harness, dry_run=False, upgrade_from=None):
    target = Path(target).resolve()
    files = plan_attachment(target, harness, upgrade_from=upgrade_from)
    changed = []
    for relative, data in files.items():
        path = target / relative
        if path.is_file() and path.read_bytes() == data:
            continue
        changed.append(relative.as_posix())
        if not dry_run:
            path.parent.mkdir(parents=True, exist_ok=True)
            if path.exists():
                with tempfile.NamedTemporaryFile(dir=path.parent, delete=False) as stream:
                    temporary = Path(stream.name)
                    stream.write(data)
                try:
                    os.replace(temporary, path)
                finally:
                    temporary.unlink(missing_ok=True)
            else:
                with path.open("xb") as stream:
                    stream.write(data)
    return changed


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--target", type=Path, required=True)
    parser.add_argument("--harness", choices=HARNESS_DIR, required=True)
    parser.add_argument("--dry-run", action="store_true")
    parser.add_argument("--upgrade-from", help="Replace unmodified entry points from this installed version")
    args = parser.parse_args()
    try:
        changed = attach(args.target, args.harness, args.dry_run, args.upgrade_from)
    except (ValueError, OSError) as error:
        parser.exit(2, f"Attachment stopped: {error}\n")
    print(json.dumps({"dryRun": args.dry_run, "changedFiles": changed}, indent=2))


if __name__ == "__main__":
    main()

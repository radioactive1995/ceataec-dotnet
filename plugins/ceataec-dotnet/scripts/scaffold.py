#!/usr/bin/env python3
"""Generate a named teaching starter from the pinned Git reference; no code execution."""
import argparse
import io
import json
from pathlib import Path, PurePosixPath
import re
import shutil
import subprocess
import tarfile
import uuid

PLUGIN = Path(__file__).resolve().parents[1]
OLD = "Ceataec.ExampleService"
NAME = re.compile(r"[A-Z][A-Za-z0-9]*(?:\.[A-Z][A-Za-z0-9]*)*")


def render(template, name, aspire):
    if not NAME.fullmatch(name) or len(name) > 100:
        raise ValueError("name must be PascalCase segments, at most 100 characters (e.g. Ceataec.Orders)")
    if any(part.upper() in {"CON", "PRN", "AUX", "NUL", *[f"COM{i}" for i in range(1, 10)],
                           *[f"LPT{i}" for i in range(1, 10)]} for part in name.split(".")):
        raise ValueError("name contains a Windows reserved device name")
    if aspire not in {"yes", "no"}:
        raise ValueError("aspire must be yes or no")
    template = Path(template).resolve()
    rules = json.loads((PLUGIN / "spec/rules.json").read_text(encoding="utf-8"))
    commit = rules["referenceCommit"]
    archive = subprocess.run(
        ["git", "-C", str(template), "archive", "--format=tar", commit, "--",
         "src", "tests", f"{OLD}.sln", ".editorconfig", ".gitignore"],
        check=True, capture_output=True,
    ).stdout
    files = {}
    database = name.replace(".", "_").lower()
    with tarfile.open(fileobj=io.BytesIO(archive)) as source:
        for member in source.getmembers():
            if member.isdir():
                continue
            path = PurePosixPath(member.name)
            if not member.isfile() or path.is_absolute() or ".." in path.parts:
                raise ValueError(f"unsupported reference member: {member.name}")
            if "/Properties/Properties/" in member.name:
                continue  # Known duplicate launch-settings path in the teaching reference.
            if aspire == "no" and f"{OLD}.AppHost" in path.parts:
                continue
            text = source.extractfile(member).read().decode("utf-8-sig")
            text = text.replace("\r\n", "\n")
            text = text.replace(OLD, name).replace("Ceataec_ExampleService", name.replace(".", "_"))
            text = text.replace("Ceataec Example Service", name)
            text = text.replace("ceataec_example", database)
            if path.suffix == ".csproj":
                text = re.sub(r"<UserSecretsId>[^<]+</UserSecretsId>",
                              f"<UserSecretsId>{uuid.uuid4()}</UserSecretsId>", text)
            if path.name in {"appsettings.json", "appsettings.Development.json"} and f"{OLD}.Api" in path.parts:
                settings = json.loads(text)
                if "Database" in settings:
                    settings["Database"]["ConnectionString"] = ""
                text = json.dumps(settings, indent=2) + "\n"
            if path.name == "AppHost.cs":
                text = text.replace('AddDatabase("ceataec")', f'AddDatabase("ceataec", "{database}")')
            if aspire == "no":
                if path.name == "Program.cs":
                    text = text.replace("builder.EnrichNpgsqlDbContext<AppDbContext>();\n", "")
                if path.name == f"{OLD}.Api.csproj":
                    text = re.sub(r'^.*<PackageReference Include="Aspire\.[^\n]+\n', "", text, flags=re.M)
                if path.name == f"{OLD}.ServiceDefaults.csproj":
                    text = text.replace("    <IsAspireSharedProject>true</IsAspireSharedProject>\n", "")
                if path.suffix == ".sln":
                    pattern = r'^Project\([^\n]+ = "' + re.escape(name) + r'\.AppHost",[^\n]+, "(\{[^}]+\})"\nEndProject\n'
                    match = re.search(pattern, text, re.M)
                    if not match:
                        raise ValueError("reference AppHost solution entry changed; update generator")
                    project_id = match.group(1)
                    text = text[:match.start()] + text[match.end():]
                    text = "\n".join(line for line in text.split("\n") if project_id not in line)
            files[member.name.replace(OLD, name)] = text.encode()
    if f"{name}.sln" not in files:
        raise ValueError("reference does not contain the expected solution")
    metadata = {"standardVersion": rules["version"], "profile": rules["profile"],
                "referenceRepository": rules["referenceRepository"], "referenceCommit": commit,
                "name": name, "aspire": aspire == "yes", "examples": True, "exceptions": []}
    files[".ceataec-template.json"] = (json.dumps(metadata, indent=2) + "\n").encode()
    run = f"dotnet run --project src/{name}.{'AppHost' if aspire == 'yes' else 'Api'}"
    files["README.md"] = f"""# {name}

Teaching starter generated from CEATAEC standard {rules['version']} (proposed).
Reference commit: `{commit}`. Aspire local orchestration: **{aspire}**.
Vessels, Voyages and Certificates are retained examples, including migrations/tests.
Replace them deliberately with your domain before treating this as a business service.

Requires .NET 10 SDK and Docker for integration tests{' and AppHost' if aspire == 'yes' else ''}.

```bash
dotnet restore {name}.sln
dotnet build {name}.sln --no-restore
dotnet test {name}.sln --no-build
{run}
```

Without AppHost, provide a local Postgres 18 database via `Database__ConnectionString`
or `ConnectionStrings__ceataec`. Committed connection-string defaults are empty.
Aspire supplies `ConnectionStrings__ceataec` when enabled. Keep credentials in your
local environment/user secrets. Integration tests start their own Postgres container;
the generated test factory overrides both connection-string keys to isolate test data.

ServiceDefaults health/telemetry/resilience remain enabled in both variants. No-AppHost
omits Aspire's Npgsql enrichment/retries/database health registration; implement the
production readiness checks your service needs. Health routes in this sample are Development-only.

Generation does not run .NET verification. Resolve restore/build/test failures before
adoption. Implement real authorization (the sample allows anonymous access), production
probe/schema deployment decisions and company provisioning before shipping.
See `.ceataec-template.json` for baseline provenance. Use the reference repository's
AI attachment tooling if you want the adoption and verification workflows in this service.
""".encode()
    return files, metadata


def scaffold(template, name, output, aspire="yes", dry_run=False):
    raw_output = Path(output).absolute()
    if raw_output.is_symlink():
        raise ValueError("output must not be a symlink")
    output = raw_output.resolve()
    if output.exists():
        raise ValueError("output must be a new directory; existing files are never overwritten")
    template = Path(template).resolve()
    if output == template or template in output.parents:
        raise ValueError("output must be outside the reference checkout")
    if not output.parent.is_dir():
        raise ValueError("output parent directory must already exist")
    files, metadata = render(template, name, aspire)
    if not dry_run:
        output.mkdir()  # Exclusive creation; re-checks races with existing directories.
        try:
            for relative, data in files.items():
                path = output / relative
                path.parent.mkdir(parents=True, exist_ok=True)
                path.write_bytes(data)
        except Exception:
            shutil.rmtree(output)  # Only the directory this invocation just created.
            raise
    return {"dryRun": dry_run, "output": str(output), "fileCount": len(files), **metadata}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--template", type=Path, required=True)
    parser.add_argument("--name", required=True)
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--aspire", choices=("yes", "no"), default="yes")
    parser.add_argument("--dry-run", action="store_true")
    args = parser.parse_args()
    try:
        result = scaffold(args.template, args.name, args.output, args.aspire, args.dry_run)
    except (ValueError, OSError, subprocess.CalledProcessError) as error:
        parser.exit(2, f"Scaffold stopped: {error}\n")
    print(json.dumps(result, indent=2))


if __name__ == "__main__":
    main()

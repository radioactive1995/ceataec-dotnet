#!/usr/bin/env python3
"""Attach a versioned plugin snapshot and skill entry points without overwriting files."""
import argparse
import hashlib
import json
from pathlib import Path
import sys

PLUGIN = Path(__file__).resolve().parents[1]
HARNESS_DIR = {"claude": ".claude", "cursor": ".cursor", "codex": ".agents"}


def plan_attachment(target, harness, plugin=PLUGIN):
    target = Path(target).resolve()
    if not target.is_dir():
        raise ValueError("target must be an existing repository directory")
    catalog = json.loads((plugin / "spec/rules.json").read_text(encoding="utf-8"))
    version = catalog["version"]
    vendor = Path(".ceataec") / "dotnet" / version
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
            raise ValueError(f"would overwrite existing content: {path}")
    return files


def attach(target, harness, dry_run=False):
    target = Path(target).resolve()
    files = plan_attachment(target, harness)
    changed = []
    for relative, data in files.items():
        path = target / relative
        if path.exists():
            continue
        changed.append(relative.as_posix())
        if not dry_run:
            path.parent.mkdir(parents=True, exist_ok=True)
            with path.open("xb") as stream:
                stream.write(data)
    return changed


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--target", type=Path, required=True)
    parser.add_argument("--harness", choices=HARNESS_DIR, required=True)
    parser.add_argument("--dry-run", action="store_true")
    args = parser.parse_args()
    try:
        changed = attach(args.target, args.harness, args.dry_run)
    except (ValueError, OSError) as error:
        parser.exit(2, f"Attachment stopped: {error}\n")
    print(json.dumps({"dryRun": args.dry_run, "newFiles": changed}, indent=2))


if __name__ == "__main__":
    main()

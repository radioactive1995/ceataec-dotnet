#!/usr/bin/env python3
"""Explicit restore/build/test runner. --plan performs no target code execution."""
import argparse
import json
from pathlib import Path
import re
import shutil
import subprocess
import sys
import xml.etree.ElementTree as ET


def commands(target, solution, scope="all"):
    target = Path(target).resolve()
    solution_path = (target / solution).resolve()
    if not solution_path.is_relative_to(target) or not solution_path.is_file():
        raise ValueError("solution must be an existing file inside target")
    relative = solution_path.relative_to(target).as_posix()
    if solution_path.suffix != ".sln":
        raise ValueError("this runner currently supports .sln solutions")
    projects = re.findall(r'"([^"\n]+\.csproj)"', solution_path.read_text(encoding="utf-8-sig"))
    checks = [
        {"name": "restore", "command": ["dotnet", "restore", relative]},
        {"name": "build", "command": ["dotnet", "build", relative, "--no-restore"]},
    ]
    for relative_project in projects:
        project = (solution_path.parent / relative_project.replace("\\", "/")).resolve()
        if not project.is_relative_to(target) or not project.is_file():
            raise ValueError(f"project reference outside target or missing: {relative_project}")
        xml = ET.parse(project)
        if not any(node.get("Include") == "Microsoft.NET.Test.Sdk" for node in xml.iter("PackageReference")):
            continue
        integration = "IntegrationTests" in project.stem
        checks.append({"name": project.stem, "integration": integration,
                       "selected": scope == "all" or not integration,
                       "command": ["dotnet", "test", project.relative_to(target).as_posix(), "--no-build"]})
    if len(checks) == 2:
        raise ValueError("no test projects found; cannot report verification success")
    return checks


def run_checks(target, solution, scope="all", runner=subprocess.run, which=shutil.which, timeout=600):
    checks = commands(target, solution, scope)
    results = []
    build_ready = True
    sdk = which("dotnet")
    docker_ready = None
    for check in checks:
        item = {**check, "status": "not-run"}
        if not check.get("selected", True):
            item.update(status="skipped", reason="Excluded by explicit unit scope")
        elif not sdk:
            item.update(status="blocked", reason="dotnet executable is unavailable")
        elif not build_ready:
            item.update(status="blocked", reason="Restore or build did not succeed")
        else:
            if check.get("integration") and docker_ready is None:
                docker = which("docker")
                try:
                    docker_ready = bool(docker) and runner(
                        [docker, "info"], cwd=target, stdout=subprocess.DEVNULL,
                        stderr=subprocess.DEVNULL, timeout=30, check=False).returncode == 0
                except (OSError, subprocess.TimeoutExpired):
                    docker_ready = False
            if check.get("integration") and not docker_ready:
                item.update(status="blocked", reason="Docker is unavailable or its daemon is unreachable")
            else:
                try:
                    result = runner(check["command"], cwd=target, stdout=sys.stderr,
                                    stderr=sys.stderr, timeout=timeout, check=False)
                    item.update(status="passed" if result.returncode == 0 else "failed",
                                exitCode=result.returncode)
                except subprocess.TimeoutExpired:
                    item.update(status="failed", reason=f"Command exceeded {timeout} seconds")
                except OSError as error:
                    item.update(status="blocked", reason=str(error))
        results.append(item)
        if check["name"] in {"restore", "build"} and item["status"] != "passed":
            build_ready = False
    selected = [item for item in results if item.get("selected", True)]
    return {"target": str(Path(target).resolve()), "solution": solution, "scope": scope,
            "selectedChecksPassed": all(item["status"] == "passed" for item in selected),
            "allChecksPassed": all(item["status"] == "passed" for item in results),
            "checks": results}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--target", type=Path, required=True)
    parser.add_argument("--solution", required=True)
    parser.add_argument("--scope", choices=("all", "unit"), default="all")
    parser.add_argument("--plan", action="store_true")
    args = parser.parse_args()
    try:
        if args.plan:
            result = {"planOnly": True, "checks": commands(args.target, args.solution, args.scope)}
        else:
            result = run_checks(args.target, args.solution, args.scope)
    except (ValueError, OSError, ET.ParseError) as error:
        parser.exit(2, f"Verification stopped: {error}\n")
    print(json.dumps(result, indent=2))
    return 0 if args.plan or result["selectedChecksPassed"] else 2


if __name__ == "__main__":
    sys.exit(main())

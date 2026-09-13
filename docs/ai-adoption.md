# Three simple workflows

This kit is **version 0.5.0, proposed**. It contains three skills, one shared baseline
document and thin Claude/Cursor plugin manifests. Optional shell installers make those same three skills available locally.
No Python, custom generator, scoring engine or separate verification skill is required.

| Workflow | Purpose | Result |
| --- | --- | --- |
| REVIEW-SCORE | Compare with the actual skeleton | Feedback in chat; five areas scored 0–2, total out of 10 when all are assessed |
| REFACTOR | Align an existing project in small steps | Scoped source changes preserving its behavior and contracts |
| SCAFFOLD | Follow the skeleton for a new project | New skeleton and run instructions; Aspire optional |

Read [the baseline](../plugins/ceataec-dotnet/spec/standard.md). The skeleton's code
defines project boundaries, structure, libraries and patterns. Each workflow inspects
a selected source revision and records its full commit. Documentation explains the
baseline; conflicts with code and reference defects are reported explicitly.

Adapt the target's own domain, business features and relationships. Omitted teaching
features and optional Aspire are not gaps. A different architecture or library may be
reasonable but is still a deviation to explain. Refactoring preserves existing contracts
and records exceptions that remain; scaffolding follows the baseline from the start.

## Install locally

Run one of these from this checkout, without administrator privileges:

```bash
# macOS/Linux (Bash): choose cursor, claude, or both
bash scripts/install-skills.sh both
```

```powershell
# Windows PowerShell 5.1, or PowerShell 7 on Windows/macOS/Linux
./scripts/install-skills.ps1 -Harness both
```

| Choice | Personal skill location |
| --- | --- |
| `cursor` | `~/.cursor/skills/ceataec-{review-score,refactor,scaffold}/` |
| `claude` | `~/.claude/skills/ceataec-{review-score,refactor,scaffold}/` |
| `both` | Same Claude location; Cursor also discovers it, so there is one copy |

These are personal **skill installations**, available across local projects. They
install the plugin's three skills without changing plugin settings or requiring a
marketplace, harness CLI, symlinks or network access. Each skill gets its own bundled
principles file with an adapted relative link, so moving the checkout does not break it.
All three workflows still need access to the actual skeleton source, through a local
reference checkout or authorized read-only repository tools. Installed guidance is not
a substitute for inspecting code. Supply a revision/checkout or resolve main once to
a full commit; don't change baselines during a task. Without reference evidence, review
can describe observations but must withhold the alignment total.

Use `--dry-run` (Bash) or `-DryRun` (PowerShell) to preview. `--user-home DIR` / `-UserHome DIR`
selects another existing user directory for testing. Identical installations are a no-op;
conflicting content or links stop installation before files are copied. To update,
inspect/back up local edits, remove only the three installed `ceataec-*` skill folders,
then rerun. Uninstall by removing those same folders. Don't remove the whole harness
skills directory. Switching from Cursor-only to both requires removing the old
Cursor copies first. Avoid loading a native plugin alongside these personal skills.

Restart the client, then select `/ceataec-review-score`, `/ceataec-refactor` or
`/ceataec-scaffold`. Confirm the three skills appear in the client's skill picker.
Personal skills are local; this does not install them on remote/cloud agents.
See the supported locations in [Cursor's skill documentation](https://cursor.com/docs/skills)
and [Claude's skill documentation](https://code.claude.com/docs/en/skills).

## Use without installation

Keep a checkout of this repository outside the target, at a chosen commit. Any harness
that can read files can follow the appropriate entry point:

- `plugins/ceataec-dotnet/skills/review-score/SKILL.md`
- `plugins/ceataec-dotnet/skills/refactor/SKILL.md`
- `plugins/ceataec-dotnet/skills/scaffold/SKILL.md`

For example, tell Cursor, Claude Code or Codex:

> Read /path/to/ceataec-dotnet/plugins/ceataec-dotnet/skills/review-score/SKILL.md.
> Review /path/to/my-service against the actual skeleton in /path/to/ceataec-dotnet.
> Record its commit, assess template alignment, preserve our domain, and explain
> deliberate technology differences separately from bugs. Give feedback only.

The package retains native plugin manifests. With Claude Code you can load it directly:

```bash
claude --plugin-dir /absolute/path/to/ceataec-dotnet/plugins/ceataec-dotnet
```

Then use `/ceataec-dotnet:review-score`, `/ceataec-dotnet:refactor` or
`/ceataec-dotnet:scaffold`. For Cursor use its approved plugin setup or the direct-file
approach above. Keep the whole package together so its relative links resolve. There
is no required installation into a consumer repository. Client discovery and permission
behavior should be checked in the approved client; instructions alone aren't a sandbox.

## Example requests

> REFACTOR our billing service toward the skeleton's structure and implementation
> patterns. Keep our invoices, routes and authorization. Start with one use case,
> run relevant tests and identify any remaining compatibility decisions.

> SCAFFOLD Acme.Booking in ../booking, an HTTP service with Postgres and no Aspire.
> Start without business features. Follow the reference's project wiring, libraries,
> persistence, error handling and test approach.

Build/test execution is part of refactoring or scaffolding, using normal project
commands. Reviews inspect existing evidence and never run builds/tests or write files.
Unknown review areas are shown without a total; there are no weighted formulas or JSON
assessment files. Scores measure template alignment; correctness, security risks and
code smells appear separately. Version 0.5.0 tightens the meaning of the /10 score:
reassess earlier reviews against a selected skeleton commit before comparing scores.

For example: a different business domain following the skeleton can fully align;
a well-structured controller/repository alternative still has alignment gaps; omitting
Aspire loses no points; unavailable reference source means no conformance total.

## Moving from the earlier kit

The extra skills, Python helpers, reviewer agent and versioned snapshot manager are retired.
The new installers only copy three self-contained personal skills; they do not manage
old project attachments or silently upgrade them.
If a consumer has an older attachment, it will keep using those copied instructions
until explicitly updated. Inspect its managed `ceataec-*` skill directories and
`.ceataec/dotnet/<version>` snapshots; preserve local edits and remove only the old
kit-owned files as a separate setup change. Then use either the local installer or the external checkout approach.
Updating this repository does not rewrite consumer repositories.

This repository's CI checks the installers in temporary user directories on Linux and
Windows, and restores, builds and tests the runnable .NET example directly.
It does not claim to validate arbitrary AI-generated projects; each scaffold must run
its own checks. The removed Python tests covered the removed tools, not application
behavior. The C# unit, architecture and integration tests remain.

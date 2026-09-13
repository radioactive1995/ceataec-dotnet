# Three simple workflows

This kit is **version 0.4.1, proposed**. It contains three skills, one shared principles
document and thin Claude/Cursor plugin manifests. Optional shell installers make those same three skills available locally.
No Python, custom generator, scoring engine or separate verification skill is required.

| Workflow | Purpose | Result |
| --- | --- | --- |
| REVIEW-SCORE | Understand an existing project and its gaps | Feedback in chat; five areas scored 0–2, total out of 10 when all are assessed |
| REFACTOR | Improve an existing project in small steps | Scoped source changes preserving its behavior and contracts |
| SCAFFOLD | Start a project suited to its own purpose | New skeleton and run instructions; Aspire optional |

Read [the principles](../plugins/ceataec-dotnet/spec/standard.md). Domain entities,
features and implementation choices in the example service illustrate the ideas;
they are not a checklist for another project. Scaffolding starts with the requested
purpose and leaves out teaching features unless requested.

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
Scaffolding still needs a reference checkout for C# source.

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
> Review /path/to/my-service. Compare the design ideas, respecting our domain and
> existing technology choices. Give feedback only.

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

> REFACTOR our billing service to make business rules and database access easier to
> change independently. Keep our existing invoices, routes and authorization. Start
> with one use case and run the relevant tests.

> SCAFFOLD Acme.Booking in ../booking, an HTTP service with Postgres and no Aspire.
> Start without business features. Use the reference for boundaries and project wiring.

Build/test execution is part of refactoring or scaffolding, using normal project
commands. Reviews inspect existing evidence and never run builds/tests or write files.
Unknown review areas are shown without a total; there are no weighted formulas or JSON
assessment files. Scores are discussion aids and are not comparable to versions 0.1–0.3.

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

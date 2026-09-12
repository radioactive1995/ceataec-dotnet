# AI adoption workflows

This is an initial **proposed** implementation, version 0.2.0. It supplies three adoption
workflows plus explicit verification and one shared standard. It does not declare company policy approved.
The template code reference is pinned in the plugin's `spec/rules.json`.

| Mode | Result | Target writes |
| --- | --- | --- |
| REVIEW-SCORE | Evidence, scores/coverage, strengths, gaps, code smells and prioritized actions in chat | None, including no build/test/restore or report files |
| REFACTOR | Incremental source changes preserving contracts/data, validation and migration record | Scoped edits when requested; no automatic commit/push/deploy |
| SCAFFOLD | Newly named starter, optional AppHost, tests and provenance | New destination only |
| VERIFY | Restore/build/unit/architecture/integration execution evidence | Build and test outputs |

## Design

`plugins/ceataec-dotnet/spec/` owns the rules, scoring contract and workflow choices.
The four `skills/*/SKILL.md` files own execution behavior and use relative links
inside that package. Claude and Cursor manifests wrap the same payload. `AGENTS.md`
and `CLAUDE.md` route work in this repository. The optional Claude reviewer agent
has only read/search tools; role separation does not require multiple agents.

Plain instructions are not a universal tool-permission system. For enforced review
isolation, use the harness's read-only tool policy/filesystem mount; in Claude the
bundled reviewer subagent excludes command/edit tools. Invoking the normal review
skill states the contract but does not sandbox the entire parent session. Hooks
are intentionally unnecessary for these workflows; no startup command silently
installs dependencies, changes source or runs target scripts.

## Use from another repository

Keep a checkout of this repository at the desired **plugin release commit**, outside
the consumer. Pin the full commit instead of tracking main for company rollout.
The plugin's separately pinned **C# reference commit** may be older than its own
release commit; this is intentional. Review/refactor use the bundled standard;
scaffolding additionally reads C# content from that reference commit in a local checkout.

### Claude Code: plugin with no target files

Launch Claude in the target repository with the plugin directory supplied:

```bash
claude --plugin-dir /absolute/path/to/ceataec-dotnet/plugins/ceataec-dotnet
```

Then invoke `/ceataec-dotnet:review-score`, `/ceataec-dotnet:refactor` or
`/ceataec-dotnet:scaffold`; `/ceataec-dotnet:verify` executes checks, supplying the target or new service choices.
No target installation is needed for feedback-only reviews.

### Cursor: plugin or project skills

For local plugin use, copy the complete `plugins/ceataec-dotnet` folder to
`~/.cursor/plugins/local/ceataec-dotnet`, reload Cursor and verify its four skills
in Customize. This depends on the organization's local-plugin-import setting.
Select the relevant skill from `/`. Use a company marketplace for managed rollout
after the acceptance checks below; this PR does not publish or install company-wide.

### Attach versioned skills to a consumer

If you want checked-in project skills, the attachment helper prepares a local
snapshot with linked entry points. This **writes setup files**, so run it as setup,
before a review, not inside REVIEW-SCORE. It does not edit existing AGENTS/CLAUDE
instructions. Python 3.10+ is sufficient; no pip packages are required.

```bash
python3 /absolute/path/to/ceataec-dotnet/plugins/ceataec-dotnet/scripts/attach.py \
  --target /absolute/path/to/consumer --harness claude --dry-run
python3 /absolute/path/to/ceataec-dotnet/plugins/ceataec-dotnet/scripts/attach.py \
  --target /absolute/path/to/consumer --harness claude
```

| Selection | Entry point directory | Invocation |
| --- | --- | --- |
| `claude` | `.claude/skills/ceataec-{mode}/` | `/ceataec-review-score`, `/ceataec-refactor`, `/ceataec-scaffold` |
| `cursor` | `.cursor/skills/ceataec-{mode}/` | Select the corresponding skill from `/` |
| `codex` | `.agents/skills/ceataec-{mode}/` | `$ceataec-review-score`, `$ceataec-refactor`, `$ceataec-scaffold` |

Cursor also reads Claude/.agents skill directories, so use one attachment mode
per consumer and avoid installing the native plugin and project skills together.
For a harness that lacks skill discovery, explicitly ask it to read the corresponding
bundled `SKILL.md`; the workflow itself does not depend on slash-command syntax.

The helper refuses conflicting files or target symlinks and is idempotent when
contents match. Review its diff before committing setup. The snapshot manifest
records file hashes and version. For an explicit update from an installed 0.1.0:

```bash
python3 /absolute/path/to/ceataec-dotnet/plugins/ceataec-dotnet/scripts/attach.py \
  --target /absolute/path/to/consumer --harness claude --upgrade-from 0.1.0 --dry-run
```

Remove `--dry-run` to apply. Every old managed file must match its recorded hash;
locally edited instructions stop the upgrade before any writes. The old snapshot
is retained and the managed entry points move to 0.2.0. Keep service-specific
exceptions outside the managed package. No automatic update daemon is installed.

## Example requests

> REVIEW-SCORE this service against the bundled CEATAEC standard. Give evidence,
> score and coverage, distinguish correctness defects from template differences,
> and return feedback only.

> REFACTOR this service toward CEATAEC 0.2.0. Start with the Orders create/read
> slices. Preserve existing routes, response/error shapes, authorization and
> database schema. Run relevant checks and leave a reviewable diff.

> SCAFFOLD Ceataec.Orders at ../orders with Aspire local orchestration. Retain the
> examples initially and explain how to run it and replace them with our domain.

## Generate directly

From the reference checkout, with the output parent already present:

```bash
python3 plugins/ceataec-dotnet/scripts/scaffold.py \
  --template . --name Ceataec.Orders --output ../orders --aspire yes --dry-run
python3 plugins/ceataec-dotnet/scripts/scaffold.py \
  --template . --name Ceataec.Orders --output ../orders --aspire yes
```

Use `--aspire no` for a standalone Api and external local Postgres 18. Both variants
retain ServiceDefaults; no-AppHost also removes the Api's Aspire Npgsql enrichment
and associated database retries/health registration. It does not remove telemetry.
The generated README explains run/configuration requirements. The generator
renames paths, namespaces, solution references and Aspire project identifiers,
assigns fresh user-secrets identity, clears connection defaults, and inherits the hardened reference test
factory that overrides both connection keys. It reads committed source, ignoring dirty
or untracked reference files. No existing output directory is accepted.

This first release retains the teaching domain and its tests. Empty/business-specific
generation is an agent migration task, not a boolean that leaves broken architecture
anchors or migrations. Database selection, identity-provider selection and native
`dotnet new` packaging are intentionally not advertised as implemented options.

## Validate and release

Use VERIFY independently or after refactoring/scaffolding:

```bash
python3 plugins/ceataec-dotnet/scripts/verify.py \
  --target . --solution Ceataec.ExampleService.sln --plan
python3 plugins/ceataec-dotnet/scripts/verify.py \
  --target . --solution Ceataec.ExampleService.sln --scope all
```

`--scope unit` explicitly omits integration tests. The runner never reports the
whole suite as passed when checks were skipped or blocked. Test output must still
be inspected for actual executed counts and skipped tests. REVIEW-SCORE does not
call this runner. The GitHub template-validation workflow exercises the reference
and both generated variants using .NET 10 and Docker. It is a repository quality
check; it is not copied into generated services or used for production deployment.


```bash
python3 -m unittest discover -s tests/ai -v
```

Tool tests cover weighted/unknown/N/A scoring, conflict-safe attachment and both
scaffold shapes. They validate project references and transformations, not C# compilation.

Before marking a release company-ready, generate both Aspire variants into new
directories; run .NET restore, build, all unit/architecture tests and Docker-backed
integration tests in each. Smoke-run Api/AppHost and check health/versioned routes.
Validate discovery and a representative REVIEW-SCORE/REFACTOR/SCAFFOLD request in
the actual approved Cursor and Claude versions. Review must leave the target's
files and Git state unchanged, including ignored/untracked files. Test refactoring
on a disposable service with fixed API/DB contracts and verify them afterward.

The authoring environment cannot start CoreCLR (HRESULT 0x8007000E), even after
downloading a .NET 10 SDK, and has no Docker or Cursor/Claude runtimes. Local C#
execution and live harness checks are therefore blocked. Consult the PR validation
workflow for hosted execution evidence; do not infer success from the workflow file.

## Decisions for the company pilot

Recommended starting scope: HTTP APIs with Postgres, optional local Aspire, preserved
contracts for migrations, and report-only scoring. Pilot on one mature service and
one new service before adopting a merge-blocking score threshold. Keep serious
defects visible regardless of the numerical score. Assign a maintainer for standards
versions/exceptions; add worker or other database profiles only with real examples/tests.

See [the template review](template-review.md) for concrete gaps in the reference.

## Format references

Adapter formats were checked against [Cursor skills](https://cursor.com/docs/skills),
[Cursor plugins](https://cursor.com/docs/plugins),
[Claude plugin reference](https://code.claude.com/docs/en/plugins-reference) and
[Claude skills](https://code.claude.com/docs/en/skills). Harness discovery and permission
features evolve; check the approved client versions during release acceptance.

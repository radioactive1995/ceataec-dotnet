---
name: scaffold
description: Create a newly named .NET service from the CEATAEC template with optional Aspire local orchestration and adoption metadata.
---

# SCAFFOLD

Read [the standard](../../spec/standard.md) and [scaffold choices](../../spec/scaffold-options.md).
Reuse supplied choices. Ask briefly for missing service name/output location and whether
local Aspire orchestration is wanted when the answer matters. State sensible defaults
for other choices. Do not invent business requirements or company identity settings.

This release generates a renamed **teaching starter**, retaining the sample domain
and tests as working examples. Explain that explicitly. If asked for an empty business
service, first generate the starter, then remove/replace sample features, entities,
queries, migrations and their tests as a coherent additional step; adapt architecture
anchors and add real acceptance tests. Never call an unmodified sample an empty service.

Locate a local Git checkout of the reference at the full commit in `spec/rules.json`.
It is separate from the target. If unavailable, acquire the named reference through
the user's authorized repository tools; do not substitute arbitrary files or moving main.
Run the bundled `../../scripts/scaffold.py` with explicit `--template`, `--name`,
`--output` and `--aspire yes|no`. Run `--dry-run` first to surface options/destination.
The script uses committed content at the bundled reference, accepts only a new output
directory, and never executes .NET, installs dependencies, commits or deploys.

Then use [VERIFY](../verify/SKILL.md) for restore/build, unit/architecture tests and
Docker integration tests. Explain failures and missing prerequisites. For no AppHost, configure an
external local Postgres 18 connection; telemetry/health defaults remain. Ask about
auth/production settings only when implementing those decisions, not to generate a demo.
Show the run command, selected choices, exact baseline and remaining production work.

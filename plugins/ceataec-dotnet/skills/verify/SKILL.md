---
name: verify
description: Build and test a CEATAEC .NET service or generated starter, reporting actual execution evidence separately from review scoring.
---

# VERIFY

Use when validation is requested, or as part of an authorized refactor/scaffold.
This mode runs code and writes build/test outputs. Never enter it implicitly from
REVIEW-SCORE. Read [the standard](../../spec/standard.md) and identify the target,
revision and solution; inspect repository build/test instructions before executing.

Use [the verification runner](../../scripts/verify.py) with an explicit `--target`
and `--solution`. `--plan` only lists commands. Default `--scope all` includes real
Postgres/Docker integration tests; `--scope unit` explicitly selects unit and
architecture tests only. Missing tools and unavailable Docker are blocked checks,
never passes. Do not substitute an in-memory database to obtain a green result.

The runner stops dependent work after restore/build failure, continues independent
test projects and returns a nonzero result for failed or blocked selected checks.
An exit code confirms command completion, not adequate test coverage: inspect test
output for discovered/executed counts, skipped tests and meaningful assertions.
Distinguish existing failures from introduced failures where baseline evidence exists.

Report target/revision, scope, commands, exit codes, test totals when available,
blocked/skipped work and next actionable failure. Redact secrets from excerpts.
Do not fix source, update snapshots/packages, commit or deploy unless separately
authorized by the enclosing task. A standalone request to verify authorizes checks.

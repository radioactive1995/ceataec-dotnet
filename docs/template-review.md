# Template adoption goal and review

## Goal

A developer can connect an approved harness to this repository or a versioned
plugin, understand the selected standard, and review, refactor, scaffold or verify
a service without relying on another knowledge store. A review remains feedback
only. Refactoring preserves contracts/data. Generation uses an immutable reference.
Updates preserve local work, and verification distinguishes passed, failed, blocked
and deliberately skipped checks.

## Implemented in version 0.2.0

- One specification and catalog, packaged for Cursor/Claude, with project attachment
  for Claude, Cursor and Codex. Root AGENTS/CLAUDE instructions route template work.
- REVIEW-SCORE, REFACTOR, SCAFFOLD and VERIFY skills. A read-only reviewer agent is
  available for harnesses that enforce its restricted tool set.
- Evidence-aware scoring with unknown/N/A treatment, coverage and separate template
  and readiness subtotals. Scores are proposed decision aids, not release approval.
- Committed-source generation with naming, optional Aspire, safe destination checks,
  fresh user-secrets identity, empty credential defaults and reference provenance.
- Explicit attachment upgrades check all managed-file hashes before writing, preserve
  the previous snapshot and refuse locally edited instructions.
- GitHub validation covers Python tooling, the reference and both generated C# variants.
  No deployment pipeline is generated or deployed by this kit.

## Reference fixes

Initial review examined `a702f800b28a0a9b86d83af5495de8d7552bcc2c`.
Generation now pins the hardened reference `520c04af3d66868e473d39196cd058b96afa39d8`.

| Finding | Change | Evidence added |
| --- | --- | --- |
| Blank preferred database connection could win over a valid fallback while validation passed | Validation and DbContext registration now share `DatabaseSettings.GetEffectiveConnectionString` | Infrastructure tests cover null/empty/whitespace preferred values, precedence and missing effective configuration |
| Integration tests overrode only the fallback connection key | Test factory supplies both keys through the final application configuration and checks the selected connection before its explicit migration | Factory assertion plus isolated Testcontainers setup |
| Endpoint persistence checks only covered concrete AppDbContext | Added a compiled dependency rule against the persistence namespace, including ICommandDbContext | Architecture test alongside the existing concrete-type checks |
| Domain independence did not explicitly forbid EF Core | Added an EF Core dependency prohibition | Architecture test |
| A source string was treated as evidence of no-tracking query behavior | Added a real-engine read test that loads an existing aggregate and checks ChangeTracker remains empty | HTTP-created data plus named query execution; the source-text guard remains supplementary |
| Standalone builds resolved conflicting EF Core relational assembly versions | Infrastructure pins the matching EF Core relational package; VERIFY fails on MSB3277 assembly conflicts | Hosted build matrix covers the reference and both generated variants |
| Duplicate nested launch-settings file | Removed the tracked duplicate | Generated/reference layouts have one Api launch-settings path |

## Remaining acceptance boundaries

The authoring environment cannot start CoreCLR (HRESULT 0x8007000E), even with a
local .NET 10 SDK, and provides no Docker or Cursor/Claude runtimes. Python tooling
checks can run locally. Hosted validation results on the PR are the source of truth
for C# execution; the workflow's existence alone is not a pass. Live harness discovery
and a representative workflow request still need an approved client environment.

The scaffold is a named teaching starter retaining the sample domain and tests.
An empty or business-specific service is a coherent subsequent implementation task.
Only HTTP APIs/Postgres are supported by this initial profile. AppHost is optional;
production auth, probes and schema deployment remain explicit service decisions.
The sample's anonymous endpoints and local credentials are not company requirements.
Existing source-text version/no-tracking guards are supplementary, not semantic proof.

## Pilot and release

Pilot the versioned kit on an existing service and a new starter. Check review leaves
all target files unchanged; check a refactored slice preserves API/DB contracts; run
VERIFY with real integration tests. Calibrate scoring and exceptions with service
owners before using scores as merge gates. Publish a company plugin version only after
hosted checks and approved harness acceptance succeed. Add more profiles or native
`dotnet new` packaging when a concrete consuming team needs them.

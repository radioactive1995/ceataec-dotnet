---
name: refactor
description: Refactor an existing .NET service toward the proposed CEATAEC architecture while preserving behavior, API contracts and data.
---

# REFACTOR

Read [the standard](../../spec/standard.md), [rules](../../spec/rules.json), and
[migration contract](../../spec/refactor-plan.md). Identify target path, baseline
version, current branch/worktree and scope. A request to review is not refactor
authorization. A request to refactor authorizes scoped source edits; proceed with
reversible work without repeatedly asking for permission.

Inspect architecture and record existing behavior, tests, uncommitted work, contracts
and migration history. Reuse a recent review only if target revision and standard
match; otherwise reassess affected areas. Explain the concrete sequence of changes.
Create an isolated branch/worktree where possible, preserving user changes. If a
dirty file conflicts with needed work, isolate the work or ask which version to use.

Use small vertical migrations that compile independently where practical: establish
project boundaries, move one use case, preserve its HTTP adapter, extract domain
behavior and persistence access, then repeat. Establish characterization/contract
tests before changing behavior that lacks coverage. Keep public routes, error shapes,
serialization, id semantics, database schema, auth and domain invariants stable.
Do not replace working code with sample domain code. Do not remove existing FKs,
rewrite applied migrations, downgrade packages or remove auth to satisfy a pattern.

When a breaking API/data/identity change is actually necessary, first finish the safe
preparatory work and present its concrete compatibility/migration decision. Obtain
the missing decision before that change; existing explicit authorization still applies.
Honor accepted exceptions. Never weaken tests merely to improve the score.

Run relevant build, unit, architecture and integration checks with available tools;
integration tests require the target's real engine. Report unavailable dependencies
and pre-existing failures separately. Stop a migration batch when it introduces a
failure; repair or isolate that batch before proceeding. Never claim unrun checks pass.
Report changed files, preserved contracts, validation, remaining rule gaps and rollback.
Do not commit, push, open a PR or deploy unless the user's task also authorizes it.

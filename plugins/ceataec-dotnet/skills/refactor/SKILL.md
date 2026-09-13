---
name: refactor
description: Improve an existing .NET project's structure using the skeleton's architectural principles while preserving its domain, behavior and contracts.
---

# REFACTOR

Read [the principles](../../spec/standard.md). Identify the target, requested scope,
existing behavior and uncommitted work. Explain a short sequence of useful changes;
proceed with authorized scoped edits without requiring a separate review or score.

Keep the target's own domain, features and technology choices. Improve boundaries and
cohesion where they solve a concrete problem; don't mechanically rename folders, add
interfaces or replace working behavior with sample entities. Use an isolated branch
or worktree when useful to preserve user work.

Work in small coherent slices. Preserve public APIs, serialization, authorization,
data, id semantics and applied migration history. Add characterization/regression tests
where behavior is at risk. If a breaking change is needed but not authorized, finish
safe preparation and explain the specific compatibility decision still needed.

Use the target's ordinary restore/build/test commands for affected code. Run integration
checks against its actual database engine when relevant; report missing prerequisites
and pre-existing failures separately. Don't weaken tests or remove constraints to match
the example. Correct failures introduced by the change before moving on.

Finish with what changed, why, checks run and remaining gaps. Keep contracts and rollback
considerations visible when relevant. Commit, push, open a PR or deploy only when the
user's task also authorizes that action.

---
name: refactor
description: Align an existing .NET project with the actual skeleton's structure, libraries and implementation patterns while preserving its domain, behavior and contracts.
---

# REFACTOR

Read [the baseline](../../spec/standard.md) and inspect the selected reference source.
Record the reference commit, target revision, requested scope and uncommitted work.
Identify concrete differences in project boundaries, features, dependencies, command/
query flow, persistence and tests. Explain a short migration sequence and proceed with
scoped edits; a separate review or score is not a prerequisite.

The destination is the skeleton's structure and decisions, applied to the target's own
domain and features. Reuse its patterns rather than substituting an unrelated architecture.
Respect explicit exceptions; do not silently preserve every existing technology choice
and call the result aligned. Use a branch/worktree when useful to protect user work.

Work in small coherent slices. Preserve public APIs, serialization, authorization,
data, id semantics and applied migration history. Add characterization/regression tests
where behavior is at risk. Library or database migrations may need compatibility work:
complete safe preparation, explain the remaining decision when it is not authorized,
and leave the divergence visible. Don't force a migration that breaks existing contracts.
Report reference defects/documentation mismatches instead of copying faulty behavior.

Use the target's ordinary restore/build/test commands for affected code. Run integration
checks against its actual database engine when relevant; report missing prerequisites
and pre-existing failures separately. Don't weaken tests or remove constraints to match
the sample. Correct failures introduced by the change before moving on.

Finish with changes tied to the reference decisions, checks run and remaining deviations/
exceptions. Include compatibility and rollback considerations when relevant. Commit,
push, open a PR or deploy only when the user's task also authorizes that action.

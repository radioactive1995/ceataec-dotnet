# Refactor record

Capture this in the conversation, or in the target's normal design location when
source edits are requested. No extra approval checkpoint is needed for ordinary edits.

1. Target revision/worktree, selected standard/profile and baseline gaps.
2. Behavioral constraints: routes, payloads/errors, auth, ids, persistence schema,
   transactions and migration history; existing tests and missing evidence.
3. Ordered batches, each with scope/files, purpose, compatibility strategy, checks
   and rollback (revert that isolated diff, preserving unrelated work).
4. Exceptions and decisions required for actual breaking changes.
5. Completed batches with executed checks/results and remaining work.

The implementer makes bounded changes. The reviewer assesses evidence and regressions.
The verifier runs relevant checks and records results. One agent may perform these
roles sequentially; separate agents are optional and depend on harness/user permission.
No workflow requires parallel agents or a specific model/provider.

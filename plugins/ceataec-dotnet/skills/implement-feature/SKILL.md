---
name: implement-feature
description: Implement a new or changed HTTP API use case as a CEATAEC vertical slice, including domain behavior, persistence and meaningful tests. Use for feature delivery in an existing service.
---

# IMPLEMENT-FEATURE

Identify the target service and read [the standard](../../spec/standard.md). Load
[conventions](../../spec/coding-conventions.md) and [patterns](../../spec/pattern-guide.md)
only for decisions relevant to the slice. This is feature delivery, not a whole-repo
refactor. Preserve unrelated work and accepted target exceptions.

Establish a short acceptance brief from the request and existing code: actor and
authorization, input/output, success and expected errors, domain invariants, persistence
effects and compatibility. Ask about missing business decisions that change behavior;
infer routine file naming and implementation choices. Don't invent permissions or add
`AllowAnonymous()` merely because the sample uses it. For an unresolved access decision,
complete safe domain/test preparation but leave endpoint exposure pending that decision.

Trace a neighboring slice and the corresponding reference example when available.
Implement the smallest complete path: HTTP adapter and contract, command/query handler,
domain behavior, named read query or aggregate write, and applicable persistence mapping.
Read slices need not acquire write-only files. Preserve shipped routes, response/error
shapes and id semantics. Additive features may reuse an existing API version; version
breaking contracts deliberately. Do not copy sample entities into the target's domain.

For schema changes, preserve applied migration history. Produce a new migration and
explain compatibility/backfill needs; do not apply it to a shared database as part of
coding. External side effects need explicit retry/idempotency decisions when relevant.

Choose tests from the actual risks: domain invariants, HTTP success/errors and access,
database behavior on the real engine, and changed architecture boundaries. Include a
regression or negative case that would catch a plausible incorrect implementation.
Run focused checks, using [VERIFY](../verify/SKILL.md) where its runner fits; report
unavailable checks honestly. Finish with behavior delivered, contract/schema impact,
execution evidence and remaining decisions. Commit/push/PR only if the task authorizes it.

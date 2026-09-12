---
name: trace-flow
description: Explain how a request or business behavior flows through an existing .NET service, with concrete symbols and useful change points. Use for onboarding and code navigation.
---

# TRACE-FLOW

Identify the target and the route, symbol or behavior the developer wants to understand.
Read source and existing evidence only; do not run the app, build, test or change files.
If a route is ambiguous, use routing/version configuration to disambiguate it.

Trace the real path through endpoint dispatch, handler, domain behavior, persistence,
result/error mapping and relevant processors. Inspect DI/registration when it determines
which implementation runs. Separate startup/migration behavior from the request path.
For a data question, start at the query or mapping rather than forcing an HTTP tour.

Explain the path with file/symbol references, the key invariant and an error path.
Use a compact diagram only when branching or multiple boundaries make it clearer.
Identify the smallest change point for the user's likely next task and the tests that
exercise it. Label missing runtime evidence and dynamically selected behavior as unknown.
Do not fabricate calls because they are conventional in the template.

Read [patterns](../../spec/pattern-guide.md) only when explaining why a boundary exists.
Keep onboarding focused; do not turn this into a conformance score or unsolicited rewrite.

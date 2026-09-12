# Local C# conventions

The [standard](standard.md) owns architecture and the [catalog](rules.json) owns scoring.
This guide explains local coding decisions; it adds no scoring criteria. The target's
accepted rules and explicit constraints govern ongoing work. Report divergence when
template alignment is requested rather than silently replacing those rules.

## Formatting authority

Read the target's nearest `.editorconfig` and analyzer/build settings. In this reference,
the root `.editorconfig` requests UTF-8, LF, final newline, trimmed trailing whitespace,
spaces, four-space C# indentation and opening braces on new lines. System usings sort
first; redundant `this.` qualification and accessibility modifier rules are suggestions.
It does not define an exhaustive naming policy, require `var`, or enable warnings as
errors globally. Don't invent those requirements. Keep generated files out of style sweeps.

## Established shapes

| Decision | Reference convention | Practical boundary |
| --- | --- | --- |
| HTTP/CQRS data types | Sealed records, one public type per file; use-case names such as `CreateVesselRequest` | Renaming JSON/public members may change a contract |
| Feature code | `Features/{BC}/{UseCase}/V{n}`; Endpoint translates, Handler orchestrates | Match the target namespace declaration; the Api project's namespace omits `.Api` in this sample |
| Domain mutation | Encapsulated state, factories/behavior returning `ErrorOr<T>` | Request validation complements domain invariants; EF materialization constructors are not public factories |
| Expected failures | Domain/handler errors flow through centralized ProblemDetails mapping | Preserve stable error codes/statuses; don't catch every exception and return validation errors |
| Async work | Pass request cancellation through dispatch, query and save operations | Don't add blocking `.Result`/`.Wait()` or discard cancellation to hide a failure |
| Logging | Structured message templates, deliberate fields | Sample handlers log identifiers; do not copy those fields without the target's data-handling decision |

## Preferences that need a reason

Primary constructors, expression bodies and collection expressions appear in the sample;
they are not a mandate to rewrite equivalent working code. Introduce an interface for
a meaningful seam, not automatically for every class. Reuse domain constants where
they express the same invariant; two similar numbers need not represent one concept.
Comments should explain intent or constraints that the code cannot show. Keep a style
fix distinct from changes to validation, null handling, public names or side effects.

For a convention check, show the affected symbol, the governing rule and the smallest
fix. In explanations, one before/after snippet is usually enough. Don't make a style
score or add a new analyzer dependency just to enforce an optional preference.

# Skeleton baseline and adoption principles

Version **0.5.0 — proposed**. The runnable .NET skeleton is the source of truth for
structure and implementation decisions. All three workflows use that same baseline;
this document explains how to apply it to another project's own domain.

## Establish the reference

Use [radioactive1995/ceataec-dotnet](https://github.com/radioactive1995/ceataec-dotnet).
Use the user's selected revision or reference checkout's committed HEAD. Otherwise
resolve main to a full commit once through authorized read-only repository access.
Record that commit separately from the target revision and keep it fixed for the task.
Inspect the reference code, project files and tests; bundled instructions alone are
not evidence of the current implementation. Installed skills still need source access.

If the source is unavailable, ask for a reference checkout/revision or authorized
read-only access. Identify what cannot be assessed; do not invent baseline details or
produce a conformance total. If code and documentation disagree, report the mismatch
with both locations. A defect in the reference is a finding to resolve, not something
to copy or silently turn into a requirement.

## Follow these decisions

For HTTP services, follow the skeleton's project boundaries, feature layout, libraries
and patterns. Inspect the selected revision; these are the current starting points:

| Decision | Reference evidence (paths relative to the reference checkout) |
| --- | --- |
| Api, Domain, Infrastructure and ServiceDefaults responsibilities; dependency direction; framework and library choices | `src/*/*.csproj`, `tests/Ceataec.ExampleService.ArchitectureTests/LayerTests.cs` |
| Versioned vertical slices, Endpoint dispatch through FastEndpoints commands/queries, typed results and sealed record contracts | `src/Ceataec.ExampleService.Api/Features/`, `src/Ceataec.ExampleService.Api/Cqrs/` |
| Domain-owned invariants and aggregate behavior, independent of HTTP/EF; ErrorOr results | `src/Ceataec.ExampleService.Domain/` |
| Aggregate writes through ICommandDbContext; named IDbQuery reads using AppDbContext and no tracking; EF Core/Postgres | `src/Ceataec.ExampleService.Infrastructure/Persistence/` and the feature handlers |
| Central ProblemDetails, versioning, configuration and service defaults | `src/Ceataec.ExampleService.Api/Http/`, Api composition, Infrastructure registration, ServiceDefaults |
| Domain/Api/Infrastructure unit tests, architecture tests and real-engine HTTP integration tests | `tests/` and the solution file |

A well-designed alternative can still diverge from this baseline. Explain deliberate
exceptions and their reasons; don't silently award full alignment or substitute a
preferred library/pattern. Framework/package-version differences need their compatibility
impact explained; never downgrade a working dependency blindly for cosmetic parity.
For a worker, library or another unsupported project type, identify the scope mismatch
and agree the relevant adaptation. Don't invent a second profile or score missing HTTP
layers as defects; omit a whole-template total when the profile does not apply.

## Adapt these to the target

Use the target's own language, aggregate names, entities, relationships and use cases.
Vessels, Voyages, Certificates, Tanks and streaming are teaching examples, never required
features. Rename projects for the new service while retaining the baseline responsibilities.
Carry over reusable patterns and checks, not sample-specific assertions or schema.
Simple business behavior need not acquire artificial aggregates or value objects.

Aspire AppHost is optional local orchestration; omission is not an alignment gap.
Retain applicable ServiceDefaults behavior and coherent dependency wiring. Authentication,
credentials, identity providers, logging fields and production deployment need the target's
actual requirements; sample anonymous access and local credentials are not defaults to copy.
Refactoring preserves shipped contracts, data and migration history. A deliberate exception
may remain necessary; list it rather than hiding it or breaking behavior to improve a score.

## Bounded context and aggregates

A bounded context defines where a domain model and its language have consistent meaning.
An aggregate is a consistency boundary inside that model, with a root controlling its
entities and invariants. A bounded context can contain several aggregates; a folder,
entity, aggregate, database table or service is not automatically a bounded context.

This example treats Vessels, Voyages and Certificates as aggregates in one bounded
context; Tank is a child entity in the Vessel aggregate. Their folders organize code.
Identity-only references or omitted foreign keys between aggregates do not establish
separate contexts. Choose referential integrity and concurrency behavior deliberately.
Do not infer context boundaries, mandate one context per aggregate, or score folder
separation as domain isolation. Split contexts when the target's language, model or
ownership actually differs, not because the sample contains several aggregate roots.

## Simple alignment score

Assess five areas against the selected skeleton revision:

| Area | What to compare |
| --- | --- |
| Responsibilities | Project separation, dependency direction and framework choices |
| Use-case organization | Versioned feature slices, HTTP adapters, command/query dispatch and contract shapes |
| Business behavior | Domain-owned invariants, aggregate encapsulation and result handling, using the target's own model |
| External boundaries | Persistence access patterns and libraries, API/error mapping, configuration and service defaults |
| Verification | Test responsibilities, architecture coverage and real-engine integration approach for relevant behavior |

For each: **0 = substantially diverges**, **1 = partly aligned**, **2 = aligned**.
Give a short explanation with both reference and target file/symbol evidence. Add the
five values for **template alignment out of 10**. No weights, percentages or calculator.
Exceptions remain visible and do not automatically turn a divergent area into a 2.

Mark missing evidence **unknown**. Explain genuinely inapplicable areas and omit the
total when any area is unknown/inapplicable or the reference cannot be inspected.
Do not penalize different domain names, absent teaching features or optional Aspire.
A target with the same patterns and its own domain can fully align. A tidy controller/
repository implementation with different libraries still has alignment gaps to explain.

Report correctness, security risks and code smells separately from the alignment score,
with severity and evidence. Matching the template does not prove correctness or passing
tests; state sampling limits and actual execution evidence. This is a discussion aid,
not a release gate. Version 0.5.0 changes what the /10 score measures; earlier scores
must be reassessed against the chosen source revision before comparison.

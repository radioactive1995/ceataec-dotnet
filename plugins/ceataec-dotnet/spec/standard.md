# CEATAEC .NET adoption standard

Version: **0.1.0 — proposed**. Reference: `radioactive1995/ceataec-dotnet`,
commit `a702f800b28a0a9b86d83af5495de8d7552bcc2c`.
The machine-readable criteria and weights are in [rules.json](rules.json).
Use this bundled version for a whole assessment; never silently compare against moving `main`.

## Scope and authority

The initial profile is **http-api-postgres**: a .NET 10 FastEndpoints service,
EF Core/Npgsql, vertical slices and a domain model. It supports optional Aspire
**local orchestration**. No Azure DevOps or Terraform files are required here:
the README assigns those to the external `build-repositories` seed.

The reference demonstrates conventions; it is not a production certification.
Existing runtime behavior, API contracts, data and explicit user constraints take
priority over cosmetic uniformity during adoption. Report conflicts rather than
inventing company decisions. Do not retrofit this HTTP profile onto a worker,
library, different persistence engine or modular monolith without a separate
profile decision. General defects can still be reviewed without a conformance score.

Rules distinguish **template** conventions from **readiness** checks. A service
can follow the template and still have security, reliability or correctness gaps.
The reference itself is subject to those checks. Do not propagate sample-only
anonymous access, credentials, domain names or missing deployment decisions as policy.

## Architecture

- `src/{Name}.Api`: HTTP composition, `Features/{BC}/{UseCase}/V{n}/`, CQRS markers,
  HTTP error mapping, processors and middleware. No Domain/Persistence/Infrastructure
  folders inside a feature; no generic repository layer or `Modules/` conversion.
- `src/{Name}.Domain`: aggregates, entities and value objects grouped by sibling
  bounded contexts. No dependency on Infrastructure, Api, FastEndpoints or EF Core.
- `src/{Name}.Infrastructure`: persistence, EF configuration/migrations, providers
  and settings. References Domain, not Api; no Aspire dependency.
- `src/{Name}.ServiceDefaults`: health, telemetry, HTTP resilience and discovery.
  Keep those capabilities with or without AppHost.
- Optional `src/{Name}.AppHost`: local orchestration, references Api. It is not
  the production deployment target. Api references Domain, Infrastructure and ServiceDefaults.
- Domain factories own validation/normalization and return `ErrorOr<T>`.
  Endpoints translate HTTP; handlers orchestrate. Avoid extra abstractions without a use case.
- Domain BCs also appear as siblings in Features and Infrastructure/Persistence.
  Sample BC names (Vessels, Voyages, Certificates) are examples, never required names.

## Features and persistence

- Write slice: Endpoint, Request, Response, Validator, Summary, Command, Handler.
  Read slice: Endpoint, Response, Summary, Query, Handler; a request type when needed.
  Contracts/DTOs/commands/queries are sealed records; one public type per file.
- Endpoints dispatch the FastEndpoints command bus and return typed result unions
  from `ExecuteAsync`; no persistence injection into endpoints.
- Commands use `ICommandDbContext.Set<TAggregateRoot>()`, constrained to aggregate
  roots. Query handlers use named `IDbQuery<TInput,TResult>` implementations under
  `Infrastructure/Persistence/{BC}/Queries/`; concrete `AppDbContext` is allowed there.
  Read queries use no-tracking access. Inspect behavior, not merely a matching string.
- Same-BC children may use SQL FKs. Cross-BC references store ids, without SQL FKs;
  handlers validate related ids. Existing FKs are not automatically dropped during refactoring.
  Assess concurrency/deletion behavior where a use case requires stronger guarantees.
- One migration history under Infrastructure/Persistence/Migrations. Integration
  tests apply real migrations against the production database engine (Postgres here).
  Preserve existing histories and data; never regenerate a live service's initial migration.
- Postgres-generated `uuidv7()` ids are the reference's provider-specific convention.
  Preserve established target id semantics during migration unless explicitly changed.

## HTTP contracts

- Explicit `Version(n)`, `V{n}` folders/namespaces and `/v{n}` routes. Release-group
  OpenAPI documents select the latest endpoint at/below their version ceiling.
- Preserve shipped contracts. Add another version for breaking changes; do not
  renumber or delete existing routes merely to match the sample.
- Centralized ProblemDetails mapping for domain errors, request validation and
  unexpected failures. Related-id misses map to 404; distinguish 400/404/409 appropriately.
- Audit/request logging uses global FE processors. Do not log raw user ids, tokens,
  connection strings or request bodies without an explicit data handling design.

## Tests and production decisions

Use Domain/Api/Infrastructure unit tests, HTTP integration tests and solution-wide
architecture tests. Add behavior/contract coverage for each changed slice; architecture
tests alone are not evidence that a service works. Adapt BC lists and assembly anchors
to the real service. Keep integration tests independent of AppHost.

Authentication/authorization, secrets, production readiness probes, schema deployment,
and dependency validation need explicit service decisions. Their absence is a readiness
finding, not a reason to invent an identity provider or deployment stack. Local example
credentials must remain local. A deterministic hash is not proof of anonymization.

## Exceptions and versioning

For a justified deviation, record rule id, reason, scope, owner, decision reference
and expiry/review date. An exception is visible alongside its finding; it does not
turn a failed criterion into a pass. Mark a criterion N/A only when its capability
is absent by design, with evidence; missing implementation is a failure, not N/A.

Change the standard, catalog, example code and relevant tests together. Bump the
version for scoring/behavior changes and document migration impact. Updates in a
consumer are explicit; never auto-upgrade its pinned baseline. The Brain note should
link to a released version of this standard rather than become a competing rule source.

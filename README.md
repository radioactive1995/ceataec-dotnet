# CEATAEC-AI-Brain-Dotnet

The **envisioned structure** of a CEATAEC .NET backend. This repo is the runnable teaching skeleton — coding agents and developers should **copy this layout**, not treat the sample as a shipping product.

Target: **.NET 10** (`net10.0`), multi-project solution (Api + Domain + Infrastructure), FastEndpoints, EF Core.

## Purpose

- Show the canonical shape: Api host, Domain and Infrastructure class libraries, Features and Cqrs in the host.
- Encode invariants as code (project references, architecture tests, union-type handlers, FK vs `VesselId`).
- Give a concrete reference when creating or reviewing new .NET services.

## Main patterns

- **Vertical Slice Architecture** — HTTP use cases live as feature folders under `Features/{BC}/{UseCase}/V{n}/` in the Api host. Each version folder owns the REPR types for that contract. Domain entities and persistence live in separate class libraries grouped by bounded context — Features do **not** contain `Domain/`, `Persistence/`, or `Infrastructure/` subfolders.
- **REPR (Request–Endpoint–Response)** — each HTTP use case is a Request, an Endpoint, and a Response (plus Validator / `Summary<TEndpoint>` on commands). Endpoints return `TypedResults` from `ExecuteAsync` instead of building `ProblemDetails` in the feature.
- **FastEndpoints command bus** — use cases own a `Command` (`ICommand` / `ICommandHandler`) or `Query` (`IQuery` / `IQueryHandler`). `IQuery` markers extend FE `ICommand` so both dispatch via `.ExecuteAsync()`. Endpoints map HTTP → command/query and map results to `TypedResults`. Handlers orchestrate application flow; Domain types own business invariants. Command handlers use `ICommandDbContext` for aggregate-root access, while query handlers delegate unrestricted no-tracking reads to `IDbQuery` implementations through `AppDbContext`.

## What this is not

Docker, Terraform, and Azure DevOps pipelines are **not** here — they come from the `build-repositories` seed when a real service repo is provisioned.

This is **not** modular architecture: there is no `Modules/` folder. Bounded contexts appear as subfolders under Domain, Features, and Infrastructure Persistence.

## Relation to CEATAEC-AI-Brain

Coding agents working in other repos do **not** need to clone this project. They follow the markdown conventions in CEATAEC-AI-Brain (`vault/Brain/Preferences/dotnet-conventions.md`).

When this skeleton’s layout or invariants change, update that Brain note by hand (or via a future sync). The Brain has no direct path or submodule link to this repo.

## Composition

`Program.cs` (Api host) only composes DI then the API pipeline:

```csharp
builder.Services
    .AddApi()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseApi();
app.Run();
```

| Project | Owns |
|---|---|
| `Ceataec.ExampleService.Api` | HTTP host: `Program`, pipeline (`DependencyInjection`, `WebApplicationExtensions`, `Middleware/`, `Processors/`, `ExceptionHandling/`), `Features/`, `Cqrs/` |
| `Ceataec.ExampleService.Domain` | Domain by BC (`Vessels`, `Voyages`, `Certificates`): aggregates/entities/VOs — ErrorOr for results; no project refs to Infra/Api |
| `Ceataec.ExampleService.Infrastructure` | `Settings/`, providers (`IUserProvider`, `IHashProvider`), `Persistence/` (`AppDbContext`, EF configs), `AddInfrastructure` — references Domain |

**References:** Infrastructure → Domain; Api → Domain + Infrastructure.

`AddInfrastructure` registers Settings, providers, and DbContext. Command/query handlers are discovered with FastEndpoints. One public type per file.

Audit/request logging uses **GlobalPre + GlobalPost** processors — not middleware. `Middleware/SampleMiddleware` is a no-op stub showing where non-FE ASP.NET middleware would go. Audit logs a **deterministic hash** of the user id via `IHashProvider` (never the raw id).

## Layout rules

- **No nested BCs.** Voyages is a sibling of Vessels under Domain / Features / Infrastructure Persistence, not `Vessels/Voyages/`.
- **Tank** lives in Domain Vessels and has a **SQL FK** to Vessel (same BC).
- **Voyage** / **Certificate** store **`VesselId` only** — no cross-BC SQL FK. Existence is checked in the command handler via `ICommandDbContext.Set<Vessel>()`.
- **Endpoints** never inject persistence contexts; they dispatch commands/queries. **Command handlers** inject `ICommandDbContext`, whose generic `Set<TAggregateRoot>()` constraint exposes only aggregate roots for direct reads and writes.
- No repository layer. **Named read queries** under `Infrastructure/Persistence/{BC}/Queries/` implement `IDbQuery<TInput, TResult>` with `static abstract QueryAsync`. They receive the concrete `AppDbContext`, may query any mapped type or projection, and may return any `TResult`; always call **`AsNoTracking()`** (enforced by ArchitectureTests). The Feature maps the result to an HTTP `*Response` (example: `GetVesselWithTanks` → `Vessel` → `GetVesselResponse`).
- **DDD:** Shared `Entity` / `AggregateRoot` (Id equality). Per BC: aggregate root at folder root (`Vessels/Vessel.cs`); child types under `Entities/` and `ValueObjects/` with matching namespaces (`...Vessels.Entities`, `...Vessels.ValueObjects`). Example: `Vessel` owns `Tank` creation through `AddTank` and uses the `ImoNumber` value object. Domain factories and handlers return `ErrorOr<T>`. Endpoints map failures to **ProblemDetails**: the first error’s type selects the status (400 / 404 / 409 / …) and every error is listed under `errors` as `{ code, description }`. FluentValidation request 400s use the same document (property name as `code`).
- Domain factories own invariants and normalization; request validators repeat basic checks only for fast client feedback. Add richer behavior when a use case needs it rather than introducing DDD abstractions preemptively.
- Endpoints use **Union-Type Returning Handlers**: override `ExecuteAsync` and return `TypedResults` (`Created` / `Ok` / `Problem`). Do not build `ProblemDetails` in the handler — enable `c.Errors.UseProblemDetails()` in `UseApi()`.
- Command features (writes): Endpoint + Request + Response + Validator + `Summary<TEndpoint>` + Command + Handler (`ICommand` / `ICommandHandler`).
- Query features (reads): Endpoint + Response + Summary + Query + Handler (`IQuery` / `IQueryHandler`). GetVessel uses named query `Persistence/Vessels/Queries/GetVesselWithTanks`, shared by both of its endpoint versions.
- Unknown related id on create or get (e.g. `VesselId`) → handler returns `Error.NotFound`; endpoint maps to a **404** ProblemDetails.
- **API versioning** uses the FastEndpoints **release group** strategy (see below). Every route is served under `/v{n}` and every endpoint calls `Version(n)` in `Configure()` (enforced by ArchitectureTests).
- Feature Request/Response/DTOs/Commands/Queries are **`sealed record`** with primary constructors; Settings POCOs are **`sealed record`** with `init` properties.
- Aggregates use private setters + factories (EF-friendly), while child entity construction stays behind its aggregate root.

## API versioning

Versioning is enabled in `UseApi()` with `c.Versioning.Prefix = "v"`, `DefaultVersion = 1`, and `PrependToRoute = true`, so `Post("/vessels")` is served as `POST /v1/vessels`. `DefaultVersion = 1` is only a safety net: each endpoint still calls `Version(n)` explicitly so the version is visible in the endpoint it belongs to.

An endpoint is **never edited to change its contract**. Instead a new endpoint class for the next version is added next to it, keeping the same bare route. OpenAPI documents act as **release groups**: each `SwaggerDocument` sets a `MaxEndpointVersion`, and for every bare route the document lists only the newest version at or below that ceiling. With `GetVessel` at v1 and v2, the two documents registered in `AddApi()` resolve to:

| Document | `MaxEndpointVersion` | Paths |
|---|---|---|
| `Release 1` | 1 | `/v1/vessels`, `/v1/vessels/{id}`, `/v1/voyages`, `/v1/certificates` |
| `Release 2` | 2 | `/v1/vessels`, `/v2/vessels/{id}`, `/v1/voyages`, `/v1/certificates` |

`POST /v1/vessels` stays in `Release 2` because it is still the latest version of that route.

Every HTTP contract lives in `Features/{BC}/{UseCase}/V{n}/` with namespace `...{UseCase}.V{n}`. The first version is always `V1/`. Type names keep the feature stem (`GetVesselResponse`) and do **not** include a version suffix; the folder and namespace carry the version.

To add a version of an existing use case:

1. Add a sibling `V{n}/` folder (`Features/Vessels/GetVessel/V2/`) with the same type names as `V1/`.
2. Keep the same bare route and call `Version(n)`.
3. Share the Domain and the named read query; give the version its **own** Request/Response. Reuse the existing Command/Query + Handler only when the contract is unchanged — a changed response shape gets its own Query + Handler (as `GetVessel.V2.GetVesselHandler` does over the shared `GetVesselWithTanks`).
4. Register a `SwaggerDocument` with `MaxEndpointVersion = n`.
5. To drop an older iteration from future releases, mark the surviving version with `Version(1, deprecateAt: 3)`. Removing the route entirely means deleting its version folder.

## Tests

- `Api.UnitTests` — feature validators (and other Api-only unit tests). Feature tests follow `Features/{BC}/{UseCase}/V{n}/`.
- `Domain.UnitTests` — domain unit tests (`Vessel.Create`, `ImoNumber`, …)
- `Infrastructure.UnitTests` — providers (`UserProvider`, `HashProvider`)
- `Api.IntegrationTests` — HTTP via `WebApplicationFactory` against the **same** database provider as the app (this sample: Npgsql + Testcontainers Postgres; Docker required). Feature tests follow `Features/{BC}/{UseCase}/V{n}/`; a test that compares two versions of one use case sits at the use-case folder (example: `Features/Vessels/GetVessel/GetVesselVersionTests.cs`).
- `ArchitectureTests` — NetArchTest boundary rules across Api, Domain, and Infrastructure (solution-wide; not prefixed with `Api.`). Split by concern: `LayerTests`, `FeatureTests`, `BoundedContextTests`, `PersistenceTests` (allowed BCs listed in `TestAssemblies`).

Do not swap a different database engine into IntegrationTests for convenience.

## Build and test

```bash
dotnet restore Ceataec.ExampleService.sln
dotnet build Ceataec.ExampleService.sln
dotnet test Ceataec.ExampleService.sln
```

IntegrationTests require Docker (Testcontainers Postgres).

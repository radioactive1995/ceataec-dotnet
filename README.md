# CEATAEC .NET template

The **envisioned structure** of a CEATAEC .NET backend. This repo is a runnable teaching skeleton. Its code is the source of truth for project structure and implementation decisions. Follow that baseline while using the target project's own domain and features.

Target: **.NET 10** (`net10.0`), multi-project solution (AppHost + ServiceDefaults + Api + Domain + Infrastructure), FastEndpoints, EF Core, Aspire for local run.

## Purpose

- Show one concrete shape: Aspire AppHost for local orchestration, ServiceDefaults, Api host (Features and Cqrs), Domain and Infrastructure class libraries.
- Encode invariants as code (project references, architecture tests, union-type handlers, FK vs `VesselId`).
- Give a concrete reference when creating or reviewing new .NET services.

## Main patterns

- **Vertical Slice Architecture** — HTTP use cases live as feature folders under `Features/{Area}/{UseCase}/V{n}/` in the Api host. Each version folder owns the REPR types for that contract. Domain entities and persistence live in separate class libraries grouped by domain area — Features do **not** contain `Domain/`, `Persistence/`, or `Infrastructure/` subfolders.
- **REPR (Request–Endpoint–Response)** — each HTTP use case is a Request, an Endpoint, and a Response (plus Validator / `Summary<TEndpoint>` on commands). Endpoints return `TypedResults` from `ExecuteAsync` instead of building `ProblemDetails` in the feature.
- **FastEndpoints command bus** — use cases own a `Command` (`ICommand` / `ICommandHandler`) or `Query` (`IQuery` / `IQueryHandler`). `IQuery` markers extend FE `ICommand` so both dispatch via `.ExecuteAsync()`. Endpoints map HTTP → command/query and map results to `TypedResults`. Handlers orchestrate application flow; Domain types own business invariants. Command handlers use `ICommandDbContext` for aggregate-root access, while query handlers delegate unrestricted no-tracking reads to `IDbQuery` implementations through `AppDbContext`.

## What this is not

Terraform and Azure DevOps pipelines are **not** here — they come from the `build-repositories` seed when a real service repo is provisioned. Docker is used only for **local** orchestration (Aspire Postgres) and integration tests (Testcontainers). This skeleton does not ship compose files, cloud deploy, or ACA.

This is **not** modular architecture: there is no `Modules/` folder. The example models one bounded context. Its subfolders organize aggregates and related use cases within that context.

## Adoption principles

[The baseline](plugins/ceataec-dotnet/spec/standard.md) explains which decisions to follow and which details to adapt. All three workflows inspect the actual skeleton at a recorded commit. Its structure, libraries and patterns define alignment; another service supplies its own domain and use cases. Deliberate deviations are explicit, and code/documentation mismatches are reported.

## Three AI workflows (proposed)

- **REVIEW-SCORE**: template alignment in five areas scored 0–2 (out of 10), with correctness and code smells reported separately. Feedback only.
- **REFACTOR**: align an existing project with the skeleton while preserving its behavior and contracts.
- **SCAFFOLD**: follow the skeleton's structure and decisions for a new project, with its own domain, optional Aspire and no teaching features unless requested.

See [setup and examples](docs/ai-adoption.md). Version 0.5.0 uses plain skills and normal .NET commands. No custom Python tooling is required. Optional Bash/PowerShell installers make the three skills available locally in Cursor and Claude; see the setup guide.

## The runnable example

The implementation below is the adoption baseline. Transfer its technical boundaries and patterns, adapting business names, relationships, behavior and tests to the target. Sample access, credentials and logging fields are not production defaults. Aspire is optional. A reasonable alternative design can still be an explicit alignment gap.

## Composition

**F5 / one-click local run** is `Ceataec.ExampleService.AppHost`. It starts Postgres 18 (persistent container + volume, pgAdmin) and the Api, then applies EF migrations in Development. Stopping AppHost leaves Postgres and pgAdmin running so the next start reuses them.

```bash
dotnet run --project src/Ceataec.ExampleService.AppHost
```

The Api can still run alone against `Database:ConnectionString` in `appsettings`. Aspire injects `ConnectionStrings:ceataec`; `AddInfrastructure` prefers that, then falls back to `Database:ConnectionString`.

`Program.cs` (Api) composes service defaults, DI, then the pipeline:

```csharp
builder.AddServiceDefaults();

builder.Services
    .AddApi()
    .AddInfrastructure(builder.Configuration);

builder.EnrichNpgsqlDbContext<AppDbContext>();

var app = builder.Build();
// Development: Migrate()
app.MapDefaultEndpoints();
app.UseApi();
app.Run();
```

| Project | Owns |
|---|---|
| `Ceataec.ExampleService.AppHost` | Local Aspire orchestration only (Postgres 18, pgAdmin, Api). Not a deploy target. |
| `Ceataec.ExampleService.ServiceDefaults` | Shared Aspire defaults: health (`/health`, `/alive`), OpenTelemetry, service discovery, HttpClient resilience |
| `Ceataec.ExampleService.Api` | HTTP host: `Program`, pipeline (`DependencyInjection`, `WebApplicationExtensions`, `Middleware/`, `Processors/`, `ExceptionHandling/`), `Http/` (ProblemDetails mapping), `Features/`, `Cqrs/` |
| `Ceataec.ExampleService.Domain` | One domain model with aggregate areas (`Vessels`, `Voyages`, `Certificates`): aggregates/entities/VOs — ErrorOr for results; no project refs to Infra/Api |
| `Ceataec.ExampleService.Infrastructure` | `Settings/`, providers (`IUserProvider`, `IHashProvider`), `Persistence/` (`AppDbContext`, EF configs, `Migrations/`), `AddInfrastructure` — references Domain. No Aspire packages. |

**References:** Infrastructure → Domain; Api → Domain + Infrastructure + ServiceDefaults; AppHost → Api.

`AddInfrastructure` registers Settings, providers, and DbContext. `EnrichNpgsqlDbContext` (Api only) adds Aspire retries/health/telemetry on that existing context. Command/query handlers are discovered with FastEndpoints. One public type per file.

Audit/request logging uses **GlobalPre + GlobalPost** processors — not middleware. `Middleware/SampleMiddleware` is a no-op stub showing where non-FE ASP.NET middleware would go. Audit logs a **deterministic hash** of the user id via `IHashProvider` (never the raw id). `IUserProvider.GetCurrentUserId()` reads `ClaimTypes.NameIdentifier` then `Identity.Name`, and returns `"anonymous"` when neither is present.

## How the sample is organized

- **Domain areas are organizational folders.** Vessels, Voyages and Certificates are peers within the same bounded context; folder nesting does not define a context boundary.
- **Tank** lives in Domain Vessels and has a **SQL FK** to Vessel (inside the Vessel aggregate).
- **Voyage** / **Certificate** store **`VesselId` only** — the sample omits SQL FKs between these aggregate roots. This is a persistence choice, not evidence of different bounded contexts or a general ban on cross-aggregate FKs. Existence is checked in the command handler via `ICommandDbContext.Set<Vessel>()`.
- **Endpoints** never inject persistence contexts; they dispatch commands/queries. **Command handlers** inject `ICommandDbContext`, whose generic `Set<TAggregateRoot>()` constraint exposes only aggregate roots for direct reads and writes.
- No repository layer. **Named read queries** under `Infrastructure/Persistence/{Area}/Queries/` implement `IDbQuery<TInput, TResult>` with `static abstract QueryAsync`. They receive the concrete `AppDbContext`, may query any mapped type or projection, and may return any `TResult`; always call **`AsNoTracking()`** (enforced by ArchitectureTests). The Feature maps the result to an HTTP `*Response` (example: `GetVesselWithTanks` → `Vessel` → `GetVesselResponse`).
- **EF migrations** live in `Infrastructure/Persistence/Migrations/` (one history for `AppDbContext`). Add them with `dotnet ef migrations add <Name> --project src/Ceataec.ExampleService.Infrastructure --startup-project src/Ceataec.ExampleService.Api --output-dir Persistence/Migrations`. Integration tests apply them with `Migrate()`.
- **DDD:** Shared `Entity` / `AggregateRoot` (Id equality). Per aggregate area: aggregate root at folder root (`Vessels/Vessel.cs`); child types under `Entities/` and `ValueObjects/` with matching namespaces (`...Vessels.Entities`, `...Vessels.ValueObjects`). Example: `Vessel` owns `Tank` creation through `AddTank` and uses the `ImoNumber` value object. Domain factories and handlers return `ErrorOr<T>`. Endpoints map failures to **ProblemDetails**: the first error’s type selects the status (400 / 404 / 409 / …) and every error is listed under `errors` as `{ code, description }`. FluentValidation request 400s use the same document (property name as `code`).
- Domain factories own invariants and normalization; request validators repeat basic checks only for fast client feedback. Add richer behavior when a use case needs it rather than introducing DDD abstractions preemptively.
- Endpoints use **Union-Type Returning Handlers**: override `ExecuteAsync` and return `TypedResults` (`Created` / `Ok` / `Problem`). Do not build `ProblemDetails` in the handler — enable `c.Errors.UseProblemDetails()` in `UseApi()`.
- Command features (writes): Endpoint + Request + Response + Validator + `Summary<TEndpoint>` + Command + Handler (`ICommand` / `ICommandHandler`).
- Query features (reads): Endpoint + Response + Summary + Query + Handler (`IQuery` / `IQueryHandler`). GetVessel uses named query `Persistence/Vessels/Queries/GetVesselWithTanks`, shared by both of its endpoint versions.
- Unknown related id on create or get (e.g. `VesselId`) → handler returns `Error.NotFound`; endpoint maps to a **404** ProblemDetails.
- **API versioning** uses the FastEndpoints **release group** strategy (see below). Every route is served under `/v{n}` and every endpoint calls `Version(n)` in `Configure()` (enforced by ArchitectureTests).
- Feature Request/Response/DTOs/Commands/Queries are **`sealed record`** with primary constructors; Settings POCOs are **`sealed record`** with `init` properties.
- Aggregates use private setters + factories (EF-friendly), while child entity construction stays behind its aggregate root. Entity ids are assigned by Postgres (`uuidv7()`); domain factories do not set `Id`.

## API versioning

Versioning is enabled in `UseApi()` with `c.Versioning.Prefix = "v"`, `DefaultVersion = 1`, and `PrependToRoute = true`, so `Post("/vessels")` is served as `POST /v1/vessels`. `DefaultVersion = 1` is only a safety net: each endpoint still calls `Version(n)` explicitly so the version is visible in the endpoint it belongs to.

An endpoint is **never edited to change its contract**. Instead a new endpoint class for the next version is added next to it, keeping the same bare route. OpenAPI documents act as **release groups**: each `SwaggerDocument` sets a `MaxEndpointVersion`, and for every bare route the document lists only the newest version at or below that ceiling. With `GetVessel` at v1 and v2, the two documents registered in `AddApi()` resolve to:

| Document | `MaxEndpointVersion` | Paths |
|---|---|---|
| `Release 1` | 1 | `/v1/vessels`, `/v1/vessels/{id}`, `/v1/voyages`, `/v1/certificates` |
| `Release 2` | 2 | `/v1/vessels`, `/v2/vessels/{id}`, `/v1/voyages`, `/v1/certificates` |

`POST /v1/vessels` stays in `Release 2` because it is still the latest version of that route.

Every HTTP contract lives in `Features/{Area}/{UseCase}/V{n}/` with namespace `...{UseCase}.V{n}`. The first version is always `V1/`. Type names keep the feature stem (`GetVesselResponse`) and do **not** include a version suffix; the folder and namespace carry the version.

To add a version of an existing use case:

1. Add a sibling `V{n}/` folder (`Features/Vessels/GetVessel/V2/`) with the same type names as `V1/`.
2. Keep the same bare route and call `Version(n)`.
3. Share the Domain and the named read query; give the version its **own** Request/Response. Reuse the existing Command/Query + Handler only when the contract is unchanged — a changed response shape gets its own Query + Handler (as `GetVessel.V2.GetVesselHandler` does over the shared `GetVesselWithTanks`).
4. Register a `SwaggerDocument` with `MaxEndpointVersion = n`.
5. To drop an older iteration from future releases, mark the surviving version with `Version(1, deprecateAt: 3)`. Removing the route entirely means deleting its version folder.

## Tests

- `Api.UnitTests` — feature validators (and other Api-only unit tests). Feature tests follow `Features/{Area}/{UseCase}/V{n}/`.
- `Domain.UnitTests` — domain unit tests (`Vessel.Create`, `ImoNumber`, …)
- `Infrastructure.UnitTests` — providers (`UserProvider`, `HashProvider`)
- `Api.IntegrationTests` — HTTP via `WebApplicationFactory` against the **same** database provider as the app (this sample: Npgsql + Testcontainers `postgres:18-alpine`; Docker required). Tests do **not** go through AppHost. Feature tests follow `Features/{Area}/{UseCase}/V{n}/`; a test that compares two versions of one use case sits at the use-case folder (example: `Features/Vessels/GetVessel/GetVesselVersionTests.cs`).
- `ArchitectureTests` — NetArchTest boundary rules across Api, Domain, and Infrastructure (solution-wide; not prefixed with `Api.`). Split by concern: `LayerTests`, `FeatureTests`, `PersistenceTests`. These check technical boundaries, not the number of bounded contexts.

Do not swap a different database engine into IntegrationTests for convenience.

## Build and test

```bash
dotnet restore Ceataec.ExampleService.sln
dotnet build Ceataec.ExampleService.sln
dotnet test Ceataec.ExampleService.sln
```

Local run: set AppHost as the startup project (F5) or `dotnet run --project src/Ceataec.ExampleService.AppHost` (Docker required for Postgres).

IntegrationTests require Docker (Testcontainers Postgres) and stay independent of AppHost.

# CEATAEC-AI-Brain-Dotnet

The **envisioned structure** of a CEATAEC .NET backend. This repo is the runnable teaching skeleton — coding agents and developers should **copy this layout**, not treat the sample as a shipping product.

Target: **.NET 10** (`net10.0`), multi-project solution (Api + Domain + Infrastructure), FastEndpoints, EF Core.

## Purpose

- Show the canonical shape: Api host, Domain and Infrastructure class libraries, Features and Cqrs in the host.
- Encode invariants as code (project references, architecture tests, union-type handlers, FK vs `VesselId`).
- Give a concrete reference when creating or reviewing new .NET services.

## Main patterns

- **Vertical Slice Architecture** — HTTP use cases live as feature folders under `Features/{BC}/{UseCase}/` in the Api host. Each folder owns the REPR types for that use case. Domain entities and persistence live in separate class libraries grouped by bounded context — Features do **not** contain `Domain/`, `Persistence/`, or `Infrastructure/` subfolders.
- **REPR (Request–Endpoint–Response)** — each HTTP use case is a Request, an Endpoint, and a Response (plus Validator / `Summary<TEndpoint>` on commands). Handlers return `TypedResults` from `ExecuteAsync` instead of building `ProblemDetails` in the feature.
- **FastEndpoints command bus** — use cases own a `Command` (`ICommand` / `ICommandHandler`) or `Query` (`IQuery` / `IQueryHandler`). `IQuery` markers extend FE `ICommand` so both dispatch via `.ExecuteAsync()`. Endpoints map HTTP → command/query and map results to `TypedResults`. Handlers own business logic and talk to `AppDbContext`. Writes mutate; reads are query-only.

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
| `Ceataec.ExampleService.Domain` | Entities by BC (`Vessels`, `Voyages`, `Certificates`) — no package or project refs to Infra/Api |
| `Ceataec.ExampleService.Infrastructure` | `Settings/`, providers (`IUserProvider`, `IHashProvider`), `Persistence/` (`AppDbContext`, EF configs), `AddInfrastructure` — references Domain |

**References:** Infrastructure → Domain; Api → Domain + Infrastructure.

`AddInfrastructure` registers Settings, providers, and DbContext. Command/query handlers are discovered with FastEndpoints. One public type per file.

Audit/request logging uses **GlobalPre + GlobalPost** processors — not middleware. `Middleware/SampleMiddleware` is a no-op stub showing where non-FE ASP.NET middleware would go. Audit logs a **deterministic hash** of the user id via `IHashProvider` (never the raw id).

## Layout rules

- **No nested BCs.** Voyages is a sibling of Vessels under Domain / Features / Infrastructure Persistence, not `Vessels/Voyages/`.
- **Tank** lives in Domain Vessels and has a **SQL FK** to Vessel (same BC).
- **Voyage** / **Certificate** store **`VesselId` only** — no cross-BC SQL FK. Existence is checked in the command handler via `AppDbContext.Vessels`.
- **Endpoints** never inject `AppDbContext`; they dispatch commands/queries. **Handlers** inject `AppDbContext` (commands own `SaveChangesAsync`; queries are read-only).
- No repository layer — handlers use EF Core directly.
- Endpoints use **Union-Type Returning Handlers**: override `ExecuteAsync` and return `TypedResults` (`Created` / `Ok` / `NotFound`). Do not build `ProblemDetails` in the handler — enable `c.Errors.UseProblemDetails()` in `UseApi()`.
- Command features (writes): Endpoint + Request + Response + Validator + `Summary<TEndpoint>` + Command + Handler (`ICommand` / `ICommandHandler`).
- Query features (reads): Endpoint + Response + Summary + Query + Handler (`IQuery` / `IQueryHandler`). Thin GET example: GetVessel.
- Unknown related id on create (e.g. `VesselId`) → handler returns `null`; endpoint maps to `TypedResults.NotFound()`.
- Feature Request/Response/DTOs/Commands/Queries are **`sealed record`** with primary constructors; Settings POCOs are **`sealed record`** with `init` properties; domain entities stay mutable classes for EF.

## Tests

- `UnitTests` — validators, `UserProvider`, `HashProvider`
- `IntegrationTests` — HTTP via `WebApplicationFactory` against the **same** database provider as the app (this sample: Npgsql + Testcontainers Postgres; Docker required)
- `ArchitectureTests` — NetArchTest boundary rules across Api, Domain, and Infrastructure assemblies

Do not swap a different database engine into IntegrationTests for convenience.

## Build and test

```bash
dotnet restore Ceataec.ExampleService.sln
dotnet build Ceataec.ExampleService.sln
dotnet test Ceataec.ExampleService.sln
```

IntegrationTests require Docker (Testcontainers Postgres).

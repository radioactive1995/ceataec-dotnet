# CEATAEC-AI-Brain-Dotnet

The **envisioned structure** of a CEATAEC .NET backend. This repo is the runnable teaching skeleton — coding agents and developers should **copy this layout**, not treat the sample as a shipping product.

Target: **.NET 10** (`net10.0`), single API project, FastEndpoints, EF Core.

## Purpose

- Show the canonical shape: `Api/`, `Domain/`, `Features/`, `Infrastructure/` (Persistence under Infrastructure).
- Encode invariants as code (architecture tests, union-type handlers, FK vs `VesselId`).
- Give a concrete reference when creating or reviewing new .NET services.

## Main patterns

- **Vertical Slice Architecture** — HTTP use cases live as feature folders under `Features/{BC}/{UseCase}/`. Each folder owns the REPR types for that use case. Domain entities and persistence are shared root/infra layers grouped by bounded context — Features do **not** contain `Domain/`, `Persistence/`, or `Infrastructure/` subfolders.
- **REPR (Request–Endpoint–Response)** — each HTTP use case is a Request, an Endpoint, and a Response (plus Validator / `Summary<TEndpoint>` on commands). Handlers return `TypedResults` from `ExecuteAsync` instead of building `ProblemDetails` in the feature.

## What this is not

Docker, Terraform, and Azure DevOps pipelines are **not** here — they come from the `build-repositories` seed when a real service repo is provisioned.

This is **not** modular architecture: there is no `Modules/` folder. Bounded contexts appear as subfolders under `Domain/`, `Features/`, and `Infrastructure/Persistence/`.

## Relation to CEATAEC-AI-Brain

Coding agents working in other repos do **not** need to clone this project. They follow the markdown conventions in CEATAEC-AI-Brain (`vault/Brain/Preferences/dotnet-conventions.md`).

When this skeleton’s layout or invariants change, update that Brain note by hand (or via a future sync). The Brain has no direct path or submodule link to this repo.

## Composition

`Program.cs` only composes DI then the API pipeline:

```csharp
builder.Services
    .AddApi()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseApi();
app.Run();
```

| Slice | Owns |
|---|---|
| `Api/` | HTTP pipeline (exception handling, **sample** middleware, GlobalPre/GlobalPost processors). **Not** endpoints. `AddApi` in `DependencyInjection.cs`; `UseApi` in `WebApplicationExtensions.cs`. |
| `Domain/` | Entities by BC (`Vessels`, `Voyages`, `Certificates`) |
| `Features/` | REPR use cases by BC — Endpoint, Request, Response, Validator, Summary only |
| `Infrastructure/` | `Settings/` (`sealed record` POCOs with `init`, e.g. `DatabaseSettings`), providers (`IUserProvider`, `IHashProvider`), `Persistence/` (`AppDbContext`, repos, EF configs by BC) |

`AddInfrastructure` registers Settings, providers, DbContext, and repositories. One public type per file.

Audit/request logging uses **GlobalPre + GlobalPost** processors — not middleware. `Api/Middleware/SampleMiddleware` is a no-op stub showing where non-FE ASP.NET middleware would go. Audit logs a **deterministic hash** of the user id via `IHashProvider` (never the raw id).

## Layout rules

- **No nested BCs.** Voyages is a sibling of Vessels under `Domain/` / `Features/` / `Infrastructure/Persistence/`, not `Vessels/Voyages/`.
- **Tank** lives in `Domain/Vessels` and has a **SQL FK** to Vessel (same BC).
- **Voyage** / **Certificate** store **`VesselId` only** — no cross-BC SQL FK. Existence is checked in the feature via `IVesselRepository`.
- Features never inject `AppDbContext`; they use repository interfaces from `Infrastructure/Persistence/{BC}/`.
- Endpoints use **Union-Type Returning Handlers**: override `ExecuteAsync` and return `TypedResults` (`Created` / `Ok` / `NotFound`). Do not build `ProblemDetails` in the handler — enable `c.Errors.UseProblemDetails()` in `UseApi()`.
- Command features: Endpoint + Request + Response + Validator + `Summary<TEndpoint>`.
- Thin GET: Endpoint + Response + Summary (copy CreateVessel for full commands).
- Unknown related id on create (e.g. `VesselId`) → `TypedResults.NotFound()`.
- Feature Request/Response/DTOs are **`sealed record`** with primary constructors; Settings POCOs are **`sealed record`** with `init` properties; domain entities stay mutable classes for EF.

## Tests

- `UnitTests` — validators, `UserProvider`, `HashProvider`
- `IntegrationTests` — HTTP via `WebApplicationFactory` against the **same** database provider as the app (this sample: Npgsql + Testcontainers Postgres; Docker required)
- `ArchitectureTests` — NetArchTest boundary rules

Do not swap a different database engine into IntegrationTests for convenience.

## Build and test

```bash
dotnet restore Ceataec.ExampleService.sln
dotnet build Ceataec.ExampleService.sln
dotnet test Ceataec.ExampleService.sln
```

IntegrationTests require Docker (Testcontainers Postgres).

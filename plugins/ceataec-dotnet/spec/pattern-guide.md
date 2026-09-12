# Pattern decisions for the HTTP/Postgres profile

Use the [standard](standard.md) as authority. Choose patterns by the problem and target
constraints. These are explanations and examples, not additional weighted criteria.

| Need | Starting point | What to avoid / decide |
| --- | --- | --- |
| Accept an HTTP write | Versioned Endpoint dispatches Command; Handler invokes Domain and `ICommandDbContext` | Persistence in Endpoint; generic CRUD that bypasses behavior |
| Return a read model | Query Handler calls a named `IDbQuery` implementation in Infrastructure | New generic repository or tracked read merely for symmetry with writes |
| Enforce a business invariant | Aggregate/value-object factory or behavior method | Making an HTTP validator the only protection for non-HTTP callers |
| Validate request shape | Feature Validator; share domain constraints when semantics match | Database round trips in every validator without a use-case reason |
| Reference another bounded context | Store its id and validate according to the use case | Assuming preflight existence checks prevent concurrent deletion; inventing a distributed transaction |
| Change a public response | Preserve existing contract; add a version when the change is breaking | Renumbering existing routes or treating every additive feature as a new API version |
| Integrate an external side effect | Establish ownership, failure/retry and duplicate-handling requirements first | Claiming an outbox, event bus or exactly-once delivery already exists in this template |
| Share cross-cutting behavior | Existing middleware/processors/DI where they fit | Hiding business decisions in global processors or adding a base handler for unrelated slices |

`ICommandDbContext` permits aggregate-root access, including related-id checks. Read
handlers in the reference can inject `AppDbContext` and delegate query construction to
the named query; do not mistakenly prohibit that or move all handler mapping into EF.
Domain must remain independent of EF and HTTP. The absence of cross-BC SQL FKs is the
reference convention, not authorization to drop an existing service's constraints.

## Source map

Paths below are **inside a reference checkout**, relative to its root, at the full
commit in [rules.json](rules.json). They are not files inside an installed plugin.
Use the local pinned checkout when available; otherwise treat these as navigation
hints and inspect equivalent target symbols. Never silently substitute moving main.

| Example | Reference path |
| --- | --- |
| HTTP write and command dispatch | `src/Ceataec.ExampleService.Api/Features/Vessels/CreateVessel/V1/CreateVesselEndpoint.cs` |
| Domain factory then aggregate save | `src/Ceataec.ExampleService.Api/Features/Vessels/CreateVessel/V1/CreateVesselHandler.cs` |
| Read orchestration and DTO mapping | `src/Ceataec.ExampleService.Api/Features/Vessels/GetVessel/V1/GetVesselHandler.cs` |
| Named no-tracking query | `src/Ceataec.ExampleService.Infrastructure/Persistence/Vessels/Queries/GetVesselWithTanks.cs` |
| Aggregate/child encapsulation | `src/Ceataec.ExampleService.Domain/Vessels/Vessel.cs` |
| Cross-context related-id check | `src/Ceataec.ExampleService.Api/Features/Voyages/CreateVoyage/V1/CreateVoyageHandler.cs` |
| Central error mapping | `src/Ceataec.ExampleService.Api/Http/ProblemDetailsMapper.cs` |
| Effective connection precedence | `src/Ceataec.ExampleService.Infrastructure/Settings/DatabaseSettings.cs` |
| Version compatibility test | `tests/Ceataec.ExampleService.Api.IntegrationTests/Features/Vessels/GetVessel/GetVesselVersionTests.cs` |

The sample's anonymous endpoints, identifier logging and local startup behavior are
teaching choices, not production defaults. For workers, other databases, event-driven
delivery or complex consistency requirements, state where this profile stops helping
and propose a decision with tradeoffs rather than manufacturing template requirements.

# Reference-template review for AI adoption

Reviewed `radioactive1995/ceataec-dotnet` at
`a702f800b28a0a9b86d83af5495de8d7552bcc2c`. Static review of the complete file inventory,
README, project graph, representative write/read paths, composition, domain/persistence
patterns and architecture/test setup. No .NET SDK was available; no C# execution or
package-availability verification is claimed. This is not an exhaustive security audit.

## Assessment

The reference has a useful, opinionated teaching structure: ten projects (five source,
five test), small versioned feature slices, typed results, domain factories, explicit
command/query persistence boundaries and real-engine integration tests. These are a
strong basis for the three workflows. The canonical specification should capture the
intent of these boundaries, not require every consumer to keep the sample domain.

The main adoption gap was the lack of distribution/workflow definitions. The README
sent coding agents to a separately maintained Brain note. This change adds versioned
rules, evidence-aware scoring, workflow skills, harness adapters and generation/setup
helpers, and makes the Brain a link to the standard instead of an independent authority.

## Findings in the pinned C# reference

| Priority / rule | Evidence | Impact and recommendation |
| --- | --- | --- |
| High — CFG-001 / TST-001 | `tests/...Api.IntegrationTests/ExampleWebApplicationFactory.cs:42` overrides only `Database:ConnectionString`; `src/...Infrastructure/DependencyInjection.cs:29-30` prefers `ConnectionStrings:ceataec`; the factory calls `Migrate()` at line 56 | An inherited preferred connection can cause tests/migrations to target a database other than their container. Override both keys or replace DbContext configuration explicitly. The generator now overrides both in generated starters; the pinned sample still needs the corresponding fix and an execution test. |
| Medium — CFG-001 | `src/...Infrastructure/DependencyInjection.cs:21-30` validates nonblank values using OR, but selects using `??` | A present empty/whitespace preferred value can win over a valid fallback, while validation accepts the fallback. Resolve one effective nonblank value and validate/use that same value. Add cases for missing/empty/whitespace preferred values. High confidence from the code; runtime behavior was not exercised. |
| Medium — TST-001 | `tests/...ArchitectureTests/FeatureTests.cs:33-34` searches for `Version(`; `PersistenceTests.IDbQuery_QueryAsync_uses_AsNoTracking` searches for `.AsNoTracking(` | A comment or unrelated call can satisfy the test without the intended behavior. Prefer endpoint metadata assertions and query behavior/semantic analysis. These tests are helpful hints but should not be treated as proof by a scoring agent. |
| Medium — ARC-001 / TST-001 | `FeatureTests.Endpoints_do_not_reference_AppDbContext` and `TestAssemblies.DependsOnAppDbContext` check the concrete type | An endpoint injecting `ICommandDbContext` can still violate the stated no-persistence rule without this check catching it. Cover persistence interfaces/base types and actual dependency relationships. |
| Readiness decision — SEC-001 | `src/...Api/Features/Vessels/CreateVessel/V1/CreateVesselEndpoint.cs:17` and other endpoints call `AllowAnonymous()` | Explicit teaching behavior, not a discovered production exposure. Consumer scaffolding must label this and require an intentional public/private access design before shipping. Do not guess a company identity provider. |
| Readiness decision — OPS-001 | `src/...ServiceDefaults/Extensions.cs`, `MapDefaultEndpoints`, maps probes only in Development; Api `Program.cs` migrates only in Development | Production health/schema-deployment behavior needs a decision in provisioning. No requirement to add Terraform or Azure DevOps here, because the README assigns them to another seed. |
| Low — MNT-001 | Both `src/...Api/Properties/launchSettings.json` and `src/...Api/Properties/Properties/launchSettings.json` are tracked | The nested duplicate is confusing and should be removed. The generator excludes it. |
| Readiness decision — MNT-001 | No root `global.json`, local EF tool manifest or lock-file policy; versions live in individual `.csproj` files | Before a company release, decide SDK/dependency/update policy and verify package restore against approved feeds. Do not blindly change versions based on this static review. |

All abbreviated paths above are rooted at `Ceataec.ExampleService` project names.
Sample connection strings and deterministic user-id hashing also need explicit
production treatment; neither is a safe company default merely because it exists here.
The generator clears committed connection defaults, but preserves the example behavior.

## Suggested adoption order

1. Fix test-database isolation and effective-connection validation in the runnable
   reference; execute the C# suite and pin a new approved reference commit.
2. Validate both generated variants and the actual harness discovery/permissions.
3. Pilot REVIEW-SCORE on an existing service; calibrate weights, exceptions and
   evidence requirements with its owners before using scores as a gate.
4. Migrate one use case with preserved contracts, then exercise new-service generation.
5. Add native `dotnet new` packaging or additional profiles once the first matrix is
   verified; publish a versioned company plugin and link the Brain conventions to it.

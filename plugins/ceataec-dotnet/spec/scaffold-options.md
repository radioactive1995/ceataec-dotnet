# Scaffold choices

| Choice | Release 0.2 behavior |
| --- | --- |
| Name/root namespace | Required PascalCase segments, e.g. `Ceataec.Orders`; used in project/file names, namespaces and Aspire generated project identifier |
| Output | Required new directory; never merges into an existing project |
| Aspire | Default yes: AppHost with local Postgres. No: removes AppHost and Api Aspire enrichment/package; keeps ServiceDefaults health/telemetry/resilience |
| Database | Postgres 18 only, matching migrations and Testcontainers; other providers require a separately validated profile |
| Domain examples | Retained and clearly labeled; business-specific or empty generation is an additional agent task |
| Tests | Keep all existing test projects, including architecture and real-engine integration tests |
| Auth | Sample anonymous endpoints retained for teaching; production policy must be implemented before shipping |
| CI/deploy | Supplied externally by company provisioning; no invented Terraform or Azure DevOps pipeline |
| AI integration | Four local skills for the chosen harness; optional attachment after generation |

The generator pins both the standard version and reference commit in
`.ceataec-template.json`, assigns a new AppHost UserSecretsId, and renames the local
database. It clears committed connection-string defaults; use environment variables
or user secrets for a standalone local database. No `dotnet new` package is published
by this release; the generator reads the runnable reference directly to avoid another
copy of the C# source. Native template packaging can wrap this once the supported
option matrix has passed .NET execution tests.

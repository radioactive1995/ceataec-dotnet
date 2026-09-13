# Adoption principles

Version **0.4.0 — proposed**. Use this guidance to understand and improve a project's
structure, not to make every repository a copy of this one.

## What carries across

- Organize code around the project's own use cases. Keep related behavior easy to find.
- Separate transport, business decisions and infrastructure responsibilities. Business
  rules should not depend on HTTP or database implementation details.
- Keep invariants where all relevant callers use them. A simple application can have
  simple behavior; introduce aggregates, value objects or abstractions when they help.
- Make reads, writes, failures and external side effects explicit. Keep persistence
  concerns out of transport adapters and preserve API/data contracts as code evolves.
- Test meaningful behavior and boundaries. Keep configuration, errors and local
  verification understandable. Follow the target's formatting/analyzer conventions.

The sample shows one implementation: vertical slices, Api/Domain/Infrastructure projects,
FastEndpoints, ErrorOr, EF Core/Postgres, versioned HTTP endpoints and optional Aspire.
Equivalent implementations are valid. Project names, folder counts, libraries, database,
identifier generation, relationships and deployment setup are not adoption requirements.
Use the target's existing decisions unless changing them is part of the task.

Vessels, Voyages, Certificates, Tanks and their features (including streaming) illustrate
ideas. Never require, score, rename another domain into, or automatically scaffold them.
The same applies to example providers, middleware, migrations and tests. Copy a pattern
only when it serves the target's requirements. Don't copy sample access, credentials or
logging choices as production defaults. A worker or library does not need HTTP layers.

## Simple review score

Assess these five areas in the target's context, using code evidence:

| Area | Question |
| --- | --- |
| Responsibilities | Are transport, business behavior and infrastructure sensibly separated? |
| Use-case organization | Is related code easy to find and change without unrelated duplication? |
| Business behavior | Are the project's actual rules enforced in appropriate, reusable places? |
| External boundaries | Are persistence, contracts, configuration and failure handling deliberate? |
| Verification | Do meaningful tests cover important behavior, with clear ways to run checks? |

For each: **0 = substantial gaps**, **1 = partly addressed**, **2 = well addressed**.
Give one short explanation with a file/symbol reference. Add the five numbers for
**a score out of 10**; no weights, percentages or automated calculator.

If evidence is missing, mark the area **unknown**. If an area truly does not apply,
explain that. In either case omit the total and report which areas were assessed;
never turn unknown into zero or silently award full marks. Judge simplicity against
real requirements, not the presence of sample entities or unnecessary abstractions.

State sampling limits and available test execution evidence separately. A source review
cannot prove tests passed. Keep serious bugs/security risks visible regardless of score.
The score is a discussion aid, not a release gate or comparable to the old weighted scores.

---
name: scaffold
description: Create a new .NET project using the skeleton's architectural ideas, tailored to the project's own purpose and domain, with optional Aspire.
---

# SCAFFOLD

Read [the principles](../../spec/standard.md). Reuse supplied choices. Ask briefly for
missing project name/output location and purpose; establish whether Aspire is wanted.
For an HTTP/Postgres service, the sample stack is a reasonable proposed default. For
another project type, adapt the design instead of inventing HTTP or persistence needs.
Don't ask for a large questionnaire or invent business requirements.

Use an available reference checkout at a known commit; record that commit and the
chosen options in the new project's README. If source is unavailable, acquire the
reference through authorized repository access or ask for its location. Never claim
to have copied or validated source you could not inspect.

Create only in a new destination. Use normal .NET tooling and the reference as a guide
for project wiring, configuration and boundaries. Generate the minimum useful skeleton
for the stated purpose. Without supplied business behavior, leave a clean starting
point and explain where the first use case belongs. Include teaching features only
when explicitly requested; don't just rename the example service.

If reusing reference files, remove sample-only entities, endpoints, queries, mappings,
providers, migrations and tests together. Keep reusable architecture checks, adapting
their assembly anchors and domain lists. Start any new database history from the new
model; don't ship the sample schema. This applies only to the new project, never to
an existing service's migration history. Use fresh secret-store identities, no copied
credentials, and an explicit access policy for any business endpoints.

Aspire is optional local orchestration. If omitted, remove AppHost-specific references
and wiring coherently; preserve applicable health/telemetry and document how dependencies
run. Don't copy this repository's AI packaging or maintenance files into the new service.

Run normal restore/build/test commands for the generated project, including real-engine
integration tests where relevant. Check that sample-only references are absent and the
chosen startup path works when prerequisites are available. Report failures or checks
that couldn't run. Deliver the new files, run instructions and remaining decisions;
no automatic commit, push or deployment.

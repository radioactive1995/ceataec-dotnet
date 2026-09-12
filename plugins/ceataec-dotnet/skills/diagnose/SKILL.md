---
name: diagnose
description: Investigate a failing .NET request, test, startup or build using evidence and discriminating checks. Use for debugging symptoms; make fixes when requested.
---

# DIAGNOSE

Establish the target, symptom, expected behavior, environment and recent relevant
change. Reuse supplied logs without exposing secret values. Start with source and
existing evidence. Separate a hypothesis from an observed cause, and choose the next
check by which competing explanation it can eliminate.

For diagnosis-only requests, report findings and proposed commands in chat; do not
edit or execute target code. A request to reproduce locally or fix authorizes scoped
local execution and its normal build/test outputs. Preserve user changes and use
disposable test resources. Don't query production, change shared infrastructure, dump
credentials or apply shared database migrations as an implicit debugging step.

Use repository-specific seams: configuration precedence and effective connection
selection; endpoint version/dispatch and ProblemDetails mapping; handler/domain errors;
EF tracking/query shape and migration compatibility; test factory overrides and Docker
availability; package resolution versus compilation failures. Inspect those that fit
the symptom, not an exhaustive checklist. Infrastructure failure is not proof of a
product defect. A single slow run is not a performance baseline.

When fixing, reproduce or otherwise establish evidence, add a meaningful regression
test when practical, apply the smallest causal fix and verify the affected path. Don't
weaken an assertion, switch database engines or retry indefinitely to get a green run.
After a failed attempt, use the new evidence before retrying; stop when further progress
requires unavailable evidence/access or an unresolved business decision.

Return observed cause or ranked hypotheses, evidence, checks executed versus proposed,
fix/verification if authorized, and the specific next evidence needed if unresolved.
Use [VERIFY](../verify/SKILL.md) when broader execution is relevant and authorized.

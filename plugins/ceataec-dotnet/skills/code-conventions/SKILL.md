---
name: code-conventions
description: Explain, check or apply CEATAEC C# naming, formatting and local coding conventions in selected files. Use for style questions and scoped convention fixes.
---

# CODE-CONVENTIONS

Read [coding conventions](../../spec/coding-conventions.md). Identify the selected
files and whether the user wants an explanation, review or edits. An explanation
or check returns advice in chat without edits or command execution. A request to
apply conventions authorizes scoped edits; preserve behavior and existing user work.

Inspect the target's applicable instructions, nearest `.editorconfig`, build/analyzer
configuration and neighboring types before proposing a change. Distinguish enforced
configuration, the proposed CEATAEC standard and optional preferences. Cite the
source of a recommendation; don't turn a personal style preference into a violation.
If the target conflicts with the template, explain the difference and preserve its
accepted convention unless aligning that convention is part of the request.

Keep this local: naming, type shape, formatting, cancellation and error/logging idioms.
Do not move projects, introduce a repository pattern or change HTTP/data contracts to
make code look uniform. Public symbol/serialization changes need compatibility review.
In edit mode use targeted formatting only; do not format the whole solution or generated
migrations as collateral work. Validate behavior-relevant edits with focused checks;
formatting alone does not justify new tests. Report fixes and unresolved conflicts.

---
name: choose-pattern
description: Advise where .NET behavior belongs and which CEATAEC command, query, domain or integration pattern fits a concrete use case. Use for design decisions before coding.
---

# CHOOSE-PATTERN

Read [the pattern guide](../../spec/pattern-guide.md), then the relevant section of
[the standard](../../spec/standard.md). Identify the use case, existing target
structure and the constraint that makes this a decision. Reuse supplied requirements;
ask only when ownership, consistency or an external contract would change the choice.

Return a recommendation in chat: where the behavior belongs, its data/dependency flow,
why it fits, the simplest viable alternative and the tradeoff. Anchor the explanation
in a target symbol and, when available, the named reference example. Distinguish observed
code from a proposed design. Do not claim a reference example was inspected if unavailable.

Prefer the existing vertical-slice mechanisms. A new interface, domain service, event
bus or repository needs a concrete boundary or requirement; pattern names alone are
not justification. For cross-context work, discuss ownership and consistency rather
than pretending an existence check prevents races. Explicitly flag unsupported profiles.

This skill is advisory: no files, ADRs, packages or source changes by default. If the
user also asks to implement, carry the chosen decision into the scoped implementation;
do not make them repeat authorization. Record an ADR only when requested or required
by the target's conventions, and mark unapproved decisions as proposed.

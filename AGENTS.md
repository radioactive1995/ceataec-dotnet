# CEATAEC .NET template

This repository is a proposed company standard and runnable teaching sample.
Read `README.md` for the sample and `plugins/ceataec-dotnet/spec/standard.md`
for versioned adoption requirements. Proposed rules are not approved company policy.

For adoption or everyday development, choose the entry point matching the task.
Load only the relevant skill; do not run every workflow on every change:

| Intent | Skill |
| --- | --- |
| REVIEW-SCORE: feedback only | `plugins/ceataec-dotnet/skills/review-score/SKILL.md` |
| REFACTOR: align an existing service | `plugins/ceataec-dotnet/skills/refactor/SKILL.md` |
| SCAFFOLD: create a new service | `plugins/ceataec-dotnet/skills/scaffold/SKILL.md` |
| VERIFY: execute builds/tests | `plugins/ceataec-dotnet/skills/verify/SKILL.md` |
| CODE-CONVENTIONS: local style guidance or fixes | `plugins/ceataec-dotnet/skills/code-conventions/SKILL.md` |
| CHOOSE-PATTERN: design advice | `plugins/ceataec-dotnet/skills/choose-pattern/SKILL.md` |
| IMPLEMENT-FEATURE: deliver one use case | `plugins/ceataec-dotnet/skills/implement-feature/SKILL.md` |
| TRACE-FLOW: explain existing behavior | `plugins/ceataec-dotnet/skills/trace-flow/SKILL.md` |
| DIAGNOSE: investigate a failure | `plugins/ceataec-dotnet/skills/diagnose/SKILL.md` |
| REVIEW-CHANGE: feedback on a diff | `plugins/ceataec-dotnet/skills/review-change/SKILL.md` |

Distinguish the **reference template** from the **target service**. Do not refactor
the template when asked to review a target. A review never authorizes writes,
commits, PRs, builds, restores, tests, or installation in the target.
Ordinary maintenance requests do not automatically invoke an adoption workflow.

Keep architecture changes, the standard, rule catalog and tests consistent.
Validate AI tooling with `python3 -m unittest discover -s tests/ai -v`.
Build/test C# with the commands in README; integration tests require Docker.
Report commands that could not run. Do not equate static inspection with passing tests.

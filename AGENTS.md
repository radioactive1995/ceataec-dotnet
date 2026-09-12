# CEATAEC .NET template

This repository is a proposed company standard and runnable teaching sample.
Read `README.md` for the sample and `plugins/ceataec-dotnet/spec/standard.md`
for versioned adoption requirements. Proposed rules are not approved company policy.

For a requested adoption workflow, read the corresponding entry point:

| Intent | Skill |
| --- | --- |
| REVIEW-SCORE: feedback only | `plugins/ceataec-dotnet/skills/review-score/SKILL.md` |
| REFACTOR: align an existing service | `plugins/ceataec-dotnet/skills/refactor/SKILL.md` |
| SCAFFOLD: create a new service | `plugins/ceataec-dotnet/skills/scaffold/SKILL.md` |

Distinguish the **reference template** from the **target service**. Do not refactor
the template when asked to review a target. A review never authorizes writes,
commits, PRs, builds, restores, tests, or installation in the target.
Ordinary maintenance requests do not automatically invoke an adoption workflow.

Keep architecture changes, the standard, rule catalog and tests consistent.
Validate AI tooling with `python3 -m unittest discover -s tests/ai -v`.
Build/test C# with the commands in README; integration tests require Docker.
Report commands that could not run. Do not equate static inspection with passing tests.

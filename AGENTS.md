# CEATAEC .NET skeleton

This is a runnable example and three adoption workflows. Read
[the principles](plugins/ceataec-dotnet/spec/standard.md) for what transfers to another
project. The sample's domain, features and technology choices are examples, not a checklist.

| Request | Read |
| --- | --- |
| REVIEW-SCORE: feedback only | `plugins/ceataec-dotnet/skills/review-score/SKILL.md` |
| REFACTOR: improve an existing project | `plugins/ceataec-dotnet/skills/refactor/SKILL.md` |
| SCAFFOLD: start a new project | `plugins/ceataec-dotnet/skills/scaffold/SKILL.md` |

Use only the workflow requested. Ordinary coding needs no separate skill.
Distinguish this reference from the target project. Preserve the target's domain,
behavior, contracts and user changes; do not replace them with the example service.
A review is feedback in chat: no writes, execution, commits or posted PR reviews.
For refactoring/scaffolding, use normal project build/test commands and report what
actually ran. Tests for this reference require .NET 10 and Docker; see README.

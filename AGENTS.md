# CEATAEC .NET skeleton

This runnable skeleton is the source of truth for the three adoption workflows. Read
[the baseline](plugins/ceataec-dotnet/spec/standard.md), then inspect the selected source
revision. Follow its structure and implementation decisions; adapt the target's own
domain and features. Optional Aspire and sample business entities are not requirements.

| Request | Read |
| --- | --- |
| REVIEW-SCORE: feedback only | `plugins/ceataec-dotnet/skills/review-score/SKILL.md` |
| REFACTOR: improve an existing project | `plugins/ceataec-dotnet/skills/refactor/SKILL.md` |
| SCAFFOLD: start a new project | `plugins/ceataec-dotnet/skills/scaffold/SKILL.md` |

Use only the workflow requested. Ordinary coding needs no separate skill.
Distinguish this reference from the target project. Preserve the target's domain,
behavior, contracts and user changes; do not replace them with the example service.
Report code/documentation mismatches and deliberate deviations explicitly.
A review scores alignment against the actual reference, with code-quality findings separate.
A review is feedback in chat: no writes, target-code execution, commits or posted PR reviews.
For refactoring/scaffolding, use normal project build/test commands and report what
actually ran. Tests for this reference require .NET 10 and Docker; see README.

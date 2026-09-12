# Review report contract

Return a readable report in chat. Include target/revision, standard version/profile,
scope/sample coverage, strengths, rule table, prioritized findings, recommended
adoption sequence and validation limitations. Do not write a report file during
REVIEW-SCORE. Optional JSON follows this contract; it can be piped to `scripts/score.py`.

```json
{
  "standardVersion": "0.1.0",
  "profile": "http-api-postgres",
  "target": "owner/service@commit-or-worktree-description",
  "assessments": [
    {
      "id": "ARC-001",
      "status": "partial",
      "evidence": ["src/Service.Domain/Service.Domain.csproj:12 references Infrastructure"],
      "reason": "Domain also contains several correctly encapsulated aggregates."
    }
  ]
}
```

All 12 ids belong in a complete report. The calculator treats omitted ids as unknown
and rejects duplicate/foreign ids. Status values are `pass`, `partial`, `fail`,
`unknown`, `not-applicable`. Pass/partial/fail need concrete evidence. N/A needs a
reason and evidence of actual inapplicability. Unknown needs an explanation when
explicitly supplied. Partial means some criterion obligations are met and others
are evidenced gaps; do not use it to conceal unknowns.

Weights total 100. Credit: pass=1, partial=0.5, fail=0. Let A be applicable weight
(all except justified N/A), K known assessed weight (pass/partial/fail), E earned
weight. **Observed score = 100×E/K**, **evidence coverage = 100×K/A**.
Also show **conservative score = 100×E/A**, treating unknowns as zero for this
lower bound only. With no assessed rules the observed score is unavailable; with
no applicable rules there is no score. Every result below full coverage is provisional.
Show template and readiness subtotals; neither is a production approval gate.

Do not compare scores from different profiles/standard versions or materially
different coverage without those qualifications. A score is an aid to prioritization,
not an objective measurement of all software quality. Approved exceptions remain
visible and do not inflate credit.

Each finding includes rule id (or `GENERAL` for issues outside the catalog), severity
(critical/high/medium/low), file and line/symbol, evidence, impact, recommendation,
confidence and any accepted exception. Critical/high defects remain prominent even
with a high average. Label hypotheses and distinguish absent tests from unrun tests.

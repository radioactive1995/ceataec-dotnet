#!/usr/bin/env python3
"""Calculate evidence-aware scores. Reads JSON on stdin, emits JSON on stdout."""
import json
from pathlib import Path
import sys

CATALOG = Path(__file__).resolve().parents[1] / "spec" / "rules.json"
CREDIT = {"pass": 1, "partial": 0.5, "fail": 0}


def calculate(document, catalog=None):
    catalog = catalog or json.loads(CATALOG.read_text(encoding="utf-8"))
    if not isinstance(document, dict):
        raise ValueError("assessment must be an object")
    if document.get("standardVersion") != catalog["version"]:
        raise ValueError("standardVersion does not match the bundled catalog")
    if document.get("profile") != catalog["profile"]:
        raise ValueError("profile does not match the bundled catalog")
    if not isinstance(document.get("target"), str) or not document["target"].strip():
        raise ValueError("target is required")
    rules = {rule["id"]: rule for rule in catalog["rules"]}
    entries = document.get("assessments")
    if not isinstance(entries, list):
        raise ValueError("assessments must be a list")
    assessed = {}
    for item in entries:
        if not isinstance(item, dict):
            raise ValueError("each assessment must be an object")
        rule_id, status = item.get("id"), item.get("status")
        if rule_id not in rules or rule_id in assessed:
            raise ValueError(f"unknown or duplicate rule id: {rule_id}")
        if status not in {*CREDIT, "unknown", "not-applicable"}:
            raise ValueError(f"invalid status for {rule_id}")
        evidence = item.get("evidence", [])
        if not isinstance(evidence, list) or any(
            not isinstance(value, str) or not value.strip() for value in evidence
        ):
            raise ValueError(f"evidence must contain nonempty strings: {rule_id}")
        if status != "unknown" and not evidence:
            raise ValueError(f"evidence required for {rule_id}")
        if status in {"unknown", "not-applicable", "partial", "fail"} and (
            not isinstance(item.get("reason"), str) or not item["reason"].strip()
        ):
            raise ValueError(f"reason required for {rule_id}")
        # Assessment input cannot change the catalog's weights, kind or criterion.
        assessed[rule_id] = {"status": status, "evidence": evidence,
                             "reason": item.get("reason", "")}

    rows = [{**rule, **assessed.get(rule["id"], {
        "status": "unknown", "evidence": [], "reason": "Not assessed"
    })} for rule in catalog["rules"]]

    def subtotal(selected):
        applicable = sum(r["weight"] for r in selected if r["status"] != "not-applicable")
        known = sum(r["weight"] for r in selected if r["status"] in CREDIT)
        earned = sum(r["weight"] * CREDIT.get(r["status"], 0) for r in selected)
        return {
            "observedScore": round(100 * earned / known, 1) if known else None,
            "evidenceCoverage": round(100 * known / applicable, 1) if applicable else None,
            "conservativeScore": round(100 * earned / applicable, 1) if applicable else None,
            "applicableWeight": applicable, "assessedWeight": known,
            "provisional": known != applicable or not applicable,
        }

    return {
        "standardVersion": catalog["version"], "profile": catalog["profile"],
        "target": document["target"], **subtotal(rows),
        "byKind": {kind: subtotal([r for r in rows if r["kind"] == kind])
                   for kind in ("template", "readiness")},
        "rules": rows,
    }


def main():
    try:
        result = calculate(json.load(sys.stdin))
    except (ValueError, TypeError, KeyError) as error:
        print(f"Invalid assessment: {error}", file=sys.stderr)
        return 2
    print(json.dumps(result, indent=2))
    return 0


if __name__ == "__main__":
    sys.exit(main())

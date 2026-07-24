#!/usr/bin/env python3
"""Mechanical gate: every status=perfect package must satisfy:
  (a) zero findings with reassessment in {needs_manual, open_unverified, open, still_open}
      at ANY severity
  (b) every public static method in src/ is named in the package's test project
      OR listed in package row deferred_apis
  (c) has_tests is true (real .cs tests exist)

Exit 0 if all perfect packages pass; else print failures and exit 1.
"""
from __future__ import annotations

import json
import os
import re
import sys
from collections import defaultdict
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
STATUS_PATH = ROOT / "scratch" / "PACKAGE_STATUS.json"
FINDINGS_PATH = ROOT / "scratch" / "findings_reassessment.json"
OPEN = {"needs_manual", "open_unverified", "open", "still_open", "needs_review"}


def has_real_tests(pkg: str) -> bool:
    d = ROOT / "test" / f"{pkg}.Tests"
    if not d.is_dir():
        return False
    for dp, _, fs in os.walk(d):
        if "/obj/" in dp or "/bin/" in dp or "\\obj\\" in dp or "\\bin\\" in dp:
            continue
        for f in fs:
            if f.endswith(".cs") and "AssemblyInfo" not in f:
                return True
    return False


def public_static_methods(pkg: str) -> set[str]:
    src = ROOT / "src" / pkg
    names: set[str] = set()
    if not src.is_dir():
        return names
    # Match: public static ... Name(
    pat = re.compile(
        r"public\s+static\s+(?:unsafe\s+)?(?:[\w.<>\[\],\s]+\s+)+([A-Za-z_]\w*)\s*\("
    )
    skip = {
        "if",
        "for",
        "while",
        "switch",
        "using",
        "where",
        "class",
        "struct",
        "get",
        "set",
        "operator",
    }
    for dp, _, fs in os.walk(src):
        if "/obj/" in dp or "/bin/" in dp:
            continue
        for f in fs:
            if not f.endswith(".cs") or "AssemblyInfo" in f:
                continue
            text = (Path(dp) / f).read_text(encoding="utf-8", errors="replace")
            for m in pat.finditer(text):
                name = m.group(1)
                if name not in skip:
                    names.add(name)
    return names


def test_text(pkg: str) -> str:
    d = ROOT / "test" / f"{pkg}.Tests"
    parts: list[str] = []
    if not d.is_dir():
        return ""
    for dp, _, fs in os.walk(d):
        if "/obj/" in dp or "/bin/" in dp:
            continue
        for f in fs:
            if f.endswith(".cs") and "AssemblyInfo" not in f:
                parts.append((Path(dp) / f).read_text(encoding="utf-8", errors="replace"))
    return "\n".join(parts)


def main() -> int:
    status = json.loads(STATUS_PATH.read_text(encoding="utf-8"))
    findings = []
    if FINDINGS_PATH.is_file():
        findings = json.loads(FINDINGS_PATH.read_text(encoding="utf-8"))

    open_by_pkg: dict[str, list] = defaultdict(list)
    for f in findings:
        st = f.get("reassessment") or f.get("status") or ""
        if st in OPEN:
            pkg = f.get("package")
            if not pkg:
                m = re.search(r"src/(IAFahim\.[^/]+)/", f.get("file", ""))
                pkg = m.group(1) if m else None
            if pkg:
                open_by_pkg[pkg].append(f)

    failures: list[tuple[str, str]] = []
    perfect_rows = [r for r in status if r.get("status") == "perfect"]

    for row in perfect_rows:
        pkg = row["package"]
        reasons: list[str] = []

        if not has_real_tests(pkg) and not row.get("has_tests"):
            reasons.append("gate: has_tests false / no real test .cs")
        elif not has_real_tests(pkg):
            reasons.append("gate: no real test .cs sources")

        open_fs = open_by_pkg.get(pkg, [])
        if open_fs:
            reasons.append(f"gate: open findings ({len(open_fs)})")

        deferred_apis = set(row.get("deferred_apis") or [])
        methods = public_static_methods(pkg)
        ttext = test_text(pkg)
        untested = sorted(m for m in methods if m not in deferred_apis and m not in ttext)
        # Operators / implicit helpers often false positives; still report if any
        if untested:
            show = ", ".join(untested[:12])
            if len(untested) > 12:
                show += f" (+{len(untested) - 12})"
            reasons.append(f"gate: public API untested: {show}")

        if reasons:
            failures.append((pkg, "; ".join(reasons)))

    if not failures:
        print(f"GATE PASS: {len(perfect_rows)} perfect packages clean")
        return 0

    print(f"GATE FAIL: {len(failures)} / {len(perfect_rows)} perfect packages")
    for pkg, reason in failures:
        print(f"  {pkg}: {reason}")
    return 1


if __name__ == "__main__":
    sys.exit(main())

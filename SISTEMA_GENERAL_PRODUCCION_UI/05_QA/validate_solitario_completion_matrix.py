#!/usr/bin/env python3
"""Valida estructura y cierre de la matriz global de Solitario."""

from __future__ import annotations

import argparse
import json
from pathlib import Path


EXPECTED = {
    "Klondike", "FreeCell", "Spider", "Pyramid", "TriPeaks", "Golf",
    "Yukon", "Canfield", "FortyThieves", "Scorpion", "Russian",
}
ALLOWED = {"PASS", "PENDING"}


def validate(path: Path) -> tuple[list[str], int, int]:
    data = json.loads(path.read_text(encoding="utf-8"))
    failures: list[str] = []
    rows = data.get("modalities", [])
    required = data.get("required_dimensions", [])
    ids = [row.get("id") for row in rows]
    if len(ids) != len(set(ids)):
        failures.append("hay modalidades duplicadas")
    if set(ids) != EXPECTED:
        failures.append(f"modalidades esperadas={sorted(EXPECTED)} obtenidas={sorted(set(ids))}")
    if data.get("expected_modalities") != len(EXPECTED):
        failures.append("expected_modalities no coincide con el catálogo autoritativo")
    complete = 0
    for row in rows:
        dimensions = row.get("dimensions", {})
        missing = sorted(set(required) - set(dimensions))
        extra = sorted(set(dimensions) - set(required))
        if missing or extra:
            failures.append(f"{row.get('id')}: dimensiones faltantes={missing} extra={extra}")
            continue
        invalid = {key: value for key, value in dimensions.items() if value not in ALLOWED}
        if invalid:
            failures.append(f"{row.get('id')}: estados inválidos={invalid}")
        if row.get("free_theme_count", -1) < 2 and dimensions.get("two_free_themes") == "PASS":
            failures.append(f"{row.get('id')}: dos temas gratuitos en PASS con conteo menor a 2")
        if row.get("premium_theme_count", -1) < 6 and dimensions.get("premium_target") == "PASS":
            failures.append(f"{row.get('id')}: objetivo premium en PASS con conteo menor a 6")
        if all(dimensions.get(key) == "PASS" for key in required):
            complete += 1
    return failures, complete, len(rows)


def main() -> int:
    default = Path(__file__).resolve().parents[1] / "09_PROYECTOS" / "SOLITARIO" / "MATRIZ_COMPLETITUD_GLOBAL.json"
    parser = argparse.ArgumentParser()
    parser.add_argument("matrix", nargs="?", type=Path, default=default)
    parser.add_argument("--structure-only", action="store_true")
    args = parser.parse_args()
    failures, complete, total = validate(args.matrix)
    if failures:
        print(f"SOLITARIO COMPLETION MATRIX FAIL - {len(failures)} error(es)")
        for failure in failures:
            print(f"- {failure}")
        return 1
    if args.structure_only:
        print(f"SOLITARIO COMPLETION MATRIX STRUCTURE PASS ({total}/{len(EXPECTED)}) - cierre {complete}/{total}")
        return 0
    if complete != len(EXPECTED):
        print(f"SOLITARIO COMPLETION MATRIX BLOCKED ({complete}/{len(EXPECTED)})")
        return 1
    print(f"SOLITARIO COMPLETION MATRIX PASS ({complete}/{len(EXPECTED)})")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

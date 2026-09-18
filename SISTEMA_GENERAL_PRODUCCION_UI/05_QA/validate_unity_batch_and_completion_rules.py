#!/usr/bin/env python3
"""Defensa documental de Unity batch, capturas, cierre y reanudación."""

from __future__ import annotations

import csv
import json
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]


def require_text(relative: str, fragments: list[str], failures: list[str]) -> str:
    path = ROOT / relative
    if not path.is_file():
        failures.append(f"falta {relative}")
        return ""
    text = path.read_text(encoding="utf-8")
    for fragment in fragments:
        if fragment not in text:
            failures.append(f"{relative}: falta defensa {fragment!r}")
    return text


def main() -> int:
    failures: list[str] = []
    require_text(
        "03_GUIAS_TECNICAS/UNITY_UGUI/GUIA_UNITY_UGUI.md",
        ["`Unity.exe`", "-noUpm", "-quit` con `-runTests", "-nographics", "Test-UnityBatchPreflight.ps1"],
        failures,
    )
    require_text(
        "05_QA/PLAN_MAESTRO_QA_UI.md",
        ["gris, vacío o de baja información", "matriz enumera todas", "Al reanudar después de una pausa"],
        failures,
    )
    require_text(
        "06_CATALOGO_APRENDIZAJES/ANTIPATRONES_Y_ERRORES.md",
        ["otra instancia", "flags batch incompatibles", "captura porque existe", "subtotal", "Reanudar tras una pausa"],
        failures,
    )
    solitaire = require_text(
        "09_PROYECTOS/SOLITARIO/README.md",
        ["## Matriz de completitud global", "PASS (11/11)", "protocolo de reanudación"],
        failures,
    )
    for name in ("Klondike", "FreeCell", "Spider", "Pyramid", "TriPeaks", "Golf", "Yukon", "Canfield", "Forty Thieves", "Scorpion", "Russian"):
        if name not in solitaire:
            failures.append(f"README de Solitario no enumera {name}")

    preflight = require_text(
        "05_QA/Test-UnityBatchPreflight.ps1",
        ["Unity.exe", "Unity Hub.exe", "-runTests", "UnityLockfile", "BLOCKED"],
        failures,
    )
    for forbidden in ("Stop-Process", "taskkill"):
        if forbidden in preflight:
            failures.append(f"preflight destructivo: contiene {forbidden}")

    for relative in ("05_QA/validate_capture_information.py", "05_QA/validate_solitario_completion_matrix.py"):
        if not (ROOT / relative).is_file():
            failures.append(f"falta validador ejecutable {relative}")

    catalog_path = ROOT / "06_CATALOGO_APRENDIZAJES" / "CATALOGO_APRENDIZAJES.csv"
    with catalog_path.open(encoding="utf-8", newline="") as handle:
        rows = list(csv.DictReader(handle))
    ids = [row.get("id", "") for row in rows]
    for expected in ("UI-GEN-050", "UI-GEN-051", "UI-GEN-052", "UI-GEN-053", "UI-GEN-054"):
        if ids.count(expected) != 1:
            failures.append(f"catálogo: {expected} aparece {ids.count(expected)} veces")

    matrix_path = ROOT / "09_PROYECTOS" / "SOLITARIO" / "MATRIZ_COMPLETITUD_GLOBAL.json"
    matrix = json.loads(matrix_path.read_text(encoding="utf-8"))
    rows_by_id = {row["id"]: row for row in matrix["modalities"]}
    free_complete = sum(row["dimensions"]["two_free_themes"] == "PASS" for row in rows_by_id.values())
    if free_complete != 9:
        failures.append(f"snapshot: se esperaban 9 modalidades con dos temas gratuitos y hay {free_complete}")
    for pending in ("Scorpion", "Russian"):
        if rows_by_id[pending]["dimensions"]["two_free_themes"] != "PENDING":
            failures.append(f"snapshot: {pending} debe bloquear el contenido gratuito")

    if failures:
        print(f"UNITY BATCH AND COMPLETION RULES FAIL - {len(failures)} error(es)")
        for failure in failures:
            print(f"- {failure}")
        return 1
    print("UNITY BATCH AND COMPLETION RULES PASS - 5 defensas y matriz 11/11 presentes")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

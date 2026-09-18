#!/usr/bin/env python3
"""Comprueba referencias locales explícitas del paquete documental."""

from __future__ import annotations

import argparse
import re
from pathlib import Path


SCRIPT = Path(__file__).resolve()
PACKAGE_ROOT = SCRIPT.parents[1]
WORKSPACE_ROOT = PACKAGE_ROOT.parents[1]
SKILL_ROOT = PACKAGE_ROOT / "10_SKILL" / "puerta-aceptacion"
KNOWN_ROOTS = {
    "00_INICIO",
    "01_PRINCIPIOS",
    "02_FLUJO_DE_TRABAJO",
    "03_GUIAS_TECNICAS",
    "04_PLANTILLAS",
    "05_QA",
    "06_CATALOGO_APRENDIZAJES",
    "07_CASOS_HISTORICOS",
    "08_INVESTIGACION_Y_FUENTES",
    "09_PROYECTOS",
    "10_SKILL",
}
EXTENSIONS = {".md", ".txt", ".csv", ".json", ".py", ".ps1", ".yaml", ".yml"}


def candidates(document: Path, raw: str) -> list[Path]:
    value = raw.strip().replace("\\", "/")
    if not value or "\n" in value or any(marker in value for marker in ("*", "{", "}")):
        return []
    if value.startswith(("http://", "https://")):
        return []
    if Path(value).suffix.lower() not in EXTENSIONS:
        return []
    if value.startswith("Logs/") or value.startswith("Tools/"):
        return [WORKSPACE_ROOT / value]
    first = value.split("/", 1)[0]
    if first in KNOWN_ROOTS or value == "README.md":
        return [PACKAGE_ROOT / value]
    if value.startswith(("scripts/", "references/", "agents/")):
        return [SKILL_ROOT / value]
    return [document.parent / value]


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument(
        "--document",
        action="append",
        default=[],
        help="Ruta relativa al paquete; puede repetirse para validar sólo el alcance cambiado.",
    )
    args = parser.parse_args()
    failures: list[str] = []
    if args.document:
        documents = [PACKAGE_ROOT / value for value in args.document]
        missing_documents = [path for path in documents if not path.is_file()]
        if missing_documents:
            print(f"PACKAGE LINKS FAIL - {len(missing_documents)} documento(s) de alcance ausente(s)")
            for document in missing_documents:
                print(f"- falta {document}")
            return 1
    else:
        documents = sorted(PACKAGE_ROOT.rglob("*.md")) + sorted(PACKAGE_ROOT.rglob("*.txt"))
    for document in documents:
        text = document.read_text(encoding="utf-8")
        refs = re.findall(r"`([^`]+)`", text)
        refs += re.findall(r"\[[^\]]+\]\(([^)]+)\)", text)
        for raw in refs:
            for candidate in candidates(document, raw):
                if not candidate.exists():
                    relative = document.relative_to(PACKAGE_ROOT)
                    failures.append(f"{relative}: falta {raw} -> {candidate}")
    if failures:
        print(f"PACKAGE LINKS FAIL - {len(failures)} referencia(s) ausente(s)")
        for failure in failures:
            print(f"- {failure}")
        return 1
    print(f"PACKAGE LINKS PASS - {len(documents)} documento(s) revisado(s)")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

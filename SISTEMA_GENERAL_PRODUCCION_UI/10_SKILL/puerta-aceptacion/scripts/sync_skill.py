#!/usr/bin/env python3
"""Sincroniza la skill canónica con una instalación sin borrar archivos ajenos."""

from __future__ import annotations

import argparse
import hashlib
import shutil
import sys
from pathlib import Path


MANAGED_FILES = (
    Path("SKILL.md"),
    Path("agents/openai.yaml"),
    Path("references/CRITERIOS_UNIVERSALES.md"),
    Path("scripts/validate_gate.py"),
    Path("scripts/sync_skill.py"),
)


def digest(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def compare(source: Path, destination: Path) -> list[str]:
    problems: list[str] = []
    for relative in MANAGED_FILES:
        src = source / relative
        dst = destination / relative
        if not src.is_file():
            problems.append(f"FALTA FUENTE: {src}")
        elif not dst.is_file():
            problems.append(f"FALTA DESTINO: {dst}")
        elif digest(src) != digest(dst):
            problems.append(f"DIVERGE: {relative}")
    return problems


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--destination", required=True, type=Path)
    parser.add_argument("--check", action="store_true", help="Sólo compara; no escribe.")
    args = parser.parse_args()

    source = Path(__file__).resolve().parents[1]
    destination = args.destination.resolve()

    if source == destination:
        print("ERROR: fuente y destino son la misma carpeta.")
        return 2

    if not args.check:
        for relative in MANAGED_FILES:
            src = source / relative
            if not src.is_file():
                print(f"ERROR: falta el archivo canónico {src}")
                return 2
            dst = destination / relative
            dst.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(src, dst)

    problems = compare(source, destination)
    if problems:
        for problem in problems:
            print(problem)
        print("SKILL SYNC FAIL")
        return 1

    print(f"SKILL SYNC PASS - {len(MANAGED_FILES)} archivos administrados")
    return 0


if __name__ == "__main__":
    sys.exit(main())

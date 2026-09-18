#!/usr/bin/env python3
"""Valida el enrutamiento documental y la sincronización de la skill."""

from __future__ import annotations

import argparse
import hashlib
import sys
from pathlib import Path


SKILL_FILES = (
    Path("SKILL.md"),
    Path("agents/openai.yaml"),
    Path("references/CRITERIOS_UNIVERSALES.md"),
    Path("scripts/validate_gate.py"),
    Path("scripts/sync_skill.py"),
)


def read(path: Path) -> str:
    return path.read_text(encoding="utf-8-sig")


def sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--global-agents", required=True, type=Path)
    parser.add_argument("--project-root", required=True, type=Path)
    parser.add_argument("--installed-skill", required=True, type=Path)
    parser.add_argument("--output", type=Path)
    args = parser.parse_args()

    system_root = Path(__file__).resolve().parents[1]
    canonical_skill = system_root / "10_SKILL/puerta-aceptacion"
    checks: list[tuple[str, bool, str]] = []

    global_text = read(args.global_agents)
    project_agents = read(args.project_root / "AGENTS.md")
    continuity = read(args.project_root / "REGLAS_DE_TRABAJO_Y_CONTINUIDAD.md")
    map_text = read(system_root / "00_INICIO/MAPA_DE_USO.md")
    readme_text = read(system_root / "README.md")
    router_text = read(system_root / "00_INICIO/ENRUTADOR_DE_PROYECTOS_Y_TAREAS.md")
    inventory_text = read(system_root / "00_INICIO/INVENTARIO_DE_CARPETAS.md")
    chat_text = read(system_root / "00_INICIO/COMO_CONTINUAR_EN_OTRO_CHAT.md")
    catalog_text = read(system_root / "06_CATALOGO_APRENDIZAJES/README.md")
    antipatterns_text = read(system_root / "06_CATALOGO_APRENDIZAJES/ANTIPATRONES_Y_ERRORES.md")
    cases_text = read(system_root / "07_CASOS_HISTORICOS/README.md")
    qf_chat_text = read(system_root / "09_PROYECTOS/QUANTUM_FORGE/00_INICIO/COMO_CONTINUAR_EN_OTRO_CHAT.txt")

    checks.extend(
        [
            ("GLOBAL01", "# Alcance y autoridad" in global_text, "La personalización global separa alcance y autoridad."),
            ("GLOBAL02", "`preflight`" not in global_text and "`unit`" not in global_text and "`final`" not in global_text, "La personalización no duplica el procedimiento de la puerta."),
            ("GLOBAL03", "no significa leerla completa" in global_text, "La activación global prohíbe leer toda la carpeta por rutina."),
            ("GLOBAL04", "# Resúmenes y continuidad" in global_text and "último bloque validado" in global_text and "siguiente paso recomendado" in global_text, "La personalización define el resumen compacto de continuidad."),
            ("PROJECT01", "Para una corrección pequeña y localizada" in project_agents, "Quantum Forge permite lectura proporcional al alcance."),
            ("PROJECT02", "## Puerta de aceptación" in project_agents, "Quantum Forge enlaza la puerta sin copiar sus criterios."),
            ("PROJECT03", "ENRUTADOR_DE_PROYECTOS_Y_TAREAS.md" in project_agents, "Quantum Forge activa el enrutador común."),
            ("CONT01", "Estas reglas deben viajar completas" not in continuity, "Los resúmenes ya no transportan copias completas de las reglas."),
            ("CONT02", "reproducidas dentro del propio resumen" not in continuity, "Los resúmenes transfieren estado y rutas canónicas."),
            ("ROUTE01", (system_root / "00_INICIO/FUENTES_DE_INSTRUCCIONES.md").is_file(), "Existe un mapa de autoridad y responsabilidades."),
            ("ROUTE02", "## Procesar una tanda de hallazgos de prueba" in map_text, "Existe una ruta para tandas de QA."),
            ("ROUTE03", "## Cerrar un hito o cambiar de chat" in map_text, "Existe una ruta compacta de continuidad."),
            ("ROUTE04", all((system_root / f"00_INICIO/{name}").is_file() for name in ("INVENTARIO_DE_CARPETAS.md", "ENRUTADOR_DE_PROYECTOS_Y_TAREAS.md", "COMO_CONTINUAR_EN_OTRO_CHAT.md")), "Existen las tres entradas del enrutamiento universal."),
            ("ROUTE05", "no significa leer la carpeta completa" in router_text.lower(), "El enrutador define lectura selectiva, no total."),
            ("ROUTE06", all(f"`{i:02d}_" in inventory_text for i in range(11)), "El inventario describe las once áreas principales."),
            ("ROUTE07", "no exigir tablas" in map_text.lower(), "La lista natural numerada del usuario es una entrada válida."),
            ("ROUTE08", "proyectos, juegos y UI" in readme_text, "El sistema declara su alcance universal y su especialización madura."),
            ("LEARN01", "no se lee completo" in catalog_text.lower(), "El catálogo se consulta por coincidencias y no completo."),
            ("LEARN02", "no se leen todos" in cases_text.lower(), "Los casos históricos sólo se abren por relevancia."),
            ("LEARN03", "no exige" in antipatterns_text.lower() and "tabla completa" in antipatterns_text.lower(), "Los antipatrones se filtran por riesgo antes de leerlos."),
            ("CHAT01", "no toda la carpeta" in chat_text.lower(), "La guía general de continuidad activa lectura selectiva."),
            ("CHAT02", "mediante su enrutador" in qf_chat_text.lower() and "completo en" not in qf_chat_text.lower(), "Quantum Forge continúa por enrutamiento sin lectura total."),
            ("STATE01", (system_root / "04_PLANTILLAS/ESTADO_PROYECTO.md").is_file(), "Existe plantilla separada para estado cambiante."),
            ("STATE02", (system_root / "04_PLANTILLAS/REGISTRO_DECISIONES.md").is_file(), "Existe plantilla separada para decisiones aprobadas."),
        ]
    )

    for relative in SKILL_FILES:
        source = canonical_skill / relative
        installed = args.installed_skill / relative
        equal = source.is_file() and installed.is_file() and sha256(source) == sha256(installed)
        checks.append((f"SKILL:{relative.as_posix()}", equal, "La copia instalada coincide con la fuente canónica."))

    lines = []
    failed = False
    for check_id, passed, description in checks:
        status = "PASS" if passed else "FAIL"
        lines.append(f"{status} {check_id} - {description}")
        failed = failed or not passed
    lines.append("INSTRUCTION SYSTEM PASS" if not failed else "INSTRUCTION SYSTEM FAIL")
    report = "\n".join(lines) + "\n"

    if args.output:
        args.output.parent.mkdir(parents=True, exist_ok=True)
        args.output.write_text(report, encoding="utf-8")
    print(report, end="")
    return 1 if failed else 0


if __name__ == "__main__":
    sys.exit(main())

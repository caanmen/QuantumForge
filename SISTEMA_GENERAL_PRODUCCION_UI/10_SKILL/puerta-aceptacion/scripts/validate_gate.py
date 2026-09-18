#!/usr/bin/env python3
"""Crea y valida registros de la puerta universal de aceptación.

Sólo usa la biblioteca estándar. Devuelve 0 para PASS y 1 para una puerta bloqueada.
"""

from __future__ import annotations

import argparse
import json
import sys
from datetime import datetime, timezone
from pathlib import Path
from typing import Any
from urllib.parse import urlparse


SCHEMA_VERSION = "1.0"
PHASES = {
    "preflight": ("U01", "U02", "U03", "U04"),
    "unit": ("U01", "U02", "U03", "U04", "U05", "U06", "U07"),
    "final": tuple(f"U{i:02d}" for i in range(1, 13)),
}
CONDITIONAL = {"U05", "U06", "U09", "U12"}
ALWAYS_PASS = set(PHASES["final"]) - CONDITIONAL
VALID_STATUS = {"PASS", "N/A", "PENDING", "FAIL", "BLOCKED"}
PLACEHOLDERS = {"todo", "tbd", "pendiente", "completar", "placeholder", "n/a"}
VISUAL_LAYER_MODULE = "visual_layer_composition"
VISUAL_LAYER_CRITERION = "VLC01"

CRITERIA = {
    "U01": "Alcance, versión y condición observable de terminado definidos",
    "U02": "Autoridad, instrucciones, módulos y exclusiones identificados",
    "U03": "Estado real inspeccionado y evidencia anterior separada",
    "U04": "Fuentes definitivas y propietarios únicos declarados",
    "U05": "Unidad de mayor riesgo aprobada antes del lote o paso costoso",
    "U06": "Datos exactos compuestos y validados de forma determinista",
    "U07": "Comprobaciones estructurales o funcionales proporcionales en PASS",
    "U08": "Resultado revisado en el contexto final representativo",
    "U09": "Colección inventariada y cada unidad o celda verificada",
    "U10": "Invariantes y regresiones relevantes verificadas",
    "U11": "Evidencia final vigente y posterior al último cambio",
    "U12": "Acción externa o irreversible autorizada y verificada",
}


def now_iso() -> str:
    return datetime.now(timezone.utc).isoformat(timespec="seconds")


def parse_time(value: Any, field: str, errors: list[str]) -> datetime | None:
    if not isinstance(value, str) or not value.strip():
        errors.append(f"{field}: falta una fecha ISO 8601")
        return None
    try:
        parsed = datetime.fromisoformat(value.replace("Z", "+00:00"))
    except ValueError:
        errors.append(f"{field}: fecha ISO 8601 inválida: {value!r}")
        return None
    if parsed.tzinfo is None:
        errors.append(f"{field}: la fecha debe incluir zona horaria")
        return None
    return parsed.astimezone(timezone.utc)


def meaningful(value: Any, minimum: int = 3) -> bool:
    if not isinstance(value, str) or len(value.strip()) < minimum:
        return False
    lowered = value.strip().lower()
    return lowered not in PLACEHOLDERS and not any(
        lowered.startswith(f"{word}:") for word in PLACEHOLDERS
    )


def make_template(project: str, deliverable: str, workspace_root: Path) -> dict[str, Any]:
    created = now_iso()
    return {
        "schema_version": SCHEMA_VERSION,
        "project": project,
        "deliverable": deliverable,
        "version": "working",
        "completion_definition": "PENDING",
        "created_at": created,
        "latest_change_at": created,
        "workspace_root": str(workspace_root.resolve()),
        "sources": [],
        "modules_reviewed": False,
        "applicable_modules": [],
        "excluded_modules": [],
        "baseline": [],
        "owners": [],
        "criteria": [
            {
                "id": criterion_id,
                "status": "PENDING",
                "reason": "",
                "evidence": [],
            }
            for criterion_id in PHASES["final"]
        ],
        "additional_criteria": [],
        "open_issues": [],
    }


def resolve_root(data: dict[str, Any], manifest_path: Path) -> Path:
    raw = data.get("workspace_root", ".")
    root = Path(raw)
    if not root.is_absolute():
        root = manifest_path.parent / root
    return root.resolve()


def resolve_evidence_path(raw: Any, root: Path) -> Path | None:
    if not meaningful(raw):
        return None
    path = Path(str(raw))
    return path.resolve() if path.is_absolute() else (root / path).resolve()


def is_within(path: Path, root: Path) -> bool:
    try:
        path.relative_to(root)
        return True
    except ValueError:
        return False


def validate_evidence(
    criterion_id: str,
    item: Any,
    index: int,
    root: Path,
    latest_change: datetime | None,
    errors: list[str],
) -> None:
    label = f"{criterion_id}.evidence[{index}]"
    if not isinstance(item, dict):
        errors.append(f"{label}: debe ser un objeto")
        return

    evidence_type = item.get("type")
    if evidence_type not in {"file", "command", "url", "observation"}:
        errors.append(f"{label}.type: use file, command, url u observation")
        return
    if not meaningful(item.get("description"), 10):
        errors.append(f"{label}.description: describa concretamente qué demuestra")

    created = parse_time(item.get("created_at"), f"{label}.created_at", errors)
    if criterion_id == "U11" and created and latest_change and created < latest_change:
        errors.append(f"{label}: la evidencia es anterior a latest_change_at")

    if evidence_type == "file":
        path = resolve_evidence_path(item.get("path"), root)
        if path is None:
            errors.append(f"{label}.path: falta una ruta válida")
        elif not is_within(path, root):
            errors.append(f"{label}.path: la evidencia debe archivarse dentro de workspace_root")
        elif not path.is_file():
            errors.append(f"{label}.path: no existe el archivo {path}")
    elif evidence_type == "command":
        if not meaningful(item.get("command"), 4):
            errors.append(f"{label}.command: falta un comando reproducible")
        if item.get("result") != "PASS":
            errors.append(f"{label}.result: debe ser PASS")
        output = resolve_evidence_path(item.get("output_file"), root)
        if output is None:
            errors.append(f"{label}.output_file: falta la salida guardada")
        elif not is_within(output, root):
            errors.append(f"{label}.output_file: la salida debe estar dentro de workspace_root")
        elif not output.is_file():
            errors.append(f"{label}.output_file: no existe el archivo {output}")
    elif evidence_type == "url":
        raw_url = item.get("url")
        parsed = urlparse(raw_url) if isinstance(raw_url, str) else None
        if not parsed or parsed.scheme not in {"http", "https"} or not parsed.netloc:
            errors.append(f"{label}.url: enlace http/https inválido")
    elif evidence_type == "observation":
        if not meaningful(item.get("context"), 8):
            errors.append(f"{label}.context: indique dónde y cómo se revisó")


def validate_manifest(data: Any, manifest_path: Path, phase: str) -> list[str]:
    errors: list[str] = []
    if not isinstance(data, dict):
        return ["La raíz del registro debe ser un objeto JSON"]
    if data.get("schema_version") != SCHEMA_VERSION:
        errors.append(f"schema_version: se requiere {SCHEMA_VERSION}")

    for field in ("project", "deliverable", "version"):
        if not meaningful(data.get(field)):
            errors.append(f"{field}: falta un valor concreto")
    if not meaningful(data.get("completion_definition"), 12):
        errors.append("completion_definition: falta una condición observable de terminado")

    parse_time(data.get("created_at"), "created_at", errors)
    latest_change = parse_time(data.get("latest_change_at"), "latest_change_at", errors)
    root = resolve_root(data, manifest_path)
    if not root.is_dir():
        errors.append(f"workspace_root: no existe el directorio {root}")

    sources = data.get("sources")
    if not isinstance(sources, list) or not sources or not all(meaningful(v, 4) for v in sources):
        errors.append("sources: registre al menos una fuente o instrucción vigente")
    if data.get("modules_reviewed") is not True:
        errors.append("modules_reviewed: debe ser true después de revisar aplicabilidad")
    for field in (
        "applicable_modules",
        "excluded_modules",
        "baseline",
        "owners",
        "additional_criteria",
        "open_issues",
    ):
        if not isinstance(data.get(field), list):
            errors.append(f"{field}: debe ser una lista")

    for index, exclusion in enumerate(data.get("excluded_modules", [])):
        if not isinstance(exclusion, dict) or not meaningful(exclusion.get("module")):
            errors.append(f"excluded_modules[{index}]: falta module")
        elif not meaningful(exclusion.get("reason"), 10):
            errors.append(f"excluded_modules[{index}].reason: falta una razón concreta")

    criteria_raw = data.get("criteria")
    if not isinstance(criteria_raw, list):
        return errors + ["criteria: debe ser una lista"]
    criteria: dict[str, dict[str, Any]] = {}
    for index, criterion in enumerate(criteria_raw):
        if not isinstance(criterion, dict):
            errors.append(f"criteria[{index}]: debe ser un objeto")
            continue
        criterion_id = criterion.get("id")
        if criterion_id not in CRITERIA:
            errors.append(f"criteria[{index}].id: ID desconocido {criterion_id!r}")
            continue
        if criterion_id in criteria:
            errors.append(f"criteria: ID duplicado {criterion_id}")
            continue
        criteria[criterion_id] = criterion

    required_ids = PHASES[phase]
    for criterion_id in required_ids:
        criterion = criteria.get(criterion_id)
        if criterion is None:
            errors.append(f"{criterion_id}: falta el criterio universal")
            continue
        status = criterion.get("status")
        if status not in VALID_STATUS:
            errors.append(f"{criterion_id}.status: estado inválido {status!r}")
            continue
        if criterion_id in ALWAYS_PASS and status != "PASS":
            errors.append(f"{criterion_id}: requiere PASS y está en {status}")
            continue
        if criterion_id in CONDITIONAL and status not in {"PASS", "N/A"}:
            errors.append(f"{criterion_id}: requiere PASS o N/A razonado y está en {status}")
            continue
        if status == "N/A":
            if criterion_id not in CONDITIONAL:
                errors.append(f"{criterion_id}: N/A no está permitido")
            if not meaningful(criterion.get("reason"), 10):
                errors.append(f"{criterion_id}.reason: N/A requiere una razón concreta")
            continue

        evidence = criterion.get("evidence")
        if not isinstance(evidence, list) or not evidence:
            errors.append(f"{criterion_id}.evidence: PASS requiere evidencia")
            continue
        for index, item in enumerate(evidence):
            validate_evidence(criterion_id, item, index, root, latest_change, errors)
        evidence_types = {item.get("type") for item in evidence if isinstance(item, dict)}
        if criterion_id == "U06" and "command" not in evidence_types:
            errors.append("U06.evidence: los datos exactos requieren al menos una prueba automática")

    phase_order = {"preflight": 0, "unit": 1, "final": 2}
    additional_criteria = data.get("additional_criteria", [])
    additional_by_id: dict[str, list[dict[str, Any]]] = {}
    for index, criterion in enumerate(additional_criteria):
        label = f"additional_criteria[{index}]"
        if not isinstance(criterion, dict):
            errors.append(f"{label}: debe ser un objeto")
            continue
        if not meaningful(criterion.get("id")) or not meaningful(criterion.get("description"), 10):
            errors.append(f"{label}: requiere id y description concretos")
            continue
        additional_by_id.setdefault(str(criterion.get("id")), []).append(criterion)
        criterion_phase = criterion.get("phase", "final")
        if criterion_phase not in phase_order:
            errors.append(f"{label}.phase: use preflight, unit o final")
            continue
        if phase_order[criterion_phase] > phase_order[phase]:
            continue
        status = criterion.get("status")
        is_conditional = criterion.get("conditional") is True
        if status == "N/A" and is_conditional:
            if not meaningful(criterion.get("reason"), 10):
                errors.append(f"{label}.reason: N/A requiere una razón concreta")
            continue
        if status != "PASS":
            errors.append(f"{label}: requiere PASS y está en {status}")
            continue
        evidence = criterion.get("evidence")
        if not isinstance(evidence, list) or not evidence:
            errors.append(f"{label}.evidence: PASS requiere evidencia")
            continue
        for evidence_index, item in enumerate(evidence):
            validate_evidence(
                str(criterion.get("id")), item, evidence_index, root, latest_change, errors
            )

    if VISUAL_LAYER_MODULE in data.get("applicable_modules", []) and phase in {"unit", "final"}:
        matches = additional_by_id.get(VISUAL_LAYER_CRITERION, [])
        if len(matches) != 1:
            errors.append(
                f"{VISUAL_LAYER_CRITERION}: el módulo {VISUAL_LAYER_MODULE} requiere exactamente un criterio bloqueante"
            )
        else:
            criterion = matches[0]
            if criterion.get("phase") != "unit":
                errors.append(f"{VISUAL_LAYER_CRITERION}.phase: debe ser unit para bloquear antes del lote")
            evidence = criterion.get("evidence", [])
            evidence_types = {
                item.get("type") for item in evidence if isinstance(item, dict)
            }
            if criterion.get("status") == "PASS" and "command" not in evidence_types:
                errors.append(
                    f"{VISUAL_LAYER_CRITERION}.evidence: requiere una prueba automática de composición y oclusión"
                )

    for index, issue in enumerate(data.get("open_issues", [])):
        if not isinstance(issue, dict):
            errors.append(f"open_issues[{index}]: debe ser un objeto")
        elif issue.get("severity") == "blocking" and issue.get("status", "open") != "closed":
            errors.append(f"open_issues[{index}]: existe un problema bloqueante abierto")

    return errors


def load_json(path: Path) -> Any:
    with path.open("r", encoding="utf-8") as handle:
        return json.load(handle)


def run_self_test() -> int:
    root = Path(__file__).resolve().parent
    manifest_path = root / "self-test.json"
    data = make_template("Proyecto de prueba", "Entregable de prueba", root)
    data["completion_definition"] = "El archivo final existe y pasa las comprobaciones"
    data["sources"] = ["Solicitud de prueba"]
    data["modules_reviewed"] = True
    data["baseline"] = ["Estado inicial inspeccionado"]
    data["owners"] = [{"item": "entregable", "owner": "validate_gate.py"}]
    stamp = now_iso()
    data["latest_change_at"] = stamp
    evidence = {
        "type": "file",
        "path": "validate_gate.py",
        "description": "Evidencia real para la prueba automática",
        "created_at": stamp,
    }
    for criterion in data["criteria"]:
        if criterion["id"] in CONDITIONAL:
            criterion["status"] = "N/A"
            criterion["reason"] = "El escenario de prueba no posee esta capacidad"
        else:
            criterion["status"] = "PASS"
            criterion["evidence"] = [evidence]
    valid_errors = validate_manifest(data, manifest_path, "final")
    if valid_errors:
        print("SELF-TEST FAIL: un registro válido fue rechazado")
        print("\n".join(valid_errors))
        return 1

    data["applicable_modules"].append(VISUAL_LAYER_MODULE)
    missing_layer_errors = validate_manifest(data, manifest_path, "unit")
    if not any(VISUAL_LAYER_CRITERION in error for error in missing_layer_errors):
        print("SELF-TEST FAIL: el módulo visual pasó sin VLC01")
        return 1
    data["additional_criteria"].append(
        {
            "id": VISUAL_LAYER_CRITERION,
            "description": "La composición conserva una sola superficie por propietario y no oculta capas temáticas",
            "phase": "unit",
            "conditional": False,
            "status": "PASS",
            "reason": "Prueba negativa del validador",
            "evidence": [evidence],
        }
    )
    non_automatic_errors = validate_manifest(data, manifest_path, "unit")
    if not any("prueba automática" in error for error in non_automatic_errors):
        print("SELF-TEST FAIL: VLC01 pasó sin evidencia automática")
        return 1
    data["additional_criteria"][0]["evidence"] = [
        {
            "type": "command",
            "command": "python validate_gate.py --self-test",
            "result": "PASS",
            "output_file": "validate_gate.py",
            "description": "Prueba automática local de composición por capas",
            "created_at": stamp,
        }
    ]
    valid_layer_errors = validate_manifest(data, manifest_path, "unit")
    if valid_layer_errors:
        print("SELF-TEST FAIL: VLC01 válido fue rechazado")
        print("\n".join(valid_layer_errors))
        return 1

    data["criteria"][0]["evidence"] = []
    invalid_errors = validate_manifest(data, manifest_path, "final")
    if not invalid_errors:
        print("SELF-TEST FAIL: un registro inválido fue aceptado")
        return 1
    print("SELF-TEST PASS")
    return 0


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description="Valida la puerta universal de aceptación")
    parser.add_argument("manifest", nargs="?", type=Path, help="Registro JSON que se validará")
    parser.add_argument("--phase", choices=tuple(PHASES), default="final")
    parser.add_argument("--init", dest="init_path", type=Path, help="Crea un registro nuevo")
    parser.add_argument("--project", help="Nombre del proyecto para --init")
    parser.add_argument("--deliverable", help="Nombre del entregable para --init")
    parser.add_argument("--workspace-root", type=Path, default=Path.cwd())
    parser.add_argument("--self-test", action="store_true")
    return parser


def main() -> int:
    args = build_parser().parse_args()
    if args.self_test:
        return run_self_test()
    if args.init_path:
        if not meaningful(args.project) or not meaningful(args.deliverable):
            print("ERROR: --init requiere --project y --deliverable", file=sys.stderr)
            return 2
        if args.init_path.exists():
            print(f"ERROR: no se sobrescribe un registro existente: {args.init_path}", file=sys.stderr)
            return 2
        args.init_path.parent.mkdir(parents=True, exist_ok=True)
        data = make_template(args.project, args.deliverable, args.workspace_root)
        args.init_path.write_text(
            json.dumps(data, ensure_ascii=False, indent=2) + "\n", encoding="utf-8"
        )
        print(f"CREATED {args.init_path.resolve()}")
        print("GATE PENDING: complete la fase preflight antes de producir")
        return 0
    if not args.manifest:
        print("ERROR: indique un registro JSON o use --init", file=sys.stderr)
        return 2
    if not args.manifest.is_file():
        print(f"ERROR: no existe el registro {args.manifest}", file=sys.stderr)
        return 2
    try:
        data = load_json(args.manifest)
    except (OSError, json.JSONDecodeError) as exc:
        print(f"ERROR: no se pudo leer el registro: {exc}", file=sys.stderr)
        return 2
    errors = validate_manifest(data, args.manifest.resolve(), args.phase)
    if errors:
        print(f"GATE FAIL [{args.phase}] - {len(errors)} bloqueo(s)")
        for error in errors:
            print(f"- {error}")
        return 1
    print(f"GATE PASS [{args.phase}] - {args.manifest.resolve()}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

#!/usr/bin/env python3
"""Impide que desaparezcan las reglas bloqueantes de índices y patrones numéricos."""

from __future__ import annotations

import csv
import json
from pathlib import Path


SCRIPT = Path(__file__).resolve()
PACKAGE_ROOT = SCRIPT.parents[1]
PATTERN_FILE = PACKAGE_ROOT / "04_PLANTILLAS" / "PATRONES_CANONICOS_CARTAS_NUMERICAS.json"
INDEX_PROFILE_FILE = PACKAGE_ROOT / "04_PLANTILLAS" / "PERFIL_CANONICO_INDICES_COMPUESTOS.json"
PRODUCTION_ORDER_FILE = PACKAGE_ROOT / "04_PLANTILLAS" / "CONTRATO_ORDEN_PRODUCCION_GRAFICA.json"
UNIVERSAL_GEOMETRY_FILE = PACKAGE_ROOT / "04_PLANTILLAS" / "CONTRATO_GEOMETRIA_UNIVERSAL_CARTAS.json"
VISUAL_SCOPE_FILE = (PACKAGE_ROOT / "09_PROYECTOS" / "SOLITARIO"
                     / "CONTRATO_NUEVAS_MODALIDADES_VISUALES.json")
UNIVERSAL_FRAME_SELF_TEST = (PACKAGE_ROOT / "05_QA" / "fixtures"
                             / "universal-frame-validator-self-test-v1.json")

EXPECTED_PATTERNS = {
    1: [1],
    2: [1, 1],
    3: [1, 1, 1],
    4: [2, 2],
    5: [2, 1, 2],
    6: [2, 2, 2],
    7: [2, 1, 2, 2],
    8: [2, 1, 2, 1, 2],
    9: [2, 2, 1, 2, 2],
    10: [2, 1, 2, 2, 1, 2],
}
EXPECTED_ROW_ORIENTATIONS = {
    1: [0], 2: [0, 180], 3: [0, 0, 180], 4: [0, 180],
    5: [0, 0, 180], 6: [0, 0, 180], 7: [0, 0, 0, 180],
    8: [0, 0, 0, 180, 180], 9: [0, 0, 0, 180, 180],
    10: [0, 0, 0, 180, 180, 180],
}
EXPECTED_SUITS = {"clubs", "diamonds", "hearts", "spades"}

REQUIRED_TEXT = {
    "03_GUIAS_TECNICAS/COMUN/GUIA_FAMILIAS_VISUALES_MODULARES.md": (
        "Orden de dependencia obligatorio",
        "no se empieza por el marco decorativo",
        "prueba geométrica limpia",
        "ocupación interior",
        "60 %",
        "2-1-2-2-1-2",
        "`7` inglés es `5/2`",
        "ocupación horizontal visible",
        "cubre del As al `10`",
        "topología, separación o simetría incorrectas",
        "separación interna mínima visible",
        "`4 px`",
        "ocupación horizontal visible de cada rango",
        "8.4 %",
        "huecos realmente pintados",
        "Geometría universal inmutable",
        "se rediseña el marco",
        "Separar el shell común de la identidad temática",
        "placeholders ni una reinterpretación generativa",
    ),
    "05_QA/PLAN_MAESTRO_QA_UI.md": (
        "medidos a escala final antes de producir el marco",
        "prueba geométrica limpia",
        "El marco envuelve la geometría aprobada",
        "ocupación vertical visible",
        "60 %",
        "firma `2-1-2-2-1-2`",
        "La matriz de barajas incluye As",
        "orientación incorrecta",
        "ocupación horizontal visible",
        "separación interna visible",
        "`4 px`",
        "huella visible de Q/K/10",
        "8.4 % del ancho",
        "diferencia máxima de `1 px`",
        "hash aprobado",
        "No existe traslación ni escala por tema",
        "captura vigente identifica el shell común",
        "no se aceptan placeholders ni reinterpretaciones",
        "una sola superficie propietaria por región visible",
        "cara completa opaca encima debe fallar",
        "RGB neutralizado",
    ),
    "07_CASOS_HISTORICOS/SOLITARIO/FORTY_THIEVES_TOPOLOGIA_Y_OCUPACION_2026-09-10.md": (
        "rango + palo",
        "2-1-2-2-1-2",
        "separación mínima",
        "del As al `10` para los cuatro palos",
        "fixtures negativas",
        "separación interna visible mínima",
        "fixture sin aire",
        "patrón correcto pero demasiado estrecho",
        "rango centrado geométricamente pero sin aire lateral",
        "validador parcial de cuatro rangos",
        "bloque visible completo",
    ),
    "07_CASOS_HISTORICOS/SOLITARIO/SCORPION_ESCALA_PALOS_Y_SIMETRIA_BOLSILLOS_2026-09-11.md": (
        "6.15 %",
        "6.25 %",
        "8.4 %",
        "realmente pintados",
        "máximo `1 px`",
        "fixture con palo reducido",
        "fixture con huecos desiguales",
    ),
    "09_PROYECTOS/SOLITARIO/README.md": (
        "CONTRATO_ORDEN_PRODUCCION_GRAFICA.json",
        "SCORPION_ORDEN_GEOMETRIA_MARCO_2026-09-11.md",
        "prohibido",
        "FORTY_THIEVES_TOPOLOGIA_Y_OCUPACION_2026-09-10.md",
        "PATRONES_CANONICOS_CARTAS_NUMERICAS.json",
        "no autorizan el lote",
        "PERFIL_CANONICO_INDICES_COMPUESTOS.json",
        "mínimo de `4 px`",
        "mínimo visible del `8.4 %`",
        "máximo `1 px`",
        "CONTRATO_GEOMETRIA_UNIVERSAL_CARTAS.json",
        "se modifica el marco",
        "CONTRATO_NUEVAS_MODALIDADES_VISUALES.json",
        "Quedan bloqueados los placeholders numéricos",
        "PNG opaco completo encima",
        "visual_layer_composition",
        "VLC01",
    ),
    "07_CASOS_HISTORICOS/SOLITARIO/RUSSIAN_SOLITAIRE_ALCANCE_TEMA_Y_SHELL_2026-09-14.md": (
        "Shell común del producto",
        "Núcleo universal de cartas",
        "Identidad temática permitida",
        "ea3603284333a5698b9db8ea293184ce530a58d98d7964bad569c224d12c7b77",
        "La primera maqueta queda registrada sólo como ejemplo rechazado",
        "Comparación antes/después del shell completo",
        "Segundo incidente: maestro correcto usado como cara opaca",
        "candidate-v2 y candidate-v3",
        "VLC01",
    ),
}

REQUIRED_CATALOG_IDS = {"UI-GEN-035", "UI-GEN-036", "UI-GEN-037", "UI-GEN-038", "UI-GEN-039", "UI-GEN-040", "UI-GEN-041", "UI-GEN-042", "UI-GEN-045", "UI-GEN-046", "UI-GEN-047", "UI-GEN-048", "UI-GEN-049"}


def validate_visual_scope_contract(failures: list[str]) -> None:
    if not VISUAL_SCOPE_FILE.exists():
        failures.append("alcance visual: falta el contrato de nuevas modalidades de Solitario")
        return

    contract = json.loads(VISUAL_SCOPE_FILE.read_text(encoding="utf-8"))
    if contract.get("schema_version") != "1.1":
        failures.append("alcance visual: se requiere contrato 1.1 con integración de capas")
    if contract.get("status") != "ACTIVE":
        failures.append("alcance visual: el contrato debe estar ACTIVE")

    authorities = contract.get("authorities", {})
    if authorities.get("universal_card_required_status") != "LOCKED":
        failures.append("alcance visual: el maestro universal debe requerir estado LOCKED")
    expected_hash = "ea3603284333a5698b9db8ea293184ce530a58d98d7964bad569c224d12c7b77"
    if authorities.get("universal_card_geometry_sha256") != expected_hash:
        failures.append("alcance visual: el hash universal aprobado no coincide")

    shell = set(contract.get("immutable_shared_shell", []))
    for required in ("header_hierarchy_and_placement", "bottom_toolbar_geometry",
                     "bottom_toolbar_action_order", "navigation_behavior",
                     "safe_area_and_mobile_coordinate_system"):
        if required not in shell:
            failures.append(f"alcance visual: falta shell inmutable {required}")

    invariants = contract.get("universal_card_invariants", {})
    required_true = ("A_to_10_must_use_locked_master", "placeholder_numeric_cards_forbidden",
                     "invalid_frame_must_be_redesigned")
    required_false = ("theme_specific_translation_allowed", "theme_specific_scale_allowed",
                      "theme_specific_position_override_allowed")
    for key in required_true:
        if invariants.get(key) is not True:
            failures.append(f"alcance visual: falta obligación {key}")
    for key in required_false:
        if invariants.get(key) is not False:
            failures.append(f"alcance visual: {key} debe estar prohibido")

    if contract.get("required_gate_modules") != ["visual_layer_composition"]:
        failures.append("alcance visual: falta el módulo bloqueante visual_layer_composition")
    layers = contract.get("layer_composition_contract", {})
    expected_layer_order = [
        "theme_card_surface",
        "locked_universal_A_to_10_content_with_transparent_non_content_pixels",
        "theme_frame_with_transparent_protected_interior",
    ]
    if layers.get("authorized_bottom_to_top_order") != expected_layer_order:
        failures.append("alcance visual: el orden integrado de superficie, contenido y marco cambió")
    for key in (
        "single_surface_owner_per_visible_region",
        "theme_card_surface_must_remain_visible_in_ink_free_samples",
        "locked_master_content_may_be_extracted_deterministically_without_geometry_change",
        "universal_content_transparent_outside_rank_suit_and_pips",
        "frame_transparent_inside_protected_field",
        "transparent_pixels_require_neutral_rgb",
        "final_must_match_deterministic_recomposition_from_declared_layers",
    ):
        if layers.get(key) is not True:
            failures.append(f"alcance visual: falta integración bloqueante {key}")
    if layers.get("locked_master_may_be_used_as_full_opaque_face") is not False:
        failures.append("alcance visual: el maestro opaco completo debe estar prohibido")
    gate_criterion = layers.get("required_gate_criterion", {})
    if gate_criterion != {
        "id": "VLC01", "phase": "unit", "status": "PASS",
        "automatic_command_evidence_required": True,
    }:
        failures.append("alcance visual: falta el criterio VLC01 automático en unit")

    required_order = contract.get("required_order", [])
    if "validate_integrated_composition_and_theme_visibility" not in required_order:
        failures.append("alcance visual: falta validar integración antes de aprobar la unidad")
    blocking = contract.get("blocking_conditions", {})
    for key in (
        "opaque_full_face_over_theme_surface",
        "duplicate_surface_or_frame_owner",
        "frame_opaque_inside_protected_field",
        "transparent_pixels_with_hidden_artifacts",
        "final_not_reproducible_from_declared_layers",
        "visual_layer_module_without_VLC01_command_evidence",
    ):
        if blocking.get(key) is not True:
            failures.append(f"alcance visual: falta bloqueo de integración {key}")

    fixtures = contract.get("negative_fixtures", {})
    expected_fixtures = {
        "redesigned_bottom_toolbar", "moved_header_or_shared_status",
        "placeholder_numeric_card", "wrong_or_missing_universal_hash",
        "theme_specific_numeric_translation_or_scale", "copied_existing_theme_identity",
        "opaque_locked_master_hides_theme_surface",
        "second_card_face_or_background_is_composited_on_top",
        "frame_fills_protected_card_interior",
        "checker_or_hidden_RGB_remains_under_zero_alpha",
        "VLC01_has_only_visual_observation_and_no_command",
        "npot_runtime_resize_changes_contractual_aspect",
        "opaque_drop_targets_cover_theme_surface",
    }
    if set(fixtures) != expected_fixtures:
        failures.append("alcance visual: inventario de fixtures negativas incompleto")
    for name in expected_fixtures:
        if fixtures.get(name) != "FAIL":
            failures.append(f"alcance visual: la fixture {name} no bloquea")


def validate_production_order_contract(failures: list[str]) -> None:
    if not PRODUCTION_ORDER_FILE.exists():
        failures.append("falta el contrato bloqueante de orden de producción gráfica")
        return

    contract = json.loads(PRODUCTION_ORDER_FILE.read_text(encoding="utf-8"))
    if contract.get("schema_version") != "1.3":
        failures.append("orden gráfico: se requiere contrato 1.3 con geometría e integración inmutables")
    expected_order = [
        "declare_asset_and_layer_ownership",
        "inventory_existing_universal_components",
        "measure_extreme_content_at_final_scale",
        "lock_reserved_zones_and_usable_field",
        "render_neutral_geometry_proof",
        "validate_all_content_variants_against_clean_geometry",
        "design_frame_around_locked_zones",
        "validate_frame_against_reserved_zones",
        "add_theme_specific_art_and_typography",
        "validate_integrated_layer_composition",
        "approve_representative_unit",
        "produce_batch",
        "validate_complete_inventory_in_final_context",
    ]
    if contract.get("required_order") != expected_order:
        failures.append("orden gráfico: la secuencia obligatoria fue alterada")

    blocking = contract.get("blocking_rules", {})
    required_blocks = {
        "frame_before_geometry_proof_forbidden",
        "shrinking_approved_universal_content_to_fit_late_frame_forbidden",
        "moving_locked_universal_topology_to_hide_frame_conflict_forbidden",
        "theme_translation_of_universal_content_forbidden",
        "theme_scaling_of_universal_content_forbidden",
        "frame_validation_before_universal_geometry_lock_forbidden",
        "ornament_inside_reserved_zone_forbidden",
        "theme_exception_requires_measured_profile",
        "batch_before_representative_unit_approval_forbidden",
        "opaque_complete_asset_over_owned_surface_forbidden",
        "duplicate_surface_owner_forbidden",
        "opaque_patch_as_integration_forbidden",
        "hidden_RGB_under_zero_alpha_forbidden",
        "visual_layer_composition_requires_VLC01_command_evidence",
    }
    for key in required_blocks:
        if blocking.get(key) is not True:
            failures.append(f"orden gráfico: falta bloqueo {key}")

    card = contract.get("playing_card_application", {})
    if card.get("universal_geometry_contract") != "04_PLANTILLAS/CONTRATO_GEOMETRIA_UNIVERSAL_CARTAS.json":
        failures.append("orden gráfico: falta la fuente única de geometría universal")
    geometry_policy = card.get("universal_geometry_policy", {})
    for key in ("positions_are_global_and_immutable", "visible_sizes_are_global_and_immutable",
                "invalid_frame_must_be_redesigned", "locked_hash_required_before_frame_validation"):
        if geometry_policy.get(key) is not True:
            failures.append(f"orden gráfico: falta política universal {key}")
    for key in ("theme_translation_allowed", "theme_scale_allowed"):
        if geometry_policy.get(key) is not False:
            failures.append(f"orden gráfico: {key} debe estar prohibido")
    envelope = card.get("universal_visible_index_envelope", {})
    if envelope.get("scope") != "new_or_modified_themes":
        failures.append("orden gráfico: el mínimo visible debe aplicarse a temas nuevos o modificados")
    if envelope.get("minimum_visible_suit_height_ratio_of_card_width") != 0.084:
        failures.append("orden gráfico: falta el mínimo visible universal de palo 0.084")
    if envelope.get("pixel_rounding") != "nearest_integer":
        failures.append("orden gráfico: el tamaño universal debe redondearse al píxel más cercano")
    if envelope.get("theme_may_reduce_below_minimum") is not False:
        failures.append("orden gráfico: un tema no puede encoger el palo visible universal")
    for key in ("frame_may_vary_pocket_size_only_when_envelope_and_margin_fit",
                "actual_lower_pocket_must_match_upper_after_180_rotation",
                "legacy_themes_require_separate_migration"):
        if envelope.get(key) is not True:
            failures.append(f"orden gráfico: falta obligación de envolvente {key}")
    if envelope.get("actual_upper_lower_pocket_dimension_delta_pixels_maximum") != 1:
        failures.append("orden gráfico: los huecos pintados sólo pueden diferir 1 px")
    universal = set(card.get("universal_core", []))
    expected_universal = {
        "A_to_10_counts", "A_to_10_pip_topology", "pip_orientation",
        "relative_pip_distribution", "rank_and_suit_content_inventory",
        "absolute_normalized_positions", "visible_rank_and_pip_sizes",
    }
    if universal != expected_universal:
        failures.append("orden gráfico: el núcleo universal de cartas está incompleto")
    fixtures = set(card.get("mandatory_extreme_fixtures", []))
    expected_fixtures = {
        "10_plus_each_suit", "Q_plus_each_suit", "K_plus_each_suit",
        "narrow_rank_plus_each_suit", "A_to_10_all_four_suits",
        "upper_and_lower_180_degree_corners",
    }
    if fixtures != expected_fixtures:
        failures.append("orden gráfico: faltan fixtures extremas previas al marco")

    integration = contract.get("layer_integration", {})
    if integration.get("required_gate_module") != "visual_layer_composition":
        failures.append("orden gráfico: falta el módulo de integración visual")
    if integration.get("required_gate_criterion") != "VLC01":
        failures.append("orden gráfico: falta el criterio VLC01")
    expected_checks = {
        "isolated_layer_dimensions_and_roles",
        "transparent_pixels_outside_authorized_content",
        "theme_surface_visible_in_uncovered_samples",
        "single_surface_and_frame_owner",
        "exact_pixel_recomposition_from_declared_layers",
        "opaque_complete_asset_negative_fixture_rejected",
    }
    if set(integration.get("required_automatic_checks", [])) != expected_checks:
        failures.append("orden gráfico: faltan comprobaciones automáticas de integración")

    negative = contract.get("negative_fixtures", {})
    expected_negative = {
        "decorative_frame_created_before_reserved_zone",
        "ten_rank_clipped_by_frame",
        "universal_symbols_reduced_to_rescue_frame",
        "ornament_crosses_clean_reserved_zone",
        "painted_upper_lower_pockets_have_different_dimensions",
        "theme_translates_universal_content",
        "theme_scales_universal_content",
        "frame_does_not_contain_locked_reserved_zones",
        "frame_uses_unapproved_geometry_hash",
        "correct_master_used_as_opaque_full_face",
        "transparent_frame_contains_hidden_checker_RGB",
        "final_render_differs_from_declared_layer_recomposition",
    }
    if set(negative) != expected_negative:
        failures.append("orden gráfico: inventario de fixtures negativas incompleto")
    for name in expected_negative:
        if negative.get(name, {}).get("expected_status") != "FAIL":
            failures.append(f"orden gráfico: la fixture {name} no bloquea")


def validate_universal_geometry_contract(failures: list[str]) -> None:
    if not UNIVERSAL_GEOMETRY_FILE.exists():
        failures.append("geometría universal: falta el contrato inmutable A–10")
        return
    contract = json.loads(UNIVERSAL_GEOMETRY_FILE.read_text(encoding="utf-8"))
    if contract.get("schema_version") != "1.0":
        failures.append("geometría universal: schema_version debe ser 1.0")
    if contract.get("required_status_before_frame_validation") != "LOCKED":
        failures.append("geometría universal: un marco sólo puede validarse contra LOCKED")
    if contract.get("lock_status") not in {"PENDING_USER_APPROVAL", "LOCKED"}:
        failures.append("geometría universal: estado de bloqueo inválido")
    ownership = contract.get("ownership", {})
    if ownership.get("theme_may_override_universal_geometry") is not False:
        failures.append("geometría universal: el tema no puede ser propietario de la geometría")
    transform = contract.get("immutable_transform", {})
    if transform.get("translation") != [0.0, 0.0] or transform.get("scale") != [1.0, 1.0]:
        failures.append("geometría universal: la transformación canónica debe ser identidad")
    for key in ("theme_translation_allowed", "theme_scale_allowed", "theme_rotation_allowed"):
        if transform.get(key) is not False:
            failures.append(f"geometría universal: {key} debe ser false")
    geometry = contract.get("geometry", {})
    patterns = geometry.get("patterns_top_left", {})
    if set(patterns) != {str(rank) for rank in range(1, 11)}:
        failures.append("geometría universal: faltan patrones A–10")
    for rank in range(1, 11):
        if len(patterns.get(str(rank), [])) != rank:
            failures.append(f"geometría universal: el rango {rank} no contiene {rank} símbolos")
    index_size = float(geometry.get("index_suit_visible_height_ratio_of_card_width", 0))
    numeric_size = float(geometry.get("numeric_pip_visible_height_ratio_of_card_width", 0))
    minimum_ratio = float(geometry.get("minimum_numeric_to_index_suit_visible_height_ratio", 0))
    if index_size < 0.084 or not index_size or numeric_size / index_size < minimum_ratio:
        failures.append("geometría universal: escalas visibles de índices o símbolos centrales insuficientes")
    interface = contract.get("frame_interface", {})
    for key in ("frame_must_contain_all_reserved_zones", "frame_art_must_not_enter_reserved_zones",
                "content_may_not_move_to_resolve_collision", "content_may_not_scale_to_resolve_collision",
                "invalid_frame_must_be_redesigned", "theme_profile_must_reference_geometry_sha256"):
        if interface.get(key) is not True:
            failures.append(f"geometría universal: falta interfaz bloqueante {key}")
    expected_fixtures = {
        "translated_numeric_pattern", "scaled_numeric_pattern", "moved_index_center",
        "reduced_numeric_pip_size", "reduced_index_suit_size", "frame_reserved_zone_too_small",
        "frame_art_intrudes_reserved_zone", "unapproved_universal_geometry",
        "modified_locked_geometry_without_reapproval",
    }
    if set(contract.get("mandatory_negative_fixtures", [])) != expected_fixtures:
        failures.append("geometría universal: inventario de fixtures negativas incompleto")
    if not UNIVERSAL_FRAME_SELF_TEST.exists():
        failures.append("geometría universal: falta el autodiagnóstico del validador")
        return
    report = json.loads(UNIVERSAL_FRAME_SELF_TEST.read_text(encoding="utf-8"))
    if report.get("status") != "PASS":
        failures.append("geometría universal: el autodiagnóstico no está en PASS")
    actual = {item.get("name") for item in report.get("negative_fixtures", [])
              if item.get("expected") == "FAIL" and item.get("actual") == "FAIL"}
    if actual != expected_fixtures:
        failures.append("geometría universal: no todas las fixtures negativas quedaron bloqueadas")


def validate_index_clearance_profile(failures: list[str]) -> int:
    if not INDEX_PROFILE_FILE.exists():
        failures.append("falta el perfil canónico de índices compuestos")
        return 0

    contract = json.loads(INDEX_PROFILE_FILE.read_text(encoding="utf-8"))
    expected_ranks = ["A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"]
    if contract.get("applies_to_ranks") != expected_ranks:
        failures.append("índices: la cobertura debe ser exactamente A-10 y J/Q/K")
    measurement = contract.get("measurement", {})
    reference = measurement.get("reference_card_pixels", {})
    minimum = measurement.get("minimum_visible_rank_suit_clearance_pixels")
    if reference != {"width": 220, "height": 348} or minimum != 4.0:
        failures.append("índices: falta la referencia bloqueante 220x348 con 4 px de aire")
    for key in ("same_clearance_upper_lower_required", "same_horizontal_visible_center_required",
                "same_vertical_visible_center_required", "composite_rank_suit_center_required",
                "rank_suit_visible_axis_alignment_required", "all_four_suits_per_rank_required",
                "maximum_visible_rank_width_occupancy_required", "theme_specific_font_fit_required",
                "integer_pixel_snap_required", "corner_specific_part_offset_allowed_only_when_measured",
                "complete_theme_inventory_required"):
        if measurement.get(key) is not True:
            failures.append(f"índices: falta obligación {key}")
    if measurement.get("layered_render_capture_required") != ["full", "rank_only", "suit_only", "clean"]:
        failures.append("índices: faltan las cuatro capas de captura renderizada")
    if measurement.get("inventory_visible_fit_may_be_na") is not False:
        failures.append("índices: el ajuste visible no puede marcarse N/A en el inventario")
    if measurement.get("lower_rotation_degrees") != 180:
        failures.append("índices: la esquina inferior debe rotar exactamente 180 grados")
    if measurement.get("minimum_visible_suit_height_ratio_of_card_width_for_new_or_modified_themes") != 0.084:
        failures.append("índices: falta el mínimo visible universal de palo 0.084")
    if measurement.get("visible_suit_height_pixel_rounding") != "nearest_integer":
        failures.append("índices: falta el redondeo visible al píxel más cercano")
    if measurement.get("theme_may_reduce_visible_suit_below_minimum") is not False:
        failures.append("índices: el tema no puede reducir el palo visible")
    if measurement.get("actual_painted_pocket_measurement_required") is not True:
        failures.append("índices: deben medirse los huecos realmente pintados")
    if measurement.get("maximum_upper_lower_pocket_dimension_delta_pixels") != 1:
        failures.append("índices: los huecos pintados sólo pueden diferir 1 px")
    if measurement.get("actual_lower_pocket_must_match_upper_after_180_rotation") is not True:
        failures.append("índices: el hueco inferior pintado debe corresponder al superior rotado")

    profile = contract.get("profiles", {}).get("forty.theme.kraken_pearl", {})
    width = float(reference.get("width", 0))
    height = float(reference.get("height", 0))
    produced = (height * (float(profile.get("rank_offset_ratio_of_card_height", 0))
                          + float(profile.get("suit_offset_ratio_of_card_height", 0)))
                - width * float(profile.get("rank_font_size_ratio_of_card_width", 0)) * .5
                - width * float(profile.get("suit_size_ratio_of_card_width", 0))
                * float(profile.get("suit_scale_y", 0)) * .5)
    if minimum is None or produced < minimum or profile.get("status") != "PASS":
        failures.append(f"índices: Perla deja {produced:.3f}px, por debajo del mínimo")
    if abs(produced - float(profile.get("calculated_visible_clearance_pixels", -999))) > .001:
        failures.append("índices: la separación declarada de Perla no coincide con la fórmula")

    fixture = contract.get("negative_fixtures", {}).get("previous_kraken_pearl_offsets", {})
    rejected = (height * (float(fixture.get("rank_offset_ratio_of_card_height", 0))
                          + float(fixture.get("suit_offset_ratio_of_card_height", 0)))
                - width * float(profile.get("rank_font_size_ratio_of_card_width", 0)) * .5
                - width * float(profile.get("suit_size_ratio_of_card_width", 0))
                * float(profile.get("suit_scale_y", 0)) * .5)
    if minimum is None or rejected >= minimum or fixture.get("expected_status") != "FAIL":
        failures.append("índices: la fixture anterior pegada no queda bloqueada")

    profiles = contract.get("profiles", {})
    expected_theme_ids = {
        "forty.theme.kraken_pearl", "forty.theme.leviathan_crown", "forty.theme.storm_siren",
        "forty.theme.crimson_flag", "forty.theme.obsidian_corsair", "forty.theme.doubloon_chamber",
    }
    if set(profiles) != expected_theme_ids:
        failures.append("índices: los seis temas de Forty Thieves deben tener perfil canónico explícito")
    evidence_path = "Logs/UniversalNumericFortyThievesBatch/all-themes-index-optical-fit-v1.json"
    for theme_id in expected_theme_ids:
        item = profiles.get(theme_id, {})
        if (item.get("status") != "PASS" or item.get("runtime_profile_required") is not True
                or item.get("validated_cards") != 52 or item.get("validated_corners") != 104
                or item.get("evidence") != evidence_path):
            failures.append(f"índices: {theme_id} no declara 52/52 y 104/104 con evidencia vigente")

    crimson = profiles.get("forty.theme.crimson_flag", {})
    expected_scales = {"A": 0.63, "2": 0.92, "4": 0.90, "6": 0.90,
                       "9": 0.90, "10": 0.55, "Q": 0.40, "K": 0.78}
    if crimson.get("maximum_visible_rank_width_occupancy") != 0.76:
        failures.append("índices: Bandera Carmesí debe limitar la ocupación horizontal a 0.76")
    if crimson.get("rank_specific_horizontal_scales") != expected_scales:
        failures.append("índices: faltan las escalas horizontales deterministas de Q/10/K")
    expected_optical_offsets = {
        "Q": {"upper_x": -0.021, "lower_x": 0.021},
        "K": {"upper_x": -0.005, "lower_x": 0.006},
    }
    if crimson.get("rank_specific_optical_offsets") != expected_optical_offsets:
        failures.append("índices: faltan las correcciones ópticas declaradas de Q/K")
    expected_composite_offsets = {
        "A": {"upper_y": 0.010, "lower_y": -0.010},
        **{str(rank): {"upper_y": 0.011, "lower_y": -0.011} for rank in range(2, 7)},
        "7": {"upper_y": 0.010, "lower_y": -0.010},
        **{str(rank): {"upper_y": 0.011, "lower_y": -0.011} for rank in range(8, 11)},
        "J": {"upper_y": 0.014, "lower_y": -0.009},
        "Q": {"upper_y": 0.013, "lower_y": -0.012},
        "K": {"upper_y": 0.013, "lower_y": -0.012},
    }
    if crimson.get("rank_specific_composite_offsets") != expected_composite_offsets:
        failures.append("índices: faltan los offsets compuestos medidos de 7/K")
    if crimson.get("rank_specific_part_offset_ratios") != {"Q": 0.049}:
        failures.append("índices: falta el aire interno específico de Q")
    expected_j_corners = {"J": {"upper": 0.045, "lower": 0.046}}
    if crimson.get("corner_specific_part_offsets") != expected_j_corners:
        failures.append("índices: falta la corrección medida por esquina de J en Bandera")
    if profiles.get("forty.theme.storm_siren", {}).get("corner_specific_part_offsets") != expected_j_corners:
        failures.append("índices: falta la corrección medida por esquina de J en Sirena")
    if crimson.get("integer_pixel_snap") is not True:
        failures.append("índices: Bandera debe conservar alineación a píxel")
    if crimson.get("status") != "PASS":
        failures.append("índices: el perfil completo validado de Bandera debe quedar en PASS")
    wide_fixture = contract.get("negative_fixtures", {}).get("crimson_flag_unscaled_wide_ranks", {})
    observed = wide_fixture.get("observed_width_occupancy", {})
    if (wide_fixture.get("expected_status") != "FAIL"
            or not observed
            or not any(float(value) > 0.76 for value in observed.values())):
        failures.append("índices: la fixture de rangos anchos sin aire no queda bloqueada")
    fixtures = contract.get("negative_fixtures", {})
    required_negative_fixtures = {
        "partial_rank_coverage", "isolated_rank_without_suit_union",
        "reported_k_and_7_composite_drift", "missing_suit_variants",
        "visible_fit_na_inventory", "reduced_universal_visible_suit",
        "unequal_painted_index_pockets",
    }
    for name in required_negative_fixtures:
        if fixtures.get(name, {}).get("expected_status") != "FAIL":
            failures.append(f"índices: falta fixture negativa bloqueante {name}")
    return len(expected_ranks) * 2


def signature_is_valid(rank: int, signature: list[int]) -> bool:
    return signature == EXPECTED_PATTERNS[rank] and sum(signature) == rank


def validate_pattern_matrix(failures: list[str]) -> int:
    if not PATTERN_FILE.exists():
        failures.append("falta la matriz canónica completa")
        return 0

    contract = json.loads(PATTERN_FILE.read_text(encoding="utf-8"))
    if contract.get("schema_version") != "2.2":
        failures.append("matriz: se requiere versión 2.2 enlazada a geometría universal")
    if contract.get("universal_geometry_contract") != "CONTRATO_GEOMETRIA_UNIVERSAL_CARTAS.json":
        failures.append("matriz: falta enlace al propietario único de geometría")
    suits = set(contract.get("suits", []))
    if suits != EXPECTED_SUITS:
        failures.append(f"matriz: palos {sorted(suits)} != {sorted(EXPECTED_SUITS)}")

    orientation = contract.get("orientation_policy", {})
    if orientation.get("mode") != "per_row":
        failures.append("matriz: la orientación debe declararse por fila")

    measurements = contract.get("measurements", {})
    if measurements.get("theme_transform_override_allowed") is not False:
        failures.append("matriz: un tema no puede transformar el patrón universal")
    for key in ("bounds_center_required", "minimum_visible_clearance_required",
                "per_theme_visible_field_occupancy_required"):
        if measurements.get(key) is not True:
            failures.append(f"matriz: falta obligación {key}")
    mean_by_rank = measurements.get("mean_center_required_by_rank", {})
    expected_mean = {str(rank): rank != 7 for rank in EXPECTED_PATTERNS}
    if mean_by_rank != expected_mean:
        failures.append("matriz: la excepción tradicional de centro promedio debe limitarse al 7")
    required_fixtures = set(measurements.get("negative_fixtures_required", []))
    expected_fixtures = {
        "missing_symbol", "extra_symbol", "shifted_pattern",
        "wrong_topology_same_count", "insufficient_clearance", "wrong_row_orientation",
        "cramped_pattern_in_wide_field",
        "theme_transform_override",
    }
    if required_fixtures != expected_fixtures:
        failures.append("matriz: inventario de fixtures negativas incompleto")

    patterns = contract.get("patterns", {})
    if set(patterns) != {str(rank) for rank in EXPECTED_PATTERNS}:
        failures.append("matriz: deben existir exactamente los valores del As al 10")

    for rank, expected in EXPECTED_PATTERNS.items():
        item = patterns.get(str(rank), {})
        signature = item.get("row_signature")
        orientations = item.get("row_orientations_degrees")
        if item.get("count") != rank:
            failures.append(f"rango {rank}: conteo {item.get('count')} incorrecto")
        if signature != expected or not signature_is_valid(rank, signature or []):
            failures.append(f"rango {rank}: firma {signature} != {expected}")
        if orientations != EXPECTED_ROW_ORIENTATIONS[rank]:
            failures.append(f"rango {rank}: orientación por fila {orientations} != {EXPECTED_ROW_ORIENTATIONS[rank]}")

        missing = expected.copy()
        missing[-1] -= 1
        missing = [value for value in missing if value > 0]
        extra = expected.copy()
        extra[0] += 1
        if signature_is_valid(rank, missing):
            failures.append(f"rango {rank}: fixture faltante fue aceptada")
        if signature_is_valid(rank, extra):
            failures.append(f"rango {rank}: fixture adicional fue aceptada")
        if rank > 1:
            alternate = [rank] if expected != [rank] else [1] * rank
            if signature_is_valid(rank, alternate):
                failures.append(f"rango {rank}: topología alternativa fue aceptada")

    return len(patterns) * len(suits)


def main() -> int:
    failures: list[str] = []

    validate_production_order_contract(failures)
    validate_universal_geometry_contract(failures)
    validate_visual_scope_contract(failures)
    coverage = validate_pattern_matrix(failures)
    index_coverage = validate_index_clearance_profile(failures)

    for relative, snippets in REQUIRED_TEXT.items():
        path = PACKAGE_ROOT / relative
        if not path.exists():
            failures.append(f"falta documento obligatorio: {relative}")
            continue
        content = path.read_text(encoding="utf-8")
        for snippet in snippets:
            if snippet not in content:
                failures.append(f"{relative}: falta regla obligatoria: {snippet}")

    catalog = PACKAGE_ROOT / "06_CATALOGO_APRENDIZAJES" / "CATALOGO_APRENDIZAJES.csv"
    with catalog.open("r", encoding="utf-8", newline="") as stream:
        rows = {row["id"]: row for row in csv.DictReader(stream)}
    for rule_id in sorted(REQUIRED_CATALOG_IDS):
        row = rows.get(rule_id)
        if row is None:
            failures.append(f"catálogo: falta {rule_id}")
        elif row.get("estado") != "CONSOLIDADO":
            failures.append(f"catálogo: {rule_id} no está CONSOLIDADO")

    if failures:
        print(f"CARD LAYOUT RULES FAIL - {len(failures)} bloqueo(s)")
        for failure in failures:
            print(f"- {failure}")
        return 1

    print(f"CARD LAYOUT RULES PASS - 10 rangos x 4 palos = {coverage}; "
          f"13 índices x 2 esquinas = {index_coverage} verificaciones de separación")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

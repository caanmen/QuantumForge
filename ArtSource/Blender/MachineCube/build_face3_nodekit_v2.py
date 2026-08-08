import bpy
import os
import sys


SOURCE_DIR = os.path.dirname(bpy.data.filepath)
sys.path.insert(0, SOURCE_DIR)

from integrate_node_kit_face1_v2 import (  # noqa: E402
    SOURCE_ROOTS,
    append_node_sources,
    clear_state,
    clone_tree,
    find_state,
    rename_status_strip,
    set_visibility,
    tune_node_materials_for_face,
)


OUTPUT_BLEND = os.path.join(
    SOURCE_DIR, "QF_MachineCube_Face3_NodeKitV2.blend"
)
OUTPUT_FBX = os.path.abspath(
    os.path.join(
        SOURCE_DIR,
        "..",
        "..",
        "..",
        "Assets",
        "Project",
        "Art",
        "MachineCubeBlender",
        "QF_MachineCube_Face3_NodeKitV2.fbx",
    )
)


NODE_IDS = (
    "z3_internal_diagnostics",
    "z3_auxiliary_conduits",
    "z3_compensation_circuit",
    "z3_machine_memory",
    "z3_sync_core",
    "z3_structural_reinforcement",
    "z3_convergence_channel",
    "z3_hidden_failure_marker",
    "z3_hidden_secondary_conduit",
)


NORMALIZED_POSITIONS = (
    (0.20, 0.76),
    (0.75, 0.77),
    (0.20, 0.52),
    (0.50, 0.53),
    (0.76, 0.50),
    (0.20, 0.27),
    (0.76, 0.26),
    (0.50, 0.18),
    (0.50, 0.82),
)


MOTIFS = (
    "RELAY",
    "HORIZONTAL",
    "SENSOR",
    "RELAY",
    "RING",
    "HORIZONTAL",
    "DIAMOND",
    "SENSOR",
    "VENT",
)


NODE_SIZES = (
    (1.15, 1.08),
    (1.34, 0.80),
    (0.90, 0.84),
    (1.18, 1.10),
    (1.18, 1.12),
    (1.34, 0.78),
    (1.14, 1.08),
    (0.76, 0.72),
    (0.76, 0.70),
)


def fit_scale(slot):
    width, height = NODE_SIZES[slot]
    return min(width * 1.12 / 1.78, height * 1.12 / 1.62)


def face_position(normalized):
    x, y = normalized
    return ((x - 0.5) * 7.40, 0.0, (y - 0.5) * 7.40)


def rename_anchor_contract(anchor, slot, node_id):
    anchor.name = f"NODE_{slot:02d}_{node_id}"
    for child in anchor.children:
        if child.name.startswith("DAMAGED_STATE_"):
            child.name = f"DAMAGED_STATE_{node_id}"
        elif child.name.startswith("REPAIRED_STATE_"):
            child.name = f"REPAIRED_STATE_{node_id}"
        elif child.name.startswith(("SELECTION_FEEDBACK_", "SelectionFeedback")):
            child.name = f"SELECTION_FEEDBACK_{node_id}"


def build_face3():
    anchors_root = bpy.data.objects.get("NODE_ANCHORS")
    if anchors_root is None:
        raise RuntimeError("NODE_ANCHORS not found in master face")
    anchors = sorted(anchors_root.children, key=lambda obj: obj.name)
    if len(anchors) != len(NODE_IDS):
        raise RuntimeError(f"Expected 9 anchors, found {len(anchors)}")

    loaded_sources = append_node_sources()
    source_by_name = {obj.name: obj for obj in loaded_sources}

    for slot, anchor in enumerate(anchors):
        node_id = NODE_IDS[slot]
        motif = MOTIFS[slot]
        rename_anchor_contract(anchor, slot, node_id)
        anchor.location = face_position(NORMALIZED_POSITIONS[slot])
        scale = fit_scale(slot)

        for state_kind, state_prefix in (
            ("DAMAGED", "DAMAGED_STATE_"),
            ("REPAIRED", "REPAIRED_STATE_"),
        ):
            state = find_state(anchor, state_prefix)
            clear_state(state)
            source_name = SOURCE_ROOTS[(motif, state_kind)]
            source_root = source_by_name.get(source_name)
            if source_root is None:
                raise RuntimeError(f"Missing source node: {source_name}")
            clone = clone_tree(
                source_root,
                state,
                f"F3S{slot:02d}_{state_kind}",
            )
            clone.location = (0.0, -0.70, 0.0)
            clone.rotation_euler = (0.0, 0.0, 0.0)
            clone.scale = (scale, scale, scale)
            clone.name = f"F3_NodeKitV2_{slot:02d}_{motif}_{state_kind}"
            rename_status_strip(clone, state_kind, slot)
            set_visibility(state, state_kind == "DAMAGED" and slot < 7)

        anchor["QF_NodeKitV2_Motif"] = motif
        anchor["QF_NodeKitV2_Scale"] = scale
        anchor["QF_Face"] = 3

    for obj in loaded_sources:
        if obj.name in bpy.data.objects:
            bpy.data.objects.remove(obj, do_unlink=True)

    tune_node_materials_for_face()
    scene = bpy.context.scene
    scene["QF_Face3_NodeKit"] = "V2"
    scene["QF_Face3_NodeCount"] = 9
    scene["QF_Face3_Layout"] = "Game Face3 functional distribution"
    bpy.ops.wm.save_as_mainfile(filepath=OUTPUT_BLEND)

    os.makedirs(os.path.dirname(OUTPUT_FBX), exist_ok=True)
    # Match the proven Face 1 export: Unity already supplies the shared cube
    # depth and corner assembly, so Blender's preview-only side shell must not
    # be exported a second time.
    excluded_names = {"C2_TopShell", "C2_RightShell", "PREVIEW_CUBE_DEPTH"}
    excluded_prefixes = ("C2_SideArmorInset_", "C2_SidePanel_", "C2_SideRib_")
    # FBX must contain both authored states as renderable geometry. Unity, not
    # Blender visibility, decides which state is active at runtime.
    for obj in bpy.context.scene.objects:
        obj.hide_viewport = False
        obj.hide_render = False
        obj.hide_set(False)
    for obj in list(bpy.context.scene.objects):
        if obj.name in excluded_names or obj.name.startswith(excluded_prefixes):
            bpy.data.objects.remove(obj, do_unlink=True)
    bpy.ops.export_scene.fbx(
        filepath=OUTPUT_FBX,
        use_selection=False,
        global_scale=1.0,
        apply_unit_scale=True,
        apply_scale_options="FBX_SCALE_UNITS",
        use_space_transform=True,
        bake_space_transform=False,
        object_types={"EMPTY", "MESH"},
        axis_forward="-Z",
        axis_up="Y",
        add_leaf_bones=False,
        bake_anim=False,
        path_mode="AUTO",
    )
    print(f"Saved Face 3 blend: {OUTPUT_BLEND}")
    print(f"Exported Face 3 FBX: {OUTPUT_FBX}")


if __name__ == "__main__":
    build_face3()

import bpy
import os
import sys


SOURCE_DIR = os.path.dirname(bpy.data.filepath)
sys.path.insert(0, SOURCE_DIR)
from integrate_node_kit_face1_v2 import (  # noqa: E402
    SOURCE_ROOTS, append_node_sources, clear_state, clone_tree, find_state,
    rename_status_strip, set_visibility, tune_node_materials_for_face,
)


OUTPUT_BLEND = os.path.join(SOURCE_DIR, "QF_MachineCube_Face4_NodeKitV2.blend")
OUTPUT_FBX = os.path.abspath(os.path.join(
    SOURCE_DIR, "..", "..", "..", "Assets", "Project", "Art",
    "MachineCubeBlender", "QF_MachineCube_Face4_NodeKitV2.fbx"))

NODE_IDS = (
    "z4_basic_chamber", "z4_seed_reading_1", "z4_archive_expansion_1",
    "z4_initial_stability_1", "z4_controlled_sync_1",
    "z4_tuned_containment_1", "z4_safe_rewind_1", "z4_seed_slot_3",
    "z4_pure_materialization_1", "z4_seed_slot_4",
    "z4_hidden_minor_chronal_pulse", "z4_hidden_resonant_archive",
)
POSITIONS = (
    (.21, .76), (.75, .76), (.22, .61), (.51, .67), (.50, .50), (.76, .51),
    (.18, .38), (.49, .32), (.75, .31), (.21, .18), (.40, .16), (.68, .18),
)
MOTIFS = (
    "RELAY", "DIAMOND", "SENSOR", "HORIZONTAL", "RING", "SENSOR",
    "VENT", "RELAY", "HORIZONTAL", "RELAY", "SENSOR", "VENT",
)
NODE_SIZES = (
    (1.15, 1.08), (1.12, 1.06), (.84, .80), (1.24, .78), (1.18, 1.12),
    (.84, .80), (.78, 1.12), (1.12, 1.08), (1.24, .78), (1.08, 1.02),
    (.72, .70), (.72, .70),
)


def face_position(position):
    return ((position[0] - .5) * 7.40, 0.0, (position[1] - .5) * 7.40)


def fit_scale(slot):
    width, height = NODE_SIZES[slot]
    return min(width * 1.12 / 1.78, height * 1.12 / 1.62)


def rename_contract(anchor, slot):
    node_id = NODE_IDS[slot]
    anchor.name = f"NODE_{slot:02d}_{node_id}"
    for child in anchor.children:
        if "DAMAGED_STATE_" in child.name:
            child.name = f"DAMAGED_STATE_{node_id}"
        elif "REPAIRED_STATE_" in child.name:
            child.name = f"REPAIRED_STATE_{node_id}"
        elif "SELECTION_FEEDBACK_" in child.name or child.name == "SelectionFeedback":
            child.name = f"SELECTION_FEEDBACK_{node_id}"


def export_unity_fbx():
    excluded = {"C2_TopShell", "C2_RightShell", "PREVIEW_CUBE_DEPTH"}
    prefixes = ("C2_SideArmorInset_", "C2_SidePanel_", "C2_SideRib_")
    for obj in bpy.context.scene.objects:
        obj.hide_viewport = False
        obj.hide_render = False
        obj.hide_set(False)
    for obj in list(bpy.context.scene.objects):
        if obj.name in excluded or obj.name.startswith(prefixes):
            bpy.data.objects.remove(obj, do_unlink=True)
    bpy.ops.export_scene.fbx(
        filepath=OUTPUT_FBX, use_selection=False, global_scale=1.0,
        apply_unit_scale=True, apply_scale_options="FBX_SCALE_UNITS",
        use_space_transform=True, bake_space_transform=False,
        object_types={"EMPTY", "MESH"}, axis_forward="-Z", axis_up="Y",
        add_leaf_bones=False, bake_anim=False, path_mode="AUTO")


def build():
    anchors_root = bpy.data.objects.get("NODE_ANCHORS")
    anchors = sorted(anchors_root.children, key=lambda obj: obj.name)
    seed_anchor = anchors[-1]
    for slot in range(len(anchors), len(NODE_IDS)):
        anchors.append(clone_tree(seed_anchor, anchors_root, f"F4A{slot:02d}"))
    if len(anchors) != len(NODE_IDS):
        raise RuntimeError(f"Expected 12 Face 4 anchors, found {len(anchors)}")

    sources = append_node_sources()
    by_name = {obj.name: obj for obj in sources}
    for slot, anchor in enumerate(anchors):
        rename_contract(anchor, slot)
        anchor.location = face_position(POSITIONS[slot])
        motif = MOTIFS[slot]
        scale = fit_scale(slot)
        for state_kind, prefix in (("DAMAGED", "DAMAGED_STATE_"),
                                   ("REPAIRED", "REPAIRED_STATE_")):
            state = find_state(anchor, prefix)
            clear_state(state)
            source = by_name[SOURCE_ROOTS[(motif, state_kind)]]
            clone = clone_tree(source, state, f"F4S{slot:02d}_{state_kind}")
            clone.location = (0.0, -0.70, 0.0)
            clone.rotation_euler = (0.0, 0.0, 0.0)
            clone.scale = (scale, scale, scale)
            clone.name = f"F4_NodeKitV2_{slot:02d}_{motif}_{state_kind}"
            rename_status_strip(clone, state_kind, slot)
            set_visibility(state, state_kind == "DAMAGED" and slot < 10)
        anchor["QF_NodeKitV2_Motif"] = motif
        anchor["QF_NodeKitV2_Scale"] = scale
        anchor["QF_Face"] = 4
    for obj in sources:
        if obj.name in bpy.data.objects:
            bpy.data.objects.remove(obj, do_unlink=True)
    tune_node_materials_for_face()
    bpy.context.scene["QF_Face4_NodeKit"] = "V2"
    bpy.context.scene["QF_Face4_NodeCount"] = 12
    bpy.ops.wm.save_as_mainfile(filepath=OUTPUT_BLEND)
    export_unity_fbx()
    print(f"Saved Face 4 blend: {OUTPUT_BLEND}")
    print(f"Exported Face 4 FBX: {OUTPUT_FBX}")


if __name__ == "__main__":
    build()

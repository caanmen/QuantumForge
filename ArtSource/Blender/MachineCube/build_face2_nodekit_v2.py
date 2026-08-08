import bpy
import os
import sys


SOURCE_DIR = os.path.dirname(bpy.data.filepath)
sys.path.insert(0, SOURCE_DIR)
from integrate_node_kit_face1_v2 import (  # noqa: E402
    SOURCE_ROOTS, append_node_sources, clear_state, clone_tree, find_state,
    rename_status_strip, set_visibility, tune_node_materials_for_face,
)


OUTPUT_BLEND = os.path.join(SOURCE_DIR, "QF_MachineCube_Face2_NodeKitV2.blend")
OUTPUT_FBX = os.path.abspath(os.path.join(
    SOURCE_DIR, "..", "..", "..", "Assets", "Project", "Art",
    "MachineCubeBlender", "QF_MachineCube_Face2_NodeKitV2.fbx"))

NODE_IDS = (
    "z2_fusion_table", "z2_fusion_slot_2", "z2_mix_stabilizer_1",
    "z2_composition_reading", "z2_fusion_slot_3",
    "z2_residual_catalyst_1", "z2_fusion_time_control_1",
    "z2_catalyst_tuning", "z2_stable_reaction_chamber",
    "z2_guided_synthesis", "z2_synthesis_core",
    "z2_hidden_catalyst_filter", "z2_hidden_chamber_cooling",
)
MOTIFS = (
    "RELAY", "HORIZONTAL", "DIAMOND", "SENSOR", "RING", "VENT",
    "VENT", "RING", "HORIZONTAL", "SENSOR", "HORIZONTAL", "SENSOR",
    "VENT",
)
NODE_SIZES = (
    (1.10, 1.02), (1.16, .74), (1.02, 1.00), (.82, .78), (1.06, 1.00),
    (.82, .78), (.74, 1.12), (1.02, .98), (1.18, .78), (.80, .76),
    (1.32, .78), (.70, .68), (.70, .68),
)


def fit_scale(slot):
    width, height = NODE_SIZES[slot]
    return min(width * 1.12 / 1.78, height * 1.12 / 1.62)


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
    if len(anchors) != len(NODE_IDS):
        raise RuntimeError(f"Expected 13 Face 2 anchors, found {len(anchors)}")
    sources = append_node_sources()
    by_name = {obj.name: obj for obj in sources}
    for slot, anchor in enumerate(anchors):
        motif = MOTIFS[slot]
        scale = fit_scale(slot)
        for state_kind, prefix in (("DAMAGED", "DAMAGED_STATE_"),
                                   ("REPAIRED", "REPAIRED_STATE_")):
            state = find_state(anchor, prefix)
            clear_state(state)
            source = by_name[SOURCE_ROOTS[(motif, state_kind)]]
            clone = clone_tree(source, state, f"F2S{slot:02d}_{state_kind}")
            clone.location = (0.0, -0.70, 0.0)
            clone.rotation_euler = (0.0, 0.0, 0.0)
            clone.scale = (scale, scale, scale)
            clone.name = f"F2_NodeKitV2_{slot:02d}_{motif}_{state_kind}"
            rename_status_strip(clone, state_kind, slot)
            set_visibility(state, state_kind == "DAMAGED" and slot < 11)
        anchor["QF_NodeKitV2_Motif"] = motif
        anchor["QF_NodeKitV2_Scale"] = scale
    for obj in sources:
        if obj.name in bpy.data.objects:
            bpy.data.objects.remove(obj, do_unlink=True)
    tune_node_materials_for_face()
    bpy.context.scene["QF_Face2_NodeKit"] = "V2"
    bpy.context.scene["QF_Face2_NodeCount"] = 13
    bpy.ops.wm.save_as_mainfile(filepath=OUTPUT_BLEND)
    export_unity_fbx()
    print(f"Saved Face 2 blend: {OUTPUT_BLEND}")
    print(f"Exported Face 2 FBX: {OUTPUT_FBX}")


if __name__ == "__main__":
    build()

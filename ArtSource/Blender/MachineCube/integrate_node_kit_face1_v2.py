import bpy
import os
from mathutils import Matrix


SOURCE_FACE = bpy.data.filepath
SOURCE_DIR = os.path.dirname(SOURCE_FACE)
NODE_KIT = os.path.join(SOURCE_DIR, "QF_MachineNodeKitV2.blend")
OUTPUT_BLEND = os.path.join(SOURCE_DIR, "QF_MachineCube_Face1_NodeKitV2.blend")


# Motifs follow the spatial rhythm of the approved face reference:
# larger primary sockets, repeated sensor nodes, a diamond near the top-right,
# and a horizontal reader at the bottom.
MOTIF_BY_SLOT = {
    0: "RING",
    1: "DIAMOND",
    2: "SENSOR",
    3: "RELAY",
    4: "SENSOR",
    5: "VENT",
    6: "HORIZONTAL",
    7: "RING",
    8: "VENT",
}


NODE_SIZE_BY_SLOT = {
    0: (1.24, 1.18),
    1: (1.14, 1.08),
    2: (1.30, 0.80),
    3: (1.24, 1.18),
    4: (0.84, 0.80),
    5: (1.16, 1.04),
    6: (1.34, 0.76),
    7: (0.76, 0.72),
    8: (0.76, 0.70),
}


SOURCE_ROOTS = {
    ("RING", "REPAIRED"): "NODE_01_RING_INTACT",
    ("RING", "DAMAGED"): "NODE_01_RING_DAMAGED",
    ("SENSOR", "REPAIRED"): "NODE_02_SENSOR_INTACT",
    ("SENSOR", "DAMAGED"): "NODE_02_SENSOR_DAMAGED",
    ("VENT", "REPAIRED"): "NODE_03_VENT_INTACT",
    ("VENT", "DAMAGED"): "NODE_03_VENT_DAMAGED",
    ("RELAY", "REPAIRED"): "NODE_04_RELAY_INTACT",
    ("RELAY", "DAMAGED"): "NODE_04_RELAY_DAMAGED",
    ("DIAMOND", "REPAIRED"): "NODE_05_DIAMOND_INTACT",
    ("DIAMOND", "DAMAGED"): "NODE_05_DIAMOND_DAMAGED",
    ("HORIZONTAL", "REPAIRED"): "NODE_06_HORIZONTAL_INTACT",
    ("HORIZONTAL", "DAMAGED"): "NODE_06_HORIZONTAL_DAMAGED",
}


def remove_tree(obj):
    for child in list(obj.children):
        remove_tree(child)
    bpy.data.objects.remove(obj, do_unlink=True)


def clear_state(state):
    for child in list(state.children):
        remove_tree(child)


def clone_tree(source, parent, prefix):
    clone = source.copy()
    if source.data is not None:
        clone.data = source.data.copy()
    clone.name = f"{prefix}_{source.name}"
    bpy.context.scene.collection.objects.link(clone)
    local_matrix = source.matrix_basis.copy()
    clone.parent = parent
    clone.matrix_parent_inverse = Matrix.Identity(4)
    clone.matrix_basis = local_matrix
    clone.hide_render = False
    clone.hide_viewport = False
    for child in source.children:
        clone_tree(child, clone, prefix)
    return clone


def set_visibility(root, visible):
    root.hide_render = not visible
    root.hide_viewport = not visible
    for child in root.children_recursive:
        child.hide_render = not visible
        child.hide_viewport = not visible


def rename_status_strip(clone_root, state_kind, slot):
    status = next(
        (obj for obj in clone_root.children_recursive if "StatusBar" in obj.name),
        None,
    )
    if status is None:
        raise RuntimeError(f"Status bar missing in slot {slot} {state_kind}")
    prefix = (
        "RepairedNode_OperationalStrip"
        if state_kind == "REPAIRED"
        else "DamagedNode_WarningStrip"
    )
    status.name = f"{prefix}_{slot:02d}"


def find_state(anchor, prefix):
    state = next((child for child in anchor.children if child.name.startswith(prefix)), None)
    if state is None:
        raise RuntimeError(f"Missing {prefix} under {anchor.name}")
    return state


def append_node_sources():
    with bpy.data.libraries.load(NODE_KIT, link=False) as (data_from, data_to):
        data_to.objects = list(data_from.objects)
    return [obj for obj in data_to.objects if obj is not None]


def fit_scale(slot):
    width, height = NODE_SIZE_BY_SLOT[slot]
    # The 1.12 factor matches the authored face's original clamp footprint.
    return min(width * 1.12 / 1.78, height * 1.12 / 1.62)


def tune_node_materials_for_face():
    settings = {
        "QF_NODE_METAL": {
            "Base Color": (0.055, 0.072, 0.095, 1.0),
            "Metallic": 0.72,
            "Roughness": 0.42,
        },
        "QF_NODE_DARK": {
            "Base Color": (0.008, 0.014, 0.022, 1.0),
            "Metallic": 0.16,
            "Roughness": 0.66,
        },
        "QF_NODE_CYAN": {
            "Base Color": (0.0, 0.18, 0.28, 1.0),
            "Emission Color": (0.0, 0.46, 0.78, 1.0),
            "Emission Strength": 1.65,
            "Metallic": 0.20,
            "Roughness": 0.30,
        },
        "QF_NODE_WARNING": {
            "Base Color": (0.28, 0.004, 0.003, 1.0),
            "Emission Color": (1.0, 0.012, 0.004, 1.0),
            "Emission Strength": 2.6,
        },
    }
    for material_name, values in settings.items():
        mat = bpy.data.materials.get(material_name)
        if mat is None or not mat.use_nodes:
            continue
        principled = next(
            (node for node in mat.node_tree.nodes if node.type == "BSDF_PRINCIPLED"),
            None,
        )
        if principled is None:
            continue
        for input_name, value in values.items():
            if input_name in principled.inputs:
                principled.inputs[input_name].default_value = value


def integrate():
    loaded_sources = append_node_sources()
    source_by_name = {obj.name: obj for obj in loaded_sources}

    anchors_root = bpy.data.objects.get("NODE_ANCHORS")
    if anchors_root is None:
        raise RuntimeError("NODE_ANCHORS not found in Face 1 template")
    anchors = sorted(anchors_root.children, key=lambda obj: obj.name)
    if len(anchors) != 9:
        raise RuntimeError(f"Expected 9 Face 1 anchors, found {len(anchors)}")

    for slot, anchor in enumerate(anchors):
        motif = MOTIF_BY_SLOT[slot]
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
                f"F1S{slot:02d}_{state_kind}",
            )
            # Place the node in front of the authored circuit layer so pipes
            # disappear behind the enclosure instead of crossing its face.
            clone.location = (0.0, -0.70, 0.0)
            clone.rotation_euler = (0.0, 0.0, 0.0)
            clone.scale = (scale, scale, scale)
            clone.name = f"F1_NodeKitV2_{slot:02d}_{motif}_{state_kind}"
            rename_status_strip(clone, state_kind, slot)

            # Preserve the game's existing public/secret state contract.
            visible_by_default = state_kind == "DAMAGED" and slot < 7
            set_visibility(state, visible_by_default)

        anchor["QF_NodeKitV2_Motif"] = motif
        anchor["QF_NodeKitV2_Scale"] = scale

    # Source objects were loaded only as an internal cloning library.
    for obj in loaded_sources:
        if obj.name in bpy.data.objects:
            bpy.data.objects.remove(obj, do_unlink=True)

    tune_node_materials_for_face()

    scene = bpy.context.scene
    scene["QF_Face1_NodeKit"] = "V2"
    scene["QF_Face1_NodeCount"] = 9
    scene["QF_Face1_Layout"] = "Authored Face1 V3 distribution"
    bpy.ops.wm.save_as_mainfile(filepath=OUTPUT_BLEND)
    print(f"Saved integrated Face 1: {OUTPUT_BLEND}")


if __name__ == "__main__":
    integrate()

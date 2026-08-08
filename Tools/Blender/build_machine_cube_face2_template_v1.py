import importlib.util
import math
import os
import sys

import bpy


SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
PROJECT_ROOT = os.path.abspath(os.path.join(SCRIPT_DIR, "..", ".."))
FACE1_PATH = os.path.join(SCRIPT_DIR,
                          "build_machine_cube_face1_template_v3.py")
BLEND_PATH = os.path.join(
    PROJECT_ROOT, "ArtSource", "Blender", "MachineCube",
    "QF_MachineCube_Face2TemplateV1.blend")
FBX_PATH = os.path.join(
    PROJECT_ROOT, "Assets", "Project", "Art", "MachineCubeBlender",
    "QF_MachineCube_Face2TemplateV1.fbx")
PREVIEW_DIR = os.path.join(
    PROJECT_ROOT, "Logs", "VisualQA", "MachineCubeFace2TemplateV1")
DAMAGED_PREVIEW = os.path.join(PREVIEW_DIR,
                               "Face2_Initial_Selected.png")
REPAIRED_PREVIEW = os.path.join(PREVIEW_DIR, "Face2_Repaired.png")


def load_face1_module():
    spec = importlib.util.spec_from_file_location("qf_face1_v3", FACE1_PATH)
    module = importlib.util.module_from_spec(spec)
    sys.modules[spec.name] = module
    spec.loader.exec_module(module)
    return module


face1 = load_face1_module()
base = face1.base
v2 = face1.v2
FACE1_DAMAGE_DRESSING = face1.build_damage_dressing
FACE1_REPAIRED_SURROUND = face1.build_repaired_surround


NODE_DEFS = (
    ("z2_fusion_table", -2.220, 1.924, 1.10, 1.02, "octagonal", False),
    ("z2_fusion_slot_2", -0.074, 1.998, 1.16, 0.74, "horizontal", False),
    ("z2_mix_stabilizer_1", 1.776, 1.702, 1.02, 1.00, "diamond", False),
    ("z2_composition_reading", -2.368, 0.296, 0.82, 0.78, "compact", False),
    ("z2_fusion_slot_3", -0.074, 0.222, 1.06, 1.00, "octagonal", False),
    ("z2_residual_catalyst_1", 1.850, 0.222, 0.82, 0.78, "compact", False),
    ("z2_fusion_time_control_1", -2.368, -1.554, 0.74, 1.12, "vertical", False),
    ("z2_catalyst_tuning", -0.888, -1.628, 1.02, 0.98, "circular", False),
    ("z2_stable_reaction_chamber", 1.628, -1.628, 1.18, 0.78, "horizontal", False),
    ("z2_guided_synthesis", 0.814, -0.518, 0.80, 0.76, "compact", False),
    ("z2_synthesis_core", -0.148, -2.812, 1.32, 0.78, "horizontal", False),
    ("z2_hidden_catalyst_filter", 2.812, -0.148, 0.70, 0.68, "compact", True),
    ("z2_hidden_chamber_cooling", 2.812, 1.998, 0.70, 0.68, "compact", True),
)

# These are the definitions whose machine data starts with damaged=true.
DAMAGED_INDICES = {3, 8, 9, 10}


def get_materials():
    materials = face1.get_materials()
    materials["cyan"] = base.emission_material(
        "C2_PURPLE", (0.48, 0.08, 0.78), 4.2)
    return materials


def build_face_circuits(parent, materials):
    circuits = base.make_empty("FACE02_CIRCUITS", parent)
    # Fusion face: two vertical trunks joined by a lower synthesis manifold.
    for lane, x in enumerate((-0.20, -0.10, 0.0)):
        v2.circuit_path(f"F2_LeftTrunk_{lane}",
                        [(x, 3.12), (x, -2.18)], materials["circuit"],
                        circuits, y=-0.59, width=0.028)
    for lane, x in enumerate((0.88, 0.98, 1.08)):
        v2.circuit_path(f"F2_RightTrunk_{lane}",
                        [(x, 2.92), (x, -2.32)], materials["circuit"],
                        circuits, y=-0.59, width=0.027)
    for lane, z in enumerate((-2.36, -2.46, -2.56)):
        v2.circuit_path(f"F2_SynthesisBus_{lane}",
                        [(-2.82, z), (2.62, z)], materials["circuit"],
                        circuits, y=-0.60, width=0.030)

    routes = (
        [(-2.220, 1.924), (-1.42, 1.924), (-1.42, 1.10), (-0.20, 1.10)],
        [(-0.074, 1.998), (-0.074, 1.18)],
        [(1.776, 1.702), (1.08, 1.702)],
        [(-2.368, 0.296), (-1.54, 0.296), (-1.54, 0.62), (-0.20, 0.62)],
        [(-0.074, 0.222), (-0.074, -0.30)],
        [(1.850, 0.222), (1.08, 0.222)],
        [(-2.368, -1.554), (-1.48, -1.554), (-1.48, -1.24), (-0.20, -1.24)],
        [(-0.888, -1.628), (-0.20, -1.628)],
        [(1.628, -1.628), (1.08, -1.628)],
        [(0.814, -0.518), (0.98, -0.518)],
        [(-0.148, -2.812), (-0.148, -2.46)],
    )
    for index, route in enumerate(routes):
        v2.circuit_path(f"F2_NodeRoute_{index:02d}", route,
                        materials["circuit"], circuits,
                        y=-0.61, width=0.033)

    return circuits


def build_damage_dressing(parent, node_index, width, height, materials):
    mapping = {3: 2, 8: 5, 9: 6, 10: 5}
    mapped = mapping.get(node_index)
    if mapped is not None:
        FACE1_DAMAGE_DRESSING(parent, mapped, width, height, materials)


def build_repaired_surround(parent, node_index, width, height, materials):
    mapping = {3: 2, 8: 5, 9: 6, 10: 5}
    mapped = mapping.get(node_index)
    if mapped is None:
        return None
    return FACE1_REPAIRED_SURROUND(
        parent, mapped, width, height, materials)


def attach_structural_damage(structural, anchors):
    mapping = {"DamageStage_01": 3, "DamageStage_02": 8,
               "DamageStage_03": 10}
    for stage in list(structural.children):
        node_index = mapping.get(stage.name)
        if node_index is None:
            continue
        anchor = next((item for item in anchors.children
                       if int(item.get("slot_index", -1)) == node_index), None)
        if anchor is None:
            continue
        damaged = next((item for item in anchor.children
                        if item.get("qf_role") == "damaged_state_root"), None)
        if damaged is None:
            continue
        world = stage.matrix_world.copy()
        stage.parent = damaged
        stage.matrix_world = world
        stage["qf_role"] = "node_local_structural_damage"


def build_template():
    v2.build_scene()
    root = bpy.data.objects.get("QF_CONCEPT_V2_ROOT")
    if root is None:
        raise RuntimeError("V2 root was not generated")
    root.name = "QF_FACE02_TEMPLATE_ROOT"
    face1.remove_original_node_art()
    materials = get_materials()
    face1.remap_existing_surface_materials(materials)

    template = base.make_empty("FACE02_MODULAR_TEMPLATE", root)
    template["qf_face_index"] = 1
    template["qf_public_nodes"] = 11
    template["qf_secret_nodes"] = 2
    build_face_circuits(template, materials)
    anchors = base.make_empty("NODE_ANCHORS", template)

    face1.build_damage_dressing = build_damage_dressing
    face1.build_repaired_surround = build_repaired_surround
    try:
        for index, definition in enumerate(NODE_DEFS):
            face1.build_node_anchor(anchors, index, definition, materials)
    finally:
        face1.build_damage_dressing = FACE1_DAMAGE_DRESSING
        face1.build_repaired_surround = FACE1_REPAIRED_SURROUND

    face1.remove_bandage_braces()
    structural = bpy.data.objects.get("DAMAGE_STATES")
    if structural is not None:
        structural.name = "STRUCTURAL_DAMAGE"
        structural["qf_role"] = "node_local_damage_container"
        attach_structural_damage(structural, anchors)
    face1.remove_non_node_lights(anchors)
    return root


def set_preview_state(repaired):
    anchors = bpy.data.objects.get("NODE_ANCHORS")
    if anchors is None:
        raise RuntimeError("NODE_ANCHORS missing")
    for anchor in anchors.children:
        index = int(anchor.get("slot_index", -1))
        secret = bool(anchor.get("secret", False))
        damaged = next((child for child in anchor.children
                        if child.get("qf_role") == "damaged_state_root"), None)
        restored = next((child for child in anchor.children
                         if child.get("qf_role") == "repaired_state_root"), None)
        selection = next((child for child in anchor.children
                          if child.get("qf_role") == "selection_feedback"), None)
        show_damage = not repaired and index in DAMAGED_INDICES and not secret
        face1.set_render_state(damaged, show_damage)
        face1.set_render_state(restored, not secret and not show_damage)
        face1.set_render_state(selection, not repaired and index == 0)


def validate_template():
    anchors = bpy.data.objects.get("NODE_ANCHORS")
    if anchors is None or len(anchors.children) != 13:
        raise RuntimeError("Face 2 must contain exactly 13 stable anchors")
    public_count = sum(1 for item in anchors.children
                       if not bool(item.get("secret", False)))
    secret_count = sum(1 for item in anchors.children
                       if bool(item.get("secret", False)))
    if public_count != 11 or secret_count != 2:
        raise RuntimeError(
            f"Invalid node counts: public={public_count} secret={secret_count}")
    for anchor in anchors.children:
        roles = {child.get("qf_role") for child in anchor.children}
        if not {"damaged_state_root", "repaired_state_root",
                "selection_feedback"}.issubset(roles):
            raise RuntimeError(f"Incomplete states for {anchor.name}")
        selection = next(child for child in anchor.children
                         if child.get("qf_role") == "selection_feedback")
        meshes = [obj for obj in selection.children_recursive
                  if obj.type == "MESH"]
        if len(meshes) != 1 or not meshes[0].name.startswith(
                "Selection_Underlight"):
            raise RuntimeError(f"Invalid selection light in {anchor.name}")
    meshes = [obj for obj in bpy.context.scene.objects if obj.type == "MESH"]
    triangles = 0
    for obj in meshes:
        obj.data.calc_loop_triangles()
        triangles += len(obj.data.loop_triangles)
    print(f"[QF Face2 V1] anchors=13 public=11 secret=2 "
          f"meshes={len(meshes)} triangles={triangles}")


def render_previews():
    os.makedirs(PREVIEW_DIR, exist_ok=True)
    scene = bpy.context.scene
    scene.render.resolution_x = 1024
    scene.render.resolution_y = 1024
    scene.render.resolution_percentage = 100
    set_preview_state(False)
    scene.render.filepath = DAMAGED_PREVIEW
    bpy.ops.render.render(write_still=True)
    set_preview_state(True)
    scene.render.filepath = REPAIRED_PREVIEW
    bpy.ops.render.render(write_still=True)
    set_preview_state(False)


def export_fbx():
    os.makedirs(os.path.dirname(FBX_PATH), exist_ok=True)
    root = bpy.data.objects.get("QF_FACE02_TEMPLATE_ROOT")
    if root is None:
        raise RuntimeError("QF_FACE02_TEMPLATE_ROOT missing")
    bpy.ops.object.select_all(action="DESELECT")
    root.select_set(True)
    for obj in root.children_recursive:
        current = obj
        preview_only = False
        while current is not None:
            if current.name.startswith("PREVIEW_"):
                preview_only = True
                break
            current = current.parent
        if not preview_only:
            obj.select_set(True)
    bpy.context.view_layer.objects.active = root
    bpy.ops.export_scene.fbx(
        filepath=FBX_PATH, use_selection=True,
        object_types={"EMPTY", "MESH"}, use_mesh_modifiers=True,
        use_custom_props=True, add_leaf_bones=False, bake_anim=False,
        apply_unit_scale=True, apply_scale_options="FBX_SCALE_ALL",
        mesh_smooth_type="FACE", path_mode="AUTO")


if __name__ == "__main__":
    build_template()
    v2.setup_render()
    validate_template()
    render_previews()
    os.makedirs(os.path.dirname(BLEND_PATH), exist_ok=True)
    bpy.ops.wm.save_as_mainfile(filepath=BLEND_PATH)
    export_fbx()
    print(DAMAGED_PREVIEW)
    print(REPAIRED_PREVIEW)
    print(BLEND_PATH)
    print(FBX_PATH)
    print("[QF Face2 V1] PASS")

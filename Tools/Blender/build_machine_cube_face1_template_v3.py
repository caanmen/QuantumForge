import importlib.util
import math
import os
import sys

import bpy


SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
PROJECT_ROOT = os.path.abspath(os.path.join(SCRIPT_DIR, "..", ".."))
V2_PATH = os.path.join(SCRIPT_DIR, "build_machine_cube_concept_v2.py")
BLEND_PATH = os.path.join(
    PROJECT_ROOT, "ArtSource", "Blender", "MachineCube",
    "QF_MachineCube_Face1TemplateV3.blend")
FBX_PATH = os.path.join(
    PROJECT_ROOT, "Assets", "Project", "Art", "MachineCubeBlender",
    "QF_MachineCube_Face1TemplateV3.fbx")
PREVIEW_DIR = os.path.join(
    PROJECT_ROOT, "Logs", "VisualQA", "MachineCubeFace1TemplateV3")
DAMAGED_PREVIEW = os.path.join(PREVIEW_DIR, "Face1_Damaged_Selected.png")
REPAIRED_PREVIEW = os.path.join(PREVIEW_DIR, "Face1_Repaired.png")
CUSTOM_TEXTURE_DIR = os.path.join(
    PROJECT_ROOT, "Assets", "Project", "UI", "Vertical", "Machine",
    "CustomTextures", "QF_IndustrialDarkMetal_v1")


def load_v2():
    spec = importlib.util.spec_from_file_location("qf_machine_v2", V2_PATH)
    module = importlib.util.module_from_spec(spec)
    sys.modules[spec.name] = module
    spec.loader.exec_module(module)
    return module


v2 = load_v2()
base = v2.base


NODE_DEFS = (
    ("z1_energy_coupling_1", -2.146, 1.850, 1.24, 1.18, "octagonal", False),
    ("z1_traces_channel_1", 1.628, 1.776, 1.14, 1.08, "diamond", False),
    ("z1_protocol_reading", -2.220, 0.148, 1.30, 0.80, "horizontal", False),
    ("z1_triangle_anchor_1", 0.000, 0.000, 1.24, 1.18, "octagonal", False),
    ("z1_artifact_calibration", 1.776, 0.222, 0.84, 0.80, "compact", False),
    ("z1_flow_distributor", 1.554, -1.702, 1.16, 1.04, "circular", False),
    ("z1_room1_synchronizer", 0.000, -2.738, 1.34, 0.76, "horizontal", False),
    ("z1_hidden_energy_echo", -2.220, -1.628, 0.76, 0.72, "compact", True),
    ("z1_hidden_residual_adjustment", 0.000, 2.368, 0.76, 0.70, "compact", True),
)


def delete_hierarchy(root):
    if root is None:
        return
    objects = [root] + list(root.children_recursive)
    for obj in reversed(objects):
        if obj.name in bpy.data.objects:
            bpy.data.objects.remove(obj, do_unlink=True)


def remove_original_node_art():
    delete_hierarchy(bpy.data.objects.get("NODES"))
    delete_hierarchy(bpy.data.objects.get("REPAIR_STATES"))
    prefixes = (
        "C2_Node_TopLeft", "C2_SecondaryBrokenNode",
        "C2_BottomBrokenNode")
    candidates = [obj for obj in list(bpy.data.objects)
                  if obj.name.startswith(prefixes)]
    roots = []
    for obj in candidates:
        if obj.parent is None or not obj.parent.name.startswith(prefixes):
            roots.append(obj)
    for root in roots:
        delete_hierarchy(root)


def material(name):
    result = bpy.data.materials.get(name)
    if result is None:
        raise RuntimeError(f"Missing material {name}")
    return result


def custom_dark_metal_material(name, tint, tile, rough_min, rough_max):
    """PBR material using the user-selected QF Industrial Dark Metal set."""
    prefix = "QF_IndustrialDarkMetal_"
    paths = {
        "base": os.path.join(CUSTOM_TEXTURE_DIR,
                             prefix + "BaseColor_1024.png"),
        "normal": os.path.join(CUSTOM_TEXTURE_DIR,
                               prefix + "Normal_1024.png"),
        "height": os.path.join(CUSTOM_TEXTURE_DIR,
                               prefix + "Height_1024.png"),
        "metallic": os.path.join(CUSTOM_TEXTURE_DIR,
                                 prefix + "Metallic_1024.png"),
        "ao": os.path.join(CUSTOM_TEXTURE_DIR,
                           prefix + "Occlusion_1024.png"),
        "roughness": os.path.join(CUSTOM_TEXTURE_DIR,
                                  prefix + "Roughness_1024.png"),
    }
    missing = [path for path in paths.values() if not os.path.isfile(path)]
    if missing:
        raise RuntimeError(f"Missing custom PBR maps: {missing}")

    mat = bpy.data.materials.get(name) or bpy.data.materials.new(name)
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    output = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    coordinates = nodes.new("ShaderNodeTexCoord")
    mapping = nodes.new("ShaderNodeMapping")
    mapping.vector_type = "POINT"
    mapping.inputs["Scale"].default_value = (tile, tile, tile)
    links.new(coordinates.outputs["Generated"], mapping.inputs["Vector"])

    def texture(label, path, non_color=False):
        node = nodes.new("ShaderNodeTexImage")
        node.label = label
        node.image = bpy.data.images.load(path, check_existing=True)
        if non_color:
            node.image.colorspace_settings.name = "Non-Color"
        node.projection = "BOX"
        node.projection_blend = 0.18
        links.new(mapping.outputs["Vector"], node.inputs["Vector"])
        return node

    albedo = texture("Industrial Dark Base Color", paths["base"])
    tint_mix = nodes.new("ShaderNodeMixRGB")
    tint_mix.blend_type = "MULTIPLY"
    tint_mix.inputs[0].default_value = 1.0
    tint_mix.inputs[2].default_value = (*tint, 1.0)
    links.new(albedo.outputs["Color"], tint_mix.inputs[1])
    ao = texture("Industrial Dark AO", paths["ao"], True)
    ao_mix = nodes.new("ShaderNodeMixRGB")
    ao_mix.blend_type = "MULTIPLY"
    ao_mix.inputs[0].default_value = 0.62
    links.new(tint_mix.outputs["Color"], ao_mix.inputs[1])
    links.new(ao.outputs["Color"], ao_mix.inputs[2])
    links.new(ao_mix.outputs["Color"], bsdf.inputs["Base Color"])

    normal = texture("Industrial Dark Normal", paths["normal"], True)
    normal_map = nodes.new("ShaderNodeNormalMap")
    normal_map.inputs["Strength"].default_value = 0.72
    links.new(normal.outputs["Color"], normal_map.inputs["Color"])
    height = texture("Industrial Dark Height", paths["height"], True)
    bump = nodes.new("ShaderNodeBump")
    bump.inputs["Strength"].default_value = 0.20
    bump.inputs["Distance"].default_value = 0.030
    links.new(height.outputs["Color"], bump.inputs["Height"])
    links.new(normal_map.outputs["Normal"], bump.inputs["Normal"])
    links.new(bump.outputs["Normal"], bsdf.inputs["Normal"])

    metallic = texture("Industrial Dark Metallic", paths["metallic"], True)
    metallic_scale = nodes.new("ShaderNodeMath")
    metallic_scale.operation = "MULTIPLY"
    metallic_scale.inputs[1].default_value = 0.88
    links.new(metallic.outputs["Color"], metallic_scale.inputs[0])
    links.new(metallic_scale.outputs["Value"], bsdf.inputs["Metallic"])
    roughness = texture("Industrial Dark Roughness", paths["roughness"], True)
    rough_range = nodes.new("ShaderNodeMapRange")
    rough_range.inputs["From Min"].default_value = 0.0
    rough_range.inputs["From Max"].default_value = 1.0
    rough_range.inputs["To Min"].default_value = rough_min
    rough_range.inputs["To Max"].default_value = rough_max
    links.new(roughness.outputs["Color"], rough_range.inputs["Value"])
    links.new(rough_range.outputs["Result"], bsdf.inputs["Roughness"])
    links.new(bsdf.outputs["BSDF"], output.inputs["Surface"])
    mat["qf_texture_set"] = "QF_IndustrialDarkMetal_v1"
    mat["qf_projection"] = "BOX"
    return mat


def get_materials():
    result = {
        "skin": custom_dark_metal_material(
            "QF_V3_DARK_SKIN", (0.88, 0.90, 0.92), 1.00, 0.40, 0.78),
        "plate": custom_dark_metal_material(
            "QF_V3_DARK_PLATE", (0.78, 0.80, 0.82), 1.30, 0.42, 0.82),
        "frame": custom_dark_metal_material(
            "QF_V3_DARK_FRAME", (0.68, 0.70, 0.72), 1.65, 0.38, 0.76),
        "edge": custom_dark_metal_material(
            "QF_V3_DARK_NODE", (0.82, 0.79, 0.73), 1.45, 0.40, 0.80),
        "inner": material("C2_INNER_STRUCTURE"),
        "cavity": material("C2_CARBON_VOID"),
        "char": material("C2_CHARRED_FRACTURE"),
        "raw": material("C2_PBR_TORN_48"),
        "machinery": material("C2_EXPOSED_MACHINERY"),
        "copper": material("C2_BURNT_COPPER"),
        "rust": material("C2_EDGE_OXIDE"),
        "circuit": material("C2_DEAD_CIRCUIT"),
        "cyan": material("C2_CYAN"),
        "amber": material("C2_AMBER"),
        "soot": material("C2_MOTTLED_SOOT"),
    }
    return result


def remap_existing_surface_materials(materials):
    """Replace V2 package materials on the authored chassis, not damage guts."""
    replacements = {
        "C2_PBR_SKIN_35": materials["skin"],
        "C2_PBR_ARMOR_32": materials["plate"],
        "C2_PBR_FRAME_32": materials["frame"],
        "C2_PBR_NODE_33": materials["edge"],
    }
    remapped_slots = 0
    for obj in bpy.context.scene.objects:
        if obj.type != "MESH":
            continue
        for index, old_mat in enumerate(obj.data.materials):
            if old_mat is None:
                continue
            replacement = replacements.get(old_mat.name)
            if replacement is not None:
                obj.data.materials[index] = replacement
                remapped_slots += 1
    print(f"[QF Face1 V3] custom texture remapped slots={remapped_slots}")


def remove_bandage_braces():
    """Remove decorative diagonal rectangles from frame and node corners."""
    targets = [
        obj for obj in list(bpy.context.scene.objects)
        if (obj.name.startswith("C2_Corner_Brace_") or
            "_Clamp_Brace_" in obj.name)
    ]
    for obj in targets:
        bpy.data.objects.remove(obj, do_unlink=True)
    print(f"[QF Face1 V3] removed bandage braces={len(targets)}")


def remove_non_node_lights(anchors):
    """Only node state/selection geometry may use emissive materials."""
    emissive_names = {"C2_CYAN", "C2_AMBER", "C2_PURPLE"}

    def belongs_to_nodes(obj):
        current = obj
        while current is not None:
            if current == anchors:
                return True
            current = current.parent
        return False

    targets = []
    for obj in list(bpy.context.scene.objects):
        if obj.type != "MESH" or belongs_to_nodes(obj):
            continue
        if any(mat is not None and mat.name in emissive_names
               for mat in obj.data.materials):
            targets.append(obj)
    for obj in targets:
        bpy.data.objects.remove(obj, do_unlink=True)
    print(f"[QF Face1 V3] removed non-node lights={len(targets)}")


def set_render_state(root, visible):
    if root is None:
        return
    root.hide_render = not visible
    root.hide_viewport = False
    for obj in root.children_recursive:
        obj.hide_render = not visible
        obj.hide_viewport = False


def box(name, parent, position, scale, mat, rotation=(0.0, 0.0, 0.0),
        bevel=0.025):
    return base.box(name, position, scale, mat, parent,
                    rotation=rotation, bevel=bevel, segments=2)


def build_partial_frame(name, parent, width, height, materials, seed):
    direction = -1.0 if seed % 2 else 1.0
    depth_y = -0.70
    rail = max(0.075, min(width, height) * 0.095)
    # The main silhouette remains readable while two corners and part of one
    # rail are visibly missing.
    box(name + "_TopSurvivor", parent,
        (-width * 0.10, depth_y, height * 0.50),
        (width * 0.70, 0.15, rail), materials["frame"],
        rotation=(math.radians(-3.0), 0.0, math.radians(direction * 2.0)))
    box(name + "_LeftRail", parent,
        (-width * 0.50, depth_y, height * 0.02),
        (rail, 0.16, height * 0.82), materials["frame"],
        rotation=(0.0, math.radians(direction * 3.0), 0.0))
    box(name + "_BottomSurvivor", parent,
        (width * 0.10, depth_y - 0.015, -height * 0.50),
        (width * 0.68, 0.17, rail), materials["edge"],
        rotation=(math.radians(direction * 4.0), 0.0,
                  math.radians(-direction * 3.0)))
    box(name + "_RightRemnant", parent,
        (width * 0.50, depth_y - 0.025, -height * 0.15),
        (rail, 0.17, height * 0.48), materials["raw"],
        rotation=(math.radians(direction * 8.0),
                  math.radians(-direction * 11.0), 0.0))


def build_damaged_node(name, parent, width, height, shape, materials, seed):
    root = base.make_empty(name, parent)
    # A visible recessed body guarantees that a damaged node never becomes an
    # anonymous hole or an unclickable-looking pile of debris.
    box(name + "_Back", root, (0.0, -0.30, 0.0),
        (width * 0.94, 0.18, height * 0.92), materials["inner"], bevel=0.055)
    box(name + "_Cavity", root, (0.02, -0.56, -0.01),
        (width * 0.57, 0.24, height * 0.54), materials["cavity"], bevel=0.045)
    base.frame_rect(name + "_RecognizableInnerFrame", 0.0, 0.0,
                    width * 0.70, height * 0.68,
                    max(0.055, min(width, height) * 0.070), 0.10,
                    materials["edge"], root, y=-0.665, bevel=0.020)
    build_partial_frame(name, root, width, height, materials, seed)

    if shape == "horizontal":
        box(name + "_BrokenCartridge", root,
            (-width * 0.06, -0.74, height * 0.02),
            (width * 0.48, 0.14, height * 0.34), materials["plate"],
            rotation=(math.radians(7.0), math.radians(-9.0),
                      math.radians(4.0)))
    elif shape == "compact":
        for index, (sx, sz) in enumerate(((-1, 1), (1, 1), (-1, -1))):
            v2.front_bolt(name + f"_DeadLamp_{index}",
                          sx * width * 0.15, sz * height * 0.15,
                          0.038, materials["circuit"], root, y=-0.75)
    elif shape == "diamond":
        core = box(name + "_SplitDiamond", root,
                   (-width * 0.02, -0.74, 0.0),
                   (width * 0.39, 0.15, height * 0.39),
                   materials["plate"], bevel=0.030)
        core.rotation_euler.y = math.radians(31.0)
        for index, sign in enumerate((-1.0, 1.0)):
            brace = box(name + f"_DiamondBrace_{index}", root,
                        (sign * width * 0.28, -0.76,
                         -sign * height * 0.24),
                        (width * 0.26, 0.11, 0.075),
                        materials["raw"], bevel=0.018)
            brace.rotation_euler.y = math.radians(sign * 42.0)
    elif shape == "circular":
        base.cylinder_between(name + "_SeizedRotor",
                              (0.0, -0.73, -height * 0.17),
                              (0.0, -0.73, height * 0.17),
                              min(width, height) * 0.17,
                              materials["machinery"], root, vertices=10)
        for index, angle in enumerate((0.15, 1.45, 2.75, 4.35)):
            px = math.cos(angle) * width * 0.28
            pz = math.sin(angle) * height * 0.28
            spoke = box(name + f"_BrokenRotorSpoke_{index}", root,
                        (px, -0.78, pz),
                        (min(width, height) * 0.25, 0.11, 0.065),
                        materials["raw"] if index == 2 else materials["edge"],
                        bevel=0.016)
            spoke.rotation_euler.y = -angle
    else:
        box(name + "_CrackedCore", root,
            (-width * 0.03, -0.74, -height * 0.02),
            (width * 0.43, 0.15, height * 0.40), materials["plate"],
            rotation=(math.radians(-5.0), math.radians(7.0),
                      math.radians(-4.0)))

    direction = -1.0 if seed % 2 else 1.0
    v2.tear_tab(name + "_TornTab", (-width * 0.23, height * 0.23),
                (direction, 0.35), min(width, height) * 0.22,
                materials["raw"], root, angle=direction * 17.0)
    # Two anchored cables are enough to communicate damage without visual
    # garbage. One retains a weak amber terminal.
    base.cable(name + "_CableA",
               [(-width * 0.20, -0.69, -height * 0.12),
                (-width * 0.05, -0.80, -height * 0.25),
                (width * 0.08, -0.82, -height * 0.30)],
               0.018, materials["copper"], root)
    base.cable(name + "_CableB",
               [(width * 0.15, -0.69, height * 0.15),
                (width * 0.28, -0.79, height * 0.04)],
               0.016, materials["edge"], root)
    box(name + "_WarningStrip", root,
        (0.0, -0.83, -height * 0.43),
        (min(0.25, width * 0.24), 0.035, 0.030),
        materials["amber"], bevel=0.010)
    root["qf_state"] = "damaged"
    return root


def build_damage_dressing(parent, node_index, width, height, materials):
    """Localized blast residue and secondary fractures for major damage."""
    if node_index not in (2, 5, 6):
        return
    dressing = base.make_empty(f"DAMAGE_DRESSING_{node_index:02d}", parent)
    dressing["qf_role"] = "localized_blast_dressing"
    direction = -1.0 if node_index in (2, 6) else 1.0
    # Broken perimeter lips expose warm raw metal instead of ending in a
    # featureless black polygon.
    lips = (
        (-width * 0.58, height * 0.34, 0.34, 0.090, 18.0),
        (width * 0.52, height * 0.24, 0.28, 0.075, -24.0),
        (-width * 0.42, -height * 0.52, 0.30, 0.080, -12.0),
        (width * 0.48, -height * 0.44, 0.24, 0.070, 31.0),
    )
    for index, (px, pz, sx, sz, angle) in enumerate(lips):
        lip = box(f"BlastLip_{node_index:02d}_{index:02d}", dressing,
                  (px, -0.70 - index * 0.008, pz),
                  (sx, 0.10, sz),
                  materials["rust"] if index % 2 else materials["raw"],
                  bevel=0.016)
        lip.rotation_euler.y = math.radians(angle * direction)

    # Thin, branching surface cracks extend beyond the cavity; their varied
    # lengths keep the blast from reading as a clean Boolean cut.
    crack_routes = (
        [(-width * 0.50, -0.64, height * 0.18),
         (-width * 0.72, -0.62, height * 0.34),
         (-width * 0.94, -0.60, height * 0.28)],
        [(width * 0.46, -0.64, height * 0.12),
         (width * 0.72, -0.62, height * 0.02),
         (width * 0.90, -0.60, -height * 0.16)],
        [(-width * 0.38, -0.64, -height * 0.42),
         (-width * 0.54, -0.62, -height * 0.70),
         (-width * 0.70, -0.60, -height * 0.86)],
    )
    for index, route in enumerate(crack_routes):
        base.cable(f"BlastCrack_{node_index:02d}_{index:02d}", route,
                   0.014 if index else 0.018,
                   materials["char"], dressing)

    # A restrained oxide fastener adds the warm wear visible in the reference.
    v2.front_bolt(f"BlastOxideBolt_{node_index:02d}",
                  direction * width * 0.62, -height * 0.58,
                  0.040, materials["rust"], dressing, y=-0.74)


def build_repaired_node(name, parent, width, height, shape, materials):
    if shape == "circular":
        root = base.make_empty(name, parent)
        box(name + "_Back", root, (0.0, -0.22, 0.0),
            (width * 1.02, 0.20, height * 1.02), materials["inner"],
            bevel=0.060)
        segments = 12
        length = min(width, height) * 0.26
        for index in range(segments):
            angle = math.tau * index / segments
            px = math.cos(angle) * width * 0.43
            pz = math.sin(angle) * height * 0.43
            box(name + f"_Ring_{index:02d}", root,
                (px, -0.62, pz), (length, 0.15, 0.085),
                materials["edge"] if index % 2 else materials["frame"],
                rotation=(0.0, -angle, 0.0), bevel=0.022)
        base.cylinder_between(name + "_Core",
                              (0.0, -0.69, -height * 0.20),
                              (0.0, -0.69, height * 0.20),
                              min(width, height) * 0.20,
                              materials["plate"], root, vertices=12)
    else:
        mode = "socket" if shape == "compact" else "solid"
        root = v2.layered_node(name, 0.0, 0.0, width, height,
                               materials, parent, mode)
        if shape == "diamond":
            root.rotation_euler.y = math.radians(45.0)
    # Repaired nodes receive a restrained operational signature instead of a
    # large glow, keeping the old-machine mood.
    box(name + "_OperationalStrip", root,
        (0.0, -0.82, -height * 0.40),
        (min(0.28, width * 0.26), 0.035, 0.032),
        materials["cyan"], bevel=0.010)
    root["qf_state"] = "repaired"
    return root


def build_repaired_surround(parent, node_index, width, height, materials):
    if node_index not in (2, 5, 6):
        return None
    if node_index == 2:
        points = [(-1.08, 1.52), (-0.40, 1.66), (0.74, 1.34),
                  (1.06, 0.48), (0.94, -0.98), (0.26, -1.54),
                  (-0.78, -1.40), (-1.18, -0.34)]
        backing_size = (2.62, 0.10, 3.42)
        tiles = (
            (-0.70, 0.96, 0.88, 0.72, -3.0, "skin"),
            (0.30, 1.02, 0.98, 0.66, 2.0, "plate"),
            (-0.78, 0.10, 0.70, 0.86, 2.5, "plate"),
            (0.72, 0.18, 0.62, 0.82, -2.0, "skin"),
            (-0.48, -0.92, 0.90, 0.68, -1.5, "skin"),
            (0.54, -0.88, 0.82, 0.72, 3.0, "plate"),
        )
    elif node_index == 5:
        points = [(-1.18, 0.94), (-0.28, 1.10), (0.98, 0.84),
                  (1.16, 0.10), (0.90, -0.94), (0.02, -1.08),
                  (-1.00, -0.76), (-1.24, 0.08)]
        backing_size = (2.72, 0.10, 2.42)
        tiles = (
            (-0.72, 0.54, 0.82, 0.62, 2.5, "skin"),
            (0.42, 0.58, 1.08, 0.58, -2.0, "plate"),
            (-0.78, -0.34, 0.72, 0.76, -3.0, "plate"),
            (0.72, -0.30, 0.68, 0.72, 2.0, "skin"),
            (0.02, -0.78, 0.78, 0.42, 1.0, "plate"),
        )
    else:
        points = [(-1.22, 0.76), (-0.38, 0.98), (0.96, 0.82),
                  (1.24, 0.10), (0.92, -0.72), (-0.12, -0.86),
                  (-1.10, -0.62)]
        backing_size = (2.78, 0.10, 2.02)
        tiles = (
            (-0.82, 0.42, 0.78, 0.56, -2.5, "skin"),
            (0.16, 0.50, 0.92, 0.50, 2.0, "plate"),
            (0.88, 0.18, 0.52, 0.76, -3.0, "skin"),
            (-0.86, -0.36, 0.72, 0.62, 2.0, "plate"),
            (0.12, -0.42, 0.92, 0.56, -1.5, "skin"),
        )
    box(f"RepairedSurround_{node_index:02d}_Backing", parent,
        (0.0, -0.105, 0.0), backing_size, materials["inner"], bevel=0.035)
    # The irregular polygon is only a recessed sealing skin.  The visible
    # repair is assembled from separate armour plates so it reads as rebuilt
    # machinery instead of a single flat patch pasted over the face.
    patch = v2.polygon_prism(f"RepairedSurround_{node_index:02d}_Seal",
                             points, -0.17, 0.08,
                             materials["inner"], parent)
    base.apply_bevel(patch, 0.018, 2)
    plate_objects = []
    for tile_index, (px, pz, sx, sz, angle, material_key) in enumerate(tiles):
        plate = box(f"RepairedSurround_{node_index:02d}_Plate_{tile_index:02d}",
                    parent, (px, -0.285 - (tile_index % 2) * 0.018, pz),
                    (sx, 0.105, sz), materials[material_key], bevel=0.032)
        plate.rotation_euler.y = math.radians(angle)
        plate_objects.append(plate)

        # Small mechanical clamps break the silhouette and make every plate
        # look physically fastened to the older chassis.
        clamp_x = px + (sx * 0.34 if tile_index % 2 == 0 else -sx * 0.34)
        clamp_z = pz + (sz * 0.38 if tile_index % 3 == 0 else -sz * 0.38)
        clamp = box(
            f"RepairedSurround_{node_index:02d}_Clamp_{tile_index:02d}",
            parent, (clamp_x, -0.372, clamp_z),
            (0.16, 0.045, 0.075), materials["frame"], bevel=0.014)
        clamp.rotation_euler.y = math.radians(angle)

    # Reconnected buses are deliberately segmented and terminate at the node
    # housing, making the repaired state communicate restored function.
    box(f"RepairedSurround_{node_index:02d}_BusH", parent,
        (-width * 0.28, -0.405, -height * 0.40),
        (width * 0.34, 0.045, 0.032), materials["circuit"], bevel=0.009)
    box(f"RepairedSurround_{node_index:02d}_BusH_Right", parent,
        (width * 0.30, -0.405, -height * 0.40),
        (width * 0.28, 0.045, 0.032), materials["circuit"], bevel=0.009)
    box(f"RepairedSurround_{node_index:02d}_BusV", parent,
        (width * 0.40, -0.405, height * 0.18),
        (0.032, 0.045, height * 0.30), materials["circuit"], bevel=0.009)
    return patch


def build_selection_brackets(parent, width, height, material):
    root = base.make_empty("SELECTION_FEEDBACK", parent)
    # Selection is communicated only by the small under-light requested by
    # the user: no halo, brackets, frame or outline around the node.
    box("Selection_Underlight", root,
        (0.0, -0.88, -height * 0.43),
        (min(0.28, width * 0.27), 0.030, 0.034),
        material, bevel=0.009)
    root["qf_role"] = "selection_feedback"
    return root


def build_node_anchor(parent, index, definition, materials):
    node_id, x, z, width, height, shape, secret = definition
    anchor = base.make_empty(f"NODE_{index:02d}_{node_id}", parent)
    anchor.location = (x, 0.0, z)
    anchor["node_id"] = node_id
    anchor["slot_index"] = index
    anchor["secret"] = secret
    anchor["collider_size_x"] = width * 1.18
    anchor["collider_size_z"] = height * 1.18
    anchor["collider_depth"] = 0.72

    damaged_state = base.make_empty(f"DAMAGED_STATE_{node_id}", anchor)
    repaired_state = base.make_empty(f"REPAIRED_STATE_{node_id}", anchor)
    damaged_state["qf_role"] = "damaged_state_root"
    repaired_state["qf_role"] = "repaired_state_root"
    build_damaged_node("DamagedNode", damaged_state, width, height,
                       shape, materials, index)
    build_damage_dressing(damaged_state, index, width, height, materials)
    build_repaired_node("RepairedNode", repaired_state, width, height,
                        shape, materials)
    build_repaired_surround(repaired_state, index, width, height, materials)
    selection = build_selection_brackets(anchor, width, height,
                                         materials["cyan"])
    selection.name = f"SELECTION_FEEDBACK_{node_id}"
    set_render_state(repaired_state, False)
    set_render_state(damaged_state, not secret)
    set_render_state(selection, index == 0 and not secret)
    if secret:
        anchor["qf_initial_visibility"] = "hidden"
        anchor["discovered"] = False
    else:
        anchor["qf_initial_visibility"] = "public"
        anchor["discovered"] = True
    return anchor


def attach_structural_damage_to_nodes(structural, anchors):
    if structural is None:
        return
    mapping = {"DamageStage_01": 2, "DamageStage_02": 5,
               "DamageStage_03": 6}
    for stage in list(structural.children):
        node_index = mapping.get(stage.name)
        if node_index is None:
            continue
        anchor = next((item for item in anchors.children
                       if int(item.get("slot_index", -1)) == node_index), None)
        if anchor is None:
            continue
        damaged_state = next((item for item in anchor.children
                              if item.get("qf_role") ==
                              "damaged_state_root"), None)
        if damaged_state is None:
            continue
        world = stage.matrix_world.copy()
        stage.parent = damaged_state
        stage.matrix_world = world
        stage["qf_role"] = "node_local_structural_damage"


def build_face_circuits(parent, materials):
    circuits = base.make_empty("FACE01_CIRCUITS", parent)
    # Two principal buses and short branches terminate at the real node
    # coordinates rather than decorative positions.
    for lane, x in enumerate((-0.12, 0.0, 0.12)):
        v2.circuit_path(f"F1_BusCenter_{lane}",
                        [(x, 3.20), (x, -3.12)], materials["circuit"],
                        circuits, y=-0.58, width=0.030)
    routes = (
        [(-2.146, 1.40), (-1.42, 1.40), (-1.42, 0.78), (-0.12, 0.78)],
        [(1.628, 1.36), (1.20, 1.36), (1.20, 0.72), (0.12, 0.72)],
        [(-2.220, 0.148), (-1.25, 0.148), (-1.25, 0.22), (-0.12, 0.22)],
        [(1.776, 0.222), (1.20, 0.222), (1.20, -0.30), (0.12, -0.30)],
        [(1.554, -1.702), (1.08, -1.702), (1.08, -1.35), (0.12, -1.35)],
        [(0.000, -2.36), (0.000, -1.90)],
    )
    for index, route in enumerate(routes):
        v2.circuit_path(f"F1_NodeRoute_{index:02d}", route,
                        materials["circuit"], circuits,
                        y=-0.60, width=0.034)
    return circuits


def build_template():
    v2.build_scene()
    root = bpy.data.objects.get("QF_CONCEPT_V2_ROOT")
    if root is None:
        raise RuntimeError("V2 root was not generated")
    root.name = "QF_FACE01_TEMPLATE_ROOT"
    remove_original_node_art()
    materials = get_materials()
    remap_existing_surface_materials(materials)

    template = base.make_empty("FACE01_MODULAR_TEMPLATE", root)
    template["qf_face_index"] = 0
    template["qf_public_nodes"] = 7
    template["qf_secret_nodes"] = 2
    build_face_circuits(template, materials)
    anchors = base.make_empty("NODE_ANCHORS", template)
    for index, definition in enumerate(NODE_DEFS):
        build_node_anchor(anchors, index, definition, materials)
    remove_bandage_braces()

    # Old authored damage remains as structural damage only; node-specific
    # geometry now lives under the stable anchors above.
    structural = bpy.data.objects.get("DAMAGE_STATES")
    if structural is not None:
        structural.name = "STRUCTURAL_DAMAGE"
        structural["qf_role"] = "node_local_damage_container"
        attach_structural_damage_to_nodes(structural, anchors)
    remove_non_node_lights(anchors)
    return root


def set_all_node_states(repaired):
    anchors = bpy.data.objects.get("NODE_ANCHORS")
    if anchors is None:
        raise RuntimeError("NODE_ANCHORS missing")
    for anchor in anchors.children:
        secret = bool(anchor.get("secret", False))
        damaged = next((child for child in anchor.children
                        if child.get("qf_role") == "damaged_state_root"), None)
        restored = next((child for child in anchor.children
                         if child.get("qf_role") == "repaired_state_root"), None)
        selection = next((child for child in anchor.children
                          if child.get("qf_role") ==
                          "selection_feedback"), None)
        set_render_state(damaged, not repaired and not secret)
        set_render_state(restored, repaired and not secret)
        set_render_state(selection, not repaired and
                         int(anchor.get("slot_index", -1)) == 0)
        if not repaired and int(anchor.get("slot_index", -1)) == 0:
            for obj in damaged.children_recursive if damaged else ():
                if obj.name.startswith("DamagedNode_Warning"):
                    obj.hide_render = True


def render_previews():
    os.makedirs(PREVIEW_DIR, exist_ok=True)
    scene = bpy.context.scene
    scene.render.resolution_x = 1024
    scene.render.resolution_y = 1024
    scene.render.resolution_percentage = 100

    set_all_node_states(False)
    scene.render.filepath = DAMAGED_PREVIEW
    bpy.ops.render.render(write_still=True)

    set_all_node_states(True)
    scene.render.filepath = REPAIRED_PREVIEW
    bpy.ops.render.render(write_still=True)

    set_all_node_states(False)


def validate_template():
    anchors = bpy.data.objects.get("NODE_ANCHORS")
    if anchors is None or len(anchors.children) != 9:
        raise RuntimeError("Face 1 must contain exactly 9 stable anchors")
    public_count = sum(1 for item in anchors.children
                       if not bool(item.get("secret", False)))
    secret_count = sum(1 for item in anchors.children
                       if bool(item.get("secret", False)))
    if public_count != 7 or secret_count != 2:
        raise RuntimeError(
            f"Invalid node counts: public={public_count} secret={secret_count}")
    ids = set()
    for anchor in anchors.children:
        node_id = anchor.get("node_id", "")
        if not node_id or node_id in ids:
            raise RuntimeError(f"Invalid or duplicate node id: {node_id}")
        ids.add(node_id)
        roles = {child.get("qf_role") for child in anchor.children}
        required = {"damaged_state_root", "repaired_state_root",
                    "selection_feedback"}
        if not required.issubset(roles):
            raise RuntimeError(f"Incomplete states for {node_id}: {roles}")
        selection = next((child for child in anchor.children
                          if child.get("qf_role") ==
                          "selection_feedback"), None)
        selection_meshes = ([obj for obj in selection.children_recursive
                             if obj.type == "MESH"] if selection else [])
        if (len(selection_meshes) != 1 or
                not selection_meshes[0].name.startswith(
                    "Selection_Underlight")):
            raise RuntimeError(
                f"Selection must be one underlight for {node_id}")
    stale_brackets = [obj.name for obj in bpy.context.scene.objects
                      if obj.name.startswith(("Selection_TL", "Selection_TR",
                                              "Selection_BL", "Selection_BR"))]
    if stale_brackets:
        raise RuntimeError(f"Stale selection brackets: {stale_brackets[:8]}")
    meshes = [obj for obj in bpy.context.scene.objects if obj.type == "MESH"]
    triangles = 0
    for obj in meshes:
        obj.data.calc_loop_triangles()
        triangles += len(obj.data.loop_triangles)
    print(f"[QF Face1 V3] anchors=9 public=7 secret=2 "
          f"meshes={len(meshes)} triangles={triangles}")


def save():
    os.makedirs(os.path.dirname(BLEND_PATH), exist_ok=True)
    bpy.ops.wm.save_as_mainfile(filepath=BLEND_PATH)


def export_fbx():
    os.makedirs(os.path.dirname(FBX_PATH), exist_ok=True)
    export_root = bpy.data.objects.get("QF_FACE01_TEMPLATE_ROOT")
    if export_root is None:
        raise RuntimeError("QF_FACE01_TEMPLATE_ROOT missing for export")
    bpy.ops.object.select_all(action="DESELECT")
    export_root.select_set(True)
    for obj in export_root.children_recursive:
        current = obj
        preview_only = False
        while current is not None:
            if current.name.startswith("PREVIEW_"):
                preview_only = True
                break
            current = current.parent
        if not preview_only:
            obj.select_set(True)
    bpy.context.view_layer.objects.active = export_root
    bpy.ops.export_scene.fbx(
        filepath=FBX_PATH,
        use_selection=True,
        object_types={"EMPTY", "MESH"},
        use_mesh_modifiers=True,
        use_custom_props=True,
        add_leaf_bones=False,
        bake_anim=False,
        apply_unit_scale=True,
        apply_scale_options="FBX_SCALE_ALL",
        mesh_smooth_type="FACE",
        path_mode="AUTO")


if __name__ == "__main__":
    build_template()
    v2.setup_render()
    validate_template()
    render_previews()
    save()
    export_fbx()
    print(DAMAGED_PREVIEW)
    print(REPAIRED_PREVIEW)
    print(BLEND_PATH)
    print(FBX_PATH)
    print("[QF Face1 V3] PASS")

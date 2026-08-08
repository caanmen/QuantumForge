import bpy
import math
import os
import random
from mathutils import Vector


ROOT_DIR = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))
SOURCE_DIR = os.path.join(ROOT_DIR, "ArtSource", "Blender", "MachineCube")
OUTPUT_DIR = os.path.join(ROOT_DIR, "Assets", "Project", "Art", "MachineCubeBlender")
PREVIEW_DIR = os.path.join(ROOT_DIR, "Logs", "VisualQA", "MachineCubeBlender")
BLEND_PATH = os.path.join(SOURCE_DIR, "QF_MachineCube_MasterFace.blend")
FBX_PATH = os.path.join(OUTPUT_DIR, "QF_MachineCube_MasterFace.fbx")
PREVIEW_PATH = os.path.join(PREVIEW_DIR, "QF_MachineCube_MasterFace_Damaged.png")
REPAIRED_PREVIEW_PATH = os.path.join(PREVIEW_DIR,
                                     "QF_MachineCube_MasterFace_Repaired.png")

FACE_SIZE = 7.55
FRONT_Y = -0.24
random.seed(74291)


def clean_scene():
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete(use_global=False)
    for datablocks in (bpy.data.meshes, bpy.data.curves, bpy.data.materials,
                       bpy.data.cameras, bpy.data.lights):
        for block in list(datablocks):
            if block.users == 0:
                datablocks.remove(block)


def make_empty(name, parent=None):
    obj = bpy.data.objects.new(name, None)
    bpy.context.scene.collection.objects.link(obj)
    obj.parent = parent
    return obj


def set_principled_input(bsdf, name, value):
    socket = bsdf.inputs.get(name)
    if socket is not None:
        socket.default_value = value


def industrial_material(name, color_a, color_b, metallic, roughness,
                        bump_strength=0.13, noise_scale=5.0):
    mat = bpy.data.materials.new(name)
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()

    output = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    noise = nodes.new("ShaderNodeTexNoise")
    noise.inputs["Scale"].default_value = noise_scale
    noise.inputs["Detail"].default_value = 6.0
    noise.inputs["Roughness"].default_value = 0.72
    noise.inputs["Distortion"].default_value = 0.18
    coordinates = nodes.new("ShaderNodeTexCoord")
    ramp = nodes.new("ShaderNodeValToRGB")
    ramp.color_ramp.elements[0].position = 0.24
    ramp.color_ramp.elements[0].color = (*color_a, 1.0)
    ramp.color_ramp.elements[1].position = 0.78
    ramp.color_ramp.elements[1].color = (*color_b, 1.0)
    bump_noise = nodes.new("ShaderNodeTexNoise")
    bump_noise.inputs["Scale"].default_value = noise_scale * 7.0
    bump_noise.inputs["Detail"].default_value = 3.0
    bump = nodes.new("ShaderNodeBump")
    bump.inputs["Strength"].default_value = bump_strength
    bump.inputs["Distance"].default_value = 0.09

    set_principled_input(bsdf, "Metallic", metallic)
    set_principled_input(bsdf, "Roughness", roughness)
    links.new(coordinates.outputs["Object"], noise.inputs["Vector"])
    links.new(coordinates.outputs["Object"], bump_noise.inputs["Vector"])
    links.new(noise.outputs["Fac"], ramp.inputs["Fac"])
    links.new(ramp.outputs["Color"], bsdf.inputs["Base Color"])
    links.new(bump_noise.outputs["Fac"], bump.inputs["Height"])
    links.new(bump.outputs["Normal"], bsdf.inputs["Normal"])
    links.new(bsdf.outputs["BSDF"], output.inputs["Surface"])
    return mat


def emission_material(name, color, strength):
    mat = bpy.data.materials.new(name)
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    set_principled_input(bsdf, "Base Color", (*color, 1.0))
    set_principled_input(bsdf, "Metallic", 0.25)
    set_principled_input(bsdf, "Roughness", 0.24)
    set_principled_input(bsdf, "Emission Color", (*color, 1.0))
    set_principled_input(bsdf, "Emission", (*color, 1.0))
    set_principled_input(bsdf, "Emission Strength", strength)
    return mat


def apply_bevel(obj, amount=0.04, segments=2):
    if amount <= 0.0:
        return
    bpy.context.view_layer.objects.active = obj
    obj.select_set(True)
    modifier = obj.modifiers.new("PrecisionBevel", "BEVEL")
    modifier.width = amount
    modifier.segments = segments
    modifier.limit_method = "ANGLE"
    bpy.ops.object.modifier_apply(modifier=modifier.name)
    obj.select_set(False)


def box(name, location, dimensions, material, parent=None, rotation=(0, 0, 0),
        bevel=0.035, segments=2):
    bpy.ops.mesh.primitive_cube_add(location=location, rotation=rotation)
    obj = bpy.context.object
    obj.name = name
    obj.dimensions = dimensions
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    apply_bevel(obj, min(bevel, min(dimensions) * 0.22), segments)
    if material:
        obj.data.materials.append(material)
    obj.parent = parent
    return obj


def cylinder_between(name, start, end, radius, material, parent=None, vertices=12):
    a = Vector(start)
    b = Vector(end)
    direction = b - a
    length = direction.length
    bpy.ops.mesh.primitive_cylinder_add(vertices=vertices, radius=radius,
                                       depth=length, location=(a + b) * 0.5)
    obj = bpy.context.object
    obj.name = name
    obj.rotation_mode = "QUATERNION"
    obj.rotation_quaternion = direction.to_track_quat("Z", "Y")
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    if material:
        obj.data.materials.append(material)
    obj.parent = parent
    return obj


def cable(name, points, radius, material, parent=None):
    curve = bpy.data.curves.new(name + "_Curve", "CURVE")
    curve.dimensions = "3D"
    curve.resolution_u = 10
    curve.bevel_depth = radius
    curve.bevel_resolution = 2
    spline = curve.splines.new("BEZIER")
    spline.bezier_points.add(len(points) - 1)
    for point, co in zip(spline.bezier_points, points):
        point.co = co
        point.handle_left_type = "AUTO"
        point.handle_right_type = "AUTO"
    obj = bpy.data.objects.new(name, curve)
    bpy.context.scene.collection.objects.link(obj)
    if material:
        obj.data.materials.append(material)
    obj.parent = parent
    return obj


def irregular_prism(name, center, radii, depth, material, parent=None,
                    rotation=0.0, seed=1):
    rng = random.Random(seed)
    count = 12
    verts = []
    cx, cy, cz = center
    front = cy - depth * 0.5
    back = cy + depth * 0.5
    for i in range(count):
        angle = math.tau * i / count + rotation
        wobble = 0.78 + rng.random() * 0.34
        x = math.cos(angle) * radii[0] * wobble
        z = math.sin(angle) * radii[1] * wobble
        verts.append((cx + x, front, cz + z))
    verts.extend((x, back, z) for x, _, z in verts[:count])
    faces = [tuple(reversed(range(count))), tuple(range(count, count * 2))]
    for i in range(count):
        j = (i + 1) % count
        faces.append((i, j, count + j, count + i))
    mesh = bpy.data.meshes.new(name + "_Mesh")
    mesh.from_pydata(verts, [], faces)
    mesh.update()
    obj = bpy.data.objects.new(name, mesh)
    bpy.context.scene.collection.objects.link(obj)
    if material:
        obj.data.materials.append(material)
    obj.parent = parent
    return obj


def irregular_ring(name, center, outer, inner, material, parent=None,
                   rotation=0.0, seed=1):
    rng = random.Random(seed)
    count = 18
    cx, cy, cz = center
    outer_verts = []
    inner_verts = []
    for i in range(count):
        angle = math.tau * i / count + rotation
        wobble = 0.88 + rng.random() * 0.22
        outer_verts.append((cx + math.cos(angle) * outer[0] * wobble, cy,
                            cz + math.sin(angle) * outer[1] * wobble))
        inner_wobble = 0.94 + rng.random() * 0.12
        inner_verts.append((cx + math.cos(angle) * inner[0] * inner_wobble,
                            cy - 0.004,
                            cz + math.sin(angle) * inner[1] * inner_wobble))
    verts = outer_verts + inner_verts
    faces = []
    for i in range(count):
        j = (i + 1) % count
        faces.append((i, j, count + j, count + i))
    mesh = bpy.data.meshes.new(name + "_Mesh")
    mesh.from_pydata(verts, [], faces)
    mesh.update()
    obj = bpy.data.objects.new(name, mesh)
    bpy.context.scene.collection.objects.link(obj)
    if material:
        obj.data.materials.append(material)
    obj.parent = parent
    return obj


def boolean_cut(target, cutter):
    bpy.context.view_layer.objects.active = target
    target.select_set(True)
    modifier = target.modifiers.new("StructuralBreach", "BOOLEAN")
    modifier.operation = "DIFFERENCE"
    modifier.solver = "EXACT"
    modifier.object = cutter
    bpy.ops.object.modifier_apply(modifier=modifier.name)
    target.select_set(False)
    bpy.data.objects.remove(cutter, do_unlink=True)


def frame_rect(name, x, z, width, height, depth, thickness, material, parent,
               y=FRONT_Y, bevel=0.04):
    box(name + "_Top", (x, y, z + height * 0.5),
        (width, depth, thickness), material, parent, bevel=bevel)
    box(name + "_Bottom", (x, y, z - height * 0.5),
        (width, depth, thickness), material, parent, bevel=bevel)
    box(name + "_Left", (x - width * 0.5, y, z),
        (thickness, depth, height), material, parent, bevel=bevel)
    box(name + "_Right", (x + width * 0.5, y, z),
        (thickness, depth, height), material, parent, bevel=bevel)


def corner_braces(name, x, z, width, height, material, parent, y, scale=1.0):
    for i, (sx, sz) in enumerate(((-1, 1), (1, 1), (-1, -1), (1, -1))):
        box(name + f"_Brace_{i + 1}",
            (x + sx * width * 0.43, y, z + sz * height * 0.43),
            (0.42 * scale, 0.20, 0.14 * scale), material, parent,
            rotation=(0, math.radians(sx * sz * 45), 0), bevel=0.035)


def node_housing(name, x, z, width, height, materials, parent, mode="solid"):
    node_root = make_empty(name, parent)
    box(name + "_Recess", (x, -0.01, z),
        (width * 0.92, 0.20, height * 0.92), materials["inner"], node_root,
        bevel=0.07, segments=3)
    frame_rect(name + "_Outer", x, z, width, height, 0.22, 0.13,
               materials["frame"], node_root, y=-0.27, bevel=0.045)
    frame_rect(name + "_Inner", x, z, width * 0.74, height * 0.74, 0.20, 0.10,
               materials["edge"], node_root, y=-0.39, bevel=0.035)
    corner_braces(name, x, z, width, height, materials["edge"], node_root,
                  y=-0.46, scale=max(width, height))
    if mode == "port":
        box(name + "_Abyss", (x, -0.30, z),
            (width * 0.50, 0.34, height * 0.50), materials["cavity"], node_root,
            bevel=0.09, segments=3)
        for offset in (-0.20, 0.20):
            cylinder_between(name + f"_InnerRail_{offset:+.2f}",
                (x + offset, -0.50, z - height * 0.28),
                (x + offset, -0.50, z + height * 0.28), 0.028,
                materials["copper"], node_root)
    else:
        box(name + "_Core", (x, -0.50, z),
            (width * 0.52, 0.22, height * 0.52), materials["plate"], node_root,
            bevel=0.075, segments=3)
        if mode == "socket":
            for ox in (-0.18, 0.18):
                for oz in (-0.18, 0.18):
                    bpy.ops.mesh.primitive_cylinder_add(vertices=16, radius=0.055,
                        depth=0.035, location=(x + ox, -0.635, z + oz),
                        rotation=(math.pi * 0.5, 0, 0))
                    light = bpy.context.object
                    light.name = name + f"_Emitter_{ox:+.2f}_{oz:+.2f}"
                    light.data.materials.append(materials["cyan"])
                    light.parent = node_root
    return node_root


def add_surface_circuit(name, x1, z1, x2, z2, materials, parent, live=False):
    y = -0.37
    mat = materials["cyan"] if live else materials["circuit"]
    if abs(x2 - x1) > 0.02:
        box(name + "_H", ((x1 + x2) * 0.5, y, z1),
            (abs(x2 - x1), 0.055, 0.045), mat, parent, bevel=0.018)
    if abs(z2 - z1) > 0.02:
        box(name + "_V", (x2, y, (z1 + z2) * 0.5),
            (0.045, 0.055, abs(z2 - z1)), mat, parent, bevel=0.018)


def build_breach(index, spec, materials, damage_root, repair_root):
    x, z, rx, rz, rotation, seed = spec
    root = make_empty(f"BREACH_{index:02d}_DAMAGE_ON", damage_root)
    cavity = irregular_prism(f"B{index:02d}_RearCavity", (x, 0.43, z),
                             (rx * 0.78, rz * 0.78), 0.18,
                             materials["cavity"], root, rotation, seed + 1)
    irregular_prism(f"B{index:02d}_IntermediateLayer", (x, 0.17, z),
                    (rx * 0.63, rz * 0.62), 0.12,
                    materials["inner"], root, rotation + 0.08, seed + 2)
    irregular_ring(f"B{index:02d}_SootHalo", (x, -0.345, z),
                   (rx * 1.38, rz * 1.30), (rx * 0.90, rz * 0.88),
                   materials["soot"], root, rotation, seed + 8)

    # The bright broken circumference makes the depth readable even in the
    # small 512 px runtime render texture.
    angle_sets = {
        1: (9, 54, 132, 205, 286),
        2: (7, 38, 101, 171, 224, 301, 338),
        3: (18, 84, 166, 246, 319),
    }
    for j, degrees in enumerate(angle_sets[index]):
        angle = math.radians(degrees) + rotation
        wobble = 0.88 + ((j * 37 + seed) % 23) / 100.0
        px = x + math.cos(angle) * rx * wobble
        pz = z + math.sin(angle) * rz * wobble
        tangent = -(angle + math.pi * 0.5)
        box(f"B{index:02d}_FractureEdge_{j + 1:02d}",
            (px, -0.405 - 0.025 * (j % 3), pz),
            ((0.40 if index == 2 else 0.31) * (0.68 + (j % 4) * 0.17),
             0.16 + 0.025 * (j % 2), 0.105 + 0.025 * ((j + 1) % 3)),
            materials["exposed"] if j % 3 != 0 else materials["edge"], root,
            rotation=(math.radians((j % 2) * 7), tangent,
                      math.radians((j % 3 - 1) * 6)), bevel=0.020)

    # Internal braces and recognisable machinery sit well behind the skin.
    cylinder_between(f"B{index:02d}_BraceA",
                     (x - rx * 0.48, 0.00, z - rz * 0.46),
                     (x + rx * 0.42, 0.00, z + rz * 0.36), 0.055,
                     materials["edge"], root, vertices=12)
    cylinder_between(f"B{index:02d}_BraceB",
                     (x - rx * 0.35, 0.04, z + rz * 0.42),
                     (x + rx * 0.44, 0.04, z - rz * 0.34), 0.035,
                     materials["circuit"], root, vertices=10)
    if index == 2:
        cylinder_between("B02_ExposedActuator",
                         (x - 0.38, -0.23, z - 0.56),
                         (x + 0.22, -0.30, z + 0.40), 0.14,
                         materials["exposed"], root, vertices=16)
        cylinder_between("B02_ActuatorCore",
                         (x - 0.31, -0.39, z - 0.43),
                         (x + 0.14, -0.43, z + 0.29), 0.060,
                         materials["copper"], root, vertices=12)

    # Recognisable remains of the impacted node housing. These are direct
    # victims of the explosion, not decorative fragments orbiting the hole.
    if index == 1:
        box("B01_NodeRemnant_Left", (x - rx * 0.92, -0.48, z - 0.03),
            (0.16, 0.25, rz * 1.15), materials["frame"], root,
            rotation=(0, math.radians(-7), math.radians(4)), bevel=0.035)
        box("B01_NodeRemnant_Top", (x - 0.08, -0.50, z + rz * 0.82),
            (rx * 1.05, 0.24, 0.15), materials["edge"], root,
            rotation=(math.radians(5), math.radians(-18), 0), bevel=0.035)
    elif index == 2:
        box("B02_CollapsedHousing", (x + rx * 0.58, -0.58, z + rz * 0.08),
            (0.28, 0.28, rz * 0.98), materials["frame"], root,
            rotation=(math.radians(11), math.radians(24), math.radians(-7)),
            bevel=0.045)
        box("B02_BuckledFacePlate", (x - rx * 0.28, -0.64, z + rz * 0.40),
            (rx * 0.78, 0.18, rz * 0.30), materials["exposed"], root,
            rotation=(math.radians(-13), math.radians(-17), math.radians(8)),
            bevel=0.035)
    else:
        box("B03_NodeRemnant_Bottom", (x + 0.04, -0.49, z - rz * 0.84),
            (rx * 1.16, 0.22, 0.15), materials["frame"], root,
            rotation=(math.radians(-6), math.radians(9), 0), bevel=0.035)

    # Torn lips are intentionally asymmetric and project toward the camera.
    rim_specs = [
        (-0.58, 0.42, 0.34, 0.12, 18),
        (0.52, 0.30, 0.26, 0.10, -28),
        (-0.40, -0.48, 0.30, 0.13, -12),
        (0.46, -0.43, 0.22, 0.11, 31),
        (-0.62, -0.05, 0.15, 0.34, 9),
    ]
    for j, (ox, oz, sx, sz, angle) in enumerate(rim_specs):
        box(f"B{index:02d}_TornLip_{j + 1}",
            (x + ox * rx, -0.39 - 0.035 * (j % 2), z + oz * rz),
            (max(0.10, sx * rx), 0.16, max(0.09, sz * rz)),
            materials["exposed"] if j in (1, 4) else materials["edge"], root,
            rotation=(math.radians((j % 2) * 8), math.radians(angle),
                      math.radians((j - 2) * 3)), bevel=0.025)

    # Soot and oxidised fragments radiate from, but never cover, the hole.
    for j, angle in enumerate((18, 74, 142, 212, 286)):
        a = math.radians(angle) + rotation
        distance = 1.04 + (j % 2) * 0.16
        px = x + math.cos(a) * rx * distance
        pz = z + math.sin(a) * rz * distance
        box(f"B{index:02d}_Scorch_{j + 1}", (px, -0.335, pz),
            (0.18 + 0.06 * (j % 3), 0.018, 0.38 + 0.07 * (j % 2)),
            materials["soot"], root,
            rotation=(0, a * 0.48, 0), bevel=0.012)

    cable(f"B{index:02d}_BrokenCable_A", [
        (x - rx * 0.52, -0.30, z + rz * 0.22),
        (x - rx * 0.18, -0.48, z + rz * 0.05),
        (x + rx * 0.08, -0.60, z - rz * 0.25)], 0.025,
        materials["copper"], root)
    cable(f"B{index:02d}_BrokenCable_B", [
        (x + rx * 0.48, -0.29, z - rz * 0.10),
        (x + rx * 0.18, -0.49, z - rz * 0.28),
        (x - rx * 0.05, -0.58, z - rz * 0.42)], 0.021,
        materials["circuit"], root)
    if index == 2:
        cable("B02_ElectricalArc", [
            (x - 0.18, -0.63, z - 0.20),
            (x - 0.08, -0.72, z - 0.27),
            (x + 0.02, -0.65, z - 0.38)], 0.013,
            materials["cyan"], root)

    patch_root = make_empty(f"BREACH_{index:02d}_REPAIR_ON", repair_root)
    patch = irregular_prism(f"B{index:02d}_RepairPatch", (x, -0.20, z),
                            (rx * 0.92, rz * 0.92), 0.14,
                            materials["plate"], patch_root, rotation, seed)
    apply_bevel(patch, 0.025, 2)
    frame_rect(f"B{index:02d}_RepairClamp", x, z, rx * 1.48, rz * 1.46,
               0.12, 0.08, materials["edge"], patch_root, y=-0.34,
               bevel=0.025)

    # A repair is a field-installed assembly, not a smooth lid. Staggered
    # armor, exposed fasteners and a short live diagnostic route keep the
    # repaired state dimensional while remaining visibly improvised.
    box(f"B{index:02d}_PatchUpperLayer",
        (x - rx * 0.12, -0.35, z + rz * 0.23),
        (rx * 1.18, 0.10, max(0.16, rz * 0.34)), materials["armor"],
        patch_root, rotation=(0, rotation * 0.45, rotation * 0.35),
        bevel=0.032)
    box(f"B{index:02d}_PatchLowerLayer",
        (x + rx * 0.16, -0.37, z - rz * 0.25),
        (rx * 1.05, 0.12, max(0.16, rz * 0.28)), materials["frame"],
        patch_root, rotation=(0, -rotation * 0.25, -rotation * 0.28),
        bevel=0.030)

    bolt_positions = ((-0.62, 0.52), (0.58, 0.50),
                      (-0.56, -0.52), (0.63, -0.48))
    for j, (bx, bz) in enumerate(bolt_positions):
        cylinder_between(f"B{index:02d}_PatchBolt_{j + 1}",
                         (x + bx * rx, -0.39, z + bz * rz),
                         (x + bx * rx, -0.49, z + bz * rz), 0.055,
                         materials["exposed"], patch_root, vertices=10)

    trace_z = z + rz * 0.02
    box(f"B{index:02d}_RepairTrace_A",
        (x - rx * 0.22, -0.455, trace_z),
        (rx * 0.72, 0.045, 0.045), materials["cyan"], patch_root,
        bevel=0.012, segments=2)
    box(f"B{index:02d}_RepairTrace_B",
        (x + rx * 0.15, -0.455, trace_z + rz * 0.18),
        (0.045, 0.045, max(0.12, rz * 0.36)), materials["cyan"],
        bevel=0.012, segments=2)
    box(f"B{index:02d}_RepairTrace_C",
        (x + rx * 0.34, -0.455, trace_z + rz * 0.36),
        (rx * 0.38, 0.045, 0.045), materials["cyan"], patch_root,
        bevel=0.012, segments=2)
    for obj in [patch_root] + list(patch_root.children_recursive):
        obj.hide_render = True
    return cavity


def build_model():
    clean_scene()
    os.makedirs(SOURCE_DIR, exist_ok=True)
    os.makedirs(OUTPUT_DIR, exist_ok=True)
    os.makedirs(PREVIEW_DIR, exist_ok=True)

    materials = {
        "armor": industrial_material("M_ARMOR_32", (0.030, 0.033, 0.036),
                                     (0.20, 0.205, 0.205), 0.78, 0.43, 0.13, 2.6),
        "plate": industrial_material("M_SURFACE_35", (0.040, 0.044, 0.048),
                                     (0.23, 0.225, 0.21), 0.70, 0.50, 0.14, 3.2),
        "frame": industrial_material("M_EXPOSED_METAL", (0.055, 0.052, 0.048),
                                     (0.30, 0.285, 0.25), 0.84, 0.34, 0.10, 2.8),
        "edge": industrial_material("M_NODE_33", (0.040, 0.041, 0.040),
                                    (0.19, 0.18, 0.16), 0.74, 0.42, 0.11, 4.2),
        "inner": industrial_material("M_INNER_DARK", (0.006, 0.007, 0.008),
                                     (0.055, 0.058, 0.058), 0.48, 0.72, 0.20, 7.5),
        "cavity": industrial_material("M_DAMAGE_48", (0.002, 0.002, 0.002),
                                      (0.030, 0.020, 0.013), 0.20, 0.86, 0.25, 8.0),
        "soot": industrial_material("M_SOOT", (0.002, 0.002, 0.002),
                                    (0.025, 0.016, 0.010), 0.08, 0.94, 0.28, 9.0),
        "exposed": industrial_material("M_TORN_METAL", (0.085, 0.070, 0.055),
                                       (0.46, 0.38, 0.27), 0.77, 0.36, 0.14, 5.0),
        "copper": industrial_material("M_COPPER_DARK", (0.045, 0.018, 0.008),
                                      (0.32, 0.12, 0.035), 0.82, 0.43, 0.12, 8.5),
        "circuit": industrial_material("M_CIRCUIT_DARK", (0.012, 0.014, 0.015),
                                       (0.10, 0.11, 0.105), 0.76, 0.38, 0.10, 6.5),
        "cyan": emission_material("M_EMISSION_FACE", (0.0, 0.48, 0.72), 5.5),
        "orange": emission_material("M_WARNING", (0.95, 0.18, 0.025), 3.2),
    }

    root = make_empty("QF_FACE01_ROOT")
    static = make_empty("STATIC_LOW", root)
    detail = make_empty("DETAIL_BALANCED", root)
    high = make_empty("DETAIL_HIGH", root)
    damage_root = make_empty("DAMAGE_STATES", root)
    repair_root = make_empty("REPAIR_STATES", root)
    nodes_root = make_empty("NODES", root)
    # Used only for the Blender review render. Unity receives one canonical
    # face and obtains the visible side from the adjacent face instance.
    side_root = make_empty("PREVIEW_ONLY_CUBE_DEPTH")

    skin = box("GEO_DamagedSkinPermanent", (0, 0.10, 0),
               (FACE_SIZE, 0.38, FACE_SIZE), materials["armor"], static,
               bevel=0.07, segments=3)

    breaches = [
        (2.10, 0.12, 0.76, 0.66, math.radians(-12), 31),
        # Primary blast scar: a tall, connected failure crossing the left
        # circuitry and the former node at (-1.812, 0.227).
        (-1.70, 0.32, 0.92, 1.75, math.radians(9), 53),
        (0.0, -2.78, 0.78, 0.57, math.radians(-7), 79),
    ]
    for index, spec in enumerate(breaches, 1):
        x, z, rx, rz, rotation, seed = spec
        cutter = irregular_prism(f"CUTTER_{index}", (x, 0.10, z),
                                 (rx, rz), 1.20, None, None, rotation, seed)
        boolean_cut(skin, cutter)
    apply_bevel(skin, 0.045, 2)

    box("GEO_RearArmor", (0, 0.56, 0), (7.84, 0.18, 7.84),
        materials["inner"], static, bevel=0.055, segments=3)

    # A frame occupying about seven percent of the face width.
    frame_rect("GEO_OuterFrame", 0, 0, 8.12, 8.12, 0.48, 0.48,
               materials["frame"], static, y=-0.18, bevel=0.075)
    frame_rect("GEO_InnerFrame", 0, 0, 7.28, 7.28, 0.32, 0.17,
               materials["edge"], static, y=-0.31, bevel=0.045)
    corner_braces("GEO_Corner", 0, 0, 8.10, 8.10, materials["edge"], static,
                  y=-0.48, scale=1.85)

    # Layered top and bottom armor establish large readable masses.
    for i, x in enumerate((-2.65, -0.88, 0.92, 2.70)):
        box(f"GEO_TopArmor_{i + 1}", (x, -0.39, 3.47),
            (1.42, 0.22, 0.36), materials["plate"], static,
            bevel=0.055, segments=3)
    for i, x in enumerate((-2.70, -0.92, 0.92, 2.68)):
        box(f"GEO_BottomArmor_{i + 1}", (x, -0.38, -3.48),
            (1.34, 0.20, 0.30), materials["plate"], static,
            bevel=0.045, segments=3)

    # Large plates first: roughly 60% large, 30% medium, 10% small detail.
    large_plates = [
        (0.10, 2.78, 1.72, 0.58, 0.20),
        (2.66, 2.78, 1.25, 0.58, 0.18),
        (2.82, 1.28, 1.42, 1.52, 0.22),
        (0.45, 1.18, 1.35, 0.92, 0.18),
        (-2.88, -0.25, 0.92, 1.25, 0.18),
        (2.82, -2.70, 1.18, 0.76, 0.20),
        (-2.88, -2.95, 1.05, 0.62, 0.16),
        (0.92, -1.70, 0.92, 1.02, 0.17),
    ]
    for i, (x, z, sx, sz, depth) in enumerate(large_plates):
        box(f"GEO_MajorPlate_{i + 1:02d}", (x, -0.31 - depth * 0.5, z),
            (sx, depth, sz), materials["plate"], detail,
            bevel=0.055, segments=3)

    # Authored medium greebles, skipping the three physical cavities.
    for i in range(32):
        x = random.uniform(-3.25, 3.25)
        z = random.uniform(-3.15, 3.15)
        if any(((x - bx) / (rx * 1.18)) ** 2 + ((z - bz) / (rz * 1.18)) ** 2 < 1
               for bx, bz, rx, rz, _, _ in breaches):
            continue
        sx = random.choice((0.24, 0.34, 0.46, 0.62))
        sz = random.choice((0.12, 0.18, 0.26, 0.34))
        depth = random.choice((0.08, 0.11, 0.15))
        box(f"GEO_ServicePlate_{i + 1:02d}",
            (x, -0.31 - depth * 0.5, z), (sx, depth, sz),
            materials["edge"] if i % 4 == 0 else materials["plate"], detail,
            rotation=(0, math.radians(random.choice((-4, 0, 0, 0, 5))), 0),
            bevel=0.025)

    # Three principal circuit routes with physical continuity and interruptions.
    add_surface_circuit("Circuit_MainUpper", -3.12, 0.70, 0.15, 0.70,
                        materials, static, live=False)
    add_surface_circuit("Circuit_MainRight", 0.15, 0.70, 0.15, -2.18,
                        materials, static, live=False)
    add_surface_circuit("Circuit_Lower", -3.00, -2.34, 2.72, -2.34,
                        materials, static, live=False)
    add_surface_circuit("Circuit_Node0", 0.15, 1.89, 2.19, 1.89,
                        materials, static, live=True)
    add_surface_circuit("Circuit_Node1", -1.66, 1.81, 0.15, 1.81,
                        materials, static, live=False)
    add_surface_circuit("Circuit_Node3", 0.15, 0.02, 0.0, 0.02,
                        materials, static, live=True)
    add_surface_circuit("Circuit_Node4", -1.81, 0.23, 0.15, 0.23,
                        materials, static, live=False)

    # A few secondary raised pipes; micro-noise is deliberately limited.
    for lane, offset in enumerate((-0.10, 0.0, 0.10)):
        cylinder_between(f"GEO_LowerPipe_{lane + 1}",
            (-2.95, -0.48 - lane * 0.012, -2.48 + offset),
            (2.80, -0.48 - lane * 0.012, -2.48 + offset), 0.027,
            materials["circuit"], detail, vertices=10)
    for lane, offset in enumerate((-0.08, 0.0, 0.08)):
        cylinder_between(f"GEO_CenterPipe_{lane + 1}",
            (0.18 + offset, -0.47, -2.12), (0.18 + offset, -0.47, 2.66),
            0.024, materials["circuit"], detail, vertices=10)

    node_specs = [
        ("NODE_00_z1_energy_coupling_1", 2.190, 1.888, 1.30, 1.26, "solid"),
        ("NODE_01_z1_traces_channel_1", -1.661, 1.812, 1.18, 1.14, "port"),
        ("NODE_03_z1_triangle_anchor_1", 0.0, 0.0, 1.30, 1.24, "solid"),
        # NODE_04 is deliberately absent: its housing is represented by the
        # collapsed/remnant geometry inside the primary explosion scar.
        ("NODE_07_hidden", 2.265, -1.661, 0.86, 0.72, "socket"),
        ("NODE_08_hidden", 0.0, 2.416, 0.86, 0.72, "solid"),
    ]
    for name, x, z, width, height, mode in node_specs:
        node_housing(name, x, z, width, height, materials, nodes_root, mode)

    for index, spec in enumerate(breaches, 1):
        build_breach(index, spec, materials, damage_root, repair_root)

    # Warning points are sparse and subordinate to the physical relief.
    for i, (x, z) in enumerate(((-3.28, 2.95), (3.25, 2.90),
                                (-3.22, -2.70), (3.20, -2.76))):
        box(f"Warning_{i + 1}", (x, -0.50, z), (0.11, 0.045, 0.055),
            materials["orange"], high, bevel=0.018)

    # Real cube depth so the fixed right-facing rest angle reads as volume.
    box("GEO_RightSideShell", (4.02, 1.42, 0), (0.42, 2.95, 7.82),
        materials["armor"], side_root, bevel=0.07, segments=3)
    box("GEO_TopShell", (0, 1.42, 4.02), (7.82, 2.95, 0.42),
        materials["armor"], side_root, bevel=0.07, segments=3)
    for i, z in enumerate((-2.75, -1.35, 0.05, 1.45, 2.85)):
        box(f"GEO_RightSideRib_{i + 1}", (4.27, 1.40, z),
            (0.18, 2.42, 0.22), materials["frame"], side_root,
            bevel=0.035)
    for i, y in enumerate((0.15, 0.82, 1.50, 2.18, 2.80)):
        box(f"GEO_SidePanel_{i + 1}", (4.27, y, 0.45 - i * 0.18),
            (0.16, 0.48, 1.02), materials["plate"], side_root,
            bevel=0.035)
        box(f"GEO_SideLight_{i + 1}", (4.37, y, 0.45 - i * 0.18),
            (0.025, 0.14, 0.045), materials["cyan"], side_root,
            bevel=0.010)

    # Custom properties make Unity-side binding deterministic.
    root["qf_asset_role"] = "machine_cube_master_face"
    root["qf_forward"] = "-Y"
    damage_root["qf_initially_active"] = True
    repair_root["qf_initially_active"] = False
    return root


def look_at(obj, target):
    direction = Vector(target) - obj.location
    obj.rotation_euler = direction.to_track_quat("-Z", "Y").to_euler()


def setup_render():
    scene = bpy.context.scene
    scene.render.engine = "BLENDER_EEVEE"
    scene.render.resolution_x = 1100
    scene.render.resolution_y = 1100
    scene.render.resolution_percentage = 100
    scene.render.image_settings.file_format = "PNG"
    scene.render.filepath = PREVIEW_PATH
    scene.render.film_transparent = False
    scene.render.image_settings.color_mode = "RGBA"
    scene.view_settings.look = "AgX - Medium High Contrast"
    scene.world.use_nodes = True
    world_background = scene.world.node_tree.nodes.get("Background")
    world_background.inputs["Color"].default_value = (0.0002, 0.0002, 0.0002, 1.0)
    world_background.inputs["Strength"].default_value = 0.015

    bpy.ops.object.camera_add(location=(8.7, -14.2, 4.4))
    camera = bpy.context.object
    camera.name = "PreviewCamera"
    camera.data.lens = 62
    look_at(camera, (0.0, 0.72, -0.18))
    scene.camera = camera

    def area(name, location, energy, color, size, target=(0, 0, 0)):
        bpy.ops.object.light_add(type="AREA", location=location)
        light = bpy.context.object
        light.name = name
        light.data.energy = energy
        light.data.color = color
        light.data.shape = "DISK"
        light.data.size = size
        look_at(light, target)
        return light

    area("Key_Raking", (-5.2, -7.2, 8.4), 1320,
         (0.92, 0.84, 0.74), 4.2, (-0.6, 0.0, 0.4))
    area("Fill_Cool", (5.8, -6.5, 2.0), 260,
         (0.25, 0.45, 0.68), 5.5, (0.5, 0.0, 0.0))
    area("Rim_Right", (6.5, 3.4, 6.4), 820,
         (0.28, 0.62, 0.85), 3.2, (2.2, 1.0, 0.5))
    area("Damage_Warm", (-3.4, -2.2, -0.8), 95,
         (0.78, 0.22, 0.08), 1.8, (-1.5, 0.0, -1.5))

    floor_mat = bpy.data.materials.new("PreviewBlack")
    floor_mat.use_nodes = True
    floor_bsdf = floor_mat.node_tree.nodes.get("Principled BSDF")
    set_principled_input(floor_bsdf, "Base Color", (0.001, 0.001, 0.001, 1.0))
    set_principled_input(floor_bsdf, "Roughness", 1.0)
    box("PreviewFloor", (0, 2.0, -4.42), (18.0, 18.0, 0.18),
        floor_mat, None, bevel=0.0)


def export_and_render():
    bpy.ops.wm.save_as_mainfile(filepath=BLEND_PATH)
    bpy.ops.object.select_all(action="DESELECT")
    export_root = bpy.data.objects.get("QF_FACE01_ROOT")
    export_root.select_set(True)
    for obj in export_root.children_recursive:
        obj.select_set(True)
    bpy.ops.export_scene.fbx(
        filepath=FBX_PATH,
        use_selection=True,
        object_types={"EMPTY", "MESH", "OTHER"},
        use_mesh_modifiers=True,
        mesh_smooth_type="FACE",
        add_leaf_bones=False,
        bake_anim=False,
        axis_forward="-Z",
        axis_up="Y",
        apply_unit_scale=True,
        apply_scale_options="FBX_SCALE_UNITS",
        path_mode="AUTO",
        use_custom_props=True,
    )
    bpy.context.scene.render.filepath = PREVIEW_PATH
    bpy.ops.render.render(write_still=True)
    damage_root = bpy.data.objects.get("DAMAGE_STATES")
    repair_root = bpy.data.objects.get("REPAIR_STATES")
    if damage_root and repair_root:
        damage_root.hide_render = True
        for obj in damage_root.children_recursive:
            obj.hide_render = True
        repair_root.hide_render = False
        for obj in repair_root.children_recursive:
            obj.hide_render = False
        bpy.context.scene.render.filepath = REPAIRED_PREVIEW_PATH
        bpy.ops.render.render(write_still=True)
        damage_root.hide_render = False
        for obj in damage_root.children_recursive:
            obj.hide_render = False
        repair_root.hide_render = True
        for obj in repair_root.children_recursive:
            obj.hide_render = True
    mesh_objects = [obj for obj in bpy.context.scene.objects if obj.type == "MESH"]
    triangle_count = sum(len(mesh.loop_triangles) if mesh.loop_triangles else
                         (mesh.calc_loop_triangles() or len(mesh.loop_triangles))
                         for mesh in (obj.data for obj in mesh_objects))
    print(f"[Quantum Forge Blender] Mesh objects={len(mesh_objects)} triangles={triangle_count}")


if __name__ == "__main__":
    build_model()
    setup_render()
    export_and_render()
    print("[Quantum Forge Blender] PASS")
    print(BLEND_PATH)
    print(FBX_PATH)
    print(PREVIEW_PATH)
    print(REPAIRED_PREVIEW_PATH)

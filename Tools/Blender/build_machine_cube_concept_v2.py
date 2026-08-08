import bpy
import importlib.util
import math
import os
from mathutils import Vector


HERE = os.path.dirname(os.path.abspath(__file__))
ROOT_DIR = os.path.abspath(os.path.join(HERE, "..", ".."))
BASE_SCRIPT = os.path.join(HERE, "build_machine_cube_master.py")
SPEC = importlib.util.spec_from_file_location("qf_machine_base", BASE_SCRIPT)
base = importlib.util.module_from_spec(SPEC)
SPEC.loader.exec_module(base)

OUT_DIR = os.path.join(ROOT_DIR, "Logs", "VisualQA", "MachineCubeConceptV2")
SOURCE_DIR = os.path.join(ROOT_DIR, "ArtSource", "Blender", "MachineCube")
BLEND_PATH = os.path.join(SOURCE_DIR, "QF_MachineCube_ConceptV2.blend")
PREVIEW_PATH = os.path.join(OUT_DIR, "QF_MachineCube_ConceptV2_Damaged.png")
FBX_PATH = os.path.join(ROOT_DIR, "Assets", "Project", "Art",
                        "MachineCubeBlender", "QF_MachineCube_ConceptV2.fbx")

FACE = 8.30
FRONT = -0.28


def mat(name, dark, light, metallic, roughness, bump=0.12, scale=4.0):
    material = base.industrial_material(name, dark, light, metallic, roughness,
                                        bump, scale)
    # A second, broad cellular bump introduces hairline stress cracking and
    # pitting in the armor itself. It is subtle enough to remain material
    # detail, while the macro damage stays real geometry.
    nodes = material.node_tree.nodes
    links = material.node_tree.links
    bsdf = next((node for node in nodes if node.bl_idname ==
                 "ShaderNodeBsdfPrincipled"), None)
    coordinates = next((node for node in nodes if node.bl_idname ==
                        "ShaderNodeTexCoord"), None)
    first_bump = next((node for node in nodes if node.bl_idname ==
                      "ShaderNodeBump"), None)
    if bsdf and coordinates and first_bump:
        for link in list(links):
            if link.to_node == bsdf and link.to_socket == bsdf.inputs.get("Normal"):
                links.remove(link)
        crack = nodes.new("ShaderNodeTexVoronoi")
        crack.feature = "DISTANCE_TO_EDGE"
        crack.distance = "EUCLIDEAN"
        crack.inputs["Scale"].default_value = 3.6
        crack.inputs["Randomness"].default_value = 0.82
        crack_bump = nodes.new("ShaderNodeBump")
        crack_bump.inputs["Strength"].default_value = 0.16
        crack_bump.inputs["Distance"].default_value = 0.026
        links.new(coordinates.outputs["Object"], crack.inputs["Vector"])
        links.new(crack.outputs["Distance"], crack_bump.inputs["Height"])
        links.new(first_bump.outputs["Normal"], crack_bump.inputs["Normal"])
        links.new(crack_bump.outputs["Normal"], bsdf.inputs["Normal"])
    return material


def pbr_panel_material(name, panel_number, tint, metallic=0.68,
                       roughness=0.44, tile=1.0):
    folder = os.path.join(ROOT_DIR, "Assets", "SciFi_Materials_vol2",
                          "Textures", f"SciFiPanels{panel_number:02d}")
    paths = {suffix: os.path.join(folder,
             f"SciFiPanels{panel_number:02d}_{suffix}.png")
             for suffix in ("a", "n", "m", "o", "h")}
    material = bpy.data.materials.new(name)
    material.use_nodes = True
    nodes = material.node_tree.nodes
    links = material.node_tree.links
    nodes.clear()
    output = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    coordinates = nodes.new("ShaderNodeTexCoord")
    mapping = nodes.new("ShaderNodeMapping")
    mapping.vector_type = "POINT"
    mapping.inputs["Scale"].default_value = (tile, tile, tile)
    links.new(coordinates.outputs["Generated"], mapping.inputs["Vector"])

    albedo = nodes.new("ShaderNodeTexImage")
    albedo.image = bpy.data.images.load(paths["a"], check_existing=True)
    albedo.projection = "BOX"
    albedo.projection_blend = 0.22
    links.new(mapping.outputs["Vector"], albedo.inputs["Vector"])
    tint_mix = nodes.new("ShaderNodeMixRGB")
    tint_mix.blend_type = "MULTIPLY"
    tint_mix.inputs[0].default_value = 1.0
    tint_mix.inputs[2].default_value = (*tint, 1.0)
    links.new(albedo.outputs["Color"], tint_mix.inputs[1])

    occlusion = nodes.new("ShaderNodeTexImage")
    occlusion.image = bpy.data.images.load(paths["o"], check_existing=True)
    occlusion.image.colorspace_settings.name = "Non-Color"
    occlusion.projection = "BOX"
    occlusion.projection_blend = 0.22
    links.new(mapping.outputs["Vector"], occlusion.inputs["Vector"])
    ao_mix = nodes.new("ShaderNodeMixRGB")
    ao_mix.blend_type = "MULTIPLY"
    ao_mix.inputs[0].default_value = 0.72
    links.new(tint_mix.outputs["Color"], ao_mix.inputs[1])
    links.new(occlusion.outputs["Color"], ao_mix.inputs[2])
    dirt_noise = nodes.new("ShaderNodeTexNoise")
    dirt_noise.inputs["Scale"].default_value = 6.5
    dirt_noise.inputs["Detail"].default_value = 7.0
    dirt_noise.inputs["Roughness"].default_value = 0.82
    links.new(mapping.outputs["Vector"], dirt_noise.inputs["Vector"])
    dirt_ramp = nodes.new("ShaderNodeValToRGB")
    dirt_ramp.color_ramp.elements[0].position = 0.30
    dirt_ramp.color_ramp.elements[0].color = (0.22, 0.215, 0.205, 1.0)
    dirt_ramp.color_ramp.elements[1].position = 0.72
    dirt_ramp.color_ramp.elements[1].color = (0.94, 0.91, 0.84, 1.0)
    links.new(dirt_noise.outputs["Fac"], dirt_ramp.inputs["Fac"])
    weather_mix = nodes.new("ShaderNodeMixRGB")
    weather_mix.blend_type = "MULTIPLY"
    weather_mix.inputs[0].default_value = 0.70
    links.new(ao_mix.outputs["Color"], weather_mix.inputs[1])
    links.new(dirt_ramp.outputs["Color"], weather_mix.inputs[2])
    links.new(weather_mix.outputs["Color"], bsdf.inputs["Base Color"])

    normal_tex = nodes.new("ShaderNodeTexImage")
    normal_tex.image = bpy.data.images.load(paths["n"], check_existing=True)
    normal_tex.image.colorspace_settings.name = "Non-Color"
    normal_tex.projection = "BOX"
    normal_tex.projection_blend = 0.22
    links.new(mapping.outputs["Vector"], normal_tex.inputs["Vector"])
    normal_map = nodes.new("ShaderNodeNormalMap")
    normal_map.inputs["Strength"].default_value = 0.56
    links.new(normal_tex.outputs["Color"], normal_map.inputs["Color"])

    height_tex = nodes.new("ShaderNodeTexImage")
    height_tex.image = bpy.data.images.load(paths["h"], check_existing=True)
    height_tex.image.colorspace_settings.name = "Non-Color"
    height_tex.projection = "BOX"
    height_tex.projection_blend = 0.22
    links.new(mapping.outputs["Vector"], height_tex.inputs["Vector"])
    height_bump = nodes.new("ShaderNodeBump")
    height_bump.inputs["Strength"].default_value = 0.16
    height_bump.inputs["Distance"].default_value = 0.028
    links.new(height_tex.outputs["Color"], height_bump.inputs["Height"])
    links.new(normal_map.outputs["Normal"], height_bump.inputs["Normal"])
    crack_tex = nodes.new("ShaderNodeTexVoronoi")
    crack_tex.feature = "DISTANCE_TO_EDGE"
    crack_tex.distance = "EUCLIDEAN"
    crack_tex.inputs["Scale"].default_value = 4.5
    crack_tex.inputs["Randomness"].default_value = 0.78
    links.new(mapping.outputs["Vector"], crack_tex.inputs["Vector"])
    crack_bump = nodes.new("ShaderNodeBump")
    crack_bump.inputs["Strength"].default_value = 0.17
    crack_bump.inputs["Distance"].default_value = 0.021
    links.new(crack_tex.outputs["Distance"], crack_bump.inputs["Height"])
    links.new(height_bump.outputs["Normal"], crack_bump.inputs["Normal"])
    links.new(crack_bump.outputs["Normal"], bsdf.inputs["Normal"])

    metallic_tex = nodes.new("ShaderNodeTexImage")
    metallic_tex.image = bpy.data.images.load(paths["m"], check_existing=True)
    metallic_tex.image.colorspace_settings.name = "Non-Color"
    metallic_tex.projection = "BOX"
    metallic_tex.projection_blend = 0.22
    links.new(mapping.outputs["Vector"], metallic_tex.inputs["Vector"])
    metal_mix = nodes.new("ShaderNodeMath")
    metal_mix.operation = "MULTIPLY"
    metal_mix.inputs[1].default_value = metallic
    links.new(metallic_tex.outputs["Color"], metal_mix.inputs[0])
    links.new(metal_mix.outputs["Value"], bsdf.inputs["Metallic"])
    set_rough = bsdf.inputs.get("Roughness")
    if set_rough:
        set_rough.default_value = roughness
        rough_map = nodes.new("ShaderNodeMapRange")
        rough_map.inputs["From Min"].default_value = 0.0
        rough_map.inputs["From Max"].default_value = 1.0
        rough_map.inputs["To Min"].default_value = max(0.18,
                                                        roughness - 0.23)
        rough_map.inputs["To Max"].default_value = min(0.92,
                                                        roughness + 0.25)
        links.new(dirt_noise.outputs["Fac"], rough_map.inputs["Value"])
        links.new(rough_map.outputs["Result"], set_rough)
    links.new(bsdf.outputs["BSDF"], output.inputs["Surface"])
    return material


def soot_material(name):
    """Mottled translucent soot decal, never a solid black polygon."""
    material = bpy.data.materials.new(name)
    material.use_nodes = True
    if hasattr(material, "surface_render_method"):
        material.surface_render_method = "DITHERED"
    elif hasattr(material, "blend_method"):
        material.blend_method = "HASHED"
    nodes = material.node_tree.nodes
    links = material.node_tree.links
    nodes.clear()
    output = nodes.new("ShaderNodeOutputMaterial")
    transparent = nodes.new("ShaderNodeBsdfTransparent")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    bsdf.inputs["Base Color"].default_value = (0.008, 0.009, 0.010, 1.0)
    bsdf.inputs["Metallic"].default_value = 0.0
    bsdf.inputs["Roughness"].default_value = 0.98
    coordinates = nodes.new("ShaderNodeTexCoord")
    noise = nodes.new("ShaderNodeTexNoise")
    noise.inputs["Scale"].default_value = 4.2
    noise.inputs["Detail"].default_value = 8.0
    noise.inputs["Roughness"].default_value = 0.82
    ramp = nodes.new("ShaderNodeValToRGB")
    ramp.color_ramp.elements[0].position = 0.18
    ramp.color_ramp.elements[0].color = (0.08, 0.08, 0.08, 1.0)
    ramp.color_ramp.elements[1].position = 0.66
    ramp.color_ramp.elements[1].color = (0.82, 0.82, 0.82, 1.0)
    mix = nodes.new("ShaderNodeMixShader")
    links.new(coordinates.outputs["Generated"], noise.inputs["Vector"])
    links.new(noise.outputs["Fac"], ramp.inputs["Fac"])
    links.new(ramp.outputs["Color"], mix.inputs[0])
    links.new(transparent.outputs["BSDF"], mix.inputs[1])
    links.new(bsdf.outputs["BSDF"], mix.inputs[2])
    links.new(mix.outputs["Shader"], output.inputs["Surface"])
    return material


def polygon_prism(name, points, center_y, depth, material, parent=None):
    count = len(points)
    front_y = center_y - depth * 0.5
    back_y = center_y + depth * 0.5
    vertices = [(x, front_y, z) for x, z in points]
    vertices += [(x, back_y, z) for x, z in points]
    faces = []
    faces.append(tuple(reversed(range(count))))
    faces.append(tuple(range(count, count * 2)))
    for i in range(count):
        j = (i + 1) % count
        faces.append((i, j, count + j, count + i))
    mesh = bpy.data.meshes.new(name + "_Mesh")
    mesh.from_pydata(vertices, [], faces)
    mesh.update()
    obj = bpy.data.objects.new(name, mesh)
    bpy.context.scene.collection.objects.link(obj)
    if material:
        obj.data.materials.append(material)
    obj.parent = parent
    return obj


def bent_plate(name, vertices_xzy, depth, material, parent=None):
    """Irregular armor shard still anchored to the surrounding skin.

    Each source vertex is (x, z, y), allowing the free edge to buckle out of
    plane instead of reading as a flat decal or a loose piece of debris.
    """
    count = len(vertices_xzy)
    front = [(x, y, z) for x, z, y in vertices_xzy]
    back = [(x, y + depth, z) for x, z, y in vertices_xzy]
    vertices = front + back
    faces = [tuple(reversed(range(count))),
             tuple(range(count, count * 2))]
    for i in range(count):
        j = (i + 1) % count
        faces.append((i, j, count + j, count + i))
    mesh = bpy.data.meshes.new(name + "_Mesh")
    mesh.from_pydata(vertices, [], faces)
    mesh.update()
    obj = bpy.data.objects.new(name, mesh)
    bpy.context.scene.collection.objects.link(obj)
    if material:
        obj.data.materials.append(material)
    obj.parent = parent
    base.apply_bevel(obj, 0.018, 2)
    return obj


def inset_polygon(points, center, scale):
    cx, cz = center
    return [(cx + (x - cx) * scale, cz + (z - cz) * scale)
            for x, z in points]


def soot_stain(name, points, center, material, parent, outer_scale=1.18):
    """Thin irregular band of smoke staining anchored to the armor."""
    inner = inset_polygon(points, center, 1.01)
    outer = inset_polygon(points, center, outer_scale)
    count = len(points)
    vertices = [(x, -0.603, z) for x, z in outer]
    vertices += [(x, -0.606, z) for x, z in inner]
    faces = []
    for i in range(count):
        j = (i + 1) % count
        if (i * 3 + count) % 7 == 0:
            continue
        faces.append((i, j, count + j, count + i))
    mesh = bpy.data.meshes.new(name + "_Mesh")
    mesh.from_pydata(vertices, [], faces)
    mesh.update()
    obj = bpy.data.objects.new(name, mesh)
    bpy.context.scene.collection.objects.link(obj)
    obj.data.materials.append(material)
    obj.parent = parent
    if hasattr(obj, "visible_shadow"):
        obj.visible_shadow = False
    return obj


def edge_wall_segments(name, points, y, depth, width, material, parent,
                       skip=None):
    skip = set(skip or [])
    for i, p1 in enumerate(points):
        if i in skip:
            continue
        p2 = points[(i + 1) % len(points)]
        dx, dz = p2[0] - p1[0], p2[1] - p1[1]
        length = math.sqrt(dx * dx + dz * dz)
        angle = -math.atan2(dz, dx)
        base.box(f"{name}_{i + 1:02d}",
                 ((p1[0] + p2[0]) * 0.5, y,
                  (p1[1] + p2[1]) * 0.5),
                 (length * 0.90, depth, width), material, parent,
                 rotation=(0, angle, 0), bevel=min(0.025, width * 0.18),
                 segments=2)


def front_bolt(name, x, z, radius, material, parent, y=-0.54):
    bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=radius,
        depth=0.055, location=(x, y, z), rotation=(math.pi * 0.5, 0, 0))
    obj = bpy.context.object
    obj.name = name
    obj.data.materials.append(material)
    obj.parent = parent
    base.apply_bevel(obj, 0.008, 2)
    return obj


def mechanical_coupling(name, x, z, scale, materials, parent, y=-0.30):
    """Layered actuator coupling viewed through a torn service cavity."""
    bpy.ops.mesh.primitive_cylinder_add(vertices=18, radius=0.25 * scale,
        depth=0.16 * scale, location=(x, y, z),
        rotation=(math.pi * 0.5, 0, 0))
    outer = bpy.context.object
    outer.name = name + "_Outer"
    outer.data.materials.append(materials.get("machinery", materials["edge"]))
    outer.parent = parent
    base.apply_bevel(outer, 0.018 * scale, 2)
    bpy.ops.mesh.primitive_cylinder_add(vertices=14, radius=0.145 * scale,
        depth=0.11 * scale, location=(x, y - 0.11 * scale, z),
        rotation=(math.pi * 0.5, 0, 0))
    core = bpy.context.object
    core.name = name + "_Core"
    core.data.materials.append(materials["raw"])
    core.parent = parent
    base.apply_bevel(core, 0.014 * scale, 2)
    for i, angle in enumerate((0.0, math.pi * 0.5, math.pi,
                               math.pi * 1.5)):
        front_bolt(name + f"_Pin_{i}",
                   x + math.cos(angle) * 0.19 * scale,
                   z + math.sin(angle) * 0.19 * scale,
                   0.027 * scale, materials["raw"], parent,
                   y=y - 0.12 * scale)
    return outer


def layered_node(name, x, z, w, h, materials, parent, mode="solid"):
    root = base.make_empty(name, parent)
    base.box(name + "_Back", (x, -0.20, z), (w * 1.06, 0.22, h * 1.06),
             materials["inner"], root, bevel=0.06, segments=3)
    base.frame_rect(name + "_Outer", x, z, w, h, 0.26, 0.16,
                    materials["frame"], root, y=-0.38, bevel=0.045)
    base.frame_rect(name + "_Step", x, z, w * 0.78, h * 0.78, 0.22, 0.11,
                    materials["edge"], root, y=-0.52, bevel=0.035)
    base.frame_rect(name + "_InnerLip", x, z, w * 0.60, h * 0.60,
                    0.105, 0.075, materials["circuit"], root,
                    y=-0.635, bevel=0.024)
    base.corner_braces(name + "_Clamp", x, z, w, h, materials["edge"], root,
                       y=-0.64, scale=max(w, h) * 0.92)
    for i, (sx, sz) in enumerate(((-1, 1), (1, 1), (-1, -1), (1, -1))):
        front_bolt(name + f"_Fastener_{i}", x + sx * w * 0.34,
                   z + sz * h * 0.34, 0.030, materials["raw"], root,
                   y=-0.665)
    base.box(name + "_ServiceTop", (x, -0.67, z + h * 0.30),
             (w * 0.30, 0.08, 0.055), materials["circuit"], root,
             bevel=0.015, segments=2)
    base.box(name + "_ServiceBottom", (x + w * 0.12, -0.67,
             z - h * 0.30), (w * 0.22, 0.08, 0.050),
             materials["circuit"], root, bevel=0.015, segments=2)
    if mode == "port":
        base.box(name + "_Void", (x, -0.47, z),
                 (w * 0.48, 0.36, h * 0.48), materials["cavity"], root,
                 bevel=0.075, segments=3)
        for ox in (-w * 0.15, w * 0.15):
            base.cylinder_between(name + f"_Rail_{ox:+.2f}",
                (x + ox, -0.69, z - h * 0.22),
                (x + ox, -0.69, z + h * 0.22), 0.024,
                materials["copper"], root, vertices=10)
    else:
        base.box(name + "_Core", (x, -0.66, z),
                 (w * 0.50, 0.20, h * 0.50), materials["plate"], root,
                 bevel=0.065, segments=3)
        if mode == "socket":
            for ox in (-w * 0.13, w * 0.13):
                for oz in (-h * 0.13, h * 0.13):
                    front_bolt(name + f"_Light_{ox:+.2f}_{oz:+.2f}",
                               x + ox, z + oz, 0.055,
                               materials["cyan"], root, y=-0.79)
    return root


def damaged_port_node(name, x, z, w, h, materials, parent):
    """Top-left service port whose lower housing was consumed by the scar."""
    root = base.make_empty(name, parent)
    base.box(name + "_Back", (x, -0.20, z + h * 0.10),
             (w * 1.02, 0.20, h * 0.82), materials["inner"], root,
             bevel=0.055, segments=3)
    # Surviving outer housing: full top and sides, only a short lower-right
    # segment. The missing lower-left section connects directly to the blast.
    base.box(name + "_OuterTop", (x, -0.43, z + h * 0.50),
             (w, 0.20, 0.19), materials["frame"], root,
             bevel=0.038, segments=3)
    base.box(name + "_OuterLeft", (x - w * 0.50, -0.43, z + h * 0.08),
             (0.19, 0.20, h * 0.66), materials["frame"], root,
             bevel=0.038, segments=3)
    base.box(name + "_OuterRight", (x + w * 0.50, -0.43, z + h * 0.03),
             (0.19, 0.20, h * 0.78), materials["frame"], root,
             rotation=(math.radians(2), math.radians(-2), 0),
             bevel=0.038, segments=3)
    base.box(name + "_BottomRemnant", (x + w * 0.28, -0.50,
             z - h * 0.44), (w * 0.38, 0.17, 0.18),
             materials["raw"], root,
             rotation=(math.radians(8), math.radians(-12),
                       math.radians(-5)), bevel=0.032, segments=3)
    base.frame_rect(name + "_InnerThroat", x, z + h * 0.08,
                    w * 0.66, h * 0.63, 0.13, 0.09,
                    materials["edge"], root, y=-0.57, bevel=0.026)
    base.box(name + "_Void", (x, -0.47, z + h * 0.09),
             (w * 0.46, 0.34, h * 0.42), materials["cavity"], root,
             bevel=0.065, segments=3)
    for i, ox in enumerate((-w * 0.13, w * 0.13)):
        base.cylinder_between(name + f"_ThroatRail_{i}",
            (x + ox, -0.68, z - h * 0.08),
            (x + ox, -0.68, z + h * 0.28), 0.021,
            materials["edge"], root, vertices=10)
    for i, (sx, sz) in enumerate(((-1, 1), (1, 1), (1, -1))):
        base.box(name + f"_Clamp_{i}",
                 (x + sx * w * 0.43, -0.62, z + sz * h * 0.42),
                 (0.34, 0.14, 0.14), materials["edge"], root,
                 rotation=(0, math.radians(-sx * sz * 42), 0),
                 bevel=0.030, segments=3)
    return root


def circuit_path(name, points, material, parent, y=-0.47, width=0.035):
    for i in range(len(points) - 1):
        a, b = points[i], points[i + 1]
        if abs(a[0] - b[0]) < 0.001:
            base.box(f"{name}_{i:02d}", (a[0], y, (a[1] + b[1]) * 0.5),
                     (width, 0.055, abs(a[1] - b[1])), material, parent,
                     bevel=width * 0.28, segments=2)
        elif abs(a[1] - b[1]) < 0.001:
            base.box(f"{name}_{i:02d}", ((a[0] + b[0]) * 0.5, y, a[1]),
                     (abs(a[0] - b[0]), 0.055, width), material, parent,
                     bevel=width * 0.28, segments=2)


def tear_tab(name, anchor, direction, size, material, parent, angle=0.0):
    ax, az = anchor
    dx, dz = direction
    length = math.sqrt(dx * dx + dz * dz) or 1.0
    dx, dz = dx / length, dz / length
    px, pz = -dz, dx
    tip = (ax + dx * size, az + dz * size)
    points = [(ax - px * size * 0.28, az - pz * size * 0.28),
              (ax + px * size * 0.28, az + pz * size * 0.28), tip]
    obj = polygon_prism(name, points, -0.45, 0.16, material, parent)
    obj.rotation_euler.x = math.radians(angle)
    base.apply_bevel(obj, 0.018, 2)
    return obj


def broken_housing(name, x, z, w, h, materials, parent, bottom=False):
    """A recognisable node frame whose missing segments coincide with a cut."""
    root = base.make_empty(name, parent)
    # Rear shoulders remain embedded in the undamaged skin.
    base.box(name + "_TopLeft", (x - w * 0.27, -0.56, z + h * 0.48),
             (w * 0.42, 0.22, 0.14), materials["frame"], root,
             rotation=(math.radians(-4), math.radians(6), math.radians(-3)),
             bevel=0.035, segments=3)
    base.box(name + "_TopRight", (x + w * 0.31, -0.53, z + h * 0.45),
             (w * 0.34, 0.20, 0.14), materials["edge"], root,
             rotation=(math.radians(7), math.radians(-12), math.radians(4)),
             bevel=0.032, segments=3)
    base.box(name + "_LeftRemnant", (x - w * 0.48, -0.55, z + h * 0.02),
             (0.14, 0.23, h * 0.57), materials["frame"], root,
             rotation=(math.radians(-5), math.radians(5), math.radians(-3)),
             bevel=0.035, segments=3)
    base.box(name + "_RightCollapsed", (x + w * 0.43, -0.66, z - h * 0.07),
             (0.16, 0.25, h * 0.49), materials["raw"], root,
             rotation=(math.radians(14), math.radians(23), math.radians(-7)),
             bevel=0.032, segments=3)
    if not bottom:
        base.box(name + "_BottomLeft", (x - w * 0.25, -0.59, z - h * 0.46),
                 (w * 0.38, 0.21, 0.14), materials["edge"], root,
                 rotation=(math.radians(-6), math.radians(-9),
                           math.radians(4)), bevel=0.032, segments=3)
    # A buckled inner cartridge is connected to the housing by the brace.
    base.box(name + "_BuckledCartridge", (x + w * 0.10, -0.61,
             z - h * (0.08 if bottom else 0.02)),
             (w * 0.27, 0.15, h * 0.24),
             materials.get("machinery", materials["raw"]), root,
             rotation=(math.radians(12), math.radians(-20),
                       math.radians(11 if bottom else -8)),
             bevel=0.04, segments=3)
    base.cylinder_between(name + "_MountBrace",
        (x - w * 0.28, -0.46, z + h * 0.20),
        (x + w * 0.18, -0.62, z - h * 0.17), 0.058,
        materials["edge"], root, vertices=12)
    return root


def surface_crack(name, points, material, parent, width=0.022):
    # Damage marks sit on the foremost armor layer; the earlier depth hid
    # them behind raised panels and made the blast look unnaturally isolated.
    circuit_path(name, points, material, parent, y=-0.622, width=width)


def rough_damage_ring(name, points, center, char_material, raw_material,
                      parent, width=0.16):
    """Crumpled multi-band transition from intact armor into a breach."""
    cx, cz = center
    sampled = []
    for i, point in enumerate(points):
        nxt = points[(i + 1) % len(points)]
        sampled.append(point)
        # Off-centre midpoint prevents the source polygon from reading as a
        # low-sided manufactured opening.
        t = 0.42 + 0.10 * (i % 3)
        mx = point[0] * (1.0 - t) + nxt[0] * t
        mz = point[1] * (1.0 - t) + nxt[1] * t
        tangent_x, tangent_z = nxt[0] - point[0], nxt[1] - point[1]
        length = math.sqrt(tangent_x * tangent_x + tangent_z * tangent_z) or 1.0
        normal_x, normal_z = -tangent_z / length, tangent_x / length
        jitter = 0.035 * ((i % 4) - 1.5)
        sampled.append((mx + normal_x * jitter, mz + normal_z * jitter))

    count = len(sampled)
    bands = [[], [], []]
    for i, (x, z) in enumerate(sampled):
        outer_scale = 1.0 + width * (0.70 + 0.12 * ((i * 5) % 5))
        middle_scale = 1.0 + width * (0.08 + 0.05 * (i % 4))
        inner_scale = 0.90 - 0.025 * ((i + 1) % 4)
        bands[0].append((cx + (x - cx) * outer_scale,
                         -0.350 - 0.016 * (i % 4),
                         cz + (z - cz) * outer_scale))
        bands[1].append((cx + (x - cx) * middle_scale,
                         -0.455 - 0.045 * ((i * 3) % 5),
                         cz + (z - cz) * middle_scale))
        bands[2].append((cx + (x - cx) * inner_scale,
                         -0.590 - 0.030 * ((i + 2) % 4),
                         cz + (z - cz) * inner_scale))

    vertices = bands[0] + bands[1] + bands[2]
    faces = []
    for band in range(2):
        start_a = band * count
        start_b = (band + 1) * count
        for i in range(count):
            phase = 1 if "Secondary" in name else (4 if "Bottom" in name else 0)
            if (i + phase) % 10 >= 7:
                continue
            j = (i + 1) % count
            if (i + band) % 2 == 0:
                faces.append((start_a + i, start_a + j, start_b + j))
                faces.append((start_a + i, start_b + j, start_b + i))
            else:
                faces.append((start_a + i, start_a + j, start_b + i))
                faces.append((start_a + j, start_b + j, start_b + i))
    mesh = bpy.data.meshes.new(name + "_Mesh")
    mesh.from_pydata(vertices, [], faces)
    mesh.materials.append(char_material)
    mesh.materials.append(raw_material)
    mesh.update()
    obj = bpy.data.objects.new(name, mesh)
    bpy.context.scene.collection.objects.link(obj)
    obj.parent = parent
    for i, polygon in enumerate(mesh.polygons):
        polygon.material_index = 1 if i % 17 == 0 else 0
        polygon.use_smooth = True
    bpy.context.view_layer.objects.active = obj
    obj.select_set(True)
    subdivide = obj.modifiers.new("FractureSubdivision", "SUBSURF")
    subdivide.subdivision_type = "SIMPLE"
    subdivide.levels = 2
    subdivide.render_levels = 2
    bpy.ops.object.modifier_apply(modifier=subdivide.name)
    texture = bpy.data.textures.new(name + "_CrumpleNoise", type="CLOUDS")
    texture.noise_scale = 0.18
    texture.noise_depth = 2
    displace = obj.modifiers.new("FractureCrumple", "DISPLACE")
    displace.texture = texture
    displace.strength = 0.030
    displace.mid_level = 0.48
    displace.texture_coords = "GLOBAL"
    bpy.ops.object.modifier_apply(modifier=displace.name)
    obj.select_set(False)
    base.apply_bevel(obj, 0.012, 1)
    return obj


def build_damage(name, points, center, materials, root, primary=False):
    cx, cz = center
    cavity_points = inset_polygon(points, center, 0.86)
    polygon_prism(name + "_DeepVoid", cavity_points, 0.54, 0.16,
                  materials["cavity"], root)
    polygon_prism(name + "_InnerLayer", inset_polygon(points, center, 0.67),
                  0.31, 0.13, materials["inner"], root)
    # The transition into the cavity is supplied by one continuous crumpled
    # ring later. Repeated radial boxes/tabs are deliberately avoided.

    if primary:
        # Collapsed actuator, structural ribs and cables follow the vertical
        # blast channel. Every element terminates inside the opening.
        for i, offset in enumerate((-0.46, -0.12, 0.32)):
            base.box(name + f"_RearRail_{i}",
                (cx + offset, -0.10 - i * 0.018, cz - 0.04),
                (0.075, 0.10, 1.82 - i * 0.12),
                materials["machinery"], root,
                rotation=(math.radians(2 * i), math.radians(-3 + i * 3),
                          math.radians(-4 + i * 4)),
                bevel=0.018, segments=2)
        base.box(name + "_RearCrossTop", (cx - 0.03, -0.13, cz + 0.67),
                 (1.12, 0.10, 0.09), materials["machinery"], root,
                 rotation=(math.radians(2), math.radians(-5),
                           math.radians(-6)), bevel=0.018, segments=2)
        base.box(name + "_RearCrossLow", (cx + 0.08, -0.12, cz - 0.63),
                 (0.98, 0.10, 0.08), materials["machinery"], root,
                 rotation=(math.radians(-3), math.radians(4),
                           math.radians(8)), bevel=0.016, segments=2)
        # Three attached service cells populate the rear rack. Their alignment
        # to the rails makes them read as surviving machinery, not debris.
        for i, (ox, oz, angle) in enumerate((
                (-0.39, 0.72, -4), (0.36, 0.02, -5),
                (-0.35, -0.70, -6))):
            base.box(name + f"_RearCell_{i}",
                     (cx + ox, -0.245 - 0.008 * (i % 2), cz + oz),
                     (0.28 if i % 2 else 0.32, 0.11,
                      0.30 if i < 4 else 0.24),
                     materials["machinery"] if i % 3 else
                     materials["inner"], root,
                     rotation=(math.radians((i % 3) - 1),
                               math.radians(angle),
                               math.radians(angle * 0.35)),
                     bevel=0.025, segments=3)
        mechanical_coupling(name + "_UpperCoupling", cx - 0.34,
                            cz + 0.52, 1.0, materials, root, y=-0.31)
        mechanical_coupling(name + "_LowerCoupling", cx + 0.25,
                            cz - 0.57, 0.82, materials, root, y=-0.34)
        base.cylinder_between(name + "_ActuatorShell",
            (cx - 0.32, -0.33, cz + 0.70),
            (cx + 0.18, -0.39, cz - 0.45), 0.145,
            materials["raw"], root, vertices=16)
        base.cylinder_between(name + "_ActuatorCore",
            (cx - 0.27, -0.48, cz + 0.59),
            (cx + 0.11, -0.53, cz - 0.32), 0.060,
            materials["copper"], root, vertices=12)
        for offset in (-0.52, 0.45):
            base.cylinder_between(name + f"_Rib_{offset:+.2f}",
                (cx + offset, -0.18, cz - 0.72),
                (cx + offset * 0.72, -0.20, cz + 0.78), 0.045,
                materials["edge"], root, vertices=10)
        base.cylinder_between(name + "_CrossBraceA",
            (cx - 0.62, -0.31, cz + 0.16),
            (cx + 0.48, -0.48, cz - 0.42), 0.050,
            materials["edge"], root, vertices=10)
        base.cylinder_between(name + "_CrossBraceB",
            (cx - 0.48, -0.24, cz - 0.55),
            (cx + 0.36, -0.46, cz + 0.52), 0.038,
            materials["raw"], root, vertices=10)
        base.cable(name + "_CablePower",
            [(cx - 0.55, -0.30, cz + 0.46),
             (cx - 0.19, -0.55, cz + 0.10),
             (cx + 0.18, -0.60, cz - 0.30)], 0.027,
            materials["copper"], root)
        base.cable(name + "_CableDead",
            [(cx + 0.49, -0.27, cz + 0.20),
             (cx + 0.22, -0.52, cz - 0.08),
             (cx - 0.12, -0.57, cz - 0.55)], 0.020,
            materials["circuit"], root)
        base.cable(name + "_Arc",
            [(cx - 0.20, -0.68, cz - 0.34),
             (cx - 0.09, -0.76, cz - 0.43),
             (cx + 0.02, -0.69, cz - 0.56)], 0.013,
            materials["cyan"], root)
    else:
        base.box(name + "_RearWeb", (cx, -0.20, cz),
                 (0.78, 0.10, 0.54), materials["inner"], root,
                 rotation=(math.radians(2), math.radians(-4),
                           math.radians(3)), bevel=0.032, segments=3)
        base.box(name + "_RearWebRail", (cx - 0.08, -0.29, cz + 0.02),
                 (0.10, 0.09, 0.72), materials["machinery"], root,
                 rotation=(math.radians(-3), math.radians(6),
                           math.radians(-8)), bevel=0.020, segments=2)
        mechanical_coupling(name + "_Coupling", cx - 0.12,
                            cz + 0.08, 0.82, materials, root, y=-0.31)
        base.cylinder_between(name + "_BrokenBrace",
            (cx - 0.38, -0.33, cz + 0.28),
            (cx + 0.30, -0.38, cz - 0.25), 0.075,
            materials["machinery"], root, vertices=12)
        base.cable(name + "_Cable",
            [(cx - 0.42, -0.28, cz + 0.12),
             (cx - 0.06, -0.55, cz - 0.05),
             (cx + 0.30, -0.58, cz - 0.30)], 0.021,
            materials["copper"], root)


def organize_unity_states(root, damage_root, node_root, materials,
                          repair_shapes):
    """Groups authored damage into the three repair stages used by Unity."""
    damage_root.name = "DAMAGE_STATES"
    original_damage = list(damage_root.children)
    damaged_port = bpy.data.objects.get("C2_Node_TopLeft")
    if damaged_port is not None:
        original_damage.append(damaged_port)

    damage_stages = [base.make_empty(f"DamageStage_{i + 1:02d}", damage_root)
                     for i in range(3)]

    def target_stage(name):
        lower = name.lower()
        if ("primary" in lower or "node_topleft" in lower or
                "fractureray_0" in lower or "fractureray_1" in lower or
                "extracrack_0" in lower or "extracrack_1" in lower or
                "extracrack_2" in lower):
            return damage_stages[0]
        if ("secondary" in lower or "fractureray_2" in lower or
                "extracrack_3" in lower or "extracrack_4" in lower):
            return damage_stages[1]
        return damage_stages[2]

    for obj in original_damage:
        if obj in damage_stages:
            continue
        world = obj.matrix_world.copy()
        obj.parent = target_stage(obj.name)
        obj.matrix_world = world

    repairs = base.make_empty("REPAIR_STATES", root)
    repair_stages = [base.make_empty(f"RepairStage_{i + 1:02d}", repairs)
                     for i in range(3)]
    for i, (points, center) in enumerate(repair_shapes):
        patch = polygon_prism(f"C2_RepairArmorPatch_{i + 1}",
                              inset_polygon(points, center, 0.985),
                              -0.47, 0.16, materials["skin"],
                              repair_stages[i])
        base.apply_bevel(patch, 0.025, 2)
    layered_node("C2_RepairNode_Primary", -2.48, 2.12, 1.48, 1.44,
                 materials, repair_stages[0], "port")
    layered_node("C2_RepairNode_Secondary", 0.75, -1.80, 1.38, 1.30,
                 materials, repair_stages[1], "solid")
    layered_node("C2_RepairNode_Bottom", -0.66, -3.23, 1.34, 1.02,
                 materials, repair_stages[2], "solid")
    for stage in repair_stages:
        stage.hide_render = True
        for obj in stage.children_recursive:
            obj.hide_render = True
    return repairs


def build_scene():
    base.clean_scene()
    os.makedirs(OUT_DIR, exist_ok=True)
    os.makedirs(SOURCE_DIR, exist_ok=True)

    materials = {
        "skin": mat("C2_SKIN_GRAPHITE", (0.018, 0.020, 0.021),
                    (0.135, 0.122, 0.102), 0.62, 0.57, 0.18, 2.8),
        "plate": mat("C2_ARMOR_PLATE", (0.028, 0.029, 0.028),
                     (0.188, 0.164, 0.130), 0.72, 0.45, 0.16, 4.4),
        "frame": mat("C2_HEAVY_FRAME", (0.025, 0.026, 0.026),
                     (0.235, 0.202, 0.154), 0.78, 0.38, 0.13, 3.2),
        "edge": mat("C2_NODE_EDGE", (0.022, 0.023, 0.023),
                    (0.125, 0.115, 0.095), 0.70, 0.48, 0.14, 5.0),
        "inner": mat("C2_INNER_STRUCTURE", (0.012, 0.014, 0.016),
                     (0.095, 0.100, 0.105), 0.52, 0.64, 0.18, 7.0),
        "cavity": mat("C2_CARBON_VOID", (0.010, 0.012, 0.014),
                      (0.080, 0.084, 0.090), 0.38, 0.78, 0.16, 9.0),
        "char": mat("C2_CHARRED_FRACTURE", (0.0012, 0.0013, 0.0014),
                    (0.030, 0.032, 0.034), 0.10, 0.91, 0.32, 10.0),
        "raw": mat("C2_TORN_RAW_METAL", (0.025, 0.026, 0.027),
                   (0.15, 0.15, 0.14), 0.76, 0.42, 0.18, 5.5),
        "machinery": mat("C2_EXPOSED_MACHINERY", (0.024, 0.027, 0.030),
                         (0.13, 0.14, 0.15), 0.70, 0.56, 0.14, 5.8),
        "copper": mat("C2_BURNT_COPPER", (0.030, 0.010, 0.004),
                      (0.30, 0.085, 0.018), 0.84, 0.40, 0.12, 8.0),
        "rust": mat("C2_EDGE_OXIDE", (0.009, 0.004, 0.002),
                    (0.075, 0.028, 0.009), 0.38, 0.76, 0.16, 9.0),
        "circuit": mat("C2_DEAD_CIRCUIT", (0.012, 0.013, 0.013),
                       (0.105, 0.103, 0.095), 0.66, 0.46, 0.10, 6.0),
        "cyan": base.emission_material("C2_CYAN", (0.0, 0.42, 0.63), 4.4),
        "amber": base.emission_material("C2_AMBER", (0.70, 0.19, 0.035), 2.0),
    }
    materials["soot"] = soot_material("C2_MOTTLED_SOOT")
    # The actual selected Unity package textures provide the authored
    # scratches, panel seams, metal response and scale cues absent from the
    # proxy noise materials. Damage interiors remain purpose-built materials.
    materials["skin"] = pbr_panel_material("C2_PBR_SKIN_35", 35,
        (0.40, 0.38, 0.35), 0.42, 0.66, 2.10)
    materials["plate"] = pbr_panel_material("C2_PBR_ARMOR_32", 32,
        (0.38, 0.36, 0.33), 0.48, 0.63, 1.85)
    materials["frame"] = pbr_panel_material("C2_PBR_FRAME_32", 32,
        (0.40, 0.39, 0.36), 0.50, 0.61, 1.55)
    materials["edge"] = pbr_panel_material("C2_PBR_NODE_33", 33,
        (0.39, 0.38, 0.36), 0.46, 0.62, 2.05)
    materials["raw"] = pbr_panel_material("C2_PBR_TORN_48", 48,
        (0.31, 0.31, 0.30), 0.50, 0.61, 1.60)

    root = base.make_empty("QF_CONCEPT_V2_ROOT")
    shell = base.make_empty("SHELL", root)
    panels = base.make_empty("PANELS", root)
    circuits = base.make_empty("CIRCUITS", root)
    nodes = base.make_empty("NODES", root)
    damage = base.make_empty("DAMAGE", root)
    preview = base.make_empty("PREVIEW_CUBE_DEPTH")

    main_cut = [(-3.30, 1.72), (-2.83, 1.84), (-2.43, 1.58),
                (-2.11, 1.12), (-2.24, 0.55), (-1.88, 0.18),
                (-2.02, -0.26), (-1.75, -0.70), (-2.08, -1.12),
                (-2.62, -1.34), (-3.08, -1.06), (-3.18, -0.66),
                (-3.43, -0.24), (-3.23, 0.18), (-3.41, 0.72),
                (-3.18, 1.05)]
    secondary_cut = [(0.02, -1.29), (0.42, -1.14), (0.72, -1.28),
                     (1.08, -1.18), (1.46, -1.45), (1.29, -1.70),
                     (1.51, -2.02), (1.15, -2.39), (0.82, -2.29),
                     (0.42, -2.50), (0.08, -2.18), (0.21, -1.83),
                     (-0.05, -1.61)]
    bottom_cut = [(-1.55, -3.01), (-1.16, -2.76), (-0.76, -2.89),
                  (-0.29, -2.79), (0.13, -3.03), (-0.05, -3.27),
                  (0.17, -3.49), (-0.35, -3.73), (-0.79, -3.58),
                  (-1.22, -3.77), (-1.56, -3.48), (-1.38, -3.24)]

    skin = base.box("C2_DamagedSkin", (0, 0.04, 0),
                    (FACE, 0.30, FACE), materials["skin"], shell,
                    bevel=0.055, segments=3)
    for idx, cut in enumerate((main_cut, secondary_cut, bottom_cut), 1):
        cutter = polygon_prism(f"C2_Cutter_{idx}", cut, 0.04, 1.10, None)
        base.boolean_cut(skin, cutter)
    base.apply_bevel(skin, 0.038, 2)
    base.box("C2_RearBulkhead", (0, 0.68, 0),
             (8.0, 0.18, 8.0), materials["inner"], shell,
             bevel=0.05, segments=3)

    # Three nested structural layers match the heavy silhouette of the
    # reference. Segmented armor prevents a pristine toy-like border.
    base.frame_rect("C2_OuterFrame", 0, 0, 8.48, 8.48, 0.44, 0.38,
                    materials["frame"], shell, y=-0.28, bevel=0.07)
    base.frame_rect("C2_MiddleFrame", 0, 0, 7.88, 7.88, 0.34, 0.22,
                    materials["plate"], shell, y=-0.43, bevel=0.05)
    base.frame_rect("C2_InnerFrame", 0, 0, 7.52, 7.52, 0.22, 0.12,
                    materials["edge"], shell, y=-0.53, bevel=0.035)
    base.corner_braces("C2_Corner", 0, 0, 8.38, 8.38,
                       materials["edge"], shell, y=-0.62, scale=2.15)

    for i, x in enumerate((-3.10, -1.82, -0.42, 1.02, 2.42, 3.45)):
        w = 0.92 if i in (0, 5) else 1.12
        base.box(f"C2_TopBlock_{i}", (x, -0.56, 3.73),
                 (w, 0.20, 0.30), materials["plate"], panels,
                 bevel=0.045, segments=3)
    for i, x in enumerate((-3.05, -1.73, -0.38, 1.02, 2.38, 3.42)):
        base.box(f"C2_BottomBlock_{i}", (x, -0.55, -3.70),
                 (0.98 if i in (0, 5) else 1.14, 0.20, 0.28),
                 materials["plate"], panels, bevel=0.042, segments=3)
    # Segmented side armor breaks the smooth picture-frame silhouette. The
    # left run intentionally has a gap where the primary explosion reached
    # the outer service channel.
    for i, z in enumerate((3.05, 2.08, -2.18, -3.10)):
        base.box(f"C2_LeftFrameBlock_{i}", (-3.74, -0.56, z),
                 (0.30, 0.20, 0.72 if i in (0, 3) else 0.66),
                 materials["plate"], panels, bevel=0.038, segments=3)
    for i, z in enumerate((3.12, 2.08, 0.96, -0.18, -1.34, -2.55, -3.30)):
        base.box(f"C2_RightFrameBlock_{i}", (3.73, -0.56, z),
                 (0.30, 0.20, 0.70 if i not in (2, 4) else 0.82),
                 materials["plate"], panels, bevel=0.038, segments=3)

    # Deliberate macro panels sampled from the concept; not a random greeble
    # field. Larger calm masses alternate with circuit corridors.
    panel_specs = [
        (-0.20, 3.18, 2.15, 0.44, 0.16), (2.20, 3.17, 1.62, 0.46, 0.18),
        (3.28, 2.14, 0.54, 1.55, 0.17), (2.42, 1.92, 1.18, 0.74, 0.18),
        (-0.28, 2.35, 1.18, 0.48, 0.14), (-1.15, 1.52, 0.74, 0.56, 0.15),
        (2.58, 0.70, 1.36, 1.12, 0.18), (3.35, -0.44, 0.50, 1.26, 0.16),
        (2.70, -2.74, 1.34, 0.48, 0.16), (2.88, -3.30, 0.84, 0.34, 0.15),
        (-2.90, -2.25, 0.72, 0.82, 0.15), (-2.32, -3.04, 1.08, 0.46, 0.15),
        (-0.92, -2.18, 0.92, 0.62, 0.17), (-0.15, -1.08, 0.74, 0.88, 0.15),
        (-0.04, 0.96, 0.76, 0.62, 0.14), (0.16, 2.42, 0.56, 0.72, 0.15),
    ]
    for i, (x, z, w, h, d) in enumerate(panel_specs):
        actual_depth = d * 0.76
        base.box(f"C2_MacroPanel_{i:02d}",
                 (x, -0.245 - actual_depth * 0.42, z),
                 (w, actual_depth, h),
                 materials["plate"] if i % 3 else materials["skin"],
                 panels, bevel=0.04, segments=3)

    # Secondary panels remain subordinate and align to the same orthogonal
    # construction language as the reference.
    secondary_specs = [
        (-0.90, 2.86, 0.42, 0.30), (0.82, 2.88, 0.52, 0.24),
        (2.98, 2.91, 0.38, 0.30), (-1.15, 0.82, 0.44, 0.30),
        (1.92, 0.78, 0.46, 0.28), (2.20, -0.42, 0.40, 0.26),
        (-0.35, -2.54, 0.52, 0.26), (1.84, -2.82, 0.45, 0.25),
        (-2.85, -2.92, 0.50, 0.24), (3.08, 1.08, 0.34, 0.62),
    ]
    for i, (x, z, w, h) in enumerate(secondary_specs):
        base.box(f"C2_Secondary_{i:02d}", (x, -0.49, z),
                 (w, 0.16, h), materials["edge"], panels,
                 bevel=0.028, segments=2)

    detail_specs = [
        (-2.96, 2.82, 0.26, 0.16), (-2.10, 2.90, 0.34, 0.15),
        (-1.48, 2.56, 0.30, 0.17), (-0.74, 2.66, 0.24, 0.18),
        (0.42, 2.82, 0.34, 0.15), (1.62, 2.70, 0.26, 0.16),
        (2.42, 2.88, 0.38, 0.15), (2.98, 2.48, 0.22, 0.18),
        (2.66, 1.32, 0.34, 0.16), (2.98, 0.54, 0.22, 0.20),
        (2.58, -0.34, 0.38, 0.15), (2.92, -1.02, 0.24, 0.18),
        (2.52, -2.62, 0.36, 0.16), (1.78, -2.82, 0.30, 0.15),
        (0.78, -2.72, 0.26, 0.17), (-1.76, -2.56, 0.32, 0.16),
        (-2.52, -2.48, 0.38, 0.15), (-2.92, -1.92, 0.22, 0.18),
        (-1.48, 1.88, 0.28, 0.16), (1.88, 1.36, 0.30, 0.16),
    ]
    for i, (x, z, w, h) in enumerate(detail_specs):
        base.box(f"C2_AuthorDetail_{i:02d}", (x, -0.500, z),
                 (w, 0.12, h), materials["edge"] if i % 4 else
                 materials["plate"], panels, bevel=0.024, segments=2)

    # Four dark circuit trunks plus short cyan service indicators.
    circuit_path("C2_Trunk_Left", [(-1.18, 3.18), (-1.18, 1.10),
        (-0.95, 1.10), (-0.95, -0.85), (-1.25, -0.85), (-1.25, -2.55)],
        materials["circuit"], circuits, width=0.040)
    circuit_path("C2_Trunk_Center", [(0.05, 3.18), (0.05, 1.45),
        (0.32, 1.45), (0.32, -0.70), (0.08, -0.70), (0.08, -2.78)],
        materials["circuit"], circuits, width=0.042)
    circuit_path("C2_Trunk_Right", [(2.08, 3.18), (2.08, 2.50),
        (2.43, 2.50), (2.43, 0.08), (2.12, 0.08), (2.12, -2.80)],
        materials["circuit"], circuits, width=0.038)
    circuit_path("C2_LowerBus", [(-3.25, -2.62), (-0.55, -2.62),
        (-0.55, -2.38), (2.85, -2.38)], materials["circuit"], circuits,
        width=0.045)
    for lane, offset in enumerate((-0.08, 0.0, 0.08)):
        base.cylinder_between(f"C2_LowerPipe_{lane}",
            (-2.85, -0.51 - lane * 0.01, -2.48 + offset),
            (2.70, -0.51 - lane * 0.01, -2.48 + offset), 0.026,
            materials["circuit"], circuits, vertices=10)

    base.box("C2_LiveTopSlot", (-0.765, -0.545, 2.62),
             (0.95, 0.09, 0.12), materials["inner"], circuits,
             bevel=0.025, segments=2)
    base.box("C2_LiveMidSlot", (0.51, -0.555, 0.44),
             (0.50, 0.09, 0.12), materials["inner"], circuits,
             bevel=0.025, segments=2)
    base.box("C2_LiveLowSlot", (-0.25, -0.545, -2.38),
             (0.72, 0.09, 0.12), materials["inner"], circuits,
             bevel=0.025, segments=2)
    circuit_path("C2_LiveTop", [(-1.10, 2.62), (-0.43, 2.62)],
                 materials["cyan"], circuits, y=-0.59, width=0.032)
    circuit_path("C2_LiveMid", [(0.36, 0.44), (0.66, 0.44)],
                 materials["cyan"], circuits, y=-0.61, width=0.032)
    circuit_path("C2_LiveLow", [(-0.49, -2.38), (-0.01, -2.38)],
                 materials["cyan"], circuits, y=-0.60, width=0.030)

    # Raised pipe bundles make the machine read as one connected system. They
    # deliberately stop at the blast boundaries instead of continuing under
    # the damaged geometry.
    for lane, offset in enumerate((-0.075, 0.0, 0.075)):
        base.cylinder_between(f"C2_CenterBus_{lane}",
            (-0.18 + offset, -0.53, 3.08),
            (-0.18 + offset, -0.53, -2.70), 0.024,
            materials["circuit"], circuits, vertices=10)
    for lane, offset in enumerate((-0.055, 0.0, 0.055)):
        base.cylinder_between(f"C2_TopLeftFeed_{lane}",
            (-2.30, -0.55, 1.30 + offset),
            (-1.26, -0.55, 1.30 + offset), 0.021,
            materials["circuit"], circuits, vertices=10)
        base.cylinder_between(f"C2_TopRightFeed_{lane}",
            (0.92, -0.55, 1.32 + offset),
            (-0.10, -0.55, 1.32 + offset), 0.021,
            materials["circuit"], circuits, vertices=10)
        base.cylinder_between(f"C2_MidRightFeed_{lane}",
            (0.98, -0.56, -0.30 + offset),
            (0.18, -0.56, -0.30 + offset), 0.021,
            materials["circuit"], circuits, vertices=10)
    # Broken feed ends on both sides of the primary scar.
    for lane, offset in enumerate((-0.05, 0.0, 0.05)):
        base.cylinder_between(f"C2_PrimaryFeedLeft_{lane}",
            (-3.55, -0.56, 0.52 + offset),
            (-3.24, -0.56, 0.52 + offset), 0.022,
            materials["circuit"], circuits, vertices=10)
        base.cylinder_between(f"C2_PrimaryFeedRight_{lane}",
            (-1.88, -0.56, 0.52 + offset),
            (-1.28, -0.56, 0.52 + offset), 0.022,
            materials["circuit"], circuits, vertices=10)

    # Narrow vertical service bundles. Each bundle is physically interrupted
    # where an explosion removed the armor, making the damage affect the
    # machine network rather than sit on top of it like decoration.
    for lane, offset in enumerate((-0.055, 0.0, 0.055)):
        x_left = -3.04 + offset
        base.cylinder_between(f"C2_LeftServiceUpper_{lane}",
            (x_left, -0.55, 3.08), (x_left, -0.55, 1.60), 0.020,
            materials["circuit"], circuits, vertices=10)
        base.cylinder_between(f"C2_LeftServiceLower_{lane}",
            (x_left, -0.55, -1.43), (x_left, -0.55, -2.66), 0.020,
            materials["circuit"], circuits, vertices=10)
        x_right = 1.62 + offset
        base.cylinder_between(f"C2_RightServiceUpper_{lane}",
            (x_right, -0.55, 3.02), (x_right, -0.55, -1.02), 0.020,
            materials["circuit"], circuits, vertices=10)
        base.cylinder_between(f"C2_RightServiceLower_{lane}",
            (x_right, -0.55, -2.47), (x_right, -0.55, -2.78), 0.020,
            materials["circuit"], circuits, vertices=10)
    for i, (x, z) in enumerate(((-3.04, 2.45), (-3.04, -2.05),
                                (1.62, 2.48), (1.62, 0.72))):
        base.box(f"C2_ServiceClamp_{i}", (x, -0.585, z),
                 (0.22, 0.08, 0.10), materials["edge"], circuits,
                 bevel=0.018, segments=2)

    damaged_port_node("C2_Node_TopLeft", -2.48, 2.12, 1.48, 1.44,
                      materials, nodes)
    layered_node("C2_Node_TopRight", 0.92, 2.08, 1.44, 1.38,
                 materials, nodes, "solid")
    layered_node("C2_Node_MidLeft", -0.88, 0.40, 1.28, 1.20,
                 materials, nodes, "solid")
    layered_node("C2_Node_MidRight", 0.98, 0.32, 1.18, 1.08,
                 materials, nodes, "socket")

    build_damage("C2_PrimaryBlast", main_cut, (-2.58, 0.18),
                 materials, damage, primary=True)
    build_damage("C2_SecondaryBlast", secondary_cut, (0.75, -1.80),
                 materials, damage, primary=False)
    build_damage("C2_BottomBlast", bottom_cut, (-0.66, -3.27),
                 materials, damage, primary=False)
    soot_stain("C2_PrimarySoot", main_cut, (-2.58, 0.18),
               materials["soot"], damage, 1.29)
    soot_stain("C2_SecondarySoot", secondary_cut, (0.75, -1.80),
               materials["soot"], damage, 1.25)
    soot_stain("C2_BottomSoot", bottom_cut, (-0.66, -3.27),
               materials["soot"], damage, 1.22)
    rough_damage_ring("C2_PrimaryCrumple", main_cut, (-2.58, 0.18),
                      materials["machinery"], materials["raw"], damage, 0.08)
    rough_damage_ring("C2_SecondaryCrumple", secondary_cut, (0.75, -1.80),
                      materials["machinery"], materials["raw"], damage, 0.075)
    rough_damage_ring("C2_BottomCrumple", bottom_cut, (-0.66, -3.27),
                      materials["machinery"], materials["raw"], damage, 0.06)

    broken_housing("C2_SecondaryBrokenNode", 0.75, -1.80, 1.54, 1.46,
                   materials, damage)
    broken_housing("C2_BottomBrokenNode", -0.66, -3.23, 1.52, 1.18,
                   materials, damage, bottom=True)

    # Visible internals belong to the destroyed housings and sit behind their
    # surviving frame sections. Compact piles at the bottom follow gravity.
    base.cylinder_between("C2_SecondaryActuator",
        (0.34, -0.43, -1.48), (1.05, -0.60, -2.09), 0.105,
        materials["raw"], damage, vertices=14)
    base.cylinder_between("C2_SecondaryActuatorCore",
        (0.40, -0.58, -1.54), (0.92, -0.68, -1.99), 0.045,
        materials["copper"], damage, vertices=10)
    base.cable("C2_SecondaryCableA",
        [(0.18, -0.34, -1.58), (0.47, -0.62, -1.78),
         (0.78, -0.65, -2.12)], 0.022, materials["copper"], damage)
    base.cable("C2_SecondaryCableB",
        [(1.30, -0.31, -1.69), (1.06, -0.60, -1.92),
         (0.76, -0.63, -2.20)], 0.019, materials["circuit"], damage)
    base.cable("C2_SecondaryLiveEnd",
        [(0.58, -0.64, -1.83), (0.48, -0.73, -1.93),
         (0.41, -0.68, -2.04)], 0.012, materials["cyan"], damage)
    for i, x in enumerate((-1.12, -0.78, -0.42)):
        base.cylinder_between(f"C2_BottomInternalRail_{i}",
            (x, -0.34, -3.02), (x + 0.08, -0.58, -3.56), 0.038,
            materials["edge"] if i != 1 else materials["copper"], damage,
            vertices=10)
    # Broad armor deformation stays anchored to the primary crater rim.
    deformation_specs = []
    for i, (shape, y, depth, angle) in enumerate(deformation_specs):
        plate = polygon_prism(f"C2_PrimaryDeformedPlate_{i}", shape, y,
                              depth, materials["skin"] if i % 2 else
                              materials["raw"], damage)
        plate.rotation_euler.x = math.radians(angle)
        base.apply_bevel(plate, 0.020, 2)

    # A few anchored fracture rays; never enough to become visual confetti.
    fracture_paths = [
        [(-3.18, 1.05), (-3.55, 1.30), (-3.72, 1.62)],
        [(-2.02, -0.26), (-1.62, -0.42), (-1.42, -0.72)],
        [(1.32, -2.14), (1.66, -2.42), (1.90, -2.52)],
        [(-1.22, -3.52), (-1.52, -3.72), (-1.78, -3.80)],
    ]
    for i, path in enumerate(fracture_paths):
        surface_crack(f"C2_FractureRay_{i}", path, materials["cavity"],
                      damage, width=0.030)

    # Short secondary cracks branch from the blast without becoming a noisy
    # spiderweb. Their dark recessed material keeps them visible without glow.
    extra_cracks = [
        [(-3.30, 1.48), (-3.46, 1.76), (-3.38, 2.05)],
        [(-1.88, 0.18), (-1.57, 0.23), (-1.39, 0.08)],
        [(-2.62, -1.34), (-2.48, -1.58), (-2.62, -1.82)],
        [(0.20, -1.32), (0.05, -1.10), (0.10, -0.88)],
        [(1.48, -1.72), (1.73, -1.60), (1.91, -1.68)],
        [(-0.12, -3.56), (0.15, -3.70), (0.38, -3.68)],
    ]
    for i, path in enumerate(extra_cracks):
        surface_crack(f"C2_ExtraCrack_{i}", path, materials["cavity"],
                      damage, width=0.026)

    # Hairline material failures echo the reference's aged armor. They stay
    # short and avoid crossing node faces, preserving the composition.
    age_cracks = [
        [(-0.96, 3.05), (-0.82, 2.88), (-0.90, 2.70)],
        [(0.78, 3.14), (0.90, 2.98), (0.86, 2.82)],
        [(2.70, 2.68), (2.53, 2.52), (2.60, 2.34)],
        [(3.12, 1.52), (2.94, 1.38), (3.01, 1.20)],
        [(2.74, 0.32), (2.56, 0.18), (2.62, -0.04)],
        [(2.46, -0.88), (2.28, -1.05), (2.34, -1.22)],
        [(2.68, -2.78), (2.48, -2.92), (2.54, -3.10)],
        [(1.42, -3.12), (1.26, -2.96), (1.14, -3.10)],
        [(-1.72, -2.64), (-1.56, -2.48), (-1.68, -2.32)],
        [(-2.82, -2.16), (-2.66, -2.02), (-2.74, -1.86)],
        [(-1.28, 1.88), (-1.12, 1.72), (-1.20, 1.54)],
        [(1.80, 1.12), (1.64, 0.96), (1.72, 0.78)],
    ]
    for i, path in enumerate(age_cracks):
        surface_crack(f"C2_AgeCrack_{i}", path, materials["cavity"],
                      panels, width=0.012)

    # Sparse fasteners establish scale and localized wear.
    bolt_positions = []
    for x in (-3.55, -2.35, -1.10, 0.15, 1.40, 2.65, 3.55):
        bolt_positions.extend(((x, 3.72), (x, -3.70)))
    for z in (-2.85, -1.70, -0.55, 0.60, 1.75, 2.85):
        bolt_positions.extend(((-3.72, z), (3.72, z)))
    for i, (x, z) in enumerate(bolt_positions):
        front_bolt(f"C2_Bolt_{i:02d}", x, z, 0.032,
                   materials["raw"] if i % 5 == 0 else materials["edge"],
                   shell, y=-0.58)

    # Small recessed pits are clustered on armor edges and lower regions. They
    # supply age/scale while keeping the large rest areas readable.
    pock_positions = [
        (-3.28, 3.08), (-2.78, 3.22), (-1.95, 3.15), (-0.72, 3.05),
        (0.54, 3.24), (1.52, 3.10), (2.68, 3.02), (3.30, 2.62),
        (3.18, 1.72), (2.72, 0.96), (3.25, 0.12), (2.90, -0.72),
        (3.22, -1.52), (2.62, -2.75), (1.78, -3.08), (0.74, -3.18),
        (-0.28, -2.65), (-1.72, -2.92), (-2.66, -2.72), (-3.22, -2.18),
        (-3.10, -1.58), (-1.46, 2.75), (1.86, 2.68), (2.10, -0.62),
    ]
    for i, (x, z) in enumerate(pock_positions):
        front_bolt(f"C2_Pock_{i:02d}", x, z,
                   0.022 + 0.008 * (i % 3), materials["cavity"], panels,
                   y=-0.515)

    service_strips = [
        (-1.72, 2.92, 0.46, 0.11), (-0.72, 2.92, 0.34, 0.10),
        (1.92, 2.90, 0.42, 0.10), (2.88, 2.36, 0.12, 0.48),
        (2.96, 1.48, 0.12, 0.38), (2.76, -0.18, 0.40, 0.10),
        (2.82, -2.82, 0.44, 0.11), (1.72, -2.92, 0.36, 0.10),
        (-1.78, -2.90, 0.42, 0.10), (-2.92, -2.15, 0.11, 0.42),
        (-1.02, 1.98, 0.36, 0.10), (1.78, 0.88, 0.34, 0.10),
    ]
    for i, (x, z, w, h) in enumerate(service_strips):
        base.box(f"C2_ServiceStrip_{i:02d}", (x, -0.535, z),
                 (w, 0.11, h), materials["edge"], panels,
                 bevel=0.020, segments=2)

    oxide_edges = []
    for i, (x, z, w, h) in enumerate(oxide_edges):
        base.box(f"C2_OxideEdge_{i:02d}", (x, -0.588, z),
                 (w, 0.035, h), materials["rust"], panels,
                 bevel=0.012, segments=2)

    for i, z in enumerate((2.42, 1.52, 0.62, -0.32, -1.22, -2.12)):
        base.box(f"C2_BusClamp_{i}", (-0.18, -0.585, z),
                 (0.34, 0.12, 0.13), materials["edge"], circuits,
                 bevel=0.022, segments=2)

    organize_unity_states(root, damage, nodes, materials,
        ((main_cut, (-2.58, 0.18)),
         (secondary_cut, (0.75, -1.80)),
         (bottom_cut, (-0.66, -3.27))))

    # Preview-only cube depth. This is deliberately substantial so the fixed
    # right-facing pose has the same mass as the concept.
    base.box("C2_RightShell", (4.18, 1.55, 0), (0.34, 3.42, 8.26),
             materials["skin"], preview, bevel=0.06, segments=3)
    base.box("C2_TopShell", (0, 1.55, 4.18), (8.26, 3.42, 0.34),
             materials["skin"], preview, bevel=0.06, segments=3)
    for i, z in enumerate((-3.20, -2.05, -0.90, 0.25, 1.40, 2.55, 3.45)):
        base.box(f"C2_SideRib_{i}", (4.38, 1.60, z),
                 (0.16, 2.82, 0.20), materials["frame"], preview,
                 bevel=0.03, segments=2)
    for i, y in enumerate((0.18, 0.72, 1.28, 1.84, 2.40, 2.96)):
        base.box(f"C2_SidePanel_{i}", (4.39, y, 1.10 - i * 0.36),
                 (0.14, 0.42, 0.92), materials["plate"], preview,
                 bevel=0.03, segments=2)
        base.box(f"C2_SideLight_{i}", (4.48, y, 1.10 - i * 0.36),
                 (0.025, 0.10, 0.035), materials["cyan"], preview,
                 bevel=0.008, segments=2)
    for i, y in enumerate((0.44, 1.02, 1.60, 2.18, 2.74)):
        base.box(f"C2_SideArmorInset_{i}", (4.40, y, -2.55 + i * 1.02),
                 (0.15, 0.36, 0.72), materials["edge"], preview,
                 bevel=0.028, segments=2)
    return root


def setup_render():
    scene = bpy.context.scene
    scene.render.engine = "BLENDER_EEVEE"
    scene.render.resolution_x = 1280
    scene.render.resolution_y = 1280
    scene.render.resolution_percentage = 100
    scene.render.image_settings.file_format = "PNG"
    scene.render.film_transparent = False
    scene.world.color = (0.0, 0.0, 0.0)
    scene.world.use_nodes = True
    world_background = scene.world.node_tree.nodes.get("Background")
    world_background.inputs["Color"].default_value = (0.0, 0.0, 0.0, 1.0)
    world_background.inputs["Strength"].default_value = 0.015
    scene.view_settings.look = "AgX - Medium High Contrast"
    scene.view_settings.exposure = 1.12

    camera_data = bpy.data.cameras.new("ConceptCamera")
    camera = bpy.data.objects.new("ConceptCamera", camera_data)
    bpy.context.scene.collection.objects.link(camera)
    # About +14 degrees around the cube and a modest elevation, matching the
    # approved concept: readable right side, only a thin strip of roof.
    camera.location = (4.25, -17.5, 1.05)
    base.look_at(camera, (0.05, 0.34, -0.05))
    camera.data.type = "PERSP"
    camera.data.lens = 66.0
    scene.camera = camera

    def area(name, location, energy, color, size, target):
        data = bpy.data.lights.new(name, "AREA")
        data.energy = energy
        data.color = color
        data.shape = "DISK"
        data.size = size
        obj = bpy.data.objects.new(name, data)
        bpy.context.scene.collection.objects.link(obj)
        obj.location = location
        base.look_at(obj, target)
        return obj

    area("C2_Key_Raking", (-5.8, -7.8, 9.5), 1550,
         (0.86, 0.82, 0.76), 4.0, (-0.7, 0.0, 0.4))
    area("C2_Fill_Cool", (6.5, -7.0, 3.0), 520,
         (0.23, 0.40, 0.58), 5.0, (0.8, 0.2, -0.2))
    area("C2_Front_Soft", (0.5, -9.5, 2.2), 360,
         (0.62, 0.66, 0.70), 6.5, (0.0, 0.0, 0.0))
    area("C2_Rim", (7.0, 3.8, 7.5), 900,
         (0.25, 0.55, 0.78), 3.1, (2.3, 1.1, 0.4))
    area("C2_DamageWarm", (-3.8, -3.0, 0.2), 18,
         (0.72, 0.20, 0.055), 1.6, (-2.5, 0.0, -0.2))
    area("C2_DamageRevealLeft", (-3.0, -4.2, 1.1), 78,
         (0.52, 0.58, 0.64), 2.2, (-2.55, -0.2, 0.05))
    area("C2_DamageRevealLow", (0.0, -4.0, -2.1), 62,
         (0.46, 0.52, 0.58), 2.4, (0.15, -0.2, -2.45))


def save_and_render():
    os.makedirs(os.path.dirname(FBX_PATH), exist_ok=True)
    bpy.ops.wm.save_as_mainfile(filepath=BLEND_PATH)
    bpy.context.scene.render.filepath = PREVIEW_PATH
    bpy.ops.render.render(write_still=True)
    bpy.ops.object.select_all(action="DESELECT")
    export_root = bpy.data.objects.get("QF_CONCEPT_V2_ROOT")
    if export_root is None:
        raise RuntimeError("Falta QF_CONCEPT_V2_ROOT para exportar Unity.")
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
    meshes = [obj for obj in bpy.context.scene.objects if obj.type == "MESH"]
    triangles = 0
    for obj in meshes:
        obj.data.calc_loop_triangles()
        triangles += len(obj.data.loop_triangles)
    print(f"[QF Concept V2] meshes={len(meshes)} triangles={triangles}")
    print(BLEND_PATH)
    print(PREVIEW_PATH)
    print(FBX_PATH)


if __name__ == "__main__":
    build_scene()
    setup_render()
    save_and_render()
    print("[QF Concept V2] PASS")

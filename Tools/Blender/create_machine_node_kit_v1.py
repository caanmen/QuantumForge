import math
import os

import bpy
from mathutils import Vector


ROOT_DIR = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))
BLEND_PATH = os.path.join(
    ROOT_DIR, "ArtSource", "Blender", "MachineCube", "QF_MachineNodeKitV1.blend"
)
FBX_PATH = os.path.join(
    ROOT_DIR,
    "Assets",
    "Project",
    "Art",
    "MachineCubeBlender",
    "QF_MachineNodeKitV1.fbx",
)


def clear_scene():
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete(use_global=False)
    for datablocks in (bpy.data.meshes, bpy.data.curves, bpy.data.materials):
        for datablock in list(datablocks):
            if datablock.users == 0:
                datablocks.remove(datablock)


def material(name, color, metallic=0.0, roughness=0.55, emission=None):
    mat = bpy.data.materials.new(name)
    mat.diffuse_color = (*color, 1.0)
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    bsdf.inputs["Base Color"].default_value = (*color, 1.0)
    bsdf.inputs["Metallic"].default_value = metallic
    bsdf.inputs["Roughness"].default_value = roughness
    if emission is not None:
        bsdf.inputs["Emission Color"].default_value = (*emission, 1.0)
        bsdf.inputs["Emission Strength"].default_value = 3.0
    return mat


def attach(obj, parent, loc, rotation=(0.0, 0.0, 0.0), mat=None):
    obj.parent = parent
    obj.location = loc
    obj.rotation_euler = rotation
    if mat is not None:
        obj.data.materials.append(mat)
    return obj


def beveled_box(name, parent, loc, dims, mat, rotation=(0.0, 0.0, 0.0), bevel=0.045):
    bpy.ops.mesh.primitive_cube_add(size=1.0)
    obj = bpy.context.object
    obj.name = name
    obj.dimensions = dims
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    attach(obj, parent, loc, rotation, mat)
    if bevel > 0.0:
        modifier = obj.modifiers.new("SimpleBevel", "BEVEL")
        modifier.width = min(bevel, min(dims) * 0.28)
        modifier.segments = 1
        bpy.context.view_layer.objects.active = obj
        bpy.ops.object.modifier_apply(modifier=modifier.name)
    return obj


def cylinder(name, parent, loc, radius, depth, mat, rotation=(math.pi / 2.0, 0.0, 0.0), vertices=12):
    bpy.ops.mesh.primitive_cylinder_add(vertices=vertices, radius=radius, depth=depth)
    obj = bpy.context.object
    obj.name = name
    return attach(obj, parent, loc, rotation, mat)


def torus(name, parent, loc, major_radius, minor_radius, mat):
    bpy.ops.mesh.primitive_torus_add(
        major_radius=major_radius,
        minor_radius=minor_radius,
        major_segments=12,
        minor_segments=4,
        location=(0.0, 0.0, 0.0),
        rotation=(math.pi / 2.0, 0.0, 0.0),
    )
    obj = bpy.context.object
    obj.name = name
    return attach(obj, parent, loc, (math.pi / 2.0, 0.0, 0.0), mat)


def polygon_plate(name, parent, loc, points, depth, mat, rotation=(0.0, 0.0, 0.0)):
    half = depth * 0.5
    vertices = [(x, -half, z) for x, z in points] + [(x, half, z) for x, z in points]
    count = len(points)
    faces = [tuple(range(count)), tuple(range(count, count * 2))[::-1]]
    for index in range(count):
        nxt = (index + 1) % count
        faces.append((index, nxt, count + nxt, count + index))
    mesh = bpy.data.meshes.new(name + "Mesh")
    mesh.from_pydata(vertices, [], faces)
    mesh.update()
    obj = bpy.data.objects.new(name, mesh)
    bpy.context.collection.objects.link(obj)
    return attach(obj, parent, loc, rotation, mat)


def cable_segment(name, parent, start, end, radius, mat):
    start_v = Vector(start)
    end_v = Vector(end)
    direction = end_v - start_v
    length = direction.length
    midpoint = (start_v + end_v) * 0.5
    bpy.ops.mesh.primitive_cylinder_add(vertices=6, radius=radius, depth=length)
    obj = bpy.context.object
    obj.name = name
    obj.parent = parent
    obj.location = midpoint
    obj.rotation_mode = "QUATERNION"
    obj.rotation_quaternion = direction.to_track_quat("Z", "Y")
    obj.data.materials.append(mat)
    return obj


def cable(name, parent, points, mat, copper):
    for index in range(len(points) - 1):
        cable_segment(f"{name}_SEG_{index:02d}", parent, points[index], points[index + 1], 0.045, mat)
    direction = Vector(points[-1]) - Vector(points[-2])
    tip_end = Vector(points[-1]) + direction.normalized() * 0.065
    cable_segment(f"{name}_COPPER_TIP", parent, points[-1], tip_end, 0.032, copper)


def empty(name, parent=None, loc=(0.0, 0.0, 0.0), rotation=(0.0, 0.0, 0.0)):
    obj = bpy.data.objects.new(name, None)
    bpy.context.collection.objects.link(obj)
    obj.parent = parent
    obj.location = loc
    obj.rotation_euler = rotation
    return obj


def frame(root, metal, dark, damaged):
    beveled_box("Cavity", root, (0.0, 0.08, 0.0), (1.48, 0.20, 1.20), dark, bevel=0.035)
    beveled_box("FrameTop", root, (0.0, -0.02, 0.70), (1.78, 0.34, 0.22), metal)
    beveled_box("FrameBottom", root, (0.0, -0.02, -0.70), (1.78, 0.34, 0.22), metal)
    beveled_box("FrameLeft", root, (-0.78, -0.02, 0.0), (0.22, 0.34, 1.20), metal)
    if not damaged:
        beveled_box("FrameRight", root, (0.78, -0.02, 0.0), (0.22, 0.34, 1.20), metal)
    else:
        beveled_box(
            "FrameRightLoose",
            root,
            (0.66, -0.28, -0.28),
            (0.22, 0.34, 0.72),
            metal,
            rotation=(0.0, math.radians(-14.0), math.radians(-10.0)),
        )


def add_status(root, cyan, warning, damaged):
    beveled_box("StatusBar", root, (0.0, -0.235, -0.70), (0.38, 0.055, 0.075), cyan, bevel=0.018)
    if damaged:
        cylinder("WarningLED", root, (-0.60, -0.235, -0.69), 0.055, 0.055, warning, vertices=8)


def insert_group(root, kind, metal, dark, cyan, detached=False):
    group = empty(
        "DetachedInsert" if detached else "FunctionalInsert",
        root,
        (0.43, -0.31, -0.18) if detached else (0.0, -0.22, 0.03),
        (0.0, math.radians(-9.0), math.radians(-12.0)) if detached else (0.0, 0.0, 0.0),
    )
    if kind == 0:
        beveled_box("RingPlate", group, (0.0, 0.0, 0.0), (1.20, 0.13, 0.98), metal)
        torus("Ring", group, (0.0, -0.10, 0.05), 0.33, 0.055, cyan)
    elif kind == 1:
        beveled_box("SensorPlate", group, (0.0, 0.0, 0.0), (1.20, 0.13, 0.98), metal)
        for x in (-0.25, 0.25):
            for z in (-0.20, 0.24):
                cylinder("SensorDot", group, (x, -0.105, z), 0.105, 0.07, cyan, vertices=10)
    elif kind == 2:
        beveled_box("VentPlate", group, (0.0, 0.0, 0.0), (1.24, 0.13, 0.98), metal)
        for x in (-0.36, -0.18, 0.0, 0.18, 0.36):
            beveled_box("VentSlot", group, (x, -0.095, 0.04), (0.095, 0.06, 0.62), dark, bevel=0.025)
    elif kind == 3:
        points = [(-0.52, -0.38), (0.52, -0.38), (0.0, 0.50)]
        polygon_plate("RelayTriangle", group, (0.0, 0.0, 0.0), points, 0.15, metal)
        cylinder("RelayCore", group, (0.0, -0.115, 0.0), 0.14, 0.07, cyan, vertices=10)
        for x, z in ((-0.32, -0.25), (0.32, -0.25), (0.0, 0.28)):
            cylinder("RelayPad", group, (x, -0.11, z), 0.065, 0.06, dark, vertices=8)
    elif kind == 4:
        beveled_box(
            "DiamondPlate",
            group,
            (0.0, 0.0, 0.0),
            (0.78, 0.15, 0.78),
            metal,
            rotation=(0.0, math.radians(45.0), 0.0),
        )
        beveled_box(
            "DiamondCore",
            group,
            (0.0, -0.115, 0.0),
            (0.25, 0.07, 0.25),
            cyan,
            rotation=(0.0, math.radians(45.0), 0.0),
            bevel=0.025,
        )
    else:
        beveled_box("HorizontalPlate", group, (0.0, 0.0, 0.0), (1.30, 0.13, 0.76), metal)
        beveled_box("HorizontalSlot", group, (0.0, -0.10, 0.02), (0.80, 0.06, 0.20), dark, bevel=0.08)
        beveled_box("HorizontalCore", group, (0.0, -0.145, 0.02), (0.42, 0.025, 0.06), cyan, bevel=0.018)
    return group


def make_node(kind, damaged, location, mats):
    state = "DAMAGED" if damaged else "INTACT"
    names = ("RING", "SENSOR", "VENT", "RELAY", "DIAMOND", "HORIZONTAL")
    root = empty(f"NODE_{kind + 1:02d}_{names[kind]}_{state}", loc=location)
    frame(root, mats["metal"], mats["dark"], damaged)
    add_status(root, mats["cyan"], mats["warning"], damaged)
    insert_group(root, kind, mats["metal"], mats["dark"], mats["cyan"], detached=damaged)
    if damaged:
        cable(
            "CableA",
            root,
            [(-0.18, -0.18, -0.12), (-0.22, -0.34, -0.38), (-0.32, -0.42, -0.82)],
            mats["cable"],
            mats["copper"],
        )
        cable(
            "CableB",
            root,
            [(0.12, -0.18, -0.10), (0.18, -0.36, -0.44), (0.10, -0.43, -0.91)],
            mats["cable"],
            mats["copper"],
        )
    root["qf_node_type"] = names[kind].lower()
    root["qf_state"] = state.lower()
    return root


def triangulated_face_count(root):
    count = 0
    for obj in root.children_recursive:
        if obj.type != "MESH":
            continue
        obj.data.calc_loop_triangles()
        count += len(obj.data.loop_triangles)
    return count


def main():
    clear_scene()
    mats = {
        "metal": material("QF_NODE_METAL", (0.23, 0.25, 0.27), 0.72, 0.34),
        "dark": material("QF_NODE_DARK", (0.035, 0.045, 0.052), 0.15, 0.62),
        "cyan": material("QF_NODE_CYAN", (0.0, 0.48, 0.62), 0.25, 0.28, (0.0, 0.72, 0.92)),
        "warning": material("QF_NODE_WARNING", (0.35, 0.01, 0.01), 0.15, 0.35, (1.0, 0.015, 0.005)),
        "cable": material("QF_NODE_CABLE", (0.025, 0.028, 0.032), 0.05, 0.70),
        "copper": material("QF_NODE_COPPER", (0.55, 0.18, 0.045), 0.75, 0.30),
    }

    spacing = 2.25
    roots = []
    for index in range(6):
        x = (index - 2.5) * spacing
        roots.append(make_node(index, False, (x, 0.0, 1.25), mats))
        roots.append(make_node(index, True, (x, 0.0, -1.25), mats))

    for root in roots:
        print(f"QF_NODE_STATS {root.name}: {triangulated_face_count(root)} triangles")

    os.makedirs(os.path.dirname(BLEND_PATH), exist_ok=True)
    os.makedirs(os.path.dirname(FBX_PATH), exist_ok=True)
    bpy.ops.wm.save_as_mainfile(filepath=BLEND_PATH)
    bpy.ops.export_scene.fbx(
        filepath=FBX_PATH,
        use_selection=False,
        apply_unit_scale=True,
        apply_scale_options="FBX_SCALE_ALL",
        axis_forward="-Z",
        axis_up="Y",
        use_mesh_modifiers=True,
        mesh_smooth_type="FACE",
        add_leaf_bones=False,
        bake_anim=False,
        path_mode="AUTO",
        embed_textures=False,
    )
    print("QF_NODE_KIT_BLEND", BLEND_PATH)
    print("QF_NODE_KIT_FBX", FBX_PATH)


if __name__ == "__main__":
    main()

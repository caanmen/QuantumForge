import bpy
import math
import os


SOURCE_DIR = os.path.dirname(bpy.data.filepath)
OUTPUT_BLEND = os.path.join(SOURCE_DIR, "QF_MachineNodeKitV2.blend")


def material(name):
    value = bpy.data.materials.get(name)
    if value is None:
        raise RuntimeError(f"Material not found: {name}")
    return value


MAT_METAL = material("QF_NODE_METAL")
MAT_DARK = material("QF_NODE_DARK")
MAT_CABLE = material("QF_NODE_CABLE")
MAT_COPPER = material("QF_NODE_COPPER")
MAT_WARNING = material("QF_NODE_WARNING")


def link_mesh_object(name, mesh, parent, mat):
    obj = bpy.data.objects.new(name, mesh)
    bpy.context.scene.collection.objects.link(obj)
    obj.parent = parent
    obj.location = (0.0, 0.0, 0.0)
    obj.data.materials.append(mat)
    return obj


def apply_bevel(obj, width, segments=2):
    modifier = obj.modifiers.new(name="EdgeBevel", type="BEVEL")
    modifier.width = width
    modifier.segments = segments
    bpy.context.view_layer.objects.active = obj
    obj.select_set(True)
    bpy.ops.object.modifier_apply(modifier=modifier.name)
    obj.select_set(False)


def chamfered_loop(hx, hz, chamfer):
    return [
        (-hx + chamfer, hz),
        (hx - chamfer, hz),
        (hx, hz - chamfer),
        (hx, -hz + chamfer),
        (hx - chamfer, -hz),
        (-hx + chamfer, -hz),
        (-hx, -hz + chamfer),
        (-hx, hz - chamfer),
    ]


def create_ring(
    name,
    parent,
    outer_x,
    outer_z,
    inner_x,
    inner_z,
    front_y,
    back_y,
    outer_chamfer,
    inner_chamfer,
    mat,
    bevel,
):
    outer = chamfered_loop(outer_x, outer_z, outer_chamfer)
    inner = chamfered_loop(inner_x, inner_z, inner_chamfer)
    vertices = []
    for y, loop in ((front_y, outer), (front_y, inner), (back_y, outer), (back_y, inner)):
        vertices.extend((x, y, z) for x, z in loop)

    faces = []
    for i in range(8):
        j = (i + 1) % 8
        faces.append((i, j, 8 + j, 8 + i))
        faces.append((16 + i, 24 + i, 24 + j, 16 + j))
        faces.append((i, 16 + i, 16 + j, j))
        faces.append((8 + i, 8 + j, 24 + j, 24 + i))

    mesh = bpy.data.meshes.new(f"{name}_Mesh")
    mesh.from_pydata(vertices, [], faces)
    mesh.update()
    obj = link_mesh_object(name, mesh, parent, mat)
    apply_bevel(obj, bevel, 3)
    return obj


def create_box(name, parent, location, dimensions, mat, bevel=0.0):
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0.0, 0.0, 0.0))
    obj = bpy.context.object
    obj.name = name
    obj.parent = parent
    obj.location = location
    obj.dimensions = dimensions
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    obj.data.materials.append(mat)
    if bevel > 0.0:
        apply_bevel(obj, bevel, 2)
    return obj


def create_cylinder(
    name,
    parent,
    location,
    radius,
    depth,
    mat,
    rotation=(math.radians(90.0), 0.0, 0.0),
    vertices=24,
):
    bpy.ops.mesh.primitive_cylinder_add(
        vertices=vertices,
        radius=radius,
        depth=depth,
        location=(0.0, 0.0, 0.0),
        rotation=rotation,
    )
    obj = bpy.context.object
    obj.name = name
    obj.parent = parent
    obj.location = location
    obj.data.materials.append(mat)
    apply_bevel(obj, min(radius * 0.18, 0.012), 2)
    return obj


def create_curve(name, parent, points, radius, mat):
    curve = bpy.data.curves.new(name=f"{name}_Curve", type="CURVE")
    curve.dimensions = "3D"
    curve.resolution_u = 16
    curve.bevel_depth = radius
    curve.bevel_resolution = 4
    spline = curve.splines.new("BEZIER")
    spline.bezier_points.add(len(points) - 1)
    for point, coordinates in zip(spline.bezier_points, points):
        point.co = coordinates
        point.handle_left_type = "AUTO"
        point.handle_right_type = "AUTO"
    obj = bpy.data.objects.new(name, curve)
    bpy.context.scene.collection.objects.link(obj)
    obj.parent = parent
    obj.location = (0.0, 0.0, 0.0)
    obj.data.materials.append(mat)
    return obj


def create_internal_light(name, parent, location):
    light_data = bpy.data.lights.new(name=f"{name}_Data", type="POINT")
    light_data.color = (1.0, 0.015, 0.005)
    light_data.energy = 24.0
    light_data.shadow_soft_size = 0.22
    light = bpy.data.objects.new(name, light_data)
    bpy.context.scene.collection.objects.link(light)
    light.parent = parent
    light.location = location
    return light


def direct_children_matching(root, prefixes):
    return [child for child in list(root.children) if child.name.startswith(prefixes)]


def remove_old_parts(root):
    prefixes = (
        "FrameTop",
        "FrameBottom",
        "FrameLeft",
        "FrameRight",
        "Cavity",
        "CableA_",
        "CableB_",
        "WarningLED",
    )
    for obj in direct_children_matching(root, prefixes):
        bpy.data.objects.remove(obj, do_unlink=True)


def add_integrated_housing(root):
    create_ring(
        f"{root.name}_FrameIntegrated",
        root,
        outer_x=0.89,
        outer_z=0.81,
        inner_x=0.665,
        inner_z=0.585,
        front_y=-0.19,
        back_y=0.15,
        outer_chamfer=0.10,
        inner_chamfer=0.055,
        mat=MAT_METAL,
        bevel=0.018,
    )
    create_ring(
        f"{root.name}_InnerLip",
        root,
        outer_x=0.645,
        outer_z=0.565,
        inner_x=0.585,
        inner_z=0.505,
        front_y=0.005,
        back_y=0.105,
        outer_chamfer=0.045,
        inner_chamfer=0.035,
        mat=MAT_DARK,
        bevel=0.010,
    )
    create_box(
        f"{root.name}_CavityBack",
        root,
        (0.0, 0.145, 0.0),
        (1.16, 0.08, 1.0),
        MAT_DARK,
        bevel=0.035,
    )
    for index, (x, z) in enumerate(
        ((-0.50, 0.40), (0.50, 0.40), (-0.50, -0.40), (0.50, -0.40)),
        start=1,
    ):
        create_cylinder(
            f"{root.name}_CavityBolt_{index}",
            root,
            (x, 0.075, z),
            0.038,
            0.045,
            MAT_METAL,
            vertices=16,
        )


def add_damaged_internals(root):
    socket_locations = ((-0.48, 0.035, -0.20), (0.26, 0.035, -0.28))
    for index, location in enumerate(socket_locations, start=1):
        create_cylinder(
            f"{root.name}_CableSocket_{index}",
            root,
            location,
            0.072,
            0.13,
            MAT_CABLE,
            vertices=28,
        )
        create_cylinder(
            f"{root.name}_CableSocketInner_{index}",
            root,
            (location[0], -0.038, location[2]),
            0.039,
            0.035,
            MAT_DARK,
            vertices=24,
        )

    cable_a = [
        (-0.48, -0.045, -0.20),
        (-0.50, -0.025, -0.38),
        (-0.52, -0.04, -0.58),
        (-0.55, -0.16, -0.77),
        (-0.58, -0.28, -0.94),
    ]
    cable_b = [
        (0.26, -0.045, -0.28),
        (0.28, -0.025, -0.46),
        (0.26, -0.04, -0.62),
        (0.23, -0.16, -0.80),
        (0.20, -0.28, -1.02),
    ]
    create_curve(f"{root.name}_CableA", root, cable_a, 0.034, MAT_CABLE)
    create_curve(f"{root.name}_CableB", root, cable_b, 0.034, MAT_CABLE)
    create_cylinder(
        f"{root.name}_CableA_CopperTip",
        root,
        (-0.58, -0.28, -0.985),
        0.037,
        0.09,
        MAT_COPPER,
        rotation=(0.0, 0.0, 0.0),
        vertices=20,
    )
    create_cylinder(
        f"{root.name}_CableB_CopperTip",
        root,
        (0.20, -0.28, -1.065),
        0.037,
        0.09,
        MAT_COPPER,
        rotation=(0.0, 0.0, 0.0),
        vertices=20,
    )

    led_location = (-0.50, -0.005, 0.30)
    create_cylinder(
        f"{root.name}_InternalWarningLED",
        root,
        led_location,
        0.065,
        0.055,
        MAT_WARNING,
        vertices=24,
    )
    create_internal_light(
        f"{root.name}_InternalRedGlow",
        root,
        (-0.50, -0.055, 0.30),
    )


def center_relay_cores():
    for name in ("RelayCore", "RelayCore.001"):
        core = bpy.data.objects.get(name)
        if core is not None:
            core.location.z = -0.087


def build_v2():
    roots = sorted(
        [obj for obj in bpy.data.objects if obj.name.startswith("NODE_")],
        key=lambda obj: obj.name,
    )
    if len(roots) != 12:
        raise RuntimeError(f"Expected 12 node roots, found {len(roots)}")

    for root in roots:
        remove_old_parts(root)
        add_integrated_housing(root)
        if root.name.endswith("_DAMAGED"):
            add_damaged_internals(root)

    center_relay_cores()
    bpy.context.scene["QF_NodeKitVersion"] = "V2"
    bpy.context.scene["QF_NodeKitChanges"] = (
        "Integrated frames, recessed cavity details, rear-routed cables, "
        "centered relay cores, internal red warning lights"
    )
    bpy.ops.wm.save_as_mainfile(filepath=OUTPUT_BLEND)
    print(f"Saved corrected Blender file: {OUTPUT_BLEND}")


if __name__ == "__main__":
    build_v2()

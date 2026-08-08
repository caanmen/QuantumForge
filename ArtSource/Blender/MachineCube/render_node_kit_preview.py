import bpy
import math
import os


OUTPUT_DIR = os.path.join(
    os.path.dirname(bpy.data.filepath),
    "Renders",
)
FILE_STEM = os.path.splitext(os.path.basename(bpy.data.filepath))[0]


def look_at(obj, target=(0.0, 0.0, 0.0)):
    direction = mathutils.Vector(target) - obj.location
    obj.rotation_euler = direction.to_track_quat("-Z", "Y").to_euler()


def add_area_light(name, location, energy, size, color, target):
    light_data = bpy.data.lights.new(name=name, type="AREA")
    light_data.energy = energy
    light_data.shape = "DISK"
    light_data.size = size
    light_data.color = color
    light = bpy.data.objects.new(name, light_data)
    bpy.context.scene.collection.objects.link(light)
    light.location = location
    look_at(light, target)
    return light


def add_backdrop():
    bpy.ops.mesh.primitive_cube_add(location=(0.0, 0.36, 0.0))
    backdrop = bpy.context.object
    backdrop.name = "RENDER_Backdrop"
    backdrop.scale = (8.2, 0.08, 4.2)
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)

    material = bpy.data.materials.new("RENDER_BackdropMaterial")
    material.diffuse_color = (0.008, 0.012, 0.018, 1.0)
    material.use_nodes = True
    principled = material.node_tree.nodes.get("Principled BSDF")
    principled.inputs["Base Color"].default_value = (0.008, 0.012, 0.018, 1.0)
    principled.inputs["Roughness"].default_value = 0.86
    principled.inputs["Metallic"].default_value = 0.15
    backdrop.data.materials.append(material)


def configure_scene():
    scene = bpy.context.scene
    scene.render.engine = "BLENDER_EEVEE"
    scene.render.image_settings.file_format = "PNG"
    scene.render.image_settings.color_mode = "RGBA"
    scene.render.film_transparent = False
    scene.render.resolution_percentage = 100
    scene.render.image_settings.color_depth = "8"
    scene.render.resolution_x = 2560
    scene.render.resolution_y = 1280

    scene.view_settings.look = "AgX - Medium High Contrast"
    scene.view_settings.exposure = -0.7

    world = scene.world or bpy.data.worlds.new("World")
    scene.world = world
    world.use_nodes = True
    background = world.node_tree.nodes.get("Background")
    background.inputs["Color"].default_value = (0.003, 0.006, 0.012, 1.0)
    background.inputs["Strength"].default_value = 0.16

    add_backdrop()

    camera_data = bpy.data.cameras.new("RENDER_Camera")
    camera = bpy.data.objects.new("RENDER_Camera", camera_data)
    scene.collection.objects.link(camera)
    scene.camera = camera
    camera.data.type = "ORTHO"
    camera.data.lens = 52
    camera.location = (0.0, -18.0, 0.0)
    camera.rotation_euler = (math.radians(90.0), 0.0, 0.0)

    add_area_light(
        "RENDER_Key",
        (-4.8, -6.5, 5.8),
        1150.0,
        5.0,
        (0.60, 0.78, 1.0),
        (-1.5, 0.0, 0.4),
    )
    add_area_light(
        "RENDER_Fill",
        (5.5, -5.0, 2.0),
        800.0,
        4.0,
        (0.24, 0.48, 0.80),
        (2.0, 0.0, -0.2),
    )
    add_area_light(
        "RENDER_Rim",
        (0.0, 1.8, 5.5),
        950.0,
        3.2,
        (0.12, 0.75, 1.0),
        (0.0, 0.0, 0.0),
    )
    add_area_light(
        "RENDER_DamageFill",
        (0.0, -4.0, -4.8),
        500.0,
        4.5,
        (0.38, 0.16, 0.12),
        (0.0, 0.0, -1.3),
    )

    scene.render.use_file_extension = True
    os.makedirs(OUTPUT_DIR, exist_ok=True)
    return scene, camera


def render(scene, camera, filename, ortho_scale, camera_z):
    camera.location.z = camera_z
    camera.data.ortho_scale = ortho_scale
    scene.render.filepath = os.path.join(OUTPUT_DIR, filename)
    bpy.ops.render.render(write_still=True)


def hide_intact_nodes_for_damage_render():
    def hide_tree(obj):
        obj.hide_render = True
        for child in obj.children:
            hide_tree(child)

    for obj in bpy.data.objects:
        if obj.name.startswith("NODE_") and obj.name.endswith("_INTACT"):
            hide_tree(obj)


if __name__ == "__main__":
    import mathutils

    scene, camera = configure_scene()
    render(scene, camera, f"{FILE_STEM}_Blender_Full.png", 14.4, 0.0)

    hide_intact_nodes_for_damage_render()
    scene.render.resolution_x = 2560
    scene.render.resolution_y = 720
    render(scene, camera, f"{FILE_STEM}_Blender_Damaged.png", 14.4, -1.55)

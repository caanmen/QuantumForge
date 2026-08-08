import bpy
import os


OUTPUT_DIR = os.path.join(os.path.dirname(bpy.data.filepath), "Renders")
os.makedirs(OUTPUT_DIR, exist_ok=True)


def face_number():
    for number in (1, 2, 3, 4):
        if f"QF_Face{number}_NodeKit" in bpy.context.scene:
            return number
    raise RuntimeError("Face number metadata missing")


def set_visible(root, visible):
    root.hide_render = not visible
    for child in root.children_recursive:
        child.hide_render = not visible


def render(repaired):
    anchors_root = bpy.data.objects.get("NODE_ANCHORS")
    anchors = sorted(anchors_root.children, key=lambda obj: obj.name)
    public_count = {1: 7, 2: 11, 3: 7, 4: 10}[face_number()]
    for slot, anchor in enumerate(anchors):
        damaged = next(c for c in anchor.children if c.name.startswith("DAMAGED_STATE_"))
        operational = next(c for c in anchor.children if c.name.startswith("REPAIRED_STATE_"))
        selection = next((c for c in anchor.children if c.name.startswith(
            ("SELECTION_FEEDBACK_", "SelectionFeedback"))), None)
        visible = repaired or slot < public_count
        anchor.hide_render = not visible
        set_visible(damaged, visible and not repaired)
        set_visible(operational, visible and repaired)
        if selection is not None:
            set_visible(selection, False)
    state = "Operational" if repaired else "Damaged"
    bpy.context.scene.render.filepath = os.path.join(
        OUTPUT_DIR, f"QF_MachineCube_Face{face_number()}_NodeKitV2_{state}.png")
    bpy.ops.render.render(write_still=True)


scene = bpy.context.scene
scene.render.engine = "BLENDER_EEVEE"
scene.render.resolution_x = 1280
scene.render.resolution_y = 1280
scene.render.resolution_percentage = 100
scene.render.image_settings.file_format = "PNG"
scene.render.image_settings.color_mode = "RGBA"
scene.render.film_transparent = False
render(True)
render(False)

import bpy
import os


OUTPUT_DIR = os.path.join(os.path.dirname(bpy.data.filepath), "Renders")
os.makedirs(OUTPUT_DIR, exist_ok=True)


def set_tree_visible(root, visible):
    root.hide_render = not visible
    for child in root.children_recursive:
        child.hide_render = not visible


def states():
    anchors = bpy.data.objects.get("NODE_ANCHORS")
    if anchors is None:
        raise RuntimeError("NODE_ANCHORS missing")
    for slot, anchor in enumerate(sorted(anchors.children, key=lambda obj: obj.name)):
        damaged = next(
            child for child in anchor.children if child.name.startswith("DAMAGED_STATE_")
        )
        repaired = next(
            child for child in anchor.children if child.name.startswith("REPAIRED_STATE_")
        )
        selection = next(
            (
                child
                for child in anchor.children
                if child.name.startswith(("SELECTION_FEEDBACK_", "SelectionFeedback"))
            ),
            None,
        )
        yield slot, anchor, damaged, repaired, selection


def configure_render():
    scene = bpy.context.scene
    scene.render.engine = "BLENDER_EEVEE"
    scene.render.resolution_x = 1280
    scene.render.resolution_y = 1280
    scene.render.resolution_percentage = 100
    scene.render.image_settings.file_format = "PNG"
    scene.render.image_settings.color_mode = "RGBA"
    scene.render.film_transparent = False
    return scene


def render_state(scene, repaired):
    for slot, anchor, damaged, operational, selection in states():
        visible = repaired or slot < 7
        anchor.hide_render = not visible
        set_tree_visible(damaged, visible and not repaired)
        set_tree_visible(operational, visible and repaired)
        if selection is not None:
            set_tree_visible(selection, False)
    suffix = "Operational" if repaired else "Damaged"
    scene.render.filepath = os.path.join(
        OUTPUT_DIR, f"QF_MachineCube_Face3_NodeKitV2_{suffix}.png"
    )
    bpy.ops.render.render(write_still=True)


if __name__ == "__main__":
    active_scene = configure_render()
    render_state(active_scene, True)
    render_state(active_scene, False)

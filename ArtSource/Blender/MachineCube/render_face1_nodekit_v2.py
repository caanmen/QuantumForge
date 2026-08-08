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


def render_operational(scene):
    for _slot, anchor, damaged, repaired, selection in states():
        anchor.hide_render = False
        set_tree_visible(damaged, False)
        set_tree_visible(repaired, True)
        if selection is not None:
            set_tree_visible(selection, False)
    scene.render.filepath = os.path.join(
        OUTPUT_DIR,
        "QF_MachineCube_Face1_NodeKitV2_Operational.png",
    )
    bpy.ops.render.render(write_still=True)


def render_damaged(scene):
    for slot, anchor, damaged, repaired, selection in states():
        anchor.hide_render = slot >= 7
        set_tree_visible(repaired, False)
        set_tree_visible(damaged, slot < 7)
        if selection is not None:
            set_tree_visible(selection, False)
    scene.render.filepath = os.path.join(
        OUTPUT_DIR,
        "QF_MachineCube_Face1_NodeKitV2_Damaged.png",
    )
    bpy.ops.render.render(write_still=True)


if __name__ == "__main__":
    active_scene = configure_render()
    render_operational(active_scene)
    render_damaged(active_scene)

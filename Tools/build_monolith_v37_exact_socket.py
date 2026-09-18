from pathlib import Path

import numpy as np
from PIL import Image, ImageDraw, ImageFilter

from build_monolith_v36_background_preview import monolith_stage_zero


ROOT = Path(__file__).resolve().parents[1]
V06 = ROOT / (
    "Assets/Project/UI/Vertical/Machine/Monolith2D/"
    "machine_destroyed_industrial_lab_background_square_v06.png"
)
V36 = ROOT / (
    "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/"
    "MAQUINA/MONOLITO_2D_CORRECCIONES_INTERACCION_2026-08-26_V02/"
    "CANDIDATOS_V36/machine_destroyed_lab_monolith_floor_perspective_square_v36.png"
)
OUTPUT_DIR = ROOT / (
    "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/"
    "MAQUINA/MONOLITO_2D_CORRECCIONES_INTERACCION_2026-08-26_V02/"
    "CANDIDATOS_V37"
)
BACKGROUND_OUTPUT = OUTPUT_DIR / (
    "machine_destroyed_lab_monolith_exact_floor_socket_square_v37.png"
)
PREVIEW_OUTPUT = OUTPUT_DIR / "monolith_exact_floor_socket_fit_preview_v37.png"
LIP_OUTPUT = OUTPUT_DIR / "monolith_floor_socket_front_lip_overlay_v37.png"


def mask_array(image: Image.Image) -> np.ndarray:
    return np.asarray(image, dtype=np.uint8) > 0


def image_mask(mask: np.ndarray, blur: float = 0.0) -> Image.Image:
    result = Image.fromarray(np.where(mask, 255, 0).astype(np.uint8), "L")
    return result.filter(ImageFilter.GaussianBlur(blur)) if blur else result


def expanded(mask: Image.Image, size: int) -> Image.Image:
    return mask.filter(ImageFilter.MaxFilter(size))


def contracted(mask: Image.Image, size: int) -> Image.Image:
    return mask.filter(ImageFilter.MinFilter(size))


def add_color_under_mask(
    canvas: Image.Image, color_layer: Image.Image, mask: Image.Image
) -> Image.Image:
    return Image.composite(color_layer, canvas, mask)


def build_socket_background() -> tuple[Image.Image, Image.Image]:
    base = Image.open(V06).convert("RGB")
    generated_metal = Image.open(V36).convert("RGB")
    width, height = base.size

    monolith = monolith_stage_zero().resize((903, 868), Image.Resampling.LANCZOS)
    placed_alpha = Image.new("L", (width, height), 0)
    placed_alpha.paste(monolith.getchannel("A"), (176, 159))
    alpha = np.asarray(placed_alpha, dtype=np.uint8) > 128

    # Build the receiving bed directly from the lower Monolith alpha instead
    # of filling its min/max bounds.  Closing only the small internal cracks
    # preserves the large stepped/asymmetric footprint and prevents a generic
    # rectangular tray.
    contact = alpha.copy()
    contact[:850] = False
    contact[998:] = False
    contact_img = image_mask(contact)
    contact_img = expanded(contact_img, 25)
    contact_img = contracted(contact_img, 13)
    contact_img = contact_img.filter(ImageFilter.GaussianBlur(1.0))
    outer = expanded(contact_img, 31)
    outer_wide = expanded(contact_img, 49)

    canvas = base.copy()
    base_np = np.asarray(base, dtype=np.float32)
    metal_np = np.asarray(generated_metal, dtype=np.float32)

    shadow_np = np.clip(base_np * 0.46, 0, 255).astype(np.uint8)
    shadow = Image.fromarray(shadow_np, "RGB")
    shadow_mask = outer_wide.filter(ImageFilter.GaussianBlur(7))
    canvas = add_color_under_mask(canvas, shadow, shadow_mask)

    recess_np = np.clip(base_np * 0.33 + np.array([5, 6, 6]), 0, 255).astype(np.uint8)
    recess = Image.fromarray(recess_np, "RGB")
    canvas = add_color_under_mask(canvas, recess, contact_img)

    # Reuse the generated industrial metal texture, but constrain it to the
    # exact silhouette-derived rim instead of importing its oversized frame.
    textured_np = np.clip(
        metal_np * 0.52 + base_np * 0.20 + np.array([18, 19, 18]), 0, 255
    ).astype(np.uint8)
    textured = Image.fromarray(textured_np, "RGB")
    rim_arr = np.clip(
        np.asarray(outer, dtype=np.int16) - np.asarray(contact_img, dtype=np.int16),
        0,
        255,
    ).astype(np.uint8)
    rim = Image.fromarray(rim_arr, "L")
    canvas = add_color_under_mask(canvas, textured, rim)

    # Narrow front retainer matched to the 267..987 footprint.  It is emitted
    # as a separate transparent environment overlay, because a lip baked into
    # the background can never occlude the lower 1% of the Monolith artwork.
    lip_mask = Image.new("L", (width, height), 0)
    draw = ImageDraw.Draw(lip_mask)
    # Three connected levels follow the actual lower base: the outer blocks
    # stop higher, while the central foot reaches lower.
    draw.polygon(
        [(252, 963), (382, 963), (404, 974), (404, 991),
         (273, 991), (273, 985), (259, 985)], fill=255
    )
    draw.polygon(
        [(374, 971), (936, 971), (936, 998), (374, 998)], fill=255
    )
    draw.polygon(
        [(920, 963), (1002, 963), (995, 985), (981, 985),
         (981, 991), (920, 991), (900, 974)], fill=255
    )
    lip_shadow = expanded(lip_mask, 11).filter(ImageFilter.GaussianBlur(4))
    lip_overlay = Image.new("RGBA", (width, height), (0, 0, 0, 0))
    shadow_rgba = shadow.convert("RGBA")
    shadow_rgba.putalpha(lip_shadow.point(lambda value: int(value * 0.42)))
    lip_overlay.alpha_composite(shadow_rgba)
    # Sample the real front metal from the generated V36 support and compress
    # only that strip to the measured width.  This avoids a translucent gray
    # rectangle and keeps the existing industrial material language.
    lip_texture = generated_metal.crop((180, 1004, 1074, 1040)).resize(
        (750, 36), Image.Resampling.LANCZOS
    ).convert("RGBA")
    positioned_texture = Image.new("RGBA", (width, height), (0, 0, 0, 0))
    positioned_texture.alpha_composite(lip_texture, (252, 963))
    positioned_texture.putalpha(lip_mask)
    lip_overlay.alpha_composite(positioned_texture)

    # Controlled bevels make the socket read as recessed floor hardware.
    outer_edge = np.clip(
        np.asarray(outer, dtype=np.int16)
        - np.asarray(contracted(outer, 7), dtype=np.int16),
        0,
        255,
    ).astype(np.uint8)
    inner_edge = np.clip(
        np.asarray(expanded(contact_img, 7), dtype=np.int16)
        - np.asarray(contact_img, dtype=np.int16),
        0,
        255,
    ).astype(np.uint8)
    highlight_np = np.clip(base_np * 0.28 + np.array([68, 69, 65]), 0, 255).astype(np.uint8)
    highlight = Image.fromarray(highlight_np, "RGB")
    canvas = add_color_under_mask(canvas, highlight, Image.fromarray(outer_edge, "L"))
    canvas = add_color_under_mask(canvas, highlight, Image.fromarray(inner_edge, "L"))

    # Front lip top highlight and lower shadow remain almost horizontal, in
    # agreement with the canonical floor perspective.
    draw = ImageDraw.Draw(lip_overlay)
    draw.line([(258, 964), (380, 964)], fill=(89, 90, 86, 255), width=2)
    draw.line([(379, 972), (931, 972)], fill=(84, 85, 81, 255), width=2)
    draw.line([(922, 964), (996, 964)], fill=(89, 90, 86, 255), width=2)
    return canvas, lip_overlay


def main() -> None:
    OUTPUT_DIR.mkdir(parents=True, exist_ok=True)
    background, lip_overlay = build_socket_background()
    background.save(BACKGROUND_OUTPUT)
    lip_overlay.save(LIP_OUTPUT)

    preview = background.convert("RGBA")
    monolith = monolith_stage_zero().resize((903, 868), Image.Resampling.LANCZOS)
    preview.alpha_composite(monolith, (176, 159))
    preview.alpha_composite(lip_overlay)
    preview.convert("RGB").save(PREVIEW_OUTPUT)
    print(BACKGROUND_OUTPUT)
    print(LIP_OUTPUT)
    print(PREVIEW_OUTPUT)


if __name__ == "__main__":
    main()

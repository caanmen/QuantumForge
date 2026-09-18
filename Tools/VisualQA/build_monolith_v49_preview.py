from pathlib import Path

import numpy as np
from PIL import Image


ROOT = Path(__file__).resolve().parents[2]
ASSET_ROOT = ROOT / "Assets/Project/UI/Vertical/Machine/Monolith2D"
OUTPUT_ROOT = (
    ROOT
    / "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS"
    / "MAQUINA/MONOLITO_2D_CORRECCIONES_INTERACCION_2026-08-26_V02"
    / "CORRECCION_V49_BANDA_PIEDRA_GRIS/PREVIEWS_OFFLINE"
)

SYMBOL_SOURCE_POSITIONS = [
    (0.4756, 0.1844),
    (0.2803, 0.3332),
    (0.6111, 0.3443),
    (0.3269, 0.5028),
    (0.4993, 0.6083),
    (0.3764, 0.7560),
    (0.6314, 0.7570),
]
CANONICAL_INDICES = {
    2: [3, 0, 5, 2, 6, 1, 4, 0, 2, 5, 1],
    3: [4, 1, 6, 0, 5, 2, 3],
}
DISPLAY_POSITIONS = {
    2: [
        (0.561, 0.697), (0.601, 0.618), (0.571, 0.533),
        (0.630, 0.491), (0.553, 0.463), (0.602, 0.413),
        (0.649, 0.358), (0.580, 0.330), (0.620, 0.270),
        (0.555, 0.240), (0.680, 0.220),
    ],
    3: [
        (0.562, 0.687), (0.606, 0.625), (0.603, 0.462),
        (0.578, 0.342), (0.646, 0.334), (0.569, 0.259),
        (0.641, 0.235),
    ],
}
FACE_PATHS = {
    2: ASSET_ROOT / "monolith_sector_2_exposed_surface_progression_v43.png",
    3: ASSET_ROOT / "monolith_sector_3_exposed_surface_progression_v43.png",
}


def smoothstep(edge0, edge1, value):
    normalized = np.clip((value - edge0) / (edge1 - edge0), 0.0, 1.0)
    return normalized * normalized * (3.0 - 2.0 * normalized)


def crop_uv(image, uv):
    x, y, width, height = uv
    pixel_width, pixel_height = image.size
    left = round(x * pixel_width)
    right = round((x + width) * pixel_width)
    top = round((1.0 - y - height) * pixel_height)
    bottom = round((1.0 - y) * pixel_height)
    return image.crop((left, top, right, bottom))


def symbol_uv(position):
    normalized_width = 0.112
    normalized_height = 0.063
    x, y = position
    return (
        0.5 + (x - normalized_width * 0.5) * 0.5,
        (y - normalized_height * 0.5) * 0.5,
        normalized_width * 0.5,
        normalized_height * 0.5,
    )


def extract_symbol(sheet, source_position, opacity):
    repaired_uv = symbol_uv(source_position)
    damaged_uv = (
        repaired_uv[0] - 0.5,
        repaired_uv[1] + 0.5,
        repaired_uv[2],
        repaired_uv[3],
    )
    repaired_image = crop_uv(sheet, repaired_uv).convert("RGB")
    damaged_image = crop_uv(sheet, damaged_uv).convert("RGB")
    if damaged_image.size != repaired_image.size:
        damaged_image = damaged_image.resize(repaired_image.size, Image.Resampling.BILINEAR)
    repaired = np.asarray(repaired_image, dtype=np.float32) / 255.0
    damaged = np.asarray(damaged_image, dtype=np.float32) / 255.0
    difference = np.max(np.abs(repaired - damaged), axis=2)
    cyan_signal = np.clip((repaired[:, :, 1] + repaired[:, :, 2]) * 0.5 - repaired[:, :, 0] * 0.62, 0.0, 1.0)
    mask = np.maximum(
        smoothstep(0.010, 0.075, difference),
        smoothstep(0.050, 0.190, cyan_signal),
    )
    mask = np.clip(mask * 1.18 * opacity, 0.0, 1.0)
    rgba = np.dstack((repaired, mask))
    return Image.fromarray(np.uint8(np.clip(rgba, 0.0, 1.0) * 255), "RGBA")


def render_sector(sector, background, symbol_sheet):
    face_sheet = Image.open(FACE_PATHS[sector]).convert("RGBA")
    cell_width = face_sheet.width // 2
    cell_height = face_sheet.height // 2
    face = face_sheet.crop((0, 0, cell_width, cell_height))
    canvas = background.resize(face.size, Image.Resampling.LANCZOS).convert("RGBA")
    canvas.alpha_composite(face)

    symbol_size = 32
    for node_index, (display_x, display_y) in enumerate(DISPLAY_POSITIONS[sector]):
        canonical_index = CANONICAL_INDICES[sector][node_index]
        opacity = 0.40 if node_index == 0 else 0.24
        symbol = extract_symbol(
            symbol_sheet,
            SYMBOL_SOURCE_POSITIONS[canonical_index],
            opacity,
        ).resize((symbol_size, symbol_size), Image.Resampling.LANCZOS)
        center_x = round(display_x * cell_width)
        center_y = round((1.0 - display_y) * cell_height)
        canvas.alpha_composite(
            symbol,
            (center_x - symbol_size // 2, center_y - symbol_size // 2),
        )

    alpha_box = face.getchannel("A").getbbox()
    left = max(0, alpha_box[0] - 45)
    top = max(0, alpha_box[1] - 12)
    right = min(cell_width, alpha_box[2] + 45)
    bottom = min(cell_height, alpha_box[3] + 12)
    portrait = canvas.crop((left, top, right, bottom))
    target_height = 900
    target_width = round(portrait.width * target_height / portrait.height)
    return portrait.resize((target_width, target_height), Image.Resampling.LANCZOS)


def main():
    OUTPUT_ROOT.mkdir(parents=True, exist_ok=True)
    background = Image.open(
        ASSET_ROOT / "machine_destroyed_lab_close_dolly_square_v27.png"
    ).convert("RGB")
    symbol_sheet = Image.open(
        ASSET_ROOT / "monolith_sector_1_progression_v02.png"
    ).convert("RGB")

    rendered = []
    for sector in (2, 3):
        preview = render_sector(sector, background, symbol_sheet)
        output = OUTPUT_ROOT / f"preview_v49_sector_{sector}_piedra_gris.png"
        preview.save(output, optimize=True)
        rendered.append(preview)

    gap = 24
    contact_width = sum(image.width for image in rendered) + gap
    contact_height = max(image.height for image in rendered)
    contact = Image.new("RGB", (contact_width, contact_height), (2, 7, 10))
    x = 0
    for image in rendered:
        contact.paste(image.convert("RGB"), (x, 0))
        x += image.width + gap
    contact.save(OUTPUT_ROOT / "comparativa_v49_sectores_2_y_3.png", optimize=True)


if __name__ == "__main__":
    main()

from pathlib import Path

from PIL import Image, ImageDraw, ImageFilter


ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "Assets/Project/UI/Vertical/Machine/Monolith2D/monolith_overview_progression_clean_v03.png"
OUTPUT = ROOT / "tmp/monolith_faces_v31"

CELL = 627
SHEET = 1254
TARGET_MARGIN = 28


# Coordinates are local to one 627 x 627 canonical stage.  Each polygon isolates
# the physical face from the adjacent central machinery while preserving the
# stage-specific broken exterior edges from the canonical Monolith.
SECTORS = {
    1: {
        "crop": (108, 8, 304, 318),
        "polygon": [(118, 12), (292, 12), (286, 302), (112, 312)],
    },
    2: {
        "crop": (344, 266, 548, 520),
        "polygon": [(360, 272), (530, 280), (544, 510), (350, 516), (352, 388)],
    },
    3: {
        "crop": (338, 8, 548, 318),
        "polygon": [(354, 12), (532, 20), (544, 304), (346, 312), (348, 152)],
    },
    4: {
        "crop": (82, 266, 326, 520),
        "polygon": [(96, 272), (312, 270), (320, 512), (88, 516), (90, 386)],
    },
}


def is_checker(rgb):
    r, g, b = rgb
    return min(rgb) > 205 and max(rgb) - min(rgb) < 18


def isolate(stage: Image.Image, sector: int) -> Image.Image:
    spec = SECTORS[sector]
    crop_box = spec["crop"]
    crop = stage.crop(crop_box).convert("RGBA")

    poly = [(x - crop_box[0], y - crop_box[1]) for x, y in spec["polygon"]]
    polygon_mask = Image.new("L", crop.size, 0)
    ImageDraw.Draw(polygon_mask).polygon(poly, fill=255)

    pixels = crop.load()
    mask_pixels = polygon_mask.load()
    for y in range(crop.height):
        for x in range(crop.width):
            if mask_pixels[x, y] == 0 or is_checker(pixels[x, y][:3]):
                pixels[x, y] = (0, 0, 0, 0)

    alpha = crop.getchannel("A")
    bbox = alpha.getbbox()
    if bbox is None:
        raise RuntimeError(f"Sector {sector} produced an empty extraction")
    return crop.crop(bbox)


def fit_to_cell(face: Image.Image) -> Image.Image:
    max_side = CELL - (TARGET_MARGIN * 2)
    scale = min(max_side / face.width, max_side / face.height)
    size = (max(1, round(face.width * scale)), max(1, round(face.height * scale)))
    resized = face.resize(size, Image.Resampling.LANCZOS)
    resized = resized.filter(ImageFilter.UnsharpMask(radius=1.0, percent=80, threshold=3))

    cell = Image.new("RGBA", (CELL, CELL), (0, 0, 0, 0))
    x = (CELL - resized.width) // 2
    y = (CELL - resized.height) // 2
    cell.alpha_composite(resized, (x, y))
    return cell


def main():
    OUTPUT.mkdir(parents=True, exist_ok=True)
    source = Image.open(SOURCE).convert("RGB")
    if source.size != (SHEET, SHEET):
        raise RuntimeError(f"Unexpected canonical sheet size: {source.size}")

    for sector in range(1, 5):
        sheet = Image.new("RGBA", (SHEET, SHEET), (0, 0, 0, 0))
        for stage_index in range(4):
            col = stage_index % 2
            row = stage_index // 2
            stage = source.crop((col * CELL, row * CELL, (col + 1) * CELL, (row + 1) * CELL))
            face = fit_to_cell(isolate(stage, sector))
            sheet.alpha_composite(face, (col * CELL, row * CELL))

        target = OUTPUT / f"monolith_sector_{sector}_canonical_extraction_v31.png"
        sheet.save(target)
        print(target)


if __name__ == "__main__":
    main()

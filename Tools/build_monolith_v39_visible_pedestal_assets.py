from pathlib import Path

import numpy as np
from PIL import Image, ImageDraw, ImageEnhance, ImageFilter


ROOT = Path(__file__).resolve().parents[1]
REFERENCE_DIR = ROOT / (
    "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/"
    "MAQUINA/MONOLITO_2D_CORRECCIONES_INTERACCION_2026-08-26_V02/"
    "CANDIDATOS_V39"
)
CONCEPT = REFERENCE_DIR / "monolith_visible_fitted_pedestal_concept_v39.png"
OVERVIEW = ROOT / (
    "Assets/Project/UI/Vertical/Machine/Monolith2D/"
    "monolith_overview_progression_clean_v03.png"
)
LAB = ROOT / (
    "Assets/Project/UI/Vertical/Machine/Monolith2D/"
    "machine_destroyed_industrial_lab_background_square_v06.png"
)
ASSET_DIR = ROOT / "Assets/Project/UI/Vertical/Machine/Monolith2D"
BACK_OUTPUT = ASSET_DIR / "machine_monolith_fitted_pedestal_back_v39.png"
FRONT_OUTPUT = ASSET_DIR / "machine_monolith_fitted_pedestal_front_v39.png"
PREVIEW_OUTPUT = REFERENCE_DIR / (
    "monolith_canonical_layered_visible_pedestal_preview_v39.png"
)

CELL = 627
DISPLAY_SIZE = (903, 868)
DISPLAY_POSITION = (176, 159)
PEDESTAL_DISPLAY_SIZE = (903, 941)
PEDESTAL_DISPLAY_POSITION = (176, 159)


def between(value: np.ndarray, minimum: float, maximum: float) -> np.ndarray:
    return (value >= minimum) & (value <= maximum)


def stage_source(sheet: Image.Image, stage: int) -> Image.Image:
    column = stage % 2
    row = stage // 2
    return sheet.crop(
        (column * CELL, row * CELL, (column + 1) * CELL, (row + 1) * CELL)
    ).convert("RGBA")


def stage_matter(stage: Image.Image) -> Image.Image:
    """Recover only the projected canonical Monolith footprint from the V03 atlas."""
    source = np.asarray(stage, dtype=np.uint8)
    rgb = source[:, :, :3]
    yy, xx = np.mgrid[0:CELL, 0:CELL]
    u = xx / (CELL - 1)
    v = 1.0 - yy / (CELL - 1)

    keyed = np.max(rgb, axis=2) >= 7
    checker = ((np.max(rgb, axis=2) - np.min(rgb, axis=2)) <= 5) & (
        np.min(rgb, axis=2) >= 180
    )

    top_left_t = np.clip((v - .555) / (.925 - .555), 0, 1)
    top_left = (
        (u >= (.255 + (.335 - .255) * top_left_t))
        & (u <= (.475 + (.455 - .475) * top_left_t))
        & between(v, .555, .925)
    )
    top_right_t = np.clip((v - .545) / (.910 - .545), 0, 1)
    top_right = (
        (u >= (.535 + (.625 - .535) * top_right_t))
        & (u <= (.770 + (.745 - .770) * top_right_t))
        & between(v, .545, .910)
    )
    bottom_left_t = np.clip((v - .125) / (.570 - .125), 0, 1)
    bottom_left = (
        (u >= (.145 + (.235 - .145) * bottom_left_t))
        & (u <= (.485 + (.475 - .485) * bottom_left_t))
        & between(v, .125, .570)
    )
    bottom_right_t = np.clip((v - .125) / (.560 - .125), 0, 1)
    bottom_right = (
        (u >= (.515 + (.535 - .515) * bottom_right_t))
        & (u <= (.855 + (.775 - .855) * bottom_right_t))
        & between(v, .125, .560)
    )
    core = between(u, .395, .615) & between(v, .105, .900)
    spine_t = np.clip((v - .115) / (.900 - .115), 0, 1)
    spine_half_width = .090 + (.055 - .090) * spine_t
    spine = (np.abs(u - .505) <= spine_half_width) & between(v, .115, .900)
    base = between(u, .100, .900) & between(v, .040, .190)
    matter = (
        top_left | top_right | bottom_left | bottom_right | spine
        | (keyed & (core | base))
    ) & ~checker
    result = Image.fromarray(np.where(matter, 255, 0).astype(np.uint8), "L")
    return result.filter(ImageFilter.GaussianBlur(.65))


def polygon_mask(points: list[tuple[int, int]]) -> Image.Image:
    mask = Image.new("L", (CELL, CELL), 0)
    ImageDraw.Draw(mask).polygon(points, fill=255)
    return mask


def support_masks(matter_image: Image.Image) -> tuple[Image.Image, Image.Image]:
    """Build a compact, visible two-tier pedestal around the locked V03 footprint.

    The outer box stays identical in every stage so repair progression never makes the
    pedestal jump. Only the narrow contact lip follows the current lower contour.
    """
    matter = np.asarray(matter_image, dtype=np.uint8) > 128
    height_ratio = .96 / 1.04

    # Shallow bed behind the Monolith: 18-30 final pixels remain visible around its base.
    back_fixed = polygon_mask(
        [(48, 553), (60, 494), (567, 494), (579, 553), (566, 561), (61, 561)]
    )
    back_array = np.asarray(back_fixed, dtype=np.uint8) > 128
    remapped_matter = np.zeros_like(matter)
    matter_rows, matter_columns = np.nonzero(matter)
    remapped_rows = np.clip(
        np.rint(matter_rows * height_ratio).astype(int), 0, CELL - 1
    )
    remapped_matter[remapped_rows, matter_columns] = True
    remapped_matter = np.asarray(
        Image.fromarray(np.where(remapped_matter, 255, 0).astype(np.uint8), "L")
        .filter(ImageFilter.MaxFilter(3)),
        dtype=np.uint8,
    ) > 128
    back_array &= ~remapped_matter
    back_image = Image.fromarray(
        np.where(back_array, 255, 0).astype(np.uint8), "L"
    ).filter(ImageFilter.GaussianBlur(.8))

    # A narrow retainer adapts to each repair stage and overlaps at most 9 local pixels.
    contact = np.zeros((CELL, CELL), dtype=bool)
    lower = matter.copy()
    lower[:470] = False
    for x in range(CELL):
        ys = np.flatnonzero(lower[:, x])
        if ys.size == 0:
            continue
        bottom = int(round(int(ys.max()) * height_ratio))
        contact[max(528, bottom - 8):min(CELL, bottom + 5), x] = True
    contact_image = Image.fromarray(
        np.where(contact, 255, 0).astype(np.uint8), "L"
    ).filter(ImageFilter.MaxFilter(7))

    # Two visible front tiers. They are only 6-9% wider than the projected base.
    upper_tier = polygon_mask(
        [(42, 557), (51, 545), (576, 545), (585, 557), (578, 585), (49, 585)]
    )
    lower_tier = polygon_mask(
        [(52, 573), (575, 573), (569, 606), (58, 606), (47, 596), (580, 596)]
    )
    front_array = np.maximum.reduce(
        [
            np.asarray(contact_image, dtype=np.uint8),
            np.asarray(upper_tier, dtype=np.uint8),
            np.asarray(lower_tier, dtype=np.uint8),
        ]
    )
    front_image = Image.fromarray(front_array, "L").filter(
        ImageFilter.GaussianBlur(.65)
    )
    return back_image, front_image


def local_concept_texture(concept: Image.Image) -> Image.Image:
    x, y = DISPLAY_POSITION
    w, h = DISPLAY_SIZE
    texture = concept.crop((x, y, x + w, y + h)).resize(
        (CELL, CELL), Image.Resampling.LANCZOS
    ).convert("RGB")
    # The V39 pedestal artwork occupies the last ~100 rows of the concept. Stretch
    # that local band into the taller production holder without moving its contact.
    source = np.asarray(texture, dtype=np.uint8)
    remapped = source.copy()
    for output_y in range(470, CELL):
        source_y = int(round(510 + (output_y - 470) * (116 / 156)))
        remapped[output_y] = source[min(CELL - 1, source_y)]
    texture = Image.fromarray(remapped, "RGB")
    # The pedestal must stay distinct from the V06 floor at mobile resolution.
    texture = ImageEnhance.Contrast(texture).enhance(1.18)
    texture = ImageEnhance.Brightness(texture).enhance(1.06)
    return texture


def rgba_from_texture(texture: Image.Image, alpha: Image.Image) -> Image.Image:
    output = texture.convert("RGBA")
    output.putalpha(alpha)
    return output


def atlas_with_stages(stages: list[Image.Image]) -> Image.Image:
    atlas = Image.new("RGBA", (CELL * 2, CELL * 2), (0, 0, 0, 0))
    for index, stage in enumerate(stages):
        atlas.alpha_composite(stage, ((index % 2) * CELL, (index // 2) * CELL))
    return atlas


def alpha_bbox(image: Image.Image) -> tuple[int, int, int, int] | None:
    return image.getchannel("A").getbbox()


def main() -> None:
    concept = Image.open(CONCEPT).convert("RGB")
    sheet = Image.open(OVERVIEW).convert("RGB")
    lab = Image.open(LAB).convert("RGB")
    texture = local_concept_texture(concept)

    matter_stages: list[Image.Image] = []
    back_stages: list[Image.Image] = []
    front_stages: list[Image.Image] = []
    for stage_index in range(4):
        stage = stage_source(sheet, stage_index)
        matter = stage_matter(stage)
        back_mask, front_mask = support_masks(matter)
        matter_stages.append(matter)
        back_stages.append(rgba_from_texture(texture, back_mask))
        front_stages.append(rgba_from_texture(texture, front_mask))

    atlas_with_stages(back_stages).save(BACK_OUTPUT)
    atlas_with_stages(front_stages).save(FRONT_OUTPUT)

    preview = lab.convert("RGBA")
    preview.alpha_composite(
        back_stages[0].resize(PEDESTAL_DISPLAY_SIZE, Image.Resampling.LANCZOS),
        PEDESTAL_DISPLAY_POSITION,
    )
    canonical = stage_source(sheet, 0)
    canonical.putalpha(matter_stages[0])
    preview.alpha_composite(
        canonical.resize(DISPLAY_SIZE, Image.Resampling.LANCZOS), DISPLAY_POSITION
    )
    preview.alpha_composite(
        front_stages[0].resize(PEDESTAL_DISPLAY_SIZE, Image.Resampling.LANCZOS),
        PEDESTAL_DISPLAY_POSITION,
    )
    preview.convert("RGB").save(PREVIEW_OUTPUT)

    print(BACK_OUTPUT)
    print(FRONT_OUTPUT)
    print(PREVIEW_OUTPUT)
    for index, (back, front) in enumerate(zip(back_stages, front_stages)):
        print(
            f"stage {index}: back={alpha_bbox(back)} front={alpha_bbox(front)}"
        )


if __name__ == "__main__":
    main()

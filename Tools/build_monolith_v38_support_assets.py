from pathlib import Path

import numpy as np
from PIL import Image, ImageFilter


ROOT = Path(__file__).resolve().parents[1]
CONCEPT = ROOT / (
    "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/"
    "MAQUINA/MONOLITO_2D_CORRECCIONES_INTERACCION_2026-08-26_V02/"
    "CANDIDATOS_V38/monolith_exact_receptacle_fit_concept_v38.png"
)
OVERVIEW = ROOT / (
    "Assets/Project/UI/Vertical/Machine/Monolith2D/"
    "monolith_overview_progression_clean_v03.png"
)
LAB = ROOT / (
    "Assets/Project/UI/Vertical/Machine/Monolith2D/"
    "machine_destroyed_industrial_lab_background_square_v06.png"
)
ASSET_DIR = ROOT / "Assets/Project/UI/Vertical/Machine/Monolith2D"
BACK_OUTPUT = ASSET_DIR / "machine_monolith_fitted_support_back_v38.png"
FRONT_OUTPUT = ASSET_DIR / "machine_monolith_fitted_support_front_v38.png"
PREVIEW_OUTPUT = ROOT / (
    "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/"
    "MAQUINA/MONOLITO_2D_CORRECCIONES_INTERACCION_2026-08-26_V02/"
    "CANDIDATOS_V38/monolith_canonical_layered_receptacle_preview_v38.png"
)

CELL = 627
DISPLAY_SIZE = (903, 868)
DISPLAY_POSITION = (176, 159)


def between(value: np.ndarray, minimum: float, maximum: float) -> np.ndarray:
    return (value >= minimum) & (value <= maximum)


def stage_source(sheet: Image.Image, stage: int) -> Image.Image:
    column = stage % 2
    row = stage // 2
    return sheet.crop((column * CELL, row * CELL,
                       (column + 1) * CELL, (row + 1) * CELL)).convert("RGBA")


def stage_matter(stage: Image.Image) -> Image.Image:
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


def support_masks(matter_image: Image.Image) -> tuple[Image.Image, Image.Image]:
    matter = np.asarray(matter_image, dtype=np.uint8) > 128

    lower = matter.copy()
    lower[:455] = False
    lower_image = Image.fromarray(np.where(lower, 255, 0).astype(np.uint8), "L")
    expanded = lower_image.filter(ImageFilter.MaxFilter(29))
    expanded_array = np.asarray(expanded, dtype=np.uint8) > 128
    back = expanded_array & ~matter
    back[:445] = False
    back[620:] = False
    back_image = Image.fromarray(np.where(back, 255, 0).astype(np.uint8), "L")
    back_image = back_image.filter(ImageFilter.GaussianBlur(1.2))

    # The front retainer follows the actual bottom contour of each stage and
    # overlaps it by 10 local pixels (about 14 px at the locked Unity scale).
    front = np.zeros((CELL, CELL), dtype=bool)
    for x in range(CELL):
        ys = np.flatnonzero(lower[:, x])
        if ys.size == 0:
            continue
        bottom = int(ys.max())
        front[max(455, bottom - 10):min(CELL, bottom + 9), x] = True
    front_image = Image.fromarray(np.where(front, 255, 0).astype(np.uint8), "L")
    front_image = front_image.filter(ImageFilter.MaxFilter(9))
    front_array = np.asarray(front_image, dtype=np.uint8).copy()
    front_array[:470] = 0
    front_array[622:] = 0
    front_image = Image.fromarray(front_array, "L").filter(ImageFilter.GaussianBlur(.8))
    return back_image, front_image


def local_concept_texture(concept: Image.Image) -> Image.Image:
    x, y = DISPLAY_POSITION
    w, h = DISPLAY_SIZE
    return concept.crop((x, y, x + w, y + h)).resize(
        (CELL, CELL), Image.Resampling.LANCZOS
    ).convert("RGB")


def rgba_from_texture(texture: Image.Image, alpha: Image.Image) -> Image.Image:
    output = texture.convert("RGBA")
    output.putalpha(alpha)
    return output


def atlas_with_stages(stages: list[Image.Image]) -> Image.Image:
    atlas = Image.new("RGBA", (CELL * 2, CELL * 2), (0, 0, 0, 0))
    for index, stage in enumerate(stages):
        atlas.alpha_composite(stage, ((index % 2) * CELL, (index // 2) * CELL))
    return atlas


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
    back = back_stages[0].resize(DISPLAY_SIZE, Image.Resampling.LANCZOS)
    preview.alpha_composite(back, DISPLAY_POSITION)

    canonical = stage_source(sheet, 0)
    canonical.putalpha(matter_stages[0])
    canonical = canonical.resize(DISPLAY_SIZE, Image.Resampling.LANCZOS)
    preview.alpha_composite(canonical, DISPLAY_POSITION)

    front = front_stages[0].resize(DISPLAY_SIZE, Image.Resampling.LANCZOS)
    preview.alpha_composite(front, DISPLAY_POSITION)
    preview.convert("RGB").save(PREVIEW_OUTPUT)

    print(BACK_OUTPUT)
    print(FRONT_OUTPUT)
    print(PREVIEW_OUTPUT)


if __name__ == "__main__":
    main()

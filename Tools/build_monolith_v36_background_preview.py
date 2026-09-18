from pathlib import Path

import numpy as np
from PIL import Image, ImageDraw, ImageFilter


ROOT = Path(__file__).resolve().parents[1]
GENERATED = Path(
    r"C:\Users\nedfla\.codex\generated_images\01a04100-8369-7ee2-9641-7697fa95bb06\exec-cb13f791-1f2b-4232-8772-bacbd80c655b.png"
)
V06 = ROOT / (
    "Assets/Project/UI/Vertical/Machine/Monolith2D/"
    "machine_destroyed_industrial_lab_background_square_v06.png"
)
MONOLITH = ROOT / (
    "Assets/Project/UI/Vertical/Machine/Monolith2D/"
    "monolith_overview_progression_clean_v03.png"
)
OUTPUT_DIR = ROOT / (
    "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/"
    "MAQUINA/MONOLITO_2D_CORRECCIONES_INTERACCION_2026-08-26_V02/"
    "CANDIDATOS_V36"
)
BACKGROUND_OUTPUT = OUTPUT_DIR / (
    "machine_destroyed_lab_monolith_floor_perspective_square_v36.png"
)
PREVIEW_OUTPUT = OUTPUT_DIR / "monolith_background_fit_preview_v36.png"


def between(value, minimum, maximum):
    return (value >= minimum) & (value <= maximum)


def monolith_stage_zero() -> Image.Image:
    sheet = Image.open(MONOLITH).convert("RGBA")
    stage = sheet.crop((0, 0, 627, 627))
    source = np.asarray(stage, dtype=np.uint8)
    height, width = source.shape[:2]
    yy, xx = np.mgrid[0:height, 0:width]
    u = xx / (width - 1)
    v = 1.0 - yy / (height - 1)
    rgb = source[:, :, :3]
    keyed = np.max(rgb, axis=2) >= 7
    # The canonical progression sheet is RGB and contains a baked light
    # checkerboard rather than transparency.  Remove only those bright,
    # nearly-neutral checker pixels from this measurement preview; the
    # production background does not depend on this extraction.
    checker = ((np.max(rgb, axis=2) - np.min(rgb, axis=2)) <= 5) & (
        np.min(rgb, axis=2) >= 180
    )

    top_left_t = np.clip((v - .555) / (.925 - .555), 0, 1)
    top_left_l = .255 + (.335 - .255) * top_left_t
    top_left_r = .475 + (.455 - .475) * top_left_t
    top_left = (u >= top_left_l) & (u <= top_left_r) & between(v, .555, .925)

    top_right_t = np.clip((v - .545) / (.910 - .545), 0, 1)
    top_right_l = .535 + (.625 - .535) * top_right_t
    top_right_r = .770 + (.745 - .770) * top_right_t
    top_right = (u >= top_right_l) & (u <= top_right_r) & between(v, .545, .910)

    bottom_left_t = np.clip((v - .125) / (.570 - .125), 0, 1)
    bottom_left_l = .145 + (.235 - .145) * bottom_left_t
    bottom_left_r = .485 + (.475 - .485) * bottom_left_t
    bottom_left = ((u >= bottom_left_l) & (u <= bottom_left_r) &
                   between(v, .125, .570))

    bottom_right_t = np.clip((v - .125) / (.560 - .125), 0, 1)
    bottom_right_l = .515 + (.535 - .515) * bottom_right_t
    bottom_right_r = .855 + (.775 - .855) * bottom_right_t
    bottom_right = ((u >= bottom_right_l) & (u <= bottom_right_r) &
                    between(v, .125, .560))

    core = between(u, .395, .615) & between(v, .105, .900)
    spine_t = np.clip((v - .115) / (.900 - .115), 0, 1)
    spine_half_width = .090 + (.055 - .090) * spine_t
    spine = (np.abs(u - .505) <= spine_half_width) & between(v, .115, .900)
    base = between(u, .100, .900) & between(v, .040, .190)
    mask = ((top_left | top_right | bottom_left | bottom_right | spine |
             (keyed & (core | base))) & ~checker)
    alpha = Image.fromarray(np.where(mask, 255, 0).astype(np.uint8), "L")
    alpha = alpha.filter(ImageFilter.GaussianBlur(0.8))
    stage.putalpha(alpha)
    return stage


def build_background() -> Image.Image:
    base = Image.open(V06).convert("RGB")
    edit = Image.open(GENERATED).convert("RGB")
    if base.size != (1254, 1254) or edit.size != base.size:
        raise RuntimeError(f"Unexpected source size: {base.size}, {edit.size}")

    mask = Image.new("L", base.size, 0)
    draw = ImageDraw.Draw(mask)
    draw.polygon(
        [(345, 680), (909, 680), (1115, 1048), (1115, 1085),
         (139, 1085), (139, 1048)],
        fill=255,
    )
    mask = mask.filter(ImageFilter.GaussianBlur(10))
    return Image.composite(edit, base, mask)


def main() -> None:
    OUTPUT_DIR.mkdir(parents=True, exist_ok=True)
    background = build_background()
    background.save(BACKGROUND_OUTPUT)

    preview = background.convert("RGBA")
    monolith = monolith_stage_zero().resize((903, 868), Image.Resampling.LANCZOS)
    preview.alpha_composite(monolith, (176, 159))
    preview.convert("RGB").save(PREVIEW_OUTPUT)
    print(BACKGROUND_OUTPUT)
    print(PREVIEW_OUTPUT)


if __name__ == "__main__":
    main()

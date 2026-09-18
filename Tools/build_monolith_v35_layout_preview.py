from pathlib import Path
import math

from PIL import Image, ImageDraw, ImageFont


ROOT = Path(__file__).resolve().parents[1]
ASSETS = ROOT / "Assets/Project/UI/Vertical/Machine/Monolith2D"
OUTPUT = ROOT / (
    "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/"
    "MAQUINA/MONOLITO_2D_CORRECCIONES_INTERACCION_2026-08-26_V02/"
    "CANDIDATOS_V35/node_safe_zone_preview_v35.png"
)

POSITIONS = {
    1: [(.625, .780), (.580, .680), (.580, .580), (.515, .480),
        (.520, .380), (.450, .280), (.430, .180)],
    2: [(.480, .790), (.555, .728), (.475, .665), (.590, .603),
        (.470, .540), (.605, .478), (.470, .415), (.615, .353),
        (.475, .290), (.625, .250), (.550, .165)],
    3: [(.515, .780), (.520, .680), (.550, .580), (.510, .480),
        (.570, .380), (.500, .280), (.600, .180)],
}

DISPLAY_WIDTH = {1: 567, 2: 506, 3: 567}
DISPLAY_HEIGHT = 780
SYMBOL_SIZE = {1: (56, 44), 2: (50, 44), 3: (56, 44)}


def font(size: int) -> ImageFont.FreeTypeFont | ImageFont.ImageFont:
    path = Path(r"C:\Windows\Fonts\arial.ttf")
    return ImageFont.truetype(path, size) if path.exists() else ImageFont.load_default()


def center_is_safe(sector: int, x: float, y: float) -> bool:
    if not .15 <= y <= .80:
        return False
    if sector == 1:
        return .34 + .34 * y <= x <= .65
    if sector == 2:
        return .34 + .14 * y <= x <= .73 - .22 * y
    if sector == 3:
        return .49 <= x <= .73 - .25 * y
    return False


def ellipse_is_opaque(alpha: Image.Image, cx: int, cy: int,
                      radius_x: int, radius_y: int) -> bool:
    pixels = alpha.load()
    for y in range(cy - radius_y, cy + radius_y + 1):
        for x in range(cx - radius_x, cx + radius_x + 1):
            normalized = ((x - cx) / radius_x) ** 2 + ((y - cy) / radius_y) ** 2
            if normalized > 1.0:
                continue
            if x < 0 or y < 0 or x >= alpha.width or y >= alpha.height:
                return False
            if pixels[x, y] < 224:
                return False
    return True


def validate_positions(sector: int, image: Image.Image) -> None:
    size = 627
    for index, (x, y) in enumerate(POSITIONS[sector], start=1):
        if not center_is_safe(sector, x, y):
            raise RuntimeError(f"Sector {sector} node {index} outside analytic safe zone")
    for left in range(len(POSITIONS[sector])):
        for right in range(left + 1, len(POSITIONS[sector])):
            ax, ay = POSITIONS[sector][left]
            bx, by = POSITIONS[sector][right]
        display_distance = math.hypot(
            (ax - bx) * DISPLAY_WIDTH[sector],
            (ay - by) * DISPLAY_HEIGHT,
        )
        if display_distance < 74:
                raise RuntimeError(
                    f"Sector {sector} nodes {left + 1}/{right + 1} too close"
                )
    for stage in range(4):
        col = stage % 2
        row = stage // 2
        alpha = image.getchannel("A").crop(
            (col * size, row * size, (col + 1) * size, (row + 1) * size)
        )
        for index, (x, y) in enumerate(POSITIONS[sector], start=1):
            cx = round(x * size)
            cy = round((1.0 - y) * size)
            symbol_width, symbol_height = SYMBOL_SIZE[sector]
            radius_x = math.ceil((symbol_width / 2 + 8) /
                                 DISPLAY_WIDTH[sector] * size)
            radius_y = math.ceil((symbol_height / 2 + 8) /
                                 DISPLAY_HEIGHT * size)
            if not ellipse_is_opaque(alpha, cx, cy, radius_x, radius_y):
                raise RuntimeError(
                    f"Sector {sector} stage {stage} node {index} halo leaves matter"
                )


def stage_preview(sector: int, stage_index: int) -> Image.Image:
    path = ASSETS / f"monolith_sector_{sector}_exposed_surface_progression_v35.png"
    sheet = Image.open(path).convert("RGBA")
    validate_positions(sector, sheet)
    column = stage_index % 2
    row = stage_index // 2
    stage = sheet.crop((column * 627, row * 627,
                        (column + 1) * 627, (row + 1) * 627))
    stage = stage.resize(
        (DISPLAY_WIDTH[sector], DISPLAY_HEIGHT), Image.Resampling.LANCZOS)
    canvas = Image.new("RGBA", stage.size, (8, 12, 15, 255))
    canvas.alpha_composite(stage)
    draw = ImageDraw.Draw(canvas)
    symbol_width, symbol_height = SYMBOL_SIZE[sector]
    halo_width = symbol_width + 16
    halo_height = symbol_height + 16
    for index, (x, y) in enumerate(POSITIONS[sector], start=1):
        cx = round(x * DISPLAY_WIDTH[sector])
        cy = round((1.0 - y) * DISPLAY_HEIGHT)
        draw.ellipse(
            (cx - halo_width // 2, cy - halo_height // 2,
             cx + halo_width // 2, cy + halo_height // 2),
            fill=(5, 190, 230, 38), outline=(92, 222, 246, 180), width=2,
        )
        draw.ellipse(
            (cx - symbol_width // 2, cy - symbol_height // 2,
             cx + symbol_width // 2, cy + symbol_height // 2),
            fill=(5, 190, 230, 75), outline=(205, 250, 255, 255), width=3,
        )
        draw.text((cx + 10, cy - 22), str(index), fill=(255, 255, 255, 255),
                  font=font(22))
    return canvas


def main() -> None:
    cell_width = max(DISPLAY_WIDTH.values())
    header = 54
    row_height = DISPLAY_HEIGHT + header
    sheet = Image.new("RGB", (cell_width * 4, row_height * 3), (2, 7, 10))
    draw = ImageDraw.Draw(sheet)
    for row_index, sector in enumerate((1, 2, 3)):
        for stage_index in range(4):
            x = stage_index * cell_width
            y = row_index * row_height
            preview = stage_preview(sector, stage_index).convert("RGB")
            offset_x = x + (cell_width - preview.width) // 2
            sheet.paste(preview, (offset_x, y + header))
            draw.text((x + 14, y + 14),
                      f"SECTOR {sector} · ETAPA {stage_index + 1} · FIRMA 56 PX",
                      fill=(90, 223, 255), font=font(19))
    OUTPUT.parent.mkdir(parents=True, exist_ok=True)
    sheet.save(OUTPUT)
    print(OUTPUT)


if __name__ == "__main__":
    main()

from pathlib import Path

from PIL import Image, ImageDraw, ImageFont


ROOT = Path(__file__).resolve().parents[1]
ASSETS = ROOT / "Assets/Project/UI/Vertical/Machine/Monolith2D"
OUTPUT = ROOT / (
    "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/"
    "MAQUINA/MONOLITO_2D_CORRECCIONES_INTERACCION_2026-08-26_V02/"
    "CANDIDATOS_V33/node_safe_zone_preview_v33.png"
)

POSITIONS = {
    1: [(.44, .22), (.56, .22), (.45, .39), (.58, .39),
        (.50, .58), (.59, .58), (.57, .76)],
    2: [(.46, .18), (.59, .18), (.68, .18), (.45, .33),
        (.58, .33), (.68, .33), (.46, .48), (.60, .48),
        (.47, .63), (.58, .63), (.51, .77)],
    3: [(.47, .22), (.60, .22), (.45, .39), (.59, .39),
        (.44, .58), (.56, .58), (.49, .76)],
}


def font(size: int) -> ImageFont.FreeTypeFont | ImageFont.ImageFont:
    path = Path(r"C:\Windows\Fonts\arial.ttf")
    return ImageFont.truetype(path, size) if path.exists() else ImageFont.load_default()


def stage_preview(sector: int) -> Image.Image:
    path = ASSETS / f"monolith_sector_{sector}_open_surface_progression_v33.png"
    stage = Image.open(path).convert("RGBA").crop((0, 0, 627, 627))
    canvas = Image.new("RGBA", stage.size, (8, 12, 15, 255))
    canvas.alpha_composite(stage)
    draw = ImageDraw.Draw(canvas)
    for index, (x, y) in enumerate(POSITIONS[sector], start=1):
        cx = round(x * 627)
        cy = round((1.0 - y) * 627)
        radius = 34
        draw.ellipse(
            (cx - radius, cy - radius, cx + radius, cy + radius),
            fill=(5, 190, 230, 78), outline=(180, 245, 255, 255), width=3,
        )
        draw.line((cx - 8, cy, cx + 8, cy), fill=(255, 255, 255, 255), width=2)
        draw.line((cx, cy - 8, cx, cy + 8), fill=(255, 255, 255, 255), width=2)
        draw.text((cx + 10, cy - 22), str(index), fill=(255, 255, 255, 255),
                  font=font(22))
    return canvas


def main() -> None:
    sheet = Image.new("RGB", (1881, 690), (2, 7, 10))
    draw = ImageDraw.Draw(sheet)
    for column, sector in enumerate((1, 2, 3)):
        x = column * 627
        sheet.paste(stage_preview(sector).convert("RGB"), (x, 63))
        draw.rectangle((x, 0, x + 627, 63), fill=(2, 7, 10))
        draw.text((x + 20, 18), f"SECTOR {sector} — ÁREA VISIBLE 64 PX",
                  fill=(90, 223, 255), font=font(24))
    OUTPUT.parent.mkdir(parents=True, exist_ok=True)
    sheet.save(OUTPUT)
    print(OUTPUT)


if __name__ == "__main__":
    main()

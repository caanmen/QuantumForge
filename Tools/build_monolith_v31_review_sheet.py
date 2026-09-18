from pathlib import Path

from PIL import Image, ImageDraw, ImageFont


ROOT = Path(__file__).resolve().parents[1]
CAPTURES = ROOT / "Logs/VisualQA/MachineMonolith2D"
OUTPUT = ROOT / (
    "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/"
    "MAQUINA/MONOLITO_2D_CORRECCIONES_INTERACCION_2026-08-26_V02/"
    "CANDIDATOS_V31/revision_unity_v31_contact_sheet.png"
)


def load(name: str) -> Image.Image:
    return Image.open(CAPTURES / name).convert("RGB")


def main() -> None:
    overview = load("01_overview_initial_720x1280.png")
    faces = [
        load("02_sector_1_initial_720x1280.png"),
        load("03_sector_2_initial_720x1280.png"),
        load("04_sector_3_initial_720x1280.png"),
        load("05_sector_4_reserved_720x1280.png"),
    ]
    labels = ["CARA 1", "CARA 2", "CARA 3", "CARA 4"]

    sheet = Image.new("RGB", (1440, 1340), (2, 7, 10))
    draw = ImageDraw.Draw(sheet)
    font_path = Path(r"C:\Windows\Fonts\arial.ttf")
    font = ImageFont.truetype(font_path, 22) if font_path.exists() else ImageFont.load_default(size=22)
    draw.text((24, 18), "REVISIÓN UNITY V31 — CANDIDATO, NO ASSET", fill=(210, 230, 236), font=font)
    sheet.paste(overview, (0, 60))

    for index, (face, label) in enumerate(zip(faces, labels)):
        thumb = face.resize((360, 640), Image.Resampling.LANCZOS)
        x = 720 + (index % 2) * 360
        y = 60 + (index // 2) * 640
        sheet.paste(thumb, (x, y))
        draw.rectangle((x, y, x + 108, y + 32), fill=(2, 7, 10))
        draw.text((x + 10, y + 6), label, fill=(90, 223, 255), font=font)

    OUTPUT.parent.mkdir(parents=True, exist_ok=True)
    sheet.save(OUTPUT)
    print(OUTPUT)


if __name__ == "__main__":
    main()

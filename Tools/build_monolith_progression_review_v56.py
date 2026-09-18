from pathlib import Path

from PIL import Image, ImageDraw, ImageFont


ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "Logs" / "VisualQA" / "MachineMonolith2D_V56"
STAGES = (
    ("0 %", "3 NODOS", "detail_{face_file}_initial_1080x1920.png"),
    ("20 %", "5 NODOS", "detail_stage_1_sector_{face}_partial_1080x1920.png"),
    ("40 %", "7 NODOS", "detail_stage_2_sector_{face}_partial_1080x1920.png"),
    ("60 %", "8 NODOS", "detail_stage_3_sector_{face}_partial_1080x1920.png"),
)
FACE_FILES = {1: "02_sector_1", 2: "03_sector_2", 3: "04_sector_3"}


def font(size: int, bold: bool = False) -> ImageFont.FreeTypeFont:
    filename = "arialbd.ttf" if bold else "arial.ttf"
    return ImageFont.truetype(str(Path("C:/Windows/Fonts") / filename), size)


def fit_detail(image: Image.Image, width: int, height: int) -> Image.Image:
    scale = min(width / image.width, height / image.height)
    resized = image.resize(
        (round(image.width * scale), round(image.height * scale)),
        Image.Resampling.LANCZOS,
    )
    canvas = Image.new("RGB", (width, height), "#03090d")
    canvas.paste(resized, ((width - resized.width) // 2, (height - resized.height) // 2))
    return canvas


def stage_path(face: int, template: str) -> Path:
    return SOURCE / template.format(face=face, face_file=FACE_FILES[face])


def make_face_sheet(face: int) -> Path:
    cell_w, image_h, header_h = 500, 893, 92
    gap, outer = 20, 28
    sheet_w = outer * 2 + cell_w * 2 + gap
    sheet_h = 116 + (header_h + image_h) * 2 + gap + outer
    sheet = Image.new("RGB", (sheet_w, sheet_h), "#041116")
    draw = ImageDraw.Draw(sheet)
    draw.text((outer, 24), f"CARA {face} · PROGRESIÓN DE REPARACIÓN", font=font(32, True), fill="#eaf8ff")
    draw.text((outer, 67), "Posiciones fijas · superficie utilizable creciente", font=font(21), fill="#53d7f5")

    for index, (percent, nodes, template) in enumerate(STAGES):
        col, row = index % 2, index // 2
        x = outer + col * (cell_w + gap)
        y = 116 + row * (header_h + image_h + gap)
        draw.rounded_rectangle((x, y, x + cell_w, y + header_h + image_h), 12, fill="#071a22", outline="#1fbfe8", width=2)
        draw.text((x + 22, y + 14), percent, font=font(31, True), fill="#ffffff")
        node_box = draw.textbbox((0, 0), nodes, font=font(23, True))
        draw.text((x + cell_w - 22 - (node_box[2] - node_box[0]), y + 21), nodes, font=font(23, True), fill="#43d8f6")
        detail = Image.open(stage_path(face, template)).convert("RGB")
        fitted = fit_detail(detail, cell_w - 8, image_h - 8)
        sheet.paste(fitted, (x + 4, y + header_h + 4))

    output = SOURCE / f"review_progression_face_{face}_0_20_40_60.png"
    sheet.save(output, quality=95)
    return output


def make_overview(face_sheets: list[Path]) -> Path:
    thumbs = [Image.open(path).convert("RGB") for path in face_sheets]
    target_w = 720
    resized = [image.resize((target_w, round(image.height * target_w / image.width)), Image.Resampling.LANCZOS) for image in thumbs]
    gap, outer = 24, 32
    header_h = 126
    canvas = Image.new("RGB", (target_w + outer * 2, header_h + sum(i.height for i in resized) + gap * 2 + outer), "#020b0f")
    draw = ImageDraw.Draw(canvas)
    draw.text((outer, 24), "MONOLITO · ESTADOS 0 / 20 / 40 / 60 %", font=font(29, True), fill="#eefaff")
    draw.text((outer, 70), "3 · 5 · 7 · 8 nodos por cara", font=font(23), fill="#53d7f5")
    y = header_h
    for image in resized:
        canvas.paste(image, (outer, y))
        y += image.height + gap
    output = SOURCE / "review_progression_all_faces_0_20_40_60.png"
    canvas.save(output, quality=95)
    return output


def main() -> None:
    missing = [str(stage_path(face, stage[2])) for face in FACE_FILES for stage in STAGES if not stage_path(face, stage[2]).is_file()]
    if missing:
        raise FileNotFoundError("Faltan capturas:\n" + "\n".join(missing))
    face_sheets = [make_face_sheet(face) for face in FACE_FILES]
    overview = make_overview(face_sheets)
    print("\n".join(str(path) for path in [*face_sheets, overview]))


if __name__ == "__main__":
    main()

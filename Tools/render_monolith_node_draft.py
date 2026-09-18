from __future__ import annotations

import json
import math
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont


ROOT = Path(__file__).resolve().parents[1]
DRAFT = ROOT / "Logs/VisualQA/MonolithNodeLayoutManual/draft.json"
OUTPUT = ROOT / "Logs/VisualQA/MonolithNodeLayoutManual/draft_review_3faces_4stages.png"
REPORT = ROOT / "Logs/VisualQA/MonolithNodeLayoutManual/draft_review_metrics.json"
TEXTURES = [
    ROOT / "Assets/Project/UI/Vertical/Machine/Monolith2D/monolith_sector_1_exposed_surface_progression_v35.png",
    ROOT / "Assets/Project/UI/Vertical/Machine/Monolith2D/monolith_sector_2_exposed_surface_progression_v43.png",
    ROOT / "Assets/Project/UI/Vertical/Machine/Monolith2D/monolith_sector_3_lower_right_progression_aligned_v57.png",
]
COUNTS = [3, 5, 7, 8]
STAGES = ["0 %", "20 %", "40 %", "60 %"]
NODE_FOOTPRINT_UV = (0.112, 0.063)
TILE = 420
GAP = 18
HEADER = 64
ROW_LABEL = 34


def font(size: int, bold: bool = False) -> ImageFont.FreeTypeFont | ImageFont.ImageFont:
    candidates = [
        Path("C:/Windows/Fonts/arialbd.ttf" if bold else "C:/Windows/Fonts/arial.ttf"),
        Path("C:/Windows/Fonts/segoeuib.ttf" if bold else "C:/Windows/Fonts/segoeui.ttf"),
    ]
    for candidate in candidates:
        if candidate.exists():
            return ImageFont.truetype(str(candidate), size)
    return ImageFont.load_default()


def split_sheet(path: Path) -> list[Image.Image]:
    sheet = Image.open(path).convert("RGBA")
    if sheet.width != sheet.height or sheet.width % 2:
        raise RuntimeError(f"Hoja inválida: {path} {sheet.size}")
    half = sheet.width // 2
    boxes = [
        (0, 0, half, half),
        (half, 0, sheet.width, half),
        (0, half, half, sheet.height),
        (half, half, sheet.width, sheet.height),
    ]
    return [sheet.crop(box) for box in boxes]


def distance(a: dict[str, float], b: dict[str, float]) -> float:
    return math.hypot(a["x"] - b["x"], a["y"] - b["y"])


def main() -> None:
    draft = json.loads(DRAFT.read_text(encoding="utf-8"))
    faces = draft["faces"]
    if len(faces) != 3 or any(len(face["positions"]) != 8 for face in faces):
        raise RuntimeError("El borrador debe contener 3 caras con 8 posiciones cada una.")

    canvas_width = GAP + 4 * (TILE + GAP)
    canvas_height = HEADER + 3 * (ROW_LABEL + TILE + GAP)
    canvas = Image.new("RGB", (canvas_width, canvas_height), (5, 10, 14))
    draw = ImageDraw.Draw(canvas)
    title_font = font(28, True)
    label_font = font(22, True)
    node_font = font(16, True)
    note_font = font(16)

    draw.text((GAP, 16), "BORRADOR MANUAL · MONOLITO · 3/5/7/8 NODOS",
              font=title_font, fill=(230, 242, 247))
    draw.text((canvas_width - 520, 23), draft.get("savedAtUtc", ""),
              font=note_font, fill=(125, 151, 164))

    metrics: dict[str, object] = {
        "draft_saved_at_utc": draft.get("savedAtUtc"),
        "faces": [],
        "result": "PASS",
    }

    for face_index, (face, texture_path) in enumerate(zip(faces, TEXTURES)):
        positions = face["positions"]
        tiles = split_sheet(texture_path)
        row_y = HEADER + face_index * (ROW_LABEL + TILE + GAP)
        draw.text((GAP, row_y + 3), f"CARA {face_index + 1}", font=label_font,
                  fill=(78, 221, 255))

        minimum_distance = min(
            distance(positions[i], positions[j])
            for i in range(len(positions))
            for j in range(i + 1, len(positions))
        )
        inside_bounds = all(
            0.0 <= point["x"] <= 1.0 and 0.0 <= point["y"] <= 1.0
            for point in positions
        )
        if not inside_bounds or minimum_distance < 0.068:
            metrics["result"] = "FAIL"

        metrics["faces"].append({
            "face": face_index + 1,
            "node_count": len(positions),
            "minimum_center_distance": round(minimum_distance, 6),
            "inside_normalized_bounds": inside_bounds,
        })

        for stage, tile in enumerate(tiles):
            tile = tile.resize((TILE, TILE), Image.Resampling.LANCZOS)
            x = GAP + stage * (TILE + GAP)
            y = row_y + ROW_LABEL
            canvas.paste((0, 0, 0), (x, y, x + TILE, y + TILE))
            canvas.paste(tile, (x, y), tile)
            draw.rectangle((x, y, x + TILE - 1, y + TILE - 1),
                           outline=(38, 190, 232), width=2)
            draw.text((x + 10, y + 8), f"{STAGES[stage]} · {COUNTS[stage]}",
                      font=label_font, fill=(230, 242, 247),
                      stroke_width=3, stroke_fill=(0, 0, 0))

            previous_count = COUNTS[stage - 1] if stage else 0
            for node_index in range(COUNTS[stage]):
                point = positions[node_index]
                center_x = x + point["x"] * TILE
                center_y = y + (1.0 - point["y"]) * TILE
                selected_color = (255, 151, 25) if node_index >= previous_count else (172, 84, 255)
                half_width = NODE_FOOTPRINT_UV[0] * TILE / 2
                half_height = NODE_FOOTPRINT_UV[1] * TILE / 2
                draw.rectangle((center_x - half_width, center_y - half_height,
                                center_x + half_width, center_y + half_height),
                               outline=(74, 226, 255), width=2)
                radius = 13
                draw.ellipse((center_x - radius - 3, center_y - radius - 3,
                              center_x + radius + 3, center_y + radius + 3),
                             fill=(0, 0, 0))
                draw.ellipse((center_x - radius, center_y - radius,
                              center_x + radius, center_y + radius),
                             fill=selected_color)
                text = str(node_index + 1)
                box = draw.textbbox((0, 0), text, font=node_font)
                text_w = box[2] - box[0]
                text_h = box[3] - box[1]
                draw.text((center_x - text_w / 2, center_y - text_h / 2 - 2),
                          text, font=node_font, fill=(0, 0, 0))

    OUTPUT.parent.mkdir(parents=True, exist_ok=True)
    canvas.save(OUTPUT, optimize=True)
    REPORT.write_text(json.dumps(metrics, indent=2), encoding="utf-8")
    print(json.dumps({"output": str(OUTPUT), **metrics}, indent=2))
    if metrics["result"] != "PASS":
        raise SystemExit(1)


if __name__ == "__main__":
    main()

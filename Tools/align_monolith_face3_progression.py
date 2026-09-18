from __future__ import annotations

import json
from pathlib import Path

import numpy as np
from PIL import Image


ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / (
    "Assets/Project/UI/Vertical/Machine/Monolith2D/"
    "monolith_sector_3_lower_right_progression_v55.png"
)
OUTPUT = ROOT / (
    "Assets/Project/UI/Vertical/Machine/Monolith2D/"
    "monolith_sector_3_lower_right_progression_aligned_v57.png"
)
REPORT = ROOT / "Logs/VisualQA/MonolithNodeLayoutManual/face3_alignment_v57.json"

# Las cuatro etapas viven en una hoja 2x2. El riel y la base son idénticos,
# pero los tiles de la columna izquierda están 45 px a la derecha y los de
# la fila superior 26 px abajo. Se alinea contra el estado final (abajo-derecha).
OFFSETS = [(-45, -26), (0, -26), (-45, 0), (0, 0)]


def split_tiles(image: Image.Image) -> list[Image.Image]:
    width, height = image.size
    if width != height or width % 2:
        raise RuntimeError(f"La hoja debe ser cuadrada y par; recibido {image.size}")
    size = width // 2
    return [
        image.crop((0, 0, size, size)),
        image.crop((size, 0, width, size)),
        image.crop((0, size, size, height)),
        image.crop((size, size, width, height)),
    ]


def shift_tile(tile: Image.Image, offset: tuple[int, int]) -> Image.Image:
    shifted = Image.new("RGBA", tile.size, (0, 0, 0, 0))
    shifted.alpha_composite(tile, dest=offset)
    return shifted


def stable_landmarks(tile: Image.Image) -> dict[str, int]:
    alpha = np.asarray(tile.getchannel("A")) > 32
    columns = alpha.sum(axis=0)
    rows = alpha.sum(axis=1)
    stable_columns = np.where(columns > 500)[0]
    stable_rows = np.where(rows > 300)[0]
    if not len(stable_columns) or not len(stable_rows):
        raise RuntimeError("No se encontraron el riel y la base estables del Monolito.")
    return {
        "rail_x": int(stable_columns[0]),
        "top_y": int(stable_rows[0]),
        "base_y": int(stable_rows[-1]),
    }


def main() -> None:
    source = Image.open(SOURCE).convert("RGBA")
    tiles = split_tiles(source)
    aligned = [shift_tile(tile, offset) for tile, offset in zip(tiles, OFFSETS)]

    size = source.width // 2
    sheet = Image.new("RGBA", source.size, (0, 0, 0, 0))
    destinations = [(0, 0), (size, 0), (0, size), (size, size)]
    for tile, destination in zip(aligned, destinations):
        sheet.alpha_composite(tile, dest=destination)

    OUTPUT.parent.mkdir(parents=True, exist_ok=True)
    sheet.save(OUTPUT, optimize=True)

    before = [stable_landmarks(tile) for tile in tiles]
    after = [stable_landmarks(tile) for tile in aligned]
    rail_spread = max(item["rail_x"] for item in after) - min(
        item["rail_x"] for item in after
    )
    top_spread = max(item["top_y"] for item in after) - min(
        item["top_y"] for item in after
    )
    base_spread = max(item["base_y"] for item in after) - min(
        item["base_y"] for item in after
    )
    passed = rail_spread <= 2 and top_spread <= 2 and base_spread <= 1

    report = {
        "source": str(SOURCE.relative_to(ROOT)).replace("\\", "/"),
        "output": str(OUTPUT.relative_to(ROOT)).replace("\\", "/"),
        "tile_size": [size, size],
        "offsets_pixels": OFFSETS,
        "landmarks_before": before,
        "landmarks_after": after,
        "maximum_spread_pixels": {
            "rail_x": rail_spread,
            "top_y": top_spread,
            "base_y": base_spread,
        },
        "result": "PASS" if passed else "FAIL",
    }
    REPORT.parent.mkdir(parents=True, exist_ok=True)
    REPORT.write_text(json.dumps(report, indent=2), encoding="utf-8")
    print(json.dumps(report, indent=2))
    if not passed:
        raise SystemExit(1)


if __name__ == "__main__":
    main()

from pathlib import Path
import math
import random

import numpy as np
from PIL import Image


ROOT = Path(__file__).resolve().parents[1]
ASSETS = ROOT / "Assets/Project/UI/Vertical/Machine/Monolith2D"
POSITIONS = {
    1: [(.625, .780), (.580, .680), (.580, .580), (.515, .480),
        (.520, .380), (.450, .280), (.430, .180)],
    2: [(.480, .790), (.555, .728), (.475, .665), (.590, .603),
        (.470, .540), (.605, .478), (.470, .415), (.615, .353),
        (.475, .290), (.625, .250), (.550, .165)],
    3: [(.515, .780), (.520, .680), (.550, .580), (.510, .480),
        (.570, .380), (.500, .280), (.600, .180)],
}


def main() -> None:
    size = 627
    for sector, positions in POSITIONS.items():
        display_width = {1: 567, 2: 506, 3: 567}[sector]
        symbol_width = {1: 56, 2: 50, 3: 56}[sector]
        radius_x = math.ceil((symbol_width / 2 + 8) / display_width * size)
        radius_y = math.ceil((44 / 2 + 8) / 780 * size)
        yy, xx = np.ogrid[-radius_y:radius_y + 1, -radius_x:radius_x + 1]
        circle = (xx / radius_x) ** 2 + (yy / radius_y) ** 2 <= 1
        path = ASSETS / f"monolith_sector_{sector}_exposed_surface_progression_v35.png"
        alpha = np.asarray(Image.open(path).convert("RGBA").getchannel("A"))
        stages = [
            alpha[(stage // 2) * size:(stage // 2 + 1) * size,
                  (stage % 2) * size:(stage % 2 + 1) * size]
            for stage in range(4)
        ]
        print(f"SECTOR {sector}")
        for normalized_y in sorted({position[1] for position in positions}):
            center_y = round((1.0 - normalized_y) * size)
            safe = []
            for center_x in range(radius_x, size - radius_x):
                valid = all(np.all(
                    stage[center_y - radius_y:center_y + radius_y + 1,
                          center_x - radius_x:center_x + radius_x + 1][circle] >= 224
                ) for stage in stages)
                if valid:
                    safe.append(center_x / size)
            ranges = []
            if safe:
                start = previous = safe[0]
                step = 1 / size
                for value in safe[1:]:
                    if value - previous > step * 1.5:
                        ranges.append((round(start, 4), round(previous, 4)))
                        start = value
                    previous = value
                ranges.append((round(start, 4), round(previous, 4)))
            print(f"  y={normalized_y:.3f}: {ranges or None}")

        candidates = []
        for normalized_y in np.arange(.16, .801, .015):
            center_y = round((1.0 - normalized_y) * size)
            for normalized_x in np.arange(.26, .761, .015):
                center_x = round(normalized_x * size)
                valid = all(np.all(
                    stage[center_y - radius_y:center_y + radius_y + 1,
                          center_x - radius_x:center_x + radius_x + 1][circle] >= 224
                ) for stage in stages)
                if valid:
                    candidates.append((float(normalized_x), float(normalized_y)))

        required = {1: 7, 2: 11, 3: 7}[sector]
        rng = random.Random(3500 + sector)
        best = []
        for _ in range(6000):
            shuffled = candidates[:]
            rng.shuffle(shuffled)
            chosen = []
            for candidate in shuffled:
                if all(math.dist(candidate, other) >= .119 for other in chosen):
                    chosen.append(candidate)
            if len(chosen) > len(best):
                best = chosen
            if len(best) >= required:
                break
        if len(best) < required:
            raise RuntimeError(f"Sector {sector}: only {len(best)} safe centers")
        selected = sorted(best[:required], key=lambda point: (-point[1], point[0]))
        print("  PACKED:", [
            (round(point[0], 3), round(point[1], 3)) for point in selected
        ])


if __name__ == "__main__":
    main()

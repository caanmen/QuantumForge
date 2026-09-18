from __future__ import annotations

import json
import math
from pathlib import Path
from typing import Iterable

from PIL import Image, ImageDraw


ROOT = Path(__file__).resolve().parents[1]
ASSETS = ROOT / "Assets/Project/UI/Vertical/Machine/Monolith2D"
OUTPUT = ROOT / "Logs/VisualQA/MachineMonolith2D_V56_Internal"
TILE_SIZE = 627
ALPHA_LIMIT = 250
REFERENCE_ART_SIZE = (291.0, 516.0)
RENDERED_ART_SIZE = (405.0, 720.0)
SYMBOL_CLEAR_HALF = (16.0 / REFERENCE_ART_SIZE[0], 16.0 / REFERENCE_ART_SIZE[1])
ALPHA_CLEAR_HALF = (16.0 / RENDERED_ART_SIZE[0], 16.0 / RENDERED_ART_SIZE[1])
HIT_HALF = (40.0 / REFERENCE_ART_SIZE[0], 40.0 / REFERENCE_ART_SIZE[1])
PREFIXES = (3, 5, 7, 8)

ASSET_NAMES = {
    1: "monolith_sector_1_exposed_surface_progression_v35.png",
    2: "monolith_sector_2_exposed_surface_progression_v43.png",
    3: "monolith_sector_3_lower_right_progression_v55.png",
}

POLYGONS = {
    1: [(.340, .920), (.660, .920), (.645, .180), (.500, .070),
        (.260, .070)],
    2: [(.495, .930), (.620, .900), (.790, .070), (.621, .055),
        (.515, .215)],
    3: [(.470, .900), (.700, .900), (.860, .100), (.480, .100)],
}

# Candidata perceptual inicial. Este archivo es una herramienta interna: las
# posiciones activas siguen perteneciendo a MachineMonolith2DVisualUI.
LAYOUTS = {
    1: [(.595, .820), (.445, .460), (.470, .120), (.505, .670),
        (.560, .210), (.595, .550), (.400, .270), (.520, .350)],
    2: [(.580, .780), (.580, .460), (.710, .120), (.605, .640),
        (.670, .280), (.625, .550), (.610, .200), (.620, .370)],
    3: [(.610, .820), (.540, .470), (.720, .150), (.560, .660),
        (.640, .250), (.680, .560), (.740, .390), (.540, .360)],
}


def cross(a: tuple[float, float], b: tuple[float, float], c: tuple[float, float]) -> float:
    return (b[0] - a[0]) * (c[1] - a[1]) - (b[1] - a[1]) * (c[0] - a[0])


def inside_convex(point: tuple[float, float], polygon: list[tuple[float, float]]) -> bool:
    signs = []
    for index, current in enumerate(polygon):
        value = cross(current, polygon[(index + 1) % len(polygon)], point)
        if abs(value) > 1e-6:
            signs.append(value > 0)
    return bool(signs) and all(sign == signs[0] for sign in signs)


def footprint_inside_polygon(face: int, point: tuple[float, float]) -> bool:
    half_x, half_y = SYMBOL_CLEAR_HALF
    return all(
        inside_convex((point[0] + half_x * dx, point[1] + half_y * dy), POLYGONS[face])
        for dx in (-1, 0, 1)
        for dy in (-1, 0, 1)
    )


def tile_alpha(image: Image.Image, stage: int, u: float, v: float) -> int:
    tile_x = (stage % 2) * TILE_SIZE
    tile_y = (stage // 2) * TILE_SIZE
    x = max(0, min(TILE_SIZE - 1, round(u * (TILE_SIZE - 1))))
    y = max(0, min(TILE_SIZE - 1, round((1.0 - v) * (TILE_SIZE - 1))))
    return image.getpixel((tile_x + x, tile_y + y))[3]


def footprint_inside_alpha(image: Image.Image, stage: int,
                           point: tuple[float, float]) -> bool:
    half_x, half_y = ALPHA_CLEAR_HALF
    for row in range(7):
        for column in range(7):
            u = point[0] - half_x + 2.0 * half_x * column / 6.0
            v = point[1] - half_y + 2.0 * half_y * row / 6.0
            if tile_alpha(image, stage, u, v) < ALPHA_LIMIT:
                return False
    return True


def hitboxes_overlap(a: tuple[float, float], b: tuple[float, float]) -> bool:
    # The visible center of either node must never fall inside the other's
    # 80x80 target. Peripheral overlap remains allowed on these narrow faces.
    return (abs(a[0] - b[0]) < HIT_HALF[0] and
            abs(a[1] - b[1]) < HIT_HALF[1])


def polygon_area(points: Iterable[tuple[float, float]]) -> float:
    ordered = list(points)
    return abs(sum(
        ordered[index][0] * ordered[(index + 1) % len(ordered)][1]
        - ordered[(index + 1) % len(ordered)][0] * ordered[index][1]
        for index in range(len(ordered))
    )) * .5


def convex_hull(points: list[tuple[float, float]]) -> list[tuple[float, float]]:
    ordered = sorted(set(points))
    if len(ordered) <= 1:
        return ordered
    lower: list[tuple[float, float]] = []
    for point in ordered:
        while len(lower) >= 2 and cross(lower[-2], lower[-1], point) <= 0:
            lower.pop()
        lower.append(point)
    upper: list[tuple[float, float]] = []
    for point in reversed(ordered):
        while len(upper) >= 2 and cross(upper[-2], upper[-1], point) <= 0:
            upper.pop()
        upper.append(point)
    return lower[:-1] + upper[:-1]


def layout_metrics(face: int, stage: int, points: list[tuple[float, float]],
                   image: Image.Image) -> dict:
    valid = [footprint_inside_polygon(face, point) and
             footprint_inside_alpha(image, stage, point)
             for point in points]
    overlaps = []
    distances = []
    for left in range(len(points)):
        for right in range(left + 1, len(points)):
            distances.append(math.dist(points[left], points[right]))
            if hitboxes_overlap(points[left], points[right]):
                overlaps.append([left + 1, right + 1])

    polygon = POLYGONS[face]
    poly_x = [point[0] for point in polygon]
    poly_y = [point[1] for point in polygon]
    x_span = (max(point[0] for point in points) - min(point[0] for point in points)) / (max(poly_x) - min(poly_x))
    y_span = (max(point[1] for point in points) - min(point[1] for point in points)) / (max(poly_y) - min(poly_y))
    hull_ratio = polygon_area(convex_hull(points)) / polygon_area(polygon)
    return {
        "safe": all(valid),
        "invalid_points": [index + 1 for index, value in enumerate(valid) if not value],
        "hitbox_overlaps": overlaps,
        "min_center_distance": round(min(distances), 4) if distances else 0.0,
        "x_span_ratio": round(x_span, 4),
        "y_span_ratio": round(y_span, 4),
        "hull_area_ratio": round(hull_ratio, 4),
    }


def draw_overlay(face: int, image: Image.Image, stage: int,
                 points: list[tuple[float, float]]) -> None:
    tile_x = (stage % 2) * TILE_SIZE
    tile_y = (stage // 2) * TILE_SIZE
    tile = image.crop((tile_x, tile_y, tile_x + TILE_SIZE, tile_y + TILE_SIZE)).convert("RGBA")
    draw = ImageDraw.Draw(tile, "RGBA")
    polygon = [(round(x * TILE_SIZE), round((1.0 - y) * TILE_SIZE)) for x, y in POLYGONS[face]]
    draw.line(polygon + [polygon[0]], fill=(0, 220, 255, 210), width=3)
    count = PREFIXES[stage]
    for index, point in enumerate(points[:count]):
        cx = point[0] * TILE_SIZE
        cy = (1.0 - point[1]) * TILE_SIZE
        hit_x = HIT_HALF[0] * TILE_SIZE
        hit_y = HIT_HALF[1] * TILE_SIZE
        draw.rectangle((cx - hit_x, cy - hit_y, cx + hit_x, cy + hit_y),
                       outline=(255, 170, 0, 170), width=2)
        radius = 10
        draw.ellipse((cx - radius, cy - radius, cx + radius, cy + radius),
                     fill=(0, 235, 255, 230), outline=(255, 255, 255, 255), width=2)
        draw.text((cx + 12, cy - 12), str(index + 1), fill=(255, 255, 255, 255))
    path = OUTPUT / f"face_{face}_stage_{stage}_{(0, 20, 40, 60)[stage]}pct_overlay.png"
    tile.save(path)


def main() -> None:
    OUTPUT.mkdir(parents=True, exist_ok=True)
    report = {
        "reference_art_size": REFERENCE_ART_SIZE,
        "symbol_clear_half_normalized": SYMBOL_CLEAR_HALF,
        "hitbox_half_normalized": HIT_HALF,
        "faces": {},
    }
    failed = False
    for face in (1, 2, 3):
        image = Image.open(ASSETS / ASSET_NAMES[face]).convert("RGBA")
        points = LAYOUTS[face]
        stages = {}
        for stage, count in enumerate(PREFIXES):
            metrics = layout_metrics(face, stage, points[:count], image)
            stages[str((0, 20, 40, 60)[stage])] = metrics
            draw_overlay(face, image, stage, points)
            failed |= not metrics["safe"] or bool(metrics["hitbox_overlaps"])
        report["faces"][str(face)] = stages
    report_path = OUTPUT / "layout_metrics.json"
    report_path.write_text(json.dumps(report, indent=2), encoding="utf-8")
    print(json.dumps(report, indent=2))
    if failed:
        raise SystemExit("FAIL: hay huellas inseguras o centros táctiles ambiguos")
    print("PASS: huellas seguras y cero capturas ambiguas entre centros táctiles")


if __name__ == "__main__":
    main()

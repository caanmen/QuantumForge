"""Prepare centered transparent destination icons for Ancient Orbits."""

from __future__ import annotations

from collections import deque
from pathlib import Path

import numpy as np
from PIL import Image, ImageDraw, ImageFilter


PROJECT = Path(__file__).resolve().parents[1]
ASSET_DIR = PROJECT / "Assets/Project/UI/Dimension1/Generated/Candidates/AncientOrbits"
CANVAS = 512
TARGET_EXTENT = 404

NAMES = (
    "abandoned_ship",
    "orbital_ruin",
    "laboratory",
    "abandoned_station",
)


def largest_component(mask: np.ndarray) -> np.ndarray:
    height, width = mask.shape
    visited = np.zeros_like(mask, dtype=bool)
    best: list[tuple[int, int]] = []
    best_score = -1.0
    center_x = (width - 1) * 0.5
    center_y = (height - 1) * 0.5

    for y in range(height):
        for x in range(width):
            if not mask[y, x] or visited[y, x]:
                continue
            queue = deque([(x, y)])
            visited[y, x] = True
            pixels: list[tuple[int, int]] = []
            sx = sy = 0.0
            while queue:
                px, py = queue.popleft()
                pixels.append((px, py))
                sx += px
                sy += py
                for nx, ny in ((px - 1, py), (px + 1, py), (px, py - 1), (px, py + 1)):
                    if 0 <= nx < width and 0 <= ny < height and mask[ny, nx] and not visited[ny, nx]:
                        visited[ny, nx] = True
                        queue.append((nx, ny))
            count = len(pixels)
            cx = sx / count
            cy = sy / count
            distance = ((cx - center_x) ** 2 + (cy - center_y) ** 2) ** 0.5
            score = count - distance * 1.2
            if score > best_score:
                best_score = score
                best = pixels

    result = np.zeros_like(mask, dtype=np.uint8)
    for x, y in best:
        result[y, x] = 255
    return result


def make_alpha(rgb: np.ndarray) -> Image.Image:
    maximum = rgb.max(axis=2)
    minimum = rgb.min(axis=2)
    chroma = maximum - minimum
    corners = np.concatenate(
        (rgb[:32, :32].reshape(-1, 3), rgb[-32:, -32:].reshape(-1, 3)), axis=0
    )
    corner_brightness = float(corners.mean())

    if corner_brightness > 100.0:
        # ImageGen sometimes paints a neutral checker into RGB. Keep cyan/dark art and
        # reject only high-value neutral squares.
        darkness = np.clip((205.0 - minimum) / 42.0, 0.0, 1.0)
        saturation = np.clip((chroma - 4.0) / 34.0, 0.0, 1.0)
        alpha = np.maximum(darkness, saturation)
    else:
        # True black backdrop: preserve luminous blueprint pixels and their soft glow.
        alpha = np.clip((maximum - 5.0) / 34.0, 0.0, 1.0)

    hard = alpha > 0.14
    hard_image = Image.fromarray(np.uint8(hard) * 255, "L")
    hard_image = hard_image.filter(ImageFilter.MaxFilter(5)).filter(ImageFilter.MinFilter(3))
    component = largest_component(np.asarray(hard_image) > 0)
    component_image = Image.fromarray(component, "L").filter(ImageFilter.MaxFilter(9))
    component_mask = np.asarray(component_image, dtype=np.float32) / 255.0
    alpha *= component_mask
    alpha_image = Image.fromarray(np.uint8(np.clip(alpha * 255.0, 0, 255)), "L")
    return alpha_image.filter(ImageFilter.GaussianBlur(0.45))


def normalize(source: Image.Image) -> Image.Image:
    rgb = np.asarray(source.convert("RGB"), dtype=np.float32)
    alpha = make_alpha(rgb)
    bbox = alpha.point(lambda value: 255 if value > 18 else 0).getbbox()
    if bbox is None:
        raise RuntimeError("No foreground found")
    padding = 10
    x0 = max(0, bbox[0] - padding)
    y0 = max(0, bbox[1] - padding)
    x1 = min(source.width, bbox[2] + padding)
    y1 = min(source.height, bbox[3] + padding)

    rgba = source.convert("RGBA")
    rgba.putalpha(alpha)
    icon = rgba.crop((x0, y0, x1, y1))
    scale = min(TARGET_EXTENT / icon.width, TARGET_EXTENT / icon.height)
    icon = icon.resize(
        (max(1, round(icon.width * scale)), max(1, round(icon.height * scale))),
        Image.Resampling.LANCZOS,
    )
    canvas = Image.new("RGBA", (CANVAS, CANVAS), (0, 0, 0, 0))
    canvas.alpha_composite(icon, ((CANVAS - icon.width) // 2, (CANVAS - icon.height) // 2))
    return canvas


def main() -> None:
    contact = Image.new("RGBA", (CANVAS * len(NAMES), CANVAS), (4, 18, 27, 255))
    draw = ImageDraw.Draw(contact)
    for index, name in enumerate(NAMES):
        source_path = ASSET_DIR / f"d1_destination_{name}_v1.png"
        output_path = ASSET_DIR / f"d1_destination_{name}_v2.png"
        icon = normalize(Image.open(source_path))
        icon.save(output_path, optimize=True)
        contact.alpha_composite(icon, (index * CANVAS, 0))
        draw.text((index * CANVAS + 12, 12), name.upper(), fill=(0, 210, 255, 255))
        print(output_path.name)
    contact_path = ASSET_DIR / "d1_destinations_contact_v2.png"
    contact.save(contact_path, optimize=True)
    print(contact_path)


if __name__ == "__main__":
    main()

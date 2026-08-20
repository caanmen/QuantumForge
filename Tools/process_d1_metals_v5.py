"""Build consistently centered D1 metal sprites from approved ImageGen strips."""

from __future__ import annotations

from collections import deque
from pathlib import Path

import numpy as np
from PIL import Image, ImageDraw, ImageFilter


PROJECT = Path(__file__).resolve().parents[1]
ASSET_DIR = PROJECT / "Assets/Project/UI/Dimension1/Generated/MetalsInventory"
CANVAS = 512
TARGET_EXTENT = 390

STRIPS = (
    ("d1_metals_strip_basic_v1.png", ("iron", "copper", "aluminum", "titanium", "nickel")),
    ("d1_metals_strip_advanced_v1.png", ("cobalt", "lithium", "tungsten", "platinum", "iridium")),
)


def largest_center_component(mask: np.ndarray) -> np.ndarray:
    """Keep the main connected icon and discard isolated background sparkle."""
    height, width = mask.shape
    visited = np.zeros_like(mask, dtype=bool)
    best_pixels: list[tuple[int, int]] = []
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
            sx = 0.0
            sy = 0.0
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
            score = count - distance * 1.5
            if score > best_score:
                best_score = score
                best_pixels = pixels

    result = np.zeros_like(mask, dtype=np.uint8)
    for x, y in best_pixels:
        result[y, x] = 255
    return result


def extract_icon(cell: Image.Image) -> tuple[Image.Image, tuple[int, int, int, int]]:
    rgb = np.asarray(cell.convert("RGB"), dtype=np.float32)
    height, width, _ = rgb.shape

    # The generated backdrop is smooth. Estimate its left-to-right color per row
    # from untouched margins, then isolate only pixels that depart from it.
    margin = max(16, int(width * 0.11))
    left = np.median(rgb[:, :margin, :], axis=1)
    right = np.median(rgb[:, width - margin :, :], axis=1)
    mix = np.linspace(0.0, 1.0, width, dtype=np.float32)[None, :, None]
    background = left[:, None, :] * (1.0 - mix) + right[:, None, :] * mix
    delta = np.sqrt(np.sum((rgb - background) ** 2, axis=2))

    hard = delta > 19.0
    hard_img = Image.fromarray((hard.astype(np.uint8) * 255), "L")
    hard_img = hard_img.filter(ImageFilter.MaxFilter(5)).filter(ImageFilter.MinFilter(3))
    component = largest_center_component(np.asarray(hard_img) > 0)
    component_img = Image.fromarray(component, "L").filter(ImageFilter.MaxFilter(9))
    component_mask = np.asarray(component_img, dtype=np.float32) / 255.0

    # Soft alpha keeps the original antialiasing while removing the dark field.
    alpha = np.clip((delta - 7.0) / 22.0, 0.0, 1.0) * component_mask
    alpha_img = Image.fromarray(np.uint8(np.clip(alpha * 255.0, 0, 255)), "L")
    alpha_img = alpha_img.filter(ImageFilter.GaussianBlur(0.65))

    bbox = alpha_img.point(lambda value: 255 if value > 18 else 0).getbbox()
    if bbox is None:
        raise RuntimeError("No foreground detected")

    padding = 8
    x0 = max(0, bbox[0] - padding)
    y0 = max(0, bbox[1] - padding)
    x1 = min(width, bbox[2] + padding)
    y1 = min(height, bbox[3] + padding)
    rgba = cell.convert("RGBA")
    rgba.putalpha(alpha_img)
    return rgba.crop((x0, y0, x1, y1)), (x0, y0, x1, y1)


def normalize(icon: Image.Image) -> Image.Image:
    width, height = icon.size
    scale = min(TARGET_EXTENT / width, TARGET_EXTENT / height)
    resized = icon.resize(
        (max(1, round(width * scale)), max(1, round(height * scale))),
        Image.Resampling.LANCZOS,
    )
    canvas = Image.new("RGBA", (CANVAS, CANVAS), (0, 0, 0, 0))
    x = (CANVAS - resized.width) // 2
    y = (CANVAS - resized.height) // 2
    canvas.alpha_composite(resized, (x, y))
    return canvas


def main() -> None:
    contact = Image.new("RGBA", (CANVAS * 5, CANVAS * 2), (4, 18, 27, 255))
    draw = ImageDraw.Draw(contact)

    for row, (strip_name, names) in enumerate(STRIPS):
        strip = Image.open(ASSET_DIR / strip_name).convert("RGB")
        boundaries = [round(strip.width * index / 5) for index in range(6)]
        for column, name in enumerate(names):
            cell = strip.crop((boundaries[column], 0, boundaries[column + 1], strip.height))
            icon, source_bbox = extract_icon(cell)
            normalized = normalize(icon)
            output = ASSET_DIR / f"d1_metal_{name}_v5.png"
            normalized.save(output, optimize=True)
            contact.alpha_composite(normalized, (column * CANVAS, row * CANVAS))
            draw.text((column * CANVAS + 12, row * CANVAS + 12), name.upper(), fill=(0, 210, 255, 255))
            print(f"{name}: source_bbox={source_bbox}, normalized={normalized.size}, output={output.name}")

    contact_path = ASSET_DIR / "d1_metals_contact_v5.png"
    contact.save(contact_path, optimize=True)
    print(f"contact={contact_path}")


if __name__ == "__main__":
    main()

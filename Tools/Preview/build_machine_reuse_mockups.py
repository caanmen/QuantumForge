from pathlib import Path

from PIL import Image, ImageDraw, ImageEnhance, ImageFilter, ImageFont, ImageOps


ROOT = Path(__file__).resolve().parents[2]
ASSETS = ROOT / "Assets/Project/UI/Vertical/GenerationPolish"
UPGRADES = ROOT / "Assets/Project/UI/Vertical/UpgradesPolish"
OUTPUT = ROOT / "Logs/VisualQA/MachineReusePreviews"
FONT = ROOT / "Assets/Project/UI/Vertical/Fonts/Rajdhani-Medium.ttf"

W, H = 1080, 1920
CYAN = (0, 198, 239)
PURPLE = (185, 73, 228)
TEAL = (0, 203, 187)
WHITE = (213, 222, 226)
MUTED = (143, 158, 166)


def cover(image: Image.Image, size: tuple[int, int]) -> Image.Image:
    image = image.convert("RGB")
    scale = max(size[0] / image.width, size[1] / image.height)
    resized = image.resize((round(image.width * scale), round(image.height * scale)), Image.Resampling.LANCZOS)
    left = (resized.width - size[0]) // 2
    top = (resized.height - size[1]) // 2
    return resized.crop((left, top, left + size[0], top + size[1]))


def fit(image: Image.Image, size: tuple[int, int]) -> Image.Image:
    result = Image.new("RGBA", size, (0, 0, 0, 0))
    copy = image.convert("RGBA")
    copy.thumbnail(size, Image.Resampling.LANCZOS)
    result.alpha_composite(copy, ((size[0] - copy.width) // 2, (size[1] - copy.height) // 2))
    return result


def nine_slice(image: Image.Image, size: tuple[int, int], border: int = 22) -> Image.Image:
    src = image.convert("RGBA")
    sw, sh = src.size
    dw, dh = size
    b = min(border, sw // 3, sh // 3, dw // 3, dh // 3)
    out = Image.new("RGBA", size, (0, 0, 0, 0))
    xs = (0, b, sw - b, sw)
    ys = (0, b, sh - b, sh)
    dx = (0, b, dw - b, dw)
    dy = (0, b, dh - b, dh)
    for yi in range(3):
        for xi in range(3):
            tile = src.crop((xs[xi], ys[yi], xs[xi + 1], ys[yi + 1]))
            target = (dx[xi + 1] - dx[xi], dy[yi + 1] - dy[yi])
            if tile.size != target:
                tile = tile.resize(target, Image.Resampling.LANCZOS)
            out.alpha_composite(tile, (dx[xi], dy[yi]))
    return out


def font(size: int) -> ImageFont.FreeTypeFont:
    return ImageFont.truetype(str(FONT), size)


def centered_text(draw: ImageDraw.ImageDraw, box: tuple[int, int, int, int], text: str, size: int,
                  color: tuple[int, int, int], spacing: int = 0) -> None:
    fnt = font(size)
    x0, y0, x1, y1 = box
    if spacing <= 0:
        bounds = draw.textbbox((0, 0), text, font=fnt)
        x = (x0 + x1 - (bounds[2] - bounds[0])) / 2
        y = (y0 + y1 - (bounds[3] - bounds[1])) / 2 - bounds[1]
        draw.text((x, y), text, font=fnt, fill=color)
        return
    glyphs = [(ch, draw.textlength(ch, font=fnt)) for ch in text]
    width = sum(g[1] for g in glyphs) + spacing * max(0, len(glyphs) - 1)
    x = (x0 + x1 - width) / 2
    bounds = draw.textbbox((0, 0), text, font=fnt)
    y = (y0 + y1 - (bounds[3] - bounds[1])) / 2 - bounds[1]
    for ch, char_width in glyphs:
        draw.text((x, y), ch, font=fnt, fill=color)
        x += char_width + spacing


def tinted_frame(frame: Image.Image, size: tuple[int, int], accent: tuple[int, int, int], active: bool) -> Image.Image:
    base = nine_slice(frame, size, 22)
    if not active:
        return ImageEnhance.Brightness(base).enhance(0.72)
    alpha = base.getchannel("A")
    gray = ImageOps.grayscale(base)
    glow_alpha = gray.point(lambda value: min(210, max(0, (value - 50) * 2)))
    glow_alpha = ImageChops_multiply(glow_alpha, alpha)
    color = Image.new("RGBA", size, (*accent, 0))
    color.putalpha(glow_alpha)
    result = base.copy()
    result.alpha_composite(color)
    return result


def ImageChops_multiply(a: Image.Image, b: Image.Image) -> Image.Image:
    # Kept local so the preview tool remains a single small dependency.
    from PIL import ImageChops
    return ImageChops.multiply(a, b)


def resource_card(canvas: Image.Image, x: int, y: int, width: int, label: str, value: str,
                  icon_path: Path, accent: tuple[int, int, int], resource_bg: Image.Image) -> None:
    bg = cover(resource_bg, (width, 92)).convert("RGBA")
    bg = ImageEnhance.Brightness(bg).enhance(0.72)
    canvas.alpha_composite(bg, (x, y))
    draw = ImageDraw.Draw(canvas)
    icon = fit(Image.open(icon_path), (58, 58))
    canvas.alpha_composite(icon, (x + 22, y + 17))
    draw.text((x + 88, y + 15), label, font=font(24), fill=WHITE)
    draw.text((x + 88, y + 48), value, font=font(21), fill=accent)


def tab(canvas: Image.Image, x: int, y: int, width: int, label: str, icon_path: Path,
        accent: tuple[int, int, int], active: bool, selector_frame: Image.Image) -> None:
    plate = tinted_frame(selector_frame, (width, 70), accent, active)
    canvas.alpha_composite(plate, (x, y))
    draw = ImageDraw.Draw(canvas)
    icon = fit(Image.open(icon_path), (35, 35))
    if not active:
        icon = ImageEnhance.Brightness(icon).enhance(0.48)
    canvas.alpha_composite(icon, (x + 20, y + 18))
    centered_text(draw, (x + 52, y, x + width - 10, y + 70), label, 22, accent if active else MUTED, 1)


def body_overlay(source: Image.Image, crop: tuple[int, int, int, int], size: tuple[int, int]) -> Image.Image:
    content = source.crop(crop).resize(size, Image.Resampling.LANCZOS).convert("RGBA")
    lum = ImageOps.grayscale(content)
    alpha = lum.point(lambda value: min(255, 125 + value * 3))
    content.putalpha(alpha)
    return content


def build_preview(name: str, source_path: Path, active_tab: int, values: tuple[str, str], body_crop: tuple[int, int, int, int]) -> Path:
    source = Image.open(source_path).convert("RGB")
    lab = Image.open(ASSETS / "qf_lab_accident_background_v3.png")
    resource_bg = Image.open(ASSETS / "qf_resource_counter_metal_v2.png")
    selector_frame = Image.open(ASSETS / "qf_selector_frame.png")

    canvas = cover(lab, (W, H)).convert("RGBA")
    canvas = ImageEnhance.Brightness(canvas).enhance(0.39)
    canvas.alpha_composite(Image.new("RGBA", (W, H), (0, 9, 16, 82)))

    # Header uses the same metal resource plates and typography as Generation.
    resource_card(canvas, 68, 34, 455, "LE", values[0], ASSETS / "qf_resource_le.png", CYAN, resource_bg)
    resource_card(canvas, 557, 34, 455, "TRAZAS", values[1], ASSETS / "qf_resource_traces.png", PURPLE, resource_bg)

    title_plate = cover(resource_bg, (944, 88)).convert("RGBA")
    title_plate = ImageEnhance.Brightness(title_plate).enhance(0.64)
    canvas.alpha_composite(title_plate, (68, 132))
    draw = ImageDraw.Draw(canvas)
    centered_text(draw, (68, 132, 1012, 220), "MÁQUINA", 38, WHITE, 5)
    draw.line((158, 176, 330, 176), fill=(155, 182, 192), width=2)
    draw.line((750, 176, 922, 176), fill=(155, 182, 192), width=2)

    tab_specs = [
        ("NODOS", ASSETS / "qf_circuit_energy.png", CYAN),
        ("MEZCLAS", ASSETS / "qf_circuit_experimental.png", PURPLE),
        ("SEMILLAS", ASSETS / "qf_circuit_phase.png", TEAL),
    ]
    for idx, (label, icon_path, accent) in enumerate(tab_specs):
        tab(canvas, 81 + idx * 309, 232, 300, label, icon_path, accent, idx == active_tab, selector_frame)

    # The exact accident-lab background becomes the chamber. Current functional UI
    # is composited over it, so every control remains traceable to the Unity screen.
    chamber = cover(lab, (1030, 1306)).convert("RGBA")
    chamber = ImageEnhance.Brightness(chamber).enhance(0.57)
    canvas.alpha_composite(chamber, (25, 307))
    canvas.alpha_composite(Image.new("RGBA", (920, 1190), (0, 4, 10, 105)), (80, 352))
    body = body_overlay(source, body_crop, (900, 1198))
    canvas.alpha_composite(body, (90, 343))

    # Preserve the proven dimension selector and main navigation exactly as they
    # currently work; only their surrounding background inherits the lab texture.
    lower = source.crop((0, 1632, 1080, 1920)).resize((1080, 288), Image.Resampling.LANCZOS).convert("RGBA")
    lum = ImageOps.grayscale(lower)
    lower.putalpha(lum.point(lambda value: min(255, 150 + value * 3)))
    canvas.alpha_composite(lower, (0, 1632))

    # Subtle damaged-glass scratches, sampled from the same lab background.
    glass = cover(lab, (1030, 1306)).convert("RGBA")
    glass.putalpha(28)
    canvas.alpha_composite(glass, (25, 307))

    canvas = canvas.filter(ImageFilter.UnsharpMask(radius=1.1, percent=112, threshold=3))
    OUTPUT.mkdir(parents=True, exist_ok=True)
    path = OUTPUT / f"{name}_reuse_preview_1080x1920.png"
    canvas.convert("RGB").save(path, quality=96)
    return path


def main() -> None:
    previews = [
        (
            "01_cube",
            ROOT / "Logs/VisualQA/MachineBlock1/true3d_01_face_1_rest.png",
            0,
            ("1000000B", "1000B"),
            (44, 286, 1036, 1632),
        ),
        (
            "02_mixes",
            ROOT / "Logs/VisualQA/MachineFusion/fusion_panel_operational_1080x1920.png",
            1,
            ("1000000B", "1000B"),
            (44, 286, 1036, 1632),
        ),
        (
            "03_seeds",
            ROOT / "Logs/VisualQA/MachineSeeds/seeds_panel_operational_1080x1920.png",
            2,
            ("12.52K", "320.36"),
            (44, 286, 1036, 1632),
        ),
    ]
    for spec in previews:
        print(build_preview(*spec))


if __name__ == "__main__":
    main()

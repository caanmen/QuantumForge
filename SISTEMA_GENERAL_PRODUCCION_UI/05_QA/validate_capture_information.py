#!/usr/bin/env python3
"""Rechaza capturas vacías, planas o sin información visual suficiente."""

from __future__ import annotations

import argparse
import math
from pathlib import Path

from PIL import Image, ImageDraw, ImageStat


def metrics(image: Image.Image) -> dict[str, float]:
    rgb = image.convert("RGB")
    luminance = rgb.convert("L")
    histogram = luminance.histogram()
    total = max(1, sum(histogram))
    entropy = -sum((count / total) * math.log2(count / total) for count in histogram if count)
    stddev = ImageStat.Stat(luminance).stddev[0]
    colorful = sum(
        1 for red, green, blue in rgb.getdata()
        if max(red, green, blue) - min(red, green, blue) >= 12
    ) / total
    dominant = max(histogram) / total
    return {"entropy": entropy, "stddev": stddev, "color_fraction": colorful, "dominant": dominant}


def validate_image(image: Image.Image, min_width: int, min_height: int, min_entropy: float,
                   min_stddev: float, min_color_fraction: float) -> list[str]:
    failures: list[str] = []
    if image.width < min_width or image.height < min_height:
        failures.append(f"resolución {image.width}x{image.height} menor a {min_width}x{min_height}")
    values = metrics(image)
    if values["entropy"] < min_entropy:
        failures.append(f"entropía {values['entropy']:.3f} menor a {min_entropy:.3f}")
    if values["stddev"] < min_stddev:
        failures.append(f"desviación tonal {values['stddev']:.3f} menor a {min_stddev:.3f}")
    if values["color_fraction"] < min_color_fraction:
        failures.append(
            f"fracción de color {values['color_fraction']:.4f} menor a {min_color_fraction:.4f}"
        )
    if values["dominant"] > 0.98:
        failures.append(f"un solo nivel ocupa {values['dominant']:.2%} de la captura")
    return failures


def validate(path: Path, min_width: int, min_height: int, min_entropy: float,
             min_stddev: float, min_color_fraction: float) -> list[str]:
    with Image.open(path) as image:
        return validate_image(
            image, min_width, min_height, min_entropy, min_stddev, min_color_fraction,
        )


def self_test() -> int:
    flat = Image.new("RGB", (360, 640), (128, 128, 128))
    valid = Image.new("RGB", (360, 640), (12, 62, 45))
    draw = ImageDraw.Draw(valid)
    draw.rectangle((0, 0, 359, 80), fill=(18, 45, 72))
    draw.rectangle((0, 560, 359, 639), fill=(39, 28, 21))
    for index in range(7):
        x = 8 + index * 50
        draw.rectangle((x, 130 + index * 12, x + 38, 220 + index * 25), fill=(242, 235, 211))
        draw.rectangle((x + 5, 138 + index * 12, x + 15, 153 + index * 12), fill=(170, 22, 30))
    if not validate_image(flat, 320, 480, 1.5, 8.0, 0.01):
        print("CAPTURE VALIDATOR SELF-TEST FAIL - la fixture plana fue aceptada")
        return 1
    if validate_image(valid, 320, 480, 1.5, 8.0, 0.01):
        print("CAPTURE VALIDATOR SELF-TEST FAIL - la fixture válida fue rechazada")
        return 1
    print("CAPTURE VALIDATOR SELF-TEST PASS")
    return 0


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("image", nargs="?", type=Path)
    parser.add_argument("--min-width", type=int, default=320)
    parser.add_argument("--min-height", type=int, default=480)
    parser.add_argument("--min-entropy", type=float, default=1.5)
    parser.add_argument("--min-stddev", type=float, default=8.0)
    parser.add_argument("--min-color-fraction", type=float, default=0.01)
    parser.add_argument("--self-test", action="store_true")
    args = parser.parse_args()
    if args.self_test:
        return self_test()
    if args.image is None or not args.image.is_file():
        parser.error("debe indicar una captura existente")
    failures = validate(
        args.image, args.min_width, args.min_height, args.min_entropy,
        args.min_stddev, args.min_color_fraction,
    )
    if failures:
        print("CAPTURE INFORMATION FAIL")
        for failure in failures:
            print(f"- {failure}")
        return 1
    print("CAPTURE INFORMATION PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

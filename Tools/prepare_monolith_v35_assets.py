from pathlib import Path
import shutil

from PIL import Image


ROOT = Path(__file__).resolve().parents[1]
GENERATED = Path(
    r"C:\Users\nedfla\.codex\generated_images\01a04100-8369-7ee2-9641-7697fa95bb06"
)
RUNTIME = ROOT / "Assets/Project/UI/Vertical/Machine/Monolith2D"
REFERENCE = ROOT / (
    "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/"
    "MAQUINA/MONOLITO_2D_CORRECCIONES_INTERACCION_2026-08-26_V02/CANDIDATOS_V35"
)

BACKGROUND_SOURCE = GENERATED / "exec-87d997f9-3afe-4c2c-970f-2e82e90bf684.png"
BACKGROUND_NAME = "machine_destroyed_lab_monolith_fitted_cradle_square_v35.png"

FACE_SOURCES = {
    1: RUNTIME / "monolith_sector_1_open_surface_progression_v33.png",
    2: RUNTIME / "monolith_sector_2_open_surface_progression_v33.png",
    3: GENERATED / "exec-39f8cd00-5f2b-4d78-9860-a966f7dc87d6.png",
    4: GENERATED / "exec-f557e52e-f211-4972-9513-49390b8eee5c.png",
}


def remove_drawn_checkerboard(source: Path) -> Image.Image:
    image = Image.open(source).convert("RGBA")
    pixels = image.load()
    for y in range(image.height):
        for x in range(image.width):
            r, g, b, a = pixels[x, y]
            neutral = max(r, g, b) - min(r, g, b) < 34
            if neutral and min(r, g, b) > 174:
                pixels[x, y] = (0, 0, 0, 0)
            else:
                pixels[x, y] = (r, g, b, a if a < 255 else 255)
    return image


def normalize_quadrants(image: Image.Image, spine_on_right: bool) -> Image.Image:
    if image.size != (1254, 1254):
        raise RuntimeError(f"Unexpected face size: {image.size}")
    size = 627
    quadrants = []
    boxes = []
    for row in range(2):
        for col in range(2):
            quadrant = image.crop(
                (col * size, row * size, (col + 1) * size, (row + 1) * size)
            )
            bbox = quadrant.getchannel("A").getbbox()
            if bbox is None:
                raise RuntimeError(f"Empty quadrant: {col}, {row}")
            quadrants.append(quadrant)
            boxes.append(bbox)

    base = boxes[0]
    base_spine = base[2] if spine_on_right else base[0]
    base_floor = base[3]
    output = Image.new("RGBA", image.size, (0, 0, 0, 0))
    for index, (quadrant, bbox) in enumerate(zip(quadrants, boxes)):
        spine = bbox[2] if spine_on_right else bbox[0]
        dx = base_spine - spine
        dy = base_floor - bbox[3]
        aligned = Image.new("RGBA", (size, size), (0, 0, 0, 0))
        aligned.alpha_composite(quadrant, (dx, dy))
        col = index % 2
        row = index // 2
        output.alpha_composite(aligned, (col * size, row * size))
        print(f"quadrant={index} bbox={bbox} shift=({dx},{dy})")
    return output


def verify_face(path: Path) -> None:
    image = Image.open(path)
    if image.mode != "RGBA" or image.size != (1254, 1254):
        raise RuntimeError(f"Invalid face sheet: {path} {image.mode} {image.size}")
    alpha = image.getchannel("A")
    if alpha.getextrema() != (0, 255):
        raise RuntimeError(f"Face sheet has no useful alpha: {path}")
    for row in range(2):
        for col in range(2):
            quadrant = alpha.crop(
                (col * 627, row * 627, (col + 1) * 627, (row + 1) * 627)
            )
            bbox = quadrant.getbbox()
            if bbox is None:
                raise RuntimeError(f"Empty quadrant in {path}: {col}, {row}")
            if bbox[2] - bbox[0] < 280 or bbox[3] - bbox[1] < 480:
                raise RuntimeError(f"Face quadrant too small in {path}: {col}, {row}, {bbox}")


def save_to_both(image: Image.Image, name: str) -> None:
    for directory in (RUNTIME, REFERENCE):
        directory.mkdir(parents=True, exist_ok=True)
        destination = directory / name
        image.save(destination)
        print(destination)


def main() -> None:
    background = Image.open(BACKGROUND_SOURCE).convert("RGB")
    if background.size != (1254, 1254):
        raise RuntimeError(f"Invalid background size: {background.size}")
    for directory in (RUNTIME, REFERENCE):
        directory.mkdir(parents=True, exist_ok=True)
        destination = directory / BACKGROUND_NAME
        shutil.copy2(BACKGROUND_SOURCE, destination)
        print(destination)

    for sector, source in FACE_SOURCES.items():
        image = Image.open(source).convert("RGBA")
        if image.getchannel("A").getextrema() == (255, 255):
            image = remove_drawn_checkerboard(source)
        image = normalize_quadrants(image, spine_on_right=sector in (1, 4))
        name = f"monolith_sector_{sector}_exposed_surface_progression_v35.png"
        runtime_target = RUNTIME / name
        save_to_both(image, name)
        verify_face(runtime_target)


if __name__ == "__main__":
    main()

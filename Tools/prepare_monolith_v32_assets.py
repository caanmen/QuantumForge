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
    "MAQUINA/MONOLITO_2D_CORRECCIONES_INTERACCION_2026-08-26_V02/CANDIDATOS_V32"
)

BACKGROUND_SOURCE = GENERATED / "exec-d7361368-7f91-4e89-b230-60902e8d8d6c.png"
BACKGROUND_NAME = "machine_destroyed_lab_exact_footprint_cradle_square_v32.png"

FACE_SOURCES = {
    1: GENERATED / "exec-f8fd0bd1-80c5-4aae-919f-023d4c70909a.png",
    2: GENERATED / "exec-82a84a4b-4c9a-4afb-b15b-a5d981b7bd22.png",
    3: GENERATED / "exec-e3b6e2e7-3d10-4a00-b41d-cccb82616b78.png",
    4: GENERATED / "exec-372903d5-a861-4d54-bc64-018172fab665.png",
}


def remove_drawn_checkerboard(source: Path, target: Path) -> None:
    image = Image.open(source).convert("RGBA")
    pixels = image.load()
    for y in range(image.height):
        for x in range(image.width):
            r, g, b, _ = pixels[x, y]
            if min(r, g, b) > 175 and max(r, g, b) - min(r, g, b) < 40:
                pixels[x, y] = (0, 0, 0, 0)
            else:
                pixels[x, y] = (r, g, b, 255)
    target.parent.mkdir(parents=True, exist_ok=True)
    image.save(target)


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
            occupied_width = bbox[2] - bbox[0]
            occupied_height = bbox[3] - bbox[1]
            if occupied_width < 240 or occupied_height < 430:
                raise RuntimeError(
                    f"Face quadrant too small in {path}: {col}, {row}, {bbox}"
                )


def main() -> None:
    REFERENCE.mkdir(parents=True, exist_ok=True)
    RUNTIME.mkdir(parents=True, exist_ok=True)

    background = Image.open(BACKGROUND_SOURCE)
    if background.size != (1254, 1254):
        raise RuntimeError(f"Invalid background size: {background.size}")
    for destination in (RUNTIME / BACKGROUND_NAME, REFERENCE / BACKGROUND_NAME):
        shutil.copy2(BACKGROUND_SOURCE, destination)
        print(destination)

    for sector, source in FACE_SOURCES.items():
        name = f"monolith_sector_{sector}_perimeter_damage_progression_v32.png"
        runtime_target = RUNTIME / name
        reference_target = REFERENCE / name
        remove_drawn_checkerboard(source, runtime_target)
        shutil.copy2(runtime_target, reference_target)
        verify_face(runtime_target)
        print(runtime_target)
        print(reference_target)


if __name__ == "__main__":
    main()

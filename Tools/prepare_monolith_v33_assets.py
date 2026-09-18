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
    "MAQUINA/MONOLITO_2D_CORRECCIONES_INTERACCION_2026-08-26_V02/CANDIDATOS_V33"
)

BACKGROUND_SOURCE = GENERATED / "exec-2206ac1b-7c59-416e-8188-74c298d164a0.png"
BACKGROUND_NAME = "machine_destroyed_lab_monolith_socket_square_v33.png"

FACE_SOURCES = {
    1: GENERATED / "exec-9b159bd0-672e-47ec-a943-419a85443a71.png",
    3: GENERATED / "exec-90b74cd8-f74a-4f41-9c82-aa3276d1d1c3.png",
    4: GENERATED / "exec-acc01909-7332-41c5-a288-f579b26c6eee.png",
}


def remove_drawn_checkerboard(source: Path, target: Path) -> None:
    image = Image.open(source).convert("RGBA")
    pixels = image.load()
    for y in range(image.height):
        for x in range(image.width):
            r, g, b, _ = pixels[x, y]
            neutral = max(r, g, b) - min(r, g, b) < 34
            if neutral and min(r, g, b) > 174:
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
            if occupied_width < 280 or occupied_height < 480:
                raise RuntimeError(
                    f"Face quadrant too small in {path}: {col}, {row}, {bbox}"
                )


def copy_to_both(source: Path, name: str) -> None:
    for directory in (RUNTIME, REFERENCE):
        directory.mkdir(parents=True, exist_ok=True)
        destination = directory / name
        shutil.copy2(source, destination)
        print(destination)


def main() -> None:
    background = Image.open(BACKGROUND_SOURCE)
    if background.size != (1254, 1254):
        raise RuntimeError(f"Invalid background size: {background.size}")
    copy_to_both(BACKGROUND_SOURCE, BACKGROUND_NAME)

    for sector, source in FACE_SOURCES.items():
        name = f"monolith_sector_{sector}_open_surface_progression_v33.png"
        runtime_target = RUNTIME / name
        remove_drawn_checkerboard(source, runtime_target)
        verify_face(runtime_target)
        REFERENCE.mkdir(parents=True, exist_ok=True)
        shutil.copy2(runtime_target, REFERENCE / name)
        print(runtime_target)
        print(REFERENCE / name)

    sector2_source = RUNTIME / "monolith_sector_2_perimeter_damage_progression_v32.png"
    sector2_name = "monolith_sector_2_open_surface_progression_v33.png"
    verify_face(sector2_source)
    copy_to_both(sector2_source, sector2_name)


if __name__ == "__main__":
    main()

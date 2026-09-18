from pathlib import Path
import shutil

from PIL import Image


ROOT = Path(__file__).resolve().parents[1]
GENERATED = Path(r"C:\Users\nedfla\.codex\generated_images\01a04100-8369-7ee2-9641-7697fa95bb06")
RUNTIME = ROOT / "Assets/Project/UI/Vertical/Machine/Monolith2D"
REFERENCE = ROOT / (
    "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/"
    "MAQUINA/MONOLITO_2D_CORRECCIONES_INTERACCION_2026-08-26_V02/CANDIDATOS_V31"
)

BACKGROUND_SOURCE = GENERATED / "exec-7978b09e-ab24-4bb5-a048-1ac1d94d7405.png"
BACKGROUND_NAME = "machine_destroyed_lab_monolith_fit_square_v31.png"

FACE_SOURCES = {
    1: GENERATED / "exec-e74cb492-42d4-48be-b314-f05b1e476473.png",
    2: GENERATED / "exec-34040b9c-4bbf-48ff-b255-1d3950122423.png",
    3: GENERATED / "exec-51b87960-bf9f-43a6-918b-a59793a76039.png",
    4: GENERATED / "exec-97e9167a-eec4-4ecd-a713-6fe35f8cce95.png",
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
            quadrant = alpha.crop((col * 627, row * 627, (col + 1) * 627, (row + 1) * 627))
            if quadrant.getbbox() is None:
                raise RuntimeError(f"Empty quadrant in {path}: {col}, {row}")


def main() -> None:
    REFERENCE.mkdir(parents=True, exist_ok=True)
    RUNTIME.mkdir(parents=True, exist_ok=True)

    for destination in (RUNTIME / BACKGROUND_NAME, REFERENCE / BACKGROUND_NAME):
        shutil.copy2(BACKGROUND_SOURCE, destination)
        print(destination)

    for sector, source in FACE_SOURCES.items():
        name = f"monolith_sector_{sector}_surface_progression_v31.png"
        runtime_target = RUNTIME / name
        reference_target = REFERENCE / name
        remove_drawn_checkerboard(source, runtime_target)
        shutil.copy2(runtime_target, reference_target)
        verify_face(runtime_target)
        print(runtime_target)
        print(reference_target)


if __name__ == "__main__":
    main()

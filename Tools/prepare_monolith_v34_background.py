from pathlib import Path
import shutil

from PIL import Image


ROOT = Path(__file__).resolve().parents[1]
SOURCE = Path(
    r"C:\Users\nedfla\.codex\generated_images\01a04100-8369-7ee2-9641-7697fa95bb06\exec-b7bd72d9-0c85-4b4b-b8bc-c8af90f85679.png"
)
NAME = "machine_destroyed_lab_monolith_wide_socket_square_v34.png"
RUNTIME = ROOT / "Assets/Project/UI/Vertical/Machine/Monolith2D" / NAME
REFERENCE = ROOT / (
    "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/"
    "MAQUINA/MONOLITO_2D_CORRECCIONES_INTERACCION_2026-08-26_V02/"
    "CANDIDATOS_V34"
) / NAME


def main() -> None:
    image = Image.open(SOURCE)
    if image.size != (1254, 1254):
        raise RuntimeError(f"Invalid background size: {image.size}")
    for target in (RUNTIME, REFERENCE):
        target.parent.mkdir(parents=True, exist_ok=True)
        shutil.copy2(SOURCE, target)
        print(target)


if __name__ == "__main__":
    main()

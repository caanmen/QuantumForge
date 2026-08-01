# Quantum Forge — continuidad de Máquina, Bloque 1

Fecha: 2026-07-31  
Alcance cerrado: cubo y cuatro caras de la Máquina. El Bloque 2 de dimensiones quedó fuera de este trabajo.

## Estado final

Bloque 1 implementado e integrado en `Assets/Project/Scenes/Main.unity`. No se modificaron prefabs compartidos. La ejecución final de Unity confirmó:

- Configuración: `4 faces | swipe | native node card`.
- Validación: `55 public | 8 secret | 44/55 threshold | exact requirements | 4 functional faces`.
- Captura: cuatro estados iniciales y cuatro reparados a 1080×1920; guardado real restaurado.
- Compilación: 0 errores. Permanecen 26 advertencias preexistentes, principalmente campos obsoletos de prestigio; ninguna procede del cubo nuevo.

## Contrato funcional

- 55 nodos públicos y 8 secretos.
- Meta de convergencia: 80% de los públicos, equivalente a 44/55.
- Los secretos no alteran el progreso global ni el progreso de una cara.
- Los requisitos se validan por ID exacto; explorar un nodo bloqueado no permite comprarlo.
- Un tier reparado no resuelve el daño de otro tier.
- No se puede analizar o reparar con la Máquina bloqueada, en una cara inaccesible o sobre un secreto no revelado.
- La cara seleccionada se persiste en `SaveData` como índice validado 0–3, con guardado diferido para evitar escrituras por cada gesto.
- El mapa permite seleccionar todos los nodos públicos y agrupa tiers en sockets estables.
- Reparar un tier selecciona el siguiente tier pendiente de la rama.

| Cara | Nodos públicos runtime | Sockets públicos agrupados | Sockets con secretos |
|---|---:|---:|---:|
| I | 13 | 7 | 9 |
| II | 17 | 11 | 13 |
| III | 7 | 7 | 9 |
| IV | 18 | 10 | 12 |

## Implementación

Runtime nuevo:

- `Assets/Project/Scripts/UI/MachineCubeVisualUI.cs`
- `Assets/Project/Scripts/UI/MachineCubeFaceViewUI.cs`
- `Assets/Project/Scripts/UI/MachineCubeNodeVisualUI.cs`
- `Assets/Project/Scripts/UI/MachineCubeSwipeSurface.cs`
- `Assets/Project/Scripts/UI/MachineCubeCircuitLineUI.cs`
- `Assets/Project/Scripts/UI/MachineCubeSideGraphic.cs`

Integración funcional:

- `Assets/Project/Scripts/Systems/MachineManager.cs`
- `Assets/Project/Scripts/Systems/SaveService.cs`
- `Assets/Project/Scripts/UI/MachinePanelUI.cs`

Automatización reservada al agente principal:

- `Assets/Project/Scripts/Editor/MachineCubeBlock1Setup.cs`
- `Assets/Project/Scripts/Editor/MachineCubeBlock1Validation.cs`
- `Assets/Project/Scripts/Editor/MachineCubeBlock1Capture.cs`

## Diseño visual

- La referencia aprobada se preservó en `Assets/Project/UI/Vertical/References/Machine_Block1_Approved_Reference.png`.
- Las cuatro bases están en `Assets/Project/UI/Vertical/Machine/machine_face_1_base.png` a `machine_face_4_base.png`.
- Las bases se crearon con ImageGen integrado: placa industrial casi negra, metal dañado, circuitos incrustados y acentos cian, violeta, ámbar y turquesa, sin texto ni estados funcionales quemados en la textura.
- La cabecera, los marcos, la tipografía Rajdhani, los iconos de recursos y los pictogramas reutilizan el lenguaje ya implementado en Triángulo/Mejoras.
- Las abreviaturas quedaron solo como apoyo; los sockets y la tarjeta usan pictogramas detallados existentes.
- La cara frontal está desplazada y comprimida hacia la izquierda; un plano lateral trapezoidal conectado refuerza el giro hacia la derecha.
- Los circuitos dinámicos provienen únicamente de dependencias reales, se recortan dentro de la cara y ganan intensidad con la reparación.
- La tarjeta nativa muestra nombre, estado, descripción, efecto, coste, requisitos y acciones contextuales; `ANALIZAR` se oculta cuando ya no corresponde.

## Validación y evidencia

- Log final de configuración/validación/captura completa: `Logs/machine_cube_block1_capture_final.log`.
- Recapturas limpias aisladas para las caras 2, 3 y 4: `Logs/machine_cube_block1_capture_face2_initial_clean.log`, `Logs/machine_cube_block1_capture_face3_clean.log`, `Logs/machine_cube_block1_capture_face4_initial_clean.log`, `Logs/machine_cube_block1_capture_face2_repaired_clean.log` y `Logs/machine_cube_block1_capture_face4_repaired_clean.log`.
- Ocho PNG finales: `Logs/VisualQA/MachineBlock1/`.
- Comparación realizada contra la referencia aprobada y `Logs/VisualQA/VerticalUIBlock7/08_upgrades_polished_es_1080x1920.png`.
- Auditoría funcional, análisis visual y QA ejecutados por agentes auxiliares de solo lectura.
- QA final aprobado sobre las ocho capturas actuales, sin bloqueadores críticos ni altos.
- No quedaron `NullReferenceException`, `MissingReferenceException`, errores de compilación ni glifos faltantes en las pasadas finales.

## Seguridad y continuidad

- El configurador reemplaza únicamente `MachineCubeVisualRoot`; no reconstruye `MachinePanelRoot`.
- Los elementos legacy que se superponían al cubo se desactivan por nombre exacto; las vistas auxiliares funcionales se conservan.
- La captura respalda `save.json` y `save.json.bak`, restaura ambos al salir y recupera respaldos de una sesión interrumpida.
- El proyecto contiene muchos cambios previos ajenos a este bloque. No deben revertirse, limpiarse ni sobrescribirse.

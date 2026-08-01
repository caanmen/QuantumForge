# Continuidad — rediseño visual de Máquina según referencia

Fecha: 2026-07-31

## Objetivo vigente

Corregir el Bloque 1 de la Máquina para que se apegue al diseño aprobado mostrado en:

- `C:\Users\nedfla\AppData\Local\Temp\codex-clipboard-1a2a5bb5-59ae-4beb-9dea-5a906011ba86.png`

La captura que motivó la corrección es:

- `C:\Users\nedfla\AppData\Local\Temp\codex-clipboard-675ca077-b15a-48f7-b5f7-78cc49a94d57.png`

No abordar el Bloque 2 dimensional.

## Reglas de coordinación

- El agente principal es el único que modifica `Main.unity`, integra y ejecuta Unity.
- Los agentes auxiliares son de solo lectura.
- Nunca ejecutar dos instancias de Unity simultáneamente.
- Preservar todos los cambios preexistentes del proyecto.
- Comparar capturas finales lado a lado con la referencia.

## Bloques

1. Bases 2.5D y eliminación de conexiones tipo árbol: completado en código/arte.
2. Composición de cabecera, cara, nodos, giro y tarjeta: parcialmente completado; falta compilar e inspeccionar.
3. Integración en `Main.unity`: pendiente.
4. Capturas y QA final: pendiente.

## Arte nuevo ya guardado

Se utilizó ImageGen integrado tomando el diseño aprobado como autoridad visual. Las cuatro caras nuevas están en:

- `Assets/Project/UI/Vertical/Machine/machine_face_1_reference_v2.png`
- `Assets/Project/UI/Vertical/Machine/machine_face_2_reference_v2.png`
- `Assets/Project/UI/Vertical/Machine/machine_face_3_reference_v2.png`
- `Assets/Project/UI/Vertical/Machine/machine_face_4_reference_v2.png`

Dirección del prompt: cubo industrial metálico 2.5D girado hacia la derecha, laterales visibles, módulos físicos, canaletas incrustadas, metal legible, acentos mínimos por cara y sin texto, iconos, navegación ni líneas de árbol.

No borrar ni sobrescribir las bases antiguas `machine_face_*_base.png`.

## Cambios de código ya aplicados

### `MachineCubeFaceViewUI.cs`

- Se retiró la generación de conexiones Manhattan entre nodos.
- Las dependencias funcionales siguen intactas en `MachineManager` y en la tarjeta.
- Brillo mínimo de la base subió de `0.38` a `0.72` para recuperar medios tonos.

### `MachineCubeNodeVisualUI.cs`

- El hit target ya no debe verse como hexágono HUD.
- La retícula aparece solo en selección/análisis.
- El estado se comunica con pictograma y marca pequeña, sin bordes saturados dominantes.

### `MachineCubeVisualUI.cs`

- Se agregaron `faceDots`.
- Se agregó `selectedFaceProgressText`.
- `RefreshHeader()` actualiza los cuatro indicadores y el progreso del sector en la tarjeta.

### `MachineCubeBlock1Setup.cs`

- Usa las texturas `*_reference_v2.png`.
- Amplía `MachinePanelRoot` mediante `sizeDelta = (-16, -20)`.
- Separa recursos y placa `MÁQUINA`.
- Agranda el viewport y usa `AspectRatioFitter.EnvelopeParent`.
- Elimina la franja lateral cian artificial; la profundidad ya está pintada en el arte.
- Añade arco, texto `DESLIZA PARA ROTAR` y cuatro indicadores.
- Usa flechas laterales sin cajas visibles.
- Alinea posiciones de nodos con los módulos de las nuevas caras.
- Comenzó a ensanchar y reducir la altura de la tarjeta.
- Eliminó los botones anterior/siguiente de nodo de la tarjeta.

## Estado exacto al detenerse

Unity está cerrado.

Se intentó ejecutar:

`MachineCubeBlock1Setup.ConfigureBlock1CubeBatch`

El primer intento se detuvo antes de integrar porque `FontStyles.Medium` no existe. Ya se corrigió a `FontStyles.Normal` en `MachineCubeBlock1Setup.cs`, pero todavía NO se ha repetido Unity batch después de esa corrección.

Por lo tanto:

- Los cambios recientes de layout aún no están integrados en `Main.unity`.
- Los scripts deben considerarse no validados hasta repetir compilación.
- No abrir Unity manualmente antes de completar el setup batch.

Log del intento fallido:

- `Logs/machine_cube_reference_redesign_setup.log`

## Próximo paso exacto

1. Confirmar que no exista proceso `Unity`.
2. Revisar estáticamente `MachineCubeBlock1Setup.cs` para referencias incompletas, en especial:
   - `BuildRotationHint`.
   - `CardParts.faceProgress`.
   - `faceDots` y `selectedFaceProgressText`.
   - ausencia de `previousNode` y `nextNode`.
3. Ejecutar Unity batch de forma exclusiva:

`MachineCubeBlock1Setup.ConfigureBlock1CubeBatch`

4. Corregir cualquier error de compilación antes de continuar.
5. Ejecutar `MachineCubeBlock1Validation.RunBatch`.
6. Generar primero una captura aislada de Cara 2 reparada, porque es el estado comparable con la referencia.
7. Comparar proporciones y visualmente antes de generar las ocho capturas.

## Criterios visuales obligatorios

- Cara ocupa aproximadamente 87–90% del ancho útil.
- Metal y medios tonos legibles; no aplastar la textura a negro.
- Módulos físicos integrados; no hexágonos HUD flotantes.
- Sin líneas rectas conectando nodos.
- Circuitos como canaletas/tuberías incrustadas en el arte.
- Cabecera separada: recursos, placa `MÁQUINA`, sector/cara.
- Flechas luminosas sin cajas.
- Arco de giro, `DESLIZA PARA ROTAR` y cuatro puntos.
- Tarjeta 94–96% del ancho, más baja y sin flechas pequeñas de nodo.
- Coherencia con la UI de Triángulo y Mejoras sin sacrificar la referencia industrial.

## Hallazgos funcionales del agente auditor

Ocultar las conexiones dibujadas es seguro: no altera selección, requisitos, reparación, guardado ni navegación. Las líneas eran una representación parcial y no autoritativa de `requiredNodeIds`. No tocar `MachineManager`, `SaveService` ni `machine_nodes.json` para esta corrección.

## QA pendiente

Después de la primera captura válida, reactivar al agente `qa_validacion` en solo lectura para revisar:

- Fidelidad lado a lado.
- Integridad de referencias/listeners.
- Selección de nodos y tiers.
- Flechas y swipe.
- Persistencia de cara.
- Ausencia de `MachineCubeCircuitLineUI` en runtime.
- Rendimiento móvil.

## Actualización de cierre técnico — 2026-07-31

El Bloque 1 del rediseño de Máquina quedó compilado, integrado y validado. Sigue fuera de alcance el Bloque 2 dimensional.

- `MachineCubeBlock1Setup.ConfigureBlock1CubeBatch` compiló e integró el layout final en `Assets/Project/Scenes/Main.unity`.
- `MachineCubeBlock1Validation.ValidateBatch` pasa con 55 nodos públicos, 8 secretos, umbral 44/55, requisitos exactos y cuatro caras funcionales.
- El validador ahora exige cuatro indicadores, progreso de sector, flechas de nodo ausentes, orden único de zonas y cero instancias de `MachineCubeCircuitLineUI`.
- Se separaron coste y progreso dentro de la tarjeta; se reforzaron las flechas laterales y se redujo la dominancia de la retícula.
- La guía nodo–tarjeta ahora espera el layout final del `AspectRatioFitter`, por lo que queda alineada en las cuatro caras tanto en runtime como en batch.
- La herramienta de captura restaura también la ausencia original de `save.json`/`.bak` tras un cierre abrupto y no marca salidas incompletas como `PASS`.
- Se generaron y revisaron 8 capturas finales de 1080x1920 en `Logs/VisualQA/MachineBlock1`: cuatro iniciales y cuatro reparadas.
- Log final: `Logs/machine_cube_reference_full_capture_final_v4.log` con `PASS | 4 initial + 4 repaired | 1080x1920 | save restored`.

Estado: código, conexiones, compilación, validación funcional y QA visual automatizado terminados. Falta solamente la aprobación visual del usuario comparando la Cara 2 reparada con la referencia aprobada. No usar Git salvo petición explícita.

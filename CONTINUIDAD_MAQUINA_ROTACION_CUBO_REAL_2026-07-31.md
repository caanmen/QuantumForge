# Continuidad — Máquina con rotación física de cubo y estados luminosos

Fecha: 2026-07-31

## Instrucción para iniciar el siguiente chat

Continúa el rediseño de la Máquina desde este archivo. Lee primero todo el resumen y revisa los archivos reales antes de modificar. Trabaja únicamente en el Bloque 1 de la Máquina; no abordar el Bloque 2 dimensional. El objetivo nuevo es que el cambio de cara se perciba como la rotación de un cubo físico continuo y no como el cambio de imágenes de un navegador. Añade también vida visual mediante iluminación animada que comunique daño, posibilidad de reparación y reparación completada, sin alterar la lógica funcional existente.

## Nueva decisión del usuario

El diseño estático actual de la Máquina se considera correcto, pero el cambio entre caras no está aprobado visualmente.

Problema percibido:

- Al cambiar de cara parece que el juego sustituye una fotografía por otra.
- La transición actual se siente como navegar entre páginas o imágenes.
- Aunque existe movimiento básico en código, no transmite que las cuatro caras pertenezcan al mismo volumen.
- Las caras se perciben demasiado estáticas, como fotografías sin vida mecánica.

Resultado solicitado:

- La Máquina debe sentirse como un cubo físico 2.5D/3D que rota alrededor de su eje vertical.
- La cara saliente debe girar y convertirse visualmente en el lateral del cubo.
- La cara entrante debe aparecer desde el lateral y terminar como cara frontal.
- Debe conservarse continuidad en aristas, profundidad, perspectiva, iluminación y sentido del giro.
- No debe existir un corte ni un simple intercambio de sprites al terminar la animación.
- Swipe y flechas deben usar exactamente la misma transición física.
- El cubo debe tener iluminación animada sutil para dejar de parecer una fotografía.
- Deben existir señales luminosas para distinguir al menos:
  - módulo dañado y todavía reparable;
  - módulo bloqueado o no disponible;
  - módulo reparado/operativo;
  - nodo seleccionado o en análisis.
- Las luces deben integrarse en módulos, canaletas y bordes industriales; no deben devolver el aspecto de hexágonos HUD flotantes ni recrear el árbol de conexiones.
- El movimiento y las luces deben ser legibles en móvil y no introducir una carga gráfica excesiva.

La paleta exacta, intensidad, ritmo del pulso y cantidad de luces todavía deben derivarse del diseño actual y presentarse al usuario si admiten varias interpretaciones materiales. No inventar un lenguaje visual que contradiga la referencia aprobada.

## Estado técnico actual

El Bloque 1 está completo y validado en su estado estático actual:

- Cuatro caras funcionales de la Máquina.
- 55 nodos públicos y 8 secretos.
- Umbral de convergencia: 44/55 nodos públicos, equivalente a 80%.
- Requisitos por ID exacto.
- Selección, tiers, reparación, análisis y persistencia de cara funcionan.
- Swipe y flechas cambian de cara.
- Cabecera, recursos, sector/cara, arco de giro, cuatro indicadores y tarjeta están integrados.
- Tarjeta sin flechas pequeñas de nodo.
- Coste y progreso del sector no se superponen.
- No existen conexiones Manhattan ni instancias runtime de `MachineCubeCircuitLineUI`.
- La guía nodo–tarjeta espera la resolución del `AspectRatioFitter` y queda alineada en las cuatro caras.
- La captura protege y restaura `save.json` y `save.json.bak`, incluso cuando originalmente no existían.

Integración final actual:

- Escena: `Assets/Project/Scenes/Main.unity`.
- Setup: `Assets/Project/Scripts/Editor/MachineCubeBlock1Setup.cs`.
- Validación: `Assets/Project/Scripts/Editor/MachineCubeBlock1Validation.cs`.
- Captura: `Assets/Project/Scripts/Editor/MachineCubeBlock1Capture.cs`.
- Control visual: `Assets/Project/Scripts/UI/MachineCubeVisualUI.cs`.
- Cara: `Assets/Project/Scripts/UI/MachineCubeFaceViewUI.cs`.
- Nodo: `Assets/Project/Scripts/UI/MachineCubeNodeVisualUI.cs`.
- Swipe: `Assets/Project/Scripts/UI/MachineCubeSwipeSurface.cs`.
- Panel funcional: `Assets/Project/Scripts/UI/MachinePanelUI.cs`.
- Lógica: `Assets/Project/Scripts/Systems/MachineManager.cs`.

Arte vigente:

- `Assets/Project/UI/Vertical/Machine/machine_face_1_reference_v2.png`
- `Assets/Project/UI/Vertical/Machine/machine_face_2_reference_v2.png`
- `Assets/Project/UI/Vertical/Machine/machine_face_3_reference_v2.png`
- `Assets/Project/UI/Vertical/Machine/machine_face_4_reference_v2.png`

No borrar ni sobrescribir las bases antiguas `machine_face_*_base.png`.

Referencia aprobada:

- `C:\Users\nedfla\AppData\Local\Temp\codex-clipboard-1a2a5bb5-59ae-4beb-9dea-5a906011ba86.png`

Captura que mostraba el diseño antiguo incorrecto:

- `C:\Users\nedfla\AppData\Local\Temp\codex-clipboard-675ca077-b15a-48f7-b5f7-78cc49a94d57.png`

Evidencia actual:

- Ocho PNG en `Logs/VisualQA/MachineBlock1/`.
- Cara comparable principal: `Logs/VisualQA/MachineBlock1/07_face_2_repaired_1080x1920.png`.
- Log final: `Logs/machine_cube_reference_full_capture_final_v4.log`.
- Resultado final vigente del log:
  - `CONFIGURED | 4 faces | swipe | native node card`.
  - `PASS | 55 public | 8 secret | 44/55 threshold | exact requirements | 4 functional faces`.
  - `PASS | 4 initial + 4 repaired | 1080x1920 | save restored`.

Unity quedó cerrado. No hay archivos temporales `.save*` dentro de la carpeta de evidencia.

## Implementación actual que debe revisarse

`MachineCubeVisualUI.RotateRoutine()` ya aplica traslación, escala, alfa y rotación local en Y a dos `RectTransform`, pero las caras siguen siendo planos independientes y el resultado se percibe como intercambio de imágenes.

No se debe limitar la corrección a aumentar la duración o cambiar el easing. Primero hay que determinar por qué no se lee el volumen y construir una solución que mantenga simultáneamente visibles la cara frontal, el lateral saliente y el lateral entrante durante el giro.

La implementación puede requerir un rig 2.5D específico, planos de cara con perspectiva, un `UI Mesh`, un `Canvas` con cámara o un componente gráfico personalizado. No fijar la técnica antes de inspeccionar la jerarquía, el Canvas y las limitaciones de Unity UI actuales. Elegir la opción más estable para Android y compatible con la escena existente.

## Bloques recomendados

### Bloque A — auditoría y prototipo de rotación

- Inspeccionar `RotateRoutine`, `RectTransform`, `Canvas`, cámara y jerarquía actual.
- Medir por qué la rotación Y de los planos actuales no produce perspectiva convincente.
- Prototipar la rotación con solo dos caras y un lateral visible.
- Confirmar que el giro funciona tanto hacia izquierda como hacia derecha.
- Verificar que no aparece un frame de intercambio plano al inicio o al final.
- No integrar todavía las luces dinámicas si el volumen físico aún no está resuelto.

### Bloque B — rig definitivo de cuatro caras

- Integrar las cuatro caras en un único controlador de volumen.
- Mantener visibles y correctamente ordenados los planos necesarios durante el giro.
- Conservar nodos, hit targets, selección, guía, tarjeta, encabezado y progreso.
- Desactivar interacción durante la rotación y restaurarla al terminar.
- Mantener persistencia del índice de cara y guardado diferido.
- Unificar flechas y swipe con el mismo método de rotación.
- Evitar dos cubos superpuestos, flashes, recortes, caras invertidas o texto espejado.

### Bloque C — iluminación y daño

- Añadir capas luminosas o gráficos animados vinculados al estado real de cada nodo.
- Las señales deben provenir de `MachineManager`; no duplicar estado ni guardado.
- Dañado pero reparable: señal visible y contenida que invite a reparar.
- Bloqueado: señal apagada, débil o de advertencia sin parecer reparable.
- Reparado: iluminación estable integrada en el módulo/circuitería.
- Seleccionado/análisis: retícula y pulso temporal diferenciados.
- Añadir respiración ambiental muy sutil al cubo, sin convertir toda la pantalla en neón.
- Pausar o simplificar animaciones cuando el panel esté oculto o en una vista auxiliar.

### Bloque D — integración y QA

- El agente principal integra mediante setup idempotente y modifica `Main.unity`.
- Compilar antes de capturar.
- Ejecutar `MachineCubeBlock1Validation.ValidateBatch` y ampliar el validador para el nuevo rig.
- Verificar que sigue habiendo cero `MachineCubeCircuitLineUI`.
- Probar giro izquierda/derecha, swipe corto cancelado, swipe válido y pulsaciones repetidas.
- Probar los cuatro estados luminosos con estados técnicos preparados, sin usarlos como evidencia de balance.
- Crear evidencia temporal de la animación: secuencia de frames o captura equivalente que permita comprobar inicio, mitad y final del giro.
- Regenerar las ocho capturas estáticas después de cerrar la animación.
- Comparar Cara 2 reparada lado a lado con la referencia aprobada.
- Comprobar rendimiento móvil y ausencia de asignaciones repetidas por frame.
- Respaldar y restaurar la partida durante toda captura o prueba automatizada.

## Criterios de aceptación

- A mitad de transición se ven claramente dos caras conectadas por una arista común.
- La cara saliente se comprime en perspectiva y la entrante se expande desde el lateral.
- El sentido de giro coincide con la flecha o el swipe.
- No existe un instante en que una imagen desaparece y otra simplemente ocupa su lugar.
- El lateral derecho/izquierdo mantiene profundidad industrial coherente con el arte.
- No hay texto, pictogramas ni nodos espejados.
- Los hit targets coinciden con los módulos al terminar la rotación.
- No se puede seleccionar o reparar un nodo mientras el cubo está girando.
- La cara final, el encabezado, los puntos y el estado persistido coinciden.
- Las luces reaccionan al estado funcional real sin alterar la lógica.
- El daño se reconoce, pero la textura metálica y los medios tonos siguen legibles.
- No reaparecen hexágonos HUD dominantes ni líneas de árbol.
- El cubo mantiene el diseño estático actualmente aprobado cuando está en reposo.
- No se introducen errores de compilación, referencias faltantes ni pérdida de guardado.

## Límites del alcance

- No abordar Dimensión 1, Dimensión 2, Dimensión 3 ni su transición visual en este bloque.
- No cambiar costes, fórmulas, requisitos, cantidades, progreso ni balance de la Máquina.
- No modificar `MachineManager`, `SaveService` ni `machine_nodes.json` salvo que una necesidad técnica real e inevitable sea demostrada primero.
- No regenerar el arte aprobado sin autorización del usuario.
- No sustituir la dirección industrial por una estética HUD o neón dominante.
- No rehacer navegación, Triángulo, Mejoras ni otras pantallas.
- No realizar Git salvo petición explícita.

## Coordinación de agentes

- El agente principal es el único que modifica archivos, integra `Main.unity` y ejecuta Unity.
- Los agentes auxiliares trabajan en solo lectura.
- Un agente puede auditar la técnica de rotación y riesgos de Unity UI.
- Otro puede auditar estados visuales y rendimiento móvil.
- Otro puede ejecutar QA de listeners, selección, persistencia y guardado, siempre en solo lectura.
- Nunca ejecutar dos instancias de Unity simultáneamente.
- Preservar todos los cambios preexistentes del proyecto.

## Reglas completas de trabajo y continuidad

1. La instrucción actual y explícita del usuario tiene prioridad. Después siguen las reglas de trabajo, el resumen y finalmente la verificación de los archivos reales. Si se contradicen, informar antes de cambios importantes.
2. Leer completamente este resumen y revisar el código real antes de actuar. No repetir bloques terminados y probados.
3. Antes de cada bloque importante, indicar brevemente archivos/sistemas afectados, motivo, resultado esperado y pruebas o conexiones de Unity necesarias.
4. Avanzar de forma autónoma por las subdivisiones internas del bloque ya solicitado. Detenerse solo por una decisión de diseño material, contradicción, error bloqueante, acción destructiva o prueba visual que requiera al usuario.
5. Trabajar en el bloque funcional completo más grande que pueda implementarse y verificarse con seguridad. No mezclar bloques grandes independientes.
6. Preservar compatibilidad con partidas antiguas y todos los cambios preexistentes. No realizar refactorizaciones generales sin necesidad concreta.
7. El usuario autoriza crear o modificar escenas, jerarquías, componentes, referencias del Inspector y eventos necesarios para el bloque. El usuario no debe hacer conexiones manuales.
8. Si Unity está abierto y puede sobrescribir escenas o assets, pedir cerrarlo. No ejecutar Unity batch mientras exista otra instancia normal o batch.
9. El agente principal es el único que modifica la escena y ejecuta Unity. Agentes auxiliares son de solo lectura.
10. Si aparece un error de compilación relacionado, detener el avance, corregirlo y validar otra vez antes de continuar.
11. Agrupar compilaciones y pruebas por hitos funcionales. Ejecutar validación integral al cerrar el bloque.
12. Separar siempre código terminado, conexiones realizadas, pruebas ejecutadas, pruebas pendientes y problemas conocidos.
13. Probar guardado/carga si se modifica estado persistente. Una partida DEBUG solo sirve para pruebas técnicas aisladas, no como evidencia de progresión o balance natural.
14. No declarar un bloque terminado si falta una conexión o prueba indispensable. En ese caso describirlo como código terminado con validación pendiente.
15. No crear commits, ramas, staging, push, reset ni otras operaciones Git salvo petición explícita. Solo se permiten consultas Git de lectura cuando sean necesarias.
16. No eliminar, reemplazar ni descartar trabajo del usuario. No modificar archivos ajenos al alcance.
17. La dirección creativa pertenece al usuario. No inventar costes, fórmulas, tasas, recompensas, requisitos, desbloqueos, narrativa ni comportamientos no aprobados.
18. Una inferencia técnica o visual no se convierte en diseño canónico. Si existen varias interpretaciones materiales, presentar opciones e impacto antes de implementar esa parte.
19. Los valores provisionales para pruebas requieren autorización; deben quedar identificados, centralizados y ser fáciles de retirar.
20. Las decisiones nuevas aprobadas deben registrarse en el diseño o continuidad al mismo tiempo que se implementan.
21. No afirmar que una función es completamente jugable si solo funciona mediante DEBUG o carece de una interfaz indispensable.
22. Comparar las capturas finales con la referencia aprobada, no únicamente con versiones anteriores del proyecto.
23. Al cerrar el siguiente bloque, documentar archivos modificados, conexiones, pruebas, errores corregidos, riesgos restantes y siguiente paso.
24. Si el chat vuelve a ser demasiado extenso, terminar el bloque actual en un punto seguro y crear otro resumen autosuficiente que reproduzca estas reglas y todo el camino pendiente.

## Primer paso exacto del siguiente chat

1. Confirmar que Unity esté cerrado.
2. Revisar `MachineCubeVisualUI.RotateRoutine()` y la jerarquía serializada actual en `Main.unity`.
3. Explicar brevemente qué técnica de rig/rotación se propone, por qué conserva nodos e interacción y cómo se verificará en Android.
4. Implementar primero un prototipo de rotación física entre Cara 1 y Cara 2, sin luces nuevas.
5. Compilar y generar evidencia de inicio/mitad/final del giro.
6. Solo después de confirmar que ya se percibe como un cubo, extender a cuatro caras y abordar los estados luminosos.


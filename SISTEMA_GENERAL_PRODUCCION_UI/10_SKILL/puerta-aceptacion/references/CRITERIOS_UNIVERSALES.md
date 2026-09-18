# Criterios universales de aceptación

Versión: 1.0  
Alcance: cualquier proyecto y cualquier tipo de entregable.

Cada criterio aparece siempre en el registro. Los criterios `U01`, `U02`, `U03`, `U04`,
`U07`, `U08`, `U10` y `U11` requieren `PASS`. Los criterios condicionales `U05`, `U06`,
`U09` y `U12` requieren `PASS` cuando aplican; si no aplican, requieren `N/A` y una razón
concreta. Ningún criterio admite omisión silenciosa.

| ID | Fase | Tipo | Criterio observable | Evidencia mínima |
|---|---|---|---|---|
| U01 | preflight | siempre | El entregable, el alcance, la versión y la condición de terminado están definidos. | Registro o contrato vigente. |
| U02 | preflight | siempre | Se identificaron las instrucciones, guías, fuentes de autoridad y criterios particulares aplicables. Las exclusiones tienen razón. | Lista de fuentes y módulos aplicables/excluidos. |
| U03 | preflight | siempre | Se inspeccionó el estado real y se separó la evidencia anterior de la nueva. | Inventario, captura base, lectura o auditoría fechada. |
| U04 | preflight | siempre | Cada dato, capa, archivo o componente afectado tiene fuente definitiva y propietario único. | Contrato, mapa de capas, rutas o matriz de propiedad. |
| U05 | unit | condicional | Antes de un lote o paso costoso se aprobó una unidad que concentra el mayor riesgo. | Unidad dentro del contexto real y prueba crítica. |
| U06 | unit | condicional | Los datos exactos provienen de una fuente determinista y se validan automáticamente. | Tabla o patrón canónico, salida del validador y revisión del render cuando sea visible. |
| U07 | unit | siempre | Existen comprobaciones estructurales o funcionales proporcionales al cambio y todas pasan. | Comando con resultado `PASS` y salida guardada, o comprobación reproducible. |
| U08 | final | siempre | El resultado se revisó en su contexto final representativo y a la escala, plataforma o formato de consumo. | Captura, render, build, archivo abierto o recorrido reproducible posterior al cambio. |
| U09 | final | condicional | Una colección o conjunto repetido tiene inventario completo y cada unidad o celda coincide con su contrato. | Matriz de cobertura y lámina ensamblada desde unidades verificadas. |
| U10 | final | siempre | Las categorías fuera del alcance conservaron sus invariantes y las regresiones relevantes pasan. | Comparación antes/después o suite focalizada. |
| U11 | final | siempre | Toda evidencia de cierre pertenece a la última versión y es posterior al último cambio relevante. | Archivos o resultados fechados y vinculados al registro. |
| U12 | final | condicional | Una acción externa, destructiva, irreversible o sobre datos reales tiene autorización y verificación posterior. | Registro de autorización y resultado comprobado. |

## Estados permitidos

- `PASS`: el criterio se demostró con evidencia identificable.
- `N/A`: sólo para un criterio condicional, con una razón que explique por qué el
  entregable no posee esa capacidad o riesgo.
- `PENDING`: falta trabajo o prueba; bloquea el avance de la fase.
- `FAIL`: la comprobación encontró un defecto; bloquea.
- `BLOCKED`: una dependencia impide demostrarlo; bloquea.

## Fases y condición de avance

`preflight` debe pasar antes de producir. `unit` debe pasar antes de un lote cuando `U05`
aplique. `final` debe pasar después del último cambio y antes de presentar el entregable
como terminado. Una fase posterior incluye los criterios de las anteriores.

El validador calcula el resultado. Un texto, una captura aislada o una impresión general no
pueden reemplazar un criterio fallido. Los problemas abiertos con severidad `blocking`
impiden el cierre.

## Evidencia

La evidencia debe tener tipo, descripción, fecha y valor. Los tipos válidos son:

- `file`: ruta existente archivada dentro de `workspace_root`.
- `command`: comando reproducible, resultado `PASS` y archivo de salida existente.
- `url`: enlace estable con descripción suficiente.
- `observation`: revisión humana concreta; se usa para cualidades que no puede resolver una
  prueba automática y debe indicar qué se observó y en qué contexto.

Para `U11`, la fecha de cada evidencia final debe ser igual o posterior a
`latest_change_at`. Las rutas vacías, marcadores de posición y afirmaciones genéricas no
cuentan.

## Módulos específicos

Después del núcleo se agregan en `additional_criteria` los criterios particulares y los
módulos que correspondan: UI visual, interacción,
accesibilidad, seguridad, privacidad, rendimiento, persistencia, motor, móvil, documentos,
datos, despliegue u otros. Omitir un módulo que parece relacionado exige una razón en
`excluded_modules`. Las reglas específicas del proyecto pueden endurecer esta puerta, pero
no rebajarla.

### Composición visual por capas

Cuando el entregable combina dos o más capas visuales con propietarios distintos, se añade
`visual_layer_composition` a `applicable_modules`. La fase `unit` requiere exactamente un
criterio adicional `VLC01`, no condicional y en `PASS`, cuya evidencia incluya un `command`.
La prueba debe revisar el resultado compuesto y no limitarse a validar archivos aislados.
Como mínimo debe detectar: una segunda superficie completa puesta encima, un parche opaco,
una capa superior que oculta el área temática, un marco opaco dentro del campo reservado,
datos RGB ocultos en alfa cero cuando puedan reaparecer y una composición que no pueda
reproducirse desde sus fuentes declaradas.

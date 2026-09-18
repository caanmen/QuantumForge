# Principios generales de producción UI

## P01 — Autoridad explícita

El contenido y la apariencia proceden del diseño aprobado del proyecto actual. La
experiencia anterior ayuda a preguntar y verificar, no a completar vacíos creativos.

## P02 — Alcance declarado

Cada regla indica si es general, técnica, de proyecto, de familia visual o de pantalla.

## P03 — Trazabilidad visual

Cada referencia, concepto, asset y captura conserva origen, estado, fecha y alcance.

## P04 — Reutilización antes de creación

Buscar en el proyecto real, catálogo, componentes y evidencia aprobada. Reutilizar sólo
cuando identidad y alcance coincidan; una semejanza no basta.

## P05 — Fidelidad sin simplificación silenciosa

Proporción, densidad, medio gráfico, volumen y jerarquía forman parte del diseño. Toda
reducción visible exige justificación y aprobación.

## P06 — Propietario único

Un dato o propiedad dinámica tiene un único escritor. Layout, estado visual, reglas,
navegación y animación deben tener responsabilidades separables.

## P07 — Estados completos

Definir los estados que realmente permita el sistema: neutral, normal, seleccionado,
bloqueado, deshabilitado, vacío, completado, error, carga u otros autorizados.

## P08 — Identidad estable

Las colecciones y selecciones dinámicas se vinculan por ID estable. Texto, arte, material,
estadísticas, accesibilidad y acción se actualizan como una unidad.

## P09 — Evidencia vigente

Una captura, prueba o aprobación pertenece a una versión concreta. Se invalida cuando
cambia cualquier fuente capaz de alterar el resultado que certificaba.

## P10 — Prueba de recorrido real

Una llamada directa, un mockup o una captura aislada no prueban que el jugador pueda entrar,
actuar, salir y regresar mediante los controles reales.

## P11 — Accesibilidad y descubribilidad

Los controles indispensables deben ser visibles, comprensibles, alcanzables y operables
con los métodos de entrada y condiciones reales del proyecto.

## P12 — Rendimiento medido

Optimizar a partir de una línea base y del dispositivo objetivo. No degradar fidelidad por
intuición ni aceptar coste excesivo sólo porque la build termina correctamente.

## P13 — Persistencia protegida

Las pruebas con datos artificiales no contaminan el progreso real. Las acciones persistentes
guardan o transaccionan según un contrato probado.

## P14 — Fuente definitiva reproducible

Escena, prefab, código, plantilla y generador deben señalar una única fuente canónica o
permanecer sincronizados mediante una reconstrucción idempotente.

## P15 — Aprendizaje depurado

Los casos se archivan con detalle; el manual general conserva reglas consolidadas, sin
duplicados ni crecimiento append-only. Un responsable revisa periódicamente el catálogo.

---
name: puerta-aceptacion
description: Aplica una puerta de aceptación bloqueante a entregables de cualquier proyecto cuando se vaya a producir un lote, integrar, presentar como final o cerrar trabajo. Exige alcance, autoridad, pruebas proporcionales y evidencia vigente; permite N/A únicamente en criterios condicionales con una razón concreta.
---

# Puerta obligatoria de aceptación

Usa esta puerta antes de producir trabajo costoso en lote y antes de afirmar que un
entregable está terminado. No sustituye las instrucciones del usuario ni los criterios
particulares del proyecto: los incorpora y permite que sean más estrictos.

## Flujo

1. Lee las instrucciones vigentes y los archivos reales del proyecto antes de modificar.
2. Identifica el entregable, su fuente definitiva, los criterios particulares y el riesgo
   que podría invalidar más trabajo.
3. Crea el registro con `scripts/validate_gate.py --init RUTA --project NOMBRE
   --deliverable NOMBRE`.
4. Completa la fase `preflight` y ejecútala antes de producir o implementar.
5. Si habrá lote, automatización costosa o muchas variantes, valida primero la fase `unit`
   con una unidad representativa de mayor riesgo.
6. Produce el resto sin reinterpretar las fuentes aprobadas.
7. Completa la fase `final` con pruebas adecuadas al cambio y evidencia posterior al último
   cambio relevante.
8. Ejecuta `scripts/validate_gate.py REGISTRO --phase final`. Sólo un código de salida cero
   permite presentar el trabajo como final o aceptado.

Lee [references/CRITERIOS_UNIVERSALES.md](references/CRITERIOS_UNIVERSALES.md) al crear o
auditar un registro. Para UI, arte, colecciones, motores o plataformas concretas, aplica
además las guías del proyecto; no copies parámetros particulares de otros proyectos.

## Reglas bloqueantes

- No continúes a un lote si la unidad representativa aplicable no tiene `PASS`.
- No inventes ni delegues a arte generativo datos exactos que puedan componerse de forma
  determinista.
- No aceptes un criterio con una afirmación sin evidencia identificable.
- No reutilices capturas o resultados anteriores al último cambio como evidencia final.
- No conviertas `FAIL`, `BLOCKED`, `PENDING` o ausencia de prueba en `PASS` narrativo.
- `N/A` sólo es válido en criterios declarados condicionales y requiere una razón específica.
- Si un fallo documentado se repite, añade una aserción, prueba o validador que lo detecte
  antes de producir nueva evidencia.
- Si el entregable compone fondos, contenido, marcos, ornamentos u otras capas visuales,
  declara el módulo `visual_layer_composition`. Desde `unit`, la puerta exige un criterio
  adicional `VLC01` en `PASS` con evidencia de tipo `command` que compruebe el render final:
  cada superficie tiene un solo propietario, las capas superiores son transparentes fuera
  de su contenido autorizado y ninguna cara completa o parche opaco oculta una capa que debe
  permanecer visible.
- Si cambias una fuente después de validar, invalida la fase afectada y vuelve a ejecutarla.

## Proporcionalidad

La puerta obliga a demostrar el resultado, pero la prueba debe corresponder al riesgo. Una
edición pequeña puede usar una revisión y una comprobación focalizadas. Un cambio de datos,
una colección, una interacción, una migración, una build o una salida visual necesita las
pruebas específicas que demuestren esas capacidades. No añadas pruebas que sólo repitan la
implementación ni ejecutes módulos incompatibles con el entregable.

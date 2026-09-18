# Puerta obligatoria de aceptación

Ésta es la puerta canónica para iniciar, ampliar y cerrar entregables. Se aplica a cualquier
proyecto que adopte el sistema, además de sus módulos técnicos y criterios particulares.

La autoridad detallada está en
`10_SKILL/puerta-aceptacion/references/CRITERIOS_UNIVERSALES.md`. El registro se crea y valida
con `10_SKILL/puerta-aceptacion/scripts/validate_gate.py`.

## Condición obligatoria

No se puede declarar un entregable `APROBADO`, `FINAL`, `TERMINADO` o equivalente cuando:

- la fase aplicable no devuelve `PASS`;
- existe un criterio `PENDING`, `FAIL` o `BLOCKED`;
- falta un criterio universal;
- se usa `N/A` en un criterio obligatorio o sin una razón concreta;
- falta evidencia, la ruta no existe o la evidencia final es anterior al último cambio;
- existe un problema abierto con severidad `blocking`;
- el proyecto exige un criterio más estricto que todavía no se demostró.

## Uso mínimo

```powershell
python Logs/general-ui-docs-package/10_SKILL/puerta-aceptacion/scripts/validate_gate.py `
  --init Logs/Acceptance/entregable.json --project "Proyecto" --deliverable "Entregable"

python Logs/general-ui-docs-package/10_SKILL/puerta-aceptacion/scripts/validate_gate.py `
  Logs/Acceptance/entregable.json --phase preflight

python Logs/general-ui-docs-package/10_SKILL/puerta-aceptacion/scripts/validate_gate.py `
  Logs/Acceptance/entregable.json --phase final
```

El código de salida cero significa `PASS`. Cualquier otro código bloquea la fase y enumera
qué falta. El archivo generado es un registro de trabajo: sus estados empiezan en `PENDING`
y deben completarse con evidencia real.

## Secuencia que reduce retrabajo

1. `preflight`: alcance, autoridad, estado real y propietarios.
2. `unit`: riesgo principal, datos exactos y validación estructural.
3. Producción o integración sin reinterpretar la unidad aprobada.
4. `final`: contexto real, cobertura, regresión y evidencia vigente.
5. Si cambia una fuente, volver a abrir los criterios afectados y renovar la evidencia.

Cuando un error ya documentado reaparece, se detiene la producción y se convierte su causa
en una comprobación ejecutable antes de generar otra propuesta.

Para entregables que integran fondo, contenido, marco u ornamentos se declara el módulo
`visual_layer_composition`. La fase `unit` no pasa sin `VLC01` y una evidencia automática
que mida el render compuesto. Una captura bonita o la presencia de las fuentes correctas no
bastan si una cara completa, un rectángulo opaco o un segundo fondo ocultan otra capa.

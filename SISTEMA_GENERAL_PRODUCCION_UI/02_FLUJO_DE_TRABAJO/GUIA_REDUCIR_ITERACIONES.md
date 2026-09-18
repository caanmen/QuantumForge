# Guía para reducir iteraciones manteniendo calidad

Alcance: cualquier videojuego. Este proceso busca reducir retrabajo, no omitir validación.

## 1. Atacar primero el riesgo

Antes de producir, registrar objetivo, público, plataforma, dispositivo mínimo, tres pilares
no negociables y los riesgos capaces de invalidar más trabajo. Prototipar la incertidumbre:
interacción, legibilidad, rendimiento, guardado, pipeline o coste de contenido.

No estimar el juego completo hasta terminar una unidad representativa —pantalla, enemigo,
nivel o animación— y medir su coste real.

## 2. Elegir el tipo correcto de prototipo

- Riesgo visual: composición estática con assets reales y contenido representativo.
- Riesgo de interacción: prototipo interactivo desechable antes del arte final.
- Riesgo técnico: prueba vertical mínima que incluya entrada, resultado, guardado, carga y
  rendimiento en el dispositivo objetivo.

Un mockup demuestra intención visual; no demuestra tacto, navegación ni respuesta real.

## 3. Trabajar con puertas pequeñas

1. Autoridad y alcance.
2. Grandes bloques, proporción y encuadre.
3. Assets y estados estáticos.
4. Datos e interacción real.
5. Transición y animación.
6. Rendimiento en dispositivo.
7. Evidencia y aprobación.

No avanzar cuando el cambio siguiente ocultaría un fallo de la puerta anterior.

## 4. Cambiar una categoría por vez

Para conservar causalidad, no modificar simultáneamente escala, posición, fondo, alfa y
tiempo. Corregir en este orden: viewport y bloques grandes; encuadre; anclas y zona segura;
capas y máscaras; estados; transición; detalle; rendimiento. Capturar después de cada grupo.

## 5. Definición observable de terminado

Antes de empezar una tarea, declarar qué evidencia la cerrará. Una unidad terminada:

- cumple función y composición aprobadas con datos y assets reales;
- mantiene sincronizadas sus fuentes definitivas y reconstructores;
- supera pruebas rápidas, recorrido real y estados aplicables;
- respeta presupuestos medidos en la plataforma objetivo;
- tiene evidencia posterior al último cambio y documentación actualizada.

Compilar no equivale a terminar. Un PASS técnico tampoco equivale a aprobación artística.

## 6. Automatización por frecuencia

- Cada cambio: compilación, validadores de datos y pruebas rápidas.
- Cada bloque: Play Mode, navegación, estados y persistencia.
- Periódicamente: capturas deterministas, regresión visual, build y perfilado en dispositivo.
- Antes de entregar: instalación limpia, actualización sobre progreso respaldado y recorrido
  humano completo.

Las suites rápidas bloquean pronto; las lentas no deben impedir la iteración cotidiana.

## 7. Revisión visual reproducible

Conservar referencias por pantalla, estado y resolución. Comparar imagen completa y recortes
críticos mediante superposición o diferencia. En una transición capturar 0 %, 25 %, 50 %,
75 % y 100 %, comprobando que nunca desaparezcan a la vez origen, destino y cobertura.

Una persona aprueba cambios artísticos intencionales antes de actualizar una referencia.
Nunca se actualiza la referencia sólo para hacer pasar una prueba.

## 8. Retrospectiva que cambia el sistema

Después de un fallo costoso registrar: esperado, ocurrido, línea temporal, causa, señal
ignorada, puerta que permitió escapar el error y acción preventiva. Una acción válida crea
o modifica una prueba, preset, validador, plantilla, presupuesto o puerta. “Tener más
cuidado” no evita la repetición.

## Cadencia mínima para un equipo pequeño

- Diario: unidad vertical pequeña y pruebas rápidas.
- Dos veces por semana: build jugable y dispositivo.
- Semanal: alcance, riesgos, rendimiento y tamaño de build.
- Por hito: recorrido completo, comparación visual y retrospectiva.
- Mensual: retirar duplicados, candidatos abandonados y deuda que ya frena el trabajo.

Fuentes y fecha de revisión: `08_INVESTIGACION_Y_FUENTES/FUENTES_VERIFICADAS_2026-08-27.md`.

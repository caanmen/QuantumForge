# Caso histórico: cartas de Golf, conteo, capas y zonas reservadas

Fecha: 2026-09-09  
Proyecto de origen: Solitario  
Alcance reutilizable: colecciones de cartas, fichas o elementos con arte ilustrado y datos
exactos.

## Problemas observados

- Cartas de valor 7 mostraron cuatro, cinco, diez u otras cantidades incorrectas de
  símbolos centrales durante distintas propuestas.
- Cartas de valor 10 llegaron a mostrar once símbolos aunque el conjunto pareciera
  equilibrado a primera vista.
- Algunas correcciones se hicieron colocando una nueva cara dentro o encima de otra, lo
  que produjo rectángulos con apariencia de pantalla, marcos dobles y fondos tapados.
- Aparecieron índices repetidos o superpuestos al mezclar información incluida en la
  imagen con otra añadida durante el montaje.
- Al corregir un problema concreto cambiaron detalles que ya estaban aprobados: fondos,
  ornamentos, riqueza visual, tamaño de zonas reservadas y marcos propios de cada tema.
- Algunas cartas quedaron inclinadas, recortadas o con distinta escala dentro de la lámina.
- Se revisó la composición general sin inventariar y contar cada celda individualmente.
- Se añadió un círculo decorativo a los índices numéricos aunque el diseño no lo necesitaba;
  el resultado parecía un botón pegado y chocaba con marcos y ornamentos distintos.

## Causas

1. Se trató una generación visual como fuente fiable para información discreta.
2. La lámina conjunta reinterpretó cartas en vez de ensamblar archivos ya aprobados.
3. No se declaró un único propietario para fondo, marco, figura, índice y símbolos.
4. El conteo se confió a la impresión visual y no a una tabla canónica verificable.
5. Las correcciones no bloquearon explícitamente las categorías que debían permanecer
   intactas.
6. Se trató un contenedor opcional como requisito y se aplicó antes de comprobar si el glifo
   solo ya era legible sobre el fondo de cada tema.

## Regla consolidada

La ilustración generativa puede resolver fondo, textura, personaje y ornamentación. Los
rangos, palos, números, textos y cantidades se componen mediante código o plantilla. Cada
valor usa posiciones canónicas y pasa dos verificaciones: conteo estructural automático y
conteo visual sobre el render final.

La lámina de presentación se construye después, colocando esas unidades verificadas en una
retícula determinista. Nunca se usa la propia lámina generada como fuente de producción.

Reglas incorporadas al catálogo general: `UI-GEN-023` a `UI-GEN-031`.

Un índice simple se prueba primero sin contenedor. Círculos, placas, cápsulas o fondos sólo se
añaden si forman parte del lenguaje visual aprobado o si una medición demuestra que resuelven
un problema real de contraste; nunca se usan para compensar una ubicación incorrecta.

## Contención y solución aplicada

- Las láminas con conteos dudosos dejaron de considerarse evidencia aprobable.
- Se consolidaron posiciones deterministas del as al 10; cada lista contiene exactamente la
  cantidad de símbolos indicada por su rango.
- Se separó el arte ilustrado de los datos exactos para que una regeneración no decida la
  cantidad de símbolos.
- Se restauraron exactamente los seis fondos numéricos originales y Unity compone sobre ellos
  un número superior sin círculo, sin palo de esquina y sin índice inferior.
- Las seis barajas se renderizaron desde Unity y se revisaron tanto por separado como en una
  sola cuadrícula. La prueba estructural del as al 10 y la auditoría visual terminaron sin fallos.

## Puerta mínima para cartas numéricas

- [x] El 10 tiene exactamente diez símbolos centrales.
- [x] El índice no tiene círculo, placa o fondo innecesario; cualquier soporte está justificado.
- [x] La zona del índice no invade ni tapa el marco o los ornamentos del tema.
- [x] El 7 tiene exactamente siete símbolos centrales.
- [x] El palo del índice no se confunde con los símbolos centrales durante el conteo.
- [x] No existen índices inferiores o duplicados si no forman parte del contrato aprobado.
- [x] Fondo, marco y cara aparecen exactamente una vez.
- [x] No hay parches rectangulares ni cambios de textura en el área corregida.
- [x] La carta está vertical, completa, con la proporción y escala aprobadas.
- [x] Cada tema conserva sus detalles, colores y marco propio.
- [x] Se revisó cada carta individualmente y después toda la colección en una sola cuadrícula.

## Aplicación al siguiente ciclo

Primero se aprueba una unidad de mayor riesgo. Después se generan los fondos y figuras sin
datos exactos, se componen índices y símbolos de forma determinista, se ejecuta la matriz de
conteo y finalmente se arma una única lámina de revisión. Si falla una celda, se sustituye
esa unidad desde su fuente; no se cubre con otra carta completa.

## Elementos que no deben heredarse

- Los temas, nombres, colores, personajes y marcos de Golf pertenecen a Solitario.
- Los valores 10 de diamantes y 7 de tréboles son casos de prueba, no una selección universal.
- La existencia y el tamaño de un medallón dependen de una necesidad visual comprobada; no son obligatorios.
- Las rutas y resoluciones del proyecto no son reglas para otros juegos.

## Evidencia en el proyecto de origen

- Candidato generativo invalidado para conteo:
  `Logs/GolfCompactIndexDesign/golf-compact-medallions-with-numbers-v1.png`.
- Candidato estructural histórico anterior a retirar los círculos:
  `Logs/GolfCompactIndexDesign/golf-compact-medallions-with-exact-numbers-v2.png`.
- Generador determinista corregido para no volver a introducir contenedores numéricos:
  `Tools/build_golf_compact_number_preview.py`.
- Cuadrícula final renderizada desde Unity:
  `Logs/GolfCompactIndexFinal/golf-readable-index-unity-audit-contact.jpg`.
- Ampliación final de las cartas numéricas:
  `Logs/GolfCompactIndexFinal/golf-readable-number-cards-unity-zoom.png`.
- Resultado de pruebas de Unity, incluido el conteo del as al 10:
  `Logs/golf-readable-index-results.xml`.
- Partida real de Golf con el tema aplicado y las siete columnas visibles:
  `Logs/golf-scottish-heath-gameplay-final.png`.
- Resultado de la prueba de integración en el tablero real:
  `Logs/golf-gameplay-final-results.xml`.

# Caso histórico: Forty Thieves, topología y ocupación interior

Fecha: 2026-09-10  
Unidad: `Perla del Kraken`  
Alcance reutilizable: índices compuestos y cartas numéricas con símbolos centrales.

## Fallos observados

1. El bloque completo `rango + palo` podía quedar centrado, pero el rango se acercaba
   demasiado al borde superior e inferior y el palo quedaba demasiado separado.
2. Inicialmente se clasificó erróneamente la firma tradicional `2-1-2-2-1-2` como un
   zigzag y se reemplazó por `2-2-1-1-2-2`. La comparación posterior con una carta real
   aportada por el usuario demostró que esa corrección anterior era incorrecta.
3. Conteo y centro por sí solos permitían que ambos defectos pasaran la validación.

## Reglas obligatorias incorporadas

### Índices compuestos

- La autoridad geométrica es el hueco visible medido en el maestro limpio, no las
  coordenadas utilizadas por el ensamblador.
- Se centra la huella visible completa de `rango + palo`.
- El perfil declara un máximo de ocupación interior. En el perfil compacto de las barajas
  actuales de Solitario, la tinta visible no puede superar el `60 %` de la altura del hueco.
- Los índices superior e inferior deben mostrar la misma huella visible; el inferior se
  obtiene mediante rotación exacta de `180 grados`.
- Una fixture con rango y palo demasiado separados debe fallar antes de guardar.

### Símbolos centrales

- La tabla canónica obligatoria incluye del As al `10` para los cuatro palos, no sólo las
  cartas `7` y `10` usadas como muestras en la propuesta.
- Para cada valor incluye cantidad, firma de filas o columnas, separación mínima,
  centrado por promedio y límites, y orientación.
- Las firmas son `A = 1`, `2 = 1-1`, `3 = 1-1-1`, `4 = 2-2`, `5 = 2-1-2`,
  `6 = 2-2-2`, `7 = 2-1-2-2`, `8 = 2-1-2-1-2`, `9 = 2-2-1-2-2` y
  `10 = 2-1-2-2-1-2`.
- El `10` clásico usa obligatoriamente `2-1-2-2-1-2`: dos mitades `2-1-2`, con cinco
  símbolos en cada orientación.
- El validador mide el centro promedio cuando corresponde y siempre el centro de los límites
  exteriores. El `7` inglés `5/2` conserva su asimetría tradicional.
- Las filas opuestas deben guardar simetría respecto al centro visible del campo.
- La separación mínima se mide entre las cajas visibles de filas consecutivas, no sólo
  entre coordenadas nominales.
- La variante `2-2-1-1-2-2`, una versión amontonada, una pieza faltante, una adicional o un
  patrón desplazado deben devolver error aunque la cantidad o el promedio coincidan.
- La orientación se valida por fila canónica, no sólo por la coordenada vertical; una
  orientación incorrecta también bloquea la unidad.

## Defensa ejecutable del caso

- Ensamblador determinista:
  `Tools/assemble_forty_thieves_perla_unit.py`
- Validador independiente:
  `Tools/validate_forty_thieves_perla_unit.py`
- Informe vigente:
  `Logs/FortyThievesDeckProposals/perla-del-kraken-validation-v6-all-numeric-rules.json`
- Puerta de aceptación:
  `Logs/FortyThievesDeckProposals/forty-thieves-representative-v1-acceptance-gate.json`
- Matriz canónica completa:
  `04_PLANTILLAS/PATRONES_CANONICOS_CARTAS_NUMERICAS.json`

La prueba de unidad debe terminar con código cero antes de producir las otras cinco
barajas. Para un lote de cartas, `U05`, `U06` y `U09` continúan siendo obligatorios y no
pueden convertirse en `N/A`.

## Criterio de no regresión

No se acepta como corregida ninguna carta del As al `10` sólo porque tenga la cantidad
exacta o porque el promedio de sus símbolos coincida con el centro. Cada valor y palo debe
pasar simultáneamente su topología, separación, centro por promedio y límites, orientación,
ocupación del índice y fixtures negativas.

## Extensión: falso positivo al detectar el hueco del índice

Durante el primer lote de seis temas, varias cartas numéricas recibieron centros como
`(79, 120)` porque el detector eligió una porción del campo numérico central en lugar del
hueco marfil estrecho de la esquina. El validador reutilizaba ese centro incorrecto y emitió
un falso `PASS`; la revisión del usuario reveló rangos fuera de lugar.

Desde esta corrección, el ensamblador y el validador independiente exigen que el hueco sea
un componente claro, aislado, estrecho y situado completamente en la esquina esperada.
Además, la caja de tinta completa debe quedar contenida dentro de su límite visible. Al
aplicar estas aserciones a la lámina defectuosa, el validador detectó 34 incumplimientos;
después de reensamblar, las 36 celdas pasan.

- Ensamblador de lote: `Tools/assemble_forty_thieves_six_themes.py`
- Validador de lote: `Tools/validate_forty_thieves_six_themes.py`
- Evidencia corregida: `Logs/FortyThievesDeckProposals/forty-thieves-six-themes-validation-v1.json`

## Extensión: rango y palo pegados dentro de un bloque centrado

La primera integración runtime de la distribución universal conservó el centro del índice,
pero reutilizó offsets internos de `0.028` para el rango y `0.030` para el palo. En una carta
de teléfono de `220 × 348 px`, las envolventes visibles se tocaban, especialmente en `10`.

Desde esta corrección es obligatoria una **separación interna visible mínima** medida entre
las huellas conservadoras, no entre sus centros. El perfil de Perla del Kraken usa offsets
simétricos de `0.045 + 0.045` y deja `5.58 px`, por encima del mínimo bloqueante de `4 px`.
La regla cubre A–10 y J/Q/K, las dos esquinas deben producir el mismo resultado y la inferior
permanece rotada `180 grados`. La fixture sin aire que reproduce los offsets anteriores debe
devolver `FAIL`.

Autoridad canónica:
`04_PLANTILLAS/PERFIL_CANONICO_INDICES_COMPUESTOS.json`.

## Extensión: patrón correcto pero demasiado estrecho para el marco

La primera captura final de Bandera Carmesí tenía diez diamantes, firma
`2-1-2-2-1-2` y centro correcto, pero reutilizaba la escala horizontal de los otros cinco
temas. La huella ocupaba sólo `43.93 %` del campo claro y visualmente formaba una columna
amontonada. El validador anterior repetía las coordenadas del generador y no comparaba la
salida con la abertura real, por lo que produjo un falso `PASS`.

Desde esta corrección, cada marco posee un perfil de escala del campo numérico y la
ocupación horizontal se obtiene de píxeles renderizados contra un campo útil detectado de
forma independiente. Bandera Carmesí exige al menos `52 %`; la captura defectuosa queda
bloqueada aunque conserve cantidad, topología y centrado. La regla se aplica a todos los
rangos A–10: no está limitada al diez usado para descubrir el problema.

Validador bloqueante:
`Tools/measure_forty_thieves_pip_occupancy.py`.

## Extensión: rango centrado geométricamente pero sin aire lateral

En Bandera Carmesí, `Q` y `10` ocupaban el `100 %` del ancho claro detectado en sus
huecos; `K` ocupaba `80 %`. Las coordenadas de los contenedores coincidían con el centro,
pero la tinta visible tocaba los bordes y producía una lectura descentrada. Reducir todo el
marco o redibujarlo no es una corrección válida.

Desde esta corrección, cada rango ancho puede declarar una escala horizontal propia que
no cambia la altura tipográfica. El validador mide píxeles del render contra el hueco claro
de cada cara limpia y exige como máximo `76 %` en Bandera Carmesí, además de comprobar el
centro horizontal y la equivalencia rotada de ambas esquinas. La fixture sin escala debe
devolver `FAIL`.

Validador bloqueante:
`Tools/validate_forty_thieves_index_optical_fit.py`.

## Extensión: validador parcial de cuatro rangos

El primer validador óptico de Bandera Carmesí sólo inspeccionaba `Q`, `10`, `J` y `K`,
y dentro de cada hueco medía principalmente la tinta del rango aislado. El inventario
marcaba `N/A` para el ajuste visible de `A` a `9`. Por ello pudo emitir `PASS` aunque los
acercamientos del usuario demostraron que `K + corazón` y `7 + trébol` no formaban un
bloque visual centrado.

Ese `PASS` queda invalidado. La defensa obligatoria captura a `220 × 348 px` cuatro capas
independientes (`full`, `rank_only`, `suit_only` y `clean`) para los trece rangos y los
cuatro palos. En las dos esquinas debe medir la tinta real del rango, la tinta real del
palo, el bloque visible completo, el centro horizontal y vertical, la alineación sobre un
mismo eje, la separación mínima, la contención, la ocupación lateral y la equivalencia por
rotación de `180 grados`. Ninguna combinación puede convertirse en `N/A`.

Las fixtures de cobertura parcial, rango aislado sin palo, deriva compuesta de `K` y `7`,
palos ausentes e inventario con `N/A` deben devolver `FAIL` antes de aceptar evidencia.

# Caso histórico: Canfield, centrado óptico de índices

Fecha: 2026-09-09  
Proyecto de origen: Solitario  
Alcance reutilizable: índices compuestos dentro de marcos curvos o asimétricos.

## Fallo observado

Durante las propuestas de Canfield se reutilizaron desplazamientos aproximados sobre una
lámina completa. Q, J, K, 10 y 7 presentaban alturas visibles distintas; letra y palo se
centraban según una caja rectangular aunque la abertura del marco era curva. La evidencia
se presentó antes de revisar las esquinas ampliadas.

Las reglas necesarias ya existían en las guías. La causa de la reincidencia fue tratarlas
como consulta y no como una puerta bloqueante del ensamblador.

## Causas concretas

1. Se produjo un lote de seis temas antes de aprobar una unidad representativa.
2. Se usó una coordenada aproximada para marcos con geometrías distintas.
3. El 10 usó un tamaño tipográfico diferente al de los demás rangos.
4. La fuente inicial utilizaba cifras de altura antigua; el 10 resultaba más bajo aun con
   un tamaño nominal parecido.
5. Letra y palo se centraron por caja, no por la huella visible del bloque.
6. Los recortes críticos se generaron después de presentar la cuadrícula.

## Regla ejecutable añadida

`Tools/assemble_canfield_fortuna_unit.py` detiene ahora la salida cuando:

- J, K, 10 y 7 no comparten una altura visible dentro de una tolerancia de dos píxeles;
- el 10 y los demás rangos no usan una única fuente y tamaño;
- Q/corazón, J/pica o K/corazón no conservan margen dentro de la zona reservada;
- el 10 no contiene diez posiciones canónicas;
- el 7 no contiene siete posiciones canónicas.

La Q conserva su descendente tipográfico, pero comparte altura de cuerpo con el resto. El
palo recibe una corrección óptica lateral dentro de la parte estrecha de la curva; todo el
bloque se rota 180 grados para la esquina inferior.

## Corrección adicional — 7 fuera de centro (2026-09-10)

Después de la primera corrección, la revisión del usuario detectó que el `7` seguía sin
centrarse verticalmente. La verificación numérica confirmó que comenzaba 30 píxeles por
debajo del borde de su carta, mientras el `10` comenzaba a 14 píxeles: una diferencia de
16 píxeles.

La causa era una condición literal: sólo `rank == "10"` elegía la zona numérica de
`71 × 60`; el `7`, al caer en la rama restante, recibía la zona alta de `66 × 93` diseñada
para bloques de figura con palo. El bloque se centraba matemáticamente dentro de la zona
equivocada.

La regla bloqueante pasa a ser estructural:

- `rango solo`, incluido `10` o `7`, usa el perfil numérico común;
- `rango + palo`, como Q/corazón, J/pica o K/corazón, usa el perfil compuesto;
- el generador afirma dimensiones y origen de la zona para un glifo ancho y otro estrecho;
- una comparación literal de un rango no puede decidir la geometría del perfil.

Esta reincidencia se consolidó como `UI-GEN-033` y se añadió al plan QA de familias de
assets repetidos.

La evidencia v3 queda supersedida por esta corrección. La nueva salida se guarda con versión
nueva para no presentar una lámina anterior como si fuera evidencia posterior:

- unidad corregida: `Logs/CanfieldDeckProposals/fortuna-nocturna-index-proof-v4.png`;
- recortes: `Logs/CanfieldDeckProposals/fortuna-nocturna-index-corners-v4.png`;
- escala de teléfono: `Logs/CanfieldDeckProposals/fortuna-nocturna-phone-scale-v2.png`.

## Corrección adicional — PASS circular del lote (2026-09-10)

La lámina conjunta v5 volvió a revelar índices y símbolos centrales desplazados aunque el
ensamblador declaraba `PASS`. El validador sólo comprobaba que cada grupo cupiera en la zona
que el propio generador había definido. El inventario copiaba la cantidad esperada en la
cantidad producida y asignaba `PASS` sin inspeccionar los píxeles.

La defensa nueva se consolidó como `UI-GEN-034`:

- `Tools/validate_canfield_six_themes.py` compara la huella renderizada con el centro del
  hueco claro medido en la fuente sin índices;
- para símbolos centrales mide tanto el promedio de componentes como el centro de sus
  límites exteriores;
- cuenta componentes reales del render y deriva de ellos el inventario;
- prueba una fixture válida y fixtures desplazadas, faltantes y adicionales;
- `Tools/assemble_canfield_six_themes.py` ejecuta esa auditoría en memoria y detiene la
  producción antes de escribir una lámina cuando hay fallos.

La lámina v5 queda como fixture negativa conocida y no puede reutilizarse como evidencia
final.

## Flujo obligatorio a partir de este caso

1. Elegir el marco con mayor riesgo.
2. Generar una única unidad sin datos exactos.
3. Definir el perfil normalizado de su abertura.
4. Componer rangos y palos mediante código.
5. Ejecutar las aserciones antes de guardar.
6. Revisar la carta completa y las esquinas ampliadas.
7. Aprobar esa unidad antes de producir el resto del lote.

## Evidencia

- Maestro sin índices:
  `Logs/CanfieldDeckProposals/fortuna-nocturna-index-master-v1.png`.
- Unidad corregida:
  `Logs/CanfieldDeckProposals/fortuna-nocturna-index-proof-v3.png`.
- Recortes ampliados:
  `Logs/CanfieldDeckProposals/fortuna-nocturna-index-corners-v3.png`.
- Ensamblador y validación:
  `Tools/assemble_canfield_fortuna_unit.py`.

# Guía común de calidad visual

Alcance: cualquier motor o plataforma cuando exista una interfaz visual evaluable.

## Referencias

- Comparar contra la autoridad exacta, no contra memoria o un ejemplo parecido.
- Medir bloques, centros, márgenes, ocupación interior y jerarquía antes del detalle.
- Distinguir composición, tipografía, color, assets, estados y movimiento.
- Registrar las adaptaciones autorizadas y su razón.

## Assets

- Conservar proporción, alfa, silueta, material, iluminación y nivel de detalle.
- Validar cada recurso a su tamaño final y sobre el fondo real.
- No confundir una cuadrícula dibujada con transparencia.
- No usar transformaciones para intentar integrar un recurso cuya perspectiva o luz no
  pertenece a la referencia.
- Separar arte estático, capas animables, datos y superficies interactivas.

## Tipografía y contenido

- Definir categorías, tamaños ideales y mínimos, líneas máximas y estrategia de desborde.
- Probar el idioma más largo, cifras extremas y todos los indicadores simultáneos.
- No resolver automáticamente el exceso de contenido reduciendo la fuente.
- Mantener una única ubicación principal para cada dato salvo necesidad explícita.
- Centrar los bloques compuestos por su huella visible completa, no sólo por los
  `RectTransform` de sus hijos. En una unidad `símbolo + rótulo + número`, primero se centra
  el conjunto dentro del panel y después se ajustan sus separaciones internas.
- En filas o hitos repetidos, usar una retícula común y comprobar en el render final el eje
  óptico del símbolo, el centro vertical del texto y la línea visual del valor. Márgenes
  transparentes, glifos, kerning y cifras de distinto ancho pueden desplazar elementos cuyas
  coordenadas matemáticas parecen iguales.

## Estados

- Definir qué cambia y qué permanece fijo en cada estado.
- Aplicar selección, bloqueo, error y completado sólo a las capas autorizadas.
- Explicar todo bloqueo que afecte una acción visible.
- Diseñar los estados vacíos para que no parezcan contenido roto.

## Movimiento

- Definir capa, propiedad, pivote, amplitud, duración, inicio, fin y elementos inmóviles.
- Validar la salida renderizada, no sólo valores internos.
- El movimiento refuerza información o ambiente sin desplazar texto ni composición.

## Medición de fidelidad

El proyecto define el umbral y el método. Una evaluación sólida separa:

1. Composición y proporciones.
2. Posición y alineación.
3. Tipografía y legibilidad.
4. Color, contraste e iluminación.
5. Assets, silueta y detalle.
6. Estados e interacción visible.
7. Movimiento, si corresponde.

Un promedio no puede ocultar la ausencia de una pieza esencial. Las diferencias aceptadas
se excluyen o ponderan sólo después de registrarse explícitamente.

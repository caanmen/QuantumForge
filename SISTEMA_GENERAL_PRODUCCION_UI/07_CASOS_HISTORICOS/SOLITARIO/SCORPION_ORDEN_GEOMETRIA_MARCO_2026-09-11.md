# Caso histórico: Scorpion, contenido universal antes del marco

Fecha: 2026-09-11  
Alcance reutilizable: barajas y otras familias gráficas con contenido universal dentro de
una envolvente temática.

## Problema observado

En Luna Venenosa se creó primero un marco decorativo y se dejó un hueco aproximado para los
índices. Al componer después rangos y palos reales, los glifos anchos rozaron o salieron del
hueco, los símbolos tuvieron que reducirse y algunas siluetas compuestas parecieron partidas.
El marco era visualmente atractivo, pero no había sido diseñado contra el contrato real del
contenido que debía alojar.

## Regla consolidada

La dependencia funcional se resuelve antes que la decoración. Si ya existe un diseño
universal, primero se inventaría, mide y bloquea. Después se definen el campo útil y las zonas
reservadas con las fixtures extremas a escala final. Sólo tras obtener `PASS` se diseña el
marco alrededor de esa geometría.

En cartas, el núcleo universal comprende conteos, topología, orientación y distribución
relativa de A–10, además del inventario de rangos y palos. El tema puede variar marco, fondo,
ornamentos, figuras, fuente y correcciones ópticas medidas, pero no debe encoger ni deformar
el núcleo universal para rescatar un marco tardío.

## Orden obligatorio

1. Declarar propietarios de capas y qué componentes universales ya existen.
2. Medir las combinaciones extremas a resolución y escala de consumo.
3. Bloquear campo útil, bolsillos de índice, márgenes y máscaras.
4. Renderizar una prueba geométrica limpia sin decoración.
5. Validar todas las variantes de contenido contra esa geometría.
6. Diseñar el marco alrededor de las zonas ya aprobadas.
7. Volver a medir el hueco visible del marco mediante una auditoría independiente.
8. Añadir arte y tipografía del tema sin alterar las invariantes universales.
9. Aprobar una unidad representativa antes del lote.
10. Producir y validar el inventario completo en su contexto final.

Para índices de cartas, la prueba previa al marco incluye como mínimo `10`, `Q`, `K` y un
rango estrecho con los cuatro palos, además de ambas esquinas rotadas. Para cartas numéricas,
la cobertura sigue siendo A–10 por cuatro palos; una muestra visual no reduce el inventario.

## Bloqueos

- Marco creado antes de la prueba geométrica: `FAIL`.
- Número o símbolo universal reducido para caber en el marco: `FAIL`.
- Ornamento que cruza una zona reservada: `FAIL`.
- Excepción temática sin perfil medido: `FAIL`.
- Lote antes de aprobar la unidad representativa: `FAIL`.

La autoridad ejecutable está en
`04_PLANTILLAS/CONTRATO_ORDEN_PRODUCCION_GRAFICA.json` y su presencia se verifica mediante
`05_QA/validate_card_layout_rules.py`.

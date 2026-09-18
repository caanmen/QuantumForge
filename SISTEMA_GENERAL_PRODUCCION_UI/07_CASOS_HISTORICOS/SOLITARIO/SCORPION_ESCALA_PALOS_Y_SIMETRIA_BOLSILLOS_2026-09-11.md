# Caso histórico: Scorpion, escala visible y simetría real de bolsillos

Fecha: 2026-09-11  
Alcance reutilizable: índices de barajas nuevas o modificadas y marcos con dos huecos de
esquina en doble orientación.

## Problema observado

La unidad Luna Venenosa pasó una prueba que verificaba que los índices cabían en rectángulos
declarados, pero el palo seguía viéndose pequeño a escala teléfono. La medición real mostró
que el diseño universal anterior también usaba aproximadamente `6.15 %` del ancho y que la
adaptación de Scorpion usaba `6.25 %`; por tanto, no era una reducción exclusiva del tema,
sino una debilidad del estándar visual existente.

Además, el marco declaraba geometría superior/inferior simétrica, pero los huecos realmente
pintados diferían entre `9` y `15 px` de altura en la carta normalizada a 512 × 768. Validar
sólo las coordenadas ideales ocultó la desigualdad visible.

## Regla consolidada

- Para temas nuevos o modificados, el palo del índice tiene una altura de tinta visible
  mínima del `8.4 %` del ancho de la carta, redondeada al píxel más cercano.
- El marco puede disminuir su hueco sólo mientras la envolvente universal completa y su
  margen obligatorio sigan cabiendo; nunca se reduce el contenido para salvar decoración.
- Los huecos realmente pintados se detectan en el render. Ancho y alto superior/inferior
  pueden diferir como máximo `1 px`.
- El hueco inferior debe corresponder al superior tras rotación exacta de 180 grados.
- Una fixture con palo reducido y una fixture con huecos desiguales deben devolver `FAIL`.
- Los temas anteriores se migran por separado; esta regla no permite declararlos actualizados
  sin evidencia nueva.

La autoridad declarativa está en
`04_PLANTILLAS/CONTRATO_ORDEN_PRODUCCION_GRAFICA.json` y
`04_PLANTILLAS/PERFIL_CANONICO_INDICES_COMPUESTOS.json`. La defensa ejecutable general está
en `05_QA/validate_card_layout_rules.py`; cada tema añade una medición independiente de su
arte final.

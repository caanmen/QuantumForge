# Bitácora de aprendizajes UI

Este registro garantiza que los errores y técnicas descubiertos durante una pantalla
se conviertan en mejoras permanentes del proceso.

## Regla de uso

- Registrar solamente hallazgos comprobados durante el trabajo real.
- Indicar si corresponde a ERROR, TIP o ambos.
- Incorporar el contenido generalizable a la guía correspondiente antes de marcarlo
  como cerrado.
- No convertir una decisión visual exclusiva de una pantalla en regla universal.
- Las decisiones específicas permanecen en `10_PANTALLAS/<DIMENSIÓN>/<PANTALLA>`.
- Registrar `Alcance generalizable` y `No generalizar` para separar el principio de su
  caso de origen.

## Plantilla de entrada

### AAAA-MM-DD — Dimensión / pantalla — Título breve

- Tipo: ERROR | TIP | ERROR Y TIP
- Alcance generalizable:
- No generalizar:
- Situación observada:
- Causa o explicación confirmada:
- Solución o método comprobado:
- Evidencia o prueba:
- Guía y sección actualizadas:
- Lista de control actualizada: SÍ | NO | NO APLICA
- Estado: PENDIENTE | INCORPORADO

## Entradas

### 2026-09-07 — Dimensión 3 / Planta de Producción — Una regla escrita necesita una puerta de evidencia

- Tipo: ERROR Y TIP
- Alcance generalizable: correcciones visuales puntuales señaladas mediante recortes por el
  usuario y cierre perceptual de cualquier pantalla.
- No generalizar: coordenadas, textos, botones, estilo industrial ni compensaciones ópticas de
  Planta de Producción D3.
- Situación observada: ya existían reglas sobre centro óptico, zona útil y separación del PASS,
  pero varias respuestas declararon corregido un detalle después de revisar la pantalla completa.
  Los recortes posteriores del usuario demostraron que aún quedaban errores en el otro eje o en
  uno de los márgenes.
- Causa o explicación confirmada: las reglas describían qué mirar, pero no bloqueaban el cierre
  ni exigían un artefacto de evidencia específico antes de responder. La vista completa reducida
  ocultó diferencias que eran evidentes dentro del marco ampliado.
- Solución o método comprobado: crear una puerta obligatoria por defecto señalado: captura nueva,
  recorte con marco completo, revisión separada X/Y, comparación de cuatro márgenes y reapertura
  automática si el usuario repite la observación. El PASS técnico permanece separado.
- Evidencia o prueba: iteraciones V18–V22 de Planta; los recortes del usuario detectaron
  contención vertical y centro óptico que la vista completa y el PASS no cerraban.
- Guía y sección actualizadas: Instrucciones maestras Puertas 2 y 5; Creación 78; Prevención 91.
- Lista de control actualizada: SÍ
- Estado: INCORPORADO

### 2026-09-05 — Dimensión 3 / Planta de Producción — Zona útil y centro óptico compuesto

- Tipo: ERROR Y TIP
- Alcance generalizable: títulos dentro de marcos ornamentales, placas anidadas y grupos de
  icono + texto construidos en Unity.
- No generalizar: coordenadas, desplazamientos, tamaños, textos, iconos ni estética industrial
  específicos de Planta de Producción D3.
- Situación observada: varias iteraciones compilaban y daban PASS, pero los títulos seguían
  altos, las placas de asignación y colas parecían salir de sus fondos y la navegación inferior
  no se percibía centrada.
- Causa o explicación confirmada: se validaban RectTransform exteriores en lugar de la zona
  útil delimitada por los biseles; un texto tenía padding asimétrico; y los iconos se centraban
  con el espacio transparente de sus recortes, no con su silueta visible.
- Solución o método comprobado: medir marco, zona útil y huella por separado; reducir y centrar
  la placa completa con márgenes simétricos; recortar cada icono a su materia visible; centrar
  icono + texto como una unidad con compensación óptica por asset; revisar X e Y en pasadas
  separadas mediante recortes ampliados de una captura nueva.
- Evidencia o prueba: `ProductionFloor_candidate_1080x1920.png`,
  `ProductionFloor_candidate_720x1280.png` y
  `dimension3_production_floor_visual_alignment_v17_full_validation.log`, todos generados desde
  Main real y con PASS. V17 bajó la navegación; V18 redujo y recentró +1 ASIGNAR; V19 aseguró
  su contención vertical, V20 equilibró ópticamente sus márgenes y V21 compensó el peso visual
  de la etiqueta −1 RETIRAR sin mover su marco; V22 ajustó conjuntamente X e Y tras revisar el
  marco completo. Aprobación perceptual final todavía pendiente.
- Guía y sección actualizadas: Creación 78; Prevención 91; decisiones específicas de Planta.
- Lista de control actualizada: SÍ
- Estado: INCORPORADO

### 2026-09-04 — Dimensión 2 / cierre — Centro óptico compuesto y cierre documental atómico

- Tipo: ERROR Y TIP
- Alcance generalizable: bloques informativos formados por símbolo, rótulo y cifra; cierre
  de pantallas o familias con varias fuentes de trazabilidad.
- No generalizar: textos, iconos, colores, coordenadas, métricas ni estética de Dimensión 2.
- Situación observada: en Analizar Restos el bloque de recurso quedaba descentrado; en
  Investigación del Ente los porcentajes parecían altos dentro de sus círculos; y en Archivo
  los textos de coste no compartían eje con sus símbolos. Al cerrar la dimensión, varias
  fichas y la matriz todavía conservaban estados de candidata aunque la evidencia ya estaba
  validada y aprobada.
- Causa o explicación confirmada: se habían centrado cajas individuales en vez de la huella
  visible del conjunto, y la aprobación no estaba tratada como una actualización coordinada
  de todas sus fuentes documentales.
- Solución o método comprobado: centrar cada unidad `símbolo + texto + número` por una
  retícula y un eje óptico comunes, validar el render final en ambas resoluciones y, al
  aprobar, sincronizar ficha, matriz, registro y archivo de capturas dentro del mismo cierre.
- Evidencia o prueba: capturas aprobadas de `ANALIZAR_RESTOS`,
  `ARCHIVO_MEJORAS_PERMANENTES` e `INVESTIGACION_DEL_ENTE`; matriz de 20 vistas y documento
  `CIERRE_DIMENSION_2_2026-09-04.md` sin estados pendientes.
- Guía y sección actualizadas: guía común de calidad visual / Tipografía y contenido;
  flujo general / Evidencia y aprendizaje; plan maestro QA / Cierre; catálogo UI-GEN-013 y
  UI-GEN-016; antipatrones.
- Lista de control actualizada: SÍ
- Estado: INCORPORADO

### 2026-08-19 — Sistema general — Planear la animación antes de Unity sin implementarla temprano

- Tipo: TIP
- Alcance generalizable: toda pantalla debe definir intención, capas, pivotes, estados y
  elementos inmóviles durante el diseño, antes de producir los assets finales.
- No generalizar: movimientos, duraciones, efectos o capas concretas de otra pantalla.
- Situación observada: esperar hasta Unity para pensar la animación puede obligar a
  rehacer imágenes aplanadas o mover rígidamente elementos que necesitaban capas.
- Causa o explicación confirmada: el arte y el movimiento comparten requisitos de
  separación, margen, pivote, recorte y resolución.
- Solución o método comprobado: planificar la animación antes de Unity, aprobar después
  la composición estática e implementar el movimiento únicamente en su puerta posterior.
- Evidencia o prueba: instrucciones maestras, guía, plantillas y plan QA actualizados.
- Guía y sección actualizadas: creación correcta 35; puertas de aprobación.
- Lista de control actualizada: SÍ
- Estado: INCORPORADO

### 2026-08-19 — Dimensión 1 / Reliquias — Colección paginada sin duplicar tarjetas

- Tipo: TIP Y PREVENCIÓN
- Alcance generalizable: colecciones visuales que superan la cantidad de tarjetas de
  una página y contienen una última página incompleta.
- No generalizar: tres páginas, ocho tarjetas, orden de reliquias, paleta ni distribución
  concreta fuera de la Cámara de Reliquias D1.
- Situación observada: debían incorporarse doce piezas adicionales sin alterar la página
  de referencia ni crear veinte tarjetas permanentes con estados duplicados.
- Causa o explicación confirmada: duplicar la jerarquía por página multiplica propietarios,
  raycasts y riesgo de conservar selección o adornos de otro contenido.
- Solución o método comprobado: reutilizar ocho ranuras estables, cambiar sus datos y
  sprites por página, desactivar ranuras sobrantes, centrar las cuatro finales y limpiar
  adornos exclusivos al reasignar cada ranura. Los valores todavía no definidos se
  comunican como pendientes y nunca como una bonificación de cero.
- Evidencia o prueba: instalación PASS con 20 sprites y 3 páginas; recorrido automático
  1 → 2 → 3 → 2 → 1; capturas 1080×1920 y 720×1280 del 19 de agosto de 2026.
- Guía y sección actualizadas: creación correcta 61; prevención de errores 21.
- Lista de control actualizada: SÍ
- Estado: INCORPORADO

### 2026-08-19 — Dimensión 1 / Cámara de Reliquias — Colección sólida por objeto y alfa verificable

- Tipo: ERROR Y TIP
- Alcance generalizable: cuadrículas de objetos volumétricos que deben mantener una
  identidad individual y compartir un acabado visual coherente.
- No generalizar: las ocho siluetas, la paleta D1, los tiers ni el contenido concreto
  de las reliquias a pantallas de otras dimensiones.
- Situación observada: la referencia exigía ocho piezas tridimensionales distintas y el
  generador devolvió fondos claros horneados aunque se pidió transparencia.
- Causa o explicación confirmada: usar la pantalla completa habría mezclado identidades;
  además, la apariencia cuadriculada no garantizaba un canal alfa real.
- Solución o método comprobado: crear ocho recortes individuales, generar cada pieza por
  separado, validar el conjunto sobre el fondo real, limpiar el matte con una herramienta
  reproducible y revisar tanto 1080×1920 como 720×1280. La segunda resolución motivó
  aumentar peso y tamaño de tier y estado.
- Evidencia o prueba: instalación y captura PASS; capturas candidatas 1080×1920 y
  720×1280 archivadas en 10_PANTALLAS/DIMENSION_1/RELIQUIAS.
- Guía y sección actualizadas: creación correcta 61.
- Lista de control actualizada: SÍ
- Estado: INCORPORADO

### 2026-08-18 — Sistema general — Aprendizaje continuo entre pantallas

- Tipo: TIP
- Alcance generalizable: todo trabajo UI debe trasladar sus hallazgos comprobados al
  sistema permanente antes del cierre.
- No generalizar: contenido, estética o decisiones exclusivas de la pantalla de origen.
- Situación observada: las guías ya reunían aprendizajes anteriores, pero no existía
  una regla explícita que obligara a mantenerlas después de cada pantalla.
- Causa o explicación confirmada: un hallazgo podía quedar únicamente dentro del chat.
- Solución o método comprobado: registrar cada aprendizaje aquí e incorporarlo a la
  guía y lista de control correspondientes antes de cerrar la pantalla.
- Evidencia o prueba: instrucciones maestras y ambas guías actualizadas.
- Guía y sección actualizadas: mantenimiento obligatorio en las dos guías.
- Lista de control actualizada: SÍ
- Estado: INCORPORADO

### 2026-08-19 — Dimensión 1 / Explorar — Cobertura completa del overlay

- Tipo: ERROR Y TIP
- Alcance generalizable: cualquier pantalla que sustituya visualmente otra interfaz debe
  cubrir el viewport autorizado y conservar su contenido dentro de la Safe Area.
- No generalizar: navegación, paleta, nombres y composición propios de Explorar D1.
- Situación observada: una franja de la interfaz anterior permanecía visible en el
  borde derecho de la captura aunque Explorar estaba abierta.
- Causa o explicación confirmada: el fondo sólo cubría el rectángulo interno heredado
  del panel y no alcanzaba los bordes reales del viewport.
- Solución o método comprobado: extender el fondo bloqueante hasta todo el viewport,
  conservar el contenido dentro de la Safe Area y revisar los cuatro bordes.
- Evidencia o prueba: captura 1080 × 1920 sin sangrado lateral.
- Guía y sección actualizadas: prevención de errores 46.
- Lista de control actualizada: SÍ
- Estado: INCORPORADO

### 2026-08-19 — Dimensión 1 / Explorar — Ocupación interior del contenido

- Tipo: TIP
- Alcance generalizable: medir contenido/panel además de medir el panel completo.
- No generalizar: tamaños, ilustraciones y distribución concretos de Explorar D1.
- Situación observada: los paneles coincidían en geometría, pero la nave, el dron,
  los metales y varios iconos se veían más pequeños que en la referencia.
- Causa o explicación confirmada: se habían medido los marcos sin comparar la
  proporción ocupada por cada ilustración y texto dentro de ellos.
- Solución o método comprobado: medir contenido/panel, escalar primero y reposicionar
  después, comprobando de nuevo los textos dinámicos para evitar recortes.
- Evidencia o prueba: captura final de Explorar a 1080 × 1920.
- Guía y sección actualizadas: creación correcta 55; prevención de errores, checklist.
- Lista de control actualizada: SÍ
- Estado: INCORPORADO

### 2026-08-18 — Sistema general — Eficiencia recomendada por Unity

- Tipo: TIP
- Alcance generalizable: medir Canvas, raycasts, reutilización, importación y rendimiento
  con criterios repetibles apropiados para cada pantalla.
- No generalizar: una jerarquía o división de Canvas sin perfilar el caso actual.
- Situación observada: el proceso interno cubría fidelidad y estabilidad, pero podía
  concretar mejor la automatización de importación, los raycasts, Canvas, atlas,
  variantes, simulación de dispositivos y líneas base de rendimiento.
- Causa o explicación confirmada: estas prácticas reducen trabajo manual y permiten
  medir reconstrucciones, interacción y coste gráfico con criterios repetibles.
- Solución o método comprobado: incorporar las recomendaciones de forma gradual y
  sólo generalizar componentes después de contar con casos aprobados.
- Evidencia o prueba: documentación oficial de Unity registrada en
  00_INICIO/RECOMENDACIONES_OFICIALES_UNITY_UI.md.
- Guía y sección actualizadas: guía de creación, secciones 46 a 53.
- Lista de control actualizada: SÍ
- Estado: INCORPORADO

### 2026-08-18 — Dimensión 1 / Explorar — Origen local de gráficos técnicos

- Tipo: ERROR Y TIP
- Alcance generalizable: declarar y convertir explícitamente el sistema de coordenadas de
  cualquier gráfico construido dentro de un RectTransform.
- No generalizar: vértices, formas o posiciones específicas de los vehículos D1.
- Situación observada: contador, nave y sondas se salían de sus paneles durante la
  primera captura aunque las medidas provenían de la referencia.
- Causa o explicación confirmada: las medidas se expresaban desde la esquina superior
  izquierda, mientras los componentes de líneas interpretaban vértices desde el centro.
- Solución o método comprobado: convertir las medidas al espacio local centrado,
  encapsular cada gráfico y verificar sus extremos en una captura 1080 × 1920.
- Evidencia o prueba: `Logs/VisualQA/Dimension1/ExploreReference/Explore_reference_1080x1920.png`.
- Guía y sección actualizadas: creación 54; prevención de errores 45.
- Lista de control actualizada: SÍ
- Estado: INCORPORADO

### 2026-08-19 — Dimensión 1 / Explorar — La topología precede al detalle

- Tipo: ERROR Y TIP
- Alcance generalizable: validar silueta, proporción y masas principales antes del detalle
  interno de cualquier ilustración.
- No generalizar: las siluetas de nave, dron o sonda ni el lenguaje tecnológico D1.
- Situación observada: añadir más líneas a la nave, el dron extractor y la sonda
  analítica no resolvió la diferencia; seguían siendo las mismas formas rechazadas.
- Causa o explicación confirmada: la topología base no coincidía con la referencia:
  dardo estrecho en vez de caza ancho, moño en vez de dron de cuatro palas y ojo
  orbital en vez de boya reticulada.
- Solución o método comprobado: medir contorno, centro y relación ancho/alto, sustituir
  por completo las tres siluetas y sólo después incorporar sus capas técnicas.
- Evidencia o prueba: capturas 1080 × 1920 y 720 × 1280 generadas el 19 de agosto;
  instalación y captura determinista con PASS.
- Guía y sección actualizadas: creación correcta 55; prevención de errores 37.
- Lista de control actualizada: SÍ
- Estado: INCORPORADO

### 2026-08-19 — Dimensión 1 / Explorar — El medio gráfico también define la fidelidad

- Tipo: ERROR Y TIP
- Alcance generalizable: elegir geometría, sprite, material u otro medio según el acabado
  exigido por la referencia y comprobarlo al tamaño real.
- No generalizar: usar siempre sprites rellenos, usar siempre líneas UGUI ni copiar la
  paleta cyan de Explorar D1. Hangar D1 demuestra que una referencia puede exigir líneas.
- Situación observada: aun después de corregir las siluetas, la nave y el dron fueron
  rechazados porque continuaban pareciendo esqueletos de líneas.
- Causa o explicación confirmada: el trazado UGUI representaba contornos y divisiones,
  pero no las superficies rellenas, sombras y volumen que daban identidad al diseño.
- Solución o método comprobado: crear ilustraciones sólidas con alfa, probar la primera
  generación en el tamaño móvil real y realizar una segunda versión con menos detalle
  fino, masas cyan más grandes y mejor lectura. Después se verificó el alfa, se importó
  sin compresión destructiva y se integró como sprite candidato, sin alterar la sonda
  analítica que ya había mejorado.
- Evidencia o prueba: capturas 1080 × 1920 y 720 × 1280 con instalación y captura PASS.
- Guía y sección actualizadas: creación correcta 56; prevención de errores 47.
- Lista de control actualizada: SÍ
- Estado: INCORPORADO

### 2026-08-19 — Dimensión 1 / Hangar — Captura aislada, raíz alineada y barras reales

- Tipo: ERROR Y TIP
- Alcance generalizable: overlays anidados en paneles provisionales, modales creados en
  runtime, capturas a RenderTexture y barras sólidas sin sprite.
- No generalizar: la compensación vertical concreta, siluetas de naves ni valores del
  escenario de referencia del Hangar D1.
- Situación observada: el encabezado se recortó por el desplazamiento del padre, el
  informe de ausencia cubrió la primera captura y las cuatro barras parecían llenas.
- Causa o explicación confirmada: HangarPanel no coincidía con el centro del viewport;
  el informe se instanciaba después de cargar el guardado; Image.Type.Filled no recortó
  las Images sin sprite.
- Solución o método comprobado: compensar una sola raíz, consumir y desactivar modales
  runtime durante QA, actualizar/renderizar dos veces y expresar la barra mediante
  anchorMax.x.
- Evidencia o prueba: captura candidata 1080 × 1920 y 720 × 1280 con PASS del 19 de agosto.
- Guía y sección actualizadas: creación correcta 57 a 59; prevención 48 a 50.
- Lista de control actualizada: SÍ
- Estado: INCORPORADO

### 2026-08-19 — Dimensión 1 / Hangar — Fidelidad de planos técnicos por objeto

- Tipo: ERROR Y TIP
- Alcance generalizable: ilustraciones blueprint o monocromáticas que se reutilizan en
  vista pequeña y ampliada.
- No generalizar: siluetas concretas, paleta cyan/ámbar ni el shader de extracción a
  dimensiones o pantallas cuya referencia use otro acabado.
- Situación observada: la primera versión de las cuatro naves respetaba marcos y medio
  técnico, pero fue rechazada por verse demasiado simple y genérica.
- Causa o explicación confirmada: se dibujaron contornos procedurales con pocas capas;
  no se comparó por nave la distribución de módulos ni la densidad mecánica interior.
- Solución o método comprobado: recortar cada vehículo de la referencia, reconstruirlo
  como sprite independiente de alta densidad, probarlo en tarjeta y vista ampliada, y
  eliminar el fondo horneado mediante un material común que mantiene el tint de estado.
- Evidencia o prueba: instalación, captura y validación PASS; capturas 1080×1920 y
  720×1280 del 19 de agosto sin rectángulos ni granulado residual.
- Guía y sección actualizadas: creación correcta 60; prevención de errores 51.
- Lista de control actualizada: SÍ
- Estado: INCORPORADO

### 2026-08-19 — Dimensión 1 / Árbol Cuántico — Orden correcto del aislamiento QA

- Tipo: ERROR Y TIP
- Alcance generalizable: capturas de overlays anidados cuyos padres ejecutan OnEnable y
  escenas que contienen varias instancias con el mismo nombre.
- No generalizar: la compensación vertical concreta del Árbol, sus nodos, colores ni el
  escenario de tres puntos.
- Situación observada: la navegación global reaparecía después de ocultarla; al ocultar
  todas las coincidencias genéricas, HUD apagó también la pantalla objetivo.
- Causa o explicación confirmada: activar los padres disparó TabsUI después del primer
  aislamiento; además HUD es ancestro real de VerticalUIRoot y no un overlay ajeno.
- Solución o método comprobado: activar primero toda la cadena de la raíz, preservar sus
  ancestros y aplicar al final el aislamiento de todas las raíces globales específicas.
- Evidencia o prueba: `Dimension1TreeReferenceCapture.Run` generó 1080×1920 y 720×1280
  con `[D1 Tree Capture] PASS` el 19 de agosto de 2026.
- Guía y sección actualizadas: prevención de errores 52.
- Lista de control actualizada: SÍ
- Estado: INCORPORADO

### 2026-08-19 — Sistema UI general — Reutilizar antes de crear y no simplificar

- Tipo: ERROR Y MÉTODO GENERAL
- Alcance generalizable: todas las dimensiones, pantallas, modales, iconos, fondos,
  componentes, prefabs, materiales, temas y generadores de Quantum Forge.
- No generalizar: paleta, marcos, iconos, cristal, órbitas ni composición de D1.
- Situación observada: la primera implementación del Árbol Cuántico empezó a dibujar
  conceptos ya existentes y redujo un diseño detallado a pictogramas y elipses simples.
- Causa o explicación confirmada: faltó una auditoría concepto por concepto antes de
  construir; el catálogo incompleto se interpretó como ausencia de assets.
- Solución o método comprobado: crear una Puerta 0 global; buscar en catálogo y proyecto
  real; completar una matriz concepto-origen-ruta-estado-decisión; reutilizar primero y
  justificar cada faltante; prohibir simplificaciones silenciosas del diseño aprobado.
- Evidencia o prueba: comparación lado a lado entre la referencia orbital permanente y
  las capturas archivadas como `CAPTURA_RECHAZADA_FIDELIDAD_E_ICONOS_*`.
- Guía y sección actualizadas: instrucciones maestras Puerta 0; creación correcta 62;
  prevención de errores 53; plantillas y catálogo.
- Lista de control actualizada: SÍ
- Estado: INCORPORADO

## 2026-08-19 — Reutilizar el tratamiento completo del asset

- Un PNG reutilizado puede requerir su material, shader o configuración de importación.
- El blueprint canónico de Hangar necesitó `d1_hangar_blueprint_keyed.mat`; el sprite
  aislado produjo un fondo claro incorrecto en la primera captura de reconstrucción.
- Las imágenes generadas deben comprobarse por canal alfa. El patrón ajedrezado visible
  puede estar horneado y no representar transparencia real.
- El instalador del Árbol configura de forma reproducible el candidato de Cartografía
  como sprite y la captura se repitió después de cada corrección.

## 2026-08-19 — Dimensión 1 — Armazón único y QA aislado no equivale a partida real

- Tipo: ERROR Y MÉTODO GENERAL.
- Alcance generalizable: familias de pantallas que comparten marco, Safe Area, cabecera
  o navegación persistente.
- Situación observada: Centro de Mando, Galaxia, Explorar, Hangar, Reliquias y Árbol
  guardaban raíces, marcos y barras inferiores con medidas distintas. Además, la
  captura del Árbol ocultaba la navegación global y por ello no mostraba el
  solapamiento que sí aparecía durante el juego.
- Causa confirmada: cada instalador era propietario de sus propios números y la prueba
  de captura aislada se interpretó como validación funcional.
- Solución aplicada: crear `Dimension1SharedLayoutTokens`, sincronizar los instaladores
  activos y aplicar una migración localizada que sólo modifica raíces, marcos y barras
  existentes. Separar desde ahora captura de fidelidad y prueba de navegación real.
- Evidencia: `[D1 Shared Shell] APPLY_PASS`, `[D1 Tree Capture] PASS` y
  `[D1 Explore Capture] PASS`; ambas capturas nuevas usan marco y navegación idénticos.
- Guía actualizada: prevención de errores 55 y 56.
- Estado: INCORPORADO; interacción del Árbol validada el 19 de agosto de 2026.

## 2026-08-19 — Dimensión 1 / Árbol Cuántico — La apariencia de botón no prueba interacción

- Tipo: ERROR Y MÉTODO GENERAL.
- Alcance generalizable: pantallas generadas por editor que comienzan como composición
  estática y después reciben interacción.
- Situación observada: Centro y las cinco tarjetas inferiores del Árbol parecían
  controles completos, pero no respondían; simultáneamente la navegación global podía
  cubrir la navegación local.
- Causa confirmada: la raíz no tenía GraphicRaycaster, el CanvasGroup bloqueaba raycasts
  y las tarjetas eran paneles sin Button ni listeners persistentes.
- Solución aplicada: añadir `Dimension1TreeNavigationUI`, convertir únicamente Centro y
  la navegación inferior en botones, suprimir las raíces globales mientras el Árbol está
  abierto y restaurarlas al cerrarlo. La escena actual y su instalador quedaron
  sincronizados sin reconstruir nodos, órbitas ni el detalle seleccionado.
- Evidencia: `[D1 Tree Interaction] APPLY_PASS`, `[D1 Tree Navigation Runtime] PASS` para
  Centro, Galaxia, Explorar, Hangar y Reliquias, y capturas 1080×1920/720×1280 con
  `[D1 Tree Capture] PASS`.
- Guía actualizada: prevención de errores 57 y lista de control.
- Estado: INCORPORADO.

## 2026-08-19 — Dimensión 1 — Iconos premium y recorte que conserva color

- Tipo: APROBACIÓN DE ASSET Y MÉTODO GENERAL.
- Alcance generalizable: ilustraciones detalladas con fondo uniforme que se presentan
  como sprites UI; los cinco diseños concretos siguen siendo exclusivos de D1.
- Situación observada: los iconos inferiores simples no mantenían el nivel de detalle
  de Hangar, Reliquias ni de la referencia. Las primeras exportaciones premium tenían
  un tablero horneado y el material de blueprint existente eliminaba también el color.
- Solución aplicada: conservar el arte aprobado, regenerar únicamente el fondo como
  negro uniforme, crear un black key que preserva RGB y una variante ámbar seleccionada,
  y centralizar su aplicación en `Dimension1PremiumNavigationApply` para las seis
  pantallas D1.
- Evidencia: `[D1 Premium Navigation] APPLY_PASS`, capturas PASS de Árbol en
  1080×1920/720×1280, capturas PASS de Centro y Galaxia, y regresión posterior
  `[D1 Tree Navigation Runtime] PASS`.
- Catálogo actualizado: cinco `d1_nav_*_premium_v1.png` canónicos D1; los v3 simples
  quedan sustituidos.
- Guía actualizada: prevención de errores 58 y lista de control.
- Estado: INCORPORADO.

## 2026-08-19 — Árbol Cuántico — Bloqueo de UGUI sólo visible con clic real

- Tipo: ERROR Y MEJORA DE QA.
- Alcance generalizable: overlays que se activan desde Button y desactivan otras raíces
  con Selectable durante OnEnable.
- Situación observada: Unity y el PlayerLoop seguían funcionando, no había excepciones,
  pero toda la interfaz dejó de responder después de pulsar Árbol en el juego real.
- Causa confirmada: `Dimension1TreeNavigationUI.OnEnable` ejecutaba inmediatamente
  `ApplyExclusiveNavigation(true)` dentro del ciclo del clic. La prueba anterior abría
  el panel llamando directamente al método y no ejercitaba `OnPointerClick`.
- Solución aplicada: diferir la exclusividad un Update y ampliar la validación para
  pulsar la tarjeta `Nav_ÁRBOL` mediante EventSystem/PointerEventData; luego pulsar Centro,
  Galaxia, Explorar, Hangar y Reliquias dejando fotogramas entre rutas.
- Evidencia: `[D1 Tree Navigation Runtime] PASS` en
  `Logs/d1_tree_real_click_runtime_fix.log`, sin excepciones ni bloqueo de controles.
- Guía actualizada: prevención de errores 59 y lista de control.
- Estado: INCORPORADO.

## 2026-08-19 — Árbol Cuántico — ExecuteEvents directo no valida un raycast físico

- Tipo: CORRECCIÓN DE DIAGNÓSTICO Y MEJORA DE QA.
- Alcance generalizable: botones UGUI generados por editor y pruebas automáticas de
  navegación táctil.
- Situación observada: una prueba declaraba PASS y el juego continuaba inerte al entrar
  en Árbol; además varias raíces D1 podían solaparse durante el cambio de pantalla.
- Causa confirmada por inspección: `Button.targetGraphic` apuntaba al Fill construido con
  `raycastTarget=false`. La prueba enviaba ExecuteEvents directamente al GameObject y
  evitaba GraphicRaycaster. La supresión global usaba un booleano sin propietarios.
- Solución aplicada: reactivar el raycast sólo en la superficie táctil del Árbol,
  sincronizar escena e instalador, registrar la supresión por raíz y diferir la visibilidad
  física un fotograma. La prueba fija 1080×1920, exige RaycastAll y mide estabilidad de
  esquinas durante diez segundos.
- Evidencia final: `[D1 Tree Interaction] APPLY_PASS` y
  `[D1 Tree Navigation Runtime] PASS` en `Logs/d1_shared_navigation_runtime_pass.log`,
  con raycast físico y estabilidad de layout durante diez segundos.
- Guías actualizadas: creación correcta 38 y 47; prevención de errores 28, 57 y 59.
- Estado: INCORPORADO Y VALIDADO.

## 2026-08-19 — Dimensión 1 — Unificar tarjetas exige unificar también sus internos

- Tipo: DECISIÓN VISUAL Y MÉTODO GENERAL.
- Alcance generalizable: familias de pantallas con encabezado o navegación persistente;
  los valores visuales concretos pertenecen sólo a D1.
- Situación observada: tras igualar raíces y tarjetas, títulos y etiquetas todavía
  cambiaban de posición al navegar porque cada instalador conservaba tamaños de fuente,
  alturas, iconos y capas diferentes.
- Causa confirmada: el propietario común cubría el RectTransform exterior, pero no el
  contenido visible de los componentes compartidos.
- Solución aplicada: elegir Hangar como autoridad D1 y centralizar encabezado completo,
  cinco tarjetas inferiores, Rajdhani, iconos premium, marcos y selección en
  `Dimension1SharedShellApply`. Sus seis constructores reaplican el contrato. La banda
  propia de Árbol bajó 22 px para respetar el final del encabezado.
- Evidencia: `[D1 Shared Shell] APPLY_PASS`; compilación con 0 errores; capturas nuevas
  PASS de las seis pantallas, incluida comprobación 1080×1920/720×1280; y
  `[D1 Tree Navigation Runtime] PASS` con clic físico y estabilidad de diez segundos.
- Guía actualizada: prevención de errores 56 y 60; perfil D1 y registro de decisiones.
- Estado: INCORPORADO.

## 2026-08-19 — Explorar D1 — El contenido estaba centrado en 502 y no en 540

- Tipo: ERROR DE GEOMETRÍA Y MÉTODO GENERAL.
- Situación observada: el título y la navegación de Explorar estaban centrados, pero el
  conjunto de escáner, destino, nave, apoyo, expedición y acciones se veía desplazado.
- Causa confirmada: el grupo ocupaba x=24..980; su centro geométrico era 502 mientras
  el lienzo 1080×1920 exige 540.
- Solución aplicada: trasladar los siete bloques 38 px a la derecha, conservar tamaños
  y distancias internas, y sincronizar escena, constructor y normalizador compartido.
- Evidencia: compilación con 0 errores, `[D1 Shared Shell] APPLY_PASS` y
  `[D1 Explore Capture] PASS` en 1080×1920 y 720×1280.
- Guía actualizada: prevención de errores 61 y lista de control.
- Estado: INCORPORADO.

## 2026-08-19 — Inventario de Metales D1 — Contrato de subpantalla y entrada compatible

- Tipo: NUEVA PANTALLA, ERROR DE GEOMETRÍA Y ERROR DE EJECUCIÓN.
- Alcance generalizable: subpantallas superpuestas, accesos compartidos y proyectos
  móviles configurados únicamente con Input System.
- Situación observada: la primera captura recortaba 82 px superiores, dejaba asomar la
  navegación inferior de Hangar y el log registraba una excepción por fotograma al leer
  Escape mediante la API legacy.
- Causa confirmada: la raíz ignoraba el desplazamiento heredado de D1 y el controlador
  mezclaba dos sistemas de entrada. Los filtros dependían además del momento de Start.
- Solución aplicada: reutilizar la compensación canónica D1, fondo opaco completo,
  CanvasGroup que conserva la base, conexiones persistentes más enlace dinámico y sólo
  Button/EventSystem para la interacción móvil.
- Evidencia: `[D1 Metals Inventory] INSTALL_PASS` y
  `[D1 Metals Inventory Capture] PASS | open + filters + back + reopen`, capturas
  1080×1920/720×1280 y búsqueda final sin excepciones relacionadas.
- Guía actualizada: prevención de errores 62 y 63; perfil D1; ficha y contrato propios.
- Estado: INCORPORADO; aprobación visual del usuario pendiente.

## 2026-08-19 — ARK Centro Galáctico D1 — Color semántico y centro geométrico

- Tipo: CORRECCIÓN VISUAL.
- Situación observada: la primera candidata teñía de verde o ámbar tarjetas completas,
  el conjunto de misiones estaba 9 px a la izquierda y los ecos duplicaban un sprite
  orbital como núcleo.
- Solución aplicada: tarjetas y ecos centrados por caja envolvente; color fuerte limitado
  al estado y al marco tenue de la misión activa; ecos reconstruidos con anillos, marcas
  y núcleo circular concéntrico.
- Evidencia: captura Unity nueva en 1080×1920 y 720×1280 y `[D1 Ark Capture] PASS`.
- Alcance: decisión de pantalla D1; no define la estética de otras dimensiones.

## 2026-08-19 — ARK Centro Galáctico D1 — Reconstrucción visual y listeners persistentes

- Tipo: NUEVA SUBPANTALLA Y ERROR DE ENLACE FUNCIONAL.
- Alcance generalizable: reconstrucción de paneles serializados que conservan lógica previa.
- Situación observada: la composición nueva aparecía correctamente y sus referencias
  estaban serializadas, pero los botones reconstruidos no heredaban siempre los listeners
  dinámicos del ciclo anterior de OnEnable.
- Causa confirmada: la referencia se sustituía después del momento en que el propietario
  había enlazado los botones antiguos.
- Solución aplicada: eventos persistentes para las acciones del ARK y enlace dinámico sólo
  cuando el botón no posee una ruta persistente; se validaron clic, regreso y reapertura.
- Evidencia: `[D1 Ark Center] INSTALL_PASS` y `[D1 Ark Capture] PASS | open + back +
  metals + sync real`, con capturas 1080×1920 y 720×1280.
- Guía actualizada: prevención de errores 66 y lista de control.
- Estado: INCORPORADO; aprobación visual del usuario pendiente.

## 2026-08-19 — Órbitas Antiguas D1 — Fondo interactivo y herencia de visibilidad

- Tipo: NUEVA SUBPANTALLA Y ERROR DE JERARQUÍA/RAYCAST.
- Alcance generalizable: overlays con ciclo propio y fondos globales decorativos.
- Situación observada: la subpantalla recibía `alpha=1` e interacción en su CanvasGroup,
  pero permanecía inactiva por heredar el cierre de GalaxyPanel. En QA, el fondo global
  `BackgroundMobile` también podía ser el primer objetivo del raycast.
- Causa confirmada: la raíz estaba anidada bajo un propietario que otra ruta desactiva y
  el fondo decorativo conservaba `raycastTarget=true`.
- Solución aplicada: convertir Órbitas Antiguas en hermana de Carta Galáctica dentro del
  propietario estable D1, mantener un CanvasGroup propio y desactivar el raycast del
  fondo decorativo. Escena e instalador quedaron sincronizados.
- Evidencia: `[D1 Ancient Orbits] INSTALL_PASS` y
  `[D1 Ancient Orbits Capture] PASS | real entry + metals + upgrade + back`, con capturas
  1080×1920 y 720×1280.
- Guía actualizada: prevención de errores 65 y lista de control.
- Estado: INCORPORADO; aprobación visual del usuario pendiente.

## 2026-08-19 — Inventario de Metales D1 — Centro visual e integridad de sprites

- Tipo: ERROR VISUAL Y MEJORA DEL MÉTODO DE ASSETS.
- Alcance generalizable: conjuntos de recursos detallados generados o separados desde
  hojas de sprites.
- Situación observada: varios metales parecían descentrados dentro de tarjetas idénticas
  y Hierro, Litio e Iridio mostraban formas degradadas.
- Causa confirmada: los V4 tenían espacios internos distintos y procedían de una limpieza
  destructiva sobre un fondo que no poseía la transparencia esperada.
- Solución aplicada: generar dos tiras coherentes sobre fondo sólido, separar los diez
  recursos, conservar sólo su componente principal, calcular su caja visible, escalarla
  a una ocupación común de 390 px y centrarla en lienzos transparentes de 512 px.
- Evidencia: mosaico V5 inspeccionado; `[D1 Metals Inventory] INSTALL_PASS` y
  `[D1 Metals Inventory Capture] PASS | open + filters + back + reopen`; capturas nuevas
  1080×1920 y 720×1280 sin cortes, deformaciones ni desplazamientos perceptibles.
- Guía actualizada: prevención de errores 64 y lista de control.
- Estado: INCORPORADO; aprobación visual del usuario pendiente.

## 2026-08-19 — ARK Centro Galáctico D1 — Centro perceptual, contraste y ecos de referencia

- Tipo: CORRECCIÓN VISUAL Y REAPLICACIÓN DE REGLAS EXISTENTES.
- Situación observada: los contenedores de misión estaban centrados, pero las siluetas
  visibles de las naves no compartían ocupación perceptual; la pantalla era más clara
  que la referencia y los ecos tenían una escala y separación diferentes.
- Solución aplicada: conservar los cuatro blueprints V2 de Hangar con su material,
  añadir contenedores de arte centrados y correcciones por silueta, oscurecer superficies
  y estructura, y reconstruir los ecos con centros x=231/434/637/840, cuatro anillos,
  ejes, doce marcas y cuatro radios diagonales.
- Evidencia: `[D1 Ark Center] INSTALL_PASS` y `[D1 Ark Capture] PASS | open + back +
  metals + sync real`, capturas nuevas 1080×1920 y 720×1280.
- Guía: no se añadió una regla duplicada; el caso vuelve a validar creación 44/55/62 y
  prevención 37/54/64 sobre medición, reutilización completa y centro de silueta.
- Alcance: la técnica de centro perceptual y comparación de contraste es generalizable;
  las medidas, paleta, naves y forma de eco pertenecen únicamente a ARK D1.
- Estado: INCORPORADO; aprobación visual del usuario pendiente.
## 2026-08-20 — Órbitas Antiguas D1 — Identidad estable en destinos dinámicos

- Hallazgo: las cuatro tarjetas enviaban índices 0..3 a Explorar, pero su desplegable
  reservaba el índice 0 para el estado neutral y reconstruía las opciones escaneadas.
- Riesgo general: una tarjeta visual estable puede abrir otro elemento cuando se conecta
  directamente a la posición transitoria de una colección dinámica.
- Solución aplicada: resolver el destino por ID oficial, sincronizar primero las opciones,
  aplicar el valor del control y deshabilitar la tarjeta si ese ID no está disponible.
- Evidencia: `Dimension1AncientOrbitsUI`, `Dimension1PanelUI` y
  `Dimension1AncientOrbitsCapture`; prueba de las cuatro tarjetas en PASS.
- Alcance: patrón funcional general. Los nombres, destinos y estética pertenecen sólo a D1.

## 2026-08-20 — Órbitas Antiguas D1 — Preservar la importación del asset reutilizado

- Hallazgo: el instalador volvía a preparar planetas, metales e iconos canónicos y podía
  reemplazar su transparencia o compresión aprobada aunque sólo necesitara leerlos.
- Solución aplicada: preparar únicamente los cuatro candidatos propios de la pantalla y
  hacer idempotente el preparador compartido de navegación con su tratamiento canónico.
- Evidencia: `Dimension1AncientOrbitsSetup`, `Dimension1PremiumNavigationApply` y ausencia
  de cambios residuales en los `.meta` compartidos después de la validación.
- Alcance: amplía la regla general existente de reutilizar sprite y tratamiento juntos;
  no convierte los candidatos de Órbitas Antiguas en assets aprobados.

## 2026-08-20 — Familia de sectores D1 — Armazón único y contenido variable

- Tipo: NUEVAS SUBPANTALLAS Y CONSOLIDACIÓN DE ARQUITECTURA VISUAL.
- Situación: Borde Exterior, Anillo de Restos y Frontera Silenciosa necesitaban conservar
  la misma composición de Órbitas Antiguas sin convertirse en tres implementaciones que
  divergieran en márgenes, navegación o interacción.
- Solución: clonar la jerarquía de autoridad desde un instalador común, parametrizar
  planetas, metales y destinos reales, y concentrar datos y acciones en
  `Dimension1SectorDetailUI`. El único planeta de Anillo se centra por la caja del área.
- Evidencia: `[D1 Sector Details] INSTALL_PASS` y `[D1 Sector Details Capture] PASS |
  3 entradas reales + 12 destinos exactos + metales + mejoras + regreso`, con capturas
  1080×1920 y 720×1280.
- Guía actualizada: creación 65.
- Estado: INCORPORADO; aprobación visual del usuario pendiente.

## 2026-08-20 — Destinos D1 — Cuadrícula horneada en falsos transparentes

- Tipo: ERROR DE ASSET Y VALIDACIÓN DE TRANSPARENCIA.
- Situación: seis ilustraciones generadas como transparentes mostraban un recuadro claro
  en Unity. La vista previa había usado cuadros de transparencia, pero los PNG eran RGB.
- Solución: comprobar el canal real, conservar V1 para trazabilidad, crear V2 con fondo
  oscuro integrado compatible con la tarjeta y repetir instalación y capturas.
- Evidencia: capturas V1 con patrón visible, inspección `Format24bppRgb`, capturas V2 sin
  patrón y segunda pasada funcional en PASS.
- Guía actualizada: prevención 68 y lista de control.
- Estado: INCORPORADO; los seis V2 continúan como candidatos.

## 2026-08-20 — Expedición Completada D1 — Material asociado y orden de catálogos

- Tipo: NUEVA SUBPANTALLA, REUTILIZACIÓN DE ASSETS Y ERROR DE ASOCIACIÓN.
- Situación: la primera captura mostraba fondo claro detrás de la nave y el texto Antena
  Fracturada acompañado por el sprite de otra reliquia.
- Causa: el blueprint del Hangar depende de `d1_hangar_blueprint_keyed.mat`; además el
  catálogo funcional de reliquias y la galería visual contienen los mismos IDs en órdenes
  diferentes, por lo que compartir el índice no preserva identidad.
- Solución: reutilizar sprite y material juntos y declarar el mapa visual de IDs en el
  mismo orden de sprites que la Cámara de Reliquias.
- Evidencia: `[D1 Expedition Result] INSTALL_PASS`, `VALIDATION_PASS` y
  `[D1 Expedition Result Capture] PASS`; capturas 1080×1920 y 720×1280, cierre seguro,
  estado vacío y duplicado convertido a metal verificados sin escribir el guardado.
- Guía actualizada: prevención 69, catálogo y mapa de reutilización.
- Estado: INCORPORADO; aprobación visual del usuario pendiente.

## 2026-08-20 — Flujo general — Varias instancias de Unity en proyectos independientes

- Tipo: SEGURIDAD DE AUTOMATIZACIÓN Y CONTINUIDAD GENERAL.
- Situación: el flujo de trabajo utilizará simultáneamente varios proyectos registrados en
  Unity Hub y posiblemente varios Editores abiertos. La regla anterior podía interpretar
  cualquier proceso de Unity como un conflicto y pedir cerrar proyectos no relacionados.
- Hallazgo: varias instancias son compatibles cuando cada una utiliza una carpeta de
  proyecto diferente. El riesgo real aparece al abrir una segunda instancia normal o batch
  sobre la misma ruta, especialmente durante escrituras serializadas y reconstrucciones.
- Solución: convertir la ruta absoluta del proyecto objetivo en la unidad del preflight;
  distinguir la lista del Hub de editores abiertos y pedir cerrar únicamente la instancia
  que usa el objetivo.
- Evidencia: `Quantum Forge`, `pequeño juego 2` y `prueba pequeño juego` están en rutas
  separadas y usan Unity `6000.3.10f1`; la documentación oficial de Unity prohíbe abrir en
  batch un proyecto que ya está abierto en el Editor normal.
- Guías actualizadas: instrucciones maestras, creación 39/45, prevención 15, plan maestro
  de pruebas, lista de control, reglas de continuidad y `AGENTS.md`.
- Alcance: regla general para cualquier dimensión, pantalla o proyecto; no autoriza dos
  instancias sobre la misma carpeta ni escrituras simultáneas.
- Estado: INCORPORADO.

## 2026-08-20 — Expedición Completada D1 — Cardinalidad real y columnas compartidas

- Tipo: ERROR DE COBERTURA DE DATOS Y ALINEACIÓN.
- Situación: el resultado disponía de tres slots aunque algunos destinos pueden entregar
  cuatro metales. Además, la columna derecha de Matrices estaba 50 píxeles desplazada
  respecto de Metales.
- Solución: ampliar a cuatro slots, conservar el layout amplio para uno/dos, redistribuir
  tres y aplicar una densidad compacta para cuatro dentro del mismo panel; Metales y
  Matrices comparten ahora las columnas `x=92/548`, ancho `360`.
- Evidencia: `[D1 Expedition Result Capture] PASS | 1080x1920 + 720x1280 | 0-4 metales
  + columnas alineadas + cierre seguro`; capturas específicas del estado de cuatro metales.
- Guía actualizada: prevención 70, lista de control, ficha, contrato y decisiones.
- Alcance: regla general para cualquier colección de recompensas o tarjetas dinámicas;
  las medidas concretas pertenecen solamente a esta subpantalla D1.
- Estado: INCORPORADO; aprobación visual corregida pendiente.

## 2026-08-20 — Registro de Expediciones D1 — Historial máximo más entrada fija

- Tipo: NUEVA SUBPANTALLA, CARDINALIDAD DINÁMICA Y REUTILIZACIÓN.
- Situación: el historial conserva 20 expediciones, pero la Clave Central de ARK puede
  aparecer como registro fijo adicional. Reservar sólo 20 filas ocultaba la expedición
  más antigua cuando ARK estaba activo.
- Solución: reservar 21 filas internas, excluir ARK de contadores/totales/filtros y probar
  simultáneamente las 20 expediciones más la entrada fija. Las recompensas se adaptaron
  de cero a cuatro fichas reales y las naves reutilizan blueprint + material del Hangar.
- Evidencia: `[D1 Expedition Record] INSTALL_PASS` y `[D1 Expedition Record Capture] PASS |
  ruta real + 20 registros + ARK 21/21 + filtros + scroll + cierre + 1080x1920 + 720x1280`.
- Guía actualizada: prevención 71, ficha, contrato, decisiones y mapa de reutilización.
- Estado: INCORPORADO; aprobación visual del usuario pendiente.

## 2026-08-20 — Registro de Expediciones D1 — Faltantes de icono y sustitutos inválidos

- Tipo: ERROR DE IDENTIDAD VISUAL Y CREACIÓN DE ASSET.
- Situación: los totales inferiores mostraban una diana dibujada con texto, el icono de
  navegación de Reliquias y una mena de hierro. Ninguno representaba con precisión el
  total de expediciones, reliquias o metales.
- Solución: auditar primero los assets existentes, confirmar los tres faltantes y crear
  candidatos específicos. Las primeras salidas con cuadrícula horneada se rechazaron;
  las versiones instaladas usan negro uniforme inspeccionado más el material black key
  canónico D1, que conserva los colores originales.
- Prevención: si no existe un icono exacto, el espacio permanece vacío durante la
  construcción hasta crear un candidato detallado. Se prohíben glifos, geometría rápida
  y assets de otra identidad como relleno provisional presentado al usuario.
- Guías actualizadas: creación 66, prevención 72, lista de control, catálogo y mapa de
  reutilización.
- Estado: INCORPORADO; los tres assets continúan como candidatos hasta aprobación visual.

## 2026-08-20 — Explorar y Hangar D1 — Skin visual completo pero flujo incompleto

- Tipo: ERROR DE CONEXIÓN ENTRE COMPOSICIÓN APROBADA Y AUTORIDAD FUNCIONAL.
- Situación: Explorar mostraba datos reales, pero iniciaba siempre el primer destino con
  la primera nave, anulaba el modo coordinado y no exponía escaneo, mejora del escáner,
  selección de apoyo ni varias expediciones. Hangar mostraba naves bloqueadas sin una
  acción para construirlas.
- Causa: el controlador visual había conectado sólo la acción demostrativa
  `OnClickStartFirstAvailableExploration`; la composición ocultó controles funcionales
  anteriores sin sustituir toda su capacidad.
- Solución: mantener `Dimension1PanelUI` como propietario único de selección y reglas,
  exponer adaptadores públicos estrechos y hacer que el skin opere los mismos dropdowns
  ocultos. El botón principal de Explorar es contextual y el de Hangar alterna entre
  desbloqueo y mejora según el estado real.
- Evidencia: instalación y capturas de Explorar/Hangar en 1080×1920 y 720×1280, más
  `[D1 Explore+Hangar Functional] PASS | escaneo, selección, coordinación, inicio y
  desbloqueo verificados`; el guardado permaneció suprimido durante QA.
- Prevención general: al reemplazar una interfaz funcional por un skin, inventariar
  todas las acciones y estados del controlador anterior. Una pantalla visualmente
  completa no está conectada hasta probar selecciones no iniciales y cada rama contextual.
- Estado: INCORPORADO.

## 2026-08-20 — Reliquias D1 — Un efecto real oculto por una etiqueta pendiente

- Tipo: DIVERGENCIA ENTRE DATOS, UI Y CONSUMIDOR FUNCIONAL.
- Situación: la Antena Fracturada mostraba su probabilidad de destino adicional, pero el
  segundo efecto aparecía pendiente y la tabla de hitos devolvía cero.
- Hallazgo: el temporizador real ya aplicaba una reducción escalada hasta 6 %. No faltaba
  diseñar esa mecánica; faltaba reconciliarla con los hitos y la Cámara.
- Solución: declarar 1.5 % por hito como segunda progresión, hacer que el temporizador lea
  esa misma autoridad y mostrar -6 % a nivel 100. No se asignaron valores a las otras seis
  reliquias cuya documentación sólo contiene intenciones.
- Evidencia: `[D1 Relic Effects] PASS | 14 definidas + Antena 2/2 + 6 pendientes sin bonus
  + 20 selecciones + mejora` y seis capturas de las tres páginas en 1080×1920 y 720×1280.
- Guías actualizadas: creación 68, prevención 74, ficha, decisiones y contrato.
- Estado: INCORPORADO; los seis efectos sin magnitudes continúan pendientes de diseño.

## 2026-08-20 — D1 — Componentes que pasan al instalar pero se pierden al recargar

- Tipo: ERROR DE SERIALIZACIÓN Y VALIDACIÓN ENTRE SESIONES.
- Situación: `Dimension1VisualSkinRoot` estaba declarado en un archivo con otro nombre.
  El instalador podía añadirlo y pasar, pero una nueva sesión lo cargaba como script perdido.
- Solución: mover el MonoBehaviour a `Dimension1VisualSkinRoot.cs`, retirar tres referencias
  perdidas y restaurar cuatro marcadores en las catorce raíces visuales conocidas sin
  reconstruir sus composiciones.
- Evidencia: `[Dimension1DarkThemeSetup] VISUAL_ROOT_REPAIR_PASS | raíces=14 | scripts
  perdidos retirados=3 | marcadores añadidos=4`, seguido por validaciones nuevas del Centro
  y los sectores.
- Guía actualizada: prevención 75 y lista de control.
- Estado: INCORPORADO como regla general de Unity.

## 2026-08-20 — Centro, sectores y Reliquias D1 — Regresión visual final

- Centro: se reservó el espacio real del icono en OBJETIVO ACTUAL y PROGRESO GLOBAL; el
  cajón pasó cerrado → abierto → cerrado y sus controles se buscaron dentro de su raíz.
- Sectores: Borde Exterior, Anillo de Restos y Frontera Silenciosa compensan la altura de
  su padre y ya no recortan el encabezado en 1080×1920 ni 720×1280.
- Reliquias: las 17 piezas con efectos definidos leen sus textos de la misma autoridad
  funcional; se retiró el fallback genérico. Mapa Estelar Incompleto, Sensor de Frecuencia
  Rara y Memoria de Máquina continúan explícitamente pendientes y en cero.
- Evidencia: PASS estructural y runtime del Centro, PASS de 3 entradas y 12 destinos de
  sectores, y `[D1 Relic Effects] PASS | 17 definidas + Resonador/Fragmento/Sello
  conectados + 3 pendientes sin bonus + 20 selecciones + mejora`.
- Guía actualizada: prevención 76, fichas y contratos afectados.
- Estado: INCORPORADO; la aprobación perceptual final del usuario continúa pendiente.

## 2026-08-20 — Reliquias D1 — Cierre funcional de las veinte piezas

- Tipo: CIERRE DE MECÁNICAS Y ELIMINACIÓN DE DOBLE AUTORIDAD VISUAL.
- Situación: Mapa Estelar Incompleto, Sensor de Frecuencia Rara y Memoria de Máquina
  conservaban valores antiguos aprobados, pero faltaba traducirlos a consumidores reales.
- Solución: Mapa actúa sobre memoria y pesos del escaneo; Sensor sobre aparición y categoría
  de puntos especiales; Memoria escala únicamente efectos numéricos de nodos y bonus
  derivados del progreso de reparación. No se añadieron sistemas, nombres ni recursos.
- Error detectado durante QA: el skin mantenía una lista local de reliquias pendientes y
  ocultaba los efectos ya conectados. Se retiró esa segunda autoridad.
- Evidencia: `[D1 Relic Effects] PASS | 20 definidas + 3 efectos finales conectados + 20
  selecciones + mejora` y `[D1 Relics Capture] PASS` con seis capturas nuevas en 1080×1920
  y 720×1280.
- Guía actualizada: prevención 74, ficha, decisiones y contrato de Reliquias.
- Estado: INCORPORADO; la aprobación perceptual final del usuario continúa pendiente.

## 2026-08-20 — Chat completo D1 — Auditoría integral de recorrido, propiedad y evidencia

- Tipo: REVISIÓN TRANSVERSAL DE TODO EL CICLO DE PRODUCCIÓN UI DEL CHAT.
- Alcance revisado: Centro de Mando, Carta, Explorar, Hangar, Reliquias, Árbol, Inventario,
  cuatro detalles sectoriales, ARK, Resultado, Registro, assets, márgenes, navegación,
  constructores, validaciones, guardado y documentación.
- Aprendizajes anteriores ya incorporados y no duplicados: reutilización antes de crear,
  prohibición de simplificar diseños aprobados, autoridad Hangar para el shell, tratamiento
  completo del asset, iconos faltantes, alfa real, varias instancias de Unity por ruta,
  cardinalidad de resultados, colecciones por ID y conservación de capacidad legacy.
- Vacíos nuevos confirmados: hit-area que escribe en el primer TMP; selección inválida al
  cambiar de ámbito; arte fijo ante estado dinámico; panel legacy que bloquea cíclicamente el
  skin; controles existentes pero indescubribles; reconstructores múltiples de una raíz;
  evidencia aprobada anterior al código actual; PASS aislado sin ruta real; registro maestro
  incompleto y acción persistente sin guardado inmediato.
- Errores D1 que originaron las reglas: parpadeo «Entrar» en los cuatro sectores, destinos de
  otro sector que bloquean Explorar, preview legacy que oculta el CTA, ruta ARK no demostrada,
  nave mecánica distinta del arte, datos desbloqueables omitidos, P6/P7 con el mismo sprite y
  márgenes distintos entre pantallas hermanas.
- Guía de creación ampliada: secciones 67 y 69–73 más lista de control.
- Guía de prevención ampliada: secciones 25, 59, 73 y nuevas 77–83 más lista de control.
- Plan maestro ampliado: constructor único, recorrido físico completo, transición de ámbito,
  identidad visual dinámica, bloqueadores legacy, guardado y resultados simultáneos.
- Evidencia permanente: `07_REGISTROS/AUDITORIA_INTEGRAL_DIMENSION_1_2026-08-20.md`.
- Alcance general: las reglas nuevas son transferibles a cualquier dimensión; los nombres,
  medidas, planetas, naves y composición citados siguen siendo ejemplos históricos D1.
- Estado: DOCUMENTADO; las correcciones de código y la revalidación visual permanecen pendientes.

## 2026-08-21 — D1 — Autoselección, propiedad de visibilidad y rótulos funcionales

- Tipo: CORRECCIÓN TRANSVERSAL DE NAVEGACIÓN, MODALES Y AUTORIDAD SEMÁNTICA.
- Situación: Explorar restauraba elementos antiguos al volver al Centro; un panel legacy
  conservaba `activeSelf=true` debajo de una rama inactiva y ocultaba el Centro moderno. La
  sincronización periódica de dropdowns podía confundirse con una elección física y cerrar
  el resultado.
- Solución: el retorno al Centro prepara un único estado neutral; sólo bloquean objetos con
  `activeInHierarchy=true`. Los dropdowns programáticos sincronizan también su valor
  observado y no comparten efectos secundarios con una elección física del jugador.
- Coherencia: Carta separa cuatro sectores del Centro y cuenta intentos completados; los detalles
  dicen Destinos del sector; Centro identifica recomendación y Prestigio 1; Hangar consume
  previews de las curvas reales para mostrar magnitudes actuales y siguientes.
- Guardado: se demostró que 23/12/1 provenían del escenario de captura y alcanzaron la
  partida porque la prueba levantaba la supresión antes de `OnApplicationQuit`. La
  supresión queda enclavada durante todo el proceso. La partida se reparó desde registros
  explícitos a 26/1/1/5, con respaldo y hash posterior estable.
- Evidencia: reconstrucción `APPLY_PASS`; clic físico Explorar -> Centro inmediato y
  sostenido; modal estable durante 12 fotogramas reales; ruta física 14/14; ciclo de cuatro
  sectores; flujo Explorar+Hangar con textos de Blindaje/Sensores; capturas nuevas en
  1080×1920 y 720×1280.
- Prevención general: las selecciones programáticas deben actualizar estado sin ejecutar
  callbacks que representen intención del jugador; visibilidad de una pantalla debe tener
  un único cierre de transición y evaluar visibilidad efectiva; una prueba visual no puede
  rehabilitar escrituras antes de cerrar el proceso; los rótulos deben nombrar la fórmula
  que realmente leen.
- Estado: INCORPORADO; aprobación perceptual y revisión gráfica adicional de Explorar pendientes.

## 2026-08-21 — D1 — Un botón reconstruido no puede depender del instalador de otra pantalla

- Tipo: ERROR DE PROPIEDAD DE NAVEGACIÓN Y ORDEN DE RECONSTRUCCIÓN.
- Situación: Carta Galáctica construía EXPLORAR con sólo la acción de cerrar Galaxia. El
  instalador de Explorar intentaba añadir después la apertura de la raíz moderna. Si Carta
  se reconstruía al final, ese enlace desaparecía y quedaba expuesta la interfaz antigua.
- Solución: Carta Galáctica construye y controla su propia ruta completa a Explorar; además,
  la presentación moderna cierra el estado secundario y mantiene suprimida la raíz legacy.
- Regresión posterior detectada: al reconstruir Centro de Mando después de Explorar, su
  referencia serializada `exploreScreenRoot` volvió a quedar vacía. Carta cerraba primero
  Galaxia y, al fallar la segunda acción, dejaba visible la pantalla inicial.
- Corrección definitiva: Centro de Mando y Explorar recuperan mutuamente sus propietarios
  cuando una reconstrucción parcial pierde el enlace; el constructor de Centro vuelve a
  sincronizarlo y Carta no se cierra antes de entregar el control a Explorar.
- Extensión: los accesos compartidos que pueden sobrevivir a reconstrucciones parciales,
  como 10 METALES, revalidan sus enlaces sin duplicarlos.
- Evidencia actual: compilación runtime/editor con 0 errores y recorrido físico
  `[D1 Full 14 Screen Route] PASS | 14/14 pantallas`, incluido el clic real
  Carta Galáctica -> EXPLORAR y el regreso Explorar -> Centro de Mando.
- Estado: INCORPORADO Y VALIDADO FÍSICAMENTE.

## 2026-08-21 — D1 — Una referencia nueva debe retirar también la semántica visual anterior

- Tipo: REDISEÑO DE COMPOSICIÓN CON COMPATIBILIDAD FUNCIONAL.
- Situación: Carta Galáctica agrupaba siete cuerpos en cuatro hexágonos y mostraba el
  historial de expediciones como si fuera contenido del sector. El usuario aprobó una
  vista orbital expandida y nombres propios para cada cuerpo.
- Solución: siete trayectorias inclinadas y siete cuerpos fijos con rotación axial;
  Elysia, Vulkar, Corona de Tántalo, Mnemos, Orpheon, Nyxara y Erebon se resuelven desde
  IDs planet_01…planet_07 sin migrar el guardado. Los contadores se sustituyen por estados
  y sector real. El Centro usa recorte negro canónico y animación por capas centradas.
- Prevención general: cuando una referencia posterior reemplaza la composición, ficha,
  contrato, pruebas y constructor deben cambiar juntos. Una imagen con fondo oscuro debe
  verificarse renderizada; coincidir en color no demuestra que su borde haya desaparecido.
- Evidencia: compilación runtime/editor 0 errores; configuración V12 PASS; prueba temporal
  7 cuerpos/7 órbitas PASS; clic físico de los siete cuerpos PASS; capturas neutral y
  seleccionada PASS en 1080×1920 y 720×1280; escritura QA suprimida al cerrar.
- Estado: INCORPORADO Y VALIDADO TÉCNICAMENTE; APROBACIÓN PERCEPTUAL PENDIENTE.

## 2026-08-21 — D1 — Un normalizador compartido no debe restaurar una composición retirada

- Tipo: CONFLICTO DE PROPIEDAD ENTRE CONSTRUCTOR DE PANTALLA Y SHELL COMPARTIDO.
- Situación: el constructor nuevo de Explorar creó correctamente sector, destinos y selector
  de modo, pero `Dimension1SharedShellApply` reconocía los nombres históricos de los bloques
  y les imponía nuevamente las medidas del radar anterior.
- Detección: la compilación y validación estructural pasaron; sólo la captura nueva mostró
  tarjetas fuera del viewport y `ModePanel` superpuesto con la expedición activa.
- Solución: la presencia de `ModePanel` identifica la composición vigente. El shell continúa
  siendo dueño del marco, encabezado y navegación, pero preserva las coordenadas del cuerpo
  declaradas por el constructor de Explorar.
- Prevención general: todo normalizador que conozca nombres de contenido debe versionar o
  reconocer la composición antes de aplicar geometría histórica. Una jerarquía válida no
  reemplaza la revisión visual en ambas resoluciones.
- Evidencia: captura final PASS en 1080×1920 y 720×1280; ruta física Explorar → Centro PASS;
  recorrido 14/14 PASS.
- Estado: INCORPORADO; aprobación perceptual final del usuario pendiente.

## 2026-08-21 — D1 — Un acceso compartido debe sobrevivir aunque el constructor lo reduzca a panel

- Tipo: REGRESIÓN DE INTERACCIÓN Y COHERENCIA DINÁMICA DEL ENCABEZADO.
- Situación: 10 METALES tenía Button en varias pantallas, pero Carta y Explorar lo habían
  reconstruido como un panel puramente visual. El reintento de enlace buscaba Buttons, por
  lo que nunca encontraba precisamente los accesos rotos. Además, las tres tarjetas de
  recursos conservaban una terna fija aun después de cambiar el sector de exploración.
- Solución: el controlador descubre primero los objetos por sus nombres estables y completa
  toda la cadena táctil si falta. El constructor del armazón conserva una ruta persistente y
  el validador exige Button, targetGraphic, raycast y destino. Un único propietario cambia
  conjuntamente ID, nombre, cantidad, ritmo e icono de los metales según el sector.
- Prevención general: no validar una acción buscando sólo el componente cuya ausencia es el
  fallo esperado. Para contenido dinámico, toda la representación visible y funcional debe
  derivarse del mismo ID estable.
- Evidencia: runtime y editor compilan con 0 errores. Prueba física y capturas pendientes
  para no interferir con la instancia de Unity que el usuario mantiene abierta.
- Estado: INCORPORADO EN FUENTE; PENDIENTE DE VALIDACIÓN FÍSICA EN UNITY.

## 2026-08-21 — D1 — Una misma Carta necesita contexto de entrada explícito

- Tipo: CORRECCIÓN DE NAVEGACIÓN CONTEXTUAL Y REVISIÓN VISUAL DE CENTRO DE MANDO.
- Situación: CAMBIAR SECTOR de Explorar reutilizaba la misma acción que GALAXIA. Después de
  elegir un sector, ENTRAR AL SECTOR abría planetas aunque la intención era cambiar el ámbito
  de exploración. En Centro de Mando, los assets ráster canónicos eran coherentes por catálogo
  pero no por composición, y el emblema del encabezado se leía como una M o puerta.
- Solución funcional: Carta recibe un contexto temporal de selección. CAMBIAR SECTOR muestra
  EXPLORAR ESTE SECTOR y vuelve a Explorar; GALAXIA conserva ENTRAR AL SECTOR y abre planetas.
  El contexto se limpia al confirmar o salir y no puede heredarlo la navegación inferior.
- Solución visual: las seis tarjetas vuelven a iconos lineales reproducibles, con más facetas,
  nodos, anillos, motores y rutas. El emblema pasa a una baliza táctica con tres nodos. El
  cristal, el campo estelar y el instrumento central permanecen intactos.
- Evidencia: instalación de Centro y Explorar PASS; prueba física contextual PASS; regresión
  Explorar -> Centro moderno exclusivo PASS; capturas de Centro PASS en 1080×1920 y 720×1280.
- Prevención general: cuando una pantalla común sirve a intenciones diferentes, la intención
  debe viajar como estado explícito y efímero; el texto y el destino del CTA deben depender de
  ese estado, que debe limpiarse al abandonar el flujo.
- Estado: INCORPORADO; aprobación perceptual final del usuario pendiente.

## 2026-08-21 — D1 — Una subpantalla visible debe bloquear explícitamente su origen

- Tipo: TRANSICIÓN INDIRECTA Y COMPETENCIA DE RAYCASTS.
- Situación: el Registro detallado existía y el botón conservaba listener, pero la apertura
  dependía del refresco general. Explorar sólo reconocía como bloqueo al panel textual
  antiguo, no a la nueva raíz detallada, y podía seguir visible e interactivo debajo.
- Solución: la acción abre directamente la raíz detallada y el origen consulta el estado
  funcional del Registro para desactivar su CanvasGroup. El constructor y la prueba física
  describen la misma relación.
- Prevención general: una referencia serializada y un listener válido no demuestran una
  transición completa. Deben verificarse también orden de capa, CanvasGroup del destino y
  desactivación de raycasts del origen durante toda la permanencia de la subpantalla.
- Estado: INCORPORADO EN FUENTE; PENDIENTE DE VALIDACIÓN FÍSICA EN UNITY.

## 2026-08-22 — D1 — Compartir sector no significa compartir selección visual

- Tipo: IDENTIDAD DE CUERPO, RECORTE DE MODAL Y DENSIDAD MÓVIL.
- Situación: Mnemos y Orpheon previsualizan el mismo sector, por lo que una selección sólo
  por sector podía acentuar la órbita equivocada. DETALLES tenía texto real detrás de un
  recorte incompatible con su fondo transparente. El Registro comprimía demasiadas filas.
- Solución: la Carta conserva temporalmente el ID exacto del planeta además del sector;
  cada órbita se valida por color tras clic físico. La modal usa recorte rectangular y
  fuerza visibilidad del contenido. El Registro amplía filas e iconos sin alterar datos.
- Prevención general: el ID de navegación y el ID de representación no deben fusionarse
  cuando varios elementos conducen al mismo destino. Una modal debe probar contenido
  visible, no sólo texto no vacío. La legibilidad móvil requiere captura en ambas medidas.
- Evidencia: aplicación PASS; siete cuerpos/órbitas PASS; Detalles PASS; sectores sin
  destinos duplicados PASS; Registro y capturas PASS; ruta completa 14/14 PASS.
- Estado: INCORPORADO Y VALIDADO EN UNITY; aprobación perceptual final del usuario pendiente.

## 2026-08-22 — D1 — El centro táctil no siempre es el centro del arte

- Tipo: GEOMETRÍA ORBITAL Y PROPIEDAD DE POSICIÓN.
- Situación: los nodos incluían 72 px para rótulos; colocar el centro de la raíz sobre una
  órbita dejaba el planeta 36 px por encima. Otras coordenadas eran aproximaciones manuales.
- Solución: proyectar el centro visual de cada cuerpo sobre su elipse rotada y desplazar la
  raíz sólo para compensar el espacio de rótulos. La escena recibió únicamente siete cambios
  de coordenadas y el constructor conserva el mismo cálculo.
- Prevención general: antes de alinear un control compuesto, identificar si la autoridad es
  su raíz, su área táctil o el centro visible del asset. Validar la ecuación y la captura.
- Evidencia: compilación 0/0, aplicación puntual PASS, siete clics y siete elipses PASS,
  capturas PASS en 1080×1920 y 720×1280.
- Estado: INCORPORADO Y VALIDADO EN UNITY; aprobación perceptual final del usuario pendiente.

## 2026-08-24 — GENERAL — Una build correcta puede ocultar una regresión grande de peso

- Tipo: CONTROL GENERAL DE ASSETS Y ENTREGA ANDROID.
- Situación: una APK QA aumentó 88,58 MiB aunque código, firma y compilación fueran válidos.
- Diagnóstico: `sharedassets0` explicó 87,15 MiB del aumento; numerosas ilustraciones de UI
  conservaban un coste de importación excesivo para su tamaño renderizado.
- Método general: comparar cada build con la anterior, revisar Build Report y grupos internos,
  mantener referencias/candidatos fuera de dependencias de producción y optimizar desde el
  tamaño visible real después de estabilizar el arte.
- Alcance: general para todas las dimensiones y futuros perfiles de build. Las cifras y assets
  de Dimensión 1 son sólo evidencia histórica y no transmiten su estética.
- Estado: INCORPORADO A GUÍAS, PLAN MAESTRO Y MATRIZ QA.

## 2026-08-24 — D2 — La colección documental vigente debe coincidir con su matriz

- Tipo: CONTINUIDAD DOCUMENTAL DE DIMENSIÓN.
- Situación: el origen registraba V4 y reemplazos V5, mientras la matriz todavía declaraba
  V3 como candidata vigente.
- Solución: perfil, contrato, matriz, catálogo y registro maestro apuntan ahora a V4 para
  vistas 01–18 y V5 para 19–20, sin promover conceptos a capturas reales ni declarar
  aprobación global inexistente.
- Prevención general: cada reemplazo de referencia debe actualizar origen, matriz, contrato,
  ficha y continuidad antes de iniciar implementación.
- Estado: DOCUMENTACIÓN SINCRONIZADA; APROBACIÓN VISUAL GLOBAL PENDIENTE.

## 2026-08-24 — GENERAL — Una pieza gráfica integrada no se corrige con Transform

- Tipo: INTEGRACIÓN VISUAL, ALFA Y ESTADOS DE INTERFAZ.
- Situación: una compuerta creada fuera de la referencia se intentó integrar sobre un
  robot ajustando escala, posición y giro. También se usaron marcos genéricos sobre un
  dock cuya imagen base ya contenía una herramienta marcada como seleccionada.
- Diagnóstico: la pieza no compartía plano, perspectiva, borde, óxido, luz ni sombra con
  el robot. El PNG incorporaba un tablero de transparencia como píxeles. Los marcos
  superpuestos podían dejar dos herramientas aparentemente activas o añadir bordes negros.
  Un cambio local aplicado a un shader compartido provocó una pantalla fucsia.
- Método general: partir de la referencia exacta y aprobar una composición estática;
  exportar alfa real; separar arte base neutro y estado seleccionado; aislar un material
  de prueba antes de tocar un shader compartido; validar en Play y con captura reciente.
- Alcance: regla transferible. Las imágenes y la estética del robot son únicamente el
  caso que originó el aprendizaje y no forman parte del catálogo visual de Quantum Forge.
- Estado: GUÍA DE INTEGRACIÓN GRÁFICA AÑADIDA; validación perceptual de cada proyecto
  pendiente antes de aprobar sus assets.

## 2026-08-26 — GENERAL — Ajustar aspecto no sustituye recortar el viewport

- Tipo: RECORTE, CAPAS Y CONTINUIDAD ENTRE PANTALLAS.
- Situación: un fondo con ajuste de aspecto por envolvente se extendió fuera del área del
  Monolito e invadió encabezado y ficha. Además, un botón de regreso construido debajo de
  un panel de Mezclas de último nivel existía y funcionaba, pero quedaba visualmente oculto.
- Causa confirmada: `AspectRatioFitter.EnvelopeParent` puede ampliar el hijo más allá del
  rectángulo padre; el padre no recorta por sí mismo. El orden de hermanos hacía que el
  panel auxiliar dibujara encima de una navegación que no le pertenecía.
- Solución: el viewport visual recibe un recorte local y el fondo conserva su aspecto dentro
  de ese límite. La salida de Mezclas se construye y valida dentro de la capa superior de
  Mezclas, mientras encabezado y pestañas permanecen bajo un propietario común activo.
- Prevención general: todo contenido con modo envolvente debe probar sus cuatro bordes en
  ambas resoluciones. Toda subpantalla superpuesta debe poseer una salida visible en su
  propia capa y una prueba física que abra, regrese y compruebe el contenido restaurado.
- Evidencia: reconstrucción PASS, navegación física Mezclas→Nodos PASS y capturas nuevas
  en 1080×1920 y 720×1280.
- Estado: INCORPORADO Y VALIDADO EN UNITY; aprobación perceptual final pendiente.

## 2026-08-27 — GENERAL — El lienzo normalizado no define la huella ni la zona útil

- Tipo: COMPOSICIÓN RASTER, HUELLA PROYECTADA Y OVERLAYS.
- Situación: un fondo cuadrado y cuatro hojas cuadradas parecían compatibles por tamaño. En
  Unity el soporte seguía debajo de una base más ancha y algunos símbolos atravesaban los
  bordes inclinados, aunque sus centros estaban dentro de rangos normalizados razonables.
- Causa confirmada: se midió el lienzo 0–1 en vez de la materia visible después de escala,
  relación de aspecto, recorte y perspectiva. Cada cara ocupaba una región distinta dentro
  del mismo lienzo.
- Solución: bloquear el objeto, medir su huella en captura real, editar sólo la capa dueña
  del alojamiento y definir posiciones seguras independientes por cara. La prueba considera
  los 64 px completos de cada símbolo y falla si un extremo sale de la banda conservadora.
- Prevención general: toda relación entre capas debe verificarse en el render final; las
  coordenadas normalizadas sólo pueden reutilizarse si también coincide la zona útil.
- Evidencia: primer intento V33 detectado por la prueba; corrección V34 con 32 capturas y
  PASS en `Logs/machine_monolith_v34_capture.log`.
- Alcance: método general para sprites, marcos, alojamientos y overlays; no generaliza la
  forma ni la estética del Monolito.
- Estado: INCORPORADO Y VALIDADO EN UNITY; aprobación perceptual final pendiente.

## 2026-08-27 — GENERAL — Un marco incorrecto no se corrige desplazando el objeto aprobado

- Tipo: AUTORIDAD GEOMÉTRICA, CAPAS Y COMPOSICIÓN.
- Situación: el marco del suelo no correspondía a la huella del objeto. Ampliar y desplazar
  el objeto acercó algunos bordes, pero invalidó su composición previamente aprobada y no
  resolvió la lectura física del soporte.
- Causa confirmada: se dio al objeto la responsabilidad de compensar una geometría horneada
  en otra capa. Además, las caras usaban máscaras y offsets para ocultar siluetas raster que
  no correspondían a las regiones del objeto completo.
- Solución: restaurar y bloquear escala y posición aprobadas; editar únicamente el fondo que
  posee el marco; rehacer las caras desde las regiones canónicas con alfa real; separar nodos
  y marcadores como capas; eliminar máscaras y compensaciones de silueta aproximadas.
- Prevención general: antes de mover una referencia aprobada, nombrar el propietario de cada
  límite, apoyo y oclusión. La capa propietaria del error debe cambiar; el resto se congela y
  se usa como referencia inmutable.
- Evidencia: V31 pasó técnicamente, pero fue rechazada porque el marco seguía siendo
  genérico. La corrección demostrada usa `machine_monolith_v32_setup_2.log` y
  `machine_monolith_v32_capture_2.log`, ambas PASS en dos resoluciones.
- Estado: INCORPORADO Y VALIDADO EN UNITY CON V32; aprobación perceptual final pendiente.

## 2026-08-27 — GENERAL — Una superficie que recibe overlays debe conservar un área útil continua

- Tipo: ARTE RASTER, ESTADOS Y CAPAS INTERACTIVAS.
- Situación: una progresión visual representaba daño eliminando grandes fragmentos del
  interior. Los símbolos dinámicos de Unity quedaban sobre huecos o sobre el laboratorio y
  parecían ausentes, aunque existían y sus hitboxes funcionaban.
- Causa confirmada: el generador recibió «cara dañada» sin separar la banda perimetral del
  área interior reservada para overlays. El daño artístico invadió el propietario visual de
  nodos, firmas y marcadores.
- Solución: definir una máscara conceptual de dos zonas antes de generar: banda exterior
  dañable y superficie interior inmutable. Todas las etapas conservan el mismo panel útil;
  sólo el perímetro progresa. Los símbolos permanecen en una capa independiente y se prueban
  a su contraste mínimo y máximo.
- Prevención general: cuando texto, nodos, iconos o firmas se superponen a un asset por
  estado, documentar y validar su zona segura sobre todas las etapas, no únicamente sobre la
  reparada. Prohibir huecos, transparencias y relieves dentro de esa zona si no son parte del
  diseño aprobado.
- Evidencia: cuatro caras V32 continuas, 7 + 11 + 7 + 0 overlays visibles y capturas PASS en
  1080×1920 y 720×1280 en `machine_monolith_v32_capture_2.log`.
- Alcance: principio general para cualquier sprite de fondo que reciba contenido dinámico;
  no generaliza la estética ni las formas del Monolito.
- Estado: INCORPORADO Y VALIDADO EN UNITY; aprobación perceptual final pendiente.

## 2026-08-26 — GENERAL — Una transición no debe escalar una composición horneada completa

- Tipo: CAPAS VISUALES, TRANSICIÓN Y REUTILIZACIÓN DE ESTILO.
- Situación: al acercar una imagen que contenía objeto y encuadre, también crecía su
  rectángulo y la pantalla parecía acercar un libro o fotografía. El intercambio se
  cubría con una barra cian ajena al arte y la nueva ficha usaba marcos lineales aunque
  la sección ya poseía un sistema metálico aprobado.
- Causa confirmada: primer plano y entorno no tenían propietarios independientes; el
  efecto actuaba sobre la composición completa. Además se eligieron assets provisionales
  por similitud de nombre sin comparar su uso real en las pantallas hermanas.
- Solución: recortar el objeto a una capa propia, mantener el fondo inmóvil y cubrir el
  cambio con un fundido contenido. Rastrear y reutilizar los mismos marcos de recursos,
  módulos y selectores que usa el constructor canónico de la sección.
- Prevención general: antes de animar una imagen compuesta, separar qué debe moverse y qué
  debe permanecer estable. Antes de añadir un marco, localizar el asset instalado por la
  pantalla hermana de autoridad y medir también su área interior útil.
- Evidencia: reconstrucción PASS; transición sin rectángulo ni barra; marcos canónicos y
  capturas en 1080×1920 y 720×1280 PASS.
- Estado: INCORPORADO Y VALIDADO EN UNITY; aprobación perceptual final pendiente.

## 2026-08-26 — GENERAL — La luminancia del arte oscuro no puede funcionar como opacidad gradual

- Tipo: RECORTE DE NEGRO, ALFA Y LEGIBILIDAD DE MATERIAL.
- Situación: el Monolito y sus caras se integraban sobre un nuevo fondo, pero sus sombras
  dejaban ver el laboratorio y hacían que piedra y metal parecieran semitransparentes.
- Causa confirmada: el shader convertía un intervalo amplio de brillo en alfa gradual. Los
  píxeles oscuros válidos del arte recibían opacidad parcial junto con el fondo negro.
- Solución: reservar alfa cero al exterior y combinar el recorte por negro con máscaras de
  materia ajustadas a cada placa y cara. Los negros internos se mantienen opacos conservando
  su RGB; la cuarta cara se oscurece mediante color, no mediante transparencia. Se descartó
  el respaldo geométrico porque su silueta podía hacerse visible alrededor del arte.
- Prevención general: medir el valor positivo mínimo antes de fijar el umbral y revisar la
  pieza sobre un fondo contrastante; la superficie debe taparlo incluso en sombras y grietas.
- Evidencia: vista general, sectores 1–4 y capturas 1080×1920/720×1280 PASS en
  `machine_monolith_2d_solid_mask_v12_capture.log`.
- Estado: INCORPORADO Y VALIDADO EN UNITY; aprobación perceptual final pendiente.

## 2026-08-26 — GENERAL — Navegación y estado interior necesitan señales distintas

- Tipo: JERARQUÍA VISUAL Y SIGNIFICADO DE INTERACCIÓN.
- Situación: la vista completa del Monolito mostraba tres firmas de reparación y un `???`
  para señalar sus cuatro accesos. Esos elementos comunicaban contenido interior en vez de
  expresar únicamente que cada cara podía tocarse.
- Solución: sustituirlos por cuatro luces cian-blancas simples, una por cara. Las firmas se
  reservan para los nodos dentro de los sectores 1–3 y el `???` aparece sólo al entrar en la
  cuarta cara.
- Prevención general: documentar por separado la señal de navegación y la identidad de los
  estados internos; una vista padre no debe reutilizar símbolos exclusivos de su destino.
- Evidencia: validación y capturas en 1080×1920 y 720×1280 PASS en
  `machine_monolith_2d_entry_lights_v13_capture.log`.
- Estado: INCORPORADO Y VALIDADO EN UNITY; aprobación perceptual final pendiente.

## 2026-08-26 — GENERAL — Un objeto oscuro debe separarse con una clave que no destruya su materia

- Tipo: ALFA, RECORTE E INTEGRACIÓN SOBRE FONDOS.
- Situación: un arte oscuro debía superponerse sobre un laboratorio distinto. El nuevo PNG
  aparentaba transparencia en la previsualización, pero el patrón claro estaba horneado en
  RGB; usar de nuevo el negro como clave habría vuelto semitransparente el objeto.
- Solución: comprobar el formato y los píxeles reales del archivo y aislar únicamente el
  exterior claro y neutro. Mantener totalmente opacos los negros internos, sombras, grietas
  y metales, y conservar el entorno en una capa independiente e inmóvil.
- Prevención general: la apariencia del preview no demuestra alfa real. Antes de integrar,
  inspeccionar el canal alfa y escoger la clave por el fondo comprobado, nunca por los tonos
  que forman parte del sujeto.
- Evidencia: Monolito completo y cuatro caras sin rectángulo oscuro ni translucidez sobre
  el laboratorio V06; prueba de estados y resoluciones PASS en
  `machine_monolith_industrial_lab_v11_capture.log`.
- Estado: INCORPORADO Y VALIDADO EN UNITY; aprobación perceptual final pendiente.

## 2026-08-26 — GENERAL — Reiniciar una transición a escala uno puede invalidar la composición estática

- Tipo: TRANSICIÓN, ESCALA Y PROPIEDAD DEL LAYOUT.
- Situación: el constructor guardaba el Monolito a una escala menor para mantenerlo humano
  y apoyado, pero el controlador restauraba `Vector3.one` después del refresco. La captura
  resultante parecía mostrar un artefacto gigantesco aunque la escena serializada tuviera
  otra medida.
- Solución: serializar la posición y escala reales de reposo desde el constructor y hacer
  que toda salida o reinicio de animación vuelva exactamente a esos valores.
- Prevención general: no usar cero o uno como sinónimos de estado inicial. La transición
  debe capturar y restaurar la composición aprobada y comprobarse después de abrir, volver
  y refrescar datos.
- Evidencia: escala estable, base apoyada y transición completa en 1080×1920 y 720×1280;
  `machine_monolith_grounded_lab_v13_capture.log` registra PASS.
- Estado: INCORPORADO Y VALIDADO EN UNITY; aprobación perceptual final pendiente.

## 2026-08-29 — GENERAL — Una zona segura debe validar la huella completa del overlay

- Tipo: LAYOUT DINÁMICO, ARTE RASTER Y VALIDACIÓN GEOMÉTRICA.
- Situación: los centros de varios símbolos estaban dentro de la superficie permitida, pero
  sus extremos todavía invadían fragmentos dañados del perímetro.
- Causa confirmada: la comprobación trataba un elemento visual con ancho y alto como si fuera
  un punto. El PASS técnico de centros no representaba el resultado perceptual completo.
- Solución: medir en Unity la media extensión normalizada del RectTransform y comprobar nueve
  puntos de la huella: centro, lados y esquinas. Mantener separadas el área visible y el área
  táctil para poder asegurar el arte sin degradar la interacción.
- Prevención general: toda regla de zona segura para iconos, texto, firmas o halos debe incluir
  sus dimensiones reales y el peor estado del fondo; nunca aprobar únicamente por el pivote.
- Evidencia: V45 valida S1–S3 en estados dañados y reparados, a 1080×1920 y 720×1280, en
  `machine_monolith_v45_full_footprint_symbols_final.log`.
- Estado: INCORPORADO Y VALIDADO EN UNITY; aprobación perceptual final pendiente.

### 2026-09-01 — Dimensión 2 / Lugar de Vínculo — Precisiones nuevas sobre texto compuesto

- Tipo: ERROR Y TIP
- Alcance generalizable: placas repetidas y familias tipográficas cuyo contenido se actualiza
  en runtime.
- No generalizar: nombres, cuerpos, coordenadas, división PEREGRI- / NACIONES ni estética D2.
- Situación observada: varias reglas generales ya existían —medir por bloques, respetar tamaño
  mínimo, usar un propietario visual y corregir la capa dueña—, pero faltaban tres precisiones:
  nombre y nivel podían divergir al tener coordenadas X separadas; el runtime podía perder los
  saltos aprobados por el constructor; y una etiqueta larga podía reducir innecesariamente a
  toda su familia.
- Causa o explicación confirmada: relaciones visuales que debían ser invariantes seguían
  expresadas como valores independientes o como excepciones resueltas globalmente.
- Solución o método comprobado: guardar un solo centro medido por placa y derivar de él todas
  sus cajas; centralizar en runtime la representación multilínea; resolver únicamente la
  etiqueta excepcional mediante espacio, tracking o división aprobada sin bajar el cuerpo del
  grupo. Las reglas ya existentes de fidelidad por bloques, zona segura, edición de la capa
  propietaria y estados vacíos continúan vigentes y no se duplican aquí.
- Evidencia o prueba: `bondCardCenterX`, `GetCardDisplayName`, `GetEffectDisplayText` y captura
  v6 en 1080×1920 / 720×1280 con PASS funcional y 95.79 % / 96.22 %.
- Guía y sección actualizadas: Creación 7, 42 y 44; Prevención 3, 18 y 19; listas de control.
- Lista de control actualizada: SÍ
- Estado: INCORPORADO

## 2026-09-08 — Dimensión 1 — Referencias entrantes y superficie de pulsación

- Reconstruir el Centro dejaba referencias vacías en tres consumidores inactivos. El constructor ahora vuelve a conectar y valida todas las entradas conocidas.
- Un clic central exitoso ocultaba la intercepción parcial de Hangar por el selector de dimensiones. Se validaron cinco puntos interiores y el desplazamiento al abrir el selector.
- Una pantalla visible encima no bloquea automáticamente los huecos: Carta requiere oclusión explícita mientras se ve el detalle sectorial.
- La captura nativa diferida no debe mezclarse con cambios temporales del Canvas para un render manual. La evidencia definitiva utiliza capturas nativas en directorios separados.
- Guía actualizada: lista de control de GUIA_PREVENCION_ERRORES_PANTALLAS.txt. Resultados: AUDITORIA_DIMENSION_1_2026-09-08.md. Aprobación perceptual pendiente.

## 2026-09-09 — Común / Máquina — La zona segura también debe medir el aprovechamiento de la superficie

- Tipo: COMPOSICIÓN DINÁMICA Y EVIDENCIA VISUAL.
- Situación: una distribución podía pasar separación, huella y límites, pero seguir
  concentrada en el centro y desperdiciar las zonas superior e inferior de la cara.
- Causa confirmada: el primer ajuste convirtió posiciones desde capturas antiguas y usó una
  región matemática más estrecha que la piedra real. El PASS comprobaba seguridad, no el
  reparto perceptual del espacio.
- Solución: calibrar las posiciones sobre capturas actuales de Unity, ajustar el polígono al
  contorno visible de cada asset y revisar conjuntamente extensión vertical, irregularidad y
  huella completa antes de presentar la captura.
- Prevención general: para grupos de overlays, la prueba de zona segura debe acompañarse de
  una comparación visual del área ocupada. Un PASS de límites no demuestra que la composición
  aproveche correctamente la superficie.
- Evidencia: distribución V53 aprobada por el usuario y PASS en
  `Logs/machine_monolith_v53_full_surface.log`, con capturas en ambas resoluciones.
- Las guías ya exigen captura vigente, comprobación perceptual y huella completa; este caso
  concreta esas reglas y no añade una excepción estética transferible a otras pantallas.
- Estado: INCORPORADO Y APROBADO.

### 2026-09-09 — Monolito V54: contorno dañado y cierre de regresión

- Reapertura posterior a V53: el usuario señaló firmas sobre el borde roto de cara 1.
  El problema también existía en la captura inicial de Unity; la lámina reparada ocultaba
  esa diferencia. La aprobación anterior no certificaba todos los estados.
- Causa: el polígono fijo admitía superficie que sólo existía al reparar el monolito.
  La validación de huella se ejecutaba antes de cambiar la resolución y no comprobaba
  los tiles intermedios ni las esquinas transformadas reales.
- Corrección local: posiciones dentro de la superficie común conservadora de las cuatro
  etapas; 4 px locales adicionales alrededor de la huella completa. Validación por
  resolución después del render y lectura del alfa del tile canónico actual.
- La prueba conserva dos posiciones defectuosas V53 que deben fallar y exige 24 casos
  distintos. La preparación parcial completa todos los tiers de las ramas elegidas,
  porque un tier aislado no hace que el representante visible cuente como reparado.
- El constructor informa sólo de estructura; el capturador informa de geometría,
  estados y navegación. La aprobación perceptual sigue siendo del usuario.
- Se añadió captura sobre la escena existente para iterar posiciones runtime sin
  reconstruir jerarquías ajenas. Se conserva Main.unity idéntica a la copia previa.
- Evidencia: Logs/VisualQA/MachineMonolith2D_V54 y registro final
  Logs/machine_monolith_v54_final.log. Referencia del defecto archivada en
  05_REFERENCIAS/MAQUINA/MONOLITO_V54_BORDE_DANADO_2026-09-09.
- Revisión auxiliar: auditoría de cobertura y transformación de la huella en modo lectura;
  integración, escrituras y todas las ejecuciones Unity a cargo del coordinador.
- Alcance transferible: overlays persistentes sobre arte que cambia de silueta. Los
  polígonos y posiciones concretos de estas caras no son un estilo para otras pantallas.
- Estado visual: CANDIDATO V54, pendiente de revisión del usuario y de dispositivo.

## 2026-09-10 — Común / QA Android — Rendering Debugger superpuesto al panel QA

- Tipo: INTERACCIÓN TÁCTIL, OVERLAYS RUNTIME Y PROPIEDAD DEL EVENTSYSTEM.
- Situación: durante una prueba idle en el teléfono, el Rendering Debugger de Unity apareció
  sobre el panel QA y capturó sus toques; CERRAR seguía conectado pero era inaccesible.
- Causa confirmada: Render Pipelines Core habilita el depurador runtime en Development Build
  y alterna su Canvas mediante doble toque con tres dedos.
- Solución: la APK QA de dispositivo deshabilita la UI runtime del depurador antes de cargar
  la escena, conserva todas las herramientas QA propias y añade Atrás como salida del panel.
- Prevención general: toda build con panel de diagnóstico propio debe decidir explícitamente
  qué overlay posee la interacción; un Canvas del motor no puede competir silenciosamente
  con los controles táctiles del juego.
- Prueba: `QaBlock3Validation` exige CERRAR, Atrás, cancelación de confirmación, política de
  exclusión y conservación de los controles QA. La verificación final se repite en Android.
- Guía actualizada: Prevención 92 y lista de control.
- Estado: INCORPORADO; validación de dispositivo pendiente.

## 2026-09-17 — Mejoras — Un filtro vacío debe explicar su resultado sin colapsar la pantalla

- Tipo: ESTADO VACÍO, FILTRADO Y PROPIEDAD DE VISIBILIDAD.
- Situación: «Ocultar completados» eliminaba todas las filas cuando la colección estaba
  completa y dejaba un laboratorio vacío que parecía una pantalla rota.
- Causa confirmada: dos controladores intervenían en la visibilidad y no existía un estado
  explícito para cero resultados; al desaparecer las secciones también se perdía la lectura
  intencional del filtro.
- Solución: `VerticalUpgradesScreenUI` quedó como propietario único de la colección y su
  estado vacío. La consola, navegación y geometría fija permanecen estables; el viewport
  muestra «Completadas: ocultas» mientras el filtro no tiene filas visibles.
- Prevención general: una colección filtrable debe diseñar cero, uno y varios resultados;
  el estado vacío explica el filtro y conserva accesible la forma de retirarlo.
- Evidencia: `Logs/post_0.1.13_final_upgrades_capture.log` y capturas
  `09_upgrades_completed_hidden_es_1080x1920.png` / `09b_upgrades_completed_hidden_es_720x1280.png`.
- Guía actualizada: Prevención 93 y lista de control.
- Estado: INCORPORADO Y VALIDADO EN UNITY; aprobación perceptual en APK pendiente.

## 2026-09-17 — Unity UGUI — Cambiar la resolución de captura invalida la geometría calculada

- Tipo: CAPTURA AUTOMATIZADA, CANVASSCALER Y LAYOUT.
- Situación: la primera captura de Mejoras era correcta, pero la segunda resolución podía
  conservar geometría calculada para el tamaño anterior y recortar contenido.
- Causa confirmada: cambiar RenderTexture o resolución no garantizaba que CanvasScaler,
  anclas y layout se recalcularan antes de leer el siguiente fotograma.
- Solución: recalcular la geometría del CanvasScaler y forzar la actualización del Canvas y
  layout antes de cada captura; restaurar después todos los valores temporales.
- Prevención general: una suite multirresolución debe demostrar que el orden de captura no
  cambia el resultado. La segunda salida es una prueba independiente, no un subproducto de
  la primera.
- Evidencia: `Logs/post_0.1.13_final_upgrades_capture.log` y capturas finales a
  1080×1920 / 720×1280 sin recorte; el guardado temporal fue restaurado.
- Guía actualizada: módulo técnico Unity UGUI, sección Pruebas.
- Estado: INCORPORADO Y VALIDADO EN UNITY.

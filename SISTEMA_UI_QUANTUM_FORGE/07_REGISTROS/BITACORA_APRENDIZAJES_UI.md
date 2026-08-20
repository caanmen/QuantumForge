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

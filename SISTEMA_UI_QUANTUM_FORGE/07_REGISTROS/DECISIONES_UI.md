# Registro de decisiones UI

## Cómo registrar

Cada decisión debe incluir fecha, pantalla, decisión, evidencia, archivos afectados y si aplica a una sola pantalla, una dimensión o todo el proyecto.

## Clasificación obligatoria

- `GENERAL`: proceso o requisito transferible a cualquier dimensión cuando corresponda.
- `DIMENSIÓN`: identidad o contrato exclusivo de la dimensión indicada.
- `PANTALLA`: decisión exclusiva de una pantalla.
- `EJEMPLO HISTÓRICO`: evidencia de origen que no transmite estética a otros diseños.

Una entrada puede originar una regla general y conservar a la vez decisiones específicas.
Deben registrarse por separado para evitar convertir accidentalmente un caso D1 en norma
visual del proyecto.

## 2026-08-19 — Separación entre método general e identidad de dimensión

Alcance: GENERAL.

- Las reglas de fidelidad, estados, evidencia, propietarios, reconstrucción y QA se
  reutilizan entre dimensiones cuando corresponda.
- La paleta, marcos, iconos, fondos, navegación, metáforas visuales y medio gráfico se
  deciden desde la referencia y el perfil de cada dimensión.
- Carta Galáctica, Centro de Mando, Explorar y Hangar son ejemplos históricos D1; no
  constituyen una plantilla visual para D2, D3, D4 o dimensiones futuras.
- Una nueva dimensión permanece sin perfil antes que heredar provisionalmente el de otra.

Evidencia:
- `00_INICIO/INSTRUCCIONES_MAESTRAS_UI.md`
- `01_GUIAS/GUIA_CREACION_CORRECTA_PANTALLAS.txt`
- `01_GUIAS/GUIA_PREVENCION_ERRORES_PANTALLAS.txt`
- `03_ESTILOS/COMUN/CONTRATO_ESTILO_COMUN.txt`

## 2026-08-19 — Auditoría de reutilización antes de crear assets

Alcance: GENERAL.

- Toda pantalla o componente debe superar una Puerta 0 antes de producir recursos.
- La búsqueda incluye catálogo, mapa de reutilización, proyecto real, escenas, prefabs,
  scripts, assets, referencias y capturas aprobadas.
- Cada concepto se registra como origen localizado, ruta exacta, estado, alcance y
  decisión de reutilizar, validar o crear.
- Un asset o componente existente dentro del alcance autorizado se reutiliza; no se
  reemplaza por una aproximación creada desde cero.
- Un diseño aprobado conserva medio gráfico, proporción, ocupación, volumen, densidad y
  nivel de detalle. No se simplifica silenciosamente por comodidad técnica.
- La regla es global; los assets y estilos siguen teniendo alcance COMUN, de dimensión
  o de pantalla. El caso D1 que originó el aprendizaje no transmite estética a D2–D4.

Evidencia:
- `00_INICIO/INSTRUCCIONES_MAESTRAS_UI.md`
- `01_GUIAS/GUIA_CREACION_CORRECTA_PANTALLAS.txt`, sección 62.
- `01_GUIAS/GUIA_PREVENCION_ERRORES_PANTALLAS.txt`, sección 53.
- `04_CATALOGO_ASSETS/MAPA_REUTILIZACION_Y_ORIGEN_UI.md`

## 2026-08-19 — Hangar como autoridad del armazón compartido D1

Alcance: DIMENSIÓN 1.

- Las seis pantallas D1 usan el encabezado, los marcos de tarjetas y los cinco botones
  inferiores de Hangar como contrato visual común.
- Se comparten medidas, fuente, etiquetas, capas Shadow/Fill/Border, iconos premium e
  indicador de selección; el contenido central conserva su composición propia.
- `Dimension1SharedLayoutTokens` es el propietario de las medidas y
  `Dimension1SharedShellApply` aplica el contrato a la escena y a cada constructor.
- Árbol Cuántico separa su banda propia 22 px adicionales para no invadir los recursos.
- Esta decisión no autoriza a reutilizar el estilo D1 en otras dimensiones.

Evidencia:
- `[D1 Shared Shell] APPLY_PASS`.
- Capturas PASS de Centro, Galaxia, Explorar, Hangar, Reliquias y Árbol.
- `[D1 Tree Navigation Runtime] PASS` con raycast físico y estabilidad de 10 s.

## 2026-08-18 — Carta Galáctica D1

Alcance: PANTALLA. Ejemplo histórico de Dimensión 1; no define el estilo de otras dimensiones.

- La apertura inicial es neutral; el sector actual persistente no equivale a previsualización.
- Los títulos de sectores son propiedad del controlador visual nuevo y no pueden ser sobrescritos por el helper antiguo de botones.
- Los sectores exteriores no muestran marcos detrás del número de expediciones; cambia el texto y la ruta seleccionada.
- No se muestran círculos técnicos añadidos alrededor de los planetas de Órbitas Antiguas.
- Los cuerpos animados usan pivote centrado y mantienen su posición al seleccionar.
- Las cuatro rutas tienen la misma longitud, grosor y ángulo reflejado alrededor del Centro Galáctico.
- Las tarjetas de materiales no utilizan la línea vertical decorativa Marker.
- Las navegaciones externas se ocultan mientras la navegación local de Dimensión 1 está activa.

Evidencia:
- `05_REFERENCIAS/DIMENSION_1/CARTA_GALACTICA`
- `06_CAPTURAS_APROBADAS/DIMENSION_1/CARTA_GALACTICA`

## 2026-08-19 — Referencia orbital del Árbol Cuántico D1

Alcance: PANTALLA.

- El usuario seleccionó la variante 3, Constelación orbital, para construir la nueva
  composición estática del Árbol Cuántico.
- Los anillos agrupan nodos por sector sin introducir dependencias nuevas.
- Los nombres, máximos, costes, requisitos, efectos y estados se obtienen del código real.
- El cristal central es decorativo y no constituye una mecánica o botón adicional.
- La referencia es un concepto seleccionado, no una captura real de Unity ni un asset runtime.

Evidencia:
- `05_REFERENCIAS/DIMENSION_1/ARBOL_CUANTICO`
- `10_PANTALLAS/DIMENSION_1/ARBOL_CUANTICO`

## Pendientes de consolidación

- Perfil visual definitivo de Dimensión 2.
- Perfil visual definitivo de Dimensión 3: perfil parcial derivado de Planta de
  Producción aprobada; falta implementación y captura real.
- Perfil visual de Dimensión 4 cuando exista diseño aprobado.
- Catálogo completo de marcos, botones e iconos canónicos.

## 2026-08-19 — Órbitas Antiguas como subpantalla del Sector 3

Alcance: PANTALLA.

- Reutiliza los planetas 4 y 5 de Carta Galáctica, el armazón premium D1, los metales V5
  y la navegación canónica de Hangar.
- Los cuatro destinos nuevos son candidatos detallados; no son canónicos hasta recibir
  aprobación visual explícita.
- Los niveles, producción, progreso hacia metal secundario y costes proceden del estado
  real; la referencia no fija números de juego.
- La raíz es hermana de GalaxyPanel para conservar una autoridad de visibilidad propia.
- La composición estática y la interacción están validadas; falta aprobación del 95 %.

## 2026-08-19 — Base visual aprobada de Dimensión 3

Alcance: DIMENSIÓN y PANTALLA / Planta de Producción.

- El usuario aprobó la composición estática V2 de Planta de Producción.
- D3 utiliza acero ennegrecido, latón envejecido, selección ámbar, acento cian técnico,
  maquinaria física con profundidad y planos técnicos dentro de tarjetas de detalle.
- El encabezado común muestra LE, Trazas y Autómatas con una iconografía única.
- El escenario de QA común usa 3.25M LE, 4.2K Trazas y 56 autómatas.
- Los rótulos visuales de colas son Piezas, Ensamble, Planos y Obras; sus sistemas
  funcionales son Producción, Ensamblaje, Investigación e Instalaciones.
- La numeración de los nombres de archivo no aparece dentro del título de las pantallas.
- Las instalaciones pueden tener ilustraciones físicas distintas, pero reutilizan el
  mismo símbolo de navegación en todas sus apariciones.
- La aprobación es de diseño estático. No hay todavía captura real aprobada de Unity.

Evidencia:
- `10_PANTALLAS/DIMENSION_3/PLANTA_PRODUCCION/CANDIDATA_PLANTA_PRODUCCION_CORREGIDA_V2_1080x1920.png`
- `10_PANTALLAS/DIMENSION_3/PLANTA_PRODUCCION/CONTRATO_APROBACION.txt`
- `03_ESTILOS/DIMENSION_3/PERFIL_ESTILO_DIMENSION_3.txt`

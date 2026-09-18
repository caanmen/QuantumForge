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
- Dimensión 3 cerrada gráficamente con 11 pantallas y capturas reales aprobadas en
  1080×1920 y 720×1280.
- Dimensión 4 no existe en Quantum Forge y no debe diseñarse ni implementarse.
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

## 2026-08-20 — Consolidación funcional y visual de las subpantallas D1

Alcance: DIMENSIÓN 1 y PANTALLAS afectadas.

- `Dimension1GalaxyPremiumSetup.ConfigureReferenceV11` es el único constructor
  ejecutable de Carta Galáctica. V2 y V10 permanecen como implementaciones históricas
  privadas y no pueden reconstruir la escena.
- El texto de selección de Carta pertenece al controlador visual moderno y debe
  permanecer estable inmediatamente después del raycast y tras el refresco posterior.
- Explorar usa únicamente su preview moderno. La lista de destinos refleja el sector
  seleccionado y el reescaneo; los cuatro tipos de nave usan su arte blueprint canónico
  y el panel de detalles consume datos reales.
- Las cuatro subpantallas de sector comparten marco exterior, cabecera despejada y
  navegación con GALAXIA seleccionada. Sus destinos muestran estados reales:
  `NO ESCANEADO`, `BARRIDO EN CURSO`, `NO DISPONIBLE` o disponible sin rótulo.
- Anillo de Restos muestra un solo planeta real centrado. Frontera Silenciosa conserva
  el sprite compartido de P6 y P7 hasta que exista un asset distinto aprobado; no se
  inventa un reemplazo.
- ARK se abre por la ruta física Carta > Centro > ARK, reutiliza el marco D1 y no muestra
  la navegación inferior de cinco pestañas.
- Cuando dos expediciones terminan a la vez, el historial sigue siendo la autoridad de
  resultados y la UI presenta cada identificador pendiente en orden. Cerrar el primer
  modal abre el segundo y cerrar el último deja la pantalla limpia.
- Las capturas nuevas se archivan como candidatas en `10_PANTALLAS`; ninguna se mueve a
  `06_CAPTURAS_APROBADAS` sin aprobación visual explícita del usuario.

Evidencia:

- `[D1 Full 14 Screen Route] PASS | 14/14 pantallas`.
- `[D1 Simultaneous Results] PASS | 2 registros preservados | 2 modales en orden`.
- `[D1 Sector Status+Shell] PASS | 4 sectores | 3 estados + disponible`.
- `[D1 Final Save Compatibility] PASS | partida nueva | partida antigua | JSON actual`.
- Capturas candidatas 1080×1920 y 720×1280 de Carta, Explorar, los cuatro sectores y ARK.

## 2026-08-24 — Continuidad Android y control de peso

Alcance: GENERAL.

- Toda actualización de Quantum Forge conserva paquete, firma y guardado compatible, e
  incrementa versionCode respecto de la última APK distribuida.
- Keystore y contraseñas permanecen fuera del proyecto y de Git.
- Antes de instalar sobre progreso real se extraen principal, `.bak` e históricos; no se
  desinstala ni se borran datos del dispositivo de actualización.
- Cada build entregable registra tamaño, hash y distribución por grupos. Un aumento grande
  requiere archivos responsables concretos y no se justifica sólo por compilación exitosa.
- La optimización puede posponerse hasta estabilizar el arte, pero el coste se mide y registra
  desde la primera build representativa.

Evidencia:
- `08_PRUEBAS/CONTRATO_CONTINUIDAD_ANDROID_QA.md`.
- `01_GUIAS/GUIA_CREACION_CORRECTA_PANTALLAS.txt`, sección 75.
- `01_GUIAS/GUIA_PREVENCION_ERRORES_PANTALLAS.txt`, sección 85.

## 2026-08-24 — Umbral del informe de regreso

Alcance: GENERAL / Progreso sin conexión.

- Por decisión explícita del usuario, el informe de regreso se muestra desde una
  ausencia real de 60 segundos.
- Ausencias menores pueden aplicar progreso sin conexión, pero no abren el informe.
- El cambio afecta únicamente el umbral de presentación; no modifica los límites ni
  las fórmulas de progreso offline de cada dimensión.

Evidencia:
- `Assets/Project/Scripts/Presentation/PresentationReturnReportService.cs`.
- `PresentationBlockP7Validation`, que prueba el límite mediante la constante vigente.

## 2026-08-24 — Colección candidata vigente de Dimensión 2

Alcance: DIMENSIÓN 2.

- Vistas 02–18 usan la colección corregida V4 del 21 de agosto de 2026.
- Vistas 01, 19 y 20 usan sus reemplazos V5.
- V2 y V3 permanecen como historial no aprobado.
- Las veinte vistas siguen agrupadas en cuatro pantallas base y no se convierten en veinte
  raíces independientes.
- La actualización documental no constituye aprobación visual global ni captura de Unity.

Evidencia:
- `10_PANTALLAS/DIMENSION_2/MATRIZ_20_VISTAS.csv`.
- `10_PANTALLAS/DIMENSION_2/CONTINUIDAD_INICIO_DIMENSION_2_2026-08-24.md`.
- `05_REFERENCIAS/DIMENSION_2/CORREGIDAS_V4_2026-08-21/ORIGEN_Y_ESTADO.txt`.
- `04_CATALOGO_ASSETS/CATALOGO_ASSETS_UI.csv`.
- `07_REGISTROS/PANTALLAS_UI.csv`.

## 2026-08-24 — Tres territorios visibles en el Mapa de los Pactos

Alcance: DIMENSIÓN 2 / Mapa de los Pactos.

- El usuario decidió mantener visibles los tres territorios desde el inicio.
- Santuario de Peregrinos conserva su estado disponible.
- Territorios Sometidos conserva el bloqueo por 300 de Confianza.
- Ruinas Sepultadas se muestra en gris y bloqueada hasta que el Dominio total de
  Civilización 2 sea 30% o menos, requisito confirmado por el sistema real.
- No existen líneas ni nodos entre territorios y no se muestran Fragmentos antes de
  que corresponda.
- `Dimension2PanelUI` todavía oculta el tercer botón cuando está bloqueado; esa
  divergencia debe corregirse al implementar la pantalla real en Unity.
- La decisión aprueba esta composición candidata, no constituye captura real ni
  validación funcional en Unity.

Evidencia:
- `05_REFERENCIAS/DIMENSION_2/CORREGIDAS_V4_2026-08-21/01_Mapa_De_Los_Pactos_Corregido_V5.png`.
- `Assets/Project/Scripts/Systems/D2Civilization2System.cs`.
- `Assets/Project/Scripts/UI/Dimension2PanelUI.cs`.

## 2026-08-24 — Contenido de la primera entrada de Dimensión 2

Alcance: DIMENSIÓN 2 / Primera entrada a los Pactos.

- La pantalla real `D2_FirstEntry` aparece una sola vez antes del Mapa de los Pactos.
- El título visible es `DIMENSIÓN 2 · PACTOS` y la única acción es `ABRIR MAPA`.
- Se elimina «Influencia» del texto porque D2 no contiene un recurso o sistema con ese
  nombre. La progresión inicial medible usa Confianza.
- Texto narrativo aprobado por el usuario: «Ante ti aparece un mundo dividido en tres
  territorios. Solo el Santuario de Peregrinos responde a tu llegada.»
- Por decisión explícita posterior del usuario, la introducción conserva sólo ese bloque
  narrativo. Se elimina por completo el segundo panel y no se muestran aquí Seguidores,
  Ofrendas, Peregrinaciones, Confianza ni el requisito de 300.
- La candidata V2 reutiliza el lenguaje de piedra carbón, bronce y las tres identidades
  territoriales de D2. No contiene recursos, pestañas, flecha ni animaciones horneadas.
- El usuario eligió explícitamente como autoridad visual la imagen
  `REFERENCIA_ELEGIDA_D2_PRIMERA_ENTRADA_2026-08-24.png`.
- Las composiciones V1 y V2 se conservan únicamente como historial.
- La referencia elegida no es todavía una captura Unity ni acredita el 95 % de similitud.
- La pantalla ya está construida como composición real en `Main.unity`: fondo, marco,
  título, ilustración, relato y botón son elementos separados.
- La ilustración `D2_FirstEntry_Hero_v1.png` fue derivada en modo edición de imagen
  para cubrir el único faltante visual; no contiene texto, botón, marcos ni UI horneada.
- `ABRIR MAPA` es un botón funcional. La prueba registra `firstEntrySeen`, oculta la
  introducción y muestra `D2_PactMap`.
- La nueva composición tiene versión de primera entrada independiente del marcador
  provisional. Una partida existente la ve una sola vez aunque `firstEntrySeen` antiguo
  ya sea verdadero; al aceptarla se registra la versión vigente.
- Las capturas candidatas 1080×1920 y 720×1280 y las pruebas técnicas están en PASS.
  No se registra todavía aprobación visual del 95 % ni se archiva en `06_CAPTURAS_APROBADAS`.
- Después de comparar la primera captura real con la referencia, se reemplazaron los
  marcos lineales y el botón plano por una revisión ornamental modular. Marco exterior,
  marcos internos y placa del botón son capas separadas; los tres textos y la acción
  permanecen nativos en Unity.

Evidencia:
- `05_REFERENCIAS/DIMENSION_2/PRIMERA_ENTRADA_2026-08-24/REFERENCIA_ELEGIDA_D2_PRIMERA_ENTRADA_2026-08-24.png`.
- `10_PANTALLAS/DIMENSION_2/PRIMERA_ENTRADA/FICHA_PANTALLA.txt`.
- `Assets/Project/Scripts/UI/Dimension2PanelUI.cs`.
- `Assets/Project/Scripts/Editor/Dimension2Block1UISetup.cs`.
- `Assets/Project/Scripts/Editor/Dimension2FirstEntryCapture.cs`.
- `10_PANTALLAS/DIMENSION_2/PRIMERA_ENTRADA/CAPTURAS_CANDIDATAS`.
- `Logs/dimension2_first_entry_build_button_polish_2026-08-24.log`.
- `Logs/dimension2_first_entry_capture_button_polish_2026-08-24.log`.
- `Logs/dimension2_first_entry_versioned_capture_2026-08-24.log`.
- `Logs/dimension2_first_entry_ornate_final_build_2026-08-24.log`.
- `Logs/dimension2_first_entry_ornate_final_capture_2026-08-24.log`.
- `Logs/dimension2_first_entry_ornate_visual_v2_capture_2026-08-24.log`.

## 2026-08-25 — Retiro de Semillas y reserva del sector 4 de la Máquina

Alcance: MÁQUINA / progresión y futuro Monolito 2D.

- Semillas deja de ser una mecánica activa; los sectores funcionales actuales son 1–3.
- Quedan 37 nodos públicos y 6 secretos activos. El 80 % corresponde a 30 públicos.
- Los 20 nodos históricos del sector 4 se conservan sólo para una migración con
  devolución nominal única; no cuentan ni aplican efectos.
- Anclaje Puro, Estable y Forzado se conservan para un posible uso posterior como
  resultados avanzados de Mezclas, ocultos hasta tener un consumidor real.
- Los costes de los 37 nodos públicos se redistribuyen por etapas para conservar el
  ritmo total anterior; los nodos iniciales no aumentan.
- El sector 4 queda reservado para Prestigio 2. En el futuro Monolito será oscuro,
  mostrará `???` y cuatro posiciones decorativas fijas sin información ni interacción.
- Esta decisión no aprueba ni implementa todavía la composición visual del Monolito.

Evidencia:
- `10_PANTALLAS/MAQUINA/DECISIONES.txt`.
- `Assets/Project/Resources/Data/machine_nodes.json`.
- `Assets/Project/Scripts/Systems/MachineManager.cs`.
- `Assets/Project/Scripts/Systems/SaveService.cs`.

## 2026-08-26 — Monolito 2D: centrado V17 y fondo cercano V18/V19

Alcance: MÁQUINA / vista general y primer acercamiento estático al sector 1.

- El centrado perceptual V17 de las cuatro luces de entrada queda aprobado.
- V19 queda aprobado como dirección de composición cercana; continúa siendo un mockup y no
  se incorpora como textura de producción.
- El fondo limpio V18 se integra como capa separada sólo para el sector 1. V08 continúa como
  fondo de la vista general y como solución temporal para las otras caras.
- Monolito, cara, nodos, firmas, fondo y ficha permanecen separados. La animación con
  paralaje se pospone hasta aprobar la captura estática real de Unity.
- La validación funcional pasó en 1080×1920 y 720×1280; la implementación visual real aún
  requiere aprobación perceptual antes de extenderse a los sectores 2–4.

Evidencia:
- `10_PANTALLAS/MAQUINA/DECISIONES.txt`.
- `05_REFERENCIAS/MAQUINA/MONOLITO_2D_CORRECCIONES_INTERACCION_2026-08-26_V02`.
- `Assets/Project/UI/Vertical/Machine/Monolith2D/machine_destroyed_lab_close_dolly_square_v18.png`.
- `Assets/Project/Scripts/Editor/MachineMonolith2DSetup.cs`.
- `Assets/Project/Scripts/UI/MachineMonolith2DVisualUI.cs`.
- `Logs/machine_monolith_close_background_v18_capture.log`.

## 2026-08-26 — Monolito 2D: descarte V22 y corrección V24

Alcance: MÁQUINA / vista general, cuatro sectores y transición de entrada.

- V22 pasó técnicamente pero fue rechazado visualmente por el usuario: redujo demasiado la
  cuarta cara y convirtió el acercamiento en un Monolito completo sobredimensionado.
- V23 corrigió el acercamiento pero su escala 0,90 se descartó al compararla nuevamente con
  la lámina V10 completa.
- V24 usa escala 0,78 en las cuatro caras: conserva la proporción original del sector 1 y
  evita reducir adicionalmente la cara 4.
- El acercamiento V24 es moderado: alcanza 1,55 veces la escala de reposo en vez de 2,15
  unidades absolutas, se dirige a la cara elegida y conserva el laboratorio visible.
- V08 queda en la vista general y V18 en los cuatro sectores; ningún mockup se usa como asset.
- V24 registró PASS técnico con 26 capturas en 1080×1920 y 720×1280, estados, navegación,
  transición y guardado restaurado. La aprobación perceptual continúa pendiente.

Evidencia:
- `Assets/Project/Scenes/Main.unity`.
- `Assets/Project/Scripts/Editor/MachineMonolith2DSetup.cs`.
- `Assets/Project/Scripts/UI/MachineMonolith2DVisualUI.cs`.
- `Assets/Project/Scripts/Editor/MachineMonolith2DCapture.cs`.
- `Logs/VisualQA/MachineMonolith2D`.
- `Logs/machine_monolith_v10_proportion_v24_capture.log`.

## 2026-08-26 — Monolito 2D: apoyo dentro del marco de suelo V25

Alcance: MÁQUINA / cuatro vistas cercanas.

- El usuario rechazó la colocación V24 porque la base de la cara invadía visualmente el
  marco del suelo.
- V25 conserva la escala 0,78 y eleva la cara un 11 % de la altura útil del viewport. La
  base queda dentro del recinto, alineada con el travesaño interior y las guías laterales.
- El ajuste es proporcional para mantener la misma relación en 1080×1920, 720×1280 y en
  el viewport ampliado del sector 4.
- V25 registró PASS técnico con 26 capturas, estados, navegación, transición y guardado
  restaurado. El usuario aprobó explícitamente las cuatro caras y pidió continuar con el
  Monolito; su escala, posición y apoyo quedan fijados.

Evidencia:

- `Assets/Project/Scripts/UI/MachineMonolith2DVisualUI.cs`.
- `Assets/Project/Scripts/Editor/MachineMonolith2DCapture.cs`.
- `Logs/machine_monolith_floor_frame_v25_capture.log`.
- `Logs/VisualQA/MachineMonolith2D`.

## 2026-08-26 — Monolito 2D: paralaje contenido V26

Alcance: MÁQUINA / transición de la vista general a una cara.

- El Monolito avanza 1,25 veces respecto de su escala de reposo y se dirige a la luz tocada.
- El laboratorio avanza 1,07 veces y usa sólo un 10 % del desplazamiento del Monolito.
- V08 cruza gradualmente a V18 antes de que el Monolito domine la sala. El velo máximo es
  0,18 y la evidencia final espera a que la cara esté totalmente opaca.
- V26 registró PASS técnico con 26 capturas en 1080×1920 y 720×1280. El usuario aprobó
  explícitamente el movimiento; V25 y V26 quedan fijados como composición aprobada.

Evidencia:

- `Assets/Project/Scripts/UI/MachineMonolith2DVisualUI.cs`.
- `Assets/Project/Scripts/Editor/MachineMonolith2DCapture.cs`.
- `Logs/machine_monolith_grounded_parallax_v26_capture.log`.
- `Logs/VisualQA/MachineMonolith2D/12_transition_mid_frame_1080x1920.png`.
- `Logs/VisualQA/MachineMonolith2D/13_transition_complete_1080x1920.png`.
- `06_CAPTURAS_APROBADAS/COMUN/MAQUINA_MONOLITO_2D`.

## 2026-08-27 — Monolito 2D: alojamiento ancho V34 y caras abiertas V33

Alcance: MÁQUINA / vista general y cuatro sectores cercanos.

- V32 queda rechazada visualmente: su soporte todavía tenía un hueco frontal más estrecho
  que la base proyectada del Monolito; las caras 1 y 3 se leían como tapas cerradas y la
  cara 4 reutilizaba la orientación del lado derecho.
- El Monolito conserva sin cambios la composición V26 aprobada: escala 0,72 y posición
  `(0, 34)`. El fondo continúa siendo el único propietario del alojamiento.
- V34 desplaza las guías laterales fuera de la huella, ensancha la cama central y mantiene
  un único borde frontal por delante de la base.
- Las caras V33 son superficies de piedra expuesta y continua. Sectores 1 y 4 sitúan la
  columna estructural en el borde derecho; sectores 2 y 3, en el borde izquierdo.
- Los símbolos siguen separados del raster. Sus posiciones se midieron por cara y la prueba
  conserva 64 px visibles, hitbox 104 px y margen completo dentro de la materia.
- La implementación y 32 capturas pasaron en 1080×1920 y 720×1280, incluida transición
  0/25/50/75/100, regreso, Mezclas y restauración del guardado.
- V34 continúa como candidata pendiente de aprobación perceptual explícita del usuario; no
  se archiva todavía como captura aprobada ni se declara 95 %.

Evidencia:

- `Assets/Project/UI/Vertical/Machine/Monolith2D/machine_destroyed_lab_monolith_wide_socket_square_v34.png`.
- `Assets/Project/UI/Vertical/Machine/Monolith2D/monolith_sector_1_open_surface_progression_v33.png` hasta sector 4.
- `Logs/machine_monolith_v34_capture.log`.
- `05_REFERENCIAS/MAQUINA/MONOLITO_2D_CORRECCIONES_INTERACCION_2026-08-26_V02/CANDIDATOS_V34/revision_unity_v34_contact_sheet.png`.

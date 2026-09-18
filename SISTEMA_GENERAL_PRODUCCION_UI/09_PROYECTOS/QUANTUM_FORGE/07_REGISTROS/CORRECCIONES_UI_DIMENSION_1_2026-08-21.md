# Correcciones UI Dimensión 1 — 21 de agosto de 2026

## Alcance aplicado

- Navegación exclusiva Explorar → Centro de Mando moderno.
- Persistencia del resultado hasta una acción explícita del jugador.
- Terminología y CTA de Carta Galáctica, sectores y misión coordinada.
- Etiquetas coherentes del Centro de Mando.
- Magnitudes funcionales actuales y siguientes de las cuatro partes del Hangar.
- Constructores y `Main.unity` reconstruidos con el mismo estado.

## Evidencia de Unity

- Reconstrucción: `Logs/dimension1_ui_corrections_2026-08-21_r2.log` — `APPLY_PASS`.
- Resultado: `Logs/dimension1_result_persistence_validation.log` — modal estable durante
  autoselección y dos resultados en orden.
- Navegación: `Logs/dimension1_full_route_after_corrections.log` — 14/14 mediante raycast.
- Sectores: `Logs/dimension1_sector_status_after_corrections.log` — cuatro sectores y
  estados no escaneado, barrido, no disponible y listo para explorar.
- Carta: `Logs/dimension1_galaxy_text_after_corrections.log` — cuatro nodos estables.
- Explorar/Hangar: `Logs/dimension1_explore_hangar_after_corrections.log` — flujo completo PASS.

## Capturas nuevas

Todas se generaron a 1080×1920 y 720×1280:

- `Logs/VisualQA/dimension1_command_center_integrated_*.png`
- `Logs/VisualQA/Dimension1/V11Reference/Galaxy_v11_*.png`
- `Logs/VisualQA/Dimension1/ExploreReference/Explore_reference_*.png`
- `Logs/VisualQA/Dimension1/HangarReference/Hangar_reference_*.png`
- `Logs/VisualQA/Dimension1/AncientOrbits/Ancient_orbits_*.png`
- `Logs/VisualQA/Dimension1/SectorDetails/*_*.png`
- `Logs/VisualQA/Dimension1/ExpeditionResult/ExpeditionResult_*.png`

## Fuera de alcance conservado

- La revisión gráfica adicional de Explorar solicitada para después.
- Nuevas referencias visuales para Borde Exterior, Anillo de Restos y Frontera Silenciosa.
- Reubicación de la Clave de Acceso Central fuera del Registro de Expediciones.
- Reescritura narrativa de ARK.
- Aprobación perceptual final del usuario.

## Corrección fuente posterior — navegación repetida y controles — 21 de agosto

- Carta Galáctica ahora es propietaria de su botón EXPLORAR. Se retiró la dependencia del
  orden de reconstrucción que podía cerrar Galaxia y exponer la interfaz funcional antigua.
- `ShowExploreScreen` prepara un estado moderno exclusivo: cierra paneles secundarios,
  mantiene oculta la vista antigua y actualiza Explorar antes de presentarlo.
- El acceso a 10 METALES vuelve a enlazar botones reconstruidos aun mientras el inventario
  está oculto, sin acumular conexiones duplicadas.
- DETALLES se lleva al frente, se reinicia arriba y conserva un mensaje explícito para los
  estados sin destino, sin nave, escaneando o bloqueado.
- El panel de apoyo completo permite alternar entre expedición simple y coordinada; al estar
  coordinada muestra `TOCA PARA VOLVER A SIMPLE`.
- El acumulado de cada sector se rotula `N EXPEDICIONES`; sigue contando expediciones
  terminadas, incluidas repeticiones.
- Verificación efectuada sin controlar Unity: `Assembly-CSharp.csproj` compiló con 0 errores.
  La prueba física y las nuevas capturas siguen pendientes porque Unity estaba en uso por el
  usuario y no se modificaron escenas ni assets serializados.

## Rediseños elegidos — Centro, Carta y Explorar — 21 de agosto

- Explorar adopta la composición elegida: sector visible, cuatro destinos reales, nave y
  apoyo paralelos, selector Simple/Coordinada y expediciones activas. Sólo utiliza assets D1.
- Centro de Mando conserva su estructura y datos, pero reemplaza seis glifos simples por
  planeta, sensor, nave, reliquia, árbol y emblema canónicos.
- Carta conserva íntegramente V11 y cambia sólo el arte del Centro Galáctico por el agujero
  negro naranja con nave elegido por el usuario. El bitmap no rota.
- La primera captura de Explorar expuso que el shell común restauraba la geometría anterior;
  la regla ahora identifica `ModePanel` y preserva la composición vigente.
- Reconstrucción final: `Logs/dimension1_selected_redesign_apply_final.log` — `APPLY_PASS`.
- Capturas: Centro, Carta y Explorar a 1080×1920 y 720×1280 — PASS.
- Prueba específica: `Logs/dimension1_explore_command_route_redesign.log` — PASS físico.
- Recorrido integral: `Logs/dimension1_full_route_selected_redesign.log` — 14/14 PASS.
- No se tocó ni cerró la otra instancia de Unity activa, perteneciente a otro proyecto.

## Corrección fuente — 10 METALES y recursos por sector — 21 de agosto

- Se confirmó que algunos constructores recreaban 10 METALES como panel visual sin Button;
  por eso funcionaba en unas pantallas y quedaba inerte en Carta o Explorar.
- El controlador compartido ahora descubre la entrada por su nombre estable, completa la
  cadena Image + Button + targetGraphic cuando falta y conecta una sola ruta al inventario.
- El normalizador del armazón D1 instala además la conexión persistente, de modo que una
  reconstrucción posterior no vuelva a degradar el acceso.
- Los tres recursos del encabezado ya no son una terna fija. Un único propietario actualiza
  nombre, inventario, producción e icono según el sector seleccionado y oculta el tercer
  espacio cuando Anillo de Restos sólo aporta dos metales.
- Los validadores rechazan ahora cualquier acceso sin Button, superficie táctil, raycast o
  ruta persistente.
- `Assembly-CSharp.csproj` y `Assembly-CSharp-Editor.csproj` compilaron con 0 errores. No se
  ejecutó Unity ni se tocaron escenas o assets serializados porque el usuario tenía el
  proyecto abierto; queda pendiente la prueba física y las capturas en ambas resoluciones.

## Corrección fuente — acceso al Registro de Expediciones — 21 de agosto

- Se identificó la lista larga como `D1_ExpeditionRecordVisualRoot`, accesible desde
  REGISTRO en Explorar.
- La apertura dejó de depender solamente del refresco general: ahora activa la raíz si
  fuese necesario, la lleva al frente, habilita su CanvasGroup y reinicia el scroll.
- Explorar consulta el estado real del Registro y desactiva alfa, interacción y raycasts
  mientras la subpantalla está visible; al volver recupera su estado anterior.
- El constructor conecta REGISTRO con el propietario visual moderno y registra la lista
  detallada como panel bloqueante. La ruta actual conserva compatibilidad con el listener
  ya serializado en la escena.
- La prueba integral quedó reforzada para exigir que Explorar no intercepte clics debajo
  del Registro. Runtime y editor compilan con 0 errores; la prueba física queda pendiente
  porque Unity continúa abierto por el usuario.

## Centro de Mando — iconos y cristal aprobados — 21 de agosto

- Se sustituyeron los seis glifos de las tarjetas por la propuesta 3 aprobada: mapa orbital,
  escáner direccional, formación de tres naves, reliquia en contención, árbol de nodos y ruta
  de expedición.
- El cristal central adoptó la propuesta 2 de alto detalle: doble carcasa, núcleo luminoso
  multicapa, cuatro fragmentos flotantes, telemetría adicional y pedestal mecánico.
- `Dimension1CommandCenterSetup` continúa siendo el único propietario y reconstruye la escena
  con ambos cambios; no se introdujeron bitmaps ni un segundo sistema de estilos.
- Instalación: `Logs/d1_command_center_icons_crystal_install.log` — `INSTALL_PASS`.
- Validación: `Logs/d1_command_center_icons_crystal_validate.log` — `VALIDATION_PASS`.
- Captura física: `Logs/d1_command_center_icons_crystal_capture.log` — `CAPTURE_PASS` en
  1080×1920 y 720×1280.
- Compilación de runtime/editor: 0 errores y 0 advertencias.

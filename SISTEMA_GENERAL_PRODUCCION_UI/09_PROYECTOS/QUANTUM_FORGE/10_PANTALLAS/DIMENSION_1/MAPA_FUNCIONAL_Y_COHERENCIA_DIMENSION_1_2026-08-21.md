# Mapa funcional y auditoría de coherencia — Dimensión 1

Fecha: 21 de agosto de 2026  
Alcance: diagnóstico de contenido, navegación y autoridad de la información.  
Estado: correcciones confirmadas aplicadas, reconstruidas y validadas en Unity.

## Cierre de correcciones — 21 de agosto de 2026

- Centro de Mando: `PROGRESO GLOBAL` pasó a `PROGRESO PRESTIGIO 1` y
  `OBJETIVO ACTUAL` a `RECOMENDACIÓN ACTUAL` sin cambiar la fórmula ni crear un
  sistema de objetivos nuevo.
- Navegación: Explorar → Centro de Mando cierra primero todos los estados secundarios y
  muestra exclusivamente la interfaz moderna. Un panel legacy con `activeSelf=true` dentro
  de una rama inactiva ya no se interpreta como visible. El recorrido físico 14/14 incluye
  esta ruta y exige Centro moderno visible, Explorar inactivo e interfaz antigua inactiva.
- Carta Galáctica: muestra cuatro sectores explorables y el Centro por separado; los
  contadores visibles ahora dicen `EXPEDICIÓN/EXPEDICIONES`. Son viajes terminados,
  incluidas repeticiones, no una lista
  de expediciones diferentes. La entrada usa un botón inequívoco `ENTRAR AL SECTOR` o
  `ENTRAR AL CENTRO`.
- Sectores: `DESTINOS DISPONIBLES` pasó a `DESTINOS DEL SECTOR`; una tarjeta escaneada y
  disponible indica `LISTO PARA EXPLORAR` porque el escaneo ocurre en Explorar.
- Explorar: el CTA coordinado dice `INICIAR EXPEDICIÓN COORDINADA` y el bloque de apoyo
  resume los multiplicadores reales. Las selecciones automáticas ya no cierran el resultado.
- Resultado: la modal permanece abierta hasta que el jugador pulsa continuar, incluso
  durante refrescos y autoselección periódica. La ruta física exige al menos 12 fotogramas
  reales consecutivos de visibilidad antes de pulsar `RECOGER Y CONTINUAR`.
- Hangar: cada parte muestra magnitudes funcionales reales y el bloque inferior compara
  el efecto actual con el siguiente nivel; blindaje y sensores ya no repiten el nivel.
- No existe una migración general por nombre de destino: varios destinos son válidos en más
  de un sector. La partida afectada sí pudo repararse con sus registros explícitos y la
  convención histórica de los registros ausentes. Se conservó un respaldo previo.
- Los valores QA 23/12/1 habían alcanzado esa partida porque las capturas reactivaban las
  escrituras justo antes de cerrar Unity. La supresión queda ahora enclavada durante todo el
  proceso; todas las pruebas posteriores conservaron el mismo hash del guardado.
- Permanecen pendientes de decisión la fila fija ARK del Registro, los textos narrativos de
  ARK y referencias visuales propias para Borde, Anillo y Frontera.

## Cómo leer este documento

- **Confirmado**: la información y la ruta tienen respaldo en el sistema funcional actual.
- **Revisar**: el contenido existe y funciona, pero su ubicación, nombre o función visual necesita una decisión del usuario.
- **Incoherencia confirmada**: el rótulo visible no describe con precisión el dato que calcula el código.
- **QA solamente**: valores que aparecen en capturas deterministas, pero no se fijan en una partida normal.

El código confirma una ruta física de 14 pantallas. Las páginas de Reliquias, los estados
neutral/seleccionado, el cajón de dimensiones y los filtros son estados internos, no pantallas
de producción adicionales.

## Arquitectura general

### Núcleo principal

1. Centro de Mando.
2. Carta Galáctica.
3. Explorar.
4. Hangar.
5. Cámara de Reliquias.
6. Árbol Cuántico.

Las cinco últimas comparten navegación local. Centro de Mando sirve como hub y también abre
esas cinco áreas.

### Subpantallas de Carta Galáctica

7. Borde Exterior.
8. Anillo de Restos.
9. Órbitas Antiguas.
10. Frontera Silenciosa.
11. ARK — Centro Galáctico.

### Subpantallas contextuales

12. Inventario de Metales.
13. Expedición Completada.
14. Registro de Expediciones.

## Matriz pantalla por pantalla

| N.º | Pantalla | Para qué sirve | Información que muestra | Entrada principal | Salidas y destinos | Estado de coherencia |
|---:|---|---|---|---|---|---|
| 1 | Centro de Mando | Hub y resumen de D1 | Sector actual, escáner, flota, reliquias, puntos del Árbol, expediciones activas, tres metales, recomendación y progreso de Prestigio 1 | Entrada a D1 / regreso desde pantallas principales | Carta, Explorar, Hangar, Reliquias, Árbol, Inventario y cajón de dimensiones | **Corregido y validado** |
| 2 | Carta Galáctica | Elegir y previsualizar una zona | Cuatro sectores explorables, Centro Galáctico separado, exploraciones, planetas, destinos, requisitos y metales | Centro o navegación Galaxia | Cuatro detalles sectoriales, ARK, Centro, Explorar, Hangar, Reliquias, Árbol e Inventario | **Corregido y validado** |
| 3 | Explorar | Escanear, seleccionar e iniciar expediciones | Sector actual, escáner, destinos del sector, nave, apoyo, modo simple/coordinado, expediciones activas, preview, metales y temporizadores | Centro, navegación Explorar o tarjeta de destino | Registro, resultado automático, Centro, Carta, Hangar, Reliquias, Árbol e Inventario | **Rediseñado y validado**; aprobación perceptual pendiente |
| 4 | Hangar | Gestionar las cuatro naves activas | Nave, cuatro partes, niveles, costes, desbloqueo, mejora y magnitud actual/siguiente del efecto | Centro o navegación Hangar | Centro, Carta, Explorar, Reliquias, Árbol e Inventario | **Corregido y validado** |
| 5 | Cámara de Reliquias | Coleccionar y mejorar reliquias | 20 reliquias en tres páginas, origen, tier, nivel, hitos, dos efectos, coste y estado | Centro, navegación Reliquias o resultado con reliquia | Centro, Carta, Explorar, Hangar, Árbol e Inventario | **Confirmado**: las 20 y sus efectos están documentados; aprobación visual pendiente |
| 6 | Árbol Cuántico | Comprar progreso D1 | 10 nodos, saldo, niveles, efectos, coste, requisitos y estados | Centro o navegación Árbol | Centro, Carta, Explorar, Hangar, Reliquias e Inventario | **Confirmado** funcionalmente; composición visual pendiente |
| 7 | Borde Exterior | Gestionar planetas 1–2 y entrar a destinos del Sector 1 | Hierro/Cobre, Aluminio/Titanio, niveles, producción, costes y cuatro destinos oficiales | Carta, nodo Borde Exterior | Carta; cada destino abre Explorar preseleccionado; navegación D1 e Inventario | **Revisar diseño**: datos reales, pero no existe referencia visual específica permanente |
| 8 | Anillo de Restos | Gestionar planeta 3 y entrar a destinos del Sector 2 | Níquel/Cobalto, nivel, producción, costes y cuatro destinos oficiales | Carta, nodo Anillo de Restos | Carta; cada destino abre Explorar preseleccionado; navegación D1 e Inventario | **Revisar diseño**: datos reales, pero no existe referencia visual específica permanente |
| 9 | Órbitas Antiguas | Gestionar planetas 4–5 y entrar a destinos del Sector 3 | Metales, niveles, producción, costes y cuatro destinos oficiales | Carta, nodo Órbitas Antiguas | Carta; cada destino abre Explorar preseleccionado; navegación D1 e Inventario | **Confirmado** contra una referencia específica; aprobación visual pendiente |
| 10 | Frontera Silenciosa | Gestionar planetas 6–7 y entrar a destinos del Sector 4 | Metales —incluidos tres en planeta 7—, niveles, producción, costes y cuatro destinos oficiales | Carta, nodo Frontera Silenciosa | Carta; cada destino abre Explorar preseleccionado; navegación D1 e Inventario | **Revisar diseño**: datos reales, pero no existe referencia visual específica permanente |
| 11 | ARK — Centro Galáctico | Investigar, sincronizar y completar el cierre de D1 | Investigación, cuatro sincronías de 60 min, cuatro ecos, 12 requisitos, misión final de 90 min y Ancla Galáctica | Carta, nodo Centro Galáctico | Carta e Inventario; acciones internas de investigar, sincronizar y entrar | **Revisar texto narrativo**: mecánicas confirmadas, algunas frases sólo tienen autoridad en la UI |
| 12 | Inventario de Metales | Consultar los diez metales sin abandonar la pantalla de origen | Cantidad, producción, procedencia, bloqueo, total y filtros | Tarjeta “10 METALES” | Regresa exactamente a la pantalla anterior | **Confirmado** |
| 13 | Expedición Completada | Presentar un resultado real | Destino, nave, duración, 0–4 metales, fragmentos, matriz, reliquia y punto especial | Automática al terminar una expedición | Continuar a Explorar; abrir Reliquias sólo si hubo reliquia | **Corregido y validado**: permanece hasta acción explícita |
| 14 | Registro de Expediciones | Consultar hasta 20 resultados recientes | Destino, sector, nave, apoyo, sinergia, metales, matriz, reliquia, filtros, totales y una entrada fija de ARK | Botón Registro en Explorar | Regresar a Explorar e Inventario | **Revisar ubicación**: la entrada fija de ARK no es una expedición y no cuenta en el total |

## Incoherencias confirmadas

### 1. “PROGRESO GLOBAL” no calcula el progreso global de Dimensión 1

La pantalla Centro de Mando muestra un porcentaje llamado **PROGRESO GLOBAL**, pero el
controlador lo calcula con `CalculateD1TreePointsFromProgress` dividido por
`Dimension1Prestige1PreviewPointCap`. Es una previsualización limitada de puntos de progreso
para el Árbol/Prestigio 1; no mide todo el avance de la dimensión.

Decisión aplicada: se conservó el cálculo vigente y se renombró el bloque a
**PROGRESO PRESTIGIO 1**, que describe su autoridad real sin inventar otra fórmula.

### 2. “5 SECTORES” cuenta al Centro Galáctico como sector

El catálogo vigente distingue **cuatro sectores explorables más Centro Galáctico**, pero Carta
Galáctica presenta el total como `x/5 SECTORES`. El código usa cinco estados sectoriales por
comodidad funcional, pero el rótulo no conserva la distinción conceptual.

Decisión aplicada: la interfaz diferencia **4 sectores explorables + Centro**, sin alterar el
estado interno que el sistema utiliza para el Centro Galáctico.

## Elementos que requieren decisión creativa

### 3. “OBJETIVO ACTUAL” del Centro de Mando

El objetivo cambia entre cuatro frases escritas directamente en el controlador:

- Explora 3 señales desconocidas.
- Espera el resultado del escáner.
- Completa las expediciones activas.
- Ancla galáctica asegurada.

No existe un sistema independiente de misiones u objetivos que sea propietario de ese texto.
Funciona como consejo contextual creado por la UI. Debe confirmarse si el Centro tendrá un
objetivo real, una recomendación contextual o ningún bloque de objetivo.

### 4. Entrada fija de ARK dentro del Registro de Expediciones

La entrada “Clave de Acceso Central” tiene respaldo en el código legacy y en la implementación
actual, pero no es una expedición. Por eso se reserva una fila 21 y se excluye del contador de 20.
La lógica es consistente; la ubicación conceptual puede confundir. El usuario debe decidir si
permanece allí, pasa a ARK/Carta o se presenta como evento separado.

### 5. Tres detalles sectoriales no tienen referencia visual propia

Borde Exterior, Anillo de Restos y Frontera Silenciosa reutilizan el armazón de Órbitas Antiguas.
Sus planetas, metales, destinos y acciones son reales, pero la distribución visual no proviene de
una referencia específica de esas tres pantallas. No hay evidencia para afirmar que cada bloque
de información esté en la ubicación creativa definitiva.

### 6. Textos narrativos del ARK

Frases como “La entrada espera una coincidencia” e “Investiga la nave para revelar su patrón de
acceso” viven en el controlador visual y no en una fuente narrativa canónica separada. No cambian
la mecánica, pero necesitan aprobación editorial antes de considerarse lore definitivo.

### 7. Valores de partes en Hangar

En una partida normal, cada tarjeta ya muestra `NIVEL x/y` y el segundo valor vuelve a mostrar
`NIVEL x`. La mecánica sí posee multiplicadores distintos para carga, velocidad, blindaje y
sensores. La pantalla no inventa un número, pero duplica información y no explica la magnitud
real que el jugador está mejorando.

## Valores de referencia que no contaminan la partida normal

Los siguientes ejemplos se aplican sólo cuando la captura activa el modo QA visual:

- Explorar: 2.41 UA, 120 UA/s, resumen coordinado ×4/×2.5 y 00:28:45.
- Hangar: 45K, 12 matrices y preview funcional de nivel actual/siguiente.
- Reliquias: 5.98M/4.73M/4.70M y el escenario del Cristal Analítico.
- Centro de Mando: la referencia usa 42 %, niveles y contadores ilustrativos, aunque la
  implementación normal refresca los valores desde el estado real.
- Carta Galáctica: 23/12/1 son acumulados ilustrativos del escenario V11. La partida
  reparada conserva 33 resultados reales distribuidos 26/1/1/5 entre los cuatro sectores.

Estos números no son constantes normales del juego. Deben seguir etiquetados como escenarios de
captura y no convertirse en decisiones de balance.

## Evidencia final de esta corrección

- `[D1 Explore -> Command Center] PASS | botón físico | Centro moderno exclusivo inmediato y sostenido`.
- `[D1 Explore Change Sector] PASS | Cambiar sector vuelve a Explorar | Galaxia normal abre planetas`.
- `[D1 Galaxy Sector Text Cycle] PASS | 4 sectores | clic físico | texto estable inmediato y después de 0.25 s`.
- `[D1 Explore+Hangar Functional] PASS` con semántica de Blindaje y Sensores verificada.
- `[D1 Full 14 Screen Route] PASS | 14/14 pantallas`, incluyendo resultado estable durante
  12 fotogramas reales y regreso final al Centro moderno.
- Capturas nuevas de Centro, Carta, Hangar y Resultado en 1080×1920 y 720×1280.
- Capturas candidatas nuevas del Centro con iconos lineales detallados y emblema táctico en
  1080×1920 y 720×1280; cristal y fondo central sin cambios.
- Hash del guardado antes y después de QA: `A4F29E703DB1CAB06942B5A2224686AC7F665B1C481808422B2EBC90C15019A9`.
- Respaldo previo a la reparación: `save.json.pre_d1_counter_repair_20260821_1155`.

## Contradicciones documentales que conviene corregir después de decidir el diseño

- Hangar: la ficha, contrato y registro maestro dicen **aprobada**, pero el archivo de origen de
  la referencia todavía dice implementación en curso y aprobación pendiente.
- Centro de Mando: el origen llama a la pantalla “aprobada”, mientras la ficha, contrato y
  registro maestro mantienen pendiente la aprobación visual final.
- Carta Galáctica: conserva capturas dentro de `06_CAPTURAS_APROBADAS`, pero el registro maestro
  indica que la evidencia quedó anterior a cambios posteriores y la aprobación visual actual está
  pendiente.
- ARK: el registro usa `D1_ARK_CENTRO_GALACTICO`, pero la ruta física actual valida la raíz
  `ArkPanel`. No afecta al jugador, pero dificulta rastrear una única identidad interna.

## Fuentes contrastadas

- Reglas, instrucciones maestras y las dos guías completas de UI.
- Perfil de estilo de Dimensión 1.
- Las fichas, contratos, decisiones y referencias de las 14 pantallas.
- Registro maestro, decisiones UI, mapa de reutilización y auditoría integral anterior.
- Controladores visuales y funcionales reales de las pantallas.
- Constructor de Centro de Mando y validador físico de la ruta 14/14.
- Capturas y referencias disponibles; no se trataron los mockups como capturas reales.

## Decisiones que todavía requieren aprobación

1. Decidir si la Clave de Acceso Central pertenece al Registro de Expediciones.
2. Aprobar referencias visuales propias para Borde, Anillo y Frontera.
3. Aprobar o reescribir el texto narrativo de ARK.
4. Aprobar perceptualmente los rediseños candidatos de Centro de Mando, Carta y Explorar.

## Actualización Carta Galáctica V12 — 21 de agosto de 2026

- La composición V11 de cuatro hexágonos fue reemplazada por la referencia orbital
  elegida por el usuario: siete cuerpos, siete trayectorias y Centro separado.
- Nombres aprobados y mapeo funcional:
  Elysia/Vulkar -> Borde Exterior;
  Corona de Tántalo -> Anillo de Restos;
  Mnemos/Orpheon -> Órbitas Antiguas;
  Nyxara/Erebon -> Frontera Silenciosa.
- Los nombres son presentación; los IDs persistentes planet_01…planet_07 no cambiaron.
- El mapa ya no presenta acumulados históricos de expediciones. Muestra sector, ACTUAL,
  disponible o bloqueado según el estado real.
- Los cuerpos no orbitan alrededor del Centro: mantienen posición y rotan sobre su eje.
- El Centro no gira como una imagen completa. Su recorte negro elimina el rectángulo,
  una forma elíptica oculta rutas posteriores y la corrección final oscurece el arte y
  retira el anillo/halo exterior que competía con los cuerpos.
- Evidencia nueva:
  `[D1 Carta Galactica V12] CONFIGURATION_PASS | 7 cuerpos | 7 órbitas | nombres canónicos | centro integrado`;
  `[D1 Carta Galactica V12] INTERACTION_PASS | 7 cuerpos | 7 órbitas | rotación axial | posiciones estables | sin contadores de expediciones`;
  `[D1 Galaxy Sector Text Cycle] PASS | 7 cuerpos | clic físico | 4 sectores correctos | estados sin contadores estables`;
  capturas reales neutral/seleccionada en 1080×1920 y 720×1280.
- Estado: validación técnica PASS; aprobación perceptual de las capturas V12 pendiente.

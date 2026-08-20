# Mapa de reutilización y origen UI

## Propósito

Impedir que una pantalla nueva empiece desde cero cuando Quantum Forge ya contiene el
mismo concepto, asset, componente o patrón aprobado. Este mapa es global; las entradas
específicas conservan siempre su alcance COMUN, de dimensión o de pantalla.

## Regla de consulta

Antes de producir cualquier pantalla:

1. Buscar el concepto en `CATALOGO_ASSETS_UI.csv`.
2. Buscar su nombre visible, ID y función en escenas, prefabs, scripts y carpetas UI.
3. Revisar su primera aparición y las capturas aprobadas correspondientes.
4. Completar la matriz de la ficha de pantalla.
5. Reutilizar el recurso exacto si su alcance y estado lo autorizan.
6. Registrar un faltante sólo después de demostrar que no existe un recurso adecuado.

No se considera reutilización copiar visualmente un asset y redibujarlo de forma más
simple. Reutilizar significa referenciar el mismo sprite, prefab, componente, material,
tema o generador, o extraer un componente compartido que conserve exactamente el
resultado ya aprobado.

## Jerarquía de alcance

- `COMUN`: autorizado para todo el juego.
- `D<n>`: autorizado dentro de la dimensión indicada.
- `PANTALLA`: exclusivo hasta que dos casos aprobados justifiquen generalizarlo.
- `candidato`: puede probarse, pero no se presenta como canónico ni se propaga solo.

## Matriz obligatoria por pantalla

| Concepto | Primera aparición | Ruta exacta | Estado | Alcance | Decisión |
|---|---|---|---|---|---|
| Ejemplo | Pantalla donde apareció | Asset, prefab, script o material | canónico/candidato/faltante | COMUN/Dn/PANTALLA | reutilizar/validar/crear |

La matriz debe incluir encabezado, navegación, marcos, fondos, iconos, ilustraciones,
efectos, tipografía, controles y patrones de estado. No basta inventariar imágenes PNG.

## Inventario inicial de Dimensión 1

Esta sección no define el estilo de otras dimensiones. Sólo registra orígenes ya
existentes para trabajos D1.

| Familia o concepto | Origen comprobado | Estado y uso |
|---|---|---|
| Marco técnico D1 | `Assets/Project/UI/Dimension1/Generated/d1_premium_frame_v4.png` | Canónico D1; reutilizar sin redibujar |
| Relleno técnico D1 | `Assets/Project/UI/Dimension1/Generated/d1_panel_fill_v4.png` | Canónico D1 por pantallas aprobadas |
| Campo estelar D1 | `Assets/Project/UI/Dimension1/Generated/d1_starfield.png` | Canónico D1; fondo separado |
| Brillo técnico D1 | `Assets/Project/UI/Dimension1/Generated/d1_glow_v3.png` | Canónico D1; efecto, no contenido |
| Anillo orbital D1 | `Assets/Project/UI/Dimension1/Generated/d1_orbit_ring_v3.png` | Canónico D1 por Carta Galáctica |
| Navegación local D1 | `Assets/Project/UI/Dimension1/Generated/NavigationPremium/d1_nav_*_premium_v1.png` + materiales black key | Cinco sprites premium canónicos D1; los `d1_nav_*_v3.png` quedaron sustituidos |
| Iconos de hogar y metales D1 | `Assets/Project/Scripts/Editor/Dimension1HangarReferenceSetup.cs` | Resultado aprobado en Hangar; reutilizar geometría exacta o consolidar un componente compartido, no redibujar aproximaciones |
| Naves blueprint D1 | `Assets/Project/UI/Dimension1/Generated/d1_hangar_*_blueprint_v2.png` | Canónicas D1 por Hangar aprobado |
| Nave y dron sólidos | `Assets/Project/UI/Dimension1/Generated/d1_explore_*_solid_v2.png` | Candidatos; conservar clasificación |
| Reliquias e iconos sólidos | `Assets/Project/UI/Dimension1/Generated/Candidates/Relics` | Candidatos; reutilizar sólo como candidatos hasta aprobación |

## Auditoría vigente — Árbol Cuántico D1

| Elemento | Recurso previo localizado | Decisión obligatoria |
|---|---|---|
| Encabezado, metales y Centro | Hangar/Relics D1 y sus instaladores | Reutilizar el resultado exacto; no volver a dibujarlo |
| Marcos, relleno, estrellas y brillo | Assets canónicos D1 listados arriba | Reutilizar los mismos archivos |
| Navegación inferior | `Generated/NavigationPremium/d1_nav_*_premium_v1.png` + `Dimension1PremiumNavigationApply` | Reutilizar los cinco sprites premium y su tratamiento; no volver a los pictogramas simples |
| Cristal central y Lectura de Reliquias | `Candidates/Relics/d1_relic_analytic_crystal.png` | Evaluar como candidato contra la referencia orbital |
| Registro de Copias | `Candidates/Relics/d1_relic_lost_navigation_record.png` | Evaluar como candidato; no forzar si cambia la identidad |
| Preparación/Coordinación de Hangar | Blueprints y navegación Hangar existentes | Reutilizar sólo donde coincida el concepto; producir faltante coherente si la referencia exige otra composición |
| Estabilización/Galaxia | Assets y navegación de Carta Galáctica | Reutilizar o derivar desde el asset autorizado, nunca un garabato nuevo |
| Iconos sin coincidencia exacta | Referencia orbital permanente | Registrar cada faltante y crear assets candidatos con el mismo nivel de detalle |
| Órbitas y conexiones | `d1_orbit_ring_v3.png` más referencia orbital | Reconstruir densidad y proporción; no reducir a elipses vacías |

### Matriz cerrada — nodos del Árbol Cuántico D1

| Nodo | Origen reutilizado | Clasificación durante la reconstrucción |
|---|---|---|
| Lectura de Destinos | Patrón de escáner de `Dimension1ExploreReferenceSetup.cs` | Geometría D1 consolidada |
| Preparación de Hangar | `d1_hangar_sonda_ligera_blueprint_v2.png` | Canónico D1 |
| Registro de Copias | `Candidates/Relics/d1_relic_lost_navigation_record.png` | Candidato; no promovido |
| Memoria de Escaneo | Patrón de escáner de `Dimension1ExploreReferenceSetup.cs` | Geometría D1 consolidada |
| Lectura de Reliquias | `Dimension1ExploreReferenceSetup.DrawNavigationIcon`, variante 3 | Constructor técnico D1 reutilizado |
| Rastreo de Hallazgos Ocultos | Lenguaje de búsqueda técnico D1 | Geometría D1 consolidada |
| Coordinación de Flota | Tres instancias del constructor técnico de sonda, variante 2 | Constructor técnico D1 compuesto |
| Optimización de Ruta | Patrón de ruta y balizas de Explorar | Geometría D1 consolidada |
| Cartografía Avanzada | `Candidates/Tree/d1_tree_advanced_cartography_v2.png` | Candidato nuevo; faltante real |
| Estabilización de Zona Inestable | Constructor técnico de galaxia, variante 0 | Constructor técnico D1 reutilizado |

No se autoriza reemplazar estos orígenes por pictogramas más simples en revisiones
posteriores. El candidato de Cartografía debe conservarse como candidato hasta que la
pantalla completa sea aprobada.

## Auditoría vigente — Órbitas Antiguas D1

| Elemento | Origen comprobado | Decisión obligatoria |
|---|---|---|
| Planetas 4 y 5 | `d1_body_planet_ancient_v3.png` y `d1_body_planet_silent_v3.png` | Reutilizar los cuerpos de Carta Galáctica sin recrearlos |
| Marcos, relleno y estrellas | Assets canónicos D1 | Reutilizar el mismo sistema visual |
| Metales y navegación | Inventario V5 y navegación premium D1 | Reutilizar sprites y tratamiento de Hangar |
| Cuatro destinos | `Generated/Candidates/AncientOrbits/d1_destination_*_v2.png` | Candidatos nuevos por faltante real; no promover antes de aprobación |
| Datos planetarios | `GameState` y `Dimension1System` | Mostrar niveles, producción, costes y bloqueos reales |

La subpantalla se mantiene como hermana de Carta Galáctica dentro del propietario D1;
no debe anidarse bajo un panel cuyo cierre también la desactive.

## Auditoría vigente — ARK Centro Galáctico D1

| Elemento | Origen comprobado | Decisión obligatoria |
|---|---|---|
| Marcos, relleno y estrellas | Assets canónicos D1 | Reutilizar el mismo sistema visual |
| Metales | `Generated/MetalsInventory/d1_metal_*_v5.png` | Reutilizar el conjunto vigente |
| Cuatro naves de sincronía | `d1_hangar_*_blueprint_v2.png` + `d1_hangar_blueprint_keyed.mat` | Reutilizar sin crear variantes y centrar por silueta visible |
| Señal investigada | `NavigationPremium/d1_nav_galaxy_premium_v1.png` + material black key | Reutilizar dentro de insignia técnica propia de ARK |
| Ecos y bloqueo | Geometría medida de la referencia + bloqueo D1 | Reconstruir forma/espaciado de pantalla sin crear un sprite sustituto |
| Hero ARK y agujero negro | `Generated/Candidates/ArkCenter/d1_ark_center_hero_candidate_v1.png` | Candidato nuevo por faltante real; no promover antes de aprobación |
| Misiones y requisitos | `Dimension1PanelUI` + `Dimension1System` | Conservar lógica, 60m/90m y bloqueos reales |

La pantalla es una subpantalla con flecha propia y no incorpora navegación inferior.

## Cómo ampliar este mapa

Al comenzar una nueva dimensión, crear su subsección sólo a partir de su perfil y de
pantallas realmente aprobadas. No copiar la tabla D1. Cuando un recurso demuestre uso
idéntico en varias dimensiones y el usuario lo apruebe como compartido, promoverlo a
COMUN y registrar la decisión.

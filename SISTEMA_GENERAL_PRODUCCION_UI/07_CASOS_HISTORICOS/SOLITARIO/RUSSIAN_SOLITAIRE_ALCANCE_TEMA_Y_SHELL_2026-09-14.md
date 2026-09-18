# Russian Solitaire: separar identidad temática, shell común y maestro universal

Fecha: 2026-09-14  
Estado: validado  
Alcance reutilizable: nuevas modalidades o temas añadidos a un producto que ya posee una
interfaz compartida y componentes visuales universales aprobados.

## Problema observado

Al iniciar la propuesta visual de Russian Solitaire se interpretó la petición de que cada
solitario fuera distinto como autorización para rediseñar toda la pantalla. La maqueta cambió
la cabecera, el estilo y la composición de los botones inferiores y el lenguaje general del
producto. También dibujó cartas simplificadas propias en vez de reutilizar el maestro
universal A–10 que ya estaba aprobado y bloqueado.

La propuesta no se aplicó en Unity ni modificó `Assets`, por lo que el error fue reversible.
Sin embargo, producir más variantes a partir de ella habría multiplicado dos regresiones:
fragmentación de la experiencia común y creación de una segunda geometría de cartas.

## Causa

El alcance visual no se dividió antes de producir en tres propietarios distintos:

1. **Shell común del producto:** cabecera, barra inferior, acciones, posiciones, navegación,
   modales, estados compartidos y lenguaje de interacción.
2. **Núcleo universal de cartas:** rangos, palos, conteos, topología, posiciones, tamaños y
   zonas reservadas gobernados por el contrato `LOCKED`.
3. **Identidad temática permitida:** fondo de mesa, reverso, marco, ornamentos, figuras y
   paleta propia de la modalidad.

La puerta inicial comprobaba resolución, siete columnas, cuatro bases y 52 cartas, pero no
exigía comparar el shell contra una captura vigente ni demostrar la ruta y el hash del maestro
universal. Por eso podía declarar correcta una composición estructural que seguía usando
fuentes visuales equivocadas.

## Corrección adoptada

- La primera maqueta queda registrada sólo como ejemplo rechazado y no se integra.
- Antes de otra propuesta se toma una captura vigente del juego y se congelan sus capas
  comunes mediante una matriz `componente / propietario / puede cambiar / evidencia`.
- La identidad distinta se limita a las capas temáticas expresamente autorizadas.
- Todo A–10 se compone desde
  `04_PLANTILLAS/CONTRATO_GEOMETRIA_UNIVERSAL_CARTAS.json`, estado `LOCKED`, hash
  `ea3603284333a5698b9db8ea293184ce530a58d98d7964bad569c224d12c7b77`.
- El marco nuevo se adapta a las zonas universales; no se crean cartas provisionales ni se
  ajustan posiciones o escalas por modalidad.
- Se presenta una sola unidad y se detiene el trabajo hasta la aprobación explícita.

## Regla consolidada

"Cada modalidad debe verse distinta" significa identidad temática distinta, no shell de
producto distinto. En una aplicación existente, ningún concepto visual puede cambiar
controles, navegación, jerarquía compartida o composición de cabecera y pie sin autorización
explícita. La novedad estética se diseña dentro de las capas tematizables declaradas.

Si existe un componente universal aprobado, un mockup no puede sustituirlo por una
aproximación, placeholder o reinterpretación. Debe referenciar su fuente y hash reales desde
el preflight. La ausencia de esa prueba bloquea incluso una propuesta no aplicada.

## Pruebas mínimas

1. Comparación antes/después del shell completo con superposición o inventario de rectángulos.
2. Igualdad de acciones, orden, etiquetas y posiciones de la barra común.
3. Lista explícita de capas temáticas cambiadas y de capas comunes conservadas.
4. Verificación automática de estado `LOCKED` y hash del maestro universal.
5. Rechazo deliberado de una fixture que cambia botones o usa un hash inexistente.
6. Captura de una única unidad a escala móvil antes de autorizar implementación o lote.

El contrato ejecutable de alcance vive en
`09_PROYECTOS/SOLITARIO/CONTRATO_NUEVAS_MODALIDADES_VISUALES.json`.

## Segundo incidente: maestro correcto usado como cara opaca

Fecha: 2026-09-15  
Estado: corregido técnicamente en candidate-v4; aprobación estética aún pendiente del usuario.

Después de corregir el shell y reutilizar el maestro A–10 real, candidate-v2 y candidate-v3
todavía componían el PNG completo del maestro encima del fondo de abedul. La geometría era
correcta, pero la cara marfil opaca ocultaba casi toda la identidad temática recién creada.
Candidate-v3 añadió además un marco cuyo RGB conservaba datos de una cuadrícula en píxeles
transparentes; aunque el alfa los ocultaba en ese render, podían reaparecer al filtrar,
reescalar o cambiar el modo de composición.

La causa fue confundir “usar el maestro universal” con “usar su imagen completa como capa”.
El maestro es autoridad del contenido y de su geometría, pero no propietario de la superficie
temática. Cuando una modalidad tiene una cara propia, el contenido A–10 se deriva de forma
determinista del maestro sin trasladar ni escalar su tinta y queda transparente en el resto.

La corrección candidate-v4 separó tres propietarios y los recompuso en este orden:

1. superficie temática de abedul;
2. tinta roja o negra derivada del maestro LOCKED en el lienzo original de `512 x 768`;
3. marco ruso con interior protegido transparente y RGB neutralizado donde alfa es cero.

La prueba automática cubrió el `10` de corazones y el `10` de picas, verificó hashes y firma
LOCKED, confirmó variación visible del abedul en una muestra sin tinta, regeneró las máscaras
de tinta sin transformación, rechazó invasión del marco y limitó las diferencias de la
pantalla a los 31 rectángulos de cartas boca arriba. No se modificaron `Assets` ni Unity.

### Defensa bloqueante añadida

- Todo entregable visual modular declara `visual_layer_composition`.
- La fase `unit` exige el criterio adicional `VLC01` con evidencia automática.
- Una cara completa opaca sobre el fondo temático falla aunque provenga del maestro y tenga
  el hash correcto.
- Un marco con interior opaco, un segundo fondo, un parche rectangular o basura RGB bajo
  alfa cero falla.
- El render final debe coincidir con la recomposición determinista de las capas aisladas.
- El PASS técnico no equivale a aprobación estética; la implementación sigue bloqueada hasta
  la aprobación explícita del usuario.

## Tercer incidente: importación NPOT y zonas interactivas opacas

Fecha: 2026-09-15  
Estado: corregido y convertido en pruebas runtime bloqueantes.

Durante la integración aprobada, el PNG de fondo de `1080 x 1632` era correcto, pero Unity
lo cargó inicialmente como `1024 x 2048` por el ajuste automático NPOT. La prueba que afirmó
las dimensiones del recurso cargado falló y evitó aceptar una captura deformada. La
importación quedó con `nPOTScale: 0`, mipmaps desactivados y bordes en clamp.

La primera captura D3D reveló además siete zonas de columna verde opacas. Eran objetivos de
arrastre funcionales heredados del shell Builder, pero actuaban como una segunda superficie
encima del abedul. Russian Solitaire conserva esos objetivos y sus raycasts, con color y
contorno transparentes. Una aserción recorre las siete columnas y bloquea cualquier alfa
visible; la captura final confirma continuidad del fondo.

Defensas permanentes:

1. validar dimensiones del `Texture2D` cargado, no sólo del archivo fuente;
2. rechazar redimensionado NPOT cuando el arte tiene proporción contractual;
3. separar geometría interactiva y decoración visible;
4. afirmar alfa cero de slots no autorizados y revisar el tablero completo;
5. regenerar la captura después de corregir cualquiera de estas capas.

## Cuarto incidente: selector interactivo pero visualmente vacío

Fecha: 2026-09-15  
Estado: corregido y convertido en prueba PlayMode bloqueante.

Al ampliar el selector común para once modalidades, el contenido se puso dentro de un
`ScrollRect`. El viewport era una `Image` completamente transparente con un `Mask` clásico.
En runtime los once botones existían y sus zonas seguían abriendo los juegos, pero el gráfico
transparente no escribía un stencil visible y ocultaba imágenes y textos de todos los hijos.

La corrección sustituyó únicamente ese componente por `RectMask2D`, sin cambiar cabecera,
desafío diario, geometría de tarjetas, botones inferiores, navegación ni Safe Area. La prueba
permanente bloquea el regreso de `Mask`, exige `RectMask2D`, verifica que KLONDIKE no esté
culled al abrir y que RUSSIAN SOLITAIRE no esté culled después de desplazar al final. Una
captura D3D de `1080 x 1920` demuestra además el render visible en contexto.

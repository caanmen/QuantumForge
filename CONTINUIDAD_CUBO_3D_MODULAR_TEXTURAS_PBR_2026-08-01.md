# Continuidad — Cubo 3D modular y texturas PBR

Fecha: 2026-08-01

## Instrucción para comenzar el siguiente chat

Continuar el trabajo visual de la Máquina leyendo primero este archivo y revisando el estado real del proyecto. No rehacer la Máquina desde cero: ya existe un cubo 3D físico, modular, con cuatro caras, rotación real, nodos físicos y perfiles gráficos. El siguiente bloque consiste en escoger y adaptar las texturas PBR recién compradas para acercar el cubo al diseño industrial original, aumentar la profundidad percibida y conservar buen rendimiento en Android.

Antes de ejecutar Unity en modo batch o regenerar `Main.unity`, confirmar que la instancia normal de Unity esté cerrada. Al crear este resumen Unity estaba abierto y el paquete acababa de importarse.

## Objetivo principal aprobado por el usuario

La Máquina debe ser un objeto real 3D, no una imagen que cambia por otra ni un plano con una transición. Su aspecto debe acercarse mucho más a la referencia original:

- metal industrial oscuro, envejecido y creíble;
- paneles, tuberías, canaletas, tornillos, uniones y placas con lectura clara;
- profundidad física visible incluso cuando una cara está casi frontal;
- receptáculos de nodos integrados en la geometría de la Máquina;
- desgaste, suciedad, arañazos, variación de rugosidad y pequeños bordes pulidos;
- luces funcionales contenidas, sin convertir el cubo en una superficie de neón;
- apariencia dañada pero todavía operativa al comienzo;
- mejora visual progresiva a medida que se reparan los nodos y sectores.

La referencia deseada sigue siendo la Máquina compleja de la imagen original:

- `C:\Users\nedfla\AppData\Local\Temp\codex-clipboard-401e1a5d-9141-4f90-82ec-f5d3ece0062b.png`

La captura más reciente del cubo modular antes de aplicar el paquete PBR es:

- `C:\Users\nedfla\AppData\Local\Temp\codex-clipboard-16e65224-bd4d-4800-a71d-5601a3482e9d.png`

El cubo modular actual gustó como base, especialmente sus luces y su rotación, pero sus materiales todavía se perciben planos, simples y demasiado cercanos a una maqueta.

## Decisiones visuales que deben respetarse

### Geometría y nodos

- No regresar a las antiguas fotografías por cara.
- Mantener la construcción modular: placas, marcos, canales, receptáculos y nodos deben ser objetos que puedan ajustarse o recolocarse.
- El receptáculo visible de cada nodo debe ser el propio elemento interactivo.
- La zona pulsable puede ser invisible y algo más amplia para móvil, pero debe quedar centrada exactamente sobre el receptáculo.
- No colocar un marco gris separado encima del nodo. Ese marco parecía un botón superpuesto, competía con el diseño de fondo y además quedaba desalineado.
- La selección debe notarse con una respuesta integrada: iluminación del borde físico, pulso contenido, cambio de emisión o profundidad/sombra; no con un HUD flotante descentrado.
- Los nodos necesitan más relieve físico para seguir pareciendo piezas reales al mirarlos de frente.
- El cubo debe conservar proporciones cuadradas. No volver a una silueta aproximada de 7 cm de ancho por 6 cm de alto.
- El borde lateral exageradamente salido que se veía en capturas anteriores no debe regresar. Las esquinas deben formar el volumen del cubo de manera coherente.

### Paleta e iluminación

- Oscurecer los materiales. El usuario no quiere grises claros ni colores luminosos dominantes.
- Base: negro grafito, acero muy oscuro, hierro envejecido y pequeñas variaciones cálidas/frías.
- Azul/cian para Cuarto 1, morado para Fusiones y los colores propios de las demás caras deben funcionar como acentos, no como pintura principal.
- Mantener las luces que gustaron, pero integrarlas en ranuras, indicadores y circuitería.
- Evitar saturación excesiva, superficies lavadas o una estética limpia de nave nueva.

### Daño y reparación

- Las dos líneas rojas simples usadas anteriormente no se leen como daño: parecen un bug visual. No reutilizar ese recurso aislado.
- El daño debe construirse con señales creíbles y combinadas: placa desplazada o rota, abolladura, grieta, borde quemado, carbonización, metal expuesto, suciedad, fuga de energía o parpadeo localizado.
- Una cara dañada todavía debe comunicar que la Máquina funciona parcialmente.
- Al reparar, reducir gradualmente daños, recuperar continuidad de placas/canales, estabilizar luces y limpiar parte de la suciedad. No convertirla de golpe en una máquina completamente nueva.
- La progresión visual debe depender del estado funcional real; no duplicar la lógica de progreso o guardado.

## Paquete de texturas comprado e importado

Paquete:

- `50 - 4K PBR Sci-Fi Materials vol.2`
- Licencia comprada: Single Entity.
- Carpeta importada: `Assets/SciFi_Materials_vol2/`
- Contiene 50 familias: `SciFiPanels01` hasta `SciFiPanels50`.
- Cada familia incluye mapas con sufijos `_a`, `_h`, `_m`, `_n` y `_o`, además de un material de muestra.

Interpretación esperada de los mapas, que debe comprobarse en Unity antes de conectarlos:

- `_a`: albedo/base color;
- `_h`: height;
- `_m`: metallic;
- `_n`: normal;
- `_o`: occlusion.

El proyecto usa Universal Render Pipeline `17.3.0`. Los materiales originales del paquete aparecen configurados con el shader Standard del renderizador clásico. No asumir que quedarán correctos al arrastrarlos sobre el cubo.

Ruta técnica recomendada:

1. No modificar los materiales originales del proveedor.
2. Inspeccionar visualmente los 50 diseños y preseleccionar entre tres y cinco que encajen con la referencia.
3. Crear materiales derivados propios con URP/Lit dentro de la zona del proyecto, por ejemplo en `Assets/Project/UI/Vertical/Machine/Prototype3D/Materials/PBR/`.
4. Conectar Base Map, Normal, Metallic y Occlusion correctamente. Evaluar si Height aporta una mejora útil mediante parallax o si es demasiado costoso/inestable para móvil.
5. Aplicar primero una prueba controlada a una cara y a varios tipos de piezas: superficie, marco, módulos y receptáculos.
6. Tomar capturas comparables antes de extender el material al cubo completo.
7. Usar variaciones de material compartidas y geometría modular; no crear una textura única de 4K para cada nodo.

No ejecutar una conversión global de todos los materiales del paquete sin revisar el resultado. Es preferible conservar intacta la carpeta comprada y mantener nuestras adaptaciones separadas.

## Qué textura estamos buscando

La mejor candidata no será necesariamente la más detallada o brillante. Debe tener:

- paneles industriales oscuros de escala compatible con el cubo;
- detalles finos visibles sin competir con las tuberías y módulos 3D;
- buen normal map y variación de rugosidad para romper la sensación plana;
- arañazos/desgaste moderado, no ruido uniforme;
- pocos colores incorporados para que las emisiones por cara sigan controladas por el juego;
- patrones que no delaten repetición al cubrir placas grandes;
- lectura aceptable al reducirse a resolución móvil.

Probablemente se necesitará una combinación pequeña, no un único material para todo:

- una textura principal oscura para placas y superficie;
- una variante más robusta para marco exterior y blindaje;
- un metal diferente para tuberías, canales y piezas móviles;
- materiales propios de emisión para luces y estados;
- decals o materiales localizados para daño, suciedad y reparación.

La geometría debe seguir aportando las siluetas y el relieve grande. Las texturas PBR deben aportar microdetalle, respuesta a la luz y desgaste, no intentar reemplazar toda la forma física con una imagen.

## Rendimiento y opciones gráficas para Android

El paquete es 4K, pero eso no significa que las texturas deban llegar a Android en 4K. El objetivo es conservar una única base artística y variar el costo según el perfil gráfico.

Punto de partida recomendado, que debe medirse en dispositivo:

- Baja: mapas principales de hasta 512, menos detalles opcionales, sin parallax y efectos reducidos.
- Equilibrada: mapas de hasta 1024, normal y occlusion esenciales, detalles modulares medios.
- Alta: mapas de 1024 o 2048 solo donde la diferencia sea visible, geometría opcional completa y mejor respuesta de iluminación.

Además:

- usar compresión Android apropiada y comprobar artefactos, especialmente en normales;
- conservar mipmaps para caras que se ven en perspectiva durante la rotación;
- compartir materiales y texturas entre módulos;
- evitar copias de materiales en tiempo de ejecución;
- limitar transparencias, luces dinámicas y parallax costoso;
- medir memoria, tamaño del APK, tiempo de carga y FPS antes de aprobar la configuración final.

Ya existe un sistema de tres perfiles en `MachineCube3DQuality.cs` y controles en Ajustes. Debe ampliarse o aprovecharse, no crear otro sistema paralelo.

## Estado actual del cubo modular

- Cuatro caras físicas registradas mediante `MachineCube3DFace`.
- Más de 100 módulos identificables y más de 430 renderizadores de geometría de relieve en la validación actual.
- Cámara en perspectiva y renderizado a RenderTexture.
- Rotación física real del mismo cubo, usando swipe y flechas.
- Calidad Baja, Equilibrada y Alta.
- Estados visuales de daño/reparación y selección preparados en código.
- Total físico actual: 43 sockets, formados por 35 públicos y 8 secretos preparados.
- Distribución pública visible: 7 + 11 + 7 + 10.
- Con los dos secretos reservados en cada cara: 9 + 13 + 9 + 12.

Distribución por cara:

- Cara 1 — Enlace con el Cuarto 1: 7 públicos + 2 secretos.
- Cara 2 — Sector de Fusiones: 11 públicos + 2 secretos.
- Cara 3 — Soporte Interno: 7 públicos + 2 secretos.
- Cara 4 — Cámara de Anclajes/Instantánea: 10 públicos + 2 secretos.

Los secretos ya tienen espacio físico reservado; no deben colocarse como botones encima del fondo ni improvisarse después.

## Rotación en reposo pendiente de cerrar

El usuario pidió que el cubo se muestre un poco más girado en reposo para que se reconozca claramente como cubo y se vea una porción del lateral, sin perder protagonismo de la cara activa.

- Código del controlador: `restingYawOffsetDegrees = 14f`.
- Generador/editor: configura `14f`.
- La escena `Assets/Project/Scenes/Main.unity` todavía conserva serializado `5.5` grados porque Unity estaba abierto y aún no se regeneró la escena.

Por tanto, el cambio no debe declararse terminado. Cuando Unity esté cerrado, regenerar/aplicar de forma segura, comprobar el ángulo en 1080x1920 y ajustarlo visualmente si 14 grados resulta excesivo. El valor exacto no está aprobado hasta que el usuario vea la captura o el juego.

## Archivos principales

- Escena: `Assets/Project/Scenes/Main.unity`
- Generador del prototipo: `Assets/Project/Scripts/Editor/MachineCube3DPrototypeSetup.cs`
- Controlador de giro/calidad: `Assets/Project/Scripts/UI/MachineCube3DPrototypeController.cs`
- Entrada y selección: `Assets/Project/Scripts/UI/MachineCube3DPrototypeDisplayUI.cs`
- Nodo físico: `Assets/Project/Scripts/UI/MachineCube3DNode.cs`
- Cara modular: `Assets/Project/Scripts/UI/MachineCube3DFace.cs`
- Módulo modular: `Assets/Project/Scripts/UI/MachineCube3DModule.cs`
- Estado visual: `Assets/Project/Scripts/UI/MachineCube3DVisualStateController.cs`
- Perfiles gráficos: `Assets/Project/Scripts/UI/MachineCube3DQuality.cs`
- Materiales actuales: `Assets/Project/UI/Vertical/Machine/Prototype3D/`
- Paquete comprado: `Assets/SciFi_Materials_vol2/`
- Captura y QA: `Assets/Project/Scripts/Editor/MachineCubeBlock1Capture.cs`

## Licencia y repositorio

La licencia Single Entity permite usar y modificar los materiales dentro de Quantum Forge, publicar y monetizar el juego sin que el paquete añada una regalía o porcentaje sobre las ventas. No se pueden revender, regalar ni distribuir los archivos originales o modificados como paquete de texturas independiente.

No subir los archivos fuente comprados a un repositorio público. Antes de incluir `Assets/SciFi_Materials_vol2/` en Git, confirmar que el repositorio remoto sea privado y que solo tengan acceso personas cubiertas por la licencia. El APK o juego compilado sí puede incluir los materiales integrados normalmente.

## Estado de trabajo y precauciones

- El árbol de trabajo contiene numerosos cambios previos de gameplay, UI, guardado y Máquina. Todos pertenecen al usuario y deben preservarse.
- La carpeta `Assets/SciFi_Materials_vol2/` está actualmente sin seguimiento en Git.
- No hacer reset, checkout destructivo ni descartar cambios.
- No hacer commit, push ni añadir los assets comprados al repositorio sin solicitud explícita y sin revisar antes la privacidad del remoto.
- No ejecutar dos instancias de Unity a la vez.
- No regenerar `Main.unity` mientras el editor normal esté abierto, porque podría sobrescribir la escena.
- No cambiar balance, costes, requisitos, progresión ni guardado durante este bloque visual.

## Primer bloque exacto del siguiente chat

1. Confirmar que Unity terminó de importar el paquete.
2. Si hay que regenerar la escena o ejecutar batch, pedir al usuario cerrar Unity.
3. Auditar las 50 familias de `SciFiPanels` mediante miniaturas/albedos y sus mapas asociados.
4. Elegir tres a cinco candidatos oscuros que se acerquen a la referencia, explicando brevemente las diferencias.
5. Crear una prueba URP no destructiva con los mejores candidatos, preferiblemente sobre una sola cara o piezas representativas.
6. Comparar visualmente la prueba con la referencia y mostrarla al usuario antes de aplicar materiales al cubo completo.
7. Después de que el usuario apruebe la dirección, integrar los materiales elegidos, ajustar desgaste/iluminación, aplicar el ángulo de reposo pendiente y validar Android/perfiles gráficos.

## Criterios de aceptación del siguiente bloque

- La cara frontal deja de parecer una foto o una maqueta plana.
- El metal tiene respuesta PBR clara pero oscura y controlada.
- Los normales no sustituyen el relieve grande: los nodos y módulos siguen siendo físicamente profundos.
- La textura no domina ni vuelve ilegibles los receptáculos.
- Las luces mantienen el lenguaje de color por cara y no lavan el metal.
- El daño parece daño mecánico real, no líneas rojas flotantes.
- El nodo seleccionado se reconoce inmediatamente y sigue integrado en la pieza.
- El cubo se reconoce como volumen incluso en reposo.
- No hay materiales rosados/incompatibles por usar shaders Standard en URP.
- El perfil Bajo continúa siendo viable en Android y el perfil Alto muestra una mejora visible real.

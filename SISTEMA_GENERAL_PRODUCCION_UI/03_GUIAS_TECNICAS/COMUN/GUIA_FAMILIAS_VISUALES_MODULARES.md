# Guía general de familias visuales modulares

Alcance: cualquier videojuego que utilice colecciones de elementos repetidos con una
estructura visual común, como cartas, fichas, retratos, objetos, marcos, insignias,
botones ilustrados o piezas de tablero.

## 1. Definir el contrato del asset

Antes de producir imágenes se debe declarar qué representa cada archivo:

- **Elemento completo:** contiene fondo, marco, decoración y contenido. El juego lo
  presenta como una sola superficie y no vuelve a añadir esas capas.
- **Capa interior:** contiene únicamente retrato, ilustración, textura o información. El
  juego añade el fondo y marco compartidos.
- **Composición modular:** un conjunto de capas con propietarios y orden explícitos.

No mezclar contratos dentro de una misma familia. Una imagen de elemento completo colocada
dentro de otra base completa produce bordes dobles, escalas incoherentes y aspecto de
recurso superpuesto.

## 2. Separar geometría compartida y contenido variable

Cuando varios elementos deban parecer parte de una misma familia:

1. Crear una plantilla maestra con tamaño, relación de aspecto, marco, zonas reservadas,
   máscara y guías.
2. Bloquear esa geometría antes de producir todas las variantes.
3. Cambiar sólo las capas variables: personaje, objeto, emblema, color autorizado o fondo.
4. Registrar cualquier excepción intencional en el perfil de la familia.

Generar cada variante completa de forma independiente puede alterar grosor del marco,
curvas, márgenes y zonas reservadas aunque las imágenes parezcan similares por separado.

### Separar el shell común de la identidad temática

En un producto existente, pedir una identidad distinta para una modalidad o tema no
autoriza rediseñar la interfaz compartida. Antes del concepto se divide la pantalla en:

- **shell común inmutable:** cabecera, barra de acciones, botones, orden, navegación,
  modales, estados compartidos, Safe Area y posiciones estructurales;
- **componentes universales:** geometría, datos o piezas aprobadas que se reutilizan por
  fuente, versión y hash;
- **capas tematizables:** fondo, textura, marco, reverso, ornamento, figuras y paleta dentro
  de límites expresamente autorizados.

La propuesta debe superponerse o compararse contra una captura vigente del shell. Cambiar
un botón, su orden, su tamaño, el encabezado o la composición general es `FAIL` salvo
autorización explícita. La variedad visual se obtiene dentro de las capas tematizables, no
creando una interfaz paralela para cada modalidad.

Si existe un maestro bloqueado, una maqueta no puede sustituirlo con cartas simplificadas,
placeholders ni una reinterpretación generativa. El preflight debe registrar la ruta, estado
y hash de la fuente universal; su ausencia bloquea incluso una propuesta todavía no aplicada.

### Orden de dependencia obligatorio

Cuando ya existe contenido universal, no se empieza por el marco decorativo. Primero se
inventaría y mide ese contenido en su escala final, usando sus casos más anchos, altos y
densos. Después se bloquean el campo útil, las zonas reservadas, los márgenes y las máscaras;
una prueba geométrica limpia debe demostrar que todas las variantes caben. Sólo entonces se
diseña el marco alrededor de esa geometría aprobada.

El marco es una envolvente del contrato, no su autoridad. Se prohíbe reducir, deformar o
desplazar contenido universal aprobado para rescatar un marco creado demasiado pronto. Una
excepción estética sólo se permite mediante un perfil propio medido que conserve las
invariantes del contenido. El orden ejecutable se conserva en
`04_PLANTILLAS/CONTRATO_ORDEN_PRODUCCION_GRAFICA.json`.

En índices de cartas nuevos o modificados, el palo debe conservar una altura de tinta
visible mínima del `8.4 %` del ancho de la carta, redondeada al píxel más cercano. El marco
puede disminuir o variar el tamaño de sus huecos únicamente mientras la envolvente completa
`rango + palo`, su separación y sus márgenes sigan cabiendo. Nunca se reduce el símbolo para
rescatar el marco. Además, el auditor mide los huecos realmente pintados: superior e inferior
deben corresponderse tras una rotación de 180 grados y sus dimensiones pueden diferir como
máximo `1 px`.

## 3. Zonas reservadas y perfiles de colocación

Una zona para número, texto, icono, precio o estado debe definirse mediante límites
normalizados respecto al elemento final. Dentro de una familia con la misma plantilla, esos
límites deben coincidir.

Si distintas familias usan marcos con geometrías diferentes, cada una necesita su propio
perfil de colocación. No se debe aplicar una única coordenada global a aberturas de forma o
tamaño distintos.

Un índice simple no recibe automáticamente un círculo, placa, cápsula ni fondo decorativo.
Se prueba primero el glifo solo dentro de una zona limpia. Un contenedor se añade únicamente
cuando pertenece al lenguaje visual aprobado o cuando una medición demuestra que es necesario
para contraste o legibilidad; debe usar el tamaño mínimo y no invadir marco ni ornamentos.

Para un bloque compuesto por letra, cifra, símbolo o icono:

- centrar primero la huella visible del conjunto;
- definir tamaño, separación y línea óptica comunes;
- limitar mediante el perfil la ocupación interior del conjunto: centrar el bloque completo
  no autoriza que el rango quede pegado al borde exterior ni que el palo se separe en exceso;
- comprobar glifos estrechos, anchos y con descendentes;
- aplicar correcciones ópticas por variante sólo después de estabilizar el conjunto;
- validar a tamaño real, porque una ampliación puede ocultar problemas de legibilidad.

La ocupación interior se calcula sobre los píxeles visibles del render y se divide por la
altura del hueco visible de la fuente limpia. Cada familia declara un máximo en su perfil.
Para las barajas de Solitario que adopten el perfil compacto vigente, el máximo es `60 %`;
superarlo bloquea la unidad aunque el centro global coincida.

La ocupación máxima no sustituye la **separación interna mínima visible**. En cada bloque
`rango + palo`, las cajas conservadoras de ambas huellas deben dejar aire entre sí. En la
referencia móvil vigente de `220 × 348 px`, se exigen al menos `4 px` libres; el mismo mínimo
se cumple en los índices superior e inferior y para todos los rangos `A–10, J, Q y K`.
El índice inferior conserva exactamente la misma separación después de rotarlo `180 grados`.
Una fixture donde las huellas se toquen o se solapen debe fallar aunque el bloque completo
esté centrado y no supere el `60 %` de ocupación.

El perfil de colocación se selecciona por la estructura visible del bloque, no por un valor
literal concreto. Todos los bloques equivalentes —por ejemplo, `rango solo`— deben compartir
la misma zona base aunque uno contenga un glifo ancho como `10` y otro uno estrecho como `7`.
Los bloques `rango + palo` pertenecen a otra clase y pueden usar una zona más alta. El
generador debe afirmar antes de guardar que cada variante recibió la clase, dimensiones y
centro previstos; comprobar únicamente `si rango == 10` deja otros rangos numéricos expuestos
a una zona incorrecta.

## 4. Proporción y encuadre

- Nunca ensanchar o comprimir una ilustración para llenar el lienzo.
- Conservar relación de aspecto y elegir explícitamente entre contener, cubrir o recortar.
- Usar máscara cuando el contenido deba cubrir una ventana fija.
- Comparar rostro, cuerpo, objeto principal y puntos de apoyo después del recorte.
- Evaluar el asset sobre el fondo y a la escala donde realmente aparecerá.

El lienzo final puede tener una proporción fija aunque las fuentes sean diferentes. La
normalización correcta recorta o añade margen controlado; no deforma el contenido.

## 5. Producir una unidad representativa antes del lote

Antes de generar una colección completa se aprueba una unidad que contenga el mayor riesgo:

- el contenido más ancho o alto;
- el glifo o cifra con peor ajuste;
- la ilustración con rostro o anatomía más sensible;
- el marco con la zona reservada más limitada;
- el estado con más indicadores simultáneos.

La aprobación debe ocurrir dentro del juego o producto real. Después se automatiza el lote
con la misma plantilla y parámetros; no se reinterpretan manualmente por archivo.

## 6. Fuente definitiva y regeneración

Cada familia debe registrar:

- plantilla y capas fuente;
- script o herramienta de generación;
- resolución y configuración de importación;
- carpeta de salida de producción;
- versión o fecha de la evidencia aprobada;
- parámetros por familia y excepciones autorizadas.

El generador debe ser reproducible y no sobrescribir silenciosamente assets aprobados con
fuentes antiguas. Regenerar dos veces debe producir la misma estructura y conservar los
contratos de capas.

## 7. Revisión de la colección

La validación mínima incluye:

1. Una cuadrícula de contacto con todas las variantes al mismo tamaño.
2. Recortes ampliados de zonas críticas.
3. Capturas reales dentro del juego o producto.
4. Comparación de proporción, marco, zona reservada y alineación.
5. Estados, fondos y escalas representativos.
6. Revisión de la variante completa después del último cambio.

Una variante correcta no demuestra que toda la colección sea consistente. Una cuadrícula
permite detectar deriva entre marcos, tamaños, márgenes y estilos; el render real confirma
que la composición de capas y el escalado también son correctos.

## 8. Datos exactos dentro de arte generado

Los elementos decorativos pueden producirse o retocarse mediante herramientas generativas,
pero una herramienta generativa no es autoridad para información finita. Se consideran
datos exactos las cifras, letras, palabras, palos, cantidades de símbolos, estados, precios,
fechas, insignias y cualquier marca cuya identidad o cardinalidad tenga significado.

Para esos datos:

1. Definir una tabla canónica con valor, cantidad, posiciones, topología de filas o columnas,
   separaciones mínimas, simetría y orientación esperadas.
2. Generar o retocar únicamente el arte sin datos cuando sea posible.
3. Componer los datos mediante código, plantilla vectorial o capas reproducibles.
4. Declarar si el índice está horneado en el asset o lo dibuja el runtime; nunca ambos.
5. Validar la estructura automáticamente y revisar el render al tamaño final y al 100 %.
6. Contar por separado los símbolos centrales y el símbolo que identifica el palo en el
   índice; el criterio debe estar escrito antes de aprobar.

En ninguna carta numérica basta con validar cantidad y centro promedio. El validador debe
agrupar los símbolos detectados en el render y comparar la firma de filas o columnas con el
patrón canónico. También debe medir la separación visible entre grupos, el centro de los
límites exteriores y la orientación de los símbolos inferiores. La tabla obligatoria del
proyecto se conserva en
`04_PLANTILLAS/PATRONES_CANONICOS_CARTAS_NUMERICAS.json` y cubre del As al `10` para los
cuatro palos. Ningún valor queda exento por no haber aparecido en la unidad representativa.

Las firmas vigentes son: `A = 1`, `2 = 1-1`, `3 = 1-1-1`, `4 = 2-2`, `5 = 2-1-2`,
`6 = 2-2-2`, `7 = 2-1-2-2`, `8 = 2-1-2-1-2`, `9 = 2-2-1-2-2` y
`10 = 2-1-2-2-1-2`. En el `10`, esto representa dos mitades tradicionales `2-1-2`, con
cinco símbolos derechos arriba y cinco rotados abajo. La alternativa `2-2-1-1-2-2`, una
distribución amontonada o cualquier variante no autorizada debe fallar aunque conserve diez
símbolos y el promedio parezca centrado. El mismo principio se aplica a los otros nueve
valores según su propia firma. La orientación se declara por fila: el `7` inglés es `5/2`
y su asimetría vertical tradicional no debe eliminarse para forzar un promedio de `0.5`.

La escala del patrón también pertenece al perfil de cada marco. Un patrón puede tener
conteo, topología y centro correctos y aun verse como una columna estrecha dentro de un
campo ancho. Antes de aprobar cada tema se mide la ocupación horizontal visible de los
símbolos sobre el campo útil detectado de forma independiente. Una fixture amontonada en
un campo ancho debe fallar; no se autoriza una única escala para marcos con aberturas
visiblemente distintas.

Una lámina de contacto no debe volver a generar las unidades aprobadas. Se ensambla desde
los archivos finales mediante coordenadas y tamaños bloqueados. Así se evitan cartas
repetidas, inclinadas, recortadas o con cantidades reinterpretadas durante la presentación.

Cuando una unidad sea incorrecta, se sustituye la capa propietaria. Colocar una carta
completa o un rectángulo opaco encima de otra deja marcos dobles, fondos tapados, cambios de
textura y aspecto de pantalla superpuesta. Toda reconstrucción debe conservar continuidad
de luz, grano, perspectiva y decoración alrededor de la máscara.

## 9. Validadores visuales no circulares

Un validador visual debe medir la salida renderizada contra una autoridad independiente.
Comprobar que un bloque cabe en las mismas coordenadas que lo generaron no demuestra
centrado. Del mismo modo, copiar `esperado` en `producido` y asignar `PASS` sólo demuestra
que existe una tabla, no que el render la cumpla.

Para índices, símbolos o colecciones repetidas:

1. Medir la huella visible del render, no sólo el tamaño nominal de fuente o contenedor.
2. Compararla contra el centro y los márgenes del hueco visible en la fuente aprobada.
3. Medir por separado centro medio y límites exteriores cuando una distribución tenga
   varias unidades.
4. Obtener cantidades producidas mediante componentes, OCR, metadatos extraídos u otra
   lectura independiente de la salida.
5. Incluir fixtures negativas desplazadas, faltantes y adicionales, además de una fixture
   válida que demuestre que el validador no falla siempre.
6. Cuando haya patrones repetidos, incluir fixtures con conteo y centro correctos pero
   topología, separación o simetría incorrectas.
7. Cuando haya bloques compuestos, incluir una fixture con ocupación interior excesiva.
8. Ejecutar la auditoría antes de guardar el lote y devolver código distinto de cero ante
   cualquier celda inválida.
9. Si el marco contiene un hueco de índice claro junto a un campo central también claro,
   el detector debe identificar el componente estrecho y aislado del hueco. Se prohíbe
   elegir una región sólo porque tenga color marfil o porque su centro esté cerca de una
   esquina. La caja completa de tinta del rango y el palo debe quedar dentro de los límites
   visibles de ese componente; comparar contra el mismo centro equivocado que produjo el
   render es un falso positivo bloqueante.
10. Medir la ocupación horizontal visible de cada rango dentro de su hueco. Una `Q`, `K`
    o `10` puede tener el centro numérico correcto y aun rozar ambos bordes. Los rangos
    anchos usan un perfil horizontal propio sin cambiar la altura tipográfica; la fixture
    sin escala debe fallar.
11. Un índice compuesto se valida como bloque visible completo, no sólo como rango aislado.
    Capturar las capas `full`, `rank_only`, `suit_only` y `clean` a tamaño móvil para cada
    rango A–10/J/Q/K y los cuatro palos. Medir ambas esquinas, los dos ejes, la separación,
    la contención y la equivalencia rotada. La cobertura parcial o un valor `N/A` bloquean.
12. Medir el tamaño visible del palo contra el ancho final de la carta y medir los huecos
    realmente pintados, no sólo los rectángulos declarados. Una fixture con palo al `6.25 %`
    o con huecos superior/inferior desiguales debe fallar.

## 10. Puerta de cierre

Esta puerta especializada aporta evidencia a `U04`, `U05`, `U06`, `U08`, `U09`, `U10` y
`U11` de la puerta universal. No se sustituye el resultado ejecutable por marcar esta lista.

- [ ] El contrato de cada asset está declarado.
- [ ] La geometría compartida procede de una plantilla maestra.
- [ ] El contenido universal y sus fixtures extremas se midieron a escala final antes de diseñar el marco.
- [ ] El marco se diseñó alrededor de zonas reservadas ya bloqueadas; no se encogió contenido aprobado para hacerlo caber.
- [ ] Las zonas reservadas coinciden dentro de cada familia.
- [ ] Cada familia diferente tiene un perfil de colocación propio.
- [ ] Ninguna ilustración fue deformada para llenar el lienzo.
- [ ] Una unidad representativa fue aprobada antes del lote.
- [ ] La colección completa fue revisada en cuadrícula y mediante recortes.
- [ ] Existe evidencia real posterior al último cambio.
- [ ] La fuente y el generador reproducen los resultados aprobados.
- [ ] Los datos exactos proceden de una plantilla, código o capa reproducible.
- [ ] Cada cantidad fue comprobada automáticamente y mediante conteo visual.
- [ ] Cantidad producida, centrado y márgenes se obtuvieron del render y no de las mismas constantes usadas para generarlo.
- [ ] Cada bloque compuesto respeta el máximo de ocupación interior de su perfil; una fixture extendida queda bloqueada.
- [ ] Todo bloque `rango + palo` conserva la separación interna visible mínima de su perfil arriba y abajo; en la referencia móvil vigente son `4 px` para A–10 y J/Q/K.
- [ ] La tinta visible de cada rango queda centrada horizontalmente y por debajo de la ocupación máxima de su hueco; los rangos anchos tienen aire lateral verificable.
- [ ] Los valores del As al `10` y los cuatro palos están presentes en la matriz canónica; ninguno se valida por excepción narrativa.
- [ ] Cada patrón numérico coincide en conteo, topología, separación, centrado aplicable y orientación por fila con su tabla canónica.
- [ ] Cada marco tiene un perfil de escala propio y la ocupación horizontal de la huella renderizada se valida contra su campo útil visible; una columna estrecha en un campo ancho queda bloqueada.
- [ ] El `10` clásico usa `2-1-2-2-1-2`; `2-2-1-1-2-2` y las disposiciones amontonadas quedan bloqueadas, igual que toda topología incorrecta de los demás valores.
- [ ] Fixtures válidas e inválidas demuestran que el validador acepta y rechaza correctamente.
- [ ] Índices y símbolos tienen un único propietario y aparecen una sola vez.
- [ ] La lámina final fue ensamblada desde unidades aprobadas sin reinterpretarlas.
- [ ] Ningún retoque dejó rectángulos, bordes dobles, halos o fondos tapados.
- [ ] El registro universal devuelve `PASS` para la fase que se va a cerrar.
## Geometría universal inmutable para cartas

Cuando una familia usa cartas del As al `10`, la fuente de autoridad es
`04_PLANTILLAS/CONTRATO_GEOMETRIA_UNIVERSAL_CARTAS.json`. Sus centros, posiciones,
orientaciones y tamaños visibles se bloquean antes de dibujar la envolvente temática.
El marco no puede proporcionar traslación ni escala al contenido universal.

Una tipografía temática puede cambiar el estilo, pero su huella visible debe conservar el
tamaño y el centro aprobados. Si el contenido no cabe o un ornamento invade una zona
reservada, se rediseña el marco. Nunca se reduce ni se desplaza el contenido para rescatarlo.
Cada perfil de tema referencia el hash aprobado del maestro; cualquier cambio de geometría
invalida ese hash y exige nueva aprobación antes de validar otro marco.

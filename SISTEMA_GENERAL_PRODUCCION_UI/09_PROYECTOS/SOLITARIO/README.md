# Solitario

Las decisiones y evidencias propias de la colección de solitarios permanecen en el
repositorio del proyecto. El caso reutilizable sobre cartas numéricas, capas y zonas
reservadas se resume en:

`07_CASOS_HISTORICOS/SOLITARIO/GOLF_CONTEO_CAPAS_Y_ZONAS_RESERVADAS_2026-09-09.md`

El caso sobre centrado óptico y selección estructural del perfil de índices se resume en:

`07_CASOS_HISTORICOS/SOLITARIO/CANFIELD_CENTRADO_INDICES_2026-09-09.md`

El caso sobre ocupación interna de índices y topología de símbolos centrales se resume en:

`07_CASOS_HISTORICOS/SOLITARIO/FORTY_THIEVES_TOPOLOGIA_Y_OCUPACION_2026-09-10.md`

Para nuevas barajas se exige una tabla canónica de rangos, palos y conteos, composición
determinista de toda información exacta, propietario único de cada capa y lámina final
ensamblada desde cartas verificadas individualmente.

Antes de diseñar un marco nuevo se aplica además
`04_PLANTILLAS/CONTRATO_ORDEN_PRODUCCION_GRAFICA.json`: se mide primero el núcleo universal,
se bloquean el campo útil y los huecos de índices con los casos extremos, se aprueba una
prueba geométrica limpia y sólo después se dibuja la envolvente temática. Está prohibido
encoger o desplazar A–10, sus palos o su topología aprobada para rescatar un marco tardío.
El origen de esta regla se documenta en
`07_CASOS_HISTORICOS/SOLITARIO/SCORPION_ORDEN_GEOMETRIA_MARCO_2026-09-11.md`.

En toda baraja nueva también es obligatorio medir la ocupación visible del bloque
`rango + palo` contra el hueco real y validar la topología, separación y simetría de los
símbolos centrales. Esta prueba cubre todas las cartas del As al `10` y los cuatro palos
según `04_PLANTILLAS/PATRONES_CANONICOS_CARTAS_NUMERICAS.json`, no sólo los valores usados
como muestra. Para el `10` clásico se exige la firma `2-1-2-2-1-2`, formada por dos
mitades `2-1-2`; conteo y centrado sin esa distribución no autorizan el lote. El `7`
conserva la orientación inglesa `5/2` y no se recentra destruyendo su asimetría tradicional.

Los índices `rango + palo` obedecen además a
`04_PLANTILLAS/PERFIL_CANONICO_INDICES_COMPUESTOS.json`: la referencia móvil vigente exige
un mínimo de `4 px` de separación interna visible para A–10 y J/Q/K, idéntico arriba y abajo.
Un rango y un palo pegados o solapados bloquean la unidad aunque el conjunto esté centrado.
Para temas nuevos o modificados, el palo conserva un mínimo visible del `8.4 %` del ancho
final de la carta. El marco puede variar sus huecos sólo si el bloque universal y sus
márgenes caben sin reducción. Los huecos realmente pintados se miden y deben ser contrapartes
rotadas con un delta máximo `1 px`. El caso que originó esta defensa está en
`07_CASOS_HISTORICOS/SOLITARIO/SCORPION_ESCALA_PALOS_Y_SIMETRIA_BOLSILLOS_2026-09-11.md`.

La presencia de estas defensas en el sistema general se comprueba con:

`05_QA/validate_card_layout_rules.py`

Todo entregable nuevo usa además un registro de
`05_QA/PUERTA_OBLIGATORIA_ACEPTACION.md`. Para lotes de cartas, `U05`, `U06` y `U09` son
aplicables y no pueden marcarse `N/A`.

El contrato definitivo de posiciones y tamaños del A–10 es
`04_PLANTILLAS/CONTRATO_GEOMETRIA_UNIVERSAL_CARTAS.json`. Un tema sólo puede empezar su
marco cuando ese contrato está `LOCKED` con el hash aprobado por el usuario. A partir de
ese momento no se permiten ajustes particulares de centro, escala, tamaño de índices o
tamaño de símbolos numéricos. Si el arte no contiene las zonas reservadas o las invade,
se modifica el marco. La geometría universal no se mueve para hacerlo caber.

La defensa ejecutable es `Tools/validate_universal_frame_contract.py --self-test`. Debe
bloquear traslación, escala, reducción, invasiones, zonas demasiado pequeñas y cualquier
maestro pendiente o cambiado después de aprobarse.

## Nuevas modalidades y temas visuales

Toda modalidad nueva aplica antes del concepto
`09_PROYECTOS/SOLITARIO/CONTRATO_NUEVAS_MODALIDADES_VISUALES.json`.

El shell actual —cabecera, estado, barra inferior, botones, orden de acciones, navegación,
modales y Safe Area— se conserva como una capa común. Pedir que cada solitario tenga estilo
distinto sólo autoriza cambiar fondo, reverso, marco, ornamentos, figuras, paleta y decoración
de slots dentro de las capas declaradas; no autoriza crear otra interfaz.

Antes de guardar una propuesta se exige una captura vigente del shell, una matriz de
propietarios, una comparación antes/después y la verificación del maestro A–10 `LOCKED` con
hash `ea3603284333a5698b9db8ea293184ce530a58d98d7964bad569c224d12c7b77`.
Quedan bloqueados los placeholders numéricos, cartas aproximadas, cambios de botones y
cualquier ajuste de posición o escala por modalidad.

Usar el maestro correcto no autoriza poner su PNG opaco completo encima del fondo de una
carta temática. La integración obligatoria conserva un único propietario por superficie y
compone, de abajo hacia arriba: superficie temática, sólo el contenido A–10 derivado del
maestro con transparencia fuera de la tinta y marco temático transparente en el campo
protegido. El render debe conservar el fondo temático visible en muestras sin tinta y debe
reproducirse exactamente desde sus capas declaradas. Toda propuesta de este tipo declara el
módulo `visual_layer_composition` y pasa `VLC01` con evidencia automática desde `unit`.

El caso que originó esta defensa se documenta en
`07_CASOS_HISTORICOS/SOLITARIO/RUSSIAN_SOLITAIRE_ALCANCE_TEMA_Y_SHELL_2026-09-14.md`.

Al integrar el arte en Unity se validan también las dimensiones importadas, no sólo las del
PNG fuente. Para texturas no potencia de dos, `nPOTScale` debe quedar desactivado cuando el
contrato exige una relación exacta; una fuente `1080 x 1632` importada como `1024 x 2048`
deforma el fondo aunque el archivo original sea correcto. Las zonas de interacción que
cubren el tablero deben ser transparentes cuando el tema posee un fondo continuo: conservar
el raycast no autoriza pintar rectángulos opacos encima de la capa temática.

Los listados desplazables del shell con fondo transparente usan recorte geométrico
`RectMask2D`. No se debe combinar una `Image` con alfa cero y un `Mask` clásico: los botones
pueden conservar sus raycasts y navegar correctamente mientras el stencil oculta todos sus
gráficos. La prueba del selector debe comprobar los once botones, visibilidad de un elemento
inicial, visibilidad de Russian Solitaire tras desplazar, ausencia del `Mask` incompatible y
una captura runtime posterior al cambio.

La personalización modular mantiene un propietario efectivo por componente. Si un dorso o
tapete básico se equipa, su componente temático equivalente vuelve al identificador clásico;
si un tema premium posee el componente, el básico guardado como preferencia no puede aparecer
como `EQUIPADO`. La vista previa de un básico debe usar la familia clásica aunque actualmente
haya un componente premium activo. Se valida con un estado mixto básico/premium y se comparan
etiqueta, textura mostrada, estado resultante y render de partida.

Esta regla es transversal y no exclusiva de Klondike. Debe comprobarse en Klondike, Spider,
FreeCell, Pyramid, TriPeaks, Golf, Yukon, Canfield, Forty Thieves, Scorpion y Russian
Solitaire. En FreeCell, el cosmético básico de frente de carta libera `EquippedThemeId`;
en las demás modalidades, el dorso básico libera `EquippedCardBackThemeId`. La superficie
básica correspondiente libera `EquippedFeltThemeId`. Los efectos sueltos permanecen
independientes porque no tienen un propietario premium paralelo.
La comprobación estructural automatizada del proyecto debe devolver `PASS (11/11)`.

## Matriz de completitud global

La lista autoritativa de modalidades procede del catálogo o enum que usa el runtime, no del
conteo recordado en una conversación. Para cada modalidad, la matriz
`09_PROYECTOS/SOLITARIO/MATRIZ_COMPLETITUD_GLOBAL.json` separa como mínimo: jugabilidad,
tema clásico, dos temas gratuitos, objetivo premium, tienda, equipamiento, persistencia,
pruebas focalizadas y puerta final.

`Jugable`, `implementado` y `terminado` no son sinónimos. Una comprobación de once juegos
jugables no autoriza declarar once modalidades completas si el contenido gratuito sólo
cubre nueve. El cierre global exige exactamente las once filas esperadas, ninguna
duplicada, todos los campos obligatorios en `PASS` y el validador de matriz en
`PASS (11/11)`. Los conteos actuales pertenecen a la matriz fechada y no se convierten en
una regla permanente.

Después de una pausa se ejecuta un protocolo de reanudación antes de modificar o cerrar:

1. comprobar procesos `Unity.exe`, pruebas antiguas de Hub y `Temp/UnityLockfile`;
2. comparar fecha y hash de fuentes, assets, scripts y documentos afectados;
3. repetir preflight y validadores rápidos;
4. reutilizar una prueba costosa sólo cuando todas sus entradas conservan el mismo hash;
5. actualizar matriz y evidencia con la fecha posterior al último cambio.

La defensa documental es `05_QA/validate_unity_batch_and_completion_rules.py`. La defensa
del entorno batch es `05_QA/Test-UnityBatchPreflight.ps1`.

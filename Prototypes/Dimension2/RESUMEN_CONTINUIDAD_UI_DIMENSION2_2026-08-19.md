# RESUMEN DE CONTINUIDAD — INTERFAZ VISUAL DE DIMENSIÓN 2

Fecha: 19 de agosto de 2026  
Proyecto: Quantum Forge  
Alcance de este traspaso: diseño visual, normalización y futura implementación de las pantallas de Dimensión 2.

## 1. Instrucción para iniciar el siguiente chat

> Lee completamente este resumen, `AGENTS.md`, `REGLAS_DE_TRABAJO_Y_CONTINUIDAD.md`, las instrucciones maestras y las dos guías centrales de UI. Después inspecciona los archivos reales. No regeneres ni implementes todavía las pantallas: el primer trabajo pendiente es corregir la consistencia visual de las 20 referencias existentes. No uses Git. No modifiques `Main.unity` ni archivos de Dimensión 1 o 3 mientras otro proceso los esté tocando. Antes de usar Unity, confirma que el editor normal esté cerrado y que el otro chat no vaya a usarlo simultáneamente.

## 2. Objetivo

Dar identidad visual propia a Dimensión 2 y posteriormente implementar sus pantallas en Unity con datos y comportamiento fieles al código. El usuario quiere conservar una identidad común de piedra oscura, bronce envejecido y tipografía serif, con acentos propios por civilización:

- Civilización 1: santuario, peregrinación y oro cálido.
- Civilización 2: resistencia, amenaza, rojo y turquesa defensivo.
- Civilización 3: arqueología, turquesa y anomalía violeta.

La meta de implementación será una captura real de Unity con similitud visual mínima aprobada del 95 %, primero en composición estática y después en datos, interacción y animación.

## 3. Orden de autoridad

1. Petición actual y explícita del usuario.
2. Diseño canónico y referencias que el usuario apruebe expresamente.
3. `AGENTS.md` y `REGLAS_DE_TRABAJO_Y_CONTINUIDAD.md`.
4. Instrucciones maestras y guías centrales de UI.
5. Perfil de estilo, fichas y decisiones registradas.
6. Código y archivos reales para determinar el estado técnico.
7. Este resumen como continuidad, sin sustituir la inspección del proyecto.

Si diseño, código o resumen se contradicen, informar y detener únicamente la parte afectada.

## 4. Estado de las imágenes

Existen 20 PNG numerados en:

`Prototypes/Dimension2/CodeFaithfulCorrected`

Todos miden 941 × 1672 y conservan una proporción equivalente a 9:16.

IMPORTANTE:

- Son CONCEPTOS / REFERENCIAS CANDIDATAS.
- No son capturas reales de Unity.
- No son todavía referencias canónicas aprobadas.
- No deben utilizarse como una textura completa dentro de Unity.
- Deben descomponerse en fondos, marcos, iconos, textos y capas animables.
- No se regeneró ni modificó ninguna imagen durante la última auditoría.

Lista:

1. `01_Mapa_De_Los_Pactos_Corregido.png` — pantalla principal global.
2. `02_El_Santuario_Refugio_Corregido.png` — principal de Civilización 1.
3. `03_Altares_Del_Santuario_Corregido.png` — subpantalla C1.
4. `04_Peregrinaciones_Corregidas.png` — subpantalla C1.
5. `05_Noviciado_Corregido.png` — subpantalla C1.
6. `06_Ritos_Del_Santuario_Corregidos.png` — subpantalla C1.
7. `07_Pactos_De_Civilizacion_Corregidos.png` — subpantalla C1.
8. `08_Umbral_Lugar_De_Vinculo_Corregido.png` — subpantalla/estado avanzado C1.
9. `09_Red_De_Resistencia_Regiones_Corregida.png` — principal C2.
10. `10_Operaciones_De_Resistencia_Corregidas.png` — subpantalla C2.
11. `11_Defensa_Y_Represalias_Corregida.png` — subpantalla C2.
12. `12_Resistencia_Y_Pactos_Corregida.png` — subpantalla C2.
13. `13_Alerta_Del_Ente_Corregida.png` — subpantalla C2.
14. `14_Contencion_Intento_Corregida.png` — subpantalla/estado de intento C2.
15. `15_Pacto_Mayor_Civilizacion2_Corregido.png` — estado posterior a Contención y cierre principal de D2.
16. `16_Ruinas_Sepultadas_Arqueologia_Corregida.png` — principal C3.
17. `17_Analizar_Restos_Corregido.png` — subpantalla C3.
18. `18_Archivo_Mejoras_Permanentes_Corregido.png` — subpantalla C3.
19. `19_Investigacion_Del_Ente_Corregida.png` — subpantalla C3.
20. `20_Pacto_Opcional_Con_El_Ente_Corregido.png` — estado posterior opcional C3.

## 5. Resultado de la auditoría de consistencia

La familia visual general es coherente, pero el conjunto todavía no puede aprobarse como diseño final.

Elementos que sí son constantes:

- Relación vertical 9:16.
- Piedra carbón, bronce envejecido, marcos tallados y serif grabada.
- Estructura de encabezado, recursos, contenido principal, panel contextual y navegación.
- Variantes cromáticas adecuadas para diferenciar las tres civilizaciones.

Correcciones pendientes obligatorias:

1. Crear un único icono canónico por pestaña y reutilizarlo en todas sus apariciones.
2. Civilización 1 cambia repetidamente los iconos de Peregrinaciones, Ritos, Pactos y Umbral. Las pantallas 7 y 8 llegan a usar el nudo para dos destinos diferentes.
3. Civilización 2 cambia los iconos de Regiones, Resistencia, Alerta y Contención entre pantallas.
4. Añadir la flecha de regreso que falta en 11 Defensa y 13 Alerta, salvo que el contrato funcional aprobado determine otra navegación.
5. Civilización 3 mezcla `ENTE` y `ENTIDAD`. El código usa `ENTE` antes del pacto y `PACTO OPCIONAL` después de establecerlo.
6. Mantener las transformaciones dinámicas del código: `UMBRAL` puede convertirse en `PACTO`; `CONTENCIÓN` en `PACTO MAYOR`; `ENTE` en `PACTO OPCIONAL`.
7. Definir un contrato único de encabezado, flecha, navegación inferior, tamaños, márgenes y brillo de selección.
8. Decidir si las referencias mostrarán un estado QA continuo por civilización o escenarios independientes. Ahora hay valores que no forman una secuencia única: Confianza 280/300, 280/500 y 80/300; Archivo III en Ruinas pero Archivo II en la pantalla de Archivo; distintos valores de Conocimiento Antiguo.
9. En el Mapa, no rotular `CONFIANZA 280/300` como si 300 fuese el máximo si el sistema usa 500. Mostrar el valor total según el código y el requisito de 300 en el territorio correspondiente.

No corregir estas imágenes eligiendo iconos por gusto. Primero comprobar código, referencias existentes y decisión del usuario; después fijar el catálogo canónico.

## 6. Relación con el código actual

La lógica funcional de Dimensión 2 ya existe. Deben inspeccionarse directamente, entre otros:

- `Assets/Project/Scripts/UI/Dimension2PanelUI.cs`
- `Assets/Project/Scripts/UI/D2Civilization1PanelUI.cs`
- `Assets/Project/Scripts/UI/D2AltarsPanelUI.cs`
- `Assets/Project/Scripts/UI/D2PilgrimagesPanelUI.cs`
- `Assets/Project/Scripts/UI/D2NovitiatePanelUI.cs`
- `Assets/Project/Scripts/UI/D2RitesPanelUI.cs`
- `Assets/Project/Scripts/UI/D2CivilizationPactsPanelUI.cs`
- `Assets/Project/Scripts/UI/D2VeiledThresholdPanelUI.cs`
- `Assets/Project/Scripts/UI/D2Civilization2PanelUI.cs`
- `Assets/Project/Scripts/UI/D2OperationsPanelUI.cs`
- `Assets/Project/Scripts/UI/D2ReprisalsPanelUI.cs`
- `Assets/Project/Scripts/UI/D2ResistancePanelUI.cs`
- `Assets/Project/Scripts/UI/D2AlertPanelUI.cs`
- `Assets/Project/Scripts/UI/D2ContainmentPanelUI.cs`
- `Assets/Project/Scripts/UI/D2Civilization3PanelUI.cs`
- `Assets/Project/Scripts/UI/D2EntityResearchPanelUI.cs`
- Sistemas D2 correspondientes dentro de `Assets/Project/Scripts/Systems`.

Nombres de navegación verificados en código:

- C1: REFUGIO, ALTARES, PEREGRINACIONES, NOVICIADO, RITOS, PACTOS y UMBRAL. Tras el pacto mayor, UMBRAL puede mostrarse como PACTO.
- C2: REGIONES, OPERACIONES, DEFENSA, RESISTENCIA, ALERTA y CONTENCIÓN. Tras contener al Ente, CONTENCIÓN se convierte en PACTO MAYOR.
- C3: EXCAVAR/arqueología, análisis, ARCHIVO y ENTE. Tras establecer el pacto del Ente, el botón se convierte en PACTO OPCIONAL.

El Pacto Mayor de Civilización 2 es el cierre principal reconocido de Dimensión 2. El Pacto con el Ente de Civilización 3 es contenido avanzado opcional.

## 7. Estado funcional de los Bloques 1–5

El resumen funcional anterior es `RESUMEN_CONTINUIDAD_DIMENSION2_HASTA_BLOQUE2D.md`, pero está fechado el 22 de julio de 2026 y debe contrastarse con el código actual. No usarlo como sustituto del proyecto.

- Bloque 1 — base común, guardado, introducción y mapa: implementado funcionalmente.
- Bloque 2 — Civilización 1, Refugio, Altares, Peregrinaciones, Noviciado, Ritos, Pactos y Lugar de Vínculo: implementado funcionalmente; balance largo pendiente.
- Bloque 3 — Civilización 2, Regiones, Operaciones, Defensa/Represalias, Resistencia, Alerta, Contención y Pacto Mayor: implementado funcionalmente; balance largo pendiente.
- Bloque 4 — Civilización 3, excavación, análisis, Archivo, anomalías, Investigación y pacto opcional: implementado funcionalmente; revisar directamente el estado actual porque el resumen antiguo menciona tres líneas, mientras el código auditado actualmente contiene cinco líneas.
- Bloque 5 — integración y cierre: existen implementaciones y validaciones previas; la interfaz visual nueva todavía no está integrada y la prueba manual integral indicada en el resumen funcional debe volver a comprobarse antes de declarar cierre actual.

No rehacer la lógica funcional durante el trabajo visual salvo que una contradicción verificada lo exija y el usuario la apruebe.

## 8. Estado real de Unity y assets de D2

Existe un prototipo separado de Pactos:

- `Assets/Project/Scenes/Dimension2PactPrototype.unity`
- `Assets/Project/Scripts/Editor/Dimension2PactPrototypeSetup.cs`
- `Assets/Project/Scripts/UI/Dimension2PactPrototypeUI.cs`
- Assets en `Assets/Project/UI/Dimension2/Pacts/Generated`

Este prototipo es la evidencia de viabilidad visual más avanzada, pero todavía no equivale a que las 20 pantallas estén implementadas o aprobadas.

El perfil oficial de D2 sigue pendiente:

- `SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/03_ESTILOS/DIMENSION_2/PERFIL_ESTILO_DIMENSION_2.txt`

No existen todavía referencias D2 consolidadas en `SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS`, fichas D2 en `10_PANTALLAS`, ni entradas D2 completas en el catálogo y registro de pantallas.

## 9. Trabajo paralelo y protección del proyecto

En la última comprobación no había un proceso normal de Unity abierto, pero otro chat estaba trabajando en el mismo proyecto. El árbol de trabajo tenía numerosos cambios en `Main.unity`, Dimensión 1, Dimensión 3, localización, GameState y otros archivos. Esos cambios pertenecen al usuario u otros procesos y deben preservarse.

Para no interferir:

- No tocar `Assets/Project/Scenes/Main.unity` durante la fase visual aislada.
- No modificar archivos D1/D3 ni materiales compartidos que el otro chat esté usando.
- Crear la primera pantalla D2 en una escena prototipo nueva y aislada cuando llegue la etapa Unity.
- Antes de modificar escenas, prefabs, assets serializados o ejecutar batch, confirmar que Unity esté cerrado y que el otro chat no vaya a abrirlo.
- Nunca ejecutar dos instancias de Unity sobre el mismo proyecto.
- No usar Git salvo petición explícita.

## 10. Máximo trabajo posible antes de abrir Unity

Se puede completar:

- Corrección y normalización de las 20 referencias.
- Perfil visual oficial D2.
- Catálogo canónico de iconos, marcos y tokens.
- Fichas y contratos de pantalla.
- Mediciones a 1080 × 1920 y verificación conceptual a 720 × 1280.
- Inventario y separación de fondos, marcos, iconos, brillos y capas animables.
- Preparación de PNG finales con alfa y proporción correcta.
- Estados neutral, seleccionado, bloqueado, vacío y completado.
- Scripts aislados, constructor de editor y validaciones preparadas sin ejecutar.

Sin Unity no se puede afirmar implementación, importación correcta, escena/prefab guardado, interacción, animación, captura real, rendimiento ni similitud mínima del 95 %.

## 11. Siguiente bloque recomendado

1. No abrir Unity todavía.
2. Crear una tabla canónica de navegación para C1, C2 y C3: etiqueta, icono, estado seleccionado y transformación posterior.
3. Fijar contrato global de encabezado, flecha, franja de recursos, paneles, CTA y navegación.
4. Corregir las 20 referencias sin cambiar mecánicas ni información real.
5. Presentarlas al usuario para aprobación explícita.
6. Archivar las aprobadas en `05_REFERENCIAS/DIMENSION_2` y completar perfil, fichas, decisiones, catálogo y registro.
7. Preparar la primera pantalla, Mapa de los Pactos, como prototipo aislado.
8. Reservar una sesión exclusiva de Unity para composición estática, captura 1080 × 1920 y 720 × 1280 y comparación ≥95 %.
9. Conectar datos e interacción después de aprobar la composición.
10. Añadir animación únicamente al final.

## 12. Cambios realizados durante este trabajo visual

- Se generaron y guardaron las 20 referencias candidatas listadas.
- Se creó previamente el prototipo aislado de Pactos y sus assets.
- Se auditó contenido frente al código y luego consistencia visual entre las 20 imágenes.
- Durante la auditoría final de consistencia no se modificaron imágenes, escenas, scripts ni assets.
- No se usó Git.
- No se alteró ninguna partida mediante DEBUG en este trabajo visual.

## 13. Reglas completas de trabajo y continuidad

# REGLAS DE TRABAJO Y CONTINUIDAD ENTRE CHATS — QUANTUM FORGE

## 1. Prioridad general

- El objetivo principal es completar Quantum Forge con la mayor rapidez posible sin sacrificar calidad, estabilidad, compatibilidad ni claridad.
- Se elegirá siempre el método más eficiente que permita verificar correctamente el resultado.
- Una instrucción actual y explícita del usuario tiene prioridad sobre estas reglas.
- El diseño aprobado por el usuario determina el contenido creativo y mecánico. El asistente no puede completar vacíos de diseño por iniciativa propia.
- El código y los archivos reales determinan el estado técnico actual del proyecto. El resumen determina la intención y continuidad. Si se contradicen, se debe informar antes de realizar cambios importantes.

## 2. Al comenzar un chat nuevo

- Leer completamente el resumen de continuidad y las reglas incluidas en él.
- Revisar directamente los archivos actuales del proyecto antes de proponer o realizar cambios.
- No asumir que el resumen reemplaza al código real.
- Continuar desde el primer bloque pendiente; no repetir bloques ya terminados y probados.
- Si existe una contradicción entre código, resumen o diseño, explicarla y resolverla antes de implementar la parte afectada.

## 3. Revisión previa a un bloque importante

Antes de iniciar un bloque funcional importante, indicar de forma breve:

- Qué archivos o sistemas se modificarán.
- Por qué necesita modificarse cada uno.
- Qué comportamiento se espera conseguir.
- Si serán necesarias conexiones, automatización o pruebas posteriores en Unity.

Una vez que el usuario haya pedido implementar el bloque, el asistente puede avanzar por todas sus subdivisiones internas sin pedir autorización repetidamente. Solo debe detenerse si el usuario pidió únicamente una revisión, aparece una contradicción importante, falta una decisión de diseño o la acción tiene un riesgo material.

## 4. Tamaño y forma de trabajo

- Trabajar en el bloque funcional completo más grande que pueda implementarse y verificarse de forma segura.
- Dividir internamente cada bloque en partes coherentes: estado y guardado, lógica, interfaz, conexiones, validaciones y pruebas.
- No detenerse ni pedir confirmación después de cada cambio pequeño.
- No modificar simultáneamente varios bloques grandes e independientes.
- No comenzar el siguiente bloque mientras el actual conserve conexiones o pruebas técnicas automatizables indispensables pendientes. La partida completa, el balance y el pulido jugable no bloquean el avance estructural.
- Trabajar principalmente en C# y usar los recursos de Unity necesarios cuando el bloque los requiera.
- Mantener compatibilidad con partidas guardadas antiguas.
- Preservar los cambios existentes del usuario y evitar refactorizaciones generales sin una necesidad concreta.
- Priorizar primero funcionalidad correcta; dejar el pulido visual fino y el balance definitivo para después de disponer de una versión jugable.
- Si aparece un error de compilación o consola relacionado con el proyecto, detener el avance, corregirlo y volver a validar antes de continuar.

## 5. Unity, jerarquía e Inspector

- El asistente está autorizado a crear y modificar los elementos de Unity necesarios para el bloque solicitado: escenas, jerarquías, paneles, botones, textos, desplegables, componentes, referencias del Inspector y eventos.
- El usuario no tiene que realizar manualmente conexiones, arrastrar referencias ni modificar jerarquías, salvo que expresamente prefiera hacerlo.
- Las modificaciones de escenas, prefabs, assets y archivos meta deben limitarse al alcance del bloque y realizarse con especial cuidado.
- Si Unity está abierto y puede sobrescribir cambios externos, avisar al usuario si debe cerrar el editor antes de modificar la escena.
- Unity puede permanecer abierto para documentación o cambios pequeños y aislados en scripts `.cs`; debe estar fuera de Play Mode y se esperará a que termine la recompilación.
- Antes de crear o modificar varios scripts interdependientes, se pedirá cerrar Unity hasta que el conjunto de archivos esté completo y compile externamente. Esto evita que Auto Refresh intente compilar estados parciales y muestre errores transitorios.
- También se pedirá cerrar Unity cuando se vayan a modificar externamente escenas, prefabs, assets, archivos `.meta` o conexiones serializadas que el editor pueda sobrescribir.
- No ejecutar una segunda instancia de Unity en modo batch sobre el proyecto mientras el Editor normal esté abierto. Las validaciones de Unity se ejecutarán desde el Editor normal o, si se necesita batch, con todas las instancias normales cerradas.
- No detenerse en Unity después de cada ajuste pequeño.
- Realizar las conexiones cuando el bloque alcance un punto funcional comprobable; no acumular todas las conexiones hasta el final del proyecto.
- El usuario podrá revisar la apariencia, distribución y claridad de la UI cuando sea útil, pero no tendrá que completar partidas ni ciclos largos para validar cada bloque.
- El asistente preparará validadores, pruebas de lógica y accesos de desarrollo seguros para comprobar cada sistema sin exigir recorrer el juego completo.
- No afirmar que una función está completamente jugable si solo funciona mediante DEBUG o todavía carece de una interfaz indispensable.

## 6. Compilación, validaciones y pruebas

- Ejecutar comprobaciones ligeras de compilación durante hitos internos importantes cuando sean útiles.
- Durante bloques de varios scripts, completar primero el conjunto interdependiente, validarlo mediante compilación externa y abrir Unity después; los mensajes generados por una compilación parcial no se tomarán como resultado final.
- Realizar una compilación y validación integral al finalizar el código de cada bloque funcional.
- Agrupar las pruebas de Unity por hitos funcionales, no por cambios diminutos.
- Solo pedir al usuario comprobaciones visuales o interacciones breves en Play Mode cuando no puedan verificarse razonablemente de forma automática; no exigir una partida completa durante la construcción por bloques.
- Separar claramente:
  - lógica terminada en código;
  - conexiones realizadas;
  - conexiones pendientes;
  - pruebas automáticas ejecutadas;
  - pruebas pendientes en Unity;
  - problemas conocidos o decisiones provisionales.
- Probar guardado y carga cuando el bloque agregue o modifique estado persistente.
- Probar una partida nueva y una antigua cuando existan migraciones o compatibilidad de guardado involucradas.
- No usar una partida alterada mediante DEBUG como evidencia de balance o migración normal.
- Una partida o estado preparado mediante herramientas de desarrollo sí puede usarse para pruebas técnicas aisladas, pero no como evidencia de balance, progresión natural o migración normal.
- La prueba jugable integral, el ritmo de progresión y el balance definitivo se realizarán después de implementar la estructura funcional del juego. Los fallos técnicos detectables automáticamente se corregirán dentro de su bloque y no se aplazarán.

## 7. Git y protección del trabajo

- No crear commits, ramas, staging, push, reset ni otras operaciones de Git salvo petición explícita del usuario.
- Se permiten consultas de solo lectura cuando sean necesarias para comprobar el estado solicitado por el usuario.
- Recomendar puntos de guardado en Git antes de cambios delicados y después de validar un bloque, pero dejar la decisión y ejecución al usuario salvo autorización expresa.
- No eliminar, reemplazar ni descartar cambios existentes del usuario.
- No modificar archivos ajenos al alcance del bloque.
- No realizar builds pesados cuando una comprobación ligera sea suficiente.

## 8. Autonomía y momentos para detenerse

El asistente debe avanzar de forma autónoma dentro del bloque solicitado y detenerse únicamente cuando:

- Falte una decisión de diseño que cambie materialmente el resultado.
- Exista una contradicción importante entre reglas, resumen y código.
- Aparezca un error que deba resolverse antes de continuar.
- Sea necesaria una prueba visual o jugable del usuario.
- La acción sea destructiva, externa al alcance o pueda afectar trabajo no relacionado.
- El bloque esté listo para cerrarse y deba decidirse el siguiente paso.

## 9. Cierre de cada bloque

Antes de declarar un bloque completamente terminado, confirmar:

- Código implementado.
- Compilación sin errores relacionados.
- Guardado y carga comprobados cuando corresponda.
- Compatibilidad con partidas antiguas preservada y probada cuando corresponda.
- Herramientas DEBUG y validaciones relevantes disponibles.
- Conexiones visuales indispensables completadas.
- Prueba funcional en Unity realizada cuando corresponda.
- Errores encontrados corregidos o registrados claramente.
- Decisiones provisionales y asuntos de balance documentados.
- Resultado resumido al usuario.

Si falta una conexión o una prueba indispensable, el bloque debe describirse como "código terminado, validación pendiente", no como completamente terminado.

## 10. Chats largos y cambio de conversación

- Avisar al usuario antes de que el chat sea demasiado extenso o pierda claridad.
- Recomendar un chat nuevo al terminar el bloque funcional actual o alcanzar un punto seguro.
- No iniciar un bloque grande nuevo si conviene preparar primero la continuidad.
- Antes de cambiar, entregar un resumen completo, autosuficiente y listo para pegar.
- El nuevo chat no debe necesitar consultar conversaciones anteriores.

## 11. Contenido obligatorio de todo resumen de continuidad

Todo resumen solicitado por longitud del chat, cambio de modelo, cambio de conversación o cualquier otra razón debe incluir:

- Objetivo general del proyecto y del trabajo actual.
- Estas reglas de trabajo completas y actualizadas, reproducidas dentro del propio resumen. No basta con enlazarlas, mencionar su nombre ni decir "usar las reglas anteriores".
- Orden de autoridad entre instrucciones actuales, reglas, resumen y archivos reales.
- Todos los bloques principales del alcance acordado, incluidos los terminados y los futuros.
- Estado exacto de cada bloque: terminado, código terminado con pruebas pendientes, en progreso o pendiente.
- Funciones implementadas.
- Archivos modificados.
- Conexiones realizadas en Unity.
- Conexiones todavía pendientes.
- Pruebas ejecutadas y resultados.
- Errores encontrados y correcciones aplicadas.
- Advertencias, inferencias o contradicciones de diseño pendientes.
- Estado especial de cualquier partida alterada mediante DEBUG.
- Siguiente bloque recomendado.
- Subdivisión interna propuesta del siguiente bloque.
- Instrucción explícita de revisar los archivos reales antes de modificar.
- Indicación de no usar Git salvo petición explícita.

## 12. Autosuficiencia y transferencia obligatoria de las reglas

- Estas reglas deben viajar completas en cada resumen de continuidad entre chats.
- Esta obligación se mantiene aunque el usuario solicite un resumen corto; en ese caso se puede condensar el estado del proyecto, pero no omitir las reglas esenciales ni su reproducción.
- El resumen debe contener también los bloques futuros y explicar qué falta para terminar todo el alcance acordado.
- No debe depender de frases como "según lo hablado antes", "como aparece en el otro chat" o "consulta el archivo anterior".
- Debe incluir las decisiones importantes necesarias para impedir que el siguiente chat invente comportamientos.
- Toda contradicción todavía sin resolver debe registrarse explícitamente para decidirla antes de implementar la parte afectada.

## 13. Uso de modelos y herramientas

- Avisar si el siguiente trabajo requiere razonamiento especialmente amplio, migraciones delicadas o cambios que afecten muchos sistemas y podría beneficiarse de un modelo con mayor capacidad.
- No detener tareas normales o pequeñas únicamente por este motivo.
- La decisión de cambiar de modelo corresponde al usuario.
- Usar automatización y herramientas disponibles cuando reduzcan trabajo manual sin disminuir la seguridad o calidad.

## 14. Inicio recomendado en el siguiente chat

El siguiente chat debe comenzar con una instrucción equivalente a:

> Lee completamente las reglas y el resumen. Después inspecciona directamente los archivos actuales del proyecto. Antes de iniciar un bloque importante, indica brevemente qué archivos o sistemas modificarás, por qué, qué comportamiento se espera y qué conexiones o pruebas de Unity serán necesarias. Avanza de forma autónoma por las subdivisiones internas del bloque, sin usar Git salvo petición explícita y sin repetir bloques ya terminados y probados.

## 15. Fidelidad obligatoria al diseño y autoridad creativa del usuario

- El juego y su dirección creativa pertenecen al usuario. El asistente implementa el diseño aprobado; no lo sustituye por preferencias, inferencias o decisiones propias.
- Antes de implementar una mecánica, revisar el documento de diseño original y el diseño canónico vigente aplicable a esa parte.
- No inventar ni fijar sin permiso explícito recursos, costes, fórmulas, tasas de producción, cantidades iniciales, niveles, límites, requisitos, recompensas, penalizaciones, desbloqueos, simultaneidad, relaciones entre sistemas, textos narrativos determinantes ni comportamientos que no estén definidos en el diseño aprobado.
- Una inferencia razonable no se convierte automáticamente en diseño canónico. Si el documento permite más de una interpretación, se debe señalar la ambigüedad y esperar la decisión del usuario antes de implementar la parte afectada.
- Cuando falte una decisión necesaria para programar o probar, detener únicamente la parte afectada y presentar al usuario: qué falta, por qué es necesario, qué opciones existen y qué impacto tendría cada una. No aplicar ninguna opción hasta recibir permiso explícito.
- Los valores o comportamientos temporales para pruebas también requieren autorización previa. Deben identificarse como provisionales, mantenerse centralizados y ser fáciles de retirar o cambiar.
- No presentar como parte del diseño original una decisión técnica o provisional creada por el asistente.
- Las decisiones nuevas aprobadas por el usuario deben registrarse en el diseño canónico antes o al mismo tiempo que se implementan.
- Si se descubre que ya se implementó algo no respaldado por el diseño, informar de inmediato, detener el avance dependiente y pedir al usuario decidir si se conserva, modifica o elimina. No corregirlo silenciosamente.
- Todo resumen de continuidad entre chats debe reproducir esta regla completa y señalar cualquier elemento implementado que siga pendiente de aprobación de diseño.

## 16. Alcance total obligatorio de los resúmenes

- Un resumen de continuidad no puede limitarse al bloque, subdivisión o civilización que se esté implementando en ese momento.
- Debe transferir el contexto completo de todo el alcance acordado hasta su cierre final: bloques terminados, bloque activo, subdivisiones siguientes, civilizaciones futuras, integración final, decisiones canónicas, contradicciones y pendientes.
- Para Dimensión 2, cada resumen debe incluir como mínimo los Bloques 1, 2, 3, 4 y 5 completos, aunque el trabajo actual todavía esté dentro del Bloque 2.
- La finalidad es avanzar de chat en chat sin volver a conversaciones anteriores. El nuevo chat debe poder continuar y también comprender todo el camino futuro usando únicamente el resumen y los archivos reales del proyecto.
- No basta con escribir “el resto está en el documento anterior” ni enlazar otro chat. Si el detalle completo vive además en un archivo canónico del proyecto, el resumen debe reproducir por lo menos todas sus decisiones, estructuras, dependencias y pendientes necesarios para no inventar ni perder contexto.

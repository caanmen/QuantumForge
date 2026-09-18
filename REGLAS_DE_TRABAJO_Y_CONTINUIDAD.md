# REGLAS DE TRABAJO Y CONTINUIDAD ENTRE CHATS — QUANTUM FORGE

## 1. Prioridad general

- El objetivo principal es completar Quantum Forge con la mayor rapidez posible sin sacrificar calidad, estabilidad, compatibilidad ni claridad.
- Se elegirá siempre el método más eficiente que permita verificar correctamente el resultado.
- Una instrucción actual y explícita del usuario tiene prioridad sobre estas reglas.
- El diseño aprobado por el usuario determina el contenido creativo y mecánico. El asistente no puede completar vacíos de diseño por iniciativa propia.
- El código y los archivos reales determinan el estado técnico actual del proyecto. El resumen determina la intención y continuidad. Si se contradicen, se debe informar antes de realizar cambios importantes.

## 2. Al comenzar un chat nuevo

- Leer este archivo completo cuando se inicia un chat nuevo de Quantum Forge o un bloque amplio y transversal. Para una corrección pequeña y localizada, consultar sólo las secciones aplicables y los archivos directamente relacionados.
- Cuando el usuario diga «revisa la carpeta de producción», aplicar `SISTEMA_GENERAL_PRODUCCION_UI/00_INICIO/ENRUTADOR_DE_PROYECTOS_Y_TAREAS.md`: conocer el inventario, buscar por la tarea y leer sólo lo aplicable; nunca recorrer todo el árbol por rutina.
- Leer el estado de continuidad vigente, si existe, sin exigir que duplique estas reglas estables.
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
- Se permiten varias instancias normales de Unity cuando cada una apunta a una carpeta de
  proyecto diferente. La lista de Unity Hub no demuestra por sí sola que un editor esté
  abierto.
- Nunca abrir dos instancias normales o batch sobre la misma ruta de proyecto. Antes de
  automatizar, resolver la ruta absoluta del proyecto objetivo y comprobar esa ruta, no
  limitarse a contar procesos llamados `Unity`.
- No ejecutar Unity batch sobre `Quantum Forge` mientras esa misma carpeta esté abierta en
  el Editor normal. Si otros proyectos distintos están abiertos, pueden permanecer así;
  sólo se solicitará cerrar la instancia del proyecto objetivo cuando la operación vaya a
  escribir escenas, prefabs, assets serializados, metadatos o conexiones.
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
- Antes de cambiar, entregar un resumen compacto, verificable y listo para pegar, centrado en el estado que sí cambia.
- El nuevo chat no debe necesitar consultar conversaciones anteriores, pero debe apoyarse en los archivos canónicos versionados del proyecto para las reglas estables y las decisiones extensas.
- Usar `SISTEMA_GENERAL_PRODUCCION_UI/00_INICIO/COMO_CONTINUAR_EN_OTRO_CHAT.md`; el usuario no necesita conocer ni enumerar las guías internas.

## 11. Contenido obligatorio de todo resumen de continuidad

Todo resumen solicitado por longitud del chat, cambio de modelo, cambio de conversación o cualquier otra razón debe incluir:

- Objetivo general del proyecto y del trabajo actual.
- Rutas exactas de `AGENTS.md`, de este archivo y de las fuentes canónicas aplicables; no reproducir reglas estables que ya viven allí.
- Cambios recientes de autoridad o reglas, sólo si ocurrieron durante el chat.
- Estado de los bloques directamente relacionados con el alcance activo y referencia al archivo canónico donde vive el alcance total.
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

## 12. Autosuficiencia y transferencia de estado

- Las reglas estables permanecen en `AGENTS.md` y en este archivo; los resúmenes las enlazan mediante rutas exactas y transfieren únicamente estado, cambios y pendientes.
- El resumen puede ser corto si conserva el punto exacto de continuación, las pruebas, los archivos afectados, las decisiones nuevas y los riesgos abiertos.
- No debe depender de frases vagas como "según lo hablado antes" o "como aparece en el otro chat"; cada referencia debe nombrar un archivo canónico concreto.
- Las decisiones importantes deben registrarse en el diseño o registro canónico correspondiente y el resumen debe señalar su ubicación.
- Toda contradicción todavía sin resolver debe registrarse explícitamente para decidirla antes de implementar la parte afectada.

## 13. Uso de modelos y herramientas

- Avisar si el siguiente trabajo requiere razonamiento especialmente amplio, migraciones delicadas o cambios que afecten muchos sistemas y podría beneficiarse de un modelo con mayor capacidad.
- No detener tareas normales o pequeñas únicamente por este motivo.
- La decisión de cambiar de modelo corresponde al usuario.
- Usar automatización y herramientas disponibles cuando reduzcan trabajo manual sin disminuir la seguridad o calidad.

## 14. Inicio recomendado en el siguiente chat

El siguiente chat debe comenzar con una instrucción equivalente a:

> Lee completamente las reglas y el resumen. Después inspecciona directamente los archivos actuales del proyecto. Antes de iniciar un bloque importante, indica brevemente qué archivos o sistemas modificarás, por qué, qué comportamiento se espera y qué conexiones o pruebas de Unity serán necesarias. Avanza de forma autónoma por las subdivisiones internas del bloque, sin usar Git salvo petición explícita y sin repetir bloques ya terminados y probados.

Para una corrección pequeña, puede sustituirse "lee completamente" por "consulta las secciones aplicables" y conservar el resto de la instrucción.

Si se pide revisar la carpeta de producción, añadir: «aplica su enrutador; no leas todo el árbol. Selecciona tú las guías, aprendizajes y pruebas relacionados e indica brevemente cuáles usarás».

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
- Todo resumen de continuidad debe enlazar esta sección y señalar cualquier elemento implementado que siga pendiente de aprobación de diseño.

## 16. Alcance de los resúmenes

- El resumen debe cubrir el bloque activo, sus dependencias inmediatas y el siguiente paso verificable.
- El alcance completo, los bloques futuros y las decisiones extensas deben vivir en archivos canónicos del proyecto; el resumen señala sus rutas y registra solamente los cambios desde la última actualización.
- Para Dimensión 2, el resumen debe indicar el estado de los Bloques 1 a 5 en una línea por bloque y enlazar la planificación o diseño canónico correspondiente, sin reproducirlo por completo.
- La finalidad es continuar sin revisar conversaciones anteriores y sin duplicar documentos que puedan divergir.
- Si una decisión necesaria todavía no está en un archivo canónico, el resumen debe incluirla y marcar la obligación de registrarla antes de implementar la parte dependiente.

## 17. Uso coordinado de agentes

- Aplicar `SISTEMA_GENERAL_PRODUCCION_UI/02_FLUJO_DE_TRABAJO/REGLAS_USO_AGENTES.md`.
- Usar agentes cuando permitan auditorías independientes, investigación paralela o una
  revisión adicional proporcional al riesgo; no utilizarlos por rutina en tareas pequeñas.
- El agente principal conserva el contexto completo, el plan, la integración, la prueba
  final y la comunicación con el usuario.
- Los agentes auxiliares trabajan en modo lectura por defecto. Sólo pueden escribir cuando
  reciben un alcance exclusivo, disjunto y verificable.
- Cada archivo, sistema, escena, prefab, asset serializado, reconstructor y propiedad tiene
  un único escritor durante el bloque.
- No ejecutar varias instancias de Unity sobre la misma ruta de proyecto mediante agentes
  diferentes.
- Verificar localmente los hallazgos de los agentes y resolver contradicciones mediante
  evidencia reproducible. Su acuerdo no sustituye una prueba nueva ni la aprobación del
  usuario.
- Todo resumen de continuidad debe indicar qué auditorías se delegaron, qué resultados se
  integraron y cuáles quedaron pendientes o fueron descartados.

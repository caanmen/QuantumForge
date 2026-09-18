# Guía universal de desarrollo y calidad de código

## Propósito

Esta guía define cómo inspeccionar, modificar y validar código sin depender de que el
usuario conozca archivos, clases o pruebas. Se aplica a proyectos existentes y nuevos
cuando la tarea crea, corrige, integra o refactoriza comportamiento.

No sustituye el diseño canónico, el `AGENTS.md`, las instrucciones actuales ni las reglas
específicas del proyecto. El código real determina el estado técnico; el usuario conserva
la autoridad sobre el producto, la experiencia y las decisiones creativas.

## Entrada natural del usuario

El usuario puede describir un objetivo, un fallo o una lista numerada con lenguaje natural.
No debe identificar clases, estimar criticidad técnica ni proponer la solución. El
asistente convierte esa explicación en alcance, propietarios, dependencias, riesgos y
pruebas.

Antes de actuar debe distinguir:

- inspección o auditoría, que no autoriza cambios;
- corrección o implementación, que autoriza cambios dentro del alcance indicado;
- decisión de producto, balance o diseño, que sigue perteneciendo al usuario;
- refactorización técnica, que sólo se realiza cuando reduce un riesgo concreto y puede
  comprobarse sin reinterpretar el diseño.

## Auditoría previa obligatoria

Antes de modificar código:

1. Leer las instrucciones vigentes y el estado actual del proyecto.
2. Inspeccionar los archivos reales implicados y su historial documental aplicable.
3. Localizar la fuente definitiva y el propietario actual del comportamiento.
4. Identificar consumidores, datos persistentes, eventos, escenas, prefabs, generadores,
   herramientas y validadores relacionados.
5. Comprobar el estado de trabajo y conservar cambios previos ajenos al alcance.
6. Definir qué resultado observable demostrará que el bloque está terminado.
7. Elegir pruebas proporcionales antes de implementar, no después de descubrir el riesgo.

Un resumen anterior orienta la búsqueda, pero no reemplaza esta inspección.

## Matriz mínima de impacto

Evaluar únicamente las categorías relacionadas con el cambio y registrar las exclusiones
que no sean evidentes:

| Categoría | Pregunta bloqueante |
|---|---|
| Propiedad | ¿Existe un único componente responsable o se duplicaría el comportamiento? |
| Persistencia | ¿Cambia datos guardados, schema, valores iniciales, migraciones o recuperación? |
| Ciclo de vida | ¿Afecta inicio, pausa, reanudación, cierre, recarga o progreso offline? |
| Integración | ¿Cambia contratos entre lógica, presentación, UI, escenas o servicios? |
| Compatibilidad | ¿Deben seguir funcionando datos, APIs, escenas o llamadas anteriores? |
| Localización | ¿Aparecen textos visibles codificados dentro de lógica o sin clave traducible? |
| Rendimiento | ¿Añade trabajo por fotograma, búsquedas globales, asignaciones o E/S? |
| Herramientas | ¿Necesita DEBUG, datos de prueba, captura, setup o limpieza aislada? |
| Validación | ¿Qué prueba dirigida y qué regresión vecina detectarán un fallo real? |

Una categoría aplicable sin respuesta impide comenzar el cambio de alto riesgo.

## Forma segura de implementar

- Corregir la causa en su fuente definitiva; no acumular parches en consumidores.
- Mantener un solo propietario por estado, cálculo, interacción, texto o escritura.
- Implementar el bloque coherente más pequeño que pueda validarse en contexto real.
- No mezclar una función solicitada con limpieza general u otras refactorizaciones.
- No añadir una responsabilidad nueva a una clase concentradora si puede vivir en un
  módulo delimitado con una interfaz clara.
- En código heredado grande, conservar una fachada compatible y extraer por rebanadas;
  nunca dividir todo el sistema de una vez sólo para reducir líneas.
- Centralizar constantes, configuraciones y valores provisionales; no dispersar números o
  decisiones temporales.
- Mantener lógica de dominio separada de presentación, localización y herramientas cuando
  sea viable sin romper compatibilidad.
- Reutilizar contratos y utilidades existentes antes de crear un segundo sistema paralelo.
- No introducir una dependencia nueva sin justificar por qué el proyecto no puede resolver
  el problema con sus capacidades actuales.

## Persistencia y migraciones

Todo cambio de estado persistente debe comprobar, según corresponda:

1. serialización y deserialización del estado nuevo;
2. partida nueva con valores iniciales válidos;
3. carga de una partida anterior;
4. migración idempotente, sin otorgar ni borrar progreso dos veces;
5. rechazo seguro de versiones futuras incompatibles;
6. recuperación desde copia o respaldo sin sobrescribir el original inválido;
7. cierre y reapertura real o recarga desde una copia independiente;
8. pausa, reanudación y progreso offline sin duplicar recompensas;
9. herramientas DEBUG aisladas de partidas reales;
10. ausencia de escrituras inesperadas durante capturas o validaciones.

Una prueba que sólo serializa en memoria no demuestra persistencia completa.

## Contrato de validadores

Cada validador nuevo o actualizado debe declarar:

- sistema y riesgo que protege;
- precondiciones y datos preparados;
- ruta de entrada manual y, cuando aplique, entrada automatizable;
- aislamiento de partidas, archivos y configuración reales;
- limpieza garantizada incluso ante excepción;
- aserciones observables, no búsquedas frágiles de texto fuente como única prueba;
- resultado final inequívoco `PASS` o `FAIL`;
- código de salida correcto cuando se ejecute en batch;
- evidencia o log reproducible posterior al último cambio;
- límites: qué no demuestra esa prueba.

Las comprobaciones de estructura pueden leer archivos cuando la estructura textual sea el
contrato. El comportamiento debe probarse ejecutando el comportamiento siempre que sea
razonablemente posible.

## Estrategia de pruebas

Usar la menor combinación que demuestre el riesgo real:

1. **Comprobación estática:** estructura, referencias, formato, claves o inventario.
2. **Prueba de lógica:** cálculo o transición sin depender de una partida completa.
3. **Prueba de integración:** colaboración entre estado, servicio, UI, escena o archivo.
4. **Prueba de persistencia:** guardar, cerrar o recargar y comparar el resultado.
5. **Prueba runtime:** comportamiento que necesita fotogramas, eventos o ciclo de vida.
6. **Regresión dirigida:** sistema vecino que podría romperse por la modificación.
7. **Prueba humana:** sensación, claridad, ritmo, balance o aprobación visual.

No ejecutar toda la batería por rutina. Un cambio pequeño usa pruebas focalizadas; una
migración, integración transversal o cierre de versión necesita cobertura más amplia.

## DEBUG y herramientas de desarrollo

- Mantener el código exclusivamente editorial fuera del runtime o protegido de builds.
- No usar un estado alterado mediante DEBUG como evidencia de balance, progresión natural
  o migración real.
- Restaurar estado, archivos, resolución y suscripciones aunque la prueba falle.
- No escribir sobre partidas reales durante capturas, pruebas o preparación visual.
- Retirar, desactivar o documentar toda puerta temporal antes de una entrega.

## Evolución de código heredado

Clasificar cada mejora encontrada:

- **Corregir ahora:** defecto activo, pérdida de datos, seguridad, bloqueo, error repetido o
  deuda que impide el bloque solicitado.
- **Mejorar al tocar:** cambio local de bajo riesgo que reduce duplicación o aclara el
  propietario dentro del mismo bloque.
- **Planificar:** separación de clases grandes, ensamblajes, sustitución de infraestructura
  o migración que requiere varias etapas.
- **No cambiar:** estilo preferencial sin beneficio observable o código estable fuera del
  alcance.

Las clases grandes se reducen mediante extracciones verificables. Primero se crea el nuevo
propietario, después se mantiene compatibilidad, se migran consumidores por grupos y sólo
al final se elimina la ruta anterior.

## Aprendizaje acumulativo

Cuando un fallo se repita:

1. registrar síntoma, causa, corrección y alcance;
2. crear una aserción, prueba o validador que lo detecte;
3. conservar el caso específico dentro del proyecto;
4. promover a regla general sólo la parte demostrada y transferible;
5. enlazar la regla con su evidencia sin copiar todo el caso histórico.

No convertir una preferencia, coincidencia o solución de un solo proyecto en regla
universal sin evidencia suficiente.

## Cierre de un bloque de código

Antes de declarar terminado:

- el resultado solicitado está implementado y conectado;
- el proyecto compila sin errores relacionados;
- pasan las pruebas dirigidas y la regresión aplicable;
- guardado, carga y migración se probaron cuando corresponda;
- no quedan datos de prueba, suscripciones o archivos temporales;
- la evidencia pertenece a la última versión;
- las limitaciones y pruebas humanas pendientes se informan claramente;
- cualquier decisión nueva quedó en su fuente canónica;
- un fallo repetido quedó convertido en protección automática.

## Adopción por proyecto

Cada proyecto debe mantener un mapa corto que indique propietarios, fuentes definitivas,
áreas de riesgo, validadores disponibles y deuda planificada. El mapa no duplica el código
ni enumera cada archivo: enruta hacia las fuentes reales y se actualiza cuando cambian sus
límites.

El `AGENTS.md` del proyecto contiene sólo las reglas obligatorias y enlaza esta guía. Los
detalles particulares permanecen en `09_PROYECTOS/NOMBRE/`; el enrutador general decide
cuándo consultarlos.

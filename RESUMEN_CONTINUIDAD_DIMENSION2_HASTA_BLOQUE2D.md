# RESUMEN COMPLETO DE CONTINUIDAD — DIMENSIÓN 2 DE QUANTUM FORGE

Última actualización: 22 de julio de 2026  
Punto estable: Bloques 1, 2A–2G, 3A–3G y 4A–4G terminados y aprobados manualmente.
La corrección 4H y los Bloques 5A–5F están implementados, compilados y validados
automáticamente. Falta una única prueba manual integral de 4H y Bloque 5.  
Siguiente trabajo: prueba manual integral de Dimensión 2; después, runs largas
de balance con los números provisionales.

Este archivo es autosuficiente. El chat siguiente no debe volver a chats
anteriores ni repetir bloques terminados. Debe leer completamente este archivo,
leer los tres diseños originales y después inspeccionar los archivos reales del proyecto
antes de modificar nada.

## 1. Instrucción lista para iniciar el chat siguiente

> Lee completamente `RESUMEN_CONTINUIDAD_DIMENSION2_HASTA_BLOQUE2D.md`,
> `REGLAS_DE_TRABAJO_Y_CONTINUIDAD.md` y los tres archivos originales de Dimensión 2.
> Después inspecciona directamente los archivos actuales del proyecto. Continúa
> con Civilización 3 desde el Bloque 4D, sin rehacer los
> Bloques 1, 2A–2G ni 3A–3G. Usa `dimension de pactos 3 parte.txt` como diseño
> original de Civilización 3 y no inventes
> números o mecánicas pendientes sin permiso. No uses Git salvo petición explícita. El contexto obligatorio abarca
> toda Dimensión 2 hasta el Bloque 5, no solamente el Bloque 2.

## 2. Objetivo general y orden de autoridad

El proyecto es **Quantum Forge**, desarrollado en Unity. Dimensión 1 se
considera funcionalmente cerrada por ahora. El trabajo actual es completar
Dimensión 2 —la Dimensión de los Pactos— con tres civilizaciones, sus sistemas
propios y una integración final.

Orden de autoridad:

1. Instrucción actual y explícita del usuario.
2. Reglas completas incluidas en este resumen.
3. Los tres diseños originales y las decisiones creativas aprobadas por el usuario.
4. `DIMENSION2_DISENO_CANONICO_COMPLETO.md` queda solo como referencia secundaria.
5. Este resumen de continuidad.
6. Código y archivos reales para determinar el estado técnico.

Archivos originales de autoridad creativa:

- `C:\Users\nedfla\Downloads\dimension de pactos 1era parte.txt` — Civilización 1.
- `C:\Users\nedfla\Downloads\dimension 2 pactos 2da parte.txt` — Civilización 2.
- `C:\Users\nedfla\Downloads\dimension de pactos 3 parte.txt` — Civilización 3.

El código determina qué existe realmente. El diseño aprobado determina qué
debe existir. Si se contradicen, se informa antes de hacer cambios importantes.

## 2A. Revisión aprobada de Civilización 1 — 19 al 21 de julio de 2026

El usuario decidió conservar los bloques existentes y corregirlos sin
rediseñarlos por completo, usando el diseño original como autoridad. Se
implementó lo siguiente:

- **Rito del Camino:** ya no reduce duración. Mejora recompensas materiales y
  fracciones de Seguidores, sin aumentar Confianza. Límite provisional +40%.
- **Puerta Interior:** ya no aumenta la potencia de Ritos. Solo puede activarse
  tras el Umbral, aumenta +25% el progreso del Lugar de Vínculo y mantiene el
  compromiso de impedir iniciar o mejorar Noviciado.
- **Apoyo adicional:** Peregrinaciones y Noviciado permiten seleccionar 0–4
  Seguidores adicionales. Regresan al completar o cancelar. Peregrinaciones
  usan `sqrt(apoyo) × 0.20`, límite +40% material; Noviciado usa
  `sqrt(apoyo) × 0.18`, límite propio −35% y límite combinado −50%.
- **Altares avanzados:** Incienso se desbloquea con Ritos; Tela Sagrada con
  Pactos de Civilización; Piedra Tallada con el Umbral. Producen mediante la
  misma infraestructura de asignación que Cera y Pan ritual.
- **Lugar de Vínculo:** preparación provisional por 100 Incienso, 100 Tela
  Sagrada y 100 Piedra Tallada. Acólitos asignados generan progreso según raíz
  cuadrada, online y offline.
- **Cinco líneas, tres niveles:** Camino Peregrino, Oficio Sagrado y Orden de
  Acólitos son internas; Eco del Santuario mejora LE y Liturgia de Trazas mejora
  Trazas. La Máquina queda excluida. Costes y porcentajes están centralizados y
  son provisionales para afinamiento posterior.
- Beneficios provisionales por nivel: Camino +5% llegada/recompensa material;
  Oficio +7.5% producción de Altares; Orden +5% formación/potencia de Ritos;
  Eco +1% LE; Liturgia +1% Trazas.
- Niveles provisionales cuestan 20/40/60 de progreso y 25/50/75 de cada Ofrenda
  avanzada.

Estado técnico: versión de progreso de Civilización 1 elevada a 9; migración
por valores predeterminados; compilación de `Assembly-CSharp` y
`Assembly-CSharp-Editor` correcta; escena reconstruida y referencias conectadas;
validación automática 2A–2G correcta, incluyendo apoyos, Altares avanzados,
Lugar de Vínculo, guardado/carga y beneficios de LE/Trazas. El usuario confirmó
el 21 de julio de 2026 que la revisión visual y jugable se presenta y funciona
correctamente. Civilización 1 y el Bloque 2 quedan cerrados.

Correcciones posteriores a la primera prueba manual:

- El contador de una tanda activa de Noviciado ahora muestra el tiempo efectivo
  restante después de apoyo, Rito y Consagración; con nivel 4 y solo cuatro
  apoyos debe iniciar aproximadamente en 6:30, no mostrar los 10:00 base.
- El panel del Lugar de Vínculo se redistribuyó hacia arriba y el botón
  `PREPARAR LUGAR` se oculta después de prepararlo.
- El validador comprobó que los controles directos de Refugio, Altares,
  Peregrinaciones, Noviciado, Ritos, Pactos y Umbral quedan dentro de sus
  respectivos paneles. Todas las validaciones 2A–2G volvieron a finalizar bien.
- El usuario repitió las comprobaciones afectadas y confirmó que el contador de
  Noviciado, la redistribución del Umbral y el ocultamiento de `PREPARAR LUGAR`
  quedaron correctos.

## 3. Reglas completas de trabajo y continuidad

### Regla 1. Prioridad general

- El objetivo principal es completar Quantum Forge con la mayor rapidez posible sin sacrificar calidad, estabilidad, compatibilidad ni claridad.
- Se elegirá siempre el método más eficiente que permita verificar correctamente el resultado.
- Una instrucción actual y explícita del usuario tiene prioridad sobre estas reglas.
- El diseño aprobado por el usuario determina el contenido creativo y mecánico. El asistente no puede completar vacíos de diseño por iniciativa propia.
- El código y los archivos reales determinan el estado técnico actual del proyecto. El resumen determina la intención y continuidad. Si se contradicen, se debe informar antes de realizar cambios importantes.

### Regla 2. Al comenzar un chat nuevo

- Leer completamente el resumen de continuidad y las reglas incluidas en él.
- Revisar directamente los archivos actuales del proyecto antes de proponer o realizar cambios.
- No asumir que el resumen reemplaza al código real.
- Continuar desde el primer bloque pendiente; no repetir bloques ya terminados y probados.
- Si existe una contradicción entre código, resumen o diseño, explicarla y resolverla antes de implementar la parte afectada.

### Regla 3. Revisión previa a un bloque importante

Antes de iniciar un bloque funcional importante, indicar brevemente:

- Qué archivos o sistemas se modificarán.
- Por qué necesita modificarse cada uno.
- Qué comportamiento se espera conseguir.
- Si serán necesarias conexiones, automatización o pruebas posteriores en Unity.

Una vez que el usuario haya pedido implementar el bloque, el asistente puede avanzar por todas sus subdivisiones internas sin pedir autorización repetidamente. Solo debe detenerse si el usuario pidió únicamente una revisión, aparece una contradicción importante, falta una decisión de diseño o la acción tiene un riesgo material.

### Regla 4. Tamaño y forma de trabajo

- Trabajar en el bloque funcional completo más grande que pueda implementarse y verificarse de forma segura.
- Dividir internamente cada bloque en partes coherentes: estado y guardado, lógica, interfaz, conexiones, validaciones y pruebas.
- No detenerse ni pedir confirmación después de cada cambio pequeño.
- No modificar simultáneamente varios bloques grandes e independientes.
- No comenzar el siguiente bloque mientras el actual conserve conexiones o pruebas indispensables pendientes.
- Trabajar principalmente en C# y usar los recursos de Unity necesarios cuando el bloque los requiera.
- Mantener compatibilidad con partidas guardadas antiguas.
- Preservar los cambios existentes del usuario y evitar refactorizaciones generales sin una necesidad concreta.
- Priorizar primero funcionalidad correcta; dejar el pulido visual fino y el balance definitivo para después de disponer de una versión jugable.
- Si aparece un error de compilación o consola relacionado con el proyecto, detener el avance, corregirlo y volver a validar antes de continuar.

### Regla 5. Unity, jerarquía e Inspector

- El asistente está autorizado a crear y modificar los elementos de Unity necesarios para el bloque solicitado: escenas, jerarquías, paneles, botones, textos, desplegables, componentes, referencias del Inspector y eventos.
- El usuario no tiene que realizar manualmente conexiones, arrastrar referencias ni modificar jerarquías, salvo que expresamente prefiera hacerlo.
- Las modificaciones de escenas, prefabs, assets y archivos meta deben limitarse al alcance del bloque y realizarse con especial cuidado.
- Si Unity está abierto y puede sobrescribir cambios externos, avisar al usuario si debe cerrar el editor antes de modificar la escena.
- Unity puede permanecer abierto cuando solo se modifiquen scripts `.cs` o documentación; normalmente basta con salir de Play Mode y esperar la recompilación.
- Pedir cerrar Unity únicamente cuando se vayan a modificar externamente escenas, prefabs, assets, archivos `.meta` o conexiones serializadas que el editor pueda sobrescribir.
- No detenerse en Unity después de cada ajuste pequeño.
- Realizar las conexiones cuando el bloque alcance un punto funcional comprobable; no acumular todas las conexiones hasta el final del proyecto.
- El usuario ejecutará las pruebas visuales y jugables en Play Mode cuando se requiera validación dentro del editor y comunicará los resultados.
- No afirmar que una función está completamente jugable si solo funciona mediante DEBUG o todavía carece de una interfaz indispensable.

### Regla 6. Compilación, validaciones y pruebas

- Ejecutar comprobaciones ligeras de compilación durante hitos internos importantes cuando sean útiles.
- Realizar una compilación y validación integral al finalizar el código de cada bloque funcional.
- Agrupar las pruebas de Unity por hitos funcionales, no por cambios diminutos.
- Indicar al usuario exactamente qué debe probar en Play Mode y cuál es el resultado esperado.
- Separar claramente lógica terminada, conexiones realizadas, conexiones pendientes, pruebas automáticas, pruebas pendientes, problemas conocidos y decisiones provisionales.
- Probar guardado y carga cuando el bloque agregue o modifique estado persistente.
- Probar una partida nueva y una antigua cuando existan migraciones o compatibilidad de guardado involucradas.
- No usar una partida alterada mediante DEBUG como evidencia de balance o migración normal.
- El balance definitivo se realizará después de tener una versión funcional y jugable.

### Regla 7. Git y protección del trabajo

- No crear commits, ramas, staging, push, reset ni otras operaciones de Git salvo petición explícita del usuario.
- Se permiten consultas de solo lectura cuando sean necesarias para comprobar el estado solicitado por el usuario.
- Recomendar puntos de guardado en Git antes de cambios delicados y después de validar un bloque, pero dejar la decisión y ejecución al usuario salvo autorización expresa.
- No eliminar, reemplazar ni descartar cambios existentes del usuario.
- No modificar archivos ajenos al alcance del bloque.
- No realizar builds pesados cuando una comprobación ligera sea suficiente.

### Regla 8. Autonomía y momentos para detenerse

Detenerse únicamente cuando falte una decisión material de diseño, exista una contradicción importante, aparezca un error que bloquee, sea necesaria una prueba del usuario, la acción sea destructiva o externa al alcance, o el bloque esté listo para cerrarse.

### Regla 9. Cierre de cada bloque

Antes de declarar un bloque terminado, confirmar código, compilación, guardado/carga, compatibilidad, validaciones, conexiones, prueba funcional en Unity, corrección de errores, documentación de decisiones provisionales y resumen al usuario. Si falta una conexión o prueba indispensable, describirlo como “código terminado, validación pendiente”.

### Regla 10. Chats largos y cambio de conversación

- Avisar antes de que el chat pierda claridad.
- Recomendar chat nuevo en un punto seguro.
- No iniciar un bloque grande nuevo si conviene preparar primero la continuidad.
- Entregar un resumen completo, autosuficiente y listo para pegar.
- El chat nuevo no debe necesitar conversaciones anteriores.

### Regla 11. Contenido obligatorio de todo resumen

Debe incluir objetivo, reglas completas, autoridad, todos los bloques terminados y futuros, estado exacto, funciones, archivos, conexiones, pruebas, errores, advertencias, contradicciones, estado DEBUG, siguiente bloque, subdivisión propuesta, revisión obligatoria de archivos y prohibición de Git salvo petición.

### Regla 12. Autosuficiencia y transferencia

- Las reglas viajan completas en cada resumen.
- Aunque se solicite un resumen corto, no se omiten reglas esenciales.
- Debe incluir también bloques futuros y explicar qué falta para cerrar todo.
- No puede depender de “según lo hablado antes”.
- Debe registrar decisiones importantes y contradicciones sin resolver.

### Regla 13. Modelos y herramientas

- Avisar si una tarea especialmente amplia o delicada puede beneficiarse de mayor capacidad, sin detener tareas normales.
- La decisión de cambiar de modelo corresponde al usuario.
- Usar automatización cuando reduzca trabajo manual sin perder seguridad.

### Regla 14. Inicio recomendado

Leer reglas y resumen completos, inspeccionar archivos reales, anunciar archivos/comportamiento/pruebas antes del bloque, avanzar autónomamente, no usar Git y no repetir bloques terminados.

### Regla 15. Fidelidad obligatoria al diseño

- El juego y su dirección creativa pertenecen al usuario. El asistente implementa el diseño aprobado; no lo sustituye por preferencias, inferencias o decisiones propias.
- Antes de implementar una mecánica, revisar el documento original y el diseño canónico aplicable.
- No inventar ni fijar sin permiso recursos, costes, fórmulas, tasas, cantidades iniciales, niveles, límites, requisitos, recompensas, penalizaciones, desbloqueos, simultaneidad, relaciones, textos narrativos determinantes ni comportamientos ausentes.
- Una inferencia razonable no se convierte automáticamente en diseño canónico. Si hay varias interpretaciones, señalarlo y esperar decisión.
- Cuando falte una decisión, detener la parte afectada y presentar qué falta, por qué, opciones e impacto. No aplicar ninguna sin permiso.
- Los valores temporales de prueba también requieren autorización, deben marcarse provisionales y mantenerse centralizados.
- No presentar una decisión del asistente como diseño original.
- Registrar decisiones nuevas aprobadas antes o junto con su implementación.
- Si ya se implementó algo no respaldado, informar, detener dependencias y pedir decisión; no corregirlo silenciosamente.
- Todo resumen debe reproducir esta regla y señalar elementos pendientes de aprobación.

### Regla 16. Alcance total obligatorio de los resúmenes

- Un resumen no puede limitarse al bloque, subdivisión o civilización actual.
- Debe transferir todo el alcance hasta el cierre: terminado, activo, futuro, integración, decisiones, contradicciones y pendientes.
- Para Dimensión 2 debe incluir obligatoriamente los Bloques 1, 2, 3, 4 y 5 completos aunque el trabajo siga dentro del 2.
- La finalidad es pasar de chat en chat sin volver a conversaciones anteriores.
- No basta enlazar otro chat o decir que el resto está en un documento anterior. Debe reproducir todas las estructuras, dependencias y decisiones necesarias para no inventar ni perder contexto.

## 4. Estado global exacto

| Bloque | Contenido | Estado |
| --- | --- | --- |
| Dimensión 1 | Sistemas jugables actuales | Cerrada funcionalmente por ahora |
| D2 Bloque 1 | Base común, estado, guardado, pestaña, introducción y mapa | Terminado y probado |
| D2 2A | Refugio y Seguidores | Terminado y probado |
| D2 2B | Altares y Ofrendas | Terminado y probado |
| D2 2C | Peregrinaciones y Confianza | Terminado y probado |
| D2 2D | Noviciado y Acólitos | Terminado y probado |
| D2 2E | Ritos | Terminado y probado |
| D2 2F | Pactos de Civilización | Terminado y probado |
| D2 2G | Umbral Velado y Lugar de Vínculo | Terminado y probado |
| D2 Bloque 3 | Civilización 2 | Terminado y probado; balance largo pendiente |
| D2 4A | Zona 1, excavación, restos y Erudito de Campo | Terminado y probado |
| D2 4B | Análisis, recursos, Investigación e Archivo I | Terminado y probado |
| D2 4C | Zona 2, Archivo II e Indicios | Terminado y probado |
| D2 4D | Anomalías y Datos Anómalos | Terminado y probado |
| D2 4E | Zona 3, Archivo IV y Anomalía Profunda | Terminado y probado |
| D2 4F | Investigación y Conocimiento del Ente | Terminado y probado |
| D2 4G | Pacto y mejoras internas de Dimensión 2 | Terminado y probado |
| D2 4H | Mejoras de Eruditos y Archivo recuperadas de los TXT originales | Implementado y validado automáticamente; prueba integral pendiente |
| D2 5A | Matriz definitiva de recompensas y conexiones | Diseño provisional aprobado |
| D2 5B | Pacto mayor de Civilización 1 | Terminado y probado |
| D2 5C | Pacto mayor de Civilización 2 | Terminado y probado |
| D2 5D | Pacto mayor de Civilización 3 | Implementado y validado automáticamente; prueba integral pendiente |
| D2 5E | Integración cruzada y offline integral | Implementado y validado automáticamente; prueba integral pendiente |
| D2 5F | Localización, pulido, migraciones y cierre funcional | Implementado y validado automáticamente; prueba integral pendiente |

## 5. Implementación terminada

### Bloque 1

- Estado raíz serializable de Dimensión 2 y tres civilizaciones.
- Integración en `GameState` y `SaveService`, migración de partidas anteriores.
- Progreso offline general con límite actual de 12 horas.
- Pestaña Dimensión 2 en `TabsUI`.
- Entrada inicial que no se repite.
- Mapa con tres territorios; Civ 1 disponible, Civ 2 y Civ 3 bloqueadas.
- Botón CERRAR abajo a la derecha y VOLVER AL MAPA abajo a la izquierda.
- La interfaz y narrativa temprana no revelan Entes.

### 2A — Refugio y Seguidores

- 5 Seguidores iniciales.
- Llegada base `0.05/s` = 3/minuto.
- Asignación exclusiva al Refugio.
- Multiplicador `1 + (√asignados × 0.15)`.
- Refugio niveles 1–10.
- Cada nivel posterior añade 25% de producción base.
- Coste `techo(12 × 1.85^(nivel actual - 1))` Seguidores.
- Producción online/offline con fracciones conservadas.

### 2B — Altares y Ofrendas

- Cinco Altares y cinco saldos separados.
- Iniciales: Cera y Pan Ritual.
- Avanzados: Incienso se abre con Ritos, Tela Sagrada con Pactos y Piedra
  Tallada con el Umbral de 500.
- Sin niveles ni mejoras de Altar.
- Producción base `0.05 Ofrendas/s` por Altar activo.
- Multiplicador `1 + (√Seguidores asignados × 0.35)`.
- Asignación exclusiva y producción online/offline.
- UI muestra Seguidores disponibles y asignados para no operar a ciegas.
- Los cinco usan la misma producción y asignación; los avanzados alimentan el
  Lugar de Vínculo y sus líneas.

### 2C — Peregrinaciones y Confianza

Una actividad simultánea. Costes al iniciar; cancelar devuelve unidades pero no
Ofrendas. Finalización y recompensas automáticas, online/offline y sin duplicar.

| Tipo | Tiempo | Unidades | Coste | Recompensa |
| --- | ---: | --- | --- | --- |
| Corta | 1 min | 1 Seguidor | 2 Cera + 2 Pan | 1 Confianza + 1 Cera + 1 Pan |
| Media | 4 min | 3 Seguidores | 10 + 10 | 5 Confianza + 3 + 3 + 25% de 1 Seguidor |
| Larga | 10 min | 6 Seguidores | 25 + 25 | 12 Confianza + 8 + 8 + 1 Seguidor |

- La probabilidad de la Media se decide una sola vez al iniciar y se guarda.
- Confianza 0–500.
- Civ 2 se desbloquea permanentemente a 300 (60%).
- A 500 aparece **Umbral Velado: ALGO RESPONDE**.
- Ningún texto temprano debe decir Ente, contacto con el Ente o pacto mayor.
- Los umbrales 300/500 son canónicos actuales; el balance de costes es provisional.
- Se pueden seleccionar 0–4 Seguidores de apoyo. Se suman a los obligatorios,
  regresan al completar/cancelar y mejoran solo recompensas materiales y
  fracciones de Seguidores mediante `sqrt(apoyo) × 0.20`, límite +40%.

### 2D — Noviciado y Acólitos

- Tandas niveles 1–5: `1/2/4/7/10` Acólitos en `5/6/8/10/12` minutos.
- Seguidores convertidos: `5/10/20/35/50`.
- Cera y Pan por tanda: `12/22/40/65/90` de cada uno.
- Mejoras 1→2, 2→3, 3→4, 4→5:
  - Seguidores: `25/60/140/300`;
  - Cera y Pan: `30/75/160/350` de cada uno.
- Solo una tanda activa. Seguidores ocupados se convierten al completar.
- Cancelar devuelve Seguidores, no Ofrendas.
- Formación offline sin duplicación.
- Acólitos sin límite duro.
- Peregrinación Larga con Acólito: 10 min, 6 Seguidores + 1 Acólito,
  coste 35+35, recompensa 16 Confianza, 10+10 y 1 Seguidor.
- Peregrinación Sagrada: 15 min, 8 Seguidores + 2 Acólitos,
  coste 50+50, recompensa 25 Confianza, 15+15 y 2 Seguidores.
- Acólitos ocupados en Peregrinaciones regresan al terminar o cancelar.
- Noviciado admite 0–4 Seguidores de apoyo que no se convierten y regresan al
  completar/cancelar. Reducen duración mediante `sqrt(apoyo) × 0.18`, límite
  propio −35% y límite combinado con Rito/Orden de Acólitos −50%.
- Todos los números de 2C/2D son balance provisional autorizado para runs.

## 6. Bloque 2 cerrado — Civilización 1

### 2E — Ritos

Cinco Ritos implementados:

| Rito | Función |
| --- | --- |
| Recibimiento | Mejora llegada de Seguidores |
| Ofrenda | Mejora producción de Altares |
| Camino | Mejora recompensas/preparación de Peregrinaciones |
| Noviciado | Mejora formación de Acólitos |
| Respeto | Mejora Confianza de Peregrinaciones |

- Se desbloquean después de formar el primer Acólito.
- Dos espacios activos inicialmente; no se puede repetir el mismo Rito.
- Seguidores y Acólitos asignados quedan ocupados y pueden retirarse sin coste
  ni penalización.
- No existe coste de activación ni mantenimiento: la población ocupada es el
  compromiso del sistema.
- Funcionan de forma continua online y offline, y no generan Confianza por sí solos.
- Potencia provisional autorizada:
  `2.5 × √Seguidores + 6 × √Acólitos`, expresada como porcentaje.
- Límites provisionales:
  - Recibimiento: +50% llegada de Seguidores;
  - Ofrenda: +60% producción de Altares;
  - Camino: +40% máximo a recompensas materiales de Peregrinaciones;
  - Noviciado: -35% duración de formación;
  - Respeto: +35% Confianza de Peregrinaciones.
- El tercer espacio se desbloquea permanentemente con 250 Confianza,
  Noviciado nivel 3 y el consumo de 5 Acólitos, 150 Cera y 150 Pan ritual.
- Los efectos se calculan dinámicamente a partir de las asignaciones guardadas.
  Noviciado acelera también actividades ya iniciadas y el progreso offline.
  Camino no modifica duración ni Confianza. Respeto conserva el límite general
  de Confianza máximo ×2.
- Todos estos valores son provisionales autorizados y se ajustarán mediante
  pruebas sin cambiar la estructura del sistema.
- Estado actual: código, guardado/migración, UI, conexiones, validación
  automática y prueba manual terminados. El usuario confirmó el 19 de julio de
  2026 que todo se ve correctamente en Play Mode.

### 2F — Pactos de Civilización

| Pacto | Beneficio | Compromiso |
| --- | --- | --- |
| Hospedaje | +35% llegada de Seguidores | Consume 1 Pan ritual/minuto |
| Camino Abierto | +25% recompensas de Peregrinaciones | +25% costes de Ofrendas |
| Consagración | +25% Acólitos por tanda | +30% duración de formación |
| Voto Silencioso | +50% Confianza | -90% llegada de Seguidores |
| Puerta Interior | +25% progreso del Lugar de Vínculo | Impide iniciar o mejorar Noviciado |

Diseño provisional aprobado e implementado:

- Se desbloquean a 200 Confianza y Noviciado nivel 2.
- Un espacio activo inicialmente.
- Segundo espacio permanente: 400 Confianza, Noviciado nivel 4 y consumo de
  10 Acólitos, 300 Cera y 300 Pan ritual.
- Costes de activación Cera/Pan:
  - Hospedaje 60/90;
  - Camino Abierto 90/90;
  - Consagración 120/120;
  - Voto Silencioso 150/150;
  - Puerta Interior 200/200.
- Cancelar no devuelve costes, no añade penalización y no tiene cooldown.
- No existe mantenimiento común; el compromiso de cada Pacto es continuo.
- Hospedaje consume Pan de forma online/offline. Si no alcanza, se suspende y
  se reanuda automáticamente cuando vuelve a existir Pan, sin otro coste.
- Camino Abierto aumenta Confianza, Ofrendas y recompensas fraccionales de
  Seguidores; sus costes visibles se actualizan dinámicamente.
- Consagración conserva fracciones de Acólito entre tandas.
- Voto Silencioso, Camino Abierto y Respeto respetan juntos el límite general
  máximo ×2 de Confianza.
- Puerta Interior solo puede activarse después del Umbral, no cambia Ritos y
  aumenta 25% la generación de progreso del Lugar de Vínculo.
- Valores centralizados y provisionales para balance posterior.
- Estado actual: código, migración, UI, conexiones, validación automática y
  prueba manual terminados. El usuario confirmó que su presentación se veía bien.

### 2G — Umbral Velado

- A 500 algo desconocido responde.
- Presentación: **Umbral Velado**, sin revelar su naturaleza.
- Se desbloquea permanentemente el botón UMBRAL y se presenta el Lugar de Vínculo.
- El Lugar de Vínculo se prepara provisionalmente con 100 Incienso, 100 Tela
  Sagrada y 100 Piedra Tallada.
- Acólitos asignados generan progreso con raíz cuadrada, online y offline.
- Cinco líneas de tres niveles, costes provisionales 20/40/60 progreso y
  25/50/75 de cada Ofrenda avanzada:
  - Camino Peregrino: +5% llegada y recompensas materiales por nivel.
  - Oficio Sagrado: +7.5% producción de Altares por nivel.
  - Orden de Acólitos: +5% formación y potencia de Ritos por nivel.
  - Eco del Santuario: +1% producción de LE por nivel.
  - Liturgia de Trazas: +1% producción de Trazas por nivel.
- La Máquina está expresamente excluida de estos beneficios.
- No usar textos visibles “Ente”, “contacto con el Ente” o “pacto mayor”.
- Estado actual: lógica, persistencia, migración, UI, conexiones, validación
  automática y prueba visual terminadas. El usuario confirmó en Play Mode que
  UMBRAL se habilita correctamente al establecer 500 de Confianza y se presenta bien.

## 7. Bloque 3 completo — Civilización 2

Identidad: civilización sometida; Resistencia contra una fuerza hostil. Se abre
a 300 de Confianza de Civ 1.

### Estado técnico de 3A — 21 de julio de 2026

- Estado persistente de Civilización 2 elevado a versión 2 con migración desde
  el estado reservado anterior.
- Diez Miembros de Resistencia iniciales provisionales, saldo global entero,
  total histórico y acumulador fraccional preparado para 3B.
- Catálogo persistente de cuatro regiones: Región 1 disponible; Regiones 2 y 3
  bloqueadas; Región 4 visible como actualización futura.
- Región 1 comienza con 100% de Dominio y 0% de Amenaza.
- Asignación regional exclusiva a Región 1 mediante +1, +10, asignar todo, -1
  y retirar todo; el total de Miembros se conserva.
- Panel propio de Civilización 2 conectado al mapa, con Dominio total, estados
  regionales, saldo/asignación y navegación de regreso.
- `Dimension2System` ya inicializa, valida y prepara tick/offline de Civ 2 sin
  introducir todavía operaciones ni producción de 3B.
- Compilación de runtime y editor correcta; configuración de escena,
  validación automática y prueba manual en Play Mode correctas. El usuario
  confirmó el 21 de julio de 2026 que todo se ve y funciona bien; 3A cerrado.

### Regiones y Dominio

Estado técnico de 3D — 21 de julio de 2026:

- Región 2 se desbloquea permanentemente con Dominio total ≤80%; Región 3 con
  Dominio total ≤60% después de Región 2.
- Cada región nueva comienza en 100% de Dominio y 0% de Amenaza; el promedio
  puede subir al incorporarla y la interfaz explica que no es una pérdida.
- Los desbloqueos nunca se revierten aunque el promedio vuelva a subir.
- Selector persistente de región disponible compartido por REGIONES,
  OPERACIONES y DEFENSA; Región 4 permanece visible pero no seleccionable.
- Asignaciones, cuatro operaciones, Amenaza, Cobertura, Espionaje y Represalias
  funcionan independientemente en la región seleccionada.
- Dominio total es el promedio de todas las regiones jugables desbloqueadas.
- Desbloqueos y progreso regional funcionan online/offline y se serializan.
- Versión de progreso de Civ 2 elevada a 5 con migración automática.
- Compilación, conexiones, validación automática y prueba manual correctas. El
  usuario confirmó el 21 de julio de 2026 los desbloqueos, el selector regional
  y el promedio con repunte; 3D cerrado.

- Tres regiones jugables y una cuarta visible para futuro.
- Región 1 inicial; Región 2 a 80% Dominio total; Región 3 a 60%; Región 4 bloqueada.
- Cada región: Dominio 0–100, Amenaza 0–100, asignaciones, operaciones,
  Cobertura, modificadores y Represalias.
- Todas comienzan en 100% Dominio. Dominio total = promedio de regiones activas.
- Al abrir una región nueva el promedio puede subir, sin perder desbloqueos.
- A 30% Dominio total: abre Civ 3, Contención y fase de Alerta.

### Miembros y operaciones

Estado técnico de 3B — 21 de julio de 2026:

- El usuario aprobó varias operaciones simultáneas por región.
- Cada operación mantiene su propia asignación exclusiva; los Miembros pasan
  del saldo global a Región 1 y desde allí se destinan sin duplicación.
- Cuatro operaciones persistentes: Rescate, Protección, Espionaje y Sabotaje.
- Una operación funciona automáticamente al alcanzar su requisito de
  `5/5/10/20` Miembros y se detiene si baja de él.
- Dominio y Amenaza usan las tasas provisionales originales por minuto.
- Rescate y Protección generan fracciones de Miembros mediante raíz cuadrada;
  las fracciones se conservan y los Miembros completos regresan al saldo global.
- Las operaciones progresan online y offline. La Amenaza se limita a 0–100;
  Represalias todavía no se ejecutan porque pertenecen a 3C.
- La generación y reglas completas de Cobertura también quedan para 3C, donde
  primero debe aprobarse su límite, consumo o deterioro.
- Nueva vista `OPERACIONES` con selector, estado, efectos, requisitos,
  distribución regional y controles +1/+5/todo/-1/retirar todo.
- Versión de progreso de Civ 2 elevada a 3 con migración automática.
- Compilación, conexiones, validación automática y prueba manual en Play Mode
  correctas. El usuario confirmó el 21 de julio de 2026 que todo se ve y
  funciona bien; 3B cerrado.

- Inicio tentativo 10 Miembros; sin límite duro; asignación exclusiva.
- Efectividad base `√Miembros × factor`.

| Operación | Miembros | Efecto | Dominio/min | Amenaza/min |
| --- | ---: | --- | ---: | ---: |
| Rescate | 5 | Genera Miembros | -0.05% | +0.20% |
| Protección | 5 | Baja Amenaza, genera Cobertura | -0.02% | -0.35% |
| Espionaje | 10 | Avance seguro | -0.12% | +0.25% |
| Sabotaje | 20 | Avance rápido | -0.30% | +0.60% |

- Rescate: `√Miembros × 0.20 Miembros/min`.
- Protección: `√Miembros × 0.08 Miembros/min` y `×0.25 Cobertura/min`.
- Espionaje reduce 5% de la próxima Represalia.
- Falta decidir simultaneidad por región; recomendación no aprobada: permitir
  varias solo con asignaciones separadas.

### Represalias, Cobertura y Fragmentos

Estado técnico de 3C — 21 de julio de 2026:

- Configuración provisional aprobada por el usuario y centralizada para runs.
- Cobertura máxima 60; Protección genera `√Miembros × 0.25/min`.
- Cada 10 de Cobertura reduce un punto porcentual de pérdida; mínimo 2%.
- Una Represalia consume la mitad de la Cobertura acumulada.
- Espionaje prepara una reducción máxima de 5% para la próxima Represalia; no
  se acumula, se consume y vuelve a prepararse si Espionaje continúa activo.
- Amenaza 100 dispara Represalia, vuelve a 25 y entrega 3 Fragmentos de Control.
- Pérdidas base 8%, redondeadas hacia arriba con al menos una pérdida cuando
  existen Miembros expuestos. Se reconcilian sin duplicar ni romper asignaciones.
- Una operación activa se elige aleatoriamente y funciona al 50% durante 3 min.
- Cobertura, Represalias, debilitamiento y Fragmentos progresan online/offline
  y se guardan. La simulación puede resolver eventos dentro de periodos offline.
- Nueva vista `DEFENSA`: Amenaza, Cobertura, pérdida estimada, preparación de
  Espionaje, Fragmentos, contador de Represalias y debilitamiento restante.
- Versión de progreso de Civ 2 elevada a 4 con migración automática.
- Compilación, conexiones, validación automática y prueba manual correctas. El
  usuario confirmó el 21 de julio de 2026 una Represalia funcional: Amenaza a
  25%, Cobertura aproximadamente a la mitad, una pérdida, 3 Fragmentos y
  Sabotaje debilitado al 50% durante 3 minutos. 3C cerrado.

- Amenaza 100% dispara Represalia.
- Tentativo: pierde 8% de asignados, debilita una operación 3 min, entrega 3
  Fragmentos (6 en Alerta), Amenaza vuelve a 25%.
- Cobertura: cada 10 reduce 1 punto de pérdida; mínimo 2%.
- Falta límite/consumo/deterioro de Cobertura y límite de Espionaje.
- Fragmentos mejoran operaciones, defensa y Pactos. Faltan niveles y costes.

### Pactos de Resistencia

- Refugios Ocultos: 30 asignados, -20% pérdidas, desgaste 1/2 min.
- Campanas Silenciadas: 25, -30% debilitamiento, desgaste 1/2 min.
- Cuchillos Bajo la Mesa: 40, +20% Sabotaje, desgaste 1/90 s.
- Penalizaciones tentativas por incumplir existen, pero falta definir el destino
  de Miembros desgastados y duración exacta de penalizaciones.

### Alerta y Contención

- Desde 30%: Rescate/Espionaje/Sabotaje generan +50% Amenaza; Represalias dan 6
  Fragmentos; cada 10 min puede marcarse una región; Protección mitiga.
- Recomendación no aprobada: una marca por región, renovable y no acumulable.
- Probabilidad tentativa de Contención: 30%=20%, 20%=45%, 10%=70%, 0%=100%.
- La fórmula histórica no coincide con la tabla; debe resolverse antes de código.
- Fallo tentativo: +20% Amenaza global, -5% Miembros no protegidos, cooldown 10 min.
- Éxito: fuerza contenida, preparación de progresión final y sostén con Miembros.
- Duración objetivo tentativa: 4–6 h atento, 7–9 h pasivo.
- Subbloques: 3A estado/Región1; 3B operaciones; 3C Represalias; 3D regiones;
  3E mejoras/Pactos; 3F Alerta; 3G Contención.

## 8. Bloque 4 completo — Civilización 3

Identidad: ruinas de una civilización perdida. Se abre cuando Civ 2 llega a 30%
de Dominio total. La narrativa debe preservar el misterio.

### Zonas y ciclo

| Zona | Nombre tentativo | Recurso |
| --- | --- | --- |
| 1 | Entrada Sepultada | Fragmentos Base |
| 2 | Galería de Inscripciones | Inscripciones Parciales |
| 3 | Santuario Sellado | Sellos Antiguos |

Excavar → restos → analizar con Erudito → Conocimiento Antiguo + recurso de
zona → mejorar zona/Erudito/Archivo → aumentar Investigación.

Calidad tentativa Baja/Media/Alta:

- Zona 1: 70/25/5%.
- Zona 2: 50/35/15%.
- Zona 3: 30/45/25%.

Decisiones provisionales aprobadas e implementadas para 4A:

- Zona 1 se llama **Entrada Sepultada** y es la única desbloqueada al comenzar.
- Cada zona admite una excavación activa; cuando existan varias zonas podrán
  operar simultáneamente entre sí.
- Cada excavación dura 30 segundos, no consume recursos y entrega un resto.
- Zona 1 usa 70% Baja, 25% Media y 5% Alta.
- El inventario de restos no tiene límite.
- Las excavaciones se inician únicamente de forma manual. Una excavación ya
  iniciada progresa online y offline.
- Ningún sistema tendrá automatización por ahora; esa capacidad llegará desde
  Dimensión 3.
- No se necesita Erudito para excavar. El análisis de restos comienza en 4B.
- El Erudito de Campo se contrata una sola vez por 10 Cera y 10 Pan ritual, sin
  mantenimiento. Sus mejoras se definirán en 4B.
- Los valores anteriores son provisionales y se afinarán mediante pruebas largas.

Decisiones provisionales aprobadas e implementadas para 4B:

- El análisis es manual, uno activo por zona, sin cola, cancelación ni
  automatización; consume un resto al comenzar y progresa online/offline.
- Requiere el Erudito de Campo. Duración base: 30 segundos.
- Calidad Baja: 1 Conocimiento Antiguo, 1 Fragmento Base y 1% de Investigación.
- Calidad Media: 3 Conocimiento, 2 Fragmentos y 3% de Investigación.
- Calidad Alta: 8 Conocimiento, 4 Fragmentos y 8% de Investigación.
- Al 20%, cada excavación acumula 5% hacia un resto adicional; al 40%, el
  análisis tarda 5% menos; al 60%, queda habilitado el requisito de Zona 2 para
  4C; al 80%, los análisis entregan 10% más Conocimiento.
- El efecto especial del 100% queda aplazado hasta la integración final.
- Archivo I se desbloquea automáticamente tras el primer análisis. En 4B
  muestra recursos, progreso e hitos; sus mejoras comprables se definirán con
  el resto del Archivo.

Decisiones provisionales aprobadas e implementadas para 4C:

- Zona 2, Galería de Inscripciones, exige Zona 1 al 60% y pago de 25 Incienso
  más 25 Tela Sagrada.
- Sus excavaciones son manuales, duran 30 segundos, usan 50% Baja, 35% Media y
  15% Alta, y pueden coexistir con actividades de Zona 1.
- El Erudito de Inscripciones cuesta 20 Cera y 20 Pan ritual, una sola vez y
  sin mantenimiento. El análisis usa la misma tabla de 4B, entregando como
  recurso propio Inscripciones Parciales.
- Archivo II se desbloquea automáticamente con Zona 1 al 40%; sus compras
  específicas siguen aplazadas hasta cerrar el Archivo completo.
- La detección de Indicios se habilita permanentemente con Zona 2 al 20%.
  Baja/Media/Alta acumulan 3/8/18% respectivamente; cada 100% entrega un
  Indicio y conserva cualquier excedente. Zona 1 genera Básicos y Zona 2
  Simbólicos. Revelar y leer Anomalías corresponde a 4D.
- No existe automatización, cola ni cancelación.

Decisiones provisionales aprobadas e implementadas para 4D:

- Una Anomalía única por zona; la Básica se revela con 8 Indicios y la
  Simbólica con 10. Los Indicios quedan como registro y no se consumen.
- Archivo III se desbloquea automáticamente con Zona 2 al 40% y habilita la lectura.
- Cada Anomalía puede leerse una sola vez y queda archivada como leída.
- Lectura Básica: 25 Conocimiento Antiguo + 15 Fragmentos Base.
- Lectura Simbólica: 40 Conocimiento Antiguo + 25 Inscripciones Parciales.
- Cada lectura entrega 1 Dato Anómalo del tipo correspondiente.
- La estructura Profunda queda preparada, pero su lectura corresponde a 4E.
- Estado técnico: estado y migración a versión 6, lógica, UI, escena, referencias,
  compilación runtime/editor, validación automática y prueba visual/jugable
  terminadas. El usuario aprobó 4D el 22 de julio de 2026.

Decisiones provisionales aprobadas e implementadas para 4E:

- Zona 3, Santuario Sellado, exige Zona 2 al 60% y 50 Incienso, 50 Tela
  Sagrada y 50 Piedra Tallada.
- Erudito de Sellos cuesta 30 Cera y 30 Pan ritual, una sola vez y sin mantenimiento.
- Excavación manual de 30 segundos, simultánea con las otras zonas, con calidad
  30% Baja, 45% Media y 25% Alta. No hay cola ni automatización.
- El análisis conserva 30 segundos y recompensas 1/3/8 de Conocimiento,
  1/2/4 Sellos Antiguos y 1/3/8% de Investigación.
- Archivo IV se abre automáticamente con Zona 3 al 30%.
- La Anomalía Profunda se revela con 12 Indicios no consumibles; su lectura
  única cuesta 60 Conocimiento y 35 Sellos, y entrega 1 Dato Profundo.
- Los bonus especiales del 100% quedan aplazados hasta 4G.
- Estado técnico: migración a versión 7, lógica, UI, escena, referencias,
  compilación runtime/editor, validación automática y prueba visual/jugable
  terminadas. El usuario aprobó 4E el 22 de julio de 2026.

Decisiones provisionales aprobadas e implementadas para 4F:

- Se desbloquea permanentemente al reunir 1 Dato Básico, 1 Simbólico y 1 Profundo.
- Vista propia con inicio y pausa manual; una investigación iniciada progresa
  online/offline y espera automáticamente si falta Conocimiento Antiguo.
- Conversión provisional 1 Conocimiento = 1%; velocidad 1% cada 30 segundos.
- Se detiene exactamente en hitos 30/60/85/100% sin consumir excedentes.
- 30%: 25 Fragmentos + 1 Dato Básico → +1 Conocimiento del Ente.
- 60%: 35 Inscripciones + 1 Dato Simbólico → +2 Conocimientos del Ente.
- 85%: 45 Sellos + 1 Dato Profundo → +3 Conocimientos del Ente.
- 100%: 50 de cada recurso de zona → prepara el Pacto para 4G.
- Conocimiento del Ente permanente, acumulativo, total inicial 6 y no gastable
  hasta cerrar el diseño de 4G.
- Estado técnico: migración a versión 8, lógica, UI propia, escena, referencias,
  compilación runtime/editor, validación automática y prueba visual/jugable
  terminadas. El usuario aprobó 4F el 22 de julio de 2026.

Decisiones provisionales aprobadas e implementadas para 4G:

- Establecer el Pacto no añade otro coste después del pago del hito 100%.
- Tres líneas independientes, tres niveles cada una:
  - Expedición Resonante: +10% acumulación de restos adicionales por nivel.
  - Archivo Inagotable: +10% Conocimiento Antiguo y recursos de zona por
    análisis por nivel.
  - Memoria Compartida: +3% resultados positivos repetibles de Civ1/Civ2 por nivel.
- Conocimiento del Ente permanente y no gastable; umbrales 1/3/6 para niveles
  1/2/3.
- Costes por nivel y por línea: 50/100/150 Conocimiento Antiguo y 25/50/75 de
  cada recurso de zona.
- Memoria mejora Seguidores, Altares, recompensas materiales de Peregrinación,
  Acólitos, progreso del Lugar de Vínculo, Miembros, reducción de Dominio y
  Cobertura. No modifica costes, Amenaza, pérdidas, Represalias ni la Máquina.
- Bonus de zonas al 100%: Zona1 +10% acumulación de restos en todas las zonas;
  Zona2 análisis globales 10% más rápidos; Zona3 +10% recompensas de análisis.
- Estado técnico: migración a versión 9, lógica, UI dentro de la vista del Ente,
  escena, referencias, compilación runtime/editor, validación automática y
  prueba manual completas. El usuario aprobó 4G y cerró todo el Bloque 4 el
  22 de julio de 2026.

### Eruditos e Investigación

- Máximo un Erudito por zona: Campo, Inscripciones y Sellos.
- Erudito de Campo: 10 Cera + 10 Pan ritual. Erudito de Inscripciones: 20 + 20.
  Coste del Erudito de Sellos todavía pendiente.
- Cada zona progresa 0–100: 20% +5% restos; 40% +5% análisis; 60% abre siguiente;
  80% +10% Conocimiento; 100% bonus especial pendiente.
- Zona 2 exige Zona1 60% + recursos Civ1; Zona3 exige Zona2 60% + recursos Civ1.
- Progreso por calidad aprobado: Baja 1%, Media 3%, Alta 8%. Falta únicamente
  definir los costes de Zona 3 y el bonus especial del 100%.

### Archivo, Indicios y Anomalías

- Archivo I tras primer análisis; II Zona1 40%; III Zona2 40%; IV Zona3 30%.
- Faltan mejoras y costes exactos.
- Indicios comienzan con Zona2 al 20% y quedan habilitados permanentemente.
- Baja/Media/Alta acumulan 3/8/18% de manera determinista; cada 100% entrega un
  Indicio, evitando rachas injustas y conservando excedentes.
- Requisitos: 8 básicos, 10 simbólicos, 12 profundos.
- Leer Anomalía consume Conocimiento y recurso de zona; produce Datos Anómalos.
- Falta decidir si las Anomalías son únicas o repetibles y sus costes exactos;
  la protección contra mala suerte ya quedó resuelta con acumulación.

### Investigación misteriosa y cierre

- Requiere Datos Básicos, Simbólicos y Profundos.
- Hitos 30/60/85/100% consumen recursos de zonas y Datos.
- Recompensas iniciales tentativas de conocimiento permanente: 1/2/3.
- No permitir gastos irreversibles que bloqueen el progreso final.
- Faltan conversión, velocidad, online/offline y bloqueos de hitos.
- Su progreso tardío debe mejorar contenido repetible de D2, no solo acelerar
  sistemas ya terminados.
- Subbloques 4A–4G: Zona1; análisis/Archivo; Zona2/Indicios; Anomalías; Zona3;
  Investigación; integración tardía.

## 9. Bloque 5 completo — Integración final

Objetivo: cerrar los tres arcos y conectar Dimensión 2 con el resto del juego.

La contradicción histórica de beneficios externos quedó resuelta con una matriz
aprobada: cada civilización ofrece tres líneas internas de Dimensión 2 y dos
líneas moderadas para sistemas del juego base; ninguna mejora la Máquina.

- Civ1: Peregrinaciones/Seguidores, Altares/Ofrendas y Acólitos/Ritos;
  externamente LE y Trazas. Ya implementado.
- Civ2: Miembros, operaciones y defensa/contención; externamente Artefactos y
  Triángulo. Pendiente de implementar.
- Civ3: excavación, Archivo/anomalías y mejora amplia repetible de D2;
  externamente Modulador y contribución limitada a Prestigio 1. Pendiente.

Reglas propuestas todavía sujetas a aprobación:

- La revelación abre un sistema progresivo, no un bonus instantáneo.
- Niveles/líneas con requisitos visibles.
- Recursos avanzados mantienen utilidad.
- Beneficios graduales, sin ciclos multiplicativos ilimitados.
- Guardado/migración y offline sin duplicación.

Decisión provisional aprobada para 5A el 22 de julio de 2026:

- Civ1 conserva sin cambios sus cinco líneas actuales y sus números.
- Civ2 tendrá Estabilidad de Contención generada por raíz cuadrada de Miembros
  asignados por minuto, online/offline. Cada línea cuesta 20/40/60 Estabilidad
  y 3/6/9 Fragmentos para niveles 1/2/3.
- Civ2: +5% Miembros; +5% reducción de Dominio; +5% Cobertura y −1 punto de
  pérdidas; +2% Artefactos; +2% Triángulo, todo por nivel según su línea.
- Civ3 añade +5% calibración del Modulador y +1 punto de vista previa de
  Prestigio 1 por nivel a sus tres líneas internas ya implementadas.
- Cinco líneas por civilización, tres niveles, permanentes, sin Máquina y sin
  duplicación offline. Números provisionales para balance posterior.

Implementación de 5B:

- El Lugar de Vínculo ya existente queda formalizado como Pacto Mayor de Civ1.
- No se cambiaron costes, niveles, porcentajes ni la partida guardada.
- La vista muestra `PACTO MAYOR — LUGAR DE VÍNCULO` tras el desbloqueo; la
  navegación cambia de `UMBRAL` a `PACTO` una vez establecido.
- Validación automática 5B: contacto y prerrequisitos reales, pago 100/100/100,
  cinco líneas, asignación de Acólitos, progreso, coste de mejora, conexión con
  LE, guardado/carga y referencias de escena OK.
- Estado técnico: compilación runtime/editor, escena y validación completas;
  prueba visual y funcional manual aprobada.

Implementación de 5C:

- El Pacto Mayor de Civ2 se establece sin coste después de contener al Ente.
- Los Miembros de sostenimiento generan `sqrt(Miembros)` de Estabilidad por
  minuto, online y offline.
- Se implementaron cinco líneas con tres niveles y costes 20/40/60 Estabilidad
  más 3/6/9 Fragmentos de Control.
- Los tres efectos internos mejoran Miembros, reducción de Dominio, Cobertura y
  pérdidas por Represalia; los externos mejoran Artefactos de LE y efectos
  positivos del Triángulo.
- Estado técnico: compilación runtime/editor, escena, referencias, migración,
  offline y validación automática completas; prueba manual aprobada.

Implementación de 5D:

- El Pacto de Civ3 pasa de tres a cinco líneas conservando todo el progreso
  previo.
- Resonancia del Modulador: +5% velocidad de calibración por nivel.
- Crónica del Primer Umbral: +1 punto de vista previa P1 por nivel, reclamable
  solamente mediante el flujo normal de Prestigio.
- Ambas usan los mismos costes 50/100/150 de Conocimiento Antiguo, 25/50/75 de
  cada recurso y umbrales permanentes 1/3/6 del pacto existente.
- La migración añade las dos líneas en nivel 0 sin borrar las tres
  anteriores.
- Estado técnico: compilación, escena, referencias, efectos externos,
  serialización y migración validadas; prueba manual pendiente.

Corrección 4H recuperada de los TXT originales:

- Los Eruditos contratados pasan a nivel 1 y pueden mejorarse hasta nivel 3.
- Cada nivel adicional aporta -5% de duración de análisis y +5% de
  Conocimiento Antiguo/recurso de zona.
- Se añadió una vista propia del Archivo con Cartografía Estratificada,
  Concordancia Anómala y Exégesis Profunda.
- El Conocimiento del Ente funciona como umbral permanente 1/3/6 y no se gasta.
- Las partidas con Eruditos contratados migran a nivel 1 sin perder progreso.
- Civilización 3 usa versión de progreso 11.

Implementación de 5E:

- Las tres civilizaciones avanzan simultáneamente con sus conexiones cruzadas.
- Se comparó el mismo intervalo online y offline para Seguidores, Altares,
  Vínculo, operaciones, Dominio, Amenaza, Miembros, Contención e Investigación.
- La equivalencia fue exacta y el límite offline permanece en 12 horas.
- Los siete efectos externos/comunes de los tres pactos fueron validados juntos.

Implementación funcional de 5F:

- Revisión de acentos, terminología y navegación dinámica.
- Validación de referencias, límites visuales y textos dañados.
- Partida nueva, partida anterior a D2, segunda carga y catálogos finales
  validados.
- El balance numérico definitivo permanece reservado para runs largas; no se
  presenta la configuración provisional como balance cerrado.

Subbloques:

- 5A matriz definitiva de recompensas y conexiones.
- 5B cierre progresivo de Civ 1.
- 5C cierre progresivo de Civ 2.
- 5D cierre progresivo de Civ 3.
- 5E interacciones cruzadas y offline integral.
- 5F cierre funcional, localización, pulido técnico y validación final.

Incluye funcionamiento paralelo, recursos cruzados, localización, revisión
terminológica, arte/pulido, partidas nuevas/antiguas, migraciones y runs sin DEBUG.

## 10. Archivos implementados o modificados

Principales:

- `Assets/Project/Scenes/Main.unity`
- `Assets/Project/Scripts/Core/GameState.cs`
- `Assets/Project/Scripts/Systems/SaveService.cs`
- `Assets/Project/Scripts/UI/TabsUI.cs`
- `Assets/Project/Scripts/Systems/Dimension2State.cs`
- `Assets/Project/Scripts/Systems/Dimension2System.cs`
- `Assets/Project/Scripts/Systems/D2Civilization1System.cs`
- `Assets/Project/Scripts/Systems/D2AltarSystem.cs`
- `Assets/Project/Scripts/Systems/D2PilgrimageSystem.cs`
- `Assets/Project/Scripts/Systems/D2NovitiateSystem.cs`
- `Assets/Project/Scripts/Systems/D2RiteSystem.cs`
- `Assets/Project/Scripts/Systems/D2CivilizationPactSystem.cs`
- `Assets/Project/Scripts/Systems/D2BondSystem.cs`
- `Assets/Project/Scripts/Systems/D2Civilization2System.cs`
- `Assets/Project/Scripts/Systems/D2Civilization3System.cs`
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
- `Assets/Project/Scripts/Editor/Dimension2Block1UISetup.cs`
- `DIMENSION2_DISENO_CANONICO_COMPLETO.md`
- `REGLAS_DE_TRABAJO_Y_CONTINUIDAD.md`

Unity generó los `.meta` de los scripts nuevos. También aparece modificado el
asset dinámico `LiberationSans SDF - Fallback.asset` por los arranques batch de
Unity. No revertir cambios del usuario ni archivos no relacionados.

## 11. Conexiones de Unity realizadas

- Pestaña Dimensión 2.
- Introducción, mapa, tres tarjetas de Civilización y estados de bloqueo.
- Panel Civ1 con navegación REFUGIO, ALTARES, PEREGRINACIONES, NOVICIADO,
  RITOS, PACTOS y UMBRAL.
- Botones, textos, sliders, selector de Altares y referencias serializadas.
- Botones VOLVER AL MAPA y CERRAR.
- Configurador automatizado y validador lógico desde editor.
- No quedan conexiones pendientes de 1 ni 2A–2G.
- 3A tiene panel, navegación, controles y referencias conectadas.
- 3B tiene navegación REGIONES/OPERACIONES, selector y controles conectados.
- 3C tiene navegación DEFENSA, barras, estados y referencias conectadas.
- 3D tiene selector y desbloqueos regionales conectados.
- 3E tiene navegación RED, mejoras, Pactos de Resistencia, desgaste,
  recuperación y penalizaciones conectados.
- 3F tiene navegación ALERTA, temporizador, efectos, marcas regionales y
  desbloqueos conectados.
- 3G tiene navegación CONTENCIÓN, probabilidad, cooldown, resultado y controles
  de sostenimiento conectados.
- 4A tiene entrada funcional desde el mapa, panel de Zona 1, excavación manual,
  inventario de restos, contratación del Erudito de Campo y
  referencias serializadas conectadas.
- 4B tiene tres controles de análisis por calidad, barra de tiempo, recursos,
  progreso de Investigación, estado de Archivo I e hitos conectados.
- 4C tiene selector de zonas, desbloqueo pagado de Zona 2, segundo Erudito,
  recursos separados, Archivo II y contadores/acumulación de Indicios conectados.
- 4D tiene Anomalías, Datos Anómalos, Archivo III, botón de lectura y referencias
  conectadas. 4E tiene Zona 3, desbloqueo, Erudito, Archivo IV y Anomalía
  Profunda conectados. 4F tiene vista propia, navegación, progreso, hitos,
  recursos y controles conectados. 4G reutiliza esa vista al 100% con controles
  para establecer el Pacto, seleccionar sus tres líneas y mejorarlas. El Bloque 5
  todavía no tiene UI funcional completa ni conexiones finales.

## 12. Pruebas y resultados

Automáticas:

- Compilación `Assembly-CSharp` y `Assembly-CSharp-Editor`: cero errores.
- Persisten tres advertencias antiguas `CS0649` de Localization/BuildingDatabase,
  ajenas a D2.
- Validación Unity 2A: producción, asignación, Refugio y serialización OK.
- 2B: cinco Altares, bloqueos, saldos, offline y serialización OK.
- 2C: costes, ocupación, actividad única, cancelación, offline, antirrepetición,
  Confianza y umbrales OK.
- 2D: tandas, conversión, cancelación, mejoras, offline y Peregrinaciones con
  Acólitos OK.
- 2E: catálogo, bloqueo hasta el primer Acólito, asignaciones exclusivas,
  límite de espacios, cinco efectos, límites, tercer espacio, consumo de costes,
  progreso offline y serialización OK.
- 2F: catálogo, desbloqueo, costes, uno/dos espacios, mantenimiento y suspensión
  de Hospedaje, cancelación, cinco beneficios/compromisos, acumuladores
  fraccionales, límites combinados y serialización OK.
- 2G: umbral exacto de 500, bloqueo previo, desbloqueo permanente, migración,
  Altares avanzados, apoyos, Lugar de Vínculo, cinco líneas, beneficios LE/Trazas,
  serialización, panel y referencias de UI OK.
- Validación de límites: Dimensión 2 general, Civilización 1 y sus siete paneles
  tienen sus controles directos dentro de pantalla/panel.
- Validación 3A: paquete inicial de Miembros, cuatro regiones, Región 1,
  asignación exclusiva, conservación del total, migración y serialización OK.
- Validación de límites de Civilización 2: controles directos dentro del panel.
- Validación 3B: cuatro operaciones simultáneas, requisitos, asignación
  exclusiva, conservación de Miembros, Dominio, Amenaza, producción fraccional,
  progreso offline y serialización OK.
- Validación de límites de las vistas REGIONES y OPERACIONES: controles dentro
  de sus paneles.
- Validación 3C: límite y consumo de Cobertura, reducción de Espionaje,
  Represalias, pérdidas enteras, debilitamiento aleatorio, Fragmentos, progreso
  offline y serialización OK.
- Validación de límites de la vista DEFENSA: controles dentro del panel.
- Validación 3D: umbrales 80/60, Dominio inicial 100, repunte del promedio,
  permanencia, selección regional, operación por región, offline y
  serialización OK.
- Validación 3E: mejoras de tres niveles con costes 3/6/9, tres Pactos de
  Resistencia simultáneos, beneficios, refuerzo, desgaste, Miembros agotados,
  recuperación a cinco minutos, incumplimiento, penalizaciones, progreso
  offline, conservación de Miembros y serialización OK.
- Validación de límites de la vista RED: controles dentro del panel.
- Validación 3F: umbral de 30%, Alerta permanente, +50% de Amenaza, seis
  Fragmentos por Represalia, temporizador de diez minutos, marcas regionales
  no acumulables, mitigación por Protección, +3% de pérdidas, consumo de marca,
  desbloqueo de Civilización 3, preparación de Contención, progreso offline y
  serialización OK.
- Validación de límites de la vista ALERTA: controles dentro del panel.
- Validación 3G: tabla interpolada 30/20/10/0, fallo determinista, +20% de
  Amenaza, pérdidas de 5% solo en regiones sin Protección, cooldown offline de
  diez minutos, éxito garantizado a Dominio 0, limpieza y cese de marcas, fin
  del multiplicador de Amenaza, preparación del pacto mayor, asignación/retiro
  de sostenimiento, conservación de Miembros y serialización OK.
- Validación de límites de la vista CONTENCIÓN: controles dentro del panel.
- Validación 4A: catálogo de tres zonas, solo Entrada Sepultada desbloqueada,
  límites exactos 70/25/5, excavación exclusivamente manual, inventario
  ilimitado, progreso online/offline, contratación única del Erudito de Campo,
  consumo de 10 Cera y 10 Pan ritual y serialización OK.
- Validación de límites de la vista de Civilización 3: controles dentro del panel.
- Validación 4B: requisito del Erudito, consumo al iniciar, exclusividad,
  recompensas 1/3/8, recursos 1/2/4, Investigación 1/3/8, hitos 20/40/80,
  requisito visible de 60, Archivo I, progreso offline y serialización OK.
- Validación 4C: requisito de 60%, pago 25/25, calidad 50/35/15, selección,
  excavaciones simultáneas entre zonas, Erudito 20/20, recompensas de Zona 2,
  Archivo II, activación de Indicios al 20%, acumulación 3/8/18, Indicios
  Básicos/Simbólicos, offline y serialización OK.
- Validación 4D: revelado único con 8/10 Indicios, Archivo III al 40% de Zona 2,
  lecturas únicas, costes 25+15 y 40+25, conservación de Indicios, entrega de
  Datos Anómalos, bloqueo Profundo, offline, migración y serialización OK.
- Validación 4E: requisito de 60%, pago 50/50/50, calidad 30/45/25, tres
  excavaciones simultáneas, Erudito 30/30, análisis y Sellos, Archivo IV,
  12 Indicios, Anomalía y Dato Profundos, offline, migración y serialización OK.
- Validación 4F: desbloqueo con tres Datos, conversión 1:1, velocidad 1%/30 s,
  inicio/pausa/espera/reanudación, progreso offline, detención exacta, costes y
  recompensas de 30/60/85/100%, Conocimiento permanente 1+2+3, preparación del
  Pacto, migración y serialización OK.
- Validación 4G: establecimiento sin segundo coste, tres líneas, niveles con
  umbrales 1/3/6, costes 50/100/150 y 25/50/75 de cada recurso, Conocimiento del
  Ente no gastable, bonus de zonas al 100%, efecto real de Memoria Compartida
  sobre Seguidores, migración y serialización OK.
- Las pruebas automáticas guardan JSON temporal y restauran el estado original.

Pruebas manuales del usuario:

- Entrada no repetida, mapa, navegación y CERRAR correctos.
- Refugio y Seguidores correctos.
- Altares, asignaciones, saldos visibles y bloqueos correctos.
- Peregrinaciones, costes, cancelación, finalización y Confianza correctos.
- Corrección narrativa Umbral Velado verificada.
- Noviciado, conversión, cancelación, mejora y Peregrinaciones avanzadas
  revisados; usuario confirmó todo sin errores.
- Bloques 1 y 2A–2D están oficialmente cerrados.
- Ritos, selector, navegación y presentación de 2E revisados en Play Mode; el
  usuario confirmó que todo se ve correctamente.
- Bloques 1 y 2A–2E están oficialmente cerrados.
- Pactos de 2F revisados en Play Mode; el usuario confirmó que se veían bien y
  decidió conservar provisionalmente sus diferencias respecto al original.
- 2F está oficialmente cerrado.
- Umbral de 2G probado con 500 de Confianza mediante DEBUG; el usuario confirmó
  que se habilita y presenta correctamente. Bloque 2 completo oficialmente cerrado.
- Revisión final del 21 de julio de 2026: apoyos, Camino, Noviciado, cinco
  Altares, Lugar de Vínculo, líneas y Puerta Interior comprobados; contador de
  Noviciado y distribución visual corregidos y aprobados por el usuario.
- Prueba manual de 3A aprobada: entrada desde el mapa, diez Miembros, estados
  regionales, asignar/retirar y regreso al mapa correctos.
- Prueba manual de 3B aprobada: navegación a OPERACIONES, selector de las
  cuatro, requisitos, distribución simultánea, avance de Dominio/Amenaza y
  producción de Miembros correctos.
- Prueba manual de 3C aprobada: navegación a DEFENSA, Cobertura, preparación de
  Espionaje, Represalia a 100, pérdidas, Fragmentos y debilitamiento correctos.
- Prueba manual de 3D aprobada: desbloqueo de Regiones 2/3, selector regional,
  repunte del promedio y separación de asignaciones/operaciones/defensa correctos.
- Prueba manual de 3E aprobada: pestaña RED, mejoras, selección y activación de
  Pactos de Resistencia, refuerzo, incumplimiento y presentación general
  correctos. El bloque 3E queda oficialmente cerrado a nivel funcional.
- Los tiempos largos, ritmo de desgaste, recuperación y balance de beneficios
  de 3E se revisarán posteriormente mediante runs prolongadas; no bloquean su
  cierre funcional.
- Prueba manual de 3F aprobada: activación permanente de Alerta al 30%, vista
  ALERTA, marcas regionales, seis Fragmentos, desbloqueo de Civilización 3 y
  preparación de Contención correctos. El bloque 3F queda oficialmente cerrado
  a nivel funcional; tiempos y balance se revisarán en runs prolongadas.
- Prueba manual de 3G aprobada: probabilidad visible, fallo y cooldown,
  Contención garantizada a Dominio 0, preparación del pacto mayor y controles
  de sostenimiento correctos. 3G y el Bloque 3 completo quedan oficialmente
  cerrados a nivel funcional.
- Probabilidades, castigos, ritmos, desgaste y tiempos de todo el Bloque 3
  permanecen provisionales hasta las runs largas de balance.
- Prueba manual de 4A aprobada después de retirar por completo la repetición
  automática; toda automatización queda reservada para Dimensión 3.
- Prueba manual de 4B aprobada: consumo del resto, análisis por calidad,
  recompensas, Investigación y desbloqueo de Archivo I correctos.
- Prueba manual de 4C aprobada: desbloqueo y navegación de Zona 2, actividades
  paralelas, Erudito de Inscripciones, Archivo II y presentación correctos.
- Se comprobó que destapar Zona 2 sí descuenta 25 Incienso y 25 Tela Sagrada.
  En la partida del usuario los saldos superaban 3000, por lo que el cambio de
  25 era poco visible; la validación exacta 25→0 también pasó.
- Archivo II aparece antes de entrar a Zona 2 correctamente: se desbloquea con
  Zona 1 al 40%, mientras Zona 2 exige 60%.
- Prueba manual de 4D aprobada: Archivo III, revelado, costes de lectura,
  conservación de 8/10 Indicios, lectura única, estado archivado y entrega de
  1 Dato Anómalo Básico/Simbólico correctos. La captura del usuario confirmó
  específicamente la Anomalía Simbólica leída, 10 Indicios conservados y 1 Dato.
- Prueba manual de 4E aprobada: pago y apertura de Zona 3, navegación,
  Erudito de Sellos, excavación/análisis, Sellos Antiguos, Archivo IV,
  12 Indicios, lectura única y Dato Anómalo Profundo correctos. La presentación
  visual de Santuario Sellado fue confirmada por el usuario.
- Prueba manual de 4F aprobada: vista propia, progreso, espera, detención en
  30/60/85/100%, costes de recursos y Datos, Conocimiento del Ente 6/6 y
  preparación del Pacto correctos. El usuario confirmó el estado final al 100%
  y el pago del requisito después de reponer los Sellos consumidos en 85%.
- Prueba manual de 4G aprobada: establecimiento del Pacto, presentación de las
  tres líneas, selección, costes, mejora y permanencia del Conocimiento del Ente
  correctos. El usuario confirmó que todo salió bien; 4G y todo el Bloque 4
  quedan oficialmente cerrados a nivel funcional.

## 13. Errores encontrados y correcciones

- Primer validador 2B no inicializaba catálogo tras `ResetState`; se añadió
  `EnsureState` y la repetición validó correctamente.
- Altares no mostraban Seguidores disponibles; se añadió el saldo visible.
- Método UI llamado `Start(string)` activó advertencia del analizador Unity; se
  renombró, quedando limpio.
- Textos tempranos revelaban “Entes”; se sustituyeron por lenguaje misterioso y
  `Umbral Velado`.
- Arranques batch de Unity fueron lentos, pero terminaron sin errores.
- La primera compilación de 2E detectó dos referencias locales incorrectas en
  Altares y Peregrinaciones; se corrigieron y la recompilación quedó limpia.
- La primera comprobación exacta del Rito de Noviciado quedó en el borde de un
  cálculo decimal; se ajustó la ventana de prueba de 338.4 a 339 segundos y el
  comportamiento validó correctamente sin alterar la fórmula jugable.
- Al dividir el progreso de Civilización 2 en pasos de un segundo para simular
  el desgaste de Pactos, una acumulación decimal dejó el promedio unas
  millonésimas sobre 60%. Se añadió una tolerancia interna de 0.000001 para los
  desbloqueos, sin cambiar los umbrales visibles ni el balance; 3D volvió a
  validar correctamente.
- Unity permaneció abierto después de la prueba manual de 2E y bloqueó dos
  intentos batch. El usuario cerró el editor; no se forzó el proceso ni se borró
  el lockfile. Después, compilación, conexiones y validación 2F terminaron bien.
- Durante 4C se revisó una aparente falta de consumo al destapar Zona 2. Código,
  prueba automática exacta y partida guardada confirmaron que el descuento sí
  ocurre; la confusión provino de saldos de más de 3000 frente a un coste de 25.
- Antes de conectar 4D se encontró un proceso de Unity que había quedado abierto
  desde el día anterior. La escena de recuperación creada al cerrarlo solo contenía
  los dos campos nuevos de 4D con referencias nulas; no había cambios manuales ni
  trabajo perdido. `Main.unity` se conservó y después se reconstruyó normalmente.
- La captura final de 4D muestra el botón DEBUG blanco `RESET` parcialmente
  superpuesto con `VOLVER AL MAPA`. Es un detalle de herramientas de desarrollo,
  ajeno a la funcionalidad de 4D, y queda registrado para el pulido final.

## 14. Partida, DEBUG y Git

- El usuario realizó la revisión final modificando temporalmente desde Inspector
  Confianza, población, Noviciado y Ofrendas. Esa partida es válida para pruebas
  funcionales, pero no debe usarse como evidencia de ritmo o balance y puede
  requerir reset cuando comiencen las runs limpias de afinamiento.
- Las validaciones temporales restauraron el estado previo.
- La partida real del usuario sí contiene progreso normal obtenido en sus
  pruebas y offline; no debe borrarse.
- No se realizó commit, rama, staging, push, reset ni otra mutación Git.
- El árbol de trabajo está sucio por el trabajo real y por cambios previos del
  usuario. Preservarlo.

## 15. Pendientes críticos de diseño

- Balance final 2A–2D mediante runs largas.
- Balance definitivo de Ritos y del tercer espacio mediante runs largas; la
  primera configuración provisional ya fue autorizada e implementada.
- Balance definitivo de costes, simultaneidad, mantenimiento y efectos de
  Pactos de Civilización; la primera configuración provisional ya fue aprobada
  e implementada.
- Balance definitivo de apoyos, Altares avanzados, Lugar de Vínculo y sus cinco
  líneas; la primera versión provisional ya fue autorizada e implementada.
- Runs largas de Civ2, Civ3 y los tres pactos mayores.
- Arte y animaciones definitivas, separados del cierre funcional actual.
- Retirar o recolocar el botón DEBUG `RESET` para una build de presentación; no
  afecta la lógica jugable.

## 16. Próximo paso obligatorio

Realizar una sola prueba manual integral de **4H y todo el Bloque 5**. Debe
comprobar la vista del Archivo, mejora de Eruditos, cinco líneas de cada pacto,
navegación, costes, guardado/carga y ausencia de solapamientos. Después se harán
runs largas separadas para balance; todos los números continúan provisionales.

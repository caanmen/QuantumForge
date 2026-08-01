# Resumen de continuidad — Dimensión 3 / Fábrica

Última actualización: 22 de julio de 2026.

Este archivo se actualizará al terminar cada bloque importante. Su propósito es permitir continuar el trabajo en otra tarea de Codex sin depender del historial del chat.

---

## 1. Objetivo general confirmado

Dimensión 3 es la **Fábrica** y será, a largo plazo, la capa encargada de permitir la automatización progresiva del juego completo.

Principios:

- Primero se aprende una acción manualmente y después se automatiza.
- La Fábrica crea piezas y autómatas, administra instalaciones, investigaciones y colas.
- La arquitectura debe admitir conectores para sistemas actuales y futuros.
- Las acciones únicas, narrativas, estratégicas, de riesgo o Prestigio permanecen manuales mientras no exista autorización explícita en el diseño.
- Dimensión 2 / Pactos se integrará más adelante, cuando su diseño completo esté cerrado.

---

## 2. Documentos canónicos guardados

### Diseño funcional

`DIMENSION3_FABRICA_DISENO_CANONICO_COMPLETO.md`

Contiene:

- identidad y alcance;
- piezas V1–V6;
- autómatas MK1–MK6;
- costos y tiempos;
- cuatro colas;
- cinco minijuegos;
- fórmula de afinidad;
- eficiencia decreciente segura;
- asignaciones;
- investigaciones;
- cinco instalaciones;
- automatizaciones actualmente diseñadas;
- reglas de guardado y offline;
- propósito final de automatizar el juego completo.

### Arquitectura de implementación

`DIMENSION3_ESTRUCTURA_IMPLEMENTACION.md`

Contiene:

- capas del sistema;
- archivos y clases propuestos;
- integración con GameState, SaveService, Tick y UI;
- división en seis bloques;
- criterios de aceptación;
- validadores y pruebas;
- orden concreta para comenzar el Bloque 1.

---

## 3. Estado actual

### Terminado

- Comparación del diseño original con las observaciones previas de Codex.
- Corrección de la fórmula imposible del Chasis.
- Definición de reducciones asintóticas para tiempo y costo.
- Definición de producción de piezas, costos, lotes y desbloqueos.
- Definición de colas, cancelaciones, reservas y offline de 12 horas.
- Definición completa de afinidad y Módulo de Control.
- Definición de asignaciones, potencia, Coordinadores e instalaciones.
- Adaptación conceptual a los sistemas reales de Cuarto 2 y Dimensión 1.
- Diseño de arquitectura de código y bloques de implementación.
- Registro del propósito final de automatización del juego completo.

### Implementado — Bloque 1A, núcleo lógico

- `Dimension3State.cs`: estado raíz, inventarios, colas, trabajos, instalaciones y estructuras preparadas para fases posteriores.
- `Dimension3Catalog.cs`: IDs, costos, tiempos, potencia MK, tamaños de lote y límite offline.
- `Dimension3System.cs`: inicialización idempotente, acceso, tick, offline, fachada pública y validación.
- `D3InventorySystem.cs`: piezas, autómatas y conteos de ensamblaje.
- `D3JobQueueSystem.cs`: cuatro colas FIFO, trabajos activos/pendientes, cancelación y finalización consecutiva.
- `D3ProductionSystem.cs`: producción de piezas V1 en lotes 1, 5, 10 y 25.
- `D3AssemblySystem.cs`: ensamblaje de MK1 Normal.
- Integración de `Dimension3State` con GameState, SaveService, carga, migración, reinicio, tick y offline.
- `Dimension3Block1Validation.cs`: validador de Editor para inicialización, producción, ensamblaje, cancelaciones, offline y JSON.
- Archivos `.meta` estables para todos los scripts nuevos.

### Pruebas realizadas

- Compilación de `Assembly-CSharp` y `Assembly-CSharp-Editor`: **0 errores**.
- Tres advertencias existentes de `BuildingDatabase` y `LocalizationManager`, no relacionadas con D3.
- Prueba lógica independiente `D3Smoke`: **PASS**.
- Verificado: MK1 inicial no se duplica, producción V1, cuatro colas, cancelación pendiente con devolución, cancelación activa sin devolución, ensamblaje MK1 y cap offline de 12 horas.
- La ejecución batch del validador quedó impedida por un fallo de inicialización de licencia/Package Manager exclusivo de aquella instancia batch. El Editor normal inició después la licencia y el Package Manager correctamente.
- Validación lógica desde el Editor normal: **PASS**.
- Configuración y validación mínima de la pestaña y el panel de Dimensión 3: **PASS**.

### Estado actual del Bloque 1 — terminado técnicamente

- `Dimension3PanelUI.cs` y `Dimension3Block1UISetup.cs` ya están implementados y compilan.
- `TabsUI` ya contiene la lógica de botón, panel, visibilidad y navegación de Dimensión 3.
- `Tools/Quantum Forge/Dimension 3/Configure Block 1 UI` ya fue ejecutado desde el Editor normal y conectó la pestaña y el panel mínimo.
- Los validadores lógico y de conexiones de UI confirmaron **PASS** en la consola del Editor.
- La prueba de partida completa y el ajuste de experiencia quedan aplazados hasta disponer de la estructura funcional del juego. No bloquean el Bloque 2 porque la lógica, persistencia, compilación y conexiones mínimas ya fueron validadas.

---

## 4. Bloque 2 — terminado técnicamente

Implementado:

- `D3PowerSystem.cs`: potencia por canal, afinidad de rasgo, coordinación, progreso, tiempo, costo y redondeos seguros.
- `D3FacilitySystem.cs`: niveles 1–3 del Banco, canales, asignación agregada y estabilización de 30 segundos.
- Separación de autómatas totales, asignados, reservados y libres.
- Retirada inmediata y prevención de sobreasignación o doble uso.
- Producción V1–V3 y ensamblaje normal MK1–MK3 con desbloqueos canónicos.
- Mejoras del Banco mediante la cola de instalaciones; nivel 2 habilita ritmo y nivel 3 ahorro.
- `Dimension3Block2Validation.cs` para validar todo el núcleo sin recorrer una partida.
- `Dimension3PanelUI.cs` ampliado con selectores, asignaciones, potencia, bonificaciones y mejora del Banco.
- `Main.unity` actualizado mediante el instalador reproducible y referencias verificadas.
- UI reorganizada en tres zonas sin superposición entre Regulador y selector de ensamblaje, con cancelaciones y mejora alineadas.
- Navegación global corregida: Prestigio 1 se muestra antes de realizarlo y se oculta después; Meta permanece oculto hasta su rediseño futuro, sin eliminar sus sistemas internos.

Resultados:

- `Assembly-CSharp` y `Assembly-CSharp-Editor`: **0 errores**.
- Validador Unity de Bloque 2: **PASS**.
- UI de Dimensión 3: **VALIDACIÓN OK**.
- La ejecución batch inició correctamente licencia y Package Manager al mantenerse cerrado el Editor normal.

La revisión visual del Banco de Procesos fue aprobada y el Bloque 3 se implementó a continuación.

---

## 5. Bloque 3 — terminado técnicamente

Implementado:

- `D3CalibrationSystem.cs` con las cinco fórmulas canónicas, validaciones y redondeos.
- Afinidad parcial y final contra Rápido, Eficiente y Coordinador.
- Empates ambiguos de hasta `0,01` y resultado Normal por debajo del 60 %.
- Ensamblaje calibrado con recargo, tiempo, lecturas fijadas y entrega del rasgo resultante.
- Perfiles persistentes con controles exactos y lecturas: guardar N1, cargar N2 y repetir una pieza N3.
- `D3CalibrationPanelUI.cs` y Mesa de Calibración adaptable a los cinco minijuegos.
- Preview, selección MK1–MK3 y confirmación del intento con rasgo.
- `Main.unity` actualizado y referencias verificadas.

Resultados:

- Compilación: **0 errores**.
- Validador Unity del Bloque 3: **PASS**.
- Interfaz de calibración: **VALIDACIÓN OK**.

La Mesa de Calibración fue revisada; se corrigió la superposición superior y se implementó el Bloque 4.

---

## 6. Bloque 4 — terminado técnicamente

- `D3ResearchSystem.cs`: quince investigaciones V4–V6, equipos, potencia, modificadores y reservas.
- Prerrequisitos canónicos por línea; V6 queda preparado y espera Núcleo nivel 4 del Bloque 5.
- Producción V4–V6 y ensamblaje normal/calibrado MK4–MK6.
- Apoyos MK4/MK5 reservados y nunca consumidos.
- Banco niveles 4–5, repetición de cinco piezas, lotes con rasgo hasta cinco y piezas ×50.
- `D3ResearchPanelUI.cs` y selectores extendidos hasta V6/MK6.
- Compilación: **0 errores**; validador: **PASS**; UI: **VALIDACIÓN OK**.

La revisión visual fue aprobada salvo la posición del botón de Investigaciones, que se corrigió y validó. Después se implementó el Bloque 5.

---

## 7. Bloque 5 — terminado técnicamente

- Consola de Producción y Banco de Diagnóstico construibles y ampliables de N1 a N5.
- Costos, tiempos, requisitos, canales, asignaciones, estabilización y capacidad efectiva canónicos.
- `D3FacilitiesPanelUI.cs` y acceso `INSTALACIONES CONECTADAS` desde el Banco.
- Navegación corregida para que Instalaciones y Banco nunca permanezcan visibles a la vez; Volver y Cerrar quedan separados.
- Análisis de nodos convertido en trabajo persistente de `MachineManager`, independiente de `MachinePanelUI` y compatible con guardado/offline.
- `D3DiagnosticSystem.cs`: autoanálisis seguro, autorreparación con reservas, prioridad preparada y evaluación de una acción a la vez.
- Marcas persistentes de recetas; solo se pueden marcar tras una ejecución manual real.
- La Consola expone sus funciones y potencia, pero no ejecuta compras hasta definir cada acción segura en el Catálogo Maestro.
- Compilación: **0 errores**; validador de Bloque 5: **PASS**; UI: **VALIDACIÓN OK**.

El orden canónico deja Puerto de Expedición y Núcleo de Automatización para el Bloque 6 junto con el Catálogo Maestro y la automatización externa/offline.

Siguiente acción: revisión visual breve de la pantalla Instalaciones Conectadas; después, comenzar el Bloque 6 por el Catálogo Maestro.

La pantalla fue aprobada después de corregir que Banco e Instalaciones permanecieran visibles simultáneamente.

---

## 8. Bloque 6A — Catálogo Maestro y seguridad D1 terminados

- `DIMENSION3_CATALOGO_MAESTRO_AUTOMATIZACIONES.md`: catálogo acción por acción con estados, reservas, parada, requisito manual, offline y API.
- `D3AutomationCatalog.cs`: 18 acciones auditadas y diez destinos normales clasificados explícitamente.
- Exclusión programática de Ark, cadenas, misiones coordinadas, naves, planetas, reliquias, árbol y destinos antiguos.
- Historial persistente de barrido manual, destinos completados manualmente y mejoras manuales de extractores.
- Marcación de exploraciones iniciadas por automatización para no satisfacer falsamente el requisito manual.
- APIs D1 separadas para automatización y exploración por `destinationId` estable.
- Compilación: **0 errores**; validador del Catálogo: **PASS**.

El subbloque 6B descrito a continuación ya fue terminado.

---

## 8B. Bloque 6B — Puerto y Núcleo terminados

- Puerto de Expediciones y Núcleo de Automatización normalizados y persistentes.
- Construcción N1–N5 con costes, tiempos, ensamblajes y requisitos de progreso reales.
- Puerto con canales de Capacidad, Respuesta y Coordinación.
- Núcleo con Coordinación y preferencia por autómatas Coordinadores.
- Capacidad calculada exclusivamente con asignaciones estabilizadas.
- Núcleo activo: límites de 2/3/4/5 rutinas, perfiles en N2/N4, +5%/+10% de eficiencia y permiso offline externo en N5.
- Historial D1 migrable de exploraciones simples totales para el requisito del Puerto N3.
- Pantalla `INSTALACIONES CONECTADAS` ampliada con las dos instalaciones nuevas.
- Compilación principal y Editor: **0 errores**.
- Configuración de `Main.unity`: **VALIDACIÓN OK**.
- Validador de instalaciones 6B: **PASS** para requisitos, construcción, canales, estabilización, capacidad, bonificaciones y JSON.

El subbloque 6C descrito a continuación ya fue terminado.

---

## 8C. Bloque 6C — Motor online de rutinas y perfiles terminado

- Rutinas persistentes con acción, objetivos, orden, prioridad, reservas, parada, repeticiones, temporizador y resultado.
- Perfiles del Núcleo con copia completa y restaurable de sus rutinas.
- Motor online integrado al `Tick`, con prioridad estable y una transacción externa máxima por ciclo.
- Pausa, reactivación, eliminación y parada automática.
- Reservas absolutas de LE, Trazas y metal antes y después de pagar.
- Requisito manual validado al crear, activar y ejecutar.
- Cinco acciones autorizadas del Puerto conectadas a las API reales de D1.
- Límites activos del Puerto/Núcleo y respuesta asintótica.
- Nueva pantalla `RUTINAS Y PERFILES ONLINE` accesible desde Puerto o Núcleo.
- Compilación principal y Editor: **0 errores**.
- Configuración de `Main.unity`: **VALIDACIÓN OK**.
- Validador 6C: **PASS** para manual, prioridad, una transacción, reservas, parada, perfiles y JSON.
- Decisiones únicas y offline externo permanecen bloqueados.

La pantalla fue revisada y aprobada. El subbloque 6D descrito a continuación ya fue terminado.

---

## 8D. Bloque 6D — Automatización externa offline terminada

- Requiere Puerto N5 y Núcleo N5 activos con capacidad suficiente al comenzar la ausencia.
- Evalúa rutinas en bloques exactos de 60 segundos, hasta 12 horas.
- Un resto menor de 60 segundos avanza D1 sin producir otra acción automatizada.
- Mantiene una transacción externa máxima por bloque.
- Conserva prioridades, reservas, historial manual, paradas y exclusiones del motor online.
- D1 y D3 se simulan intercalados para impedir efectos retroactivos de escaneos, exploraciones o mejoras.
- `SaveService` evita duplicar minería D1 durante la simulación combinada.
- Indicador visible de disponibilidad offline en la pantalla de rutinas.
- Compilación: **0 errores**; validador 6D: **PASS**.
- Regresión completa: Bloque 1, Catálogo, Instalaciones, Online y Offline: **todos PASS**.

La auditoría integral fue completada en `DIMENSION3_AUDITORIA_CIERRE_FUNCIONAL.md`.

---

## 8E. Auditoría de cierre — resultado

- Los bloques 1–6 son estables, pero no cubren aún todo el diseño canónico.
- Brecha de núcleo: la velocidad de trabajos está fijada al confirmar y debe responder dinámicamente a las asignaciones.
- Consola: construcción y canales completos; automatizaciones N1–N5 pendientes.
- Diagnóstico: N1–N2 online completos; N3–N5 incompletos.
- Perfiles, coordinación y eficiencia general del Núcleo son parciales.
- Catálogo C# incompleto frente al Catálogo Maestro.
- UI pendiente: lotes, previsiones, cantidades, cancelación individual y localización.
- Arte, balance, D2 y Prestigio siguen aplazados deliberadamente.

Siguiente acción: **Bloque 7A**, conformidad del núcleo y UI operativa. Luego 7B Consola, 7C Diagnóstico y 7D cierre de perfiles/Núcleo/localización.

---

## 8F. Bloque 7A — Conformidad del núcleo y UI operativa terminado

- Los costos continúan fijándose al confirmar, pero los trabajos nuevos guardan trabajo base y recalculan su velocidad durante el progreso.
- Cambiar asignaciones del Banco acelera o desacelera de inmediato Producción, Ensamblaje, Investigación e Instalaciones.
- Los trabajos guardados por versiones anteriores conservan su duración ya calculada y no reciben el modificador dos veces.
- Producción expone lotes 1, 5, 10, 25 y 50; el lote 50 requiere Banco N5.
- Ensamblaje normal expone cantidades 1, 5, 10 y 25.
- La fábrica muestra piezas faltantes, costo de producirlas, costo de ensamblaje, total y tiempo estimado con la potencia actual.
- Nueva pantalla `GESTIÓN DE COLAS`: permite elegir cualquiera de las cuatro colas, seleccionar cualquier trabajo y cancelarlo.
- Los pendientes devuelven recursos y piezas reservadas; los activos requieren doble confirmación y no devuelven consumibles.
- Banco de Diagnóstico N2 exige realmente `NodeAnalysisUnlocked`.
- Versión de progreso D3 aumentada a 7 con compatibilidad de guardado anterior.
- Compilación principal y Editor: **0 errores**.
- Configuración de `Main.unity`: **VALIDACIÓN OK**.
- Validador 7A: **PASS**.
- Regresión transversal Bloques 1, 2, 3, 4, 5, 6A, 6B, 6C, 6D y 7A: **todos PASS**.

Siguiente acción exacta: abrir Unity y revisar visualmente la fábrica y la nueva pantalla `GESTIÓN DE COLAS`. Si la distribución está bien, continuar con **Bloque 7B — Consola de Producción**.

---

## 8G. Bloque 7B — Consola de Producción terminado

- Historial manual persistente para compras de Higgs/Tetra, fases del Modulador y configuraciones básicas del Triángulo.
- N1 compra automáticamente únicamente Condensador de Higgs y Núcleo Tetraquark previamente comprados a mano.
- N2 guarda política LE, Trazas o Equilibrio y reservas mínimas persistentes de LE/Trazas.
- Disciplina de recursos reduce solamente compras repetibles ejecutadas por la Consola; no cambia compras manuales ni sistemas externos.
- N3 permanece bloqueado de forma explícita como `PendingDesign`; no se inventó una lista de mejoras.
- N4 mantiene únicamente una fase desbloqueada y elegida manualmente antes.
- N5 registra y aplica únicamente configuraciones básicas del Triángulo usadas manualmente.
- Las cuatro acciones ejecutables están integradas al Catálogo, prioridades, parada, perfiles y motor online/offline.
- El offline de Consola exige Consola N5 con capacidad activa y Núcleo N5 activo.
- Los perfiles guardan/restauran política, reservas y presets sin borrar historial manual posterior.
- Nueva pantalla `CONTROL DE CONSOLA`, accesible desde Instalaciones; las rutinas de compra se administran en `RUTINAS Y PERFILES ONLINE`.
- Versión de progreso D3 aumentada a 8.
- Compilación: **0 errores**; UI: **VALIDACIÓN OK**; validador 7B: **PASS**.
- Regresión Bloques 1–7B: **todos PASS**.

Siguiente acción exacta: revisar visualmente `CONTROL DE CONSOLA` y las nuevas acciones de `RUTINAS Y PERFILES ONLINE`. Después continuar con **Bloque 7C — Banco de Diagnóstico completo**.

---

## 8H. Bloque 7C — Banco de Diagnóstico completo terminado

- N3 incorpora prioridad global o por zona y reservas persistentes de LE/Trazas.
- N4 repite únicamente recetas de fusión ejecutadas manualmente y marcadas por el jugador.
- La fusión automática usa un servicio independiente de `Room2PanelUI`, consume los fragmentos reales y conserva las reglas y recompensas normales del modo seguro.
- Continúan prohibidas la recolección automática de fragmentos y las decisiones de nodos únicos, ocultos o sin `tierGroup`.
- N5 guarda y carga una rutina diagnóstica completa: estados automáticos, prioridad, zona, reservas y recetas marcadas.
- Autoanálisis, autorreparación y autofusión offline exigen simultáneamente Diagnóstico N5 y Núcleo N5.
- Nueva pantalla `CONTROL DE DIAGNÓSTICO`, separada de la pantalla general de instalaciones.
- Progreso D3 actualizado a versión 9 durante 7C.
- Validador 7C: **PASS**.

## 8I. Bloque 7D — Núcleo, perfiles y cierre terminado

- El multiplicador del Núcleo (+5 % / +10 %) se aplica coherentemente a bonus autorizados de otras instalaciones, sin multiplicar su propia capacidad ni recompensas externas.
- Los perfiles completos guardan y restauran rutinas, Consola, Diagnóstico, recetas marcadas y calibraciones; el historial manual obtenido después no se elimina.
- El catálogo C# representa acciones internas de Fábrica, acciones autorizadas, pendientes y prohibidas del Catálogo Maestro.
- `repetir última ruta` consulta el último destino simple completado manualmente, ya no el objetivo escrito en la rutina.
- Se mantiene la regla segura aprobada: una sola transacción externa máxima por evaluación global; ampliar simultaneidad requiere una decisión futura de diseño.
- Capa de localización EN/ES añadida a toda la interfaz D3, incluidos textos dinámicos.
- Migración tolerante para estados anteriores sin campos de Consola, Diagnóstico, perfiles o recetas.
- Progreso D3 final del Bloque 7 actualizado a versión 10.
- Compilación: **0 errores**; escena: **VALIDACIÓN OK**; validadores 7C y 7D: **PASS**.
- Regresión final Bloques 1–7D: **todos PASS** (`Logs/dimension3_full_through_7d_final.log`).

Siguiente acción exacta: revisar visualmente `CONTROL DE DIAGNÓSTICO` y el cambio EN/ES. Después diseñar y ejecutar la reorganización progresiva de paneles del juego completo; luego pruebas de jugabilidad de recorrido completo y balance numérico.

Corrección posterior a 7D: la capa EN/ES dejó de aplicar traducciones inversas sobre su propia salida. Cada texto conserva ahora una fuente española limpia, por lo que cambios repetidos ES→EN→ES no acumulan sufijos como `CANCELARARAR…`. También limpia ese patrón si ya apareció en la sesión. Compilación del Editor: correcta.

---

## 9. Método de trabajo acordado

No se implementará toda la dimensión de una sola vez.

Para cada bloque:

1. Implementar lógica y persistencia.
2. Compilar.
3. Ejecutar validaciones automatizadas.
4. Corregir errores.
5. Probar el flujo mínimo en Unity.
6. Actualizar este resumen.
7. Comenzar el bloque siguiente solamente cuando el anterior esté estable.

La UI definitiva, el arte y las animaciones se harán después de validar los sistemas funcionales correspondientes.

Las pruebas de cada bloque serán principalmente técnicas y automáticas. El usuario solo tendrá que revisar la UI o realizar interacciones breves cuando sea realmente necesario. La partida completa, la sensación de progreso y el afinado de números se realizarán cuando esté construida la estructura funcional del juego; esto no permite aplazar errores de compilación, persistencia, cálculos o integración.

Para evitar errores transitorios durante la implementación, Unity se cerrará antes de bloques con varios scripts interdependientes. Primero se completará y compilará externamente el conjunto, y después se abrirá el Editor para las validaciones visuales. No se ejecutará Unity batch mientras el Editor normal esté abierto.

---

## 10. Pendiente de diseño futuro

El **Catálogo Maestro de Automatizaciones** de la primera versión quedó creado y sincronizado con el código al cerrar 7D.

Las siguientes familias continúan fuera del catálogo ejecutable y deberán auditarse cuando su diseño se amplíe:

- producción base;
- compra de edificios;
- investigaciones generales;
- mejoras repetibles anteriores a Prestigio 1;
- acciones restantes de Cuarto 1;
- acciones restantes de Cuarto 2;
- Prestigio y meta-progresión, si se autoriza;
- contenido completo de Dimensión 1;
- Dimensión 2 cuando quede cerrada;
- sistemas futuros.

Para cada acción se decidirá:

- qué se automatiza exactamente;
- instalación responsable;
- requisito manual previo;
- nivel y potencia;
- reservas y prioridades;
- condición de parada;
- funcionamiento online/offline;
- clasificación como repetible, estratégica o única;
- API de código que ejecutará la acción.

Este pendiente no bloquea la estructura terminada de Dimensión 3: cualquier acción no clasificada como autorizada permanece sin ruta automática.

---

## 11. Regla para próximos resúmenes

Cuando el usuario solicite «el resumen», se debe responder usando este archivo y actualizarlo con:

- último bloque terminado;
- archivos creados o modificados;
- pruebas realizadas y resultado;
- decisiones nuevas;
- problemas conocidos;
- siguiente bloque exacto;
- pendientes de diseño y de implementación.

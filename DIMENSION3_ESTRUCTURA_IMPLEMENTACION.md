# Dimensión 3 — Estructura para implementación

Documento complementario a `DIMENSION3_FABRICA_DISENO_CANONICO_COMPLETO.md`.

Objetivo: convertir el diseño canónico en una arquitectura de código compatible con el proyecto Unity actual, con bloques pequeños, persistentes y verificables.

La arquitectura debe permitir que Dimensión 3 termine automatizando los procesos repetibles del juego completo mediante conectores independientes. El núcleo no debe conocer detalles internos de todos los sistemas.

---

## 1. Principio de implementación

No se debe programar toda la Fábrica en un único bloque. El primer bloque debe entregar un ciclo vertical completo:

1. Entrar a Dimensión 3.
2. Ver el Banco de Procesos.
3. Fabricar piezas V1.
4. Esperar o aplicar progreso offline.
5. Recibir las piezas automáticamente.
6. Ensamblar un MK1 Normal.
7. Guardar, cerrar, cargar y conservar todo el estado.

Cuando ese ciclo sea estable se incorporan asignaciones, rasgos, investigaciones e instalaciones externas.

---

## 2. Capas del sistema

```text
UI de Dimensión 3
        ↓
API pública de Dimension3System
        ↓
Sistemas especializados
        ↓
Dimension3State persistente
        ↓
GameState / SaveService / TickSystem
```

La UI nunca debe modificar listas, recursos o temporizadores directamente. Debe llamar métodos públicos como `TryQueuePartProduction`, `TryQueueAssembly`, `TryAssignAutomatons` o `TryCancelJob`.

Las tablas de costos y requisitos no deben almacenarse dentro de la UI ni duplicarse entre clases.

---

## 3. Archivos de estado persistente

### `Assets/Project/Scripts/Systems/Dimension3State.cs`

Contendrá únicamente clases serializables y datos guardables.

Clases propuestas:

```text
Dimension3State
D3PartStackState
D3AutomatonStackState
D3AssignmentState
D3ReservedAutomatonState
D3JobState
D3QueueState
D3ResearchState
D3FacilityState
D3CalibrationReadingState
D3CalibrationProfileState
D3AutomationRoutineState
D3AutomationProfileState
```

### Estado raíz mínimo

`Dimension3State` debe incluir:

```text
progressVersion
initialized
firstEntrySeen
selectedInstallationId
parts
automatons
assignments
queues
research
facilities
calibrationProfiles
automationRoutines
automationProfiles
totalAssembledByMk
```

### Reglas de modelado

- Usar IDs de texto estables para piezas, rasgos, instalaciones, canales, trabajos e investigaciones.
- Usar enteros para versiones, MK, niveles y cantidades.
- Usar `double` para costos, progreso y segundos.
- Inicializar todas las listas para tolerar partidas antiguas y JSON incompleto.
- No guardar valores calculados como bonus finales o potencia efectiva; deben recalcularse.
- Guardar recursos reservados dentro del trabajo para poder cancelar con exactitud.
- Guardar lecturas de calibración como datos de la entrada de ensamblaje, no en las piezas.

IDs iniciales recomendados:

```text
Partes: chassis, motor, tool, control, regulator
Rasgos: normal, fast, efficient, coordinator
Instalaciones: process_bank, production_console, diagnostic_bank,
               expedition_port, automation_core
Colas: part_production, assembly, research, facility
```

---

## 4. Catálogo de definiciones

### `Assets/Project/Scripts/Systems/Dimension3Catalog.cs`

Será la fuente única de verdad para los valores del documento canónico:

- potencia MK1–MK6;
- costos y tiempos de piezas V1–V6;
- costos y tiempos de ensamblaje;
- recargo de intentos con rasgo;
- patrones de afinidad;
- costos y requisitos de investigaciones;
- costos y requisitos de instalaciones;
- factores de canales;
- umbrales de capacidad;
- límite offline y tamaño máximo de colas.

Debe exponer definiciones inmutables o copias de solo lectura. La primera implementación puede usar clases C# y tablas estáticas, siguiendo el patrón actual de Dimensiones 1 y 2. No es necesario introducir ScriptableObjects antes de validar el sistema.

Definiciones propuestas:

```text
D3PartDefinition
D3AssemblyDefinition
D3ResearchDefinition
D3FacilityLevelDefinition
D3TraitPatternDefinition
```

El catálogo también valida IDs, versiones y MK. Ningún sistema debe confiar en un ID proveniente de la UI o del archivo de guardado sin comprobarlo.

---

## 5. Sistema coordinador

### `Assets/Project/Scripts/Systems/Dimension3System.cs`

Responsabilidades:

- `CreateInitialState()`.
- `EnsureState(GameState)` y migraciones.
- `ResetState(GameState)`.
- `CanAccessDimension3(GameState)`.
- `MarkFirstEntrySeen(GameState)`.
- `Tick(GameState, double dt)`.
- `ApplyOfflineProgress(GameState, double seconds)`.
- `ValidateState(GameState, out string result)`.
- coordinar los sistemas especializados en un orden estable.

Orden recomendado por tick:

1. Normalizar y validar estado básico.
2. Actualizar estabilización de asignaciones.
3. Recalcular potencia y modificadores.
4. Avanzar las cuatro colas.
5. Depositar resultados terminados.
6. Evaluar desbloqueos.
7. Evaluar automatizaciones autorizadas.

`Dimension3System` será la fachada pública principal, pero no debe contener toda la lógica interna.

---

## 6. Sistemas especializados

### `D3InventorySystem.cs`

- Consultar, añadir y consumir piezas.
- Consultar, añadir, reservar y liberar autómatas.
- Calcular autómatas disponibles.
- Impedir cantidades negativas o duplicación de reservas.

### `D3JobQueueSystem.cs`

- Administrar las cuatro colas FIFO.
- Máximo de diez entradas por cola.
- Cobrar/reservar al confirmar.
- Iniciar, avanzar, completar y cancelar trabajos.
- Aplicar progreso sobrante al siguiente trabajo.
- Compartir exactamente la misma simulación entre tick online y offline.

No deben existir dos implementaciones distintas de progreso. `Tick` y `ApplyOfflineProgress` deben llamar a la misma función interna con diferente cantidad de segundos.

### `D3PowerSystem.cs`

- Potencia base por MK.
- +25 % por rasgo en canal correcto.
- raíz cuadrada de potencia total.
- coordinación y multiplicador de grupo.
- bonus de progreso.
- reducciones asintóticas de tiempo y costo.
- redondeos finales.

Todas las fórmulas matemáticas deben permanecer aquí para evitar diferencias entre previews y resultados reales.

### `D3ProductionSystem.cs`

- Validar versión y desbloqueo de piezas.
- Calcular costo y duración de lotes.
- Crear trabajos de producción.
- Entregar resultados al inventario.

### `D3AssemblySystem.cs`

- Validar las cinco piezas de la misma versión.
- Validar MK desbloqueado.
- Calcular costo normal o con rasgo.
- Reservar apoyos MK4/MK5 para MK5/MK6.
- Crear trabajo de ensamblaje.
- Entregar el autómata correcto al finalizar.

### `D3CalibrationSystem.cs`

- Calcular las cinco lecturas.
- Calcular afinidad provisional y final.
- Resolver empates.
- Determinar el rasgo resultante.
- Guardar y validar perfiles descubiertos.

Este sistema debe ser matemático y no depender de componentes visuales de los minijuegos.

### `D3ResearchSystem.cs`

- Validar prerrequisitos V4–V6.
- Calcular potencia mínima y modificadores.
- Reservar autómatas investigadores.
- Completar investigaciones por línea.
- Desbloquear piezas y MK correspondientes.

### `D3FacilitySystem.cs`

- Construcción y niveles.
- Canales y asignaciones.
- estabilización de 30 segundos.
- capacidad efectiva.
- activación o pausa de funciones por potencia.
- modificador del Núcleo sin recursión.

### Sistemas posteriores de automatización

```text
D3AutomationSystem.cs
D3Room1AutomationSystem.cs
D3MachineAutomationSystem.cs
D3ExpeditionAutomationSystem.cs
```

Los conectores externos solo deben añadirse después de estabilizar el núcleo de la Fábrica. Cada conector llama APIs públicas del sistema que automatiza; no debe imitar clics de UI.

Antes del Bloque 6 se añadirá:

```text
D3AutomationCatalog.cs
D3AutomationActionDefinition
D3AutomationExecutionResult
```

Cada acción automatizable deberá registrarse con ID, propietario, nivel, potencia, prerrequisito manual, reservas, compatibilidad offline y clasificación de seguridad. De esta manera los sistemas futuros podrán añadirse sin reescribir `Dimension3System`.

---

## 7. Integraciones con archivos existentes

### `GameState.cs`

Añadir:

```csharp
public Dimension3State dimension3 = new Dimension3State();
```

También:

- `EnsureDimension3State()`.
- llamada en `Start()`.
- `Dimension3System.Tick(this, dt)` dentro del tick general.
- inicialización al ejecutar `UnlockDimensionSystemAfterPrestige1()`.
- reinicio dentro de `ResetDimensionSystemState()`.

### `SaveService.cs`

Añadir `Dimension3State dimension3` a `SaveData` y conectar:

- guardado;
- carga con fallback `CreateInitialState()`;
- migración de partidas antiguas;
- reinicio total;
- `Dimension3System.ApplyOfflineProgress(...)` después de cargar.

El autómata MK1 inicial solo se concede cuando `initialized` cambia de falso a verdadero. Nunca debe concederse nuevamente por una migración o por abrir la pestaña.

### `TabsUI.cs` y `TabManager.cs`

- Añadir botón o panel de Dimensión 3.
- Visibilidad condicionada por `dimension03Unlocked`.
- Mantener el mismo comportamiento de cierre y retorno que Dimensiones 1 y 2.

### Localización

Añadir claves a:

```text
Assets/Project/Resources/Localization/lang_es.json
Assets/Project/Resources/Localization/lang_en.json
```

No se debe dejar texto funcional nuevo solo en español dentro de scripts si la pantalla ya participa del sistema de localización.

---

## 8. Estructura de UI

### Panel raíz

`Assets/Project/Scripts/UI/Dimension3PanelUI.cs`

Vistas principales:

```text
firstEntryRoot
factoryMapRoot
processBankRoot
productionConsoleRoot
diagnosticBankRoot
expeditionPortRoot
automationCoreRoot
```

La primera versión solo necesita `firstEntryRoot`, `factoryMapRoot` y `processBankRoot`.

### Paneles especializados

```text
D3FactoryMapPanelUI.cs
D3ProcessBankPanelUI.cs
D3PartProductionPanelUI.cs
D3AssemblyPanelUI.cs
D3QueuePanelUI.cs
D3AutomatonInventoryPanelUI.cs
D3AssignmentPanelUI.cs
D3ResearchPanelUI.cs
D3CalibrationPanelUI.cs
D3FacilityUpgradePanelUI.cs
D3AutomationPanelUI.cs
```

No es necesario crear todos en el primer bloque. Cada panel consulta el sistema y refresca su estado; no conserva la verdad del juego.

Los cinco minijuegos pueden usar subpaneles separados, pero todos entregan su resultado a `D3CalibrationSystem`:

```text
D3ChassisCalibrationUI.cs
D3MotorCalibrationUI.cs
D3ToolCalibrationUI.cs
D3ControlCalibrationUI.cs
D3RegulatorCalibrationUI.cs
```

---

## 9. Bloques de codificación

### Bloque 1 — Núcleo persistente y ciclo MK1

Crear:

- `Dimension3State.cs`.
- `Dimension3Catalog.cs` con datos V1 y MK1, aunque deje preparadas las tablas completas.
- `Dimension3System.cs`.
- `D3InventorySystem.cs`.
- `D3JobQueueSystem.cs`.
- `D3ProductionSystem.cs`.
- parte normal de `D3AssemblySystem.cs`.
- integración con GameState, SaveService y tick.
- UI mínima de entrada, inventario, producción, ensamblaje y colas.

Criterios de aceptación:

- La primera entrada concede exactamente un MK1 Normal.
- Se puede producir cada pieza V1 en lotes.
- Se puede ensamblar un MK1 Normal.
- Las cuatro colas existen, aunque solo dos tengan trabajos disponibles.
- Cancelar pendiente devuelve todo.
- Cancelar activo no devuelve consumibles.
- Guardar/cargar conserva inventarios, trabajo activo y cola.
- El progreso offline completa trabajos hasta 12 horas.
- Una partida antigua no duplica el MK1 inicial.

### Bloque 2 — Asignaciones y Banco de Procesos

Crear:

- `D3PowerSystem.cs`.
- `D3FacilitySystem.cs` básico.
- canales del Banco.
- asignación agregada por MK/rasgo.
- estabilización de 30 segundos.
- niveles 1–3 y desbloqueos V2/V3.

Criterios:

- No se puede asignar más de lo disponible.
- Un autómata no aparece simultáneamente libre y asignado.
- Los previews y los trabajos usan las mismas fórmulas.
- Tiempo y costo nunca llegan a cero o valores negativos.

### Bloque 3 — Calibración y rasgos

Crear los cinco minijuegos, `D3CalibrationSystem` y ensamblaje avanzado.

Criterios:

- Chasis puede alcanzar todo el rango 0–100.
- Los patrones producen afinidades reproducibles.
- Empates generan Normal.
- Fallar el 60 % consume el intento y entrega Normal.
- Repetir una pieza antes de confirmar no consume recursos.
- Guardar/cargar conserva perfiles descubiertos.

### Bloque 4 — Investigación y MK4–MK6

Crear `D3ResearchSystem`, cola de investigación completa, niveles 4–5 del Banco y apoyos de ensamblaje.

Criterios:

- Cada línea V4–V6 respeta prerrequisitos.
- Los investigadores y apoyos quedan reservados.
- Cancelar siempre libera autómatas reservados.
- MK5 y MK6 no consumen sus cinco apoyos.

### Bloque 5 — Instalaciones conectadas

Construir Consola, Banco de Diagnóstico, Puerto y Núcleo, inicialmente con UI y niveles aunque sus automatizaciones se habiliten por subbloques.

Antes de activar Banco de Diagnóstico:

- mover el análisis de nodos desde `MachinePanelUI` a estado persistente;
- exponer API de análisis/reparación en `MachineManager`;
- añadir marcas persistentes de recetas.

Antes de activar Puerto:

- definir en datos qué destinos son repetibles;
- añadir clasificación de riesgo;
- exponer APIs seguras de reenvío y extractores.

### Bloque 6 — Automatizaciones y offline externo

Primero completar el Catálogo Maestro de Automatizaciones revisando todos los sistemas reales del proyecto. Después crear rutinas, reservas, prioridades, perfiles, simultaneidad y automatización offline del Núcleo nivel 5.

Cada automatización debe tener pruebas que demuestren que no toca decisiones únicas, nodos clave, blueprints especiales ni misiones de cadena.

---

## 10. Validación y pruebas

Seguir el patrón de validadores de Editor ya presente en el proyecto.

Archivos sugeridos:

```text
Assets/Project/Scripts/Editor/Dimension3Block1Validation.cs
Assets/Project/Scripts/Editor/Dimension3Block2Validation.cs
Assets/Project/Scripts/Editor/Dimension3CalibrationValidation.cs
Assets/Project/Scripts/Editor/Dimension3AutomationValidation.cs
```

Pruebas mínimas transversales:

- estado inicial idempotente;
- migración desde save sin `dimension3`;
- inventarios nunca negativos;
- reservas conservan cantidades;
- orden FIFO;
- cancelación pendiente y activa;
- exceso de segundos pasa al siguiente trabajo;
- equivalencia online/offline;
- cap offline de 12 horas;
- afinidad y redondeos;
- reducción asintótica con potencia extrema;
- asignación y liberación de apoyos;
- instalaciones pausadas al perder capacidad;
- automatizaciones respetan reservas y exclusiones.

Cada bloque debe compilar y pasar su validador antes de modificar escenas o comenzar el siguiente.

---

## 11. Primera orden concreta para Codex

La primera solicitud de implementación debería limitarse a:

> Implementar el Bloque 1 de `DIMENSION3_ESTRUCTURA_IMPLEMENTACION.md` usando `DIMENSION3_FABRICA_DISENO_CANONICO_COMPLETO.md` como fuente de diseño. Crear el estado persistente, catálogo, inventario, colas, producción V1, ensamblaje MK1 Normal, integración con GameState/SaveService/Tick y validaciones. No implementar todavía rasgos, minijuegos, investigaciones, instalaciones externas ni la UI visual definitiva. Conservar compatibilidad con partidas antiguas y verificar guardado, carga y progreso offline de 12 horas.

Esta delimitación evita que Codex mezcle en el mismo cambio el núcleo económico, cinco minijuegos, cuatro integraciones externas y la construcción visual completa.

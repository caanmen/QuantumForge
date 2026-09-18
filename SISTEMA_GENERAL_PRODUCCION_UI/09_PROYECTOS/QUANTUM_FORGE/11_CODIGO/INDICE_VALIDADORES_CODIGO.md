# Índice de validadores de código de Quantum Forge

Este índice permite elegir cobertura sin leer los 109 validadores ni ejecutarlos todos.
Los nombres reflejan la línea base del 2026-09-18; confirmar existencia y entrada real antes
de ejecutar.

## Regla de selección

Elegir primero el validador del sistema modificado, después una regresión vecina y sólo al
cerrar una integración o release ampliar la cobertura. Revisar si la entrada es menú,
`ValidateBatch`, `SessionState` o Play Mode antes de lanzar Unity.

Nunca ejecutar dos instancias de Unity sobre la misma ruta del proyecto.

## Persistencia, recuperación y ciclo de vida

- `SaveRecoveryValidation.cs`: escritura, respaldo, recuperación y schema futuro.
- `LoadedProgressReplacementValidation.cs`: reemplazo del progreso cargado sin residuos.
- `DimensionDiscoveryMigrationValidation.cs`: migración del descubrimiento dimensional.
- `F2ProgressionMigrationValidation.cs`: migración de progresión F2.
- `AndroidPauseResumeValidation.cs`: pausa y reanudación.
- `AndroidOfflineResumeBudgetValidation.cs`: presupuesto de reanudación offline.
- `ReleaseOfflineIntegrationValidation.cs`: integración offline de release.
- `ReleaseRecoveryRuntimeValidation.cs`: recuperación real en Play Mode y UI de error.
- `ReleaseLifecycleValidation.cs`: ciclo de vida integrado.

Cuando cambie persistencia, combinar una prueba focalizada con partida nueva, partida
anterior y reapertura o copia independiente según el riesgo.

## Convergencia

- `ConvergenceStateValidation.cs`
- `ConvergenceC1ToC3Validation.cs`
- `ConvergenceC4C5Validation.cs`
- `ConvergenceEndpointValidation.cs`
- `ConvergenceSynchronizationValidation.cs`
- `ConvergenceStartupPulseValidation.cs`
- `ConvergenceTelemetryValidation.cs`
- `ConvergenceFullValidation.cs`
- `ConvergenceAuditCorrectionValidation.cs`

`ConvergenceFullValidation` es una cobertura amplia; no sustituye las pruebas focalizadas
durante una corrección pequeña.

## Dimensión 1

Buscar `Dimension1*Validation.cs`. Validadores representativos:

- `Dimension1FinalValidation.cs`
- `Dimension1RelicEffectsValidation.cs`
- `Dimension1SimultaneousResultsValidation.cs`
- `Dimension1ExploreHangarFunctionalValidation.cs`
- `Dimension1ExploreSectorFilteringValidation.cs`
- `Dimension1TreeNavigationRuntimeValidation.cs`
- `Dimension1CommandCenterDrawerRuntimeValidation.cs`
- `Dimension1ArkRouteValidation.cs`

Seleccionar por exploración, sectores, reliquias, árbol, Ark, navegación o resultado; no
ejecutar toda la familia por rutina.

## Dimensión 2

Buscar `Dimension2*Validation.cs` y los validadores `D2*`. Ampliar con validaciones de
release únicamente cuando cambien temporización, progreso offline, navegación compartida o
contratos entre dimensiones.

## Dimensión 3

Buscar `Dimension3*Validation.cs`. La familia incluye bloques 1 a 7, automatización,
offline, colas, investigación, instalaciones, producción, calibración, diagnóstico y
consola. Elegir la unidad afectada y una regresión de integración cuando corresponda.

## QA y release

- `QaBlock0Validation.cs` a `QaBlock5Validation.cs`
- `QaGlobalAccelerationValidation.cs`
- `QaMainSceneIntegrityValidation.cs`
- `QaResetValidation.cs`
- `GameplayCorrectionsValidation.cs`
- `ReleaseD1D3RegressionValidation.cs`
- `ReleaseJourneyValidation.cs`
- `SpanishReleaseReadinessValidation.cs`

Las validaciones de release no se usan como sustituto de una prueba dirigida ni se ejecutan
por defecto después de cada cambio pequeño.

## UI y visual

La cobertura visual permanece en `08_PRUEBAS`, las fichas de pantalla y los validadores de
UI/Editor correspondientes. Una comprobación estructural no reemplaza captura nueva,
comparación a resolución objetivo ni aprobación humana cuando cambie el resultado visible.

## Búsqueda reproducible

Para obtener la lista vigente sin abrir archivos:

```powershell
rg --files Assets/Project/Scripts/Editor -g "*Validation.cs"
```

Después buscar por sistema o síntoma y leer sólo los candidatos. Si cambia una entrada,
salida o requisito importante, actualizar este índice.

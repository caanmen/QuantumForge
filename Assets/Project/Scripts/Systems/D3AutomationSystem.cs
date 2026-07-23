using System;
using System.Collections.Generic;

public static class D3AutomationSystem
{
    public static bool TryCreateRoutine(
        GameState gameState, string actionId, string targetId,
        IList<string> orderedTargetIds, int priority, double leReserve,
        double tracesReserve, string resourceReserveId,
        double resourceReserveAmount, int stopAfterExecutions,
        out D3AutomationRoutineState routine, out string reason)
    {
        routine = null;
        reason = "";
        if (!Dimension3System.CanAccessDimension3(gameState))
        {
            reason = "Dimension 3 esta bloqueada.";
            return false;
        }
        Dimension3System.EnsureState(gameState);
        D3AutomationActionDefinition definition =
            D3AutomationCatalog.GetAction(actionId);
        if (definition == null ||
            definition.status != D3AutomationCatalogStatus.Authorized)
        {
            reason = "La accion no esta autorizada por el Catalogo Maestro.";
            return false;
        }
        if (D3FacilitySystem.GetFacilityLevel(
                gameState.dimension3, definition.facilityId) <
            definition.requiredFacilityLevel)
        {
            reason = "La instalacion no posee el nivel requerido.";
            return false;
        }
        if (!HasManualAuthorization(
                gameState, actionId, targetId, orderedTargetIds))
        {
            reason = "Falta la ejecucion manual previa requerida.";
            return false;
        }
        if (leReserve < 0.0 || tracesReserve < 0.0 ||
            resourceReserveAmount < 0.0 || stopAfterExecutions < 0)
        {
            reason = "Reservas o condicion de parada invalidas.";
            return false;
        }

        routine = new D3AutomationRoutineState
        {
            routineId = "automation_" +
                gameState.dimension3.nextAutomationSequence++,
            actionId = actionId,
            enabled = false,
            priority = priority,
            leReserve = leReserve,
            tracesReserve = tracesReserve,
            resourceReserveId = resourceReserveId ?? "",
            resourceReserveAmount = resourceReserveAmount,
            targetId = targetId ?? "",
            stopAfterExecutions = stopAfterExecutions,
            executionsCompleted = 0,
            lastResult = "Creada; pausada.",
            evaluationRemainingSeconds = 0.0
        };
        CopyTargets(orderedTargetIds, routine.orderedTargetIds);
        gameState.dimension3.automationRoutines.Add(routine);
        return true;
    }

    public static bool TrySetRoutineEnabled(
        GameState gameState, string routineId, bool enabled, out string reason)
    {
        reason = "";
        if (!Dimension3System.CanAccessDimension3(gameState))
        {
            reason = "Dimension 3 esta bloqueada.";
            return false;
        }
        Dimension3System.EnsureState(gameState);
        D3AutomationRoutineState routine = GetRoutine(
            gameState.dimension3, routineId);
        if (routine == null)
        {
            reason = "Rutina inexistente.";
            return false;
        }
        if (!enabled)
        {
            routine.enabled = false;
            routine.lastResult = "Pausada por el jugador.";
            return true;
        }
        D3AutomationActionDefinition definition =
            D3AutomationCatalog.GetAction(routine.actionId);
        if (definition == null ||
            definition.status != D3AutomationCatalogStatus.Authorized ||
            !D3FacilitySystem.IsFunctionActive(
                gameState.dimension3, definition.facilityId,
                definition.requiredFacilityLevel))
        {
            reason = "La funcion requerida no esta activa por nivel o capacidad.";
            return false;
        }
        if (!HasManualAuthorization(gameState, routine.actionId,
                routine.targetId, routine.orderedTargetIds))
        {
            reason = "La autorizacion manual ya no es valida.";
            return false;
        }
        int enabledCount = CountEnabled(gameState.dimension3, routineId);
        if (enabledCount >= GetRoutineLimit(gameState.dimension3))
        {
            reason = "Se alcanzo el limite de rutinas activas.";
            return false;
        }
        if (HasReachedStop(routine))
        {
            reason = "La condicion de parada ya fue alcanzada.";
            return false;
        }
        routine.enabled = true;
        routine.evaluationRemainingSeconds = 0.0;
        routine.lastResult = "Activa; esperando evaluacion.";
        return true;
    }

    public static bool TryDeleteRoutine(
        Dimension3State state, string routineId, out string reason)
    {
        reason = "";
        D3AutomationRoutineState routine = GetRoutine(state, routineId);
        if (routine == null)
        {
            reason = "Rutina inexistente.";
            return false;
        }
        state.automationRoutines.Remove(routine);
        return true;
    }

    public static D3AutomationRoutineState GetRoutine(
        Dimension3State state, string routineId)
    {
        if (state == null || state.automationRoutines == null) return null;
        for (int i = 0; i < state.automationRoutines.Count; i++)
        {
            D3AutomationRoutineState routine = state.automationRoutines[i];
            if (routine != null && routine.routineId == routineId) return routine;
        }
        return null;
    }

    public static int GetRoutineLimit(Dimension3State state)
    {
        int coreLevel = D3FacilitySystem.GetFacilityLevel(
            state, Dimension3Catalog.FacilityAutomationCore);
        if (coreLevel > 0 && D3FacilitySystem.IsFunctionActive(
                state, Dimension3Catalog.FacilityAutomationCore, 1))
            return D3FacilitySystem.GetAutomationCoreRoutineLimit(state);
        return D3FacilitySystem.IsFunctionActive(
            state, Dimension3Catalog.FacilityExpeditionPort, 5) ? 2 : 1;
    }

    public static int CountEnabled(Dimension3State state, string exceptRoutineId = "")
    {
        if (state == null || state.automationRoutines == null) return 0;
        int count = 0;
        for (int i = 0; i < state.automationRoutines.Count; i++)
        {
            D3AutomationRoutineState routine = state.automationRoutines[i];
            if (routine != null && routine.enabled &&
                routine.routineId != exceptRoutineId) count++;
        }
        return count;
    }

    public static bool TrySaveProfile(
        GameState gameState, string profileId, string displayName,
        out string reason)
    {
        reason = "";
        if (!Dimension3System.CanAccessDimension3(gameState))
        {
            reason = "Dimension 3 esta bloqueada.";
            return false;
        }
        Dimension3System.EnsureState(gameState);
        int limit = D3FacilitySystem.GetAutomationCoreProfileLimit(
            gameState.dimension3);
        if (limit <= 0)
        {
            reason = "El Nucleo activo no permite perfiles.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(profileId))
        {
            reason = "ID de perfil invalido.";
            return false;
        }
        D3AutomationProfileState profile = GetProfile(
            gameState.dimension3, profileId);
        if (profile == null)
        {
            if (gameState.dimension3.automationProfiles.Count >= limit)
            {
                reason = "Se alcanzo el limite de perfiles.";
                return false;
            }
            profile = new D3AutomationProfileState { profileId = profileId };
            gameState.dimension3.automationProfiles.Add(profile);
        }
        profile.displayName = displayName ?? "";
        profile.snapshotVersion = 1;
        profile.routineIds.Clear();
        profile.savedRoutines.Clear();
        profile.savedConsoleSettings = D3ConsoleSystem.CloneSettings(
            gameState.dimension3.consoleSettings);
        profile.savedDiagnosticSettings = CloneDiagnosticSettings(
            gameState.dimension3.diagnosticSettings);
        profile.savedMarkedFusionRecipes.Clear();
        for (int i = 0; i < gameState.dimension3.markedFusionRecipes.Count; i++)
        {
            D3MarkedFusionRecipeState recipe = CloneMarkedRecipe(
                gameState.dimension3.markedFusionRecipes[i]);
            if (recipe != null) profile.savedMarkedFusionRecipes.Add(recipe);
        }
        profile.savedCalibrationProfiles.Clear();
        for (int i = 0; i < gameState.dimension3.calibrationProfiles.Count; i++)
        {
            D3CalibrationProfileState calibration = CloneCalibrationProfile(
                gameState.dimension3.calibrationProfiles[i]);
            if (calibration != null)
                profile.savedCalibrationProfiles.Add(calibration);
        }
        for (int i = 0; i < gameState.dimension3.automationRoutines.Count; i++)
        {
            D3AutomationRoutineState source =
                gameState.dimension3.automationRoutines[i];
            if (source == null) continue;
            profile.routineIds.Add(source.routineId);
            profile.savedRoutines.Add(CloneRoutine(source));
        }
        return true;
    }

    public static bool TryLoadProfile(
        GameState gameState, string profileId, out string reason)
    {
        reason = "";
        if (!Dimension3System.CanAccessDimension3(gameState))
        {
            reason = "Dimension 3 esta bloqueada.";
            return false;
        }
        Dimension3System.EnsureState(gameState);
        if (D3FacilitySystem.GetAutomationCoreProfileLimit(
                gameState.dimension3) <= 0)
        {
            reason = "El Nucleo activo no permite cargar perfiles.";
            return false;
        }
        D3AutomationProfileState profile = GetProfile(
            gameState.dimension3, profileId);
        if (profile == null || profile.savedRoutines == null)
        {
            reason = "Perfil inexistente o antiguo sin configuracion guardada.";
            return false;
        }
        gameState.dimension3.automationRoutines.Clear();
        if (profile.savedConsoleSettings != null)
        {
            D3ConsoleSettingsState currentHistory =
                gameState.dimension3.consoleSettings;
            D3ConsoleSettingsState loadedSettings = D3ConsoleSystem.CloneSettings(
                profile.savedConsoleSettings);
            D3ConsoleSystem.MergeManualHistory(loadedSettings, currentHistory);
            gameState.dimension3.consoleSettings = loadedSettings;
        }
        if (profile.snapshotVersion >= 1 &&
            profile.savedDiagnosticSettings != null)
        {
            gameState.dimension3.diagnosticSettings = CloneDiagnosticSettings(
                profile.savedDiagnosticSettings);
            D3DiagnosticSystem.NormalizeSettings(
                gameState.dimension3.diagnosticSettings);
        }
        if (profile.snapshotVersion >= 1)
            MergeProfileFusionRecipes(gameState.dimension3,
                profile.savedMarkedFusionRecipes);
        if (profile.snapshotVersion >= 1 &&
            profile.savedCalibrationProfiles != null)
        {
            gameState.dimension3.calibrationProfiles.Clear();
            for (int i = 0; i < profile.savedCalibrationProfiles.Count; i++)
            {
                D3CalibrationProfileState calibration = CloneCalibrationProfile(
                    profile.savedCalibrationProfiles[i]);
                if (calibration != null)
                    gameState.dimension3.calibrationProfiles.Add(calibration);
            }
        }
        int enabledLimit = GetRoutineLimit(gameState.dimension3);
        int enabled = 0;
        for (int i = 0; i < profile.savedRoutines.Count; i++)
        {
            D3AutomationRoutineState copy = CloneRoutine(profile.savedRoutines[i]);
            if (copy == null) continue;
            if (copy.enabled && enabled++ >= enabledLimit)
                copy.enabled = false;
            copy.evaluationRemainingSeconds = 0.0;
            gameState.dimension3.automationRoutines.Add(copy);
        }
        return true;
    }

    public static void TickOnline(GameState gameState, double dt)
    {
        Tick(gameState, dt, false);
    }

    public static bool CanRunExternalOffline(GameState gameState)
    {
        if (!Dimension3System.CanAccessDimension3(gameState) ||
            gameState == null || !gameState.dimension01Unlocked) return false;
        Dimension3System.EnsureState(gameState);
        return D3FacilitySystem.IsFunctionActive(gameState.dimension3,
                   Dimension3Catalog.FacilityExpeditionPort, 5) &&
               D3FacilitySystem.CanRunExternalAutomationOffline(
                   gameState.dimension3);
    }

    public static bool CanRunAutomationOffline(GameState gameState)
    {
        if (!Dimension3System.CanAccessDimension3(gameState)) return false;
        Dimension3System.EnsureState(gameState);
        if (!D3FacilitySystem.CanRunExternalAutomationOffline(gameState.dimension3))
            return false;
        for (int i = 0; i < gameState.dimension3.automationRoutines.Count; i++)
        {
            D3AutomationRoutineState routine = gameState.dimension3.automationRoutines[i];
            D3AutomationActionDefinition definition = routine == null ? null :
                D3AutomationCatalog.GetAction(routine.actionId);
            if (routine != null && routine.enabled && definition != null &&
                definition.status == D3AutomationCatalogStatus.Authorized &&
                definition.offlineAllowedWithCore5 &&
                D3FacilitySystem.IsFunctionActive(gameState.dimension3,
                    definition.facilityId, 5)) return true;
        }
        return false;
    }

    public static double ApplyOfflineExternal(
        GameState gameState, double offlineSeconds)
    {
        if (!CanRunAutomationOffline(gameState) || offlineSeconds <= 0.0 ||
            double.IsNaN(offlineSeconds) || double.IsInfinity(offlineSeconds))
            return 0.0;
        double applied = Math.Min(offlineSeconds,
            Dimension3Catalog.OfflineProgressCapSeconds);
        int fullBlocks = (int)Math.Floor(applied / 60.0);
        for (int i = 0; i < fullBlocks; i++)
        {
            Dimension1System.Tick(gameState, 60.0);
            Tick(gameState, 60.0, true);
        }
        double remainder = applied - fullBlocks * 60.0;
        if (remainder > 0.0)
            Dimension1System.Tick(gameState, remainder);
        return applied;
    }

    private static void Tick(GameState gameState, double dt, bool offline)
    {
        if (gameState == null || gameState.dimension3 == null || dt <= 0.0)
            return;
        List<D3AutomationRoutineState> ordered = GetOrderedActiveRoutines(
            gameState.dimension3);
        int activeLimit = Math.Min(GetRoutineLimit(gameState.dimension3),
            ordered.Count);
        bool transactionExecuted = false;
        for (int i = 0; i < activeLimit; i++)
        {
            D3AutomationRoutineState routine = ordered[i];
            D3AutomationActionDefinition definition =
                D3AutomationCatalog.GetAction(routine.actionId);
            if (offline && (definition == null ||
                !definition.offlineAllowedWithCore5 ||
                !D3FacilitySystem.IsFunctionActive(gameState.dimension3,
                    definition.facilityId, 5) ||
                !D3FacilitySystem.CanRunExternalAutomationOffline(
                    gameState.dimension3)))
            {
                routine.lastResult = "Espera: accion no autorizada offline.";
                continue;
            }
            routine.evaluationRemainingSeconds -= dt;
            if (routine.evaluationRemainingSeconds > 0.0) continue;
            routine.evaluationRemainingSeconds = GetEvaluationInterval(
                gameState.dimension3, definition);
            if (HasReachedStop(routine))
            {
                routine.enabled = false;
                routine.lastResult = "Condicion de parada alcanzada.";
                continue;
            }
            if (transactionExecuted)
            {
                routine.lastResult = "Espera: otra rutina actuo en este ciclo.";
                continue;
            }
            if (!HasReserves(gameState, routine))
            {
                routine.lastResult = "Espera: reservas minimas.";
                continue;
            }
            if (TryExecute(gameState, routine, out string result))
            {
                transactionExecuted = true;
                routine.executionsCompleted++;
                routine.lastResult = result;
                if (HasReachedStop(routine))
                {
                    routine.enabled = false;
                    routine.lastResult += " Parada alcanzada.";
                }
            }
            else routine.lastResult = result;
        }
    }

    private static bool TryExecute(
        GameState gameState, D3AutomationRoutineState routine,
        out string result)
    {
        result = "Espera: la accion no puede ejecutarse ahora.";
        D3AutomationActionDefinition definition =
            D3AutomationCatalog.GetAction(routine.actionId);
        if (definition == null ||
            definition.status != D3AutomationCatalogStatus.Authorized ||
            !D3FacilitySystem.IsFunctionActive(gameState.dimension3,
                definition.facilityId, definition.requiredFacilityLevel) ||
            !HasManualAuthorization(gameState, routine.actionId,
                routine.targetId, routine.orderedTargetIds)) return false;

        if (routine.actionId == D3AutomationCatalog.ActionPortScan)
        {
            if (!Dimension1System.TryScanSimpleDestinationAutomated(gameState))
                return false;
            result = "Barrido simple iniciado.";
            return true;
        }
        if (routine.actionId == D3AutomationCatalog.ActionPortRepeatLast)
            return TryStartRoute(gameState,
                gameState.dimension1LastManualSimpleDestinationId,
                "Última ruta manual iniciada.", out result);
        if (routine.actionId == D3AutomationCatalog.ActionPortPriorityRoutes)
        {
            for (int i = 0; i < routine.orderedTargetIds.Count; i++)
                if (TryStartRoute(gameState, routine.orderedTargetIds[i],
                        "Ruta prioritaria iniciada.", out result)) return true;
            return false;
        }
        if (routine.actionId == D3AutomationCatalog.ActionPortSafeRoute)
        {
            for (int i = 0; i < D3AutomationCatalog.Destinations.Length; i++)
            {
                string id = D3AutomationCatalog.Destinations[i].destinationId;
                if (gameState.HasManualD1SimpleDestination(id) &&
                    TryStartRoute(gameState, id,
                        "Ruta segura iniciada.", out result)) return true;
            }
            return false;
        }
        if (routine.actionId == D3AutomationCatalog.ActionPortExtractor)
        {
            D1PlanetState planet = FindPlanet(gameState, routine.targetId);
            if (planet == null) return false;
            string metal = Dimension1System.GetExtractorUpgradeMainCostMetal(planet);
            double cost = Dimension1System.GetExtractorUpgradeCost(gameState, planet);
            double requiredReserve = routine.resourceReserveId == metal
                ? routine.resourceReserveAmount : 0.0;
            if (gameState.GetD1MetalAmount(metal) + 0.000001 <
                cost + requiredReserve)
            {
                result = "Espera: reserva de metal del extractor.";
                return false;
            }
            if (!Dimension1System.TryUpgradeExtractorAutomated(
                    gameState, routine.targetId)) return false;
            result = "Extractor mejorado automaticamente.";
            return true;
        }
        if (routine.actionId == D3AutomationCatalog.ActionConsoleBuyHiggs ||
            routine.actionId == D3AutomationCatalog.ActionConsoleBuyTetraquark)
        {
            string buildingId = routine.actionId ==
                D3AutomationCatalog.ActionConsoleBuyHiggs
                ? D3ConsoleSystem.BuildingHiggs
                : D3ConsoleSystem.BuildingTetraquark;
            if (!D3ConsoleSystem.TryPurchaseRepeatableBuilding(
                    gameState, buildingId, routine.leReserve,
                    routine.tracesReserve, out result)) return false;
            return true;
        }
        if (routine.actionId == D3AutomationCatalog.ActionConsoleModulator)
        {
            if (!int.TryParse(routine.targetId, out int rawMode) ||
                !Enum.IsDefined(typeof(PhaseModulatorMode), rawMode)) return false;
            PhaseModulatorMode mode = (PhaseModulatorMode)rawMode;
            if (gameState.phaseModulatorMode == mode)
            {
                result = "Fase preferida ya activa.";
                return false;
            }
            return D3ConsoleSystem.TryApplyPreferredPhase(gameState, mode, out result);
        }
        if (routine.actionId == D3AutomationCatalog.ActionConsoleTriangle)
        {
            D3TrianglePresetState preset = D3ConsoleSystem.GetPreset(
                gameState.dimension3.consoleSettings, routine.targetId);
            if (D3ConsoleSystem.IsTrianglePresetApplied(gameState, preset))
            {
                result = "Configuración básica ya aplicada.";
                return false;
            }
            return D3ConsoleSystem.TryApplyTrianglePreset(
                gameState, routine.targetId, out result);
        }
        return false;
    }

    private static bool TryStartRoute(
        GameState gameState, string destinationId, string success,
        out string result)
    {
        result = "Espera: ruta no disponible.";
        if (!gameState.HasManualD1SimpleDestination(destinationId) ||
            !D3AutomationCatalog.IsRepeatableSafeDestination(destinationId) ||
            !Dimension1System.TryStartExplorationByDestinationId(
                gameState, Dimension1System.ShipLightProbe,
                destinationId, true)) return false;
        result = success;
        return true;
    }

    private static bool HasManualAuthorization(
        GameState gameState, string actionId, string targetId,
        IList<string> orderedTargetIds)
    {
        if (actionId == D3AutomationCatalog.ActionPortScan)
            return gameState.dimension1ManualSimpleScanCompleted;
        if (actionId == D3AutomationCatalog.ActionPortRepeatLast)
            return gameState.HasManualD1SimpleDestination(
                gameState.dimension1LastManualSimpleDestinationId);
        if (actionId == D3AutomationCatalog.ActionPortPriorityRoutes)
        {
            if (orderedTargetIds == null || orderedTargetIds.Count == 0)
                return false;
            for (int i = 0; i < orderedTargetIds.Count; i++)
                if (!gameState.HasManualD1SimpleDestination(
                        orderedTargetIds[i])) return false;
            return true;
        }
        if (actionId == D3AutomationCatalog.ActionPortSafeRoute)
        {
            for (int i = 0; i < gameState.dimension1ManualSimpleDestinationIds.Count;
                 i++)
                if (D3AutomationCatalog.IsRepeatableSafeDestination(
                        gameState.dimension1ManualSimpleDestinationIds[i]))
                    return true;
            return false;
        }
        if (actionId == D3AutomationCatalog.ActionPortExtractor)
            return gameState.HasManualD1ExtractorUpgrade(targetId);
        if (actionId == D3AutomationCatalog.ActionConsoleBuyHiggs)
            return D3ConsoleSystem.HasManualBuildingAuthorization(
                gameState.dimension3, D3ConsoleSystem.BuildingHiggs);
        if (actionId == D3AutomationCatalog.ActionConsoleBuyTetraquark)
            return D3ConsoleSystem.HasManualBuildingAuthorization(
                gameState.dimension3, D3ConsoleSystem.BuildingTetraquark);
        if (actionId == D3AutomationCatalog.ActionConsoleModulator &&
            int.TryParse(targetId, out int mode))
            return Enum.IsDefined(typeof(PhaseModulatorMode), mode) &&
                D3ConsoleSystem.HasManualPhaseAuthorization(
                    gameState, (PhaseModulatorMode)mode);
        if (actionId == D3AutomationCatalog.ActionConsoleTriangle)
            return D3ConsoleSystem.GetPreset(
                gameState.dimension3.consoleSettings, targetId) != null;
        return false;
    }

    private static bool HasReserves(
        GameState gameState, D3AutomationRoutineState routine)
    {
        if (gameState.LE + 0.000001 < routine.leReserve ||
            gameState.Traces + 0.000001 < routine.tracesReserve) return false;
        if (!string.IsNullOrWhiteSpace(routine.resourceReserveId) &&
            gameState.GetD1MetalAmount(routine.resourceReserveId) + 0.000001 <
            routine.resourceReserveAmount) return false;
        return true;
    }

    private static bool HasReachedStop(D3AutomationRoutineState routine)
    {
        return routine.stopAfterExecutions > 0 &&
               routine.executionsCompleted >= routine.stopAfterExecutions;
    }

    private static double GetEvaluationInterval(
        Dimension3State state, D3AutomationActionDefinition definition)
    {
        if (definition == null) return 1.0;
        string responseChannel = definition.facilityId ==
            Dimension3Catalog.FacilityProductionConsole
            ? Dimension3Catalog.ChannelConsoleResponse
            : definition.facilityId == Dimension3Catalog.FacilityDiagnosticBank
                ? Dimension3Catalog.ChannelDiagnosticResponse
                : Dimension3Catalog.ChannelPortResponse;
        double raw = D3PowerSystem.GetRawChannelPower(
            state, definition.facilityId, responseChannel);
        double coordinationRaw = D3PowerSystem.GetRawChannelPower(
            state, definition.facilityId,
            D3FacilitySystem.GetCoordinationChannel(definition.facilityId));
        double coordination = 1.0 + Math.Sqrt(coordinationRaw) * 0.25 / 100.0;
        double bonus = Math.Sqrt(raw) * 0.15 * coordination *
            D3FacilitySystem.GetAutomationCoreEfficiencyMultiplier(state);
        return Math.Max(0.2, 1.0 / (1.0 + bonus / 100.0));
    }

    private static List<D3AutomationRoutineState> GetOrderedActiveRoutines(
        Dimension3State state)
    {
        var ordered = new List<D3AutomationRoutineState>();
        for (int i = 0; i < state.automationRoutines.Count; i++)
        {
            D3AutomationRoutineState routine = state.automationRoutines[i];
            if (routine != null && routine.enabled) ordered.Add(routine);
        }
        ordered.Sort((a, b) =>
        {
            int priority = b.priority.CompareTo(a.priority);
            if (priority != 0) return priority;
            return state.automationRoutines.IndexOf(a).CompareTo(
                state.automationRoutines.IndexOf(b));
        });
        return ordered;
    }

    private static D3AutomationProfileState GetProfile(
        Dimension3State state, string profileId)
    {
        if (state == null || state.automationProfiles == null) return null;
        for (int i = 0; i < state.automationProfiles.Count; i++)
        {
            D3AutomationProfileState profile = state.automationProfiles[i];
            if (profile != null && profile.profileId == profileId) return profile;
        }
        return null;
    }

    private static D3AutomationRoutineState CloneRoutine(
        D3AutomationRoutineState source)
    {
        if (source == null) return null;
        var clone = new D3AutomationRoutineState
        {
            routineId = source.routineId,
            actionId = source.actionId,
            enabled = source.enabled,
            priority = source.priority,
            leReserve = source.leReserve,
            tracesReserve = source.tracesReserve,
            resourceReserveId = source.resourceReserveId ?? "",
            resourceReserveAmount = source.resourceReserveAmount,
            targetId = source.targetId ?? "",
            stopAfterExecutions = source.stopAfterExecutions,
            executionsCompleted = source.executionsCompleted,
            lastResult = source.lastResult ?? "",
            evaluationRemainingSeconds = source.evaluationRemainingSeconds
        };
        CopyTargets(source.orderedTargetIds, clone.orderedTargetIds);
        return clone;
    }

    private static D3DiagnosticSettingsState CloneDiagnosticSettings(
        D3DiagnosticSettingsState source)
    {
        if (source == null) return new D3DiagnosticSettingsState();
        D3DiagnosticSettingsState clone =
            UnityEngine.JsonUtility.FromJson<D3DiagnosticSettingsState>(
                UnityEngine.JsonUtility.ToJson(source));
        if (clone == null) clone = new D3DiagnosticSettingsState();
        D3DiagnosticSystem.NormalizeSettings(clone);
        return clone;
    }

    private static D3MarkedFusionRecipeState CloneMarkedRecipe(
        D3MarkedFusionRecipeState source)
    {
        if (source == null) return null;
        return new D3MarkedFusionRecipeState
        {
            recipeId = source.recipeId ?? "",
            manuallyExecuted = source.manuallyExecuted,
            automationMarked = source.automationMarked
        };
    }

    private static D3CalibrationProfileState CloneCalibrationProfile(
        D3CalibrationProfileState source)
    {
        if (source == null) return null;
        return UnityEngine.JsonUtility.FromJson<D3CalibrationProfileState>(
            UnityEngine.JsonUtility.ToJson(source));
    }

    private static void MergeProfileFusionRecipes(
        Dimension3State state, IList<D3MarkedFusionRecipeState> saved)
    {
        if (state == null) return;
        var current = new List<D3MarkedFusionRecipeState>();
        if (state.markedFusionRecipes != null)
            for (int i = 0; i < state.markedFusionRecipes.Count; i++)
            {
                D3MarkedFusionRecipeState clone = CloneMarkedRecipe(
                    state.markedFusionRecipes[i]);
                if (clone != null) current.Add(clone);
            }
        state.markedFusionRecipes = new List<D3MarkedFusionRecipeState>();
        if (saved != null)
            for (int i = 0; i < saved.Count; i++)
            {
                D3MarkedFusionRecipeState clone = CloneMarkedRecipe(saved[i]);
                if (clone != null && clone.manuallyExecuted &&
                    D3DiagnosticSystem.FindMarkedRecipe(state, clone.recipeId) == null)
                    state.markedFusionRecipes.Add(clone);
            }
        for (int i = 0; i < current.Count; i++)
        {
            D3MarkedFusionRecipeState existing =
                D3DiagnosticSystem.FindMarkedRecipe(state, current[i].recipeId);
            if (existing == null)
            {
                current[i].automationMarked = false;
                state.markedFusionRecipes.Add(current[i]);
            }
            else existing.manuallyExecuted |= current[i].manuallyExecuted;
        }
    }

    private static void CopyTargets(
        IList<string> source, List<string> target)
    {
        target.Clear();
        if (source == null) return;
        for (int i = 0; i < source.Count; i++)
        {
            string id = source[i];
            if (!string.IsNullOrWhiteSpace(id) && !target.Contains(id))
                target.Add(id);
        }
    }

    private static D1PlanetState FindPlanet(
        GameState gameState, string planetId)
    {
        if (gameState == null || gameState.dimension1Planets == null) return null;
        for (int i = 0; i < gameState.dimension1Planets.Count; i++)
        {
            D1PlanetState planet = gameState.dimension1Planets[i];
            if (planet != null && planet.planetId == planetId) return planet;
        }
        return null;
    }
}

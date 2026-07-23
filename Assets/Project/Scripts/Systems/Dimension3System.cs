using System;
using System.Collections.Generic;


public static class Dimension3System
{
    public const int ProgressVersion = 10;

    public static Dimension3State CreateInitialState()
    {
        Dimension3State state = new Dimension3State
        {
            progressVersion = ProgressVersion,
            initialized = false,
            firstEntrySeen = false,
            selectedInstallationId = Dimension3Catalog.FacilityProcessBank,
            nextJobSequence = 1L
        };

        NormalizeState(state);
        return state;
    }

    public static bool CanAccessDimension3(GameState gameState)
    {
        return gameState != null && gameState.dimension03Unlocked;
    }

    public static void EnsureState(GameState gameState)
    {
        if (gameState == null)
            return;

        if (gameState.dimension3 == null)
            gameState.dimension3 = CreateInitialState();

        NormalizeState(gameState.dimension3);

        if (gameState.dimension03Unlocked && !gameState.dimension3.initialized)
            InitializeUnlockedState(gameState.dimension3);
    }

    public static void ResetState(GameState gameState)
    {
        if (gameState == null)
            return;

        gameState.dimension3 = CreateInitialState();
    }

    public static void MarkFirstEntrySeen(GameState gameState)
    {
        if (!CanAccessDimension3(gameState))
            return;

        EnsureState(gameState);
        gameState.dimension3.firstEntrySeen = true;
    }

    public static void Tick(GameState gameState, double dt)
    {
        if (!CanAccessDimension3(gameState) || dt <= 0.0 ||
            double.IsNaN(dt) || double.IsInfinity(dt))
        {
            return;
        }

        EnsureState(gameState);
        D3FacilitySystem.AdvanceStabilization(gameState.dimension3, dt);
        D3JobQueueSystem.AdvanceAllQueues(gameState, dt);
        D3DiagnosticSystem.Tick(gameState, dt, false);
        D3AutomationSystem.TickOnline(gameState, dt);
    }

    public static double ApplyOfflineProgress(GameState gameState, double offlineSeconds)
    {
        if (!CanAccessDimension3(gameState) || offlineSeconds <= 0.0 ||
            double.IsNaN(offlineSeconds) || double.IsInfinity(offlineSeconds))
        {
            return 0.0;
        }

        EnsureState(gameState);
        bool externalOfflineAuthorized =
            D3AutomationSystem.CanRunAutomationOffline(gameState);
        double applied = Math.Min(
            offlineSeconds,
            Dimension3Catalog.OfflineProgressCapSeconds
        );
        D3FacilitySystem.AdvanceStabilization(gameState.dimension3, applied);
        D3JobQueueSystem.AdvanceAllQueues(gameState, applied);
        D3DiagnosticSystem.Tick(gameState, applied, true);
        if (externalOfflineAuthorized)
            D3AutomationSystem.ApplyOfflineExternal(gameState, applied);
        return applied;
    }

    public static bool TryQueuePartProduction(
        GameState gameState,
        string partId,
        int version,
        long quantity,
        out string reason
    )
    {
        return D3ProductionSystem.TryQueuePartProduction(
            gameState,
            partId,
            version,
            quantity,
            out reason
        );
    }

    public static bool TryQueueNormalAssembly(
        GameState gameState,
        int mk,
        long quantity,
        out string reason
    )
    {
        return D3AssemblySystem.TryQueueNormalAssembly(
            gameState,
            mk,
            quantity,
            out reason
        );
    }

    public static bool TryQueueTraitAssembly(
        GameState gameState,
        int mk,
        IList<D3CalibrationReadingState> readings,
        out string reason)
    {
        return D3AssemblySystem.TryQueueTraitAssembly(
            gameState, mk, readings, out reason);
    }

    public static bool TryQueueTraitAssembly(
        GameState gameState,
        int mk,
        long quantity,
        IList<D3CalibrationReadingState> readings,
        out string reason)
    {
        return D3AssemblySystem.TryQueueTraitAssembly(
            gameState, mk, quantity, readings, out reason);
    }

    public static bool TryCancelJob(
        GameState gameState,
        string queueId,
        string jobId,
        out string reason
    )
    {
        return D3JobQueueSystem.TryCancelJob(gameState, queueId, jobId, out reason);
    }

    public static bool TrySetProcessBankAssignment(
        GameState gameState,
        string channelId,
        int mk,
        string traitId,
        long targetAmount,
        out string reason)
    {
        return D3FacilitySystem.TrySetProcessBankAssignment(
            gameState, channelId, mk, traitId, targetAmount, out reason);
    }

    public static bool TryQueueProcessBankUpgrade(
        GameState gameState,
        out string reason)
    {
        return D3FacilitySystem.TryQueueProcessBankUpgrade(gameState, out reason);
    }

    public static bool TryQueueFacilityUpgrade(
        GameState gameState, string facilityId, out string reason)
    {
        return D3FacilitySystem.TryQueueFacilityUpgrade(
            gameState, facilityId, out reason);
    }

    public static bool TrySetFacilityAssignment(
        GameState gameState, string facilityId, string channelId, int mk,
        string traitId, long targetAmount, out string reason)
    {
        return D3FacilitySystem.TrySetFacilityAssignment(
            gameState, facilityId, channelId, mk, traitId, targetAmount, out reason);
    }

    public static bool TryQueueResearch(
        GameState gameState, string partId, int version,
        IList<D3ReservedAutomatonState> team, out string reason)
    {
        return D3ResearchSystem.TryQueueResearch(
            gameState, partId, version, team, out reason);
    }

    public static bool ValidateState(GameState gameState, out string result)
    {
        if (gameState == null)
        {
            result = "GameState es null.";
            return false;
        }

        EnsureState(gameState);
        Dimension3State state = gameState.dimension3;

        if (state.progressVersion != ProgressVersion)
        {
            result = "Versión de progreso D3 inválida.";
            return false;
        }

        if (gameState.dimension03Unlocked && !state.initialized)
        {
            result = "D3 desbloqueada sin inicialización.";
            return false;
        }

        for (int i = 0; i < Dimension3Catalog.QueueIds.Length; i++)
        {
            D3QueueState queue = D3JobQueueSystem.GetQueue(
                state,
                Dimension3Catalog.QueueIds[i]
            );
            if (queue == null || queue.jobs == null)
            {
                result = "Falta una cola obligatoria de D3.";
                return false;
            }

            if (queue.jobs.Count > Dimension3Catalog.MaxQueueEntries)
            {
                result = "Una cola supera su tamaño máximo.";
                return false;
            }
        }

        for (int i = 0; i < state.parts.Count; i++)
        {
            D3PartStackState stack = state.parts[i];
            if (stack == null || !Dimension3Catalog.IsPartId(stack.partId) ||
                Dimension3Catalog.GetPartDefinition(stack.version) == null ||
                stack.amount < 0L)
            {
                result = "Inventario de piezas D3 inválido.";
                return false;
            }
        }

        for (int i = 0; i < state.automatons.Count; i++)
        {
            D3AutomatonStackState stack = state.automatons[i];
            if (stack == null || Dimension3Catalog.GetMkPower(stack.mk) <= 0 ||
                !Dimension3Catalog.IsTraitId(stack.traitId) || stack.totalAmount < 0L)
            {
                result = "Inventario de autómatas D3 inválido.";
                return false;
            }
        }


        for (int i = 0; i < state.assignments.Count; i++)
        {
            D3AssignmentState assignment = state.assignments[i];
            if (assignment == null ||
                !Dimension3Catalog.IsFacilityChannel(
                    assignment.installationId, assignment.channelId) ||
                Dimension3Catalog.GetMkPower(assignment.mk) <= 0 ||
                !Dimension3Catalog.IsTraitId(assignment.traitId) ||
                assignment.amount < 0L || assignment.stabilizedAmount < 0L ||
                assignment.stabilizedAmount > assignment.amount)
            {
                result = "Asignaciones D3 inválidas.";
                return false;
            }
        }


        for (int i = 0; i < state.calibrationProfiles.Count; i++)
        {
            D3CalibrationProfileState profile = state.calibrationProfiles[i];
            if (profile == null || string.IsNullOrWhiteSpace(profile.profileId) ||
                profile.readings == null || profile.controls == null)
            {
                result = "Perfil de calibración D3 inválido.";
                return false;
            }
        }

        for (int mk = 1; mk <= 6; mk++)
        {
            for (int traitIndex = 0; traitIndex < Dimension3Catalog.TraitIds.Length;
                 traitIndex++)
            {
                string traitId = Dimension3Catalog.TraitIds[traitIndex];
                long assigned = D3InventorySystem.GetAssignedAutomatonAmount(
                    state, mk, traitId);
                long reserved = D3InventorySystem.GetReservedAutomatonAmount(
                    state, mk, traitId);
                long total = D3InventorySystem.GetAutomatonAmount(state, mk, traitId);
                if (assigned > total || reserved > total - assigned)
                {
                    result = "Un autómata ocupa más de un estado.";
                    return false;
                }
            }
        }

        result = "Estado base de Dimensión 3 válido.";
        return true;
    }

    private static void InitializeUnlockedState(Dimension3State state)
    {
        state.initialized = true;
        state.selectedInstallationId = Dimension3Catalog.FacilityProcessBank;

        D3FacilityState processBank = FindFacility(
            state,
            Dimension3Catalog.FacilityProcessBank
        );
        processBank.built = true;
        processBank.level = Math.Max(1, processBank.level);

        D3InventorySystem.AddAutomatons(
            state,
            1,
            Dimension3Catalog.TraitNormal,
            1L
        );
    }

    private static void NormalizeState(Dimension3State state)
    {
        if (state.parts == null) state.parts = new List<D3PartStackState>();
        if (state.automatons == null) state.automatons = new List<D3AutomatonStackState>();
        if (state.assignments == null) state.assignments = new List<D3AssignmentState>();
        if (state.queues == null) state.queues = new List<D3QueueState>();
        if (state.research == null) state.research = new List<D3ResearchState>();
        if (state.facilities == null) state.facilities = new List<D3FacilityState>();
        if (state.calibrationProfiles == null)
            state.calibrationProfiles = new List<D3CalibrationProfileState>();
        for (int i = state.calibrationProfiles.Count - 1; i >= 0; i--)
        {
            D3CalibrationProfileState profile = state.calibrationProfiles[i];
            if (profile == null)
            {
                state.calibrationProfiles.RemoveAt(i);
                continue;
            }
            if (profile.readings == null)
                profile.readings = new List<D3CalibrationReadingState>();
            if (profile.controls == null)
                profile.controls = new List<D3CalibrationControlState>();
        }
        if (state.automationRoutines == null)
            state.automationRoutines = new List<D3AutomationRoutineState>();
        if (state.automationProfiles == null)
            state.automationProfiles = new List<D3AutomationProfileState>();
        state.nextAutomationSequence = Math.Max(1L, state.nextAutomationSequence);
        for (int i = state.automationRoutines.Count - 1; i >= 0; i--)
        {
            D3AutomationRoutineState routine = state.automationRoutines[i];
            if (routine == null)
            {
                state.automationRoutines.RemoveAt(i);
                continue;
            }
            if (routine.orderedTargetIds == null)
                routine.orderedTargetIds = new List<string>();
            routine.leReserve = Math.Max(0.0, routine.leReserve);
            routine.tracesReserve = Math.Max(0.0, routine.tracesReserve);
            routine.resourceReserveAmount = Math.Max(
                0.0, routine.resourceReserveAmount);
            routine.stopAfterExecutions = Math.Max(
                0, routine.stopAfterExecutions);
            routine.executionsCompleted = Math.Max(
                0, routine.executionsCompleted);
            routine.evaluationRemainingSeconds = Math.Max(
                0.0, routine.evaluationRemainingSeconds);
        }
        for (int i = state.automationProfiles.Count - 1; i >= 0; i--)
        {
            D3AutomationProfileState profile = state.automationProfiles[i];
            if (profile == null)
            {
                state.automationProfiles.RemoveAt(i);
                continue;
            }
            if (profile.routineIds == null)
                profile.routineIds = new List<string>();
            if (profile.savedRoutines == null)
                profile.savedRoutines = new List<D3AutomationRoutineState>();
            if (profile.savedConsoleSettings == null)
                profile.savedConsoleSettings = new D3ConsoleSettingsState();
            D3ConsoleSystem.NormalizeSettings(profile.savedConsoleSettings);
            if (profile.savedDiagnosticSettings == null)
                profile.savedDiagnosticSettings = new D3DiagnosticSettingsState();
            D3DiagnosticSystem.NormalizeSettings(profile.savedDiagnosticSettings);
            if (profile.savedMarkedFusionRecipes == null)
                profile.savedMarkedFusionRecipes =
                    new List<D3MarkedFusionRecipeState>();
            if (profile.savedCalibrationProfiles == null)
                profile.savedCalibrationProfiles =
                    new List<D3CalibrationProfileState>();
            for (int j = 0; j < profile.savedRoutines.Count; j++)
                if (profile.savedRoutines[j] != null &&
                    profile.savedRoutines[j].orderedTargetIds == null)
                    profile.savedRoutines[j].orderedTargetIds =
                        new List<string>();
        }
        if (state.markedFusionRecipes == null)
            state.markedFusionRecipes = new List<D3MarkedFusionRecipeState>();
        if (state.diagnosticSettings == null)
            state.diagnosticSettings = new D3DiagnosticSettingsState();
        D3DiagnosticSystem.NormalizeSettings(state);
        if (state.consoleSettings == null)
            state.consoleSettings = new D3ConsoleSettingsState();
        D3ConsoleSystem.NormalizeSettings(state.consoleSettings);
        if (state.totalAssembledByMk == null)
            state.totalAssembledByMk = new List<D3MkAssemblyCountState>();

        for (int i = state.assignments.Count - 1; i >= 0; i--)
        {
            D3AssignmentState assignment = state.assignments[i];
            if (assignment == null)
            {
                state.assignments.RemoveAt(i);
                continue;
            }
            assignment.amount = Math.Max(0L, assignment.amount);
            assignment.stabilizedAmount = Math.Min(
                assignment.amount, Math.Max(0L, assignment.stabilizedAmount));
            assignment.stabilizationRemainingSeconds = Math.Max(
                0.0, assignment.stabilizationRemainingSeconds);
        }

        state.progressVersion = ProgressVersion;
        state.nextJobSequence = Math.Max(1L, state.nextJobSequence);

        for (int i = 0; i < Dimension3Catalog.QueueIds.Length; i++)
            EnsureQueue(state, Dimension3Catalog.QueueIds[i]);

        FindFacility(state, Dimension3Catalog.FacilityProcessBank);
        FindFacility(state, Dimension3Catalog.FacilityProductionConsole);
        FindFacility(state, Dimension3Catalog.FacilityDiagnosticBank);
        FindFacility(state, Dimension3Catalog.FacilityExpeditionPort);
        FindFacility(state, Dimension3Catalog.FacilityAutomationCore);
    }

    private static void EnsureQueue(Dimension3State state, string queueId)
    {
        D3QueueState queue = D3JobQueueSystem.GetQueue(state, queueId);
        if (queue == null)
        {
            queue = new D3QueueState { queueId = queueId };
            state.queues.Add(queue);
        }

        if (queue.jobs == null)
            queue.jobs = new List<D3JobState>();
    }

    private static D3FacilityState FindFacility(Dimension3State state, string facilityId)
    {
        for (int i = 0; i < state.facilities.Count; i++)
        {
            D3FacilityState facility = state.facilities[i];
            if (facility != null && facility.facilityId == facilityId)
                return facility;
        }

        D3FacilityState created = new D3FacilityState
        {
            facilityId = facilityId,
            built = false,
            level = 0
        };
        state.facilities.Add(created);
        return created;
    }
}

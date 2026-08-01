using System;
using System.Collections.Generic;

// Los valores existentes se conservan para que los saves v3 y la UI temporal de C1
// sigan siendo legibles durante la migración del Bloque A.
public enum ConvergencePhase
{
    Inactive = 0,
    Ready = 1,
    CircuitAwarded = 2,
    ConfigurationPending = 3,
    CommitPrepared = 4,
    NewCycleStarted = 5,
    Completed = 6,
    Synchronizing = 7
}

public enum ConvergenceTransactionStage
{
    None = 0,
    CommitPrepared = 1,
    BaseResetApplied = 2,
    SynchronizationResetApplied = 3,
    TelemetryRecorded = 4,
    FinalStatePromoted = 5
}

[Serializable]
public class ConvergenceState
{
    public int progressVersion;
    public ConvergencePhase phase;
    public int completedCycles;
    public int nextAwardOrdinal;
    public long cycleStartedUnix;
    public long receiverRebuiltUnix;
    public long synchronizationReadyUnix;
    public long configurationStartedUnix;
    public double cycleOfflineSeconds;
    public double baseRebuildOfflineSeconds;
    public double synchronizationOfflineSeconds;
    public double configurationOfflineSeconds;
    public List<ConvergenceCycleMeasurement> normalCycleMeasurements = new List<ConvergenceCycleMeasurement>();
    public bool dimensionalReceiverRebuilt;
    public List<ConvergenceSignalState> signals = new List<ConvergenceSignalState>();
    public double currentStability;
    public List<string> processedSynchronizationSourceIds = new List<string>();
    public List<OwnedConvergenceCircuit> ownedCircuits = new List<OwnedConvergenceCircuit>();
    public List<OwnedConvergenceCircuit> unavailableOwnedCircuits = new List<OwnedConvergenceCircuit>();
    public List<ConvergenceCircuitPlacement> quarantinedPlacements = new List<ConvergenceCircuitPlacement>();

    // Autoridades v4. La configuración activa nunca se altera mientras se edita el borrador.
    public List<ConvergenceCircuitPlacement> committedPlacements = new List<ConvergenceCircuitPlacement>();
    public List<ConvergenceCircuitPlacement> draftPlacements = new List<ConvergenceCircuitPlacement>();
    public bool invalidDraftDetected;
    public ConvergenceModifierSnapshot activeSnapshot = new ConvergenceModifierSnapshot();
    public ConvergenceTransactionJournal pendingTransaction;
    public List<string> recordedTransactionIds = new List<string>();
    public bool normalConvergenceCompleted;
    public bool unknownSignalTriggered;
    public bool unknownSignalAcknowledged;
    public string pendingNarrativeEventId;

    // Compatibilidad temporal v3/C1. Se mantiene sincronizado con draft o committed.
    public List<ConvergenceCircuitPlacement> boardPlacements = new List<ConvergenceCircuitPlacement>();
    public bool boardConfigurationLocked;
    public ConvergenceModifierSnapshot modifierSnapshot = new ConvergenceModifierSnapshot();
}

[Serializable] public class ConvergenceSignalState { public int dimensionId; public bool activated; }

[Serializable]
public class ConvergenceCycleMeasurement
{
    public int completedCycleNumber;
    public double baseRebuildSeconds;
    public double baseRebuildOfflineSeconds;
    public double synchronizationSeconds;
    public double synchronizationOfflineSeconds;
    public double configurationSeconds;
    public double configurationOfflineSeconds;
    public double endToEndCycleSeconds;
    public double onlineSeconds;
    public double offlineSeconds;
}

[Serializable] public class OwnedConvergenceCircuit { public string circuitId; public bool obtained; public int awardOrdinal; public int acquiredCycle; }
[Serializable] public class ConvergenceCircuitPlacement { public string circuitId; public int x; public int y; public int rotationDegrees; }

[Serializable]
public class ConvergenceModifierSnapshot
{
    public int schemaVersion = 1;
    public int catalogRevision = 1;
    public int cycleId;
    public string layoutHash = "";
    public double baseLEProductionMultiplier = 1.0;
    public double baseTracesProductionMultiplier = 1.0;
    public double stabilityGainMultiplier = 1.0;
    public List<string> activeCircuitIds = new List<string>();
}

[Serializable]
public class ConvergenceTransactionJournal
{
    public string transactionId;
    public int targetCompletedCycles;
    public string awardedCircuitId;
    public int stage;
    public List<ConvergenceCircuitPlacement> candidatePlacements = new List<ConvergenceCircuitPlacement>();
    public ConvergenceModifierSnapshot candidateSnapshot = new ConvergenceModifierSnapshot();
    public bool baseResetApplied;
    public bool synchronizationResetApplied;
    public bool telemetryRecorded;
    public bool finalStatePromoted;
    public bool invalidCandidateDetected;
}

public static class ConvergenceSystem
{
    public const int ProgressVersion = 4;
    public const int SnapshotSchemaVersion = 1;
    public const string UnknownSignalEventId = "convergence_unknown_signal_001";
    public const double MaxLEMultiplier = 2.0;
    public const double MaxTracesMultiplier = 2.0;
    public const double MaxStabilityMultiplier = 1.5;

    public static ConvergenceState CreateInitialState()
    {
        return new ConvergenceState
        {
            progressVersion = ProgressVersion,
            phase = ConvergencePhase.Inactive,
            nextAwardOrdinal = 1,
            signals = CreateSignals()
        };
    }

    public static bool IsFutureVersion(ConvergenceState state) => state != null && state.progressVersion > ProgressVersion;

    public static void EnsureState(GameState gameState)
    {
        if (gameState == null) return;
        if (gameState.convergence == null) { gameState.convergence = CreateInitialState(); return; }
        ConvergenceState state = gameState.convergence;
        // Nunca rebajar ni normalizar un formato que esta versión no entiende.
        if (IsFutureVersion(state)) return;

        MigrateToV4(state);
        state.completedCycles = Math.Max(0, state.completedCycles);
        state.normalCycleMeasurements ??= new List<ConvergenceCycleMeasurement>();
        state.currentStability = IsFiniteNonNegative(state.currentStability) ? state.currentStability : 0.0;
        if (!Enum.IsDefined(typeof(ConvergencePhase), state.phase)) state.phase = ConvergencePhase.Inactive;
        NormalizeSignals(state);
        NormalizeProcessedSourceIds(state);
        NormalizeCircuitState(state);
        NormalizeJournal(state);
        NormalizePhase(state);
        NormalizeSnapshots(state);
        NormalizeNarrativeState(state);
        state.progressVersion = ProgressVersion;
    }

    private static void MigrateToV4(ConvergenceState state)
    {
        if (state.progressVersion >= ProgressVersion) return;
        state.ownedCircuits ??= new List<OwnedConvergenceCircuit>();
        int ordinal = 1;
        foreach (OwnedConvergenceCircuit owned in state.ownedCircuits)
        {
            if (owned == null || !owned.obtained) continue;
            if (owned.awardOrdinal <= 0) owned.awardOrdinal = ordinal;
            if (owned.acquiredCycle < 0) owned.acquiredCycle = 0;
            ordinal++;
        }
        state.nextAwardOrdinal = state.nextAwardOrdinal > 0 ? state.nextAwardOrdinal : Math.Max(1, ordinal);
        List<ConvergenceCircuitPlacement> legacy = ClonePlacements(state.boardPlacements);
        if (state.phase == ConvergencePhase.ConfigurationPending || state.phase == ConvergencePhase.CircuitAwarded)
        {
            state.draftPlacements = legacy;
            state.committedPlacements ??= new List<ConvergenceCircuitPlacement>();
        }
        else
        {
            state.committedPlacements = legacy;
            state.draftPlacements ??= new List<ConvergenceCircuitPlacement>();
        }
        state.activeSnapshot = CloneSnapshot(state.modifierSnapshot);
        state.recordedTransactionIds ??= new List<string>();
    }

    private static void NormalizeCircuitState(ConvergenceState state)
    {
        state.ownedCircuits ??= new List<OwnedConvergenceCircuit>();
        var owned = new List<OwnedConvergenceCircuit>(); var ids = new HashSet<string>(); int ordinal = 1;
        state.unavailableOwnedCircuits ??= new List<OwnedConvergenceCircuit>();
        foreach (OwnedConvergenceCircuit circuit in state.ownedCircuits)
        {
            if (circuit == null || !circuit.obtained) continue;
            if (ConvergenceCircuitCatalog.IsKnownCircuit(circuit.circuitId) && ids.Add(circuit.circuitId))
                owned.Add(new OwnedConvergenceCircuit { circuitId = circuit.circuitId, obtained = true, awardOrdinal = circuit.awardOrdinal > 0 ? circuit.awardOrdinal : ordinal++, acquiredCycle = Math.Max(0, circuit.acquiredCycle) });
            else if (!state.unavailableOwnedCircuits.Exists(c => c != null && c.circuitId == circuit.circuitId))
                state.unavailableOwnedCircuits.Add(new OwnedConvergenceCircuit { circuitId = circuit.circuitId, obtained = true, awardOrdinal = circuit.awardOrdinal, acquiredCycle = circuit.acquiredCycle });
        }
        state.ownedCircuits = owned;
        bool awardPending = state.phase == ConvergencePhase.ConfigurationPending ||
            state.phase == ConvergencePhase.CircuitAwarded ||
            state.phase == ConvergencePhase.CommitPrepared ||
            state.pendingTransaction != null;
        state.nextAwardOrdinal = awardPending
            ? Math.Max(1, state.completedCycles + 1)
            : Math.Max(state.nextAwardOrdinal, owned.Count + 1);
        state.quarantinedPlacements ??= new List<ConvergenceCircuitPlacement>();
        state.committedPlacements = NormalizePlacements(state.committedPlacements, ids, state.quarantinedPlacements);
        if (!ConvergenceCircuitResolver.ResolveBoard(state.draftPlacements).structurallyValid)
            state.invalidDraftDetected = true;
        state.draftPlacements = NormalizePlacements(state.draftPlacements, ids, state.quarantinedPlacements);
        bool configuring = state.phase == ConvergencePhase.ConfigurationPending || state.phase == ConvergencePhase.CircuitAwarded;
        state.boardPlacements = configuring ? state.draftPlacements : state.committedPlacements;
        state.boardConfigurationLocked = !configuring && state.committedPlacements.Count > 0;
    }

    private static List<ConvergenceCircuitPlacement> NormalizePlacements(List<ConvergenceCircuitPlacement> source, HashSet<string> ownedIds, List<ConvergenceCircuitPlacement> quarantine = null)
    {
        var result = new List<ConvergenceCircuitPlacement>(); var placed = new HashSet<string>(); var cells = new HashSet<string>();
        if (source == null) return result;
        foreach (ConvergenceCircuitPlacement p in source)
        {
            string cell = p == null ? "" : p.x + ":" + p.y;
            if (p != null && ownedIds.Contains(p.circuitId) && ConvergenceCircuitCatalog.IsValidRotation(p.rotationDegrees) && ConvergenceCircuitCatalog.IsValidBoardCoordinate(p.x, p.y) && !(p.x == 0 && p.y == 0) && placed.Add(p.circuitId) && cells.Add(cell))
                result.Add(new ConvergenceCircuitPlacement { circuitId = p.circuitId, x = p.x, y = p.y, rotationDegrees = p.rotationDegrees });
            else if (p != null && quarantine != null && !quarantine.Exists(q => q != null && q.circuitId == p.circuitId && q.x == p.x && q.y == p.y && q.rotationDegrees == p.rotationDegrees))
                quarantine.Add(new ConvergenceCircuitPlacement { circuitId = p.circuitId, x = p.x, y = p.y, rotationDegrees = p.rotationDegrees });
        }
        return result;
    }

    private static void NormalizeSnapshots(ConvergenceState state)
    {
        // Un snapshot ya estabilizado es histórico: no se recalcula ni se actualiza
        // su revisión de catálogo hasta que el jugador vuelva a estabilizar la placa.
        if (IsSnapshotValid(state.activeSnapshot))
        {
            state.activeSnapshot = CloneSnapshot(state.activeSnapshot);
        }
        else
        {
            ConvergenceBoardResolution resolution = ConvergenceCircuitResolver.ResolveBoard(
                state.committedPlacements);
            state.activeSnapshot = resolution.structurallyValid
                ? PrepareSnapshot(resolution.candidateSnapshot, state.completedCycles)
                : CreateDefaultSnapshot(state.completedCycles);
        }
        state.modifierSnapshot = state.activeSnapshot;
    }

    public static bool IsSnapshotValid(ConvergenceModifierSnapshot snapshot)
    {
        return snapshot != null &&
            IsFiniteInRange(snapshot.baseLEProductionMultiplier, 1.0, MaxLEMultiplier) &&
            IsFiniteInRange(snapshot.baseTracesProductionMultiplier, 1.0, MaxTracesMultiplier) &&
            IsFiniteInRange(snapshot.stabilityGainMultiplier, 1.0, MaxStabilityMultiplier);
    }

    public static ConvergenceModifierSnapshot PrepareSnapshot(
        ConvergenceModifierSnapshot snapshot, int cycleId)
    {
        if (!IsSnapshotValid(snapshot))
            return CreateDefaultSnapshot(cycleId);
        ConvergenceModifierSnapshot prepared = CloneSnapshot(snapshot);
        prepared.schemaVersion = SnapshotSchemaVersion;
        prepared.catalogRevision = ConvergenceCircuitCatalog.CatalogRevision;
        prepared.cycleId = Math.Max(0, cycleId);
        prepared.layoutHash ??= "";
        prepared.activeCircuitIds ??= new List<string>();
        return prepared;
    }

    public static ConvergenceModifierSnapshot CreateDefaultSnapshot(int cycleId = 0)
    {
        return new ConvergenceModifierSnapshot
        {
            schemaVersion = SnapshotSchemaVersion,
            catalogRevision = ConvergenceCircuitCatalog.CatalogRevision,
            cycleId = Math.Max(0, cycleId),
            layoutHash = "",
            baseLEProductionMultiplier = 1.0,
            baseTracesProductionMultiplier = 1.0,
            stabilityGainMultiplier = 1.0,
            activeCircuitIds = new List<string>()
        };
    }
    private static void NormalizeJournal(ConvergenceState state)
    {
        if (state.pendingTransaction == null) return;
        ConvergenceTransactionJournal j = state.pendingTransaction;
        if (string.IsNullOrWhiteSpace(j.transactionId) || j.targetCompletedCycles < state.completedCycles) { state.pendingTransaction = null; return; }
        ConvergenceBoardResolution rawResolution = ConvergenceCircuitResolver.ResolveBoard(
            j.candidatePlacements);
        if (!rawResolution.structurallyValid)
            j.invalidCandidateDetected = true;
        j.candidatePlacements = NormalizePlacements(j.candidatePlacements, OwnedIds(state), state.quarantinedPlacements);
        ConvergenceBoardResolution normalizedResolution = ConvergenceCircuitResolver.ResolveBoard(
            j.candidatePlacements);
        if (!j.invalidCandidateDetected && normalizedResolution.structurallyValid)
        {
            j.candidateSnapshot = PrepareSnapshot(
                normalizedResolution.candidateSnapshot, j.targetCompletedCycles);
        }
        else
        {
            j.candidateSnapshot = CreateDefaultSnapshot(j.targetCompletedCycles);
        }
    }
    private static HashSet<string> OwnedIds(ConvergenceState state) { var ids = new HashSet<string>(); foreach (var c in state.ownedCircuits) if (c != null && c.obtained) ids.Add(c.circuitId); return ids; }
    private static void NormalizePhase(ConvergenceState state)
    {
        if (state.normalConvergenceCompleted)
        {
            state.phase = ConvergencePhase.Completed;
            return;
        }
        if (state.pendingTransaction != null)
        {
            if (state.pendingTransaction.finalStatePromoted)
            {
                state.phase = state.pendingTransaction.awardedCircuitId ==
                    "convergence_circuit_006_closure_matrix"
                    ? ConvergencePhase.Completed
                    : ConvergencePhase.NewCycleStarted;
            }
            else
            {
                state.phase = ConvergencePhase.CommitPrepared;
            }
            return;
        }
        if (state.phase == ConvergencePhase.CircuitAwarded)
        {
            state.phase = ConvergencePhase.ConfigurationPending;
            return;
        }
        if (state.phase == ConvergencePhase.ConfigurationPending ||
            state.phase == ConvergencePhase.CommitPrepared)
            return;
        if (!state.dimensionalReceiverRebuilt)
        {
            if (state.phase == ConvergencePhase.Synchronizing ||
                state.phase == ConvergencePhase.Ready)
                state.phase = ConvergencePhase.Inactive;
            return;
        }
        bool allSignals = state.signals != null &&
            state.signals.Exists(s => s != null && s.dimensionId == 1 && s.activated) &&
            state.signals.Exists(s => s != null && s.dimensionId == 2 && s.activated) &&
            state.signals.Exists(s => s != null && s.dimensionId == 3 && s.activated);
        double requirement = ConvergenceBalance.GetRequiredStabilityForNextCircuit(
            state.ownedCircuits == null ? 0 : state.ownedCircuits.Count);
        state.phase = allSignals && state.currentStability >= requirement
            ? ConvergencePhase.Ready : ConvergencePhase.Synchronizing;
    }
    private static void NormalizeNarrativeState(ConvergenceState state)
    {
        if (state.unknownSignalAcknowledged)
        {
            state.pendingNarrativeEventId = "";
            return;
        }
        if (state.normalConvergenceCompleted && state.unknownSignalTriggered)
            state.pendingNarrativeEventId = UnknownSignalEventId;
        else if (state.pendingNarrativeEventId == "convergence_unknown_signal")
            state.pendingNarrativeEventId = UnknownSignalEventId;
    }
    private static void NormalizeSignals(ConvergenceState state) { var result = new List<ConvergenceSignalState>(); for (int id = 1; id <= 3; id++) { bool active = state.signals != null && state.signals.Exists(s => s != null && s.dimensionId == id && s.activated); result.Add(new ConvergenceSignalState { dimensionId = id, activated = active }); } state.signals = result; }
    private static List<ConvergenceSignalState> CreateSignals() => new List<ConvergenceSignalState> { new ConvergenceSignalState { dimensionId = 1 }, new ConvergenceSignalState { dimensionId = 2 }, new ConvergenceSignalState { dimensionId = 3 } };
    private static void NormalizeProcessedSourceIds(ConvergenceState state) { var result = new List<string>(); var seen = new HashSet<string>(); if (state.processedSynchronizationSourceIds != null) foreach (string id in state.processedSynchronizationSourceIds) if (!string.IsNullOrWhiteSpace(id) && seen.Add(id)) result.Add(id); state.processedSynchronizationSourceIds = result; }
    public static List<ConvergenceCircuitPlacement> ClonePlacements(List<ConvergenceCircuitPlacement> source) { var result = new List<ConvergenceCircuitPlacement>(); if (source != null) foreach (var p in source) if (p != null) result.Add(new ConvergenceCircuitPlacement { circuitId = p.circuitId, x = p.x, y = p.y, rotationDegrees = p.rotationDegrees }); return result; }
    public static ConvergenceModifierSnapshot CloneSnapshot(ConvergenceModifierSnapshot s) { s ??= new ConvergenceModifierSnapshot(); return new ConvergenceModifierSnapshot { schemaVersion = s.schemaVersion, catalogRevision = s.catalogRevision, cycleId = s.cycleId, layoutHash = s.layoutHash ?? "", baseLEProductionMultiplier = s.baseLEProductionMultiplier, baseTracesProductionMultiplier = s.baseTracesProductionMultiplier, stabilityGainMultiplier = s.stabilityGainMultiplier, activeCircuitIds = new List<string>(s.activeCircuitIds ?? new List<string>()) }; }
    private static bool IsFiniteNonNegative(double value) => !double.IsNaN(value) && !double.IsInfinity(value) && value >= 0.0;
    private static bool IsFiniteInRange(double value, double min, double max) =>
        !double.IsNaN(value) && !double.IsInfinity(value) && value >= min && value <= max;
}

using System;
using System.Collections.Generic;

public enum ConvergencePhase
{
    Inactive = 0,
    Ready = 1,
    CircuitAwarded = 2,
    ConfigurationPending = 3,
    ConfigurationConfirmed = 4,
    NewCycleStarted = 5
}

[Serializable]
public class ConvergenceState
{
    // Este objeto representa el Núcleo permanente de Convergencia. La Máquina
    // reconstruible conserva sus nodos y análisis únicamente en MachineManager.
    public int progressVersion;
    public ConvergencePhase phase;
    public int completedCycles;
    public long cycleStartedUnix;
    public long receiverRebuiltUnix;
    public long synchronizationReadyUnix;
    public long configurationStartedUnix;
    public double cycleOfflineSeconds;
    public double baseRebuildOfflineSeconds;
    public double synchronizationOfflineSeconds;
    public double configurationOfflineSeconds;
    public List<ConvergenceCycleMeasurement> normalCycleMeasurements =
        new List<ConvergenceCycleMeasurement>();
    public bool dimensionalReceiverRebuilt;
    public List<ConvergenceSignalState> signals = new List<ConvergenceSignalState>();
    public double currentStability;
    public List<string> processedSynchronizationSourceIds = new List<string>();
    public List<OwnedConvergenceCircuit> ownedCircuits =
        new List<OwnedConvergenceCircuit>();
    public List<ConvergenceCircuitPlacement> boardPlacements =
        new List<ConvergenceCircuitPlacement>();
    public bool boardConfigurationLocked;
    public ConvergenceModifierSnapshot modifierSnapshot =
        new ConvergenceModifierSnapshot();
}

[Serializable]
public class ConvergenceSignalState
{
    public int dimensionId;
    public bool activated;
}

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

[Serializable]
public class OwnedConvergenceCircuit
{
    public string circuitId;
    public bool obtained;
}

[Serializable]
public class ConvergenceCircuitPlacement
{
    public string circuitId;
    public int x;
    public int y;
    public int rotationDegrees;
}

[Serializable]
public class ConvergenceModifierSnapshot
{
    public double baseLEProductionMultiplier = 1.0;
    public List<string> activeCircuitIds = new List<string>();
}

public static class ConvergenceSystem
{
    public const int ProgressVersion = 3;

    public static ConvergenceState CreateInitialState()
    {
        return new ConvergenceState
        {
            progressVersion = ProgressVersion,
            phase = ConvergencePhase.Inactive,
            completedCycles = 0,
            cycleStartedUnix = 0L,
            receiverRebuiltUnix = 0L,
            synchronizationReadyUnix = 0L,
            configurationStartedUnix = 0L,
            normalCycleMeasurements = new List<ConvergenceCycleMeasurement>(),
            dimensionalReceiverRebuilt = false,
            signals = new List<ConvergenceSignalState>
            {
                new ConvergenceSignalState { dimensionId = 1 },
                new ConvergenceSignalState { dimensionId = 2 },
                new ConvergenceSignalState { dimensionId = 3 }
            },
            currentStability = 0.0,
            processedSynchronizationSourceIds = new List<string>(),
            ownedCircuits = new List<OwnedConvergenceCircuit>(),
            boardPlacements = new List<ConvergenceCircuitPlacement>(),
            boardConfigurationLocked = false,
            modifierSnapshot = new ConvergenceModifierSnapshot()
        };
    }

    public static void EnsureState(GameState gameState)
    {
        if (gameState == null)
            return;

        if (gameState.convergence == null)
            gameState.convergence = CreateInitialState();

        gameState.convergence.progressVersion = ProgressVersion;
        gameState.convergence.completedCycles = Math.Max(
            0,
            gameState.convergence.completedCycles
        );
        gameState.convergence.normalCycleMeasurements ??=
            new List<ConvergenceCycleMeasurement>();
        gameState.convergence.currentStability =
            double.IsNaN(gameState.convergence.currentStability) ||
            double.IsInfinity(gameState.convergence.currentStability)
                ? 0.0
                : Math.Max(0.0, gameState.convergence.currentStability);

        if (!Enum.IsDefined(
                typeof(ConvergencePhase),
                gameState.convergence.phase))
        {
            gameState.convergence.phase = ConvergencePhase.Inactive;
        }

        NormalizeSignals(gameState.convergence);
        NormalizeProcessedSourceIds(gameState.convergence);
        NormalizeCircuitState(gameState.convergence);
    }

    private static void NormalizeSignals(ConvergenceState state)
    {
        var normalized = new List<ConvergenceSignalState>();
        for (int dimensionId = 1; dimensionId <= 3; dimensionId++)
        {
            bool activated = false;
            if (state.signals != null)
            {
                foreach (ConvergenceSignalState signal in state.signals)
                {
                    if (signal != null && signal.dimensionId == dimensionId)
                    {
                        activated = signal.activated;
                        break;
                    }
                }
            }

            normalized.Add(new ConvergenceSignalState
            {
                dimensionId = dimensionId,
                activated = activated
            });
        }

        state.signals = normalized;
    }

    private static void NormalizeProcessedSourceIds(ConvergenceState state)
    {
        var normalized = new List<string>();
        var seen = new HashSet<string>();
        if (state.processedSynchronizationSourceIds != null)
        {
            foreach (string sourceId in state.processedSynchronizationSourceIds)
            {
                if (!string.IsNullOrWhiteSpace(sourceId) && seen.Add(sourceId))
                    normalized.Add(sourceId);
            }
        }

        state.processedSynchronizationSourceIds = normalized;
    }

    private static void NormalizeCircuitState(ConvergenceState state)
    {
        var owned = new List<OwnedConvergenceCircuit>();
        var ownedIds = new HashSet<string>();
        if (state.ownedCircuits != null)
        {
            foreach (OwnedConvergenceCircuit circuit in state.ownedCircuits)
            {
                if (circuit != null && circuit.obtained &&
                    ConvergenceCircuitCatalog.IsKnownCircuit(circuit.circuitId) &&
                    ownedIds.Add(circuit.circuitId))
                {
                    owned.Add(new OwnedConvergenceCircuit
                    {
                        circuitId = circuit.circuitId,
                        obtained = true
                    });
                }
            }
        }
        state.ownedCircuits = owned;

        var placements = new List<ConvergenceCircuitPlacement>();
        var placedCircuitIds = new HashSet<string>();
        var occupiedCells = new HashSet<string>();
        if (state.boardPlacements != null)
        {
            foreach (ConvergenceCircuitPlacement placement in state.boardPlacements)
            {
                string cell = placement != null ? placement.x + ":" + placement.y : "";
                if (placement != null && ownedIds.Contains(placement.circuitId) &&
                    ConvergenceCircuitCatalog.IsValidRotation(placement.rotationDegrees) &&
                    !(placement.x == 0 && placement.y == 0) &&
                    placedCircuitIds.Add(placement.circuitId) && occupiedCells.Add(cell))
                {
                    placements.Add(placement);
                }
            }
        }
        state.boardPlacements = placements;

        if (state.modifierSnapshot == null)
            state.modifierSnapshot = new ConvergenceModifierSnapshot();
        state.modifierSnapshot.baseLEProductionMultiplier =
            double.IsNaN(state.modifierSnapshot.baseLEProductionMultiplier) ||
            double.IsInfinity(state.modifierSnapshot.baseLEProductionMultiplier)
                ? 1.0
                : Math.Max(1.0, state.modifierSnapshot.baseLEProductionMultiplier);
        state.modifierSnapshot.activeCircuitIds ??= new List<string>();
    }
}

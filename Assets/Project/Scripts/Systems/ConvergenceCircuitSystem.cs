using System.Collections.Generic;

public static class ConvergenceCircuitSystem
{
    public static bool IsConvergenceUnlocked(GameState gameState)
    {
        return DimensionCompletionService.AreAllDimensionsCompleted(gameState);
    }

    public static int GetOwnedCircuitCount(GameState gameState)
    {
        if (gameState == null) return 0;
        gameState.EnsureConvergenceState();
        return gameState.convergence.ownedCircuits.Count;
    }

    public static bool TryStartNormalConvergence(GameState gameState, out string reason)
    {
        if (gameState == null)
        {
            reason = "No hay estado de juego disponible.";
            return false;
        }

        gameState.EnsureConvergenceState();
        ConvergenceState state = gameState.convergence;
        if (state.phase == ConvergencePhase.ConfigurationPending ||
            state.phase == ConvergencePhase.CircuitAwarded)
        {
            state.phase = ConvergencePhase.ConfigurationPending;
            state.boardConfigurationLocked = false;
            Persist();
            reason = "La configuración del circuito está pendiente.";
            return true;
        }

        if (state.phase != ConvergencePhase.Inactive)
        {
            reason = "La transacción de Convergencia actual no puede iniciarse de nuevo.";
            return false;
        }

        if (!ConvergenceSynchronizationSystem.IsSynchronizationReadyForNextConvergence(
                gameState, GetOwnedCircuitCount(gameState)))
        {
            reason = "Falta completar la sincronización dimensional requerida.";
            return false;
        }

        if (HasOwnedCircuit(state, ConvergenceCircuitCatalog.StartupPulseCircuitId))
        {
            reason = "No hay otro circuito diseñado disponible para esta prueba.";
            return false;
        }

        state.phase = ConvergencePhase.Ready;
        ConvergenceTelemetrySystem.RecordReady(gameState);
        Persist();
        state.ownedCircuits.Add(new OwnedConvergenceCircuit
        {
            circuitId = ConvergenceCircuitCatalog.StartupPulseCircuitId,
            obtained = true
        });
        state.phase = ConvergencePhase.CircuitAwarded;
        Persist();
        state.boardConfigurationLocked = false;
        state.phase = ConvergencePhase.ConfigurationPending;
        Persist();
        reason = "Pulso de Arranque obtenido. Configura la placa.";
        return true;
    }

    public static bool TryPlaceCircuit(
        GameState gameState, string circuitId, int x, int y, int rotationDegrees,
        out string reason)
    {
        if (gameState == null)
        {
            reason = "No hay estado de juego disponible.";
            return false;
        }

        gameState.EnsureConvergenceState();
        ConvergenceState state = gameState.convergence;
        if (state.phase != ConvergencePhase.ConfigurationPending ||
            state.boardConfigurationLocked)
        {
            reason = "La placa no está disponible para configuración.";
            return false;
        }
        if (!HasOwnedCircuit(state, circuitId) ||
            !ConvergenceCircuitCatalog.IsValidRotation(rotationDegrees))
        {
            reason = "El circuito o la rotación no son válidos.";
            return false;
        }
        if (!ConvergenceCircuitCatalog.IsValidBoardCoordinate(x, y))
        {
            reason = "La posición está fuera de la placa experimental 5×5.";
            return false;
        }
        if (x == 0 && y == 0)
        {
            reason = "El Núcleo ocupa la posición central de la placa.";
            return false;
        }

        ConvergenceCircuitPlacement current = FindPlacement(state, circuitId);
        foreach (ConvergenceCircuitPlacement placement in state.boardPlacements)
        {
            if (placement != current && placement.x == x && placement.y == y)
            {
                reason = "La posición de la placa ya está ocupada.";
                return false;
            }
        }

        if (current == null)
        {
            current = new ConvergenceCircuitPlacement { circuitId = circuitId };
            state.boardPlacements.Add(current);
        }
        current.x = x;
        current.y = y;
        current.rotationDegrees = rotationDegrees;
        Persist();
        reason = "Circuito colocado.";
        return true;
    }

    public static bool TryConfirmConfiguration(GameState gameState, out string reason)
    {
        if (gameState == null)
        {
            reason = "No hay estado de juego disponible.";
            return false;
        }

        gameState.EnsureConvergenceState();
        ConvergenceState state = gameState.convergence;
        if (state.phase != ConvergencePhase.ConfigurationPending ||
            state.boardConfigurationLocked)
        {
            reason = "No hay una configuración pendiente para estabilizar.";
            return false;
        }

        ResolveModifierSnapshot(state);
        state.boardConfigurationLocked = true;
        state.phase = ConvergencePhase.ConfigurationConfirmed;
        Persist();

        ConvergenceTelemetrySystem.RecordConfigurationConfirmed(gameState);
        gameState.ResetGameBaseForConvergence();
        ConvergenceSynchronizationSystem.ResetCycleSynchronization(gameState);
        state.completedCycles++;
        state.phase = ConvergencePhase.NewCycleStarted;
        Persist();
        reason = "Configuración estabilizada. Comenzó un nuevo ciclo.";
        return true;
    }

    public static void RecoverTransaction(GameState gameState)
    {
        if (gameState == null) return;
        gameState.EnsureConvergenceState();
        if (gameState.convergence.phase == ConvergencePhase.CircuitAwarded)
        {
            gameState.convergence.phase = ConvergencePhase.ConfigurationPending;
            gameState.convergence.boardConfigurationLocked = false;
        }
        else if (gameState.convergence.phase == ConvergencePhase.ConfigurationConfirmed)
        {
            gameState.ResetGameBaseForConvergence();
            ConvergenceSynchronizationSystem.ResetCycleSynchronization(gameState);
            gameState.convergence.phase = ConvergencePhase.NewCycleStarted;
        }
    }

    public static double GetBaseLEProductionMultiplier(GameState gameState)
    {
        if (gameState == null) return 1.0;
        gameState.EnsureConvergenceState();
        ConvergenceState state = gameState.convergence;
        return state.boardConfigurationLocked &&
            state.modifierSnapshot != null
            ? state.modifierSnapshot.baseLEProductionMultiplier
            : 1.0;
    }

    private static void ResolveModifierSnapshot(ConvergenceState state)
    {
        var snapshot = new ConvergenceModifierSnapshot();
        ConvergenceCircuitPlacement startupPulse = FindPlacement(
            state, ConvergenceCircuitCatalog.StartupPulseCircuitId);
        if (startupPulse != null && IsStartupPulsePowered(startupPulse))
        {
            snapshot.activeCircuitIds.Add(
                ConvergenceCircuitCatalog.StartupPulseCircuitId);
            snapshot.baseLEProductionMultiplier *=
                1.0 + ConvergenceCircuitCatalog.StartupPulseBaseLEProductionBonus;
        }
        state.modifierSnapshot = snapshot;
    }

    private static bool IsStartupPulsePowered(ConvergenceCircuitPlacement placement)
    {
        bool vertical = placement.rotationDegrees == 0 ||
            placement.rotationDegrees == 180;
        bool horizontal = placement.rotationDegrees == 90 ||
            placement.rotationDegrees == 270;
        return (vertical && placement.x == 0 &&
                (placement.y == 1 || placement.y == -1)) ||
            (horizontal && placement.y == 0 &&
                (placement.x == 1 || placement.x == -1));
    }

    private static bool HasOwnedCircuit(ConvergenceState state, string circuitId)
    {
        return state.ownedCircuits.Exists(c => c != null && c.obtained &&
            c.circuitId == circuitId);
    }

    private static ConvergenceCircuitPlacement FindPlacement(
        ConvergenceState state, string circuitId)
    {
        return state.boardPlacements.Find(p => p != null && p.circuitId == circuitId);
    }

    private static void Persist()
    {
        if (SaveService.I != null) SaveService.I.Save();
    }
}

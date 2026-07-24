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

    public static bool HasNextDesignedCircuit(GameState gameState)
    {
        if (gameState == null) return false;
        gameState.EnsureConvergenceState();
        return !HasOwnedCircuit(gameState.convergence,
            ConvergenceCircuitCatalog.StartupPulseCircuitId);
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

        if (!ConvergenceSynchronizationSystem.CanRebuildReceiver(gameState, out reason))
            return false;

        if (!ConvergenceSynchronizationSystem.IsSynchronizationReadyForNextConvergence(
                gameState, GetOwnedCircuitCount(gameState)))
        {
            reason = "Falta completar la sincronización dimensional requerida.";
            return false;
        }

        if (!HasNextDesignedCircuit(gameState))
        {
            reason = "No hay otro circuito diseñado disponible para esta prueba.";
            return false;
        }

        state.ownedCircuits.Add(new OwnedConvergenceCircuit
        {
            circuitId = ConvergenceCircuitCatalog.StartupPulseCircuitId,
            obtained = true
        });
        state.boardConfigurationLocked = false;
        state.phase = ConvergencePhase.ConfigurationPending;
        ConvergenceTelemetrySystem.RecordConfigurationStarted(gameState);
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

        if (state.boardPlacements == null || state.boardPlacements.Count == 0)
        {
            reason = "Coloca al menos un circuito antes de estabilizar la placa.";
            return false;
        }
        ConvergenceCircuitPlacement startupPulse = FindPlacement(state,
            ConvergenceCircuitCatalog.StartupPulseCircuitId);
        if (HasOwnedCircuit(state, ConvergenceCircuitCatalog.StartupPulseCircuitId) &&
            (startupPulse == null || !IsStartupPulsePowered(startupPulse)))
        {
            reason = "Pulso de Arranque sin energía. Colócalo junto al Núcleo y oriéntalo hacia él.";
            return false;
        }

        ResolveModifierSnapshot(state);
        state.boardConfigurationLocked = true;
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
        if (gameState.convergence.phase == ConvergencePhase.Ready)
        {
            gameState.convergence.phase = HasOwnedCircuit(gameState.convergence,
                ConvergenceCircuitCatalog.StartupPulseCircuitId)
                ? ConvergencePhase.ConfigurationPending : ConvergencePhase.Inactive;
            gameState.convergence.boardConfigurationLocked = false;
        }
        else if (gameState.convergence.phase == ConvergencePhase.CircuitAwarded)
        {
            gameState.convergence.phase = ConvergencePhase.ConfigurationPending;
            gameState.convergence.boardConfigurationLocked = false;
        }
        else if (gameState.convergence.phase == ConvergencePhase.ConfigurationConfirmed)
        {
            ConvergenceTelemetrySystem.RecordConfigurationConfirmed(gameState);
            gameState.ResetGameBaseForConvergence();
            ConvergenceSynchronizationSystem.ResetCycleSynchronization(gameState);
            gameState.convergence.completedCycles++;
            gameState.convergence.phase = ConvergencePhase.NewCycleStarted;
            Persist();
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

    public static bool IsCircuitPowered(GameState gameState, string circuitId)
    {
        if (gameState == null || circuitId != ConvergenceCircuitCatalog.StartupPulseCircuitId)
            return false;
        gameState.EnsureConvergenceState();
        ConvergenceCircuitPlacement placement = FindPlacement(gameState.convergence, circuitId);
        return placement != null && IsStartupPulsePowered(placement);
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
        return (placement.x == 0 && placement.y == 1 &&
                placement.rotationDegrees == 0) ||
            (placement.x == 0 && placement.y == -1 &&
                placement.rotationDegrees == 180) ||
            (placement.x == -1 && placement.y == 0 &&
                placement.rotationDegrees == 90) ||
            (placement.x == 1 && placement.y == 0 &&
                placement.rotationDegrees == 270);
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

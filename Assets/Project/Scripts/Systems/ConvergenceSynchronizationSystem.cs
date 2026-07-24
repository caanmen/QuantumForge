using System;

public static class ConvergenceSynchronizationSystem
{
    public static bool CanRebuildReceiver(GameState gameState, out string reason)
    {
        if (gameState == null)
        {
            reason = "No hay estado de juego disponible.";
            return false;
        }
        MachineManager machine = MachineManager.I;
        if (machine == null || !machine.MachineUnlocked)
        {
            reason = "Redescubre y reconstruye la Máquina primero.";
            return false;
        }
        if (!machine.HasEnoughRepairForPrestige1())
        {
            reason = "Alcanza 80 % de reparación de la Máquina.";
            return false;
        }
        if (!machine.Prestige1Prepared)
        {
            reason = "Activa el Canal de Convergencia en la Máquina.";
            return false;
        }
        if (!DimensionCompletionService.AreAllDimensionsCompleted(gameState))
        {
            reason = "Las tres dimensiones deben completar su hito final.";
            return false;
        }
        reason = "La Máquina está lista para reconstruir el Receptor.";
        return true;
    }

    public static bool TryRebuildReceiver(GameState gameState, out string reason)
    {
        if (gameState == null)
        {
            reason = "No hay estado de juego disponible.";
            return false;
        }

        gameState.EnsureConvergenceState();
        if (!CanRebuildReceiver(gameState, out reason))
            return false;

        if (!ConvergenceCircuitSystem.HasNextDesignedCircuit(gameState))
        {
            reason = "Contenido experimental de Convergencia completado. El siguiente circuito aún no está disponible.";
            return false;
        }

        if (gameState.convergence.dimensionalReceiverRebuilt)
        {
            reason = "El Receptor Dimensional ya fue reconstruido.";
            return false;
        }

        gameState.convergence.dimensionalReceiverRebuilt = true;
        ConvergenceTelemetrySystem.RecordReceiverRebuilt(gameState);
        if (gameState.convergence.phase == ConvergencePhase.NewCycleStarted)
            gameState.convergence.phase = ConvergencePhase.Inactive;
        reason = "Receptor Dimensional reconstruido.";
        return true;
    }

    public static bool TryAddSynchronization(
        GameState gameState,
        int dimensionId,
        bool activateSignal,
        double stabilityAmount,
        string sourceId,
        out string reason)
    {
        if (gameState == null)
        {
            reason = "No hay estado de juego disponible.";
            return false;
        }

        gameState.EnsureConvergenceState();
        if (!gameState.convergence.dimensionalReceiverRebuilt)
        {
            reason = "Reconstruye el Receptor Dimensional primero.";
            return false;
        }

        if (!DimensionCompletionService.IsDimensionCompleted(gameState, dimensionId))
        {
            reason = "La dimensión debe estar descubierta y completada.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(sourceId))
        {
            reason = "La sincronización requiere un identificador de fuente.";
            return false;
        }

        if (double.IsNaN(stabilityAmount) || double.IsInfinity(stabilityAmount) ||
            stabilityAmount < 0.0 || (!activateSignal && stabilityAmount <= 0.0))
        {
            reason = "El aporte de estabilidad no es válido.";
            return false;
        }

        if (gameState.convergence.processedSynchronizationSourceIds.Contains(sourceId))
        {
            reason = "Esta fuente de sincronización ya fue procesada.";
            return false;
        }

        ConvergenceSignalState signal = GetSignal(gameState.convergence, dimensionId);
        if (signal == null)
        {
            reason = "La señal dimensional no es válida.";
            return false;
        }

        int ownedCircuitCount = ConvergenceCircuitSystem.GetOwnedCircuitCount(gameState);
        if (!ConvergenceCircuitSystem.HasNextDesignedCircuit(gameState))
        {
            reason = "Contenido experimental de Convergencia completado. El siguiente circuito aún no está disponible.";
            return false;
        }
        // GetOwnedCircuitCount normaliza el estado y puede reconstruir la lista
        // de señales; vuelve a tomar la referencia antes de modificarla.
        signal = GetSignal(gameState.convergence, dimensionId);
        if (signal == null)
        {
            reason = "La señal dimensional no es válida.";
            return false;
        }
        double requirement = ConvergenceBalance.GetRequiredStabilityForNextCircuit(
            ownedCircuitCount
        );
        double currentStability = Math.Max(0.0, gameState.convergence.currentStability);
        bool signalWasActivated = signal.activated;
        bool canActivateSignal = activateSignal && !signalWasActivated;

        // Cuando el requisito ya está lleno, solo se conserva una fuente que
        // todavía desbloquea una señal ausente. Así no crece la lista de IDs
        // sin aportar progreso y la última dimensión puede cerrar el ciclo.
        if (currentStability >= requirement && !canActivateSignal)
        {
            reason = "La estabilidad ya alcanzó el requisito actual.";
            return false;
        }

        if (canActivateSignal)
            signal.activated = true;
        gameState.convergence.currentStability = Math.Min(
            requirement,
            currentStability + stabilityAmount
        );
        gameState.convergence.processedSynchronizationSourceIds.Add(sourceId);
        if (IsSynchronizationReadyForNextConvergence(gameState, ownedCircuitCount))
            ConvergenceTelemetrySystem.RecordReady(gameState);
        reason = "Sincronización registrada.";
        return true;
    }

    public static bool IsSignalActivated(GameState gameState, int dimensionId)
    {
        if (gameState == null)
            return false;

        gameState.EnsureConvergenceState();
        ConvergenceSignalState signal = GetSignal(gameState.convergence, dimensionId);
        return signal != null && signal.activated;
    }

    public static bool IsSynchronizationReadyForNextConvergence(
        GameState gameState,
        int ownedCircuitCount)
    {
        if (gameState == null)
            return false;

        gameState.EnsureConvergenceState();
        if (!gameState.convergence.dimensionalReceiverRebuilt ||
            !IsSignalActivated(gameState, 1) ||
            !IsSignalActivated(gameState, 2) ||
            !IsSignalActivated(gameState, 3))
        {
            return false;
        }

        return gameState.convergence.currentStability >=
            ConvergenceBalance.GetRequiredStabilityForNextCircuit(ownedCircuitCount);
    }

    public static void ResetCycleSynchronization(GameState gameState)
    {
        if (gameState == null)
            return;

        gameState.EnsureConvergenceState();
        gameState.convergence.dimensionalReceiverRebuilt = false;
        gameState.convergence.currentStability = 0.0;
        gameState.convergence.processedSynchronizationSourceIds.Clear();
        foreach (ConvergenceSignalState signal in gameState.convergence.signals)
            signal.activated = false;
    }

    private static ConvergenceSignalState GetSignal(
        ConvergenceState convergence,
        int dimensionId)
    {
        if (convergence == null || convergence.signals == null)
            return null;

        foreach (ConvergenceSignalState signal in convergence.signals)
        {
            if (signal != null && signal.dimensionId == dimensionId)
                return signal;
        }

        return null;
    }
}

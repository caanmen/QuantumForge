using System;

public static class ConvergenceTelemetrySystem
{
    public static long GetCurrentUnixSeconds()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public static void RecordOfflineElapsed(GameState gameState, double seconds)
    {
        if (gameState == null || seconds <= 0.0) return;
        gameState.EnsureConvergenceState();
        ConvergenceState state = gameState.convergence;
        state.cycleOfflineSeconds += seconds;
        if (state.receiverRebuiltUnix <= 0L) state.baseRebuildOfflineSeconds += seconds;
        else if (state.configurationStartedUnix <= 0L) state.synchronizationOfflineSeconds += seconds;
        else state.configurationOfflineSeconds += seconds;
    }

    public static void RecordReceiverRebuilt(GameState gameState)
    {
        if (gameState == null) return;
        gameState.EnsureConvergenceState();
        gameState.convergence.receiverRebuiltUnix = GetCurrentUnixSeconds();
    }

    public static void RecordReady(GameState gameState)
    {
        if (gameState == null) return;
        gameState.EnsureConvergenceState();
        if (gameState.convergence.configurationStartedUnix <= 0L)
            gameState.convergence.configurationStartedUnix = GetCurrentUnixSeconds();
    }

    public static void RecordConfigurationConfirmed(GameState gameState)
    {
        if (gameState == null) return;
        gameState.EnsureConvergenceState();
        ConvergenceState state = gameState.convergence;
        long now = GetCurrentUnixSeconds();
        if (state.cycleStartedUnix > 0L && state.receiverRebuiltUnix > 0L &&
            state.configurationStartedUnix > 0L)
        {
            double baseSeconds = Math.Max(0.0, state.receiverRebuiltUnix - state.cycleStartedUnix);
            double syncSeconds = Math.Max(0.0, state.configurationStartedUnix - state.receiverRebuiltUnix);
            double configSeconds = Math.Max(0.0, now - state.configurationStartedUnix);
            state.normalCycleMeasurements.Add(new ConvergenceCycleMeasurement
            {
                completedCycleNumber = state.completedCycles + 1,
                baseRebuildSeconds = baseSeconds,
                baseRebuildOfflineSeconds = state.baseRebuildOfflineSeconds,
                synchronizationSeconds = syncSeconds,
                synchronizationOfflineSeconds = state.synchronizationOfflineSeconds,
                configurationSeconds = configSeconds,
                configurationOfflineSeconds = state.configurationOfflineSeconds,
                endToEndCycleSeconds = Math.Max(0.0, now - state.cycleStartedUnix),
                offlineSeconds = state.cycleOfflineSeconds,
                onlineSeconds = Math.Max(0.0, now - state.cycleStartedUnix - state.cycleOfflineSeconds)
            });
        }
        state.cycleStartedUnix = now;
        state.receiverRebuiltUnix = 0L;
        state.configurationStartedUnix = 0L;
        state.cycleOfflineSeconds = 0.0;
        state.baseRebuildOfflineSeconds = 0.0;
        state.synchronizationOfflineSeconds = 0.0;
        state.configurationOfflineSeconds = 0.0;
    }
}

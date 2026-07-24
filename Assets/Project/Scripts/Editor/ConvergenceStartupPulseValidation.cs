#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ConvergenceStartupPulseValidation
{
    [MenuItem("Tools/Quantum Forge/Convergence/Validate Startup Pulse")]
    public static void ValidateStartupPulse()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("[Startup Pulse] Ejecutar fuera de Play Mode.");
            return;
        }

        var failures = new List<string>();
        ValidateAwardAndPersistence(failures);
        ValidateConnectionAndSnapshot(failures);
        ValidateProductionOnlineAndOffline(failures);
        ValidateConfigurationLock(failures);
        ValidateExperimentalBoardBounds(failures);

        if (failures.Count == 0)
        {
            Debug.Log("[Startup Pulse] PASS | concesión | placa | snapshot | LE | lock");
            return;
        }

        Debug.LogError("[Startup Pulse] FAIL\n- " + string.Join("\n- ", failures));
    }

    public static void ValidateStartupPulseBatch()
    {
        ValidateStartupPulse();
    }

    private static void ValidateAwardAndPersistence(List<string> failures)
    {
        GameState state = CreateSynchronizationReadyState("Startup Award State");
        try
        {
            string reason;
            Check(ConvergenceCircuitSystem.TryStartNormalConvergence(state, out reason) &&
                  state.convergence.ownedCircuits.Count == 1 &&
                  state.convergence.ownedCircuits[0].circuitId ==
                      ConvergenceCircuitCatalog.StartupPulseCircuitId &&
                  state.convergence.phase == ConvergencePhase.ConfigurationPending,
                "La primera Convergencia no entrega exactamente Pulso de Arranque.",
                failures);

            string json = JsonUtility.ToJson(state.convergence);
            ConvergenceState loaded = JsonUtility.FromJson<ConvergenceState>(json);
            state.convergence = loaded;
            state.EnsureConvergenceState();
            Check(ConvergenceCircuitSystem.TryStartNormalConvergence(state, out reason) &&
                  state.convergence.ownedCircuits.Count == 1,
                "Cargar durante ConfigurationPending duplica el circuito.", failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateConnectionAndSnapshot(List<string> failures)
    {
        GameState powered = CreatePendingConfigurationState("Powered Startup Pulse");
        GameState rotated = CreatePendingConfigurationState("Rotated Startup Pulse");
        GameState disconnected = CreatePendingConfigurationState("Disconnected Startup Pulse");
        try
        {
            string reason;
            ConvergenceCircuitSystem.TryPlaceCircuit(powered,
                ConvergenceCircuitCatalog.StartupPulseCircuitId, 0, 1, 0, out reason);
            ConvergenceCircuitSystem.TryConfirmConfiguration(powered, out reason);
            Check(ConvergenceCircuitSystem.GetBaseLEProductionMultiplier(powered) == 1.10,
                "Pulso conectado no genera exactamente ×1.10.", failures);

            ConvergenceCircuitSystem.TryPlaceCircuit(rotated,
                ConvergenceCircuitCatalog.StartupPulseCircuitId, 0, 1, 90, out reason);
            ConvergenceCircuitSystem.TryConfirmConfiguration(rotated, out reason);
            Check(ConvergenceCircuitSystem.GetBaseLEProductionMultiplier(rotated) == 1.0,
                "Pulso girado hacia una dirección incorrecta queda energizado.", failures);

            ConvergenceCircuitSystem.TryPlaceCircuit(disconnected,
                ConvergenceCircuitCatalog.StartupPulseCircuitId, 0, 2, 0, out reason);
            ConvergenceCircuitSystem.TryConfirmConfiguration(disconnected, out reason);
            Check(ConvergenceCircuitSystem.GetBaseLEProductionMultiplier(disconnected) == 1.0,
                "Pulso sin ruta al Núcleo modifica la producción.", failures);
        }
        finally
        {
            Object.DestroyImmediate(powered.gameObject);
            Object.DestroyImmediate(rotated.gameObject);
            Object.DestroyImmediate(disconnected.gameObject);
        }
    }

    private static void ValidateProductionOnlineAndOffline(List<string> failures)
    {
        GameState online = CreatePoweredNewCycleState("Online Startup Pulse");
        GameState offline = CreatePoweredNewCycleState("Offline Startup Pulse");
        try
        {
            online.baseLEps = 10.0;
            online.LE = 0.0;
            online.Tick(1.0);
            offline.baseLEps = 10.0;
            offline.LE = 0.0;
            offline.ApplyOfflineBaseProgress(1.0);
            Check(System.Math.Abs(online.LE - 11.0) < 0.0001 &&
                  System.Math.Abs(offline.LE - 11.0) < 0.0001,
                "El bonus de LE no coincide una sola vez online y offline.",
                failures);
        }
        finally
        {
            Object.DestroyImmediate(online.gameObject);
            Object.DestroyImmediate(offline.gameObject);
        }
    }

    private static void ValidateConfigurationLock(List<string> failures)
    {
        GameState state = CreatePoweredNewCycleState("Locked Startup Pulse");
        try
        {
            string reason;
            Check(state.convergence.boardConfigurationLocked &&
                  state.convergence.completedCycles == 1 &&
                  !ConvergenceCircuitSystem.TryPlaceCircuit(state,
                      ConvergenceCircuitCatalog.StartupPulseCircuitId, 1, 0, 90,
                      out reason),
                "La placa no queda bloqueada después de estabilizar.", failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateExperimentalBoardBounds(List<string> failures)
    {
        GameState state = CreatePendingConfigurationState("Board Bounds State");
        try
        {
            string reason;
            Check(!ConvergenceCircuitSystem.TryPlaceCircuit(state,
                      ConvergenceCircuitCatalog.StartupPulseCircuitId, 3, 0, 90,
                      out reason),
                "La placa experimental permite coordenadas fuera de 5×5.", failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static GameState CreatePendingConfigurationState(string name)
    {
        GameState state = CreateSynchronizationReadyState(name);
        string reason;
        ConvergenceCircuitSystem.TryStartNormalConvergence(state, out reason);
        return state;
    }

    private static GameState CreatePoweredNewCycleState(string name)
    {
        GameState state = CreatePendingConfigurationState(name);
        string reason;
        ConvergenceCircuitSystem.TryPlaceCircuit(state,
            ConvergenceCircuitCatalog.StartupPulseCircuitId, 0, 1, 0, out reason);
        ConvergenceCircuitSystem.TryConfirmConfiguration(state, out reason);
        return state;
    }

    private static GameState CreateSynchronizationReadyState(string name)
    {
        var gameObject = new GameObject(name)
        {
            hideFlags = HideFlags.HideAndDontSave
        };
        gameObject.SetActive(false);
        GameState state = gameObject.AddComponent<GameState>();
        state.dimension01Unlocked = true;
        state.dimension02Unlocked = true;
        state.dimension03Unlocked = true;
        state.dimension1GalacticAnchorDiscovered = true;
        state.dimension2 = Dimension2System.CreateInitialState();
        state.dimension2.civilization2.majorPactEstablished = true;
        state.dimension3 = Dimension3System.CreateInitialState();
        state.dimension3.autonomyCoreIntegrated = true;
        state.convergence = ConvergenceSystem.CreateInitialState();
        state.convergence.dimensionalReceiverRebuilt = true;
        foreach (ConvergenceSignalState signal in state.convergence.signals)
            signal.activated = true;
        state.convergence.currentStability = 120.0;
        return state;
    }

    private static void Check(bool condition, string message, List<string> failures)
    {
        if (!condition)
            failures.Add(message);
    }
}
#endif

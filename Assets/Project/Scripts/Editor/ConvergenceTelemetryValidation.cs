#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ConvergenceTelemetryValidation
{
    [MenuItem("Tools/Quantum Forge/Convergence/Validate Telemetry")]
    public static void ValidateTelemetry()
    {
        var failures = new List<string>();
        var go = new GameObject("Convergence Telemetry Validation")
        {
            hideFlags = HideFlags.HideAndDontSave
        };
        try
        {
            GameState state = go.AddComponent<GameState>();
            state.convergence = ConvergenceSystem.CreateInitialState();
            long now = ConvergenceTelemetrySystem.GetCurrentUnixSeconds();
            state.convergence.cycleStartedUnix = now - 100;
            state.convergence.receiverRebuiltUnix = now - 70;
            state.convergence.configurationStartedUnix = now - 10;
            state.convergence.cycleOfflineSeconds = 40.0;
            state.convergence.baseRebuildOfflineSeconds = 12.0;
            state.convergence.synchronizationOfflineSeconds = 23.0;
            state.convergence.configurationOfflineSeconds = 5.0;

            ConvergenceTelemetrySystem.RecordConfigurationConfirmed(state);
            ConvergenceCycleMeasurement record =
                state.convergence.normalCycleMeasurements[0];
            Check(record.baseRebuildSeconds == 30.0 &&
                  record.synchronizationSeconds == 60.0 &&
                  record.configurationSeconds >= 10.0 &&
                  record.endToEndCycleSeconds >= 100.0,
                "Los límites temporales del ciclo no se registran correctamente.", failures);
            Check(record.offlineSeconds == 40.0 && record.onlineSeconds >= 60.0 &&
                  record.baseRebuildOfflineSeconds == 12.0 &&
                  record.synchronizationOfflineSeconds == 23.0 &&
                  record.configurationOfflineSeconds == 5.0,
                "La separación entre tiempo online y offline no se conserva.", failures);
            ConvergenceState loaded = JsonUtility.FromJson<ConvergenceState>(
                JsonUtility.ToJson(state.convergence));
            Check(loaded != null && loaded.normalCycleMeasurements != null &&
                  loaded.normalCycleMeasurements.Count == 1,
                "La telemetría no sobrevive al guardado JSON.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(go); }

        if (failures.Count == 0)
            Debug.Log("[Convergence Telemetry] PASS | tramos | online/offline | save");
        else
            Debug.LogError("[Convergence Telemetry] FAIL\n- " +
                string.Join("\n- ", failures));
    }

    public static void ValidateTelemetryBatch() { ValidateTelemetry(); }

    private static void Check(bool condition, string message, List<string> failures)
    {
        if (!condition) failures.Add(message);
    }
}
#endif

#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ConvergenceStateValidation
{
    [MenuItem("Tools/Quantum Forge/Convergence/Validate Minimum State")]
    public static void ValidateMinimumState()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("[Convergence State] Ejecutar fuera de Play Mode.");
            return;
        }

        var failures = new List<string>();
        ValidateInitialState(failures);
        ValidateLegacySaveState(failures);
        ValidateNormalization(failures);
        ValidateSaveRoundTrip(failures);
        ValidateV3MigrationAndFutureGuard(failures);
        ValidateAtomicSaveAndBackup(failures);

        if (failures.Count == 0)
        {
            Debug.Log(
                "[Convergence State] PASS | inicial | legado | normalización | save"
            );
            return;
        }

        Debug.LogError("[Convergence State] FAIL\n- " +
            string.Join("\n- ", failures));
    }

    public static void ValidateMinimumStateBatch()
    {
        ValidateMinimumState();
    }

    private static void ValidateInitialState(List<string> failures)
    {
        ConvergenceState state = ConvergenceSystem.CreateInitialState();
        Check(state.progressVersion == ConvergenceSystem.ProgressVersion &&
              state.phase == ConvergencePhase.Inactive &&
              state.completedCycles == 0,
            "El estado inicial no parte inactivo y sin ciclos.", failures);
    }

    private static void ValidateLegacySaveState(List<string> failures)
    {
        GameState state = CreateState("Legacy Convergence State");
        try
        {
            state.convergence = null;
            state.EnsureConvergenceState();
            Check(state.convergence != null &&
                  state.convergence.phase == ConvergencePhase.Inactive &&
                  state.convergence.completedCycles == 0,
                "Un save legado sin ConvergenceState no recibe un estado seguro.",
                failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateNormalization(List<string> failures)
    {
        GameState state = CreateState("Invalid Convergence State");
        try
        {
            state.convergence = new ConvergenceState
            {
                progressVersion = 3,
                phase = (ConvergencePhase)99,
                completedCycles = -3
            };
            state.EnsureConvergenceState();
            Check(state.convergence.progressVersion == ConvergenceSystem.ProgressVersion &&
                  state.convergence.phase == ConvergencePhase.Inactive &&
                  state.convergence.completedCycles == 0,
                "La normalización no recupera un estado corrupto.", failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateSaveRoundTrip(List<string> failures)
    {
        var save = new SaveData
        {
            convergence = new ConvergenceState
            {
                progressVersion = ConvergenceSystem.ProgressVersion,
                phase = ConvergencePhase.ConfigurationPending,
                completedCycles = 4
            }
        };
        SaveData loaded = JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(save));
        Check(loaded != null && loaded.convergence != null &&
              loaded.convergence.phase == ConvergencePhase.ConfigurationPending &&
              loaded.convergence.completedCycles == 4,
            "El save no conserva la fase y los ciclos de Convergencia.", failures);
    }

    private static void ValidateV3MigrationAndFutureGuard(List<string> failures)
    {
        GameState state = CreateState("Convergence V3 Migration State");
        try
        {
            state.convergence = new ConvergenceState
            {
                progressVersion = 3,
                phase = ConvergencePhase.ConfigurationPending,
                ownedCircuits = new List<OwnedConvergenceCircuit>
                {
                    new OwnedConvergenceCircuit { circuitId = ConvergenceCircuitCatalog.StartupPulseCircuitId, obtained = true }
                },
                boardPlacements = new List<ConvergenceCircuitPlacement>
                {
                    new ConvergenceCircuitPlacement { circuitId = ConvergenceCircuitCatalog.StartupPulseCircuitId, x = 0, y = 1, rotationDegrees = 0 }
                },
                modifierSnapshot = new ConvergenceModifierSnapshot { baseLEProductionMultiplier = 1.10 }
            };
            state.EnsureConvergenceState();
            Check(state.convergence.progressVersion == 4 && state.convergence.draftPlacements.Count == 1 &&
                  System.Math.Abs(state.convergence.activeSnapshot.baseLEProductionMultiplier - 1.10) < 0.0001,
                "La migraciÃ³n v3 no separa borrador y snapshot activo.", failures);

            state.convergence.progressVersion = 99;
            state.convergence.completedCycles = 42;
            state.EnsureConvergenceState();
            Check(state.convergence.progressVersion == 99 && state.convergence.completedCycles == 42,
                "Un save futuro se rebaja o normaliza indebidamente.", failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateAtomicSaveAndBackup(List<string> failures)
    {
        string folder = Path.Combine(Path.GetTempPath(), "QuantumForgeConvergenceValidation_" + System.Guid.NewGuid().ToString("N"));
        string path = Path.Combine(folder, "save.json");
        try
        {
            Check(SaveService.TryWriteAtomicJson(path, JsonUtility.ToJson(new SaveData { LE = 10.0 }), out _), "No se pudo crear el save atÃ³mico inicial.", failures);
            Check(SaveService.TryWriteAtomicJson(path, JsonUtility.ToJson(new SaveData { LE = 20.0 }), out _), "No se pudo reemplazar el save atÃ³mico.", failures);
            Check(SaveService.TryReadSaveData(path + ".bak", out SaveData backup) && backup.LE == 10.0, "El backup no conserva el save anterior.", failures);
            foreach (SaveFailureInjectionPoint point in new[]
            {
                SaveFailureInjectionPoint.BeforeTempWrite,
                SaveFailureInjectionPoint.AfterTempWrite,
                SaveFailureInjectionPoint.AfterTempValidation,
                SaveFailureInjectionPoint.BeforeReplace
            })
            {
                SaveService.FailureInjectionPoint = point;
                Check(!SaveService.TryWriteAtomicJson(path,
                    JsonUtility.ToJson(new SaveData { LE = 30.0 }), out _),
                    "El fallo inyectado " + point + " no interrumpe el reemplazo.", failures);
                Check(SaveService.TryReadSaveData(path, out SaveData current) && current.LE == 20.0,
                    "El fallo inyectado " + point + " altera el save confirmado.", failures);
            }
        }
        finally
        {
            SaveService.FailureInjectionPoint = SaveFailureInjectionPoint.None;
            if (Directory.Exists(folder)) Directory.Delete(folder, true);
        }
    }

    private static GameState CreateState(string name)
    {
        var gameObject = new GameObject(name)
        {
            hideFlags = HideFlags.HideAndDontSave
        };
        gameObject.SetActive(false);
        return gameObject.AddComponent<GameState>();
    }

    private static void Check(bool condition, string message, List<string> failures)
    {
        if (!condition)
            failures.Add(message);
    }
}
#endif

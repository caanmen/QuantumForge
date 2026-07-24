#if UNITY_EDITOR
using System.Collections.Generic;
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
                progressVersion = 99,
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

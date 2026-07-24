#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class DimensionDiscoveryMigrationValidation
{
    [MenuItem("Tools/Quantum Forge/Convergence/Validate Discovery Migration")]
    public static void ValidateDiscoveryMigration()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("[Discovery Migration] Ejecutar fuera de Play Mode.");
            return;
        }

        var failures = new List<string>();
        ValidateLegacyAllDimensionsCase(failures);
        ValidateSingleDimensionCase(failures);
        ValidateTwoDimensionCase(failures);
        ValidateThreeDimensionCase(failures);
        ValidateFreshModernSaveCase(failures);
        ValidateSaveVersionRoundTrip(failures);

        if (failures.Count == 0)
        {
            Debug.Log(
                "[Discovery Migration] PASS | legado | 1D | 2D | 3D | nueva | versión"
            );
            return;
        }

        Debug.LogError("[Discovery Migration] FAIL\n- " +
            string.Join("\n- ", failures));
    }

    public static void ValidateDiscoveryMigrationBatch()
    {
        ValidateDiscoveryMigration();
    }

    private static void ValidateLegacyAllDimensionsCase(List<string> failures)
    {
        GameState state = CreateState("Legacy Discovery Migration");
        try
        {
            state.hasDonePrestige1 = true;
            state.prestige1Count = 1;
            SaveService.ApplyDimensionDiscoveryMigration(state, 0);

            Check(AllUnlocked(state) && state.prestige1CurrentDimensionId == 0,
                "Save legado sin flags no migra a las tres dimensiones descubiertas.",
                failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateSingleDimensionCase(List<string> failures)
    {
        GameState state = CreateState("Single Discovery Migration");
        try
        {
            state.hasDonePrestige1 = true;
            state.dimension02Unlocked = true;
            SaveService.ApplyDimensionDiscoveryMigration(state, 1);

            Check(!state.dimension01Unlocked && state.dimension02Unlocked &&
                  !state.dimension03Unlocked &&
                  state.prestige1CurrentDimensionId == 2,
                "Save moderno con una dimensión no conserva exactamente D2.",
                failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateTwoDimensionCase(List<string> failures)
    {
        GameState state = CreateState("Two Discovery Migration");
        try
        {
            state.hasDonePrestige1 = true;
            state.dimension01Unlocked = true;
            state.dimension03Unlocked = true;
            state.dimension1GalacticAnchorDiscovered = true;
            Dimension3System.EnsureState(state);
            state.dimension3.autonomyCoreIntegrated = false;
            SaveService.ApplyDimensionDiscoveryMigration(state, 1);

            Check(state.dimension01Unlocked && !state.dimension02Unlocked &&
                  state.dimension03Unlocked &&
                  state.prestige1CurrentDimensionId == 3,
                "Save moderno con dos dimensiones no conserva flags o hito pendiente.",
                failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateThreeDimensionCase(List<string> failures)
    {
        GameState state = CreateState("Three Discovery Migration");
        try
        {
            state.hasDonePrestige1 = true;
            state.dimension01Unlocked = true;
            state.dimension02Unlocked = true;
            state.dimension03Unlocked = true;
            state.prestige1CurrentDimensionId = 2;
            SaveService.ApplyDimensionDiscoveryMigration(state, 1);

            Check(AllUnlocked(state) && state.prestige1CurrentDimensionId == 0,
                "Save moderno con tres dimensiones no cierra la selección antigua.",
                failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateFreshModernSaveCase(List<string> failures)
    {
        GameState state = CreateState("Fresh Discovery Migration");
        try
        {
            SaveService.ApplyDimensionDiscoveryMigration(state, 1);
            Check(!state.dimension01Unlocked && !state.dimension02Unlocked &&
                  !state.dimension03Unlocked &&
                  state.prestige1CurrentDimensionId == 0,
                "Partida nueva moderna revela dimensiones sin Prestigio.", failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateSaveVersionRoundTrip(List<string> failures)
    {
        var current = new SaveData
        {
            dimensionDiscoverySaveVersion = 1,
            dimension01Unlocked = true,
            dimension03Unlocked = true
        };
        SaveData roundTrip = JsonUtility.FromJson<SaveData>(
            JsonUtility.ToJson(current)
        );
        Check(roundTrip != null && roundTrip.dimensionDiscoverySaveVersion == 1 &&
              roundTrip.dimension01Unlocked && !roundTrip.dimension02Unlocked &&
              roundTrip.dimension03Unlocked,
            "El save no conserva versión y flags dimensionales exactas.", failures);
    }

    private static bool AllUnlocked(GameState state)
    {
        return state.dimension01Unlocked && state.dimension02Unlocked &&
            state.dimension03Unlocked;
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

    private static void Check(
        bool condition,
        string message,
        List<string> failures)
    {
        if (!condition)
            failures.Add(message);
    }
}
#endif

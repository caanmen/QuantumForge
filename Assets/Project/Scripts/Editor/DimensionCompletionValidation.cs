#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class DimensionCompletionValidation
{
    [MenuItem("Tools/Quantum Forge/Convergence/Validate Dimension Completion")]
    public static void ValidateDimensionCompletion()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("[Dimension Completion] Ejecutar fuera de Play Mode.");
            return;
        }

        var failures = new List<string>();
        GameState state = CreateCompletedState();
        try
        {
            ValidateAllCompleted(state, failures);
            ValidateEachCanonicalMilestone(state, failures);
            ValidateUnlockedRequirement(state, failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }

        if (failures.Count == 0)
        {
            Debug.Log("[Dimension Completion] PASS | D1 | D2 | D3 | conjunto");
            return;
        }

        Debug.LogError("[Dimension Completion] FAIL\n- " +
            string.Join("\n- ", failures));
    }

    public static void ValidateDimensionCompletionBatch()
    {
        ValidateDimensionCompletion();
    }

    private static void ValidateAllCompleted(GameState state, List<string> failures)
    {
        Check(DimensionCompletionService.IsDimensionCompleted(state, 1) &&
              DimensionCompletionService.IsDimensionCompleted(state, 2) &&
              DimensionCompletionService.IsDimensionCompleted(state, 3) &&
              DimensionCompletionService.AreAllDimensionsCompleted(state) &&
              state.IsPrestige1CycleComplete() &&
              ConvergenceCircuitSystem.IsConvergenceUnlocked(state),
            "Los tres hitos canónicos no completan el ciclo conjunto.", failures);
    }

    private static void ValidateEachCanonicalMilestone(
        GameState state,
        List<string> failures)
    {
        state.dimension1GalacticAnchorDiscovered = false;
        Check(!DimensionCompletionService.IsDimensionCompleted(state, 1) &&
              !DimensionCompletionService.AreAllDimensionsCompleted(state),
            "D1 no depende del Ancla Galáctica canónica.", failures);
        state.dimension1GalacticAnchorDiscovered = true;

        state.dimension2.civilization2.majorPactEstablished = false;
        Check(!DimensionCompletionService.IsDimensionCompleted(state, 2) &&
              !DimensionCompletionService.AreAllDimensionsCompleted(state),
            "D2 no depende del Pacto Mayor canónico.", failures);
        state.dimension2.civilization2.majorPactEstablished = true;

        state.dimension3.autonomyCoreIntegrated = false;
        Check(!DimensionCompletionService.IsDimensionCompleted(state, 3) &&
              !DimensionCompletionService.AreAllDimensionsCompleted(state),
            "D3 no depende del Núcleo de Autonomía canónico.", failures);
        state.dimension3.autonomyCoreIntegrated = true;
    }

    private static void ValidateUnlockedRequirement(
        GameState state,
        List<string> failures)
    {
        state.dimension02Unlocked = false;
        Check(!DimensionCompletionService.IsDimensionCompleted(state, 2),
            "Un hito D2 cuenta aunque la dimensión no esté descubierta.", failures);
        state.dimension02Unlocked = true;
    }

    private static GameState CreateCompletedState()
    {
        var gameObject = new GameObject("Dimension Completion Validation")
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
        return state;
    }

    private static void Check(bool condition, string message, List<string> failures)
    {
        if (!condition)
            failures.Add(message);
    }
}
#endif

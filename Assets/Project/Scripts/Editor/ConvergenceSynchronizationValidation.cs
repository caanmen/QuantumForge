#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ConvergenceSynchronizationValidation
{
    [MenuItem("Tools/Quantum Forge/Convergence/Validate Synchronization State")]
    public static void ValidateSynchronizationState()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("[Convergence Sync] Ejecutar fuera de Play Mode.");
            return;
        }

        var failures = new List<string>();
        ValidateLegacyNormalization(failures);
        ValidateReceiverAndSynchronization(failures);
        ValidateCycleReset(failures);
        ValidateTentativeRequirement(failures);
        ValidateStabilityCap(failures);

        if (failures.Count == 0)
        {
            Debug.Log("[Convergence Sync] PASS | migración | receptor | fuentes | reset");
            return;
        }

        Debug.LogError("[Convergence Sync] FAIL\n- " +
            string.Join("\n- ", failures));
    }

    public static void ValidateSynchronizationStateBatch()
    {
        ValidateSynchronizationState();
    }

    private static void ValidateLegacyNormalization(List<string> failures)
    {
        GameState state = CreateCompletedState("Legacy Synchronization State");
        try
        {
            state.convergence = new ConvergenceState
            {
                progressVersion = 1,
                currentStability = double.NaN,
                signals = null,
                processedSynchronizationSourceIds = null
            };
            state.EnsureConvergenceState();
            Check(state.convergence.progressVersion == ConvergenceSystem.ProgressVersion &&
                  state.convergence.signals.Count == 3 &&
                  state.convergence.currentStability == 0.0 &&
                  state.convergence.processedSynchronizationSourceIds.Count == 0,
                "El save anterior no migra al estado mínimo de sincronización.",
                failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateReceiverAndSynchronization(List<string> failures)
    {
        GameState state = CreateCompletedState("Synchronization State");
        try
        {
            string reason;
            Check(!ConvergenceSynchronizationSystem.TryAddSynchronization(
                      state, 1, true, 1.0, "d1_before_receiver", out reason),
                "Acepta sincronización antes de reconstruir el Receptor.", failures);
            Check(ConvergenceSynchronizationSystem.TryRebuildReceiver(state, out reason),
                "No permite reconstruir el Receptor con las tres dimensiones completas.",
                failures);
            Check(ConvergenceSynchronizationSystem.TryAddSynchronization(
                      state, 1, true, 12.5, "d1_final_activity", out reason) &&
                  ConvergenceSynchronizationSystem.IsSignalActivated(state, 1) &&
                  state.convergence.currentStability == 12.5,
                "No registra una activación y estabilidad válidas.", failures);
            Check(!ConvergenceSynchronizationSystem.TryAddSynchronization(
                      state, 1, true, 12.5, "d1_final_activity", out reason) &&
                  state.convergence.currentStability == 12.5,
                "Una fuente de sincronización se acredita dos veces.", failures);
            Check(ConvergenceSynchronizationSystem.TryAddSynchronization(
                      state, 2, false, 3.0, "d2_repeatable_activity", out reason) &&
                  state.convergence.currentStability == 15.5,
                "No acepta estabilidad posterior desde otra dimensión completa.",
                failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateCycleReset(List<string> failures)
    {
        GameState state = CreateCompletedState("Synchronization Reset State");
        try
        {
            string reason;
            ConvergenceSynchronizationSystem.TryRebuildReceiver(state, out reason);
            ConvergenceSynchronizationSystem.TryAddSynchronization(
                state, 3, true, 7.0, "d3_activity", out reason);
            ConvergenceSynchronizationSystem.ResetCycleSynchronization(state);
            Check(!state.convergence.dimensionalReceiverRebuilt &&
                  !ConvergenceSynchronizationSystem.IsSignalActivated(state, 3) &&
                  state.convergence.currentStability == 0.0 &&
                  state.convergence.processedSynchronizationSourceIds.Count == 0,
                "El nuevo ciclo conserva sincronización temporal.", failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateTentativeRequirement(List<string> failures)
    {
        Check(ConvergenceBalance.GetRequiredStabilityForNextCircuit(0) == 120.0 &&
              ConvergenceBalance.GetRequiredStabilityForNextCircuit(1) == 289.0 &&
              ConvergenceBalance.GetRequiredStabilityForNextCircuit(2) == 530.0 &&
              ConvergenceBalance.GetRequiredStabilityForNextCircuit(3) == 843.0,
            "La fórmula experimental de estabilidad no produce la progresión acordada.",
            failures);

        GameState state = CreateCompletedState("Synchronization Requirement State");
        try
        {
            string reason;
            ConvergenceSynchronizationSystem.TryRebuildReceiver(state, out reason);
            ConvergenceSynchronizationSystem.TryAddSynchronization(
                state, 1, true, 40.0, "d1_activation", out reason);
            ConvergenceSynchronizationSystem.TryAddSynchronization(
                state, 2, true, 40.0, "d2_activation", out reason);
            ConvergenceSynchronizationSystem.TryAddSynchronization(
                state, 3, true, 40.0, "d3_activation", out reason);
            Check(ConvergenceSynchronizationSystem.IsSynchronizationReadyForNextConvergence(
                      state, 0),
                "Tres señales activas y el requisito inicial no habilitan la Convergencia.",
                failures);
            Check(!ConvergenceSynchronizationSystem.IsSynchronizationReadyForNextConvergence(
                      state, 1),
                "El requisito no crece al poseer circuitos.", failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateStabilityCap(List<string> failures)
    {
        Check(ConvergenceBalance.GetStabilityForBaseWorkSeconds(25.0) == 5.0,
            "La conversión de cinco segundos por punto no es estable.", failures);

        GameState state = CreateCompletedState("Synchronization Cap State");
        try
        {
            string reason;
            ConvergenceSynchronizationSystem.TryRebuildReceiver(state, out reason);
            Check(ConvergenceSynchronizationSystem.TryAddSynchronization(
                      state, 1, true, 1000.0, "d1_cap", out reason) &&
                  state.convergence.currentStability == 120.0,
                "La estabilidad no se limita al requisito actual.", failures);
            Check(!ConvergenceSynchronizationSystem.TryAddSynchronization(
                      state, 1, false, 1.0, "d1_after_cap", out reason) &&
                  state.convergence.processedSynchronizationSourceIds.Count == 1,
                "El límite sigue guardando fuentes que ya no aportan.", failures);
            Check(ConvergenceSynchronizationSystem.TryAddSynchronization(
                      state, 2, true, 0.0, "d2_signal_at_cap", out reason) &&
                  ConvergenceSynchronizationSystem.IsSignalActivated(state, 2) &&
                  state.convergence.currentStability == 120.0,
                "El límite impide activar una señal pendiente.", failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static GameState CreateCompletedState(string name)
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
        return state;
    }

    private static void Check(bool condition, string message, List<string> failures)
    {
        if (!condition)
            failures.Add(message);
    }
}
#endif

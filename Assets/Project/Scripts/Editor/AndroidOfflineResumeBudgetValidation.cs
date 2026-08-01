#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class AndroidOfflineResumeBudgetValidation
{
    private const double MaximumOfflineSeconds = 12.0 * 60.0 * 60.0;

    [MenuItem("Tools/Quantum Forge/QA/Measure Android Offline Resume")]
    public static void Validate()
    {
        GameState previousState = GameState.I;
        GameObject target = new GameObject("QA Android Offline Budget")
            { hideFlags = HideFlags.HideAndDontSave };
        target.SetActive(false);
        GameState state = target.AddComponent<GameState>();

        try
        {
            SetGameStateSingleton(state);
            state.LE = 1000.0;
            state.EnsureDimension1State();
            state.EnsureDimension2State();
            state.EnsureDimension3State();
            state.EnsureConvergenceState();

            Stopwatch stopwatch = Stopwatch.StartNew();
            TriangleOfflineReport report =
                state.ApplyOfflineBaseProgress(MaximumOfflineSeconds);
            stopwatch.Stop();

            Require(report != null &&
                    Math.Abs(report.appliedSeconds - MaximumOfflineSeconds) <
                        0.000001,
                "El progreso base no aplico exactamente el limite de 12 horas.");
            Require(stopwatch.Elapsed.TotalSeconds < 0.25,
                "El progreso base de 12 horas tarda " +
                stopwatch.Elapsed.TotalSeconds.ToString("0.000") +
                " s en el Editor y puede bloquear un telefono.");

            UnityEngine.Debug.Log(
                "[Android Offline Resume Budget] PASS | 12 h exactas | " +
                "tiempo=" + stopwatch.Elapsed.TotalMilliseconds.ToString("0.0") +
                " ms | sin espera prolongada");
        }
        finally
        {
            SetGameStateSingleton(previousState);
            UnityEngine.Object.DestroyImmediate(target);
        }
    }

    public static void ValidateBatch()
    {
        try
        {
            Validate();
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            UnityEngine.Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    private static void SetGameStateSingleton(GameState state)
    {
        PropertyInfo property = typeof(GameState).GetProperty(
            "I", BindingFlags.Public | BindingFlags.Static);
        property?.SetValue(null, state, null);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
#endif

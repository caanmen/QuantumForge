#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class AndroidPauseResumeValidation
{
    [MenuItem("Tools/Quantum Forge/QA/Validate Android Pause Resume")]
    public static void Validate()
    {
        GameState previousState = GameState.I;
        GameObject stateObject = new GameObject("QA Android Resume State")
            { hideFlags = HideFlags.HideAndDontSave };
        stateObject.SetActive(false);
        GameState state = stateObject.AddComponent<GameState>();

        GameObject tickObject = new GameObject("QA Android Resume Tick")
            { hideFlags = HideFlags.HideAndDontSave };
        tickObject.SetActive(false);
        TickSystem tick = tickObject.AddComponent<TickSystem>();

        GameObject machineObject = new GameObject("QA Android Resume Machine")
            { hideFlags = HideFlags.HideAndDontSave };
        machineObject.SetActive(false);
        MachineManager machine = machineObject.AddComponent<MachineManager>();

        try
        {
            SetGameStateSingleton(state);
            QaRuntimeService.ResetToNormalSpeed();
            state.LE = 0.0;
            state.baseLEps = 1.0;

            SetField(tick, "_step", 0.1f);
            SetField(tick, "_acc", 0.0f);
            MethodInfo advanceFrame = typeof(TickSystem).GetMethod(
                "AdvanceFrame", BindingFlags.NonPublic | BindingFlags.Instance);
            Require(advanceFrame != null,
                "No se encontro el avance protegido de TickSystem.");

            // Simula un primer fotograma con una hora acumulada al volver de
            // segundo plano. Debe procesar una ventana acotada, no 36 000 ticks.
            advanceFrame.Invoke(tick, new object[] { 3600.0f });
            Require(state.LE >= 0.0 && state.LE <= 2.000001,
                "La reanudacion intento convertir una hora en ticks de un frame.");

            tick.ResetAccumulator();
            float accumulator = (float)GetField(tick, "_acc");
            Require(Math.Abs(accumulator) < 0.000001f,
                "El acumulador no se limpia al reanudar.");

            SetField(machine, "_analysisNodeId", "qa_resume_node");
            SetField(machine, "_analysisRemainingSeconds", 100.0);
            MethodInfo advanceMachineFrame = typeof(MachineManager).GetMethod(
                "AdvanceOnlineFrame",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Require(advanceMachineFrame != null,
                "No se encontro el avance protegido de MachineManager.");
            advanceMachineFrame.Invoke(machine, new object[] { 3600.0f });
            Require(machine.AnalysisRemainingSeconds >= 99.49 &&
                    machine.AnalysisRemainingSeconds <= 100.0,
                "La Maquina acredito como online el tiempo en segundo plano.");

            Debug.Log("[Android Pause Resume] PASS | delta de 1 h acotado | " +
                "sin bucle masivo | acumulador limpio | Maquina acotada | " +
                "progreso offline separado");
        }
        finally
        {
            QaRuntimeService.ResetToNormalSpeed();
            SetGameStateSingleton(previousState);
            UnityEngine.Object.DestroyImmediate(machineObject);
            UnityEngine.Object.DestroyImmediate(tickObject);
            UnityEngine.Object.DestroyImmediate(stateObject);
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
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    private static void SetGameStateSingleton(GameState state)
    {
        PropertyInfo property = typeof(GameState).GetProperty(
            "I", BindingFlags.Public | BindingFlags.Static);
        property?.SetValue(null, state, null);
    }

    private static void SetField(object target, string name, object value)
    {
        target.GetType().GetField(name,
            BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(target, value);
    }

    private static object GetField(object target, string name)
    {
        return target.GetType().GetField(name,
            BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(target);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
#endif

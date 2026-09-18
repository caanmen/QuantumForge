#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class QaBlock4Validation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";

    [MenuItem("Tools/Quantum Forge/QA/Validate Block 4")]
    public static void ValidateBlock4()
    {
        var failures = new List<string>();
        GameState previousState = GameState.I;
        FieldInfo availabilityOverride = typeof(QaRuntimeService).GetField(
            "availabilityOverrideForValidation",
            BindingFlags.NonPublic | BindingFlags.Static);

        try
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            QaFastForwardRunner[] runners =
                UnityEngine.Object.FindObjectsByType<QaFastForwardRunner>(
                    FindObjectsInactive.Include, FindObjectsSortMode.None);
            Check(runners.Length == 1,
                "QaFastForwardRunner está ausente o duplicado.", failures);
            if (runners.Length != 1)
                Finish(failures);

            QaFastForwardRunner runner = runners[0];
            Check(runner.panel != null &&
                runner.panel.operationStatusText != null,
                "El runner no está conectado al estado visible del panel.", failures);

            ValidateSourceContract(failures);
            ValidateHudPrecision(failures);
            ValidateExactDeterministicSteps(failures);
            ValidateAggregateOfflineAdvance(failures);
            ValidateMachineAndCooldownStep(failures);
            ValidateConcurrentGuard(runner, availabilityOverride, failures);
        }
        finally
        {
            SetGameStateSingleton(previousState);
            if (availabilityOverride != null)
                availabilityOverride.SetValue(null, null);
            QaRuntimeService.ResetToNormalSpeed();
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        }

        Finish(failures);
    }

    private static void ValidateHudPrecision(List<string> failures)
    {
        string source = File.ReadAllText(
            "Assets/Project/Scripts/UI/HUD.cs");
        Check(source.Contains("public void RefreshNow()") &&
            source.Contains("{gs.LE:0}") &&
            source.Contains("{gs.Traces:0}") &&
            !source.Contains("(float)gs.LE") &&
            !source.Contains("(float)gs.Traces"),
            "El HUD todavía pierde precisión o no ofrece refresco inmediato.",
            failures);
    }

    private static void ValidateSourceContract(List<string> failures)
    {
        string source = File.ReadAllText(
            "Assets/Project/Scripts/QA/QaFastForwardRunner.cs");
        Check(source.Contains("StepSeconds = 1.0") &&
            source.Contains("StepsPerYield = 50") &&
            source.Contains("yield return null"),
            "La ruta corta no usa pasos de 1 s con cesión periódica.", failures);
        Check(source.Contains("state.Tick(step)") &&
            source.Contains("machine.AdvanceAnalysis(") &&
            source.Contains("room2.AdvanceQaFusionCooldown(step)"),
            "El paso no cubre juego, Máquina y cooldown en el orden requerido.",
            failures);
        Check(source.Contains("AggregateOfflineThresholdSeconds = 3600.0") &&
            source.Contains("state.ApplyOfflineBaseProgress(seconds)") &&
            source.Contains("ApplyAggregateOfflineProgress(") &&
            !source.Contains("lastUnix"),
            "La ruta larga no usa el cálculo agregado de ausencia o altera lastUnix.",
            failures);
        Check(source.Contains("TrySave(\"antes de comenzar\")") &&
            source.Contains("TrySave(\"al finalizar\")") &&
            source.Contains("RefreshAfterAdvance()") &&
            source.Contains("LE +") && source.Contains("TRAZAS +") &&
            source.Contains("SetControlsInteractable(false)"),
            "Falta guardado, refresco, reporte de ganancia o bloqueo de controles.",
            failures);
    }

    private static void ValidateAggregateOfflineAdvance(
        List<string> failures)
    {
        GameState state = CreateState("QA B4 Aggregate 24H");
        MethodInfo aggregate = typeof(QaFastForwardRunner).GetMethod(
            "ApplyAggregateOfflineProgress",
            BindingFlags.NonPublic | BindingFlags.Static);
        try
        {
            state.baseLEps = 2.0;
            state.LE = 10.0;
            SetGameStateSingleton(state);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            object rawReport = aggregate?.Invoke(null,
                new object[] { 86400.0, state, null, null });
            stopwatch.Stop();
            TriangleOfflineReport report = rawReport as TriangleOfflineReport;

            Check(aggregate != null && report != null,
                "No se pudo ejecutar la ruta agregada de ausencia.", failures);
            Check(report != null && report.appliedSeconds == 86400.0 &&
                report.leGained >= 172800.0 &&
                Math.Abs(state.LE - 10.0 - report.leGained) < 0.000001,
                "+24 H no concedió la ganancia base agregada esperada.", failures);
            Check(stopwatch.ElapsedMilliseconds < 1000,
                "+24 H tardó demasiado y puede volver a bloquear el dispositivo: " +
                stopwatch.ElapsedMilliseconds + " ms.", failures);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(state.gameObject);
        }
    }

    private static void ValidateExactDeterministicSteps(List<string> failures)
    {
        GameState normal = CreateState("QA B4 Normal 300");
        GameState fast = CreateState("QA B4 Fast 300");
        MethodInfo processStep = GetProcessStep();
        try
        {
            normal.baseLEps = 2.0;
            fast.baseLEps = 2.0;
            normal.LE = 0.0;
            fast.LE = 0.0;

            SetGameStateSingleton(normal);
            for (int index = 0; index < 300; index++)
                normal.Tick(1.0);

            SetGameStateSingleton(fast);
            QaRuntimeService.TrySetSpeed(20f);
            for (int index = 0; index < 300; index++)
                processStep?.Invoke(null, new object[] { 1.0, fast, null, null });

            Check(processStep != null &&
                Math.Abs(normal.LE - fast.LE) < 0.000001 &&
                Math.Abs(fast.LE - 600.0) < 0.000001 &&
                Math.Abs(normal.triangleSynchronization -
                    fast.triangleSynchronization) < 0.000001,
                "+5 MIN no equivale a 300 pasos online exactos o se multiplicó por QA.",
                failures);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(normal.gameObject);
            UnityEngine.Object.DestroyImmediate(fast.gameObject);
            QaRuntimeService.ResetToNormalSpeed();
        }
    }

    private static void ValidateMachineAndCooldownStep(List<string> failures)
    {
        GameState state = CreateState("QA B4 Machine Cooldown");
        GameObject machineObject = new GameObject("QA B4 Machine")
            { hideFlags = HideFlags.HideAndDontSave };
        machineObject.SetActive(false);
        MachineManager machine = machineObject.AddComponent<MachineManager>();
        GameObject roomObject = new GameObject("QA B4 Room2")
            { hideFlags = HideFlags.HideAndDontSave };
        roomObject.SetActive(false);
        Room2PanelUI room = roomObject.AddComponent<Room2PanelUI>();
        try
        {
            SetField(machine, "_analysisNodeId", "qa_test_node");
            SetField(machine, "_analysisRemainingSeconds", 100.0);
            state.fusionCooldownRemainingSeconds = 100.0;
            state.pendingFusion = new ExperimentalPendingFusionState
            {
                active = true
            };
            SetGameStateSingleton(state);

            GetProcessStep()?.Invoke(null,
                new object[] { 10.0, state, machine, room });

            Check(Math.Abs(machine.AnalysisRemainingSeconds - 90.0) < 0.000001,
                "El análisis de Máquina no avanzó con el paso QA.", failures);
            double cooldown = (double)GetField(room,
                "currentFusionCooldownSeconds");
            Check(Math.Abs(cooldown - 90.0) < 0.000001 &&
                Math.Abs(state.fusionCooldownRemainingSeconds - 90.0) < 0.000001,
                "El cooldown persistente del Cuarto 2 no avanzó con el paso QA.",
                failures);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(state.gameObject);
            UnityEngine.Object.DestroyImmediate(machineObject);
            UnityEngine.Object.DestroyImmediate(roomObject);
        }
    }

    private static void ValidateConcurrentGuard(
        QaFastForwardRunner runner, FieldInfo availabilityOverride,
        List<string> failures)
    {
        if (availabilityOverride == null)
        {
            failures.Add("No se puede activar QA para validar concurrencia.");
            return;
        }

        availabilityOverride.SetValue(null, true);
        FieldInfo runningField = typeof(QaFastForwardRunner).GetField(
            "<IsRunning>k__BackingField",
            BindingFlags.NonPublic | BindingFlags.Instance);
        runningField?.SetValue(runner, true);
        runner.RequestAdvance(300.0);
        Check(runningField != null && runner.IsRunning &&
            runner.panel.operationStatusText.text.Contains("YA HAY"),
            "Una segunda solicitud no se rechaza mientras hay avance activo.",
            failures);
        runningField?.SetValue(runner, false);
    }

    private static MethodInfo GetProcessStep()
    {
        return typeof(QaFastForwardRunner).GetMethod(
            "ProcessGameStep", BindingFlags.NonPublic | BindingFlags.Static);
    }

    private static GameState CreateState(string name)
    {
        var target = new GameObject(name) { hideFlags = HideFlags.HideAndDontSave };
        target.SetActive(false);
        return target.AddComponent<GameState>();
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

    private static void Finish(List<string> failures)
    {
        if (failures.Count == 0)
        {
            Debug.Log("[QA Block 4] PASS | 300 s exactos | pasos de 1 s | " +
                "independiente de xN | +24 H agregado | ganancia visible | Máquina | " +
                "cooldown | guardado previo/final | una sola coroutine");
            return;
        }

        Debug.LogError("[QA Block 4] FAIL\n- " + string.Join("\n- ", failures));
        throw new InvalidOperationException(
            "El Bloque 4 no superó su validación.");
    }

    private static void Check(
        bool condition, string failure, List<string> failures)
    {
        if (!condition)
            failures.Add(failure);
    }
}
#endif

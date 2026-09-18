#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MobilePerformanceRuntimeValidation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ActiveKey = "QF.MobilePerformanceValidation.Active";
    private const string FailedKey = "QF.MobilePerformanceValidation.Failed";
    private const string FrameKey = "QF.MobilePerformanceValidation.Frame";

    [InitializeOnLoadMethod]
    private static void Resume()
    {
        if (!SessionState.GetBool(ActiveKey, false))
            return;

        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        if (EditorApplication.isPlaying)
        {
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
        }
    }

    [MenuItem("Tools/Quantum Forge/QA/Validate Mobile Performance Runtime")]
    public static void Run()
    {
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetBool(FailedKey, false);
        SessionState.SetInt(FrameKey, 0);
        SaveService.SuppressWritesForVisualQa = true;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        EditorApplication.isPlaying = true;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            SaveService.SuppressWritesForVisualQa = true;
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            SaveService.SuppressWritesForVisualQa = false;
            EditorApplication.update -= Tick;
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            bool failed = SessionState.GetBool(FailedKey, false);
            SessionState.SetBool(ActiveKey, false);
            Debug.Log(failed
                ? "[Mobile Performance Runtime] FAIL"
                : "[Mobile Performance Runtime] PASS | singleton | 30 FPS activo | " +
                  "15 FPS reposo | foco restaura | pausa del sistema | QA x20");
            EditorApplication.Exit(failed ? 1 : 0);
        }
    }

    private static void Tick()
    {
        try
        {
            TickCore();
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            SessionState.SetBool(FailedKey, true);
            EditorApplication.update -= Tick;
            EditorApplication.isPlaying = false;
        }
    }

    private static void TickCore()
    {
        int frame = SessionState.GetInt(FrameKey, 0) + 1;
        SessionState.SetInt(FrameKey, frame);
        if (frame != 25)
            return;

        MobilePerformanceGovernor[] governors =
            Resources.FindObjectsOfTypeAll<MobilePerformanceGovernor>()
                .Where(value => value != null).ToArray();
        if (governors.Length != 1)
            throw new InvalidOperationException(
                "Debe existir un solo MobilePerformanceGovernor; encontrados: " +
                governors.Length + ".");

        MobilePerformanceGovernor governor = governors[0];
        int initialFrameRate = GetAppliedFrameRate(governor);
        if (initialFrameRate != MobilePerformanceGovernor.ActiveFrameRate &&
            initialFrameRate != MobilePerformanceGovernor.IdleFrameRate)
            throw new InvalidOperationException(
                "El controlador móvil no aplicó una política inicial válida.");
        if (Application.runInBackground)
            throw new InvalidOperationException(
                "El juego continúa ejecutándose en segundo plano.");
        if (Screen.sleepTimeout != SleepTimeout.SystemSetting)
            throw new InvalidOperationException(
                "El juego impide que Android aplique su suspensión de pantalla.");

        Invoke(governor, "OnApplicationFocus", true);
        if (GetAppliedFrameRate(governor) != MobilePerformanceGovernor.ActiveFrameRate)
            throw new InvalidOperationException(
                "El modo activo no aplica 30 FPS.");

        Invoke(governor, "ApplyFrameRate", MobilePerformanceGovernor.IdleFrameRate);
        if (GetAppliedFrameRate(governor) != MobilePerformanceGovernor.IdleFrameRate)
            throw new InvalidOperationException(
                "El modo de reposo no aplica 15 FPS.");

        Invoke(governor, "OnApplicationFocus", true);
        if (GetAppliedFrameRate(governor) != MobilePerformanceGovernor.ActiveFrameRate)
            throw new InvalidOperationException(
                "Recuperar el foco no restaura 30 FPS.");

        if (!QaRuntimeService.TrySetSpeed(20f) ||
            Mathf.Abs(QaRuntimeService.SimulationMultiplier - 20f) > 0.001f)
        {
            throw new InvalidOperationException(
                "La política térmica interfiere con la aceleración QA.");
        }
        QaRuntimeService.ResetToNormalSpeed();

        EditorApplication.update -= Tick;
        EditorApplication.isPlaying = false;
    }

    private static void Invoke(object target, string methodName, object argument)
    {
        MethodInfo method = target.GetType().GetMethod(methodName,
            BindingFlags.NonPublic | BindingFlags.Instance);
        if (method == null)
            throw new MissingMethodException(target.GetType().Name, methodName);
        method.Invoke(target, new[] { argument });
    }

    private static int GetAppliedFrameRate(MobilePerformanceGovernor governor)
    {
        FieldInfo field = typeof(MobilePerformanceGovernor).GetField(
            "appliedFrameRate", BindingFlags.NonPublic | BindingFlags.Instance);
        if (field == null)
            throw new MissingFieldException(
                typeof(MobilePerformanceGovernor).Name, "appliedFrameRate");
        return (int)field.GetValue(governor);
    }
}
#endif

#if UNITY_EDITOR
using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;


public static class PresentationReturnReportCapture
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ActiveKey = "QF.ReturnReportCapture.Active";
    private const string StageKey = "QF.ReturnReportCapture.Stage";
    private const string FramesKey = "QF.ReturnReportCapture.Frames";
    private const string FailedKey = "QF.ReturnReportCapture.Failed";
    private const string SaveExistedKey = "QF.ReturnReportCapture.SaveExisted";
    private const string BackupExistedKey = "QF.ReturnReportCapture.BackupExisted";
    private static string OutputDirectory =>
        Path.GetFullPath("Logs/VisualQA/OfflineReturn");
    private static string SavePath =>
        Path.Combine(Application.persistentDataPath, "save.json");
    private static string BackupPath => SavePath + ".bak";
    private static string SessionSave =>
        Path.Combine(OutputDirectory, ".save.session");
    private static string SessionBackup =>
        Path.Combine(OutputDirectory, ".save.bak.session");
    private static double stageStartedAt;

    [InitializeOnLoadMethod]
    private static void Resume()
    {
        if (!SessionState.GetBool(ActiveKey, false)) return;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        if (EditorApplication.isPlaying)
        {
            stageStartedAt = EditorApplication.timeSinceStartup;
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
        }
    }

    [MenuItem("Tools/Quantum Forge/Presentation/Capture Away Log")]
    public static void Run()
    {
        Directory.CreateDirectory(OutputDirectory);
        BackupUserSave();
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetBool(FailedKey, false);
        SessionState.SetInt(StageKey, 0);
        SessionState.SetInt(FramesKey, 0);
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        EditorApplication.isPlaying = true;
    }

    private static void OnPlayModeChanged(PlayModeStateChange change)
    {
        if (change == PlayModeStateChange.EnteredPlayMode)
        {
            stageStartedAt = EditorApplication.timeSinceStartup;
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
            return;
        }
        if (change != PlayModeStateChange.EnteredEditMode) return;

        EditorApplication.update -= Tick;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        bool failed = SessionState.GetBool(FailedKey, false);
        try { RestoreUserSave(); }
        catch (Exception exception)
        {
            failed = true;
            Debug.LogException(exception);
        }
        SessionState.SetBool(ActiveKey, false);
        if (!failed)
            Debug.Log("[Away Log Capture] PASS | 1080x1920 | balance real | single Continue | user save restored");
        EditorApplication.Exit(failed ? 1 : 0);
    }

    private static void Tick()
    {
        int frames = SessionState.GetInt(FramesKey, 0) + 1;
        SessionState.SetInt(FramesKey, frames);
        if (frames < 5 || EditorApplication.timeSinceStartup - stageStartedAt < 1.1)
            return;

        try
        {
            switch (SessionState.GetInt(StageKey, 0))
            {
                case 0:
                    PrepareReport();
                    Advance(1);
                    break;
                case 1:
                    ValidateAndCapture();
                    Advance(2);
                    break;
                default:
                    EditorApplication.update -= Tick;
                    EditorApplication.isPlaying = false;
                    break;
            }
        }
        catch (Exception exception)
        {
            SessionState.SetBool(FailedKey, true);
            Debug.LogException(exception);
            EditorApplication.update -= Tick;
            EditorApplication.isPlaying = false;
        }
    }

    private static void PrepareReport()
    {
        Require(GameState.I != null, "No inició GameState.");
        SaveService.I?.CancelInvoke("Save");
        LocalizationManager.I?.SetLanguage(LocalizationManager.Language.ES);
        PresentationReturnSnapshot before =
            PresentationReturnReportService.Capture(GameState.I);
        GameState.I.LE += 12800.0;
        GameState.I.Traces += 23.0;
        GameState.I.triangleEnergy += 52.0;
        PresentationReturnReportService.Prepare(
            before, GameState.I, 2880.0, 2880.0, 2880.0, 2880.0);
    }

    private static void ValidateAndCapture()
    {
        PresentationReturnReportUI ui = UnityEngine.Object.FindFirstObjectByType<
            PresentationReturnReportUI>(FindObjectsInactive.Include);
        Require(ui != null && ui.gameObject.activeInHierarchy,
            "No apareció el informe de ausencia.");
        Button[] buttons = ui.GetComponentsInChildren<Button>(true);
        Require(buttons.Length == 1 && buttons[0].name == "Continue",
            "El informe no tiene un único botón Continuar.");
        PrepareResolution(1080, 1920);
        Capture("away_log_es_1080x1920.png", 1080, 1920);
    }

    private static void PrepareResolution(int width, int height)
    {
        MethodInfo method = typeof(VerticalUiBlock7Capture).GetMethod(
            "PrepareResolution", BindingFlags.Static | BindingFlags.NonPublic);
        method?.Invoke(null, new object[] { width, height });
    }

    private static void Capture(string fileName, int width, int height)
    {
        string path = Path.Combine(OutputDirectory, fileName);
        Camera camera = Camera.main != null ? Camera.main :
            UnityEngine.Object.FindFirstObjectByType<Camera>();
        Require(camera != null, "No hay cámara para capturar.");
        Canvas[] canvases = UnityEngine.Object.FindObjectsByType<Canvas>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        RenderMode[] modes = new RenderMode[canvases.Length];
        Camera[] cameras = new Camera[canvases.Length];
        float[] distances = new float[canvases.Length];
        RenderTexture target = new RenderTexture(width, height, 24,
            RenderTextureFormat.ARGB32);
        RenderTexture previousTarget = camera.targetTexture;
        RenderTexture previousActive = RenderTexture.active;
        try
        {
            target.Create();
            camera.targetTexture = target;
            for (int index = 0; index < canvases.Length; index++)
            {
                modes[index] = canvases[index].renderMode;
                cameras[index] = canvases[index].worldCamera;
                distances[index] = canvases[index].planeDistance;
                if (modes[index] == RenderMode.ScreenSpaceOverlay)
                {
                    canvases[index].renderMode = RenderMode.ScreenSpaceCamera;
                    canvases[index].worldCamera = camera;
                    canvases[index].planeDistance = 1f;
                }
            }
            RecalculateCanvasScalers(canvases);
            Canvas.ForceUpdateCanvases();
            camera.Render();
            RenderTexture.active = target;
            Texture2D image = new Texture2D(
                width, height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
            image.Apply(false, false);
            File.WriteAllBytes(path, image.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(image);
        }
        finally
        {
            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            target.Release();
            UnityEngine.Object.DestroyImmediate(target);
            for (int index = 0; index < canvases.Length; index++)
            {
                canvases[index].renderMode = modes[index];
                canvases[index].worldCamera = cameras[index];
                canvases[index].planeDistance = distances[index];
            }
        }
        Require(File.Exists(path) && new FileInfo(path).Length > 4096,
            "No se generó " + fileName);
        Debug.Log("[Away Log Capture] PNG | " + path);
    }

    private static void RecalculateCanvasScalers(Canvas[] canvases)
    {
        MethodInfo handle = typeof(CanvasScaler).GetMethod(
            "Handle", BindingFlags.Instance | BindingFlags.NonPublic);
        if (handle == null) return;
        foreach (Canvas canvas in canvases)
        {
            CanvasScaler scaler = canvas != null
                ? canvas.GetComponent<CanvasScaler>() : null;
            if (scaler != null && scaler.enabled) handle.Invoke(scaler, null);
        }
    }

    private static void Advance(int next)
    {
        SessionState.SetInt(StageKey, next);
        SessionState.SetInt(FramesKey, 0);
        stageStartedAt = EditorApplication.timeSinceStartup;
    }

    private static void BackupUserSave()
    {
        SessionState.SetBool(SaveExistedKey, File.Exists(SavePath));
        SessionState.SetBool(BackupExistedKey, File.Exists(BackupPath));
        if (File.Exists(SavePath)) File.Copy(SavePath, SessionSave, true);
        if (File.Exists(BackupPath)) File.Copy(BackupPath, SessionBackup, true);
    }

    private static void RestoreUserSave()
    {
        RestoreFile(SavePath, SessionSave,
            SessionState.GetBool(SaveExistedKey, false));
        RestoreFile(BackupPath, SessionBackup,
            SessionState.GetBool(BackupExistedKey, false));
    }

    private static void RestoreFile(string target, string session, bool existed)
    {
        if (existed && File.Exists(session)) File.Copy(session, target, true);
        else if (!existed && File.Exists(target)) File.Delete(target);
        if (File.Exists(session)) File.Delete(session);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
#endif

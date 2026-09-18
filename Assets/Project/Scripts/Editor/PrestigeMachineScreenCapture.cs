#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public static class PrestigeMachineScreenCapture
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ActiveKey = "QF.PrestigeMachineCapture.Active";
    private const string FrameKey = "QF.PrestigeMachineCapture.Frame";
    private const string FailureKey = "QF.PrestigeMachineCapture.Failed";
    private static string OutputDirectory =>
        Path.GetFullPath("Logs/VisualQA/PrestigeMachineScreen");

    [InitializeOnLoadMethod]
    private static void Resume()
    {
        if (!SessionState.GetBool(ActiveKey, false)) return;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        if (EditorApplication.isPlaying)
        {
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
        }
    }

    [MenuItem("Tools/Quantum Forge/Prestige/Capture Approved Machine Screen")]
    public static void Run()
    {
        Directory.CreateDirectory(OutputDirectory);
        foreach (string file in Directory.GetFiles(OutputDirectory, "*.png"))
            File.Delete(file);
        PrestigeMachineScreenSetup.Configure();
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetBool(FailureKey, false);
        SessionState.SetInt(FrameKey, 0);
        SaveService.SuppressWritesForVisualQa = true;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        EditorApplication.isPlaying = true;
    }

    public static void RunBatch()
    {
        Run();
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            Application.runInBackground = true;
            EditorApplication.isPaused = false;
            SaveService.SuppressWritesForVisualQa = true;
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            SaveService.SuppressWritesForVisualQa = false;
            SessionState.SetBool(ActiveKey, false);
            EditorApplication.update -= Tick;
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            bool failed = SessionState.GetBool(FailureKey, false);
            Debug.Log(failed
                ? "[Prestige Machine Screen Capture] FAIL"
                : "[Prestige Machine Screen Capture] PASS | static locked + ready | 1080x1920 + 720x1280 | " + OutputDirectory);
            EditorApplication.Exit(failed ? 1 : 0);
        }
    }

    private static void Tick()
    {
        try
        {
            int frame = SessionState.GetInt(FrameKey, 0) + 1;
            SessionState.SetInt(FrameKey, frame);
            if (frame == 18) PrepareAndOpen();
            if (frame == 32) CaptureState(false);
            if (frame == 44) CaptureState(true);
            if (frame != 52) return;
            EditorApplication.update -= Tick;
            EditorApplication.isPlaying = false;
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            SessionState.SetBool(FailureKey, true);
            EditorApplication.update -= Tick;
            EditorApplication.isPlaying = false;
        }
    }

    private static void PrepareAndOpen()
    {
        Require(GameState.I != null && MachineManager.I != null && TabsUI.Instance != null,
            "Los sistemas principales no terminaron de iniciar.");
        GameState.I.experimentalChamberUnlocked = true;
        MachineManager.I.MarkIntroSeenAndUnlockMachine();
        SuppressReports();
        TabsUI.Instance.ShowPrestigeFromMachine();
        ForceVisible();
    }

    private static void CaptureState(bool ready)
    {
        ForceVisible();
        PrestigeMachineScreenUI screen = Object.FindFirstObjectByType<PrestigeMachineScreenUI>(
            FindObjectsInactive.Include);
        Require(screen != null && screen.gameObject.activeInHierarchy,
            "La pantalla de Prestigio no está visible.");
        screen.PreviewForCapture(ready ? .82f : .65f, ready, ready);
        Canvas.ForceUpdateCanvases();
        if (!ready)
        {
            LogRect(screen.transform, "MachinePrimaryContent");
            LogRect(screen.transform, "MachinePrimaryContent/MonolithViewHeader");
            LogRect(screen.transform, "MachinePrimaryContent/MonolithViewport");
            LogRect(screen.transform, "MachinePrimaryContent/PrestigeStatusPanel");
        }
        string state = ready ? "ready" : "locked_65";
        RenderToPng(1080, 1920, Path.Combine(OutputDirectory,
            "Prestige_" + state + "_1080x1920.png"));
        RenderToPng(720, 1280, Path.Combine(OutputDirectory,
            "Prestige_" + state + "_720x1280.png"));
    }

    private static void ForceVisible()
    {
        SuppressReports();
        TabsUI.Instance.ShowPrestigeFromMachine();
        PrestigeMachineScreenUI screen = Object.FindFirstObjectByType<PrestigeMachineScreenUI>(
            FindObjectsInactive.Include);
        Require(screen != null, "No se encontró PrestigeMachineScreenUI.");
        screen.gameObject.SetActive(true);
        Canvas.ForceUpdateCanvases();
    }

    private static void SuppressReports()
    {
        PresentationReturnReportService.Consume();
        foreach (PresentationReturnReportUI report in Object.FindObjectsByType<PresentationReturnReportUI>(
                     FindObjectsInactive.Include, FindObjectsSortMode.None))
            report.gameObject.SetActive(false);
        foreach (TriangleOfflineReportUI report in Object.FindObjectsByType<TriangleOfflineReportUI>(
                     FindObjectsInactive.Include, FindObjectsSortMode.None))
            report.gameObject.SetActive(false);
    }

    private static void RenderToPng(int width, int height, string path)
    {
        Camera camera = Camera.main != null ? Camera.main : Object.FindFirstObjectByType<Camera>();
        Require(camera != null, "No hay cámara para captura.");
        Canvas[] canvases = Object.FindObjectsByType<Canvas>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        var modes = new RenderMode[canvases.Length];
        var cameras = new Camera[canvases.Length];
        var distances = new float[canvases.Length];
        RenderTexture target = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        RenderTexture oldTarget = camera.targetTexture;
        RenderTexture oldActive = RenderTexture.active;
        try
        {
            target.Create();
            camera.targetTexture = target;
            for (int i = 0; i < canvases.Length; i++)
            {
                modes[i] = canvases[i].renderMode;
                cameras[i] = canvases[i].worldCamera;
                distances[i] = canvases[i].planeDistance;
                if (modes[i] == RenderMode.ScreenSpaceOverlay)
                {
                    canvases[i].renderMode = RenderMode.ScreenSpaceCamera;
                    canvases[i].worldCamera = camera;
                    canvases[i].planeDistance = 1f;
                }
            }
            Canvas.ForceUpdateCanvases();
            camera.Render();
            camera.Render();
            RenderTexture.active = target;
            Texture2D image = new Texture2D(width, height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            image.Apply();
            File.WriteAllBytes(path, image.EncodeToPNG());
            Object.DestroyImmediate(image);
            Require(File.Exists(path) && new FileInfo(path).Length > 4096,
                "Captura inválida: " + path);
        }
        finally
        {
            camera.targetTexture = oldTarget;
            RenderTexture.active = oldActive;
            target.Release();
            Object.DestroyImmediate(target);
            for (int i = 0; i < canvases.Length; i++)
            {
                canvases[i].renderMode = modes[i];
                canvases[i].worldCamera = cameras[i];
                canvases[i].planeDistance = distances[i];
            }
        }
    }

    private static void LogRect(Transform root, string path)
    {
        RectTransform rect = root.Find(path)?.GetComponent<RectTransform>();
        if (rect == null) return;
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);
        Debug.Log($"[Prestige Layout Rect] {path} | rect={rect.rect} | " +
            $"worldBL={corners[0]} worldTR={corners[2]} | scale={rect.lossyScale}");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
#endif

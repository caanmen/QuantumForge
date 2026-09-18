#if UNITY_EDITOR
using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class PrestigeDimensionTransitionCapture
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ActiveKey = "QF.PrestigeDimensionCapture.Active";
    private const string StageKey = "QF.PrestigeDimensionCapture.Stage";
    private const string FailedKey = "QF.PrestigeDimensionCapture.Failed";
    private const string SaveExistedKey = "QF.PrestigeDimensionCapture.SaveExisted";
    private const string ExitWhenDoneKey = "QF.PrestigeDimensionCapture.ExitWhenDone";

    private static double _stageStartedAt;
    private static string OutputDirectory =>
        Path.GetFullPath("Logs/VisualQA/PrestigeDimension");
    private static string SavePath =>
        Path.Combine(Application.persistentDataPath, "save.json");
    private static string SaveBackupPath => SavePath + ".bak";
    private static string SessionSave =>
        Path.Combine(OutputDirectory, ".save.session-backup");
    private static string SessionBackup =>
        Path.Combine(OutputDirectory, ".save.bak.session-backup");

    [InitializeOnLoadMethod]
    private static void ResumeAfterReload()
    {
        if (!SessionState.GetBool(ActiveKey, false))
            return;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        if (EditorApplication.isPlaying)
        {
            _stageStartedAt = EditorApplication.timeSinceStartup;
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
        }
    }

    [MenuItem("Tools/Quantum Forge/Prestige/Capture Dimensional Transition")]
    public static void Run()
    {
        StartCapture(false);
    }

    private static void StartCapture(bool exitWhenDone)
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            throw new InvalidOperationException(
                "La captura debe comenzar fuera de Play Mode.");

        Directory.CreateDirectory(OutputDirectory);
        foreach (string capture in Directory.GetFiles(OutputDirectory, "*.png"))
            File.Delete(capture);
        BackupUserSave();

        try
        {
            PrestigeDimensionTransitionSetup.Configure();
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            SessionState.SetBool(ActiveKey, true);
            SessionState.SetBool(FailedKey, false);
            SessionState.SetBool(ExitWhenDoneKey, exitWhenDone);
            SessionState.SetInt(StageKey, 0);
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            EditorApplication.isPlaying = true;
        }
        catch
        {
            SessionState.SetBool(ActiveKey, false);
            RestoreUserSave();
            throw;
        }
    }

    public static void RunBatch()
    {
        StartCapture(true);
    }

    private static void OnPlayModeChanged(PlayModeStateChange change)
    {
        if (change == PlayModeStateChange.EnteredPlayMode)
        {
            Application.runInBackground = true;
            EditorApplication.isPaused = false;
            _stageStartedAt = EditorApplication.timeSinceStartup;
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
            return;
        }
        if (change != PlayModeStateChange.EnteredEditMode)
            return;

        EditorApplication.update -= Tick;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        bool failed = SessionState.GetBool(FailedKey, false);
        try
        {
            RestoreUserSave();
        }
        catch (Exception exception)
        {
            failed = true;
            Debug.LogException(exception);
        }
        finally
        {
            SessionState.SetBool(ActiveKey, false);
            SessionState.SetBool(FailedKey, false);
        }

        if (!failed)
        {
            Debug.Log(
                "[Prestige Dimension Capture] PASS | Monolito abierto | cuatro luces | " +
                "tres portales | " +
                "selección vertical | " +
                "confirmación 2/3 | 1080x1920 + 720x1280 | save restaurado");
        }
        bool exitWhenDone = SessionState.GetBool(ExitWhenDoneKey, false);
        SessionState.SetBool(ExitWhenDoneKey, false);
        if (Application.isBatchMode || exitWhenDone)
            EditorApplication.Exit(failed ? 1 : 0);
    }

    private static void Tick()
    {
        double elapsed = EditorApplication.timeSinceStartup - _stageStartedAt;
        try
        {
            int stage = SessionState.GetInt(StageKey, 0);
            switch (stage)
            {
                case 0 when elapsed >= 1.0:
                    PreparePrestigePanel();
                    Advance(1);
                    break;
                case 1 when elapsed >= 0.8:
                    PreparePrestigePanel();
                    FindFlow().PreviewOpenMonolithForCapture();
                    Advance(2);
                    break;
                case 2 when elapsed >= 1.5:
                    Capture("01_monolito_abierto_estatico.png", 1080, 1920);
                    Capture("01b_monolito_abierto_estatico_720x1280.png", 720, 1280);
                    FindFlow().PreviewTransitionForCapture();
                    Advance(3);
                    break;
                case 3 when elapsed >= 1.55:
                    Capture("02_monolito_reparado_luces_secuenciales.png", 1080, 1920);
                    Advance(4);
                    break;
                case 4 when elapsed >= 1.4:
                    Capture("03_energia_converge_al_nucleo.png", 1080, 1920);
                    Advance(5);
                    break;
                case 5 when elapsed >= 0.95:
                    Capture("04_placas_abiertas_nucleo_visible.png", 1080, 1920);
                    FindFlow().PreviewPortalsForCapture();
                    Advance(6);
                    break;
                case 6 when elapsed >= 0.45:
                    Capture("05_tres_portales_emergen.png", 1080, 1920);
                    Advance(7);
                    break;
                case 7 when elapsed >= 1.7:
                    FindFlow().PreviewSelectionForCapture(2, 0);
                    Advance(8);
                    break;
                case 8 when elapsed >= 0.4:
                    Capture("06_seleccion_dimensional_vertical.png", 1080, 1920);
                    Capture("06b_seleccion_dimensional_720x1280.png", 720, 1280);
                    Advance(9);
                    break;
                case 9 when elapsed >= 0.6:
                    FindFlow().PreviewSelectionForCapture(2, 2);
                    Advance(10);
                    break;
                case 10 when elapsed >= 0.4:
                    Capture("07_confirmacion_sintonizar_2_de_3.png", 1080, 1920);
                    Advance(11);
                    break;
                case 11 when elapsed >= 0.5:
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

    private static void PreparePrestigePanel()
    {
        Application.runInBackground = true;
        EditorApplication.isPaused = false;
        Screen.SetResolution(1080, 1920, false);
        TabManager manager = UnityEngine.Object.FindFirstObjectByType<TabManager>(
            FindObjectsInactive.Include);
        if (manager != null)
            manager.ShowPrestigio();
        else if (TabsUI.Instance != null && TabsUI.Instance.prestigePanel != null)
            TabsUI.Instance.prestigePanel.SetActive(true);
        Canvas.ForceUpdateCanvases();
    }

    private static PrestigeDimensionTransitionUI FindFlow()
    {
        PrestigeDimensionTransitionUI flow =
            UnityEngine.Object.FindFirstObjectByType<PrestigeDimensionTransitionUI>(
                FindObjectsInactive.Include);
        if (flow == null)
            throw new InvalidOperationException(
                "No se encontró PrestigeDimensionTransitionUI en Play Mode.");
        return flow;
    }

    private static void Advance(int stage)
    {
        SessionState.SetInt(StageKey, stage);
        _stageStartedAt = EditorApplication.timeSinceStartup;
    }

    private static void Capture(string fileName, int width, int height)
    {
        string path = Path.Combine(OutputDirectory, fileName);
        RenderToPng(width, height, path);
        if (!File.Exists(path) || new FileInfo(path).Length < 4096)
            throw new InvalidOperationException(
                "La captura no se generó correctamente: " + fileName);
        Debug.Log("[Prestige Dimension Capture] PNG | " + fileName);
    }

    private static void RenderToPng(int width, int height, string path)
    {
        Camera camera = Camera.main != null
            ? Camera.main
            : UnityEngine.Object.FindFirstObjectByType<Camera>();
        if (camera == null)
            throw new InvalidOperationException("No hay cámara para la captura.");

        Canvas[] canvases = UnityEngine.Object.FindObjectsByType<Canvas>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        var modes = new RenderMode[canvases.Length];
        var cameras = new Camera[canvases.Length];
        var distances = new float[canvases.Length];
        RenderTexture target = new RenderTexture(
            width, height, 24, RenderTextureFormat.ARGB32);
        RenderTexture previousTarget = camera.targetTexture;
        RenderTexture previousActive = RenderTexture.active;

        try
        {
            target.Create();
            camera.targetTexture = target;
            for (int i = 0; i < canvases.Length; i++)
            {
                modes[i] = canvases[i].renderMode;
                cameras[i] = canvases[i].worldCamera;
                distances[i] = canvases[i].planeDistance;
                if (modes[i] != RenderMode.ScreenSpaceOverlay)
                    continue;
                canvases[i].renderMode = RenderMode.ScreenSpaceCamera;
                canvases[i].worldCamera = camera;
                canvases[i].planeDistance = 1f;
            }

            RecalculateCanvasScalers(canvases);
            Canvas.ForceUpdateCanvases();
            foreach (Canvas canvas in canvases)
                if (canvas != null && canvas.transform is RectTransform root)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(root);
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
            for (int i = 0; i < canvases.Length; i++)
            {
                canvases[i].renderMode = modes[i];
                canvases[i].worldCamera = cameras[i];
                canvases[i].planeDistance = distances[i];
            }
        }
    }

    private static void RecalculateCanvasScalers(Canvas[] canvases)
    {
        MethodInfo handle = typeof(CanvasScaler).GetMethod(
            "Handle", BindingFlags.Instance | BindingFlags.NonPublic);
        if (handle == null)
            return;
        foreach (Canvas canvas in canvases)
        {
            CanvasScaler scaler = canvas != null
                ? canvas.GetComponent<CanvasScaler>()
                : null;
            if (scaler != null && scaler.isActiveAndEnabled)
                handle.Invoke(scaler, null);
        }
    }

    private static void BackupUserSave()
    {
        Directory.CreateDirectory(OutputDirectory);
        bool existed = File.Exists(SavePath);
        SessionState.SetBool(SaveExistedKey, existed);
        if (existed)
            File.Copy(SavePath, SessionSave, true);
        if (File.Exists(SaveBackupPath))
            File.Copy(SaveBackupPath, SessionBackup, true);
    }

    private static void RestoreUserSave()
    {
        bool existed = SessionState.GetBool(SaveExistedKey, false);
        if (existed && File.Exists(SessionSave))
            File.Copy(SessionSave, SavePath, true);
        else if (!existed && File.Exists(SavePath))
            File.Delete(SavePath);

        if (File.Exists(SessionBackup))
            File.Copy(SessionBackup, SaveBackupPath, true);
        File.Delete(SessionSave);
        File.Delete(SessionBackup);
    }
}
#endif

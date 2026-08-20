#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class MachineFusionPanelCapture
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ActiveKey = "QF.MachineFusionCapture.Active";
    private const string FramesKey = "QF.MachineFusionCapture.Frames";
    private const string FailedKey = "QF.MachineFusionCapture.Failed";
    private const string CompletedKey = "QF.MachineFusionCapture.Completed";
    private const string SaveExistedKey = "QF.MachineFusionCapture.SaveExisted";
    private const string BackupExistedKey = "QF.MachineFusionCapture.BackupExisted";
    private const string ExitWhenCompleteKey =
        "QF.MachineFusionCapture.ExitWhenComplete";

    private static Camera _camera;
    private static RenderTexture _target;
    private static RenderTexture _previousTarget;
    private static Canvas[] _canvases;
    private static RenderMode[] _modes;
    private static Camera[] _canvasCameras;
    private static float[] _planeDistances;

    private static string OutputDirectory =>
        Path.GetFullPath("Logs/VisualQA/MachineFusion");
    private static string SavePath =>
        Path.Combine(Application.persistentDataPath, "save.json");
    private static string SaveBackupPath => SavePath + ".bak";
    private static string SessionSavePath =>
        Path.Combine(OutputDirectory, ".save.session-backup");
    private static string SessionBackupPath =>
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
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
        }
    }

    [MenuItem("Tools/Quantum Forge/Machine/Capture Fusion Panel")]
    public static void Run()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            throw new InvalidOperationException("La captura debe iniciar fuera de Play Mode.");

        Directory.CreateDirectory(OutputDirectory);
        BackupUserSave();
        MachineFusionPanelVisualSetup.Configure();
        MachineFusionApprovedStyleSetup.Configure();
        MachineFusionApprovedStyleSetup.Validate();
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetBool(FailedKey, false);
        SessionState.SetBool(CompletedKey, false);
        SessionState.SetInt(FramesKey, 0);
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        EditorApplication.isPlaying = true;
    }

    public static void RunBatch()
    {
        Run();
    }

    public static void RunNormalAndExit()
    {
        SessionState.SetBool(ExitWhenCompleteKey, true);
        try
        {
            Run();
        }
        catch (Exception exception)
        {
            SessionState.SetBool(ExitWhenCompleteKey, false);
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    private static void OnPlayModeChanged(PlayModeStateChange change)
    {
        if (change == PlayModeStateChange.EnteredPlayMode)
        {
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
            return;
        }
        if (change != PlayModeStateChange.EnteredEditMode)
            return;

        EditorApplication.update -= Tick;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EndCapture();
        bool failed = SessionState.GetBool(FailedKey, false) ||
            !SessionState.GetBool(CompletedKey, false);
        try
        {
            RestoreUserSave();
        }
        catch (Exception exception)
        {
            failed = true;
            Debug.LogException(exception);
        }
        SessionState.SetBool(ActiveKey, false);
        SessionState.SetBool(FailedKey, false);
        if (!failed)
            Debug.Log("[Machine Fusion Capture] PASS | functional panel | 1080x1920");
        bool exitWhenComplete = SessionState.GetBool(ExitWhenCompleteKey, false);
        SessionState.SetBool(ExitWhenCompleteKey, false);
        if (Application.isBatchMode || exitWhenComplete)
            EditorApplication.Exit(failed ? 1 : 0);
    }

    private static void Tick()
    {
        int frames = SessionState.GetInt(FramesKey, 0) + 1;
        SessionState.SetInt(FramesKey, frames);
        try
        {
            if (frames == 30)
            {
                PreparePanel();
                HideTransientOverlays();
                BeginCapture(1080, 1920);
                return;
            }
            if (frames < 55)
                return;

            HideTransientOverlays();
            string path = Path.Combine(OutputDirectory,
                "fusion_panel_operational_1080x1920.png");
            RenderToPng(1080, 1920, path);
            Require(File.Exists(path) && new FileInfo(path).Length > 10000,
                "La captura del panel de mezclas quedó vacía.");
            EditorApplication.update -= Tick;
            EndCapture();
            SessionState.SetBool(CompletedKey, true);
            EditorApplication.isPlaying = false;
        }
        catch (Exception exception)
        {
            SessionState.SetBool(FailedKey, true);
            Debug.LogException(exception);
            EditorApplication.update -= Tick;
            EndCapture();
            EditorApplication.isPlaying = false;
        }
    }

    private static void PreparePanel()
    {
        Require(GameState.I != null, "GameState no disponible.");
        Require(MachineManager.I != null, "MachineManager no disponible.");
        GameState.I.experimentalChamberUnlocked = true;
        GameState.I.LE = 1e15;
        GameState.I.Traces = 1e12;
        GameState.I.fragmentCondensation = 8;
        GameState.I.fragmentConfinement = 5;
        GameState.I.fragmentResidualInterference = 3;
        GameState.I.synthesisCoreFusionCounter = 4;

        var repairedIds = MachineManager.I.GetAllNodes(true)
            .Where(node => node != null && !node.hidden)
            .Select(node => node.id).ToList();
        MachineManager.I.LoadProgressFromSave(new SaveData
        {
            machineIntroSeen = true,
            machineUnlocked = true,
            machineAllZonesUnlocked = true,
            machineFusionPanelUnlocked = true,
            machineSelectedFaceIndex = 1,
            machineRepairedNodeIds = repairedIds,
            machineAnalyzedNodeIds = repairedIds
        });

        TabsUI.Instance?.ShowRoom2();
        MachinePanelUI panel = UnityEngine.Object.FindFirstObjectByType<MachinePanelUI>(
            FindObjectsInactive.Include);
        Require(panel != null, "MachinePanelUI no disponible.");
        ActivateAncestors(panel.transform);
        panel.SelectZoneFromCube(MachineZoneType.FusionSector);
        panel.Refresh();

        Button mixes = panel.transform.Find(
            "MachineCubeVisualRoot/MachineContextTabs/MixesTab")
            ?.GetComponent<Button>();
        Require(mixes != null, "No se encontró la pestaña MEZCLAS.");
        mixes.onClick.Invoke();
        panel.Refresh();

        Transform fusionRoot = panel.transform.parent.Find("LegacyFusionPanel");
        Require(fusionRoot != null && fusionRoot.gameObject.activeInHierarchy,
            "El panel visual de mezclas no se abrió.");
        Button slotA = fusionRoot.Find(
            "FusionVisualShell/FragmentA/Selector")?.GetComponent<Button>();
        Button slotB = fusionRoot.Find(
            "FusionVisualShell/FragmentB/Selector")?.GetComponent<Button>();
        Button catalyst = fusionRoot.Find(
            "FusionVisualShell/Catalyst/Selector")?.GetComponent<Button>();
        Button guidedIntent = fusionRoot.Find(
            "FusionVisualShell/GuidedIntentButton")?.GetComponent<Button>();
        Button fusion = fusionRoot.Find(
            "FusionVisualShell/FusionButton")?.GetComponent<Button>();
        Room2PanelUI roomPanel = UnityEngine.Object.FindFirstObjectByType<Room2PanelUI>(
            FindObjectsInactive.Include);
        Require(slotA != null && slotB != null && catalyst != null,
            "Faltan selectores funcionales del panel.");
        Require(guidedIntent != null && fusion != null && roomPanel != null,
            "Faltan controles dinámicos del panel.");
        slotA.onClick.Invoke();
        slotB.onClick.Invoke();
        slotB.onClick.Invoke();
        catalyst.onClick.Invoke();
        guidedIntent.onClick.Invoke();

        UnityEngine.Random.InitState(20260806);
        for (int i = 0; i < 4; i++)
        {
            fusion.onClick.Invoke();
            if (i < 3)
                roomPanel.AdvanceQaFusionCooldown(99.0);
        }
        GameState.I.fragmentCondensation = 8;
        GameState.I.fragmentConfinement = 5;

        Canvas.ForceUpdateCanvases();
        RefreshGeometry();
    }

    private static void ActivateAncestors(Transform target)
    {
        Transform current = target;
        while (current != null)
        {
            current.gameObject.SetActive(true);
            current = current.parent;
        }
    }

    private static void HideTransientOverlays()
    {
        foreach (PresentationReturnReportUI report in
                 UnityEngine.Object.FindObjectsByType<PresentationReturnReportUI>(
                     FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (report != null)
                report.gameObject.SetActive(false);
        }
    }

    private static void BackupUserSave()
    {
        bool saveExists = File.Exists(SavePath);
        bool backupExists = File.Exists(SaveBackupPath);
        SessionState.SetBool(SaveExistedKey, saveExists);
        SessionState.SetBool(BackupExistedKey, backupExists);
        if (saveExists)
            File.Copy(SavePath, SessionSavePath, true);
        else if (File.Exists(SessionSavePath))
            File.Delete(SessionSavePath);
        if (backupExists)
            File.Copy(SaveBackupPath, SessionBackupPath, true);
        else if (File.Exists(SessionBackupPath))
            File.Delete(SessionBackupPath);
    }

    private static void RestoreUserSave()
    {
        if (SessionState.GetBool(SaveExistedKey, false))
        {
            Require(File.Exists(SessionSavePath),
                "Falta la copia temporal del guardado del usuario.");
            File.Copy(SessionSavePath, SavePath, true);
        }
        else if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }

        if (SessionState.GetBool(BackupExistedKey, false))
        {
            Require(File.Exists(SessionBackupPath),
                "Falta la copia temporal del respaldo del usuario.");
            File.Copy(SessionBackupPath, SaveBackupPath, true);
        }
        else if (File.Exists(SaveBackupPath))
        {
            File.Delete(SaveBackupPath);
        }

        if (File.Exists(SessionSavePath))
            File.Delete(SessionSavePath);
        if (File.Exists(SessionBackupPath))
            File.Delete(SessionBackupPath);
    }

    private static void BeginCapture(int width, int height)
    {
        if (_target != null && _camera != null)
            return;
        _camera = Camera.main != null
            ? Camera.main
            : UnityEngine.Object.FindFirstObjectByType<Camera>();
        Require(_camera != null, "No hay cámara para la captura.");
        _canvases = UnityEngine.Object.FindObjectsByType<Canvas>(
                FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .Where(canvas => canvas != null && canvas.isRootCanvas)
            .OrderBy(canvas => canvas.sortingOrder)
            .ThenBy(canvas => canvas.transform.GetSiblingIndex()).ToArray();
        _modes = new RenderMode[_canvases.Length];
        _canvasCameras = new Camera[_canvases.Length];
        _planeDistances = new float[_canvases.Length];
        _target = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        _previousTarget = _camera.targetTexture;
        _camera.targetTexture = _target;
        for (int i = 0; i < _canvases.Length; i++)
        {
            Canvas canvas = _canvases[i];
            _modes[i] = canvas.renderMode;
            _canvasCameras[i] = canvas.worldCamera;
            _planeDistances[i] = canvas.planeDistance;
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = _camera;
                canvas.planeDistance = 1f + (_canvases.Length - i) * 0.05f;
            }
        }
        RefreshGeometry();
    }

    private static void RenderToPng(int width, int height, string path)
    {
        BeginCapture(width, height);
        RenderTexture previousActive = RenderTexture.active;
        try
        {
            for (int pass = 0; pass < 5; pass++)
            {
                RefreshGeometry();
                _camera.Render();
            }
            RenderTexture.active = _target;
            Texture2D image = new Texture2D(width, height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            image.Apply();
            File.WriteAllBytes(path, image.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(image);
        }
        finally
        {
            RenderTexture.active = previousActive;
        }
    }

    private static void RefreshGeometry()
    {
        Canvas.ForceUpdateCanvases();
        foreach (RectTransform rect in UnityEngine.Object.FindObjectsByType<RectTransform>(
                     FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            rect.ForceUpdateRectTransforms();
        foreach (TMP_Text text in UnityEngine.Object.FindObjectsByType<TMP_Text>(
                     FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            text.ForceMeshUpdate(false, true);
        Canvas.ForceUpdateCanvases();
    }

    private static void EndCapture()
    {
        if (_canvases != null)
        {
            for (int i = 0; i < _canvases.Length; i++)
            {
                Canvas canvas = _canvases[i];
                if (canvas == null)
                    continue;
                canvas.renderMode = _modes[i];
                canvas.worldCamera = _canvasCameras[i];
                canvas.planeDistance = _planeDistances[i];
            }
        }
        if (_camera != null)
            _camera.targetTexture = _previousTarget;
        if (_target != null)
        {
            _target.Release();
            UnityEngine.Object.DestroyImmediate(_target);
        }
        _camera = null;
        _target = null;
        _previousTarget = null;
        _canvases = null;
        _modes = null;
        _canvasCameras = null;
        _planeDistances = null;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
#endif

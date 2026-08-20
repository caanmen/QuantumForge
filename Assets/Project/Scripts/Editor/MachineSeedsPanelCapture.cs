#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class MachineSeedsPanelCapture
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ActiveKey = "QF.MachineSeedsCapture.Active";
    private const string FramesKey = "QF.MachineSeedsCapture.Frames";
    private const string FailedKey = "QF.MachineSeedsCapture.Failed";
    private const string CompletedKey = "QF.MachineSeedsCapture.Completed";
    private const string SaveExistedKey = "QF.MachineSeedsCapture.SaveExisted";
    private const string BackupExistedKey = "QF.MachineSeedsCapture.BackupExisted";
    private const string ExitWhenCompleteKey =
        "QF.MachineSeedsCapture.ExitWhenComplete";

    private static Camera _camera;
    private static RenderTexture _target;
    private static RenderTexture _previousTarget;
    private static Canvas[] _canvases;
    private static RenderMode[] _modes;
    private static Camera[] _canvasCameras;
    private static float[] _planeDistances;

    private static string OutputDirectory =>
        Path.GetFullPath("Logs/VisualQA/MachineSeeds");
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

    [MenuItem("Tools/Quantum Forge/Machine/Capture Seeds Panel")]
    public static void Run()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            throw new InvalidOperationException(
                "La captura debe iniciar fuera de Play Mode.");
        Directory.CreateDirectory(OutputDirectory);
        BackupUserSave();
        if (!Application.isBatchMode)
            MachineSeedsPanelVisualSetup.Configure();
        MachineSeedsApprovedStyleSetup.Configure();
        MachineSeedsApprovedStyleSetup.Validate();
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetBool(FailedKey, false);
        SessionState.SetBool(CompletedKey, false);
        SessionState.SetInt(FramesKey, 0);
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        EditorApplication.isPlaying = true;
    }

    public static void RunBatch() => Run();

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
            Debug.Log("[Machine Seeds Capture] PASS | functional panel | 1080x1920");
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
            if (frames < 120)
                return;

            HideTransientOverlays();
            string path = Path.Combine(OutputDirectory,
                "seeds_panel_operational_1080x1920.png");
            RenderToPng(1080, 1920, path);
            Require(File.Exists(path) && new FileInfo(path).Length > 10000,
                "La captura del panel de semillas quedó vacía.");
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
        GameState state = GameState.I;
        state.experimentalChamberUnlocked = true;
        state.LE = 12450;
        state.Traces = 320;

        string[] repaired =
        {
            "z4_basic_chamber", "z4_seed_reading_1", "z4_seed_reading_2",
            "z4_seed_slot_3", "z4_archive_expansion_1",
            "z4_initial_stability_1", "z4_controlled_sync_1",
            "z4_tuned_containment_1", "z4_safe_rewind_1"
        };
        MachineManager.I.LoadProgressFromSave(new SaveData
        {
            machineIntroSeen = true,
            machineUnlocked = true,
            machineAllZonesUnlocked = true,
            machineFusionPanelUnlocked = true,
            machineSelectedFaceIndex = 3,
            machineRepairedNodeIds = repaired.ToList(),
            machineAnalyzedNodeIds = repaired.ToList()
        });

        state.chronalSeedDurationSeconds = 1000.0;
        state.EnsureChronalSeedSlots();
        state.chronalSeedSlots[0].hasSeed = true;
        state.chronalSeedSlots[0].progressSeconds = 720.0;
        state.chronalSeedSlots[1].hasSeed = false;
        state.chronalSeedSlots[2].hasSeed = true;
        state.chronalSeedSlots[2].progressSeconds = 350.0;
        state.chronalMatureSeedsStored = 2;
        state.chronalPureInstants = 1;
        state.chronalStableInstants = 2;
        state.chronalForcedInstants = 1;
        state.chronalInstant = new ChronalInstantState
        {
            hasInstant = true,
            stability = 68.0,
            tension = 22.0
        };

        TabsUI.Instance?.ShowRoom2();
        MachinePanelUI panel = UnityEngine.Object.FindFirstObjectByType<MachinePanelUI>(
            FindObjectsInactive.Include);
        Require(panel != null, "MachinePanelUI no disponible.");
        ActivateAncestors(panel.transform);
        panel.SelectZoneFromCube(MachineZoneType.InstantChamber);
        panel.Refresh();
        Button seeds = panel.transform.Find(
            "MachineCubeVisualRoot/MachineContextTabs/SeedsTab")
            ?.GetComponent<Button>();
        if (seeds == null)
        {
            seeds = panel.GetComponentsInChildren<Button>(true)
                .FirstOrDefault(button => button != null &&
                    button.name == "OpenAnchors");
        }
        Require(seeds != null, "No se encontró el acceso SEMILLAS.");
        seeds.onClick.Invoke();
        MachineSeedsPanelVisualUI visual =
            UnityEngine.Object.FindFirstObjectByType<MachineSeedsPanelVisualUI>(
                FindObjectsInactive.Include);
        Transform root = visual != null ? visual.transform : null;
        Require(root != null && root.gameObject.activeInHierarchy,
            "La vista operativa de semillas no se abrió.");
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
        if (saveExists) File.Copy(SavePath, SessionSavePath, true);
        else if (File.Exists(SessionSavePath)) File.Delete(SessionSavePath);
        if (backupExists) File.Copy(SaveBackupPath, SessionBackupPath, true);
        else if (File.Exists(SessionBackupPath)) File.Delete(SessionBackupPath);
    }

    private static void RestoreUserSave()
    {
        if (SessionState.GetBool(SaveExistedKey, false))
        {
            Require(File.Exists(SessionSavePath),
                "Falta la copia temporal del guardado del usuario.");
            File.Copy(SessionSavePath, SavePath, true);
        }
        else if (File.Exists(SavePath)) File.Delete(SavePath);

        if (SessionState.GetBool(BackupExistedKey, false))
        {
            Require(File.Exists(SessionBackupPath),
                "Falta la copia temporal del respaldo del usuario.");
            File.Copy(SessionBackupPath, SaveBackupPath, true);
        }
        else if (File.Exists(SaveBackupPath)) File.Delete(SaveBackupPath);

        if (File.Exists(SessionSavePath)) File.Delete(SessionSavePath);
        if (File.Exists(SessionBackupPath)) File.Delete(SessionBackupPath);
    }

    private static void BeginCapture(int width, int height)
    {
        if (_target != null && _camera != null)
            return;
        _camera = Camera.main != null ? Camera.main :
            UnityEngine.Object.FindFirstObjectByType<Camera>();
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
                if (canvas == null) continue;
                canvas.renderMode = _modes[i];
                canvas.worldCamera = _canvasCameras[i];
                canvas.planeDistance = _planeDistances[i];
            }
        }
        if (_camera != null) _camera.targetTexture = _previousTarget;
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

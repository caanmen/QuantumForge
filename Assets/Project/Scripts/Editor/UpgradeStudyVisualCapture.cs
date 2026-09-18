#if UNITY_EDITOR
using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class UpgradeStudyVisualCapture
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ActiveKey = "QF.UpgradeStudyCapture.Active";
    private const string StageKey = "QF.UpgradeStudyCapture.Stage";
    private const string FramesKey = "QF.UpgradeStudyCapture.Frames";
    private const string FailedKey = "QF.UpgradeStudyCapture.Failed";
    private const string SaveExistedKey = "QF.UpgradeStudyCapture.SaveExisted";
    private const string BackupExistedKey = "QF.UpgradeStudyCapture.BackupExisted";
    private static string OutputDirectory => Path.GetFullPath(
        "Logs/VisualQA/UpgradeStudies_PreDimensions_20260918");
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");
    private static string BackupPath => SavePath + ".bak";
    private static string SessionSave => Path.Combine(OutputDirectory, ".save.session");
    private static string SessionBackup => Path.Combine(OutputDirectory, ".save.bak.session");
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

    [MenuItem("Tools/Quantum Forge/Studies/Capture Visual Evidence")]
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
            Debug.Log("[Upgrade Studies Capture] PASS | upgrades clues + active observatory | 1080x1920 + 720x1280 | user save restored");
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
                    PrepareStudyClues();
                    Advance(1);
                    break;
                case 1:
                    Capture("01_upgrade_study_clues_es_1080x1920.png", 1080, 1920);
                    Capture("01_upgrade_study_clues_es_720x1280.png", 720, 1280);
                    PrepareActiveObservatory();
                    Advance(2);
                    break;
                case 2:
                    ValidateObservatoryGeometry();
                    Capture("02_triangle_observatory_active_es_1080x1920.png", 1080, 1920);
                    Capture("02_triangle_observatory_active_es_720x1280.png", 720, 1280);
                    Advance(3);
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

    private static void PrepareStudyClues()
    {
        Require(GameState.I != null && F2UpgradeManager.I != null &&
            TabsUI.Instance != null, "No inició el juego base.");
        SaveService.I?.CancelInvoke("Save");
        BuildingListUI list = UnityEngine.Object.FindFirstObjectByType<BuildingListUI>(
            FindObjectsInactive.Include);
        Require(list != null && list.EnsureInitialized(),
            "No se inicializaron los artefactos.");
        foreach (string id in new[]
        {
            "vacuum_observer", "casimir_panel", "fluctuation_antenna"
        })
        {
            BuildingState building = GameState.I.GetBuildingState(id);
            Require(building != null, "Falta artefacto " + id);
            building.level = 1;
        }
        GameState.I.LE = 1000000.0;
        GameState.I.Traces = 5000.0;
        GameState.I.triangleEnergy = 0.0;
        F2UpgradeManager.I.DebugResetAllPurchases();
        UpgradeStudySystem.ResetForNewRun(GameState.I);
        LocalizationManager.I?.SetLanguage(LocalizationManager.Language.ES);
        TabsUI.Instance.ShowMejoras();
        RefreshUi();
        VerticalUpgradesScreenUI upgrades = UnityEngine.Object.FindFirstObjectByType<
            VerticalUpgradesScreenUI>(FindObjectsInactive.Include);
        Require(upgrades != null && upgrades.content != null,
            "Falta Mejoras vertical.");
        ScrollRect scroll = upgrades.GetComponentInChildren<ScrollRect>(true);
        if (scroll != null) scroll.verticalNormalizedPosition = 1f;
        Canvas.ForceUpdateCanvases();
    }

    private static void PrepareActiveObservatory()
    {
        GameState state = GameState.I;
        state.upgradeStudies.discoveredIds.Add("triangle_unlock_1");
        Require(F2UpgradeManager.I.TryBuy("triangle_unlock_1"),
            "No se activó Acople para la captura.");
        Require(state.SetTriangleCircuit(TriangleCircuitType.Energy),
            "No se activó Energía.");
        state.triangleSynchronization = 1f;
        state.triangleEnergy = 480.0;
        UpgradeStudySystem.RecordCircuitSynchronized(state, TriangleCircuitType.Energy);
        Require(UpgradeStudySystem.TryStartStudy(state,
            "study_triangle_impulse_tuning"), "No inició el estudio visual.");
        UpgradeStudySystem.Advance(state, 120.0);
        TabsUI.Instance.ShowMejoras();
        RefreshUi();
        VerticalUpgradesScreenUI upgrades = UnityEngine.Object.FindFirstObjectByType<
            VerticalUpgradesScreenUI>(FindObjectsInactive.Include);
        Require(upgrades != null, "Falta la pantalla vertical de Mejoras.");
        ScrollRect scroll = upgrades.GetComponentInChildren<ScrollRect>(true);
        if (scroll != null) scroll.verticalNormalizedPosition = 1f;
        Canvas.ForceUpdateCanvases();
    }

    private static void RefreshUi()
    {
        foreach (F2UpgradeRowUI row in UnityEngine.Object.FindObjectsByType<
            F2UpgradeRowUI>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            row.RefreshNow();
        foreach (VerticalUpgradesScreenUI screen in UnityEngine.Object.FindObjectsByType<
            VerticalUpgradesScreenUI>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            screen.RefreshNow();
        foreach (VerticalTriangleObservatoryUI observatory in
            UnityEngine.Object.FindObjectsByType<VerticalTriangleObservatoryUI>(
                FindObjectsInactive.Include, FindObjectsSortMode.None))
            observatory.RefreshNow();
    }

    private static void ValidateObservatoryGeometry()
    {
        VerticalTriangleObservatoryUI observatory =
            UnityEngine.Object.FindFirstObjectByType<VerticalTriangleObservatoryUI>(
                FindObjectsInactive.Include);
        Require(observatory != null && observatory.tuningHintText != null &&
            observatory.tuneButton != null && observatory.conclusionButton != null,
            "La consola de sintonizacion no esta conectada.");

        RectTransform root = observatory.transform as RectTransform;
        RectTransform hint = observatory.tuningHintText.rectTransform;
        RectTransform tune = observatory.tuneButton.transform as RectTransform;
        RectTransform conclusion =
            observatory.conclusionButton.transform as RectTransform;
        Require(root != null && tune != null && conclusion != null,
            "La consola perdio su geometria rectangular.");

        Canvas.ForceUpdateCanvases();
        Bounds hintBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(
            root, hint);
        Bounds tuneBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(
            root, tune);
        Bounds conclusionBounds =
            RectTransformUtility.CalculateRelativeRectTransformBounds(root, conclusion);
        float nearestButtonTop = Mathf.Max(
            tuneBounds.max.y, conclusionBounds.max.y);
        Require(hintBounds.min.y >= nearestButtonTop + 8f,
            "El texto de sintonizacion invade el boton de accion.");
        Require(hintBounds.min.y >= root.rect.yMin &&
            hintBounds.max.y <= root.rect.yMax,
            "El texto de sintonizacion queda fuera de la consola.");
        Vector2 preferred = observatory.tuningHintText.GetPreferredValues(
            observatory.tuningHintText.text, hint.rect.width, 0f);
        Require(preferred.y <= hint.rect.height + 1f,
            "El texto completo de sintonizacion no cabe en su area reservada.");

        VerticalUpgradesScreenUI upgrades =
            UnityEngine.Object.FindFirstObjectByType<VerticalUpgradesScreenUI>(
                FindObjectsInactive.Include);
        ScrollRect scroll = upgrades != null
            ? upgrades.GetComponentInChildren<ScrollRect>(true)
            : null;
        Require(scroll != null &&
            scroll.movementType == ScrollRect.MovementType.Clamped,
            "La lista de Mejoras todavia permite sobrepasar sus extremos.");
    }

    private static void Advance(int next)
    {
        SessionState.SetInt(StageKey, next);
        SessionState.SetInt(FramesKey, 0);
        stageStartedAt = EditorApplication.timeSinceStartup;
    }

    private static void Capture(string fileName, int width, int height)
    {
        HideTransientOverlays();
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
            Texture2D image = new Texture2D(width, height, TextureFormat.RGB24, false);
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
        Debug.Log("[Upgrade Studies Capture] PNG | " + path);
    }

    private static void HideTransientOverlays()
    {
        foreach (string name in new[]
        {
            "PresentationReturnReportRuntime", "TriangleOfflineReportRuntime"
        })
        {
            GameObject target = FindNamed(name);
            if (target != null) target.SetActive(false);
        }
    }

    private static void RecalculateCanvasScalers(Canvas[] canvases)
    {
        MethodInfo handle = typeof(CanvasScaler).GetMethod("Handle",
            BindingFlags.Instance | BindingFlags.NonPublic);
        if (handle == null) return;
        foreach (Canvas canvas in canvases)
        {
            CanvasScaler scaler = canvas != null ? canvas.GetComponent<CanvasScaler>() : null;
            if (scaler != null && scaler.enabled) handle.Invoke(scaler, null);
        }
    }

    private static GameObject FindNamed(string name)
    {
        foreach (Transform item in UnityEngine.Object.FindObjectsByType<Transform>(
            FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (item.name == name) return item.gameObject;
        return null;
    }

    private static void BackupUserSave()
    {
        bool saveExists = File.Exists(SavePath);
        bool backupExists = File.Exists(BackupPath);
        SessionState.SetBool(SaveExistedKey, saveExists);
        SessionState.SetBool(BackupExistedKey, backupExists);
        if (saveExists) File.Copy(SavePath, SessionSave, true);
        if (backupExists) File.Copy(BackupPath, SessionBackup, true);
    }

    private static void RestoreUserSave()
    {
        if (SessionState.GetBool(SaveExistedKey, false))
            File.Copy(SessionSave, SavePath, true);
        else if (File.Exists(SavePath)) File.Delete(SavePath);
        if (SessionState.GetBool(BackupExistedKey, false))
            File.Copy(SessionBackup, BackupPath, true);
        else if (File.Exists(BackupPath)) File.Delete(BackupPath);
        if (File.Exists(SessionSave)) File.Delete(SessionSave);
        if (File.Exists(SessionBackup)) File.Delete(SessionBackup);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
#endif

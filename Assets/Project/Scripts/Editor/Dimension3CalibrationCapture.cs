#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public static class Dimension3CalibrationCapture
{
    private const string ActiveKey = "QF.D3CalibrationCapture.Active";
    private const string FrameKey = "QF.D3CalibrationCapture.Frame";
    private const string FailureKey = "QF.D3CalibrationCapture.Failed";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory =>
        Path.GetFullPath("Logs/VisualQA/Dimension3/Calibration");
    private static string Output1080 => Path.Combine(
        OutputDirectory, "Calibration_candidate_1080x1920.png");
    private static string Output720 => Path.Combine(
        OutputDirectory, "Calibration_candidate_720x1280.png");

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

    [MenuItem("Tools/Quantum Forge/Dimension 3/Capture Calibration")]
    public static void Run()
    {
        Directory.CreateDirectory(OutputDirectory);
        DeleteIfExists(Output1080);
        DeleteIfExists(Output720);
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
            SaveService.SuppressWritesForVisualQa = true;
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            SaveService.SuppressWritesForVisualQa = false;
            SessionState.SetBool(ActiveKey, false);
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            bool failed = SessionState.GetBool(FailureKey, false);
            Debug.Log(failed
                ? "[D3 Calibration Capture] FAIL"
                : "[D3 Calibration Capture] PASS | real Main + real controller + 35/40/25 + 1080x1920 + 720x1280 | " + Output1080);
            EditorApplication.Exit(failed ? 1 : 0);
        }
    }

    private static void Tick()
    {
        try
        {
            int frame = SessionState.GetInt(FrameKey, 0) + 1;
            SessionState.SetInt(FrameKey, frame);
            if (frame >= 12)
                SuppressReports();
            if (frame == 20)
                PrepareRealState();
            if (frame != 52)
                return;

            ValidateVisibleState();
            RenderToPng(1080, 1920, Output1080);
            RenderToPng(720, 1280, Output720);
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

    private static void PrepareRealState()
    {
        GameState state = GameState.I != null
            ? GameState.I
            : UnityEngine.Object.FindFirstObjectByType<GameState>();
        if (state == null)
            throw new InvalidOperationException("GameState no existe.");

        state.dimension03Unlocked = true;
        state.LE = 3250000d;
        state.Traces = 4200d;
        Dimension3System.EnsureState(state);
        state.dimension3.firstEntrySeen = true;
        state.dimension3.presentation.onboardingStage = (int)D3OnboardingStage.Completed;
        state.dimension3.presentation.lastScreenId = PresentationFeatureIds.D3Calibration;

        D3FacilityState bank = null;
        for (int i = 0; i < state.dimension3.facilities.Count; i++)
        {
            D3FacilityState candidate = state.dimension3.facilities[i];
            if (candidate != null &&
                candidate.facilityId == Dimension3Catalog.FacilityProcessBank)
            {
                bank = candidate;
                break;
            }
        }
        if (bank == null)
            throw new InvalidOperationException("El Banco de Procesos no existe.");
        bank.built = true;
        bank.level = 4;

        state.dimension3.automatons.Clear();
        state.dimension3.totalAssembledByMk.Clear();
        state.dimension3.assignments.Clear();
        D3InventorySystem.AddAutomatons(
            state.dimension3, 1, Dimension3Catalog.TraitNormal, 40L);
        D3InventorySystem.AddAutomatons(
            state.dimension3, 2, Dimension3Catalog.TraitFast, 10L);
        D3InventorySystem.AddAutomatons(
            state.dimension3, 3, Dimension3Catalog.TraitNormal, 5L);
        D3InventorySystem.AddAutomatons(
            state.dimension3, 4, Dimension3Catalog.TraitNormal, 1L);
        D3PresentationRules.EnsurePresentationState(state);

        TabsUI tabs = TabsUI.Instance != null
            ? TabsUI.Instance
            : UnityEngine.Object.FindFirstObjectByType<TabsUI>();
        if (tabs == null)
            throw new InvalidOperationException("TabsUI no existe.");
        tabs.ShowDimension3();

        Dimension3PanelUI dimension = UnityEngine.Object.FindFirstObjectByType<Dimension3PanelUI>(
            FindObjectsInactive.Include);
        if (dimension == null || dimension.calibrationPanel == null)
            throw new InvalidOperationException("La pantalla de Calibración está incompleta.");

        dimension.gameObject.SetActive(true);
        if (dimension.firstEntryRoot != null)
            dimension.firstEntryRoot.SetActive(false);
        HideSiblingViews(dimension, dimension.calibrationPanel.gameObject);
        dimension.calibrationPanel.Open();
        dimension.calibrationPanel.valueASlider.value = 35f;
        dimension.calibrationPanel.valueBSlider.value = 40f;
        dimension.calibrationPanel.valueCSlider.value = 25f;
        dimension.calibrationPanel.recordPartButton.onClick.Invoke();
        HideUnrelated();
        Canvas.ForceUpdateCanvases();
    }

    private static void HideSiblingViews(Dimension3PanelUI panel, GameObject keep)
    {
        for (int i = 0; i < panel.transform.childCount; i++)
        {
            Transform child = panel.transform.GetChild(i);
            if (child.gameObject != keep && child.name.StartsWith("D3_"))
                child.gameObject.SetActive(false);
        }
        keep.SetActive(true);
    }

    private static void ValidateVisibleState()
    {
        D3CalibrationPanelUI panel = UnityEngine.Object.FindFirstObjectByType<D3CalibrationPanelUI>(
            FindObjectsInactive.Include);
        if (panel == null || !panel.gameObject.activeInHierarchy)
            throw new InvalidOperationException("Mesa de Calibración no está visible.");
        D3CalibrationStaticSkinUI skin = panel.GetComponentInChildren<D3CalibrationStaticSkinUI>(true);
        if (skin == null || skin.referencePlate == null || skin.referencePlate.texture == null ||
            skin.referencePlate.raycastTarget)
            throw new InvalidOperationException("La composición estática no cumple el contrato.");
        if (Mathf.RoundToInt(panel.valueASlider.value) != 35 ||
            Mathf.RoundToInt(panel.valueBSlider.value) != 40 ||
            Mathf.RoundToInt(panel.valueCSlider.value) != 25)
            throw new InvalidOperationException("La distribución real no es 35/40/25.");
        if (panel.instructionsText == null || !panel.instructionsText.text.Contains("100"))
            throw new InvalidOperationException("El total real de calibración no es 100.");
        if (panel.readingsText == null || !panel.readingsText.text.Contains("Chasis: 48"))
            throw new InvalidOperationException("La lectura real de Chasis no es 48.");
        if (panel.previewText == null ||
            !panel.previewText.text.ToUpperInvariant().Contains("COORDINADOR"))
            throw new InvalidOperationException("La afinidad real no es Coordinador.");
        if (TabsUI.Instance != null && TabsUI.Instance.verticalNavigation != null &&
            !TabsUI.Instance.verticalNavigation.NavigationSuppressed)
            throw new InvalidOperationException("La navegación global debe estar oculta.");
    }

    private static void HideUnrelated()
    {
        string[] hide =
        {
            "Panel_Generacion", "Panel_Lab", "Panel_Logros", "Panel_Ajustes",
            "Dimension1Panel", "Dimension2Panel", "BottomDrawer", "PrimaryNavigationSlot",
            "SecondaryNavigationSlot", "MachineContextTabs", "PrestigePanel",
            "MetaPrestigePanel", "QA_PanelRoot", "QA_ToolsButton"
        };
        foreach (string name in hide)
        {
            Transform target = FindSceneTransform(name);
            if (target != null)
                target.gameObject.SetActive(false);
        }
    }

    private static void SuppressReports()
    {
        PresentationReturnReportService.Consume();
        foreach (PresentationReturnReportUI report in
            UnityEngine.Object.FindObjectsByType<PresentationReturnReportUI>(
                FindObjectsInactive.Include, FindObjectsSortMode.None))
            report.gameObject.SetActive(false);
        foreach (TriangleOfflineReportUI report in
            UnityEngine.Object.FindObjectsByType<TriangleOfflineReportUI>(
                FindObjectsInactive.Include, FindObjectsSortMode.None))
            report.gameObject.SetActive(false);
    }

    private static void RenderToPng(int width, int height, string path)
    {
        Camera camera = Camera.main != null
            ? Camera.main
            : UnityEngine.Object.FindFirstObjectByType<Camera>(FindObjectsInactive.Include);
        if (camera == null)
            throw new InvalidOperationException("No hay cámara para captura.");

        Canvas[] canvases = UnityEngine.Object.FindObjectsByType<Canvas>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        var modes = new RenderMode[canvases.Length];
        var cameras = new Camera[canvases.Length];
        for (int i = 0; i < canvases.Length; i++)
        {
            modes[i] = canvases[i].renderMode;
            cameras[i] = canvases[i].worldCamera;
            canvases[i].renderMode = RenderMode.ScreenSpaceCamera;
            canvases[i].worldCamera = camera;
            canvases[i].planeDistance = 1f;
        }

        RenderTexture target = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        RenderTexture previousTarget = camera.targetTexture;
        RenderTexture previousActive = RenderTexture.active;
        try
        {
            camera.targetTexture = target;
            for (int pass = 0; pass < 6; pass++)
            {
                Canvas.ForceUpdateCanvases();
                camera.Render();
                GL.Flush();
            }
            RenderTexture.active = target;
            Texture2D image = new Texture2D(width, height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            image.Apply();
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
            }
        }
    }

    private static Transform FindSceneTransform(string name)
    {
        foreach (Transform transform in Resources.FindObjectsOfTypeAll<Transform>())
            if (transform.gameObject.scene.IsValid() && transform.name == name)
                return transform;
        return null;
    }

    private static void DeleteIfExists(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
    }
}
#endif

#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public static class Dimension3ProductionFloorCapture
{
    private const string ActiveKey = "QF.D3ProductionFloorCapture.Active";
    private const string FrameKey = "QF.D3ProductionFloorCapture.Frame";
    private const string FailureKey = "QF.D3ProductionFloorCapture.Failed";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory =>
        Path.GetFullPath("Logs/VisualQA/Dimension3/ProductionFloor");
    private static string Output1080 => Path.Combine(
        OutputDirectory, "ProductionFloor_candidate_1080x1920.png");
    private static string Output720 => Path.Combine(
        OutputDirectory, "ProductionFloor_candidate_720x1280.png");

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

    [MenuItem("Tools/Quantum Forge/Dimension 3/Capture Production Floor")]
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
                ? "[D3 Production Floor Capture] FAIL"
                : "[D3 Production Floor Capture] PASS | real Main + real controller + 1080x1920 + 720x1280 | " + Output1080);
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

            ValidateVisible();
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
        state.dimension3.presentation.lastScreenId = PresentationFeatureIds.D3Factory;

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
        bank.level = 5;

        foreach (string partId in Dimension3Catalog.PartIds)
        {
            long amount = D3InventorySystem.GetPartAmount(state.dimension3, partId, 1);
            if (amount < 3L)
                D3InventorySystem.AddParts(state.dimension3, partId, 1, 3L - amount);
        }

        // Escenario visual canónico de la referencia: 56 autómatas reales,
        // repartidos sin depender de la partida personal cargada.
        state.dimension3.automatons.Clear();
        state.dimension3.totalAssembledByMk.Clear();
        state.dimension3.assignments.Clear();
        for (int i = 0; i < state.dimension3.queues.Count; i++)
            if (state.dimension3.queues[i] != null &&
                state.dimension3.queues[i].jobs != null)
                state.dimension3.queues[i].jobs.Clear();
        D3InventorySystem.AddAutomatons(
            state.dimension3, 1, Dimension3Catalog.TraitNormal, 40L);
        D3InventorySystem.AddAutomatons(
            state.dimension3, 2, Dimension3Catalog.TraitFast, 10L);
        D3InventorySystem.AddAutomatons(
            state.dimension3, 3, Dimension3Catalog.TraitNormal, 5L);
        D3InventorySystem.AddAutomatons(
            state.dimension3, 4, Dimension3Catalog.TraitNormal, 1L);
        D3InventorySystem.AddAssemblyCount(state.dimension3, 1, 40L);
        D3InventorySystem.AddAssemblyCount(state.dimension3, 2, 10L);
        D3InventorySystem.AddAssemblyCount(state.dimension3, 3, 10L);
        D3InventorySystem.AddAssemblyCount(state.dimension3, 4, 1L);
        D3PresentationRules.EnsurePresentationState(state);

        TabsUI tabs = TabsUI.Instance != null
            ? TabsUI.Instance
            : UnityEngine.Object.FindFirstObjectByType<TabsUI>();
        if (tabs == null)
            throw new InvalidOperationException("TabsUI no existe.");
        tabs.ShowDimension3();

        Dimension3PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension3PanelUI>(
            FindObjectsInactive.Include);
        if (panel == null || panel.factoryRoot == null)
            throw new InvalidOperationException("Dimension3PanelUI está incompleto.");

        panel.gameObject.SetActive(true);
        panel.firstEntryRoot.SetActive(false);
        panel.factoryRoot.SetActive(true);
        HideSiblingViews(panel);
        panel.Refresh();
        HideUnrelated();
        Canvas.ForceUpdateCanvases();
    }

    private static void HideSiblingViews(Dimension3PanelUI panel)
    {
        for (int i = 0; i < panel.transform.childCount; i++)
        {
            Transform child = panel.transform.GetChild(i);
            if (child.gameObject != panel.factoryRoot && child.name.StartsWith("D3_") &&
                child.gameObject != panel.closeDimension3Button.gameObject)
                child.gameObject.SetActive(false);
        }
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

    private static void ValidateVisible()
    {
        Dimension3PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension3PanelUI>(
            FindObjectsInactive.Include);
        if (panel == null || !panel.gameObject.activeInHierarchy ||
            panel.factoryRoot == null || !panel.factoryRoot.activeInHierarchy)
            throw new InvalidOperationException("La Planta no está visible.");
        if (panel.productionFloorSkin == null)
            throw new InvalidOperationException("Falta el skin visual de Planta.");
        if (TabsUI.Instance != null && TabsUI.Instance.verticalNavigation != null &&
            !TabsUI.Instance.verticalNavigation.NavigationSuppressed)
            throw new InvalidOperationException(
                "La navegación global debe estar oculta dentro de Dimensión 3.");
        RawImage plate = panel.productionFloorSkin.GetComponent<RawImage>();
        if (plate == null || plate.texture == null || plate.raycastTarget)
            throw new InvalidOperationException("El backplate no cumple el contrato visual.");
        Button[] actions =
        {
            panel.produceChassisButton, panel.produceMotorButton, panel.produceToolButton,
            panel.produceControlButton, panel.produceRegulatorButton,
            panel.assembleMk1Button, panel.addAssignmentButton,
            panel.removeAssignmentButton, panel.openQueuesButton
        };
        for (int i = 0; i < actions.Length; i++)
            if (actions[i] == null || !actions[i].gameObject.activeInHierarchy)
                throw new InvalidOperationException("Acción real no visible en índice " + i + ".");
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

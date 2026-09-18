#if UNITY_EDITOR
using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public static class Dimension3ResearchCapture
{
    private const string ActiveKey = "QF.D3ResearchCapture.Active";
    private const string FrameKey = "QF.D3ResearchCapture.Frame";
    private const string FailureKey = "QF.D3ResearchCapture.Failed";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory =>
        Path.GetFullPath("Logs/VisualQA/Dimension3/Research");
    private static string Output1080 => Path.Combine(
        OutputDirectory, "Research_candidate_1080x1920.png");
    private static string Output720 => Path.Combine(
        OutputDirectory, "Research_candidate_720x1280.png");

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

    [MenuItem("Tools/Quantum Forge/Dimension 3/Capture Research")]
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

    public static void RunBatch() => Run();

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
                ? "[D3 Research Capture] FAIL"
                : "[D3 Research Capture] PASS | real Main + real D3ResearchPanelUI + Control V4 + 40 MK1 Normal + 1080x1920 + 720x1280 | " + Output1080);
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
        state.dimension3.presentation.lastScreenId = PresentationFeatureIds.D3Research;
        state.dimension3.research.Clear();
        state.dimension3.assignments.Clear();
        ClearQueues(state.dimension3);

        D3FacilityState bank = D3FacilitySystem.GetFacility(
            state.dimension3, Dimension3Catalog.FacilityProcessBank);
        bank.built = true;
        bank.level = 4;

        state.dimension3.automatons.Clear();
        state.dimension3.totalAssembledByMk.Clear();
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

        Dimension3PanelUI dimension = UnityEngine.Object.FindFirstObjectByType<Dimension3PanelUI>(
            FindObjectsInactive.Include);
        if (dimension == null || dimension.researchPanel == null)
            throw new InvalidOperationException("La pantalla de investigación está incompleta.");
        dimension.gameObject.SetActive(true);
        if (dimension.firstEntryRoot != null)
            dimension.firstEntryRoot.SetActive(false);
        HideSiblingViews(dimension, dimension.researchPanel.gameObject);

        D3ResearchPanelUI panel = dimension.researchPanel;
        panel.Open();
        SetDropdownByText(panel.partDropdown, "Módulo de Control");
        SetDropdownByText(panel.versionDropdown, "V4");
        SetDropdownByText(panel.teamMkDropdown, "MK1");
        SetDropdownByText(panel.teamTraitDropdown, "Normal");
        for (int i = 0; i < 40; i++)
            panel.addTeamButton.onClick.Invoke();

        HideUnrelated();
        Canvas.ForceUpdateCanvases();
    }

    private static void ClearQueues(Dimension3State state)
    {
        for (int i = 0; i < state.queues.Count; i++)
            if (state.queues[i] != null && state.queues[i].jobs != null)
                state.queues[i].jobs.Clear();
    }

    private static void SetDropdownByText(TMP_Dropdown dropdown, string value)
    {
        if (dropdown == null)
            throw new InvalidOperationException("Falta selector para " + value + ".");
        for (int i = 0; i < dropdown.options.Count; i++)
            if (string.Equals(dropdown.options[i].text, value,
                    StringComparison.OrdinalIgnoreCase))
            {
                dropdown.value = i;
                dropdown.RefreshShownValue();
                return;
            }
        throw new InvalidOperationException("El selector no contiene " + value + ".");
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
        GameState state = GameState.I;
        D3ResearchPanelUI panel = UnityEngine.Object.FindFirstObjectByType<D3ResearchPanelUI>(
            FindObjectsInactive.Include);
        if (state == null || panel == null || !panel.gameObject.activeInHierarchy)
            throw new InvalidOperationException("Investigación de piezas no está visible.");
        D3ResearchStaticSkinUI skin = panel.GetComponentInChildren<D3ResearchStaticSkinUI>(true);
        if (skin == null || skin.referencePlate == null || skin.referencePlate.texture == null ||
            skin.referencePlate.raycastTarget)
            throw new InvalidOperationException("La composición estática no cumple el contrato.");

        D3ResearchDefinition definition = Dimension3Catalog.GetResearchDefinition(
            Dimension3Catalog.PartControl, 4);
        if (definition == null || definition.leCost != 100000d ||
            definition.tracesCost != 100d || definition.durationSeconds != 1200d ||
            definition.minimumPower != 50d)
            throw new InvalidOperationException("El catálogo real de Control V4 no coincide.");
        // La simulación puede sumar producción pasiva entre la preparación y la captura.
        // Al no existir trabajo en cola, sólo sería incorrecto que los saldos bajaran.
        if (state.LE + .001 < 3250000d || state.Traces + .001 < 4200d)
            throw new InvalidOperationException("La investigación consumió recursos antes de iniciarse.");
        if (D3InventorySystem.GetAssemblyCount(state.dimension3, 3) < 10L)
            throw new InvalidOperationException("No se cumple el requisito real de 10 MK3.");
        if (!D3ResearchSystem.ValidatePrerequisites(
                state, Dimension3Catalog.PartControl, 4, out string reason))
            throw new InvalidOperationException("Control V4 no está habilitado: " + reason);

        string team = panel.teamText == null ? "" : panel.teamText.text;
        if (!team.Contains("MK1 normal ×40") || !team.Contains("Potencia: 50"))
            throw new InvalidOperationException("El equipo real no coincide: " + team);
        string queue = panel.queueText == null ? "" : panel.queueText.text;
        if (!queue.Contains("Sin investigaciones"))
            throw new InvalidOperationException("La cola real no está vacía: " + queue);
        if (panel.queueResearchButton == null || !panel.queueResearchButton.interactable ||
            panel.cancelResearchButton == null || panel.cancelResearchButton.interactable)
            throw new InvalidOperationException("Los estados reales de iniciar/cancelar no coinciden.");
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

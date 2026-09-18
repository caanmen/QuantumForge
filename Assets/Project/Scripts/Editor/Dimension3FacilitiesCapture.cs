#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public static class Dimension3FacilitiesCapture
{
    private const string ActiveKey = "QF.D3FacilitiesCapture.Active";
    private const string FrameKey = "QF.D3FacilitiesCapture.Frame";
    private const string FailureKey = "QF.D3FacilitiesCapture.Failed";
    private const string ModeKey = "QF.D3FacilitiesCapture.Mode";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory =>
        Path.GetFullPath(SessionState.GetInt(ModeKey, 0) == 3
            ? "Logs/VisualQA/Dimension3/FacilitiesPort"
            : SessionState.GetInt(ModeKey, 0) == 2
                ? "Logs/VisualQA/Dimension3/FacilitiesImprovement"
            : SessionState.GetInt(ModeKey, 0) == 1
                ? "Logs/VisualQA/Dimension3/FacilitiesConsole"
                : "Logs/VisualQA/Dimension3/FacilitiesNucleus");
    private static string Output1080 => Path.Combine(
        OutputDirectory, SessionState.GetInt(ModeKey, 0) == 3
            ? "FacilitiesPort_candidate_1080x1920.png"
            : SessionState.GetInt(ModeKey, 0) == 2
                ? "FacilitiesImprovement_candidate_1080x1920.png"
            : SessionState.GetInt(ModeKey, 0) == 1
                ? "FacilitiesConsole_candidate_1080x1920.png"
                : "FacilitiesNucleus_candidate_1080x1920.png");
    private static string Output720 => Path.Combine(
        OutputDirectory, SessionState.GetInt(ModeKey, 0) == 3
            ? "FacilitiesPort_candidate_720x1280.png"
            : SessionState.GetInt(ModeKey, 0) == 2
                ? "FacilitiesImprovement_candidate_720x1280.png"
            : SessionState.GetInt(ModeKey, 0) == 1
                ? "FacilitiesConsole_candidate_720x1280.png"
                : "FacilitiesNucleus_candidate_720x1280.png");

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

    [MenuItem("Tools/Quantum Forge/Dimension 3/Capture Facilities Nucleus")]
    public static void Run()
    {
        RunMode(0);
    }

    public static void RunConsole()
    {
        RunMode(1);
    }

    public static void RunImprovement()
    {
        RunMode(2);
    }

    public static void RunPort()
    {
        RunMode(3);
    }

    private static void RunMode(int mode)
    {
        SessionState.SetInt(ModeKey, mode);
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
    public static void RunConsoleBatch() => RunConsole();
    public static void RunImprovementBatch() => RunImprovement();
    public static void RunPortBatch() => RunPort();

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
            int mode = SessionState.GetInt(ModeKey, 0);
            Debug.Log(failed
                ? "[D3 Facilities Capture] FAIL"
                : mode == 3
                    ? "[D3 Facilities Port Capture] PASS | Puerto N1 enlazado + 5 MK1 asignados + 4 estables + capacidad 2 + 1080x1920 + 720x1280 | " + Output1080
                    : mode == 2
                    ? "[D3 Facilities Improvement Capture] PASS | Consola N2 + mejora N3 + 2 MK2 Rapidos estables + 2.83 + 1080x1920 + 720x1280 | " + Output1080
                    : mode == 1
                        ? "[D3 Facilities Console Capture] PASS | Consola N2 + 2 MK2 Rapidos estables + 2.83 + 1080x1920 + 720x1280 | " + Output1080
                        : "[D3 Facilities Capture] PASS | Nucleo N2 + MK4 Normal estable + 5.92 + 1080x1920 + 720x1280 | " + Output1080);
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
                PrepareRealState(SessionState.GetInt(ModeKey, 0));
            if (frame != 52)
                return;
            ValidateVisibleState(SessionState.GetInt(ModeKey, 0));
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

    private static void PrepareRealState(int mode)
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
        state.dimension3.presentation.lastScreenId = PresentationFeatureIds.D3Facilities;
        state.dimension3.successfulAutomationExecutions = 0L;
        state.dimension3.autonomyCoreIntegrated = false;
        state.dimension3.assignments.Clear();
        state.dimension01Unlocked = true;
        state.dimension1ManualSimpleDestinationIds.Clear();
        state.dimension1ManualSimpleDestinationIds.Add("mineral_belt");
        ClearQueues(state.dimension3);

        BuildDirect(state.dimension3, Dimension3Catalog.FacilityProcessBank, 5);
        BuildDirect(state.dimension3, Dimension3Catalog.FacilityProductionConsole, 2);
        BuildDirect(state.dimension3, Dimension3Catalog.FacilityDiagnosticBank, 4);
        BuildDirect(state.dimension3, Dimension3Catalog.FacilityExpeditionPort, 1);
        BuildDirect(state.dimension3, Dimension3Catalog.FacilityAutomationCore, 2);

        state.dimension3.research.Clear();
        for (int part = 0; part < Dimension3Catalog.PartIds.Length; part++)
            state.dimension3.research.Add(new D3ResearchState
            {
                researchId = Dimension3Catalog.GetResearchId(
                    Dimension3Catalog.PartIds[part], 4),
                completed = true
            });

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
        for (int mk = 1; mk <= 4; mk++)
            D3InventorySystem.AddAssemblyCount(state.dimension3, mk, mk == 1 ? 40L : 10L);

        bool console = mode == 1 || mode == 2;
        bool port = mode == 3;
        string selectedFacility = port
            ? Dimension3Catalog.FacilityExpeditionPort
            : console
                ? Dimension3Catalog.FacilityProductionConsole
                : Dimension3Catalog.FacilityAutomationCore;
        string selectedChannel = port
            ? Dimension3Catalog.ChannelPortCapacity
            : console
                ? Dimension3Catalog.ChannelConsoleCapacity
                : Dimension3Catalog.ChannelCoreCoordination;
        int selectedMk = port ? 1 : console ? 2 : 4;
        string selectedTrait = console
            ? Dimension3Catalog.TraitFast
            : Dimension3Catalog.TraitNormal;
        long selectedAmount = port ? 5L : console ? 2L : 1L;
        if (!Dimension3System.TrySetFacilityAssignment(
                state, selectedFacility, selectedChannel, selectedMk,
                selectedTrait, selectedAmount, out string reason))
            throw new InvalidOperationException("No se pudo preparar la asignación canónica: " + reason);
        Dimension3System.Tick(state, 31d);
        if (port)
        {
            D3AssignmentState portAssignment = D3FacilitySystem.GetAssignment(
                state.dimension3, selectedFacility, selectedChannel,
                selectedMk, selectedTrait);
            if (portAssignment == null)
                throw new InvalidOperationException("No existe la asignación canónica del Puerto.");
            portAssignment.stabilizedAmount = 4L;
            portAssignment.stabilizationRemainingSeconds =
                Dimension3Catalog.AssignmentStabilizationSeconds;
        }
        D3PresentationRules.EnsurePresentationState(state);

        TabsUI tabs = TabsUI.Instance != null
            ? TabsUI.Instance
            : UnityEngine.Object.FindFirstObjectByType<TabsUI>();
        if (tabs == null)
            throw new InvalidOperationException("TabsUI no existe.");
        tabs.ShowDimension3();

        Dimension3PanelUI dimension = UnityEngine.Object.FindFirstObjectByType<Dimension3PanelUI>(
            FindObjectsInactive.Include);
        if (dimension == null || dimension.facilitiesPanel == null)
            throw new InvalidOperationException("La pantalla de instalaciones está incompleta.");
        dimension.gameObject.SetActive(true);
        if (dimension.firstEntryRoot != null)
            dimension.firstEntryRoot.SetActive(false);
        HideSiblingViews(dimension, dimension.facilitiesPanel.gameObject);
        D3FacilitiesPanelUI panel = dimension.facilitiesPanel;
        panel.Open();
        SetDropdownByText(panel.facilityDropdown, port
            ? "PUERTO DE EXPEDICIONES"
            : console ? "CONSOLA DE PRODUCCIÓN" : "NÚCLEO DE AUTOMATIZACIÓN");
        SetDropdownByText(panel.mkDropdown, port ? "MK1" : console ? "MK2" : "MK4");
        SetDropdownByText(panel.traitDropdown, console ? "Rápido" : "Normal");
        SetDropdownByText(panel.channelDropdown, port || console ? "Capacidad" : "Coordinación");
        D3FacilitiesStaticSkinUI skin =
            panel.GetComponentInChildren<D3FacilitiesStaticSkinUI>(true);
        if (skin == null)
            throw new InvalidOperationException("Falta el skin visual de instalaciones.");
        skin.SetImprovementVariant(mode == 2);
        if (port && panel.noticeText != null)
            panel.noticeText.text = "Asignación añadida; estabiliza en 30 segundos.";
        HideUnrelated();
        Canvas.ForceUpdateCanvases();
    }

    private static void BuildDirect(Dimension3State state, string facilityId, int level)
    {
        D3FacilityState facility = D3FacilitySystem.GetFacility(state, facilityId);
        facility.built = level > 0;
        facility.level = level;
    }

    private static void ClearQueues(Dimension3State state)
    {
        for (int i = 0; i < state.queues.Count; i++)
            if (state.queues[i] != null && state.queues[i].jobs != null)
                state.queues[i].jobs.Clear();
    }

    private static void SetDropdownByText(TMPro.TMP_Dropdown dropdown, string value)
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

    private static void ValidateVisibleState(int mode)
    {
        D3FacilitiesPanelUI panel = UnityEngine.Object.FindFirstObjectByType<D3FacilitiesPanelUI>(
            FindObjectsInactive.Include);
        if (panel == null || !panel.gameObject.activeInHierarchy)
            throw new InvalidOperationException("Instalaciones conectadas no está visible.");
        D3FacilitiesStaticSkinUI skin = panel.GetComponentInChildren<D3FacilitiesStaticSkinUI>(true);
        if (skin == null || skin.referencePlate == null || skin.referencePlate.texture == null ||
            skin.referencePlate.raycastTarget)
            throw new InvalidOperationException("La composición estática no cumple el contrato.");
        string status = panel.statusText == null ? "" : panel.statusText.text;
        bool console = mode == 1 || mode == 2;
        bool port = mode == 3;
        string expectedFacility = port ? "PUERTO DE EXPEDICIONES" :
            console ? "CONSOLA DE PRODUCCIÓN" : "NÚCLEO DE AUTOMATIZACIÓN";
        string expectedCapacity = port ? "2" : console ? "2.83" : "5.92";
        string expectedAssigned = port ? "Asignados: 5" : console ? "Asignados: 2" : "Asignados: 1";
        string expectedStable = port ? "Estables: 4" : console ? "Estables: 2" : "Estables: 1";
        string expectedLevel = port ? "NIVEL 1" : "NIVEL 2";
        if (!status.Contains(expectedFacility) || !status.Contains(expectedLevel) ||
            !status.Contains(expectedCapacity) || !status.Contains(expectedAssigned) ||
            !status.Contains(expectedStable))
            throw new InvalidOperationException("El estado real de la instalación N2 no coincide: " + status);
        string functions = panel.functionsText == null ? "" : panel.functionsText.text;
        if (port)
        {
            if (!functions.ToUpperInvariant().Contains("REPITE EXPEDICIONES D1") ||
                !functions.Contains("N1") || !functions.Contains("N2"))
                throw new InvalidOperationException("La función real del Puerto no coincide: " + functions);
        }
        else if (console)
        {
            if (!functions.ToUpperInvariant().Contains("COMPRA PRODUCCIÓN BASE") ||
                !functions.Contains("N2") || !functions.Contains("N3"))
                throw new InvalidOperationException("La función real de Consola no coincide: " + functions);
        }
        else if (!functions.Contains("3 rutinas") || !functions.Contains("x1.05") ||
                 !functions.Contains("PENDIENTE"))
            throw new InvalidOperationException("La función real del Núcleo no coincide: " + functions);
        string expectedUpgrade = port ? "NIVEL 2" : "NIVEL 3";
        if (panel.upgradeButton == null ||
            !panel.upgradeButton.GetComponentInChildren<TMPro.TMP_Text>(true).text.Contains(expectedUpgrade))
            throw new InvalidOperationException("La acción real de mejora no coincide.");
        if (mode == 2 && skin.referencePlate.texture != skin.improvementReference)
            throw new InvalidOperationException("La variante visual de Mejora no está activa.");
        if (port && (skin.referencePlate.texture != skin.portReference ||
            panel.openAutomationButton == null || !panel.openAutomationButton.interactable ||
            !D3FacilitySystem.IsExpeditionPortLinked(GameState.I)))
            throw new InvalidOperationException("La variante funcional/visual del Puerto no está activa.");
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

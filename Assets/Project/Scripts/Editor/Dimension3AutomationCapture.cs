#if UNITY_EDITOR
using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class Dimension3AutomationCapture
{
    private const string ActiveKey = "QF.D3AutomationCapture.Active";
    private const string FrameKey = "QF.D3AutomationCapture.Frame";
    private const string FailureKey = "QF.D3AutomationCapture.Failed";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory =>
        Path.GetFullPath("Logs/VisualQA/Dimension3/Automation");
    private static string Output1080 => Path.Combine(
        OutputDirectory, "Automation_candidate_1080x1920.png");
    private static string Output720 => Path.Combine(
        OutputDirectory, "Automation_candidate_720x1280.png");

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

    [MenuItem("Tools/Quantum Forge/Dimension 3/Capture Automation")]
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
            Time.timeScale = 1f;
            SaveService.SuppressWritesForVisualQa = false;
            SessionState.SetBool(ActiveKey, false);
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            bool failed = SessionState.GetBool(FailureKey, false);
            Debug.Log(failed
                ? "[D3 Automation Capture] FAIL"
                : "[D3 Automation Capture] PASS | Barrido simple ON 1/3 + perfil 1/1 + prioridad 3 + 2/5 ejecuciones + reserva 100 + 1080x1920 + 720x1280 | " + Output1080);
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

        Time.timeScale = 0f;
        state.dimension03Unlocked = true;
        state.dimension01Unlocked = true;
        state.LE = 3250000d;
        state.Traces = 4200d;
        Dimension3System.EnsureState(state);
        state.dimension3.firstEntrySeen = true;
        state.dimension3.presentation.onboardingStage = (int)D3OnboardingStage.Completed;
        state.dimension3.presentation.lastScreenId = PresentationFeatureIds.D3Automation;
        state.dimension3.assignments.Clear();
        state.dimension3.automationRoutines.Clear();
        state.dimension3.automationProfiles.Clear();
        state.dimension1ManualSimpleScanCompleted = true;
        state.dimension1ManualSimpleDestinationIds.Clear();
        state.dimension1ManualExtractorUpgradePlanetIds.Clear();
        ClearQueues(state.dimension3);

        BuildDirect(state.dimension3, Dimension3Catalog.FacilityExpeditionPort, 1);
        BuildDirect(state.dimension3, Dimension3Catalog.FacilityDiagnosticBank, 4);
        BuildDirect(state.dimension3, Dimension3Catalog.FacilityAutomationCore, 2);

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
            D3InventorySystem.AddAssemblyCount(
                state.dimension3, mk, mk == 1 ? 40L : 10L);

        Assign(state, Dimension3Catalog.FacilityExpeditionPort,
            Dimension3Catalog.ChannelPortCapacity, 1,
            Dimension3Catalog.TraitNormal, 4L);
        Assign(state, Dimension3Catalog.FacilityAutomationCore,
            Dimension3Catalog.ChannelCoreCoordination, 4,
            Dimension3Catalog.TraitNormal, 1L);
        Dimension3System.Tick(state, 31d);

        if (!D3AutomationSystem.TryCreateRoutine(
                state, D3AutomationCatalog.ActionPortScan, "", null,
                3, 0d, 0d, "", 100d, 5,
                out D3AutomationRoutineState routine, out string reason))
            throw new InvalidOperationException("No se pudo crear Barrido simple: " + reason);
        if (!D3AutomationSystem.TrySetRoutineEnabled(
                state, routine.routineId, true, out reason))
            throw new InvalidOperationException("No se pudo activar Barrido simple: " + reason);
        routine.executionsCompleted = 2;
        routine.lastResult = "Barrido simple iniciado.";
        routine.evaluationRemainingSeconds = 0.8d;
        if (!D3AutomationSystem.TrySaveProfile(
                state, "automation_profile_1", "Perfil de automatizacion 1",
                out reason))
            throw new InvalidOperationException("No se pudo guardar Perfil 1: " + reason);
        D3PresentationRules.EnsurePresentationState(state);

        TabsUI tabs = TabsUI.Instance != null
            ? TabsUI.Instance
            : UnityEngine.Object.FindFirstObjectByType<TabsUI>();
        if (tabs == null)
            throw new InvalidOperationException("TabsUI no existe.");
        tabs.ShowDimension3();

        Dimension3PanelUI dimension = UnityEngine.Object.FindFirstObjectByType<Dimension3PanelUI>(
            FindObjectsInactive.Include);
        D3AutomationPanelUI panel = UnityEngine.Object.FindFirstObjectByType<D3AutomationPanelUI>(
            FindObjectsInactive.Include);
        if (dimension == null || panel == null)
            throw new InvalidOperationException("Rutinas y perfiles está incompleto.");
        dimension.gameObject.SetActive(true);
        if (dimension.firstEntryRoot != null)
            dimension.firstEntryRoot.SetActive(false);
        HideSiblingViews(dimension, panel.gameObject);
        panel.Open();
        SetDropdownByText(panel.actionDropdown, "Barrido simple");
        SetDropdownByText(panel.priorityDropdown, "Prioridad 3");
        SetDropdownByText(panel.stopDropdown, "5 ejecuciones");
        SetDropdownByText(panel.reserveDropdown, "Reserva 100");
        SetDropdownByText(panel.profileDropdown, "Perfil 1");
        SetDropdownByText(panel.routineDropdown, "[ON] Barrido simple #1");
        HideUnrelated();
        Canvas.ForceUpdateCanvases();
    }

    private static void BuildDirect(Dimension3State state, string facilityId, int level)
    {
        D3FacilityState facility = D3FacilitySystem.GetFacility(state, facilityId);
        facility.built = level > 0;
        facility.level = level;
    }

    private static void Assign(GameState state, string facilityId, string channelId,
        int mk, string trait, long amount)
    {
        if (!Dimension3System.TrySetFacilityAssignment(
                state, facilityId, channelId, mk, trait, amount, out string reason))
            throw new InvalidOperationException("No se pudo preparar la asignación: " + reason);
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
        D3AutomationPanelUI panel = UnityEngine.Object.FindFirstObjectByType<D3AutomationPanelUI>(
            FindObjectsInactive.Include);
        if (state == null || panel == null || !panel.gameObject.activeInHierarchy)
            throw new InvalidOperationException("Rutinas y perfiles no está visible.");
        D3AutomationStaticSkinUI skin =
            panel.GetComponentInChildren<D3AutomationStaticSkinUI>(true);
        if (skin == null || skin.referencePlate == null ||
            skin.referencePlate.texture == null || skin.referencePlate.raycastTarget)
            throw new InvalidOperationException("La composición estática no cumple el contrato.");

        if (state.dimension3.automationRoutines.Count != 1 ||
            state.dimension3.automationProfiles.Count != 1 ||
            D3AutomationSystem.CountEnabled(state.dimension3) != 1 ||
            D3AutomationSystem.GetRoutineLimit(state.dimension3) != 3 ||
            D3FacilitySystem.GetAutomationCoreProfileLimit(state.dimension3) != 1)
            throw new InvalidOperationException("Los límites reales de rutinas/perfiles no coinciden.");
        D3AutomationRoutineState routine = state.dimension3.automationRoutines[0];
        if (routine.actionId != D3AutomationCatalog.ActionPortScan || !routine.enabled ||
            routine.priority != 3 || routine.stopAfterExecutions != 5 ||
            routine.executionsCompleted != 2 ||
            Math.Abs(routine.resourceReserveAmount - 100d) > .001 ||
            routine.lastResult != "Barrido simple iniciado.")
            throw new InvalidOperationException("El estado real de Barrido simple no coincide.");
        string status = panel.statusText == null ? "" : panel.statusText.text;
        string selected = panel.routineText == null ? "" : panel.routineText.text;
        if (!status.Contains("CREATE ROUTINE") || !status.Contains("Activas: 1 / 3") ||
            !status.Contains("Perfiles: 1 / 1") || !status.Contains("BLOQUEADO") ||
            !selected.Contains("Barrido simple") || !selected.Contains("Prioridad 3") ||
            !selected.Contains("Ejecuciones: 2 / 5") || !selected.Contains("Recurso: 100"))
            throw new InvalidOperationException(
                "El texto real de Rutinas no coincide: " + status + " | " + selected);
        if (panel.targetDropdown == null || panel.targetDropdown.gameObject.activeSelf ||
            panel.createButton == null || !panel.createButton.interactable ||
            panel.toggleButton == null || !panel.toggleButton.interactable ||
            panel.deleteButton == null || !panel.deleteButton.interactable ||
            panel.saveProfileButton == null || !panel.saveProfileButton.interactable ||
            panel.loadProfileButton == null || !panel.loadProfileButton.interactable)
            throw new InvalidOperationException("Los estados reales de los controles no coinciden.");
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
        RenderTexture target = new RenderTexture(width, height, 24,
            RenderTextureFormat.ARGB32);
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

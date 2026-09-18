#if UNITY_EDITOR
using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public static class Dimension3DiagnosticCapture
{
    private const string ActiveKey = "QF.D3DiagnosticCapture.Active";
    private const string FrameKey = "QF.D3DiagnosticCapture.Frame";
    private const string FailureKey = "QF.D3DiagnosticCapture.Failed";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory =>
        Path.GetFullPath("Logs/VisualQA/Dimension3/Diagnostic");
    private static string Output1080 => Path.Combine(
        OutputDirectory, "Diagnostic_candidate_1080x1920.png");
    private static string Output720 => Path.Combine(
        OutputDirectory, "Diagnostic_candidate_720x1280.png");

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

    [MenuItem("Tools/Quantum Forge/Dimension 3/Capture Diagnostic")]
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
                ? "[D3 Diagnostic Capture] FAIL"
                : "[D3 Diagnostic Capture] PASS | Banco N4 + autoanalisis/reparacion ON + autofusion OFF + zona Fusion + reservas 10K/100 + 1080x1920 + 720x1280 | " + Output1080);
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
        state.dimension3.presentation.lastScreenId = PresentationFeatureIds.D3FacilityDiagnostic;
        state.dimension3.assignments.Clear();
        state.dimension3.markedFusionRecipes.Clear();
        ClearQueues(state.dimension3);

        D3FacilityState diagnostic = D3FacilitySystem.GetFacility(
            state.dimension3, Dimension3Catalog.FacilityDiagnosticBank);
        diagnostic.built = true;
        diagnostic.level = 4;

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
        D3InventorySystem.AddAssemblyCount(state.dimension3, 3, 5L);
        D3InventorySystem.AddAssemblyCount(state.dimension3, 4, 1L);

        Assign(state, 1, Dimension3Catalog.TraitNormal, 40L);
        Assign(state, 2, Dimension3Catalog.TraitFast, 10L);
        Assign(state, 3, Dimension3Catalog.TraitNormal, 5L);
        Dimension3System.Tick(state, 31d);

        D3DiagnosticSettingsState settings = state.dimension3.diagnosticSettings;
        settings.autoAnalyzeEnabled = true;
        settings.autoRepairEnabled = true;
        settings.autoFusionEnabled = false;
        settings.savedRoutine = new D3DiagnosticRoutineState();
        if (!D3DiagnosticSystem.TryConfigure(state, 1, 2, 10000d, 100d,
                out string reason))
            throw new InvalidOperationException("No se pudo configurar Diagnóstico N4: " + reason);
        D3PresentationRules.EnsurePresentationState(state);

        TabsUI tabs = TabsUI.Instance != null
            ? TabsUI.Instance
            : UnityEngine.Object.FindFirstObjectByType<TabsUI>();
        if (tabs == null)
            throw new InvalidOperationException("TabsUI no existe.");
        tabs.ShowDimension3();

        Dimension3PanelUI dimension = UnityEngine.Object.FindFirstObjectByType<Dimension3PanelUI>(
            FindObjectsInactive.Include);
        D3DiagnosticPanelUI panel = UnityEngine.Object.FindFirstObjectByType<D3DiagnosticPanelUI>(
            FindObjectsInactive.Include);
        if (dimension == null || panel == null)
            throw new InvalidOperationException("Control de Diagnóstico está incompleto.");
        dimension.gameObject.SetActive(true);
        if (dimension.firstEntryRoot != null)
            dimension.firstEntryRoot.SetActive(false);
        HideSiblingViews(dimension, panel.gameObject);
        panel.Open();
        SetDropdownByText(panel.priorityModeDropdown, "Prioridad por zona");
        SetDropdownByText(panel.zoneDropdown, "Sector de Fusión");
        SetDropdownByText(panel.leReserveDropdown, "Reserva LE 10K");
        SetDropdownByText(panel.tracesReserveDropdown, "Reserva T 100");
        SetDropdownByText(panel.recipeDropdown, "Sin recetas manuales");
        HideUnrelated();
        Canvas.ForceUpdateCanvases();
    }

    private static void Assign(GameState state, int mk, string trait, long amount)
    {
        if (!Dimension3System.TrySetFacilityAssignment(
                state, Dimension3Catalog.FacilityDiagnosticBank,
                Dimension3Catalog.ChannelDiagnosticCapacity,
                mk, trait, amount, out string reason))
            throw new InvalidOperationException("No se pudo asignar capacidad diagnóstica: " + reason);
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
        D3DiagnosticPanelUI panel = UnityEngine.Object.FindFirstObjectByType<D3DiagnosticPanelUI>(
            FindObjectsInactive.Include);
        if (state == null || panel == null || !panel.gameObject.activeInHierarchy)
            throw new InvalidOperationException("Control de Diagnóstico no está visible.");
        D3DiagnosticStaticSkinUI skin =
            panel.GetComponentInChildren<D3DiagnosticStaticSkinUI>(true);
        if (skin == null || skin.referencePlate == null || skin.referencePlate.texture == null ||
            skin.referencePlate.raycastTarget)
            throw new InvalidOperationException("La composición estática no cumple el contrato.");

        D3DiagnosticSettingsState settings = state.dimension3.diagnosticSettings;
        if (!settings.autoAnalyzeEnabled || !settings.autoRepairEnabled ||
            settings.autoFusionEnabled || settings.priorityMode != 1 ||
            settings.priorityZone != 2 || Math.Abs(settings.leReserve - 10000d) > .001 ||
            Math.Abs(settings.tracesReserve - 100d) > .001)
            throw new InvalidOperationException("Los ajustes diagnósticos reales no coinciden.");
        double capacity = D3FacilitySystem.GetEffectiveCapacity(
            state.dimension3, Dimension3Catalog.FacilityDiagnosticBank);
        if (capacity + .001 < D3FacilitySystem.GetRequiredEffectiveCapacity(3))
            throw new InvalidOperationException("La capacidad real no activa la configuración N3.");
        string status = panel.statusText == null ? "" : panel.statusText.text;
        if (!status.Contains("BANCO DE DIAGNÓSTICO — NIVEL 4") ||
            !status.Contains("Sector de Fusión") || !status.Contains("10000 LE / 100 T"))
            throw new InvalidOperationException("El estado real de Diagnóstico no coincide: " + status);
        string recipes = panel.recipeText == null ? "" : panel.recipeText.text;
        if (!recipes.Contains("0 ejecutadas manualmente") ||
            state.dimension3.markedFusionRecipes.Count != 0)
            throw new InvalidOperationException("La cola real de recetas no está vacía.");
        if (panel.toggleAnalyzeButton == null || !panel.toggleAnalyzeButton.interactable ||
            panel.toggleRepairButton == null || !panel.toggleRepairButton.interactable ||
            panel.toggleFusionButton == null || !panel.toggleFusionButton.interactable ||
            panel.saveSettingsButton == null || !panel.saveSettingsButton.interactable ||
            panel.toggleRecipeMarkButton == null || panel.toggleRecipeMarkButton.interactable ||
            panel.saveRoutineButton == null || panel.saveRoutineButton.interactable ||
            panel.loadRoutineButton == null || panel.loadRoutineButton.interactable ||
            panel.zoneDropdown == null || !panel.zoneDropdown.interactable)
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

#if UNITY_EDITOR
using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public static class Dimension3ConsoleCapture
{
    private const string ActiveKey = "QF.D3ConsoleCapture.Active";
    private const string FrameKey = "QF.D3ConsoleCapture.Frame";
    private const string FailureKey = "QF.D3ConsoleCapture.Failed";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory =>
        Path.GetFullPath("Logs/VisualQA/Dimension3/Console");
    private static string Output1080 => Path.Combine(
        OutputDirectory, "Console_candidate_1080x1920.png");
    private static string Output720 => Path.Combine(
        OutputDirectory, "Console_candidate_720x1280.png");

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

    [MenuItem("Tools/Quantum Forge/Dimension 3/Capture Console")]
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
                ? "[D3 Console Capture] FAIL"
                : "[D3 Console Capture] PASS | Consola N2 + Equilibrio + reservas 10K/100 + Higgs/Tetra autorizados + 1080x1920 + 720x1280 | " + Output1080);
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
        state.dimension3.presentation.lastScreenId = PresentationFeatureIds.D3FacilityConsole;
        state.dimension3.assignments.Clear();
        ClearQueues(state.dimension3);

        D3FacilityState console = D3FacilitySystem.GetFacility(
            state.dimension3, Dimension3Catalog.FacilityProductionConsole);
        console.built = true;
        console.level = 2;

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

        if (!Dimension3System.TrySetFacilityAssignment(
                state, Dimension3Catalog.FacilityProductionConsole,
                Dimension3Catalog.ChannelConsoleCapacity, 2,
                Dimension3Catalog.TraitFast, 2L, out string reason))
            throw new InvalidOperationException("No se pudo preparar Consola N2: " + reason);
        Dimension3System.Tick(state, 31d);
        // Estado persistido válido: la política pudo guardarse con capacidad suficiente
        // y mantenerse después de retirar autómatas. Con 2.83, el sistema real impide
        // volver a guardarla hasta recuperar la capacidad N2 requerida (5.0).
        state.dimension3.consoleSettings.purchasePolicy = D3ConsoleSystem.PolicyBalanced;
        state.dimension3.consoleSettings.leReserve = 10000d;
        state.dimension3.consoleSettings.tracesReserve = 100d;
        state.dimension3.consoleSettings.manuallyPurchasedBuildingIds.Clear();
        state.dimension3.consoleSettings.manuallySelectedTriangleCircuits.Clear();
        D3ConsoleSystem.RecordManualBuildingPurchase(
            state, D3ConsoleSystem.BuildingHiggs);
        D3ConsoleSystem.RecordManualBuildingPurchase(
            state, D3ConsoleSystem.BuildingTetraquark);
        D3PresentationRules.EnsurePresentationState(state);

        TabsUI tabs = TabsUI.Instance != null
            ? TabsUI.Instance
            : UnityEngine.Object.FindFirstObjectByType<TabsUI>();
        if (tabs == null)
            throw new InvalidOperationException("TabsUI no existe.");
        tabs.ShowDimension3();

        Dimension3PanelUI dimension = UnityEngine.Object.FindFirstObjectByType<Dimension3PanelUI>(
            FindObjectsInactive.Include);
        D3ConsolePanelUI panel = UnityEngine.Object.FindFirstObjectByType<D3ConsolePanelUI>(
            FindObjectsInactive.Include);
        if (dimension == null || panel == null)
            throw new InvalidOperationException("La pantalla Control de Consola está incompleta.");
        dimension.gameObject.SetActive(true);
        if (dimension.firstEntryRoot != null)
            dimension.firstEntryRoot.SetActive(false);
        HideSiblingViews(dimension, panel.gameObject);
        panel.Open();
        SetDropdownByText(panel.policyDropdown, "Equilibrio");
        SetDropdownByText(panel.leReserveDropdown, "Reserva LE 10K");
        SetDropdownByText(panel.tracesReserveDropdown, "Reserva T 100");
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
        D3ConsolePanelUI panel = UnityEngine.Object.FindFirstObjectByType<D3ConsolePanelUI>(
            FindObjectsInactive.Include);
        if (state == null || panel == null || !panel.gameObject.activeInHierarchy)
            throw new InvalidOperationException("Control de Consola no está visible.");
        D3ConsoleStaticSkinUI skin = panel.GetComponentInChildren<D3ConsoleStaticSkinUI>(true);
        if (skin == null || skin.referencePlate == null || skin.referencePlate.texture == null ||
            skin.referencePlate.raycastTarget)
            throw new InvalidOperationException("La composición estática no cumple el contrato.");

        D3ConsoleSettingsState settings = state.dimension3.consoleSettings;
        if (settings.purchasePolicy != D3ConsoleSystem.PolicyBalanced ||
            Math.Abs(settings.leReserve - 10000d) > .001 ||
            Math.Abs(settings.tracesReserve - 100d) > .001)
            throw new InvalidOperationException("La política o las reservas reales no coinciden.");
        if (!D3ConsoleSystem.HasManualBuildingAuthorization(
                state.dimension3, D3ConsoleSystem.BuildingHiggs) ||
            !D3ConsoleSystem.HasManualBuildingAuthorization(
                state.dimension3, D3ConsoleSystem.BuildingTetraquark))
            throw new InvalidOperationException("Faltan autorizaciones manuales reales.");

        string status = panel.statusText == null ? "" : panel.statusText.text;
        if (!status.Contains("CONSOLA — NIVEL 2") || !status.Contains("Política: Equilibrio") ||
            !status.Contains("10000 LE") || !status.Contains("100 T"))
            throw new InvalidOperationException("El estado real de Consola no coincide: " + status);
        string history = panel.historyText == null ? "" : panel.historyText.text;
        if (!history.Contains("Higgs: Sí") || !history.Contains("Tetra: Sí") ||
            !history.Contains("Circuitos registrados: 0"))
            throw new InvalidOperationException("El historial real no coincide: " + history);
        if (panel.savePolicyButton == null || panel.savePolicyButton.interactable ||
            panel.recordTriangleButton == null || panel.recordTriangleButton.interactable ||
            panel.openRoutinesButton == null || !panel.openRoutinesButton.interactable)
            throw new InvalidOperationException("Los estados reales de los botones no coinciden.");
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

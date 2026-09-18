#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Dimension2BondCapture
{
    private const string ActiveKey = "QF.D2BondCapture.Active";
    private const string FrameKey = "QF.D2BondCapture.Frame";
    private const string FailureKey = "QF.D2BondCapture.Failed";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory => Path.GetFullPath(
        "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/DIMENSION_2/PACTO_LUGAR_DE_VINCULO/CAPTURAS_CANDIDATAS");
    private static string Output1080 => Path.Combine(OutputDirectory,
        "D2_Pacto_Lugar_De_Vinculo_1080x1920.png");
    private static string Output720 => Path.Combine(OutputDirectory,
        "D2_Pacto_Lugar_De_Vinculo_720x1280.png");

    [InitializeOnLoadMethod]
    private static void Resume()
    {
        if (!SessionState.GetBool(ActiveKey, false)) return;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        if (EditorApplication.isPlaying)
        {
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
        }
    }

    public static void Run()
    {
        Directory.CreateDirectory(OutputDirectory);
        if (File.Exists(Output1080)) File.Delete(Output1080);
        if (File.Exists(Output720)) File.Delete(Output720);
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetBool(FailureKey, false);
        SessionState.SetInt(FrameKey, 0);
        SaveService.SuppressWritesForVisualQa = true;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        EditorApplication.isPlaying = true;
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
            Debug.Log((failed ? "[D2 Bond Capture] FAIL | " :
                "[D2 Bond Capture] PASS | botones reales | 1080x1920 + 720x1280 | ") + Output1080);
            EditorApplication.Exit(failed ? 1 : 0);
        }
    }

    private static void Tick()
    {
        try
        {
            int frame = SessionState.GetInt(FrameKey, 0) + 1;
            SessionState.SetInt(FrameKey, frame);
            if (frame == 12) PrepareAndOpen();
            if (frame == 22) ValidateActionsAndRestore();
            if (frame != 25) return;
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

    private static void PrepareAndOpen()
    {
        GameState gameState = GameState.I != null
            ? GameState.I : UnityEngine.Object.FindFirstObjectByType<GameState>();
        if (gameState == null) throw new InvalidOperationException("GameState no existe.");
        gameState.dimension02Unlocked = true;
        Dimension2System.EnsureState(gameState);
        gameState.dimension2.firstEntrySeen = true;
        gameState.dimension2.firstEntryVisualVersionSeen = Dimension2System.FirstEntryVisualVersion;
        D2Civilization1State state = new D2Civilization1State
        {
            initialFollowersGranted = true,
            followersAvailable = 3845L,
            totalFollowersReceived = 3845L,
            trust = D2VeiledThresholdSystem.UnlockTrustRequired,
            totalPilgrimagesCompleted = 10L,
            shortPilgrimagesCompleted = 5L,
            mediumPilgrimagesCompleted = 2L,
            novitiateLevel = 4,
            totalAcolytesCreated = 1284L,
            acolytesAvailable = 1280L,
            acolytesAssignedToBond = 4L,
            entityContactAvailable = true,
            bondPlacePrepared = true,
            bondProgress = 45.0
        };
        gameState.dimension2.civilization1 = state;
        gameState.dimension2.civilization1Unlocked = true;
        D2Civilization1System.EnsureState(state);
        D2BondSystem.EnsureState(state);
        foreach (D2BondLineState line in state.bondLines) line.level = 1;
        SetOffering(state, D2AltarSystem.IncenseAltarId, 300.0);
        SetOffering(state, D2AltarSystem.SacredClothAltarId, 300.0);
        SetOffering(state, D2AltarSystem.CarvedStoneAltarId, 300.0);

        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        if (panel == null) throw new InvalidOperationException("Dimension2PanelUI no existe.");
        ActivateAncestors(panel.transform);
        panel.OpenCivilization1();
        panel.civilization1PanelUI.ShowPactsSection();
        panel.Refresh();
        Click(panel.civilization1PanelUI.pactsPanelUI.thresholdNavButton);
        panel.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateActionsAndRestore()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2VeiledThresholdPanelUI ui = panel?.civilization1PanelUI?.veiledThresholdPanelUI;
        D2Civilization1State state = GameState.I?.dimension2?.civilization1;
        if (ui == null || state == null || ui.lineButtons == null || ui.lineButtons.Length != 5)
            throw new InvalidOperationException("Falta la matriz funcional del vínculo.");
        Click(ui.lineButtons[1]);
        if (ui.SelectedLineId != D2BondSystem.SacredCraftId)
            throw new InvalidOperationException("Oficio Sagrado no seleccionó su ID.");
        Click(ui.releaseAcolyteButton);
        Click(ui.assignAcolyteButton);
        if (state.acolytesAssignedToBond != 4L || state.acolytesAvailable != 1280L)
            throw new InvalidOperationException("Los controles de Acólitos no conservaron el total.");
        double before = state.bondProgress;
        Click(ui.upgradeLineButton);
        if (D2BondSystem.GetLevel(state, D2BondSystem.SacredCraftId) != 2 ||
            Math.Abs((before - state.bondProgress) - 40.0) > 0.02)
            throw new InvalidOperationException("MEJORAR LÍNEA no aplicó nivel/coste real.");
        foreach (D2BondLineState line in state.bondLines) line.level = 1;
        state.bondProgress = 45.0;
        SetOffering(state, D2AltarSystem.IncenseAltarId, 300.0);
        SetOffering(state, D2AltarSystem.SacredClothAltarId, 300.0);
        SetOffering(state, D2AltarSystem.CarvedStoneAltarId, 300.0);
        ui.SelectLine(D2BondSystem.PilgrimPathId);
        Click(ui.pactsNavButton);
        Click(panel.civilization1PanelUI.pactsPanelUI.thresholdNavButton);
        ui.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateVisible()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2Civilization1PanelUI sanctuary = panel?.civilization1PanelUI;
        D2VeiledThresholdPanelUI ui = sanctuary?.veiledThresholdPanelUI;
        if (ui == null || !sanctuary.veiledThresholdSectionRoot.activeInHierarchy ||
            sanctuary.refugeSectionRoot.activeInHierarchy || sanctuary.altarsSectionRoot.activeInHierarchy ||
            sanctuary.pilgrimagesSectionRoot.activeInHierarchy || sanctuary.novitiateSectionRoot.activeInHierarchy ||
            sanctuary.ritesSectionRoot.activeInHierarchy || sanctuary.pactsSectionRoot.activeInHierarchy)
            throw new InvalidOperationException("Lugar de Vínculo no es la única sección visible.");
        if (ui.titleText?.text != "PACTO — LUGAR DE VÍNCULO" ||
            ui.incenseValueText?.text != "300" || ui.sacredClothValueText?.text != "300" ||
            ui.carvedStoneValueText?.text != "300" || ui.progressValueText?.text != "45" ||
            ui.SelectedLineId != D2BondSystem.PilgrimPathId ||
            ui.detailTitleText?.text != "CAMINO PEREGRINO" ||
            ui.detailLevelText?.text != "NIVEL 1 / 3" || ui.progressCostText?.text != "40 PROGRESO" ||
            ui.availableAcolytesValueText?.text != "1,280" ||
            ui.assignedAcolytesValueText?.text != "4" ||
            ui.revelationText?.text != "PACTO ESTABLECIDO" ||
            ui.placeText?.text != "PREPARACIÓN COMPLETADA" ||
            ui.thresholdTabLabelText?.text != "PACTO" || !ui.upgradeLineButton.interactable)
            throw new InvalidOperationException("Contenido dinámico del vínculo no coincide con V4.");
        Debug.Log("[D2 Bond Capture] STRUCTURE_PASS | STATE_PASS | ACTIONS_PASS | ROUTE_PASS | 1080x1920 + 720x1280");
    }

    private static void SetOffering(D2Civilization1State state, string id, double amount)
    {
        D2AltarState altar = D2AltarSystem.GetAltar(state, id);
        if (altar == null) throw new InvalidOperationException("Falta la Ofrenda " + id + ".");
        altar.unlocked = true;
        altar.offeringAmount = amount;
        altar.totalOfferingProduced = amount;
    }

    private static void Click(Button button)
    {
        if (button == null || EventSystem.current == null || !button.gameObject.activeInHierarchy)
            throw new InvalidOperationException("No se puede pulsar un botón real del vínculo.");
        PointerEventData data = new PointerEventData(EventSystem.current)
        {
            button = PointerEventData.InputButton.Left
        };
        ExecuteEvents.Execute(button.gameObject, data, ExecuteEvents.pointerClickHandler);
        Canvas.ForceUpdateCanvases();
    }

    private static void ActivateAncestors(Transform transform)
    {
        for (Transform current = transform; current != null; current = current.parent)
            current.gameObject.SetActive(true);
    }

    private static void RenderToPng(int width, int height, string path)
    {
        Camera camera = Camera.main != null ? Camera.main : UnityEngine.Object.FindFirstObjectByType<Camera>();
        if (camera == null) throw new InvalidOperationException("No hay cámara para captura.");
        Canvas[] canvases = UnityEngine.Object.FindObjectsByType<Canvas>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        RenderMode[] modes = new RenderMode[canvases.Length];
        Camera[] cameras = new Camera[canvases.Length];
        for (int i = 0; i < canvases.Length; i++)
        {
            modes[i] = canvases[i].renderMode;
            cameras[i] = canvases[i].worldCamera;
            if (modes[i] != RenderMode.ScreenSpaceOverlay) continue;
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
            Canvas.ForceUpdateCanvases();
            camera.Render();
            camera.Render();
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
}
#endif

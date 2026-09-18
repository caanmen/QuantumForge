#if UNITY_EDITOR
using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Dimension2ResistanceRegionsCapture
{
    private const string ActiveKey = "QF.D2ResistanceRegionsCapture.Active";
    private const string FrameKey = "QF.D2ResistanceRegionsCapture.Frame";
    private const string FailureKey = "QF.D2ResistanceRegionsCapture.Failed";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory => Path.GetFullPath(
        "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/" +
        "DIMENSION_2/RED_DE_RESISTENCIA/CAPTURAS_CANDIDATAS");
    private static string Output1080 => Path.Combine(OutputDirectory,
        "D2_Red_De_Resistencia_1080x1920.png");
    private static string Output720 => Path.Combine(OutputDirectory,
        "D2_Red_De_Resistencia_720x1280.png");

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
            Debug.Log((failed ? "[D2 Resistance Regions Capture] FAIL | " :
                "[D2 Resistance Regions Capture] PASS | 3 regiones reales + selección + " +
                "−1/+1/+10/todos + regreso + ayuda | 1080x1920 + 720x1280 | ") +
                Output1080);
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
            if (frame != 26) return;
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
        Time.timeScale = 0f;
        gameState.dimension02Unlocked = true;
        Dimension2System.EnsureState(gameState);
        gameState.dimension2.firstEntrySeen = true;
        gameState.dimension2.firstEntryVisualVersionSeen = Dimension2System.FirstEntryVisualVersion;
        gameState.dimension2.civilization1Unlocked = true;
        gameState.dimension2.civilization2Unlocked = true;
        gameState.dimension2.civilization3Unlocked = false;
        gameState.dimension2.civilization2 = new D2Civilization2State
        {
            initialMembersGranted = true,
            membersAvailable = 3845L,
            totalMembersRecruited = 5170L,
            selectedRegionId = D2Civilization2System.Region1Id,
            totalReprisals = 1L,
            controlFragments = 3L,
            alertActive = true,
            containmentAvailable = true
        };
        D2Civilization2State state = gameState.dimension2.civilization2;
        D2Civilization2System.EnsureState(state);
        ConfigureRegion(state, D2Civilization2System.Region1Id, 52.0, 68.0, 520L, 400L);
        ConfigureRegion(state, D2Civilization2System.Region2Id, 72.0, 40.0, 410L, 0L);
        ConfigureRegion(state, D2Civilization2System.Region3Id, 92.0, 15.0, 395L, 0L);
        D2PresentationRules.EnsurePresentationState(gameState);
        PresentationStateUtility.Acknowledge(gameState.dimension2.presentation,
            PresentationFeatureIds.D2C2Alert);

        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        if (panel == null) throw new InvalidOperationException("Dimension2PanelUI no existe.");
        ActivateAncestors(panel.transform);
        panel.OpenCivilization2();
        panel.civilization2PanelUI.ShowRegions();
        panel.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ConfigureRegion(D2Civilization2State state, string id,
        double dominance, double threat, long members, long rescueMembers)
    {
        D2RegionState region = D2Civilization2System.GetRegion(state, id);
        if (region == null) throw new InvalidOperationException("Falta " + id + ".");
        region.unlocked = true;
        region.dominance = dominance;
        region.threat = threat;
        region.membersAssigned = members;
        D2OperationState rescue = D2Civilization2System.GetOperation(
            region, D2Civilization2System.RescueOperationId);
        if (rescue == null) throw new InvalidOperationException("Falta Rescate en " + id + ".");
        rescue.membersAssigned = rescueMembers;
    }

    private static void ValidateActionsAndRestore()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2Civilization2PanelUI ui = panel?.civilization2PanelUI;
        D2Civilization2State state = GameState.I?.dimension2?.civilization2;
        if (ui == null || state == null || ui.regionButtons == null || ui.regionButtons.Length != 3)
            throw new InvalidOperationException("Falta la red regional funcional.");

        Click(ui.regionButtons[1]);
        if (ui.GetSelectedRegionId() != D2Civilization2System.Region2Id ||
            ui.detailTitleText?.text != "REGIÓN 2 — DISPONIBLE")
            throw new InvalidOperationException("Región 2 no actualizó el detalle.");
        Click(ui.regionButtons[0]);
        D2RegionState region1 = D2Civilization2System.GetRegion(
            state, D2Civilization2System.Region1Id);
        Click(ui.releaseOneButton);
        if (region1.membersAssigned != 519L || state.membersAvailable != 3846L)
            throw new InvalidOperationException("−1 no liberó un Miembro regional.");
        Click(ui.assignOneButton);
        if (region1.membersAssigned != 520L || state.membersAvailable != 3845L)
            throw new InvalidOperationException("+1 no restauró el Miembro regional.");
        Click(ui.assignTenButton);
        if (region1.membersAssigned != 530L || state.membersAvailable != 3835L)
            throw new InvalidOperationException("+10 no asignó diez Miembros.");
        D2Civilization2System.TryReleaseMembers(GameState.I,
            D2Civilization2System.Region1Id, 10L);
        Click(ui.assignAllButton);
        if (state.membersAvailable != 0L)
            throw new InvalidOperationException("ASIGNAR TODOS dejó saldo disponible.");
        RestoreReferenceState(state);
        ui.Refresh();
        Click(ui.releaseAllButton);
        if (D2Civilization2System.GetRegionIdleMembers(region1) != 0L)
            throw new InvalidOperationException("LIBERAR TODOS dejó saldo sin destinar.");
        RestoreReferenceState(state);
        ui.Refresh();

        Click(ui.helpButton);
        if (panel.helpRoot == null || !panel.helpRoot.activeInHierarchy)
            throw new InvalidOperationException("La ayuda contextual no abrió.");
        panel.CloseContextualHelp();
        Click(ui.backToMapButton);
        if (panel.mapRoot == null || !panel.mapRoot.activeInHierarchy)
            throw new InvalidOperationException("El regreso no abrió el mapa.");
        panel.OpenCivilization2();
        ui.ShowRegions();
        ui.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void RestoreReferenceState(D2Civilization2State state)
    {
        state.membersAvailable = 3845L;
        state.selectedRegionId = D2Civilization2System.Region1Id;
        ConfigureRegion(state, D2Civilization2System.Region1Id, 52.0, 68.0, 520L, 400L);
        ConfigureRegion(state, D2Civilization2System.Region2Id, 72.0, 40.0, 410L, 0L);
        ConfigureRegion(state, D2Civilization2System.Region3Id, 92.0, 15.0, 395L, 0L);
    }

    private static void ValidateVisible()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2Civilization2PanelUI ui = panel?.civilization2PanelUI;
        if (ui == null || !ui.regionSectionRoot.activeInHierarchy ||
            ui.operationsSectionRoot.activeInHierarchy || ui.defenseSectionRoot.activeInHierarchy ||
            ui.resistanceSectionRoot.activeInHierarchy || ui.alertSectionRoot.activeInHierarchy ||
            ui.containmentSectionRoot.activeInHierarchy)
            throw new InvalidOperationException("Regiones no es la única sección visible.");
        if (ui.availableMembersValueText?.text != "3,845" ||
            ui.assignedMembersValueText?.text != "1,325" ||
            ui.totalMembersValueText?.text != "5,170" ||
            ui.totalDominanceValueText?.text != "72%" ||
            ui.regionDominanceValueTexts[0].text != "52%" ||
            ui.regionThreatValueTexts[0].text != "68%" ||
            ui.regionMembersValueTexts[0].text != "520" ||
            ui.detailIdleValueText?.text != "120")
            throw new InvalidOperationException("El estado V4 de la Red no coincide.");
        RectTransform root = panel.civilization2Root.GetComponent<RectTransform>();
        if (Mathf.Abs(root.rect.width - 1080f) > 1f || Mathf.Abs(root.rect.height - 1920f) > 1f)
            throw new InvalidOperationException("La Red no conserva el lienzo 1080x1920.");
        ValidateTextGeometry(root);
        Debug.Log("[D2 Resistance Regions Capture] STRUCTURE_PASS | STATE_PASS | " +
            "ACTIONS_PASS | ROUTE_PASS | TEXT_GEOMETRY_PASS | 1080x1920 + 720x1280");
    }

    private static void ValidateTextGeometry(RectTransform root)
    {
        TMP_Text[] texts = root.GetComponentsInChildren<TMP_Text>(false);
        for (int i = 0; i < texts.Length; i++)
        {
            TMP_Text text = texts[i];
            if (text == null || string.IsNullOrWhiteSpace(text.text) || IsVisuallyHidden(text.transform))
                continue;
            text.ForceMeshUpdate();
            Vector2 rendered = text.GetRenderedValues(false);
            Rect rect = text.rectTransform.rect;
            if (rendered.x > rect.width + 2f || rendered.y > rect.height + 2f)
                throw new InvalidOperationException(
                    "Texto fuera de su caja: " + text.name + " (" + rendered + " / " + rect.size + ").");
            Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(
                root, text.rectTransform);
            if (bounds.min.x < root.rect.xMin - 2f || bounds.max.x > root.rect.xMax + 2f ||
                bounds.min.y < root.rect.yMin - 2f || bounds.max.y > root.rect.yMax + 2f)
                throw new InvalidOperationException("Texto fuera del lienzo: " + text.name + ".");
        }
    }

    private static bool IsVisuallyHidden(Transform transform)
    {
        for (Transform current = transform; current != null; current = current.parent)
        {
            CanvasGroup group = current.GetComponent<CanvasGroup>();
            if (group != null && group.alpha <= 0.01f) return true;
        }
        return false;
    }

    private static void Click(Button button)
    {
        if (button == null || EventSystem.current == null || !button.gameObject.activeInHierarchy)
            throw new InvalidOperationException("No se puede pulsar un botón real de la Red.");
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
        Camera camera = Camera.main != null
            ? Camera.main : UnityEngine.Object.FindFirstObjectByType<Camera>();
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

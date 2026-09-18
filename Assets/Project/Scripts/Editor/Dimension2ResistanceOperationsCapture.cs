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

public static class Dimension2ResistanceOperationsCapture
{
    private const string ActiveKey = "QF.D2ResistanceOperationsCapture.Active";
    private const string FrameKey = "QF.D2ResistanceOperationsCapture.Frame";
    private const string FailureKey = "QF.D2ResistanceOperationsCapture.Failed";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory => Path.GetFullPath(
        "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/" +
        "DIMENSION_2/OPERACIONES_DE_RESISTENCIA/CAPTURAS_CANDIDATAS");
    private static string Output1080 => Path.Combine(OutputDirectory,
        "D2_Operaciones_De_Resistencia_1080x1920.png");
    private static string Output720 => Path.Combine(OutputDirectory,
        "D2_Operaciones_De_Resistencia_720x1280.png");

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
            Debug.Log((failed ? "[D2 Resistance Operations Capture] FAIL | " :
                "[D2 Resistance Operations Capture] PASS | 4 tarjetas + selección + " +
                "−1/+1/+5/todos + rutas | 1080x1920 + 720x1280 | ") + Output1080);
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
        gameState.dimension2.firstEntryVisualVersionSeen =
            Dimension2System.FirstEntryVisualVersion;
        gameState.dimension2.civilization1Unlocked = true;
        gameState.dimension2.civilization2Unlocked = true;
        gameState.dimension2.civilization2 = new D2Civilization2State
        {
            initialMembersGranted = true,
            membersAvailable = 3845L,
            totalMembersRecruited = 3975L,
            selectedRegionId = D2Civilization2System.Region1Id,
            totalReprisals = 1L
        };
        D2Civilization2State state = gameState.dimension2.civilization2;
        D2Civilization2System.EnsureState(state);
        RestoreReferenceState(state);
        D2PresentationRules.EnsurePresentationState(gameState);

        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        if (panel == null) throw new InvalidOperationException("Dimension2PanelUI no existe.");
        ActivateAncestors(panel.transform);
        panel.OpenCivilization2();
        panel.civilization2PanelUI.ShowOperations();
        panel.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void RestoreReferenceState(D2Civilization2State state)
    {
        state.membersAvailable = 3845L;
        state.selectedRegionId = D2Civilization2System.Region1Id;
        state.totalReprisals = 1L;
        D2RegionState region = D2Civilization2System.GetRegion(
            state, D2Civilization2System.Region1Id);
        if (region == null) throw new InvalidOperationException("Falta Región 1.");
        region.unlocked = true;
        region.membersAssigned = 130L;
        region.dominance = 52.0;
        region.threat = 68.0;
        SetOperationMembers(region, D2Civilization2System.RescueOperationId, 5L);
        SetOperationMembers(region, D2Civilization2System.ProtectionOperationId, 5L);
        SetOperationMembers(region, D2Civilization2System.EspionageOperationId, 0L);
        SetOperationMembers(region, D2Civilization2System.SabotageOperationId, 0L);
    }

    private static void SetOperationMembers(D2RegionState region, string operationId, long value)
    {
        D2OperationState operation = D2Civilization2System.GetOperation(region, operationId);
        if (operation == null) throw new InvalidOperationException("Falta " + operationId + ".");
        operation.membersAssigned = value;
    }

    private static void ValidateActionsAndRestore()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2Civilization2PanelUI civilization = panel?.civilization2PanelUI;
        D2OperationsPanelUI ui = civilization?.operationsPanelUI;
        D2Civilization2State state = GameState.I?.dimension2?.civilization2;
        D2RegionState region = D2Civilization2System.GetRegion(
            state, D2Civilization2System.Region1Id);
        if (ui == null || region == null || ui.operationButtons == null ||
            ui.operationButtons.Length != 4)
            throw new InvalidOperationException("Falta la interfaz real de Operaciones.");

        Click(ui.operationButtons[1]);
        if (ui.GetSelectedOperationId() != D2Civilization2System.ProtectionOperationId ||
            ui.detailNameText?.text != "PROTECCIÓN")
            throw new InvalidOperationException("Protección no actualizó el detalle.");
        Click(ui.operationButtons[0]);

        D2OperationState rescue = D2Civilization2System.GetOperation(
            region, D2Civilization2System.RescueOperationId);
        Click(ui.releaseOneButton);
        if (rescue.membersAssigned != 4L ||
            D2Civilization2System.GetRegionIdleMembers(region) != 121L)
            throw new InvalidOperationException("−1 no liberó un Miembro de Rescate.");
        Click(ui.assignOneButton);
        if (rescue.membersAssigned != 5L ||
            D2Civilization2System.GetRegionIdleMembers(region) != 120L)
            throw new InvalidOperationException("+1 no restauró Rescate.");
        Click(ui.assignFiveButton);
        if (rescue.membersAssigned != 10L ||
            D2Civilization2System.GetRegionIdleMembers(region) != 115L)
            throw new InvalidOperationException("+5 no asignó cinco Miembros.");
        RestoreReferenceState(state);
        ui.Refresh();
        Click(ui.assignAllButton);
        if (D2Civilization2System.GetRegionIdleMembers(region) != 0L ||
            rescue.membersAssigned != 125L)
            throw new InvalidOperationException("ASIGNAR TODOS dejó saldo regional.");
        RestoreReferenceState(state);
        ui.Refresh();
        Click(ui.releaseAllButton);
        if (rescue.membersAssigned != 0L ||
            D2Civilization2System.GetRegionIdleMembers(region) != 125L)
            throw new InvalidOperationException("LIBERAR TODOS dejó Miembros en Rescate.");
        RestoreReferenceState(state);
        ui.Refresh();

        Click(civilization.showRegionsButton);
        if (!civilization.regionSectionRoot.activeInHierarchy)
            throw new InvalidOperationException("REGIONES no abrió su sección.");
        Click(civilization.showOperationsButton);
        if (!civilization.operationsSectionRoot.activeInHierarchy)
            throw new InvalidOperationException("OPERACIONES no reabrió su sección.");
        Click(civilization.backToMapButton);
        if (panel.mapRoot == null || !panel.mapRoot.activeInHierarchy)
            throw new InvalidOperationException("El regreso no abrió el mapa.");
        panel.OpenCivilization2();
        civilization.ShowOperations();
        ui.SelectOperationCard(0);
        ui.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateVisible()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2Civilization2PanelUI civilization = panel?.civilization2PanelUI;
        D2OperationsPanelUI ui = civilization?.operationsPanelUI;
        if (ui == null || !civilization.operationsSectionRoot.activeInHierarchy ||
            civilization.regionSectionRoot.activeInHierarchy ||
            civilization.defenseSectionRoot.activeInHierarchy ||
            civilization.resistanceSectionRoot.activeInHierarchy ||
            civilization.alertSectionRoot.activeInHierarchy ||
            civilization.containmentSectionRoot.activeInHierarchy)
            throw new InvalidOperationException("Operaciones no es la única sección visible.");
        if (ui.regionNameText?.text != "REGIÓN 1" ||
            ui.regionalIdleValueText?.text != "120" ||
            ui.regionalOperationsValueText?.text != "10" ||
            ui.operationStateTexts[0].text != "ACTIVA" ||
            ui.operationStateTexts[1].text != "ACTIVA" ||
            ui.operationStateTexts[2].text != "INACTIVA" ||
            ui.operationAssignedValueTexts[0].text != "5" ||
            ui.detailNameText?.text != "RESCATE" ||
            ui.detailStateText?.text != "ACTIVA" ||
            ui.memberRateText?.text != "+0.447 / MIN" ||
            ui.dominanceRateText?.text != "−0.05 / MIN" ||
            ui.threatRateText?.text != "+0.20 / MIN" ||
            ui.assignmentValueText?.text != "5")
            throw new InvalidOperationException("El estado V4 de Operaciones no coincide.");
        if (ui.operationSelectionOverlays == null ||
            !ui.operationSelectionOverlays[0].activeInHierarchy ||
            ui.operationSelectionOverlays[1].activeInHierarchy ||
            ui.operationSelectionOverlays[2].activeInHierarchy ||
            ui.operationSelectionOverlays[3].activeInHierarchy)
            throw new InvalidOperationException("La selección de Rescate no es única.");

        RectTransform root = panel.civilization2Root.GetComponent<RectTransform>();
        if (Mathf.Abs(root.rect.width - 1080f) > 1f ||
            Mathf.Abs(root.rect.height - 1920f) > 1f)
            throw new InvalidOperationException("Operaciones no conserva el lienzo 1080x1920.");
        ValidateTextGeometry(root);
        Debug.Log("[D2 Resistance Operations Capture] STRUCTURE_PASS | STATE_PASS | " +
            "ACTIONS_PASS | ROUTE_PASS | TEXT_GEOMETRY_PASS | 1080x1920 + 720x1280");
    }

    private static void ValidateTextGeometry(RectTransform root)
    {
        TMP_Text[] texts = root.GetComponentsInChildren<TMP_Text>(false);
        for (int i = 0; i < texts.Length; i++)
        {
            TMP_Text text = texts[i];
            if (text == null || string.IsNullOrWhiteSpace(text.text) ||
                IsVisuallyHidden(text.transform)) continue;
            text.ForceMeshUpdate();
            Vector2 rendered = text.GetRenderedValues(false);
            Rect rect = text.rectTransform.rect;
            if (rendered.x > rect.width + 2f || rendered.y > rect.height + 2f)
                throw new InvalidOperationException(
                    "Texto fuera de su caja: " + text.name + " (" + rendered + " / " +
                    rect.size + ").");
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
            throw new InvalidOperationException("No se puede pulsar un botón real de Operaciones.");
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

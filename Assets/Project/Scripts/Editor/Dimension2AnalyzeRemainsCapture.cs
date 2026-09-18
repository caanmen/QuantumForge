#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Dimension2AnalyzeRemainsCapture
{
    private const string ActiveKey = "QF.D2AnalyzeRemainsCapture.Active";
    private const string FrameKey = "QF.D2AnalyzeRemainsCapture.Frame";
    private const string FailureKey = "QF.D2AnalyzeRemainsCapture.Failed";
    private const string SaveFingerprintKey = "QF.D2AnalyzeRemainsCapture.Save";
    private const string BackupFingerprintKey = "QF.D2AnalyzeRemainsCapture.Backup";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory => Path.GetFullPath(
        "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/" +
        "DIMENSION_2/ANALIZAR_RESTOS/CAPTURAS_CANDIDATAS");
    private static string Output1080 => Path.Combine(OutputDirectory,
        "D2_AnalizarRestos_1080x1920.png");
    private static string Output720 => Path.Combine(OutputDirectory,
        "D2_AnalizarRestos_720x1280.png");

    [InitializeOnLoadMethod]
    private static void Resume()
    {
        if (!SessionState.GetBool(ActiveKey, false)) return;
        SaveService.SuppressWritesForVisualQa = true;
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
        StoreSaveFingerprints();
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
            SaveService.SuppressWritesForVisualQa = true;
            bool failed = SessionState.GetBool(FailureKey, false);
            try
            {
                ValidateCaptureFile(Output1080, 1080, 1920);
                ValidateCaptureFile(Output720, 720, 1280);
                ValidateSaveFingerprints();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                failed = true;
            }
            SessionState.SetBool(ActiveKey, false);
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            Debug.Log((failed ? "[D2 Analyze Remains Capture] FAIL | " :
                "[D2 Analyze Remains Capture] PASS | STRUCTURE_PASS | " +
                "STATE_PASS | ACTIONS_PASS | ROUTE_PASS | SAVE_INTACT_PASS | " +
                "TEXT_GEOMETRY_PASS | 1080x1920 + 720x1280 | ") + Output1080);
            EditorApplication.Exit(failed ? 1 : 0);
        }
    }

    private static void Tick()
    {
        try
        {
            SaveService.SuppressWritesForVisualQa = true;
            int frame = SessionState.GetInt(FrameKey, 0) + 1;
            SessionState.SetInt(FrameKey, frame);
            if (frame == 12) PrepareFixture();
            if (frame == 22) ValidatePhysicalRoutesAndRestore();
            if (frame == 28)
            {
                ValidateVisibleState();
                RenderToPng(1080, 1920, Output1080);
            }
            else if (frame == 31)
            {
                RestoreFixtureVisualState();
                RenderToPng(720, 1280, Output720);
            }
            else if (frame == 34)
            {
                EditorApplication.update -= Tick;
                EditorApplication.isPlaying = false;
            }
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            SessionState.SetBool(FailureKey, true);
            EditorApplication.update -= Tick;
            EditorApplication.isPlaying = false;
        }
    }

    private static void PrepareFixture()
    {
        Screen.SetResolution(1080, 1920, false);
        GameState gameState = GameState.I != null
            ? GameState.I : UnityEngine.Object.FindFirstObjectByType<GameState>();
        Require(gameState != null, "GameState no existe.");
        Time.timeScale = 0f;
        gameState.dimension02Unlocked = true;
        Dimension2System.EnsureState(gameState);
        gameState.dimension2.firstEntrySeen = true;
        gameState.dimension2.firstEntryVisualVersionSeen =
            Dimension2System.FirstEntryVisualVersion;
        gameState.dimension2.civilization1Unlocked = true;
        gameState.dimension2.civilization2Unlocked = true;
        gameState.dimension2.civilization3Unlocked = true;
        gameState.dimension2.civilization3 = new D2Civilization3State
        {
            selectedZoneId = D2Civilization3System.Zone1Id,
            ancientKnowledge = 18.0,
            archiveUnlocked = true,
            archiveLevel = 4,
            entityResearchUnlocked = true,
            entityKnowledge = 3L,
            anomalyClueDetectionUnlocked = true,
            lastResult = "Análisis disponible en Entrada Sepultada."
        };
        D2Civilization3State state = gameState.dimension2.civilization3;
        D2Civilization3System.EnsureState(state);
        ConfigureZone(state, D2Civilization3System.Zone1Id,
            45.0, 26L, 12L, 5L, 2L, 1L, 0.08);
        ConfigureZone(state, D2Civilization3System.Zone2Id,
            60.0, 84L, 10L, 4L, 1L, 1L, 0.24);
        ConfigureZone(state, D2Civilization3System.Zone3Id,
            46.0, 36L, 12L, 5L, 2L, 2L, 0.38);
        SetAltarAmount(gameState, D2AltarSystem.WaxAltarId, 3450.0);
        SetAltarAmount(gameState, D2AltarSystem.RitualBreadAltarId, 3450.0);
        SetAltarAmount(gameState, D2AltarSystem.IncenseAltarId, 300.0);
        SetAltarAmount(gameState, D2AltarSystem.SacredClothAltarId, 300.0);
        SetAltarAmount(gameState, D2AltarSystem.CarvedStoneAltarId, 300.0);
        D2PresentationRules.EnsurePresentationState(gameState);
        PresentationStateUtility.Introduce(
            gameState.dimension2.presentation, PresentationFeatureIds.D2C3Analysis);
        PresentationStateUtility.Introduce(
            gameState.dimension2.presentation, PresentationFeatureIds.D2C3Archive);
        PresentationStateUtility.Introduce(
            gameState.dimension2.presentation, PresentationFeatureIds.D2C3EntityResearch);

        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        Require(panel?.civilization3PanelUI != null,
            "D2Civilization3PanelUI no existe.");
        HideRuntimeOverlay("ReturnModal");
        HideRuntimeOverlay("ReturnPanel");
        ActivateAncestors(panel.transform);
        panel.ShowMap();
        panel.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ConfigureZone(
        D2Civilization3State state, string zoneId, double research,
        long resource, long low, long medium, long high,
        long clues, double clueProgress)
    {
        D2C3ZoneState zone = D2Civilization3System.GetZone(state, zoneId);
        Require(zone != null, "Falta " + zoneId + ".");
        zone.unlocked = true;
        zone.excavationActive = false;
        zone.lowQualityRemains = low;
        zone.mediumQualityRemains = medium;
        zone.highQualityRemains = high;
        zone.totalExcavationsCompleted = 3L;
        zone.scholarHired = true;
        zone.scholarLevel = 1;
        zone.analysisActive = false;
        zone.analysisRemainingSeconds = 0.0;
        zone.analysisQualityId = "";
        zone.zoneResourceAmount = resource;
        zone.researchProgress = research;
        zone.totalAnalysesCompleted = 2L;
        zone.anomalyClues = clues;
        zone.anomalyClueProgress = clueProgress;
    }

    private static void SetAltarAmount(GameState gameState, string altarId, double amount)
    {
        D2AltarState altar = D2AltarSystem.GetAltar(
            gameState.dimension2.civilization1, altarId);
        Require(altar != null, "Falta altar " + altarId + ".");
        altar.offeringAmount = amount;
    }

    private static void ValidatePhysicalRoutesAndRestore()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2Civilization3PanelUI ui = panel?.civilization3PanelUI;
        Require(panel != null && ui != null, "Falta Analizar Restos.");
        ClickPhysical(panel.civilization3Button);
        Require(ui.archaeologySectionRoot.activeInHierarchy,
            "La entrada real no abrió Ruinas Sepultadas.");
        ClickPhysical(ui.showAnalysisButton);
        Require(ui.analysisSectionRoot.activeInHierarchy,
            "La navegación ANALIZAR no abrió la subvista.");
        ClickPhysical(ui.analysisQualityButtons[0]);
        Require(ui.analysisQualitySelectionOverlays[0].activeSelf,
            "La calidad Baja no se seleccionó físicamente.");
        ClickPhysical(ui.analysisQualityButtons[1]);
        Require(ui.analysisQualitySelectionOverlays[1].activeSelf,
            "La calidad Media no se seleccionó físicamente.");
        ClickPhysical(ui.analysisActionButton);
        D2C3ZoneState zone = D2Civilization3System.GetSelectedZone(
            GameState.I.dimension2.civilization3);
        Require(zone.analysisActive &&
            zone.analysisQualityId == D2Civilization3System.MediumQualityId &&
            zone.mediumQualityRemains == 4L,
            "ANALIZAR MEDIA no activó el sistema real o no consumió un resto.");
        RestoreFixtureVisualState();

        ClickPhysical(ui.analysisArchiveButton);
        Require(ui.archiveSectionRoot.activeInHierarchy,
            "ARCHIVO no abrió desde Analizar Restos.");
        ui.ShowAnalysis();
        ClickPhysical(ui.analysisEntityButton);
        Require(ui.entityResearchSectionRoot.activeInHierarchy,
            "ENTE no abrió desde Analizar Restos.");
        ui.ShowAnalysis();
        ClickPhysical(ui.analysisBackButton);
        Require(ui.archaeologySectionRoot.activeInHierarchy,
            "Atrás no regresó a Ruinas Sepultadas.");
        ui.ShowAnalysis();
        RestoreFixtureVisualState();
    }

    private static void RestoreFixtureVisualState()
    {
        GameState gameState = GameState.I;
        D2Civilization3State state = gameState?.dimension2?.civilization3;
        Require(state != null, "No existe el fixture de Analizar Restos.");
        state.selectedZoneId = D2Civilization3System.Zone1Id;
        state.ancientKnowledge = 18.0;
        state.entityKnowledge = 3L;
        ConfigureZone(state, D2Civilization3System.Zone1Id,
            45.0, 26L, 12L, 5L, 2L, 1L, 0.08);
        SetAltarAmount(gameState, D2AltarSystem.WaxAltarId, 3450.0);
        SetAltarAmount(gameState, D2AltarSystem.RitualBreadAltarId, 3450.0);
        SetAltarAmount(gameState, D2AltarSystem.IncenseAltarId, 300.0);
        SetAltarAmount(gameState, D2AltarSystem.SacredClothAltarId, 300.0);
        SetAltarAmount(gameState, D2AltarSystem.CarvedStoneAltarId, 300.0);
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        panel.civilization3PanelUI.ShowAnalysis();
        panel.civilization3PanelUI.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateVisibleState()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2Civilization3PanelUI ui = panel?.civilization3PanelUI;
        Require(ui != null && ui.analysisSectionRoot.activeInHierarchy &&
            !ui.archaeologySectionRoot.activeInHierarchy &&
            !ui.archiveSectionRoot.activeInHierarchy &&
            !ui.entityResearchSectionRoot.activeInHierarchy,
            "Analizar Restos no es el único estado visible de Civilización 3.");
        Require(ui.analysisZoneTitleText.text == "ZONA 1 — ENTRADA SEPULTADA" &&
            ui.analysisAncientKnowledgeValueText.text == "18" &&
            ui.analysisResourceValueText.text == "26" &&
            ui.analysisQualityCountTexts[0].text == "12" &&
            ui.analysisQualityCountTexts[1].text == "5" &&
            ui.analysisQualityCountTexts[2].text == "2" &&
            ui.analysisTotalRemainsText.text == "TOTAL 19" &&
            ui.analysisSelectedQualityText.text == "CALIDAD MEDIA" &&
            ui.analysisResearchValueText.text == "45%" &&
            ui.analysisCivilization1ResourceValueTexts[0].text == "3,450" &&
            ui.analysisCivilization1ResourceValueTexts[4].text == "300",
            "Los datos V4 visibles de Analizar Restos no coinciden.");
        Require(ui.analysisQualitySelectionOverlays.Length == 3 &&
            !ui.analysisQualitySelectionOverlays[0].activeSelf &&
            ui.analysisQualitySelectionOverlays[1].activeSelf &&
            !ui.analysisQualitySelectionOverlays[2].activeSelf &&
            ui.analysisActionButton.interactable,
            "La calidad Media o su acción no refleja el estado real.");
        RectTransform basePlate = ui.analysisSectionRoot.transform
            .Find("AnalyzeRemainsBasePlate") as RectTransform;
        Require(basePlate != null &&
            Mathf.Abs(basePlate.sizeDelta.x - 1080f) <= 1f &&
            Mathf.Abs(basePlate.sizeDelta.y - 1920f) <= 1f,
            "La placa de Analizar Restos no conserva el lienzo 1080x1920.");
        ValidateTextGeometry(ui.analysisSectionRoot.GetComponent<RectTransform>());
        Debug.Log("[D2 Analyze Remains Capture] STRUCTURE_PASS | STATE_PASS | " +
            "ACTIONS_PASS | ROUTE_PASS | TEXT_GEOMETRY_PASS | 1080x1920 + 720x1280");
    }

    private static void ValidateTextGeometry(RectTransform root)
    {
        TMP_Text[] texts = root.GetComponentsInChildren<TMP_Text>(false);
        for (int i = 0; i < texts.Length; i++)
        {
            TMP_Text text = texts[i];
            if (text == null || string.IsNullOrWhiteSpace(text.text)) continue;
            text.ForceMeshUpdate();
            if (text.overflowMode == TextOverflowModes.Truncate)
                Require(!text.isTextOverflowing, "Texto truncado: " + text.name + ".");
            Vector2 rendered = text.GetRenderedValues(false);
            Rect rect = text.rectTransform.rect;
            Require(rendered.x <= rect.width + 2f && rendered.y <= rect.height + 2f,
                "Texto fuera de su caja: " + text.name + ".");
        }
    }

    private static void ClickPhysical(Button button)
    {
        Require(button != null && button.interactable && button.gameObject.activeInHierarchy,
            "No se puede pulsar el botón solicitado: " +
            (button == null ? "NULL" : button.name +
             " | interactable=" + button.interactable +
             " | active=" + button.gameObject.activeInHierarchy) + ".");
        EventSystem eventSystem = EventSystem.current != null
            ? EventSystem.current
            : UnityEngine.Object.FindFirstObjectByType<EventSystem>();
        Require(eventSystem != null, "No existe EventSystem.");
        Canvas.ForceUpdateCanvases();
        RectTransform rect = button.transform as RectTransform;
        Canvas canvas = button.GetComponentInParent<Canvas>();
        Camera camera = canvas != null &&
            canvas.rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? canvas.rootCanvas.worldCamera : null;
        PointerEventData pointer = new PointerEventData(eventSystem)
        {
            button = PointerEventData.InputButton.Left,
            pointerId = -1,
            position = RectTransformUtility.WorldToScreenPoint(
                camera, rect.TransformPoint(rect.rect.center))
        };
        List<RaycastResult> hits = new List<RaycastResult>();
        eventSystem.RaycastAll(pointer, hits);
        GameObject handler = null;
        for (int i = 0; i < hits.Count; i++)
        {
            handler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[i].gameObject);
            if (handler != null) break;
        }
        if (handler == button.gameObject)
        {
            ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerClickHandler);
        }
        else
        {
            Require(button.targetGraphic != null && button.targetGraphic.raycastTarget,
                "Objetivo UGUI inválido para " + button.name + ".");
            button.onClick.Invoke();
        }
        Canvas.ForceUpdateCanvases();
    }

    private static void ActivateAncestors(Transform transform)
    {
        for (Transform current = transform; current != null; current = current.parent)
            current.gameObject.SetActive(true);
    }

    private static void HideRuntimeOverlay(string objectName)
    {
        Transform[] transforms = UnityEngine.Object.FindObjectsByType<Transform>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < transforms.Length; i++)
        {
            if (transforms[i] != null && transforms[i].name == objectName)
                transforms[i].gameObject.SetActive(false);
        }
    }

    private static void StoreSaveFingerprints()
    {
        SessionState.SetString(SaveFingerprintKey,
            Fingerprint(Path.Combine(Application.persistentDataPath, "save.json")));
        SessionState.SetString(BackupFingerprintKey,
            Fingerprint(Path.Combine(Application.persistentDataPath, "save.json.bak")));
    }

    private static void ValidateSaveFingerprints()
    {
        Require(Fingerprint(Path.Combine(Application.persistentDataPath, "save.json")) ==
            SessionState.GetString(SaveFingerprintKey, ""),
            "save.json cambió durante el QA visual.");
        Require(Fingerprint(Path.Combine(Application.persistentDataPath, "save.json.bak")) ==
            SessionState.GetString(BackupFingerprintKey, ""),
            "save.json.bak cambió durante el QA visual.");
    }

    private static void ValidateCaptureFile(string path, int width, int height)
    {
        Require(File.Exists(path) && new FileInfo(path).Length > 0L,
            "No se generó la captura " + width + "x" + height + ".");
        Texture2D image = new Texture2D(2, 2, TextureFormat.RGB24, false);
        try
        {
            Require(ImageConversion.LoadImage(image, File.ReadAllBytes(path), false),
                "PNG inválido: " + path);
            Require(image.width == width && image.height == height,
                "Dimensiones incorrectas en " + path + ".");
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(image);
        }
    }

    private static string Fingerprint(string path)
    {
        if (!File.Exists(path)) return "MISSING";
        using (SHA256 sha = SHA256.Create())
        using (FileStream stream = File.OpenRead(path))
            return new FileInfo(path).Length + ":" +
                BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "");
    }

    private static void RenderToPng(int width, int height, string path)
    {
        Camera camera = Camera.main != null
            ? Camera.main : UnityEngine.Object.FindFirstObjectByType<Camera>();
        Require(camera != null, "No hay cámara para captura.");
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
        RenderTexture target = new RenderTexture(width, height, 24,
            RenderTextureFormat.ARGB32);
        target.antiAliasing = 4;
        target.Create();
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

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
#endif

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

public static class Dimension2RuinsArchaeologyCapture
{
    private const string ActiveKey = "QF.D2RuinsArchaeologyCapture.Active";
    private const string FrameKey = "QF.D2RuinsArchaeologyCapture.Frame";
    private const string FailureKey = "QF.D2RuinsArchaeologyCapture.Failed";
    private const string SaveFingerprintKey = "QF.D2RuinsArchaeologyCapture.Save";
    private const string BackupFingerprintKey = "QF.D2RuinsArchaeologyCapture.Backup";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory => Path.GetFullPath(
        "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/" +
        "DIMENSION_2/RUINAS_SEPULTADAS/CAPTURAS_CANDIDATAS");
    private static string Output1080 => Path.Combine(OutputDirectory,
        "D2_RuinasSepultadas_1080x1920.png");
    private static string Output720 => Path.Combine(OutputDirectory,
        "D2_RuinasSepultadas_720x1280.png");

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
            Debug.Log((failed ? "[D2 Ruins Archaeology Capture] FAIL | " :
                "[D2 Ruins Archaeology Capture] PASS | STRUCTURE_PASS | " +
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
            if (frame == 12) PrepareFixtureAndMap();
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

    private static void PrepareFixtureAndMap()
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
            selectedZoneId = D2Civilization3System.Zone3Id,
            ancientKnowledge = 78.0,
            archiveUnlocked = true,
            archiveLevel = 4,
            stratifiedCartographyUnlocked = false,
            anomalousConcordanceUnlocked = true,
            deepExegesisUnlocked = true,
            anomalyClueDetectionUnlocked = true,
            entityResearchUnlocked = true,
            entityResearchProgress = 50.0,
            entityResearchMilestone30Completed = true,
            entityKnowledge = 3L,
            lastResult = "El Santuario Sellado aguarda una nueva excavación."
        };
        D2Civilization3State state = gameState.dimension2.civilization3;
        D2Civilization3System.EnsureState(state);
        ConfigureZone(state, D2Civilization3System.Zone1Id,
            65.0, 120L, 8L, 3L, 1L, 1L, 0.12);
        ConfigureZone(state, D2Civilization3System.Zone2Id,
            60.0, 84L, 10L, 4L, 1L, 1L, 0.24);
        ConfigureZone(state, D2Civilization3System.Zone3Id,
            46.0, 36L, 12L, 5L, 2L, 2L, 0.38);
        D2PresentationRules.EnsurePresentationState(gameState);

        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        Require(panel != null, "Dimension2PanelUI no existe.");
        HideRuntimeOverlay("ReturnModal");
        HideRuntimeOverlay("ReturnPanel");
        ActivateAncestors(panel.transform);
        Require(panel.civilization3PanelUI != null,
            "D2Civilization3PanelUI no existe.");
        panel.civilization3PanelUI.ShowArchaeology();
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
        zone.excavationRemainingSeconds = 0.0;
        zone.lowQualityRemains = low;
        zone.mediumQualityRemains = medium;
        zone.highQualityRemains = high;
        zone.totalExcavationsCompleted = 3L;
        zone.scholarHired = true;
        zone.scholarLevel = 1;
        zone.analysisActive = false;
        zone.analysisRemainingSeconds = 0.0;
        zone.zoneResourceAmount = resource;
        zone.researchProgress = research;
        zone.totalAnalysesCompleted = 2L;
        zone.anomalyClues = clues;
        zone.anomalyClueProgress = clueProgress;
        zone.anomalyRevealed = false;
        zone.anomalyRead = false;
    }

    private static void ValidatePhysicalRoutesAndRestore()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2Civilization3PanelUI ui = panel?.civilization3PanelUI;
        Require(panel != null && ui != null, "Falta la pantalla de Ruinas Sepultadas.");

        ClickPhysical(panel.civilization3Button);
        Require(ui.archaeologySectionRoot.activeInHierarchy,
            "La entrada real no abrió Ruinas Sepultadas.");
        ClickPhysical(ui.zone1Button);
        Require(GameState.I.dimension2.civilization3.selectedZoneId ==
            D2Civilization3System.Zone1Id && ui.zoneSelectionOverlays[0].activeSelf,
            "La Zona 1 no se seleccionó físicamente.");
        ClickPhysical(ui.zone3Button);
        Require(GameState.I.dimension2.civilization3.selectedZoneId ==
            D2Civilization3System.Zone3Id && ui.zoneSelectionOverlays[2].activeSelf,
            "La Zona 3 no se restauró físicamente.");

        ClickPhysical(ui.excavateButton);
        D2C3ZoneState zone3 = D2Civilization3System.GetSelectedZone(
            GameState.I.dimension2.civilization3);
        Require(zone3 != null && zone3.excavationActive,
            "INICIAR EXCAVACIÓN no activó el sistema real.");
        RestoreFixtureVisualState();

        ClickPhysical(ui.showArchiveButton);
        Require(ui.archiveSectionRoot.activeInHierarchy,
            "ARCHIVO no abrió desde la navegación real.");
        ui.ShowArchaeology();
        ClickPhysical(ui.showEntityResearchButton);
        Require(ui.entityResearchSectionRoot.activeInHierarchy,
            "ENTE no abrió desde la navegación real.");
        ui.ShowArchaeology();
        ClickPhysical(ui.backToMapButton);
        Require(panel.mapRoot.activeInHierarchy,
            "Atrás no regresó al Mapa de los Pactos.");
        ClickPhysical(panel.civilization3Button);
        Require(ui.archaeologySectionRoot.activeInHierarchy,
            "Ruinas Sepultadas no reabrió tras regresar.");
        RestoreFixtureVisualState();
    }

    private static void RestoreFixtureVisualState()
    {
        D2Civilization3State state = GameState.I?.dimension2?.civilization3;
        Require(state != null, "No existe el fixture de Ruinas Sepultadas.");
        state.selectedZoneId = D2Civilization3System.Zone3Id;
        state.ancientKnowledge = 78.0;
        state.archiveLevel = 4;
        state.entityKnowledge = 3L;
        ConfigureZone(state, D2Civilization3System.Zone1Id,
            65.0, 120L, 8L, 3L, 1L, 1L, 0.12);
        ConfigureZone(state, D2Civilization3System.Zone2Id,
            60.0, 84L, 10L, 4L, 1L, 1L, 0.24);
        ConfigureZone(state, D2Civilization3System.Zone3Id,
            46.0, 36L, 12L, 5L, 2L, 2L, 0.38);
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        Require(panel?.civilization3PanelUI != null,
            "No existe la UI de Ruinas Sepultadas.");
        panel.civilization3PanelUI.ShowArchaeology();
        panel.civilization3PanelUI.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateVisibleState()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2Civilization3PanelUI ui = panel?.civilization3PanelUI;
        Require(ui != null && ui.archaeologySectionRoot.activeInHierarchy &&
            !ui.archiveSectionRoot.activeInHierarchy &&
            !ui.entityResearchSectionRoot.activeInHierarchy,
            "Arqueología no es el único estado visible de Civilización 3.");
        Require(ui.ancientKnowledgeHeaderText.text == "CONOCIMIENTO ANTIGUO" &&
            ui.ancientKnowledgeHeaderValueText.text == "78" &&
            ui.archiveHeaderText.text == "ARCHIVO IV" &&
            ui.entityKnowledgeHeaderText.text == "CONOCIMIENTO DEL ENTE" &&
            ui.entityKnowledgeHeaderValueText.text == "3 / 6" &&
            ui.zoneResearchValueTexts[0].text == "65%" &&
            ui.zoneResearchValueTexts[1].text == "60%" &&
            ui.zoneResearchValueTexts[2].text == "46%" &&
            ui.zoneResourceValueTexts[0].text == "120" &&
            ui.zoneResourceValueTexts[1].text == "84" &&
            ui.zoneResourceValueTexts[2].text == "36" &&
            ui.detailTitleText.text == "SANTUARIO SELLADO" &&
            ui.remainsLowText.text == "12" &&
            ui.remainsMediumText.text == "5" &&
            ui.remainsHighText.text == "2" &&
            ui.cluePatternText.text == "PATRÓN        2 / 12" &&
            ui.clueAccumulationText.text == "ACUMULACIÓN        38%" &&
            ui.excavationActionText.text == "INICIAR EXCAVACIÓN",
            "Los datos V4 visibles de Ruinas Sepultadas no coinciden.");
        Require(ui.zoneSelectionOverlays.Length == 3 &&
            !ui.zoneSelectionOverlays[0].activeSelf &&
            !ui.zoneSelectionOverlays[1].activeSelf &&
            ui.zoneSelectionOverlays[2].activeSelf &&
            Mathf.Abs(ui.zoneResearchFillImages[2].fillAmount - 0.46f) <= 0.001f,
            "La selección o barra de Zona 3 no refleja el estado real.");
        Require(ui.excavateButton.interactable &&
            ui.showArchiveButton.interactable &&
            ui.showEntityResearchButton.interactable,
            "Las acciones disponibles no están conectadas.");
        RectTransform root = panel.civilization3Root.GetComponent<RectTransform>();
        RectTransform basePlate = ui.archaeologySectionRoot.transform
            .Find("RuinsArchaeologyBasePlate") as RectTransform;
        Require(basePlate != null &&
            Mathf.Abs(basePlate.sizeDelta.x - 1080f) <= 1f &&
            Mathf.Abs(basePlate.sizeDelta.y - 1920f) <= 1f,
            "La placa de Ruinas Sepultadas no conserva el lienzo 1080x1920.");
        ValidateTextGeometry(root);
        Debug.Log("[D2 Ruins Archaeology Capture] STRUCTURE_PASS | STATE_PASS | " +
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
            {
                Require(!text.isTextOverflowing, "Texto truncado: " + text.name + ".");
            }
            Vector2 rendered = text.GetRenderedValues(false);
            Rect rect = text.rectTransform.rect;
            Require(rendered.x <= rect.width + 2f && rendered.y <= rect.height + 2f,
                "Texto fuera de su caja: " + text.name + ".");
        }
    }

    private static void ClickPhysical(Button button)
    {
        Require(button != null && button.interactable && button.gameObject.activeInHierarchy,
            "No se puede pulsar el botón solicitado.");
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

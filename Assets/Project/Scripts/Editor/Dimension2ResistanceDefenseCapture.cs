#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Dimension2ResistanceDefenseCapture
{
    private const string ActiveKey = "QF.D2ResistanceDefenseCapture.Active";
    private const string FrameKey = "QF.D2ResistanceDefenseCapture.Frame";
    private const string FailureKey = "QF.D2ResistanceDefenseCapture.Failed";
    private const string SaveFingerprintKey = "QF.D2ResistanceDefenseCapture.Save";
    private const string BackupFingerprintKey = "QF.D2ResistanceDefenseCapture.Backup";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory => Path.GetFullPath(
        "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/" +
        "DIMENSION_2/DEFENSA_Y_REPRESALIAS/CAPTURAS_CANDIDATAS");
    private static string Output1080 => Path.Combine(OutputDirectory,
        "D2_Defensa_Y_Represalias_1080x1920.png");
    private static string Output720 => Path.Combine(OutputDirectory,
        "D2_Defensa_Y_Represalias_720x1280.png");

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
            Debug.Log((failed ? "[D2 Resistance Defense Capture] FAIL | " :
                "[D2 Resistance Defense Capture] PASS | STRUCTURE_PASS | STATE_PASS | " +
                "ROUTE_PASS | SAVE_INTACT_PASS | TEXT_GEOMETRY_PASS | " +
                "1080x1920 + 720x1280 | ") + Output1080);
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
        gameState.dimension2.civilization2 = new D2Civilization2State
        {
            initialMembersGranted = true,
            membersAvailable = 3845L,
            totalMembersRecruited = 4253L,
            controlFragments = 78L,
            totalReprisals = 12L,
            selectedRegionId = D2Civilization2System.Region1Id,
            alertActive = false
        };
        D2Civilization2State resistance = gameState.dimension2.civilization2;
        D2Civilization2System.EnsureState(resistance);
        ConfigureRegion1(resistance);
        ConfigureSelectableRegion(resistance, D2Civilization2System.Region2Id);
        ConfigureSelectableRegion(resistance, D2Civilization2System.Region3Id);
        D2PresentationRules.EnsurePresentationState(gameState);

        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        Require(panel != null, "Dimension2PanelUI no existe.");
        HideRuntimeOverlay("ReturnModal");
        HideRuntimeOverlay("ReturnPanel");
        ActivateAncestors(panel.transform);
        Require(panel.civilization2PanelUI != null,
            "D2Civilization2PanelUI no existe.");
        panel.civilization2PanelUI.ShowRegions();
        panel.ShowMap();
        panel.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ConfigureRegion1(D2Civilization2State state)
    {
        D2RegionState region = D2Civilization2System.GetRegion(
            state, D2Civilization2System.Region1Id);
        Require(region != null, "Falta Región 1.");
        region.unlocked = true;
        region.dominance = 52.0;
        region.threat = 72.0;
        region.coverage = 58.0;
        region.membersAssigned = 400L;
        region.totalReprisals = 12L;
        region.nextReprisalEspionageReduction =
            D2Civilization2System.EspionageReprisalReduction;
        region.alertMarked = false;
        region.weakenedOperationId = D2Civilization2System.SabotageOperationId;
        region.weakenedOperationRemainingSeconds = 130.0;
        region.hasLastReprisalResult = true;
        region.lastReprisalMemberLosses = 8L;
        foreach (string operationId in D2Civilization2System.OperationIds)
        {
            D2OperationState operation = D2Civilization2System.GetOperation(
                region, operationId);
            Require(operation != null, "Falta operación " + operationId + ".");
            operation.membersAssigned = operationId ==
                D2Civilization2System.SabotageOperationId ? 20L : 0L;
        }
    }

    private static void ConfigureSelectableRegion(D2Civilization2State state, string id)
    {
        D2RegionState region = D2Civilization2System.GetRegion(state, id);
        Require(region != null, "Falta " + id + ".");
        region.unlocked = true;
        region.dominance = 50.0;
        region.threat = 10.0;
        region.coverage = 0.0;
        region.membersAssigned = 0L;
    }

    private static void RestoreFixtureVisualState()
    {
        D2Civilization2State state = GameState.I?.dimension2?.civilization2;
        Require(state != null, "No existe el estado para restaurar el fixture.");
        ConfigureRegion1(state);
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        Require(panel?.civilization2PanelUI?.reprisalsPanelUI != null,
            "No existe la UI para restaurar el fixture.");
        panel.civilization2PanelUI.reprisalsPanelUI.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidatePhysicalRoutesAndRestore()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2Civilization2PanelUI civilization = panel?.civilization2PanelUI;
        D2ReprisalsPanelUI defense = civilization?.reprisalsPanelUI;
        Require(panel != null && civilization != null && defense != null,
            "Falta la pantalla de Defensa.");

        ClickPhysical(panel.civilization2Button);
        Require(civilization.regionSectionRoot.activeInHierarchy,
            "La entrada real no abrió Regiones.");
        ClickPhysical(civilization.showOperationsButton);
        Require(civilization.operationsSectionRoot.activeInHierarchy,
            "OPERACIONES no abrió.");
        ClickPhysical(civilization.showDefenseButton);
        Require(civilization.defenseSectionRoot.activeInHierarchy,
            "DEFENSA no abrió.");

        ClickPhysical(defense.regionSelectorButton);
        Require(defense.regionNameText.text == "REGIÓN 2",
            "El selector no cambió a Región 2.");
        ClickPhysical(defense.regionSelectorButton);
        Require(defense.regionNameText.text == "REGIÓN 3",
            "El selector no cambió a Región 3.");
        ClickPhysical(defense.regionSelectorButton);
        Require(defense.regionNameText.text == "REGIÓN 1",
            "El selector no restauró Región 1.");

        ClickPhysical(civilization.showResistanceButton);
        Require(civilization.resistanceSectionRoot.activeInHierarchy,
            "RESISTENCIA no abrió.");
        ClickPhysical(civilization.showDefenseButton);
        Require(civilization.defenseSectionRoot.activeInHierarchy,
            "DEFENSA no se restauró.");
        ClickPhysical(civilization.backToMapButton);
        Require(panel.mapRoot.activeInHierarchy,
            "Atrás no regresó al Mapa de los Pactos.");
        ClickPhysical(panel.civilization2Button);
        ClickPhysical(civilization.showDefenseButton);
        Require(civilization.defenseSectionRoot.activeInHierarchy,
            "DEFENSA no abrió tras regresar.");
        defense.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateVisibleState()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2Civilization2PanelUI civilization = panel?.civilization2PanelUI;
        D2ReprisalsPanelUI ui = civilization?.reprisalsPanelUI;
        Require(ui != null && civilization.defenseSectionRoot.activeInHierarchy &&
            !civilization.regionSectionRoot.activeInHierarchy &&
            !civilization.operationsSectionRoot.activeInHierarchy &&
            !civilization.resistanceSectionRoot.activeInHierarchy &&
            !civilization.alertSectionRoot.activeInHierarchy &&
            !civilization.containmentSectionRoot.activeInHierarchy,
            "Defensa no es la única sección visible.");

        Require(ui.availableMembersText.text == "3,845" &&
            ui.fragmentsText.text == "78" &&
            ui.reprisalsCountText.text == "12" &&
            ui.regionNameText.text == "REGIÓN 1" &&
            ui.threatText.text == "72%" &&
            ui.coverageText.text == "58 / 60" &&
            ui.estimatedLossText.text == "2%" &&
            ui.protectionText.text == "LISTO (-5%)" &&
            ui.weakeningText.text == "SABOTAJE AL 50%" &&
            ui.weakeningDurationText.text == "02:10" &&
            ui.lastResultText.text == "8 MIEMBROS PERDIDOS",
            "Los datos V4 de Defensa no coinciden.");
        Require(ui.rulesText.text ==
            "AL LLEGAR A 100% OCURRE UNA REPRESALIA\n" +
            "LA AMENAZA VUELVE A 25%\n" +
            "LA COBERTURA CONSERVA LA MITAD\n" +
            "RECOMPENSA 3 FRAGMENTOS",
            "Las reglas V4 no coinciden con el sistema.");
        Require(Mathf.Abs(ui.threatGauge.Progress - 0.72f) < 0.001f &&
            Mathf.Abs(ui.coverageGauge.Progress - 58f / 60f) < 0.001f,
            "Los medidores no representan los valores reales.");
        D2Civilization2State state = GameState.I.dimension2.civilization2;
        D2RegionState region = D2Civilization2System.GetRegion(
            state, D2Civilization2System.Region1Id);
        Require(state.membersAvailable == 3845L && state.controlFragments == 78L &&
            state.totalReprisals == 12L && Math.Abs(region.threat - 72.0) < 0.01 &&
            Math.Abs(region.coverage - 58.0) < 0.01 &&
            region.lastReprisalMemberLosses == 8L,
            "La navegación alteró el fixture funcional: miembros=" +
            state.membersAvailable + ", fragmentos=" + state.controlFragments +
            ", represalias=" + state.totalReprisals + ", amenaza=" +
            region.threat.ToString(CultureInfo.InvariantCulture) + ", cobertura=" +
            region.coverage.ToString(CultureInfo.InvariantCulture) + ", pérdidas=" +
            region.lastReprisalMemberLosses + ".");

        RectTransform root = panel.civilization2Root.GetComponent<RectTransform>();
        Require(Mathf.Abs(root.rect.width - 1080f) <= 1f &&
            Mathf.Abs(root.rect.height - 1920f) <= 1f,
            "Defensa no conserva el lienzo 1080x1920.");
        ValidateTextGeometry(root);
        Debug.Log("[D2 Resistance Defense Capture] STRUCTURE_PASS | STATE_PASS | " +
            "ROUTE_PASS | TEXT_GEOMETRY_PASS | 1080x1920 + 720x1280");
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
            if (text.overflowMode == TextOverflowModes.Truncate)
            {
                Require(!text.isTextOverflowing,
                    "Texto truncado: " + text.name + ".");
                Vector2 preferred = text.GetPreferredValues(
                    text.text, 10000f, 10000f);
                Require(preferred.x <= text.rectTransform.rect.width + 2f &&
                    preferred.y <= text.rectTransform.rect.height + 2f,
                    "Texto no cabe completo: " + text.name + " (" + preferred +
                    " / " + text.rectTransform.rect.size + ").");
            }
            Vector2 rendered = text.GetRenderedValues(false);
            Rect rect = text.rectTransform.rect;
            Require(!float.IsNaN(rendered.x) && !float.IsInfinity(rendered.x) &&
                !float.IsNaN(rendered.y) && !float.IsInfinity(rendered.y) &&
                rendered.x >= 0f && rendered.y >= 0f,
                "Malla de texto inválida: " + text.name + " (" + rendered + ").");
            Require(rendered.x <= rect.width + 2f && rendered.y <= rect.height + 2f,
                "Texto fuera de su caja: " + text.name + " (" + rendered + " / " +
                rect.size + ").");
            Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(
                root, text.rectTransform);
            Require(bounds.min.x >= root.rect.xMin - 2f &&
                bounds.max.x <= root.rect.xMax + 2f &&
                bounds.min.y >= root.rect.yMin - 2f &&
                bounds.max.y <= root.rect.yMax + 2f,
                "Texto fuera del lienzo: " + text.name + ".");
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
        if (hits.Count == 0)
        {
            Require(button.targetGraphic != null && button.targetGraphic.raycastTarget,
                "Objetivo UGUI inválido para " + button.name + ".");
            // Batchmode puede no activar el GraphicRaycaster del Game View aunque
            // el Button y su UnityEvent serializado sí estén listos para dispositivo.
            button.onClick.Invoke();
            Canvas.ForceUpdateCanvases();
            return;
        }
        GameObject handler = null;
        for (int i = 0; i < hits.Count; i++)
        {
            handler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(
                hits[i].gameObject);
            if (handler != null) break;
        }
        if (handler == null)
        {
            Require(button.targetGraphic != null && button.targetGraphic.raycastTarget,
                "Objetivo UGUI inválido para " + button.name + ".");
            button.onClick.Invoke();
            Canvas.ForceUpdateCanvases();
            return;
        }
        if (handler != button.gameObject)
        {
            List<string> hitNames = new List<string>();
            for (int i = 0; i < hits.Count && i < 8; i++)
            {
                GameObject clickHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(
                    hits[i].gameObject);
                hitNames.Add(hits[i].gameObject.name + "->" +
                    (clickHandler != null ? clickHandler.name : "sin-handler"));
            }
            throw new InvalidOperationException(
                "El raycast fue interceptado antes de " + button.name +
                ". Primer handler: " + (handler != null ? handler.name : "ninguno") +
                ". Hits: " + string.Join(" | ", hitNames));
        }
        ExecuteEvents.Execute(button.gameObject, pointer,
            ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(button.gameObject, pointer,
            ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(button.gameObject, pointer,
            ExecuteEvents.pointerClickHandler);
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
        string save = Fingerprint(Path.Combine(Application.persistentDataPath, "save.json"));
        string backup = Fingerprint(Path.Combine(
            Application.persistentDataPath, "save.json.bak"));
        Require(save == SessionState.GetString(SaveFingerprintKey, ""),
            "save.json cambió durante el QA visual.");
        Require(backup == SessionState.GetString(BackupFingerprintKey, ""),
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
                "Dimensiones incorrectas en " + path + ": " + image.width + "x" +
                image.height + ".");
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
        RenderTexture target = new RenderTexture(
            width, height, 24, RenderTextureFormat.ARGB32);
        RenderTexture previousTarget = camera.targetTexture;
        RenderTexture previousActive = RenderTexture.active;
        try
        {
            camera.targetTexture = target;
            Canvas.ForceUpdateCanvases();
            camera.Render();
            camera.Render();
            RenderTexture.active = target;
            Texture2D image = new Texture2D(
                width, height, TextureFormat.RGB24, false);
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

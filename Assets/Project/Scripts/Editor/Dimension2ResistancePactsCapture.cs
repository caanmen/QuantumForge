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

public static class Dimension2ResistancePactsCapture
{
    private const string ActiveKey = "QF.D2ResistancePactsCapture.Active";
    private const string FrameKey = "QF.D2ResistancePactsCapture.Frame";
    private const string FailureKey = "QF.D2ResistancePactsCapture.Failed";
    private const string SaveFingerprintKey = "QF.D2ResistancePactsCapture.Save";
    private const string BackupFingerprintKey = "QF.D2ResistancePactsCapture.Backup";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory => Path.GetFullPath(
        "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/" +
        "DIMENSION_2/RESISTENCIA_Y_PACTOS/CAPTURAS_CANDIDATAS");
    private static string Output1080 => Path.Combine(OutputDirectory,
        "D2_Resistencia_Y_Pactos_1080x1920.png");
    private static string Output720 => Path.Combine(OutputDirectory,
        "D2_Resistencia_Y_Pactos_720x1280.png");

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
            Debug.Log((failed ? "[D2 Resistance Pacts Capture] FAIL | " :
                "[D2 Resistance Pacts Capture] PASS | STRUCTURE_PASS | " +
                "STATE_PASS | ACTIONS_PASS | ROUTE_PASS | WEAR_PASS | " +
                "SAVE_INTACT_PASS | TEXT_GEOMETRY_PASS | 1080x1920 + " +
                "720x1280 | ") + Output1080);
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
            if (frame == 12) PrepareFixtureAndScreen();
            if (frame == 22) ValidateActionsAndRoutes();
            if (frame == 28)
            {
                RestoreFixtureVisualState();
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

    private static void PrepareFixtureAndScreen()
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
            totalMembersRecruited = 4499L,
            controlFragments = 78L,
            selectedRegionId = D2Civilization2System.Region1Id,
            alertActive = false
        };
        D2Civilization2System.EnsureState(gameState.dimension2.civilization2);
        ConfigureFixture(gameState.dimension2.civilization2);
        D2PresentationRules.EnsurePresentationState(gameState);

        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        Require(panel != null, "Dimension2PanelUI no existe.");
        HideRuntimeOverlay("ReturnModal");
        HideRuntimeOverlay("ReturnPanel");
        HideRuntimeOverlay("Panel_Generacion");
        HideRuntimeOverlay("PrimaryNavigationSlot");
        ActivateAncestors(panel.transform);
        Require(panel.civilization2PanelUI?.resistancePanelUI != null,
            "D2ResistancePanelUI no existe.");
        panel.civilization2PanelUI.ShowRegions();
        panel.ShowMap();
        panel.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ConfigureFixture(D2Civilization2State state)
    {
        state.membersAvailable = 3845L;
        state.controlFragments = 78L;
        state.hiddenSheltersPenaltySeconds = 0.0;
        state.silencedBellsPenaltySeconds = 0.0;
        state.knivesPenaltySeconds = 0.0;
        state.exhaustedMemberBatches.Clear();
        state.exhaustedMemberBatches.Add(new D2ExhaustedMemberBatch
        {
            amount = 215L,
            remainingSeconds = D2Civilization2System.ExhaustedRecoverySeconds
        });
        string[] upgrades = D2Civilization2System.UpgradeIds;
        for (int i = 0; i < state.resistanceUpgrades.Count; i++)
        {
            D2ResistanceUpgradeState upgrade = state.resistanceUpgrades[i];
            if (upgrade == null) continue;
            upgrade.level = upgrade.upgradeId == upgrades[0] ||
                upgrade.upgradeId == upgrades[1] ? 1 : 0;
        }
        for (int i = 0; i < D2Civilization2System.ResistancePactIds.Length; i++)
        {
            D2ResistancePactState pact = D2Civilization2System.GetResistancePact(
                state, D2Civilization2System.ResistancePactIds[i]);
            Require(pact != null, "Falta pacto de resistencia " + i + ".");
            pact.active = i == 0;
            pact.membersAssigned = i == 0 ? 29L : 0L;
            pact.wearProgressSeconds = i == 0 ? 36.0 : 0.0;
        }
    }

    private static void RestoreFixtureVisualState()
    {
        D2Civilization2State state = GameState.I?.dimension2?.civilization2;
        Require(state != null, "No existe el estado del fixture.");
        ConfigureFixture(state);
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2Civilization2PanelUI civilization = panel?.civilization2PanelUI;
        D2ResistancePanelUI ui = civilization?.resistancePanelUI;
        Require(ui != null, "No existe la UI de Resistencia.");
        HideRuntimeOverlay("PrimaryNavigationSlot");
        ClickPhysical(ui.pactCardButtons[0]);
        civilization.ShowResistance();
        ui.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateActionsAndRoutes()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2Civilization2PanelUI civilization = panel?.civilization2PanelUI;
        D2ResistancePanelUI ui = civilization?.resistancePanelUI;
        D2Civilization2State state = GameState.I?.dimension2?.civilization2;
        Require(panel != null && civilization != null && ui != null && state != null,
            "Falta la pantalla de Resistencia.");

        ClickPhysical(panel.civilization2Button);
        Require(civilization.regionSectionRoot.activeInHierarchy,
            "La entrada real no abrió Regiones.");
        ClickPhysical(civilization.showResistanceButton);
        Require(civilization.resistanceSectionRoot.activeInHierarchy,
            "RESISTENCIA no abrió.");
        HideRuntimeOverlay("PrimaryNavigationSlot");

        ClickPhysical(ui.upgradeCardButtons[2]);
        Require(D2Civilization2System.GetUpgradeLevel(
                state, D2Civilization2System.EspionageUpgradeId) == 1 &&
            state.controlFragments == 75L,
            "La compra desde PRÓXIMO COSTE no se aplicó.");
        ConfigureFixture(state);
        ui.Refresh();

        ClickPhysical(ui.pactCardButtons[1]);
        Require(ui.pactSelectionRoots[1].activeSelf &&
            ui.pactDetailTitleText.text.Contains("CAMPANAS SILENCIADAS"),
            "La tarjeta no seleccionó Campanas Silenciadas.");
        ClickPhysical(ui.activateButton);
        D2ResistancePactState bells = D2Civilization2System.GetResistancePact(
            state, D2Civilization2System.SilencedBellsPactId);
        Require(bells.active && bells.membersAssigned == 25L &&
            state.membersAvailable == 3820L,
            "ACTIVAR PACTO no aplicó el requisito real.");
        ClickPhysical(ui.reinforceOneButton);
        Require(bells.membersAssigned == 26L && state.membersAvailable == 3819L,
            "+1 no reforzó el pacto.");
        long exhaustedBefore = D2Civilization2System.GetExhaustedMembers(state);
        bells.wearProgressSeconds = 119.5;
        D2Civilization2System.Tick(GameState.I, 1.0);
        Require(bells.membersAssigned == 25L &&
            D2Civilization2System.GetExhaustedMembers(state) == exhaustedBefore + 1L,
            "El desgaste no movió un miembro a agotados.");
        ClickPhysical(ui.cancelButton);
        Require(!bells.active && bells.membersAssigned == 0L &&
            state.silencedBellsPenaltySeconds > 0.0,
            "CANCELAR PACTO no devolvió miembros ni aplicó penalización.");

        ConfigureFixture(state);
        ui.Refresh();
        ClickPhysical(civilization.showDefenseButton);
        Require(civilization.defenseSectionRoot.activeInHierarchy,
            "DEFENSA no abrió desde Resistencia.");
        ClickPhysical(civilization.showResistanceButton);
        ClickPhysical(civilization.backToMapButton);
        Require(panel.mapRoot.activeInHierarchy,
            "Atrás no regresó al Mapa de los Pactos.");
        ClickPhysical(panel.civilization2Button);
        ClickPhysical(civilization.showResistanceButton);
        Require(civilization.resistanceSectionRoot.activeInHierarchy,
            "RESISTENCIA no reabrió tras regresar.");
    }

    private static void ValidateVisibleState()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2Civilization2PanelUI civilization = panel?.civilization2PanelUI;
        D2ResistancePanelUI ui = civilization?.resistancePanelUI;
        Require(ui != null && civilization.resistanceSectionRoot.activeInHierarchy &&
            !civilization.regionSectionRoot.activeInHierarchy &&
            !civilization.operationsSectionRoot.activeInHierarchy &&
            !civilization.defenseSectionRoot.activeInHierarchy &&
            !civilization.alertSectionRoot.activeInHierarchy &&
            !civilization.containmentSectionRoot.activeInHierarchy,
            "Resistencia no es la única sección visible.");
        Require(ui.availableMembersText.text == "3,845" &&
            ui.fragmentsText.text == "78" && ui.exhaustedText.text == "215",
            "La cabecera no coincide con el fixture.");
        Require(ui.upgradeLevelTexts[0].text == "NIVEL 1/3" &&
            ui.upgradeLevelTexts[1].text == "NIVEL 1/3" &&
            ui.upgradeLevelTexts[2].text == "NIVEL 0/3" &&
            ui.upgradeLevelTexts[3].text == "NIVEL 0/3" &&
            ui.upgradeCostTexts[0].text.EndsWith("6") &&
            ui.upgradeCostTexts[2].text.EndsWith("3"),
            "Los niveles o costes no coinciden.");
        Require(ui.pactStateTexts[0].text == "ACTIVO" &&
            ui.pactMembersOrRequirementTexts[0].text == "29 MIEMBROS" &&
            ui.pactWearTexts[0].text.Contains("84 S") &&
            ui.pactStateTexts[1].text == "INACTIVO" &&
            ui.pactMembersOrRequirementTexts[1].text == "REQUIERE 25\nMIEMBROS" &&
            ui.pactMembersOrRequirementTexts[2].text == "REQUIERE 40\nMIEMBROS" &&
            ui.pactMembersText.text == "29" &&
            ui.pactWearDetailText.text == "1 CADA 120 S" &&
            ui.penaltiesText.text == "NINGUNA",
            "Los pactos no coinciden con la referencia V4.");
        RectTransform root = panel.civilization2Root.GetComponent<RectTransform>();
        Require(Mathf.Abs(root.rect.width - 1080f) <= 1f &&
            Mathf.Abs(root.rect.height - 1920f) <= 1f,
            "Resistencia no conserva el lienzo 1080x1920.");
        ValidateTextGeometry(root);
        ValidateCostRowSpacing(ui);
        Debug.Log("[D2 Resistance Pacts Capture] STRUCTURE_PASS | STATE_PASS | " +
            "ACTIONS_PASS | ROUTE_PASS | WEAR_PASS | TEXT_GEOMETRY_PASS");
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
            Require(!text.isTextOverflowing,
                "Texto desbordado: " + text.name + ".");
            if (text.overflowMode == TextOverflowModes.Truncate)
            {
                Vector2 preferred = text.GetPreferredValues(
                    text.text, 10000f, 10000f);
                Require(preferred.x <= rect.width + 2f &&
                    preferred.y <= rect.height + 2f,
                    "Texto truncado silenciosamente: " + text.name + " (" +
                    preferred + " / " + rect.size + ").");
            }
            Require(rendered.x <= rect.width + 2f && rendered.y <= rect.height + 2f,
                "Texto fuera de su caja: " + text.name + " (" + rendered +
                " / " + rect.size + ").");
            Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(
                root, text.rectTransform);
            Require(bounds.min.x >= root.rect.xMin - 2f &&
                bounds.max.x <= root.rect.xMax + 2f &&
                bounds.min.y >= root.rect.yMin - 2f &&
                bounds.max.y <= root.rect.yMax + 2f,
                "Texto fuera del lienzo: " + text.name + ".");
        }
    }

    private static void ValidateCostRowSpacing(D2ResistancePanelUI ui)
    {
        Require(ui.upgradeCostNormalRoots != null &&
            ui.upgradeCostNormalRoots.Length == 4,
            "Faltan filas de coste independientes.");
        for (int i = 0; i < ui.upgradeCostNormalRoots.Length; i++)
        {
            GameObject row = ui.upgradeCostNormalRoots[i];
            Require(row != null, "Fila de coste nula: " + i + ".");
            RectTransform label = row.transform.Find("Label") as RectTransform;
            RectTransform icon = row.transform.Find("FragmentIcon") as RectTransform;
            RectTransform value = row.transform.Find("Value") as RectTransform;
            Require(label != null && icon != null && value != null,
                "Fila de coste incompleta: " + i + ".");
            Bounds labelBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(
                row.transform, label);
            Bounds iconBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(
                row.transform, icon);
            Bounds valueBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(
                row.transform, value);
            Require(labelBounds.max.x + 3f <= iconBounds.min.x &&
                iconBounds.max.x + 3f <= valueBounds.min.x,
                "Colisión entre etiqueta, icono y cifra del coste: " + i + ".");
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
            "No se puede pulsar " + (button != null ? button.name : "un botón nulo") + ".");
        EventSystem eventSystem = EventSystem.current != null
            ? EventSystem.current : UnityEngine.Object.FindFirstObjectByType<EventSystem>();
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
        if (hits.Count == 0)
        {
            Require(button.targetGraphic != null &&
                button.targetGraphic.raycastTarget &&
                RectTransformUtility.RectangleContainsScreenPoint(
                    rect, pointer.position, camera),
                "Botón fuera de su rectángulo físico: " + button.name + ".");
        }
        else
        {
            Require(handler == button.gameObject,
                "Botón interceptado: " + button.name +
                "; handler=" + (handler != null ? handler.name : "NINGUNO") +
                "; primer hit=" + GetHierarchyPath(hits[0].gameObject) + ".");
        }
        ExecuteEvents.Execute(button.gameObject, pointer,
            ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(button.gameObject, pointer,
            ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(button.gameObject, pointer,
            ExecuteEvents.pointerClickHandler);
        Canvas.ForceUpdateCanvases();
    }

    private static string GetHierarchyPath(GameObject target)
    {
        if (target == null) return "NINGUNO";
        string path = target.name;
        for (Transform parent = target.transform.parent;
             parent != null; parent = parent.parent)
            path = parent.name + "/" + path;
        return path;
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
            if (transforms[i] != null && transforms[i].name == objectName)
                transforms[i].gameObject.SetActive(false);
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

    private static string Fingerprint(string path)
    {
        if (!File.Exists(path)) return "MISSING";
        using (SHA256 sha = SHA256.Create())
        using (FileStream stream = File.OpenRead(path))
            return new FileInfo(path).Length + ":" +
                BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "");
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
                "Dimensiones incorrectas: " + image.width + "x" + image.height + ".");
        }
        finally { UnityEngine.Object.DestroyImmediate(image); }
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

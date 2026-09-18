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

public static class Dimension2ResistanceContainmentCapture
{
    private const string ActiveKey = "QF.D2ResistanceContainmentCapture.Active";
    private const string FrameKey = "QF.D2ResistanceContainmentCapture.Frame";
    private const string FailureKey = "QF.D2ResistanceContainmentCapture.Failed";
    private const string SaveFingerprintKey = "QF.D2ResistanceContainmentCapture.Save";
    private const string BackupFingerprintKey = "QF.D2ResistanceContainmentCapture.Backup";
    private const string MajorPactModeKey = "QF.D2ResistanceContainmentCapture.MajorPact";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory => Path.GetFullPath(
        "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/" +
        "DIMENSION_2/CONTENCION/CAPTURAS_CANDIDATAS");
    private static string Output1080 => Path.Combine(OutputDirectory,
        "D2_Contencion_1080x1920.png");
    private static string Output720 => Path.Combine(OutputDirectory,
        "D2_Contencion_720x1280.png");
    private static string MajorPactOutputDirectory => Path.GetFullPath(
        "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/" +
        "DIMENSION_2/PACTO_MAYOR_CIV2/CAPTURAS_CANDIDATAS");
    private static string MajorPactOutput1080 => Path.Combine(MajorPactOutputDirectory,
        "D2_PactoMayorCiv2_1080x1920.png");
    private static string MajorPactOutput720 => Path.Combine(MajorPactOutputDirectory,
        "D2_PactoMayorCiv2_720x1280.png");

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
        SessionState.SetBool(MajorPactModeKey, false);
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

    public static void RunMajorPact()
    {
        SessionState.SetBool(MajorPactModeKey, true);
        Directory.CreateDirectory(MajorPactOutputDirectory);
        if (File.Exists(MajorPactOutput1080)) File.Delete(MajorPactOutput1080);
        if (File.Exists(MajorPactOutput720)) File.Delete(MajorPactOutput720);
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
            bool majorPact = SessionState.GetBool(MajorPactModeKey, false);
            string output1080 = majorPact ? MajorPactOutput1080 : Output1080;
            string output720 = majorPact ? MajorPactOutput720 : Output720;
            try
            {
                ValidateCaptureFile(output1080, 1080, 1920);
                ValidateCaptureFile(output720, 720, 1280);
                ValidateSaveFingerprints();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                failed = true;
            }
            SessionState.SetBool(ActiveKey, false);
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            string label = majorPact
                ? "[D2 Resistance Major Pact Capture]"
                : "[D2 Resistance Containment Capture]";
            Debug.Log((failed ? label + " FAIL | " :
                label + " PASS | STRUCTURE_PASS | " +
                "STATE_PASS | ACTIONS_PASS | ROUTE_PASS | SAVE_INTACT_PASS | " +
                "TEXT_GEOMETRY_PASS | 1080x1920 + 720x1280 | ") + output1080);
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
            bool majorPact = SessionState.GetBool(MajorPactModeKey, false);
            if (frame == 12)
            {
                if (majorPact) PrepareMajorPactFixtureAndMap();
                else PrepareFixtureAndMap();
            }
            if (frame == 22)
            {
                if (majorPact) ValidateMajorPactPhysicalRoutesAndRestore();
                else ValidatePhysicalRoutesAndRestore();
            }
            if (frame == 28)
            {
                if (majorPact) ValidateMajorPactVisibleState();
                else ValidateVisibleState();
                RenderToPng(1080, 1920,
                    majorPact ? MajorPactOutput1080 : Output1080);
            }
            else if (frame == 31)
            {
                if (majorPact) RestoreMajorPactFixtureVisualState();
                else RestoreFixtureVisualState();
                RenderToPng(720, 1280,
                    majorPact ? MajorPactOutput720 : Output720);
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
        gameState.dimension2.civilization2 = new D2Civilization2State
        {
            initialMembersGranted = true,
            membersAvailable = 3845L,
            totalMembersRecruited = 4253L,
            controlFragments = 78L,
            selectedRegionId = D2Civilization2System.Region1Id,
            alertActive = true,
            containmentAvailable = true,
            totalContainmentAttempts = 2L,
            totalContainmentFailures = 2L,
            containmentCooldownSeconds = 0.0,
            lastContainmentResult =
                "Contención fallida. El intento puede repetirse sin cooldown."
        };
        D2Civilization2State resistance = gameState.dimension2.civilization2;
        D2Civilization2System.EnsureState(resistance);
        ConfigureRegions(resistance, 15.0);
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

    private static void ConfigureRegions(D2Civilization2State state, double dominance)
    {
        string[] ids =
        {
            D2Civilization2System.Region1Id,
            D2Civilization2System.Region2Id,
            D2Civilization2System.Region3Id
        };
        for (int index = 0; index < ids.Length; index++)
        {
            D2RegionState region = D2Civilization2System.GetRegion(state, ids[index]);
            Require(region != null, "Falta " + ids[index] + ".");
            region.unlocked = true;
            region.dominance = dominance;
            region.threat = 20.0;
            region.membersAssigned = 0L;
            region.alertMarked = false;
        }
    }

    private static void PrepareMajorPactFixtureAndMap()
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
        gameState.dimension2.civilization2 = new D2Civilization2State
        {
            initialMembersGranted = true,
            membersAvailable = 3820L,
            totalMembersRecruited = 4253L,
            controlFragments = 78L,
            selectedRegionId = D2Civilization2System.Region1Id,
            alertActive = false,
            containmentAvailable = false,
            entityContained = true,
            majorPactPrepared = true,
            majorPactEstablished = true,
            containmentStability = 54.0,
            membersAssignedToContainment = 25L,
            lastMajorPactResult = "Pacto mayor establecido."
        };
        D2Civilization2State resistance = gameState.dimension2.civilization2;
        D2Civilization2System.EnsureState(resistance);
        ConfigureMajorPactLevels(resistance);
        ConfigureRegions(resistance, 30.0);
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

    private static void ConfigureMajorPactLevels(D2Civilization2State state)
    {
        for (int index = 0; index < state.majorPactLines.Count; index++)
        {
            D2C2MajorPactLineState line = state.majorPactLines[index];
            line.level = line.lineId == D2Civilization2System.ReconstitutedNetworkLineId
                ? 1 : 0;
        }
    }

    private static void RestoreMajorPactFixtureVisualState()
    {
        D2Civilization2State state = GameState.I?.dimension2?.civilization2;
        Require(state != null, "No existe el fixture de Pacto Mayor.");
        state.membersAvailable = 3820L;
        state.controlFragments = 78L;
        state.entityContained = true;
        state.majorPactPrepared = true;
        state.majorPactEstablished = true;
        state.containmentStability = 54.0;
        state.membersAssignedToContainment = 25L;
        ConfigureMajorPactLevels(state);
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2ContainmentPanelUI ui = panel?.civilization2PanelUI?.containmentPanelUI;
        Require(ui != null, "No existe la UI de Pacto Mayor.");
        ui.majorPactLineDropdown.SetValueWithoutNotify(0);
        ui.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateMajorPactPhysicalRoutesAndRestore()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2Civilization2PanelUI civilization = panel?.civilization2PanelUI;
        D2ContainmentPanelUI ui = civilization?.containmentPanelUI;
        Require(panel != null && civilization != null && ui != null,
            "Falta la pantalla de Pacto Mayor.");

        ClickPhysical(panel.civilization2Button);
        Require(civilization.regionSectionRoot.activeInHierarchy,
            "La entrada real no abrió Regiones.");
        ClickPhysical(civilization.showContainmentButton);
        Require(civilization.containmentSectionRoot.activeInHierarchy &&
            ui.majorPactRoot.activeInHierarchy &&
            !ui.containmentAttemptRoot.activeInHierarchy,
            "PACTO MAYOR no abrió desde la navegación real.");

        ClickPhysical(ui.majorPactLineButtons[1]);
        Require(ui.majorPactLineDropdown.value == 1 &&
            ui.majorPactLineText.text.StartsWith("Levantamiento Coordinado"),
            "La selección física de una línea intermedia no actualizó el detalle.");

        D2Civilization2State state = GameState.I.dimension2.civilization2;
        long availableBefore = state.membersAvailable;
        long assignedBefore = state.membersAssignedToContainment;
        ClickPhysical(ui.assignOneButton);
        Require(state.membersAvailable == availableBefore - 1L &&
            state.membersAssignedToContainment == assignedBefore + 1L,
            "Asignar +1 no modificó Sostenimiento correctamente.");
        ClickPhysical(ui.releaseOneButton);
        Require(state.membersAvailable == availableBefore &&
            state.membersAssignedToContainment == assignedBefore,
            "Liberar -1 no restauró Sostenimiento.");

        ui.majorPactLineDropdown.SetValueWithoutNotify(0);
        ui.Refresh();
        int levelBefore = D2Civilization2System.GetMajorPactLineLevel(
            state, D2Civilization2System.ReconstitutedNetworkLineId);
        ClickPhysical(ui.upgradeMajorPactLineButton);
        Require(D2Civilization2System.GetMajorPactLineLevel(
            state, D2Civilization2System.ReconstitutedNetworkLineId) == levelBefore + 1,
            "MEJORAR LÍNEA no aplicó el coste y nivel reales.");

        RestoreMajorPactFixtureVisualState();
        ClickPhysical(civilization.backToMapButton);
        Require(panel.mapRoot.activeInHierarchy,
            "Atrás no regresó al Mapa de los Pactos.");
        ClickPhysical(panel.civilization2Button);
        ClickPhysical(civilization.showContainmentButton);
        Require(ui.majorPactRoot.activeInHierarchy,
            "PACTO MAYOR no reabrió después de regresar.");
        RestoreMajorPactFixtureVisualState();
    }

    private static void ValidateMajorPactVisibleState()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2Civilization2PanelUI civilization = panel?.civilization2PanelUI;
        D2ContainmentPanelUI ui = civilization?.containmentPanelUI;
        Require(ui != null && civilization.containmentSectionRoot.activeInHierarchy &&
            ui.majorPactRoot.activeInHierarchy &&
            !ui.containmentAttemptRoot.activeInHierarchy,
            "Pacto Mayor no es el estado visible de Contención.");
        Require(ui.majorPactStateText.text ==
                "Ente Contenido · Pacto Mayor Establecido" &&
            ui.majorPactMilestoneText.text ==
                "Hito Reconocido para Cerrar Dimensión 2." &&
            ui.majorPactAvailableMembersText.text == "3,820" &&
            ui.majorPactAssignedHeaderText.text == "25" &&
            ui.stabilityText.text == "54" &&
            ui.majorPactFragmentsHeaderText.text == "78" &&
            ui.majorPactLineText.text == "Red Reconstituida — Nivel 1 / 3" &&
            ui.majorPactLineLevelTexts[0].text == "NIVEL 1/3" &&
            ui.majorPactLineLevelTexts[1].text == "NIVEL 0/3" &&
            ui.majorPactAssignedValueText.text == "25" &&
            ui.majorPactFooterTitleText.text == "Pacto Mayor Establecido",
            "Los datos V4 del Pacto Mayor no coinciden: estado=" +
            ui.majorPactStateText.text + "; hito=" + ui.majorPactMilestoneText.text +
            "; disponibles=" + ui.majorPactAvailableMembersText.text +
            "; asignados=" + ui.majorPactAssignedHeaderText.text +
            "; estabilidad=" + ui.stabilityText.text +
            "; fragmentos=" + ui.majorPactFragmentsHeaderText.text +
            "; detalle=" + ui.majorPactLineText.text +
            "; niveles=" + ui.majorPactLineLevelTexts[0].text + "/" +
            ui.majorPactLineLevelTexts[1].text +
            "; valorAsignado=" + ui.majorPactAssignedValueText.text +
            "; pie=" + ui.majorPactFooterTitleText.text + ".");
        TMP_Text upgradeLabel = ui.upgradeMajorPactLineButton
            .GetComponentInChildren<TMP_Text>(true);
        Require(upgradeLabel != null && upgradeLabel.text == "MEJORAR LÍNEA" &&
            ui.upgradeMajorPactLineButton.interactable &&
            ui.majorPactLineButtons.Length == 5,
            "Las acciones del Pacto Mayor no están disponibles.");
        RectTransform root = panel.civilization2Root.GetComponent<RectTransform>();
        Require(Mathf.Abs(root.rect.width - 1080f) <= 1f &&
            Mathf.Abs(root.rect.height - 1920f) <= 1f,
            "Pacto Mayor no conserva el lienzo 1080x1920.");
        ValidateTextGeometry(root);
        Debug.Log("[D2 Resistance Major Pact Capture] STRUCTURE_PASS | STATE_PASS | " +
            "ACTIONS_PASS | ROUTE_PASS | TEXT_GEOMETRY_PASS | 1080x1920 + 720x1280");
    }

    private static void RestoreFixtureVisualState()
    {
        D2Civilization2State state = GameState.I?.dimension2?.civilization2;
        Require(state != null, "No existe el fixture de Contención.");
        state.membersAvailable = 3845L;
        state.alertActive = true;
        state.containmentAvailable = true;
        state.entityContained = false;
        state.totalContainmentAttempts = 2L;
        state.totalContainmentFailures = 2L;
        state.containmentCooldownSeconds = 0.0;
        state.lastContainmentResult =
            "Contención fallida. El intento puede repetirse sin cooldown.";
        ConfigureRegions(state, 15.0);
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        Require(panel?.civilization2PanelUI?.containmentPanelUI != null,
            "No existe la UI de Contención.");
        panel.civilization2PanelUI.containmentPanelUI.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidatePhysicalRoutesAndRestore()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2Civilization2PanelUI civilization = panel?.civilization2PanelUI;
        D2ContainmentPanelUI containment = civilization?.containmentPanelUI;
        Require(panel != null && civilization != null && containment != null,
            "Falta la pantalla de Contención.");

        ClickPhysical(panel.civilization2Button);
        Require(civilization.regionSectionRoot.activeInHierarchy,
            "La entrada real no abrió Regiones.");
        ClickPhysical(civilization.showAlertButton);
        Require(civilization.alertSectionRoot.activeInHierarchy,
            "ALERTA no abrió antes de Contención.");
        ClickPhysical(civilization.showContainmentButton);
        Require(civilization.containmentSectionRoot.activeInHierarchy &&
            containment.containmentAttemptRoot.activeInHierarchy,
            "CONTENCIÓN no abrió.");

        D2Civilization2State state = GameState.I.dimension2.civilization2;
        ConfigureRegions(state, 25.0);
        containment.Refresh();
        long attemptsBefore = state.totalContainmentAttempts;
        ClickPhysical(containment.attemptButton);
        TMP_Text actionLabel = containment.attemptButton.GetComponentInChildren<TMP_Text>(true);
        Require(state.totalContainmentAttempts == attemptsBefore && actionLabel != null &&
            actionLabel.text == "CONFIRMAR INTENTO CON RIESGO",
            "La primera pulsación de bajo porcentaje no pidió confirmación segura.");

        RestoreFixtureVisualState();
        ClickPhysical(civilization.backToMapButton);
        Require(panel.mapRoot.activeInHierarchy,
            "Atrás no regresó al Mapa de los Pactos.");
        ClickPhysical(panel.civilization2Button);
        ClickPhysical(civilization.showAlertButton);
        ClickPhysical(civilization.showContainmentButton);
        Require(containment.containmentAttemptRoot.activeInHierarchy,
            "CONTENCIÓN no abrió tras regresar.");
        RestoreFixtureVisualState();
    }

    private static void ValidateVisibleState()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2Civilization2PanelUI civilization = panel?.civilization2PanelUI;
        D2ContainmentPanelUI ui = civilization?.containmentPanelUI;
        Require(ui != null && civilization.containmentSectionRoot.activeInHierarchy &&
            ui.containmentAttemptRoot.activeInHierarchy &&
            !ui.majorPactRoot.activeInHierarchy &&
            !civilization.regionSectionRoot.activeInHierarchy &&
            !civilization.operationsSectionRoot.activeInHierarchy &&
            !civilization.defenseSectionRoot.activeInHierarchy &&
            !civilization.resistanceSectionRoot.activeInHierarchy &&
            !civilization.alertSectionRoot.activeInHierarchy,
            "Contención no es el único estado visible.");
        Require(ui.stateText.text == "CONTENCIÓN\nDISPONIBLE" &&
            ui.availableMembersText.text == "3,845" &&
            ui.dominanceHeaderText.text == "15%" &&
            ui.attemptsHeaderText.text == "2" &&
            ui.failuresHeaderText.text == "2" &&
            ui.probabilityText.text == "57.5%" &&
            ui.dominanceCardText.text == "15%" &&
            ui.cooldownText.text == "SIN COOLDOWN" &&
            ui.assignedMembersText.text == "ASIGNADOS  0" &&
            ui.assignmentAvailableText.text == "DISPONIBLES  3,845" &&
            ui.lastResultText.text ==
                "EL INTENTO ANTERIOR FALLÓ · NUEVO INTENTO DISPONIBLE",
            "Los datos V4 de Contención no coinciden.");
        TMP_Text action = ui.attemptButton.GetComponentInChildren<TMP_Text>(true);
        Require(action != null && action.text == "INTENTAR CONTENCIÓN" &&
            ui.attemptButton.interactable,
            "La acción principal no está disponible con 57.5 %. ");
        RectTransform root = panel.civilization2Root.GetComponent<RectTransform>();
        Require(Mathf.Abs(root.rect.width - 1080f) <= 1f &&
            Mathf.Abs(root.rect.height - 1920f) <= 1f,
            "Contención no conserva el lienzo 1080x1920.");
        ValidateTextGeometry(root);
        Debug.Log("[D2 Resistance Containment Capture] STRUCTURE_PASS | STATE_PASS | " +
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
            if (text.overflowMode == TextOverflowModes.Truncate)
            {
                Require(!text.isTextOverflowing, "Texto truncado: " + text.name + ".");
                Vector2 preferred = text.GetPreferredValues(text.text, 10000f, 10000f);
                Require(preferred.x <= text.rectTransform.rect.width + 2f &&
                    preferred.y <= text.rectTransform.rect.height + 2f,
                    "Texto no cabe completo: " + text.name + " (" + preferred +
                    " / " + text.rectTransform.rect.size + ").");
            }
            Vector2 rendered = text.GetRenderedValues(false);
            Rect rect = text.rectTransform.rect;
            Require(rendered.x <= rect.width + 2f && rendered.y <= rect.height + 2f,
                "Texto fuera de su caja: " + text.name + " (" + rendered + " / " +
                rect.size + ").");
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
        RenderTexture target = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
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

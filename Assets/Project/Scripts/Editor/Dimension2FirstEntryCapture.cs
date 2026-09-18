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

public static class Dimension2FirstEntryCapture
{
    private const string ActiveKey = "QF.D2FirstEntryCapture.Active";
    private const string FrameKey = "QF.D2FirstEntryCapture.Frame";
    private const string FailureKey = "QF.D2FirstEntryCapture.Failed";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory => Path.GetFullPath(
        "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/DIMENSION_2/PRIMERA_ENTRADA/CAPTURAS_CANDIDATAS"
    );
    private static string Output1080 => Path.Combine(OutputDirectory,
        "D2_FirstEntry_1080x1920.png");
    private static string Output720 => Path.Combine(OutputDirectory,
        "D2_FirstEntry_720x1280.png");
    private static string OutputHover1080 => Path.Combine(OutputDirectory,
        "D2_FirstEntry_Hover_1080x1920.png");
    private static string MapOutputDirectory => Path.GetFullPath(
        "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/DIMENSION_2/MAPA_DE_LOS_PACTOS/CAPTURAS_CANDIDATAS"
    );
    private static string MapOutput1080 => Path.Combine(MapOutputDirectory,
        "D2_Mapa_De_Los_Pactos_1080x1920.png");
    private static string MapOutput720 => Path.Combine(MapOutputDirectory,
        "D2_Mapa_De_Los_Pactos_720x1280.png");
    private static string SanctuaryOutputDirectory => Path.GetFullPath(
        "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/DIMENSION_2/SANTUARIO_REFUGIO/CAPTURAS_CANDIDATAS"
    );
    private static string SanctuaryOutput1080 => Path.Combine(SanctuaryOutputDirectory,
        "D2_Santuario_Refugio_1080x1920.png");
    private static string SanctuaryOutput720 => Path.Combine(SanctuaryOutputDirectory,
        "D2_Santuario_Refugio_720x1280.png");
    private static string AltarsOutputDirectory => Path.GetFullPath(
        "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/DIMENSION_2/ALTARES_DEL_SANTUARIO/CAPTURAS_CANDIDATAS"
    );
    private static string AltarsOutput1080 => Path.Combine(AltarsOutputDirectory,
        "D2_Altares_Del_Santuario_1080x1920.png");
    private static string AltarsOutput720 => Path.Combine(AltarsOutputDirectory,
        "D2_Altares_Del_Santuario_720x1280.png");
    private static string PilgrimagesOutputDirectory => Path.GetFullPath(
        "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/DIMENSION_2/PEREGRINACIONES/CAPTURAS_CANDIDATAS"
    );
    private static string PilgrimagesOutput1080 => Path.Combine(PilgrimagesOutputDirectory,
        "D2_Peregrinaciones_1080x1920.png");
    private static string PilgrimagesOutput720 => Path.Combine(PilgrimagesOutputDirectory,
        "D2_Peregrinaciones_720x1280.png");
    private static string NovitiateOutputDirectory => Path.GetFullPath(
        "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/DIMENSION_2/NOVICIADO/CAPTURAS_CANDIDATAS"
    );
    private static string NovitiateOutput1080 => Path.Combine(NovitiateOutputDirectory,
        "D2_Noviciado_1080x1920.png");
    private static string NovitiateOutput720 => Path.Combine(NovitiateOutputDirectory,
        "D2_Noviciado_720x1280.png");
    private static string RitesOutputDirectory => Path.GetFullPath(
        "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/DIMENSION_2/RITOS_DEL_SANTUARIO/CAPTURAS_CANDIDATAS"
    );
    private static string RitesOutput1080 => Path.Combine(RitesOutputDirectory,
        "D2_Ritos_Del_Santuario_1080x1920.png");
    private static string RitesOutput720 => Path.Combine(RitesOutputDirectory,
        "D2_Ritos_Del_Santuario_720x1280.png");
    private static string PactsOutputDirectory => Path.GetFullPath(
        "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/DIMENSION_2/PACTOS_DE_CIVILIZACION/CAPTURAS_CANDIDATAS"
    );
    private static string PactsOutput1080 => Path.Combine(PactsOutputDirectory,
        "D2_Pactos_De_Civilizacion_1080x1920.png");
    private static string PactsOutput720 => Path.Combine(PactsOutputDirectory,
        "D2_Pactos_De_Civilizacion_720x1280.png");
    private static string BondOutputDirectory => Path.GetFullPath(
        "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/DIMENSION_2/PACTO_LUGAR_DE_VINCULO/CAPTURAS_CANDIDATAS"
    );
    private static string BondOutput1080 => Path.Combine(BondOutputDirectory,
        "D2_Pacto_Lugar_De_Vinculo_1080x1920.png");
    private static string BondOutput720 => Path.Combine(BondOutputDirectory,
        "D2_Pacto_Lugar_De_Vinculo_720x1280.png");

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

    public static void Run()
    {
        Directory.CreateDirectory(OutputDirectory);
        Directory.CreateDirectory(MapOutputDirectory);
        Directory.CreateDirectory(SanctuaryOutputDirectory);
        Directory.CreateDirectory(AltarsOutputDirectory);
        Directory.CreateDirectory(PilgrimagesOutputDirectory);
        Directory.CreateDirectory(NovitiateOutputDirectory);
        Directory.CreateDirectory(RitesOutputDirectory);
        Directory.CreateDirectory(PactsOutputDirectory);
        Directory.CreateDirectory(BondOutputDirectory);
        if (File.Exists(Output1080)) File.Delete(Output1080);
        if (File.Exists(Output720)) File.Delete(Output720);
        if (File.Exists(OutputHover1080)) File.Delete(OutputHover1080);
        if (File.Exists(MapOutput1080)) File.Delete(MapOutput1080);
        if (File.Exists(MapOutput720)) File.Delete(MapOutput720);
        if (File.Exists(SanctuaryOutput1080)) File.Delete(SanctuaryOutput1080);
        if (File.Exists(SanctuaryOutput720)) File.Delete(SanctuaryOutput720);
        if (File.Exists(AltarsOutput1080)) File.Delete(AltarsOutput1080);
        if (File.Exists(AltarsOutput720)) File.Delete(AltarsOutput720);
        if (File.Exists(PilgrimagesOutput1080)) File.Delete(PilgrimagesOutput1080);
        if (File.Exists(PilgrimagesOutput720)) File.Delete(PilgrimagesOutput720);
        if (File.Exists(NovitiateOutput1080)) File.Delete(NovitiateOutput1080);
        if (File.Exists(NovitiateOutput720)) File.Delete(NovitiateOutput720);
        if (File.Exists(RitesOutput1080)) File.Delete(RitesOutput1080);
        if (File.Exists(RitesOutput720)) File.Delete(RitesOutput720);
        if (File.Exists(PactsOutput1080)) File.Delete(PactsOutput1080);
        if (File.Exists(PactsOutput720)) File.Delete(PactsOutput720);
        if (File.Exists(BondOutput1080)) File.Delete(BondOutput1080);
        if (File.Exists(BondOutput720)) File.Delete(BondOutput720);
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
            Debug.Log((failed
                ? "[D2 First Entry Capture] FAIL | "
                : "[D2 First Entry Capture] PASS | primera entrada + ABRIR MAPA real | 1080x1920 + 720x1280 | ")
                + Output1080 + " | mapa: " + MapOutput1080 + " | altares: " + AltarsOutput1080 +
                " | peregrinaciones: " + PilgrimagesOutput1080 +
                " | noviciado: " + NovitiateOutput1080 +
                " | ritos: " + RitesOutput1080 +
                " | pactos: " + PactsOutput1080 +
                " | vínculo: " + BondOutput1080);
            EditorApplication.Exit(failed ? 1 : 0);
        }
    }

    private static void Tick()
    {
        try
        {
            int frame = SessionState.GetInt(FrameKey, 0) + 1;
            SessionState.SetInt(FrameKey, frame);
            if (frame >= 12) SuppressReports();
            if (frame == 22) PrepareFirstEntry();
            if (frame >= 23 && frame <= 58) ForceFirstEntryVisible();
            if (frame == 48)
            {
                ValidateFirstEntryVisible();
                RenderToPng(1080, 1920, Output1080);
                RenderToPng(720, 1280, Output720);
            }
            if (frame == 50) SetButtonHover(true);
            if (frame == 56)
            {
                RenderToPng(1080, 1920, OutputHover1080);
                ValidateHoverBackground(OutputHover1080);
            }
            if (frame == 58) SetButtonHover(false);
            if (frame == 60) PrepareMapState();
            if (frame == 62) PressOpenMap();
            if (frame == 72)
            {
                ValidateTransition();
                ValidatePactMapVisible();
                RenderToPng(1080, 1920, MapOutput1080);
                RenderToPng(720, 1280, MapOutput720);
            }
            if (frame == 74)
            {
                PrepareSanctuaryState();
                PressEnterSanctuary();
            }
            if (frame == 86)
            {
                ValidateSanctuaryVisible();
                RenderToPng(1080, 1920, SanctuaryOutput1080);
                RenderToPng(720, 1280, SanctuaryOutput720);
            }
            if (frame == 88) PressOpenAltars();
            if (frame == 99) NormalizeAltarsCaptureState();
            if (frame == 100)
            {
                ValidateAltarsVisible();
                RenderToPng(1080, 1920, AltarsOutput1080);
                RenderToPng(720, 1280, AltarsOutput720);
            }
            if (frame == 102)
            {
                PreparePilgrimagesCaptureState();
                PressOpenPilgrimages();
            }
            if (frame == 112) ValidatePilgrimagesActionsAndRestore();
            if (frame == 114)
            {
                ValidatePilgrimagesVisible();
                RenderToPng(1080, 1920, PilgrimagesOutput1080);
                RenderToPng(720, 1280, PilgrimagesOutput720);
            }
            if (frame == 116) { PrepareNovitiateCaptureState(); PressOpenNovitiate(); }
            if (frame == 126) ValidateNovitiateActionsAndRestore();
            if (frame == 128)
            {
                ValidateNovitiateVisible();
                RenderToPng(1080, 1920, NovitiateOutput1080);
                RenderToPng(720, 1280, NovitiateOutput720);
            }
            if (frame == 130) { PrepareRitesCaptureState(); PressOpenRites(); }
            if (frame == 140) ValidateRitesActionsAndRestore();
            if (frame == 142)
            {
                ValidateRitesVisible();
                RenderToPng(1080, 1920, RitesOutput1080);
                RenderToPng(720, 1280, RitesOutput720);
            }
            if (frame == 144) { PreparePactsCaptureState(); PressOpenPacts(); }
            if (frame == 154) ValidatePactsActionsAndRestore();
            if (frame == 156)
            {
                ValidatePactsVisible();
                RenderToPng(1080, 1920, PactsOutput1080);
                RenderToPng(720, 1280, PactsOutput720);
            }
            if (frame == 158) { PrepareBondCaptureState(); PressOpenBond(); }
            if (frame == 168) ValidateBondActionsAndRestore();
            if (frame != 170) return;
            ValidateBondVisible();
            RenderToPng(1080, 1920, BondOutput1080);
            RenderToPng(720, 1280, BondOutput720);
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

    private static void PrepareFirstEntry()
    {
        GameState state = GameState.I != null
            ? GameState.I
            : UnityEngine.Object.FindFirstObjectByType<GameState>();
        if (state == null)
            throw new InvalidOperationException("GameState no existe.");
        state.dimension02Unlocked = true;
        Dimension2System.EnsureState(state);
        // Simula una partida existente que ya vio la introducción antigua, pero todavía
        // no ha visto la composición visual vigente.
        state.dimension2.firstEntrySeen = true;
        state.dimension2.firstEntryVisualVersionSeen =
            Dimension2System.FirstEntryVisualVersion - 1;
        state.dimension2.presentation.onboardingStage = 0;

        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include
        );
        if (panel == null)
            throw new InvalidOperationException("Dimension2PanelUI no existe.");
        ActivateAncestors(panel.transform);
        panel.OpenFromTab();
        if (!panel.firstEntryRoot.activeSelf)
            throw new InvalidOperationException(
                "Una partida existente omitió la nueva primera entrada versionada.");
        Canvas.ForceUpdateCanvases();
    }

    private static void ForceFirstEntryVisible()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include
        );
        if (panel == null || panel.firstEntryRoot == null)
            throw new InvalidOperationException("Falta D2_FirstEntry.");
        ActivateAncestors(panel.transform);
        panel.firstEntryRoot.SetActive(true);
        if (panel.mapRoot != null) panel.mapRoot.SetActive(false);
        if (panel.civilization1Root != null) panel.civilization1Root.SetActive(false);
        if (panel.civilization2Root != null) panel.civilization2Root.SetActive(false);
        if (panel.civilization3Root != null) panel.civilization3Root.SetActive(false);
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateFirstEntryVisible()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include
        );
        if (panel == null || panel.firstEntryRoot == null ||
            !panel.firstEntryRoot.activeInHierarchy)
            throw new InvalidOperationException("La primera entrada no está visible.");

        RectTransform root = panel.firstEntryRoot.GetComponent<RectTransform>();
        if (root == null || Mathf.Abs(root.rect.width - 1080f) > 1f ||
            Mathf.Abs(root.rect.height - 1920f) > 1f)
            throw new InvalidOperationException("D2_FirstEntry no conserva el lienzo 1080x1920.");
        Canvas canvas = panel.firstEntryRoot.GetComponent<Canvas>();
        if (canvas == null || !canvas.overrideSorting)
            throw new InvalidOperationException("D2_FirstEntry no está en primer plano.");
        if (FindChild(panel.firstEntryRoot.transform, "HeroArt")?.GetComponent<Image>()?.sprite == null)
            throw new InvalidOperationException("La ilustración ceremonial no está conectada.");
        if (panel.firstEntryTitleText == null ||
            panel.firstEntryTitleText.text != "DIMENSIÓN 2 · PACTOS")
            throw new InvalidOperationException("El título no coincide.");
        const string expected =
            "ANTE TI APARECE UN MUNDO DIVIDIDO EN TRES TERRITORIOS.\n" +
            "SOLO EL SANTUARIO DE PEREGRINOS RESPONDE A TU LLEGADA.";
        if (panel.firstEntryDescriptionText == null ||
            panel.firstEntryDescriptionText.text != expected)
            throw new InvalidOperationException("El texto narrativo no coincide.");
        if (FindChild(panel.firstEntryRoot.transform, "MechanicsPanel") != null)
            throw new InvalidOperationException("Existe el panel de mecánica eliminado.");
        TMP_Text label = panel.continueFirstEntryButton != null
            ? panel.continueFirstEntryButton.GetComponentInChildren<TMP_Text>(true)
            : null;
        if (label == null || label.text != "ABRIR MAPA" ||
            !panel.continueFirstEntryButton.interactable)
            throw new InvalidOperationException("ABRIR MAPA no está disponible.");
    }

    private static void PressOpenMap()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include
        );
        if (panel?.continueFirstEntryButton == null)
            throw new InvalidOperationException("Falta el botón real ABRIR MAPA.");
        panel.continueFirstEntryButton.onClick.Invoke();
        Canvas.ForceUpdateCanvases();
    }

    private static void PrepareMapState()
    {
        GameState state = GameState.I;
        if (state?.dimension2?.civilization1 == null)
            throw new InvalidOperationException("No existe el estado requerido del mapa.");
        state.dimension2.civilization1 = new D2Civilization1State();
        state.dimension2.civilization2 = new D2Civilization2State();
        D2Civilization1State civilization1 = state.dimension2.civilization1;
        civilization1.followersAvailable = 3845;
        civilization1.followersAssignedToRefuge = 0;
        civilization1.trust = 280.0;
        state.dimension2.civilization1Unlocked = true;
        state.dimension2.civilization2Unlocked = false;
        state.dimension2.civilization3Unlocked = false;
    }

    private static void PrepareSanctuaryState()
    {
        GameState state = GameState.I;
        if (state?.dimension2 == null)
            throw new InvalidOperationException("No existe el estado requerido del Santuario.");
        D2Civilization1State civilization1 = new D2Civilization1State
        {
            initialFollowersGranted = true,
            followersAvailable = 3845,
            followersAssignedToRefuge = 1280,
            totalFollowersReceived = 5525,
            refugeLevel = 4,
            trust = 280.0,
            totalPilgrimagesCompleted = 10,
            mediumPilgrimagesCompleted = 2,
            novitiateLevel = 2,
            totalAcolytesCreated = 1,
            acolytesAvailable = 1
        };
        state.dimension2.civilization1 = civilization1;
        D2Civilization1System.EnsureState(civilization1);
        D2AltarSystem.GetAltar(civilization1,
            D2AltarSystem.WaxAltarId).offeringAmount = 3450.0;
        D2AltarSystem.GetAltar(civilization1,
            D2AltarSystem.WaxAltarId).followersAssigned = 120;
        D2AltarSystem.GetAltar(civilization1,
            D2AltarSystem.RitualBreadAltarId).offeringAmount = 3450.0;
        D2RiteSystem.GetRite(civilization1,
            D2RiteSystem.WelcomeId).followersAssigned = 400;
        D2CivilizationPactSystem.GetPact(civilization1,
            D2CivilizationPactSystem.HospitalityId).active = true;
        D2PresentationRules.EnsurePresentationState(state);
        state.dimension2.civilization1Unlocked = true;
        state.dimension2.civilization2Unlocked = false;
        state.dimension2.civilization3Unlocked = false;
    }

    private static void PressEnterSanctuary()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        if (panel?.civilization1Button == null)
            throw new InvalidOperationException("Falta ENTRAR AL SANTUARIO.");
        panel.civilization1Button.onClick.Invoke();
        panel.civilization1PanelUI.ShowRefugeSection();
        panel.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateSanctuaryVisible()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2Civilization1PanelUI sanctuary = panel?.civilization1PanelUI;
        if (panel?.civilization1Root == null || sanctuary == null ||
            !panel.civilization1Root.activeInHierarchy ||
            !sanctuary.refugeSectionRoot.activeInHierarchy)
            throw new InvalidOperationException("El Santuario/Refugio no está visible.");
        RectTransform root = panel.civilization1Root.GetComponent<RectTransform>();
        Canvas canvas = panel.civilization1Root.GetComponent<Canvas>();
        if (root == null || Mathf.Abs(root.sizeDelta.x - 1080f) > 1f ||
            Mathf.Abs(root.sizeDelta.y - 1920f) > 1f || canvas == null ||
            !canvas.overrideSorting)
            throw new InvalidOperationException(
                "El Santuario no conserva su lienzo 1080x1920: " +
                (root != null ? root.sizeDelta.x + "x" + root.sizeDelta.y : "sin rect") +
                ", canvas=" + (canvas != null) +
                ", override=" + (canvas != null && canvas.overrideSorting));
        if (sanctuary.followersText?.text != "3,845" ||
            sanctuary.trustText?.text != "280 / 500" ||
            sanctuary.waxText?.text != "3,450" ||
            sanctuary.ritualBreadText?.text != "3,450" ||
            sanctuary.followersAvailableText?.text != "3,845" ||
            sanctuary.assignmentText?.text != "1,280" ||
            sanctuary.multiplierText?.text != "×6.367" ||
            sanctuary.arrivalText?.text != "68.00 / MIN" ||
            sanctuary.upgradeRefugeButtonText?.text != "MEJORAR REFUGIO\n76 SEGUIDORES")
            throw new InvalidOperationException("Los datos dinámicos del Santuario no coinciden.");
        if (sanctuary.showVeiledThresholdButton == null ||
            !sanctuary.showVeiledThresholdButton.gameObject.activeInHierarchy ||
            sanctuary.showVeiledThresholdButton.interactable)
            throw new InvalidOperationException(
                "UMBRAL debe estar visible y bloqueado antes de 500 de Confianza.");
        Debug.Log("[D2 Sanctuary Capture] STRUCTURE_PASS | STATE_PASS | ROUTE_PASS | 1080x1920 + 720x1280");
    }

    private static void PressOpenAltars()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        if (panel?.civilization1PanelUI?.showAltarsButton == null)
            throw new InvalidOperationException("Falta la ruta del Santuario hacia Altares.");
        panel.civilization1PanelUI.showAltarsButton.onClick.Invoke();
        panel.civilization1PanelUI.altarsPanelUI.SelectAltar(D2AltarSystem.WaxAltarId);
        panel.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void NormalizeAltarsCaptureState()
    {
        D2Civilization1State state = GameState.I?.dimension2?.civilization1;
        if (state == null) throw new InvalidOperationException("Falta el estado QA de Altares.");
        state.followersAvailable = 3845L;
        state.followerArrivalProgress = 0.0;
        D2AltarState wax = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(state, D2AltarSystem.RitualBreadAltarId);
        wax.offeringAmount = 3450.0;
        wax.totalOfferingProduced = 3450.0;
        wax.followersAssigned = 120;
        bread.offeringAmount = 3450.0;
        bread.totalOfferingProduced = 3450.0;
        D2AltarsPanelUI altars = UnityEngine.Object.FindFirstObjectByType<D2AltarsPanelUI>(
            FindObjectsInactive.Include);
        altars?.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateAltarsVisible()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2Civilization1PanelUI sanctuary = panel?.civilization1PanelUI;
        D2AltarsPanelUI altars = sanctuary?.altarsPanelUI;
        if (altars == null || sanctuary.altarsSectionRoot == null ||
            !sanctuary.altarsSectionRoot.activeInHierarchy ||
            sanctuary.refugeSectionRoot.activeInHierarchy)
            throw new InvalidOperationException("Altares no es la única sección visible del Santuario.");
        if (altars.headerFollowersText?.text != "3,845" ||
            altars.headerWaxText?.text != "3,450" ||
            altars.headerBreadText?.text != "3,450" ||
            altars.detailTitleText?.text != "ALTAR DE CERA" ||
            altars.offeringValueText?.text != "3,450.00" ||
            altars.productionPerSecondText?.text != "0.242 / S" ||
            altars.productionPerMinuteText?.text != "(14.50 / MIN)" ||
            altars.followersAvailableText?.text != "3,845" ||
            altars.followersAssignedText?.text != "120" ||
            altars.multiplierText?.text != "×4.834")
            throw new InvalidOperationException(
                "Los datos dinámicos de Altares no coinciden con V4: seguidores=" +
                altars.headerFollowersText?.text + ", cera=" + altars.headerWaxText?.text +
                ", pan=" + altars.headerBreadText?.text + ", titulo=" + altars.detailTitleText?.text +
                ", ofrenda=" + altars.offeringValueText?.text + ", s=" +
                altars.productionPerSecondText?.text + ", min=" +
                altars.productionPerMinuteText?.text + ", disponibles=" +
                altars.followersAvailableText?.text + ", asignados=" +
                altars.followersAssignedText?.text + ", mult=" + altars.multiplierText?.text);
        if (altars.waxCardButton == null || altars.breadCardButton == null ||
            altars.incenseCardButton == null || altars.clothCardButton == null ||
            altars.stoneCardButton == null || altars.stoneCardButton.interactable ||
            altars.SelectedAltarId != D2AltarSystem.WaxAltarId)
            throw new InvalidOperationException("La cuadrícula 3+2 o el estado bloqueado de Piedra no coinciden.");
        Debug.Log("[D2 Altars Capture] STRUCTURE_PASS | STATE_PASS | ACTIONS_PASS | ROUTE_PASS | 1080x1920 + 720x1280");
    }

    private static void PreparePilgrimagesCaptureState()
    {
        GameState gameState = GameState.I;
        if (gameState?.dimension2 == null)
            throw new InvalidOperationException("No existe el estado requerido de Peregrinaciones.");

        D2Civilization1State state = new D2Civilization1State
        {
            initialFollowersGranted = true,
            followersAvailable = 3845,
            totalFollowersReceived = 3845,
            trust = 80.0,
            totalPilgrimagesCompleted = 10,
            shortPilgrimagesCompleted = 5,
            mediumPilgrimagesCompleted = 2,
            longPilgrimagesCompleted = 1,
            novitiateLevel = 2,
            totalAcolytesCreated = 2,
            acolytesAvailable = 2,
            pilgrimageSupportFollowersSelected = 0,
            lastPilgrimageResult = ""
        };
        gameState.dimension2.civilization1 = state;
        D2Civilization1System.EnsureState(state);
        D2AltarState wax = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(state, D2AltarSystem.RitualBreadAltarId);
        wax.offeringAmount = 3450.0;
        wax.totalOfferingProduced = 3450.0;
        wax.followersAssigned = 0;
        bread.offeringAmount = 3450.0;
        bread.totalOfferingProduced = 3450.0;
        bread.followersAssigned = 0;
        D2PresentationRules.EnsurePresentationState(gameState);
        PresentationStateUtility.Acknowledge(
            gameState.dimension2.presentation, "d2.c1.pilgrimages.offline_help");
        gameState.dimension2.civilization1Unlocked = true;
        gameState.dimension2.civilization2Unlocked = false;
        gameState.dimension2.civilization3Unlocked = false;
    }

    private static void PressOpenPilgrimages()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2AltarsPanelUI altars = panel?.civilization1PanelUI?.altarsPanelUI;
        if (altars?.pilgrimagesNavButton == null)
            throw new InvalidOperationException("Falta la ruta real Altares → Peregrinaciones.");
        altars.pilgrimagesNavButton.onClick.Invoke();
        D2PilgrimagesPanelUI pilgrimages = panel.civilization1PanelUI.pilgrimagesPanelUI;
        pilgrimages.SelectPilgrimage(D2PilgrimageSystem.ShortId);
        panel.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidatePilgrimagesActionsAndRestore()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2PilgrimagesPanelUI ui = panel?.civilization1PanelUI?.pilgrimagesPanelUI;
        D2Civilization1State state = GameState.I?.dimension2?.civilization1;
        if (ui == null || state == null)
            throw new InvalidOperationException("Falta la vista o el estado QA de Peregrinaciones.");

        ui.SelectPilgrimage(D2PilgrimageSystem.MediumId);
        if (ui.SelectedPilgrimageId != D2PilgrimageSystem.MediumId ||
            state.activePilgrimage.active)
            throw new InvalidOperationException("Una tarjeta no actuó como selector independiente.");
        ui.SelectPilgrimage(D2PilgrimageSystem.ShortId);

        ui.addSupportButton.onClick.Invoke();
        if (state.pilgrimageSupportFollowersSelected != 1L ||
            ui.supportText?.text != "1 / 4\nSEGUIDORES" ||
            ui.materialBonusText?.text != "+20%")
            throw new InvalidOperationException("El control + de apoyo no alcanzó 1/4 y +20%.");
        ui.removeSupportButton.onClick.Invoke();
        if (state.pilgrimageSupportFollowersSelected != 0L)
            throw new InvalidOperationException("El control − de apoyo no regresó a 0/4.");

        D2AltarState wax = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(state, D2AltarSystem.RitualBreadAltarId);
        bool canStartBefore = D2PilgrimageSystem.CanStart(
            GameState.I, D2PilgrimageSystem.ShortId);
        ui.primaryActionButton.onClick.Invoke();
        if (!state.activePilgrimage.active ||
            state.activePilgrimage.pilgrimageId != D2PilgrimageSystem.ShortId ||
            state.followersAvailable != 3844L ||
            Math.Abs(wax.offeringAmount - 3448.0) > 0.10 ||
            Math.Abs(bread.offeringAmount - 3448.0) > 0.10)
            throw new InvalidOperationException(
                "El CTA no inició Corta con los costes reales: canStart=" + canStartBefore +
                ", selected=" + ui.SelectedPilgrimageId +
                ", active=" + state.activePilgrimage.active +
                ", id=" + state.activePilgrimage.pilgrimageId +
                ", followers=" + state.followersAvailable +
                ", wax=" + wax.offeringAmount + ", bread=" + bread.offeringAmount +
                ", listenerCount=" + ui.primaryActionButton.onClick.GetPersistentEventCount() + ".");

        ui.primaryActionButton.onClick.Invoke();
        if (state.activePilgrimage.active || state.followersAvailable != 3845L ||
            Math.Abs(wax.offeringAmount - 3448.0) > 0.10 ||
            Math.Abs(bread.offeringAmount - 3448.0) > 0.10)
            throw new InvalidOperationException(
                "Cancelar no devolvió Seguidores o devolvió Ofrendas indebidamente.");

        wax.offeringAmount = wax.totalOfferingProduced = 3450.0;
        bread.offeringAmount = bread.totalOfferingProduced = 3450.0;
        state.lastPilgrimageResult = "";
        state.pilgrimageSupportFollowersSelected = 0L;
        ui.SelectPilgrimage(D2PilgrimageSystem.ShortId);
        ui.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidatePilgrimagesVisible()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2Civilization1PanelUI sanctuary = panel?.civilization1PanelUI;
        D2PilgrimagesPanelUI ui = sanctuary?.pilgrimagesPanelUI;
        if (ui == null || sanctuary.pilgrimagesSectionRoot == null ||
            !sanctuary.pilgrimagesSectionRoot.activeInHierarchy ||
            sanctuary.refugeSectionRoot.activeInHierarchy ||
            sanctuary.altarsSectionRoot.activeInHierarchy)
            throw new InvalidOperationException(
                "Peregrinaciones no es la única sección visible del Santuario.");

        if (ui.headerTrustText?.text != "80 / 500" ||
            ui.headerFollowersText?.text != "3,845" ||
            ui.headerWaxText?.text != "3,450" ||
            ui.headerBreadText?.text != "3,450" ||
            ui.SelectedPilgrimageId != D2PilgrimageSystem.ShortId ||
            ui.supportText?.text != "0 / 4\nSEGUIDORES" ||
            ui.materialBonusText?.text != "+0%" ||
            ui.activePilgrimageText?.text != "NO HAY UNA\nPEREGRINACIÓN ACTIVA" ||
            ui.primaryActionButtonText?.text != "INICIAR\nPEREGRINACIÓN CORTA")
            throw new InvalidOperationException(
                "Los datos dinámicos de Peregrinaciones no coinciden con V4.");

        if (ui.shortSelectionOverlay == null || !ui.shortSelectionOverlay.activeInHierarchy ||
            ui.startShortButton == null || !ui.startShortButton.gameObject.activeInHierarchy ||
            ui.startMediumButton == null || !ui.startMediumButton.gameObject.activeInHierarchy ||
            ui.startLongButton == null || !ui.startLongButton.gameObject.activeInHierarchy ||
            ui.startGuidedLongButton == null || !ui.startGuidedLongButton.gameObject.activeInHierarchy ||
            ui.startSacredButton == null || !ui.startSacredButton.gameObject.activeInHierarchy ||
            !ui.startSacredButton.interactable)
            throw new InvalidOperationException(
                "Las cinco tarjetas persistentes o la selección Corta no coinciden con V4.");

        Debug.Log("[D2 Pilgrimages Capture] STRUCTURE_PASS | STATE_PASS | ACTIONS_PASS | ROUTE_PASS | 1080x1920 + 720x1280");
    }

    private static void PrepareNovitiateCaptureState()
    {
        GameState gameState = GameState.I;
        if (gameState?.dimension2 == null)
            throw new InvalidOperationException("No existe el estado requerido de Noviciado.");
        D2Civilization1State state = new D2Civilization1State
        {
            initialFollowersGranted = true,
            followersAvailable = 3845,
            totalFollowersReceived = 3845,
            trust = 80.0,
            totalPilgrimagesCompleted = 10,
            shortPilgrimagesCompleted = 5,
            mediumPilgrimagesCompleted = 2,
            novitiateLevel = 2,
            totalAcolytesCreated = 1280,
            acolytesAvailable = 1280,
            novitiateBatchesCompleted = 1,
            novitiateSupportFollowersSelected = 0,
            lastNovitiateResult = "",
            activeNovitiateTraining = new D2NovitiateTrainingState()
        };
        gameState.dimension2.civilization1 = state;
        D2Civilization1System.EnsureState(state);
        D2AltarState wax = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(state, D2AltarSystem.RitualBreadAltarId);
        wax.offeringAmount = wax.totalOfferingProduced = 3450.0;
        bread.offeringAmount = bread.totalOfferingProduced = 3450.0;
        gameState.dimension2.civilization1Unlocked = true;
        gameState.dimension2.civilization2Unlocked = false;
        gameState.dimension2.civilization3Unlocked = false;
    }

    private static void PressOpenNovitiate()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2PilgrimagesPanelUI pilgrimages = panel?.civilization1PanelUI?.pilgrimagesPanelUI;
        if (pilgrimages?.novitiateNavButton == null)
            throw new InvalidOperationException("Falta la ruta Peregrinaciones → Noviciado.");
        pilgrimages.novitiateNavButton.onClick.Invoke();
        panel.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateNovitiateActionsAndRestore()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2NovitiatePanelUI ui = panel?.civilization1PanelUI?.novitiatePanelUI;
        D2Civilization1State state = GameState.I?.dimension2?.civilization1;
        if (ui == null || state == null)
            throw new InvalidOperationException("Falta la vista o el estado QA de Noviciado.");
        D2AltarState wax = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(state, D2AltarSystem.RitualBreadAltarId);

        ui.addSupportButton.onClick.Invoke();
        if (state.novitiateSupportFollowersSelected != 1L || ui.supportText?.text != "1 / 4\nSEGUIDORES")
            throw new InvalidOperationException("El control + de Noviciado no alcanzó 1/4.");
        ui.removeSupportButton.onClick.Invoke();
        if (state.novitiateSupportFollowersSelected != 0L)
            throw new InvalidOperationException("El control − de Noviciado no regresó a 0/4.");

        ui.startTrainingButton.onClick.Invoke();
        if (!state.activeNovitiateTraining.active || state.followersAvailable != 3835L ||
            Math.Abs(wax.offeringAmount - 3428.0) > 0.01 ||
            Math.Abs(bread.offeringAmount - 3428.0) > 0.01 ||
            state.activeNovitiateTraining.trainingLevel != 2 ||
            state.activeNovitiateTraining.acolytesToCreate != 2L)
            throw new InvalidOperationException("FORMAR TANDA no aplicó el estado real de Nivel II.");
        ui.cancelTrainingButton.onClick.Invoke();
        if (state.activeNovitiateTraining.active || state.followersAvailable != 3845L ||
            Math.Abs(wax.offeringAmount - 3428.0) > 0.01)
            throw new InvalidOperationException("CANCELAR no devolvió Seguidores o devolvió Ofrendas.");

        wax.offeringAmount = wax.totalOfferingProduced = 3450.0;
        bread.offeringAmount = bread.totalOfferingProduced = 3450.0;
        state.lastNovitiateResult = "";
        state.novitiateSupportFollowersSelected = 0L;
        state.activeNovitiateTraining = new D2NovitiateTrainingState();
        ui.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateNovitiateVisible()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2Civilization1PanelUI sanctuary = panel?.civilization1PanelUI;
        D2NovitiatePanelUI ui = sanctuary?.novitiatePanelUI;
        if (ui == null || sanctuary.novitiateSectionRoot == null ||
            !sanctuary.novitiateSectionRoot.activeInHierarchy ||
            sanctuary.refugeSectionRoot.activeInHierarchy || sanctuary.altarsSectionRoot.activeInHierarchy ||
            sanctuary.pilgrimagesSectionRoot.activeInHierarchy)
            throw new InvalidOperationException("Noviciado no es la única sección visible del Santuario.");
        if (ui.headerFollowersText?.text != "3,845" || ui.headerAcolytesText?.text != "1,280" ||
            ui.headerPresentText?.text != "1,280" || ui.headerWaxText?.text != "3,450" ||
            ui.headerBreadText?.text != "3,450" || ui.levelText?.text != "NIVEL II" ||
            ui.batchAcolytesText?.text != "2\nACÓLITOS" || ui.batchDurationText?.text != "06:00" ||
            ui.batchFollowerCostText?.text != "10 SEGUIDORES" || ui.batchWaxCostText?.text != "22 CERA" ||
            ui.batchBreadCostText?.text != "22 PAN RITUAL" || ui.supportText?.text != "0 / 4\nSEGUIDORES" ||
            ui.activeTrainingText?.text != "NO HAY UNA TANDA EN FORMACIÓN" ||
            ui.startTrainingButtonText?.text != "FORMAR TANDA" ||
            ui.upgradeButtonText?.text != "MEJORAR A NIVEL III" ||
            !ui.startTrainingButton.interactable || ui.cancelTrainingButton.interactable ||
            !ui.upgradeButton.interactable)
            throw new InvalidOperationException("Los datos o estados dinámicos de Noviciado no coinciden con V4.");
        Debug.Log("[D2 Novitiate Capture] STRUCTURE_PASS | STATE_PASS | ACTIONS_PASS | ROUTE_PASS | 1080x1920 + 720x1280");
    }

    private static void PrepareRitesCaptureState()
    {
        GameState gameState = GameState.I;
        if (gameState?.dimension2 == null)
            throw new InvalidOperationException("No existe el estado requerido de Ritos.");
        D2Civilization1State state = new D2Civilization1State
        {
            initialFollowersGranted = true,
            followersAvailable = 3845,
            totalFollowersReceived = 3849,
            trust = 200.0,
            totalPilgrimagesCompleted = 10,
            shortPilgrimagesCompleted = 5,
            mediumPilgrimagesCompleted = 2,
            novitiateLevel = 2,
            totalAcolytesCreated = 1281,
            acolytesAvailable = 1280,
            novitiateBatchesCompleted = 1,
            thirdRiteSlotUnlocked = false
        };
        gameState.dimension2.civilization1 = state;
        D2Civilization1System.EnsureState(state);
        D2RiteState welcome = D2RiteSystem.GetRite(state, D2RiteSystem.WelcomeId);
        welcome.followersAssigned = 4L;
        welcome.acolytesAssigned = 1L;
        D2AltarState wax = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(state, D2AltarSystem.RitualBreadAltarId);
        wax.offeringAmount = wax.totalOfferingProduced = 3450.0;
        bread.offeringAmount = bread.totalOfferingProduced = 3450.0;
        gameState.dimension2.civilization1Unlocked = true;
        gameState.dimension2.civilization2Unlocked = false;
        gameState.dimension2.civilization3Unlocked = false;
    }

    private static void PressOpenRites()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2NovitiatePanelUI novitiate = panel?.civilization1PanelUI?.novitiatePanelUI;
        if (novitiate?.ritesNavButton == null)
            throw new InvalidOperationException("Falta la ruta Noviciado → Ritos.");
        novitiate.ritesNavButton.onClick.Invoke();
        panel.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateRitesActionsAndRestore()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2RitesPanelUI ui = panel?.civilization1PanelUI?.ritesPanelUI;
        D2Civilization1State state = GameState.I?.dimension2?.civilization1;
        if (ui == null || state == null)
            throw new InvalidOperationException("Falta la vista o el estado QA de Ritos.");
        D2RiteState welcome = D2RiteSystem.GetRite(state, D2RiteSystem.WelcomeId);

        ui.offeringCardButton.onClick.Invoke();
        if (ui.SelectedRiteId != D2RiteSystem.OfferingId ||
            ui.offeringSelectionOverlay == null || !ui.offeringSelectionOverlay.activeInHierarchy)
            throw new InvalidOperationException("La tarjeta Ofrenda no seleccionó su ID/overlay.");
        ui.welcomeCardButton.onClick.Invoke();
        if (ui.SelectedRiteId != D2RiteSystem.WelcomeId)
            throw new InvalidOperationException("No se pudo regresar a Recibimiento.");

        ui.releaseFollowerOneButton.onClick.Invoke();
        if (welcome.followersAssigned != 3L || state.followersAvailable != 3846L)
            throw new InvalidOperationException("Seguidores −1 no conservó el total.");
        ui.assignFollowerOneButton.onClick.Invoke();
        ui.assignFollowerTenButton.onClick.Invoke();
        if (welcome.followersAssigned != 14L || state.followersAvailable != 3835L)
            throw new InvalidOperationException("Seguidores +1/+10 no aplicó cantidades reales.");
        welcome.followersAssigned = 4L;
        state.followersAvailable = 3845L;

        ui.releaseAcolyteOneButton.onClick.Invoke();
        if (welcome.acolytesAssigned != 0L || state.acolytesAvailable != 1281L)
            throw new InvalidOperationException("Acólitos −1 no conservó el total.");
        ui.assignAcolyteOneButton.onClick.Invoke();
        ui.assignAcolyteFiveButton.onClick.Invoke();
        if (welcome.acolytesAssigned != 6L || state.acolytesAvailable != 1275L)
            throw new InvalidOperationException("Acólitos +1/+5 no aplicó cantidades reales.");
        welcome.acolytesAssigned = 1L;
        state.acolytesAvailable = 1280L;

        ui.Refresh();
        ui.releaseAllButton.onClick.Invoke();
        if (welcome.followersAssigned != 0L || welcome.acolytesAssigned != 0L ||
            state.followersAvailable != 3849L || state.acolytesAvailable != 1281L)
            throw new InvalidOperationException("LIBERAR TODO no devolvió ambos recursos.");
        welcome.followersAssigned = 4L;
        welcome.acolytesAssigned = 1L;
        state.followersAvailable = 3845L;
        state.acolytesAvailable = 1280L;
        ui.SelectRite(D2RiteSystem.WelcomeId);
        ui.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateRitesVisible()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2Civilization1PanelUI sanctuary = panel?.civilization1PanelUI;
        D2RitesPanelUI ui = sanctuary?.ritesPanelUI;
        if (ui == null || sanctuary.ritesSectionRoot == null ||
            !sanctuary.ritesSectionRoot.activeInHierarchy || sanctuary.refugeSectionRoot.activeInHierarchy ||
            sanctuary.altarsSectionRoot.activeInHierarchy || sanctuary.pilgrimagesSectionRoot.activeInHierarchy ||
            sanctuary.novitiateSectionRoot.activeInHierarchy)
            throw new InvalidOperationException("Ritos no es la única sección visible del Santuario.");
        if (ui.headerFollowersText?.text != "3,845" || ui.headerAcolytesText?.text != "1,280" ||
            ui.headerWaxText?.text != "3,450" || ui.headerBreadText?.text != "3,450" ||
            ui.slotsText?.text != "RITOS ACTIVOS 1 / 2" ||
            ui.SelectedRiteId != D2RiteSystem.WelcomeId ||
            ui.detailTitleText?.text != "RITO DE RECIBIMIENTO" ||
            ui.descriptionText?.text != "AUMENTA LA LLEGADA DE SEGUIDORES" ||
            ui.currentEffectText?.text != "+11%" || ui.limitText?.text != "+50%" ||
            ui.followersAssignedText?.text != "4" || ui.acolytesAssignedText?.text != "1" ||
            ui.unlockThirdSlotButton.interactable || !ui.releaseAllButton.interactable)
            throw new InvalidOperationException("Los datos o estados dinámicos de Ritos no coinciden con V4.");
        if (ui.welcomeCardButton == null || ui.offeringCardButton == null ||
            ui.pathCardButton == null || ui.novitiateCardButton == null || ui.respectCardButton == null ||
            !ui.welcomeCardButton.gameObject.activeInHierarchy ||
            !ui.respectCardButton.gameObject.activeInHierarchy ||
            (ui.welcomeNeutralOverlay != null && ui.welcomeNeutralOverlay.activeInHierarchy))
            throw new InvalidOperationException("Las cinco tarjetas o la selección Recibimiento no coinciden con V4.");
        Debug.Log("[D2 Rites Capture] STRUCTURE_PASS | STATE_PASS | ACTIONS_PASS | ROUTE_PASS | 1080x1920 + 720x1280");
    }

    private static void PreparePactsCaptureState()
    {
        GameState gameState = GameState.I;
        if (gameState?.dimension2 == null)
            throw new InvalidOperationException("No existe el estado requerido de Pactos.");
        D2Civilization1State state = new D2Civilization1State
        {
            initialFollowersGranted = true,
            followersAvailable = 3845,
            totalFollowersReceived = 3845,
            trust = 280.0,
            totalPilgrimagesCompleted = 10,
            shortPilgrimagesCompleted = 5,
            mediumPilgrimagesCompleted = 2,
            novitiateLevel = 2,
            totalAcolytesCreated = 1280,
            acolytesAvailable = 1280,
            novitiateBatchesCompleted = 1,
            secondCivilizationPactSlotUnlocked = false
        };
        gameState.dimension2.civilization1 = state;
        D2Civilization1System.EnsureState(state);
        D2AltarState wax = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(state, D2AltarSystem.RitualBreadAltarId);
        wax.offeringAmount = wax.totalOfferingProduced = 3510.0;
        bread.offeringAmount = bread.totalOfferingProduced = 3540.0;
        if (!D2CivilizationPactSystem.TryActivate(gameState,
                D2CivilizationPactSystem.HospitalityId))
            throw new InvalidOperationException("No se pudo preparar Hospedaje activo.");
        gameState.dimension2.civilization1Unlocked = true;
        gameState.dimension2.civilization2Unlocked = false;
        gameState.dimension2.civilization3Unlocked = false;
    }

    private static void PressOpenPacts()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2RitesPanelUI rites = panel?.civilization1PanelUI?.ritesPanelUI;
        if (rites?.pactsNavButton == null)
            throw new InvalidOperationException("Falta la ruta Ritos → Pactos.");
        ClickButton(rites.pactsNavButton);
        panel.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidatePactsActionsAndRestore()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2CivilizationPactsPanelUI ui = panel?.civilization1PanelUI?.pactsPanelUI;
        D2Civilization1State state = GameState.I?.dimension2?.civilization1;
        if (ui == null || state == null)
            throw new InvalidOperationException("Falta la vista o el estado QA de Pactos.");
        D2AltarState wax = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(state, D2AltarSystem.RitualBreadAltarId);

        ClickButton(ui.openPathCardButton);
        if (ui.SelectedPactId != D2CivilizationPactSystem.OpenPathId ||
            ui.openPathSelectionOverlay == null || !ui.openPathSelectionOverlay.activeInHierarchy ||
            ui.hospitalityNeutralOverlay == null || !ui.hospitalityNeutralOverlay.activeInHierarchy)
            throw new InvalidOperationException("Camino Abierto no seleccionó su ID/overlay real.");
        ClickButton(ui.hospitalityCardButton);
        if (ui.SelectedPactId != D2CivilizationPactSystem.HospitalityId)
            throw new InvalidOperationException("No se pudo regresar a Hospedaje.");

        wax.offeringAmount = 3450.0;
        bread.offeringAmount = 3450.0;
        ClickButton(ui.cancelButton);
        if (D2CivilizationPactSystem.GetPact(state,
                D2CivilizationPactSystem.HospitalityId).active ||
            Math.Abs(wax.offeringAmount - 3450.0) > 0.001 ||
            Math.Abs(bread.offeringAmount - 3450.0) > 0.001)
            throw new InvalidOperationException("CANCELAR PACTO devolvió recursos o no desactivó Hospedaje.");

        wax.offeringAmount = 3510.0;
        bread.offeringAmount = 3540.0;
        ui.Refresh();
        ClickButton(ui.activateButton);
        if (!D2CivilizationPactSystem.GetPact(state,
                D2CivilizationPactSystem.HospitalityId).active ||
            Math.Abs(wax.offeringAmount - 3450.0) > 0.001 ||
            Math.Abs(bread.offeringAmount - 3450.0) > 0.001)
            throw new InvalidOperationException("ACTIVAR PACTO no descontó 60 Cera y 90 Pan.");

        ClickButton(ui.openPathCardButton);
        double waxBeforeRejectedActivation = wax.offeringAmount;
        double breadBeforeRejectedActivation = bread.offeringAmount;
        ClickButton(ui.activateButton);
        if (D2CivilizationPactSystem.GetPact(state,
                D2CivilizationPactSystem.OpenPathId).active ||
            Math.Abs(wax.offeringAmount - waxBeforeRejectedActivation) > 0.001 ||
            Math.Abs(bread.offeringAmount - breadBeforeRejectedActivation) > 0.001)
            throw new InvalidOperationException("El segundo Pacto burló el límite real de una ranura.");

        state.trust = 400.0;
        state.novitiateLevel = 4;
        state.acolytesAvailable = 1280L;
        wax.offeringAmount = 3750.0;
        bread.offeringAmount = 3750.0;
        ui.Refresh();
        ClickButton(ui.unlockSecondSlotButton);
        if (!state.secondCivilizationPactSlotUnlocked ||
            ui.secondSlotUnlockedOverlay == null || !ui.secondSlotUnlockedOverlay.activeInHierarchy)
            throw new InvalidOperationException("El segundo espacio no cambió a su estado desbloqueado.");
        ClickButton(ui.activateButton);
        if (!D2CivilizationPactSystem.GetPact(state,
                D2CivilizationPactSystem.OpenPathId).active ||
            ui.secondActivePactIcon == null || !ui.secondActivePactIcon.gameObject.activeInHierarchy)
            throw new InvalidOperationException(
                "El círculo vacío no recibió el medallón del segundo Pacto activo.");

        D2CivilizationPactSystem.GetPact(state,
            D2CivilizationPactSystem.OpenPathId).active = false;
        state.secondCivilizationPactSlotUnlocked = false;
        state.trust = 280.0;
        state.novitiateLevel = 2;
        state.acolytesAvailable = 1280L;

        ClickButton(ui.hospitalityCardButton);
        wax.offeringAmount = wax.totalOfferingProduced = 3450.0;
        bread.offeringAmount = bread.totalOfferingProduced = 3450.0;
        state.lastCivilizationPactResult = "";
        ui.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidatePactsVisible()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2Civilization1PanelUI sanctuary = panel?.civilization1PanelUI;
        D2CivilizationPactsPanelUI ui = sanctuary?.pactsPanelUI;
        if (ui == null || sanctuary.pactsSectionRoot == null ||
            !sanctuary.pactsSectionRoot.activeInHierarchy || sanctuary.refugeSectionRoot.activeInHierarchy ||
            sanctuary.altarsSectionRoot.activeInHierarchy || sanctuary.pilgrimagesSectionRoot.activeInHierarchy ||
            sanctuary.novitiateSectionRoot.activeInHierarchy || sanctuary.ritesSectionRoot.activeInHierarchy)
            throw new InvalidOperationException("Pactos no es la única sección visible del Santuario.");
        if (ui.headerTrustText?.text != "280 / 500" || ui.headerAcolytesText?.text != "1,280" ||
            ui.headerWaxText?.text != "3,450" || ui.headerBreadText?.text != "3,450" ||
            ui.slotsText?.text != "PACTOS ACTIVOS 1/2" ||
            ui.SelectedPactId != D2CivilizationPactSystem.HospitalityId ||
            ui.detailTitleText?.text != "PACTO DE HOSPEDAJE – ACTIVO" ||
            ui.benefitValueText?.text != "+35% LLEGADA DE SEGUIDORES" ||
            ui.commitmentValueText?.text != "CONSUME 1 PAN RITUAL POR MINUTO" ||
            ui.stateValueText?.text != "ACTIVO" || ui.activateButton.interactable ||
            !ui.cancelButton.interactable || ui.unlockSecondSlotButton.interactable)
            throw new InvalidOperationException("Los datos o estados dinámicos de Pactos no coinciden con V4.");
        if (ui.hospitalityCardButton == null || ui.openPathCardButton == null ||
            ui.consecrationCardButton == null || ui.silentVowCardButton == null ||
            ui.innerDoorCardButton == null ||
            !ui.hospitalityCardButton.gameObject.activeInHierarchy ||
            !ui.innerDoorCardButton.gameObject.activeInHierarchy ||
            (ui.hospitalityNeutralOverlay != null && ui.hospitalityNeutralOverlay.activeInHierarchy) ||
            (ui.hospitalitySlotNeutralOverlay != null && ui.hospitalitySlotNeutralOverlay.activeInHierarchy) ||
            (ui.secondSlotUnlockedOverlay != null && ui.secondSlotUnlockedOverlay.activeInHierarchy) ||
            ui.lockRequirementIconsOverlay == null || !ui.lockRequirementIconsOverlay.activeInHierarchy ||
            (ui.secondActivePactIcon != null && ui.secondActivePactIcon.gameObject.activeInHierarchy))
            throw new InvalidOperationException("Las cinco tarjetas, ranuras o selección de Pactos no coinciden con V4.");
        Debug.Log("[D2 Pacts Capture] STRUCTURE_PASS | STATE_PASS | ACTIONS_PASS | ROUTE_PASS | 1080x1920 + 720x1280");
    }

    private static void PrepareBondCaptureState()
    {
        GameState gameState = GameState.I;
        D2Civilization1State state = gameState?.dimension2?.civilization1;
        if (state == null)
            throw new InvalidOperationException("No existe el estado requerido del Lugar de Vínculo.");
        state.trust = D2VeiledThresholdSystem.UnlockTrustRequired;
        state.entityContactAvailable = true;
        state.bondPlacePrepared = true;
        state.acolytesAvailable = 1280L;
        state.acolytesAssignedToBond = 4L;
        state.bondProgress = 45.0;
        D2BondSystem.EnsureState(state);
        foreach (D2BondLineState line in state.bondLines)
            line.level = 1;
        SetAdvancedOffering(state, D2AltarSystem.IncenseAltarId, 300.0);
        SetAdvancedOffering(state, D2AltarSystem.SacredClothAltarId, 300.0);
        SetAdvancedOffering(state, D2AltarSystem.CarvedStoneAltarId, 300.0);
        state.lastBondResult = "";
        gameState.dimension2.civilization1Unlocked = true;
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        panel?.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void PressOpenBond()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2CivilizationPactsPanelUI pacts = panel?.civilization1PanelUI?.pactsPanelUI;
        if (pacts?.thresholdNavButton == null)
            throw new InvalidOperationException("Falta la ruta Pactos → Pacto/Lugar de Vínculo.");
        ClickButton(pacts.thresholdNavButton);
        panel.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateBondActionsAndRestore()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2VeiledThresholdPanelUI ui = panel?.civilization1PanelUI?.veiledThresholdPanelUI;
        D2Civilization1State state = GameState.I?.dimension2?.civilization1;
        if (ui == null || state == null || ui.lineButtons == null || ui.lineButtons.Length != 5)
            throw new InvalidOperationException("Falta la vista, el estado o las cinco líneas del vínculo.");

        ClickButton(ui.lineButtons[1]);
        if (ui.SelectedLineId != D2BondSystem.SacredCraftId ||
            ui.detailTitleText?.text != "OFICIO SAGRADO")
            throw new InvalidOperationException("Oficio Sagrado no seleccionó su ID real.");

        ClickButton(ui.releaseAcolyteButton);
        if (state.acolytesAssignedToBond != 3L || state.acolytesAvailable != 1281L)
            throw new InvalidOperationException("Acólitos −1 no conservó el total del vínculo.");
        ClickButton(ui.assignAcolyteButton);
        if (state.acolytesAssignedToBond != 4L || state.acolytesAvailable != 1280L)
            throw new InvalidOperationException("Acólitos +1 no restauró el total del vínculo.");

        double progressBeforeUpgrade = state.bondProgress;
        ClickButton(ui.upgradeLineButton);
        if (D2BondSystem.GetLevel(state, D2BondSystem.SacredCraftId) != 2 ||
            Math.Abs((progressBeforeUpgrade - state.bondProgress) - 40.0) > 0.01 ||
            Math.Abs(GetOffering(state, D2AltarSystem.IncenseAltarId) - 250.0) > 0.001 ||
            Math.Abs(GetOffering(state, D2AltarSystem.SacredClothAltarId) - 250.0) > 0.001 ||
            Math.Abs(GetOffering(state, D2AltarSystem.CarvedStoneAltarId) - 250.0) > 0.001)
            throw new InvalidOperationException(
                "MEJORAR LÍNEA no aplicó nivel y costes reales: nivel=" +
                D2BondSystem.GetLevel(state, D2BondSystem.SacredCraftId) +
                ", delta=" + (progressBeforeUpgrade - state.bondProgress).ToString("F4") +
                ", incienso=" + GetOffering(state, D2AltarSystem.IncenseAltarId).ToString("F3") +
                ", tela=" + GetOffering(state, D2AltarSystem.SacredClothAltarId).ToString("F3") +
                ", piedra=" + GetOffering(state, D2AltarSystem.CarvedStoneAltarId).ToString("F3") +
                ", interactable=" + ui.upgradeLineButton.interactable + ".");

        foreach (D2BondLineState line in state.bondLines)
            line.level = 1;
        state.bondProgress = 45.0;
        SetAdvancedOffering(state, D2AltarSystem.IncenseAltarId, 300.0);
        SetAdvancedOffering(state, D2AltarSystem.SacredClothAltarId, 300.0);
        SetAdvancedOffering(state, D2AltarSystem.CarvedStoneAltarId, 300.0);
        ui.SelectLine(D2BondSystem.PilgrimPathId);

        ClickButton(ui.pactsNavButton);
        if (!panel.civilization1PanelUI.pactsSectionRoot.activeInHierarchy)
            throw new InvalidOperationException("La ruta Pacto → Pactos no abrió su destino.");
        ClickButton(panel.civilization1PanelUI.pactsPanelUI.thresholdNavButton);
        if (!panel.civilization1PanelUI.veiledThresholdSectionRoot.activeInHierarchy)
            throw new InvalidOperationException("La ruta Pactos → Pacto no restauró la pantalla.");
        ui.Refresh();
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateBondVisible()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        D2Civilization1PanelUI sanctuary = panel?.civilization1PanelUI;
        D2VeiledThresholdPanelUI ui = sanctuary?.veiledThresholdPanelUI;
        if (ui == null || sanctuary.veiledThresholdSectionRoot == null ||
            !sanctuary.veiledThresholdSectionRoot.activeInHierarchy ||
            sanctuary.refugeSectionRoot.activeInHierarchy || sanctuary.altarsSectionRoot.activeInHierarchy ||
            sanctuary.pilgrimagesSectionRoot.activeInHierarchy || sanctuary.novitiateSectionRoot.activeInHierarchy ||
            sanctuary.ritesSectionRoot.activeInHierarchy || sanctuary.pactsSectionRoot.activeInHierarchy)
            throw new InvalidOperationException("Pacto/Lugar de Vínculo no es la única sección visible.");
        if (ui.titleText?.text != "PACTO — LUGAR DE VÍNCULO" ||
            ui.incenseValueText?.text != "300" || ui.sacredClothValueText?.text != "300" ||
            ui.carvedStoneValueText?.text != "300" || ui.progressValueText?.text != "45" ||
            ui.SelectedLineId != D2BondSystem.PilgrimPathId ||
            ui.detailTitleText?.text != "CAMINO PEREGRINO" ||
            ui.detailLevelText?.text != "NIVEL 1 / 3" ||
            ui.progressCostText?.text != "40 PROGRESO" ||
            ui.incenseCostText?.text != "50 INCIENSO" ||
            ui.availableAcolytesValueText?.text != "1,280" ||
            ui.assignedAcolytesValueText?.text != "4" ||
            ui.revelationText?.text != "PACTO ESTABLECIDO" ||
            ui.placeText?.text != "PREPARACIÓN COMPLETADA" ||
            ui.thresholdTabLabelText?.text != "PACTO" || !ui.upgradeLineButton.interactable)
            throw new InvalidOperationException("Los datos o estados dinámicos del Lugar de Vínculo no coinciden con V4.");
        if (ui.lineButtons == null || ui.lineButtons.Length != 5 ||
            ui.lineNameTexts == null || ui.lineNameTexts.Length != 5 ||
            ui.lineLevelTexts == null || ui.lineLevelTexts.Length != 5)
            throw new InvalidOperationException("La matriz de cinco líneas del vínculo está incompleta.");
        for (int index = 0; index < 5; index++)
        {
            if (ui.lineButtons[index] == null || !ui.lineButtons[index].gameObject.activeInHierarchy ||
                ui.lineLevelTexts[index]?.text != "NIVEL 1 / 3")
                throw new InvalidOperationException("Una línea del vínculo no conserva su estado visible.");
        }
        Debug.Log("[D2 Bond Capture] STRUCTURE_PASS | STATE_PASS | ACTIONS_PASS | ROUTE_PASS | 1080x1920 + 720x1280");
    }

    private static void SetAdvancedOffering(D2Civilization1State state, string altarId, double amount)
    {
        D2AltarState altar = D2AltarSystem.GetAltar(state, altarId);
        if (altar == null) throw new InvalidOperationException("Falta la Ofrenda avanzada " + altarId + ".");
        altar.unlocked = true;
        altar.offeringAmount = amount;
        altar.totalOfferingProduced = Math.Max(altar.totalOfferingProduced, amount);
    }

    private static double GetOffering(D2Civilization1State state, string altarId)
    {
        return D2AltarSystem.GetAltar(state, altarId)?.offeringAmount ?? 0.0;
    }

    private static void ClickButton(Button button)
    {
        if (button == null || EventSystem.current == null || !button.gameObject.activeInHierarchy)
            throw new InvalidOperationException("No se puede ejecutar el botón físico de Pactos.");
        PointerEventData data = new PointerEventData(EventSystem.current)
        {
            button = PointerEventData.InputButton.Left
        };
        ExecuteEvents.Execute(button.gameObject, data, ExecuteEvents.pointerClickHandler);
        Canvas.ForceUpdateCanvases();
    }

    private static void SetButtonHover(bool hovering)
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include
        );
        if (panel?.continueFirstEntryButton == null || EventSystem.current == null)
            throw new InvalidOperationException("No se puede simular el cursor sobre ABRIR MAPA.");

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        if (hovering)
        {
            ExecuteEvents.Execute(
                panel.continueFirstEntryButton.gameObject,
                eventData,
                ExecuteEvents.pointerEnterHandler
            );
        }
        else
        {
            ExecuteEvents.Execute(
                panel.continueFirstEntryButton.gameObject,
                eventData,
                ExecuteEvents.pointerExitHandler
            );
        }
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateHoverBackground(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        Texture2D image = new Texture2D(2, 2, TextureFormat.RGB24, false);
        try
        {
            if (!image.LoadImage(bytes))
                throw new InvalidOperationException("No se pudo leer la captura hover.");
            Color sample = image.GetPixel(540, 1920 - 1540);
            float luminance = sample.r * 0.299f + sample.g * 0.587f + sample.b * 0.114f;
            if (luminance > 0.42f)
                throw new InvalidOperationException(
                    "El cursor reveló el fondo neutro de la placa ABRIR MAPA."
                );
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(image);
        }
    }

    private static void ValidateTransition()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include
        );
        if (panel == null || panel.mapRoot == null || !panel.mapRoot.activeSelf ||
            panel.firstEntryRoot == null || panel.firstEntryRoot.activeSelf)
            throw new InvalidOperationException("ABRIR MAPA no realizó la transición real.");
        if (!Dimension2System.HasSeenCurrentFirstEntry(GameState.I))
            throw new InvalidOperationException(
                "ABRIR MAPA no registró la versión vigente de la primera entrada.");
    }

    private static void ValidatePactMapVisible()
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include);
        if (panel?.mapRoot == null || panel.mapRoot.name != "D2_PactMap" ||
            !panel.mapRoot.activeInHierarchy)
            throw new InvalidOperationException("D2_PactMap no está visible.");
        RectTransform root = panel.mapRoot.GetComponent<RectTransform>();
        if (root == null || Mathf.Abs(root.rect.width - 1080f) > 1f ||
            Mathf.Abs(root.rect.height - 1920f) > 1f)
            throw new InvalidOperationException("D2_PactMap no conserva 1080x1920.");
        if (panel.mapFollowersText?.text != "3,845" ||
            panel.mapTrustText?.text != "280 / 500")
            throw new InvalidOperationException(
                "Los recursos del mapa no corresponden al estado real: seguidores='" +
                panel.mapFollowersText?.text + "', confianza='" + panel.mapTrustText?.text + "'.");
        if (panel.civilization1StateText?.text != "DISPONIBLE" ||
            panel.civilization2StateText?.text != "BLOQUEADO" ||
            panel.civilization3StateText?.text != "BLOQUEADO")
            throw new InvalidOperationException("Los estados de territorios no coinciden.");
        if (panel.civilization3Button == null || !panel.civilization3Button.gameObject.activeInHierarchy)
            throw new InvalidOperationException("Ruinas Sepultadas debe permanecer visible bloqueada.");
        Canvas canvas = panel.mapRoot.GetComponent<Canvas>();
        if (canvas == null || !canvas.overrideSorting)
            throw new InvalidOperationException("D2_PactMap no está en primer plano.");
        Debug.Log("[D2 Pact Map Capture] STRUCTURE_PASS | STATE_PASS | 1080x1920 + 720x1280");
    }

    private static void ActivateAncestors(Transform transform)
    {
        Transform current = transform;
        while (current != null)
        {
            current.gameObject.SetActive(true);
            current = current.parent;
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
            : UnityEngine.Object.FindFirstObjectByType<Camera>();
        if (camera == null)
            throw new InvalidOperationException("No hay cámara para captura.");
        Canvas[] canvases = UnityEngine.Object.FindObjectsByType<Canvas>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None
        );
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
            Canvas.ForceUpdateCanvases();
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

    private static Transform FindChild(Transform root, string name)
    {
        if (root == null) return null;
        foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
            if (transform.name == name) return transform;
        return null;
    }
}
#endif

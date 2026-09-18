#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class VerticalUiBlock7Capture
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ActiveKey = "QF.VerticalUiBlock7.Active";
    private const string StageKey = "QF.VerticalUiBlock7.Stage";
    private const string FramesKey = "QF.VerticalUiBlock7.Frames";
    private const string FailedKey = "QF.VerticalUiBlock7.Failed";
    private const string AdvancedOnlyKey = "QF.VerticalUiBlock7.AdvancedOnly";
    private const string UpgradesOnlyKey = "QF.VerticalUiBlock7.UpgradesOnly";
    private const string SaveExistedKey = "QF.VerticalUiBlock7.SaveExisted";
    private const string SaveBackupExistedKey = "QF.VerticalUiBlock7.SaveBackupExisted";

    private static string OutputDirectory => Path.GetFullPath("Logs/VisualQA/VerticalUIBlock7");
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");
    private static string SaveBackupPath => SavePath + ".bak";
    private static string SessionSaveCopy => Path.Combine(OutputDirectory, ".save.json.session-backup");
    private static string SessionSaveBackupCopy => Path.Combine(OutputDirectory, ".save.json.bak.session-backup");
    private static double stageStartedAt;

    [InitializeOnLoadMethod]
    private static void ResumeAfterDomainReload()
    {
        if (!SessionState.GetBool(ActiveKey, false))
            return;

        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        if (EditorApplication.isPlaying)
        {
            stageStartedAt = EditorApplication.timeSinceStartup;
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
        }
    }

    [MenuItem("Tools/Quantum Forge/Vertical UI/Block 7/Capture Play Mode Evidence")]
    public static void Run()
    {
        StartCapture(false);
    }

    [MenuItem("Tools/Quantum Forge/Vertical UI/Block 7/Capture Advanced 1080x1920 Only")]
    public static void RunAdvancedOnly()
    {
        StartCapture(true);
    }

    [MenuItem("Tools/Quantum Forge/Vertical UI/Block 7/Capture Polished Upgrades Only")]
    public static void RunUpgradesOnly()
    {
        StartCapture(false, true);
    }

    private static void StartCapture(bool advancedOnly, bool upgradesOnly = false)
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            throw new InvalidOperationException("La captura del Bloque 7 debe iniciarse fuera de Play Mode.");

        Directory.CreateDirectory(OutputDirectory);
        RecoverStaleSessionBackupIfNecessary();
        if (!advancedOnly && !upgradesOnly)
            foreach (string capture in Directory.GetFiles(OutputDirectory, "*.png"))
                File.Delete(capture);
        BackupUserSave();

        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetBool(FailedKey, false);
        SessionState.SetBool(AdvancedOnlyKey, advancedOnly);
        SessionState.SetBool(UpgradesOnlyKey, upgradesOnly);
        SessionState.SetInt(StageKey, 0);
        SessionState.SetInt(FramesKey, 0);

        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        EditorApplication.isPlaying = true;
    }

    private static void OnPlayModeChanged(PlayModeStateChange change)
    {
        if (change == PlayModeStateChange.EnteredPlayMode)
        {
            stageStartedAt = EditorApplication.timeSinceStartup;
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
            return;
        }

        if (change != PlayModeStateChange.EnteredEditMode)
            return;

        EditorApplication.update -= Tick;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;

        bool failed = SessionState.GetBool(FailedKey, false);
        try
        {
            RestoreUserSave();
        }
        catch (Exception exception)
        {
            failed = true;
            Debug.LogException(exception);
        }
        finally
        {
            SessionState.SetBool(ActiveKey, false);
            SessionState.SetBool(FailedKey, false);
            SessionState.SetBool(AdvancedOnlyKey, false);
            SessionState.SetBool(UpgradesOnlyKey, false);
        }

        if (!failed)
        {
            Debug.Log("[Vertical UI Block 7 Capture] PASS | Play Mode | compras reales | " +
                "tutorial | circuitos | save/load | idiomas | navegacion progresiva | " +
                "1080x1920, 720x1280, 9:19.5 y Safe Area simulada | guardado restaurado");
        }

        EditorApplication.Exit(failed ? 1 : 0);
    }

    private static void Tick()
    {
        int frames = SessionState.GetInt(FramesKey, 0) + 1;
        SessionState.SetInt(FramesKey, frames);
        if (frames < 5 || EditorApplication.timeSinceStartup - stageStartedAt < 1.20)
            return;

        try
        {
            if (SessionState.GetBool(UpgradesOnlyKey, false))
            {
                TickUpgradesOnly();
                return;
            }
            if (SessionState.GetBool(AdvancedOnlyKey, false))
            {
                TickAdvancedOnly();
                return;
            }
            int stage = SessionState.GetInt(StageKey, 0);
            switch (stage)
            {
                case 0:
                    PrepareEarlyGeneration();
                    PrepareResolution(1080, 1920);
                    Advance(1);
                    break;
                case 1:
                    ValidateEarlyPresentation();
                    Capture("01_generation_initial_es_1080x1920.png", 1080, 1920);
                    Capture("01b_generation_initial_es_720x1280.png", 720, 1280);
                    Require(TabsUI.Instance != null, "TabsUI no esta disponible.");
                    TabsUI.Instance.ShowMejoras();
                    PrepareResolution(1080, 2340);
                    Advance(2);
                    break;
                case 2:
                    Capture("02_upgrades_early_es_1080x2340.png", 1080, 2340);
                    BuyEarlyUpgradeAndRemainingVertices();
                    TabsUI.Instance.ShowMejoras();
                    Advance(3);
                    break;
                case 3:
                    BuyTriangleUnlockFromUi();
                    PrepareResolution(1080, 1920);
                    Advance(4);
                    break;
                case 4:
                    ValidateAndCaptureTriangleTutorial();
                    TabsUI.Instance.ShowMejoras();
                    PrepareResolution(1080, 2340);
                    Advance(5);
                    break;
                case 5:
                    ValidateAdvancedUpgradesPresentation();
                    Capture("03_upgrades_triangle_es_1080x2340.png", 1080, 2340);
                    PrepareAdvancedGenerationEnglish();
                    Advance(6);
                    break;
                case 6:
                    Capture("04_generation_energy_en_1080x2340.png", 1080, 2340);
                    PrepareEnergyFocusSpanish();
                    PrepareResolution(1080, 1920);
                    Advance(7);
                    break;
                case 7:
                    ValidateEnergyFocusAvailable();
                    Capture("05_generation_energy_focus_es_1080x1920.png", 1080, 1920);
                    Capture("05b_generation_energy_focus_es_720x1280.png", 720, 1280);
                    PrepareProgressiveNavigation();
                    PrepareResolution(1080, 1920);
                    Advance(8);
                    break;
                case 8:
                    ValidateProgressiveNavigation();
                    Capture("06b_secondary_navigation_es_1080x1920.png", 1080, 1920);
                    PrepareResolution(720, 1600);
                    Advance(9);
                    break;
                case 9:
                    Capture("06_secondary_navigation_es_720x1600.png", 720, 1600);
                    PrepareResolution(1080, 2340);
                    Advance(10);
                    break;
                case 10:
                    ApplySimulatedSafeArea();
                    Capture("07_safe_area_notch_es_1080x2340.png", 1080, 2340);
                    ValidateSaveAndLoadRoundTrip();
                    Advance(11);
                    break;
                default:
                    EditorApplication.update -= Tick;
                    EditorApplication.isPlaying = false;
                    break;
            }
        }
        catch (Exception exception)
        {
            SessionState.SetBool(FailedKey, true);
            Debug.LogException(exception);
            EditorApplication.update -= Tick;
            EditorApplication.isPlaying = false;
        }
    }

    private static void TickUpgradesOnly()
    {
        int stage = SessionState.GetInt(StageKey, 0);
        switch (stage)
        {
            case 0:
                PrepareEarlyGeneration();
                TabsUI.Instance.ShowMejoras();
                RefreshAllUi();
                Advance(1);
                break;
            case 1:
                BuyEarlyUpgradeAndRemainingVertices();
                TabsUI.Instance.ShowMejoras();
                Advance(2);
                break;
            case 2:
                BuyTriangleUnlockFromUi();
                DismissTriangleTutorialForUpgradesCapture();
                TabsUI.Instance.ShowMejoras();
                PrepareResolution(1080, 1920);
                Advance(3);
                break;
            case 3:
                ValidateAdvancedUpgradesPresentation();
                Capture("08_upgrades_polished_es_1080x1920.png", 1080, 1920);
                PrepareCompletedUpgradesHidden();
                Advance(4);
                break;
            case 4:
                ValidateCompletedUpgradesHidden();
                Capture("09_upgrades_completed_hidden_es_1080x1920.png", 1080, 1920);
                Capture("09b_upgrades_completed_hidden_es_720x1280.png", 720, 1280);
                Advance(5);
                break;
            default:
                EditorApplication.update -= Tick;
                EditorApplication.isPlaying = false;
                break;
        }
    }

    private static void DismissTriangleTutorialForUpgradesCapture()
    {
        TriangleActivationTutorialUI tutorial =
            UnityEngine.Object.FindFirstObjectByType<TriangleActivationTutorialUI>(
                FindObjectsInactive.Include);
        Require(tutorial != null,
            "El tutorial del Triángulo no apareció antes de capturar Mejoras.");
        Button close = tutorial.GetComponentsInChildren<Button>(true)
            .FirstOrDefault(button => button.name == "Close");
        Require(close != null, "El tutorial no tiene cierre accesible.");
        close.onClick.Invoke();
    }

    private static void TickAdvancedOnly()
    {
        int stage = SessionState.GetInt(StageKey, 0);
        switch (stage)
        {
            case 0:
                PrepareAdvancedVisualOnly();
                PrepareResolution(1080, 1920);
                Advance(1);
                break;
            case 1:
                VerticalNavigationUI navigation = TabsUI.Instance.verticalNavigation;
                LayoutRebuilder.ForceRebuildLayoutImmediate(
                    navigation.secondaryScroll.content);
                navigation.secondaryScroll.horizontalNormalizedPosition = 0f;
                VerticalGenerationBeforeTriangleUI generation =
                    UnityEngine.Object.FindFirstObjectByType<
                        VerticalGenerationBeforeTriangleUI>(
                        FindObjectsInactive.Include);
                generation?.RefreshState();
                ScrollRect advancedScroll = generation?.triangleRoot != null
                    ? generation.triangleRoot.transform.Find("TriangleScroll")?
                        .GetComponent<ScrollRect>()
                    : null;
                if (advancedScroll != null)
                    advancedScroll.verticalNormalizedPosition = 1f;
                Canvas.ForceUpdateCanvases();
                Capture("06c_secondary_navigation_corrected_es_1080x1920.png", 1080, 1920);
                Capture("06c_secondary_navigation_corrected_es_720x1280.png", 720, 1280);
                Advance(2);
                break;
            default:
                EditorApplication.update -= Tick;
                EditorApplication.isPlaying = false;
                break;
        }
    }

    private static void PrepareAdvancedVisualOnly()
    {
        PrepareEarlyGeneration();
        GameState state = GameState.I;
        state.LE = 1000000.0;
        state.Traces = 5000.0;
        foreach (string id in BuildingIds)
            Require(BuildingPurchaseService.TryPurchase(state, RequireBuilding(id)),
                "No se pudo preparar el vertice visual " + id + ".");
        state.upgradeStudies.discoveredIds.Add("triangle_unlock_1");
        Require(F2UpgradeManager.I.TryBuy("triangle_unlock_1"),
            "No se pudo preparar Acople para la captura avanzada.");
        Require(state.SetTriangleCircuit(TriangleCircuitType.Phase),
            "No se pudo preparar el enfoque Energía para la captura avanzada.");
        state.experimentalChamberUnlocked = true;
        state.triangleEnergy = 480.0;
        state.dimension01Unlocked = true;
        state.dimension02Unlocked = true;
        state.dimension03Unlocked = true;
        LocalizationManager.I?.SetLanguage(LocalizationManager.Language.ES);
        TabsUI.Instance.ShowGeneracion();
        VerticalNavigationUI navigation = TabsUI.Instance.verticalNavigation;
        navigation.RefreshAvailability();
        RefreshAllUi();
        navigation.prestigeButton.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(navigation.secondaryScroll.content);
        navigation.secondaryScroll.horizontalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
        navigation.enabled = false;
    }

    private static void PrepareEarlyGeneration()
    {
        Require(GameState.I != null, "GameState no se inicializo en Play Mode.");
        Require(F2UpgradeManager.I != null, "F2UpgradeManager no se inicializo.");
        Require(TabsUI.Instance != null, "TabsUI no se inicializo.");

        SaveService.I?.CancelInvoke("Save");
        BuildingListUI buildingList = UnityEngine.Object.FindFirstObjectByType<BuildingListUI>(
            FindObjectsInactive.Include);
        Require(buildingList != null && buildingList.EnsureInitialized(),
            "BuildingListUI no pudo inicializar el registro real.");

        foreach (string id in BuildingIds)
        {
            BuildingState building = RequireBuilding(id);
            building.ResetForPrestige();
        }

        F2UpgradeManager.I.DebugResetAllPurchases();
        F2UpgradeManager.I.ApplyLoadedPurchasedTiers(
            new List<SavedF2UpgradeTier>(), F2UpgradeManager.ProgressionMigrationVersion);

        GameState state = GameState.I;
        state.LE = 5000.0;
        state.Traces = 0.0;
        state.triangleSystemUnlocked = false;
        state.triangleActiveCircuit = TriangleCircuitType.None;
        state.triangleSynchronization = 0f;
        state.triangleSynchronizationBaseRatePerSecond = 0.0;
        state.experimentalChamberUnlocked = false;
        state.dimension01Unlocked = false;
        state.dimension02Unlocked = false;
        state.dimension03Unlocked = false;
        UpgradeStudySystem.EnsureState(state).hideCompletedUpgrades = false;
        MachineManager.I?.ResetOperationalProgress();

        LocalizationManager.I?.SetLanguage(LocalizationManager.Language.ES);
        TabsUI.Instance.ShowGeneracion();

        BuildingRowUI higgsRow = RequireBuildingRow("vacuum_observer");
        int before = state.GetBuildingLevel("vacuum_observer");
        higgsRow.buyButton.onClick.Invoke();
        Require(state.GetBuildingLevel("vacuum_observer") == before + 1,
            "La compra de Higgs desde la fila temprana no incremento el nivel.");

        RefreshAllUi();
        Debug.Log("[Vertical UI Block 7] FUNCTION PASS | compra Higgs desde Generacion temprana");
    }

    private static void ValidateEarlyPresentation()
    {
        VerticalGenerationBeforeTriangleUI generation =
            UnityEngine.Object.FindFirstObjectByType<VerticalGenerationBeforeTriangleUI>(
                FindObjectsInactive.Include);
        Require(generation != null && generation.beforeTriangleRoot.activeSelf &&
            !generation.triangleRoot.activeSelf,
            "Generacion temprana no es el unico estado visible.");
        Require(RequireBuildingRow("casimir_panel").gameObject.activeInHierarchy,
            "Tetraquark no se revelo tras comprar Higgs.");
        Require(!RequireBuildingRow("fluctuation_antenna").gameObject.activeInHierarchy,
            "Modulador se revelo antes de comprar Tetraquark.");
        Require(TabsUI.Instance.verticalNavigation != null &&
            !TabsUI.Instance.verticalNavigation.secondaryNavigationRoot.gameObject.activeSelf,
            "La barra secundaria aparece en el estado inicial.");
    }

    private static void BuyEarlyUpgradeAndRemainingVertices()
    {
        GameState state = GameState.I;
        state.LE = 100000.0;
        state.Traces = 5000.0;

        F2UpgradeRowUI emission = RequireUpgradeRow("emission_focus");
        int emissionBefore = F2UpgradeManager.I.GetPurchasedTierCount("emission_focus");
        emission.BuyButton.onClick.Invoke();
        Require(F2UpgradeManager.I.GetPurchasedTierCount("emission_focus") == emissionBefore + 1,
            "Emision Calibrada no se compro desde Panel_Mejoras.");

        TabsUI.Instance.ShowGeneracion();
        BuyBuildingFromEarlyRow("casimir_panel");
        BuyBuildingFromEarlyRow("fluctuation_antenna");
        int modulatorLevel = state.GetBuildingLevel("fluctuation_antenna");
        Require(modulatorLevel == 1, "El Modulador no quedo en nivel 1.");
        BuildingState energyGenerator = state.GetBuildingState("fluctuation_antenna");
        double leCost = state.GetTriangleEnergyGeneratorLECost();
        double traceCost = state.GetTriangleEnergyGeneratorTraceCost();
        double leBefore = state.LE;
        double tracesBefore = state.Traces;
        Require(BuildingPurchaseService.TryPurchase(state, energyGenerator) &&
            state.GetBuildingLevel("fluctuation_antenna") == 2,
            "El Captador no admitio su segundo nivel repetible.");
        Require(Math.Abs(state.LE - (leBefore - leCost)) < 0.0001 &&
            Math.Abs(state.Traces - (tracesBefore - traceCost)) < 0.0001,
            "El Captador no desconto LE y Trazas de forma atomica.");

        Debug.Log("[Vertical UI Block 7] FUNCTION PASS | F2 desde Mejoras | " +
            "Tetra y Modulador desde Generacion | Captador repetible con LE + Trazas");
    }

    private static void BuyTriangleUnlockFromUi()
    {
        GameState.I.Traces = 5000.0;
        VerticalUpgradesScreenUI screen =
            UnityEngine.Object.FindFirstObjectByType<VerticalUpgradesScreenUI>(
                FindObjectsInactive.Include);
        screen?.RefreshNow();

        F2UpgradeRowUI unlock = RequireUpgradeRow("triangle_unlock_1");
        Require(unlock.gameObject.activeInHierarchy,
            "Acople de Vertices no se hizo visible con los tres vertices.");
        Debug.Log("[Vertical UI Block 7] Acople diagnostic | lock=" +
            F2UpgradeManager.I.GetLockReason("triangle_unlock_1") +
            " | higgs=" + GameState.I.GetBuildingLevel("vacuum_observer") +
            " | tetra=" + GameState.I.GetBuildingLevel("casimir_panel") +
            " | mod=" + GameState.I.GetBuildingLevel("fluctuation_antenna") +
            " | traces=" + GameState.I.Traces);
        Require(F2UpgradeManager.I.CanBuy("triangle_unlock_1"),
            "Acople no esta comprable antes de invocar su boton: " +
            F2UpgradeManager.I.GetLockReason("triangle_unlock_1"));
        unlock.BuyButton.onClick.Invoke();
        Require(GameState.I.triangleSystemUnlocked &&
            F2UpgradeManager.I.GetPurchasedTierCount("triangle_unlock_1") == 1,
            "Acople no activo el Triangulo desde Panel_Mejoras.");
        RefreshAllUi();
        Debug.Log("[Vertical UI Block 7] FUNCTION PASS | Acople desde Mejoras | cambio avanzado");
    }

    private static void ValidateAndCaptureTriangleTutorial()
    {
        TriangleActivationTutorialUI tutorial =
            UnityEngine.Object.FindFirstObjectByType<TriangleActivationTutorialUI>(
                FindObjectsInactive.Include);
        Require(tutorial != null && tutorial.gameObject.activeInHierarchy &&
                GameState.I.triangleActivationTutorialSeen,
            "Acople no devolvio a Generacion con el tutorial persistido y visible.");
        Require(TabsUI.Instance.panelGeneracion != null &&
                TabsUI.Instance.panelGeneracion.activeInHierarchy,
            "El tutorial se mostro fuera de la pantalla de Generacion.");
        Capture("02b_triangle_tutorial_es_1080x1920.png", 1080, 1920);
        Capture("02c_triangle_tutorial_es_720x1280.png", 720, 1280);
        Button close = tutorial.GetComponentsInChildren<Button>(true)
            .FirstOrDefault(button => button.name == "Close");
        Require(close != null, "El tutorial no tiene cierre accesible.");
        close.onClick.Invoke();
    }

    private static void ValidateAdvancedUpgradesPresentation()
    {
        VerticalUpgradesScreenUI screen =
            UnityEngine.Object.FindFirstObjectByType<VerticalUpgradesScreenUI>(
                FindObjectsInactive.Include);
        Require(screen != null, "VerticalUpgradesScreenUI no esta disponible.");
        screen.RefreshNow();
        foreach (string id in TriangleUpgradeIds)
            Require(RequireUpgradeRow(id).gameObject.activeInHierarchy,
                "La fila triangular no esta visible: " + id);
    }

    private static void PrepareCompletedUpgradesHidden()
    {
        VerticalUpgradesScreenUI screen =
            UnityEngine.Object.FindFirstObjectByType<VerticalUpgradesScreenUI>(
                FindObjectsInactive.Include);
        Require(screen != null && screen.rows != null,
            "VerticalUpgradesScreenUI no esta disponible para ocultar completadas.");

        var completed = new List<SavedF2UpgradeTier>();
        foreach (F2UpgradeRowUI row in screen.rows)
        {
            if (row == null || string.IsNullOrWhiteSpace(row.UpgradeId)) continue;
            F2UpgradeDef def = F2UpgradeManager.I.GetDef(row.UpgradeId);
            if (def?.tiers == null || def.tiers.Count == 0) continue;
            completed.Add(new SavedF2UpgradeTier
            {
                id = row.UpgradeId,
                purchasedTiers = def.tiers.Count
            });
        }

        F2UpgradeManager.I.ApplyLoadedPurchasedTiers(
            completed, F2UpgradeManager.ProgressionMigrationVersion);
        GameState.I.experimentalChamberUnlocked = true;
        UpgradeStudySystem.EnsureState(GameState.I).hideCompletedUpgrades = true;
        TabsUI.Instance.ShowMejoras();
        screen.RefreshNow();
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateCompletedUpgradesHidden()
    {
        VerticalUpgradesScreenUI screen =
            UnityEngine.Object.FindFirstObjectByType<VerticalUpgradesScreenUI>(
                FindObjectsInactive.Include);
        Require(screen != null && screen.emptyCompletedStateRoot != null &&
            screen.emptyCompletedStateRoot.activeInHierarchy,
            "El estado vacio no aparece al ocultar todas las mejoras completadas.");
        Require(screen.productionSection != null &&
            screen.productionSection.GetComponent<CanvasGroup>().alpha < .01f &&
            screen.tracesSection != null &&
            screen.tracesSection.GetComponent<CanvasGroup>().alpha < .01f &&
            screen.triangleSection != null &&
            screen.triangleSection.GetComponent<CanvasGroup>().alpha < .01f,
            "Alguna seccion completada sigue visible tras activar el filtro.");
    }

    private static void PrepareAdvancedGenerationEnglish()
    {
        TabsUI.Instance.ShowGeneracion();
        LocalizationManager.I?.SetLanguage(LocalizationManager.Language.EN);

        TriangleSlotUI energy = FindNamedComponent<TriangleSlotUI>("Circuit_Energy");
        Require(energy != null, "No se encontro el selector Energy.");
        energy.OnPointerClick(null);
        Require(GameState.I.triangleActiveCircuit == TriangleCircuitType.Energy,
            "El selector visual no activo el circuito Energy.");

        BuildingState higgs = RequireBuilding("vacuum_observer");
        int before = higgs.level;
        VerticalTriangleArtifactCardUI card = FindArtifactCard("vacuum_observer");
        Require(card != null && card.buyButton != null,
            "La tarjeta avanzada de Higgs no esta conectada.");
        card.buyButton.onClick.Invoke();
        Require(higgs.level == before + 1,
            "La tarjeta avanzada de Higgs no compro un nivel.");
        RefreshAllUi();
        Debug.Log("[Vertical UI Block 7] FUNCTION PASS | selector Energy | compra desde tarjeta avanzada | EN");
    }

    private static void PrepareEnergyFocusSpanish()
    {
        LocalizationManager.I?.SetLanguage(LocalizationManager.Language.ES);
        TabsUI.Instance.ShowGeneracion();
        RefreshAllUi();
    }

    private static void ValidateEnergyFocusAvailable()
    {
        TriangleSlotUI phase = FindNamedComponent<TriangleSlotUI>("Circuit_Phase");
        Require(phase != null, "No se encontro el selector de Energia.");
        phase.OnPointerClick(null);
        Require(GameState.I.IsTrianglePhaseUnlocked() &&
            GameState.I.triangleActiveCircuit == TriangleCircuitType.Phase,
            "El enfoque de Energia no se activo con el Triangulo completo.");
        Debug.Log("[Vertical UI Block 7] FUNCTION PASS | enfoque Energia disponible desde el Triangulo");
    }

    private static void PrepareProgressiveNavigation()
    {
        GameState state = GameState.I;
        state.experimentalChamberUnlocked = true;
        state.dimension01Unlocked = true;
        state.dimension02Unlocked = true;
        state.dimension03Unlocked = true;
        TabsUI.Instance.ShowGeneracion();
        TabsUI.Instance.verticalNavigation.RefreshAvailability();
        RefreshAllUi();
    }

    private static void ValidateProgressiveNavigation()
    {
        VerticalNavigationUI navigation = TabsUI.Instance.verticalNavigation;
        Require(navigation != null && navigation.secondaryNavigationRoot.gameObject.activeSelf,
            "La barra secundaria no aparecio con sistemas desbloqueados.");
        Require(navigation.room2Button.gameObject.activeSelf &&
            navigation.dimension1Button.gameObject.activeSelf &&
            navigation.dimension2Button.gameObject.activeSelf &&
            navigation.dimension3Button.gameObject.activeSelf,
            "La barra secundaria no refleja los cuatro gates reales preparados.");
        Require(navigation.secondaryScroll != null && navigation.secondaryScroll.horizontal &&
            !navigation.secondaryScroll.vertical,
            "La barra secundaria no conserva desplazamiento exclusivamente horizontal.");

        // El mockup avanzado usa todos los accesos a la vez como prueba de
        // capacidad. Los cuatro gates reales anteriores ya se comprobaron;
        // se habilita Prestigio solo para forzar el caso visual de overflow.
        navigation.prestigeButton.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(navigation.secondaryScroll.content);
        Canvas.ForceUpdateCanvases();
        navigation.secondaryScroll.horizontalNormalizedPosition = 1f;
        Canvas.ForceUpdateCanvases();
        Debug.Log("[Vertical UI Block 7] FUNCTION PASS | barra ausente al inicio | " +
            "Cuarto 2 + Dim 1-3 progresivos | capacidad maxima preparada");
    }

    private static void ApplySimulatedSafeArea()
    {
        VerticalSafeAreaLayout safe =
            UnityEngine.Object.FindFirstObjectByType<VerticalSafeAreaLayout>(
                FindObjectsInactive.Include);
        Require(safe != null && safe.safeAreaRoot != null,
            "VerticalSafeAreaLayout no esta disponible.");

        safe.enabled = false;
        Rect simulated = new Rect(0f, 96f, 1080f, 2148f);
        VerticalSafeAreaLayout.CalculateSafeAnchors(simulated, 1080, 2340,
            out Vector2 min, out Vector2 max);
        safe.safeAreaRoot.anchorMin = min;
        safe.safeAreaRoot.anchorMax = max;
        safe.safeAreaRoot.anchoredPosition = Vector2.zero;
        safe.safeAreaRoot.sizeDelta = Vector2.zero;
        Canvas.ForceUpdateCanvases();
        Require(min.y > 0f && max.y < 1f,
            "La simulacion de notch/barra inferior no produjo margenes seguros.");
    }

    private static void ValidateSaveAndLoadRoundTrip()
    {
        Require(SaveService.I != null, "SaveService no esta disponible.");
        GameState state = GameState.I;
        state.triangleSynchronization = 0.42f;
        state.triangleSynchronizationBaseRatePerSecond = 0.001;
        int higgsLevel = state.GetBuildingLevel("vacuum_observer");
        int emissionTier = F2UpgradeManager.I.GetPurchasedTierCount("emission_focus");
        TriangleCircuitType circuit = state.triangleActiveCircuit;

        Require(SaveService.I.TrySave(out string error),
            "No se pudo crear el guardado de prueba: " + error);

        state.LE = 1.0;
        state.triangleSystemUnlocked = false;
        state.triangleActiveCircuit = TriangleCircuitType.None;
        state.triangleSynchronization = 0f;
        RequireBuilding("vacuum_observer").ResetForPrestige();
        F2UpgradeManager.I.DebugResetAllPurchases();

        SaveService.I.Load();
        state.ApplyBuildingLevelsFromSave(SaveService.LastLoadedBuildingLevels);

        Require(state.triangleSystemUnlocked && state.triangleActiveCircuit == circuit,
            "Save/load no restauro Acople o circuito.");
        Require(state.triangleSynchronization >= 0.419f,
            "Save/load no restauro sincronizacion.");
        Require(state.GetBuildingLevel("vacuum_observer") == higgsLevel,
            "Save/load no restauro edificios.");
        Require(F2UpgradeManager.I.GetPurchasedTierCount("emission_focus") == emissionTier,
            "Save/load no restauro mejoras F2.");

        Debug.Log("[Vertical UI Block 7] FUNCTION PASS | save/load | Acople | circuito | " +
            "sincronizacion | edificios | mejoras");
    }

    private static void BuyBuildingFromEarlyRow(string id)
    {
        BuildingState state = RequireBuilding(id);
        int before = state.level;
        BuildingRowUI row = RequireBuildingRow(id);
        row.buyButton.onClick.Invoke();
        Require(state.level == before + 1,
            "La fila temprana no compro " + id + ".");
        RefreshAllUi();
    }

    private static void RefreshAllUi()
    {
        HideTransientOverlays();
        TabsUI tabs = TabsUI.Instance;
        tabs?.RefreshGenerationLayoutFromOutside();
        tabs?.verticalNavigation?.RefreshAvailability();

        VerticalGenerationBeforeTriangleUI generation =
            UnityEngine.Object.FindFirstObjectByType<VerticalGenerationBeforeTriangleUI>(
                FindObjectsInactive.Include);
        generation?.RefreshState();
        VerticalUpgradesScreenUI upgrades =
            UnityEngine.Object.FindFirstObjectByType<VerticalUpgradesScreenUI>(
                FindObjectsInactive.Include);
        upgrades?.RefreshNow();
        Canvas.ForceUpdateCanvases();
    }

    private static void PrepareResolution(int width, int height)
    {
        Screen.SetResolution(width, height, false);
        Canvas.ForceUpdateCanvases();
    }

    private static void Capture(string fileName, int width, int height)
    {
        HideTransientOverlays();
        string path = Path.Combine(OutputDirectory, fileName);
        RenderToPng(width, height, path);
        Require(File.Exists(path) && new FileInfo(path).Length > 4096,
            "La captura no se genero correctamente: " + fileName);
        Debug.Log("[Vertical UI Block 7 Capture] PNG | " + fileName +
            " | " + width + "x" + height);
    }

    private static void RenderToPng(int width, int height, string path)
    {
        Camera camera = Camera.main != null
            ? Camera.main
            : UnityEngine.Object.FindFirstObjectByType<Camera>();
        Require(camera != null, "No hay camara para renderizar Game View.");

        Canvas[] canvases = UnityEngine.Object.FindObjectsByType<Canvas>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        var modes = new RenderMode[canvases.Length];
        var cameras = new Camera[canvases.Length];
        var distances = new float[canvases.Length];

        for (int i = 0; i < canvases.Length; i++)
        {
            modes[i] = canvases[i].renderMode;
            cameras[i] = canvases[i].worldCamera;
            distances[i] = canvases[i].planeDistance;
        }

        RenderTexture target = new RenderTexture(
            width, height, 24, RenderTextureFormat.ARGB32);
        RenderTexture previousTarget = camera.targetTexture;
        RenderTexture previousActive = RenderTexture.active;
        try
        {
            target.Create();
            camera.targetTexture = target;
            for (int i = 0; i < canvases.Length; i++)
            {
                if (modes[i] != RenderMode.ScreenSpaceOverlay)
                    continue;
                canvases[i].renderMode = RenderMode.ScreenSpaceCamera;
                canvases[i].worldCamera = camera;
                canvases[i].planeDistance = 1f;
            }
            RecalculateCanvasScalers(canvases);
            Canvas.ForceUpdateCanvases();
            foreach (Canvas canvas in canvases)
                if (canvas != null && canvas.transform is RectTransform rootRect)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(rootRect);
            Canvas.ForceUpdateCanvases();
            ValidateGenerationModulesClearNavigation();
            ValidateTargetScrollCapacity();
            camera.Render();
            RenderTexture.active = target;

            Texture2D image = new Texture2D(
                width, height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
            image.Apply(false, false);
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
                canvases[i].planeDistance = distances[i];
            }
            // Restablecer también la geometría impulsada por CanvasScaler.
            // Sin este paso, una segunda captura en la misma sesión puede
            // conservar el recorte temporal de ScreenSpaceCamera.
            RecalculateCanvasScalers(canvases);
            Canvas.ForceUpdateCanvases();
            foreach (Canvas canvas in canvases)
                if (canvas != null && canvas.transform is RectTransform rootRect)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(rootRect);
            Canvas.ForceUpdateCanvases();
        }
    }

    private static void ValidateGenerationModulesClearNavigation()
    {
        VerticalNavigationUI navigation = TabsUI.Instance != null
            ? TabsUI.Instance.verticalNavigation
            : null;
        if (navigation == null || navigation.secondaryNavigationRoot == null ||
            !navigation.secondaryNavigationRoot.gameObject.activeSelf)
            return;

        VerticalGenerationBeforeTriangleUI generation =
            UnityEngine.Object.FindFirstObjectByType<VerticalGenerationBeforeTriangleUI>(
                FindObjectsInactive.Include);
        ScrollRect scroll = generation?.triangleRoot != null
            ? generation.triangleRoot.transform.Find("TriangleScroll")?
                .GetComponent<ScrollRect>()
            : null;
        Require(generation != null && scroll != null &&
            scroll.viewport != null && scroll.content != null,
            "Falta el viewport avanzado de Generacion.");
        generation.RefreshState();
        scroll.verticalNormalizedPosition = 1f;
        Canvas.ForceUpdateCanvases();

        RectTransform viewport = scroll.viewport;
        RectTransform cards = scroll.content.Find(
            "TriangleArtifactCards") as RectTransform;
        Require(cards != null, "Faltan las tarjetas de modulos avanzados.");
        Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(
            viewport, cards);
        Require(bounds.min.y >= viewport.rect.yMin - .5f,
            "Las tarjetas de modulos quedan cortadas por la navegacion secundaria: " +
            $"cardsMin={bounds.min.y:F1}, viewportMin={viewport.rect.yMin:F1}.");
        Require(bounds.max.y <= viewport.rect.yMax + .5f,
            "Las tarjetas de modulos exceden la parte superior del viewport.");
        foreach (string nodeName in new[]
        {
            "Vertex_Higgs", "Vertex_Tetra", "Vertex_Modulator"
        })
        {
            RectTransform node = scroll.content.Find(
                "TriangleFocus/" + nodeName) as RectTransform;
            Require(node != null, "Falta el nodo " + nodeName + ".");
            Bounds nodeBounds =
                RectTransformUtility.CalculateRelativeRectTransformBounds(
                    viewport, node);
            Require(nodeBounds.max.y <= viewport.rect.yMax + .5f &&
                nodeBounds.min.y >= viewport.rect.yMin - .5f,
                nodeName + " o su rotulo quedan recortados por el viewport.");
        }
    }

    private static void RecalculateCanvasScalers(Canvas[] canvases)
    {
        MethodInfo handle = typeof(CanvasScaler).GetMethod(
            "Handle", BindingFlags.Instance | BindingFlags.NonPublic);
        if (handle == null)
            return;
        foreach (Canvas canvas in canvases)
        {
            CanvasScaler scaler = canvas != null
                ? canvas.GetComponent<CanvasScaler>()
                : null;
            if (scaler != null && scaler.enabled)
                handle.Invoke(scaler, null);
        }
    }

    private static void ValidateTargetScrollCapacity()
    {
        VerticalNavigationUI navigation =
            UnityEngine.Object.FindFirstObjectByType<VerticalNavigationUI>(
                FindObjectsInactive.Include);
        if (navigation == null || navigation.secondaryScroll == null ||
            !navigation.secondaryNavigationRoot.gameObject.activeSelf ||
            !navigation.prestigeButton.gameObject.activeSelf)
        {
            return;
        }

        ScrollRect scroll = navigation.secondaryScroll;
        LayoutRebuilder.ForceRebuildLayoutImmediate(scroll.content);
        Canvas.ForceUpdateCanvases();
        if (scroll.viewport.rect.width >= 900f)
            return;
        Require(scroll.content.rect.width > scroll.viewport.rect.width,
            "La barra secundaria no genera overflow en el target vertical estrecho.");
        scroll.horizontalNormalizedPosition = 1f;
        Canvas.ForceUpdateCanvases();
        Require(scroll.horizontalNormalizedPosition > 0.01f,
            "La barra secundaria no se desplaza horizontalmente en el target estrecho.");
    }

    private static void HideTransientOverlays()
    {
        PresentationReturnReportUI[] reports =
            UnityEngine.Object.FindObjectsByType<PresentationReturnReportUI>(
                FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (PresentationReturnReportUI report in reports)
            if (report != null && report.gameObject.activeSelf)
                report.gameObject.SetActive(false);
    }

    private static BuildingState RequireBuilding(string id)
    {
        BuildingState state = GameState.I?.GetBuildingState(id);
        Require(state != null && state.def != null,
            "No existe el edificio registrado: " + id);
        return state;
    }

    private static BuildingRowUI RequireBuildingRow(string id)
    {
        VerticalGenerationBeforeTriangleUI generation =
            UnityEngine.Object.FindFirstObjectByType<VerticalGenerationBeforeTriangleUI>(
                FindObjectsInactive.Include);
        Require(generation != null && generation.beforeTriangleRoot != null,
            "No existe GenerationBeforeTriangleRoot para localizar filas verticales.");
        BuildingRowUI row = UnityEngine.Object.FindObjectsByType<BuildingRowUI>(
            FindObjectsInactive.Include, FindObjectsSortMode.None)
            .FirstOrDefault(candidate => candidate.BuildingId == id &&
                candidate.transform.IsChildOf(generation.beforeTriangleRoot.transform));
        Require(row != null && row.buyButton != null,
            "No existe la fila de edificio: " + id);
        return row;
    }

    private static F2UpgradeRowUI RequireUpgradeRow(string id)
    {
        F2UpgradeRowUI row = UnityEngine.Object.FindObjectsByType<F2UpgradeRowUI>(
            FindObjectsInactive.Include, FindObjectsSortMode.None)
            .FirstOrDefault(candidate => candidate.UpgradeId == id);
        Require(row != null && row.BuyButton != null,
            "No existe la fila F2: " + id);
        return row;
    }

    private static VerticalTriangleArtifactCardUI FindArtifactCard(string id)
    {
        return UnityEngine.Object.FindObjectsByType<VerticalTriangleArtifactCardUI>(
            FindObjectsInactive.Include, FindObjectsSortMode.None)
            .FirstOrDefault(card => card.buildingId == id);
    }

    private static T FindNamedComponent<T>(string name) where T : Component
    {
        return UnityEngine.Object.FindObjectsByType<T>(
            FindObjectsInactive.Include, FindObjectsSortMode.None)
            .FirstOrDefault(component => component.name == name);
    }

    private static void Advance(int nextStage)
    {
        SessionState.SetInt(StageKey, nextStage);
        SessionState.SetInt(FramesKey, 0);
        stageStartedAt = EditorApplication.timeSinceStartup;
    }

    private static void BackupUserSave()
    {
        bool saveExists = File.Exists(SavePath);
        bool backupExists = File.Exists(SaveBackupPath);
        SessionState.SetBool(SaveExistedKey, saveExists);
        SessionState.SetBool(SaveBackupExistedKey, backupExists);

        if (saveExists)
            File.Copy(SavePath, SessionSaveCopy, true);
        if (backupExists)
            File.Copy(SaveBackupPath, SessionSaveBackupCopy, true);
    }

    private static void RestoreUserSave()
    {
        RestoreOne(SavePath, SessionSaveCopy,
            SessionState.GetBool(SaveExistedKey, false));
        RestoreOne(SaveBackupPath, SessionSaveBackupCopy,
            SessionState.GetBool(SaveBackupExistedKey, false));
    }

    private static void RestoreOne(string target, string backup, bool existed)
    {
        if (existed)
        {
            if (!File.Exists(backup))
                throw new IOException("Falta el respaldo de sesion: " + backup);
            File.Copy(backup, target, true);
        }
        else if (File.Exists(target))
        {
            File.Delete(target);
        }

        if (File.Exists(backup))
            File.Delete(backup);
    }

    private static void RecoverStaleSessionBackupIfNecessary()
    {
        if (!SessionState.GetBool(ActiveKey, false))
            return;
        RestoreUserSave();
        SessionState.SetBool(ActiveKey, false);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }

    private static readonly string[] BuildingIds =
    {
        "vacuum_observer",
        "casimir_panel",
        "fluctuation_antenna"
    };

    private static readonly string[] TriangleUpgradeIds =
    {
        "triangle_unlock_1",
        "triangle_impulse_tuning",
        "triangle_synergy_resonance",
        "triangle_persistence_anchor",
        "triangle_energy_efficiency"
    };
}
#endif

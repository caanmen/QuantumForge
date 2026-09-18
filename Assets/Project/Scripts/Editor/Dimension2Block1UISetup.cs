#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public static class Dimension2Block1UISetup
{
    private const string MainScenePath = "Assets/Project/Scenes/Main.unity";
    private const string FirstEntryStonePath =
        "Assets/Project/UI/Dimension2/Pacts/Generated/PactsScreen_Background.png";
    private const string FirstEntryHeroPath =
        "Assets/Project/UI/Dimension2/FirstEntry/Generated/D2_FirstEntry_Hero_v1.png";
    private const string FirstEntryFrameAtlasPath =
        "Assets/Project/UI/Dimension2/FirstEntry/Generated/D2_FirstEntry_FrameAtlas_v2.png";
    private const string FirstEntryButtonPlatePath =
        "Assets/Project/UI/Dimension2/FirstEntry/Generated/D2_FirstEntry_ButtonPlate_v2.png";
    private const string FirstEntryOuterFramePath =
        "Assets/Project/UI/Dimension2/FirstEntry/Generated/D2_FirstEntry_OuterFrame_v2.png";
    private const string FirstEntryWhiteKeyShaderPath =
        "Assets/Project/UI/Dimension2/FirstEntry/Generated/D2FirstEntryWhiteKey.shader";
    private const string FirstEntryWhiteKeyMaterialPath =
        "Assets/Project/UI/Dimension2/FirstEntry/Generated/D2FirstEntryWhiteKey.mat";
    private const string FirstEntryFontPath =
        "Assets/Project/Fonts/Cinzel/Cinzel SDF.asset";
    private const string MapGeneratedPath =
        "Assets/Project/UI/Dimension2/Map/Generated/";
    private const string MapShellPath = MapGeneratedPath + "D2_Map_Shell_v1.png";
    private const string MapCleanBasePlatePath = MapGeneratedPath + "D2_Map_CleanBasePlate_v1.png";
    private const string MapSanctuaryPath = MapGeneratedPath + "D2_Map_Territory_Sanctuary_v1.png";
    private const string MapResistancePath = MapGeneratedPath + "D2_Map_Territory_Resistance_v1.png";
    private const string MapRuinsPath = MapGeneratedPath + "D2_Map_Territory_Ruins_v1.png";
    private const string MapSanctuaryMedallionPath = MapGeneratedPath + "D2_Map_Medallion_Sanctuary_v1.png";
    private const string MapResistanceMedallionPath = MapGeneratedPath + "D2_Map_Medallion_Resistance_v1.png";
    private const string MapLockMedallionPath = MapGeneratedPath + "D2_Map_Medallion_Lock_v1.png";
    private const string MapHelpPath = MapGeneratedPath + "D2_Map_Button_Help_v1.png";
    private const string MapBackPath = MapGeneratedPath + "D2_Map_Button_Back_v1.png";
    private const string MapFollowersPath = MapGeneratedPath + "D2_Map_Icon_Followers_v1.png";
    private const string MapTrustPath = MapGeneratedPath + "D2_Map_Icon_Trust_v1.png";
    private const string MapSanctuaryButtonPath = MapGeneratedPath + "D2_Map_Button_Sanctuary_v1.png";
    private const string MapWhiteKeyMaterialPath = MapGeneratedPath + "D2MapWhiteKey.mat";
    private const string SanctuaryGeneratedPath =
        "Assets/Project/UI/Dimension2/Sanctuary/Generated/";
    private const string SanctuaryCleanBasePlatePath =
        SanctuaryGeneratedPath + "D2_Sanctuary_CleanBasePlate_v1.png";
    private const string SanctuaryStaticIconOverlayPath =
        SanctuaryGeneratedPath + "D2_Sanctuary_StaticIconOverlay_v1.png";
    private const string AltarsGeneratedPath =
        "Assets/Project/UI/Dimension2/Altars/Generated/";
    private const string AltarsCleanBasePlatePath =
        AltarsGeneratedPath + "D2_Altars_CleanBasePlate_v1.png";
    private const string AltarsStaticIconOverlayPath =
        AltarsGeneratedPath + "D2_Altars_StaticIconOverlay_v1.png";
    private const string AltarsWaxSelectionOverlayPath =
        AltarsGeneratedPath + "D2_Altars_WaxSelectionOverlay_v1.png";
    private const string PilgrimagesGeneratedPath =
        "Assets/Project/UI/Dimension2/Pilgrimages/Generated/";
    private const string PilgrimagesCleanBasePlatePath =
        PilgrimagesGeneratedPath + "D2_Pilgrimages_CleanBasePlate_v1.png";
    private const string PilgrimagesStaticIconOverlayPath =
        PilgrimagesGeneratedPath + "D2_Pilgrimages_StaticIconOverlay_v1.png";
    private const string PilgrimagesShortSelectionOverlayPath =
        PilgrimagesGeneratedPath + "D2_Pilgrimages_ShortSelectionOverlay_v1.png";
    private const string NovitiateGeneratedPath =
        "Assets/Project/UI/Dimension2/Novitiate/Generated/";
    private const string NovitiateCleanBasePlatePath =
        NovitiateGeneratedPath + "D2_Novitiate_CleanBasePlate_v1.png";
    private const string NovitiateStaticIconOverlayPath =
        NovitiateGeneratedPath + "D2_Novitiate_StaticIconOverlay_v1.png";
    private const string RitesGeneratedPath =
        "Assets/Project/UI/Dimension2/Rites/Generated/";
    private const string RitesCleanBasePlatePath =
        RitesGeneratedPath + "D2_Rites_CleanBasePlate_v1.png";
    private const string RitesWelcomeNeutralOverlayPath =
        RitesGeneratedPath + "D2_Rites_WelcomeNeutralOverlay_v1.png";
    private const string RitesOfferingSelectionOverlayPath =
        RitesGeneratedPath + "D2_Rites_OfferingSelectionOverlay_v1.png";
    private const string RitesPathSelectionOverlayPath =
        RitesGeneratedPath + "D2_Rites_PathSelectionOverlay_v1.png";
    private const string RitesNovitiateSelectionOverlayPath =
        RitesGeneratedPath + "D2_Rites_NovitiateSelectionOverlay_v1.png";
    private const string RitesRespectSelectionOverlayPath =
        RitesGeneratedPath + "D2_Rites_RespectSelectionOverlay_v1.png";
    private const string PactsGeneratedPath =
        "Assets/Project/UI/Dimension2/Pacts/Generated/";
    private const string PactsCleanBasePlatePath =
        PactsGeneratedPath + "D2_Pacts_CleanBasePlate_v1.png";
    private const string PactsHospitalityNeutralOverlayPath =
        PactsGeneratedPath + "D2_Pacts_HospitalityNeutralOverlay_v1.png";
    private const string PactsOpenPathSelectionOverlayPath =
        PactsGeneratedPath + "D2_Pacts_OpenPathSelectionOverlay_v1.png";
    private const string PactsConsecrationSelectionOverlayPath =
        PactsGeneratedPath + "D2_Pacts_ConsecrationSelectionOverlay_v1.png";
    private const string PactsSilentVowSelectionOverlayPath =
        PactsGeneratedPath + "D2_Pacts_SilentVowSelectionOverlay_v1.png";
    private const string PactsInnerDoorSelectionOverlayPath =
        PactsGeneratedPath + "D2_Pacts_InnerDoorSelectionOverlay_v1.png";
    private const string PactsHospitalitySlotNeutralOverlayPath =
        PactsGeneratedPath + "D2_Pacts_HospitalitySlotNeutralOverlay_v1.png";
    private const string PactsSecondSlotUnlockedOverlayPath =
        PactsGeneratedPath + "D2_Pacts_SecondSlotUnlockedOverlay_v1.png";
    private const string PactsStaticDetailIconsOverlayPath =
        PactsGeneratedPath + "D2_Pacts_StaticDetailIconsOverlay_v1.png";
    private const string PactsLockRequirementIconsOverlayPath =
        PactsGeneratedPath + "D2_Pacts_LockRequirementIconsOverlay_v1.png";
    private const string PactsActionButtonsOverlayPath =
        PactsGeneratedPath + "D2_Pacts_ActionButtonsOverlay_v1.png";
    private const string PactsHospitalityMedallionPath = PactsGeneratedPath + "Medallion_Hospitality.png";
    private const string PactsRoadMedallionPath = PactsGeneratedPath + "Medallion_Road.png";
    private const string PactsCandleMedallionPath = PactsGeneratedPath + "Medallion_Candle.png";
    private const string PactsSilentVowMedallionPath = PactsGeneratedPath + "Medallion_SilentVow.png";
    private const string PactsDoorMedallionPath = PactsGeneratedPath + "Medallion_Door.png";
    private const string BondGeneratedPath =
        "Assets/Project/UI/Dimension2/Bond/Generated/";
    private const string BondCleanBasePlatePath =
        BondGeneratedPath + "D2_Bond_CleanBasePlate_v1.png";
    private const string ResistanceGeneratedPath =
        "Assets/Project/UI/Dimension2/Resistance/Generated/";
    private const string ResistanceRegionsCleanBasePlatePath =
        ResistanceGeneratedPath + "D2_ResistanceRegions_CleanBasePlate_v1.png";
    private const string ResistanceOperationsCleanBasePlatePath =
        ResistanceGeneratedPath + "D2_ResistanceOperations_CleanBasePlate_v1.png";
    private const string ResistanceDefenseCleanBasePlatePath =
        ResistanceGeneratedPath + "D2_ResistanceDefense_CleanBasePlate_v1.png";
    private const string ResistanceAlertCleanBasePlatePath =
        ResistanceGeneratedPath + "D2_ResistanceAlert_CleanBasePlate_v1.png";
    private const string ResistanceAlertIconSheetPath =
        ResistanceGeneratedPath + "D2_ResistanceAlert_IconSheetChroma_v1.png";
    private const string ResistanceContainmentCleanBasePlatePath =
        ResistanceGeneratedPath + "D2_ResistanceContainment_CleanBasePlate_v3.png";
    private const string ResistanceMajorPactCleanBasePlatePath =
        ResistanceGeneratedPath + "D2_ResistanceMajorPact_CleanBasePlate_v1.png";
    private const string ResistanceMajorPactLineSymbolsPath =
        ResistanceGeneratedPath + "D2_ResistanceMajorPact_LineSymbolsChroma_v1.png";
    private const string ResistancePactsCleanBasePlatePath =
        ResistanceGeneratedPath + "D2_ResistancePacts_CleanBasePlate_v1.png";
    private const string ResistancePactsFidelityPlatePath =
        ResistanceGeneratedPath + "D2_ResistancePacts_CleanBasePlate_v2.png";
    private const string ResistancePactsStaticIconOverlayPath =
        ResistanceGeneratedPath + "D2_ResistancePacts_StaticIconOverlay_v1.png";
    private const string ResistancePactsIconSheetPath =
        ResistanceGeneratedPath + "D2_ResistancePacts_IconSheetChroma_v1.png";
    private const string ResistancePactsInnerSymbolsPath =
        ResistanceGeneratedPath + "D2_ResistancePacts_InnerSymbolsChroma_v2.png";
    private const string ResistanceChromaKeyShaderPath =
        ResistanceGeneratedPath + "D2ResistanceChromaKey.shader";
    private const string ResistanceChromaKeyMaterialPath =
        ResistanceGeneratedPath + "D2ResistanceChromaKey.mat";
    private const string ResistanceMemberIconPath =
        "Assets/Project/UI/Dimension2/Pacts/Generated/Icon_Acolyte.png";
    private const string ArchaeologyGeneratedPath =
        "Assets/Project/UI/Dimension2/Archaeology/Generated/";
    private const string ArchaeologyCleanBasePlatePath =
        ArchaeologyGeneratedPath + "D2_RuinsArchaeology_CleanBasePlate_v1.png";
    private const string AnalysisGeneratedPath =
        "Assets/Project/UI/Dimension2/Analysis/Generated/";
    private const string AnalysisCleanBasePlatePath =
        AnalysisGeneratedPath + "D2_AnalyzeRemains_CleanBasePlate_v2.png";
    private const string ArchiveGeneratedPath =
        "Assets/Project/UI/Dimension2/Archive/Generated/";
    private const string ArchiveCleanBasePlatePath =
        ArchiveGeneratedPath + "D2_Archive_CleanBasePlate_v1.png";
    private const string ArchiveLockIconPath =
        ArchiveGeneratedPath + "D2_Archive_LockIcon_v1.png";
    private const string EntityResearchGeneratedPath =
        "Assets/Project/UI/Dimension2/EntityResearch/Generated/";
    private const string EntityResearchCleanBasePlatePath =
        EntityResearchGeneratedPath + "D2_EntityResearch_CleanBasePlate_v1.png";
    private const string EntityPactGeneratedPath =
        "Assets/Project/UI/Dimension2/EntityPact/Generated/";
    private const string EntityPactCleanBasePlatePath =
        EntityPactGeneratedPath + "D2_EntityPact_CleanBasePlate_v1.png";
    private const float FirstEntryWidth = 1080f;
    private const float FirstEntryHeight = 1920f;
    private const string ConfigureMenu =
        "Tools/Quantum Forge/Dimension 2/Configure Block 1 UI";
    private const string ValidateMenu =
        "Tools/Quantum Forge/Dimension 2/Validate Block 1";

    [MenuItem(ConfigureMenu)]
    public static void ConfigureBlock1UI()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.path != MainScenePath)
        {
            activeScene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
        }

        TabsUI tabs = Object.FindFirstObjectByType<TabsUI>(FindObjectsInactive.Include);
        if (tabs == null)
        {
            Debug.LogError("[D2 Block 1] No se encontró TabsUI en la escena abierta.");
            return;
        }

        if (tabs.btnDimension1 == null || tabs.dimension1Panel == null)
        {
            Debug.LogError(
                "[D2 Block 1] TabsUI necesita sus referencias de Dimensión 1 " +
                "para ubicar el nuevo botón y panel."
            );
            return;
        }

        Undo.SetCurrentGroupName("Configure Dimension 2 Block 1 UI");
        int undoGroup = Undo.GetCurrentGroup();

        Button dimension2Button = GetOrCreateTabButton(tabs);
        Dimension2PanelUI panel = GetOrCreateDimension2Panel(tabs);

        tabs.btnDimension2 = dimension2Button;
        tabs.dimension2Panel = panel.gameObject;
        EditorUtility.SetDirty(tabs);
        EditorUtility.SetDirty(panel);

        if (!ValidateFirstEntryVisual(panel))
        {
            Debug.LogError("[D2 First Entry] La composición estática no superó la validación previa al guardado.");
            return;
        }

        if (!ValidatePactMapVisual(panel))
        {
            Debug.LogError("[D2 Pact Map] La composición estática no superó la validación previa al guardado.");
            return;
        }

        if (!ValidateResistanceRegionsVisual(panel))
        {
            Debug.LogError("[D2 Resistance Regions] La composición estática no superó la validación previa al guardado.");
            return;
        }

        if (!ValidateResistanceDefenseVisual(panel))
        {
            Debug.LogError("[D2 Resistance Defense] La composición V4 no superó la validación previa al guardado.");
            return;
        }

        if (!ValidateResistanceAlertVisual(panel))
        {
            Debug.LogError("[D2 Resistance Alert] La composición V4 no superó la validación previa al guardado.");
            return;
        }

        if (!ValidateResistanceContainmentVisual(panel))
        {
            Debug.LogError("[D2 Resistance Containment] La composición V4 no superó la validación previa al guardado.");
            return;
        }

        panel.gameObject.SetActive(false);
        EditorSceneManager.MarkSceneDirty(activeScene);
        Undo.CollapseUndoOperations(undoGroup);

        if (!EditorSceneManager.SaveScene(activeScene))
        {
            Debug.LogError("[D2 Block 1] No se pudo guardar Main.unity.");
            return;
        }

        ValidateBlock1();
        Selection.activeGameObject = panel.gameObject;
        Debug.Log(
            "[D2 First Entry] BUILD_PASS | composición estática real | " +
            "texto único | botón ABRIR MAPA funcional | referencia 2026-08-24"
        );
        Debug.Log("[D2 Block 1] UI creada y conectada. Guarda la escena si la validación es correcta.");
    }

    [MenuItem(ValidateMenu)]
    public static void ValidateBlock1()
    {
        bool valid = true;
        TabsUI tabs = Object.FindFirstObjectByType<TabsUI>(FindObjectsInactive.Include);
        GameState gameState = Object.FindFirstObjectByType<GameState>(FindObjectsInactive.Include);

        if (tabs == null)
        {
            Debug.LogError("[D2 Block 1] Falta TabsUI.");
            valid = false;
        }
        else
        {
            valid &= Require(tabs.btnDimension2, "TabsUI.btnDimension2");
            valid &= Require(tabs.dimension2Panel, "TabsUI.dimension2Panel");
        }

        Dimension2PanelUI panel = Object.FindFirstObjectByType<Dimension2PanelUI>(
            FindObjectsInactive.Include
        );

        if (panel == null)
        {
            Debug.LogError("[D2 Block 1] Falta Dimension2PanelUI.");
            valid = false;
        }
        else
        {
            valid &= Require(panel.firstEntryRoot, "D2.firstEntryRoot");
            valid &= Require(panel.mapRoot, "D2.mapRoot");
            valid &= Require(panel.civilization1Root, "D2.civilization1Root");
            valid &= Require(panel.civilization2Root, "D2.civilization2Root");
            valid &= Require(panel.civilization3Root, "D2.civilization3Root");
            valid &= Require(panel.closeDimension2Button, "D2.closeDimension2Button");
            valid &= Require(panel.contextualHelpButton, "D2.contextualHelpButton");
            valid &= Require(panel.helpRoot, "D2.helpRoot");
            valid &= Require(panel.helpTitleText, "D2.helpTitleText");
            valid &= Require(panel.helpBodyText, "D2.helpBodyText");
            valid &= Require(panel.closeHelpButton, "D2.closeHelpButton");
            valid &= Require(panel.continueFirstEntryButton, "D2.continueFirstEntryButton");
            valid &= Require(panel.mapStatusText, "D2.mapStatusText");
            valid &= Require(panel.mapSecondaryStatusText, "D2.mapSecondaryStatusText");
            valid &= Require(panel.mapFollowersText, "D2.mapFollowersText");
            valid &= Require(panel.mapTrustText, "D2.mapTrustText");
            valid &= Require(panel.mapCivilization1TrustText, "D2.mapCivilization1TrustText");
            valid &= Require(panel.mapCivilization2RequirementText, "D2.mapCivilization2RequirementText");
            valid &= Require(panel.mapCivilization3RequirementText, "D2.mapCivilization3RequirementText");
            valid &= Require(panel.mapHelpButton, "D2.mapHelpButton");
            valid &= Require(panel.mapBackButton, "D2.mapBackButton");
            valid &= Require(panel.civilization1Button, "D2.civilization1Button");
            valid &= Require(panel.civilization2Button, "D2.civilization2Button");
            valid &= Require(panel.civilization3Button, "D2.civilization3Button");
            valid &= Require(panel.civilization1StateText, "D2.civilization1StateText");
            valid &= Require(panel.civilization2StateText, "D2.civilization2StateText");
            valid &= Require(panel.civilization3StateText, "D2.civilization3StateText");
            valid &= Require(panel.civilization1PlaceholderText, "D2.civilization1PlaceholderText");
            valid &= Require(panel.backToMapButton, "D2.backToMapButton");
            valid &= Require(panel.sanctuaryHelpButton, "D2.sanctuaryHelpButton");
            valid &= Require(panel.civilization1PanelUI, "D2.civilization1PanelUI");
            valid &= Require(panel.civilization2PanelUI, "D2.civilization2PanelUI");
            valid &= Require(panel.civilization3PanelUI, "D2.civilization3PanelUI");

            D2Civilization3PanelUI civilization3UI = panel.civilization3PanelUI;
            if (civilization3UI != null)
            {
                valid &= Require(civilization3UI.dimension2PanelUI, "D2 Civ3.dimension2PanelUI");
                valid &= Require(civilization3UI.archaeologySectionRoot, "D2 Civ3.archaeologySectionRoot");
                valid &= Require(civilization3UI.archiveSectionRoot, "D2 Civ3.archiveSectionRoot");
                valid &= Require(civilization3UI.entityResearchSectionRoot, "D2 Civ3.entityResearchSectionRoot");
                valid &= Require(civilization3UI.archivePanelUI, "D2 Civ3.archivePanelUI");
                valid &= Require(civilization3UI.entityResearchPanelUI, "D2 Civ3.entityResearchPanelUI");
                valid &= Require(civilization3UI.showEntityResearchButton, "D2 Civ3.showEntityResearchButton");
                valid &= Require(civilization3UI.showArchiveButton, "D2 Civ3.showArchiveButton");
                valid &= Require(civilization3UI.zoneText, "D2 Civ3.zoneText");
                valid &= Require(civilization3UI.lockedZonesText, "D2 Civ3.lockedZonesText");
                valid &= Require(civilization3UI.excavationText, "D2 Civ3.excavationText");
                valid &= Require(civilization3UI.excavationSlider, "D2 Civ3.excavationSlider");
                valid &= Require(civilization3UI.inventoryText, "D2 Civ3.inventoryText");
                valid &= Require(civilization3UI.analysisText, "D2 Civ3.analysisText");
                valid &= Require(civilization3UI.analysisSlider, "D2 Civ3.analysisSlider");
                valid &= Require(civilization3UI.researchText, "D2 Civ3.researchText");
                valid &= Require(civilization3UI.archiveText, "D2 Civ3.archiveText");
                valid &= Require(civilization3UI.cluesText, "D2 Civ3.cluesText");
                valid &= Require(civilization3UI.anomalyText, "D2 Civ3.anomalyText");
                valid &= Require(civilization3UI.scholarText, "D2 Civ3.scholarText");
                valid &= Require(civilization3UI.civilization1ResourcesText, "D2 Civ3.civilization1ResourcesText");
                valid &= Require(civilization3UI.lastResultText, "D2 Civ3.lastResultText");
                valid &= Require(civilization3UI.objectiveText, "D2 Civ3.objectiveText");
                valid &= Require(civilization3UI.excavateButton, "D2 Civ3.excavateButton");
                valid &= Require(civilization3UI.zone1Button, "D2 Civ3.zone1Button");
                valid &= Require(civilization3UI.zone2Button, "D2 Civ3.zone2Button");
                valid &= Require(civilization3UI.zone3Button, "D2 Civ3.zone3Button");
                valid &= Require(civilization3UI.unlockZone2Button, "D2 Civ3.unlockZone2Button");
                valid &= Require(civilization3UI.unlockZone3Button, "D2 Civ3.unlockZone3Button");
                valid &= Require(civilization3UI.analyzeLowButton, "D2 Civ3.analyzeLowButton");
                valid &= Require(civilization3UI.analyzeMediumButton, "D2 Civ3.analyzeMediumButton");
                valid &= Require(civilization3UI.analyzeHighButton, "D2 Civ3.analyzeHighButton");
                valid &= Require(civilization3UI.hireScholarButton, "D2 Civ3.hireScholarButton");
                valid &= Require(civilization3UI.readAnomalyButton, "D2 Civ3.readAnomalyButton");
                valid &= Require(civilization3UI.backToMapButton, "D2 Civ3.backToMapButton");

                D2EntityResearchPanelUI entityResearchUI = civilization3UI.entityResearchPanelUI;
                if (entityResearchUI != null)
                {
                    valid &= Require(entityResearchUI.civilization3PanelUI, "D2 Civ3 Entity.civilization3PanelUI");
                    valid &= Require(entityResearchUI.unlockText, "D2 Civ3 Entity.unlockText");
                    valid &= Require(entityResearchUI.statusText, "D2 Civ3 Entity.statusText");
                    valid &= Require(entityResearchUI.progressText, "D2 Civ3 Entity.progressText");
                    valid &= Require(entityResearchUI.progressSlider, "D2 Civ3 Entity.progressSlider");
                    valid &= Require(entityResearchUI.milestoneText, "D2 Civ3 Entity.milestoneText");
                    valid &= Require(entityResearchUI.resourcesText, "D2 Civ3 Entity.resourcesText");
                    valid &= Require(entityResearchUI.entityKnowledgeText, "D2 Civ3 Entity.entityKnowledgeText");
                    valid &= Require(entityResearchUI.lastResultText, "D2 Civ3 Entity.lastResultText");
                    valid &= Require(entityResearchUI.startPauseButton, "D2 Civ3 Entity.startPauseButton");
                    valid &= Require(entityResearchUI.completeMilestoneButton, "D2 Civ3 Entity.completeMilestoneButton");
                    valid &= Require(entityResearchUI.resonantExpeditionButton, "D2 Civ3 Entity.resonantExpeditionButton");
                    valid &= Require(entityResearchUI.endlessArchiveButton, "D2 Civ3 Entity.endlessArchiveButton");
                    valid &= Require(entityResearchUI.sharedMemoryButton, "D2 Civ3 Entity.sharedMemoryButton");
                    valid &= Require(entityResearchUI.modulatorResonanceButton, "D2 Civ3 Entity.modulatorResonanceButton");
                    valid &= Require(entityResearchUI.firstThresholdChronicleButton, "D2 Civ3 Entity.firstThresholdChronicleButton");
                    valid &= Require(entityResearchUI.backToArchaeologyButton, "D2 Civ3 Entity.backToArchaeologyButton");
                    valid &= Require(entityResearchUI.backToMapButton, "D2 Civ3 Entity.backToMapButton");
                    valid &= Require(entityResearchUI.researchPhaseVisualRoot, "D2 Civ3 Entity.researchPhaseVisualRoot");
                    valid &= Require(entityResearchUI.pactPhaseVisualRoot, "D2 Civ3 Entity.pactPhaseVisualRoot");
                    valid &= Require(entityResearchUI.unlockDetailText, "D2 Civ3 Entity.unlockDetailText");
                    valid &= Require(entityResearchUI.progressGaugeText, "D2 Civ3 Entity.progressGaugeText");
                    valid &= Require(entityResearchUI.ancientKnowledgeValueText, "D2 Civ3 Entity.ancientKnowledgeValueText");
                    valid &= Require(entityResearchUI.progressRadialGraphic, "D2 Civ3 Entity.progressRadialGraphic");
                    valid &= Require(entityResearchUI.milestoneGaugeText, "D2 Civ3 Entity.milestoneGaugeText");
                    valid &= Require(entityResearchUI.milestoneCostText, "D2 Civ3 Entity.milestoneCostText");
                    valid &= Require(entityResearchUI.milestoneRewardText, "D2 Civ3 Entity.milestoneRewardText");
                    valid &= RequireArray(entityResearchUI.milestoneSelectionRoots, 4, "D2 Civ3 Entity.milestoneSelectionRoots");
                    valid &= RequireArray(entityResearchUI.milestoneNodeTexts, 4, "D2 Civ3 Entity.milestoneNodeTexts");
                    valid &= RequireArray(entityResearchUI.milestoneCardTitleTexts, 4, "D2 Civ3 Entity.milestoneCardTitleTexts");
                    valid &= Require(entityResearchUI.fragmentsValueText, "D2 Civ3 Entity.fragmentsValueText");
                    valid &= Require(entityResearchUI.inscriptionsValueText, "D2 Civ3 Entity.inscriptionsValueText");
                    valid &= Require(entityResearchUI.sealsValueText, "D2 Civ3 Entity.sealsValueText");
                    valid &= Require(entityResearchUI.basicDataValueText, "D2 Civ3 Entity.basicDataValueText");
                    valid &= Require(entityResearchUI.symbolicDataValueText, "D2 Civ3 Entity.symbolicDataValueText");
                    valid &= Require(entityResearchUI.deepDataValueText, "D2 Civ3 Entity.deepDataValueText");
                    valid &= Require(entityResearchUI.entityKnowledgeValueText, "D2 Civ3 Entity.entityKnowledgeValueText");
                    valid &= RequireArray(entityResearchUI.entityKnowledgeFillRoots, 6, "D2 Civ3 Entity.entityKnowledgeFillRoots");
                    valid &= Require(entityResearchUI.objectiveText, "D2 Civ3 Entity.objectiveText");
                    valid &= Require(entityResearchUI.startPauseActionText, "D2 Civ3 Entity.startPauseActionText");
                    valid &= Require(entityResearchUI.milestoneActionText, "D2 Civ3 Entity.milestoneActionText");
                    valid &= Require(entityResearchUI.startPauseHighlightImage, "D2 Civ3 Entity.startPauseHighlightImage");
                    valid &= Require(entityResearchUI.milestoneHighlightImage, "D2 Civ3 Entity.milestoneHighlightImage");
                    valid &= Require(entityResearchUI.excavateNavigationButton, "D2 Civ3 Entity.excavateNavigationButton");
                    valid &= Require(entityResearchUI.analyzeNavigationButton, "D2 Civ3 Entity.analyzeNavigationButton");
                    valid &= Require(entityResearchUI.archiveNavigationButton, "D2 Civ3 Entity.archiveNavigationButton");
                    valid &= Require(entityResearchUI.pactStatusText, "D2 Civ3 Entity.pactStatusText");
                    valid &= Require(entityResearchUI.pactProgressText, "D2 Civ3 Entity.pactProgressText");
                    valid &= Require(entityResearchUI.pactAncientKnowledgeText, "D2 Civ3 Entity.pactAncientKnowledgeText");
                    valid &= RequireArray(entityResearchUI.pactCardTitleTexts, 4, "D2 Civ3 Entity.pactCardTitleTexts");
                    valid &= RequireArray(entityResearchUI.pactLevelTexts, 4, "D2 Civ3 Entity.pactLevelTexts");
                    valid &= RequireArray(entityResearchUI.pactSelectionRoots, 4, "D2 Civ3 Entity.pactSelectionRoots");
                    valid &= RequireArray(entityResearchUI.pactDetailIconRoots, 4, "D2 Civ3 Entity.pactDetailIconRoots");
                    valid &= Require(entityResearchUI.pactDetailTitleText, "D2 Civ3 Entity.pactDetailTitleText");
                    valid &= Require(entityResearchUI.pactDetailEffectText, "D2 Civ3 Entity.pactDetailEffectText");
                    valid &= Require(entityResearchUI.pactDetailCostText, "D2 Civ3 Entity.pactDetailCostText");
                    valid &= Require(entityResearchUI.pactThresholdText, "D2 Civ3 Entity.pactThresholdText");
                    valid &= Require(entityResearchUI.pactFragmentsValueText, "D2 Civ3 Entity.pactFragmentsValueText");
                    valid &= Require(entityResearchUI.pactInscriptionsValueText, "D2 Civ3 Entity.pactInscriptionsValueText");
                    valid &= Require(entityResearchUI.pactSealsValueText, "D2 Civ3 Entity.pactSealsValueText");
                    valid &= Require(entityResearchUI.pactEntityKnowledgeValueText, "D2 Civ3 Entity.pactEntityKnowledgeValueText");
                    valid &= Require(entityResearchUI.pactEstablishActionText, "D2 Civ3 Entity.pactEstablishActionText");
                    valid &= Require(entityResearchUI.pactUpgradeActionText, "D2 Civ3 Entity.pactUpgradeActionText");
                    valid &= Require(entityResearchUI.pactLastResultText, "D2 Civ3 Entity.pactLastResultText");
                    valid &= Require(entityResearchUI.pactNavigationLabelText, "D2 Civ3 Entity.pactNavigationLabelText");
                    valid &= Require(entityResearchUI.pactEstablishHighlightImage, "D2 Civ3 Entity.pactEstablishHighlightImage");
                    valid &= Require(entityResearchUI.pactUpgradeHighlightImage, "D2 Civ3 Entity.pactUpgradeHighlightImage");
                    valid &= Require(entityResearchUI.pactNavigationSelectionRoot, "D2 Civ3 Entity.pactNavigationSelectionRoot");
                    valid &= Require(entityResearchUI.pactEstablishButton, "D2 Civ3 Entity.pactEstablishButton");
                    valid &= Require(entityResearchUI.pactUpgradeButton, "D2 Civ3 Entity.pactUpgradeButton");
                    valid &= Require(entityResearchUI.pactBackToArchaeologyButton, "D2 Civ3 Entity.pactBackToArchaeologyButton");
                    valid &= Require(entityResearchUI.pactExcavateNavigationButton, "D2 Civ3 Entity.pactExcavateNavigationButton");
                    valid &= Require(entityResearchUI.pactAnalyzeNavigationButton, "D2 Civ3 Entity.pactAnalyzeNavigationButton");
                    valid &= Require(entityResearchUI.pactArchiveNavigationButton, "D2 Civ3 Entity.pactArchiveNavigationButton");
                }

                D2ArchivePanelUI archiveUI = civilization3UI.archivePanelUI;
                if (archiveUI != null)
                {
                    valid &= Require(archiveUI.civilization3PanelUI, "D2 Civ3 Archivo.civilization3PanelUI");
                    valid &= Require(archiveUI.stateText, "D2 Civ3 Archivo.stateText");
                    valid &= Require(archiveUI.resourcesText, "D2 Civ3 Archivo.resourcesText");
                    valid &= Require(archiveUI.cartographyText, "D2 Civ3 Archivo.cartographyText");
                    valid &= Require(archiveUI.concordanceText, "D2 Civ3 Archivo.concordanceText");
                    valid &= Require(archiveUI.exegesisText, "D2 Civ3 Archivo.exegesisText");
                    valid &= Require(archiveUI.lastResultText, "D2 Civ3 Archivo.lastResultText");
                    valid &= Require(archiveUI.cartographyButton, "D2 Civ3 Archivo.cartographyButton");
                    valid &= Require(archiveUI.concordanceButton, "D2 Civ3 Archivo.concordanceButton");
                    valid &= Require(archiveUI.exegesisButton, "D2 Civ3 Archivo.exegesisButton");
                    valid &= Require(archiveUI.backToArchaeologyButton, "D2 Civ3 Archivo.backToArchaeologyButton");
                    valid &= Require(archiveUI.levelText, "D2 Civ3 Archivo.levelText");
                    valid &= Require(archiveUI.ancientKnowledgeValueText, "D2 Civ3 Archivo.ancientKnowledgeValueText");
                    valid &= Require(archiveUI.entityKnowledgeValueText, "D2 Civ3 Archivo.entityKnowledgeValueText");
                    valid &= Require(archiveUI.fragmentsValueText, "D2 Civ3 Archivo.fragmentsValueText");
                    valid &= Require(archiveUI.inscriptionsValueText, "D2 Civ3 Archivo.inscriptionsValueText");
                    valid &= Require(archiveUI.sealsValueText, "D2 Civ3 Archivo.sealsValueText");
                    valid &= RequireArray(archiveUI.actionTexts, 3, "D2 Civ3 Archivo.actionTexts");
                    valid &= RequireArray(archiveUI.upgradeStateTexts, 3, "D2 Civ3 Archivo.upgradeStateTexts");
                    valid &= RequireArray(archiveUI.actionHighlightImages, 3, "D2 Civ3 Archivo.actionHighlightImages");
                    valid &= RequireArray(archiveUI.lockIconRoots, 3, "D2 Civ3 Archivo.lockIconRoots");
                    valid &= Require(archiveUI.excavateNavigationButton, "D2 Civ3 Archivo.excavateNavigationButton");
                    valid &= Require(archiveUI.analyzeNavigationButton, "D2 Civ3 Archivo.analyzeNavigationButton");
                    valid &= Require(archiveUI.entityNavigationButton, "D2 Civ3 Archivo.entityNavigationButton");
                }
            }

            D2Civilization2PanelUI civilization2UI = panel.civilization2PanelUI;
            if (civilization2UI != null)
            {
                valid &= Require(civilization2UI.dimension2PanelUI, "D2 Civ2.dimension2PanelUI");
                valid &= Require(civilization2UI.regionSectionRoot, "D2 Civ2.regionSectionRoot");
                valid &= Require(civilization2UI.operationsSectionRoot, "D2 Civ2.operationsSectionRoot");
                valid &= Require(civilization2UI.defenseSectionRoot, "D2 Civ2.defenseSectionRoot");
                valid &= Require(civilization2UI.resistanceSectionRoot, "D2 Civ2.resistanceSectionRoot");
                valid &= Require(civilization2UI.alertSectionRoot, "D2 Civ2.alertSectionRoot");
                valid &= Require(civilization2UI.containmentSectionRoot, "D2 Civ2.containmentSectionRoot");
                valid &= Require(civilization2UI.showRegionsButton, "D2 Civ2.showRegionsButton");
                valid &= Require(civilization2UI.showOperationsButton, "D2 Civ2.showOperationsButton");
                valid &= Require(civilization2UI.showDefenseButton, "D2 Civ2.showDefenseButton");
                valid &= Require(civilization2UI.showResistanceButton, "D2 Civ2.showResistanceButton");
                valid &= Require(civilization2UI.showAlertButton, "D2 Civ2.showAlertButton");
                valid &= Require(civilization2UI.showContainmentButton, "D2 Civ2.showContainmentButton");
                valid &= Require(civilization2UI.regionDropdown, "D2 Civ2.regionDropdown");
                valid &= Require(civilization2UI.operationsPanelUI, "D2 Civ2.operationsPanelUI");
                valid &= Require(civilization2UI.reprisalsPanelUI, "D2 Civ2.reprisalsPanelUI");
                valid &= Require(civilization2UI.resistancePanelUI, "D2 Civ2.resistancePanelUI");
                valid &= Require(civilization2UI.alertPanelUI, "D2 Civ2.alertPanelUI");
                valid &= Require(civilization2UI.containmentPanelUI, "D2 Civ2.containmentPanelUI");
                valid &= Require(civilization2UI.membersText, "D2 Civ2.membersText");
                valid &= Require(civilization2UI.dominanceText, "D2 Civ2.dominanceText");
                valid &= Require(civilization2UI.dominanceSlider, "D2 Civ2.dominanceSlider");
                valid &= Require(civilization2UI.region1Text, "D2 Civ2.region1Text");
                valid &= Require(civilization2UI.region2Text, "D2 Civ2.region2Text");
                valid &= Require(civilization2UI.region3Text, "D2 Civ2.region3Text");
                valid &= Require(civilization2UI.region4Text, "D2 Civ2.region4Text");
                valid &= Require(civilization2UI.assignmentText, "D2 Civ2.assignmentText");
                valid &= Require(civilization2UI.lastResultText, "D2 Civ2.lastResultText");
                valid &= Require(civilization2UI.assignOneButton, "D2 Civ2.assignOneButton");
                valid &= Require(civilization2UI.assignTenButton, "D2 Civ2.assignTenButton");
                valid &= Require(civilization2UI.assignAllButton, "D2 Civ2.assignAllButton");
                valid &= Require(civilization2UI.releaseOneButton, "D2 Civ2.releaseOneButton");
                valid &= Require(civilization2UI.releaseAllButton, "D2 Civ2.releaseAllButton");
                valid &= Require(civilization2UI.backToMapButton, "D2 Civ2.backToMapButton");
                valid &= Require(civilization2UI.helpButton, "D2 Civ2.helpButton");
                valid &= Require(civilization2UI.availableMembersValueText,
                    "D2 Civ2.availableMembersValueText");
                valid &= Require(civilization2UI.assignedMembersValueText,
                    "D2 Civ2.assignedMembersValueText");
                valid &= Require(civilization2UI.totalMembersValueText,
                    "D2 Civ2.totalMembersValueText");
                valid &= Require(civilization2UI.totalDominanceValueText,
                    "D2 Civ2.totalDominanceValueText");
                valid &= Require(civilization2UI.detailTitleText, "D2 Civ2.detailTitleText");
                valid &= Require(civilization2UI.detailIdleValueText,
                    "D2 Civ2.detailIdleValueText");
                if (civilization2UI.regionButtons == null ||
                    civilization2UI.regionButtons.Length != 3 ||
                    civilization2UI.regionNameTexts == null ||
                    civilization2UI.regionNameTexts.Length != 3 ||
                    civilization2UI.regionDominanceValueTexts == null ||
                    civilization2UI.regionDominanceValueTexts.Length != 3 ||
                    civilization2UI.regionThreatValueTexts == null ||
                    civilization2UI.regionThreatValueTexts.Length != 3 ||
                    civilization2UI.regionMembersValueTexts == null ||
                    civilization2UI.regionMembersValueTexts.Length != 3)
                {
                    Debug.LogError("[D2 Resistance Regions] La matriz visual regional no contiene 3 entradas.");
                    valid = false;
                }

                D2OperationsPanelUI operationsUI = civilization2UI.operationsPanelUI;
                if (operationsUI != null)
                {
                    valid &= Require(operationsUI.civilization2PanelUI, "D2 Operaciones.civilization2PanelUI");
                    valid &= Require(operationsUI.operationDropdown, "D2 Operaciones.operationDropdown");
                    valid &= Require(operationsUI.operationStateText, "D2 Operaciones.operationStateText");
                    valid &= Require(operationsUI.effectText, "D2 Operaciones.effectText");
                    valid &= Require(operationsUI.regionalMembersText, "D2 Operaciones.regionalMembersText");
                    valid &= Require(operationsUI.assignmentText, "D2 Operaciones.assignmentText");
                    valid &= Require(operationsUI.assignOneButton, "D2 Operaciones.assignOneButton");
                    valid &= Require(operationsUI.assignFiveButton, "D2 Operaciones.assignFiveButton");
                    valid &= Require(operationsUI.assignAllButton, "D2 Operaciones.assignAllButton");
                    valid &= Require(operationsUI.releaseOneButton, "D2 Operaciones.releaseOneButton");
                    valid &= Require(operationsUI.releaseAllButton, "D2 Operaciones.releaseAllButton");
                    valid &= Require(operationsUI.regionNameText, "D2 Operaciones.regionNameText");
                    valid &= Require(operationsUI.regionalIdleValueText, "D2 Operaciones.regionalIdleValueText");
                    valid &= Require(operationsUI.regionalOperationsValueText,
                        "D2 Operaciones.regionalOperationsValueText");
                    valid &= Require(operationsUI.detailNameText, "D2 Operaciones.detailNameText");
                    valid &= Require(operationsUI.detailStateText, "D2 Operaciones.detailStateText");
                    valid &= Require(operationsUI.detailDescriptionText,
                        "D2 Operaciones.detailDescriptionText");
                    valid &= Require(operationsUI.assignmentValueText,
                        "D2 Operaciones.assignmentValueText");
                    if (operationsUI.operationButtons == null ||
                        operationsUI.operationButtons.Length != 4 ||
                        operationsUI.operationSelectionOverlays == null ||
                        operationsUI.operationSelectionOverlays.Length != 4 ||
                        operationsUI.operationStateTexts == null ||
                        operationsUI.operationStateTexts.Length != 4 ||
                        operationsUI.operationAssignedValueTexts == null ||
                        operationsUI.operationAssignedValueTexts.Length != 4 ||
                        operationsUI.operationRequirementValueTexts == null ||
                        operationsUI.operationRequirementValueTexts.Length != 4)
                    {
                        Debug.LogError("[D2 Resistance Operations] La matriz visual no contiene 4 operaciones.");
                        valid = false;
                    }
                }

                D2ReprisalsPanelUI reprisalsUI = civilization2UI.reprisalsPanelUI;
                if (reprisalsUI != null)
                {
                    valid &= Require(reprisalsUI.civilization2PanelUI, "D2 Defensa.civilization2PanelUI");
                    valid &= Require(reprisalsUI.availableMembersText, "D2 Defensa.availableMembersText");
                    valid &= Require(reprisalsUI.fragmentsText, "D2 Defensa.fragmentsText");
                    valid &= Require(reprisalsUI.reprisalsCountText, "D2 Defensa.reprisalsCountText");
                    valid &= Require(reprisalsUI.regionNameText, "D2 Defensa.regionNameText");
                    valid &= Require(reprisalsUI.regionSelectorButton, "D2 Defensa.regionSelectorButton");
                    valid &= Require(reprisalsUI.threatText, "D2 Defensa.threatText");
                    valid &= Require(reprisalsUI.threatSlider, "D2 Defensa.threatSlider");
                    valid &= Require(reprisalsUI.threatGauge, "D2 Defensa.threatGauge");
                    valid &= Require(reprisalsUI.coverageText, "D2 Defensa.coverageText");
                    valid &= Require(reprisalsUI.coverageSlider, "D2 Defensa.coverageSlider");
                    valid &= Require(reprisalsUI.coverageGauge, "D2 Defensa.coverageGauge");
                    valid &= Require(reprisalsUI.estimatedLossText, "D2 Defensa.estimatedLossText");
                    valid &= Require(reprisalsUI.protectionText, "D2 Defensa.protectionText");
                    valid &= Require(reprisalsUI.weakeningText, "D2 Defensa.weakeningText");
                    valid &= Require(reprisalsUI.weakeningDurationText, "D2 Defensa.weakeningDurationText");
                    valid &= Require(reprisalsUI.rulesText, "D2 Defensa.rulesText");
                    valid &= Require(reprisalsUI.lastResultText, "D2 Defensa.lastResultText");
                }

                D2ResistancePanelUI resistanceUI = civilization2UI.resistancePanelUI;
                if (resistanceUI != null)
                {
                    valid &= Require(resistanceUI.availableMembersText, "D2 RED.availableMembersText");
                    valid &= Require(resistanceUI.fragmentsText, "D2 RED.fragmentsText");
                    valid &= RequireArray(resistanceUI.upgradeCardRoots, 4, "D2 RED.upgradeCardRoots");
                    valid &= RequireArray(resistanceUI.upgradeCardButtons, 4, "D2 RED.upgradeCardButtons");
                    valid &= RequireArray(resistanceUI.upgradeLevelTexts, 4, "D2 RED.upgradeLevelTexts");
                    valid &= RequireArray(resistanceUI.upgradeCostTexts, 4, "D2 RED.upgradeCostTexts");
                    valid &= RequireArray(resistanceUI.upgradeCostNormalRoots, 4, "D2 RED.upgradeCostNormalRoots");
                    valid &= RequireArray(resistanceUI.upgradeMaxTexts, 4, "D2 RED.upgradeMaxTexts");
                    valid &= RequireArray(resistanceUI.pactCardRoots, 3, "D2 RED.pactCardRoots");
                    valid &= RequireArray(resistanceUI.pactCardButtons, 3, "D2 RED.pactCardButtons");
                    valid &= RequireArray(resistanceUI.pactSelectionRoots, 3, "D2 RED.pactSelectionRoots");
                    valid &= RequireArray(resistanceUI.pactStateTexts, 3, "D2 RED.pactStateTexts");
                    valid &= RequireArray(resistanceUI.pactMembersOrRequirementTexts, 3, "D2 RED.pactMembersOrRequirementTexts");
                    valid &= RequireArray(resistanceUI.pactEffectTexts, 3, "D2 RED.pactEffectTexts");
                    valid &= RequireArray(resistanceUI.pactWearTexts, 3, "D2 RED.pactWearTexts");
                    valid &= RequireArray(resistanceUI.pactActiveMemberIcons, 3, "D2 RED.pactActiveMemberIcons");
                    valid &= RequireArray(resistanceUI.pactInactiveLockIcons, 3, "D2 RED.pactInactiveLockIcons");
                    valid &= RequireArray(resistanceUI.pactEffectIcons, 3, "D2 RED.pactEffectIcons");
                    valid &= RequireArray(resistanceUI.pactWearIcons, 3, "D2 RED.pactWearIcons");
                    valid &= Require(resistanceUI.pactDetailTitleText, "D2 RED.pactDetailTitleText");
                    valid &= Require(resistanceUI.pactMembersText, "D2 RED.pactMembersText");
                    valid &= Require(resistanceUI.pactWearDetailText, "D2 RED.pactWearDetailText");
                    valid &= Require(resistanceUI.exhaustedText, "D2 RED.exhaustedText");
                    valid &= Require(resistanceUI.penaltiesText, "D2 RED.penaltiesText");
                    valid &= Require(resistanceUI.pactAdjustRoot, "D2 RED.pactAdjustRoot");
                    valid &= Require(resistanceUI.noPenaltyIconRoot, "D2 RED.noPenaltyIconRoot");
                    valid &= Require(resistanceUI.activateButton, "D2 RED.activateButton");
                    valid &= Require(resistanceUI.reinforceOneButton, "D2 RED.reinforceOneButton");
                    valid &= Require(resistanceUI.reinforceTenButton, "D2 RED.reinforceTenButton");
                    valid &= Require(resistanceUI.cancelButton, "D2 RED.cancelButton");
                }

                D2AlertPanelUI alertUI = civilization2UI.alertPanelUI;
                if (alertUI != null)
                {
                    valid &= Require(alertUI.stateText, "D2 Alerta.stateText");
                    valid &= Require(alertUI.dominanceText, "D2 Alerta.dominanceText");
                    valid &= Require(alertUI.timerText, "D2 Alerta.timerText");
                    valid &= Require(alertUI.effectsText, "D2 Alerta.effectsText");
                    valid &= Require(alertUI.regionsText, "D2 Alerta.regionsText");
                    valid &= Require(alertUI.unlocksText, "D2 Alerta.unlocksText");
                    valid &= Require(alertUI.lastResultText, "D2 Alerta.lastResultText");
                }

                D2ContainmentPanelUI containmentUI = civilization2UI.containmentPanelUI;
                if (containmentUI != null)
                {
                    valid &= Require(containmentUI.containmentAttemptRoot, "D2 Contención.containmentAttemptRoot");
                    valid &= Require(containmentUI.majorPactRoot, "D2 Contención.majorPactRoot");
                    valid &= Require(containmentUI.stateText, "D2 Contención.stateText");
                    valid &= Require(containmentUI.probabilityText, "D2 Contención.probabilityText");
                    valid &= Require(containmentUI.cooldownText, "D2 Contención.cooldownText");
                    valid &= Require(containmentUI.rulesText, "D2 Contención.rulesText");
                    valid &= Require(containmentUI.assignmentText, "D2 Contención.assignmentText");
                    valid &= Require(containmentUI.attemptsText, "D2 Contención.attemptsText");
                    valid &= Require(containmentUI.lastResultText, "D2 Contención.lastResultText");
                    valid &= Require(containmentUI.attemptButton, "D2 Contención.attemptButton");
                    valid &= Require(containmentUI.assignOneButton, "D2 Contención.assignOneButton");
                    valid &= Require(containmentUI.assignTenButton, "D2 Contención.assignTenButton");
                    valid &= Require(containmentUI.assignAllButton, "D2 Contención.assignAllButton");
                    valid &= Require(containmentUI.releaseOneButton, "D2 Contención.releaseOneButton");
                    valid &= Require(containmentUI.releaseAllButton, "D2 Contención.releaseAllButton");
                    valid &= Require(containmentUI.majorPactStateText, "D2 Pacto Mayor Civ2.majorPactStateText");
                    valid &= Require(containmentUI.majorPactMilestoneText, "D2 Pacto Mayor Civ2.majorPactMilestoneText");
                    valid &= Require(containmentUI.majorPactAvailableMembersText, "D2 Pacto Mayor Civ2.availableMembersText");
                    valid &= Require(containmentUI.majorPactAssignedHeaderText, "D2 Pacto Mayor Civ2.assignedHeaderText");
                    valid &= Require(containmentUI.stabilityText, "D2 Pacto Mayor Civ2.stabilityText");
                    valid &= Require(containmentUI.majorPactFragmentsHeaderText, "D2 Pacto Mayor Civ2.fragmentsHeaderText");
                    valid &= Require(containmentUI.majorPactLineDropdown, "D2 Pacto Mayor Civ2.majorPactLineDropdown");
                    valid &= Require(containmentUI.majorPactLineText, "D2 Pacto Mayor Civ2.majorPactLineText");
                    valid &= Require(containmentUI.majorPactDetailEffectText, "D2 Pacto Mayor Civ2.detailEffectText");
                    valid &= Require(containmentUI.majorPactDetailCostText, "D2 Pacto Mayor Civ2.detailCostText");
                    valid &= Require(containmentUI.majorPactAssignedValueText, "D2 Pacto Mayor Civ2.assignedValueText");
                    valid &= Require(containmentUI.majorPactFooterTitleText, "D2 Pacto Mayor Civ2.footerTitleText");
                    valid &= Require(containmentUI.majorPactLastResultText, "D2 Pacto Mayor Civ2.majorPactLastResultText");
                    valid &= RequireArray(containmentUI.majorPactLineLevelTexts, 5, "D2 Pacto Mayor Civ2.lineLevelTexts");
                    valid &= RequireArray(containmentUI.majorPactLineButtons, 5, "D2 Pacto Mayor Civ2.lineButtons");
                    valid &= Require(containmentUI.establishMajorPactButton, "D2 Pacto Mayor Civ2.establishMajorPactButton");
                    valid &= Require(containmentUI.upgradeMajorPactLineButton, "D2 Pacto Mayor Civ2.upgradeMajorPactLineButton");
                }
            }

            if (civilization3UI != null)
            {
                Canvas.ForceUpdateCanvases();
                valid &= ValidateDirectChildrenInside(
                    civilization3UI.GetComponent<RectTransform>(),
                    "Civilización 3 general"
                );
                valid &= ValidateDirectChildrenInside(
                    civilization3UI.archaeologySectionRoot.GetComponent<RectTransform>(),
                    "Civilización 3 Arqueología"
                );
                valid &= ValidateDirectChildrenInside(
                    civilization3UI.archiveSectionRoot.GetComponent<RectTransform>(),
                    "Civilización 3 Archivo"
                );
                valid &= ValidateDirectChildrenInside(
                    civilization3UI.entityResearchSectionRoot.GetComponent<RectTransform>(),
                    "Civilización 3 Ente/Pacto"
                );
            }

            D2Civilization1PanelUI civilization1UI = panel.civilization1PanelUI;
            if (civilization1UI != null)
            {
                valid &= Require(civilization1UI.refugeSectionRoot, "D2 Civ1.refugeSectionRoot");
                valid &= Require(civilization1UI.altarsSectionRoot, "D2 Civ1.altarsSectionRoot");
                valid &= Require(civilization1UI.pilgrimagesSectionRoot, "D2 Civ1.pilgrimagesSectionRoot");
                valid &= Require(civilization1UI.novitiateSectionRoot, "D2 Civ1.novitiateSectionRoot");
                valid &= Require(civilization1UI.ritesSectionRoot, "D2 Civ1.ritesSectionRoot");
                valid &= Require(civilization1UI.pactsSectionRoot, "D2 Civ1.pactsSectionRoot");
                valid &= Require(civilization1UI.veiledThresholdSectionRoot, "D2 Civ1.veiledThresholdSectionRoot");
                valid &= Require(civilization1UI.showRefugeButton, "D2 Civ1.showRefugeButton");
                valid &= Require(civilization1UI.showAltarsButton, "D2 Civ1.showAltarsButton");
                valid &= Require(civilization1UI.showPilgrimagesButton, "D2 Civ1.showPilgrimagesButton");
                valid &= Require(civilization1UI.showNovitiateButton, "D2 Civ1.showNovitiateButton");
                valid &= Require(civilization1UI.showRitesButton, "D2 Civ1.showRitesButton");
                valid &= Require(civilization1UI.showPactsButton, "D2 Civ1.showPactsButton");
                valid &= Require(civilization1UI.showVeiledThresholdButton, "D2 Civ1.showVeiledThresholdButton");
                valid &= Require(civilization1UI.altarsPanelUI, "D2 Civ1.altarsPanelUI");
                valid &= Require(civilization1UI.pilgrimagesPanelUI, "D2 Civ1.pilgrimagesPanelUI");
                valid &= Require(civilization1UI.novitiatePanelUI, "D2 Civ1.novitiatePanelUI");
                valid &= Require(civilization1UI.ritesPanelUI, "D2 Civ1.ritesPanelUI");
                valid &= Require(civilization1UI.pactsPanelUI, "D2 Civ1.pactsPanelUI");
                valid &= Require(civilization1UI.veiledThresholdPanelUI, "D2 Civ1.veiledThresholdPanelUI");
                valid &= Require(civilization1UI.followersText, "D2 Civ1.followersText");
                valid &= Require(civilization1UI.arrivalText, "D2 Civ1.arrivalText");
                valid &= Require(civilization1UI.refugeText, "D2 Civ1.refugeText");
                valid &= Require(civilization1UI.assignmentText, "D2 Civ1.assignmentText");
                valid &= Require(civilization1UI.trustText, "D2 Civ1.trustText");
                valid &= Require(civilization1UI.waxText, "D2 Civ1.waxText");
                valid &= Require(civilization1UI.ritualBreadText, "D2 Civ1.ritualBreadText");
                valid &= Require(civilization1UI.followersAvailableText, "D2 Civ1.followersAvailableText");
                valid &= Require(civilization1UI.multiplierText, "D2 Civ1.multiplierText");
                valid &= Require(civilization1UI.arrivalProgressSlider, "D2 Civ1.arrivalProgressSlider");
                valid &= Require(civilization1UI.assignOneButton, "D2 Civ1.assignOneButton");
                valid &= Require(civilization1UI.assignTenButton, "D2 Civ1.assignTenButton");
                valid &= Require(civilization1UI.assignAllButton, "D2 Civ1.assignAllButton");
                valid &= Require(civilization1UI.releaseOneButton, "D2 Civ1.releaseOneButton");
                valid &= Require(civilization1UI.releaseAllButton, "D2 Civ1.releaseAllButton");
                valid &= Require(civilization1UI.upgradeRefugeButton, "D2 Civ1.upgradeRefugeButton");
                valid &= Require(civilization1UI.upgradeRefugeButtonText, "D2 Civ1.upgradeRefugeButtonText");

                D2AltarsPanelUI altarsUI = civilization1UI.altarsPanelUI;
                if (altarsUI != null)
                {
                    valid &= Require(altarsUI.altarDropdown, "D2 Altares.altarDropdown");
                    valid &= Require(altarsUI.detailTitleText, "D2 Altares.detailTitleText");
                    valid &= Require(altarsUI.offeringValueText, "D2 Altares.offeringValueText");
                    valid &= Require(altarsUI.productionPerSecondText, "D2 Altares.productionPerSecondText");
                    valid &= Require(altarsUI.followersAssignedText, "D2 Altares.followersAssignedText");
                    valid &= Require(altarsUI.waxCardButton, "D2 Altares.waxCardButton");
                    valid &= Require(altarsUI.breadCardButton, "D2 Altares.breadCardButton");
                    valid &= Require(altarsUI.incenseCardButton, "D2 Altares.incenseCardButton");
                    valid &= Require(altarsUI.clothCardButton, "D2 Altares.clothCardButton");
                    valid &= Require(altarsUI.stoneCardButton, "D2 Altares.stoneCardButton");
                    valid &= Require(altarsUI.assignOneButton, "D2 Altares.assignOneButton");
                    valid &= Require(altarsUI.assignTenButton, "D2 Altares.assignTenButton");
                    valid &= Require(altarsUI.assignAllButton, "D2 Altares.assignAllButton");
                    valid &= Require(altarsUI.releaseOneButton, "D2 Altares.releaseOneButton");
                    valid &= Require(altarsUI.releaseAllButton, "D2 Altares.releaseAllButton");
                }

                D2PilgrimagesPanelUI pilgrimagesUI = civilization1UI.pilgrimagesPanelUI;
                if (pilgrimagesUI != null)
                {
                    valid &= Require(pilgrimagesUI.trustText, "D2 Peregrinaciones.trustText");
                    valid &= Require(pilgrimagesUI.trustSlider, "D2 Peregrinaciones.trustSlider");
                    valid &= Require(pilgrimagesUI.resourcesText, "D2 Peregrinaciones.resourcesText");
                    valid &= Require(pilgrimagesUI.activePilgrimageText, "D2 Peregrinaciones.activeText");
                    valid &= Require(pilgrimagesUI.supportText, "D2 Peregrinaciones.supportText");
                    valid &= Require(pilgrimagesUI.addSupportButton, "D2 Peregrinaciones.addSupportButton");
                    valid &= Require(pilgrimagesUI.removeSupportButton, "D2 Peregrinaciones.removeSupportButton");
                    valid &= Require(pilgrimagesUI.lastResultText, "D2 Peregrinaciones.lastResultText");
                    valid &= Require(pilgrimagesUI.startShortButton, "D2 Peregrinaciones.shortButton");
                    valid &= Require(pilgrimagesUI.startMediumButton, "D2 Peregrinaciones.mediumButton");
                    valid &= Require(pilgrimagesUI.startLongButton, "D2 Peregrinaciones.longButton");
                    valid &= Require(pilgrimagesUI.startGuidedLongButton, "D2 Peregrinaciones.guidedLongButton");
                    valid &= Require(pilgrimagesUI.startSacredButton, "D2 Peregrinaciones.sacredButton");
                    valid &= Require(pilgrimagesUI.cancelButton, "D2 Peregrinaciones.cancelButton");
                }

                D2NovitiatePanelUI novitiateUI = civilization1UI.novitiatePanelUI;
                if (novitiateUI != null)
                {
                    valid &= Require(novitiateUI.acolytesText, "D2 Noviciado.acolytesText");
                    valid &= Require(novitiateUI.resourcesText, "D2 Noviciado.resourcesText");
                    valid &= Require(novitiateUI.batchText, "D2 Noviciado.batchText");
                    valid &= Require(novitiateUI.activeTrainingText, "D2 Noviciado.activeTrainingText");
                    valid &= Require(novitiateUI.supportText, "D2 Noviciado.supportText");
                    valid &= Require(novitiateUI.addSupportButton, "D2 Noviciado.addSupportButton");
                    valid &= Require(novitiateUI.removeSupportButton, "D2 Noviciado.removeSupportButton");
                    valid &= Require(novitiateUI.lastResultText, "D2 Noviciado.lastResultText");
                    valid &= Require(novitiateUI.startTrainingButton, "D2 Noviciado.startButton");
                    valid &= Require(novitiateUI.startTrainingButtonText, "D2 Noviciado.startButtonText");
                    valid &= Require(novitiateUI.cancelTrainingButton, "D2 Noviciado.cancelButton");
                    valid &= Require(novitiateUI.upgradeButton, "D2 Noviciado.upgradeButton");
                    valid &= Require(novitiateUI.upgradeButtonText, "D2 Noviciado.upgradeButtonText");
                }

                D2RitesPanelUI ritesUI = civilization1UI.ritesPanelUI;
                if (ritesUI != null)
                {
                    valid &= Require(ritesUI.riteDropdown, "D2 Ritos.riteDropdown");
                    valid &= Require(ritesUI.slotsText, "D2 Ritos.slotsText");
                    valid &= Require(ritesUI.effectText, "D2 Ritos.effectText");
                    valid &= Require(ritesUI.resourcesText, "D2 Ritos.resourcesText");
                    valid &= Require(ritesUI.assignmentText, "D2 Ritos.assignmentText");
                    valid &= Require(ritesUI.assignFollowerOneButton, "D2 Ritos.assignFollowerOneButton");
                    valid &= Require(ritesUI.assignFollowerTenButton, "D2 Ritos.assignFollowerTenButton");
                    valid &= Require(ritesUI.releaseFollowerOneButton, "D2 Ritos.releaseFollowerOneButton");
                    valid &= Require(ritesUI.assignAcolyteOneButton, "D2 Ritos.assignAcolyteOneButton");
                    valid &= Require(ritesUI.assignAcolyteFiveButton, "D2 Ritos.assignAcolyteFiveButton");
                    valid &= Require(ritesUI.releaseAcolyteOneButton, "D2 Ritos.releaseAcolyteOneButton");
                    valid &= Require(ritesUI.releaseAllButton, "D2 Ritos.releaseAllButton");
                    valid &= Require(ritesUI.unlockThirdSlotButton, "D2 Ritos.unlockThirdSlotButton");
                    valid &= Require(ritesUI.unlockThirdSlotButtonText, "D2 Ritos.unlockThirdSlotButtonText");
                }

                D2CivilizationPactsPanelUI pactsUI = civilization1UI.pactsPanelUI;
                if (pactsUI != null)
                {
                    valid &= Require(pactsUI.pactDropdown, "D2 Pactos.pactDropdown");
                    valid &= Require(pactsUI.headerTrustText, "D2 Pactos.headerTrustText");
                    valid &= Require(pactsUI.headerAcolytesText, "D2 Pactos.headerAcolytesText");
                    valid &= Require(pactsUI.headerWaxText, "D2 Pactos.headerWaxText");
                    valid &= Require(pactsUI.headerBreadText, "D2 Pactos.headerBreadText");
                    valid &= Require(pactsUI.slotsText, "D2 Pactos.slotsText");
                    valid &= Require(pactsUI.pactStateText, "D2 Pactos.pactStateText");
                    valid &= Require(pactsUI.benefitText, "D2 Pactos.benefitText");
                    valid &= Require(pactsUI.commitmentText, "D2 Pactos.commitmentText");
                    valid &= Require(pactsUI.resourcesText, "D2 Pactos.resourcesText");
                    valid &= Require(pactsUI.lastResultText, "D2 Pactos.lastResultText");
                    valid &= Require(pactsUI.activateButton, "D2 Pactos.activateButton");
                    valid &= Require(pactsUI.activateButtonText, "D2 Pactos.activateButtonText");
                    valid &= Require(pactsUI.cancelButton, "D2 Pactos.cancelButton");
                    valid &= Require(pactsUI.unlockSecondSlotButton, "D2 Pactos.unlockSecondSlotButton");
                    valid &= Require(pactsUI.unlockSecondSlotButtonText, "D2 Pactos.unlockSecondSlotButtonText");
                    valid &= Require(pactsUI.secondSlotRequirementsText, "D2 Pactos.secondSlotRequirementsText");
                    valid &= Require(pactsUI.lockRequirementIconsOverlay, "D2 Pactos.lockRequirementIconsOverlay");
                    valid &= Require(pactsUI.hospitalityCardButton, "D2 Pactos.hospitalityCardButton");
                    valid &= Require(pactsUI.openPathCardButton, "D2 Pactos.openPathCardButton");
                    valid &= Require(pactsUI.consecrationCardButton, "D2 Pactos.consecrationCardButton");
                    valid &= Require(pactsUI.silentVowCardButton, "D2 Pactos.silentVowCardButton");
                    valid &= Require(pactsUI.innerDoorCardButton, "D2 Pactos.innerDoorCardButton");
                    valid &= Require(pactsUI.cancelButton, "D2 Pactos.cancelButton");
                }

                D2VeiledThresholdPanelUI thresholdUI = civilization1UI.veiledThresholdPanelUI;
                if (thresholdUI != null)
                {
                    valid &= Require(thresholdUI.titleText, "D2 Umbral.titleText");
                    valid &= Require(thresholdUI.revelationText, "D2 Umbral.revelationText");
                    valid &= Require(thresholdUI.placeText, "D2 Umbral.placeText");
                    valid &= Require(thresholdUI.pendingText, "D2 Umbral.pendingText");
                    valid &= Require(thresholdUI.resourcesText, "D2 Umbral.resourcesText");
                    valid &= Require(thresholdUI.acolytesText, "D2 Umbral.acolytesText");
                    valid &= Require(thresholdUI.lineDropdown, "D2 Umbral.lineDropdown");
                    valid &= Require(thresholdUI.lineText, "D2 Umbral.lineText");
                    valid &= Require(thresholdUI.prepareButton, "D2 Umbral.prepareButton");
                    valid &= Require(thresholdUI.assignAcolyteButton, "D2 Umbral.assignAcolyteButton");
                    valid &= Require(thresholdUI.releaseAcolyteButton, "D2 Umbral.releaseAcolyteButton");
                    valid &= Require(thresholdUI.upgradeLineButton, "D2 Umbral.upgradeLineButton");
                    valid &= Require(thresholdUI.incenseValueText, "D2 Vínculo.incenseValueText");
                    valid &= Require(thresholdUI.sacredClothValueText, "D2 Vínculo.sacredClothValueText");
                    valid &= Require(thresholdUI.carvedStoneValueText, "D2 Vínculo.carvedStoneValueText");
                    valid &= Require(thresholdUI.progressValueText, "D2 Vínculo.progressValueText");
                    valid &= Require(thresholdUI.centerStateText, "D2 Vínculo.centerStateText");
                    valid &= Require(thresholdUI.detailTitleText, "D2 Vínculo.detailTitleText");
                    valid &= Require(thresholdUI.detailLevelText, "D2 Vínculo.detailLevelText");
                    valid &= Require(thresholdUI.effectText, "D2 Vínculo.effectText");
                    valid &= Require(thresholdUI.progressCostText, "D2 Vínculo.progressCostText");
                    valid &= Require(thresholdUI.availableAcolytesValueText, "D2 Vínculo.availableAcolytesValueText");
                    valid &= Require(thresholdUI.assignedAcolytesValueText, "D2 Vínculo.assignedAcolytesValueText");
                    valid &= Require(thresholdUI.actionButtonText, "D2 Vínculo.actionButtonText");
                    valid &= Require(thresholdUI.thresholdTabLabelText, "D2 Vínculo.thresholdTabLabelText");
                    valid &= Require(thresholdUI.backButton, "D2 Vínculo.backButton");
                    valid &= thresholdUI.lineButtons != null && thresholdUI.lineButtons.Length == 5;
                    valid &= thresholdUI.lineNameTexts != null && thresholdUI.lineNameTexts.Length == 5;
                    valid &= thresholdUI.lineLevelTexts != null && thresholdUI.lineLevelTexts.Length == 5;
                }

                Canvas.ForceUpdateCanvases();
                valid &= ValidateDirectChildrenInside(
                    panel.GetComponent<RectTransform>(),
                    "Dimensión 2 general"
                );
                valid &= ValidateDirectChildrenInside(
                    civilization1UI.GetComponent<RectTransform>(),
                    "Civilización 1 general"
                );
                valid &= ValidateDirectChildrenInside(
                    civilization1UI.refugeSectionRoot.GetComponent<RectTransform>(),
                    "Refugio"
                );
                valid &= ValidateDirectChildrenInside(
                    civilization1UI.altarsSectionRoot.GetComponent<RectTransform>(),
                    "Altares"
                );
                valid &= ValidateDirectChildrenInside(
                    civilization1UI.pilgrimagesSectionRoot.GetComponent<RectTransform>(),
                    "Peregrinaciones"
                );
                valid &= ValidateDirectChildrenInside(
                    civilization1UI.novitiateSectionRoot.GetComponent<RectTransform>(),
                    "Noviciado"
                );
                valid &= ValidateDirectChildrenInside(
                    civilization1UI.ritesSectionRoot.GetComponent<RectTransform>(),
                    "Ritos"
                );
                valid &= ValidateDirectChildrenInside(
                    civilization1UI.pactsSectionRoot.GetComponent<RectTransform>(),
                    "Pactos"
                );
                valid &= ValidateDirectChildrenInside(
                    civilization1UI.veiledThresholdSectionRoot.GetComponent<RectTransform>(),
                    "Umbral"
                );
            }

            if (civilization2UI != null)
            {
                Canvas.ForceUpdateCanvases();
                valid &= ValidateDirectChildrenInside(
                    civilization2UI.GetComponent<RectTransform>(),
                    "Civilización 2 general"
                );
                valid &= ValidateDirectChildrenInside(
                    civilization2UI.regionSectionRoot.GetComponent<RectTransform>(),
                    "Civilización 2 regiones"
                );
                valid &= ValidateDirectChildrenInside(
                    civilization2UI.operationsSectionRoot.GetComponent<RectTransform>(),
                    "Civilización 2 operaciones"
                );
                valid &= ValidateDirectChildrenInside(
                    civilization2UI.defenseSectionRoot.GetComponent<RectTransform>(),
                    "Civilización 2 defensa"
                );
                valid &= ValidateDirectChildrenInside(
                    civilization2UI.resistanceSectionRoot.GetComponent<RectTransform>(),
                    "Civilización 2 RED"
                );
                valid &= ValidateDirectChildrenInside(
                    civilization2UI.alertSectionRoot.GetComponent<RectTransform>(),
                    "Civilización 2 Alerta"
                );
                valid &= ValidateDirectChildrenInside(
                    civilization2UI.containmentSectionRoot.GetComponent<RectTransform>(),
                    "Civilización 2 Contención"
                );
            }

            valid &= ValidateDimension2Block5FPresentation(panel);
        }

        if (gameState == null)
        {
            Debug.LogError("[D2 Block 1] Falta GameState.");
            valid = false;
        }
        else
        {
            gameState.EnsureDimension2State();
            if (!Dimension2System.ValidateState(gameState, out string stateResult))
            {
                Debug.LogError("[D2 Block 1] " + stateResult);
                valid = false;
            }

            valid &= ValidateCivilization1Logic(gameState);
            valid &= ValidateCivilization1Block5BLogic(gameState);
            valid &= ValidateCivilization2Block3ALogic(gameState);
            valid &= ValidateCivilization2Block3BLogic(gameState);
            valid &= ValidateCivilization2Block3CLogic(gameState);
            valid &= ValidateCivilization2Block3DLogic(gameState);
            valid &= ValidateCivilization2Block3ELogic(gameState);
            valid &= ValidateCivilization2Block3FLogic(gameState);
            valid &= ValidateCivilization2Block3GLogic(gameState);
            valid &= ValidateCivilization2Block5CLogic(gameState);
            valid &= ValidateCivilization3Block4ALogic(gameState);
            valid &= ValidateCivilization3Block4BLogic(gameState);
            valid &= ValidateCivilization3Block4CLogic(gameState);
            valid &= ValidateCivilization3Block4DLogic(gameState);
            valid &= ValidateCivilization3Block4ELogic(gameState);
            valid &= ValidateCivilization3Block4FLogic(gameState);
            valid &= ValidateCivilization3Block4GLogic(gameState);
            valid &= ValidateCivilization3Block4HLogic(gameState);
            valid &= ValidateCivilization3Block5DLogic(gameState);
            valid &= ValidateDimension2Block5ELogic(gameState);
            valid &= ValidateDimension2Block5FLogic(gameState);
        }

        if (valid)
            Debug.Log("[D2 Block 1] VALIDACIÓN OK: estado, pestaña, mapa y navegación conectados.");
    }

    private static Button GetOrCreateTabButton(TabsUI tabs)
    {
        VerticalNavigationUI navigation = Object.FindFirstObjectByType<VerticalNavigationUI>(
            FindObjectsInactive.Include
        );
        if (navigation != null && navigation.dimension2Button != null)
        {
            RemoveLegacyDimension2Tab(navigation);
            return navigation.dimension2Button;
        }

        Transform parent = tabs.btnDimension1.transform.parent;
        Transform existing = parent.Find("Btn_Dimension2");
        Button button;

        if (existing != null)
        {
            button = existing.GetComponent<Button>();
        }
        else
        {
            GameObject clone = Object.Instantiate(
                tabs.btnDimension1.gameObject,
                parent,
                false
            );
            clone.name = "Btn_Dimension2";
            Undo.RegisterCreatedObjectUndo(clone, "Create Dimension 2 tab button");
            button = clone.GetComponent<Button>();
        }

        if (button == null)
            button = Undo.AddComponent<Button>(existing != null ? existing.gameObject : parent.Find("Btn_Dimension2").gameObject);

        button.transform.SetSiblingIndex(tabs.btnDimension1.transform.GetSiblingIndex() + 1);
        button.onClick = new Button.ButtonClickedEvent();

        TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>(true);
        if (label != null)
        {
            LocalizedTMP localized = label.GetComponent<LocalizedTMP>();
            if (localized != null)
            {
                localized.key = string.Empty;
                EditorUtility.SetDirty(localized);
            }

            label.text = "DIMENSIÓN 2";
            EditorUtility.SetDirty(label);
        }

        EditorUtility.SetDirty(button);
        return button;
    }

    [MenuItem("Tools/Quantum Forge/Dimension 2/Remove Legacy Dimension 2 Tab")]
    public static void RemoveLegacyDimension2TabFromMainScene()
    {
        Scene scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
        VerticalNavigationUI navigation = Object.FindFirstObjectByType<VerticalNavigationUI>(
            FindObjectsInactive.Include);
        if (navigation == null || navigation.dimension2Button == null ||
            navigation.dimension1Button == null)
            throw new System.InvalidOperationException("Navegación canónica D2 no disponible.");
        RemoveLegacyDimension2Tab(navigation);
        navigation.dimension2Button.transform.SetSiblingIndex(
            navigation.dimension1Button.transform.GetSiblingIndex() + 1);
        EditorUtility.SetDirty(navigation.dimension2Button.transform);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[D2 Navigation] PASS | botón heredado eliminado y orden progresivo restaurado.");
    }

    private static void RemoveLegacyDimension2Tab(VerticalNavigationUI navigation)
    {
        Transform canonical = navigation.dimension2Button.transform;
        Transform legacy = canonical.parent != null
            ? canonical.parent.Find("Btn_Dimension2") : null;
        if (legacy != null && legacy != canonical)
            Object.DestroyImmediate(legacy.gameObject);
    }

    private static Dimension2PanelUI GetOrCreateDimension2Panel(TabsUI tabs)
    {
        Transform parent = tabs.dimension1Panel.transform.parent;
        Transform existing = parent.Find("Dimension2Panel");
        GameObject panelObject;

        if (existing != null)
        {
            panelObject = existing.gameObject;
        }
        else
        {
            panelObject = CreateUIObject("Dimension2Panel", parent);
            Undo.AddComponent<Image>(panelObject).color = new Color(0.035f, 0.04f, 0.075f, 1f);
        }

        Stretch(panelObject.GetComponent<RectTransform>());
        Dimension2PanelUI panel = panelObject.GetComponent<Dimension2PanelUI>();
        if (panel == null)
            panel = Undo.AddComponent<Dimension2PanelUI>(panelObject);
        Graphic panelBackground = panelObject.GetComponent<Graphic>();
        if (panelBackground != null)
            panelBackground.raycastTarget = false;

        ClearGeneratedChildren(panelObject.transform);
        BuildFirstEntry(panel);
        BuildMap(panel);
        BuildCivilization1Placeholder(panel);
        BuildCivilization2(panel);
        BuildCivilization3(panel);
        BuildGlobalCloseButton(panel);
        return panel;
    }

    private static void BuildFirstEntry(Dimension2PanelUI panel)
    {
        PrepareFirstEntrySprite(FirstEntryStonePath, 2048);
        PrepareFirstEntrySprite(FirstEntryHeroPath, 2048);
        PrepareFirstEntryTexture(FirstEntryFrameAtlasPath, 2048);
        PrepareFirstEntryTexture(FirstEntryButtonPlatePath, 2048);
        PrepareFirstEntryTexture(FirstEntryOuterFramePath, 2048);
        Sprite stone = AssetDatabase.LoadAssetAtPath<Sprite>(FirstEntryStonePath);
        Sprite hero = AssetDatabase.LoadAssetAtPath<Sprite>(FirstEntryHeroPath);
        Texture2D frameAtlas = AssetDatabase.LoadAssetAtPath<Texture2D>(FirstEntryFrameAtlasPath);
        Texture2D buttonPlate = AssetDatabase.LoadAssetAtPath<Texture2D>(FirstEntryButtonPlatePath);
        Texture2D outerFrame = AssetDatabase.LoadAssetAtPath<Texture2D>(FirstEntryOuterFramePath);
        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FirstEntryFontPath);
        Material whiteKey = GetOrCreateFirstEntryWhiteKeyMaterial();

        if (stone == null || hero == null || frameAtlas == null || buttonPlate == null ||
            outerFrame == null || font == null || whiteKey == null)
        {
            Debug.LogError("[D2 First Entry] Faltan capas ornamentales requeridas.");
            return;
        }

        GameObject root = CreateUIObject("D2_FirstEntry", panel.transform);
        panel.firstEntryRoot = root;
        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = rootRect.anchorMax = new Vector2(0.5f, 0.5f);
        rootRect.pivot = new Vector2(0.5f, 0.5f);
        rootRect.anchoredPosition = new Vector2(0f, -82f);
        rootRect.sizeDelta = new Vector2(FirstEntryWidth, FirstEntryHeight);
        Canvas canvas = Undo.AddComponent<Canvas>(root);
        canvas.overrideSorting = true;
        canvas.sortingOrder = 32740;
        SerializedObject canvasSettings = new SerializedObject(canvas);
        canvasSettings.FindProperty("m_OverrideSorting").boolValue = true;
        canvasSettings.FindProperty("m_SortingOrder").intValue = 32740;
        canvasSettings.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(canvas);
        Undo.AddComponent<GraphicRaycaster>(root);

        Color coal = FirstEntryHex("080B0B");
        Color surface = FirstEntryHex("10110F", 250);
        Color gold = FirstEntryHex("C49A58");
        Color ivory = FirstEntryHex("D6C3A0");

        Image background = CreateFirstEntryImage(
            "StoneBackground", root.transform, stone,
            0f, -40f, FirstEntryWidth, 2050f, Color.white, false
        );
        background.raycastTarget = false;
        Image darkener = CreateFirstEntryImage(
            "StoneDarkener", root.transform, null,
            0f, -40f, FirstEntryWidth, 2050f,
            new Color(coal.r, coal.g, coal.b, 0.34f), false
        );
        darkener.raycastTarget = false;

        Image titleSurface = CreateFirstEntryImage(
            "TitlePanel", root.transform, stone,
            64f, 100f, 952f, 150f, new Color(0.09f, 0.10f, 0.09f, 0.96f), false
        );
        RectTransform header = titleSurface.rectTransform;
        CreateFirstEntryRawImage(
            "TitleFrame", root.transform, frameAtlas, whiteKey,
            44f, 75f, 992f, 198f,
            new Rect(0.066f, 0.861f, 0.858f, 0.113f)
        );

        panel.firstEntryTitleText = CreateText(
            "Title",
            header,
            "DIMENSIÓN 2 · PACTOS",
            56f,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.5f),
            Vector2.one
        );
        ConfigureFirstEntryText(
            panel.firstEntryTitleText as TextMeshProUGUI, font, gold,
            50f, 64f, FontStyles.Bold, 0.7f, false
        );
        TextMeshProUGUI titleText = panel.firstEntryTitleText as TextMeshProUGUI;
        titleText.outlineColor = FirstEntryHex("24180D");
        titleText.outlineWidth = 0.16f;
        titleText.enableVertexGradient = true;
        titleText.colorGradient = new VertexGradient(
            FirstEntryHex("E2C79B"), FirstEntryHex("E2C79B"),
            FirstEntryHex("8D673A"), FirstEntryHex("8D673A")
        );
        Shadow titleShadow = Undo.AddComponent<Shadow>(titleText.gameObject);
        titleShadow.effectColor = FirstEntryHex("1D1108", 230);
        titleShadow.effectDistance = new Vector2(3f, -4f);
        titleShadow.useGraphicAlpha = true;
        Stretch(panel.firstEntryTitleText.rectTransform);

        Image heroSurface = CreateFirstEntryImage(
            "HeroPanel", root.transform, null,
            55f, 322f, 970f, 966f, coal, false
        );
        RectTransform heroPanel = heroSurface.rectTransform;
        GameObject clipObject = CreateUIObject("HeroClip", heroPanel);
        RectTransform clipRect = clipObject.GetComponent<RectTransform>();
        clipRect.anchorMin = Vector2.zero;
        clipRect.anchorMax = Vector2.one;
        clipRect.pivot = new Vector2(0.5f, 0.5f);
        clipRect.anchoredPosition = Vector2.zero;
        clipRect.offsetMin = new Vector2(20f, 20f);
        clipRect.offsetMax = new Vector2(-20f, -20f);
        Undo.AddComponent<RectMask2D>(clipObject);
        GameObject heroObject = CreateUIObject("HeroArt", clipObject.transform);
        RectTransform heroRect = heroObject.GetComponent<RectTransform>();
        Stretch(heroRect);
        Image heroImage = Undo.AddComponent<Image>(heroObject);
        heroImage.sprite = hero;
        heroImage.color = Color.white;
        heroImage.preserveAspect = true;
        heroImage.raycastTarget = false;
        AspectRatioFitter heroAspect = Undo.AddComponent<AspectRatioFitter>(heroObject);
        heroAspect.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        heroAspect.aspectRatio = 1f;

        CreateFirstEntryRawImage(
            "HeroFrame", root.transform, frameAtlas, whiteKey,
            42f, 294f, 996f, 1014f,
            new Rect(0.059f, 0.293f, 0.881f, 0.562f)
        );

        Image narrativeSurface = CreateFirstEntryImage(
            "NarrativePanel", root.transform, stone,
            61f, 1333f, 958f, 216f, new Color(0.08f, 0.085f, 0.075f, 0.98f), false
        );
        RectTransform narrative = narrativeSurface.rectTransform;
        CreateFirstEntryRawImage(
            "NarrativeFrame", root.transform, frameAtlas, whiteKey,
            43f, 1309f, 994f, 262f,
            new Rect(0.052f, 0.038f, 0.902f, 0.266f)
        );

        panel.firstEntryDescriptionText = CreateText(
            "Description",
            narrative,
            "ANTE TI APARECE UN MUNDO DIVIDIDO EN TRES TERRITORIOS.\n" +
            "SOLO EL SANTUARIO DE PEREGRINOS RESPONDE A TU LLEGADA.",
            31f,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.5f),
            Vector2.one
        );
        ConfigureFirstEntryText(
            panel.firstEntryDescriptionText as TextMeshProUGUI, font, ivory,
            22f, 28f, FontStyles.Bold, 0.05f, false
        );
        TextMeshProUGUI descriptionText = panel.firstEntryDescriptionText as TextMeshProUGUI;
        descriptionText.outlineColor = FirstEntryHex("24180D");
        descriptionText.outlineWidth = 0.10f;
        descriptionText.enableVertexGradient = true;
        descriptionText.colorGradient = new VertexGradient(
            FirstEntryHex("E1CFAD"), FirstEntryHex("E1CFAD"),
            FirstEntryHex("A78254"), FirstEntryHex("A78254")
        );
        Shadow descriptionShadow = Undo.AddComponent<Shadow>(descriptionText.gameObject);
        descriptionShadow.effectColor = FirstEntryHex("160D07", 220);
        descriptionShadow.effectDistance = new Vector2(2f, -2f);
        descriptionShadow.useGraphicAlpha = true;
        RectTransform descriptionRect = panel.firstEntryDescriptionText.rectTransform;
        Stretch(descriptionRect);
        descriptionRect.offsetMin = new Vector2(34f, 24f);
        descriptionRect.offsetMax = new Vector2(-34f, -24f);

        RawImage buttonTarget = CreateFirstEntryRawImage(
            "Btn_ContinueFirstEntry", root.transform, buttonPlate, whiteKey,
            155f, 1520f, 770f, 260f, new Rect(0f, 0f, 1f, 1f)
        );
        RectTransform buttonRect = buttonTarget.rectTransform;
        panel.continueFirstEntryButton = Undo.AddComponent<Button>(buttonRect.gameObject);
        buttonTarget.raycastTarget = true;
        panel.continueFirstEntryButton.targetGraphic = buttonTarget;
        panel.continueFirstEntryButton.onClick = new Button.ButtonClickedEvent();
        ColorBlock buttonColors = panel.continueFirstEntryButton.colors;
        buttonColors.normalColor = Color.white;
        buttonColors.highlightedColor = FirstEntryHex("FFF0BF");
        buttonColors.pressedColor = FirstEntryHex("C68D3B");
        buttonColors.selectedColor = Color.white;
        buttonColors.disabledColor = FirstEntryHex("665846", 180);
        buttonColors.colorMultiplier = 1f;
        buttonColors.fadeDuration = 0.08f;
        panel.continueFirstEntryButton.colors = buttonColors;
        TextMeshProUGUI buttonLabel = CreateText(
            "Label", buttonRect, "ABRIR MAPA", 78f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), Vector2.one
        );
        ConfigureFirstEntryText(
            buttonLabel, font, FirstEntryHex("E6C78F"),
            90f, 132f, FontStyles.Bold, 0.5f, false
        );
        buttonLabel.enableAutoSizing = false;
        buttonLabel.fontSize = 126f;
        buttonLabel.outlineColor = FirstEntryHex("281609");
        buttonLabel.outlineWidth = 0.18f;
        buttonLabel.enableVertexGradient = true;
        buttonLabel.colorGradient = new VertexGradient(
            FirstEntryHex("F2D79B"), FirstEntryHex("F2D79B"),
            FirstEntryHex("9B6426"), FirstEntryHex("9B6426")
        );
        Shadow buttonShadow = Undo.AddComponent<Shadow>(buttonLabel.gameObject);
        buttonShadow.effectColor = FirstEntryHex("241005", 240);
        buttonShadow.effectDistance = new Vector2(4f, -5f);
        buttonShadow.useGraphicAlpha = true;
        Stretch(buttonLabel.rectTransform);
        buttonLabel.rectTransform.localScale = new Vector3(1.8f, 1.8f, 1f);

        CreateFirstEntryRawImage(
            "OuterFrame", root.transform, outerFrame, whiteKey,
            0f, 0f, FirstEntryWidth, FirstEntryHeight,
            new Rect(0f, 0f, 1f, 1f)
        );
    }

    private static void PrepareFirstEntryTexture(string path, int maxSize)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
            return;
        importer.textureType = TextureImporterType.Default;
        importer.alphaIsTransparency = false;
        importer.mipmapEnabled = false;
        importer.sRGBTexture = true;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.maxTextureSize = maxSize;
        importer.SaveAndReimport();
    }

    private static Material GetOrCreateFirstEntryWhiteKeyMaterial()
    {
        Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(FirstEntryWhiteKeyShaderPath);
        if (shader == null)
            shader = Shader.Find("UI/Dimension2/WhiteKey");
        if (shader == null)
            return null;

        Material material = AssetDatabase.LoadAssetAtPath<Material>(
            FirstEntryWhiteKeyMaterialPath
        );
        if (material == null)
        {
            material = new Material(shader)
            {
                name = "D2FirstEntryWhiteKey"
            };
            AssetDatabase.CreateAsset(material, FirstEntryWhiteKeyMaterialPath);
        }
        else if (material.shader != shader)
        {
            material.shader = shader;
        }

        material.SetFloat("_KeyThreshold", 0.53f);
        material.SetFloat("_KeySoftness", 0.10f);
        EditorUtility.SetDirty(material);
        return material;
    }

    private static void PrepareFirstEntrySprite(string path, int maxSize)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
            return;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.sRGBTexture = true;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.maxTextureSize = maxSize;
        importer.SaveAndReimport();
    }

    private static RectTransform CreateFirstEntryPanel(
        string name, Transform parent, float x, float y, float width, float height,
        Color fillColor, Color outerColor, Color innerColor,
        float outerThickness, float innerInset, bool centerMarks,
        out Image fill
    )
    {
        GameObject panel = CreateUIObject(name, parent);
        RectTransform rect = panel.GetComponent<RectTransform>();
        SetFirstEntryTopLeft(rect, x, y, width, height);
        fill = Undo.AddComponent<Image>(panel);
        fill.color = fillColor;
        fill.raycastTarget = false;

        CreateFirstEntryRail("FrameTop", rect, 0f, 0f, width, outerThickness, outerColor);
        CreateFirstEntryRail("FrameBottom", rect, 0f, height - outerThickness, width, outerThickness, outerColor);
        CreateFirstEntryRail("FrameLeft", rect, 0f, 0f, outerThickness, height, outerColor);
        CreateFirstEntryRail("FrameRight", rect, width - outerThickness, 0f, outerThickness, height, outerColor);
        CreateFirstEntryRail("InnerTop", rect, innerInset, innerInset, width - innerInset * 2f, 2f, innerColor);
        CreateFirstEntryRail("InnerBottom", rect, innerInset, height - innerInset - 2f, width - innerInset * 2f, 2f, innerColor);
        CreateFirstEntryRail("InnerLeft", rect, innerInset, innerInset, 2f, height - innerInset * 2f, innerColor);
        CreateFirstEntryRail("InnerRight", rect, width - innerInset - 2f, innerInset, 2f, height - innerInset * 2f, innerColor);
        if (centerMarks)
        {
            CreateFirstEntryDiamond("TopMark", rect, width * 0.5f, 0f, 18f, outerColor, fillColor);
            CreateFirstEntryDiamond("BottomMark", rect, width * 0.5f, height, 18f, outerColor, fillColor);
        }
        return rect;
    }

    private static Image CreateFirstEntryImage(
        string name, Transform parent, Sprite sprite,
        float x, float y, float width, float height, Color color, bool preserveAspect
    )
    {
        GameObject imageObject = CreateUIObject(name, parent);
        RectTransform rect = imageObject.GetComponent<RectTransform>();
        SetFirstEntryTopLeft(rect, x, y, width, height);
        Image image = Undo.AddComponent<Image>(imageObject);
        image.sprite = sprite;
        image.color = color;
        image.preserveAspect = preserveAspect;
        image.raycastTarget = false;
        return image;
    }

    private static RawImage CreateFirstEntryRawImage(
        string name, Transform parent, Texture texture, Material material,
        float x, float y, float width, float height, Rect uvRect
    )
    {
        GameObject imageObject = CreateUIObject(name, parent);
        RectTransform rect = imageObject.GetComponent<RectTransform>();
        SetFirstEntryTopLeft(rect, x, y, width, height);
        RawImage image = Undo.AddComponent<RawImage>(imageObject);
        image.texture = texture;
        image.material = material;
        image.uvRect = uvRect;
        image.color = Color.white;
        image.raycastTarget = false;
        return image;
    }

    private static void CreateFirstEntryRail(
        string name, Transform parent, float x, float y,
        float width, float height, Color color
    )
    {
        Image rail = CreateFirstEntryImage(
            name, parent, null, x, y, width, height, color, false
        );
        rail.raycastTarget = false;
    }

    private static GameObject CreateFirstEntryDiamond(
        string name, Transform parent, float centerX, float centerY,
        float size, Color outerColor, Color innerColor
    )
    {
        GameObject outerObject = CreateUIObject(name, parent);
        RectTransform outer = outerObject.GetComponent<RectTransform>();
        outer.anchorMin = outer.anchorMax = new Vector2(0f, 1f);
        outer.pivot = new Vector2(0.5f, 0.5f);
        outer.anchoredPosition = new Vector2(centerX, -centerY);
        outer.sizeDelta = new Vector2(size, size);
        outer.localEulerAngles = new Vector3(0f, 0f, 45f);
        Image outerImage = Undo.AddComponent<Image>(outerObject);
        outerImage.color = outerColor;
        outerImage.raycastTarget = false;

        GameObject innerObject = CreateUIObject("Inner", outerObject.transform);
        RectTransform inner = innerObject.GetComponent<RectTransform>();
        inner.anchorMin = new Vector2(0.22f, 0.22f);
        inner.anchorMax = new Vector2(0.78f, 0.78f);
        inner.pivot = new Vector2(0.5f, 0.5f);
        inner.anchoredPosition = Vector2.zero;
        inner.sizeDelta = Vector2.zero;
        Image innerImage = Undo.AddComponent<Image>(innerObject);
        innerImage.color = innerColor;
        innerImage.raycastTarget = false;
        return outerObject;
    }

    private static void SetFirstEntryTopLeft(
        RectTransform rect, float x, float y, float width, float height
    )
    {
        rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(x, -y);
        rect.sizeDelta = new Vector2(width, height);
    }

    private static void ConfigureFirstEntryText(
        TextMeshProUGUI label, TMP_FontAsset font, Color color,
        float minSize, float maxSize, FontStyles style,
        float characterSpacing, bool wrap
    )
    {
        if (label == null)
            return;
        label.font = font;
        label.color = color;
        label.fontStyle = style;
        label.enableAutoSizing = true;
        label.fontSizeMin = minSize;
        label.fontSizeMax = maxSize;
        label.characterSpacing = characterSpacing;
        label.textWrappingMode = wrap ? TextWrappingModes.Normal : TextWrappingModes.NoWrap;
        label.overflowMode = TextOverflowModes.Ellipsis;
        label.raycastTarget = false;
    }

    private static Color FirstEntryHex(string rgb, byte alpha = 255)
    {
        ColorUtility.TryParseHtmlString("#" + rgb, out Color color);
        color.a = alpha / 255f;
        return color;
    }

    private static bool ValidateFirstEntryVisual(Dimension2PanelUI panel)
    {
        bool valid = true;
        GameObject root = panel != null ? panel.firstEntryRoot : null;
        if (root == null)
        {
            Debug.LogError("[D2 First Entry] Falta D2_FirstEntry.");
            return false;
        }

        Canvas canvas = root.GetComponent<Canvas>();
        SerializedObject canvasSettings = canvas != null ? new SerializedObject(canvas) : null;
        bool overrideSorting = canvasSettings != null &&
            canvasSettings.FindProperty("m_OverrideSorting").boolValue;
        if (canvas == null || !overrideSorting)
        {
            Debug.LogError("[D2 First Entry] La pantalla no tiene su Canvas de primer plano.");
            valid = false;
        }

        string[] paths =
        {
            "StoneBackground", "OuterFrame", "TitlePanel", "TitleFrame",
            "HeroPanel/HeroClip/HeroArt", "HeroFrame", "NarrativePanel",
            "NarrativeFrame", "Btn_ContinueFirstEntry"
        };
        for (int i = 0; i < paths.Length; i++)
        {
            if (root.transform.Find(paths[i]) != null)
                continue;
            Debug.LogError("[D2 First Entry] Falta bloque visual: " + paths[i]);
            valid = false;
        }

        const string expected =
            "ANTE TI APARECE UN MUNDO DIVIDIDO EN TRES TERRITORIOS.\n" +
            "SOLO EL SANTUARIO DE PEREGRINOS RESPONDE A TU LLEGADA.";
        if (panel.firstEntryTitleText == null || panel.firstEntryTitleText.text != "DIMENSIÓN 2 · PACTOS")
        {
            Debug.LogError("[D2 First Entry] El título no coincide con la referencia elegida.");
            valid = false;
        }
        if (panel.firstEntryDescriptionText == null || panel.firstEntryDescriptionText.text != expected)
        {
            Debug.LogError("[D2 First Entry] El texto narrativo no coincide con la decisión vigente.");
            valid = false;
        }
        TMP_Text buttonLabel = panel.continueFirstEntryButton != null
            ? panel.continueFirstEntryButton.GetComponentInChildren<TMP_Text>(true)
            : null;
        if (buttonLabel == null || buttonLabel.text != "ABRIR MAPA")
        {
            Debug.LogError("[D2 First Entry] El botón ABRIR MAPA no está conectado.");
            valid = false;
        }
        if (root.transform.Find("MechanicsPanel") != null)
        {
            Debug.LogError("[D2 First Entry] Reapareció el panel de mecánica eliminado por el usuario.");
            valid = false;
        }
        return valid;
    }

    private static bool ValidatePactMapVisual(Dimension2PanelUI panel)
    {
        bool valid = true;
        GameObject root = panel != null ? panel.mapRoot : null;
        if (root == null || root.name != "D2_PactMap")
        {
            Debug.LogError("[D2 Pact Map] Falta la raíz canónica D2_PactMap.");
            return false;
        }

        Canvas canvas = root.GetComponent<Canvas>();
        if (canvas == null || !canvas.overrideSorting || canvas.sortingOrder != 32741)
        {
            Debug.LogError("[D2 Pact Map] El mapa no tiene su Canvas canónico de primer plano.");
            valid = false;
        }

        string[] requiredPaths =
        {
            "MapBasePlate", "FollowersIcon", "TrustIcon",
            "SanctuaryMedallion", "ResistanceMedallion", "RuinsLockMedallion",
            "Title", "SanctuaryTitle", "ResistanceTitle", "RuinsTitle",
            "Btn_EnterSanctuary", "BottomMedallion", "MapStatus",
            "MapSecondaryStatus", "Btn_MapHelp", "Btn_MapBack"
        };
        for (int i = 0; i < requiredPaths.Length; i++)
        {
            if (root.transform.Find(requiredPaths[i]) != null)
                continue;
            Debug.LogError("[D2 Pact Map] Falta bloque visual: " + requiredPaths[i]);
            valid = false;
        }

        valid &= panel.mapFollowersText != null;
        valid &= panel.mapTrustText != null;
        valid &= panel.mapCivilization1TrustText != null;
        valid &= panel.mapCivilization2RequirementText != null;
        valid &= panel.mapCivilization3RequirementText != null;
        valid &= panel.mapHelpButton != null;
        valid &= panel.mapBackButton != null;
        valid &= panel.civilization1Button != null;
        valid &= panel.civilization2Button != null;
        valid &= panel.civilization3Button != null;
        if (!valid)
            Debug.LogError("[D2 Pact Map] Faltan referencias serializadas del mapa.");
        return valid;
    }

    private static bool ValidateResistanceRegionsVisual(Dimension2PanelUI panel)
    {
        GameObject root = panel != null ? panel.civilization2Root : null;
        D2Civilization2PanelUI ui = panel != null ? panel.civilization2PanelUI : null;
        if (root == null || ui == null)
        {
            Debug.LogError("[D2 Resistance Regions] Falta la raíz o el controlador.");
            return false;
        }
        bool valid = true;
        Canvas canvas = root.GetComponent<Canvas>();
        if (canvas == null || !canvas.overrideSorting || canvas.sortingOrder != 32743)
        {
            Debug.LogError("[D2 Resistance Regions] El Canvas canónico no está configurado.");
            valid = false;
        }
        string[] paths =
        {
            "D2_Civ2_RegionSection/ResistanceRegionsBasePlate",
            "D2_Civ2_RegionSection/ResistanceRegionsTitle",
            "D2_Civ2_RegionSection/RegionDetailTitle",
            "D2_Civ2_RegionSection/RegionAssignmentLabel",
            "Btn_SelectRegion1", "Btn_SelectRegion2", "Btn_SelectRegion3",
            "Btn_Civ2BackToMap", "Btn_Civ2Help",
            "Btn_Civ2Nav0", "Btn_Civ2Nav1", "Btn_Civ2Nav2",
            "Btn_Civ2Nav3", "Btn_Civ2Nav4", "Btn_Civ2Nav5"
        };
        for (int i = 0; i < paths.Length; i++)
        {
            if (root.transform.Find(paths[i]) != null) continue;
            Debug.LogError("[D2 Resistance Regions] Falta bloque visual: " + paths[i]);
            valid = false;
        }
        if (ui.regionButtons == null || ui.regionButtons.Length != 3 ||
            ui.regionNameTexts == null || ui.regionNameTexts.Length != 3 ||
            ui.availableMembersValueText == null || ui.assignedMembersValueText == null ||
            ui.totalMembersValueText == null || ui.totalDominanceValueText == null ||
            ui.detailTitleText == null || ui.detailIdleValueText == null)
        {
            Debug.LogError("[D2 Resistance Regions] Faltan referencias dinámicas V4.");
            valid = false;
        }
        return valid;
    }

    private static bool ValidateResistanceDefenseVisual(Dimension2PanelUI panel)
    {
        GameObject root = panel != null ? panel.civilization2Root : null;
        D2ReprisalsPanelUI ui = panel?.civilization2PanelUI?.reprisalsPanelUI;
        if (root == null || ui == null)
        {
            Debug.LogError("[D2 Resistance Defense] Falta la raíz o el controlador.");
            return false;
        }

        bool valid = true;
        string[] paths =
        {
            "D2_Civ2_DefenseSection/ResistanceDefenseBasePlate",
            "D2_Civ2_DefenseSection/ResistanceDefenseTitle",
            "D2_Civ2_DefenseSection/DefenseThreatGauge",
            "D2_Civ2_DefenseSection/DefenseCoverageGauge",
            "D2_Civ2_DefenseSection/DefenseWeakeningOperation",
            "D2_Civ2_DefenseSection/DefenseRules",
            "D2_Civ2_DefenseSection/DefenseLastResult",
            "D2_Civ2_DefenseSection/DefenseNavSelection",
            "D2_Civ2_DefenseSection/Btn_DefenseCycleRegion"
        };
        for (int i = 0; i < paths.Length; i++)
        {
            if (root.transform.Find(paths[i]) != null) continue;
            Debug.LogError("[D2 Resistance Defense] Falta bloque visual: " + paths[i]);
            valid = false;
        }
        if (ui.availableMembersText == null || ui.fragmentsText == null ||
            ui.reprisalsCountText == null || ui.regionNameText == null ||
            ui.threatText == null || ui.coverageText == null ||
            ui.estimatedLossText == null || ui.protectionText == null ||
            ui.weakeningText == null || ui.weakeningDurationText == null ||
            ui.rulesText == null || ui.lastResultText == null ||
            ui.threatGauge == null || ui.coverageGauge == null ||
            ui.regionSelectorButton == null)
        {
            Debug.LogError("[D2 Resistance Defense] Faltan referencias V4.");
            valid = false;
        }
        return valid;
    }

    private static bool ValidateResistanceAlertVisual(Dimension2PanelUI panel)
    {
        GameObject root = panel != null ? panel.civilization2Root : null;
        D2AlertPanelUI ui = panel?.civilization2PanelUI?.alertPanelUI;
        if (root == null || ui == null)
        {
            Debug.LogError("[D2 Resistance Alert] Falta la raíz o el controlador.");
            return false;
        }

        bool valid = true;
        string[] paths =
        {
            "D2_Civ2_AlertSection/ResistanceAlertBasePlate",
            "D2_Civ2_AlertSection/ResistanceAlertTitle",
            "D2_Civ2_AlertSection/AlertDominanceGauge",
            "D2_Civ2_AlertSection/AlertEntityIcon",
            "D2_Civ2_AlertSection/AlertRegions",
            "D2_Civ2_AlertSection/AlertLastResult",
            "D2_Civ2_AlertSection/AlertNavSelection"
        };
        for (int i = 0; i < paths.Length; i++)
        {
            if (root.transform.Find(paths[i]) != null) continue;
            Debug.LogError("[D2 Resistance Alert] Falta bloque visual: " + paths[i]);
            valid = false;
        }
        if (ui.stateText == null || ui.dominanceText == null || ui.timerText == null ||
            ui.marksCountText == null || ui.effectsText == null ||
            ui.fragmentsEffectText == null || ui.regionsText == null ||
            ui.regionNameTexts == null || ui.regionNameTexts.Length != 3 ||
            ui.regionMarkTexts == null || ui.regionMarkTexts.Length != 3 ||
            ui.regionProtectionTexts == null || ui.regionProtectionTexts.Length != 3 ||
            ui.regionProtectionActiveIcons == null ||
            ui.regionProtectionActiveIcons.Length != 3 ||
            ui.regionProtectionInactiveIcons == null ||
            ui.regionProtectionInactiveIcons.Length != 3 ||
            ui.unlocksText == null || ui.unlockDetailText == null ||
            ui.lastResultText == null || ui.lastResultDetailText == null ||
            ui.dominanceGauge == null)
        {
            Debug.LogError("[D2 Resistance Alert] Faltan referencias V4.");
            valid = false;
        }
        return valid;
    }

    private static bool ValidateResistanceContainmentVisual(Dimension2PanelUI panel)
    {
        GameObject root = panel != null ? panel.civilization2Root : null;
        D2ContainmentPanelUI ui = panel?.civilization2PanelUI?.containmentPanelUI;
        if (root == null || ui == null)
        {
            Debug.LogError("[D2 Resistance Containment] Falta la raíz o el controlador.");
            return false;
        }

        bool valid = true;
        string[] paths =
        {
            "D2_Civ2_ContainmentSection/D2_Civ2_ContainmentAttempt/" +
                "ResistanceContainmentBasePlate",
            "D2_Civ2_ContainmentSection/D2_Civ2_ContainmentAttempt/" +
                "ResistanceContainmentTitle",
            "D2_Civ2_ContainmentSection/D2_Civ2_ContainmentAttempt/ContainmentState",
            "D2_Civ2_ContainmentSection/D2_Civ2_ContainmentAttempt/" +
                "ContainmentProbability",
            "D2_Civ2_ContainmentSection/D2_Civ2_ContainmentAttempt/" +
                "ContainmentFailureRule",
            "D2_Civ2_ContainmentSection/D2_Civ2_ContainmentAttempt/" +
                "ContainmentLastResult",
            "D2_Civ2_ContainmentSection/D2_Civ2_ContainmentAttempt/" +
                "Btn_AttemptContainment",
            "D2_Civ2_ContainmentSection/D2_Civ2_ContainmentAttempt/" +
                "ContainmentNavSelection"
        };
        for (int i = 0; i < paths.Length; i++)
        {
            if (root.transform.Find(paths[i]) != null) continue;
            Debug.LogError("[D2 Resistance Containment] Falta bloque visual: " + paths[i]);
            valid = false;
        }
        if (ui.stateText == null || ui.availableMembersText == null ||
            ui.dominanceHeaderText == null || ui.attemptsHeaderText == null ||
            ui.failuresHeaderText == null || ui.probabilityText == null ||
            ui.probabilityDetailText == null || ui.dominanceCardText == null ||
            ui.cooldownText == null || ui.failureRuleText == null ||
            ui.successRuleText == null || ui.assignedMembersText == null ||
            ui.assignmentAvailableText == null || ui.lastResultText == null ||
            ui.attemptButton == null)
        {
            Debug.LogError("[D2 Resistance Containment] Faltan referencias V4.");
            valid = false;
        }
        return valid;
    }

    private static void BuildMap(Dimension2PanelUI panel)
    {
        string[] texturePaths =
        {
            MapCleanBasePlatePath, MapShellPath, MapSanctuaryPath, MapResistancePath, MapRuinsPath,
            MapSanctuaryMedallionPath, MapResistanceMedallionPath, MapLockMedallionPath,
            MapHelpPath, MapBackPath, MapFollowersPath, MapTrustPath, MapSanctuaryButtonPath
        };
        for (int i = 0; i < texturePaths.Length; i++)
            PrepareFirstEntryTexture(texturePaths[i], 2048);
        PrepareFirstEntrySprite(FirstEntryStonePath, 2048);

        Sprite stone = AssetDatabase.LoadAssetAtPath<Sprite>(FirstEntryStonePath);
        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FirstEntryFontPath);
        Material whiteKey = GetOrCreateMapWhiteKeyMaterial();
        Texture2D cleanBasePlate = AssetDatabase.LoadAssetAtPath<Texture2D>(MapCleanBasePlatePath);
        Texture2D shell = AssetDatabase.LoadAssetAtPath<Texture2D>(MapShellPath);
        Texture2D sanctuary = AssetDatabase.LoadAssetAtPath<Texture2D>(MapSanctuaryPath);
        Texture2D resistance = AssetDatabase.LoadAssetAtPath<Texture2D>(MapResistancePath);
        Texture2D ruins = AssetDatabase.LoadAssetAtPath<Texture2D>(MapRuinsPath);
        Texture2D sanctuaryMedallion = AssetDatabase.LoadAssetAtPath<Texture2D>(MapSanctuaryMedallionPath);
        Texture2D resistanceMedallion = AssetDatabase.LoadAssetAtPath<Texture2D>(MapResistanceMedallionPath);
        Texture2D lockMedallion = AssetDatabase.LoadAssetAtPath<Texture2D>(MapLockMedallionPath);
        Texture2D help = AssetDatabase.LoadAssetAtPath<Texture2D>(MapHelpPath);
        Texture2D back = AssetDatabase.LoadAssetAtPath<Texture2D>(MapBackPath);
        Texture2D followers = AssetDatabase.LoadAssetAtPath<Texture2D>(MapFollowersPath);
        Texture2D trust = AssetDatabase.LoadAssetAtPath<Texture2D>(MapTrustPath);
        Texture2D sanctuaryButton = AssetDatabase.LoadAssetAtPath<Texture2D>(MapSanctuaryButtonPath);
        if (stone == null || font == null || whiteKey == null || cleanBasePlate == null || shell == null ||
            sanctuary == null || resistance == null || ruins == null ||
            sanctuaryMedallion == null || resistanceMedallion == null || lockMedallion == null ||
            help == null || back == null || followers == null || trust == null || sanctuaryButton == null)
        {
            Debug.LogError("[D2 Pact Map] Faltan capas canónicas requeridas.");
            return;
        }

        GameObject root = CreateUIObject("D2_PactMap", panel.transform);
        panel.mapRoot = root;
        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = rootRect.anchorMax = new Vector2(0.5f, 0.5f);
        rootRect.pivot = new Vector2(0.5f, 0.5f);
        rootRect.anchoredPosition = new Vector2(0f, -82f);
        rootRect.sizeDelta = new Vector2(FirstEntryWidth, FirstEntryHeight);
        Canvas mapCanvas = Undo.AddComponent<Canvas>(root);
        mapCanvas.overrideSorting = true;
        mapCanvas.sortingOrder = 32741;
        SerializedObject mapCanvasSettings = new SerializedObject(mapCanvas);
        mapCanvasSettings.FindProperty("m_OverrideSorting").boolValue = true;
        mapCanvasSettings.FindProperty("m_SortingOrder").intValue = 32741;
        mapCanvasSettings.ApplyModifiedPropertiesWithoutUndo();
        Undo.AddComponent<GraphicRaycaster>(root);

        Color ivory = FirstEntryHex("F0D5A7");
        Color bronze = FirstEntryHex("DFB276");
        Color green = FirstEntryHex("82B887");
        Color red = FirstEntryHex("D8664B");
        Color gray = FirstEntryHex("BDB3A8");

        RawImage baseLayer = CreateFirstEntryRawImage(
            "MapBasePlate", root.transform, cleanBasePlate, null,
            0f, 40f, FirstEntryWidth, 1926f, new Rect(0f, 0f, 1f, 1f));
        baseLayer.raycastTarget = true;

        CreateFirstEntryRawImage(
            "FollowersIcon", root.transform, followers, whiteKey,
            194f, 170f, 84f, 84f, new Rect(0f, 0f, 1f, 1f));
        CreateFirstEntryRawImage(
            "TrustIcon", root.transform, trust, whiteKey,
            560f, 166f, 92f, 92f, new Rect(0f, 0f, 1f, 1f));
        CreateFirstEntryRawImage(
            "SanctuaryMedallion", root.transform, sanctuaryMedallion, whiteKey,
            179f, 426f, 220f, 220f, new Rect(0f, 0f, 1f, 1f));
        CreateFirstEntryRawImage(
            "ResistanceMedallion", root.transform, resistanceMedallion, whiteKey,
            681f, 432f, 220f, 220f, new Rect(0f, 0f, 1f, 1f));
        CreateFirstEntryRawImage(
            "RuinsLockMedallion", root.transform, lockMedallion, whiteKey,
            397f, 1146f, 286f, 286f, new Rect(0f, 0f, 1f, 1f));

        CreateMapText("Title", root.transform, "MAPA DE LOS PACTOS", font,
            150f, 78f, 780f, 68f, 48f, ivory, TextAlignmentOptions.Center, true);
        CreateMapText("FollowersLabel", root.transform, "SEGUIDORES", font,
            277f, 175f, 255f, 35f, 26f, bronze, TextAlignmentOptions.Left, true);
        panel.mapFollowersText = CreateMapText("FollowersValue", root.transform, "3,845", font,
            277f, 206f, 255f, 48f, 39f, ivory, TextAlignmentOptions.Left, true);
        CreateMapText("TrustLabel", root.transform, "CONFIANZA", font,
            655f, 175f, 300f, 35f, 26f, bronze, TextAlignmentOptions.Left, true);
        panel.mapTrustText = CreateMapText("TrustValue", root.transform, "280 / 500", font,
            655f, 206f, 330f, 48f, 39f, ivory, TextAlignmentOptions.Left, true);

        CreateMapText("SanctuaryTitle", root.transform, "SANTUARIO DE\nPEREGRINOS", font,
            100f, 658f, 380f, 96f, 34f, bronze, TextAlignmentOptions.Center, true);
        CreateMapSeparator(root.transform, "SanctuarySeparator", 150f, 751f, 280f, bronze);
        panel.civilization1StateText = CreateMapText("SanctuaryState", root.transform, "DISPONIBLE", font,
            130f, 789f, 320f, 44f, 28f, green, TextAlignmentOptions.Center, true);
        panel.mapCivilization1TrustText = CreateMapText("SanctuaryTrust", root.transform,
            "CONFIANZA 280 / 500", font, 105f, 843f, 370f, 43f, 25f, ivory,
            TextAlignmentOptions.Center, true);

        panel.civilization1Button = CreateMapInvisibleButton(
            "Btn_EnterSanctuary", root.transform, 120f, 881f, 360f, 104f);
        CreateMapText("Label", panel.civilization1Button.transform, "ENTRAR AL SANTUARIO", font,
            18f, 34f, 324f, 58f, 27f, ivory, TextAlignmentOptions.Center, true);

        CreateMapText("ResistanceTitle", root.transform, "TERRITORIO\nPRÓXIMO", font,
            610f, 696f, 350f, 92f, 34f, red, TextAlignmentOptions.Center, true);
        CreateMapSeparator(root.transform, "ResistanceSeparator", 650f, 780f, 270f, red);
        panel.civilization2StateText = CreateMapText("ResistanceState", root.transform, "BLOQUEADO", font,
            625f, 823f, 320f, 40f, 28f, gray, TextAlignmentOptions.Center, true);
        panel.mapCivilization2RequirementText = CreateMapText("ResistanceRequirement", root.transform,
            "SE LOCALIZA CON\n300 DE CONFIANZA", font, 610f, 863f, 350f, 84f, 23f, ivory,
            TextAlignmentOptions.Center, true);
        panel.civilization2Button = CreateMapInvisibleButton(
            "Btn_Resistance", root.transform, 560f, 280f, 465f, 810f);

        CreateMapText("RuinsTitle", root.transform, "RUINAS SEPULTADAS", font,
            300f, 1366f, 480f, 56f, 36f, gray, TextAlignmentOptions.Center, true);
        CreateMapSeparator(root.transform, "RuinsSeparator", 370f, 1408f, 340f, gray);
        panel.civilization3StateText = CreateMapText("RuinsState", root.transform, "BLOQUEADO", font,
            330f, 1438f, 420f, 40f, 27f, gray, TextAlignmentOptions.Center, true);
        panel.mapCivilization3RequirementText = CreateMapText("RuinsRequirement", root.transform,
            "SE DESBLOQUEA CON\nDOMINIO TOTAL 30% O MENOS", font,
            285f, 1488f, 510f, 80f, 21f, ivory, TextAlignmentOptions.Center, true);
        panel.civilization3Button = CreateMapInvisibleButton(
            "Btn_Ruins", root.transform, 275f, 990f, 530f, 600f);

        CreateFirstEntryRawImage(
            "BottomMedallion", root.transform, sanctuaryMedallion, whiteKey,
            490f, 1658f, 100f, 100f, new Rect(0f, 0f, 1f, 1f));
        panel.mapStatusText = CreateMapText("MapStatus", root.transform,
            "EL SANTUARIO ESTÁ DISPONIBLE", font,
            105f, 1783f, 870f, 48f, 30f, green, TextAlignmentOptions.Center, true);
        panel.mapSecondaryStatusText = CreateMapText("MapSecondaryStatus", root.transform,
            "OTRO TERRITORIO SE PERFILA CON LA CONFIANZA.", font,
            85f, 1835f, 910f, 48f, 25f, ivory, TextAlignmentOptions.Center, true);

        panel.mapHelpButton = CreateMapInvisibleButton(
            "Btn_MapHelp", root.transform, 45f, 62f, 125f, 96f);
        TextMeshProUGUI helpGlyph = CreateMapText("Glyph", panel.mapHelpButton.transform, "?", font,
            20f, 2f, 85f, 88f, 72f, bronze, TextAlignmentOptions.Center, true);
        helpGlyph.enableAutoSizing = false;
        helpGlyph.fontSize = 72f;
        panel.mapBackButton = CreateMapInvisibleButton(
            "Btn_MapBack", root.transform, 905f, 62f, 125f, 96f);
        TextMeshProUGUI backGlyph = CreateMapText("Glyph", panel.mapBackButton.transform, "←", font,
            14f, 2f, 97f, 88f, 64f, bronze, TextAlignmentOptions.Center, true);
        backGlyph.enableAutoSizing = false;
        backGlyph.fontSize = 64f;
    }

    private static TextMeshProUGUI CreateMapText(
        string name, Transform parent, string text, TMP_FontAsset font,
        float x, float y, float width, float height, float size,
        Color color, TextAlignmentOptions alignment, bool shadow)
    {
        TextMeshProUGUI label = CreateText(
            name, parent, text, size, alignment, new Vector2(0.5f, 0.5f), Vector2.one);
        SetFirstEntryTopLeft(label.rectTransform, x, y, width, height);
        ConfigureFirstEntryText(label, font, color, size * 0.72f, size,
            FontStyles.Bold, 0.25f, true);
        label.overflowMode = TextOverflowModes.Overflow;
        label.outlineColor = FirstEntryHex("180E08", 240);
        label.outlineWidth = 0.08f;
        label.enableVertexGradient = true;
        label.colorGradient = new VertexGradient(
            Color.Lerp(color, Color.white, 0.30f), Color.Lerp(color, Color.white, 0.30f),
            Color.Lerp(color, Color.black, 0.08f), Color.Lerp(color, Color.black, 0.08f));
        if (shadow)
        {
            Shadow textShadow = Undo.AddComponent<Shadow>(label.gameObject);
            textShadow.effectColor = FirstEntryHex("0A0603", 220);
            textShadow.effectDistance = new Vector2(2f, -3f);
            textShadow.useGraphicAlpha = true;
        }
        return label;
    }

    private static TextMeshProUGUI CreateDefenseText(
        string name, Transform parent, string text, TMP_FontAsset font,
        float x, float y, float width, float height, float size,
        Color color, TextAlignmentOptions alignment)
    {
        TextMeshProUGUI label = CreateMapText(
            name, parent, text, font, x, y, width, height, size,
            color, alignment, true);
        label.enableAutoSizing = false;
        label.fontSize = size;
        label.fontSizeMin = size;
        label.fontSizeMax = size;
        label.textWrappingMode = TextWrappingModes.NoWrap;
        label.overflowMode = TextOverflowModes.Truncate;
        label.margin = new Vector4(2f, 0f, 2f, 0f);
        label.raycastTarget = false;
        return label;
    }

    private static void ConfigureBondText(
        TMP_Text label, float size, float minSize, Vector4 margin,
        float lineSpacing = 0f, float outlineWidth = 0.025f)
    {
        if (label == null)
            return;

        label.enableAutoSizing = minSize < size;
        label.fontSize = size;
        label.fontSizeMax = size;
        label.fontSizeMin = minSize;
        label.characterSpacing = 0f;
        label.lineSpacing = lineSpacing;
        label.margin = margin;
        label.textWrappingMode = TextWrappingModes.NoWrap;
        label.overflowMode = TextOverflowModes.Truncate;
        label.outlineWidth = outlineWidth;

        Shadow textShadow = label.GetComponent<Shadow>();
        if (textShadow != null)
            textShadow.effectDistance = size >= 30f
                ? new Vector2(1f, -1f)
                : new Vector2(0.5f, -0.5f);
    }

    private static void CreateMapSeparator(
        Transform parent, string name, float x, float y, float width, Color color)
    {
        CreateFirstEntryRail(name + "_Left", parent, x, y, width * 0.46f, 2f, color);
        CreateFirstEntryRail(name + "_Right", parent, x + width * 0.54f, y, width * 0.46f, 2f, color);
        CreateFirstEntryDiamond(name + "_Mark", parent, x + width * 0.5f, y + 1f,
            11f, color, FirstEntryHex("16120E"));
    }

    private static Material GetOrCreateMapWhiteKeyMaterial()
    {
        Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(FirstEntryWhiteKeyShaderPath);
        if (shader == null)
            shader = Shader.Find("UI/Dimension2/WhiteKey");
        if (shader == null)
            return null;
        Material material = AssetDatabase.LoadAssetAtPath<Material>(MapWhiteKeyMaterialPath);
        if (material == null)
        {
            material = new Material(shader) { name = "D2MapWhiteKey" };
            AssetDatabase.CreateAsset(material, MapWhiteKeyMaterialPath);
        }
        else if (material.shader != shader)
        {
            material.shader = shader;
        }
        material.SetFloat("_KeyThreshold", 0.53f);
        material.SetFloat("_KeySoftness", 0.10f);
        EditorUtility.SetDirty(material);
        return material;
    }

    private static Material GetOrCreateResistanceChromaKeyMaterial()
    {
        Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(
            ResistanceChromaKeyShaderPath);
        if (shader == null)
            shader = Shader.Find("UI/Dimension2/ResistanceChromaKey");
        if (shader == null)
            return null;
        Material material = AssetDatabase.LoadAssetAtPath<Material>(
            ResistanceChromaKeyMaterialPath);
        if (material == null)
        {
            material = new Material(shader) { name = "D2ResistanceChromaKey" };
            AssetDatabase.CreateAsset(material, ResistanceChromaKeyMaterialPath);
        }
        else if (material.shader != shader)
        {
            material.shader = shader;
        }
        material.SetFloat("_KeyThreshold", 0.08f);
        material.SetFloat("_KeySoftness", 0.02f);
        EditorUtility.SetDirty(material);
        return material;
    }

    private static RawImage CreateResistanceIconCrop(
        string name, Transform parent, Texture2D texture, Material material,
        Rect sourceTopLeftPixels, float x, float y, float width, float height)
    {
        const float sourceWidth = 941f;
        const float sourceHeight = 1672f;
        Rect uv = new Rect(
            sourceTopLeftPixels.x / sourceWidth,
            1f - (sourceTopLeftPixels.y + sourceTopLeftPixels.height) / sourceHeight,
            sourceTopLeftPixels.width / sourceWidth,
            sourceTopLeftPixels.height / sourceHeight);
        RawImage image = CreateFirstEntryRawImage(
            name, parent, texture, material, x, y, width, height, uv);
        image.raycastTarget = false;
        return image;
    }

    private static RawImage CreateResistanceMappedIconCrop(
        string name, Transform parent, Texture2D texture, Material material,
        Rect sourceTopLeftPixels)
    {
        return CreateResistanceIconCrop(
            name, parent, texture, material, sourceTopLeftPixels,
            sourceTopLeftPixels.x / 941f * FirstEntryWidth,
            sourceTopLeftPixels.y / 1672f * FirstEntryHeight,
            sourceTopLeftPixels.width / 941f * FirstEntryWidth,
            sourceTopLeftPixels.height / 1672f * FirstEntryHeight);
    }

    private static RawImage CreateResistanceSourceIconCrop(
        string name, Transform parent, Texture2D texture, Material material,
        Rect sourceTopLeftPixels, Rect destinationTopLeftPixels)
    {
        return CreateResistanceIconCrop(
            name, parent, texture, material, sourceTopLeftPixels,
            destinationTopLeftPixels.x / 941f * FirstEntryWidth,
            destinationTopLeftPixels.y / 1672f * FirstEntryHeight,
            destinationTopLeftPixels.width / 941f * FirstEntryWidth,
            destinationTopLeftPixels.height / 1672f * FirstEntryHeight);
    }

    private static TextMeshProUGUI CreateResistanceReferenceText(
        string name, Transform parent, string text, TMP_FontAsset font,
        float x, float y, float width, float height, float size,
        Color color, TextAlignmentOptions alignment)
    {
        return CreateDefenseText(
            name, parent, text, font,
            x / 941f * FirstEntryWidth,
            y / 1672f * FirstEntryHeight,
            width / 941f * FirstEntryWidth,
            height / 1672f * FirstEntryHeight,
            size / 1672f * FirstEntryHeight,
            color, alignment);
    }

    private static void TuneResistanceReferenceText(
        TMP_Text label, float horizontalScale, float verticalScale,
        float sourceOffsetX = 0f, float sourceOffsetY = 0f)
    {
        if (label == null)
            return;

        RectTransform rect = label.rectTransform;
        Vector2 position = rect.anchoredPosition;
        position.x += (1f - horizontalScale) * rect.sizeDelta.x * 0.5f +
            sourceOffsetX / 941f * FirstEntryWidth;
        position.y -= (1f - verticalScale) * rect.sizeDelta.y * 0.5f +
            sourceOffsetY / 1672f * FirstEntryHeight;
        rect.anchoredPosition = position;
        rect.localScale = new Vector3(horizontalScale, verticalScale, 1f);
    }

    private static void CompressReferenceTextHorizontally(
        TMP_Text label, float horizontalScale)
    {
        if (label == null)
            return;
        horizontalScale = Mathf.Clamp(horizontalScale, 0.1f, 1f);
        RectTransform rect = label.rectTransform;
        rect.sizeDelta = new Vector2(
            rect.sizeDelta.x / horizontalScale, rect.sizeDelta.y);
        rect.localScale = new Vector3(horizontalScale, 1f, 1f);
    }

    private static Button ConfigureMapButton(RawImage target, bool interactable)
    {
        target.raycastTarget = true;
        Button button = Undo.AddComponent<Button>(target.gameObject);
        button.targetGraphic = target;
        button.interactable = interactable;
        button.onClick = new Button.ButtonClickedEvent();
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = FirstEntryHex("FFF0C2");
        colors.pressedColor = FirstEntryHex("B96D38");
        colors.selectedColor = Color.white;
        colors.disabledColor = FirstEntryHex("7A7168", 180);
        colors.fadeDuration = 0.08f;
        button.colors = colors;
        return button;
    }

    private static Button CreateMapInvisibleButton(
        string name, Transform parent, float x, float y, float width, float height)
    {
        GameObject targetObject = CreateUIObject(name, parent);
        RectTransform rect = targetObject.GetComponent<RectTransform>();
        SetFirstEntryTopLeft(rect, x, y, width, height);
        Image target = Undo.AddComponent<Image>(targetObject);
        target.color = new Color(1f, 1f, 1f, 0.001f);
        target.raycastTarget = true;
        Button button = Undo.AddComponent<Button>(targetObject);
        button.targetGraphic = target;
        button.onClick = new Button.ButtonClickedEvent();
        return button;
    }

    private static void BuildCivilization1Placeholder(Dimension2PanelUI panel)
    {
        PrepareFirstEntryTexture(SanctuaryCleanBasePlatePath, 2048);
        PrepareFirstEntryTexture(SanctuaryStaticIconOverlayPath, 2048);
        PrepareFirstEntryTexture(AltarsCleanBasePlatePath, 2048);
        PrepareFirstEntryTexture(AltarsStaticIconOverlayPath, 2048);
        PrepareFirstEntryTexture(AltarsWaxSelectionOverlayPath, 2048);
        PrepareFirstEntryTexture(PilgrimagesCleanBasePlatePath, 2048);
        PrepareFirstEntryTexture(PilgrimagesStaticIconOverlayPath, 2048);
        PrepareFirstEntryTexture(PilgrimagesShortSelectionOverlayPath, 2048);
        PrepareFirstEntryTexture(NovitiateCleanBasePlatePath, 2048);
        PrepareFirstEntryTexture(NovitiateStaticIconOverlayPath, 2048);
        PrepareFirstEntryTexture(RitesCleanBasePlatePath, 2048);
        PrepareFirstEntryTexture(RitesWelcomeNeutralOverlayPath, 2048);
        PrepareFirstEntryTexture(RitesOfferingSelectionOverlayPath, 2048);
        PrepareFirstEntryTexture(RitesPathSelectionOverlayPath, 2048);
        PrepareFirstEntryTexture(RitesNovitiateSelectionOverlayPath, 2048);
        PrepareFirstEntryTexture(RitesRespectSelectionOverlayPath, 2048);
        PrepareFirstEntryTexture(PactsCleanBasePlatePath, 2048);
        PrepareFirstEntryTexture(PactsHospitalityNeutralOverlayPath, 2048);
        PrepareFirstEntryTexture(PactsOpenPathSelectionOverlayPath, 2048);
        PrepareFirstEntryTexture(PactsConsecrationSelectionOverlayPath, 2048);
        PrepareFirstEntryTexture(PactsSilentVowSelectionOverlayPath, 2048);
        PrepareFirstEntryTexture(PactsInnerDoorSelectionOverlayPath, 2048);
        PrepareFirstEntryTexture(PactsHospitalitySlotNeutralOverlayPath, 2048);
        PrepareFirstEntryTexture(PactsSecondSlotUnlockedOverlayPath, 2048);
        PrepareFirstEntryTexture(PactsStaticDetailIconsOverlayPath, 2048);
        PrepareFirstEntryTexture(PactsLockRequirementIconsOverlayPath, 2048);
        PrepareFirstEntryTexture(PactsActionButtonsOverlayPath, 2048);
        PrepareFirstEntryTexture(PactsHospitalityMedallionPath, 1024);
        PrepareFirstEntryTexture(PactsRoadMedallionPath, 1024);
        PrepareFirstEntryTexture(PactsCandleMedallionPath, 1024);
        PrepareFirstEntryTexture(PactsSilentVowMedallionPath, 1024);
        PrepareFirstEntryTexture(PactsDoorMedallionPath, 1024);
        PrepareFirstEntryTexture(BondCleanBasePlatePath, 2048);
        Texture2D basePlate = AssetDatabase.LoadAssetAtPath<Texture2D>(
            SanctuaryCleanBasePlatePath);
        Texture2D iconOverlay = AssetDatabase.LoadAssetAtPath<Texture2D>(
            SanctuaryStaticIconOverlayPath);
        Texture2D altarsBasePlate = AssetDatabase.LoadAssetAtPath<Texture2D>(
            AltarsCleanBasePlatePath);
        Texture2D altarsIconOverlay = AssetDatabase.LoadAssetAtPath<Texture2D>(
            AltarsStaticIconOverlayPath);
        Texture2D altarsSelectionOverlay = AssetDatabase.LoadAssetAtPath<Texture2D>(
            AltarsWaxSelectionOverlayPath);
        Texture2D pilgrimagesBasePlate = AssetDatabase.LoadAssetAtPath<Texture2D>(
            PilgrimagesCleanBasePlatePath);
        Texture2D pilgrimagesIconOverlay = AssetDatabase.LoadAssetAtPath<Texture2D>(
            PilgrimagesStaticIconOverlayPath);
        Texture2D pilgrimagesSelectionOverlay = AssetDatabase.LoadAssetAtPath<Texture2D>(
            PilgrimagesShortSelectionOverlayPath);
        Texture2D novitiateBasePlate = AssetDatabase.LoadAssetAtPath<Texture2D>(
            NovitiateCleanBasePlatePath);
        Texture2D novitiateIconOverlay = AssetDatabase.LoadAssetAtPath<Texture2D>(
            NovitiateStaticIconOverlayPath);
        Texture2D ritesBasePlate = AssetDatabase.LoadAssetAtPath<Texture2D>(RitesCleanBasePlatePath);
        Texture2D ritesWelcomeNeutral = AssetDatabase.LoadAssetAtPath<Texture2D>(RitesWelcomeNeutralOverlayPath);
        Texture2D ritesOfferingSelection = AssetDatabase.LoadAssetAtPath<Texture2D>(RitesOfferingSelectionOverlayPath);
        Texture2D ritesPathSelection = AssetDatabase.LoadAssetAtPath<Texture2D>(RitesPathSelectionOverlayPath);
        Texture2D ritesNovitiateSelection = AssetDatabase.LoadAssetAtPath<Texture2D>(RitesNovitiateSelectionOverlayPath);
        Texture2D ritesRespectSelection = AssetDatabase.LoadAssetAtPath<Texture2D>(RitesRespectSelectionOverlayPath);
        Texture2D pactsBasePlate = AssetDatabase.LoadAssetAtPath<Texture2D>(PactsCleanBasePlatePath);
        Texture2D pactsHospitalityNeutral = AssetDatabase.LoadAssetAtPath<Texture2D>(PactsHospitalityNeutralOverlayPath);
        Texture2D pactsOpenPathSelection = AssetDatabase.LoadAssetAtPath<Texture2D>(PactsOpenPathSelectionOverlayPath);
        Texture2D pactsConsecrationSelection = AssetDatabase.LoadAssetAtPath<Texture2D>(PactsConsecrationSelectionOverlayPath);
        Texture2D pactsSilentVowSelection = AssetDatabase.LoadAssetAtPath<Texture2D>(PactsSilentVowSelectionOverlayPath);
        Texture2D pactsInnerDoorSelection = AssetDatabase.LoadAssetAtPath<Texture2D>(PactsInnerDoorSelectionOverlayPath);
        Texture2D pactsHospitalitySlotNeutral = AssetDatabase.LoadAssetAtPath<Texture2D>(PactsHospitalitySlotNeutralOverlayPath);
        Texture2D pactsSecondSlotUnlocked = AssetDatabase.LoadAssetAtPath<Texture2D>(PactsSecondSlotUnlockedOverlayPath);
        Texture2D pactsStaticDetailIcons = AssetDatabase.LoadAssetAtPath<Texture2D>(PactsStaticDetailIconsOverlayPath);
        Texture2D pactsLockRequirementIcons = AssetDatabase.LoadAssetAtPath<Texture2D>(PactsLockRequirementIconsOverlayPath);
        Texture2D pactsActionButtons = AssetDatabase.LoadAssetAtPath<Texture2D>(PactsActionButtonsOverlayPath);
        Texture2D[] pactsMedallions =
        {
            AssetDatabase.LoadAssetAtPath<Texture2D>(PactsHospitalityMedallionPath),
            AssetDatabase.LoadAssetAtPath<Texture2D>(PactsRoadMedallionPath),
            AssetDatabase.LoadAssetAtPath<Texture2D>(PactsCandleMedallionPath),
            AssetDatabase.LoadAssetAtPath<Texture2D>(PactsSilentVowMedallionPath),
            AssetDatabase.LoadAssetAtPath<Texture2D>(PactsDoorMedallionPath)
        };
        Texture2D bondBasePlate = AssetDatabase.LoadAssetAtPath<Texture2D>(BondCleanBasePlatePath);
        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FirstEntryFontPath);
        if (basePlate == null || iconOverlay == null || altarsBasePlate == null ||
            altarsIconOverlay == null || altarsSelectionOverlay == null ||
            pilgrimagesBasePlate == null || pilgrimagesIconOverlay == null ||
            pilgrimagesSelectionOverlay == null || novitiateBasePlate == null ||
            novitiateIconOverlay == null || ritesBasePlate == null ||
            ritesWelcomeNeutral == null || ritesOfferingSelection == null ||
            ritesPathSelection == null || ritesNovitiateSelection == null ||
            ritesRespectSelection == null || pactsBasePlate == null ||
            pactsHospitalityNeutral == null || pactsOpenPathSelection == null ||
            pactsConsecrationSelection == null || pactsSilentVowSelection == null ||
            pactsInnerDoorSelection == null || pactsHospitalitySlotNeutral == null ||
            pactsSecondSlotUnlocked == null || pactsStaticDetailIcons == null ||
            pactsLockRequirementIcons == null || pactsActionButtons == null ||
            pactsMedallions[0] == null ||
            pactsMedallions[1] == null || pactsMedallions[2] == null ||
            pactsMedallions[3] == null || pactsMedallions[4] == null ||
            bondBasePlate == null || font == null)
        {
            Debug.LogError("[D2 Sanctuary] Faltan la placa limpia, el overlay o Cinzel.");
            return;
        }

        GameObject root = CreateUIObject("D2_Civilization1", panel.transform);
        panel.civilization1Root = root;
        D2Civilization1PanelUI civilization1UI = Undo.AddComponent<D2Civilization1PanelUI>(root);
        panel.civilization1PanelUI = civilization1UI;
        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = rootRect.anchorMax = new Vector2(0.5f, 0.5f);
        rootRect.pivot = new Vector2(0.5f, 0.5f);
        // El Santuario ya ocupa el lienzo completo; a diferencia del mapa no usa
        // una placa con 40 px de compensación interna.
        rootRect.anchoredPosition = new Vector2(0f, -122f);
        rootRect.sizeDelta = new Vector2(FirstEntryWidth, FirstEntryHeight);
        Canvas sanctuaryCanvas = Undo.AddComponent<Canvas>(root);
        sanctuaryCanvas.overrideSorting = true;
        sanctuaryCanvas.sortingOrder = 32742;
        SerializedObject sanctuaryCanvasSettings = new SerializedObject(sanctuaryCanvas);
        sanctuaryCanvasSettings.FindProperty("m_OverrideSorting").boolValue = true;
        sanctuaryCanvasSettings.FindProperty("m_SortingOrder").intValue = 32742;
        sanctuaryCanvasSettings.ApplyModifiedPropertiesWithoutUndo();
        Undo.AddComponent<GraphicRaycaster>(root);

        RawImage baseLayer = CreateFirstEntryRawImage(
            "SanctuaryBasePlate", root.transform, basePlate, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight, new Rect(0f, 0f, 1f, 1f));
        baseLayer.raycastTarget = false;
        RawImage staticIcons = CreateFirstEntryRawImage(
            "SanctuaryStaticIcons", root.transform, iconOverlay, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight, new Rect(0f, 0f, 1f, 1f));
        staticIcons.raycastTarget = false;

        Color ivory = FirstEntryHex("E3C99D");
        Color bronze = FirstEntryHex("C59A5B");
        Color muted = FirstEntryHex("B99A72");
        Color release = FirstEntryHex("D09A70");

        panel.civilization1PlaceholderText = CreateMapText(
            "SanctuaryTitle", root.transform, "EL SANTUARIO", font,
            190f, 38f, 700f, 78f, 56f, ivory, TextAlignmentOptions.Center, true);

        CreateMapText("FollowersHeaderLabel", root.transform, "SEGUIDORES", font,
            128f, 139f, 170f, 30f, 22f, bronze, TextAlignmentOptions.Left, true);
        civilization1UI.followersText = CreateMapText(
            "FollowersHeaderValue", root.transform, "3,845", font,
            140f, 170f, 170f, 48f, 36f, ivory, TextAlignmentOptions.Left, true);
        CreateMapText("TrustHeaderLabel", root.transform, "CONFIANZA", font,
            410f, 139f, 190f, 30f, 22f, bronze, TextAlignmentOptions.Left, true);
        civilization1UI.trustText = CreateMapText(
            "TrustHeaderValue", root.transform, "280 / 500", font,
            410f, 170f, 230f, 48f, 36f, ivory, TextAlignmentOptions.Left, true);
        CreateMapText("WaxHeaderLabel", root.transform, "CERA", font,
            718f, 139f, 100f, 30f, 22f, bronze, TextAlignmentOptions.Left, true);
        civilization1UI.waxText = CreateMapText(
            "WaxHeaderValue", root.transform, "3,450", font,
            720f, 170f, 150f, 48f, 34f, ivory, TextAlignmentOptions.Left, true);
        CreateMapText("BreadHeaderLabel", root.transform, "PAN RITUAL", font,
            906f, 139f, 150f, 30f, 21f, bronze, TextAlignmentOptions.Left, true);
        civilization1UI.ritualBreadText = CreateMapText(
            "BreadHeaderValue", root.transform, "3,450", font,
            914f, 170f, 145f, 48f, 34f, ivory, TextAlignmentOptions.Left, true);

        civilization1UI.showRefugeButton = CreateMapInvisibleButton(
            "Btn_ShowRefuge", root.transform, 49f, 317f, 301f, 307f);
        civilization1UI.showAltarsButton = CreateMapInvisibleButton(
            "Btn_ShowAltars", root.transform, 718f, 324f, 277f, 292f);
        civilization1UI.showPilgrimagesButton = CreateMapInvisibleButton(
            "Btn_ShowPilgrimages", root.transform, 48f, 636f, 310f, 195f);
        civilization1UI.showNovitiateButton = CreateMapInvisibleButton(
            "Btn_ShowNovitiate", root.transform, 716f, 634f, 286f, 196f);
        civilization1UI.showRitesButton = CreateMapInvisibleButton(
            "Btn_ShowRites", root.transform, 48f, 850f, 310f, 198f);
        civilization1UI.showVeiledThresholdButton = CreateMapInvisibleButton(
            "Btn_ShowVeiledThreshold", root.transform, 716f, 851f, 286f, 198f);
        civilization1UI.showPactsButton = CreateMapInvisibleButton(
            "Btn_ShowPacts", root.transform, 48f, 1068f, 310f, 168f);

        CreateMapText("RefugeNavLabel", civilization1UI.showRefugeButton.transform,
            "REFUGIO", font, 0f, 223f, 301f, 70f, 38f, ivory,
            TextAlignmentOptions.Center, true);
        CreateMapText("AltarsNavLabel", civilization1UI.showAltarsButton.transform,
            "ALTARES", font, 0f, 218f, 277f, 66f, 35f, muted,
            TextAlignmentOptions.Center, true);
        CreateMapText("PilgrimagesNavLabel", civilization1UI.showPilgrimagesButton.transform,
            "PEREGRINACIONES", font, 0f, 126f, 310f, 60f, 27f, muted,
            TextAlignmentOptions.Center, true);
        CreateMapText("NovitiateNavLabel", civilization1UI.showNovitiateButton.transform,
            "NOVICIADO", font, 0f, 126f, 286f, 60f, 32f, muted,
            TextAlignmentOptions.Center, true);
        CreateMapText("RitesNavLabel", civilization1UI.showRitesButton.transform,
            "RITOS", font, 0f, 128f, 310f, 58f, 34f, muted,
            TextAlignmentOptions.Center, true);
        CreateMapText("ThresholdNavLabel", civilization1UI.showVeiledThresholdButton.transform,
            "UMBRAL", font, 0f, 128f, 286f, 58f, 34f, muted,
            TextAlignmentOptions.Center, true);
        CreateMapText("PactsNavLabel", civilization1UI.showPactsButton.transform,
            "PACTOS", font, 0f, 103f, 310f, 58f, 34f, muted,
            TextAlignmentOptions.Center, true);

        panel.backToMapButton = CreateMapInvisibleButton(
            "Btn_BackToD2Map", root.transform, 40f, 37f, 106f, 86f);
        panel.sanctuaryHelpButton = CreateMapInvisibleButton(
            "Btn_SanctuaryHelp", root.transform, 935f, 37f, 106f, 86f);

        civilization1UI.refugeSectionRoot = CreateView(
            "D2_Civ1_RefugeSection", root.transform
        );
        Transform refugeRoot = civilization1UI.refugeSectionRoot.transform;
        civilization1UI.refugeText = CreateMapText(
            "RefugeStatus", refugeRoot, "REFUGIO DE PEREGRINOS – NIVEL 4 / 10", font,
            155f, 1260f, 865f, 60f, 35f, ivory, TextAlignmentOptions.Center, true);
        CreateMapText("AvailableFollowersLabel", refugeRoot, "SEGUIDORES DISPONIBLES", font,
            120f, 1340f, 560f, 44f, 25f, bronze, TextAlignmentOptions.Left, true);
        civilization1UI.followersAvailableText = CreateMapText(
            "AvailableFollowersValue", refugeRoot, "3,845", font,
            805f, 1340f, 210f, 44f, 29f, ivory, TextAlignmentOptions.Right, true);
        CreateMapText("AssignedFollowersLabel", refugeRoot, "ASIGNADOS AL REFUGIO", font,
            120f, 1392f, 560f, 44f, 25f, bronze, TextAlignmentOptions.Left, true);
        civilization1UI.assignmentText = CreateMapText(
            "AssignedFollowersValue", refugeRoot, "1,280", font,
            805f, 1392f, 210f, 44f, 29f, ivory, TextAlignmentOptions.Right, true);
        CreateMapText("MultiplierLabel", refugeRoot, "MULTIPLICADOR DE APOYO", font,
            120f, 1444f, 590f, 44f, 25f, bronze, TextAlignmentOptions.Left, true);
        civilization1UI.multiplierText = CreateMapText(
            "MultiplierValue", refugeRoot, "×6.367", font,
            785f, 1444f, 230f, 44f, 29f, ivory, TextAlignmentOptions.Right, true);
        CreateMapText("ArrivalLabel", refugeRoot, "LLEGADA", font,
            120f, 1496f, 330f, 44f, 25f, bronze, TextAlignmentOptions.Left, true);
        civilization1UI.arrivalText = CreateMapText(
            "FollowerArrival", refugeRoot, "68.00 / MIN", font,
            720f, 1496f, 295f, 44f, 29f, ivory, TextAlignmentOptions.Right, true);
        civilization1UI.arrivalProgressSlider = CreateProgressSlider(
            "FollowerArrivalProgress", refugeRoot, new Vector2(0.5f, -0.1f),
            new Vector2(0.001f, 0.001f));
        civilization1UI.arrivalProgressSlider.gameObject.SetActive(false);

        civilization1UI.releaseOneButton = CreateMapInvisibleButton(
            "Btn_ReleaseOneFollower", refugeRoot, 83f, 1562f, 166f, 70f);
        civilization1UI.assignOneButton = CreateMapInvisibleButton(
            "Btn_AssignOneFollower", refugeRoot, 253f, 1562f, 166f, 70f);
        civilization1UI.assignTenButton = CreateMapInvisibleButton(
            "Btn_AssignTenFollowers", refugeRoot, 423f, 1562f, 166f, 70f);
        civilization1UI.assignAllButton = CreateMapInvisibleButton(
            "Btn_AssignAllFollowers", refugeRoot, 593f, 1562f, 166f, 70f);
        civilization1UI.releaseAllButton = CreateMapInvisibleButton(
            "Btn_ReleaseAllFollowers", refugeRoot, 763f, 1562f, 235f, 70f);
        CreateMapText("Label", civilization1UI.releaseOneButton.transform, "−1", font,
            0f, 4f, 166f, 60f, 31f, ivory, TextAlignmentOptions.Center, true);
        CreateMapText("Label", civilization1UI.assignOneButton.transform, "+1", font,
            0f, 4f, 166f, 60f, 31f, ivory, TextAlignmentOptions.Center, true);
        CreateMapText("Label", civilization1UI.assignTenButton.transform, "+10", font,
            0f, 4f, 166f, 60f, 31f, ivory, TextAlignmentOptions.Center, true);
        CreateMapText("Label", civilization1UI.assignAllButton.transform, "TODOS", font,
            0f, 5f, 166f, 58f, 25f, ivory, TextAlignmentOptions.Center, true);
        CreateMapText("Label", civilization1UI.releaseAllButton.transform, "LIBERAR", font,
            0f, 5f, 235f, 58f, 27f, release, TextAlignmentOptions.Center, true);

        civilization1UI.upgradeRefugeButton = CreateMapInvisibleButton(
            "Btn_UpgradeRefuge", refugeRoot, 218f, 1647f, 644f, 96f);
        civilization1UI.upgradeRefugeButtonText = CreateMapText(
            "Label", civilization1UI.upgradeRefugeButton.transform,
            "MEJORAR REFUGIO\n76 SEGUIDORES", font,
            5f, 6f, 500f, 84f, 30f, ivory, TextAlignmentOptions.Center, true);

        civilization1UI.objectiveText = CreateMapText(
            "Civilization1Objective", refugeRoot,
            "AHORA · FORTALECE EL REFUGIO\n→ DESPUÉS · CONTINÚA HACIA 300 DE CONFIANZA.",
            font, 150f, 1784f, 850f, 106f, 28f, bronze,
            TextAlignmentOptions.Left, true);

        civilization1UI.altarsSectionRoot = CreateView(
            "D2_Civ1_AltarsSection", root.transform
        );
        D2AltarsPanelUI altarsUI = Undo.AddComponent<D2AltarsPanelUI>(
            civilization1UI.altarsSectionRoot
        );
        civilization1UI.altarsPanelUI = altarsUI;
        Transform altarsRoot = civilization1UI.altarsSectionRoot.transform;
        RawImage altarsBase = CreateFirstEntryRawImage(
            "AltarsBasePlate", altarsRoot, altarsBasePlate, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight, new Rect(0f, 0f, 1f, 1f));
        // Bloquea las zonas clicables del hub que viven detrás de esta vista.
        altarsBase.raycastTarget = true;
        RawImage altarsIcons = CreateFirstEntryRawImage(
            "AltarsStaticIcons", altarsRoot, altarsIconOverlay, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight, new Rect(0f, 0f, 1f, 1f));
        altarsIcons.raycastTarget = false;
        RawImage waxSelection = CreateFirstEntryRawImage(
            "AltarsWaxSelection", altarsRoot, altarsSelectionOverlay, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight, new Rect(0f, 0f, 1f, 1f));
        waxSelection.raycastTarget = false;
        altarsUI.waxSelectionOverlay = waxSelection.gameObject;

        Color altarIvory = FirstEntryHex("D8B486");
        Color altarBronze = FirstEntryHex("C69A61");
        Color altarGreen = FirstEntryHex("9EBB64");
        Color altarRed = FirstEntryHex("D49A72");

        CreateMapText("AltarsTitle", altarsRoot, "ALTARES DEL SANTUARIO", font,
            210f, 29f, 760f, 70f, 42f, altarIvory, TextAlignmentOptions.Center, true);
        CreateMapText("AltarsFollowersLabel", altarsRoot, "SEGUIDORES", font,
            135f, 119f, 230f, 31f, 23f, altarBronze, TextAlignmentOptions.Left, true);
        altarsUI.headerFollowersText = CreateMapText("AltarsFollowersValue", altarsRoot, "3,845", font,
            135f, 151f, 220f, 45f, 34f, altarIvory, TextAlignmentOptions.Left, true);
        CreateMapText("AltarsWaxLabel", altarsRoot, "CERA", font,
            518f, 119f, 150f, 31f, 23f, altarBronze, TextAlignmentOptions.Left, true);
        altarsUI.headerWaxText = CreateMapText("AltarsWaxValue", altarsRoot, "3,450", font,
            518f, 151f, 170f, 45f, 34f, altarIvory, TextAlignmentOptions.Left, true);
        CreateMapText("AltarsBreadLabel", altarsRoot, "PAN RITUAL", font,
            836f, 119f, 190f, 31f, 22f, altarBronze, TextAlignmentOptions.Left, true);
        altarsUI.headerBreadText = CreateMapText("AltarsBreadValue", altarsRoot, "3,450", font,
            836f, 151f, 180f, 45f, 34f, altarIvory, TextAlignmentOptions.Left, true);

        CreateMapText("WaxCardTitle", altarsRoot, "ALTAR DE\nCERA", font,
            61f, 244f, 284f, 90f, 34f, altarIvory, TextAlignmentOptions.Center, true);
        CreateMapText("BreadCardTitle", altarsRoot, "ALTAR DE\nPAN RITUAL", font,
            391f, 245f, 310f, 90f, 32f, altarIvory, TextAlignmentOptions.Center, true);
        CreateMapText("IncenseCardTitle", altarsRoot, "ALTAR DE\nINCIENSO", font,
            724f, 245f, 310f, 90f, 32f, altarIvory, TextAlignmentOptions.Center, true);
        CreateMapText("ClothCardTitle", altarsRoot, "ALTAR DE\nTELA SAGRADA", font,
            196f, 628f, 321f, 82f, 30f, altarIvory, TextAlignmentOptions.Center, true);
        CreateMapText("StoneCardTitle", altarsRoot, "ALTAR DE\nPIEDRA TALLADA", font,
            560f, 628f, 322f, 82f, 30f, altarIvory, TextAlignmentOptions.Center, true);
        altarsUI.selectedCardStateText = CreateMapText("SelectedCardState", altarsRoot,
            "DISPONIBLE", font, 100f, 548f, 241f, 44f, 25f, altarGreen,
            TextAlignmentOptions.Center, true);

        altarsUI.detailTitleText = CreateMapText("AltarDetailTitle", altarsRoot,
            "ALTAR DE CERA", font, 325f, 1006f, 650f, 61f, 41f, altarIvory,
            TextAlignmentOptions.Center, true);
        altarsUI.offeringNameText = CreateMapText("OfferingName", altarsRoot,
            "OFRENDA  ·  CERA", font, 333f, 1085f, 430f, 50f, 28f, altarBronze,
            TextAlignmentOptions.Left, true);
        altarsUI.offeringValueText = CreateMapText("OfferingValue", altarsRoot,
            "3,450.00", font, 775f, 1085f, 220f, 50f, 30f, altarIvory,
            TextAlignmentOptions.Right, true);
        CreateMapText("ProductionLabel", altarsRoot, "PRODUCCIÓN", font,
            390f, 1157f, 330f, 49f, 28f, altarBronze, TextAlignmentOptions.Left, true);
        altarsUI.productionPerSecondText = CreateMapText("ProductionPerSecond", altarsRoot,
            "0.242 / S", font, 790f, 1151f, 205f, 45f, 31f, altarIvory,
            TextAlignmentOptions.Right, true);
        altarsUI.productionPerMinuteText = CreateMapText("ProductionPerMinute", altarsRoot,
            "(14.50 / MIN)", font, 750f, 1195f, 245f, 42f, 27f, altarIvory,
            TextAlignmentOptions.Right, true);

        CreateMapText("AltarFollowersAvailableLabel", altarsRoot, "SEGUIDORES DISPONIBLES", font,
            170f, 1287f, 600f, 45f, 25f, altarBronze, TextAlignmentOptions.Left, true);
        altarsUI.followersAvailableText = CreateMapText("AltarFollowersAvailableValue", altarsRoot,
            "3,845", font, 795f, 1287f, 180f, 45f, 31f, altarIvory, TextAlignmentOptions.Right, true);
        CreateMapText("AltarFollowersAssignedLabel", altarsRoot, "ASIGNADOS AL ALTAR", font,
            170f, 1360f, 600f, 45f, 25f, altarBronze, TextAlignmentOptions.Left, true);
        altarsUI.followersAssignedText = CreateMapText("AltarFollowersAssignedValue", altarsRoot,
            "120", font, 795f, 1360f, 180f, 45f, 31f, altarIvory, TextAlignmentOptions.Right, true);
        CreateMapText("AltarMultiplierLabel", altarsRoot, "MULTIPLICADOR", font,
            170f, 1435f, 520f, 45f, 25f, altarBronze, TextAlignmentOptions.Left, true);
        altarsUI.multiplierText = CreateMapText("AltarMultiplierValue", altarsRoot,
            "×4.834", font, 770f, 1435f, 205f, 45f, 31f, altarGreen, TextAlignmentOptions.Right, true);

        altarsUI.backButton = CreateMapInvisibleButton("Btn_AltarsBack", altarsRoot, 34f, 28f, 100f, 72f);
        altarsUI.waxCardButton = CreateMapInvisibleButton("Btn_AltarWax", altarsRoot, 35f, 240f, 337f, 370f);
        altarsUI.breadCardButton = CreateMapInvisibleButton("Btn_AltarBread", altarsRoot, 390f, 241f, 312f, 357f);
        altarsUI.incenseCardButton = CreateMapInvisibleButton("Btn_AltarIncense", altarsRoot, 722f, 241f, 319f, 357f);
        altarsUI.clothCardButton = CreateMapInvisibleButton("Btn_AltarCloth", altarsRoot, 196f, 620f, 321f, 351f);
        altarsUI.stoneCardButton = CreateMapInvisibleButton("Btn_AltarStone", altarsRoot, 561f, 620f, 321f, 351f);

        altarsUI.releaseOneButton = CreateMapInvisibleButton("Btn_AltarReleaseOne", altarsRoot, 84f, 1547f, 108f, 76f);
        altarsUI.assignOneButton = CreateMapInvisibleButton("Btn_AltarAssignOne", altarsRoot, 211f, 1547f, 108f, 76f);
        altarsUI.assignTenButton = CreateMapInvisibleButton("Btn_AltarAssignTen", altarsRoot, 338f, 1547f, 113f, 76f);
        altarsUI.assignAllButton = CreateMapInvisibleButton("Btn_AltarAssignAll", altarsRoot, 479f, 1547f, 260f, 76f);
        altarsUI.releaseAllButton = CreateMapInvisibleButton("Btn_AltarReleaseAll", altarsRoot, 762f, 1547f, 229f, 76f);
        altarsUI.assignAllButton.targetGraphic.color = FirstEntryHex("9C6426", 105);
        altarsUI.releaseAllButton.targetGraphic.color = FirstEntryHex("6D2C20", 135);
        CreateMapText("Label", altarsUI.releaseOneButton.transform, "−1", font, 0f, 4f, 108f, 66f, 36f, altarIvory, TextAlignmentOptions.Center, true);
        CreateMapText("Label", altarsUI.assignOneButton.transform, "+1", font, 0f, 4f, 108f, 66f, 36f, altarIvory, TextAlignmentOptions.Center, true);
        CreateMapText("Label", altarsUI.assignTenButton.transform, "+10", font, 0f, 4f, 113f, 66f, 36f, altarIvory, TextAlignmentOptions.Center, true);
        CreateMapText("Label", altarsUI.assignAllButton.transform, "ASIGNAR TODOS", font, 0f, 5f, 260f, 64f, 25f, altarIvory, TextAlignmentOptions.Center, true);
        CreateMapText("Label", altarsUI.releaseAllButton.transform, "LIBERAR TODOS", font, 0f, 5f, 229f, 64f, 25f, altarRed, TextAlignmentOptions.Center, true);

        altarsUI.refugeNavButton = CreateMapInvisibleButton("Btn_AltarsNavRefuge", altarsRoot, 25f, 1701f, 146f, 190f);
        Button activeAltarsTab = CreateMapInvisibleButton("Btn_AltarsNavActive", altarsRoot, 176f, 1701f, 143f, 190f);
        activeAltarsTab.interactable = false;
        altarsUI.pilgrimagesNavButton = CreateMapInvisibleButton("Btn_AltarsNavPilgrimages", altarsRoot, 321f, 1701f, 143f, 190f);
        altarsUI.novitiateNavButton = CreateMapInvisibleButton("Btn_AltarsNavNovitiate", altarsRoot, 467f, 1701f, 143f, 190f);
        altarsUI.ritesNavButton = CreateMapInvisibleButton("Btn_AltarsNavRites", altarsRoot, 614f, 1701f, 143f, 190f);
        altarsUI.pactsNavButton = CreateMapInvisibleButton("Btn_AltarsNavPacts", altarsRoot, 760f, 1701f, 143f, 190f);
        altarsUI.thresholdNavButton = CreateMapInvisibleButton("Btn_AltarsNavThreshold", altarsRoot, 907f, 1701f, 146f, 190f);
        CreateMapText("RefugeTabLabel", altarsRoot, "REFUGIO", font, 27f, 1831f, 142f, 47f, 22f, altarBronze, TextAlignmentOptions.Center, true);
        CreateMapText("AltarsTabLabel", altarsRoot, "ALTARES", font, 177f, 1831f, 141f, 47f, 22f, altarIvory, TextAlignmentOptions.Center, true);
        CreateMapText("PilgrimagesTabLabel", altarsRoot, "PEREGRINACIONES", font, 319f, 1831f, 148f, 47f, 13f, altarBronze, TextAlignmentOptions.Center, true);
        CreateMapText("NovitiateTabLabel", altarsRoot, "NOVICIADO", font, 467f, 1831f, 143f, 47f, 20f, altarBronze, TextAlignmentOptions.Center, true);
        CreateMapText("RitesTabLabel", altarsRoot, "RITOS", font, 614f, 1831f, 143f, 47f, 22f, altarBronze, TextAlignmentOptions.Center, true);
        CreateMapText("PactsTabLabel", altarsRoot, "PACTOS", font, 760f, 1831f, 143f, 47f, 22f, altarBronze, TextAlignmentOptions.Center, true);
        CreateMapText("ThresholdTabLabel", altarsRoot, "UMBRAL", font, 908f, 1831f, 143f, 47f, 22f, altarBronze, TextAlignmentOptions.Center, true);

        altarsUI.altarDropdown = CreateDropdown("AltarSelector", altarsRoot,
            new Vector2(0.5f, 0.5f), new Vector2(0.01f, 0.01f));
        altarsUI.altarDropdown.gameObject.SetActive(false);
        civilization1UI.altarsSectionRoot.SetActive(false);

        civilization1UI.pilgrimagesSectionRoot = CreateView(
            "D2_Civ1_PilgrimagesSection", root.transform
        );
        D2PilgrimagesPanelUI pilgrimagesUI = Undo.AddComponent<D2PilgrimagesPanelUI>(
            civilization1UI.pilgrimagesSectionRoot
        );
        civilization1UI.pilgrimagesPanelUI = pilgrimagesUI;
        Transform pilgrimagesRoot = civilization1UI.pilgrimagesSectionRoot.transform;
        RawImage pilgrimagesBase = CreateFirstEntryRawImage(
            "PilgrimagesBasePlate", pilgrimagesRoot, pilgrimagesBasePlate, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight, new Rect(0f, 0f, 1f, 1f));
        pilgrimagesBase.raycastTarget = true;
        RawImage pilgrimagesIcons = CreateFirstEntryRawImage(
            "PilgrimagesStaticIcons", pilgrimagesRoot, pilgrimagesIconOverlay, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight, new Rect(0f, 0f, 1f, 1f));
        pilgrimagesIcons.raycastTarget = false;
        RawImage shortSelection = CreateFirstEntryRawImage(
            "PilgrimagesShortSelection", pilgrimagesRoot, pilgrimagesSelectionOverlay, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight, new Rect(0f, 0f, 1f, 1f));
        shortSelection.raycastTarget = false;
        pilgrimagesUI.shortSelectionOverlay = shortSelection.gameObject;

        Color pilgrimageIvory = FirstEntryHex("D8B486");
        Color pilgrimageBronze = FirstEntryHex("C69A61");
        Color pilgrimageGreen = FirstEntryHex("9CB65B");

        CreateMapText("PilgrimagesTitle", pilgrimagesRoot, "PEREGRINACIONES", font,
            190f, 33f, 700f, 70f, 47f, pilgrimageIvory, TextAlignmentOptions.Center, true);
        CreateMapText("PilgrimagesTrustLabel", pilgrimagesRoot, "CONFIANZA", font,
            120f, 111f, 210f, 30f, 22f, pilgrimageBronze, TextAlignmentOptions.Left, true);
        pilgrimagesUI.headerTrustText = CreateMapText("PilgrimagesTrustValue", pilgrimagesRoot,
            "80 / 500", font, 120f, 144f, 205f, 43f, 31f, pilgrimageIvory,
            TextAlignmentOptions.Left, true);
        CreateMapText("PilgrimagesFollowersLabel", pilgrimagesRoot, "SEGUIDORES", font,
            418f, 111f, 175f, 30f, 22f, pilgrimageBronze, TextAlignmentOptions.Left, true);
        pilgrimagesUI.headerFollowersText = CreateMapText("PilgrimagesFollowersValue", pilgrimagesRoot,
            "3,845", font, 418f, 144f, 175f, 43f, 31f, pilgrimageIvory,
            TextAlignmentOptions.Left, true);
        CreateMapText("PilgrimagesWaxLabel", pilgrimagesRoot, "CERA", font,
            657f, 111f, 125f, 30f, 22f, pilgrimageBronze, TextAlignmentOptions.Left, true);
        pilgrimagesUI.headerWaxText = CreateMapText("PilgrimagesWaxValue", pilgrimagesRoot,
            "3,450", font, 657f, 144f, 155f, 43f, 31f, pilgrimageIvory,
            TextAlignmentOptions.Left, true);
        CreateMapText("PilgrimagesBreadLabel", pilgrimagesRoot, "PAN RITUAL", font,
            873f, 111f, 160f, 30f, 21f, pilgrimageBronze, TextAlignmentOptions.Left, true);
        pilgrimagesUI.headerBreadText = CreateMapText("PilgrimagesBreadValue", pilgrimagesRoot,
            "3,450", font, 873f, 144f, 160f, 43f, 31f, pilgrimageIvory,
            TextAlignmentOptions.Left, true);

        CreateMapText("ShortCardTitle", pilgrimagesRoot, "PEREGRINACIÓN CORTA", font,
            177f, 230f, 320f, 45f, 27f, pilgrimageIvory, TextAlignmentOptions.Center, true);
        pilgrimagesUI.shortBodyText = CreateMapText("ShortCardBody", pilgrimagesRoot, "", font,
            238f, 274f, 246f, 170f, 21f, pilgrimageBronze, TextAlignmentOptions.Left, true);
        pilgrimagesUI.shortRewardText = CreateMapText("ShortCardReward", pilgrimagesRoot, "", font,
            86f, 455f, 395f, 55f, 18f, pilgrimageBronze, TextAlignmentOptions.Center, true);

        CreateMapText("MediumCardTitle", pilgrimagesRoot, "PEREGRINACIÓN MEDIA", font,
            177f, 558f, 320f, 44f, 27f, pilgrimageIvory, TextAlignmentOptions.Center, true);
        pilgrimagesUI.mediumBodyText = CreateMapText("MediumCardBody", pilgrimagesRoot, "", font,
            238f, 603f, 246f, 170f, 21f, pilgrimageBronze, TextAlignmentOptions.Left, true);
        pilgrimagesUI.mediumRewardText = CreateMapText("MediumCardReward", pilgrimagesRoot, "", font,
            80f, 780f, 405f, 68f, 17f, pilgrimageBronze, TextAlignmentOptions.Center, true);

        CreateMapText("LongCardTitle", pilgrimagesRoot, "PEREGRINACIÓN LARGA", font,
            177f, 882f, 320f, 44f, 27f, pilgrimageIvory, TextAlignmentOptions.Center, true);
        pilgrimagesUI.longBodyText = CreateMapText("LongCardBody", pilgrimagesRoot, "", font,
            238f, 927f, 246f, 164f, 21f, pilgrimageBronze, TextAlignmentOptions.Left, true);
        pilgrimagesUI.longRewardText = CreateMapText("LongCardReward", pilgrimagesRoot, "", font,
            80f, 1091f, 405f, 68f, 17f, pilgrimageBronze, TextAlignmentOptions.Center, true);

        CreateMapText("GuidedCardTitle", pilgrimagesRoot, "LARGA CON ACÓLITO", font,
            177f, 1193f, 320f, 44f, 27f, pilgrimageIvory, TextAlignmentOptions.Center, true);
        pilgrimagesUI.guidedLongBodyText = CreateMapText("GuidedCardBody", pilgrimagesRoot, "", font,
            238f, 1238f, 246f, 184f, 20f, pilgrimageBronze, TextAlignmentOptions.Left, true);
        pilgrimagesUI.guidedLongRewardText = CreateMapText("GuidedCardReward", pilgrimagesRoot, "", font,
            80f, 1425f, 405f, 68f, 17f, pilgrimageBronze, TextAlignmentOptions.Center, true);

        CreateMapText("SacredCardTitle", pilgrimagesRoot, "PEREGRINACIÓN SAGRADA", font,
            177f, 1512f, 320f, 44f, 25f, pilgrimageIvory, TextAlignmentOptions.Center, true);
        pilgrimagesUI.sacredBodyText = CreateMapText("SacredCardBody", pilgrimagesRoot, "", font,
            238f, 1554f, 246f, 177f, 19f, pilgrimageBronze, TextAlignmentOptions.Left, true);
        pilgrimagesUI.sacredRewardText = CreateMapText("SacredCardReward", pilgrimagesRoot, "", font,
            80f, 1722f, 405f, 56f, 16f, pilgrimageBronze, TextAlignmentOptions.Center, true);

        pilgrimagesUI.selectedTitleText = CreateMapText("SelectedPilgrimageTitle", pilgrimagesRoot,
            "PEREGRINACIÓN CORTA", font, 565f, 858f, 455f, 63f, 34f,
            pilgrimageIvory, TextAlignmentOptions.Center, true);
        CreateMapText("AdditionalSupportLabel", pilgrimagesRoot, "APOYO ADICIONAL", font,
            612f, 930f, 360f, 43f, 23f, pilgrimageBronze, TextAlignmentOptions.Center, true);
        pilgrimagesUI.supportText = CreateMapText("PilgrimageSupport", pilgrimagesRoot,
            "0 / 4\nSEGUIDORES", font, 749f, 981f, 170f, 111f, 25f,
            pilgrimageIvory, TextAlignmentOptions.Center, true);
        CreateMapText("MaterialBonusLabel", pilgrimagesRoot, "BONUS MATERIAL", font,
            630f, 1110f, 330f, 40f, 22f, pilgrimageBronze, TextAlignmentOptions.Center, true);
        pilgrimagesUI.materialBonusText = CreateMapText("MaterialBonusValue", pilgrimagesRoot,
            "+0%", font, 680f, 1151f, 235f, 55f, 35f,
            pilgrimageGreen, TextAlignmentOptions.Center, true);
        pilgrimagesUI.activePilgrimageText = CreateMapText("ActivePilgrimage", pilgrimagesRoot,
            "NO HAY UNA\nPEREGRINACIÓN ACTIVA", font, 650f, 1260f, 350f, 116f, 23f,
            pilgrimageBronze, TextAlignmentOptions.Center, true);

        pilgrimagesUI.removeSupportButton = CreateMapInvisibleButton(
            "Btn_PilgrimageSupportRemove", pilgrimagesRoot, 576f, 981f, 95f, 94f);
        pilgrimagesUI.addSupportButton = CreateMapInvisibleButton(
            "Btn_PilgrimageSupportAdd", pilgrimagesRoot, 923f, 981f, 95f, 94f);
        CreateMapText("Label", pilgrimagesUI.removeSupportButton.transform, "−", font,
            0f, 0f, 95f, 88f, 44f, pilgrimageIvory, TextAlignmentOptions.Center, true);
        CreateMapText("Label", pilgrimagesUI.addSupportButton.transform, "+", font,
            0f, 0f, 95f, 88f, 44f, pilgrimageIvory, TextAlignmentOptions.Center, true);

        pilgrimagesUI.primaryActionButton = CreateMapInvisibleButton(
            "Btn_PilgrimagePrimaryAction", pilgrimagesRoot, 570f, 1440f, 452f, 158f);
        pilgrimagesUI.primaryActionButtonText = CreateMapText(
            "Label", pilgrimagesUI.primaryActionButton.transform,
            "INICIAR\nPEREGRINACIÓN CORTA", font, 8f, 9f, 436f, 140f, 32f,
            pilgrimageIvory, TextAlignmentOptions.Center, true);
        CreateMapText("PilgrimageCancelWarning", pilgrimagesRoot,
            "CANCELAR DEVUELVE SEGUIDORES;\nLAS OFRENDAS NO SE RECUPERAN.", font,
            566f, 1620f, 455f, 72f, 17f, pilgrimageBronze,
            TextAlignmentOptions.Center, true);
        pilgrimagesUI.lastResultText = CreateMapText("PilgrimageLastResult", pilgrimagesRoot,
            "", font, 565f, 1385f, 455f, 48f, 15f, pilgrimageBronze,
            TextAlignmentOptions.Center, true);

        pilgrimagesUI.backButton = CreateMapInvisibleButton(
            "Btn_PilgrimagesBack", pilgrimagesRoot, 31f, 28f, 95f, 75f);
        pilgrimagesUI.startShortButton = CreateMapInvisibleButton(
            "Btn_SelectShortPilgrimage", pilgrimagesRoot, 34f, 221f, 483f, 318f);
        pilgrimagesUI.startMediumButton = CreateMapInvisibleButton(
            "Btn_SelectMediumPilgrimage", pilgrimagesRoot, 34f, 548f, 483f, 317f);
        pilgrimagesUI.startLongButton = CreateMapInvisibleButton(
            "Btn_SelectLongPilgrimage", pilgrimagesRoot, 34f, 873f, 483f, 305f);
        pilgrimagesUI.startGuidedLongButton = CreateMapInvisibleButton(
            "Btn_SelectGuidedLongPilgrimage", pilgrimagesRoot, 34f, 1186f, 483f, 312f);
        pilgrimagesUI.startSacredButton = CreateMapInvisibleButton(
            "Btn_SelectSacredPilgrimage", pilgrimagesRoot, 34f, 1507f, 483f, 271f);
        pilgrimagesUI.cancelButton = CreateMapInvisibleButton(
            "Btn_CancelPilgrimageLegacy", pilgrimagesRoot, 0f, 0f, 1f, 1f);
        pilgrimagesUI.cancelButton.gameObject.SetActive(false);

        pilgrimagesUI.refugeNavButton = CreateMapInvisibleButton(
            "Btn_PilgrimagesNavRefuge", pilgrimagesRoot, 28f, 1777f, 145f, 118f);
        pilgrimagesUI.altarsNavButton = CreateMapInvisibleButton(
            "Btn_PilgrimagesNavAltars", pilgrimagesRoot, 174f, 1777f, 145f, 118f);
        Button activePilgrimagesTab = CreateMapInvisibleButton(
            "Btn_PilgrimagesNavActive", pilgrimagesRoot, 320f, 1777f, 167f, 118f);
        activePilgrimagesTab.interactable = false;
        pilgrimagesUI.novitiateNavButton = CreateMapInvisibleButton(
            "Btn_PilgrimagesNavNovitiate", pilgrimagesRoot, 523f, 1710f, 129f, 181f);
        pilgrimagesUI.ritesNavButton = CreateMapInvisibleButton(
            "Btn_PilgrimagesNavRites", pilgrimagesRoot, 653f, 1710f, 130f, 181f);
        pilgrimagesUI.pactsNavButton = CreateMapInvisibleButton(
            "Btn_PilgrimagesNavPacts", pilgrimagesRoot, 784f, 1710f, 130f, 181f);
        pilgrimagesUI.thresholdNavButton = CreateMapInvisibleButton(
            "Btn_PilgrimagesNavThreshold", pilgrimagesRoot, 915f, 1710f, 132f, 181f);
        CreateMapText("RefugeTabLabel", pilgrimagesRoot, "REFUGIO", font,
            29f, 1841f, 143f, 44f, 20f, pilgrimageBronze, TextAlignmentOptions.Center, true);
        CreateMapText("AltarsTabLabel", pilgrimagesRoot, "ALTARES", font,
            175f, 1841f, 143f, 44f, 20f, pilgrimageBronze, TextAlignmentOptions.Center, true);
        CreateMapText("PilgrimagesTabLabel", pilgrimagesRoot, "PEREGRINACIONES", font,
            320f, 1841f, 167f, 44f, 14f, pilgrimageIvory, TextAlignmentOptions.Center, true);
        CreateMapText("NovitiateTabLabel", pilgrimagesRoot, "NOVICIADO", font,
            523f, 1838f, 129f, 46f, 17f, pilgrimageBronze, TextAlignmentOptions.Center, true);
        CreateMapText("RitesTabLabel", pilgrimagesRoot, "RITOS", font,
            653f, 1838f, 130f, 46f, 20f, pilgrimageBronze, TextAlignmentOptions.Center, true);
        CreateMapText("PactsTabLabel", pilgrimagesRoot, "PACTOS", font,
            784f, 1838f, 130f, 46f, 20f, pilgrimageBronze, TextAlignmentOptions.Center, true);
        CreateMapText("ThresholdTabLabel", pilgrimagesRoot, "UMBRAL", font,
            915f, 1838f, 132f, 46f, 19f, pilgrimageBronze, TextAlignmentOptions.Center, true);
        civilization1UI.pilgrimagesSectionRoot.SetActive(false);

        civilization1UI.novitiateSectionRoot = CreateView(
            "D2_Civ1_NovitiateSection", root.transform
        );
        D2NovitiatePanelUI novitiateUI = Undo.AddComponent<D2NovitiatePanelUI>(
            civilization1UI.novitiateSectionRoot
        );
        civilization1UI.novitiatePanelUI = novitiateUI;
        Transform novitiateRoot = civilization1UI.novitiateSectionRoot.transform;
        RawImage novitiateBase = CreateFirstEntryRawImage(
            "NovitiateBasePlate", novitiateRoot, novitiateBasePlate, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight, new Rect(0f, 0f, 1f, 1f));
        novitiateBase.raycastTarget = true;
        CreateFirstEntryRawImage("NovitiateStaticIcons", novitiateRoot,
            novitiateIconOverlay, null, 0f, 0f, FirstEntryWidth, FirstEntryHeight,
            new Rect(0f, 0f, 1f, 1f));

        Color novitiateIvory = FirstEntryHex("D8B486");
        Color novitiateBronze = FirstEntryHex("C69A61");
        Color novitiateDisabled = FirstEntryHex("6F665A");
        CreateMapText("NovitiateTitle", novitiateRoot, "NOVICIADO", font,
            220f, 38f, 640f, 76f, 49f, novitiateIvory, TextAlignmentOptions.Center, true);

        CreateMapText("FollowersHeaderLabel", novitiateRoot, "SEGUIDORES\nDISPONIBLES", font,
            102f, 116f, 160f, 52f, 16f, novitiateBronze, TextAlignmentOptions.Center, true);
        novitiateUI.headerFollowersText = CreateMapText("FollowersHeaderValue", novitiateRoot,
            "3,845", font, 102f, 166f, 160f, 42f, 29f, novitiateIvory, TextAlignmentOptions.Center, true);
        CreateMapText("AcolytesHeaderLabel", novitiateRoot, "ACÓLITOS\nDISPONIBLES", font,
            307f, 116f, 155f, 52f, 16f, novitiateBronze, TextAlignmentOptions.Center, true);
        novitiateUI.headerAcolytesText = CreateMapText("AcolytesHeaderValue", novitiateRoot,
            "1,280", font, 307f, 166f, 155f, 42f, 29f, novitiateIvory, TextAlignmentOptions.Center, true);
        CreateMapText("PresentHeaderLabel", novitiateRoot, "TOTALES\nPRESENTES", font,
            516f, 116f, 155f, 52f, 16f, novitiateBronze, TextAlignmentOptions.Center, true);
        novitiateUI.headerPresentText = CreateMapText("PresentHeaderValue", novitiateRoot,
            "1,280", font, 516f, 166f, 155f, 42f, 29f, novitiateIvory, TextAlignmentOptions.Center, true);
        CreateMapText("WaxHeaderLabel", novitiateRoot, "CERA", font,
            719f, 122f, 120f, 30f, 17f, novitiateBronze, TextAlignmentOptions.Center, true);
        novitiateUI.headerWaxText = CreateMapText("WaxHeaderValue", novitiateRoot,
            "3,450", font, 719f, 164f, 120f, 42f, 29f, novitiateIvory, TextAlignmentOptions.Center, true);
        CreateMapText("BreadHeaderLabel", novitiateRoot, "PAN RITUAL", font,
            899f, 122f, 150f, 30f, 16f, novitiateBronze, TextAlignmentOptions.Center, true);
        novitiateUI.headerBreadText = CreateMapText("BreadHeaderValue", novitiateRoot,
            "3,450", font, 899f, 164f, 150f, 42f, 29f, novitiateIvory, TextAlignmentOptions.Center, true);

        novitiateUI.levelText = CreateMapText("NovitiateLevel", novitiateRoot, "NIVEL II", font,
            310f, 276f, 460f, 57f, 35f, novitiateIvory, TextAlignmentOptions.Center, true);
        CreateMapText("CurrentBatchLabel", novitiateRoot, "TANDA ACTUAL", font,
            320f, 714f, 440f, 54f, 30f, novitiateBronze, TextAlignmentOptions.Center, true);
        novitiateUI.batchAcolytesText = CreateMapText("BatchAcolytes", novitiateRoot,
            "2\nACÓLITOS", font, 145f, 783f, 205f, 142f, 27f, novitiateIvory, TextAlignmentOptions.Center, true);
        novitiateUI.batchDurationText = CreateMapText("BatchDuration", novitiateRoot,
            "06:00", font, 432f, 808f, 180f, 65f, 38f, novitiateIvory, TextAlignmentOptions.Center, true);
        CreateMapText("BatchCostLabel", novitiateRoot, "COSTE", font,
            730f, 774f, 240f, 34f, 24f, novitiateBronze, TextAlignmentOptions.Center, true);
        novitiateUI.batchFollowerCostText = CreateMapText("BatchFollowersCost", novitiateRoot,
            "10 SEGUIDORES", font, 715f, 810f, 280f, 35f, 22f, novitiateIvory, TextAlignmentOptions.Left, true);
        novitiateUI.batchWaxCostText = CreateMapText("BatchWaxCost", novitiateRoot,
            "22 CERA", font, 715f, 848f, 280f, 35f, 22f, novitiateIvory, TextAlignmentOptions.Left, true);
        novitiateUI.batchBreadCostText = CreateMapText("BatchBreadCost", novitiateRoot,
            "22 PAN RITUAL", font, 715f, 886f, 280f, 35f, 21f, novitiateIvory, TextAlignmentOptions.Left, true);

        CreateMapText("AdditionalSupportLabel", novitiateRoot, "APOYO ADICIONAL", font,
            330f, 964f, 420f, 46f, 24f, novitiateBronze, TextAlignmentOptions.Center, true);
        novitiateUI.supportText = CreateMapText("NovitiateSupport", novitiateRoot,
            "0 / 4\nSEGUIDORES", font, 400f, 1022f, 280f, 84f, 27f, novitiateIvory, TextAlignmentOptions.Center, true);
        novitiateUI.removeSupportButton = CreateMapInvisibleButton(
            "Btn_NovitiateSupportRemove", novitiateRoot, 157f, 1021f, 118f, 96f);
        novitiateUI.addSupportButton = CreateMapInvisibleButton(
            "Btn_NovitiateSupportAdd", novitiateRoot, 783f, 1021f, 118f, 96f);
        CreateMapText("Label", novitiateUI.removeSupportButton.transform, "−", font,
            0f, 0f, 118f, 90f, 45f, novitiateIvory, TextAlignmentOptions.Center, true);
        CreateMapText("Label", novitiateUI.addSupportButton.transform, "+", font,
            0f, 0f, 118f, 90f, 45f, novitiateIvory, TextAlignmentOptions.Center, true);

        novitiateUI.activeTrainingText = CreateMapText("NovitiateActiveTraining", novitiateRoot,
            "NO HAY UNA TANDA EN FORMACIÓN", font, 165f, 1135f, 750f, 72f, 24f,
            novitiateBronze, TextAlignmentOptions.Center, true);
        novitiateUI.startTrainingButton = CreateMapInvisibleButton(
            "Btn_StartNovitiateTraining", novitiateRoot, 139f, 1225f, 794f, 141f);
        novitiateUI.startTrainingButtonText = CreateMapText("Title",
            novitiateUI.startTrainingButton.transform, "FORMAR TANDA", font,
            10f, 10f, 774f, 65f, 38f, novitiateIvory, TextAlignmentOptions.Center, true);
        novitiateUI.startCostText = CreateMapText("Cost",
            novitiateUI.startTrainingButton.transform, "10 SEGUIDORES     22 CERA     22 PAN RITUAL", font,
            25f, 77f, 744f, 48f, 19f, novitiateBronze, TextAlignmentOptions.Center, true);
        novitiateUI.cancelTrainingButton = CreateMapInvisibleButton(
            "Btn_CancelNovitiateTraining", novitiateRoot, 181f, 1386f, 712f, 106f);
        CreateMapText("Label", novitiateUI.cancelTrainingButton.transform, "CANCELAR FORMACIÓN", font,
            8f, 8f, 696f, 90f, 31f, novitiateDisabled, TextAlignmentOptions.Center, true);

        novitiateUI.upgradeButton = CreateMapInvisibleButton(
            "Btn_UpgradeNovitiate", novitiateRoot, 61f, 1509f, 959f, 142f);
        novitiateUI.upgradeButtonText = CreateMapText("Title", novitiateUI.upgradeButton.transform,
            "MEJORAR A NIVEL III", font, 10f, 10f, 939f, 66f, 34f,
            novitiateIvory, TextAlignmentOptions.Center, true);
        novitiateUI.upgradeCostText = CreateMapText("Cost", novitiateUI.upgradeButton.transform,
            "60 SEGUIDORES     75 CERA     75 PAN RITUAL", font, 35f, 77f, 889f, 48f, 19f,
            novitiateBronze, TextAlignmentOptions.Center, true);

        novitiateUI.backButton = CreateMapInvisibleButton("Btn_NovitiateBack", novitiateRoot,
            36f, 36f, 96f, 79f);
        novitiateUI.refugeNavButton = CreateMapInvisibleButton("Btn_NovitiateNavRefuge", novitiateRoot,
            23f, 1737f, 148f, 163f);
        novitiateUI.altarsNavButton = CreateMapInvisibleButton("Btn_NovitiateNavAltars", novitiateRoot,
            171f, 1737f, 148f, 163f);
        novitiateUI.pilgrimagesNavButton = CreateMapInvisibleButton("Btn_NovitiateNavPilgrimages", novitiateRoot,
            319f, 1737f, 160f, 163f);
        Button activeNovitiateTab = CreateMapInvisibleButton("Btn_NovitiateNavActive", novitiateRoot,
            479f, 1737f, 148f, 163f);
        activeNovitiateTab.interactable = false;
        novitiateUI.ritesNavButton = CreateMapInvisibleButton("Btn_NovitiateNavRites", novitiateRoot,
            627f, 1737f, 148f, 163f);
        novitiateUI.pactsNavButton = CreateMapInvisibleButton("Btn_NovitiateNavPacts", novitiateRoot,
            775f, 1737f, 148f, 163f);
        novitiateUI.thresholdNavButton = CreateMapInvisibleButton("Btn_NovitiateNavThreshold", novitiateRoot,
            923f, 1737f, 134f, 163f);
        string[] novitiateTabs = { "REFUGIO", "ALTARES", "PEREGRINACIONES", "NOVICIADO", "RITOS", "PACTOS", "UMBRAL" };
        float[] tabXs = { 23f, 171f, 319f, 479f, 627f, 775f, 923f };
        float[] tabWs = { 148f, 148f, 160f, 148f, 148f, 148f, 134f };
        for (int tabIndex = 0; tabIndex < novitiateTabs.Length; tabIndex++)
            CreateMapText("NovitiateTabLabel" + tabIndex, novitiateRoot, novitiateTabs[tabIndex], font,
                tabXs[tabIndex], 1844f, tabWs[tabIndex], 42f,
                tabIndex == 2 ? 13f : 18f,
                tabIndex == 3 ? novitiateIvory : novitiateBronze,
                TextAlignmentOptions.Center, true);

        novitiateUI.acolytesText = novitiateUI.headerAcolytesText;
        novitiateUI.resourcesText = novitiateUI.headerFollowersText;
        novitiateUI.batchText = novitiateUI.batchAcolytesText;
        novitiateUI.lastResultText = novitiateUI.activeTrainingText;
        civilization1UI.novitiateSectionRoot.SetActive(false);

        civilization1UI.ritesSectionRoot = CreateView(
            "D2_Civ1_RitesSection", root.transform
        );
        D2RitesPanelUI ritesUI = Undo.AddComponent<D2RitesPanelUI>(
            civilization1UI.ritesSectionRoot
        );
        civilization1UI.ritesPanelUI = ritesUI;
        Transform ritesRoot = civilization1UI.ritesSectionRoot.transform;
        RawImage ritesBase = CreateFirstEntryRawImage("RitesBasePlate", ritesRoot,
            ritesBasePlate, null, 0f, 0f, FirstEntryWidth, FirstEntryHeight,
            new Rect(0f, 0f, 1f, 1f));
        ritesBase.raycastTarget = true;
        ritesUI.welcomeNeutralOverlay = CreateFirstEntryRawImage("WelcomeNeutralOverlay", ritesRoot,
            ritesWelcomeNeutral, null, 0f, 0f, FirstEntryWidth, FirstEntryHeight,
            new Rect(0f, 0f, 1f, 1f)).gameObject;
        ritesUI.offeringSelectionOverlay = CreateFirstEntryRawImage("OfferingSelectionOverlay", ritesRoot,
            ritesOfferingSelection, null, 0f, 0f, FirstEntryWidth, FirstEntryHeight,
            new Rect(0f, 0f, 1f, 1f)).gameObject;
        ritesUI.pathSelectionOverlay = CreateFirstEntryRawImage("PathSelectionOverlay", ritesRoot,
            ritesPathSelection, null, 0f, 0f, FirstEntryWidth, FirstEntryHeight,
            new Rect(0f, 0f, 1f, 1f)).gameObject;
        ritesUI.novitiateSelectionOverlay = CreateFirstEntryRawImage("NovitiateSelectionOverlay", ritesRoot,
            ritesNovitiateSelection, null, 0f, 0f, FirstEntryWidth, FirstEntryHeight,
            new Rect(0f, 0f, 1f, 1f)).gameObject;
        ritesUI.respectSelectionOverlay = CreateFirstEntryRawImage("RespectSelectionOverlay", ritesRoot,
            ritesRespectSelection, null, 0f, 0f, FirstEntryWidth, FirstEntryHeight,
            new Rect(0f, 0f, 1f, 1f)).gameObject;
        ritesUI.welcomeNeutralOverlay.SetActive(false);
        ritesUI.offeringSelectionOverlay.SetActive(false);
        ritesUI.pathSelectionOverlay.SetActive(false);
        ritesUI.novitiateSelectionOverlay.SetActive(false);
        ritesUI.respectSelectionOverlay.SetActive(false);

        Color ritesIvory = FirstEntryHex("D8B486");
        Color ritesBronze = FirstEntryHex("C69A61");
        Color ritesGreen = FirstEntryHex("9CB65B");
        CreateMapText("RitesTitle", ritesRoot, "RITOS DEL SANTUARIO", font,
            180f, 30f, 720f, 75f, 47f, ritesIvory, TextAlignmentOptions.Center, true);
        CreateMapText("RitesFollowersLabel", ritesRoot, "SEGUIDORES", font,
            110f, 130f, 175f, 30f, 20f, ritesBronze, TextAlignmentOptions.Left, true);
        ritesUI.headerFollowersText = CreateMapText("RitesFollowersValue", ritesRoot,
            "3,845", font, 110f, 163f, 175f, 43f, 31f, ritesIvory, TextAlignmentOptions.Left, true);
        CreateMapText("RitesAcolytesLabel", ritesRoot, "ACÓLITOS", font,
            410f, 130f, 175f, 30f, 20f, ritesBronze, TextAlignmentOptions.Left, true);
        ritesUI.headerAcolytesText = CreateMapText("RitesAcolytesValue", ritesRoot,
            "1,280", font, 410f, 163f, 175f, 43f, 31f, ritesIvory, TextAlignmentOptions.Left, true);
        CreateMapText("RitesWaxLabel", ritesRoot, "CERA", font,
            650f, 130f, 125f, 30f, 20f, ritesBronze, TextAlignmentOptions.Left, true);
        ritesUI.headerWaxText = CreateMapText("RitesWaxValue", ritesRoot,
            "3,450", font, 650f, 163f, 145f, 43f, 31f, ritesIvory, TextAlignmentOptions.Left, true);
        CreateMapText("RitesBreadLabel", ritesRoot, "PAN RITUAL", font,
            884f, 130f, 160f, 30f, 19f, ritesBronze, TextAlignmentOptions.Left, true);
        ritesUI.headerBreadText = CreateMapText("RitesBreadValue", ritesRoot,
            "3,450", font, 884f, 163f, 160f, 43f, 31f, ritesIvory, TextAlignmentOptions.Left, true);
        ritesUI.slotsText = CreateMapText("RiteSlots", ritesRoot, "RITOS ACTIVOS 1 / 2", font,
            315f, 269f, 450f, 52f, 30f, ritesIvory, TextAlignmentOptions.Center, true);

        float[] riteCardX = { 33f, 252f, 448f, 648f, 846f };
        float[] riteCardW = { 210f, 185f, 192f, 187f, 193f };
        string[] riteCardTitles = { "RITO DE\nRECIBIMIENTO", "RITO DE\nOFRENDA", "RITO DEL\nCAMINO", "RITO DE\nNOVICIADO", "RITO DE\nRESPETO" };
        Button[] riteButtons = new Button[5];
        for (int riteIndex = 0; riteIndex < 5; riteIndex++)
        {
            riteButtons[riteIndex] = CreateMapInvisibleButton("Btn_RiteCard" + riteIndex,
                ritesRoot, riteCardX[riteIndex], 336f, riteCardW[riteIndex], 397f);
            CreateMapText("Label", riteButtons[riteIndex].transform, riteCardTitles[riteIndex], font,
                4f, 255f, riteCardW[riteIndex] - 8f, 125f, 23f,
                ritesIvory, TextAlignmentOptions.Center, true);
        }
        ritesUI.welcomeCardButton = riteButtons[0];
        ritesUI.offeringCardButton = riteButtons[1];
        ritesUI.pathCardButton = riteButtons[2];
        ritesUI.novitiateCardButton = riteButtons[3];
        ritesUI.respectCardButton = riteButtons[4];

        ritesUI.detailTitleText = CreateMapText("RiteDetailTitle", ritesRoot,
            "RITO DE RECIBIMIENTO", font, 170f, 812f, 740f, 64f, 38f,
            ritesIvory, TextAlignmentOptions.Center, true);
        ritesUI.descriptionText = CreateMapText("RiteDescription", ritesRoot,
            "AUMENTA LA LLEGADA DE SEGUIDORES", font, 170f, 905f, 740f, 48f, 25f,
            ritesBronze, TextAlignmentOptions.Center, true);
        CreateMapText("CurrentEffectLabel", ritesRoot, "EFECTO ACTUAL", font,
            200f, 986f, 260f, 45f, 25f, ritesBronze, TextAlignmentOptions.Center, true);
        ritesUI.currentEffectText = CreateMapText("CurrentEffectValue", ritesRoot,
            "+11%", font, 450f, 986f, 140f, 45f, 29f, ritesGreen,
            TextAlignmentOptions.Center, true);
        CreateMapText("LimitLabel", ritesRoot, "LÍMITE", font,
            620f, 986f, 150f, 45f, 25f, ritesBronze, TextAlignmentOptions.Center, true);
        ritesUI.limitText = CreateMapText("LimitValue", ritesRoot,
            "+50%", font, 770f, 986f, 135f, 45f, 29f, ritesGreen,
            TextAlignmentOptions.Center, true);
        CreateMapText("AssignmentsTitle", ritesRoot, "ASIGNADOS A ESTE RITO", font,
            310f, 1090f, 460f, 50f, 27f, ritesBronze, TextAlignmentOptions.Center, true);

        CreateMapText("AssignedFollowersLabel", ritesRoot, "SEGUIDORES", font,
            185f, 1163f, 260f, 60f, 27f, ritesBronze, TextAlignmentOptions.Left, true);
        ritesUI.followersAssignedText = CreateMapText("AssignedFollowersValue", ritesRoot,
            "4", font, 480f, 1160f, 125f, 65f, 35f, ritesIvory, TextAlignmentOptions.Center, true);
        CreateMapText("AssignedAcolytesLabel", ritesRoot, "ACÓLITOS", font,
            185f, 1278f, 260f, 60f, 27f, ritesBronze, TextAlignmentOptions.Left, true);
        ritesUI.acolytesAssignedText = CreateMapText("AssignedAcolytesValue", ritesRoot,
            "1", font, 480f, 1275f, 125f, 65f, 35f, ritesIvory, TextAlignmentOptions.Center, true);

        ritesUI.releaseFollowerOneButton = CreateMapInvisibleButton("Btn_RiteReleaseFollowerOne", ritesRoot, 665f, 1150f, 105f, 82f);
        ritesUI.assignFollowerOneButton = CreateMapInvisibleButton("Btn_RiteAssignFollowerOne", ritesRoot, 775f, 1150f, 105f, 82f);
        ritesUI.assignFollowerTenButton = CreateMapInvisibleButton("Btn_RiteAssignFollowerTen", ritesRoot, 885f, 1150f, 105f, 82f);
        ritesUI.releaseAcolyteOneButton = CreateMapInvisibleButton("Btn_RiteReleaseAcolyteOne", ritesRoot, 665f, 1264f, 105f, 82f);
        ritesUI.assignAcolyteOneButton = CreateMapInvisibleButton("Btn_RiteAssignAcolyteOne", ritesRoot, 775f, 1264f, 105f, 82f);
        ritesUI.assignAcolyteFiveButton = CreateMapInvisibleButton("Btn_RiteAssignAcolyteFive", ritesRoot, 885f, 1264f, 105f, 82f);
        Button[] riteActionButtons = { ritesUI.releaseFollowerOneButton, ritesUI.assignFollowerOneButton,
            ritesUI.assignFollowerTenButton, ritesUI.releaseAcolyteOneButton,
            ritesUI.assignAcolyteOneButton, ritesUI.assignAcolyteFiveButton };
        string[] riteActionLabels = { "−1", "+1", "+10", "−1", "+1", "+5" };
        for (int actionIndex = 0; actionIndex < riteActionButtons.Length; actionIndex++)
            CreateMapText("Label", riteActionButtons[actionIndex].transform, riteActionLabels[actionIndex],
                font, 0f, 0f, 105f, 78f, 30f, ritesIvory, TextAlignmentOptions.Center, true);

        ritesUI.releaseAllButton = CreateMapInvisibleButton("Btn_RiteReleaseAll", ritesRoot,
            255f, 1377f, 568f, 93f);
        CreateMapText("Label", ritesUI.releaseAllButton.transform, "LIBERAR TODO", font,
            8f, 7f, 552f, 78f, 34f, ritesIvory, TextAlignmentOptions.Center, true);
        ritesUI.unlockThirdSlotButton = CreateMapInvisibleButton("Btn_UnlockThirdRiteSlot", ritesRoot,
            83f, 1486f, 914f, 141f);
        ritesUI.unlockThirdSlotButtonText = CreateMapText("Label", ritesUI.unlockThirdSlotButton.transform,
            "DESBLOQUEAR TERCER ESPACIO\n250 CONFIANZA · NOVICIADO 3 · 5 ACÓLITOS · 150 CERA · 150 PAN RITUAL",
            font, 100f, 18f, 790f, 105f, 21f, ritesBronze, TextAlignmentOptions.Center, true);

        ritesUI.backButton = CreateMapInvisibleButton("Btn_RitesBack", ritesRoot, 33f, 26f, 99f, 75f);
        float[] ritesTabX = { 23f, 171f, 319f, 479f, 627f, 775f, 923f };
        float[] ritesTabW = { 148f, 148f, 160f, 148f, 148f, 148f, 134f };
        Button[] ritesTabs = new Button[7];
        string[] ritesTabNames = { "REFUGIO", "ALTARES", "PEREGRINACIONES", "NOVICIADO", "RITOS", "PACTOS", "UMBRAL" };
        for (int tabIndex = 0; tabIndex < 7; tabIndex++)
        {
            ritesTabs[tabIndex] = CreateMapInvisibleButton("Btn_RitesNav" + tabIndex, ritesRoot,
                ritesTabX[tabIndex], 1701f, ritesTabW[tabIndex], 193f);
            CreateMapText("Label", ritesTabs[tabIndex].transform, ritesTabNames[tabIndex], font,
                0f, 135f, ritesTabW[tabIndex], 42f, tabIndex == 2 ? 13f : 18f,
                tabIndex == 4 ? ritesIvory : ritesBronze, TextAlignmentOptions.Center, true);
        }
        ritesUI.refugeNavButton = ritesTabs[0];
        ritesUI.altarsNavButton = ritesTabs[1];
        ritesUI.pilgrimagesNavButton = ritesTabs[2];
        ritesUI.novitiateNavButton = ritesTabs[3];
        ritesTabs[4].interactable = false;
        ritesUI.pactsNavButton = ritesTabs[5];
        ritesUI.thresholdNavButton = ritesTabs[6];

        ritesUI.riteDropdown = CreateDropdown("RiteSelectorLegacy", ritesRoot,
            new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        ritesUI.riteDropdown.gameObject.SetActive(false);
        ritesUI.effectText = ritesUI.detailTitleText;
        ritesUI.resourcesText = ritesUI.headerFollowersText;
        ritesUI.assignmentText = ritesUI.followersAssignedText;
        civilization1UI.ritesSectionRoot.SetActive(false);

        civilization1UI.pactsSectionRoot = CreateView(
            "D2_Civ1_PactsSection", root.transform
        );
        D2CivilizationPactsPanelUI pactsUI = Undo.AddComponent<D2CivilizationPactsPanelUI>(
            civilization1UI.pactsSectionRoot
        );
        civilization1UI.pactsPanelUI = pactsUI;
        Transform pactsRoot = civilization1UI.pactsSectionRoot.transform;
        RawImage pactsBase = CreateFirstEntryRawImage("PactsBasePlate", pactsRoot,
            pactsBasePlate, null, 0f, 0f, FirstEntryWidth, FirstEntryHeight,
            new Rect(0f, 0f, 1f, 1f));
        pactsBase.raycastTarget = true;
        pactsUI.hospitalityNeutralOverlay = CreateFirstEntryRawImage(
            "HospitalityNeutralOverlay", pactsRoot, pactsHospitalityNeutral, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight, new Rect(0f, 0f, 1f, 1f)).gameObject;
        pactsUI.openPathSelectionOverlay = CreateFirstEntryRawImage(
            "OpenPathSelectionOverlay", pactsRoot, pactsOpenPathSelection, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight, new Rect(0f, 0f, 1f, 1f)).gameObject;
        pactsUI.consecrationSelectionOverlay = CreateFirstEntryRawImage(
            "ConsecrationSelectionOverlay", pactsRoot, pactsConsecrationSelection, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight, new Rect(0f, 0f, 1f, 1f)).gameObject;
        pactsUI.silentVowSelectionOverlay = CreateFirstEntryRawImage(
            "SilentVowSelectionOverlay", pactsRoot, pactsSilentVowSelection, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight, new Rect(0f, 0f, 1f, 1f)).gameObject;
        pactsUI.innerDoorSelectionOverlay = CreateFirstEntryRawImage(
            "InnerDoorSelectionOverlay", pactsRoot, pactsInnerDoorSelection, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight, new Rect(0f, 0f, 1f, 1f)).gameObject;
        pactsUI.hospitalitySlotNeutralOverlay = CreateFirstEntryRawImage(
            "HospitalitySlotNeutralOverlay", pactsRoot, pactsHospitalitySlotNeutral, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight, new Rect(0f, 0f, 1f, 1f)).gameObject;
        CreateFirstEntryRawImage("StaticDetailIconsOverlay", pactsRoot,
            pactsStaticDetailIcons, null, 0f, 0f, FirstEntryWidth, FirstEntryHeight,
            new Rect(0f, 0f, 1f, 1f));
        CreateFirstEntryRawImage("ActionButtonsOverlay", pactsRoot,
            pactsActionButtons, null, 0f, 0f, FirstEntryWidth, FirstEntryHeight,
            new Rect(0f, 0f, 1f, 1f));
        pactsUI.lockRequirementIconsOverlay = CreateFirstEntryRawImage(
            "LockRequirementIconsOverlay", pactsRoot, pactsLockRequirementIcons, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight,
            new Rect(0f, 0f, 1f, 1f)).gameObject;
        pactsUI.secondSlotUnlockedOverlay = CreateFirstEntryRawImage(
            "SecondSlotUnlockedOverlay", pactsRoot, pactsSecondSlotUnlocked, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight, new Rect(0f, 0f, 1f, 1f)).gameObject;
        pactsUI.hospitalityNeutralOverlay.SetActive(false);
        pactsUI.openPathSelectionOverlay.SetActive(false);
        pactsUI.consecrationSelectionOverlay.SetActive(false);
        pactsUI.silentVowSelectionOverlay.SetActive(false);
        pactsUI.innerDoorSelectionOverlay.SetActive(false);
        pactsUI.hospitalitySlotNeutralOverlay.SetActive(false);
        pactsUI.secondSlotUnlockedOverlay.SetActive(false);

        Color pactsIvory = FirstEntryHex("D8B486");
        Color pactsBronze = FirstEntryHex("C69A61");
        Color pactsGreen = FirstEntryHex("9CB65B");
        Color pactsOrange = FirstEntryHex("C87535");
        CreateMapText("PactsTitle", pactsRoot, "PACTOS DE CIVILIZACIÓN", font,
            195f, 31f, 690f, 74f, 44f, pactsIvory, TextAlignmentOptions.Center, true);
        CreateMapText("PactsTrustLabel", pactsRoot, "CONFIANZA", font,
            148f, 124f, 155f, 27f, 19f, pactsBronze, TextAlignmentOptions.Left, true);
        pactsUI.headerTrustText = CreateMapText("PactsTrustValue", pactsRoot, "280 / 500", font,
            148f, 153f, 175f, 40f, 30f, pactsIvory, TextAlignmentOptions.Left, true);
        CreateMapText("PactsAcolytesLabel", pactsRoot, "ACÓLITOS", font,
            401f, 124f, 145f, 27f, 19f, pactsBronze, TextAlignmentOptions.Left, true);
        pactsUI.headerAcolytesText = CreateMapText("PactsAcolytesValue", pactsRoot, "1,280", font,
            401f, 153f, 145f, 40f, 30f, pactsIvory, TextAlignmentOptions.Left, true);
        CreateMapText("PactsWaxLabel", pactsRoot, "CERA", font,
            621f, 124f, 115f, 27f, 19f, pactsBronze, TextAlignmentOptions.Left, true);
        pactsUI.headerWaxText = CreateMapText("PactsWaxValue", pactsRoot, "3,450", font,
            672f, 153f, 120f, 40f, 30f, pactsIvory, TextAlignmentOptions.Left, true);
        CreateMapText("PactsBreadLabel", pactsRoot, "PAN RITUAL", font,
            835f, 124f, 180f, 27f, 18f, pactsBronze, TextAlignmentOptions.Left, true);
        pactsUI.headerBreadText = CreateMapText("PactsBreadValue", pactsRoot, "3,450", font,
            884f, 153f, 130f, 40f, 30f, pactsIvory, TextAlignmentOptions.Left, true);

        string[] pactCardTitles = { "HOSPEDAJE", "CAMINO ABIERTO", "CONSAGRACIÓN", "VOTO SILENCIOSO", "PUERTA INTERIOR" };
        float[] pactTitleX = { 70f, 565f, 70f, 565f, 450f };
        float[] pactTitleY = { 418f, 418f, 710f, 710f, 870f };
        float[] pactTitleW = { 415f, 450f, 415f, 450f, 390f };
        for (int pactTitleIndex = 0; pactTitleIndex < pactCardTitles.Length; pactTitleIndex++)
            CreateMapText("PactCardTitle" + pactTitleIndex, pactsRoot, pactCardTitles[pactTitleIndex], font,
                pactTitleX[pactTitleIndex], pactTitleY[pactTitleIndex], pactTitleW[pactTitleIndex], 65f,
                pactTitleIndex == 4 ? 31f : 34f, pactsIvory, TextAlignmentOptions.Center, true);
        pactsUI.hospitalityCardButton = CreateMapInvisibleButton("Btn_PactHospitality", pactsRoot, 42f, 229f, 474f, 263f);
        pactsUI.openPathCardButton = CreateMapInvisibleButton("Btn_PactOpenPath", pactsRoot, 543f, 229f, 495f, 264f);
        pactsUI.consecrationCardButton = CreateMapInvisibleButton("Btn_PactConsecration", pactsRoot, 42f, 514f, 475f, 261f);
        pactsUI.silentVowCardButton = CreateMapInvisibleButton("Btn_PactSilentVow", pactsRoot, 543f, 514f, 495f, 261f);
        pactsUI.innerDoorCardButton = CreateMapInvisibleButton("Btn_PactInnerDoor", pactsRoot, 180f, 792f, 718f, 196f);

        pactsUI.slotsText = CreateMapText("PactSlots", pactsRoot, "PACTOS ACTIVOS 1/2", font,
            315f, 1000f, 450f, 54f, 30f, pactsIvory, TextAlignmentOptions.Center, true);
        pactsUI.firstActivePactIcon = CreateFirstEntryRawImage("FirstActivePactIcon", pactsRoot,
            null, null, 206f, 1072f, 185f, 150f, new Rect(0f, 0f, 1f, 1f));
        pactsUI.secondActivePactIcon = CreateFirstEntryRawImage("SecondActivePactIcon", pactsRoot,
            null, null, 674f, 1072f, 185f, 150f, new Rect(0f, 0f, 1f, 1f));
        pactsUI.firstActivePactIcon.gameObject.SetActive(false);
        pactsUI.secondActivePactIcon.gameObject.SetActive(false);
        pactsUI.pactMedallionTextures = pactsMedallions;

        pactsUI.detailTitleText = CreateMapText("PactDetailTitle", pactsRoot,
            "PACTO DE HOSPEDAJE – ACTIVO", font, 75f, 1271f, 625f, 70f, 31f,
            pactsIvory, TextAlignmentOptions.Center, true);
        CreateMapText("PactBenefitLabel", pactsRoot, "BENEFICIO", font,
            145f, 1362f, 190f, 46f, 22f, pactsBronze, TextAlignmentOptions.Left, true);
        pactsUI.benefitValueText = CreateMapText("PactBenefitValue", pactsRoot,
            "+35% LLEGADA DE SEGUIDORES", font, 340f, 1362f, 355f, 46f, 20f,
            pactsGreen, TextAlignmentOptions.Left, true);
        CreateMapText("PactCommitmentLabel", pactsRoot, "COMPROMISO", font,
            145f, 1422f, 205f, 46f, 22f, pactsBronze, TextAlignmentOptions.Left, true);
        pactsUI.commitmentValueText = CreateMapText("PactCommitmentValue", pactsRoot,
            "CONSUME 1 PAN RITUAL POR MINUTO", font, 340f, 1416f, 368f, 58f, 16f,
            pactsOrange, TextAlignmentOptions.Left, true);
        CreateMapText("PactStateLabel", pactsRoot, "ESTADO", font,
            145f, 1483f, 180f, 46f, 22f, pactsBronze, TextAlignmentOptions.Left, true);
        pactsUI.stateValueText = CreateMapText("PactStateValue", pactsRoot, "ACTIVO", font,
            340f, 1483f, 240f, 46f, 22f, pactsGreen, TextAlignmentOptions.Left, true);

        pactsUI.activateButton = CreateMapInvisibleButton("Btn_ActivateCivilizationPact", pactsRoot,
            72f, 1555f, 297f, 92f);
        pactsUI.activateButtonText = CreateMapText("Label", pactsUI.activateButton.transform,
            "PACTO ACTIVO", font, 0f, 8f, 297f, 74f, 24f, pactsBronze,
            TextAlignmentOptions.Center, true);
        pactsUI.cancelButton = CreateMapInvisibleButton("Btn_CancelCivilizationPact", pactsRoot,
            395f, 1555f, 320f, 92f);
        CreateMapText("Label", pactsUI.cancelButton.transform, "CANCELAR PACTO", font,
            0f, 8f, 320f, 74f, 24f, pactsIvory, TextAlignmentOptions.Center, true);
        pactsUI.warningText = CreateMapText("PactCancellationWarning", pactsRoot,
            "CANCELAR NO DEVUELVE EL COSTE DE ACTIVACIÓN.", font,
            89f, 1653f, 620f, 39f, 16f, pactsBronze, TextAlignmentOptions.Center, true);
        pactsUI.unlockSecondSlotButton = CreateMapInvisibleButton(
            "Btn_UnlockSecondCivilizationPactSlot", pactsRoot, 760f, 1249f, 282f, 421f);
        pactsUI.unlockSecondSlotButtonText = CreateMapText("Label",
            pactsUI.unlockSecondSlotButton.transform,
            "SEGUNDO ESPACIO", font,
            16f, 135f, 250f, 55f, 19f, pactsBronze, TextAlignmentOptions.Center, true);
        pactsUI.secondSlotRequirementsText = CreateMapText("Requirements",
            pactsUI.unlockSecondSlotButton.transform,
            "400 CONFIANZA\nNOVICIADO 4\n10 ACÓLITOS\n300 CERA\n300 PAN RITUAL", font,
            82f, 183f, 175f, 210f, 18f, pactsBronze, TextAlignmentOptions.Left, true);

        pactsUI.backButton = CreateMapInvisibleButton("Btn_PactsBack", pactsRoot, 24f, 25f, 91f, 78f);
        float[] pactsTabX = { 32f, 176f, 317f, 459f, 604f, 751f, 900f };
        float[] pactsTabW = { 143f, 141f, 142f, 145f, 147f, 149f, 149f };
        Button[] pactsTabs = new Button[7];
        string[] pactsTabNames = { "REFUGIO", "ALTARES", "PEREGRINACIONES", "NOVICIADO", "RITOS", "PACTOS", "UMBRAL" };
        for (int tabIndex = 0; tabIndex < 7; tabIndex++)
        {
            pactsTabs[tabIndex] = CreateMapInvisibleButton("Btn_PactsNav" + tabIndex, pactsRoot,
                pactsTabX[tabIndex], 1718f, pactsTabW[tabIndex], 177f);
            CreateMapText("Label", pactsTabs[tabIndex].transform, pactsTabNames[tabIndex], font,
                0f, 124f, pactsTabW[tabIndex], 44f, tabIndex == 2 ? 13f : 18f,
                tabIndex == 5 ? pactsIvory : pactsBronze, TextAlignmentOptions.Center, true);
        }
        pactsUI.refugeNavButton = pactsTabs[0];
        pactsUI.altarsNavButton = pactsTabs[1];
        pactsUI.pilgrimagesNavButton = pactsTabs[2];
        pactsUI.novitiateNavButton = pactsTabs[3];
        pactsUI.ritesNavButton = pactsTabs[4];
        pactsTabs[5].interactable = false;
        pactsUI.thresholdNavButton = pactsTabs[6];

        pactsUI.pactDropdown = CreateDropdown("CivilizationPactSelectorLegacy", pactsRoot,
            new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        pactsUI.pactDropdown.gameObject.SetActive(false);
        pactsUI.pactStateText = pactsUI.detailTitleText;
        pactsUI.benefitText = pactsUI.benefitValueText;
        pactsUI.commitmentText = pactsUI.commitmentValueText;
        pactsUI.resourcesText = pactsUI.headerTrustText;
        pactsUI.lastResultText = pactsUI.warningText;
        civilization1UI.pactsSectionRoot.SetActive(false);

        civilization1UI.veiledThresholdSectionRoot = CreateView(
            "D2_Civ1_VeiledThresholdSection", root.transform
        );
        D2VeiledThresholdPanelUI thresholdUI = Undo.AddComponent<D2VeiledThresholdPanelUI>(
            civilization1UI.veiledThresholdSectionRoot
        );
        civilization1UI.veiledThresholdPanelUI = thresholdUI;
        Transform thresholdRoot = civilization1UI.veiledThresholdSectionRoot.transform;
        RawImage bondBase = CreateFirstEntryRawImage("BondBasePlate", thresholdRoot,
            bondBasePlate, null, 0f, 0f, FirstEntryWidth, FirstEntryHeight,
            new Rect(0f, 0f, 1f, 1f));
        bondBase.raycastTarget = true;

        Color bondIvory = FirstEntryHex("D8B486");
        Color bondBronze = FirstEntryHex("C69A61");
        Color bondGreen = FirstEntryHex("A8B85D");
        thresholdUI.titleText = CreateMapText("BondTitle", thresholdRoot,
            "PACTO — LUGAR DE VÍNCULO", font, 150f, 32f, 810f, 82f, 46f,
            bondIvory, TextAlignmentOptions.Center, true);
        ConfigureBondText(thresholdUI.titleText, 46f, 34f,
            new Vector4(8f, 2f, 8f, 2f), 0f, 0.035f);

        TMP_Text bondIncenseLabel = CreateMapText("BondIncenseLabel", thresholdRoot,
            "INCIENSO", font, 137f, 147f, 180f, 34f, 19f,
            bondBronze, TextAlignmentOptions.Left, true);
        thresholdUI.incenseValueText = CreateMapText("BondIncenseValue", thresholdRoot,
            "300", font, 137f, 177f, 150f, 46f, 31f, bondIvory,
            TextAlignmentOptions.Left, true);
        TMP_Text bondClothLabel = CreateMapText("BondClothLabel", thresholdRoot,
            "TELA SAGRADA", font, 380f, 147f, 215f, 34f, 19f,
            bondBronze, TextAlignmentOptions.Left, true);
        thresholdUI.sacredClothValueText = CreateMapText("BondClothValue", thresholdRoot,
            "300", font, 382f, 177f, 150f, 46f, 31f, bondIvory,
            TextAlignmentOptions.Left, true);
        TMP_Text bondStoneLabel = CreateMapText("BondStoneLabel", thresholdRoot,
            "PIEDRA TALLADA", font, 630f, 147f, 235f, 34f, 19f,
            bondBronze, TextAlignmentOptions.Left, true);
        thresholdUI.carvedStoneValueText = CreateMapText("BondStoneValue", thresholdRoot,
            "300", font, 630f, 177f, 150f, 46f, 31f, bondIvory,
            TextAlignmentOptions.Left, true);
        TMP_Text bondProgressLabel = CreateMapText("BondProgressLabel", thresholdRoot,
            "PROGRESO", font, 917f, 147f, 145f, 34f, 19f,
            bondBronze, TextAlignmentOptions.Left, true);
        thresholdUI.progressValueText = CreateMapText("BondProgressValue", thresholdRoot,
            "45", font, 917f, 177f, 100f, 46f, 31f, bondIvory,
            TextAlignmentOptions.Left, true);
        ConfigureBondText(bondIncenseLabel, 19f, 19f, new Vector4(2f, 0f, 2f, 0f));
        ConfigureBondText(bondClothLabel, 19f, 19f, new Vector4(2f, 0f, 2f, 0f));
        ConfigureBondText(bondStoneLabel, 19f, 19f, new Vector4(2f, 0f, 2f, 0f));
        ConfigureBondText(bondProgressLabel, 19f, 19f, new Vector4(2f, 0f, 2f, 0f));
        ConfigureBondText(thresholdUI.incenseValueText, 31f, 31f,
            new Vector4(2f, 0f, 2f, 0f), 0f, 0.035f);
        ConfigureBondText(thresholdUI.sacredClothValueText, 31f, 31f,
            new Vector4(2f, 0f, 2f, 0f), 0f, 0.035f);
        ConfigureBondText(thresholdUI.carvedStoneValueText, 31f, 31f,
            new Vector4(2f, 0f, 2f, 0f), 0f, 0.035f);
        ConfigureBondText(thresholdUI.progressValueText, 31f, 31f,
            new Vector4(2f, 0f, 2f, 0f), 0f, 0.035f);

        string[] bondLineNames =
        {
            "CAMINO\nPEREGRINO", "OFICIO\nSAGRADO", "ORDEN\nDE ACÓLITOS",
            "ECO DEL\nSANTUARIO", "LITURGIA\nDE TRAZAS"
        };
        float[] bondCardCenterX = { 540f, 208.5f, 870.5f, 287f, 783f };
        float[] bondNameY = { 384f, 579f, 582f, 908f, 908f };
        float[] bondNameW = { 210f, 210f, 215f, 215f, 220f };
        float[] bondNameH = { 55f, 55f, 55f, 55f, 55f };
        float[] bondNameFont = { 19f, 19f, 17f, 17f, 17f };
        float[] bondLevelY = { 445f, 646f, 650f, 976f, 976f };
        float[] bondLevelW = { 180f, 179f, 189f, 179f, 180f };
        float[] bondButtonX = { 414f, 76f, 748f, 136f, 680f };
        float[] bondButtonY = { 224f, 430f, 430f, 742f, 742f };
        float[] bondButtonW = { 252f, 270f, 270f, 276f, 280f };
        float[] bondButtonH = { 265f, 285f, 285f, 285f, 285f };
        thresholdUI.lineNameTexts = new TMP_Text[D2BondSystem.LineIds.Length];
        thresholdUI.lineLevelTexts = new TMP_Text[D2BondSystem.LineIds.Length];
        thresholdUI.lineButtons = new Button[D2BondSystem.LineIds.Length];
        for (int lineIndex = 0; lineIndex < D2BondSystem.LineIds.Length; lineIndex++)
        {
            thresholdUI.lineNameTexts[lineIndex] = CreateMapText(
                "BondLineName" + lineIndex, thresholdRoot, bondLineNames[lineIndex], font,
                bondCardCenterX[lineIndex] - (bondNameW[lineIndex] * 0.5f),
                bondNameY[lineIndex], bondNameW[lineIndex],
                bondNameH[lineIndex],
                bondNameFont[lineIndex], bondBronze, TextAlignmentOptions.Center, true);
            ConfigureBondText(thresholdUI.lineNameTexts[lineIndex],
                bondNameFont[lineIndex], bondNameFont[lineIndex],
                new Vector4(12f, 2f, 12f, 2f), -5f, 0.035f);
            thresholdUI.lineLevelTexts[lineIndex] = CreateMapText(
                "BondLineLevel" + lineIndex, thresholdRoot, "NIVEL 1 / 3", font,
                bondCardCenterX[lineIndex] - (bondLevelW[lineIndex] * 0.5f),
                bondLevelY[lineIndex], bondLevelW[lineIndex],
                26f, 15f, bondBronze, TextAlignmentOptions.Center, true);
            ConfigureBondText(thresholdUI.lineLevelTexts[lineIndex], 15f, 15f,
                new Vector4(8f, 2f, 8f, 2f));
            thresholdUI.lineButtons[lineIndex] = CreateMapInvisibleButton(
                "Btn_BondLine" + lineIndex, thresholdRoot, bondButtonX[lineIndex],
                bondButtonY[lineIndex], bondButtonW[lineIndex], bondButtonH[lineIndex]);
        }
        thresholdUI.centerStateText = CreateMapText("BondCenterState", thresholdRoot,
            "PACTO\nESTABLECIDO", font, 415f, 690f, 250f, 82f, 24f, bondBronze,
            TextAlignmentOptions.Center, true);
        ConfigureBondText(thresholdUI.centerStateText, 24f, 24f,
            new Vector4(12f, 3f, 12f, 3f), -6f, 0.035f);

        thresholdUI.detailTitleText = CreateMapText("BondDetailTitle", thresholdRoot,
            "CAMINO PEREGRINO", font, 64f, 1065f, 350f, 62f, 30f, bondIvory,
            TextAlignmentOptions.Left, true);
        thresholdUI.detailLevelText = CreateMapText("BondDetailLevel", thresholdRoot,
            "NIVEL 1 / 3", font, 400f, 1052f, 175f, 48f, 22f, bondBronze,
            TextAlignmentOptions.Right, true);
        thresholdUI.effectText = CreateMapText("BondEffect", thresholdRoot,
            "+5% LLEGADA Y RECOMPENSAS\nMATERIALES POR NIVEL", font,
            165f, 1130f, 400f, 105f, 23f, bondBronze, TextAlignmentOptions.Left, true);
        TMP_Text bondNextLabel = CreateMapText("BondNextLabel", thresholdRoot,
            "SIGUIENTE", font, 610f, 1039f, 240f, 44f, 22f,
            bondBronze, TextAlignmentOptions.Left, true);
        thresholdUI.progressCostText = CreateMapText("BondProgressCost", thresholdRoot,
            "40 PROGRESO", font, 665f, 1081f, 335f, 42f, 21f, bondBronze,
            TextAlignmentOptions.Left, true);
        thresholdUI.incenseCostText = CreateMapText("BondIncenseCost", thresholdRoot,
            "50 INCIENSO", font, 665f, 1126f, 335f, 42f, 21f, bondBronze,
            TextAlignmentOptions.Left, true);
        thresholdUI.sacredClothCostText = CreateMapText("BondClothCost", thresholdRoot,
            "50 TELA SAGRADA", font, 665f, 1168f, 335f, 42f, 21f, bondBronze,
            TextAlignmentOptions.Left, true);
        thresholdUI.carvedStoneCostText = CreateMapText("BondStoneCost", thresholdRoot,
            "50 PIEDRA TALLADA", font, 665f, 1210f, 335f, 42f, 21f, bondBronze,
            TextAlignmentOptions.Left, true);
        ConfigureBondText(thresholdUI.detailTitleText, 30f, 22f,
            new Vector4(4f, 1f, 4f, 1f), 0f, 0.035f);
        ConfigureBondText(thresholdUI.detailLevelText, 22f, 22f,
            new Vector4(4f, 1f, 4f, 1f));
        ConfigureBondText(thresholdUI.effectText, 23f, 23f,
            new Vector4(4f, 2f, 4f, 2f), -3f);
        ConfigureBondText(bondNextLabel, 22f, 22f,
            new Vector4(2f, 1f, 2f, 1f));
        ConfigureBondText(thresholdUI.progressCostText, 21f, 21f,
            new Vector4(2f, 1f, 2f, 1f));
        ConfigureBondText(thresholdUI.incenseCostText, 21f, 21f,
            new Vector4(2f, 1f, 2f, 1f));
        ConfigureBondText(thresholdUI.sacredClothCostText, 21f, 21f,
            new Vector4(2f, 1f, 2f, 1f));
        ConfigureBondText(thresholdUI.carvedStoneCostText, 21f, 21f,
            new Vector4(2f, 1f, 2f, 1f));

        TMP_Text availableAcolytesLabel = CreateMapText("BondAcolytesAvailableLabel",
            thresholdRoot, "ACÓLITOS DISPONIBLES", font, 93f, 1307f, 310f, 44f, 21f,
            bondBronze, TextAlignmentOptions.Center, true);
        thresholdUI.availableAcolytesValueText = CreateMapText(
            "BondAcolytesAvailableValue", thresholdRoot, "1,280", font,
            179f, 1356f, 205f, 58f, 39f, bondIvory, TextAlignmentOptions.Center, true);
        TMP_Text assignedAcolytesLabel = CreateMapText("BondAcolytesAssignedLabel",
            thresholdRoot, "ACÓLITOS ASIGNADOS", font, 634f, 1308f, 310f, 44f, 21f,
            bondBronze, TextAlignmentOptions.Center, true);
        thresholdUI.assignedAcolytesValueText = CreateMapText(
            "BondAcolytesAssignedValue", thresholdRoot, "4", font,
            738f, 1351f, 120f, 58f, 39f, bondIvory, TextAlignmentOptions.Center, true);
        ConfigureBondText(availableAcolytesLabel, 21f, 21f,
            new Vector4(3f, 1f, 3f, 1f));
        ConfigureBondText(assignedAcolytesLabel, 21f, 21f,
            new Vector4(3f, 1f, 3f, 1f));
        ConfigureBondText(thresholdUI.availableAcolytesValueText, 39f, 39f,
            new Vector4(4f, 1f, 4f, 1f), 0f, 0.035f);
        ConfigureBondText(thresholdUI.assignedAcolytesValueText, 39f, 39f,
            new Vector4(4f, 1f, 4f, 1f), 0f, 0.035f);
        thresholdUI.releaseAcolyteButton = CreateMapInvisibleButton(
            "Btn_BondReleaseAcolyte", thresholdRoot, 442f, 1315f, 82f, 86f);
        thresholdUI.assignAcolyteButton = CreateMapInvisibleButton(
            "Btn_BondAssignAcolyte", thresholdRoot, 532f, 1315f, 82f, 86f);

        thresholdUI.prepareButton = CreateMapInvisibleButton(
            "Btn_PrepareBondPlace", thresholdRoot, 224f, 1425f, 632f, 92f);
        thresholdUI.upgradeLineButton = CreateMapInvisibleButton(
            "Btn_UpgradeBondLine", thresholdRoot, 224f, 1425f, 632f, 92f);
        thresholdUI.actionButtonText = CreateMapText("BondActionLabel", thresholdRoot,
            "MEJORAR LÍNEA", font, 242f, 1445f, 596f, 58f, 34f, bondIvory,
            TextAlignmentOptions.Center, true);
        ConfigureBondText(thresholdUI.actionButtonText, 34f, 25f,
            new Vector4(16f, 3f, 16f, 3f), 0f, 0.035f);

        thresholdUI.revelationText = CreateMapText("BondEstablishedTitle", thresholdRoot,
            "PACTO ESTABLECIDO", font, 300f, 1553f, 580f, 64f, 36f, bondGreen,
            TextAlignmentOptions.Center, true);
        thresholdUI.placeText = CreateMapText("BondEstablishedSubtitle", thresholdRoot,
            "PREPARACIÓN COMPLETADA", font, 285f, 1627f, 610f, 56f, 28f,
            bondBronze, TextAlignmentOptions.Center, true);
        ConfigureBondText(thresholdUI.revelationText, 36f, 36f,
            new Vector4(8f, 2f, 8f, 2f), 0f, 0.035f);
        ConfigureBondText(thresholdUI.placeText, 28f, 28f,
            new Vector4(8f, 2f, 8f, 2f), 0f, 0.035f);

        thresholdUI.backButton = CreateMapInvisibleButton(
            "Btn_BondBack", thresholdRoot, 23f, 22f, 92f, 82f);
        float[] bondTabButtonX = { 17f, 165f, 317f, 467f, 619f, 768f, 916f };
        float[] bondTabCenterX = { 100f, 248f, 395f, 537f, 682f, 829f, 975f };
        string[] bondTabNames =
        {
            "REFUGIO", "ALTARES", "PEREGRI-\nNACIONES", "NOVICIADO",
            "RITOS", "PACTOS", "PACTO"
        };
        Button[] bondTabs = new Button[bondTabButtonX.Length];
        for (int tabIndex = 0; tabIndex < bondTabs.Length; tabIndex++)
        {
            bondTabs[tabIndex] = CreateMapInvisibleButton(
                "Btn_BondNav" + tabIndex, thresholdRoot, bondTabButtonX[tabIndex],
                1722f, 147f, 181f);
            TMP_Text tabLabel = CreateMapText("BondNavLabel" + tabIndex, thresholdRoot,
                bondTabNames[tabIndex], font, bondTabCenterX[tabIndex] - 73.5f,
                1827f, 147f, 58f, 19f, bondBronze,
                TextAlignmentOptions.Center, true);
            ConfigureBondText(tabLabel, 19f, 19f,
                new Vector4(3f, 1f, 3f, 1f), tabIndex == 2 ? -5f : 0f);
            if (tabIndex == 6) thresholdUI.thresholdTabLabelText = tabLabel;
        }
        thresholdUI.refugeNavButton = bondTabs[0];
        thresholdUI.altarsNavButton = bondTabs[1];
        thresholdUI.pilgrimagesNavButton = bondTabs[2];
        thresholdUI.novitiateNavButton = bondTabs[3];
        thresholdUI.ritesNavButton = bondTabs[4];
        thresholdUI.pactsNavButton = bondTabs[5];

        thresholdUI.lineDropdown = CreateDropdown(
            "BondLineSelectorData", thresholdRoot, new Vector2(0.5f, 0.5f),
            new Vector2(0.001f, 0.001f));
        CanvasGroup hiddenDropdown = Undo.AddComponent<CanvasGroup>(
            thresholdUI.lineDropdown.gameObject);
        hiddenDropdown.alpha = 0f;
        hiddenDropdown.interactable = false;
        hiddenDropdown.blocksRaycasts = false;

        thresholdUI.resourcesText = thresholdUI.incenseValueText;
        thresholdUI.acolytesText = thresholdUI.availableAcolytesValueText;
        thresholdUI.lineText = thresholdUI.effectText;
        thresholdUI.pendingText = thresholdUI.placeText;
        thresholdUI.prepareButton.gameObject.SetActive(false);
        civilization1UI.veiledThresholdSectionRoot.SetActive(false);

        EditorUtility.SetDirty(civilization1UI);
    }

    private static void BuildCivilization2(Dimension2PanelUI panel)
    {
        PrepareFirstEntryTexture(ResistanceRegionsCleanBasePlatePath, 2048);
        PrepareFirstEntryTexture(ResistanceOperationsCleanBasePlatePath, 2048);
        PrepareFirstEntryTexture(ResistanceDefenseCleanBasePlatePath, 2048);
        PrepareFirstEntryTexture(ResistanceAlertCleanBasePlatePath, 2048);
        PrepareFirstEntryTexture(ResistanceAlertIconSheetPath, 2048);
        PrepareFirstEntryTexture(ResistanceContainmentCleanBasePlatePath, 2048);
        PrepareFirstEntryTexture(ResistanceMajorPactCleanBasePlatePath, 2048);
        PrepareFirstEntryTexture(ResistanceMajorPactLineSymbolsPath, 2048);
        PrepareFirstEntryTexture(ResistancePactsCleanBasePlatePath, 2048);
        PrepareFirstEntryTexture(ResistancePactsFidelityPlatePath, 2048);
        PrepareFirstEntryTexture(ResistancePactsStaticIconOverlayPath, 2048);
        PrepareFirstEntryTexture(ResistancePactsIconSheetPath, 2048);
        PrepareFirstEntryTexture(ResistancePactsInnerSymbolsPath, 2048);
        Texture2D regionsBasePlate = AssetDatabase.LoadAssetAtPath<Texture2D>(
            ResistanceRegionsCleanBasePlatePath);
        Texture2D operationsBasePlate = AssetDatabase.LoadAssetAtPath<Texture2D>(
            ResistanceOperationsCleanBasePlatePath);
        Texture2D defenseBasePlate = AssetDatabase.LoadAssetAtPath<Texture2D>(
            ResistanceDefenseCleanBasePlatePath);
        Texture2D alertBasePlate = AssetDatabase.LoadAssetAtPath<Texture2D>(
            ResistanceAlertCleanBasePlatePath);
        Texture2D alertIconSheet = AssetDatabase.LoadAssetAtPath<Texture2D>(
            ResistanceAlertIconSheetPath);
        Texture2D containmentBasePlate = AssetDatabase.LoadAssetAtPath<Texture2D>(
            ResistanceContainmentCleanBasePlatePath);
        Texture2D majorPactBasePlate = AssetDatabase.LoadAssetAtPath<Texture2D>(
            ResistanceMajorPactCleanBasePlatePath);
        Texture2D majorPactLineSymbols = AssetDatabase.LoadAssetAtPath<Texture2D>(
            ResistanceMajorPactLineSymbolsPath);
        Texture2D resistancePactsBasePlate = AssetDatabase.LoadAssetAtPath<Texture2D>(
            ResistancePactsCleanBasePlatePath);
        Texture2D resistancePactsFidelityPlate = AssetDatabase.LoadAssetAtPath<Texture2D>(
            ResistancePactsFidelityPlatePath);
        Texture2D resistancePactsStaticIcons = AssetDatabase.LoadAssetAtPath<Texture2D>(
            ResistancePactsStaticIconOverlayPath);
        Texture2D resistancePactsIconSheet = AssetDatabase.LoadAssetAtPath<Texture2D>(
            ResistancePactsIconSheetPath);
        Texture2D resistancePactsInnerSymbols = AssetDatabase.LoadAssetAtPath<Texture2D>(
            ResistancePactsInnerSymbolsPath);
        Material resistanceChromaKey = GetOrCreateResistanceChromaKeyMaterial();
        Sprite resistanceMemberIcon = AssetDatabase.LoadAssetAtPath<Sprite>(
            ResistanceMemberIconPath);
        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FirstEntryFontPath);
        if (regionsBasePlate == null || operationsBasePlate == null ||
            defenseBasePlate == null || alertBasePlate == null || alertIconSheet == null ||
            containmentBasePlate == null || majorPactBasePlate == null ||
            majorPactLineSymbols == null ||
            resistancePactsBasePlate == null ||
            resistancePactsFidelityPlate == null ||
            resistancePactsStaticIcons == null || resistancePactsIconSheet == null ||
            resistancePactsInnerSymbols == null ||
            resistanceChromaKey == null ||
            resistanceMemberIcon == null || font == null)
        {
            Debug.LogError("[D2 Resistance] Falta una placa limpia o Cinzel.");
            return;
        }

        GameObject root = CreateUIObject("D2_Civilization2", panel.transform);
        panel.civilization2Root = root;

        D2Civilization2PanelUI civilization2UI = Undo.AddComponent<D2Civilization2PanelUI>(root);
        panel.civilization2PanelUI = civilization2UI;
        civilization2UI.dimension2PanelUI = panel;

        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = rootRect.anchorMax = new Vector2(0.5f, 0.5f);
        rootRect.pivot = new Vector2(0.5f, 0.5f);
        rootRect.anchoredPosition = new Vector2(0f, -122f);
        rootRect.sizeDelta = new Vector2(FirstEntryWidth, FirstEntryHeight);
        Canvas resistanceCanvas = Undo.AddComponent<Canvas>(root);
        resistanceCanvas.overrideSorting = true;
        resistanceCanvas.sortingOrder = 32743;
        SerializedObject resistanceCanvasSettings = new SerializedObject(resistanceCanvas);
        resistanceCanvasSettings.FindProperty("m_OverrideSorting").boolValue = true;
        resistanceCanvasSettings.FindProperty("m_SortingOrder").intValue = 32743;
        resistanceCanvasSettings.ApplyModifiedPropertiesWithoutUndo();
        Undo.AddComponent<GraphicRaycaster>(root);

        Color ivory = FirstEntryHex("E3C99D");
        Color bronze = FirstEntryHex("C49A63");
        Color red = FirstEntryHex("E46145");
        Color purple = FirstEntryHex("B978CE");
        Color muted = FirstEntryHex("B79B74");

        civilization2UI.regionDropdown = CreateDropdown(
            "Civ2RegionDropdownData", root.transform,
            new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        CanvasGroup hiddenRegionDropdown = Undo.AddComponent<CanvasGroup>(
            civilization2UI.regionDropdown.gameObject);
        hiddenRegionDropdown.alpha = 0f;
        hiddenRegionDropdown.interactable = false;
        hiddenRegionDropdown.blocksRaycasts = false;
        civilization2UI.objectiveText = CreateText(
            "Civilization2ObjectiveData", root.transform, "", 1f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f),
            new Vector2(0.001f, 0.001f));
        civilization2UI.objectiveText.gameObject.SetActive(false);

        civilization2UI.regionSectionRoot = CreateView(
            "D2_Civ2_RegionSection", root.transform
        );
        Transform regionRoot = civilization2UI.regionSectionRoot.transform;
        RawImage regionBase = CreateFirstEntryRawImage(
            "ResistanceRegionsBasePlate", regionRoot, regionsBasePlate, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight, new Rect(0f, 0f, 1f, 1f));
        regionBase.raycastTarget = false;

        CreateMapText("ResistanceRegionsTitle", regionRoot, "RED DE RESISTENCIA", font,
            170f, 34f, 740f, 70f, 49f, ivory, TextAlignmentOptions.Center, true);
        CreateMapText("AvailableHeaderLabel", regionRoot, "DISPONIBLES", font,
            140f, 118f, 175f, 31f, 21f, bronze, TextAlignmentOptions.Left, true);
        civilization2UI.availableMembersValueText = CreateMapText(
            "AvailableHeaderValue", regionRoot, "3,845", font,
            140f, 148f, 175f, 45f, 34f, ivory, TextAlignmentOptions.Left, true);
        CreateMapText("AssignedHeaderLabel", regionRoot, "ASIGNADOS", font,
            386f, 118f, 170f, 31f, 21f, bronze, TextAlignmentOptions.Left, true);
        civilization2UI.assignedMembersValueText = CreateMapText(
            "AssignedHeaderValue", regionRoot, "1,325", font,
            410f, 148f, 150f, 45f, 34f, ivory, TextAlignmentOptions.Left, true);
        CreateMapText("TotalHeaderLabel", regionRoot, "TOTAL", font,
            650f, 118f, 115f, 31f, 21f, bronze, TextAlignmentOptions.Left, true);
        civilization2UI.totalMembersValueText = CreateMapText(
            "TotalHeaderValue", regionRoot, "5,170", font,
            676f, 148f, 120f, 45f, 34f, ivory, TextAlignmentOptions.Left, true);
        CreateMapText("DominanceHeaderLabel", regionRoot, "DOMINIO TOTAL", font,
            846f, 118f, 190f, 31f, 20f, bronze, TextAlignmentOptions.Left, true);
        civilization2UI.totalDominanceValueText = CreateMapText(
            "DominanceHeaderValue", regionRoot, "72%", font,
            880f, 148f, 130f, 45f, 34f, ivory, TextAlignmentOptions.Left, true);
        civilization2UI.membersText = civilization2UI.availableMembersValueText;
        civilization2UI.dominanceText = civilization2UI.totalDominanceValueText;
        civilization2UI.dominanceSlider = CreateProgressSlider(
            "TotalDominanceSliderData", regionRoot,
            new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f)
        );
        civilization2UI.dominanceSlider.gameObject.SetActive(false);

        // These bounds follow the visible card frames in the clean plate.  Keeping
        // one measured frame per card avoids centering the text against the map
        // region (whose silhouette is deliberately asymmetric).
        float[] cardX = { 164f, 660f, 410f };
        float[] cardY = { 382f, 382f, 724f };
        float[] cardW = { 273f, 253f, 265f };
        civilization2UI.regionNameTexts = new TMP_Text[3];
        civilization2UI.regionDominanceValueTexts = new TMP_Text[3];
        civilization2UI.regionThreatValueTexts = new TMP_Text[3];
        civilization2UI.regionMembersValueTexts = new TMP_Text[3];
        civilization2UI.regionButtons = new Button[3];
        for (int regionIndex = 0; regionIndex < 3; regionIndex++)
        {
            civilization2UI.regionNameTexts[regionIndex] = CreateMapText(
                "RegionName" + regionIndex, regionRoot, "REGIÓN " + (regionIndex + 1), font,
                cardX[regionIndex], cardY[regionIndex], cardW[regionIndex], 55f,
                36f, ivory, TextAlignmentOptions.Center, true);
            CreateMapText("RegionDominanceLabel" + regionIndex, regionRoot, "DOMINIO", font,
                cardX[regionIndex] + 55f, cardY[regionIndex] + 66f, 120f, 42f,
                20f, bronze, TextAlignmentOptions.Left, true);
            civilization2UI.regionDominanceValueTexts[regionIndex] = CreateMapText(
                "RegionDominanceValue" + regionIndex, regionRoot, "72%", font,
                cardX[regionIndex] + 166f, cardY[regionIndex] + 66f,
                cardW[regionIndex] - 184f, 42f, 31f, red,
                TextAlignmentOptions.Right, true);
            CreateMapText("RegionThreatLabel" + regionIndex, regionRoot, "AMENAZA", font,
                cardX[regionIndex] + 55f, cardY[regionIndex] + 126f, 125f, 42f,
                20f, bronze, TextAlignmentOptions.Left, true);
            civilization2UI.regionThreatValueTexts[regionIndex] = CreateMapText(
                "RegionThreatValue" + regionIndex, regionRoot, "40%", font,
                cardX[regionIndex] + 166f, cardY[regionIndex] + 126f,
                cardW[regionIndex] - 184f, 42f, 31f, purple,
                TextAlignmentOptions.Right, true);
            CreateMapText("RegionMembersLabel" + regionIndex, regionRoot, "MIEMBROS", font,
                cardX[regionIndex] + 55f, cardY[regionIndex] + 186f, 130f, 42f,
                20f, bronze, TextAlignmentOptions.Left, true);
            civilization2UI.regionMembersValueTexts[regionIndex] = CreateMapText(
                "RegionMembersValue" + regionIndex, regionRoot, "410", font,
                cardX[regionIndex] + 166f, cardY[regionIndex] + 186f,
                cardW[regionIndex] - 184f, 42f, 31f, ivory,
                TextAlignmentOptions.Right, true);
        }
        civilization2UI.regionButtons[0] = CreateMapInvisibleButton(
            "Btn_SelectRegion1", root.transform, 70f, 245f, 465f, 520f);
        civilization2UI.regionButtons[1] = CreateMapInvisibleButton(
            "Btn_SelectRegion2", root.transform, 545f, 245f, 465f, 520f);
        civilization2UI.regionButtons[2] = CreateMapInvisibleButton(
            "Btn_SelectRegion3", root.transform, 300f, 650f, 480f, 425f);
        civilization2UI.region1Text = civilization2UI.regionNameTexts[0];
        civilization2UI.region2Text = civilization2UI.regionNameTexts[1];
        civilization2UI.region3Text = civilization2UI.regionNameTexts[2];
        civilization2UI.region4Text = CreateText(
            "Region4Data", regionRoot, "", 1f, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        civilization2UI.region4Text.gameObject.SetActive(false);

        civilization2UI.detailTitleText = CreateMapText(
            "RegionDetailTitle", regionRoot, "REGIÓN 1 — DISPONIBLE", font,
            145f, 1084f, 790f, 66f, 43f, ivory, TextAlignmentOptions.Center, true);
        CreateMapText("DetailDominanceLabel", regionRoot, "DOMINIO", font,
            142f, 1180f, 125f, 48f, 23f, bronze, TextAlignmentOptions.Left, true);
        civilization2UI.detailDominanceValueText = CreateMapText(
            "DetailDominanceValue", regionRoot, "52%", font,
            275f, 1180f, 95f, 48f, 34f, red, TextAlignmentOptions.Right, true);
        CreateMapText("DetailThreatLabel", regionRoot, "AMENAZA", font,
            450f, 1180f, 125f, 48f, 23f, bronze, TextAlignmentOptions.Left, true);
        civilization2UI.detailThreatValueText = CreateMapText(
            "DetailThreatValue", regionRoot, "68%", font,
            585f, 1180f, 90f, 48f, 34f, purple, TextAlignmentOptions.Right, true);
        CreateMapText("DetailMembersLabel", regionRoot, "MIEMBROS", font,
            770f, 1180f, 125f, 48f, 23f, bronze, TextAlignmentOptions.Left, true);
        civilization2UI.detailMembersValueText = CreateMapText(
            "DetailMembersValue", regionRoot, "520", font,
            900f, 1180f, 95f, 48f, 34f, ivory, TextAlignmentOptions.Right, true);
        civilization2UI.assignmentText = CreateMapText(
            "RegionAssignmentLabel", regionRoot,
            "ASIGNACIÓN REGIONAL · SIN DESTINAR", font,
            142f, 1261f, 650f, 56f, 27f, bronze, TextAlignmentOptions.Center, true);
        civilization2UI.detailIdleValueText = CreateMapText(
            "RegionIdleValue", regionRoot, "120", font,
            850f, 1254f, 150f, 64f, 41f, ivory, TextAlignmentOptions.Center, true);

        civilization2UI.releaseOneButton = CreateMapInvisibleButton(
            "Btn_Civ2ReleaseOne", regionRoot, 66f, 1348f, 151f, 107f);
        civilization2UI.assignOneButton = CreateMapInvisibleButton(
            "Btn_Civ2AssignOne", regionRoot, 224f, 1348f, 151f, 107f);
        civilization2UI.assignTenButton = CreateMapInvisibleButton(
            "Btn_Civ2AssignTen", regionRoot, 383f, 1348f, 151f, 107f);
        civilization2UI.assignAllButton = CreateMapInvisibleButton(
            "Btn_Civ2AssignAll", regionRoot, 542f, 1348f, 205f, 107f);
        civilization2UI.releaseAllButton = CreateMapInvisibleButton(
            "Btn_Civ2ReleaseAll", regionRoot, 755f, 1348f, 258f, 107f);
        string[] assignmentLabels = { "−1", "+1", "+10", "ASIGNAR\nTODOS", "LIBERAR\nTODOS" };
        Button[] assignmentButtons =
        {
            civilization2UI.releaseOneButton, civilization2UI.assignOneButton,
            civilization2UI.assignTenButton, civilization2UI.assignAllButton,
            civilization2UI.releaseAllButton
        };
        float[] assignmentWidths = { 151f, 151f, 151f, 205f, 258f };
        for (int buttonIndex = 0; buttonIndex < assignmentButtons.Length; buttonIndex++)
        {
            bool isCompactAmountButton = buttonIndex < 3;
            CreateMapText("Label", assignmentButtons[buttonIndex].transform,
                assignmentLabels[buttonIndex], font,
                8f, isCompactAmountButton ? 31f : 28f,
                assignmentWidths[buttonIndex] - 16f,
                isCompactAmountButton ? 64f : 74f,
                isCompactAmountButton ? 36f : 27f,
                buttonIndex == 4 ? FirstEntryHex("D39A76") : ivory,
                TextAlignmentOptions.Center, true);
        }

        CreateMapText("NowLabel", regionRoot, "AHORA", font,
            168f, 1482f, 220f, 42f, 29f, bronze, TextAlignmentOptions.Left, true);
        civilization2UI.lastResultText = CreateMapText(
            "NowBody", regionRoot, "DISTRIBUYE MIEMBROS EN LAS REGIONES.", font,
            168f, 1523f, 780f, 48f, 27f, muted, TextAlignmentOptions.Left, true);
        CreateMapText("NextLabel", regionRoot, "DESPUÉS", font,
            168f, 1593f, 230f, 42f, 29f, bronze, TextAlignmentOptions.Left, true);
        CreateMapText("NextBody", regionRoot, "ORGANIZA OPERACIONES.", font,
            168f, 1634f, 720f, 48f, 27f, muted, TextAlignmentOptions.Left, true);

        // Exact cell borders measured on the bottom plate.  The old equal-width
        // approximation displaced every label after REGIONES progressively left.
        float[] navX = { 31f, 214f, 403f, 570f, 737f, 904f };
        float[] navW = { 183f, 189f, 167f, 167f, 167f, 145f };
        float[] navLabelW = { 175f, 185f, 159f, 163f, 155f, 165f };
        string[] navLabels =
        {
            "REGIONES", "OPERACIONES", "DEFENSA",
            "RESISTENCIA", "ALERTA", "CONTENCIÓN"
        };
        Button[] navButtons = new Button[6];
        for (int navIndex = 0; navIndex < navButtons.Length; navIndex++)
        {
            navButtons[navIndex] = CreateMapInvisibleButton(
                "Btn_Civ2Nav" + navIndex, root.transform,
                navX[navIndex], 1733f, navW[navIndex], 174f);
            CreateMapText("Label", navButtons[navIndex].transform, navLabels[navIndex], font,
                (navW[navIndex] - navLabelW[navIndex]) * 0.5f, 120f,
                navLabelW[navIndex], 43f,
                navIndex == 0 ? 22f : 18f,
                navIndex == 0 ? ivory : bronze, TextAlignmentOptions.Center, true);
        }
        civilization2UI.showRegionsButton = navButtons[0];
        civilization2UI.showOperationsButton = navButtons[1];
        civilization2UI.showDefenseButton = navButtons[2];
        civilization2UI.showResistanceButton = navButtons[3];
        civilization2UI.showAlertButton = navButtons[4];
        civilization2UI.showContainmentButton = navButtons[5];

        civilization2UI.backToMapButton = CreateMapInvisibleButton(
            "Btn_Civ2BackToMap", root.transform, 34f, 27f, 92f, 76f);
        civilization2UI.helpButton = CreateMapInvisibleButton(
            "Btn_Civ2Help", root.transform, 945f, 27f, 92f, 76f);
        CreateMapText("Glyph", civilization2UI.helpButton.transform, "?", font,
            7f, -2f, 78f, 74f, 58f, bronze, TextAlignmentOptions.Center, true);

        civilization2UI.operationsSectionRoot = CreateView(
            "D2_Civ2_OperationsSection", root.transform
        );
        Transform operationsRoot = civilization2UI.operationsSectionRoot.transform;
        D2OperationsPanelUI operationsUI = Undo.AddComponent<D2OperationsPanelUI>(
            civilization2UI.operationsSectionRoot
        );
        civilization2UI.operationsPanelUI = operationsUI;
        operationsUI.civilization2PanelUI = civilization2UI;

        RawImage operationsBase = CreateFirstEntryRawImage(
            "ResistanceOperationsBasePlate", operationsRoot, operationsBasePlate, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight, new Rect(0f, 0f, 1f, 1f));
        operationsBase.raycastTarget = false;

        operationsUI.operationDropdown = CreateDropdown(
            "OperationDropdownData", operationsRoot,
            new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        CanvasGroup hiddenOperationDropdown = Undo.AddComponent<CanvasGroup>(
            operationsUI.operationDropdown.gameObject);
        hiddenOperationDropdown.alpha = 0f;
        hiddenOperationDropdown.interactable = false;
        hiddenOperationDropdown.blocksRaycasts = false;

        Color operationsGreen = FirstEntryHex("8EAF70");
        Color operationsGray = FirstEntryHex("A28E72");
        Color operationsRed = FirstEntryHex("E2553E");
        Color operationsPurple = FirstEntryHex("A96CAE");

        TMP_Text operationsTitle = CreateMapText("ResistanceOperationsTitle", operationsRoot,
            "OPERACIONES DE RESISTENCIA", font,
            130f, 27f, 820f, 72f, 52f, ivory, TextAlignmentOptions.Center, true);
        operationsTitle.enableAutoSizing = false;
        operationsTitle.fontSize = 44f;
        operationsTitle.fontSizeMin = 44f;
        operationsTitle.fontSizeMax = 44f;
        operationsTitle.textWrappingMode = TextWrappingModes.NoWrap;
        operationsTitle.overflowMode = TextOverflowModes.Overflow;
        operationsUI.regionNameText = CreateMapText(
            "OperationsRegionName", operationsRoot, "REGIÓN 1", font,
            165f, 132f, 240f, 64f, 42f, ivory, TextAlignmentOptions.Center, true);
        operationsUI.regionalMembersText = operationsUI.regionNameText;
        CreateMapText("OperationsIdleLabel", operationsRoot, "SIN DESTINAR", font,
            515f, 124f, 185f, 36f, 23f, bronze, TextAlignmentOptions.Left, true);
        operationsUI.regionalIdleValueText = CreateMapText(
            "OperationsIdleValue", operationsRoot, "120", font,
            515f, 158f, 150f, 48f, 36f, ivory, TextAlignmentOptions.Left, true);
        CreateMapText("OperationsAssignedLabel", operationsRoot, "EN OPERACIONES", font,
            817f, 124f, 225f, 36f, 22f, bronze, TextAlignmentOptions.Left, true);
        operationsUI.regionalOperationsValueText = CreateMapText(
            "OperationsAssignedValue", operationsRoot, "10", font,
            827f, 158f, 120f, 48f, 36f, ivory, TextAlignmentOptions.Left, true);

        float[] operationCardX = { 31f, 298f, 554f, 808f };
        float[] operationCardW = { 252f, 240f, 239f, 240f };
        string[] operationNames = { "RESCATE", "PROTECCIÓN", "ESPIONAJE", "SABOTAJE" };
        string[] operationRequirements = { "5", "5", "10", "20" };
        operationsUI.operationButtons = new Button[4];
        operationsUI.operationSelectionOverlays = new GameObject[4];
        operationsUI.operationStatusFillImages = new Image[4];
        operationsUI.operationStateTexts = new TMP_Text[4];
        operationsUI.operationAssignedValueTexts = new TMP_Text[4];
        operationsUI.operationRequirementValueTexts = new TMP_Text[4];
        for (int operationIndex = 0; operationIndex < 4; operationIndex++)
        {
            Image selectionFill;
            RectTransform selection = CreateFirstEntryPanel(
                "OperationSelection" + operationIndex, operationsRoot,
                operationCardX[operationIndex] - 2f, 256f,
                operationCardW[operationIndex] + 4f, 459f,
                Color.clear, FirstEntryHex("E2553E", 220),
                FirstEntryHex("FF9A64", 150), 3f, 7f, false,
                out selectionFill);
            selectionFill.raycastTarget = false;
            foreach (Image selectionRail in selection.GetComponentsInChildren<Image>(true))
            {
                if (selectionRail == selectionFill) continue;
                Outline glow = Undo.AddComponent<Outline>(selectionRail.gameObject);
                glow.effectColor = FirstEntryHex("D74331", 80);
                glow.effectDistance = new Vector2(2f, -2f);
                glow.useGraphicAlpha = true;
            }
            operationsUI.operationSelectionOverlays[operationIndex] = selection.gameObject;
            selection.gameObject.SetActive(operationIndex == 0);

            operationsUI.operationStatusFillImages[operationIndex] = CreateFirstEntryImage(
                "OperationStatusFill" + operationIndex, operationsRoot, null,
                operationCardX[operationIndex] + 56f, 516f,
                operationCardW[operationIndex] - 112f, 34f,
                operationIndex < 2
                    ? FirstEntryHex("355328", 150)
                    : FirstEntryHex("11130F", 75), false);

            CreateMapText("OperationName" + operationIndex, operationsRoot,
                operationNames[operationIndex], font,
                operationCardX[operationIndex], 278f,
                operationCardW[operationIndex], 54f,
                operationIndex == 1 ? 28f : 31f,
                ivory, TextAlignmentOptions.Center, true);
            operationsUI.operationStateTexts[operationIndex] = CreateMapText(
                "OperationCardState" + operationIndex, operationsRoot,
                operationIndex < 2 ? "ACTIVA" : "INACTIVA", font,
                operationCardX[operationIndex] + 40f, 512f,
                operationCardW[operationIndex] - 80f, 46f,
                22f, operationIndex < 2 ? operationsGreen : operationsGray,
                TextAlignmentOptions.Center, true);
            CreateMapText("OperationAssignedLabel" + operationIndex, operationsRoot,
                "ASIGNADOS", font,
                operationCardX[operationIndex] + 62f, 582f, 122f, 42f,
                18f, bronze, TextAlignmentOptions.Left, true);
            operationsUI.operationAssignedValueTexts[operationIndex] = CreateMapText(
                "OperationAssignedValue" + operationIndex, operationsRoot,
                operationIndex < 2 ? "5" : "0", font,
                operationCardX[operationIndex] + operationCardW[operationIndex] - 60f,
                578f, 40f, 48f, 29f,
                operationIndex < 2 ? operationsGreen : operationsGray,
                TextAlignmentOptions.Right, true);
            CreateMapText("OperationRequirementLabel" + operationIndex, operationsRoot,
                "REQUIERE", font,
                operationCardX[operationIndex] + 62f, 646f, 122f, 42f,
                18f, bronze, TextAlignmentOptions.Left, true);
            operationsUI.operationRequirementValueTexts[operationIndex] = CreateMapText(
                "OperationRequirementValue" + operationIndex, operationsRoot,
                operationRequirements[operationIndex], font,
                operationCardX[operationIndex] + operationCardW[operationIndex] - 66f,
                642f, 46f, 48f, 29f, operationsRed,
                TextAlignmentOptions.Right, true);
            operationsUI.operationButtons[operationIndex] = CreateMapInvisibleButton(
                "Btn_SelectOperation" + operationIndex, operationsRoot,
                operationCardX[operationIndex], 258f,
                operationCardW[operationIndex], 455f);
        }

        operationsUI.detailNameText = CreateMapText(
            "OperationDetailName", operationsRoot, "RESCATE", font,
            255f, 770f, 570f, 68f, 50f, ivory, TextAlignmentOptions.Center, true);
        Image detailStatusFill = CreateFirstEntryImage(
            "OperationDetailStatusFill", operationsRoot, null,
            428f, 850f, 224f, 43f, FirstEntryHex("355328", 150), false);
        detailStatusFill.raycastTarget = false;
        operationsUI.detailStatusFillImage = detailStatusFill;
        operationsUI.detailStateText = CreateMapText(
            "OperationDetailState", operationsRoot, "ACTIVA", font,
            410f, 843f, 260f, 60f, 31f, operationsGreen,
            TextAlignmentOptions.Center, true);
        operationsUI.operationStateText = operationsUI.detailStateText;
        operationsUI.detailDescriptionText = CreateMapText(
            "OperationDetailDescription", operationsRoot,
            "GENERA MIEMBROS; REDUCE LENTAMENTE\nDOMINIO Y AUMENTA AMENAZA.", font,
            132f, 914f, 816f, 92f, 27f, bronze,
            TextAlignmentOptions.Center, true);
        operationsUI.effectText = operationsUI.detailDescriptionText;

        CreateMapText("OperationMembersLabel", operationsRoot, "MIEMBROS", font,
            194f, 1047f, 260f, 58f, 32f, bronze, TextAlignmentOptions.Left, true);
        operationsUI.memberRateText = CreateMapText(
            "OperationMembersRate", operationsRoot, "+0.447 / MIN", font,
            650f, 1044f, 315f, 62f, 35f, operationsGreen,
            TextAlignmentOptions.Right, true);
        CreateMapText("OperationDominanceLabel", operationsRoot, "DOMINIO", font,
            194f, 1143f, 260f, 58f, 32f, bronze, TextAlignmentOptions.Left, true);
        operationsUI.dominanceRateText = CreateMapText(
            "OperationDominanceRate", operationsRoot, "−0.05 / MIN", font,
            650f, 1140f, 315f, 62f, 35f, operationsRed,
            TextAlignmentOptions.Right, true);
        CreateMapText("OperationThreatLabel", operationsRoot, "AMENAZA", font,
            194f, 1238f, 260f, 58f, 32f, bronze, TextAlignmentOptions.Left, true);
        operationsUI.threatRateText = CreateMapText(
            "OperationThreatRate", operationsRoot, "+0.20 / MIN", font,
            650f, 1235f, 315f, 62f, 35f, operationsPurple,
            TextAlignmentOptions.Right, true);

        operationsUI.assignmentNameText = CreateMapText(
            "OperationAssignmentName", operationsRoot, "ASIGNADOS A RESCATE", font,
            285f, 1359f, 510f, 54f, 28f, bronze,
            TextAlignmentOptions.Center, true);
        operationsUI.assignmentText = operationsUI.assignmentNameText;
        operationsUI.assignmentValueText = CreateMapText(
            "OperationAssignmentValue", operationsRoot, "5", font,
            458f, 1414f, 165f, 78f, 54f, operationsGreen,
            TextAlignmentOptions.Center, true);

        operationsUI.releaseOneButton = CreateMapInvisibleButton(
            "Btn_OperationReleaseOne", operationsRoot, 101f, 1510f, 133f, 103f);
        operationsUI.assignOneButton = CreateMapInvisibleButton(
            "Btn_OperationAssignOne", operationsRoot, 249f, 1510f, 133f, 103f);
        operationsUI.assignFiveButton = CreateMapInvisibleButton(
            "Btn_OperationAssignFive", operationsRoot, 396f, 1510f, 132f, 103f);
        operationsUI.assignAllButton = CreateMapInvisibleButton(
            "Btn_OperationAssignAll", operationsRoot, 542f, 1510f, 205f, 103f);
        operationsUI.releaseAllButton = CreateMapInvisibleButton(
            "Btn_OperationReleaseAll", operationsRoot, 767f, 1510f, 210f, 103f);
        CreateMapText("Label", operationsUI.releaseOneButton.transform, "−1", font,
            7f, 20f, 119f, 72f, 38f, ivory, TextAlignmentOptions.Center, true);
        CreateMapText("Label", operationsUI.assignOneButton.transform, "+1", font,
            7f, 20f, 119f, 72f, 38f, ivory, TextAlignmentOptions.Center, true);
        CreateMapText("Label", operationsUI.assignFiveButton.transform, "+5", font,
            7f, 20f, 118f, 72f, 38f, ivory, TextAlignmentOptions.Center, true);
        CreateMapText("Label", operationsUI.assignAllButton.transform,
            "ASIGNAR\nTODOS", font,
            70f, 14f, 127f, 82f, 25f, ivory, TextAlignmentOptions.Center, true);
        CreateMapText("Label", operationsUI.releaseAllButton.transform,
            "LIBERAR\nTODOS", font,
            70f, 14f, 132f, 82f, 25f, operationsRed,
            TextAlignmentOptions.Center, true);

        Image operationsNavSelectionFill;
        RectTransform operationsNavSelection = CreateFirstEntryPanel(
            "OperationsNavSelection", operationsRoot,
            191f, 1672f, 198f, 215f,
            Color.clear, FirstEntryHex("E2553E", 220),
            FirstEntryHex("FF9A64", 150), 3f, 7f, false,
            out operationsNavSelectionFill);
        operationsNavSelectionFill.raycastTarget = false;
        foreach (Image selectionRail in
                 operationsNavSelection.GetComponentsInChildren<Image>(true))
        {
            if (selectionRail == operationsNavSelectionFill) continue;
            Outline glow = Undo.AddComponent<Outline>(selectionRail.gameObject);
            glow.effectColor = FirstEntryHex("D74331", 80);
            glow.effectDistance = new Vector2(2f, -2f);
            glow.useGraphicAlpha = true;
        }
        float[] operationsNavX = { 25f, 193f, 387f, 549f, 720f, 889f };
        float[] operationsNavW = { 168f, 194f, 162f, 171f, 169f, 165f };
        string[] operationsNavLabels =
        {
            "REGIONES", "OPERACIONES", "DEFENSA",
            "RESISTENCIA", "ALERTA", "CONTENCIÓN"
        };
        for (int operationNavIndex = 0; operationNavIndex < operationsNavLabels.Length;
             operationNavIndex++)
        {
            CreateMapText("OperationsNavLabel" + operationNavIndex, operationsRoot,
                operationsNavLabels[operationNavIndex], font,
                operationsNavX[operationNavIndex], 1801f,
                operationsNavW[operationNavIndex], 48f,
                operationNavIndex == 1 ? 21f : 18f,
                operationNavIndex == 1 ? operationsRed : bronze,
                TextAlignmentOptions.Center, true);
        }

        civilization2UI.defenseSectionRoot = CreateView(
            "D2_Civ2_DefenseSection", root.transform
        );
        Transform defenseRoot = civilization2UI.defenseSectionRoot.transform;
        D2ReprisalsPanelUI reprisalsUI = Undo.AddComponent<D2ReprisalsPanelUI>(
            civilization2UI.defenseSectionRoot
        );
        civilization2UI.reprisalsPanelUI = reprisalsUI;
        reprisalsUI.civilization2PanelUI = civilization2UI;

        RawImage defenseBase = CreateFirstEntryRawImage(
            "ResistanceDefenseBasePlate", defenseRoot, defenseBasePlate, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight, new Rect(0f, 0f, 1f, 1f));
        defenseBase.raycastTarget = false;

        Color defenseRed = FirstEntryHex("E2553E");
        Color defenseTeal = FirstEntryHex("65B6A5");
        Color defensePurple = FirstEntryHex("B978CE");

        Image defenseNavSelectionFill;
        RectTransform defenseNavSelection = CreateFirstEntryPanel(
            "DefenseNavSelection", defenseRoot,
            376f, 1779f, 169f, 141f,
            FirstEntryHex("7B1D13", 52), defenseRed,
            FirstEntryHex("FF9A64", 125), 3f, 6f, false,
            out defenseNavSelectionFill);
        defenseNavSelectionFill.raycastTarget = false;
        foreach (Image selectionRail in
                 defenseNavSelection.GetComponentsInChildren<Image>(true))
        {
            if (selectionRail == defenseNavSelectionFill) continue;
            Outline glow = Undo.AddComponent<Outline>(selectionRail.gameObject);
            glow.effectColor = FirstEntryHex("D74331", 75);
            glow.effectDistance = new Vector2(2f, -2f);
            glow.useGraphicAlpha = true;
        }

        CreateDefenseText("ResistanceDefenseTitle", defenseRoot,
            "DEFENSA Y REPRESALIAS", font,
            205f, 24f, 670f, 70f, 45f, ivory, TextAlignmentOptions.Center);

        TextMeshProUGUI availableHeader = CreateDefenseText(
            "DefenseAvailableLabel", defenseRoot,
            "MIEMBROS\nDISPONIBLES", font,
            112f, 101f, 254f, 64f, 20f, bronze, TextAlignmentOptions.Left);
        availableHeader.characterSpacing = -0.5f;
        availableHeader.lineSpacing = -8f;
        reprisalsUI.availableMembersText = CreateDefenseText(
            "DefenseAvailableValue", defenseRoot, "3,845", font,
            116f, 155f, 170f, 43f, 30f, ivory, TextAlignmentOptions.Left);
        TextMeshProUGUI fragmentsHeader = CreateDefenseText(
            "DefenseFragmentsLabel", defenseRoot,
            "FRAGMENTOS DE\nCONTROL", font,
            454f, 101f, 256f, 64f, 20f, bronze, TextAlignmentOptions.Left);
        fragmentsHeader.characterSpacing = -0.5f;
        fragmentsHeader.lineSpacing = -8f;
        reprisalsUI.fragmentsText = CreateDefenseText(
            "DefenseFragmentsValue", defenseRoot, "78", font,
            460f, 155f, 120f, 43f, 30f, ivory, TextAlignmentOptions.Left);
        TextMeshProUGUI reprisalsHeader = CreateDefenseText(
            "DefenseReprisalsLabel", defenseRoot,
            "REPRESALIAS\nRESISTIDAS", font,
            802f, 101f, 238f, 64f, 20f, bronze, TextAlignmentOptions.Left);
        reprisalsHeader.characterSpacing = -0.5f;
        reprisalsHeader.lineSpacing = -8f;
        reprisalsUI.reprisalsCountText = CreateDefenseText(
            "DefenseReprisalsValue", defenseRoot, "12", font,
            809f, 155f, 120f, 43f, 30f, ivory, TextAlignmentOptions.Left);

        reprisalsUI.regionNameText = CreateDefenseText(
            "DefenseRegionName", defenseRoot, "REGIÓN 1", font,
            417f, 245f, 247f, 60f, 38f, ivory, TextAlignmentOptions.Center);
        reprisalsUI.regionSelectorButton = CreateMapInvisibleButton(
            "Btn_DefenseCycleRegion", defenseRoot, 104f, 224f, 868f, 99f);

        CreateDefenseText("DefenseThreatLabel", defenseRoot, "AMENAZA", font,
            177f, 361f, 236f, 56f, 35f, defenseRed, TextAlignmentOptions.Center);
        CreateDefenseText("DefenseCoverageLabel", defenseRoot, "COBERTURA", font,
            650f, 361f, 264f, 56f, 35f, defenseTeal, TextAlignmentOptions.Center);

        GameObject threatGaugeObject = CreateUIObject("DefenseThreatGauge", defenseRoot);
        SetFirstEntryTopLeft(threatGaugeObject.GetComponent<RectTransform>(),
            123f, 412f, 337f, 313f);
        reprisalsUI.threatGauge = Undo.AddComponent<D2SegmentedGaugeGraphic>(
            threatGaugeObject);
        reprisalsUI.threatGauge.color = FirstEntryHex("E64F35", 225);
        reprisalsUI.threatGauge.raycastTarget = false;
        reprisalsUI.threatGauge.Configure(14, 0.39f, 0.49f, 3.2f, 90f, true);
        reprisalsUI.threatGauge.SetProgress(0.72f);

        GameObject coverageGaugeObject = CreateUIObject(
            "DefenseCoverageGauge", defenseRoot);
        SetFirstEntryTopLeft(coverageGaugeObject.GetComponent<RectTransform>(),
            614f, 412f, 336f, 313f);
        reprisalsUI.coverageGauge = Undo.AddComponent<D2SegmentedGaugeGraphic>(
            coverageGaugeObject);
        reprisalsUI.coverageGauge.color = FirstEntryHex("4DB7A5", 220);
        reprisalsUI.coverageGauge.raycastTarget = false;
        reprisalsUI.coverageGauge.Configure(14, 0.39f, 0.49f, 3.2f, 90f, true);
        reprisalsUI.coverageGauge.SetProgress(58f / 60f);

        reprisalsUI.threatText = CreateDefenseText(
            "DefenseThreatValue", defenseRoot, "72%", font,
            190f, 727f, 205f, 68f, 51f, defenseRed, TextAlignmentOptions.Center);
        reprisalsUI.threatText.overflowMode = TextOverflowModes.Overflow;
        reprisalsUI.threatSlider = CreateProgressSlider(
            "DefenseThreatSliderData", defenseRoot,
            new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        reprisalsUI.threatSlider.gameObject.SetActive(false);
        reprisalsUI.coverageText = CreateDefenseText(
            "DefenseCoverageValue", defenseRoot, "58 / 60", font,
            681f, 729f, 200f, 67f, 47f, defenseTeal, TextAlignmentOptions.Center);
        reprisalsUI.coverageSlider = CreateProgressSlider(
            "DefenseCoverageSliderData", defenseRoot,
            new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        reprisalsUI.coverageSlider.gameObject.SetActive(false);

        CreateDefenseText("DefenseLossLabel", defenseRoot,
            "PÉRDIDA ESTIMADA", font,
            127f, 843f, 317f, 48f, 27f, bronze, TextAlignmentOptions.Center);
        reprisalsUI.estimatedLossText = CreateDefenseText(
            "DefenseLossValue", defenseRoot, "2%", font,
            288f, 930f, 129f, 75f, 49f, defenseRed, TextAlignmentOptions.Center);
        CreateDefenseText("DefenseEspionageLabel", defenseRoot,
            "ESPIONAJE PREPARADO", font,
            614f, 844f, 356f, 49f, 27f, bronze, TextAlignmentOptions.Center);
        reprisalsUI.protectionText = CreateDefenseText(
            "DefenseEspionageValue", defenseRoot, "LISTO (-5%)", font,
            702f, 928f, 307f, 77f, 41f, defensePurple,
            TextAlignmentOptions.Center);

        CreateDefenseText("DefenseWeakeningLabel", defenseRoot,
            "OPERACIÓN DEBILITADA", font,
            324f, 1081f, 465f, 48f, 28f, bronze, TextAlignmentOptions.Left);
        reprisalsUI.weakeningText = CreateDefenseText(
            "DefenseWeakeningOperation", defenseRoot, "SABOTAJE AL 50%", font,
            324f, 1139f, 430f, 52f, 31f, bronze, TextAlignmentOptions.Left);
        reprisalsUI.weakeningDurationText = CreateDefenseText(
            "DefenseWeakeningDuration", defenseRoot, "02:10", font,
            331f, 1200f, 195f, 60f, 34f, bronze, TextAlignmentOptions.Left);

        CreateDefenseText("DefenseRulesLabel", defenseRoot,
            "REGLAS DE REPRESALIA", font,
            329f, 1307f, 406f, 52f, 29f, bronze, TextAlignmentOptions.Left);
        // The Cinzel line advance is about 29–30 px here. Keep each marker on
        // the optical glyph center instead of distributing them at a fixed 32 px.
        float[] ruleBulletCenterY = { 1403f, 1432f, 1462f, 1491f };
        for (int ruleIndex = 0; ruleIndex < ruleBulletCenterY.Length; ruleIndex++)
        {
            CreateFirstEntryDiamond("DefenseRuleBullet" + ruleIndex, defenseRoot,
                352f, ruleBulletCenterY[ruleIndex], 10f,
                defenseRed, FirstEntryHex("2B100B"));
        }
        reprisalsUI.rulesText = CreateDefenseText(
            "DefenseRules", defenseRoot,
            "AL LLEGAR A 100% OCURRE UNA REPRESALIA\n" +
            "LA AMENAZA VUELVE A 25%\n" +
            "LA COBERTURA CONSERVA LA MITAD\n" +
            "RECOMPENSA 3 FRAGMENTOS", font,
            362f, 1366f, 630f, 164f, 20f, bronze, TextAlignmentOptions.Left);
        reprisalsUI.rulesText.lineSpacing = 13f;

        CreateDefenseText("DefenseLastResultLabel", defenseRoot,
            "ÚLTIMA REPRESALIA", font,
            328f, 1588f, 471f, 60f, 35f, defenseRed, TextAlignmentOptions.Left);
        reprisalsUI.lastResultText = CreateDefenseText(
            "DefenseLastResult", defenseRoot, "8 MIEMBROS PERDIDOS", font,
            375f, 1664f, 500f, 62f, 31f, defenseRed,
            TextAlignmentOptions.Left);
        CreateFirstEntryImage("DefenseLastResultMemberIcon", defenseRoot,
            resistanceMemberIcon, 335f, 1668f, 30f, 38f,
            Color.white, true);

        float[] defenseNavX = { 33f, 202f, 376f, 545f, 717f, 887f };
        float[] defenseNavW = { 169f, 174f, 169f, 172f, 170f, 164f };
        string[] defenseNavLabels =
        {
            "REGIONES", "OPERACIONES", "DEFENSA",
            "RESISTENCIA", "ALERTA", "CONTENCIÓN"
        };
        for (int defenseNavIndex = 0; defenseNavIndex < defenseNavLabels.Length;
             defenseNavIndex++)
        {
            CreateDefenseText("DefenseNavLabel" + defenseNavIndex, defenseRoot,
                defenseNavLabels[defenseNavIndex], font,
                defenseNavX[defenseNavIndex], 1860f,
                defenseNavW[defenseNavIndex], 44f,
                defenseNavIndex == 2 ? 20f : 17f,
                defenseNavIndex == 2 ? defenseRed : bronze,
                TextAlignmentOptions.Center);
        }
        // Keep the dynamic value above every decorative layer. This is important
        // in batch captures, where custom graphics can otherwise retain stale culling.
        reprisalsUI.threatText.transform.SetAsLastSibling();

        civilization2UI.resistanceSectionRoot = CreateView(
            "D2_Civ2_ResistanceSection", root.transform
        );
        Transform resistanceRoot = civilization2UI.resistanceSectionRoot.transform;
        D2ResistancePanelUI resistanceUI = Undo.AddComponent<D2ResistancePanelUI>(
            civilization2UI.resistanceSectionRoot
        );
        civilization2UI.resistancePanelUI = resistanceUI;
        RawImage resistanceBase = CreateFirstEntryRawImage(
            "ResistancePactsBasePlate", resistanceRoot,
            resistancePactsBasePlate, null, 0f, 0f,
            FirstEntryWidth, FirstEntryHeight, new Rect(0f, 0f, 1f, 1f));
        resistanceBase.raycastTarget = false;
        RawImage upgradeFidelity = CreateFirstEntryRawImage(
            "ResistancePactsUpgradeFidelity", resistanceRoot,
            resistancePactsFidelityPlate, null,
            0f, 225.1f, 1080f, 581.1f,
            new Rect(0f, 0.5801f, 1f, 0.3026f));
        upgradeFidelity.raycastTarget = false;
        RawImage headerFidelity = CreateFirstEntryRawImage(
            "ResistancePactsHeaderFidelity", resistanceRoot,
            resistancePactsFidelityPlate, null,
            0f, 0f, 1080f, 225.1f,
            new Rect(0f, 0.8828f, 1f, 0.1172f));
        headerFidelity.raycastTarget = false;
        RawImage secondaryCardsFidelity = CreateFirstEntryRawImage(
            "ResistancePactsSecondaryCardsFidelity", resistanceRoot,
            resistancePactsFidelityPlate, null,
            384.5f, 806f, 695.5f, 574.2f,
            new Rect(0.356f, 0.2811f, 0.644f, 0.299f));
        secondaryCardsFidelity.raycastTarget = false;
        RawImage detailFidelity = CreateFirstEntryRawImage(
            "ResistancePactsDetailFidelity", resistanceRoot,
            resistancePactsFidelityPlate, null,
            0f, 1380.3f, 1080f, 345.6f,
            new Rect(0f, 0.1011f, 1f, 0.18f));
        detailFidelity.raycastTarget = false;
        RawImage navFidelity = CreateFirstEntryRawImage(
            "ResistancePactsNavFidelity", resistanceRoot,
            resistancePactsFidelityPlate, null,
            0f, 1725.6f, 1080f, 194.4f,
            new Rect(0f, 0f, 1f, 0.1011f));
        navFidelity.raycastTarget = false;
        Rect[] staticSymbolCrops =
        {
            new Rect(25f, 31f, 63f, 57f),
            new Rect(78f, 122f, 63f, 78f),
            new Rect(355f, 119f, 72f, 82f),
            new Rect(657f, 119f, 76f, 83f),
            new Rect(65f, 1535f, 88f, 90f),
            new Rect(218f, 1536f, 75f, 89f),
            new Rect(360f, 1534f, 70f, 91f),
            new Rect(502f, 1534f, 70f, 91f),
            new Rect(652f, 1534f, 78f, 91f),
            new Rect(803f, 1534f, 70f, 91f)
        };
        for (int staticSymbolIndex = 0;
             staticSymbolIndex < staticSymbolCrops.Length;
             staticSymbolIndex++)
        {
            CreateResistanceMappedIconCrop(
                "ResistanceStaticSymbol" + staticSymbolIndex,
                resistanceRoot, resistancePactsStaticIcons,
                resistanceChromaKey, staticSymbolCrops[staticSymbolIndex]);
        }

        Color resistanceRed = FirstEntryHex("D94E34");
        Color resistanceTeal = FirstEntryHex("78AAA0");
        Color resistanceDark = FirstEntryHex("120B08", 196);

        TMP_Text resistanceTitle = CreateDefenseText(
            "ResistanceTitle", resistanceRoot, "RESISTENCIA", font,
            90f, 27f, 900f, 92f, 58f, ivory, TextAlignmentOptions.Center);
        resistanceTitle.overflowMode = TextOverflowModes.Overflow;

        CreateDefenseText("ResistanceAvailableLabel", resistanceRoot,
            "MIEMBROS DISPONIBLES", font,
            130f, 124f, 300f, 30f, 18f, bronze, TextAlignmentOptions.Center);
        resistanceUI.availableMembersText = CreateDefenseText(
            "ResistanceAvailableValue", resistanceRoot, "3,845", font,
            167f, 147f, 230f, 54f, 36f, ivory, TextAlignmentOptions.Center);
        CreateDefenseText("ResistanceFragmentsLabel", resistanceRoot,
            "FRAGMENTOS DE CONTROL", font,
            435f, 124f, 360f, 30f, 18f, bronze, TextAlignmentOptions.Center);
        resistanceUI.fragmentsText = CreateDefenseText(
            "ResistanceFragmentsValue", resistanceRoot, "78", font,
            487f, 147f, 263f, 54f, 36f, ivory, TextAlignmentOptions.Center);
        CreateDefenseText("ResistanceExhaustedLabel", resistanceRoot,
            "AGOTADOS", font,
            835f, 124f, 155f, 30f, 20f, bronze, TextAlignmentOptions.Center);
        resistanceUI.exhaustedText = CreateDefenseText(
            "ResistanceExhaustedValue", resistanceRoot, "215", font,
            836f, 147f, 153f, 54f, 36f, ivory, TextAlignmentOptions.Center);

        CreateDefenseText("ResistanceUpgradesTitle", resistanceRoot,
            "MEJORAS DE RESISTENCIA", font,
            190f, 246f, 700f, 61f, 43f, ivory, TextAlignmentOptions.Center);

        float[] upgradeCardX = { 52f, 297f, 544f, 790f };
        float[] upgradeCardW = { 230f, 232f, 232f, 234f };
        string[] upgradeTitles =
        {
            "ROMPER MARCA\nMENOR", "REFUGIOS\nSELLADOS",
            "LECTURA DE\nSÍMBOLOS", "FALLA EN\nLA CADENA"
        };
        string[] upgradeEffects =
        {
            "RESCATE\n+10% MIEMBROS\nPOR NIVEL",
            "PROTECCIÓN\n+10% COBERTURA\nPOR NIVEL",
            "ESPIONAJE\nREDUCE 1 PUNTO\nEXTRA DE PÉRDIDA",
            "SABOTAJE\n+10% REDUCCIÓN\nDE DOMINIO"
        };
        Rect[] upgradeSymbolCrops =
        {
            new Rect(102f, 316f, 167f, 181f),
            new Rect(405f, 319f, 120f, 175f),
            new Rect(670f, 314f, 175f, 194f),
            new Rect(82f, 669f, 204f, 165f)
        };
        Vector2[] upgradeSymbolCenters =
        {
            new Vector2(117f, 166f), new Vector2(113f, 166f),
            new Vector2(110f, 166f), new Vector2(117f, 167f)
        };
        Vector2[] upgradeSymbolSizes =
        {
            new Vector2(92f, 100f), new Vector2(72f, 105f),
            new Vector2(90f, 100f), new Vector2(105f, 85f)
        };
        resistanceUI.upgradeCardRoots = new GameObject[4];
        resistanceUI.upgradeCardButtons = new Button[4];
        resistanceUI.upgradeLevelTexts = new TMP_Text[4];
        resistanceUI.upgradeCostTexts = new TMP_Text[4];
        resistanceUI.upgradeCostNormalRoots = new GameObject[4];
        resistanceUI.upgradeMaxTexts = new TMP_Text[4];
        for (int upgradeIndex = 0; upgradeIndex < 4; upgradeIndex++)
        {
            GameObject cardRoot = CreateUIObject(
                "ResistanceUpgradeCard" + upgradeIndex, resistanceRoot);
            SetFirstEntryTopLeft(cardRoot.GetComponent<RectTransform>(),
                upgradeCardX[upgradeIndex], 315f,
                upgradeCardW[upgradeIndex], 456f);
            resistanceUI.upgradeCardRoots[upgradeIndex] = cardRoot;

            CreateDefenseText("Title", cardRoot.transform,
                upgradeTitles[upgradeIndex], font,
                2f, 6f, upgradeCardW[upgradeIndex] - 4f, 82f,
                24f, ivory, TextAlignmentOptions.Center);
            resistanceUI.upgradeLevelTexts[upgradeIndex] = CreateDefenseText(
                "Level", cardRoot.transform,
                upgradeIndex < 2 ? "NIVEL 1/3" : "NIVEL 0/3", font,
                10f, 244f, upgradeCardW[upgradeIndex] - 20f, 48f,
                24f, bronze, TextAlignmentOptions.Center);
            TMP_Text effect = CreateDefenseText(
                "Effect", cardRoot.transform, upgradeEffects[upgradeIndex], font,
                8f, 294f, upgradeCardW[upgradeIndex] - 16f, 104f,
                19f, bronze, TextAlignmentOptions.Center);
            effect.lineSpacing = 5f;
            GameObject costRow = CreateUIObject(
                "CostRow", cardRoot.transform);
            SetFirstEntryTopLeft(costRow.GetComponent<RectTransform>(),
                0f, 404f, upgradeCardW[upgradeIndex], 42f);
            resistanceUI.upgradeCostNormalRoots[upgradeIndex] = costRow;
            CreateDefenseText("Label", costRow.transform,
                "PRÓXIMO COSTE", font,
                4f, 0f, upgradeCardW[upgradeIndex] - 78f, 42f,
                16f, bronze, TextAlignmentOptions.Center);
            CreateResistanceIconCrop("FragmentIcon", costRow.transform,
                resistancePactsIconSheet, resistanceChromaKey,
                new Rect(358f, 123f, 65f, 76f),
                upgradeCardW[upgradeIndex] - 70f, 5f, 26f, 32f);
            resistanceUI.upgradeCostTexts[upgradeIndex] = CreateDefenseText(
                "Value", costRow.transform,
                upgradeIndex < 2 ? "6" : "3", font,
                upgradeCardW[upgradeIndex] - 38f, 0f, 34f, 42f,
                17f, bronze, TextAlignmentOptions.Center);
            resistanceUI.upgradeMaxTexts[upgradeIndex] = CreateDefenseText(
                "Maximum", cardRoot.transform, "NIVEL MÁXIMO", font,
                4f, 404f, upgradeCardW[upgradeIndex] - 8f, 42f,
                17f, bronze, TextAlignmentOptions.Center);
            resistanceUI.upgradeMaxTexts[upgradeIndex].gameObject.SetActive(false);
            resistanceUI.upgradeCardButtons[upgradeIndex] =
                CreateMapInvisibleButton("Btn_Purchase", cardRoot.transform,
                    4f, 398f, upgradeCardW[upgradeIndex] - 8f, 54f);

            Rect upgradeSymbol = upgradeSymbolCrops[upgradeIndex];
            Vector2 upgradeSymbolCenter = upgradeSymbolCenters[upgradeIndex];
            Vector2 upgradeSymbolSize = upgradeSymbolSizes[upgradeIndex];
            CreateResistanceIconCrop(
                "UpgradeSymbol", cardRoot.transform,
                resistancePactsInnerSymbols, resistanceChromaKey,
                upgradeSymbol,
                upgradeSymbolCenter.x - upgradeSymbolSize.x * 0.5f,
                upgradeSymbolCenter.y - upgradeSymbolSize.y * 0.5f,
                upgradeSymbolSize.x, upgradeSymbolSize.y);
        }

        CreateDefenseText("ResistancePactsTitle", resistanceRoot,
            "PACTOS DE RESISTENCIA", font,
            220f, 823f, 640f, 58f, 43f, ivory, TextAlignmentOptions.Center);

        float[] pactCardX = { 60f, 388f, 711f };
        float[] pactCardW = { 305f, 300f, 300f };
        string[] pactTitles =
        {
            "REFUGIOS\nOCULTOS", "CAMPANAS\nSILENCIADAS",
            "CUCHILLOS BAJO\nLA MESA"
        };
        Rect[] pactSymbolCrops =
        {
            new Rect(374f, 655f, 182f, 182f),
            new Rect(679f, 658f, 154f, 190f),
            new Rect(379f, 997f, 174f, 176f)
        };
        Vector2[] pactSymbolCenters =
        {
            new Vector2(152.5f, 178f), new Vector2(150f, 178f),
            new Vector2(145f, 178f)
        };
        Vector2[] pactSymbolSizes =
        {
            new Vector2(72f, 72f), new Vector2(62f, 76f),
            new Vector2(70f, 71f)
        };
        Color[] pactColors = { resistanceRed, resistanceTeal, bronze };
        resistanceUI.pactCardRoots = new GameObject[3];
        resistanceUI.pactCardButtons = new Button[3];
        resistanceUI.pactSelectionRoots = new GameObject[3];
        resistanceUI.pactStateTexts = new TMP_Text[3];
        resistanceUI.pactMembersOrRequirementTexts = new TMP_Text[3];
        resistanceUI.pactEffectTexts = new TMP_Text[3];
        resistanceUI.pactWearTexts = new TMP_Text[3];
        resistanceUI.pactActiveMemberIcons = new GameObject[3];
        resistanceUI.pactInactiveLockIcons = new GameObject[3];
        resistanceUI.pactEffectIcons = new GameObject[3];
        resistanceUI.pactWearIcons = new GameObject[3];
        for (int pactIndex = 0; pactIndex < 3; pactIndex++)
        {
            RectTransform selection;
            if (pactIndex == 0)
            {
                GameObject selectionObject = CreateUIObject(
                    "ResistancePactSelection0", resistanceRoot);
                selection = selectionObject.GetComponent<RectTransform>();
                SetFirstEntryTopLeft(selection,
                    pactCardX[pactIndex] - 5f, 875f,
                    pactCardW[pactIndex] + 10f, 488f);
                RawImage selectedPactFidelity = CreateFirstEntryRawImage(
                    "SelectedPactFidelity", selection,
                    resistancePactsFidelityPlate, null,
                    0f, 0f, pactCardW[pactIndex] + 10f, 488f,
                    new Rect(0.0509f, 0.2901f, 0.2917f, 0.2542f));
                selectedPactFidelity.raycastTarget = false;
            }
            else
            {
                Image selectionFill;
                selection = CreateFirstEntryPanel(
                    "ResistancePactSelection" + pactIndex, resistanceRoot,
                    pactCardX[pactIndex] - 5f, 875f,
                    pactCardW[pactIndex] + 10f, 488f,
                    Color.clear, resistanceRed,
                    FirstEntryHex("FF8B60", 145), 3f, 8f, false,
                    out selectionFill);
                selectionFill.raycastTarget = false;
                foreach (Image rail in selection.GetComponentsInChildren<Image>(true))
                {
                    if (rail == selectionFill) continue;
                    Outline glow = Undo.AddComponent<Outline>(rail.gameObject);
                    glow.effectColor = FirstEntryHex("D74331", 75);
                    glow.effectDistance = new Vector2(2f, -2f);
                    glow.useGraphicAlpha = true;
                }
            }
            resistanceUI.pactSelectionRoots[pactIndex] = selection.gameObject;
            selection.gameObject.SetActive(pactIndex == 0);

            GameObject cardRoot = CreateUIObject(
                "ResistancePactCard" + pactIndex, resistanceRoot);
            SetFirstEntryTopLeft(cardRoot.GetComponent<RectTransform>(),
                pactCardX[pactIndex], 880f, pactCardW[pactIndex], 478f);
            resistanceUI.pactCardRoots[pactIndex] = cardRoot;

            CreateDefenseText("Title", cardRoot.transform,
                pactTitles[pactIndex], font,
                8f, 4f, pactCardW[pactIndex] - 16f, 86f,
                26f, pactColors[pactIndex], TextAlignmentOptions.Center);

            RectTransform statePlate;
            if (pactIndex == 0)
            {
                Image stateFill;
                statePlate = CreateFirstEntryPanel(
                    "StatePlate", cardRoot.transform,
                    (pactCardW[pactIndex] - 160f) * 0.5f, 226f,
                    160f, 48f, resistanceDark,
                    pactColors[pactIndex], FirstEntryHex("6D563C", 175),
                    2f, 5f, false, out stateFill);
                stateFill.raycastTarget = false;
            }
            else
            {
                GameObject stateObject = CreateUIObject(
                    "StatePlate", cardRoot.transform);
                statePlate = stateObject.GetComponent<RectTransform>();
                SetFirstEntryTopLeft(statePlate,
                    (pactCardW[pactIndex] - 160f) * 0.5f, 226f,
                    160f, 48f);
            }
            resistanceUI.pactStateTexts[pactIndex] = CreateDefenseText(
                "State", statePlate, pactIndex == 0 ? "ACTIVO" : "INACTIVO", font,
                    pactIndex == 0 ? 7f : (pactIndex == 1 ? 11f : 7f),
                pactIndex == 0 ? 6f : 11f,
                146f, 42f, 23f, pactColors[pactIndex],
                TextAlignmentOptions.Center);

            resistanceUI.pactMembersOrRequirementTexts[pactIndex] =
                CreateDefenseText("MembersOrRequirement", cardRoot.transform,
                    pactIndex == 0 ? "29 MIEMBROS" :
                        (pactIndex == 1 ? "REQUIERE 25\nMIEMBROS" :
                            "REQUIERE 40\nMIEMBROS"),
                    font, 62f, 286f, pactCardW[pactIndex] - 72f, 72f,
                    21f, bronze, TextAlignmentOptions.Center);
            resistanceUI.pactEffectTexts[pactIndex] = CreateDefenseText(
                "Effect", cardRoot.transform,
                pactIndex == 0 ? "−20% PÉRDIDAS\nEN REPRESALIAS" :
                    (pactIndex == 1 ? "−30% DURACIÓN\nDEL DEBILITAMIENTO" :
                        "+20% EFICACIA\nDE SABOTAJE"),
                font, 58f, 361f, pactCardW[pactIndex] - 62f, 74f,
                19f, bronze, TextAlignmentOptions.Center);
            resistanceUI.pactWearTexts[pactIndex] = CreateDefenseText(
                "Wear", cardRoot.transform,
                pactIndex == 0 ? "PRÓXIMO DESGASTE        84 S" : string.Empty,
                font, 8f, 433f, pactCardW[pactIndex] - 16f, 39f,
                16f, pactIndex == 0 ? resistanceRed : bronze,
                TextAlignmentOptions.Center);

            RawImage memberIcon = CreateResistanceIconCrop(
                "ActiveMemberIcon", cardRoot.transform,
                resistancePactsIconSheet, resistanceChromaKey,
                new Rect(84f, 1090f, 55f, 71f),
                48f, 291f, 34f, 44f);
            resistanceUI.pactActiveMemberIcons[pactIndex] = memberIcon.gameObject;
            memberIcon.gameObject.SetActive(pactIndex == 0);
            RawImage lockIcon = CreateResistanceIconCrop(
                "InactiveLockIcon", cardRoot.transform,
                resistancePactsIconSheet, resistanceChromaKey,
                new Rect(pactIndex == 2 ? 661f : 347f, 1090f, 58f, 75f),
                41f, 291f, 36f, 46f);
            resistanceUI.pactInactiveLockIcons[pactIndex] = lockIcon.gameObject;
            lockIcon.gameObject.SetActive(pactIndex != 0);

            Rect effectSource = pactIndex == 0
                ? new Rect(77f, 1164f, 63f, 72f)
                : (pactIndex == 1
                    ? new Rect(342f, 1160f, 74f, 67f)
                    : new Rect(658f, 1161f, 74f, 72f));
            RawImage effectIcon = CreateResistanceIconCrop(
                "EffectIcon", cardRoot.transform,
                resistancePactsIconSheet, resistanceChromaKey,
                effectSource,
                pactIndex == 0 ? 36f : (pactIndex == 1 ? 34f : 38f),
                365f, 40f, 44f);
            resistanceUI.pactEffectIcons[pactIndex] = effectIcon.gameObject;
            RawImage wearIcon = CreateResistanceIconCrop(
                "WearHourglassIcon", cardRoot.transform,
                resistancePactsIconSheet, resistanceChromaKey,
                new Rect(78f, 1234f, 61f, 72f),
                pactCardW[pactIndex] - 94f, 432f, 30f, 36f);
            resistanceUI.pactWearIcons[pactIndex] = wearIcon.gameObject;
            wearIcon.gameObject.SetActive(pactIndex == 0);

            Rect pactSymbol = pactSymbolCrops[pactIndex];
            Vector2 pactSymbolCenter = pactSymbolCenters[pactIndex];
            Vector2 pactSymbolSize = pactSymbolSizes[pactIndex];
            CreateResistanceIconCrop(
                "ResistancePactSymbol" + pactIndex,
                cardRoot.transform, resistancePactsInnerSymbols,
                resistanceChromaKey, pactSymbol,
                pactSymbolCenter.x - pactSymbolSize.x * 0.5f,
                pactSymbolCenter.y - pactSymbolSize.y * 0.5f,
                pactSymbolSize.x, pactSymbolSize.y);

            resistanceUI.pactCardButtons[pactIndex] = CreateMapInvisibleButton(
                "Btn_SelectPact", cardRoot.transform,
                0f, 0f, pactCardW[pactIndex], 428f);
        }

        resistanceUI.pactDetailTitleText = CreateDefenseText(
            "ResistancePactDetailTitle", resistanceRoot,
            "DETALLES DEL PACTO: REFUGIOS OCULTOS", font,
            130f, 1395f, 820f, 51f, 26f, resistanceRed,
            TextAlignmentOptions.Center);
        CreateDefenseText("ResistanceCommittedLabel", resistanceRoot,
            "COMPROMETIDOS", font,
            137f, 1452f, 185f, 35f, 17f, bronze,
            TextAlignmentOptions.Center);
        resistanceUI.pactMembersText = CreateDefenseText(
            "ResistanceCommittedValue", resistanceRoot, "29", font,
            137f, 1483f, 185f, 49f, 30f, ivory,
            TextAlignmentOptions.Center);
        CreateDefenseText("ResistanceWearLabel", resistanceRoot,
            "DESGASTE", font,
            138f, 1530f, 180f, 34f, 17f, bronze,
            TextAlignmentOptions.Center);
        resistanceUI.pactWearDetailText = CreateDefenseText(
            "ResistanceWearValue", resistanceRoot, "1 CADA 120 S", font,
            137f, 1561f, 185f, 45f, 23f, ivory,
            TextAlignmentOptions.Center);
        resistanceUI.pactAdjustRoot = CreateUIObject(
            "ResistanceAdjustRoot", resistanceRoot);
        SetFirstEntryTopLeft(
            resistanceUI.pactAdjustRoot.GetComponent<RectTransform>(),
            0f, 0f, FirstEntryWidth, FirstEntryHeight);
        CreateDefenseText("ResistanceAdjustLabel", resistanceUI.pactAdjustRoot.transform,
            "AJUSTAR COMPROMISO", font,
            377f, 1452f, 332f, 40f, 24f, bronze,
            TextAlignmentOptions.Center);

        resistanceUI.reinforceOneButton = CreateMapInvisibleButton(
            "Btn_ReinforceResistancePactOne", resistanceRoot,
            360f, 1494f, 121f, 97f);
        resistanceUI.reinforceTenButton = CreateMapInvisibleButton(
            "Btn_ReinforceResistancePactTen", resistanceRoot,
            588f, 1494f, 123f, 97f);
        CreateDefenseText("Label", resistanceUI.reinforceOneButton.transform,
            "+1", font, 5f, 8f, 111f, 78f, 36f, ivory,
            TextAlignmentOptions.Center);
        CreateDefenseText("Label", resistanceUI.reinforceTenButton.transform,
            "+10", font, 5f, 8f, 113f, 78f, 36f, ivory,
            TextAlignmentOptions.Center);
        CreateResistanceIconCrop("AdjustMemberIcon", resistanceUI.pactAdjustRoot.transform,
            resistancePactsIconSheet, resistanceChromaKey,
            new Rect(94f, 1104f, 31f, 40f),
            512f, 1508f, 46f, 60f);

        resistanceUI.activateButton = CreateMapInvisibleButton(
            "Btn_ActivateResistancePact", resistanceRoot,
            755f, 1465f, 245f, 130f);
        resistanceUI.cancelButton = CreateMapInvisibleButton(
            "Btn_CancelResistancePact", resistanceRoot,
            755f, 1465f, 245f, 130f);
        CreateDefenseText("Label", resistanceUI.activateButton.transform,
            "ACTIVAR\nPACTO", font, 12f, 16f, 221f, 101f,
            31f, resistanceRed, TextAlignmentOptions.Center);
        CreateDefenseText("Label", resistanceUI.cancelButton.transform,
            "CANCELAR\nPACTO", font, 15f, 5f, 221f, 101f,
            31f, ivory, TextAlignmentOptions.Center);

        CreateResistanceIconCrop("DetailMemberIcon", resistanceRoot,
            resistancePactsIconSheet, resistanceChromaKey,
            new Rect(84f, 1090f, 55f, 71f),
            82f, 1450f, 43f, 56f);
        CreateResistanceIconCrop("DetailWearIcon", resistanceRoot,
            resistancePactsIconSheet, resistanceChromaKey,
            new Rect(78f, 1234f, 61f, 72f),
            81f, 1524f, 43f, 52f);

        CreateDefenseText("ResistancePenaltiesLabel", resistanceRoot,
            "PENALIZACIONES", font,
            315f, 1606f, 450f, 44f, 29f, resistanceRed,
            TextAlignmentOptions.Center);
        TMP_Text noPenaltyIcon = CreateDefenseText(
            "ResistanceNoPenaltyIcon", resistanceRoot,
            "Ø", font, 71f, 1651f, 82f, 68f, 35f, bronze,
            TextAlignmentOptions.Center);
        resistanceUI.noPenaltyIconRoot = noPenaltyIcon.gameObject;
        resistanceUI.penaltiesText = CreateDefenseText(
            "ResistancePenalties", resistanceRoot, "NINGUNA", font,
            151f, 1664f, 780f, 43f, 22f, bronze,
            TextAlignmentOptions.Left);

        float[] resistanceNavX = { 34f, 205f, 373f, 539f, 716f, 886f };
        float[] resistanceNavW = { 168f, 168f, 166f, 177f, 170f, 165f };
        string[] resistanceNavLabels =
        {
            "REGIONES", "OPERACIONES", "DEFENSA",
            "RESISTENCIA", "ALERTA", "CONTENCIÓN"
        };
        for (int navIndex = 0; navIndex < resistanceNavLabels.Length; navIndex++)
        {
            CreateDefenseText("ResistanceNavLabel" + navIndex, resistanceRoot,
                resistanceNavLabels[navIndex], font,
                resistanceNavX[navIndex], 1847f,
                resistanceNavW[navIndex], 43f,
                navIndex == 3 ? 20f : 17f,
                navIndex == 3 ? resistanceRed : bronze,
                TextAlignmentOptions.Center);
        }
        civilization2UI.resistanceSectionRoot.SetActive(false);

        civilization2UI.alertSectionRoot = CreateView(
            "D2_Civ2_AlertSection", root.transform
        );
        Transform alertRoot = civilization2UI.alertSectionRoot.transform;
        D2AlertPanelUI alertUI = Undo.AddComponent<D2AlertPanelUI>(
            civilization2UI.alertSectionRoot
        );
        civilization2UI.alertPanelUI = alertUI;
        RawImage alertBase = CreateFirstEntryRawImage(
            "ResistanceAlertBasePlate", alertRoot, alertBasePlate, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight, new Rect(0f, 0f, 1f, 1f));
        alertBase.raycastTarget = false;

        Color alertRed = FirstEntryHex("E55339");
        Color alertTeal = FirstEntryHex("63B3A6");
        Color alertMuted = FirstEntryHex("A58D72");

        Image alertNavSelectionFill;
        RectTransform alertNavSelection = CreateFirstEntryPanel(
            "AlertNavSelection", alertRoot,
            700f, 1768f, 169f, 150f,
            FirstEntryHex("7B1D13", 52), alertRed,
            FirstEntryHex("FF9A64", 125), 3f, 6f, false,
            out alertNavSelectionFill);
        alertNavSelectionFill.raycastTarget = false;
        foreach (Image selectionRail in alertNavSelection.GetComponentsInChildren<Image>(true))
        {
            if (selectionRail == alertNavSelectionFill) continue;
            Outline glow = Undo.AddComponent<Outline>(selectionRail.gameObject);
            glow.effectColor = FirstEntryHex("D74331", 75);
            glow.effectDistance = new Vector2(2f, -2f);
            glow.useGraphicAlpha = true;
        }

        CreateResistanceReferenceText("ResistanceAlertTitle", alertRoot,
            "ALERTA DEL ENTE", font,
            220f, 28f, 505f, 62f, 40f, ivory, TextAlignmentOptions.Center);
        alertUI.stateText = CreateResistanceReferenceText(
            "AlertState", alertRoot, "EL ENTE HA ENTRADO EN ALERTA", font,
            171f, 124f, 700f, 62f, 32f, alertRed, TextAlignmentOptions.Center);

        CreateResistanceReferenceText("AlertDominanceLabel", alertRoot,
            "DOMINIO TOTAL", font,
            116f, 265f, 282f, 50f, 26f, bronze, TextAlignmentOptions.Center);
        alertUI.dominanceText = CreateResistanceReferenceText(
            "AlertDominance", alertRoot, "30%", font,
            177f, 309f, 170f, 66f, 43f, alertRed, TextAlignmentOptions.Center);
        CreateResistanceReferenceText("AlertDominanceThreshold", alertRoot,
            "SE ACTIVA PERMANENTEMENTE\nAL LLEGAR A 30%", font,
            67f, 688f, 366f, 56f, 18f, bronze, TextAlignmentOptions.Center);

        GameObject dominanceGaugeObject = CreateUIObject("AlertDominanceGauge", alertRoot);
        SetFirstEntryTopLeft(dominanceGaugeObject.GetComponent<RectTransform>(),
            35f, 257f, 490f, 550f);
        alertUI.dominanceGauge = Undo.AddComponent<D2SegmentedGaugeGraphic>(
            dominanceGaugeObject);
        alertUI.dominanceGauge.color = FirstEntryHex("E64F35", 220);
        alertUI.dominanceGauge.raycastTarget = false;
        alertUI.dominanceGauge.Configure(30, 0.405f, 0.49f, 2.6f, 140f, false);
        alertUI.dominanceGauge.SetProgress(0.30f);

        CreateResistanceReferenceText("AlertNextMarkLabel", alertRoot,
            "PRÓXIMA MARCA DEL ENTE", font,
            493f, 228f, 385f, 45f, 24f, bronze, TextAlignmentOptions.Center);
        alertUI.timerText = CreateResistanceReferenceText(
            "AlertTimer", alertRoot, "06:18", font,
            579f, 271f, 215f, 68f, 48f, alertRed, TextAlignmentOptions.Center);
        CreateResistanceReferenceText("AlertMarksLabel", alertRoot,
            "MARCAS REALIZADAS", font,
            536f, 381f, 300f, 42f, 23f, bronze, TextAlignmentOptions.Center);
        alertUI.marksCountText = CreateResistanceReferenceText(
            "AlertMarksCount", alertRoot, "2", font,
            620f, 421f, 130f, 65f, 42f, alertRed, TextAlignmentOptions.Center);

        CreateResistanceReferenceText("AlertEffectsLabel", alertRoot,
            "EFECTOS DE ALERTA", font,
            530f, 510f, 330f, 44f, 25f, alertRed, TextAlignmentOptions.Center);
        alertUI.effectsText = CreateResistanceReferenceText(
            "AlertEffects", alertRoot,
            "RESCATE, ESPIONAJE Y\nSABOTAJE GENERAN\n+50% AMENAZA", font,
            588f, 554f, 276f, 91f, 18f, bronze, TextAlignmentOptions.Left);
        alertUI.effectsText.textWrappingMode = TextWrappingModes.Normal;
        alertUI.effectsText.lineSpacing = -5f;
        alertUI.fragmentsEffectText = CreateResistanceReferenceText(
            "AlertFragmentsEffect", alertRoot,
            "CADA REPRESALIA\nENTREGA 6 FRAGMENTOS", font,
            588f, 658f, 276f, 70f, 18f, bronze, TextAlignmentOptions.Left);
        alertUI.fragmentsEffectText.textWrappingMode = TextWrappingModes.Normal;
        alertUI.fragmentsEffectText.lineSpacing = -4f;
        CreateResistanceIconCrop(
            "AlertThreatEffectIcon", alertRoot,
            alertIconSheet, resistanceChromaKey,
            new Rect(523f, 426f, 83f, 91f),
            575f, 620f, 83f, 96f);
        CreateResistanceIconCrop(
            "AlertFragmentEffectIcon", alertRoot,
            alertIconSheet, resistanceChromaKey,
            new Rect(527f, 519f, 78f, 94f),
            590f, 738f, 72f, 88f);

        alertUI.regionsText = CreateResistanceReferenceText(
            "AlertRegions", alertRoot, "ESTADO DE LAS REGIONES", font,
            248f, 766f, 445f, 49f, 27f, bronze, TextAlignmentOptions.Center);
        alertUI.regionNameTexts = new TMP_Text[3];
        alertUI.regionMarkTexts = new TMP_Text[3];
        alertUI.regionProtectionTexts = new TMP_Text[3];
        alertUI.regionProtectionActiveIcons = new GameObject[3];
        alertUI.regionProtectionInactiveIcons = new GameObject[3];
        float[] alertRowY = { 848f, 980f, 1111f };
        float[] alertShieldSourceY = { 840f, 969f, 1098f };
        for (int regionIndex = 0; regionIndex < 3; regionIndex++)
        {
            alertUI.regionNameTexts[regionIndex] = CreateResistanceReferenceText(
                "AlertRegionName" + regionIndex, alertRoot,
                "REGIÓN " + (regionIndex + 1), font,
                201f, alertRowY[regionIndex], 174f, 50f, 31f,
                bronze, TextAlignmentOptions.Left);
            alertUI.regionMarkTexts[regionIndex] = CreateResistanceReferenceText(
                "AlertRegionMark" + regionIndex, alertRoot,
                regionIndex == 0
                    ? "MARCADA\n<size=68%>(+3% PRÓXIMA REPRESALIA)</size>"
                    : "SIN MARCA", font,
                397f, alertRowY[regionIndex] - 3f, 257f, 64f,
                regionIndex == 0 ? 22f : 22f,
                regionIndex == 0 ? alertRed : alertMuted,
                TextAlignmentOptions.Center);
            alertUI.regionMarkTexts[regionIndex].textWrappingMode = TextWrappingModes.Normal;
            alertUI.regionMarkTexts[regionIndex].lineSpacing = -6f;
            alertUI.regionProtectionTexts[regionIndex] = CreateResistanceReferenceText(
                "AlertRegionProtection" + regionIndex, alertRoot,
                "PROTECCIÓN\n" + (regionIndex == 0 ? "ACTIVA" : "INACTIVA"), font,
                660f, alertRowY[regionIndex] - 5f, 150f, 67f, 16f,
                regionIndex == 0 ? alertTeal : alertMuted,
                TextAlignmentOptions.Center);
            alertUI.regionProtectionTexts[regionIndex].textWrappingMode = TextWrappingModes.Normal;
            alertUI.regionProtectionTexts[regionIndex].lineSpacing = -5f;

            float shieldX = 918f;
            float shieldY = alertShieldSourceY[regionIndex] / 1672f * FirstEntryHeight;
            RawImage activeShield = CreateResistanceIconCrop(
                "AlertProtectionActive" + regionIndex, alertRoot,
                alertIconSheet, resistanceChromaKey,
                new Rect(799f, 837f, 67f, 80f),
                shieldX, shieldY, 77f, 92f);
            RawImage inactiveShield = CreateResistanceIconCrop(
                "AlertProtectionInactive" + regionIndex, alertRoot,
                alertIconSheet, resistanceChromaKey,
                new Rect(799f, 966f, 67f, 80f),
                shieldX, shieldY, 77f, 92f);
            alertUI.regionProtectionActiveIcons[regionIndex] = activeShield.gameObject;
            alertUI.regionProtectionInactiveIcons[regionIndex] = inactiveShield.gameObject;
        }

        alertUI.unlocksText = CreateResistanceReferenceText(
            "AlertUnlocks", alertRoot, "CIVILIZACIÓN 3 · NUEVA", font,
            220f, 1221f, 505f, 51f, 31f, bronze, TextAlignmentOptions.Center);
        alertUI.unlockDetailText = CreateResistanceReferenceText(
            "AlertUnlockDetail", alertRoot,
            "CIVILIZACIÓN 3 Y CONTENCIÓN\nDESBLOQUEADAS AL LLEGAR A 30% DE DOMINIO.", font,
            195f, 1275f, 550f, 66f, 18f, bronze, TextAlignmentOptions.Center);
        alertUI.unlockDetailText.textWrappingMode = TextWrappingModes.Normal;
        alertUI.unlockDetailText.lineSpacing = -5f;

        CreateResistanceReferenceText("AlertLastResultLabel", alertRoot,
            "ÚLTIMO RESULTADO", font,
            298f, 1372f, 348f, 48f, 29f, alertRed, TextAlignmentOptions.Center);
        alertUI.lastResultText = CreateResistanceReferenceText(
            "AlertLastResult", alertRoot, "REGIÓN 1 FUE MARCADA", font,
            222f, 1421f, 595f, 58f, 32f, alertRed, TextAlignmentOptions.Center);
        alertUI.lastResultDetailText = CreateResistanceReferenceText(
            "AlertLastResultDetail", alertRoot,
            "LA PRÓXIMA REPRESALIA AÑADE 3% DE PÉRDIDAS", font,
            217f, 1475f, 605f, 45f, 18f, bronze, TextAlignmentOptions.Center);

        Rect[] alertIconRects =
        {
            new Rect(31f, 24f, 83f, 74f),
            new Rect(61f, 112f, 83f, 80f),
            new Rect(129f, 367f, 261f, 256f),
            new Rect(78f, 814f, 119f, 119f),
            new Rect(78f, 943f, 119f, 119f),
            new Rect(78f, 1073f, 119f, 119f),
            new Rect(78f, 1201f, 140f, 142f),
            new Rect(729f, 1207f, 144f, 139f),
            new Rect(91f, 1373f, 121f, 151f),
            new Rect(65f, 1538f, 86f, 86f),
            new Rect(207f, 1537f, 92f, 87f),
            new Rect(363f, 1538f, 80f, 86f),
            new Rect(510f, 1537f, 75f, 87f),
            new Rect(636f, 1537f, 87f, 87f),
            new Rect(795f, 1538f, 74f, 85f)
        };
        string[] alertIconNames =
        {
            "AlertBackIcon", "AlertWarningIcon", "AlertEntityIcon",
            "AlertRegionMedallion1", "AlertRegionMedallion2", "AlertRegionMedallion3",
            "AlertCivilization3Medallion", "AlertCivilization3Compass",
            "AlertLastResultIcon", "AlertNavRegionsIcon", "AlertNavOperationsIcon",
            "AlertNavDefenseIcon", "AlertNavResistanceIcon", "AlertNavAlertIcon",
            "AlertNavContainmentIcon"
        };
        for (int iconIndex = 0; iconIndex < alertIconRects.Length; iconIndex++)
            CreateResistanceMappedIconCrop(alertIconNames[iconIndex], alertRoot,
                alertIconSheet, resistanceChromaKey, alertIconRects[iconIndex]);

        float[] alertNavLabelX = { 44f, 183f, 333f, 477f, 611f, 753f };
        float[] alertNavLabelW = { 139f, 150f, 144f, 134f, 142f, 142f };
        string[] alertNavLabels =
        {
            "REGIONES", "OPERACIONES", "DEFENSA",
            "RESISTENCIA", "ALERTA", "CONTENCIÓN"
        };
        for (int navIndex = 0; navIndex < alertNavLabels.Length; navIndex++)
        {
            CreateResistanceReferenceText("AlertNavLabel" + navIndex, alertRoot,
                alertNavLabels[navIndex], font,
                alertNavLabelX[navIndex], 1621f, alertNavLabelW[navIndex], 36f,
                navIndex == 4 ? 17f : 15f,
                navIndex == 4 ? alertRed : bronze,
                TextAlignmentOptions.Center);
        }
        civilization2UI.alertSectionRoot.SetActive(false);

        civilization2UI.containmentSectionRoot = CreateView(
            "D2_Civ2_ContainmentSection", root.transform
        );
        Transform containmentRoot = civilization2UI.containmentSectionRoot.transform;
        D2ContainmentPanelUI containmentUI = Undo.AddComponent<D2ContainmentPanelUI>(
            civilization2UI.containmentSectionRoot
        );
        civilization2UI.containmentPanelUI = containmentUI;
        containmentUI.containmentAttemptRoot = CreateView(
            "D2_Civ2_ContainmentAttempt", containmentRoot);
        containmentUI.majorPactRoot = CreateView(
            "D2_Civ2_MajorPact", containmentRoot);
        Transform containmentAttemptRoot = containmentUI.containmentAttemptRoot.transform;
        Transform majorPactRoot = containmentUI.majorPactRoot.transform;
        BuildResistanceMajorPactVisual(containmentUI, majorPactRoot,
            majorPactBasePlate, majorPactLineSymbols, resistanceChromaKey,
            font, ivory, bronze);
        containmentUI.majorPactRoot.SetActive(false);
        RawImage containmentBase = CreateFirstEntryRawImage(
            "ResistanceContainmentBasePlate", containmentAttemptRoot,
            containmentBasePlate, null, 0f, 0f, FirstEntryWidth, FirstEntryHeight,
            new Rect(0f, 0f, 1f, 1f));
        containmentBase.raycastTarget = false;

        Color containmentRed = FirstEntryHex("E55339");
        Color containmentGreen = FirstEntryHex("789A72");
        Color containmentMuted = FirstEntryHex("AA9273");

        Image containmentNavSelectionFill;
        RectTransform containmentNavSelection = CreateFirstEntryPanel(
            "ContainmentNavSelection", containmentAttemptRoot,
            884f, 1768f, 169f, 150f,
            FirstEntryHex("7B1D13", 52), containmentRed,
            FirstEntryHex("FF9A64", 125), 3f, 6f, false,
            out containmentNavSelectionFill);
        containmentNavSelectionFill.raycastTarget = false;
        foreach (Image selectionRail in
                 containmentNavSelection.GetComponentsInChildren<Image>(true))
        {
            if (selectionRail == containmentNavSelectionFill) continue;
            Outline glow = Undo.AddComponent<Outline>(selectionRail.gameObject);
            glow.effectColor = FirstEntryHex("D74331", 75);
            glow.effectDistance = new Vector2(2f, -2f);
            glow.useGraphicAlpha = true;
        }

        CreateResistanceReferenceText("ResistanceContainmentTitle", containmentAttemptRoot,
            "CONTENCIÓN", font,
            270f, 27f, 405f, 67f, 42f, ivory, TextAlignmentOptions.Center);
        CreateResistanceReferenceText("ContainmentAvailableLabel", containmentAttemptRoot,
            "MIEMBROS DISPONIBLES", font,
            102f, 118f, 188f, 29f, 13f, bronze, TextAlignmentOptions.Left);
        containmentUI.availableMembersText = CreateResistanceReferenceText(
            "ContainmentAvailableValue", containmentAttemptRoot, "3,845", font,
            102f, 145f, 188f, 42f, 30f, ivory, TextAlignmentOptions.Center);
        CreateResistanceReferenceText("ContainmentDominanceHeaderLabel", containmentAttemptRoot,
            "DOMINIO TOTAL", font,
            379f, 118f, 154f, 29f, 14f, bronze, TextAlignmentOptions.Left);
        containmentUI.dominanceHeaderText = CreateResistanceReferenceText(
            "ContainmentDominanceHeaderValue", containmentAttemptRoot, "15%", font,
            379f, 145f, 154f, 42f, 30f, ivory, TextAlignmentOptions.Center);
        CreateResistanceReferenceText("ContainmentAttemptsHeaderLabel", containmentAttemptRoot,
            "INTENTOS", font,
            594f, 118f, 119f, 29f, 14f, bronze, TextAlignmentOptions.Left);
        containmentUI.attemptsHeaderText = CreateResistanceReferenceText(
            "ContainmentAttemptsHeaderValue", containmentAttemptRoot, "2", font,
            594f, 145f, 119f, 42f, 30f, ivory, TextAlignmentOptions.Center);
        CreateResistanceReferenceText("ContainmentFailuresHeaderLabel", containmentAttemptRoot,
            "FALLOS", font,
            802f, 118f, 108f, 29f, 14f, bronze, TextAlignmentOptions.Left);
        containmentUI.failuresHeaderText = CreateResistanceReferenceText(
            "ContainmentFailuresHeaderValue", containmentAttemptRoot, "2", font,
            802f, 145f, 108f, 42f, 30f, ivory, TextAlignmentOptions.Center);

        containmentUI.stateText = CreateResistanceReferenceText(
            "ContainmentState", containmentAttemptRoot, "CONTENCIÓN\nDISPONIBLE", font,
            322f, 574f, 300f, 112f, 31f, bronze, TextAlignmentOptions.Center);
        containmentUI.stateText.textWrappingMode = TextWrappingModes.Normal;
        containmentUI.stateText.lineSpacing = -5f;

        CreateResistanceReferenceText("ContainmentProbabilityLabel", containmentAttemptRoot,
            "PROBABILIDAD DE ÉXITO", font,
            35f, 777f, 325f, 43f, 23f, bronze, TextAlignmentOptions.Center);
        containmentUI.probabilityText = CreateResistanceReferenceText(
            "ContainmentProbability", containmentAttemptRoot, "57.5%", font,
            68f, 824f, 260f, 76f, 47f, ivory, TextAlignmentOptions.Center);
        containmentUI.probabilityDetailText = CreateResistanceReferenceText(
            "ContainmentProbabilityDetail", containmentAttemptRoot,
            "PROBABILIDAD FAVORABLE\nSEGÚN EL ESTADO ACTUAL.", font,
            61f, 917f, 276f, 69f, 17f, bronze, TextAlignmentOptions.Center);
        containmentUI.probabilityDetailText.textWrappingMode = TextWrappingModes.Normal;
        containmentUI.probabilityDetailText.lineSpacing = -5f;

        CreateResistanceReferenceText("ContainmentDominanceCardLabel", containmentAttemptRoot,
            "DOMINIO TOTAL", font,
            381f, 777f, 228f, 43f, 23f, bronze, TextAlignmentOptions.Center);
        containmentUI.dominanceCardText = CreateResistanceReferenceText(
            "ContainmentDominanceCardValue", containmentAttemptRoot, "15%", font,
            405f, 816f, 180f, 76f, 47f, ivory, TextAlignmentOptions.Center);
        containmentUI.cooldownText = CreateResistanceReferenceText(
            "ContainmentCooldown", containmentAttemptRoot, "SIN COOLDOWN", font,
            653f, 789f, 245f, 48f, 23f, bronze, TextAlignmentOptions.Center);
        containmentUI.cooldownText.textWrappingMode = TextWrappingModes.Normal;

        containmentUI.rulesText = CreateText(
            "ContainmentRulesData", containmentAttemptRoot, "", 1f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f),
            new Vector2(0.001f, 0.001f));
        containmentUI.rulesText.gameObject.SetActive(false);
        containmentUI.failureRuleText = CreateResistanceReferenceText(
            "ContainmentFailureRule", containmentAttemptRoot,
            "<color=#E55339>FALLO</color> · +20% AMENAZA Y -5% DE MIEMBROS\n" +
            "REGIONALES SIN PROTECCIÓN.", font,
            179f, 1014f, 708f, 78f, 22f, bronze, TextAlignmentOptions.Left);
        containmentUI.failureRuleText.textWrappingMode = TextWrappingModes.Normal;
        containmentUI.failureRuleText.lineSpacing = -5f;
        containmentUI.successRuleText = CreateResistanceReferenceText(
            "ContainmentSuccessRule", containmentAttemptRoot,
            "<color=#789A72>ÉXITO</color> · CESAN LAS MARCAS Y SE PREPARA\n" +
            "EL PACTO MAYOR.", font,
            179f, 1106f, 708f, 78f, 22f, bronze, TextAlignmentOptions.Left);
        containmentUI.successRuleText.textWrappingMode = TextWrappingModes.Normal;
        containmentUI.successRuleText.lineSpacing = -5f;

        CreateResistanceReferenceText("ContainmentSustainTitle", containmentAttemptRoot,
            "SOSTENIMIENTO", font,
            348f, 1202f, 248f, 46f, 25f, bronze, TextAlignmentOptions.Center);
        containmentUI.assignedMembersText = CreateResistanceReferenceText(
            "ContainmentAssignedMembers", containmentAttemptRoot, "ASIGNADOS  0", font,
            105f, 1237f, 238f, 48f, 22f, bronze, TextAlignmentOptions.Left);
        containmentUI.assignmentAvailableText = CreateResistanceReferenceText(
            "ContainmentAssignmentAvailable", containmentAttemptRoot,
            "DISPONIBLES  3,845", font,
            617f, 1237f, 260f, 48f, 22f, bronze, TextAlignmentOptions.Left);

        float[] containmentButtonSourceX = { 51f, 219f, 386f, 558f, 729f };
        float[] containmentButtonSourceW = { 162f, 160f, 160f, 160f, 160f };
        string[] containmentButtonLabels = { "−1", "+1", "+10", "TODOS", "LIBERAR" };
        for (int buttonIndex = 0; buttonIndex < containmentButtonLabels.Length; buttonIndex++)
        {
            float buttonX = containmentButtonSourceX[buttonIndex] / 941f * FirstEntryWidth;
            float buttonY = 1286f / 1672f * FirstEntryHeight;
            float buttonW = containmentButtonSourceW[buttonIndex] / 941f * FirstEntryWidth;
            float buttonH = 67f / 1672f * FirstEntryHeight;
            Button disabledButton = CreateMapInvisibleButton(
                "Btn_ContainmentPreview" + buttonIndex, containmentAttemptRoot,
                buttonX, buttonY, buttonW, buttonH);
            disabledButton.interactable = false;
            CreateDefenseText("Label", disabledButton.transform,
                containmentButtonLabels[buttonIndex], font,
                5f, -1f, buttonW - 10f, buttonH - 14f,
                buttonIndex < 3 ? 31f : 23f,
                containmentMuted, TextAlignmentOptions.Center);
        }

        containmentUI.attemptButton = CreateMapInvisibleButton(
            "Btn_AttemptContainment", containmentAttemptRoot,
            204f, 1668f, 672f, 103f);
        CreateDefenseText("Label", containmentUI.attemptButton.transform,
            "INTENTAR CONTENCIÓN", font,
            15f, 11f, 642f, 80f, 38f, ivory, TextAlignmentOptions.Center);
        containmentUI.attemptsText = CreateText(
            "ContainmentAttemptsData", containmentAttemptRoot, "2", 1f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f),
            new Vector2(0.001f, 0.001f));
        containmentUI.attemptsText.gameObject.SetActive(false);
        containmentUI.lastResultText = CreateResistanceReferenceText(
            "ContainmentLastResult", containmentAttemptRoot,
            "EL INTENTO ANTERIOR FALLÓ · NUEVO INTENTO DISPONIBLE", font,
            125f, 1380f, 770f, 55f, 21f, containmentRed,
            TextAlignmentOptions.Center);

        float[] containmentNavLabelX = { 22f, 169f, 321f, 470f, 618f, 767f };
        float[] containmentNavLabelW = { 147f, 152f, 149f, 148f, 149f, 162f };
        string[] containmentNavLabels =
        {
            "REGIONES", "OPERACIONES", "DEFENSA",
            "RESISTENCIA", "ALERTA", "CONTENCIÓN"
        };
        for (int navIndex = 0; navIndex < containmentNavLabels.Length; navIndex++)
        {
            CreateResistanceReferenceText("ContainmentNavLabel" + navIndex,
                containmentAttemptRoot, containmentNavLabels[navIndex], font,
                containmentNavLabelX[navIndex], 1621f,
                containmentNavLabelW[navIndex], 38f,
                navIndex == 5 ? 17f : 15f,
                navIndex == 5 ? containmentRed : bronze,
                TextAlignmentOptions.Center);
        }
        civilization2UI.containmentSectionRoot.SetActive(false);

        EditorUtility.SetDirty(civilization2UI);
        EditorUtility.SetDirty(operationsUI);
        EditorUtility.SetDirty(reprisalsUI);
        EditorUtility.SetDirty(resistanceUI);
        EditorUtility.SetDirty(alertUI);
        EditorUtility.SetDirty(containmentUI);
    }

    private static void BuildResistanceMajorPactVisual(
        D2ContainmentPanelUI ui,
        Transform root,
        Texture2D basePlate,
        Texture2D lineSymbols,
        Material chromaKey,
        TMP_FontAsset font,
        Color ivory,
        Color bronze)
    {
        RawImage plate = CreateFirstEntryRawImage(
            "ResistanceMajorPactBasePlate", root, basePlate, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight,
            new Rect(0f, 0f, 1f, 1f));
        plate.raycastTarget = false;

        Color gold = FirstEntryHex("E3B56B");
        Color green = FirstEntryHex("83B56E");
        Color red = FirstEntryHex("C84D38");
        float scaleX = FirstEntryWidth / 941f;
        float scaleY = FirstEntryHeight / 1672f;

        TextMeshProUGUI majorPactTitle = CreateResistanceReferenceText(
            "MajorPactTitle", root,
            "Pacto Mayor de Civilización 2", font,
            160f, 27f, 620f, 63f, 31f, ivory, TextAlignmentOptions.Center);
        TuneResistanceReferenceText(majorPactTitle, 0.84f, 1.20f, 0f, 4f);
        ui.majorPactStateText = CreateResistanceReferenceText(
            "MajorPactState", root,
            "Ente Contenido · Pacto Mayor Establecido", font,
            70f, 95f, 800f, 54f, 28f, ivory, TextAlignmentOptions.Center);
        ui.majorPactMilestoneText = CreateResistanceReferenceText(
            "MajorPactMilestone", root,
            "Hito Reconocido para Cerrar Dimensión 2.", font,
            210f, 150f, 520f, 38f, 19f, bronze, TextAlignmentOptions.Center);

        CreateResistanceReferenceText("MajorPactAvailableLabel", root,
            "MIEMBROS\nDISPONIBLES", font,
            112f, 247f, 125f, 54f, 16f, bronze, TextAlignmentOptions.Left);
        ui.majorPactAvailableMembersText = CreateResistanceReferenceText(
            "MajorPactAvailableValue", root, "3,820", font,
            112f, 294f, 125f, 42f, 30f, ivory, TextAlignmentOptions.Center);
        CreateResistanceReferenceText("MajorPactAssignedHeaderLabel", root,
            "ASIGNADOS AL\nSOSTENIMIENTO", font,
            325f, 247f, 150f, 54f, 15f, bronze, TextAlignmentOptions.Left);
        ui.majorPactAssignedHeaderText = CreateResistanceReferenceText(
            "MajorPactAssignedHeaderValue", root, "25", font,
            327f, 294f, 110f, 42f, 30f, ivory, TextAlignmentOptions.Center);
        CreateResistanceReferenceText("MajorPactStabilityLabel", root,
            "ESTABILIDAD", font,
            558f, 257f, 125f, 34f, 16f, bronze, TextAlignmentOptions.Left);
        ui.stabilityText = CreateResistanceReferenceText(
            "MajorPactStabilityValue", root, "54", font,
            552f, 294f, 115f, 42f, 30f, ivory, TextAlignmentOptions.Center);
        CreateResistanceReferenceText("MajorPactFragmentsLabel", root,
            "FRAGMENTOS DE\nCONTROL", font,
            762f, 247f, 150f, 54f, 15f, bronze, TextAlignmentOptions.Left);
        ui.majorPactFragmentsHeaderText = CreateResistanceReferenceText(
            "MajorPactFragmentsValue", root, "78", font,
            763f, 294f, 110f, 42f, 30f, ivory, TextAlignmentOptions.Center);

        ui.majorPactLineDropdown = CreateDropdown(
            "Civ2MajorPactLineSelectorData", root,
            new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        CanvasGroup hiddenDropdown = Undo.AddComponent<CanvasGroup>(
            ui.majorPactLineDropdown.gameObject);
        hiddenDropdown.alpha = 0f;
        hiddenDropdown.interactable = false;
        hiddenDropdown.blocksRaycasts = false;

        string[] lineNames =
        {
            "Red Reconstituida", "Levantamiento Coordinado",
            "Defensa Vinculada", "Custodia de Artefactos",
            "Geometría de Resistencia"
        };
        Rect[] symbolCrops =
        {
            new Rect(346f, 120f, 246f, 270f),
            new Rect(350f, 420f, 240f, 230f),
            new Rect(365f, 690f, 205f, 230f),
            new Rect(355f, 955f, 235f, 235f),
            new Rect(345f, 1235f, 250f, 235f)
        };
        ui.majorPactLineButtons = new Button[5];
        ui.majorPactLineLevelTexts = new TMP_Text[5];
        ui.majorPactLineSelections = new GameObject[5];
        ui.majorPactCentralIcons = new GameObject[5];
        ui.majorPactDetailIcons = new GameObject[5];
        for (int lineIndex = 0; lineIndex < 5; lineIndex++)
        {
            float lineY = 354f + lineIndex * 101f;
            Image selectionFill = CreateFirstEntryImage(
                "MajorPactLineSelection" + lineIndex, root, null,
                328f * scaleX, lineY * scaleY,
                574f * scaleX, 96f * scaleY,
                FirstEntryHex("C38A3C", 8), false);
            selectionFill.raycastTarget = false;
            ui.majorPactLineSelections[lineIndex] = selectionFill.gameObject;
            selectionFill.gameObject.SetActive(lineIndex == 0);

            CreateResistanceSourceIconCrop(
                "MajorPactRowIcon" + lineIndex, root, lineSymbols, chromaKey,
                symbolCrops[lineIndex],
                new Rect(352f, lineY + 15f, 79f, 79f));
            TextMeshProUGUI lineName = CreateResistanceReferenceText(
                "MajorPactLineName" + lineIndex, root, lineNames[lineIndex], font,
                452f, lineY + 19f, 340f, 53f,
                lineIndex == 1 ? 19f : lineIndex >= 3 ? 22f : 26f,
                bronze, TextAlignmentOptions.Left);
            TuneResistanceReferenceText(lineName, 1f, 1.12f, 0f, 14f);
            ui.majorPactLineLevelTexts[lineIndex] = CreateResistanceReferenceText(
                "MajorPactLineLevel" + lineIndex, root,
                lineIndex == 0 ? "NIVEL 1/3" : "NIVEL 0/3", font,
                794f, lineY + 23f, 92f, 43f, 16f, bronze,
                TextAlignmentOptions.Center);
            TuneResistanceReferenceText(
                ui.majorPactLineLevelTexts[lineIndex], 0.84f, 1.12f, 0f, 12f);
            ui.majorPactLineButtons[lineIndex] = CreateMapInvisibleButton(
                "Btn_MajorPactLine" + lineIndex, root,
                328f * scaleX, lineY * scaleY,
                574f * scaleX, 96f * scaleY);

            RawImage centralIcon = CreateResistanceSourceIconCrop(
                "MajorPactCentralIcon" + lineIndex, root, lineSymbols, chromaKey,
                symbolCrops[lineIndex], new Rect(175f, 545f, 115f, 115f));
            ui.majorPactCentralIcons[lineIndex] = centralIcon.gameObject;
            centralIcon.gameObject.SetActive(lineIndex == 0);
            RawImage detailIcon = CreateResistanceSourceIconCrop(
                "MajorPactDetailIcon" + lineIndex, root, lineSymbols, chromaKey,
                symbolCrops[lineIndex], new Rect(111f, 914f, 112f, 112f));
            ui.majorPactDetailIcons[lineIndex] = detailIcon.gameObject;
            detailIcon.gameObject.SetActive(lineIndex == 0);
        }

        CreateFirstEntryRail("MajorPactDetailDividerTop", root,
            300f * scaleX, 947f * scaleY, 575f * scaleX, 2f * scaleY,
            FirstEntryHex("9C6A39", 95));
        CreateFirstEntryRail("MajorPactDetailDividerMiddle", root,
            300f * scaleX, 1014f * scaleY, 575f * scaleX, 2f * scaleY,
            FirstEntryHex("9C6A39", 95));
        CreateResistanceSourceIconCrop(
            "MajorPactDetailStabilityIcon", root, basePlate, null,
            new Rect(495f, 251f, 55f, 66f), new Rect(480f, 1019f, 30f, 36f));
        CreateResistanceSourceIconCrop(
            "MajorPactDetailFragmentsIcon", root, basePlate, null,
            new Rect(702f, 249f, 53f, 69f), new Rect(690f, 1016f, 31f, 40f));
        CreateResistanceReferenceText(
            "MajorPactDetailEffectArrow", root, "↑", font,
            305f, 956f, 38f, 43f, 25f, bronze, TextAlignmentOptions.Center);
        CreateResistanceReferenceText(
            "MajorPactDetailCostArrow", root, "→", font,
            305f, 1021f, 38f, 43f, 24f, bronze, TextAlignmentOptions.Center);

        ui.majorPactLineText = CreateResistanceReferenceText(
            "MajorPactDetailTitle", root,
            "Red Reconstituida — Nivel 1 / 3", font,
            280f, 897f, 590f, 58f, 29f, ivory, TextAlignmentOptions.Left);
        TuneResistanceReferenceText(ui.majorPactLineText, 0.92f, 1.12f);
        ui.majorPactLineText.enableAutoSizing = true;
        ui.majorPactLineText.fontSizeMin = 20f * scaleY;
        ui.majorPactLineText.fontSizeMax = 29f * scaleY;
        ui.majorPactDetailEffectText = CreateResistanceReferenceText(
            "MajorPactDetailEffect", root,
            "+5% Generación de Miembros por Nivel", font,
            351f, 970f, 520f, 43f, 20f, green, TextAlignmentOptions.Left);
        ui.majorPactDetailEffectText.enableAutoSizing = true;
        ui.majorPactDetailEffectText.fontSizeMin = 13f * scaleY;
        ui.majorPactDetailEffectText.fontSizeMax = 20f * scaleY;
        ui.majorPactDetailCostText = CreateResistanceReferenceText(
            "MajorPactDetailCost", root,
            "Siguiente · 40 Estabilidad + 6 Fragmentos", font,
            351f, 1035f, 525f, 42f, 17f, bronze, TextAlignmentOptions.Left);
        ui.majorPactDetailCostText.enableAutoSizing = true;
        ui.majorPactDetailCostText.fontSizeMin = 15f * scaleY;
        ui.majorPactDetailCostText.fontSizeMax = 17f * scaleY;

        Image upgradePlateFill;
        CreateFirstEntryPanel("MajorPactUpgradePlate", root,
            300f * scaleX, 1084f * scaleY, 340f * scaleX, 69f * scaleY,
            FirstEntryHex("5F2018", 210), FirstEntryHex("B45A39"),
            FirstEntryHex("E8A866", 140), 3f, 6f, false, out upgradePlateFill);
        ui.establishMajorPactButton = CreateMapInvisibleButton(
            "Btn_EstablishCiv2MajorPact", root,
            300f * scaleX, 1084f * scaleY, 340f * scaleX, 69f * scaleY);
        CreateDefenseText("Label", ui.establishMajorPactButton.transform,
            "ESTABLECER PACTO", font,
            8f, 3f, 324f * scaleX, 60f * scaleY,
            28f * scaleY, ivory, TextAlignmentOptions.Center);
        ui.upgradeMajorPactLineButton = CreateMapInvisibleButton(
            "Btn_UpgradeCiv2MajorPactLine", root,
            300f * scaleX, 1084f * scaleY, 340f * scaleX, 69f * scaleY);
        CreateDefenseText("Label", ui.upgradeMajorPactLineButton.transform,
            "MEJORAR LÍNEA", font,
            8f, 3f, 324f * scaleX, 60f * scaleY,
            28f * scaleY, ivory, TextAlignmentOptions.Center);

        CreateResistanceReferenceText("MajorPactAssignedLabel", root,
            "Miembros Asignados", font,
            298f, 1186f, 345f, 43f, 26f, bronze, TextAlignmentOptions.Center);
        CreateResistanceSourceIconCrop(
            "MajorPactAssignedPersonIcon", root, basePlate, null,
            new Rect(68f, 252f, 43f, 63f), new Rect(303f, 1185f, 24f, 35f));
        ui.majorPactAssignedValueText = CreateResistanceReferenceText(
            "MajorPactAssignedValue", root, "25", font,
            420f, 1223f, 100f, 58f, 42f, ivory, TextAlignmentOptions.Center);
        ui.assignmentText = ui.majorPactAssignedValueText;

        float[] buttonX = { 62f, 218f, 371f, 523f, 696f };
        float[] buttonW = { 145f, 140f, 140f, 160f, 179f };
        string[] buttonLabels = { "−1", "+1", "+10", "Todos", "Liberar" };
        Button[] assignmentButtons = new Button[5];
        for (int buttonIndex = 0; buttonIndex < 5; buttonIndex++)
        {
            float width = buttonW[buttonIndex] * scaleX;
            assignmentButtons[buttonIndex] = CreateMapInvisibleButton(
                "Btn_MajorPactAssignment" + buttonIndex, root,
                buttonX[buttonIndex] * scaleX, 1271f * scaleY,
                width, 67f * scaleY);
            CreateDefenseText("Label", assignmentButtons[buttonIndex].transform,
                buttonLabels[buttonIndex], font,
                4f, -1f, width - 8f, 63f * scaleY,
                (buttonIndex < 3 ? 28f : 22f) * scaleY,
                buttonIndex == 4 ? red : bronze, TextAlignmentOptions.Center);
        }
        ui.releaseOneButton = assignmentButtons[0];
        ui.assignOneButton = assignmentButtons[1];
        ui.assignTenButton = assignmentButtons[2];
        ui.assignAllButton = assignmentButtons[3];
        ui.releaseAllButton = assignmentButtons[4];

        ui.majorPactFooterTitleText = CreateResistanceReferenceText(
            "MajorPactFooterTitle", root, "Pacto Mayor Establecido", font,
            210f, 1385f, 610f, 54f, 33f, ivory, TextAlignmentOptions.Center);
        TuneResistanceReferenceText(
            ui.majorPactFooterTitleText, 0.92f, 1.18f, -6f, 0f);
        ui.majorPactLastResultText = CreateResistanceReferenceText(
            "MajorPactFooterBody", root,
            "LA CONTENCIÓN YA GENERA ESTABILIDAD.", font,
            215f, 1440f, 600f, 40f, 22f, bronze, TextAlignmentOptions.Center);
        TuneResistanceReferenceText(
            ui.majorPactLastResultText, 0.86f, 1.10f);

        float[] navX = { 27f, 176f, 323f, 469f, 616f, 765f };
        float[] navW = { 149f, 147f, 146f, 147f, 149f, 151f };
        string[] navLabels =
        {
            "REGIONES", "OPERACIONES", "DEFENSA",
            "RESISTENCIA", "ALERTA", "PACTO MAYOR"
        };
        for (int navIndex = 0; navIndex < 6; navIndex++)
        {
            CreateResistanceReferenceText(
                "MajorPactNavLabel" + navIndex, root, navLabels[navIndex], font,
                navX[navIndex], 1603f, navW[navIndex], 42f,
                navIndex == 5 ? 17.5f : 17f,
                navIndex == 5 ? red : bronze, TextAlignmentOptions.Center);
        }
    }

    private static void BuildCivilization3(Dimension2PanelUI panel)
    {
        PrepareFirstEntryTexture(ArchaeologyCleanBasePlatePath, 2048);
        PrepareFirstEntryTexture(AnalysisCleanBasePlatePath, 2048);
        PrepareFirstEntryTexture(ArchiveCleanBasePlatePath, 2048);
        PrepareFirstEntryTexture(ArchiveLockIconPath, 2048);
        PrepareFirstEntryTexture(EntityResearchCleanBasePlatePath, 2048);
        PrepareFirstEntryTexture(EntityPactCleanBasePlatePath, 2048);
        Texture2D archaeologyBasePlate = AssetDatabase.LoadAssetAtPath<Texture2D>(
            ArchaeologyCleanBasePlatePath);
        Texture2D analysisBasePlate = AssetDatabase.LoadAssetAtPath<Texture2D>(
            AnalysisCleanBasePlatePath);
        Texture2D archiveBasePlate = AssetDatabase.LoadAssetAtPath<Texture2D>(
            ArchiveCleanBasePlatePath);
        Texture2D archiveLockIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(
            ArchiveLockIconPath);
        Texture2D entityResearchBasePlate = AssetDatabase.LoadAssetAtPath<Texture2D>(
            EntityResearchCleanBasePlatePath);
        Texture2D entityPactBasePlate = AssetDatabase.LoadAssetAtPath<Texture2D>(
            EntityPactCleanBasePlatePath);
        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
            FirstEntryFontPath);

        GameObject root = CreateView("D2_Civilization3", panel.transform);
        Canvas archaeologyCanvas = Undo.AddComponent<Canvas>(root);
        archaeologyCanvas.overrideSorting = true;
        archaeologyCanvas.sortingOrder = 32744;
        SerializedObject archaeologyCanvasSettings = new SerializedObject(archaeologyCanvas);
        archaeologyCanvasSettings.FindProperty("m_OverrideSorting").boolValue = true;
        archaeologyCanvasSettings.FindProperty("m_SortingOrder").intValue = 32744;
        archaeologyCanvasSettings.ApplyModifiedPropertiesWithoutUndo();
        Undo.AddComponent<GraphicRaycaster>(root);
        panel.civilization3Root = root;
        D2Civilization3PanelUI civilization3UI = Undo.AddComponent<D2Civilization3PanelUI>(root);
        panel.civilization3PanelUI = civilization3UI;
        civilization3UI.dimension2PanelUI = panel;
        civilization3UI.archaeologySectionRoot = CreateView(
            "D2_Civ3_ArchaeologySection", root.transform
        );
        Transform archaeologyRoot = civilization3UI.archaeologySectionRoot.transform;
        RectTransform archaeologyRect = archaeologyRoot as RectTransform;
        archaeologyRect.anchoredPosition = new Vector2(-24f, 32f);
        RawImage archaeologyBase = CreateFirstEntryRawImage(
            "RuinsArchaeologyBasePlate", archaeologyRoot,
            archaeologyBasePlate, null, 0f, 0f, FirstEntryWidth, FirstEntryHeight,
            new Rect(0f, 0f, 1f, 1f));
        archaeologyBase.raycastTarget = false;

        Color ivory = FirstEntryHex("D8C09A");
        Color bronze = FirstEntryHex("B18A5C");
        Color teal = FirstEntryHex("65C5C5");
        Color purple = FirstEntryHex("B286C1");
        float sourceScaleX = FirstEntryWidth / 941f;
        float sourceScaleY = FirstEntryHeight / 1672f;

        civilization3UI.zoneSelectionOverlays = new GameObject[3];
        float[] selectionY = { 193f, 435f, 665f };
        float[] selectionH = { 226f, 216f, 225f };
        for (int zoneIndex = 0; zoneIndex < 3; zoneIndex++)
        {
            Image selectionFill;
            RectTransform selection = CreateFirstEntryPanel(
                "RuinsZoneSelection" + (zoneIndex + 1), archaeologyRoot,
                20f * sourceScaleX, selectionY[zoneIndex] * sourceScaleY,
                825f * sourceScaleX, selectionH[zoneIndex] * sourceScaleY,
                FirstEntryHex("42214D", 18), purple,
                FirstEntryHex("D79BE4", 115), 3f, 6f, false,
                out selectionFill);
            selectionFill.raycastTarget = false;
            civilization3UI.zoneSelectionOverlays[zoneIndex] = selection.gameObject;
            selection.gameObject.SetActive(false);
        }

        for (int segment = 0; segment < 11; segment++)
        {
            CreateFirstEntryRail(
                "RuinsProgressRail" + segment, archaeologyRoot,
                864f * sourceScaleX,
                (321f + segment * 39f) * sourceScaleY,
                5f * sourceScaleX, 19f * sourceScaleY,
                FirstEntryHex("58BFC1", 205));
        }
        CreateFirstEntryDiamond(
            "RuinsProgressStart", archaeologyRoot,
            868f * sourceScaleX, 302f * sourceScaleY,
            29f * sourceScaleX, FirstEntryHex("65D6D5"),
            FirstEntryHex("1F6D72"));
        civilization3UI.zoneRailMarkers = new GameObject[3];
        float[] markerY = { 302f, 538f, 770f };
        for (int zoneIndex = 0; zoneIndex < 3; zoneIndex++)
        {
            GameObject markerRoot = CreateFirstEntryDiamond(
                "RuinsSelectedMarker" + (zoneIndex + 1), archaeologyRoot,
                868f * sourceScaleX, markerY[zoneIndex] * sourceScaleY,
                35f * sourceScaleX, FirstEntryHex("C17BDB"),
                FirstEntryHex("6D2E87"));
            civilization3UI.zoneRailMarkers[zoneIndex] = markerRoot;
            markerRoot.SetActive(false);
        }

        civilization3UI.excavationActionHighlight = CreateFirstEntryImage(
            "RuinsExcavationActionHighlight", archaeologyRoot, null,
            104f * sourceScaleX, 1387f * sourceScaleY,
            727f * sourceScaleX, 82f * sourceScaleY,
            FirstEntryHex("5C2871", 82), false);
        civilization3UI.excavationActionHighlight.raycastTarget = false;

        CreateResistanceSourceIconCrop(
            "RuinsDetailSpiral", archaeologyRoot, archaeologyBasePlate, null,
            new Rect(43f, 111f, 82f, 72f),
            new Rect(55f, 919f, 87f, 74f));

        CreateResistanceReferenceText(
            "RuinsTitle", archaeologyRoot, "RUINAS SEPULTADAS", font,
            153f, 25f, 635f, 70f, 43f, ivory, TextAlignmentOptions.Center);
        civilization3UI.ancientKnowledgeHeaderText = CreateResistanceReferenceText(
            "RuinsAncientKnowledge", archaeologyRoot,
            "CONOCIMIENTO ANTIGUO", font,
            136f, 118f, 225f, 27f, 13f, bronze, TextAlignmentOptions.Left);
        civilization3UI.ancientKnowledgeHeaderText.enableAutoSizing = true;
        civilization3UI.ancientKnowledgeHeaderText.fontSizeMin =
            11.5f / 1672f * FirstEntryHeight;
        civilization3UI.ancientKnowledgeHeaderValueText = CreateResistanceReferenceText(
            "RuinsAncientKnowledgeValue", archaeologyRoot, "78", font,
            136f, 145f, 100f, 35f, 24f, bronze, TextAlignmentOptions.Left);
        civilization3UI.archiveHeaderText = CreateResistanceReferenceText(
            "RuinsArchiveHeader", archaeologyRoot, "ARCHIVO IV", font,
            446f, 126f, 180f, 45f, 18.5f, bronze, TextAlignmentOptions.Center);
        civilization3UI.entityKnowledgeHeaderText = CreateResistanceReferenceText(
            "RuinsEntityKnowledge", archaeologyRoot,
            "CONOCIMIENTO DEL ENTE", font,
            683f, 118f, 220f, 27f, 12f, bronze, TextAlignmentOptions.Left);
        civilization3UI.entityKnowledgeHeaderText.enableAutoSizing = true;
        civilization3UI.entityKnowledgeHeaderText.fontSizeMin =
                10.5f / 1672f * FirstEntryHeight;
        civilization3UI.entityKnowledgeHeaderValueText = CreateResistanceReferenceText(
            "RuinsEntityKnowledgeValue", archaeologyRoot, "3 / 6", font,
            683f, 145f, 110f, 35f, 23f, bronze, TextAlignmentOptions.Left);

        civilization3UI.zoneNumberTexts = new TMP_Text[3];
        civilization3UI.zoneNameTexts = new TMP_Text[3];
        civilization3UI.zoneResearchValueTexts = new TMP_Text[3];
        civilization3UI.zoneResourceNameTexts = new TMP_Text[3];
        civilization3UI.zoneResourceValueTexts = new TMP_Text[3];
        civilization3UI.zoneResearchFillImages = new Image[3];
        float[] zoneTitleY = { 218f, 458f, 691f };
        float[] zoneInvestigationY = { 280f, 519f, 750f };
        float[] zoneBarY = { 317f, 555f, 786f };
        float[] zoneResourceY = { 359f, 596f, 827f };
        string[] zoneNames =
        {
            "ZONA 1 · ENTRADA SEPULTADA",
            "ZONA 2 · GALERÍA DE INSCRIPCIONES",
            "ZONA 3 · SANTUARIO SELLADO"
        };
        string[] zoneResources =
        {
            "FRAGMENTOS BASE", "INSCRIPCIONES PARCIALES", "SELLOS ANTIGUOS"
        };
        string[] zonePercentages = { "65%", "60%", "46%" };
        string[] zoneAmounts = { "120", "84", "36" };
        for (int zoneIndex = 0; zoneIndex < 3; zoneIndex++)
        {
            float cardTextColorY = zoneTitleY[zoneIndex];
            civilization3UI.zoneNumberTexts[zoneIndex] = CreateResistanceReferenceText(
                "RuinsZoneNumber" + (zoneIndex + 1), archaeologyRoot,
                (zoneIndex + 1).ToString(), font,
                51f, cardTextColorY + 8f, 72f, 62f, 39f,
                zoneIndex == 2 ? purple : bronze, TextAlignmentOptions.Center);
            civilization3UI.zoneNameTexts[zoneIndex] = CreateResistanceReferenceText(
                "RuinsZoneName" + (zoneIndex + 1), archaeologyRoot,
                zoneNames[zoneIndex], font,
                397f, cardTextColorY, 435f, 46f,
                zoneIndex == 1 ? 20f : 27f,
                zoneIndex == 2 ? purple : bronze, TextAlignmentOptions.Left);
            civilization3UI.zoneNameTexts[zoneIndex].enableAutoSizing = true;
            civilization3UI.zoneNameTexts[zoneIndex].fontSizeMin =
                20f / 1672f * FirstEntryHeight;
            CreateResistanceReferenceText(
                "RuinsInvestigationLabel" + (zoneIndex + 1), archaeologyRoot,
                "INVESTIGACIÓN", font,
                443f, zoneInvestigationY[zoneIndex], 260f, 38f, 22f,
                bronze, TextAlignmentOptions.Left);
            Image progressFill = CreateFirstEntryImage(
                "RuinsResearchFill" + (zoneIndex + 1), archaeologyRoot, null,
                445f * sourceScaleX, zoneBarY[zoneIndex] * sourceScaleY,
                289f * sourceScaleX, 17f * sourceScaleY,
                zoneIndex == 2 ? purple : teal, false);
            progressFill.type = Image.Type.Simple;
            progressFill.fillAmount = zoneIndex == 0 ? 0.65f :
                (zoneIndex == 1 ? 0.60f : 0.46f);
            civilization3UI.zoneResearchFillImages[zoneIndex] = progressFill;
            civilization3UI.zoneResearchValueTexts[zoneIndex] =
                CreateResistanceReferenceText(
                    "RuinsResearchValue" + (zoneIndex + 1), archaeologyRoot,
                    zonePercentages[zoneIndex], font,
                    748f, zoneBarY[zoneIndex] - 12f, 70f, 48f, 30f,
                    bronze, TextAlignmentOptions.Right);
            civilization3UI.zoneResourceNameTexts[zoneIndex] =
                CreateResistanceReferenceText(
                    "RuinsResourceName" + (zoneIndex + 1), archaeologyRoot,
                    zoneResources[zoneIndex], font,
                    443f, zoneResourceY[zoneIndex], 310f, 43f,
                    zoneIndex == 1 ? 18f : 22f,
                    bronze, TextAlignmentOptions.Left);
            civilization3UI.zoneResourceValueTexts[zoneIndex] =
                CreateResistanceReferenceText(
                    "RuinsResourceValue" + (zoneIndex + 1), archaeologyRoot,
                    zoneAmounts[zoneIndex], font,
                    754f, zoneResourceY[zoneIndex] - 3f, 62f, 48f, 30f,
                    bronze, TextAlignmentOptions.Right);
        }

        civilization3UI.detailTitleText = CreateResistanceReferenceText(
            "RuinsDetailTitle", archaeologyRoot, "SANTUARIO SELLADO", font,
            151f, 917f, 692f, 61f, 37f, purple, TextAlignmentOptions.Left);
        civilization3UI.detailExcavationText = CreateResistanceReferenceText(
            "RuinsDetailExcavation", archaeologyRoot,
            "EXCAVACIÓN DISPONIBLE · DURACIÓN 00:30", font,
            168f, 1009f, 690f, 52f, 27f, bronze, TextAlignmentOptions.Left);
        CreateResistanceReferenceText(
            "RuinsRemainsLabel", archaeologyRoot, "RESTOS", font,
            168f, 1090f, 150f, 45f, 27f, bronze, TextAlignmentOptions.Left);
        CreateResistanceReferenceText(
            "RuinsLowLabel", archaeologyRoot, "BAJA", font,
            351f, 1086f, 120f, 35f, 24f, FirstEntryHex("9EBD68"),
            TextAlignmentOptions.Center);
        CreateResistanceReferenceText(
            "RuinsMediumLabel", archaeologyRoot, "MEDIA", font,
            541f, 1086f, 120f, 35f, 24f, FirstEntryHex("70B8CA"),
            TextAlignmentOptions.Center);
        CreateResistanceReferenceText(
            "RuinsHighLabel", archaeologyRoot, "ALTA", font,
            731f, 1086f, 120f, 35f, 24f, purple,
            TextAlignmentOptions.Center);
        civilization3UI.remainsLowText = CreateResistanceReferenceText(
            "RuinsLowValue", archaeologyRoot, "12", font,
            401f, 1120f, 62f, 40f, 25f, ivory, TextAlignmentOptions.Center);
        civilization3UI.remainsMediumText = CreateResistanceReferenceText(
            "RuinsMediumValue", archaeologyRoot, "5", font,
            590f, 1120f, 62f, 40f, 25f, ivory, TextAlignmentOptions.Center);
        civilization3UI.remainsHighText = CreateResistanceReferenceText(
            "RuinsHighValue", archaeologyRoot, "2", font,
            778f, 1120f, 62f, 40f, 25f, ivory, TextAlignmentOptions.Center);
        civilization3UI.detailAnalysisText = CreateResistanceReferenceText(
            "RuinsDetailAnalysis", archaeologyRoot,
            "ANÁLISIS DISPONIBLE · ERUDITO DE SELLOS NIVEL 1 / 3", font,
            168f, 1184f, 690f, 55f, 25f, bronze, TextAlignmentOptions.Left);
        civilization3UI.detailAnalysisText.enableAutoSizing = true;
        civilization3UI.detailAnalysisText.fontSizeMin =
            19f / 1672f * FirstEntryHeight;
        civilization3UI.clueTitleText = CreateResistanceReferenceText(
            "RuinsClueTitle", archaeologyRoot,
            "INDICIOS ANÓMALOS PROFUNDOS", font,
            169f, 1267f, 665f, 46f, 25f, bronze, TextAlignmentOptions.Left);
        civilization3UI.clueTitleText.enableAutoSizing = true;
        civilization3UI.clueTitleText.fontSizeMin =
            20f / 1672f * FirstEntryHeight;
        civilization3UI.cluePatternText = CreateResistanceReferenceText(
            "RuinsCluePattern", archaeologyRoot,
            "PATRÓN        2 / 12", font,
            169f, 1310f, 305f, 50f, 24f, purple, TextAlignmentOptions.Left);
        civilization3UI.clueAccumulationText = CreateResistanceReferenceText(
            "RuinsClueAccumulation", archaeologyRoot,
            "ACUMULACIÓN        38%", font,
            544f, 1310f, 300f, 50f, 21f, purple, TextAlignmentOptions.Left);
        civilization3UI.excavationActionText = CreateResistanceReferenceText(
            "RuinsExcavationAction", archaeologyRoot,
            "INICIAR EXCAVACIÓN", font,
            132f, 1392f, 696f, 79f, 41f, ivory, TextAlignmentOptions.Center);

        float[] ruinsNavX = { 20f, 245f, 470f, 696f };
        float[] ruinsNavW = { 225f, 225f, 226f, 225f };
        string[] ruinsNavLabels = { "EXCAVAR", "ANALIZAR", "ARCHIVO", "ENTE" };
        for (int navIndex = 0; navIndex < ruinsNavLabels.Length; navIndex++)
        {
            CreateResistanceReferenceText(
                "RuinsNavLabel" + navIndex, archaeologyRoot,
                ruinsNavLabels[navIndex], font,
                ruinsNavX[navIndex], 1587f, ruinsNavW[navIndex], 46f,
                24f, navIndex == 0 ? ivory : bronze,
                TextAlignmentOptions.Center);
        }

        civilization3UI.backToMapButton = CreateMapInvisibleButton(
            "Btn_Civ3BackToMap", archaeologyRoot,
            27f, 24f, 112f, 88f);
        civilization3UI.zone1Button = CreateMapInvisibleButton(
            "Btn_Civ3Zone1", archaeologyRoot,
            21f * sourceScaleX, 193f * sourceScaleY,
            823f * sourceScaleX, 226f * sourceScaleY);
        civilization3UI.zone2Button = CreateMapInvisibleButton(
            "Btn_Civ3Zone2", archaeologyRoot,
            21f * sourceScaleX, 435f * sourceScaleY,
            823f * sourceScaleX, 216f * sourceScaleY);
        civilization3UI.zone3Button = CreateMapInvisibleButton(
            "Btn_Civ3Zone3", archaeologyRoot,
            21f * sourceScaleX, 665f * sourceScaleY,
            823f * sourceScaleX, 225f * sourceScaleY);
        civilization3UI.excavateButton = CreateMapInvisibleButton(
            "Btn_Civ3Excavate", archaeologyRoot,
            95f * sourceScaleX, 1378f * sourceScaleY,
            744f * sourceScaleX, 108f * sourceScaleY);
        civilization3UI.showAnalysisButton = CreateMapInvisibleButton(
            "Btn_Civ3AnalyzePreview", archaeologyRoot,
            245f * sourceScaleX, 1508f * sourceScaleY,
            225f * sourceScaleX, 145f * sourceScaleY);
        civilization3UI.showArchiveButton = CreateMapInvisibleButton(
            "Btn_Civ3ShowArchive", archaeologyRoot,
            470f * sourceScaleX, 1508f * sourceScaleY,
            226f * sourceScaleX, 145f * sourceScaleY);
        civilization3UI.showEntityResearchButton = CreateMapInvisibleButton(
            "Btn_Civ3ShowEntityResearch", archaeologyRoot,
            696f * sourceScaleX, 1508f * sourceScaleY,
            225f * sourceScaleX, 145f * sourceScaleY);

        BuildCivilization3AnalysisView(
            civilization3UI, root.transform, analysisBasePlate, font);

        GameObject legacyDataRoot = CreateView(
            "RuinsLegacyDataBindings", archaeologyRoot);
        Transform legacy = legacyDataRoot.transform;
        civilization3UI.zoneText = CreateText("Civ3ZoneData", legacy, "", 1f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        civilization3UI.lockedZonesText = CreateText("Civ3LockedZonesData", legacy, "", 1f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        civilization3UI.excavationText = CreateText("Civ3ExcavationData", legacy, "", 1f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        civilization3UI.excavationSlider = CreateProgressSlider("Civ3ExcavationProgress", legacy,
            new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        civilization3UI.inventoryText = CreateText("Civ3InventoryData", legacy, "", 1f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        civilization3UI.analysisText = CreateText("Civ3AnalysisData", legacy, "", 1f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        civilization3UI.analysisSlider = CreateProgressSlider("Civ3AnalysisProgress", legacy,
            new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        civilization3UI.researchText = CreateText("Civ3ResearchData", legacy, "", 1f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        civilization3UI.archiveText = CreateText("Civ3ArchiveData", legacy, "", 1f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        civilization3UI.cluesText = CreateText("Civ3CluesData", legacy, "", 1f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        civilization3UI.anomalyText = CreateText("Civ3AnomalyData", legacy, "", 1f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        civilization3UI.scholarText = CreateText("Civ3ScholarData", legacy, "", 1f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        civilization3UI.civilization1ResourcesText = CreateText("Civ3Civilization1Data", legacy, "", 1f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        civilization3UI.lastResultText = CreateText("Civ3LastResultData", legacy, "", 1f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        civilization3UI.objectiveText = civilization3UI.lastResultText;
        civilization3UI.unlockZone2Button = CreateButton("Btn_Civ3UnlockZone2", legacy, "", new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        civilization3UI.unlockZone3Button = CreateButton("Btn_Civ3UnlockZone3", legacy, "", new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        civilization3UI.analyzeLowButton = CreateButton("Btn_Civ3AnalyzeLow", legacy, "", new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        civilization3UI.analyzeMediumButton = CreateButton("Btn_Civ3AnalyzeMedium", legacy, "", new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        civilization3UI.analyzeHighButton = CreateButton("Btn_Civ3AnalyzeHigh", legacy, "", new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        CreateButton("Btn_Civ3HireScholarLegacy", legacy, "", new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        civilization3UI.readAnomalyButton = CreateButton("Btn_Civ3ReadAnomaly", legacy, "", new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        legacyDataRoot.SetActive(false);

        D2ArchivePanelUI archiveUI = BuildCivilization3ArchiveView(
            civilization3UI, root.transform, archiveBasePlate,
            archiveLockIcon, font);

        D2EntityResearchPanelUI entityResearchUI =
            BuildCivilization3EntityResearchView(
                civilization3UI, root.transform, entityResearchBasePlate,
                entityPactBasePlate, font);

#if false // Sustituido por la composición V5 construida arriba.
        civilization3UI.entityResearchSectionRoot = CreateView(
            "D2_Civ3_EntityResearchSection", root.transform
        );
        D2EntityResearchPanelUI entityResearchUI = Undo.AddComponent<D2EntityResearchPanelUI>(
            civilization3UI.entityResearchSectionRoot
        );
        civilization3UI.entityResearchPanelUI = entityResearchUI;
        entityResearchUI.civilization3PanelUI = civilization3UI;
        Transform entityRoot = civilization3UI.entityResearchSectionRoot.transform;
        CreateText(
            "Civ3EntityResearchTitle", entityRoot,
            "INVESTIGACIÓN DEL ENTE", 30f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.92f),
            new Vector2(0.82f, 0.07f)
        );
        entityResearchUI.unlockText = CreateText(
            "Civ3EntityResearchUnlock", entityRoot,
            "BLOQUEADA — requiere los tres Datos Anómalos", 20f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.82f),
            new Vector2(0.86f, 0.06f)
        );
        entityResearchUI.statusText = CreateText(
            "Civ3EntityResearchStatus", entityRoot,
            "PAUSADA", 22f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.735f),
            new Vector2(0.72f, 0.055f)
        );
        entityResearchUI.progressText = CreateText(
            "Civ3EntityResearchProgress", entityRoot,
            "PROGRESO: 0% | Conocimiento Antiguo: 0", 21f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.66f),
            new Vector2(0.82f, 0.055f)
        );
        entityResearchUI.progressSlider = CreateProgressSlider(
            "Civ3EntityResearchSlider", entityRoot,
            new Vector2(0.5f, 0.615f), new Vector2(0.56f, 0.025f)
        );
        entityResearchUI.startPauseButton = CreateButton(
            "Btn_Civ3EntityResearchStart", entityRoot, "INICIAR INVESTIGACIÓN",
            new Vector2(0.5f, 0.545f), new Vector2(0.28f, 0.06f)
        );
        entityResearchUI.resonantExpeditionButton = CreateButton(
            "Btn_Civ3PactResonantExpedition", entityRoot, "EXPEDICION",
            new Vector2(0.14f, 0.495f), new Vector2(0.16f, 0.045f)
        );
        entityResearchUI.endlessArchiveButton = CreateButton(
            "Btn_Civ3PactEndlessArchive", entityRoot, "ARCHIVO",
            new Vector2(0.32f, 0.495f), new Vector2(0.16f, 0.045f)
        );
        entityResearchUI.sharedMemoryButton = CreateButton(
            "Btn_Civ3PactSharedMemory", entityRoot, "MEMORIA",
            new Vector2(0.5f, 0.495f), new Vector2(0.16f, 0.045f)
        );
        entityResearchUI.modulatorResonanceButton = CreateButton(
            "Btn_Civ3PactModulatorResonance", entityRoot, "MODULADOR",
            new Vector2(0.68f, 0.495f), new Vector2(0.16f, 0.045f)
        );
        entityResearchUI.firstThresholdChronicleButton = CreateButton(
            "Btn_Civ3PactFirstThresholdChronicle", entityRoot, "UMBRAL P1",
            new Vector2(0.86f, 0.495f), new Vector2(0.16f, 0.045f)
        );
        entityResearchUI.resonantExpeditionButton.gameObject.SetActive(false);
        entityResearchUI.endlessArchiveButton.gameObject.SetActive(false);
        entityResearchUI.sharedMemoryButton.gameObject.SetActive(false);
        entityResearchUI.modulatorResonanceButton.gameObject.SetActive(false);
        entityResearchUI.firstThresholdChronicleButton.gameObject.SetActive(false);
        entityResearchUI.milestoneText = CreateText(
            "Civ3EntityResearchMilestone", entityRoot,
            "PRÓXIMO HITO 30%", 19f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.455f),
            new Vector2(0.9f, 0.075f)
        );
        entityResearchUI.completeMilestoneButton = CreateButton(
            "Btn_Civ3EntityResearchMilestone", entityRoot, "COMPLETAR HITO",
            new Vector2(0.5f, 0.385f), new Vector2(0.24f, 0.055f)
        );
        entityResearchUI.resourcesText = CreateText(
            "Civ3EntityResearchResources", entityRoot,
            "RECURSOS Y DATOS", 18f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.295f),
            new Vector2(0.9f, 0.09f)
        );
        entityResearchUI.entityKnowledgeText = CreateText(
            "Civ3EntityKnowledge", entityRoot,
            "CONOCIMIENTO DEL ENTE: 0/6", 20f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.205f),
            new Vector2(0.84f, 0.055f)
        );
        entityResearchUI.lastResultText = CreateText(
            "Civ3EntityResearchLastResult", entityRoot,
            "La investigación todavía no ha comenzado.", 17f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.135f),
            new Vector2(0.68f, 0.06f)
        );
        entityResearchUI.backToArchaeologyButton = CreateButton(
            "Btn_Civ3BackToArchaeology", entityRoot, "VOLVER A ARQUEOLOGÍA",
            new Vector2(0.2f, 0.055f), new Vector2(0.28f, 0.055f)
        );
        entityResearchUI.backToMapButton = CreateButton(
            "Btn_Civ3EntityBackToMap", entityRoot, "VOLVER AL MAPA",
            new Vector2(0.8f, 0.055f), new Vector2(0.22f, 0.055f)
        );
        civilization3UI.entityResearchSectionRoot.SetActive(false);
#endif

        EditorUtility.SetDirty(civilization3UI);
        EditorUtility.SetDirty(archiveUI);
        EditorUtility.SetDirty(entityResearchUI);
    }

    private static D2EntityResearchPanelUI BuildCivilization3EntityResearchView(
        D2Civilization3PanelUI civilization3UI,
        Transform parent,
        Texture2D basePlate,
        Texture2D pactBasePlate,
        TMP_FontAsset font)
    {
        civilization3UI.entityResearchSectionRoot = CreateView(
            "D2_Civ3_EntityResearchSection", parent);
        Transform container = civilization3UI.entityResearchSectionRoot.transform;
        RectTransform containerRect = container as RectTransform;
        containerRect.anchoredPosition = new Vector2(-24f, 32f);

        D2EntityResearchPanelUI ui = Undo.AddComponent<D2EntityResearchPanelUI>(
            civilization3UI.entityResearchSectionRoot);
        civilization3UI.entityResearchPanelUI = ui;
        ui.civilization3PanelUI = civilization3UI;
        ui.researchPhaseVisualRoot = CreateView(
            "EntityResearchPhaseV5", container);
        Transform root = ui.researchPhaseVisualRoot.transform;
        ui.pactPhaseVisualRoot = CreateView(
            "EntityPactPhaseV5", container);
        Transform pactRoot = ui.pactPhaseVisualRoot.transform;
        GameObject legacyBindings = CreateView(
            "EntityLegacyBindings", container);
        Transform legacy = legacyBindings.transform;

        RawImage plate = CreateFirstEntryRawImage(
            "EntityResearchBasePlate", root, basePlate, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight,
            new Rect(0f, 0f, 1f, 1f));
        plate.raycastTarget = false;

        Color ivory = FirstEntryHex("D8C09A");
        Color bronze = FirstEntryHex("B18A5C");
        Color teal = FirstEntryHex("65C5C5");
        Color purple = FirstEntryHex("B286C1");
        Color disabled = FirstEntryHex("5C5650");
        float sx = FirstEntryWidth / 941f;
        float sy = FirstEntryHeight / 1672f;

        TMP_Text title = CreateResistanceReferenceText(
            "EntityResearchTitle", root, "INVESTIGACIÓN DEL ENTE", font,
            160f, 20f, 651f, 62f, 42f, ivory, TextAlignmentOptions.Center);
        CompressReferenceTextHorizontally(title, 0.85f);
        ui.unlockText = CreateResistanceReferenceText(
            "EntityResearchUnlockTitle", root,
            "INVESTIGACIÓN DESBLOQUEADA", font,
            235f, 106f, 590f, 42f, 31f, purple, TextAlignmentOptions.Left);
        CompressReferenceTextHorizontally(ui.unlockText, 0.74f);
        ui.unlockDetailText = CreateResistanceReferenceText(
            "EntityResearchUnlockDetail", root,
            "LOS TRES DATOS ANÓMALOS HAN TRAZADO UN PATRÓN", font,
            235f, 144f, 610f, 37f, 23f, bronze, TextAlignmentOptions.Left);
        CompressReferenceTextHorizontally(ui.unlockDetailText, 0.70f);

        GameObject gaugeObject = CreateUIObject("EntityResearchProgressArc", root);
        SetFirstEntryTopLeft(gaugeObject.GetComponent<RectTransform>(),
            103f * sx, 194f * sy, 353f * sx, 326f * sy);
        ui.progressRadialGraphic = Undo.AddComponent<D2SegmentedGaugeGraphic>(gaugeObject);
        ui.progressRadialGraphic.color = FirstEntryHex("65C5C5", 215);
        ui.progressRadialGraphic.raycastTarget = false;
        ui.progressRadialGraphic.Configure(100, 0.462f, 0.49f, 0.35f, 180f, false);
        ui.progressRadialGraphic.SetProgress(0.24f);
        ui.progressGaugeText = CreateResistanceReferenceText(
            "EntityResearchGaugeValue", root, "24%", font,
            210f, 481f, 132f, 47f, 32f, teal, TextAlignmentOptions.Center);

        ui.statusText = CreateResistanceReferenceText(
            "EntityResearchStatus", root, "EN CURSO", font,
            505f, 205f, 350f, 65f, 42f, teal, TextAlignmentOptions.Left);
        CompressReferenceTextHorizontally(ui.statusText, 0.78f);
        ui.progressText = CreateResistanceReferenceText(
            "EntityResearchProgress", root, "PROGRESO 24%", font,
            505f, 264f, 350f, 45f, 28f, teal, TextAlignmentOptions.Left);
        CompressReferenceTextHorizontally(ui.progressText, 0.85f);
        ui.ancientKnowledgeValueText = CreateResistanceReferenceText(
            "EntityResearchAncientKnowledge", root,
            "CONOCIMIENTO ANTIGUO 76", font,
            505f, 305f, 365f, 43f, 24f, bronze, TextAlignmentOptions.Left);
        CompressReferenceTextHorizontally(ui.ancientKnowledgeValueText, 0.70f);
        TMP_Text consumption = CreateResistanceReferenceText(
            "EntityResearchConsumption", root,
            "CONSUME 1 CONOCIMIENTO\nPOR CADA 1%", font,
            505f, 368f, 350f, 70f, 22f, bronze, TextAlignmentOptions.Left);
        consumption.lineSpacing = -3f;
        CompressReferenceTextHorizontally(consumption, 0.69f);
        TMP_Text speed = CreateResistanceReferenceText(
            "EntityResearchSpeed", root,
            "AVANCE: 1% CADA 30 SEGUNDOS", font,
            505f, 456f, 365f, 46f, 22f, bronze, TextAlignmentOptions.Left);
        CompressReferenceTextHorizontally(speed, 0.72f);

        GameObject nextRing = CreateUIObject("EntityResearchNextMilestoneRing", root);
        SetFirstEntryTopLeft(nextRing.GetComponent<RectTransform>(),
            63f * sx, 552f * sy, 135f * sx, 142f * sy);
        D2SegmentedGaugeGraphic nextGraphic = Undo.AddComponent<D2SegmentedGaugeGraphic>(nextRing);
        nextGraphic.color = FirstEntryHex("9E55B2", 150);
        nextGraphic.raycastTarget = false;
        nextGraphic.Configure(64, 0.455f, 0.49f, 0f, 0f, true);
        nextGraphic.SetProgress(1f);
        ui.milestoneGaugeText = CreateResistanceReferenceText(
            "EntityResearchNextBadge", root, "30%", font,
            86f, 590f, 90f, 66f, 34f, purple, TextAlignmentOptions.Center);
        ui.milestoneText = CreateResistanceReferenceText(
            "EntityResearchNextTitle", root, "PRÓXIMO HITO 30%", font,
            207f, 561f, 510f, 57f, 34f, purple, TextAlignmentOptions.Left);
        CompressReferenceTextHorizontally(ui.milestoneText, 0.78f);
        ui.milestoneCostText = CreateResistanceReferenceText(
            "EntityResearchNextCost", root,
            "25 FRAGMENTOS BASE + 1 DATO BÁSICO", font,
            207f, 610f, 535f, 47f, 25f, bronze, TextAlignmentOptions.Left);
        CompressReferenceTextHorizontally(ui.milestoneCostText, 0.73f);
        ui.milestoneRewardText = CreateResistanceReferenceText(
            "EntityResearchNextReward", root,
            "+1 CONOCIMIENTO DEL ENTE", font,
            207f, 649f, 500f, 45f, 25f, teal, TextAlignmentOptions.Left);
        CompressReferenceTextHorizontally(ui.milestoneRewardText, 0.79f);

        float[] nodeX = { 88f, 305f, 520f, 735f };
        float[] nodeW = { 103f, 101f, 103f, 103f };
        float[] nodeTextX = { 99f, 316f, 526f, 742f };
        float[] nodeTextW = { 80f, 80f, 90f, 90f };
        string[] milestones = { "30%", "60%", "85%", "100%" };
        ui.milestoneSelectionRoots = new GameObject[4];
        ui.milestoneNodeTexts = new TMP_Text[4];
        ui.milestoneCardTitleTexts = new TMP_Text[4];
        for (int i = 0; i < 4; i++)
        {
            GameObject selection = CreateUIObject(
                "EntityResearchMilestoneSelection" + i, root);
            SetFirstEntryTopLeft(selection.GetComponent<RectTransform>(),
                nodeX[i] * sx, 718f * sy, nodeW[i] * sx, 96f * sy);
            D2SegmentedGaugeGraphic selectionGraphic =
                Undo.AddComponent<D2SegmentedGaugeGraphic>(selection);
            selectionGraphic.color = FirstEntryHex("9E55B2", 150);
            selectionGraphic.raycastTarget = false;
            selectionGraphic.Configure(64, 0.44f, 0.49f, 0f, 0f, true);
            selectionGraphic.SetProgress(1f);
            ui.milestoneSelectionRoots[i] = selection;
            selection.SetActive(i == 0);
            ui.milestoneNodeTexts[i] = CreateResistanceReferenceText(
                "EntityResearchMilestoneNode" + i, root, milestones[i], font,
                nodeTextX[i], 744f, nodeTextW[i], 50f,
                i == 3 ? 27f : 28f,
                i == 0 ? purple : bronze, TextAlignmentOptions.Center);
        }

        float[] cardTitleX = { 45f, 255f, 466f, 681f };
        float[] cardTitleW = { 190f, 193f, 196f, 215f };
        float[] costX = { 41f, 258f, 467f, 679f };
        float[] costW = { 197f, 190f, 195f, 211f };
        float[] rewardX = { 52f, 267f, 478f, 694f };
        float[] rewardW = { 178f, 170f, 175f, 188f };
        string[] cardCosts =
        {
            "25 FRAGMENTOS BASE\n+ 1 DATO BÁSICO",
            "35 INSCRIPCIONES\n+ 1 DATO SIMBÓLICO",
            "45 SELLOS\n+ 1 DATO PROFUNDO",
            "50 FRAGMENTOS\n+ 50 INSCRIPCIONES\n+ 50 SELLOS"
        };
        string[] cardRewards =
        {
            "+1 CONOCIMIENTO\nDEL ENTE",
            "+2 CONOCIMIENTO\nDEL ENTE",
            "+3 CONOCIMIENTO\nDEL ENTE",
            "PREPARAR PACTO\nOPCIONAL"
        };
        for (int i = 0; i < 4; i++)
        {
            ui.milestoneCardTitleTexts[i] = CreateResistanceReferenceText(
                "EntityResearchMilestoneCardTitle" + i, root,
                "HITO " + milestones[i], font,
                cardTitleX[i], 818f, cardTitleW[i], 43f, 22f,
                i == 0 ? purple : bronze, TextAlignmentOptions.Center);
            CompressReferenceTextHorizontally(ui.milestoneCardTitleTexts[i], 0.88f);
            TMP_Text cost = CreateResistanceReferenceText(
                "EntityResearchMilestoneCardCost" + i, root, cardCosts[i], font,
                costX[i], 850f, costW[i], i == 3 ? 79f : 74f, 16.5f,
                bronze, TextAlignmentOptions.Center);
            cost.lineSpacing = -5f;
            CompressReferenceTextHorizontally(cost, 0.88f);
            TMP_Text reward = CreateResistanceReferenceText(
                "EntityResearchMilestoneCardReward" + i, root,
                cardRewards[i], font,
                rewardX[i], i == 3 ? 930f : 921f, rewardW[i],
                i == 3 ? 55f : 64f, 16.5f, teal, TextAlignmentOptions.Center);
            reward.lineSpacing = -5f;
            CompressReferenceTextHorizontally(reward, 0.88f);
        }

        TMP_Text resourcesHeading = CreateResistanceReferenceText(
            "EntityResearchResourcesHeading", root, "RECURSOS", font,
            70f, 1008f, 355f, 49f, 30f, bronze, TextAlignmentOptions.Center);
        CompressReferenceTextHorizontally(resourcesHeading, 0.84f);
        TMP_Text dataHeading = CreateResistanceReferenceText(
            "EntityResearchDataHeading", root, "DATOS", font,
            500f, 1008f, 358f, 49f, 30f, bronze, TextAlignmentOptions.Center);
        CompressReferenceTextHorizontally(dataHeading, 0.84f);
        string[] resourceLabels = { "FRAGMENTOS BASE", "INSCRIPCIONES", "SELLOS" };
        string[] dataLabels = { "BÁSICOS", "SIMBÓLICOS", "PROFUNDOS" };
        TMP_Text[] resourceValues = new TMP_Text[3];
        TMP_Text[] dataValues = new TMP_Text[3];
        string[] resourceDefaults = { "40", "35", "45" };
        for (int i = 0; i < 3; i++)
        {
            float y = 1062f + i * 55f;
            TMP_Text resourceLabel = CreateResistanceReferenceText(
                "EntityResearchResourceLabel" + i, root, resourceLabels[i], font,
                135f, y, 210f, 42f, 22f, bronze, TextAlignmentOptions.Left);
            CompressReferenceTextHorizontally(resourceLabel, 0.80f);
            resourceValues[i] = CreateResistanceReferenceText(
                "EntityResearchResourceValue" + i, root, resourceDefaults[i], font,
                356f, y - 1f, 55f, 43f, 24f, teal, TextAlignmentOptions.Right);
            TMP_Text dataLabel = CreateResistanceReferenceText(
                "EntityResearchDataLabel" + i, root, dataLabels[i], font,
                572f, y, 190f, 42f, 22f, bronze, TextAlignmentOptions.Left);
            CompressReferenceTextHorizontally(dataLabel, 0.86f);
            dataValues[i] = CreateResistanceReferenceText(
                "EntityResearchDataValue" + i, root, "1", font,
                786f, y - 1f, 48f, 43f, 24f, teal, TextAlignmentOptions.Right);
        }
        ui.fragmentsValueText = resourceValues[0];
        ui.inscriptionsValueText = resourceValues[1];
        ui.sealsValueText = resourceValues[2];
        ui.basicDataValueText = dataValues[0];
        ui.symbolicDataValueText = dataValues[1];
        ui.deepDataValueText = dataValues[2];
        ui.resourcesText = ui.fragmentsValueText;

        TMP_Text knowledgeHeading = CreateResistanceReferenceText(
            "EntityResearchKnowledgeHeading", root,
            "CONOCIMIENTO DEL ENTE", font,
            132f, 1255f, 550f, 56f, 30f, purple, TextAlignmentOptions.Left);
        CompressReferenceTextHorizontally(knowledgeHeading, 0.74f);
        ui.entityKnowledgeValueText = CreateResistanceReferenceText(
            "EntityResearchKnowledgeValue", root, "0 / 6", font,
            740f, 1250f, 120f, 60f, 34f, purple, TextAlignmentOptions.Right);
        ui.entityKnowledgeText = ui.entityKnowledgeValueText;
        ui.entityKnowledgeFillRoots = new GameObject[6];
        float[] knowledgeX = { 161f, 294f, 425f, 558f, 692f, 825f };
        for (int i = 0; i < knowledgeX.Length; i++)
        {
            ui.entityKnowledgeFillRoots[i] = CreateFirstEntryDiamond(
                "EntityResearchKnowledgeFill" + i, root,
                knowledgeX[i] * sx, 1337f * sy, 29f * sx,
                FirstEntryHex("D299E4", 220), FirstEntryHex("6B287D", 230));
            ui.entityKnowledgeFillRoots[i].SetActive(false);
        }

        ui.startPauseHighlightImage = CreateFirstEntryImage(
            "EntityResearchStartHighlight", root, null,
            39f * sx, 1380f * sy, 416f * sx, 77f * sy,
            FirstEntryHex("58236D", 105), false);
        ui.milestoneHighlightImage = CreateFirstEntryImage(
            "EntityResearchMilestoneHighlight", root, null,
            490f * sx, 1380f * sy, 416f * sx, 77f * sy,
            FirstEntryHex("2D7779", 95), false);
        ui.milestoneHighlightImage.gameObject.SetActive(false);
        ui.startPauseActionText = CreateResistanceReferenceText(
            "EntityResearchStartLabel", root, "PAUSAR INVESTIGACIÓN", font,
            54f, 1391f, 392f, 60f, 29f, ivory, TextAlignmentOptions.Center);
        CompressReferenceTextHorizontally(ui.startPauseActionText, 0.83f);
        ui.milestoneActionText = CreateResistanceReferenceText(
            "EntityResearchMilestoneActionLabel", root, "APORTAR RECURSOS", font,
            500f, 1392f, 395f, 58f, 27f, disabled, TextAlignmentOptions.Center);
        ui.startPauseButton = CreateMapInvisibleButton(
            "Btn_EntityResearchStartPause", root,
            35f * sx, 1377f * sy, 423f * sx, 83f * sy);
        ui.completeMilestoneButton = CreateMapInvisibleButton(
            "Btn_EntityResearchMilestone", root,
            486f * sx, 1377f * sy, 423f * sx, 83f * sy);

        ui.lastResultText = CreateResistanceReferenceText(
            "EntityResearchLastResult", root,
            "ÚLTIMO RESULTADO — LA INVESTIGACIÓN HA COMENZADO.", font,
            130f, 1472f, 755f, 58f, 23.5f, purple, TextAlignmentOptions.Left);
        CompressReferenceTextHorizontally(ui.lastResultText, 0.73f);
        ui.objectiveText = CreateResistanceReferenceText(
            "EntityResearchObjective", root,
            "OBJETIVO ACTUAL — REVISA EL HITO ACTUAL DEL ENTE.", font,
            130f, 1537f, 755f, 50f, 23.5f, teal, TextAlignmentOptions.Left);

        string[] navLabels = { "EXCAVAR", "ANALIZAR", "ARCHIVO", "ENTE" };
        float[] navX = { 20f, 245f, 470f, 696f };
        float[] navW = { 225f, 225f, 226f, 225f };
        for (int i = 0; i < navLabels.Length; i++)
        {
            CreateResistanceReferenceText(
                "EntityResearchNavLabel" + i, root, navLabels[i], font,
                navX[i], 1638f, navW[i], 34f, i == 3 ? 24f : 22f,
                i == 3 ? ivory : bronze, TextAlignmentOptions.Center);
        }
        ui.backToArchaeologyButton = CreateMapInvisibleButton(
            "Btn_EntityResearchBack", root,
            31f * sx, 16f * sy, 82f * sx, 72f * sy);
        ui.excavateNavigationButton = CreateMapInvisibleButton(
            "Btn_EntityResearchNavExcavate", root,
            15f * sx, 1596f * sy, 231f * sx, 76f * sy);
        ui.analyzeNavigationButton = CreateMapInvisibleButton(
            "Btn_EntityResearchNavAnalyze", root,
            247f * sx, 1596f * sy, 223f * sx, 76f * sy);
        ui.archiveNavigationButton = CreateMapInvisibleButton(
            "Btn_EntityResearchNavArchive", root,
            472f * sx, 1596f * sy, 220f * sx, 76f * sy);

        BuildCivilization3EntityPactPhase(ui, pactRoot, pactBasePlate, font);

        ui.progressSlider = CreateProgressSlider(
            "EntityResearchLegacyProgress", legacy,
            new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        ui.backToMapButton = CreateButton(
            "Btn_EntityResearchLegacyMap", legacy, "",
            new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        ui.firstThresholdChronicleButton = CreateButton(
            "Btn_EntityPactFirstThresholdChronicle", legacy, "",
            new Vector2(0.5f, 0.5f), new Vector2(0.001f, 0.001f));
        legacyBindings.SetActive(false);
        ui.pactPhaseVisualRoot.SetActive(false);
        civilization3UI.entityResearchSectionRoot.SetActive(false);
        return ui;
    }

    private static void BuildCivilization3EntityPactPhase(
        D2EntityResearchPanelUI ui,
        Transform root,
        Texture2D basePlate,
        TMP_FontAsset font)
    {
        RawImage plate = CreateFirstEntryRawImage(
            "EntityPactBasePlate", root, basePlate, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight,
            new Rect(0f, 0f, 1f, 1f));
        plate.raycastTarget = false;

        Color ivory = FirstEntryHex("D8C09A");
        Color bronze = FirstEntryHex("B18A5C");
        Color teal = FirstEntryHex("65C5C5");
        Color purple = FirstEntryHex("B286C1");
        Color disabled = FirstEntryHex("5C5650");
        float sx = FirstEntryWidth / 941f;
        float sy = FirstEntryHeight / 1672f;

        TMP_Text title = CreateResistanceReferenceText(
            "EntityPactTitle", root, "PACTO CON EL ENTE", font,
            180f, 20f, 620f, 70f, 47f, ivory, TextAlignmentOptions.Center);
        CompressReferenceTextHorizontally(title, 0.84f);
        TMP_Text notice = CreateResistanceReferenceText(
            "EntityPactNotice", root,
            "CONTENIDO AVANZADO OPCIONAL — Pacto con el Ente disponible.", font,
            164f, 122f, 705f, 42f, 20f, purple, TextAlignmentOptions.Left);
        CompressReferenceTextHorizontally(notice, 0.88f);
        TMP_Text noticeDetail = CreateResistanceReferenceText(
            "EntityPactNoticeDetail", root,
            "El objetivo principal de cierre sigue siendo el Pacto Mayor de Civilización 2.", font,
            164f, 158f, 715f, 42f, 16f, bronze, TextAlignmentOptions.Left);
        CompressReferenceTextHorizontally(noticeDetail, 0.88f);

        ui.pactStatusText = CreateResistanceReferenceText(
            "EntityPactStatus", root, "PACTO CON EL ENTE — ESTABLECIDO", font,
            241f, 249f, 655f, 65f, 42f, teal, TextAlignmentOptions.Left);
        CompressReferenceTextHorizontally(ui.pactStatusText, 0.74f);
        ui.pactProgressText = CreateResistanceReferenceText(
            "EntityPactProgress", root, "PROGRESO 100%", font,
            297f, 330f, 195f, 52f, 25.5f, teal, TextAlignmentOptions.Left);
        CompressReferenceTextHorizontally(ui.pactProgressText, 0.82f);
        ui.pactAncientKnowledgeText = CreateResistanceReferenceText(
            "EntityPactAncientKnowledge", root, "CONOCIMIENTO ANTIGUO 200", font,
            580f, 329f, 300f, 54f, 24.5f, teal, TextAlignmentOptions.Left);
        CompressReferenceTextHorizontally(ui.pactAncientKnowledgeText, 0.69f);

        float[] cardX = { 31f, 248f, 468f, 689f };
        float[] cardW = { 210f, 212f, 216f, 220f };
        string[] cardTitles =
        {
            "EXPEDICIÓN\nRESONANTE",
            "ARCHIVO\nINAGOTABLE",
            "MEMORIA\nCOMPARTIDA",
            "RESONANCIA\nDEL MODULADOR"
        };
        Rect[] iconCrops =
        {
            new Rect(49f, 519f, 178f, 188f),
            new Rect(270f, 519f, 174f, 188f),
            new Rect(486f, 519f, 172f, 188f),
            new Rect(709f, 519f, 179f, 188f)
        };
        ui.pactSelectionRoots = new GameObject[4];
        ui.pactDetailIconRoots = new GameObject[4];
        ui.pactCardTitleTexts = new TMP_Text[4];
        ui.pactLevelTexts = new TMP_Text[4];
        Button[] cardButtons = new Button[4];
        for (int i = 0; i < 4; i++)
        {
            GameObject selection = CreateUIObject(
                "EntityPactSelection" + i, root);
            SetFirstEntryTopLeft(selection.GetComponent<RectTransform>(),
                cardX[i] * sx, 430f * sy, cardW[i] * sx, 369f * sy);
            CreateFirstEntryRail("SelectionTop", selection.transform,
                0f, 0f, cardW[i] * sx, 5f * sy, FirstEntryHex("C47DE1", 225));
            CreateFirstEntryRail("SelectionBottom", selection.transform,
                0f, 364f * sy, cardW[i] * sx, 5f * sy, FirstEntryHex("C47DE1", 225));
            CreateFirstEntryRail("SelectionLeft", selection.transform,
                0f, 0f, 5f * sx, 369f * sy, FirstEntryHex("C47DE1", 225));
            CreateFirstEntryRail("SelectionRight", selection.transform,
                (cardW[i] - 5f) * sx, 0f, 5f * sx, 369f * sy,
                FirstEntryHex("C47DE1", 225));
            ui.pactSelectionRoots[i] = selection;
            selection.SetActive(i == 0);

            ui.pactDetailIconRoots[i] = CreateResistanceSourceIconCrop(
                "EntityPactDetailIcon" + i, root, basePlate, null,
                iconCrops[i], new Rect(47f, 832f, 205f, 216f)).gameObject;
            ui.pactDetailIconRoots[i].SetActive(i == 0);

            ui.pactCardTitleTexts[i] = CreateResistanceReferenceText(
                "EntityPactCardTitle" + i, root, cardTitles[i], font,
                cardX[i] + 9f, 438f, cardW[i] - 18f, 76f, 22f,
                i == 0 ? purple : bronze, TextAlignmentOptions.Center);
            ui.pactCardTitleTexts[i].lineSpacing = -3f;
            CompressReferenceTextHorizontally(ui.pactCardTitleTexts[i], 0.84f);
            ui.pactLevelTexts[i] = CreateResistanceReferenceText(
                "EntityPactLevel" + i, root, i == 0 ? "NIVEL 1/3" : "NIVEL 0/3", font,
                cardX[i] + 21f, 720f, cardW[i] - 42f, 48f, 25f,
                i == 0 ? purple : bronze, TextAlignmentOptions.Center);
            cardButtons[i] = CreateMapInvisibleButton(
                "Btn_EntityPactCard" + i, root,
                cardX[i] * sx, 430f * sy, cardW[i] * sx, 369f * sy);
        }
        ui.resonantExpeditionButton = cardButtons[0];
        ui.endlessArchiveButton = cardButtons[1];
        ui.sharedMemoryButton = cardButtons[2];
        ui.modulatorResonanceButton = cardButtons[3];

        ui.pactDetailTitleText = CreateResistanceReferenceText(
            "EntityPactDetailTitle", root,
            "PACTO — EXPEDICIÓN RESONANTE | NIVEL 1/3", font,
            275f, 835f, 625f, 66f, 25.5f, purple, TextAlignmentOptions.Left);
        CompressReferenceTextHorizontally(ui.pactDetailTitleText, 0.86f);
        ui.pactDetailEffectText = CreateResistanceReferenceText(
            "EntityPactDetailEffect", root,
            "<color=#65C5C5>+10%</color> acumulación de restos adicionales por nivel.", font,
            275f, 898f, 600f, 48f, 23f, bronze, TextAlignmentOptions.Left);
        CompressReferenceTextHorizontally(ui.pactDetailEffectText, 0.76f);
        ui.pactDetailCostText = CreateResistanceReferenceText(
            "EntityPactDetailCost", root,
            "SIGUIENTE: 100 CONOCIMIENTO + 50 DE CADA RECURSO |", font,
            275f, 973f, 610f, 42f, 22f, teal, TextAlignmentOptions.Left);
        CompressReferenceTextHorizontally(ui.pactDetailCostText, 0.71f);
        ui.pactThresholdText = CreateResistanceReferenceText(
            "EntityPactThreshold", root, "UMBRAL DEL ENTE: 3", font,
            275f, 1010f, 400f, 43f, 22f, teal, TextAlignmentOptions.Left);
        CompressReferenceTextHorizontally(ui.pactThresholdText, 0.79f);

        string[] resourceLabels = { "FRAGMENTOS", "INSCRIPCIONES", "SELLOS" };
        float[] labelX = { 145f, 437f, 724f };
        float[] labelW = { 170f, 178f, 145f };
        float[] valueX = { 155f, 452f, 710f };
        for (int i = 0; i < 3; i++)
        {
            TMP_Text label = CreateResistanceReferenceText(
                "EntityPactResourceLabel" + i, root, resourceLabels[i], font,
                labelX[i], 1082f, labelW[i], 43f, 23.5f, bronze,
                TextAlignmentOptions.Center);
            CompressReferenceTextHorizontally(label, 0.82f);
        }
        ui.pactFragmentsValueText = CreateResistanceReferenceText(
            "EntityPactFragmentsValue", root, "100", font,
            valueX[0], 1118f, 110f, 44f, 29f, teal, TextAlignmentOptions.Center);
        ui.pactInscriptionsValueText = CreateResistanceReferenceText(
            "EntityPactInscriptionsValue", root, "100", font,
            valueX[1], 1118f, 110f, 44f, 29f, teal, TextAlignmentOptions.Center);
        ui.pactSealsValueText = CreateResistanceReferenceText(
            "EntityPactSealsValue", root, "100", font,
            valueX[2], 1118f, 120f, 44f, 29f, teal, TextAlignmentOptions.Center);

        TMP_Text knowledgeLabel = CreateResistanceReferenceText(
            "EntityPactKnowledgeLabel", root, "CONOCIMIENTO DEL ENTE:", font,
            137f, 1202f, 340f, 50f, 26f, purple, TextAlignmentOptions.Left);
        CompressReferenceTextHorizontally(knowledgeLabel, 0.76f);
        ui.pactEntityKnowledgeValueText = CreateResistanceReferenceText(
            "EntityPactKnowledgeValue", root, "6/6", font,
            477f, 1202f, 60f, 50f, 27f, teal, TextAlignmentOptions.Left);
        TMP_Text knowledgeSuffix = CreateResistanceReferenceText(
            "EntityPactKnowledgeSuffix", root, "— permanente y no gastable.", font,
            536f, 1202f, 315f, 50f, 22.5f, bronze, TextAlignmentOptions.Left);
        CompressReferenceTextHorizontally(knowledgeSuffix, 0.78f);

        ui.pactEstablishHighlightImage = CreateFirstEntryImage(
            "EntityPactEstablishHighlight", root, null,
            54f * sx, 1285f * sy, 419f * sx, 94f * sy,
            FirstEntryHex("2D7779", 95), false);
        ui.pactUpgradeHighlightImage = CreateFirstEntryImage(
            "EntityPactUpgradeHighlight", root, null,
            495f * sx, 1285f * sy, 387f * sx, 94f * sy,
            FirstEntryHex("58236D", 72), false);
        ui.pactEstablishHighlightImage.gameObject.SetActive(false);
        ui.pactEstablishActionText = CreateResistanceReferenceText(
            "EntityPactEstablishLabel", root, "PACTO ESTABLECIDO", font,
            67f, 1301f, 389f, 62f, 31f, disabled, TextAlignmentOptions.Center);
        CompressReferenceTextHorizontally(ui.pactEstablishActionText, 0.82f);
        ui.pactUpgradeActionText = CreateResistanceReferenceText(
            "EntityPactUpgradeLabel", root, "MEJORAR LÍNEA", font,
            515f, 1298f, 350f, 69f, 35f, ivory, TextAlignmentOptions.Center);
        CompressReferenceTextHorizontally(ui.pactUpgradeActionText, 0.82f);
        ui.pactEstablishButton = CreateMapInvisibleButton(
            "Btn_EntityPactEstablish", root,
            54f * sx, 1285f * sy, 419f * sx, 94f * sy);
        ui.pactUpgradeButton = CreateMapInvisibleButton(
            "Btn_EntityPactUpgrade", root,
            495f * sx, 1285f * sy, 387f * sx, 94f * sy);

        ui.pactLastResultText = CreateResistanceReferenceText(
            "EntityPactLastResult", root,
            "Expedición Resonante mejorada a nivel 1.", font,
            154f, 1433f, 700f, 67f, 31f, purple, TextAlignmentOptions.Left);
        CompressReferenceTextHorizontally(ui.pactLastResultText, 0.78f);

        ui.pactNavigationSelectionRoot = CreateFirstEntryImage(
            "EntityPactNavigationSelection", root, null,
            694f * sx, 1534f * sy, 218f * sx, 133f * sy,
            FirstEntryHex("58236D", 62), false).gameObject;
        string[] navLabels = { "EXCAVAR", "ANALIZAR", "ARCHIVO", "PACTO\nOPCIONAL" };
        float[] navX = { 26f, 250f, 470f, 696f };
        float[] navW = { 220f, 220f, 220f, 215f };
        for (int i = 0; i < 4; i++)
        {
            TMP_Text nav = CreateResistanceReferenceText(
                "EntityPactNavLabel" + i, root, navLabels[i], font,
                navX[i], i == 3 ? 1594f : 1620f, navW[i], i == 3 ? 76f : 48f,
                24f, i == 3 ? ivory : bronze,
                TextAlignmentOptions.Center);
            if (i == 3)
            {
                nav.lineSpacing = -4f;
                ui.pactNavigationLabelText = nav;
            }
            CompressReferenceTextHorizontally(nav, 0.84f);
        }
        ui.pactBackToArchaeologyButton = CreateMapInvisibleButton(
            "Btn_EntityPactBack", root,
            31f * sx, 17f * sy, 82f * sx, 70f * sy);
        ui.pactExcavateNavigationButton = CreateMapInvisibleButton(
            "Btn_EntityPactNavExcavate", root,
            25f * sx, 1534f * sy, 224f * sx, 133f * sy);
        ui.pactAnalyzeNavigationButton = CreateMapInvisibleButton(
            "Btn_EntityPactNavAnalyze", root,
            254f * sx, 1534f * sy, 211f * sx, 133f * sy);
        ui.pactArchiveNavigationButton = CreateMapInvisibleButton(
            "Btn_EntityPactNavArchive", root,
            476f * sx, 1534f * sy, 210f * sx, 133f * sy);
    }

    private static D2ArchivePanelUI BuildCivilization3ArchiveView(
        D2Civilization3PanelUI civilization3UI,
        Transform parent,
        Texture2D basePlate,
        Texture2D lockIcon,
        TMP_FontAsset font)
    {
        civilization3UI.archiveSectionRoot = CreateView(
            "D2_Civ3_ArchiveSection", parent);
        Transform root = civilization3UI.archiveSectionRoot.transform;
        RectTransform rootRect = root as RectTransform;
        rootRect.anchoredPosition = new Vector2(-24f, 32f);

        D2ArchivePanelUI ui = Undo.AddComponent<D2ArchivePanelUI>(
            civilization3UI.archiveSectionRoot);
        civilization3UI.archivePanelUI = ui;
        ui.civilization3PanelUI = civilization3UI;

        RawImage plate = CreateFirstEntryRawImage(
            "ArchiveBasePlate", root, basePlate, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight,
            new Rect(0f, 0f, 1f, 1f));
        plate.raycastTarget = false;

        Color ivory = FirstEntryHex("D8C09A");
        Color bronze = FirstEntryHex("B18A5C");
        Color teal = FirstEntryHex("65C5C5");
        Color purple = FirstEntryHex("B286C1");
        float sx = FirstEntryWidth / 941f;
        float sy = FirstEntryHeight / 1672f;

        ui.stateText = CreateResistanceReferenceText(
            "ArchiveTitle", root, "ARCHIVO IV — MEJORAS PERMANENTES", font,
            123f, 22f, 796f, 69f, 42f, ivory, TextAlignmentOptions.Center);
        ui.stateText.enableAutoSizing = true;
        ui.stateText.fontSizeMin = 32f * sy;
        ui.levelText = CreateResistanceReferenceText(
            "ArchiveLevel", root, "IV", font,
            416f, 216f, 109f, 78f, 54f, teal, TextAlignmentOptions.Center);

        string[] resourceLabels =
        {
            "CONOCIMIENTO\nANTIGUO",
            "CONOCIMIENTO\nDEL ENTE",
            "FRAGMENTOS\nBASE",
            "INSCRIPCIONES\nPARCIALES",
            "SELLOS\nANTIGUOS"
        };
        float[] resourceX = { 96f, 307f, 501f, 663f, 847f };
        float[] resourceW = { 128f, 122f, 98f, 104f, 73f };
        for (int i = 0; i < resourceLabels.Length; i++)
        {
            TMP_Text label = CreateResistanceReferenceText(
                "ArchiveResourceLabel" + i, root, resourceLabels[i], font,
                resourceX[i], 108f, resourceW[i], 45f, 14f,
                bronze, TextAlignmentOptions.Left);
            label.lineSpacing = -5f;
            label.enableAutoSizing = true;
            label.fontSizeMin = 11f * sy;
        }

        ui.ancientKnowledgeValueText = CreateResistanceReferenceText(
            "ArchiveAncientKnowledgeValue", root, "90", font,
            96f, 146f, 75f, 35f, 24f, bronze, TextAlignmentOptions.Left);
        ui.entityKnowledgeValueText = CreateResistanceReferenceText(
            "ArchiveEntityKnowledgeValue", root, "3 / 6", font,
            307f, 146f, 81f, 35f, 24f, bronze, TextAlignmentOptions.Left);
        ui.fragmentsValueText = CreateResistanceReferenceText(
            "ArchiveFragmentsValue", root, "40", font,
            501f, 146f, 58f, 35f, 24f, bronze, TextAlignmentOptions.Left);
        ui.inscriptionsValueText = CreateResistanceReferenceText(
            "ArchiveInscriptionsValue", root, "20", font,
            663f, 146f, 58f, 35f, 24f, bronze, TextAlignmentOptions.Left);
        ui.sealsValueText = CreateResistanceReferenceText(
            "ArchiveSealsValue", root, "5", font,
            847f, 146f, 45f, 35f, 24f, bronze, TextAlignmentOptions.Left);
        ui.resourcesText = ui.ancientKnowledgeValueText;

        string[] upgradeIds =
        {
            D2Civilization3System.StratifiedCartographyUpgradeId,
            D2Civilization3System.AnomalousConcordanceUpgradeId,
            D2Civilization3System.DeepExegesisUpgradeId
        };
        string[] upgradeNames =
        {
            "CARTOGRAFÍA\nESTRATIFICADA",
            "CONCORDANCIA\nANÓMALA",
            "EXÉGESIS\nPROFUNDA"
        };
        string[] descriptions =
        {
            "+5% VELOCIDAD DE\nEXCAVACIÓN\nEN LAS TRES ZONAS",
            "+10% ACUMULACIÓN DE\nINDICIOS ANÓMALOS",
            "-10% COSTES DE LECTURA\nDE ANOMALÍAS"
        };
        string[] levelRoman = { "II", "III", "IV" };
        string[] resourceCostNames =
        {
            "FRAGMENTOS BASE",
            "INSCRIPCIONES\nPARCIALES",
            "SELLOS ANTIGUOS"
        };
        float[] cardX = { 60f, 358f, 656f };
        float[] cardWidth = { 225f, 223f, 222f };
        float[] descriptionX = { 74f, 364f, 660f };
        float[] descriptionWidth = { 199f, 210f, 216f };
        float[] costTextX = { 108f, 404f, 702f };
        float[] actionX = { 65f, 362f, 660f };
        float[] statusX = { 75f, 373f, 670f };
        float[] buttonX = { 52f, 349f, 646f };
        float[] lockX = { 158f, 455f, 752f };

        ui.actionTexts = new TMP_Text[3];
        ui.upgradeStateTexts = new TMP_Text[3];
        ui.actionHighlightImages = new Image[3];
        ui.lockIconRoots = new GameObject[3];
        Button[] upgradeButtons = new Button[3];
        TMP_Text[] upgradeTitles = new TMP_Text[3];
        Material whiteKey = GetOrCreateFirstEntryWhiteKeyMaterial();

        for (int i = 0; i < upgradeIds.Length; i++)
        {
            upgradeTitles[i] = CreateResistanceReferenceText(
                "ArchiveUpgradeTitle" + i, root, upgradeNames[i], font,
                cardX[i], 711f, cardWidth[i], 65f, 27f,
                ivory, TextAlignmentOptions.Center);
            upgradeTitles[i].lineSpacing = -5f;
            upgradeTitles[i].enableAutoSizing = true;
            upgradeTitles[i].fontSizeMin = 21f * sy;

            TMP_Text description = CreateResistanceReferenceText(
                "ArchiveUpgradeDescription" + i, root, descriptions[i], font,
                descriptionX[i], 944f, descriptionWidth[i],
                i == 0 ? 62f : 50f, 15.5f,
                bronze, TextAlignmentOptions.Center);
            description.lineSpacing = -4f;
            description.enableAutoSizing = true;
            description.fontSizeMin = 12f * sy;

            TMP_Text requirement = CreateResistanceReferenceText(
                "ArchiveUpgradeRequirement" + i, root,
                "REQUIERE ARCHIVO " + levelRoman[i], font,
                i == 0 ? 71f : i == 1 ? 367f : 664f,
                1011f, i == 0 ? 215f : i == 1 ? 213f : 214f,
                38f, 18f, bronze, TextAlignmentOptions.Center);
            requirement.enableAutoSizing = true;
            requirement.fontSizeMin = 14f * sy;

            TMP_Text knowledgeCost = CreateResistanceReferenceText(
                "ArchiveKnowledgeCost" + i, root,
                D2Civilization3System.GetArchiveUpgradeKnowledgeCost(
                    upgradeIds[i]).ToString("0") + " CONOCIMIENTO", font,
                costTextX[i], 1060f, i == 2 ? 181f : i == 1 ? 178f : 177f,
                36f, 19f, bronze, TextAlignmentOptions.Left);
            knowledgeCost.enableAutoSizing = true;
            knowledgeCost.fontSizeMin = 13f * sy;

            TMP_Text resourceCost = CreateResistanceReferenceText(
                "ArchiveResourceCost" + i, root,
                D2Civilization3System.GetArchiveUpgradeResourceCost(
                    upgradeIds[i]).ToString("0") + " " + resourceCostNames[i], font,
                costTextX[i], i == 1 ? 1104f : 1109f,
                i == 2 ? 181f : i == 1 ? 180f : 179f,
                i == 1 ? 57f : 39f, 18f,
                bronze, TextAlignmentOptions.Left);
            resourceCost.lineSpacing = -4f;
            resourceCost.enableAutoSizing = true;
            resourceCost.fontSizeMin = 13f * sy;

            TMP_Text entityThreshold = CreateResistanceReferenceText(
                "ArchiveEntityThreshold" + i, root,
                "UMBRAL DEL ENTE: " +
                D2Civilization3System.GetArchiveUpgradeEntityKnowledgeRequirement(
                    upgradeIds[i]).ToString("0"), font,
                i == 0 ? 68f : i == 1 ? 366f : 663f,
                1174f, i == 0 ? 219f : i == 1 ? 219f : 220f,
                38f, 17.5f, bronze, TextAlignmentOptions.Center);
            D2ArchivePanelUI.RefreshEntityThreshold(entityThreshold, upgradeIds[i]);

            Image highlightFill;
            CreateFirstEntryPanel(
                "ArchiveActionAvailable" + i, root,
                buttonX[i] * sx, 1212f * sy,
                238f * sx, 70f * sy,
                FirstEntryHex("174B4C", 205), teal,
                FirstEntryHex("9FE7D3", 145), 3f, 6f, false,
                out highlightFill);
            highlightFill.raycastTarget = false;
            ui.actionHighlightImages[i] = highlightFill;

            RawImage lockImage = CreateFirstEntryRawImage(
                "ArchiveLock" + i, root, lockIcon, whiteKey,
                (lockX[i] - 26f) * sx, 1207f * sy,
                84f * sx, 79f * sy,
                new Rect(0f, 0f, 1f, 1f));
            lockImage.raycastTarget = false;
            ui.lockIconRoots[i] = lockImage.gameObject;

            ui.actionTexts[i] = CreateResistanceReferenceText(
                "ArchiveActionText" + i, root,
                i == 0 ? "DESBLOQUEAR" : string.Empty, font,
                actionX[i], 1218f, 221f, 58f, 24f,
                ivory, TextAlignmentOptions.Center);
            ui.upgradeStateTexts[i] = CreateResistanceReferenceText(
                "ArchiveUpgradeState" + i, root,
                i == 0 ? "DISPONIBLE" : "BLOQUEADA", font,
                statusX[i], 1287f, i == 2 ? 210f : 208f, 42f, 19f,
                i == 0 ? teal : purple, TextAlignmentOptions.Center);

            upgradeButtons[i] = CreateMapInvisibleButton(
                "Btn_Civ3ArchiveUpgrade" + i, root,
                buttonX[i] * sx, 1212f * sy,
                238f * sx, 70f * sy);
        }

        ui.cartographyText = upgradeTitles[0];
        ui.concordanceText = upgradeTitles[1];
        ui.exegesisText = upgradeTitles[2];
        ui.cartographyButton = upgradeButtons[0];
        ui.concordanceButton = upgradeButtons[1];
        ui.exegesisButton = upgradeButtons[2];

        ui.lastResultText = CreateResistanceReferenceText(
            "ArchiveLastResult", root,
            "EL ARCHIVO AGUARDA NUEVOS HALLAZGOS", font,
            111f, 1368f, 719f, 57f, 26f,
            teal, TextAlignmentOptions.Center);
        ui.lastResultText.enableAutoSizing = true;
        ui.lastResultText.fontSizeMin = 20f * sy;

        string[] navLabels = { "EXCAVAR", "ANALIZAR", "ARCHIVO", "ENTE" };
        float[] navX = { 20f, 245f, 470f, 696f };
        float[] navW = { 225f, 225f, 226f, 225f };
        for (int i = 0; i < navLabels.Length; i++)
        {
            CreateResistanceReferenceText(
                "ArchiveNavLabel" + i, root, navLabels[i], font,
                navX[i], 1587f, navW[i], 46f, 24f,
                i == 2 ? ivory : bronze, TextAlignmentOptions.Center);
        }

        ui.backToArchaeologyButton = CreateMapInvisibleButton(
            "Btn_Civ3ArchiveBack", root,
            27f * sx, 24f * sy, 112f * sx, 88f * sy);
        ui.excavateNavigationButton = CreateMapInvisibleButton(
            "Btn_Civ3ArchiveExcavateNav", root,
            20f * sx, 1508f * sy, 225f * sx, 145f * sy);
        ui.analyzeNavigationButton = CreateMapInvisibleButton(
            "Btn_Civ3ArchiveAnalyzeNav", root,
            245f * sx, 1508f * sy, 225f * sx, 145f * sy);
        ui.entityNavigationButton = CreateMapInvisibleButton(
            "Btn_Civ3ArchiveEntityNav", root,
            696f * sx, 1508f * sy, 225f * sx, 145f * sy);

        civilization3UI.archiveSectionRoot.SetActive(false);
        return ui;
    }

    private static void BuildCivilization3AnalysisView(
        D2Civilization3PanelUI ui,
        Transform parent,
        Texture2D basePlate,
        TMP_FontAsset font)
    {
        ui.analysisSectionRoot = CreateView("D2_Civ3_AnalysisSection", parent);
        Transform root = ui.analysisSectionRoot.transform;
        RectTransform rootRect = root as RectTransform;
        rootRect.anchoredPosition = new Vector2(-24f, 32f);

        RawImage plate = CreateFirstEntryRawImage(
            "AnalyzeRemainsBasePlate", root, basePlate, null,
            0f, 0f, FirstEntryWidth, FirstEntryHeight,
            new Rect(0f, 0f, 1f, 1f));
        plate.raycastTarget = false;

        Color ivory = FirstEntryHex("D8C09A");
        Color bronze = FirstEntryHex("B18A5C");
        Color teal = FirstEntryHex("65C5C5");
        Color purple = FirstEntryHex("B286C1");
        float sx = FirstEntryWidth / 941f;
        float sy = FirstEntryHeight / 1672f;

        ui.analysisQualitySelectionOverlays = new GameObject[3];
        float[] cardX = { 41f, 332f, 629f };
        float[] cardW = { 263f, 268f, 271f };
        for (int i = 0; i < 3; i++)
        {
            Image selectionFill;
            RectTransform selection = CreateFirstEntryPanel(
                "AnalyzeQualitySelection" + i, root,
                cardX[i] * sx, 303f * sy,
                cardW[i] * sx, 350f * sy,
                FirstEntryHex("42214D", 10), purple,
                FirstEntryHex("D79BE4", 125), 3f, 6f, false,
                out selectionFill);
            selectionFill.raycastTarget = false;
            ui.analysisQualitySelectionOverlays[i] = selection.gameObject;
            selection.gameObject.SetActive(i == 1);
        }

        ui.analysisResearchFillImage = CreateFirstEntryImage(
            "AnalyzeResearchFill", root, null,
            158f * sx, 1305f * sy, 314f * sx, 20f * sy,
            FirstEntryHex("4CC4C1", 225), false);
        ui.analysisResearchFillImage.raycastTarget = false;

        CreateResistanceReferenceText(
            "AnalyzeTitle", root, "ANALIZAR RESTOS", font,
            146f, 24f, 650f, 66f, 42f, ivory, TextAlignmentOptions.Center);
        ui.analysisZoneTitleText = CreateResistanceReferenceText(
            "AnalyzeZoneTitle", root, "ZONA 1 — ENTRADA SEPULTADA", font,
            143f, 92f, 655f, 57f, 30f, bronze, TextAlignmentOptions.Center);
        ui.analysisZoneTitleText.enableAutoSizing = true;
        ui.analysisZoneTitleText.fontSizeMin = 24f / 1672f * FirstEntryHeight;

        TMP_Text analysisKnowledgeLabel = CreateResistanceReferenceText(
            "AnalyzeKnowledgeLabel", root, "Conocimiento Antiguo", font,
            114f, 169f, 270f, 30f, 18f, bronze, TextAlignmentOptions.Left);
        analysisKnowledgeLabel.enableAutoSizing = true;
        analysisKnowledgeLabel.fontSizeMin = 16f / 1672f * FirstEntryHeight;
        ui.analysisAncientKnowledgeValueText = CreateResistanceReferenceText(
            "AnalyzeKnowledgeValue", root, "18", font,
            114f, 200f, 72f, 34f, 25f, bronze, TextAlignmentOptions.Left);
        ui.analysisResourceNameText = CreateResistanceReferenceText(
            "AnalyzeResourceName", root, "Recurso Propio:\nFragmentos Base", font,
            562f, 168f, 226f, 68f, 18f, bronze, TextAlignmentOptions.Left);
        ui.analysisResourceNameText.enableAutoSizing = true;
        ui.analysisResourceNameText.fontSizeMin =
            16f / 1672f * FirstEntryHeight;
        ui.analysisResourceValueText = CreateResistanceReferenceText(
            "AnalyzeResourceValue", root, "26", font,
            788f, 185f, 58f, 34f, 25f, bronze, TextAlignmentOptions.Center);
        CreateResistanceReferenceText(
            "AnalyzeRemainsTitle", root, "RESTOS ARQUEOLÓGICOS", font,
            253f, 258f, 435f, 49f, 27f, ivory, TextAlignmentOptions.Center);

        ui.analysisQualityNameTexts = new TMP_Text[3];
        ui.analysisQualityCountTexts = new TMP_Text[3];
        string[] qualityNames = { "BAJA", "MEDIA", "ALTA" };
        for (int i = 0; i < 3; i++)
        {
            ui.analysisQualityNameTexts[i] = CreateResistanceReferenceText(
                "AnalyzeQualityName" + i, root, qualityNames[i], font,
                cardX[i], 311f, cardW[i], 48f, 28f,
                i == 1 ? purple : bronze, TextAlignmentOptions.Center);
            ui.analysisQualityCountTexts[i] = CreateResistanceReferenceText(
                "AnalyzeQualityCount" + i, root,
                i == 0 ? "12" : i == 1 ? "5" : "2", font,
                cardX[i] + cardW[i] * 0.5f - 55f, 616f,
                110f, 46f, 29f, i == 1 ? purple : bronze,
                TextAlignmentOptions.Center);
        }
        ui.analysisTotalRemainsText = CreateResistanceReferenceText(
            "AnalyzeTotal", root, "TOTAL 19", font,
            320f, 666f, 300f, 50f, 29f, ivory, TextAlignmentOptions.Center);
        ui.analysisAvailabilityText = CreateResistanceReferenceText(
            "AnalyzeAvailability", root,
            "ANÁLISIS DISPONIBLE — SELECCIONA UNA CALIDAD", font,
            67f, 724f, 808f, 58f, 26f, teal, TextAlignmentOptions.Center);
        ui.analysisAvailabilityText.enableAutoSizing = true;
        ui.analysisAvailabilityText.fontSizeMin = 19f / 1672f * FirstEntryHeight;

        ui.analysisSelectedQualityText = CreateResistanceReferenceText(
            "AnalyzeSelectedQuality", root, "CALIDAD MEDIA", font,
            48f, 805f, 235f, 50f, 25f, purple, TextAlignmentOptions.Left);
        ui.analysisSelectedQualityText.enableAutoSizing = true;
        ui.analysisSelectedQualityText.fontSizeMin = 20f / 1672f * FirstEntryHeight;
        ui.analysisDurationText = CreateResistanceReferenceText(
            "AnalyzeDuration", root, "DURACIÓN\n00:30", font,
            131f, 864f, 145f, 82f, 20f, bronze, TextAlignmentOptions.Left);
        CreateResistanceReferenceText(
            "AnalyzeRewardsTitle", root, "RECOMPENSA BASE", font,
            300f, 789f, 350f, 36f, 18f, bronze, TextAlignmentOptions.Left);
        ui.analysisRewardLineTexts = new TMP_Text[4];
        string[] rewardLines =
        {
            "3  CONOCIMIENTO ANTIGUO",
            "2  FRAGMENTOS BASE",
            "+3%  INVESTIGACIÓN",
            "+8%  ACUMULACIÓN DE INDICIOS"
        };
        float[] rewardLineY = { 838f, 872f, 905f, 938f };
        for (int rewardIndex = 0; rewardIndex < rewardLines.Length; rewardIndex++)
        {
            ui.analysisRewardLineTexts[rewardIndex] = CreateResistanceReferenceText(
                "AnalyzeRewardLine" + rewardIndex, root,
                rewardLines[rewardIndex], font,
                344f, rewardLineY[rewardIndex], 315f, 31f, 17f,
                bronze, TextAlignmentOptions.Left);
            ui.analysisRewardLineTexts[rewardIndex].enableAutoSizing = true;
            ui.analysisRewardLineTexts[rewardIndex].fontSizeMin =
                14f / 1672f * FirstEntryHeight;
        }
        ui.analysisRewardsText = ui.analysisRewardLineTexts[0];
        ui.analysisActionText = CreateResistanceReferenceText(
            "AnalyzeAction", root, "ANALIZAR\nMEDIA", font,
            669f, 831f, 210f, 105f, 29f, ivory, TextAlignmentOptions.Center);

        ui.analysisScholarTitleText = CreateResistanceReferenceText(
            "AnalyzeScholarTitle", root, "ERUDITO DE CAMPO — NIVEL 1/3", font,
            218f, 1008f, 650f, 50f, 27f, bronze, TextAlignmentOptions.Left);
        ui.analysisScholarTitleText.enableAutoSizing = true;
        ui.analysisScholarTitleText.fontSizeMin = 21f / 1672f * FirstEntryHeight;
        ui.analysisScholarNextText = CreateResistanceReferenceText(
            "AnalyzeScholarNext", root,
            "SIGUIENTE\n30  CONOCIMIENTO\n20  FRAGMENTOS BASE\nUMBRAL DEL ENTE: 3", font,
            555f, 1056f, 320f, 119f, 17f, bronze, TextAlignmentOptions.TopLeft);
        ui.analysisScholarNextText.enableAutoSizing = true;
        ui.analysisScholarNextText.fontSizeMin = 14f / 1672f * FirstEntryHeight;

        ui.analysisResearchTitleText = CreateResistanceReferenceText(
            "AnalyzeResearchTitle", root, "INVESTIGACIÓN DE ZONA 1", font,
            150f, 1247f, 510f, 42f, 25f, bronze, TextAlignmentOptions.Left);
        ui.analysisResearchValueText = CreateResistanceReferenceText(
            "AnalyzeResearchValue", root, "45%", font,
            785f, 1242f, 95f, 47f, 29f, teal, TextAlignmentOptions.Center);

        string[] resourceNames =
        {
            "CERA", "PAN RITUAL", "INCIENSO", "TELA SAGRADA", "PIEDRA TALLADA"
        };
        string[] resourceValues = { "3,450", "3,450", "300", "300", "300" };
        float[] resourceX = { 32f, 208f, 384f, 561f, 737f };
        ui.analysisCivilization1ResourceValueTexts = new TMP_Text[5];
        for (int i = 0; i < 5; i++)
        {
            CreateResistanceReferenceText(
                "AnalyzeResourceLabel" + i, root, resourceNames[i], font,
                resourceX[i], 1362f, 172f, 38f,
                i == 1 || i == 3 || i == 4 ? 18f : 20f,
                bronze, TextAlignmentOptions.Center);
            ui.analysisCivilization1ResourceValueTexts[i] =
                CreateResistanceReferenceText(
                    "AnalyzeResourceValue" + i, root, resourceValues[i], font,
                    resourceX[i], 1471f, 172f, 43f, 27f,
                    bronze, TextAlignmentOptions.Center);
        }

        string[] navLabels = { "EXCAVAR", "ANALIZAR", "ARCHIVO", "ENTE" };
        float[] navX = { 20f, 245f, 470f, 696f };
        float[] navW = { 225f, 225f, 226f, 225f };
        for (int i = 0; i < navLabels.Length; i++)
        {
            CreateResistanceReferenceText(
                "AnalyzeNavLabel" + i, root, navLabels[i], font,
                navX[i], 1610f, navW[i], 43f, 24f,
                i == 1 ? ivory : bronze, TextAlignmentOptions.Center);
        }

        ui.analysisBackButton = CreateMapInvisibleButton(
            "Btn_Civ3AnalysisBack", root, 26f * sx, 20f * sy, 112f * sx, 87f * sy);
        ui.analysisQualityButtons = new Button[3];
        for (int i = 0; i < 3; i++)
        {
            ui.analysisQualityButtons[i] = CreateMapInvisibleButton(
                "Btn_Civ3AnalysisQuality" + i, root,
                cardX[i] * sx, 303f * sy, cardW[i] * sx, 350f * sy);
        }
        ui.analysisActionButton = CreateMapInvisibleButton(
            "Btn_Civ3AnalysisStart", root,
            657f * sx, 819f * sy, 234f * sx, 144f * sy);
        ui.hireScholarButton = CreateMapInvisibleButton(
            "Btn_Civ3AnalysisUpgradeScholar", root,
            527f * sx, 1177f * sy, 364f * sx, 59f * sy);
        ui.analysisScholarUpgradeText = CreateResistanceReferenceText(
            "AnalyzeScholarUpgradeLabel", root, "MEJORAR ERUDITO", font,
            529f, 1181f, 358f, 50f, 25f,
            FirstEntryHex("6E6861"), TextAlignmentOptions.Center);
        ui.analysisExcavateButton = CreateMapInvisibleButton(
            "Btn_Civ3AnalysisExcavateNav", root,
            20f * sx, 1530f * sy, 225f * sx, 140f * sy);
        ui.analysisArchiveButton = CreateMapInvisibleButton(
            "Btn_Civ3AnalysisArchiveNav", root,
            470f * sx, 1530f * sy, 226f * sx, 140f * sy);
        ui.analysisEntityButton = CreateMapInvisibleButton(
            "Btn_Civ3AnalysisEntityNav", root,
            696f * sx, 1530f * sy, 225f * sx, 140f * sy);

        ui.analysisSectionRoot.SetActive(false);
    }

    private static void BuildCivilization1PlaceholderLegacy(Dimension2PanelUI panel)
    {
        GameObject root = CreateView("D2_Civilization1", panel.transform);
        panel.civilization1Root = root;
        D2Civilization1PanelUI civilization1UI = Undo.AddComponent<D2Civilization1PanelUI>(root);
        panel.civilization1PanelUI = civilization1UI;

        panel.civilization1PlaceholderText = CreateText(
            "Civilization1Status",
            root.transform,
            "CIVILIZACIÓN 1 — SANTUARIO DE PEREGRINOS",
            28f,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.89f),
            new Vector2(0.82f, 0.10f)
        );

        civilization1UI.followersText = CreateText(
            "FollowersStatus",
            root.transform,
            "SEGUIDORES",
            21f,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.76f),
            new Vector2(0.82f, 0.12f)
        );

        civilization1UI.arrivalText = CreateText(
            "FollowerArrival",
            root.transform,
            "Llegada: 0/s",
            18f,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.67f),
            new Vector2(0.60f, 0.06f)
        );

        civilization1UI.arrivalProgressSlider = CreateProgressSlider(
            "FollowerArrivalProgress",
            root.transform,
            new Vector2(0.5f, 0.62f),
            new Vector2(0.48f, 0.025f)
        );

        civilization1UI.refugeText = CreateText(
            "RefugeStatus",
            root.transform,
            "REFUGIO DE PEREGRINOS — Nivel 1",
            21f,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.54f),
            new Vector2(0.70f, 0.07f)
        );

        civilization1UI.assignmentText = CreateText(
            "RefugeAssignment",
            root.transform,
            "Asignados al Refugio: 0",
            18f,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.47f),
            new Vector2(0.78f, 0.07f)
        );

        civilization1UI.assignOneButton = CreateButton(
            "Btn_AssignOneFollower",
            root.transform,
            "ASIGNAR +1",
            new Vector2(0.18f, 0.37f),
            new Vector2(0.13f, 0.065f)
        );

        civilization1UI.assignTenButton = CreateButton(
            "Btn_AssignTenFollowers",
            root.transform,
            "ASIGNAR +10",
            new Vector2(0.34f, 0.37f),
            new Vector2(0.13f, 0.065f)
        );

        civilization1UI.assignAllButton = CreateButton(
            "Btn_AssignAllFollowers",
            root.transform,
            "ASIGNAR TODO",
            new Vector2(0.50f, 0.37f),
            new Vector2(0.13f, 0.065f)
        );

        civilization1UI.releaseOneButton = CreateButton(
            "Btn_ReleaseOneFollower",
            root.transform,
            "RETIRAR -1",
            new Vector2(0.66f, 0.37f),
            new Vector2(0.13f, 0.065f)
        );

        civilization1UI.releaseAllButton = CreateButton(
            "Btn_ReleaseAllFollowers",
            root.transform,
            "RETIRAR TODO",
            new Vector2(0.82f, 0.37f),
            new Vector2(0.13f, 0.065f)
        );

        civilization1UI.upgradeRefugeButton = CreateButton(
            "Btn_UpgradeRefuge",
            root.transform,
            "MEJORAR REFUGIO",
            new Vector2(0.5f, 0.23f),
            new Vector2(0.25f, 0.09f)
        );
        civilization1UI.upgradeRefugeButtonText =
            civilization1UI.upgradeRefugeButton.GetComponentInChildren<TextMeshProUGUI>(true);

        panel.backToMapButton = CreateButton(
            "Btn_BackToD2Map",
            root.transform,
            "VOLVER AL MAPA",
            new Vector2(0.14f, 0.08f),
            new Vector2(0.20f, 0.07f)
        );
    }

    private static void BuildGlobalCloseButton(Dimension2PanelUI panel)
    {
        panel.contextualHelpButton = CreateButton(
            "D2_Btn_Help", panel.transform, "REPASAR",
            new Vector2(0.72f, 0.08f), new Vector2(0.16f, 0.07f)
        );
        panel.closeDimension2Button = CreateButton(
            "D2_Btn_Close",
            panel.transform,
            "CERRAR",
            new Vector2(0.89f, 0.08f),
            new Vector2(0.16f, 0.07f)
        );
        panel.helpRoot = CreateView("D2_ContextualHelp", panel.transform);
        Canvas helpCanvas = Undo.AddComponent<Canvas>(panel.helpRoot);
        helpCanvas.overrideSorting = true;
        helpCanvas.sortingOrder = 32750;
        Undo.AddComponent<GraphicRaycaster>(panel.helpRoot);
        Image helpBackground = Undo.AddComponent<Image>(panel.helpRoot);
        helpBackground.color = new Color(0.035f, 0.055f, 0.1f, 0.98f);
        panel.helpTitleText = CreateText(
            "D2_HelpTitle", panel.helpRoot.transform,
            "GUÍA DE DIMENSIÓN 2", 34f, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.72f), new Vector2(0.75f, 0.08f));
        panel.helpBodyText = CreateText(
            "D2_HelpBody", panel.helpRoot.transform,
            "AHORA: sigue la acción principal de la pantalla actual.", 22f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f),
            new Vector2(0.72f, 0.28f));
        panel.closeHelpButton = CreateButton(
            "D2_Btn_CloseHelp", panel.helpRoot.transform, "CERRAR",
            new Vector2(0.5f, 0.27f), new Vector2(0.22f, 0.07f));
        panel.helpRoot.SetActive(false);
        panel.contextualHelpButton.transform.SetAsLastSibling();
        panel.closeDimension2Button.transform.SetAsLastSibling();
        panel.helpRoot.transform.SetAsLastSibling();
    }

    private static void CreateTerritoryCard(
        Transform parent,
        string name,
        string title,
        Vector2 anchor,
        out Button button,
        out TMP_Text stateText
    )
    {
        button = CreateButton(
            name,
            parent,
            title,
            anchor,
            new Vector2(0.24f, 0.22f)
        );

        stateText = CreateText(
            name + "_State",
            parent,
            "BLOQUEADA",
            16f,
            TextAlignmentOptions.Center,
            new Vector2(anchor.x, anchor.y - 0.17f),
            new Vector2(0.25f, 0.09f)
        );
    }

    private static GameObject CreateView(string name, Transform parent)
    {
        GameObject root = CreateUIObject(name, parent);
        Stretch(root.GetComponent<RectTransform>());
        return root;
    }

    private static Button CreateButton(
        string name,
        Transform parent,
        string labelText,
        Vector2 anchor,
        Vector2 size
    )
    {
        GameObject buttonObject = CreateUIObject(name, parent);
        Image image = Undo.AddComponent<Image>(buttonObject);
        image.color = new Color(0.14f, 0.18f, 0.30f, 0.98f);
        Button button = Undo.AddComponent<Button>(buttonObject);
        button.targetGraphic = image;
        button.onClick = new Button.ButtonClickedEvent();

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(1100f * size.x, 650f * size.y);

        TextMeshProUGUI label = CreateText(
            "Label",
            buttonObject.transform,
            labelText,
            19f,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.5f),
            Vector2.one
        );
        Stretch(label.rectTransform);
        return button;
    }

    private static Slider CreateProgressSlider(
        string name,
        Transform parent,
        Vector2 anchor,
        Vector2 size
    )
    {
        GameObject sliderObject = CreateUIObject(name, parent);
        RectTransform sliderRect = sliderObject.GetComponent<RectTransform>();
        sliderRect.anchorMin = anchor;
        sliderRect.anchorMax = anchor;
        sliderRect.pivot = new Vector2(0.5f, 0.5f);
        sliderRect.anchoredPosition = Vector2.zero;
        sliderRect.sizeDelta = new Vector2(1100f * size.x, 650f * size.y);

        Image background = Undo.AddComponent<Image>(sliderObject);
        background.color = new Color(0.08f, 0.10f, 0.17f, 1f);

        GameObject fillObject = CreateUIObject("Fill", sliderObject.transform);
        Image fill = Undo.AddComponent<Image>(fillObject);
        fill.color = new Color(0.42f, 0.68f, 0.92f, 1f);
        Stretch(fill.rectTransform);

        Slider slider = Undo.AddComponent<Slider>(sliderObject);
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0f;
        slider.wholeNumbers = false;
        slider.interactable = false;
        slider.fillRect = fill.rectTransform;
        slider.direction = Slider.Direction.LeftToRight;
        return slider;
    }

    private static TMP_Dropdown CreateDropdown(
        string name,
        Transform parent,
        Vector2 anchor,
        Vector2 size
    )
    {
        GameObject dropdownObject = CreateUIObject(name, parent);
        Image image = Undo.AddComponent<Image>(dropdownObject);
        image.color = new Color(0.14f, 0.18f, 0.30f, 0.98f);
        TMP_Dropdown dropdown = Undo.AddComponent<TMP_Dropdown>(dropdownObject);
        dropdown.targetGraphic = image;
        dropdown.onValueChanged = new TMP_Dropdown.DropdownEvent();

        RectTransform rect = dropdownObject.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(1100f * size.x, 650f * size.y);

        TextMeshProUGUI caption = CreateText(
            "Label", dropdownObject.transform, "Altar de Cera", 19f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), Vector2.one
        );
        Stretch(caption.rectTransform);

        GameObject templateObject = CreateUIObject("Template", dropdownObject.transform);
        RectTransform templateRect = templateObject.GetComponent<RectTransform>();
        templateRect.anchorMin = new Vector2(0f, 0f);
        templateRect.anchorMax = new Vector2(1f, 0f);
        templateRect.pivot = new Vector2(0.5f, 1f);
        templateRect.anchoredPosition = Vector2.zero;
        templateRect.sizeDelta = new Vector2(0f, 170f);
        Image templateImage = Undo.AddComponent<Image>(templateObject);
        templateImage.color = new Color(0.08f, 0.10f, 0.17f, 1f);
        ScrollRect scrollRect = Undo.AddComponent<ScrollRect>(templateObject);
        scrollRect.horizontal = false;

        GameObject viewportObject = CreateUIObject("Viewport", templateObject.transform);
        RectTransform viewportRect = viewportObject.GetComponent<RectTransform>();
        Stretch(viewportRect);
        Image viewportImage = Undo.AddComponent<Image>(viewportObject);
        viewportImage.color = Color.white;
        Mask mask = Undo.AddComponent<Mask>(viewportObject);
        mask.showMaskGraphic = false;

        GameObject contentObject = CreateUIObject("Content", viewportObject.transform);
        RectTransform contentRect = contentObject.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(0f, 150f);

        GameObject itemObject = CreateUIObject("Item", contentObject.transform);
        RectTransform itemRect = itemObject.GetComponent<RectTransform>();
        itemRect.anchorMin = new Vector2(0f, 1f);
        itemRect.anchorMax = new Vector2(1f, 1f);
        itemRect.pivot = new Vector2(0.5f, 1f);
        itemRect.anchoredPosition = Vector2.zero;
        itemRect.sizeDelta = new Vector2(0f, 30f);
        Image itemImage = Undo.AddComponent<Image>(itemObject);
        itemImage.color = new Color(0.14f, 0.18f, 0.30f, 1f);
        Toggle itemToggle = Undo.AddComponent<Toggle>(itemObject);
        itemToggle.targetGraphic = itemImage;
        itemToggle.onValueChanged = new Toggle.ToggleEvent();

        TextMeshProUGUI itemLabel = CreateText(
            "Item Label", itemObject.transform, "Altar", 17f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), Vector2.one
        );
        Stretch(itemLabel.rectTransform);

        scrollRect.viewport = viewportRect;
        scrollRect.content = contentRect;
        dropdown.template = templateRect;
        dropdown.captionText = caption;
        dropdown.itemText = itemLabel;
        templateObject.SetActive(false);
        return dropdown;
    }

    private static TextMeshProUGUI CreateText(
        string name,
        Transform parent,
        string text,
        float fontSize,
        TextAlignmentOptions alignment,
        Vector2 anchor,
        Vector2 size
    )
    {
        GameObject textObject = CreateUIObject(name, parent);
        TextMeshProUGUI label = Undo.AddComponent<TextMeshProUGUI>(textObject);
        label.text = text;
        label.fontSize = fontSize;
        label.alignment = alignment;
        label.color = new Color(0.90f, 0.93f, 1f, 1f);
        label.textWrappingMode = TextWrappingModes.Normal;

        RectTransform rect = label.rectTransform;
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(1100f * size.x, 650f * size.y);
        return label;
    }

    private static GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject result = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
        Undo.RegisterCreatedObjectUndo(result, "Create " + name);
        result.transform.SetParent(parent, false);
        return result;
    }

    private static void ClearGeneratedChildren(Transform panel)
    {
        for (int i = panel.childCount - 1; i >= 0; i--)
        {
            Transform child = panel.GetChild(i);
            if (child.name.StartsWith("D2_"))
                Undo.DestroyObjectImmediate(child.gameObject);
        }
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
    }

    private static bool Require(Object value, string label)
    {
        if (value != null)
            return true;

        Debug.LogError("[D2 Block 1] Referencia faltante: " + label);
        return false;
    }

    private static bool RequireArray<T>(T[] values, int expectedLength, string label)
        where T : Object
    {
        if (values != null && values.Length == expectedLength)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == null)
                {
                    Debug.LogError("[D2 Block 1] Referencia faltante: " +
                        label + "[" + i + "]");
                    return false;
                }
            }
            return true;
        }

        Debug.LogError("[D2 Block 1] Arreglo inválido: " + label +
            " (esperado " + expectedLength + ").");
        return false;
    }

    private static bool ValidateDirectChildrenInside(
        RectTransform container,
        string label
    )
    {
        if (container == null || container.rect.width <= 0f || container.rect.height <= 0f)
        {
            Debug.LogError("[D2 Layout] Contenedor sin tamaño válido: " + label + ".");
            return false;
        }

        Rect bounds = container.rect;
        Vector3[] worldCorners = new Vector3[4];
        const float tolerance = 1f;
        bool valid = true;
        for (int i = 0; i < container.childCount; i++)
        {
            RectTransform child = container.GetChild(i) as RectTransform;
            if (child == null)
                continue;
            if (label == "Dimensión 2 general" && child.name == "D2_FirstEntry")
            {
                Canvas overlayCanvas = child.GetComponent<Canvas>();
                if (overlayCanvas != null && overlayCanvas.overrideSorting)
                    continue;
            }

            child.GetWorldCorners(worldCorners);
            for (int cornerIndex = 0; cornerIndex < worldCorners.Length; cornerIndex++)
                worldCorners[cornerIndex] = container.InverseTransformPoint(worldCorners[cornerIndex]);

            float minX = Mathf.Min(worldCorners[0].x, worldCorners[2].x);
            float maxX = Mathf.Max(worldCorners[0].x, worldCorners[2].x);
            float minY = Mathf.Min(worldCorners[0].y, worldCorners[2].y);
            float maxY = Mathf.Max(worldCorners[0].y, worldCorners[2].y);
            if (minX < bounds.xMin - tolerance || maxX > bounds.xMax + tolerance ||
                minY < bounds.yMin - tolerance || maxY > bounds.yMax + tolerance)
            {
                Debug.LogError(
                    "[D2 Layout] " + label + "/" + child.name +
                    " queda fuera de su panel."
                );
                valid = false;
            }
        }

        if (valid)
            Debug.Log("[D2 Layout] " + label + ": controles dentro del panel.");
        return valid;
    }

    private static bool ValidateDimension2Block5FPresentation(Dimension2PanelUI panel)
    {
        if (panel == null)
            return false;
        bool valid = true;
        string[] damagedMarkers = { "Ã", "Â", "�" };
        string[] unaccentedTokens =
        {
            "CONTENCION", "INVESTIGACION", "MEJORAR LINEA", "NIVEL MAXIMO"
        };
        TMP_Text[] texts = panel.GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text textComponent in texts)
        {
            if (textComponent == null || string.IsNullOrEmpty(textComponent.text))
                continue;
            foreach (string marker in damagedMarkers)
            {
                if (!textComponent.text.Contains(marker))
                    continue;
                Debug.LogError(
                    "[D2 Block 5F] Texto dañado en " + textComponent.name +
                    ": " + textComponent.text
                );
                valid = false;
            }
            foreach (string token in unaccentedTokens)
            {
                if (!textComponent.text.Contains(token))
                    continue;
                Debug.LogError(
                    "[D2 Block 5F] Texto sin localizar en " + textComponent.name +
                    ": " + token
                );
                valid = false;
            }
        }
        if (valid)
        {
            Debug.Log(
                "[D2 Block 5F] VALIDACIÓN DE PRESENTACIÓN OK: referencias, " +
                "límites de panel, acentos y textos principales."
            );
        }
        return valid;
    }

    private static bool ValidateCivilization1Logic(GameState gameState)
    {
        string originalStateJson = JsonUtility.ToJson(gameState.dimension2);
        bool originalDimension2Unlocked = gameState.dimension02Unlocked;

        try
        {
            gameState.dimension02Unlocked = true;
            Dimension2System.ResetState(gameState);
            D2Civilization1State state = gameState.dimension2.civilization1;

            if (state.followersAvailable != D2Civilization1System.InitialFollowers)
                return FailCivilization1Validation("paquete inicial incorrecto");

            if (state.altars == null || state.altars.Count != D2AltarSystem.AltarIds.Length)
                return FailCivilization1Validation("catálogo inicial de Altares incorrecto");

            D2AltarState waxAltar = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
            D2AltarState breadAltar = D2AltarSystem.GetAltar(
                state,
                D2AltarSystem.RitualBreadAltarId
            );
            D2AltarState incenseAltar = D2AltarSystem.GetAltar(
                state,
                D2AltarSystem.IncenseAltarId
            );
            if (waxAltar == null || breadAltar == null || incenseAltar == null ||
                !waxAltar.unlocked || !breadAltar.unlocked || incenseAltar.unlocked)
            {
                return FailCivilization1Validation("desbloqueo inicial de Altares incorrecto");
            }

            D2Civilization1System.ApplyOfflineProgress(gameState, 20.0);
            if (state.followersAvailable != D2Civilization1System.InitialFollowers + 1L)
                return FailCivilization1Validation("producción base de 20 segundos incorrecta");

            long totalBeforeAssignment = D2Civilization1System.GetTotalFollowers(state);
            if (!D2Civilization1System.TryAssignFollowersToRefuge(gameState, 1L))
                return FailCivilization1Validation("no se pudo asignar un Seguidor");

            if (D2Civilization1System.GetTotalFollowers(state) != totalBeforeAssignment)
                return FailCivilization1Validation("la asignación alteró el total de Seguidores");

            double boostedRate = D2Civilization1System.GetFollowerArrivalPerSecond(state);
            if (boostedRate <= D2Civilization1System.BaseFollowerArrivalPerSecond)
                return FailCivilization1Validation("la asignación no mejoró la llegada");

            if (!D2Civilization1System.TryReleaseAllFollowersFromRefuge(gameState))
                return FailCivilization1Validation("no se pudieron retirar los Seguidores");

            long totalBeforeAltarAssignment = D2Civilization1System.GetTotalFollowers(state);
            if (!D2AltarSystem.TryAssignFollowers(
                gameState,
                D2AltarSystem.WaxAltarId,
                1L
            ))
            {
                return FailCivilization1Validation("no se pudo asignar al Altar de Cera");
            }

            if (D2Civilization1System.GetTotalFollowers(state) != totalBeforeAltarAssignment)
                return FailCivilization1Validation("el Altar alteró el total de Seguidores");

            if (D2AltarSystem.GetOfferingPerSecond(waxAltar) <=
                D2AltarSystem.BaseOfferingPerSecond)
            {
                return FailCivilization1Validation("la asignación no mejoró el Altar");
            }

            if (D2AltarSystem.TryAssignFollowers(
                gameState,
                D2AltarSystem.IncenseAltarId,
                1L
            ))
            {
                return FailCivilization1Validation("un Altar bloqueado aceptó Seguidores");
            }

            double waxBefore = waxAltar.offeringAmount;
            double breadBefore = breadAltar.offeringAmount;
            D2Civilization1System.ApplyOfflineProgress(gameState, 20.0);
            double waxProduced = waxAltar.offeringAmount - waxBefore;
            double breadProduced = breadAltar.offeringAmount - breadBefore;
            if (waxProduced <= 1.0 || System.Math.Abs(breadProduced - 1.0) > 0.000001)
                return FailCivilization1Validation("producción online/offline de Ofrendas incorrecta");

            if (!D2AltarSystem.TryReleaseAllFollowers(
                gameState,
                D2AltarSystem.WaxAltarId
            ))
            {
                return FailCivilization1Validation("no se pudo liberar el Altar de Cera");
            }

            state.followersAvailable = 100L;
            long upgradeCost = D2Civilization1System.GetNextRefugeUpgradeCost(state);
            if (!D2Civilization1System.TryUpgradeRefuge(gameState) ||
                state.refugeLevel != 2 ||
                state.followersAvailable != 100L - upgradeCost)
            {
                return FailCivilization1Validation("mejora de Refugio incorrecta");
            }

            waxAltar.offeringAmount = 100.0;
            breadAltar.offeringAmount = 100.0;
            long totalBeforePilgrimage = D2Civilization1System.GetTotalFollowers(state);
            if (!D2PilgrimageSystem.TryStart(gameState, D2PilgrimageSystem.ShortId))
                return FailCivilization1Validation("no se pudo iniciar la Peregrinación Corta");

            if (D2Civilization1System.GetTotalFollowers(state) != totalBeforePilgrimage ||
                state.activePilgrimage.followersCommitted != 1L ||
                System.Math.Abs(waxAltar.offeringAmount - 98.0) > 0.000001 ||
                System.Math.Abs(breadAltar.offeringAmount - 98.0) > 0.000001)
            {
                return FailCivilization1Validation("costes u ocupación de Peregrinación incorrectos");
            }

            if (D2PilgrimageSystem.TryStart(gameState, D2PilgrimageSystem.MediumId))
                return FailCivilization1Validation("se inició una segunda Peregrinación simultánea");

            D2Civilization1System.ApplyOfflineProgress(gameState, 60.0);
            if (state.activePilgrimage.active || state.trust != 1.0 ||
                state.shortPilgrimagesCompleted != 1L)
            {
                return FailCivilization1Validation("finalización offline de Peregrinación incorrecta");
            }

            D2Civilization1System.ApplyOfflineProgress(gameState, 60.0);
            if (state.trust != 1.0 || state.shortPilgrimagesCompleted != 1L)
                return FailCivilization1Validation("recompensa de Peregrinación duplicada");

            waxAltar.offeringAmount = 100.0;
            breadAltar.offeringAmount = 100.0;
            long followersBeforeCancel = state.followersAvailable;
            if (!D2PilgrimageSystem.TryStart(gameState, D2PilgrimageSystem.MediumId))
                return FailCivilization1Validation("no se pudo iniciar la Peregrinación Media");

            double waxAfterMediumStart = waxAltar.offeringAmount;
            if (!D2PilgrimageSystem.TryCancel(gameState) ||
                state.followersAvailable != followersBeforeCancel ||
                waxAltar.offeringAmount != waxAfterMediumStart)
            {
                return FailCivilization1Validation("cancelación de Peregrinación incorrecta");
            }

            state.trust = D2PilgrimageSystem.Civilization2UnlockTrust;
            D2PilgrimageSystem.EnsureState(gameState);
            if (!gameState.dimension2.civilization2Unlocked)
                return FailCivilization1Validation("Civ 2 no se desbloqueó a 300 de Confianza");

            state.trust = D2PilgrimageSystem.EntityContactTrust;
            D2PilgrimageSystem.EnsureState(gameState);
            if (!state.entityContactAvailable)
                return FailCivilization1Validation("contacto con el Ente no se habilitó a 500");

            state.followersAvailable = 1000L;
            waxAltar.offeringAmount = 1000.0;
            breadAltar.offeringAmount = 1000.0;
            long followersBeforeTraining = D2Civilization1System.GetTotalFollowers(state);
            if (!D2NovitiateSystem.TryStartTraining(gameState))
                return FailCivilization1Validation("no se pudo iniciar la tanda del Noviciado");

            if (D2Civilization1System.GetTotalFollowers(state) != followersBeforeTraining ||
                state.activeNovitiateTraining.followersCommitted != 5L ||
                System.Math.Abs(waxAltar.offeringAmount - 988.0) > 0.000001)
            {
                return FailCivilization1Validation("costes u ocupación del Noviciado incorrectos");
            }

            D2Civilization1System.ApplyOfflineProgress(gameState, 300.0);
            if (state.activeNovitiateTraining.active || state.acolytesAvailable != 1L ||
                state.novitiateBatchesCompleted != 1L)
            {
                return FailCivilization1Validation("formación offline de Acólitos incorrecta");
            }

            state.trust = 0.0;
            waxAltar.offeringAmount = 1000.0;
            breadAltar.offeringAmount = 1000.0;
            long acolytesBeforeGuided = D2NovitiateSystem.GetTotalAcolytes(state);
            if (!D2PilgrimageSystem.TryStart(gameState, D2PilgrimageSystem.GuidedLongId) ||
                D2NovitiateSystem.GetTotalAcolytes(state) != acolytesBeforeGuided)
            {
                return FailCivilization1Validation("Peregrinación con Acólito incorrecta");
            }

            D2Civilization1System.ApplyOfflineProgress(gameState, 600.0);
            if (state.trust != 16.0 || state.acolytesAvailable != acolytesBeforeGuided)
                return FailCivilization1Validation("retorno o recompensa del Acólito incorrectos");

            waxAltar.offeringAmount = 1000.0;
            breadAltar.offeringAmount = 1000.0;
            long followersBeforeTrainingCancel = state.followersAvailable;
            if (!D2NovitiateSystem.TryStartTraining(gameState))
                return FailCivilization1Validation("no se pudo iniciar tanda para cancelación");
            double waxAfterTrainingStart = waxAltar.offeringAmount;
            if (!D2NovitiateSystem.TryCancelTraining(gameState) ||
                state.followersAvailable != followersBeforeTrainingCancel ||
                waxAltar.offeringAmount != waxAfterTrainingStart)
            {
                return FailCivilization1Validation("cancelación del Noviciado incorrecta");
            }

            state.followersAvailable = 1000L;
            waxAltar.offeringAmount = 1000.0;
            breadAltar.offeringAmount = 1000.0;
            if (!D2NovitiateSystem.TryUpgrade(gameState) || state.novitiateLevel != 2)
                return FailCivilization1Validation("mejora del Noviciado incorrecta");

            state.followersAvailable = 1000L;
            state.acolytesAvailable = 100L;
            state.totalAcolytesCreated = 100L;
            waxAltar.offeringAmount = 1000.0;
            breadAltar.offeringAmount = 1000.0;
            D2RiteSystem.EnsureState(state);
            if (!D2RiteSystem.AreRitesUnlocked(state) ||
                state.rites.Count != D2RiteSystem.RiteIds.Length)
            {
                return FailCivilization1Validation("desbloqueo o catálogo de Ritos incorrecto");
            }

            double arrivalBeforeRite = D2Civilization1System.GetFollowerArrivalPerSecond(state);
            if (!D2RiteSystem.TryAssignFollowers(
                gameState,
                D2RiteSystem.WelcomeId,
                4L
            ) || D2Civilization1System.GetFollowerArrivalPerSecond(state) <= arrivalBeforeRite)
            {
                return FailCivilization1Validation("Rito de Recibimiento no aplicó su bonus");
            }

            if (!D2RiteSystem.TryAssignAcolytes(
                gameState,
                D2RiteSystem.OfferingId,
                1L
            ))
            {
                return FailCivilization1Validation("no se activó el segundo Rito");
            }

            if (D2RiteSystem.TryAssignFollowers(gameState, D2RiteSystem.PathId, 1L))
                return FailCivilization1Validation("se permitió un tercer Rito sin espacio");

            double altarBaseRate = D2AltarSystem.GetOfferingPerSecond(waxAltar);
            if (D2AltarSystem.GetOfferingPerSecond(state, waxAltar) <= altarBaseRate)
                return FailCivilization1Validation("Rito de Ofrenda no aplicó su bonus");

            if (!D2RiteSystem.TryReleaseAll(gameState, D2RiteSystem.OfferingId) ||
                !D2RiteSystem.TryAssignAcolytes(gameState, D2RiteSystem.PathId, 1L))
            {
                return FailCivilization1Validation("liberación o cambio de Rito incorrecto");
            }

            state.trust = 0.0;
            waxAltar.offeringAmount = 1000.0;
            breadAltar.offeringAmount = 1000.0;
            if (!D2PilgrimageSystem.TryStart(gameState, D2PilgrimageSystem.ShortId))
                return FailCivilization1Validation("no se inició prueba del Rito del Camino");
            D2PilgrimageSystem.Tick(gameState, 56.4);
            if (!state.activePilgrimage.active)
                return FailCivilization1Validation("Rito del Camino alteró la duración");
            double waxBeforePathReward = waxAltar.offeringAmount;
            D2PilgrimageSystem.Tick(gameState, 4.0);
            if (state.activePilgrimage.active ||
                waxAltar.offeringAmount <= waxBeforePathReward + 1.0)
            {
                return FailCivilization1Validation(
                    "Rito del Camino no mejoró la recompensa material"
                );
            }

            D2RiteSystem.TryReleaseAll(gameState, D2RiteSystem.PathId);
            if (!D2RiteSystem.TryAssignAcolytes(
                gameState,
                D2RiteSystem.NovitiateId,
                1L
            ) || !D2NovitiateSystem.TryStartTraining(gameState))
            {
                return FailCivilization1Validation("no se inició prueba del Rito de Noviciado");
            }
            D2Civilization1System.ApplyOfflineProgress(gameState, 339.0);
            if (state.activeNovitiateTraining.active)
                return FailCivilization1Validation("Rito de Noviciado no redujo la duración offline");

            D2RiteSystem.TryReleaseAll(gameState, D2RiteSystem.NovitiateId);
            if (!D2RiteSystem.TryAssignFollowers(gameState, D2RiteSystem.RespectId, 4L))
                return FailCivilization1Validation("no se activó el Rito de Respeto");
            double trustBeforeRespect = state.trust;
            if (!D2PilgrimageSystem.TryStart(gameState, D2PilgrimageSystem.ShortId))
                return FailCivilization1Validation("no se inició prueba del Rito de Respeto");
            D2PilgrimageSystem.Tick(gameState, 60.0);
            if (state.trust <= trustBeforeRespect + 1.0)
                return FailCivilization1Validation("Rito de Respeto no aumentó la Confianza");

            state.trust = D2RiteSystem.ThirdSlotTrustRequired;
            state.novitiateLevel = D2RiteSystem.ThirdSlotNovitiateLevelRequired;
            state.acolytesAvailable = 10L;
            waxAltar.offeringAmount = 500.0;
            breadAltar.offeringAmount = 500.0;
            if (!D2RiteSystem.TryUnlockThirdSlot(gameState) ||
                !state.thirdRiteSlotUnlocked || state.acolytesAvailable != 5L ||
                System.Math.Abs(waxAltar.offeringAmount - 350.0) > 0.000001 ||
                System.Math.Abs(breadAltar.offeringAmount - 350.0) > 0.000001)
            {
                return FailCivilization1Validation("desbloqueo o coste del tercer espacio incorrecto");
            }

            if (!D2RiteSystem.TryAssignFollowers(gameState, D2RiteSystem.PathId, 1L) ||
                D2RiteSystem.GetActiveRiteCount(state) != 3)
            {
                return FailCivilization1Validation("el tercer espacio no permitió otro Rito");
            }

            foreach (string riteId in D2RiteSystem.RiteIds)
                D2RiteSystem.TryReleaseAll(gameState, riteId);

            state.trust = D2CivilizationPactSystem.UnlockTrustRequired;
            state.novitiateLevel = D2CivilizationPactSystem.UnlockNovitiateLevelRequired;
            state.followersAvailable = 1000L;
            state.acolytesAvailable = 100L;
            waxAltar.offeringAmount = 1000.0;
            breadAltar.offeringAmount = 1000.0;
            D2CivilizationPactSystem.EnsureState(state);
            if (!D2CivilizationPactSystem.ArePactsUnlocked(state) ||
                state.civilizationPacts.Count != D2CivilizationPactSystem.PactIds.Length)
            {
                return FailCivilization1Validation("desbloqueo o catálogo de Pactos incorrecto");
            }

            double arrivalWithoutPact = D2Civilization1System
                .GetFollowerArrivalPerSecond(state);
            if (!D2CivilizationPactSystem.TryActivate(
                gameState,
                D2CivilizationPactSystem.HospitalityId
            ) || D2Civilization1System.GetFollowerArrivalPerSecond(state) <= arrivalWithoutPact)
            {
                return FailCivilization1Validation("Pacto de Hospedaje no aplicó su beneficio");
            }

            if (D2CivilizationPactSystem.TryActivate(
                gameState,
                D2CivilizationPactSystem.OpenPathId
            ))
            {
                return FailCivilization1Validation("se permitió un segundo Pacto sin espacio");
            }

            breadAltar.offeringAmount = 2.0;
            double supported = D2CivilizationPactSystem
                .ConsumeHospitalityMaintenance(state, 60.0);
            if (System.Math.Abs(supported - 60.0) > 0.000001 ||
                System.Math.Abs(breadAltar.offeringAmount - 1.0) > 0.000001)
            {
                return FailCivilization1Validation("mantenimiento de Hospedaje incorrecto");
            }

            supported = D2CivilizationPactSystem
                .ConsumeHospitalityMaintenance(state, 120.0);
            D2CivilizationPactState hospitality = D2CivilizationPactSystem.GetPact(
                state,
                D2CivilizationPactSystem.HospitalityId
            );
            if (System.Math.Abs(supported - 60.0) > 0.000001 ||
                !hospitality.suspended || breadAltar.offeringAmount != 0.0)
            {
                return FailCivilization1Validation("suspensión de Hospedaje incorrecta");
            }

            breadAltar.offeringAmount = 2.0;
            D2CivilizationPactSystem.ConsumeHospitalityMaintenance(state, 60.0);
            if (hospitality.suspended ||
                !D2CivilizationPactSystem.TryCancel(
                    gameState,
                    D2CivilizationPactSystem.HospitalityId
                ))
            {
                return FailCivilization1Validation("reanudación o cancelación de Hospedaje incorrecta");
            }

            state.trust = D2CivilizationPactSystem.UnlockTrustRequired;
            state.followersAvailable = 1000L;
            waxAltar.offeringAmount = 1000.0;
            breadAltar.offeringAmount = 1000.0;
            if (!D2CivilizationPactSystem.TryActivate(
                gameState,
                D2CivilizationPactSystem.OpenPathId
            ) || !D2PilgrimageSystem.TryStart(gameState, D2PilgrimageSystem.ShortId))
            {
                return FailCivilization1Validation("no se inició prueba de Camino Abierto");
            }
            if (System.Math.Abs(waxAltar.offeringAmount - 907.5) > 0.000001)
                return FailCivilization1Validation("compromiso de Camino Abierto incorrecto");
            D2PilgrimageSystem.Tick(gameState, 60.0);
            if (System.Math.Abs(state.trust - 201.25) > 0.000001 ||
                System.Math.Abs(waxAltar.offeringAmount - 908.75) > 0.000001)
            {
                return FailCivilization1Validation("recompensas de Camino Abierto incorrectas");
            }
            D2CivilizationPactSystem.TryCancel(
                gameState,
                D2CivilizationPactSystem.OpenPathId
            );

            state.followersAvailable = 1000L;
            state.acolytesAvailable = 100L;
            waxAltar.offeringAmount = 1000.0;
            breadAltar.offeringAmount = 1000.0;
            if (!D2CivilizationPactSystem.TryActivate(
                gameState,
                D2CivilizationPactSystem.ConsecrationId
            ))
            {
                return FailCivilization1Validation("no se activó Consagración");
            }
            long acolytesBeforeConsecration = state.acolytesAvailable;
            if (!D2NovitiateSystem.TryStartTraining(gameState))
                return FailCivilization1Validation("no se inició tanda con Consagración");
            D2NovitiateSystem.Tick(gameState, 469.0);
            if (!D2NovitiateSystem.TryStartTraining(gameState))
                return FailCivilization1Validation("no se inició segunda tanda con Consagración");
            D2NovitiateSystem.Tick(gameState, 469.0);
            if (state.acolytesAvailable != acolytesBeforeConsecration + 5L ||
                System.Math.Abs(state.pactConsecrationAcolyteProgress) > 0.000001)
            {
                return FailCivilization1Validation("beneficio fraccional de Consagración incorrecto");
            }
            D2CivilizationPactSystem.TryCancel(
                gameState,
                D2CivilizationPactSystem.ConsecrationId
            );

            double arrivalBeforeSilentVow = D2Civilization1System
                .GetFollowerArrivalPerSecond(state);
            waxAltar.offeringAmount = 1000.0;
            breadAltar.offeringAmount = 1000.0;
            if (!D2CivilizationPactSystem.TryActivate(
                gameState,
                D2CivilizationPactSystem.SilentVowId
            ) || D2Civilization1System.GetFollowerArrivalPerSecond(state) >=
                arrivalBeforeSilentVow ||
                System.Math.Abs(
                    D2PilgrimageSystem.GetEffectiveTrustReward(
                        state,
                        D2PilgrimageSystem.ShortId
                    ) - 1.5
                ) > 0.000001)
            {
                return FailCivilization1Validation("Voto Silencioso aplicó efectos incorrectos");
            }
            D2CivilizationPactSystem.TryCancel(
                gameState,
                D2CivilizationPactSystem.SilentVowId
            );

            state.followersAvailable = 1000L;
            if (!D2RiteSystem.TryAssignFollowers(
                gameState,
                D2RiteSystem.WelcomeId,
                4L
            ))
            {
                return FailCivilization1Validation("no se preparó Rito para Puerta Interior");
            }
            double riteBeforeInnerDoor = D2RiteSystem.GetBonusFraction(
                state,
                D2RiteSystem.WelcomeId
            );
            state.entityContactAvailable = true;
            state.bondPlacePrepared = true;
            state.acolytesAssignedToBond = 1L;
            state.bondProgress = 0.0;
            waxAltar.offeringAmount = 1000.0;
            breadAltar.offeringAmount = 1000.0;
            if (!D2CivilizationPactSystem.TryActivate(
                gameState,
                D2CivilizationPactSystem.InnerDoorId
            ) || System.Math.Abs(
                D2RiteSystem.GetBonusFraction(state, D2RiteSystem.WelcomeId) -
                riteBeforeInnerDoor
            ) > 0.000001 || D2NovitiateSystem.CanStartTraining(gameState) ||
                D2NovitiateSystem.CanUpgrade(gameState))
            {
                return FailCivilization1Validation("Puerta Interior aplicó efectos incorrectos");
            }
            D2BondSystem.Tick(state, 60.0);
            if (System.Math.Abs(state.bondProgress - 1.25) > 0.000001)
                return FailCivilization1Validation("Puerta Interior no mejoró el vínculo");
            D2CivilizationPactSystem.TryCancel(
                gameState,
                D2CivilizationPactSystem.InnerDoorId
            );
            D2RiteSystem.TryReleaseAll(gameState, D2RiteSystem.WelcomeId);

            state.trust = D2CivilizationPactSystem.SecondSlotTrustRequired;
            state.novitiateLevel = D2CivilizationPactSystem
                .SecondSlotNovitiateLevelRequired;
            state.acolytesAvailable = 20L;
            waxAltar.offeringAmount = 1000.0;
            breadAltar.offeringAmount = 1000.0;
            if (!D2CivilizationPactSystem.TryUnlockSecondSlot(gameState) ||
                !state.secondCivilizationPactSlotUnlocked ||
                state.acolytesAvailable != 10L ||
                System.Math.Abs(waxAltar.offeringAmount - 700.0) > 0.000001 ||
                System.Math.Abs(breadAltar.offeringAmount - 700.0) > 0.000001)
            {
                return FailCivilization1Validation("desbloqueo del segundo espacio incorrecto");
            }
            if (!D2CivilizationPactSystem.TryActivate(
                    gameState,
                    D2CivilizationPactSystem.HospitalityId
                ) || !D2CivilizationPactSystem.TryActivate(
                    gameState,
                    D2CivilizationPactSystem.SilentVowId
                ) || D2CivilizationPactSystem.GetActivePactCount(state) != 2 ||
                D2CivilizationPactSystem.TryActivate(
                    gameState,
                    D2CivilizationPactSystem.OpenPathId
                ))
            {
                return FailCivilization1Validation("simultaneidad del segundo espacio incorrecta");
            }

            state.followersAvailable = 1000L;
            waxAltar.offeringAmount = 1000.0;
            breadAltar.offeringAmount = 1000.0;
            long followersBeforeSupportTests = state.followersAvailable;
            if (!D2PilgrimageSystem.TryChangeSupportFollowers(gameState, 4L) ||
                !D2PilgrimageSystem.TryStart(gameState, D2PilgrimageSystem.ShortId) ||
                state.activePilgrimage.supportFollowersCommitted != 4L ||
                !D2PilgrimageSystem.TryCancel(gameState) ||
                state.followersAvailable != followersBeforeSupportTests)
            {
                return FailCivilization1Validation("apoyo adicional de Peregrinación incorrecto");
            }
            D2PilgrimageSystem.TryChangeSupportFollowers(gameState, -4L);

            followersBeforeSupportTests = state.followersAvailable;
            if (!D2NovitiateSystem.TryChangeSupportFollowers(gameState, 4L) ||
                !D2NovitiateSystem.TryStartTraining(gameState) ||
                state.activeNovitiateTraining.supportFollowersCommitted != 4L ||
                !D2NovitiateSystem.TryCancelTraining(gameState) ||
                state.followersAvailable != followersBeforeSupportTests)
            {
                return FailCivilization1Validation("apoyo adicional de Noviciado incorrecto");
            }
            D2NovitiateSystem.TryChangeSupportFollowers(gameState, -4L);

            string roundTripJson = JsonUtility.ToJson(gameState.dimension2);
            Dimension2State roundTrip = JsonUtility.FromJson<Dimension2State>(roundTripJson);
            gameState.dimension2 = roundTrip;
            gameState.EnsureDimension2State();

            if (!D2Civilization1System.ValidateState(
                gameState.dimension2.civilization1,
                out string result
            ))
            {
                return FailCivilization1Validation("round-trip inválido: " + result);
            }

            if (!D2PilgrimageSystem.ValidateState(gameState, out result))
                return FailCivilization1Validation("round-trip 2C inválido: " + result);

            if (!D2NovitiateSystem.ValidateState(
                gameState.dimension2.civilization1,
                out result
            ))
            {
                return FailCivilization1Validation("round-trip 2D inválido: " + result);
            }

            if (!D2RiteSystem.ValidateState(gameState.dimension2.civilization1, out result))
                return FailCivilization1Validation("round-trip 2E inválido: " + result);

            if (!D2CivilizationPactSystem.ValidateState(
                gameState.dimension2.civilization1,
                out result
            ))
            {
                return FailCivilization1Validation("round-trip 2F inválido: " + result);
            }

            state = gameState.dimension2.civilization1;
            state.trust = D2VeiledThresholdSystem.UnlockTrustRequired - 1.0;
            state.entityContactAvailable = false;
            D2VeiledThresholdSystem.EnsureState(state);
            if (D2VeiledThresholdSystem.IsUnlocked(state))
                return FailCivilization1Validation("Umbral desbloqueado antes de 500 de Confianza");

            state.trust = D2VeiledThresholdSystem.UnlockTrustRequired;
            D2VeiledThresholdSystem.EnsureState(state);
            if (!D2VeiledThresholdSystem.IsUnlocked(state))
                return FailCivilization1Validation("Umbral no desbloqueado a 500 de Confianza");

            state.trust = 0.0;
            if (!D2VeiledThresholdSystem.IsUnlocked(state) ||
                !D2VeiledThresholdSystem.ValidateState(state, out result))
            {
                return FailCivilization1Validation("desbloqueo permanente o estado 2G inválido");
            }

            D2AltarSystem.EnsureState(state);
            D2AltarState incense = D2AltarSystem.GetAltar(
                state,
                D2AltarSystem.IncenseAltarId
            );
            D2AltarState cloth = D2AltarSystem.GetAltar(
                state,
                D2AltarSystem.SacredClothAltarId
            );
            D2AltarState stone = D2AltarSystem.GetAltar(
                state,
                D2AltarSystem.CarvedStoneAltarId
            );
            if (incense == null || cloth == null || stone == null ||
                !incense.unlocked || !cloth.unlocked || !stone.unlocked)
            {
                return FailCivilization1Validation("desbloqueo de Altares avanzados incorrecto");
            }

            state.bondPlacePrepared = false;
            state.acolytesAssignedToBond = 0L;
            state.bondProgress = 0.0;
            state.acolytesAvailable = System.Math.Max(1L, state.acolytesAvailable);
            incense.offeringAmount = 500.0;
            cloth.offeringAmount = 500.0;
            stone.offeringAmount = 500.0;
            if (!D2BondSystem.TryPrepare(gameState) ||
                !D2BondSystem.TryAssignAcolytes(gameState, 1L))
            {
                return FailCivilization1Validation("preparación del Lugar de Vínculo incorrecta");
            }
            D2BondSystem.Tick(state, 60.0);
            if (System.Math.Abs(state.bondProgress - 1.0) > 0.000001)
                return FailCivilization1Validation("progreso básico del vínculo incorrecto");

            state.bondProgress = 200.0;
            if (!D2BondSystem.TryUpgrade(gameState, D2BondSystem.PilgrimPathId) ||
                !D2BondSystem.TryUpgrade(gameState, D2BondSystem.SanctuaryEchoId) ||
                !D2BondSystem.TryUpgrade(gameState, D2BondSystem.TraceLiturgyId) ||
                System.Math.Abs(gameState.GetDimension2SanctuaryLEMultiplier() - 1.01) > 0.000001 ||
                System.Math.Abs(gameState.GetDimension2TraceMultiplier() - 1.01) > 0.000001 ||
                !D2BondSystem.ValidateState(state, out result))
            {
                return FailCivilization1Validation("líneas o beneficios del vínculo incorrectos");
            }

            string thresholdRoundTripJson = JsonUtility.ToJson(gameState.dimension2);
            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(thresholdRoundTripJson);
            gameState.EnsureDimension2State();
            if (!D2VeiledThresholdSystem.IsUnlocked(gameState.dimension2.civilization1))
                return FailCivilization1Validation("serialización del desbloqueo 2G inválida");

            Debug.Log(
                "[D2 Block 2A] VALIDACIÓN LÓGICA OK: producción, asignación, " +
                "mejora y serialización."
            );
            Debug.Log(
                "[D2 Block 2B] VALIDACIÓN LÓGICA OK: cinco Altares, bloqueos, " +
                "Ofrendas separadas, asignación, progreso offline y serialización."
            );
            Debug.Log(
                "[D2 Block 2C] VALIDACIÓN LÓGICA OK: costes, ocupación, actividad única, " +
                "cancelación, progreso offline, antirrepetición, Confianza y umbrales."
            );
            Debug.Log(
                "[D2 Block 2D] VALIDACIÓN LÓGICA OK: tandas, conversión, cancelación, " +
                "mejoras, progreso offline y Peregrinaciones con Acólitos."
            );
            Debug.Log(
                "[D2 Block 2E] VALIDACIÓN LÓGICA OK: catálogo, espacios, asignaciones, " +
                "límites, efectos, tercer espacio, progreso offline y serialización."
            );
            Debug.Log(
                "[D2 Block 2F] VALIDACIÓN LÓGICA OK: catálogo, costes, espacios, " +
                "mantenimiento, suspensión, cancelación, cinco efectos y serialización."
            );
            Debug.Log(
                "[D2 Block 2G] VALIDACIÓN LÓGICA OK: umbral de Confianza, " +
                "desbloqueo permanente, presentación y serialización."
            );
            return true;
        }
        finally
        {
            gameState.dimension02Unlocked = originalDimension2Unlocked;
            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(originalStateJson);
            gameState.EnsureDimension2State();
        }
    }

    private static bool ValidateCivilization2Block3ALogic(GameState gameState)
    {
        string originalStateJson = JsonUtility.ToJson(gameState.dimension2);
        bool originalDimension2Unlocked = gameState.dimension02Unlocked;

        try
        {
            gameState.dimension02Unlocked = true;
            Dimension2System.ResetState(gameState);
            gameState.dimension2.civilization2Unlocked = true;
            D2Civilization2State state = gameState.dimension2.civilization2;
            D2Civilization2System.EnsureState(state);

            if (state.membersAvailable != D2Civilization2System.InitialMembers ||
                D2Civilization2System.GetTotalMembers(state) !=
                D2Civilization2System.InitialMembers)
            {
                return FailCivilization2Validation("paquete inicial de Miembros incorrecto");
            }

            if (state.regions == null ||
                state.regions.Count != D2Civilization2System.RegionIds.Length)
            {
                return FailCivilization2Validation("catálogo regional incorrecto");
            }

            D2RegionState region1 = D2Civilization2System.GetRegion(
                state,
                D2Civilization2System.Region1Id
            );
            if (region1 == null || !region1.unlocked ||
                region1.dominance != D2Civilization2System.InitialDominance ||
                region1.threat != D2Civilization2System.InitialThreat ||
                D2Civilization2System.GetTotalDominance(state) != 100.0)
            {
                return FailCivilization2Validation("estado inicial de Región 1 incorrecto");
            }

            if (D2Civilization2System.TryAssignMembers(
                gameState,
                D2Civilization2System.Region2Id,
                1L
            ))
            {
                return FailCivilization2Validation("una región bloqueada aceptó Miembros");
            }

            if (!D2Civilization2System.TryAssignMembers(
                    gameState,
                    D2Civilization2System.Region1Id,
                    4L
                ) ||
                state.membersAvailable != 6L || region1.membersAssigned != 4L ||
                D2Civilization2System.GetTotalMembers(state) !=
                D2Civilization2System.InitialMembers)
            {
                return FailCivilization2Validation("asignación exclusiva a Región 1 incorrecta");
            }

            if (!D2Civilization2System.TryAssignAllMembers(
                    gameState,
                    D2Civilization2System.Region1Id
                ) ||
                state.membersAvailable != 0L || region1.membersAssigned != 10L)
            {
                return FailCivilization2Validation("asignación total incorrecta");
            }

            if (!D2Civilization2System.TryReleaseMembers(
                    gameState,
                    D2Civilization2System.Region1Id,
                    3L
                ) ||
                state.membersAvailable != 3L || region1.membersAssigned != 7L ||
                !D2Civilization2System.TryReleaseAllMembers(
                    gameState,
                    D2Civilization2System.Region1Id
                ) ||
                state.membersAvailable != 10L || region1.membersAssigned != 0L)
            {
                return FailCivilization2Validation("retorno de Miembros incorrecto");
            }

            string serialized = JsonUtility.ToJson(state);
            D2Civilization2State restored = JsonUtility.FromJson<D2Civilization2State>(serialized);
            if (!D2Civilization2System.ValidateState(restored, out _ ) ||
                restored.membersAvailable != 10L || restored.regions.Count != 4)
            {
                return FailCivilization2Validation("serialización de estado incorrecta");
            }

            D2Civilization2State migrated = JsonUtility.FromJson<D2Civilization2State>(
                "{\"progressVersion\":1}"
            );
            D2Civilization2System.EnsureState(migrated);
            if (migrated.membersAvailable != D2Civilization2System.InitialMembers ||
                migrated.progressVersion != Dimension2System.Civilization2ProgressVersion)
            {
                return FailCivilization2Validation("migración desde el estado reservado incorrecta");
            }

            Debug.Log(
                "[D2 Block 3A] VALIDACIÓN LÓGICA OK: Miembros iniciales, catálogo " +
                "regional, asignaciones exclusivas, migración y serialización."
            );
            return true;
        }
        finally
        {
            gameState.dimension02Unlocked = originalDimension2Unlocked;
            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(originalStateJson);
            gameState.EnsureDimension2State();
        }
    }

    private static bool FailCivilization2Validation(string reason)
    {
        Debug.LogError("[D2 Block 3A] Validación lógica fallida: " + reason + ".");
        return false;
    }

    private static bool ValidateCivilization2Block3BLogic(GameState gameState)
    {
        string originalStateJson = JsonUtility.ToJson(gameState.dimension2);
        bool originalDimension2Unlocked = gameState.dimension02Unlocked;

        try
        {
            gameState.dimension02Unlocked = true;
            Dimension2System.ResetState(gameState);
            gameState.dimension2.civilization2Unlocked = true;
            D2Civilization2State state = gameState.dimension2.civilization2;
            state.membersAvailable = 50L;
            state.totalMembersRecruited = 50L;
            D2Civilization2System.EnsureState(state);

            D2RegionState region = D2Civilization2System.GetRegion(
                state,
                D2Civilization2System.Region1Id
            );
            if (region == null || region.operations == null ||
                region.operations.Count != D2Civilization2System.OperationIds.Length)
            {
                return FailCivilization2Block3BValidation("catálogo de operaciones incorrecto");
            }

            if (!D2Civilization2System.TryAssignAllMembers(
                    gameState,
                    D2Civilization2System.Region1Id
                ) ||
                region.membersAssigned != 50L || state.membersAvailable != 0L)
            {
                return FailCivilization2Block3BValidation("asignación regional previa incorrecta");
            }

            bool assignedAllOperations =
                D2Civilization2System.TryAssignMembersToOperation(
                    gameState, D2Civilization2System.Region1Id,
                    D2Civilization2System.RescueOperationId, 5L
                ) &&
                D2Civilization2System.TryAssignMembersToOperation(
                    gameState, D2Civilization2System.Region1Id,
                    D2Civilization2System.ProtectionOperationId, 5L
                ) &&
                D2Civilization2System.TryAssignMembersToOperation(
                    gameState, D2Civilization2System.Region1Id,
                    D2Civilization2System.EspionageOperationId, 10L
                ) &&
                D2Civilization2System.TryAssignMembersToOperation(
                    gameState, D2Civilization2System.Region1Id,
                    D2Civilization2System.SabotageOperationId, 20L
                );
            if (!assignedAllOperations ||
                D2Civilization2System.GetMembersAssignedToOperations(region) != 40L ||
                D2Civilization2System.GetRegionIdleMembers(region) != 10L)
            {
                return FailCivilization2Block3BValidation("simultaneidad o exclusividad incorrecta");
            }

            foreach (string operationId in D2Civilization2System.OperationIds)
            {
                if (!D2Civilization2System.IsOperationActive(
                    D2Civilization2System.GetOperation(region, operationId)
                ))
                {
                    return FailCivilization2Block3BValidation(
                        "una operación con requisito completo quedó inactiva"
                    );
                }
            }

            long totalBeforeProgress = D2Civilization2System.GetTotalMembers(state);
            D2Civilization2System.ApplyOfflineProgress(gameState, 120.0);
            if (System.Math.Abs(region.dominance - 99.02) > 0.000001 ||
                System.Math.Abs(region.threat - 1.40) > 0.000001 ||
                state.membersAvailable != 1L ||
                D2Civilization2System.GetTotalMembers(state) != totalBeforeProgress + 1L)
            {
                return FailCivilization2Block3BValidation(
                    "progreso simultáneo online/offline incorrecto"
                );
            }

            if (!D2Civilization2System.TryReleaseAllMembersFromOperation(
                    gameState,
                    D2Civilization2System.Region1Id,
                    D2Civilization2System.RescueOperationId
                ) ||
                D2Civilization2System.GetRegionIdleMembers(region) != 15L ||
                !D2Civilization2System.TryReleaseAllMembers(
                    gameState,
                    D2Civilization2System.Region1Id
                ) ||
                region.membersAssigned != 35L || state.membersAvailable != 16L)
            {
                return FailCivilization2Block3BValidation(
                    "retorno exclusivo desde operaciones o región incorrecto"
                );
            }

            string serialized = JsonUtility.ToJson(state);
            D2Civilization2State restored = JsonUtility.FromJson<D2Civilization2State>(serialized);
            if (!D2Civilization2System.ValidateState(restored, out _) ||
                D2Civilization2System.GetOperation(
                    D2Civilization2System.GetRegion(
                        restored,
                        D2Civilization2System.Region1Id
                    ),
                    D2Civilization2System.SabotageOperationId
                ).membersAssigned != 20L)
            {
                return FailCivilization2Block3BValidation(
                    "serialización de operaciones incorrecta"
                );
            }

            Debug.Log(
                "[D2 Block 3B] VALIDACIÓN LÓGICA OK: cuatro operaciones " +
                "simultáneas, asignación exclusiva, Dominio, Amenaza, producción, " +
                "progreso offline y serialización."
            );
            return true;
        }
        finally
        {
            gameState.dimension02Unlocked = originalDimension2Unlocked;
            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(originalStateJson);
            gameState.EnsureDimension2State();
        }
    }

    private static bool FailCivilization2Block3BValidation(string reason)
    {
        Debug.LogError("[D2 Block 3B] Validación lógica fallida: " + reason + ".");
        return false;
    }

    private static bool ValidateCivilization2Block3CLogic(GameState gameState)
    {
        string originalStateJson = JsonUtility.ToJson(gameState.dimension2);
        bool originalDimension2Unlocked = gameState.dimension02Unlocked;

        try
        {
            gameState.dimension02Unlocked = true;
            Dimension2System.ResetState(gameState);
            gameState.dimension2.civilization2Unlocked = true;
            D2Civilization2State state = gameState.dimension2.civilization2;
            state.membersAvailable = 100L;
            state.totalMembersRecruited = 100L;
            D2Civilization2System.EnsureState(state);

            D2RegionState region = D2Civilization2System.GetRegion(
                state,
                D2Civilization2System.Region1Id
            );
            if (!D2Civilization2System.TryAssignAllMembers(
                    gameState,
                    D2Civilization2System.Region1Id
                ) ||
                !D2Civilization2System.TryAssignMembersToOperation(
                    gameState, D2Civilization2System.Region1Id,
                    D2Civilization2System.ProtectionOperationId, 5L
                ) ||
                !D2Civilization2System.TryAssignMembersToOperation(
                    gameState, D2Civilization2System.Region1Id,
                    D2Civilization2System.EspionageOperationId, 10L
                ) ||
                !D2Civilization2System.TryAssignMembersToOperation(
                    gameState, D2Civilization2System.Region1Id,
                    D2Civilization2System.SabotageOperationId, 20L
                ))
            {
                return FailCivilization2Block3CValidation(
                    "preparación de operaciones para Represalia incorrecta"
                );
            }

            region.coverage = 20.0;
            region.threat = 99.4;
            D2Civilization2System.ApplyOfflineProgress(gameState, 120.0);

            if (state.totalReprisals != 1L || region.totalReprisals != 1L ||
                state.controlFragments != D2Civilization2System.ControlFragmentsPerReprisal ||
                D2Civilization2System.GetTotalMembers(state) != 98L ||
                region.membersAssigned != 98L)
            {
                return FailCivilization2Block3CValidation(
                    "pérdidas, conteo o Fragmentos de Represalia incorrectos"
                );
            }

            if (region.coverage < 10.0 || region.coverage > 11.0 ||
                region.threat < 25.0 || region.threat > 26.0 ||
                System.Math.Abs(
                    region.nextReprisalEspionageReduction -
                    D2Civilization2System.EspionageReprisalReduction
                ) > 0.000001)
            {
                return FailCivilization2Block3CValidation(
                    "Cobertura, reinicio de Amenaza o protección de Espionaje incorrectos"
                );
            }

            if (string.IsNullOrEmpty(region.weakenedOperationId) ||
                region.weakenedOperationRemainingSeconds < 131.9 ||
                region.weakenedOperationRemainingSeconds > 132.1 ||
                D2Civilization2System.GetOperation(
                    region,
                    region.weakenedOperationId
                ) == null)
            {
                return FailCivilization2Block3CValidation(
                    "selección o duración del debilitamiento incorrecta"
                );
            }

            if (System.Math.Abs(
                    D2Civilization2System.GetExpectedReprisalLossFraction(state, region) -
                    D2Civilization2System.MinimumReprisalLossFraction
                ) > 0.000001)
            {
                return FailCivilization2Block3CValidation(
                    "límite mínimo de pérdidas incorrecto"
                );
            }

            D2Civilization2System.ApplyOfflineProgress(gameState, 132.1);
            if (!string.IsNullOrEmpty(region.weakenedOperationId) ||
                region.weakenedOperationRemainingSeconds != 0.0 ||
                state.totalReprisals != 1L)
            {
                return FailCivilization2Block3CValidation(
                    "finalización offline del debilitamiento incorrecta"
                );
            }

            string serialized = JsonUtility.ToJson(state);
            D2Civilization2State restored = JsonUtility.FromJson<D2Civilization2State>(serialized);
            if (!D2Civilization2System.ValidateState(restored, out _) ||
                restored.controlFragments != 3L || restored.totalReprisals != 1L)
            {
                return FailCivilization2Block3CValidation(
                    "serialización de Represalias incorrecta"
                );
            }

            Debug.Log(
                "[D2 Block 3C] VALIDACIÓN LÓGICA OK: Cobertura, Espionaje, " +
                "Represalias, pérdidas, debilitamiento, Fragmentos, progreso " +
                "offline y serialización."
            );
            return true;
        }
        finally
        {
            gameState.dimension02Unlocked = originalDimension2Unlocked;
            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(originalStateJson);
            gameState.EnsureDimension2State();
        }
    }

    private static bool FailCivilization2Block3CValidation(string reason)
    {
        Debug.LogError("[D2 Block 3C] Validación lógica fallida: " + reason + ".");
        return false;
    }

    private static bool ValidateCivilization2Block3DLogic(GameState gameState)
    {
        string originalStateJson = JsonUtility.ToJson(gameState.dimension2);
        bool originalDimension2Unlocked = gameState.dimension02Unlocked;

        try
        {
            gameState.dimension02Unlocked = true;
            Dimension2System.ResetState(gameState);
            gameState.dimension2.civilization2Unlocked = true;
            D2Civilization2State state = gameState.dimension2.civilization2;
            state.membersAvailable = 30L;
            state.totalMembersRecruited = 30L;
            D2Civilization2System.EnsureState(state);

            D2RegionState region1 = D2Civilization2System.GetRegion(
                state,
                D2Civilization2System.Region1Id
            );
            D2RegionState region2 = D2Civilization2System.GetRegion(
                state,
                D2Civilization2System.Region2Id
            );
            D2RegionState region3 = D2Civilization2System.GetRegion(
                state,
                D2Civilization2System.Region3Id
            );
            if (!D2Civilization2System.TryAssignAllMembers(
                    gameState,
                    D2Civilization2System.Region1Id
                ) ||
                !D2Civilization2System.TryAssignMembersToOperation(
                    gameState,
                    D2Civilization2System.Region1Id,
                    D2Civilization2System.SabotageOperationId,
                    20L
                ))
            {
                return FailCivilization2Block3DValidation(
                    "preparación regional para desbloqueos incorrecta"
                );
            }

            region1.dominance = 80.05;
            D2Civilization2System.ApplyOfflineProgress(gameState, 20.0);
            if (!region2.unlocked || region3.unlocked ||
                System.Math.Abs(region2.dominance - 100.0) > 0.000001 ||
                D2Civilization2System.GetTotalDominance(state) <= 80.0)
            {
                return FailCivilization2Block3DValidation(
                    "desbloqueo offline o repunte de Región 2 incorrecto"
                );
            }

            if (!D2Civilization2System.TrySelectRegion(
                    gameState,
                    D2Civilization2System.Region2Id
                ) ||
                state.selectedRegionId != D2Civilization2System.Region2Id ||
                D2Civilization2System.TrySelectRegion(
                    gameState,
                    D2Civilization2System.Region4Id
                ))
            {
                return FailCivilization2Block3DValidation(
                    "selección de regiones desbloqueadas incorrecta"
                );
            }

            region1.dominance = 20.10;
            region2.dominance = 100.0;
            D2Civilization2System.ApplyOfflineProgress(gameState, 20.0);
            double dominanceAfterRegion3 = D2Civilization2System.GetTotalDominance(state);
            if (!region3.unlocked ||
                System.Math.Abs(region3.dominance - 100.0) > 0.000001 ||
                dominanceAfterRegion3 < 73.2 || dominanceAfterRegion3 > 73.5)
            {
                return FailCivilization2Block3DValidation(
                    "desbloqueo offline o repunte de Región 3 incorrecto"
                );
            }

            if (!D2Civilization2System.TrySelectRegion(
                    gameState,
                    D2Civilization2System.Region3Id
                ) ||
                D2Civilization2System.GetSelectedRegion(state) != region3)
            {
                return FailCivilization2Block3DValidation(
                    "selección de Región 3 incorrecta"
                );
            }

            region1.dominance = 100.0;
            region2.dominance = 100.0;
            region3.dominance = 100.0;
            D2Civilization2System.EnsureState(state);
            if (!region2.unlocked || !region3.unlocked ||
                D2Civilization2System.GetTotalDominance(state) != 100.0)
            {
                return FailCivilization2Block3DValidation(
                    "permanencia de desbloqueos o promedio total incorrecto"
                );
            }

            string serialized = JsonUtility.ToJson(state);
            D2Civilization2State restored = JsonUtility.FromJson<D2Civilization2State>(serialized);
            D2Civilization2System.EnsureState(restored);
            if (!D2Civilization2System.ValidateState(restored, out _) ||
                !D2Civilization2System.GetRegion(
                    restored,
                    D2Civilization2System.Region2Id
                ).unlocked ||
                !D2Civilization2System.GetRegion(
                    restored,
                    D2Civilization2System.Region3Id
                ).unlocked ||
                restored.selectedRegionId != D2Civilization2System.Region3Id)
            {
                return FailCivilization2Block3DValidation(
                    "serialización de regiones o selección incorrecta"
                );
            }

            Debug.Log(
                "[D2 Block 3D] VALIDACIÓN LÓGICA OK: Región 2, Región 3, " +
                "promedio, repunte, selección, permanencia, offline y serialización."
            );
            return true;
        }
        finally
        {
            gameState.dimension02Unlocked = originalDimension2Unlocked;
            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(originalStateJson);
            gameState.EnsureDimension2State();
        }
    }

    private static bool FailCivilization2Block3DValidation(string reason)
    {
        Debug.LogError("[D2 Block 3D] Validación lógica fallida: " + reason + ".");
        return false;
    }

    private static bool ValidateCivilization2Block3ELogic(GameState gameState)
    {
        string originalStateJson = JsonUtility.ToJson(gameState.dimension2);
        bool originalDimension2Unlocked = gameState.dimension02Unlocked;

        try
        {
            gameState.dimension02Unlocked = true;
            Dimension2System.ResetState(gameState);
            gameState.dimension2.civilization2Unlocked = true;
            D2Civilization2State state = gameState.dimension2.civilization2;
            state.membersAvailable = 120L;
            state.totalMembersRecruited = 120L;
            state.controlFragments = 30L;
            D2Civilization2System.EnsureState(state);

            if (!D2Civilization2System.TryUpgradeResistance(gameState, D2Civilization2System.RescueUpgradeId) ||
                !D2Civilization2System.TryUpgradeResistance(gameState, D2Civilization2System.RescueUpgradeId) ||
                !D2Civilization2System.TryUpgradeResistance(gameState, D2Civilization2System.RescueUpgradeId) ||
                D2Civilization2System.GetUpgradeLevel(state, D2Civilization2System.RescueUpgradeId) != 3 ||
                state.controlFragments != 12L)
            {
                return FailCivilization2Block3EValidation("niveles o costes 3/6/9 de mejoras incorrectos");
            }

            if (!D2Civilization2System.TryActivateResistancePact(gameState, D2Civilization2System.HiddenSheltersPactId) ||
                !D2Civilization2System.TryActivateResistancePact(gameState, D2Civilization2System.SilencedBellsPactId) ||
                !D2Civilization2System.TryActivateResistancePact(gameState, D2Civilization2System.KnivesPactId) ||
                state.membersAvailable != 25L ||
                D2Civilization2System.GetMembersAssignedToPacts(state) != 95L ||
                D2Civilization2System.GetTotalMembers(state) != 120L)
            {
                return FailCivilization2Block3EValidation("activación simultánea o conservación inicial incorrecta");
            }

            D2RegionState region = D2Civilization2System.GetRegion(state, D2Civilization2System.Region1Id);
            double protectedLoss = D2Civilization2System.GetExpectedReprisalLossFraction(state, region);
            if (System.Math.Abs(protectedLoss - 0.064) > 0.000001)
                return FailCivilization2Block3EValidation("beneficio de Refugios Ocultos incorrecto");

            D2Civilization2System.ApplyOfflineProgress(gameState, 120.0);
            if (D2Civilization2System.GetExhaustedMembers(state) != 3L ||
                D2Civilization2System.GetMembersAssignedToPacts(state) != 92L ||
                D2Civilization2System.GetTotalMembers(state) != 120L)
            {
                return FailCivilization2Block3EValidation("desgaste offline o conservación de Miembros incorrecta");
            }

            if (!D2Civilization2System.TryReinforceResistancePact(gameState, D2Civilization2System.KnivesPactId, 1L) ||
                !D2Civilization2System.TryCancelResistancePact(gameState, D2Civilization2System.HiddenSheltersPactId) ||
                !D2Civilization2System.TryCancelResistancePact(gameState, D2Civilization2System.SilencedBellsPactId) ||
                !D2Civilization2System.TryCancelResistancePact(gameState, D2Civilization2System.KnivesPactId) ||
                state.hiddenSheltersPenaltySeconds != 300.0 ||
                state.silencedBellsPenaltySeconds != 300.0 ||
                state.knivesPenaltySeconds != 300.0 ||
                D2Civilization2System.GetTotalMembers(state) != 120L)
            {
                return FailCivilization2Block3EValidation("refuerzo, incumplimiento o penalizaciones incorrectos");
            }

            D2Civilization2System.ApplyOfflineProgress(gameState, 300.0);
            if (D2Civilization2System.GetExhaustedMembers(state) != 0L ||
                state.hiddenSheltersPenaltySeconds != 0.0 ||
                state.silencedBellsPenaltySeconds != 0.0 ||
                state.knivesPenaltySeconds != 0.0 ||
                state.membersAvailable != 120L ||
                D2Civilization2System.GetTotalMembers(state) != 120L)
            {
                return FailCivilization2Block3EValidation("recuperación o vencimiento offline a 5 minutos incorrecto");
            }

            string serialized = JsonUtility.ToJson(state);
            D2Civilization2State restored = JsonUtility.FromJson<D2Civilization2State>(serialized);
            D2Civilization2System.EnsureState(restored);
            if (!D2Civilization2System.ValidateState(restored, out _) ||
                D2Civilization2System.GetUpgradeLevel(restored, D2Civilization2System.RescueUpgradeId) != 3 ||
                D2Civilization2System.GetTotalMembers(restored) != 120L)
            {
                return FailCivilization2Block3EValidation("serialización de mejoras, pactos o recuperación incorrecta");
            }

            Debug.Log(
                "[D2 Block 3E] VALIDACIÓN LÓGICA OK: mejoras, tres pactos simultáneos, " +
                "beneficios, refuerzo, desgaste, recuperación, incumplimiento, " +
                "penalizaciones, offline y serialización."
            );
            return true;
        }
        finally
        {
            gameState.dimension02Unlocked = originalDimension2Unlocked;
            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(originalStateJson);
            gameState.EnsureDimension2State();
        }
    }

    private static bool FailCivilization2Block3EValidation(string reason)
    {
        Debug.LogError("[D2 Block 3E] Validación lógica fallida: " + reason + ".");
        return false;
    }

    private static bool ValidateCivilization2Block3FLogic(GameState gameState)
    {
        string originalStateJson = JsonUtility.ToJson(gameState.dimension2);
        bool originalDimension2Unlocked = gameState.dimension02Unlocked;

        try
        {
            gameState.dimension02Unlocked = true;
            Dimension2System.ResetState(gameState);
            gameState.dimension2.civilization2Unlocked = true;
            D2Civilization2State state = gameState.dimension2.civilization2;
            state.membersAvailable = 100L;
            state.totalMembersRecruited = 100L;

            foreach (string regionId in new[]
            {
                D2Civilization2System.Region1Id,
                D2Civilization2System.Region2Id,
                D2Civilization2System.Region3Id
            })
            {
                D2RegionState region = D2Civilization2System.GetRegion(state, regionId);
                region.unlocked = true;
                region.dominance = 30.0;
            }

            gameState.EnsureDimension2State();
            if (!state.alertActive || !state.containmentAvailable ||
                !gameState.dimension2.civilization3Unlocked ||
                D2Civilization2System.GetTotalDominance(state) != 30.0)
            {
                return FailCivilization2Block3FValidation(
                    "activación permanente, Contención o desbloqueo de Civilización 3 incorrectos"
                );
            }

            foreach (string regionId in new[]
            {
                D2Civilization2System.Region1Id,
                D2Civilization2System.Region2Id,
                D2Civilization2System.Region3Id
            })
            {
                D2RegionState region = D2Civilization2System.GetRegion(state, regionId);
                region.membersAssigned = 5L;
                D2Civilization2System.GetOperation(
                    region,
                    D2Civilization2System.ProtectionOperationId
                ).membersAssigned = 5L;
            }
            state.alertMarkProgressSeconds = 599.0;
            D2Civilization2System.ApplyOfflineProgress(gameState, 1.0);
            if (state.totalAlertMarks != 1L || CountAlertMarkedRegions(state) != 0L)
            {
                return FailCivilization2Block3FValidation(
                    "mitigación de marca mediante Protección incorrecta"
                );
            }

            foreach (string regionId in new[]
            {
                D2Civilization2System.Region1Id,
                D2Civilization2System.Region2Id,
                D2Civilization2System.Region3Id
            })
            {
                D2RegionState region = D2Civilization2System.GetRegion(state, regionId);
                region.membersAssigned = 0L;
                D2Civilization2System.GetOperation(
                    region,
                    D2Civilization2System.ProtectionOperationId
                ).membersAssigned = 0L;
            }

            D2RegionState region1 = D2Civilization2System.GetRegion(
                state,
                D2Civilization2System.Region1Id
            );
            region1.membersAssigned = 20L;
            region1.threat = 0.0;
            D2Civilization2System.GetOperation(
                region1,
                D2Civilization2System.SabotageOperationId
            ).membersAssigned = 20L;
            D2Civilization2System.ApplyOfflineProgress(gameState, 60.0);
            if (region1.threat < 0.899 || region1.threat > 0.901)
            {
                return FailCivilization2Block3FValidation(
                    "multiplicador de +50% de Amenaza durante Alerta incorrecto"
                );
            }

            D2Civilization2System.GetOperation(
                region1,
                D2Civilization2System.SabotageOperationId
            ).membersAssigned = 0L;
            region1.membersAssigned = 0L;
            state.alertMarkProgressSeconds = 599.0;
            D2Civilization2System.ApplyOfflineProgress(gameState, 1.0);
            D2RegionState markedRegion = FindAlertMarkedRegion(state);
            if (state.totalAlertMarks != 2L || markedRegion == null ||
                CountAlertMarkedRegions(state) != 1L)
            {
                return FailCivilization2Block3FValidation(
                    "marca no acumulable o selección de región desbloqueada incorrecta"
                );
            }

            markedRegion.membersAssigned = 100L;
            markedRegion.threat = 100.0;
            double markedLoss = D2Civilization2System.GetExpectedReprisalLossFraction(
                state,
                markedRegion
            );
            state.controlFragments = 0L;
            D2Civilization2System.ApplyOfflineProgress(gameState, 0.1);
            if (System.Math.Abs(markedLoss - 0.11) > 0.000001 ||
                markedRegion.alertMarked || state.controlFragments != 6L ||
                markedRegion.membersAssigned != 89L)
            {
                return FailCivilization2Block3FValidation(
                    "pérdida marcada, consumo de marca o recompensa de 6 Fragmentos incorrectos"
                );
            }

            foreach (D2RegionState region in state.regions)
                if (region != null && region.regionId != D2Civilization2System.Region4Id)
                    region.dominance = 100.0;
            gameState.EnsureDimension2State();
            if (!state.alertActive || !gameState.dimension2.civilization3Unlocked)
            {
                return FailCivilization2Block3FValidation(
                    "Alerta o desbloqueo dejaron de ser permanentes"
                );
            }

            string serialized = JsonUtility.ToJson(state);
            D2Civilization2State restored = JsonUtility.FromJson<D2Civilization2State>(serialized);
            D2Civilization2System.EnsureState(restored);
            if (!D2Civilization2System.ValidateState(restored, out _) ||
                !restored.alertActive || !restored.containmentAvailable ||
                restored.totalAlertMarks != 2L)
            {
                return FailCivilization2Block3FValidation(
                    "serialización de Alerta y marcas incorrecta"
                );
            }

            Debug.Log(
                "[D2 Block 3F] VALIDACIÓN LÓGICA OK: umbral 30%, Alerta permanente, " +
                "Amenaza +50%, seis Fragmentos, marcas, Protección, consumo, " +
                "Civilización 3, Contención, offline y serialización."
            );
            return true;
        }
        finally
        {
            gameState.dimension02Unlocked = originalDimension2Unlocked;
            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(originalStateJson);
            gameState.EnsureDimension2State();
        }
    }

    private static long CountAlertMarkedRegions(D2Civilization2State state)
    {
        long count = 0L;
        foreach (D2RegionState region in state.regions)
            if (region != null && region.alertMarked) count++;
        return count;
    }

    private static D2RegionState FindAlertMarkedRegion(D2Civilization2State state)
    {
        foreach (D2RegionState region in state.regions)
            if (region != null && region.alertMarked) return region;
        return null;
    }

    private static bool FailCivilization2Block3FValidation(string reason)
    {
        Debug.LogError("[D2 Block 3F] Validación lógica fallida: " + reason + ".");
        return false;
    }

    private static bool ValidateCivilization2Block3GLogic(GameState gameState)
    {
        string originalStateJson = JsonUtility.ToJson(gameState.dimension2);
        bool originalDimension2Unlocked = gameState.dimension02Unlocked;

        try
        {
            gameState.dimension02Unlocked = true;
            Dimension2System.ResetState(gameState);
            gameState.dimension2.civilization2Unlocked = true;
            D2Civilization2State state = gameState.dimension2.civilization2;
            state.membersAvailable = 100L;
            state.totalMembersRecruited = 100L;
            foreach (string regionId in new[]
            {
                D2Civilization2System.Region1Id,
                D2Civilization2System.Region2Id,
                D2Civilization2System.Region3Id
            })
            {
                D2RegionState region = D2Civilization2System.GetRegion(state, regionId);
                region.unlocked = true;
                region.dominance = 30.0;
            }
            gameState.EnsureDimension2State();

            double[] dominancePoints = { 30.0, 20.0, 10.0, 0.0 };
            double[] expectedProbabilities = { 0.20, 0.45, 0.70, 1.00 };
            for (int i = 0; i < dominancePoints.Length; i++)
            {
                foreach (D2RegionState region in state.regions)
                    if (region != null && region.regionId != D2Civilization2System.Region4Id)
                        region.dominance = dominancePoints[i];
                double probability = D2Civilization2System.GetContainmentSuccessProbability(state);
                if (System.Math.Abs(probability - expectedProbabilities[i]) > 0.000001)
                    return FailCivilization2Block3GValidation("interpolación de probabilidades incorrecta");
            }

            foreach (D2RegionState region in state.regions)
                if (region != null && region.regionId != D2Civilization2System.Region4Id)
                    region.dominance = 30.0;
            D2RegionState region1 = D2Civilization2System.GetRegion(state, D2Civilization2System.Region1Id);
            D2RegionState region2 = D2Civilization2System.GetRegion(state, D2Civilization2System.Region2Id);
            D2RegionState region3 = D2Civilization2System.GetRegion(state, D2Civilization2System.Region3Id);
            region1.membersAssigned = 100L;
            region2.membersAssigned = 100L;
            region1.threat = 10.0;
            region2.threat = 80.0;
            region3.threat = 0.0;
            D2Civilization2System.GetOperation(
                region2,
                D2Civilization2System.ProtectionOperationId
            ).membersAssigned = 5L;

            if (!D2Civilization2System.TryAttemptContainment(gameState, 0.99) ||
                state.entityContained || state.totalContainmentAttempts != 1L ||
                state.totalContainmentFailures != 1L ||
                state.containmentCooldownSeconds != 600.0 ||
                region1.threat != 30.0 || region2.threat != 100.0 || region3.threat != 20.0 ||
                region1.membersAssigned != 95L || region2.membersAssigned != 100L ||
                D2Civilization2System.TryAttemptContainment(gameState, 0.0))
            {
                return FailCivilization2Block3GValidation(
                    "fallo, Protección, pérdidas, Amenaza o bloqueo por cooldown incorrectos"
                );
            }

            D2Civilization2System.GetOperation(
                region2,
                D2Civilization2System.ProtectionOperationId
            ).membersAssigned = 0L;
            D2Civilization2System.ApplyOfflineProgress(gameState, 600.0);
            if (state.containmentCooldownSeconds != 0.0)
                return FailCivilization2Block3GValidation("cooldown offline incorrecto");

            foreach (D2RegionState region in state.regions)
            {
                if (region == null || region.regionId == D2Civilization2System.Region4Id)
                    continue;
                region.dominance = 0.0;
                region.alertMarked = true;
            }
            if (!D2Civilization2System.TryAttemptContainment(gameState, 0.99) ||
                !state.entityContained || !state.majorPactPrepared ||
                state.totalContainmentAttempts != 2L || CountAlertMarkedRegions(state) != 0L)
            {
                return FailCivilization2Block3GValidation(
                    "éxito permanente, limpieza de marcas o preparación del pacto mayor incorrectos"
                );
            }

            region3.membersAssigned = 20L;
            region3.threat = 0.0;
            D2Civilization2System.GetOperation(
                region3,
                D2Civilization2System.SabotageOperationId
            ).membersAssigned = 20L;
            D2Civilization2System.ApplyOfflineProgress(gameState, 60.0);
            if (region3.threat < 0.599 || region3.threat > 0.601 ||
                CountAlertMarkedRegions(state) != 0L)
            {
                return FailCivilization2Block3GValidation(
                    "fin del aumento de Amenaza o de nuevas marcas incorrecto"
                );
            }

            long totalBeforeSustain = D2Civilization2System.GetTotalMembers(state);
            if (!D2Civilization2System.TryAssignMembersToContainment(gameState, 10L) ||
                !D2Civilization2System.TryReleaseMembersFromContainment(gameState, 1L) ||
                state.membersAssignedToContainment != 9L ||
                D2Civilization2System.GetTotalMembers(state) != totalBeforeSustain)
            {
                return FailCivilization2Block3GValidation(
                    "asignación, retiro o conservación del sostenimiento incorrectos"
                );
            }

            string serialized = JsonUtility.ToJson(state);
            D2Civilization2State restored = JsonUtility.FromJson<D2Civilization2State>(serialized);
            D2Civilization2System.EnsureState(restored);
            if (!D2Civilization2System.ValidateState(restored, out _) ||
                !restored.entityContained || !restored.majorPactPrepared ||
                restored.membersAssignedToContainment != 9L ||
                restored.totalContainmentAttempts != 2L ||
                restored.totalContainmentFailures != 1L)
            {
                return FailCivilization2Block3GValidation(
                    "serialización del resultado o sostenimiento incorrecta"
                );
            }

            Debug.Log(
                "[D2 Block 3G] VALIDACIÓN LÓGICA OK: tabla interpolada, fallo, " +
                "Protección, pérdidas, cooldown offline, éxito, marcas, fin del aumento " +
                "de Amenaza, pacto mayor, sostenimiento y serialización."
            );
            return true;
        }
        finally
        {
            gameState.dimension02Unlocked = originalDimension2Unlocked;
            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(originalStateJson);
            gameState.EnsureDimension2State();
        }
    }

    private static bool FailCivilization2Block3GValidation(string reason)
    {
        Debug.LogError("[D2 Block 3G] Validación lógica fallida: " + reason + ".");
        return false;
    }

    private static bool ValidateCivilization2Block5CLogic(GameState gameState)
    {
        string originalStateJson = JsonUtility.ToJson(gameState.dimension2);
        bool originalDimension2Unlocked = gameState.dimension02Unlocked;

        try
        {
            gameState.dimension02Unlocked = true;
            Dimension2System.ResetState(gameState);
            gameState.dimension2.civilization2Unlocked = true;
            D2Civilization2State state = gameState.dimension2.civilization2;
            D2Civilization2System.EnsureState(state);

            if (D2Civilization2System.TryEstablishMajorPact(gameState))
                return FailCivilization2Block5CValidation("el pacto se estableció antes de contener al Ente");

            state.entityContained = true;
            D2Civilization2System.EnsureState(state);
            long membersBeforePact = state.membersAvailable;
            long fragmentsBeforePact = state.controlFragments;
            double stabilityBeforePact = state.containmentStability;
            if (!D2Civilization2System.TryEstablishMajorPact(gameState) ||
                state.membersAvailable != membersBeforePact ||
                state.controlFragments != fragmentsBeforePact ||
                state.containmentStability != stabilityBeforePact)
            {
                return FailCivilization2Block5CValidation(
                    "establecimiento sin coste o requisito de Contención incorrecto"
                );
            }

            if (!D2Civilization2System.TryAssignMembersToContainment(gameState, 4L))
                return FailCivilization2Block5CValidation("asignación al sostenimiento incorrecta");
            D2Civilization2System.ApplyOfflineProgress(gameState, 60.0);
            if (System.Math.Abs(state.containmentStability - 2.0) > 0.000001)
                return FailCivilization2Block5CValidation("generación offline de Estabilidad incorrecta");

            string firstLine = D2Civilization2System.ReconstitutedNetworkLineId;
            if (D2Civilization2System.TryUpgradeMajorPactLine(gameState, firstLine))
                return FailCivilization2Block5CValidation("una línea ignoró sus costes");

            state.containmentStability = 120.0;
            state.controlFragments = 18L;
            for (int expectedLevel = 1; expectedLevel <= 3; expectedLevel++)
            {
                if (!D2Civilization2System.TryUpgradeMajorPactLine(gameState, firstLine) ||
                    D2Civilization2System.GetMajorPactLineLevel(state, firstLine) != expectedLevel)
                {
                    return FailCivilization2Block5CValidation("progresión de niveles o costes incorrecta");
                }
            }
            if (System.Math.Abs(state.containmentStability) > 0.000001 ||
                state.controlFragments != 0L ||
                D2Civilization2System.TryUpgradeMajorPactLine(gameState, firstLine))
            {
                return FailCivilization2Block5CValidation("coste acumulado o nivel máximo incorrecto");
            }

            state.containmentStability = 1000.0;
            state.controlFragments = 1000L;
            for (int i = 1; i < D2Civilization2System.MajorPactLineIds.Length; i++)
            {
                string lineId = D2Civilization2System.MajorPactLineIds[i];
                for (int level = 1; level <= 3; level++)
                    if (!D2Civilization2System.TryUpgradeMajorPactLine(gameState, lineId))
                        return FailCivilization2Block5CValidation("mejora de las cinco líneas incorrecta");
            }

            D2RegionState region = D2Civilization2System.GetRegion(
                state, D2Civilization2System.Region1Id);
            if (System.Math.Abs(D2Civilization2System.GetMajorPactMemberMultiplier(state) - 1.15) > 0.000001 ||
                System.Math.Abs(D2Civilization2System.GetMajorPactDominanceMultiplier(state) - 1.15) > 0.000001 ||
                System.Math.Abs(D2Civilization2System.GetMajorPactCoverageMultiplier(state) - 1.15) > 0.000001 ||
                System.Math.Abs(D2Civilization2System.GetExpectedReprisalLossFraction(state, region) - 0.05) > 0.000001 ||
                System.Math.Abs(gameState.GetDimension2ArtifactProductionMultiplier("vacuum_observer") - 1.06) > 0.000001 ||
                System.Math.Abs(gameState.GetDimension2ArtifactProductionMultiplier("basic_generator") - 1.0) > 0.000001 ||
                System.Math.Abs(gameState.GetDimension2TriangleEffectMultiplier() - 1.06) > 0.000001)
            {
                return FailCivilization2Block5CValidation("efectos internos o externos incorrectos");
            }

            string serialized = JsonUtility.ToJson(state);
            D2Civilization2State restored = JsonUtility.FromJson<D2Civilization2State>(serialized);
            D2Civilization2System.EnsureState(restored);
            if (!D2Civilization2System.ValidateState(restored, out _) ||
                !restored.majorPactEstablished ||
                restored.progressVersion != Dimension2System.Civilization2ProgressVersion ||
                restored.majorPactLines == null ||
                restored.majorPactLines.Count != D2Civilization2System.MajorPactLineIds.Length)
            {
                return FailCivilization2Block5CValidation("serialización del pacto mayor incorrecta");
            }

            Debug.Log(
                "[D2 Block 5C] VALIDACIÓN LÓGICA OK: establecimiento, Estabilidad " +
                "online/offline, cinco líneas, costes, efectos y serialización."
            );
            return true;
        }
        finally
        {
            gameState.dimension02Unlocked = originalDimension2Unlocked;
            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(originalStateJson);
            gameState.EnsureDimension2State();
        }
    }

    private static bool FailCivilization2Block5CValidation(string reason)
    {
        Debug.LogError("[D2 Block 5C] Validación lógica fallida: " + reason + ".");
        return false;
    }

    private static bool ValidateCivilization3Block4ALogic(GameState gameState)
    {
        string originalStateJson = JsonUtility.ToJson(gameState.dimension2);
        bool originalDimension2Unlocked = gameState.dimension02Unlocked;

        try
        {
            gameState.dimension02Unlocked = true;
            Dimension2System.ResetState(gameState);
            gameState.dimension2.civilization3Unlocked = true;
            D2Civilization3State state = gameState.dimension2.civilization3;
            D2Civilization3System.EnsureState(state);
            D2C3ZoneState zone1 = D2Civilization3System.GetZone(
                state,
                D2Civilization3System.Zone1Id
            );
            if (zone1 == null || !zone1.unlocked ||
                D2Civilization3System.GetZone(state, D2Civilization3System.Zone2Id).unlocked ||
                D2Civilization3System.GetZone(state, D2Civilization3System.Zone3Id).unlocked ||
                state.zones.Count != 3)
            {
                return FailCivilization3Block4AValidation("catálogo o desbloqueos iniciales incorrectos");
            }

            if (D2Civilization3System.GetQualityForRoll(D2Civilization3System.Zone1Id, 0.69) !=
                    D2Civilization3System.LowQualityId ||
                D2Civilization3System.GetQualityForRoll(D2Civilization3System.Zone1Id, 0.70) !=
                    D2Civilization3System.MediumQualityId ||
                D2Civilization3System.GetQualityForRoll(D2Civilization3System.Zone1Id, 0.95) !=
                    D2Civilization3System.HighQualityId)
            {
                return FailCivilization3Block4AValidation("distribución 70/25/5 de calidad incorrecta");
            }

            if (!D2Civilization3System.TryStartExcavation(
                    gameState,
                    D2Civilization3System.Zone1Id
                ))
            {
                return FailCivilization3Block4AValidation("inicio manual de excavación incorrecto");
            }
            D2Civilization3System.ApplyOfflineProgress(gameState, 29.0);
            if (!zone1.excavationActive || zone1.totalExcavationsCompleted != 0L ||
                zone1.excavationRemainingSeconds < 0.999 ||
                zone1.excavationRemainingSeconds > 1.001)
            {
                return FailCivilization3Block4AValidation("progreso parcial offline incorrecto");
            }
            D2Civilization3System.ApplyOfflineProgress(gameState, 1.0);
            if (zone1.excavationActive || zone1.totalExcavationsCompleted != 1L ||
                D2Civilization3System.GetTotalRemains(zone1) != 1L)
            {
                return FailCivilization3Block4AValidation("finalización manual o inventario incorrectos");
            }

            D2AltarState wax = D2AltarSystem.GetAltar(
                gameState.dimension2.civilization1,
                D2AltarSystem.WaxAltarId
            );
            D2AltarState bread = D2AltarSystem.GetAltar(
                gameState.dimension2.civilization1,
                D2AltarSystem.RitualBreadAltarId
            );
            wax.offeringAmount = 10.0;
            bread.offeringAmount = 10.0;
            if (!D2Civilization3System.TryHireFieldScholar(gameState) ||
                !zone1.scholarHired || wax.offeringAmount != 0.0 ||
                bread.offeringAmount != 0.0 ||
                D2Civilization3System.TryHireFieldScholar(gameState))
            {
                return FailCivilization3Block4AValidation("contratación o coste del Erudito incorrectos");
            }

            string serialized = JsonUtility.ToJson(state);
            D2Civilization3State restored = JsonUtility.FromJson<D2Civilization3State>(serialized);
            D2Civilization3System.EnsureState(restored);
            D2C3ZoneState restoredZone = D2Civilization3System.GetZone(
                restored,
                D2Civilization3System.Zone1Id
            );
            if (!D2Civilization3System.ValidateState(restored, out _) ||
                restoredZone == null || !restoredZone.scholarHired ||
                restoredZone.totalExcavationsCompleted != 1L ||
                D2Civilization3System.GetTotalRemains(restoredZone) != 1L)
            {
                return FailCivilization3Block4AValidation("serialización de Zona 1 incorrecta");
            }

            Debug.Log(
                "[D2 Block 4A] VALIDACIÓN LÓGICA OK: estado, tres zonas, Entrada " +
                "Sepultada, calidad 70/25/5, excavación exclusivamente manual, " +
                "inventario, offline, Erudito de Campo, costes y serialización."
            );
            return true;
        }
        finally
        {
            gameState.dimension02Unlocked = originalDimension2Unlocked;
            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(originalStateJson);
            gameState.EnsureDimension2State();
        }
    }

    private static bool FailCivilization3Block4AValidation(string reason)
    {
        Debug.LogError("[D2 Block 4A] Validación lógica fallida: " + reason + ".");
        return false;
    }

    private static bool ValidateCivilization3Block4BLogic(GameState gameState)
    {
        string originalStateJson = JsonUtility.ToJson(gameState.dimension2);
        bool originalDimension2Unlocked = gameState.dimension02Unlocked;

        try
        {
            gameState.dimension02Unlocked = true;
            Dimension2System.ResetState(gameState);
            gameState.dimension2.civilization3Unlocked = true;
            D2Civilization3State state = gameState.dimension2.civilization3;
            D2Civilization3System.EnsureState(state);
            D2C3ZoneState zone1 = D2Civilization3System.GetZone(
                state,
                D2Civilization3System.Zone1Id
            );
            zone1.lowQualityRemains = 2L;
            zone1.mediumQualityRemains = 1L;
            zone1.highQualityRemains = 2L;

            if (D2Civilization3System.TryStartAnalysis(
                    gameState,
                    D2Civilization3System.Zone1Id,
                    D2Civilization3System.LowQualityId
                ))
            {
                return FailCivilization3Block4BValidation("análisis permitido sin Erudito");
            }
            zone1.scholarHired = true;
            if (!D2Civilization3System.TryStartAnalysis(
                    gameState,
                    D2Civilization3System.Zone1Id,
                    D2Civilization3System.LowQualityId
                ) ||
                zone1.lowQualityRemains != 1L ||
                D2Civilization3System.TryStartAnalysis(
                    gameState,
                    D2Civilization3System.Zone1Id,
                    D2Civilization3System.MediumQualityId
                ))
            {
                return FailCivilization3Block4BValidation("inicio, consumo o exclusividad incorrectos");
            }
            D2Civilization3System.ApplyOfflineProgress(gameState, 29.0);
            if (!zone1.analysisActive || zone1.analysisRemainingSeconds < 0.999 ||
                zone1.analysisRemainingSeconds > 1.001)
            {
                return FailCivilization3Block4BValidation("progreso parcial offline incorrecto");
            }
            D2Civilization3System.ApplyOfflineProgress(gameState, 1.0);
            if (zone1.analysisActive || state.ancientKnowledge != 1.0 ||
                zone1.zoneResourceAmount != 1L || zone1.researchProgress != 1.0 ||
                !state.archiveUnlocked || state.archiveLevel != 1)
            {
                return FailCivilization3Block4BValidation("recompensa Baja o Archivo I incorrectos");
            }

            D2Civilization3System.TryStartAnalysis(
                gameState,
                D2Civilization3System.Zone1Id,
                D2Civilization3System.MediumQualityId
            );
            D2Civilization3System.ApplyOfflineProgress(gameState, 30.0);
            D2Civilization3System.TryStartAnalysis(
                gameState,
                D2Civilization3System.Zone1Id,
                D2Civilization3System.HighQualityId
            );
            D2Civilization3System.ApplyOfflineProgress(gameState, 30.0);
            if (state.ancientKnowledge != 12.0 || zone1.zoneResourceAmount != 7L ||
                zone1.researchProgress != 12.0 || zone1.totalAnalysesCompleted != 3L)
            {
                return FailCivilization3Block4BValidation("recompensas Media o Alta incorrectas");
            }

            zone1.researchProgress = 20.0;
            zone1.bonusRemainsProgress = 0.95;
            long remainsBeforeBonus = D2Civilization3System.GetTotalRemains(zone1);
            D2Civilization3System.TryStartExcavation(
                gameState,
                D2Civilization3System.Zone1Id
            );
            D2Civilization3System.ApplyOfflineProgress(gameState, 30.0);
            if (D2Civilization3System.GetTotalRemains(zone1) != remainsBeforeBonus + 2L ||
                zone1.bonusRemainsProgress > 0.000001)
            {
                return FailCivilization3Block4BValidation("hito de restos al 20% incorrecto");
            }

            zone1.researchProgress = 40.0;
            zone1.lowQualityRemains++;
            if (!D2Civilization3System.TryStartAnalysis(
                    gameState,
                    D2Civilization3System.Zone1Id,
                    D2Civilization3System.LowQualityId
                ) ||
                System.Math.Abs(zone1.analysisRemainingSeconds - 28.5) > 0.000001)
            {
                return FailCivilization3Block4BValidation("hito de velocidad al 40% incorrecto");
            }
            D2Civilization3System.ApplyOfflineProgress(gameState, 28.5);

            zone1.researchProgress = 80.0;
            zone1.highQualityRemains++;
            double knowledgeBeforeBonus = state.ancientKnowledge;
            D2Civilization3System.TryStartAnalysis(
                gameState,
                D2Civilization3System.Zone1Id,
                D2Civilization3System.HighQualityId
            );
            D2Civilization3System.ApplyOfflineProgress(gameState, 28.5);
            if (System.Math.Abs(state.ancientKnowledge - knowledgeBeforeBonus - 8.8) > 0.000001 ||
                zone1.researchProgress != 88.0)
            {
                return FailCivilization3Block4BValidation("hito de Conocimiento al 80% incorrecto");
            }

            string serialized = JsonUtility.ToJson(state);
            D2Civilization3State restored = JsonUtility.FromJson<D2Civilization3State>(serialized);
            D2Civilization3System.EnsureState(restored);
            D2C3ZoneState restoredZone = D2Civilization3System.GetZone(
                restored,
                D2Civilization3System.Zone1Id
            );
            if (!D2Civilization3System.ValidateState(restored, out _) ||
                !restored.archiveUnlocked || restored.archiveLevel != 2 ||
                restoredZone == null || restoredZone.researchProgress != 88.0 ||
                restoredZone.zoneResourceAmount != 12L ||
                restoredZone.totalAnalysesCompleted != 5L)
            {
                return FailCivilization3Block4BValidation("serialización de análisis incorrecta");
            }

            Debug.Log(
                "[D2 Block 4B] VALIDACIÓN LÓGICA OK: análisis manual, calidades, " +
                "consumo, recompensas, Investigación, hitos 20/40/60/80, Archivo I, " +
                "progreso offline y serialización."
            );
            return true;
        }
        finally
        {
            gameState.dimension02Unlocked = originalDimension2Unlocked;
            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(originalStateJson);
            gameState.EnsureDimension2State();
        }
    }

    private static bool FailCivilization3Block4BValidation(string reason)
    {
        Debug.LogError("[D2 Block 4B] Validación lógica fallida: " + reason + ".");
        return false;
    }

    private static bool ValidateCivilization3Block4CLogic(GameState gameState)
    {
        string originalStateJson = JsonUtility.ToJson(gameState.dimension2);
        bool originalDimension2Unlocked = gameState.dimension02Unlocked;

        try
        {
            gameState.dimension02Unlocked = true;
            Dimension2System.ResetState(gameState);
            gameState.dimension2.civilization3Unlocked = true;
            D2Civilization3State state = gameState.dimension2.civilization3;
            D2Civilization3System.EnsureState(state);
            D2C3ZoneState zone1 = D2Civilization3System.GetZone(
                state, D2Civilization3System.Zone1Id);
            D2C3ZoneState zone2 = D2Civilization3System.GetZone(
                state, D2Civilization3System.Zone2Id);

            if (D2Civilization3System.GetQualityForRoll(D2Civilization3System.Zone2Id, 0.49) !=
                    D2Civilization3System.LowQualityId ||
                D2Civilization3System.GetQualityForRoll(D2Civilization3System.Zone2Id, 0.50) !=
                    D2Civilization3System.MediumQualityId ||
                D2Civilization3System.GetQualityForRoll(D2Civilization3System.Zone2Id, 0.85) !=
                    D2Civilization3System.HighQualityId)
            {
                return FailCivilization3Block4CValidation("distribución 50/35/15 incorrecta");
            }

            D2AltarState incense = D2AltarSystem.GetAltar(
                gameState.dimension2.civilization1, D2AltarSystem.IncenseAltarId);
            D2AltarState cloth = D2AltarSystem.GetAltar(
                gameState.dimension2.civilization1, D2AltarSystem.SacredClothAltarId);
            incense.offeringAmount = 25.0;
            cloth.offeringAmount = 25.0;
            if (D2Civilization3System.TryUnlockZone2(gameState))
                return FailCivilization3Block4CValidation("Zona 2 abierta antes del 60%");
            zone1.researchProgress = 60.0;
            incense.offeringAmount = 24.0;
            if (D2Civilization3System.TryUnlockZone2(gameState))
                return FailCivilization3Block4CValidation("Zona 2 abierta sin recursos");
            incense.offeringAmount = 25.0;
            if (!D2Civilization3System.TryUnlockZone2(gameState) || !zone2.unlocked ||
                state.selectedZoneId != D2Civilization3System.Zone2Id ||
                incense.offeringAmount != 0.0 || cloth.offeringAmount != 0.0 ||
                D2Civilization3System.TryUnlockZone2(gameState) ||
                !state.archiveUnlocked || state.archiveLevel != 2)
            {
                return FailCivilization3Block4CValidation("pago, selección o Archivo II incorrectos");
            }

            if (!D2Civilization3System.TryStartExcavation(
                    gameState, D2Civilization3System.Zone1Id) ||
                !D2Civilization3System.TryStartExcavation(
                    gameState, D2Civilization3System.Zone2Id))
            {
                return FailCivilization3Block4CValidation("excavaciones paralelas no permitidas");
            }
            D2Civilization3System.ApplyOfflineProgress(gameState, 30.0);
            if (zone1.totalExcavationsCompleted != 1L ||
                zone2.totalExcavationsCompleted != 1L ||
                zone1.excavationActive || zone2.excavationActive)
            {
                return FailCivilization3Block4CValidation("progreso paralelo offline incorrecto");
            }

            D2AltarState wax = D2AltarSystem.GetAltar(
                gameState.dimension2.civilization1, D2AltarSystem.WaxAltarId);
            D2AltarState bread = D2AltarSystem.GetAltar(
                gameState.dimension2.civilization1, D2AltarSystem.RitualBreadAltarId);
            wax.offeringAmount = 19.0;
            bread.offeringAmount = 20.0;
            if (D2Civilization3System.TryHireScholar(gameState, D2Civilization3System.Zone2Id))
                return FailCivilization3Block4CValidation("Erudito 2 contratado sin coste completo");
            wax.offeringAmount = 20.0;
            if (!D2Civilization3System.TryHireScholar(
                    gameState, D2Civilization3System.Zone2Id) ||
                !zone2.scholarHired || wax.offeringAmount != 0.0 ||
                bread.offeringAmount != 0.0)
            {
                return FailCivilization3Block4CValidation("contratación del Erudito 2 incorrecta");
            }

            zone2.highQualityRemains++;
            if (!D2Civilization3System.TryStartAnalysis(
                    gameState,
                    D2Civilization3System.Zone2Id,
                    D2Civilization3System.HighQualityId))
            {
                return FailCivilization3Block4CValidation("análisis de Zona 2 no iniciado");
            }
            D2Civilization3System.ApplyOfflineProgress(gameState, 30.0);
            if (state.ancientKnowledge != 8.0 || zone2.zoneResourceAmount != 4L ||
                zone2.researchProgress != 8.0 || zone2.anomalyClues != 0L ||
                state.anomalyClueDetectionUnlocked)
            {
                return FailCivilization3Block4CValidation("recompensas previas a Indicios incorrectas");
            }

            zone2.researchProgress = 20.0;
            zone2.anomalyClueProgress = 0.82;
            zone2.highQualityRemains++;
            D2Civilization3System.EnsureState(state);
            D2Civilization3System.TryStartAnalysis(
                gameState,
                D2Civilization3System.Zone2Id,
                D2Civilization3System.HighQualityId);
            D2Civilization3System.ApplyOfflineProgress(gameState, 30.0);
            if (!state.anomalyClueDetectionUnlocked || zone2.anomalyClues != 1L ||
                zone2.anomalyClueProgress > 0.000001)
            {
                return FailCivilization3Block4CValidation("Indicio Simbólico acumulado incorrecto");
            }

            zone1.scholarHired = true;
            zone1.lowQualityRemains++;
            zone1.anomalyClueProgress = 0.97;
            D2Civilization3System.TryStartAnalysis(
                gameState,
                D2Civilization3System.Zone1Id,
                D2Civilization3System.LowQualityId);
            D2Civilization3System.ApplyOfflineProgress(gameState, 28.5);
            if (zone1.anomalyClues != 1L || zone1.anomalyClueProgress > 0.000001)
            {
                return FailCivilization3Block4CValidation("Indicio Básico acumulado incorrecto");
            }

            string serialized = JsonUtility.ToJson(state);
            D2Civilization3State restored = JsonUtility.FromJson<D2Civilization3State>(serialized);
            D2Civilization3System.EnsureState(restored);
            D2C3ZoneState restoredZone2 = D2Civilization3System.GetZone(
                restored, D2Civilization3System.Zone2Id);
            if (!D2Civilization3System.ValidateState(restored, out _) ||
                restoredZone2 == null || !restoredZone2.unlocked ||
                !restoredZone2.scholarHired || restoredZone2.anomalyClues != 1L ||
                !restored.anomalyClueDetectionUnlocked || restored.archiveLevel != 2)
            {
                return FailCivilization3Block4CValidation("serialización de Zona 2 incorrecta");
            }

            Debug.Log(
                "[D2 Block 4C] VALIDACIÓN LÓGICA OK: Zona 2, pago 25/25, " +
                "calidad 50/35/15, simultaneidad entre zonas, Erudito 20/20, " +
                "Archivo II, Indicios Básicos/Simbólicos, acumulación, offline y serialización."
            );
            return true;
        }
        finally
        {
            gameState.dimension02Unlocked = originalDimension2Unlocked;
            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(originalStateJson);
            gameState.EnsureDimension2State();
        }
    }

    private static bool FailCivilization3Block4CValidation(string reason)
    {
        Debug.LogError("[D2 Block 4C] Validación lógica fallida: " + reason + ".");
        return false;
    }

    private static bool ValidateCivilization3Block4DLogic(GameState gameState)
    {
        string originalStateJson = JsonUtility.ToJson(gameState.dimension2);
        bool originalDimension2Unlocked = gameState.dimension02Unlocked;

        try
        {
            gameState.dimension02Unlocked = true;
            Dimension2System.ResetState(gameState);
            gameState.dimension2.civilization3Unlocked = true;
            D2Civilization3State state = gameState.dimension2.civilization3;
            D2Civilization3System.EnsureState(state);
            D2C3ZoneState zone1 = D2Civilization3System.GetZone(
                state, D2Civilization3System.Zone1Id);
            D2C3ZoneState zone2 = D2Civilization3System.GetZone(
                state, D2Civilization3System.Zone2Id);
            D2C3ZoneState zone3 = D2Civilization3System.GetZone(
                state, D2Civilization3System.Zone3Id);

            zone2.unlocked = true;
            state.anomalyClueDetectionUnlocked = true;
            state.archiveUnlocked = true;
            state.archiveLevel = 2;
            zone1.scholarHired = true;
            zone2.scholarHired = true;
            zone1.anomalyClues = 7L;
            zone2.anomalyClues = 9L;
            zone1.anomalyClueProgress = 0.97;
            zone2.anomalyClueProgress = 0.97;
            zone1.lowQualityRemains = 1L;
            zone2.lowQualityRemains = 1L;
            zone2.researchProgress = 39.0;
            D2Civilization3System.EnsureState(state);

            if (zone1.anomalyRevealed || zone2.anomalyRevealed || state.archiveLevel != 2)
                return FailCivilization3Block4DValidation("revelado o Archivo III anticipado");
            if (!D2Civilization3System.TryStartAnalysis(
                    gameState, D2Civilization3System.Zone1Id,
                    D2Civilization3System.LowQualityId) ||
                !D2Civilization3System.TryStartAnalysis(
                    gameState, D2Civilization3System.Zone2Id,
                    D2Civilization3System.LowQualityId))
            {
                return FailCivilization3Block4DValidation("análisis paralelos de preparación no iniciados");
            }

            D2Civilization3System.ApplyOfflineProgress(gameState, 30.0);
            if (!zone1.anomalyRevealed || zone1.anomalyClues != 8L ||
                !zone2.anomalyRevealed || zone2.anomalyClues != 10L ||
                state.archiveLevel != 3 || zone2.researchProgress != 40.0)
            {
                return FailCivilization3Block4DValidation(
                    "revelado 8/10, progreso offline o Archivo III incorrecto");
            }

            state.ancientKnowledge = 65.0;
            zone1.zoneResourceAmount = 15L;
            zone2.zoneResourceAmount = 24L;
            if (!D2Civilization3System.TryReadAnomaly(
                    gameState, D2Civilization3System.Zone1Id) ||
                !zone1.anomalyRead || zone1.anomalousData != 1L ||
                state.ancientKnowledge != 40.0 || zone1.zoneResourceAmount != 0L ||
                zone1.anomalyClues != 8L)
            {
                return FailCivilization3Block4DValidation(
                    "lectura, coste o conservación de Indicios Básicos incorrectos");
            }
            if (D2Civilization3System.TryReadAnomaly(
                    gameState, D2Civilization3System.Zone1Id) ||
                D2Civilization3System.TryReadAnomaly(
                    gameState, D2Civilization3System.Zone2Id))
            {
                return FailCivilization3Block4DValidation(
                    "relectura única o bloqueo por coste Simbólico incorrecto");
            }

            zone2.zoneResourceAmount = 25L;
            if (!D2Civilization3System.TryReadAnomaly(
                    gameState, D2Civilization3System.Zone2Id) ||
                !zone2.anomalyRead || zone2.anomalousData != 1L ||
                state.ancientKnowledge != 0.0 || zone2.zoneResourceAmount != 0L ||
                zone2.anomalyClues != 10L ||
                D2Civilization3System.CanReadAnomaly(
                    gameState, D2Civilization3System.Zone3Id))
            {
                return FailCivilization3Block4DValidation(
                    "lectura Simbólica, costes o reserva Profunda incorrectos");
            }

            string serialized = JsonUtility.ToJson(state);
            D2Civilization3State restored = JsonUtility.FromJson<D2Civilization3State>(serialized);
            D2Civilization3System.EnsureState(restored);
            D2C3ZoneState restoredZone1 = D2Civilization3System.GetZone(
                restored, D2Civilization3System.Zone1Id);
            D2C3ZoneState restoredZone2 = D2Civilization3System.GetZone(
                restored, D2Civilization3System.Zone2Id);
            if (!D2Civilization3System.ValidateState(restored, out _) ||
                restored.progressVersion != Dimension2System.Civilization3ProgressVersion ||
                restored.archiveLevel != 3 || !restoredZone1.anomalyRead ||
                restoredZone1.anomalousData != 1L || !restoredZone2.anomalyRead ||
                restoredZone2.anomalousData != 1L || zone3.anomalyRead)
            {
                return FailCivilization3Block4DValidation(
                    "migración o serialización de Anomalías incorrecta");
            }

            Debug.Log(
                "[D2 Block 4D] VALIDACIÓN LÓGICA OK: revelado único 8/10, " +
                "Archivo III, lecturas únicas, costes 25+15/40+25, Datos Anómalos, " +
                "conservación de Indicios, offline, migración y serialización."
            );
            return true;
        }
        finally
        {
            gameState.dimension02Unlocked = originalDimension2Unlocked;
            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(originalStateJson);
            gameState.EnsureDimension2State();
        }
    }

    private static bool FailCivilization3Block4DValidation(string reason)
    {
        Debug.LogError("[D2 Block 4D] Validación lógica fallida: " + reason + ".");
        return false;
    }

    private static bool ValidateCivilization3Block4ELogic(GameState gameState)
    {
        string originalStateJson = JsonUtility.ToJson(gameState.dimension2);
        bool originalDimension2Unlocked = gameState.dimension02Unlocked;

        try
        {
            gameState.dimension02Unlocked = true;
            Dimension2System.ResetState(gameState);
            gameState.dimension2.civilization3Unlocked = true;
            D2Civilization3State state = gameState.dimension2.civilization3;
            D2Civilization3System.EnsureState(state);
            D2C3ZoneState zone1 = D2Civilization3System.GetZone(
                state, D2Civilization3System.Zone1Id);
            D2C3ZoneState zone2 = D2Civilization3System.GetZone(
                state, D2Civilization3System.Zone2Id);
            D2C3ZoneState zone3 = D2Civilization3System.GetZone(
                state, D2Civilization3System.Zone3Id);
            D2AltarState incense = D2AltarSystem.GetAltar(
                gameState.dimension2.civilization1, D2AltarSystem.IncenseAltarId);
            D2AltarState cloth = D2AltarSystem.GetAltar(
                gameState.dimension2.civilization1, D2AltarSystem.SacredClothAltarId);
            D2AltarState stone = D2AltarSystem.GetAltar(
                gameState.dimension2.civilization1, D2AltarSystem.CarvedStoneAltarId);

            zone1.researchProgress = 60.0;
            incense.offeringAmount = 25.0;
            cloth.offeringAmount = 25.0;
            if (!D2Civilization3System.TryUnlockZone2(gameState))
                return FailCivilization3Block4EValidation("preparación de Zona 2 incorrecta");

            if (D2Civilization3System.GetQualityForRoll(D2Civilization3System.Zone3Id, 0.29) !=
                    D2Civilization3System.LowQualityId ||
                D2Civilization3System.GetQualityForRoll(D2Civilization3System.Zone3Id, 0.30) !=
                    D2Civilization3System.MediumQualityId ||
                D2Civilization3System.GetQualityForRoll(D2Civilization3System.Zone3Id, 0.75) !=
                    D2Civilization3System.HighQualityId)
            {
                return FailCivilization3Block4EValidation("distribución 30/45/25 incorrecta");
            }

            zone2.researchProgress = 59.0;
            incense.offeringAmount = 50.0;
            cloth.offeringAmount = 50.0;
            stone.offeringAmount = 50.0;
            if (D2Civilization3System.TryUnlockZone3(gameState))
                return FailCivilization3Block4EValidation("Zona 3 abierta antes del 60%");
            zone2.researchProgress = 60.0;
            stone.offeringAmount = 49.0;
            if (D2Civilization3System.TryUnlockZone3(gameState))
                return FailCivilization3Block4EValidation("Zona 3 abierta sin coste completo");
            stone.offeringAmount = 50.0;
            if (!D2Civilization3System.TryUnlockZone3(gameState) || !zone3.unlocked ||
                state.selectedZoneId != D2Civilization3System.Zone3Id ||
                incense.offeringAmount != 0.0 || cloth.offeringAmount != 0.0 ||
                stone.offeringAmount != 0.0 ||
                D2Civilization3System.TryUnlockZone3(gameState))
            {
                return FailCivilization3Block4EValidation(
                    "desbloqueo, costes 50/50/50 o selección incorrectos");
            }

            if (!D2Civilization3System.TryStartExcavation(
                    gameState, D2Civilization3System.Zone1Id) ||
                !D2Civilization3System.TryStartExcavation(
                    gameState, D2Civilization3System.Zone2Id) ||
                !D2Civilization3System.TryStartExcavation(
                    gameState, D2Civilization3System.Zone3Id))
            {
                return FailCivilization3Block4EValidation(
                    "excavaciones simultáneas entre tres zonas no permitidas");
            }
            D2Civilization3System.ApplyOfflineProgress(gameState, 30.0);
            if (zone1.totalExcavationsCompleted != 1L ||
                zone2.totalExcavationsCompleted != 1L ||
                zone3.totalExcavationsCompleted != 1L || zone3.excavationActive)
            {
                return FailCivilization3Block4EValidation(
                    "progreso paralelo u offline de Zona 3 incorrecto");
            }

            D2AltarState wax = D2AltarSystem.GetAltar(
                gameState.dimension2.civilization1, D2AltarSystem.WaxAltarId);
            D2AltarState bread = D2AltarSystem.GetAltar(
                gameState.dimension2.civilization1, D2AltarSystem.RitualBreadAltarId);
            wax.offeringAmount = 29.0;
            bread.offeringAmount = 30.0;
            if (D2Civilization3System.TryHireScholar(
                    gameState, D2Civilization3System.Zone3Id))
            {
                return FailCivilization3Block4EValidation(
                    "Erudito de Sellos contratado sin coste completo");
            }
            wax.offeringAmount = 30.0;
            if (!D2Civilization3System.TryHireScholar(
                    gameState, D2Civilization3System.Zone3Id) ||
                !zone3.scholarHired || wax.offeringAmount != 0.0 ||
                bread.offeringAmount != 0.0)
            {
                return FailCivilization3Block4EValidation(
                    "contratación 30/30 del Erudito de Sellos incorrecta");
            }

            state.anomalyClueDetectionUnlocked = true;
            state.archiveUnlocked = true;
            state.archiveLevel = 3;
            zone3.researchProgress = 29.0;
            zone3.anomalyClues = 11L;
            zone3.anomalyClueProgress = 0.97;
            zone3.lowQualityRemains = 1L;
            if (!D2Civilization3System.TryStartAnalysis(
                    gameState, D2Civilization3System.Zone3Id,
                    D2Civilization3System.LowQualityId))
            {
                return FailCivilization3Block4EValidation("análisis Profundo no iniciado");
            }
            D2Civilization3System.ApplyOfflineProgress(gameState, 30.0);
            if (zone3.researchProgress != 30.0 || state.archiveLevel != 4 ||
                zone3.anomalyClues != 12L || !zone3.anomalyRevealed ||
                zone3.zoneResourceAmount < 1L)
            {
                return FailCivilization3Block4EValidation(
                    "Archivo IV, Sellos, 12 Indicios o revelado Profundo incorrectos");
            }

            state.ancientKnowledge = 60.0;
            zone3.zoneResourceAmount = 35L;
            if (!D2Civilization3System.TryReadAnomaly(
                    gameState, D2Civilization3System.Zone3Id) ||
                !zone3.anomalyRead || zone3.anomalousData != 1L ||
                state.ancientKnowledge != 0.0 || zone3.zoneResourceAmount != 0L ||
                zone3.anomalyClues != 12L ||
                D2Civilization3System.TryReadAnomaly(
                    gameState, D2Civilization3System.Zone3Id))
            {
                return FailCivilization3Block4EValidation(
                    "lectura única, coste 60+35 o Dato Profundo incorrecto");
            }

            string serialized = JsonUtility.ToJson(state);
            D2Civilization3State restored = JsonUtility.FromJson<D2Civilization3State>(serialized);
            D2Civilization3System.EnsureState(restored);
            D2C3ZoneState restoredZone3 = D2Civilization3System.GetZone(
                restored, D2Civilization3System.Zone3Id);
            if (!D2Civilization3System.ValidateState(restored, out _) ||
                restored.progressVersion != Dimension2System.Civilization3ProgressVersion ||
                restoredZone3 == null || !restoredZone3.unlocked ||
                !restoredZone3.scholarHired || restored.archiveLevel != 4 ||
                !restoredZone3.anomalyRead || restoredZone3.anomalousData != 1L)
            {
                return FailCivilization3Block4EValidation(
                    "migración o serialización de Zona 3 incorrecta");
            }

            Debug.Log(
                "[D2 Block 4E] VALIDACIÓN LÓGICA OK: Zona 3, costes 50/50/50, " +
                "calidad 30/45/25, simultaneidad, Erudito 30/30, Sellos, Archivo IV, " +
                "12 Indicios, Anomalía y Dato Profundos, offline, migración y serialización."
            );
            return true;
        }
        finally
        {
            gameState.dimension02Unlocked = originalDimension2Unlocked;
            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(originalStateJson);
            gameState.EnsureDimension2State();
        }
    }

    private static bool FailCivilization3Block4EValidation(string reason)
    {
        Debug.LogError("[D2 Block 4E] Validación lógica fallida: " + reason + ".");
        return false;
    }

    private static bool ValidateCivilization3Block4FLogic(GameState gameState)
    {
        string originalStateJson = JsonUtility.ToJson(gameState.dimension2);
        bool originalDimension2Unlocked = gameState.dimension02Unlocked;

        try
        {
            gameState.dimension02Unlocked = true;
            Dimension2System.ResetState(gameState);
            gameState.dimension2.civilization3Unlocked = true;
            D2Civilization3State state = gameState.dimension2.civilization3;
            D2Civilization3System.EnsureState(state);
            D2C3ZoneState zone1 = D2Civilization3System.GetZone(
                state, D2Civilization3System.Zone1Id);
            D2C3ZoneState zone2 = D2Civilization3System.GetZone(
                state, D2Civilization3System.Zone2Id);
            D2C3ZoneState zone3 = D2Civilization3System.GetZone(
                state, D2Civilization3System.Zone3Id);

            if (state.entityResearchUnlocked ||
                D2Civilization3System.TryStartEntityResearch(gameState))
            {
                return FailCivilization3Block4FValidation(
                    "Investigación desbloqueada sin los tres Datos");
            }
            zone1.anomalousData = 1L;
            zone2.anomalousData = 1L;
            zone3.anomalousData = 1L;
            D2Civilization3System.EnsureState(state);
            if (!state.entityResearchUnlocked)
                return FailCivilization3Block4FValidation("desbloqueo con tres Datos incorrecto");

            state.ancientKnowledge = 40.0;
            if (!D2Civilization3System.TryStartEntityResearch(gameState))
                return FailCivilization3Block4FValidation("inicio manual incorrecto");
            D2Civilization3System.ApplyOfflineProgress(gameState, 300.0);
            if (System.Math.Abs(state.entityResearchProgress - 10.0) > 0.000001 ||
                System.Math.Abs(state.ancientKnowledge - 30.0) > 0.000001 ||
                !state.entityResearchActive)
            {
                return FailCivilization3Block4FValidation(
                    "conversión, velocidad o progreso offline incorrectos");
            }
            if (!D2Civilization3System.TryPauseEntityResearch(gameState))
                return FailCivilization3Block4FValidation("pausa manual incorrecta");
            D2Civilization3System.ApplyOfflineProgress(gameState, 300.0);
            if (System.Math.Abs(state.entityResearchProgress - 10.0) > 0.000001)
                return FailCivilization3Block4FValidation("progreso durante pausa incorrecto");

            D2Civilization3System.TryStartEntityResearch(gameState);
            D2Civilization3System.ApplyOfflineProgress(gameState, 600.0);
            if (System.Math.Abs(state.entityResearchProgress - 30.0) > 0.000001 ||
                System.Math.Abs(state.ancientKnowledge - 10.0) > 0.000001 ||
                state.entityResearchActive ||
                D2Civilization3System.TryStartEntityResearch(gameState))
            {
                return FailCivilization3Block4FValidation(
                    "detención exacta o bloqueo del hito 30 incorrecto");
            }
            if (D2Civilization3System.TryPayEntityResearchMilestone(gameState))
                return FailCivilization3Block4FValidation("hito 30 pagado sin recursos");
            zone1.zoneResourceAmount = 25L;
            if (!D2Civilization3System.TryPayEntityResearchMilestone(gameState) ||
                !state.entityResearchMilestone30Completed ||
                zone1.zoneResourceAmount != 0L || zone1.anomalousData != 0L ||
                state.entityKnowledge != 1L)
            {
                return FailCivilization3Block4FValidation("coste o recompensa del hito 30 incorrectos");
            }

            state.ancientKnowledge = 0.0;
            if (!D2Civilization3System.TryStartEntityResearch(gameState))
                return FailCivilization3Block4FValidation("inicio sin combustible incorrecto");
            D2Civilization3System.ApplyOfflineProgress(gameState, 600.0);
            if (state.entityResearchProgress != 30.0 || !state.entityResearchActive)
                return FailCivilization3Block4FValidation("espera sin combustible incorrecta");
            state.ancientKnowledge = 30.0;
            D2Civilization3System.ApplyOfflineProgress(gameState, 900.0);
            if (state.entityResearchProgress != 60.0 || state.ancientKnowledge != 0.0 ||
                state.entityResearchActive)
            {
                return FailCivilization3Block4FValidation(
                    "reanudación automática o hito 60 incorrecto");
            }
            zone2.zoneResourceAmount = 35L;
            if (!D2Civilization3System.TryPayEntityResearchMilestone(gameState) ||
                zone2.zoneResourceAmount != 0L || zone2.anomalousData != 0L ||
                state.entityKnowledge != 3L)
            {
                return FailCivilization3Block4FValidation("coste o recompensa del hito 60 incorrectos");
            }

            state.ancientKnowledge = 25.0;
            D2Civilization3System.TryStartEntityResearch(gameState);
            D2Civilization3System.ApplyOfflineProgress(gameState, 750.0);
            zone3.zoneResourceAmount = 45L;
            if (state.entityResearchProgress != 85.0 ||
                !D2Civilization3System.TryPayEntityResearchMilestone(gameState) ||
                zone3.zoneResourceAmount != 0L || zone3.anomalousData != 0L ||
                state.entityKnowledge != 6L)
            {
                return FailCivilization3Block4FValidation("hito 85 incorrecto");
            }

            state.ancientKnowledge = 15.0;
            D2Civilization3System.TryStartEntityResearch(gameState);
            D2Civilization3System.ApplyOfflineProgress(gameState, 450.0);
            zone1.zoneResourceAmount = 50L;
            zone2.zoneResourceAmount = 50L;
            zone3.zoneResourceAmount = 49L;
            if (state.entityResearchProgress != 100.0 ||
                D2Civilization3System.TryPayEntityResearchMilestone(gameState))
            {
                return FailCivilization3Block4FValidation("hito 100 aceptado sin recursos");
            }
            zone3.zoneResourceAmount = 50L;
            if (!D2Civilization3System.TryPayEntityResearchMilestone(gameState) ||
                !state.entityResearchMilestone100Completed || !state.entityPactAvailable ||
                zone1.zoneResourceAmount != 0L || zone2.zoneResourceAmount != 0L ||
                zone3.zoneResourceAmount != 0L || state.entityKnowledge != 6L ||
                D2Civilization3System.TryPayEntityResearchMilestone(gameState))
            {
                return FailCivilization3Block4FValidation(
                    "coste, permanencia o preparación del Pacto al 100% incorrectos");
            }

            string serialized = JsonUtility.ToJson(state);
            D2Civilization3State restored = JsonUtility.FromJson<D2Civilization3State>(serialized);
            D2Civilization3System.EnsureState(restored);
            if (!D2Civilization3System.ValidateState(restored, out _) ||
                restored.progressVersion != Dimension2System.Civilization3ProgressVersion ||
                !restored.entityResearchUnlocked || restored.entityResearchProgress != 100.0 ||
                restored.entityKnowledge != 6L || !restored.entityPactAvailable ||
                !restored.entityResearchMilestone100Completed)
            {
                return FailCivilization3Block4FValidation(
                    "migración o serialización de Investigación incorrecta");
            }

            Debug.Log(
                "[D2 Block 4F] VALIDACIÓN LÓGICA OK: desbloqueo con tres Datos, " +
                "conversión 1:1, 1%/30 s, inicio/pausa/espera, offline, hitos " +
                "30/60/85/100, costes, Conocimiento 1+2+3, Pacto preparado, migración y serialización."
            );
            return true;
        }
        finally
        {
            gameState.dimension02Unlocked = originalDimension2Unlocked;
            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(originalStateJson);
            gameState.EnsureDimension2State();
        }
    }

    private static bool FailCivilization3Block4FValidation(string reason)
    {
        Debug.LogError("[D2 Block 4F] Validación lógica fallida: " + reason + ".");
        return false;
    }

    private static bool ValidateCivilization3Block4GLogic(GameState gameState)
    {
        string originalStateJson = JsonUtility.ToJson(gameState.dimension2);
        bool originalDimension2Unlocked = gameState.dimension02Unlocked;
        try
        {
            gameState.dimension02Unlocked = true;
            Dimension2System.ResetState(gameState);
            gameState.dimension2.civilization3Unlocked = true;
            D2Civilization3State state = gameState.dimension2.civilization3;
            D2Civilization3System.EnsureState(state);
            state.entityPactAvailable = true;
            state.entityResearchMilestone100Completed = true;
            state.entityResearchProgress = 100.0;
            D2Civilization3System.EnsureState(state);

            if (!D2Civilization3System.TryEstablishEntityPact(gameState) ||
                !state.entityPactEstablished ||
                D2Civilization3System.TryEstablishEntityPact(gameState))
            {
                return FailCivilization3Block4GValidation("establecimiento del Pacto incorrecto");
            }

            state.entityKnowledge = 0L;
            state.ancientKnowledge = 1000.0;
            foreach (string zoneId in D2Civilization3System.ZoneIds)
                D2Civilization3System.GetZone(state, zoneId).zoneResourceAmount = 1000L;
            if (D2Civilization3System.TryUpgradeEntityPactLine(
                gameState, D2Civilization3System.ResonantExpeditionLineId))
            {
                return FailCivilization3Block4GValidation("umbral no gastable ignorado");
            }

            state.entityKnowledge = 6L;
            state.ancientKnowledge = 300.0;
            foreach (string zoneId in D2Civilization3System.ZoneIds)
                D2Civilization3System.GetZone(state, zoneId).zoneResourceAmount = 150L;
            for (int level = 1; level <= 3; level++)
            {
                if (!D2Civilization3System.TryUpgradeEntityPactLine(
                    gameState, D2Civilization3System.ResonantExpeditionLineId))
                {
                    return FailCivilization3Block4GValidation("mejora de Expedicion incorrecta");
                }
            }
            if (state.entityKnowledge != 6L || state.ancientKnowledge != 0.0 ||
                D2Civilization3System.GetZone(state, D2Civilization3System.Zone1Id)
                    .zoneResourceAmount != 0L ||
                D2Civilization3System.GetEntityPactLineLevel(
                    state, D2Civilization3System.ResonantExpeditionLineId) != 3)
            {
                return FailCivilization3Block4GValidation("costes o permanencia incorrectos");
            }

            foreach (D2EntityPactLineState line in state.entityPactLines)
            {
                if (line.lineId == D2Civilization3System.EndlessArchiveLineId)
                    line.level = 2;
                else if (line.lineId == D2Civilization3System.SharedMemoryLineId)
                    line.level = 3;
            }
            foreach (string zoneId in D2Civilization3System.ZoneIds)
                D2Civilization3System.GetZone(state, zoneId).researchProgress = 100.0;
            D2C3ZoneState zone1 = D2Civilization3System.GetZone(
                state, D2Civilization3System.Zone1Id);
            if (System.Math.Abs(D2Civilization3System.GetSharedMemoryMultiplier(gameState) - 1.09) > 0.000001 ||
                System.Math.Abs(D2Civilization3System.GetAnalysisRewardMultiplier(state) - 1.30) > 0.000001 ||
                System.Math.Abs(D2Civilization3System.GetAnalysisDuration(state, zone1) - 25.65) > 0.000001 ||
                System.Math.Abs(D2Civilization3System.GetExtraRemainsProgressPerExcavation(state, zone1) - 0.45) > 0.000001)
            {
                return FailCivilization3Block4GValidation("bonos de lineas o Zonas 100 incorrectos");
            }

            D2Civilization1State civ1 = gameState.dimension2.civilization1;
            double initialArrivalProgress = civ1.followerArrivalProgress;
            long initialFollowers = civ1.totalFollowersReceived;
            double baseArrival = D2Civilization1System.GetFollowerArrivalPerSecond(civ1);
            D2Civilization1System.Tick(gameState, 10.0);
            double produced = (civ1.totalFollowersReceived - initialFollowers) +
                (civ1.followerArrivalProgress - initialArrivalProgress);
            if (System.Math.Abs(produced - baseArrival * 1.09 * 10.0) > 0.000001)
                return FailCivilization3Block4GValidation("Memoria no alcanzo llegada de Seguidores");

            string serialized = JsonUtility.ToJson(state);
            D2Civilization3State restored = JsonUtility.FromJson<D2Civilization3State>(serialized);
            D2Civilization3System.EnsureState(restored);
            if (!D2Civilization3System.ValidateState(restored, out _) ||
                restored.progressVersion != Dimension2System.Civilization3ProgressVersion ||
                !restored.entityPactEstablished ||
                D2Civilization3System.GetEntityPactLineLevel(
                    restored, D2Civilization3System.SharedMemoryLineId) != 3)
            {
                return FailCivilization3Block4GValidation("serializacion del Pacto incorrecta");
            }

            Debug.Log("[D2 Block 4G] VALIDACION LOGICA OK: Pacto, tres lineas, niveles 1/3/6, costes, bonos de Zonas 100, Memoria Compartida y serializacion.");
            return true;
        }
        finally
        {
            gameState.dimension02Unlocked = originalDimension2Unlocked;
            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(originalStateJson);
            gameState.EnsureDimension2State();
        }
    }

    private static bool FailCivilization3Block4GValidation(string reason)
    {
        Debug.LogError("[D2 Block 4G] Validacion logica fallida: " + reason + ".");
        return false;
    }

    private static bool ValidateCivilization3Block4HLogic(GameState gameState)
    {
        string originalStateJson = JsonUtility.ToJson(gameState.dimension2);
        bool originalDimension2Unlocked = gameState.dimension02Unlocked;
        try
        {
            gameState.dimension02Unlocked = true;
            Dimension2System.ResetState(gameState);
            gameState.dimension2.civilization3Unlocked = true;
            D2Civilization3State state = gameState.dimension2.civilization3;
            D2Civilization3System.EnsureState(state);
            state.archiveUnlocked = true;
            state.archiveLevel = 4;
            state.entityKnowledge = 6L;
            state.ancientKnowledge = 315.0;
            D2C3ZoneState zone1 = D2Civilization3System.GetZone(
                state, D2Civilization3System.Zone1Id);
            D2C3ZoneState zone2 = D2Civilization3System.GetZone(
                state, D2Civilization3System.Zone2Id);
            D2C3ZoneState zone3 = D2Civilization3System.GetZone(
                state, D2Civilization3System.Zone3Id);
            zone2.unlocked = true;
            zone3.unlocked = true;
            zone1.scholarHired = true;
            zone1.scholarLevel = 1;
            zone2.scholarHired = true;
            zone2.scholarLevel = 1;
            zone3.scholarHired = true;
            zone3.scholarLevel = 1;
            zone1.zoneResourceAmount = 85L;
            zone2.zoneResourceAmount = 35L;
            zone3.zoneResourceAmount = 50L;
            D2Civilization3System.EnsureState(state);

            if (!D2Civilization3System.TryUpgradeScholar(
                    gameState, D2Civilization3System.Zone1Id) ||
                !D2Civilization3System.TryUpgradeScholar(
                    gameState, D2Civilization3System.Zone1Id) ||
                zone1.scholarLevel != 3 || state.entityKnowledge != 6L ||
                System.Math.Abs(state.ancientKnowledge - 225.0) > 0.000001 ||
                zone1.zoneResourceAmount != 25L ||
                System.Math.Abs(D2Civilization3System.GetAnalysisDuration(state, zone1) - 27.0) > 0.000001 ||
                System.Math.Abs(D2Civilization3System.GetScholarRewardMultiplier(zone1) - 1.10) > 0.000001)
            {
                return FailCivilization3Block4HValidation(
                    "niveles, costes o efectos del Erudito incorrectos");
            }

            foreach (string upgradeId in D2Civilization3System.ArchiveUpgradeIds)
            {
                if (!D2Civilization3System.TryUnlockArchiveUpgrade(gameState, upgradeId))
                    return FailCivilization3Block4HValidation(
                        "desbloqueo o coste de una mejora del Archivo incorrecto");
            }
            if (state.entityKnowledge != 6L || state.ancientKnowledge != 0.0 ||
                zone1.zoneResourceAmount != 0L || zone2.zoneResourceAmount != 0L ||
                zone3.zoneResourceAmount != 0L ||
                System.Math.Abs(D2Civilization3System.GetExcavationDuration(state) - 28.5) > 0.000001 ||
                System.Math.Abs(D2Civilization3System.GetEffectiveAnomalyKnowledgeCost(
                    state, D2Civilization3System.Zone1Id) - 22.5) > 0.000001 ||
                D2Civilization3System.GetEffectiveAnomalyResourceCost(
                    state, D2Civilization3System.Zone1Id) != 14L)
            {
                return FailCivilization3Block4HValidation(
                    "efectos, costes o permanencia del Conocimiento del Ente incorrectos");
            }

            zone2.highQualityRemains = 1L;
            zone2.anomalyClueProgress = 0.0;
            state.anomalyClueDetectionUnlocked = true;
            if (!D2Civilization3System.TryStartAnalysis(
                    gameState, D2Civilization3System.Zone2Id,
                    D2Civilization3System.HighQualityId))
            {
                return FailCivilization3Block4HValidation(
                    "análisis para Concordancia no iniciado");
            }
            D2Civilization3System.ApplyOfflineProgress(
                gameState, D2Civilization3System.GetAnalysisDuration(state, zone2));
            if (System.Math.Abs(zone2.anomalyClueProgress - 0.198) > 0.000001)
                return FailCivilization3Block4HValidation(
                    "Concordancia Anómala no mejoró los Indicios offline");

            string serialized = JsonUtility.ToJson(state);
            D2Civilization3State restored = JsonUtility.FromJson<D2Civilization3State>(serialized);
            D2Civilization3System.EnsureState(restored);
            if (!D2Civilization3System.ValidateState(restored, out _) ||
                restored.progressVersion != Dimension2System.Civilization3ProgressVersion ||
                !restored.stratifiedCartographyUnlocked ||
                !restored.anomalousConcordanceUnlocked ||
                !restored.deepExegesisUnlocked ||
                D2Civilization3System.GetZone(
                    restored, D2Civilization3System.Zone1Id).scholarLevel != 3)
            {
                return FailCivilization3Block4HValidation(
                    "serialización de Eruditos o Archivo incorrecta");
            }

            D2C3ZoneState legacyScholar = D2Civilization3System.GetZone(
                restored, D2Civilization3System.Zone2Id);
            legacyScholar.scholarHired = true;
            legacyScholar.scholarLevel = 0;
            restored.progressVersion = 10;
            D2Civilization3System.EnsureState(restored);
            if (legacyScholar.scholarLevel != 1 ||
                restored.progressVersion != Dimension2System.Civilization3ProgressVersion)
            {
                return FailCivilization3Block4HValidation(
                    "migración de Eruditos contratados incorrecta");
            }

            Debug.Log(
                "[D2 Block 4H] VALIDACIÓN LÓGICA OK: Eruditos 1-3, Archivo, " +
                "costes, efectos, offline, migración y serialización."
            );
            return true;
        }
        finally
        {
            gameState.dimension02Unlocked = originalDimension2Unlocked;
            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(originalStateJson);
            gameState.EnsureDimension2State();
        }
    }

    private static bool FailCivilization3Block4HValidation(string reason)
    {
        Debug.LogError("[D2 Block 4H] Validación lógica fallida: " + reason + ".");
        return false;
    }

    private static bool ValidateCivilization3Block5DLogic(GameState gameState)
    {
        string originalStateJson = JsonUtility.ToJson(gameState.dimension2);
        bool originalDimension2Unlocked = gameState.dimension02Unlocked;
        bool originalDimension1Unlocked = gameState.dimension01Unlocked;
        int originalD1TreePoints = gameState.d1TreePoints;
        double originalMaxLE = gameState.maxLEAlcanzado;
        double originalLE = gameState.LE;

        try
        {
            gameState.dimension02Unlocked = true;
            gameState.dimension01Unlocked = false;
            gameState.maxLEAlcanzado = 0.0;
            gameState.LE = 0.0;
            gameState.d1TreePoints = 0;
            Dimension2System.ResetState(gameState);
            gameState.dimension2.civilization3Unlocked = true;
            D2Civilization3State state = gameState.dimension2.civilization3;
            D2Civilization3System.EnsureState(state);
            state.entityPactAvailable = true;
            state.entityResearchMilestone100Completed = true;
            state.entityResearchProgress = 100.0;
            D2Civilization3System.EnsureState(state);

            if (!D2Civilization3System.TryEstablishEntityPact(gameState) ||
                state.entityPactLines.Count != 5)
            {
                return FailCivilization3Block5DValidation(
                    "establecimiento o catálogo de cinco líneas incorrecto");
            }

            state.entityKnowledge = 6L;
            state.ancientKnowledge = 600.0;
            foreach (string zoneId in D2Civilization3System.ZoneIds)
                D2Civilization3System.GetZone(state, zoneId).zoneResourceAmount = 300L;

            foreach (string lineId in new[]
            {
                D2Civilization3System.ModulatorResonanceLineId,
                D2Civilization3System.FirstThresholdChronicleLineId
            })
            {
                for (int expectedLevel = 1; expectedLevel <= 3; expectedLevel++)
                {
                    if (!D2Civilization3System.TryUpgradeEntityPactLine(gameState, lineId) ||
                        D2Civilization3System.GetEntityPactLineLevel(state, lineId) != expectedLevel)
                    {
                        return FailCivilization3Block5DValidation(
                            "costes, umbrales o niveles de una línea externa incorrectos");
                    }
                }
            }

            if (state.entityKnowledge != 6L || state.ancientKnowledge != 0.0 ||
                D2Civilization3System.GetZone(state, D2Civilization3System.Zone1Id)
                    .zoneResourceAmount != 0L ||
                System.Math.Abs(
                    D2Civilization3System.GetModulatorCalibrationMultiplier(gameState) - 1.15
                ) > 0.000001 ||
                D2Civilization3System.GetPrestige1PreviewBonus(gameState) != 0 ||
                Dimension1System.CalculateD1TreePointsFromProgress(gameState) != 0 ||
                gameState.d1TreePoints != 0)
            {
                return FailCivilization3Block5DValidation(
                    "efectos externos, permanencia del umbral o cobro anticipado incorrectos");
            }

            string serialized = JsonUtility.ToJson(state);
            D2Civilization3State restored = JsonUtility.FromJson<D2Civilization3State>(serialized);
            D2Civilization3System.EnsureState(restored);
            if (!D2Civilization3System.ValidateState(restored, out _) ||
                restored.progressVersion != Dimension2System.Civilization3ProgressVersion ||
                restored.entityPactLines.Count != 5 ||
                D2Civilization3System.GetEntityPactLineLevel(
                    restored, D2Civilization3System.ModulatorResonanceLineId) != 3 ||
                D2Civilization3System.GetEntityPactLineLevel(
                    restored, D2Civilization3System.FirstThresholdChronicleLineId) != 3)
            {
                return FailCivilization3Block5DValidation(
                    "serialización de las cinco líneas incorrecta");
            }

            restored.entityPactLines.RemoveAll(line =>
                line != null &&
                (line.lineId == D2Civilization3System.ModulatorResonanceLineId ||
                 line.lineId == D2Civilization3System.FirstThresholdChronicleLineId));
            restored.progressVersion = 9;
            D2Civilization3System.EnsureState(restored);
            if (restored.progressVersion != Dimension2System.Civilization3ProgressVersion ||
                restored.entityPactLines.Count != 5 ||
                D2Civilization3System.GetEntityPactLineLevel(
                    restored, D2Civilization3System.ResonantExpeditionLineId) != 0 ||
                D2Civilization3System.GetEntityPactLineLevel(
                    restored, D2Civilization3System.ModulatorResonanceLineId) != 0)
            {
                return FailCivilization3Block5DValidation(
                    "migración de tres a cinco líneas incorrecta");
            }

            Debug.Log(
                "[D2 Block 5D] VALIDACIÓN LÓGICA OK: cinco líneas, costes, " +
                "calibración del Modulador, vista previa P1, migración y serialización."
            );
            return true;
        }
        finally
        {
            gameState.dimension02Unlocked = originalDimension2Unlocked;
            gameState.dimension01Unlocked = originalDimension1Unlocked;
            gameState.d1TreePoints = originalD1TreePoints;
            gameState.maxLEAlcanzado = originalMaxLE;
            gameState.LE = originalLE;
            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(originalStateJson);
            gameState.EnsureDimension2State();
        }
    }

    private static bool FailCivilization3Block5DValidation(string reason)
    {
        Debug.LogError("[D2 Block 5D] Validación lógica fallida: " + reason + ".");
        return false;
    }

    private static bool ValidateDimension2Block5ELogic(GameState gameState)
    {
        string originalStateJson = JsonUtility.ToJson(gameState.dimension2);
        bool originalDimension2Unlocked = gameState.dimension02Unlocked;
        try
        {
            gameState.dimension02Unlocked = true;
            Dimension2System.ResetState(gameState);
            gameState.dimension2.civilization2Unlocked = true;
            gameState.dimension2.civilization3Unlocked = true;
            gameState.EnsureDimension2State();

            D2Civilization1State civ1 = gameState.dimension2.civilization1;
            D2Civilization2State civ2 = gameState.dimension2.civilization2;
            D2Civilization3State civ3 = gameState.dimension2.civilization3;

            civ1.entityContactAvailable = true;
            civ1.bondPlacePrepared = true;
            civ1.acolytesAssignedToBond = 4L;
            civ1.followersAvailable = 96L;
            civ1.followersAssignedToRefuge = 4L;
            civ1.totalFollowersReceived = 100L;
            D2AltarState wax = D2AltarSystem.GetAltar(civ1, D2AltarSystem.WaxAltarId);
            wax.unlocked = true;
            wax.followersAssigned = 4L;
            foreach (D2BondLineState line in civ1.bondLines)
            {
                if (line.lineId == D2BondSystem.SanctuaryEchoId ||
                    line.lineId == D2BondSystem.TraceLiturgyId)
                {
                    line.level = 3;
                }
            }

            civ2.entityContained = true;
            civ2.majorPactPrepared = true;
            civ2.majorPactEstablished = true;
            civ2.membersAvailable = 92L;
            civ2.membersAssignedToContainment = 4L;
            civ2.totalMembersRecruited = 100L;
            D2RegionState region1 = D2Civilization2System.GetRegion(
                civ2, D2Civilization2System.Region1Id);
            region1.membersAssigned = 4L;
            D2Civilization2System.GetOperation(
                region1, D2Civilization2System.RescueOperationId).membersAssigned = 4L;
            foreach (D2C2MajorPactLineState line in civ2.majorPactLines)
                line.level = 3;

            civ3.entityPactAvailable = true;
            civ3.entityPactEstablished = true;
            civ3.entityResearchUnlocked = true;
            civ3.entityResearchActive = true;
            civ3.entityResearchProgress = 0.0;
            civ3.ancientKnowledge = 20.0;
            foreach (D2EntityPactLineState line in civ3.entityPactLines)
                line.level = 3;
            gameState.EnsureDimension2State();

            if (System.Math.Abs(gameState.GetDimension2SanctuaryLEMultiplier() - 1.03) > 0.000001 ||
                System.Math.Abs(gameState.GetDimension2TraceMultiplier() - 1.03) > 0.000001 ||
                System.Math.Abs(gameState.GetDimension2ArtifactProductionMultiplier(
                    "vacuum_observer") - 1.06) > 0.000001 ||
                System.Math.Abs(gameState.GetDimension2TriangleEffectMultiplier() - 1.06) > 0.000001 ||
                System.Math.Abs(D2Civilization3System.GetSharedMemoryMultiplier(gameState) - 1.09) > 0.000001 ||
                System.Math.Abs(D2Civilization3System.GetModulatorCalibrationMultiplier(gameState) - 1.15) > 0.000001 ||
                D2Civilization3System.GetPrestige1PreviewBonus(gameState) != 0)
            {
                return FailDimension2Block5EValidation(
                    "matriz combinada de conexiones cruzadas incorrecta");
            }

            string baselineJson = JsonUtility.ToJson(gameState.dimension2);
            Dimension2System.Tick(gameState, 600.0);
            long onlineFollowers = civ1.followersAvailable;
            double onlineFollowerProgress = civ1.followerArrivalProgress;
            double onlineWax = wax.offeringAmount;
            double onlineBond = civ1.bondProgress;
            double onlineDominance = region1.dominance;
            double onlineThreat = region1.threat;
            double onlineMemberProgress = civ2.memberProductionProgress;
            double onlineStability = civ2.containmentStability;
            double onlineEntityResearch = civ3.entityResearchProgress;
            double onlineAncientKnowledge = civ3.ancientKnowledge;

            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(baselineJson);
            gameState.EnsureDimension2State();
            double applied = Dimension2System.ApplyOfflineProgress(gameState, 600.0);
            civ1 = gameState.dimension2.civilization1;
            civ2 = gameState.dimension2.civilization2;
            civ3 = gameState.dimension2.civilization3;
            wax = D2AltarSystem.GetAltar(civ1, D2AltarSystem.WaxAltarId);
            region1 = D2Civilization2System.GetRegion(civ2, D2Civilization2System.Region1Id);

            if (System.Math.Abs(applied - 600.0) > 0.000001 ||
                civ1.followersAvailable != onlineFollowers ||
                System.Math.Abs(civ1.followerArrivalProgress - onlineFollowerProgress) > 0.000001 ||
                System.Math.Abs(wax.offeringAmount - onlineWax) > 0.000001 ||
                System.Math.Abs(civ1.bondProgress - onlineBond) > 0.000001 ||
                System.Math.Abs(region1.dominance - onlineDominance) > 0.000001 ||
                System.Math.Abs(region1.threat - onlineThreat) > 0.000001 ||
                System.Math.Abs(civ2.memberProductionProgress - onlineMemberProgress) > 0.000001 ||
                System.Math.Abs(civ2.containmentStability - onlineStability) > 0.000001 ||
                System.Math.Abs(civ3.entityResearchProgress - onlineEntityResearch) > 0.000001 ||
                System.Math.Abs(civ3.ancientKnowledge - onlineAncientKnowledge) > 0.000001)
            {
                return FailDimension2Block5EValidation(
                    "simulación simultánea online/offline no equivalente");
            }

            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(baselineJson);
            gameState.EnsureDimension2State();
            double capped = Dimension2System.ApplyOfflineProgress(gameState, 100000.0);
            if (System.Math.Abs(capped - Dimension2System.OfflineProgressCapSeconds) > 0.000001)
                return FailDimension2Block5EValidation("límite offline de 12 horas incorrecto");

            string roundTrip = JsonUtility.ToJson(gameState.dimension2);
            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(roundTrip);
            gameState.EnsureDimension2State();
            if (!Dimension2System.ValidateState(gameState, out _))
                return FailDimension2Block5EValidation(
                    "estado combinado inválido después de guardar y cargar");

            Debug.Log(
                "[D2 Block 5E] VALIDACIÓN LÓGICA OK: tres civilizaciones " +
                "simultáneas, conexiones cruzadas, equivalencia online/offline, " +
                "límite de 12 horas y guardado integral."
            );
            return true;
        }
        finally
        {
            gameState.dimension02Unlocked = originalDimension2Unlocked;
            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(originalStateJson);
            gameState.EnsureDimension2State();
        }
    }

    private static bool FailDimension2Block5EValidation(string reason)
    {
        Debug.LogError("[D2 Block 5E] Validación lógica fallida: " + reason + ".");
        return false;
    }

    private static bool ValidateDimension2Block5FLogic(GameState gameState)
    {
        string originalStateJson = JsonUtility.ToJson(gameState.dimension2);
        bool originalDimension2Unlocked = gameState.dimension02Unlocked;
        try
        {
            gameState.dimension02Unlocked = true;
            Dimension2System.ResetState(gameState);
            if (!Dimension2System.ValidateState(gameState, out _) ||
                gameState.dimension2.civilization1.progressVersion !=
                    Dimension2System.Civilization1ProgressVersion ||
                gameState.dimension2.civilization2.progressVersion !=
                    Dimension2System.Civilization2ProgressVersion ||
                gameState.dimension2.civilization3.progressVersion !=
                    Dimension2System.Civilization3ProgressVersion)
            {
                return FailDimension2Block5FValidation(
                    "partida nueva o versiones de progreso incorrectas");
            }

            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(
                "{\"progressVersion\":0,\"selectedTerritoryId\":\"legacy_unknown\"," +
                "\"civilization1Unlocked\":false}"
            );
            gameState.EnsureDimension2State();
            D2Civilization3State migratedCiv3 = gameState.dimension2.civilization3;
            if (!Dimension2System.ValidateState(gameState, out _) ||
                gameState.dimension2.selectedTerritoryId !=
                    Dimension2System.Civilization1TerritoryId ||
                !gameState.dimension2.civilization1Unlocked ||
                gameState.dimension2.civilization1.bondLines.Count != 5 ||
                gameState.dimension2.civilization2.majorPactLines.Count != 5 ||
                migratedCiv3.entityPactLines.Count != 5 ||
                migratedCiv3.zones.Count != 3)
            {
                return FailDimension2Block5FValidation(
                    "migración desde una partida anterior a Dimensión 2 incorrecta");
            }

            foreach (D2C3ZoneState zone in migratedCiv3.zones)
            {
                if (zone == null || zone.scholarLevel != 0 || zone.scholarHired)
                    return FailDimension2Block5FValidation(
                        "estado inicial migrado de Eruditos incorrecto");
            }

            string stableJson = JsonUtility.ToJson(gameState.dimension2);
            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(stableJson);
            gameState.EnsureDimension2State();
            if (!Dimension2System.ValidateState(gameState, out _))
                return FailDimension2Block5FValidation(
                    "segunda carga de la partida migrada no fue estable");

            Debug.Log(
                "[D2 Block 5F] VALIDACIÓN LÓGICA OK: partida nueva, partida " +
                "anterior a D2, migración estable, catálogos y versiones finales."
            );
            return true;
        }
        finally
        {
            gameState.dimension02Unlocked = originalDimension2Unlocked;
            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(originalStateJson);
            gameState.EnsureDimension2State();
        }
    }

    private static bool FailDimension2Block5FValidation(string reason)
    {
        Debug.LogError("[D2 Block 5F] Validación lógica fallida: " + reason + ".");
        return false;
    }

    private static bool ValidateCivilization1Block5BLogic(GameState gameState)
    {
        string originalStateJson = JsonUtility.ToJson(gameState.dimension2);
        bool originalDimension2Unlocked = gameState.dimension02Unlocked;
        try
        {
            gameState.dimension02Unlocked = true;
            Dimension2System.ResetState(gameState);
            D2Civilization1State state = gameState.dimension2.civilization1;
            D2Civilization1System.EnsureState(state);
            if (D2BondSystem.IsMajorPactEstablished(state) ||
                D2BondSystem.CanPrepare(gameState))
            {
                return FailCivilization1Block5BValidation(
                    "pacto disponible antes del contacto");
            }

            state.trust = D2VeiledThresholdSystem.UnlockTrustRequired;
            state.novitiateLevel = D2CivilizationPactSystem.UnlockNovitiateLevelRequired;
            state.totalAcolytesCreated = 1L;
            D2VeiledThresholdSystem.EnsureState(state);
            D2AltarSystem.EnsureState(state);
            foreach (string altarId in new[]
            {
                D2AltarSystem.IncenseAltarId,
                D2AltarSystem.SacredClothAltarId,
                D2AltarSystem.CarvedStoneAltarId
            })
            {
                D2AltarSystem.GetAltar(state, altarId).offeringAmount = 100.0;
            }
            bool canPrepareMajorPact = D2BondSystem.CanPrepare(gameState);
            if (!canPrepareMajorPact || !D2BondSystem.TryPrepare(gameState) ||
                !D2BondSystem.IsMajorPactEstablished(state))
            {
                D2AltarState debugIncense = D2AltarSystem.GetAltar(
                    state, D2AltarSystem.IncenseAltarId);
                D2AltarState debugCloth = D2AltarSystem.GetAltar(
                    state, D2AltarSystem.SacredClothAltarId);
                D2AltarState debugStone = D2AltarSystem.GetAltar(
                    state, D2AltarSystem.CarvedStoneAltarId);
                return FailCivilization1Block5BValidation(
                    "establecimiento incorrecto: acceso=" + gameState.dimension02Unlocked +
                    ", contacto=" + state.entityContactAvailable +
                    ", incienso=" + debugIncense.unlocked + "/" + debugIncense.offeringAmount +
                    ", tela=" + debugCloth.unlocked + "/" + debugCloth.offeringAmount +
                    ", piedra=" + debugStone.unlocked + "/" + debugStone.offeringAmount);
            }

            state.acolytesAvailable = 4L;
            if (!D2BondSystem.TryAssignAcolytes(gameState, 4L))
                return FailCivilization1Block5BValidation("asignacion de Acolitos incorrecta");
            D2BondSystem.Tick(state, 60.0);
            if (System.Math.Abs(state.bondProgress - 2.0) > 0.000001)
                return FailCivilization1Block5BValidation("progreso del vinculo incorrecto");

            state.bondProgress = D2BondSystem.GetProgressCost(1);
            foreach (string altarId in new[]
            {
                D2AltarSystem.IncenseAltarId,
                D2AltarSystem.SacredClothAltarId,
                D2AltarSystem.CarvedStoneAltarId
            })
            {
                D2AltarSystem.GetAltar(state, altarId).offeringAmount =
                    D2BondSystem.GetOfferingCost(1);
            }
            if (!D2BondSystem.TryUpgrade(gameState, D2BondSystem.SanctuaryEchoId) ||
                System.Math.Abs(gameState.GetDimension2SanctuaryLEMultiplier() - 1.01) > 0.000001 ||
                state.bondProgress != 0.0)
            {
                return FailCivilization1Block5BValidation(
                    "coste o conexion externa de LE incorrectos");
            }

            string serialized = JsonUtility.ToJson(state);
            D2Civilization1State restored = JsonUtility.FromJson<D2Civilization1State>(serialized);
            D2Civilization1System.EnsureState(restored);
            if (!D2Civilization1System.ValidateState(restored, out _) ||
                !D2BondSystem.IsMajorPactEstablished(restored) ||
                restored.bondLines.Count != D2BondSystem.LineIds.Length ||
                D2BondSystem.GetLevel(restored, D2BondSystem.SanctuaryEchoId) != 1)
            {
                return FailCivilization1Block5BValidation(
                    "catalogo o serializacion del pacto incorrectos");
            }

            Debug.Log("[D2 Block 5B] VALIDACION LOGICA OK: pacto mayor de Civ1, cinco lineas, Acolitos, progreso, costes, LE y serializacion.");
            return true;
        }
        finally
        {
            gameState.dimension02Unlocked = originalDimension2Unlocked;
            gameState.dimension2 = JsonUtility.FromJson<Dimension2State>(originalStateJson);
            gameState.EnsureDimension2State();
        }
    }

    private static bool FailCivilization1Block5BValidation(string reason)
    {
        Debug.LogError("[D2 Block 5B] Validacion logica fallida: " + reason + ".");
        return false;
    }

    private static bool FailCivilization1Validation(string reason)
    {
        Debug.LogError("[D2 Block 2A/2B/2C/2D/2E/2F/2G] Validación lógica fallida: " + reason + ".");
        return false;
    }
}
#endif

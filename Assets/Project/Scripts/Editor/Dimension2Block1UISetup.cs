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
            valid &= Require(panel.continueFirstEntryButton, "D2.continueFirstEntryButton");
            valid &= Require(panel.mapStatusText, "D2.mapStatusText");
            valid &= Require(panel.civilization1Button, "D2.civilization1Button");
            valid &= Require(panel.civilization2Button, "D2.civilization2Button");
            valid &= Require(panel.civilization3Button, "D2.civilization3Button");
            valid &= Require(panel.civilization1StateText, "D2.civilization1StateText");
            valid &= Require(panel.civilization2StateText, "D2.civilization2StateText");
            valid &= Require(panel.civilization3StateText, "D2.civilization3StateText");
            valid &= Require(panel.civilization1PlaceholderText, "D2.civilization1PlaceholderText");
            valid &= Require(panel.backToMapButton, "D2.backToMapButton");
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
                    valid &= Require(archiveUI.backToMapButton, "D2 Civ3 Archivo.backToMapButton");
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
                }

                D2ReprisalsPanelUI reprisalsUI = civilization2UI.reprisalsPanelUI;
                if (reprisalsUI != null)
                {
                    valid &= Require(reprisalsUI.civilization2PanelUI, "D2 Defensa.civilization2PanelUI");
                    valid &= Require(reprisalsUI.threatText, "D2 Defensa.threatText");
                    valid &= Require(reprisalsUI.threatSlider, "D2 Defensa.threatSlider");
                    valid &= Require(reprisalsUI.coverageText, "D2 Defensa.coverageText");
                    valid &= Require(reprisalsUI.coverageSlider, "D2 Defensa.coverageSlider");
                    valid &= Require(reprisalsUI.protectionText, "D2 Defensa.protectionText");
                    valid &= Require(reprisalsUI.fragmentsText, "D2 Defensa.fragmentsText");
                    valid &= Require(reprisalsUI.weakeningText, "D2 Defensa.weakeningText");
                    valid &= Require(reprisalsUI.rulesText, "D2 Defensa.rulesText");
                    valid &= Require(reprisalsUI.lastResultText, "D2 Defensa.lastResultText");
                }

                D2ResistancePanelUI resistanceUI = civilization2UI.resistancePanelUI;
                if (resistanceUI != null)
                {
                    valid &= Require(resistanceUI.upgradeDropdown, "D2 RED.upgradeDropdown");
                    valid &= Require(resistanceUI.fragmentsText, "D2 RED.fragmentsText");
                    valid &= Require(resistanceUI.upgradeText, "D2 RED.upgradeText");
                    valid &= Require(resistanceUI.upgradeButton, "D2 RED.upgradeButton");
                    valid &= Require(resistanceUI.pactDropdown, "D2 RED.pactDropdown");
                    valid &= Require(resistanceUI.pactText, "D2 RED.pactText");
                    valid &= Require(resistanceUI.pactMembersText, "D2 RED.pactMembersText");
                    valid &= Require(resistanceUI.exhaustedText, "D2 RED.exhaustedText");
                    valid &= Require(resistanceUI.penaltiesText, "D2 RED.penaltiesText");
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
                    valid &= Require(containmentUI.stabilityText, "D2 Pacto Mayor Civ2.stabilityText");
                    valid &= Require(containmentUI.majorPactLineDropdown, "D2 Pacto Mayor Civ2.majorPactLineDropdown");
                    valid &= Require(containmentUI.majorPactLineText, "D2 Pacto Mayor Civ2.majorPactLineText");
                    valid &= Require(containmentUI.majorPactLastResultText, "D2 Pacto Mayor Civ2.majorPactLastResultText");
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
                    valid &= Require(altarsUI.altarStateText, "D2 Altares.altarStateText");
                    valid &= Require(altarsUI.offeringText, "D2 Altares.offeringText");
                    valid &= Require(altarsUI.productionText, "D2 Altares.productionText");
                    valid &= Require(altarsUI.assignmentText, "D2 Altares.assignmentText");
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
        GameObject root = CreateView("D2_FirstEntry", panel.transform);
        panel.firstEntryRoot = root;

        CreateText(
            "Title",
            root.transform,
            "DIMENSIÓN DE LOS PACTOS",
            38f,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.72f),
            new Vector2(0.88f, 0.18f)
        );

        CreateText(
            "Description",
            root.transform,
            "Una aproximación revela un mundo dividido en tres territorios. " +
            "Solo el Santuario de Peregrinos responde a tu llegada.",
            23f,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.50f),
            new Vector2(0.76f, 0.22f)
        );

        panel.continueFirstEntryButton = CreateButton(
            "Btn_ContinueFirstEntry",
            root.transform,
            "ABRIR MAPA",
            new Vector2(0.5f, 0.27f),
            new Vector2(0.28f, 0.09f)
        );
    }

    private static void BuildMap(Dimension2PanelUI panel)
    {
        GameObject root = CreateView("D2_Map", panel.transform);
        panel.mapRoot = root;

        panel.mapStatusText = CreateText(
            "MapStatus",
            root.transform,
            "DIMENSIÓN DE LOS PACTOS",
            25f,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.82f),
            new Vector2(0.86f, 0.18f)
        );

        CreateTerritoryCard(
            root.transform,
            "Territory_Civ1",
            "CIVILIZACIÓN 1\nSantuario de Peregrinos",
            new Vector2(0.22f, 0.48f),
            out panel.civilization1Button,
            out panel.civilization1StateText
        );

        CreateTerritoryCard(
            root.transform,
            "Territory_Civ2",
            "CIVILIZACIÓN 2\nTerritorios Sometidos",
            new Vector2(0.50f, 0.48f),
            out panel.civilization2Button,
            out panel.civilization2StateText
        );

        CreateTerritoryCard(
            root.transform,
            "Territory_Civ3",
            "CIVILIZACIÓN 3\nRuinas Sepultadas",
            new Vector2(0.78f, 0.48f),
            out panel.civilization3Button,
            out panel.civilization3StateText
        );
    }

    private static void BuildCivilization1Placeholder(Dimension2PanelUI panel)
    {
        GameObject root = CreateView("D2_Civilization1", panel.transform);
        panel.civilization1Root = root;
        D2Civilization1PanelUI civilization1UI = Undo.AddComponent<D2Civilization1PanelUI>(root);
        panel.civilization1PanelUI = civilization1UI;

        panel.civilization1PlaceholderText = CreateText(
            "Civilization1Status", root.transform,
            "CIVILIZACIÓN 1 — SANTUARIO DE PEREGRINOS", 28f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.89f),
            new Vector2(0.82f, 0.10f)
        );
        civilization1UI.showRefugeButton = CreateButton(
            "Btn_ShowRefuge", root.transform, "REFUGIO",
            new Vector2(0.071f, 0.81f), new Vector2(0.125f, 0.065f)
        );
        civilization1UI.showAltarsButton = CreateButton(
            "Btn_ShowAltars", root.transform, "ALTARES",
            new Vector2(0.214f, 0.81f), new Vector2(0.125f, 0.065f)
        );
        civilization1UI.showPilgrimagesButton = CreateButton(
            "Btn_ShowPilgrimages", root.transform, "PEREGRINACIONES",
            new Vector2(0.357f, 0.81f), new Vector2(0.125f, 0.065f)
        );
        civilization1UI.showNovitiateButton = CreateButton(
            "Btn_ShowNovitiate", root.transform, "NOVICIADO",
            new Vector2(0.500f, 0.81f), new Vector2(0.125f, 0.065f)
        );
        civilization1UI.showRitesButton = CreateButton(
            "Btn_ShowRites", root.transform, "RITOS",
            new Vector2(0.643f, 0.81f), new Vector2(0.125f, 0.065f)
        );
        civilization1UI.showPactsButton = CreateButton(
            "Btn_ShowPacts", root.transform, "PACTOS",
            new Vector2(0.786f, 0.81f), new Vector2(0.125f, 0.065f)
        );
        civilization1UI.showVeiledThresholdButton = CreateButton(
            "Btn_ShowVeiledThreshold", root.transform, "UMBRAL",
            new Vector2(0.929f, 0.81f), new Vector2(0.125f, 0.065f)
        );

        civilization1UI.refugeSectionRoot = CreateView(
            "D2_Civ1_RefugeSection", root.transform
        );
        Transform refugeRoot = civilization1UI.refugeSectionRoot.transform;
        civilization1UI.followersText = CreateText(
            "FollowersStatus", refugeRoot, "SEGUIDORES", 21f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.69f),
            new Vector2(0.82f, 0.12f)
        );
        civilization1UI.arrivalText = CreateText(
            "FollowerArrival", refugeRoot, "Llegada: 0/s", 18f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.60f),
            new Vector2(0.60f, 0.06f)
        );
        civilization1UI.arrivalProgressSlider = CreateProgressSlider(
            "FollowerArrivalProgress", refugeRoot, new Vector2(0.5f, 0.55f),
            new Vector2(0.48f, 0.025f)
        );
        civilization1UI.refugeText = CreateText(
            "RefugeStatus", refugeRoot, "REFUGIO DE PEREGRINOS — Nivel 1", 21f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.47f),
            new Vector2(0.70f, 0.07f)
        );
        civilization1UI.assignmentText = CreateText(
            "RefugeAssignment", refugeRoot, "Asignados al Refugio: 0", 18f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.40f),
            new Vector2(0.78f, 0.07f)
        );
        civilization1UI.assignOneButton = CreateButton(
            "Btn_AssignOneFollower", refugeRoot, "ASIGNAR +1",
            new Vector2(0.18f, 0.31f), new Vector2(0.13f, 0.065f)
        );
        civilization1UI.assignTenButton = CreateButton(
            "Btn_AssignTenFollowers", refugeRoot, "ASIGNAR +10",
            new Vector2(0.34f, 0.31f), new Vector2(0.13f, 0.065f)
        );
        civilization1UI.assignAllButton = CreateButton(
            "Btn_AssignAllFollowers", refugeRoot, "ASIGNAR TODO",
            new Vector2(0.50f, 0.31f), new Vector2(0.13f, 0.065f)
        );
        civilization1UI.releaseOneButton = CreateButton(
            "Btn_ReleaseOneFollower", refugeRoot, "RETIRAR -1",
            new Vector2(0.66f, 0.31f), new Vector2(0.13f, 0.065f)
        );
        civilization1UI.releaseAllButton = CreateButton(
            "Btn_ReleaseAllFollowers", refugeRoot, "RETIRAR TODO",
            new Vector2(0.82f, 0.31f), new Vector2(0.13f, 0.065f)
        );
        civilization1UI.upgradeRefugeButton = CreateButton(
            "Btn_UpgradeRefuge", refugeRoot, "MEJORAR REFUGIO",
            new Vector2(0.5f, 0.18f), new Vector2(0.25f, 0.09f)
        );
        civilization1UI.upgradeRefugeButtonText =
            civilization1UI.upgradeRefugeButton.GetComponentInChildren<TextMeshProUGUI>(true);

        civilization1UI.altarsSectionRoot = CreateView(
            "D2_Civ1_AltarsSection", root.transform
        );
        D2AltarsPanelUI altarsUI = Undo.AddComponent<D2AltarsPanelUI>(
            civilization1UI.altarsSectionRoot
        );
        civilization1UI.altarsPanelUI = altarsUI;
        Transform altarsRoot = civilization1UI.altarsSectionRoot.transform;
        altarsUI.altarDropdown = CreateDropdown(
            "AltarSelector", altarsRoot, new Vector2(0.5f, 0.69f),
            new Vector2(0.40f, 0.075f)
        );
        altarsUI.altarStateText = CreateText(
            "AltarStatus", altarsRoot, "ALTAR", 21f, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.59f), new Vector2(0.82f, 0.07f)
        );
        altarsUI.offeringText = CreateText(
            "OfferingStatus", altarsRoot, "Ofrenda: 0", 21f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.50f),
            new Vector2(0.70f, 0.07f)
        );
        altarsUI.productionText = CreateText(
            "OfferingProduction", altarsRoot, "Producción: 0/s", 18f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.43f),
            new Vector2(0.72f, 0.06f)
        );
        altarsUI.assignmentText = CreateText(
            "AltarAssignment", altarsRoot, "Seguidores asignados: 0", 18f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.36f),
            new Vector2(0.82f, 0.06f)
        );
        altarsUI.assignOneButton = CreateButton(
            "Btn_AltarAssignOne", altarsRoot, "ASIGNAR +1",
            new Vector2(0.18f, 0.25f), new Vector2(0.13f, 0.065f)
        );
        altarsUI.assignTenButton = CreateButton(
            "Btn_AltarAssignTen", altarsRoot, "ASIGNAR +10",
            new Vector2(0.34f, 0.25f), new Vector2(0.13f, 0.065f)
        );
        altarsUI.assignAllButton = CreateButton(
            "Btn_AltarAssignAll", altarsRoot, "ASIGNAR TODO",
            new Vector2(0.50f, 0.25f), new Vector2(0.13f, 0.065f)
        );
        altarsUI.releaseOneButton = CreateButton(
            "Btn_AltarReleaseOne", altarsRoot, "RETIRAR -1",
            new Vector2(0.66f, 0.25f), new Vector2(0.13f, 0.065f)
        );
        altarsUI.releaseAllButton = CreateButton(
            "Btn_AltarReleaseAll", altarsRoot, "RETIRAR TODO",
            new Vector2(0.82f, 0.25f), new Vector2(0.13f, 0.065f)
        );
        civilization1UI.altarsSectionRoot.SetActive(false);

        civilization1UI.pilgrimagesSectionRoot = CreateView(
            "D2_Civ1_PilgrimagesSection", root.transform
        );
        D2PilgrimagesPanelUI pilgrimagesUI = Undo.AddComponent<D2PilgrimagesPanelUI>(
            civilization1UI.pilgrimagesSectionRoot
        );
        civilization1UI.pilgrimagesPanelUI = pilgrimagesUI;
        Transform pilgrimagesRoot = civilization1UI.pilgrimagesSectionRoot.transform;
        pilgrimagesUI.trustText = CreateText(
            "TrustStatus", pilgrimagesRoot, "CONFIANZA: 0/500", 22f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.70f),
            new Vector2(0.88f, 0.07f)
        );
        pilgrimagesUI.trustSlider = CreateProgressSlider(
            "TrustProgress", pilgrimagesRoot, new Vector2(0.5f, 0.64f),
            new Vector2(0.58f, 0.025f)
        );
        pilgrimagesUI.resourcesText = CreateText(
            "PilgrimageResources", pilgrimagesRoot,
            "Disponibles — Seguidores: 0 | Cera: 0 | Pan ritual: 0", 18f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.58f),
            new Vector2(0.88f, 0.06f)
        );
        pilgrimagesUI.activePilgrimageText = CreateText(
            "ActivePilgrimage", pilgrimagesRoot,
            "No hay una Peregrinación activa.", 19f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.50f),
            new Vector2(0.88f, 0.08f)
        );
        pilgrimagesUI.supportText = CreateText(
            "PilgrimageSupport", pilgrimagesRoot,
            "Apoyo adicional: 0/4 Seguidores", 16f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.54f),
            new Vector2(0.55f, 0.045f)
        );
        pilgrimagesUI.removeSupportButton = CreateButton(
            "Btn_PilgrimageSupportRemove", pilgrimagesRoot, "-1 APOYO",
            new Vector2(0.17f, 0.54f), new Vector2(0.13f, 0.045f)
        );
        pilgrimagesUI.addSupportButton = CreateButton(
            "Btn_PilgrimageSupportAdd", pilgrimagesRoot, "+1 APOYO",
            new Vector2(0.83f, 0.54f), new Vector2(0.13f, 0.045f)
        );
        pilgrimagesUI.startShortButton = CreateButton(
            "Btn_StartShortPilgrimage", pilgrimagesRoot,
            "CORTA · 1 min\n1 Seg · 2 Cera · 2 Pan\n+1 Confianza",
            new Vector2(0.24f, 0.38f), new Vector2(0.22f, 0.12f)
        );
        pilgrimagesUI.startMediumButton = CreateButton(
            "Btn_StartMediumPilgrimage", pilgrimagesRoot,
            "MEDIA · 4 min\n3 Seg · 10 Cera · 10 Pan\n+5 Confianza",
            new Vector2(0.50f, 0.38f), new Vector2(0.22f, 0.12f)
        );
        pilgrimagesUI.startLongButton = CreateButton(
            "Btn_StartLongPilgrimage", pilgrimagesRoot,
            "LARGA · 10 min\n6 Seg · 25 Cera · 25 Pan\n+12 Confianza",
            new Vector2(0.76f, 0.38f), new Vector2(0.22f, 0.12f)
        );
        pilgrimagesUI.startGuidedLongButton = CreateButton(
            "Btn_StartGuidedLongPilgrimage", pilgrimagesRoot,
            "LARGA CON ACÓLITO · 10 min\n6 Seg · 1 Acólito · 35 Cera/Pan\n+16 Confianza",
            new Vector2(0.36f, 0.25f), new Vector2(0.25f, 0.10f)
        );
        pilgrimagesUI.startSacredButton = CreateButton(
            "Btn_StartSacredPilgrimage", pilgrimagesRoot,
            "SAGRADA · 15 min\n8 Seg · 2 Acólitos · 50 Cera/Pan\n+25 Confianza",
            new Vector2(0.64f, 0.25f), new Vector2(0.25f, 0.10f)
        );
        pilgrimagesUI.lastResultText = CreateText(
            "PilgrimageLastResult", pilgrimagesRoot,
            "Las recompensas se entregan automáticamente al completar.", 17f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.15f),
            new Vector2(0.88f, 0.07f)
        );
        pilgrimagesUI.cancelButton = CreateButton(
            "Btn_CancelPilgrimage", pilgrimagesRoot, "CANCELAR PEREGRINACIÓN",
            new Vector2(0.5f, 0.08f), new Vector2(0.28f, 0.06f)
        );
        civilization1UI.pilgrimagesSectionRoot.SetActive(false);

        civilization1UI.novitiateSectionRoot = CreateView(
            "D2_Civ1_NovitiateSection", root.transform
        );
        D2NovitiatePanelUI novitiateUI = Undo.AddComponent<D2NovitiatePanelUI>(
            civilization1UI.novitiateSectionRoot
        );
        civilization1UI.novitiatePanelUI = novitiateUI;
        Transform novitiateRoot = civilization1UI.novitiateSectionRoot.transform;
        novitiateUI.acolytesText = CreateText(
            "NovitiateStatus", novitiateRoot, "NOVICIADO — Nivel 1/5", 22f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.69f),
            new Vector2(0.88f, 0.08f)
        );
        novitiateUI.resourcesText = CreateText(
            "NovitiateResources", novitiateRoot,
            "Seguidores: 0 | Cera: 0 | Pan ritual: 0", 18f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.61f),
            new Vector2(0.88f, 0.06f)
        );
        novitiateUI.batchText = CreateText(
            "NovitiateBatch", novitiateRoot, "Tanda actual", 19f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.53f),
            new Vector2(0.88f, 0.07f)
        );
        novitiateUI.activeTrainingText = CreateText(
            "NovitiateActiveTraining", novitiateRoot,
            "No hay una tanda en formación.", 19f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.45f),
            new Vector2(0.88f, 0.07f)
        );
        novitiateUI.supportText = CreateText(
            "NovitiateSupport", novitiateRoot, "Apoyo adicional: 0/4 Seguidores", 16f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.49f),
            new Vector2(0.55f, 0.045f)
        );
        novitiateUI.removeSupportButton = CreateButton(
            "Btn_NovitiateSupportRemove", novitiateRoot, "-1 APOYO",
            new Vector2(0.17f, 0.49f), new Vector2(0.13f, 0.045f)
        );
        novitiateUI.addSupportButton = CreateButton(
            "Btn_NovitiateSupportAdd", novitiateRoot, "+1 APOYO",
            new Vector2(0.83f, 0.49f), new Vector2(0.13f, 0.045f)
        );
        novitiateUI.startTrainingButton = CreateButton(
            "Btn_StartNovitiateTraining", novitiateRoot, "FORMAR TANDA",
            new Vector2(0.34f, 0.34f), new Vector2(0.28f, 0.10f)
        );
        novitiateUI.startTrainingButtonText =
            novitiateUI.startTrainingButton.GetComponentInChildren<TextMeshProUGUI>(true);
        novitiateUI.upgradeButton = CreateButton(
            "Btn_UpgradeNovitiate", novitiateRoot, "MEJORAR NOVICIADO",
            new Vector2(0.66f, 0.34f), new Vector2(0.28f, 0.10f)
        );
        novitiateUI.upgradeButtonText =
            novitiateUI.upgradeButton.GetComponentInChildren<TextMeshProUGUI>(true);
        novitiateUI.lastResultText = CreateText(
            "NovitiateLastResult", novitiateRoot,
            "Los Seguidores se convierten en Acólitos al completar la tanda.", 17f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.22f),
            new Vector2(0.88f, 0.08f)
        );
        novitiateUI.cancelTrainingButton = CreateButton(
            "Btn_CancelNovitiateTraining", novitiateRoot, "CANCELAR FORMACIÓN",
            new Vector2(0.5f, 0.12f), new Vector2(0.28f, 0.07f)
        );
        civilization1UI.novitiateSectionRoot.SetActive(false);

        civilization1UI.ritesSectionRoot = CreateView(
            "D2_Civ1_RitesSection", root.transform
        );
        D2RitesPanelUI ritesUI = Undo.AddComponent<D2RitesPanelUI>(
            civilization1UI.ritesSectionRoot
        );
        civilization1UI.ritesPanelUI = ritesUI;
        Transform ritesRoot = civilization1UI.ritesSectionRoot.transform;
        ritesUI.riteDropdown = CreateDropdown(
            "RiteSelector", ritesRoot, new Vector2(0.5f, 0.69f),
            new Vector2(0.42f, 0.075f)
        );
        ritesUI.slotsText = CreateText(
            "RiteSlots", ritesRoot, "RITOS ACTIVOS: 0/2", 20f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.61f),
            new Vector2(0.88f, 0.06f)
        );
        ritesUI.effectText = CreateText(
            "RiteEffect", ritesRoot, "Efecto del Rito", 18f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.52f),
            new Vector2(0.90f, 0.10f)
        );
        ritesUI.resourcesText = CreateText(
            "RiteResources", ritesRoot,
            "Seguidores: 0 | Acólitos: 0 | Cera: 0 | Pan: 0", 17f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.44f),
            new Vector2(0.90f, 0.06f)
        );
        ritesUI.assignmentText = CreateText(
            "RiteAssignment", ritesRoot, "Asignados al Rito", 17f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.38f),
            new Vector2(0.90f, 0.06f)
        );
        ritesUI.assignFollowerOneButton = CreateButton(
            "Btn_RiteAssignFollowerOne", ritesRoot, "SEGUIDOR +1",
            new Vector2(0.18f, 0.29f), new Vector2(0.14f, 0.06f)
        );
        ritesUI.assignFollowerTenButton = CreateButton(
            "Btn_RiteAssignFollowerTen", ritesRoot, "SEGUIDOR +10",
            new Vector2(0.34f, 0.29f), new Vector2(0.14f, 0.06f)
        );
        ritesUI.releaseFollowerOneButton = CreateButton(
            "Btn_RiteReleaseFollowerOne", ritesRoot, "SEGUIDOR −1",
            new Vector2(0.50f, 0.29f), new Vector2(0.14f, 0.06f)
        );
        ritesUI.assignAcolyteOneButton = CreateButton(
            "Btn_RiteAssignAcolyteOne", ritesRoot, "ACÓLITO +1",
            new Vector2(0.66f, 0.29f), new Vector2(0.14f, 0.06f)
        );
        ritesUI.assignAcolyteFiveButton = CreateButton(
            "Btn_RiteAssignAcolyteFive", ritesRoot, "ACÓLITO +5",
            new Vector2(0.82f, 0.29f), new Vector2(0.14f, 0.06f)
        );
        ritesUI.releaseAcolyteOneButton = CreateButton(
            "Btn_RiteReleaseAcolyteOne", ritesRoot, "ACÓLITO −1",
            new Vector2(0.30f, 0.20f), new Vector2(0.16f, 0.06f)
        );
        ritesUI.releaseAllButton = CreateButton(
            "Btn_RiteReleaseAll", ritesRoot, "RETIRAR TODO",
            new Vector2(0.50f, 0.20f), new Vector2(0.16f, 0.06f)
        );
        ritesUI.unlockThirdSlotButton = CreateButton(
            "Btn_UnlockThirdRiteSlot", ritesRoot, "DESBLOQUEAR TERCER ESPACIO",
            new Vector2(0.72f, 0.16f), new Vector2(0.34f, 0.12f)
        );
        ritesUI.unlockThirdSlotButtonText = ritesUI.unlockThirdSlotButton
            .GetComponentInChildren<TextMeshProUGUI>(true);
        civilization1UI.ritesSectionRoot.SetActive(false);

        civilization1UI.pactsSectionRoot = CreateView(
            "D2_Civ1_PactsSection", root.transform
        );
        D2CivilizationPactsPanelUI pactsUI = Undo.AddComponent<D2CivilizationPactsPanelUI>(
            civilization1UI.pactsSectionRoot
        );
        civilization1UI.pactsPanelUI = pactsUI;
        Transform pactsRoot = civilization1UI.pactsSectionRoot.transform;
        pactsUI.pactDropdown = CreateDropdown(
            "CivilizationPactSelector", pactsRoot, new Vector2(0.5f, 0.69f),
            new Vector2(0.46f, 0.075f)
        );
        pactsUI.slotsText = CreateText(
            "CivilizationPactSlots", pactsRoot, "PACTOS ACTIVOS: 0/1", 20f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.61f),
            new Vector2(0.90f, 0.06f)
        );
        pactsUI.pactStateText = CreateText(
            "CivilizationPactState", pactsRoot, "PACTO INACTIVO", 20f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.55f),
            new Vector2(0.90f, 0.06f)
        );
        pactsUI.benefitText = CreateText(
            "CivilizationPactBenefit", pactsRoot, "BENEFICIO", 18f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.49f),
            new Vector2(0.90f, 0.055f)
        );
        pactsUI.commitmentText = CreateText(
            "CivilizationPactCommitment", pactsRoot, "COMPROMISO", 18f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.43f),
            new Vector2(0.90f, 0.055f)
        );
        pactsUI.resourcesText = CreateText(
            "CivilizationPactResources", pactsRoot,
            "Confianza: 0 | Acólitos: 0 | Cera: 0 | Pan: 0", 17f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.37f),
            new Vector2(0.92f, 0.055f)
        );
        pactsUI.activateButton = CreateButton(
            "Btn_ActivateCivilizationPact", pactsRoot, "ACTIVAR PACTO",
            new Vector2(0.36f, 0.27f), new Vector2(0.25f, 0.095f)
        );
        pactsUI.activateButtonText = pactsUI.activateButton
            .GetComponentInChildren<TextMeshProUGUI>(true);
        pactsUI.cancelButton = CreateButton(
            "Btn_CancelCivilizationPact", pactsRoot, "CANCELAR PACTO",
            new Vector2(0.64f, 0.27f), new Vector2(0.25f, 0.095f)
        );
        pactsUI.lastResultText = CreateText(
            "CivilizationPactLastResult", pactsRoot,
            "Cancelar no devuelve el coste de activación.", 16f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.19f),
            new Vector2(0.90f, 0.06f)
        );
        pactsUI.unlockSecondSlotButton = CreateButton(
            "Btn_UnlockSecondCivilizationPactSlot", pactsRoot,
            "DESBLOQUEAR SEGUNDO ESPACIO",
            new Vector2(0.70f, 0.10f), new Vector2(0.40f, 0.115f)
        );
        pactsUI.unlockSecondSlotButtonText = pactsUI.unlockSecondSlotButton
            .GetComponentInChildren<TextMeshProUGUI>(true);
        civilization1UI.pactsSectionRoot.SetActive(false);

        civilization1UI.veiledThresholdSectionRoot = CreateView(
            "D2_Civ1_VeiledThresholdSection", root.transform
        );
        D2VeiledThresholdPanelUI thresholdUI = Undo.AddComponent<D2VeiledThresholdPanelUI>(
            civilization1UI.veiledThresholdSectionRoot
        );
        civilization1UI.veiledThresholdPanelUI = thresholdUI;
        Transform thresholdRoot = civilization1UI.veiledThresholdSectionRoot.transform;
        thresholdUI.titleText = CreateText(
            "VeiledThresholdTitle", thresholdRoot, "PACTO MAYOR — LUGAR DE VÍNCULO", 30f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.70f),
            new Vector2(0.84f, 0.09f)
        );
        thresholdUI.revelationText = CreateText(
            "VeiledThresholdRevelation", thresholdRoot, "ALGO RESPONDE", 25f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.61f),
            new Vector2(0.84f, 0.08f)
        );
        thresholdUI.placeText = CreateText(
            "VeiledThresholdPlace", thresholdRoot,
            "La civilización permite establecer el pacto en el Lugar de Vínculo.", 20f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.52f),
            new Vector2(0.84f, 0.10f)
        );
        thresholdUI.resourcesText = CreateText(
            "BondResources", thresholdRoot, "Recursos del vinculo", 16f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.44f),
            new Vector2(0.92f, 0.05f)
        );
        thresholdUI.acolytesText = CreateText(
            "BondAcolytes", thresholdRoot, "Acolitos del vinculo", 16f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.385f),
            new Vector2(0.82f, 0.045f)
        );
        thresholdUI.prepareButton = CreateButton(
            "Btn_PrepareBondPlace", thresholdRoot, "PREPARAR LUGAR",
            new Vector2(0.5f, 0.32f), new Vector2(0.22f, 0.055f)
        );
        thresholdUI.assignAcolyteButton = CreateButton(
            "Btn_BondAssignAcolyte", thresholdRoot, "ACOLITO +1",
            new Vector2(0.20f, 0.32f), new Vector2(0.15f, 0.05f)
        );
        thresholdUI.releaseAcolyteButton = CreateButton(
            "Btn_BondReleaseAcolyte", thresholdRoot, "ACOLITO -1",
            new Vector2(0.80f, 0.32f), new Vector2(0.15f, 0.05f)
        );
        thresholdUI.lineDropdown = CreateDropdown(
            "BondLineSelector", thresholdRoot, new Vector2(0.5f, 0.25f),
            new Vector2(0.40f, 0.055f)
        );
        thresholdUI.lineText = CreateText(
            "BondLineStatus", thresholdRoot, "Linea del vinculo", 15f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.175f),
            new Vector2(0.90f, 0.07f)
        );
        thresholdUI.upgradeLineButton = CreateButton(
            "Btn_UpgradeBondLine", thresholdRoot, "MEJORAR LÍNEA",
            new Vector2(0.5f, 0.105f), new Vector2(0.22f, 0.05f)
        );
        thresholdUI.pendingText = CreateText(
            "VeiledThresholdPending", thresholdRoot,
            "Su naturaleza, sus líneas y sus efectos posteriores permanecen ocultos.", 18f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.055f),
            new Vector2(0.84f, 0.035f)
        );
        civilization1UI.veiledThresholdSectionRoot.SetActive(false);

        panel.backToMapButton = CreateButton(
            "Btn_BackToD2Map", root.transform, "VOLVER AL MAPA",
            new Vector2(0.14f, 0.08f), new Vector2(0.20f, 0.07f)
        );
    }

    private static void BuildCivilization2(Dimension2PanelUI panel)
    {
        GameObject root = CreateView("D2_Civilization2", panel.transform);
        panel.civilization2Root = root;

        D2Civilization2PanelUI civilization2UI = Undo.AddComponent<D2Civilization2PanelUI>(root);
        panel.civilization2PanelUI = civilization2UI;
        civilization2UI.dimension2PanelUI = panel;

        CreateText(
            "Civilization2Title", root.transform,
            "CIVILIZACIÓN 2 — TERRITORIOS SOMETIDOS", 30f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.92f),
            new Vector2(0.86f, 0.08f)
        );

        civilization2UI.showRegionsButton = CreateButton(
            "Btn_Civ2ShowRegions", root.transform, "REGIONES",
            new Vector2(0.095f, 0.84f), new Vector2(0.14f, 0.06f)
        );
        civilization2UI.showOperationsButton = CreateButton(
            "Btn_Civ2ShowOperations", root.transform, "OPERACIONES",
            new Vector2(0.257f, 0.84f), new Vector2(0.14f, 0.06f)
        );
        civilization2UI.showDefenseButton = CreateButton(
            "Btn_Civ2ShowDefense", root.transform, "DEFENSA",
            new Vector2(0.419f, 0.84f), new Vector2(0.14f, 0.06f)
        );
        civilization2UI.showResistanceButton = CreateButton(
            "Btn_Civ2ShowResistance", root.transform, "RED",
            new Vector2(0.581f, 0.84f), new Vector2(0.14f, 0.06f)
        );
        civilization2UI.showAlertButton = CreateButton(
            "Btn_Civ2ShowAlert", root.transform, "ALERTA",
            new Vector2(0.743f, 0.84f), new Vector2(0.14f, 0.06f)
        );
        civilization2UI.showContainmentButton = CreateButton(
            "Btn_Civ2ShowContainment", root.transform, "CONTENCIÓN",
            new Vector2(0.905f, 0.84f), new Vector2(0.14f, 0.06f)
        );
        civilization2UI.regionDropdown = CreateDropdown(
            "Civ2RegionDropdown", root.transform,
            new Vector2(0.5f, 0.765f), new Vector2(0.28f, 0.055f)
        );

        civilization2UI.regionSectionRoot = CreateView(
            "D2_Civ2_RegionSection", root.transform
        );
        Transform regionRoot = civilization2UI.regionSectionRoot.transform;
        civilization2UI.membersText = CreateText(
            "ResistanceMembers", regionRoot,
            "MIEMBROS DE RESISTENCIA", 21f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.69f),
            new Vector2(0.82f, 0.06f)
        );
        civilization2UI.dominanceText = CreateText(
            "TotalDominance", regionRoot,
            "DOMINIO TOTAL: 100%", 20f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.625f),
            new Vector2(0.5f, 0.05f)
        );
        civilization2UI.dominanceSlider = CreateProgressSlider(
            "TotalDominanceSlider", regionRoot,
            new Vector2(0.5f, 0.585f), new Vector2(0.52f, 0.025f)
        );

        civilization2UI.region1Text = CreateText(
            "Region1", regionRoot,
            "REGIÓN 1 — DISPONIBLE\nDominio: 100% | Amenaza: 0%", 19f,
            TextAlignmentOptions.Center, new Vector2(0.25f, 0.465f),
            new Vector2(0.40f, 0.11f)
        );
        civilization2UI.region2Text = CreateText(
            "Region2", regionRoot,
            "REGIÓN 2 — BLOQUEADA\nSe abre con Dominio total en 80%", 18f,
            TextAlignmentOptions.Center, new Vector2(0.75f, 0.465f),
            new Vector2(0.40f, 0.11f)
        );
        civilization2UI.region3Text = CreateText(
            "Region3", regionRoot,
            "REGIÓN 3 — BLOQUEADA\nSe abre con Dominio total en 60%", 18f,
            TextAlignmentOptions.Center, new Vector2(0.25f, 0.335f),
            new Vector2(0.40f, 0.11f)
        );
        civilization2UI.region4Text = CreateText(
            "Region4", regionRoot,
            "REGIÓN 4 — ACTUALIZACIÓN FUTURA", 18f,
            TextAlignmentOptions.Center, new Vector2(0.75f, 0.335f),
            new Vector2(0.40f, 0.11f)
        );

        civilization2UI.assignmentText = CreateText(
            "RegionAssignment", regionRoot,
            "Asignación regional — Región 1: 0 Miembros", 20f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.245f),
            new Vector2(0.74f, 0.055f)
        );
        civilization2UI.assignOneButton = CreateButton(
            "Btn_Civ2AssignOne", regionRoot, "+1",
            new Vector2(0.23f, 0.18f), new Vector2(0.12f, 0.06f)
        );
        civilization2UI.assignTenButton = CreateButton(
            "Btn_Civ2AssignTen", regionRoot, "+10",
            new Vector2(0.365f, 0.18f), new Vector2(0.12f, 0.06f)
        );
        civilization2UI.assignAllButton = CreateButton(
            "Btn_Civ2AssignAll", regionRoot, "ASIGNAR TODO",
            new Vector2(0.50f, 0.18f), new Vector2(0.15f, 0.06f)
        );
        civilization2UI.releaseOneButton = CreateButton(
            "Btn_Civ2ReleaseOne", regionRoot, "-1",
            new Vector2(0.65f, 0.18f), new Vector2(0.12f, 0.06f)
        );
        civilization2UI.releaseAllButton = CreateButton(
            "Btn_Civ2ReleaseAll", regionRoot, "RETIRAR TODO",
            new Vector2(0.79f, 0.18f), new Vector2(0.15f, 0.06f)
        );
        civilization2UI.lastResultText = CreateText(
            "Civ2LastResult", regionRoot,
            "La Resistencia está preparada para organizar sus primeras operaciones.",
            17f, TextAlignmentOptions.Center, new Vector2(0.5f, 0.105f),
            new Vector2(0.72f, 0.065f)
        );

        civilization2UI.operationsSectionRoot = CreateView(
            "D2_Civ2_OperationsSection", root.transform
        );
        Transform operationsRoot = civilization2UI.operationsSectionRoot.transform;
        D2OperationsPanelUI operationsUI = Undo.AddComponent<D2OperationsPanelUI>(
            civilization2UI.operationsSectionRoot
        );
        civilization2UI.operationsPanelUI = operationsUI;
        operationsUI.civilization2PanelUI = civilization2UI;
        operationsUI.operationDropdown = CreateDropdown(
            "OperationDropdown", operationsRoot,
            new Vector2(0.5f, 0.69f), new Vector2(0.42f, 0.07f)
        );
        operationsUI.operationStateText = CreateText(
            "OperationState", operationsRoot,
            "RESCATE — INACTIVA", 22f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.58f),
            new Vector2(0.74f, 0.10f)
        );
        operationsUI.effectText = CreateText(
            "OperationEffect", operationsRoot,
            "Genera Miembros y modifica Dominio y Amenaza.", 18f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.46f),
            new Vector2(0.78f, 0.12f)
        );
        operationsUI.regionalMembersText = CreateText(
            "OperationRegionalMembers", operationsRoot,
            "Región 1 — Sin destinar: 0 | En operaciones: 0", 19f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.355f),
            new Vector2(0.76f, 0.055f)
        );
        operationsUI.assignmentText = CreateText(
            "OperationAssignment", operationsRoot,
            "Asignados a Rescate: 0", 20f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.29f),
            new Vector2(0.64f, 0.055f)
        );
        operationsUI.assignOneButton = CreateButton(
            "Btn_OperationAssignOne", operationsRoot, "+1",
            new Vector2(0.23f, 0.21f), new Vector2(0.12f, 0.065f)
        );
        operationsUI.assignFiveButton = CreateButton(
            "Btn_OperationAssignFive", operationsRoot, "+5",
            new Vector2(0.365f, 0.21f), new Vector2(0.12f, 0.065f)
        );
        operationsUI.assignAllButton = CreateButton(
            "Btn_OperationAssignAll", operationsRoot, "ASIGNAR TODO",
            new Vector2(0.50f, 0.21f), new Vector2(0.15f, 0.065f)
        );
        operationsUI.releaseOneButton = CreateButton(
            "Btn_OperationReleaseOne", operationsRoot, "-1",
            new Vector2(0.65f, 0.21f), new Vector2(0.12f, 0.065f)
        );
        operationsUI.releaseAllButton = CreateButton(
            "Btn_OperationReleaseAll", operationsRoot, "RETIRAR TODO",
            new Vector2(0.79f, 0.21f), new Vector2(0.15f, 0.065f)
        );

        civilization2UI.defenseSectionRoot = CreateView(
            "D2_Civ2_DefenseSection", root.transform
        );
        Transform defenseRoot = civilization2UI.defenseSectionRoot.transform;
        D2ReprisalsPanelUI reprisalsUI = Undo.AddComponent<D2ReprisalsPanelUI>(
            civilization2UI.defenseSectionRoot
        );
        civilization2UI.reprisalsPanelUI = reprisalsUI;
        reprisalsUI.civilization2PanelUI = civilization2UI;
        reprisalsUI.threatText = CreateText(
            "DefenseThreat", defenseRoot, "AMENAZA REGIONAL: 0%", 22f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.70f),
            new Vector2(0.64f, 0.06f)
        );
        reprisalsUI.threatSlider = CreateProgressSlider(
            "DefenseThreatSlider", defenseRoot,
            new Vector2(0.5f, 0.65f), new Vector2(0.54f, 0.025f)
        );
        reprisalsUI.coverageText = CreateText(
            "DefenseCoverage", defenseRoot, "COBERTURA: 0 / 60", 22f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.57f),
            new Vector2(0.64f, 0.06f)
        );
        reprisalsUI.coverageSlider = CreateProgressSlider(
            "DefenseCoverageSlider", defenseRoot,
            new Vector2(0.5f, 0.52f), new Vector2(0.54f, 0.025f)
        );
        reprisalsUI.protectionText = CreateText(
            "DefenseProtection", defenseRoot,
            "Pérdida estimada: 8%\nPreparación de Espionaje: NO PREPARADA", 19f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.42f),
            new Vector2(0.72f, 0.10f)
        );
        reprisalsUI.fragmentsText = CreateText(
            "DefenseFragments", defenseRoot,
            "FRAGMENTOS DE CONTROL: 0 | Represalias resistidas: 0", 20f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.335f),
            new Vector2(0.76f, 0.06f)
        );
        reprisalsUI.weakeningText = CreateText(
            "DefenseWeakening", defenseRoot,
            "Ninguna operación debilitada.", 19f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.27f),
            new Vector2(0.72f, 0.06f)
        );
        reprisalsUI.rulesText = CreateText(
            "DefenseRules", defenseRoot,
            "Al llegar a 100% de Amenaza ocurre una Represalia.", 17f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.19f),
            new Vector2(0.76f, 0.08f)
        );
        reprisalsUI.lastResultText = CreateText(
            "DefenseLastResult", defenseRoot,
            "Aún no se ha producido ninguna Represalia.", 17f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.12f),
            new Vector2(0.76f, 0.06f)
        );

        civilization2UI.resistanceSectionRoot = CreateView(
            "D2_Civ2_ResistanceSection", root.transform
        );
        Transform resistanceRoot = civilization2UI.resistanceSectionRoot.transform;
        D2ResistancePanelUI resistanceUI = Undo.AddComponent<D2ResistancePanelUI>(
            civilization2UI.resistanceSectionRoot
        );
        civilization2UI.resistancePanelUI = resistanceUI;
        resistanceUI.upgradeDropdown = CreateDropdown(
            "ResistanceUpgradeDropdown", resistanceRoot,
            new Vector2(0.28f, 0.70f), new Vector2(0.38f, 0.06f)
        );
        resistanceUI.fragmentsText = CreateText(
            "ResistanceFragments", resistanceRoot, "FRAGMENTOS DE CONTROL: 0", 18f,
            TextAlignmentOptions.Center, new Vector2(0.72f, 0.70f),
            new Vector2(0.36f, 0.055f)
        );
        resistanceUI.upgradeText = CreateText(
            "ResistanceUpgradeState", resistanceRoot,
            "MEJORA DE LA RED — NIVEL 0/3", 18f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.61f),
            new Vector2(0.84f, 0.105f)
        );
        resistanceUI.upgradeButton = CreateButton(
            "Btn_ResistanceUpgrade", resistanceRoot, "MEJORAR",
            new Vector2(0.5f, 0.535f), new Vector2(0.20f, 0.055f)
        );
        resistanceUI.pactDropdown = CreateDropdown(
            "ResistancePactDropdown", resistanceRoot,
            new Vector2(0.5f, 0.445f), new Vector2(0.42f, 0.06f)
        );
        resistanceUI.pactText = CreateText(
            "ResistancePactState", resistanceRoot,
            "PACTO DE RESISTENCIA — INACTIVO", 18f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.355f),
            new Vector2(0.88f, 0.11f)
        );
        resistanceUI.pactMembersText = CreateText(
            "ResistancePactMembers", resistanceRoot,
            "Comprometidos: 0 | Disponibles: 0", 17f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.285f),
            new Vector2(0.84f, 0.05f)
        );
        resistanceUI.activateButton = CreateButton(
            "Btn_ActivateResistancePact", resistanceRoot, "ACTIVAR",
            new Vector2(0.25f, 0.225f), new Vector2(0.16f, 0.055f)
        );
        resistanceUI.reinforceOneButton = CreateButton(
            "Btn_ReinforceResistancePactOne", resistanceRoot, "REFORZAR +1",
            new Vector2(0.42f, 0.225f), new Vector2(0.16f, 0.055f)
        );
        resistanceUI.reinforceTenButton = CreateButton(
            "Btn_ReinforceResistancePactTen", resistanceRoot, "REFORZAR +10",
            new Vector2(0.59f, 0.225f), new Vector2(0.16f, 0.055f)
        );
        resistanceUI.cancelButton = CreateButton(
            "Btn_CancelResistancePact", resistanceRoot, "INCUMPLIR",
            new Vector2(0.76f, 0.225f), new Vector2(0.16f, 0.055f)
        );
        resistanceUI.exhaustedText = CreateText(
            "ResistanceExhaustedMembers", resistanceRoot,
            "MIEMBROS AGOTADOS: 0", 17f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.155f),
            new Vector2(0.88f, 0.05f)
        );
        resistanceUI.penaltiesText = CreateText(
            "ResistancePenalties", resistanceRoot,
            "PENALIZACIONES — ninguna", 16f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.105f),
            new Vector2(0.90f, 0.05f)
        );
        civilization2UI.resistanceSectionRoot.SetActive(false);

        civilization2UI.alertSectionRoot = CreateView(
            "D2_Civ2_AlertSection", root.transform
        );
        Transform alertRoot = civilization2UI.alertSectionRoot.transform;
        D2AlertPanelUI alertUI = Undo.AddComponent<D2AlertPanelUI>(
            civilization2UI.alertSectionRoot
        );
        civilization2UI.alertPanelUI = alertUI;
        alertUI.stateText = CreateText(
            "AlertState", alertRoot, "FASE DE ALERTA — INACTIVA", 27f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.70f),
            new Vector2(0.82f, 0.07f)
        );
        alertUI.dominanceText = CreateText(
            "AlertDominance", alertRoot,
            "Dominio total: 100% | Se activa permanentemente al llegar a 30%.", 19f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.625f),
            new Vector2(0.86f, 0.055f)
        );
        alertUI.timerText = CreateText(
            "AlertTimer", alertRoot, "El Ente todavía no está marcando regiones.", 19f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.555f),
            new Vector2(0.86f, 0.055f)
        );
        alertUI.effectsText = CreateText(
            "AlertEffects", alertRoot,
            "Antes de Alerta se mantienen la Amenaza normal y 3 Fragmentos por Represalia.", 18f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.485f),
            new Vector2(0.88f, 0.07f)
        );
        alertUI.regionsText = CreateText(
            "AlertRegions", alertRoot, "ESTADO DE LAS REGIONES", 18f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.355f),
            new Vector2(0.86f, 0.17f)
        );
        alertUI.unlocksText = CreateText(
            "AlertUnlocks", alertRoot,
            "Civilización 3 y Contención permanecen bloqueadas.", 19f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.235f),
            new Vector2(0.86f, 0.06f)
        );
        alertUI.lastResultText = CreateText(
            "AlertLastResult", alertRoot, "No se han producido ataques de Alerta.", 17f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.155f),
            new Vector2(0.84f, 0.075f)
        );
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
        containmentUI.majorPactStateText = CreateText(
            "MajorPactState", majorPactRoot,
            "ENTE CONTENIDO — PACTO MAYOR PREPARADO", 27f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.70f),
            new Vector2(0.86f, 0.07f)
        );
        containmentUI.stabilityText = CreateText(
            "MajorPactStability", majorPactRoot,
            "ESTABILIDAD DE CONTENCIÓN: 0 | Fragmentos de Control: 0", 19f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.625f),
            new Vector2(0.86f, 0.055f)
        );
        containmentUI.establishMajorPactButton = CreateButton(
            "Btn_EstablishCiv2MajorPact", majorPactRoot, "ESTABLECER PACTO",
            new Vector2(0.5f, 0.555f), new Vector2(0.25f, 0.055f)
        );
        containmentUI.majorPactLineDropdown = CreateDropdown(
            "Civ2MajorPactLineSelector", majorPactRoot,
            new Vector2(0.5f, 0.49f), new Vector2(0.42f, 0.05f)
        );
        containmentUI.majorPactLineText = CreateText(
            "Civ2MajorPactLine", majorPactRoot, "LÍNEA DEL PACTO", 16f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.425f),
            new Vector2(0.90f, 0.075f)
        );
        containmentUI.upgradeMajorPactLineButton = CreateButton(
            "Btn_UpgradeCiv2MajorPactLine", majorPactRoot, "MEJORAR LÍNEA",
            new Vector2(0.5f, 0.355f), new Vector2(0.23f, 0.045f)
        );
        containmentUI.majorPactLastResultText = CreateText(
            "Civ2MajorPactLastResult", majorPactRoot,
            "Establece el pacto y asigna Miembros para generar Estabilidad.", 17f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.145f),
            new Vector2(0.86f, 0.06f)
        );
        containmentUI.majorPactRoot.SetActive(false);
        containmentUI.stateText = CreateText(
            "ContainmentState", containmentAttemptRoot, "CONTENCIÓN BLOQUEADA", 27f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.70f),
            new Vector2(0.84f, 0.07f)
        );
        containmentUI.probabilityText = CreateText(
            "ContainmentProbability", containmentAttemptRoot,
            "Dominio total: 100% | Probabilidad: 20%", 20f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.625f),
            new Vector2(0.82f, 0.055f)
        );
        containmentUI.cooldownText = CreateText(
            "ContainmentCooldown", containmentAttemptRoot, "Sin cooldown.", 19f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.565f),
            new Vector2(0.72f, 0.05f)
        );
        containmentUI.attemptButton = CreateButton(
            "Btn_AttemptContainment", containmentAttemptRoot, "INTENTAR CONTENCIÓN",
            new Vector2(0.5f, 0.495f), new Vector2(0.28f, 0.065f)
        );
        containmentUI.rulesText = CreateText(
            "ContainmentRules", containmentAttemptRoot,
            "Fallo: +20% Amenaza y -5% de Miembros regionales sin Protección.", 17f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.42f),
            new Vector2(0.88f, 0.07f)
        );
        containmentUI.assignmentText = CreateText(
            "ContainmentAssignment", majorPactRoot,
            "SOSTENIMIENTO — Asignados: 0 | Disponibles: 0", 19f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.295f),
            new Vector2(0.82f, 0.055f)
        );
        containmentUI.assignOneButton = CreateButton(
            "Btn_ContainmentAssignOne", majorPactRoot, "+1",
            new Vector2(0.23f, 0.225f), new Vector2(0.12f, 0.055f)
        );
        containmentUI.assignTenButton = CreateButton(
            "Btn_ContainmentAssignTen", majorPactRoot, "+10",
            new Vector2(0.365f, 0.225f), new Vector2(0.12f, 0.055f)
        );
        containmentUI.assignAllButton = CreateButton(
            "Btn_ContainmentAssignAll", majorPactRoot, "ASIGNAR TODO",
            new Vector2(0.50f, 0.225f), new Vector2(0.15f, 0.055f)
        );
        containmentUI.releaseOneButton = CreateButton(
            "Btn_ContainmentReleaseOne", majorPactRoot, "-1",
            new Vector2(0.65f, 0.225f), new Vector2(0.12f, 0.055f)
        );
        containmentUI.releaseAllButton = CreateButton(
            "Btn_ContainmentReleaseAll", majorPactRoot, "RETIRAR TODO",
            new Vector2(0.79f, 0.225f), new Vector2(0.15f, 0.055f)
        );
        containmentUI.attemptsText = CreateText(
            "ContainmentAttempts", containmentAttemptRoot, "Intentos: 0 | Fallos: 0", 17f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.205f),
            new Vector2(0.72f, 0.045f)
        );
        containmentUI.lastResultText = CreateText(
            "ContainmentLastResult", containmentAttemptRoot,
            "Todavía no se ha intentado la Contención.", 17f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.145f),
            new Vector2(0.86f, 0.06f)
        );
        civilization2UI.containmentSectionRoot.SetActive(false);

        civilization2UI.backToMapButton = CreateButton(
            "Btn_Civ2BackToMap", root.transform, "VOLVER AL MAPA",
            new Vector2(0.14f, 0.07f), new Vector2(0.22f, 0.07f)
        );

        EditorUtility.SetDirty(civilization2UI);
        EditorUtility.SetDirty(operationsUI);
        EditorUtility.SetDirty(reprisalsUI);
        EditorUtility.SetDirty(resistanceUI);
        EditorUtility.SetDirty(alertUI);
        EditorUtility.SetDirty(containmentUI);
    }

    private static void BuildCivilization3(Dimension2PanelUI panel)
    {
        GameObject root = CreateView("D2_Civilization3", panel.transform);
        panel.civilization3Root = root;
        D2Civilization3PanelUI civilization3UI = Undo.AddComponent<D2Civilization3PanelUI>(root);
        panel.civilization3PanelUI = civilization3UI;
        civilization3UI.dimension2PanelUI = panel;

        CreateText(
            "Civilization3Title", root.transform,
            "CIVILIZACIÓN 3 — RUINAS SEPULTADAS", 30f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.955f),
            new Vector2(0.86f, 0.045f)
        );
        civilization3UI.zone1Button = CreateButton(
            "Btn_Civ3Zone1", root.transform, "ZONA 1",
            new Vector2(0.20f, 0.905f), new Vector2(0.15f, 0.045f)
        );
        civilization3UI.zone2Button = CreateButton(
            "Btn_Civ3Zone2", root.transform, "ZONA 2",
            new Vector2(0.38f, 0.905f), new Vector2(0.15f, 0.045f)
        );
        civilization3UI.zone3Button = CreateButton(
            "Btn_Civ3Zone3", root.transform, "ZONA 3",
            new Vector2(0.56f, 0.905f), new Vector2(0.15f, 0.045f)
        );
        civilization3UI.zoneText = CreateText(
            "Civ3Zone1", root.transform,
            "ZONA 1 — ENTRADA SEPULTADA\nRecurso propio: Fragmentos Base", 23f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.845f),
            new Vector2(0.82f, 0.055f)
        );
        civilization3UI.lockedZonesText = CreateText(
            "Civ3LockedZones", root.transform,
            "ZONA 2 — GALERÍA DE INSCRIPCIONES: BLOQUEADA\n" +
            "ZONA 3 — SANTUARIO SELLADO: BLOQUEADA", 17f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.79f),
            new Vector2(0.9f, 0.05f)
        );
        civilization3UI.unlockZone2Button = CreateButton(
            "Btn_Civ3UnlockZone2", root.transform, "DESTAPAR ZONA 2",
            new Vector2(0.5f, 0.74f), new Vector2(0.24f, 0.045f)
        );
        civilization3UI.unlockZone3Button = CreateButton(
            "Btn_Civ3UnlockZone3", root.transform, "DESTAPAR ZONA 3",
            new Vector2(0.5f, 0.74f), new Vector2(0.24f, 0.045f)
        );
        civilization3UI.excavationText = CreateText(
            "Civ3ExcavationState", root.transform,
            "EXCAVACIÓN DISPONIBLE — duración base 00:30", 21f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.695f),
            new Vector2(0.76f, 0.045f)
        );
        civilization3UI.excavationSlider = CreateProgressSlider(
            "Civ3ExcavationProgress", root.transform,
            new Vector2(0.5f, 0.67f), new Vector2(0.52f, 0.018f)
        );
        civilization3UI.excavateButton = CreateButton(
            "Btn_Civ3Excavate", root.transform, "EXCAVAR",
            new Vector2(0.5f, 0.63f), new Vector2(0.20f, 0.045f)
        );
        civilization3UI.inventoryText = CreateText(
            "Civ3RemainsInventory", root.transform,
            "RESTOS ARQUEOLÓGICOS — Baja: 0 | Media: 0 | Alta: 0 | Total: 0", 19f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.585f),
            new Vector2(0.88f, 0.045f)
        );
        civilization3UI.analysisText = CreateText(
            "Civ3AnalysisState", root.transform,
            "ANÁLISIS BLOQUEADO — requiere Erudito de Campo", 19f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.54f),
            new Vector2(0.84f, 0.04f)
        );
        civilization3UI.analysisSlider = CreateProgressSlider(
            "Civ3AnalysisProgress", root.transform,
            new Vector2(0.5f, 0.515f), new Vector2(0.52f, 0.018f)
        );
        civilization3UI.analyzeLowButton = CreateButton(
            "Btn_Civ3AnalyzeLow", root.transform, "ANALIZAR BAJA",
            new Vector2(0.28f, 0.475f), new Vector2(0.19f, 0.045f)
        );
        civilization3UI.analyzeMediumButton = CreateButton(
            "Btn_Civ3AnalyzeMedium", root.transform, "ANALIZAR MEDIA",
            new Vector2(0.5f, 0.475f), new Vector2(0.19f, 0.045f)
        );
        civilization3UI.analyzeHighButton = CreateButton(
            "Btn_Civ3AnalyzeHigh", root.transform, "ANALIZAR ALTA",
            new Vector2(0.72f, 0.475f), new Vector2(0.19f, 0.045f)
        );
        civilization3UI.researchText = CreateText(
            "Civ3Research", root.transform,
            "INVESTIGACIÓN DE ZONA 1: 0% | Conocimiento Antiguo: 0 | Fragmentos Base: 0", 17f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.425f),
            new Vector2(0.9f, 0.045f)
        );
        civilization3UI.archiveText = CreateText(
            "Civ3Archive", root.transform,
            "ARCHIVO DE INTERPRETACIÓN — se desbloquea tras el primer análisis", 16f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.37f),
            new Vector2(0.9f, 0.06f)
        );
        civilization3UI.cluesText = CreateText(
            "Civ3AnomalyClues", root.transform,
            "INDICIOS ANÓMALOS — se habilitan con Zona 2 al 20%", 16f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.32f),
            new Vector2(0.88f, 0.04f)
        );
        civilization3UI.anomalyText = CreateText(
            "Civ3Anomaly", root.transform,
            "ANOMALÍA — SIN REVELAR", 16f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.275f),
            new Vector2(0.88f, 0.05f)
        );
        civilization3UI.readAnomalyButton = CreateButton(
            "Btn_Civ3ReadAnomaly", root.transform, "LEER ANOMALÍA",
            new Vector2(0.5f, 0.225f), new Vector2(0.22f, 0.045f)
        );
        civilization3UI.scholarText = CreateText(
            "Civ3FieldScholar", root.transform,
            "ERUDITO DE CAMPO — NO CONTRATADO\nCoste: 10 Cera + 10 Pan ritual.", 20f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.17f),
            new Vector2(0.78f, 0.055f)
        );
        civilization3UI.civilization1ResourcesText = CreateText(
            "Civ3Civilization1Resources", root.transform,
            "RECURSOS DE CIVILIZACIÓN 1 — Cera: 0 | Pan ritual: 0", 17f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.12f),
            new Vector2(0.82f, 0.04f)
        );
        civilization3UI.backToMapButton = CreateButton(
            "Btn_Civ3BackToMap", root.transform, "VOLVER AL MAPA",
            new Vector2(0.14f, 0.055f), new Vector2(0.22f, 0.05f)
        );
        civilization3UI.lastResultText = CreateText(
            "Civ3LastResult", root.transform,
            "Las ruinas aguardan la primera excavación.", 16f,
            TextAlignmentOptions.Center, new Vector2(0.45f, 0.055f),
            new Vector2(0.36f, 0.05f)
        );
        civilization3UI.hireScholarButton = CreateButton(
            "Btn_Civ3HireFieldScholar", root.transform, "CONTRATAR ERUDITO",
            new Vector2(0.77f, 0.055f), new Vector2(0.24f, 0.05f)
        );

        int archaeologyChildCount = root.transform.childCount;
        civilization3UI.archaeologySectionRoot = CreateView(
            "D2_Civ3_ArchaeologySection", root.transform
        );
        for (int i = archaeologyChildCount - 1; i >= 0; i--)
        {
            root.transform.GetChild(i).SetParent(
                civilization3UI.archaeologySectionRoot.transform,
                false
            );
        }
        civilization3UI.showEntityResearchButton = CreateButton(
            "Btn_Civ3ShowEntityResearch",
            civilization3UI.archaeologySectionRoot.transform,
            "ENTE",
            new Vector2(0.91f, 0.905f), new Vector2(0.12f, 0.045f)
        );
        civilization3UI.showArchiveButton = CreateButton(
            "Btn_Civ3ShowArchive",
            civilization3UI.archaeologySectionRoot.transform,
            "ARCHIVO",
            new Vector2(0.75f, 0.905f), new Vector2(0.14f, 0.045f)
        );

        civilization3UI.archiveSectionRoot = CreateView(
            "D2_Civ3_ArchiveSection", root.transform
        );
        D2ArchivePanelUI archiveUI = Undo.AddComponent<D2ArchivePanelUI>(
            civilization3UI.archiveSectionRoot
        );
        civilization3UI.archivePanelUI = archiveUI;
        archiveUI.civilization3PanelUI = civilization3UI;
        Transform archiveRoot = civilization3UI.archiveSectionRoot.transform;
        CreateText(
            "Civ3ArchiveTitle", archiveRoot, "ARCHIVO DE INTERPRETACIÓN", 30f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.91f),
            new Vector2(0.84f, 0.07f)
        );
        archiveUI.stateText = CreateText(
            "Civ3ArchiveState", archiveRoot, "ARCHIVO I — MEJORAS PERMANENTES", 22f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.82f),
            new Vector2(0.82f, 0.06f)
        );
        archiveUI.resourcesText = CreateText(
            "Civ3ArchiveResources", archiveRoot, "RECURSOS DEL ARCHIVO", 18f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.74f),
            new Vector2(0.88f, 0.075f)
        );
        archiveUI.cartographyText = CreateText(
            "Civ3ArchiveCartography", archiveRoot, "CARTOGRAFÍA ESTRATIFICADA", 18f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.61f),
            new Vector2(0.86f, 0.10f)
        );
        archiveUI.cartographyButton = CreateButton(
            "Btn_Civ3ArchiveCartography", archiveRoot, "DESBLOQUEAR",
            new Vector2(0.5f, 0.535f), new Vector2(0.22f, 0.05f)
        );
        archiveUI.concordanceText = CreateText(
            "Civ3ArchiveConcordance", archiveRoot, "CONCORDANCIA ANÓMALA", 18f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.42f),
            new Vector2(0.86f, 0.10f)
        );
        archiveUI.concordanceButton = CreateButton(
            "Btn_Civ3ArchiveConcordance", archiveRoot, "DESBLOQUEAR",
            new Vector2(0.5f, 0.345f), new Vector2(0.22f, 0.05f)
        );
        archiveUI.exegesisText = CreateText(
            "Civ3ArchiveExegesis", archiveRoot, "EXÉGESIS PROFUNDA", 18f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.23f),
            new Vector2(0.86f, 0.10f)
        );
        archiveUI.exegesisButton = CreateButton(
            "Btn_Civ3ArchiveExegesis", archiveRoot, "DESBLOQUEAR",
            new Vector2(0.5f, 0.155f), new Vector2(0.22f, 0.05f)
        );
        archiveUI.lastResultText = CreateText(
            "Civ3ArchiveLastResult", archiveRoot, "El Archivo aguarda nuevos hallazgos.", 16f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.105f),
            new Vector2(0.55f, 0.045f)
        );
        archiveUI.backToArchaeologyButton = CreateButton(
            "Btn_Civ3ArchiveBackToArchaeology", archiveRoot, "VOLVER A ARQUEOLOGÍA",
            new Vector2(0.18f, 0.055f), new Vector2(0.26f, 0.05f)
        );
        archiveUI.backToMapButton = CreateButton(
            "Btn_Civ3ArchiveBackToMap", archiveRoot, "VOLVER AL MAPA",
            new Vector2(0.82f, 0.055f), new Vector2(0.22f, 0.05f)
        );
        civilization3UI.archiveSectionRoot.SetActive(false);

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

        EditorUtility.SetDirty(civilization3UI);
        EditorUtility.SetDirty(archiveUI);
        EditorUtility.SetDirty(entityResearchUI);
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
        panel.closeDimension2Button = CreateButton(
            "D2_Btn_Close",
            panel.transform,
            "CERRAR",
            new Vector2(0.89f, 0.08f),
            new Vector2(0.16f, 0.07f)
        );
        panel.closeDimension2Button.transform.SetAsLastSibling();
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

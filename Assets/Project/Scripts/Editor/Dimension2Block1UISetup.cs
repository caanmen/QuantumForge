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
            }
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
            "VeiledThresholdTitle", thresholdRoot, "UMBRAL VELADO", 30f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.63f),
            new Vector2(0.84f, 0.09f)
        );
        thresholdUI.revelationText = CreateText(
            "VeiledThresholdRevelation", thresholdRoot, "ALGO RESPONDE", 25f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.52f),
            new Vector2(0.84f, 0.08f)
        );
        thresholdUI.placeText = CreateText(
            "VeiledThresholdPlace", thresholdRoot,
            "La civilización ha permitido el acceso al Lugar de Vínculo.", 20f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.40f),
            new Vector2(0.84f, 0.10f)
        );
        thresholdUI.resourcesText = CreateText(
            "BondResources", thresholdRoot, "Recursos del vinculo", 16f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.34f),
            new Vector2(0.92f, 0.05f)
        );
        thresholdUI.acolytesText = CreateText(
            "BondAcolytes", thresholdRoot, "Acolitos del vinculo", 16f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.29f),
            new Vector2(0.82f, 0.045f)
        );
        thresholdUI.prepareButton = CreateButton(
            "Btn_PrepareBondPlace", thresholdRoot, "PREPARAR LUGAR",
            new Vector2(0.5f, 0.23f), new Vector2(0.22f, 0.055f)
        );
        thresholdUI.assignAcolyteButton = CreateButton(
            "Btn_BondAssignAcolyte", thresholdRoot, "ACOLITO +1",
            new Vector2(0.20f, 0.23f), new Vector2(0.15f, 0.05f)
        );
        thresholdUI.releaseAcolyteButton = CreateButton(
            "Btn_BondReleaseAcolyte", thresholdRoot, "ACOLITO -1",
            new Vector2(0.80f, 0.23f), new Vector2(0.15f, 0.05f)
        );
        thresholdUI.lineDropdown = CreateDropdown(
            "BondLineSelector", thresholdRoot, new Vector2(0.5f, 0.17f),
            new Vector2(0.40f, 0.055f)
        );
        thresholdUI.lineText = CreateText(
            "BondLineStatus", thresholdRoot, "Linea del vinculo", 15f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.105f),
            new Vector2(0.90f, 0.07f)
        );
        thresholdUI.upgradeLineButton = CreateButton(
            "Btn_UpgradeBondLine", thresholdRoot, "MEJORAR LINEA",
            new Vector2(0.5f, 0.045f), new Vector2(0.22f, 0.05f)
        );
        thresholdUI.pendingText = CreateText(
            "VeiledThresholdPending", thresholdRoot,
            "Su naturaleza, sus líneas y sus efectos posteriores permanecen ocultos.", 18f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.005f),
            new Vector2(0.84f, 0.035f)
        );
        civilization1UI.veiledThresholdSectionRoot.SetActive(false);

        panel.backToMapButton = CreateButton(
            "Btn_BackToD2Map", root.transform, "VOLVER AL MAPA",
            new Vector2(0.14f, 0.08f), new Vector2(0.20f, 0.07f)
        );
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

    private static bool FailCivilization1Validation(string reason)
    {
        Debug.LogError("[D2 Block 2A/2B/2C/2D/2E/2F/2G] Validación lógica fallida: " + reason + ".");
        return false;
    }
}
#endif

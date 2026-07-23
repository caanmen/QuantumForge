#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public static class Dimension3Block1UISetup
{
    private const string MainScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ConfigureMenu =
        "Tools/Quantum Forge/Dimension 3/Configure Block 1 UI";
    private const string ValidateMenu =
        "Tools/Quantum Forge/Dimension 3/Validate Block 1 UI";

    [MenuItem(ConfigureMenu)]
    public static void ConfigureBlock1UI()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.path != MainScenePath)
            activeScene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);

        TabsUI tabs = Object.FindFirstObjectByType<TabsUI>(FindObjectsInactive.Include);
        if (tabs == null)
        {
            Debug.LogError("[D3 Block 1 UI] No se encontró TabsUI.");
            return;
        }

        if (tabs.btnDimension2 == null || tabs.dimension2Panel == null)
        {
            Debug.LogError(
                "[D3 Block 1 UI] Se requieren las referencias existentes de Dimensión 2 " +
                "para ubicar la nueva pestaña sin alterar otros paneles."
            );
            return;
        }

        Undo.SetCurrentGroupName("Configure Dimension 3 Block 1 UI");
        int undoGroup = Undo.GetCurrentGroup();

        Button tabButton = GetOrCreateTabButton(tabs);
        Dimension3PanelUI panel = GetOrCreatePanel(tabs);
        if (panel.GetComponent<D3RuntimeLocalizer>() == null)
            Undo.AddComponent<D3RuntimeLocalizer>(panel.gameObject);
        HideObsoleteNavigationButtons(tabs);

        tabs.btnDimension3 = tabButton;
        tabs.dimension3Panel = panel.gameObject;
        EditorUtility.SetDirty(tabs);
        EditorUtility.SetDirty(panel);

        panel.gameObject.SetActive(false);
        EditorSceneManager.MarkSceneDirty(activeScene);
        Undo.CollapseUndoOperations(undoGroup);

        if (!EditorSceneManager.SaveScene(activeScene))
        {
            Debug.LogError("[D3 Block 1 UI] No se pudo guardar Main.unity.");
            return;
        }

        ValidateBlock1UI();
        Selection.activeGameObject = panel.gameObject;
        Debug.Log("[D3 Block 1 UI] Pestaña, panel y referencias creados.");
    }

    private static void HideObsoleteNavigationButtons(TabsUI tabs)
    {
        if (tabs.btnPrestigio != null)
        {
            // Se conserva visible en la escena base: TabsUI lo oculta al completar Prestigio 1.
            tabs.btnPrestigio.gameObject.SetActive(true);
            EditorUtility.SetDirty(tabs.btnPrestigio.gameObject);
        }

        Transform navigationParent = tabs.btnGeneracion != null
            ? tabs.btnGeneracion.transform.parent
            : null;
        if (navigationParent == null) return;

        Button[] buttons = navigationParent.GetComponentsInChildren<Button>(true);
        for (int i = 0; i < buttons.Length; i++)
        {
            Button button = buttons[i];
            if (button == null) continue;
            TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>(true);
            if (label == null) continue;
            string text = label.text == null ? "" : label.text.ToLowerInvariant();
            if (!text.Contains("meta")) continue;
            button.gameObject.SetActive(false);
            EditorUtility.SetDirty(button.gameObject);
        }

        MetaPrestigeUI metaPrestige = Object.FindFirstObjectByType<MetaPrestigeUI>(
            FindObjectsInactive.Include);
        if (metaPrestige != null && metaPrestige.metaPrestigeButton != null)
        {
            metaPrestige.metaPrestigeButton.gameObject.SetActive(false);
            EditorUtility.SetDirty(metaPrestige.metaPrestigeButton.gameObject);
        }
    }

    [MenuItem(ValidateMenu)]
    public static void ValidateBlock1UI()
    {
        bool valid = true;
        TabsUI tabs = Object.FindFirstObjectByType<TabsUI>(FindObjectsInactive.Include);
        Dimension3PanelUI panel = Object.FindFirstObjectByType<Dimension3PanelUI>(
            FindObjectsInactive.Include
        );

        bool uiNotConfiguredYet =
            tabs != null &&
            tabs.btnDimension3 == null &&
            tabs.dimension3Panel == null &&
            panel == null;

        if (uiNotConfiguredYet)
        {
            Debug.LogWarning(
                "[D3 Block 1 UI] La interfaz todavía no está configurada. " +
                "Ejecuta Tools > Quantum Forge > Dimension 3 > Configure Block 1 UI " +
                "antes de validarla."
            );
            return;
        }

        if (tabs == null)
        {
            Debug.LogError("[D3 Block 1 UI] Falta TabsUI.");
            valid = false;
        }
        else
        {
            valid &= Require(tabs.btnDimension3, "TabsUI.btnDimension3");
            valid &= Require(tabs.dimension3Panel, "TabsUI.dimension3Panel");
        }

        if (panel == null)
        {
            Debug.LogError("[D3 Block 1 UI] Falta Dimension3PanelUI.");
            valid = false;
        }
        else
        {
            valid &= Require(panel.firstEntryRoot, "D3.firstEntryRoot");
            valid &= Require(panel.factoryRoot, "D3.factoryRoot");
            valid &= Require(panel.continueFirstEntryButton, "D3.continueFirstEntryButton");
            valid &= Require(panel.closeDimension3Button, "D3.closeDimension3Button");
            valid &= Require(panel.factoryStatusText, "D3.factoryStatusText");
            valid &= Require(panel.inventoryText, "D3.inventoryText");
            valid &= Require(panel.queueText, "D3.queueText");
            valid &= Require(panel.noticeText, "D3.noticeText");
            valid &= Require(panel.assignmentText, "D3.assignmentText");
            valid &= Require(panel.powerText, "D3.powerText");
            valid &= Require(panel.costPreviewText, "D3.costPreviewText");
            valid &= Require(panel.produceChassisButton, "D3.produceChassisButton");
            valid &= Require(panel.produceMotorButton, "D3.produceMotorButton");
            valid &= Require(panel.produceToolButton, "D3.produceToolButton");
            valid &= Require(panel.produceControlButton, "D3.produceControlButton");
            valid &= Require(panel.produceRegulatorButton, "D3.produceRegulatorButton");
            valid &= Require(panel.productionVersionDropdown, "D3.productionVersionDropdown");
            valid &= Require(panel.productionQuantityDropdown, "D3.productionQuantityDropdown");
            valid &= Require(panel.assembleMk1Button, "D3.assembleMk1Button");
            valid &= Require(panel.assemblyMkDropdown, "D3.assemblyMkDropdown");
            valid &= Require(panel.assemblyQuantityDropdown, "D3.assemblyQuantityDropdown");
            valid &= Require(panel.cancelPartJobButton, "D3.cancelPartJobButton");
            valid &= Require(panel.cancelAssemblyJobButton, "D3.cancelAssemblyJobButton");
            valid &= Require(panel.assignmentMkDropdown, "D3.assignmentMkDropdown");
            valid &= Require(panel.assignmentTraitDropdown, "D3.assignmentTraitDropdown");
            valid &= Require(panel.assignmentChannelDropdown, "D3.assignmentChannelDropdown");
            valid &= Require(panel.addAssignmentButton, "D3.addAssignmentButton");
            valid &= Require(panel.removeAssignmentButton, "D3.removeAssignmentButton");
            valid &= Require(panel.upgradeProcessBankButton, "D3.upgradeProcessBankButton");
            valid &= Require(panel.openCalibrationButton, "D3.openCalibrationButton");
            valid &= Require(panel.calibrationPanel, "D3.calibrationPanel");
            valid &= Require(panel.openResearchButton, "D3.openResearchButton");
            valid &= Require(panel.researchPanel, "D3.researchPanel");
            valid &= Require(panel.openFacilitiesButton, "D3.openFacilitiesButton");
            valid &= Require(panel.facilitiesPanel, "D3.facilitiesPanel");
            valid &= Require(panel.automationPanel, "D3.automationPanel");
            if (panel.facilitiesPanel != null)
            {
                valid &= Require(panel.facilitiesPanel.openConsoleButton,
                    "D3.Facilities.openConsoleButton");
                valid &= Require(panel.facilitiesPanel.consolePanel,
                    "D3.Facilities.consolePanel");
                valid &= Require(panel.facilitiesPanel.openDiagnosticButton,
                    "D3.Facilities.openDiagnosticButton");
                valid &= Require(panel.facilitiesPanel.diagnosticPanel,
                    "D3.Facilities.diagnosticPanel");
            }
            valid &= Require(panel.openQueuesButton, "D3.openQueuesButton");
            valid &= Require(panel.queuesPanel, "D3.queuesPanel");
            if (panel.queuesPanel != null)
            {
                valid &= Require(panel.queuesPanel.factoryRoot, "D3.Queues.factoryRoot");
                valid &= Require(panel.queuesPanel.queueDropdown, "D3.Queues.queueDropdown");
                valid &= Require(panel.queuesPanel.jobDropdown, "D3.Queues.jobDropdown");
                valid &= Require(panel.queuesPanel.cancelSelectedButton,
                    "D3.Queues.cancelSelectedButton");
            }
            if (panel.facilitiesPanel != null)
            {
                valid &= Require(panel.facilitiesPanel.factoryRoot,
                    "D3.Facilities.factoryRoot");
                valid &= Require(panel.facilitiesPanel.openAutomationButton,
                    "D3.Facilities.openAutomationButton");
                valid &= Require(panel.facilitiesPanel.integrateAutonomyCoreButton,
                    "D3.Facilities.integrateAutonomyCoreButton");
                valid &= Require(panel.facilitiesPanel.automationPanel,
                    "D3.Facilities.automationPanel");
            }
            if (panel.automationPanel != null)
            {
                valid &= Require(panel.automationPanel.facilitiesPanel,
                    "D3.Automation.facilitiesPanel");
                valid &= Require(panel.automationPanel.actionDropdown,
                    "D3.Automation.actionDropdown");
                valid &= Require(panel.automationPanel.routineDropdown,
                    "D3.Automation.routineDropdown");
                valid &= Require(panel.automationPanel.createButton,
                    "D3.Automation.createButton");
                valid &= Require(panel.automationPanel.toggleButton,
                    "D3.Automation.toggleButton");
                valid &= Require(panel.automationPanel.deleteButton,
                    "D3.Automation.deleteButton");
            }
            if (panel.calibrationPanel != null)
            {
                D3CalibrationPanelUI calibration = panel.calibrationPanel;
                valid &= Require(calibration.partDropdown, "D3.Calibration.partDropdown");
                valid &= Require(calibration.mkDropdown, "D3.Calibration.mkDropdown");
                valid &= Require(calibration.quantityDropdown, "D3.Calibration.quantityDropdown");
                valid &= Require(calibration.instructionsText, "D3.Calibration.instructionsText");
                valid &= Require(calibration.valueASlider, "D3.Calibration.valueASlider");
                valid &= Require(calibration.valueBSlider, "D3.Calibration.valueBSlider");
                valid &= Require(calibration.valueCSlider, "D3.Calibration.valueCSlider");
                valid &= Require(calibration.optionADropdown, "D3.Calibration.optionADropdown");
                valid &= Require(calibration.optionBDropdown, "D3.Calibration.optionBDropdown");
                valid &= Require(calibration.optionCDropdown, "D3.Calibration.optionCDropdown");
                valid &= Require(calibration.recordPartButton, "D3.Calibration.recordPartButton");
                valid &= Require(calibration.saveProfileButton, "D3.Calibration.saveProfileButton");
                valid &= Require(calibration.loadProfileButton, "D3.Calibration.loadProfileButton");
                valid &= Require(calibration.autoRepeatPartButton, "D3.Calibration.autoRepeatPartButton");
                valid &= Require(calibration.autoRepeatAllButton, "D3.Calibration.autoRepeatAllButton");
                valid &= Require(calibration.queueTraitAssemblyButton, "D3.Calibration.queueTraitAssemblyButton");
                valid &= Require(calibration.backButton, "D3.Calibration.backButton");
                valid &= Require(calibration.readingsText, "D3.Calibration.readingsText");
                valid &= Require(calibration.previewText, "D3.Calibration.previewText");
                valid &= Require(calibration.noticeText, "D3.Calibration.noticeText");
            }
        }

        if (valid)
            Debug.Log("[D3 Block 1 UI] VALIDACIÓN OK: pestaña y panel mínimo conectados.");
    }

    public static void ConfigureBlock1UIBatch()
    {
        ConfigureBlock1UI();
    }

    private static Button GetOrCreateTabButton(TabsUI tabs)
    {
        Transform parent = tabs.btnDimension2.transform.parent;
        Transform existing = parent.Find("Btn_Dimension3");
        GameObject buttonObject;

        if (existing != null)
        {
            buttonObject = existing.gameObject;
        }
        else
        {
            buttonObject = Object.Instantiate(tabs.btnDimension2.gameObject, parent, false);
            buttonObject.name = "Btn_Dimension3";
            Undo.RegisterCreatedObjectUndo(buttonObject, "Create Dimension 3 tab button");
        }

        Button button = buttonObject.GetComponent<Button>();
        if (button == null)
            button = Undo.AddComponent<Button>(buttonObject);

        button.transform.SetSiblingIndex(tabs.btnDimension2.transform.GetSiblingIndex() + 1);
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

            label.text = "DIMENSIÓN 3";
            EditorUtility.SetDirty(label);
        }

        EditorUtility.SetDirty(button);
        return button;
    }

    private static Dimension3PanelUI GetOrCreatePanel(TabsUI tabs)
    {
        Transform parent = tabs.dimension2Panel.transform.parent;
        Transform existing = parent.Find("Dimension3Panel");
        GameObject panelObject;

        if (existing != null)
        {
            panelObject = existing.gameObject;
        }
        else
        {
            panelObject = CreateUIObject("Dimension3Panel", parent);
            Image background = Undo.AddComponent<Image>(panelObject);
            background.color = new Color(0.035f, 0.055f, 0.065f, 1f);
        }

        Stretch(panelObject.GetComponent<RectTransform>());
        Dimension3PanelUI panel = panelObject.GetComponent<Dimension3PanelUI>();
        if (panel == null)
            panel = Undo.AddComponent<Dimension3PanelUI>(panelObject);

        ClearGeneratedChildren(panelObject.transform);
        BuildFirstEntry(panel);
        BuildFactory(panel);
        BuildCalibration(panel);
        BuildResearch(panel);
        BuildFacilities(panel);
        BuildAutomation(panel);
        BuildConsole(panel);
        BuildDiagnostic(panel);
        BuildQueues(panel);
        BuildCloseButton(panel);
        return panel;
    }

    private static void BuildFirstEntry(Dimension3PanelUI panel)
    {
        GameObject root = CreateView("D3_FirstEntry", panel.transform);
        panel.firstEntryRoot = root;

        CreateText(
            "Title",
            root.transform,
            "DIMENSIÓN 3 — FÁBRICA",
            38f,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.72f),
            new Vector2(0.86f, 0.15f)
        );
        CreateText(
            "Description",
            root.transform,
            "Las instalaciones despiertan alrededor de un único operario MK1. " +
            "Produce cinco piezas V1 y ensambla la primera unidad de tu propia línea.",
            23f,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.49f),
            new Vector2(0.76f, 0.22f)
        );
        panel.continueFirstEntryButton = CreateButton(
            "Btn_OpenFactory",
            root.transform,
            "ABRIR BANCO DE PROCESOS",
            new Vector2(0.5f, 0.25f),
            new Vector2(0.34f, 0.09f)
        );
    }

    private static void BuildFactory(Dimension3PanelUI panel)
    {
        GameObject root = CreateView("D3_Factory", panel.transform);
        panel.factoryRoot = root;

        CreateText(
            "Title",
            root.transform,
            "FÁBRICA — BANCO DE PROCESOS",
            30f,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.93f),
            new Vector2(0.70f, 0.08f)
        );

        panel.factoryStatusText = CreateText(
            "FactoryStatus",
            root.transform,
            "BANCO DE PROCESOS — NIVEL 1",
            18f,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.84f),
            new Vector2(0.76f, 0.11f)
        );

        panel.inventoryText = CreateText(
            "Inventory",
            root.transform,
            "INVENTARIO V1",
            20f,
            TextAlignmentOptions.TopLeft,
            new Vector2(0.15f, 0.59f),
            new Vector2(0.25f, 0.40f)
        );

        CreateText(
            "ProductionTitle",
            root.transform,
            "PRODUCIR LOTE DE PIEZAS",
            20f,
            TextAlignmentOptions.Center,
            new Vector2(0.50f, 0.73f),
            new Vector2(0.30f, 0.06f)
        );

        panel.productionVersionDropdown = CreateDropdown(
            "D3_ProductionVersion", root.transform, "V1",
            new Vector2(0.45f, 0.68f), new Vector2(0.11f, 0.055f));
        panel.productionQuantityDropdown = CreateDropdown(
            "D3_ProductionQuantity", root.transform, "Cantidad 1",
            new Vector2(0.57f, 0.68f), new Vector2(0.14f, 0.055f));

        panel.produceChassisButton = CreateButton(
            "Btn_ProduceChassis", root.transform, "CHASIS — 100 LE + 1 TRAZA",
            new Vector2(0.50f, 0.60f), new Vector2(0.31f, 0.06f));
        panel.produceMotorButton = CreateButton(
            "Btn_ProduceMotor", root.transform, "SISTEMA MOTRIZ — 100 LE + 1 TRAZA",
            new Vector2(0.50f, 0.52f), new Vector2(0.31f, 0.06f));
        panel.produceToolButton = CreateButton(
            "Btn_ProduceTool", root.transform, "HERRAMIENTA — 100 LE + 1 TRAZA",
            new Vector2(0.50f, 0.44f), new Vector2(0.31f, 0.06f));
        panel.produceControlButton = CreateButton(
            "Btn_ProduceControl", root.transform, "MÓDULO DE CONTROL — 100 LE + 1 TRAZA",
            new Vector2(0.50f, 0.36f), new Vector2(0.31f, 0.06f));
        panel.produceRegulatorButton = CreateButton(
            "Btn_ProduceRegulator", root.transform, "REGULADOR — 100 LE + 1 TRAZA",
            new Vector2(0.50f, 0.28f), new Vector2(0.31f, 0.06f));

        panel.assembleMk1Button = CreateButton(
            "Btn_AssembleMk1",
            root.transform,
            "ENSAMBLAR NORMAL",
            new Vector2(0.50f, 0.15f),
            new Vector2(0.34f, 0.065f)
        );
        panel.assemblyMkDropdown = CreateDropdown(
            "D3_AssemblyMk", root.transform, "MK1",
            new Vector2(0.45f, 0.215f), new Vector2(0.11f, 0.05f));
        panel.assemblyQuantityDropdown = CreateDropdown(
            "D3_AssemblyQuantity", root.transform, "Cantidad 1",
            new Vector2(0.57f, 0.215f), new Vector2(0.14f, 0.05f));

        panel.queueText = CreateText(
            "Queues",
            root.transform,
            "PRODUCCIÓN\nSin trabajos.\n\nENSAMBLAJE\nSin trabajos.",
            18f,
            TextAlignmentOptions.TopLeft,
            new Vector2(0.83f, 0.57f),
            new Vector2(0.27f, 0.40f)
        );

        panel.cancelPartJobButton = CreateButton(
            "Btn_CancelPartJob",
            root.transform,
            "CANCELAR ÚLTIMA PIEZA",
            new Vector2(0.83f, 0.31f),
            new Vector2(0.24f, 0.055f)
        );
        panel.cancelAssemblyJobButton = CreateButton(
            "Btn_CancelAssemblyJob",
            root.transform,
            "CANCELAR ENSAMBLAJE",
            new Vector2(0.83f, 0.24f),
            new Vector2(0.24f, 0.055f)
        );

        panel.assignmentText = CreateText(
            "AssignmentStatus", root.transform, "ASIGNACIÓN SELECCIONADA",
            15f, TextAlignmentOptions.TopLeft,
            new Vector2(0.15f, 0.30f), new Vector2(0.25f, 0.12f));
        panel.assignmentMkDropdown = CreateDropdown(
            "D3_AssignmentMk", root.transform, "MK1",
            new Vector2(0.07f, 0.21f), new Vector2(0.09f, 0.05f));
        panel.assignmentTraitDropdown = CreateDropdown(
            "D3_AssignmentTrait", root.transform, "Normal",
            new Vector2(0.16f, 0.21f), new Vector2(0.12f, 0.05f));
        panel.assignmentChannelDropdown = CreateDropdown(
            "D3_AssignmentChannel", root.transform, "Potencia",
            new Vector2(0.28f, 0.21f), new Vector2(0.15f, 0.05f));
        panel.addAssignmentButton = CreateButton(
            "D3_AddAssignment", root.transform, "+1 ASIGNAR",
            new Vector2(0.10f, 0.13f), new Vector2(0.13f, 0.05f));
        panel.removeAssignmentButton = CreateButton(
            "D3_RemoveAssignment", root.transform, "−1 RETIRAR",
            new Vector2(0.25f, 0.13f), new Vector2(0.13f, 0.05f));

        panel.powerText = CreateText(
            "PowerStatus", root.transform, "BONIFICACIONES DEL BANCO",
            14f, TextAlignmentOptions.Center,
            new Vector2(0.50f, 0.045f), new Vector2(0.48f, 0.05f));
        panel.costPreviewText = CreateText(
            "CostPreview", root.transform, "PREVISIÓN DEL ENSAMBLAJE",
            13f, TextAlignmentOptions.Center,
            new Vector2(0.50f, 0.09f), new Vector2(0.52f, 0.055f));
        panel.upgradeProcessBankButton = CreateButton(
            "D3_UpgradeProcessBank", root.transform, "MEJORAR BANCO",
            new Vector2(0.83f, 0.09f), new Vector2(0.24f, 0.065f));
        panel.openQueuesButton = CreateButton(
            "D3_OpenQueues", root.transform, "GESTIONAR LAS 4 COLAS",
            new Vector2(0.83f, 0.17f), new Vector2(0.24f, 0.055f));

        panel.openCalibrationButton = CreateButton(
            "D3_OpenCalibration", root.transform, "MESA DE CALIBRACIÓN",
            new Vector2(0.83f, 0.76f), new Vector2(0.24f, 0.06f));
        panel.openResearchButton = CreateButton(
            "D3_OpenResearch", root.transform, "INVESTIGACIONES V4–V6",
            new Vector2(0.83f, 0.84f), new Vector2(0.24f, 0.055f));
        panel.openFacilitiesButton = CreateButton(
            "D3_OpenFacilities", root.transform, "INSTALACIONES CONECTADAS",
            new Vector2(0.16f, 0.84f), new Vector2(0.24f, 0.055f));

        panel.noticeText = CreateText(
            "Notice",
            root.transform,
            "Produce una unidad de cada pieza V1.",
            17f,
            TextAlignmentOptions.Center,
            new Vector2(0.50f, 0.015f),
            new Vector2(0.64f, 0.03f)
        );
    }

    private static void BuildCalibration(Dimension3PanelUI panel)
    {
        GameObject root = CreateView("D3_Calibration", panel.transform);
        D3CalibrationPanelUI calibration = Undo.AddComponent<D3CalibrationPanelUI>(root);
        calibration.factoryRoot = panel.factoryRoot;
        panel.calibrationPanel = calibration;

        CreateText("Title", root.transform, "MESA DE CALIBRACIÓN", 30f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.93f),
            new Vector2(0.65f, 0.08f));
        calibration.backButton = CreateButton("D3_CalibrationBack", root.transform,
            "VOLVER AL BANCO", new Vector2(0.77f, 0.93f), new Vector2(0.16f, 0.06f));
        calibration.partDropdown = CreateDropdown("D3_CalibrationPart", root.transform,
            "Chasis", new Vector2(0.25f, 0.83f), new Vector2(0.28f, 0.065f));
        calibration.mkDropdown = CreateDropdown("D3_CalibrationMk", root.transform,
            "MK1", new Vector2(0.62f, 0.83f), new Vector2(0.14f, 0.065f));
        calibration.quantityDropdown = CreateDropdown("D3_CalibrationQuantity", root.transform,
            "Cantidad 1", new Vector2(0.80f, 0.83f), new Vector2(0.16f, 0.065f));
        calibration.instructionsText = CreateText("Instructions", root.transform,
            "Configura la pieza seleccionada.", 18f, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.73f), new Vector2(0.76f, 0.08f));

        calibration.valueALabel = CreateText("ValueALabel", root.transform, "Valor A", 16f,
            TextAlignmentOptions.Center, new Vector2(0.25f, 0.64f), new Vector2(0.22f, 0.05f));
        calibration.valueBLabel = CreateText("ValueBLabel", root.transform, "Valor B", 16f,
            TextAlignmentOptions.Center, new Vector2(0.50f, 0.64f), new Vector2(0.22f, 0.05f));
        calibration.valueCLabel = CreateText("ValueCLabel", root.transform, "Valor C", 16f,
            TextAlignmentOptions.Center, new Vector2(0.75f, 0.64f), new Vector2(0.22f, 0.05f));
        calibration.valueASlider = CreateSlider("D3_ValueA", root.transform,
            new Vector2(0.25f, 0.57f), new Vector2(0.22f, 0.045f));
        calibration.valueBSlider = CreateSlider("D3_ValueB", root.transform,
            new Vector2(0.50f, 0.57f), new Vector2(0.22f, 0.045f));
        calibration.valueCSlider = CreateSlider("D3_ValueC", root.transform,
            new Vector2(0.75f, 0.57f), new Vector2(0.22f, 0.045f));

        calibration.optionADropdown = CreateDropdown("D3_OptionA", root.transform,
            "Opción A", new Vector2(0.25f, 0.48f), new Vector2(0.22f, 0.06f));
        calibration.optionBDropdown = CreateDropdown("D3_OptionB", root.transform,
            "Opción B", new Vector2(0.50f, 0.48f), new Vector2(0.22f, 0.06f));
        calibration.optionCDropdown = CreateDropdown("D3_OptionC", root.transform,
            "Opción C", new Vector2(0.75f, 0.48f), new Vector2(0.22f, 0.06f));
        calibration.recordPartButton = CreateButton("D3_RecordPart", root.transform,
            "REGISTRAR LECTURA DE LA PIEZA", new Vector2(0.5f, 0.39f),
            new Vector2(0.34f, 0.065f));

        calibration.readingsText = CreateText("Readings", root.transform,
            "LECTURAS\nChasis: Pendiente", 17f, TextAlignmentOptions.TopLeft,
            new Vector2(0.18f, 0.25f), new Vector2(0.30f, 0.22f));
        calibration.previewText = CreateText("Preview", root.transform,
            "PREVIEW\nCalibra al menos una pieza.", 17f, TextAlignmentOptions.TopLeft,
            new Vector2(0.82f, 0.25f), new Vector2(0.30f, 0.22f));

        calibration.saveProfileButton = CreateButton("D3_SaveProfile", root.transform,
            "GUARDAR (N1)", new Vector2(0.15f, 0.14f), new Vector2(0.16f, 0.055f));
        calibration.loadProfileButton = CreateButton("D3_LoadProfile", root.transform,
            "CARGAR (N2)", new Vector2(0.33f, 0.14f), new Vector2(0.16f, 0.055f));
        calibration.autoRepeatPartButton = CreateButton("D3_AutoPart", root.transform,
            "UNA PIEZA (N3)", new Vector2(0.51f, 0.14f), new Vector2(0.16f, 0.055f));
        calibration.autoRepeatAllButton = CreateButton("D3_AutoAll", root.transform,
            "CINCO PIEZAS (N4)", new Vector2(0.69f, 0.14f), new Vector2(0.16f, 0.055f));
        calibration.queueTraitAssemblyButton = CreateButton("D3_QueueTrait", root.transform,
            "CONFIRMAR INTENTO CON RASGO", new Vector2(0.5f, 0.07f),
            new Vector2(0.34f, 0.06f));
        calibration.noticeText = CreateText("CalibrationNotice", root.transform,
            "Las piezas y recursos solo se consumen al confirmar.", 16f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.02f),
            new Vector2(0.70f, 0.035f));

        root.SetActive(false);
    }

    private static void BuildResearch(Dimension3PanelUI panel)
    {
        GameObject root = CreateView("D3_Research", panel.transform);
        D3ResearchPanelUI ui = Undo.AddComponent<D3ResearchPanelUI>(root);
        ui.factoryRoot = panel.factoryRoot;
        panel.researchPanel = ui;
        CreateText("Title", root.transform, "INVESTIGACIÓN DE PIEZAS V4–V6", 30f,
            TextAlignmentOptions.Center, new Vector2(.5f,.93f), new Vector2(.65f,.08f));
        ui.backButton = CreateButton("D3_ResearchBack", root.transform, "VOLVER AL BANCO",
            new Vector2(.77f,.93f), new Vector2(.16f,.06f));
        ui.partDropdown = CreateDropdown("D3_ResearchPart", root.transform, "Chasis",
            new Vector2(.30f,.82f), new Vector2(.28f,.065f));
        ui.versionDropdown = CreateDropdown("D3_ResearchVersion", root.transform, "V4",
            new Vector2(.65f,.82f), new Vector2(.14f,.065f));
        ui.teamMkDropdown = CreateDropdown("D3_TeamMk", root.transform, "MK1",
            new Vector2(.25f,.68f), new Vector2(.14f,.06f));
        ui.teamTraitDropdown = CreateDropdown("D3_TeamTrait", root.transform, "Normal",
            new Vector2(.45f,.68f), new Vector2(.18f,.06f));
        ui.addTeamButton = CreateButton("D3_AddResearcher", root.transform, "+1 INVESTIGADOR",
            new Vector2(.67f,.68f), new Vector2(.19f,.06f));
        ui.removeTeamButton = CreateButton("D3_RemoveResearcher", root.transform, "−1 INVESTIGADOR",
            new Vector2(.86f,.68f), new Vector2(.19f,.06f));
        ui.teamText = CreateText("Team", root.transform, "EQUIPO\nSin investigadores.", 18f,
            TextAlignmentOptions.TopLeft, new Vector2(.22f,.43f), new Vector2(.34f,.34f));
        ui.researchText = CreateText("Lines", root.transform, "LÍNEAS", 18f,
            TextAlignmentOptions.TopLeft, new Vector2(.52f,.43f), new Vector2(.28f,.34f));
        ui.queueText = CreateText("Queue", root.transform, "COLA\nSin investigaciones.", 18f,
            TextAlignmentOptions.TopLeft, new Vector2(.82f,.43f), new Vector2(.28f,.34f));
        ui.queueResearchButton = CreateButton("D3_QueueResearch", root.transform, "INICIAR INVESTIGACIÓN",
            new Vector2(.38f,.17f), new Vector2(.27f,.07f));
        ui.cancelResearchButton = CreateButton("D3_CancelResearch", root.transform, "CANCELAR INVESTIGACIÓN",
            new Vector2(.68f,.17f), new Vector2(.27f,.07f));
        ui.noticeText = CreateText("Notice", root.transform,
            "Los investigadores quedan reservados hasta completar o cancelar.", 17f,
            TextAlignmentOptions.Center, new Vector2(.5f,.07f), new Vector2(.70f,.06f));
        root.SetActive(false);
    }

    private static void BuildFacilities(Dimension3PanelUI panel)
    {
        GameObject root = CreateView("D3_Facilities", panel.transform);
        D3FacilitiesPanelUI ui = Undo.AddComponent<D3FacilitiesPanelUI>(root);
        ui.factoryRoot = panel.factoryRoot;
        panel.facilitiesPanel = ui;

        CreateText("Title", root.transform, "INSTALACIONES CONECTADAS", 29f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.93f),
            new Vector2(0.58f, 0.07f));
        ui.backButton = CreateButton("D3_FacilitiesBack", root.transform,
            "VOLVER AL BANCO", new Vector2(0.77f, 0.93f),
            new Vector2(0.16f, 0.06f));
        ui.facilityDropdown = CreateDropdown("D3_FacilitySelect", root.transform,
            "Consola de Producción", new Vector2(0.25f, 0.82f),
            new Vector2(0.30f, 0.06f));
        ui.statusText = CreateText("D3_FacilityStatus", root.transform,
            "CONSOLA DE PRODUCCIÓN — NIVEL 0", 18f,
            TextAlignmentOptions.TopLeft, new Vector2(0.25f, 0.68f),
            new Vector2(0.38f, 0.20f));
        ui.functionsText = CreateText("D3_FacilityFunctions", root.transform,
            "FUNCIONES", 17f, TextAlignmentOptions.TopLeft,
            new Vector2(0.73f, 0.57f), new Vector2(0.42f, 0.47f));
        ui.mkDropdown = CreateDropdown("D3_FacilityMk", root.transform, "MK1",
            new Vector2(0.10f, 0.48f), new Vector2(0.11f, 0.055f));
        ui.traitDropdown = CreateDropdown("D3_FacilityTrait", root.transform,
            "Normal", new Vector2(0.24f, 0.48f), new Vector2(0.15f, 0.055f));
        ui.channelDropdown = CreateDropdown("D3_FacilityChannel", root.transform,
            "Capacidad", new Vector2(0.40f, 0.48f), new Vector2(0.17f, 0.055f));
        ui.addAssignmentButton = CreateButton("D3_FacilityAdd", root.transform,
            "+1 ASIGNAR", new Vector2(0.17f, 0.38f),
            new Vector2(0.16f, 0.06f));
        ui.removeAssignmentButton = CreateButton("D3_FacilityRemove", root.transform,
            "−1 RETIRAR", new Vector2(0.37f, 0.38f),
            new Vector2(0.16f, 0.06f));
        ui.upgradeButton = CreateButton("D3_FacilityUpgrade", root.transform,
            "CONSTRUIR NIVEL 1", new Vector2(0.27f, 0.25f),
            new Vector2(0.30f, 0.075f));
        ui.toggleAutoAnalyzeButton = CreateButton("D3_AutoAnalyze", root.transform,
            "AUTOANÁLISIS: OFF", new Vector2(0.65f, 0.25f),
            new Vector2(0.21f, 0.06f));
        ui.toggleAutoRepairButton = CreateButton("D3_AutoRepair", root.transform,
            "AUTORREPARACIÓN: OFF", new Vector2(0.85f, 0.25f),
            new Vector2(0.21f, 0.06f));
        ui.openAutomationButton = CreateButton("D3_OpenAutomation", root.transform,
            "RUTINAS Y PERFILES", new Vector2(0.75f, 0.22f),
            new Vector2(0.25f, 0.06f));
        ui.integrateAutonomyCoreButton = CreateButton("D3_IntegrateAutonomyCore",
            root.transform, "INTEGRAR NÚCLEO DE AUTONOMÍA",
            new Vector2(0.75f, 0.14f), new Vector2(0.30f, 0.06f));
        ui.openConsoleButton = CreateButton("D3_OpenConsole", root.transform,
            "CONTROL DE CONSOLA", new Vector2(0.75f, 0.14f),
            new Vector2(0.25f, 0.06f));
        ui.openDiagnosticButton = CreateButton("D3_OpenDiagnostic", root.transform,
            "CONTROL DE DIAGNÓSTICO", new Vector2(0.75f, 0.14f),
            new Vector2(0.25f, 0.06f));
        ui.noticeText = CreateText("D3_FacilityNotice", root.transform,
            "Construye una instalación para asignar autómatas.", 17f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.045f),
            new Vector2(0.80f, 0.055f));
        root.SetActive(false);
    }

    private static void BuildAutomation(Dimension3PanelUI panel)
    {
        GameObject root = CreateView("D3_Automation", panel.transform);
        D3AutomationPanelUI ui = Undo.AddComponent<D3AutomationPanelUI>(root);
        panel.automationPanel = ui;
        ui.facilitiesPanel = panel.facilitiesPanel;
        panel.facilitiesPanel.automationPanel = ui;

        CreateText("Title", root.transform, "RUTINAS Y PERFILES ONLINE", 29f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.93f),
            new Vector2(0.62f, 0.07f));
        ui.backButton = CreateButton("D3_AutomationBack", root.transform,
            "VOLVER A INSTALACIONES", new Vector2(0.80f, 0.93f),
            new Vector2(0.22f, 0.06f));
        ui.statusText = CreateText("D3_AutomationStatus", root.transform,
            "MOTOR ONLINE", 18f, TextAlignmentOptions.TopLeft,
            new Vector2(0.25f, 0.80f), new Vector2(0.40f, 0.16f));

        ui.actionDropdown = CreateDropdown("D3_AutomationAction", root.transform,
            "Barrido simple", new Vector2(0.16f, 0.64f),
            new Vector2(0.24f, 0.06f));
        ui.targetDropdown = CreateDropdown("D3_AutomationTarget", root.transform,
            "Cinturón mineral", new Vector2(0.43f, 0.64f),
            new Vector2(0.25f, 0.06f));
        ui.priorityDropdown = CreateDropdown("D3_AutomationPriority", root.transform,
            "Prioridad 0", new Vector2(0.68f, 0.64f),
            new Vector2(0.18f, 0.06f));
        ui.stopDropdown = CreateDropdown("D3_AutomationStop", root.transform,
            "Sin límite", new Vector2(0.20f, 0.53f),
            new Vector2(0.22f, 0.06f));
        ui.reserveDropdown = CreateDropdown("D3_AutomationReserve", root.transform,
            "Reserva 0", new Vector2(0.45f, 0.53f),
            new Vector2(0.20f, 0.06f));
        ui.createButton = CreateButton("D3_AutomationCreate", root.transform,
            "CREAR RUTINA PAUSADA", new Vector2(0.72f, 0.53f),
            new Vector2(0.26f, 0.065f));

        ui.routineDropdown = CreateDropdown("D3_AutomationRoutine", root.transform,
            "Sin rutinas", new Vector2(0.24f, 0.39f),
            new Vector2(0.34f, 0.06f));
        ui.routineText = CreateText("D3_AutomationRoutineStatus", root.transform,
            "RUTINA SELECCIONADA\nNo hay rutinas creadas.", 17f,
            TextAlignmentOptions.TopLeft, new Vector2(0.68f, 0.34f),
            new Vector2(0.50f, 0.25f));
        ui.toggleButton = CreateButton("D3_AutomationToggle", root.transform,
            "ACTIVAR RUTINA", new Vector2(0.16f, 0.27f),
            new Vector2(0.20f, 0.06f));
        ui.deleteButton = CreateButton("D3_AutomationDelete", root.transform,
            "ELIMINAR RUTINA", new Vector2(0.39f, 0.27f),
            new Vector2(0.20f, 0.06f));
        ui.profileDropdown = CreateDropdown("D3_AutomationProfile", root.transform,
            "Perfil 1", new Vector2(0.55f, 0.18f),
            new Vector2(0.15f, 0.06f));
        ui.saveProfileButton = CreateButton("D3_AutomationSaveProfile", root.transform,
            "GUARDAR PERFIL", new Vector2(0.72f, 0.18f),
            new Vector2(0.17f, 0.06f));
        ui.loadProfileButton = CreateButton("D3_AutomationLoadProfile", root.transform,
            "CARGAR PERFIL", new Vector2(0.90f, 0.18f),
            new Vector2(0.17f, 0.06f));
        ui.noticeText = CreateText("D3_AutomationNotice", root.transform,
            "Solo se aceptan acciones autorizadas y ejecutadas manualmente antes.",
            16f, TextAlignmentOptions.Center, new Vector2(0.5f, 0.07f),
            new Vector2(0.82f, 0.07f));
        root.SetActive(false);
    }

    private static void BuildConsole(Dimension3PanelUI panel)
    {
        GameObject root = CreateView("D3_Console", panel.transform);
        D3ConsolePanelUI ui = Undo.AddComponent<D3ConsolePanelUI>(root);
        ui.facilitiesPanel = panel.facilitiesPanel;
        ui.automationPanel = panel.automationPanel;
        panel.facilitiesPanel.consolePanel = ui;

        CreateText("Title", root.transform, "CONTROL DE CONSOLA", 30f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.92f),
            new Vector2(0.58f, 0.08f));
        ui.backButton = CreateButton("D3_ConsoleBack", root.transform,
            "VOLVER A INSTALACIONES", new Vector2(0.82f, 0.92f),
            new Vector2(0.22f, 0.06f));
        ui.statusText = CreateText("D3_ConsoleStatus", root.transform,
            "CONSOLA — NIVEL 0", 19f, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.78f), new Vector2(0.78f, 0.15f));
        ui.policyDropdown = CreateDropdown("D3_ConsolePolicy", root.transform,
            "Equilibrio", new Vector2(0.24f, 0.61f), new Vector2(0.22f, 0.06f));
        ui.leReserveDropdown = CreateDropdown("D3_ConsoleLEReserve", root.transform,
            "Reserva LE 0", new Vector2(0.50f, 0.61f), new Vector2(0.22f, 0.06f));
        ui.tracesReserveDropdown = CreateDropdown("D3_ConsoleTReserve", root.transform,
            "Reserva T 0", new Vector2(0.76f, 0.61f), new Vector2(0.22f, 0.06f));
        ui.savePolicyButton = CreateButton("D3_ConsoleSavePolicy", root.transform,
            "GUARDAR POLÍTICA Y RESERVAS", new Vector2(0.5f, 0.51f),
            new Vector2(0.32f, 0.065f));
        ui.historyText = CreateText("D3_ConsoleHistory", root.transform,
            "AUTORIZACIONES MANUALES", 18f, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.36f), new Vector2(0.78f, 0.18f));
        ui.recordPhaseButton = CreateButton("D3_ConsoleRecordPhase", root.transform,
            "REGISTRAR FASE ACTUAL", new Vector2(0.28f, 0.22f),
            new Vector2(0.25f, 0.06f));
        ui.recordTriangleButton = CreateButton("D3_ConsoleRecordTriangle", root.transform,
            "REGISTRAR TRIÁNGULO ACTUAL", new Vector2(0.55f, 0.22f),
            new Vector2(0.27f, 0.06f));
        ui.openRoutinesButton = CreateButton("D3_ConsoleRoutines", root.transform,
            "RUTINAS DE COMPRA", new Vector2(0.80f, 0.22f),
            new Vector2(0.20f, 0.06f));
        ui.noticeText = CreateText("D3_ConsoleNotice", root.transform,
            "Solo se automatiza lo que ya fue ejecutado manualmente.", 17f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.08f),
            new Vector2(0.82f, 0.08f));
        root.SetActive(false);
    }

    private static void BuildDiagnostic(Dimension3PanelUI panel)
    {
        GameObject root = CreateView("D3_Diagnostic", panel.transform);
        D3DiagnosticPanelUI ui = Undo.AddComponent<D3DiagnosticPanelUI>(root);
        ui.facilitiesPanel = panel.facilitiesPanel;
        panel.facilitiesPanel.diagnosticPanel = ui;

        CreateText("Title", root.transform, "CONTROL DE DIAGNÓSTICO", 30f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.93f),
            new Vector2(0.58f, 0.08f));
        ui.backButton = CreateButton("D3_DiagnosticBack", root.transform,
            "VOLVER A INSTALACIONES", new Vector2(0.82f, 0.93f),
            new Vector2(0.22f, 0.06f));
        ui.statusText = CreateText("D3_DiagnosticStatus", root.transform,
            "BANCO DE DIAGNÓSTICO — NIVEL 0", 18f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.82f),
            new Vector2(0.78f, 0.15f));

        ui.toggleAnalyzeButton = CreateButton("D3_DiagnosticAnalyze", root.transform,
            "AUTOANÁLISIS: OFF", new Vector2(0.22f, 0.68f),
            new Vector2(0.22f, 0.06f));
        ui.toggleRepairButton = CreateButton("D3_DiagnosticRepair", root.transform,
            "AUTORREPARACIÓN: OFF", new Vector2(0.50f, 0.68f),
            new Vector2(0.22f, 0.06f));
        ui.toggleFusionButton = CreateButton("D3_DiagnosticFusion", root.transform,
            "AUTOFUSIÓN: OFF", new Vector2(0.78f, 0.68f),
            new Vector2(0.22f, 0.06f));

        ui.priorityModeDropdown = CreateDropdown("D3_DiagnosticPriorityMode",
            root.transform, "Orden ascendente", new Vector2(0.18f, 0.55f),
            new Vector2(0.24f, 0.06f));
        ui.zoneDropdown = CreateDropdown("D3_DiagnosticZone", root.transform,
            "Sin zona", new Vector2(0.43f, 0.55f), new Vector2(0.22f, 0.06f));
        ui.leReserveDropdown = CreateDropdown("D3_DiagnosticLEReserve",
            root.transform, "Reserva LE 0", new Vector2(0.66f, 0.55f),
            new Vector2(0.20f, 0.06f));
        ui.tracesReserveDropdown = CreateDropdown("D3_DiagnosticTReserve",
            root.transform, "Reserva T 0", new Vector2(0.86f, 0.55f),
            new Vector2(0.17f, 0.06f));
        ui.saveSettingsButton = CreateButton("D3_DiagnosticSaveSettings",
            root.transform, "GUARDAR PRIORIDAD Y RESERVAS",
            new Vector2(0.5f, 0.46f), new Vector2(0.31f, 0.06f));

        ui.recipeText = CreateText("D3_DiagnosticRecipeStatus", root.transform,
            "RECETAS DE FUSIÓN", 18f, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.37f), new Vector2(0.72f, 0.09f));
        ui.recipeDropdown = CreateDropdown("D3_DiagnosticRecipe", root.transform,
            "Sin recetas manuales", new Vector2(0.35f, 0.28f),
            new Vector2(0.38f, 0.06f));
        ui.toggleRecipeMarkButton = CreateButton("D3_DiagnosticMarkRecipe",
            root.transform, "MARCAR RECETA", new Vector2(0.68f, 0.28f),
            new Vector2(0.22f, 0.06f));
        ui.saveRoutineButton = CreateButton("D3_DiagnosticSaveRoutine",
            root.transform, "GUARDAR RUTINA N5", new Vector2(0.35f, 0.17f),
            new Vector2(0.23f, 0.06f));
        ui.loadRoutineButton = CreateButton("D3_DiagnosticLoadRoutine",
            root.transform, "CARGAR RUTINA N5", new Vector2(0.65f, 0.17f),
            new Vector2(0.23f, 0.06f));
        ui.noticeText = CreateText("D3_DiagnosticNotice", root.transform,
            "Solo se repiten recetas ejecutadas y marcadas manualmente.", 17f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.07f),
            new Vector2(0.82f, 0.07f));
        root.SetActive(false);
    }

    private static void BuildQueues(Dimension3PanelUI panel)
    {
        GameObject root = CreateView("D3_Queues", panel.transform);
        D3QueuesPanelUI ui = Undo.AddComponent<D3QueuesPanelUI>(root);
        panel.queuesPanel = ui;
        ui.factoryRoot = panel.factoryRoot;

        CreateText("Title", root.transform, "GESTIÓN DE COLAS", 30f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.91f),
            new Vector2(0.60f, 0.08f));
        ui.backButton = CreateButton("D3_QueuesBack", root.transform,
            "VOLVER AL BANCO", new Vector2(0.82f, 0.91f),
            new Vector2(0.20f, 0.06f));
        CreateText("QueueLabel", root.transform, "COLA", 17f,
            TextAlignmentOptions.Center, new Vector2(0.28f, 0.75f),
            new Vector2(0.24f, 0.05f));
        CreateText("JobLabel", root.transform, "TRABAJO", 17f,
            TextAlignmentOptions.Center, new Vector2(0.67f, 0.75f),
            new Vector2(0.42f, 0.05f));
        ui.queueDropdown = CreateDropdown("D3_QueueSelect", root.transform,
            "Producción", new Vector2(0.28f, 0.68f),
            new Vector2(0.25f, 0.065f));
        ui.jobDropdown = CreateDropdown("D3_JobSelect", root.transform,
            "Sin trabajos", new Vector2(0.67f, 0.68f),
            new Vector2(0.45f, 0.065f));
        ui.detailText = CreateText("D3_QueueDetail", root.transform,
            "Esta cola está vacía.", 20f, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.45f), new Vector2(0.72f, 0.25f));
        ui.cancelSelectedButton = CreateButton("D3_CancelSelected", root.transform,
            "CANCELAR TRABAJO SELECCIONADO", new Vector2(0.5f, 0.24f),
            new Vector2(0.34f, 0.075f));
        ui.noticeText = CreateText("D3_QueueNotice", root.transform,
            "Los pendientes devuelven todo; los activos no devuelven consumibles.",
            17f, TextAlignmentOptions.Center, new Vector2(0.5f, 0.10f),
            new Vector2(0.78f, 0.08f));
        root.SetActive(false);
    }

    private static Slider CreateSlider(
        string name, Transform parent, Vector2 anchor, Vector2 size)
    {
        GameObject sliderObject = CreateUIObject(name, parent);
        RectTransform rect = sliderObject.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(1100f * size.x, 650f * size.y);

        Slider slider = Undo.AddComponent<Slider>(sliderObject);
        GameObject backgroundObject = CreateUIObject("Background", sliderObject.transform);
        Image background = Undo.AddComponent<Image>(backgroundObject);
        background.color = new Color(0.08f, 0.14f, 0.16f, 1f);
        Stretch(background.rectTransform);

        GameObject fillArea = CreateUIObject("Fill Area", sliderObject.transform);
        Stretch(fillArea.GetComponent<RectTransform>());
        GameObject fillObject = CreateUIObject("Fill", fillArea.transform);
        Image fill = Undo.AddComponent<Image>(fillObject);
        fill.color = new Color(0.18f, 0.52f, 0.58f, 1f);
        Stretch(fill.rectTransform);

        GameObject handleArea = CreateUIObject("Handle Slide Area", sliderObject.transform);
        Stretch(handleArea.GetComponent<RectTransform>());
        GameObject handleObject = CreateUIObject("Handle", handleArea.transform);
        Image handle = Undo.AddComponent<Image>(handleObject);
        handle.color = new Color(0.82f, 0.95f, 0.96f, 1f);
        RectTransform handleRect = handle.rectTransform;
        handleRect.anchorMin = new Vector2(0.5f, 0.5f);
        handleRect.anchorMax = new Vector2(0.5f, 0.5f);
        handleRect.sizeDelta = new Vector2(18f, 30f);

        slider.fillRect = fill.rectTransform;
        slider.handleRect = handleRect;
        slider.targetGraphic = handle;
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0f;
        slider.maxValue = 100f;
        slider.value = 50f;
        slider.wholeNumbers = true;
        return slider;
    }

    private static TMP_Dropdown CreateDropdown(
        string name,
        Transform parent,
        string initialText,
        Vector2 anchor,
        Vector2 size)
    {
        GameObject dropdownObject = CreateUIObject(name, parent);
        Image image = Undo.AddComponent<Image>(dropdownObject);
        image.color = new Color(0.12f, 0.24f, 0.27f, 0.98f);
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
            "Label", dropdownObject.transform, initialText, 15f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), Vector2.one);
        Stretch(caption.rectTransform);

        GameObject templateObject = CreateUIObject("Template", dropdownObject.transform);
        RectTransform templateRect = templateObject.GetComponent<RectTransform>();
        templateRect.anchorMin = new Vector2(0f, 0f);
        templateRect.anchorMax = new Vector2(1f, 0f);
        templateRect.pivot = new Vector2(0.5f, 1f);
        templateRect.anchoredPosition = Vector2.zero;
        templateRect.sizeDelta = new Vector2(0f, 160f);
        Image templateImage = Undo.AddComponent<Image>(templateObject);
        templateImage.color = new Color(0.035f, 0.075f, 0.085f, 1f);
        ScrollRect scrollRect = Undo.AddComponent<ScrollRect>(templateObject);
        scrollRect.horizontal = false;

        GameObject viewportObject = CreateUIObject("Viewport", templateObject.transform);
        RectTransform viewportRect = viewportObject.GetComponent<RectTransform>();
        Stretch(viewportRect);
        Undo.AddComponent<Image>(viewportObject).color = Color.white;
        Undo.AddComponent<Mask>(viewportObject).showMaskGraphic = false;

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
        itemImage.color = new Color(0.12f, 0.24f, 0.27f, 1f);
        Toggle itemToggle = Undo.AddComponent<Toggle>(itemObject);
        itemToggle.targetGraphic = itemImage;
        itemToggle.onValueChanged = new Toggle.ToggleEvent();
        TextMeshProUGUI itemLabel = CreateText(
            "Item Label", itemObject.transform, initialText, 15f,
            TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), Vector2.one);
        Stretch(itemLabel.rectTransform);

        scrollRect.viewport = viewportRect;
        scrollRect.content = contentRect;
        dropdown.template = templateRect;
        dropdown.captionText = caption;
        dropdown.itemText = itemLabel;
        templateObject.SetActive(false);
        return dropdown;
    }

    private static void BuildCloseButton(Dimension3PanelUI panel)
    {
        panel.closeDimension3Button = CreateButton(
            "D3_Close",
            panel.transform,
            "CERRAR",
            new Vector2(0.93f, 0.94f),
            new Vector2(0.12f, 0.07f)
        );
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
        image.color = new Color(0.12f, 0.24f, 0.27f, 0.98f);
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
            16f,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.5f),
            Vector2.one
        );
        Stretch(label.rectTransform);
        return button;
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
        label.color = new Color(0.88f, 0.96f, 0.96f, 1f);
        label.textWrappingMode = TextWrappingModes.Normal;

        RectTransform rect = label.rectTransform;
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(1100f * size.x, 650f * size.y);
        return label;
    }

    private static GameObject CreateView(string name, Transform parent)
    {
        GameObject root = CreateUIObject(name, parent);
        Stretch(root.GetComponent<RectTransform>());
        return root;
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
            if (child.name.StartsWith("D3_"))
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

        Debug.LogError("[D3 Block 1 UI] Referencia faltante: " + label);
        return false;
    }
}
#endif

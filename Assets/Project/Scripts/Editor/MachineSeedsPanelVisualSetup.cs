#if UNITY_EDITOR
using System;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class MachineSeedsPanelVisualSetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ThemePath =
        "Assets/Project/UI/Vertical/Generated/VerticalUiTheme.asset";
    private const string FontPath =
        "Assets/Project/UI/Vertical/Fonts/Rajdhani-Medium SDF.asset";
    private const string ModuleFramePath =
        "Assets/Project/UI/Vertical/UpgradesPolish/qf_upgrade_module_frame.png";
    private const string ButtonFramePath =
        "Assets/Project/UI/Vertical/UpgradesPolish/qf_upgrade_button_frame.png";
    private const string SelectorFramePath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_selector_frame.png";
    private const string RingPath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_gauge_progress_ring.png";
    private const string GlowPath =
        "Assets/Project/UI/Vertical/Generated/qf_vertical_glow.png";
    private const string GridPath =
        "Assets/Project/UI/Vertical/Generated/qf_vertical_grid.png";
    private const string NodeCorePath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_node_higgs.png";
    private const string PhaseIconPath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_circuit_phase.png";
    private const string ArchiveIconPath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_circuit_experimental.png";
    private const string EnergyIconPath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_circuit_energy.png";

    private static readonly Color Background = Hex("02070D", 250);
    private static readonly Color Panel = Hex("06131D", 246);
    private static readonly Color PanelRaised = Hex("081925", 250);
    private static readonly Color TextPrimary = Hex("E8EAEC");
    private static readonly Color TextSecondary = Hex("89969F");
    private static readonly Color Cyan = Hex("00D5D0");
    private static readonly Color CyanDim = Hex("075F61");
    private static readonly Color Amber = Hex("F0A018");
    private static readonly Color AmberDim = Hex("704707");
    private static readonly Color Track = Hex("142027");

    [MenuItem("Tools/Quantum Forge/Machine/Configure Seeds Panel Visual")]
    public static void Configure()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Room2PanelUI room = UnityEngine.Object.FindFirstObjectByType<Room2PanelUI>(
            FindObjectsInactive.Include);
        MachinePanelUI machine = UnityEngine.Object.FindFirstObjectByType<MachinePanelUI>(
            FindObjectsInactive.Include);
        Require(room != null, "No se encontró Room2PanelUI.");
        Require(machine != null, "No se encontró MachinePanelUI.");

        VerticalUiTheme theme = AssetDatabase.LoadAssetAtPath<VerticalUiTheme>(ThemePath);
        TMP_FontAsset font = theme != null && theme.primaryFont != null
            ? theme.primaryFont
            : AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        Sprite panelSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ModuleFramePath);
        Sprite buttonSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ButtonFramePath);
        Sprite selectorSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SelectorFramePath);
        Sprite ringSprite = AssetDatabase.LoadAssetAtPath<Sprite>(RingPath);
        Sprite glowSprite = AssetDatabase.LoadAssetAtPath<Sprite>(GlowPath);
        Sprite gridSprite = AssetDatabase.LoadAssetAtPath<Sprite>(GridPath);
        Sprite nodeCoreSprite = AssetDatabase.LoadAssetAtPath<Sprite>(NodeCorePath);
        Sprite phaseIconSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PhaseIconPath);
        Sprite archiveIconSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ArchiveIconPath);
        Sprite energyIconSprite = AssetDatabase.LoadAssetAtPath<Sprite>(EnergyIconPath);
        if (panelSprite == null && theme != null) panelSprite = theme.panelFrame;
        if (buttonSprite == null && theme != null) buttonSprite = theme.buttonFrame;
        if (selectorSprite == null) selectorSprite = buttonSprite;
        Require(font != null && panelSprite != null && buttonSprite != null &&
            selectorSprite != null && ringSprite != null && glowSprite != null &&
            gridSprite != null && nodeCoreSprite != null &&
            phaseIconSprite != null && archiveIconSprite != null &&
            energyIconSprite != null,
            "Faltan recursos compartidos de la UI vertical.");

        Transform oldRoot = room.transform.Find("InstantSeedsViewRoot");
        GameObject rootObject;
        if (oldRoot != null)
        {
            rootObject = oldRoot.gameObject;
            for (int i = oldRoot.childCount - 1; i >= 0; i--)
                UnityEngine.Object.DestroyImmediate(oldRoot.GetChild(i).gameObject);
        }
        else
        {
            rootObject = CreateRect("InstantSeedsViewRoot", room.transform,
                Vector2.zero, Vector2.one);
        }
        SetRect(rootObject.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);

        GameObject shell = CreatePanel("SeedsVisualShell", rootObject.transform,
            new Vector2(0f, 0.030f), new Vector2(1f, 0.838f),
            panelSprite, Background, true);
        RectTransform shellRect = shell.GetComponent<RectTransform>();
        shellRect.offsetMin = new Vector2(96f, 0f);
        shellRect.offsetMax = new Vector2(-96f, 0f);
        Image shellGrid = CreateSpriteImage("TechnicalGrid", shell.transform,
            Vector2.zero, Vector2.one, gridSprite, Hex("0B6172", 24), false);
        shellGrid.type = Image.Type.Tiled;
        shellGrid.transform.SetAsFirstSibling();
        AddOuterRails(shell.transform);

        TextMeshProUGUI heading = CreateText("SeedsTitle", shell.transform,
            "SEMILLAS Y ANCLAJES", new Vector2(0.10f, 0.952f),
            new Vector2(0.90f, 0.995f), font, 32f, TextPrimary,
            TextAlignmentOptions.Center);
        heading.fontStyle = FontStyles.Bold;
        heading.characterSpacing = 2f;
        AddHeaderRail(shell.transform, 0.06f, 0.29f);
        AddHeaderRail(shell.transform, 0.71f, 0.94f);
        CreateText("SeedsSubtitle", shell.transform,
            "CARA 4 / 4  ·  SISTEMA OPERATIVO",
            new Vector2(0.10f, 0.925f), new Vector2(0.90f, 0.958f),
            font, 20f, Cyan, TextAlignmentOptions.Center);

        GameObject status = CreatePanel("StatusStrip", shell.transform,
            new Vector2(0.025f, 0.874f), new Vector2(0.975f, 0.923f),
            selectorSprite, Hex("073B42", 210), false);
        TextMeshProUGUI matureText = CreateText("MatureSeeds", status.transform,
            "SEMILLAS MADURAS  0", new Vector2(0.02f, 0.08f),
            new Vector2(0.33f, 0.92f), font, 18f, TextPrimary,
            TextAlignmentOptions.Center);
        TextMeshProUGUI archiveUsageText = CreateText("ArchiveUsage", status.transform,
            "ARCHIVO  0 / 30", new Vector2(0.345f, 0.08f),
            new Vector2(0.655f, 0.92f), font, 18f, TextPrimary,
            TextAlignmentOptions.Center);
        TextMeshProUGUI activeAnchorText = CreateText("ActiveAnchor", status.transform,
            "CÁMARA DISPONIBLE", new Vector2(0.67f, 0.08f),
            new Vector2(0.98f, 0.92f), font, 18f, Cyan,
            TextAlignmentOptions.Center);
        AddDivider(status.transform, 0.337f);
        AddDivider(status.transform, 0.663f);
        CreateSpriteImage("SeedStatusIcon", status.transform,
            new Vector2(0.022f, 0.20f), new Vector2(0.074f, 0.80f),
            phaseIconSprite, Cyan, true);
        CreateSpriteImage("ArchiveStatusIcon", status.transform,
            new Vector2(0.365f, 0.20f), new Vector2(0.417f, 0.80f),
            archiveIconSprite, Hex("B74CF5"), true);
        CreateSpriteImage("AnchorStatusIcon", status.transform,
            new Vector2(0.690f, 0.20f), new Vector2(0.742f, 0.80f),
            energyIconSprite, Color.white, true);

        GameObject incubation = CreatePanel("Incubation", shell.transform,
            new Vector2(0.025f, 0.655f), new Vector2(0.975f, 0.865f),
            panelSprite, Panel, false);
        AddSectionAccent(incubation.transform, Cyan);
        CreateSectionTitle(incubation.transform, "INCUBACIÓN DE SEMILLAS", font,
            phaseIconSprite, Cyan);
        GameObject[] slotRoots = new GameObject[4];
        TextMeshProUGUI[] slotStates = new TextMeshProUGUI[4];
        TextMeshProUGUI[] slotMeta = new TextMeshProUGUI[4];
        Image[] slotFills = new Image[4];
        for (int i = 0; i < 4; i++)
        {
            float top = 0.79f - i * 0.185f;
            float bottom = top - 0.15f;
            slotRoots[i] = CreatePanel("SeedSlot" + (i + 1), incubation.transform,
                new Vector2(0.025f, bottom), new Vector2(0.625f, top),
                selectorSprite, PanelRaised, false);
            CreateSolidImage("Accent", slotRoots[i].transform,
                new Vector2(0.006f, 0.12f), new Vector2(0.014f, 0.88f),
                i < 3 ? Cyan : TextSecondary);
            GameObject badge = CreatePanel("Index", slotRoots[i].transform,
                new Vector2(0.02f, 0.10f), new Vector2(0.13f, 0.90f),
                buttonSprite, Hex("0A3337"), false);
            CreateText("Value", badge.transform, (i + 1).ToString(),
                new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.95f),
                font, 22f, Cyan, TextAlignmentOptions.Center);
            Image seedCircuit = CreateSpriteImage("SeedCircuit", badge.transform,
                new Vector2(0.18f, 0.14f), new Vector2(0.82f, 0.86f),
                phaseIconSprite, Hex("00D5D0", 92), true);
            seedCircuit.transform.SetAsFirstSibling();
            slotStates[i] = CreateText("State", slotRoots[i].transform,
                "SLOT " + (i + 1) + "  ·  VACÍO", new Vector2(0.16f, 0.50f),
                new Vector2(0.97f, 0.92f), font, 17f, TextSecondary,
                TextAlignmentOptions.Left);
            slotMeta[i] = CreateText("Meta", slotRoots[i].transform,
                "LISTO PARA NUEVA SEMILLA", new Vector2(0.16f, 0.10f),
                new Vector2(0.97f, 0.43f), font, 13f, TextSecondary,
                TextAlignmentOptions.Left);
            Image track = CreateSolidImage("ProgressTrack", slotRoots[i].transform,
                new Vector2(0.16f, 0.06f), new Vector2(0.96f, 0.12f), Track);
            slotFills[i] = CreateFilledBar("Progress", track.transform, Cyan);
        }
        Button createSeed = CreateButton("CreateSeed", incubation.transform,
            new Vector2(0.655f, 0.43f), new Vector2(0.97f, 0.63f),
            selectorSprite, CyanDim, font, "CREAR SEMILLA", 24f,
            TextPrimary, out _);
        Button formAnchor = CreateButton("FormAnchor", incubation.transform,
            new Vector2(0.655f, 0.19f), new Vector2(0.97f, 0.39f),
            buttonSprite, PanelRaised, font, "FORMAR ANCLAJE", 21f,
            TextPrimary, out _);
        CreateSpriteImage("SeedCoreGlow", incubation.transform,
            new Vector2(0.735f, 0.66f), new Vector2(0.895f, 0.98f),
            glowSprite, Hex("00BFD0", 68), true);
        CreateSpriteImage("SeedCore", incubation.transform,
            new Vector2(0.765f, 0.69f), new Vector2(0.865f, 0.96f),
            nodeCoreSprite, Hex("D8FAFF", 232), true);
        CreateText("IncubationHint", incubation.transform,
            "LAS SEMILLAS MADURAS PASAN AUTOMÁTICAMENTE A LA RESERVA",
            new Vector2(0.65f, 0.03f), new Vector2(0.98f, 0.16f),
            font, 12f, TextSecondary, TextAlignmentOptions.Center);

        GameObject anchor = CreatePanel("AnchorChamber", shell.transform,
            new Vector2(0.025f, 0.352f), new Vector2(0.975f, 0.646f),
            panelSprite, Panel, false);
        AddSectionAccent(anchor.transform, Cyan);
        CreateSectionTitle(anchor.transform, "ANCLAJE INESTABLE", font,
            energyIconSprite, Cyan);
        GameObject latticeFrame = CreatePanel("LatticeFrame", anchor.transform,
            new Vector2(0.025f, 0.09f), new Vector2(0.535f, 0.82f),
            selectorSprite, Hex("031016"), false);
        AddChamberRails(latticeFrame.transform);
        CreateSpriteImage("CoreGlow", latticeFrame.transform,
            new Vector2(0.16f, 0.12f), new Vector2(0.84f, 0.88f),
            glowSprite, Hex("00BFD0", 54), true);
        CreateSpriteImage("AnchorCore", latticeFrame.transform,
            new Vector2(0.27f, 0.14f), new Vector2(0.73f, 0.88f),
            nodeCoreSprite, Hex("76B7C4", 78), true);
        GameObject latticeObject = CreateRect("ChronalLattice", latticeFrame.transform,
            new Vector2(0.08f, 0.12f), new Vector2(0.92f, 0.88f));
        ChronalLatticeGraphic lattice = latticeObject.AddComponent<ChronalLatticeGraphic>();
        lattice.raycastTarget = false;
        BuildStaticLattice(latticeObject.transform);
        TextMeshProUGUI anchorStateText = CreateText("AnchorState", latticeFrame.transform,
            "SIN ANCLAJE EN CÁMARA", new Vector2(0.08f, 0.02f),
            new Vector2(0.92f, 0.14f), font, 14f, TextSecondary,
            TextAlignmentOptions.Center);

        GaugeParts stability = BuildGauge(anchor.transform, "Stability", "ESTABILIDAD",
            new Vector2(0.56f, 0.43f), new Vector2(0.755f, 0.82f),
            font, ringSprite, glowSprite, Cyan);
        GaugeParts tension = BuildGauge(anchor.transform, "Tension", "TENSIÓN",
            new Vector2(0.775f, 0.43f), new Vector2(0.97f, 0.82f),
            font, ringSprite, glowSprite, Amber);
        TextMeshProUGUI thresholdText = CreateText("Threshold", anchor.transform,
            "UMBRAL DE FIJACIÓN  70%", new Vector2(0.56f, 0.31f),
            new Vector2(0.97f, 0.42f), font, 17f, TextSecondary,
            TextAlignmentOptions.Center);
        TextMeshProUGUI forecastText = CreateText("Forecast", anchor.transform,
            "FORMA UN ANCLAJE PARA INICIAR LA LECTURA",
            new Vector2(0.56f, 0.16f), new Vector2(0.97f, 0.30f),
            font, 16f, Cyan, TextAlignmentOptions.Center);
        CreateText("QualityLegend", anchor.transform,
            "PURO ≤15%   ·   ESTABLE ≤30%   ·   FORZADO >30%",
            new Vector2(0.56f, 0.04f), new Vector2(0.97f, 0.15f),
            font, 12f, TextSecondary, TextAlignmentOptions.Center);

        GameObject intensity = CreatePanel("Stabilization", shell.transform,
            new Vector2(0.025f, 0.246f), new Vector2(0.975f, 0.343f),
            panelSprite, Panel, false);
        AddSectionAccent(intensity.transform, Cyan);
        CreateSpriteImage("EnergyIcon", intensity.transform,
            new Vector2(0.022f, 0.66f), new Vector2(0.052f, 0.90f),
            energyIconSprite, Color.white, true);
        CreateText("Title", intensity.transform, "INTENSIDAD DE ESTABILIZACIÓN",
            new Vector2(0.065f, 0.70f), new Vector2(0.65f, 0.96f),
            font, 19f, TextPrimary, TextAlignmentOptions.Left);
        Slider slider = BuildSlider(intensity.transform, selectorSprite);
        TextMeshProUGUI intensityValue = CreateText("IntensityValue",
            intensity.transform, "50%", new Vector2(0.86f, 0.50f),
            new Vector2(0.97f, 0.94f), font, 28f, Cyan,
            TextAlignmentOptions.Center);
        TextMeshProUGUI stabilizationEffect = CreateText("Effect", intensity.transform,
            "+10% ESTABILIDAD  ·  +5% TENSIÓN", new Vector2(0.05f, 0.06f),
            new Vector2(0.62f, 0.35f), font, 15f, Cyan,
            TextAlignmentOptions.Left);
        TextMeshProUGUI compensateEffect = CreateText("CompensateEffect",
            intensity.transform, "COMPENSAR  −10% TENSIÓN  ·  −5% ESTABILIDAD",
            new Vector2(0.45f, 0.06f), new Vector2(0.96f, 0.35f),
            font, 14f, Amber, TextAlignmentOptions.Right);

        GameObject actions = CreatePanel("Actions", shell.transform,
            new Vector2(0.025f, 0.173f), new Vector2(0.975f, 0.237f),
            panelSprite, Hex("050C12"), false);
        Button stabilize = CreateButton("Stabilize", actions.transform,
            new Vector2(0.015f, 0.10f), new Vector2(0.252f, 0.90f),
            selectorSprite, CyanDim, font, "ESTABILIZAR", 19f,
            TextPrimary, out _);
        Button compensate = CreateButton("Compensate", actions.transform,
            new Vector2(0.262f, 0.10f), new Vector2(0.499f, 0.90f),
            buttonSprite, PanelRaised, font, "COMPENSAR", 18f,
            TextPrimary, out _);
        Button materialize = CreateButton("Materialize", actions.transform,
            new Vector2(0.509f, 0.10f), new Vector2(0.746f, 0.90f),
            selectorSprite, CyanDim, font, "FIJAR", 19f,
            TextPrimary, out _);
        Button discard = CreateButton("Discard", actions.transform,
            new Vector2(0.756f, 0.10f), new Vector2(0.985f, 0.90f),
            buttonSprite, AmberDim, font, "DESCARTAR", 18f,
            Amber, out _);

        GameObject archive = CreatePanel("Archive", shell.transform,
            new Vector2(0.025f, 0.050f), new Vector2(0.975f, 0.164f),
            panelSprite, Panel, false);
        AddSectionAccent(archive.transform, Hex("B74CF5"));
        CreateText("Title", archive.transform, "ARCHIVO DE ANCLAJES",
            new Vector2(0.05f, 0.70f), new Vector2(0.95f, 0.96f),
            font, 20f, TextPrimary, TextAlignmentOptions.Center);
        TextMeshProUGUI pureText = CreateArchiveCell(archive.transform, "Pure",
            "PUROS  0", 0.025f, 0.325f, font, selectorSprite,
            phaseIconSprite, Cyan, Cyan);
        TextMeshProUGUI stableText = CreateArchiveCell(archive.transform, "Stable",
            "ESTABLES  0", 0.345f, 0.655f, font, selectorSprite,
            archiveIconSprite, Color.white, Hex("55DF9A"));
        TextMeshProUGUI forcedText = CreateArchiveCell(archive.transform, "Forced",
            "FORZADOS  0", 0.675f, 0.975f, font, selectorSprite,
            phaseIconSprite, Amber, Amber);
        Image archiveTrack = CreateSolidImage("ArchiveTrack", archive.transform,
            new Vector2(0.035f, 0.06f), new Vector2(0.965f, 0.12f), Track);
        Image archiveFill = CreateFilledBar("ArchiveFill", archiveTrack.transform, Cyan);

        TextMeshProUGUI legacyStatus = CreateText("LegacyStatus", rootObject.transform,
            "", new Vector2(-2f, -2f), new Vector2(-1.9f, -1.9f),
            font, 1f, Color.clear, TextAlignmentOptions.Center);
        legacyStatus.gameObject.SetActive(false);

        MachineSeedsPanelVisualUI visual =
            rootObject.GetComponent<MachineSeedsPanelVisualUI>();
        if (visual == null)
            visual = rootObject.AddComponent<MachineSeedsPanelVisualUI>();
        SerializedObject visualSo = new SerializedObject(visual);
        SetObject(visualSo, "matureSeedsText", matureText);
        SetObject(visualSo, "archiveUsageText", archiveUsageText);
        SetObject(visualSo, "activeAnchorText", activeAnchorText);
        SetObjectArray(visualSo, "seedSlotRoots", slotRoots);
        SetObjectArray(visualSo, "seedSlotStateTexts", slotStates);
        SetObjectArray(visualSo, "seedSlotMetaTexts", slotMeta);
        SetObjectArray(visualSo, "seedSlotProgressFills", slotFills);
        SetObject(visualSo, "lattice", lattice);
        SetObject(visualSo, "stabilityGauge", stability.fill);
        SetObject(visualSo, "tensionGauge", tension.fill);
        SetObject(visualSo, "stabilityValueText", stability.value);
        SetObject(visualSo, "tensionValueText", tension.value);
        SetObject(visualSo, "anchorStateText", anchorStateText);
        SetObject(visualSo, "thresholdText", thresholdText);
        SetObject(visualSo, "forecastText", forecastText);
        SetObject(visualSo, "intensitySlider", slider);
        SetObject(visualSo, "intensityValueText", intensityValue);
        SetObject(visualSo, "stabilizationEffectText", stabilizationEffect);
        SetObject(visualSo, "compensateEffectText", compensateEffect);
        SetObject(visualSo, "createSeedButton", createSeed);
        SetObject(visualSo, "formAnchorButton", formAnchor);
        SetObject(visualSo, "stabilizeButton", stabilize);
        SetObject(visualSo, "compensateButton", compensate);
        SetObject(visualSo, "materializeButton", materialize);
        SetObject(visualSo, "discardButton", discard);
        SetObject(visualSo, "pureCountText", pureText);
        SetObject(visualSo, "stableCountText", stableText);
        SetObject(visualSo, "forcedCountText", forcedText);
        SetObject(visualSo, "archiveFill", archiveFill);
        visualSo.ApplyModifiedPropertiesWithoutUndo();

        SerializedObject machineSo = new SerializedObject(machine);
        SetObject(machineSo, "btnInstantChamberHelp", null);
        SetObject(machineSo, "instantSeedsViewRoot", rootObject);
        SetObject(machineSo, "btnBackToMachine", null);
        SetObject(machineSo, "seedsSlotsText", legacyStatus);
        SetObject(machineSo, "btnCreateSeed", createSeed);
        SetObject(machineSo, "btnFormInstant", formAnchor);
        SetObject(machineSo, "btnStabilizeInstant", stabilize);
        SetObject(machineSo, "stabilizationIntensitySlider", slider);
        SetObject(machineSo, "stabilizationPreviewText", stabilizationEffect);
        SetObject(machineSo, "btnRewindInstant", compensate);
        SetObject(machineSo, "btnMaterializeInstant", materialize);
        SetObject(machineSo, "btnDiscardInstant", discard);
        machineSo.ApplyModifiedPropertiesWithoutUndo();

        SetLayerRecursively(rootObject, 5);
        rootObject.SetActive(false);
        EditorUtility.SetDirty(machine);
        EditorUtility.SetDirty(visual);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Validate(machine, rootObject);
        Debug.Log("[Machine Seeds Visual] CONFIGURED | operational controls wired | 4 incubation slots");
    }

    public static void ConfigureBatch() => Configure();

    [MenuItem("Tools/Quantum Forge/Machine/Configure Seeds Context Tab Only")]
    public static void ConfigureSeedsContextTabOnly()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        MachinePanelUI machine = UnityEngine.Object.FindFirstObjectByType<MachinePanelUI>(
            FindObjectsInactive.Include);
        Require(machine != null, "No se encontró MachinePanelUI.");

        Transform tabs = machine.transform.Find(
            "MachineCubeVisualRoot/MachineContextTabs");
        Require(tabs != null, "No se encontró MachineContextTabs.");

        VerticalUiTheme theme = AssetDatabase.LoadAssetAtPath<VerticalUiTheme>(ThemePath);
        TMP_FontAsset font = theme != null && theme.primaryFont != null
            ? theme.primaryFont
            : AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        Sprite buttonSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ButtonFramePath);
        if (buttonSprite == null && theme != null)
            buttonSprite = theme.buttonFrame;
        Require(font != null && buttonSprite != null,
            "Faltan fuente o marco para la pestaña Semillas.");

        RectTransform tabsRect = tabs as RectTransform;
        Require(tabsRect != null, "MachineContextTabs no tiene RectTransform.");
        SetRect(tabsRect, new Vector2(0.18f, 0.840f),
            new Vector2(0.82f, 0.880f));

        Button nodes = tabs.Find("NodesTab")?.GetComponent<Button>();
        Button mixes = tabs.Find("MixesTab")?.GetComponent<Button>();
        Require(nodes != null && mixes != null,
            "Faltan las pestañas NODOS o MEZCLAS.");
        SetRect(nodes.transform as RectTransform, new Vector2(0f, 0.04f),
            new Vector2(0.32f, 0.96f));
        SetRect(mixes.transform as RectTransform, new Vector2(0.34f, 0.04f),
            new Vector2(0.66f, 0.96f));

        Transform oldSeeds = tabs.Find("SeedsTab");
        if (oldSeeds != null)
            UnityEngine.Object.DestroyImmediate(oldSeeds.gameObject);
        Button seeds = CreateButton("SeedsTab", tabs,
            new Vector2(0.68f, 0.04f), new Vector2(1f, 0.96f),
            buttonSprite, new Color(0f, 0.40f, 0.36f, 0.92f), font,
            "SEMILLAS", 20f, TextPrimary, out _);
        seeds.gameObject.SetActive(false);

        SerializedObject machineSo = new SerializedObject(machine);
        SetObject(machineSo, "btnSeedsTab", seeds);
        machineSo.ApplyModifiedPropertiesWithoutUndo();

        EditorUtility.SetDirty(machine);
        EditorUtility.SetDirty(tabs.gameObject);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();

        SerializedObject validationSo = new SerializedObject(machine);
        SerializedProperty seedsProperty = validationSo.FindProperty("btnSeedsTab");
        Require(seedsProperty != null && seedsProperty.objectReferenceValue == seeds,
            "La pestaña SEMILLAS no quedó conectada a MachinePanelUI.");
        Require(!seeds.gameObject.activeSelf,
            "SEMILLAS debe permanecer oculta antes de evaluar el desbloqueo.");
        Debug.Log("[Machine Seeds Tab] CONFIGURED | hidden until cube unlock | " +
            "NODOS + MEZCLAS + SEMILLAS");
    }

    public static void ConfigureSeedsContextTabOnlyBatch()
    {
        try
        {
            ConfigureSeedsContextTabOnly();
            Debug.Log("[Machine Seeds Tab] PASS");
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    private static void CreateSectionTitle(Transform parent, string value,
        TMP_FontAsset font, Sprite icon, Color accent)
    {
        CreateSpriteImage("SectionIcon", parent,
            new Vector2(0.038f, 0.855f), new Vector2(0.072f, 0.955f),
            icon, accent, true);
        TextMeshProUGUI title = CreateText("Title", parent, value,
            new Vector2(0.082f, 0.82f), new Vector2(0.96f, 0.98f),
            font, 22f, TextPrimary, TextAlignmentOptions.Left);
        title.fontStyle = FontStyles.Bold;
        title.characterSpacing = 1.2f;
        CreateSolidImage("TitleUnderline", parent,
            new Vector2(0.082f, 0.812f), new Vector2(0.94f, 0.823f),
            Hex("0B6680", 176));
        CreateSolidImage("TitleCap", parent,
            new Vector2(0.94f, 0.800f), new Vector2(0.952f, 0.835f), accent);
    }

    private static void AddOuterRails(Transform parent)
    {
        CreateSolidImage("OuterRailLeft", parent,
            new Vector2(0.008f, 0.065f), new Vector2(0.013f, 0.915f),
            Hex("00D5D0", 142));
        CreateSolidImage("OuterRailRight", parent,
            new Vector2(0.987f, 0.065f), new Vector2(0.992f, 0.915f),
            Hex("0A6075", 122));
        CreateSolidImage("OuterCapTop", parent,
            new Vector2(0.008f, 0.915f), new Vector2(0.040f, 0.921f), Cyan);
        CreateSolidImage("OuterCapBottom", parent,
            new Vector2(0.008f, 0.059f), new Vector2(0.040f, 0.065f), CyanDim);
    }

    private static void AddHeaderRail(Transform parent, float minX, float maxX)
    {
        CreateSolidImage("HeaderRail", parent,
            new Vector2(minX, 0.970f), new Vector2(maxX, 0.973f),
            Hex("00D5D0", 190));
        CreateSolidImage("HeaderCap", parent,
            new Vector2(maxX, 0.962f), new Vector2(maxX + 0.008f, 0.981f),
            Cyan);
    }

    private static void AddSectionAccent(Transform parent, Color accent)
    {
        CreateSolidImage("SectionRail", parent,
            new Vector2(0.008f, 0.12f), new Vector2(0.014f, 0.88f), accent);
        CreateSolidImage("SectionRailTop", parent,
            new Vector2(0.008f, 0.88f), new Vector2(0.040f, 0.895f), accent);
    }

    private static void AddDivider(Transform parent, float x)
    {
        CreateSolidImage("Divider", parent, new Vector2(x, 0.14f),
            new Vector2(x + 0.002f, 0.86f), CyanDim);
    }

    private static void AddChamberRails(Transform parent)
    {
        CreateSolidImage("RailTop", parent, new Vector2(0.10f, 0.86f),
            new Vector2(0.90f, 0.89f), CyanDim);
        CreateSolidImage("RailBottom", parent, new Vector2(0.10f, 0.11f),
            new Vector2(0.90f, 0.14f), CyanDim);
        CreateSolidImage("RailLeft", parent, new Vector2(0.07f, 0.18f),
            new Vector2(0.08f, 0.82f), CyanDim);
        CreateSolidImage("RailRight", parent, new Vector2(0.92f, 0.18f),
            new Vector2(0.93f, 0.82f), CyanDim);
    }

    private static void BuildStaticLattice(Transform parent)
    {
        Vector2[] points =
        {
            new(0.12f, 0.23f), new(0.24f, 0.68f), new(0.38f, 0.36f),
            new(0.47f, 0.78f), new(0.58f, 0.20f), new(0.69f, 0.58f),
            new(0.84f, 0.32f), new(0.79f, 0.82f), new(0.42f, 0.10f),
            new(0.15f, 0.49f), new(0.55f, 0.50f), new(0.88f, 0.63f)
        };
        Vector2Int[] edges =
        {
            new(0, 2), new(0, 9), new(1, 2), new(1, 3), new(1, 9),
            new(2, 3), new(2, 4), new(2, 10), new(3, 5), new(3, 7),
            new(3, 10), new(4, 6), new(4, 8), new(4, 10), new(5, 6),
            new(5, 7), new(5, 10), new(5, 11), new(6, 11), new(7, 11)
        };
        for (int i = 0; i < edges.Length; i++)
            CreateLatticeLine("Link" + i, parent, points[edges[i].x],
                points[edges[i].y], i % 3 == 0 ? 2.2f : 1.2f);
        for (int i = 0; i < points.Length; i++)
        {
            GameObject node = CreateRect("Node" + i, parent, points[i], points[i]);
            RectTransform rect = node.GetComponent<RectTransform>();
            float size = i % 4 == 0 ? 9f : 6f;
            rect.sizeDelta = new Vector2(size, size);
            rect.localRotation = Quaternion.Euler(0f, 0f, 45f);
            Image image = node.AddComponent<Image>();
            image.color = i % 4 == 0 ? Cyan : Hex("53C7B1", 210);
            image.raycastTarget = false;
        }
    }

    private static void CreateLatticeLine(string name, Transform parent,
        Vector2 start, Vector2 end, float thickness)
    {
        Vector2 delta = new Vector2((end.x - start.x) * 360f,
            (end.y - start.y) * 190f);
        Vector2 midpoint = (start + end) * 0.5f;
        GameObject line = CreateRect(name, parent, midpoint, midpoint);
        RectTransform rect = line.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(delta.magnitude, thickness);
        rect.localRotation = Quaternion.Euler(0f, 0f,
            Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
        Image image = line.AddComponent<Image>();
        image.color = Hex("32D6BA", 170);
        image.raycastTarget = false;
    }

    private static GaugeParts BuildGauge(Transform parent, string name,
        string label, Vector2 min, Vector2 max, TMP_FontAsset font,
        Sprite ringSprite, Sprite glowSprite, Color accent)
    {
        GameObject root = CreateRect(name, parent, min, max);
        CreateSpriteImage("Glow", root.transform,
            new Vector2(-0.18f, -0.18f), new Vector2(1.18f, 1.18f),
            glowSprite, new Color(accent.r, accent.g, accent.b, 0.19f), true);
        Image track = root.AddComponent<Image>();
        track.sprite = ringSprite;
        track.preserveAspect = true;
        track.color = Track;
        track.raycastTarget = false;
        GameObject fillObject = CreateRect("Fill", root.transform,
            new Vector2(0.08f, 0.08f), new Vector2(0.92f, 0.92f));
        Image fill = fillObject.AddComponent<Image>();
        fill.sprite = ringSprite;
        fill.preserveAspect = true;
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Radial360;
        fill.fillOrigin = 2;
        fill.fillClockwise = true;
        fill.fillAmount = 0f;
        fill.color = accent;
        fill.raycastTarget = false;
        CreateText("Label", root.transform, label, new Vector2(0.10f, 0.53f),
            new Vector2(0.90f, 0.70f), font, 13f, accent,
            TextAlignmentOptions.Center);
        TextMeshProUGUI value = CreateText("Value", root.transform, "0%",
            new Vector2(0.10f, 0.25f), new Vector2(0.90f, 0.55f),
            font, 30f, accent, TextAlignmentOptions.Center);
        return new GaugeParts { fill = fill, value = value };
    }

    private static Slider BuildSlider(Transform parent, Sprite handleSprite)
    {
        GameObject root = CreateRect("IntensitySlider", parent,
            new Vector2(0.05f, 0.36f), new Vector2(0.83f, 0.62f));
        Slider slider = root.AddComponent<Slider>();
        slider.minValue = 10f;
        slider.maxValue = 100f;
        slider.value = 50f;
        slider.wholeNumbers = true;
        slider.direction = Slider.Direction.LeftToRight;
        Image track = CreateSolidImage("Track", root.transform,
            new Vector2(0f, 0.38f), new Vector2(1f, 0.62f), Track);
        GameObject fillArea = CreateRect("FillArea", root.transform,
            new Vector2(0.01f, 0.38f), new Vector2(0.99f, 0.62f));
        Image fill = CreateSolidImage("Fill", fillArea.transform,
            Vector2.zero, Vector2.one, Cyan);
        GameObject handleArea = CreateRect("HandleArea", root.transform,
            new Vector2(0.01f, 0f), new Vector2(0.99f, 1f));
        GameObject handleObject = CreateRect("Handle", handleArea.transform,
            new Vector2(0f, 0.05f), new Vector2(0.055f, 0.95f));
        Image handle = handleObject.AddComponent<Image>();
        handle.sprite = handleSprite;
        handle.type = Image.Type.Sliced;
        handle.color = Cyan;
        slider.targetGraphic = handle;
        slider.fillRect = fill.rectTransform;
        slider.handleRect = handle.rectTransform;
        return slider;
    }

    private static TextMeshProUGUI CreateArchiveCell(Transform parent,
        string name, string label, float minX, float maxX, TMP_FontAsset font,
        Sprite sprite, Sprite icon, Color iconColor, Color accent)
    {
        GameObject cell = CreatePanel(name, parent, new Vector2(minX, 0.19f),
            new Vector2(maxX, 0.66f), sprite, PanelRaised, false);
        CreateSolidImage("Accent", cell.transform,
            new Vector2(0.010f, 0.16f), new Vector2(0.022f, 0.84f), accent);
        CreateSpriteImage("Icon", cell.transform,
            new Vector2(0.055f, 0.18f), new Vector2(0.245f, 0.82f),
            icon, iconColor, true);
        return CreateText("Value", cell.transform, label,
            new Vector2(0.22f, 0.10f), new Vector2(0.95f, 0.90f),
            font, 17f, TextPrimary, TextAlignmentOptions.Center);
    }

    private static Image CreateFilledBar(string name, Transform parent, Color color)
    {
        GameObject root = CreateRect(name, parent, Vector2.zero, Vector2.one);
        Image image = root.AddComponent<Image>();
        image.color = color;
        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Horizontal;
        image.fillOrigin = 0;
        image.fillAmount = 0f;
        image.raycastTarget = false;
        return image;
    }

    private static GameObject CreatePanel(string name, Transform parent,
        Vector2 min, Vector2 max, Sprite sprite, Color color, bool raycast)
    {
        GameObject root = CreateRect(name, parent, min, max);
        Image image = root.AddComponent<Image>();
        image.sprite = sprite;
        image.type = Image.Type.Sliced;
        image.color = color;
        image.raycastTarget = raycast;
        return root;
    }

    private static Button CreateButton(string name, Transform parent,
        Vector2 min, Vector2 max, Sprite sprite, Color color, TMP_FontAsset font,
        string label, float fontSize, Color textColor,
        out TextMeshProUGUI labelText)
    {
        GameObject root = CreatePanel(name, parent, min, max, sprite, color, true);
        Image image = root.GetComponent<Image>();
        Button button = root.AddComponent<Button>();
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.12f, 1.12f, 1.12f, 1f);
        colors.pressedColor = new Color(0.72f, 0.78f, 0.80f, 1f);
        colors.disabledColor = new Color(0.38f, 0.40f, 0.42f, 0.62f);
        button.colors = colors;
        labelText = CreateText("Label", root.transform, label,
            new Vector2(0.05f, 0.08f), new Vector2(0.95f, 0.92f),
            font, fontSize, textColor, TextAlignmentOptions.Center);
        labelText.fontStyle = FontStyles.Bold;
        return button;
    }

    private static TextMeshProUGUI CreateText(string name, Transform parent,
        string value, Vector2 min, Vector2 max, TMP_FontAsset font, float size,
        Color color, TextAlignmentOptions alignment)
    {
        GameObject root = CreateRect(name, parent, min, max);
        TextMeshProUGUI text = root.AddComponent<TextMeshProUGUI>();
        text.font = font;
        text.text = value;
        text.fontSize = size;
        text.fontSizeMax = size;
        text.fontSizeMin = Mathf.Max(9f, size * 0.67f);
        text.enableAutoSizing = true;
        text.color = color;
        text.alignment = alignment;
        text.raycastTarget = false;
        text.overflowMode = TextOverflowModes.Ellipsis;
        return text;
    }

    private static Image CreateSolidImage(string name, Transform parent,
        Vector2 min, Vector2 max, Color color)
    {
        GameObject root = CreateRect(name, parent, min, max);
        Image image = root.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static Image CreateSpriteImage(string name, Transform parent,
        Vector2 min, Vector2 max, Sprite sprite, Color color, bool preserveAspect)
    {
        GameObject root = CreateRect(name, parent, min, max);
        Image image = root.AddComponent<Image>();
        image.sprite = sprite;
        image.color = color;
        image.preserveAspect = preserveAspect;
        image.raycastTarget = false;
        return image;
    }

    private static GameObject CreateRect(string name, Transform parent,
        Vector2 min, Vector2 max)
    {
        GameObject root = new GameObject(name, typeof(RectTransform));
        root.layer = 5;
        RectTransform rect = root.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        SetRect(rect, min, max);
        return root;
    }

    private static void SetRect(RectTransform rect, Vector2 min, Vector2 max)
    {
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
    }

    private static void SetObject(SerializedObject so, string name,
        UnityEngine.Object value)
    {
        SerializedProperty property = so.FindProperty(name);
        Require(property != null, "No existe el campo serializado: " + name);
        property.objectReferenceValue = value;
    }

    private static void SetObjectArray(SerializedObject so, string name,
        UnityEngine.Object[] values)
    {
        SerializedProperty property = so.FindProperty(name);
        Require(property != null, "No existe el arreglo serializado: " + name);
        property.arraySize = values != null ? values.Length : 0;
        for (int i = 0; i < property.arraySize; i++)
            property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
    }

    private static void SetLayerRecursively(GameObject root, int layer)
    {
        root.layer = layer;
        foreach (Transform child in root.transform)
            SetLayerRecursively(child.gameObject, layer);
    }

    private static void Validate(MachinePanelUI machine, GameObject root)
    {
        RectTransform shell = root.transform.Find("SeedsVisualShell") as RectTransform;
        Require(shell != null && Mathf.Abs(shell.offsetMin.x - 96f) < 0.1f &&
            Mathf.Abs(shell.offsetMax.x + 96f) < 0.1f,
            "Semillas debe conservar el mismo margen lateral de 96 px del Triángulo.");
        Require(root.transform.Find("SeedsVisualShell/Incubation/CreateSeed") != null,
            "Falta el control Crear Semilla.");
        Require(root.transform.Find("SeedsVisualShell/AnchorChamber/ChronalLattice") == null,
            "El lattice debe vivir dentro de LatticeFrame.");
        Require(root.transform.Find(
            "SeedsVisualShell/AnchorChamber/LatticeFrame/ChronalLattice") != null,
            "Falta la cámara visual del anclaje.");
        Require(root.GetComponent<MachineSeedsPanelVisualUI>() != null,
            "Falta MachineSeedsPanelVisualUI.");
        SerializedObject so = new SerializedObject(machine);
        Require(so.FindProperty("instantSeedsViewRoot").objectReferenceValue == root,
            "MachinePanelUI no apunta a la vista de semillas.");
    }

    private static Color Hex(string value, byte alpha = 255)
    {
        ColorUtility.TryParseHtmlString("#" + value, out Color color);
        color.a = alpha / 255f;
        return color;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }

    private sealed class GaugeParts
    {
        public Image fill;
        public TextMeshProUGUI value;
    }
}
#endif

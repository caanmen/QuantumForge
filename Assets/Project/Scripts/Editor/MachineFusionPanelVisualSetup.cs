#if UNITY_EDITOR
using System;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class MachineFusionPanelVisualSetup
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
    private static readonly string[] FragmentIconPaths =
    {
        "Assets/Project/UI/Vertical/GenerationPolish/qf_node_higgs.png",
        "Assets/Project/UI/Vertical/GenerationPolish/qf_node_tetraquark.png",
        "Assets/Project/UI/Vertical/GenerationPolish/qf_node_modulator.png"
    };
    private static readonly string[] CatalystIconPaths =
    {
        "Assets/Project/UI/Vertical/GenerationPolish/qf_catalyst_alpha_option3.png",
        "Assets/Project/UI/Vertical/GenerationPolish/qf_circuit_experimental.png"
    };

    private static readonly Color Background = Hex("02070D", 250);
    private static readonly Color Panel = Hex("071017", 246);
    private static readonly Color PanelRaised = Hex("0A1118", 250);
    private static readonly Color TextPrimary = Hex("E8EAEC");
    private static readonly Color TextSecondary = Hex("8A949D");
    private static readonly Color Cyan = Hex("00C9FF");
    private static readonly Color Violet = Hex("B55CFF");
    private static readonly Color VioletDim = Hex("6C3A83");
    private static readonly Color Amber = Hex("F0A018");

    [MenuItem("Tools/Quantum Forge/Machine/Configure Fusion Panel Visual")]
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
        Require(font != null, "Falta la fuente principal de la UI vertical.");

        ConfigureSpriteImport(CatalystIconPaths[0]);

        Sprite panelSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ModuleFramePath);
        if (panelSprite == null && theme != null)
            panelSprite = theme.panelFrame;
        Sprite buttonSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ButtonFramePath);
        if (buttonSprite == null && theme != null)
            buttonSprite = theme.buttonFrame;
        Sprite selectorSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SelectorFramePath);
        if (selectorSprite == null)
            selectorSprite = buttonSprite;
        Sprite selectedSprite = theme != null ? theme.selectedButtonFrame : buttonSprite;
        Sprite ringSprite = AssetDatabase.LoadAssetAtPath<Sprite>(RingPath);
        Require(panelSprite != null && buttonSprite != null && ringSprite != null,
            "Faltan marcos compartidos para el panel de mezclas.");

        Transform oldRoot = room.transform.Find("LegacyFusionPanel");
        GameObject rootObject;
        if (oldRoot != null)
        {
            rootObject = oldRoot.gameObject;
            for (int i = oldRoot.childCount - 1; i >= 0; i--)
                UnityEngine.Object.DestroyImmediate(oldRoot.GetChild(i).gameObject);
        }
        else
        {
            rootObject = CreateRect("LegacyFusionPanel", room.transform,
                Vector2.zero, Vector2.one);
        }

        RectTransform rootRect = rootObject.GetComponent<RectTransform>();
        SetRect(rootRect, Vector2.zero, Vector2.one);
        rootObject.layer = 5;

        GameObject shell = CreatePanel("FusionVisualShell", rootObject.transform,
            new Vector2(0.022f, 0.030f), new Vector2(0.978f, 0.838f),
            panelSprite, Background, true);

        BuildHeading(shell.transform, font);

        GameObject statusPanel = CreatePanel("FusionStatusStrip", shell.transform,
            new Vector2(0.025f, 0.875f), new Vector2(0.975f, 0.918f),
            selectorSprite, new Color(0.34f, 0.23f, 0.42f, 0.72f), false);
        TextMeshProUGUI fusionSlotsText = CreateText("FusionStatus",
            statusPanel.transform,
            "RANURAS 2     •     FUSIÓN LISTA     •     NÚCLEO 4/10",
            new Vector2(0.025f, 0.08f), new Vector2(0.975f, 0.92f),
            font, 23f, TextPrimary, TextAlignmentOptions.Center);

        Sprite[] fragmentIcons = new Sprite[FragmentIconPaths.Length];
        for (int i = 0; i < fragmentIcons.Length; i++)
            fragmentIcons[i] = AssetDatabase.LoadAssetAtPath<Sprite>(FragmentIconPaths[i]);
        Sprite[] catalystIcons = new Sprite[CatalystIconPaths.Length];
        for (int i = 0; i < catalystIcons.Length; i++)
            catalystIcons[i] = AssetDatabase.LoadAssetAtPath<Sprite>(CatalystIconPaths[i]);

        SlotParts slotA = BuildSlot(shell.transform, "FragmentA", "FRAGMENTO A",
            "CONDENSACIÓN (8)", 0.025f, 0.325f, font, panelSprite,
            selectorSprite, fragmentIcons[0]);
        SlotParts slotB = BuildSlot(shell.transform, "FragmentB", "FRAGMENTO B",
            "CONFINAMIENTO (5)", 0.345f, 0.655f, font, panelSprite,
            selectorSprite, fragmentIcons[1]);
        SlotParts catalyst = BuildSlot(shell.transform, "Catalyst", "CATALIZADOR",
            "ALPHA", 0.675f, 0.975f, font, panelSprite,
            selectorSprite, catalystIcons[0]);

        ReactorParts reactor = BuildReactor(shell.transform, font, panelSprite,
            ringSprite);

        Button modeButton = CreateButton("ModeButton", shell.transform,
            new Vector2(0.025f, 0.375f), new Vector2(0.492f, 0.425f),
            selectorSprite, PanelRaised, font, "MODO: EQUILIBRADO", 22f,
            Violet, out TextMeshProUGUI modeButtonText);
        AddChevron(modeButton.transform, font);

        Button guidedButton = CreateButton("GuidedIntentButton", shell.transform,
            new Vector2(0.508f, 0.375f), new Vector2(0.975f, 0.425f),
            selectorSprite, PanelRaised, font, "INTENCIÓN: HALLAZGO", 22f,
            Violet, out TextMeshProUGUI guidedButtonText);
        AddChevron(guidedButton.transform, font);

        GameObject readingPanel = CreatePanel("CompositionReading", shell.transform,
            new Vector2(0.025f, 0.225f), new Vector2(0.492f, 0.365f),
            panelSprite, Panel, false);
        CreateText("Heading", readingPanel.transform, "LECTURA DE COMPOSICIÓN",
            new Vector2(0.07f, 0.72f), new Vector2(0.93f, 0.95f), font, 22f,
            Violet, TextAlignmentOptions.Center);
        TextMeshProUGUI compositionText = CreateText("Reading", readingPanel.transform,
            "Lectura de composición: activa\nResultado probable: ???\nRiesgo estimado: 0%",
            new Vector2(0.08f, 0.16f), new Vector2(0.92f, 0.72f), font, 19f,
            TextPrimary, TextAlignmentOptions.Left);
        compositionText.lineSpacing = 4f;
        Image[] riskSegments = BuildSegmentBar(readingPanel.transform, "RiskSegments", 12, 0,
            new Vector2(0.10f, 0.06f), new Vector2(0.90f, 0.14f), Amber);

        GameObject instabilityPanel = CreatePanel("Instability", shell.transform,
            new Vector2(0.508f, 0.225f), new Vector2(0.975f, 0.365f),
            panelSprite, Panel, false);
        TextMeshProUGUI instabilityText = CreateText("InstabilityText",
            instabilityPanel.transform, "INESTABILIDAD: 0\nESTADO: BAJA",
            new Vector2(0.08f, 0.58f), new Vector2(0.92f, 0.94f), font, 21f,
            Violet, TextAlignmentOptions.Center);
        Image[] instabilitySegments = BuildSegmentBar(instabilityPanel.transform, "InstabilitySegments", 12, 0,
            new Vector2(0.10f, 0.45f), new Vector2(0.90f, 0.56f), Violet);
        Button coolButton = CreateButton("CoolButton", instabilityPanel.transform,
            new Vector2(0.08f, 0.08f), new Vector2(0.92f, 0.38f),
            selectedSprite, VioletDim, font, "ENFRIAR · 30 TRAZAS", 20f,
            TextPrimary, out TextMeshProUGUI coolButtonText);

        Button mixButton = CreateButton("FusionButton", shell.transform,
            new Vector2(0.025f, 0.135f), new Vector2(0.592f, 0.215f),
            selectorSprite, Violet, font, "FUSIONAR", 34f,
            TextPrimary, out TextMeshProUGUI mixButtonText);
        Button logButton = CreateButton("LogButton", shell.transform,
            new Vector2(0.608f, 0.135f), new Vector2(0.975f, 0.215f),
            buttonSprite, PanelRaised, font, "REGISTRO", 25f,
            TextPrimary, out TextMeshProUGUI logButtonText);

        GameObject logBadge = CreatePanel("LogBadge", logButton.transform,
            new Vector2(0.88f, 0.68f), new Vector2(0.98f, 0.96f),
            selectorSprite, Violet, false);
        TextMeshProUGUI logBadgeText = CreateText("Count", logBadge.transform, "1",
            new Vector2(0.06f, 0.06f), new Vector2(0.94f, 0.94f), font, 18f,
            TextPrimary, TextAlignmentOptions.Center);
        logBadge.SetActive(false);

        GameObject resultPanel = CreatePanel("ResultConsole", shell.transform,
            new Vector2(0.025f, 0.050f), new Vector2(0.975f, 0.125f),
            panelSprite, Panel, false);
        GameObject resultIconObject = CreateRect("ResultIcon", resultPanel.transform,
            new Vector2(0.025f, 0.10f), new Vector2(0.145f, 0.90f));
        Image resultIcon = resultIconObject.AddComponent<Image>();
        resultIcon.sprite = ringSprite;
        resultIcon.preserveAspect = true;
        resultIcon.color = Violet;
        resultIcon.raycastTarget = false;
        TextMeshProUGUI resultGlyph = CreateText("Glyph", resultIconObject.transform, "R",
            new Vector2(0.18f, 0.18f), new Vector2(0.82f, 0.82f), font, 32f,
            TextPrimary, TextAlignmentOptions.Center);
        TextMeshProUGUI resultTitle = CreateText("ResultTitle", resultPanel.transform,
            "RESULTADO DEL ENSAYO: —", new Vector2(0.16f, 0.62f),
            new Vector2(0.96f, 0.92f), font, 23f, TextPrimary,
            TextAlignmentOptions.Left);
        resultTitle.fontStyle = FontStyles.Bold;
        TextMeshProUGUI resultDetail = CreateText("ResultDetail", resultPanel.transform,
            "Selecciona componentes y ejecuta un ensayo", new Vector2(0.16f, 0.31f),
            new Vector2(0.96f, 0.62f), font, 19f, TextSecondary,
            TextAlignmentOptions.Left);
        TextMeshProUGUI resultMeta = CreateText("ResultMeta", resultPanel.transform,
            "RIESGO Y RECOMPENSA SE ACTUALIZAN EN TIEMPO REAL",
            new Vector2(0.16f, 0.06f), new Vector2(0.96f, 0.31f), font, 16f,
            Violet, TextAlignmentOptions.Left);

        GameObject diagnosticPanel = CreatePanel("CoreDiagnostic", shell.transform,
            new Vector2(0.025f, 0.005f), new Vector2(0.975f, 0.045f),
            selectorSprite, PanelRaised, false);
        TextMeshProUGUI diagnosticText = CreateText("DiagnosticText",
            diagnosticPanel.transform, "DIAGNÓSTICO DEL NÚCLEO",
            new Vector2(0.02f, 0.05f), new Vector2(0.64f, 0.95f), font, 14f,
            TextSecondary, TextAlignmentOptions.Left);
        Image[] diagnosticBars = BuildDiagnosticBars(diagnosticPanel.transform,
            new Vector2(0.66f, 0.10f), new Vector2(0.98f, 0.90f), 18);

        LogParts log = BuildLogOverlay(rootObject.transform, font, panelSprite,
            buttonSprite);

        MachineFusionPanelVisualUI visual = rootObject.GetComponent<MachineFusionPanelVisualUI>();
        if (visual == null)
            visual = rootObject.AddComponent<MachineFusionPanelVisualUI>();
        SerializedObject visualSo = new SerializedObject(visual);
        SetObject(visualSo, "roomPanel", room);
        SetObject(visualSo, "fragmentAOutline", slotA.outline);
        SetObject(visualSo, "fragmentBOutline", slotB.outline);
        SetObject(visualSo, "catalystOutline", catalyst.outline);
        SetObjectArray(visualSo, "riskSegments", riskSegments);
        SetObjectArray(visualSo, "instabilitySegments", instabilitySegments);
        SetObject(visualSo, "cyanStream", reactor.cyan);
        SetObject(visualSo, "violetStream", reactor.violet);
        SetObject(visualSo, "coreGlow", reactor.core);
        SetObject(visualSo, "coreTransform", reactor.core.rectTransform);
        SetObjectArray(visualSo, "reactorParticles", reactor.particles);
        SetObject(visualSo, "energyWave", reactor.energyWave);
        SetObject(visualSo, "coolButton", coolButton);
        SetObject(visualSo, "coolButtonText", coolButtonText);
        SetObject(visualSo, "mixButton", mixButton);
        SetObject(visualSo, "mixButtonText", mixButtonText);
        SetObject(visualSo, "logBadgeRoot", logBadge);
        SetObject(visualSo, "logBadgeText", logBadgeText);
        SetObject(visualSo, "resultIcon", resultIcon);
        SetObject(visualSo, "resultGlyph", resultGlyph);
        SetObject(visualSo, "resultTitle", resultTitle);
        SetObject(visualSo, "resultDetail", resultDetail);
        SetObject(visualSo, "resultMeta", resultMeta);
        SetObject(visualSo, "diagnosticText", diagnosticText);
        SetObjectArray(visualSo, "diagnosticBars", diagnosticBars);
        visualSo.ApplyModifiedPropertiesWithoutUndo();

        SerializedObject roomSo = new SerializedObject(room);
        SetObject(roomSo, "closedBlock", null);
        SetObject(roomSo, "openedBlock", null);
        SetObject(roomSo, "mixButton", mixButton);
        SetObject(roomSo, "logButton", logButton);
        SetObject(roomSo, "logPanel", log.root);
        SetObject(roomSo, "logContentText", log.content);
        SetObject(roomSo, "logCloseButton", log.close);
        SetObject(roomSo, "statusText", resultTitle);
        SetObject(roomSo, "titleText", null);
        SetObject(roomSo, "introText", null);
        SetObject(roomSo, "fusionSlotsText", fusionSlotsText);
        SetObject(roomSo, "compositionReadingText", compositionText);
        SetObject(roomSo, "fragmentSlotAButton", slotA.button);
        SetObject(roomSo, "fragmentSlotBButton", slotB.button);
        SetObject(roomSo, "catalystSlotButton", catalyst.button);
        SetObject(roomSo, "fragmentSlotAText", slotA.value);
        SetObject(roomSo, "fragmentSlotBText", slotB.value);
        SetObject(roomSo, "catalystSlotText", catalyst.value);
        SetObject(roomSo, "fragmentSlotAIcon", slotA.icon);
        SetObject(roomSo, "fragmentSlotBIcon", slotB.icon);
        SetObject(roomSo, "catalystSlotIcon", catalyst.icon);
        SetObject(roomSo, "fragmentCondensationIcon", fragmentIcons[0]);
        SetObject(roomSo, "fragmentConfinementIcon", fragmentIcons[1]);
        SetObject(roomSo, "fragmentResidualIcon", fragmentIcons[2]);
        SetObject(roomSo, "catalystAlphaIcon", catalystIcons[0]);
        SetObject(roomSo, "catalystBetaIcon", catalystIcons[1]);
        SetObject(roomSo, "modeText", null);
        SetObject(roomSo, "modeButton", modeButton);
        SetObject(roomSo, "modeButtonText", modeButtonText);
        SetObject(roomSo, "guidedIntentText", null);
        SetObject(roomSo, "guidedIntentButton", guidedButton);
        SetObject(roomSo, "guidedIntentButtonText", guidedButtonText);
        SetObject(roomSo, "instabilityText", instabilityText);
        SetObject(roomSo, "coolButton", coolButton);
        SetObject(roomSo, "coolButtonText", coolButtonText);
        SetObject(roomSo, "logButtonText", logButtonText);
        roomSo.ApplyModifiedPropertiesWithoutUndo();

        SerializedObject machineSo = new SerializedObject(machine);
        SetObject(machineSo, "legacyFusionPanel", rootObject);
        SetObject(machineSo, "btnBackToNodesFromFusion", null);
        machineSo.ApplyModifiedPropertiesWithoutUndo();

        SetLayerRecursively(rootObject, 5);
        rootObject.SetActive(false);
        EditorUtility.SetDirty(room);
        EditorUtility.SetDirty(machine);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        ValidateBuiltPanel(room, machine, rootObject);
        Debug.Log("[Machine Fusion Visual] CONFIGURED | tabs preserved | functional controls wired");
    }

    public static void ConfigureBatch()
    {
        Configure();
    }

    private static void BuildHeading(Transform parent, TMP_FontAsset font)
    {
        TextMeshProUGUI title = CreateText("FusionTitle", parent, "PANEL DE FUSIÓN",
            new Vector2(0.10f, 0.952f), new Vector2(0.90f, 0.995f), font, 32f,
            TextPrimary, TextAlignmentOptions.Center);
        title.fontStyle = FontStyles.Bold;
        title.characterSpacing = 2f;
        CreateText("FusionSubtitle", parent, "CARA 2 / 4 · SISTEMA OPERATIVO",
            new Vector2(0.10f, 0.925f), new Vector2(0.90f, 0.958f), font, 20f,
            Violet, TextAlignmentOptions.Center);
    }

    private static SlotParts BuildSlot(Transform parent, string name, string title,
        string value, float minX, float maxX, TMP_FontAsset font,
        Sprite panelSprite, Sprite selectorSprite, Sprite iconSprite)
    {
        GameObject card = CreatePanel(name, parent,
            new Vector2(minX, 0.545f), new Vector2(maxX, 0.885f), panelSprite,
            Panel, false);
        Outline outline = card.AddComponent<Outline>();
        outline.effectColor = new Color(Violet.r, Violet.g, Violet.b, 0f);
        outline.effectDistance = new Vector2(2f, -2f);
        outline.useGraphicAlpha = true;
        outline.enabled = false;
        CreateText("Title", card.transform, title, new Vector2(0.05f, 0.82f),
            new Vector2(0.95f, 0.97f), font, 22f, TextPrimary,
            TextAlignmentOptions.Center);

        Image icon = null;
        if (iconSprite != null)
        {
            GameObject iconObject = CreateRect("Glyph", card.transform,
                new Vector2(0.23f, 0.28f), new Vector2(0.77f, 0.78f));
            icon = iconObject.AddComponent<Image>();
            icon.sprite = iconSprite;
            icon.preserveAspect = true;
            icon.color = Color.white;
            icon.raycastTarget = false;
        }
        else
        {
            CreateText("Glyph", card.transform, "◇", new Vector2(0.20f, 0.28f),
                new Vector2(0.80f, 0.78f), font, 54f, Violet,
                TextAlignmentOptions.Center);
        }

        Button button = CreateButton("Selector", card.transform,
            new Vector2(0.04f, 0.05f), new Vector2(0.96f, 0.25f),
            selectorSprite, PanelRaised, font, value, 19f, Violet,
            out TextMeshProUGUI valueText);
        CreateText("Prev", button.transform, "‹", new Vector2(0.01f, 0.06f),
            new Vector2(0.14f, 0.94f), font, 31f, Violet,
            TextAlignmentOptions.Center);
        CreateText("Next", button.transform, "›", new Vector2(0.86f, 0.06f),
            new Vector2(0.99f, 0.94f), font, 31f, Violet,
            TextAlignmentOptions.Center);
        RectTransform valueRect = valueText.rectTransform;
        valueRect.anchorMin = new Vector2(0.14f, 0.06f);
        valueRect.anchorMax = new Vector2(0.86f, 0.94f);
        valueRect.offsetMin = Vector2.zero;
        valueRect.offsetMax = Vector2.zero;
        return new SlotParts
        {
            button = button,
            value = valueText,
            icon = icon,
            outline = outline
        };
    }

    private static ReactorParts BuildReactor(Transform parent, TMP_FontAsset font,
        Sprite panelSprite, Sprite ringSprite)
    {
        GameObject reactor = CreatePanel("ReactionChamber", parent,
            new Vector2(0.025f, 0.425f), new Vector2(0.975f, 0.535f),
            panelSprite, PanelRaised, false);
        GameObject energyObject = CreateRect("EnergyWaveField", reactor.transform,
            new Vector2(0.015f, 0.16f), new Vector2(0.985f, 0.98f));
        FusionEnergyWaveGraphic energyWave =
            energyObject.AddComponent<FusionEnergyWaveGraphic>();
        energyWave.raycastTarget = false;
        Image cyan = CreateSolidImage("CyanStream", reactor.transform,
            new Vector2(0.045f, 0.055f), new Vector2(0.495f, 0.135f), Cyan);
        Image violet = CreateSolidImage("VioletStream", reactor.transform,
            new Vector2(0.505f, 0.055f), new Vector2(0.955f, 0.135f), Violet);
        CreateSolidImage("CyanCoreLine", reactor.transform,
            new Vector2(0.08f, 0.085f), new Vector2(0.50f, 0.105f), Color.white);
        CreateSolidImage("VioletCoreLine", reactor.transform,
            new Vector2(0.50f, 0.085f), new Vector2(0.92f, 0.105f), Color.white);

        GameObject coreObject = CreateRect("FusionCore", reactor.transform,
            new Vector2(0.435f, 0.08f), new Vector2(0.565f, 0.98f));
        Image core = coreObject.AddComponent<Image>();
        core.sprite = ringSprite;
        core.preserveAspect = true;
        core.color = Violet;
        core.raycastTarget = false;
        CreateText("CoreGlyph", coreObject.transform, "+", new Vector2(0.1f, 0.1f),
            new Vector2(0.9f, 0.9f), font, 30f, TextPrimary,
            TextAlignmentOptions.Center);
        Image[] particles = new Image[12];
        for (int i = 0; i < particles.Length; i++)
        {
            float x = 0.08f + i * (0.84f / (particles.Length - 1));
            float y = 0.28f + (i % 3) * 0.22f;
            particles[i] = CreateSolidImage("Particle_" + (i + 1),
                reactor.transform, new Vector2(x - 0.004f, y - 0.035f),
                new Vector2(x + 0.004f, y + 0.035f),
                i % 2 == 0 ? Cyan : Violet);
        }
        coreObject.transform.SetAsLastSibling();
        return new ReactorParts
        {
            cyan = cyan,
            violet = violet,
            core = core,
            particles = particles,
            energyWave = energyWave
        };
    }

    private static Image[] BuildSegmentBar(Transform parent, string name, int count,
        int activeCount, Vector2 anchorMin, Vector2 anchorMax, Color activeColor)
    {
        GameObject root = CreateRect(name, parent, anchorMin, anchorMax);
        Image[] segments = new Image[count];
        float gap = 0.012f;
        float width = (1f - gap * (count - 1)) / count;
        for (int i = 0; i < count; i++)
        {
            float x = i * (width + gap);
            segments[i] = CreateSolidImage("Segment_" + (i + 1), root.transform,
                new Vector2(x, 0f), new Vector2(x + width, 1f),
                i < activeCount ? activeColor : Hex("182028"));
            segments[i].raycastTarget = false;
        }
        return segments;
    }

    private static Image[] BuildDiagnosticBars(Transform parent,
        Vector2 anchorMin, Vector2 anchorMax, int count)
    {
        GameObject root = CreateRect("DiagnosticGraph", parent, anchorMin, anchorMax);
        Image[] bars = new Image[count];
        float gap = 0.018f;
        float width = (1f - gap * (count - 1)) / count;
        for (int i = 0; i < count; i++)
        {
            float x = i * (width + gap);
            bars[i] = CreateSolidImage("Pulse_" + (i + 1), root.transform,
                new Vector2(x, 0.08f), new Vector2(x + width, 0.5f),
                i % 3 == 0 ? Cyan : Violet);
        }
        return bars;
    }

    private static LogParts BuildLogOverlay(Transform parent, TMP_FontAsset font,
        Sprite panelSprite, Sprite buttonSprite)
    {
        GameObject root = CreatePanel("MixLogOverlay", parent,
            new Vector2(0.055f, 0.16f), new Vector2(0.945f, 0.79f),
            panelSprite, Background, true);
        CreateText("Title", root.transform, "REGISTRO DE MEZCLAS",
            new Vector2(0.08f, 0.90f), new Vector2(0.92f, 0.98f), font, 30f,
            TextPrimary, TextAlignmentOptions.Center);
        TextMeshProUGUI content = CreateText("Content", root.transform,
            "Recetas descubiertas", new Vector2(0.07f, 0.12f),
            new Vector2(0.93f, 0.88f), font, 18f, TextSecondary,
            TextAlignmentOptions.TopLeft);
        content.textWrappingMode = TextWrappingModes.Normal;
        content.overflowMode = TextOverflowModes.Ellipsis;
        Button close = CreateButton("Close", root.transform,
            new Vector2(0.34f, 0.025f), new Vector2(0.66f, 0.105f),
            buttonSprite, PanelRaised, font, "CERRAR", 22f, TextPrimary, out _);
        root.SetActive(false);
        return new LogParts { root = root, content = content, close = close };
    }

    private static void AddChevron(Transform parent, TMP_FontAsset font)
    {
        CreateText("Chevron", parent, "V", new Vector2(0.88f, 0.08f),
            new Vector2(0.98f, 0.92f), font, 27f, Violet,
            TextAlignmentOptions.Center);
    }

    private static GameObject CreatePanel(string name, Transform parent,
        Vector2 anchorMin, Vector2 anchorMax, Sprite sprite, Color color,
        bool raycast)
    {
        GameObject panel = CreateRect(name, parent, anchorMin, anchorMax);
        Image image = panel.AddComponent<Image>();
        image.sprite = sprite;
        image.type = Image.Type.Sliced;
        image.color = color;
        image.raycastTarget = raycast;
        return panel;
    }

    private static Button CreateButton(string name, Transform parent,
        Vector2 anchorMin, Vector2 anchorMax, Sprite sprite, Color color,
        TMP_FontAsset font, string label, float fontSize, Color textColor,
        out TextMeshProUGUI labelText)
    {
        GameObject buttonObject = CreateRect(name, parent, anchorMin, anchorMax);
        Image image = buttonObject.AddComponent<Image>();
        image.sprite = sprite;
        image.type = Image.Type.Sliced;
        image.color = color;
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.transition = Selectable.Transition.ColorTint;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 1f, 1f, 1f);
        colors.pressedColor = new Color(0.72f, 0.72f, 0.78f, 1f);
        colors.disabledColor = new Color(0.45f, 0.45f, 0.48f, 0.62f);
        colors.colorMultiplier = 1f;
        button.colors = colors;
        labelText = CreateText("Label", buttonObject.transform, label,
            new Vector2(0.06f, 0.08f), new Vector2(0.94f, 0.92f), font,
            fontSize, textColor, TextAlignmentOptions.Center);
        labelText.fontStyle = FontStyles.Bold;
        return button;
    }

    private static TextMeshProUGUI CreateText(string name, Transform parent,
        string value, Vector2 anchorMin, Vector2 anchorMax, TMP_FontAsset font,
        float size, Color color, TextAlignmentOptions alignment)
    {
        GameObject textObject = CreateRect(name, parent, anchorMin, anchorMax);
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.font = font;
        text.text = value;
        text.fontSize = size;
        text.fontSizeMax = size;
        text.fontSizeMin = Mathf.Max(12f, size * 0.68f);
        text.enableAutoSizing = true;
        text.color = color;
        text.alignment = alignment;
        text.raycastTarget = false;
        text.overflowMode = TextOverflowModes.Ellipsis;
        return text;
    }

    private static Image CreateSolidImage(string name, Transform parent,
        Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        GameObject imageObject = CreateRect(name, parent, anchorMin, anchorMax);
        Image image = imageObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static GameObject CreateRect(string name, Transform parent,
        Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject child = new GameObject(name, typeof(RectTransform));
        child.layer = 5;
        RectTransform rect = child.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        SetRect(rect, anchorMin, anchorMax);
        return child;
    }

    private static void SetRect(RectTransform rect, Vector2 anchorMin,
        Vector2 anchorMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
    }

    private static void SetObject(SerializedObject serializedObject,
        string propertyName, UnityEngine.Object value)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        Require(property != null, "No existe el campo serializado: " + propertyName);
        property.objectReferenceValue = value;
    }

    private static void SetObjectArray(SerializedObject serializedObject,
        string propertyName, UnityEngine.Object[] values)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        Require(property != null, "No existe el arreglo serializado: " + propertyName);
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

    private static void ConfigureSpriteImport(string assetPath)
    {
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        Require(importer != null, "No se pudo importar el icono: " + assetPath);
        bool changed = importer.textureType != TextureImporterType.Sprite ||
            importer.spriteImportMode != SpriteImportMode.Single ||
            !importer.alphaIsTransparency || importer.mipmapEnabled;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        if (changed)
            importer.SaveAndReimport();
    }

    private static void ValidateBuiltPanel(Room2PanelUI room,
        MachinePanelUI machine, GameObject root)
    {
        Require(root.transform.Find("FusionVisualShell") != null,
            "Falta FusionVisualShell.");
        Require(root.GetComponent<MachineFusionPanelVisualUI>() != null,
            "Falta MachineFusionPanelVisualUI.");
        SerializedObject roomSo = new SerializedObject(room);
        string[] required =
        {
            "mixButton", "logButton", "logPanel", "statusText",
            "fusionSlotsText", "compositionReadingText", "fragmentSlotAButton",
            "fragmentSlotBButton", "catalystSlotButton", "modeButton",
            "guidedIntentButton", "instabilityText", "coolButton"
        };
        foreach (string propertyName in required)
        {
            SerializedProperty property = roomSo.FindProperty(propertyName);
            Require(property != null && property.objectReferenceValue != null,
                "Referencia faltante en Room2PanelUI: " + propertyName);
        }
        SerializedObject machineSo = new SerializedObject(machine);
        Require(machineSo.FindProperty("legacyFusionPanel").objectReferenceValue == root,
            "MachinePanelUI no apunta al panel visual nuevo.");
    }

    private static Color Hex(string hex, byte alpha = 255)
    {
        if (!ColorUtility.TryParseHtmlString("#" + hex, out Color color))
            color = Color.white;
        color.a = alpha / 255f;
        return color;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }

    private sealed class SlotParts
    {
        public Button button;
        public TextMeshProUGUI value;
        public Image icon;
        public Outline outline;
    }

    private sealed class ReactorParts
    {
        public Image cyan;
        public Image violet;
        public Image core;
        public Image[] particles;
        public FusionEnergyWaveGraphic energyWave;
    }

    private sealed class LogParts
    {
        public GameObject root;
        public TextMeshProUGUI content;
        public Button close;
    }
}
#endif

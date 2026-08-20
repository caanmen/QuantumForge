#if UNITY_EDITOR
using System;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class GenerationLabQuarantineSetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ThemePath =
        "Assets/Project/UI/Vertical/Generated/VerticalUiTheme.asset";
    private const string BackgroundPath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_lab_accident_background_v3.png";
    private const string SelectorPlatePath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_selector_metal_plate_v2.png";
    private const string ModuleCardPath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_module_card_metal_v2.png";
    private const string SidePanelPath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_side_status_metal_v3.png";
    private const string ResourcePanelPath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_resource_counter_metal_v2.png";
    private const string CardBorderPath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_selector_frame.png";

    private static readonly Color WarningRed = new(0.86f, 0.14f, 0.12f, 1f);
    private static readonly Color WarningDark = new(0.17f, 0.025f, 0.025f, 0.98f);
    private static readonly Color HiggsBlue = new(0.01f, 0.30f, 0.76f, 1f);

    [MenuItem("Tools/Quantum Forge/Vertical UI/Apply Lab Quarantine Redesign 96")]
    public static void Apply()
    {
        ConfigureTextureImports();
        VerticalUiTheme theme = AssetDatabase.LoadAssetAtPath<VerticalUiTheme>(ThemePath);
        Sprite background = AssetDatabase.LoadAssetAtPath<Sprite>(BackgroundPath);
        Sprite selectorPlate = AssetDatabase.LoadAssetAtPath<Sprite>(SelectorPlatePath);
        Sprite moduleCard = AssetDatabase.LoadAssetAtPath<Sprite>(ModuleCardPath);
        Sprite sidePanel = AssetDatabase.LoadAssetAtPath<Sprite>(SidePanelPath);
        Sprite resourcePanel = AssetDatabase.LoadAssetAtPath<Sprite>(ResourcePanelPath);
        Sprite cardBorder = AssetDatabase.LoadAssetAtPath<Sprite>(CardBorderPath);
        Require(theme != null, "Falta VerticalUiTheme.");
        Require(background != null && selectorPlate != null && moduleCard != null &&
            sidePanel != null && resourcePanel != null && cardBorder != null,
            "No se importaron los nuevos assets metalicos.");

        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Transform header = FindUnique(scene, "VerticalGenerationHeader");
        Transform root = FindUnique(scene, "GenerationTriangleRoot");
        Require(header != null && root != null,
            "La pantalla avanzada de Generacion no esta configurada.");

        Transform content = root.Find("TriangleScroll/Viewport/Content");
        Transform focus = content?.Find("TriangleFocus");
        Transform circuits = content?.Find("CircuitSelectors");
        Transform cards = content?.Find("TriangleArtifactCards");
        Transform purchasesTitle = content?.Find("TriangleArtifactsTitle");
        Require(content != null && focus != null && circuits != null &&
            cards != null && purchasesTitle != null,
            "La jerarquia del Triangulo esta incompleta.");

        ConfigureHeader(header, resourcePanel, theme);
        ConfigureRoot(root, content, background, theme);
        GenerationLabQuarantineUI controller = ConfigureFocus(
            focus, background, sidePanel, theme);
        Transform oldBanner = content.Find("QuarantineBanner");
        if (oldBanner != null) oldBanner.gameObject.SetActive(false);
        controller.quarantineBannerText = null;
        ConfigureCircuits(circuits, selectorPlate, theme);
        ConfigurePurchases(content, purchasesTitle, cards,
            moduleCard, resourcePanel, cardBorder, theme);
        ApplyFont(root, theme);

        EditorUtility.SetDirty(controller);
        EditorSceneManager.MarkSceneDirty(scene);
        Require(EditorSceneManager.SaveScene(scene, ScenePath),
            "No se pudo guardar Main.unity.");
        AssetDatabase.SaveAssets();
        Debug.Log("[Lab Accident UI] APPLIED | laboratorio legible | " +
            "placas metalicas | ENERGIA real | sin cartel inventado");
    }


    [MenuItem("Tools/Quantum Forge/Vertical UI/Validate Lab Quarantine Redesign")]
    public static void Validate()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Transform root = FindUnique(scene, "GenerationTriangleRoot");
        Transform focus = FindUnique(scene, "TriangleFocus");
        Transform banner = FindUnique(scene, "QuarantineBanner");
        Transform left = FindUnique(scene, "RealCircuitStatus");
        Transform right = FindUnique(scene, "RealEffectStatus");
        Transform background = FindUnique(scene, "LabContainmentBackground");
        GenerationLabQuarantineUI controller = focus != null
            ? focus.GetComponent<GenerationLabQuarantineUI>()
            : null;

        Require(root != null && focus != null &&
            left != null && right != null && background != null,
            "Falta una pieza visual del rediseño de cuarentena.");
        Require(banner == null || !banner.gameObject.activeSelf,
            "El cartel de cuarentena no debe mostrarse en esta pantalla.");
        Require(controller != null && controller.activeCircuitValue != null &&
            controller.synchronizationFill != null &&
            controller.benefitValue != null && controller.sacrificeValue != null,
            "Los laterales no estan conectados a datos reales.");
        Require(background.GetComponent<Image>()?.sprite != null,
            "El fondo del laboratorio no tiene sprite.");
        Require(Mathf.Abs(((RectTransform)root).offsetMin.x - 2f) < 0.1f,
            "La composicion perdio el margen objetivo de 2 px.");
        Require(FindText(scene, "PRESION") == null &&
            FindText(scene, "NIVEL DE BRECHA") == null &&
            FindText(scene, "SISTEMAS ESTRES") == null,
            "Reaparecio informacion ambiental que parece una mecanica.");
        Require(FindUnique(scene, "Circuit_Energy")?.GetComponent<Button>() != null &&
            FindUnique(scene, "Circuit_Experimental")?.GetComponent<Button>() != null &&
            FindUnique(scene, "Circuit_Phase")?.GetComponent<Button>() != null,
            "Los tres selectores dejaron de ser interactivos.");
        Require(FindUnique(scene, "TriangleCard_Higgs")
                ?.GetComponent<VerticalTriangleArtifactCardUI>() != null &&
            FindUnique(scene, "TriangleCard_Tetra")
                ?.GetComponent<VerticalTriangleArtifactCardUI>() != null &&
            FindUnique(scene, "TriangleCard_Modulator")
                ?.GetComponent<VerticalTriangleArtifactCardUI>() != null,
            "Las compras reales del Triangulo no estan conectadas.");
        Debug.Log("[Lab Quarantine UI] VALIDATION PASS | composicion | fondo | " +
            "datos reales | botones | compras");
    }

    public static void ApplyAndValidateBatch()
    {
        try
        {
            Apply();
            Apply();
            Validate();
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    private static void ConfigureTextureImports()
    {
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        foreach (string path in new[]
        {
            BackgroundPath, SelectorPlatePath, ModuleCardPath,
            SidePanelPath, ResourcePanelPath
        })
            ConfigureTextureImport(path);
    }

    private static void ConfigureTextureImport(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        Require(importer != null, "No se pudo configurar " + path + ".");
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = false;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Bilinear;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        importer.maxTextureSize = 2048;
        TextureImporterPlatformSettings android =
            importer.GetPlatformTextureSettings("Android");
        android.overridden = true;
        android.maxTextureSize = 2048;
        android.format = TextureImporterFormat.ASTC_6x6;
        android.compressionQuality = 100;
        importer.SetPlatformTextureSettings(android);
        importer.SaveAndReimport();
    }

    private static void ConfigureHeader(
        Transform header,
        Sprite resourcePanel,
        VerticalUiTheme theme)
    {
        RectTransform rect = (RectTransform)header;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -22f);
        rect.sizeDelta = new Vector2(-168f, 96f);

        Transform le = header.Find("Resource_LE");
        Transform traces = header.Find("Resource_Traces");
        Transform energy = header.Find("Resource_Energy");
        Require(le != null && traces != null && energy != null,
            "Faltan paneles de recursos.");
        SetAnchors((RectTransform)le, new Vector2(0f, 0f), new Vector2(0.324f, 1f));
        SetAnchors((RectTransform)traces, new Vector2(0.338f, 0f), new Vector2(0.662f, 1f));
        SetAnchors((RectTransform)energy, new Vector2(0.676f, 0f), new Vector2(1f, 1f));
        foreach (Transform resource in new[] { le, traces, energy })
        {
            Image image = resource.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = resourcePanel;
                image.type = Image.Type.Simple;
                image.color = Color.white;
            }
            TextMeshProUGUI value = resource.Find("Value")?.GetComponent<TextMeshProUGUI>();
            if (value != null)
            {
                value.fontSize = 28f;
                value.fontSizeMax = 28f;
                value.fontSizeMin = 20f;
            }
        }
    }

    private static void ConfigureRoot(
        Transform root,
        Transform content,
        Sprite background,
        VerticalUiTheme theme)
    {
        RectTransform rootRect = (RectTransform)root;
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = new Vector2(2f, 20f);
        rootRect.offsetMax = new Vector2(-2f, -138f);

        Image rootBackground = GetOrAdd<Image>(root.gameObject);
        rootBackground.sprite = theme.backgroundGrid;
        rootBackground.type = Image.Type.Simple;
        rootBackground.color = new Color(0.54f, 0.62f, 0.66f, 1f);
        rootBackground.raycastTarget = false;

        RectTransform contentRect = (RectTransform)content;
        contentRect.sizeDelta = new Vector2(0f, 1620f);
        ScrollRect scroll = root.Find("TriangleScroll")?.GetComponent<ScrollRect>();
        if (scroll != null)
        {
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 34f;
        }

        Transform oldTitle = content.Find("TriangleGenerationTitle");
        Transform oldFrame = content.Find("GenerationTitleFrame");
        if (oldTitle != null) oldTitle.gameObject.SetActive(false);
        if (oldFrame != null) oldFrame.gameObject.SetActive(false);
    }

    private static GenerationLabQuarantineUI ConfigureFocus(
        Transform focus,
        Sprite background,
        Sprite sidePanel,
        VerticalUiTheme theme)
    {
        SetTopRect((RectTransform)focus, 0f, 850f, 0f, 0f);
        Image rootImage = focus.GetComponent<Image>();
        if (rootImage != null)
        {
            rootImage.sprite = theme.panelFrame;
            rootImage.type = Image.Type.Sliced;
            rootImage.color = new Color(0.22f, 0.31f, 0.36f, 0.35f);
        }

        Image backgroundImage = CreateImage(
            "LabContainmentBackground", focus, background, Color.white);
        Stretch(backgroundImage.rectTransform, 3f);
        backgroundImage.type = Image.Type.Simple;
        backgroundImage.preserveAspect = false;
        backgroundImage.transform.SetAsFirstSibling();

        SetActive(focus.Find("TechnologyGrid"), false);
        SetActive(focus.Find("FocusInnerFrame"), false);

        Transform higgs = focus.Find("Vertex_Higgs");
        Transform tetra = focus.Find("Vertex_Tetra");
        Transform modulator = focus.Find("Vertex_Modulator");
        Require(higgs != null && tetra != null && modulator != null,
            "Faltan vertices del Triangulo.");
        ConfigureNode(higgs, new Vector2(-202f, 215f), new Vector2(224f, 224f), true);
        ConfigureNode(tetra, new Vector2(202f, 215f), new Vector2(230f, 230f), true);
        ConfigureNode(modulator, new Vector2(0f, -220f), new Vector2(230f, 230f), false);

        ConfigureBeam(focus.Find("Line_Experimental"),
            new Vector2(-202f, 215f), new Vector2(202f, 215f), 24f);
        ConfigureBeam(focus.Find("Line_Energy"),
            new Vector2(-202f, 215f), new Vector2(0f, -220f), 26f);
        ConfigureBeam(focus.Find("Line_Phase"),
            new Vector2(202f, 215f), new Vector2(0f, -220f), 26f);

        Transform gauge = focus.Find("EnergyGauge");
        Require(gauge != null, "Falta EnergyGauge.");
        SetCentered((RectTransform)gauge, new Vector2(0f, 46f),
            new Vector2(176f, 176f));
        Image gaugeBackdrop = gauge.Find("GaugeBackdrop")?.GetComponent<Image>();
        if (gaugeBackdrop != null)
            gaugeBackdrop.color = new Color(0.018f, 0.026f, 0.030f, 0.98f);
        TextMeshProUGUI gaugeTitle = gauge.Find("GaugeCircuit")?
            .GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI gaugeValue = gauge.Find("GaugeProgress")?
            .GetComponent<TextMeshProUGUI>();
        if (gaugeTitle != null)
        {
            gaugeTitle.fontSize = 15f;
            gaugeTitle.fontSizeMax = 15f;
            gaugeTitle.fontSizeMin = 10f;
            gaugeTitle.fontStyle = FontStyles.Bold;
            SetCentered(gaugeTitle.rectTransform, new Vector2(0f, 38f),
                new Vector2(144f, 28f));
            gaugeTitle.gameObject.SetActive(false);
        }
        if (gaugeValue != null)
        {
            gaugeValue.fontSize = 18f;
            gaugeValue.fontSizeMax = 18f;
            gaugeValue.fontSizeMin = 11f;
            gaugeValue.fontStyle = FontStyles.Bold;
            gaugeValue.lineSpacing = 6f;
            SetCentered(gaugeValue.rectTransform, new Vector2(0f, -12f),
                new Vector2(150f, 68f));
            gaugeValue.gameObject.SetActive(false);
        }
        Transform legacyGlow = focus.Find("SynchronizationCore");
        if (legacyGlow != null) legacyGlow.gameObject.SetActive(false);
        SetActive(focus.Find("ProtocolStatus"), false);
        SetActive(focus.Find("SynchronizationStatus"), false);
        SetActive(focus.Find("CircuitEffect"), false);

        GenerationLabQuarantineUI controller =
            GetOrAdd<GenerationLabQuarantineUI>(focus.gameObject);
        BuildRealCircuitPanel(focus, controller, sidePanel, theme);
        BuildRealEffectPanel(focus, controller, sidePanel, theme);
        return controller;
    }

    private static void ConfigureBanner(
        Transform content,
        GenerationLabQuarantineUI controller,
        VerticalUiTheme theme)
    {
        GameObject banner = GetOrCreateUi("QuarantineBanner", content);
        SetTopRect((RectTransform)banner.transform, 0f, 72f, 134f, -134f);
        Image background = GetOrAdd<Image>(banner);
        background.sprite = theme.buttonFrame;
        background.type = Image.Type.Sliced;
        background.color = WarningDark;
        background.raycastTarget = false;

        Image border = CreateImage("WarningBorder", banner.transform,
            theme.selectedButtonFrame, new Color(WarningRed.r, WarningRed.g, WarningRed.b, 0.72f));
        Stretch(border.rectTransform, 5f);
        border.type = Image.Type.Sliced;

        TextMeshProUGUI label = CreateText("QuarantineText", banner.transform,
            "PROTOCOLO DE CUARENTENA", 28f, TextAlignmentOptions.Center,
            new Color(1f, 0.34f, 0.30f, 1f), theme);
        SetAnchors(label.rectTransform, new Vector2(0.20f, 0f), new Vector2(0.80f, 1f));
        label.characterSpacing = 2.6f;
        label.fontStyle = FontStyles.Bold;
        controller.quarantineBannerText = label;

        BuildWarningMark("WarningLeft", banner.transform, new Vector2(0.175f, 0.5f));
        BuildWarningMark("WarningRight", banner.transform, new Vector2(0.825f, 0.5f));
        BuildHazardStripes("StripesLeft", banner.transform, 0.045f, 1f);
        BuildHazardStripes("StripesRight", banner.transform, 0.955f, -1f);
        banner.transform.SetSiblingIndex(0);
    }

    private static void BuildRealCircuitPanel(
        Transform focus,
        GenerationLabQuarantineUI controller,
        Sprite sidePanel,
        VerticalUiTheme theme)
    {
        GameObject panel = GetOrCreateUi("RealCircuitStatus", focus);
        RectTransform rect = (RectTransform)panel.transform;
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0f, 0.5f);
        rect.anchoredPosition = new Vector2(8f, 0f);
        rect.sizeDelta = new Vector2(166f, 760f);
        StyleSidePanel(panel, sidePanel, theme);

        controller.activeAccent = CreateImage("ActiveAccent", panel.transform,
            null, HiggsBlue);
        controller.activeAccent.rectTransform.anchorMin = new Vector2(0f, 0.04f);
        controller.activeAccent.rectTransform.anchorMax = new Vector2(0f, 0.96f);
        controller.activeAccent.rectTransform.anchoredPosition = new Vector2(7f, 0f);
        controller.activeAccent.rectTransform.sizeDelta = new Vector2(4f, 0f);
        controller.activeAccent.gameObject.SetActive(false);

        controller.activeSectionTitle = CreateText("ActiveTitle", panel.transform,
            "CIRCUITO ACTIVO", 15f, TextAlignmentOptions.Center,
            theme.secondaryText, theme);
        SetAnchors(controller.activeSectionTitle.rectTransform,
            new Vector2(0.13f, 0.85f), new Vector2(0.87f, 0.93f));
        controller.activeSectionTitle.characterSpacing = 0.5f;
        controller.activeSectionTitle.enableAutoSizing = true;
        controller.activeSectionTitle.fontSizeMin = 11f;

        controller.activeCircuitValue = CreateText("ActiveValue", panel.transform,
            "LE", 30f, TextAlignmentOptions.Center, HiggsBlue, theme);
        SetAnchors(controller.activeCircuitValue.rectTransform,
            new Vector2(0.12f, 0.73f), new Vector2(0.88f, 0.84f));
        controller.activeCircuitValue.fontStyle = FontStyles.Bold;

        CreateSeparator("SeparatorTop", panel.transform, 0.63f, theme);

        controller.synchronizationTitle = CreateText("SyncTitle", panel.transform,
            "SINCRONIZACION", 14f, TextAlignmentOptions.Center,
            theme.secondaryText, theme);
        SetAnchors(controller.synchronizationTitle.rectTransform,
            new Vector2(0.12f, 0.55f), new Vector2(0.88f, 0.63f));
        controller.synchronizationTitle.enableAutoSizing = true;
        controller.synchronizationTitle.fontSizeMin = 10f;
        controller.synchronizationValue = CreateText("SyncValue", panel.transform,
            "68%", 34f, TextAlignmentOptions.Center, theme.energy, theme);
        SetAnchors(controller.synchronizationValue.rectTransform,
            new Vector2(0.12f, 0.43f), new Vector2(0.88f, 0.54f));
        controller.synchronizationValue.fontStyle = FontStyles.Bold;

        Image track = CreateImage("SyncTrack", panel.transform, null,
            new Color(0.02f, 0.06f, 0.08f, 0.94f));
        SetAnchors(track.rectTransform,
            new Vector2(0.16f, 0.355f), new Vector2(0.84f, 0.375f));
        controller.synchronizationFill = CreateImage("SyncFill", track.transform,
            null, theme.energy);
        Stretch(controller.synchronizationFill.rectTransform, 1f);
        controller.synchronizationFill.type = Image.Type.Filled;
        controller.synchronizationFill.fillMethod = Image.FillMethod.Horizontal;
        controller.synchronizationFill.fillOrigin = 0;
        controller.synchronizationFill.fillAmount = 0.68f;

        controller.synchronizationEta = CreateText("SyncEta", panel.transform,
            "ESTABLE", 14f, TextAlignmentOptions.Center,
            theme.secondaryText, theme);
        SetAnchors(controller.synchronizationEta.rectTransform,
            new Vector2(0.10f, 0.145f), new Vector2(0.90f, 0.255f));
        controller.synchronizationEta.enableAutoSizing = true;
        controller.synchronizationEta.fontSizeMin = 10f;
        HideLegacySidePanelDamage(panel.transform);
    }

    private static void BuildRealEffectPanel(
        Transform focus,
        GenerationLabQuarantineUI controller,
        Sprite sidePanel,
        VerticalUiTheme theme)
    {
        GameObject panel = GetOrCreateUi("RealEffectStatus", focus);
        RectTransform rect = (RectTransform)panel.transform;
        rect.anchorMin = new Vector2(1f, 0.5f);
        rect.anchorMax = new Vector2(1f, 0.5f);
        rect.pivot = new Vector2(1f, 0.5f);
        rect.anchoredPosition = new Vector2(-8f, 0f);
        rect.sizeDelta = new Vector2(166f, 760f);
        StyleSidePanel(panel, sidePanel, theme);

        controller.effectAccent = CreateImage("EffectAccent", panel.transform,
            null, HiggsBlue);
        controller.effectAccent.rectTransform.anchorMin = new Vector2(1f, 0.04f);
        controller.effectAccent.rectTransform.anchorMax = new Vector2(1f, 0.96f);
        controller.effectAccent.rectTransform.anchoredPosition = new Vector2(-7f, 0f);
        controller.effectAccent.rectTransform.sizeDelta = new Vector2(4f, 0f);
        controller.effectAccent.gameObject.SetActive(false);

        controller.effectSectionTitle = CreateText("EffectTitle", panel.transform,
            "EFECTO ACTUAL", 15f, TextAlignmentOptions.Center,
            theme.secondaryText, theme);
        SetAnchors(controller.effectSectionTitle.rectTransform,
            new Vector2(0.13f, 0.85f), new Vector2(0.87f, 0.93f));
        controller.effectSectionTitle.characterSpacing = 0.5f;
        controller.effectSectionTitle.enableAutoSizing = true;
        controller.effectSectionTitle.fontSizeMin = 11f;

        controller.activeEffectValue = CreateText(
            "ActiveEffectValue", panel.transform, "ENERGÍA", 27f,
            TextAlignmentOptions.Center, theme.triangle, theme);
        SetAnchors(controller.activeEffectValue.rectTransform,
            new Vector2(0.13f, 0.69f), new Vector2(0.87f, 0.83f));
        controller.activeEffectValue.fontStyle = FontStyles.Bold;
        controller.activeEffectValue.enableAutoSizing = true;
        controller.activeEffectValue.fontSizeMax = 27f;
        controller.activeEffectValue.fontSizeMin = 15f;

        controller.benefitTitle = CreateText("BenefitTitle", panel.transform,
            "BONIFICACIÓN", 15f, TextAlignmentOptions.Center,
            theme.secondaryText, theme);
        SetAnchors(controller.benefitTitle.rectTransform,
            new Vector2(0.13f, 0.555f), new Vector2(0.87f, 0.625f));
        controller.benefitTitle.enableAutoSizing = true;
        controller.benefitTitle.fontSizeMin = 10f;
        controller.benefitValue = CreateText("BenefitValue", panel.transform,
            "+12% LE", 20f, TextAlignmentOptions.Center, theme.energy, theme);
        SetAnchors(controller.benefitValue.rectTransform,
            new Vector2(0.16f, 0.405f), new Vector2(0.84f, 0.545f));
        controller.benefitValue.enableAutoSizing = true;
        controller.benefitValue.fontSizeMax = 18f;
        controller.benefitValue.fontSizeMin = 9f;
        controller.benefitValue.textWrappingMode = TextWrappingModes.Normal;
        controller.benefitValue.fontStyle = FontStyles.Bold;

        SetActive(panel.transform.Find("EffectSeparator"), false);

        controller.sacrificeTitle = CreateText("SacrificeTitle", panel.transform,
            "PENALIZACIÓN", 15f, TextAlignmentOptions.Center,
            new Color(1f, 0.35f, 0.31f, 1f), theme);
        SetAnchors(controller.sacrificeTitle.rectTransform,
            new Vector2(0.13f, 0.275f), new Vector2(0.87f, 0.335f));
        controller.sacrificeTitle.enableAutoSizing = true;
        controller.sacrificeTitle.fontSizeMin = 10f;
        controller.sacrificeValue = CreateText("SacrificeValue", panel.transform,
            "-10% TRAZAS", 18f, TextAlignmentOptions.Center,
            new Color(1f, 0.40f, 0.34f, 1f), theme);
        SetAnchors(controller.sacrificeValue.rectTransform,
            new Vector2(0.16f, 0.12f), new Vector2(0.84f, 0.26f));
        controller.sacrificeValue.enableAutoSizing = true;
        controller.sacrificeValue.fontSizeMax = 16f;
        controller.sacrificeValue.fontSizeMin = 9f;
        controller.sacrificeValue.textWrappingMode = TextWrappingModes.Normal;
        controller.sacrificeValue.fontStyle = FontStyles.Bold;
        HideLegacySidePanelDamage(panel.transform);
    }

    private static void ConfigureCircuits(
        Transform circuits,
        Sprite selectorPlate,
        VerticalUiTheme theme)
    {
        SetTopRect((RectTransform)circuits, 860f, 260f, 10f, -10f);
        HorizontalLayoutGroup layout = circuits.GetComponent<HorizontalLayoutGroup>();
        Require(layout != null, "CircuitSelectors perdio su layout.");
        layout.padding = new RectOffset(2, 2, 2, 2);
        layout.spacing = 8f;
        foreach (Transform circuit in circuits)
        {
            Image circuitBackground = GetOrAdd<Image>(circuit.gameObject);
            circuitBackground.sprite = selectorPlate;
            circuitBackground.type = Image.Type.Simple;
            circuitBackground.color = new Color(0.52f, 0.55f, 0.56f, 1f);
            Button circuitButton = circuit.GetComponent<Button>();
            if (circuitButton != null) circuitButton.targetGraphic = circuitBackground;
            LayoutElement element = GetOrAdd<LayoutElement>(circuit.gameObject);
            element.minHeight = 256f;
            element.preferredHeight = 256f;

            SetActive(circuit.Find("SelectorInnerPlate"), false);
            SetActive(circuit.Find("SelectorHeaderPlate"), false);

            Image diagramWell = CreateImage("DiagramWell", circuit,
                null, new Color(0.018f, 0.022f, 0.024f, 0.92f));
            diagramWell.type = Image.Type.Simple;
            SetAnchors(diagramWell.rectTransform,
                new Vector2(0.12f, 0.11f), new Vector2(0.88f, 0.66f));
            diagramWell.transform.SetAsFirstSibling();
            diagramWell.gameObject.SetActive(false);

            Transform icon = circuit.Find("CircuitIcon");
            Transform label = circuit.Find("Label");
            bool tracesCircuit = circuit.name.Contains("Experimental");
            bool energyCircuit = circuit.name.Contains("Phase");
            if (icon != null)
            {
                RectTransform iconRect = (RectTransform)icon;
                iconRect.anchorMin = new Vector2(0.5f, 0.5f);
                iconRect.anchorMax = new Vector2(0.5f, 0.5f);
                iconRect.pivot = new Vector2(0.5f, 0.5f);
                iconRect.anchoredPosition = new Vector2(0f, -36f);
                iconRect.sizeDelta = new Vector2(96f, 96f);
                iconRect.localRotation = Quaternion.identity;
                icon.gameObject.SetActive(false);
            }
            Color diagramColor = tracesCircuit
                ? theme.traces
                : energyCircuit ? theme.triangle : HiggsBlue;
            BuildNativeTriangleIcon(circuit, diagramColor, theme);
            if (label != null)
            {
                TextMeshProUGUI text = label.GetComponent<TextMeshProUGUI>();
                text.fontSize = 22f;
                text.fontSizeMax = 22f;
                text.fontSizeMin = 14f;
                text.alignment = TextAlignmentOptions.Center;
                text.verticalAlignment = VerticalAlignmentOptions.Middle;
                text.fontStyle = FontStyles.Bold;
                text.color = theme.secondaryText;
                SetAnchors(text.rectTransform,
                    new Vector2(0.08f, 0.69f), new Vector2(0.92f, 0.94f));
            }

            Color lampColor = circuit.name.Contains("Experimental")
                ? theme.traces
                : circuit.name.Contains("Phase") ? theme.triangle : HiggsBlue;
            Image lamp = CreateImage("SelectorLamp", circuit, null, lampColor);
            lamp.rectTransform.anchorMin = new Vector2(0.42f, 0f);
            lamp.rectTransform.anchorMax = new Vector2(0.58f, 0f);
            lamp.rectTransform.pivot = new Vector2(0.5f, 0f);
            lamp.rectTransform.anchoredPosition = new Vector2(0f, 15f);
            lamp.rectTransform.sizeDelta = new Vector2(0f, 5f);
            BuildPanelBolts(circuit, theme);
        }
    }

    private static void ConfigurePurchases(
        Transform content,
        Transform title,
        Transform cards,
        Sprite moduleCard,
        Sprite buttonPlate,
        Sprite cardBorder,
        VerticalUiTheme theme)
    {
        Transform frame = content.Find("TrianglePurchasesFrame");
        Require(frame != null, "Falta TrianglePurchasesFrame.");
        SetTopRect((RectTransform)frame, 1128f, 374f, 10f, -10f);
        Image frameImage = frame.GetComponent<Image>();
        if (frameImage != null)
        {
            frameImage.sprite = theme.panelFrame;
            frameImage.type = Image.Type.Sliced;
            frameImage.color = new Color(0.56f, 0.65f, 0.70f, 1f);
        }

        title.gameObject.SetActive(true);
        SetTopRect((RectTransform)title, 1134f, 42f, 28f, -28f);
        TextMeshProUGUI titleText = title.GetComponent<TextMeshProUGUI>();
        titleText.SetText("MODULOS DISPONIBLES");
        titleText.fontSize = 22f;
        titleText.fontSizeMax = 22f;
        titleText.fontSizeMin = 17f;
        titleText.characterSpacing = 2.2f;
        titleText.alignment = TextAlignmentOptions.MidlineLeft;

        SetTopRect((RectTransform)cards, 1176f, 292f, 22f, -22f);
        VerticalLayoutGroup oldVertical = cards.GetComponent<VerticalLayoutGroup>();
        if (oldVertical != null)
            UnityEngine.Object.DestroyImmediate(oldVertical);
        HorizontalLayoutGroup layout = GetOrAdd<HorizontalLayoutGroup>(cards.gameObject);
        layout.padding = new RectOffset(4, 4, 4, 4);
        layout.spacing = 20f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;

        foreach (Transform card in cards)
            ConfigurePurchaseCard(card, moduleCard, buttonPlate, cardBorder, theme);
    }

    private static void ConfigurePurchaseCard(
        Transform card,
        Sprite moduleCard,
        Sprite buttonPlate,
        Sprite cardBorder,
        VerticalUiTheme theme)
    {
        LayoutElement layout = GetOrAdd<LayoutElement>(card.gameObject);
        layout.minWidth = 220f;
        layout.preferredWidth = 300f;
        layout.flexibleWidth = 1f;
        layout.minHeight = 280f;
        layout.preferredHeight = 286f;
        layout.flexibleHeight = 1f;

        Transform accent = card.Find("AccentBar");
        if (accent != null)
            accent.gameObject.SetActive(false);

        Transform icon = card.Find("Icon");
        Transform name = card.Find("Name");
        Transform state = card.Find("State");
        Transform buy = card.Find("BuyButton");
        Require(icon != null && name != null && state != null && buy != null,
            card.name + " perdio una pieza.");

        RectTransform iconRect = (RectTransform)icon;
        iconRect.anchorMin = new Vector2(0f, 1f);
        iconRect.anchorMax = new Vector2(0f, 1f);
        iconRect.pivot = new Vector2(0f, 1f);
        iconRect.anchoredPosition = new Vector2(22f, -35f);
        iconRect.sizeDelta = new Vector2(108f, 108f);

        TextMeshProUGUI nameText = name.GetComponent<TextMeshProUGUI>();
        nameText.fontSize = 22f;
        nameText.fontSizeMax = 22f;
        nameText.fontSizeMin = 12f;
        nameText.alignment = TextAlignmentOptions.MidlineLeft;
        nameText.textWrappingMode = TextWrappingModes.NoWrap;
        SetAnchors(nameText.rectTransform,
            new Vector2(0.40f, 0.75f), new Vector2(0.93f, 0.92f));

        TextMeshProUGUI roleText = CreateText("Role", card,
            "MODULO CUANTICO", 19f, TextAlignmentOptions.MidlineLeft,
            state.GetComponent<TextMeshProUGUI>().color, theme);
        SetAnchors(roleText.rectTransform,
            new Vector2(0.40f, 0.52f), new Vector2(0.93f, 0.73f));

        TextMeshProUGUI stateText = state.GetComponent<TextMeshProUGUI>();
        stateText.fontSize = 18f;
        stateText.fontSizeMax = 18f;
        stateText.fontSizeMin = 13f;
        stateText.alignment = TextAlignmentOptions.Center;
        SetAnchors(stateText.rectTransform,
            new Vector2(0.08f, 0.28f), new Vector2(0.92f, 0.43f));

        RectTransform buyRect = (RectTransform)buy;
        SetAnchors(buyRect, new Vector2(0.055f, 0.055f), new Vector2(0.945f, 0.255f));
        Color accentColor = card.name.Contains("Tetra")
            ? theme.traces
            : card.name.Contains("Modulator") ? theme.triangle : theme.energy;
        Image buyImage = buy.GetComponent<Image>();
        if (buyImage != null)
        {
            buyImage.sprite = theme.buttonFrame;
            buyImage.type = Image.Type.Sliced;
            buyImage.color = new Color(
                0.025f + accentColor.r * 0.055f,
                0.038f + accentColor.g * 0.045f,
                0.050f + accentColor.b * 0.040f,
                0.98f);
        }
        Image buyBorder = CreateImage("BuyAccentBorder", buy,
            cardBorder, accentColor);
        buyBorder.type = Image.Type.Sliced;
        Stretch(buyBorder.rectTransform, 2f);
        buyBorder.transform.SetAsFirstSibling();
        LayoutElement buyLayout = GetOrAdd<LayoutElement>(buy.gameObject);
        buyLayout.ignoreLayout = true;
        TextMeshProUGUI buyLabel = buy.Find("Label")?.GetComponent<TextMeshProUGUI>();
        if (buyLabel != null)
        {
            buyLabel.fontSize = 20f;
            buyLabel.fontSizeMax = 20f;
            buyLabel.fontSizeMin = 13f;
            buyLabel.alignment = TextAlignmentOptions.Center;
        }

        VerticalTriangleArtifactCardUI controller =
            card.GetComponent<VerticalTriangleArtifactCardUI>();
        if (controller != null)
        {
            controller.roleText = roleText;
            EditorUtility.SetDirty(controller);
        }
        Image cardBackground = GetOrAdd<Image>(card.gameObject);
        cardBackground.sprite = moduleCard;
        cardBackground.type = Image.Type.Simple;
        cardBackground.color = new Color(0.82f, 0.86f, 0.88f, 1f);
        Image cardSubfloor = CreateImage("CardSubfloor", card,
            null, new Color(
                accentColor.r * 0.18f,
                accentColor.g * 0.15f,
                accentColor.b * 0.12f,
                0.13f));
        Stretch(cardSubfloor.rectTransform, 9f);
        cardSubfloor.transform.SetAsFirstSibling();
        Image infoDivider = CreateImage("InfoDivider", card, null,
            new Color(accentColor.r, accentColor.g, accentColor.b, 0.36f));
        infoDivider.rectTransform.anchorMin = new Vector2(0.08f, 0.455f);
        infoDivider.rectTransform.anchorMax = new Vector2(0.92f, 0.455f);
        infoDivider.rectTransform.anchoredPosition = Vector2.zero;
        infoDivider.rectTransform.sizeDelta = new Vector2(0f, 2f);
        infoDivider.gameObject.SetActive(false);
        Image outerBorder = CreateImage("CardAccentBorder", card,
            cardBorder, accentColor);
        outerBorder.type = Image.Type.Sliced;
        Stretch(outerBorder.rectTransform, 2f);
        outerBorder.transform.SetSiblingIndex(1);
        SetActive(card.Find("CardInnerPlate"), false);
    }

    private static void BuildNativeTriangleIcon(
        Transform circuit,
        Color accent,
        VerticalUiTheme theme)
    {
        GameObject root = GetOrCreateUi("PhaseTriangleIcon", circuit);
        RectTransform rect = (RectTransform)root.transform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, -36f);
        rect.sizeDelta = new Vector2(96f, 96f);

        foreach (Transform child in root.transform)
            child.gameObject.SetActive(false);
        TriangleCircuitDiagramGraphic diagram =
            GetOrAdd<TriangleCircuitDiagramGraphic>(root);
        diagram.color = accent;
        diagram.lineThickness = 3f;
        diagram.glowThickness = 7f;
        diagram.nodeRadius = 10.5f;
        diagram.nodeThickness = 3f;
        diagram.coreRadius = 2.4f;
        diagram.raycastTarget = false;
        diagram.SetVerticesDirty();
        root.SetActive(true);
    }

    private static void BuildPanelBolts(
        Transform panel,
        VerticalUiTheme theme)
    {
        Vector2[] anchors =
        {
            new(0.035f, 0.05f), new(0.965f, 0.05f),
            new(0.035f, 0.95f), new(0.965f, 0.95f)
        };
        for (int i = 0; i < anchors.Length; i++)
        {
            Image bolt = CreateImage("Bolt_" + i, panel,
                theme.softGlow, new Color(0.33f, 0.39f, 0.42f, 0.76f));
            bolt.rectTransform.anchorMin = anchors[i];
            bolt.rectTransform.anchorMax = anchors[i];
            bolt.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            bolt.rectTransform.anchoredPosition = Vector2.zero;
            bolt.rectTransform.sizeDelta = new Vector2(13f, 13f);
            bolt.preserveAspect = true;
        }
    }

    private static void StyleSidePanel(
        GameObject panel,
        Sprite sidePanel,
        VerticalUiTheme theme)
    {
        Image background = GetOrAdd<Image>(panel);
        background.sprite = sidePanel;
        background.type = Image.Type.Simple;
        background.color = Color.white;
        background.raycastTarget = false;
        SetActive(panel.transform.Find("InnerBorder"), false);
    }

    private static void AddSidePanelDamage(
        Transform panel,
        bool mirrored,
        VerticalUiTheme theme)
    {
        Vector2[] positions =
        {
            new(mirrored ? 34f : -34f, 250f),
            new(mirrored ? -30f : 30f, 82f),
            new(mirrored ? 36f : -36f, -118f),
            new(mirrored ? -28f : 28f, -292f)
        };
        float[] rotations = { 18f, -24f, 32f, -14f };
        for (int i = 0; i < positions.Length; i++)
        {
            Image scratch = CreateImage("DamageScratch_" + i, panel,
                null, new Color(0.48f, 0.52f, 0.52f, 0.20f));
            SetCentered(scratch.rectTransform, positions[i],
                new Vector2(34f, 2f));
            scratch.rectTransform.localRotation =
                Quaternion.Euler(0f, 0f, rotations[i]);
            scratch.transform.SetAsFirstSibling();
        }

        Image grime = CreateImage("DamageGrime", panel,
            theme.softGlow, new Color(0.01f, 0.012f, 0.014f, 0.34f));
        SetCentered(grime.rectTransform,
            new Vector2(mirrored ? 32f : -32f, -215f),
            new Vector2(58f, 84f));
        grime.transform.SetAsFirstSibling();
    }

    private static void HideLegacySidePanelDamage(Transform panel)
    {
        foreach (Transform child in panel)
            if (child.name.StartsWith("DamageScratch_", StringComparison.Ordinal) ||
                child.name == "DamageGrime")
                child.gameObject.SetActive(false);
    }

    private static void CreateSeparator(
        string name,
        Transform parent,
        float anchorY,
        VerticalUiTheme theme)
    {
        Image separator = CreateImage(name, parent, null,
            new Color(theme.border.r, theme.border.g, theme.border.b, 0.48f));
        separator.rectTransform.anchorMin = new Vector2(0.14f, anchorY);
        separator.rectTransform.anchorMax = new Vector2(0.86f, anchorY);
        separator.rectTransform.sizeDelta = new Vector2(0f, 2f);
        separator.rectTransform.anchoredPosition = Vector2.zero;
    }

    private static void ConfigureNode(
        Transform node,
        Vector2 position,
        Vector2 size,
        bool labelAbove)
    {
        SetCentered((RectTransform)node, position, size);
        Transform icon = node.Find("Icon");
        if (icon != null)
            SetCentered((RectTransform)icon, new Vector2(0f, 4f), size * 0.94f);
        TextMeshProUGUI label = node.Find("Label")?.GetComponent<TextMeshProUGUI>();
        if (label == null) return;
        label.fontSize = 20f;
        label.fontSizeMax = 20f;
        label.fontSizeMin = 15f;
        label.rectTransform.anchorMin = new Vector2(0f, labelAbove ? 1f : 0f);
        label.rectTransform.anchorMax = new Vector2(1f, labelAbove ? 1f : 0f);
        label.rectTransform.pivot = new Vector2(0.5f, labelAbove ? 0f : 1f);
        label.rectTransform.anchoredPosition = new Vector2(0f, labelAbove ? 4f : -6f);
        label.rectTransform.sizeDelta = new Vector2(40f, 34f);
    }

    private static void ConfigureBeam(
        Transform line,
        Vector2 from,
        Vector2 to,
        float thickness)
    {
        Require(line != null, "Falta una linea del Triangulo.");
        Vector2 delta = to - from;
        SetCentered((RectTransform)line, (from + to) * 0.5f,
            new Vector2(delta.magnitude, thickness));
        line.localEulerAngles = new Vector3(0f, 0f,
            Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
    }

    private static void BuildWarningMark(
        string name,
        Transform parent,
        Vector2 anchor)
    {
        GameObject root = GetOrCreateUi(name, parent);
        RectTransform rect = (RectTransform)root.transform;
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(42f, 42f);
        CreateLine("EdgeA", root.transform, new Vector2(-14f, -12f),
            new Vector2(0f, 15f), 3f, WarningRed);
        CreateLine("EdgeB", root.transform, new Vector2(0f, 15f),
            new Vector2(14f, -12f), 3f, WarningRed);
        CreateLine("EdgeC", root.transform, new Vector2(14f, -12f),
            new Vector2(-14f, -12f), 3f, WarningRed);
        Image stem = CreateImage("Stem", root.transform, null, WarningRed);
        SetCentered(stem.rectTransform, new Vector2(0f, 2f), new Vector2(3f, 12f));
        Image dot = CreateImage("Dot", root.transform, null, WarningRed);
        SetCentered(dot.rectTransform, new Vector2(0f, -8f), new Vector2(4f, 4f));
    }

    private static void BuildHazardStripes(
        string name,
        Transform parent,
        float anchorX,
        float direction)
    {
        GameObject root = GetOrCreateUi(name, parent);
        RectTransform rect = (RectTransform)root.transform;
        rect.anchorMin = new Vector2(anchorX, 0.5f);
        rect.anchorMax = new Vector2(anchorX, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(92f, 34f);
        for (int i = 0; i < 4; i++)
        {
            Image stripe = CreateImage("Stripe_" + i, root.transform, null,
                new Color(WarningRed.r, WarningRed.g, WarningRed.b, 0.75f));
            SetCentered(stripe.rectTransform,
                new Vector2(direction * (-30f + i * 20f), 0f),
                new Vector2(12f, 26f));
            stripe.rectTransform.localEulerAngles = new Vector3(0f, 0f, -28f);
        }
    }

    private static void CreateLine(
        string name,
        Transform parent,
        Vector2 from,
        Vector2 to,
        float thickness,
        Color color)
    {
        Image image = CreateImage(name, parent, null, color);
        Vector2 delta = to - from;
        SetCentered(image.rectTransform, (from + to) * 0.5f,
            new Vector2(delta.magnitude, thickness));
        image.rectTransform.localEulerAngles = new Vector3(0f, 0f,
            Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
    }

    private static Image CreateImage(
        string name,
        Transform parent,
        Sprite sprite,
        Color color)
    {
        GameObject go = GetOrCreateUi(name, parent);
        Image image = GetOrAdd<Image>(go);
        image.sprite = sprite;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static TextMeshProUGUI CreateText(
        string name,
        Transform parent,
        string value,
        float size,
        TextAlignmentOptions alignment,
        Color color,
        VerticalUiTheme theme)
    {
        GameObject go = GetOrCreateUi(name, parent);
        TextMeshProUGUI text = GetOrAdd<TextMeshProUGUI>(go);
        text.text = value;
        text.font = theme.primaryFont != null ? theme.primaryFont : TMP_Settings.defaultFontAsset;
        text.fontSize = size;
        text.fontSizeMax = size;
        text.fontSizeMin = Mathf.Max(10f, size - 8f);
        text.enableAutoSizing = true;
        text.alignment = alignment;
        text.color = color;
        text.raycastTarget = false;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.margin = new Vector4(2f, 2f, 2f, 2f);
        return text;
    }

    private static GameObject GetOrCreateUi(string name, Transform parent)
    {
        Transform existing = parent.Find(name);
        if (existing != null) return existing.gameObject;
        GameObject go = new(name, typeof(RectTransform), typeof(CanvasRenderer));
        go.layer = 5;
        go.transform.SetParent(parent, false);
        return go;
    }

    private static T GetOrAdd<T>(GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();
        return component != null ? component : go.AddComponent<T>();
    }

    private static void SetActive(Transform target, bool active)
    {
        if (target != null) target.gameObject.SetActive(active);
    }

    private static void SetTopRect(
        RectTransform rect,
        float top,
        float height,
        float left,
        float right)
    {
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2((left + right) * 0.5f, -top);
        rect.sizeDelta = new Vector2(right - left, height);
    }

    private static void SetCentered(
        RectTransform rect,
        Vector2 position,
        Vector2 size)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    private static void SetAnchors(RectTransform rect, Vector2 min, Vector2 max)
    {
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void Stretch(RectTransform rect, float inset)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(inset, inset);
        rect.offsetMax = new Vector2(-inset, -inset);
    }

    private static void ApplyFont(Transform root, VerticalUiTheme theme)
    {
        if (root == null || theme == null || theme.primaryFont == null) return;
        foreach (TextMeshProUGUI text in root.GetComponentsInChildren<TextMeshProUGUI>(true))
            text.font = theme.primaryFont;
    }

    private static Transform FindUnique(Scene scene, string name)
    {
        Transform found = null;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
            {
                if (current.name != name) continue;
                if (found != null)
                    throw new InvalidOperationException(name + " esta duplicado.");
                found = current;
            }
        }
        return found;
    }

    private static TextMeshProUGUI FindText(Scene scene, string value)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
            foreach (TextMeshProUGUI text in root.GetComponentsInChildren<TextMeshProUGUI>(true))
                if (string.Equals(text.text?.Trim(), value,
                    StringComparison.OrdinalIgnoreCase))
                    return text;
        return null;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
#endif

#if UNITY_EDITOR
using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class VerticalUiBlock2Setup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string GeneratedFolder = "Assets/Project/UI/Vertical/Generated";
    private const string ThemePath = GeneratedFolder + "/VerticalUiTheme.asset";
    private const string ResearchPrefabPath =
        "Assets/Project/Prefabs/UI/ResearchItem.prefab";

    [MenuItem("Tools/Quantum Forge/Vertical UI/Configure Block 2 Navigation")]
    public static void ConfigureBlock2Navigation()
    {
        GenerateNavigationIcons();
        VerticalUiTheme theme = LoadAndUpdateTheme();
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        TabsUI tabs = UnityEngine.Object.FindFirstObjectByType<TabsUI>(
            FindObjectsInactive.Include);
        VerticalUiSkinRoot skin = UnityEngine.Object.FindFirstObjectByType<VerticalUiSkinRoot>(
            FindObjectsInactive.Include);
        VerticalSafeAreaLayout safeLayout =
            UnityEngine.Object.FindFirstObjectByType<VerticalSafeAreaLayout>(
                FindObjectsInactive.Include);
        if (tabs == null || skin == null || safeLayout == null ||
            safeLayout.contentSlot == null ||
            safeLayout.primaryNavigationSlot == null ||
            safeLayout.secondaryNavigationSlot == null)
        {
            throw new InvalidOperationException(
                "El Bloque 1 vertical debe estar configurado antes del Bloque 2.");
        }

        Transform content = safeLayout.contentSlot;
        MoveManagedPanels(tabs, content);
        GameObject improvementsPanel = BuildImprovementsPanel(tabs, content);
        BuildResearchPanel(tabs.panelLab, theme);
        GameObject settingsPanel = BuildSettingsPanel(content, theme);

        BuildPrimaryNavigation(safeLayout.primaryNavigationSlot, theme,
            out Button generation,
            out Button upgrades,
            out Button research,
            out Button settings,
            out Button qa);
        BuildSecondaryNavigation(safeLayout.secondaryNavigationSlot, theme,
            out ScrollRect secondaryScroll,
            out Button room2,
            out Button dimension1,
            out Button dimension2,
            out Button dimension3,
            out Button prestige);

        VerticalNavigationUI navigation = GetOrAdd<VerticalNavigationUI>(skin.gameObject);
        navigation.tabs = tabs;
        navigation.safeAreaLayout = safeLayout;
        navigation.theme = theme;
        navigation.primaryNavigationRoot = safeLayout.primaryNavigationSlot;
        navigation.secondaryNavigationRoot = safeLayout.secondaryNavigationSlot;
        navigation.secondaryScroll = secondaryScroll;
        navigation.generationButton = generation;
        navigation.upgradesButton = upgrades;
        navigation.researchButton = research;
        navigation.settingsButton = settings;
        navigation.qaButton = qa;
        navigation.room2Button = room2;
        navigation.dimension1Button = dimension1;
        navigation.dimension2Button = dimension2;
        navigation.dimension3Button = dimension3;
        navigation.prestigeButton = prestige;

        QaPanelUI qaPanel = UnityEngine.Object.FindFirstObjectByType<QaPanelUI>(
            FindObjectsInactive.Include);
        ConfigureTabs(tabs, navigation, improvementsPanel, settingsPanel,
            generation, upgrades, research, settings, qa,
            room2, dimension1, dimension2, dimension3, prestige, qaPanel);
        ConfigureQaAccess(scene, qaPanel, qa);
        DisableLegacyNavigation(scene);
        OrderOverlayRoots(scene, skin.gameObject);

        room2.gameObject.SetActive(false);
        dimension1.gameObject.SetActive(false);
        dimension2.gameObject.SetActive(false);
        dimension3.gameObject.SetActive(false);
        prestige.gameObject.SetActive(false);
        safeLayout.secondaryNavigationSlot.gameObject.SetActive(false);
        safeLayout.primaryNavigationSlot.gameObject.SetActive(true);
        safeLayout.ApplyLayout();

        EditorUtility.SetDirty(theme);
        EditorUtility.SetDirty(navigation);
        EditorUtility.SetDirty(tabs);
        if (qaPanel != null)
            EditorUtility.SetDirty(qaPanel);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("No se pudo guardar Main.unity.");
        AssetDatabase.SaveAssets();

        Debug.Log("[Vertical UI Block 2] CONFIGURED | main navigation 5 | " +
            "progressive systems | upgrades separated | settings | QA gated");
    }

    public static void ConfigureAndValidateBatch()
    {
        VerticalUiBlock1Setup.ConfigureBlock1Base();
        ConfigureBlock2Navigation();
        ConfigureBlock2Navigation();
        VerticalUiBlock2Validation.Validate();
        MobileQaFriendlyLayoutValidation.Validate();
        MobileButtonLegibilityValidation.Validate();
        QaMainSceneIntegrityValidation.ValidateMainSceneIntegrity();
        Debug.Log("[Vertical UI Block 2] IDEMPOTENCE PASS | setup executed twice");
    }

    private static void MoveManagedPanels(TabsUI tabs, Transform content)
    {
        MoveAndStretch(tabs.panelGeneracion, content);
        MoveAndStretch(tabs.panelLab, content);
        MoveAndStretch(tabs.room2Panel, content);
        MoveAndStretch(tabs.dimension1Panel, content);
        MoveAndStretch(tabs.dimension2Panel, content);
        MoveAndStretch(tabs.dimension3Panel, content);
        MoveAndStretch(tabs.panelLogros, content);
        MoveAndStretch(tabs.prestigePanel, content);

        GameObject metaPrestige = FindInScene(SceneManager.GetActiveScene(),
            "MetaPrestigePanel");
        MoveAndStretch(metaPrestige, content);
    }

    private static GameObject BuildImprovementsPanel(TabsUI tabs, Transform content)
    {
        GameObject panel = GetOrCreateUiObject("Panel_Mejoras", content);
        Stretch((RectTransform)panel.transform);

        GameObject f2Panel = FindInScene(SceneManager.GetActiveScene(), "F2Panel");
        if (f2Panel == null)
            throw new InvalidOperationException("Falta F2Panel en Main.unity.");
        f2Panel.transform.SetParent(panel.transform, false);
        Stretch((RectTransform)f2Panel.transform);
        ResearchUI legacyResearch = f2Panel.GetComponent<ResearchUI>();
        if (legacyResearch != null)
            legacyResearch.enabled = false;

        panel.SetActive(false);
        tabs.panelMejoras = panel;
        return panel;
    }

    private static void BuildResearchPanel(GameObject panel, VerticalUiTheme theme)
    {
        if (panel == null)
            throw new InvalidOperationException("Falta Panel_Lab para Investigacion.");
        Stretch((RectTransform)panel.transform);

        GameObject shell = GetOrCreateUiObject("VerticalResearchShell", panel.transform);
        Stretch((RectTransform)shell.transform);
        Image shellBackground = GetOrAdd<Image>(shell);
        shellBackground.color = new Color(0f, 0f, 0f, 0f);
        shellBackground.raycastTarget = false;

        TextMeshProUGUI title = CreateOrUpdateText("Title", shell.transform,
            "INVESTIGACIÓN", "research.title", 38f,
            TextAlignmentOptions.Center, theme.primaryText);
        RectTransform titleRect = title.rectTransform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -20f);
        titleRect.sizeDelta = new Vector2(0f, 76f);

        ScrollRect scroll = GetOrCreateVerticalScroll(
            "ResearchScroll", shell.transform, theme,
            out RectTransform content);
        RectTransform scrollRect = scroll.GetComponent<RectTransform>();
        scrollRect.anchorMin = Vector2.zero;
        scrollRect.anchorMax = Vector2.one;
        scrollRect.offsetMin = new Vector2(16f, 16f);
        scrollRect.offsetMax = new Vector2(-16f, -108f);

        ResearchUI research = GetOrAdd<ResearchUI>(panel);
        research.listContainer = content;
        research.itemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ResearchPrefabPath);
        research.enabled = true;
        EditorUtility.SetDirty(research);
        panel.SetActive(false);
    }

    private static GameObject BuildSettingsPanel(Transform content, VerticalUiTheme theme)
    {
        GameObject panel = GetOrCreateUiObject("Panel_Ajustes", content);
        Stretch((RectTransform)panel.transform);

        TextMeshProUGUI title = CreateOrUpdateText("SettingsTitle", panel.transform,
            "AJUSTES", "settings.title", 42f,
            TextAlignmentOptions.Center, theme.primaryText);
        ConfigureAnchored(title.rectTransform, new Vector2(0.5f, 0.90f),
            new Vector2(820f, 90f));

        GameObject card = GetOrCreateUiObject("LanguageCard", panel.transform);
        ConfigureAnchored((RectTransform)card.transform,
            new Vector2(0.5f, 0.64f), new Vector2(820f, 420f));
        Image cardImage = GetOrAdd<Image>(card);
        cardImage.sprite = theme.panelFrame;
        cardImage.type = Image.Type.Sliced;
        cardImage.color = Color.white;

        TextMeshProUGUI languageTitle = CreateOrUpdateText(
            "LanguageTitle", card.transform, "IDIOMA", "settings.language.title",
            30f, TextAlignmentOptions.Center, theme.energy);
        ConfigureAnchored(languageTitle.rectTransform,
            new Vector2(0.5f, 0.80f), new Vector2(650f, 62f));

        TextMeshProUGUI current = CreateOrUpdateText(
            "CurrentLanguage", card.transform, "Idioma actual: Español", "",
            24f, TextAlignmentOptions.Center, theme.primaryText);
        ConfigureAnchored(current.rectTransform,
            new Vector2(0.5f, 0.59f), new Vector2(650f, 60f));

        Button spanish = CreateOrUpdateButton("SpanishButton", card.transform,
            "ESPAÑOL", "settings.language.spanish", theme.settingsIcon,
            theme, false);
        ConfigureAnchored((RectTransform)spanish.transform,
            new Vector2(0.30f, 0.29f), new Vector2(280f, 96f));
        Button english = CreateOrUpdateButton("EnglishButton", card.transform,
            "INGLÉS", "settings.language.english", theme.settingsIcon,
            theme, false);
        ConfigureAnchored((RectTransform)english.transform,
            new Vector2(0.70f, 0.29f), new Vector2(280f, 96f));

        VerticalSettingsPanelUI settings = GetOrAdd<VerticalSettingsPanelUI>(panel);
        settings.spanishButton = spanish;
        settings.englishButton = english;
        settings.currentLanguageText = current;
        EditorUtility.SetDirty(settings);
        panel.SetActive(false);
        return panel;
    }

    private static void BuildPrimaryNavigation(
        RectTransform root,
        VerticalUiTheme theme,
        out Button generation,
        out Button upgrades,
        out Button research,
        out Button settings,
        out Button qa)
    {
        Image background = GetOrAdd<Image>(root.gameObject);
        background.sprite = theme.panelFrame;
        background.type = Image.Type.Sliced;
        background.color = Color.white;
        background.raycastTarget = false;

        HorizontalLayoutGroup group = GetOrAdd<HorizontalLayoutGroup>(root.gameObject);
        group.padding = new RectOffset(12, 12, 10, 10);
        group.spacing = 10f;
        group.childAlignment = TextAnchor.MiddleCenter;
        group.childControlWidth = true;
        group.childControlHeight = true;
        group.childForceExpandWidth = true;
        group.childForceExpandHeight = true;

        generation = CreateOrUpdateButton("Nav_Generation", root,
            "GENERACIÓN", "nav.generation", theme.generationIcon, theme, true);
        upgrades = CreateOrUpdateButton("Nav_Upgrades", root,
            "MEJORAS", "nav.upgrades", theme.upgradesIcon, theme, true);
        research = CreateOrUpdateButton("Nav_Research", root,
            "INVESTIGACIÓN", "nav.research", theme.researchIcon, theme, true);
        settings = CreateOrUpdateButton("Nav_Settings", root,
            "AJUSTES", "nav.settings", theme.settingsIcon, theme, true);
        qa = CreateOrUpdateButton("Nav_QA", root,
            "QA", "nav.qa", theme.qaIcon, theme, true);

        SetSiblingOrder(generation, upgrades, research, settings, qa);
        root.gameObject.SetActive(true);
    }

    private static void BuildSecondaryNavigation(
        RectTransform root,
        VerticalUiTheme theme,
        out ScrollRect scroll,
        out Button room2,
        out Button dimension1,
        out Button dimension2,
        out Button dimension3,
        out Button prestige)
    {
        Image background = GetOrAdd<Image>(root.gameObject);
        background.sprite = theme.panelFrame;
        background.type = Image.Type.Sliced;
        background.color = Color.white;
        background.raycastTarget = false;

        TextMeshProUGUI leftHint = CreateOrUpdateText("ScrollHintLeft", root,
            "‹", "", 42f, TextAlignmentOptions.Center, theme.secondaryText);
        ConfigureSideHint(leftHint.rectTransform, false);
        TextMeshProUGUI rightHint = CreateOrUpdateText("ScrollHintRight", root,
            "›", "", 42f, TextAlignmentOptions.Center, theme.secondaryText);
        ConfigureSideHint(rightHint.rectTransform, true);

        GameObject scrollObject = GetOrCreateUiObject("SecondaryScroll", root);
        RectTransform scrollTransform = (RectTransform)scrollObject.transform;
        Stretch(scrollTransform);
        scrollTransform.offsetMin = new Vector2(42f, 8f);
        scrollTransform.offsetMax = new Vector2(-42f, -8f);
        scroll = GetOrAdd<ScrollRect>(scrollObject);
        scroll.horizontal = true;
        scroll.vertical = false;
        scroll.movementType = ScrollRect.MovementType.Elastic;
        scroll.scrollSensitivity = 34f;

        GameObject viewport = GetOrCreateUiObject("Viewport", scrollObject.transform);
        Stretch((RectTransform)viewport.transform);
        Image viewportImage = GetOrAdd<Image>(viewport);
        viewportImage.color = new Color(1f, 1f, 1f, 0.001f);
        viewportImage.raycastTarget = true;
        GetOrAdd<RectMask2D>(viewport);

        GameObject contentObject = GetOrCreateUiObject("Content", viewport.transform);
        RectTransform content = (RectTransform)contentObject.transform;
        content.anchorMin = new Vector2(0f, 0f);
        content.anchorMax = new Vector2(0f, 1f);
        content.pivot = new Vector2(0f, 0.5f);
        content.anchoredPosition = Vector2.zero;
        content.sizeDelta = Vector2.zero;
        HorizontalLayoutGroup group = GetOrAdd<HorizontalLayoutGroup>(contentObject);
        group.padding = new RectOffset(4, 4, 2, 2);
        group.spacing = 10f;
        group.childAlignment = TextAnchor.MiddleLeft;
        group.childControlWidth = true;
        group.childControlHeight = true;
        group.childForceExpandWidth = false;
        group.childForceExpandHeight = true;
        ContentSizeFitter fitter = GetOrAdd<ContentSizeFitter>(contentObject);
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        scroll.viewport = (RectTransform)viewport.transform;
        scroll.content = content;

        room2 = CreateOrUpdateButton("System_Room2", content,
            "CUARTO 2", "nav.room2", theme.room2Icon, theme, false);
        dimension1 = CreateOrUpdateButton("System_Dimension1", content,
            "DIM. 1", "nav.dimension1", theme.dimension1Icon, theme, false);
        dimension2 = CreateOrUpdateButton("System_Dimension2", content,
            "DIM. 2", "nav.dimension2", theme.dimension2Icon, theme, false);
        dimension3 = CreateOrUpdateButton("System_Dimension3", content,
            "DIM. 3", "nav.dimension3", theme.dimension3Icon, theme, false);
        prestige = CreateOrUpdateButton("System_Prestige", content,
            "PRESTIGIO", "", theme.prestigeIcon, theme, false);
        SetSiblingOrder(room2, dimension1, dimension2, dimension3, prestige);
    }

    private static void ConfigureTabs(
        TabsUI tabs,
        VerticalNavigationUI navigation,
        GameObject improvementsPanel,
        GameObject settingsPanel,
        Button generation,
        Button upgrades,
        Button research,
        Button settings,
        Button qa,
        Button room2,
        Button dimension1,
        Button dimension2,
        Button dimension3,
        Button prestige,
        QaPanelUI qaPanel)
    {
        tabs.btnGeneracion = generation;
        tabs.btnMejoras = upgrades;
        tabs.btnLab = research;
        tabs.btnAjustes = settings;
        tabs.btnQA = qa;
        tabs.btnRoom2 = room2;
        tabs.btnDimension1 = dimension1;
        tabs.btnDimension2 = dimension2;
        tabs.btnDimension3 = dimension3;
        tabs.btnPrestigio = prestige;
        tabs.panelMejoras = improvementsPanel;
        tabs.panelAjustes = settingsPanel;
        tabs.verticalNavigation = navigation;
        tabs.qaPanel = qaPanel;
    }

    private static void ConfigureQaAccess(
        Scene scene, QaPanelUI qaPanel, Button qaButton)
    {
        GameObject legacy = FindInScene(scene, "QA_ToolsButton");
        if (legacy != null && legacy != qaButton.gameObject)
            legacy.SetActive(false);
        if (qaPanel != null)
            qaPanel.toolsButton = qaButton;
    }

    private static void DisableLegacyNavigation(Scene scene)
    {
        GameObject legacyRoot = FindInScene(scene, "MobileSafeAreaRoot");
        if (legacyRoot != null)
            legacyRoot.SetActive(false);
    }

    private static void OrderOverlayRoots(Scene scene, GameObject verticalRoot)
    {
        verticalRoot.transform.SetAsLastSibling();
        GameObject popup = FindInScene(scene, "Panel_AchievementPopup");
        if (popup != null)
        {
            popup.transform.SetParent(verticalRoot.transform, false);
            popup.transform.SetAsLastSibling();
        }
        GameObject qaSafeArea = FindInScene(scene, "QA_SafeArea");
        if (qaSafeArea != null)
            qaSafeArea.transform.SetAsLastSibling();
    }

    private static ScrollRect GetOrCreateVerticalScroll(
        string name,
        Transform parent,
        VerticalUiTheme theme,
        out RectTransform content)
    {
        GameObject scrollObject = GetOrCreateUiObject(name, parent);
        ScrollRect scroll = GetOrAdd<ScrollRect>(scrollObject);
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Elastic;
        scroll.scrollSensitivity = 36f;

        GameObject viewport = GetOrCreateUiObject("Viewport", scrollObject.transform);
        Stretch((RectTransform)viewport.transform);
        Image viewportImage = GetOrAdd<Image>(viewport);
        viewportImage.color = new Color(1f, 1f, 1f, 0.001f);
        viewportImage.raycastTarget = true;
        GetOrAdd<RectMask2D>(viewport);

        GameObject contentObject = GetOrCreateUiObject("Content", viewport.transform);
        content = (RectTransform)contentObject.transform;
        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = new Vector2(1f, 1f);
        content.pivot = new Vector2(0.5f, 1f);
        content.anchoredPosition = Vector2.zero;
        content.sizeDelta = Vector2.zero;
        VerticalLayoutGroup group = GetOrAdd<VerticalLayoutGroup>(contentObject);
        group.padding = new RectOffset(12, 12, 12, 12);
        group.spacing = 12f;
        group.childAlignment = TextAnchor.UpperCenter;
        group.childControlWidth = true;
        group.childControlHeight = true;
        group.childForceExpandWidth = true;
        group.childForceExpandHeight = false;
        ContentSizeFitter fitter = GetOrAdd<ContentSizeFitter>(contentObject);
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        scroll.viewport = (RectTransform)viewport.transform;
        scroll.content = content;
        return scroll;
    }

    private static Button CreateOrUpdateButton(
        string name,
        Transform parent,
        string labelText,
        string localizationKey,
        Sprite icon,
        VerticalUiTheme theme,
        bool primary)
    {
        GameObject buttonObject = GetOrCreateUiObject(name, parent);
        Image background = GetOrAdd<Image>(buttonObject);
        background.sprite = theme.buttonFrame;
        background.type = Image.Type.Sliced;
        background.color = Color.white;
        Button button = GetOrAdd<Button>(buttonObject);
        button.targetGraphic = background;
        button.onClick = new Button.ButtonClickedEvent();

        LayoutElement layout = GetOrAdd<LayoutElement>(buttonObject);
        layout.minWidth = primary ? 0f : 160f;
        layout.preferredWidth = primary ? 190f : 170f;
        layout.minHeight = primary ? 100f : 76f;
        layout.preferredHeight = primary ? 112f : 86f;
        layout.flexibleWidth = primary ? 1f : 0f;
        layout.flexibleHeight = 1f;

        GameObject iconObject = GetOrCreateUiObject("Icon", buttonObject.transform);
        Image iconImage = GetOrAdd<Image>(iconObject);
        iconImage.sprite = icon;
        iconImage.preserveAspect = true;
        iconImage.raycastTarget = false;
        RectTransform iconRect = (RectTransform)iconObject.transform;
        iconRect.anchorMin = new Vector2(0.5f, primary ? 0.54f : 0.5f);
        iconRect.anchorMax = iconRect.anchorMin;
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.anchoredPosition = primary
            ? new Vector2(0f, 12f)
            : new Vector2(-58f, 0f);
        iconRect.sizeDelta = primary ? new Vector2(48f, 48f) : new Vector2(46f, 46f);

        TextMeshProUGUI label = CreateOrUpdateText("Label", buttonObject.transform,
            labelText, localizationKey, primary ? 17f : 18f,
            TextAlignmentOptions.Center, theme.primaryText);
        RectTransform labelRect = label.rectTransform;
        if (primary)
        {
            labelRect.anchorMin = new Vector2(0.04f, 0f);
            labelRect.anchorMax = new Vector2(0.96f, 0.42f);
            labelRect.offsetMin = new Vector2(2f, 5f);
            labelRect.offsetMax = new Vector2(-2f, -2f);
        }
        else
        {
            labelRect.anchorMin = new Vector2(0.36f, 0f);
            labelRect.anchorMax = new Vector2(0.96f, 1f);
            labelRect.offsetMin = new Vector2(0f, 6f);
            labelRect.offsetMax = new Vector2(-4f, -6f);
        }
        return button;
    }

    private static TextMeshProUGUI CreateOrUpdateText(
        string name,
        Transform parent,
        string text,
        string localizationKey,
        float fontSize,
        TextAlignmentOptions alignment,
        Color color)
    {
        GameObject textObject = GetOrCreateUiObject(name, parent);
        TextMeshProUGUI label = GetOrAdd<TextMeshProUGUI>(textObject);
        label.text = text;
        label.fontSize = fontSize;
        label.enableAutoSizing = true;
        label.fontSizeMin = 10f;
        label.fontSizeMax = fontSize;
        label.alignment = alignment;
        label.color = color;
        label.textWrappingMode = TextWrappingModes.Normal;
        label.raycastTarget = false;
        label.margin = new Vector4(4f, 4f, 4f, 4f);
        LocalizedTMP localized = GetOrAdd<LocalizedTMP>(textObject);
        localized.key = localizationKey ?? string.Empty;
        EditorUtility.SetDirty(localized);
        return label;
    }

    private static void GenerateNavigationIcons()
    {
        CreateIcon("qf_icon_generation.png", IconKind.Atom);
        CreateIcon("qf_icon_upgrades.png", IconKind.Upgrades);
        CreateIcon("qf_icon_research.png", IconKind.Research);
        CreateIcon("qf_icon_settings.png", IconKind.Settings);
        CreateIcon("qf_icon_qa.png", IconKind.Qa);
        CreateIcon("qf_icon_room2.png", IconKind.Room2);
        CreateIcon("qf_icon_dimension1.png", IconKind.Dimension1);
        CreateIcon("qf_icon_dimension2.png", IconKind.Dimension2);
        CreateIcon("qf_icon_dimension3.png", IconKind.Dimension3);
        CreateIcon("qf_icon_prestige.png", IconKind.Prestige);
    }

    private static VerticalUiTheme LoadAndUpdateTheme()
    {
        VerticalUiTheme theme = AssetDatabase.LoadAssetAtPath<VerticalUiTheme>(ThemePath);
        if (theme == null)
            throw new InvalidOperationException("Falta VerticalUiTheme del Bloque 1.");
        theme.generationIcon = LoadIcon("qf_icon_generation.png");
        theme.upgradesIcon = LoadIcon("qf_icon_upgrades.png");
        theme.researchIcon = LoadIcon("qf_icon_research.png");
        theme.settingsIcon = LoadIcon("qf_icon_settings.png");
        theme.qaIcon = LoadIcon("qf_icon_qa.png");
        theme.room2Icon = LoadIcon("qf_icon_room2.png");
        theme.dimension1Icon = LoadIcon("qf_icon_dimension1.png");
        theme.dimension2Icon = LoadIcon("qf_icon_dimension2.png");
        theme.dimension3Icon = LoadIcon("qf_icon_dimension3.png");
        theme.prestigeIcon = LoadIcon("qf_icon_prestige.png");
        EditorUtility.SetDirty(theme);
        return theme;
    }

    private static Sprite LoadIcon(string name)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(GeneratedFolder + "/" + name);
        if (sprite == null)
            throw new InvalidOperationException("No se pudo cargar " + name + ".");
        return sprite;
    }

    private enum IconKind
    {
        Atom, Upgrades, Research, Settings, Qa,
        Room2, Dimension1, Dimension2, Dimension3, Prestige
    }

    private static void CreateIcon(string fileName, IconKind kind)
    {
        const int size = 128;
        var pixels = new Color32[size * size];
        Color32 ink = new(240, 248, 255, 255);
        Vector2 center = new(63.5f, 63.5f);

        if (kind == IconKind.Atom)
        {
            DrawEllipse(pixels, size, center, 44f, 18f, 0f, 3f, ink);
            DrawEllipse(pixels, size, center, 44f, 18f, 60f, 3f, ink);
            DrawEllipse(pixels, size, center, 44f, 18f, -60f, 3f, ink);
            DrawCircle(pixels, size, center, 6f, 6f, ink);
        }
        else if (kind == IconKind.Upgrades)
        {
            DrawChevron(pixels, size, 34f, ink);
            DrawChevron(pixels, size, 58f, ink);
        }
        else if (kind == IconKind.Research)
        {
            DrawLine(pixels, size, new Vector2(43, 29), new Vector2(76, 66), 7f, ink);
            DrawLine(pixels, size, new Vector2(74, 63), new Vector2(55, 83), 7f, ink);
            DrawCircle(pixels, size, new Vector2(40, 27), 11f, 5f, ink);
            DrawLine(pixels, size, new Vector2(54, 82), new Vector2(86, 82), 7f, ink);
            DrawLine(pixels, size, new Vector2(39, 101), new Vector2(91, 101), 7f, ink);
            DrawLine(pixels, size, new Vector2(62, 82), new Vector2(51, 101), 7f, ink);
        }
        else if (kind == IconKind.Settings || kind == IconKind.Dimension3)
        {
            DrawCircle(pixels, size, center, 28f, 6f, ink);
            DrawCircle(pixels, size, center, 10f, 6f, ink);
            for (int i = 0; i < 8; i++)
            {
                float angle = i * Mathf.PI / 4f;
                DrawLine(pixels, size,
                    center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 31f,
                    center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 46f,
                    8f, ink);
            }
            if (kind == IconKind.Dimension3)
                DrawLine(pixels, size, new Vector2(22, 108), new Vector2(106, 108), 6f, ink);
        }
        else if (kind == IconKind.Qa)
        {
            DrawLine(pixels, size, new Vector2(30, 28), new Vector2(98, 100), 9f, ink);
            DrawLine(pixels, size, new Vector2(98, 28), new Vector2(30, 100), 9f, ink);
            DrawCircle(pixels, size, new Vector2(29, 27), 10f, 5f, ink);
            DrawCircle(pixels, size, new Vector2(99, 27), 10f, 5f, ink);
        }
        else if (kind == IconKind.Room2)
        {
            DrawLine(pixels, size, new Vector2(34, 105), new Vector2(34, 52), 7f, ink);
            DrawLine(pixels, size, new Vector2(94, 105), new Vector2(94, 52), 7f, ink);
            DrawArc(pixels, size, center + new Vector2(0, -12), 30f, 0f, 180f, 7f, ink);
            DrawLine(pixels, size, new Vector2(25, 106), new Vector2(103, 106), 7f, ink);
            DrawCircle(pixels, size, new Vector2(80, 78), 4f, 4f, ink);
        }
        else if (kind == IconKind.Dimension1)
        {
            DrawCircle(pixels, size, center, 31f, 5f, ink);
            DrawEllipse(pixels, size, center, 52f, 14f, -22f, 4f, ink);
            DrawCircle(pixels, size, new Vector2(51, 50), 5f, 5f, ink);
        }
        else if (kind == IconKind.Dimension2)
        {
            Vector2[] nodes =
            {
                new(28, 67), new(49, 31), new(81, 39),
                new(99, 72), new(67, 99), new(42, 91)
            };
            for (int i = 0; i < nodes.Length; i++)
            {
                DrawLine(pixels, size, nodes[i], nodes[(i + 1) % nodes.Length], 4f, ink);
                DrawCircle(pixels, size, nodes[i], 7f, 5f, ink);
            }
            DrawLine(pixels, size, nodes[0], nodes[3], 3f, ink);
            DrawLine(pixels, size, nodes[1], nodes[4], 3f, ink);
        }
        else if (kind == IconKind.Prestige)
        {
            Vector2[] outer = new Vector2[10];
            for (int i = 0; i < outer.Length; i++)
            {
                float radius = i % 2 == 0 ? 47f : 23f;
                float angle = (-90f + i * 36f) * Mathf.Deg2Rad;
                outer[i] = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            }
            for (int i = 0; i < outer.Length; i++)
                DrawLine(pixels, size, outer[i], outer[(i + 1) % outer.Length], 5f, ink);
            DrawCircle(pixels, size, center, 12f, 4f, ink);
        }

        SaveIcon(fileName, pixels, size);
    }

    private static void DrawChevron(Color32[] pixels, int size, float y, Color32 color)
    {
        DrawLine(pixels, size, new Vector2(28, y + 24), new Vector2(64, y), 7f, color);
        DrawLine(pixels, size, new Vector2(64, y), new Vector2(100, y + 24), 7f, color);
    }

    private static void DrawEllipse(
        Color32[] pixels, int size, Vector2 center,
        float radiusX, float radiusY, float degrees, float thickness, Color32 color)
    {
        Vector2 previous = Vector2.zero;
        for (int i = 0; i <= 96; i++)
        {
            float angle = i * Mathf.PI * 2f / 96f;
            Vector2 point = new(Mathf.Cos(angle) * radiusX, Mathf.Sin(angle) * radiusY);
            float rotation = degrees * Mathf.Deg2Rad;
            point = new Vector2(
                point.x * Mathf.Cos(rotation) - point.y * Mathf.Sin(rotation),
                point.x * Mathf.Sin(rotation) + point.y * Mathf.Cos(rotation));
            point += center;
            if (i > 0)
                DrawLine(pixels, size, previous, point, thickness, color);
            previous = point;
        }
    }

    private static void DrawArc(
        Color32[] pixels, int size, Vector2 center, float radius,
        float startDegrees, float endDegrees, float thickness, Color32 color)
    {
        Vector2 previous = Vector2.zero;
        for (int i = 0; i <= 48; i++)
        {
            float degrees = Mathf.Lerp(startDegrees, endDegrees, i / 48f);
            float radians = degrees * Mathf.Deg2Rad;
            Vector2 point = center + new Vector2(
                Mathf.Cos(radians), Mathf.Sin(radians)) * radius;
            if (i > 0)
                DrawLine(pixels, size, previous, point, thickness, color);
            previous = point;
        }
    }

    private static void DrawCircle(
        Color32[] pixels, int size, Vector2 center,
        float radius, float thickness, Color32 color)
    {
        float inner = Mathf.Max(0f, radius - thickness * 0.5f);
        float outer = radius + thickness * 0.5f;
        int minX = Mathf.Max(0, Mathf.FloorToInt(center.x - outer));
        int maxX = Mathf.Min(size - 1, Mathf.CeilToInt(center.x + outer));
        int minY = Mathf.Max(0, Mathf.FloorToInt(center.y - outer));
        int maxY = Mathf.Min(size - 1, Mathf.CeilToInt(center.y + outer));
        for (int y = minY; y <= maxY; y++)
            for (int x = minX; x <= maxX; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                if (distance >= inner && distance <= outer)
                    pixels[y * size + x] = color;
            }
    }

    private static void DrawLine(
        Color32[] pixels, int size, Vector2 a, Vector2 b,
        float thickness, Color32 color)
    {
        float minX = Mathf.Min(a.x, b.x) - thickness;
        float maxX = Mathf.Max(a.x, b.x) + thickness;
        float minY = Mathf.Min(a.y, b.y) - thickness;
        float maxY = Mathf.Max(a.y, b.y) + thickness;
        for (int y = Mathf.Max(0, Mathf.FloorToInt(minY));
             y <= Mathf.Min(size - 1, Mathf.CeilToInt(maxY)); y++)
        {
            for (int x = Mathf.Max(0, Mathf.FloorToInt(minX));
                 x <= Mathf.Min(size - 1, Mathf.CeilToInt(maxX)); x++)
            {
                if (DistanceToSegment(new Vector2(x, y), a, b) <= thickness * 0.5f)
                    pixels[y * size + x] = color;
            }
        }
    }

    private static float DistanceToSegment(Vector2 point, Vector2 a, Vector2 b)
    {
        Vector2 delta = b - a;
        float lengthSquared = delta.sqrMagnitude;
        if (lengthSquared <= 0.0001f)
            return Vector2.Distance(point, a);
        float t = Mathf.Clamp01(Vector2.Dot(point - a, delta) / lengthSquared);
        return Vector2.Distance(point, a + delta * t);
    }

    private static void SaveIcon(string fileName, Color32[] pixels, int size)
    {
        string assetPath = GeneratedFolder + "/" + fileName;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false, false);
        texture.SetPixels32(pixels);
        texture.Apply(false, false);
        byte[] png = texture.EncodeToPNG();
        UnityEngine.Object.DestroyImmediate(texture);
        string absolute = Path.GetFullPath(assetPath);
        if (!File.Exists(absolute) || !BytesEqual(File.ReadAllBytes(absolute), png))
            File.WriteAllBytes(absolute, png);
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 100f;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Bilinear;
        importer.textureCompression = TextureImporterCompression.Compressed;
        importer.maxTextureSize = 256;
        importer.SetPlatformTextureSettings(new TextureImporterPlatformSettings
        {
            name = "Android",
            overridden = true,
            maxTextureSize = 256,
            format = TextureImporterFormat.ASTC_4x4,
            compressionQuality = 50
        });
        importer.SaveAndReimport();
    }

    private static bool BytesEqual(byte[] left, byte[] right)
    {
        if (left.Length != right.Length)
            return false;
        for (int i = 0; i < left.Length; i++)
            if (left[i] != right[i])
                return false;
        return true;
    }

    private static void ConfigureSideHint(RectTransform rect, bool right)
    {
        rect.anchorMin = new Vector2(right ? 1f : 0f, 0f);
        rect.anchorMax = new Vector2(right ? 1f : 0f, 1f);
        rect.pivot = new Vector2(right ? 1f : 0f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(40f, 0f);
    }

    private static void SetSiblingOrder(params Button[] buttons)
    {
        for (int i = 0; i < buttons.Length; i++)
            buttons[i].transform.SetSiblingIndex(i);
    }

    private static void MoveAndStretch(GameObject panel, Transform parent)
    {
        if (panel == null)
            return;
        panel.transform.SetParent(parent, false);
        Stretch((RectTransform)panel.transform);
    }

    private static void ConfigureAnchored(
        RectTransform rect, Vector2 anchor, Vector2 size)
    {
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;
    }

    private static T GetOrAdd<T>(GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        return component != null ? component : gameObject.AddComponent<T>();
    }

    private static GameObject GetOrCreateUiObject(string name, Transform parent)
    {
        Transform existing = FindDirectChild(parent, name);
        if (existing != null)
            return existing.gameObject;
        var result = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
        result.layer = 5;
        result.transform.SetParent(parent, false);
        return result;
    }

    private static Transform FindDirectChild(Transform parent, string name)
    {
        for (int i = 0; i < parent.childCount; i++)
            if (parent.GetChild(i).name == name)
                return parent.GetChild(i);
        return null;
    }

    private static GameObject FindInScene(Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
            foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
                if (current.name == name)
                    return current.gameObject;
        return null;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
    }
}
#endif

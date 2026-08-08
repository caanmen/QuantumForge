#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class VerticalUpgradesVisualPolish
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ThemePath =
        "Assets/Project/UI/Vertical/Generated/VerticalUiTheme.asset";
    private const string PolishFolder =
        "Assets/Project/UI/Vertical/UpgradesPolish";
    private const string ModuleFramePath = PolishFolder + "/qf_upgrade_module_frame.png";
    private const string ButtonFramePath = PolishFolder + "/qf_upgrade_button_frame.png";
    private const string GenerationPolishFolder =
        "Assets/Project/UI/Vertical/GenerationPolish";

    private static readonly string[] CanonicalIds =
    {
        "emission_focus",
        "containment_tuning",
        "tetraquark_stabilization",
        "triangle_unlock_1",
        "triangle_impulse_tuning",
        "triangle_synergy_resonance",
        "triangle_persistence_anchor",
        "triangle_energy_efficiency"
    };

    [MenuItem("Tools/Quantum Forge/Vertical UI/Apply Upgrades Visual Polish")]
    public static void ApplyVisualPolish()
    {
        GenerateNeutralFrames();
        ConfigureImports();

        VerticalUiTheme theme = AssetDatabase.LoadAssetAtPath<VerticalUiTheme>(ThemePath);
        Sprite moduleFrame = AssetDatabase.LoadAssetAtPath<Sprite>(ModuleFramePath);
        Sprite buttonFrame = AssetDatabase.LoadAssetAtPath<Sprite>(ButtonFramePath);
        Sprite higgs = LoadGenerationSprite("qf_node_higgs.png");
        Sprite tetra = LoadGenerationSprite("qf_node_tetraquark.png");
        Sprite modulator = LoadGenerationSprite("qf_node_modulator.png");
        Sprite triangleSymbol = LoadGenerationSprite("qf_circuit_experimental.png");
        Sprite resourceLe = LoadGenerationSprite("qf_resource_le.png");
        Sprite resourceTraces = LoadGenerationSprite("qf_resource_traces.png");
        Require(theme != null && moduleFrame != null && buttonFrame != null &&
            higgs != null && tetra != null && modulator != null &&
            triangleSymbol != null && resourceLe != null && resourceTraces != null,
            "Faltan recursos visuales para pulir Mejoras.");

        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        GameObject panel = FindUnique(scene, "Panel_Mejoras");
        GameObject f2Panel = FindUnique(scene, "F2Panel");
        GameObject shell = FindUnique(scene, "VerticalUpgradesShell");
        HUD hud = UnityEngine.Object.FindFirstObjectByType<HUD>(FindObjectsInactive.Include);
        Require(panel != null && f2Panel != null && shell != null && hud != null,
            "La pantalla funcional de Mejoras o su HUD estan incompletos.");
        Require(f2Panel.transform.IsChildOf(panel.transform) &&
            shell.transform.IsChildOf(f2Panel.transform),
            "La jerarquia independiente de Mejoras cambio inesperadamente.");

        VerticalUpgradesPolishUI polish = GetOrAdd<VerticalUpgradesPolishUI>(f2Panel);
        BuildResourceHeader(panel.transform, hud, resourceLe, resourceTraces, modulator,
            theme, moduleFrame, polish);
        StyleShell(shell, moduleFrame, buttonFrame, higgs, tetra, modulator,
            triangleSymbol, theme, polish);

        EditorUtility.SetDirty(polish);
        EditorSceneManager.MarkSceneDirty(scene);
        Require(EditorSceneManager.SaveScene(scene), "No se pudo guardar Main.unity.");
        AssetDatabase.SaveAssets();
        Debug.Log("[Upgrades Visual Polish] APPLIED | cabecera real | barra mecanica | " +
            "secciones con rail | iconos mecanicos | filas compactas | acentos por categoria");
    }

    [MenuItem("Tools/Quantum Forge/Vertical UI/Apply and Validate Upgrades Visual Polish")]
    public static void ApplyAndValidateBatch()
    {
        try
        {
            ApplyVisualPolish();
            ApplyVisualPolish();
            VerticalUiBlock5Validation.Validate();
            VerticalUpgradesVisualPolishValidation.Validate();
            F2ProgressionMigrationValidation.Validate();
            MobileButtonLegibilityValidation.Validate();
            QaMainSceneIntegrityValidation.ValidateMainSceneIntegrity();
            Debug.Log("[Upgrades Visual Polish] PASS | aplicado dos veces | " +
                "sin duplicados | logica F2 preservada");
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    private static void BuildResourceHeader(
        Transform panel,
        HUD hud,
        Sprite leIcon,
        Sprite tracesIcon,
        Sprite energyIcon,
        VerticalUiTheme theme,
        Sprite moduleFrame,
        VerticalUpgradesPolishUI polish)
    {
        GameObject header = GetOrCreateUi("VerticalUpgradesHeader", panel);
        RectTransform headerRect = (RectTransform)header.transform;
        headerRect.anchorMin = new Vector2(0f, 1f);
        headerRect.anchorMax = new Vector2(1f, 1f);
        headerRect.pivot = new Vector2(0.5f, 1f);
        headerRect.anchoredPosition = new Vector2(0f, -34f);
        headerRect.sizeDelta = new Vector2(-250f, 92f);

        TextMeshProUGUI leText = BuildResourcePanel(header.transform,
            "UpgradeResource_LE", leIcon, theme.energy,
            new Vector2(0f, 0f), new Vector2(0.32f, 1f), theme, moduleFrame);
        TextMeshProUGUI tracesText = BuildResourcePanel(header.transform,
            "UpgradeResource_Traces", tracesIcon, theme.traces,
            new Vector2(0.34f, 0f), new Vector2(0.66f, 1f), theme, moduleFrame);
        TextMeshProUGUI energyText = BuildResourcePanel(header.transform,
            "UpgradeResource_Energy", energyIcon, theme.triangle,
            new Vector2(0.68f, 0f), new Vector2(1f, 1f), theme, moduleFrame);
        energyText.SetText("ENERGÍA 0\n+0.00/s");
        polish.sourceLeText = hud.leText;
        polish.sourceTracesText = hud.tracesText;
        polish.sourceEnergyText = hud.energyText;
        polish.leText = leText;
        polish.tracesText = tracesText;
        polish.energyText = energyText;
        polish.theme = theme;
        header.transform.SetAsFirstSibling();
    }

    private static TextMeshProUGUI BuildResourcePanel(
        Transform parent,
        string name,
        Sprite iconSprite,
        Color accent,
        Vector2 anchorMin,
        Vector2 anchorMax,
        VerticalUiTheme theme,
        Sprite moduleFrame)
    {
        GameObject panel = GetOrCreateUi(name, parent);
        RectTransform rect = (RectTransform)panel.transform;
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        Image background = GetOrAdd<Image>(panel);
        background.sprite = moduleFrame;
        background.type = Image.Type.Sliced;
        background.color = new Color(0.56f, 0.75f, 0.92f, 0.82f);
        background.raycastTarget = false;

        Image inner = CreateImage("TechInnerBorder", panel.transform,
            moduleFrame, accent);
        inner.type = Image.Type.Sliced;
        Color innerColor = accent;
        innerColor.a = 0.22f;
        inner.color = innerColor;
        Stretch(inner.rectTransform, 7f);
        inner.transform.SetAsFirstSibling();

        Image icon = CreateImage("Icon", panel.transform, iconSprite, Color.white);
        SetLeftCenter(icon.rectTransform, 24f, 58f, 58f);
        icon.preserveAspect = true;

        TextMeshProUGUI value = CreateText("Value", panel.transform,
            name.EndsWith("LE", StringComparison.Ordinal)
                ? "LE 0\n+0.00/s"
                : "TRAZAS 0\n+0.00/s",
            29f, TextAlignmentOptions.MidlineLeft, theme.primaryText, theme);
        value.fontSizeMin = 22f;
        value.characterSpacing = 0.5f;
        value.rectTransform.anchorMin = Vector2.zero;
        value.rectTransform.anchorMax = Vector2.one;
        value.rectTransform.offsetMin = new Vector2(92f, 6f);
        value.rectTransform.offsetMax = new Vector2(-12f, -8f);
        return value;
    }

    private static void StyleShell(
        GameObject shell,
        Sprite moduleFrame,
        Sprite buttonFrame,
        Sprite higgs,
        Sprite tetra,
        Sprite modulator,
        Sprite triangleSymbol,
        VerticalUiTheme theme,
        VerticalUpgradesPolishUI polish)
    {
        RectTransform shellRect = (RectTransform)shell.transform;
        shellRect.anchorMin = Vector2.zero;
        shellRect.anchorMax = Vector2.one;
        shellRect.offsetMin = new Vector2(96f, 20f);
        shellRect.offsetMax = new Vector2(-96f, -148f);
        Image shellImage = GetOrAdd<Image>(shell);
        shellImage.color = Color.clear;
        shellImage.raycastTarget = false;

        TextMeshProUGUI title = shell.transform.Find("Title")
            ?.GetComponent<TextMeshProUGUI>();
        Require(title != null, "VerticalUpgradesShell perdio Title.");
        SetTopRect(title.rectTransform, 0f, 78f, 8f, -8f);
        title.font = theme.primaryFont;
        title.fontSize = 38f;
        title.fontSizeMax = 38f;
        title.fontSizeMin = 26f;
        title.fontStyle = FontStyles.Normal;
        title.characterSpacing = 5f;

        Image titleFrame = CreateImage("UpgradesTitleFrame", shell.transform,
            moduleFrame, new Color(0.62f, 0.82f, 0.98f, 0.86f));
        titleFrame.type = Image.Type.Sliced;
        SetTopRect(titleFrame.rectTransform, 0f, 78f, 8f, -8f);
        titleFrame.transform.SetSiblingIndex(title.transform.GetSiblingIndex());
        title.transform.SetSiblingIndex(titleFrame.transform.GetSiblingIndex() + 1);
        CreateTitleAccents(titleFrame.transform, theme.energy);

        Transform scrollTransform = shell.transform.Find("UpgradesScroll");
        Require(scrollTransform != null, "VerticalUpgradesShell perdio UpgradesScroll.");
        RectTransform scrollRect = (RectTransform)scrollTransform;
        scrollRect.anchorMin = Vector2.zero;
        scrollRect.anchorMax = Vector2.one;
        scrollRect.offsetMin = Vector2.zero;
        scrollRect.offsetMax = new Vector2(0f, -88f);
        Transform content = scrollTransform.Find("Viewport/Content");
        Require(content != null, "UpgradesScroll perdio Content.");
        VerticalLayoutGroup contentLayout = content.GetComponent<VerticalLayoutGroup>();
        Require(contentLayout != null, "Content de Mejoras perdio VerticalLayoutGroup.");
        contentLayout.padding = new RectOffset(8, 8, 8, 22);
        contentLayout.spacing = 12f;

        var sections = new List<VerticalUpgradesPolishUI.SectionBinding>();
        sections.Add(StyleSection(content.Find("Section_Production"), higgs,
            theme.energy, moduleFrame, theme));
        sections.Add(StyleSection(content.Find("Section_Traces"), tetra,
            theme.traces, moduleFrame, theme));
        sections.Add(StyleSection(content.Find("Section_Triangle"), modulator,
            theme.triangle, moduleFrame, theme));
        polish.sections = sections.ToArray();

        var rows = new List<VerticalUpgradesPolishUI.RowBinding>();
        foreach (string id in CanonicalIds)
        {
            F2UpgradeRowUI row = FindRow(shell.transform, id);
            Require(row != null, "Falta la mejora canonica " + id + ".");
            Color accent = GetAccent(id, theme);
            Sprite icon = GetIcon(id, higgs, tetra, modulator);
            rows.Add(StyleRow(row, accent, icon,
                id == "triangle_unlock_1" ? triangleSymbol : null,
                moduleFrame, buttonFrame, theme));
        }
        polish.rows = rows.ToArray();

        KeycardPurchaseUI keycard =
            UnityEngine.Object.FindFirstObjectByType<KeycardPurchaseUI>(
                FindObjectsInactive.Include);
        Transform triangleSection = content.Find("Section_Triangle");
        Require(keycard != null && triangleSection != null,
            "La Keycard o la seccion activa del Triangulo no existe.");
        keycard.transform.SetParent(triangleSection, false);
        keycard.transform.SetAsLastSibling();
        StyleKeycardRow(keycard, theme.energy, moduleFrame, buttonFrame, theme);

        VerticalUpgradesScreenUI screen =
            polish.GetComponent<VerticalUpgradesScreenUI>();
        Require(screen != null,
            "F2Panel perdio VerticalUpgradesScreenUI.");
        screen.keycardRow = keycard;
        EditorUtility.SetDirty(screen);
        ApplyFont(shell.transform, theme);
    }

    private static VerticalUpgradesPolishUI.SectionBinding StyleSection(
        Transform section,
        Sprite iconSprite,
        Color accent,
        Sprite moduleFrame,
        VerticalUiTheme theme)
    {
        Require(section != null, "Falta una seccion canonica de Mejoras.");
        Image background = GetOrAdd<Image>(section.gameObject);
        background.sprite = moduleFrame;
        background.type = Image.Type.Sliced;
        background.color = WithAlpha(accent, 0.70f);
        background.raycastTarget = false;

        VerticalLayoutGroup layout = section.GetComponent<VerticalLayoutGroup>();
        Require(layout != null, section.name + " perdio VerticalLayoutGroup.");
        layout.padding = new RectOffset(12, 12, 10, 12);
        layout.spacing = 6f;

        Transform header = section.Find("Header");
        Require(header != null, section.name + " perdio Header.");
        LayoutElement headerLayout = GetOrAdd<LayoutElement>(header.gameObject);
        headerLayout.minHeight = 108f;
        headerLayout.preferredHeight = 108f;
        headerLayout.flexibleHeight = 0f;
        Image oldBackground = GetOrAdd<Image>(header.gameObject);
        oldBackground.color = Color.clear;
        oldBackground.raycastTarget = false;

        Transform oldAccent = header.Find("Accent");
        if (oldAccent != null)
        {
            RectTransform accentRect = (RectTransform)oldAccent;
            accentRect.anchorMin = new Vector2(0f, 0.10f);
            accentRect.anchorMax = new Vector2(0f, 0.90f);
            accentRect.pivot = new Vector2(0f, 0.5f);
            accentRect.anchoredPosition = new Vector2(0f, 0f);
            accentRect.sizeDelta = new Vector2(4f, 0f);
            Image accentImage = oldAccent.GetComponent<Image>();
            accentImage.color = accent;
            accentImage.raycastTarget = false;
        }

        Image iconGlow = CreateImage("IconGlow", header, theme.softGlow,
            WithAlpha(accent, 0.14f));
        SetLeftCenter(iconGlow.rectTransform, 10f, 104f, 104f);
        Image icon = CreateImage("MechanicalIcon", header, iconSprite, Color.white);
        SetLeftCenter(icon.rectTransform, 14f, 96f, 96f);
        icon.preserveAspect = true;

        TextMeshProUGUI label = header.Find("Label")?.GetComponent<TextMeshProUGUI>();
        Require(label != null, section.name + " perdio Label.");
        label.font = theme.primaryFont;
        label.fontSize = 27f;
        label.fontSizeMax = 27f;
        label.fontSizeMin = 20f;
        label.characterSpacing = 1.6f;
        label.alignment = TextAlignmentOptions.MidlineLeft;
        label.color = accent;
        label.rectTransform.anchorMin = new Vector2(0f, 0f);
        label.rectTransform.anchorMax = new Vector2(1f, 1f);
        label.rectTransform.offsetMin = new Vector2(126f, 52f);
        label.rectTransform.offsetMax = new Vector2(-12f, -6f);

        Image railBase = CreateImage("RailBase", header, null,
            new Color(0.018f, 0.080f, 0.115f, 0.96f));
        SetHorizontalRail(railBase.rectTransform, 122f, 10f, 15f, 43f);
        Image railGlow = CreateImage("RailGlow", header, theme.softGlow,
            WithAlpha(accent, 0.14f));
        SetHorizontalRail(railGlow.rectTransform, 126f, 8f, 19f, 38f);
        Image railCore = CreateImage("RailCore", header, null, accent);
        SetHorizontalRail(railCore.rectTransform, 128f, 10f, 28f, 33f);
        TextMeshProUGUI segments = CreateText("RailSegments", header,
            ">  >  >  >  >  >  >  >  >  >", 13f,
            TextAlignmentOptions.Center, WithAlpha(accent, 0.48f), theme);
        SetHorizontalRail(segments.rectTransform, 142f, 24f, 18f, 42f);
        segments.textWrappingMode = TextWrappingModes.NoWrap;

        Image cap = CreateImage("RailCap", header, null, accent);
        cap.rectTransform.anchorMin = new Vector2(1f, 0f);
        cap.rectTransform.anchorMax = new Vector2(1f, 0f);
        cap.rectTransform.pivot = new Vector2(1f, 0f);
        cap.rectTransform.anchoredPosition = new Vector2(-8f, 30f);
        cap.rectTransform.sizeDelta = new Vector2(10f, 10f);

        return new VerticalUpgradesPolishUI.SectionBinding
        {
            railGlow = railGlow,
            railCore = railCore,
            iconGlow = iconGlow,
            icon = icon.rectTransform,
            accent = accent
        };
    }

    private static VerticalUpgradesPolishUI.RowBinding StyleRow(
        F2UpgradeRowUI row,
        Color accent,
        Sprite iconSprite,
        Sprite symbolSprite,
        Sprite moduleFrame,
        Sprite buttonFrame,
        VerticalUiTheme theme)
    {
        RectTransform rect = (RectTransform)row.transform;
        rect.localScale = Vector3.one;
        LayoutElement layout = GetOrAdd<LayoutElement>(row.gameObject);
        layout.ignoreLayout = false;
        layout.minHeight = 176f;
        layout.preferredHeight = 176f;
        layout.flexibleHeight = 0f;

        Image frame = GetOrAdd<Image>(row.gameObject);
        frame.sprite = moduleFrame;
        frame.type = Image.Type.Sliced;
        frame.color = WithAlpha(accent, 0.82f);
        frame.raycastTarget = false;
        Image innerFrame = CreateImage("RowInnerFrame", row.transform,
            moduleFrame, WithAlpha(accent, 0.20f));
        innerFrame.type = Image.Type.Sliced;
        Stretch(innerFrame.rectTransform, 6f);
        innerFrame.transform.SetAsFirstSibling();

        Transform accentTransform = row.transform.Find("Accent");
        Require(accentTransform != null, row.name + " perdio Accent.");
        RectTransform accentRect = (RectTransform)accentTransform;
        accentRect.anchorMin = new Vector2(0f, 0.12f);
        accentRect.anchorMax = new Vector2(0f, 0.88f);
        accentRect.pivot = new Vector2(0f, 0.5f);
        accentRect.anchoredPosition = new Vector2(5f, 0f);
        accentRect.sizeDelta = new Vector2(4f, 0f);
        Image accentBar = accentTransform.GetComponent<Image>();
        accentBar.color = accent;
        accentBar.raycastTarget = false;

        Image iconGlow = CreateImage("IconGlow", row.transform, theme.softGlow,
            WithAlpha(accent, 0.14f));
        SetLeftCenter(iconGlow.rectTransform, 12f, 124f, 124f);
        Image icon = CreateImage("MechanicalIcon", row.transform, iconSprite,
            Color.white);
        SetLeftCenter(icon.rectTransform, 18f, 112f, 112f);
        icon.preserveAspect = true;

        Image symbolBackdrop = CreateImage("IconSymbolBackdrop", row.transform,
            null, new Color(0.01f, 0.025f, 0.035f, 0.92f));
        SetLeftCenter(symbolBackdrop.rectTransform, 46f, 56f, 56f);
        bool showSymbol = symbolSprite != null;
        symbolBackdrop.gameObject.SetActive(showSymbol);
        GameObject symbolObject = GetOrCreateUi("IconSymbol", row.transform);
        Image oldSymbolImage = symbolObject.GetComponent<Image>();
        if (oldSymbolImage != null)
            UnityEngine.Object.DestroyImmediate(oldSymbolImage);
        TextMeshProUGUI oldSymbolText = symbolObject.GetComponent<TextMeshProUGUI>();
        if (oldSymbolText != null)
            UnityEngine.Object.DestroyImmediate(oldSymbolText);
        SetLeftCenter((RectTransform)symbolObject.transform, 49f, 50f, 50f);
        Image symbolLeft = CreateImage("Left", symbolObject.transform, null, accent);
        SetCentered(symbolLeft.rectTransform, new Vector2(-8f, 0f),
            new Vector2(34f, 4f));
        symbolLeft.rectTransform.localEulerAngles = new Vector3(0f, 0f, 60f);
        Image symbolRight = CreateImage("Right", symbolObject.transform, null, accent);
        SetCentered(symbolRight.rectTransform, new Vector2(8f, 0f),
            new Vector2(34f, 4f));
        symbolRight.rectTransform.localEulerAngles = new Vector3(0f, 0f, -60f);
        Image symbolBottom = CreateImage("Bottom", symbolObject.transform, null, accent);
        SetCentered(symbolBottom.rectTransform, new Vector2(0f, -14f),
            new Vector2(32f, 4f));
        symbolObject.SetActive(showSymbol);

        Image divider = CreateImage("IconDivider", row.transform, null,
            new Color(accent.r, accent.g, accent.b, 0.18f));
        divider.rectTransform.anchorMin = new Vector2(0f, 0.16f);
        divider.rectTransform.anchorMax = new Vector2(0f, 0.84f);
        divider.rectTransform.pivot = new Vector2(0f, 0.5f);
        divider.rectTransform.anchoredPosition = new Vector2(144f, 0f);
        divider.rectTransform.sizeDelta = new Vector2(2f, 0f);

        ConfigureTopStretch(row.TitleText.rectTransform, 156f, 224f, 17f, 42f);
        row.TitleText.fontSize = 27f;
        row.TitleText.fontSizeMax = 27f;
        row.TitleText.fontSizeMin = 20f;
        row.TitleText.characterSpacing = 0.4f;
        ConfigureTopStretch(row.DescriptionText.rectTransform,
            156f, 224f, 58f, 52f);
        row.DescriptionText.fontSize = 21f;
        row.DescriptionText.fontSizeMax = 21f;
        row.DescriptionText.fontSizeMin = 17f;
        row.DescriptionText.overflowMode = TextOverflowModes.Ellipsis;
        ConfigureBottomStretch(row.CostText.rectTransform,
            156f, 224f, 16f, 34f);
        row.CostText.fontSize = 23f;
        row.CostText.fontSizeMax = 23f;
        row.CostText.fontSizeMin = 18f;

        RectTransform tierRect = row.TierText.rectTransform;
        tierRect.anchorMin = new Vector2(1f, 1f);
        tierRect.anchorMax = new Vector2(1f, 1f);
        tierRect.pivot = new Vector2(1f, 1f);
        tierRect.anchoredPosition = new Vector2(-18f, -16f);
        tierRect.sizeDelta = new Vector2(200f, 34f);
        row.TierText.fontSize = 22f;
        row.TierText.fontSizeMax = 22f;
        row.TierText.fontSizeMin = 17f;

        Button button = row.BuyButton;
        Require(button != null, row.name + " perdio su boton real.");
        RectTransform buttonRect = (RectTransform)button.transform;
        buttonRect.anchorMin = new Vector2(1f, 0f);
        buttonRect.anchorMax = new Vector2(1f, 0f);
        buttonRect.pivot = new Vector2(1f, 0f);
        buttonRect.anchoredPosition = new Vector2(-18f, 16f);
        buttonRect.sizeDelta = new Vector2(206f, 70f);
        Image buttonImage = GetOrAdd<Image>(button.gameObject);
        buttonImage.sprite = theme.buttonFrame;
        buttonImage.type = Image.Type.Sliced;
        buttonImage.color = Color.white;
        button.targetGraphic = buttonImage;
        Image buttonAccentFrame = CreateImage("AccentFrame", button.transform,
            buttonFrame, accent);
        buttonAccentFrame.type = Image.Type.Sliced;
        Stretch(buttonAccentFrame.rectTransform, 0f);
        buttonAccentFrame.transform.SetAsFirstSibling();
        TextMeshProUGUI buttonLabel = button.GetComponentInChildren<TextMeshProUGUI>(true);
        Require(buttonLabel != null, row.name + " perdio Label del boton.");
        buttonLabel.font = theme.primaryFont;
        buttonLabel.fontSize = 22f;
        buttonLabel.fontSizeMax = 22f;
        buttonLabel.fontSizeMin = 18f;
        buttonLabel.characterSpacing = 1f;
        buttonLabel.margin = new Vector4(8f, 4f, 8f, 4f);

        return new VerticalUpgradesPolishUI.RowBinding
        {
            row = row,
            frame = frame,
            innerFrame = innerFrame,
            accentBar = accentBar,
            iconGlow = iconGlow,
            icon = icon,
            buttonFrame = buttonAccentFrame,
            accent = accent
        };
    }

    private static void StyleKeycardRow(
        KeycardPurchaseUI keycard,
        Color accent,
        Sprite moduleFrame,
        Sprite buttonFrame,
        VerticalUiTheme theme)
    {
        GameObject row = keycard.gameObject;
        row.name = "VerticalKeycardRow";
        row.SetActive(true);

        RectTransform rect = (RectTransform)row.transform;
        rect.localScale = Vector3.one;
        LayoutElement layout = GetOrAdd<LayoutElement>(row);
        layout.ignoreLayout = false;
        layout.minHeight = 204f;
        layout.preferredHeight = 204f;
        layout.flexibleHeight = 0f;

        HorizontalLayoutGroup legacyLayout = row.GetComponent<HorizontalLayoutGroup>();
        if (legacyLayout != null)
            legacyLayout.enabled = false;

        Image frame = GetOrAdd<Image>(row);
        frame.enabled = true;
        frame.sprite = moduleFrame;
        frame.type = Image.Type.Sliced;
        frame.color = WithAlpha(accent, 0.82f);
        frame.raycastTarget = false;

        Image inner = CreateImage("KeycardInnerFrame", row.transform,
            moduleFrame, WithAlpha(accent, 0.20f));
        inner.type = Image.Type.Sliced;
        Stretch(inner.rectTransform, 6f);
        inner.transform.SetAsFirstSibling();

        Image accentBar = CreateImage("KeycardAccent", row.transform,
            null, WithAlpha(accent, 0.92f));
        accentBar.rectTransform.anchorMin = new Vector2(0f, 0.12f);
        accentBar.rectTransform.anchorMax = new Vector2(0f, 0.88f);
        accentBar.rectTransform.pivot = new Vector2(0f, 0.5f);
        accentBar.rectTransform.anchoredPosition = new Vector2(5f, 0f);
        accentBar.rectTransform.sizeDelta = new Vector2(4f, 0f);

        Require(keycard.nameText != null && keycard.descText != null &&
            keycard.costText != null && keycard.buyButton != null &&
            keycard.buttonText != null,
            "La fila de Keycard tiene referencias incompletas.");

        ConfigureTopStretch(keycard.nameText.rectTransform,
            26f, 232f, 18f, 42f);
        keycard.nameText.font = theme.primaryFont;
        keycard.nameText.fontSize = 27f;
        keycard.nameText.fontSizeMax = 27f;
        keycard.nameText.fontSizeMin = 20f;
        keycard.nameText.alignment = TextAlignmentOptions.MidlineLeft;
        keycard.nameText.color = theme.primaryText;

        ConfigureTopStretch(keycard.descText.rectTransform,
            26f, 232f, 62f, 70f);
        keycard.descText.font = theme.primaryFont;
        keycard.descText.fontSize = 20f;
        keycard.descText.fontSizeMax = 20f;
        keycard.descText.fontSizeMin = 16f;
        keycard.descText.alignment = TextAlignmentOptions.TopLeft;
        keycard.descText.color = theme.secondaryText;

        ConfigureBottomStretch(keycard.costText.rectTransform,
            26f, 232f, 18f, 36f);
        keycard.costText.font = theme.primaryFont;
        keycard.costText.fontSize = 22f;
        keycard.costText.fontSizeMax = 22f;
        keycard.costText.fontSizeMin = 17f;
        keycard.costText.alignment = TextAlignmentOptions.MidlineLeft;
        keycard.costText.color = accent;

        RectTransform buttonRect = (RectTransform)keycard.buyButton.transform;
        buttonRect.anchorMin = new Vector2(1f, 0f);
        buttonRect.anchorMax = new Vector2(1f, 0f);
        buttonRect.pivot = new Vector2(1f, 0f);
        buttonRect.anchoredPosition = new Vector2(-18f, 18f);
        buttonRect.sizeDelta = new Vector2(206f, 72f);
        Image buttonImage = GetOrAdd<Image>(keycard.buyButton.gameObject);
        buttonImage.sprite = theme.buttonFrame;
        buttonImage.type = Image.Type.Sliced;
        buttonImage.color = Color.white;
        keycard.buyButton.targetGraphic = buttonImage;

        Image buttonAccent = CreateImage("AccentFrame",
            keycard.buyButton.transform, buttonFrame, accent);
        buttonAccent.type = Image.Type.Sliced;
        Stretch(buttonAccent.rectTransform, 0f);
        buttonAccent.transform.SetAsFirstSibling();

        keycard.buttonText.font = theme.primaryFont;
        keycard.buttonText.fontSize = 21f;
        keycard.buttonText.fontSizeMax = 21f;
        keycard.buttonText.fontSizeMin = 15f;
        keycard.buttonText.alignment = TextAlignmentOptions.Center;
        keycard.buttonText.color = accent;
        Stretch(keycard.buttonText.rectTransform, 8f);

        EditorUtility.SetDirty(keycard);
    }

    private static void GenerateNeutralFrames()
    {
        Directory.CreateDirectory(Path.GetFullPath(PolishFolder));
        WriteFrame(ModuleFramePath, 128, 64, 12, 2,
            new Color32(255, 255, 255, 220), new Color32(8, 20, 29, 248));
        WriteFrame(ButtonFramePath, 128, 64, 13, 3,
            new Color32(255, 255, 255, 255), new Color32(5, 17, 25, 250));
    }

    private static void ConfigureImports()
    {
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        foreach (string path in new[] { ModuleFramePath, ButtonFramePath })
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            Require(importer != null, "No se pudo importar " + path + ".");
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 100f;
            importer.spriteBorder = new Vector4(18f, 18f, 18f, 18f);
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            importer.maxTextureSize = 256;
            TextureImporterPlatformSettings android =
                importer.GetPlatformTextureSettings("Android");
            android.overridden = true;
            android.maxTextureSize = 256;
            android.format = TextureImporterFormat.ASTC_4x4;
            android.compressionQuality = 100;
            importer.SetPlatformTextureSettings(android);
            importer.SaveAndReimport();
        }
    }

    private static void WriteFrame(
        string assetPath,
        int width,
        int height,
        int chamfer,
        int border,
        Color32 borderColor,
        Color32 fillColor)
    {
        Color32[] pixels = new Color32[width * height];
        Color32 clear = new Color32(0, 0, 0, 0);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (!InsideChamfer(x, y, width, height, chamfer))
                {
                    pixels[y * width + x] = clear;
                    continue;
                }
                bool insideInner = InsideChamfer(
                    x - border, y - border,
                    width - border * 2, height - border * 2,
                    Mathf.Max(1, chamfer - border));
                pixels[y * width + x] = insideInner ? fillColor : borderColor;
            }
        }

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.SetPixels32(pixels);
        texture.Apply(false, false);
        byte[] bytes = texture.EncodeToPNG();
        UnityEngine.Object.DestroyImmediate(texture);
        string absolutePath = Path.GetFullPath(assetPath);
        if (!File.Exists(absolutePath) || !BytesEqual(File.ReadAllBytes(absolutePath), bytes))
            File.WriteAllBytes(absolutePath, bytes);
    }

    private static bool InsideChamfer(
        int x, int y, int width, int height, int chamfer)
    {
        if (x < 0 || y < 0 || x >= width || y >= height) return false;
        if (x + y < chamfer) return false;
        if ((width - 1 - x) + y < chamfer) return false;
        if (x + (height - 1 - y) < chamfer) return false;
        if ((width - 1 - x) + (height - 1 - y) < chamfer) return false;
        return true;
    }

    private static bool BytesEqual(byte[] left, byte[] right)
    {
        if (left == null || right == null || left.Length != right.Length) return false;
        for (int i = 0; i < left.Length; i++)
            if (left[i] != right[i]) return false;
        return true;
    }

    private static Sprite LoadGenerationSprite(string fileName) =>
        AssetDatabase.LoadAssetAtPath<Sprite>(GenerationPolishFolder + "/" + fileName);

    private static F2UpgradeRowUI FindRow(Transform root, string id)
    {
        foreach (F2UpgradeRowUI row in root.GetComponentsInChildren<F2UpgradeRowUI>(true))
            if (row.UpgradeId == id) return row;
        return null;
    }

    private static Sprite GetIcon(
        string id, Sprite higgs, Sprite tetra, Sprite modulator)
    {
        if (id == "emission_focus" || id == "containment_tuning") return higgs;
        if (id == "tetraquark_stabilization") return tetra;
        return modulator;
    }

    private static Color GetAccent(string id, VerticalUiTheme theme)
    {
        if (id == "emission_focus" || id == "containment_tuning")
            return theme.energy;
        if (id == "tetraquark_stabilization") return theme.traces;
        return theme.triangle;
    }

    private static void CreateTitleAccents(Transform frame, Color accent)
    {
        Image left = CreateImage("AccentLeft", frame, null, accent);
        left.rectTransform.anchorMin = new Vector2(0.08f, 0.5f);
        left.rectTransform.anchorMax = new Vector2(0.29f, 0.5f);
        left.rectTransform.sizeDelta = new Vector2(0f, 3f);
        left.rectTransform.anchoredPosition = Vector2.zero;
        Image right = CreateImage("AccentRight", frame, null, accent);
        right.rectTransform.anchorMin = new Vector2(0.71f, 0.5f);
        right.rectTransform.anchorMax = new Vector2(0.92f, 0.5f);
        right.rectTransform.sizeDelta = new Vector2(0f, 3f);
        right.rectTransform.anchoredPosition = Vector2.zero;
    }

    private static void ApplyFont(Transform root, VerticalUiTheme theme)
    {
        if (root == null || theme?.primaryFont == null) return;
        foreach (TextMeshProUGUI text in root.GetComponentsInChildren<TextMeshProUGUI>(true))
            text.font = theme.primaryFont;
    }

    private static Image CreateImage(
        string name, Transform parent, Sprite sprite, Color color)
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
        float fontSize,
        TextAlignmentOptions alignment,
        Color color,
        VerticalUiTheme theme)
    {
        GameObject go = GetOrCreateUi(name, parent);
        TextMeshProUGUI text = GetOrAdd<TextMeshProUGUI>(go);
        text.text = value;
        text.font = theme.primaryFont != null
            ? theme.primaryFont
            : TMP_Settings.defaultFontAsset;
        text.fontSize = fontSize;
        text.fontSizeMax = fontSize;
        text.fontSizeMin = Mathf.Max(11f, fontSize - 7f);
        text.enableAutoSizing = true;
        text.alignment = alignment;
        text.color = color;
        text.raycastTarget = false;
        text.margin = new Vector4(2f, 2f, 2f, 2f);
        return text;
    }

    private static GameObject GetOrCreateUi(string name, Transform parent)
    {
        Transform existing = parent.Find(name);
        if (existing != null) return existing.gameObject;
        GameObject go = new GameObject(name,
            typeof(RectTransform), typeof(CanvasRenderer));
        go.layer = 5;
        go.transform.SetParent(parent, false);
        return go;
    }

    private static T GetOrAdd<T>(GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();
        return component != null ? component : go.AddComponent<T>();
    }

    private static GameObject FindUnique(Scene scene, string name)
    {
        GameObject found = null;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
            {
                if (current.name != name) continue;
                if (found != null)
                    throw new InvalidOperationException(name + " esta duplicado.");
                found = current.gameObject;
            }
        }
        return found;
    }

    private static void SetTopRect(
        RectTransform rect, float top, float height, float left, float right)
    {
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2((left + right) * 0.5f, -top);
        rect.sizeDelta = new Vector2(right - left, height);
    }

    private static void SetLeftCenter(
        RectTransform rect, float left, float width, float height)
    {
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0f, 0.5f);
        rect.anchoredPosition = new Vector2(left, 0f);
        rect.sizeDelta = new Vector2(width, height);
    }

    private static void SetCentered(
        RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    private static void SetHorizontalRail(
        RectTransform rect, float left, float right, float bottom, float top)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = new Vector2(1f, 0f);
        rect.offsetMin = new Vector2(left, bottom);
        rect.offsetMax = new Vector2(-right, top);
    }

    private static void ConfigureTopStretch(
        RectTransform rect, float left, float right, float top, float height)
    {
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.offsetMin = new Vector2(left, -top - height);
        rect.offsetMax = new Vector2(-right, -top);
    }

    private static void ConfigureBottomStretch(
        RectTransform rect, float left, float right, float bottom, float height)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.offsetMin = new Vector2(left, bottom);
        rect.offsetMax = new Vector2(-right, bottom + height);
    }

    private static void Stretch(RectTransform rect, float inset)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(inset, inset);
        rect.offsetMax = new Vector2(-inset, -inset);
    }

    private static Color WithAlpha(Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
#endif

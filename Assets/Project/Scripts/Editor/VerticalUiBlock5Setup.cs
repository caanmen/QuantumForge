#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class VerticalUiBlock5Setup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ThemePath =
        "Assets/Project/UI/Vertical/Generated/VerticalUiTheme.asset";

    private static readonly string[] CanonicalIds =
    {
        "emission_focus",
        "containment_tuning",
        "tetraquark_stabilization",
        "triangle_unlock_1",
        "triangle_impulse_tuning",
        "triangle_synergy_resonance",
        "triangle_persistence_anchor"
    };

    [MenuItem("Tools/Quantum Forge/Vertical UI/Configure Block 5 Upgrades")]
    public static void ConfigureBlock5Upgrades()
    {
        VerticalUiTheme theme = AssetDatabase.LoadAssetAtPath<VerticalUiTheme>(ThemePath);
        if (theme == null)
            throw new InvalidOperationException("Falta VerticalUiTheme de los bloques anteriores.");

        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        TabsUI tabs = UnityEngine.Object.FindFirstObjectByType<TabsUI>(
            FindObjectsInactive.Include);
        GameObject panel = FindNamed(scene, "Panel_Mejoras");
        GameObject f2Panel = FindNamed(scene, "F2Panel");
        if (tabs == null || panel == null || f2Panel == null)
            throw new InvalidOperationException(
                "El Bloque 2 debe existir antes de configurar Mejoras.");
        if (!f2Panel.transform.IsChildOf(panel.transform))
            throw new InvalidOperationException("F2Panel no vive dentro de Panel_Mejoras.");

        Stretch((RectTransform)panel.transform);
        Stretch((RectTransform)f2Panel.transform);
        ResearchUI legacyResearch = f2Panel.GetComponent<ResearchUI>();
        if (legacyResearch != null)
            legacyResearch.enabled = false;
        Image legacyBackground = f2Panel.GetComponent<Image>();
        if (legacyBackground != null)
        {
            legacyBackground.color = Color.clear;
            legacyBackground.raycastTarget = false;
        }

        GameObject shell = GetOrCreateUiObject("VerticalUpgradesShell", f2Panel.transform);
        Stretch((RectTransform)shell.transform);
        Image shellImage = GetOrAdd<Image>(shell);
        shellImage.color = Color.clear;
        shellImage.raycastTarget = false;

        TextMeshProUGUI title = CreateText(
            "Title", shell.transform, "MEJORAS", 42f,
            TextAlignmentOptions.Center, theme.primaryText, theme);
        LocalizedTMP localizedTitle = GetOrAdd<LocalizedTMP>(title.gameObject);
        localizedTitle.key = "upgrades.title";
        ConfigureTopTitle(title.rectTransform);

        ScrollRect scroll = BuildScroll(shell.transform, out RectTransform content);
        RectTransform production = BuildSection(
            "Section_Production", "PRODUCCIÓN",
            "upgrades.section.production", theme.energy, content, theme,
            out RectTransform productionRows);
        RectTransform traces = BuildSection(
            "Section_Traces", "TRAZAS",
            "upgrades.section.traces", theme.traces, content, theme,
            out RectTransform tracesRows);
        RectTransform triangle = BuildSection(
            "Section_Triangle", "TRIÁNGULO",
            "upgrades.section.triangle", theme.triangle, content, theme,
            out RectTransform triangleRows);

        Dictionary<string, F2UpgradeRowUI> rows = FindCanonicalRows(scene);
        BuildOrMoveRow(rows, "emission_focus", productionRows, theme.energy, theme);
        BuildOrMoveRow(rows, "containment_tuning", productionRows, theme.energy, theme);
        BuildOrMoveRow(rows, "tetraquark_stabilization", tracesRows, theme.traces, theme);
        BuildOrMoveRow(rows, "triangle_unlock_1", triangleRows, theme.triangle, theme);
        BuildOrMoveRow(rows, "triangle_impulse_tuning", triangleRows, theme.triangle, theme);
        BuildOrMoveRow(rows, "triangle_synergy_resonance", triangleRows, theme.triangle, theme);
        BuildOrMoveRow(rows, "triangle_persistence_anchor", triangleRows, theme.triangle, theme);

        SetOrder(productionRows, "emission_focus", "containment_tuning");
        SetOrder(tracesRows, "tetraquark_stabilization");
        SetOrder(triangleRows, "triangle_unlock_1", "triangle_impulse_tuning",
            "triangle_synergy_resonance", "triangle_persistence_anchor");
        production.SetSiblingIndex(0);
        traces.SetSiblingIndex(1);
        triangle.SetSiblingIndex(2);

        MoveLegacyLayout(f2Panel.transform, shell.transform);
        shell.transform.SetSiblingIndex(0);

        VerticalUpgradesScreenUI screen = GetOrAdd<VerticalUpgradesScreenUI>(f2Panel);
        screen.content = content;
        screen.productionSection = production;
        screen.tracesSection = traces;
        screen.triangleSection = triangle;
        screen.rows = BuildOrderedRows(rows);
        EditorUtility.SetDirty(screen);

        tabs.panelMejoras = panel;
        EditorUtility.SetDirty(tabs);
        panel.SetActive(false);
        f2Panel.SetActive(true);
        shell.SetActive(true);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[Vertical UI Block 5] CONFIGURED | independent upgrades | " +
            "Production, Traces and Triangle | seven real rows | progressive gates");
    }

    public static void ConfigureAndValidateBatch()
    {
        ConfigureBlock5Upgrades();
        ConfigureBlock5Upgrades();
        VerticalUiBlock1Validation.Validate();
        VerticalUiBlock2Validation.Validate();
        VerticalUiBlock3Validation.Validate();
        VerticalUiBlock4Validation.Validate();
        VerticalUiBlock5Validation.Validate();
        F2ProgressionMigrationValidation.Validate();
        TriangleRedesignValidation.Validate();
        MobileQaFriendlyLayoutValidation.Validate();
        MobileButtonLegibilityValidation.Validate();
        QaMainSceneIntegrityValidation.ValidateMainSceneIntegrity();
        Debug.Log("[Vertical UI Block 5] IDEMPOTENCE PASS | setup executed twice");
    }

    private static ScrollRect BuildScroll(Transform parent, out RectTransform content)
    {
        GameObject scrollObject = GetOrCreateUiObject("UpgradesScroll", parent);
        RectTransform scrollRect = (RectTransform)scrollObject.transform;
        scrollRect.anchorMin = Vector2.zero;
        scrollRect.anchorMax = Vector2.one;
        scrollRect.offsetMin = new Vector2(18f, 18f);
        scrollRect.offsetMax = new Vector2(-18f, -108f);
        Image inputSurface = GetOrAdd<Image>(scrollObject);
        inputSurface.color = new Color(1f, 1f, 1f, 0.001f);
        inputSurface.raycastTarget = true;

        GameObject viewportObject = GetOrCreateUiObject("Viewport", scrollObject.transform);
        RectTransform viewport = (RectTransform)viewportObject.transform;
        Stretch(viewport);
        GetOrAdd<RectMask2D>(viewportObject);

        GameObject contentObject = GetOrCreateUiObject("Content", viewportObject.transform);
        content = (RectTransform)contentObject.transform;
        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = new Vector2(1f, 1f);
        content.pivot = new Vector2(0.5f, 1f);
        content.anchoredPosition = Vector2.zero;
        content.sizeDelta = Vector2.zero;

        VerticalLayoutGroup layout = GetOrAdd<VerticalLayoutGroup>(contentObject);
        layout.padding = new RectOffset(10, 10, 8, 24);
        layout.spacing = 20f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        ContentSizeFitter fitter = GetOrAdd<ContentSizeFitter>(contentObject);
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        ScrollRect scroll = GetOrAdd<ScrollRect>(scrollObject);
        scroll.viewport = viewport;
        scroll.content = content;
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Elastic;
        scroll.elasticity = 0.08f;
        scroll.inertia = true;
        scroll.decelerationRate = 0.12f;
        scroll.scrollSensitivity = 34f;
        return scroll;
    }

    private static RectTransform BuildSection(
        string name,
        string fallbackTitle,
        string localizationKey,
        Color accent,
        Transform parent,
        VerticalUiTheme theme,
        out RectTransform rowsRoot)
    {
        GameObject sectionObject = GetOrCreateUiObject(name, parent);
        RectTransform section = (RectTransform)sectionObject.transform;
        Image background = GetOrAdd<Image>(sectionObject);
        background.sprite = theme.panelFrame;
        background.type = Image.Type.Sliced;
        background.color = Color.white;
        background.raycastTarget = false;

        VerticalLayoutGroup layout = GetOrAdd<VerticalLayoutGroup>(sectionObject);
        layout.padding = new RectOffset(18, 18, 14, 18);
        layout.spacing = 12f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        ContentSizeFitter fitter = GetOrAdd<ContentSizeFitter>(sectionObject);
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        GetOrAdd<LayoutElement>(sectionObject).ignoreLayout = false;
        CanvasGroup group = GetOrAdd<CanvasGroup>(sectionObject);
        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;

        GameObject headerObject = GetOrCreateUiObject("Header", sectionObject.transform);
        LayoutElement headerLayout = GetOrAdd<LayoutElement>(headerObject);
        headerLayout.minHeight = 62f;
        headerLayout.preferredHeight = 62f;
        Image headerBackground = GetOrAdd<Image>(headerObject);
        headerBackground.color = new Color(accent.r, accent.g, accent.b, 0.075f);
        headerBackground.raycastTarget = false;

        GameObject barObject = GetOrCreateUiObject("Accent", headerObject.transform);
        Image bar = GetOrAdd<Image>(barObject);
        bar.color = accent;
        bar.raycastTarget = false;
        RectTransform barRect = (RectTransform)barObject.transform;
        barRect.anchorMin = new Vector2(0f, 0.18f);
        barRect.anchorMax = new Vector2(0f, 0.82f);
        barRect.pivot = new Vector2(0f, 0.5f);
        barRect.anchoredPosition = new Vector2(2f, 0f);
        barRect.sizeDelta = new Vector2(5f, 0f);

        TextMeshProUGUI header = CreateText("Label", headerObject.transform,
            fallbackTitle, 27f, TextAlignmentOptions.MidlineLeft, accent, theme);
        header.rectTransform.anchorMin = Vector2.zero;
        header.rectTransform.anchorMax = Vector2.one;
        header.rectTransform.offsetMin = new Vector2(24f, 4f);
        header.rectTransform.offsetMax = new Vector2(-12f, -4f);
        LocalizedTMP localized = GetOrAdd<LocalizedTMP>(header.gameObject);
        localized.key = localizationKey;

        GameObject rowsObject = GetOrCreateUiObject("Rows", sectionObject.transform);
        rowsRoot = (RectTransform)rowsObject.transform;
        VerticalLayoutGroup rowsLayout = GetOrAdd<VerticalLayoutGroup>(rowsObject);
        rowsLayout.padding = new RectOffset(0, 0, 0, 0);
        rowsLayout.spacing = 12f;
        rowsLayout.childAlignment = TextAnchor.UpperCenter;
        rowsLayout.childControlWidth = true;
        rowsLayout.childControlHeight = true;
        rowsLayout.childForceExpandWidth = true;
        rowsLayout.childForceExpandHeight = false;
        ContentSizeFitter rowsFitter = GetOrAdd<ContentSizeFitter>(rowsObject);
        rowsFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        rowsFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        return section;
    }

    private static Dictionary<string, F2UpgradeRowUI> FindCanonicalRows(Scene scene)
    {
        var result = new Dictionary<string, F2UpgradeRowUI>();
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (F2UpgradeRowUI row in root.GetComponentsInChildren<F2UpgradeRowUI>(true))
            {
                if (Array.IndexOf(CanonicalIds, row.UpgradeId) < 0)
                    continue;
                if (result.ContainsKey(row.UpgradeId))
                    throw new InvalidOperationException(
                        "Hay filas F2 duplicadas para " + row.UpgradeId + ".");
                result[row.UpgradeId] = row;
            }
        }
        return result;
    }

    private static void BuildOrMoveRow(
        Dictionary<string, F2UpgradeRowUI> rows,
        string id,
        Transform parent,
        Color accent,
        VerticalUiTheme theme)
    {
        if (!rows.TryGetValue(id, out F2UpgradeRowUI row) || row == null)
        {
            GameObject rowObject = GetOrCreateUiObject("Upgrade_" + id, parent);
            row = GetOrAdd<F2UpgradeRowUI>(rowObject);
            rows[id] = row;
        }
        row.name = "Upgrade_" + id;
        row.transform.SetParent(parent, false);
        row.gameObject.SetActive(true);
        RebuildRow(row, id, accent, theme);
    }

    private static void RebuildRow(
        F2UpgradeRowUI row,
        string id,
        Color accent,
        VerticalUiTheme theme)
    {
        bool alreadyVertical =
            FindDirectChild(row.transform, "Accent") != null &&
            FindDirectChild(row.transform, "Name") != null &&
            FindDirectChild(row.transform, "Tier") != null &&
            FindDirectChild(row.transform, "Description") != null &&
            FindDirectChild(row.transform, "Cost") != null &&
            FindDirectChild(row.transform, "ActionButton") != null;
        if (!alreadyVertical)
        {
            for (int i = row.transform.childCount - 1; i >= 0; i--)
                UnityEngine.Object.DestroyImmediate(row.transform.GetChild(i).gameObject);
        }
        RemoveComponent<HorizontalLayoutGroup>(row.gameObject);
        RemoveComponent<VerticalLayoutGroup>(row.gameObject);
        RemoveComponent<ContentSizeFitter>(row.gameObject);

        RectTransform rect = (RectTransform)row.transform;
        rect.localScale = Vector3.one;
        Image background = GetOrAdd<Image>(row.gameObject);
        background.sprite = theme.panelFrame;
        background.type = Image.Type.Sliced;
        background.color = new Color(
            theme.deepSurface.r, theme.deepSurface.g, theme.deepSurface.b, 0.98f);
        background.raycastTarget = false;
        LayoutElement rowLayout = GetOrAdd<LayoutElement>(row.gameObject);
        rowLayout.ignoreLayout = false;
        rowLayout.minHeight = 226f;
        rowLayout.preferredHeight = 236f;
        rowLayout.flexibleHeight = 0f;

        GameObject accentObject = GetOrCreateUiObject("Accent", row.transform);
        Image accentImage = GetOrAdd<Image>(accentObject);
        accentImage.color = accent;
        accentImage.raycastTarget = false;
        RectTransform accentRect = (RectTransform)accentObject.transform;
        accentRect.anchorMin = new Vector2(0f, 0f);
        accentRect.anchorMax = new Vector2(0f, 1f);
        accentRect.pivot = new Vector2(0f, 0.5f);
        accentRect.anchoredPosition = Vector2.zero;
        accentRect.sizeDelta = new Vector2(5f, 0f);

        TextMeshProUGUI title = CreateText("Name", row.transform, id, 28f,
            TextAlignmentOptions.MidlineLeft, theme.primaryText, theme);
        ConfigurePoint(title.rectTransform, new Vector2(0f, 1f),
            new Vector2(28f, -18f), new Vector2(600f, 48f), new Vector2(0f, 1f));

        TextMeshProUGUI tier = CreateText("Tier", row.transform, "Nivel", 20f,
            TextAlignmentOptions.MidlineRight, accent, theme);
        ConfigurePoint(tier.rectTransform, new Vector2(1f, 1f),
            new Vector2(-28f, -20f), new Vector2(280f, 44f), new Vector2(1f, 1f));

        TextMeshProUGUI description = CreateText("Description", row.transform,
            string.Empty, 21f, TextAlignmentOptions.TopLeft,
            theme.secondaryText, theme);
        ConfigurePoint(description.rectTransform, new Vector2(0f, 1f),
            new Vector2(28f, -72f), new Vector2(640f, 92f), new Vector2(0f, 1f));

        TextMeshProUGUI cost = CreateText("Cost", row.transform, string.Empty, 21f,
            TextAlignmentOptions.MidlineLeft, accent, theme);
        ConfigurePoint(cost.rectTransform, Vector2.zero,
            new Vector2(28f, 22f), new Vector2(590f, 42f), Vector2.zero);

        GameObject buttonObject = GetOrCreateUiObject("ActionButton", row.transform);
        Button button = GetOrAdd<Button>(buttonObject);
        Image buttonImage = GetOrAdd<Image>(buttonObject);
        buttonImage.sprite = theme.buttonFrame;
        buttonImage.type = Image.Type.Sliced;
        buttonImage.color = Color.white;
        button.targetGraphic = buttonImage;
        button.transition = Selectable.Transition.ColorTint;
        button.onClick = new Button.ButtonClickedEvent();
        ConfigurePoint((RectTransform)buttonObject.transform, Vector2.right,
            new Vector2(-24f, 20f), new Vector2(250f, 72f), Vector2.right);

        TextMeshProUGUI buttonLabel = CreateText("Label", buttonObject.transform,
            "MEJORAR", 22f, TextAlignmentOptions.Center, accent, theme);
        Stretch(buttonLabel.rectTransform);
        buttonLabel.margin = new Vector4(8f, 4f, 8f, 4f);

        row.Configure(id, title, tier, cost, description, button);
        EditorUtility.SetDirty(row);
        EditorUtility.SetDirty(button);
    }

    private static void MoveLegacyLayout(Transform f2Panel, Transform shell)
    {
        GameObject legacy = GetOrCreateUiObject("LegacyF2Layout", f2Panel);
        List<Transform> move = new();
        for (int i = 0; i < f2Panel.childCount; i++)
        {
            Transform child = f2Panel.GetChild(i);
            if (child != shell && child != legacy.transform)
                move.Add(child);
        }
        foreach (Transform child in move)
            child.SetParent(legacy.transform, false);
        legacy.SetActive(false);
    }

    private static void SetOrder(Transform parent, params string[] ids)
    {
        for (int i = 0; i < ids.Length; i++)
        {
            Transform child = FindDirectChild(parent, "Upgrade_" + ids[i]);
            if (child != null)
                child.SetSiblingIndex(i);
        }
    }

    private static F2UpgradeRowUI[] BuildOrderedRows(
        Dictionary<string, F2UpgradeRowUI> rows)
    {
        var result = new F2UpgradeRowUI[CanonicalIds.Length];
        for (int i = 0; i < CanonicalIds.Length; i++)
            rows.TryGetValue(CanonicalIds[i], out result[i]);
        return result;
    }

    private static TextMeshProUGUI CreateText(
        string name,
        Transform parent,
        string text,
        float fontSize,
        TextAlignmentOptions alignment,
        Color color,
        VerticalUiTheme theme)
    {
        GameObject textObject = GetOrCreateUiObject(name, parent);
        TextMeshProUGUI label = GetOrAdd<TextMeshProUGUI>(textObject);
        label.text = text;
        label.font = theme.primaryFont != null ? theme.primaryFont : label.font;
        label.fontSize = fontSize;
        label.enableAutoSizing = true;
        label.fontSizeMin = 12f;
        label.fontSizeMax = fontSize;
        label.alignment = alignment;
        label.color = color;
        label.textWrappingMode = TextWrappingModes.Normal;
        label.overflowMode = TextOverflowModes.Ellipsis;
        label.raycastTarget = false;
        return label;
    }

    private static void ConfigureTopTitle(RectTransform rect)
    {
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -18f);
        rect.sizeDelta = new Vector2(0f, 76f);
    }

    private static void ConfigurePoint(
        RectTransform rect,
        Vector2 anchor,
        Vector2 position,
        Vector2 size,
        Vector2 pivot)
    {
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = pivot;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        rect.localScale = Vector3.one;
    }

    private static void RemoveComponent<T>(GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component != null)
            UnityEngine.Object.DestroyImmediate(component);
    }

    private static GameObject FindNamed(Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
            foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
                if (current.name == name)
                    return current.gameObject;
        return null;
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

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
        rect.localScale = Vector3.one;
    }
}
#endif

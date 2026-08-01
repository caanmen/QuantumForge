#if UNITY_EDITOR
using System;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class VerticalUiBlock3Setup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string GeneratedFolder = "Assets/Project/UI/Vertical/Generated";
    private const string ThemePath = GeneratedFolder + "/VerticalUiTheme.asset";
    private const string RowPrefabPath = GeneratedFolder + "/VerticalBuildingRow.prefab";

    [MenuItem("Tools/Quantum Forge/Vertical UI/Configure Block 3 Generation Before Triangle")]
    public static void ConfigureBlock3Generation()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        TabsUI tabs = UnityEngine.Object.FindFirstObjectByType<TabsUI>(
            FindObjectsInactive.Include);
        HUD hud = UnityEngine.Object.FindFirstObjectByType<HUD>(
            FindObjectsInactive.Include);
        BuildingListUI buildingList = UnityEngine.Object.FindFirstObjectByType<BuildingListUI>(
            FindObjectsInactive.Include);
        VerticalUiTheme theme = AssetDatabase.LoadAssetAtPath<VerticalUiTheme>(ThemePath);

        if (tabs == null || tabs.panelGeneracion == null || hud == null ||
            buildingList == null || theme == null)
        {
            throw new InvalidOperationException(
                "Los Bloques 1 y 2 y los sistemas de edificios deben existir antes del Bloque 3.");
        }

        theme.higgsArtifactIcon = LoadSprite("qf_icon_generation.png");
        theme.tetraArtifactIcon = LoadSprite("qf_icon_prestige.png");
        theme.modulatorArtifactIcon = LoadSprite("qf_icon_upgrades.png");
        EditorUtility.SetDirty(theme);

        GameObject rowPrefab = BuildVerticalBuildingRowPrefab(theme);
        Transform panel = tabs.panelGeneracion.transform;
        GameObject triangleRoot = tabs.generationTriangleLayout != null
            ? tabs.generationTriangleLayout
            : FindInScene(scene, "TopArea");
        if (triangleRoot == null)
            throw new InvalidOperationException("No se encontro el layout heredado del Triangulo.");

        BuildHeader(panel, hud, theme);
        GameObject beforeRoot = BuildBeforeTriangleRoot(
            panel, buildingList, rowPrefab, theme, out ScrollRect scroll);

        CanvasGroup beforeCanvas = GetOrAdd<CanvasGroup>(beforeRoot);
        beforeCanvas.alpha = 1f;
        beforeCanvas.interactable = true;
        beforeCanvas.blocksRaycasts = true;

        VerticalGenerationBeforeTriangleUI controller =
            GetOrAdd<VerticalGenerationBeforeTriangleUI>(tabs.panelGeneracion);
        controller.beforeTriangleRoot = beforeRoot;
        controller.triangleRoot = triangleRoot;
        controller.artifactScroll = scroll;
        controller.buildingList = buildingList;

        tabs.generationDefaultLayout = beforeRoot;
        tabs.generationTriangleLayout = triangleRoot;
        tabs.generationDefaultCanvasGroup = beforeCanvas;

        GameObject legacyHud = FindInScene(scene, "Panel_HUD");
        if (legacyHud != null)
            legacyHud.SetActive(false);
        beforeRoot.SetActive(true);
        triangleRoot.SetActive(false);

        EditorUtility.SetDirty(buildingList);
        EditorUtility.SetDirty(hud);
        EditorUtility.SetDirty(controller);
        EditorUtility.SetDirty(tabs);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("No se pudo guardar Main.unity.");
        AssetDatabase.SaveAssets();

        Debug.Log("[Vertical UI Block 3] CONFIGURED | common resource header | " +
            "GenerationBeforeTriangleRoot | known artifacts only | real purchases | no triangle clues");
    }

    public static void ConfigureAndValidateBatch()
    {
        VerticalUiBlock1Setup.ConfigureBlock1Base();
        VerticalUiBlock2Setup.ConfigureBlock2Navigation();
        ConfigureBlock3Generation();
        ConfigureBlock3Generation();
        VerticalUiBlock1Validation.Validate();
        VerticalUiBlock2Validation.Validate();
        VerticalUiBlock3Validation.Validate();
        MobileQaFriendlyLayoutValidation.Validate();
        MobileButtonLegibilityValidation.Validate();
        QaMainSceneIntegrityValidation.ValidateMainSceneIntegrity();
        Debug.Log("[Vertical UI Block 3] IDEMPOTENCE PASS | setup executed twice");
    }

    private static void BuildHeader(Transform panel, HUD hud, VerticalUiTheme theme)
    {
        GameObject header = GetOrCreateUiObject("VerticalGenerationHeader", panel);
        RectTransform rect = (RectTransform)header.transform;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -16f);
        rect.sizeDelta = new Vector2(-44f, 148f);

        BuildResourcePanel(header.transform, "Resource_LE", theme.higgsArtifactIcon,
            theme.energy, new Vector2(0f, 0f), new Vector2(0.49f, 1f),
            out TextMeshProUGUI leText);
        BuildResourcePanel(header.transform, "Resource_Traces", theme.tetraArtifactIcon,
            theme.traces, new Vector2(0.51f, 0f), new Vector2(1f, 1f),
            out TextMeshProUGUI tracesText);
        hud.leText = leText;
        hud.tracesText = tracesText;
        header.transform.SetAsFirstSibling();
    }

    private static void BuildResourcePanel(
        Transform parent, string name, Sprite icon, Color accent,
        Vector2 anchorMin, Vector2 anchorMax, out TextMeshProUGUI value)
    {
        VerticalUiTheme theme = AssetDatabase.LoadAssetAtPath<VerticalUiTheme>(ThemePath);
        GameObject panel = GetOrCreateUiObject(name, parent);
        RectTransform rect = (RectTransform)panel.transform;
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        Image background = GetOrAdd<Image>(panel);
        background.sprite = theme.panelFrame;
        background.type = Image.Type.Sliced;
        background.color = new Color(0.7f, 0.85f, 1f, 1f);
        background.raycastTarget = false;

        GameObject iconObject = GetOrCreateUiObject("Icon", panel.transform);
        RectTransform iconRect = (RectTransform)iconObject.transform;
        iconRect.anchorMin = new Vector2(0f, 0.5f);
        iconRect.anchorMax = new Vector2(0f, 0.5f);
        iconRect.pivot = new Vector2(0f, 0.5f);
        iconRect.anchoredPosition = new Vector2(32f, 0f);
        iconRect.sizeDelta = new Vector2(86f, 86f);
        Image iconImage = GetOrAdd<Image>(iconObject);
        iconImage.sprite = icon;
        iconImage.color = accent;
        iconImage.preserveAspect = true;
        iconImage.raycastTarget = false;

        value = CreateOrUpdateText("Value", panel.transform,
            name == "Resource_LE" ? "LE 0\n+0.00/s" : "TRAZAS 0\n+0.00/s",
            34f, TextAlignmentOptions.MidlineLeft, theme.primaryText);
        RectTransform valueRect = value.rectTransform;
        valueRect.anchorMin = new Vector2(0f, 0f);
        valueRect.anchorMax = new Vector2(1f, 1f);
        valueRect.offsetMin = new Vector2(136f, 14f);
        valueRect.offsetMax = new Vector2(-18f, -14f);
    }

    private static GameObject BuildBeforeTriangleRoot(
        Transform panel,
        BuildingListUI buildingList,
        GameObject rowPrefab,
        VerticalUiTheme theme,
        out ScrollRect scroll)
    {
        GameObject root = GetOrCreateUiObject("GenerationBeforeTriangleRoot", panel);
        RectTransform rootRect = (RectTransform)root.transform;
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = new Vector2(24f, 20f);
        rootRect.offsetMax = new Vector2(-24f, -176f);

        TextMeshProUGUI title = CreateLocalizedText("GenerationTitle", root.transform,
            "GENERACION", "generation.title", 43f,
            TextAlignmentOptions.Center, theme.primaryText);
        RectTransform titleRect = title.rectTransform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = Vector2.zero;
        titleRect.sizeDelta = new Vector2(0f, 92f);

        GameObject artifactFrame = GetOrCreateUiObject("ArtifactsFrame", root.transform);
        RectTransform frameRect = (RectTransform)artifactFrame.transform;
        frameRect.anchorMin = Vector2.zero;
        frameRect.anchorMax = Vector2.one;
        frameRect.offsetMin = Vector2.zero;
        frameRect.offsetMax = new Vector2(0f, -104f);
        Image frameImage = GetOrAdd<Image>(artifactFrame);
        frameImage.sprite = theme.panelFrame;
        frameImage.type = Image.Type.Sliced;
        frameImage.color = Color.white;
        frameImage.raycastTarget = false;

        TextMeshProUGUI section = CreateLocalizedText("ArtifactsTitle", artifactFrame.transform,
            "ARTEFACTOS", "generation.artifacts", 30f,
            TextAlignmentOptions.MidlineLeft, theme.primaryText);
        section.color = theme.energy;
        RectTransform sectionRect = section.rectTransform;
        sectionRect.anchorMin = new Vector2(0f, 1f);
        sectionRect.anchorMax = new Vector2(1f, 1f);
        sectionRect.pivot = new Vector2(0.5f, 1f);
        sectionRect.anchoredPosition = new Vector2(0f, -8f);
        sectionRect.sizeDelta = new Vector2(-54f, 72f);

        GameObject scrollObject = GetOrCreateUiObject("ArtifactsScroll", artifactFrame.transform);
        RectTransform scrollRect = (RectTransform)scrollObject.transform;
        scrollRect.anchorMin = Vector2.zero;
        scrollRect.anchorMax = Vector2.one;
        scrollRect.offsetMin = new Vector2(22f, 22f);
        scrollRect.offsetMax = new Vector2(-22f, -86f);
        Image scrollBackground = GetOrAdd<Image>(scrollObject);
        scrollBackground.color = Color.clear;
        scrollBackground.raycastTarget = false;
        scroll = GetOrAdd<ScrollRect>(scrollObject);
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Elastic;
        scroll.scrollSensitivity = 42f;

        GameObject viewport = GetOrCreateUiObject("Viewport", scrollObject.transform);
        Stretch((RectTransform)viewport.transform);
        Image viewportImage = GetOrAdd<Image>(viewport);
        viewportImage.color = new Color(1f, 1f, 1f, 0.001f);
        viewportImage.raycastTarget = true;
        GetOrAdd<RectMask2D>(viewport);
        scroll.viewport = (RectTransform)viewport.transform;

        buildingList.transform.SetParent(viewport.transform, false);
        buildingList.gameObject.name = "KnownArtifactsList";
        RectTransform listRect = (RectTransform)buildingList.transform;
        listRect.anchorMin = new Vector2(0f, 1f);
        listRect.anchorMax = new Vector2(1f, 1f);
        listRect.pivot = new Vector2(0.5f, 1f);
        listRect.anchoredPosition = Vector2.zero;
        listRect.sizeDelta = Vector2.zero;
        VerticalLayoutGroup layout = GetOrAdd<VerticalLayoutGroup>(buildingList.gameObject);
        layout.padding = new RectOffset(8, 8, 8, 16);
        layout.spacing = 18f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        ContentSizeFitter fitter = GetOrAdd<ContentSizeFitter>(buildingList.gameObject);
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        buildingList.rowsParent = buildingList.transform;
        buildingList.buildingRowPrefab = rowPrefab;
        buildingList.hideLockedRows = true;
        scroll.content = listRect;

        foreach (Transform child in buildingList.transform)
            child.gameObject.SetActive(false);

        root.transform.SetSiblingIndex(1);
        return root;
    }

    private static GameObject BuildVerticalBuildingRowPrefab(VerticalUiTheme theme)
    {
        GameObject root = new GameObject("VerticalBuildingRow",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image),
            typeof(CanvasGroup), typeof(LayoutElement), typeof(BuildingRowUI));
        root.layer = 5;
        RectTransform rootRect = (RectTransform)root.transform;
        rootRect.sizeDelta = new Vector2(0f, 204f);
        Image background = root.GetComponent<Image>();
        background.sprite = theme.panelFrame;
        background.type = Image.Type.Sliced;
        background.color = Color.white;
        background.raycastTarget = false;
        LayoutElement layout = root.GetComponent<LayoutElement>();
        layout.minHeight = 190f;
        layout.preferredHeight = 204f;
        layout.flexibleHeight = 0f;

        GameObject iconObject = GetOrCreateUiObject("ArtifactIcon", root.transform);
        RectTransform iconRect = (RectTransform)iconObject.transform;
        iconRect.anchorMin = new Vector2(0f, 0.5f);
        iconRect.anchorMax = new Vector2(0f, 0.5f);
        iconRect.pivot = new Vector2(0f, 0.5f);
        iconRect.anchoredPosition = new Vector2(34f, 0f);
        iconRect.sizeDelta = new Vector2(138f, 138f);
        Image icon = GetOrAdd<Image>(iconObject);
        icon.preserveAspect = true;
        icon.raycastTarget = false;

        TextMeshProUGUI name = CreateOrUpdateText("Name", root.transform,
            "ARTEFACTO - NV. 0", 27f, TextAlignmentOptions.MidlineLeft,
            theme.primaryText);
        SetOffsets(name.rectTransform,
            new Vector2(0.21f, 0.58f), new Vector2(0.73f, 0.94f),
            new Vector2(0f, 0f), new Vector2(0f, 0f));

        TextMeshProUGUI stats = CreateOrUpdateText("Stats", root.transform,
            "Coste: 0 LE\nTick: +0.00 LE / 1.00s", 21f,
            TextAlignmentOptions.MidlineLeft, theme.secondaryText);
        SetOffsets(stats.rectTransform,
            new Vector2(0.21f, 0.15f), new Vector2(0.74f, 0.62f),
            Vector2.zero, Vector2.zero);

        GameObject buyObject = GetOrCreateUiObject("BuyButton", root.transform);
        RectTransform buyRect = (RectTransform)buyObject.transform;
        buyRect.anchorMin = new Vector2(0.77f, 0.27f);
        buyRect.anchorMax = new Vector2(0.97f, 0.76f);
        buyRect.offsetMin = Vector2.zero;
        buyRect.offsetMax = Vector2.zero;
        Image buyImage = GetOrAdd<Image>(buyObject);
        buyImage.sprite = theme.buttonFrame;
        buyImage.type = Image.Type.Sliced;
        buyImage.color = Color.white;
        Button buyButton = GetOrAdd<Button>(buyObject);
        buyButton.targetGraphic = buyImage;
        TextMeshProUGUI buyLabel = CreateOrUpdateText("Label", buyObject.transform,
            "COMPRAR", 25f, TextAlignmentOptions.Center, theme.primaryText);
        Stretch(buyLabel.rectTransform);

        GameObject tickTrack = GetOrCreateUiObject("TickTrack", root.transform);
        RectTransform trackRect = (RectTransform)tickTrack.transform;
        trackRect.anchorMin = new Vector2(0.21f, 0.08f);
        trackRect.anchorMax = new Vector2(0.73f, 0.08f);
        trackRect.pivot = new Vector2(0.5f, 0.5f);
        trackRect.anchoredPosition = Vector2.zero;
        trackRect.sizeDelta = new Vector2(0f, 7f);
        Image track = GetOrAdd<Image>(tickTrack);
        track.color = new Color(theme.border.r, theme.border.g, theme.border.b, 0.25f);
        track.raycastTarget = false;
        GameObject fillObject = GetOrCreateUiObject("TickFill", tickTrack.transform);
        Stretch((RectTransform)fillObject.transform);
        Image fill = GetOrAdd<Image>(fillObject);
        fill.color = theme.energy;
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = 0;
        fill.raycastTarget = false;

        BuildingRowUI row = root.GetComponent<BuildingRowUI>();
        row.nameText = name;
        row.levelText = null;
        row.costText = null;
        row.buyButton = buyButton;
        row.statsText = stats;
        row.tickFill = fill;
        row.rowCanvasGroup = root.GetComponent<CanvasGroup>();
        row.artifactIcon = icon;
        row.higgsIcon = theme.higgsArtifactIcon;
        row.tetraIcon = theme.tetraArtifactIcon;
        row.modulatorIcon = theme.modulatorArtifactIcon;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, RowPrefabPath);
        UnityEngine.Object.DestroyImmediate(root);
        if (prefab == null)
            throw new InvalidOperationException("No se pudo crear VerticalBuildingRow.prefab.");
        return prefab;
    }

    private static TextMeshProUGUI CreateLocalizedText(
        string name, Transform parent, string text, string key, float fontSize,
        TextAlignmentOptions alignment, Color color)
    {
        TextMeshProUGUI label = CreateOrUpdateText(
            name, parent, text, fontSize, alignment, color);
        LocalizedTMP localized = GetOrAdd<LocalizedTMP>(label.gameObject);
        localized.key = key;
        EditorUtility.SetDirty(localized);
        return label;
    }

    private static TextMeshProUGUI CreateOrUpdateText(
        string name, Transform parent, string text, float fontSize,
        TextAlignmentOptions alignment, Color color)
    {
        VerticalUiTheme theme = AssetDatabase.LoadAssetAtPath<VerticalUiTheme>(ThemePath);
        GameObject textObject = GetOrCreateUiObject(name, parent);
        TextMeshProUGUI label = GetOrAdd<TextMeshProUGUI>(textObject);
        label.text = text;
        label.font = theme != null && theme.primaryFont != null
            ? theme.primaryFont
            : TMP_Settings.defaultFontAsset;
        label.fontSize = fontSize;
        label.enableAutoSizing = true;
        label.fontSizeMin = 14f;
        label.fontSizeMax = fontSize;
        label.alignment = alignment;
        label.color = color;
        label.textWrappingMode = TextWrappingModes.Normal;
        label.raycastTarget = false;
        label.margin = new Vector4(6f, 4f, 6f, 4f);
        return label;
    }

    private static Sprite LoadSprite(string fileName)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            GeneratedFolder + "/" + fileName);
        if (sprite == null)
            throw new InvalidOperationException("No se pudo cargar " + fileName + ".");
        return sprite;
    }

    private static void SetOffsets(
        RectTransform rect, Vector2 anchorMin, Vector2 anchorMax,
        Vector2 offsetMin, Vector2 offsetMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
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
        GameObject result = new GameObject(name,
            typeof(RectTransform), typeof(CanvasRenderer));
        result.layer = 5;
        result.transform.SetParent(parent, false);
        return result;
    }

    private static Transform FindDirectChild(Transform parent, string name)
    {
        for (int index = 0; index < parent.childCount; index++)
            if (parent.GetChild(index).name == name)
                return parent.GetChild(index);
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

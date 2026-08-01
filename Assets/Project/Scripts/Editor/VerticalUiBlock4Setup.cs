#if UNITY_EDITOR
using System;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class VerticalUiBlock4Setup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ThemePath =
        "Assets/Project/UI/Vertical/Generated/VerticalUiTheme.asset";

    [MenuItem("Tools/Quantum Forge/Vertical UI/Configure Block 4 Generation Triangle")]
    public static void ConfigureBlock4GenerationTriangle()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        TabsUI tabs = UnityEngine.Object.FindFirstObjectByType<TabsUI>(
            FindObjectsInactive.Include);
        VerticalGenerationBeforeTriangleUI stateController =
            UnityEngine.Object.FindFirstObjectByType<VerticalGenerationBeforeTriangleUI>(
                FindObjectsInactive.Include);
        VerticalUiTheme theme = AssetDatabase.LoadAssetAtPath<VerticalUiTheme>(ThemePath);
        if (tabs == null || tabs.panelGeneracion == null || stateController == null ||
            stateController.buildingList == null || theme == null)
        {
            throw new InvalidOperationException("El Bloque 3 debe existir antes del Bloque 4.");
        }

        GameObject legacy = FindInScene(scene, "LegacyGenerationTriangleLayout");
        if (legacy == null && tabs.generationTriangleLayout != null &&
            tabs.generationTriangleLayout.name != "GenerationTriangleRoot")
            legacy = tabs.generationTriangleLayout;
        if (legacy == null)
            legacy = FindInScene(scene, "GenerationTriangleLayout");
        if (legacy != null)
        {
            legacy.name = "LegacyGenerationTriangleLayout";
            legacy.SetActive(false);
        }

        GameObject root = BuildTriangleRoot(
            tabs.panelGeneracion.transform, theme, out ScrollRect scroll);
        tabs.generationTriangleLayout = root;
        stateController.triangleRoot = root;

        root.SetActive(false);
        if (stateController.beforeTriangleRoot != null)
            stateController.beforeTriangleRoot.SetActive(true);

        EditorUtility.SetDirty(tabs);
        EditorUtility.SetDirty(stateController);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("No se pudo guardar Main.unity.");
        AssetDatabase.SaveAssets();
        Debug.Log("[Vertical UI Block 4] CONFIGURED | GenerationTriangleRoot | " +
            "three real circuits | synchronization and effect | shared artifact purchases");
    }

    public static void ConfigureAndValidateBatch()
    {
        VerticalUiBlock1Setup.ConfigureBlock1Base();
        VerticalUiBlock2Setup.ConfigureBlock2Navigation();
        VerticalUiBlock3Setup.ConfigureBlock3Generation();
        ConfigureBlock4GenerationTriangle();
        ConfigureBlock4GenerationTriangle();
        VerticalUiBlock1Validation.Validate();
        VerticalUiBlock2Validation.Validate();
        VerticalUiBlock3Validation.Validate();
        VerticalUiBlock4Validation.Validate();
        TriangleRedesignValidation.Validate();
        MobileQaFriendlyLayoutValidation.Validate();
        MobileButtonLegibilityValidation.Validate();
        QaMainSceneIntegrityValidation.ValidateMainSceneIntegrity();
        Debug.Log("[Vertical UI Block 4] IDEMPOTENCE PASS | setup executed twice");
    }

    private static GameObject BuildTriangleRoot(
        Transform panel, VerticalUiTheme theme, out ScrollRect scroll)
    {
        GameObject root = GetOrCreate("GenerationTriangleRoot", panel);
        RectTransform rootRect = (RectTransform)root.transform;
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = new Vector2(24f, 20f);
        rootRect.offsetMax = new Vector2(-24f, -176f);

        GameObject scrollObject = GetOrCreate("TriangleScroll", root.transform);
        Stretch((RectTransform)scrollObject.transform);
        Image scrollImage = GetOrAdd<Image>(scrollObject);
        scrollImage.color = Color.clear;
        scrollImage.raycastTarget = false;
        scroll = GetOrAdd<ScrollRect>(scrollObject);
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.scrollSensitivity = 42f;
        scroll.movementType = ScrollRect.MovementType.Elastic;

        GameObject viewport = GetOrCreate("Viewport", scrollObject.transform);
        Stretch((RectTransform)viewport.transform);
        Image viewportImage = GetOrAdd<Image>(viewport);
        viewportImage.color = new Color(1f, 1f, 1f, 0.001f);
        viewportImage.raycastTarget = true;
        GetOrAdd<RectMask2D>(viewport);
        scroll.viewport = (RectTransform)viewport.transform;

        GameObject content = GetOrCreate("Content", viewport.transform);
        RectTransform contentRect = (RectTransform)content.transform;
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(0f, 1760f);
        scroll.content = contentRect;

        TextMeshProUGUI title = CreateLocalizedText("TriangleGenerationTitle",
            content.transform, "GENERACION", "generation.title", 43f,
            TextAlignmentOptions.Center, theme.primaryText);
        SetTopRect(title.rectTransform, 0f, 88f, 0f, 0f);

        GameObject focus = GetOrCreate("TriangleFocus", content.transform);
        RectTransform focusRect = (RectTransform)focus.transform;
        focusRect.anchorMin = new Vector2(0f, 1f);
        focusRect.anchorMax = new Vector2(1f, 1f);
        focusRect.pivot = new Vector2(0.5f, 1f);
        focusRect.anchoredPosition = new Vector2(0f, -98f);
        focusRect.sizeDelta = new Vector2(-10f, 850f);
        Image focusImage = GetOrAdd<Image>(focus);
        focusImage.sprite = theme.panelFrame;
        focusImage.type = Image.Type.Sliced;
        focusImage.color = Color.white;
        focusImage.raycastTarget = false;

        Image experimentalLine = CreateLine(focus.transform, "Line_Experimental",
            new Vector2(-235f, 225f), new Vector2(235f, 225f), 16f);
        Image energyLine = CreateLine(focus.transform, "Line_Energy",
            new Vector2(-235f, 225f), new Vector2(0f, -170f), 18f);
        Image phaseLine = CreateLine(focus.transform, "Line_Phase",
            new Vector2(235f, 225f), new Vector2(0f, -170f), 16f);

        BuildVertex(focus.transform, "Vertex_Higgs", "HIGGS",
            theme.higgsArtifactIcon, theme.energy, new Vector2(-235f, 225f), theme);
        BuildVertex(focus.transform, "Vertex_Tetra", "TETRAQUARK",
            theme.tetraArtifactIcon, theme.traces, new Vector2(235f, 225f), theme);
        BuildVertex(focus.transform, "Vertex_Modulator", "MODULADOR",
            theme.modulatorArtifactIcon, theme.triangle, new Vector2(0f, -170f), theme);

        GameObject glowObject = GetOrCreate("SynchronizationCore", focus.transform);
        RectTransform glowRect = (RectTransform)glowObject.transform;
        SetCentered(glowRect, new Vector2(0f, 30f), new Vector2(190f, 190f));
        Image glow = GetOrAdd<Image>(glowObject);
        glow.sprite = theme.softGlow;
        glow.preserveAspect = true;
        glow.raycastTarget = false;

        TextMeshProUGUI protocol = CreateText("ProtocolStatus", focus.transform,
            "Circuito activo", 27f, TextAlignmentOptions.Center, theme.primaryText);
        SetCentered(protocol.rectTransform, new Vector2(0f, -315f), new Vector2(820f, 54f));
        TextMeshProUGUI synchronization = CreateText("SynchronizationStatus", focus.transform,
            "Sincronizacion: 0%", 25f, TextAlignmentOptions.Center, theme.energy);
        SetCentered(synchronization.rectTransform, new Vector2(0f, -365f), new Vector2(820f, 48f));
        TextMeshProUGUI effect = CreateText("CircuitEffect", focus.transform,
            "Efecto", 22f, TextAlignmentOptions.Center, theme.secondaryText);
        SetCentered(effect.rectTransform, new Vector2(0f, -390f), new Vector2(860f, 46f));

        GameObject circuits = GetOrCreate("CircuitSelectors", content.transform);
        RectTransform circuitsRect = (RectTransform)circuits.transform;
        circuitsRect.anchorMin = new Vector2(0f, 1f);
        circuitsRect.anchorMax = new Vector2(1f, 1f);
        circuitsRect.pivot = new Vector2(0.5f, 1f);
        circuitsRect.anchoredPosition = new Vector2(0f, -966f);
        circuitsRect.sizeDelta = new Vector2(-10f, 140f);
        HorizontalLayoutGroup circuitLayout = GetOrAdd<HorizontalLayoutGroup>(circuits);
        circuitLayout.padding = new RectOffset(8, 8, 8, 8);
        circuitLayout.spacing = 14f;
        circuitLayout.childControlWidth = true;
        circuitLayout.childControlHeight = true;
        circuitLayout.childForceExpandWidth = true;
        circuitLayout.childForceExpandHeight = true;

        TriangleSlotUI energy = BuildCircuit(circuits.transform, "Circuit_Energy",
            TriangleSlotRole.Primary, theme, out TextMeshProUGUI energyLabel);
        TriangleSlotUI experimental = BuildCircuit(circuits.transform, "Circuit_Experimental",
            TriangleSlotRole.Reinforcement, theme, out TextMeshProUGUI experimentalLabel);
        TriangleSlotUI phase = BuildCircuit(circuits.transform, "Circuit_Phase",
            TriangleSlotRole.Alteration, theme, out TextMeshProUGUI phaseLabel);

        TrianglePanelUI trianglePanel = GetOrAdd<TrianglePanelUI>(focus);
        SerializedObject panelSerialized = new SerializedObject(trianglePanel);
        panelSerialized.FindProperty("assignedLabelPrimary").objectReferenceValue = energyLabel;
        panelSerialized.FindProperty("assignedLabelReinforcement").objectReferenceValue = experimentalLabel;
        panelSerialized.FindProperty("assignedLabelAlteration").objectReferenceValue = phaseLabel;
        panelSerialized.FindProperty("protocolStatusLabel").objectReferenceValue = protocol;
        panelSerialized.FindProperty("modulatorModeLabel").objectReferenceValue = synchronization;
        panelSerialized.FindProperty("modulatorEffectLabel").objectReferenceValue = effect;
        panelSerialized.ApplyModifiedPropertiesWithoutUndo();

        VerticalTrianglePresentationUI presentation =
            GetOrAdd<VerticalTrianglePresentationUI>(focus);
        presentation.energyLine = energyLine;
        presentation.experimentalLine = experimentalLine;
        presentation.phaseLine = phaseLine;
        presentation.centerGlow = glow;
        presentation.inactiveColor = new Color(theme.border.r, theme.border.g, theme.border.b, 0.45f);
        presentation.energyColor = theme.energy;
        presentation.experimentalColor = theme.traces;
        presentation.phaseColor = theme.triangle;

        TextMeshProUGUI artifactsTitle = CreateLocalizedText("TriangleArtifactsTitle",
            content.transform, "ARTEFACTOS", "generation.artifacts", 29f,
            TextAlignmentOptions.MidlineLeft, theme.energy);
        SetTopRect(artifactsTitle.rectTransform, 1124f, 64f, 28f, -28f);

        GameObject cards = GetOrCreate("TriangleArtifactCards", content.transform);
        RectTransform cardsRect = (RectTransform)cards.transform;
        cardsRect.anchorMin = new Vector2(0f, 1f);
        cardsRect.anchorMax = new Vector2(1f, 1f);
        cardsRect.pivot = new Vector2(0.5f, 1f);
        cardsRect.anchoredPosition = new Vector2(0f, -1192f);
        cardsRect.sizeDelta = new Vector2(-10f, 540f);
        VerticalLayoutGroup cardsLayout = GetOrAdd<VerticalLayoutGroup>(cards);
        cardsLayout.spacing = 14f;
        cardsLayout.padding = new RectOffset(8, 8, 8, 8);
        cardsLayout.childControlWidth = true;
        cardsLayout.childControlHeight = true;
        cardsLayout.childForceExpandWidth = true;
        cardsLayout.childForceExpandHeight = false;
        BuildArtifactCard(cards.transform, "TriangleCard_Higgs", "vacuum_observer",
            theme.higgsArtifactIcon, theme.energy, theme);
        BuildArtifactCard(cards.transform, "TriangleCard_Tetra", "casimir_panel",
            theme.tetraArtifactIcon, theme.traces, theme);
        BuildArtifactCard(cards.transform, "TriangleCard_Modulator", "fluctuation_antenna",
            theme.modulatorArtifactIcon, theme.triangle, theme);

        root.transform.SetSiblingIndex(2);
        return root;
    }

    private static TriangleSlotUI BuildCircuit(
        Transform parent, string name, TriangleSlotRole role,
        VerticalUiTheme theme, out TextMeshProUGUI label)
    {
        GameObject item = GetOrCreate(name, parent);
        Image image = GetOrAdd<Image>(item);
        image.sprite = theme.buttonFrame;
        image.type = Image.Type.Sliced;
        image.color = theme.deepSurface;
        Button button = GetOrAdd<Button>(item);
        button.targetGraphic = image;
        TriangleSlotUI slot = GetOrAdd<TriangleSlotUI>(item);
        SerializedObject serialized = new SerializedObject(slot);
        serialized.FindProperty("slotRole").enumValueIndex = (int)role;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        label = CreateText("Label", item.transform, name, 21f,
            TextAlignmentOptions.Center, theme.primaryText);
        Stretch(label.rectTransform);
        return slot;
    }

    private static void BuildVertex(
        Transform parent, string name, string labelText, Sprite sprite,
        Color accent, Vector2 position, VerticalUiTheme theme)
    {
        GameObject vertex = GetOrCreate(name, parent);
        RectTransform rect = (RectTransform)vertex.transform;
        SetCentered(rect, position, new Vector2(190f, 190f));
        Image frame = GetOrAdd<Image>(vertex);
        frame.sprite = theme.softGlow;
        frame.color = new Color(accent.r, accent.g, accent.b, 0.35f);
        frame.preserveAspect = true;
        frame.raycastTarget = false;
        GameObject iconObject = GetOrCreate("Icon", vertex.transform);
        RectTransform iconRect = (RectTransform)iconObject.transform;
        SetCentered(iconRect, new Vector2(0f, 8f), new Vector2(112f, 112f));
        Image icon = GetOrAdd<Image>(iconObject);
        icon.sprite = sprite;
        icon.color = accent;
        icon.preserveAspect = true;
        icon.raycastTarget = false;
        TextMeshProUGUI label = CreateText("Label", vertex.transform, labelText,
            21f, TextAlignmentOptions.Center, accent);
        RectTransform labelRect = label.rectTransform;
        labelRect.anchorMin = new Vector2(0f, 0f);
        labelRect.anchorMax = new Vector2(1f, 0f);
        labelRect.pivot = new Vector2(0.5f, 0f);
        labelRect.anchoredPosition = new Vector2(0f, -12f);
        labelRect.sizeDelta = new Vector2(30f, 44f);
    }

    private static Image CreateLine(
        Transform parent, string name, Vector2 from, Vector2 to, float thickness)
    {
        GameObject lineObject = GetOrCreate(name, parent);
        lineObject.transform.SetAsFirstSibling();
        RectTransform rect = (RectTransform)lineObject.transform;
        Vector2 delta = to - from;
        SetCentered(rect, (from + to) * 0.5f,
            new Vector2(delta.magnitude, thickness));
        rect.localEulerAngles = new Vector3(0f, 0f,
            Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
        Image line = GetOrAdd<Image>(lineObject);
        line.color = new Color(0.08f, 0.18f, 0.28f, 0.75f);
        line.raycastTarget = false;
        return line;
    }

    private static void BuildArtifactCard(
        Transform parent, string name, string buildingId, Sprite sprite,
        Color accent, VerticalUiTheme theme)
    {
        GameObject card = GetOrCreate(name, parent);
        Image background = GetOrAdd<Image>(card);
        background.sprite = theme.panelFrame;
        background.type = Image.Type.Sliced;
        background.color = Color.white;
        background.raycastTarget = false;
        LayoutElement layout = GetOrAdd<LayoutElement>(card);
        layout.preferredHeight = 160f;
        layout.minHeight = 150f;

        GameObject iconObject = GetOrCreate("Icon", card.transform);
        RectTransform iconRect = (RectTransform)iconObject.transform;
        iconRect.anchorMin = new Vector2(0f, 0.5f);
        iconRect.anchorMax = new Vector2(0f, 0.5f);
        iconRect.pivot = new Vector2(0f, 0.5f);
        iconRect.anchoredPosition = new Vector2(30f, 0f);
        iconRect.sizeDelta = new Vector2(112f, 112f);
        Image icon = GetOrAdd<Image>(iconObject);
        icon.sprite = sprite;
        icon.color = accent;
        icon.preserveAspect = true;
        icon.raycastTarget = false;

        TextMeshProUGUI title = CreateText("Name", card.transform, buildingId,
            25f, TextAlignmentOptions.MidlineLeft, theme.primaryText);
        SetAnchors(title.rectTransform,
            new Vector2(0.18f, 0.48f), new Vector2(0.72f, 0.92f));
        TextMeshProUGUI state = CreateText("State", card.transform, "Coste",
            21f, TextAlignmentOptions.MidlineLeft, accent);
        SetAnchors(state.rectTransform,
            new Vector2(0.18f, 0.10f), new Vector2(0.72f, 0.52f));

        GameObject buyObject = GetOrCreate("BuyButton", card.transform);
        RectTransform buyRect = (RectTransform)buyObject.transform;
        SetAnchors(buyRect, new Vector2(0.75f, 0.22f), new Vector2(0.97f, 0.78f));
        Image buyImage = GetOrAdd<Image>(buyObject);
        buyImage.sprite = theme.buttonFrame;
        buyImage.type = Image.Type.Sliced;
        buyImage.color = Color.white;
        Button buy = GetOrAdd<Button>(buyObject);
        buy.targetGraphic = buyImage;
        LayoutElement buyLayout = GetOrAdd<LayoutElement>(buyObject);
        buyLayout.minWidth = 160f;
        buyLayout.minHeight = 64f;
        buyLayout.preferredWidth = 190f;
        buyLayout.preferredHeight = 76f;
        buyLayout.ignoreLayout = true;
        TextMeshProUGUI buyLabel = CreateText("Label", buyObject.transform,
            "COMPRAR", 23f, TextAlignmentOptions.Center, theme.primaryText);
        Stretch(buyLabel.rectTransform);

        VerticalTriangleArtifactCardUI controller =
            GetOrAdd<VerticalTriangleArtifactCardUI>(card);
        controller.buildingId = buildingId;
        controller.nameText = title;
        controller.stateText = state;
        controller.icon = icon;
        controller.buyButton = buy;
        controller.higgsIcon = theme.higgsArtifactIcon;
        controller.tetraIcon = theme.tetraArtifactIcon;
        controller.modulatorIcon = theme.modulatorArtifactIcon;
        EditorUtility.SetDirty(controller);
    }

    private static TextMeshProUGUI CreateLocalizedText(
        string name, Transform parent, string text, string key, float size,
        TextAlignmentOptions alignment, Color color)
    {
        TextMeshProUGUI label = CreateText(name, parent, text, size, alignment, color);
        LocalizedTMP localized = GetOrAdd<LocalizedTMP>(label.gameObject);
        localized.key = key;
        return label;
    }

    private static TextMeshProUGUI CreateText(
        string name, Transform parent, string text, float size,
        TextAlignmentOptions alignment, Color color)
    {
        VerticalUiTheme theme = AssetDatabase.LoadAssetAtPath<VerticalUiTheme>(ThemePath);
        GameObject textObject = GetOrCreate(name, parent);
        TextMeshProUGUI label = GetOrAdd<TextMeshProUGUI>(textObject);
        label.text = text;
        label.font = theme.primaryFont != null ? theme.primaryFont : TMP_Settings.defaultFontAsset;
        label.fontSize = size;
        label.enableAutoSizing = true;
        label.fontSizeMin = 12f;
        label.fontSizeMax = size;
        label.alignment = alignment;
        label.color = color;
        label.textWrappingMode = TextWrappingModes.Normal;
        label.raycastTarget = false;
        label.margin = new Vector4(5f, 4f, 5f, 4f);
        return label;
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

    private static void SetCentered(RectTransform rect, Vector2 position, Vector2 size)
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

    private static T GetOrAdd<T>(GameObject target) where T : Component
    {
        T value = target.GetComponent<T>();
        return value != null ? value : target.AddComponent<T>();
    }

    private static GameObject GetOrCreate(string name, Transform parent)
    {
        Transform existing = parent.Find(name);
        if (existing != null && existing.parent == parent)
            return existing.gameObject;
        GameObject result = new GameObject(name,
            typeof(RectTransform), typeof(CanvasRenderer));
        result.layer = 5;
        result.transform.SetParent(parent, false);
        return result;
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

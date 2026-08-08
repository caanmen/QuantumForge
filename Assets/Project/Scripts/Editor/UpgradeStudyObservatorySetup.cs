#if UNITY_EDITOR
using System;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class UpgradeStudyObservatorySetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ThemePath =
        "Assets/Project/UI/Vertical/Generated/VerticalUiTheme.asset";

    [MenuItem("Tools/Quantum Forge/Studies/Configure Observatory")]
    public static void ConfigureObservatory()
    {
        VerticalUiTheme theme = AssetDatabase.LoadAssetAtPath<VerticalUiTheme>(ThemePath);
        if (theme == null) throw new InvalidOperationException("Falta VerticalUiTheme.");

        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        BuildTriangleObservatory(scene, theme);
        BuildCompletedFilter(scene, theme);
        HideLegacyResearchNavigation(scene);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[Upgrade Studies] CONFIGURED | Observatory + upgrade discoveries + completed filter");
    }

    public static void ConfigureAndValidateBatch()
    {
        ConfigureObservatory();
        ConfigureObservatory();
        ValidateScene();
        Debug.Log("[Upgrade Studies] IDEMPOTENCE PASS | setup executed twice");
    }

    private static void BuildTriangleObservatory(Scene scene, VerticalUiTheme theme)
    {
        GameObject oldObservatory = FindNamed(scene, "TriangleObservatory");
        if (oldObservatory != null)
            UnityEngine.Object.DestroyImmediate(oldObservatory);

        VerticalUpgradesScreenUI screen = UnityEngine.Object.FindFirstObjectByType<
            VerticalUpgradesScreenUI>(FindObjectsInactive.Include);
        if (screen == null || screen.content == null)
            throw new InvalidOperationException("Falta la pantalla vertical de Mejoras.");

        Transform content = screen.content;
        GameObject panel = GetOrCreate("UpgradeStudyConsole", content);
        RectTransform panelRect = (RectTransform)panel.transform;
        panelRect.sizeDelta = Vector2.zero;
        LayoutElement panelLayout = GetOrAdd<LayoutElement>(panel);
        panelLayout.preferredHeight = 560f;
        panelLayout.minHeight = 520f;
        panel.transform.SetSiblingIndex(Mathf.Min(1, content.childCount - 1));
        Image panelImage = GetOrAdd<Image>(panel);
        panelImage.sprite = theme.panelFrame;
        panelImage.type = Image.Type.Sliced;
        panelImage.color = Color.white;
        panelImage.raycastTarget = true;

        TextMeshProUGUI title = CreateText("Title", panel.transform, "SINTONIZACIÓN EXPERIMENTAL",
            30f, TextAlignmentOptions.Center, theme.energy, theme);
        SetTop(title.rectTransform, 18f, 54f, 24f);
        TextMeshProUGUI status = CreateText("Status", panel.transform,
            "Consola disponible", 23f, TextAlignmentOptions.Center,
            theme.primaryText, theme);
        SetTop(status.rectTransform, 74f, 46f, 30f);
        TextMeshProUGUI detail = CreateText("Detail", panel.transform,
            "Selecciona una mejora sin descubrir.", 20f, TextAlignmentOptions.Center,
            theme.secondaryText, theme);
        SetTop(detail.rectTransform, 116f, 70f, 38f);

        GameObject progressObject = GetOrCreate("Progress", panel.transform);
        RectTransform progressRect = (RectTransform)progressObject.transform;
        SetTop(progressRect, 184f, 24f, 70f);
        Image progressBackground = GetOrAdd<Image>(progressObject);
        progressBackground.color = theme.deepSurface;
        Slider progress = GetOrAdd<Slider>(progressObject);
        progress.minValue = 0f;
        progress.maxValue = 1f;
        progress.interactable = false;
        GameObject fillArea = GetOrCreate("Fill Area", progressObject.transform);
        Stretch((RectTransform)fillArea.transform, 4f);
        GameObject fillObject = GetOrCreate("Fill", fillArea.transform);
        Stretch((RectTransform)fillObject.transform, 0f);
        Image fill = GetOrAdd<Image>(fillObject);
        fill.color = theme.energy;
        fill.raycastTarget = false;
        progress.fillRect = (RectTransform)fillObject.transform;
        progress.targetGraphic = fill;

        Transform oldOptions = panel.transform.Find("Opportunities");
        if (oldOptions != null) UnityEngine.Object.DestroyImmediate(oldOptions.gameObject);

        GameObject tuning = GetOrCreate("Tuning", panel.transform);
        RectTransform tuningRect = (RectTransform)tuning.transform;
        SetTop(tuningRect, 226f, 244f, 46f);

        TextMeshProUGUI amplitudeLabel = CreateText("AmplitudeLabel", tuning.transform,
            "AMPLITUD", 19f, TextAlignmentOptions.MidlineLeft,
            theme.secondaryText, theme);
        SetTop(amplitudeLabel.rectTransform, 0f, 34f, 0f);
        Slider amplitude = BuildSlider("Amplitude", tuning.transform, theme, 38f);

        TextMeshProUGUI frequencyLabel = CreateText("FrequencyLabel", tuning.transform,
            "FRECUENCIA", 19f, TextAlignmentOptions.MidlineLeft,
            theme.secondaryText, theme);
        SetTop(frequencyLabel.rectTransform, 82f, 34f, 0f);
        Slider frequency = BuildSlider("Frequency", tuning.transform, theme, 120f);

        TextMeshProUGUI hint = CreateText("Hint", tuning.transform,
            "Firma 1/3 · Coincidencia: 0%", 18f,
            TextAlignmentOptions.Center, theme.primaryText, theme);
        SetTop(hint.rectTransform, 160f, 74f, 0f);

        Button tune = BuildButton("TuneButton", panel.transform,
            "AJUSTA LA SEÑAL", theme, out TextMeshProUGUI tuneLabel);
        RectTransform tuneRect = (RectTransform)tune.transform;
        tuneRect.anchorMin = new Vector2(0.18f, 0f);
        tuneRect.anchorMax = new Vector2(0.82f, 0f);
        tuneRect.pivot = new Vector2(0.5f, 0f);
        tuneRect.anchoredPosition = new Vector2(0f, 24f);
        tuneRect.sizeDelta = new Vector2(0f, 70f);
        LayoutElement tuneLayout = tune.GetComponent<LayoutElement>();
        if (tuneLayout != null) tuneLayout.ignoreLayout = true;

        Button conclusion = BuildButton("ConclusionButton", panel.transform,
            "REVELAR", theme, out TextMeshProUGUI conclusionLabel);
        RectTransform conclusionRect = (RectTransform)conclusion.transform;
        conclusionRect.anchorMin = new Vector2(0.18f, 0f);
        conclusionRect.anchorMax = new Vector2(0.82f, 0f);
        conclusionRect.pivot = new Vector2(0.5f, 0f);
        conclusionRect.anchoredPosition = new Vector2(0f, 24f);
        conclusionRect.sizeDelta = new Vector2(0f, 76f);
        LayoutElement conclusionLayout = conclusion.GetComponent<LayoutElement>();
        if (conclusionLayout != null) conclusionLayout.ignoreLayout = true;

        VerticalTriangleObservatoryUI controller =
            GetOrAdd<VerticalTriangleObservatoryUI>(panel);
        controller.titleText = title;
        controller.statusText = status;
        controller.detailText = detail;
        controller.progressSlider = progress;
        controller.conclusionButton = conclusion;
        controller.conclusionButtonText = conclusionLabel;
        controller.opportunityButtons = Array.Empty<Button>();
        controller.opportunityLabels = Array.Empty<TMP_Text>();
        controller.tuningRoot = tuning;
        controller.amplitudeSlider = amplitude;
        controller.frequencySlider = frequency;
        controller.tuningHintText = hint;
        controller.tuneButton = tune;
        controller.tuneButtonText = tuneLabel;
        EditorUtility.SetDirty(controller);

        EnsureEnergyUpgradeRow(screen);
    }

    private static Slider BuildSlider(
        string name, Transform parent, VerticalUiTheme theme, float top)
    {
        GameObject root = GetOrCreate(name, parent);
        RectTransform rect = (RectTransform)root.transform;
        SetTop(rect, top, 34f, 0f);
        Image background = GetOrAdd<Image>(root);
        background.color = theme.deepSurface;
        Slider slider = GetOrAdd<Slider>(root);
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0.5f;
        slider.direction = Slider.Direction.LeftToRight;

        GameObject fillArea = GetOrCreate("Fill Area", root.transform);
        Stretch((RectTransform)fillArea.transform, 4f);
        GameObject fillObject = GetOrCreate("Fill", fillArea.transform);
        Stretch((RectTransform)fillObject.transform, 0f);
        Image fill = GetOrAdd<Image>(fillObject);
        fill.color = theme.energy;
        slider.fillRect = (RectTransform)fillObject.transform;

        GameObject handleArea = GetOrCreate("Handle Slide Area", root.transform);
        Stretch((RectTransform)handleArea.transform, 8f);
        GameObject handleObject = GetOrCreate("Handle", handleArea.transform);
        RectTransform handleRect = (RectTransform)handleObject.transform;
        handleRect.sizeDelta = new Vector2(28f, 42f);
        Image handle = GetOrAdd<Image>(handleObject);
        handle.sprite = theme.selectedButtonFrame;
        handle.type = Image.Type.Sliced;
        handle.color = Color.white;
        slider.handleRect = handleRect;
        slider.targetGraphic = handle;
        return slider;
    }

    private static void EnsureEnergyUpgradeRow(VerticalUpgradesScreenUI screen)
    {
        foreach (F2UpgradeRowUI existing in screen.GetComponentsInChildren<
            F2UpgradeRowUI>(true))
        {
            if (existing.UpgradeId == "triangle_energy_efficiency")
            {
                AddRowReference(screen, existing);
                return;
            }
        }

        F2UpgradeRowUI source = null;
        foreach (F2UpgradeRowUI candidate in screen.GetComponentsInChildren<
            F2UpgradeRowUI>(true))
        {
            if (candidate.UpgradeId == "triangle_persistence_anchor")
            {
                source = candidate;
                break;
            }
        }
        if (source == null || screen.triangleSection == null)
            throw new InvalidOperationException("Falta fila base para Captación Resonante.");

        GameObject clone = UnityEngine.Object.Instantiate(
            source.gameObject, screen.triangleSection);
        clone.name = "Upgrade_triangle_energy_efficiency";
        F2UpgradeRowUI row = clone.GetComponent<F2UpgradeRowUI>();
        row.Configure("triangle_energy_efficiency", row.TitleText, row.TierText,
            row.CostText, row.DescriptionText, row.BuyButton);
        AddRowReference(screen, row);
        EditorUtility.SetDirty(row);
    }

    private static void AddRowReference(
        VerticalUpgradesScreenUI screen, F2UpgradeRowUI row)
    {
        var rows = new System.Collections.Generic.List<F2UpgradeRowUI>(
            screen.rows ?? Array.Empty<F2UpgradeRowUI>());
        if (!rows.Contains(row)) rows.Add(row);
        screen.rows = rows.ToArray();
        EditorUtility.SetDirty(screen);
    }

    private static void BuildCompletedFilter(Scene scene, VerticalUiTheme theme)
    {
        VerticalUpgradesScreenUI screen = UnityEngine.Object.FindFirstObjectByType<
            VerticalUpgradesScreenUI>(FindObjectsInactive.Include);
        if (screen == null || screen.content == null)
            throw new InvalidOperationException("Falta la pantalla vertical de Mejoras.");

        Button button = BuildButton("HideCompletedButton", screen.content,
            "OCULTAR COMPLETADAS", theme, out TextMeshProUGUI label);
        LayoutElement layout = GetOrAdd<LayoutElement>(button.gameObject);
        layout.preferredHeight = 70f;
        layout.minHeight = 64f;
        button.transform.SetSiblingIndex(0);
        screen.hideCompletedButton = button;
        screen.hideCompletedLabel = label;
        EditorUtility.SetDirty(screen);
    }

    private static void HideLegacyResearchNavigation(Scene scene)
    {
        GameObject research = FindNamed(scene, "Nav_Research");
        if (research != null) research.SetActive(false);
    }

    private static Button BuildButton(
        string name, Transform parent, string text, VerticalUiTheme theme,
        out TextMeshProUGUI label)
    {
        GameObject buttonObject = GetOrCreate(name, parent);
        Image image = GetOrAdd<Image>(buttonObject);
        image.sprite = theme.buttonFrame;
        image.type = Image.Type.Sliced;
        image.color = Color.white;
        Button button = GetOrAdd<Button>(buttonObject);
        button.targetGraphic = image;
        LayoutElement layout = GetOrAdd<LayoutElement>(buttonObject);
        layout.preferredHeight = 68f;
        layout.minHeight = 62f;
        label = CreateText("Label", buttonObject.transform, text, 21f,
            TextAlignmentOptions.Center, theme.primaryText, theme);
        Stretch(label.rectTransform, 8f);
        return button;
    }

    private static TextMeshProUGUI CreateText(
        string name, Transform parent, string text, float size,
        TextAlignmentOptions alignment, Color color, VerticalUiTheme theme)
    {
        GameObject target = GetOrCreate(name, parent);
        TextMeshProUGUI label = GetOrAdd<TextMeshProUGUI>(target);
        label.text = text;
        label.font = theme.primaryFont != null ? theme.primaryFont : TMP_Settings.defaultFontAsset;
        label.fontSize = size;
        label.enableAutoSizing = true;
        label.fontSizeMin = 13f;
        label.fontSizeMax = size;
        label.alignment = alignment;
        label.color = color;
        label.textWrappingMode = TextWrappingModes.Normal;
        label.overflowMode = TextOverflowModes.Ellipsis;
        label.raycastTarget = false;
        return label;
    }

    private static void ValidateScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        GameObject legacyObservatory = FindNamed(scene, "TriangleObservatory");
        GameObject console = FindNamed(scene, "UpgradeStudyConsole");
        VerticalUpgradesScreenUI upgrades = UnityEngine.Object.FindFirstObjectByType<
            VerticalUpgradesScreenUI>(FindObjectsInactive.Include);
        GameObject research = FindNamed(scene, "Nav_Research");
        VerticalTriangleObservatoryUI controller = console != null
            ? console.GetComponent<VerticalTriangleObservatoryUI>()
            : null;
        bool hasEnergyRow = false;
        if (upgrades != null)
        {
            foreach (F2UpgradeRowUI row in upgrades.GetComponentsInChildren<F2UpgradeRowUI>(true))
                if (row.UpgradeId == "triangle_energy_efficiency") hasEnergyRow = true;
        }
        if (legacyObservatory != null || console == null || controller == null ||
            controller.amplitudeSlider == null || controller.frequencySlider == null ||
            controller.tuneButton == null || !hasEnergyRow ||
            upgrades == null || upgrades.hideCompletedButton == null ||
            research == null || research.activeSelf)
            throw new InvalidOperationException("Falló la validación de la consola de estudios.");
    }

    private static GameObject FindNamed(Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
            foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
                if (current.name == name) return current.gameObject;
        return null;
    }

    private static GameObject GetOrCreate(string name, Transform parent)
    {
        Transform existing = parent.Find(name);
        if (existing != null && existing.parent == parent) return existing.gameObject;
        GameObject result = new GameObject(name,
            typeof(RectTransform), typeof(CanvasRenderer));
        result.layer = 5;
        result.transform.SetParent(parent, false);
        return result;
    }

    private static T GetOrAdd<T>(GameObject target) where T : Component
    {
        T component = target.GetComponent<T>();
        return component != null ? component : target.AddComponent<T>();
    }

    private static void SetTop(RectTransform rect, float top, float height, float margin)
    {
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -top);
        rect.sizeDelta = new Vector2(-margin * 2f, height);
    }

    private static void Stretch(RectTransform rect, float inset)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = new Vector2(inset, inset);
        rect.offsetMax = new Vector2(-inset, -inset);
    }
}
#endif

#if UNITY_EDITOR
using System;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Reuses the approved Generation metal/lab vocabulary on the operational fusion
/// panel. It deliberately preserves every button and MachineFusionPanelVisualUI
/// reference created by MachineFusionPanelVisualSetup.
/// </summary>
public static class MachineFusionApprovedStyleSetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string LabBackgroundPath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_lab_accident_background_v3.png";
    private const string ModuleCardPath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_module_card_metal_v2.png";
    private const string ResourceFramePath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_resource_counter_metal_v2.png";
    private const string SelectorFramePath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_selector_frame.png";

    private static readonly Color Violet = new Color(0.72f, 0.29f, 0.89f, 1f);
    private static readonly Color Cyan = new Color(0f, 0.78f, 0.94f, 1f);
    private static readonly Color Amber = new Color(0.94f, 0.58f, 0.08f, 1f);
    private static readonly Color Metal = new Color(0.68f, 0.72f, 0.74f, 0.88f);

    [MenuItem("Tools/Quantum Forge/Machine/Apply Approved Fusion Style")]
    public static void Configure()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        ApplyToOpenScene(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[Machine Fusion Approved Style] CONFIGURED | accident lab | " +
            "metal modules | operational controls preserved");
    }

    public static void ApplyToOpenScene(Scene scene)
    {
        Room2PanelUI room = UnityEngine.Object.FindFirstObjectByType<Room2PanelUI>(
            FindObjectsInactive.Include);
        Require(room != null, "No se encontró Room2PanelUI.");
        Transform root = room.transform.Find("LegacyFusionPanel");
        Require(root != null, "No se encontró LegacyFusionPanel.");
        Transform shell = RequireChild(root, "FusionVisualShell");

        Sprite lab = LoadSprite(LabBackgroundPath);
        Sprite module = LoadSprite(ModuleCardPath);
        Sprite resource = LoadSprite(ResourceFramePath);
        Sprite selector = LoadSprite(SelectorFramePath);

        Image shellImage = RequireImage(shell);
        shellImage.sprite = lab;
        shellImage.type = Image.Type.Simple;
        shellImage.preserveAspect = false;
        // Same restrained accident-lab exposure approved for the cube chamber:
        // environmental detail remains readable without looking illuminated.
        shellImage.color = new Color(0.76f, 0.78f, 0.79f, 1f);

        RebuildShade(shell);
        StylePanel(shell.Find("FusionInventoryStrip"), resource,
            new Color(0.72f, 0.76f, 0.78f, 0.92f), Image.Type.Simple);
        StylePanel(shell.Find("FusionStatusStrip"), resource,
            new Color(0.70f, 0.74f, 0.76f, 0.94f), Image.Type.Simple);

        foreach (string panelName in new[]
        {
            "FragmentA", "FragmentB", "Catalyst", "ReactionChamber",
            "CompositionReading", "Instability", "ResultConsole", "CoreDiagnostic"
        })
        {
            StylePanel(shell.Find(panelName), module, Metal, Image.Type.Simple);
        }

        StyleSlot(shell.Find("FragmentA"), resource, Cyan);
        StyleSlot(shell.Find("FragmentB"), resource, Violet);
        StyleSlot(shell.Find("Catalyst"), resource, Amber);

        StyleButton(shell.Find("ModeButton"), selector, Violet);
        StyleButton(shell.Find("GuidedIntentButton"), selector, Violet);
        StyleWideButton(shell.Find("FusionButton"), resource, Violet);
        StyleWideButton(shell.Find("LogButton"), resource,
            new Color(0.70f, 0.74f, 0.76f, 1f));

        Transform instability = shell.Find("Instability");
        if (instability != null)
            StyleButton(instability.Find("CoolButton"), selector, Cyan);

        AlignTopStrips(shell);
        AlignAnalysisAndActions(shell);
        StyleResultArea(shell, resource);
        PolishTypography(shell);

        EditorUtility.SetDirty(root.gameObject);
        EditorSceneManager.MarkSceneDirty(scene);
    }

    public static void ConfigureBatch()
    {
        try
        {
            Configure();
            Validate();
            Debug.Log("[Machine Fusion Approved Style] PASS");
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    public static void ConfigureFusionAndSeedsBatch()
    {
        try
        {
            Configure();
            Validate();
            MachineSeedsApprovedStyleSetup.Configure();
            MachineSeedsApprovedStyleSetup.Validate();
            Debug.Log("[Machine Fusion + Seeds Polish] PASS | layouts saved");
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    public static void Validate()
    {
        MachineFusionPanelVisualUI visual =
            UnityEngine.Object.FindFirstObjectByType<MachineFusionPanelVisualUI>(
                FindObjectsInactive.Include);
        Require(visual != null, "Falta MachineFusionPanelVisualUI.");
        Transform shell = visual.transform.Find("FusionVisualShell");
        Require(shell != null && shell.Find("ApprovedFusionShade") != null,
            "La carcasa aprobada de Mezclas no está completa.");
        Require(shell.Find("ApprovedResultFrame") != null,
            "Falta el marco unificado de resultado y diagnóstico.");
        foreach (string path in new[]
        {
            "FragmentA/Selector", "FragmentB/Selector", "Catalyst/Selector",
            "ModeButton", "GuidedIntentButton", "FusionButton", "LogButton",
            "Instability/CoolButton"
        })
        {
            Button button = shell.Find(path)?.GetComponent<Button>();
            Require(button != null, "Se perdió el control funcional: " + path);
        }
    }

    private static void RebuildShade(Transform shell)
    {
        Transform old = shell.Find("ApprovedFusionShade");
        if (old != null)
            UnityEngine.Object.DestroyImmediate(old.gameObject);
        GameObject shadeObject = CreateRect("ApprovedFusionShade", shell,
            new Vector2(0.018f, 0.025f), new Vector2(0.982f, 0.975f));
        Image shade = shadeObject.AddComponent<Image>();
        shade.color = new Color(0f, 0.012f, 0.020f, 0.22f);
        shade.raycastTarget = false;
        shadeObject.transform.SetAsFirstSibling();
    }

    private static void StyleSlot(Transform slot, Sprite resource, Color accent)
    {
        if (slot == null)
            return;
        RebuildSlotInterior(slot);
        StyleNeutralButton(slot.Find("Selector"), resource);
        TextMeshProUGUI title = slot.Find("Title")?.GetComponent<TextMeshProUGUI>();
        if (title != null)
        {
            title.color = new Color(0.90f, 0.92f, 0.93f, 1f);
            title.fontStyle = FontStyles.Bold;
            title.alignment = TextAlignmentOptions.Center;
        }

        Transform selector = slot.Find("Selector");
        TextMeshProUGUI label = selector?.Find("Label")?.GetComponent<TextMeshProUGUI>();
        if (label != null)
        {
            SetRect(label.rectTransform, new Vector2(0.18f, 0.16f),
                new Vector2(0.82f, 0.84f));
            label.alignment = TextAlignmentOptions.Center;
            label.color = new Color(accent.r, accent.g, accent.b, 0.92f);
            label.margin = Vector4.zero;
        }

        TextMeshProUGUI previous = selector?.Find("Prev")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI next = selector?.Find("Next")?.GetComponent<TextMeshProUGUI>();
        AlignSelectorArrow(previous, new Vector2(0.035f, 0.16f),
            new Vector2(0.18f, 0.84f), accent);
        AlignSelectorArrow(next, new Vector2(0.82f, 0.16f),
            new Vector2(0.965f, 0.84f), accent);
    }

    private static void RebuildSlotInterior(Transform slot)
    {
        Transform old = slot.Find("ApprovedSlotInterior");
        if (old != null)
            UnityEngine.Object.DestroyImmediate(old.gameObject);

        // The source module plate contains technical cross-lines. They looked like
        // missing content whenever a slot was empty, so cover only the flat inner
        // surface while preserving the metal title and selector bevels.
        GameObject interiorObject = CreateRect("ApprovedSlotInterior", slot,
            new Vector2(0.075f, 0.27f), new Vector2(0.925f, 0.81f));
        Image interior = interiorObject.AddComponent<Image>();
        interior.color = new Color(0.012f, 0.025f, 0.033f, 0.985f);
        interior.raycastTarget = false;
        interiorObject.transform.SetAsFirstSibling();
    }

    private static void StyleNeutralButton(Transform target, Sprite resource)
    {
        if (target == null)
            return;
        Image image = target.GetComponent<Image>();
        Button button = target.GetComponent<Button>();
        if (image == null || button == null)
            return;
        image.sprite = resource;
        image.type = Image.Type.Simple;
        image.preserveAspect = false;
        image.color = new Color(0.70f, 0.74f, 0.76f, 0.92f);
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 1f, 1f, 1f);
        colors.pressedColor = new Color(0.76f, 0.79f, 0.81f, 1f);
        colors.selectedColor = Color.white;
        colors.disabledColor = new Color(0.48f, 0.50f, 0.52f, 0.72f);
        button.colors = colors;
    }

    private static void AlignSelectorArrow(TextMeshProUGUI arrow,
        Vector2 min, Vector2 max, Color accent)
    {
        if (arrow == null)
            return;
        SetRect(arrow.rectTransform, min, max);
        arrow.alignment = TextAlignmentOptions.Center;
        arrow.color = new Color(accent.r, accent.g, accent.b, 0.72f);
        arrow.margin = Vector4.zero;
    }

    private static void AlignTopStrips(Transform shell)
    {
        CenterStripText(shell.Find("FusionInventoryStrip/InventoryText"),
            new Vector2(0.055f, 0.17f), new Vector2(0.945f, 0.83f));
        CenterStripText(shell.Find("FusionStatusStrip/FusionStatus"),
            new Vector2(0.055f, 0.17f), new Vector2(0.945f, 0.83f));
    }

    private static void AlignAnalysisAndActions(Transform shell)
    {
        RectTransform composition = shell.Find("CompositionReading") as RectTransform;
        RectTransform instability = shell.Find("Instability") as RectTransform;
        SetRect(composition, new Vector2(0.035f, 0.230f),
            new Vector2(0.490f, 0.360f));
        SetRect(instability, new Vector2(0.510f, 0.230f),
            new Vector2(0.965f, 0.360f));

        if (composition != null)
        {
            SetTextRect(composition.Find("Heading"), new Vector2(0.12f, 0.73f),
                new Vector2(0.88f, 0.91f), TextAlignmentOptions.Center, 19f, 16f);
            SetTextRect(composition.Find("Reading"), new Vector2(0.11f, 0.22f),
                new Vector2(0.89f, 0.70f), TextAlignmentOptions.Left, 17f, 14f);
            SetRect(composition.Find("RiskSegments") as RectTransform,
                new Vector2(0.12f, 0.09f), new Vector2(0.88f, 0.16f));
        }

        if (instability != null)
        {
            SetTextRect(instability.Find("InstabilityText"),
                new Vector2(0.12f, 0.62f), new Vector2(0.88f, 0.90f),
                TextAlignmentOptions.Center, 18f, 15f);
            SetRect(instability.Find("InstabilitySegments") as RectTransform,
                new Vector2(0.13f, 0.44f), new Vector2(0.87f, 0.54f));
            SetRect(instability.Find("CoolButton") as RectTransform,
                new Vector2(0.12f, 0.11f), new Vector2(0.88f, 0.36f));
            SetTextRect(instability.Find("CoolButton/Label"),
                new Vector2(0.08f, 0.12f), new Vector2(0.92f, 0.88f),
                TextAlignmentOptions.Center, 18f, 15f);
        }

        SetRect(shell.Find("FusionButton") as RectTransform,
            new Vector2(0.055f, 0.145f), new Vector2(0.620f, 0.205f));
        SetRect(shell.Find("LogButton") as RectTransform,
            new Vector2(0.650f, 0.145f), new Vector2(0.945f, 0.205f));
        SetTextRect(shell.Find("FusionButton/Label"),
            new Vector2(0.08f, 0.12f), new Vector2(0.92f, 0.88f),
            TextAlignmentOptions.Center, 25f, 18f);
        SetTextRect(shell.Find("LogButton/Label"),
            new Vector2(0.10f, 0.12f), new Vector2(0.90f, 0.88f),
            TextAlignmentOptions.Center, 22f, 17f);
    }

    private static void SetTextRect(Transform target, Vector2 min, Vector2 max,
        TextAlignmentOptions alignment, float maxSize, float minSize)
    {
        TextMeshProUGUI text = target?.GetComponent<TextMeshProUGUI>();
        if (text == null)
            return;
        SetRect(text.rectTransform, min, max);
        text.alignment = alignment;
        text.enableAutoSizing = true;
        text.fontSize = maxSize;
        text.fontSizeMax = maxSize;
        text.fontSizeMin = minSize;
        text.margin = Vector4.zero;
    }

    private static void CenterStripText(Transform target, Vector2 min, Vector2 max)
    {
        TextMeshProUGUI text = target?.GetComponent<TextMeshProUGUI>();
        if (text == null)
            return;
        SetRect(text.rectTransform, min, max);
        text.alignment = TextAlignmentOptions.Center;
        text.margin = Vector4.zero;
    }

    private static void StyleResultArea(Transform shell, Sprite resource)
    {
        Transform old = shell.Find("ApprovedResultFrame");
        if (old != null)
            UnityEngine.Object.DestroyImmediate(old.gameObject);

        GameObject frameObject = CreateRect("ApprovedResultFrame", shell,
            new Vector2(0.025f, 0.003f), new Vector2(0.975f, 0.128f));
        Image frame = frameObject.AddComponent<Image>();
        frame.sprite = resource;
        frame.type = Image.Type.Simple;
        frame.preserveAspect = false;
        frame.color = new Color(0.72f, 0.76f, 0.78f, 0.96f);
        frame.raycastTarget = false;
        frameObject.transform.SetSiblingIndex(Mathf.Min(1, shell.childCount - 1));

        RectTransform result = shell.Find("ResultConsole") as RectTransform;
        RectTransform diagnostic = shell.Find("CoreDiagnostic") as RectTransform;
        // Reserve a real safe area above the ornamental lower bevel. The previous
        // values only moved these rows a few pixels and they still read as if they
        // were painted over the frame.
        // Center the complete information group in the flat inner plate, using
        // symmetrical side margins and balanced space above and below.
        SetRect(result, new Vector2(0.105f, 0.054f), new Vector2(0.895f, 0.111f));
        SetRect(diagnostic, new Vector2(0.105f, 0.029f), new Vector2(0.895f, 0.052f));
        ClearPanelImage(result);
        ClearPanelImage(diagnostic);

        if (result != null)
        {
            SetRect(result.Find("ResultIcon") as RectTransform,
                new Vector2(0.015f, 0.02f), new Vector2(0.135f, 0.82f));
            SetRect(result.Find("ResultTitle") as RectTransform,
                new Vector2(0.145f, 0.50f), new Vector2(0.965f, 0.80f));
            SetRect(result.Find("ResultDetail") as RectTransform,
                new Vector2(0.145f, 0.22f), new Vector2(0.965f, 0.52f));
            SetRect(result.Find("ResultMeta") as RectTransform,
                new Vector2(0.145f, 0.02f), new Vector2(0.965f, 0.24f));
        }

        if (diagnostic != null)
        {
            SetRect(diagnostic.Find("DiagnosticText") as RectTransform,
                new Vector2(0.02f, 0.04f), new Vector2(0.62f, 0.96f));
            SetRect(diagnostic.Find("DiagnosticGraph") as RectTransform,
                new Vector2(0.66f, 0.10f), new Vector2(0.97f, 0.90f));
        }
    }

    private static void ClearPanelImage(RectTransform panel)
    {
        Image image = panel != null ? panel.GetComponent<Image>() : null;
        if (image != null)
        {
            image.sprite = null;
            image.color = Color.clear;
        }
    }

    private static void StyleButton(Transform target, Sprite selector, Color accent)
    {
        if (target == null)
            return;
        Image image = target.GetComponent<Image>();
        Button button = target.GetComponent<Button>();
        if (image == null || button == null)
            return;
        image.sprite = selector;
        image.type = Image.Type.Sliced;
        image.color = new Color(accent.r, accent.g, accent.b, 0.82f);
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 1f, 1f, 1f);
        colors.pressedColor = new Color(0.72f, 0.76f, 0.80f, 1f);
        colors.selectedColor = Color.white;
        colors.disabledColor = new Color(0.48f, 0.50f, 0.52f, 0.72f);
        button.colors = colors;
    }

    private static void StyleWideButton(Transform target, Sprite resource,
        Color accent)
    {
        if (target == null)
            return;
        Image image = target.GetComponent<Image>();
        Button button = target.GetComponent<Button>();
        if (image == null || button == null)
            return;
        RebuildWideButtonInterior(target);
        image.sprite = resource;
        image.type = Image.Type.Simple;
        image.preserveAspect = false;
        image.color = new Color(accent.r, accent.g, accent.b, 0.90f);
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = Color.white;
        colors.pressedColor = new Color(0.72f, 0.76f, 0.80f, 1f);
        colors.selectedColor = Color.white;
        colors.disabledColor = new Color(0.48f, 0.50f, 0.52f, 0.72f);
        button.colors = colors;
    }

    private static void RebuildWideButtonInterior(Transform target)
    {
        Transform old = target.Find("ApprovedButtonInterior");
        if (old != null)
            UnityEngine.Object.DestroyImmediate(old.gameObject);
        GameObject plateObject = CreateRect("ApprovedButtonInterior", target,
            new Vector2(0.055f, 0.17f), new Vector2(0.945f, 0.83f));
        Image plate = plateObject.AddComponent<Image>();
        plate.color = new Color(0.010f, 0.020f, 0.028f, 0.97f);
        plate.raycastTarget = false;
        plateObject.transform.SetAsFirstSibling();
    }

    private static void StylePanel(Transform target, Sprite sprite, Color color,
        Image.Type type)
    {
        if (target == null)
            return;
        Image image = target.GetComponent<Image>();
        if (image == null)
            return;
        image.sprite = sprite;
        image.type = type;
        image.preserveAspect = false;
        image.color = color;
        image.raycastTarget = false;
    }

    private static void PolishTypography(Transform shell)
    {
        TextMeshProUGUI title = shell.Find("FusionTitle")?.GetComponent<TextMeshProUGUI>();
        if (title != null)
        {
            title.fontSize = 36f;
            title.fontSizeMin = 29f;
            title.characterSpacing = 3f;
            title.color = new Color(0.91f, 0.92f, 0.93f, 1f);
        }
        TextMeshProUGUI subtitle = shell.Find("FusionSubtitle")?.GetComponent<TextMeshProUGUI>();
        if (subtitle != null)
            subtitle.color = Violet;

        foreach (TextMeshProUGUI text in shell.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (text == null || text == title || text == subtitle)
                continue;
            text.enableAutoSizing = true;
            text.fontSizeMin = Mathf.Min(text.fontSizeMin, 13f);
            text.overflowMode = TextOverflowModes.Ellipsis;
        }
    }

    private static Image RequireImage(Transform target)
    {
        Image image = target.GetComponent<Image>();
        Require(image != null, target.name + " no tiene Image.");
        return image;
    }

    private static Sprite LoadSprite(string path)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        Require(sprite != null, "No se encontró el recurso: " + path);
        return sprite;
    }

    private static Transform RequireChild(Transform parent, string name)
    {
        Transform child = parent.Find(name);
        Require(child != null, "No se encontró " + parent.name + "/" + name + ".");
        return child;
    }

    private static GameObject CreateRect(string name, Transform parent,
        Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject result = new GameObject(name, typeof(RectTransform));
        result.layer = parent.gameObject.layer;
        RectTransform rect = result.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
        return result;
    }

    private static void SetRect(RectTransform rect, Vector2 anchorMin,
        Vector2 anchorMax)
    {
        if (rect == null)
            return;
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
#endif

#if UNITY_EDITOR
using System;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Applies the approved accident-lab shell to the operational Seeds/Anchors view
/// while preserving MachineSeedsPanelVisualUI and every gameplay control.
/// </summary>
public static class MachineSeedsApprovedStyleSetup
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
    private const string SelectorPlatePath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_selector_metal_plate_v2.png";

    private static readonly Color Cyan = new Color(0f, 0.82f, 0.78f, 1f);
    private static readonly Color Violet = new Color(0.72f, 0.29f, 0.89f, 1f);
    private static readonly Color Amber = new Color(0.94f, 0.58f, 0.08f, 1f);
    private static readonly Color Metal = new Color(0.67f, 0.71f, 0.73f, 0.87f);

    [MenuItem("Tools/Quantum Forge/Machine/Apply Approved Seeds Style")]
    public static void Configure()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        ApplyToOpenScene(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[Machine Seeds Approved Style] CONFIGURED | accident lab | " +
            "wide operational layout | controls preserved");
    }

    public static void ApplyToOpenScene(Scene scene)
    {
        MachineSeedsPanelVisualUI visual =
            UnityEngine.Object.FindFirstObjectByType<MachineSeedsPanelVisualUI>(
                FindObjectsInactive.Include);
        Require(visual != null, "No se encontró MachineSeedsPanelVisualUI.");
        Transform root = visual.transform;
        Transform shell = RequireChild(root, "SeedsVisualShell");

        Sprite lab = LoadSprite(LabBackgroundPath);
        Sprite module = LoadSprite(ModuleCardPath);
        Sprite resource = LoadSprite(ResourceFramePath);
        Sprite selector = LoadSprite(SelectorFramePath);
        Sprite selectorPlate = LoadSprite(SelectorPlatePath);

        RectTransform shellRect = shell as RectTransform;
        Require(shellRect != null, "SeedsVisualShell no tiene RectTransform.");
        SetRect(shellRect, new Vector2(0.022f, 0.030f),
            new Vector2(0.978f, 0.838f));

        Image shellImage = RequireImage(shell);
        shellImage.sprite = lab;
        shellImage.type = Image.Type.Simple;
        shellImage.preserveAspect = false;
        shellImage.color = new Color(0.76f, 0.78f, 0.79f, 1f);

        Transform grid = shell.Find("TechnicalGrid");
        if (grid != null)
            grid.gameObject.SetActive(false);
        RebuildShade(shell);

        StylePanel(shell.Find("StatusStrip"), resource,
            new Color(0.70f, 0.75f, 0.77f, 0.92f), Image.Type.Simple);
        foreach (string panelName in new[]
        {
            "Incubation", "AnchorChamber", "Stabilization", "Actions", "Archive"
        })
        {
            // These are wide horizontal sections. The resource plate provides one
            // uninterrupted inner surface, unlike the vertically divided module card.
            StylePanel(shell.Find(panelName), resource,
                new Color(0.72f, 0.76f, 0.78f, 0.95f), Image.Type.Simple);
        }

        RemoveDecorativeRails(shell);

        Transform incubation = shell.Find("Incubation");
        if (incubation != null)
        {
            for (int i = 1; i <= 4; i++)
            {
                Transform slot = incubation.Find("SeedSlot" + i);
                // The large cyan boxes consumed most of the incubation surface.
                // Keep each row functional, but present it as a clean data line.
                ClearPanelImage(slot);
                SetActive(slot?.Find("Accent"), false);
                StylePanel(slot?.Find("Index"), selector,
                    new Color(0.53f, 0.58f, 0.60f, 0.82f), Image.Type.Sliced);
                RebuildRowSeparator(slot, i < 4);
            }
            StyleButton(incubation.Find("CreateSeed"), selector, Cyan);
            StyleButton(incubation.Find("FormAnchor"), selector,
                new Color(0.62f, 0.67f, 0.69f, 1f));
            AlignIncubation(incubation);
        }

        Transform anchor = shell.Find("AnchorChamber");
        if (anchor != null)
        {
            // AnchorChamber already owns the approved metal shell. Keep the lattice
            // and core, but remove the redundant frame nested inside it.
            ClearPanelImage(anchor.Find("LatticeFrame"));
            HideLatticeRails(anchor.Find("LatticeFrame"));
            AlignAnchorChamber(anchor);
        }

        AlignStabilization(shell.Find("Stabilization"));

        Transform actions = shell.Find("Actions");
        if (actions != null)
        {
            StyleButton(actions.Find("Stabilize"), selector, Cyan);
            StyleButton(actions.Find("Compensate"), selector, Violet);
            StyleButton(actions.Find("Materialize"), selector, Cyan);
            StyleButton(actions.Find("Discard"), selector, Amber);
            AlignActions(actions);
        }

        Transform archive = shell.Find("Archive");
        if (archive != null)
        {
            StylePanel(archive.Find("Pure"), selector,
                new Color(Cyan.r, Cyan.g, Cyan.b, 0.72f), Image.Type.Sliced);
            StylePanel(archive.Find("Stable"), selector,
                new Color(Violet.r, Violet.g, Violet.b, 0.72f), Image.Type.Sliced);
            StylePanel(archive.Find("Forced"), selector,
                new Color(Amber.r, Amber.g, Amber.b, 0.72f), Image.Type.Sliced);
            AlignArchive(archive);
        }

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
            Debug.Log("[Machine Seeds Approved Style] PASS");
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
        MachineSeedsPanelVisualUI visual =
            UnityEngine.Object.FindFirstObjectByType<MachineSeedsPanelVisualUI>(
                FindObjectsInactive.Include);
        Require(visual != null, "Falta MachineSeedsPanelVisualUI.");
        Transform shell = visual.transform.Find("SeedsVisualShell");
        Require(shell != null && shell.Find("ApprovedSeedsShade") != null,
            "La carcasa aprobada de Semillas no está completa.");
        foreach (string path in new[]
        {
            "Incubation/CreateSeed", "Incubation/FormAnchor",
            "Actions/Stabilize", "Actions/Compensate", "Actions/Materialize",
            "Actions/Discard"
        })
        {
            Button button = shell.Find(path)?.GetComponent<Button>();
            Require(button != null, "Se perdió el control funcional: " + path);
        }
        Require(shell.Find("Incubation/SeedSlot1") != null &&
            shell.Find("Incubation/SeedSlot4") != null,
            "Las cuatro ranuras de incubación no están presentes.");
        Require(shell.Find("AnchorChamber/LatticeFrame/ChronalLattice") != null,
            "La retícula de anclaje no está presente.");
    }

    private static void RebuildShade(Transform shell)
    {
        Transform old = shell.Find("ApprovedSeedsShade");
        if (old != null)
            UnityEngine.Object.DestroyImmediate(old.gameObject);
        GameObject shadeObject = CreateRect("ApprovedSeedsShade", shell,
            new Vector2(0.018f, 0.025f), new Vector2(0.982f, 0.975f));
        Image shade = shadeObject.AddComponent<Image>();
        shade.color = new Color(0f, 0.012f, 0.020f, 0.22f);
        shade.raycastTarget = false;
        shadeObject.transform.SetAsFirstSibling();
    }

    private static void RemoveDecorativeRails(Transform shell)
    {
        foreach (string name in new[]
        {
            "OuterRailLeft", "OuterRailRight", "OuterCapTop", "OuterCapBottom"
        })
            SetActive(shell.Find(name), false);

        foreach (string panelName in new[]
        {
            "Incubation", "AnchorChamber", "Stabilization", "Archive"
        })
        {
            Transform panel = shell.Find(panelName);
            SetActive(panel?.Find("SectionRail"), false);
            SetActive(panel?.Find("SectionRailTop"), false);
        }

        foreach (string panelName in new[] { "Incubation", "AnchorChamber" })
        {
            Transform panel = shell.Find(panelName);
            SetActive(panel?.Find("TitleUnderline"), false);
            SetActive(panel?.Find("TitleCap"), false);
        }
    }

    private static void AlignIncubation(Transform incubation)
    {
        CenterText(incubation.Find("Title"), new Vector2(0.12f, 0.83f),
            new Vector2(0.88f, 0.95f));
        SetRectSafe(incubation.Find("SectionIcon") as RectTransform,
            new Vector2(0.060f, 0.845f), new Vector2(0.092f, 0.945f));

        float[] bottoms = { 0.64f, 0.475f, 0.31f, 0.145f };
        float[] tops = { 0.78f, 0.615f, 0.45f, 0.285f };
        for (int i = 0; i < 4; i++)
        {
            SetRectSafe(incubation.Find("SeedSlot" + (i + 1)) as RectTransform,
                new Vector2(0.075f, bottoms[i]), new Vector2(0.620f, tops[i]));
            Transform slot = incubation.Find("SeedSlot" + (i + 1));
            SetRectSafe(slot?.Find("Index") as RectTransform,
                new Vector2(0.01f, 0.12f), new Vector2(0.105f, 0.88f));
            AlignText(slot?.Find("State"), new Vector2(0.125f, 0.50f),
                new Vector2(0.98f, 0.92f), TextAlignmentOptions.Left);
            AlignText(slot?.Find("Meta"), new Vector2(0.125f, 0.12f),
                new Vector2(0.98f, 0.43f), TextAlignmentOptions.Left);
            SetTextSize(slot?.Find("State"), 18f, 15f);
            SetTextSize(slot?.Find("Meta"), 15f, 12f);
            SetTextSize(slot?.Find("Index/Value"), 21f, 17f);
            TextMeshProUGUI meta = slot?.Find("Meta")?.GetComponent<TextMeshProUGUI>();
            if (meta != null)
                meta.color = new Color(0.62f, 0.68f, 0.71f, 1f);
            SetRectSafe(slot?.Find("ProgressTrack") as RectTransform,
                new Vector2(0.125f, 0.07f), new Vector2(0.98f, 0.11f));
        }

        SetRectSafe(incubation.Find("CreateSeed") as RectTransform,
            new Vector2(0.68f, 0.49f), new Vector2(0.89f, 0.65f));
        SetRectSafe(incubation.Find("FormAnchor") as RectTransform,
            new Vector2(0.68f, 0.29f), new Vector2(0.89f, 0.45f));
        SetRectSafe(incubation.Find("SeedCoreGlow") as RectTransform,
            new Vector2(0.74f, 0.67f), new Vector2(0.89f, 0.95f));
        SetRectSafe(incubation.Find("SeedCore") as RectTransform,
            new Vector2(0.77f, 0.70f), new Vector2(0.86f, 0.92f));
        CenterText(incubation.Find("IncubationHint"), new Vector2(0.64f, 0.15f),
            new Vector2(0.89f, 0.23f));
        TextMeshProUGUI hint = incubation.Find("IncubationHint")
            ?.GetComponent<TextMeshProUGUI>();
        if (hint != null)
            hint.text = "MADURAS A RESERVA AUTOMÁTICA";
        SetTextSize(incubation.Find("IncubationHint"), 15f, 13f);
    }

    private static void AlignAnchorChamber(Transform anchor)
    {
        CenterText(anchor.Find("Title"), new Vector2(0.12f, 0.84f),
            new Vector2(0.88f, 0.95f));
        SetRectSafe(anchor.Find("SectionIcon") as RectTransform,
            new Vector2(0.060f, 0.85f), new Vector2(0.092f, 0.94f));
        SetRectSafe(anchor.Find("LatticeFrame") as RectTransform,
            new Vector2(0.065f, 0.18f), new Vector2(0.53f, 0.80f));
        Transform latticeFrame = anchor.Find("LatticeFrame");
        SetRectSafe(latticeFrame?.Find("CoreGlow") as RectTransform,
            new Vector2(0.16f, 0.18f), new Vector2(0.84f, 0.96f));
        SetRectSafe(latticeFrame?.Find("AnchorCore") as RectTransform,
            new Vector2(0.27f, 0.22f), new Vector2(0.73f, 0.94f));
        SetRectSafe(latticeFrame?.Find("ChronalLattice") as RectTransform,
            new Vector2(0.08f, 0.18f), new Vector2(0.92f, 0.94f));
        CenterText(latticeFrame?.Find("AnchorState"), new Vector2(0.10f, 0.03f),
            new Vector2(0.90f, 0.14f));
        SetRectSafe(anchor.Find("Stability") as RectTransform,
            new Vector2(0.52f, 0.45f), new Vector2(0.68f, 0.79f));
        SetRectSafe(anchor.Find("Tension") as RectTransform,
            new Vector2(0.70f, 0.45f), new Vector2(0.86f, 0.79f));
        CenterText(anchor.Find("Threshold"), new Vector2(0.50f, 0.33f),
            new Vector2(0.88f, 0.42f));
        CenterText(anchor.Find("Forecast"), new Vector2(0.50f, 0.21f),
            new Vector2(0.88f, 0.30f));
        CenterText(anchor.Find("QualityLegend"), new Vector2(0.50f, 0.14f),
            new Vector2(0.88f, 0.21f));
    }

    private static void AlignStabilization(Transform stabilization)
    {
        if (stabilization == null)
            return;
        CenterText(stabilization.Find("Title"), new Vector2(0.12f, 0.70f),
            new Vector2(0.82f, 0.88f));
        SetRectSafe(stabilization.Find("EnergyIcon") as RectTransform,
            new Vector2(0.065f, 0.69f), new Vector2(0.095f, 0.88f));
        SetRectSafe(stabilization.Find("IntensitySlider") as RectTransform,
            new Vector2(0.08f, 0.39f), new Vector2(0.81f, 0.61f));
        CenterText(stabilization.Find("IntensityValue"), new Vector2(0.84f, 0.49f),
            new Vector2(0.92f, 0.86f));
        AlignText(stabilization.Find("Effect"), new Vector2(0.08f, 0.16f),
            new Vector2(0.52f, 0.34f), TextAlignmentOptions.Left);
        AlignText(stabilization.Find("CompensateEffect"),
            new Vector2(0.50f, 0.16f), new Vector2(0.92f, 0.34f),
            TextAlignmentOptions.Right);
    }

    private static void AlignActions(Transform actions)
    {
        string[] names = { "Stabilize", "Compensate", "Materialize", "Discard" };
        float[] mins = { 0.12f, 0.32f, 0.52f, 0.72f };
        float[] maxs = { 0.28f, 0.48f, 0.68f, 0.88f };
        for (int i = 0; i < names.Length; i++)
            SetRectSafe(actions.Find(names[i]) as RectTransform,
                new Vector2(mins[i], 0.22f), new Vector2(maxs[i], 0.78f));
    }

    private static void AlignArchive(Transform archive)
    {
        CenterText(archive.Find("Title"), new Vector2(0.12f, 0.70f),
            new Vector2(0.88f, 0.88f));
        SetRectSafe(archive.Find("Pure") as RectTransform,
            new Vector2(0.13f, 0.25f), new Vector2(0.31f, 0.61f));
        SetRectSafe(archive.Find("Stable") as RectTransform,
            new Vector2(0.41f, 0.25f), new Vector2(0.59f, 0.61f));
        SetRectSafe(archive.Find("Forced") as RectTransform,
            new Vector2(0.69f, 0.25f), new Vector2(0.87f, 0.61f));
        SetRectSafe(archive.Find("ArchiveTrack") as RectTransform,
            new Vector2(0.13f, 0.13f), new Vector2(0.87f, 0.16f));
    }

    private static void RebuildRowSeparator(Transform slot, bool visible)
    {
        if (slot == null)
            return;
        Transform old = slot.Find("ApprovedRowSeparator");
        if (old != null)
            UnityEngine.Object.DestroyImmediate(old.gameObject);
        if (!visible)
            return;
        GameObject separatorObject = CreateRect("ApprovedRowSeparator", slot,
            new Vector2(0.02f, 0.005f), new Vector2(0.98f, 0.025f));
        Image separator = separatorObject.AddComponent<Image>();
        separator.color = new Color(0.42f, 0.48f, 0.50f, 0.35f);
        separator.raycastTarget = false;
        separatorObject.transform.SetAsFirstSibling();
    }

    private static void HideLatticeRails(Transform latticeFrame)
    {
        if (latticeFrame == null)
            return;
        foreach (string name in new[] { "RailTop", "RailBottom", "RailLeft", "RailRight" })
            SetActive(latticeFrame.Find(name), false);
    }

    private static void ClearPanelImage(Transform target)
    {
        Image image = target != null ? target.GetComponent<Image>() : null;
        if (image == null)
            return;
        image.sprite = null;
        image.color = Color.clear;
        image.raycastTarget = false;
    }

    private static void CenterText(Transform target, Vector2 min, Vector2 max)
    {
        AlignText(target, min, max, TextAlignmentOptions.Center);
    }

    private static void AlignText(Transform target, Vector2 min, Vector2 max,
        TextAlignmentOptions alignment)
    {
        TextMeshProUGUI text = target?.GetComponent<TextMeshProUGUI>();
        if (text == null)
            return;
        SetRectSafe(text.rectTransform, min, max);
        text.alignment = alignment;
        text.margin = Vector4.zero;
    }

    private static void SetTextSize(Transform target, float maxSize,
        float minSize)
    {
        TextMeshProUGUI text = target?.GetComponent<TextMeshProUGUI>();
        if (text == null)
            return;
        text.enableAutoSizing = true;
        text.fontSize = maxSize;
        text.fontSizeMax = maxSize;
        text.fontSizeMin = minSize;
    }

    private static void SetActive(Transform target, bool active)
    {
        if (target != null)
            target.gameObject.SetActive(active);
    }

    private static void SetRectSafe(RectTransform rect, Vector2 anchorMin,
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
        colors.highlightedColor = Color.white;
        colors.pressedColor = new Color(0.72f, 0.76f, 0.80f, 1f);
        colors.selectedColor = Color.white;
        colors.disabledColor = new Color(0.48f, 0.50f, 0.52f, 0.72f);
        button.colors = colors;
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
        TextMeshProUGUI title = shell.Find("SeedsTitle")?.GetComponent<TextMeshProUGUI>();
        if (title != null)
        {
            title.fontSize = 36f;
            title.fontSizeMin = 29f;
            title.characterSpacing = 3f;
            title.color = new Color(0.91f, 0.92f, 0.93f, 1f);
        }
        TextMeshProUGUI subtitle = shell.Find("SeedsSubtitle")?.GetComponent<TextMeshProUGUI>();
        if (subtitle != null)
            subtitle.color = Cyan;

        foreach (TextMeshProUGUI text in shell.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (text == null || text == title || text == subtitle)
                continue;
            text.enableAutoSizing = true;
            text.fontSizeMin = Mathf.Min(text.fontSizeMin, 12f);
            text.overflowMode = TextOverflowModes.Truncate;
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
        SetRect(rect, anchorMin, anchorMax);
        return result;
    }

    private static void SetRect(RectTransform rect, Vector2 anchorMin,
        Vector2 anchorMax)
    {
        Require(rect != null, "RectTransform no disponible.");
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
#endif

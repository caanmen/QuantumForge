#if UNITY_EDITOR
using System;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

/// <summary>
/// Applies the approved Generation/Upgrades visual shell to the existing machine
/// cube without rebuilding its operational UI or its physical 3D prototype.
/// </summary>
public static class MachineCubeApprovedStyleSetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string LabBackgroundPath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_lab_accident_background_v3.png";
    private const string SelectorFramePath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_selector_frame.png";
    private const string ModuleFramePath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_module_card_metal_v2.png";
    private const string ResourceFramePath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_resource_counter_metal_v2.png";
    private const string NodesIconPath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_circuit_energy.png";
    private const string MixesIconPath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_circuit_experimental.png";
    private const string SeedsIconPath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_circuit_phase.png";

    private static readonly Color WhiteMetal = new Color(0.83f, 0.86f, 0.87f, 1f);
    private static readonly Color Cyan = new Color(0f, 0.78f, 0.94f, 1f);
    private static readonly Color Purple = new Color(0.72f, 0.29f, 0.89f, 1f);
    private static readonly Color Teal = new Color(0f, 0.80f, 0.73f, 1f);

    [MenuItem("Tools/Quantum Forge/Machine/Apply Approved Cube Style")]
    public static void Configure()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        ApplyToOpenScene(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[Machine Cube Approved Style] CONFIGURED | triangle shell | " +
            "three context tabs | centered true 3D framing");
    }

    public static void ApplyToOpenScene(Scene scene)
    {
        MachinePanelUI panel = UnityEngine.Object.FindFirstObjectByType<MachinePanelUI>(
            FindObjectsInactive.Include);
        Require(panel != null, "No se encontró MachinePanelUI.");
        Transform root = panel.transform.Find("MachineCubeVisualRoot");
        Require(root != null, "No se encontró MachineCubeVisualRoot.");

        Sprite lab = LoadSprite(LabBackgroundPath);
        Sprite selector = LoadSprite(SelectorFramePath);
        Sprite module = LoadSprite(ModuleFramePath);
        Sprite resource = LoadSprite(ResourceFramePath);
        Sprite nodesIcon = LoadSprite(NodesIconPath);
        Sprite mixesIcon = LoadSprite(MixesIconPath);
        Sprite seedsIcon = LoadSprite(SeedsIconPath);

        StyleRoot(root, lab);
        StyleHeader(root, resource);
        StyleTabs(panel, root, selector, nodesIcon, mixesIcon, seedsIcon);
        StyleChamber(root, lab, module, resource, selector);
        CenterPhysicalCube();

        EditorUtility.SetDirty(root.gameObject);
        EditorUtility.SetDirty(panel);
        EditorSceneManager.MarkSceneDirty(scene);
    }

    public static void ConfigureBatch()
    {
        try
        {
            Configure();
            Validate();
            Debug.Log("[Machine Cube Approved Style] PASS");
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
        MachinePanelUI panel = UnityEngine.Object.FindFirstObjectByType<MachinePanelUI>(
            FindObjectsInactive.Include);
        Require(panel != null, "MachinePanelUI no disponible.");
        Transform root = panel.transform.Find("MachineCubeVisualRoot");
        Require(root != null, "Falta MachineCubeVisualRoot.");
        Require(root.Find("ApprovedLabChamber") != null,
            "Falta el fondo reutilizado del laboratorio.");
        Transform tabs = root.Find("MachineContextTabs");
        Require(tabs != null && tabs.Find("NodesTab") != null &&
            tabs.Find("MixesTab") != null && tabs.Find("SeedsTab") != null,
            "Las tres pestañas de contexto no están presentes.");
        Transform display = root.Find("FaceViewport/MachineCube3DPrototypeDisplay");
        Require(display != null && display.GetComponent<RawImage>() != null,
            "La pantalla 3D del cubo perdió su RenderTexture.");
        Camera camera = UnityEngine.Object.FindFirstObjectByType<
            MachineCube3DPrototypeController>(FindObjectsInactive.Include)
            ?.PrototypeCamera;
        Require(camera != null && !camera.orthographic,
            "La cámara física del cubo no está disponible.");
        Require(camera.backgroundColor.a <= 0.01f,
            "El fondo de la cámara 3D debe ser transparente para mostrar el laboratorio.");
    }

    private static void StyleRoot(Transform root, Sprite lab)
    {
        Image rootImage = root.GetComponent<Image>();
        Require(rootImage != null, "MachineCubeVisualRoot no tiene Image.");
        rootImage.sprite = lab;
        rootImage.type = Image.Type.Simple;
        rootImage.preserveAspect = false;
        rootImage.color = new Color(0.50f, 0.54f, 0.56f, 0.72f);
        rootImage.raycastTarget = true;

        Transform grid = root.Find("CircuitGrid");
        if (grid != null)
            grid.gameObject.SetActive(false);
    }

    private static void StyleHeader(Transform root, Sprite resource)
    {
        Transform header = RequireChild(root, "MachineHeader");
        StylePanel(RequireChild(header, "MachineResource_LE"), resource,
            new Color(0.82f, 0.86f, 0.87f, 0.96f));
        StylePanel(RequireChild(header, "MachineResource_Traces"), resource,
            new Color(0.82f, 0.86f, 0.87f, 0.96f));
        Transform titlePlate = RequireChild(header, "MachineTitlePlate");
        StylePanel(titlePlate, resource, new Color(0.72f, 0.76f, 0.77f, 0.98f));

        SetRect(RequireChild(header, "MachineResource_LE") as RectTransform,
            new Vector2(0.065f, 0.946f), new Vector2(0.493f, 0.995f));
        SetRect(RequireChild(header, "MachineResource_Traces") as RectTransform,
            new Vector2(0.507f, 0.946f), new Vector2(0.935f, 0.995f));
        SetRect(titlePlate as RectTransform, new Vector2(0.065f, 0.884f),
            new Vector2(0.935f, 0.942f));

        TextMeshProUGUI title = titlePlate.Find("Title")?.GetComponent<TextMeshProUGUI>();
        if (title != null)
        {
            // Optical correction for the Rajdhani glyphs: the accented word looks
            // left-heavy even in a geometrically centered rectangle.
            SetRect(title.rectTransform, new Vector2(0.25f, 0.12f),
                new Vector2(0.75f, 0.84f));
            title.text = "MÁQUINA";
            title.fontSize = 48f;
            title.fontSizeMin = 38f;
            title.characterSpacing = 5f;
            title.color = new Color(0.90f, 0.92f, 0.93f, 1f);
            title.alignment = TextAlignmentOptions.Center;
        }

        // The approved mock-up leaves the title plate clean. Repair progress still
        // remains available in the selected-node card and sector indicators.
        SetActive(titlePlate.Find("TotalProgress"), false);
        SetActive(titlePlate.Find("Convergence"), false);
        SetActive(titlePlate.Find("ProgressTrack"), false);
    }

    private static void StyleTabs(MachinePanelUI panel, Transform root,
        Sprite selector, Sprite nodesIcon, Sprite mixesIcon, Sprite seedsIcon)
    {
        RectTransform tabs = RequireChild(root, "MachineContextTabs") as RectTransform;
        Require(tabs != null, "MachineContextTabs no tiene RectTransform.");
        SetRect(tabs, new Vector2(0.070f, 0.840f), new Vector2(0.930f, 0.880f));

        Transform nodes = RequireChild(tabs, "NodesTab");
        Transform mixes = RequireChild(tabs, "MixesTab");
        Transform seeds = tabs.Find("SeedsTab");
        if (seeds == null)
        {
            GameObject clone = UnityEngine.Object.Instantiate(mixes.gameObject,
                tabs, false);
            clone.name = "SeedsTab";
            seeds = clone.transform;
            seeds.gameObject.SetActive(false);
        }

        Button seedsButton = seeds.GetComponent<Button>();
        Require(seedsButton != null, "SeedsTab no tiene Button.");
        SerializedObject panelSo = new SerializedObject(panel);
        SerializedProperty seedsProperty = panelSo.FindProperty("btnSeedsTab");
        Require(seedsProperty != null, "Falta la referencia btnSeedsTab.");
        seedsProperty.objectReferenceValue = seedsButton;
        panelSo.ApplyModifiedPropertiesWithoutUndo();

        StyleTab(nodes, selector, nodesIcon, Cyan,
            new Vector2(0f, 0.04f), new Vector2(0.32f, 0.96f));
        StyleTab(mixes, selector, mixesIcon, Purple,
            new Vector2(0.34f, 0.04f), new Vector2(0.66f, 0.96f));
        StyleTab(seeds, selector, seedsIcon, Teal,
            new Vector2(0.68f, 0.04f), new Vector2(1f, 0.96f));
    }

    private static void StyleTab(Transform tab, Sprite selector, Sprite iconSprite,
        Color accent, Vector2 anchorMin, Vector2 anchorMax)
    {
        SetRect(tab as RectTransform, anchorMin, anchorMax);
        Image image = tab.GetComponent<Image>();
        Require(image != null, tab.name + " no tiene Image.");
        image.sprite = selector;
        image.type = Image.Type.Sliced;
        image.color = new Color(accent.r, accent.g, accent.b, 0.88f);

        Transform previous = tab.Find("ApprovedReuseIcon");
        if (previous != null)
            UnityEngine.Object.DestroyImmediate(previous.gameObject);
        GameObject iconObject = CreateRect("ApprovedReuseIcon", tab,
            new Vector2(0.055f, 0.20f), new Vector2(0.235f, 0.80f));
        Image icon = iconObject.AddComponent<Image>();
        icon.sprite = iconSprite;
        icon.preserveAspect = true;
        icon.color = Color.white;
        icon.raycastTarget = false;

        TextMeshProUGUI label = tab.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
        {
            RectTransform labelRect = label.rectTransform;
            labelRect.anchorMin = new Vector2(0.24f, 0.08f);
            labelRect.anchorMax = new Vector2(0.95f, 0.92f);
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            label.fontSize = 21f;
            label.fontSizeMin = 17f;
            label.characterSpacing = 1.2f;
            label.color = WhiteMetal;
            label.alignment = TextAlignmentOptions.Center;
        }
    }

    private static void StyleChamber(Transform root, Sprite lab, Sprite module,
        Sprite resource, Sprite selector)
    {
        Transform old = root.Find("ApprovedLabChamber");
        if (old != null)
            UnityEngine.Object.DestroyImmediate(old.gameObject);
        GameObject chamberObject = CreateRect("ApprovedLabChamber", root,
            new Vector2(0.015f, 0.190f), new Vector2(0.985f, 0.838f));
        Image chamber = chamberObject.AddComponent<Image>();
        chamber.sprite = lab;
        chamber.type = Image.Type.Simple;
        chamber.preserveAspect = false;
        // Match the restrained Triangle treatment: readable environmental detail,
        // but still an unlit laboratory after the accident.
        chamber.color = new Color(0.76f, 0.78f, 0.79f, 1f);
        chamber.raycastTarget = false;
        chamberObject.transform.SetAsFirstSibling();

        GameObject shadeObject = CreateRect("ChamberShade", chamberObject.transform,
            new Vector2(0.055f, 0.055f), new Vector2(0.945f, 0.945f));
        Image shade = shadeObject.AddComponent<Image>();
        shade.color = new Color(0f, 0.018f, 0.028f, 0.18f);
        shade.raycastTarget = false;

        RectTransform viewport = RequireChild(root, "FaceViewport") as RectTransform;
        Require(viewport != null, "FaceViewport no tiene RectTransform.");
        SetRect(viewport, new Vector2(0.045f, 0.245f),
            new Vector2(0.955f, 0.785f));

        Transform display = viewport.Find("MachineCube3DPrototypeDisplay");
        if (display != null)
        {
            SetRect(display as RectTransform, Vector2.zero, Vector2.one);
            RawImage raw = display.GetComponent<RawImage>();
            if (raw != null)
                raw.color = Color.white;
        }

        Transform card = RequireChild(root, "SelectedNodeCard");
        // The node card is horizontal. The former module-card texture contains
        // internal vertical divisions, which made its right frame cross the action
        // buttons. The resource plate has the correct uninterrupted horizontal shell.
        StylePanel(card, resource, new Color(0.82f, 0.85f, 0.86f, 0.98f));
        SetHorizontalRange(card as RectTransform, 0.025f, 0.975f);
        Transform iconPlate = card.Find("SelectedNodeIcon");
        if (iconPlate != null)
        {
            StylePanel(iconPlate, module, new Color(0.62f, 0.67f, 0.69f, 0.98f));
            // The horizontal shell has a wide ornamental bevel. Keep the icon plate
            // completely inside its flat content area instead of covering that bevel.
            SetRect(iconPlate as RectTransform, new Vector2(0.055f, 0.16f),
                new Vector2(0.185f, 0.88f));
        }

        // The extra vertical accent competes with the left edge of the metal shell.
        SetActive(card.Find("AccentRail"), false);

        // Pull the first and last text rows away from the ornamental top/bottom
        // bevels. The middle rows already sit inside the usable center surface.
        SetRect(card.Find("NodeName") as RectTransform,
            new Vector2(0.205f, 0.76f), new Vector2(0.73f, 0.90f));
        SetRect(card.Find("NodeState") as RectTransform,
            new Vector2(0.205f, 0.66f), new Vector2(0.73f, 0.77f));
        SetRect(card.Find("NodeDescription") as RectTransform,
            new Vector2(0.205f, 0.51f), new Vector2(0.73f, 0.66f));
        SetRect(card.Find("NodeEffect") as RectTransform,
            new Vector2(0.205f, 0.40f), new Vector2(0.73f, 0.50f));
        SetRect(card.Find("NodeRequirements") as RectTransform,
            new Vector2(0.205f, 0.30f), new Vector2(0.73f, 0.40f));
        SetRect(card.Find("NodeCost") as RectTransform,
            new Vector2(0.205f, 0.20f), new Vector2(0.70f, 0.30f));

        // Keep all context actions inside the visual border instead of sitting on it.
        foreach (string buttonName in new[]
        {
            "RepairNode", "AnalyzeNode", "OpenFusion", "OpenAnchors"
        })
        {
            RectTransform button = card.Find(buttonName) as RectTransform;
            if (button == null)
                continue;

            bool primaryAction = buttonName == "RepairNode" ||
                buttonName == "AnalyzeNode";
            float minY = primaryAction
                ? 0.50f
                : 0.19f;
            float maxY = primaryAction
                ? 0.82f
                : 0.47f;
            SetRect(button, new Vector2(0.745f, minY),
                new Vector2(0.895f, maxY));
        }
        SetRect(card.Find("FaceProgress") as RectTransform,
            new Vector2(0.705f, 0.25f), new Vector2(0.915f, 0.35f));
        SetActive(card.Find("OpenAnchors"), false);

        foreach (string arrowName in new[] { "PreviousFaceButton", "NextFaceButton" })
        {
            Transform arrow = root.Find(arrowName);
            Image arrowImage = arrow != null ? arrow.GetComponent<Image>() : null;
            if (arrowImage == null)
                continue;
            arrowImage.sprite = selector;
            arrowImage.type = Image.Type.Sliced;
            arrowImage.color = new Color(0.48f, 0.55f, 0.58f, 0.88f);
        }
    }

    private static void CenterPhysicalCube()
    {
        MachineCube3DPrototypeController controller =
            UnityEngine.Object.FindFirstObjectByType<MachineCube3DPrototypeController>(
                FindObjectsInactive.Include);
        Require(controller != null && controller.PrototypeCamera != null,
            "No se encontró la cámara del cubo físico.");
        Camera camera = controller.PrototypeCamera;
        camera.transform.localPosition = new Vector3(0f, 0.35f, 20f);
        Vector3 target = new Vector3(0.72f, 0f, 0f);
        camera.transform.localRotation = Quaternion.LookRotation(
            (target - camera.transform.localPosition).normalized, Vector3.up);
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0f, 0f, 0f, 0f);
        UniversalAdditionalCameraData cameraData =
            camera.GetComponent<UniversalAdditionalCameraData>();
        if (cameraData != null)
        {
            // URP post-processing writes an opaque alpha into the RenderTexture.
            // The cube already uses emissive materials, so disabling it here keeps
            // the look while allowing the lab UI behind the RawImage to show through.
            cameraData.renderPostProcessing = false;
            EditorUtility.SetDirty(cameraData);
        }
        EditorUtility.SetDirty(camera);
    }

    private static void SetHorizontalRange(RectTransform rect, float minX,
        float maxX)
    {
        if (rect == null)
            return;
        rect.anchorMin = new Vector2(minX, rect.anchorMin.y);
        rect.anchorMax = new Vector2(maxX, rect.anchorMax.y);
        rect.offsetMin = new Vector2(0f, rect.offsetMin.y);
        rect.offsetMax = new Vector2(0f, rect.offsetMax.y);
    }

    private static void StylePanel(Transform target, Sprite sprite, Color color)
    {
        Image image = target.GetComponent<Image>();
        Require(image != null, target.name + " no tiene Image.");
        image.sprite = sprite;
        image.type = Image.Type.Sliced;
        image.preserveAspect = false;
        image.color = color;
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

    private static void SetActive(Transform target, bool active)
    {
        if (target != null)
            target.gameObject.SetActive(active);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
#endif

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Dimension1ExpeditionResultSetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string RootName = "D1_ExpeditionResultVisualRoot";
    private const string ArtPath = "Assets/Project/UI/Dimension1/Generated";
    private const string CandidatePath = ArtPath + "/Candidates/ExpeditionResult";
    private const string MetalPath = ArtPath + "/MetalsInventory";
    private const string RelicPath = ArtPath + "/Candidates/Relics";
    private const string RelicExpansionPath = ArtPath + "/Candidates/RelicsExpansion";
    private const string FontPath = "Assets/Project/UI/Vertical/Fonts/Rajdhani-Medium SDF.asset";

    private static readonly Color Void = Hex("01090E", 252);
    private static readonly Color Fill = Hex("031019", 252);
    private static readonly Color Raised = Hex("051721", 252);
    private static readonly Color Cyan = Hex("18C8FF");
    private static readonly Color CyanMuted = Hex("087FA9", 230);
    private static readonly Color Primary = Hex("EAF1F4");
    private static readonly Color Secondary = Hex("99A6B0");
    private static readonly Color Amber = Hex("F4A70B");
    private static readonly Color AmberFill = Hex("1A1407", 254);

    private sealed class Refs
    {
        public TMP_Text destination;
        public Image shipIcon;
        public TMP_Text ship;
        public TMP_Text duration;
        public Dimension1ExpeditionResultUI.RewardSlot[] metals;
        public TMP_Text metalsEmpty;
        public Dimension1ExpeditionResultUI.RewardSlot fragments;
        public Dimension1ExpeditionResultUI.RewardSlot matrix;
        public TMP_Text matricesEmpty;
        public GameObject relicPanel;
        public Image relicIcon;
        public TMP_Text relicHeading;
        public TMP_Text relicName;
        public TMP_Text relicTier;
        public TMP_Text relicOrigin;
        public TMP_Text relicDescription;
        public GameObject specialBanner;
        public TMP_Text specialText;
        public Button continueButton;
        public Button relicsButton;
    }

    [MenuItem("Quantum Forge/Dimension 1/Install Expedition Result Screen")]
    public static void Install()
    {
        foreach (string path in CandidatePaths()) PrepareSprite(path, 2048, false);
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Dimension1PanelUI panel = FindSceneComponent<Dimension1PanelUI>(scene);
        if (panel == null) throw new InvalidOperationException("Falta Dimension1PanelUI.");

        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        Sprite frame = LoadRequired(ArtPath + "/d1_premium_frame_v4.png");
        Sprite fill = LoadRequired(ArtPath + "/d1_panel_fill_v4.png");
        Sprite stars = LoadRequired(ArtPath + "/d1_starfield.png");
        Sprite badge = LoadRequired(CandidatePath + "/d1_expedition_complete_badge_v1.png");
        Sprite fragments = LoadRequired(CandidatePath + "/d1_matrix_fragments_v1.png");
        Sprite matrix = LoadRequired(CandidatePath + "/d1_matrix_specific_v1.png");
        Sprite[] metalSprites = LoadMetalSprites();
        Sprite[] shipSprites = LoadShipSprites();
        Sprite[] relicSprites = LoadRelicSprites();
        if (font == null || HasNull(metalSprites) || HasNull(shipSprites) || HasNull(relicSprites))
            throw new InvalidOperationException("Faltan assets de la pantalla de resultado.");

        Transform previous = FindSceneTransform(scene, RootName);
        if (previous != null) UnityEngine.Object.DestroyImmediate(previous.gameObject);

        RectTransform root = Rect(RootName, panel.transform);
        root.anchorMin = root.anchorMax = new Vector2(.5f, .5f);
        root.pivot = new Vector2(.5f, .5f);
        root.anchoredPosition = Dimension1SharedLayoutTokens.RootOffset;
        root.sizeDelta = new Vector2(1080f, 1920f);
        root.SetAsLastSibling();
        root.gameObject.AddComponent<Dimension1VisualSkinRoot>();
        Canvas canvas = root.gameObject.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 32760;
        root.gameObject.AddComponent<GraphicRaycaster>();
        CanvasGroup group = root.gameObject.AddComponent<CanvasGroup>();
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;
        Dimension1ExpeditionResultUI visual = root.gameObject.AddComponent<Dimension1ExpeditionResultUI>();

        Image backdrop = Image("Backdrop", root, null, Hex("00070B", 248));
        Stretch(backdrop.rectTransform, new Vector2(-16f, -20f), new Vector2(-16f, -20f));
        backdrop.raycastTarget = true;
        Image starfield = Image("Starfield", root, stars, Hex("4EC8E8", 14));
        Stretch(starfield.rectTransform);
        RectTransform modal = Panel("ResultFrame", root, frame, fill,
            new Vector2(28f, 24f), new Vector2(1024f, 1872f), Void, CyanMuted, out _, out _);

        Refs refs = new Refs();
        BuildHeader(modal, font, badge, refs);
        BuildMetals(modal, frame, fill, font, metalSprites, refs);
        BuildMatrices(modal, frame, fill, font, fragments, matrix, refs);
        BuildRelic(modal, frame, fill, font, refs);
        BuildActions(modal, frame, fill, font, refs);

        visual.Configure(panel, group, refs.destination, refs.shipIcon, refs.ship, refs.duration,
            refs.metals, refs.metalsEmpty, refs.fragments, refs.matrix, refs.matricesEmpty,
            refs.relicPanel, refs.relicIcon, refs.relicHeading, refs.relicName, refs.relicTier,
            refs.relicOrigin, refs.relicDescription, refs.specialBanner, refs.specialText,
            refs.continueButton, refs.relicsButton, metalSprites, shipSprites, fragments, matrix,
            relicSprites);
        UnityEventTools.AddPersistentListener(refs.continueButton.onClick, visual.Continue);
        UnityEventTools.AddPersistentListener(refs.relicsButton.onClick, visual.ViewRelics);

        SerializedObject serialized = new SerializedObject(panel);
        SerializedProperty property = serialized.FindProperty("expeditionResultUI");
        if (property == null) throw new InvalidOperationException("Falta expeditionResultUI en Dimension1PanelUI.");
        property.objectReferenceValue = visual;
        serialized.ApplyModifiedPropertiesWithoutUndo();

        EditorUtility.SetDirty(visual);
        EditorUtility.SetDirty(panel);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("Unity no pudo guardar Main.unity.");
        ValidateInternal(scene);
        Debug.Log("[D1 Expedition Result] INSTALL_PASS | datos reales | metales + matrices + reliquia + punto especial");
    }

    [MenuItem("Quantum Forge/Dimension 1/Validate Expedition Result Screen")]
    public static void Validate()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        ValidateInternal(scene);
        Debug.Log("[D1 Expedition Result] VALIDATION_PASS");
    }

    private static void BuildHeader(Transform modal, TMP_FontAsset font, Sprite badge, Refs refs)
    {
        TMP_Text title = Text("Title", modal, font, "EXPEDICIÓN COMPLETADA", 48f,
            FontStyles.Bold, Primary, TextAlignmentOptions.Center);
        Top(title.rectTransform, 92f, 40f, 840f, 70f);
        Line(modal, "TitleLineL", 76f, 78f, 125f, CyanMuted);
        Line(modal, "TitleLineR", 823f, 78f, 125f, CyanMuted);

        Image emblem = Image("CompletionBadge", modal, badge, Color.white);
        Top(emblem.rectTransform, 357f, 116f, 310f, 310f);
        emblem.preserveAspect = true;

        TMP_Text destinationLabel = Text("DestinationLabel", modal, font, "DESTINO", 25f,
            FontStyles.Normal, Secondary, TextAlignmentOptions.Center);
        Top(destinationLabel.rectTransform, 290f, 404f, 444f, 36f);
        refs.destination = Text("Destination", modal, font, "LABORATORIO", 42f,
            FontStyles.Normal, Hex("4FBDEB"), TextAlignmentOptions.Center);
        Top(refs.destination.rectTransform, 74f, 438f, 876f, 60f);
        Line(modal, "HeaderDivider", 96f, 507f, 832f, Hex("087FA9", 120));

        refs.shipIcon = Image("ShipIcon", modal, null, Cyan);
        Top(refs.shipIcon.rectTransform, 230f, 530f, 56f, 56f);
        refs.shipIcon.preserveAspect = true;
        refs.shipIcon.material = AssetDatabase.LoadAssetAtPath<Material>(
            ArtPath + "/d1_hangar_blueprint_keyed.mat");
        if (refs.shipIcon.material == null)
            throw new InvalidOperationException("Falta el material canónico de blueprint del Hangar.");
        refs.ship = Text("Ship", modal, font, "SONDA ANALÍTICA", 31f,
            FontStyles.Normal, Hex("4FBDEB"), TextAlignmentOptions.Left);
        Top(refs.ship.rectTransform, 300f, 531f, 430f, 54f);
        refs.duration = Text("Duration", modal, font, "", 31f,
            FontStyles.Normal, Primary, TextAlignmentOptions.Right);
        Top(refs.duration.rectTransform, 730f, 531f, 190f, 54f);
    }

    private static void BuildMetals(Transform modal, Sprite frame, Sprite fill, TMP_FontAsset font,
        Sprite[] metalSprites, Refs refs)
    {
        RectTransform panel = Panel("MetalsPanel", modal, frame, fill,
            new Vector2(44f, 610f), new Vector2(936f, 250f), Fill, CyanMuted, out _, out _);
        SectionTitle(panel, font, "METALES");
        refs.metals = new Dimension1ExpeditionResultUI.RewardSlot[4];
        for (int i = 0; i < refs.metals.Length; i++)
            refs.metals[i] = RewardSlot("MetalReward_" + i, panel, frame, fill, font,
                new Vector2(92f + i * 228f, 66f), new Vector2(216f, 150f),
                i < metalSprites.Length ? metalSprites[i] : null);
        refs.metalsEmpty = Text("MetalsEmpty", panel, font, "SIN METALES OBTENIDOS", 28f,
            FontStyles.Normal, Secondary, TextAlignmentOptions.Center);
        Top(refs.metalsEmpty.rectTransform, 120f, 108f, 696f, 48f);
        refs.metalsEmpty.gameObject.SetActive(false);
    }

    private static void BuildMatrices(Transform modal, Sprite frame, Sprite fill, TMP_FontAsset font,
        Sprite fragments, Sprite matrix, Refs refs)
    {
        RectTransform panel = Panel("MatricesPanel", modal, frame, fill,
            new Vector2(44f, 876f), new Vector2(936f, 236f), Fill, CyanMuted, out _, out _);
        SectionTitle(panel, font, "MATRICES");
        refs.fragments = RewardSlot("FragmentReward", panel, frame, fill, font,
            new Vector2(92f, 64f), new Vector2(360f, 142f), fragments);
        refs.matrix = RewardSlot("SpecificMatrixReward", panel, frame, fill, font,
            new Vector2(548f, 64f), new Vector2(360f, 142f), matrix);
        refs.matricesEmpty = Text("MatricesEmpty", panel, font, "SIN MATRICES OBTENIDAS", 28f,
            FontStyles.Normal, Secondary, TextAlignmentOptions.Center);
        Top(refs.matricesEmpty.rectTransform, 120f, 105f, 696f, 48f);
        refs.matricesEmpty.gameObject.SetActive(false);
    }

    private static void BuildRelic(Transform modal, Sprite frame, Sprite fill, TMP_FontAsset font, Refs refs)
    {
        RectTransform panel = Panel("RelicPanel", modal, frame, fill,
            new Vector2(44f, 1128f), new Vector2(936f, 352f), Fill, CyanMuted, out _, out _);
        refs.relicPanel = panel.gameObject;
        RectTransform artFrame = Panel("RelicArtFrame", panel, frame, fill,
            new Vector2(38f, 50f), new Vector2(238f, 250f), AmberFill, Amber, out _, out _);
        refs.relicIcon = Image("RelicIcon", artFrame, null, Color.white);
        Stretch(refs.relicIcon.rectTransform, new Vector2(18f, 18f), new Vector2(18f, 18f));
        refs.relicIcon.preserveAspect = true;

        refs.relicHeading = Text("RelicHeading", panel, font, "RELIQUIA DESCUBIERTA", 30f,
            FontStyles.Normal, Hex("4FBDEB"), TextAlignmentOptions.Left);
        Top(refs.relicHeading.rectTransform, 304f, 32f, 585f, 44f);
        refs.relicName = Text("RelicName", panel, font, "ANTENA FRACTURADA", 34f,
            FontStyles.Bold, Primary, TextAlignmentOptions.Left);
        Top(refs.relicName.rectTransform, 304f, 92f, 510f, 58f);
        refs.relicTier = Text("RelicTier", panel, font, "TIER 1", 22f,
            FontStyles.Bold, Amber, TextAlignmentOptions.Right);
        Top(refs.relicTier.rectTransform, 800f, 102f, 96f, 36f);
        refs.relicOrigin = Text("RelicOrigin", panel, font, "", 25f,
            FontStyles.Normal, Hex("4FBDEB"), TextAlignmentOptions.Left);
        Top(refs.relicOrigin.rectTransform, 304f, 166f, 580f, 44f);
        refs.relicDescription = Text("RelicDescription", panel, font, "", 25f,
            FontStyles.Normal, Secondary, TextAlignmentOptions.TopLeft);
        Top(refs.relicDescription.rectTransform, 304f, 222f, 580f, 88f);
        refs.relicDescription.textWrappingMode = TextWrappingModes.Normal;
        refs.relicDescription.overflowMode = TextOverflowModes.Truncate;
    }

    private static void BuildActions(Transform modal, Sprite frame, Sprite fill, TMP_FontAsset font, Refs refs)
    {
        refs.specialBanner = Panel("SpecialPointBanner", modal, frame, fill,
            new Vector2(58f, 1502f), new Vector2(908f, 70f), Hex("0D100D", 252),
            Hex("C78E0A", 210), out _, out _).gameObject;
        refs.specialText = Text("Label", refs.specialBanner.transform, font, "PUNTO ESPECIAL", 25f,
            FontStyles.Bold, Amber, TextAlignmentOptions.Center);
        Stretch(refs.specialText.rectTransform, new Vector2(40f, 6f), new Vector2(40f, 6f));

        refs.continueButton = ButtonPanel("CollectAndContinue", modal, frame, fill,
            new Vector2(116f, 1600f), new Vector2(792f, 122f), AmberFill, Amber);
        TMP_Text primary = Text("Label", refs.continueButton.transform, font,
            "RECOGER Y CONTINUAR", 39f, FontStyles.Bold, Hex("FFD060"), TextAlignmentOptions.Center);
        Stretch(primary.rectTransform, new Vector2(22f, 10f), new Vector2(22f, 10f));

        refs.relicsButton = ButtonPanel("ViewInRelics", modal, frame, fill,
            new Vector2(244f, 1750f), new Vector2(536f, 84f), Fill, CyanMuted);
        TMP_Text secondary = Text("Label", refs.relicsButton.transform, font,
            "VER EN RELIQUIAS", 30f, FontStyles.Normal, Hex("4FBDEB"), TextAlignmentOptions.Center);
        Stretch(secondary.rectTransform, new Vector2(18f, 8f), new Vector2(18f, 8f));
    }

    private static Dimension1ExpeditionResultUI.RewardSlot RewardSlot(string name, Transform parent,
        Sprite frame, Sprite fill, TMP_FontAsset font, Vector2 position, Vector2 size, Sprite preview)
    {
        RectTransform root = Rect(name, parent);
        Top(root, position.x, position.y, size.x, size.y);
        RectTransform badge = Panel("IconBadge", root, frame, fill, new Vector2(0f, 4f),
            new Vector2(126f, 126f), Raised, CyanMuted, out _, out _);
        Image icon = Image("Icon", badge, preview, Color.white);
        Stretch(icon.rectTransform, new Vector2(13f, 13f), new Vector2(13f, 13f));
        icon.preserveAspect = true;
        TMP_Text label = Text("Name", root, font, "RECOMPENSA", 24f,
            FontStyles.Normal, Secondary, TextAlignmentOptions.TopLeft);
        Top(label.rectTransform, 144f, 12f, Mathf.Max(120f, size.x - 146f), 64f);
        label.textWrappingMode = TextWrappingModes.Normal;
        label.overflowMode = TextOverflowModes.Truncate;
        TMP_Text amount = Text("Amount", root, font, "+0", 40f,
            FontStyles.Normal, Primary, TextAlignmentOptions.Left);
        Top(amount.rectTransform, 144f, 77f, Mathf.Max(120f, size.x - 146f), 56f);
        return new Dimension1ExpeditionResultUI.RewardSlot
        {
            root = root.gameObject, rect = root, icon = icon, nameText = label, amountText = amount
        };
    }

    private static void SectionTitle(Transform panel, TMP_FontAsset font, string value)
    {
        TMP_Text title = Text("SectionTitle", panel, font, value, 32f, FontStyles.Normal,
            Hex("4FBDEB"), TextAlignmentOptions.Center);
        Top(title.rectTransform, 250f, 12f, 436f, 46f);
        Line(panel, "SectionLineL", 35f, 37f, 210f, Hex("087FA9", 100));
        Line(panel, "SectionLineR", 691f, 37f, 210f, Hex("087FA9", 100));
    }

    private static Sprite[] LoadMetalSprites()
    {
        string[] names = { "iron", "copper", "aluminum", "titanium", "nickel",
            "cobalt", "lithium", "tungsten", "platinum", "iridium" };
        var result = new Sprite[names.Length];
        for (int i = 0; i < names.Length; i++)
            result[i] = LoadRequired(MetalPath + "/d1_metal_" + names[i] + "_v5.png");
        return result;
    }

    private static Sprite[] LoadShipSprites()
    {
        return new[]
        {
            LoadRequired(ArtPath + "/d1_hangar_sonda_ligera_blueprint_v2.png"),
            LoadRequired(ArtPath + "/d1_hangar_dron_extractor_blueprint_v2.png"),
            LoadRequired(ArtPath + "/d1_hangar_sonda_analitica_blueprint_v2.png"),
            LoadRequired(ArtPath + "/d1_hangar_nave_carga_blueprint_v2.png")
        };
    }

    private static Sprite[] LoadRelicSprites()
    {
        string[] paths =
        {
            RelicPath + "/d1_relic_drift_compass.png",
            RelicPath + "/d1_relic_ancient_drill.png",
            RelicPath + "/d1_relic_analytic_crystal.png",
            RelicPath + "/d1_relic_lost_navigation_record.png",
            RelicPath + "/d1_relic_modular_container.png",
            RelicPath + "/d1_relic_prospecting_core.png",
            RelicPath + "/d1_relic_fractured_antenna.png",
            RelicPath + "/d1_relic_extraction_seal.png",
            RelicExpansionPath + "/d1_relic_explorer_plate_candidate.png",
            RelicExpansionPath + "/d1_relic_incomplete_starmap_candidate.png",
            RelicExpansionPath + "/d1_relic_room1_echo_candidate.png",
            RelicExpansionPath + "/d1_relic_extraction_hook_candidate.png",
            RelicExpansionPath + "/d1_relic_matrix_archive_candidate.png",
            RelicExpansionPath + "/d1_relic_traces_resonator_candidate.png",
            RelicExpansionPath + "/d1_relic_remembered_alloy_candidate.png",
            RelicExpansionPath + "/d1_relic_ancient_cargo_core_candidate.png",
            RelicExpansionPath + "/d1_relic_calibration_fragment_candidate.png",
            RelicExpansionPath + "/d1_relic_rare_frequency_sensor_candidate.png",
            RelicExpansionPath + "/d1_relic_triangular_seal_candidate.png",
            RelicExpansionPath + "/d1_relic_machine_memory_candidate.png"
        };
        var result = new Sprite[paths.Length];
        for (int i = 0; i < paths.Length; i++) result[i] = LoadRequired(paths[i]);
        return result;
    }

    private static IEnumerable<string> CandidatePaths()
    {
        yield return CandidatePath + "/d1_expedition_complete_badge_v1.png";
        yield return CandidatePath + "/d1_matrix_fragments_v1.png";
        yield return CandidatePath + "/d1_matrix_specific_v1.png";
    }

    private static void ValidateInternal(Scene scene)
    {
        Transform root = FindSceneTransform(scene, RootName);
        if (root == null || root.GetComponent<Dimension1ExpeditionResultUI>() == null)
            throw new InvalidOperationException("Falta la pantalla Expedición Completada.");
        CanvasGroup group = root.GetComponent<CanvasGroup>();
        if (group == null || group.interactable || group.blocksRaycasts || group.alpha != 0f)
            throw new InvalidOperationException("El resultado debe iniciar cerrado y sin raycasts.");
        if (FindChild(root, "CompletionBadge") == null || FindChild(root, "MetalsPanel") == null ||
            FindChild(root, "MatricesPanel") == null || FindChild(root, "RelicPanel") == null ||
            FindChild(root, "CollectAndContinue")?.GetComponent<Button>() == null ||
            FindChild(root, "ViewInRelics")?.GetComponent<Button>() == null)
            throw new InvalidOperationException("La composición del resultado está incompleta.");
    }

    private static RectTransform Panel(string name, Transform parent, Sprite frame, Sprite fill,
        Vector2 topLeft, Vector2 size, Color fillColor, Color borderColor,
        out Image fillImage, out Image borderImage)
    {
        RectTransform rect = Rect(name, parent);
        Top(rect, topLeft.x, topLeft.y, size.x, size.y);
        fillImage = Image("Fill", rect, fill, fillColor);
        Stretch(fillImage.rectTransform, new Vector2(4f, 4f), new Vector2(4f, 4f));
        fillImage.type = UnityEngine.UI.Image.Type.Sliced;
        borderImage = Image("Border", rect, frame, borderColor);
        Stretch(borderImage.rectTransform);
        borderImage.type = UnityEngine.UI.Image.Type.Sliced;
        return rect;
    }

    private static Button ButtonPanel(string name, Transform parent, Sprite frame, Sprite fill,
        Vector2 position, Vector2 size, Color fillColor, Color borderColor)
    {
        RectTransform rect = Panel(name, parent, frame, fill, position, size, fillColor,
            borderColor, out _, out _);
        Image hit = rect.gameObject.AddComponent<Image>();
        hit.color = Color.clear;
        hit.raycastTarget = true;
        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = hit;
        return button;
    }

    private static RectTransform Rect(string name, Transform parent)
    {
        GameObject value = new GameObject(name, typeof(RectTransform));
        value.transform.SetParent(parent, false);
        return (RectTransform)value.transform;
    }

    private static Image Image(string name, Transform parent, Sprite sprite, Color color)
    {
        RectTransform rect = Rect(name, parent);
        Image image = rect.gameObject.AddComponent<Image>();
        image.sprite = sprite;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static TMP_Text Text(string name, Transform parent, TMP_FontAsset font, string value,
        float size, FontStyles style, Color color, TextAlignmentOptions alignment)
    {
        RectTransform rect = Rect(name, parent);
        TextMeshProUGUI text = rect.gameObject.AddComponent<TextMeshProUGUI>();
        text.font = font;
        text.text = value;
        text.fontSize = size;
        text.fontStyle = style;
        text.color = color;
        text.alignment = alignment;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.overflowMode = TextOverflowModes.Truncate;
        text.raycastTarget = false;
        return text;
    }

    private static void Top(RectTransform rect, float x, float y, float width, float height)
    {
        rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(x, -y);
        rect.sizeDelta = new Vector2(width, height);
    }

    private static void Stretch(RectTransform rect, Vector2 min = default, Vector2 max = default)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(.5f, .5f);
        rect.offsetMin = min;
        rect.offsetMax = -max;
    }

    private static void Line(Transform parent, string name, float x, float y, float width, Color color)
    {
        Image line = Image(name, parent, null, color);
        Top(line.rectTransform, x, y, width, 2f);
    }

    private static void PrepareSprite(string path, int maxSize, bool compressed)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) throw new InvalidOperationException("No se pudo importar " + path + ".");
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Bilinear;
        importer.textureCompression = compressed ? TextureImporterCompression.CompressedHQ : TextureImporterCompression.Uncompressed;
        importer.maxTextureSize = maxSize;
        importer.SaveAndReimport();
    }

    private static Sprite LoadRequired(string path)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite == null) throw new InvalidOperationException("Sprite no disponible: " + path);
        return sprite;
    }

    private static bool HasNull(Sprite[] sprites)
    {
        foreach (Sprite sprite in sprites) if (sprite == null) return true;
        return false;
    }

    private static T FindSceneComponent<T>(Scene scene) where T : Component
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            T value = root.GetComponentInChildren<T>(true);
            if (value != null) return value;
        }
        return null;
    }

    private static Transform FindSceneTransform(Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
                if (child.name == name) return child;
        return null;
    }

    private static Transform FindChild(Transform root, string name)
    {
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            if (child.name == name) return child;
        return null;
    }

    private static Color Hex(string html, byte alpha = 255)
    {
        ColorUtility.TryParseHtmlString("#" + html, out Color color);
        color.a = alpha / 255f;
        return color;
    }
}
#endif

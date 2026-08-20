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

public static class Dimension1MetalsInventorySetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ArtPath = "Assets/Project/UI/Dimension1/Generated";
    private const string MetalArtPath = ArtPath + "/MetalsInventory";
    private const string RootName = "D1_MetalsInventoryRoot";
    private const string FontPath = "Assets/Project/UI/Vertical/Fonts/Rajdhani-Medium SDF.asset";
    private const float W = 1080f;
    private const float H = 1920f;

    private static readonly Color Void = Hex("01090E");
    private static readonly Color Fill = Hex("04121B", 248);
    private static readonly Color FillRaised = Hex("071924", 252);
    private static readonly Color Cyan = Hex("18C8FF");
    private static readonly Color CyanMuted = Hex("087FA9");
    private static readonly Color Primary = Hex("EDF4F7");
    private static readonly Color Secondary = Hex("9EABB4");
    private static readonly Color Amber = Hex("F4A70B");
    private static readonly Color AmberFill = Hex("1A1508", 252);

    private sealed class Refs
    {
        public Button backButton;
        public readonly List<Button> filterButtons = new List<Button>();
        public readonly List<Image> filterBorders = new List<Image>();
        public readonly List<Image> filterFills = new List<Image>();
        public readonly List<TMP_Text> filterLabels = new List<TMP_Text>();
        public TMP_Text totalMetals;
        public TMP_Text unlockedMetals;
        public TMP_Text totalProduction;
        public TMP_Text highestMetal;
        public TMP_Text highestProduction;
        public readonly List<RectTransform> cards = new List<RectTransform>();
        public readonly List<Image> borders = new List<Image>();
        public readonly List<Image> icons = new List<Image>();
        public readonly List<TMP_Text> names = new List<TMP_Text>();
        public readonly List<TMP_Text> amounts = new List<TMP_Text>();
        public readonly List<TMP_Text> rates = new List<TMP_Text>();
        public readonly List<TMP_Text> sources = new List<TMP_Text>();
        public readonly List<TMP_Text> locked = new List<TMP_Text>();
        public readonly List<TMP_Text> requirements = new List<TMP_Text>();
    }

    [MenuItem("Quantum Forge/Dimension 1/Install Metals Inventory Subscreen")]
    public static void Install()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Dimension1PanelUI panel = FindSceneComponent<Dimension1PanelUI>(scene);
        if (panel == null) throw new InvalidOperationException("No existe Dimension1PanelUI en Main.unity.");

        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        Sprite frame = LoadSprite(ArtPath + "/d1_premium_frame_v4.png");
        Sprite fill = LoadSprite(ArtPath + "/d1_panel_fill_v4.png");
        Sprite starfield = LoadSprite(ArtPath + "/d1_starfield.png");
        Sprite lockSprite = LoadSprite(ArtPath + "/d1_lock_v3.png");
        Sprite[] metalSprites = LoadMetalSprites();
        if (font == null || frame == null || fill == null || metalSprites.Length != 10)
            throw new InvalidOperationException("Faltan recursos canónicos del Inventario de Metales.");

        Transform previous = FindDirectChild(panel.transform, RootName);
        if (previous != null) UnityEngine.Object.DestroyImmediate(previous.gameObject);

        RectTransform root = Rect(RootName, panel.transform);
        root.anchorMin = root.anchorMax = new Vector2(.5f, .5f);
        root.pivot = new Vector2(.5f, .5f);
        root.anchoredPosition = Dimension1SharedLayoutTokens.RootOffset;
        root.sizeDelta = new Vector2(W, H);
        root.SetAsLastSibling();
        root.gameObject.AddComponent<Dimension1VisualSkinRoot>();
        Canvas canvas = root.gameObject.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 32760;
        root.gameObject.AddComponent<GraphicRaycaster>();
        CanvasGroup canvasGroup = root.gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        Dimension1MetalsInventoryUI visual = root.gameObject.AddComponent<Dimension1MetalsInventoryUI>();

        Image background = Image("Background", root, null, Void);
        Stretch(background.rectTransform, new Vector2(-22f, -26f), new Vector2(-22f, -26f));
        background.raycastTarget = true;
        if (starfield != null)
        {
            Image stars = Image("Starfield", root, starfield, Hex("6DC9E6", 7));
            Stretch(stars.rectTransform);
            stars.raycastTarget = false;
        }
        Image outer = Image("OuterFrame", root, frame, Hex("087FA9", 210));
        Top(outer.rectTransform, 8f, 8f, 1064f, 1904f);
        outer.type = UnityEngine.UI.Image.Type.Sliced;
        outer.raycastTarget = false;

        Refs refs = new Refs();
        BuildHeading(root, frame, fill, font, refs);
        BuildSummary(root, frame, fill, font, lockSprite, metalSprites[0], refs);
        BuildFilters(root, frame, fill, font, refs);
        BuildCards(root, frame, fill, font, metalSprites, refs);
        BuildHighestProduction(root, frame, fill, font, refs);
        EnsureEntryButtons(panel.transform, visual);

        visual.Configure(panel, canvasGroup, refs.backButton,
            refs.filterButtons.ToArray(), refs.filterBorders.ToArray(), refs.filterFills.ToArray(),
            refs.filterLabels.ToArray(), refs.totalMetals, refs.unlockedMetals, refs.totalProduction,
            refs.highestMetal, refs.highestProduction, refs.cards.ToArray(), refs.borders.ToArray(),
            refs.icons.ToArray(), refs.names.ToArray(), refs.amounts.ToArray(), refs.rates.ToArray(),
            refs.sources.ToArray(), refs.locked.ToArray(), refs.requirements.ToArray());
        UnityEventTools.AddPersistentListener(refs.backButton.onClick, visual.Close);
        UnityEventTools.AddPersistentListener(refs.filterButtons[0].onClick, visual.SelectAll);
        UnityEventTools.AddPersistentListener(refs.filterButtons[1].onClick, visual.SelectProducing);
        UnityEventTools.AddPersistentListener(refs.filterButtons[2].onClick, visual.SelectLocked);

        EditorUtility.SetDirty(visual);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("Unity no pudo guardar Main.unity.");
        ValidateInternal(scene);
        Debug.Log("[D1 Metals Inventory] INSTALL_PASS | subpantalla 1080x1920 | 10 metales reales | filtros y regreso conectados");
    }

    [MenuItem("Quantum Forge/Dimension 1/Validate Metals Inventory Subscreen")]
    public static void Validate()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        ValidateInternal(scene);
        Debug.Log("[D1 Metals Inventory] VALIDATION_PASS");
    }

    private static void BuildHeading(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font, Refs refs)
    {
        refs.backButton = ButtonPanel("BackButton", root, frame, fill,
            new Vector2(28f, 27f), new Vector2(78f, 68f), Fill, CyanMuted, out _, out _);
        Segment(refs.backButton.transform, "ArrowShaft", new Vector2(-10f, 0f), new Vector2(21f, 0f), 4f, Primary);
        Segment(refs.backButton.transform, "ArrowUpper", new Vector2(-10f, 0f), new Vector2(3f, 13f), 4f, Primary);
        Segment(refs.backButton.transform, "ArrowLower", new Vector2(-10f, 0f), new Vector2(3f, -13f), 4f, Primary);

        TMP_Text title = Text("Title", root, font, "INVENTARIO DE METALES", 38f, FontStyles.Bold, Primary);
        Top(title.rectTransform, 137f, 18f, 806f, 54f);
        title.alignment = TextAlignmentOptions.Center;
        title.characterSpacing = 2.4f;
        TMP_Text subtitle = Text("Subtitle", root, font, "DIMENSIÓN 1 / RECURSOS", 22f, FontStyles.Normal, Cyan);
        Top(subtitle.rectTransform, 250f, 73f, 580f, 34f);
        subtitle.alignment = TextAlignmentOptions.Center;
        Segment(root, "HeadingLineL", new Vector2(-492f, 861f), new Vector2(-226f, 861f), 1.4f, Hex("087FA9", 115));
        Segment(root, "HeadingLineR", new Vector2(226f, 861f), new Vector2(492f, 861f), 1.4f, Hex("087FA9", 115));
    }

    private static void BuildSummary(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font,
        Sprite lockSprite, Sprite ironSprite, Refs refs)
    {
        RectTransform total = Panel("TotalMetals", root, frame, fill,
            new Vector2(26f, 124f), new Vector2(300f, 150f), FillRaised, CyanMuted, out _, out _);
        Image totalIcon = Image("Icon", total, ironSprite, Color.white);
        Top(totalIcon.rectTransform, 4f, 14f, 132f, 122f);
        totalIcon.preserveAspect = true;
        refs.totalMetals = Text("ValueAndLabel", total, font,
            "<size=43><color=#EDF4F7>10</color></size>\n<size=23><color=#9EABB4>METALES</color></size>",
            23f, FontStyles.Normal, Secondary);
        Top(refs.totalMetals.rectTransform, 128f, 23f, 160f, 108f);

        RectTransform unlocked = Panel("UnlockedMetals", root, frame, fill,
            new Vector2(346f, 124f), new Vector2(300f, 150f), FillRaised, CyanMuted, out _, out _);
        Image unlockedIcon = Image("Icon", unlocked, lockSprite, Hex("5AAFCB"));
        Top(unlockedIcon.rectTransform, 28f, 35f, 80f, 80f);
        unlockedIcon.preserveAspect = true;
        refs.unlockedMetals = Text("ValueAndLabel", unlocked, font,
            "<size=43><color=#EDF4F7>0</color></size>\n<size=18><color=#9EABB4>DESBLOQUEADOS</color></size>",
            18f, FontStyles.Normal, Secondary);
        Top(refs.unlockedMetals.rectTransform, 122f, 23f, 166f, 108f);

        RectTransform production = Panel("TotalProduction", root, frame, fill,
            new Vector2(666f, 124f), new Vector2(388f, 150f), FillRaised, CyanMuted, out _, out _);
        DrawProductionIcon(production, new Vector2(-146f, -1f), CyanMuted);
        TMP_Text productionLabel = Text("Label", production, font, "PRODUCCIÓN TOTAL", 19f, FontStyles.Normal, Secondary);
        Top(productionLabel.rectTransform, 138f, 31f, 226f, 31f);
        refs.totalProduction = Text("Value", production, font, "+0/s", 39f, FontStyles.Normal, Cyan);
        Top(refs.totalProduction.rectTransform, 138f, 65f, 226f, 54f);
    }

    private static void BuildFilters(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font, Refs refs)
    {
        string[] labels = { "TODOS", "PRODUCIENDO", "BLOQUEADOS" };
        float[] xs = { 26f, 350f, 706f };
        float[] widths = { 308f, 340f, 348f };
        for (int i = 0; i < 3; i++)
        {
            bool selected = i == 0;
            Button filter = ButtonPanel("Filter_" + i, root, frame, fill,
                new Vector2(xs[i], 296f), new Vector2(widths[i], 74f),
                selected ? AmberFill : Fill, selected ? Amber : CyanMuted,
                out Image filterFill, out Image filterBorder);
            TMP_Text label = Text("Label", filter.transform, font, labels[i], 25f,
                FontStyles.Bold, selected ? Amber : Cyan);
            Stretch(label.rectTransform, new Vector2(18f, 8f), new Vector2(18f, 8f));
            label.alignment = TextAlignmentOptions.Center;
            label.characterSpacing = 2f;
            refs.filterButtons.Add(filter);
            refs.filterFills.Add(filterFill);
            refs.filterBorders.Add(filterBorder);
            refs.filterLabels.Add(label);
        }
    }

    private static void BuildCards(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font,
        Sprite[] metalSprites, Refs refs)
    {
        string[] names = { "HIERRO", "COBRE", "ALUMINIO", "TITANIO", "NÍQUEL", "COBALTO", "LITIO", "TUNGSTENO", "PLATINO", "IRIDIO" };
        for (int i = 0; i < 10; i++)
        {
            int column = i % 2;
            int row = i / 2;
            RectTransform card = Panel("MetalCard_" + i, root, frame, fill,
                new Vector2(26f + column * 524f, 400f + row * 256f), new Vector2(504f, 236f),
                Fill, CyanMuted, out _, out Image border);
            Image icon = Image("MetalIcon", card, metalSprites[i], Color.white);
            Top(icon.rectTransform, -2f, 10f, 194f, 210f);
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            Image divider = Image("Divider", card, null, Hex("087FA9", 85));
            Top(divider.rectTransform, 174f, 38f, 2f, 158f);
            divider.raycastTarget = false;
            TMP_Text name = Text("MetalName", card, font, names[i], 29f, FontStyles.Normal, Secondary);
            Top(name.rectTransform, 197f, 25f, 275f, 42f);
            TMP_Text amount = Text("Amount", card, font, "0", 43f, FontStyles.Normal, Primary);
            Top(amount.rectTransform, 197f, 60f, 275f, 57f);
            TMP_Text rate = Text("Rate", card, font, "+0/s", 33f, FontStyles.Normal, Cyan);
            Top(rate.rectTransform, 197f, 111f, 275f, 45f);
            Image sourceLine = Image("SourceLine", card, null, Hex("087FA9", 75));
            Top(sourceLine.rectTransform, 197f, 163f, 276f, 2f);
            sourceLine.raycastTarget = false;
            TMP_Text source = Text("Source", card, font, "PLANETA 1", 24f, FontStyles.Normal, Secondary);
            Top(source.rectTransform, 228f, 174f, 245f, 38f);
            DrawPlanetIcon(source.transform, font, new Vector2(-17f, 0f), Secondary);
            TMP_Text locked = Text("Locked", card, font, "BLOQUEADO", 25f, FontStyles.Normal, Hex("78858E"));
            Top(locked.rectTransform, 232f, 78f, 240f, 42f);
            DrawLockIcon(locked.transform, new Vector2(-18f, 0f), Hex("78858E"));
            locked.gameObject.SetActive(false);
            TMP_Text requirement = Text("Requirement", card, font, "REQUISITO", 23f, FontStyles.Normal, Hex("78858E"));
            Top(requirement.rectTransform, 197f, 149f, 282f, 51f);
            requirement.alignment = TextAlignmentOptions.Center;
            requirement.gameObject.SetActive(false);
            refs.cards.Add(card);
            refs.borders.Add(border);
            refs.icons.Add(icon);
            refs.names.Add(name);
            refs.amounts.Add(amount);
            refs.rates.Add(rate);
            refs.sources.Add(source);
            refs.locked.Add(locked);
            refs.requirements.Add(requirement);
        }
    }

    private static void BuildHighestProduction(Transform root, Sprite frame, Sprite fill,
        TMP_FontAsset font, Refs refs)
    {
        RectTransform summary = Panel("HighestProduction", root, frame, fill,
            new Vector2(26f, 1688f), new Vector2(1028f, 166f), FillRaised, CyanMuted, out _, out _);
        DrawTargetIcon(summary, font, new Vector2(-438f, 0f), CyanMuted);
        Image divider = Image("Divider", summary, null, Hex("087FA9", 95));
        Top(divider.rectTransform, 142f, 27f, 2f, 112f);
        TMP_Text eyebrow = Text("Eyebrow", summary, font, "MAYOR PRODUCCIÓN", 24f, FontStyles.Normal, Cyan);
        Top(eyebrow.rectTransform, 174f, 35f, 390f, 38f);
        refs.highestMetal = Text("Metal", summary, font, "HIERRO", 36f, FontStyles.Normal, Primary);
        Top(refs.highestMetal.rectTransform, 174f, 75f, 410f, 51f);
        refs.highestProduction = Text("Production", summary, font, "+0/s", 45f, FontStyles.Normal, Cyan);
        Top(refs.highestProduction.rectTransform, 650f, 51f, 338f, 70f);
        refs.highestProduction.alignment = TextAlignmentOptions.Right;
    }

    private static void EnsureEntryButtons(Transform owner, Dimension1MetalsInventoryUI visual)
    {
        foreach (Transform transform in owner.GetComponentsInChildren<Transform>(true))
        {
            if (transform.name != "MetalsButton" && transform.name != "AllMetals") continue;
            Image hit = transform.GetComponent<Image>();
            if (hit == null)
            {
                hit = transform.gameObject.AddComponent<Image>();
                hit.color = Color.clear;
            }
            hit.raycastTarget = true;
            Button button = transform.GetComponent<Button>();
            if (button == null) button = transform.gameObject.AddComponent<Button>();
            button.targetGraphic = hit;
            button.transition = Selectable.Transition.None;
            while (button.onClick.GetPersistentEventCount() > 0)
                UnityEventTools.RemovePersistentListener(button.onClick, 0);
            UnityEventTools.AddPersistentListener(button.onClick, visual.Open);
            EditorUtility.SetDirty(transform.gameObject);
        }
    }

    private static Sprite[] LoadMetalSprites()
    {
        string[] names = { "iron", "copper", "aluminum", "titanium", "nickel", "cobalt", "lithium", "tungsten", "platinum", "iridium" };
        Sprite[] sprites = new Sprite[names.Length];
        for (int i = 0; i < names.Length; i++)
        {
            string path = MetalArtPath + "/d1_metal_" + names[i] + "_v5.png";
            PrepareSprite(path);
            sprites[i] = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
        return sprites;
    }

    private static void PrepareSprite(string path)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;
        bool changed = importer.textureType != TextureImporterType.Sprite || !importer.alphaIsTransparency ||
                       importer.mipmapEnabled || importer.wrapMode != TextureWrapMode.Clamp;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Bilinear;
        if (changed) importer.SaveAndReimport();
    }

    private static void ValidateInternal(Scene scene)
    {
        Transform root = FindSceneTransform(scene, RootName);
        if (root == null) throw new InvalidOperationException("Falta " + RootName + ".");
        if (root.GetComponent<Dimension1MetalsInventoryUI>() == null)
            throw new InvalidOperationException("Falta el controlador del inventario.");
        for (int i = 0; i < 10; i++)
            if (FindDirectChild(root, "MetalCard_" + i) == null)
                throw new InvalidOperationException("Falta MetalCard_" + i + ".");
        int entries = 0;
        foreach (Transform transform in root.parent.GetComponentsInChildren<Transform>(true))
            if ((transform.name == "MetalsButton" || transform.name == "AllMetals") && transform.GetComponent<Button>() != null)
                entries++;
        if (entries < 6) throw new InvalidOperationException("No están conectados los seis accesos de metales.");
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
        fillImage.raycastTarget = false;
        borderImage = Image("Border", rect, frame, borderColor);
        Stretch(borderImage.rectTransform);
        borderImage.type = UnityEngine.UI.Image.Type.Sliced;
        borderImage.raycastTarget = false;
        return rect;
    }

    private static Button ButtonPanel(string name, Transform parent, Sprite frame, Sprite fill,
        Vector2 topLeft, Vector2 size, Color fillColor, Color borderColor,
        out Image fillImage, out Image borderImage)
    {
        RectTransform rect = Panel(name, parent, frame, fill, topLeft, size, fillColor,
            borderColor, out fillImage, out borderImage);
        Image hit = rect.gameObject.AddComponent<Image>();
        hit.color = Color.clear;
        hit.raycastTarget = true;
        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = hit;
        button.transition = Selectable.Transition.None;
        return button;
    }

    private static RectTransform Rect(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return (RectTransform)go.transform;
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
        float size, FontStyles style, Color color)
    {
        RectTransform rect = Rect(name, parent);
        TextMeshProUGUI text = rect.gameObject.AddComponent<TextMeshProUGUI>();
        text.font = font;
        text.text = value;
        text.fontSize = size;
        text.fontStyle = style;
        text.color = color;
        text.alignment = TextAlignmentOptions.Left;
        text.enableWordWrapping = false;
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

    private static void Stretch(RectTransform rect, Vector2 insetMin = default, Vector2 insetMax = default)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(.5f, .5f);
        rect.offsetMin = insetMin;
        rect.offsetMax = -insetMax;
    }

    private static void Segment(Transform parent, string name, Vector2 a, Vector2 b,
        float thickness, Color color)
    {
        Image line = Image(name, parent, null, color);
        RectTransform rect = line.rectTransform;
        rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
        rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = (a + b) * .5f;
        rect.sizeDelta = new Vector2(Vector2.Distance(a, b), thickness);
        rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(b.y - a.y, b.x - a.x) * Mathf.Rad2Deg);
    }

    private static void DrawProductionIcon(Transform parent, Vector2 center, Color color)
    {
        for (int i = 0; i < 4; i++)
        {
            Image bar = Image("ProductionBar_" + i, parent, null, color);
            RectTransform rect = bar.rectTransform;
            rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
            rect.pivot = new Vector2(.5f, 0f);
            rect.anchoredPosition = center + new Vector2(-26f + i * 17f, -31f);
            rect.sizeDelta = new Vector2(9f, 22f + i * 15f);
        }
        Segment(parent, "ProductionTrendA", center + new Vector2(-31f, 17f), center + new Vector2(-5f, 40f), 4f, Cyan);
        Segment(parent, "ProductionTrendB", center + new Vector2(-5f, 40f), center + new Vector2(15f, 25f), 4f, Cyan);
        Segment(parent, "ProductionTrendC", center + new Vector2(15f, 25f), center + new Vector2(39f, 54f), 4f, Cyan);
    }

    private static void DrawTargetIcon(Transform parent, TMP_FontAsset font, Vector2 center, Color color)
    {
        for (int i = 0; i < 3; i++)
        {
            TMP_Text ring = Text("TargetRing_" + i, parent, font, "○", 76f - i * 18f,
                FontStyles.Normal, i == 2 ? Cyan : color);
            RectTransform rect = ring.rectTransform;
            rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
            rect.pivot = new Vector2(.5f, .5f);
            rect.anchoredPosition = center;
            rect.sizeDelta = new Vector2(90f, 90f);
            ring.alignment = TextAlignmentOptions.Center;
        }
        Segment(parent, "TargetV", center + new Vector2(0f, -46f), center + new Vector2(0f, 46f), 2f, color);
        Segment(parent, "TargetH", center + new Vector2(-46f, 0f), center + new Vector2(46f, 0f), 2f, color);
    }

    private static void DrawPlanetIcon(Transform parent, TMP_FontAsset font, Vector2 center, Color color)
    {
        TMP_Text globe = Text("PlanetCircle", parent, font, "○", 39f, FontStyles.Normal, color);
        globe.rectTransform.anchorMin = globe.rectTransform.anchorMax = new Vector2(0f, .5f);
        globe.rectTransform.pivot = new Vector2(.5f, .5f);
        globe.rectTransform.anchoredPosition = center;
        globe.rectTransform.sizeDelta = new Vector2(43f, 43f);
        globe.alignment = TextAlignmentOptions.Center;
        Segment(globe.transform, "PlanetAxis", new Vector2(-13f, 0f), new Vector2(13f, 0f), 1.5f, color);
    }

    private static void DrawLockIcon(Transform parent, Vector2 center, Color color)
    {
        Image body = Image("LockBody", parent, null, color);
        body.rectTransform.anchorMin = body.rectTransform.anchorMax = new Vector2(0f, .5f);
        body.rectTransform.pivot = new Vector2(.5f, .5f);
        body.rectTransform.anchoredPosition = center + new Vector2(0f, -5f);
        body.rectTransform.sizeDelta = new Vector2(23f, 20f);
        SmallBar(parent, "LockShackleL", center + new Vector2(-9f, 9f), new Vector2(3f, 12f), color);
        SmallBar(parent, "LockShackleTop", center + new Vector2(0f, 15f), new Vector2(21f, 3f), color);
        SmallBar(parent, "LockShackleR", center + new Vector2(9f, 9f), new Vector2(3f, 12f), color);
    }

    private static void SmallBar(Transform parent, string name, Vector2 position, Vector2 size, Color color)
    {
        Image bar = Image(name, parent, null, color);
        bar.rectTransform.anchorMin = bar.rectTransform.anchorMax = new Vector2(0f, .5f);
        bar.rectTransform.pivot = new Vector2(.5f, .5f);
        bar.rectTransform.anchoredPosition = position;
        bar.rectTransform.sizeDelta = size;
    }

    private static Sprite LoadSprite(string path)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private static T FindSceneComponent<T>(Scene scene) where T : Component
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            T component = root.GetComponentInChildren<T>(true);
            if (component != null) return component;
        }
        return null;
    }

    private static Transform FindSceneTransform(Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
                if (transform.name == name) return transform;
        }
        return null;
    }

    private static Transform FindDirectChild(Transform parent, string name)
    {
        if (parent == null) return null;
        for (int i = 0; i < parent.childCount; i++)
            if (parent.GetChild(i).name == name) return parent.GetChild(i);
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

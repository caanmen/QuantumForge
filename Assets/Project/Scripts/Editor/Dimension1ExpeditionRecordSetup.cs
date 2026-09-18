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

public static class Dimension1ExpeditionRecordSetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string RootName = "D1_ExpeditionRecordVisualRoot";
    private const string ArtPath = "Assets/Project/UI/Dimension1/Generated";
    private const string MetalPath = ArtPath + "/MetalsInventory";
    private const string AncientPath = ArtPath + "/Candidates/AncientOrbits";
    private const string SectorPath = ArtPath + "/Candidates/SectorDetails";
    private const string SummaryIconPath = ArtPath + "/Candidates/ExpeditionRecordSummary";
    private const string FontPath = "Assets/Project/UI/Vertical/Fonts/Rajdhani-Medium SDF.asset";

    private static readonly Color Void = Hex("01090E", 254);
    private static readonly Color Fill = Hex("031019", 252);
    private static readonly Color Raised = Hex("051721", 252);
    private static readonly Color Cyan = Hex("18C8FF");
    private static readonly Color CyanBright = Hex("4FCFF0");
    private static readonly Color CyanMuted = Hex("087FA9", 230);
    private static readonly Color Primary = Hex("EAF1F4");
    private static readonly Color Secondary = Hex("99A6B0");
    private static readonly Color Amber = Hex("F4A70B");
    private static readonly Color AmberFill = Hex("1A1407", 254);

    private sealed class Refs
    {
        public Button back;
        public Button bottomBack;
        public Button allMetals;
        public readonly List<Button> filters = new List<Button>();
        public readonly List<Image> filterBorders = new List<Image>();
        public readonly List<Image> filterFills = new List<Image>();
        public readonly List<TMP_Text> filterLabels = new List<TMP_Text>();
        public TMP_Text recentCount;
        public TMP_Text empty;
        public ScrollRect scroll;
        public RectTransform content;
        public readonly List<Dimension1ExpeditionRecordUI.RowView> rows =
            new List<Dimension1ExpeditionRecordUI.RowView>();
        public TMP_Text totalExpeditions;
        public TMP_Text totalRelics;
        public TMP_Text totalMetals;
        public readonly List<TMP_Text> headerAmounts = new List<TMP_Text>();
        public readonly List<TMP_Text> headerRates = new List<TMP_Text>();
    }

    [MenuItem("Quantum Forge/Dimension 1/Install Expedition Record Screen")]
    public static void Install()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Dimension1PanelUI panel = FindSceneComponent<Dimension1PanelUI>(scene);
        if (panel == null) throw new InvalidOperationException("Falta Dimension1PanelUI.");

        TMP_FontAsset font = LoadRequired<TMP_FontAsset>(FontPath);
        Sprite frame = LoadRequired<Sprite>(ArtPath + "/d1_premium_frame_v4.png");
        Sprite fill = LoadRequired<Sprite>(ArtPath + "/d1_panel_fill_v4.png");
        Sprite stars = LoadRequired<Sprite>(ArtPath + "/d1_starfield.png");
        Sprite[] metals = LoadMetalSprites();
        Sprite[] ships = LoadShipSprites();
        Sprite[] destinations = LoadDestinationSprites();
        Sprite fallback = LoadRequired<Sprite>(ArtPath + "/d1_nav_explore_v3.png");
        ConfigureSummarySpriteImporters();
        Sprite[] summaryIcons = LoadSummaryIcons();
        Material summaryIconMaterial = LoadRequired<Material>(
            ArtPath + "/NavigationPremium/d1_nav_premium_black_key.mat");
        Material shipMaterial = LoadRequired<Material>(ArtPath + "/d1_hangar_blueprint_keyed.mat");

        Transform old = FindSceneTransform(scene, RootName);
        if (old != null) UnityEngine.Object.DestroyImmediate(old.gameObject);

        RectTransform root = Rect(RootName, panel.transform);
        root.anchorMin = root.anchorMax = new Vector2(.5f, .5f);
        root.pivot = new Vector2(.5f, .5f);
        root.anchoredPosition = Dimension1SharedLayoutTokens.RootOffset;
        root.sizeDelta = new Vector2(1080f, 1920f);
        Canvas canvas = root.gameObject.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 880;
        root.gameObject.AddComponent<GraphicRaycaster>();
        CanvasGroup group = root.gameObject.AddComponent<CanvasGroup>();
        Image backdrop = Image("Backdrop", root, null, Void);
        Stretch(backdrop.rectTransform);
        backdrop.raycastTarget = true;
        Image starfield = Image("Starfield", root, stars, Hex("FFFFFF", 34));
        Stretch(starfield.rectTransform);

        var refs = new Refs();
        BuildHeader(root, frame, fill, font, metals, refs);
        BuildBody(root, frame, fill, font, fallback, summaryIcons, summaryIconMaterial,
            shipMaterial, refs);

        Dimension1ExpeditionRecordUI visual = root.gameObject.AddComponent<Dimension1ExpeditionRecordUI>();
        visual.Configure(panel, group, refs.back, refs.bottomBack, refs.allMetals,
            refs.filters.ToArray(), refs.filterBorders.ToArray(), refs.filterFills.ToArray(),
            refs.filterLabels.ToArray(), refs.recentCount, refs.empty, refs.scroll, refs.content,
            refs.rows.ToArray(), refs.totalExpeditions, refs.totalRelics, refs.totalMetals,
            refs.headerAmounts.ToArray(), refs.headerRates.ToArray(), destinations, fallback, ships);

        UnityEventTools.AddPersistentListener(refs.back.onClick, visual.Close);
        UnityEventTools.AddPersistentListener(refs.bottomBack.onClick, visual.Close);
        UnityEventTools.AddPersistentListener(refs.allMetals.onClick, visual.OpenMetals);
        UnityEventTools.AddPersistentListener(refs.filters[0].onClick, visual.SelectAll);
        UnityEventTools.AddPersistentListener(refs.filters[1].onClick, visual.SelectRelics);
        UnityEventTools.AddPersistentListener(refs.filters[2].onClick, visual.SelectCoordinated);
        UnityEventTools.AddPersistentListener(refs.filters[3].onClick, visual.CycleSector);

        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;

        SerializedObject panelObject = new SerializedObject(panel);
        SerializedProperty property = panelObject.FindProperty("expeditionRecordUI");
        if (property == null) throw new InvalidOperationException("Falta expeditionRecordUI en Dimension1PanelUI.");
        property.objectReferenceValue = visual;
        panelObject.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        ValidateInternal(scene);
        Debug.Log("[D1 Expedition Record] INSTALL_PASS | 20 registros reales | filtros + scroll + Ark fijo");
    }

    [MenuItem("Quantum Forge/Dimension 1/Validate Expedition Record Screen")]
    public static void Validate()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        ValidateInternal(scene);
        Debug.Log("[D1 Expedition Record] VALIDATION_PASS");
    }

    private static void BuildHeader(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font,
        Sprite[] metals, Refs refs)
    {
        refs.back = ButtonPanel("BackButton", root, frame, fill,
            new Vector2(26f, 24f), new Vector2(106f, 106f), Fill, CyanMuted, out _, out _);
        Segment(refs.back.transform, "ArrowShaft", new Vector2(-18f, 0f), new Vector2(22f, 0f), 5f, CyanBright);
        Segment(refs.back.transform, "ArrowUpper", new Vector2(-18f, 0f), new Vector2(2f, 20f), 5f, CyanBright);
        Segment(refs.back.transform, "ArrowLower", new Vector2(-18f, 0f), new Vector2(2f, -20f), 5f, CyanBright);

        TMP_Text title = Text("DimensionTitle", root, font, "DIMENSIÓN 1", 43f,
            FontStyles.Bold, Primary, TextAlignmentOptions.Center);
        Top(title.rectTransform, 285f, 18f, 510f, 58f);
        title.characterSpacing = 2.6f;

        string[] names = { "HIERRO", "ALUMINIO", "NÍQUEL" };
        float[] xs = { 150f, 365f, 580f };
        for (int i = 0; i < 3; i++)
        {
            RectTransform card = Panel("Metal_" + i, root, frame, fill,
                new Vector2(xs[i], 78f), new Vector2(205f, 86f), Raised, CyanMuted, out _, out _);
            Image icon = Image("Icon", card, metals[i], Color.white);
            Top(icon.rectTransform, 8f, 9f, 62f, 62f);
            icon.preserveAspect = true;
            TMP_Text name = Text("Name", card, font, names[i], 16f,
                FontStyles.Normal, Secondary, TextAlignmentOptions.Left);
            Top(name.rectTransform, 68f, 8f, 126f, 24f);
            TMP_Text amount = Text("Amount", card, font, "0", 29f,
                FontStyles.Normal, Primary, TextAlignmentOptions.Left);
            Top(amount.rectTransform, 68f, 29f, 112f, 39f);
            TMP_Text rate = Text("Rate", card, font, "+0/s", 15f,
                FontStyles.Bold, Cyan, TextAlignmentOptions.Right);
            Top(rate.rectTransform, 132f, 57f, 62f, 22f);
            refs.headerAmounts.Add(amount);
            refs.headerRates.Add(rate);
        }

        refs.allMetals = ButtonPanel("AllMetals", root, frame, fill,
            new Vector2(795f, 78f), new Vector2(259f, 86f), Raised, CyanMuted, out _, out _);
        DrawMetalCluster(refs.allMetals.transform, new Vector2(-91f, 0f), font);
        TMP_Text all = Text("Label", refs.allMetals.transform, font, "10 METALES", 23f,
            FontStyles.Bold, Cyan, TextAlignmentOptions.Center);
        Top(all.rectTransform, 64f, 21f, 160f, 42f);
        Segment(refs.allMetals.transform, "ChevronA", new Vector2(96f, 7f), new Vector2(106f, -3f), 3f, Cyan);
        Segment(refs.allMetals.transform, "ChevronB", new Vector2(106f, -3f), new Vector2(116f, 7f), 3f, Cyan);
    }

    private static void BuildBody(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font,
        Sprite fallback, Sprite[] summaryIcons, Material summaryIconMaterial,
        Material shipMaterial, Refs refs)
    {
        RectTransform body = Panel("RecordBody", root, frame, fill,
            new Vector2(18f, 188f), new Vector2(1044f, 1704f), Fill, CyanMuted, out _, out _);
        TMP_Text heading = Text("Heading", body, font, "REGISTRO DE EXPEDICIONES", 40f,
            FontStyles.Bold, Primary, TextAlignmentOptions.Center);
        Top(heading.rectTransform, 160f, 22f, 724f, 56f);
        heading.characterSpacing = 2.4f;
        TMP_Text breadcrumb = Text("Breadcrumb", body, font, "EXPLORAR / REGISTRO", 22f,
            FontStyles.Normal, Cyan, TextAlignmentOptions.Left);
        Top(breadcrumb.rectTransform, 34f, 88f, 430f, 34f);
        refs.recentCount = Text("RecentCount", body, font, "0 REGISTROS RECIENTES", 22f,
            FontStyles.Normal, Cyan, TextAlignmentOptions.Right);
        Top(refs.recentCount.rectTransform, 610f, 88f, 400f, 34f);
        Line(body, "HeadingDivider", 32f, 130f, 980f, Hex("087FA9", 120));

        string[] labels = { "TODAS", "CON RELIQUIAS", "COORDINADAS", "POR SECTOR  V" };
        float[] xs = { 34f, 246f, 470f, 722f };
        float[] widths = { 196f, 208f, 236f, 288f };
        for (int i = 0; i < 4; i++)
        {
            bool selected = i == 0;
            Button filter = ButtonPanel("Filter_" + i, body, frame, fill,
                new Vector2(xs[i], 146f), new Vector2(widths[i], 62f),
                selected ? Raised : Fill, selected ? Cyan : CyanMuted,
                out Image filterFill, out Image filterBorder);
            TMP_Text label = Text("Label", filter.transform, font, labels[i], 22f,
                FontStyles.Bold, selected ? Cyan : CyanMuted, TextAlignmentOptions.Center);
            Stretch(label.rectTransform, new Vector2(12f, 7f), new Vector2(12f, 7f));
            refs.filters.Add(filter);
            refs.filterFills.Add(filterFill);
            refs.filterBorders.Add(filterBorder);
            refs.filterLabels.Add(label);
        }

        RectTransform viewport = Rect("RecordViewport", body);
        Top(viewport, 28f, 224f, 988f, 1210f);
        Image viewportHit = viewport.gameObject.AddComponent<Image>();
        viewportHit.color = Color.clear;
        viewportHit.raycastTarget = true;
        viewport.gameObject.AddComponent<RectMask2D>();
        refs.scroll = viewport.gameObject.AddComponent<ScrollRect>();
        refs.scroll.horizontal = false;
        refs.scroll.vertical = true;
        refs.scroll.movementType = ScrollRect.MovementType.Clamped;
        refs.scroll.inertia = true;
        refs.scroll.decelerationRate = .12f;
        refs.scroll.scrollSensitivity = 52f;
        refs.scroll.viewport = viewport;

        refs.content = Rect("Content", viewport);
        refs.content.anchorMin = new Vector2(0f, 1f);
        refs.content.anchorMax = new Vector2(1f, 1f);
        refs.content.pivot = new Vector2(.5f, 1f);
        refs.content.anchoredPosition = Vector2.zero;
        refs.content.sizeDelta = new Vector2(0f, 5292f);
        refs.scroll.content = refs.content;

        // Se reserva una fila adicional para la clave fija de ARK. Así, cuando
        // existen los 20 registros reales, mostrar ARK no descarta el más antiguo.
        for (int i = 0; i <= Dimension1System.Dimension1RecentExplorationHistoryLimit; i++)
            refs.rows.Add(BuildRow(refs.content, frame, fill, font, fallback, shipMaterial, i));

        refs.empty = Text("EmptyState", viewport, font, "SIN EXPEDICIONES COMPLETADAS", 32f,
            FontStyles.Bold, Secondary, TextAlignmentOptions.Center);
        Stretch(refs.empty.rectTransform, new Vector2(120f, 420f), new Vector2(120f, 420f));
        refs.empty.gameObject.SetActive(false);

        RectTransform summary = Panel("Summary", body, frame, fill,
            new Vector2(28f, 1448f), new Vector2(988f, 132f), Raised, CyanMuted, out _, out _);
        BuildSummaryBlock(summary, font, summaryIcons[0], summaryIconMaterial,
            "EXPEDICIONES", 0f, out refs.totalExpeditions);
        BuildSummaryBlock(summary, font, summaryIcons[1], summaryIconMaterial,
            "RELIQUIAS", 329f, out refs.totalRelics);
        BuildSummaryBlock(summary, font, summaryIcons[2], summaryIconMaterial,
            "METALES", 658f, out refs.totalMetals);
        Line(summary, "SepA", 329f, 20f, 2f, Hex("087FA9", 135), true, 92f);
        Line(summary, "SepB", 658f, 20f, 2f, Hex("087FA9", 135), true, 92f);

        refs.bottomBack = ButtonPanel("ReturnToExplore", body, frame, fill,
            new Vector2(84f, 1596f), new Vector2(876f, 92f), AmberFill, Amber, out _, out _);
        RectTransform backTarget = Rect("TargetIcon", refs.bottomBack.transform);
        backTarget.anchorMin = backTarget.anchorMax = new Vector2(.5f, .5f);
        backTarget.pivot = new Vector2(.5f, .5f);
        backTarget.anchoredPosition = new Vector2(-332f, 0f);
        backTarget.sizeDelta = new Vector2(70f, 70f);
        DrawTargetSymbol(backTarget, Amber);
        TMP_Text backLabel = Text("Label", refs.bottomBack.transform, font, "VOLVER A EXPLORAR", 34f,
            FontStyles.Bold, Hex("FFD46A"), TextAlignmentOptions.Center);
        Stretch(backLabel.rectTransform, new Vector2(110f, 10f), new Vector2(30f, 10f));
        backLabel.characterSpacing = 2.8f;
    }

    private static Dimension1ExpeditionRecordUI.RowView BuildRow(Transform parent, Sprite frame,
        Sprite fill, TMP_FontAsset font, Sprite fallback, Material shipMaterial, int index)
    {
        RectTransform card = Panel("RecordRow_" + index, parent, frame, fill,
            new Vector2(0f, index * 252f), new Vector2(988f, 240f), Raised, CyanMuted, out _, out _);
        RectTransform previewFrame = Panel("PreviewFrame", card, frame, fill,
            new Vector2(12f, 12f), new Vector2(128f, 216f), Fill, CyanMuted, out _, out _);
        Image preview = Image("Preview", previewFrame, fallback, Color.white);
        Top(preview.rectTransform, 14f, 67f, 100f, 132f);
        preview.preserveAspect = true;
        RectTransform centralAccessIcon = Rect("CentralAccessIcon", previewFrame);
        Top(centralAccessIcon, 14f, 68f, 100f, 126f);
        DrawCentralAccessKeyIcon(centralAccessIcon);
        centralAccessIcon.gameObject.SetActive(false);
        TMP_Text indexText = Text("Index", previewFrame, font, "EXPEDICIÓN\n#0", 20f,
            FontStyles.Normal, Primary, TextAlignmentOptions.TopLeft);
        Top(indexText.rectTransform, 8f, 8f, 112f, 58f);

        Line(card, "SepDestination", 148f, 18f, 2f, Hex("087FA9", 120), true, 204f);
        TMP_Text destination = Text("Destination", card, font, "DESTINO", 30f,
            FontStyles.Bold, CyanBright, TextAlignmentOptions.Left);
        Top(destination.rectTransform, 164f, 28f, 280f, 52f);
        destination.enableAutoSizing = true;
        destination.fontSizeMin = 22f;
        destination.fontSizeMax = 30f;
        destination.textWrappingMode = TextWrappingModes.NoWrap;
        TMP_Text sector = Text("Sector", card, font, "SECTOR", 24f,
            FontStyles.Normal, Cyan, TextAlignmentOptions.Left);
        Top(sector.rectTransform, 164f, 88f, 280f, 70f);
        sector.enableAutoSizing = true;
        sector.fontSizeMin = 18f;
        sector.fontSizeMax = 24f;
        sector.textWrappingMode = TextWrappingModes.NoWrap;

        Line(card, "SepShip", 456f, 18f, 2f, Hex("087FA9", 120), true, 204f);
        Image mainShip = Image("MainShip", card, null, Color.white);
        Top(mainShip.rectTransform, 527f, 20f, 98f, 116f);
        mainShip.preserveAspect = true;
        mainShip.material = shipMaterial;
        Image supportShip = Image("SupportShip", card, null, Color.white);
        Top(supportShip.rectTransform, 583f, 20f, 98f, 116f);
        supportShip.preserveAspect = true;
        supportShip.material = shipMaterial;
        TMP_Text shipText = Text("Ship", card, font, "NAVE", 22f,
            FontStyles.Normal, Primary, TextAlignmentOptions.Center);
        Top(shipText.rectTransform, 458f, 142f, 240f, 58f);
        shipText.textWrappingMode = TextWrappingModes.Normal;
        shipText.enableAutoSizing = true;
        shipText.fontSizeMin = 17f;
        shipText.fontSizeMax = 22f;
        TMP_Text synergy = Text("Synergy", card, font, "", 16f,
            FontStyles.Bold, Cyan, TextAlignmentOptions.Center);
        Top(synergy.rectTransform, 458f, 204f, 240f, 24f);
        synergy.enableAutoSizing = true;
        synergy.fontSizeMin = 12f;
        synergy.fontSizeMax = 16f;

        Line(card, "SepStatus", 700f, 18f, 2f, Hex("087FA9", 120), true, 204f);
        RectTransform completed = Rect("CompletedIcon", card);
        Top(completed, 718f, 28f, 42f, 42f);
        DrawShield(completed);
        RectTransform coordinated = Rect("CoordinatedIcon", card);
        Top(coordinated, 718f, 28f, 42f, 42f);
        DrawLinks(coordinated);
        coordinated.gameObject.SetActive(false);
        TMP_Text status = Text("Status", card, font, "COMPLETADA", 29f,
            FontStyles.Bold, CyanBright, TextAlignmentOptions.Left);
        Top(status.rectTransform, 766f, 28f, 204f, 44f);
        TMP_Text rewards = Text("Rewards", card, font, "SIN METALES", 21f,
            FontStyles.Normal, Primary, TextAlignmentOptions.Left);
        Top(rewards.rectTransform, 718f, 88f, 252f, 72f);
        rewards.textWrappingMode = TextWrappingModes.Normal;
        rewards.overflowMode = TextOverflowModes.Truncate;
        var rewardBadges = new GameObject[4];
        var rewardBadgeTexts = new TMP_Text[4];
        for (int rewardIndex = 0; rewardIndex < 4; rewardIndex++)
        {
            int column = rewardIndex % 2;
            int rewardRow = rewardIndex / 2;
            RectTransform rewardBadge = Panel("RewardChip_" + rewardIndex, card, frame, fill,
                new Vector2(718f + column * 130f, 88f + rewardRow * 41f),
                new Vector2(122f, 36f), Fill, Hex("61727C", 220), out _, out _);
            TMP_Text rewardLabel = Text("Label", rewardBadge, font, "NI  0", 17f,
                FontStyles.Normal, Primary, TextAlignmentOptions.Center);
            Stretch(rewardLabel.rectTransform, new Vector2(5f, 2f), new Vector2(5f, 2f));
            rewardBadges[rewardIndex] = rewardBadge.gameObject;
            rewardBadgeTexts[rewardIndex] = rewardLabel;
            rewardBadge.gameObject.SetActive(false);
        }
        RectTransform badge = Panel("ExtraBadge", card, frame, fill,
            new Vector2(718f, 184f), new Vector2(252f, 42f), AmberFill, Amber, out _, out _);
        TMP_Text badgeText = Text("Label", badge, font, "RELIQUIA", 19f,
            FontStyles.Bold, Amber, TextAlignmentOptions.Center);
        Stretch(badgeText.rectTransform, new Vector2(8f, 3f), new Vector2(8f, 3f));

        return new Dimension1ExpeditionRecordUI.RowView
        {
            root = card.gameObject,
            rect = card,
            preview = preview,
            centralAccessIcon = centralAccessIcon.gameObject,
            indexText = indexText,
            destinationText = destination,
            sectorText = sector,
            mainShipIcon = mainShip,
            supportShipIcon = supportShip,
            shipText = shipText,
            synergyText = synergy,
            completedIcon = completed.gameObject,
            coordinatedIcon = coordinated.gameObject,
            statusText = status,
            rewardsText = rewards,
            rewardBadges = rewardBadges,
            rewardBadgeTexts = rewardBadgeTexts,
            extraBadge = badge.gameObject,
            extraBadgeText = badgeText
        };
    }

    private static void BuildSummaryBlock(Transform parent, TMP_FontAsset font, Sprite iconSprite,
        Material iconMaterial, string label, float x, out TMP_Text value)
    {
        Image icon = Image(label + "Icon", parent, iconSprite, Color.white);
        Top(icon.rectTransform, x + 30f, 23f, 80f, 80f);
        icon.preserveAspect = true;
        icon.material = iconMaterial;
        icon.raycastTarget = false;
        value = Text(label + "Value", parent, font, "0", 48f,
            FontStyles.Bold, label == "RELIQUIAS" ? Amber : CyanBright, TextAlignmentOptions.Center);
        Top(value.rectTransform, x + 116f, 18f, 190f, 62f);
        TMP_Text caption = Text(label + "Label", parent, font, label, 20f,
            FontStyles.Normal, label == "RELIQUIAS" ? Amber : Cyan, TextAlignmentOptions.Center);
        Top(caption.rectTransform, x + 116f, 76f, 190f, 34f);
    }

    private static void DrawTargetSymbol(Transform parent, Color color)
    {
        Circle("OuterRing", parent, 31f, 36, 2.7f, color);
        Circle("InnerRing", parent, 18f, 28, 2.2f, color);
        Segment(parent, "RadarSweep", Vector2.zero, new Vector2(18f, 15f), 3f, color);
        Segment(parent, "ReturnShaft", new Vector2(-38f, 0f), new Vector2(-10f, 0f), 3.5f, color);
        Segment(parent, "ReturnUpper", new Vector2(-38f, 0f), new Vector2(-25f, 13f), 3.5f, color);
        Segment(parent, "ReturnLower", new Vector2(-38f, 0f), new Vector2(-25f, -13f), 3.5f, color);
        Dot(parent, "Signal", new Vector2(20f, -8f), 6f, color);
    }

    private static void DrawCentralAccessKeyIcon(Transform parent)
    {
        Circle("OuterCircuit", parent, 41f, 8, 2.8f, CyanBright);
        Circle("InnerCircuit", parent, 28f, 16, 2.1f, Cyan);
        Circle("KeyHead", parent, 10f, 12, 2.7f, Amber);
        Segment(parent, "KeyStem", new Vector2(0f, -10f), new Vector2(0f, -35f), 3.2f, Amber);
        Segment(parent, "KeyToothA", new Vector2(0f, -25f), new Vector2(11f, -25f), 3.2f, Amber);
        Segment(parent, "KeyToothB", new Vector2(0f, -34f), new Vector2(-9f, -34f), 3.2f, Amber);
        Dot(parent, "EchoTop", new Vector2(0f, 46f), 5f, CyanBright);
        Dot(parent, "EchoRight", new Vector2(46f, 0f), 5f, CyanBright);
        Dot(parent, "EchoBottom", new Vector2(0f, -46f), 5f, CyanBright);
        Dot(parent, "EchoLeft", new Vector2(-46f, 0f), 5f, CyanBright);
    }

    private static Sprite[] LoadMetalSprites()
    {
        string[] names = { "iron", "aluminum", "nickel" };
        var result = new Sprite[names.Length];
        for (int i = 0; i < names.Length; i++)
            result[i] = LoadRequired<Sprite>(MetalPath + "/d1_metal_" + names[i] + "_v5.png");
        return result;
    }

    private static Sprite[] LoadSummaryIcons()
    {
        string[] names = { "expeditions", "relics", "metals" };
        var result = new Sprite[names.Length];
        for (int i = 0; i < names.Length; i++)
            result[i] = LoadRequired<Sprite>(SummaryIconPath +
                "/d1_record_summary_" + names[i] + "_candidate_v1.png");
        return result;
    }

    private static void ConfigureSummarySpriteImporters()
    {
        string[] names = { "expeditions", "relics", "metals" };
        for (int i = 0; i < names.Length; i++)
        {
            string path = SummaryIconPath + "/d1_record_summary_" + names[i] +
                "_candidate_v1.png";
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
                throw new InvalidOperationException("Falta importador de textura: " + path);
            bool changed = importer.textureType != TextureImporterType.Sprite ||
                importer.spriteImportMode != SpriteImportMode.Single || importer.mipmapEnabled ||
                importer.wrapMode != TextureWrapMode.Clamp ||
                importer.textureCompression != TextureImporterCompression.Uncompressed;
            if (!changed) continue;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = 2048;
            importer.SaveAndReimport();
        }
    }

    private static Sprite[] LoadShipSprites()
    {
        return new[]
        {
            LoadRequired<Sprite>(ArtPath + "/d1_hangar_sonda_ligera_blueprint_v2.png"),
            LoadRequired<Sprite>(ArtPath + "/d1_hangar_dron_extractor_blueprint_v2.png"),
            LoadRequired<Sprite>(ArtPath + "/d1_hangar_sonda_analitica_blueprint_v2.png"),
            LoadRequired<Sprite>(ArtPath + "/d1_hangar_nave_carga_blueprint_v2.png")
        };
    }

    private static Sprite[] LoadDestinationSprites()
    {
        string[] paths =
        {
            SectorPath + "/d1_destination_mineral_belt_v2.png",
            SectorPath + "/d1_destination_ship_graveyard_v2.png",
            SectorPath + "/d1_destination_drifting_probes_v2.png",
            AncientPath + "/d1_destination_abandoned_ship_v2.png",
            AncientPath + "/d1_destination_orbital_ruin_v2.png",
            AncientPath + "/d1_destination_laboratory_v2.png",
            AncientPath + "/d1_destination_abandoned_station_v2.png",
            SectorPath + "/d1_destination_minor_anomaly_v2.png",
            SectorPath + "/d1_destination_ancient_structure_v2.png",
            SectorPath + "/d1_destination_unstable_zone_v2.png"
        };
        var result = new Sprite[paths.Length];
        for (int i = 0; i < paths.Length; i++) result[i] = LoadRequired<Sprite>(paths[i]);
        return result;
    }

    private static void ValidateInternal(Scene scene)
    {
        Transform root = FindSceneTransform(scene, RootName);
        if (root == null || root.GetComponent<Dimension1ExpeditionRecordUI>() == null ||
            root.GetComponent<CanvasGroup>() == null || FindChild(root, "RecordViewport") == null ||
            FindChild(root, "RecordRow_20") == null || FindChild(root, "ReturnToExplore")?.GetComponent<Button>() == null)
            throw new InvalidOperationException("La estructura del Registro de Expediciones está incompleta.");
        Dimension1PanelUI panel = FindSceneComponent<Dimension1PanelUI>(scene);
        SerializedObject panelObject = new SerializedObject(panel);
        if (panelObject.FindProperty("expeditionRecordUI")?.objectReferenceValue == null)
            throw new InvalidOperationException("Dimension1PanelUI no conserva el Registro visual.");
    }

    private static RectTransform Panel(string name, Transform parent, Sprite frame, Sprite fill,
        Vector2 position, Vector2 size, Color fillColor, Color borderColor,
        out Image fillImage, out Image borderImage)
    {
        RectTransform rect = Rect(name, parent);
        Top(rect, position.x, position.y, size.x, size.y);
        fillImage = Image("Fill", rect, fill, fillColor);
        Stretch(fillImage.rectTransform, new Vector2(4f, 4f), new Vector2(4f, 4f));
        fillImage.type = UnityEngine.UI.Image.Type.Sliced;
        borderImage = Image("Border", rect, frame, borderColor);
        Stretch(borderImage.rectTransform);
        borderImage.type = UnityEngine.UI.Image.Type.Sliced;
        return rect;
    }

    private static Button ButtonPanel(string name, Transform parent, Sprite frame, Sprite fill,
        Vector2 position, Vector2 size, Color fillColor, Color borderColor,
        out Image fillImage, out Image borderImage)
    {
        RectTransform rect = Panel(name, parent, frame, fill, position, size, fillColor,
            borderColor, out fillImage, out borderImage);
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

    private static void Segment(Transform parent, string name, Vector2 a, Vector2 b,
        float thickness, Color color)
    {
        Image line = Image(name, parent, null, color);
        RectTransform rect = line.rectTransform;
        rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
        rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = (a + b) * .5f;
        rect.sizeDelta = new Vector2(Vector2.Distance(a, b), thickness);
        rect.localRotation = Quaternion.Euler(0f, 0f,
            Mathf.Atan2(b.y - a.y, b.x - a.x) * Mathf.Rad2Deg);
    }

    private static void Line(Transform parent, string name, float x, float y, float width,
        Color color, bool vertical = false, float verticalHeight = 150f)
    {
        Image line = Image(name, parent, null, color);
        Top(line.rectTransform, x, y, width, vertical ? verticalHeight : 2f);
    }

    private static void Circle(string name, Transform parent, float radius, int segments,
        float thickness, Color color)
    {
        RectTransform rect = Rect(name, parent);
        rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
        rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.one * (radius * 2f + thickness * 2f);
        var points = new List<Vector2>(segments);
        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.PI * 2f * i / segments;
            points.Add(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius);
        }
        Dimension1CommandCenterLineGraphic ring =
            rect.gameObject.AddComponent<Dimension1CommandCenterLineGraphic>();
        ring.color = color;
        ring.raycastTarget = false;
        ring.SetLine(points, thickness, true);
    }

    private static void Dot(Transform parent, string name, Vector2 position,
        float diameter, Color color)
    {
        Image dot = Image(name, parent, null, color);
        dot.rectTransform.anchorMin = dot.rectTransform.anchorMax = new Vector2(.5f, .5f);
        dot.rectTransform.pivot = new Vector2(.5f, .5f);
        dot.rectTransform.anchoredPosition = position;
        dot.rectTransform.sizeDelta = Vector2.one * diameter;
        dot.raycastTarget = false;
    }

    private static void DrawShield(Transform parent)
    {
        Segment(parent, "ShieldA", new Vector2(-13f, 12f), new Vector2(13f, 12f), 3f, CyanBright);
        Segment(parent, "ShieldB", new Vector2(-13f, 12f), new Vector2(-11f, -5f), 3f, CyanBright);
        Segment(parent, "ShieldC", new Vector2(13f, 12f), new Vector2(11f, -5f), 3f, CyanBright);
        Segment(parent, "ShieldD", new Vector2(-11f, -5f), new Vector2(0f, -15f), 3f, CyanBright);
        Segment(parent, "ShieldE", new Vector2(11f, -5f), new Vector2(0f, -15f), 3f, CyanBright);
        Segment(parent, "CheckA", new Vector2(-6f, 0f), new Vector2(-1f, -5f), 3f, CyanBright);
        Segment(parent, "CheckB", new Vector2(-1f, -5f), new Vector2(8f, 5f), 3f, CyanBright);
    }

    private static void DrawLinks(Transform parent)
    {
        DrawDiamond(parent, new Vector2(-7f, 0f));
        DrawDiamond(parent, new Vector2(7f, 0f));
        Segment(parent, "Link", new Vector2(-2f, 0f), new Vector2(2f, 0f), 3f, CyanBright);
    }

    private static void DrawDiamond(Transform parent, Vector2 center)
    {
        Segment(parent, "D1", center + new Vector2(0f, 9f), center + new Vector2(7f, 0f), 3f, CyanBright);
        Segment(parent, "D2", center + new Vector2(7f, 0f), center + new Vector2(0f, -9f), 3f, CyanBright);
        Segment(parent, "D3", center + new Vector2(0f, -9f), center + new Vector2(-7f, 0f), 3f, CyanBright);
        Segment(parent, "D4", center + new Vector2(-7f, 0f), center + new Vector2(0f, 9f), 3f, CyanBright);
    }

    private static void DrawMetalCluster(Transform parent, Vector2 center, TMP_FontAsset font)
    {
        Vector2[] offsets =
        {
            new Vector2(-12f, 7f), new Vector2(0f, 15f), new Vector2(12f, 7f),
            new Vector2(-12f, -8f), new Vector2(0f, 0f), new Vector2(12f, -8f)
        };
        for (int i = 0; i < offsets.Length; i++)
        {
            TMP_Text dot = Text("MetalDot_" + i, parent, font, "○", 23f,
                FontStyles.Bold, i == 4 ? CyanBright : Cyan, TextAlignmentOptions.Center);
            dot.rectTransform.anchorMin = dot.rectTransform.anchorMax = new Vector2(.5f, .5f);
            dot.rectTransform.pivot = new Vector2(.5f, .5f);
            dot.rectTransform.anchoredPosition = center + offsets[i];
            dot.rectTransform.sizeDelta = new Vector2(28f, 28f);
        }
    }

    private static void DrawTarget(Transform parent, Vector2 center)
    {
        for (int i = 0; i < 3; i++)
        {
            float size = 22f + i * 13f;
            Image ring = Image("TargetRing_" + i, parent, null, Color.clear);
            ring.rectTransform.anchorMin = ring.rectTransform.anchorMax = new Vector2(.5f, .5f);
            ring.rectTransform.pivot = new Vector2(.5f, .5f);
            ring.rectTransform.anchoredPosition = center;
            ring.rectTransform.sizeDelta = new Vector2(size, size);
            Outline outline = ring.gameObject.AddComponent<Outline>();
            outline.effectColor = Amber;
            outline.effectDistance = new Vector2(2f, 2f);
        }
        Segment(parent, "TargetH", center + new Vector2(-33f, 0f), center + new Vector2(33f, 0f), 2f, Amber);
        Segment(parent, "TargetV", center + new Vector2(0f, -33f), center + new Vector2(0f, 33f), 2f, Amber);
    }

    private static T LoadRequired<T>(string path) where T : UnityEngine.Object
    {
        T value = AssetDatabase.LoadAssetAtPath<T>(path);
        if (value == null) throw new InvalidOperationException("Asset no disponible: " + path);
        return value;
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

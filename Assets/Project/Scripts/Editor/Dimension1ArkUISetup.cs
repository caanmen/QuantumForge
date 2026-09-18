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

public static class Dimension1ArkUISetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ArtPath = "Assets/Project/UI/Dimension1/Generated";
    private const string MetalPath = ArtPath + "/MetalsInventory";
    private const string HeroPath = ArtPath + "/Candidates/ArkCenter/d1_ark_center_hero_candidate_v1.png";
    private const string FontPath = "Assets/Project/UI/Vertical/Fonts/Rajdhani-Medium SDF.asset";
    private const float W = 1080f;
    private const float H = 1920f;

    private static readonly Color Void = Hex("00070B");
    private static readonly Color Fill = Hex("010A0F", 252);
    private static readonly Color Raised = Hex("020D14", 252);
    private static readonly Color Cyan = Hex("16B9E8");
    private static readonly Color CyanBright = Hex("59CCE8");
    private static readonly Color CyanMuted = Hex("07536C", 184);
    private static readonly Color Primary = Hex("DCE4E8");
    private static readonly Color Secondary = Hex("8B969F");
    private static readonly Color Amber = Hex("D99A12");
    private static readonly Color Disabled = Hex("565E63");
    private static readonly Color BlueprintNeutral = Hex("71C7DA", 214);

    private sealed class BuildRefs
    {
        public Button back;
        public Button allMetals;
        public Button investigate;
        public readonly List<TMP_Text> amounts = new List<TMP_Text>();
        public readonly List<TMP_Text> rates = new List<TMP_Text>();
        public TMP_Text investigationTitle;
        public TMP_Text investigationSubtitle;
        public readonly List<Button> missionButtons = new List<Button>();
        public readonly List<Dimension1ArkVisualUI.MissionView> missions = new List<Dimension1ArkVisualUI.MissionView>();
        public TMP_Text accessProgress;
        public readonly List<Graphic> echoFrames = new List<Graphic>();
        public readonly List<Graphic> echoCores = new List<Graphic>();
        public readonly List<TMP_Text> echoStatuses = new List<TMP_Text>();
        public Button enter;
        public Image finalFrame;
        public TMP_Text finalLabel;
        public TMP_Text feedback;
        public TMP_Text legacyInfo;
    }

    [MenuItem("Quantum Forge/Dimension 1/Install Ark Center Subscreen")]
    public static void Install()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Dimension1PanelUI panel = FindSceneComponent<Dimension1PanelUI>(scene);
        Dimension1MetalsInventoryUI metals = FindSceneComponent<Dimension1MetalsInventoryUI>(scene);
        if (panel == null || metals == null)
            throw new InvalidOperationException("Faltan propietarios reales de la pantalla del ARK.");

        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        string[] paths =
        {
            ArtPath + "/d1_premium_frame_v4.png", ArtPath + "/d1_panel_fill_v4.png",
            ArtPath + "/d1_starfield.png", HeroPath,
            MetalPath + "/d1_metal_iron_v5.png", MetalPath + "/d1_metal_aluminum_v5.png",
            MetalPath + "/d1_metal_nickel_v5.png",
            ArtPath + "/d1_hangar_sonda_ligera_blueprint_v2.png",
            ArtPath + "/d1_hangar_dron_extractor_blueprint_v2.png",
            ArtPath + "/d1_hangar_sonda_analitica_blueprint_v2.png",
            ArtPath + "/d1_hangar_nave_carga_blueprint_v2.png",
            ArtPath + "/NavigationPremium/d1_nav_galaxy_premium_v1.png", ArtPath + "/d1_orbit_ring_v3.png",
            ArtPath + "/d1_lock_v3.png"
        };
        foreach (string path in paths) PrepareSprite(path);
        Sprite[] sprites = new Sprite[paths.Length];
        for (int i = 0; i < paths.Length; i++) sprites[i] = LoadSprite(paths[i]);
        if (font == null || HasNull(sprites))
            throw new InvalidOperationException("Faltan assets canónicos requeridos para el ARK.");
        Material blueprint = AssetDatabase.LoadAssetAtPath<Material>(ArtPath + "/d1_hangar_blueprint_keyed.mat");
        Material navBlackKey = AssetDatabase.LoadAssetAtPath<Material>(
            ArtPath + "/NavigationPremium/d1_nav_premium_black_key.mat");

        Transform existing = FindSceneTransform(scene, "ArkPanel");
        Transform arkParent = existing != null && existing.parent != null ? existing.parent : panel.transform;
        if (existing != null) UnityEngine.Object.DestroyImmediate(existing.gameObject);
        GameObject ark = new GameObject("ArkPanel", typeof(RectTransform));
        ark.transform.SetParent(arkParent, false);

        RectTransform root = ark.GetComponent<RectTransform>();
        root.anchorMin = root.anchorMax = new Vector2(.5f, .5f);
        root.pivot = new Vector2(.5f, .5f);
        root.anchoredPosition = Dimension1SharedLayoutTokens.RootOffset;
        root.sizeDelta = new Vector2(W, H);
        root.SetAsLastSibling();
        ark.AddComponent<Dimension1VisualSkinRoot>();
        Canvas canvas = ark.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 32760;
        ark.AddComponent<GraphicRaycaster>();
        Dimension1ArkVisualUI visual = ark.AddComponent<Dimension1ArkVisualUI>();

        Sprite frame = sprites[0];
        Sprite fill = sprites[1];
        Image background = Image("Background", root, null, Void);
        Stretch(background.rectTransform, new Vector2(-22f, -28f), new Vector2(-22f, -28f));
        background.raycastTarget = false;
        Image starfield = Image("Starfield", root, sprites[2], Hex("8CC8DA", 11));
        Stretch(starfield.rectTransform);
        Image outer = Image("OuterFrame", root, frame, CyanMuted);
        Top(outer.rectTransform, 8f, 8f, 1064f, 1898f); outer.type = UnityEngine.UI.Image.Type.Sliced;

        BuildRefs refs = new BuildRefs();
        BuildHeader(root, frame, fill, font, new[] { sprites[4], sprites[5], sprites[6] }, refs);
        BuildHero(root, frame, fill, font, sprites[3], sprites[11], navBlackKey, refs);
        BuildMissions(root, frame, fill, font, new[] { sprites[7], sprites[8], sprites[9], sprites[10] }, blueprint, refs);
        BuildAccess(root, frame, fill, font, sprites[12], refs);
        BuildFinal(root, frame, fill, font, sprites[13], refs);

        visual.Configure(metals, refs.amounts.ToArray(), refs.rates.ToArray(),
            refs.investigationTitle, refs.investigationSubtitle, refs.missions.ToArray(),
            refs.accessProgress, refs.echoFrames.ToArray(), refs.echoCores.ToArray(),
            refs.echoStatuses.ToArray(), refs.finalFrame, refs.finalLabel, refs.feedback);
        AddPersistent(refs.allMetals.onClick, visual.OpenMetals);
        AddPersistent(refs.back.onClick, panel.OnClickCloseArkPanel);
        AddPersistent(refs.investigate.onClick, panel.OnClickInvestigateArk);
        AddPersistent(refs.missionButtons[0].onClick, panel.OnClickStartOuterSync);
        AddPersistent(refs.missionButtons[1].onClick, panel.OnClickStartDebrisSync);
        AddPersistent(refs.missionButtons[2].onClick, panel.OnClickStartAncientSync);
        AddPersistent(refs.missionButtons[3].onClick, panel.OnClickStartSilentSync);
        AddPersistent(refs.enter.onClick, panel.OnClickEnterArk);

        // ARK no usa navegación inferior por contrato, pero sí el mismo lienzo,
        // desplazamiento y marco exterior de las pantallas hermanas D1.
        Dimension1SharedShellApply.ApplyFrameToSubscreen(root);

        SerializedObject serialized = new SerializedObject(panel);
        Assign(serialized, "arkPanel", ark);
        Assign(serialized, "closeArkPanelButton", refs.back);
        Assign(serialized, "arkInfoText", refs.legacyInfo);
        Assign(serialized, "investigateArkButton", refs.investigate);
        Assign(serialized, "startOuterSyncButton", refs.missionButtons[0]);
        Assign(serialized, "startDebrisSyncButton", refs.missionButtons[1]);
        Assign(serialized, "startAncientSyncButton", refs.missionButtons[2]);
        Assign(serialized, "startSilentSyncButton", refs.missionButtons[3]);
        Assign(serialized, "enterArkButton", refs.enter);
        serialized.ApplyModifiedPropertiesWithoutUndo();

        Image globalBackground = FindSceneTransform(scene, "BackgroundMobile")?.GetComponent<Image>();
        if (globalBackground != null) globalBackground.raycastTarget = false;
        ark.SetActive(false);
        EditorUtility.SetDirty(visual);
        EditorUtility.SetDirty(panel);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("Unity no pudo guardar Main.unity.");
        ValidateInternal(scene);
        Debug.Log("[D1 Ark Center] INSTALL_PASS | gráfica estática | 4 sincronías reales | ARK 90m real");
    }

    [MenuItem("Quantum Forge/Dimension 1/Validate Ark Center Subscreen")]
    public static void Validate()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        ValidateInternal(scene);
        Debug.Log("[D1 Ark Center] VALIDATION_PASS");
    }

    private static void BuildHeader(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font,
        Sprite[] metals, BuildRefs refs)
    {
        RectTransform header = Panel("Header", root, frame, fill, new Vector2(18f, 16f),
            new Vector2(1044f, 204f), Fill, CyanMuted, out _, out _);
        refs.back = ButtonPanel("CloseArkPanelButton", header, frame, fill, new Vector2(16f, 16f),
            new Vector2(84f, 72f), Raised, CyanMuted, out _, out _);
        Segment(refs.back.transform, "ArrowShaft", new Vector2(-12f, 0f), new Vector2(18f, 0f), 5f, CyanBright);
        Segment(refs.back.transform, "ArrowUpper", new Vector2(-12f, 0f), new Vector2(3f, 15f), 5f, CyanBright);
        Segment(refs.back.transform, "ArrowLower", new Vector2(-12f, 0f), new Vector2(3f, -15f), 5f, CyanBright);
        TMP_Text title = Text("Title", header, font, "DIMENSIÓN 1", 42f, FontStyles.Bold, Primary);
        Top(title.rectTransform, 210f, 10f, 624f, 58f); title.alignment = TextAlignmentOptions.Center; title.characterSpacing = 2f;

        string[] names = { "HIERRO", "ALUMINIO", "NÍQUEL" };
        float[] xs = { 16f, 274f, 532f };
        for (int i = 0; i < 3; i++)
        {
            RectTransform card = Panel("Metal_" + i, header, frame, fill, new Vector2(xs[i], 88f),
                new Vector2(244f, 96f), Raised, CyanMuted, out _, out _);
            Image icon = Image("Icon", card, metals[i], Color.white);
            Top(icon.rectTransform, 12f, 16f, 66f, 66f); icon.preserveAspect = true;
            TMP_Text name = Text("Name", card, font, names[i], 17f, FontStyles.Normal, Secondary);
            Top(name.rectTransform, 79f, 10f, 144f, 25f);
            TMP_Text amount = Text("Amount", card, font, "0", 29f, FontStyles.Normal, Primary);
            Top(amount.rectTransform, 79f, 34f, 112f, 39f);
            TMP_Text rate = Text("Rate", card, font, "+0/s", 16f, FontStyles.Bold, Cyan);
            Top(rate.rectTransform, 158f, 61f, 70f, 22f); rate.alignment = TextAlignmentOptions.Right;
            refs.amounts.Add(amount); refs.rates.Add(rate);
        }
        refs.allMetals = ButtonPanel("AllMetals", header, frame, fill, new Vector2(790f, 88f),
            new Vector2(238f, 96f), Raised, CyanMuted, out _, out _);
        DrawMetalCluster(refs.allMetals.transform, new Vector2(-82f, 0f));
        TMP_Text label = Text("Label", refs.allMetals.transform, font, "10 METALES", 22f, FontStyles.Bold, Cyan);
        Top(label.rectTransform, 56f, 28f, 148f, 34f); label.alignment = TextAlignmentOptions.Center;
        Segment(refs.allMetals.transform, "ChevronA", new Vector2(90f, 7f), new Vector2(99f, -2f), 3f, Cyan);
        Segment(refs.allMetals.transform, "ChevronB", new Vector2(99f, -2f), new Vector2(108f, 7f), 3f, Cyan);
    }

    private static void BuildHero(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font,
        Sprite hero, Sprite galaxy, Material navBlackKey, BuildRefs refs)
    {
        RectTransform panel = Panel("Hero", root, frame, fill, new Vector2(18f, 230f),
            new Vector2(1044f, 720f), Fill, CyanMuted, out _, out _);
        TMP_Text eye = Text("Eyebrow", panel, font, "CARTA GALÁCTICA / CENTRO", 23f, FontStyles.Normal, Cyan);
        Top(eye.rectTransform, 30f, 24f, 700f, 34f); eye.characterSpacing = 1.2f;
        TMP_Text title = Text("Title", panel, font, "ARK — CENTRO GALÁCTICO", 42f, FontStyles.Bold, Primary);
        Top(title.rectTransform, 30f, 62f, 850f, 58f); title.characterSpacing = 1.4f;
        Image art = Image("ArkHeroArt", panel, hero, Hex("B8CDD6", 208));
        Top(art.rectTransform, 18f, 112f, 1008f, 462f); art.preserveAspect = false;
        RectTransform strip = Panel("InvestigationStrip", panel, frame, fill, new Vector2(28f, 585f),
            new Vector2(988f, 108f), Raised, CyanMuted, out _, out _);
        RectTransform badge = Panel("SignalBadge", strip, frame, fill, new Vector2(14f, 10f),
            new Vector2(92f, 88f), Void, CyanMuted, out _, out _);
        Image icon = Image("Icon", badge, galaxy, Color.white);
        Center(icon.rectTransform, new Vector2(-2f, 0f), new Vector2(66f, 66f)); icon.preserveAspect = true;
        if (navBlackKey != null) icon.material = navBlackKey;
        TMP_Text unknown = Text("UnknownMark", badge, font, "?", 24f, FontStyles.Bold, CyanBright);
        Top(unknown.rectTransform, 57f, 48f, 24f, 28f); unknown.alignment = TextAlignmentOptions.Center;
        refs.investigationTitle = Text("Title", strip, font, "ARK INVESTIGADA", 27f, FontStyles.Bold, Cyan);
        Top(refs.investigationTitle.rectTransform, 128f, 16f, 680f, 38f);
        refs.investigationSubtitle = Text("Subtitle", strip, font, "La entrada espera una coincidencia.",
            22f, FontStyles.Normal, Secondary);
        Top(refs.investigationSubtitle.rectTransform, 128f, 55f, 750f, 34f);
        refs.investigate = ClickSurface("InvestigateArkButton", strip);
        refs.legacyInfo = Text("ArkInfoText", panel, font, "", 1f, FontStyles.Normal, Color.clear);
        Top(refs.legacyInfo.rectTransform, 0f, 0f, 1f, 1f);
    }

    private static void BuildMissions(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font,
        Sprite[] ships, Material blueprint, BuildRefs refs)
    {
        SectionTitle(root, font, "MISIONES DE SINCRONÍA", 958f);
        string[] ids = { Dimension1System.D1CentralSyncOuter, Dimension1System.D1CentralSyncDebris,
            Dimension1System.D1CentralSyncAncient, Dimension1System.D1CentralSyncSilent };
        string[] titles = { "SINCRONÍA\nEXTERIOR", "SINCRONÍA\nDE RESTOS", "SINCRONÍA\nANTIGUA", "SINCRONÍA\nSILENCIOSA" };
        string[] shipNames = { "SONDA LIGERA", "DRON EXTRACTOR", "SONDA ANALÍTICA", "NAVE DE CARGA" };
        for (int i = 0; i < 4; i++)
        {
            RectTransform card = Panel("Mission_" + i, root, frame, fill,
                new Vector2(31f + i * 258f, 1002f), new Vector2(244f, 376f), Fill, CyanMuted,
                out Image border, out Image surface);
            Image glow = Image("StateGlow", card, null, Color.clear);
            Stretch(glow.rectTransform, new Vector2(8f, 8f), new Vector2(8f, 8f));
            TMP_Text title = Text("Title", card, font, titles[i], 23f, FontStyles.Normal, Primary);
            Top(title.rectTransform, 12f, 12f, 220f, 60f); title.alignment = TextAlignmentOptions.Center;
            RectTransform artwork = Rect("Artwork", card);
            Top(artwork, 22f, 72f, 200f, 172f);
            Image ship = Image("Ship", artwork, ships[i], BlueprintNeutral);
            Vector2[] sizes = { new Vector2(174f, 154f), new Vector2(184f, 154f),
                new Vector2(168f, 158f), new Vector2(188f, 154f) };
            float[] centerCorrections = { -4f, -8f, -5f, 0f };
            Center(ship.rectTransform, new Vector2(0f, centerCorrections[i]), sizes[i]);
            ship.preserveAspect = true;
            if (blueprint != null) ship.material = blueprint;
            TMP_Text shipName = Text("ShipName", card, font, shipNames[i], 19f, FontStyles.Bold, Cyan);
            Top(shipName.rectTransform, 8f, 244f, 228f, 32f); shipName.alignment = TextAlignmentOptions.Center;
            LineTop(card, "Separator", 18f, 282f, 208f);
            TMP_Text status = Text("Status", card, font, "PENDIENTE · 60 MIN", 18f, FontStyles.Bold, Disabled);
            Top(status.rectTransform, 8f, 302f, 228f, 30f); status.alignment = TextAlignmentOptions.Center;
            TMP_Text timer = Text("Timer", card, font, "", 30f, FontStyles.Bold, Amber);
            Top(timer.rectTransform, 8f, 334f, 228f, 34f); timer.alignment = TextAlignmentOptions.Center;
            Button action = ClickSurface("MissionAction", card);
            refs.missionButtons.Add(action);
            refs.missions.Add(new Dimension1ArkVisualUI.MissionView
                { missionId = ids[i], frame = border, glow = glow, artwork = ship,
                    title = title, status = status, timer = timer });
            surface.raycastTarget = false;
        }
    }

    private static void BuildAccess(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font,
        Sprite orbit, BuildRefs refs)
    {
        RectTransform panel = Panel("AccessKey", root, frame, fill, new Vector2(22f, 1400f),
            new Vector2(1036f, 270f), Fill, CyanMuted, out _, out _);
        TMP_Text title = Text("Title", panel, font, "CLAVE DE ACCESO CENTRAL", 26f, FontStyles.Normal, Cyan);
        Top(title.rectTransform, 170f, 12f, 696f, 38f); title.alignment = TextAlignmentOptions.Center;
        refs.accessProgress = Text("Progress", panel, font, "0/4 ECOS", 22f, FontStyles.Bold, Cyan);
        Top(refs.accessProgress.rectTransform, 368f, 48f, 300f, 34f); refs.accessProgress.alignment = TextAlignmentOptions.Center;
        for (int i = 0; i < 4; i++)
        {
            float centerX = 209f + i * 203f;
            RectTransform diagram = Rect("EchoDiagram_" + i, panel);
            Top(diagram, centerX - 54f, 82f, 108f, 108f);
            Graphic ring = DrawEchoDiagram(diagram, "EchoRing_" + i, Disabled, out Graphic core);
            TMP_Text name = Text("EchoName_" + i, panel, font, "ECO " + (i + 1), 21f,
                FontStyles.Normal, i < 2 ? Cyan : Disabled);
            Top(name.rectTransform, centerX - 90f, 186f, 180f, 30f); name.alignment = TextAlignmentOptions.Center;
            TMP_Text status = Text("EchoStatus_" + i, panel, font, "INACTIVO", 17f, FontStyles.Bold, Disabled);
            Top(status.rectTransform, centerX - 90f, 220f, 180f, 28f); status.alignment = TextAlignmentOptions.Center;
            refs.echoFrames.Add(ring); refs.echoCores.Add(core); refs.echoStatuses.Add(status);
        }
    }

    private static void BuildFinal(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font,
        Sprite lockSprite, BuildRefs refs)
    {
        RectTransform panel = Panel("FinalMission", root, frame, fill, new Vector2(22f, 1690f),
            new Vector2(1036f, 170f), Hex("070D12", 252), Disabled, out Image border, out _);
        refs.finalFrame = border;
        Image lockIcon = Image("Lock", panel, lockSprite, Disabled);
        Top(lockIcon.rectTransform, 170f, 50f, 66f, 66f); lockIcon.preserveAspect = true;
        refs.finalLabel = Text("Label", panel, font, "ENTRAR A ARK · 90 MIN", 40f, FontStyles.Bold, Disabled);
        Top(refs.finalLabel.rectTransform, 245f, 48f, 650f, 58f); refs.finalLabel.alignment = TextAlignmentOptions.Center;
        refs.feedback = Text("Feedback", panel, font, "REQUISITOS DE ACCESO PENDIENTES", 18f,
            FontStyles.Normal, Secondary);
        Top(refs.feedback.rectTransform, 245f, 112f, 650f, 32f); refs.feedback.alignment = TextAlignmentOptions.Center;
        refs.enter = ClickSurface("EnterArkButton", panel);
    }

    private static void SectionTitle(Transform parent, TMP_FontAsset font, string value, float y)
    {
        LineTop(parent, "SectionLeft", 34f, y + 19f, 300f); LineTop(parent, "SectionRight", 746f, y + 19f, 300f);
        TMP_Text text = Text("SectionTitle", parent, font, value, 25f, FontStyles.Normal, Cyan);
        Top(text.rectTransform, 340f, y, 400f, 42f); text.alignment = TextAlignmentOptions.Center; text.characterSpacing = 1.4f;
    }

    private static Graphic DrawEchoDiagram(
        RectTransform parent,
        string name,
        Color color,
        out Graphic core)
    {
        Dimension1CommandCenterLineGraphic outer = Circle(name, parent, 43f, 56, 1.7f, color);
        Circle("MiddleRing", parent, 33f, 48, 1.25f, color);
        Circle("InnerRing", parent, 22f, 40, 1.15f, color);
        Circle("CoreRing", parent, 11f, 28, 1f, color);
        Segment(parent, "AxisHorizontal", new Vector2(-52f, 0f), new Vector2(52f, 0f), 1.15f, color);
        Segment(parent, "AxisVertical", new Vector2(0f, -52f), new Vector2(0f, 52f), 1.15f, color);
        for (int i = 0; i < 12; i++)
        {
            float angle = i * Mathf.PI * 2f / 12f;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Segment(parent, "Tick_" + i, direction * 43f, direction * (i % 3 == 0 ? 52f : 48f),
                i % 3 == 0 ? 1.8f : 1.15f, color);
        }
        for (int i = 0; i < 4; i++)
        {
            float angle = (45f + i * 90f) * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Segment(parent, "Spoke_" + i, direction * 23f, direction * 32f, 1.1f, color);
        }

        RectTransform coreRect = Rect("Core", parent);
        Stretch(coreRect);
        Dimension1CommandCenterPolygonGraphic disk =
            coreRect.gameObject.AddComponent<Dimension1CommandCenterPolygonGraphic>();
        var points = new List<Vector2>();
        for (int i = 0; i < 24; i++)
        {
            float angle = i * Mathf.PI * 2f / 24f;
            points.Add(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 7.5f);
        }
        disk.SetPolygon(points);
        disk.color = color;
        disk.raycastTarget = false;
        core = disk;
        return outer;
    }

    private static Dimension1CommandCenterLineGraphic Circle(
        string name,
        Transform parent,
        float radius,
        int segments,
        float thickness,
        Color color)
    {
        RectTransform rect = Rect(name, parent);
        Stretch(rect);
        Dimension1CommandCenterLineGraphic line =
            rect.gameObject.AddComponent<Dimension1CommandCenterLineGraphic>();
        var points = new List<Vector2>();
        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            points.Add(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius);
        }
        line.SetLine(points, thickness, true);
        line.color = color;
        line.raycastTarget = false;
        return line;
    }

    private static RectTransform Panel(string name, Transform parent, Sprite frame, Sprite fill,
        Vector2 position, Vector2 size, Color fillColor, Color borderColor,
        out Image border, out Image surface)
    {
        RectTransform root = Rect(name, parent); Top(root, position.x, position.y, size.x, size.y);
        surface = Image("Surface", root, fill, fillColor);
        Stretch(surface.rectTransform, new Vector2(7f, 7f), new Vector2(7f, 7f)); surface.type = UnityEngine.UI.Image.Type.Sliced;
        border = Image("Frame", root, frame, borderColor); Stretch(border.rectTransform); border.type = UnityEngine.UI.Image.Type.Sliced;
        return root;
    }

    private static Button ButtonPanel(string name, Transform parent, Sprite frame, Sprite fill,
        Vector2 position, Vector2 size, Color fillColor, Color borderColor,
        out Image border, out Image surface)
    {
        RectTransform root = Panel(name, parent, frame, fill, position, size, fillColor, borderColor, out border, out surface);
        surface.raycastTarget = true;
        Button button = root.gameObject.AddComponent<Button>(); button.targetGraphic = surface;
        button.transition = Selectable.Transition.ColorTint; return button;
    }

    private static Button ClickSurface(string name, Transform parent)
    {
        Image image = Image(name, parent, null, Color.clear); Stretch(image.rectTransform); image.raycastTarget = true;
        Button button = image.gameObject.AddComponent<Button>(); button.targetGraphic = image;
        button.transition = Selectable.Transition.None; return button;
    }

    private static RectTransform Rect(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        RectTransform rect = go.GetComponent<RectTransform>(); rect.SetParent(parent, false); return rect;
    }

    private static Image Image(string name, Transform parent, Sprite sprite, Color color)
    {
        RectTransform rect = Rect(name, parent); Image image = rect.gameObject.AddComponent<Image>();
        image.sprite = sprite; image.color = color; image.raycastTarget = false; return image;
    }

    private static TMP_Text Text(string name, Transform parent, TMP_FontAsset font, string value,
        float size, FontStyles style, Color color)
    {
        RectTransform rect = Rect(name, parent); TextMeshProUGUI text = rect.gameObject.AddComponent<TextMeshProUGUI>();
        text.font = font; text.text = value; text.fontSize = size; text.fontStyle = style; text.color = color;
        text.raycastTarget = false; text.textWrappingMode = TextWrappingModes.Normal;
        text.overflowMode = TextOverflowModes.Overflow; text.alignment = TextAlignmentOptions.Left; return text;
    }

    private static void Top(RectTransform rect, float x, float y, float width, float height)
    {
        rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f); rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(x, -y); rect.sizeDelta = new Vector2(width, height);
    }

    private static void Stretch(RectTransform rect, Vector2 insetMin = default, Vector2 insetMax = default)
    {
        rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.pivot = new Vector2(.5f, .5f);
        rect.offsetMin = insetMin; rect.offsetMax = -insetMax;
    }

    private static void Center(RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    private static void Segment(Transform parent, string name, Vector2 a, Vector2 b, float thickness, Color color)
    {
        Image line = Image(name, parent, null, color); RectTransform rect = line.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        Vector2 delta = b - a; rect.sizeDelta = new Vector2(delta.magnitude, thickness);
        rect.anchoredPosition = (a + b) * .5f;
        rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
    }

    private static void LineTop(Transform parent, string name, float x, float y, float width)
    {
        Image line = Image(name, parent, null, Hex("0D6C8D", 150)); Top(line.rectTransform, x, y, width, 2f);
    }

    private static void DrawMetalCluster(Transform parent, Vector2 center)
    {
        for (int i = 0; i < 7; i++)
        {
            float angle = i * Mathf.PI * 2f / 7f; Image dot = Image("MetalDot_" + i, parent, null, Cyan);
            RectTransform rect = dot.rectTransform; rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
            rect.sizeDelta = new Vector2(10f, 10f); rect.anchoredPosition = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 15f;
        }
    }

    private static void PrepareSprite(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            importer = AssetImporter.GetAtPath(path) as TextureImporter;
        }
        if (importer == null) return;
        importer.textureType = TextureImporterType.Sprite; importer.spriteImportMode = SpriteImportMode.Single;
        importer.mipmapEnabled = false; importer.alphaIsTransparency = true;
        importer.textureCompression = TextureImporterCompression.Uncompressed; importer.SaveAndReimport();
    }

    private static void ValidateInternal(Scene scene)
    {
        Dimension1PanelUI panel = FindSceneComponent<Dimension1PanelUI>(scene);
        Dimension1ArkVisualUI visual = FindSceneComponent<Dimension1ArkVisualUI>(scene);
        Transform ark = FindSceneTransform(scene, "ArkPanel");
        if (panel == null || visual == null || ark == null)
            throw new InvalidOperationException("La pantalla visual del ARK no quedó instalada.");
        SerializedObject serialized = new SerializedObject(panel);
        string[] fields = { "arkPanel", "closeArkPanelButton", "arkInfoText", "investigateArkButton",
            "startOuterSyncButton", "startDebrisSyncButton", "startAncientSyncButton",
            "startSilentSyncButton", "enterArkButton" };
        foreach (string field in fields)
        {
            SerializedProperty property = serialized.FindProperty(field);
            if (property == null || property.objectReferenceValue == null)
                throw new InvalidOperationException("Conexión del ARK faltante: " + field);
        }
        if (Dimension1System.D1CentralSyncMissionIds.Length != 4 ||
            Dimension1System.D1CentralSyncMissionDurationSeconds != 3600d ||
            Dimension1System.D1ArkFinalMissionDurationSeconds != 5400d)
            throw new InvalidOperationException("Las reglas temporales reales del ARK no coinciden.");

        Transform background = FindChild(ark, "Background");
        if (background == null || background.GetComponent<Image>() == null ||
            background.GetComponent<Image>().raycastTarget)
            throw new InvalidOperationException("El fondo decorativo del ARK no debe interceptar clics.");
        if (FindChild(ark, "SignalBadge") == null || FindChild(ark, "UnknownMark") == null)
            throw new InvalidOperationException("La señal investigada no conserva su insignia técnica.");
        for (int i = 0; i < 4; i++)
        {
            Transform mission = FindChild(ark, "Mission_" + i);
            Transform artwork = mission != null ? FindChild(mission, "Artwork") : null;
            Transform echo = FindChild(ark, "EchoDiagram_" + i);
            if (mission == null || artwork == null || echo == null ||
                FindChild(echo, "CoreRing") == null || FindChild(echo, "AxisHorizontal") == null)
                throw new InvalidOperationException("Composición visual incompleta en misión/eco " + (i + 1) + ".");
            RectTransform missionRect = mission.GetComponent<RectTransform>();
            if (Mathf.Abs(missionRect.sizeDelta.x - 244f) > .01f ||
                Mathf.Abs(missionRect.sizeDelta.y - 376f) > .01f)
                throw new InvalidOperationException("Las tarjetas de sincronía no conservan la proporción aprobada.");
        }
    }

    private static void Assign(SerializedObject target, string field, UnityEngine.Object value)
    {
        SerializedProperty property = target.FindProperty(field);
        if (property == null) throw new InvalidOperationException("No existe el campo: " + field);
        property.objectReferenceValue = value;
    }

    private static void AddPersistent(UnityEngine.Events.UnityEvent value,
        UnityEngine.Events.UnityAction action) => UnityEventTools.AddPersistentListener(value, action);
    private static Sprite LoadSprite(string path) => AssetDatabase.LoadAssetAtPath<Sprite>(path);
    private static bool HasNull(Sprite[] sprites)
    {
        foreach (Sprite sprite in sprites) if (sprite == null) return true; return false;
    }
    private static T FindSceneComponent<T>(Scene scene) where T : Component
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            T value = root.GetComponentInChildren<T>(true); if (value != null) return value;
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
        if (root == null) return null;
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            if (child.name == name) return child;
        return null;
    }
    private static Color Hex(string html, byte alpha = 255)
    {
        ColorUtility.TryParseHtmlString("#" + html, out Color color); color.a = alpha / 255f; return color;
    }
}
#endif

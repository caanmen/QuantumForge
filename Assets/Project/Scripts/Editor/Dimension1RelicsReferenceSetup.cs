#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Dimension1RelicsReferenceSetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ArtPath = "Assets/Project/UI/Dimension1/Generated";
    private const string RelicPath = ArtPath + "/Candidates/Relics";
    private const string RelicExpansionPath = ArtPath + "/Candidates/RelicsExpansion";
    private const string RootName = "D1_RelicsVisualRoot";
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
        public readonly List<TMP_Text> metalAmounts = new List<TMP_Text>();
        public readonly List<TMP_Text> metalRates = new List<TMP_Text>();
        public readonly List<Button> relicButtons = new List<Button>();
        public readonly List<RectTransform> relicCardRoots = new List<RectTransform>();
        public readonly List<Image> relicBorders = new List<Image>();
        public readonly List<Image> relicFills = new List<Image>();
        public readonly List<Image> relicSelectionGlows = new List<Image>();
        public readonly List<Graphic> relicTierFrames = new List<Graphic>();
        public readonly List<GameObject> relicReticles = new List<GameObject>();
        public readonly List<Image> relicArts = new List<Image>();
        public readonly List<TMP_Text> relicNames = new List<TMP_Text>();
        public readonly List<TMP_Text> relicTiers = new List<TMP_Text>();
        public readonly List<TMP_Text> relicStatuses = new List<TMP_Text>();
        public readonly List<TMP_Text> costNames = new List<TMP_Text>();
        public readonly List<TMP_Text> costRequired = new List<TMP_Text>();
        public readonly List<TMP_Text> costOwned = new List<TMP_Text>();
        public TMP_Text discoveredCounter;
        public Button previousPageButton;
        public Button nextPageButton;
        public TMP_Text previousPageGlyph;
        public TMP_Text nextPageGlyph;
        public TMP_Text pageIndicator;
        public Image detailArt;
        public TMP_Text detailName;
        public TMP_Text detailOrigin;
        public TMP_Text detailLevel;
        public TMP_Text nextMilestone;
        public TMP_Text effectPrimary;
        public TMP_Text effectSecondary;
        public Button upgradeButton;
        public TMP_Text upgradeButtonLabel;
    }

    [MenuItem("Quantum Forge/Dimension 1/Install Relics Reference Screen")]
    public static void Install()
    {
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Dimension1PanelUI panel = FindSceneComponent<Dimension1PanelUI>(scene);
        if (panel == null) throw new InvalidOperationException("No existe Dimension1PanelUI en Main.unity.");
        Transform relicPanel = FindChild(panel.transform, "RelicChamberPanel");
        if (relicPanel == null) throw new InvalidOperationException("No existe RelicChamberPanel en Main.unity.");

        TMP_FontAsset font = FindFont(panel.transform);
        Sprite frame = LoadSprite(ArtPath + "/d1_premium_frame_v4.png");
        Sprite fillSprite = LoadSprite(ArtPath + "/d1_panel_fill_v4.png");
        Sprite starfield = LoadSprite(ArtPath + "/d1_starfield.png");
        Sprite glow = LoadSprite(ArtPath + "/d1_glow_v3.png");
        Sprite[] relicSprites = LoadRelicSprites();
        Sprite[] navigationSprites = LoadNavigationSprites();
        if (font == null || frame == null || fillSprite == null || glow == null)
            throw new InvalidOperationException("Faltan recursos base para la pantalla de Reliquias.");
        if (relicSprites.Length != 20 || navigationSprites.Length != 5)
            throw new InvalidOperationException("Faltan ilustraciones de Reliquias o navegación.");

        Transform previous = FindDirectChild(relicPanel, RootName);
        if (previous != null) UnityEngine.Object.DestroyImmediate(previous.gameObject);
        for (int i = 0; i < relicPanel.childCount; i++) relicPanel.GetChild(i).gameObject.SetActive(false);

        RectTransform root = Rect(RootName, relicPanel);
        root.anchorMin = root.anchorMax = new Vector2(.5f, .5f);
        root.pivot = new Vector2(.5f, .5f);
        root.anchoredPosition = Dimension1SharedLayoutTokens.RootOffset;
        root.sizeDelta = new Vector2(W, H);
        root.SetAsLastSibling();
        root.gameObject.AddComponent<Dimension1VisualSkinRoot>();
        Canvas canvas = root.gameObject.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 32000;
        root.gameObject.AddComponent<GraphicRaycaster>();
        CanvasGroup canvasGroup = root.gameObject.AddComponent<CanvasGroup>();
        Dimension1RelicsVisualUI visual = root.gameObject.AddComponent<Dimension1RelicsVisualUI>();

        Image background = Image("Background", root, null, Void);
        Stretch(background.rectTransform, new Vector2(-28f, -34f), new Vector2(-28f, -34f));
        background.raycastTarget = true;
        Image stars = Image("Starfield", root, starfield, Hex("6DC9E6", 7));
        Stretch(stars.rectTransform);
        stars.raycastTarget = false;
        Image outer = Image("OuterFrame", root, frame, Hex("087FA9", 185));
        Top(outer.rectTransform, Dimension1SharedLayoutTokens.OuterFrameX,
            Dimension1SharedLayoutTokens.OuterFrameY,
            Dimension1SharedLayoutTokens.OuterFrameWidth,
            Dimension1SharedLayoutTokens.OuterFrameHeight);
        outer.type = UnityEngine.UI.Image.Type.Sliced;
        outer.raycastTarget = false;

        Refs refs = new Refs();
        BuildHeader(root, frame, fillSprite, font, visual, refs);
        BuildGallery(root, frame, fillSprite, glow, font, visual, refs, relicSprites);
        BuildDetail(root, frame, fillSprite, font, visual, refs, relicSprites[2]);
        BuildNavigation(root, frame, fillSprite, font, visual, navigationSprites);
        Dimension1SharedShellApply.ApplyToRoot(root);

        SerializedObject so = new SerializedObject(visual);
        Assign(so, "panel", panel);
        Assign(so, "commandCenter", FindSceneComponent<Dimension1CommandCenterUI>(scene));
        Assign(so, "canvasGroup", canvasGroup);
        SetObjectArray(so, "hideWhileOpen", FindNavigationRoots(scene));
        SetObjectArray(so, "metalAmounts", refs.metalAmounts.ToArray());
        SetObjectArray(so, "metalRates", refs.metalRates.ToArray());
        SetObjectArray(so, "relicButtons", refs.relicButtons.ToArray());
        SetObjectArray(so, "relicCardRoots", refs.relicCardRoots.ToArray());
        SetObjectArray(so, "relicBorders", refs.relicBorders.ToArray());
        SetObjectArray(so, "relicFills", refs.relicFills.ToArray());
        SetObjectArray(so, "relicSelectionGlows", refs.relicSelectionGlows.ToArray());
        SetObjectArray(so, "relicTierFrames", refs.relicTierFrames.ToArray());
        SetObjectArray(so, "relicReticles", refs.relicReticles.ToArray());
        SetObjectArray(so, "relicArts", refs.relicArts.ToArray());
        SetObjectArray(so, "relicNames", refs.relicNames.ToArray());
        SetObjectArray(so, "relicTiers", refs.relicTiers.ToArray());
        SetObjectArray(so, "relicStatuses", refs.relicStatuses.ToArray());
        SetObjectArray(so, "relicSprites", relicSprites);
        Assign(so, "discoveredCounter", refs.discoveredCounter);
        Assign(so, "previousPageButton", refs.previousPageButton);
        Assign(so, "nextPageButton", refs.nextPageButton);
        Assign(so, "previousPageGlyph", refs.previousPageGlyph);
        Assign(so, "nextPageGlyph", refs.nextPageGlyph);
        Assign(so, "pageIndicator", refs.pageIndicator);
        Assign(so, "detailArt", refs.detailArt);
        Assign(so, "detailName", refs.detailName);
        Assign(so, "detailOrigin", refs.detailOrigin);
        Assign(so, "detailLevel", refs.detailLevel);
        Assign(so, "nextMilestone", refs.nextMilestone);
        Assign(so, "effectPrimary", refs.effectPrimary);
        Assign(so, "effectSecondary", refs.effectSecondary);
        SetObjectArray(so, "costNames", refs.costNames.ToArray());
        SetObjectArray(so, "costRequired", refs.costRequired.ToArray());
        SetObjectArray(so, "costOwned", refs.costOwned.ToArray());
        Assign(so, "upgradeButton", refs.upgradeButton);
        Assign(so, "upgradeButtonLabel", refs.upgradeButtonLabel);
        so.ApplyModifiedPropertiesWithoutUndo();

        root.gameObject.SetActive(true);
        EditorUtility.SetDirty(visual);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("Unity no pudo guardar Main.unity.");
        ValidateInternal(scene);
        Debug.Log("[D1 Relics] INSTALL_PASS | referencia 552x1153 | destino 1080x1920 | 20 sprites sólidos | 3 páginas");
    }

    [MenuItem("Quantum Forge/Dimension 1/Validate Relics Reference Screen")]
    public static void Validate()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        ValidateInternal(scene);
        Debug.Log("[D1 Relics] VALIDATION_PASS");
    }

    private static void BuildHeader(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font,
        Dimension1RelicsVisualUI visual, Refs refs)
    {
        TMP_Text title = Text("DimensionTitle", root, font, "DIMENSIÓN 1", 42f, FontStyles.Bold, Primary);
        Top(title.rectTransform, 285f, 22f, 510f, 58f);
        title.alignment = TextAlignmentOptions.Center;
        title.characterSpacing = 4f;
        RectTransform decor = Rect("HeaderDecor", root);
        Top(decor, 0f, 18f, W, 68f);
        Line("HeaderLineL", decor, new Vector2(-415f, 2f), new Vector2(-220f, 2f), 1.5f, CyanMuted);
        Line("HeaderLineR", decor, new Vector2(220f, 2f), new Vector2(415f, 2f), 1.5f, CyanMuted);
        Dot(decor, new Vector2(-205f, 2f), 7f, CyanMuted);
        Dot(decor, new Vector2(205f, 2f), 7f, CyanMuted);

        Button command = ButtonPanel("CommandCenter", root, frame, fill,
            new Vector2(28f, 58f), new Vector2(168f, 76f), Fill, CyanMuted, out _, out _);
        DrawHomeIcon(command.transform, new Vector2(-58f, 0f), 23f, Secondary);
        TMP_Text home = Text("Label", command.transform, font, "CENTRO\nDE MANDO", 15f, FontStyles.Bold, Secondary);
        Top(home.rectTransform, 50f, 17f, 104f, 44f);
        home.alignment = TextAlignmentOptions.Center;
        AddPersistent(command.onClick, visual.OpenCommandCenter);

        string[] names = { "HIERRO", "ALUMINIO", "NÍQUEL" };
        float x = 208f;
        for (int i = 0; i < 3; i++)
        {
            RectTransform chip = Panel("Metal_" + i, root, frame, fill,
                new Vector2(x, 86f), new Vector2(188f, 100f), Fill, CyanMuted, out _, out _);
            DrawMetalIcon(chip, new Vector2(-66f, 0f), 27f, i, Secondary);
            TMP_Text name = Text("Name", chip, font, names[i], 15f, FontStyles.Bold, Secondary);
            Top(name.rectTransform, 51f, 12f, 124f, 23f);
            TMP_Text amount = Text("Amount", chip, font, "0", 27f, FontStyles.Normal, Primary);
            Top(amount.rectTransform, 50f, 32f, 124f, 39f);
            TMP_Text rate = Text("Rate", chip, font, "+0/s", 15f, FontStyles.Bold, Cyan);
            Top(rate.rectTransform, 105f, 67f, 70f, 23f);
            rate.alignment = TextAlignmentOptions.Right;
            refs.metalAmounts.Add(amount);
            refs.metalRates.Add(rate);
            x += 197f;
        }

        RectTransform metals = Panel("MetalsButton", root, frame, fill,
            new Vector2(799f, 86f), new Vector2(245f, 100f), Fill, CyanMuted, out _, out _);
        DrawMetalIcon(metals, new Vector2(-90f, 0f), 21f, 4, Cyan);
        TMP_Text metalLabel = Text("Label", metals, font, "10 METALES", 19f, FontStyles.Bold, Cyan);
        Top(metalLabel.rectTransform, 55f, 29f, 145f, 36f);
        metalLabel.alignment = TextAlignmentOptions.Center;
        Line("ChevronA", metals, new Vector2(92f, 4f), new Vector2(101f, -5f), 2f, Cyan);
        Line("ChevronB", metals, new Vector2(101f, -5f), new Vector2(110f, 4f), 2f, Cyan);
    }

    private static void BuildGallery(Transform root, Sprite frame, Sprite fill, Sprite glow,
        TMP_FontAsset font, Dimension1RelicsVisualUI visual, Refs refs, Sprite[] relicSprites)
    {
        Panel("GalleryShell", root, frame, fill, new Vector2(20f, 221f), new Vector2(1040f, 897f),
            Hex("020C12", 246), CyanMuted, out _, out _);
        TMP_Text heading = Text("MainHeading", root, font, "CÁMARA DE RELIQUIAS", 42f, FontStyles.Bold, Primary);
        Top(heading.rectTransform, 62f, 235f, 535f, 58f);
        heading.characterSpacing = 2f;
        refs.discoveredCounter = Text("DiscoveredCounter", root, font, "12 / 20 DESCUBIERTAS", 19f,
            FontStyles.Bold, Cyan);
        Top(refs.discoveredCounter.rectTransform, 790f, 247f, 225f, 36f);
        refs.discoveredCounter.alignment = TextAlignmentOptions.Right;

        refs.previousPageButton = ButtonPanel("PreviousPage", root, frame, fill,
            new Vector2(610f, 244f), new Vector2(38f, 38f), Fill, CyanMuted, out _, out _);
        refs.previousPageGlyph = Text("Glyph", refs.previousPageButton.transform, font, "‹", 28f,
            FontStyles.Bold, Secondary);
        Stretch(refs.previousPageGlyph.rectTransform);
        refs.previousPageGlyph.alignment = TextAlignmentOptions.Center;
        AddPersistent(refs.previousPageButton.onClick, visual.PreviousPage);

        refs.pageIndicator = Text("PageIndicator", root, font, "1 / 3", 18f, FontStyles.Bold, Cyan);
        Top(refs.pageIndicator.rectTransform, 650f, 250f, 80f, 28f);
        refs.pageIndicator.alignment = TextAlignmentOptions.Center;

        refs.nextPageButton = ButtonPanel("NextPage", root, frame, fill,
            new Vector2(732f, 244f), new Vector2(38f, 38f), Fill, CyanMuted, out _, out _);
        refs.nextPageGlyph = Text("Glyph", refs.nextPageButton.transform, font, "›", 28f,
            FontStyles.Bold, Cyan);
        Stretch(refs.nextPageGlyph.rectTransform);
        refs.nextPageGlyph.alignment = TextAlignmentOptions.Center;
        AddPersistent(refs.nextPageButton.onClick, visual.NextPage);
        Line("HeadingDivider", root, new Vector2(-493f, 652f), new Vector2(493f, 652f), 1f, Hex("087FA9", 120));

        string[] labels =
        {
            "BRÚJULA\nDE DERIVA", "TALADRO\nANTIGUO", "CRISTAL\nANALÍTICO", "REGISTRO DE\nNAVEGACIÓN\nPERDIDO",
            "CONTENEDOR\nMODULAR", "NÚCLEO DE\nPROSPECCIÓN", "ANTENA\nFRACTURADA", "SELLO DE\nEXTRACCIÓN"
        };
        int[] tiers = { 1, 1, 2, 2, 2, 1, 1, 2 };
        for (int i = 0; i < 8; i++)
        {
            int col = i % 4;
            int row = i / 4;
            bool selected = i == 2;
            float x = 38f + col * 252f;
            float y = 310f + row * 397f;
            Button card = ButtonPanel("RelicCard_" + i, root, frame, fill,
                new Vector2(x, y), new Vector2(236f, 378f), selected ? AmberFill : Fill,
                selected ? Amber : CyanMuted, out Image cardFill, out Image cardBorder);
            refs.relicButtons.Add(card);
            refs.relicCardRoots.Add(card.GetComponent<RectTransform>());
            refs.relicFills.Add(cardFill);
            refs.relicBorders.Add(cardBorder);
            Image selectionGlow = Image("SelectionGlow", card.transform, glow, Hex("F4A70B", 40));
            Stretch(selectionGlow.rectTransform, new Vector2(-12f, -10f), new Vector2(-12f, -10f));
            selectionGlow.raycastTarget = false;
            selectionGlow.transform.SetAsFirstSibling();
            selectionGlow.gameObject.SetActive(selected);
            refs.relicSelectionGlows.Add(selectionGlow);

            if (i == 2)
            {
                RectTransform reticle = Rect("CrystalReticle", card.transform);
                Stretch(reticle);
                Vector2 reticleCenter = new Vector2(0f, 78f);
                Circle(reticle, reticleCenter, 72f, 1.2f, Hex("18C8FF", 95));
                Circle(reticle, reticleCenter, 48f, 1f, Hex("087FA9", 90));
                Line("CrystalAxisH", reticle, reticleCenter + new Vector2(-92f, 0f),
                    reticleCenter + new Vector2(92f, 0f), 1f, Hex("18C8FF", 75));
                Line("CrystalAxisV", reticle, reticleCenter + new Vector2(0f, 88f),
                    reticleCenter + new Vector2(0f, -88f), 1f, Hex("18C8FF", 55));
                refs.relicReticles.Add(reticle.gameObject);
            }
            else refs.relicReticles.Add(null);

            Image art = Image("RelicArt", card.transform, relicSprites[i], Color.white);
            Top(art.rectTransform, 15f, 15f, 206f, 190f);
            art.preserveAspect = true;
            art.raycastTarget = false;
            refs.relicArts.Add(art);

            TMP_Text label = Text("RelicName", card.transform, font, labels[i], i == 3 ? 20f : 23f,
                FontStyles.Normal, Primary);
            Top(label.rectTransform, 12f, 205f, 212f, 92f);
            label.alignment = TextAlignmentOptions.Center;
            label.enableWordWrapping = true;
            label.overflowMode = TextOverflowModes.Overflow;
            refs.relicNames.Add(label);

            TMP_Text tier = Text("Tier", card.transform, font, "TIER " + tiers[i], 19f, FontStyles.Bold,
                selected ? Amber : Cyan);
            Top(tier.rectTransform, 62f, 284f, 112f, 30f);
            tier.alignment = TextAlignmentOptions.Center;
            refs.relicTiers.Add(tier);
            RectTransform tierBorder = Rect("TierBorder", card.transform);
            Top(tierBorder, 61f, 283f, 114f, 32f);
            refs.relicTierFrames.Add(LineLoop("TierFrame", tierBorder, new[]
            {
                new Vector2(-52f, 13f), new Vector2(52f, 13f), new Vector2(56f, 0f),
                new Vector2(52f, -13f), new Vector2(-52f, -13f), new Vector2(-56f, 0f)
            }, 1.2f, selected ? Amber : CyanMuted));

            TMP_Text status = Text("Status", card.transform, font, "DESCUBIERTA", 20f, FontStyles.Bold,
                selected ? Amber : Cyan);
            Top(status.rectTransform, 12f, 315f, 212f, 30f);
            status.alignment = TextAlignmentOptions.Center;
            refs.relicStatuses.Add(status);

            if (i == 0) AddPersistent(card.onClick, visual.SelectRelic0);
            else if (i == 1) AddPersistent(card.onClick, visual.SelectRelic1);
            else if (i == 2) AddPersistent(card.onClick, visual.SelectRelic2);
            else if (i == 3) AddPersistent(card.onClick, visual.SelectRelic3);
            else if (i == 4) AddPersistent(card.onClick, visual.SelectRelic4);
            else if (i == 5) AddPersistent(card.onClick, visual.SelectRelic5);
            else if (i == 6) AddPersistent(card.onClick, visual.SelectRelic6);
            else AddPersistent(card.onClick, visual.SelectRelic7);
        }
    }

    private static void BuildDetail(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font,
        Dimension1RelicsVisualUI visual, Refs refs, Sprite crystal)
    {
        RectTransform detail = Panel("RelicDetail", root, frame, fill,
            new Vector2(20f, 1132f), new Vector2(1040f, 585f), Hex("020C12", 252), CyanMuted, out _, out _);
        Line("DetailDivider", detail, new Vector2(-205f, 265f), new Vector2(-205f, -205f), 1.2f, Hex("087FA9", 130));

        refs.detailArt = Image("DetailArt", detail, crystal, Color.white);
        Top(refs.detailArt.rectTransform, 25f, 18f, 280f, 245f);
        refs.detailArt.preserveAspect = true;
        refs.detailArt.raycastTarget = false;
        DrawEllipse(detail, new Vector2(-360f, 145f), 125f, 35f, 1.2f, Hex("18C8FF", 100));
        Circle(detail, new Vector2(-360f, 145f), 88f, 1f, Hex("087FA9", 100));
        Line("DetailAxis", detail, new Vector2(-495f, 145f), new Vector2(-225f, 145f), 1f, Hex("18C8FF", 90));

        TMP_Text levelLabel = Text("LevelLabel", detail, font, "NIVEL", 22f, FontStyles.Normal, Amber);
        Top(levelLabel.rectTransform, 38f, 264f, 250f, 32f);
        levelLabel.alignment = TextAlignmentOptions.Center;
        refs.detailLevel = Text("DetailLevel", detail, font, "25 / 100", 35f, FontStyles.Bold, Amber);
        Top(refs.detailLevel.rectTransform, 35f, 298f, 256f, 48f);
        refs.detailLevel.alignment = TextAlignmentOptions.Center;
        Line("LevelRule", detail, new Vector2(-486f, -1f), new Vector2(-232f, -1f), 1f, Hex("087FA9", 90));
        TMP_Text nextLabel = Text("NextLabel", detail, font, "PRÓXIMO HITO:", 18f, FontStyles.Normal, Cyan);
        Top(nextLabel.rectTransform, 46f, 357f, 235f, 27f);
        nextLabel.alignment = TextAlignmentOptions.Center;
        refs.nextMilestone = Text("NextMilestone", detail, font, "NIVEL 50", 18f, FontStyles.Bold, Cyan);
        Top(refs.nextMilestone.rectTransform, 46f, 384f, 235f, 28f);
        refs.nextMilestone.alignment = TextAlignmentOptions.Center;
        TMP_Text progressLabel = Text("ProgressLabel", detail, font, "PROGRESO DE MEJORA", 18f, FontStyles.Normal, Cyan);
        Top(progressLabel.rectTransform, 30f, 469f, 282f, 30f);
        progressLabel.alignment = TextAlignmentOptions.Center;
        Line("MilestoneLine", detail, new Vector2(-479f, -225f), new Vector2(-240f, -225f), 5f, Hex("087FA9", 160));
        int[] milestones = { 25, 50, 75, 100 };
        for (int i = 0; i < 4; i++)
        {
            Vector2 c = new Vector2(-480f + i * 80f, -225f);
            Color color = i == 0 ? Amber : CyanMuted;
            Vector2[] hex = Hexagon(c, 23f);
            Polygon("MilestoneFill_" + i, detail, hex, i == 0 ? Hex("3A2905", 255) : Hex("061923", 255));
            LineLoop("Milestone_" + i, detail, hex, 2f, color);
            TMP_Text marker = Text("MilestoneValue_" + i, detail, font, milestones[i].ToString(), 16f, FontStyles.Bold, color);
            Centered(marker.rectTransform, c, new Vector2(48f, 30f));
            marker.alignment = TextAlignmentOptions.Center;
        }

        refs.detailName = Text("DetailName", detail, font, "CRISTAL ANALÍTICO", 34f, FontStyles.Bold, Primary);
        Top(refs.detailName.rectTransform, 350f, 24f, 640f, 45f);
        refs.detailOrigin = Text("DetailOrigin", detail, font, "ORIGEN: ANILLO DE RESTOS", 18f, FontStyles.Bold, Cyan);
        Top(refs.detailOrigin.rectTransform, 351f, 70f, 610f, 30f);

        BuildEffectBox(detail, font, 350f, 111f, "EffectPrimary", out refs.effectPrimary, true);
        BuildEffectBox(detail, font, 350f, 207f, "EffectSecondary", out refs.effectSecondary, false);

        TMP_Text costTitle = Text("CostTitle", detail, font, "COSTO DE MEJORA", 18f, FontStyles.Normal, Cyan);
        Top(costTitle.rectTransform, 675f, 323f, 300f, 28f);
        costTitle.alignment = TextAlignmentOptions.Center;
        string[] costNames = { "HIERRO", "ALUMINIO", "NÍQUEL" };
        string[] required = { "250K", "120K", "60K" };
        string[] owned = { "5.98M", "4.73M", "4.70M" };
        for (int i = 0; i < 3; i++)
        {
            RectTransform cost = Panel("Cost_" + i, detail, frame, fill,
                new Vector2(505f + i * 154f, 356f), new Vector2(148f, 100f), FillRaised, CyanMuted, out _, out _);
            DrawMetalIcon(cost, new Vector2(-52f, 4f), 20f, i, Secondary);
            TMP_Text name = Text("Name", cost, font, costNames[i], 13f, FontStyles.Bold, Secondary);
            Top(name.rectTransform, 52f, 12f, 88f, 22f);
            TMP_Text amount = Text("Required", cost, font, required[i], 20f, FontStyles.Normal, Primary);
            Top(amount.rectTransform, 52f, 34f, 88f, 28f);
            TMP_Text have = Text("Owned", cost, font, owned[i], 14f, FontStyles.Normal, Secondary);
            Top(have.rectTransform, 52f, 66f, 88f, 22f);
            refs.costNames.Add(name);
            refs.costRequired.Add(amount);
            refs.costOwned.Add(have);
        }

        refs.upgradeButton = ButtonPanel("UpgradeButton", detail, frame, fill,
            new Vector2(507f, 470f), new Vector2(455f, 88f), AmberFill, Amber, out _, out _);
        refs.upgradeButtonLabel = Text("Label", refs.upgradeButton.transform, font, "MEJORAR RELIQUIA",
            27f, FontStyles.Bold, Amber);
        Stretch(refs.upgradeButtonLabel.rectTransform, new Vector2(18f, 12f), new Vector2(18f, 12f));
        refs.upgradeButtonLabel.alignment = TextAlignmentOptions.Center;
        refs.upgradeButtonLabel.characterSpacing = 2f;
        AddPersistent(refs.upgradeButton.onClick, visual.UpgradeSelected);
    }

    private static void BuildEffectBox(Transform parent, TMP_FontAsset font, float x, float y,
        string name, out TMP_Text content, bool targetIcon)
    {
        RectTransform box = Rect(name, parent);
        Top(box, x, y, 625f, 84f);
        Image bg = Image("Fill", box, null, Hex("03131D", 220));
        Stretch(bg.rectTransform);
        bg.raycastTarget = false;
        RectTransform border = Rect("Border", box);
        Stretch(border);
        LineLoop("Outline", border, new[]
        {
            new Vector2(-307f, 40f), new Vector2(307f, 40f), new Vector2(311f, 31f),
            new Vector2(311f, -31f), new Vector2(307f, -40f), new Vector2(-307f, -40f),
            new Vector2(-311f, -31f), new Vector2(-311f, 31f)
        }, 1f, Hex("087FA9", 130));
        if (targetIcon)
        {
            Circle(box, new Vector2(-270f, 0f), 22f, 2f, Cyan);
            Circle(box, new Vector2(-270f, 0f), 7f, 2f, Cyan);
            Line("CrossV", box, new Vector2(-270f, 31f), new Vector2(-270f, -31f), 1f, Cyan);
            Line("CrossH", box, new Vector2(-301f, 0f), new Vector2(-239f, 0f), 1f, Cyan);
        }
        else
        {
            Circle(box, new Vector2(-270f, 3f), 19f, 2f, Cyan);
            Line("Handle", box, new Vector2(-256f, -11f), new Vector2(-238f, -29f), 4f, Cyan);
        }
        content = Text("Text", box, font,
            targetIcon ? "Fragmentos y matrices con\nSonda Analítica: <color=#F4A70B>+0.5 pp</color>"
                       : "Reliquias en destinos de\ninvestigación: <color=#F4A70B>+0.2 pp</color>",
            20f, FontStyles.Normal, Primary);
        Top(content.rectTransform, 82f, 14f, 520f, 58f);
        content.enableWordWrapping = true;
        content.overflowMode = TextOverflowModes.Overflow;
    }

    private static void BuildNavigation(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font,
        Dimension1RelicsVisualUI visual, Sprite[] icons)
    {
        RectTransform nav = Rect("BottomNavigation", root);
        Top(nav, Dimension1SharedLayoutTokens.NavigationX,
            Dimension1SharedLayoutTokens.NavigationY,
            Dimension1SharedLayoutTokens.NavigationWidth,
            Dimension1SharedLayoutTokens.NavigationHeight);
        string[] labels = { "GALAXIA", "EXPLORAR", "HANGAR", "RELIQUIAS", "ÁRBOL" };
        for (int i = 0; i < 5; i++)
        {
            bool selected = i == 3;
            Button button = ButtonPanel("Nav_" + labels[i], nav, frame, fill,
                new Vector2(Dimension1SharedLayoutTokens.NavigationCardX(i),
                    Dimension1SharedLayoutTokens.NavigationCardY),
                new Vector2(Dimension1SharedLayoutTokens.NavigationCardWidth,
                    Dimension1SharedLayoutTokens.NavigationCardHeight),
                selected ? AmberFill : Fill, selected ? Amber : CyanMuted, out _, out _);
            Image icon = Image("Icon", button.transform, icons[i], selected ? Amber : Cyan);
            Top(icon.rectTransform, 50f, 15f, 98f, 92f);
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            TMP_Text label = Text("Label", button.transform, font, labels[i], 21f, FontStyles.Bold,
                selected ? Amber : Cyan);
            Top(label.rectTransform, 10f, 111f, 178f, 34f);
            label.alignment = TextAlignmentOptions.Center;
            if (selected) button.interactable = false;
            else if (i == 0) AddPersistent(button.onClick, visual.OpenGalaxy);
            else if (i == 1) AddPersistent(button.onClick, visual.OpenExplore);
            else if (i == 2) AddPersistent(button.onClick, visual.OpenHangar);
            else if (i == 4) AddPersistent(button.onClick, visual.OpenTree);
        }
    }

    private static void DrawHomeIcon(Transform parent, Vector2 c, float s, Color color)
    {
        Vector2[] roof =
        {
            c + new Vector2(-s, -s*.05f), c + new Vector2(0, s*.85f), c + new Vector2(s, -s*.05f)
        };
        LineGraphic("Roof", parent, roof, 2f, false, color);
        Line("WallL", parent, c + new Vector2(-s*.72f,-s*.02f), c + new Vector2(-s*.72f,-s*.88f), 2f, color);
        Line("WallR", parent, c + new Vector2(s*.72f,-s*.02f), c + new Vector2(s*.72f,-s*.88f), 2f, color);
        Line("Floor", parent, c + new Vector2(-s*.72f,-s*.88f), c + new Vector2(s*.72f,-s*.88f), 2f, color);
        Line("DoorL", parent, c + new Vector2(-s*.18f,-s*.88f), c + new Vector2(-s*.18f,-s*.30f), 1.5f, color);
        Line("DoorR", parent, c + new Vector2(s*.18f,-s*.88f), c + new Vector2(s*.18f,-s*.30f), 1.5f, color);
    }

    private static void DrawMetalIcon(Transform parent, Vector2 c, float s, int variant, Color color)
    {
        if (variant == 4)
        {
            for (int i = 0; i < 6; i++)
            {
                float a = Mathf.PI * 2f * i / 6f;
                Circle(parent, c + new Vector2(Mathf.Cos(a), Mathf.Sin(a))*s*.58f, s*.20f, 2.3f, color);
            }
            Circle(parent, c, s*.18f, 2f, color);
            return;
        }
        if (variant == 1)
        {
            Vector2[] ingot =
            {
                c + new Vector2(-s*.62f,s*.72f), c + new Vector2(s*.55f,s),
                c + new Vector2(s*.88f,-s*.58f), c + new Vector2(-s*.42f,-s)
            };
            Polygon("IngotFill", parent, ingot, WithAlpha(color, 70));
            LineLoop("Ingot", parent, ingot, 1.8f, color);
            Line("IngotFacet", parent, ingot[0], ingot[2], 1f, color);
            return;
        }
        DrawShard(parent, c + new Vector2(-s*.32f,s*.12f), s*.67f, color);
        DrawShard(parent, c + new Vector2(s*.32f,s*.05f), s*.57f, color);
        DrawShard(parent, c + new Vector2(0,-s*.34f), s*.48f, color);
    }

    private static void DrawShard(Transform parent, Vector2 c, float s, Color color)
    {
        Vector2[] points =
        {
            c + new Vector2(0,s), c + new Vector2(s*.55f,s*.10f), c + new Vector2(s*.25f,-s),
            c + new Vector2(-s*.52f,-s*.48f), c + new Vector2(-s*.44f,s*.18f)
        };
        Polygon("ShardFill", parent, points, WithAlpha(color, 65));
        LineLoop("Shard", parent, points, 1.5f, color);
        Line("ShardFacet", parent, points[0], points[2], 1f, color);
    }

    private static RectTransform Panel(string name, Transform parent, Sprite frame, Sprite fill,
        Vector2 topLeft, Vector2 size, Color fillColor, Color accent, out Image fillImage, out Image borderImage)
    {
        RectTransform result = Rect(name, parent);
        Top(result, topLeft.x, topLeft.y, size.x, size.y);
        Image shadow = Image("Shadow", result, fill, Hex("000000", 120));
        Stretch(shadow.rectTransform, new Vector2(5f,-5f), new Vector2(5f,-5f));
        shadow.type = UnityEngine.UI.Image.Type.Sliced;
        shadow.raycastTarget = false;
        fillImage = Image("Fill", result, fill, fillColor);
        Stretch(fillImage.rectTransform);
        fillImage.type = UnityEngine.UI.Image.Type.Sliced;
        fillImage.raycastTarget = false;
        borderImage = Image("Border", result, frame, WithAlpha(accent, 205));
        Stretch(borderImage.rectTransform);
        borderImage.type = UnityEngine.UI.Image.Type.Sliced;
        borderImage.raycastTarget = false;
        return result;
    }

    private static Button ButtonPanel(string name, Transform parent, Sprite frame, Sprite fill,
        Vector2 topLeft, Vector2 size, Color fillColor, Color accent, out Image fillImage, out Image borderImage)
    {
        RectTransform result = Panel(name,parent,frame,fill,topLeft,size,fillColor,accent,out fillImage,out borderImage);
        Image hit = result.gameObject.AddComponent<Image>();
        hit.color = new Color(1f,1f,1f,.001f);
        Button button = result.gameObject.AddComponent<Button>();
        button.targetGraphic = hit;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(.78f,.95f,1f,1f);
        colors.pressedColor = new Color(.58f,.84f,1f,1f);
        colors.disabledColor = new Color(.48f,.52f,.55f,.72f);
        colors.fadeDuration = .08f;
        button.colors = colors;
        return button;
    }

    private static void Circle(Transform parent, Vector2 center, float radius, float width, Color color)
    {
        var points = new List<Vector2>();
        for (int i = 0; i < 72; i++)
        {
            float a = Mathf.PI*2f*i/72f;
            points.Add(center + new Vector2(Mathf.Cos(a),Mathf.Sin(a))*radius);
        }
        LineGraphic("Circle",parent,points,width,true,color);
    }

    private static void DrawEllipse(Transform parent, Vector2 center, float rx, float ry, float width, Color color)
    {
        var points = new List<Vector2>();
        for (int i = 0; i < 64; i++)
        {
            float a = Mathf.PI*2f*i/64f;
            points.Add(center + new Vector2(Mathf.Cos(a)*rx,Mathf.Sin(a)*ry));
        }
        LineGraphic("Ellipse",parent,points,width,true,color);
    }

    private static void Dot(Transform parent, Vector2 center, float diameter, Color color)
    {
        var points = new List<Vector2>();
        for (int i = 0; i < 28; i++)
        {
            float a = Mathf.PI*2f*i/28f;
            points.Add(center + new Vector2(Mathf.Cos(a),Mathf.Sin(a))*diameter*.5f);
        }
        Polygon("Dot",parent,points,color);
    }

    private static void Line(string name, Transform parent, Vector2 a, Vector2 b, float width, Color color)
        => LineGraphic(name,parent,new[] { a,b },width,false,color);

    private static Dimension1CommandCenterLineGraphic LineLoop(string name, Transform parent, IList<Vector2> points, float width, Color color)
        => LineGraphic(name,parent,points,width,true,color);

    private static Dimension1CommandCenterLineGraphic LineGraphic(string name, Transform parent, IList<Vector2> points, float width, bool closed, Color color)
    {
        GameObject go = new GameObject(name,typeof(RectTransform));
        go.transform.SetParent(parent,false);
        Stretch((RectTransform)go.transform);
        Dimension1CommandCenterLineGraphic line = go.AddComponent<Dimension1CommandCenterLineGraphic>();
        line.color = color;
        line.raycastTarget = false;
        line.SetLine(points,width,closed);
        return line;
    }

    private static void Polygon(string name, Transform parent, IList<Vector2> points, Color color)
    {
        GameObject go = new GameObject(name,typeof(RectTransform));
        go.transform.SetParent(parent,false);
        Stretch((RectTransform)go.transform);
        Dimension1CommandCenterPolygonGraphic polygon = go.AddComponent<Dimension1CommandCenterPolygonGraphic>();
        polygon.color = color;
        polygon.raycastTarget = false;
        polygon.SetPolygon(points);
    }

    private static Vector2[] Hexagon(Vector2 c, float s)
    {
        var result = new Vector2[6];
        for (int i = 0; i < 6; i++)
        {
            float a = Mathf.PI/3f*i + Mathf.PI/6f;
            result[i] = c + new Vector2(Mathf.Cos(a),Mathf.Sin(a))*s;
        }
        return result;
    }

    private static RectTransform Rect(string name, Transform parent)
    {
        GameObject go = new GameObject(name,typeof(RectTransform));
        go.transform.SetParent(parent,false);
        return (RectTransform)go.transform;
    }

    private static Image Image(string name, Transform parent, Sprite sprite, Color color)
    {
        RectTransform rect = Rect(name,parent);
        Image image = rect.gameObject.AddComponent<Image>();
        image.sprite = sprite;
        image.color = color;
        return image;
    }

    private static TMP_Text Text(string name, Transform parent, TMP_FontAsset font, string value,
        float size, FontStyles style, Color color)
    {
        RectTransform rect = Rect(name,parent);
        TextMeshProUGUI text = rect.gameObject.AddComponent<TextMeshProUGUI>();
        text.font = font;
        text.text = value;
        text.fontSize = size;
        text.fontStyle = style;
        text.color = color;
        text.enableWordWrapping = false;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.raycastTarget = false;
        return text;
    }

    private static void Top(RectTransform rect, float x, float y, float width, float height)
    {
        rect.anchorMin = rect.anchorMax = new Vector2(0f,1f);
        rect.pivot = new Vector2(0f,1f);
        rect.anchoredPosition = new Vector2(x,-y);
        rect.sizeDelta = new Vector2(width,height);
    }

    private static void Centered(RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchorMin = rect.anchorMax = new Vector2(.5f,.5f);
        rect.pivot = new Vector2(.5f,.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    private static void Stretch(RectTransform rect, Vector2 min = default, Vector2 max = default)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(.5f,.5f);
        rect.offsetMin = min;
        rect.offsetMax = new Vector2(-max.x,-max.y);
    }

    private static TMP_FontAsset FindFont(Transform root)
    {
        foreach (TMP_Text text in root.GetComponentsInChildren<TMP_Text>(true))
            if (text != null && text.font != null && text.font.name.ToLowerInvariant().Contains("rajdhani")) return text.font;
        foreach (TMP_Text text in root.GetComponentsInChildren<TMP_Text>(true))
            if (text != null && text.font != null) return text.font;
        return TMP_Settings.defaultFontAsset;
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
        var sprites = new Sprite[paths.Length];
        for (int i = 0; i < paths.Length; i++)
        {
            string path = paths[i];
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) throw new InvalidOperationException("No se pudo importar reliquia: " + path);
            bool changed = importer.textureType != TextureImporterType.Sprite ||
                           importer.spriteImportMode != SpriteImportMode.Single || importer.mipmapEnabled ||
                           importer.textureCompression != TextureImporterCompression.Uncompressed ||
                           !importer.alphaIsTransparency || importer.maxTextureSize < 2048;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.sRGBTexture = true;
            importer.alphaIsTransparency = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = 2048;
            if (changed) importer.SaveAndReimport();
            sprites[i] = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprites[i] == null) throw new InvalidOperationException("Reliquia no disponible como Sprite: " + path);
        }
        return sprites;
    }

    private static Sprite[] LoadNavigationSprites()
    {
        string[] paths =
        {
            ArtPath + "/d1_nav_galaxy_v3.png", ArtPath + "/d1_nav_explore_v3.png",
            ArtPath + "/d1_nav_hangar_v3.png", ArtPath + "/d1_nav_relics_v3.png",
            ArtPath + "/d1_nav_tree_v3.png"
        };
        var result = new Sprite[paths.Length];
        for (int i = 0; i < paths.Length; i++) result[i] = LoadSprite(paths[i]);
        return result;
    }

    private static Sprite LoadSprite(string path) => AssetDatabase.LoadAssetAtPath<Sprite>(path);

    private static T FindSceneComponent<T>(Scene scene) where T : Component
    {
        foreach (GameObject sceneRoot in scene.GetRootGameObjects())
        {
            T result = sceneRoot.GetComponentInChildren<T>(true);
            if (result != null) return result;
        }
        return null;
    }

    private static Transform FindDirectChild(Transform parent, string name)
    {
        for (int i = 0; i < parent.childCount; i++) if (parent.GetChild(i).name == name) return parent.GetChild(i);
        return null;
    }

    private static Transform FindChild(Transform parent, string name)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true)) if (child.name == name) return child;
        return null;
    }

    private static GameObject[] FindNavigationRoots(Scene scene)
    {
        var result = new List<GameObject>();
        string[] names = { "PrimaryNavigationSlot", "SecondaryNavigationSlot", "MachineContextTabs" };
        foreach (string name in names)
        {
            Transform target = null;
            foreach (GameObject sceneRoot in scene.GetRootGameObjects())
            {
                target = FindChild(sceneRoot.transform,name);
                if (target != null) break;
            }
            if (target != null) result.Add(target.gameObject);
        }
        return result.ToArray();
    }

    private static void Assign(SerializedObject so, string propertyName, UnityEngine.Object value)
    {
        SerializedProperty property = so.FindProperty(propertyName);
        if (property == null) throw new InvalidOperationException("No existe propiedad serializada: " + propertyName);
        property.objectReferenceValue = value;
    }

    private static void SetObjectArray<T>(SerializedObject so, string propertyName, T[] values) where T : UnityEngine.Object
    {
        SerializedProperty property = so.FindProperty(propertyName);
        if (property == null) throw new InvalidOperationException("No existe arreglo serializado: " + propertyName);
        property.arraySize = values == null ? 0 : values.Length;
        for (int i = 0; values != null && i < values.Length; i++)
            property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
    }

    private static void AddPersistent(UnityEvent evt, UnityAction action) => UnityEventTools.AddPersistentListener(evt,action);

    private static void ValidateInternal(Scene scene)
    {
        Transform root = null;
        foreach (GameObject sceneRoot in scene.GetRootGameObjects())
        {
            root = FindChild(sceneRoot.transform,RootName);
            if (root != null) break;
        }
        if (root == null) throw new InvalidOperationException("Falta " + RootName + ".");
        if (root.GetComponent<Dimension1RelicsVisualUI>() == null) throw new InvalidOperationException("Falta controlador visual de Reliquias.");
        if (root.GetComponent<Canvas>() == null || root.GetComponent<GraphicRaycaster>() == null)
            throw new InvalidOperationException("Falta overlay interactivo de Reliquias.");
        string[] required =
        {
            "DimensionTitle", "MainHeading", "RelicCard_0", "RelicCard_7", "RelicDetail",
            "DetailArt", "UpgradeButton", "BottomNavigation", "PreviousPage", "NextPage", "PageIndicator"
        };
        foreach (string name in required)
            if (FindChild(root,name) == null) throw new InvalidOperationException("Falta bloque visual: " + name);
        if (FindChild(root,"Nav_RELIQUIAS")?.GetComponent<Button>() == null)
            throw new InvalidOperationException("Falta navegación Reliquias.");
        Dimension1RelicsVisualUI visual = root.GetComponent<Dimension1RelicsVisualUI>();
        SerializedObject serialized = new SerializedObject(visual);
        if (serialized.FindProperty("relicSprites")?.arraySize != 20)
            throw new InvalidOperationException("La Cámara de Reliquias no tiene sus 20 sprites.");
        if (serialized.FindProperty("relicCardRoots")?.arraySize != 8)
            throw new InvalidOperationException("La galería no tiene sus ocho tarjetas reutilizables.");
    }

    private static Color WithAlpha(Color color, byte alpha)
    {
        color.a = alpha/255f;
        return color;
    }

    private static Color Hex(string value, byte alpha = 255)
    {
        ColorUtility.TryParseHtmlString("#" + value,out Color color);
        color.a = alpha/255f;
        return color;
    }
}
#endif

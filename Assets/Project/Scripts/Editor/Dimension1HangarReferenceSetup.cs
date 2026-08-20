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

public static class Dimension1HangarReferenceSetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ArtPath = "Assets/Project/UI/Dimension1/Generated";
    private const string BlueprintMaterialPath = ArtPath + "/d1_hangar_blueprint_keyed.mat";
    private const string RootName = "D1_HangarVisualRoot";
    private const float W = 1080f;
    private const float H = 1920f;

    private static readonly Color Void = Hex("01090E");
    private static readonly Color Fill = Hex("04121B", 248);
    private static readonly Color FillRaised = Hex("071924", 252);
    private static readonly Color Cyan = Hex("18C8FF");
    private static readonly Color CyanBright = Hex("8DEAFF");
    private static readonly Color CyanMuted = Hex("087FA9");
    private static readonly Color Primary = Hex("EDF4F7");
    private static readonly Color Secondary = Hex("9EABB4");
    private static readonly Color Amber = Hex("F4A70B");
    private static readonly Color AmberFill = Hex("1A1508", 252);

    private sealed class Refs
    {
        public readonly List<TMP_Text> metalAmounts = new List<TMP_Text>();
        public readonly List<TMP_Text> metalRates = new List<TMP_Text>();
        public readonly List<Button> shipButtons = new List<Button>();
        public readonly List<Image> shipBorders = new List<Image>();
        public readonly List<Image> shipFills = new List<Image>();
        public readonly List<GameObject> shipArtRoots = new List<GameObject>();
        public readonly List<GameObject> largeShipArtRoots = new List<GameObject>();
        public readonly List<TMP_Text> shipLabels = new List<TMP_Text>();
        public readonly List<Button> partButtons = new List<Button>();
        public readonly List<Image> partBorders = new List<Image>();
        public readonly List<Image> partFills = new List<Image>();
        public readonly List<GameObject> partIconRoots = new List<GameObject>();
        public readonly List<TMP_Text> partLabels = new List<TMP_Text>();
        public readonly List<TMP_Text> partLevels = new List<TMP_Text>();
        public readonly List<TMP_Text> partValues = new List<TMP_Text>();
        public readonly List<Image> partBars = new List<Image>();
        public readonly List<TMP_Text> costNames = new List<TMP_Text>();
        public readonly List<TMP_Text> costValues = new List<TMP_Text>();
        public TMP_Text upgradeButtonLabel;
        public Button upgradeButton;
        public TMP_Text missionBonus;
    }

    [MenuItem("Quantum Forge/Dimension 1/Install Hangar Reference Screen")]
    public static void Install()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Dimension1PanelUI panel = FindSceneComponent<Dimension1PanelUI>(scene);
        if (panel == null) throw new InvalidOperationException("No existe Dimension1PanelUI en Main.unity.");
        Transform hangarPanel = FindChild(panel.transform, "HangarPanel");
        if (hangarPanel == null) throw new InvalidOperationException("No existe HangarPanel en Main.unity.");

        TMP_FontAsset font = FindFont(panel.transform);
        Sprite frame = LoadSprite(ArtPath + "/d1_premium_frame_v4.png");
        Sprite fillSprite = LoadSprite(ArtPath + "/d1_panel_fill_v4.png");
        Sprite starfield = LoadSprite(ArtPath + "/d1_starfield.png");
        Sprite glow = LoadSprite(ArtPath + "/d1_glow_v3.png");
        Sprite[] vehicleSprites = LoadVehicleSprites();
        Material blueprintMaterial = LoadOrCreateBlueprintMaterial();
        if (font == null || frame == null || fillSprite == null || glow == null)
            throw new InvalidOperationException("Faltan recursos base para la pantalla Hangar.");
        if (blueprintMaterial == null || vehicleSprites.Length != 4)
            throw new InvalidOperationException("Faltan las ilustraciones blueprint de las naves del Hangar.");

        Transform previous = FindDirectChild(hangarPanel, RootName);
        if (previous != null) UnityEngine.Object.DestroyImmediate(previous.gameObject);
        for (int i = 0; i < hangarPanel.childCount; i++)
            hangarPanel.GetChild(i).gameObject.SetActive(false);

        RectTransform root = Rect(RootName, hangarPanel);
        root.anchorMin = root.anchorMax = new Vector2(.5f, .5f);
        root.pivot = new Vector2(.5f, .5f);
        // HangarPanel conserva un desplazamiento vertical heredado de la interfaz
        // provisional. Esta compensación alinea el lienzo visual con el viewport real.
        root.anchoredPosition = Dimension1SharedLayoutTokens.RootOffset;
        root.sizeDelta = new Vector2(W, H);
        root.SetAsLastSibling();
        root.gameObject.AddComponent<Dimension1VisualSkinRoot>();
        Canvas canvas = root.gameObject.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 32000;
        root.gameObject.AddComponent<GraphicRaycaster>();
        CanvasGroup canvasGroup = root.gameObject.AddComponent<CanvasGroup>();
        Dimension1HangarVisualUI visual = root.gameObject.AddComponent<Dimension1HangarVisualUI>();

        Image background = Image("Background", root, null, Void);
        Stretch(background.rectTransform, new Vector2(-28f, -34f), new Vector2(-28f, -34f));
        background.raycastTarget = true;
        Image stars = Image("Starfield", root, starfield, Hex("6DC9E6", 6));
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
        BuildHangarShell(root, frame, fillSprite, glow, font, visual, refs, vehicleSprites, blueprintMaterial);
        BuildNavigation(root, frame, fillSprite, font, visual);
        Dimension1SharedShellApply.ApplyToRoot(root);

        GameObject[] navRoots = FindNavigationRoots(scene);
        SerializedObject so = new SerializedObject(visual);
        Assign(so, "panel", panel);
        Assign(so, "commandCenter", FindSceneComponent<Dimension1CommandCenterUI>(scene));
        Assign(so, "canvasGroup", canvasGroup);
        SetObjectArray(so, "hideWhileOpen", navRoots);
        SetObjectArray(so, "metalAmounts", refs.metalAmounts.ToArray());
        SetObjectArray(so, "metalRates", refs.metalRates.ToArray());
        SetObjectArray(so, "shipButtons", refs.shipButtons.ToArray());
        SetObjectArray(so, "shipBorders", refs.shipBorders.ToArray());
        SetObjectArray(so, "shipFills", refs.shipFills.ToArray());
        SetObjectArray(so, "shipArtRoots", refs.shipArtRoots.ToArray());
        SetObjectArray(so, "largeShipArtRoots", refs.largeShipArtRoots.ToArray());
        SetObjectArray(so, "shipLabels", refs.shipLabels.ToArray());
        SetObjectArray(so, "partButtons", refs.partButtons.ToArray());
        SetObjectArray(so, "partBorders", refs.partBorders.ToArray());
        SetObjectArray(so, "partFills", refs.partFills.ToArray());
        SetObjectArray(so, "partIconRoots", refs.partIconRoots.ToArray());
        SetObjectArray(so, "partLabels", refs.partLabels.ToArray());
        SetObjectArray(so, "partLevels", refs.partLevels.ToArray());
        SetObjectArray(so, "partValues", refs.partValues.ToArray());
        SetObjectArray(so, "partBars", refs.partBars.ToArray());
        SetObjectArray(so, "costNames", refs.costNames.ToArray());
        SetObjectArray(so, "costValues", refs.costValues.ToArray());
        Assign(so, "upgradeButtonLabel", refs.upgradeButtonLabel);
        Assign(so, "upgradeButton", refs.upgradeButton);
        Assign(so, "missionBonus", refs.missionBonus);
        so.ApplyModifiedPropertiesWithoutUndo();

        root.gameObject.SetActive(true);
        EditorUtility.SetDirty(visual);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("Unity no pudo guardar Main.unity.");
        ValidateInternal(scene);
        Debug.Log("[D1 Hangar] INSTALL_PASS | referencia 665x1160 | destino 1080x1920 | datos y navegación conectados");
    }

    [MenuItem("Quantum Forge/Dimension 1/Validate Hangar Reference Screen")]
    public static void Validate()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        ValidateInternal(scene);
        Debug.Log("[D1 Hangar] VALIDATION_PASS");
    }

    private static void BuildHeader(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font,
        Dimension1HangarVisualUI visual, Refs refs)
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

    private static void BuildHangarShell(Transform root, Sprite frame, Sprite fill, Sprite glow,
        TMP_FontAsset font, Dimension1HangarVisualUI visual, Refs refs, Sprite[] vehicleSprites,
        Material blueprintMaterial)
    {
        RectTransform shell = Panel("HangarShell", root, frame, fill,
            new Vector2(22f, 247f), new Vector2(1036f, 1444f), Hex("020C12", 252), CyanMuted, out _, out _);
        TMP_Text title = Text("MainHeading", root, font, "HANGAR", 48f, FontStyles.Bold, Hex("48BDE8"));
        Top(title.rectTransform, 68f, 269f, 340f, 66f);
        title.alignment = TextAlignmentOptions.Left;
        title.characterSpacing = 5f;
        TMP_Text subtitle = Text("FleetSubtitle", root, font, "FLOTA ACTIVA", 29f, FontStyles.Normal, Hex("48BDE8"));
        Top(subtitle.rectTransform, 70f, 326f, 300f, 42f);
        subtitle.characterSpacing = 4f;
        Line("TitleAccent", shell, new Vector2(388f, 630f), new Vector2(491f, 630f), 1.2f, Hex("087FA9", 120));

        BuildShipCards(root, frame, fill, glow, font, visual, refs, vehicleSprites, blueprintMaterial);
        BuildDetails(root, frame, fill, glow, font, visual, refs, vehicleSprites, blueprintMaterial);
        BuildCosts(root, frame, fill, font, visual, refs);
        BuildMissionBonus(root, frame, fill, font, refs);
    }

    private static void BuildShipCards(Transform root, Sprite frame, Sprite fill, Sprite glow,
        TMP_FontAsset font, Dimension1HangarVisualUI visual, Refs refs, Sprite[] vehicleSprites,
        Material blueprintMaterial)
    {
        string[] labels = { "SONDA LIGERA", "DRON\nEXTRACTOR", "SONDA\nANALÍTICA", "NAVE DE\nCARGA" };
        for (int i = 0; i < 4; i++)
        {
            bool selected = i == 0;
            float x = 59f + i * 244f;
            Button card = ButtonPanel("ShipCard_" + i, root, frame, fill,
                new Vector2(x, 390f), new Vector2(229f, 349f), selected ? AmberFill : Fill,
                selected ? Amber : CyanMuted, out Image cardFill, out Image border);
            refs.shipButtons.Add(card);
            refs.shipFills.Add(cardFill);
            refs.shipBorders.Add(border);
            if (selected)
            {
                Image selectionGlow = Image("SelectionGlow", card.transform, glow, Hex("F4A70B", 34));
                Stretch(selectionGlow.rectTransform, new Vector2(-15f, -10f), new Vector2(-15f, -10f));
                selectionGlow.raycastTarget = false;
                selectionGlow.transform.SetAsFirstSibling();
            }

            RectTransform art = Rect("ShipArt", card.transform);
            Centered(art, new Vector2(0f, 44f), new Vector2(210f, 230f));
            DrawVehicleSprite(art, vehicleSprites[i], Vector2.zero, CardBlueprintSize(i),
                selected ? Amber : Cyan, blueprintMaterial);
            refs.shipArtRoots.Add(art.gameObject);

            TMP_Text label = Text("ShipLabel", card.transform, font, labels[i], 25f, FontStyles.Bold, selected ? Amber : Cyan);
            Top(label.rectTransform, 12f, 274f, 205f, 61f);
            label.alignment = TextAlignmentOptions.Center;
            label.characterSpacing = 2f;
            refs.shipLabels.Add(label);

            if (i == 0) AddPersistent(card.onClick, visual.SelectShip0);
            else if (i == 1) AddPersistent(card.onClick, visual.SelectShip1);
            else if (i == 2) AddPersistent(card.onClick, visual.SelectShip2);
            else AddPersistent(card.onClick, visual.SelectShip3);
        }
    }

    private static void BuildDetails(Transform root, Sprite frame, Sprite fill, Sprite glow,
        TMP_FontAsset font, Dimension1HangarVisualUI visual, Refs refs, Sprite[] vehicleSprites,
        Material blueprintMaterial)
    {
        RectTransform display = Panel("ShipDisplay", root, frame, fill,
            new Vector2(57f, 769f), new Vector2(383f, 547f), Fill, CyanMuted, out _, out _);
        RectTransform grid = Rect("TechnicalGrid", display);
        Centered(grid, new Vector2(0f, -5f), new Vector2(350f, 500f));
        Circle(grid, Vector2.zero, 152f, 1f, Hex("087FA9", 42));
        Circle(grid, Vector2.zero, 118f, 1f, Hex("087FA9", 34));
        Circle(grid, Vector2.zero, 78f, 1f, Hex("087FA9", 28));
        Line("GridAxisV", grid, new Vector2(0f, 230f), new Vector2(0f, -230f), 1f, Hex("18C8FF", 45));
        Line("GridAxisH", grid, new Vector2(-165f, 0f), new Vector2(165f, 0f), 1f, Hex("18C8FF", 45));
        Dot(grid, new Vector2(0f, 230f), 7f, CyanMuted);
        Dot(grid, new Vector2(0f, -230f), 7f, CyanMuted);
        Dot(grid, new Vector2(-165f, 0f), 7f, CyanMuted);
        Dot(grid, new Vector2(165f, 0f), 7f, CyanMuted);
        for (int i = 0; i < 4; i++)
        {
            RectTransform art = Rect("LargeShipArt_" + i, display);
            Centered(art, Vector2.zero, new Vector2(350f, 500f));
            DrawVehicleSprite(art, vehicleSprites[i], Vector2.zero, LargeBlueprintSize(i), Cyan,
                blueprintMaterial);
            art.gameObject.SetActive(i == 0);
            refs.largeShipArtRoots.Add(art.gameObject);
        }

        string[] labels = { "CARGA", "VELOCIDAD", "BLINDAJE", "SENSORES" };
        string[] levels = { "NIVEL 2/4", "NIVEL 2/4", "NIVEL 1/4", "NIVEL 2/4" };
        string[] values = { "120 / 200", "120 UA/s", "80 / 150", "3.5 UA" };
        float[] bars = { .53f, .62f, .48f, .55f };
        for (int i = 0; i < 4; i++)
        {
            bool selected = i == 1;
            Button stat = ButtonPanel("PartCard_" + i, root, frame, fill,
                new Vector2(452f, 769f + i * 137f), new Vector2(566f, 123f),
                selected ? AmberFill : Fill, selected ? Amber : CyanMuted,
                out Image statFill, out Image statBorder);
            refs.partButtons.Add(stat);
            refs.partFills.Add(statFill);
            refs.partBorders.Add(statBorder);
            RectTransform icon = Rect("PartIcon", stat.transform);
            Centered(icon, new Vector2(-235f, 0f), new Vector2(86f, 86f));
            DrawPartIcon(icon, i, Vector2.zero, 31f, selected ? Amber : Cyan);
            refs.partIconRoots.Add(icon.gameObject);
            TMP_Text label = Text("PartLabel", stat.transform, font, labels[i], 26f, FontStyles.Bold, selected ? Amber : Cyan);
            Top(label.rectTransform, 108f, 17f, 250f, 34f);
            TMP_Text level = Text("PartLevel", stat.transform, font, levels[i], 18f, FontStyles.Bold, Secondary);
            Top(level.rectTransform, 108f, 49f, 210f, 27f);
            TMP_Text value = Text("PartValue", stat.transform, font, values[i], 23f, FontStyles.Normal, Secondary);
            Top(value.rectTransform, 388f, 27f, 144f, 38f);
            value.alignment = TextAlignmentOptions.Right;
            Image barBg = Image("BarBackground", stat.transform, null, Hex("253541", 210));
            Top(barBg.rectTransform, 108f, 82f, 424f, 15f);
            barBg.raycastTarget = false;
            Image bar = Image("Bar", barBg.transform, null, selected ? Amber : Hex("3BAED9"));
            bar.rectTransform.anchorMin = Vector2.zero;
            bar.rectTransform.anchorMax = new Vector2(bars[i], 1f);
            bar.rectTransform.offsetMin = Vector2.zero;
            bar.rectTransform.offsetMax = Vector2.zero;
            bar.raycastTarget = false;
            refs.partLabels.Add(label);
            refs.partLevels.Add(level);
            refs.partValues.Add(value);
            refs.partBars.Add(bar);
            if (i == 0) AddPersistent(stat.onClick, visual.SelectCargo);
            else if (i == 1) AddPersistent(stat.onClick, visual.SelectSpeed);
            else if (i == 2) AddPersistent(stat.onClick, visual.SelectArmor);
            else AddPersistent(stat.onClick, visual.SelectSensors);
        }
    }

    private static void BuildCosts(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font,
        Dimension1HangarVisualUI visual, Refs refs)
    {
        Line("CostDivider", root, new Vector2(-478f, -406f), new Vector2(478f, -406f), 1.2f, Hex("087FA9", 150));
        TMP_Text section = Text("CostSection", root, font, "COSTE DE MEJORA", 21f, FontStyles.Bold, Hex("48BDE8"));
        Top(section.rectTransform, 87f, 1340f, 330f, 34f);
        for (int i = 0; i < 2; i++)
        {
            RectTransform cost = Panel("UpgradeCost_" + i, root, frame, fill,
                new Vector2(72f + i * 249f, 1383f), new Vector2(235f, 102f), Fill, CyanMuted, out _, out _);
            RectTransform icon = Rect("CostIcon", cost);
            Centered(icon, new Vector2(-79f, 0f), new Vector2(62f, 62f));
            if (i == 0) DrawMetalIcon(icon, Vector2.zero, 24f, 0, Secondary);
            else DrawMatrix(icon, Vector2.zero, 27f, CyanMuted);
            TMP_Text name = Text("CostName", cost, font, i == 0 ? "HIERRO" : "MATRIZ", 17f, FontStyles.Bold, Secondary);
            Top(name.rectTransform, 78f, 20f, 142f, 25f);
            TMP_Text value = Text("CostValue", cost, font, i == 0 ? "45K" : "12", 28f, FontStyles.Normal, Primary);
            Top(value.rectTransform, 78f, 43f, 142f, 40f);
            refs.costNames.Add(name);
            refs.costValues.Add(value);
        }

        refs.upgradeButton = ButtonPanel("UpgradeButton", root, frame, fill,
            new Vector2(585f, 1358f), new Vector2(420f, 128f), AmberFill, Amber, out _, out _);
        RectTransform upgradeIcon = Rect("UpgradeIcon", refs.upgradeButton.transform);
        Centered(upgradeIcon, new Vector2(-153f, 0f), new Vector2(90f, 90f));
        DrawUpgradeIcon(upgradeIcon, Vector2.zero, 34f, Amber);
        refs.upgradeButtonLabel = Text("Label", refs.upgradeButton.transform, font,
            "MEJORAR\nVELOCIDAD", 31f, FontStyles.Bold, Amber);
        Top(refs.upgradeButtonLabel.rectTransform, 114f, 25f, 275f, 78f);
        refs.upgradeButtonLabel.alignment = TextAlignmentOptions.Center;
        refs.upgradeButtonLabel.characterSpacing = 3f;
        AddPersistent(refs.upgradeButton.onClick, visual.UpgradeSelected);
    }

    private static void BuildMissionBonus(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font, Refs refs)
    {
        RectTransform bonus = Panel("MissionBonus", root, frame, fill,
            new Vector2(56f, 1539f), new Vector2(963f, 126f), Fill, CyanMuted, out _, out _);
        RectTransform icon = Rect("BonusIcon", bonus);
        Centered(icon, new Vector2(-394f, 0f), new Vector2(112f, 92f));
        DrawMissionIcon(icon, Vector2.zero, 38f, CyanMuted);
        TMP_Text eyebrow = Text("BonusTitle", bonus, font, "BONIFICACIÓN DE MISIÓN", 23f, FontStyles.Bold, Hex("48BDE8"));
        Top(eyebrow.rectTransform, 160f, 28f, 440f, 34f);
        refs.missionBonus = Text("BonusValue", bonus, font, "+12% VELOCIDAD DE EXPLORACIÓN", 22f, FontStyles.Normal, Secondary);
        Top(refs.missionBonus.rectTransform, 160f, 66f, 700f, 36f);
        refs.missionBonus.characterSpacing = 2f;
    }

    private static void BuildNavigation(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font,
        Dimension1HangarVisualUI visual)
    {
        RectTransform nav = Rect("BottomNavigation", root);
        Top(nav, Dimension1SharedLayoutTokens.NavigationX,
            Dimension1SharedLayoutTokens.NavigationY,
            Dimension1SharedLayoutTokens.NavigationWidth,
            Dimension1SharedLayoutTokens.NavigationHeight);
        string[] labels = { "GALAXIA", "EXPLORAR", "HANGAR", "RELIQUIAS", "ÁRBOL" };
        for (int i = 0; i < 5; i++)
        {
            bool selected = i == 2;
            Button button = ButtonPanel("Nav_" + labels[i], nav, frame, fill,
                new Vector2(Dimension1SharedLayoutTokens.NavigationCardX(i),
                    Dimension1SharedLayoutTokens.NavigationCardY),
                new Vector2(Dimension1SharedLayoutTokens.NavigationCardWidth,
                    Dimension1SharedLayoutTokens.NavigationCardHeight),
                selected ? AmberFill : Fill, selected ? Amber : CyanMuted, out _, out _);
            DrawNavigationIcon(button.transform, i, new Vector2(0f, 31f), 39f, selected ? Amber : Cyan);
            TMP_Text label = Text("Label", button.transform, font, labels[i], 22f, FontStyles.Bold, selected ? Amber : Cyan);
            Top(label.rectTransform, 10f, 118f, 178f, 34f);
            label.alignment = TextAlignmentOptions.Center;
            if (selected)
            {
                button.interactable = false;
                Polygon("SelectedPointer", nav, new[]
                {
                    new Vector2(-424f + i * Dimension1SharedLayoutTokens.NavigationCardStep, 88f),
                    new Vector2(-410f + i * Dimension1SharedLayoutTokens.NavigationCardStep, 108f),
                    new Vector2(-396f + i * Dimension1SharedLayoutTokens.NavigationCardStep, 88f)
                }, Amber);
            }
            else if (i == 0) AddPersistent(button.onClick, visual.OpenGalaxy);
            else if (i == 1) AddPersistent(button.onClick, visual.OpenExplore);
            else if (i == 3) AddPersistent(button.onClick, visual.OpenRelics);
            else if (i == 4) AddPersistent(button.onClick, visual.OpenTree);
        }
    }

    private static void DrawVehicle(Transform parent, int variant, Vector2 center, float size, Color color)
    {
        if (variant == 0) DrawLightProbe(parent, center, size, color);
        else if (variant == 1) DrawExtractor(parent, center, size, color);
        else if (variant == 2) DrawAnalyticCraft(parent, center, size, color);
        else DrawCargoCraft(parent, center, size, color);
    }

    private static void DrawVehicleSprite(Transform parent, Sprite sprite, Vector2 center, Vector2 size,
        Color color, Material material)
    {
        Image image = Image("BlueprintSprite", parent, sprite, color);
        Centered(image.rectTransform, center, size);
        image.preserveAspect = true;
        image.material = material;
        image.raycastTarget = false;
    }

    private static Vector2 CardBlueprintSize(int variant)
    {
        if (variant == 0) return new Vector2(252f, 252f);
        if (variant == 1) return new Vector2(232f, 232f);
        if (variant == 2) return new Vector2(242f, 242f);
        return new Vector2(226f, 226f);
    }

    private static Vector2 LargeBlueprintSize(int variant)
    {
        if (variant == 0) return new Vector2(470f, 470f);
        if (variant == 1) return new Vector2(438f, 438f);
        if (variant == 2) return new Vector2(454f, 454f);
        return new Vector2(430f, 430f);
    }

    private static void DrawLightProbe(Transform parent, Vector2 c, float s, Color color)
    {
        float o = Mathf.Clamp(s * .018f, 1.25f, 2.8f);
        float d = Mathf.Max(.9f, o * .56f);
        Color inner = WithAlpha(CyanBright, 155);
        AddGlow(parent, c, new Vector2(s * 2.15f, s * 2.45f), color, .045f);
        Vector2[] body =
        {
            c + new Vector2(0,s), c + new Vector2(s*.15f,s*.72f), c + new Vector2(s*.22f,s*.25f),
            c + new Vector2(s*.19f,-s*.44f), c + new Vector2(s*.30f,-s*.72f), c + new Vector2(s*.20f,-s),
            c + new Vector2(-s*.20f,-s), c + new Vector2(-s*.30f,-s*.72f), c + new Vector2(-s*.19f,-s*.44f),
            c + new Vector2(-s*.22f,s*.25f), c + new Vector2(-s*.15f,s*.72f)
        };
        Vector2[] leftWing =
        {
            c + new Vector2(-s*.18f,s*.43f), c + new Vector2(-s*.40f,s*.08f), c + new Vector2(-s*.88f,-s*.34f),
            c + new Vector2(-s*.82f,-s*.68f), c + new Vector2(-s*.34f,-s*.41f), c + new Vector2(-s*.23f,-s*.10f)
        };
        Vector2[] rightWing = Mirror(leftWing, c);
        Polygon("WingFillL", parent, leftWing, WithAlpha(color, 22));
        Polygon("WingFillR", parent, rightWing, WithAlpha(color, 22));
        Polygon("BodyFill", parent, body, WithAlpha(color, 30));
        LineLoop("WingL", parent, leftWing, o, color);
        LineLoop("WingR", parent, rightWing, o, color);
        LineLoop("Body", parent, body, o, color);
        LineLoop("BodyInset", parent, new[]
        {
            c + new Vector2(0,s*.86f), c + new Vector2(s*.10f,s*.58f), c + new Vector2(s*.12f,-s*.47f),
            c + new Vector2(0,-s*.79f), c + new Vector2(-s*.12f,-s*.47f), c + new Vector2(-s*.10f,s*.58f)
        }, d, inner);
        Line("WingRibL1", parent, leftWing[1], leftWing[3], d, inner);
        Line("WingRibL2", parent, leftWing[2], leftWing[4], d, WithAlpha(color, 105));
        Line("WingRibR1", parent, rightWing[1], rightWing[3], d, inner);
        Line("WingRibR2", parent, rightWing[2], rightWing[4], d, WithAlpha(color, 105));
        Line("Axis", parent, c + new Vector2(0,s*.90f), c + new Vector2(0,-s*.90f), d, inner);
        for (int i = 0; i < 5; i++)
        {
            float y = Mathf.Lerp(s*.48f, -s*.43f, i/4f);
            float w = i < 2 ? s*.13f : s*.16f;
            Line("BodyBand" + i, parent, c + new Vector2(-w,y), c + new Vector2(w,y), d, WithAlpha(color, 125));
        }
        Vector2 engine = c + new Vector2(0,-s*.66f);
        LineLoop("EngineHousing", parent, new[]
        {
            engine + new Vector2(0,s*.24f), engine + new Vector2(s*.22f,s*.10f),
            engine + new Vector2(s*.18f,-s*.23f), engine + new Vector2(0,-s*.30f),
            engine + new Vector2(-s*.18f,-s*.23f), engine + new Vector2(-s*.22f,s*.10f)
        }, d, inner);
        Circle(parent, engine, s*.13f, d, color);
        Circle(parent, engine, s*.055f, d, inner);
        Line("EngineBand", parent, engine + new Vector2(-s*.20f,-s*.14f), engine + new Vector2(s*.20f,-s*.14f), d, inner);
        Line("NoseRailL", parent, c + new Vector2(-s*.07f,s*.78f), c + new Vector2(-s*.07f,s*.28f), d, inner);
        Line("NoseRailR", parent, c + new Vector2(s*.07f,s*.78f), c + new Vector2(s*.07f,s*.28f), d, inner);
    }

    private static void DrawExtractor(Transform parent, Vector2 c, float s, Color color)
    {
        float o = Mathf.Clamp(s * .019f, 1.2f, 2.7f);
        float d = Mathf.Max(.85f, o * .55f);
        Color inner = WithAlpha(CyanBright, 150);
        AddGlow(parent, c, new Vector2(s * 2.4f, s * 2.25f), color, .04f);
        Vector2[] core =
        {
            c + new Vector2(0,s), c + new Vector2(s*.25f,s*.70f), c + new Vector2(s*.29f,-s*.52f),
            c + new Vector2(s*.17f,-s), c + new Vector2(-s*.17f,-s), c + new Vector2(-s*.29f,-s*.52f),
            c + new Vector2(-s*.25f,s*.70f)
        };
        Vector2[] leftPod =
        {
            c + new Vector2(-s*.25f,s*.61f), c + new Vector2(-s*.52f,s*.55f), c + new Vector2(-s*.72f,s*.15f),
            c + new Vector2(-s*.72f,-s*.62f), c + new Vector2(-s*.47f,-s*.78f), c + new Vector2(-s*.35f,-s*.37f),
            c + new Vector2(-s*.36f,s*.32f)
        };
        Vector2[] rightPod = Mirror(leftPod, c);
        Vector2[] leftArm =
        {
            c + new Vector2(-s*.51f,s*.52f), c + new Vector2(-s*.92f,s*.42f), c + new Vector2(-s*.95f,-s*.28f),
            c + new Vector2(-s*.73f,-s*.48f), c + new Vector2(-s*.68f,s*.10f)
        };
        Vector2[] rightArm = Mirror(leftArm, c);
        foreach (Vector2[] shape in new[] { core, leftPod, rightPod, leftArm, rightArm })
        {
            Polygon("ExtractorFill", parent, shape, WithAlpha(color, 20));
            LineLoop("ExtractorShape", parent, shape, o, color);
        }
        LineLoop("CoreInset", parent, new[]
        {
            c + new Vector2(0,s*.78f), c + new Vector2(s*.14f,s*.52f), c + new Vector2(s*.14f,-s*.50f),
            c + new Vector2(0,-s*.78f), c + new Vector2(-s*.14f,-s*.50f), c + new Vector2(-s*.14f,s*.52f)
        }, d, inner);
        Line("CoreAxis", parent, c + new Vector2(0,s*.84f), c + new Vector2(0,-s*.82f), d, inner);
        Line("PodAxisL", parent, c + new Vector2(-s*.53f,s*.42f), c + new Vector2(-s*.53f,-s*.58f), d, inner);
        Line("PodAxisR", parent, c + new Vector2(s*.53f,s*.42f), c + new Vector2(s*.53f,-s*.58f), d, inner);
        for (int i = 0; i < 4; i++)
        {
            float y = Mathf.Lerp(s*.35f,-s*.45f,i/3f);
            Line("CoreBand" + i, parent, c + new Vector2(-s*.13f,y), c + new Vector2(s*.13f,y), d, WithAlpha(color, 120));
        }
        Circle(parent, c + new Vector2(0,-s*.26f), s*.17f, d, color);
        Circle(parent, c + new Vector2(0,-s*.26f), s*.07f, d, inner);
        LineLoop("ExtractorBridge", parent, new[]
        {
            c + new Vector2(-s*.18f,s*.74f), c + new Vector2(s*.18f,s*.74f),
            c + new Vector2(s*.12f,s*.42f), c + new Vector2(-s*.12f,s*.42f)
        }, d, inner);
        Line("ExtractorArmBandL", parent, c + new Vector2(-s*.91f,s*.05f), c + new Vector2(-s*.70f,s*.05f), d, inner);
        Line("ExtractorArmBandR", parent, c + new Vector2(s*.91f,s*.05f), c + new Vector2(s*.70f,s*.05f), d, inner);
    }

    private static void DrawAnalyticCraft(Transform parent, Vector2 c, float s, Color color)
    {
        float o = Mathf.Clamp(s * .019f, 1.2f, 2.8f);
        float d = Mathf.Max(.9f, o * .55f);
        Color inner = WithAlpha(CyanBright, 150);
        AddGlow(parent, c, new Vector2(s * 2.2f, s * 2.35f), color, .04f);
        Vector2[] body =
        {
            c + new Vector2(0,s), c + new Vector2(s*.13f,s*.63f), c + new Vector2(s*.19f,-s*.46f),
            c + new Vector2(s*.08f,-s), c + new Vector2(-s*.08f,-s), c + new Vector2(-s*.19f,-s*.46f),
            c + new Vector2(-s*.13f,s*.63f)
        };
        Vector2[] leftWing =
        {
            c + new Vector2(-s*.15f,s*.30f), c + new Vector2(-s*.34f,s*.02f), c + new Vector2(-s*.94f,-s*.52f),
            c + new Vector2(-s*.87f,-s*.71f), c + new Vector2(-s*.29f,-s*.41f), c + new Vector2(-s*.20f,-s*.08f)
        };
        Vector2[] rightWing = Mirror(leftWing, c);
        Polygon("AnalyticBodyFill", parent, body, WithAlpha(color, 24));
        Polygon("AnalyticWingFillL", parent, leftWing, WithAlpha(color, 18));
        Polygon("AnalyticWingFillR", parent, rightWing, WithAlpha(color, 18));
        LineLoop("AnalyticBody", parent, body, o, color);
        LineLoop("AnalyticWingL", parent, leftWing, o, color);
        LineLoop("AnalyticWingR", parent, rightWing, o, color);
        LineLoop("AnalyticCockpit", parent, new[]
        {
            c + new Vector2(0,s*.74f), c + new Vector2(s*.08f,s*.44f), c + new Vector2(s*.08f,-s*.12f),
            c + new Vector2(0,-s*.32f), c + new Vector2(-s*.08f,-s*.12f), c + new Vector2(-s*.08f,s*.44f)
        }, d, inner);
        Line("AnalyticAxis", parent, c + new Vector2(0,s*.88f), c + new Vector2(0,-s*.87f), d, inner);
        Line("AnalyticRibL1", parent, leftWing[1], leftWing[3], d, inner);
        Line("AnalyticRibL2", parent, leftWing[2], leftWing[4], d, WithAlpha(color, 105));
        Line("AnalyticRibR1", parent, rightWing[1], rightWing[3], d, inner);
        Line("AnalyticRibR2", parent, rightWing[2], rightWing[4], d, WithAlpha(color, 105));
        Circle(parent, c + new Vector2(0,-s*.55f), s*.15f, d, color);
        Circle(parent, c + new Vector2(0,-s*.55f), s*.055f, d, inner);
        Line("AnalyticRailL", parent, c + new Vector2(-s*.045f,s*.62f), c + new Vector2(-s*.045f,-s*.38f), d, inner);
        Line("AnalyticRailR", parent, c + new Vector2(s*.045f,s*.62f), c + new Vector2(s*.045f,-s*.38f), d, inner);
        Line("AnalyticWingBandL", parent, leftWing[0], leftWing[4], d, WithAlpha(color, 130));
        Line("AnalyticWingBandR", parent, rightWing[0], rightWing[4], d, WithAlpha(color, 130));
    }

    private static void DrawCargoCraft(Transform parent, Vector2 c, float s, Color color)
    {
        float o = Mathf.Clamp(s * .020f, 1.2f, 2.8f);
        float d = Mathf.Max(.9f, o * .54f);
        Color inner = WithAlpha(CyanBright, 145);
        AddGlow(parent, c, new Vector2(s * 2.55f, s * 2.25f), color, .04f);
        Vector2[] core =
        {
            c + new Vector2(-s*.31f,s*.88f), c + new Vector2(s*.31f,s*.88f), c + new Vector2(s*.42f,s*.55f),
            c + new Vector2(s*.42f,-s*.68f), c + new Vector2(s*.21f,-s), c + new Vector2(-s*.21f,-s),
            c + new Vector2(-s*.42f,-s*.68f), c + new Vector2(-s*.42f,s*.55f)
        };
        Vector2[] leftPod =
        {
            c + new Vector2(-s*.43f,s*.55f), c + new Vector2(-s*.78f,s*.48f), c + new Vector2(-s*.86f,s*.22f),
            c + new Vector2(-s*.86f,-s*.55f), c + new Vector2(-s*.69f,-s*.72f), c + new Vector2(-s*.46f,-s*.60f)
        };
        Vector2[] rightPod = Mirror(leftPod, c);
        Vector2[] leftCargo =
        {
            c + new Vector2(-s*.78f,s*.34f), c + new Vector2(-s*1.08f,s*.28f), c + new Vector2(-s*1.08f,-s*.48f),
            c + new Vector2(-s*.82f,-s*.55f)
        };
        Vector2[] rightCargo = Mirror(leftCargo, c);
        foreach (Vector2[] shape in new[] { core, leftPod, rightPod, leftCargo, rightCargo })
        {
            Polygon("CargoFill", parent, shape, WithAlpha(color, 22));
            LineLoop("CargoShape", parent, shape, o, color);
        }
        LineLoop("CargoCoreInset", parent, new[]
        {
            c + new Vector2(-s*.18f,s*.66f), c + new Vector2(s*.18f,s*.66f), c + new Vector2(s*.23f,-s*.53f),
            c + new Vector2(0,-s*.79f), c + new Vector2(-s*.23f,-s*.53f)
        }, d, inner);
        Line("CargoAxis", parent, c + new Vector2(0,s*.78f), c + new Vector2(0,-s*.80f), d, inner);
        for (int i = 0; i < 4; i++)
        {
            float y = Mathf.Lerp(s*.37f,-s*.42f,i/3f);
            Line("CargoBand" + i, parent, c + new Vector2(-s*.20f,y), c + new Vector2(s*.20f,y), d, WithAlpha(color, 120));
        }
        Line("CargoPodL", parent, c + new Vector2(-s*.67f,s*.34f), c + new Vector2(-s*.67f,-s*.53f), d, inner);
        Line("CargoPodR", parent, c + new Vector2(s*.67f,s*.34f), c + new Vector2(s*.67f,-s*.53f), d, inner);
        LineLoop("CargoBridge", parent, new[]
        {
            c + new Vector2(-s*.16f,s*.88f), c + new Vector2(s*.16f,s*.88f),
            c + new Vector2(s*.10f,s*.60f), c + new Vector2(-s*.10f,s*.60f)
        }, d, inner);
        for (int i = 0; i < 3; i++)
        {
            float y = Mathf.Lerp(s*.15f,-s*.32f,i/2f);
            Line("CargoPodBandL" + i, parent, c + new Vector2(-s*1.05f,y), c + new Vector2(-s*.80f,y), d, WithAlpha(color, 125));
            Line("CargoPodBandR" + i, parent, c + new Vector2(s*.80f,y), c + new Vector2(s*1.05f,y), d, WithAlpha(color, 125));
        }
    }

    private static void DrawPartIcon(Transform parent, int variant, Vector2 c, float s, Color color)
    {
        if (variant == 0)
        {
            LineLoop("Case", parent, new[]
            {
                c + new Vector2(-s*.75f,s*.45f), c + new Vector2(s*.75f,s*.45f),
                c + new Vector2(s*.75f,-s*.58f), c + new Vector2(-s*.75f,-s*.58f)
            }, 2.5f, color);
            LineLoop("Handle", parent, new[]
            {
                c + new Vector2(-s*.34f,s*.45f), c + new Vector2(-s*.30f,s*.78f),
                c + new Vector2(s*.30f,s*.78f), c + new Vector2(s*.34f,s*.45f)
            }, 2.2f, color);
            Line("Latch", parent, c + new Vector2(0,s*.30f), c + new Vector2(0,-s*.10f), 2f, color);
            return;
        }
        if (variant == 1)
        {
            Circle(parent, c, s*.78f, 2.2f, color);
            Polygon("Bolt", parent, new[]
            {
                c + new Vector2(s*.20f,s*.72f), c + new Vector2(-s*.48f,s*.02f),
                c + new Vector2(-s*.08f,-s*.02f), c + new Vector2(-s*.30f,-s*.69f),
                c + new Vector2(s*.48f,s*.10f), c + new Vector2(s*.09f,s*.13f)
            }, WithAlpha(color, 180));
            return;
        }
        if (variant == 2)
        {
            LineLoop("Shield", parent, new[]
            {
                c + new Vector2(0,s*.88f), c + new Vector2(s*.66f,s*.57f), c + new Vector2(s*.58f,-s*.31f),
                c + new Vector2(0,-s*.90f), c + new Vector2(-s*.58f,-s*.31f), c + new Vector2(-s*.66f,s*.57f)
            }, 2.6f, color);
            Line("ShieldAxis", parent, c + new Vector2(0,s*.70f), c + new Vector2(0,-s*.66f), 1.5f, color);
            return;
        }
        Circle(parent, c, s*.60f, 2f, color);
        Circle(parent, c, s*.22f, 1.7f, color);
        Line("RadarV", parent, c + new Vector2(0,s), c + new Vector2(0,-s), 1.7f, color);
        Line("RadarH", parent, c + new Vector2(-s,0), c + new Vector2(s,0), 1.7f, color);
        Dot(parent, c, 7f, color);
    }

    private static void DrawUpgradeIcon(Transform parent, Vector2 c, float s, Color color)
    {
        LineLoop("UpgradeHex", parent, Hexagon(c, s), 2.5f, color);
        Line("ArrowL1", parent, c + new Vector2(-s*.42f,-s*.22f), c + new Vector2(0,s*.18f), 4f, color);
        Line("ArrowR1", parent, c + new Vector2(0,s*.18f), c + new Vector2(s*.42f,-s*.22f), 4f, color);
        Line("ArrowL2", parent, c + new Vector2(-s*.42f,s*.10f), c + new Vector2(0,s*.50f), 4f, color);
        Line("ArrowR2", parent, c + new Vector2(0,s*.50f), c + new Vector2(s*.42f,s*.10f), 4f, color);
    }

    private static void DrawMissionIcon(Transform parent, Vector2 c, float s, Color color)
    {
        Circle(parent, c, s*.54f, 2.2f, color);
        Circle(parent, c, s*.18f, 2f, CyanBright);
        Line("MissionV", parent, c + new Vector2(0,s*.95f), c + new Vector2(0,-s*.95f), 1.8f, color);
        Line("MissionH", parent, c + new Vector2(-s*.95f,0), c + new Vector2(s*.95f,0), 1.8f, color);
        for (int i = 0; i < 8; i++)
        {
            float a = Mathf.PI * 2f * i / 8f;
            Vector2 p1 = c + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * s*.70f;
            Vector2 p2 = c + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * s*.88f;
            Line("MissionTick" + i, parent, p1, p2, 1.5f, color);
        }
        Dot(parent, c, 7f, CyanBright);
    }

    private static void DrawHomeIcon(Transform parent, Vector2 c, float s, Color color)
    {
        LineLoop("Home", parent, new[]
        {
            c + new Vector2(-s*.75f,-s*.10f), c + new Vector2(0,s*.72f), c + new Vector2(s*.75f,-s*.10f),
            c + new Vector2(s*.55f,-s*.10f), c + new Vector2(s*.55f,-s*.72f), c + new Vector2(-s*.55f,-s*.72f),
            c + new Vector2(-s*.55f,-s*.10f)
        }, 2f, color);
        LineLoop("Door", parent, new[]
        {
            c + new Vector2(-s*.17f,-s*.72f), c + new Vector2(-s*.17f,-s*.27f),
            c + new Vector2(s*.17f,-s*.27f), c + new Vector2(s*.17f,-s*.72f)
        }, 1.5f, color);
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

    private static void DrawMatrix(Transform parent, Vector2 c, float s, Color color)
    {
        Vector2[] front =
        {
            c + new Vector2(0,s), c + new Vector2(s*.78f,s*.48f), c + new Vector2(s*.78f,-s*.48f),
            c + new Vector2(0,-s), c + new Vector2(-s*.78f,-s*.48f), c + new Vector2(-s*.78f,s*.48f)
        };
        LineLoop("Matrix", parent, front, 1.8f, color);
        Line("MatrixV", parent, front[0], front[3], 1f, color);
        Line("MatrixD1", parent, front[1], front[4], 1f, color);
        Line("MatrixD2", parent, front[2], front[5], 1f, color);
        Circle(parent, c, s*.20f, 1f, color);
    }

    private static void DrawNavigationIcon(Transform parent, int variant, Vector2 c, float s, Color color)
    {
        if (variant == 0)
        {
            Circle(parent, c, s*.18f, 1.5f, color);
            for (int arm = 0; arm < 3; arm++)
            {
                var points = new List<Vector2>();
                float baseA = arm * Mathf.PI * 2f / 3f;
                for (int i = 0; i < 13; i++)
                {
                    float t = i/12f;
                    float a = baseA + t*2.35f;
                    points.Add(c + new Vector2(Mathf.Cos(a),Mathf.Sin(a)*.48f)*Mathf.Lerp(s*.14f,s,t));
                }
                LineGraphic("GalaxyArm", parent, points, 2f, false, color);
            }
            return;
        }
        if (variant == 1)
        {
            Circle(parent, c, s*.72f, 1.7f, color);
            Circle(parent, c, s*.28f, 1.2f, color);
            Line("RadarV", parent, c + new Vector2(0,s), c + new Vector2(0,-s), 1.3f, color);
            Line("RadarH", parent, c + new Vector2(-s,0), c + new Vector2(s,0), 1.2f, color);
            Polygon("Sweep", parent, new[] { c, c + new Vector2(s*.25f,s*.66f), c + new Vector2(s*.61f,s*.30f) }, WithAlpha(color, 42));
            return;
        }
        if (variant == 2)
        {
            DrawLightProbe(parent, c, s*.82f, color);
            return;
        }
        if (variant == 3)
        {
            Vector2[] crystal =
            {
                c + new Vector2(0,s), c + new Vector2(s*.34f,s*.20f), c + new Vector2(s*.18f,-s*.60f),
                c + new Vector2(0,-s*.88f), c + new Vector2(-s*.18f,-s*.60f), c + new Vector2(-s*.34f,s*.20f)
            };
            LineLoop("Relic", parent, crystal, 2f, color);
            Line("RelicFacetL", parent, crystal[0], crystal[4], 1f, color);
            Line("RelicFacetR", parent, crystal[0], crystal[2], 1f, color);
            DrawEllipse(parent, c + new Vector2(0,-s*.92f), s*.95f, s*.26f, 1.4f, color);
            return;
        }
        Line("TreeTrunk", parent, c + new Vector2(0,-s), c + new Vector2(0,s*.78f), 2f, color);
        Vector2[] nodes =
        {
            c + new Vector2(-s*.72f,s*.62f), c + new Vector2(-s*.52f,s*.10f), c + new Vector2(-s*.80f,-s*.25f),
            c + new Vector2(s*.72f,s*.62f), c + new Vector2(s*.52f,s*.10f), c + new Vector2(s*.80f,-s*.25f),
            c + new Vector2(0,s*.98f)
        };
        foreach (Vector2 node in nodes)
        {
            Vector2 joint = c + new Vector2(node.x > c.x ? s*.12f : node.x < c.x ? -s*.12f : 0f,
                Mathf.Lerp(-s*.18f,s*.55f,Mathf.InverseLerp(-s*.3f,s,node.y-c.y)));
            Line("TreeBranch", parent, joint, node, 1.4f, color);
            Circle(parent, node, 3.4f, 1.1f, color);
        }
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

    private static Vector2[] Mirror(Vector2[] source, Vector2 c)
    {
        Vector2[] result = new Vector2[source.Length];
        for (int i = 0; i < source.Length; i++) result[i] = new Vector2(c.x*2f-source[i].x,source[i].y);
        return result;
    }

    private static void AddGlow(Transform parent, Vector2 center, Vector2 size, Color color, float alpha)
    {
        Image glow = Image("TechnicalGlow", parent, LoadSprite(ArtPath + "/d1_glow_v3.png"),
            new Color(color.r,color.g,color.b,alpha));
        Centered(glow.rectTransform, center, size);
        glow.raycastTarget = false;
        glow.transform.SetAsFirstSibling();
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

    private static void LineLoop(string name, Transform parent, IList<Vector2> points, float width, Color color)
        => LineGraphic(name,parent,points,width,true,color);

    private static void LineGraphic(string name, Transform parent, IList<Vector2> points, float width, bool closed, Color color)
    {
        GameObject go = new GameObject(name,typeof(RectTransform));
        go.transform.SetParent(parent,false);
        Stretch((RectTransform)go.transform);
        Dimension1CommandCenterLineGraphic line = go.AddComponent<Dimension1CommandCenterLineGraphic>();
        line.color = color;
        line.raycastTarget = false;
        line.SetLine(points,width,closed);
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
        TMP_Text[] texts = root.GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text text in texts)
            if (text != null && text.font != null && text.font.name.ToLowerInvariant().Contains("rajdhani")) return text.font;
        foreach (TMP_Text text in texts)
            if (text != null && text.font != null) return text.font;
        return TMP_Settings.defaultFontAsset;
    }

    private static Sprite[] LoadVehicleSprites()
    {
        string[] paths =
        {
            ArtPath + "/d1_hangar_sonda_ligera_blueprint_v2.png",
            ArtPath + "/d1_hangar_dron_extractor_blueprint_v2.png",
            ArtPath + "/d1_hangar_sonda_analitica_blueprint_v2.png",
            ArtPath + "/d1_hangar_nave_carga_blueprint_v2.png"
        };
        var sprites = new Sprite[paths.Length];
        for (int i = 0; i < paths.Length; i++)
        {
            TextureImporter importer = AssetImporter.GetAtPath(paths[i]) as TextureImporter;
            if (importer == null)
                throw new InvalidOperationException("No se pudo importar blueprint: " + paths[i]);
            bool changed = importer.textureType != TextureImporterType.Sprite ||
                           importer.spriteImportMode != SpriteImportMode.Single ||
                           importer.mipmapEnabled || importer.textureCompression != TextureImporterCompression.Uncompressed ||
                           importer.maxTextureSize < 2048;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.sRGBTexture = true;
            importer.alphaIsTransparency = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = 2048;
            if (changed) importer.SaveAndReimport();
            sprites[i] = AssetDatabase.LoadAssetAtPath<Sprite>(paths[i]);
            if (sprites[i] == null)
                throw new InvalidOperationException("Blueprint no disponible como Sprite: " + paths[i]);
        }
        return sprites;
    }

    private static Material LoadOrCreateBlueprintMaterial()
    {
        Shader shader = Shader.Find("UI/QuantumForgeBlueprintKeyed");
        if (shader == null) throw new InvalidOperationException("No se encontró el shader de blueprint del Hangar.");
        Material material = AssetDatabase.LoadAssetAtPath<Material>(BlueprintMaterialPath);
        if (material == null)
        {
            material = new Material(shader) { name = "D1 Hangar Blueprint Keyed" };
            AssetDatabase.CreateAsset(material, BlueprintMaterialPath);
        }
        else if (material.shader != shader)
        {
            material.shader = shader;
            EditorUtility.SetDirty(material);
        }
        return material;
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
        if (root.GetComponent<Dimension1HangarVisualUI>() == null) throw new InvalidOperationException("Falta controlador visual del Hangar.");
        if (root.GetComponent<Canvas>() == null || root.GetComponent<GraphicRaycaster>() == null)
            throw new InvalidOperationException("Falta overlay interactivo del Hangar.");
        string[] required = { "DimensionTitle", "MainHeading", "ShipCard_0", "ShipCard_3", "ShipDisplay", "PartCard_0", "PartCard_3", "UpgradeButton", "MissionBonus", "BottomNavigation" };
        foreach (string name in required)
            if (FindChild(root,name) == null) throw new InvalidOperationException("Falta bloque visual: " + name);
        if (FindChild(root,"Nav_HANGAR")?.GetComponent<Button>() == null)
            throw new InvalidOperationException("Falta navegación Hangar.");
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

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

public static class Dimension1AncientOrbitsSetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ArtPath = "Assets/Project/UI/Dimension1/Generated";
    private const string MetalPath = ArtPath + "/MetalsInventory";
    private const string DestinationPath = ArtPath + "/Candidates/AncientOrbits";
    private const string RootName = "D1_AncientOrbitsVisualRoot";
    private const string FontPath = "Assets/Project/UI/Vertical/Fonts/Rajdhani-Medium SDF.asset";
    private const float W = 1080f;
    private const float H = 1920f;

    private static readonly Color Void = Hex("01090E");
    private static readonly Color Fill = Hex("04121B", 250);
    private static readonly Color FillRaised = Hex("071924", 252);
    private static readonly Color Cyan = Hex("18C8FF");
    private static readonly Color CyanBright = Hex("63DCFF");
    private static readonly Color CyanMuted = Hex("087FA9");
    private static readonly Color Primary = Hex("EDF4F7");
    private static readonly Color Secondary = Hex("9EABB4");
    private static readonly Color Amber = Hex("F4A70B");
    private static readonly Color AmberFill = Hex("1A1508", 252);

    private sealed class BuildRefs
    {
        public Button back;
        public Button backToMap;
        public Button allMetals;
        public readonly List<TMP_Text> amounts = new List<TMP_Text>();
        public readonly List<TMP_Text> rates = new List<TMP_Text>();
        public Dimension1AncientOrbitsUI.PlanetCardView planet4;
        public Dimension1AncientOrbitsUI.PlanetCardView planet5;
        public readonly List<Button> destinations = new List<Button>();
        public readonly List<TMP_Text> destinationStatuses = new List<TMP_Text>();
        public readonly List<Button> navigation = new List<Button>();
    }

    [MenuItem("Quantum Forge/Dimension 1/Install Ancient Orbits Subscreen")]
    public static void Install()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Dimension1PanelUI panel = FindSceneComponent<Dimension1PanelUI>(scene);
        Dimension1CommandCenterUI commandCenter = FindSceneComponent<Dimension1CommandCenterUI>(scene);
        Dimension1MetalsInventoryUI metals = FindSceneComponent<Dimension1MetalsInventoryUI>(scene);
        Transform galaxyPanel = FindSceneTransform(scene, "GalaxyPanel");
        if (panel == null || commandCenter == null || metals == null || galaxyPanel == null)
            throw new InvalidOperationException("Faltan propietarios reales de la subpantalla Órbitas Antiguas.");

        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        Sprite frame = LoadSprite(ArtPath + "/d1_premium_frame_v4.png");
        Sprite fill = LoadSprite(ArtPath + "/d1_panel_fill_v4.png");
        Sprite starfield = LoadSprite(ArtPath + "/d1_starfield.png");
        string[] importPaths =
        {
            ArtPath + "/d1_body_planet_ancient_v3.png",
            ArtPath + "/d1_body_planet_silent_v3.png",
            MetalPath + "/d1_metal_iron_v5.png",
            MetalPath + "/d1_metal_aluminum_v5.png",
            MetalPath + "/d1_metal_nickel_v5.png",
            MetalPath + "/d1_metal_lithium_v5.png",
            MetalPath + "/d1_metal_platinum_v5.png"
        };
        Sprite planet4 = LoadSprite(importPaths[0]);
        Sprite planet5 = LoadSprite(importPaths[1]);
        Sprite[] headers = { LoadSprite(importPaths[2]), LoadSprite(importPaths[3]), LoadSprite(importPaths[4]) };
        Sprite[] production = { LoadSprite(importPaths[5]), LoadSprite(importPaths[6]) };
        if (font == null || frame == null || fill == null || starfield == null ||
            planet4 == null || planet5 == null || HasNull(headers) || HasNull(production))
            throw new InvalidOperationException("Faltan assets canónicos de Órbitas Antiguas.");

        Transform previous = FindSceneTransform(scene, RootName);
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
        canvas.sortingOrder = 32750;
        root.gameObject.AddComponent<GraphicRaycaster>();
        CanvasGroup group = root.gameObject.AddComponent<CanvasGroup>();
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;
        Dimension1AncientOrbitsUI visual = root.gameObject.AddComponent<Dimension1AncientOrbitsUI>();

        Image background = Image("Background", root, null, Void);
        Stretch(background.rectTransform, new Vector2(-22f, -26f), new Vector2(-22f, -26f));
        background.raycastTarget = false;
        Image stars = Image("Starfield", root, starfield, Hex("6DC9E6", 10));
        Stretch(stars.rectTransform);
        Image outer = Image("OuterFrame", root, frame, Hex("087FA9", 220));
        Top(outer.rectTransform, 8f, 8f, 1064f, 1904f);
        outer.type = UnityEngine.UI.Image.Type.Sliced;

        BuildRefs refs = new BuildRefs();
        BuildHeading(root, frame, fill, font, refs);
        BuildResources(root, frame, fill, font, headers, refs);
        refs.planet4 = BuildPlanet(root, "Planet4", frame, fill, starfield, planet4,
            production[0], font, new Vector2(26f, 258f), "PLANETA 4", "LITIO / TUNGSTENO",
            Dimension1System.Planet04, Dimension1System.MetalLithium, Dimension1System.MetalTungsten);
        refs.planet5 = BuildPlanet(root, "Planet5", frame, fill, starfield, planet5,
            production[1], font, new Vector2(26f, 766f), "PLANETA 5", "PLATINO / NÍQUEL",
            Dimension1System.Planet05, Dimension1System.MetalPlatinum, Dimension1System.MetalNickel);
        BuildBackToMap(root, frame, fill, font, refs);
        BuildNavigation(root, frame, fill, font, refs);

        visual.Configure(panel, commandCenter, metals, group, refs.back, refs.backToMap,
            refs.allMetals, refs.amounts.ToArray(), refs.rates.ToArray(), refs.planet4, refs.planet5,
            refs.destinations.ToArray(), refs.destinationStatuses.ToArray(), refs.navigation.ToArray());
        AddPersistent(refs.back.onClick, visual.CloseToGalaxy);
        AddPersistent(refs.backToMap.onClick, visual.CloseToGalaxy);
        AddPersistent(refs.allMetals.onClick, visual.OpenMetals);
        AddPersistent(refs.planet4.actionButton.onClick, visual.ActOnPlanet4);
        AddPersistent(refs.planet5.actionButton.onClick, visual.ActOnPlanet5);
        AddPersistent(refs.navigation[0].onClick, visual.OpenGalaxy);
        AddPersistent(refs.navigation[1].onClick, visual.OpenExplore);
        AddPersistent(refs.navigation[2].onClick, visual.OpenHangar);
        AddPersistent(refs.navigation[3].onClick, visual.OpenRelics);
        AddPersistent(refs.navigation[4].onClick, visual.OpenTree);

        SerializedObject panelSerialized = new SerializedObject(panel);
        SerializedProperty ancient = panelSerialized.FindProperty("ancientOrbitsUI");
        if (ancient == null) throw new InvalidOperationException("Falta el enlace ancientOrbitsUI.");
        ancient.objectReferenceValue = visual;
        panelSerialized.ApplyModifiedPropertiesWithoutUndo();

        // El fondo global es puramente decorativo. Si conserva raycastTarget,
        // puede quedar por encima de pantallas verticales al cambiar resolución.
        Image globalBackground = FindSceneTransform(scene, "BackgroundMobile")?.GetComponent<Image>();
        if (globalBackground != null)
        {
            globalBackground.raycastTarget = false;
            EditorUtility.SetDirty(globalBackground);
        }

        Dimension1SharedShellApply.ApplyToRoot(root);
        EditorUtility.SetDirty(visual);
        EditorUtility.SetDirty(panel);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("Unity no pudo guardar Main.unity.");
        ValidateInternal(scene);
        Debug.Log("[D1 Ancient Orbits] INSTALL_PASS | Sector 3 subpantalla | datos reales | 4 destinos");
    }

    [MenuItem("Quantum Forge/Dimension 1/Validate Ancient Orbits Subscreen")]
    public static void Validate()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        ValidateInternal(scene);
        Debug.Log("[D1 Ancient Orbits] VALIDATION_PASS");
    }

    private static void BuildHeading(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font, BuildRefs refs)
    {
        RectTransform heading = Panel("Heading", root, frame, fill, new Vector2(18f, 16f),
            new Vector2(1044f, 122f), Fill, CyanMuted, out _, out _);
        refs.back = ButtonPanel("BackButton", heading, frame, fill, new Vector2(16f, 20f),
            new Vector2(88f, 80f), FillRaised, CyanMuted, out _, out _);
        Segment(refs.back.transform, "ArrowShaft", new Vector2(-10f, 0f), new Vector2(18f, 0f), 5f, CyanBright);
        Segment(refs.back.transform, "ArrowUpper", new Vector2(-10f, 0f), new Vector2(4f, 15f), 5f, CyanBright);
        Segment(refs.back.transform, "ArrowLower", new Vector2(-10f, 0f), new Vector2(4f, -15f), 5f, CyanBright);
        TMP_Text title = Text("Title", heading, font, "ÓRBITAS ANTIGUAS", 42f, FontStyles.Bold, Primary);
        Top(title.rectTransform, 124f, 12f, 796f, 56f);
        title.alignment = TextAlignmentOptions.Center;
        title.characterSpacing = 2f;
        TMP_Text subtitle = Text("Subtitle", heading, font, "GALAXIA / SECTOR 3", 25f, FontStyles.Normal, Cyan);
        Top(subtitle.rectTransform, 224f, 67f, 596f, 39f);
        subtitle.alignment = TextAlignmentOptions.Center;
        subtitle.characterSpacing = 1.5f;
    }

    private static void BuildResources(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font,
        Sprite[] sprites, BuildRefs refs)
    {
        string[] names = { "HIERRO", "ALUMINIO", "NÍQUEL" };
        float[] xs = { 26f, 282f, 538f };
        for (int i = 0; i < 3; i++)
        {
            RectTransform card = Panel("Metal_" + i, root, frame, fill, new Vector2(xs[i], 148f),
                new Vector2(240f, 88f), FillRaised, CyanMuted, out _, out _);
            Image icon = Image("Icon", card, sprites[i], Color.white);
            Top(icon.rectTransform, 10f, 9f, 66f, 66f);
            icon.preserveAspect = true;
            TMP_Text name = Text("Name", card, font, names[i], 17f, FontStyles.Normal, Secondary);
            Top(name.rectTransform, 76f, 8f, 144f, 26f);
            TMP_Text amount = Text("Amount", card, font, "0", 29f, FontStyles.Normal, Primary);
            Top(amount.rectTransform, 76f, 30f, 112f, 41f);
            TMP_Text rate = Text("Rate", card, font, "+0/s", 16f, FontStyles.Bold, Cyan);
            Top(rate.rectTransform, 158f, 55f, 70f, 24f);
            rate.alignment = TextAlignmentOptions.Right;
            refs.amounts.Add(amount);
            refs.rates.Add(rate);
        }
        refs.allMetals = ButtonPanel("AllMetals", root, frame, fill, new Vector2(794f, 148f),
            new Vector2(260f, 88f), FillRaised, CyanMuted, out _, out _);
        DrawMetalCluster(refs.allMetals.transform, new Vector2(-94f, 0f));
        TMP_Text label = Text("Label", refs.allMetals.transform, font, "10 METALES", 23f, FontStyles.Bold, Cyan);
        Top(label.rectTransform, 66f, 23f, 158f, 40f);
        label.alignment = TextAlignmentOptions.Center;
        Segment(refs.allMetals.transform, "ChevronA", new Vector2(100f, 7f), new Vector2(108f, -1f), 3f, Cyan);
        Segment(refs.allMetals.transform, "ChevronB", new Vector2(108f, -1f), new Vector2(116f, 7f), 3f, Cyan);
    }

    private static Dimension1AncientOrbitsUI.PlanetCardView BuildPlanet(
        Transform root, string objectName, Sprite frame, Sprite fill, Sprite starfield,
        Sprite planetSprite, Sprite productionSprite, TMP_FontAsset font, Vector2 position,
        string titleValue, string metalsValue, string planetId, string primaryMetal, string secondaryMetal)
    {
        RectTransform card = Panel(objectName, root, frame, fill, position, new Vector2(1028f, 490f),
            Fill, CyanMuted, out _, out _);
        RectTransform well = Panel("PlanetWell", card, frame, fill, new Vector2(14f, 14f),
            new Vector2(390f, 462f), Void, CyanMuted, out _, out _);
        Image wellStars = Image("Starfield", well, starfield, Hex("B8E9FF", 175));
        Stretch(wellStars.rectTransform, new Vector2(6f, 6f), new Vector2(6f, 6f));
        Image planet = Image("PlanetArt", well, planetSprite, Color.white);
        Center(planet.rectTransform, Vector2.zero, new Vector2(342f, 342f));
        planet.preserveAspect = true;

        TMP_Text title = Text("PlanetTitle", card, font, titleValue, 40f, FontStyles.Bold, Primary);
        Top(title.rectTransform, 438f, 28f, 520f, 54f);
        TMP_Text metals = Text("Metals", card, font, metalsValue, 27f, FontStyles.Normal, Cyan);
        Top(metals.rectTransform, 438f, 78f, 520f, 42f);
        LineTop(card, "Line0", 438f, 125f, 542f);

        TMP_Text levelLabel = Text("LevelLabel", card, font, "EXTRACTOR NIVEL", 25f, FontStyles.Normal, Secondary);
        Top(levelLabel.rectTransform, 438f, 146f, 300f, 38f);
        RectTransform badge = Panel("LevelBadge", card, frame, fill, new Vector2(785f, 139f),
            new Vector2(118f, 54f), FillRaised, CyanMuted, out _, out _);
        TMP_Text level = Text("LevelValue", badge, font, "0", 30f, FontStyles.Bold, Primary);
        Stretch(level.rectTransform, new Vector2(8f, 4f), new Vector2(8f, 4f));
        level.alignment = TextAlignmentOptions.Center;
        LineTop(card, "Line1", 438f, 204f, 542f);

        TMP_Text productionLabel = Text("ProductionLabel", card, font, "PRODUCCIÓN", 25f,
            FontStyles.Normal, Secondary);
        Top(productionLabel.rectTransform, 438f, 222f, 260f, 38f);
        Image productionIcon = Image("ProductionMetal", card, productionSprite, Color.white);
        Top(productionIcon.rectTransform, 760f, 214f, 52f, 52f);
        productionIcon.preserveAspect = true;
        TMP_Text production = Text("ProductionValue", card, font, "+0/s", 30f, FontStyles.Bold, Amber);
        Top(production.rectTransform, 814f, 218f, 164f, 43f);
        production.alignment = TextAlignmentOptions.Right;
        LineTop(card, "Line2", 438f, 274f, 542f);

        TMP_Text extractor = Text("ExtractorLabel", card, font, "EXTRACTOR", 25f,
            FontStyles.Normal, Secondary);
        Top(extractor.rectTransform, 438f, 291f, 260f, 38f);
        TMP_Text percent = Text("ProgressValue", card, font, "0%", 27f, FontStyles.Bold, Cyan);
        Top(percent.rectTransform, 820f, 291f, 158f, 38f);
        percent.alignment = TextAlignmentOptions.Right;
        Image barBg = Image("ProgressBackground", card, null, Hex("28343C"));
        Top(barBg.rectTransform, 438f, 337f, 540f, 15f);
        Image barFill = Image("ProgressFill", barBg.transform, null, Hex("47BCE7"));
        Stretch(barFill.rectTransform);
        barFill.rectTransform.anchorMax = new Vector2(.2f, 1f);
        barFill.rectTransform.offsetMax = Vector2.zero;

        Button action = ButtonPanel("ActionButton", card, frame, fill, new Vector2(422f, 374f),
            new Vector2(572f, 94f), AmberFill, Amber, out _, out _);
        TMP_Text actionLabel = Text("ActionLabel", action.transform, font, "MEJORAR EXTRACTOR", 27f,
            FontStyles.Bold, Amber);
        Top(actionLabel.rectTransform, 20f, 15f, 532f, 37f);
        actionLabel.alignment = TextAlignmentOptions.Center;
        TMP_Text cost = Text("Cost", action.transform, font, "0 METAL", 18f, FontStyles.Normal, Hex("FFD15E"));
        Top(cost.rectTransform, 20f, 51f, 532f, 28f);
        cost.alignment = TextAlignmentOptions.Center;

        return new Dimension1AncientOrbitsUI.PlanetCardView
        {
            planetId = planetId,
            primaryMetalId = primaryMetal,
            secondaryMetalId = secondaryMetal,
            levelText = level,
            productionText = production,
            progressText = percent,
            progressFill = barFill,
            actionText = actionLabel,
            costText = cost,
            actionButton = action
        };
    }

    private static void BuildDestinations(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font,
        Sprite[] sprites, BuildRefs refs)
    {
        RectTransform panel = Panel("Destinations", root, frame, fill, new Vector2(26f, 1274f),
            new Vector2(1028f, 242f), Fill, CyanMuted, out _, out _);
        Segment(panel, "TitleLineL", new Vector2(-392f, 96f), new Vector2(-210f, 96f), 2f, CyanMuted);
        Segment(panel, "TitleLineR", new Vector2(210f, 96f), new Vector2(392f, 96f), 2f, CyanMuted);
        TMP_Text title = Text("Title", panel, font, "DESTINOS DEL SECTOR", 25f, FontStyles.Normal, Cyan);
        Top(title.rectTransform, 286f, 10f, 456f, 38f);
        title.alignment = TextAlignmentOptions.Center;
        title.characterSpacing = 1.3f;
        string[] names = { "NAVE\nABANDONADA", "RUINA\nORBITAL", "LABORATORIO", "ESTACIÓN\nABANDONADA" };
        for (int i = 0; i < 4; i++)
        {
            Button button = ButtonPanel("Destination_" + i, panel, frame, fill,
                new Vector2(25f + i * 247f, 51f), new Vector2(226f, 174f), FillRaised, CyanMuted,
                out _, out _);
            Image icon = Image("Icon", button.transform, sprites[i], Color.white);
            Top(icon.rectTransform, 43f, 5f, 140f, 112f);
            icon.preserveAspect = true;
            TMP_Text label = Text("Label", button.transform, font, names[i], 20f, FontStyles.Normal, Secondary);
            Top(label.rectTransform, 10f, 114f, 206f, 52f);
            label.textWrappingMode = TextWrappingModes.Normal;
            label.overflowMode = TextOverflowModes.Overflow;
            label.alignment = TextAlignmentOptions.Center;
            TMP_Text status = Text("Status", button.transform, font, "NO ESCANEADO", 17f,
                FontStyles.Bold, Amber);
            Top(status.rectTransform, 12f, 86f, 202f, 28f);
            status.alignment = TextAlignmentOptions.Center;
            refs.destinations.Add(button);
            refs.destinationStatuses.Add(status);
        }
    }

    private static void BuildBackToMap(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font, BuildRefs refs)
    {
        refs.backToMap = ButtonPanel("BackToGalaxyMap", root, frame, fill,
            new Vector2(240f, 1314f), new Vector2(600f, 92f), FillRaised, CyanMuted, out _, out _);
        Segment(refs.backToMap.transform, "ArrowShaft", new Vector2(-242f, 0f), new Vector2(-222f, 0f), 4f, Cyan);
        Segment(refs.backToMap.transform, "ArrowUpper", new Vector2(-242f, 0f), new Vector2(-232f, 10f), 4f, Cyan);
        Segment(refs.backToMap.transform, "ArrowLower", new Vector2(-242f, 0f), new Vector2(-232f, -10f), 4f, Cyan);
        TMP_Text label = Text("Label", refs.backToMap.transform, font, "VOLVER A CARTA GALÁCTICA", 25f,
            FontStyles.Normal, Cyan);
        Stretch(label.rectTransform, new Vector2(54f, 10f), new Vector2(24f, 10f));
        label.alignment = TextAlignmentOptions.Center;
        label.characterSpacing = 1.4f;
    }

    private static void BuildNavigation(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font, BuildRefs refs)
    {
        RectTransform nav = Rect("BottomNavigation", root);
        Top(nav, Dimension1SharedLayoutTokens.NavigationX, Dimension1SharedLayoutTokens.NavigationY,
            Dimension1SharedLayoutTokens.NavigationWidth, Dimension1SharedLayoutTokens.NavigationHeight);
        string[] labels = { "GALAXIA", "EXPLORAR", "HANGAR", "RELIQUIAS", "ÁRBOL" };
        for (int i = 0; i < 5; i++)
        {
            bool selected = i == 0;
            Button button = ButtonPanel("Nav_" + labels[i], nav, frame, fill,
                new Vector2(Dimension1SharedLayoutTokens.NavigationCardX(i),
                    Dimension1SharedLayoutTokens.NavigationCardY),
                new Vector2(Dimension1SharedLayoutTokens.NavigationCardWidth,
                    Dimension1SharedLayoutTokens.NavigationCardHeight),
                selected ? AmberFill : Fill, selected ? Amber : CyanMuted, out _, out _);
            TMP_Text label = Text("Label", button.transform, font, labels[i], 22f, FontStyles.Bold,
                selected ? Amber : Cyan);
            Top(label.rectTransform, 10f, 118f, 178f, 34f);
            label.alignment = TextAlignmentOptions.Center;
            refs.navigation.Add(button);
        }
        Image pointer = Image("SelectedPointer", nav, null, Amber);
        Top(pointer.rectTransform, 83f, -13f, 30f, 18f);
    }

    private static void ValidateInternal(Scene scene)
    {
        Transform root = FindSceneTransform(scene, RootName);
        if (root == null || root.GetComponent<Dimension1AncientOrbitsUI>() == null)
            throw new InvalidOperationException("Falta la raíz o controlador de Órbitas Antiguas.");
        string[] required =
        {
            "Heading", "AllMetals", "Planet4", "Planet5",
            "BackToGalaxyMap", "BottomNavigation"
        };
        foreach (string name in required)
            if (FindChild(root, name) == null) throw new InvalidOperationException("Falta " + name + ".");
        if (root.GetComponentsInChildren<Button>(true).Length < 9)
            throw new InvalidOperationException("Faltan interacciones de Órbitas Antiguas.");

        Image background = FindDirectChild(root, "Background")?.GetComponent<Image>();
        if (background == null || background.raycastTarget)
            throw new InvalidOperationException("El fondo de Órbitas Antiguas no puede interceptar clics.");

        ValidateCenteredPlanet(root, "Planet4");
        ValidateCenteredPlanet(root, "Planet5");
    }

    private static void ValidateCenteredPlanet(Transform root, string cardName)
    {
        Transform card = FindDirectChild(root, cardName);
        Transform well = FindDirectChild(card, "PlanetWell");
        RectTransform art = FindDirectChild(well, "PlanetArt") as RectTransform;
        if (art == null || art.anchorMin != new Vector2(.5f, .5f) ||
            art.anchorMax != new Vector2(.5f, .5f) || art.pivot != new Vector2(.5f, .5f) ||
            art.anchoredPosition.sqrMagnitude > .0001f)
        {
            throw new InvalidOperationException("El planeta no está centrado dentro de " + cardName + ".");
        }
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
        Vector2 topLeft, Vector2 size, Color fillColor, Color borderColor,
        out Image fillImage, out Image borderImage)
    {
        RectTransform rect = Panel(name, parent, frame, fill, topLeft, size, fillColor, borderColor,
            out fillImage, out borderImage);
        Image hit = rect.gameObject.AddComponent<Image>();
        hit.color = Color.clear;
        hit.raycastTarget = true;
        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = hit;
        button.transition = Selectable.Transition.ColorTint;
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(1f, 1f, 1f, .93f);
        colors.pressedColor = new Color(.65f, .88f, 1f, .82f);
        colors.disabledColor = new Color(.42f, .48f, .52f, .62f);
        button.colors = colors;
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

    private static void Center(RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
        rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    private static void Stretch(RectTransform rect, Vector2 insetMin = default, Vector2 insetMax = default)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(.5f, .5f);
        rect.offsetMin = insetMin;
        rect.offsetMax = -insetMax;
    }

    private static void Segment(Transform parent, string name, Vector2 a, Vector2 b, float thickness, Color color)
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

    private static void LineTop(Transform parent, string name, float x, float y, float width)
    {
        Image line = Image(name, parent, null, Hex("087FA9", 110));
        Top(line.rectTransform, x, y, width, 2f);
    }

    private static void DrawMetalCluster(Transform parent, Vector2 center)
    {
        Vector2[] offsets =
        {
            new Vector2(-12f, 7f), new Vector2(0f, 15f), new Vector2(12f, 7f),
            new Vector2(-12f, -8f), new Vector2(0f, 0f), new Vector2(12f, -8f)
        };
        for (int i = 0; i < offsets.Length; i++)
        {
            TMP_Text dot = Text("MetalDot_" + i, parent,
                AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath), "○", 23f,
                FontStyles.Bold, i == 4 ? CyanBright : Cyan);
            dot.rectTransform.anchorMin = dot.rectTransform.anchorMax = new Vector2(.5f, .5f);
            dot.rectTransform.pivot = new Vector2(.5f, .5f);
            dot.rectTransform.anchoredPosition = center + offsets[i];
            dot.rectTransform.sizeDelta = new Vector2(25f, 25f);
            dot.alignment = TextAlignmentOptions.Center;
        }
    }

    private static void PrepareSprite(string path)
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
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        importer.maxTextureSize = 1024;
        importer.SaveAndReimport();
    }

    private static Sprite LoadSprite(string path) => AssetDatabase.LoadAssetAtPath<Sprite>(path);

    private static bool HasNull(Sprite[] sprites)
    {
        foreach (Sprite sprite in sprites) if (sprite == null) return true;
        return false;
    }

    private static void AddPersistent(UnityEngine.Events.UnityEvent value, UnityEngine.Events.UnityAction action)
    {
        UnityEventTools.AddPersistentListener(value, action);
    }

    private static T FindSceneComponent<T>(Scene scene) where T : Component
    {
        foreach (GameObject sceneRoot in scene.GetRootGameObjects())
        {
            T found = sceneRoot.GetComponentInChildren<T>(true);
            if (found != null) return found;
        }
        return null;
    }

    private static Transform FindSceneTransform(Scene scene, string name)
    {
        foreach (GameObject sceneRoot in scene.GetRootGameObjects())
            foreach (Transform child in sceneRoot.GetComponentsInChildren<Transform>(true))
                if (child.name == name) return child;
        return null;
    }

    private static Transform FindDirectChild(Transform parent, string name)
    {
        if (parent == null) return null;
        for (int i = 0; i < parent.childCount; i++)
            if (parent.GetChild(i).name == name) return parent.GetChild(i);
        return null;
    }

    private static Transform FindChild(Transform parent, string name)
    {
        if (parent == null) return null;
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
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

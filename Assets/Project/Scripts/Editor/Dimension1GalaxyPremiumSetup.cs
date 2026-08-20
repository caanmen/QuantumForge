#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static partial class Dimension1GalaxyPremiumSetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ArtPath = "Assets/Project/UI/Dimension1/Generated";
    private const string RootName = "D1_GalaxyVisualRoot";
    private const string LayeredGalaxyPath = ArtPath + "/GalaxyLayeredV10";
    private const string GalaxyArtPath = LayeredGalaxyPath + "/d1_galaxy_v10_base.png";
    private const string GalaxyArmsPath = LayeredGalaxyPath + "/d1_galaxy_v10_arms_open.png";
    private const string GalaxyDustPath = LayeredGalaxyPath + "/d1_galaxy_v10_dust.png";
    private const string GalaxyCorePath = LayeredGalaxyPath + "/d1_galaxy_v10_core.png";
    private const string GalaxyMaterialPath = "Assets/Project/Materials/UI/D1GalaxyLayeredV10.mat";

    private static readonly Color Void = Hex("03070D");
    private static readonly Color Fill = Hex("07111A", 242);
    private static readonly Color FillRaised = Hex("0D1A25", 248);
    private static readonly Color Cyan = Hex("55CFFF");
    private static readonly Color Amber = Hex("F2B344");
    private static readonly Color Red = Hex("FF5B58");
    private static readonly Color Primary = Hex("EFF8FC");
    private static readonly Color Secondary = Hex("94ADBC");

    [MenuItem("Quantum Forge/Dimension 1/Configure Layered Galaxy V10")]
    public static void Configure()
    {
        ConfigureFinalArt();

        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Dimension1PanelUI panel = FindSceneComponent<Dimension1PanelUI>(scene);
        if (panel == null) throw new InvalidOperationException("Dimension1PanelUI no existe.");
        Transform galaxyPanel = FindChild(panel.transform, "GalaxyPanel");
        if (galaxyPanel == null) throw new InvalidOperationException("GalaxyPanel no existe.");

        Stretch((RectTransform)galaxyPanel);
        Transform old = FindDirectChild(galaxyPanel, RootName);
        if (old != null) UnityEngine.Object.DestroyImmediate(old.gameObject);
        DisableLegacyGalaxyChildren(galaxyPanel);

        Sprite frame = LoadSprite(ArtPath + "/d1_premium_frame_v4.png");
        Sprite orbit = LoadSprite(ArtPath + "/d1_orbit_ring_v3.png");
        Sprite glow = LoadSprite(ArtPath + "/d1_glow_v3.png");
        Sprite lockSprite = LoadSprite(ArtPath + "/d1_lock_v3.png");
        Sprite starfieldSprite = LoadSprite(ArtPath + "/d1_starfield.png");
        Sprite galaxy = LoadSprite(GalaxyArtPath);
        Material galaxyMaterial = GetOrCreateGalaxyMaterial();
        Dictionary<string, Sprite> bodies = LoadCelestialSprites();
        TMP_FontAsset font = FindFont(panel.transform);

        RectTransform root = CreateRect(RootName, galaxyPanel, Vector2.zero, new Vector2(1080f, 1920f));
        Stretch(root);
        root.gameObject.AddComponent<Dimension1VisualSkinRoot>();
        Canvas overlay = root.gameObject.AddComponent<Canvas>();
        overlay.overrideSorting = true;
        overlay.sortingOrder = 32000;
        root.gameObject.AddComponent<GraphicRaycaster>();
        Dimension1GalaxyVisualUI visual = root.gameObject.AddComponent<Dimension1GalaxyVisualUI>();

        Image baseVoid = CreateImage("VoidBase", root, null, Void);
        Stretch(baseVoid.rectTransform);
        baseVoid.raycastTarget = false;
        CreateVignette(root);

        RectTransform header = CreatePremiumPanel("Header", root, frame,
            new Vector2(8f, -10f), new Vector2(1064f, 118f), Hex("07111A", 248), Cyan);
        TMP_Text title = CreateText("Title", header, font, "CARTA GALÁCTICA", 39f, FontStyles.Bold, Primary);
        SetTopRect(title.rectTransform, 42f, 13f, 500f, 58f);
        title.alignment = TextAlignmentOptions.MidlineLeft;
        title.enableWordWrapping = false;
        TMP_Text currentSector = CreateText("CurrentSector", header, font, "ACTUAL", 15f, FontStyles.Bold, Cyan);
        SetTopRect(currentSector.rectTransform, 46f, 72f, 360f, 27f);
        TMP_Text unlockedSectors = CreateText("UnlockedSectors", header, font, "1/5 SECTORES", 16f, FontStyles.Bold, Amber);
        SetTopRect(unlockedSectors.rectTransform, 412f, 75f, 130f, 25f);
        unlockedSectors.alignment = TextAlignmentOptions.Right;
        unlockedSectors.enableWordWrapping = false;

        var chips = new Dimension1GalaxyVisualUI.MetalChip[3];
        for (int i = 0; i < 3; i++)
            chips[i] = CreateMetalChip(header, frame, font, 558f + i * 164f);

        // Background and motion live only inside the map viewport. RectMask2D
        // keeps them away from the header and the lower information panel.
        RectTransform map = CreateRect("FullBleedGalaxyMap", root, new Vector2(0f, -128f), new Vector2(1080f, 1005f));
        SetTopLeft(map, new Vector2(0f, -128f), new Vector2(1080f, 1005f));
        map.gameObject.AddComponent<RectMask2D>();
        Image mapVoid = CreateImage("MapVoid", map, null, Void);
        Stretch(mapVoid.rectTransform);
        mapVoid.raycastTarget = false;
        Image starfield = CreateImage("StaticStarfield", map, starfieldSprite, Hex("B7DCE8", 92));
        Stretch(starfield.rectTransform);
        starfield.preserveAspect = false;
        starfield.raycastTarget = false;
        Image animatedGalaxy = CreateImage("GalaxyAnimated", map, galaxy, Hex("FFFFFF", 238));
        CenterAt(animatedGalaxy.rectTransform, new Vector2(540f, -510f));
        animatedGalaxy.rectTransform.sizeDelta = new Vector2(800f, 800f);
        animatedGalaxy.preserveAspect = true;
        animatedGalaxy.raycastTarget = false;
        animatedGalaxy.material = galaxyMaterial;

        Vector2 s1 = new Vector2(205f, -190f);
        Vector2 s2 = new Vector2(870f, -193f);
        Vector2 center = new Vector2(540f, -500f);
        Vector2 s3 = new Vector2(220f, -790f);
        Vector2 s4 = new Vector2(860f, -790f);
        Image[] noRoutes = Array.Empty<Image>();

        var nodeViews = new Dimension1GalaxyVisualUI.SectorNodeView[5];
        nodeViews[0] = CreateSectorNode("Sector01", map, frame, orbit, glow, lockSprite, bodies["planet_blue"], font,
            Dimension1System.Sector01OuterRim, s1, new Vector2(300f, 245f), noRoutes, false);
        nodeViews[1] = CreateSectorNode("Sector02", map, frame, orbit, glow, lockSprite, bodies["debris_ring"], font,
            Dimension1System.Sector02DebrisRing, s2, new Vector2(310f, 250f), noRoutes, false);
        nodeViews[2] = CreateSectorNode("Sector03", map, frame, orbit, glow, lockSprite, bodies["planet_ancient"], font,
            Dimension1System.Sector03AncientOrbits, s3, new Vector2(330f, 260f), noRoutes, false);
        nodeViews[3] = CreateSectorNode("Sector04", map, frame, orbit, glow, lockSprite, bodies["planet_silent"], font,
            Dimension1System.Sector04SilentFrontier, s4, new Vector2(330f, 260f), noRoutes, false);
        nodeViews[4] = CreateSectorNode("GalacticCenter", map, frame, orbit, glow, lockSprite, bodies["black_hole"], font,
            Dimension1System.Sector05GalacticCenter, center, new Vector2(330f, 280f), Array.Empty<Image>(), true);

        AddPersistent(nodeViews[0].root.GetComponent<Button>().onClick, panel.OnClickPreviewGalaxySector1);
        AddPersistent(nodeViews[1].root.GetComponent<Button>().onClick, panel.OnClickPreviewGalaxySector2);
        AddPersistent(nodeViews[2].root.GetComponent<Button>().onClick, panel.OnClickPreviewGalaxySector3);
        AddPersistent(nodeViews[3].root.GetComponent<Button>().onClick, panel.OnClickPreviewGalaxySector4);
        AddPersistent(nodeViews[4].root.GetComponent<Button>().onClick, panel.OnClickPreviewGalaxyCenter);

        RectTransform details = CreatePremiumPanel("SectorDetails", root, frame,
            new Vector2(28f, -1133f), new Vector2(1024f, 622f), Hex("050D15", 248), Amber);
        SetCenteredParentTop(details, -1133f, new Vector2(1024f, 622f));
        BuildDetailsPanel(details, frame, font, bodies, panel,
            out TMP_Text summaryProxy, out Button enterButton,
            out TMP_Text selectedTitle, out TMP_Text selectedExplorations,
            out TMP_Text selectedStatus, out TMP_Text selectedDestinations,
            out TMP_Text selectedRequirements, out Image selectedPlanet,
            out Image secondaryPlanet, out TMP_Text neutralInstruction,
            out GameObject[] selectedDetailRoots);

        RectTransform navigation = CreatePremiumPanel("D1BottomNavigation", root, frame,
            new Vector2(16f, -1764f), new Vector2(1048f, 148f), Hex("040B12", 252), Cyan);
        SetCenteredParentTop(navigation, -1764f, new Vector2(1048f, 148f));
        CreateBottomNavigation(navigation, frame, font, panel);

        RectTransform closeProxyRect = CreateRect("CloseGalaxyProxy", root, Vector2.zero, Vector2.one);
        SetTopRect(closeProxyRect, 0f, 0f, 1f, 1f);
        Image closeProxyImage = closeProxyRect.gameObject.AddComponent<Image>();
        closeProxyImage.color = Color.clear;
        Button closeProxy = closeProxyRect.gameObject.AddComponent<Button>();
        closeProxy.targetGraphic = closeProxyImage;

        TMP_Text titleProxy = CreateText("GalaxyTitleProxy", root, font, "", 1f, FontStyles.Normal, Color.clear);
        SetTopRect(titleProxy.rectTransform, 0f, 0f, 1f, 1f);

        SerializedObject serializedPanel = new SerializedObject(panel);
        Assign(serializedPanel, "galaxyTitleText", titleProxy);
        Assign(serializedPanel, "galaxySectorSummaryText", summaryProxy);
        Assign(serializedPanel, "galaxySector1Button", nodeViews[0].root.GetComponent<Button>());
        Assign(serializedPanel, "galaxySector2Button", nodeViews[1].root.GetComponent<Button>());
        Assign(serializedPanel, "galaxySector3Button", nodeViews[2].root.GetComponent<Button>());
        Assign(serializedPanel, "galaxySector4Button", nodeViews[3].root.GetComponent<Button>());
        Assign(serializedPanel, "galaxyCenterButton", nodeViews[4].root.GetComponent<Button>());
        Assign(serializedPanel, "enterGalaxySectorButton", enterButton);
        Assign(serializedPanel, "closeGalaxyPanelButton", closeProxy);
        serializedPanel.ApplyModifiedPropertiesWithoutUndo();

        SerializedObject serializedVisual = new SerializedObject(visual);
        Assign(serializedVisual, "panel", panel);
        Assign(serializedVisual, "currentSectorText", currentSector);
        Assign(serializedVisual, "unlockedSectorText", unlockedSectors);
        Assign(serializedVisual, "selectedTitleText", selectedTitle);
        Assign(serializedVisual, "selectedExplorationsText", selectedExplorations);
        Assign(serializedVisual, "selectedStatusText", selectedStatus);
        Assign(serializedVisual, "selectedDestinationsText", selectedDestinations);
        Assign(serializedVisual, "selectedRequirementsText", selectedRequirements);
        Assign(serializedVisual, "selectedPlanetPreview", selectedPlanet);
        Assign(serializedVisual, "secondaryPlanetPreview", secondaryPlanet);
        Assign(serializedVisual, "animatedGalaxyImage", animatedGalaxy);
        Assign(serializedVisual, "neutralInstructionText", neutralInstruction);
        SetObjectArray(serializedVisual, "selectedDetailRoots", selectedDetailRoots);
        SetObjectArray(serializedVisual, "hideWhileOpen", FindNavigationRoots(scene));
        SetObjectArray(serializedVisual, "routeLines", Array.Empty<Image>());
        // The technical rings remain fixed. Motion belongs to the selected
        // celestial body so the map frame never appears to wobble.
        SetObjectArray(serializedVisual, "rotatingBodies", System.Array.Empty<RectTransform>());
        SetObjectArray(serializedVisual, "driftingAsteroids", Array.Empty<RectTransform>());
        SetMetalChipArray(serializedVisual, chips);
        SetSectorNodeArray(serializedVisual, nodeViews);
        serializedVisual.ApplyModifiedPropertiesWithoutUndo();

        EditorUtility.SetDirty(panel);
        EditorUtility.SetDirty(visual);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("No se pudo guardar Main.unity.");

        Validate(scene);
        Debug.Log("[D1 Layered Galaxy V10] CONFIGURATION_PASS | brazos continuos | polvo contrario | sin particulas | panel centrado");
    }

    private static void BuildDetailsPanel(
        RectTransform details, Sprite frame, TMP_FontAsset font,
        Dictionary<string, Sprite> bodies, Dimension1PanelUI panel,
        out TMP_Text summaryProxy, out Button enterButton,
        out TMP_Text selectedTitle, out TMP_Text selectedExplorations,
        out TMP_Text selectedStatus, out TMP_Text selectedDestinations,
        out TMP_Text selectedRequirements, out Image selectedPlanet,
        out Image secondaryPlanet, out TMP_Text neutralInstruction,
        out GameObject[] selectedDetailRoots)
    {
        selectedTitle = CreateText("SelectedSectorTitle", details, font, "BORDE EXTERIOR", 38f, FontStyles.Bold, Amber);
        SetCenteredTopRect(selectedTitle.rectTransform, 252f, 22f, 520f, 55f);
        selectedTitle.alignment = TextAlignmentOptions.Center;

        RectTransform imageWell = CreatePremiumPanel("SectorImage", details, frame,
            new Vector2(28f, -72f), new Vector2(275f, 205f), Hex("07111A", 245), Cyan);
        Image miniNebula = CreateImage("MiniNebula", imageWell, LoadSprite(ArtPath + "/d1_nebula_bg_v3.png"), Hex("B9DAF2", 165));
        Stretch(miniNebula.rectTransform, new Vector2(5f, 5f), new Vector2(-5f, -5f));
        miniNebula.preserveAspect = false;
        selectedPlanet = CreateImage("SelectedPlanet", imageWell, bodies["planet_blue"], Color.white);
        SetCenterRect(selectedPlanet.rectTransform, new Vector2(0f, -4f), new Vector2(182f, 182f));
        selectedPlanet.raycastTarget = false;

        RectTransform stats = CreatePremiumPanel("ExplorationStats", details, frame,
            new Vector2(322f, -88f), new Vector2(420f, 173f), Hex("07111A", 238), Cyan);
        TMP_Text explLabel = CreateText("ExplorationLabel", stats, font, "EXPLORACIONES", 20f, FontStyles.Bold, Secondary);
        SetTopRect(explLabel.rectTransform, 20f, 16f, 240f, 35f);
        selectedExplorations = CreateText("ExplorationCount", stats, font, "0", 58f, FontStyles.Bold, Cyan);
        SetTopRect(selectedExplorations.rectTransform, 20f, 48f, 190f, 88f);
        selectedStatus = CreateText("SelectedStatus", stats, font, "SECTOR DISPONIBLE", 16f, FontStyles.Bold, Amber);
        SetTopRect(selectedStatus.rectTransform, 190f, 65f, 210f, 50f);
        selectedStatus.alignment = TextAlignmentOptions.Center;

        RectTransform planetColumn = CreatePremiumPanel("PlanetColumn", details, frame,
            new Vector2(764f, -72f), new Vector2(248f, 392f), Hex("07111A", 238), Cyan);
        TMP_Text planetsLabel = CreateText("PlanetsLabel", planetColumn, font, "PLANETAS", 21f, FontStyles.Bold, Secondary);
        SetTopRect(planetsLabel.rectTransform, 18f, 12f, 212f, 34f);
        planetsLabel.alignment = TextAlignmentOptions.Center;
        secondaryPlanet = CreateImage("PlanetPreview", planetColumn, bodies["planet_habitable"], Color.white);
        SetTopRect(secondaryPlanet.rectTransform, 38f, 48f, 172f, 172f);
        selectedRequirements = CreateText("Requirements", planetColumn, font, "PLANETAS", 16f, FontStyles.Bold, Primary);
        SetTopRect(selectedRequirements.rectTransform, 18f, 232f, 212f, 135f);
        selectedRequirements.alignment = TextAlignmentOptions.Top;

        RectTransform destinations = CreatePremiumPanel("Destinations", details, frame,
            new Vector2(28f, -298f), new Vector2(714f, 218f), Hex("07111A", 238), Amber);
        TMP_Text destLabel = CreateText("DestinationsLabel", destinations, font, "DESTINOS DISPONIBLES", 19f, FontStyles.Bold, Secondary);
        SetTopRect(destLabel.rectTransform, 20f, 10f, 350f, 34f);
        selectedDestinations = CreateText("DestinationsText", destinations, font,
            "- Cinturón Mineral\n- Cementerio de Naves", 20f, FontStyles.Normal, Primary);
        SetTopRect(selectedDestinations.rectTransform, 20f, 48f, 660f, 150f);
        selectedDestinations.lineSpacing = 15f;

        enterButton = CreateButton("EnterGalaxySectorButtonV3", details, frame, font,
            "ENTRAR AL SECTOR", new Vector2(284f, -526f), new Vector2(472f, 78f), Amber);
        AddPersistent(enterButton.onClick, panel.OnClickEnterGalaxySector);

        neutralInstruction = CreateText("NeutralInstruction", details, font,
            "TOCA UN SECTOR PARA VER SUS DATOS", 21f, FontStyles.Bold, Secondary);
        SetCenteredTopRect(neutralInstruction.rectTransform, 162f, 235f, 700f, 80f);
        neutralInstruction.alignment = TextAlignmentOptions.Center;

        selectedDetailRoots = new[]
        {
            imageWell.gameObject,
            stats.gameObject,
            planetColumn.gameObject,
            destinations.gameObject,
            enterButton.gameObject
        };

        summaryProxy = CreateText("GalaxySectorSummaryTextV3", details, font, "", 1f, FontStyles.Normal, Color.clear);
        SetTopRect(summaryProxy.rectTransform, 1f, 1f, 1f, 1f);
    }

    private static Dimension1GalaxyVisualUI.SectorNodeView CreateSectorNode(
        string name, Transform parent, Sprite frame, Sprite orbit, Sprite glow,
        Sprite lockSprite, Sprite planet, TMP_FontAsset font, string sectorId,
        Vector2 center, Vector2 size, Image[] routes, bool isCenter)
    {
        RectTransform root = CreateRect(name, parent, center, size);
        root.anchorMin = new Vector2(0f, 1f);
        root.anchorMax = new Vector2(0f, 1f);
        root.pivot = new Vector2(0.5f, 0.5f);
        root.anchoredPosition = center;
        root.sizeDelta = size;
        Image hit = root.gameObject.AddComponent<Image>();
        hit.color = Color.clear;
        Button button = root.gameObject.AddComponent<Button>();
        button.targetGraphic = hit;

        // Selection is communicated by the premium label plate and the planet's
        // rotation. No halo or technical ring is created around the body.
        Image glowImage = null;
        Image orbitImage = null;
        Image body = CreateImage("PlanetBody", root, planet, Color.white);
        float bodySize = isCenter ? 186f : 154f;
        // The planet itself rotates when selected. A centered pivot prevents the
        // old floating-ring bug from reappearing on the planet.
        SetCenteredTopRect(body.rectTransform, (size.x - bodySize) * 0.5f, 30f, bodySize, bodySize);
        body.raycastTarget = false;

        RectTransform plate = CreatePremiumPanel("LabelPlate", root, frame,
            new Vector2(10f, -176f), new Vector2(size.x - 20f, isCenter ? 92f : 82f), Hex("07111A", 242), Cyan);
        Image plateFill = FindChild(plate, "Fill").GetComponent<Image>();
        Image plateBorder = FindChild(plate, "Border").GetComponent<Image>();
        TMP_Text label = CreateText("Title", plate, font, GetShortName(sectorId), 20f, FontStyles.Bold, Primary);
        SetTopRect(label.rectTransform, 10f, 7f, size.x - 40f, 34f);
        label.alignment = TextAlignmentOptions.Center;
        label.enableAutoSizing = true;
        label.fontSizeMin = 15f;
        label.fontSizeMax = 21f;
        TMP_Text state = CreateText("State", plate, font, "BLOQUEADO", 14f, FontStyles.Bold, Secondary);
        SetTopRect(state.rectTransform, 10f, 43f, size.x - 40f, 27f);
        state.alignment = TextAlignmentOptions.Center;

        Image lockImage = CreateImage("LockBadge", root, lockSprite, Color.white);
        SetTopRect(lockImage.rectTransform, size.x * 0.5f + 36f, 125f, 42f, 42f);
        lockImage.raycastTarget = false;

        return new Dimension1GalaxyVisualUI.SectorNodeView
        {
            sectorId = sectorId,
            root = root,
            planet = body,
            orbitRing = orbitImage,
            glow = glowImage,
            labelPlate = plateFill,
            labelBorder = plateBorder,
            routes = routes,
            lockBadge = lockImage.gameObject,
            titleText = label,
            stateText = state
        };
    }

    private static Image[] CreateBentRoute(
        string name, Transform parent, Vector2 start, Vector2 bend, Vector2 end,
        Sprite dash, Color color, List<Image> all)
    {
        Image first = CreateRouteSegment(name + "A", parent, start, bend, dash, color);
        Image second = CreateRouteSegment(name + "B", parent, bend, end, dash, color);
        CreateConnector(name + "Joint", parent, bend, color);
        all.Add(first);
        all.Add(second);
        return new[] { first, second };
    }

    private static Image CreateRouteSegment(string name, Transform parent, Vector2 a, Vector2 b, Sprite dash, Color color)
    {
        Vector2 delta = b - a;
        Vector2 midpoint = (a + b) * 0.5f;
        RectTransform rect = CreateRect(name, parent, midpoint, new Vector2(delta.magnitude, 14f));
        CenterAt(rect, midpoint);
        rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
        Image image = rect.gameObject.AddComponent<Image>();
        image.sprite = dash;
        image.type = Image.Type.Sliced;
        color.a = 0.72f;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static void CreateConnector(string name, Transform parent, Vector2 position, Color color)
    {
        Image outer = CreateImage(name, parent, null, color);
        CenterAt(outer.rectTransform, position);
        outer.rectTransform.sizeDelta = new Vector2(10f, 10f);
        outer.raycastTarget = false;
    }

    private static void CreateBottomNavigation(RectTransform navigation, Sprite frame, TMP_FontAsset font, Dimension1PanelUI panel)
    {
        float width = 204f;
        float start = (navigation.rect.width - (width * 5f + 4f * 4f)) * 0.5f;
        Button galaxy = CreateNavButton("GalaxyButton", navigation, frame, font, LoadSprite(ArtPath + "/d1_nav_galaxy_v3.png"), "GALAXIA", start, width, true);
        galaxy.interactable = false;
        Button explore = CreateNavButton("ExploreButton", navigation, frame, font, LoadSprite(ArtPath + "/d1_nav_explore_v3.png"), "EXPLORAR", start + 208f, width, false);
        AddPersistent(explore.onClick, panel.OnClickCloseGalaxyPanel);
        Button hangar = CreateNavButton("HangarButton", navigation, frame, font, LoadSprite(ArtPath + "/d1_nav_hangar_v3.png"), "HANGAR", start + 416f, width, false);
        AddPersistent(hangar.onClick, panel.OnClickCloseGalaxyPanel);
        AddPersistent(hangar.onClick, panel.OnClickOpenHangarPanel);
        Button relics = CreateNavButton("RelicsButton", navigation, frame, font, LoadSprite(ArtPath + "/d1_nav_relics_v3.png"), "RELIQUIAS", start + 624f, width, false);
        AddPersistent(relics.onClick, panel.OnClickCloseGalaxyPanel);
        AddPersistent(relics.onClick, panel.OnClickOpenRelicChamberPanel);
        Button tree = CreateNavButton("TreeButton", navigation, frame, font, LoadSprite(ArtPath + "/d1_nav_tree_v3.png"), "ÁRBOL", start + 832f, width, false);
        AddPersistent(tree.onClick, panel.OnClickCloseGalaxyPanel);
        AddPersistent(tree.onClick, panel.OnClickOpenDimension1TreePanel);
    }

    private static Button CreateNavButton(string name, Transform parent, Sprite frame, TMP_FontAsset font,
        Sprite iconSprite, string label, float x, float width, bool active)
    {
        RectTransform rect = CreatePremiumPanel(name, parent, frame,
            new Vector2(x, -8f), new Vector2(width, 132f), active ? Hex("172014", 252) : Hex("07111A", 248), active ? Amber : Hex("587181"));
        Image hit = rect.gameObject.GetComponent<Image>();
        if (hit == null) hit = rect.gameObject.AddComponent<Image>();
        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = hit;
        Image icon = CreateImage("Icon", rect, iconSprite, active ? Amber : Secondary);
        SetTopRect(icon.rectTransform, (width - 58f) * 0.5f, 12f, 58f, 58f);
        icon.raycastTarget = false;
        TMP_Text text = CreateText("Label", rect, font, label, 17f, FontStyles.Bold, active ? Amber : Secondary);
        SetTopRect(text.rectTransform, 8f, 77f, width - 16f, 36f);
        text.alignment = TextAlignmentOptions.Center;
        return button;
    }

    private static Dimension1GalaxyVisualUI.MetalChip CreateMetalChip(Transform parent, Sprite frame, TMP_FontAsset font, float x)
    {
        RectTransform rect = CreatePremiumPanel("MetalChip", parent, frame,
            new Vector2(x, -22f), new Vector2(154f, 68f), Hex("0A141E", 248), Cyan);
        Image marker = CreateImage("Marker", rect, null, Cyan);
        SetTopRect(marker.rectTransform, 10f, 13f, 9f, 42f);
        TMP_Text name = CreateText("Name", rect, font, "METAL", 12f, FontStyles.Bold, Secondary);
        SetTopRect(name.rectTransform, 25f, 8f, 115f, 22f);
        TMP_Text amount = CreateText("Amount", rect, font, "0", 22f, FontStyles.Bold, Primary);
        SetTopRect(amount.rectTransform, 25f, 25f, 78f, 34f);
        TMP_Text rate = CreateText("Rate", rect, font, "+0/s", 12f, FontStyles.Bold, Cyan);
        SetTopRect(rate.rectTransform, 95f, 35f, 48f, 24f);
        rate.alignment = TextAlignmentOptions.Right;
        return new Dimension1GalaxyVisualUI.MetalChip
        {
            root = rect.gameObject,
            nameText = name,
            amountText = amount,
            rateText = rate,
            marker = marker
        };
    }

    private static RectTransform CreatePremiumPanel(string name, Transform parent, Sprite frame,
        Vector2 topLeft, Vector2 size, Color fillColor, Color accent)
    {
        RectTransform root = CreateRect(name, parent, topLeft, size);
        SetTopLeft(root, topLeft, size);
        Sprite panelFill = LoadSprite(ArtPath + "/d1_panel_fill_v4.png");
        Image shadow = CreateImage("Shadow", root, panelFill, Hex("000000", 150));
        Stretch(shadow.rectTransform, new Vector2(5f, -5f), new Vector2(5f, -5f));
        shadow.type = Image.Type.Sliced;
        shadow.raycastTarget = false;
        Image fill = CreateImage("Fill", root, panelFill, fillColor);
        Stretch(fill.rectTransform);
        fill.type = Image.Type.Sliced;
        fill.raycastTarget = false;
        float accentMix = size.x >= 500f ? 0.16f : 0.24f;
        Image border = CreateImage("Border", root, frame, Color.Lerp(Hex("596A75"), accent, accentMix));
        Stretch(border.rectTransform);
        border.type = Image.Type.Sliced;
        border.raycastTarget = false;
        if (size.x >= 500f && size.y >= 150f)
        {
            Image crown = CreateImage("TopCrown", root, LoadSprite(ArtPath + "/d1_panel_crown_v4.png"), accent);
            SetCenteredTopRect(crown.rectTransform, (size.x - 156f) * 0.5f, -7f, 156f, 38f);
            crown.raycastTarget = false;
            CreatePanelBolt("BoltTL", root, new Vector2(16f, -18f), accent);
            CreatePanelBolt("BoltTR", root, new Vector2(size.x - 16f, -18f), accent);
            CreatePanelBolt("BoltBL", root, new Vector2(16f, -(size.y - 18f)), accent);
            CreatePanelBolt("BoltBR", root, new Vector2(size.x - 16f, -(size.y - 18f)), accent);
        }
        return root;
    }

    private static void CreatePanelBolt(string name, Transform parent, Vector2 position, Color color)
    {
        Image bolt = CreateImage(name, parent, LoadSprite(ArtPath + "/d1_glow_v3.png"), color);
        CenterAt(bolt.rectTransform, position);
        bolt.rectTransform.sizeDelta = new Vector2(10f, 10f);
        bolt.raycastTarget = false;
    }

    private static Button CreateButton(string name, Transform parent, Sprite frame, TMP_FontAsset font,
        string label, Vector2 topLeft, Vector2 size, Color accent)
    {
        RectTransform rect = CreatePremiumPanel(name, parent, frame, topLeft, size, Hex("3A2708", 252), accent);
        Image target = FindChild(rect, "Fill").GetComponent<Image>();
        Image buttonBorder = FindChild(rect, "Border").GetComponent<Image>();
        if (buttonBorder != null)
            buttonBorder.color = Color.Lerp(Hex("74591E"), accent, 0.62f);
        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = target;
        ColorBlock colors = button.colors;
        colors.normalColor = Hex("965B08");
        colors.highlightedColor = Hex("D58B12");
        colors.pressedColor = Hex("603605");
        colors.disabledColor = Hex("20272C", 210);
        button.colors = colors;
        TMP_Text text = CreateText("Label", rect, font, label, 25f, FontStyles.Bold, Hex("FFE5A3"));
        Stretch(text.rectTransform, new Vector2(16f, 8f), new Vector2(-16f, -8f));
        text.alignment = TextAlignmentOptions.Center;
        return button;
    }

    private static void CreateDistantPlanetEdges(Transform map, Dictionary<string, Sprite> bodies)
    {
        Image left = CreateImage("DistantPlanetLeft", map, bodies["planet_blue"], Hex("8CB7CF", 85));
        SetTopRect(left.rectTransform, -210f, 650f, 420f, 420f);
        Image right = CreateImage("DistantPlanetRight", map, bodies["planet_silent"], Hex("8196A8", 70));
        SetTopRect(right.rectTransform, 945f, 260f, 390f, 390f);
        left.raycastTarget = right.raycastTarget = false;
    }

    private static RectTransform[] CreateAsteroidField(Transform parent, Sprite sprite)
    {
        Vector2[] positions =
        {
            new Vector2(90f, -380f), new Vector2(420f, -150f), new Vector2(930f, -360f),
            new Vector2(90f, -610f), new Vector2(720f, -500f), new Vector2(960f, -700f)
        };
        RectTransform[] result = new RectTransform[positions.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            Image image = CreateImage("Asteroid" + i, parent, sprite, Hex("ABB4BA", (byte)(105 + i * 14)));
            CenterAt(image.rectTransform, positions[i]);
            float size = 20f + (i % 3) * 9f;
            image.rectTransform.sizeDelta = new Vector2(size, size);
            image.raycastTarget = false;
            result[i] = image.rectTransform;
        }
        return result;
    }

    private static Image[] CreateGalaxyTwinkles(Transform parent, Sprite glowSprite)
    {
        Vector2[] positions =
        {
            new Vector2(154f, 292f),
            new Vector2(794f, 385f),
            new Vector2(88f, 1270f),
            new Vector2(808f, 1360f),
            new Vector2(620f, 230f)
        };
        float[] sizes = { 42f, 52f, 34f, 46f, 30f };
        var result = new Image[positions.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            Image image = CreateImage("GalaxyTwinkle" + (i + 1), parent, glowSprite, Hex("BDEBFF", (byte)(72 + i * 5)));
            SetCenteredTopRect(image.rectTransform,
                positions[i].x - sizes[i] * 0.5f,
                positions[i].y - sizes[i] * 0.5f,
                sizes[i], sizes[i]);
            image.raycastTarget = false;
            result[i] = image;
        }
        return result;
    }

    private static void CreateVignette(Transform root)
    {
        Image top = CreateImage("TopShade", root, null, Hex("01040A", 95));
        SetTopRect(top.rectTransform, 0f, 0f, 1080f, 155f);
        Image bottom = CreateImage("BottomShade", root, null, Hex("01040A", 105));
        SetTopRect(bottom.rectTransform, 0f, 1120f, 1080f, 800f);
        top.raycastTarget = bottom.raycastTarget = false;
    }

    private static void Validate(Scene scene)
    {
        Transform root = null;
        foreach (GameObject sceneRoot in scene.GetRootGameObjects())
        {
            root = FindChild(sceneRoot.transform, RootName);
            if (root != null) break;
        }
        if (root == null) throw new InvalidOperationException("No existe el root premium.");
        Dimension1GalaxyVisualUI visual = root.GetComponent<Dimension1GalaxyVisualUI>();
        if (visual == null) throw new InvalidOperationException("Falta el controlador visual.");
        if (FindChild(root, "D1BottomNavigation") == null)
            throw new InvalidOperationException("Falta navegación D1.");
        if (FindChild(root, "FullBleedGalaxyMap") == null)
            throw new InvalidOperationException("Falta mapa full-bleed.");
        Transform galaxy = FindChild(root, "GalaxyAnimated");
        if (galaxy == null || galaxy.GetComponent<Image>() == null)
            throw new InvalidOperationException("Falta la galaxia animada localizada.");
        bool referenceLayout = FindChild(root, "MapTitle") != null;
        if (!referenceLayout && (galaxy.parent == null || galaxy.parent.GetComponent<RectMask2D>() == null))
            throw new InvalidOperationException("La galaxia animada no esta dentro de RectMask2D.");
        if (galaxy.GetComponent<Image>().material == null ||
            galaxy.GetComponent<Image>().material.shader == null ||
            galaxy.GetComponent<Image>().material.shader.name != "UI/QuantumForge/GalaxyWarp")
            throw new InvalidOperationException("La galaxia animada no usa GalaxyWarpUI.");
    }

    private static Dictionary<string, Sprite> LoadCelestialSprites()
    {
        var result = new Dictionary<string, Sprite>();
        string[] required = { "planet_blue", "debris_ring", "planet_ancient", "planet_silent", "black_hole", "planet_habitable" };
        foreach (string key in required)
        {
            Sprite sprite = LoadSprite(ArtPath + "/d1_body_" + key + "_v3.png");
            if (sprite == null) throw new InvalidOperationException("Falta sprite celestial: " + key);
            result[key] = sprite;
        }
        return result;
    }

    private static RectTransform[] GetOrbitRects(Dimension1GalaxyVisualUI.SectorNodeView[] nodes)
    {
        RectTransform[] result = new RectTransform[nodes.Length];
        for (int i = 0; i < nodes.Length; i++) result[i] = nodes[i].orbitRing.rectTransform;
        return result;
    }

    private static GameObject[] FindNavigationRoots(Scene scene)
    {
        var result = new List<GameObject>();
        string[] names = { "PrimaryNavigationSlot", "SecondaryNavigationSlot", "MachineContextTabs" };
        foreach (string name in names)
        {
            Transform found = null;
            foreach (GameObject sceneRoot in scene.GetRootGameObjects())
            {
                found = FindChild(sceneRoot.transform, name);
                if (found != null) break;
            }
            if (found != null) result.Add(found.gameObject);
        }
        return result.ToArray();
    }

    private static void SetSectorNodeArray(SerializedObject target, Dimension1GalaxyVisualUI.SectorNodeView[] nodes)
    {
        SerializedProperty property = target.FindProperty("sectorNodes");
        property.arraySize = nodes.Length;
        for (int i = 0; i < nodes.Length; i++)
        {
            SerializedProperty item = property.GetArrayElementAtIndex(i);
            item.FindPropertyRelative("sectorId").stringValue = nodes[i].sectorId;
            item.FindPropertyRelative("root").objectReferenceValue = nodes[i].root;
            item.FindPropertyRelative("planet").objectReferenceValue = nodes[i].planet;
            item.FindPropertyRelative("orbitRing").objectReferenceValue = nodes[i].orbitRing;
            item.FindPropertyRelative("glow").objectReferenceValue = nodes[i].glow;
            item.FindPropertyRelative("labelPlate").objectReferenceValue = nodes[i].labelPlate;
            item.FindPropertyRelative("labelBorder").objectReferenceValue = nodes[i].labelBorder;
            item.FindPropertyRelative("lockBadge").objectReferenceValue = nodes[i].lockBadge;
            SerializedProperty currentBadge = item.FindPropertyRelative("currentBadge");
            if (currentBadge != null) currentBadge.objectReferenceValue = nodes[i].currentBadge;
            item.FindPropertyRelative("titleText").objectReferenceValue = nodes[i].titleText;
            item.FindPropertyRelative("stateText").objectReferenceValue = nodes[i].stateText;
            SerializedProperty routes = item.FindPropertyRelative("routes");
            routes.arraySize = nodes[i].routes == null ? 0 : nodes[i].routes.Length;
            for (int j = 0; j < routes.arraySize; j++)
                routes.GetArrayElementAtIndex(j).objectReferenceValue = nodes[i].routes[j];
        }
    }

    private static void SetMetalChipArray(SerializedObject target, Dimension1GalaxyVisualUI.MetalChip[] chips)
    {
        SerializedProperty property = target.FindProperty("metalChips");
        property.arraySize = chips.Length;
        for (int i = 0; i < chips.Length; i++)
        {
            SerializedProperty item = property.GetArrayElementAtIndex(i);
            item.FindPropertyRelative("root").objectReferenceValue = chips[i].root;
            item.FindPropertyRelative("nameText").objectReferenceValue = chips[i].nameText;
            item.FindPropertyRelative("amountText").objectReferenceValue = chips[i].amountText;
            item.FindPropertyRelative("rateText").objectReferenceValue = chips[i].rateText;
            item.FindPropertyRelative("marker").objectReferenceValue = chips[i].marker;
        }
    }

    private static void SetObjectArray(SerializedObject target, string name, UnityEngine.Object[] values)
    {
        SerializedProperty property = target.FindProperty(name);
        property.arraySize = values.Length;
        for (int i = 0; i < values.Length; i++) property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
    }

    private static void Assign(SerializedObject target, string name, UnityEngine.Object value)
    {
        SerializedProperty property = target.FindProperty(name);
        if (property == null) throw new InvalidOperationException("No existe campo: " + name);
        property.objectReferenceValue = value;
    }

    private static void AddPersistent(Button.ButtonClickedEvent click, UnityAction action)
    {
        UnityEventTools.AddPersistentListener(click, action);
    }

    private static TMP_Text CreateText(string name, Transform parent, TMP_FontAsset font, string value,
        float size, FontStyles style, Color color)
    {
        RectTransform rect = CreateRect(name, parent, Vector2.zero, new Vector2(100f, 40f));
        TextMeshProUGUI text = rect.gameObject.AddComponent<TextMeshProUGUI>();
        text.font = font;
        text.text = value;
        text.fontSize = size;
        text.fontStyle = style;
        text.color = color;
        text.raycastTarget = false;
        text.enableWordWrapping = true;
        return text;
    }

    private static Image CreateImage(string name, Transform parent, Sprite sprite, Color color)
    {
        RectTransform rect = CreateRect(name, parent, Vector2.zero, new Vector2(100f, 100f));
        Image image = rect.gameObject.AddComponent<Image>();
        image.sprite = sprite;
        image.color = color;
        image.preserveAspect = sprite != null;
        return image;
    }

    private static RectTransform CreateRect(string name, Transform parent, Vector2 position, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return rect;
    }

    private static void SetTopLeft(RectTransform rect, Vector2 topLeft, Vector2 size)
    {
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = topLeft;
        rect.sizeDelta = size;
    }

    private static void SetTopRect(RectTransform rect, float x, float y, float w, float h)
    {
        SetTopLeft(rect, new Vector2(x, -y), new Vector2(w, h));
    }

    private static void SetCenteredTopRect(RectTransform rect, float x, float y, float w, float h)
    {
        rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(x + w * 0.5f, -(y + h * 0.5f));
        rect.sizeDelta = new Vector2(w, h);
    }

    private static void SetCenteredParentTop(RectTransform rect, float top, Vector2 size)
    {
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, top);
        rect.sizeDelta = size;
    }

    private static void SetCenterRect(RectTransform rect, Vector2 offset, Vector2 size)
    {
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = offset;
        rect.sizeDelta = size;
    }

    private static void CenterAt(RectTransform rect, Vector2 position)
    {
        rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
    }

    private static void Stretch(RectTransform rect) => Stretch(rect, Vector2.zero, Vector2.zero);

    private static void Stretch(RectTransform rect, Vector2 min, Vector2 max)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = min;
        rect.offsetMax = max;
    }

    private static string GetShortName(string id)
    {
        if (id == Dimension1System.Sector01OuterRim) return "BORDE EXTERIOR";
        if (id == Dimension1System.Sector02DebrisRing) return "ANILLO DE RESTOS";
        if (id == Dimension1System.Sector03AncientOrbits) return "ÓRBITAS ANTIGUAS";
        if (id == Dimension1System.Sector04SilentFrontier) return "FRONTERA SILENCIOSA";
        return "CENTRO GALÁCTICO";
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

    private static Transform FindChild(Transform parent, string name)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
            if (child.name == name) return child;
        return null;
    }

    private static Transform FindDirectChild(Transform parent, string name)
    {
        foreach (Transform child in parent) if (child.name == name) return child;
        return null;
    }

    private static TMP_FontAsset FindFont(Transform root)
    {
        TMP_Text text = root.GetComponentInChildren<TMP_Text>(true);
        if (text != null && text.font != null) return text.font;
        if (TMP_Settings.defaultFontAsset != null) return TMP_Settings.defaultFontAsset;
        throw new InvalidOperationException("No hay fuente TMP.");
    }

    private static Sprite LoadSprite(string path) => AssetDatabase.LoadAssetAtPath<Sprite>(path);

    private static void ConfigureFinalArt()
    {
        ConfigureTransparentGalaxy(GalaxyArtPath);
        ConfigureTransparentGalaxy(GalaxyArmsPath);
        ConfigureTransparentGalaxy(GalaxyDustPath);
        ConfigureTransparentGalaxy(GalaxyCorePath);
        ConfigureBackgroundSprite(ArtPath + "/d1_starfield.png", 2048, true);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }

    private static void ConfigureTransparentGalaxy(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) throw new InvalidOperationException("No se encontro la galaxia RGBA seleccionada.");
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        var textureSettings = new TextureImporterSettings();
        importer.ReadTextureSettings(textureSettings);
        textureSettings.spriteMeshType = SpriteMeshType.FullRect;
        importer.SetTextureSettings(textureSettings);
        importer.alphaSource = TextureImporterAlphaSource.FromInput;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = true;
        importer.filterMode = FilterMode.Trilinear;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.maxTextureSize = 2048;
        importer.isReadable = false;
        importer.crunchedCompression = false;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;

        TextureImporterPlatformSettings android = importer.GetPlatformTextureSettings("Android");
        android.name = "Android";
        android.overridden = true;
        android.maxTextureSize = 2048;
        android.format = TextureImporterFormat.ASTC_4x4;
        android.compressionQuality = 100;
        importer.SetPlatformTextureSettings(android);
        importer.SaveAndReimport();
    }

    private static Material GetOrCreateGalaxyMaterial()
    {
        Shader shader = Shader.Find("UI/QuantumForge/GalaxyWarp");
        if (shader == null)
            throw new InvalidOperationException("No se encontro el shader UI/QuantumForge/GalaxyWarp.");

        string directory = Path.GetDirectoryName(GalaxyMaterialPath).Replace('\\', '/');
        if (!AssetDatabase.IsValidFolder(directory))
        {
            if (!AssetDatabase.IsValidFolder("Assets/Project/Materials"))
                AssetDatabase.CreateFolder("Assets/Project", "Materials");
            if (!AssetDatabase.IsValidFolder(directory))
                AssetDatabase.CreateFolder("Assets/Project/Materials", "UI");
        }

        Material material = AssetDatabase.LoadAssetAtPath<Material>(GalaxyMaterialPath);
        if (material == null)
        {
            material = new Material(shader) { name = "D1GalaxyLayeredV10" };
            AssetDatabase.CreateAsset(material, GalaxyMaterialPath);
        }
        else
        {
            material.shader = shader;
        }

        Texture2D arms = AssetDatabase.LoadAssetAtPath<Texture2D>(GalaxyArmsPath);
        Texture2D dust = AssetDatabase.LoadAssetAtPath<Texture2D>(GalaxyDustPath);
        Texture2D core = AssetDatabase.LoadAssetAtPath<Texture2D>(GalaxyCorePath);
        if (arms == null || dust == null || core == null)
            throw new InvalidOperationException("Faltan capas de la galaxia V10.");
        material.SetTexture("_ArmsTex", arms);
        material.SetTexture("_DustTex", dust);
        material.SetTexture("_CoreTex", core);
        material.SetVector("_GalaxyCenter", new Vector4(0.5f, 0.5f, 0f, 0f));
        material.SetFloat("_AxisAngleDeg", 0f);
        material.SetFloat("_AxisRatio", 0.70f);
        material.SetFloat("_ArmSpeedDeg", 2.0f);
        material.SetFloat("_DustSpeedDeg", -0.65f);
        material.SetFloat("_BaseVisibility", 0.30f);
        material.SetFloat("_ArmIntensity", 1.45f);
        material.SetFloat("_DustIntensity", 0.72f);
        material.SetFloat("_CoreIntensity", 1.0f);
        material.SetFloat("_CorePeriod", 4.8f);
        material.SetFloat("_CoreBrightness", 0.055f);
        EditorUtility.SetDirty(material);
        AssetDatabase.SaveAssets();
        return material;
    }

    private static void DisableLegacyGalaxyChildren(Transform galaxyPanel)
    {
        string[] legacyNames =
        {
            "GalaxyTitleText",
            "GalaxySectorSummaryText",
            "GalaxySector1Button",
            "GalaxySector2Button",
            "GalaxySector3Button",
            "GalaxySector4Button",
            "GalaxyCenterButton",
            "EnterGalaxySectorButton",
            "CloseGalaxyPanelButton"
        };

        foreach (string name in legacyNames)
        {
            Transform child = FindDirectChild(galaxyPanel, name);
            if (child != null) child.gameObject.SetActive(false);
        }
    }

    private static void ExtractCelestialSprites()
    {
        string sourcePath = ArtPath + "/d1_celestial_sheet_v3.png";
        Texture2D source = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!source.LoadImage(File.ReadAllBytes(sourcePath), false))
            throw new InvalidOperationException("No se pudo decodificar la hoja celestial.");

        string[] names = { "planet_blue", "debris_ring", "planet_ancient", "planet_silent", "black_hole", "planet_habitable" };
        int cellWidth = source.width / 3;
        int cellHeight = source.height / 2;
        for (int index = 0; index < names.Length; index++)
        {
            int column = index % 3;
            int row = index < 3 ? 1 : 0;
            int x0 = column * cellWidth;
            int y0 = row * cellHeight;
            int minX = x0 + cellWidth;
            int minY = y0 + cellHeight;
            int maxX = x0;
            int maxY = y0;
            for (int y = y0; y < y0 + cellHeight; y++)
            for (int x = x0; x < x0 + cellWidth; x++)
            {
                if (source.GetPixel(x, y).a <= 0.035f) continue;
                minX = Mathf.Min(minX, x);
                minY = Mathf.Min(minY, y);
                maxX = Mathf.Max(maxX, x);
                maxY = Mathf.Max(maxY, y);
            }

            if (maxX <= minX || maxY <= minY)
                throw new InvalidOperationException("Celestial vacío: " + names[index]);
            int objectWidth = maxX - minX + 1;
            int objectHeight = maxY - minY + 1;
            int padding = 14;
            int size = Mathf.Min(512, Mathf.Max(objectWidth, objectHeight) + padding * 2);
            Texture2D output = NewTexture(size, size);
            Color[] clear = new Color[size * size];
            output.SetPixels(clear);
            int offsetX = (size - objectWidth) / 2;
            int offsetY = (size - objectHeight) / 2;
            for (int y = 0; y < objectHeight; y++)
            for (int x = 0; x < objectWidth; x++)
                output.SetPixel(offsetX + x, offsetY + y, source.GetPixel(minX + x, minY + y));
            Save(output, ArtPath + "/d1_body_" + names[index] + "_v3.png");
        }
        UnityEngine.Object.DestroyImmediate(source);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }

    private static void ConfigureCelestialSheet()
    {
        string path = ArtPath + "/d1_celestial_sheet_v3.png";
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) throw new InvalidOperationException("No se encontró la hoja celestial.");
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Bilinear;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        Texture2D source = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (source == null) throw new InvalidOperationException("No se pudo leer la hoja celestial.");
        int width = source.width;
        int height = source.height;
        float cellW = width / 3f;
        float square = Mathf.Min(cellW, height / 2f);
        float bottomY = (height * 0.5f - square) * 0.5f;
        float topY = height * 0.5f + bottomY;
        string[] names = { "planet_blue", "debris_ring", "planet_ancient", "planet_silent", "black_hole", "planet_habitable" };
        var metas = new SpriteMetaData[6];
        for (int i = 0; i < 6; i++)
        {
            int column = i % 3;
            bool top = i < 3;
            metas[i] = new SpriteMetaData
            {
                name = names[i],
                alignment = (int)SpriteAlignment.Center,
                pivot = new Vector2(0.5f, 0.5f),
                rect = new Rect(column * cellW, top ? topY : bottomY, square, square)
            };
        }
#pragma warning disable 618
        importer.spritesheet = metas;
#pragma warning restore 618
        importer.SaveAndReimport();
    }

    private static void EnsureStructuralArt()
    {
        Directory.CreateDirectory(ArtPath);
        GenerateFrame(ArtPath + "/d1_premium_frame_v4.png");
        GeneratePanelFill(ArtPath + "/d1_panel_fill_v4.png");
        GeneratePanelCrown(ArtPath + "/d1_panel_crown_v4.png");
        GenerateAmbientVeil(ArtPath + "/d1_ambient_veil_v4.png");
        GenerateOrbit(ArtPath + "/d1_orbit_ring_v3.png");
        GenerateGlow(ArtPath + "/d1_glow_v3.png");
        GenerateRouteDash(ArtPath + "/d1_route_dash_v3.png");
        GenerateRouteCore(ArtPath + "/d1_route_core_v4.png");
        GenerateLock(ArtPath + "/d1_lock_v3.png");
        GenerateNavIcons();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        ConfigureSingleSprite(ArtPath + "/d1_premium_frame_v4.png", new Vector4(46f, 46f, 46f, 46f), 256, false);
        ConfigureSingleSprite(ArtPath + "/d1_panel_fill_v4.png", new Vector4(46f, 46f, 46f, 46f), 256, false);
        ConfigureSingleSprite(ArtPath + "/d1_panel_crown_v4.png", Vector4.zero, 256, false);
        ConfigureSingleSprite(ArtPath + "/d1_ambient_veil_v4.png", Vector4.zero, 512, false);
        ConfigureSingleSprite(ArtPath + "/d1_orbit_ring_v3.png", Vector4.zero, 512, false);
        ConfigureSingleSprite(ArtPath + "/d1_glow_v3.png", Vector4.zero, 512, false);
        ConfigureSingleSprite(ArtPath + "/d1_route_dash_v3.png", new Vector4(8f, 0f, 8f, 0f), 128, true);
        ConfigureSingleSprite(ArtPath + "/d1_route_core_v4.png", new Vector4(8f, 0f, 8f, 0f), 128, false);
        ConfigureSingleSprite(ArtPath + "/d1_lock_v3.png", Vector4.zero, 128, false);
        string[] navNames = { "galaxy", "explore", "hangar", "relics", "tree" };
        foreach (string navName in navNames)
            ConfigureSingleSprite(ArtPath + "/d1_nav_" + navName + "_v3.png", Vector4.zero, 128, false);
    }

    private static void ConfigureSingleSprite(string path, Vector4 border, int maxSize, bool repeat)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Bilinear;
        importer.wrapMode = repeat ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;
        importer.maxTextureSize = maxSize;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        importer.spriteBorder = border;
        importer.SaveAndReimport();
    }

    private static void ConfigureBackgroundSprite(string path, int maxSize, bool uncompressed = false)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = true;
        importer.filterMode = FilterMode.Trilinear;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.maxTextureSize = maxSize;
        importer.textureCompression = uncompressed
            ? TextureImporterCompression.Uncompressed
            : TextureImporterCompression.CompressedHQ;
        importer.SaveAndReimport();
    }

    private static void GenerateFrame(string path)
    {
        const int size = 192;
        Texture2D texture = NewTexture(size, size);
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            int dx = Mathf.Min(x, size - 1 - x);
            int dy = Mathf.Min(y, size - 1 - y);
            bool outerStraight = (dy >= 2 && dy <= 5 && dx >= 25) ||
                                 (dx >= 2 && dx <= 5 && dy >= 25);
            bool outerBevel = dx <= 25 && dy <= 25 && Mathf.Abs(dx + dy - 27) <= 2;
            bool innerStraight = (dy >= 13 && dy <= 15 && dx >= 29) ||
                                 (dx >= 13 && dx <= 15 && dy >= 29);
            bool innerBevel = dx >= 13 && dy >= 13 && dx <= 30 && dy <= 30 &&
                              Mathf.Abs(dx + dy - 43) <= 1;
            bool cornerPlate = dx >= 7 && dx <= 11 && dy >= 24 && dy <= 32 ||
                               dy >= 7 && dy <= 11 && dx >= 24 && dx <= 32;
            Color color = outerStraight || outerBevel
                ? Hex("D7E4EA")
                : innerStraight || innerBevel
                    ? Hex("557181")
                    : cornerPlate ? Hex("91A9B6") : Color.clear;
            texture.SetPixel(x, y, color);
        }
        Save(texture, path);
    }

    private static void GeneratePanelFill(string path)
    {
        const int size = 192;
        Texture2D texture = NewTexture(size, size);
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            int dx = Mathf.Min(x, size - 1 - x);
            int dy = Mathf.Min(y, size - 1 - y);
            bool outsideBevel = dx + dy < 27;
            texture.SetPixel(x, y, outsideBevel ? Color.clear : Color.white);
        }
        Save(texture, path);
    }

    private static void GeneratePanelCrown(string path)
    {
        const int width = 192;
        const int height = 48;
        Texture2D texture = NewTexture(width, height);
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            int dx = Mathf.Min(x, width - 1 - x);
            int dy = Mathf.Min(y, height - 1 - y);
            bool plate = dx + dy >= 15 && y >= 5 && y <= 40;
            bool edge = plate && (y <= 8 || y >= 37 || Mathf.Abs(dx + dy - 15) <= 2);
            bool rail = y >= 20 && y <= 24 && x >= 60 && x <= 132;
            texture.SetPixel(x, y, edge || rail ? Color.white : plate ? Hex("25323B") : Color.clear);
        }
        Save(texture, path);
    }

    private static void GenerateAmbientVeil(string path)
    {
        // Low-frequency translucent clouds only. There are deliberately no stars
        // in this layer, so its slow drift cannot create high-frequency shimmer.
        const int size = 512;
        Texture2D texture = NewTexture(size, size);
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float u = x / (float)(size - 1);
            float v = y / (float)(size - 1);
            float broad = Mathf.PerlinNoise(u * 2.05f + 11.7f, v * 1.75f + 4.3f);
            float detail = Mathf.PerlinNoise(u * 4.1f + 3.2f, v * 3.6f + 17.1f);
            float diagonalA = Mathf.Exp(-Mathf.Pow((v - (0.76f - u * 0.42f)) / 0.28f, 2f));
            float diagonalB = Mathf.Exp(-Mathf.Pow((v - (0.16f + u * 0.30f)) / 0.22f, 2f));
            float shape = Mathf.Max(diagonalA, diagonalB * 0.62f);
            float alpha = Mathf.Clamp01((broad * 0.72f + detail * 0.28f - 0.38f) * 0.95f) * shape;
            Color cyan = new Color(0.08f, 0.48f, 0.72f, alpha * 0.76f);
            Color violet = new Color(0.32f, 0.12f, 0.48f, alpha * 0.68f);
            texture.SetPixel(x, y, Color.Lerp(violet, cyan, Mathf.Clamp01(u * 0.72f + broad * 0.28f)));
        }
        Save(texture, path);
    }

    private static void GenerateOrbit(string path)
    {
        const int size = 256;
        Texture2D texture = NewTexture(size, size);
        Vector2 c = new Vector2(size * 0.5f, size * 0.5f);
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), c);
            float angle = Mathf.Atan2(y - c.y, x - c.x) * Mathf.Rad2Deg + 180f;
            bool outer = Mathf.Abs(d - 112f) < 2.2f && ((int)(angle / 12f) % 2 == 0);
            bool inner = Mathf.Abs(d - 97f) < 1.2f;
            bool tick = d > 103f && d < 117f && Mathf.Repeat(angle, 45f) < 2.3f;
            texture.SetPixel(x, y, outer || inner || tick ? Color.white : Color.clear);
        }
        Save(texture, path);
    }

    private static void GenerateGlow(string path)
    {
        const int size = 256;
        Texture2D texture = NewTexture(size, size);
        Vector2 c = new Vector2(size * 0.5f, size * 0.5f);
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float d = Vector2.Distance(new Vector2(x, y), c) / (size * 0.5f);
            float a = Mathf.Pow(Mathf.Clamp01(1f - d), 2.6f) * 0.9f;
            texture.SetPixel(x, y, new Color(1f, 1f, 1f, a));
        }
        Save(texture, path);
    }

    private static void GenerateRouteDash(string path)
    {
        Texture2D texture = NewTexture(64, 16);
        for (int y = 0; y < 16; y++)
        for (int x = 0; x < 64; x++)
        {
            bool dash = (x % 24) < 14;
            float a = dash ? Mathf.Clamp01(1f - Mathf.Abs(y - 7.5f) / 6f) : 0f;
            texture.SetPixel(x, y, new Color(1f, 1f, 1f, a));
        }
        Save(texture, path);
    }

    private static void GenerateRouteCore(string path)
    {
        const int width = 64;
        const int height = 16;
        Texture2D texture = NewTexture(width, height);
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            float distance = Mathf.Abs(y - (height - 1) * 0.5f);
            float alpha = distance <= 1.25f
                ? 1f
                : Mathf.Pow(Mathf.Clamp01(1f - distance / 8f), 2f) * 0.42f;
            float endFade = Mathf.Clamp01(Mathf.Min(x + 1f, width - x) / 5f);
            texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha * endFade));
        }
        Save(texture, path);
    }

    private static void GenerateLock(string path)
    {
        const int size = 64;
        Texture2D texture = NewTexture(size, size);
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float dx = x - 32f;
            float dy = y - 39f;
            bool shackle = y >= 28 && y <= 50 && Mathf.Abs(Mathf.Sqrt(dx * dx + dy * dy) - 13f) < 4f && y >= 39f;
            bool body = x >= 16 && x <= 48 && y >= 10 && y <= 35;
            bool key = Vector2.Distance(new Vector2(x, y), new Vector2(32f, 23f)) < 4f || (x >= 30 && x <= 34 && y >= 14 && y <= 23);
            Color color = (shackle || body) && !key ? Color.white : Color.clear;
            texture.SetPixel(x, y, color);
        }
        Save(texture, path);
    }

    private static void GenerateNavIcons()
    {
        GenerateLineIcon("galaxy", (texture) =>
        {
            DrawCircle(texture, 32, 32, 20, 3, 20, 310);
            DrawCircle(texture, 32, 32, 11, 3, 190, 490);
            FillCircle(texture, 32, 32, 4);
        });
        GenerateLineIcon("explore", (texture) =>
        {
            DrawCircle(texture, 32, 32, 23, 3, 0, 360);
            DrawLine(texture, 32, 8, 32, 56, 3);
            DrawLine(texture, 8, 32, 56, 32, 3);
            DrawLine(texture, 32, 14, 40, 35, 4);
            DrawLine(texture, 40, 35, 32, 50, 4);
        });
        GenerateLineIcon("hangar", (texture) =>
        {
            DrawLine(texture, 32, 56, 17, 18, 4);
            DrawLine(texture, 32, 56, 47, 18, 4);
            DrawLine(texture, 17, 18, 32, 26, 4);
            DrawLine(texture, 47, 18, 32, 26, 4);
            DrawLine(texture, 25, 24, 18, 8, 4);
            DrawLine(texture, 39, 24, 46, 8, 4);
        });
        GenerateLineIcon("relics", (texture) =>
        {
            DrawLine(texture, 32, 57, 13, 35, 4);
            DrawLine(texture, 13, 35, 32, 7, 4);
            DrawLine(texture, 32, 7, 51, 35, 4);
            DrawLine(texture, 51, 35, 32, 57, 4);
            DrawLine(texture, 13, 35, 51, 35, 3);
        });
        GenerateLineIcon("tree", (texture) =>
        {
            DrawLine(texture, 32, 7, 32, 44, 5);
            DrawLine(texture, 32, 28, 18, 40, 4);
            DrawLine(texture, 32, 34, 47, 48, 4);
            FillCircle(texture, 17, 43, 8);
            FillCircle(texture, 48, 50, 8);
            FillCircle(texture, 32, 52, 9);
        });
    }

    private static void GenerateLineIcon(string name, Action<Texture2D> draw)
    {
        Texture2D texture = NewTexture(64, 64);
        texture.SetPixels(new Color[64 * 64]);
        draw(texture);
        Save(texture, ArtPath + "/d1_nav_" + name + "_v3.png");
    }

    private static void DrawCircle(Texture2D texture, int cx, int cy, int radius, int thickness, float startDegrees, float endDegrees)
    {
        for (float angle = startDegrees; angle <= endDegrees; angle += 1f)
        {
            float radians = angle * Mathf.Deg2Rad;
            int x = Mathf.RoundToInt(cx + Mathf.Cos(radians) * radius);
            int y = Mathf.RoundToInt(cy + Mathf.Sin(radians) * radius);
            FillCircle(texture, x, y, thickness);
        }
    }

    private static void DrawLine(Texture2D texture, int x0, int y0, int x1, int y1, int thickness)
    {
        int steps = Mathf.Max(Mathf.Abs(x1 - x0), Mathf.Abs(y1 - y0));
        for (int i = 0; i <= steps; i++)
        {
            float t = steps == 0 ? 0f : i / (float)steps;
            FillCircle(texture, Mathf.RoundToInt(Mathf.Lerp(x0, x1, t)), Mathf.RoundToInt(Mathf.Lerp(y0, y1, t)), thickness);
        }
    }

    private static void FillCircle(Texture2D texture, int cx, int cy, int radius)
    {
        for (int y = -radius; y <= radius; y++)
        for (int x = -radius; x <= radius; x++)
        {
            int px = cx + x;
            int py = cy + y;
            if (px < 0 || py < 0 || px >= texture.width || py >= texture.height || x * x + y * y > radius * radius) continue;
            texture.SetPixel(px, py, Color.white);
        }
    }

    private static Texture2D NewTexture(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        return texture;
    }

    private static void Save(Texture2D texture, string path)
    {
        texture.Apply(false, false);
        File.WriteAllBytes(path, texture.EncodeToPNG());
        UnityEngine.Object.DestroyImmediate(texture);
    }

    private static Color Hex(string value, byte alpha = 255)
    {
        ColorUtility.TryParseHtmlString("#" + value, out Color color);
        color.a = alpha / 255f;
        return color;
    }
}
#endif

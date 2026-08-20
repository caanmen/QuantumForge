#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static partial class Dimension1GalaxyPremiumSetup
{
    private const string MoltenPlanetSourcePath = ArtPath + "/d1_body_planet_molten_v1.png";
    private const string MoltenPlanetPath = ArtPath + "/d1_body_planet_molten_alpha_v2.png";
    private const string MoltenMaterialPath = "Assets/Project/Materials/UI/D1MoltenPlanetChromaKey.mat";
    private const string RajdhaniFontPath = "Assets/Project/UI/Vertical/Fonts/Rajdhani-Medium SDF.asset";

    private sealed class ReferenceDetails
    {
        public TMP_Text summaryProxy;
        public Button enterButton;
        public TMP_Text title;
        public TMP_Text explorations;
        public TMP_Text status;
        public TMP_Text destinations;
        public TMP_Text requirements;
        public Image planetPreview;
        public Image secondaryPreview;
        public TMP_Text neutralInstruction;
        public GameObject[] detailRoots;
    }

    [MenuItem("Quantum Forge/Dimension 1/Configure Reference Carta Galactica V11")]
    public static void ConfigureReferenceV11()
    {
        ConfigureFinalArt();
        GenerateMoltenTransparentAsset();
        ConfigureSingleSprite(MoltenPlanetPath, Vector4.zero, 2048, false);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

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
        Sprite nebula = LoadSprite(ArtPath + "/d1_nebula_bg_v3.png");
        Sprite starfieldSprite = LoadSprite(ArtPath + "/d1_starfield.png");
        Dictionary<string, Sprite> bodies = LoadCelestialSprites();
        bodies["planet_molten"] = LoadSprite(MoltenPlanetPath);
        if (bodies["planet_molten"] == null)
            throw new InvalidOperationException("Falta el planeta fundido de referencia.");
        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(RajdhaniFontPath);
        if (font == null) font = FindFont(panel.transform);

        RectTransform root = CreateRect(RootName, galaxyPanel, Dimension1SharedLayoutTokens.RootOffset,
            new Vector2(Dimension1SharedLayoutTokens.Width, Dimension1SharedLayoutTokens.Height));
        root.anchorMin = root.anchorMax = new Vector2(.5f, .5f);
        root.pivot = new Vector2(.5f, .5f);
        root.anchoredPosition = Dimension1SharedLayoutTokens.RootOffset;
        root.sizeDelta = new Vector2(Dimension1SharedLayoutTokens.Width,
            Dimension1SharedLayoutTokens.Height);
        root.gameObject.AddComponent<Dimension1VisualSkinRoot>();
        Canvas overlay = root.gameObject.AddComponent<Canvas>();
        overlay.overrideSorting = true;
        overlay.sortingOrder = 32000;
        root.gameObject.AddComponent<GraphicRaycaster>();
        Dimension1GalaxyVisualUI visual = root.gameObject.AddComponent<Dimension1GalaxyVisualUI>();

        Image background = CreateImage("VoidBase", root, null, Hex("01070C"));
        Stretch(background.rectTransform);
        background.raycastTarget = false;
        Image ambient = CreateImage("AmbientStars", root, starfieldSprite, Hex("79CBEA", 18));
        Stretch(ambient.rectTransform);
        ambient.preserveAspect = false;
        ambient.raycastTarget = false;
        ambient.color = Color.clear;
        CreateReferenceStars(root);

        Image outer = CreateImage("OuterFrame", root, frame, Hex("087FA9", 185));
        SetTopRect(outer.rectTransform, Dimension1SharedLayoutTokens.OuterFrameX,
            Dimension1SharedLayoutTokens.OuterFrameY,
            Dimension1SharedLayoutTokens.OuterFrameWidth,
            Dimension1SharedLayoutTokens.OuterFrameHeight);
        outer.type = Image.Type.Sliced;
        outer.raycastTarget = false;

        Button backButton = CreateReferenceHeader(root, frame, font, out TMP_Text currentSector,
            out TMP_Text unlockedSectors, out Dimension1GalaxyVisualUI.MetalChip[] chips);

        RectTransform map = CreateRect("FullBleedGalaxyMap", root,
            new Vector2(0f, -246f), new Vector2(1080f, 1260f));
        SetTopLeft(map, new Vector2(0f, -246f), new Vector2(1080f, 1260f));
        Image mapBase = CreateImage("MapBase", map, null, Hex("01070C"));
        Stretch(mapBase.rectTransform);
        mapBase.raycastTarget = false;

        Image animatedGalaxy = CreateImage("GalaxyAnimated", map,
            LoadSprite(GalaxyArtPath), Color.clear);
        SetTopRect(animatedGalaxy.rectTransform, 539f, 1f, 1f, 1f);
        animatedGalaxy.material = GetOrCreateGalaxyMaterial();
        animatedGalaxy.raycastTarget = false;

        CreateReferenceMapTitle(map, font);

        var routeCores = new List<Image>();
        Image[] routeTopLeft = CreateReferenceRoute(map, new Vector2(432f, 610f), new Vector2(367f, 550f), routeCores);
        Image[] routeTopRight = CreateReferenceRoute(map, new Vector2(648f, 610f), new Vector2(713f, 550f), routeCores);
        Image[] routeBottomLeft = CreateReferenceRoute(map, new Vector2(432f, 860f), new Vector2(367f, 920f), routeCores);
        Image[] routeBottomRight = CreateReferenceRoute(map, new Vector2(648f, 860f), new Vector2(713f, 920f), routeCores);

        var ambientRotators = new List<RectTransform>();
        var pulsingBodies = new List<RectTransform>();
        var nodes = new Dimension1GalaxyVisualUI.SectorNodeView[5];
        nodes[0] = CreateReferenceSectorNode("Sector01", map, frame, orbit, glow, lockSprite,
            nebula, bodies, font, Dimension1System.Sector01OuterRim,
            new Vector2(240f, 355f), new Vector2(420f, 380f), routeTopLeft,
            ambientRotators, pulsingBodies);
        nodes[1] = CreateReferenceSectorNode("Sector02", map, frame, orbit, glow, lockSprite,
            nebula, bodies, font, Dimension1System.Sector02DebrisRing,
            new Vector2(840f, 355f), new Vector2(420f, 380f), routeTopRight,
            ambientRotators, pulsingBodies);
        nodes[2] = CreateReferenceSectorNode("Sector03", map, frame, orbit, glow, lockSprite,
            nebula, bodies, font, Dimension1System.Sector03AncientOrbits,
            new Vector2(235f, 1045f), new Vector2(420f, 380f), routeBottomLeft,
            ambientRotators, pulsingBodies);
        nodes[3] = CreateReferenceSectorNode("Sector04", map, frame, orbit, glow, lockSprite,
            nebula, bodies, font, Dimension1System.Sector04SilentFrontier,
            new Vector2(845f, 1045f), new Vector2(420f, 380f), routeBottomRight,
            ambientRotators, pulsingBodies);
        nodes[4] = CreateReferenceSectorNode("GalacticCenter", map, frame, orbit, glow, lockSprite,
            nebula, bodies, font, Dimension1System.Sector05GalacticCenter,
            new Vector2(540f, 735f), new Vector2(400f, 365f), Array.Empty<Image>(),
            ambientRotators, pulsingBodies);
        BringRouteForward(routeTopLeft);
        BringRouteForward(routeTopRight);
        BringRouteForward(routeBottomLeft);
        BringRouteForward(routeBottomRight);
        nodes[4].root.SetAsLastSibling();
        // The lower pair needs to sit above the central rim; otherwise the overlapping
        // hexagons hide the short luminous connectors completely.
        BringRouteForward(routeBottomLeft);
        BringRouteForward(routeBottomRight);

        ReferenceDetails details = CreateReferenceSelectionPanel(root, frame, font, bodies, panel);

        RectTransform navigation = CreateRect("D1BottomNavigation", root,
            new Vector2(Dimension1SharedLayoutTokens.NavigationX,
                -Dimension1SharedLayoutTokens.NavigationY),
            new Vector2(Dimension1SharedLayoutTokens.NavigationWidth,
                Dimension1SharedLayoutTokens.NavigationHeight));
        SetTopLeft(navigation,
            new Vector2(Dimension1SharedLayoutTokens.NavigationX,
                -Dimension1SharedLayoutTokens.NavigationY),
            new Vector2(Dimension1SharedLayoutTokens.NavigationWidth,
                Dimension1SharedLayoutTokens.NavigationHeight));
        CreateReferenceBottomNavigation(navigation, font, panel);
        Dimension1SharedShellApply.ApplyToRoot(root);

        TMP_Text titleProxy = CreateText("GalaxyTitleProxy", root, font, "", 1f,
            FontStyles.Normal, Color.clear);
        SetTopRect(titleProxy.rectTransform, 0f, 0f, 1f, 1f);

        SerializedObject serializedPanel = new SerializedObject(panel);
        Assign(serializedPanel, "galaxyTitleText", titleProxy);
        Assign(serializedPanel, "galaxySectorSummaryText", details.summaryProxy);
        Assign(serializedPanel, "galaxySector1Button", nodes[0].root.GetComponent<Button>());
        Assign(serializedPanel, "galaxySector2Button", nodes[1].root.GetComponent<Button>());
        Assign(serializedPanel, "galaxySector3Button", nodes[2].root.GetComponent<Button>());
        Assign(serializedPanel, "galaxySector4Button", nodes[3].root.GetComponent<Button>());
        Assign(serializedPanel, "galaxyCenterButton", nodes[4].root.GetComponent<Button>());
        Assign(serializedPanel, "enterGalaxySectorButton", details.enterButton);
        Assign(serializedPanel, "closeGalaxyPanelButton", backButton);
        serializedPanel.ApplyModifiedPropertiesWithoutUndo();

        SerializedObject serializedVisual = new SerializedObject(visual);
        Assign(serializedVisual, "panel", panel);
        Assign(serializedVisual, "currentSectorText", currentSector);
        Assign(serializedVisual, "unlockedSectorText", unlockedSectors);
        Assign(serializedVisual, "selectedTitleText", details.title);
        Assign(serializedVisual, "selectedExplorationsText", details.explorations);
        Assign(serializedVisual, "selectedStatusText", details.status);
        Assign(serializedVisual, "selectedDestinationsText", details.destinations);
        Assign(serializedVisual, "selectedRequirementsText", details.requirements);
        Assign(serializedVisual, "selectedPlanetPreview", details.planetPreview);
        Assign(serializedVisual, "secondaryPlanetPreview", details.secondaryPreview);
        Assign(serializedVisual, "animatedGalaxyImage", animatedGalaxy);
        Assign(serializedVisual, "neutralInstructionText", details.neutralInstruction);
        SetObjectArray(serializedVisual, "selectedDetailRoots", details.detailRoots);
        SetObjectArray(serializedVisual, "hideWhileOpen", FindNavigationRoots(scene));
        SetObjectArray(serializedVisual, "routeLines", routeCores.ToArray());
        SetObjectArray(serializedVisual, "rotatingBodies", Array.Empty<RectTransform>());
        SetObjectArray(serializedVisual, "driftingAsteroids", Array.Empty<RectTransform>());
        SetObjectArray(serializedVisual, "pulsingRouteLines", routeCores.ToArray());
        SetObjectArray(serializedVisual, "ambientRotatingBodies", ambientRotators.ToArray());
        SetObjectArray(serializedVisual, "pulsingBodies", pulsingBodies.ToArray());
        SetMetalChipArray(serializedVisual, chips);
        SetSectorNodeArray(serializedVisual, nodes);
        serializedVisual.ApplyModifiedPropertiesWithoutUndo();

        EditorUtility.SetDirty(panel);
        EditorUtility.SetDirty(visual);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("No se pudo guardar Main.unity.");

        Validate(scene);
        Debug.Log("[D1 Carta Galactica V11] CONFIGURATION_PASS | referencia hexagonal | datos reales | animaciones por capas");
    }

    private static Button CreateReferenceHeader(
        Transform root, Sprite frame, TMP_FontAsset font,
        out TMP_Text currentSector, out TMP_Text unlockedSectors,
        out Dimension1GalaxyVisualUI.MetalChip[] chips)
    {
        RectTransform back = CreateReferencePanel("CommandCenterBack", root,
            new Vector2(20f, -26f), new Vector2(245f, 58f), Hex("031019", 250), Cyan);
        Button backButton = back.gameObject.AddComponent<Button>();
        backButton.targetGraphic = FindChild(back, "Fill").GetComponent<Image>();
        backButton.transition = Selectable.Transition.None;
        TMP_Text backText = CreateText("Label", back, font, "CENTRO DE MANDO", 18f,
            FontStyles.Bold, Cyan);
        Stretch(backText.rectTransform, new Vector2(10f, 4f), new Vector2(-10f, -4f));
        backText.alignment = TextAlignmentOptions.Center;

        TMP_Text dimension = CreateText("DimensionTitle", root, font, "DIMENSIÓN 1", 45f,
            FontStyles.Bold, Primary);
        SetCenteredTopRect(dimension.rectTransform, 326f, 22f, 428f, 65f);
        dimension.alignment = TextAlignmentOptions.Center;
        dimension.characterSpacing = 3f;
        CreateAbsoluteLine("HeaderCircuitLeft", root, new[]
        {
            new Vector2(270f, 54f), new Vector2(312f, 54f), new Vector2(329f, 70f)
        }, 2.2f, Cyan);
        CreateAbsoluteLine("HeaderCircuitRight", root, new[]
        {
            new Vector2(751f, 70f), new Vector2(768f, 54f), new Vector2(812f, 54f)
        }, 2.2f, Cyan);

        chips = new Dimension1GalaxyVisualUI.MetalChip[3];
        chips[0] = CreateReferenceMetalChip(root, frame, font, 20f, "HIERRO", "5.98M", "+0.30/s", 0);
        chips[1] = CreateReferenceMetalChip(root, frame, font, 272f, "ALUMINIO", "4.73M", "+0.08/s", 1);
        chips[2] = CreateReferenceMetalChip(root, frame, font, 524f, "NÍQUEL", "4.70M", "+0.03/s", 2);

        RectTransform metals = CreateReferencePanel("AllMetals", root,
            new Vector2(776f, -102f), new Vector2(284f, 112f), Hex("031019", 250), Cyan);
        TMP_Text allMetals = CreateText("Label", metals, font, "10 METALES   ▼", 22f,
            FontStyles.Bold, Cyan);
        Stretch(allMetals.rectTransform, new Vector2(12f, 8f), new Vector2(-12f, -8f));
        allMetals.alignment = TextAlignmentOptions.Center;

        currentSector = CreateText("CurrentSectorProxy", root, font, "", 1f,
            FontStyles.Normal, Color.clear);
        SetTopRect(currentSector.rectTransform, 0f, 0f, 1f, 1f);
        unlockedSectors = CreateText("UnlockedSectorsProxy", root, font, "", 1f,
            FontStyles.Normal, Color.clear);
        SetTopRect(unlockedSectors.rectTransform, 0f, 0f, 1f, 1f);
        return backButton;
    }

    private static Dimension1GalaxyVisualUI.MetalChip CreateReferenceMetalChip(
        Transform parent, Sprite frame, TMP_FontAsset font, float x,
        string nameValue, string amountValue, string rateValue, int variant)
    {
        RectTransform card = CreateReferencePanel("Metal_" + nameValue, parent,
            new Vector2(x, -102f), new Vector2(236f, 112f), Hex("031019", 250), Cyan);
        // No vertical marker: the metal glyph already provides the visual anchor.
        Image marker = null;
        // The procedural glyph canvas is centred on the metal card.
        CreateReferenceMetalGlyph(card, new Vector2(-70f, 0f), variant);
        TMP_Text name = CreateText("Name", card, font, nameValue, 17f,
            FontStyles.Bold, Secondary);
        SetTopRect(name.rectTransform, 82f, 15f, 138f, 28f);
        TMP_Text amount = CreateText("Amount", card, font, amountValue, 31f,
            FontStyles.Bold, Primary);
        SetTopRect(amount.rectTransform, 80f, 39f, 112f, 42f);
        TMP_Text rate = CreateText("Rate", card, font, rateValue, 18f,
            FontStyles.Bold, Cyan);
        SetTopRect(rate.rectTransform, 80f, 76f, 130f, 28f);
        return new Dimension1GalaxyVisualUI.MetalChip
        {
            root = card.gameObject,
            nameText = name,
            amountText = amount,
            rateText = rate,
            marker = marker
        };
    }

    private static void CreateReferenceMetalGlyph(Transform parent, Vector2 center, int variant)
    {
        if (variant == 1)
        {
            CreateLocalPolygon("IngotFill", parent, new Vector2(236f, 112f), new[]
            {
                center + new Vector2(-20f, -22f), center + new Vector2(8f, -26f),
                center + new Vector2(24f, 22f), center + new Vector2(-6f, 25f)
            }, Hex("B8D9E8", 175));
            CreateLocalLine("IngotEdge", parent, new Vector2(236f, 112f), new[]
            {
                center + new Vector2(-20f, -22f), center + new Vector2(8f, -26f),
                center + new Vector2(24f, 22f), center + new Vector2(-6f, 25f)
            }, 2.5f, Hex("DDF7FF"), true);
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            Vector2 c = center + new Vector2((i - 1) * 20f, i == 1 ? 11f : -7f);
            float radius = i == 1 ? 18f : 15f;
            Vector2[] points =
            {
                c + new Vector2(-radius, -5f), c + new Vector2(-6f, radius),
                c + new Vector2(radius * 0.7f, radius * 0.65f),
                c + new Vector2(radius, -radius * 0.55f), c + new Vector2(0f, -radius)
            };
            CreateLocalPolygon("OreFill", parent, new Vector2(236f, 112f), points,
                variant == 2 ? Hex("B7C5C9", 170) : Hex("9DB5C5", 170));
            CreateLocalLine("OreEdge", parent, new Vector2(236f, 112f), points,
                2f, Hex("DDF7FF"), true);
        }
    }

    private static void CreateReferenceMapTitle(Transform map, TMP_FontAsset font)
    {
        TMP_Text title = CreateText("MapTitle", map, font, "CARTA GALÁCTICA", 47f,
            FontStyles.Bold, Primary);
        SetCenteredTopRect(title.rectTransform, 275f, 6f, 530f, 74f);
        title.alignment = TextAlignmentOptions.Center;
        title.characterSpacing = 6f;
        CreateAbsoluteLine("MapTitleCircuitLeft", map, new[]
        {
            new Vector2(55f, 62f), new Vector2(250f, 62f), new Vector2(273f, 82f),
            new Vector2(460f, 82f)
        }, 2f, Cyan, 1080f, 1260f);
        CreateAbsoluteLine("MapTitleCircuitRight", map, new[]
        {
            new Vector2(620f, 82f), new Vector2(807f, 82f), new Vector2(830f, 62f),
            new Vector2(1025f, 62f)
        }, 2f, Cyan, 1080f, 1260f);
    }

    private static Image[] CreateReferenceRoute(
        Transform parent, Vector2 from, Vector2 to, List<Image> cores)
    {
        Image glowLine = CreateRouteImage("RouteGlow", parent, from, to, 14f, Hex("0DAEEA", 70));
        Image coreLine = CreateRouteImage("RouteCore", parent, from, to, 4f, Hex("71E8FF", 205));
        cores.Add(coreLine);
        Image startNode = CreateRouteNode(parent, from, 18f);
        Image endNode = CreateRouteNode(parent, to, 14f);
        return new[] { glowLine, coreLine, startNode, endNode };
    }

    private static Image CreateRouteImage(
        string name, Transform parent, Vector2 from, Vector2 to, float thickness, Color color)
    {
        Vector2 delta = new Vector2(to.x - from.x, -(to.y - from.y));
        float length = delta.magnitude;
        RectTransform rect = CreateRect(name, parent, Vector2.zero, new Vector2(length, thickness));
        rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2((from.x + to.x) * 0.5f, -(from.y + to.y) * 0.5f);
        rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
        Image image = rect.gameObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static Image CreateRouteNode(Transform parent, Vector2 position, float size)
    {
        Image glowNode = CreateImage("RouteNodeGlow", parent,
            LoadSprite(ArtPath + "/d1_glow_v3.png"), Hex("24CFFF", 190));
        CenterAt(glowNode.rectTransform, new Vector2(position.x, -position.y));
        glowNode.rectTransform.sizeDelta = Vector2.one * size * 2f;
        glowNode.raycastTarget = false;
        return glowNode;
    }

    private static void BringRouteForward(Image[] route)
    {
        if (route == null) return;
        foreach (Image image in route)
            if (image != null) image.rectTransform.SetAsLastSibling();
    }

    private static Dimension1GalaxyVisualUI.SectorNodeView CreateReferenceSectorNode(
        string name, Transform parent, Sprite frame, Sprite orbit, Sprite glow, Sprite lockSprite,
        Sprite nebula, Dictionary<string, Sprite> bodies, TMP_FontAsset font, string sectorId,
        Vector2 center, Vector2 size, Image[] routes, List<RectTransform> ambientRotators,
        List<RectTransform> pulsingBodies)
    {
        RectTransform root = CreateRect(name, parent, Vector2.zero, size);
        CenterAt(root, new Vector2(center.x, -center.y));
        root.sizeDelta = size;
        Image hit = root.gameObject.AddComponent<Image>();
        hit.color = Color.clear;
        Button button = root.gameObject.AddComponent<Button>();
        button.targetGraphic = hit;
        button.transition = Selectable.Transition.None;

        bool isCenter = sectorId == Dimension1System.Sector05GalacticCenter;
        bool isSilent = sectorId == Dimension1System.Sector04SilentFrontier;
        Color accent = isCenter ? Hex("F4A300") : isSilent ? Hex("526673") : Cyan;
        Vector2[] hex = BuildHexPoints(size, 8f);
        CreateLocalPolygon("HexFill", root, size, hex, Hex("020B12", 252));

        Image art = CreateImage("SectorBackdrop", root, nebula,
            isCenter ? Hex("D2852A", 90) : isSilent ? Hex("7A8790", 65) : Hex("7EC9EB", 92));
        SetTopRect(art.rectTransform, 48f, 58f, size.x - 96f, size.y - 154f);
        art.preserveAspect = false;
        art.raycastTarget = false;

        Image glowImage = CreateImage("Glow", root, glow, accent);
        SetCenterRect(glowImage.rectTransform, new Vector2(0f, 6f),
            new Vector2(size.x * 0.84f, size.y * 0.70f));
        glowImage.raycastTarget = false;
        Image orbitImage = CreateImage("Orbit", root, orbit, Hex("38DFFF", 95));
        SetCenterRect(orbitImage.rectTransform, new Vector2(0f, 4f),
            new Vector2(size.x * 0.72f, size.x * 0.72f));
        orbitImage.raycastTarget = false;

        TMP_Text title = CreateText("Title", root, font, GetShortName(sectorId),
            isCenter ? 25f : 27f, FontStyles.Bold, Primary);
        SetTopRect(title.rectTransform, 35f, 18f, size.x - 70f, 42f);
        title.alignment = TextAlignmentOptions.Center;
        title.enableWordWrapping = false;

        Image primaryPlanet;
        GameObject lockGroup = new GameObject("LockState", typeof(RectTransform));
        RectTransform lockRect = lockGroup.GetComponent<RectTransform>();
        lockRect.SetParent(root, false);
        Stretch(lockRect);

        if (sectorId == Dimension1System.Sector01OuterRim)
        {
            primaryPlanet = CreateBodyImage("PlanetBlue", root, bodies["planet_blue"],
                45f, 82f, 150f, Color.white);
            CreateBodyImage("PlanetMolten", root, bodies["planet_molten"],
                225f, 87f, 140f, Color.white);
        }
        else if (sectorId == Dimension1System.Sector02DebrisRing)
        {
            primaryPlanet = CreateBodyImage("DebrisRing", root, bodies["debris_ring"],
                67.5f, 48f, 285f, Color.white);
            ambientRotators.Add(primaryPlanet.rectTransform);
        }
        else if (sectorId == Dimension1System.Sector03AncientOrbits)
        {
            primaryPlanet = CreateBodyImage("PlanetAncient", root, bodies["planet_ancient"],
                55f, 90f, 150f, Color.white);
            CreateBodyImage("PlanetBlue", root, bodies["planet_blue"],
                215f, 90f, 150f, Hex("CDEEFF", 255));
        }
        else if (isSilent)
        {
            primaryPlanet = CreateBodyImage("SilentPlanetA", root, bodies["planet_silent"],
                55f, 90f, 150f, Hex("8D969C", 220));
            CreateBodyImage("SilentPlanetB", root, bodies["planet_silent"],
                215f, 90f, 150f, Hex("7D858B", 215));
            CreateLockIcon(lockRect, lockSprite, new Vector2(130f, 179f), 54f);
            CreateLockIcon(lockRect, lockSprite, new Vector2(290f, 179f), 54f);
        }
        else
        {
            Image centerGlow = CreateBodyImage("CenterGlow", root, glow,
                70f, 49f, 260f, Hex("FF9800", 150));
            primaryPlanet = CreateBodyImage("BlackHole", root, bodies["black_hole"],
                78f, 54f, 244f, Color.white);
            ambientRotators.Add(primaryPlanet.rectTransform);
            pulsingBodies.Add(centerGlow.rectTransform);
            CreateLockIcon(lockRect, lockSprite, new Vector2(size.x * 0.5f, 238f), 58f);
        }

        float footerTop = size.y - 112f;
        Image labelPlate = null;
        Image labelBorder = null;
        if (isCenter)
        {
            labelPlate = CreateImage("LabelPlate", root, null, Hex("271A04", 248));
            SetTopRect(labelPlate.rectTransform, 76f, footerTop, size.x - 152f, 88f);
            labelPlate.raycastTarget = false;
            labelBorder = CreateImage("LabelBorder", root, frame, accent);
            SetTopRect(labelBorder.rectTransform, 68f, footerTop - 5f, size.x - 136f, 98f);
            labelBorder.type = Image.Type.Sliced;
            labelBorder.raycastTarget = false;
        }

        TMP_Text state = CreateText("State", root, font,
            isCenter ? "???" : "0\nEXPEDICIONES", isCenter ? 34f : 24f,
            FontStyles.Bold, isCenter ? Amber : Primary);
        SetTopRect(state.rectTransform, isCenter ? 100f : 60f,
            footerTop + 3f, isCenter ? size.x - 200f : size.x - 120f, 78f);
        state.alignment = TextAlignmentOptions.Center;
        state.lineSpacing = -8f;

        GameObject currentBadge = null;
        if (!isCenter)
        {
            RectTransform badge = CreateReferencePanel("CurrentBadge", root,
                new Vector2((size.x - 148f) * 0.5f, -(footerTop - 27f)),
                new Vector2(148f, 34f), Hex("04202B", 252), Cyan);
            TMP_Text badgeText = CreateText("Label", badge, font, "ACTUAL", 18f,
                FontStyles.Bold, Cyan);
            Stretch(badgeText.rectTransform, new Vector2(8f, 2f), new Vector2(-8f, -2f));
            badgeText.alignment = TextAlignmentOptions.Center;
            currentBadge = badge.gameObject;

        }

        CreateLocalLine("HexOuter", root, size, hex, 3.1f, accent, true);
        CreateLocalLine("HexInner", root, size, BuildHexPoints(size, 18f), 1.3f,
            new Color(accent.r, accent.g, accent.b, 0.54f), true);

        if (!isCenter && !isSilent)
            lockGroup.SetActive(false);
        lockGroup.transform.SetAsLastSibling();

        return new Dimension1GalaxyVisualUI.SectorNodeView
        {
            sectorId = sectorId,
            root = root,
            planet = primaryPlanet,
            orbitRing = orbitImage,
            glow = glowImage,
            labelPlate = labelPlate,
            labelBorder = labelBorder,
            routes = routes,
            lockBadge = lockGroup,
            currentBadge = currentBadge,
            titleText = title,
            stateText = state
        };
    }

    private static Image CreateBodyImage(
        string name, Transform parent, Sprite sprite, float x, float y, float size, Color color)
    {
        Image image = CreateImage(name, parent, sprite, color);
        SetCenteredTopRect(image.rectTransform, x, y, size, size);
        image.raycastTarget = false;
        return image;
    }

    private static void GenerateMoltenTransparentAsset()
    {
        TextureImporter sourceImporter = AssetImporter.GetAtPath(MoltenPlanetSourcePath) as TextureImporter;
        if (sourceImporter == null)
            throw new InvalidOperationException("No se encontró el planeta fundido de origen.");

        bool restoreReadable = !sourceImporter.isReadable;
        if (restoreReadable)
        {
            sourceImporter.isReadable = true;
            sourceImporter.SaveAndReimport();
        }

        Texture2D source = AssetDatabase.LoadAssetAtPath<Texture2D>(MoltenPlanetSourcePath);
        if (source == null) throw new InvalidOperationException("No se pudo leer el planeta fundido.");
        Color32[] pixels = source.GetPixels32();
        for (int i = 0; i < pixels.Length; i++)
        {
            Color32 pixel = pixels[i];
            byte maximum = Math.Max(pixel.r, Math.Max(pixel.g, pixel.b));
            byte minimum = Math.Min(pixel.r, Math.Min(pixel.g, pixel.b));
            bool brightNeutralBackground = minimum >= 205 && maximum - minimum <= 18;
            pixel.a = brightNeutralBackground ? (byte)0 : (byte)255;
            pixels[i] = pixel;
        }

        Texture2D output = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        output.SetPixels32(pixels);
        output.Apply(false, false);
        File.WriteAllBytes(Path.GetFullPath(MoltenPlanetPath), output.EncodeToPNG());
        UnityEngine.Object.DestroyImmediate(output);

        if (restoreReadable)
        {
            sourceImporter.isReadable = false;
            sourceImporter.SaveAndReimport();
        }
        AssetDatabase.ImportAsset(MoltenPlanetPath, ImportAssetOptions.ForceSynchronousImport);
    }

    private static void CreateLockIcon(Transform parent, Sprite sprite, Vector2 center, float size)
    {
        Image lockImage = CreateImage("Lock", parent, sprite, Hex("D7DEE2", 235));
        SetTopRect(lockImage.rectTransform, center.x - size * 0.5f,
            center.y - size * 0.5f, size, size);
        lockImage.raycastTarget = false;
    }

    private static ReferenceDetails CreateReferenceSelectionPanel(
        Transform root, Sprite frame, TMP_FontAsset font,
        Dictionary<string, Sprite> bodies, Dimension1PanelUI panel)
    {
        RectTransform selection = CreateReferencePanel("SectorDetails", root,
            new Vector2(22f, -1515f), new Vector2(1036f, 182f), Hex("020B12", 252), Cyan);

        var details = new ReferenceDetails();
        details.title = CreateText("SelectedSectorTitle", selection, font,
            "SELECCIONA UN SECTOR", 42f, FontStyles.Bold, Primary);
        SetCenteredTopRect(details.title.rectTransform, 170f, 15f, 696f, 62f);
        details.title.alignment = TextAlignmentOptions.Center;
        details.title.characterSpacing = 4f;

        details.neutralInstruction = CreateText("NeutralInstruction", selection, font,
            "TOCA UN SECTOR PARA VER SUS DATOS.", 25f, FontStyles.Normal, Secondary);
        SetCenteredTopRect(details.neutralInstruction.rectTransform, 160f, 100f, 716f, 50f);
        details.neutralInstruction.alignment = TextAlignmentOptions.Center;
        details.neutralInstruction.characterSpacing = 2f;

        RectTransform selectedData = CreateRect("SelectedData", selection, Vector2.zero,
            new Vector2(1036f, 182f));
        Stretch(selectedData);
        Image selectedHit = CreateImage("Hit", selectedData, null, Color.clear);
        Stretch(selectedHit.rectTransform);
        details.enterButton = selectedData.gameObject.AddComponent<Button>();
        details.enterButton.targetGraphic = selectedHit;
        details.enterButton.transition = Selectable.Transition.None;

        details.explorations = CreateText("ExplorationCount", selectedData, font, "0",
            34f, FontStyles.Bold, Cyan);
        SetTopRect(details.explorations.rectTransform, 155f, 92f, 110f, 50f);
        details.explorations.alignment = TextAlignmentOptions.Center;
        details.status = CreateText("SelectedStatus", selectedData, font, "SECTOR DISPONIBLE",
            22f, FontStyles.Bold, Amber);
        SetTopRect(details.status.rectTransform, 275f, 94f, 310f, 45f);
        details.status.alignment = TextAlignmentOptions.Center;
        details.destinations = CreateText("Destinations", selectedData, font, "DESTINOS",
            18f, FontStyles.Normal, Secondary);
        SetTopRect(details.destinations.rectTransform, 605f, 80f, 230f, 72f);
        details.destinations.alignment = TextAlignmentOptions.Center;
        details.requirements = CreateText("Requirements", selectedData, font, "REQUISITOS",
            17f, FontStyles.Normal, Secondary);
        SetTopRect(details.requirements.rectTransform, 820f, 75f, 190f, 82f);
        details.requirements.alignment = TextAlignmentOptions.Center;
        details.planetPreview = CreateImage("PlanetPreviewProxy", selectedData,
            bodies["planet_blue"], Color.clear);
        SetTopRect(details.planetPreview.rectTransform, 20f, 75f, 1f, 1f);
        details.secondaryPreview = CreateImage("SecondaryPlanetProxy", selectedData,
            bodies["planet_habitable"], Color.clear);
        SetTopRect(details.secondaryPreview.rectTransform, 22f, 75f, 1f, 1f);
        details.detailRoots = new[] { selectedData.gameObject };

        details.summaryProxy = CreateText("GalaxySectorSummaryTextV3", selection, font,
            "", 1f, FontStyles.Normal, Color.clear);
        SetTopRect(details.summaryProxy.rectTransform, 1f, 1f, 1f, 1f);
        return details;
    }

    private static Vector2[] BuildHexPoints(Vector2 size, float inset)
    {
        float halfW = size.x * 0.5f - inset;
        float halfH = size.y * 0.5f - inset;
        float shoulder = Mathf.Min(112f, size.x * 0.24f);
        return new[]
        {
            new Vector2(-halfW, 0f),
            new Vector2(-halfW + shoulder, halfH),
            new Vector2(halfW - shoulder, halfH),
            new Vector2(halfW, 0f),
            new Vector2(halfW - shoulder, -halfH),
            new Vector2(-halfW + shoulder, -halfH)
        };
    }

    private static Dimension1CommandCenterLineGraphic CreateLocalLine(
        string name, Transform parent, Vector2 size, IList<Vector2> points,
        float thickness, Color color, bool closed = false)
    {
        RectTransform rect = CreateGraphicRect(name, parent, size);
        Dimension1CommandCenterLineGraphic line = rect.gameObject.AddComponent<Dimension1CommandCenterLineGraphic>();
        line.color = color;
        line.raycastTarget = false;
        line.SetLine(points, thickness, closed);
        return line;
    }

    private static Dimension1CommandCenterPolygonGraphic CreateLocalPolygon(
        string name, Transform parent, Vector2 size, IList<Vector2> points, Color color)
    {
        RectTransform rect = CreateGraphicRect(name, parent, size);
        Dimension1CommandCenterPolygonGraphic polygon = rect.gameObject.AddComponent<Dimension1CommandCenterPolygonGraphic>();
        polygon.color = color;
        polygon.raycastTarget = false;
        polygon.SetPolygon(points);
        return polygon;
    }

    private static RectTransform CreateGraphicRect(string name, Transform parent, Vector2 size)
    {
        RectTransform rect = CreateRect(name, parent, Vector2.zero, size);
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;
        return rect;
    }

    private static void CreateAbsoluteLine(
        string name, Transform parent, IList<Vector2> topCoordinates,
        float thickness, Color color, float width = 1080f, float height = 1920f)
    {
        var local = new List<Vector2>();
        foreach (Vector2 point in topCoordinates)
            local.Add(new Vector2(point.x - width * 0.5f, height * 0.5f - point.y));
        CreateLocalLine(name, parent, new Vector2(width, height), local, thickness, color);
    }

    private static RectTransform CreateReferencePanel(
        string name, Transform parent, Vector2 topLeft, Vector2 size, Color fillColor, Color accent)
    {
        RectTransform root = CreateRect(name, parent, topLeft, size);
        SetTopLeft(root, topLeft, size);
        Vector2[] outer = BuildChamferedPanelPoints(size, 3f, 15f);
        Vector2[] inner = BuildChamferedPanelPoints(size, 10f, 11f);
        CreateLocalPolygon("PanelFill", root, size, outer, fillColor);
        Image fill = CreateImage("Fill", root, null, Color.clear);
        Stretch(fill.rectTransform);
        CreateLocalLine("OuterLine", root, size, outer, 2.4f, accent, true);
        CreateLocalLine("InnerLine", root, size, inner, 1.15f,
            new Color(accent.r, accent.g, accent.b, 0.48f), true);
        return root;
    }

    private static Vector2[] BuildChamferedPanelPoints(Vector2 size, float inset, float corner)
    {
        float left = -size.x * 0.5f + inset;
        float right = size.x * 0.5f - inset;
        float top = size.y * 0.5f - inset;
        float bottom = -size.y * 0.5f + inset;
        return new[]
        {
            new Vector2(left + corner, top), new Vector2(right - corner, top),
            new Vector2(right, top - corner), new Vector2(right, bottom + corner),
            new Vector2(right - corner, bottom), new Vector2(left + corner, bottom),
            new Vector2(left, bottom + corner), new Vector2(left, top - corner)
        };
    }

    private static void CreateReferenceBottomNavigation(
        RectTransform navigation, TMP_FontAsset font, Dimension1PanelUI panel)
    {
        const float width = Dimension1SharedLayoutTokens.NavigationCardWidth;
        Sprite[] icons =
        {
            LoadSprite(ArtPath + "/d1_nav_galaxy_v3.png"),
            LoadSprite(ArtPath + "/d1_nav_explore_v3.png"),
            LoadSprite(ArtPath + "/d1_nav_hangar_v3.png"),
            LoadSprite(ArtPath + "/d1_nav_relics_v3.png"),
            LoadSprite(ArtPath + "/d1_nav_tree_v3.png")
        };
        string[] labels = { "GALAXIA", "EXPLORAR", "HANGAR", "RELIQUIAS", "ÁRBOL" };
        Button[] buttons = new Button[5];
        for (int i = 0; i < 5; i++)
        {
            bool active = i == 0;
            Color accent = active ? Amber : Cyan;
            RectTransform card = CreateReferencePanel(labels[i] + "Button", navigation,
                new Vector2(Dimension1SharedLayoutTokens.NavigationCardX(i),
                    -Dimension1SharedLayoutTokens.NavigationCardY),
                new Vector2(width, Dimension1SharedLayoutTokens.NavigationCardHeight),
                active ? Hex("171609", 252) : Hex("020B12", 252), accent);
            Image hit = FindChild(card, "Fill").GetComponent<Image>();
            buttons[i] = card.gameObject.AddComponent<Button>();
            buttons[i].targetGraphic = hit;
            buttons[i].transition = Selectable.Transition.None;
            Image icon = CreateImage("Icon", card, icons[i], active ? Amber : Hex("54C8ED"));
            SetTopRect(icon.rectTransform, 66f, 12f, 68f, 68f);
            icon.raycastTarget = false;
            TMP_Text label = CreateText("Label", card, font, labels[i], 18f,
                FontStyles.Bold, active ? Amber : Secondary);
            SetTopRect(label.rectTransform, 8f, 100f, width - 16f, 36f);
            label.alignment = TextAlignmentOptions.Center;
        }
        buttons[0].interactable = false;
        AddPersistent(buttons[1].onClick, panel.OnClickCloseGalaxyPanel);
        AddPersistent(buttons[2].onClick, panel.OnClickCloseGalaxyPanel);
        AddPersistent(buttons[2].onClick, panel.OnClickOpenHangarPanel);
        AddPersistent(buttons[3].onClick, panel.OnClickCloseGalaxyPanel);
        AddPersistent(buttons[3].onClick, panel.OnClickOpenRelicChamberPanel);
        AddPersistent(buttons[4].onClick, panel.OnClickCloseGalaxyPanel);
        AddPersistent(buttons[4].onClick, panel.OnClickOpenDimension1TreePanel);
    }

    private static void CreateReferenceStars(Transform root)
    {
        Vector2[] positions =
        {
            new Vector2(112f, -70f), new Vector2(366f, -44f), new Vector2(558f, -92f),
            new Vector2(771f, -55f), new Vector2(966f, -122f), new Vector2(76f, -271f),
            new Vector2(1010f, -358f), new Vector2(54f, -761f), new Vector2(1000f, -1184f),
            new Vector2(88f, -1452f), new Vector2(968f, -1510f)
        };
        for (int i = 0; i < positions.Length; i++)
        {
            Image star = CreateImage("AmbientStar", root, null,
                Hex("1ECFFF", (byte)(i % 3 == 0 ? 80 : 42)));
            CenterAt(star.rectTransform, positions[i]);
            float size = i % 3 == 0 ? 4f : 2f;
            star.rectTransform.sizeDelta = Vector2.one * size;
            star.raycastTarget = false;
        }
    }

    private static Material GetOrCreateMoltenMaterial()
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>(MoltenMaterialPath);
        Shader shader = Shader.Find("UI/Quantum Forge Chroma Key");
        if (shader == null) throw new InvalidOperationException("Falta el shader de transparencia del planeta fundido.");
        if (material == null)
        {
            material = new Material(shader) { name = "D1MoltenPlanetChromaKey" };
            AssetDatabase.CreateAsset(material, MoltenMaterialPath);
        }
        else if (material.shader != shader)
        {
            material.shader = shader;
            EditorUtility.SetDirty(material);
        }
        return material;
    }
}
#endif

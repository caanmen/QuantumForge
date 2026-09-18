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

public static class Dimension1ExploreReferenceSetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ArtPath = "Assets/Project/UI/Dimension1/Generated";
    private const string BlueprintMaterialPath = ArtPath + "/d1_hangar_blueprint_keyed.mat";
    private static readonly string[] ShipBlueprintPaths =
    {
        ArtPath + "/d1_hangar_sonda_ligera_blueprint_v2.png",
        ArtPath + "/d1_hangar_dron_extractor_blueprint_v2.png",
        ArtPath + "/d1_hangar_sonda_analitica_blueprint_v2.png",
        ArtPath + "/d1_hangar_nave_carga_blueprint_v2.png"
    };
    private static readonly string[] SectorArtworkIds =
    {
        Dimension1System.Sector01OuterRim,
        Dimension1System.Sector02DebrisRing,
        Dimension1System.Sector03AncientOrbits,
        Dimension1System.Sector04SilentFrontier
    };
    private static readonly string[] SectorArtworkPaths =
    {
        ArtPath + "/d1_body_planet_blue_v3.png",
        ArtPath + "/Candidates/DebrisRing/d1_debris_ring_option_1_dense_orbit.png",
        ArtPath + "/d1_body_planet_ancient_v3.png",
        ArtPath + "/d1_body_planet_silent_v3.png"
    };
    private const string RootName = "D1_ExploreVisualRoot";
    private const float W = 1080f;
    private const float H = 1920f;

    private static readonly Color Void = Hex("01090E");
    private static readonly Color Fill = Hex("04121B", 248);
    private static readonly Color FillRaised = Hex("071924", 250);
    private static readonly Color Cyan = Hex("18C8FF");
    private static readonly Color CyanBright = Hex("8DEAFF");
    private static readonly Color CyanMuted = Hex("087FA9");
    private static readonly Color Primary = Hex("EDF4F7");
    private static readonly Color Secondary = Hex("A8B1B8");
    private static readonly Color Green = Hex("20C99A");
    private static readonly Color Amber = Hex("F4A70B");

    private sealed class Refs
    {
        public readonly List<TMP_Text> metalAmounts = new List<TMP_Text>();
        public readonly List<TMP_Text> metalRates = new List<TMP_Text>();
        public TMP_Text sectorName;
        public Image sectorArtwork;
        public TMP_Text scannerLevel;
        public TMP_Text destinationName;
        public TMP_Text destinationLevel;
        public TMP_Text destinationDistance;
        public TMP_Text destinationCount;
        public TMP_Text shipName;
        public TMP_Text shipStatus;
        public TMP_Text shipMetricLabel;
        public TMP_Text shipSpeed;
        public TMP_Text supportName;
        public TMP_Text supportStatus;
        public TMP_Text supportBonus;
        public TMP_Text supportAvailability;
        public TMP_Text activeShip;
        public TMP_Text activeDestination;
        public TMP_Text activeTimer;
        public TMP_Text activeCount;
        public TMP_Text scannerUpgradeLabel;
        public TMP_Text startLabel;
        public Image shipIllustration;
        public Image supportIllustration;
        public Button scannerUpgrade;
        public Button commandCenter;
        public Button start;
        public Button record;
        public Button openDetails;
        public Button closeDetails;
        public GameObject detailsOverlay;
        public TMP_Text detailsText;
        public RectTransform detailsContent;
        public ScrollRect detailsScroll;
        public readonly List<Button> destinationCards = new List<Button>();
        public readonly List<Image> destinationCardImages = new List<Image>();
        public readonly List<Image> destinationCardBorders = new List<Image>();
        public readonly List<TMP_Text> destinationCardLabels = new List<TMP_Text>();
        public Image simpleModeBorder;
        public Image coordinatedModeBorder;
        public TMP_Text simpleModeStatus;
        public TMP_Text coordinatedModeStatus;
        public readonly List<RectTransform> pulses = new List<RectTransform>();
    }

    [MenuItem("Quantum Forge/Dimension 1/Install Explore Reference Screen")]
    public static void Install()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Dimension1PanelUI panel = FindSceneComponent<Dimension1PanelUI>(scene);
        if (panel == null) throw new InvalidOperationException("No existe Dimension1PanelUI en Main.unity.");

        foreach (string path in ShipBlueprintPaths)
            PrepareUiSprite(path);
        foreach (string path in SectorArtworkPaths)
            PrepareUiSprite(path);
        TMP_FontAsset font = FindFont(panel.transform);
        Sprite frame = LoadSprite(ArtPath + "/d1_premium_frame_v4.png");
        Sprite fillSprite = LoadSprite(ArtPath + "/d1_panel_fill_v4.png");
        Sprite starfield = LoadSprite(ArtPath + "/d1_starfield.png");
        Material blueprintMaterial = AssetDatabase.LoadAssetAtPath<Material>(BlueprintMaterialPath);
        Sprite[] shipBlueprints = new Sprite[ShipBlueprintPaths.Length];
        for (int i = 0; i < ShipBlueprintPaths.Length; i++)
            shipBlueprints[i] = LoadSprite(ShipBlueprintPaths[i]);
        Sprite[] sectorArtworkSprites = Array.ConvertAll(SectorArtworkPaths, LoadSprite);
        Sprite activeBadge = LoadSprite(ArtPath + "/Candidates/ExpeditionResult/d1_expedition_complete_badge_v1.png");
        string[] destinationArtIds =
        {
            Dimension1System.DestinationMineralBelt,
            Dimension1System.DestinationShipGraveyard,
            Dimension1System.DestinationAbandonedShip,
            Dimension1System.DestinationOrbitalRuin,
            Dimension1System.DestinationDriftingProbes,
            Dimension1System.DestinationLaboratory,
            Dimension1System.DestinationAbandonedStation,
            Dimension1System.DestinationMinorAnomaly,
            Dimension1System.DestinationAncientStructure,
            Dimension1System.DestinationUnstableZone
        };
        string[] destinationArtPaths =
        {
            ArtPath + "/Candidates/SectorDetails/d1_destination_mineral_belt_v2.png",
            ArtPath + "/Candidates/SectorDetails/d1_destination_ship_graveyard_v2.png",
            ArtPath + "/Candidates/AncientOrbits/d1_destination_abandoned_ship_v2.png",
            ArtPath + "/Candidates/AncientOrbits/d1_destination_orbital_ruin_v2.png",
            ArtPath + "/Candidates/SectorDetails/d1_destination_drifting_probes_v2.png",
            ArtPath + "/Candidates/AncientOrbits/d1_destination_laboratory_v2.png",
            ArtPath + "/Candidates/AncientOrbits/d1_destination_abandoned_station_v2.png",
            ArtPath + "/Candidates/SectorDetails/d1_destination_minor_anomaly_v2.png",
            ArtPath + "/Candidates/SectorDetails/d1_destination_ancient_structure_v2.png",
            ArtPath + "/Candidates/SectorDetails/d1_destination_unstable_zone_v2.png"
        };
        Sprite[] destinationArt = Array.ConvertAll(destinationArtPaths, LoadSprite);
        if (font == null || frame == null || fillSprite == null || blueprintMaterial == null ||
            activeBadge == null ||
            Array.Exists(shipBlueprints, sprite => sprite == null) ||
            Array.Exists(sectorArtworkSprites, sprite => sprite == null) ||
            Array.Exists(destinationArt, sprite => sprite == null))
            throw new InvalidOperationException("Faltan recursos base para la pantalla Explorar.");

        Transform previous = FindDirectChild(panel.transform, RootName);
        if (previous != null) UnityEngine.Object.DestroyImmediate(previous.gameObject);

        RectTransform root = Rect(RootName, panel.transform);
        root.anchorMin = root.anchorMax = new Vector2(.5f, .5f);
        root.pivot = new Vector2(.5f, .5f);
        root.anchoredPosition = Dimension1SharedLayoutTokens.RootOffset;
        root.sizeDelta = new Vector2(Dimension1SharedLayoutTokens.Width,
            Dimension1SharedLayoutTokens.Height);
        root.SetAsLastSibling();
        root.gameObject.AddComponent<Dimension1VisualSkinRoot>();
        Canvas canvas = root.gameObject.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 31900;
        root.gameObject.AddComponent<GraphicRaycaster>();
        CanvasGroup group = root.gameObject.AddComponent<CanvasGroup>();
        Dimension1ExploreVisualUI visual = root.gameObject.AddComponent<Dimension1ExploreVisualUI>();

        Image background = Image("Background", root, null, Void);
        Stretch(background.rectTransform, new Vector2(-24f, -32f), new Vector2(-24f, -32f));
        background.raycastTarget = true;
        Image stars = Image("Starfield", root, starfield, Hex("6DC9E6", 5));
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
        BuildHeader(root, frame, fillSprite, font, refs, visual);
        BuildTitle(root, font);
        BuildScanner(root, frame, fillSprite, font, refs, panel, visual, starfield,
            sectorArtworkSprites[0]);
        BuildDestination(root, frame, fillSprite, font, refs, visual, destinationArt);
        BuildShip(root, frame, fillSprite, font, refs, visual, shipBlueprints[0], blueprintMaterial);
        BuildSupport(root, frame, fillSprite, font, refs, visual, shipBlueprints[1], blueprintMaterial);
        BuildMode(root, frame, fillSprite, font, refs, visual);
        BuildActiveExpedition(root, frame, fillSprite, font, refs, visual, activeBadge);
        BuildActions(root, frame, fillSprite, font, refs, panel, visual);
        BuildNavigation(root, frame, fillSprite, font, refs, panel);
        BuildPreviewDetails(root, frame, fillSprite, font, refs, visual);
        Dimension1SharedShellApply.ApplyToRoot(root);

        GameObject[] blockers = FindObjects(panel.transform,
            "GalaxyPanel", "HangarPanel", "RelicChamberPanel", "Dimension1TreePanel",
            "ArkPanel", "Exploration Record Panel", "D1_ExpeditionRecordVisualRoot");
        GameObject[] navRoots = FindNavigationRoots(scene);

        SerializedObject so = new SerializedObject(visual);
        Assign(so, "panel", panel);
        Assign(so, "commandCenter", FindSceneComponent<Dimension1CommandCenterUI>(scene));
        Assign(so, "canvasGroup", group);
        SetObjectArray(so, "blockingPanels", blockers);
        SetObjectArray(so, "hideWhileOpen", navRoots);
        SetObjectArray(so, "metalAmounts", refs.metalAmounts.ToArray());
        SetObjectArray(so, "metalRates", refs.metalRates.ToArray());
        Assign(so, "sectorName", refs.sectorName);
        Assign(so, "sectorArtwork", refs.sectorArtwork);
        SetStringArray(so, "sectorArtworkIds", SectorArtworkIds);
        SetObjectArray(so, "sectorArtworkSprites", sectorArtworkSprites);
        Assign(so, "scannerLevel", refs.scannerLevel);
        Assign(so, "destinationName", refs.destinationName);
        Assign(so, "destinationLevel", refs.destinationLevel);
        Assign(so, "destinationDistance", refs.destinationDistance);
        Assign(so, "destinationCount", refs.destinationCount);
        Assign(so, "shipName", refs.shipName);
        Assign(so, "shipStatus", refs.shipStatus);
        Assign(so, "shipMetricLabel", refs.shipMetricLabel);
        Assign(so, "shipSpeed", refs.shipSpeed);
        Assign(so, "shipIllustration", refs.shipIllustration);
        Assign(so, "supportIllustration", refs.supportIllustration);
        SetObjectArray(so, "shipIllustrations", shipBlueprints);
        Assign(so, "supportName", refs.supportName);
        Assign(so, "supportStatus", refs.supportStatus);
        Assign(so, "supportBonus", refs.supportBonus);
        Assign(so, "supportAvailability", refs.supportAvailability);
        Assign(so, "activeShip", refs.activeShip);
        Assign(so, "activeDestination", refs.activeDestination);
        Assign(so, "activeTimer", refs.activeTimer);
        Assign(so, "activeCount", refs.activeCount);
        Assign(so, "scannerUpgradeLabel", refs.scannerUpgradeLabel);
        Assign(so, "scannerUpgradeButton", refs.scannerUpgrade);
        Assign(so, "startButtonLabel", refs.startLabel);
        Assign(so, "startButton", refs.start);
        SetObjectArray(so, "destinationCardButtons", refs.destinationCards.ToArray());
        SetObjectArray(so, "destinationCardImages", refs.destinationCardImages.ToArray());
        SetObjectArray(so, "destinationCardBorders", refs.destinationCardBorders.ToArray());
        SetObjectArray(so, "destinationCardLabels", refs.destinationCardLabels.ToArray());
        SetStringArray(so, "destinationArtIds", destinationArtIds);
        SetObjectArray(so, "destinationArtSprites", destinationArt);
        Assign(so, "simpleModeBorder", refs.simpleModeBorder);
        Assign(so, "coordinatedModeBorder", refs.coordinatedModeBorder);
        Assign(so, "simpleModeStatus", refs.simpleModeStatus);
        Assign(so, "coordinatedModeStatus", refs.coordinatedModeStatus);
        Assign(so, "previewDetailsOverlay", refs.detailsOverlay);
        Assign(so, "previewDetailsText", refs.detailsText);
        Assign(so, "previewDetailsContent", refs.detailsContent);
        Assign(so, "previewDetailsScroll", refs.detailsScroll);
        so.ApplyModifiedPropertiesWithoutUndo();

        Dimension1CommandCenterUI commandCenter = FindSceneComponent<Dimension1CommandCenterUI>(scene);
        if (commandCenter != null)
        {
            commandCenter.ConfigureExploreScreen(root.gameObject);
            EditorUtility.SetDirty(commandCenter);
        }

        refs.detailsOverlay.SetActive(false);
        root.gameObject.SetActive(false);
        EditorUtility.SetDirty(visual);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("Unity no pudo guardar Main.unity.");
        ValidateInternal(scene);
        Debug.Log("[D1 Explore] INSTALL_PASS | referencia 507x876 | destino 1080x1920 | navegación y datos reales conectados");
    }

    [MenuItem("Quantum Forge/Dimension 1/Validate Explore Reference Screen")]
    public static void Validate()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        ValidateInternal(scene);
        Debug.Log("[D1 Explore] VALIDATION_PASS");
    }

    private static void BuildHeader(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font, Refs refs,
        Dimension1ExploreVisualUI visual)
    {
        TMP_Text title = Text("DimensionTitle", root, font, "DIMENSIÓN 1", 42f, FontStyles.Bold, Primary);
        Top(title.rectTransform, 285f, 3f, 510f, 58f);
        title.alignment = TextAlignmentOptions.Center;
        title.characterSpacing = 4f;

        refs.commandCenter = ButtonPanel("CommandCenter", root, frame, fill,
            new Vector2(14f, 70f), new Vector2(142f, 118f), Fill, Cyan);
        AddPersistent(refs.commandCenter.onClick, visual.OpenCommandCenter);
        DrawCommandCompass(refs.commandCenter.transform, new Vector2(0f, 13f), 26f, Cyan);
        TMP_Text home = Text("Label", refs.commandCenter.transform, font, "CENTRO DE MANDO", 14f, FontStyles.Bold, Cyan);
        Top(home.rectTransform, 6f, 77f, 130f, 30f);
        home.alignment = TextAlignmentOptions.Center;

        string[] names = { "HIERRO", "ALUMINIO", "NÍQUEL" };
        float[] widths = { 192f, 192f, 192f };
        float x = 170f;
        for (int i = 0; i < 3; i++)
        {
            RectTransform chip = Panel("Metal_" + i, root, frame, fill,
                new Vector2(x, 70f), new Vector2(widths[i], 118f), Fill, CyanMuted);
            DrawCrystal(chip, new Vector2(-69f, 0f), 30f, i, Secondary);
            TMP_Text name = Text("Name", chip, font, names[i], 17f, FontStyles.Bold, Secondary);
            Top(name.rectTransform, 55f, 14f, 125f, 25f);
            TMP_Text amount = Text("Amount", chip, font, "0", 29f, FontStyles.Normal, Primary);
            Top(amount.rectTransform, 52f, 35f, 126f, 42f);
            TMP_Text rate = Text("Rate", chip, font, "+0/s", 17f, FontStyles.Bold, Cyan);
            Top(rate.rectTransform, 105f, 76f, 72f, 27f);
            rate.alignment = TextAlignmentOptions.Right;
            refs.metalAmounts.Add(amount);
            refs.metalRates.Add(rate);
            x += widths[i] + 8f;
        }

        RectTransform metals = Panel("MetalsButton", root, frame, fill,
            new Vector2(770f, 70f), new Vector2(242f, 118f), Fill, CyanMuted);
        DrawCrystal(metals, new Vector2(-88f, 0f), 22f, 4, Cyan);
        TMP_Text metalLabel = Text("Label", metals, font, "10 METALES", 20f, FontStyles.Bold, Cyan);
        Top(metalLabel.rectTransform, 58f, 38f, 145f, 42f);
        metalLabel.alignment = TextAlignmentOptions.Center;
        Line("ChevronA", metals, new Vector2(91f, 5f), new Vector2(99f, -3f), 2f, Cyan);
        Line("ChevronB", metals, new Vector2(99f, -3f), new Vector2(107f, 5f), 2f, Cyan);
    }

    private static void BuildTitle(Transform root, TMP_FontAsset font)
    {
        TMP_Text title = Text("ExploreTitle", root, font, "EXPLORAR", 43f, FontStyles.Bold, Primary);
        Top(title.rectTransform, 340f, 205f, 400f, 58f);
        title.alignment = TextAlignmentOptions.Center;
        title.characterSpacing = 5f;
        RectTransform decor = Rect("TitleDecor", root);
        Top(decor, 24f, 205f, 1032f, 58f);
        Line("TitleLineLeft", decor, new Vector2(-490f, 0f), new Vector2(-235f, 0f), 2f, CyanMuted);
        Line("TitleLineRight", decor, new Vector2(235f, 0f), new Vector2(490f, 0f), 2f, CyanMuted);
        Line("TitleOuterLeftA", decor, new Vector2(-524f, -13f), new Vector2(-507f, 0f), 2f, CyanMuted);
        Line("TitleOuterLeftB", decor, new Vector2(-507f, 0f), new Vector2(-490f, 0f), 2f, CyanMuted);
        Line("TitleOuterRightA", decor, new Vector2(524f, -13f), new Vector2(507f, 0f), 2f, CyanMuted);
        Line("TitleOuterRightB", decor, new Vector2(507f, 0f), new Vector2(490f, 0f), 2f, CyanMuted);
        Dot(decor, new Vector2(-218f, 0f), 6f, Cyan);
        Dot(decor, new Vector2(218f, 0f), 6f, Cyan);
        Line("TitleStepLeft", decor, new Vector2(-235f, 0f), new Vector2(-218f, -12f), 2f, CyanMuted);
        Line("TitleStepRight", decor, new Vector2(235f, 0f), new Vector2(218f, -12f), 2f, CyanMuted);
    }

    private static void BuildScanner(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font, Refs refs,
        Dimension1PanelUI functionalPanel, Dimension1ExploreVisualUI visual,
        Sprite starfield, Sprite initialSectorArtwork)
    {
        RectTransform panel = Panel("ScannerPanel", root, frame, fill,
            new Vector2(62f, 276f), new Vector2(954f, 350f), Fill, CyanMuted);
        Image stars = Image("SectorStarfield", panel, starfield, Hex("BFEFFF", 115));
        Stretch(stars.rectTransform, new Vector2(4f, 4f), new Vector2(4f, 4f));
        stars.preserveAspect = false;
        stars.raycastTarget = false;
        RectTransform artworkViewport = new GameObject("SectorArtworkViewport", typeof(RectTransform), typeof(RectMask2D)).GetComponent<RectTransform>();
        artworkViewport.gameObject.layer = panel.gameObject.layer;
        artworkViewport.SetParent(panel, false);
        Stretch(artworkViewport, new Vector2(4f, 4f), new Vector2(4f, 4f));
        refs.sectorArtwork = Image("SectorArtwork", artworkViewport, initialSectorArtwork, Color.white);
        Top(refs.sectorArtwork.rectTransform, 500f, -78f, 520f, 520f);
        refs.sectorArtwork.preserveAspect = true;
        refs.sectorArtwork.raycastTarget = false;
        Image veil = Image("SectorVeil", panel, null, Hex("01070C", 120));
        Stretch(veil.rectTransform);
        veil.raycastTarget = false;

        refs.sectorName = Text("SectorName", panel, font, "SECTOR 1 · BORDE EXTERIOR", 27f,
            FontStyles.Bold, Primary);
        Top(refs.sectorName.rectTransform, 34f, 34f, 560f, 44f);
        TMP_Text scannerLabel = Text("ScannerLabel", panel, font, "ESCÁNER", 19f,
            FontStyles.Bold, Secondary);
        Top(scannerLabel.rectTransform, 36f, 91f, 130f, 28f);
        refs.scannerLevel = Text("Level", panel, font, "3/15", 25f, FontStyles.Bold, Cyan);
        Top(refs.scannerLevel.rectTransform, 155f, 84f, 96f, 38f);

        Button changeSector = ButtonPanel("ChangeSector", panel, frame, fill,
            new Vector2(686f, 30f), new Vector2(232f, 62f), FillRaised, CyanMuted);
        TMP_Text changeLabel = Text("Label", changeSector.transform, font, "CAMBIAR SECTOR", 18f,
            FontStyles.Bold, Cyan);
        Stretch(changeLabel.rectTransform, new Vector2(12f, 8f), new Vector2(12f, 8f));
        changeLabel.alignment = TextAlignmentOptions.Center;
        AddPersistent(changeSector.onClick,
            functionalPanel.OnClickOpenGalaxyForExploreSectorSelection);

        refs.scannerUpgrade = ButtonPanel("UpgradeScanner", panel, frame, fill,
            new Vector2(34f, 258f), new Vector2(250f, 58f), FillRaised, CyanMuted);
        refs.scannerUpgradeLabel = Text("Label", refs.scannerUpgrade.transform, font,
            "MEJORAR ESCÁNER", 19f, FontStyles.Bold, Cyan);
        Stretch(refs.scannerUpgradeLabel.rectTransform, new Vector2(20f, 8f), new Vector2(20f, 8f));
        refs.scannerUpgradeLabel.alignment = TextAlignmentOptions.Center;
        AddPersistent(refs.scannerUpgrade.onClick, visual.UpgradeScanner);
    }

    private static void BuildDestination(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font, Refs refs,
        Dimension1ExploreVisualUI visual, Sprite[] destinationArt)
    {
        RectTransform panel = Panel("DestinationPanel", root, frame, fill,
            new Vector2(62f, 640f), new Vector2(954f, 350f), Fill, CyanMuted);
        TMP_Text section = Text("SectionLabel", panel, font, "DESTINOS ESCANEADOS", 23f,
            FontStyles.Bold, Secondary);
        Top(section.rectTransform, 28f, 20f, 270f, 34f);
        refs.destinationName = Text("SelectedDestination", panel, font, "SIN SEÑALES", 20f,
            FontStyles.Bold, Cyan);
        Top(refs.destinationName.rectTransform, 310f, 20f, 360f, 34f);
        refs.destinationLevel = Text("DestinationState", panel, font, "ESCANEA PARA BUSCAR", 16f,
            FontStyles.Bold, Secondary);
        Top(refs.destinationLevel.rectTransform, 310f, 49f, 260f, 26f);
        refs.destinationDistance = Text("DestinationOrdinal", panel, font, "—", 16f,
            FontStyles.Bold, Secondary);
        Top(refs.destinationDistance.rectTransform, 572f, 49f, 120f, 26f);
        refs.destinationCount = Text("Count", panel, font, "0", 22f, FontStyles.Bold, Cyan);
        Top(refs.destinationCount.rectTransform, 700f, 20f, 70f, 34f);
        refs.destinationCount.alignment = TextAlignmentOptions.Center;
        refs.openDetails = ButtonPanel("OpenDetails", panel, frame, fill,
            new Vector2(786f, 17f), new Vector2(140f, 58f), FillRaised, CyanMuted);
        TMP_Text detailsLabel = Text("Label", refs.openDetails.transform, font,
            "DETALLES", 17f, FontStyles.Bold, CyanBright);
        Stretch(detailsLabel.rectTransform, new Vector2(10f, 8f), new Vector2(10f, 8f));
        detailsLabel.alignment = TextAlignmentOptions.Center;
        AddPersistent(refs.openDetails.onClick, visual.OpenPreviewDetails);

        UnityAction[] actions =
        {
            visual.SelectDestinationCard0,
            visual.SelectDestinationCard1,
            visual.SelectDestinationCard2,
            visual.SelectDestinationCard3
        };
        for (int i = 0; i < 4; i++)
        {
            Button card = ButtonPanel("DestinationCard" + i, panel, frame, fill,
                new Vector2(24f + i * 230f, 88f), new Vector2(216f, 238f),
                Hex("031019", 248), i == 0 ? Amber : CyanMuted);
            Image art = Image("Art", card.transform,
                destinationArt != null && i < destinationArt.Length ? destinationArt[i] : null, Color.white);
            Top(art.rectTransform, 8f, 8f, 200f, 154f);
            art.preserveAspect = false;
            art.raycastTarget = false;
            Image shade = Image("Shade", card.transform, null, Hex("01070C", 72));
            Top(shade.rectTransform, 8f, 8f, 200f, 154f);
            shade.raycastTarget = false;
            TMP_Text label = Text("Name", card.transform, font,
                i == 0 ? "CINTURÓN MINERAL" : "SIN SEÑAL", 18f, FontStyles.Bold,
                i == 0 ? Amber : Secondary);
            Top(label.rectTransform, 12f, 168f, 192f, 56f);
            label.alignment = TextAlignmentOptions.Center;
            label.textWrappingMode = TextWrappingModes.Normal;
            refs.destinationCards.Add(card);
            refs.destinationCardImages.Add(art);
            refs.destinationCardBorders.Add(FindChild(card.transform, "Border").GetComponent<Image>());
            refs.destinationCardLabels.Add(label);
            AddPersistent(card.onClick, actions[i]);
        }
    }

    private static void BuildShip(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font, Refs refs,
        Dimension1ExploreVisualUI visual, Sprite initialBlueprint, Material blueprintMaterial)
    {
        RectTransform panel = Panel("ShipPanel", root, frame, fill,
            new Vector2(62f, 1004f), new Vector2(462f, 224f), Fill, CyanMuted);
        Label(panel, font, "NAVE", 24f, 20f, 120f);
        refs.shipName = Value(panel, font, "SONDA LIGERA", 24f, 58f, 250f, Cyan);
        refs.shipStatus = Value(panel, font, "DISPONIBLE", 24f, 96f, 210f, Green);
        refs.shipMetricLabel = Label(panel, font, "VELOCIDAD", 24f, 139f, 140f);
        refs.shipSpeed = Value(panel, font, "NIVEL 4", 24f, 174f, 160f, Secondary);
        refs.shipIllustration = Image("ShipIllustration", panel, initialBlueprint, Color.white);
        refs.shipIllustration.material = blueprintMaterial;
        refs.shipIllustration.rectTransform.anchorMin = refs.shipIllustration.rectTransform.anchorMax = new Vector2(.5f, .5f);
        refs.shipIllustration.rectTransform.pivot = new Vector2(.5f, .5f);
        refs.shipIllustration.rectTransform.anchoredPosition = new Vector2(122f, 0f);
        refs.shipIllustration.rectTransform.sizeDelta = new Vector2(202f, 202f);
        refs.shipIllustration.preserveAspect = true;
        refs.shipIllustration.raycastTarget = false;
        AddCycleControls(panel, frame, fill, font, visual.SelectPreviousShip, visual.SelectNextShip,
            new Vector2(300f, 147f), "CAMBIAR");
    }

    private static void BuildSupport(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font, Refs refs,
        Dimension1ExploreVisualUI visual, Sprite initialBlueprint, Material blueprintMaterial)
    {
        RectTransform panel = Panel("SupportPanel", root, frame, fill,
            new Vector2(550f, 1004f), new Vector2(466f, 224f), Fill, CyanMuted);
        TMP_Text supportLabel = Text("SupportLabel", panel, font, "APOYO", 24f, FontStyles.Bold, Secondary);
        Top(supportLabel.rectTransform, 24f, 20f, 112f, 36f);
        TMP_Text optional = Text("Optional", panel, font, "OPCIONAL", 20f, FontStyles.Bold, Cyan);
        Top(optional.rectTransform, 128f, 23f, 125f, 31f);
        refs.supportIllustration = Image("DroneIllustration", panel, initialBlueprint, Color.white);
        refs.supportIllustration.material = blueprintMaterial;
        refs.supportIllustration.rectTransform.anchorMin = refs.supportIllustration.rectTransform.anchorMax = new Vector2(.5f, .5f);
        refs.supportIllustration.rectTransform.pivot = new Vector2(.5f, .5f);
        refs.supportIllustration.rectTransform.anchoredPosition = new Vector2(125f, 0f);
        refs.supportIllustration.rectTransform.sizeDelta = new Vector2(196f, 196f);
        refs.supportIllustration.preserveAspect = true;
        refs.supportIllustration.raycastTarget = false;
        refs.supportName = Text("SupportName", panel, font, "DRON EXTRACTOR", 27f, FontStyles.Bold, Cyan);
        Top(refs.supportName.rectTransform, 24f, 62f, 250f, 38f);
        refs.supportStatus = Text("SupportStatus", panel, font, "DISPONIBLE", 22f, FontStyles.Bold, Green);
        Top(refs.supportStatus.rectTransform, 24f, 101f, 250f, 32f);
        refs.supportBonus = Text("Bonus", panel, font, "×4 METALES · ×2.5 TIEMPO", 18f, FontStyles.Bold, Secondary);
        Top(refs.supportBonus.rectTransform, 24f, 139f, 250f, 54f);
        refs.supportBonus.alignment = TextAlignmentOptions.Left;
        refs.supportBonus.textWrappingMode = TextWrappingModes.Normal;
        refs.supportAvailability = Text("Availability", panel, font, "● DISPONIBLE", 19f, FontStyles.Bold, Green);
        Top(refs.supportAvailability.rectTransform, 24f, 187f, 260f, 26f);
        AddCycleControls(panel, frame, fill, font, visual.SelectPreviousSupport, visual.SelectNextSupport,
            new Vector2(304f, 147f), "CAMBIAR");
    }

    private static void BuildMode(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font, Refs refs,
        Dimension1ExploreVisualUI visual)
    {
        RectTransform panel = Panel("ModePanel", root, frame, fill,
            new Vector2(62f, 1242f), new Vector2(954f, 146f), Fill, CyanMuted);
        TMP_Text heading = Text("Heading", panel, font, "MODO DE EXPEDICIÓN", 21f,
            FontStyles.Bold, Secondary);
        Top(heading.rectTransform, 25f, 12f, 280f, 30f);

        Button simple = ButtonPanel("SimpleMode", panel, frame, fill,
            new Vector2(24f, 48f), new Vector2(440f, 78f), Hex("1B1508", 248), Amber);
        refs.simpleModeBorder = FindChild(simple.transform, "Border").GetComponent<Image>();
        TMP_Text simpleTitle = Text("Title", simple.transform, font, "SIMPLE", 24f,
            FontStyles.Bold, Primary);
        Top(simpleTitle.rectTransform, 25f, 8f, 180f, 34f);
        refs.simpleModeStatus = Text("Status", simple.transform, font, "MODO ACTIVO", 16f,
            FontStyles.Bold, Amber);
        Top(refs.simpleModeStatus.rectTransform, 25f, 40f, 250f, 25f);
        AddPersistent(simple.onClick, visual.SetSimpleMode);

        Button coordinated = ButtonPanel("CoordinatedMode", panel, frame, fill,
            new Vector2(490f, 48f), new Vector2(440f, 78f), FillRaised, CyanMuted);
        refs.coordinatedModeBorder = FindChild(coordinated.transform, "Border").GetComponent<Image>();
        TMP_Text coordinatedTitle = Text("Title", coordinated.transform, font, "COORDINADA", 24f,
            FontStyles.Bold, Cyan);
        Top(coordinatedTitle.rectTransform, 25f, 8f, 220f, 34f);
        refs.coordinatedModeStatus = Text("Status", coordinated.transform, font,
            "MAYORES RECOMPENSAS", 16f, FontStyles.Bold, Secondary);
        Top(refs.coordinatedModeStatus.rectTransform, 25f, 40f, 300f, 25f);
        AddPersistent(coordinated.onClick, visual.SetCoordinatedMode);
    }

    private static void BuildActiveExpedition(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font, Refs refs,
        Dimension1ExploreVisualUI visual, Sprite activeBadge)
    {
        RectTransform panel = Panel("ActiveExpedition", root, frame, fill,
            new Vector2(62f, 1402f), new Vector2(954f, 146f), Fill, CyanMuted);
        Image badge = Image("ExpeditionBadge", panel, activeBadge, Color.white);
        Top(badge.rectTransform, 28f, 12f, 122f, 122f);
        badge.preserveAspect = true;
        badge.raycastTarget = false;
        TMP_Text eyebrow = Text("Eyebrow", panel, font, "EXPEDICIONES ACTIVAS", 22f, FontStyles.Bold, Cyan);
        Top(eyebrow.rectTransform, 170f, 20f, 325f, 32f);
        refs.activeShip = Text("ActiveShip", panel, font, "SONDA ANALÍTICA", 26f, FontStyles.Bold, Cyan);
        Top(refs.activeShip.rectTransform, 170f, 55f, 335f, 36f);
        refs.activeDestination = Text("ActiveDestination", panel, font, "SEÑAL DESCONOCIDA NIVEL 1", 22f, FontStyles.Normal, Secondary);
        Top(refs.activeDestination.rectTransform, 170f, 91f, 430f, 34f);
        TMP_Text remaining = Text("Remaining", panel, font, "TIEMPO RESTANTE", 18f, FontStyles.Bold, Secondary);
        Top(remaining.rectTransform, 714f, 38f, 220f, 30f);
        remaining.alignment = TextAlignmentOptions.Center;
        refs.activeTimer = Text("Timer", panel, font, "00:28:45", 40f, FontStyles.Normal, Primary);
        Top(refs.activeTimer.rectTransform, 706f, 69f, 236f, 55f);
        refs.activeTimer.alignment = TextAlignmentOptions.Center;
        refs.activeCount = Text("ActiveCount", panel, font, "1 / 1", 18f, FontStyles.Bold, CyanMuted);
        Top(refs.activeCount.rectTransform, 814f, 10f, 90f, 28f);
        refs.activeCount.alignment = TextAlignmentOptions.Center;
        AddCycleControls(panel, frame, fill, font, visual.SelectPreviousActiveExpedition, visual.SelectNextActiveExpedition,
            new Vector2(534f, 46f), "");
    }

    private static void BuildActions(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font, Refs refs,
        Dimension1PanelUI panel, Dimension1ExploreVisualUI visual)
    {
        refs.start = ButtonPanel("StartExpedition", root, frame, fill,
            new Vector2(62f, 1562f), new Vector2(544f, 122f), Hex("1B1508", 250), Amber);
        refs.startLabel = Text("Label", refs.start.transform, font, "INICIAR EXPEDICIÓN", 29f, FontStyles.Bold, Amber);
        Stretch(refs.startLabel.rectTransform, new Vector2(25f, 18f), new Vector2(25f, 18f));
        refs.startLabel.alignment = TextAlignmentOptions.Center;
        AddPersistent(refs.start.onClick, visual.ExecutePrimaryAction);

        refs.record = ButtonPanel("ExplorationRecord", root, frame, fill,
            new Vector2(648f, 1562f), new Vector2(368f, 122f), Fill, CyanMuted);
        TMP_Text recordText = Text("Label", refs.record.transform, font, "REGISTRO", 27f, FontStyles.Bold, CyanMuted);
        Stretch(recordText.rectTransform, new Vector2(25f, 18f), new Vector2(25f, 18f));
        recordText.alignment = TextAlignmentOptions.Center;
        AddPersistent(refs.record.onClick, visual.OpenExpeditionRecord);
    }

    private static void BuildPreviewDetails(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font,
        Refs refs, Dimension1ExploreVisualUI visual)
    {
        RectTransform overlay = Rect("PreviewDetailsOverlay", root);
        Stretch(overlay);
        refs.detailsOverlay = overlay.gameObject;
        Image blocker = Image("Blocker", overlay, null, Hex("00070B", 225));
        Stretch(blocker.rectTransform);
        blocker.raycastTarget = true;

        RectTransform panel = Panel("DetailsPanel", overlay, frame, fill,
            new Vector2(62f, 300f), new Vector2(954f, 1335f), Fill, CyanMuted);
        TMP_Text title = Text("Title", panel, font, "DETALLES DE LA SEÑAL", 32f,
            FontStyles.Bold, Primary);
        Top(title.rectTransform, 36f, 34f, 690f, 52f);

        refs.closeDetails = ButtonPanel("CloseDetails", panel, frame, fill,
            new Vector2(838f, 22f), new Vector2(82f, 72f), FillRaised, CyanMuted);
        TMP_Text closeLabel = Text("Label", refs.closeDetails.transform, font, "×", 42f,
            FontStyles.Normal, CyanBright);
        Stretch(closeLabel.rectTransform, new Vector2(8f, 4f), new Vector2(8f, 4f));
        closeLabel.alignment = TextAlignmentOptions.Center;
        AddPersistent(refs.closeDetails.onClick, visual.ClosePreviewDetails);

        RectTransform viewport = Rect("Viewport", panel);
        Top(viewport, 36f, 116f, 882f, 1175f);
        Image viewportHit = viewport.gameObject.AddComponent<Image>();
        viewportHit.color = Color.clear;
        viewportHit.raycastTarget = true;
        viewport.gameObject.AddComponent<RectMask2D>();
        refs.detailsScroll = viewport.gameObject.AddComponent<ScrollRect>();
        refs.detailsScroll.horizontal = false;
        refs.detailsScroll.vertical = true;
        refs.detailsScroll.movementType = ScrollRect.MovementType.Clamped;
        refs.detailsScroll.inertia = true;
        refs.detailsScroll.decelerationRate = .12f;
        refs.detailsScroll.scrollSensitivity = 48f;
        refs.detailsScroll.viewport = viewport;

        refs.detailsContent = Rect("Content", viewport);
        refs.detailsContent.anchorMin = new Vector2(0f, 1f);
        refs.detailsContent.anchorMax = new Vector2(1f, 1f);
        refs.detailsContent.pivot = new Vector2(.5f, 1f);
        refs.detailsContent.anchoredPosition = Vector2.zero;
        refs.detailsContent.sizeDelta = new Vector2(0f, 1800f);
        refs.detailsScroll.content = refs.detailsContent;

        refs.detailsText = Text("DetailsText", refs.detailsContent, font,
            "SELECCIONA UNA SEÑAL Y UNA NAVE PARA VER DESTINO, DISTANCIA, MODO, " +
            "RECOMPENSAS POSIBLES Y ESTADO DE SALIDA.", 27f, FontStyles.Normal, Primary);
        Top(refs.detailsText.rectTransform, 18f, 12f, 846f, 1760f);
        refs.detailsText.textWrappingMode = TextWrappingModes.Normal;
        refs.detailsText.overflowMode = TextOverflowModes.Overflow;
        refs.detailsText.lineSpacing = 5f;
    }

    private static void BuildNavigation(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font, Refs refs,
        Dimension1PanelUI panel)
    {
        RectTransform nav = Rect("BottomNavigation", root);
        Top(nav, Dimension1SharedLayoutTokens.NavigationX,
            Dimension1SharedLayoutTokens.NavigationY,
            Dimension1SharedLayoutTokens.NavigationWidth,
            Dimension1SharedLayoutTokens.NavigationHeight);
        string[] labels = { "GALAXIA", "EXPLORAR", "HANGAR", "RELIQUIAS", "ÁRBOL" };
        for (int i = 0; i < 5; i++)
        {
            bool active = i == 1;
            Button button = ButtonPanel("Nav_" + labels[i], nav, frame, fill,
                new Vector2(Dimension1SharedLayoutTokens.NavigationCardX(i),
                    Dimension1SharedLayoutTokens.NavigationCardY),
                new Vector2(Dimension1SharedLayoutTokens.NavigationCardWidth,
                    Dimension1SharedLayoutTokens.NavigationCardHeight), active ? Hex("1A1609", 250) : Fill,
                active ? Amber : CyanMuted);
            DrawNavigationIcon(button.transform, i, new Vector2(0f, 28f), 38f, active ? Amber : Cyan);
            TMP_Text label = Text("Label", button.transform, font, labels[i], 21f, FontStyles.Bold, active ? Amber : Cyan);
            Top(label.rectTransform, 7f, 108f, 170f, 34f);
            label.alignment = TextAlignmentOptions.Center;
            if (active)
            {
                button.interactable = false;
                Polygon("SelectedPointer", nav, new[]
                {
                    new Vector2(-424f + i * Dimension1SharedLayoutTokens.NavigationCardStep, 88f),
                    new Vector2(-410f + i * Dimension1SharedLayoutTokens.NavigationCardStep, 108f),
                    new Vector2(-396f + i * Dimension1SharedLayoutTokens.NavigationCardStep, 88f)
                }, Amber);
            }
            else if (i == 0) AddPersistent(button.onClick, panel.OnClickOpenGalaxyPanel);
            else if (i == 2) AddPersistent(button.onClick, panel.OnClickOpenHangarPanel);
            else if (i == 3) AddPersistent(button.onClick, panel.OnClickOpenRelicChamberPanel);
            else if (i == 4) AddPersistent(button.onClick, panel.OnClickOpenDimension1TreePanel);
        }
    }

    private static RectTransform Panel(string name, Transform parent, Sprite frame, Sprite fill,
        Vector2 topLeft, Vector2 size, Color fillColor, Color accent)
    {
        RectTransform root = Rect(name, parent);
        Top(root, topLeft.x, topLeft.y, size.x, size.y);
        Image shadow = Image("Shadow", root, fill, Hex("000000", 130));
        Stretch(shadow.rectTransform, new Vector2(5f, -5f), new Vector2(5f, -5f));
        shadow.type = UnityEngine.UI.Image.Type.Sliced;
        shadow.raycastTarget = false;
        Image baseImage = Image("Fill", root, fill, fillColor);
        Stretch(baseImage.rectTransform);
        baseImage.type = UnityEngine.UI.Image.Type.Sliced;
        baseImage.raycastTarget = false;
        Color borderColor = accent;
        borderColor.a *= .74f;
        Image border = Image("Border", root, frame, borderColor);
        Stretch(border.rectTransform);
        border.type = UnityEngine.UI.Image.Type.Sliced;
        border.raycastTarget = false;
        return root;
    }

    private static Button ButtonPanel(string name, Transform parent, Sprite frame, Sprite fill,
        Vector2 topLeft, Vector2 size, Color fillColor, Color accent)
    {
        RectTransform root = Panel(name, parent, frame, fill, topLeft, size, fillColor, accent);
        Image hit = root.gameObject.AddComponent<Image>();
        hit.color = new Color(1f, 1f, 1f, 0.001f);
        Button button = root.gameObject.AddComponent<Button>();
        button.targetGraphic = hit;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.75f, 0.95f, 1f, 1f);
        colors.pressedColor = new Color(0.55f, 0.85f, 1f, 1f);
        colors.disabledColor = new Color(0.45f, 0.5f, 0.52f, 0.72f);
        colors.fadeDuration = 0.08f;
        button.colors = colors;
        return button;
    }

    private static void AddCycleControls(Transform parent, Sprite frame, Sprite fill, TMP_FontAsset font,
        UnityAction previous, UnityAction next, Vector2 topLeft, string hint)
    {
        const float controlSize = 72f;
        const float controlStep = 86f;
        Button left = ButtonPanel("Previous", parent, frame, fill, topLeft,
            new Vector2(controlSize, controlSize), FillRaised, CyanMuted);
        Line("ChevronA", left.transform, new Vector2(8f, 14f), new Vector2(-8f, 0f), 3.2f, CyanBright);
        Line("ChevronB", left.transform, new Vector2(-8f, 0f), new Vector2(8f, -14f), 3.2f, CyanBright);
        AddPersistent(left.onClick, previous);

        Button right = ButtonPanel("Next", parent, frame, fill, topLeft + new Vector2(controlStep, 0f),
            new Vector2(controlSize, controlSize), FillRaised, CyanMuted);
        Line("ChevronA", right.transform, new Vector2(-8f, 14f), new Vector2(8f, 0f), 3.2f, CyanBright);
        Line("ChevronB", right.transform, new Vector2(8f, 0f), new Vector2(-8f, -14f), 3.2f, CyanBright);
        AddPersistent(right.onClick, next);

        if (string.IsNullOrEmpty(hint)) return;
        TMP_Text label = Text("CycleHint", parent, font, hint, 14f, FontStyles.Bold, CyanMuted);
        Top(label.rectTransform, topLeft.x - 142f, topLeft.y + 20f, 134f, 32f);
        label.alignment = TextAlignmentOptions.Right;
    }

    private static Button TransparentButton(string name, Transform parent, Vector2 topLeft, Vector2 size)
    {
        RectTransform rect = Rect(name, parent);
        Top(rect, topLeft, size);
        Image hit = rect.gameObject.AddComponent<Image>();
        hit.color = new Color(1f, 1f, 1f, .001f);
        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = hit;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(.70f, .94f, 1f, 1f);
        colors.pressedColor = new Color(.45f, .78f, .92f, 1f);
        colors.disabledColor = new Color(.35f, .40f, .42f, .45f);
        colors.fadeDuration = .06f;
        button.colors = colors;
        return button;
    }

    private static TMP_Text Label(Transform parent, TMP_FontAsset font, string value, float x, float y, float width)
    {
        TMP_Text text = Text(value.Replace(" ", "") + "Label", parent, font, value, 23f, FontStyles.Bold, Secondary);
        Top(text.rectTransform, x, y, width, 32f);
        return text;
    }

    private static TMP_Text Value(Transform parent, TMP_FontAsset font, string value, float x, float y, float width, Color color)
    {
        TMP_Text text = Text(value.Replace(" ", "") + "Value", parent, font, value, 26f, FontStyles.Bold, color);
        Top(text.rectTransform, x, y, width, 37f);
        return text;
    }

    private static void DrawCommandCompass(Transform parent, Vector2 center, float size, Color color)
    {
        Circle(parent, center, size, 2f, color);
        Circle(parent, center, size * .52f, 1.2f, Hex("18C8FF", 150));
        Line("CompassAxis", parent, center + new Vector2(0f, size * 1.25f), center + new Vector2(0f, -size * 1.25f), 1.5f, color);
        Line("CompassCross", parent, center + new Vector2(-size * 1.25f, 0f), center + new Vector2(size * 1.25f, 0f), 1.5f, color);
        Polygon("CompassNeedle", parent, new[]
        {
            center + new Vector2(0f, size * .92f), center + new Vector2(size * .22f, 0f),
            center + new Vector2(0f, -size * .48f), center + new Vector2(-size * .22f, 0f)
        }, new Color(color.r, color.g, color.b, .38f));
        Dot(parent, center, 7f, CyanBright);
    }

    private static void DrawCrystal(Transform parent, Vector2 centerTop, float size, int variant, Color color)
    {
        Vector2 c = centerTop;
        if (variant == 4)
        {
            for (int i = 0; i < 6; i++)
            {
                float a = Mathf.PI * 2f * i / 6f;
                Vector2 petal = c + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * size * .56f;
                Circle(parent, petal, size * .20f, 2.6f, color);
            }
            Circle(parent, c, size * .18f, 2.2f, color);
            return;
        }

        if (variant == 1)
        {
            var ingot = new[]
            {
                c + new Vector2(-size * .65f, size * .72f), c + new Vector2(size * .65f, size),
                c + new Vector2(size * .92f, -size * .58f), c + new Vector2(-size * .45f, -size)
            };
            Polygon("IngotFill", parent, ingot, new Color(color.r, color.g, color.b, .32f));
            LineLoop("Ingot", parent, ingot, 2f, color);
            Line("IngotFacet", parent, ingot[0], ingot[2], 1.2f, color);
            return;
        }

        if (variant == 0)
        {
            DrawShard(parent, c + new Vector2(-10f, 7f), size * .72f, -10f, color, "IronA");
            DrawShard(parent, c + new Vector2(10f, 4f), size * .62f, 14f, color, "IronB");
            DrawShard(parent, c + new Vector2(0f, -11f), size * .52f, 0f, color, "IronC");
            return;
        }

        DrawShard(parent, c + new Vector2(-11f, -4f), size * .7f, -12f, color, "NickelA");
        DrawShard(parent, c + new Vector2(8f, 7f), size * .9f, 8f, color, "NickelB");
        DrawShard(parent, c + new Vector2(14f, -10f), size * .55f, 18f, color, "NickelC");
    }

    private static void DrawShard(Transform parent, Vector2 center, float size, float tilt, Color color, string name)
    {
        float dx = tilt * .15f;
        var points = new[]
        {
            center + new Vector2(dx, size), center + new Vector2(size * .62f, size * .06f),
            center + new Vector2(size * .28f, -size), center + new Vector2(-size * .54f, -size * .5f),
            center + new Vector2(-size * .48f, size * .18f)
        };
        Polygon(name + "Fill", parent, points, new Color(color.r, color.g, color.b, .26f));
        LineLoop(name, parent, points, 1.7f, color);
        Line(name + "Facet", parent, points[0], points[2], 1f, color);
    }

    private static void DrawProbe(Transform parent, Vector2 center, float size, Color color)
    {
        float outline = Mathf.Clamp(size * .022f, 1.3f, 2.5f);
        float detail = Mathf.Max(1f, outline * .58f);
        Color inner = Hex("8DEAFF", 155);
        Color faint = Hex("2488AD", 110);
        AddTechnicalGlow(parent, center + new Vector2(0f, -size * .08f),
            new Vector2(size * 1.90f, size * 2.10f), color, .075f);

        // Silueta canónica: caza ancho de doble ala, no un dardo alargado.
        var leftUpperWing = new[]
        {
            center + new Vector2(-size * .13f, size * .48f),
            center + new Vector2(-size * .34f, size * .20f),
            center + new Vector2(-size * .91f, -size * .32f),
            center + new Vector2(-size * .86f, -size * .50f),
            center + new Vector2(-size * .39f, -size * .22f),
            center + new Vector2(-size * .21f, size * .08f)
        };
        var rightUpperWing = Mirror(leftUpperWing, center);
        var leftLowerWing = new[]
        {
            center + new Vector2(-size * .27f, -size * .08f),
            center + new Vector2(-size * .86f, -size * .50f),
            center + new Vector2(-size * .86f, -size * .76f),
            center + new Vector2(-size * .40f, -size * .53f),
            center + new Vector2(-size * .24f, -size * .34f)
        };
        var rightLowerWing = Mirror(leftLowerWing, center);
        Polygon("ProbeUpperWingFillL", parent, leftUpperWing, new Color(color.r, color.g, color.b, .10f));
        Polygon("ProbeUpperWingFillR", parent, rightUpperWing, new Color(color.r, color.g, color.b, .10f));
        Polygon("ProbeLowerWingFillL", parent, leftLowerWing, new Color(color.r, color.g, color.b, .075f));
        Polygon("ProbeLowerWingFillR", parent, rightLowerWing, new Color(color.r, color.g, color.b, .075f));
        LineLoop("ProbeUpperWingL", parent, leftUpperWing, outline, color);
        LineLoop("ProbeUpperWingR", parent, rightUpperWing, outline, color);
        LineLoop("ProbeLowerWingL", parent, leftLowerWing, detail, inner);
        LineLoop("ProbeLowerWingR", parent, rightLowerWing, detail, inner);

        var body = new[]
        {
            center + new Vector2(0f, size),
            center + new Vector2(size * .15f, size * .56f),
            center + new Vector2(size * .21f, size * .13f),
            center + new Vector2(size * .20f, -size * .30f),
            center + new Vector2(size * .30f, -size * .75f),
            center + new Vector2(size * .27f, -size),
            center + new Vector2(0f, -size * .90f),
            center + new Vector2(-size * .27f, -size),
            center + new Vector2(-size * .30f, -size * .75f),
            center + new Vector2(-size * .20f, -size * .30f),
            center + new Vector2(-size * .21f, size * .13f),
            center + new Vector2(-size * .15f, size * .56f)
        };
        Polygon("ProbeBodyFill", parent, body, new Color(color.r, color.g, color.b, .16f));
        LineLoop("ProbeBodyOutline", parent, body, outline, color);
        LineLoop("ProbeBodyInset", parent, new[]
        {
            center + new Vector2(0f, size * .88f),
            center + new Vector2(size * .10f, size * .51f),
            center + new Vector2(size * .13f, -size * .22f),
            center + new Vector2(size * .22f, -size * .72f),
            center + new Vector2(0f, -size * .80f),
            center + new Vector2(-size * .22f, -size * .72f),
            center + new Vector2(-size * .13f, -size * .22f),
            center + new Vector2(-size * .10f, size * .51f)
        }, detail, inner);

        // Cabina escalonada y raíles verticales.
        var cockpit = new[]
        {
            center + new Vector2(0f, size * .79f),
            center + new Vector2(size * .10f, size * .48f),
            center + new Vector2(size * .09f, size * .10f),
            center + new Vector2(0f, -size * .06f),
            center + new Vector2(-size * .09f, size * .10f),
            center + new Vector2(-size * .10f, size * .48f)
        };
        Polygon("ProbeCockpitFill", parent, cockpit, new Color(CyanBright.r, CyanBright.g, CyanBright.b, .18f));
        LineLoop("ProbeCockpit", parent, cockpit, detail, CyanBright);
        Line("ProbeCockpitRailL", parent, center + new Vector2(-size * .045f, size * .59f), center + new Vector2(-size * .045f, size * .12f), detail, inner);
        Line("ProbeCockpitRailR", parent, center + new Vector2(size * .045f, size * .59f), center + new Vector2(size * .045f, size * .12f), detail, inner);

        // Carcasas laterales largas y paneles de las cuatro alas.
        var casingL = new[]
        {
            center + new Vector2(-size * .20f, size * .20f),
            center + new Vector2(-size * .33f, size * .02f),
            center + new Vector2(-size * .36f, -size * .61f),
            center + new Vector2(-size * .26f, -size * .74f),
            center + new Vector2(-size * .18f, -size * .29f)
        };
        var casingR = Mirror(casingL, center);
        Polygon("ProbeCasingFillL", parent, casingL, new Color(color.r, color.g, color.b, .13f));
        Polygon("ProbeCasingFillR", parent, casingR, new Color(color.r, color.g, color.b, .13f));
        LineLoop("ProbeCasingL", parent, casingL, detail, color);
        LineLoop("ProbeCasingR", parent, casingR, detail, color);
        Line("ProbeWingRibL1", parent, leftUpperWing[1], leftUpperWing[3], detail, inner);
        Line("ProbeWingRibL2", parent, leftUpperWing[2], leftUpperWing[4], detail, faint);
        Line("ProbeWingRibR1", parent, rightUpperWing[1], rightUpperWing[3], detail, inner);
        Line("ProbeWingRibR2", parent, rightUpperWing[2], rightUpperWing[4], detail, faint);
        Line("ProbeLowerRibL", parent, leftLowerWing[0], leftLowerWing[2], detail, faint);
        Line("ProbeLowerRibR", parent, rightLowerWing[0], rightLowerWing[2], detail, faint);

        // Reactor inferior grande dentro de una cápsula poligonal.
        Vector2 reactor = center + new Vector2(0f, -size * .46f);
        LineLoop("ProbeReactorHousing", parent, new[]
        {
            reactor + new Vector2(0f, size * .29f), reactor + new Vector2(size * .24f, size * .13f),
            reactor + new Vector2(size * .22f, -size * .19f), reactor + new Vector2(0f, -size * .29f),
            reactor + new Vector2(-size * .22f, -size * .19f), reactor + new Vector2(-size * .24f, size * .13f)
        }, detail, inner);
        Circle(parent, reactor, size * .25f, detail, color);
        Circle(parent, reactor, size * .16f, detail, CyanBright);
        Circle(parent, reactor, size * .075f, 1f, Hex("8DEAFF", 220));
        Dot(parent, reactor, size * .055f, CyanBright);
        Line("ProbeAxis", parent, center + new Vector2(0f, size * .93f), center + new Vector2(0f, -size * .91f), detail, Hex("8DEAFF", 170));
    }

    private static void DrawDrone(Transform parent, Vector2 center, float size, Color color)
    {
        float outline = Mathf.Clamp(size * .022f, 1.25f, 2.2f);
        float detail = Mathf.Max(.9f, outline * .58f);
        Color inner = Hex("8DEAFF", 150);
        AddTechnicalGlow(parent, center, new Vector2(size * 2.02f, size * 1.40f), color, .06f);

        // Cuatro palas separadas: perfil de insecto mecánico de la referencia.
        var upperLeft = new[]
        {
            center + new Vector2(-size * .18f, size * .21f),
            center + new Vector2(-size * .42f, size * .05f),
            center + new Vector2(-size * .95f, size * .35f),
            center + new Vector2(-size * .55f, size * .42f),
            center + new Vector2(-size * .25f, size * .33f)
        };
        var lowerLeft = new[]
        {
            center + new Vector2(-size * .22f, -size * .05f),
            center + new Vector2(-size * .41f, -size * .20f),
            center + new Vector2(-size, -size * .18f),
            center + new Vector2(-size * .60f, size * .01f),
            center + new Vector2(-size * .31f, size * .08f)
        };
        var upperRight = Mirror(upperLeft, center);
        var lowerRight = Mirror(lowerLeft, center);
        foreach (var blade in new[] { upperLeft, upperRight, lowerLeft, lowerRight })
        {
            Polygon("DroneBladeFill", parent, blade, new Color(color.r, color.g, color.b, .075f));
            LineLoop("DroneBlade", parent, blade, outline, color);
        }
        Line("DroneBladeRibUL1", parent, upperLeft[0], upperLeft[2], detail, inner);
        Line("DroneBladeRibUL2", parent, upperLeft[1], upperLeft[3], detail, Hex("2488AD", 115));
        Line("DroneBladeRibUR1", parent, upperRight[0], upperRight[2], detail, inner);
        Line("DroneBladeRibUR2", parent, upperRight[1], upperRight[3], detail, Hex("2488AD", 115));
        Line("DroneBladeRibLL", parent, lowerLeft[0], lowerLeft[2], detail, inner);
        Line("DroneBladeRibLR", parent, lowerRight[0], lowerRight[2], detail, inner);

        var topFin = new[]
        {
            center + new Vector2(0f, size * .70f),
            center + new Vector2(size * .22f, size * .24f),
            center + new Vector2(-size * .22f, size * .24f)
        };
        Polygon("DroneTopFinFill", parent, topFin, new Color(color.r, color.g, color.b, .13f));
        LineLoop("DroneTopFin", parent, topFin, detail, color);
        Line("DroneTopFinAxis", parent, topFin[0], center + new Vector2(0f, size * .23f), detail, inner);
        Line("DroneCollarA", parent, center + new Vector2(-size * .16f, size * .29f), center + new Vector2(size * .16f, size * .29f), detail, color);

        var chassis = new[]
        {
            center + new Vector2(0f, size * .33f),
            center + new Vector2(size * .27f, size * .14f),
            center + new Vector2(size * .28f, -size * .22f),
            center + new Vector2(0f, -size * .40f),
            center + new Vector2(-size * .28f, -size * .22f),
            center + new Vector2(-size * .27f, size * .14f)
        };
        Polygon("DroneChassisFill", parent, chassis, new Color(color.r, color.g, color.b, .17f));
        LineLoop("DroneChassis", parent, chassis, outline, color);
        Circle(parent, center, size * .30f, outline, color);
        Circle(parent, center, size * .21f, detail, inner);
        Circle(parent, center, size * .11f, detail, CyanBright);
        Dot(parent, center, size * .055f, CyanBright);

        Circle(parent, center + new Vector2(-size * .36f, size * .02f), size * .06f, detail, color);
        Circle(parent, center + new Vector2(size * .36f, size * .02f), size * .06f, detail, color);
        var tail = new[]
        {
            center + new Vector2(-size * .22f, -size * .23f),
            center + new Vector2(-size * .15f, -size * .39f),
            center + new Vector2(-size * .13f, -size * .55f),
            center + new Vector2(0f, -size * .70f),
            center + new Vector2(size * .13f, -size * .55f),
            center + new Vector2(size * .15f, -size * .39f),
            center + new Vector2(size * .22f, -size * .23f)
        };
        Polygon("DroneTailFill", parent, tail, new Color(color.r, color.g, color.b, .12f));
        LineLoop("DroneTail", parent, tail, detail, color);
        Line("DroneTailAxis", parent, center + new Vector2(0f, -size * .27f), center + new Vector2(0f, -size * .67f), detail, inner);
        Line("DroneTailBand", parent, center + new Vector2(-size * .14f, -size * .47f), center + new Vector2(size * .14f, -size * .47f), detail, inner);
    }

    internal static void DrawNavigationIcon(Transform parent, int variant, Vector2 center, float size, Color color)
    {
        if (variant == 0)
        {
            // Galaxia espiral: elipse central y brazos curvos segmentados.
            Circle(parent, center, size * .17f, 1.5f, color);
            for (int arm = 0; arm < 3; arm++)
            {
                var points = new List<Vector2>();
                float baseAngle = arm * Mathf.PI * 2f / 3f;
                for (int i = 0; i < 13; i++)
                {
                    float t = i / 12f;
                    float a = baseAngle + t * 2.35f;
                    float r = Mathf.Lerp(size * .14f, size, t);
                    points.Add(center + new Vector2(Mathf.Cos(a), Mathf.Sin(a) * .48f) * r);
                }
                LineGraphic("GalaxyArm" + arm, parent, points, 2.1f, false, color);
            }
            Dot(parent, center + new Vector2(size * .78f, size * .48f), 4f, color);
            Dot(parent, center + new Vector2(-size * .74f, size * .58f), 3f, color);
            return;
        }

        if (variant == 1)
        {
            Circle(parent, center + new Vector2(0f, 4f), size * .72f, 1.7f, color);
            Circle(parent, center + new Vector2(0f, 4f), size * .30f, 1.2f, color);
            Line("NavRadarAxis", parent, center + new Vector2(0f, size * .95f), center + new Vector2(0f, -size * .96f), 1.3f, color);
            Line("NavRadarCross", parent, center + new Vector2(-size * .92f, 4f), center + new Vector2(size * .92f, 4f), 1.1f, color);
            Polygon("NavRadarSweep", parent, new[]
            {
                center + new Vector2(0f, 4f), center + new Vector2(size * .22f, size * .69f),
                center + new Vector2(size * .60f, size * .32f)
            }, new Color(color.r, color.g, color.b, .22f));
            Dot(parent, center + new Vector2(size * .47f, size * .39f), 5f, color);
            return;
        }

        if (variant == 2)
        {
            DrawProbe(parent, center + new Vector2(0f, -2f), size * .82f, color);
            return;
        }

        if (variant == 3)
        {
            var crystal = new[]
            {
                center + new Vector2(0f, size), center + new Vector2(size * .34f, size * .22f),
                center + new Vector2(size * .20f, -size * .58f), center + new Vector2(0f, -size * .88f),
                center + new Vector2(-size * .20f, -size * .58f), center + new Vector2(-size * .34f, size * .22f)
            };
            Polygon("NavRelicFill", parent, crystal, new Color(color.r, color.g, color.b, .12f));
            LineLoop("NavRelic", parent, crystal, 2f, color);
            Line("NavRelicFacetL", parent, crystal[0], crystal[4], 1f, color);
            Line("NavRelicFacetR", parent, crystal[0], crystal[2], 1f, color);
            DrawEllipse(parent, center + new Vector2(0f, -size * .92f), size * .72f, size * .20f, 1.5f, color);
            DrawEllipse(parent, center + new Vector2(0f, -size * .92f), size * .98f, size * .32f, 1f, Hex("18C8FF", 135));
            return;
        }

        // Árbol tecnológico ramificado.
        Line("TreeTrunk", parent, center + new Vector2(0f, -size), center + new Vector2(0f, size * .72f), 2.2f, color);
        Vector2[] nodes =
        {
            center + new Vector2(-size * .74f, size * .62f), center + new Vector2(-size * .50f, size * .12f),
            center + new Vector2(-size * .82f, -size * .22f), center + new Vector2(size * .74f, size * .62f),
            center + new Vector2(size * .50f, size * .12f), center + new Vector2(size * .82f, -size * .22f),
            center + new Vector2(0f, size * .96f)
        };
        foreach (Vector2 node in nodes)
        {
            Vector2 joint = center + new Vector2(node.x > center.x ? size * .12f : node.x < center.x ? -size * .12f : 0f,
                Mathf.Lerp(-size * .20f, size * .55f, Mathf.InverseLerp(-size * .3f, size, node.y - center.y)));
            Line("TreeBranch", parent, joint, node, 1.5f, color);
            Circle(parent, node, 3.5f, 1.2f, color);
        }
    }

    private static void DrawEllipse(Transform parent, Vector2 center, float radiusX, float radiusY, float width, Color color)
    {
        var points = new List<Vector2>();
        for (int i = 0; i < 64; i++)
        {
            float a = Mathf.PI * 2f * i / 64f;
            points.Add(center + new Vector2(Mathf.Cos(a) * radiusX, Mathf.Sin(a) * radiusY));
        }
        LineGraphic("Ellipse", parent, points, width, true, color);
    }

    private static void DrawAnalyticProbe(Transform parent, Vector2 center, float size, Color color)
    {
        float outline = Mathf.Clamp(size * .021f, 1.2f, 2.1f);
        float detail = Mathf.Max(.85f, outline * .57f);
        Color inner = Hex("8DEAFF", 150);
        Vector2 globe = center + new Vector2(0f, size * .13f);
        AddTechnicalGlow(parent, globe, new Vector2(size * .76f, size * .82f), color, .055f);

        // Mástil superior estrecho con punta y tres collares.
        Line("AnalyticMast", parent, center + new Vector2(0f, size), globe + new Vector2(0f, size * .34f), outline, color);
        Line("AnalyticMastInset", parent, center + new Vector2(0f, size * .95f), globe + new Vector2(0f, size * .37f), detail, inner);
        Dot(parent, center + new Vector2(0f, size * .96f), size * .045f, CyanBright);
        Line("AnalyticCollarA", parent, center + new Vector2(-size * .10f, size * .86f), center + new Vector2(size * .10f, size * .86f), detail, color);
        Line("AnalyticCollarB", parent, center + new Vector2(-size * .14f, size * .79f), center + new Vector2(size * .14f, size * .79f), detail, color);
        Line("AnalyticCollarC", parent, center + new Vector2(-size * .17f, size * .70f), center + new Vector2(size * .17f, size * .70f), detail, inner);

        // Globo-jaula: doble contorno, meridianos y paralelos, sin alas laterales.
        DrawEllipse(parent, globe, size * .38f, size * .40f, outline, color);
        DrawEllipse(parent, globe, size * .33f, size * .35f, detail, inner);
        DrawEllipse(parent, globe, size * .17f, size * .36f, detail, Hex("2488AD", 140));
        DrawEllipse(parent, globe, size * .07f, size * .35f, detail, Hex("2488AD", 110));
        DrawEllipse(parent, globe + new Vector2(0f, size * .13f), size * .32f, size * .10f, detail, inner);
        DrawEllipse(parent, globe, size * .36f, size * .12f, detail, Hex("2488AD", 145));
        DrawEllipse(parent, globe - new Vector2(0f, size * .14f), size * .31f, size * .095f, detail, inner);
        Dot(parent, globe, size * .055f, CyanBright);
        Line("AnalyticGlobeAxis", parent, globe + new Vector2(0f, size * .34f), globe - new Vector2(0f, size * .35f), detail, inner);

        // Collar inferior en capas y aguja de lectura larga y segmentada.
        var collar = new[]
        {
            center + new Vector2(-size * .25f, -size * .23f),
            center + new Vector2(-size * .14f, -size * .36f),
            center + new Vector2(0f, -size * .43f),
            center + new Vector2(size * .14f, -size * .36f),
            center + new Vector2(size * .25f, -size * .23f),
            center + new Vector2(size * .17f, -size * .16f),
            center + new Vector2(-size * .17f, -size * .16f)
        };
        Polygon("AnalyticCollarFill", parent, collar, new Color(color.r, color.g, color.b, .12f));
        LineLoop("AnalyticCollar", parent, collar, detail, color);
        Line("AnalyticCollarBand", parent, center + new Vector2(-size * .17f, -size * .29f), center + new Vector2(size * .17f, -size * .29f), detail, inner);

        var needle = new[]
        {
            center + new Vector2(-size * .13f, -size * .38f),
            center + new Vector2(-size * .10f, -size * .66f),
            center + new Vector2(0f, -size),
            center + new Vector2(size * .10f, -size * .66f),
            center + new Vector2(size * .13f, -size * .38f)
        };
        Polygon("AnalyticNeedleFill", parent, needle, new Color(color.r, color.g, color.b, .10f));
        LineLoop("AnalyticNeedle", parent, needle, outline, color);
        Line("AnalyticNeedleAxis", parent, center + new Vector2(0f, -size * .39f), center + new Vector2(0f, -size * .96f), detail, CyanBright);
        Line("AnalyticNeedleBandA", parent, center + new Vector2(-size * .11f, -size * .52f), center + new Vector2(size * .11f, -size * .52f), detail, inner);
        Line("AnalyticNeedleBandB", parent, center + new Vector2(-size * .095f, -size * .66f), center + new Vector2(size * .095f, -size * .66f), detail, inner);
    }

    private static Vector2[] Mirror(Vector2[] source, Vector2 center)
    {
        var mirrored = new Vector2[source.Length];
        for (int i = 0; i < source.Length; i++)
            mirrored[i] = new Vector2(center.x * 2f - source[i].x, source[i].y);
        return mirrored;
    }

    private static void AddTechnicalGlow(Transform parent, Vector2 center, Vector2 size, Color color, float alpha)
    {
        Image glow = Image("TechnicalGlow", parent, LoadSprite(ArtPath + "/d1_glow_v3.png"),
            new Color(color.r, color.g, color.b, alpha));
        glow.rectTransform.anchorMin = glow.rectTransform.anchorMax = new Vector2(.5f, .5f);
        glow.rectTransform.pivot = new Vector2(.5f, .5f);
        glow.rectTransform.anchoredPosition = center;
        glow.rectTransform.sizeDelta = size;
        glow.raycastTarget = false;
    }

    private static void Circle(Transform parent, Vector2 center, float radius, float width, Color color)
    {
        var points = new List<Vector2>();
        const int segments = 72;
        for (int i = 0; i < segments; i++)
        {
            float a = Mathf.PI * 2f * i / segments;
            points.Add(center + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius);
        }
        LineGraphic("Circle", parent, points, width, true, color);
    }

    private static void Dot(Transform parent, Vector2 center, float diameter, Color color)
    {
        var points = new List<Vector2>();
        const int segments = 32;
        for (int i = 0; i < segments; i++)
        {
            float a = Mathf.PI * 2f * i / segments;
            points.Add(center + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * diameter * .5f);
        }
        Polygon("Dot", parent, points, color);
    }

    private static void GlowDot(Transform parent, Vector2 center, float diameter, Color color)
    {
        Image glow = Image("BlipGlow", parent, LoadSprite(ArtPath + "/d1_glow_v3.png"),
            new Color(color.r, color.g, color.b, .38f));
        glow.rectTransform.anchorMin = glow.rectTransform.anchorMax = new Vector2(.5f, .5f);
        glow.rectTransform.pivot = new Vector2(.5f, .5f);
        glow.rectTransform.anchoredPosition = center;
        glow.rectTransform.sizeDelta = new Vector2(diameter * 3.4f, diameter * 3.4f);
        glow.raycastTarget = false;
        Dot(parent, center, diameter, color);
        if (diameter > 14f) Circle(parent, center, diameter * .82f, 1f, Hex("8DEAFF", 135));
    }

    private static void Line(string name, Transform parent, Vector2 a, Vector2 b, float width, Color color)
    {
        LineGraphic(name, parent, new[] { a, b }, width, false, color);
    }

    private static void LineLoop(string name, Transform parent, IList<Vector2> points, float width, Color color)
    {
        LineGraphic(name, parent, points, width, true, color);
    }

    private static void LineGraphic(string name, Transform parent, IList<Vector2> points, float width, bool closed, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        RectTransform rect = (RectTransform)go.transform;
        Stretch(rect);
        Dimension1CommandCenterLineGraphic line = go.AddComponent<Dimension1CommandCenterLineGraphic>();
        line.color = color;
        line.raycastTarget = false;
        line.SetLine(points, width, closed);
    }

    private static void Polygon(string name, Transform parent, IList<Vector2> points, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        RectTransform rect = (RectTransform)go.transform;
        Stretch(rect);
        Dimension1CommandCenterPolygonGraphic polygon = go.AddComponent<Dimension1CommandCenterPolygonGraphic>();
        polygon.color = color;
        polygon.raycastTarget = false;
        polygon.SetPolygon(points);
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

    private static void Top(RectTransform rect, Vector2 topLeft, Vector2 size) => Top(rect, topLeft.x, topLeft.y, size.x, size.y);

    private static void Stretch(RectTransform rect, Vector2 min = default, Vector2 max = default)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(.5f, .5f);
        rect.offsetMin = min;
        rect.offsetMax = new Vector2(-max.x, -max.y);
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

    private static void PrepareUiSprite(string path)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
            throw new InvalidOperationException("No se pudo importar el recurso gráfico: " + path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.maxTextureSize = 2048;
        importer.SaveAndReimport();
    }

    private static Sprite LoadSprite(string path) => AssetDatabase.LoadAssetAtPath<Sprite>(path);

    private static T FindSceneComponent<T>(Scene scene) where T : Component
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            T result = root.GetComponentInChildren<T>(true);
            if (result != null) return result;
        }
        return null;
    }

    private static Transform FindDirectChild(Transform parent, string name)
    {
        for (int i = 0; i < parent.childCount; i++)
            if (parent.GetChild(i).name == name) return parent.GetChild(i);
        return null;
    }

    private static Transform FindChild(Transform parent, string name)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children) if (child.name == name) return child;
        return null;
    }

    private static GameObject[] FindObjects(Transform parent, params string[] names)
    {
        var result = new List<GameObject>();
        foreach (string name in names)
        {
            Transform target = FindChild(parent, name);
            if (target != null) result.Add(target.gameObject);
        }
        return result.ToArray();
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
                target = FindChild(sceneRoot.transform, name);
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

    private static void SetStringArray(SerializedObject so, string propertyName, string[] values)
    {
        SerializedProperty property = so.FindProperty(propertyName);
        if (property == null) throw new InvalidOperationException("No existe arreglo serializado: " + propertyName);
        property.arraySize = values == null ? 0 : values.Length;
        for (int i = 0; values != null && i < values.Length; i++)
            property.GetArrayElementAtIndex(i).stringValue = values[i];
    }

    private static void AddPersistent(UnityEvent evt, UnityAction action)
    {
        UnityEventTools.AddPersistentListener(evt, action);
    }

    private static bool HasPersistentMethod(UnityEvent evt, string methodName)
    {
        if (evt == null) return false;
        for (int i = 0; i < evt.GetPersistentEventCount(); i++)
            if (evt.GetPersistentMethodName(i) == methodName) return true;
        return false;
    }

    private static void ValidateInternal(Scene scene)
    {
        Transform root = null;
        foreach (GameObject sceneRoot in scene.GetRootGameObjects())
        {
            root = FindChild(sceneRoot.transform, RootName);
            if (root != null) break;
        }
        if (root == null) throw new InvalidOperationException("Falta " + RootName + ".");
        if (root.GetComponent<Dimension1ExploreVisualUI>() == null) throw new InvalidOperationException("Falta controlador visual.");
        if (root.GetComponent<Canvas>() == null || root.GetComponent<GraphicRaycaster>() == null) throw new InvalidOperationException("Falta overlay interactivo.");
        string[] required = { "ScannerPanel", "DestinationPanel", "ShipPanel", "SupportPanel", "ActiveExpedition", "StartExpedition", "BottomNavigation" };
        foreach (string name in required)
            if (FindChild(root, name) == null) throw new InvalidOperationException("Falta bloque visual: " + name);
        Image sectorArtwork = FindChild(root, "SectorArtwork")?.GetComponent<Image>();
        if (sectorArtwork == null || sectorArtwork.sprite == null)
            throw new InvalidOperationException("Falta el arte dinámico del sector en Explorar.");
        if (FindChild(root, "Nav_EXPLORAR")?.GetComponent<Button>() == null) throw new InvalidOperationException("Falta navegación Explorar.");
        Button start = FindChild(root, "StartExpedition")?.GetComponent<Button>();
        if (start == null || !HasPersistentMethod(start.onClick, "ExecutePrimaryAction"))
            throw new InvalidOperationException("La acción principal de Explorar no usa el flujo contextual.");
        Button upgradeScanner = FindChild(root, "UpgradeScanner")?.GetComponent<Button>();
        if (upgradeScanner == null || !HasPersistentMethod(upgradeScanner.onClick, "UpgradeScanner"))
            throw new InvalidOperationException("Falta la mejora funcional del escáner.");
    }

    private static Color Hex(string value, byte alpha = 255)
    {
        ColorUtility.TryParseHtmlString("#" + value, out Color color);
        color.a = alpha / 255f;
        return color;
    }
}
#endif

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

public static class Dimension1TreeReferenceSetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ArtPath = "Assets/Project/UI/Dimension1/Generated";
    private const string RelicPath = ArtPath + "/Candidates/Relics";
    private const string TreePath = ArtPath + "/Candidates/Tree";
    private const string RootName = "D1_TreeVisualRoot";
    private const string LegacyName = "D1_TreeLegacyFunctionalRoot";
    private const float W = 1080f;
    private const float H = 1920f;

    private static readonly Color Void = Hex("01090E");
    private static readonly Color Fill = Hex("04121B", 249);
    private static readonly Color FillRaised = Hex("071924", 252);
    private static readonly Color Cyan = Hex("18C8FF");
    private static readonly Color CyanBright = Hex("8DEAFF");
    private static readonly Color CyanMuted = Hex("087FA9");
    private static readonly Color Primary = Hex("EDF4F7");
    private static readonly Color Secondary = Hex("9EABB4");
    private static readonly Color Amber = Hex("F4A70B");
    private static readonly Color AmberFill = Hex("1A1508", 252);
    private static readonly Color Locked = Hex("687680");

    private sealed class NodeSpec
    {
        public readonly int number;
        public readonly string label;
        public readonly string progress;
        public readonly Vector2 center;
        public readonly Vector2 size;
        public readonly bool selected;

        public NodeSpec(int number, string label, string progress, float x, float y,
            float width = 174f, float height = 194f, bool selected = false)
        {
            this.number = number;
            this.label = label;
            this.progress = progress;
            center = new Vector2(x, y);
            size = new Vector2(width, height);
            this.selected = selected;
        }
    }

    [MenuItem("Quantum Forge/Dimension 1/Install Tree Orbital Reference")]
    public static void Install()
    {
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        ConfigureSingleSprite(TreePath + "/d1_tree_advanced_cartography_v2.png");
        ConfigureSingleSprite(TreePath + "/d1_tree_orbital_field_v1.png");
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Dimension1PanelUI panel = FindSceneComponent<Dimension1PanelUI>(scene);
        if (panel == null) throw new InvalidOperationException("No existe Dimension1PanelUI en Main.unity.");
        Transform treePanel = FindChild(panel.transform, "Dimension1TreePanel");
        if (treePanel == null) throw new InvalidOperationException("No existe Dimension1TreePanel en Main.unity.");

        TMP_FontAsset font = FindFont(panel.transform);
        Sprite frame = LoadSprite(ArtPath + "/d1_premium_frame_v4.png");
        Sprite fillSprite = LoadSprite(ArtPath + "/d1_panel_fill_v4.png");
        Sprite stars = LoadSprite(ArtPath + "/d1_starfield.png");
        Sprite glow = LoadSprite(ArtPath + "/d1_glow_v3.png");
        Sprite lockSprite = LoadSprite(ArtPath + "/d1_lock_v3.png");
        if (font == null || frame == null || fillSprite == null || stars == null || glow == null ||
            lockSprite == null)
            throw new InvalidOperationException("Faltan recursos canónicos de Dimensión 1 para Árbol Cuántico.");

        PreserveLegacyControls(treePanel);
        Transform previous = FindDirectChild(treePanel, RootName);
        if (previous != null) UnityEngine.Object.DestroyImmediate(previous.gameObject);

        RectTransform root = Rect(RootName, treePanel);
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
        CanvasGroup group = root.gameObject.AddComponent<CanvasGroup>();
        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;
        Dimension1TreeNavigationUI navigation = root.gameObject.AddComponent<Dimension1TreeNavigationUI>();
        Dimension1TreeVisualUI visual = root.gameObject.AddComponent<Dimension1TreeVisualUI>();

        Image background = Image("Background", root, null, Void);
        Stretch(background.rectTransform, new Vector2(-28f, -34f), new Vector2(-28f, -34f));
        background.raycastTarget = false;
        Image starfield = Image("Starfield", root, stars, Hex("6DC9E6", 10));
        Stretch(starfield.rectTransform);
        starfield.raycastTarget = false;
        Image outer = Image("OuterFrame", root, frame, Hex("087FA9", 205));
        Top(outer.rectTransform, Dimension1SharedLayoutTokens.OuterFrameX,
            Dimension1SharedLayoutTokens.OuterFrameY,
            Dimension1SharedLayoutTokens.OuterFrameWidth,
            Dimension1SharedLayoutTokens.OuterFrameHeight);
        outer.type = UnityEngine.UI.Image.Type.Sliced;
        outer.raycastTarget = false;

        BuildHeader(root, frame, fillSprite, font, navigation,
            FindSceneComponent<Dimension1MetalsInventoryUI>(scene));
        BuildTreeShell(root, frame, fillSprite, glow, font, lockSprite, visual);
        BuildDetail(root, frame, fillSprite, glow, font, visual);
        BuildNavigation(root, frame, fillSprite, font, navigation);
        Dimension1SharedShellApply.ApplyToRoot(root);
        ConfigureVisual(root, visual);

        SerializedObject navigationObject = new SerializedObject(navigation);
        Assign(navigationObject, "panel", panel);
        Assign(navigationObject, "commandCenter", FindSceneComponent<Dimension1CommandCenterUI>(scene));
        Assign(navigationObject, "canvasGroup", group);
        SetObjectArray(navigationObject, "hideWhileOpen", FindNavigationRoots(scene));
        navigationObject.ApplyModifiedPropertiesWithoutUndo();

        root.gameObject.SetActive(true);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("Unity no pudo guardar Main.unity.");
        ValidateInternal(scene);
        Debug.Log("[D1 Tree] INSTALL_PASS | referencia orbital v1 | 10 nodos funcionales | selección, compra, estados y guardado");
    }

    [MenuItem("Quantum Forge/Dimension 1/Validate Tree Orbital Reference")]
    public static void Validate()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        ValidateInternal(scene);
        Debug.Log("[D1 Tree] VALIDATION_PASS | composición orbital funcional");
    }

    private static void PreserveLegacyControls(Transform treePanel)
    {
        Transform legacy = FindDirectChild(treePanel, LegacyName);
        if (legacy == null) legacy = Rect(LegacyName, treePanel);
        string[] names =
        {
            "TreeTitleText", "TreeNodeDropdown", "TreeInfoText", "BuyTreeNodeButton", "CloseTreePanelButton"
        };
        foreach (string name in names)
        {
            Transform child = FindDirectChild(treePanel, name);
            if (child != null) child.SetParent(legacy, false);
        }
        legacy.gameObject.SetActive(false);
        legacy.SetAsFirstSibling();
    }

    private static void BuildHeader(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font,
        Dimension1TreeNavigationUI navigation, Dimension1MetalsInventoryUI metalsInventory)
    {
        TMP_Text title = Text("DimensionTitle", root, font, "DIMENSIÓN 1", 34f, FontStyles.Bold, Primary);
        Top(title.rectTransform, 305f, 12f, 470f, 52f);
        title.alignment = TextAlignmentOptions.Center;
        title.characterSpacing = 3f;
        RectTransform decor = Rect("HeaderDecor", root);
        Top(decor, 0f, 6f, W, 58f);
        Line("HeaderLineL", decor, new Vector2(-420f, 1f), new Vector2(-245f, 1f), 1.3f, CyanMuted);
        Line("HeaderLineR", decor, new Vector2(245f, 1f), new Vector2(420f, 1f), 1.3f, CyanMuted);

        RectTransform command = Panel("CommandCenter", root, frame, fill,
            new Vector2(18f, 12f), new Vector2(166f, 49f), Fill, CyanMuted);
        DrawHomeIcon(command, new Vector2(-59f, 0f), 20f, Cyan);
        TMP_Text home = Text("Label", command, font, "CENTRO", 15f, FontStyles.Bold, Secondary);
        Top(home.rectTransform, 49f, 11f, 103f, 28f);
        home.alignment = TextAlignmentOptions.Center;
        Button commandButton = AddButton(command);
        AddPersistent(commandButton.onClick, navigation.OpenCommandCenter);

        string[] names = { "HIERRO", "ALUMINIO", "NÍQUEL" };
        string[] amounts = { "5.98M", "4.73M", "4.70M" };
        string[] rates = { "+0.30/s", "+0.08/s", "+0.03/s" };
        float x = 18f;
        for (int i = 0; i < 3; i++)
        {
            RectTransform chip = Panel("Metal_" + i, root, frame, fill,
                new Vector2(x, 66f), new Vector2(190f, 74f), Fill, CyanMuted);
            DrawMetalIcon(chip, new Vector2(-68f, -1f), 24f, i, Secondary);
            TMP_Text name = Text("Name", chip, font, names[i], 13f, FontStyles.Bold, Secondary);
            Top(name.rectTransform, 49f, 8f, 120f, 20f);
            TMP_Text amount = Text("Amount", chip, font, amounts[i], 24f, FontStyles.Normal, Primary);
            Top(amount.rectTransform, 49f, 25f, 106f, 34f);
            TMP_Text rate = Text("Rate", chip, font, rates[i], 13f, FontStyles.Bold, Cyan);
            Top(rate.rectTransform, 119f, 42f, 62f, 23f);
            rate.alignment = TextAlignmentOptions.Right;
            x += 197f;
        }

        RectTransform metals = Panel("MetalsButton", root, frame, fill,
            new Vector2(802f, 66f), new Vector2(260f, 74f), Fill, CyanMuted);
        TMP_Text metalLabel = Text("Label", metals, font, "10 METALES", 17f, FontStyles.Bold, Cyan);
        Top(metalLabel.rectTransform, 22f, 20f, 185f, 34f);
        metalLabel.alignment = TextAlignmentOptions.Center;
        Line("ChevronA", metals, new Vector2(94f, 4f), new Vector2(103f, -5f), 2f, Cyan);
        Line("ChevronB", metals, new Vector2(103f, -5f), new Vector2(112f, 4f), 2f, Cyan);
        Button metalsButton = AddButton(metals);
        if (metalsInventory != null) AddPersistent(metalsButton.onClick, metalsInventory.Open);
    }

    private static void BuildTreeShell(Transform root, Sprite frame, Sprite fill, Sprite glow,
        TMP_FontAsset font, Sprite lockSprite, Dimension1TreeVisualUI visual)
    {
        RectTransform titleBand = Panel("TreeTitleBand", root, frame, fill,
            new Vector2(18f, 190f), new Vector2(1044f, 79f), Hex("020C12", 247), CyanMuted);
        TMP_Text heading = Text("MainHeading", titleBand, font, "ÁRBOL CUÁNTICO", 31f, FontStyles.Bold, Primary);
        Top(heading.rectTransform, 80f, 17f, 430f, 47f);
        heading.characterSpacing = 1.5f;
        Image treeGlyph = Image("TreeGlyph", root,
            LoadSprite(ArtPath + "/NavigationPremium/d1_nav_tree_premium_v1.png"), Color.white);
        Top(treeGlyph.rectTransform, 37f, 199f, 55f, 55f);
        treeGlyph.preserveAspect = true;
        treeGlyph.raycastTarget = false;
        treeGlyph.material = AssetDatabase.LoadAssetAtPath<Material>(
            ArtPath + "/NavigationPremium/d1_nav_premium_black_key.mat");
        TMP_Text pointsLabel = Text("PointsLabel", titleBand, font, "PUNTOS DISPONIBLES:", 18f,
            FontStyles.Bold, Cyan);
        Top(pointsLabel.rectTransform, 676f, 21f, 260f, 35f);
        pointsLabel.alignment = TextAlignmentOptions.Right;
        TMP_Text points = Text("AvailablePoints", titleBand, font, "3", 24f, FontStyles.Bold, Primary);
        Top(points.rectTransform, 942f, 16f, 48f, 43f);
        points.alignment = TextAlignmentOptions.Center;

        RectTransform orbit = Rect("OrbitalConstellation", root);
        Top(orbit, 10f, 254f, 1060f, 1172f);
        BuildOrbitalField(orbit, glow, font);

        NodeSpec[] nodes =
        {
            new NodeSpec(1, "LECTURA\nDE DESTINOS", "1 / 1", 530f, 111f, 150f, 194f),
            new NodeSpec(2, "PREPARACIÓN\nDE HANGAR", "1 / 1", 246f, 225f, 148f, 198f),
            new NodeSpec(3, "REGISTRO\nDE COPIAS", "1 / 3", 814f, 225f, 148f, 198f),
            new NodeSpec(4, "MEMORIA\nDE ESCANEO", "1 / 3", 112f, 505f, 148f, 200f),
            new NodeSpec(5, "LECTURA\nDE RELIQUIAS", "2 / 3", 948f, 505f, 148f, 200f),
            new NodeSpec(6, "RASTREO DE\nHALLAZGOS OCULTOS", "1 / 3", 152f, 760f, 148f, 202f),
            new NodeSpec(7, "COORDINACIÓN\nDE FLOTA", "1 / 1", 908f, 760f, 148f, 202f),
            new NodeSpec(8, "OPTIMIZACIÓN\nDE RUTA", "0 / 1", 286f, 984f, 148f, 198f),
            new NodeSpec(9, "CARTOGRAFÍA\nAVANZADA", "0 / 1", 774f, 984f, 148f, 198f, true),
            new NodeSpec(10, "ESTABILIZACIÓN DE\nZONA INESTABLE", "0 / 1", 530f, 1072f, 170f, 184f)
        };
        foreach (NodeSpec node in nodes) BuildNode(orbit, frame, fill, glow, font,
            lockSprite, visual, node);
    }

    private static void BuildOrbitalField(RectTransform orbit, Sprite glow, TMP_FontAsset font)
    {
        Vector2 core = Local(orbit, 530f, 568f);
        Image orbitalField = Image("DetailedOrbitalField", orbit,
            LoadSprite(TreePath + "/d1_tree_orbital_field_v1.png"), Hex("D8F7FF", 178));
        Centered(orbitalField.rectTransform, core, new Vector2(1040f, 1040f));
        orbitalField.preserveAspect = true;
        orbitalField.raycastTarget = false;
        Image aura = Image("CoreAura", orbit, glow, Hex("18C8FF", 48));
        Centered(aura.rectTransform, core, new Vector2(470f, 470f));
        aura.raycastTarget = false;
        Image canonicalOrbit = Image("CanonicalOrbitRing", orbit,
            LoadSprite(ArtPath + "/d1_orbit_ring_v3.png"), Hex("18C8FF", 34));
        Centered(canonicalOrbit.rectTransform, core, new Vector2(955f, 955f));
        canonicalOrbit.preserveAspect = true;
        canonicalOrbit.raycastTarget = false;
        Circle(orbit, core, 113f, 1.2f, Hex("18C8FF", 105));
        Circle(orbit, core, 166f, 1.2f, Hex("18C8FF", 95));
        Circle(orbit, core, 221f, 1.4f, Hex("18C8FF", 185));
        Circle(orbit, core, 285f, 1.2f, Hex("18C8FF", 105));
        Circle(orbit, core, 350f, 1.7f, Hex("18C8FF", 210));
        Circle(orbit, core, 418f, 1.1f, Hex("087FA9", 105));
        Circle(orbit, core, 486f, 1.4f, Hex("18C8FF", 145));
        DrawEllipse(orbit, core + Vector2.down * 11f, 126f, 29f, 1.4f, Hex("18C8FF", 190));
        DrawEllipse(orbit, core + Vector2.down * 11f, 205f, 49f, 1.1f, Hex("087FA9", 125));
        for (int i = 0; i < 96; i++)
        {
            float a = Mathf.PI * 2f * i / 96f;
            float inner = i % 8 == 0 ? 392f : 399f;
            float outer = i % 8 == 0 ? 415f : 408f;
            Vector2 d = new Vector2(Mathf.Cos(a), Mathf.Sin(a));
            Line("OrbitTick_" + i, orbit, core + d * inner, core + d * outer,
                i % 8 == 0 ? 1.5f : .8f, Hex("18C8FF", i % 8 == 0 ? (byte)145 : (byte)65));
        }

        Vector2[] centers =
        {
            Local(orbit,530f,111f), Local(orbit,246f,225f), Local(orbit,814f,225f),
            Local(orbit,112f,505f), Local(orbit,948f,505f), Local(orbit,152f,760f),
            Local(orbit,908f,760f), Local(orbit,286f,984f), Local(orbit,774f,984f), Local(orbit,530f,1072f)
        };
        for (int i = 0; i < centers.Length; i++)
        {
            Vector2 target = centers[i];
            Vector2 direction = (target - core).normalized;
            Line("EnergyRoute_" + i, orbit, core + direction * 115f, target - direction * 82f, 2.1f,
                Hex("18C8FF", 205));
            Dot(orbit, core + direction * 220f, 8f, CyanBright);
            Dot(orbit, core + direction * 350f, 9f, Cyan);
            Dot(orbit, target - direction * 83f, 9f, CyanBright);
        }

        for (int i = 0; i < 54; i++)
        {
            float a = Mathf.PI * 2f * i / 54f;
            float radius = 92f + (i % 5) * 28f;
            Vector2 p = core + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius;
            Dot(orbit, p, i % 3 == 0 ? 5f : 3f, Hex("18C8FF", 100));
        }

        DrawQuantumCrystal(orbit, glow, core);
        DrawEllipse(orbit, core + Vector2.down * 122f, 62f, 15f, 1.5f, Hex("18C8FF", 220));
        Dot(orbit, core + Vector2.down * 122f, 8f, CyanBright);

        AddSectorLabel(orbit, "SECTOR 1", font, core + Vector2.up * 246f);
        AddSectorLabel(orbit, "SECTOR 2", font, core + Vector2.down * 222f);
        AddSectorLabel(orbit, "SECTOR 3", font, core + Vector2.down * 344f);
    }

    private static void DrawQuantumCrystal(Transform parent, Sprite glow, Vector2 center)
    {
        Image crystalGlow = Image("QuantumCrystalGlow", parent, glow, Hex("18C8FF", 118));
        Centered(crystalGlow.rectTransform, center, new Vector2(250f, 310f));
        crystalGlow.raycastTarget = false;
        Vector2 top = center + Vector2.up * 121f;
        Vector2 upperLeft = center + new Vector2(-42f, 32f);
        Vector2 upperRight = center + new Vector2(42f, 32f);
        Vector2 middle = center + new Vector2(0f, -8f);
        Vector2 lowerLeft = center + new Vector2(-31f, -43f);
        Vector2 lowerRight = center + new Vector2(31f, -43f);
        Vector2 bottom = center + Vector2.down * 121f;
        Polygon("CrystalBody", parent,
            new[] { top, upperRight, lowerRight, bottom, lowerLeft, upperLeft },
            Hex("075C86", 48));
        Polygon("CrystalLightFacet", parent,
            new[] { top, upperLeft, middle, upperRight }, Hex("18C8FF", 38));
        Polygon("CrystalDeepFacet", parent,
            new[] { middle, lowerRight, bottom, lowerLeft }, Hex("003C62", 52));
        LineLoop("CrystalEdge", parent,
            new[] { top, upperRight, lowerRight, bottom, lowerLeft, upperLeft },
            2.8f, CyanBright);
        Line("CrystalSpine", parent, top, bottom, 2.4f, CyanBright);
        Line("CrystalFacetA", parent, upperLeft, middle, 2f, Cyan);
        Line("CrystalFacetB", parent, upperRight, middle, 2f, Cyan);
        Line("CrystalFacetC", parent, lowerLeft, middle, 2f, Cyan);
        Line("CrystalFacetD", parent, lowerRight, middle, 2f, Cyan);
        Line("CrystalWaist", parent, upperLeft, lowerLeft, 1.8f, Cyan);
        Line("CrystalWaistR", parent, upperRight, lowerRight, 1.8f, Cyan);
    }

    private static void AddSectorLabel(Transform parent, string value, TMP_FontAsset font, Vector2 center)
    {
        TMP_Text label = Text(value.Replace(" ", "") + "Label", parent, font, value, 15f,
            FontStyles.Bold, Cyan);
        Centered(label.rectTransform, center, new Vector2(160f, 26f));
        label.alignment = TextAlignmentOptions.Center;
        label.characterSpacing = 1.5f;
    }

    private static void BuildNode(RectTransform orbit, Sprite frame, Sprite fill, Sprite glow,
        TMP_FontAsset font, Sprite lockSprite, Dimension1TreeVisualUI visual, NodeSpec spec)
    {
        Color accent = spec.selected ? Amber : Cyan;
        Color fillColor = spec.selected ? Hex("171105", 249) : Hex("03121C", 247);
        Vector2 topLeft = spec.center - spec.size * .5f;
        RectTransform card = Panel("TreeNode_" + spec.number, orbit, frame, fill, topLeft, spec.size,
            fillColor, accent);
        Image selectionGlow = Image("SelectionGlow", card, glow, Hex("F4A70B", 42));
        Stretch(selectionGlow.rectTransform, new Vector2(-18f, -18f), new Vector2(-18f, -18f));
        selectionGlow.raycastTarget = false;
        selectionGlow.transform.SetAsFirstSibling();
        selectionGlow.gameObject.SetActive(spec.selected);
        TMP_Text number = Text("Number", card, font, spec.number.ToString(), 19f, FontStyles.Normal, accent);
        Top(number.rectTransform, spec.size.x * .5f - 22f, 5f, 44f, 28f);
        number.alignment = TextAlignmentOptions.Center;
        RectTransform icon = Rect("NodeIcon", card);
        Top(icon, 22f, 31f, spec.size.x - 44f, 74f);
        BuildNodeIconArt(icon, spec.number, accent);
        TMP_Text label = Text("Name", card, font, spec.label,
            spec.number == 6 || spec.number == 10 ? 13f : 14.5f,
            FontStyles.Bold, spec.selected ? Amber : Primary);
        Top(label.rectTransform, 7f, 108f, spec.size.x - 14f, 53f);
        label.textWrappingMode = TextWrappingModes.Normal;
        label.overflowMode = TextOverflowModes.Overflow;
        label.alignment = TextAlignmentOptions.Center;
        label.lineSpacing = -8f;
        TMP_Text progress = Text("Progress", card, font, spec.progress, 17f, FontStyles.Bold, accent);
        Top(progress.rectTransform, 18f, spec.size.y - 34f, spec.size.x - 36f, 27f);
        progress.alignment = TextAlignmentOptions.Center;

        Image lockedOverlay = Image("LockedOverlay", card, fill, Hex("01070B", 224));
        Stretch(lockedOverlay.rectTransform, new Vector2(4f, 4f), new Vector2(4f, 4f));
        lockedOverlay.type = UnityEngine.UI.Image.Type.Sliced;
        lockedOverlay.raycastTarget = false;
        Image lockIcon = Image("LockIcon", lockedOverlay.transform, lockSprite, Locked);
        Centered(lockIcon.rectTransform, new Vector2(0f, 7f), new Vector2(44f, 44f));
        lockIcon.preserveAspect = true;
        lockIcon.raycastTarget = false;
        lockedOverlay.gameObject.SetActive(false);

        Button button = AddButton(card);
        UnityEventTools.AddIntPersistentListener(button.onClick, visual.SelectNode, spec.number - 1);
    }

    private static void BuildNodeIconArt(RectTransform icon, int number, Color accent)
    {
        switch (number)
        {
            case 2:
                Image blueprint = AddSpriteIcon(icon,
                    LoadSprite(ArtPath + "/d1_hangar_sonda_ligera_blueprint_v2.png"),
                    new Vector2(112f, 94f), Cyan);
                blueprint.material = AssetDatabase.LoadAssetAtPath<Material>(
                    ArtPath + "/d1_hangar_blueprint_keyed.mat");
                if (blueprint.material == null)
                    throw new InvalidOperationException("Falta el material canónico de blueprint del Hangar.");
                return;
            case 3:
                AddSpriteIcon(icon, LoadSprite(RelicPath + "/d1_relic_lost_navigation_record.png"),
                    new Vector2(62f, 66f), Primary);
                return;
            case 5:
                Dimension1ExploreReferenceSetup.DrawNavigationIcon(icon, 3, Vector2.zero, 32f, accent);
                return;
            case 7:
                for (int i = -1; i <= 1; i++)
                {
                    Dimension1ExploreReferenceSetup.DrawNavigationIcon(icon, 2,
                        new Vector2(i * 27f, i == 0 ? 4f : -8f), i == 0 ? 20f : 14f, accent);
                }
                return;
            case 9:
                AddSpriteIcon(icon, LoadSprite(TreePath + "/d1_tree_advanced_cartography_v2.png"),
                    new Vector2(82f, 76f), Color.white);
                return;
            case 10:
                Dimension1ExploreReferenceSetup.DrawNavigationIcon(icon, 0, Vector2.zero, 31f, Cyan);
                return;
            default:
                DrawNodeIcon(icon, number, Vector2.zero, 61f, accent);
                return;
        }
    }

    private static Image AddSpriteIcon(Transform parent, Sprite sprite, Vector2 size, Color color)
    {
        if (sprite == null) throw new InvalidOperationException("Falta un icono reutilizado del Árbol Cuántico.");
        Image image = Image("CanonicalIcon", parent, sprite, color);
        Centered(image.rectTransform, Vector2.zero, size);
        image.preserveAspect = true;
        image.raycastTarget = false;
        return image;
    }

    private static void BuildDetail(Transform root, Sprite frame, Sprite fill, Sprite glow,
        TMP_FontAsset font, Dimension1TreeVisualUI visual)
    {
        RectTransform detail = Panel("SelectedNodeDetail", root, frame, fill,
            new Vector2(20f, 1426f), new Vector2(1040f, 278f), Hex("031018", 252), CyanMuted);
        RectTransform iconCard = Panel("SelectedIconCard", detail, frame, fill,
            new Vector2(24f, 18f), new Vector2(184f, 240f), AmberFill, Amber);
        Image selectedGlow = Image("SelectionGlow", iconCard, glow, Hex("F4A70B", 42));
        Stretch(selectedGlow.rectTransform, new Vector2(-14f, -14f), new Vector2(-14f, -14f));
        selectedGlow.transform.SetAsFirstSibling();
        for (int i = 0; i < Dimension1System.Dimension1TreeNodeIds.Length; i++)
        {
            RectTransform detailIcon = Rect("DetailIcon_" + (i + 1), iconCard);
            Centered(detailIcon, new Vector2(0f, 3f), new Vector2(154f, 154f));
            BuildNodeIconArt(detailIcon, i + 1, Amber);
            detailIcon.localScale = Vector3.one * 1.65f;
            detailIcon.gameObject.SetActive(i == 8);
        }

        TMP_Text name = Text("SelectedName", detail, font, "CARTOGRAFÍA AVANZADA", 28f,
            FontStyles.Bold, Primary);
        Top(name.rectTransform, 233f, 21f, 492f, 44f);
        name.enableAutoSizing = true;
        name.fontSizeMin = 18f;
        name.fontSizeMax = 28f;
        RectTransform levelChip = Panel("LevelChip", detail, frame, fill,
            new Vector2(733f, 17f), new Vector2(194f, 52f), Fill, Amber);
        TMP_Text level = Text("SelectedLevel", levelChip, font, "NIVEL 0 / 1", 18f,
            FontStyles.Bold, Amber);
        Stretch(level.rectTransform, new Vector2(10f, 7f), new Vector2(10f, 7f));
        level.alignment = TextAlignmentOptions.Center;

        RectTransform effectA = Panel("EffectPrimary", detail, frame, fill,
            new Vector2(230f, 83f), new Vector2(480f, 67f), Hex("03131C", 252), CyanMuted);
        DrawTargetIcon(effectA, new Vector2(-206f, 0f), 18f, Cyan);
        TMP_Text effectAText = Text("Value", effectA, font, "+5% DESTINOS RAROS COMPATIBLES", 16f,
            FontStyles.Normal, Secondary);
        Top(effectAText.rectTransform, 58f, 8f, 403f, 51f);
        effectAText.textWrappingMode = TextWrappingModes.Normal;
        effectAText.overflowMode = TextOverflowModes.Truncate;
        effectAText.alignment = TextAlignmentOptions.MidlineLeft;

        RectTransform effectB = Panel("EffectSecondary", detail, frame, fill,
            new Vector2(230f, 158f), new Vector2(480f, 67f), Hex("03131C", 252), CyanMuted);
        DrawSpecialPointIcon(effectB, new Vector2(-206f, 0f), 18f, Cyan);
        TMP_Text effectBText = Text("Value", effectB, font, "+2% PROBABILIDAD DE PUNTO ESPECIAL", 16f,
            FontStyles.Normal, Secondary);
        Top(effectBText.rectTransform, 58f, 8f, 403f, 51f);
        effectBText.textWrappingMode = TextWrappingModes.Normal;
        effectBText.overflowMode = TextOverflowModes.Truncate;
        effectBText.alignment = TextAlignmentOptions.MidlineLeft;

        RectTransform cost = Panel("CostPanel", detail, frame, fill,
            new Vector2(730f, 83f), new Vector2(286f, 67f), Fill, CyanMuted);
        DrawTargetIcon(cost, new Vector2(-111f, 0f), 20f, Cyan);
        TMP_Text costLabel = Text("Label", cost, font, "COSTO", 14f, FontStyles.Bold, Cyan);
        Top(costLabel.rectTransform, 57f, 8f, 205f, 20f);
        TMP_Text costValue = Text("Value", cost, font, "3 PUNTOS", 21f, FontStyles.Bold, Amber);
        Top(costValue.rectTransform, 57f, 29f, 205f, 30f);

        RectTransform unlock = Panel("UnlockButton", detail, frame, fill,
            new Vector2(730f, 158f), new Vector2(286f, 78f), AmberFill, Amber);
        TMP_Text unlockLabel = Text("Label", unlock, font, "DESBLOQUEAR", 23f, FontStyles.Bold, Amber);
        Stretch(unlockLabel.rectTransform);
        unlockLabel.alignment = TextAlignmentOptions.Center;
        unlockLabel.characterSpacing = 2f;
        unlockLabel.enableAutoSizing = true;
        unlockLabel.fontSizeMin = 14f;
        unlockLabel.fontSizeMax = 23f;
        Button unlockButton = AddButton(unlock);
        AddPersistent(unlockButton.onClick, visual.BuySelectedNode);
    }

    private static void BuildNavigation(Transform root, Sprite frame, Sprite fill, TMP_FontAsset font,
        Dimension1TreeNavigationUI navigation)
    {
        RectTransform nav = Rect("BottomNavigation", root);
        Top(nav, Dimension1SharedLayoutTokens.NavigationX,
            Dimension1SharedLayoutTokens.NavigationY,
            Dimension1SharedLayoutTokens.NavigationWidth,
            Dimension1SharedLayoutTokens.NavigationHeight);
        string[] labels = { "GALAXIA", "EXPLORAR", "HANGAR", "RELIQUIAS", "ÁRBOL" };
        for (int i = 0; i < labels.Length; i++)
        {
            bool selected = i == 4;
            RectTransform item = Panel("Nav_" + labels[i], nav, frame, fill,
                new Vector2(Dimension1SharedLayoutTokens.NavigationCardX(i),
                    Dimension1SharedLayoutTokens.NavigationCardY),
                new Vector2(Dimension1SharedLayoutTokens.NavigationCardWidth,
                    Dimension1SharedLayoutTokens.NavigationCardHeight),
                selected ? AmberFill : Fill, selected ? Amber : CyanMuted);
            Dimension1ExploreReferenceSetup.DrawNavigationIcon(item, i, new Vector2(0f, 28f), 42f,
                selected ? Amber : Cyan);
            TMP_Text label = Text("Label", item, font, labels[i], 17f, FontStyles.Bold,
                selected ? Amber : Cyan);
            Top(label.rectTransform, 12f, 116f, 176f, 32f);
            label.alignment = TextAlignmentOptions.Center;
            Button button = AddButton(item);
            if (selected) button.interactable = false;
            else if (i == 0) AddPersistent(button.onClick, navigation.OpenGalaxy);
            else if (i == 1) AddPersistent(button.onClick, navigation.OpenExplore);
            else if (i == 2) AddPersistent(button.onClick, navigation.OpenHangar);
            else if (i == 3) AddPersistent(button.onClick, navigation.OpenRelics);
        }
    }

    private static void ConfigureVisual(Transform root, Dimension1TreeVisualUI visual)
    {
        var metalAmounts = new TMP_Text[3];
        var metalRates = new TMP_Text[3];
        for (int i = 0; i < 3; i++)
        {
            Transform chip = FindChild(root, "Metal_" + i);
            metalAmounts[i] = FindDirectChild(chip, "Amount")?.GetComponent<TMP_Text>();
            metalRates[i] = FindDirectChild(chip, "Rate")?.GetComponent<TMP_Text>();
        }

        int count = Dimension1System.Dimension1TreeNodeIds.Length;
        var buttons = new Button[count];
        var fills = new Image[count];
        var borders = new Image[count];
        var glows = new Image[count];
        var locks = new GameObject[count];
        var names = new TMP_Text[count];
        var numbers = new TMP_Text[count];
        var progress = new TMP_Text[count];
        var detailIcons = new GameObject[count];
        Transform iconCard = FindChild(root, "SelectedIconCard");
        for (int i = 0; i < count; i++)
        {
            Transform card = FindChild(root, "TreeNode_" + (i + 1));
            buttons[i] = card != null ? card.GetComponent<Button>() : null;
            fills[i] = FindDirectChild(card, "Fill")?.GetComponent<Image>();
            borders[i] = FindDirectChild(card, "Border")?.GetComponent<Image>();
            glows[i] = FindDirectChild(card, "SelectionGlow")?.GetComponent<Image>();
            locks[i] = FindDirectChild(card, "LockedOverlay")?.gameObject;
            names[i] = FindDirectChild(card, "Name")?.GetComponent<TMP_Text>();
            numbers[i] = FindDirectChild(card, "Number")?.GetComponent<TMP_Text>();
            progress[i] = FindDirectChild(card, "Progress")?.GetComponent<TMP_Text>();
            detailIcons[i] = FindDirectChild(iconCard, "DetailIcon_" + (i + 1))?.gameObject;
        }

        Transform detail = FindChild(root, "SelectedNodeDetail");
        Transform effectPrimary = FindDirectChild(detail, "EffectPrimary");
        Transform effectSecondary = FindDirectChild(detail, "EffectSecondary");
        Transform cost = FindDirectChild(detail, "CostPanel");
        Transform unlock = FindDirectChild(detail, "UnlockButton");
        visual.Configure(
            metalAmounts,
            metalRates,
            FindChild(root, "AvailablePoints")?.GetComponent<TMP_Text>(),
            buttons,
            fills,
            borders,
            glows,
            locks,
            names,
            numbers,
            progress,
            detailIcons,
            FindDirectChild(detail, "SelectedName")?.GetComponent<TMP_Text>(),
            FindChild(detail, "SelectedLevel")?.GetComponent<TMP_Text>(),
            FindDirectChild(effectPrimary, "Value")?.GetComponent<TMP_Text>(),
            FindDirectChild(effectSecondary, "Value")?.GetComponent<TMP_Text>(),
            FindDirectChild(cost, "Value")?.GetComponent<TMP_Text>(),
            unlock != null ? unlock.GetComponent<Button>() : null,
            FindDirectChild(unlock, "Label")?.GetComponent<TMP_Text>(),
            FindDirectChild(unlock, "Fill")?.GetComponent<Image>(),
            FindDirectChild(unlock, "Border")?.GetComponent<Image>());
    }

    private static void DrawNodeIcon(Transform parent, int variant, Vector2 center, float size, Color color)
    {
        float s = size;
        switch (variant)
        {
            case 1:
                Circle(parent, center, s * .43f, 1.8f, color);
                Circle(parent, center, s * .27f, 1.1f, color);
                Circle(parent, center, s * .08f, 1.2f, color);
                DrawEllipse(parent, center, s * .44f, s * .18f, 1.2f, color);
                Line("Axis", parent, center + Vector2.down * s * .48f, center + Vector2.up * s * .48f, 1.2f, color);
                Polygon("Needle", parent, new[] { center + Vector2.up * s * .30f, center + new Vector2(-s * .10f, 0f), center + Vector2.down * s * .28f, center + new Vector2(s * .10f, 0f) }, Hex("18C8FF", 55));
                LineLoop("NeedleEdge", parent, new[] { center + Vector2.up * s * .30f, center + new Vector2(-s * .10f, 0f), center + Vector2.down * s * .28f, center + new Vector2(s * .10f, 0f) }, 1.2f, color);
                break;
            case 2:
                LineLoop("Hangar", parent, new[] { center + new Vector2(-s*.38f,-s*.32f), center + new Vector2(-s*.38f,s*.18f), center + new Vector2(0f,s*.42f), center + new Vector2(s*.38f,s*.18f), center + new Vector2(s*.38f,-s*.32f) }, 1.8f, color);
                Line("Ship", parent, center + new Vector2(0f,-s*.22f), center + new Vector2(0f,s*.22f), 2f, color);
                Line("WingL", parent, center, center + new Vector2(-s*.18f,-s*.16f), 1.7f, color);
                Line("WingR", parent, center, center + new Vector2(s*.18f,-s*.16f), 1.7f, color);
                break;
            case 3:
                LineLoop("Clipboard", parent, new[] { center + new Vector2(-s*.30f,-s*.38f), center + new Vector2(-s*.30f,s*.32f), center + new Vector2(s*.30f,s*.32f), center + new Vector2(s*.30f,-s*.38f) }, 1.8f, color);
                LineLoop("Clip", parent, new[] { center + new Vector2(-s*.13f,s*.42f), center + new Vector2(s*.13f,s*.42f), center + new Vector2(s*.13f,s*.25f), center + new Vector2(-s*.13f,s*.25f) }, 1.6f, color);
                for (int i=0;i<3;i++) Line("Entry", parent, center + new Vector2(-s*.16f,s*(.12f-i*.18f)), center + new Vector2(s*.18f,s*(.12f-i*.18f)), 1.3f, color);
                break;
            case 4:
                Circle(parent, center, s*.39f, 1.7f, color); Circle(parent, center, s*.25f, 1.2f, color); Circle(parent, center, s*.10f, 1.3f, color); Dot(parent, center, s*.07f, color);
                for (int i=0;i<12;i++){ float a=Mathf.PI*2*i/12; Vector2 d=new Vector2(Mathf.Cos(a),Mathf.Sin(a)); Line("Ray",parent,center+d*s*.39f,center+d*s*(i%3==0?.53f:.47f),i%3==0?1.5f:1f,color); }
                Dot(parent, center + new Vector2(s*.31f,s*.18f), s*.09f, color);
                break;
            case 5:
                Polygon("RelicFill", parent, new[] { center+Vector2.up*s*.44f, center+new Vector2(s*.23f,0f), center+Vector2.down*s*.44f, center+new Vector2(-s*.23f,0f) }, Hex("18C8FF", 38));
                LineLoop("Relic", parent, new[] { center+Vector2.up*s*.44f, center+new Vector2(s*.23f,0f), center+Vector2.down*s*.44f, center+new Vector2(-s*.23f,0f) }, 1.8f,color);
                Line("FacetA",parent,center+Vector2.up*s*.44f,center,1.2f,color); Line("FacetB",parent,center+Vector2.down*s*.44f,center,1.2f,color); Line("FacetC",parent,center+new Vector2(-s*.23f,0f),center+new Vector2(s*.23f,0f),1.2f,color);
                break;
            case 6:
                Circle(parent, center + new Vector2(-s*.08f,s*.08f), s*.29f, 2f, color);
                Circle(parent, center + new Vector2(-s*.08f,s*.08f), s*.21f, 1f, Hex("18C8FF", 140));
                Line("MagnifierHandle", parent, center + new Vector2(s*.12f,-s*.14f), center + new Vector2(s*.39f,-s*.41f), 3f, color);
                Line("MagnifierGlint", parent, center + new Vector2(-s*.22f,s*.23f), center + new Vector2(-s*.10f,s*.31f), 1.3f, CyanBright);
                break;
            case 7:
                for(int i=-1;i<=1;i++){ float x=i*s*.24f; LineLoop("Fleet",parent,new[]{center+new Vector2(x,-s*.32f),center+new Vector2(x-s*.10f,-s*.08f),center+new Vector2(x,s*.34f),center+new Vector2(x+s*.10f,-s*.08f)},1.5f,color); }
                Line("Formation",parent,center+new Vector2(-s*.36f,-s*.38f),center+new Vector2(s*.36f,-s*.38f),1.2f,color);
                break;
            case 8:
                Circle(parent,center+new Vector2(s*.25f,s*.20f),s*.15f,1.7f,color); Dot(parent,center+new Vector2(s*.25f,s*.20f),s*.08f,color);
                LineGraphic("Route",parent,new[]{center+new Vector2(-s*.38f,-s*.26f),center+new Vector2(-s*.18f,-s*.21f),center+new Vector2(-s*.08f,-s*.10f),center+new Vector2(s*.02f,s*.08f),center+new Vector2(s*.18f,s*.16f)},1.7f,false,color);
                Dot(parent,center+new Vector2(-s*.38f,-s*.26f),s*.11f,color);
                Dot(parent,center+new Vector2(-s*.08f,-s*.10f),s*.06f,color);
                break;
            case 9:
                DrawEllipse(parent,center+new Vector2(0f,-s*.08f),s*.40f,s*.22f,1.7f,color);
                LineGraphic("ContoursA",parent,new[]{center+new Vector2(-s*.34f,-s*.07f),center+new Vector2(-s*.13f,s*.02f),center+new Vector2(s*.02f,-s*.10f),center+new Vector2(s*.28f,-s*.02f)},1.2f,false,color);
                Circle(parent,center+new Vector2(s*.24f,s*.27f),s*.15f,1.7f,color); Dot(parent,center+new Vector2(s*.24f,s*.27f),s*.08f,color);
                Line("Pin",parent,center+new Vector2(s*.24f,s*.12f),center+new Vector2(s*.10f,-s*.06f),1.6f,color);
                break;
            default:
                DrawEllipse(parent,center,s*.40f,s*.20f,1.6f,color); DrawEllipse(parent,center,s*.30f,s*.13f,1.2f,color);
                Line("GalaxyA",parent,center+new Vector2(-s*.40f,-s*.18f),center+new Vector2(s*.40f,s*.18f),1.2f,color);
                Line("GalaxyB",parent,center+new Vector2(-s*.40f,s*.18f),center+new Vector2(s*.40f,-s*.18f),1.2f,color); Dot(parent,center,s*.10f,color);
                break;
        }
    }

    private static void DrawCrystal(Transform parent, Vector2 c, float size, Color color)
    {
        Polygon("CrystalFill", parent, new[] { c+Vector2.up*size*.62f, c+new Vector2(size*.30f,size*.12f), c+new Vector2(size*.22f,-size*.50f), c+Vector2.down*size*.72f, c+new Vector2(-size*.22f,-size*.50f), c+new Vector2(-size*.30f,size*.12f) }, Hex("18C8FF", 45));
        LineLoop("CrystalEdge", parent, new[] { c+Vector2.up*size*.62f, c+new Vector2(size*.30f,size*.12f), c+new Vector2(size*.22f,-size*.50f), c+Vector2.down*size*.72f, c+new Vector2(-size*.22f,-size*.50f), c+new Vector2(-size*.30f,size*.12f) }, 2.6f,color);
        Line("FacetTop",parent,c+Vector2.up*size*.62f,c,1.8f,color); Line("FacetBottom",parent,c,c+Vector2.down*size*.72f,1.8f,color);
        Line("FacetL",parent,c+new Vector2(-size*.30f,size*.12f),c,1.6f,color); Line("FacetR",parent,c+new Vector2(size*.30f,size*.12f),c,1.6f,color);
        DrawEllipse(parent,c+Vector2.down*size*.82f,size*.42f,size*.10f,1.5f,Hex("18C8FF",180));
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

    private static void DrawTargetIcon(Transform parent, Vector2 c, float s, Color color)
    {
        Circle(parent,c,s,1.5f,color); Circle(parent,c,s*.48f,1.4f,color); Dot(parent,c,s*.18f,color);
        Line("TargetH",parent,c+Vector2.left*s*1.25f,c+Vector2.right*s*1.25f,1.2f,color);
        Line("TargetV",parent,c+Vector2.down*s*1.25f,c+Vector2.up*s*1.25f,1.2f,color);
    }

    private static void DrawSpecialPointIcon(Transform parent, Vector2 c, float s, Color color)
    {
        Circle(parent, c, s, 1.5f, color);
        for (int i = 0; i < 8; i++)
        {
            float a = Mathf.PI * 2f * i / 8f;
            Vector2 d = new Vector2(Mathf.Cos(a), Mathf.Sin(a));
            Line("SpecialRay_" + i, parent, c + d * s * 1.2f, c + d * s * 1.55f, 1.2f, color);
        }
        Polygon("SpecialCore", parent, new[]
        {
            c + Vector2.up * s*.52f, c + Vector2.right * s*.52f,
            c + Vector2.down * s*.52f, c + Vector2.left * s*.52f
        }, WithAlpha(color, 120));
    }

    private static RectTransform Panel(string name, Transform parent, Sprite frame, Sprite fill,
        Vector2 topLeft, Vector2 size, Color fillColor, Color accent)
    {
        RectTransform result = Rect(name,parent);
        Top(result,topLeft.x,topLeft.y,size.x,size.y);
        Image shadow = Image("Shadow",result,fill,Hex("000000",125));
        Stretch(shadow.rectTransform,new Vector2(5f,-5f),new Vector2(5f,-5f));
        shadow.type=UnityEngine.UI.Image.Type.Sliced; shadow.raycastTarget=false;
        Image surface = Image("Fill",result,fill,fillColor);
        Stretch(surface.rectTransform); surface.type=UnityEngine.UI.Image.Type.Sliced; surface.raycastTarget=false;
        Image border = Image("Border",result,frame,WithAlpha(accent,215));
        Stretch(border.rectTransform); border.type=UnityEngine.UI.Image.Type.Sliced; border.raycastTarget=false;
        return result;
    }

    private static Button AddButton(RectTransform root)
    {
        Button button = root.gameObject.AddComponent<Button>();
        Transform fill = FindDirectChild(root, "Fill");
        button.targetGraphic = fill != null ? fill.GetComponent<Image>() : null;
        if (button.targetGraphic != null)
            button.targetGraphic.raycastTarget = true;
        button.transition = Selectable.Transition.ColorTint;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 1f, 1f, .88f);
        colors.pressedColor = new Color(.72f, .88f, .94f, 1f);
        colors.selectedColor = Color.white;
        colors.disabledColor = new Color(.72f, .72f, .72f, .72f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = .08f;
        button.colors = colors;
        return button;
    }

    private static void Circle(Transform parent, Vector2 center, float radius, float width, Color color)
    {
        var points = new List<Vector2>();
        for(int i=0;i<72;i++){float a=Mathf.PI*2f*i/72f;points.Add(center+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*radius);}
        LineGraphic("Circle",parent,points,width,true,color);
    }

    private static void DrawEllipse(Transform parent, Vector2 center, float rx, float ry, float width, Color color)
    {
        var points = new List<Vector2>();
        for(int i=0;i<64;i++){float a=Mathf.PI*2f*i/64f;points.Add(center+new Vector2(Mathf.Cos(a)*rx,Mathf.Sin(a)*ry));}
        LineGraphic("Ellipse",parent,points,width,true,color);
    }

    private static void Dot(Transform parent, Vector2 center, float diameter, Color color)
    {
        var points=new List<Vector2>();
        for(int i=0;i<28;i++){float a=Mathf.PI*2f*i/28f;points.Add(center+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*diameter*.5f);}
        Polygon("Dot",parent,points,color);
    }

    private static void Line(string name, Transform parent, Vector2 a, Vector2 b, float width, Color color)
        => LineGraphic(name,parent,new[]{a,b},width,false,color);

    private static void LineLoop(string name, Transform parent, IList<Vector2> points, float width, Color color)
        => LineGraphic(name,parent,points,width,true,color);

    private static void LineGraphic(string name, Transform parent, IList<Vector2> points, float width, bool closed, Color color)
    {
        GameObject go=new GameObject(name,typeof(RectTransform)); go.transform.SetParent(parent,false); Stretch((RectTransform)go.transform);
        Dimension1CommandCenterLineGraphic line=go.AddComponent<Dimension1CommandCenterLineGraphic>();
        line.color=color; line.raycastTarget=false; line.SetLine(points,width,closed);
    }

    private static void Polygon(string name, Transform parent, IList<Vector2> points, Color color)
    {
        GameObject go=new GameObject(name,typeof(RectTransform)); go.transform.SetParent(parent,false); Stretch((RectTransform)go.transform);
        Dimension1CommandCenterPolygonGraphic polygon=go.AddComponent<Dimension1CommandCenterPolygonGraphic>();
        polygon.color=color; polygon.raycastTarget=false; polygon.SetPolygon(points);
    }

    private static RectTransform Rect(string name, Transform parent)
    {
        GameObject go=new GameObject(name,typeof(RectTransform)); go.transform.SetParent(parent,false); return (RectTransform)go.transform;
    }

    private static Image Image(string name, Transform parent, Sprite sprite, Color color)
    {
        RectTransform rect=Rect(name,parent); Image image=rect.gameObject.AddComponent<Image>(); image.sprite=sprite; image.color=color; return image;
    }

    private static TMP_Text Text(string name, Transform parent, TMP_FontAsset font, string value, float size, FontStyles style, Color color)
    {
        RectTransform rect=Rect(name,parent); TextMeshProUGUI text=rect.gameObject.AddComponent<TextMeshProUGUI>();
        text.font=font; text.text=value; text.fontSize=size; text.fontStyle=style; text.color=color;
        text.textWrappingMode = TextWrappingModes.NoWrap; text.overflowMode=TextOverflowModes.Truncate; text.raycastTarget=false; return text;
    }

    private static void Top(RectTransform rect,float x,float y,float width,float height)
    {
        rect.anchorMin=rect.anchorMax=new Vector2(0f,1f); rect.pivot=new Vector2(0f,1f);
        rect.anchoredPosition=new Vector2(x,-y); rect.sizeDelta=new Vector2(width,height);
    }

    private static void Centered(RectTransform rect,Vector2 position,Vector2 size)
    {
        rect.anchorMin=rect.anchorMax=new Vector2(.5f,.5f); rect.pivot=new Vector2(.5f,.5f);
        rect.anchoredPosition=position; rect.sizeDelta=size;
    }

    private static void Stretch(RectTransform rect,Vector2 min=default,Vector2 max=default)
    {
        rect.anchorMin=Vector2.zero; rect.anchorMax=Vector2.one; rect.pivot=new Vector2(.5f,.5f);
        rect.offsetMin=min; rect.offsetMax=new Vector2(-max.x,-max.y);
    }

    private static Vector2 Local(RectTransform rect,float x,float y) =>
        new Vector2(x-rect.rect.width*.5f,rect.rect.height*.5f-y);

    private static TMP_FontAsset FindFont(Transform root)
    {
        foreach(TMP_Text text in root.GetComponentsInChildren<TMP_Text>(true))
            if(text!=null&&text.font!=null&&text.font.name.ToLowerInvariant().Contains("rajdhani")) return text.font;
        foreach(TMP_Text text in root.GetComponentsInChildren<TMP_Text>(true)) if(text!=null&&text.font!=null) return text.font;
        return TMP_Settings.defaultFontAsset;
    }

    private static Sprite LoadSprite(string path)=>AssetDatabase.LoadAssetAtPath<Sprite>(path);

    private static void ConfigureSingleSprite(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) throw new InvalidOperationException("No se pudo importar como sprite: " + path);
        bool changed = importer.textureType != TextureImporterType.Sprite ||
                       importer.spriteImportMode != SpriteImportMode.Single ||
                       importer.mipmapEnabled || !importer.alphaIsTransparency ||
                       importer.textureCompression != TextureImporterCompression.Uncompressed;
        if (!changed) return;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.maxTextureSize = 2048;
        importer.SaveAndReimport();
    }

    private static T FindSceneComponent<T>(Scene scene) where T:Component
    {
        foreach(GameObject root in scene.GetRootGameObjects()){T result=root.GetComponentInChildren<T>(true);if(result!=null)return result;} return null;
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

    private static void Assign(SerializedObject serializedObject, string propertyName,
        UnityEngine.Object value)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property == null) throw new InvalidOperationException("No existe propiedad serializada: " + propertyName);
        property.objectReferenceValue = value;
    }

    private static void SetObjectArray<T>(SerializedObject serializedObject, string propertyName,
        T[] values) where T : UnityEngine.Object
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property == null) throw new InvalidOperationException("No existe arreglo serializado: " + propertyName);
        property.arraySize = values == null ? 0 : values.Length;
        for (int i = 0; values != null && i < values.Length; i++)
            property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
    }

    private static void AddPersistent(UnityEvent evt, UnityAction action) =>
        UnityEventTools.AddPersistentListener(evt, action);

    private static Transform FindDirectChild(Transform parent,string name)
    {
        if(parent==null)return null; for(int i=0;i<parent.childCount;i++) if(parent.GetChild(i).name==name)return parent.GetChild(i); return null;
    }

    private static Transform FindChild(Transform parent,string name)
    {
        if(parent==null)return null; foreach(Transform child in parent.GetComponentsInChildren<Transform>(true)) if(child.name==name)return child; return null;
    }

    private static void ValidateInternal(Scene scene)
    {
        Transform root=null;
        foreach(GameObject sceneRoot in scene.GetRootGameObjects()){root=FindChild(sceneRoot.transform,RootName);if(root!=null)break;}
        if(root==null)throw new InvalidOperationException("Falta "+RootName+".");
        if(root.GetComponent<Canvas>()==null||root.GetComponent<CanvasGroup>()==null||
           root.GetComponent<GraphicRaycaster>()==null||root.GetComponent<Dimension1TreeNavigationUI>()==null||
           root.GetComponent<Dimension1TreeVisualUI>()==null)
            throw new InvalidOperationException("Falta la capa interactiva del Árbol Cuántico.");
        string[] required={"DimensionTitle","MainHeading","AvailablePoints","OrbitalConstellation","TreeNode_1","TreeNode_10","SelectedNodeDetail","UnlockButton","BottomNavigation"};
        foreach(string name in required)if(FindChild(root,name)==null)throw new InvalidOperationException("Falta bloque visual: "+name);
        TMP_Text cost=FindChild(FindChild(root,"CostPanel"),"Value")?.GetComponent<TMP_Text>();
        if(cost==null||cost.text!="3 PUNTOS")throw new InvalidOperationException("El costo de Cartografía Avanzada no coincide con los datos reales.");
        for(int i=1;i<=Dimension1System.Dimension1TreeNodeIds.Length;i++)
        {
            Button nodeButton=FindChild(root,"TreeNode_"+i)?.GetComponent<Button>();
            if(nodeButton==null||nodeButton.onClick.GetPersistentEventCount()!=1)
                throw new InvalidOperationException("El nodo "+i+" no tiene una única ruta persistente de selección.");
        }
        Button unlock=FindChild(root,"UnlockButton")?.GetComponent<Button>();
        if(unlock==null||unlock.onClick.GetPersistentEventCount()!=1)
            throw new InvalidOperationException("El botón de compra no tiene una única ruta persistente.");
        Transform legacy=FindChild(root.parent,LegacyName);
        if(legacy==null||legacy.gameObject.activeSelf)throw new InvalidOperationException("Los controles provisionales no quedaron preservados y ocultos.");
    }

    private static Color WithAlpha(Color color,byte alpha){color.a=alpha/255f;return color;}
    private static Color Hex(string value,byte alpha=255){ColorUtility.TryParseHtmlString("#"+value,out Color color);color.a=alpha/255f;return color;}
}
#endif

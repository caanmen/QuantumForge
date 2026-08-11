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

public static class Dimension1GalaxyVisualSetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string GeneratedPath = "Assets/Project/UI/Dimension1/Generated";
    private const string RootName = "D1_GalaxyVisualRoot";

    private static readonly Color Void = Hex("030711");
    private static readonly Color Surface = Hex("0B121C", 244);
    private static readonly Color SurfaceRaised = Hex("111D29", 248);
    private static readonly Color Border = Hex("3C5261", 225);
    private static readonly Color Cyan = Hex("4FD9E8");
    private static readonly Color Amber = Hex("F2A64A");
    private static readonly Color Violet = Hex("A875FF");
    private static readonly Color Primary = Hex("EAF4F8");
    private static readonly Color Secondary = Hex("9FB5C2");

    [MenuItem("Quantum Forge/Dimension 1/Configure Galaxy Visual V2")]
    public static void Configure()
    {
        EnsureGeneratedArt();

        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Dimension1PanelUI panel = FindSceneComponent<Dimension1PanelUI>(scene);
        if (panel == null)
            throw new InvalidOperationException("Dimension1PanelUI no existe en Main.unity.");

        Transform galaxyPanel = FindChild(panel.transform, "GalaxyPanel");
        if (galaxyPanel == null)
            throw new InvalidOperationException("GalaxyPanel no existe en Main.unity.");

        RectTransform galaxyPanelRect = galaxyPanel as RectTransform;
        if (galaxyPanelRect == null)
            throw new InvalidOperationException("GalaxyPanel no posee RectTransform.");
        Stretch(galaxyPanelRect);
        galaxyPanelRect.localScale = Vector3.one;
        galaxyPanelRect.localRotation = Quaternion.identity;

        Transform oldGenerated = FindDirectChild(galaxyPanel, RootName);
        if (oldGenerated != null)
            UnityEngine.Object.DestroyImmediate(oldGenerated.gameObject);

        foreach (Transform child in galaxyPanel)
            child.gameObject.SetActive(false);

        Sprite frame = AssetDatabase.LoadAssetAtPath<Sprite>(GeneratedPath + "/d1_frame.png");
        Sprite starfield = AssetDatabase.LoadAssetAtPath<Sprite>(GeneratedPath + "/d1_starfield.png");
        Sprite planet1 = AssetDatabase.LoadAssetAtPath<Sprite>(GeneratedPath + "/d1_planet_1.png");
        Sprite planet2 = AssetDatabase.LoadAssetAtPath<Sprite>(GeneratedPath + "/d1_planet_2.png");
        Sprite planet3 = AssetDatabase.LoadAssetAtPath<Sprite>(GeneratedPath + "/d1_planet_3.png");
        Sprite planet4 = AssetDatabase.LoadAssetAtPath<Sprite>(GeneratedPath + "/d1_planet_4.png");
        Sprite blackHole = AssetDatabase.LoadAssetAtPath<Sprite>(GeneratedPath + "/d1_black_hole.png");
        Sprite asteroid = AssetDatabase.LoadAssetAtPath<Sprite>(GeneratedPath + "/d1_asteroid.png");

        TMP_FontAsset font = FindFont(panel.transform);
        RectTransform root = CreateRect(RootName, galaxyPanel, Vector2.zero, new Vector2(1080f, 1920f));
        Stretch(root);
        root.gameObject.AddComponent<Dimension1VisualSkinRoot>();
        Dimension1GalaxyVisualUI visual = root.gameObject.AddComponent<Dimension1GalaxyVisualUI>();

        Image background = CreateImage("Starfield", root, starfield, Color.white);
        Stretch(background.rectTransform, new Vector2(-18f, -18f), new Vector2(18f, 18f));
        background.raycastTarget = false;

        Image shade = CreateImage("VoidShade", root, null, Hex("02050B", 92));
        Stretch(shade.rectTransform);
        shade.raycastTarget = false;

        CreateCornerBrackets(root, frame);

        RectTransform header = CreatePanel("Header", root, frame, new Vector2(20f, -18f), new Vector2(1040f, 128f));
        TMP_Text visibleTitle = CreateText("VisibleTitle", header, font, "CARTA GALÁCTICA", 45f, FontStyles.Bold, Primary);
        SetTopRect(visibleTitle.rectTransform, 32f, 14f, 440f, 88f);
        visibleTitle.alignment = TextAlignmentOptions.MidlineLeft;

        TMP_Text currentSector = CreateText("CurrentSector", header, font, "SECTOR ACTUAL", 18f, FontStyles.Bold, Cyan);
        SetTopRect(currentSector.rectTransform, 500f, 20f, 330f, 42f);
        currentSector.alignment = TextAlignmentOptions.MidlineRight;

        TMP_Text unlockedSectors = CreateText("UnlockedSectors", header, font, "1/5 SECTORES", 18f, FontStyles.Bold, Amber);
        SetTopRect(unlockedSectors.rectTransform, 500f, 62f, 330f, 42f);
        unlockedSectors.alignment = TextAlignmentOptions.MidlineRight;

        Button close = CreateButton("CloseGalaxyV2", header, frame, font, "VOLVER", new Vector2(858f, -22f), new Vector2(150f, 78f), Cyan);
        AddPersistent(close.onClick, panel.OnClickCloseGalaxyPanel);

        TMP_Text titleProxy = CreateText("GalaxyTitleProxy", root, font, "", 1f, FontStyles.Normal, Color.clear);
        SetTopRect(titleProxy.rectTransform, 0f, 0f, 1f, 1f);

        RectTransform resources = CreatePanel("MetalsRibbon", root, frame, new Vector2(20f, -158f), new Vector2(1040f, 116f));
        TMP_Text metalsLabel = CreateText("MetalsLabel", resources, font, "METALES DESBLOQUEADOS", 16f, FontStyles.Bold, Secondary);
        SetTopRect(metalsLabel.rectTransform, 22f, 8f, 260f, 30f);

        var chips = new Dimension1GalaxyVisualUI.MetalChip[3];
        for (int i = 0; i < chips.Length; i++)
            chips[i] = CreateMetalChip(resources, frame, font, i, 286f + i * 244f);

        RectTransform mapPanel = CreatePanel("SectorMap", root, frame, new Vector2(20f, -288f), new Vector2(1040f, 850f));
        TMP_Text mapLabel = CreateText("MapLabel", mapPanel, font, "RUTA DE EXPEDICIÓN", 18f, FontStyles.Bold, Secondary);
        SetTopRect(mapLabel.rectTransform, 26f, 14f, 320f, 38f);

        RectTransform[] asteroids = CreateAsteroidField(mapPanel, asteroid);

        Vector2 s1 = new Vector2(215f, -190f);
        Vector2 s2 = new Vector2(805f, -205f);
        Vector2 center = new Vector2(510f, -410f);
        Vector2 s3 = new Vector2(230f, -645f);
        Vector2 s4 = new Vector2(800f, -650f);

        var routes = new List<Image>
        {
            CreateRoute("RouteS1", mapPanel, s1, center, Cyan),
            CreateRoute("RouteS2", mapPanel, s2, center, Cyan),
            CreateRoute("RouteS3", mapPanel, s3, center, Violet),
            CreateRoute("RouteS4", mapPanel, s4, center, Hex("667586"))
        };

        Button sector1 = CreateSectorButton("GalaxySector1ButtonV2", mapPanel, frame, planet1, font, s1, Cyan);
        Button sector2 = CreateSectorButton("GalaxySector2ButtonV2", mapPanel, frame, planet2, font, s2, Cyan);
        Button sector3 = CreateSectorButton("GalaxySector3ButtonV2", mapPanel, frame, planet3, font, s3, Violet);
        Button sector4 = CreateSectorButton("GalaxySector4ButtonV2", mapPanel, frame, planet4, font, s4, Hex("667586"));
        Button galacticCenter = CreateSectorButton("GalaxyCenterButtonV2", mapPanel, frame, blackHole, font, center, Amber);

        AddPersistent(sector1.onClick, panel.OnClickPreviewGalaxySector1);
        AddPersistent(sector2.onClick, panel.OnClickPreviewGalaxySector2);
        AddPersistent(sector3.onClick, panel.OnClickPreviewGalaxySector3);
        AddPersistent(sector4.onClick, panel.OnClickPreviewGalaxySector4);
        AddPersistent(galacticCenter.onClick, panel.OnClickPreviewGalaxyCenter);

        RectTransform details = CreatePanel("SectorDetails", root, frame, new Vector2(20f, -1152f), new Vector2(1040f, 500f));
        TMP_Text detailsLabel = CreateText("DetailsLabel", details, font, "INFORME DEL SECTOR", 20f, FontStyles.Bold, Amber);
        SetTopRect(detailsLabel.rectTransform, 28f, 14f, 400f, 42f);

        RectTransform viewport = CreateRect("SummaryViewport", details, new Vector2(24f, -62f), new Vector2(992f, 318f));
        Image viewportImage = viewport.gameObject.AddComponent<Image>();
        viewportImage.color = Hex("07101A", 210);
        viewportImage.raycastTarget = true;
        Mask mask = viewport.gameObject.AddComponent<Mask>();
        mask.showMaskGraphic = true;

        TMP_Text summary = CreateText("GalaxySectorSummaryTextV2", viewport, font, "Selecciona un sector", 21f, FontStyles.Normal, Primary);
        summary.rectTransform.anchorMin = new Vector2(0f, 1f);
        summary.rectTransform.anchorMax = new Vector2(1f, 1f);
        summary.rectTransform.pivot = new Vector2(0.5f, 1f);
        summary.rectTransform.anchoredPosition = new Vector2(0f, -14f);
        summary.rectTransform.sizeDelta = new Vector2(-34f, 300f);
        summary.alignment = TextAlignmentOptions.TopLeft;
        summary.enableWordWrapping = true;
        summary.overflowMode = TextOverflowModes.Overflow;
        ContentSizeFitter summaryFitter = summary.gameObject.AddComponent<ContentSizeFitter>();
        summaryFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        summaryFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        ScrollRect scroll = details.gameObject.AddComponent<ScrollRect>();
        scroll.viewport = viewport;
        scroll.content = summary.rectTransform;
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.scrollSensitivity = 24f;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.verticalNormalizedPosition = 1f;

        Button enter = CreateButton("EnterGalaxySectorButtonV2", details, frame, font, "ENTRAR", new Vector2(250f, -396f), new Vector2(540f, 82f), Amber);
        AddPersistent(enter.onClick, panel.OnClickEnterGalaxySector);

        RectTransform footer = CreatePanel("FooterHint", root, frame, new Vector2(20f, -1668f), new Vector2(1040f, 92f));
        TMP_Text footerText = CreateText("FooterText", footer, font, "SELECCIONA UN NODO · REVISA SUS REQUISITOS · ENTRA AL SECTOR", 18f, FontStyles.Bold, Secondary);
        Stretch(footerText.rectTransform, new Vector2(22f, 8f), new Vector2(-22f, -8f));
        footerText.alignment = TextAlignmentOptions.Center;

        SerializedObject serializedPanel = new SerializedObject(panel);
        Assign(serializedPanel, "galaxyTitleText", titleProxy);
        Assign(serializedPanel, "galaxySectorSummaryText", summary);
        Assign(serializedPanel, "galaxySector1Button", sector1);
        Assign(serializedPanel, "galaxySector2Button", sector2);
        Assign(serializedPanel, "galaxySector3Button", sector3);
        Assign(serializedPanel, "galaxySector4Button", sector4);
        Assign(serializedPanel, "galaxyCenterButton", galacticCenter);
        Assign(serializedPanel, "enterGalaxySectorButton", enter);
        Assign(serializedPanel, "closeGalaxyPanelButton", close);
        serializedPanel.ApplyModifiedPropertiesWithoutUndo();

        SerializedObject serializedVisual = new SerializedObject(visual);
        Assign(serializedVisual, "currentSectorText", currentSector);
        Assign(serializedVisual, "unlockedSectorText", unlockedSectors);
        Assign(serializedVisual, "starLayer", background.rectTransform);
        SetObjectArray(serializedVisual, "routeLines", routes.ToArray());
        SetObjectArray(serializedVisual, "rotatingBodies", new UnityEngine.Object[]
        {
            FindChild(sector1.transform, "NodeRing").GetComponent<RectTransform>(),
            FindChild(sector2.transform, "NodeRing").GetComponent<RectTransform>(),
            FindChild(sector3.transform, "NodeRing").GetComponent<RectTransform>(),
            FindChild(sector4.transform, "NodeRing").GetComponent<RectTransform>(),
            FindChild(galacticCenter.transform, "NodeRing").GetComponent<RectTransform>()
        });
        SetObjectArray(serializedVisual, "driftingAsteroids", asteroids);
        SetMetalChipArray(serializedVisual, chips);
        serializedVisual.ApplyModifiedPropertiesWithoutUndo();

        root.gameObject.SetActive(true);
        EditorUtility.SetDirty(panel);
        EditorUtility.SetDirty(visual);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("Unity no pudo guardar Main.unity.");

        Validate(scene);
        Debug.Log("[D1 Galaxy Visual V2] CONFIGURATION_PASS | metales reales | 5 sectores | UI portrait");
    }

    [MenuItem("Quantum Forge/Dimension 1/Validate Galaxy Visual V2")]
    public static void ValidateMenu()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Validate(scene);
        Debug.Log("[D1 Galaxy Visual V2] VALIDATION_PASS");
    }

    private static void Validate(Scene scene)
    {
        Dimension1PanelUI panel = FindSceneComponent<Dimension1PanelUI>(scene);
        Dimension1GalaxyVisualUI visual = FindSceneComponent<Dimension1GalaxyVisualUI>(scene);
        if (panel == null || visual == null)
            throw new InvalidOperationException("La pantalla Galaxia V2 no está conectada.");

        Transform root = FindChild(panel.transform, RootName);
        if (root == null || root.GetComponent<Dimension1VisualSkinRoot>() == null)
            throw new InvalidOperationException("Falta la raíz visual propia de Dimensión 1.");

        SerializedObject serializedPanel = new SerializedObject(panel);
        string[] required =
        {
            "galaxySectorSummaryText", "galaxySector1Button", "galaxySector2Button",
            "galaxySector3Button", "galaxySector4Button", "galaxyCenterButton",
            "enterGalaxySectorButton", "closeGalaxyPanelButton"
        };
        foreach (string field in required)
        {
            SerializedProperty property = serializedPanel.FindProperty(field);
            if (property == null || property.objectReferenceValue == null)
                throw new InvalidOperationException("Conexión faltante: " + field);
        }

        string[] metals =
        {
            Dimension1System.MetalIron, Dimension1System.MetalCopper,
            Dimension1System.MetalAluminum, Dimension1System.MetalTitanium,
            Dimension1System.MetalNickel, Dimension1System.MetalCobalt,
            Dimension1System.MetalLithium, Dimension1System.MetalTungsten,
            Dimension1System.MetalPlatinum, Dimension1System.MetalIridium
        };
        if (metals.Length != 10 || Dimension1System.Dimension1SectorIds.Length != 5)
            throw new InvalidOperationException("Catálogo D1 inesperado.");
    }

    private static Dimension1GalaxyVisualUI.MetalChip CreateMetalChip(
        Transform parent, Sprite frame, TMP_FontAsset font, int index, float x)
    {
        RectTransform chip = CreatePanel("MetalChip_" + (index + 1), parent, frame, new Vector2(x, -14f), new Vector2(226f, 84f));
        Image marker = CreateImage("Marker", chip, null, Cyan);
        SetTopRect(marker.rectTransform, 12f, 12f, 8f, 60f);

        TMP_Text name = CreateText("Name", chip, font, "METAL", 15f, FontStyles.Bold, Secondary);
        SetTopRect(name.rectTransform, 30f, 8f, 112f, 30f);
        name.alignment = TextAlignmentOptions.MidlineLeft;

        TMP_Text amount = CreateText("Amount", chip, font, "0", 22f, FontStyles.Bold, Primary);
        SetTopRect(amount.rectTransform, 30f, 36f, 105f, 38f);
        amount.alignment = TextAlignmentOptions.MidlineLeft;

        TMP_Text rate = CreateText("Rate", chip, font, "+0/s", 16f, FontStyles.Bold, Cyan);
        SetTopRect(rate.rectTransform, 132f, 37f, 84f, 34f);
        rate.alignment = TextAlignmentOptions.MidlineRight;

        return new Dimension1GalaxyVisualUI.MetalChip
        {
            root = chip.gameObject,
            nameText = name,
            amountText = amount,
            rateText = rate,
            marker = marker
        };
    }

    private static Button CreateSectorButton(
        string name, Transform parent, Sprite frame, Sprite planet,
        TMP_FontAsset font, Vector2 position, Color accent)
    {
        RectTransform rect = CreateRect(name, parent, position, new Vector2(260f, 184f));
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        Image hit = rect.gameObject.AddComponent<Image>();
        hit.color = new Color(1f, 1f, 1f, 0.001f);
        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = hit;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = accent;
        colors.pressedColor = new Color(accent.r * 0.7f, accent.g * 0.7f, accent.b * 0.7f, 1f);
        colors.selectedColor = accent;
        colors.disabledColor = new Color(0.35f, 0.4f, 0.45f, 0.6f);
        button.colors = colors;

        Image ring = CreateImage("NodeRing", rect, planet, Color.white);
        ring.rectTransform.anchorMin = new Vector2(0.5f, 1f);
        ring.rectTransform.anchorMax = new Vector2(0.5f, 1f);
        ring.rectTransform.pivot = new Vector2(0.5f, 1f);
        ring.rectTransform.anchoredPosition = new Vector2(0f, 0f);
        ring.rectTransform.sizeDelta = new Vector2(116f, 116f);
        ring.raycastTarget = false;

        RectTransform plate = CreatePanel("LabelPlate", rect, frame, new Vector2(0f, -112f), new Vector2(260f, 72f));
        plate.anchorMin = new Vector2(0.5f, 1f);
        plate.anchorMax = new Vector2(0.5f, 1f);
        plate.pivot = new Vector2(0.5f, 1f);

        TMP_Text label = CreateText("Label", plate, font, "SECTOR", 17f, FontStyles.Bold, Primary);
        Stretch(label.rectTransform, new Vector2(10f, 6f), new Vector2(-10f, -6f));
        label.alignment = TextAlignmentOptions.Center;
        label.enableAutoSizing = true;
        label.fontSizeMin = 12f;
        label.fontSizeMax = 17f;
        label.raycastTarget = false;

        return button;
    }

    private static Image CreateRoute(string name, Transform parent, Vector2 a, Vector2 b, Color color)
    {
        Vector2 delta = b - a;
        float length = delta.magnitude - 100f;
        Vector2 midpoint = (a + b) * 0.5f;
        RectTransform rect = CreateRect(name, parent, midpoint, new Vector2(Mathf.Max(30f, length), 7f));
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
        Image image = rect.gameObject.AddComponent<Image>();
        color.a = 0.45f;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static void CreateCornerBrackets(Transform root, Sprite frame)
    {
        Color color = Hex("243746", 190);
        Image top = CreateImage("TopTechnicalRail", root, frame, color);
        SetTopRect(top.rectTransform, 12f, 4f, 1056f, 12f);
        Image left = CreateImage("LeftTechnicalRail", root, frame, color);
        SetTopRect(left.rectTransform, 5f, 20f, 10f, 1735f);
        Image right = CreateImage("RightTechnicalRail", root, frame, color);
        SetTopRect(right.rectTransform, 1065f, 20f, 10f, 1735f);
    }

    private static RectTransform[] CreateAsteroidField(Transform parent, Sprite sprite)
    {
        Vector2[] positions =
        {
            new Vector2(80f, -110f), new Vector2(440f, -130f),
            new Vector2(930f, -125f), new Vector2(105f, -420f),
            new Vector2(890f, -430f), new Vector2(455f, -690f),
            new Vector2(620f, -770f), new Vector2(955f, -720f)
        };
        RectTransform[] result = new RectTransform[positions.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            Image image = CreateImage("Asteroid_" + (i + 1), parent, sprite, Hex("81909B", (byte)(85 + i * 8)));
            image.raycastTarget = false;
            RectTransform rect = image.rectTransform;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = positions[i];
            float size = 17f + (i % 3) * 8f;
            rect.sizeDelta = new Vector2(size, size);
            rect.localRotation = Quaternion.Euler(0f, 0f, i * 31f);
            result[i] = rect;
        }
        return result;
    }

    private static RectTransform CreatePanel(string name, Transform parent, Sprite frame, Vector2 topLeft, Vector2 size)
    {
        RectTransform outer = CreateRect(name, parent, topLeft, size);
        SetTopLeft(outer, topLeft, size);
        Image border = outer.gameObject.AddComponent<Image>();
        border.sprite = frame;
        border.type = Image.Type.Sliced;
        border.color = Border;

        Image inner = CreateImage("Inner", outer, frame, Surface);
        inner.type = Image.Type.Sliced;
        Stretch(inner.rectTransform, new Vector2(4f, 4f), new Vector2(-4f, -4f));
        inner.raycastTarget = false;
        return outer;
    }

    private static Button CreateButton(
        string name, Transform parent, Sprite frame, TMP_FontAsset font,
        string labelText, Vector2 topLeft, Vector2 size, Color accent)
    {
        RectTransform rect = CreateRect(name, parent, topLeft, size);
        SetTopLeft(rect, topLeft, size);
        Image image = rect.gameObject.AddComponent<Image>();
        image.sprite = frame;
        image.type = Image.Type.Sliced;
        image.color = SurfaceRaised;
        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.normalColor = SurfaceRaised;
        colors.highlightedColor = new Color(accent.r * 0.45f, accent.g * 0.45f, accent.b * 0.45f, 1f);
        colors.pressedColor = new Color(accent.r * 0.25f, accent.g * 0.25f, accent.b * 0.25f, 1f);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = Hex("1B252E", 205);
        button.colors = colors;

        Image edge = CreateImage("Accent", rect, null, accent);
        SetTopRect(edge.rectTransform, 9f, 9f, 5f, size.y - 18f);
        edge.raycastTarget = false;

        TMP_Text text = CreateText("Label", rect, font, labelText, 21f, FontStyles.Bold, Primary);
        Stretch(text.rectTransform, new Vector2(16f, 8f), new Vector2(-12f, -8f));
        text.alignment = TextAlignmentOptions.Center;
        text.enableAutoSizing = true;
        text.fontSizeMin = 14f;
        text.fontSizeMax = 21f;
        text.raycastTarget = false;
        return button;
    }

    private static TMP_Text CreateText(
        string name, Transform parent, TMP_FontAsset font, string value,
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

    private static void SetTopRect(RectTransform rect, float x, float y, float width, float height)
    {
        SetTopLeft(rect, new Vector2(x, -y), new Vector2(width, height));
    }

    private static void Stretch(RectTransform rect)
    {
        Stretch(rect, Vector2.zero, Vector2.zero);
    }

    private static void Stretch(RectTransform rect, Vector2 minOffset, Vector2 maxOffset)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = minOffset;
        rect.offsetMax = maxOffset;
    }

    private static void AddPersistent(Button.ButtonClickedEvent click, UnityAction action)
    {
        click.RemoveAllListeners();
        UnityEventTools.AddPersistentListener(click, action);
    }

    private static void Assign(SerializedObject target, string propertyName, UnityEngine.Object value)
    {
        SerializedProperty property = target.FindProperty(propertyName);
        if (property == null)
            throw new InvalidOperationException("No existe el campo: " + propertyName);
        property.objectReferenceValue = value;
    }

    private static void SetObjectArray(SerializedObject target, string propertyName, UnityEngine.Object[] values)
    {
        SerializedProperty property = target.FindProperty(propertyName);
        property.arraySize = values.Length;
        for (int i = 0; i < values.Length; i++)
            property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
    }

    private static void SetMetalChipArray(
        SerializedObject target, Dimension1GalaxyVisualUI.MetalChip[] chips)
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

    private static TMP_FontAsset FindFont(Transform root)
    {
        TMP_Text text = root.GetComponentInChildren<TMP_Text>(true);
        if (text != null && text.font != null) return text.font;
        if (TMP_Settings.defaultFontAsset != null) return TMP_Settings.defaultFontAsset;
        throw new InvalidOperationException("No hay TMP Font Asset disponible.");
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

    private static Transform FindDirectChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
            if (child.name == name) return child;
        return null;
    }

    private static Transform FindChild(Transform parent, string name)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
            if (child.name == name) return child;
        return null;
    }

    private static void EnsureGeneratedArt()
    {
        Directory.CreateDirectory(GeneratedPath);
        GenerateFrame(GeneratedPath + "/d1_frame.png");
        GenerateStarfield(GeneratedPath + "/d1_starfield.png");
        GeneratePlanet(GeneratedPath + "/d1_planet_1.png", 11, Hex("315B70"), Hex("8AC4D4"), false);
        GeneratePlanet(GeneratedPath + "/d1_planet_2.png", 29, Hex("4D362C"), Hex("D78B4D"), true);
        GeneratePlanet(GeneratedPath + "/d1_planet_3.png", 47, Hex("392D51"), Hex("B596D7"), false);
        GeneratePlanet(GeneratedPath + "/d1_planet_4.png", 71, Hex("202737"), Hex("667B92"), true);
        GenerateBlackHole(GeneratedPath + "/d1_black_hole.png");
        GenerateAsteroid(GeneratedPath + "/d1_asteroid.png");
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        ConfigureSprite(GeneratedPath + "/d1_frame.png", new Vector4(18f, 18f, 18f, 18f));
        ConfigureSprite(GeneratedPath + "/d1_starfield.png", Vector4.zero);
        ConfigureSprite(GeneratedPath + "/d1_planet_1.png", Vector4.zero);
        ConfigureSprite(GeneratedPath + "/d1_planet_2.png", Vector4.zero);
        ConfigureSprite(GeneratedPath + "/d1_planet_3.png", Vector4.zero);
        ConfigureSprite(GeneratedPath + "/d1_planet_4.png", Vector4.zero);
        ConfigureSprite(GeneratedPath + "/d1_black_hole.png", Vector4.zero);
        ConfigureSprite(GeneratedPath + "/d1_asteroid.png", Vector4.zero);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }

    private static void ConfigureSprite(string path, Vector4 border)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Bilinear;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        importer.spriteBorder = border;
        importer.SaveAndReimport();
    }

    private static void GenerateFrame(string path)
    {
        const int size = 96;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int edge = Mathf.Min(Mathf.Min(x, size - 1 - x), Mathf.Min(y, size - 1 - y));
                bool cut = (x + y < 14) || ((size - 1 - x) + y < 14) ||
                           (x + (size - 1 - y) < 14) ||
                           ((size - 1 - x) + (size - 1 - y) < 14);
                float alpha = cut ? 0f : edge < 3 ? 0.94f : 1f;
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }
        SaveTexture(texture, path);
    }

    private static void GenerateStarfield(string path)
    {
        const int width = 540;
        const int height = 960;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        var random = new System.Random(8011);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float nx = (x - width * 0.52f) / width;
                float ny = (y - height * 0.54f) / height;
                float band = Mathf.Exp(-Mathf.Pow((ny + nx * 0.34f) * 7f, 2f));
                float noise = (float)random.NextDouble();
                Color baseColor = Color.Lerp(Hex("02050B"), Hex("101B2B"), band * (0.22f + noise * 0.10f));
                texture.SetPixel(x, y, baseColor);
            }
        }

        for (int i = 0; i < 440; i++)
        {
            int x = random.Next(2, width - 2);
            int y = random.Next(2, height - 2);
            int radius = random.NextDouble() > 0.91 ? 2 : 1;
            Color star = random.NextDouble() > 0.82 ? Hex("8FDDE8") : Hex("D9E8F1");
            for (int oy = -radius; oy <= radius; oy++)
                for (int ox = -radius; ox <= radius; ox++)
                    if (ox * ox + oy * oy <= radius * radius)
                        texture.SetPixel(x + ox, y + oy, star);
        }
        SaveTexture(texture, path);
    }

    private static void GeneratePlanet(string path, int seed, Color dark, Color light, bool ring)
    {
        const int size = 256;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var random = new System.Random(seed);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = (x + 0.5f - size * 0.5f) / (size * 0.5f);
                float dy = (y + 0.5f - size * 0.5f) / (size * 0.5f);
                float distance = Mathf.Sqrt(dx * dx + dy * dy);
                Color color = Color.clear;

                if (ring && Mathf.Abs(dy * 1.9f + dx * 0.2f) < 0.045f && distance < 0.98f && distance > 0.55f)
                    color = new Color(light.r, light.g, light.b, 0.75f);

                float sphere = Mathf.Sqrt(dx * dx + dy * dy);
                if (sphere <= 0.68f)
                {
                    float normalX = dx / 0.68f;
                    float normalY = dy / 0.68f;
                    float z = Mathf.Sqrt(Mathf.Max(0f, 1f - normalX * normalX - normalY * normalY));
                    float lighting = Mathf.Clamp01(normalX * -0.65f + normalY * 0.35f + z * 0.85f);
                    float bands = 0.5f + 0.5f * Mathf.Sin(normalY * 22f + Mathf.Sin(normalX * 8f) * 2f);
                    float noise = (float)random.NextDouble() * 0.12f;
                    color = Color.Lerp(dark, light, Mathf.Clamp01(lighting * 0.75f + bands * 0.16f + noise));
                    color.a = 1f;
                }
                else if (sphere < 0.73f)
                {
                    float alpha = Mathf.InverseLerp(0.73f, 0.68f, sphere);
                    color = new Color(light.r, light.g, light.b, alpha * 0.5f);
                }

                texture.SetPixel(x, y, color);
            }
        }
        SaveTexture(texture, path);
    }

    private static void GenerateBlackHole(string path)
    {
        const int size = 256;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = (x + 0.5f - size * 0.5f) / (size * 0.5f);
                float dy = (y + 0.5f - size * 0.5f) / (size * 0.5f);
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                Color color = Color.clear;
                if (d < 0.33f) color = new Color(0f, 0f, 0f, 1f);
                else if (d < 0.62f)
                {
                    float ring = 1f - Mathf.Abs(d - 0.46f) / 0.16f;
                    color = Color.Lerp(Violet, Amber, Mathf.Clamp01((dx + 1f) * 0.5f));
                    color.a = Mathf.Clamp01(ring);
                }
                else if (d < 0.72f)
                    color = new Color(Violet.r, Violet.g, Violet.b, Mathf.InverseLerp(0.72f, 0.62f, d) * 0.25f);
                texture.SetPixel(x, y, color);
            }
        }
        SaveTexture(texture, path);
    }

    private static void GenerateAsteroid(string path)
    {
        const int size = 64;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2[] polygon =
        {
            new Vector2(0.50f, 0.08f), new Vector2(0.78f, 0.18f),
            new Vector2(0.92f, 0.46f), new Vector2(0.80f, 0.78f),
            new Vector2(0.56f, 0.91f), new Vector2(0.27f, 0.82f),
            new Vector2(0.08f, 0.55f), new Vector2(0.18f, 0.24f)
        };
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 point = new Vector2((x + 0.5f) / size, (y + 0.5f) / size);
                bool inside = false;
                for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
                {
                    Vector2 a = polygon[i];
                    Vector2 b = polygon[j];
                    if (((a.y > point.y) != (b.y > point.y)) &&
                        point.x < (b.x - a.x) * (point.y - a.y) / (b.y - a.y) + a.x)
                        inside = !inside;
                }
                if (!inside)
                {
                    texture.SetPixel(x, y, Color.clear);
                    continue;
                }
                float shade = Mathf.Clamp01(0.28f + (1f - point.x) * 0.52f + point.y * 0.16f);
                texture.SetPixel(x, y, Color.Lerp(Hex("242C33"), Hex("8D9AA3"), shade));
            }
        }
        SaveTexture(texture, path);
    }

    private static void SaveTexture(Texture2D texture, string path)
    {
        texture.Apply(false, false);
        File.WriteAllBytes(path, texture.EncodeToPNG());
        UnityEngine.Object.DestroyImmediate(texture);
    }

    private static Color Hex(string rgb, byte alpha = 255)
    {
        ColorUtility.TryParseHtmlString("#" + rgb, out Color color);
        color.a = alpha / 255f;
        return color;
    }
}
#endif

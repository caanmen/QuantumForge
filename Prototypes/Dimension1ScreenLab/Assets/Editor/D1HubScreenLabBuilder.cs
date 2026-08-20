#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class D1HubScreenLabBuilder
{
    private const int Width = 1080;
    private const int Height = 1920;
    private const string ScenePath = "Assets/Scenes/D1HubScreenPrototype.unity";
    private const string CapturePath = "Captures/D1_CentroDeMando_Unity_1080x1920.png";

    private static readonly Color Background = Hex("01070C");
    private static readonly Color PanelBase = Hex("031019", 0.98f);
    private static readonly Color PanelSoft = Hex("061722", 0.96f);
    private static readonly Color PanelInner = Hex("020C13", 0.94f);
    private static readonly Color Cyan = Hex("18C8FF");
    private static readonly Color CyanBright = Hex("91EEFF");
    private static readonly Color CyanMuted = Hex("087BA7");
    private static readonly Color CyanDark = Hex("06435D");
    private static readonly Color Steel = Hex("B8C4CC");
    private static readonly Color White = Hex("EDF2F5");
    private static readonly Color Amber = Hex("F5A719");

    private static Font font;
    private static Sprite frame;
    private static Sprite[] navIcons;
    private static Canvas canvas;
    private static Camera camera;
    private static D1HubPrototypeUI controller;

    public static void BuildAndCapture()
    {
        Directory.CreateDirectory("Assets/Scenes");
        Directory.CreateDirectory("Captures");
        ConfigureSpriteImports();

        font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/Rajdhani-Medium.ttf");
        frame = LoadSprite("Assets/Art/d1_premium_frame_v4.png");
        navIcons = new[]
        {
            LoadSprite("Assets/Art/d1_nav_galaxy_v3.png"),
            LoadSprite("Assets/Art/d1_nav_explore_v3.png"),
            LoadSprite("Assets/Art/d1_nav_hangar_v3.png"),
            LoadSprite("Assets/Art/d1_nav_relics_v3.png"),
            LoadSprite("Assets/Art/d1_nav_tree_v3.png")
        };

        if (font == null || frame == null)
            throw new InvalidOperationException("Faltan recursos del laboratorio de Dimensión 1.");

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        CreateCameraAndCanvas();

        GameObject root = Panel(
            "D1_CentroDeMando",
            canvas.transform,
            Vector2.zero,
            new Vector2(Width, Height),
            Background
        );
        controller = root.AddComponent<D1HubPrototypeUI>();

        CreateBackdrop(root.transform);
        CreateHeader(root.transform);
        CreateCommandBanner(root.transform);
        CreateDashboard(root.transform, out RectTransform crystal, out CanvasGroup glow);
        CreateMissionCards(root.transform);
        CreateBottomNavigation(root.transform);

        GameObject drawer = CreateMetalsDrawer(root.transform);
        drawer.SetActive(false);
        controller.Configure(drawer, crystal, glow);

        EditorSceneManager.SaveScene(scene, ScenePath);
        Canvas.ForceUpdateCanvases();
        RenderCapture(CapturePath);
        Debug.Log("[D1 Screen Lab] PASS | Centro de Mando fiel | 1080x1920");
    }

    private static void CreateCameraAndCanvas()
    {
        GameObject cameraObject = new GameObject("PrototypeCamera");
        camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Background;
        camera.orthographic = true;
        camera.orthographicSize = Height * 0.5f;
        camera.transform.position = new Vector3(0f, 0f, -10f);

        GameObject canvasObject = new GameObject("D1PrototypeCanvas");
        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = camera;
        canvas.planeDistance = 1f;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(Width, Height);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        canvasObject.AddComponent<GraphicRaycaster>();
    }

    private static void CreateBackdrop(Transform parent)
    {
        for (int x = 0; x <= Width; x += 90)
        {
            Line(
                "GridV",
                parent,
                new[]
                {
                    new Vector2(x - Width * 0.5f, -Height * 0.5f),
                    new Vector2(x - Width * 0.5f, Height * 0.5f)
                },
                1f,
                Hex("0B3D50", 0.08f)
            );
        }

        for (int y = 0; y <= Height; y += 90)
        {
            Line(
                "GridH",
                parent,
                new[]
                {
                    new Vector2(-Width * 0.5f, y - Height * 0.5f),
                    new Vector2(Width * 0.5f, y - Height * 0.5f)
                },
                1f,
                Hex("0B3D50", 0.065f)
            );
        }

        Line("TopLeftBracket", parent, new[]
        {
            new Vector2(-526, 882), new Vector2(-526, 922), new Vector2(-507, 942),
            new Vector2(-458, 942), new Vector2(-446, 949), new Vector2(-356, 949)
        }, 3f, CyanMuted);
        Line("TopRightBracket", parent, new[]
        {
            new Vector2(526, 882), new Vector2(526, 922), new Vector2(507, 942),
            new Vector2(458, 942), new Vector2(446, 949), new Vector2(356, 949)
        }, 3f, CyanMuted);
    }

    private static void CreateHeader(Transform parent)
    {
        Text("DimensionTitle", parent, "DIMENSIÓN 1", new Vector2(0, 908),
            new Vector2(500, 62), 48, White, TextAnchor.MiddleCenter, FontStyle.Bold, 4f);

        CreateTitleCircuit(parent, -1f);
        CreateTitleCircuit(parent, 1f);

        string[] names = { "HIERRO", "ALUMINIO", "NÍQUEL" };
        string[] values = { "5.98M", "4.73M", "4.70M" };
        string[] rates = { "+0.30/s", "+0.08/s", "+0.03/s" };
        float[] xs = { -405f, -137f, 132f };
        float[] widths = { 250f, 252f, 252f };

        for (int i = 0; i < names.Length; i++)
        {
            GameObject chip = FramedPanel(
                "Resource_" + names[i], parent, new Vector2(xs[i], 813),
                new Vector2(widths[i], 92), PanelSoft, CyanMuted, 0.8f
            );
            CreateMetalGlyph(chip.transform, new Vector2(-92, 1), 24, i);
            Text("Name", chip.transform, names[i], new Vector2(5, 20),
                new Vector2(120, 24), 18, Steel, TextAnchor.MiddleLeft, FontStyle.Normal, 1.2f);
            Text("Value", chip.transform, values[i], new Vector2(5, -17),
                new Vector2(126, 38), 30, White, TextAnchor.MiddleLeft, FontStyle.Normal, 0.7f);
            Text("Rate", chip.transform, rates[i], new Vector2(83, -17),
                new Vector2(76, 30), 16, Cyan, TextAnchor.MiddleRight, FontStyle.Normal, 0.4f);
        }

        GameObject metals = FramedPanel(
            "MetalsButton", parent, new Vector2(410, 813),
            new Vector2(244, 92), PanelSoft, CyanMuted, 0.8f
        );
        Button button = metals.AddComponent<Button>();
        button.targetGraphic = metals.GetComponent<Image>();
        UnityEventTools.AddPersistentListener(button.onClick, controller.ToggleMetalsDrawer);
        Text("Label", metals.transform, "10 METALES⌄", Vector2.zero,
            new Vector2(210, 46), 22, Cyan, TextAnchor.MiddleCenter, FontStyle.Normal, 1.1f);
    }

    private static void CreateTitleCircuit(Transform parent, float direction)
    {
        float s = direction;
        Line("TitleCircuit", parent, new[]
        {
            new Vector2(178 * s, 906), new Vector2(240 * s, 906),
            new Vector2(255 * s, 918), new Vector2(320 * s, 918),
            new Vector2(330 * s, 908), new Vector2(410 * s, 908)
        }, 2f, CyanMuted);
        Dot("TitleNode", parent, new Vector2(178 * s, 906), 5f, CyanMuted);
        Dot("TitleNode", parent, new Vector2(255 * s, 918), 5f, CyanMuted);
        Dot("TitleNode", parent, new Vector2(330 * s, 908), 4f, CyanMuted);
        Dot("TitleNode", parent, new Vector2(410 * s, 908), 5f, CyanMuted);
    }

    private static void CreateCommandBanner(Transform parent)
    {
        GameObject banner = FramedPanel(
            "CommandBanner", parent, new Vector2(0, 646),
            new Vector2(1044, 190), PanelBase, CyanMuted, 0.86f
        );

        GameObject emblem = Panel("CommandEmblem", banner.transform,
            new Vector2(-402, 0), new Vector2(145, 145), Color.clear);
        CreateHexagon(emblem.transform, Vector2.zero, 68, CyanMuted, 3f);
        CreateCommandGlyph(emblem.transform);

        Text("Title", banner.transform, "CENTRO DE MANDO", new Vector2(104, 27),
            new Vector2(720, 88), 54, White, TextAnchor.MiddleLeft, FontStyle.Normal, 3.1f);
        Text("Subtitle", banner.transform, "Tu base de operaciones en la Dimensión 1.",
            new Vector2(88, -39), new Vector2(740, 40), 25, Cyan,
            TextAnchor.MiddleLeft, FontStyle.Normal, 1.2f);
    }

    private static void CreateDashboard(
        Transform parent,
        out RectTransform crystal,
        out CanvasGroup glow)
    {
        GameObject dashboard = Panel("CommandDashboard", parent, new Vector2(0, 45),
            new Vector2(1080, 900), Color.clear);

        CreateSideCard(dashboard.transform, "SectorCard", "SECTOR ACTUAL", "ÓRBITAS\nANTIGUAS",
            new Vector2(-389, 303), Amber, IconKind.Radar);
        CreateSideCard(dashboard.transform, "ScannerCard", "ESCÁNER", "NIVEL 3/15",
            new Vector2(-389, 0), Cyan, IconKind.Scanner);
        CreateSideCard(dashboard.transform, "FleetCard", "FLOTA", "4 NAVES",
            new Vector2(-389, -303), Cyan, IconKind.Ship);

        CreateSideCard(dashboard.transform, "RelicsCard", "RELIQUIAS", "12/20",
            new Vector2(389, 303), Cyan, IconKind.Relic);
        CreateSideCard(dashboard.transform, "TreeCard", "PUNTOS DEL\nÁRBOL", "3",
            new Vector2(389, 0), Cyan, IconKind.Tree);
        CreateSideCard(dashboard.transform, "ExpeditionsCard", "EXPEDICIONES\nACTIVAS", "1",
            new Vector2(389, -303), Cyan, IconKind.Compass);

        GameObject hologram = Panel("CentralHologram", dashboard.transform,
            new Vector2(0, -4), new Vector2(500, 880), Color.clear);
        GameObject pulseRoot = new GameObject("CrystalPulseGlow", typeof(RectTransform), typeof(CanvasGroup));
        pulseRoot.transform.SetParent(hologram.transform, false);
        RectTransform pulseRect = pulseRoot.GetComponent<RectTransform>();
        pulseRect.sizeDelta = new Vector2(220, 540);
        pulseRect.anchoredPosition = new Vector2(0, 28);
        glow = pulseRoot.GetComponent<CanvasGroup>();
        glow.alpha = 0.42f;
        CreateHolographicInstrument(hologram.transform);
        crystal = CreateCrystal(hologram.transform, new Vector2(0, 28));

        CreateCircuitConnection(dashboard.transform, new Vector2(-244, 303), new Vector2(-177, 245));
        CreateCircuitConnection(dashboard.transform, new Vector2(-244, 0), new Vector2(-127, -18));
        CreateCircuitConnection(dashboard.transform, new Vector2(-244, -303), new Vector2(-164, -300));
        CreateCircuitConnection(dashboard.transform, new Vector2(244, 303), new Vector2(177, 245));
        CreateCircuitConnection(dashboard.transform, new Vector2(244, 0), new Vector2(127, -18));
        CreateCircuitConnection(dashboard.transform, new Vector2(244, -303), new Vector2(164, -300));
    }

    private static void CreateSideCard(
        Transform parent,
        string name,
        string label,
        string value,
        Vector2 position,
        Color valueColor,
        IconKind iconKind)
    {
        GameObject card = FramedPanel(name, parent, position,
            new Vector2(286, 284), PanelInner, CyanMuted, 0.76f);
        Button button = card.AddComponent<Button>();
        button.targetGraphic = card.GetComponent<Image>();
        UnityEventTools.AddStringPersistentListener(button.onClick, controller.SelectModule, label);

        CreateCardIcon(card.transform, iconKind, new Vector2(0, 69));
        Text("Label", card.transform, label, new Vector2(0, -24),
            new Vector2(238, 62), 26, Steel, TextAnchor.MiddleCenter, FontStyle.Normal, 1.4f);
        Text("Value", card.transform, value, new Vector2(0, -88),
            new Vector2(244, 88), 29, valueColor, TextAnchor.MiddleCenter, FontStyle.Normal, 1.6f);

        Dot("CornerMark", card.transform, new Vector2(-108, 116), 2.5f, CyanMuted);
        Dot("CornerMark", card.transform, new Vector2(108, -116), 2.5f, CyanMuted);
    }

    private static void CreateHolographicInstrument(Transform parent)
    {
        Vector2 center = new Vector2(0, 40);
        Circle("OrbitOuter", parent, center, 226, 62, 2f, Hex("0ABFF5", 0.44f));
        Circle("OrbitMid", parent, center, 190, 56, 2f, Hex("0ABFF5", 0.48f));
        Circle("OrbitInner", parent, center, 150, 48, 2f, Hex("0ABFF5", 0.34f));
        Circle("OrbitDotted", parent, center, 118, 42, 3f, Hex("3AD6FF", 0.28f), true);

        for (int i = 0; i < 24; i++)
        {
            float angle = i * Mathf.PI * 2f / 24f;
            Vector2 inner = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 207f;
            Vector2 outer = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * (i % 3 == 0 ? 228f : 216f);
            Line("RadialTick", parent, new[] { inner, outer }, i % 3 == 0 ? 2f : 1f,
                Hex("22C8F5", i % 3 == 0 ? 0.5f : 0.25f));
        }

        Ellipse("PedestalOuter", parent, new Vector2(0, -307), 198, 68, 60, 2.2f, Hex("18C8FF", 0.70f));
        Ellipse("PedestalMid", parent, new Vector2(0, -307), 150, 50, 56, 2f, Hex("18C8FF", 0.75f));
        Ellipse("PedestalInner", parent, new Vector2(0, -307), 93, 31, 48, 2f, Hex("63E8FF", 0.82f));
        Ellipse("PedestalCore", parent, new Vector2(0, -307), 35, 13, 36, 3f, CyanBright);
        Line("EnergyBeam", parent, new[] { new Vector2(0, -310), new Vector2(0, 328) },
            4f, Hex("51DFFF", 0.72f));

        for (int i = 0; i < 68; i++)
        {
            float a = i * 2.39996323f;
            float r = 28f + (i * 31 % 165);
            Vector2 p = center + new Vector2(Mathf.Cos(a) * r, Mathf.Sin(a) * r * 1.18f);
            float radius = i % 9 == 0 ? 2.5f : 1.35f;
            Dot("StarParticle", parent, p, radius, Hex("38D6FF", i % 4 == 0 ? 0.8f : 0.48f));
        }
    }

    private static RectTransform CreateCrystal(Transform parent, Vector2 center)
    {
        GameObject root = Panel("QuantumCrystal", parent, center,
            new Vector2(220, 520), Color.clear);

        root.GetComponent<Image>().color = Color.clear;

        Vector2[] outer =
        {
            new Vector2(0, 242), new Vector2(62, 123), new Vector2(58, -122),
            new Vector2(0, -242), new Vector2(-58, -122), new Vector2(-62, 123)
        };
        Polygon("FacetFillTopLeft", root.transform, new[]
        {
            outer[0], outer[5], new Vector2(0, 65)
        }, Hex("0586C4", 0.48f));
        Polygon("FacetFillTopRight", root.transform, new[]
        {
            outer[0], new Vector2(0, 65), outer[1]
        }, Hex("00B9F2", 0.34f));
        Polygon("FacetFillMidLeft", root.transform, new[]
        {
            outer[5], new Vector2(0, 65), new Vector2(0, -24), outer[4]
        }, Hex("006FAE", 0.30f));
        Polygon("FacetFillMidRight", root.transform, new[]
        {
            new Vector2(0, 65), outer[1], outer[2], new Vector2(0, -24)
        }, Hex("009DDB", 0.36f));
        Polygon("FacetFillBottomLeft", root.transform, new[]
        {
            outer[4], new Vector2(0, -24), outer[3]
        }, Hex("007FBC", 0.42f));
        Polygon("FacetFillBottomRight", root.transform, new[]
        {
            new Vector2(0, -24), outer[2], outer[3]
        }, Hex("00B6E9", 0.31f));
        Line("CrystalOuterGlow", root.transform, outer, 13f, Hex("19CAFF", 0.22f), true);
        Line("CrystalOuter", root.transform, outer, 5.5f, CyanBright, true);
        Line("CrystalSpine", root.transform, new[] { outer[0], new Vector2(0, -242) },
            3f, Hex("D5FCFF", 0.9f));
        Line("CrystalUpperFacet", root.transform, new[]
        {
            outer[5], new Vector2(0, 65), outer[1]
        }, 3.2f, CyanBright);
        Line("CrystalLowerFacet", root.transform, new[]
        {
            outer[4], new Vector2(0, -24), outer[2]
        }, 3.2f, CyanBright);
        Line("CrystalFacetLeft", root.transform, new[]
        {
            outer[0], outer[5], new Vector2(0, 65), outer[4], outer[3]
        }, 2f, Hex("29D3FF", 0.78f));
        Line("CrystalFacetRight", root.transform, new[]
        {
            outer[0], outer[1], new Vector2(0, 65), outer[2], outer[3]
        }, 2f, Hex("29D3FF", 0.78f));

        for (int i = 0; i < 18; i++)
        {
            float y = -175 + i * 20.5f;
            float x = ((i * 37) % 82) - 41;
            Dot("CrystalSpark", root.transform, new Vector2(x, y), i % 5 == 0 ? 2.2f : 1.2f,
                Hex("C6F9FF", i % 3 == 0 ? 0.9f : 0.55f));
        }

        return root.GetComponent<RectTransform>();
    }

    private static void CreateCircuitConnection(Transform parent, Vector2 cardEdge, Vector2 coreEdge)
    {
        float direction = Mathf.Sign(coreEdge.x - cardEdge.x);
        Vector2 midA = cardEdge + new Vector2(direction * 22, 0);
        Vector2 midB = new Vector2((midA.x + coreEdge.x) * 0.5f, coreEdge.y);
        Line("CircuitLink", parent, new[] { cardEdge, midA, midB, coreEdge },
            2.2f, Hex("18C8FF", 0.82f));
        Dot("CircuitNode", parent, cardEdge, 5f, CyanBright);
        Dot("CircuitNode", parent, coreEdge, 4f, Cyan);
    }

    private static void CreateMissionCards(Transform parent)
    {
        GameObject objective = FramedPanel("CurrentObjective", parent,
            new Vector2(0, -520), new Vector2(1044, 152), PanelBase, CyanMuted, 0.78f);
        CreateRadarGlyph(objective.transform, new Vector2(-421, 0), 42, Cyan);
        Text("Label", objective.transform, "OBJETIVO ACTUAL", new Vector2(-190, 24),
            new Vector2(390, 38), 26, Cyan, TextAnchor.MiddleLeft, FontStyle.Normal, 1.6f);
        Text("Value", objective.transform, "Explora 3 señales desconocidas.", new Vector2(-92, -28),
            new Vector2(590, 42), 30, Amber, TextAnchor.MiddleLeft, FontStyle.Normal, 1.2f);

        GameObject progress = FramedPanel("GlobalProgress", parent,
            new Vector2(0, -691), new Vector2(1044, 136), PanelBase, CyanMuted, 0.78f);
        CreateCompassGlyph(progress.transform, new Vector2(-421, 0), 41, Cyan);
        Text("Label", progress.transform, "PROGRESO GLOBAL", new Vector2(-190, 20),
            new Vector2(390, 36), 25, Cyan, TextAnchor.MiddleLeft, FontStyle.Normal, 1.5f);
        Text("Value", progress.transform, "42%", new Vector2(-190, -27),
            new Vector2(390, 44), 34, Amber, TextAnchor.MiddleLeft, FontStyle.Normal, 1.2f);
        CreateProgressRing(progress.transform, new Vector2(428, 0), 42, 0.42f);
    }

    private static void CreateBottomNavigation(Transform parent)
    {
        string[] labels = { "GALAXIA", "EXPLORAR", "HANGAR", "RELIQUIAS", "ÁRBOL" };
        float cardWidth = 199f;
        for (int i = 0; i < labels.Length; i++)
        {
            GameObject card = FramedPanel("Nav_" + labels[i], parent,
                new Vector2(-410 + i * 205, -875), new Vector2(cardWidth, 148),
                PanelBase, CyanMuted, 0.72f);
            Button button = card.AddComponent<Button>();
            button.targetGraphic = card.GetComponent<Image>();
            UnityEventTools.AddStringPersistentListener(button.onClick, controller.SelectModule, labels[i]);

            if (i == 0)
            {
                Image icon = CreateImage("Icon", card.transform, navIcons[i], new Vector2(0, 29),
                    new Vector2(83, 78), Cyan);
                icon.preserveAspect = true;
            }
            else if (i == 1) CreateRadarGlyph(card.transform, new Vector2(0, 29), 34, Cyan);
            else if (i == 2) CreateShipGlyph(card.transform, new Vector2(0, 29), 0.78f, Cyan);
            else if (i == 3) CreateRelicGlyph(card.transform, new Vector2(0, 29), 0.78f, Cyan);
            else CreateTreeGlyph(card.transform, new Vector2(0, 29), 39, Cyan);
            Text("Label", card.transform, labels[i], new Vector2(0, -47),
                new Vector2(cardWidth - 18, 34), 22, Cyan, TextAnchor.MiddleCenter,
                FontStyle.Normal, 1.1f);
        }
    }

    private static GameObject CreateMetalsDrawer(Transform parent)
    {
        GameObject drawer = FramedPanel("MetalsDrawer", parent, new Vector2(350, 560),
            new Vector2(350, 520), Hex("020D14", 0.995f), Cyan, 0.92f);
        drawer.transform.SetAsLastSibling();
        Text("Title", drawer.transform, "INVENTARIO DE METALES", new Vector2(0, 222),
            new Vector2(310, 42), 23, White, TextAnchor.MiddleCenter, FontStyle.Bold, 1.2f);

        string[] names =
        {
            "HIERRO", "COBRE", "ALUMINIO", "TITANIO", "NÍQUEL",
            "COBALTO", "LITIO", "TUNGSTENO", "PLATINO", "IRIDIO"
        };
        string[] values =
        {
            "5.98M", "3.62M", "4.73M", "2.11M", "4.70M",
            "1.83M", "1.27M", "1.05M", "0.64M", "0.42M"
        };

        for (int i = 0; i < names.Length; i++)
        {
            float y = 172 - i * 39;
            Text("MetalName", drawer.transform, names[i], new Vector2(-80, y),
                new Vector2(160, 30), 17, Steel, TextAnchor.MiddleLeft);
            Text("MetalValue", drawer.transform, values[i], new Vector2(95, y),
                new Vector2(110, 30), 17, White, TextAnchor.MiddleRight);
        }
        Text("Footer", drawer.transform, "TOTAL · 10 METALES", new Vector2(0, -225),
            new Vector2(300, 34), 18, Cyan, TextAnchor.MiddleCenter);
        return drawer;
    }

    private enum IconKind { Radar, Scanner, Ship, Relic, Tree, Compass }

    private static void CreateCardIcon(Transform parent, IconKind kind, Vector2 center)
    {
        switch (kind)
        {
            case IconKind.Radar:
                CreateRadarGlyph(parent, center, 54, Cyan);
                break;
            case IconKind.Scanner:
                CreateRadarGlyph(parent, center, 52, Cyan);
                Line("Probe", parent, new[]
                {
                    center + new Vector2(0, 19), center + new Vector2(12, -5),
                    center + new Vector2(0, -29), center + new Vector2(-12, -5)
                }, 2.5f, CyanBright, true);
                break;
            case IconKind.Ship:
                CreateShipGlyph(parent, center, 1f, Cyan);
                break;
            case IconKind.Relic:
                CreateRelicGlyph(parent, center, 1f, Cyan);
                break;
            case IconKind.Tree:
                CreateTreeGlyph(parent, center, 53, Cyan);
                break;
            case IconKind.Compass:
                CreateCompassGlyph(parent, center, 52, Cyan);
                break;
        }
    }

    private static void CreateRadarGlyph(Transform parent, Vector2 center, float radius, Color color)
    {
        Circle("RadarOuter", parent, center, radius, 42, 2f, color);
        Circle("RadarInner", parent, center, radius * 0.66f, 36, 1.3f, Hex("39D7FF", 0.72f));
        Circle("RadarCore", parent, center, radius * 0.23f, 24, 3f, CyanBright);
        Line("RadarH", parent, new[]
        {
            center + new Vector2(-radius - 9, 0), center + new Vector2(radius + 9, 0)
        }, 1.2f, Hex("18C8FF", 0.72f));
        Line("RadarV", parent, new[]
        {
            center + new Vector2(0, -radius - 9), center + new Vector2(0, radius + 9)
        }, 1.2f, Hex("18C8FF", 0.72f));
        Line("RadarSweep", parent, new[] { center, center + new Vector2(radius * 0.72f, radius * 0.48f) },
            2f, Hex("7AEAFF", 0.78f));
    }

    private static void CreateTreeGlyph(Transform parent, Vector2 center, float radius, Color color)
    {
        float s = radius / 53f;
        Vector2 basePoint = center + new Vector2(0, -42 * s);
        Line("TreeTrunk", parent, new[]
        {
            basePoint, center + new Vector2(-3, -12) * s,
            center + new Vector2(3, 15) * s, center + new Vector2(0, 46) * s
        }, 3.2f, color);

        Vector2[][] branches =
        {
            new[] { center + new Vector2(-1, -17) * s, center + new Vector2(-22, -1) * s, center + new Vector2(-43, 4) * s },
            new[] { center + new Vector2(1, -9) * s, center + new Vector2(24, 4) * s, center + new Vector2(44, 12) * s },
            new[] { center + new Vector2(1, 2) * s, center + new Vector2(-21, 19) * s, center + new Vector2(-38, 30) * s },
            new[] { center + new Vector2(2, 11) * s, center + new Vector2(23, 25) * s, center + new Vector2(37, 38) * s },
            new[] { center + new Vector2(1, 23) * s, center + new Vector2(-14, 37) * s, center + new Vector2(-20, 49) * s }
        };
        foreach (Vector2[] branch in branches)
        {
            Line("TreeBranch", parent, branch, 2f, color);
            Dot("TreeNode", parent, branch[branch.Length - 1], 3.1f * s, CyanBright);
        }
        Line("TreeRoots", parent, new[]
        {
            basePoint + new Vector2(-37, -7) * s, basePoint,
            basePoint + new Vector2(37, -7) * s
        }, 2.4f, color);
        Dot("TreeCrown", parent, center + new Vector2(0, 48) * s, 3.2f * s, CyanBright);
    }

    private static void CreateShipGlyph(Transform parent, Vector2 center, float scale, Color color)
    {
        Vector2[] hull =
        {
            center + new Vector2(0, 48) * scale,
            center + new Vector2(17, 16) * scale,
            center + new Vector2(43, -22) * scale,
            center + new Vector2(15, -14) * scale,
            center + new Vector2(0, -42) * scale,
            center + new Vector2(-15, -14) * scale,
            center + new Vector2(-43, -22) * scale,
            center + new Vector2(-17, 16) * scale
        };
        Polygon("ShipFill", parent, hull, Hex("079BC9", 0.38f));
        Line("ShipHull", parent, hull, 3f * scale, color, true);
        Line("ShipCockpit", parent, new[]
        {
            center + new Vector2(0, 29) * scale,
            center + new Vector2(8, 6) * scale,
            center + new Vector2(0, -6) * scale,
            center + new Vector2(-8, 6) * scale
        }, 2.4f * scale, CyanBright, true);
        Line("ShipWingLeft", parent, new[]
        {
            center + new Vector2(-17, 16) * scale,
            center + new Vector2(-15, -14) * scale
        }, 2f * scale, CyanBright);
        Line("ShipWingRight", parent, new[]
        {
            center + new Vector2(17, 16) * scale,
            center + new Vector2(15, -14) * scale
        }, 2f * scale, CyanBright);
    }

    private static void CreateRelicGlyph(Transform parent, Vector2 center, float scale, Color color)
    {
        Vector2[] body =
        {
            center + new Vector2(0, 51) * scale,
            center + new Vector2(16, 23) * scale,
            center + new Vector2(12, -19) * scale,
            center + new Vector2(0, -43) * scale,
            center + new Vector2(-12, -19) * scale,
            center + new Vector2(-16, 23) * scale
        };
        Polygon("RelicFill", parent, body, Hex("087EB5", 0.35f));
        Line("RelicBody", parent, body, 3f * scale, color, true);
        Line("RelicSpine", parent, new[]
        {
            center + new Vector2(0, 43) * scale, center + new Vector2(0, -40) * scale
        }, 2f * scale, CyanBright);
        Line("RelicFins", parent, new[]
        {
            center + new Vector2(-12, -11) * scale,
            center + new Vector2(-30, -30) * scale,
            center + new Vector2(0, -20) * scale,
            center + new Vector2(30, -30) * scale,
            center + new Vector2(12, -11) * scale
        }, 2.2f * scale, color);
        Ellipse("RelicOrbitOuter", parent, center + new Vector2(0, -34) * scale,
            45 * scale, 13 * scale, 38, 1.8f * scale, Hex("18C8FF", 0.85f));
        Ellipse("RelicOrbitInner", parent, center + new Vector2(0, -34) * scale,
            27 * scale, 7 * scale, 32, 1.4f * scale, Hex("75EAFF", 0.75f));
    }

    private static void CreateCompassGlyph(Transform parent, Vector2 center, float radius, Color color)
    {
        Circle("CompassRing", parent, center, radius, 44, 2f, color);
        Line("CompassCrossH", parent, new[]
        {
            center + new Vector2(-radius - 10, 0), center + new Vector2(radius + 10, 0)
        }, 1.4f, color);
        Line("CompassCrossV", parent, new[]
        {
            center + new Vector2(0, -radius - 10), center + new Vector2(0, radius + 10)
        }, 1.4f, color);
        Vector2[] star =
        {
            center + new Vector2(0, radius * 0.72f), center + new Vector2(8, 8),
            center + new Vector2(radius * 0.72f, 0), center + new Vector2(8, -8),
            center + new Vector2(0, -radius * 0.72f), center + new Vector2(-8, -8),
            center + new Vector2(-radius * 0.72f, 0), center + new Vector2(-8, 8)
        };
        Line("CompassStar", parent, star, 2.5f, CyanBright, true);
    }

    private static void CreateCommandGlyph(Transform parent)
    {
        Color c = Hex("36CFFF", 0.78f);
        Line("CommandOuter", parent, new[]
        {
            new Vector2(0, 50), new Vector2(45, 25), new Vector2(45, -26),
            new Vector2(0, -52), new Vector2(-45, -26), new Vector2(-45, 25)
        }, 9f, c, true);
        Line("CommandInner", parent, new[]
        {
            new Vector2(-27, -24), new Vector2(-27, 15), new Vector2(0, 32),
            new Vector2(28, 16), new Vector2(28, -24), new Vector2(7, -35),
            new Vector2(7, 5), new Vector2(-8, 5), new Vector2(-8, -26)
        }, 8f, c);
    }

    private static void CreateHexagon(Transform parent, Vector2 center, float radius, Color color, float thickness)
    {
        var points = new List<Vector2>();
        for (int i = 0; i < 6; i++)
        {
            float a = Mathf.PI * 2f * i / 6f + Mathf.PI * 0.5f;
            points.Add(center + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius);
        }
        Line("Hexagon", parent, points, thickness, color, true);
    }

    private static void CreateProgressRing(Transform parent, Vector2 center, float radius, float progress)
    {
        const int segments = 52;
        var back = new List<Vector2>();
        var filled = new List<Vector2>();
        for (int i = 0; i <= segments; i++)
        {
            float a = Mathf.PI * 0.5f - Mathf.PI * 2f * i / segments;
            Vector2 p = center + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius;
            back.Add(p);
            if (i <= Mathf.RoundToInt(segments * progress)) filled.Add(p);
        }
        Line("RingBack", parent, back, 9f, Hex("17465B"));
        Line("RingProgress", parent, filled, 9f, Cyan);
    }

    private static void CreateMetalGlyph(Transform parent, Vector2 position, float radius, int variant)
    {
        Color metal = variant == 0 ? Hex("BFCAD2") : variant == 1 ? Hex("D8E4EA") : Hex("C6C19A");
        if (variant == 1)
        {
            Vector2[] ingot =
            {
                position + new Vector2(-16, -21), position + new Vector2(7, -24),
                position + new Vector2(21, 18), position + new Vector2(-3, 21)
            };
            Line("Ingot", parent, ingot, 4f, metal, true);
            Line("IngotFacet", parent, new[] { ingot[0], position + new Vector2(1, 10), ingot[2] },
                2f, CyanMuted);
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            Vector2 c = position + new Vector2((i - 1) * 15, i == 1 ? 9 : -4);
            float r = radius * (i == 1 ? 0.62f : 0.48f);
            Vector2[] ore =
            {
                c + new Vector2(-r, -r * 0.35f), c + new Vector2(-r * 0.35f, r),
                c + new Vector2(r * 0.7f, r * 0.6f), c + new Vector2(r, -r * 0.55f),
                c + new Vector2(0, -r)
            };
            Line("Ore", parent, ore, 2.5f, metal, true);
            Line("OreFacet", parent, new[] { ore[1], c, ore[3] }, 1.3f, CyanMuted);
        }
    }

    private static GameObject FramedPanel(
        string name,
        Transform parent,
        Vector2 position,
        Vector2 size,
        Color fill,
        Color border,
        float frameAlpha)
    {
        GameObject panel = Panel(name, parent, position, size, fill);
        Image borderImage = CreateImage(name + "_Border", panel.transform, frame, Vector2.zero, size, border);
        borderImage.type = Image.Type.Sliced;
        Color borderColor = border;
        borderColor.a *= Mathf.Clamp01(frameAlpha);
        borderImage.color = borderColor;
        borderImage.raycastTarget = false;

        Image inner = CreateImage(name + "_InnerBorder", panel.transform, frame, Vector2.zero,
            size - new Vector2(13, 13), Hex("1ACBFF", 0.18f));
        inner.type = Image.Type.Sliced;
        inner.raycastTarget = false;
        return panel;
    }

    private static GameObject Panel(
        string name,
        Transform parent,
        Vector2 position,
        Vector2 size,
        Color color,
        Sprite sprite = null,
        float rotation = 0f)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        rect.localRotation = Quaternion.Euler(0, 0, rotation);

        Image image = go.GetComponent<Image>();
        image.color = color;
        image.sprite = sprite;
        image.type = sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        return go;
    }

    private static Image CreateImage(
        string name,
        Transform parent,
        Sprite sprite,
        Vector2 position,
        Vector2 size,
        Color color)
    {
        return Panel(name, parent, position, size, color, sprite).GetComponent<Image>();
    }

    private static Text Text(
        string name,
        Transform parent,
        string value,
        Vector2 position,
        Vector2 size,
        int fontSize,
        Color color,
        TextAnchor anchor,
        FontStyle fontStyle = FontStyle.Normal,
        float letterSpacing = 0f)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        go.transform.SetParent(parent, false);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = position;

        Text text = go.GetComponent<Text>();
        text.font = font;
        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.color = color;
        text.alignment = anchor;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.lineSpacing = 0.88f;
        text.raycastTarget = false;
        return text;
    }

    private static D1HubLineGraphic Line(
        string name,
        Transform parent,
        IList<Vector2> points,
        float thickness,
        Color color,
        bool closed = false)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(D1HubLineGraphic));
        go.transform.SetParent(parent, false);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(Width, Height);
        rect.anchoredPosition = Vector2.zero;

        D1HubLineGraphic line = go.GetComponent<D1HubLineGraphic>();
        line.color = color;
        line.raycastTarget = false;
        line.SetLine(points, thickness, closed);
        return line;
    }

    private static D1HubPolygonGraphic Polygon(
        string name,
        Transform parent,
        IList<Vector2> points,
        Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(D1HubPolygonGraphic));
        go.transform.SetParent(parent, false);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(Width, Height);
        rect.anchoredPosition = Vector2.zero;

        D1HubPolygonGraphic polygon = go.GetComponent<D1HubPolygonGraphic>();
        polygon.color = color;
        polygon.raycastTarget = false;
        polygon.SetPolygon(points);
        return polygon;
    }

    private static void Circle(
        string name,
        Transform parent,
        Vector2 center,
        float radius,
        int segments,
        float thickness,
        Color color,
        bool dotted = false)
    {
        if (dotted)
        {
            for (int i = 0; i < segments; i += 2)
            {
                float a = Mathf.PI * 2f * i / segments;
                Dot(name, parent, center + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius,
                    thickness, color);
            }
            return;
        }

        var points = new List<Vector2>(segments);
        for (int i = 0; i < segments; i++)
        {
            float a = Mathf.PI * 2f * i / segments;
            points.Add(center + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius);
        }
        Line(name, parent, points, thickness, color, true);
    }

    private static void Ellipse(
        string name,
        Transform parent,
        Vector2 center,
        float radiusX,
        float radiusY,
        int segments,
        float thickness,
        Color color)
    {
        var points = new List<Vector2>(segments);
        for (int i = 0; i < segments; i++)
        {
            float a = Mathf.PI * 2f * i / segments;
            points.Add(center + new Vector2(Mathf.Cos(a) * radiusX, Mathf.Sin(a) * radiusY));
        }
        Line(name, parent, points, thickness, color, true);
    }

    private static void Dot(string name, Transform parent, Vector2 position, float radius, Color color)
    {
        Image dot = CreateImage(name, parent, null, position, Vector2.one * radius * 2f, color);
        dot.raycastTarget = false;
    }

    private static Sprite LoadSprite(string path)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private static void ConfigureSpriteImports()
    {
        string[] paths =
        {
            "Assets/Art/d1_premium_frame_v4.png",
            "Assets/Art/d1_nav_galaxy_v3.png",
            "Assets/Art/d1_nav_explore_v3.png",
            "Assets/Art/d1_nav_hangar_v3.png",
            "Assets/Art/d1_nav_relics_v3.png",
            "Assets/Art/d1_nav_tree_v3.png"
        };

        foreach (string path in paths)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) continue;
            Vector4 desiredBorder = path.Contains("premium_frame")
                ? new Vector4(18, 18, 18, 18)
                : Vector4.zero;
            bool changed = importer.textureType != TextureImporterType.Sprite ||
                           importer.spriteImportMode != SpriteImportMode.Single ||
                           !importer.alphaIsTransparency || importer.mipmapEnabled ||
                           importer.wrapMode != TextureWrapMode.Clamp ||
                           importer.filterMode != FilterMode.Bilinear ||
                           importer.spriteBorder != desiredBorder;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Bilinear;
            importer.spriteBorder = desiredBorder;
            if (changed) importer.SaveAndReimport();
        }
        AssetDatabase.Refresh();
    }

    private static Color Hex(string value, float alpha = 1f)
    {
        ColorUtility.TryParseHtmlString("#" + value, out Color color);
        color.a = alpha;
        return color;
    }

    private static void RenderCapture(string relativePath)
    {
        const string captureGlyphs =
            "DIMENSIÓN CENTRO DE MANDO Tu base operaciones en la Hierro Aluminio Níquel " +
            "METALES SECTOR ACTUAL ÓRBITAS ANTIGUAS ESCÁNER NIVEL FLOTA NAVES RELIQUIAS " +
            "PUNTOS DEL ÁRBOL EXPEDICIONES ACTIVAS OBJETIVO Explora señales desconocidas " +
            "PROGRESO GLOBAL GALAXIA EXPLORAR HANGAR 0123456789.+/%";
        font.RequestCharactersInTexture(captureGlyphs, 64, FontStyle.Normal);
        font.RequestCharactersInTexture(captureGlyphs, 64, FontStyle.Bold);

        RenderTexture target = new RenderTexture(Width, Height, 24, RenderTextureFormat.ARGB32);
        RenderTexture previous = RenderTexture.active;
        camera.targetTexture = target;
        for (int pass = 0; pass < 8; pass++)
        {
            Canvas.ForceUpdateCanvases();
            camera.Render();
            GL.Flush();
        }
        RenderTexture.active = target;

        Texture2D image = new Texture2D(Width, Height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
        image.Apply();
        File.WriteAllBytes(Path.GetFullPath(relativePath), image.EncodeToPNG());

        UnityEngine.Object.DestroyImmediate(image);
        camera.targetTexture = null;
        RenderTexture.active = previous;
        target.Release();
        UnityEngine.Object.DestroyImmediate(target);
    }
}
#endif

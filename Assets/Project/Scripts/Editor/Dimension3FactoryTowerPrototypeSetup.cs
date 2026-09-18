#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Builds and captures an isolated, code-native Unity UI prototype for Dimension 3.
/// It never opens or modifies Main.unity.
/// </summary>
public static class Dimension3FactoryTowerPrototypeSetup
{
    private const int Width = 1080;
    private const int Height = 1920;
    private const string ScenePath = "Assets/Project/Scenes/Dimension3FactoryTowerPrototype.unity";
    private const string CapturePath = "Logs/VisualQA/Dimension3Prototype/D3_FactoryTower_Unity_1080x1920.png";
    private const string FontPath = "Assets/Project/UI/Vertical/Fonts/Rajdhani-Medium SDF.asset";
    private const string BackgroundPath = "Assets/Project/Art/Dimension3/Prototype/FactoryInteriorBackground_v2.png";

    private static readonly Color Bg = Hex("080D0F");
    private static readonly Color Deep = Hex("0D1416");
    private static readonly Color Metal = Hex("162023");
    private static readonly Color Metal2 = Hex("202D30");
    private static readonly Color Metal3 = Hex("2D3B3E");
    private static readonly Color Edge = Hex("526064");
    private static readonly Color Copper = Hex("A96932");
    private static readonly Color CopperBright = Hex("DD9145");
    private static readonly Color Amber = Hex("E3A53D");
    private static readonly Color Ivory = Hex("DDD8C6");
    private static readonly Color Muted = Hex("879397");
    private static readonly Color Green = Hex("55B982");
    private static readonly Color Red = Hex("C25742");
    private static readonly Color Cyan = Hex("62AAB1");

    private static TMP_FontAsset font;
    private static Sprite uiSprite;
    private static Texture2D factoryBackground;
    private static readonly List<RectTransform> conveyorMarkers = new List<RectTransform>();

    [MenuItem("Tools/Quantum Forge/Dimension 3/Build Factory Tower Prototype")]
    public static void BuildAndCapture()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(CapturePath));
        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        if (font == null)
            throw new System.InvalidOperationException("Dimension 3 prototype font was not found.");

        uiSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        factoryBackground = AssetDatabase.LoadAssetAtPath<Texture2D>(BackgroundPath);
        if (factoryBackground == null)
            throw new System.InvalidOperationException("Dimension 3 factory background was not found.");
        conveyorMarkers.Clear();

        Scene previousScene = SceneManager.GetActiveScene();
        bool useAdditive = !Application.isBatchMode && previousScene.IsValid() &&
                           !string.IsNullOrEmpty(previousScene.path);
        Scene prototypeScene = EditorSceneManager.NewScene(
            NewSceneSetup.EmptyScene,
            useAdditive ? NewSceneMode.Additive : NewSceneMode.Single);
        try
        {
            SceneManager.SetActiveScene(prototypeScene);
            Camera camera = CreateCamera();
            Canvas canvas = CreateCanvas(camera);
            BuildScreen(canvas.transform);

            EditorSceneManager.SaveScene(prototypeScene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Render(camera, Path.GetFullPath(CapturePath));
            Debug.Log("[D3 Factory Prototype] Scene: " + ScenePath);
            Debug.Log("[D3 Factory Prototype] Capture: " + Path.GetFullPath(CapturePath));
        }
        finally
        {
            if (useAdditive && previousScene.IsValid() && previousScene.isLoaded)
                SceneManager.SetActiveScene(previousScene);
            if (useAdditive && prototypeScene.IsValid() && prototypeScene.isLoaded)
                EditorSceneManager.CloseScene(prototypeScene, true);
        }
    }

    private static Camera CreateCamera()
    {
        GameObject go = new GameObject("D3FactoryPrototypeCamera", typeof(Camera));
        Camera camera = go.GetComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Bg;
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.transform.position = new Vector3(0f, 0f, -10f);
        return camera;
    }

    private static Canvas CreateCanvas(Camera camera)
    {
        GameObject go = new GameObject("D3FactoryTowerPrototypeCanvas",
            typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = camera;
        canvas.planeDistance = 1f;

        CanvasScaler scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(Width, Height);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = .5f;
        return canvas;
    }

    private static void BuildScreen(Transform root)
    {
        Image background = Box(root, "Background", 0, 0, Width, Height, Bg);
        AddIndustrialBackdrop(background.transform);

        Panel(root, "OuterFrame", 18, 14, 1044, 1888, new Color(0, 0, 0, 0), Metal3, 3);
        Header(root);

        Transform scene = Panel(root, "FactoryProductionFloor", 28, 244, 1024, 910, Deep, Edge, 3).transform;
        FactoryInterior(scene);
        QueueRail(root);
        OperatorConsole(root);
        BottomNavigation(root);

        Dimension3FactoryTowerPrototypeUI controller = root.gameObject.AddComponent<Dimension3FactoryTowerPrototypeUI>();
        controller.pressHead = FindRect(root, "PressHead");
        controller.liftCar = FindRect(root, "CraneTrolley");
        controller.fanRotor = FindRect(root, "FanRotor");
        controller.conveyorMarkers = conveyorMarkers.ToArray();
        controller.runningLamp = FindImage(root, "RunningLamp");
        controller.queueProgress = FindImage(root, "QueueProgressFill");
        Transform steam = FindDeep(root, "Steam");
        controller.steam = steam != null ? steam.GetComponent<CanvasGroup>() : null;
        controller.ApplyPreviewPose(.78f);
    }

    private static void Header(Transform root)
    {
        Transform header = Panel(root, "Header", 28, 24, 1024, 118, Metal, Metal3, 2).transform;
        ButtonPlate(header, "Back", 16, 18, 74, 76, "<", 42, Metal2, Copper);
        Label(header, "PLANTA DE PRODUCCION 03", 112, 18, 790, 48, 34, Ivory,
            TextAlignmentOptions.Center, FontStyles.Bold, 2f);
        Label(header, "LINEA RECUPERADA · BANCO N2", 190, 65, 630, 30, 20, CopperBright,
            TextAlignmentOptions.Center, FontStyles.Bold, 1.4f);
        ButtonPlate(header, "Help", 924, 18, 74, 76, "?", 34, Metal2, Copper);

        Transform resources = Panel(root, "Resources", 28, 150, 1024, 82, Deep, Metal3, 2).transform;
        Resource(resources, 20, "LE", "12,450", Amber);
        Separator(resources, 322, 12, 2, 58, Metal3);
        Resource(resources, 350, "TRAZAS", "320", Cyan);
        Separator(resources, 654, 12, 2, 58, Metal3);
        Resource(resources, 680, "AUTOMATAS", "2 / 4", Green);
        Box(resources, "HeaderCable", 12, 72, 998, 3, Copper);
    }

    private static void QueueRail(Transform root)
    {
        Transform rail = Panel(root, "QueueRail", 44, 1172, 992, 108, Metal, Metal3, 2).transform;
        Label(rail, "RIEL DE ORDENES", 18, 10, 190, 24, 18, Muted,
            TextAlignmentOptions.Left, FontStyles.Bold, 1.2f);
        QueueSlot(rail, 18, 40, 230, "PIEZAS", "CHASIS V1", true);
        QueueSlot(rail, 258, 40, 230, "ENSAMBLE", "1 PEND.", false);
        QueueSlot(rail, 498, 40, 230, "PLANOS", "LIBRE", false);
        QueueSlot(rail, 738, 40, 230, "OBRAS", "LIBRE", false);
    }

    private static void QueueSlot(Transform root, float x, float y, float w, string title, string value, bool active)
    {
        Color border = active ? CopperBright : Edge;
        Transform slot = Panel(root, "Queue_" + title, x, y, w, 54, active ? Hex("292018") : Deep, border, 2).transform;
        Box(slot, active ? "RunningLamp" : "Lamp", 10, 17, 16, 16, active ? Green : Metal3);
        Label(slot, title, 34, 5, 96, 20, 15, active ? Ivory : Muted,
            TextAlignmentOptions.Left, FontStyles.Bold, .8f);
        Label(slot, value, 34, 26, 182, 18, 14, active ? CopperBright : Muted,
            TextAlignmentOptions.Left, FontStyles.Normal, .6f);
        if (active)
        {
            Image bg = Box(slot, "QueueProgress", 136, 10, 78, 7, Metal3);
            Image fill = Box(bg.transform, "QueueProgressFill", 0, 0, 78, 7, Green);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillAmount = .46f;
        }
    }

    private static void Cutaway(Transform root)
    {
        AddBrickWall(root);
        AddTowerFrame(root);
        AddFloorSelector(root);
        AddControlFloor(root);
        AddWorkshopFloor(root);
        AddProductionFloor(root);
        AddLiftShaft(root);
        AddForegroundPipes(root);
    }

    private static void FactoryInterior(Transform root)
    {
        AddFactoryBackgroundPlate(root);
        AddFactorySceneHeader(root);
        AddMaterialIntake(root);
        AddFlexibleProductionCell(root);
        AddParallelAssemblyCell(root);
        AddProductionFlow(root);
        AddFactoryForegroundV2(root);
    }

    private static void AddFactoryBackgroundPlate(Transform root)
    {
        GameObject go = new GameObject("FactoryBackgroundPlate", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
        go.transform.SetParent(root, false);
        RectTransform rect = go.GetComponent<RectTransform>();
        SetRect(rect, 0, 0, 1024, 910);
        RawImage image = go.GetComponent<RawImage>();
        image.texture = factoryBackground;
        image.uvRect = new Rect(0f, .13f, 1f, .74f);
        image.color = new Color(.78f, .82f, .80f, 1f);
        image.raycastTarget = false;

        Box(root, "BackgroundShade", 0, 0, 1024, 910, new Color(.015f, .025f, .025f, .24f));
        Box(root, "SharedWarmLight", 210, 352, 584, 380, new Color(.96f, .25f, .025f, .018f));
        Box(root, "FloorHeatReflection", 286, 692, 514, 118, new Color(.92f, .28f, .035f, .016f));
        Box(root, "LeftVignette", 0, 0, 92, 910, new Color(0f, 0f, 0f, .22f));
        Box(root, "RightVignette", 944, 0, 80, 910, new Color(0f, 0f, 0f, .22f));
    }

    private static void AddFactorySceneHeader(Transform root)
    {
        Transform plate = Panel(root, "LineIdentification", 28, 24, 430, 58,
            new Color(.035f, .05f, .05f, .92f), Hex("4C5555"), 2).transform;
        Box(plate, "ActiveLineLamp", 16, 18, 18, 18, Green);
        Label(plate, "LINEA A · PRODUCCION ACTIVA", 46, 10, 364, 34, 18, Ivory,
            TextAlignmentOptions.Left, FontStyles.Bold, .9f);

        Transform bank = Panel(root, "ProcessBankPlate", 790, 24, 204, 58,
            new Color(.035f, .05f, .05f, .92f), Copper, 2).transform;
        Label(bank, "BANCO N2", 12, 8, 180, 25, 18, CopperBright,
            TextAlignmentOptions.Center, FontStyles.Bold, .9f);
        Label(bank, "MECANIZADO", 12, 31, 180, 18, 13, Muted,
            TextAlignmentOptions.Center, FontStyles.Bold, .5f);
    }

    private static void AddMaterialIntake(Transform root)
    {
        Box(root, "IntakeContactShadow", 46, 684, 196, 54, new Color(0, 0, 0, .70f));
        Transform intake = Box(root, "MaterialIntake", 42, 304, 204, 430,
            new Color(0, 0, 0, 0)).transform;

        Box(intake, "IntakeFrameLeft", 8, 52, 18, 348, Metal3);
        Box(intake, "IntakeFrameRight", 178, 52, 18, 348, Metal3);
        Box(intake, "IntakeFrameTop", 8, 52, 188, 18, Metal3);
        Box(intake, "IntakeFootLeft", 18, 382, 34, 72, Hex("293331"));
        Box(intake, "IntakeFootRight", 152, 382, 34, 72, Hex("293331"));

        Label(intake, "ENTRADA", 16, 14, 172, 26, 17, Muted,
            TextAlignmentOptions.Center, FontStyles.Bold, 1f);
        Label(intake, "MATERIA PRIMA", 12, 34, 180, 30, 20, Ivory,
            TextAlignmentOptions.Center, FontStyles.Bold, .7f);

        Transform hopper = Panel(intake, "RawHopper", 26, 82, 152, 128,
            Hex("4A3424"), Hex("7D5A39"), 3).transform;
        Box(hopper, "HopperMouth", 12, 12, 128, 36, Hex("151616"));
        for (int i = 0; i < 7; i++)
            Box(hopper, "RawChunk_" + i, 20 + (i % 4) * 27, 28 + (i / 4) * 24,
                22, 18, i % 2 == 0 ? Hex("6D5744") : Hex("4C5B59"));
        Box(hopper, "HopperNeck", 56, 90, 40, 50, Copper);
        HazardStrip(hopper, 10, 106, 132, 10, 22);

        Box(intake, "FeederChute", 84, 208, 34, 104, Hex("6A4129"));
        for (int i = 0; i < 4; i++)
            Box(intake, "FeederGate_" + i, 64, 226 + i * 25, 74, 8, Metal3);
        Box(intake, "FeederTray", 30, 310, 144, 28, Metal3);
        Box(intake, "RawBlank", 72, 298, 64, 30, Hex("7A654E"));

        Box(intake, "IntakeLamp", 22, 352, 18, 18, Green);
        Label(intake, "SUMINISTRO ESTABLE", 50, 344, 138, 28, 14, Green,
            TextAlignmentOptions.Left, FontStyles.Bold, .45f);
    }

    private static void AddFlexibleProductionCell(Transform root)
    {
        Box(root, "ProductionContactShadow", 254, 706, 416, 58, new Color(0, 0, 0, .76f));
        Transform cell = Box(root, "FlexibleProductionCell", 238, 178, 426, 590,
            new Color(0, 0, 0, 0)).transform;

        Box(cell, "CellTopBody", 62, 18, 302, 62, Hex("4D3424"));
        HazardStrip(cell, 68, 70, 290, 10, 25);
        Box(cell, "CellTitleLamp", 80, 38, 18, 18, Amber);
        Label(cell, "CELULA FLEXIBLE F-01", 110, 28, 234, 32, 19, Ivory,
            TextAlignmentOptions.Center, FontStyles.Bold, 1.1f);

        Box(cell, "PressColumnLeft", 26, 102, 68, 340, Hex("313E3E"));
        Box(cell, "PressColumnRight", 332, 102, 68, 340, Hex("313E3E"));
        Box(cell, "PressFootLeft", 34, 428, 58, 142, Hex("293331"));
        Box(cell, "PressFootRight", 334, 428, 58, 142, Hex("293331"));
        Box(cell, "HydraulicCylinder", 172, 102, 82, 126, Hex("77807A"));
        Box(cell, "HydraulicRod", 195, 206, 36, 92, Hex("A6AAA0"));
        RectTransform head = Panel(cell, "PressHead", 112, 272, 202, 76,
            Hex("6B3C21"), CopperBright, 3).rectTransform;
        Box(head, "DieClampLeft", 16, 62, 48, 40, Copper);
        Box(head, "DieClampRight", 138, 62, 48, 40, Copper);

        Box(cell, "WorkBed", 88, 402, 250, 44, Hex("704428"));
        Box(cell, "HotBlankGlowWide", 106, 346, 214, 82, new Color(1f, .25f, .02f, .12f));
        Box(cell, "HotBlankGlow", 132, 366, 162, 52, new Color(1f, .43f, .05f, .28f));
        Box(cell, "HotPart", 156, 378, 114, 32, Hex("F06B24"));
        Box(cell, "HotCore", 184, 384, 58, 20, Hex("FFC15C"));

        Transform dieRack = Panel(cell, "DieMagazine", 12, 452, 402, 80,
            Hex("111716"), Hex("47514F"), 2).transform;
        Label(dieRack, "MATRICES", 12, 7, 76, 18, 13, Muted,
            TextAlignmentOptions.Left, FontStyles.Bold, .5f);
        string[] dies = { "CH", "MO", "HE", "CO", "RE" };
        string[] amounts = { "03", "02", "01", "04", "00" };
        for (int i = 0; i < dies.Length; i++)
        {
            bool selected = i == 0;
            Transform die = Panel(dieRack, "Die_" + dies[i], 92 + i * 60, 8, 52, 58,
                selected ? Hex("4B2918") : Hex("1C2524"), selected ? CopperBright : Edge,
                selected ? 3 : 1).transform;
            Label(die, dies[i], 4, 5, 44, 24, 16, selected ? Ivory : Muted,
                TextAlignmentOptions.Center, FontStyles.Bold, .5f);
            Label(die, amounts[i], 4, 31, 44, 18, 13, selected ? Green : Ivory,
                TextAlignmentOptions.Center, FontStyles.Bold, .4f);
        }

        Transform progress = Panel(cell, "ProductionJob", 280, 112, 110, 74,
            Hex("121918"), Edge, 2).transform;
        Label(progress, "CHASIS V1", 8, 8, 94, 19, 13, Ivory,
            TextAlignmentOptions.Center, FontStyles.Bold, .45f);
        Label(progress, "6.0 SEG", 8, 30, 94, 20, 15, CopperBright,
            TextAlignmentOptions.Center, FontStyles.Bold, .5f);
        Box(progress, "CellProgressBg", 10, 56, 90, 7, Metal3);
        Box(progress, "CellProgressFill", 10, 56, 48, 7, Green);

        for (int i = 0; i < 6; i++)
        {
            Image spark = Box(cell, "Spark_" + i, 140 + i * 25, 354 + (i % 3) * 10,
                5, 28, i % 2 == 0 ? Amber : CopperBright);
            spark.rectTransform.localRotation = Quaternion.Euler(0, 0, -36f + i * 13f);
        }
    }

    private static void AddParallelAssemblyCell(Transform root)
    {
        Box(root, "AssemblyContactShadow", 680, 714, 292, 52, new Color(0, 0, 0, .72f));
        Transform cell = Box(root, "ParallelAssemblyCell", 670, 274, 302, 500,
            new Color(0, 0, 0, 0)).transform;
        Box(cell, "AssemblyHeader", 34, 10, 234, 54, Hex("263532"));
        Box(cell, "AssemblyLamp", 48, 27, 18, 18, Amber);
        Label(cell, "ENSAMBLAJE MK1", 78, 18, 174, 28, 18, Ivory,
            TextAlignmentOptions.Left, FontStyles.Bold, .8f);

        Box(cell, "CageRailLeft", 24, 78, 18, 262, Metal3);
        Box(cell, "CageRailRight", 260, 78, 18, 262, Metal3);
        Box(cell, "CageFootLeft", 24, 330, 34, 152, Hex("293331"));
        Box(cell, "CageFootRight", 244, 330, 34, 152, Hex("293331"));
        for (int i = 0; i < 6; i++)
            Box(cell, "CageBar_" + i, 36, 86 + i * 47, 230, 5,
                new Color(.34f, .42f, .40f, .68f));

        Transform armBase = Panel(cell, "ArmBase", 48, 250, 68, 68, Metal2, Edge, 2).transform;
        Image upper = Box(cell, "AssemblyArmUpper", 80, 178, 28, 104, Copper);
        upper.rectTransform.pivot = new Vector2(.5f, 1f);
        upper.rectTransform.localRotation = Quaternion.Euler(0, 0, -42f);
        Image lower = Box(cell, "AssemblyArmForearm", 124, 132, 24, 96, CopperBright);
        lower.rectTransform.pivot = new Vector2(.5f, 1f);
        lower.rectTransform.localRotation = Quaternion.Euler(0, 0, 34f);
        Box(cell, "AssemblyClaw", 150, 124, 48, 22, Hex("8B9188"));
        Box(armBase, "ArmJoint", 20, 20, 28, 28, Amber);

        Transform bot = Box(cell, "AutomatonInRig", 164, 112, 82, 172, new Color(0, 0, 0, 0)).transform;
        Box(bot, "RigBotHead", 22, 12, 38, 38, Metal3);
        Box(bot, "RigBotEye", 27, 26, 28, 7, Green);
        Box(bot, "RigBotBody", 14, 52, 54, 68, Hex("573821"));
        Box(bot, "RigBotCore", 29, 72, 24, 24, Amber);
        Box(bot, "RigBotArmL", 0, 58, 14, 74, Metal3);
        Box(bot, "RigBotArmR", 68, 58, 14, 74, Metal3);
        Box(bot, "RigBotLegL", 18, 120, 16, 48, Metal3);
        Box(bot, "RigBotLegR", 48, 120, 16, 48, Metal3);

        Transform sockets = Panel(cell, "AssemblySockets", 24, 350, 254, 58,
            Hex("111716"), Edge, 2).transform;
        for (int i = 0; i < 5; i++)
            Box(sockets, "SetSocket_" + i, 18 + i * 46, 15, 26, 26, i < 3 ? Green : Hex("6B2D22"));
        Box(cell, "AssemblyStatusBand", 22, 410, 258, 32, new Color(.04f, .06f, .055f, .88f));
        Label(cell, "SET  3 / 5 · ESPERANDO PIEZAS", 24, 412, 254, 28, 16, Amber,
            TextAlignmentOptions.Center, FontStyles.Bold, .55f);

        Image scan = Box(cell, "ScannerBeam", 152, 88, 5, 218, new Color(.25f, .85f, .75f, .35f));
        scan.rectTransform.localRotation = Quaternion.Euler(0, 0, -5f);
    }

    private static void AddProductionFlow(Transform root)
    {
        Box(root, "ConveyorShadowV2", 34, 760, 940, 76, new Color(0, 0, 0, .72f));
        Box(root, "ConveyorFrameV2", 28, 742, 950, 70, Hex("303B39"));
        Box(root, "ConveyorBeltV2", 42, 730, 922, 34, Hex("4B5551"));
        for (int i = 0; i < 12; i++)
            Box(root, "ConveyorRollerV2_" + i, 56 + i * 76, 780, 36, 20, Hex("0A0E0E"));
        for (int i = 0; i < 4; i++)
        {
            RectTransform marker = Box(root, "ConveyorMarker_" + i, 68 + i * 74, 739, 28, 18,
                CopperBright).rectTransform;
            conveyorMarkers.Add(marker);
        }
        Transform crate = Panel(root, "OutputCrateV2", 830, 688, 108, 74,
            Hex("56321E"), CopperBright, 2).transform;
        Label(crate, "CH-V1", 10, 19, 88, 26, 16, Ivory,
            TextAlignmentOptions.Center, FontStyles.Bold, .7f);

        Box(root, "FlowLabelsBand", 38, 816, 936, 38, new Color(.025f, .04f, .038f, .80f));
        Label(root, "ENTRADA", 54, 823, 160, 26, 16, Muted,
            TextAlignmentOptions.Left, FontStyles.Bold, .7f);
        Label(root, "FABRICACION", 384, 823, 210, 26, 16, CopperBright,
            TextAlignmentOptions.Center, FontStyles.Bold, .7f);
        Label(root, "SALIDA", 816, 823, 150, 26, 16, Green,
            TextAlignmentOptions.Right, FontStyles.Bold, .7f);
        Box(root, "FlowLine", 190, 837, 624, 4, Hex("6D5337"));
    }

    private static void AddFactoryForegroundV2(Transform root)
    {
        Box(root, "ForegroundPipe", -18, 94, 42, 680, Hex("54331F"));
        for (int i = 0; i < 5; i++)
            Box(root, "ForegroundPipeClamp_" + i, -24, 142 + i * 128, 56, 14, Hex("68706B"));

        Box(root, "ForegroundRail", 0, 862, 1024, 24, Hex("313B38"));
        for (int i = 0; i < 9; i++)
            Box(root, "RailPost_" + i, 38 + i * 118, 816, 14, 62, Hex("303A37"));
        HazardStrip(root, 0, 886, 1024, 12, 26);

        Box(root, "HotLightCone", 332, 474, 238, 264, new Color(1f, .25f, .02f, .035f));
        Box(root, "AssemblyCoolCone", 718, 386, 198, 330, new Color(.16f, .70f, .62f, .025f));
    }

    private static void AddFactoryHall(Transform root)
    {
        Box(root, "FactoryWall", 0, 0, 1024, 846, Hex("0C1213"));
        Box(root, "UpperSoot", 0, 0, 1024, 235, Hex("090E0F"));

        for (int y = 0; y < 12; y++)
        {
            float offset = y % 2 == 0 ? 0f : 44f;
            Box(root, "HallBrickH_" + y, 0, 84 + y * 62, 1024, 2,
                new Color(.24f, .30f, .30f, .16f));
            for (int x = 0; x < 13; x++)
                Box(root, "HallBrickV_" + y + "_" + x, offset + x * 88, 84 + y * 62, 2, 62,
                    new Color(.24f, .30f, .30f, .11f));
        }

        Transform windows = Panel(root, "ClerestoryWindows", 114, 54, 796, 142,
            Hex("132426"), Hex("344245"), 4).transform;
        for (int i = 0; i < 6; i++)
        {
            Box(windows, "Window_" + i, 10 + i * 130, 10, 116, 122,
                i % 3 == 1 ? new Color(.33f, .20f, .08f, .58f) : new Color(.08f, .24f, .25f, .60f));
            Box(windows, "WindowBarV_" + i, 66 + i * 130, 10, 4, 122, Metal3);
            Box(windows, "WindowBarH_" + i, 10 + i * 130, 66, 116, 4, Metal3);
        }
        Box(windows, "WindowHaze", 6, 6, 784, 130, new Color(.84f, .43f, .12f, .035f));

        Box(root, "CeilingBeam", 0, 28, 1024, 24, Metal3);
        for (int i = 0; i < 6; i++)
        {
            Image trussA = Box(root, "RoofTrussA_" + i, 65 + i * 178, 26, 12, 190, Hex("2B3739"));
            trussA.rectTransform.localRotation = Quaternion.Euler(0, 0, -33f);
            Image trussB = Box(root, "RoofTrussB_" + i, 155 + i * 178, 26, 12, 190, Hex("2B3739"));
            trussB.rectTransform.localRotation = Quaternion.Euler(0, 0, 33f);
        }

        Box(root, "BackPipe", 32, 222, 960, 18, Hex("563621"));
        for (int i = 0; i < 9; i++)
            Box(root, "BackPipeClamp_" + i, 62 + i * 108, 216, 10, 30, Edge);
        Box(root, "BackPipeDown", 930, 220, 20, 336, Hex("563621"));
        Box(root, "BackPipeElbow", 902, 526, 48, 22, Hex("563621"));

        Label(root, "PLANTA DE PRODUCCION · LINEA A", 34, 18, 530, 34, 22, Ivory,
            TextAlignmentOptions.Left, FontStyles.Bold, 1.2f);
        Box(root, "ShiftLamp", 744, 25, 17, 17, Green);
        Label(root, "TURNO ACTIVO", 772, 18, 210, 32, 17, Green,
            TextAlignmentOptions.Right, FontStyles.Bold, .8f);

        Box(root, "FactoryFloor", 0, 622, 1024, 224, Hex("151A1A"));
        Box(root, "FloorLip", 0, 618, 1024, 18, Metal3);
        for (int i = 0; i < 13; i++)
        {
            Image seam = Box(root, "FloorPerspective_" + i, 22 + i * 84, 620, 3, 300,
                new Color(.22f, .28f, .28f, .30f));
            seam.rectTransform.localRotation = Quaternion.Euler(0, 0, i < 6 ? -10f : 10f);
        }
    }

    private static void AddOverheadCrane(Transform root)
    {
        Box(root, "CraneRailShadow", 58, 199, 900, 30, new Color(0, 0, 0, .65f));
        Box(root, "CraneRail", 48, 188, 900, 30, Hex("4B3A2C"));
        HazardStrip(root, 48, 210, 900, 8, 22);
        Label(root, "PUENTE GRUA  03", 72, 190, 224, 19, 13, Ivory,
            TextAlignmentOptions.Left, FontStyles.Bold, .7f);

        RectTransform trolley = Panel(root, "CraneTrolley", 724, 176, 92, 72,
            Metal2, CopperBright, 3).rectTransform;
        Box(trolley, "CraneWheelL", 12, 56, 22, 16, Deep);
        Box(trolley, "CraneWheelR", 58, 56, 22, 16, Deep);
        Box(trolley, "CraneCable", 44, 69, 5, 104, Copper);
        Box(trolley, "CraneHook", 30, 158, 34, 27, CopperBright);
        Label(trolley, "5T", 20, 16, 52, 26, 17, Ivory,
            TextAlignmentOptions.Center, FontStyles.Bold, .7f);

        Box(root, "PendantCable", 866, 215, 4, 142, Copper);
        Transform pendant = Panel(root, "CranePendant", 842, 348, 52, 80, Metal2, Edge, 2).transform;
        Box(pendant, "PendantRed", 17, 12, 18, 18, Red);
        Box(pendant, "PendantGreen", 17, 42, 18, 18, Green);
    }

    private static void AddAssemblyPress(Transform root)
    {
        Box(root, "PressShadow", 302, 274, 432, 352, new Color(0, 0, 0, .72f));
        Transform press = Panel(root, "AssemblyPress", 286, 254, 432, 350,
            Hex("1D292B"), Hex("576366"), 4).transform;

        Box(press, "PressWarmBay", 76, 72, 280, 226, new Color(.48f, .21f, .055f, .20f));
        Box(press, "PressColumnL", 28, 62, 62, 250, Metal3);
        Box(press, "PressColumnR", 342, 62, 62, 250, Metal3);
        Box(press, "PressUpperBody", 18, 20, 396, 70, Hex("54331F"));
        HazardStrip(press, 22, 76, 388, 12, 24);
        Label(press, "PRENSA DE ENSAMBLAJE A-03", 48, 30, 336, 30, 18, Ivory,
            TextAlignmentOptions.Center, FontStyles.Bold, .9f);

        Box(press, "HydraulicStem", 184, 82, 64, 76, Hex("667174"));
        RectTransform head = Box(press, "PressHead", 124, 144, 184, 58, Copper).rectTransform;
        Box(head, "PressTeethL", 18, 50, 36, 28, CopperBright);
        Box(head, "PressTeethR", 130, 50, 36, 28, CopperBright);
        Box(press, "AssemblyBed", 102, 248, 228, 34, Hex("694127"));
        Box(press, "AssemblyPart", 160, 216, 112, 45, CopperBright);
        Box(press, "AssemblyCore", 196, 225, 40, 26, Amber);

        for (int i = 0; i < 5; i++)
            Box(press, "FeedIndicator_" + i, 78 + i * 58, 304, 24, 12, i < 3 ? Green : Amber);
        Label(press, "SET DE PIEZAS  3 / 5", 98, 319, 236, 22, 14, iColor(3, 5),
            TextAlignmentOptions.Center, FontStyles.Bold, .6f);

        Transform gauge = Panel(press, "PressureGauge", 348, 102, 46, 46, Deep, Edge, 2).transform;
        Box(gauge, "GaugeNeedle", 22, 8, 3, 28, Amber).rectTransform.localRotation = Quaternion.Euler(0, 0, 34f);

        Transform steam = new GameObject("Steam", typeof(RectTransform), typeof(CanvasGroup)).transform;
        steam.SetParent(press, false);
        SetRect((RectTransform)steam, 326, 172, 74, 64);
        for (int i = 0; i < 5; i++)
            Box(steam, "SteamPuff_" + i, 3 + i * 13, (i % 3) * 9, 22, 22,
                new Color(.78f, .84f, .80f, .26f));
    }

    private static Color iColor(int current, int total)
    {
        return current >= total ? Green : Amber;
    }

    private static void AddProductionStations(Transform root)
    {
        string[] names = { "CHASIS", "MOTOR", "HERRAM.", "CONTROL", "REGULADOR" };
        string[] counts = { "03", "02", "01", "04", "00" };
        for (int i = 0; i < names.Length; i++)
            AddProductionMachine(root, 106 + i * 164, 636, i, names[i], counts[i], i == 0);

        for (int i = 0; i < 5; i++)
        {
            float x = 166 + i * 164;
            Box(root, "FeedPipeV_" + i, x, 572, 12, 74, i < 3 ? Hex("704429") : Metal3);
            Image pipe = Box(root, "FeedPipeD_" + i, x, 544 - Mathf.Abs(2 - i) * 21, 12,
                150 + Mathf.Abs(2 - i) * 18, i < 3 ? Hex("704429") : Metal3);
            pipe.rectTransform.localRotation = Quaternion.Euler(0, 0, (2 - i) * 17f);
        }
    }

    private static void AddProductionMachine(Transform root, float x, float y, int index,
        string title, string count, bool selected)
    {
        Transform machine = Panel(root, "Machine_" + title, x, y, 144, 142,
            selected ? Hex("302218") : Hex("182224"), selected ? CopperBright : Edge, selected ? 3 : 2).transform;
        Box(machine, "MachineBase", 12, 102, 120, 24, Metal3);
        Box(machine, "MachineFootL", 20, 124, 24, 12, Deep);
        Box(machine, "MachineFootR", 100, 124, 24, 12, Deep);

        switch (index)
        {
            case 0:
                Box(machine, "StampColumns", 25, 25, 18, 70, Metal3);
                Box(machine, "StampColumns2", 101, 25, 18, 70, Metal3);
                Box(machine, "StampTop", 20, 18, 104, 24, Copper);
                Box(machine, "StampHead", 52, 42, 40, 34, CopperBright);
                Box(machine, "StampPart", 42, 82, 60, 16, Copper);
                break;
            case 1:
                Panel(machine, "MotorWheel", 30, 18, 84, 84, Deep, Cyan, 3);
                Box(machine, "MotorBladeH", 42, 53, 60, 14, Metal3).rectTransform.localRotation = Quaternion.Euler(0, 0, 28f);
                Box(machine, "MotorBladeV", 65, 30, 14, 60, Metal3).rectTransform.localRotation = Quaternion.Euler(0, 0, 28f);
                Box(machine, "MotorHub", 59, 47, 26, 26, CopperBright);
                break;
            case 2:
                Box(machine, "ToolTower", 24, 18, 28, 84, Metal3);
                Box(machine, "ToolArm", 44, 28, 54, 16, Copper);
                Box(machine, "ToolJoint", 88, 24, 28, 28, Amber);
                Image drill = Box(machine, "ToolDrill", 101, 46, 12, 52, CopperBright);
                drill.rectTransform.localRotation = Quaternion.Euler(0, 0, 10f);
                break;
            case 3:
                Box(machine, "ControlScreen", 22, 18, 100, 70, Hex("103336"));
                for (int row = 0; row < 3; row++)
                for (int col = 0; col < 4; col++)
                    Box(machine, "ControlNode_" + row + "_" + col, 32 + col * 22, 29 + row * 18, 10, 7,
                        (row + col) % 3 == 0 ? Green : Cyan);
                Box(machine, "ControlKeys", 34, 92, 76, 8, Metal3);
                break;
            default:
                Box(machine, "CoilFrameL", 25, 20, 16, 78, Metal3);
                Box(machine, "CoilFrameR", 103, 20, 16, 78, Metal3);
                for (int c = 0; c < 5; c++)
                    Box(machine, "Coil_" + c, 42, 25 + c * 14, 60, 8, c % 2 == 0 ? CopperBright : Copper);
                break;
        }

        Transform plate = Panel(machine, "MachinePlate", 8, 108, 128, 28,
            selected ? Hex("4B2D1B") : Deep, selected ? CopperBright : Metal3, 1).transform;
        Label(plate, title, 5, 2, 82, 20, 13, selected ? Ivory : Muted,
            TextAlignmentOptions.Left, FontStyles.Bold, .45f);
        Label(plate, count, 91, 2, 30, 20, 13, selected ? Green : Ivory,
            TextAlignmentOptions.Right, FontStyles.Bold, .45f);
    }

    private static void AddFactoryConveyor(Transform root)
    {
        Box(root, "ConveyorShadow", 78, 796, 852, 42, new Color(0, 0, 0, .72f));
        Box(root, "ConveyorFrame", 72, 782, 860, 40, Metal3);
        Box(root, "ConveyorBelt", 86, 774, 832, 22, Hex("4B5556"));
        for (int i = 0; i < 11; i++)
            Box(root, "ConveyorRoller_" + i, 94 + i * 76, 802, 30, 14, Deep);
        for (int i = 0; i < 4; i++)
        {
            RectTransform marker = Box(root, "ConveyorMarker_" + i, 104 + i * 74, 778, 26, 14,
                CopperBright).rectTransform;
            conveyorMarkers.Add(marker);
        }
        Transform crate = Panel(root, "OutputCrate", 748, 742, 94, 58, Hex("5B351D"), CopperBright, 2).transform;
        Label(crate, "V1", 12, 13, 70, 24, 15, Ivory,
            TextAlignmentOptions.Center, FontStyles.Bold, .6f);
    }

    private static void AddFactoryForeground(Transform root)
    {
        Box(root, "LeftMainPipe", 26, 88, 26, 620, Hex("6B4127"));
        Box(root, "LeftPipeTop", 26, 88, 108, 26, Hex("6B4127"));
        for (int i = 0; i < 5; i++)
            Box(root, "LeftClamp_" + i, 18, 142 + i * 116, 42, 12, Edge);
        Panel(root, "Valve", 7, 420, 64, 64, Deep, CopperBright, 3);
        Box(root, "ValveH", 0, 448, 78, 8, Ivory);
        Box(root, "ValveV", 35, 413, 8, 78, Ivory);

        Transform bot = Box(root, "AutomatonService", 880, 588, 94, 124, new Color(0, 0, 0, 0)).transform;
        Box(bot, "ServiceBotHead", 27, 0, 40, 33, Metal3);
        Box(bot, "ServiceBotEye", 34, 11, 26, 7, Green);
        Box(bot, "ServiceBotBody", 18, 34, 58, 56, Hex("503723"));
        Box(bot, "ServiceBotCore", 35, 49, 24, 24, Amber);
        Box(bot, "ServiceBotArmL", 3, 41, 15, 53, Metal3);
        Box(bot, "ServiceBotArmR", 76, 41, 15, 53, Metal3);
        Box(bot, "ServiceBotLegL", 24, 88, 18, 32, Metal3);
        Box(bot, "ServiceBotLegR", 52, 88, 18, 32, Metal3);
        Label(bot, "MK1", 18, 96, 58, 20, 13, Ivory,
            TextAlignmentOptions.Center, FontStyles.Bold, .6f);

        Box(root, "RightWarning", 956, 72, 38, 496, Hex("101819"));
        for (int i = 0; i < 4; i++)
            Box(root, "RightWarningLamp_" + i, 965, 112 + i * 92, 20, 20, i == 2 ? Amber : Green);
        Label(root, "P\nW\nR", 962, 492, 28, 66, 13, Muted,
            TextAlignmentOptions.Center, FontStyles.Bold, 2f);
    }

    private static void AddIndustrialBackdrop(Transform root)
    {
        for (int i = 0; i < 16; i++)
        {
            Color line = i % 4 == 0 ? new Color(.10f, .15f, .16f, .55f) : new Color(.08f, .12f, .13f, .32f);
            Box(root, "BackdropLine_" + i, 0, 120 + i * 112, Width, 2, line);
        }
        Box(root, "WarmWash", 0, 580, Width, 780, new Color(.22f, .10f, .025f, .07f));
    }

    private static void AddBrickWall(Transform root)
    {
        Box(root, "Wall", 0, 0, 1024, 1038, Hex("101719"));
        for (int y = 0; y < 18; y++)
        {
            float offset = y % 2 == 0 ? 0 : 38;
            Box(root, "BrickH_" + y, 0, y * 58, 1024, 2, new Color(.20f, .27f, .28f, .20f));
            for (int x = 0; x < 15; x++)
                Box(root, "BrickV_" + y + "_" + x, offset + x * 76, y * 58, 2, 58,
                    new Color(.20f, .27f, .28f, .15f));
        }
    }

    private static void AddTowerFrame(Transform root)
    {
        Panel(root, "TowerShell", 122, 24, 770, 984, new Color(.03f, .05f, .05f, .76f), Metal3, 5);
        Box(root, "LeftColumn", 122, 24, 28, 984, Metal3);
        Box(root, "RightColumn", 864, 24, 28, 984, Metal3);
        Box(root, "RoofBeam", 122, 24, 770, 32, Metal3);
        Box(root, "FloorBeam2", 122, 304, 770, 26, Metal3);
        Box(root, "FloorBeam1", 122, 646, 770, 30, Metal3);
        HazardStrip(root, 150, 646, 714, 18, 18);

        for (int i = 0; i < 12; i++)
        {
            Rivet(root, 132, 56 + i * 80);
            Rivet(root, 872, 56 + i * 80);
        }
    }

    private static void AddFloorSelector(Transform root)
    {
        Transform selector = Panel(root, "FloorSelector", 14, 110, 92, 666, Metal, Edge, 2).transform;
        Label(selector, "NIVEL", 5, 16, 82, 24, 16, Muted, TextAlignmentOptions.Center, FontStyles.Bold, 1f);
        FloorBadge(selector, 12, 76, "02", "CTRL", Cyan, false);
        Box(selector, "SelectorRailA", 44, 151, 4, 118, Metal3);
        FloorBadge(selector, 12, 278, "01", "TALL", Amber, false);
        Box(selector, "SelectorRailB", 44, 353, 4, 138, Metal3);
        FloorBadge(selector, 12, 500, "00", "PLTA", CopperBright, true);
    }

    private static void FloorBadge(Transform root, float x, float y, string level, string code, Color accent, bool active)
    {
        Transform badge = Panel(root, "Floor_" + level, x, y, 68, 76,
            active ? Hex("312218") : Deep, active ? accent : Edge, active ? 3 : 2).transform;
        Label(badge, level, 5, 5, 58, 34, 27, accent, TextAlignmentOptions.Center, FontStyles.Bold, 1f);
        Label(badge, code, 3, 42, 62, 20, 13, active ? Ivory : Muted,
            TextAlignmentOptions.Center, FontStyles.Bold, .6f);
    }

    private static void AddControlFloor(Transform root)
    {
        Transform floor = Box(root, "ControlFloor", 150, 58, 714, 246, Hex("111A1C")).transform;
        Label(floor, "02  SALA DE CONTROL", 18, 10, 310, 28, 19, Cyan,
            TextAlignmentOptions.Left, FontStyles.Bold, 1.2f);
        Label(floor, "SINCRONIA  87%", 420, 12, 210, 24, 16, Green,
            TextAlignmentOptions.Right, FontStyles.Bold, .8f);

        Transform window = Panel(floor, "ControlWindow", 18, 48, 240, 170, Hex("102326"), Hex("34565A"), 3).transform;
        Box(window, "WindowGlow", 8, 8, 224, 154, new Color(.08f, .28f, .30f, .28f));
        for (int i = 0; i < 5; i++)
            Box(window, "DistantPipe_" + i, 18 + i * 42, 38 + (i % 2) * 30, 14, 106 - (i % 2) * 30,
                new Color(.24f, .42f, .43f, .48f));
        Box(window, "Catwalk", 8, 124, 224, 11, Metal3);
        for (int i = 0; i < 9; i++)
            Box(window, "WindowLight_" + i, 14 + i * 24, 139, 8, 6, i % 3 == 0 ? Amber : Green);

        Transform console = Panel(floor, "RelayConsole", 282, 50, 276, 166, Metal2, Edge, 2).transform;
        Label(console, "NUCLEO DE RELES", 12, 8, 252, 22, 16, Ivory,
            TextAlignmentOptions.Center, FontStyles.Bold, .8f);
        for (int row = 0; row < 3; row++)
        for (int col = 0; col < 7; col++)
            Box(console, "Relay_" + row + "_" + col, 18 + col * 34, 44 + row * 30, 17, 12,
                (row + col) % 4 == 0 ? Green : ((row + col) % 5 == 0 ? Amber : Hex("334347")));
        Box(console, "ConsoleDesk", 12, 140, 252, 16, Hex("47301F"));

        Transform fan = Panel(floor, "Ventilator", 556, 74, 76, 76, Deep, Edge, 2).transform;
        RectTransform rotor = Box(fan, "FanRotor", 13, 13, 50, 50, Metal3).rectTransform;
        Box(rotor, "FanBladeH", 3, 20, 44, 10, Cyan);
        Box(rotor, "FanBladeV", 20, 3, 10, 44, Cyan);
        Box(fan, "FanHub", 29, 29, 18, 18, CopperBright);
        Label(floor, "VENT. 04", 544, 160, 100, 20, 14, Muted,
            TextAlignmentOptions.Center, FontStyles.Bold, .6f);
    }

    private static void AddWorkshopFloor(Transform root)
    {
        Transform floor = Box(root, "WorkshopFloor", 150, 330, 714, 316, Hex("151B1B")).transform;
        Label(floor, "01  TALLER DE INGENIERIA", 18, 10, 350, 28, 19, Amber,
            TextAlignmentOptions.Left, FontStyles.Bold, 1.2f);
        Label(floor, "BANCO DISPONIBLE", 420, 12, 210, 24, 15, Green,
            TextAlignmentOptions.Right, FontStyles.Bold, .7f);

        Box(floor, "WorkshopLampStem", 344, 42, 10, 64, Metal3);
        Box(floor, "WorkshopLampArm", 344, 42, 126, 10, Metal3);
        Box(floor, "WorkshopLamp", 446, 48, 46, 28, Amber);
        Box(floor, "LampCone", 404, 76, 124, 140, new Color(1f, .62f, .18f, .055f));

        Transform blueprint = Panel(floor, "BlueprintDesk", 20, 62, 208, 204, Hex("102225"), Hex("3C696E"), 2).transform;
        Label(blueprint, "PLANOS V2", 10, 8, 188, 22, 15, Cyan,
            TextAlignmentOptions.Center, FontStyles.Bold, .7f);
        for (int i = 0; i < 5; i++)
            Box(blueprint, "BlueprintH_" + i, 22, 52 + i * 25, 164 - i * 14, 2, new Color(.35f, .73f, .76f, .55f));
        Box(blueprint, "BlueprintBody", 74, 62, 62, 78, new Color(.24f, .65f, .68f, .30f));
        Box(blueprint, "BlueprintAxis", 104, 40, 2, 122, Cyan);
        Label(blueprint, "ESCANEO  42%", 12, 171, 184, 19, 14, Muted,
            TextAlignmentOptions.Center, FontStyles.Bold, .5f);

        Transform bench = Panel(floor, "CalibrationBench", 252, 110, 294, 156, Metal2, Edge, 2).transform;
        Box(bench, "BenchTop", 10, 20, 274, 16, Hex("5B3A24"));
        Box(bench, "ViceLeft", 52, 3, 24, 54, Metal3);
        Box(bench, "ViceRight", 216, 3, 24, 54, Metal3);
        Box(bench, "TestPart", 90, 0, 112, 48, Copper);
        Box(bench, "BenchLegL", 28, 38, 20, 104, Metal3);
        Box(bench, "BenchLegR", 246, 38, 20, 104, Metal3);
        for (int i = 0; i < 3; i++)
        {
            Panel(bench, "Gauge_" + i, 72 + i * 54, 62, 38, 38, Deep, Edge, 2);
            Box(bench, "Needle_" + i, 90 + i * 54, 70, 2, 17, i == 0 ? Green : Amber);
        }
        Label(bench, "METROLOGIA", 64, 112, 166, 20, 14, Ivory,
            TextAlignmentOptions.Center, FontStyles.Bold, .7f);

        Transform shelves = Panel(floor, "ToolRack", 558, 58, 74, 208, Deep, Edge, 2).transform;
        for (int i = 0; i < 4; i++)
        {
            Box(shelves, "Shelf_" + i, 7, 42 + i * 39, 60, 5, Metal3);
            Box(shelves, "Tool_" + i, 16 + (i % 2) * 28, 18 + i * 39, 12, 28, i % 2 == 0 ? CopperBright : Cyan);
        }
        Label(shelves, "UTILES", 6, 178, 62, 18, 13, Muted,
            TextAlignmentOptions.Center, FontStyles.Bold, .6f);
    }

    private static void AddProductionFloor(Transform root)
    {
        Transform floor = Box(root, "ProductionFloor", 150, 676, 714, 308, Hex("181817")).transform;
        Label(floor, "00  PLANTA DE PRODUCCION", 18, 10, 360, 28, 19, CopperBright,
            TextAlignmentOptions.Left, FontStyles.Bold, 1.2f);
        Label(floor, "LINEA A · EN MARCHA", 410, 12, 220, 24, 15, Green,
            TextAlignmentOptions.Right, FontStyles.Bold, .8f);

        string[] feeds = { "CH", "MO", "HE", "CO", "RE" };
        for (int i = 0; i < feeds.Length; i++)
        {
            Transform hopper = Panel(floor, "Hopper_" + feeds[i], 18 + i * 104, 48, 88, 64,
                i == 0 ? Hex("392719") : Metal, i == 0 ? CopperBright : Edge, 2).transform;
            Label(hopper, feeds[i], 5, 8, 78, 24, 18, i == 0 ? CopperBright : Muted,
                TextAlignmentOptions.Center, FontStyles.Bold, .8f);
            Box(hopper, "FeedLamp", 34, 40, 20, 8, i == 0 ? Green : Metal3);
            Box(floor, "Chute_" + i, 54 + i * 104, 112, 16, 48 + i % 2 * 18, i == 0 ? Copper : Metal3);
        }

        Transform press = Panel(floor, "MainPress", 218, 112, 254, 126, Metal2, Edge, 3).transform;
        Box(press, "PressColumnL", 18, 18, 32, 94, Metal3);
        Box(press, "PressColumnR", 204, 18, 32, 94, Metal3);
        Box(press, "PressTop", 10, 8, 234, 32, Hex("55331F"));
        RectTransform pressHead = Box(press, "PressHead", 82, 38, 90, 42, Copper).rectTransform;
        Box(pressHead, "PressTool", 26, 38, 38, 36, CopperBright);
        Box(press, "PressBed", 58, 94, 138, 18, Hex("5A3A26"));
        Label(press, "PRENSA 03", 63, 4, 128, 22, 14, Ivory,
            TextAlignmentOptions.Center, FontStyles.Bold, .7f);

        Transform steam = new GameObject("Steam", typeof(RectTransform), typeof(CanvasGroup)).transform;
        steam.SetParent(press, false);
        SetRect((RectTransform)steam, 170, 46, 66, 42);
        for (int i = 0; i < 4; i++)
            Box(steam, "SteamPuff_" + i, 6 + i * 12, i % 2 * 10, 18, 18, new Color(.75f, .82f, .78f, .30f));

        Box(floor, "ConveyorBase", 20, 248, 656, 32, Metal3);
        Box(floor, "ConveyorBelt", 30, 237, 636, 20, Hex("3D4849"));
        for (int i = 0; i < 8; i++)
            Box(floor, "Roller_" + i, 46 + i * 78, 262, 28, 12, Deep);
        for (int i = 0; i < 4; i++)
        {
            RectTransform marker = Box(floor, "ConveyorMarker_" + i, 38 + i * 74, 240, 24, 14, CopperBright).rectTransform;
            conveyorMarkers.Add(marker);
        }
        Box(floor, "FreshPart", 492, 214, 70, 42, Copper);
        Label(floor, "V1", 505, 220, 44, 22, 14, Deep,
            TextAlignmentOptions.Center, FontStyles.Bold, .5f);

        AddAutomaton(floor, 542, 124);
    }

    private static void AddAutomaton(Transform root, float x, float y)
    {
        Transform bot = Box(root, "AutomatonMK1", x, y, 92, 112, new Color(0, 0, 0, 0)).transform;
        Box(bot, "BotHead", 27, 0, 38, 32, Metal3);
        Box(bot, "BotEye", 33, 10, 26, 7, Green);
        Box(bot, "BotBody", 19, 32, 54, 52, Hex("4A3627"));
        Box(bot, "BotCore", 35, 46, 22, 22, Amber);
        Box(bot, "BotArmL", 4, 39, 15, 48, Metal3);
        Box(bot, "BotArmR", 73, 39, 15, 48, Metal3);
        Box(bot, "BotLegL", 24, 82, 17, 28, Metal3);
        Box(bot, "BotLegR", 51, 82, 17, 28, Metal3);
        Label(bot, "MK1", 14, 88, 64, 18, 13, Ivory,
            TextAlignmentOptions.Center, FontStyles.Bold, .6f);
    }

    private static void AddLiftShaft(Transform root)
    {
        Transform shaft = Panel(root, "LiftShaft", 800, 56, 52, 914, Hex("0B1113"), Edge, 2).transform;
        Box(shaft, "LiftRailL", 7, 12, 5, 890, Metal3);
        Box(shaft, "LiftRailR", 40, 12, 5, 890, Metal3);
        for (int i = 0; i < 18; i++)
            Box(shaft, "LiftTie_" + i, 10, 22 + i * 49, 34, 4, Hex("364347"));
        RectTransform car = Panel(shaft, "LiftCar", 2, 510, 48, 92, Hex("3A2B21"), CopperBright, 2).rectTransform;
        Box(car, "LiftPayload", 10, 16, 28, 42, Copper);
        Label(car, "L-03", 3, 64, 42, 17, 11, Ivory,
            TextAlignmentOptions.Center, FontStyles.Bold, .5f);
        Box(shaft, "LiftCable", 24, 0, 4, 510, Copper);
    }

    private static void AddForegroundPipes(Transform root)
    {
        Box(root, "PipeLeft", 104, 56, 12, 870, Hex("6B4127"));
        Box(root, "PipeTop", 104, 56, 110, 12, Hex("6B4127"));
        Box(root, "PipeValve", 88, 420, 44, 44, CopperBright);
        Box(root, "ValveBarH", 76, 438, 68, 7, Ivory);
        Box(root, "ValveBarV", 106, 408, 7, 68, Ivory);
        for (int i = 0; i < 6; i++)
            Box(root, "PipeClamp_" + i, 98, 102 + i * 142, 24, 9, Edge);

        Transform status = Panel(root, "StatusStack", 906, 120, 94, 620, Metal, Edge, 2).transform;
        Label(status, "RED", 6, 14, 82, 22, 15, Muted,
            TextAlignmentOptions.Center, FontStyles.Bold, .7f);
        string[] labels = { "PWR", "TMP", "MAT", "SYNC", "CORE" };
        for (int i = 0; i < labels.Length; i++)
        {
            Label(status, labels[i], 8, 64 + i * 96, 78, 18, 13, Muted,
                TextAlignmentOptions.Center, FontStyles.Bold, .5f);
            Box(status, "StatusLamp_" + i, 32, 86 + i * 96, 30, 30,
                i == 3 ? Amber : Green);
            Box(status, "StatusTick_" + i, 20, 124 + i * 96, 54, 5,
                i == 3 ? Amber : Cyan);
        }
    }

    private static void OperatorConsole(Transform root)
    {
        Transform console = Panel(root, "OperatorConsole", 28, 1310, 1024, 390, Metal, Edge, 3).transform;
        Box(console, "ConsoleTopStrip", 0, 0, 1024, 14, Copper);
        Label(console, "PUPITRE DE LINEA A", 22, 22, 300, 28, 18, Muted,
            TextAlignmentOptions.Left, FontStyles.Bold, 1f);
        Label(console, "CHASIS · VERSION 1", 22, 54, 470, 50, 31, Ivory,
            TextAlignmentOptions.Left, FontStyles.Bold, 1.6f);
        Label(console, "TROQUEL INSTALADO", 24, 101, 260, 24, 15, Green,
            TextAlignmentOptions.Left, FontStyles.Bold, .7f);

        Transform schematic = Panel(console, "PartSchematic", 22, 136, 300, 164, Deep, Hex("3B5D61"), 2).transform;
        Box(schematic, "SchematicAxisH", 26, 80, 248, 2, new Color(.38f, .75f, .78f, .45f));
        Box(schematic, "SchematicAxisV", 149, 20, 2, 124, new Color(.38f, .75f, .78f, .45f));
        Box(schematic, "ChassisMain", 78, 42, 144, 78, new Color(.30f, .70f, .72f, .28f));
        Panel(schematic, "ChassisInner", 96, 58, 108, 46, Deep, Cyan, 2);
        Label(schematic, "PLACA  V1-CH-001", 28, 132, 244, 20, 13, Cyan,
            TextAlignmentOptions.Center, FontStyles.Bold, .5f);

        Transform stats = Panel(console, "ProductionStats", 346, 136, 300, 164, Deep, Metal3, 2).transform;
        StatRow(stats, 14, "COSTE", "100 LE + 1 TRAZA", Amber);
        StatRow(stats, 58, "TIEMPO", "10 SEG", Ivory);
        StatRow(stats, 102, "ALMACEN", "3 / 25", Green);

        Transform amount = Panel(console, "Amount", 682, 46, 316, 80, Deep, Metal3, 2).transform;
        ButtonPlate(amount, "Minus", 10, 10, 58, 58, "-", 30, Metal2, Edge);
        Label(amount, "LOTE  01", 78, 12, 160, 52, 23, Ivory,
            TextAlignmentOptions.Center, FontStyles.Bold, 1f);
        ButtonPlate(amount, "Plus", 248, 10, 58, 58, "+", 28, Metal2, Edge);

        ButtonPlate(console, "Produce", 682, 144, 316, 86, "FABRICAR PIEZA", 25, Hex("6A3C20"), CopperBright);
        ButtonPlate(console, "Assemble", 682, 246, 316, 54, "ENSAMBLAJE MK1", 19, Metal2, Edge);

        Box(console, "ProgressBg", 22, 326, 976, 28, Deep);
        Box(console, "ProgressFill", 26, 330, 432, 20, CopperBright);
        Label(console, "ORDEN ACTIVA · PRENSADO  46%", 32, 329, 938, 20, 15, Ivory,
            TextAlignmentOptions.Center, FontStyles.Bold, .7f);
        HazardStrip(console, 22, 366, 976, 10, 22);
    }

    private static void BottomNavigation(Transform root)
    {
        Transform nav = Panel(root, "BottomNavigation", 28, 1724, 1024, 160, Deep, Metal3, 2).transform;
        NavButton(nav, 12, "00", "PLANTA", CopperBright, true);
        NavButton(nav, 344, "01", "TALLER", Amber, false);
        NavButton(nav, 676, "02", "CONTROL", Cyan, false);
        Label(nav, "EL MONTACARGAS CONECTA LOS TRES NIVELES DEL COMPLEJO", 132, 122, 760, 22,
            14, Muted, TextAlignmentOptions.Center, FontStyles.Bold, .7f);
    }

    private static void NavButton(Transform root, float x, string level, string title, Color accent, bool active)
    {
        Transform button = Panel(root, "Nav_" + title, x, 12, 324, 98,
            active ? Hex("30241B") : Metal, active ? accent : Edge, active ? 3 : 2).transform;
        Box(button, "LevelPlate", 16, 16, 54, 54, active ? accent : Metal3);
        Label(button, level, 18, 22, 50, 36, 23, active ? Deep : Muted,
            TextAlignmentOptions.Center, FontStyles.Bold, 1f);
        Label(button, title, 86, 17, 216, 34, 23, active ? Ivory : Muted,
            TextAlignmentOptions.Left, FontStyles.Bold, 1.2f);
        Label(button, active ? "EN OPERACION" : "SUBIR NIVEL", 86, 53, 216, 22, 14,
            active ? Green : accent, TextAlignmentOptions.Left, FontStyles.Bold, .6f);
    }

    private static void Resource(Transform root, float x, string title, string value, Color accent)
    {
        Box(root, "ResourceLamp_" + title, x, 28, 20, 20, accent);
        Label(root, title, x + 32, 10, 124, 23, 15, Muted,
            TextAlignmentOptions.Left, FontStyles.Bold, .7f);
        Label(root, value, x + 32, 34, 230, 32, 22, Ivory,
            TextAlignmentOptions.Left, FontStyles.Bold, 1f);
    }

    private static void StatRow(Transform root, float y, string title, string value, Color accent)
    {
        Box(root, "StatMarker_" + title, 12, y + 10, 12, 12, accent);
        Label(root, title, 34, y, 86, 30, 15, Muted,
            TextAlignmentOptions.Left, FontStyles.Bold, .7f);
        Label(root, value, 116, y, 170, 30, 16, accent,
            TextAlignmentOptions.Right, FontStyles.Bold, .7f);
        if (y < 100) Box(root, "StatLine_" + title, 12, y + 34, 274, 2, Metal3);
    }

    private static void ButtonPlate(Transform root, string name, float x, float y, float w, float h,
        string text, float size, Color fill, Color border)
    {
        Transform button = Panel(root, name, x, y, w, h, fill, border, 3).transform;
        Label(button, text, 6, 4, w - 12, h - 8, size, Ivory,
            TextAlignmentOptions.Center, FontStyles.Bold, 1.1f);
        Rivet(button, 8, 8);
        Rivet(button, w - 14, 8);
        Rivet(button, 8, h - 14);
        Rivet(button, w - 14, h - 14);
    }

    private static void HazardStrip(Transform root, float x, float y, float w, float h, float step)
    {
        Image baseStrip = Box(root, "HazardBase", x, y, w, h, Hex("15191A"));
        int count = Mathf.CeilToInt(w / step);
        for (int i = 0; i < count; i++)
        {
            Image stripe = Box(baseStrip.transform, "Hazard_" + i, i * step, -h * .55f, step * .42f, h * 2.1f,
                i % 2 == 0 ? Amber : Copper);
            stripe.rectTransform.localRotation = Quaternion.Euler(0, 0, -28f);
        }
        baseStrip.gameObject.AddComponent<RectMask2D>();
    }

    private static Image Panel(Transform parent, string name, float x, float y, float w, float h,
        Color fill, Color border, float borderWidth)
    {
        Image outer = Box(parent, name, x, y, w, h, border);
        if (borderWidth <= 0) return outer;
        Image inner = Box(outer.transform, name + "_Fill", borderWidth, borderWidth,
            w - borderWidth * 2, h - borderWidth * 2, fill);
        inner.transform.SetAsFirstSibling();
        return outer;
    }

    private static Image Box(Transform parent, string name, float x, float y, float w, float h, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        RectTransform rect = go.GetComponent<RectTransform>();
        SetRect(rect, x, y, w, h);
        Image image = go.GetComponent<Image>();
        image.sprite = uiSprite;
        image.type = uiSprite != null ? Image.Type.Sliced : Image.Type.Simple;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static TextMeshProUGUI Label(Transform parent, string name, float x, float y, float w, float h,
        float size, Color color, TextAlignmentOptions alignment, FontStyles style, float spacing)
    {
        GameObject go = new GameObject("Text_" + name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        RectTransform rect = go.GetComponent<RectTransform>();
        SetRect(rect, x, y, w, h);
        TextMeshProUGUI label = go.GetComponent<TextMeshProUGUI>();
        label.font = font;
        label.text = name;
        label.fontSize = size;
        label.color = color;
        label.alignment = alignment;
        label.fontStyle = style;
        label.characterSpacing = spacing;
        label.textWrappingMode = TextWrappingModes.NoWrap;
        label.overflowMode = TextOverflowModes.Truncate;
        label.raycastTarget = false;
        return label;
    }

    private static void Separator(Transform root, float x, float y, float w, float h, Color color)
    {
        Box(root, "Separator", x, y, w, h, color);
    }

    private static void Rivet(Transform root, float x, float y)
    {
        Box(root, "Rivet", x, y, 6, 6, Hex("8B8F84"));
    }

    private static void SetRect(RectTransform rect, float x, float y, float w, float h)
    {
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(x, -y);
        rect.sizeDelta = new Vector2(w, h);
        rect.localScale = Vector3.one;
    }

    private static Transform FindDeep(Transform root, string name)
    {
        foreach (Transform child in root)
        {
            if (child.name == name) return child;
            Transform nested = FindDeep(child, name);
            if (nested != null) return nested;
        }
        return null;
    }

    private static RectTransform FindRect(Transform root, string name)
    {
        Transform found = FindDeep(root, name);
        return found != null ? found.GetComponent<RectTransform>() : null;
    }

    private static Image FindImage(Transform root, string name)
    {
        Transform found = FindDeep(root, name);
        return found != null ? found.GetComponent<Image>() : null;
    }

    private static void Render(Camera camera, string outputPath)
    {
        RenderTexture target = new RenderTexture(Width, Height, 24, RenderTextureFormat.ARGB32);
        RenderTexture previousTarget = camera.targetTexture;
        RenderTexture previousActive = RenderTexture.active;
        try
        {
            camera.targetTexture = target;
            Canvas.ForceUpdateCanvases();
            camera.Render();
            RenderTexture.active = target;
            Texture2D image = new Texture2D(Width, Height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
            image.Apply();
            File.WriteAllBytes(outputPath, image.EncodeToPNG());
            Object.DestroyImmediate(image);
        }
        finally
        {
            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            target.Release();
            Object.DestroyImmediate(target);
        }
    }

    private static Color Hex(string value)
    {
        return ColorUtility.TryParseHtmlString("#" + value, out Color color) ? color : Color.magenta;
    }
}

[InitializeOnLoad]
internal static class Dimension3FactoryTowerPrototypeAutoRun
{
    private const string RequestPath = "Temp/Dimension3FactoryTowerPrototype.request";

    static Dimension3FactoryTowerPrototypeAutoRun()
    {
        if (!Application.isBatchMode && File.Exists(RequestPath))
            EditorApplication.delayCall += TryRun;
    }

    private static void TryRun()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorApplication.delayCall += TryRun;
            return;
        }

        try
        {
            Dimension3FactoryTowerPrototypeSetup.BuildAndCapture();
            File.Delete(RequestPath);
            Debug.Log("[D3 Factory Prototype] Solicitud completada desde la instancia abierta.");
        }
        catch (System.Exception exception)
        {
            Debug.LogException(exception);
        }
    }
}
#endif

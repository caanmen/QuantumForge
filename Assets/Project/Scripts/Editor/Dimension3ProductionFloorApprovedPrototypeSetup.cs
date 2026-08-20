#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Builds a pixel-faithful, isolated Unity approval scene from the accepted
/// Dimension 3 production-floor mockup. Main.unity is never opened or edited.
/// </summary>
public static class Dimension3ProductionFloorApprovedPrototypeSetup
{
    private const int CanvasWidth = 1080;
    private const int CanvasHeight = 1920;
    private const int SourceWidth = 983;
    private const int SourceHeight = 1600;

    private const string ImagePath =
        "Assets/Project/Art/Dimension3/ApprovedPrototype/D3_ProductionFloor_ApprovedMockup_v1.png";
    private const string ScenePath =
        "Assets/Project/Scenes/Dimension3ProductionFloorApprovedPrototype.unity";
    private const string CapturePath =
        "Logs/VisualQA/Dimension3ApprovedPrototype/D3_ProductionFloor_Approved_Unity_1080x1920.png";

    [MenuItem("Tools/Quantum Forge/Dimension 3/Build Approved Production Floor Prototype")]
    public static void BuildAndCapture()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(CapturePath));
        ConfigureTextureImport();

        Texture2D design = AssetDatabase.LoadAssetAtPath<Texture2D>(ImagePath);
        if (design == null)
            throw new System.InvalidOperationException("Approved Dimension 3 mockup was not found.");

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        Camera camera = CreateCamera();
        Canvas canvas = CreateCanvas(camera);
        BuildScreen(canvas.transform, design);

        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Render(camera, Path.GetFullPath(CapturePath));
        Debug.Log("[D3 Approved Prototype] Scene: " + ScenePath);
        Debug.Log("[D3 Approved Prototype] Capture: " + Path.GetFullPath(CapturePath));
    }

    private static void ConfigureTextureImport()
    {
        AssetDatabase.ImportAsset(ImagePath, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter importer = AssetImporter.GetAtPath(ImagePath) as TextureImporter;
        if (importer == null) return;

        bool dirty = importer.textureType != TextureImporterType.Default ||
                     importer.textureCompression != TextureImporterCompression.Uncompressed ||
                     importer.mipmapEnabled || importer.maxTextureSize != 2048 ||
                     importer.filterMode != FilterMode.Bilinear;
        importer.textureType = TextureImporterType.Default;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.maxTextureSize = 2048;
        importer.filterMode = FilterMode.Bilinear;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.sRGBTexture = true;
        if (dirty) importer.SaveAndReimport();
    }

    private static Camera CreateCamera()
    {
        GameObject go = new GameObject("D3ApprovedPrototypeCamera", typeof(Camera));
        Camera camera = go.GetComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Hex("050706");
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.transform.position = new Vector3(0f, 0f, -10f);
        return camera;
    }

    private static Canvas CreateCanvas(Camera camera)
    {
        GameObject go = new GameObject("D3ApprovedProductionFloorCanvas",
            typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = camera;
        canvas.planeDistance = 1f;

        CanvasScaler scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(CanvasWidth, CanvasHeight);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = .5f;
        return canvas;
    }

    private static void BuildScreen(Transform root, Texture2D design)
    {
        Image matte = CreateImage(root, "PrototypeMatte", 0, 0, CanvasWidth, CanvasHeight,
            Hex("050706"), true);
        AddMatteDetails(matte.transform);

        const float designWidth = 1080f;
        float designHeight = designWidth * SourceHeight / SourceWidth;
        float designY = (CanvasHeight - designHeight) * .5f;

        GameObject plateGo = new GameObject("ApprovedDesignPlate",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
        plateGo.transform.SetParent(root, false);
        RectTransform plateRect = plateGo.GetComponent<RectTransform>();
        SetRect(plateRect, 0f, designY, designWidth, designHeight);
        RawImage plate = plateGo.GetComponent<RawImage>();
        plate.texture = design;
        plate.color = Color.white;
        plate.raycastTarget = false;

        Transform semantic = CreateRect(root, "SemanticInteractionLayer", 0f, designY,
            designWidth, designHeight);
        BuildHotspots(semantic);
        BuildFutureAnimationSlots(semantic);

        Transform metadata = CreateRect(root, "PrototypeMetadata_Disabled", 0, 0, 1, 1);
        metadata.gameObject.SetActive(false);
    }

    private static void AddMatteDetails(Transform root)
    {
        CreateImage(root, "TopSafeAreaLine", 18, 72, 1044, 2, Hex("2C302D"), false);
        CreateImage(root, "BottomSafeAreaLine", 18, 1846, 1044, 2, Hex("2C302D"), false);
    }

    private static void BuildHotspots(Transform root)
    {
        // Header and resources.
        Hotspot(root, "Menu", 28, 28, 122, 82, "Open D3 contextual menu");
        Hotspot(root, "Resource_LE", 62, 112, 286, 76, "GameState.LE display");
        Hotspot(root, "Resource_Traces", 365, 112, 262, 76, "GameState.Traces display");
        Hotspot(root, "Resource_Automatons", 642, 112, 292, 76,
            "Dimension3State automatons summary");

        // Five real part IDs shown as die selectors.
        Hotspot(root, "Part_Chassis", 194, 738, 124, 146,
            "Dimension3Catalog.PartChassis / QueuePartProduction");
        Hotspot(root, "Part_Motor", 318, 738, 116, 146,
            "Dimension3Catalog.PartMotor / QueuePartProduction");
        Hotspot(root, "Part_Tool", 434, 738, 118, 146,
            "Dimension3Catalog.PartTool / QueuePartProduction");
        Hotspot(root, "Part_Control", 552, 738, 118, 146,
            "Dimension3Catalog.PartControl / QueuePartProduction");
        Hotspot(root, "Part_Regulator", 670, 738, 130, 146,
            "Dimension3Catalog.PartRegulator / QueuePartProduction");

        // Four job queues.
        Hotspot(root, "Queue_Parts", 38, 902, 252, 132,
            "D3QueueIds.PartProduction / open Queues drawer");
        Hotspot(root, "Queue_Assembly", 290, 902, 218, 132,
            "D3QueueIds.Assembly / open Queues drawer");
        Hotspot(root, "Queue_Research", 508, 902, 220, 132,
            "D3QueueIds.Research / open Queues drawer");
        Hotspot(root, "Queue_Facilities", 728, 902, 220, 132,
            "D3QueueIds.Facilities / open Queues drawer");

        // Contextual production controls.
        Hotspot(root, "Blueprint_SelectedPart", 64, 1118, 340, 326,
            "Selected part/version preview");
        Hotspot(root, "Production_Version", 64, 1048, 340, 74,
            "productionVersionDropdown V1-V6");
        Hotspot(root, "Production_QuantityMinus", 434, 1268, 132, 74,
            "productionQuantityDropdown previous option");
        Hotspot(root, "Production_QuantityPlus", 794, 1268, 132, 74,
            "productionQuantityDropdown next option");
        Hotspot(root, "Production_Fabricate", 424, 1350, 248, 92,
            "Dimension3PanelUI.QueuePart(selectedPartId)");
        Hotspot(root, "Production_AssembleMK1", 678, 1350, 250, 92,
            "Dimension3PanelUI.QueueAssembly");

        // D3 room navigation.
        Hotspot(root, "Nav_ProductionFloor", 22, 1484, 316, 94,
            "Dimension3 screen: Factory / Production Floor");
        Hotspot(root, "Nav_EngineeringWorkshop", 338, 1484, 302, 94,
            "Dimension3 screen: Engineering Workshop");
        Hotspot(root, "Nav_ControlRoom", 640, 1484, 320, 94,
            "Dimension3 screen: Control Room");
    }

    private static void BuildFutureAnimationSlots(Transform root)
    {
        Transform slots = CreateRect(root, "FutureAnimationSlots", 0, 0, SourceWidth, SourceHeight);
        slots.gameObject.SetActive(false);

        Slot(slots, "Anim_ConveyorAndBlanks", 18, 580, 310, 142,
            "Later: belt UV/marker motion and blank translation");
        Slot(slots, "Anim_PressHead", 338, 270, 326, 448,
            "Later: hydraulic press head vertical cycle");
        Slot(slots, "Anim_PressGlow", 390, 582, 184, 108,
            "Later: glow, sparks and short steam burst");
        Slot(slots, "Anim_AutomatonMK1", 710, 424, 212, 302,
            "Later: idle head/arm movement and status light");
        Slot(slots, "Anim_StatusLights", 188, 712, 620, 56,
            "Later: active die and machine state lamps");
    }

    private static void Hotspot(Transform root, string id, float x, float y, float w, float h,
        string binding)
    {
        Vector4 rect = ScaleSourceRect(x, y, w, h);
        Image image = CreateImage(root, "Hotspot_" + id, rect.x, rect.y, rect.z, rect.w,
            new Color(1f, 1f, 1f, .001f), false);
        image.raycastTarget = true;
        image.gameObject.AddComponent<Button>().transition = Selectable.Transition.None;
        Dimension3ApprovedPrototypeHotspot marker =
            image.gameObject.AddComponent<Dimension3ApprovedPrototypeHotspot>();
        marker.semanticId = id;
        marker.futureBinding = binding;
        marker.sourcePixelRect = new Rect(x, y, w, h);
    }

    private static void Slot(Transform root, string id, float x, float y, float w, float h,
        string binding)
    {
        Transform rect = CreateRect(root, id, x, y, w, h);
        Dimension3ApprovedPrototypeHotspot marker =
            rect.gameObject.AddComponent<Dimension3ApprovedPrototypeHotspot>();
        marker.semanticId = id;
        marker.futureBinding = binding;
        marker.sourcePixelRect = new Rect(x, y, w, h);
    }

    private static Vector4 ScaleSourceRect(float x, float y, float w, float h)
    {
        float scale = 1080f / SourceWidth;
        return new Vector4(x * scale, y * scale, w * scale, h * scale);
    }

    private static Image CreateImage(Transform parent, string name, float x, float y, float w,
        float h, Color color, bool raycast)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        SetRect(go.GetComponent<RectTransform>(), x, y, w, h);
        Image image = go.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = raycast;
        return image;
    }

    private static Transform CreateRect(Transform parent, string name, float x, float y, float w,
        float h)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        SetRect(go.GetComponent<RectTransform>(), x, y, w, h);
        return go.transform;
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

    private static void Render(Camera camera, string outputPath)
    {
        RenderTexture target = new RenderTexture(CanvasWidth, CanvasHeight, 24,
            RenderTextureFormat.ARGB32);
        RenderTexture previousTarget = camera.targetTexture;
        RenderTexture previousActive = RenderTexture.active;
        try
        {
            camera.targetTexture = target;
            Canvas.ForceUpdateCanvases();
            camera.Render();
            RenderTexture.active = target;
            Texture2D image = new Texture2D(CanvasWidth, CanvasHeight, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, CanvasWidth, CanvasHeight), 0, 0);
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
#endif

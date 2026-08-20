#if UNITY_EDITOR
using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

/// <summary>
/// Builds a self-contained visual prototype for Dimension 2 / Civilization Pacts.
/// Main.unity is never opened, saved, or modified by this builder.
/// </summary>
public static class Dimension2PactPrototypeSetup
{
    private const int Width = 1080;
    private const int Height = 1920;
    private const string ScenePath = "Assets/Project/Scenes/Dimension2PactPrototype.unity";
    private const string CapturePath = "Logs/VisualQA/Dimension2Prototype/D2_PactosCivilizacion_ApprovedArt_1080x1920.png";
    private const string ArtRoot = "Assets/Project/UI/Dimension2/Pacts/Generated/";
    private const string CinzelSourcePath = "Assets/Project/Fonts/Cinzel/Cinzel-VariableFont_wght.ttf";
    private const string CinzelAssetPath = "Assets/Project/Fonts/Cinzel/Cinzel SDF.asset";

    private static readonly Color Deep = Hex("090D0C");
    private static readonly Color Panel = Hex("111713");
    private static readonly Color Bronze = Hex("735735");
    private static readonly Color BronzeDark = Hex("382D20");
    private static readonly Color Gold = Hex("C49A58");
    private static readonly Color GoldBright = Hex("E1B869");
    private static readonly Color Ivory = Hex("D6C3A0");
    private static readonly Color Muted = Hex("9B896C");
    private static readonly Color Green = Hex("90A85D");
    private static readonly Color Ember = Hex("B9773E");

    private static TMP_FontAsset cinzel;

    [MenuItem("Tools/Quantum Forge/Dimension 2/Build Pact Visual Prototype")]
    public static void BuildAndCapture()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(CapturePath));
        ImportArt();
        cinzel = CreateOrLoadCinzel();

        Scene previousScene = SceneManager.GetActiveScene();
        bool additive = !Application.isBatchMode && previousScene.IsValid() && !string.IsNullOrEmpty(previousScene.path);
        Scene scene = EditorSceneManager.NewScene(
            NewSceneSetup.EmptyScene,
            additive ? NewSceneMode.Additive : NewSceneMode.Single);

        try
        {
            SceneManager.SetActiveScene(scene);
            Camera camera = CreateCamera();
            Canvas canvas = CreateCanvas(camera);
            BuildScreen(canvas.transform);

            if (!EditorSceneManager.SaveScene(scene, ScenePath))
                throw new InvalidOperationException("No se pudo guardar la escena de Pactos de Dimensión 2.");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Render(camera, Path.GetFullPath(CapturePath));
            Debug.Log("[D2 Pact Prototype] Scene: " + ScenePath);
            Debug.Log("[D2 Pact Prototype] Capture: " + Path.GetFullPath(CapturePath));
        }
        finally
        {
            if (additive && previousScene.IsValid() && previousScene.isLoaded)
                SceneManager.SetActiveScene(previousScene);
            if (additive && scene.IsValid() && scene.isLoaded)
                EditorSceneManager.CloseScene(scene, true);
        }
    }

    private static void ImportArt()
    {
        string[] paths = Directory.GetFiles(Path.GetFullPath(ArtRoot), "*.png", SearchOption.TopDirectoryOnly);
        foreach (string absolutePath in paths)
        {
            string assetPath = absolutePath.Replace('\\', '/');
            int assetsIndex = assetPath.IndexOf("Assets/", StringComparison.Ordinal);
            if (assetsIndex < 0) continue;
            assetPath = assetPath.Substring(assetsIndex);

            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null) continue;

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.sRGBTexture = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = 4096;
            importer.SaveAndReimport();
        }
    }

    private static TMP_FontAsset CreateOrLoadCinzel()
    {
        TMP_FontAsset asset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(CinzelAssetPath);
        if (asset == null)
        {
            AssetDatabase.ImportAsset(CinzelSourcePath, ImportAssetOptions.ForceSynchronousImport);
            Font source = AssetDatabase.LoadAssetAtPath<Font>(CinzelSourcePath);
            if (source == null)
                throw new InvalidOperationException("No se encontró la fuente Cinzel del prototipo.");

            asset = TMP_FontAsset.CreateFontAsset(
                source, 96, 10, GlyphRenderMode.SDFAA, 2048, 2048,
                AtlasPopulationMode.Dynamic, true);
            if (asset == null)
                throw new InvalidOperationException("No se pudo crear el atlas TMP de Cinzel.");

            asset.name = "Cinzel SDF";
            AssetDatabase.CreateAsset(asset, CinzelAssetPath);
            if (asset.material != null && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(asset.material)))
            {
                asset.material.name = "Cinzel Atlas Material";
                AssetDatabase.AddObjectToAsset(asset.material, asset);
            }
            if (asset.atlasTexture != null && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(asset.atlasTexture)))
            {
                asset.atlasTexture.name = "Cinzel Atlas";
                AssetDatabase.AddObjectToAsset(asset.atlasTexture, asset);
            }
        }

        asset.TryAddCharacters(
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ÁÉÍÓÚÜÑ/%+-.,:<> ", out _);
        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();
        return asset;
    }

    private static Camera CreateCamera()
    {
        GameObject go = new GameObject("D2PactPrototypeCamera", typeof(Camera));
        Camera camera = go.GetComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Deep;
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.transform.position = new Vector3(0f, 0f, -10f);
        return camera;
    }

    private static Canvas CreateCanvas(Camera camera)
    {
        GameObject go = new GameObject(
            "Dimension2PactPrototypeCanvas",
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

    private static void BuildScreen(Transform canvas)
    {
        Image background = Artwork(canvas, "StoneBackdrop", ArtRoot + "PactsScreen_Background.png", 0, 0, Width, Height);
        background.color = new Color(.68f, .71f, .63f, 1f);

        CanvasGroup content = new GameObject("AnimatedContent", typeof(RectTransform), typeof(CanvasGroup)).GetComponent<CanvasGroup>();
        content.transform.SetParent(canvas, false);
        SetRect(content.GetComponent<RectTransform>(), 0, 0, Width, Height);

        BuildHeader(content.transform);
        BuildCards(content.transform);
        BuildDoorAndSlots(content.transform, out RectTransform selectedMedallion, out Image selectedAura);
        BuildDetail(content.transform);
        BuildNavigation(content.transform);
        AddOuterRail(content.transform);

        Dimension2PactPrototypeUI animation = canvas.gameObject.AddComponent<Dimension2PactPrototypeUI>();
        SerializedObject serialized = new SerializedObject(animation);
        serialized.FindProperty("content").objectReferenceValue = content;
        serialized.FindProperty("selectedMedallion").objectReferenceValue = selectedMedallion;
        serialized.FindProperty("selectedAura").objectReferenceValue = selectedAura;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void BuildHeader(Transform root)
    {
        Box(root, "HeaderShade", 8, 8, 1064, 108, new Color(.025f, .035f, .031f, .94f));
        Line(root, 8, 110, 1064, 3, Bronze);
        Line(root, 20, 116, 1040, 1, BronzeDark);
        Box(root, "BackPlate", 22, 24, 72, 68, new Color(.04f, .06f, .052f, .95f), Bronze, 2);
        Label(root, "<", 22, 12, 72, 80, 54, Gold, TextAlignmentOptions.Center, FontStyles.Normal, 0f);
        Label(root, "PACTOS DE CIVILIZACIÓN", 112, 23, 856, 64, 43, Gold,
            TextAlignmentOptions.Center, FontStyles.Normal, 2.4f);

        Transform resources = Box(root, "Resources", 10, 126, 1060, 92,
            new Color(.035f, .05f, .043f, .93f), BronzeDark, 2).transform;
        Resource(resources, 10, "Icon_Confidence.png", "CONFIANZA", "12,580");
        Resource(resources, 360, "Icon_Acolyte.png", "ACÓLITOS", "1,280");
        Resource(resources, 710, "Icon_Offering.png", "CERA/PAN", "3,450");
        Line(resources, 350, 10, 1, 70, BronzeDark);
        Line(resources, 700, 10, 1, 70, BronzeDark);
    }

    private static void Resource(Transform root, float x, string iconFile, string title, string value)
    {
        Artwork(root, "Resource_" + title, ArtRoot + iconFile, x + 12, 14, 54, 54).color = new Color(1f, .88f, .58f, .92f);
        Label(root, title, x + 76, 10, 230, 28, 18, Muted,
            TextAlignmentOptions.Left, FontStyles.Normal, 1.6f);
        Label(root, value, x + 78, 43, 230, 34, 24, Gold,
            TextAlignmentOptions.Left, FontStyles.Normal, 1.2f);
    }

    private static void BuildCards(Transform root)
    {
        Artwork(root, "PactCardGrid", ArtRoot + "PactCardGrid_Background.png", 14, 230, 1052, 613);

        Pact(root, "Hospedaje", "Medallion_Hospitality.png", 145, 242, 235, 26, 444, 512, true);
        Pact(root, "Camino abierto", "Medallion_Road.png", 685, 242, 235, 540, 444, 512, false);
        Pact(root, "Consagración", "Medallion_Candle.png", 153, 548, 220, 26, 748, 512, false);
        Pact(root, "Voto silencioso", "Medallion_SilentVow.png", 690, 548, 225, 540, 748, 512, false);
    }

    private static void Pact(
        Transform root, string title, string iconFile,
        float iconX, float iconY, float iconSize,
        float labelX, float labelY, float labelW, bool selected)
    {
        Image icon = Artwork(root, "Pact_" + title, ArtRoot + iconFile, iconX, iconY, iconSize, iconSize);
        icon.color = selected ? Color.white : new Color(.72f, .69f, .59f, .92f);
        Label(root, title.ToUpperInvariant(), labelX, labelY, labelW, 60, 42,
            selected ? GoldBright : Ivory, TextAlignmentOptions.Center,
            FontStyles.Normal, 1.1f);
    }

    private static void BuildDoorAndSlots(Transform root, out RectTransform selectedMedallion, out Image selectedAura)
    {
        Artwork(root, "DoorAndActiveSlots", ArtRoot + "DoorAndSlots_Background.png", 79, 870, 922, 476);

        Artwork(root, "DoorMedallion", ArtRoot + "Medallion_Door.png", 300, 907, 201, 201)
            .color = new Color(.84f, .78f, .65f, .96f);
        Label(root, "PUERTA INTERIOR", 442, 964, 470, 72, 36, Ivory,
            TextAlignmentOptions.Center, FontStyles.Normal, 1.2f);

        Box(root, "DividerMask", 322, 1110, 436, 43, new Color(.025f, .038f, .033f, .96f));
        Label(root, "ESPACIOS ACTIVOS  1/2", 286, 1111, 508, 43, 27, Gold,
            TextAlignmentOptions.Center, FontStyles.Normal, 1.8f);
        Diamond(root, 302, 1130, 10, Gold);
        Diamond(root, 767, 1130, 10, Gold);

        selectedAura = Artwork(root, "SelectedSlotAura", ArtRoot + "Medallion_Hospitality.png", 190, 1143, 204, 204);
        selectedAura.color = new Color(1f, .72f, .3f, .12f);
        Image selected = Artwork(root, "SelectedHospitality", ArtRoot + "Medallion_Hospitality.png", 198, 1151, 188, 188);
        selectedMedallion = selected.rectTransform;
        Artwork(root, "LockedSlot", ArtRoot + "Medallion_Lock.png", 686, 1164, 164, 164)
            .color = new Color(.63f, .61f, .54f, .88f);
    }

    private static void BuildDetail(Transform root)
    {
        Artwork(root, "PactDetailPanel", ArtRoot + "PactDetail_Background.png", 12, 1350, 1056, 415);
        Artwork(root, "DetailSun", ArtRoot + "Icon_SunCompass.png", 40, 1371, 74, 74);
        Artwork(root, "DetailHospitality", ArtRoot + "Medallion_Hospitality.png", 948, 1374, 70, 70);
        Label(root, "HOSPEDAJE", 244, 1382, 592, 52, 38, Gold,
            TextAlignmentOptions.Center, FontStyles.Normal, 2f);

        DetailRow(root, 1474, "Icon_Shield.png", "BENEFICIO", "+15% DEFENSA DEL REFUGIO", Green);
        DetailRow(root, 1551, "Icon_Offering.png", "COMPROMISO", "-5% CONFIANZA / HORA", Ember);
        DetailRow(root, 1628, "Icon_Devotee.png", "ESTADO", "ACTIVO", Green);
        Label(root, "ACTIVAR", 318, 1688, 444, 54, 34, Ivory,
            TextAlignmentOptions.Center, FontStyles.Normal, 2.2f);
    }

    private static void DetailRow(Transform root, float y, string iconFile, string label, string value, Color valueColor)
    {
        Label(root, label, 52, y + 8, 210, 34, 19, Muted,
            TextAlignmentOptions.Left, FontStyles.Normal, 1.7f);
        Artwork(root, "Detail_" + label, ArtRoot + iconFile, 280, y + 2, 46, 46);
        Label(root, value, 354, y + 7, 620, 36, 21, valueColor,
            TextAlignmentOptions.Left, FontStyles.Normal, 1f);
    }

    private static void BuildNavigation(Transform root)
    {
        Box(root, "NavigationShade", 8, 1774, 1064, 138,
            new Color(.025f, .04f, .034f, .96f), BronzeDark, 2);
        Line(root, 8, 1772, 1064, 3, Bronze);
        Nav(root, 8, "Icon_Refuge.png", "REFUGIO", false);
        Nav(root, 274, "Icon_Path.png", "CAMINO", false);
        Nav(root, 540, "Icon_Rites.png", "RITOS", false);
        Nav(root, 806, "Icon_Bond.png", "VÍNCULO", true);
    }

    private static void Nav(Transform root, float x, string iconFile, string title, bool selected)
    {
        if (selected)
            Box(root, "NavSelected", x, 1776, 266, 134, new Color(.25f, .17f, .075f, .74f), Gold, 2);
        if (x > 8)
            Line(root, x, 1790, 1, 104, BronzeDark);
        Artwork(root, "Nav_" + title, ArtRoot + iconFile, x + 91, 1780, 84, 82)
            .color = selected ? Color.white : new Color(.72f, .70f, .61f, .9f);
        Label(root, title, x + 18, 1868, 230, 34, 21,
            selected ? GoldBright : Muted, TextAlignmentOptions.Center, FontStyles.Normal, 1.1f);
    }

    private static void AddOuterRail(Transform root)
    {
        Line(root, 3, 3, 1074, 3, Bronze);
        Line(root, 3, 1914, 1074, 3, Bronze);
        Line(root, 3, 3, 3, 1914, Bronze);
        Line(root, 1074, 3, 3, 1914, Bronze);
        Line(root, 10, 10, 1060, 1, BronzeDark);
    }

    private static Image Artwork(Transform root, string name, string path, float x, float y, float w, float h)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite == null)
            throw new InvalidOperationException("No se pudo cargar el recurso visual: " + path);

        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(root, false);
        Image image = go.GetComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = false;
        image.raycastTarget = false;
        SetRect(image.rectTransform, x, y, w, h);
        return image;
    }

    private static Image Box(Transform root, string name, float x, float y, float w, float h, Color fill)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(root, false);
        Image image = go.GetComponent<Image>();
        image.color = fill;
        image.raycastTarget = false;
        SetRect(image.rectTransform, x, y, w, h);
        return image;
    }

    private static Image Box(
        Transform root, string name, float x, float y, float w, float h,
        Color fill, Color border, float borderWidth)
    {
        Image box = Box(root, name, x, y, w, h, fill);
        Line(box.transform, 0, 0, w, borderWidth, border);
        Line(box.transform, 0, h - borderWidth, w, borderWidth, border);
        Line(box.transform, 0, 0, borderWidth, h, border);
        Line(box.transform, w - borderWidth, 0, borderWidth, h, border);
        return box;
    }

    private static void Line(Transform root, float x, float y, float w, float h, Color color)
    {
        Box(root, "OrnamentLine", x, y, w, h, color);
    }

    private static TextMeshProUGUI Label(
        Transform root, string value, float x, float y, float w, float h,
        float size, Color color, TextAlignmentOptions alignment,
        FontStyles style, float spacing)
    {
        GameObject go = new GameObject("Text_" + value, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(root, false);
        TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();
        text.text = value;
        text.font = cinzel;
        text.fontSize = size;
        text.color = color;
        text.alignment = alignment;
        text.fontStyle = style;
        text.characterSpacing = spacing;
        text.enableWordWrapping = false;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.raycastTarget = false;
        text.outlineColor = new Color(.02f, .018f, .012f, .88f);
        text.outlineWidth = .12f;
        SetRect(text.rectTransform, x, y, w, h);
        text.ForceMeshUpdate(true, true);
        return text;
    }

    private static void Diamond(Transform root, float centerX, float centerY, float size, Color color)
    {
        Image diamond = Box(root, "DividerDiamond", centerX - size * .5f, centerY - size * .5f, size, size, color);
        diamond.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 45f);
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
        RenderTexture target = new RenderTexture(Width, Height, 24, RenderTextureFormat.ARGB32);
        RenderTexture previousTarget = camera.targetTexture;
        RenderTexture previousActive = RenderTexture.active;
        try
        {
            camera.targetTexture = target;
            foreach (TextMeshProUGUI text in UnityEngine.Object.FindObjectsByType<TextMeshProUGUI>(
                         FindObjectsInactive.Include, FindObjectsSortMode.None))
                text.ForceMeshUpdate(true, true);
            UnityEngine.Canvas.ForceUpdateCanvases();
            UnityEngine.Canvas.ForceUpdateCanvases();
            camera.Render();
            RenderTexture.active = target;
            Texture2D image = new Texture2D(Width, Height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
            image.Apply();
            File.WriteAllBytes(outputPath, image.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(image);
        }
        finally
        {
            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            target.Release();
            UnityEngine.Object.DestroyImmediate(target);
        }
    }

    private static Color Hex(string value)
    {
        return ColorUtility.TryParseHtmlString("#" + value, out Color color) ? color : Color.magenta;
    }
}

[InitializeOnLoad]
internal static class Dimension2PactPrototypeAutoRun
{
    private const string RequestPath = "Temp/Dimension2PactPrototype.request";

    static Dimension2PactPrototypeAutoRun()
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
            Dimension2PactPrototypeSetup.BuildAndCapture();
            File.Delete(RequestPath);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
    }
}
#endif

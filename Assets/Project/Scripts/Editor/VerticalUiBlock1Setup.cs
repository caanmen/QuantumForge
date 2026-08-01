#if UNITY_EDITOR
using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class VerticalUiBlock1Setup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string GeneratedFolder = "Assets/Project/UI/Vertical/Generated";
    private const string ThemePath = GeneratedFolder + "/VerticalUiTheme.asset";
    private const string PrimaryFontPath =
        "Assets/Project/UI/Vertical/Fonts/Rajdhani-Medium.ttf";
    private const string PrimaryFontAssetPath =
        "Assets/Project/UI/Vertical/Fonts/Rajdhani-Medium SDF.asset";
    private const string ConfigureMenu =
        "Tools/Quantum Forge/Vertical UI/Configure Block 1 Base";

    [MenuItem(ConfigureMenu)]
    public static void ConfigureBlock1Base()
    {
        ConfigurePlayerSettings();
        EnsureGeneratedFolders();
        GenerateModularSprites();
        VerticalUiTheme theme = CreateOrUpdateTheme();

        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        GameObject hud = Require(scene, "HUD");
        Canvas canvas = Require(scene, "Canvas").GetComponent<Canvas>();
        if (canvas == null)
            throw new InvalidOperationException("Canvas no contiene el componente Canvas.");

        ConfigureCanvas(canvas);
        ConfigureVerticalRoot(hud.transform, theme);
        ConfigureLegacyPortraitAdapter(hud);

        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("No se pudo guardar Main.unity.");

        AssetDatabase.SaveAssets();
        Debug.Log("[Vertical UI Block 1] CONFIGURED | portrait 1080x1920 | " +
            "Safe Area | skin modular | legacy portrait adapter");
    }

    public static void ConfigureAndValidateBatch()
    {
        ConfigureBlock1Base();
        ConfigureBlock1Base();
        VerticalUiBlock1Validation.Validate();
        MobileQaFriendlyLayoutValidation.Validate();
        MobileButtonLegibilityValidation.Validate();
        QaMainSceneIntegrityValidation.ValidateMainSceneIntegrity();
        Debug.Log("[Vertical UI Block 1] IDEMPOTENCE PASS | setup executed twice");
    }

    private static void ConfigurePlayerSettings()
    {
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.allowedAutorotateToPortrait = true;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = false;
        PlayerSettings.allowedAutorotateToLandscapeRight = false;
        PlayerSettings.defaultScreenWidth = 1080;
        PlayerSettings.defaultScreenHeight = 1920;
    }

    private static void ConfigureCanvas(Canvas canvas)
    {
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler == null)
            scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        EditorUtility.SetDirty(scaler);
    }

    private static void ConfigureVerticalRoot(Transform hud, VerticalUiTheme theme)
    {
        GameObject root = GetOrCreateUiObject("VerticalUIRoot", hud);
        root.transform.SetSiblingIndex(0);
        Stretch((RectTransform)root.transform);

        VerticalUiSkinRoot skinRoot = GetOrAdd<VerticalUiSkinRoot>(root);
        skinRoot.theme = theme;

        GameObject background = GetOrCreateUiObject("Background", root.transform);
        background.transform.SetSiblingIndex(0);
        Stretch((RectTransform)background.transform);
        Image backgroundImage = GetOrAdd<Image>(background);
        backgroundImage.sprite = null;
        backgroundImage.color = theme.background;
        backgroundImage.raycastTarget = false;

        GameObject grid = GetOrCreateUiObject("BackgroundGrid", root.transform);
        grid.transform.SetSiblingIndex(1);
        Stretch((RectTransform)grid.transform);
        Image gridImage = GetOrAdd<Image>(grid);
        gridImage.sprite = theme.backgroundGrid;
        gridImage.type = Image.Type.Tiled;
        gridImage.color = new Color(0.23f, 0.72f, 1f, 0.16f);
        gridImage.raycastTarget = false;

        GameObject safeArea = GetOrCreateUiObject("VerticalSafeAreaRoot", root.transform);
        Stretch((RectTransform)safeArea.transform);
        GameObject header = GetOrCreateUiObject("HeaderSlot", safeArea.transform);
        GameObject content = GetOrCreateUiObject("ContentSlot", safeArea.transform);
        GameObject secondary = GetOrCreateUiObject(
            "SecondaryNavigationSlot", safeArea.transform);
        GameObject primary = GetOrCreateUiObject(
            "PrimaryNavigationSlot", safeArea.transform);

        if (secondary.transform.childCount == 0)
            secondary.SetActive(false);
        primary.SetActive(true);

        VerticalSafeAreaLayout layout = GetOrAdd<VerticalSafeAreaLayout>(root);
        layout.safeAreaRoot = (RectTransform)safeArea.transform;
        layout.headerSlot = (RectTransform)header.transform;
        layout.contentSlot = (RectTransform)content.transform;
        layout.secondaryNavigationSlot = (RectTransform)secondary.transform;
        layout.primaryNavigationSlot = (RectTransform)primary.transform;
        layout.horizontalMargin = 24f;
        layout.headerHeight = 0f;
        layout.primaryNavigationHeight = 132f;
        layout.secondaryNavigationHeight = 104f;
        layout.verticalGap = 16f;
        layout.ApplyLayout();

        EditorUtility.SetDirty(skinRoot);
        EditorUtility.SetDirty(layout);
        EditorUtility.SetDirty(backgroundImage);
        EditorUtility.SetDirty(gridImage);
    }

    private static void ConfigureLegacyPortraitAdapter(GameObject hud)
    {
        MobileQaFriendlyLayout layout = hud.GetComponent<MobileQaFriendlyLayout>();
        if (layout == null)
            throw new InvalidOperationException(
                "HUD no contiene MobileQaFriendlyLayout; no se altero la escena.");
        layout.forcePortraitLayout = true;
        layout.ApplyLayout();
        EditorUtility.SetDirty(layout);
    }

    private static VerticalUiTheme CreateOrUpdateTheme()
    {
        VerticalUiTheme theme = AssetDatabase.LoadAssetAtPath<VerticalUiTheme>(ThemePath);
        if (theme == null)
        {
            theme = ScriptableObject.CreateInstance<VerticalUiTheme>();
            AssetDatabase.CreateAsset(theme, ThemePath);
        }

        theme.ResetToApprovedDefaults();
        theme.backgroundGrid = LoadSprite("qf_vertical_grid.png");
        theme.panelFrame = LoadSprite("qf_vertical_panel.png");
        theme.buttonFrame = LoadSprite("qf_vertical_button.png");
        theme.selectedButtonFrame = LoadSprite("qf_vertical_button_selected.png");
        theme.softGlow = LoadSprite("qf_vertical_glow.png");
        theme.primaryFont = CreateOrLoadPrimaryFont();
        EditorUtility.SetDirty(theme);
        return theme;
    }

    private static TMP_FontAsset CreateOrLoadPrimaryFont()
    {
        TMP_FontAsset fontAsset =
            AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(PrimaryFontAssetPath);
        if (fontAsset != null)
            return fontAsset;

        Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(PrimaryFontPath);
        if (sourceFont == null)
        {
            Debug.LogWarning("[Vertical UI Block 1] Rajdhani no esta disponible; " +
                "se conserva Liberation Sans como respaldo.");
            return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        }

        fontAsset = TMP_FontAsset.CreateFontAsset(sourceFont);
        fontAsset.name = "Rajdhani-Medium SDF";
        AssetDatabase.CreateAsset(fontAsset, PrimaryFontAssetPath);
        if (fontAsset.material != null &&
            string.IsNullOrEmpty(AssetDatabase.GetAssetPath(fontAsset.material)))
        {
            fontAsset.material.name = "Rajdhani-Medium Atlas Material";
            AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);
        }
        if (fontAsset.atlasTexture != null &&
            string.IsNullOrEmpty(AssetDatabase.GetAssetPath(fontAsset.atlasTexture)))
        {
            fontAsset.atlasTexture.name = "Rajdhani-Medium Atlas";
            AssetDatabase.AddObjectToAsset(fontAsset.atlasTexture, fontAsset);
        }
        EditorUtility.SetDirty(fontAsset);
        AssetDatabase.SaveAssets();
        return fontAsset;
    }

    private static Sprite LoadSprite(string fileName)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            GeneratedFolder + "/" + fileName);
        if (sprite == null)
            throw new InvalidOperationException("No se pudo importar " + fileName + ".");
        return sprite;
    }

    private static void GenerateModularSprites()
    {
        CreateOrUpdateTexture("qf_vertical_grid.png", 128, 128,
            CreateGridPixels(128, 128), Vector4.zero, true);
        CreateOrUpdateTexture("qf_vertical_panel.png", 96, 96,
            CreateFramePixels(96, 96,
                new Color32(3, 13, 20, 250), new Color32(10, 53, 77, 228), 12, 2),
            new Vector4(18f, 18f, 18f, 18f), false);
        CreateOrUpdateTexture("qf_vertical_button.png", 96, 64,
            CreateFramePixels(96, 64,
                new Color32(3, 16, 25, 252), new Color32(10, 59, 86, 235), 10, 2),
            new Vector4(16f, 16f, 16f, 16f), false);
        CreateOrUpdateTexture("qf_vertical_button_selected.png", 96, 64,
            CreateFramePixels(96, 64,
                new Color32(3, 31, 54, 255), new Color32(0, 181, 255, 255), 10, 4),
            new Vector4(16f, 16f, 16f, 16f), false);
        CreateOrUpdateTexture("qf_vertical_glow.png", 128, 128,
            CreateGlowPixels(128, 128), Vector4.zero, false);
    }

    private static Color32[] CreateGridPixels(int width, int height)
    {
        var pixels = new Color32[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool major = x % 64 == 0 || y % 64 == 0;
                bool minor = x % 16 == 0 || y % 16 == 0;
                byte alpha = major ? (byte)42 : minor ? (byte)18 : (byte)0;
                if ((x == 32 && y == 32) || (x == 96 && y == 80))
                    alpha = 64;
                pixels[y * width + x] = new Color32(20, 128, 190, alpha);
            }
        }
        return pixels;
    }

    private static Color32[] CreateFramePixels(
        int width,
        int height,
        Color32 fill,
        Color32 border,
        int cornerCut,
        int borderWidth)
    {
        var pixels = new Color32[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool inside = IsInsideChamfer(x, y, width, height, cornerCut, 0);
                bool inner = IsInsideChamfer(x, y, width, height,
                    cornerCut, borderWidth);
                Color32 color = !inside
                    ? new Color32(0, 0, 0, 0)
                    : inner ? fill : border;
                pixels[y * width + x] = color;
            }
        }
        return pixels;
    }

    private static bool IsInsideChamfer(
        int x, int y, int width, int height, int cut, int inset)
    {
        int left = inset;
        int right = width - 1 - inset;
        int bottom = inset;
        int top = height - 1 - inset;
        int adjustedCut = Mathf.Max(0, cut - inset);
        if (x < left || x > right || y < bottom || y > top)
            return false;
        return x + y >= left + bottom + adjustedCut &&
            (right - x) + y >= bottom + adjustedCut &&
            x + (top - y) >= left + adjustedCut &&
            (right - x) + (top - y) >= adjustedCut;
    }

    private static Color32[] CreateGlowPixels(int width, int height)
    {
        var pixels = new Color32[width * height];
        Vector2 center = new((width - 1) * 0.5f, (height - 1) * 0.5f);
        float radius = Mathf.Min(width, height) * 0.5f;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float normalized = Vector2.Distance(new Vector2(x, y), center) / radius;
                float intensity = Mathf.Pow(Mathf.Clamp01(1f - normalized), 2.2f);
                pixels[y * width + x] = new Color32(0, 198, 255,
                    (byte)Mathf.RoundToInt(intensity * 180f));
            }
        }
        return pixels;
    }

    private static void CreateOrUpdateTexture(
        string fileName,
        int width,
        int height,
        Color32[] pixels,
        Vector4 border,
        bool repeat)
    {
        string assetPath = GeneratedFolder + "/" + fileName;
        var texture = new Texture2D(width, height, TextureFormat.RGBA32, false, false);
        texture.SetPixels32(pixels);
        texture.Apply(false, false);
        byte[] png = texture.EncodeToPNG();
        UnityEngine.Object.DestroyImmediate(texture);

        string absolutePath = Path.GetFullPath(assetPath);
        if (!File.Exists(absolutePath) || !BytesEqual(File.ReadAllBytes(absolutePath), png))
            File.WriteAllBytes(absolutePath, png);

        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer == null)
            throw new InvalidOperationException("No se encontro TextureImporter para " + assetPath);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 100f;
        importer.spriteBorder = border;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.wrapMode = repeat ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Bilinear;
        importer.textureCompression = TextureImporterCompression.Compressed;
        importer.maxTextureSize = 256;
        importer.SetPlatformTextureSettings(new TextureImporterPlatformSettings
        {
            name = "Android",
            overridden = true,
            maxTextureSize = 256,
            format = TextureImporterFormat.ASTC_4x4,
            compressionQuality = 50
        });
        importer.SaveAndReimport();
    }

    private static bool BytesEqual(byte[] left, byte[] right)
    {
        if (left.Length != right.Length)
            return false;
        for (int index = 0; index < left.Length; index++)
            if (left[index] != right[index])
                return false;
        return true;
    }

    private static void EnsureGeneratedFolders()
    {
        EnsureFolder("Assets/Project/UI");
        EnsureFolder("Assets/Project/UI/Vertical");
        EnsureFolder(GeneratedFolder);
    }

    private static void EnsureFolder(string assetPath)
    {
        if (AssetDatabase.IsValidFolder(assetPath))
            return;
        string parent = assetPath.Substring(0, assetPath.LastIndexOf('/'));
        string name = assetPath.Substring(assetPath.LastIndexOf('/') + 1);
        AssetDatabase.CreateFolder(parent, name);
    }

    private static T GetOrAdd<T>(GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        return component != null ? component : gameObject.AddComponent<T>();
    }

    private static GameObject GetOrCreateUiObject(string name, Transform parent)
    {
        Transform existing = FindDirectChild(parent, name);
        if (existing != null)
            return existing.gameObject;
        var result = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
        result.layer = 5;
        result.transform.SetParent(parent, false);
        return result;
    }

    private static Transform FindDirectChild(Transform parent, string name)
    {
        for (int index = 0; index < parent.childCount; index++)
        {
            Transform child = parent.GetChild(index);
            if (child.name == name)
                return child;
        }
        return null;
    }

    private static GameObject Require(Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
            {
                if (current.name == name)
                    return current.gameObject;
            }
        }
        throw new InvalidOperationException("Falta " + name + " en Main.unity.");
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
    }
}
#endif

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PrestigeDimensionTransitionSetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string CardsPath =
        "Assets/Project/UI/Vertical/Prestige/Concepts/" +
        "QF_DimensionalSelection_Landscape_Reference_v1.png";
    private const string PortraitPath =
        "Assets/Project/UI/Vertical/Prestige/Concepts/" +
        "QF_DimensionalSelection_Portrait_Carousel_v1.png";
    private const string LaboratoryPath =
        "Assets/Project/UI/Vertical/Prestige/Concepts/" +
        "QF_PrestigeCube_LabBackground_Portrait_v1.png";
    private const string Cube2DPath =
        "Assets/Project/UI/Vertical/Prestige/Cinematic/" +
        "QF_PrestigeCube2D_v1.png";
    private const string CharacterPath =
        "Assets/Project/UI/Vertical/Prestige/Cinematic/" +
        "QF_PrestigeCharacterBackRaised_v1.png";
    private const string PortalRingPath =
        "Assets/Project/UI/Vertical/Prestige/Cinematic/" +
        "QF_PrestigePortalRing2D_v1.png";
    private const string ResourceConfigPath =
        "Assets/Project/Resources/Prestige/" +
        "PrestigeDimensionTransitionConfig.asset";

    [MenuItem("Tools/Quantum Forge/Prestige/Configure Dimensional Transition")]
    public static void Configure()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("[Prestige Dimension Setup] Ejecutar fuera de Play Mode.");
            return;
        }

        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        PrestigeUI prestige = Object.FindFirstObjectByType<PrestigeUI>(
            FindObjectsInactive.Include);
        if (prestige == null)
            throw new System.InvalidOperationException(
                "No se encontró PrestigeUI en Main.unity.");

        ConfigureTexture(LaboratoryPath, 2048);
        ConfigureTexture(Cube2DPath, 1024);
        ConfigureTexture(CharacterPath, 1024);
        ConfigureTexture(PortalRingPath, 256);

        PrestigeDimensionTransitionUI transition =
            prestige.GetComponent<PrestigeDimensionTransitionUI>();
        if (transition == null)
            transition = Undo.AddComponent<PrestigeDimensionTransitionUI>(
                prestige.gameObject);

        Texture2D cards = AssetDatabase.LoadAssetAtPath<Texture2D>(CardsPath);
        Texture2D portrait = AssetDatabase.LoadAssetAtPath<Texture2D>(PortraitPath);
        Texture2D laboratory = AssetDatabase.LoadAssetAtPath<Texture2D>(LaboratoryPath);
        Texture2D cube2D = AssetDatabase.LoadAssetAtPath<Texture2D>(Cube2DPath);
        Texture2D character = AssetDatabase.LoadAssetAtPath<Texture2D>(CharacterPath);
        Texture2D portalRing = AssetDatabase.LoadAssetAtPath<Texture2D>(PortalRingPath);
        if (cards == null || portrait == null || laboratory == null ||
            cube2D == null || character == null || portalRing == null)
            throw new System.InvalidOperationException(
                "Faltan las referencias visuales de la transición dimensional.");

        SerializedObject transitionObject = new SerializedObject(transition);
        transitionObject.FindProperty("cardsTexture").objectReferenceValue = cards;
        transitionObject.FindProperty("portraitReference").objectReferenceValue = portrait;
        transitionObject.FindProperty("laboratoryBackground").objectReferenceValue =
            laboratory;
        transitionObject.FindProperty("cube2DTexture").objectReferenceValue = cube2D;
        transitionObject.FindProperty("characterTexture").objectReferenceValue =
            character;
        transitionObject.FindProperty("portalRingTexture").objectReferenceValue =
            portalRing;
        transitionObject.FindProperty("transitionDuration").floatValue = 8.6f;
        transitionObject.FindProperty("confirmationResetSeconds").floatValue = 5f;
        transitionObject.ApplyModifiedPropertiesWithoutUndo();

        SerializedObject prestigeObject = new SerializedObject(prestige);
        prestigeObject.FindProperty("dimensionTransition").objectReferenceValue = transition;
        prestigeObject.ApplyModifiedPropertiesWithoutUndo();

        EditorUtility.SetDirty(transition);
        EditorUtility.SetDirty(prestige);

        PrestigeDimensionTransitionConfig resourceConfig =
            AssetDatabase.LoadAssetAtPath<PrestigeDimensionTransitionConfig>(
                ResourceConfigPath);
        if (resourceConfig != null)
        {
            resourceConfig.cardsTexture = cards;
            resourceConfig.portraitReference = portrait;
            resourceConfig.laboratoryBackground = laboratory;
            resourceConfig.cube2DTexture = cube2D;
            resourceConfig.characterTexture = character;
            resourceConfig.portalRingTexture = portalRing;
            EditorUtility.SetDirty(resourceConfig);
        }
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();

        PrestigeDimensionTransitionValidation.Validate();
        Debug.Log(
            "[Prestige Dimension Setup] PASS | cinemática 2D ligera | " +
            "transición 8.6s | personaje + tres portales + cubo ilustrado | " +
            "carrusel vertical | confirmación 3 pulsaciones | cancelación segura");
    }

    public static void ConfigureBatch()
    {
        Configure();
    }

    private static void ConfigureTexture(string path, int maxSize)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
            return;

        bool changed = importer.mipmapEnabled || importer.maxTextureSize != maxSize ||
            importer.wrapMode != TextureWrapMode.Clamp ||
            importer.textureCompression != TextureImporterCompression.Compressed ||
            !importer.alphaIsTransparency;
        if (!changed)
            return;

        importer.mipmapEnabled = false;
        importer.maxTextureSize = maxSize;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.textureCompression = TextureImporterCompression.Compressed;
        importer.alphaIsTransparency = true;
        importer.SaveAndReimport();
    }
}
#endif

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
    private const string MonolithPath =
        "Assets/Project/UI/Vertical/Machine/Monolith2D/" +
        "monolith_overview_progression_clean_v03.png";
    private const string MonolithCutoutMaterialPath =
        "Assets/Project/UI/Vertical/Machine/Monolith2D/" +
        "machine_monolith_light_key.mat";
    private const string EntryLightPath =
        "Assets/Project/UI/Vertical/Machine/Monolith2D/" +
        "machine_monolith_entry_light_depth_pulse_emission_v01.png";
    private const string EntryLightAdditiveMaterialPath =
        "Assets/Project/UI/Vertical/Machine/Monolith2D/" +
        "machine_monolith_entry_light_additive.mat";
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
        ConfigureTexture(MonolithPath, 2048);
        ConfigureTexture(EntryLightPath, 1024);
        ConfigureTexture(PortalRingPath, 256);

        PrestigeDimensionTransitionUI transition =
            prestige.GetComponent<PrestigeDimensionTransitionUI>();
        if (transition == null)
            transition = Undo.AddComponent<PrestigeDimensionTransitionUI>(
                prestige.gameObject);

        Texture2D cards = AssetDatabase.LoadAssetAtPath<Texture2D>(CardsPath);
        Texture2D portrait = AssetDatabase.LoadAssetAtPath<Texture2D>(PortraitPath);
        Texture2D laboratory = AssetDatabase.LoadAssetAtPath<Texture2D>(LaboratoryPath);
        Texture2D monolith = AssetDatabase.LoadAssetAtPath<Texture2D>(MonolithPath);
        Material monolithCutout =
            AssetDatabase.LoadAssetAtPath<Material>(MonolithCutoutMaterialPath);
        Texture2D entryLight =
            AssetDatabase.LoadAssetAtPath<Texture2D>(EntryLightPath);
        Material entryLightAdditive =
            AssetDatabase.LoadAssetAtPath<Material>(
                EntryLightAdditiveMaterialPath);
        Texture2D portalRing = AssetDatabase.LoadAssetAtPath<Texture2D>(PortalRingPath);
        if (cards == null || portrait == null || laboratory == null ||
            monolith == null || monolithCutout == null || entryLight == null ||
            entryLightAdditive == null || portalRing == null)
            throw new System.InvalidOperationException(
                "Faltan las referencias visuales de la transición dimensional.");

        SerializedObject transitionObject = new SerializedObject(transition);
        transitionObject.FindProperty("cardsTexture").objectReferenceValue = cards;
        transitionObject.FindProperty("portraitReference").objectReferenceValue = portrait;
        transitionObject.FindProperty("laboratoryBackground").objectReferenceValue =
            laboratory;
        transitionObject.FindProperty("monolithTexture").objectReferenceValue =
            monolith;
        transitionObject.FindProperty("monolithCutoutMaterial").objectReferenceValue =
            monolithCutout;
        transitionObject.FindProperty("entryLightTexture").objectReferenceValue =
            entryLight;
        transitionObject.FindProperty("entryLightAdditiveMaterial").objectReferenceValue =
            entryLightAdditive;
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
            resourceConfig.monolithTexture = monolith;
            resourceConfig.monolithCutoutMaterial = monolithCutout;
            resourceConfig.entryLightTexture = entryLight;
            resourceConfig.entryLightAdditiveMaterial = entryLightAdditive;
            resourceConfig.portalRingTexture = portalRing;
            EditorUtility.SetDirty(resourceConfig);
        }
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();

        PrestigeDimensionTransitionValidation.Validate();
        Debug.Log(
            "[Prestige Dimension Setup] PASS | cinemática 2D ligera | " +
            "transición 8.6s | Monolito reparado + apertura + tres portales | " +
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

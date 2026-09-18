#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class UIProductionManifestSetup
{
    private const string ManifestFolder = "Assets/Project/UI/ProductionSystem/Manifests";
    private const string ThemePath = "Assets/Project/UI/Vertical/Generated/VerticalUiTheme.asset";

    [MenuItem("Quantum Forge/UI Production/Configurar manifiestos conocidos")]
    public static void ConfigureKnownManifests()
    {
        EnsureAssetFolder(ManifestFolder);

        VerticalUiTheme theme = AssetDatabase.LoadAssetAtPath<VerticalUiTheme>(ThemePath);
        ConfigureManifest(
            ManifestFolder + "/D1_CentroDeMando.asset",
            "D1_CENTRO_DE_MANDO",
            "Centro de Mando",
            UIProductionScreenManifest.ApprovalStage.DataAndInteraction,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/DIMENSION_1/CENTRO_DE_MANDO/referencia_centro_de_mando.png",
            new string[0],
            theme,
            "Dimension1CommandCenterSetup",
            "Dimension1CommandCenterDrawerRuntimeValidation",
            new[] { "Neutral", "Drawer/Navegación" },
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/DIMENSION_1/CENTRO_DE_MANDO",
            "La captura final aún debe consolidarse en el sistema central.");

        ConfigureManifest(
            ManifestFolder + "/D1_CartaGalactica.asset",
            "D1_CARTA_GALACTICA",
            "Carta Galáctica",
            UIProductionScreenManifest.ApprovalStage.Approved,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/DIMENSION_1/CARTA_GALACTICA/referencia_carta_galactica.png",
            ApprovedGalaxyCaptures(),
            theme,
            "Dimension1GalaxyPremiumSetup.ConfigureReferenceV11",
            "Dimension1GalaxyReferenceCapture.RunInteractionRegression",
            new[] { "Neutral", "Órbitas Antiguas seleccionada" },
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/DIMENSION_1/CARTA_GALACTICA",
            "Referencia V11 con rutas simétricas e interacción regresiva aprobada.");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[UI Production System] MANIFESTS_CONFIGURED | 2 manifiestos conocidos.");
    }

    private static void ConfigureManifest(
        string assetPath,
        string screenId,
        string displayName,
        UIProductionScreenManifest.ApprovalStage stage,
        string referenceRelativePath,
        string[] captureRelativePaths,
        VerticalUiTheme theme,
        string setupMethod,
        string validationMethod,
        string[] captureStates,
        string documentationFolder,
        string notes)
    {
        UIProductionScreenManifest manifest =
            AssetDatabase.LoadAssetAtPath<UIProductionScreenManifest>(assetPath);
        if (manifest == null)
        {
            manifest = ScriptableObject.CreateInstance<UIProductionScreenManifest>();
            AssetDatabase.CreateAsset(manifest, assetPath);
        }

        manifest.screenId = screenId;
        manifest.displayName = displayName;
        manifest.dimension = UIProductionScreenManifest.DimensionId.Dimension1;
        manifest.approvalStage = stage;
        manifest.baseResolution = new Vector2Int(1080, 1920);
        manifest.perceivedSimilarityTarget = 0.95f;
        manifest.referenceRelativePath = referenceRelativePath;
        manifest.approvedCaptureRelativePaths = captureRelativePaths;
        manifest.sharedTheme = theme;
        manifest.setupMethod = setupMethod;
        manifest.validationMethod = validationMethod;
        manifest.requiredCaptureStates = captureStates;
        manifest.documentationFolder = documentationFolder;
        manifest.notes = notes;
        EditorUtility.SetDirty(manifest);
    }

    private static string[] ApprovedGalaxyCaptures()
    {
        return new[]
        {
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/06_CAPTURAS_APROBADAS/DIMENSION_1/CARTA_GALACTICA/carta_galactica_neutral_1080x1920.png",
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/06_CAPTURAS_APROBADAS/DIMENSION_1/CARTA_GALACTICA/carta_galactica_orbitas_seleccionada_1080x1920.png"
        };
    }

    private static void EnsureAssetFolder(string assetFolder)
    {
        string[] parts = assetFolder.Split('/');
        string current = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }
}
#endif

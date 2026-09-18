#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public static class Dimension3FacilitiesVisualSetup
{
    private const string MainScenePath = "Assets/Project/Scenes/Main.unity";
    private const string NucleusReferencePath =
        "Assets/Project/Art/Dimension3/Facilities/D3_InstalacionesNucleo_Reference_Final_v1.png";
    private const string ConsoleReferencePath =
        "Assets/Project/Art/Dimension3/Facilities/D3_InstalacionesConsola_Reference_Final_v1.png";
    private const string ImprovementReferencePath =
        "Assets/Project/Art/Dimension3/Facilities/D3_InstalacionesMejoras_Reference_Final_v1.png";
    private const string PortReferencePath =
        "Assets/Project/Art/Dimension3/Facilities/D3_InstalacionesPuerto_Reference_Final_v1.png";
    private const float SourceHeight = 1920f;
    private const float DisplayHeight = 1890f;
    private const float ScaleY = DisplayHeight / SourceHeight;
    private static readonly Vector2 ViewportCompensation = new Vector2(-15f, 17f);

    [MenuItem("Tools/Quantum Forge/Dimension 3/Apply Facilities Visual")]
    public static void ConfigureOnly()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != MainScenePath)
            scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
        D3FacilitiesPanelUI panel = Object.FindFirstObjectByType<D3FacilitiesPanelUI>(
            FindObjectsInactive.Include);
        if (panel == null)
        {
            Debug.LogError("[D3 Facilities Visual] Falta D3FacilitiesPanelUI.");
            return;
        }

        ApplyTo(panel);
        EditorUtility.SetDirty(panel);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene))
        {
            Debug.LogError("[D3 Facilities Visual] No se pudo guardar Main.unity.");
            return;
        }
        ValidateCurrent();
        Debug.Log("[D3 Facilities Visual] APPLY PASS");
    }

    public static void ConfigureOnlyBatch() => ConfigureOnly();

    public static void ApplyTo(D3FacilitiesPanelUI panel)
    {
        EnsureReferenceImport(NucleusReferencePath);
        EnsureReferenceImport(ConsoleReferencePath);
        EnsureReferenceImport(ImprovementReferencePath);
        EnsureReferenceImport(PortReferencePath);
        Texture2D nucleusReference =
            AssetDatabase.LoadAssetAtPath<Texture2D>(NucleusReferencePath);
        Texture2D consoleReference =
            AssetDatabase.LoadAssetAtPath<Texture2D>(ConsoleReferencePath);
        Texture2D improvementReference =
            AssetDatabase.LoadAssetAtPath<Texture2D>(ImprovementReferencePath);
        Texture2D portReference =
            AssetDatabase.LoadAssetAtPath<Texture2D>(PortReferencePath);
        if (nucleusReference == null || consoleReference == null ||
            improvementReference == null || portReference == null)
        {
            Debug.LogError("[D3 Facilities Visual] No se pudo cargar la referencia final.");
            return;
        }

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        if (panelRect != null)
        {
            panelRect.anchoredPosition = ViewportCompensation;
            panelRect.sizeDelta = Vector2.zero;
            panelRect.localScale = Vector3.one;
        }

        Transform previous = panel.transform.Find("D3_FacilitiesVisual");
        if (previous != null)
            Object.DestroyImmediate(previous.gameObject);
        GameObject visual = Create("D3_FacilitiesVisual", panel.transform);
        SetTopLeft(visual.GetComponent<RectTransform>(), 0f, 0f, 1080f, DisplayHeight);
        RawImage plate = visual.AddComponent<RawImage>();
        plate.texture = consoleReference;
        plate.uvRect = new Rect(0f, 0f, 1f, 1f);
        plate.color = Color.white;
        plate.raycastTarget = false;
        D3FacilitiesStaticSkinUI skin = visual.AddComponent<D3FacilitiesStaticSkinUI>();
        skin.referencePlate = plate;
        skin.consoleReference = consoleReference;
        skin.nucleusReference = nucleusReference;
        skin.improvementReference = improvementReference;
        skin.portReference = portReference;
        skin.facilityDropdown = panel.facilityDropdown;
        visual.transform.SetAsLastSibling();

        skin.backRect = RectOf(panel.backButton);
        skin.facilityRect = RectOf(panel.facilityDropdown);
        skin.channelRect = RectOf(panel.channelDropdown);
        skin.mkRect = RectOf(panel.mkDropdown);
        skin.traitRect = RectOf(panel.traitDropdown);
        skin.addRect = RectOf(panel.addAssignmentButton);
        skin.removeRect = RectOf(panel.removeAssignmentButton);
        skin.upgradeRect = RectOf(panel.upgradeButton);
        skin.automationRect = RectOf(panel.openAutomationButton);
        skin.autonomyRect = RectOf(panel.integrateAutonomyCoreButton);
        skin.consoleRect = RectOf(panel.openConsoleButton);
        SetControl(panel.openConsoleButton, 550, 1502, 469, 94);
        skin.SetImprovementVariant(false);

        EditorUtility.SetDirty(skin);
        EditorUtility.SetDirty(panel);
    }

    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Facilities Visual")]
    public static void ValidateCurrent()
    {
        D3FacilitiesPanelUI panel = Object.FindFirstObjectByType<D3FacilitiesPanelUI>(
            FindObjectsInactive.Include);
        Transform visual = panel == null ? null : panel.transform.Find("D3_FacilitiesVisual");
        D3FacilitiesStaticSkinUI skin = visual == null
            ? null : visual.GetComponent<D3FacilitiesStaticSkinUI>();
        bool valid = panel != null && visual != null && skin != null &&
            skin.referencePlate != null && skin.referencePlate.texture != null &&
            !skin.referencePlate.raycastTarget &&
            visual.GetSiblingIndex() == panel.transform.childCount - 1 &&
            HasTouchTarget(panel.backButton) && HasTouchTarget(panel.facilityDropdown) &&
            HasTouchTarget(panel.channelDropdown) && HasTouchTarget(panel.mkDropdown) &&
            HasTouchTarget(panel.traitDropdown) &&
            HasTouchTarget(panel.addAssignmentButton) &&
            HasTouchTarget(panel.removeAssignmentButton) &&
            HasTouchTarget(panel.upgradeButton) &&
            HasTouchTarget(panel.openAutomationButton) &&
            HasTouchTarget(panel.integrateAutonomyCoreButton) &&
            HasTouchTarget(panel.openConsoleButton) &&
            skin.consoleReference != null && skin.nucleusReference != null &&
            skin.improvementReference != null &&
            skin.portReference != null &&
            skin.facilityDropdown == panel.facilityDropdown;
        if (valid)
            Debug.Log("[D3 Facilities Visual] VALIDATION PASS");
        else
            Debug.LogError("[D3 Facilities Visual] VALIDATION FAIL");
    }

    public static void ValidateCurrentBatch()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != MainScenePath)
            EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
        ValidateCurrent();
    }

    private static void SetControl(Component control, float x, float y, float width, float height)
    {
        if (control == null)
            return;
        SetTopLeft(control.GetComponent<RectTransform>(), x, y * ScaleY, width, height * ScaleY);
        control.gameObject.SetActive(true);
    }

    private static bool HasTouchTarget(Component control)
    {
        if (control == null)
            return false;
        RectTransform rect = control.GetComponent<RectTransform>();
        return rect != null && rect.sizeDelta.x >= 44f && rect.sizeDelta.y >= 44f;
    }

    private static RectTransform RectOf(Component control)
    {
        return control == null ? null : control.GetComponent<RectTransform>();
    }

    private static void EnsureReferenceImport(string path)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
            return;
        importer.textureType = TextureImporterType.Default;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.maxTextureSize = 2048;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();
    }

    private static GameObject Create(string name, Transform parent)
    {
        GameObject target = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
        target.transform.SetParent(parent, false);
        return target;
    }

    private static void SetTopLeft(RectTransform rect,
        float x, float y, float width, float height)
    {
        if (rect == null)
            return;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(x, -y);
        rect.sizeDelta = new Vector2(width, height);
        rect.localScale = Vector3.one;
    }
}
#endif

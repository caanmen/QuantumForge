#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public static class Dimension3DiagnosticVisualSetup
{
    private const string MainScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ReferencePath =
        "Assets/Project/Art/Dimension3/Diagnostic/D3_ControlDiagnostico_Reference_Final_v1.png";
    private const float SourceHeight = 1920f;
    private const float DisplayHeight = 1890f;
    private const float ScaleY = DisplayHeight / SourceHeight;
    private static readonly Vector2 ViewportCompensation = new Vector2(-15f, 17f);

    [MenuItem("Tools/Quantum Forge/Dimension 3/Apply Diagnostic Visual")]
    public static void ConfigureOnly()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != MainScenePath)
            scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
        D3DiagnosticPanelUI panel = Object.FindFirstObjectByType<D3DiagnosticPanelUI>(
            FindObjectsInactive.Include);
        if (panel == null)
        {
            Debug.LogError("[D3 Diagnostic Visual] Falta D3DiagnosticPanelUI.");
            return;
        }
        ApplyTo(panel);
        EditorUtility.SetDirty(panel);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene))
        {
            Debug.LogError("[D3 Diagnostic Visual] No se pudo guardar Main.unity.");
            return;
        }
        ValidateCurrent();
        Debug.Log("[D3 Diagnostic Visual] APPLY PASS");
    }

    public static void ConfigureOnlyBatch() => ConfigureOnly();

    public static void ApplyTo(D3DiagnosticPanelUI panel)
    {
        EnsureReferenceImport();
        Texture2D reference = AssetDatabase.LoadAssetAtPath<Texture2D>(ReferencePath);
        if (reference == null)
        {
            Debug.LogError("[D3 Diagnostic Visual] No se pudo cargar la referencia final.");
            return;
        }

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        if (panelRect != null)
        {
            panelRect.anchoredPosition = ViewportCompensation;
            panelRect.sizeDelta = Vector2.zero;
            panelRect.localScale = Vector3.one;
        }

        Transform previous = panel.transform.Find("D3_DiagnosticVisual");
        if (previous != null)
            Object.DestroyImmediate(previous.gameObject);
        GameObject visual = Create("D3_DiagnosticVisual", panel.transform);
        SetTopLeft(visual.GetComponent<RectTransform>(), 0f, 0f, 1080f, DisplayHeight);
        RawImage plate = visual.AddComponent<RawImage>();
        plate.texture = reference;
        plate.uvRect = new Rect(0f, 0f, 1f, 1f);
        plate.color = Color.white;
        plate.raycastTarget = false;
        D3DiagnosticStaticSkinUI skin = visual.AddComponent<D3DiagnosticStaticSkinUI>();
        skin.referencePlate = plate;
        visual.transform.SetAsLastSibling();

        SetControl(panel.toggleAnalyzeButton, 55, 1100, 318, 133);
        SetControl(panel.toggleRepairButton, 374, 1100, 332, 133);
        SetControl(panel.toggleFusionButton, 707, 1100, 329, 133);
        SetControl(panel.priorityModeDropdown, 164, 1244, 365, 66);
        SetControl(panel.zoneDropdown, 693, 1244, 324, 66);
        SetControl(panel.leReserveDropdown, 55, 1318, 480, 66);
        SetControl(panel.tracesReserveDropdown, 540, 1318, 479, 66);
        SetControl(panel.recipeDropdown, 289, 1392, 410, 68);
        SetControl(panel.toggleRecipeMarkButton, 714, 1392, 304, 68);
        SetControl(panel.saveSettingsButton, 55, 1465, 964, 82);
        SetControl(panel.saveRoutineButton, 55, 1555, 480, 76);
        SetControl(panel.loadRoutineButton, 540, 1555, 479, 76);
        SetControl(panel.backButton, 55, 1635, 964, 80);

        EditorUtility.SetDirty(skin);
        EditorUtility.SetDirty(panel);
    }

    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Diagnostic Visual")]
    public static void ValidateCurrent()
    {
        D3DiagnosticPanelUI panel = Object.FindFirstObjectByType<D3DiagnosticPanelUI>(
            FindObjectsInactive.Include);
        Transform visual = panel == null ? null : panel.transform.Find("D3_DiagnosticVisual");
        D3DiagnosticStaticSkinUI skin = visual == null
            ? null : visual.GetComponent<D3DiagnosticStaticSkinUI>();
        bool valid = panel != null && visual != null && skin != null &&
            skin.referencePlate != null && skin.referencePlate.texture != null &&
            !skin.referencePlate.raycastTarget &&
            visual.GetSiblingIndex() == panel.transform.childCount - 1 &&
            HasTouchTarget(panel.toggleAnalyzeButton) &&
            HasTouchTarget(panel.toggleRepairButton) &&
            HasTouchTarget(panel.toggleFusionButton) &&
            HasTouchTarget(panel.priorityModeDropdown) &&
            HasTouchTarget(panel.zoneDropdown) &&
            HasTouchTarget(panel.leReserveDropdown) &&
            HasTouchTarget(panel.tracesReserveDropdown) &&
            HasTouchTarget(panel.recipeDropdown) &&
            HasTouchTarget(panel.toggleRecipeMarkButton) &&
            HasTouchTarget(panel.saveSettingsButton) &&
            HasTouchTarget(panel.saveRoutineButton) &&
            HasTouchTarget(panel.loadRoutineButton) &&
            HasTouchTarget(panel.backButton);
        if (valid)
            Debug.Log("[D3 Diagnostic Visual] VALIDATION PASS");
        else
            Debug.LogError("[D3 Diagnostic Visual] VALIDATION FAIL");
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

    private static void EnsureReferenceImport()
    {
        AssetDatabase.ImportAsset(ReferencePath, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter importer = AssetImporter.GetAtPath(ReferencePath) as TextureImporter;
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

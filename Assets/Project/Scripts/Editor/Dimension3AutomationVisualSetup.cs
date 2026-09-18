#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public static class Dimension3AutomationVisualSetup
{
    private const string MainScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ReferencePath =
        "Assets/Project/Art/Dimension3/Automation/D3_RutinasPerfiles_Reference_Final_v1.png";
    private const float SourceHeight = 1920f;
    private const float DisplayHeight = 1890f;
    private const float ScaleY = DisplayHeight / SourceHeight;
    private static readonly Vector2 ViewportCompensation = new Vector2(-15f, 17f);

    [MenuItem("Tools/Quantum Forge/Dimension 3/Apply Automation Visual")]
    public static void ConfigureOnly()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != MainScenePath)
            scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
        D3AutomationPanelUI panel = Object.FindFirstObjectByType<D3AutomationPanelUI>(
            FindObjectsInactive.Include);
        if (panel == null)
        {
            Debug.LogError("[D3 Automation Visual] Falta D3AutomationPanelUI.");
            return;
        }

        ApplyTo(panel);
        EditorUtility.SetDirty(panel);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene))
        {
            Debug.LogError("[D3 Automation Visual] No se pudo guardar Main.unity.");
            return;
        }
        ValidateCurrent();
        Debug.Log("[D3 Automation Visual] APPLY PASS");
    }

    public static void ConfigureOnlyBatch() => ConfigureOnly();

    public static void ApplyTo(D3AutomationPanelUI panel)
    {
        EnsureReferenceImport();
        Texture2D reference = AssetDatabase.LoadAssetAtPath<Texture2D>(ReferencePath);
        if (reference == null)
        {
            Debug.LogError("[D3 Automation Visual] No se pudo cargar la referencia final.");
            return;
        }

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        if (panelRect != null)
        {
            panelRect.anchoredPosition = ViewportCompensation;
            panelRect.sizeDelta = Vector2.zero;
            panelRect.localScale = Vector3.one;
        }

        Transform previous = panel.transform.Find("D3_AutomationVisual");
        if (previous != null)
            Object.DestroyImmediate(previous.gameObject);
        GameObject visual = Create("D3_AutomationVisual", panel.transform);
        SetTopLeft(visual.GetComponent<RectTransform>(), 0f, 0f, 1080f, DisplayHeight);
        RawImage plate = visual.AddComponent<RawImage>();
        plate.texture = reference;
        plate.uvRect = new Rect(0f, 0f, 1f, 1f);
        plate.color = Color.white;
        plate.raycastTarget = false;
        D3AutomationStaticSkinUI skin = visual.AddComponent<D3AutomationStaticSkinUI>();
        skin.referencePlate = plate;
        visual.transform.SetAsLastSibling();

        SetControl(panel.backButton, 55, 40, 220, 95);
        SetControl(panel.actionDropdown, 55, 950, 286, 77);
        SetControl(panel.targetDropdown, 55, 950, 286, 77);
        SetControl(panel.priorityDropdown, 350, 950, 224, 77);
        SetControl(panel.stopDropdown, 583, 950, 237, 77);
        SetControl(panel.reserveDropdown, 829, 950, 225, 77);
        SetControl(panel.createButton, 282, 1038, 542, 77);
        SetControl(panel.routineDropdown, 52, 1124, 682, 66);
        SetControl(panel.toggleButton, 86, 1480, 448, 79);
        SetControl(panel.deleteButton, 568, 1480, 409, 79);
        SetControl(panel.profileDropdown, 99, 1584, 278, 71);
        SetControl(panel.saveProfileButton, 394, 1584, 312, 71);
        SetControl(panel.loadProfileButton, 727, 1584, 301, 71);

        EditorUtility.SetDirty(skin);
        EditorUtility.SetDirty(panel);
    }

    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Automation Visual")]
    public static void ValidateCurrent()
    {
        D3AutomationPanelUI panel = Object.FindFirstObjectByType<D3AutomationPanelUI>(
            FindObjectsInactive.Include);
        Transform visual = panel == null ? null : panel.transform.Find("D3_AutomationVisual");
        D3AutomationStaticSkinUI skin = visual == null
            ? null : visual.GetComponent<D3AutomationStaticSkinUI>();
        bool valid = panel != null && visual != null && skin != null &&
            skin.referencePlate != null && skin.referencePlate.texture != null &&
            !skin.referencePlate.raycastTarget &&
            visual.GetSiblingIndex() == panel.transform.childCount - 1 &&
            HasTouchTarget(panel.backButton) &&
            HasTouchTarget(panel.actionDropdown) &&
            HasTouchTarget(panel.targetDropdown) &&
            HasTouchTarget(panel.priorityDropdown) &&
            HasTouchTarget(panel.stopDropdown) &&
            HasTouchTarget(panel.reserveDropdown) &&
            HasTouchTarget(panel.createButton) &&
            HasTouchTarget(panel.routineDropdown) &&
            HasTouchTarget(panel.toggleButton) &&
            HasTouchTarget(panel.deleteButton) &&
            HasTouchTarget(panel.profileDropdown) &&
            HasTouchTarget(panel.saveProfileButton) &&
            HasTouchTarget(panel.loadProfileButton);
        if (valid)
            Debug.Log("[D3 Automation Visual] VALIDATION PASS");
        else
            Debug.LogError("[D3 Automation Visual] VALIDATION FAIL");
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
        SetTopLeft(control.GetComponent<RectTransform>(), x, y * ScaleY,
            width, height * ScaleY);
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

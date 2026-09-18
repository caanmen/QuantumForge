#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public static class Dimension3ConsoleVisualSetup
{
    private const string MainScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ReferencePath =
        "Assets/Project/Art/Dimension3/Console/D3_ControlConsola_Reference_Final_v1.png";
    private const float SourceHeight = 1920f;
    private const float DisplayHeight = 1890f;
    private const float ScaleY = DisplayHeight / SourceHeight;
    private static readonly Vector2 ViewportCompensation = new Vector2(-15f, 17f);

    [MenuItem("Tools/Quantum Forge/Dimension 3/Apply Console Visual")]
    public static void ConfigureOnly()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != MainScenePath)
            scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
        D3ConsolePanelUI panel = Object.FindFirstObjectByType<D3ConsolePanelUI>(
            FindObjectsInactive.Include);
        if (panel == null)
        {
            Debug.LogError("[D3 Console Visual] Falta D3ConsolePanelUI.");
            return;
        }

        ApplyTo(panel);
        EditorUtility.SetDirty(panel);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene))
        {
            Debug.LogError("[D3 Console Visual] No se pudo guardar Main.unity.");
            return;
        }
        ValidateCurrent();
        Debug.Log("[D3 Console Visual] APPLY PASS");
    }

    public static void ConfigureOnlyBatch() => ConfigureOnly();

    public static void ApplyTo(D3ConsolePanelUI panel)
    {
        EnsureReferenceImport();
        Texture2D reference = AssetDatabase.LoadAssetAtPath<Texture2D>(ReferencePath);
        if (reference == null)
        {
            Debug.LogError("[D3 Console Visual] No se pudo cargar la referencia final.");
            return;
        }

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        if (panelRect != null)
        {
            panelRect.anchoredPosition = ViewportCompensation;
            panelRect.sizeDelta = Vector2.zero;
            panelRect.localScale = Vector3.one;
        }

        Transform previous = panel.transform.Find("D3_ConsoleVisual");
        if (previous != null)
            Object.DestroyImmediate(previous.gameObject);
        GameObject visual = Create("D3_ConsoleVisual", panel.transform);
        SetTopLeft(visual.GetComponent<RectTransform>(), 0f, 0f, 1080f, DisplayHeight);
        RawImage plate = visual.AddComponent<RawImage>();
        plate.texture = reference;
        plate.uvRect = new Rect(0f, 0f, 1f, 1f);
        plate.color = Color.white;
        plate.raycastTarget = false;
        D3ConsoleStaticSkinUI skin = visual.AddComponent<D3ConsoleStaticSkinUI>();
        skin.referencePlate = plate;
        visual.transform.SetAsLastSibling();

        SetControl(panel.policyDropdown, 528, 1177, 447, 67);
        SetControl(panel.leReserveDropdown, 528, 1246, 447, 67);
        SetControl(panel.tracesReserveDropdown, 528, 1313, 447, 67);
        SetControl(panel.savePolicyButton, 56, 1463, 480, 87);
        SetControl(panel.recordTriangleButton, 551, 1463, 464, 87);
        SetControl(panel.openRoutinesButton, 56, 1553, 480, 87);
        SetControl(panel.backButton, 551, 1553, 464, 87);
        if (panel.recordPhaseButton != null)
            panel.recordPhaseButton.gameObject.SetActive(false);

        EditorUtility.SetDirty(skin);
        EditorUtility.SetDirty(panel);
    }

    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Console Visual")]
    public static void ValidateCurrent()
    {
        D3ConsolePanelUI panel = Object.FindFirstObjectByType<D3ConsolePanelUI>(
            FindObjectsInactive.Include);
        Transform visual = panel == null ? null : panel.transform.Find("D3_ConsoleVisual");
        D3ConsoleStaticSkinUI skin = visual == null
            ? null : visual.GetComponent<D3ConsoleStaticSkinUI>();
        bool valid = panel != null && visual != null && skin != null &&
            skin.referencePlate != null && skin.referencePlate.texture != null &&
            !skin.referencePlate.raycastTarget &&
            visual.GetSiblingIndex() == panel.transform.childCount - 1 &&
            HasTouchTarget(panel.policyDropdown) &&
            HasTouchTarget(panel.leReserveDropdown) &&
            HasTouchTarget(panel.tracesReserveDropdown) &&
            HasTouchTarget(panel.savePolicyButton) &&
            HasTouchTarget(panel.recordTriangleButton) &&
            HasTouchTarget(panel.openRoutinesButton) &&
            HasTouchTarget(panel.backButton) &&
            (panel.recordPhaseButton == null || !panel.recordPhaseButton.gameObject.activeSelf);
        if (valid)
            Debug.Log("[D3 Console Visual] VALIDATION PASS");
        else
            Debug.LogError("[D3 Console Visual] VALIDATION FAIL");
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

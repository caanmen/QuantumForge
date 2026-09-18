#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public static class Dimension3QueuesVisualSetup
{
    private const string MainScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ReferencePath =
        "Assets/Project/Art/Dimension3/Queues/D3_GestionColas_Reference_Final_v1.png";
    private const float SourceHeight = 1920f;
    private const float DisplayHeight = 1920f;
    private const float ScaleY = DisplayHeight / SourceHeight;
    private static readonly Vector2 ViewportCompensation = new Vector2(-25f, 32f);

    [MenuItem("Tools/Quantum Forge/Dimension 3/Apply Queues Visual")]
    public static void ConfigureOnly()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != MainScenePath)
            scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
        D3QueuesPanelUI panel = Object.FindFirstObjectByType<D3QueuesPanelUI>(
            FindObjectsInactive.Include);
        if (panel == null)
        {
            Debug.LogError("[D3 Queues Visual] Falta D3QueuesPanelUI.");
            return;
        }

        ApplyTo(panel);
        EditorUtility.SetDirty(panel);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene))
        {
            Debug.LogError("[D3 Queues Visual] No se pudo guardar Main.unity.");
            return;
        }
        ValidateCurrent();
        Debug.Log("[D3 Queues Visual] APPLY PASS");
    }

    public static void ConfigureOnlyBatch() => ConfigureOnly();

    public static void ApplyTo(D3QueuesPanelUI panel)
    {
        EnsureReferenceImport();
        Texture2D reference = AssetDatabase.LoadAssetAtPath<Texture2D>(ReferencePath);
        if (reference == null)
        {
            Debug.LogError("[D3 Queues Visual] No se pudo cargar la referencia final.");
            return;
        }

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        if (panelRect != null)
        {
            panelRect.anchoredPosition = ViewportCompensation;
            panelRect.sizeDelta = Vector2.zero;
            panelRect.localScale = Vector3.one;
        }

        Transform previous = panel.transform.Find("D3_QueuesVisual");
        if (previous != null)
            Object.DestroyImmediate(previous.gameObject);
        GameObject visual = Create("D3_QueuesVisual", panel.transform);
        SetTopLeft(visual.GetComponent<RectTransform>(), 0f, 0f, 1080f, DisplayHeight);
        RawImage plate = visual.AddComponent<RawImage>();
        plate.texture = reference;
        plate.uvRect = new Rect(0f, 0f, 1f, 1f);
        plate.color = Color.white;
        plate.raycastTarget = false;
        D3QueuesStaticSkinUI skin = visual.AddComponent<D3QueuesStaticSkinUI>();
        skin.referencePlate = plate;
        visual.transform.SetAsLastSibling();

        SetControl(panel.backButton, 802, 673, 229, 78);
        SetControl(panel.queueDropdown, 282, 773, 705, 102);
        SetControl(panel.jobDropdown, 282, 900, 705, 102);
        SetControl(panel.cancelSelectedButton, 98, 1471, 891, 101);

        EditorUtility.SetDirty(skin);
        EditorUtility.SetDirty(panel);
    }

    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Queues Visual")]
    public static void ValidateCurrent()
    {
        D3QueuesPanelUI panel = Object.FindFirstObjectByType<D3QueuesPanelUI>(
            FindObjectsInactive.Include);
        Transform visual = panel == null ? null : panel.transform.Find("D3_QueuesVisual");
        D3QueuesStaticSkinUI skin = visual == null
            ? null : visual.GetComponent<D3QueuesStaticSkinUI>();
        bool valid = panel != null && visual != null && skin != null &&
            skin.referencePlate != null && skin.referencePlate.texture != null &&
            !skin.referencePlate.raycastTarget &&
            visual.GetSiblingIndex() == panel.transform.childCount - 1 &&
            HasTouchTarget(panel.backButton) &&
            HasTouchTarget(panel.queueDropdown) &&
            HasTouchTarget(panel.jobDropdown) &&
            HasTouchTarget(panel.cancelSelectedButton);
        if (valid)
            Debug.Log("[D3 Queues Visual] VALIDATION PASS");
        else
            Debug.LogError("[D3 Queues Visual] VALIDATION FAIL");
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

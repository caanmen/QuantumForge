#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public static class Dimension3CalibrationVisualSetup
{
    private const string MainScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ReferencePath =
        "Assets/Project/Art/Dimension3/Calibration/D3_MesaCalibracion_Reference_Final_v1.png";
    private const float SourceHeight = 1920f;
    private const float DisplayHeight = 1890f;
    private const float ScaleY = DisplayHeight / SourceHeight;
    private static readonly Vector2 ViewportCompensation = new Vector2(-15f, 17f);

    [MenuItem("Tools/Quantum Forge/Dimension 3/Apply Calibration Visual")]
    public static void ConfigureOnly()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != MainScenePath)
            scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);

        D3CalibrationPanelUI panel = Object.FindFirstObjectByType<D3CalibrationPanelUI>(
            FindObjectsInactive.Include);
        if (panel == null)
        {
            Debug.LogError("[D3 Calibration Visual] Falta D3CalibrationPanelUI.");
            return;
        }

        ApplyTo(panel);
        EditorUtility.SetDirty(panel);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene))
        {
            Debug.LogError("[D3 Calibration Visual] No se pudo guardar Main.unity.");
            return;
        }

        ValidateCurrent();
        Debug.Log("[D3 Calibration Visual] APPLY PASS");
    }

    public static void ConfigureOnlyBatch()
    {
        ConfigureOnly();
    }

    public static void ApplyTo(D3CalibrationPanelUI panel)
    {
        if (panel == null)
            return;

        EnsureReferenceImport();
        Texture2D reference = AssetDatabase.LoadAssetAtPath<Texture2D>(ReferencePath);
        if (reference == null)
        {
            Debug.LogError("[D3 Calibration Visual] No se pudo cargar la referencia final.");
            return;
        }

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        if (panelRect != null)
        {
            panelRect.anchoredPosition = ViewportCompensation;
            panelRect.sizeDelta = Vector2.zero;
            panelRect.localScale = Vector3.one;
        }

        Transform previous = panel.transform.Find("D3_CalibrationVisual");
        if (previous != null)
            Object.DestroyImmediate(previous.gameObject);

        GameObject visual = Create("D3_CalibrationVisual", panel.transform);
        SetTopLeft(visual.GetComponent<RectTransform>(), 0f, 0f, 1080f, DisplayHeight);
        RawImage plate = visual.AddComponent<RawImage>();
        plate.texture = reference;
        plate.uvRect = new Rect(0f, 0f, 1f, 1f);
        plate.color = Color.white;
        plate.raycastTarget = false;
        D3CalibrationStaticSkinUI skin = visual.AddComponent<D3CalibrationStaticSkinUI>();
        skin.referencePlate = plate;
        visual.transform.SetAsLastSibling();

        // Los controles reales quedan alineados debajo de la composición estática.
        // La placa no recibe raycasts, por lo que la lógica puede probarse sin crear
        // todavía una segunda fuente visual para cada estado.
        SetControl(panel.backButton, 31, 31, 174, 91);
        SetControl(panel.partDropdown, 59, 294, 495, 101);
        SetControl(panel.mkDropdown, 582, 294, 439, 101);
        SetControl(panel.quantityDropdown, 582, 294, 439, 101);
        SetControl(panel.valueASlider, 342, 515, 448, 100);
        SetControl(panel.valueBSlider, 342, 636, 448, 100);
        SetControl(panel.valueCSlider, 342, 757, 448, 100);
        SetControl(panel.recordPartButton, 153, 879, 765, 105);
        SetControl(panel.saveProfileButton, 38, 1625, 205, 126);
        SetControl(panel.loadProfileButton, 252, 1625, 201, 126);
        SetControl(panel.autoRepeatPartButton, 461, 1625, 196, 126);
        SetControl(panel.autoRepeatAllButton, 665, 1625, 196, 126);
        SetControl(panel.queueTraitAssemblyButton, 870, 1625, 171, 126);

        EditorUtility.SetDirty(skin);
        EditorUtility.SetDirty(panel);
    }

    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Calibration Visual")]
    public static void ValidateCurrent()
    {
        D3CalibrationPanelUI panel = Object.FindFirstObjectByType<D3CalibrationPanelUI>(
            FindObjectsInactive.Include);
        Transform visual = panel == null ? null : panel.transform.Find("D3_CalibrationVisual");
        D3CalibrationStaticSkinUI skin = visual == null
            ? null
            : visual.GetComponent<D3CalibrationStaticSkinUI>();
        bool valid = panel != null && visual != null && skin != null &&
            skin.referencePlate != null && skin.referencePlate.texture != null &&
            !skin.referencePlate.raycastTarget &&
            visual.GetSiblingIndex() == panel.transform.childCount - 1 &&
            HasTouchTarget(panel.backButton) && HasTouchTarget(panel.partDropdown) &&
            HasTouchTarget(panel.mkDropdown) && HasTouchTarget(panel.valueASlider) &&
            HasTouchTarget(panel.valueBSlider) && HasTouchTarget(panel.valueCSlider) &&
            HasTouchTarget(panel.recordPartButton) &&
            HasTouchTarget(panel.queueTraitAssemblyButton);

        if (valid)
            Debug.Log("[D3 Calibration Visual] VALIDATION PASS");
        else
            Debug.LogError("[D3 Calibration Visual] VALIDATION FAIL");
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
        RectTransform rect = control.GetComponent<RectTransform>();
        SetTopLeft(rect, x, y * ScaleY, width, height * ScaleY);
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

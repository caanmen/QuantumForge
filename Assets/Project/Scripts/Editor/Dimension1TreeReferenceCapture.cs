#if UNITY_EDITOR
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Dimension1TreeReferenceCapture
{
    private const string ActiveKey = "QF.D1TreeCapture.Active";
    private const string FrameKey = "QF.D1TreeCapture.Frame";
    private const string FailureKey = "QF.D1TreeCapture.Failed";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory => Path.GetFullPath(
        "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/DIMENSION_1/ARBOL_CUANTICO");
    private static string OutputPath => Path.Combine(OutputDirectory, "CAPTURA_CANDIDATA_1080x1920.png");
    private static string OutputPath720 => Path.Combine(OutputDirectory, "CAPTURA_CANDIDATA_720x1280.png");

    [InitializeOnLoadMethod]
    private static void Resume()
    {
        if (!SessionState.GetBool(ActiveKey, false)) return;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        if (!EditorApplication.isPlaying) return;
        EditorApplication.update -= Tick;
        EditorApplication.update += Tick;
    }

    public static void Run()
    {
        Directory.CreateDirectory(OutputDirectory);
        if (File.Exists(OutputPath)) File.Delete(OutputPath);
        if (File.Exists(OutputPath720)) File.Delete(OutputPath720);
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetBool(FailureKey, false);
        SessionState.SetInt(FrameKey, 0);
        SaveService.SuppressWritesForVisualQa = true;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        EditorApplication.isPlaying = true;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            SaveService.SuppressWritesForVisualQa = true;
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            SaveService.SuppressWritesForVisualQa = false;
            SessionState.SetBool(ActiveKey, false);
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            bool failed = SessionState.GetBool(FailureKey, false);
            Debug.Log((failed ? "[D1 Tree Capture] FAIL | " : "[D1 Tree Capture] PASS | ") + OutputPath);
            EditorApplication.Exit(failed ? 1 : 0);
        }
    }

    private static void Tick()
    {
        try
        {
            int frame = SessionState.GetInt(FrameKey, 0) + 1;
            SessionState.SetInt(FrameKey, frame);
            if (frame == 24) Prepare();
            if (frame >= 24 && frame <= 60) ForceVisible();
            if (frame != 60) return;
            ValidateVisibleScreen();
            RenderToPng(1080, 1920, OutputPath);
            RenderToPng(720, 1280, OutputPath720);
            EditorApplication.update -= Tick;
            EditorApplication.isPlaying = false;
        }
        catch (System.Exception exception)
        {
            Debug.LogException(exception);
            SessionState.SetBool(FailureKey, true);
            EditorApplication.update -= Tick;
            EditorApplication.isPlaying = false;
        }
    }

    private static void Prepare()
    {
        SaveService.SuppressWritesForVisualQa = true;
        TabsUI tabs = TabsUI.Instance != null ? TabsUI.Instance : Object.FindFirstObjectByType<TabsUI>();
        if (tabs != null) tabs.ShowDimension1();
        Dimension1PanelUI panel = Object.FindFirstObjectByType<Dimension1PanelUI>(FindObjectsInactive.Include);
        if (panel == null) throw new System.InvalidOperationException("No existe Dimension1PanelUI.");
        panel.OnClickOpenDimension1TreePanel();
        Dimension1TreeVisualUI visual = Object.FindFirstObjectByType<Dimension1TreeVisualUI>(
            FindObjectsInactive.Include);
        if (visual == null) throw new System.InvalidOperationException("No existe Dimension1TreeVisualUI.");
        visual.SetReferencePreviewForVisualQa(true);
        visual.SelectNode(8);
        string[] hide =
        {
            "Dimension2Panel", "Dimension3Panel", "Panel_Generacion", "Panel_Lab", "Panel_Logros",
            "Panel_Ajustes", "Panel_HUD", "HUD", "BottomDrawer", "PrimaryNavigationSlot",
            "SecondaryNavigationSlot", "MachineContextTabs", "PrestigePanel", "MetaPrestigePanel",
            "QA_PanelRoot", "QA_ToolsButton"
        };
        foreach (string name in hide) SetSceneObjectActive(name, false);
        ForceVisible();
        PrepareCanvasesForCapture();
    }

    private static void ForceVisible()
    {
        PresentationReturnReportService.Consume();
        foreach (PresentationReturnReportUI report in
                 Object.FindObjectsByType<PresentationReturnReportUI>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            report.gameObject.SetActive(false);
        foreach (TriangleOfflineReportUI report in
                 Object.FindObjectsByType<TriangleOfflineReportUI>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            report.gameObject.SetActive(false);
        string[] hideEveryFrame =
        {
            "Panel_Lab", "Panel_Logros", "Panel_Ajustes",
            "BottomDrawer", "PrimaryNavigationSlot", "SecondaryNavigationSlot", "MachineContextTabs",
            "QA_PanelRoot", "QA_ToolsButton"
        };
        foreach (string name in hideEveryFrame) SetSceneObjectActive(name, false);
        Transform root = FindSceneTransform("D1_TreeVisualRoot");
        if (root == null) throw new System.InvalidOperationException("Falta D1_TreeVisualRoot.");
        Transform current = root;
        while (current != null)
        {
            current.gameObject.SetActive(true);
            current = current.parent;
        }
        string[] close =
        {
            "GalaxyPanel", "RelicChamberPanel", "HangarPanel", "ArkPanel", "ExplorationRewardsPanel",
            "Exploration Record Panel", "D1_ExploreVisualRoot", "D1_HangarVisualRoot", "D1_RelicsVisualRoot"
        };
        foreach (string name in close) SetSceneObjectActive(name, false);
        Transform legacy = FindSceneTransform("D1_TreeLegacyFunctionalRoot");
        if (legacy != null) legacy.gameObject.SetActive(false);
        CanvasGroup group = root.GetComponent<CanvasGroup>();
        if (group != null)
        {
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }
        // Activar la cadena de padres puede disparar OnEnable de TabsUI y reabrir
        // navegación global; el aislamiento definitivo debe aplicarse al final.
        foreach (string name in hideEveryFrame) SetSceneObjectActive(name, false);
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateVisibleScreen()
    {
        Transform root = FindSceneTransform("D1_TreeVisualRoot");
        if (root == null || !root.gameObject.activeInHierarchy)
            throw new System.InvalidOperationException("La pantalla Árbol Cuántico no quedó visible.");
        string[] required =
        {
            "DimensionTitle", "MainHeading", "AvailablePoints", "OrbitalConstellation", "TreeNode_1",
            "TreeNode_9", "TreeNode_10", "SelectedNodeDetail", "UnlockButton", "BottomNavigation"
        };
        foreach (string name in required)
            if (FindChild(root, name) == null) throw new System.InvalidOperationException("Falta bloque: " + name);
        TMP_Text title = FindChild(root, "MainHeading")?.GetComponent<TMP_Text>();
        if (title == null || title.text != "ÁRBOL CUÁNTICO")
            throw new System.InvalidOperationException("Título del Árbol Cuántico inválido.");
        TMP_Text points = FindChild(root, "AvailablePoints")?.GetComponent<TMP_Text>();
        if (points == null || points.text != "3")
            throw new System.InvalidOperationException("El escenario visual debe mostrar 3 puntos disponibles.");
        TMP_Text cost = FindChild(FindChild(root, "CostPanel"), "Value")?.GetComponent<TMP_Text>();
        if (cost == null || cost.text != "3 PUNTOS")
            throw new System.InvalidOperationException("Cartografía Avanzada debe costar 3 puntos.");
        if (FindSceneTransform("PrimaryNavigationSlot")?.gameObject.activeSelf == true)
            throw new System.InvalidOperationException("La navegación global se mezcló con el Árbol Cuántico.");
        CanvasGroup group = root.GetComponent<CanvasGroup>();
        if (root.GetComponent<GraphicRaycaster>() == null || group == null ||
            !group.interactable || !group.blocksRaycasts)
            throw new System.InvalidOperationException("La captura no corresponde al Árbol interactivo real.");
        for (int i = 1; i <= Dimension1System.Dimension1TreeNodeIds.Length; i++)
            if (FindChild(root, "TreeNode_" + i)?.GetComponent<Button>() == null)
                throw new System.InvalidOperationException("El nodo " + i + " no es interactivo.");
        if (FindChild(root, "UnlockButton")?.GetComponent<Button>() == null)
            throw new System.InvalidOperationException("El botón de compra no es interactivo.");
    }

    private static void PrepareCanvasesForCapture()
    {
        Camera camera = Camera.main != null ? Camera.main : Object.FindFirstObjectByType<Camera>();
        if (camera == null) throw new System.InvalidOperationException("No hay cámara para QA visual.");
        Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (Canvas canvas in canvases)
        {
            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay) continue;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 1f;
        }
        Canvas.ForceUpdateCanvases();
    }

    private static void RenderToPng(int width, int height, string path)
    {
        Camera camera = Camera.main != null ? Camera.main : Object.FindFirstObjectByType<Camera>();
        if (camera == null) throw new System.InvalidOperationException("No hay cámara para captura.");
        Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        var modes = new RenderMode[canvases.Length];
        var cameras = new Camera[canvases.Length];
        for (int i = 0; i < canvases.Length; i++)
        {
            modes[i] = canvases[i].renderMode;
            cameras[i] = canvases[i].worldCamera;
            if (modes[i] != RenderMode.ScreenSpaceOverlay) continue;
            canvases[i].renderMode = RenderMode.ScreenSpaceCamera;
            canvases[i].worldCamera = camera;
            canvases[i].planeDistance = 1f;
        }
        RenderTexture target = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        RenderTexture previousTarget = camera.targetTexture;
        RenderTexture previousActive = RenderTexture.active;
        try
        {
            camera.targetTexture = target;
            Canvas.ForceUpdateCanvases();
            camera.Render();
            Canvas.ForceUpdateCanvases();
            camera.Render();
            RenderTexture.active = target;
            Texture2D image = new Texture2D(width, height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            image.Apply();
            File.WriteAllBytes(path, image.EncodeToPNG());
            Object.DestroyImmediate(image);
        }
        finally
        {
            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            target.Release();
            Object.DestroyImmediate(target);
            for (int i = 0; i < canvases.Length; i++)
            {
                canvases[i].renderMode = modes[i];
                canvases[i].worldCamera = cameras[i];
            }
        }
    }

    private static Transform FindSceneTransform(string name)
    {
        foreach (Transform transform in Resources.FindObjectsOfTypeAll<Transform>())
            if (transform.gameObject.scene.IsValid() && transform.name == name) return transform;
        return null;
    }

    private static Transform FindChild(Transform root, string name)
    {
        if (root == null) return null;
        foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
            if (transform.name == name) return transform;
        return null;
    }

    private static void SetSceneObjectActive(string name, bool active)
    {
        foreach (Transform target in Resources.FindObjectsOfTypeAll<Transform>())
            if (target.gameObject.scene.IsValid() && target.name == name)
                target.gameObject.SetActive(active);
    }
}
#endif

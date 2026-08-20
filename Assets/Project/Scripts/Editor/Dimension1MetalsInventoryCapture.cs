#if UNITY_EDITOR
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Dimension1MetalsInventoryCapture
{
    private const string ActiveKey = "QF.D1MetalsInventoryCapture.Active";
    private const string FrameKey = "QF.D1MetalsInventoryCapture.Frame";
    private const string FailureKey = "QF.D1MetalsInventoryCapture.Failed";
    private const string HangarStateKey = "QF.D1MetalsInventoryCapture.HangarWasActive";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory => Path.GetFullPath("Logs/VisualQA/Dimension1/MetalsInventory");
    private static string Output1080 => Path.Combine(OutputDirectory, "Metals_inventory_1080x1920.png");
    private static string Output720 => Path.Combine(OutputDirectory, "Metals_inventory_720x1280.png");

    [InitializeOnLoadMethod]
    private static void Resume()
    {
        if (!SessionState.GetBool(ActiveKey, false)) return;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        if (EditorApplication.isPlaying)
        {
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
        }
    }

    public static void Run()
    {
        Directory.CreateDirectory(OutputDirectory);
        if (File.Exists(Output1080)) File.Delete(Output1080);
        if (File.Exists(Output720)) File.Delete(Output720);
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
            Debug.Log((failed ? "[D1 Metals Inventory Capture] FAIL | " :
                "[D1 Metals Inventory Capture] PASS | open + filters + back + reopen | 1080x1920 + 720x1280 | ") + Output1080);
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
            if (frame == 32) OpenFromHeader();
            if (frame == 40) TestProducingFilter();
            if (frame == 46) TestLockedFilter();
            if (frame == 52) TestBack();
            if (frame == 60) ReopenFromHeader();
            if (frame >= 60 && frame <= 80) ForceVisible();
            if (frame != 80) return;
            ValidateVisibleScreen();
            RenderToPng(1080, 1920, Output1080);
            RenderToPng(720, 1280, Output720);
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
        panel.OnClickOpenHangarPanel();
        Transform hangarRoot = FindSceneTransform("D1_HangarVisualRoot");
        Transform activeParent = hangarRoot;
        while (activeParent != null)
        {
            activeParent.gameObject.SetActive(true);
            activeParent = activeParent.parent;
        }
        string[] hide =
        {
            "Dimension2Panel", "Dimension3Panel", "Panel_Generacion", "Panel_Lab", "Panel_Logros",
            "Panel_Ajustes", "Panel_HUD", "HUD", "BottomDrawer", "PrimaryNavigationSlot",
            "SecondaryNavigationSlot", "MachineContextTabs", "PrestigePanel", "MetaPrestigePanel",
            "QA_PanelRoot", "QA_ToolsButton"
        };
        foreach (string name in hide) SetSceneObjectActive(name, false);
        PrepareCanvasesForCapture();
    }

    private static void OpenFromHeader()
    {
        Transform hangarRoot = FindSceneTransform("D1_HangarVisualRoot");
        Button entry = FindChild(hangarRoot, "MetalsButton")?.GetComponent<Button>();
        if (entry == null) throw new System.InvalidOperationException("Hangar no tiene acceso funcional a metales.");
        SessionState.SetBool(HangarStateKey, hangarRoot.gameObject.activeSelf);
        entry.onClick.Invoke();
        AssertVisible(true, "abrir desde Hangar");
    }

    private static void TestProducingFilter()
    {
        Button filter = FindChild(FindSceneTransform("D1_MetalsInventoryRoot"), "Filter_1")?.GetComponent<Button>();
        if (filter == null) throw new System.InvalidOperationException("Falta filtro Produciendo.");
        filter.onClick.Invoke();
        ValidateFilteredCards(1);
    }

    private static void TestLockedFilter()
    {
        Button filter = FindChild(FindSceneTransform("D1_MetalsInventoryRoot"), "Filter_2")?.GetComponent<Button>();
        if (filter == null) throw new System.InvalidOperationException("Falta filtro Bloqueados.");
        filter.onClick.Invoke();
        ValidateFilteredCards(2);
    }

    private static void TestBack()
    {
        Button back = FindChild(FindSceneTransform("D1_MetalsInventoryRoot"), "BackButton")?.GetComponent<Button>();
        if (back == null) throw new System.InvalidOperationException("Falta botón Regresar.");
        back.onClick.Invoke();
        AssertVisible(false, "regresar");
        Transform hangar = FindSceneTransform("D1_HangarVisualRoot");
        bool expected = SessionState.GetBool(HangarStateKey, false);
        if (hangar == null || hangar.gameObject.activeSelf != expected)
            throw new System.InvalidOperationException("Regresar alteró el estado previo de Hangar.");
    }

    private static void ReopenFromHeader()
    {
        Button entry = FindChild(FindSceneTransform("D1_HangarVisualRoot"), "MetalsButton")?.GetComponent<Button>();
        entry?.onClick.Invoke();
        Button all = FindChild(FindSceneTransform("D1_MetalsInventoryRoot"), "Filter_0")?.GetComponent<Button>();
        all?.onClick.Invoke();
        AssertVisible(true, "reabrir");
    }

    private static void ValidateFilteredCards(int filter)
    {
        GameState state = GameState.I;
        string[] ids =
        {
            Dimension1System.MetalIron, Dimension1System.MetalCopper,
            Dimension1System.MetalAluminum, Dimension1System.MetalTitanium,
            Dimension1System.MetalNickel, Dimension1System.MetalCobalt,
            Dimension1System.MetalLithium, Dimension1System.MetalTungsten,
            Dimension1System.MetalPlatinum, Dimension1System.MetalIridium
        };
        Transform root = FindSceneTransform("D1_MetalsInventoryRoot");
        for (int i = 0; i < ids.Length; i++)
        {
            bool unlocked = Dimension1System.IsMetalUnlockedForDimension1(state, ids[i]);
            double production = Dimension1System.GetMetalProductionPerSecond(state, ids[i]);
            bool expected = filter == 1 ? unlocked && production > 0d : !unlocked;
            Transform card = FindChild(root, "MetalCard_" + i);
            if (card == null || card.gameObject.activeSelf != expected)
                throw new System.InvalidOperationException("El filtro no coincide con los datos reales en MetalCard_" + i + ".");
        }
    }

    private static void ForceVisible()
    {
        PresentationReturnReportService.Consume();
        foreach (PresentationReturnReportUI report in Object.FindObjectsByType<PresentationReturnReportUI>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            report.gameObject.SetActive(false);
        foreach (TriangleOfflineReportUI report in Object.FindObjectsByType<TriangleOfflineReportUI>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            report.gameObject.SetActive(false);
        Transform root = FindSceneTransform("D1_MetalsInventoryRoot");
        if (root == null) throw new System.InvalidOperationException("Falta D1_MetalsInventoryRoot.");
        Transform current = root;
        while (current != null)
        {
            current.gameObject.SetActive(true);
            current = current.parent;
        }
        Dimension1MetalsInventoryUI visual = root.GetComponent<Dimension1MetalsInventoryUI>();
        if (visual == null) throw new System.InvalidOperationException("Falta Dimension1MetalsInventoryUI.");
        visual.Open();
        Canvas.ForceUpdateCanvases();
    }

    private static void AssertVisible(bool expected, string action)
    {
        CanvasGroup group = FindSceneTransform("D1_MetalsInventoryRoot")?.GetComponent<CanvasGroup>();
        bool actual = group != null && group.alpha > .99f && group.blocksRaycasts;
        if (actual != expected) throw new System.InvalidOperationException("Falló " + action + " del inventario.");
    }

    private static void ValidateVisibleScreen()
    {
        AssertVisible(true, "visibilidad final");
        Transform root = FindSceneTransform("D1_MetalsInventoryRoot");
        string[] required = { "Title", "TotalMetals", "UnlockedMetals", "TotalProduction", "Filter_0", "MetalCard_0", "MetalCard_9", "HighestProduction" };
        foreach (string name in required)
            if (FindChild(root, name) == null) throw new System.InvalidOperationException("Falta bloque: " + name);
        TMP_Text title = FindChild(root, "Title")?.GetComponent<TMP_Text>();
        if (title == null || title.text != "INVENTARIO DE METALES")
            throw new System.InvalidOperationException("Título del inventario inválido.");
    }

    private static void PrepareCanvasesForCapture()
    {
        Camera camera = Camera.main != null ? Camera.main : Object.FindFirstObjectByType<Camera>();
        if (camera == null) throw new System.InvalidOperationException("No hay cámara para QA visual.");
        foreach (Canvas canvas in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
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
        Transform target = FindSceneTransform(name);
        if (target != null) target.gameObject.SetActive(active);
    }
}
#endif

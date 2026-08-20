#if UNITY_EDITOR
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Dimension1RelicsReferenceCapture
{
    private const string ActiveKey = "QF.D1RelicsCapture.Active";
    private const string FrameKey = "QF.D1RelicsCapture.Frame";
    private const string FailureKey = "QF.D1RelicsCapture.Failed";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory => Path.GetFullPath("Logs/VisualQA/Dimension1/RelicsReference");
    private static string OutputPath => Path.Combine(OutputDirectory, "Relics_reference_1080x1920.png");
    private static string OutputPath720 => Path.Combine(OutputDirectory, "Relics_reference_720x1280.png");
    private static string OutputPathPage2 => Path.Combine(OutputDirectory, "Relics_page_2_1080x1920.png");
    private static string OutputPathPage2_720 => Path.Combine(OutputDirectory, "Relics_page_2_720x1280.png");
    private static string OutputPathPage3 => Path.Combine(OutputDirectory, "Relics_page_3_1080x1920.png");
    private static string OutputPathPage3_720 => Path.Combine(OutputDirectory, "Relics_page_3_720x1280.png");

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
        string[] outputs =
        {
            OutputPath, OutputPath720, OutputPathPage2, OutputPathPage2_720,
            OutputPathPage3, OutputPathPage3_720
        };
        foreach (string output in outputs) if (File.Exists(output)) File.Delete(output);
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
            Debug.Log((failed ? "[D1 Relics Capture] FAIL | " : "[D1 Relics Capture] PASS | ") + OutputPath);
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
            if (frame >= 24 && frame <= 96) ForceVisible();
            Dimension1RelicsVisualUI visual = GetVisual();
            if (frame == 60)
            {
                ValidateVisibleScreen(0, "CRISTAL ANALÍTICO");
                RenderToPng(1080, 1920, OutputPath);
                RenderToPng(720, 1280, OutputPath720);
                visual.NextPage();
            }
            else if (frame == 72)
            {
                ValidateVisibleScreen(1, "PLACA DE EXPLORADOR");
                RenderToPng(1080, 1920, OutputPathPage2);
                RenderToPng(720, 1280, OutputPathPage2_720);
                visual.SelectRelic7();
                ValidateSelectedDetail("NÚCLEO DE BODEGA ANTIGUA");
                visual.NextPage();
            }
            else if (frame == 84)
            {
                ValidateVisibleScreen(2, "FRAGMENTO DE CALIBRACIÓN");
                RenderToPng(1080, 1920, OutputPathPage3);
                RenderToPng(720, 1280, OutputPathPage3_720);
                visual.SelectRelic3();
                ValidateSelectedDetail("MEMORIA DE MÁQUINA");
                visual.PreviousPage();
            }
            else if (frame == 90)
            {
                ValidateVisibleScreen(1, "PLACA DE EXPLORADOR");
                visual.PreviousPage();
            }
            else if (frame == 96)
            {
                ValidateVisibleScreen(0, "BRÚJULA DE DERIVA");
                EditorApplication.update -= Tick;
                EditorApplication.isPlaying = false;
            }
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
        panel.OnClickOpenRelicChamberPanel();
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
        Transform root = FindSceneTransform("D1_RelicsVisualRoot");
        if (root == null) throw new System.InvalidOperationException("Falta D1_RelicsVisualRoot.");
        Transform current = root;
        while (current != null)
        {
            current.gameObject.SetActive(true);
            current = current.parent;
        }
        string[] close =
        {
            "GalaxyPanel", "HangarPanel", "Dimension1TreePanel", "ArkPanel", "ExplorationRewardsPanel",
            "Exploration Record Panel", "D1_ExploreVisualRoot", "D1_HangarVisualRoot"
        };
        foreach (string name in close) SetSceneObjectActive(name, false);
        CanvasGroup group = root.GetComponent<CanvasGroup>();
        if (group != null)
        {
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }
        Dimension1RelicsVisualUI visual = root.GetComponent<Dimension1RelicsVisualUI>();
        if (visual == null) throw new System.InvalidOperationException("Falta Dimension1RelicsVisualUI.");
        visual.SetReferencePreviewForVisualQa(true);
        Canvas.ForceUpdateCanvases();
    }

    private static Dimension1RelicsVisualUI GetVisual()
    {
        Transform root = FindSceneTransform("D1_RelicsVisualRoot");
        Dimension1RelicsVisualUI visual = root != null ? root.GetComponent<Dimension1RelicsVisualUI>() : null;
        if (visual == null) throw new System.InvalidOperationException("Falta Dimension1RelicsVisualUI.");
        return visual;
    }

    private static void ValidateVisibleScreen(int expectedPage, string expectedDetail)
    {
        Transform root = FindSceneTransform("D1_RelicsVisualRoot");
        if (root == null || !root.gameObject.activeInHierarchy)
            throw new System.InvalidOperationException("La pantalla Reliquias no quedó visible.");
        string[] required =
        {
            "DimensionTitle", "MainHeading", "RelicCard_0", "RelicCard_7", "RelicDetail",
            "DetailArt", "UpgradeButton", "BottomNavigation"
        };
        foreach (string name in required)
            if (FindChild(root, name) == null) throw new System.InvalidOperationException("Falta bloque: " + name);
        TMP_Text title = FindChild(root, "MainHeading")?.GetComponent<TMP_Text>();
        if (title == null || title.text != "CÁMARA DE RELIQUIAS")
            throw new System.InvalidOperationException("Título de Reliquias inválido.");
        Dimension1RelicsVisualUI visual = GetVisual();
        if (visual.CurrentPage != expectedPage)
            throw new System.InvalidOperationException("Página de Reliquias incorrecta: " + visual.CurrentPage);
        TMP_Text indicator = FindChild(root, "PageIndicator")?.GetComponent<TMP_Text>();
        string expectedIndicator = (expectedPage + 1) + " / 3";
        if (indicator == null || indicator.text != expectedIndicator)
            throw new System.InvalidOperationException("Indicador de página inválido: " + indicator?.text);
        ValidateSelectedDetail(expectedDetail);
        Transform card0 = FindChild(root, "RelicCard_0");
        Image art0 = FindChild(card0, "RelicArt")?.GetComponent<Image>();
        if (card0 == null || !card0.gameObject.activeSelf || art0 == null || art0.sprite == null)
            throw new System.InvalidOperationException("La primera tarjeta de la página no tiene arte.");
        Transform card4 = FindChild(root, "RelicCard_4");
        if (expectedPage == 2 && card4 != null && card4.gameObject.activeSelf)
            throw new System.InvalidOperationException("La página final debe mostrar sólo cuatro reliquias.");
        if (FindSceneTransform("PrimaryNavigationSlot")?.gameObject.activeSelf == true)
            throw new System.InvalidOperationException("La navegación global se mezcló con Reliquias.");
    }

    private static void ValidateSelectedDetail(string expected)
    {
        Transform root = FindSceneTransform("D1_RelicsVisualRoot");
        TMP_Text detail = FindChild(root, "DetailName")?.GetComponent<TMP_Text>();
        if (detail == null || detail.text != expected)
            throw new System.InvalidOperationException("Reliquia seleccionada inválida. Esperada: " + expected + ", actual: " + detail?.text);
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
            if (modes[i] == RenderMode.ScreenSpaceOverlay)
            {
                canvases[i].renderMode = RenderMode.ScreenSpaceCamera;
                canvases[i].worldCamera = camera;
                canvases[i].planeDistance = 1f;
            }
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

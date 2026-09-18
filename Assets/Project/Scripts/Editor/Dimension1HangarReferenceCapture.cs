#if UNITY_EDITOR
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Dimension1HangarReferenceCapture
{
    private const string ActiveKey = "QF.D1HangarCapture.Active";
    private const string FrameKey = "QF.D1HangarCapture.Frame";
    private const string FailureKey = "QF.D1HangarCapture.Failed";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory => Path.GetFullPath("Logs/VisualQA/Dimension1/HangarReference");
    private static string OutputPath => Path.Combine(OutputDirectory, "Hangar_reference_1080x1920.png");
    private static string OutputPath720 => Path.Combine(OutputDirectory, "Hangar_reference_720x1280.png");
    private static string OutputArmor => Path.Combine(OutputDirectory, "Hangar_blindaje_1080x1920.png");
    private static string OutputArmor720 => Path.Combine(OutputDirectory, "Hangar_blindaje_720x1280.png");
    private static string OutputSensors => Path.Combine(OutputDirectory, "Hangar_sensores_1080x1920.png");
    private static string OutputSensors720 => Path.Combine(OutputDirectory, "Hangar_sensores_720x1280.png");

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
        if (File.Exists(OutputPath)) File.Delete(OutputPath);
        if (File.Exists(OutputPath720)) File.Delete(OutputPath720);
        if (File.Exists(OutputArmor)) File.Delete(OutputArmor);
        if (File.Exists(OutputArmor720)) File.Delete(OutputArmor720);
        if (File.Exists(OutputSensors)) File.Delete(OutputSensors);
        if (File.Exists(OutputSensors720)) File.Delete(OutputSensors720);
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
            Debug.Log((failed ? "[D1 Hangar Capture] FAIL | " : "[D1 Hangar Capture] PASS | ") + OutputPath);
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
            if (frame == 60)
            {
                ValidateVisibleScreen();
                RenderToPng(1080, 1920, OutputPath);
                RenderToPng(720, 1280, OutputPath720);
                GetVisual().SelectArmor();
                return;
            }
            if (frame == 64)
            {
                CaptureSelectedPartVariant(
                    "METALES OBTENIDOS", "RECOMPENSA CONSERVADA", OutputArmor, OutputArmor720);
                GetVisual().SelectSensors();
                return;
            }
            if (frame != 68) return;
            CaptureSelectedPartVariant(
                "PROB. DE FRAGMENTO", " PP", OutputSensors, OutputSensors720,
                "MATRIZ ESPECÍFICA");
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
        Transform root = FindSceneTransform("D1_HangarVisualRoot");
        if (root == null) throw new System.InvalidOperationException("Falta D1_HangarVisualRoot.");
        Transform current = root;
        while (current != null)
        {
            current.gameObject.SetActive(true);
            current = current.parent;
        }
        string[] close =
        {
            "GalaxyPanel", "RelicChamberPanel", "Dimension1TreePanel", "ArkPanel",
            "ExplorationRewardsPanel", "Exploration Record Panel", "D1_ExploreVisualRoot"
        };
        foreach (string name in close) SetSceneObjectActive(name, false);
        CanvasGroup group = root.GetComponent<CanvasGroup>();
        if (group != null)
        {
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }
        Dimension1HangarVisualUI visual = root.GetComponent<Dimension1HangarVisualUI>();
        if (visual == null) throw new System.InvalidOperationException("Falta Dimension1HangarVisualUI.");
        visual.SetReferencePreviewForVisualQa(true);
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateVisibleScreen()
    {
        Transform root = FindSceneTransform("D1_HangarVisualRoot");
        if (root == null || !root.gameObject.activeInHierarchy)
            throw new System.InvalidOperationException("La pantalla Hangar no quedó visible.");
        string[] required =
        {
            "DimensionTitle", "MainHeading", "ShipCard_0", "ShipCard_3", "ShipDisplay",
            "PartCard_0", "PartCard_3", "UpgradeButton", "MissionBonus", "BottomNavigation"
        };
        foreach (string name in required)
            if (FindChild(root, name) == null) throw new System.InvalidOperationException("Falta bloque: " + name);
        TMP_Text title = FindChild(root, "MainHeading")?.GetComponent<TMP_Text>();
        if (title == null || title.text != "HANGAR")
            throw new System.InvalidOperationException("Título Hangar inválido.");
        TMP_Text button = FindChild(FindChild(root, "UpgradeButton"), "Label")?.GetComponent<TMP_Text>();
        if (button == null || !button.text.Contains("VELOCIDAD"))
            throw new System.InvalidOperationException("La mejora seleccionada no coincide con la referencia.");
        if (FindSceneTransform("PrimaryNavigationSlot")?.gameObject.activeSelf == true)
            throw new System.InvalidOperationException("La navegación global se mezcló con Hangar.");
    }

    private static Dimension1HangarVisualUI GetVisual()
    {
        Transform root = FindSceneTransform("D1_HangarVisualRoot");
        Dimension1HangarVisualUI visual = root != null ? root.GetComponent<Dimension1HangarVisualUI>() : null;
        if (visual == null)
            throw new System.InvalidOperationException("Falta el controlador visual del Hangar.");
        return visual;
    }

    private static void CaptureSelectedPartVariant(
        string requiredText, string forbiddenText, string output1080, string output720,
        string secondRequiredText = null)
    {
        Transform root = FindSceneTransform("D1_HangarVisualRoot");
        TMP_Text bonus = FindChild(root, "BonusValue")?.GetComponent<TMP_Text>();
        if (bonus == null)
            throw new System.InvalidOperationException("Falta el detalle funcional del Hangar.");
        Canvas.ForceUpdateCanvases();
        if (!bonus.text.Contains(requiredText) ||
            (!string.IsNullOrEmpty(secondRequiredText) && !bonus.text.Contains(secondRequiredText)) ||
            (!string.IsNullOrEmpty(forbiddenText) && bonus.text.Contains(forbiddenText)))
            throw new System.InvalidOperationException("La variante seleccionada no muestra el texto funcional esperado.");
        RenderToPng(1080, 1920, output1080);
        RenderToPng(720, 1280, output720);
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

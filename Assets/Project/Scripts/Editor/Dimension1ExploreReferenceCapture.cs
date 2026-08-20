#if UNITY_EDITOR
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Dimension1ExploreReferenceCapture
{
    private const string ActiveKey = "QF.D1ExploreCapture.Active";
    private const string FrameKey = "QF.D1ExploreCapture.Frame";
    private const string FailureKey = "QF.D1ExploreCapture.Failed";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory => Path.GetFullPath("Logs/VisualQA/Dimension1/ExploreReference");
    private static string OutputPath => Path.Combine(OutputDirectory, "Explore_reference_1080x1920.png");
    private static string OutputPath720 => Path.Combine(OutputDirectory, "Explore_reference_720x1280.png");

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
            Debug.Log((failed ? "[D1 Explore Capture] FAIL | " : "[D1 Explore Capture] PASS | ") + OutputPath);
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
            if (frame >= 24 && frame <= 58) ForceVisible();
            if (frame != 58) return;
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
        SetSceneObjectActive("Dimension1Panel", true);
        SetSceneObjectActive("Dimension1MainContent", true);
        string[] close =
        {
            "GalaxyPanel", "HangarPanel", "RelicChamberPanel", "Dimension1TreePanel", "ArkPanel",
            "ExplorationRewardsPanel", "Exploration Record Panel", "PresentationReturnReport"
        };
        foreach (string name in close) SetSceneObjectActive(name, false);
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
        Transform root = FindSceneTransform("D1_ExploreVisualRoot");
        if (root == null) throw new System.InvalidOperationException("Falta D1_ExploreVisualRoot.");
        // TabsUI puede volver a su pestaña inicial durante los primeros frames.
        // Se fuerza toda la cadena de padres para que la captura no dependa de ese orden.
        Transform current = root;
        while (current != null)
        {
            current.gameObject.SetActive(true);
            current = current.parent;
        }
        string[] close =
        {
            "GalaxyPanel", "HangarPanel", "RelicChamberPanel", "Dimension1TreePanel", "ArkPanel",
            "ExplorationRewardsPanel", "Exploration Record Panel"
        };
        foreach (string name in close) SetSceneObjectActive(name, false);
        CanvasGroup group = root.GetComponent<CanvasGroup>();
        if (group != null)
        {
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }
        Dimension1ExploreVisualUI visual = root.GetComponent<Dimension1ExploreVisualUI>();
        if (visual == null) throw new System.InvalidOperationException("Falta Dimension1ExploreVisualUI.");
        visual.SetReferencePreviewForVisualQa(true);
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateVisibleScreen()
    {
        Transform root = FindSceneTransform("D1_ExploreVisualRoot");
        if (root == null || !root.gameObject.activeInHierarchy)
            throw new System.InvalidOperationException("La pantalla Explorar no quedó visible.");
        string[] required =
        {
            "DimensionTitle", "ExploreTitle", "ScannerPanel", "DestinationPanel", "ShipPanel",
            "SupportPanel", "ActiveExpedition", "StartExpedition", "ExplorationRecord", "BottomNavigation"
        };
        foreach (string name in required)
            if (FindChild(root, name) == null) throw new System.InvalidOperationException("Falta bloque: " + name);
        TMP_Text title = FindChild(root, "ExploreTitle")?.GetComponent<TMP_Text>();
        if (title == null || title.text != "EXPLORAR")
            throw new System.InvalidOperationException("Título Explorar inválido.");
        if (FindSceneTransform("PrimaryNavigationSlot")?.gameObject.activeSelf == true)
            throw new System.InvalidOperationException("La navegación global se mezcló con Explorar.");
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

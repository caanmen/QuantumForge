#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Dimension1GalaxyVisualCapture
{
    private const string ActiveKey = "QF.D1GalaxyCapture.Active";
    private const string FrameKey = "QF.D1GalaxyCapture.Frame";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputPath => Path.GetFullPath(
        "Logs/VisualQA/Dimension1/Galaxy_v3_1080x1920.png"
    );

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
        Directory.CreateDirectory(Path.GetDirectoryName(OutputPath));
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetInt(FrameKey, 0);
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        EditorApplication.isPlaying = true;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            SessionState.SetBool(ActiveKey, false);
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            Debug.Log("[D1 Galaxy Capture] PASS | " + OutputPath);
            EditorApplication.Exit(0);
        }
    }

    private static void PreparePreviewState()
    {
        GameState gameState = GameState.I != null
            ? GameState.I
            : Object.FindFirstObjectByType<GameState>();
        if (gameState == null) return;

        gameState.dimension01Unlocked = true;
        gameState.EnsureDimension1State();
        gameState.dimension1SelectedSectorId = Dimension1System.Sector03AncientOrbits;

        if (gameState.dimension1Sectors != null)
        {
            for (int i = 0; i < gameState.dimension1Sectors.Count; i++)
            {
                D1SectorState sector = gameState.dimension1Sectors[i];
                if (sector == null) continue;
                sector.unlocked = i < 3;
                sector.completedExplorations = i == 0 ? 23 : i == 1 ? 12 : i == 2 ? 1 : 0;
            }
        }

        if (gameState.dimension1Planets != null)
        {
            for (int i = 0; i < gameState.dimension1Planets.Count; i++)
            {
                D1PlanetState planet = gameState.dimension1Planets[i];
                if (planet == null) continue;
                planet.unlocked = i < 3;
                planet.extractorTier = i < 3 ? 3 - i : 0;
            }
        }

        gameState.AddD1Metal(Dimension1System.MetalIron, 18420d);
        gameState.AddD1Metal(Dimension1System.MetalCopper, 7320d);
        gameState.AddD1Metal(Dimension1System.MetalAluminum, 2840d);

        Dimension1PanelUI panel = Object.FindFirstObjectByType<Dimension1PanelUI>();
        TabsUI tabs = TabsUI.Instance != null
            ? TabsUI.Instance
            : Object.FindFirstObjectByType<TabsUI>();
        if (tabs != null) tabs.ShowDimension1();
        if (panel != null)
        {
            panel.OnClickOpenGalaxyPanel();
            panel.OnClickPreviewGalaxySector3();
        }

        PresentationReturnReportUI report = Object.FindFirstObjectByType<PresentationReturnReportUI>();
        if (report != null) report.gameObject.SetActive(false);
    }

    private static void ForceGalaxyVisible()
    {
        Dimension1PanelUI panel = Object.FindFirstObjectByType<Dimension1PanelUI>();
        bool galaxyAlreadyVisible = false;
        foreach (Transform transform in Resources.FindObjectsOfTypeAll<Transform>())
        {
            if (!transform.gameObject.scene.IsValid()) continue;
            if (transform.name == "GalaxyPanel" && transform.gameObject.activeInHierarchy)
            {
                galaxyAlreadyVisible = true;
                break;
            }
        }
        if (!galaxyAlreadyVisible && panel != null) panel.OnClickOpenGalaxyPanel();

        foreach (Transform transform in Resources.FindObjectsOfTypeAll<Transform>())
        {
            if (!transform.gameObject.scene.IsValid()) continue;
            if (transform.name == "GalaxyPanel") transform.gameObject.SetActive(true);
            else if (transform.name == "Dimension1MainContent") transform.gameObject.SetActive(false);
        }
    }

    private static void Tick()
    {
        int frame = SessionState.GetInt(FrameKey, 0) + 1;
        SessionState.SetInt(FrameKey, frame);
        if (frame == 25) PreparePreviewState();
        if (frame >= 25 && frame <= 70)
        {
            ForceGalaxyVisible();
            PresentationReturnReportUI report = Object.FindFirstObjectByType<PresentationReturnReportUI>();
            if (report != null) report.gameObject.SetActive(false);
        }
        if (frame < 80) return;

        EditorApplication.update -= Tick;
        RenderToPng(1080, 1920, OutputPath);
        EditorApplication.isPlaying = false;
    }

    private static void RenderToPng(int width, int height, string path)
    {
        Camera camera = Camera.main != null
            ? Camera.main
            : Object.FindFirstObjectByType<Camera>();
        if (camera == null)
            throw new System.InvalidOperationException("No hay cámara para captura.");

        Canvas[] canvases = Object.FindObjectsByType<Canvas>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );
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
}
#endif

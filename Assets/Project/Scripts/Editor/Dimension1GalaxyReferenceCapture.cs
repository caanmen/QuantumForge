#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Dimension1GalaxyReferenceCapture
{
    private const string ActiveKey = "QF.D1GalaxyReferenceCapture.Active";
    private const string FrameKey = "QF.D1GalaxyReferenceCapture.Frame";
    private const string FailureKey = "QF.D1GalaxyReferenceCapture.Failed";
    private const string SelectedOnlyKey = "QF.D1GalaxyReferenceCapture.SelectedOnly";
    private const string RegressionKey = "QF.D1GalaxyReferenceCapture.Regression";
    private const string NeutralOnlyKey = "QF.D1GalaxyReferenceCapture.NeutralOnly";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory => Path.GetFullPath("Logs/VisualQA/Dimension1/V12OrbitalSystem");
    private static readonly Dictionary<string, Vector2> StableBodyPositions = new Dictionary<string, Vector2>();
    private static readonly Dictionary<string, Quaternion> InitialBodyRotations = new Dictionary<string, Quaternion>();

    [InitializeOnLoadMethod]
    private static void Resume()
    {
        if (!SessionState.GetBool(ActiveKey, false)) return;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        if (!EditorApplication.isPlaying) return;
        SaveService.SuppressWritesForVisualQa = true;
        EditorApplication.update -= Tick;
        EditorApplication.update += Tick;
    }

    public static void Run()
    {
        Start(false, false, false);
    }

    public static void RunSelectedOnly()
    {
        Start(true, false, false);
    }

    public static void RunInteractionRegression()
    {
        Start(false, true, false);
    }

    public static void RunNeutralOnly()
    {
        Start(false, false, true);
    }

    private static void Start(bool selectedOnly, bool regression, bool neutralOnly)
    {
        Directory.CreateDirectory(OutputDirectory);
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetBool(FailureKey, false);
        SessionState.SetBool(SelectedOnlyKey, selectedOnly);
        SessionState.SetBool(RegressionKey, regression);
        SessionState.SetBool(NeutralOnlyKey, neutralOnly);
        SessionState.SetInt(FrameKey, 0);
        SaveService.SuppressWritesForVisualQa = true;
        Screen.SetResolution(1080, 1920, false);
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
            // La guarda QA permanece enclavada hasta cerrar el proceso para impedir
            // que OnApplicationQuit persista el escenario de captura.
            SessionState.SetBool(ActiveKey, false);
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            bool failed = SessionState.GetBool(FailureKey, false);
            Debug.Log(failed
                ? "[D1 Carta Galactica V12] CAPTURE_FAIL"
                : "[D1 Carta Galactica V12] CAPTURE_PASS | neutral + selected | 1080x1920 + 720x1280");
            EditorApplication.Exit(failed ? 1 : 0);
        }
    }

    private static void Tick()
    {
        try
        {
            int frame = SessionState.GetInt(FrameKey, 0) + 1;
            SessionState.SetInt(FrameKey, frame);
            bool selectedOnly = SessionState.GetBool(SelectedOnlyKey, false);
            bool regression = SessionState.GetBool(RegressionKey, false);
            bool neutralOnly = SessionState.GetBool(NeutralOnlyKey, false);
            if (regression)
            {
                TickInteractionRegression(frame);
                return;
            }
            if (frame == 20)
            {
                PrepareState();
                if (selectedOnly)
                {
                    Dimension1PanelUI selectedPanel = UnityEngine.Object.FindFirstObjectByType<Dimension1PanelUI>();
                    if (selectedPanel == null) throw new InvalidOperationException("Dimension1PanelUI no existe.");
                    selectedPanel.OnClickPreviewGalaxySector3();
                }
            }
            if (frame >= 20 && frame <= 150) ForceGalaxyVisible();
            if (neutralOnly && frame == 85)
            {
                ValidateNeutral();
                RenderToPng(1080, 1920, Path.Combine(OutputDirectory, "Galaxy_v12_neutral_1080x1920.png"));
                RenderToPng(720, 1280, Path.Combine(OutputDirectory, "Galaxy_v12_neutral_720x1280.png"));
                EditorApplication.update -= Tick;
                EditorApplication.isPlaying = false;
                return;
            }
            if (!neutralOnly && !selectedOnly && frame == 55)
            {
                ValidateNeutral();
                RenderToPng(1080, 1920, Path.Combine(OutputDirectory, "Galaxy_v12_neutral_1080x1920.png"));
                RenderToPng(720, 1280, Path.Combine(OutputDirectory, "Galaxy_v12_neutral_720x1280.png"));
                Dimension1PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension1PanelUI>();
                if (panel == null) throw new InvalidOperationException("Dimension1PanelUI no existe.");
                panel.OnClickPreviewGalaxySector3();
            }
            if (!neutralOnly && ((selectedOnly && frame == 85) || (!selectedOnly && frame == 125)))
            {
                ValidateSelected();
                RenderToPng(1080, 1920, Path.Combine(OutputDirectory, "Galaxy_v12_selected_1080x1920.png"));
                RenderToPng(720, 1280, Path.Combine(OutputDirectory, "Galaxy_v12_selected_720x1280.png"));
                EditorApplication.update -= Tick;
                EditorApplication.isPlaying = false;
            }
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            SessionState.SetBool(FailureKey, true);
            EditorApplication.update -= Tick;
            EditorApplication.isPlaying = false;
        }
    }

    private static void TickInteractionRegression(int frame)
    {
        if (frame == 20) PrepareState();
        if (frame >= 20 && frame <= 130) ForceGalaxyVisible();
        Dimension1PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension1PanelUI>();
        if (frame == 30)
        {
            if (panel == null) throw new InvalidOperationException("Dimension1PanelUI no existe.");
            panel.OnClickPreviewGalaxySector1();
            ValidateStableSectorTitlesAndMetalCards();
            CaptureBodyBaselines();
        }
        else if (frame == 55)
        {
            ValidateInteractionLayout(Dimension1System.Sector01OuterRim);
            panel.OnClickPreviewGalaxySector3();
            ValidateStableSectorTitlesAndMetalCards();
        }
        else if (frame == 80)
        {
            ValidateInteractionLayout(Dimension1System.Sector03AncientOrbits);
            panel.OnClickPreviewGalaxySector4();
            ValidateStableSectorTitlesAndMetalCards();
        }
        else if (frame == 110)
        {
            ValidateInteractionLayout(Dimension1System.Sector04SilentFrontier);
            Debug.Log("[D1 Carta Galactica V12] INTERACTION_PASS | 7 cuerpos | 7 órbitas | rotación axial | posiciones estables | sin contadores de expediciones");
            EditorApplication.update -= Tick;
            EditorApplication.isPlaying = false;
        }
    }

    private static void ValidateStableSectorTitlesAndMetalCards()
    {
        (string nodeName, string expectedTitle)[] expectedTitles =
        {
            ("Sector01", "ELYSIA"),
            ("Planet02Node", "VULKAR"),
            ("Sector02", "CORONA DE TÁNTALO"),
            ("Sector03", "MNEMOS"),
            ("Planet05Node", "ORPHEON"),
            ("Sector04", "NYXARA"),
            ("Planet07Node", "EREBON"),
            ("GalacticCenter", "CENTRO GALÁCTICO")
        };

        foreach ((string nodeName, string expectedTitle) in expectedTitles)
        {
            Transform node = FindSceneTransform(nodeName);
            Transform titleTransform = node != null ? FindChild(node, "Title") : null;
            TMP_Text title = titleTransform != null ? titleTransform.GetComponent<TMP_Text>() : null;
            if (title == null || title.text != expectedTitle)
                throw new InvalidOperationException(
                    nodeName + " cambió su título al pulsar: " + (title != null ? title.text : "<sin título>"));
        }

        Transform root = FindSceneTransform("D1_GalaxyVisualRoot");
        if (root == null) throw new InvalidOperationException("No existe D1_GalaxyVisualRoot.");
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            if (child.name == "Marker" && child.parent != null && child.parent.name.StartsWith("Metal_"))
                throw new InvalidOperationException("Persiste una línea Marker en las tarjetas de materiales.");
    }

    private static void CaptureBodyBaselines()
    {
        StableBodyPositions.Clear();
        InitialBodyRotations.Clear();
        foreach ((string nodeName, string bodyName) in TestedBodies())
        {
            Transform node = FindSceneTransform(nodeName);
            RectTransform body = node != null ? FindChild(node, bodyName) as RectTransform : null;
            if (body == null) throw new InvalidOperationException("Falta cuerpo de prueba: " + bodyName);
            string key = nodeName + "/" + bodyName;
            StableBodyPositions[key] = body.anchoredPosition;
            InitialBodyRotations[key] = body.localRotation;
        }
    }

    private static void ValidateInteractionLayout(string expectedPreview)
    {
        Transform root = FindSceneTransform("D1_GalaxyVisualRoot");
        if (root == null) throw new InvalidOperationException("No existe D1_GalaxyVisualRoot.");
        Dimension1PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension1PanelUI>();
        if (panel == null || panel.GalaxyPreviewSectorId != expectedPreview)
            throw new InvalidOperationException("No quedó seleccionado el sector esperado: " + expectedPreview);

        foreach ((string nodeName, string bodyName) in TestedBodies())
        {
            RectTransform node = FindSceneTransform(nodeName) as RectTransform;
            RectTransform body = node != null ? FindChild(node, bodyName) as RectTransform : null;
            if (body == null) throw new InvalidOperationException("Desapareció cuerpo: " + bodyName);
            if (Vector2.Distance(body.pivot, new Vector2(0.5f, 0.5f)) > 0.001f)
                throw new InvalidOperationException(bodyName + " no gira desde el centro.");
            string key = nodeName + "/" + bodyName;
            if (!StableBodyPositions.TryGetValue(key, out Vector2 baseline) ||
                Vector2.Distance(body.anchoredPosition, baseline) > 0.01f)
                throw new InvalidOperationException(bodyName + " salió volando al seleccionar un sector.");
            Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(node, body);
            if (!node.rect.Contains(bounds.center))
                throw new InvalidOperationException(bodyName + " quedó fuera de su área táctil.");
            if (nodeName != "GalacticCenter" &&
                (!InitialBodyRotations.TryGetValue(key, out Quaternion initialRotation) ||
                 Quaternion.Angle(initialRotation, body.localRotation) < 0.1f))
                throw new InvalidOperationException(bodyName + " no mostró rotación axial entre fotogramas.");
        }

        int orbitCount = 0;
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            if (child.name.StartsWith("OrbitPath0")) orbitCount++;
            if (child.name != "State") continue;
            TMP_Text stateText = child.GetComponent<TMP_Text>();
            if (stateText != null && stateText.text.Contains("EXPEDICI"))
                throw new InvalidOperationException("Persiste un contador de expediciones en " + child.parent.name + ".");
        }
        if (orbitCount != 7)
            throw new InvalidOperationException("Se esperaban 7 trayectorias orbitales y hay " + orbitCount + ".");
    }

    private static (string nodeName, string bodyName)[] TestedBodies()
    {
        return new[]
        {
            ("Sector01", "PlanetBlue"),
            ("Planet02Node", "PlanetMolten"),
            ("Sector02", "DebrisRing"),
            ("Sector03", "PlanetAncient"),
            ("Planet05Node", "PlanetBlue"),
            ("Sector04", "SilentPlanetA"),
            ("Planet07Node", "SilentPlanetB"),
            ("GalacticCenter", "BlackHole")
        };
    }

    private static void PrepareState()
    {
        SaveService.SuppressWritesForVisualQa = true;
        GameState state = GameState.I != null
            ? GameState.I
            : UnityEngine.Object.FindFirstObjectByType<GameState>();
        if (state == null) throw new InvalidOperationException("GameState no existe.");
        state.dimension01Unlocked = true;
        state.EnsureDimension1State();
        state.dimension1SelectedSectorId = Dimension1System.Sector03AncientOrbits;
        if (state.dimension1Sectors != null)
        {
            for (int i = 0; i < state.dimension1Sectors.Count; i++)
            {
                D1SectorState sector = state.dimension1Sectors[i];
                if (sector == null) continue;
                sector.unlocked = i < 3;
                sector.completedExplorations = i == 0 ? 23 : i == 1 ? 12 : i == 2 ? 1 : 0;
            }
        }
        TabsUI tabs = TabsUI.Instance != null ? TabsUI.Instance : UnityEngine.Object.FindFirstObjectByType<TabsUI>();
        if (tabs != null) tabs.ShowDimension1();
        Dimension1PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension1PanelUI>();
        if (panel == null) throw new InvalidOperationException("Dimension1PanelUI no existe.");
        panel.OnClickOpenGalaxyPanel();
        PrepareCanvasesForCapture();
        PresentationReturnReportUI report = UnityEngine.Object.FindFirstObjectByType<PresentationReturnReportUI>();
        if (report != null) report.gameObject.SetActive(false);
    }

    private static void ForceGalaxyVisible()
    {
        Transform galaxyPanel = FindSceneTransform("GalaxyPanel");
        if (galaxyPanel != null) galaxyPanel.gameObject.SetActive(true);
        Transform main = FindSceneTransform("Dimension1MainContent");
        if (main != null) main.gameObject.SetActive(false);
        PresentationReturnReportUI report = UnityEngine.Object.FindFirstObjectByType<PresentationReturnReportUI>();
        if (report != null) report.gameObject.SetActive(false);
    }

    private static void ValidateNeutral()
    {
        Transform root = FindSceneTransform("D1_GalaxyVisualRoot");
        if (root == null || !root.gameObject.activeInHierarchy)
            throw new InvalidOperationException("La Carta Galáctica no está visible.");
        string[] expected =
        {
            "Sector01", "Planet02Node", "Sector02", "Sector03",
            "Planet05Node", "Sector04", "Planet07Node", "GalacticCenter"
        };
        foreach (string name in expected)
        {
            Transform node = FindChild(root, name);
            if (node == null || !node.gameObject.activeInHierarchy)
                throw new InvalidOperationException("Falta nodo visual: " + name);
        }
        Transform neutral = FindChild(root, "NeutralInstruction");
        if (neutral == null || !neutral.gameObject.activeInHierarchy)
            throw new InvalidOperationException("La apertura neutral no muestra su instrucción.");
        Transform bottom = FindChild(root, "D1BottomNavigation");
        if (bottom == null || !bottom.gameObject.activeInHierarchy)
            throw new InvalidOperationException("Falta la navegación propia de Dimensión 1.");
        EnsureGlobalNavigationHidden("PrimaryNavigationSlot");
        EnsureGlobalNavigationHidden("SecondaryNavigationSlot");
    }

    private static void ValidateSelected()
    {
        Transform root = FindSceneTransform("D1_GalaxyVisualRoot");
        Transform details = root != null ? FindChild(root, "SelectedData") : null;
        if (details == null || !details.gameObject.activeInHierarchy)
            throw new InvalidOperationException("La selección de sector no muestra sus datos.");
        Dimension1PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension1PanelUI>();
        if (panel == null || panel.GalaxyPreviewSectorId != Dimension1System.Sector03AncientOrbits)
            throw new InvalidOperationException("Órbitas Antiguas no quedó seleccionada.");
    }

    private static void EnsureGlobalNavigationHidden(string name)
    {
        Transform navigation = FindSceneTransform(name);
        if (navigation != null && navigation.gameObject.activeSelf)
            throw new InvalidOperationException(name + " se mezcló con la navegación de Carta Galáctica.");
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

    private static void PrepareCanvasesForCapture()
    {
        Camera camera = Camera.main != null ? Camera.main : UnityEngine.Object.FindFirstObjectByType<Camera>();
        if (camera == null) throw new InvalidOperationException("No hay cámara de captura.");
        foreach (Canvas canvas in UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
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
        Camera camera = Camera.main != null ? Camera.main : UnityEngine.Object.FindFirstObjectByType<Camera>();
        if (camera == null) throw new InvalidOperationException("No hay cámara de captura.");
        Canvas[] canvases = UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        RenderMode[] modes = new RenderMode[canvases.Length];
        Camera[] cameras = new Camera[canvases.Length];
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
            RenderTexture.active = target;
            GL.Clear(true, true, Color.black);
            camera.Render();
            Canvas.ForceUpdateCanvases();
            camera.Render();
            RenderTexture.active = target;
            Texture2D image = new Texture2D(width, height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
            image.Apply();
            File.WriteAllBytes(path, image.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(image);
        }
        finally
        {
            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            target.Release();
            UnityEngine.Object.DestroyImmediate(target);
            for (int i = 0; i < canvases.Length; i++)
            {
                canvases[i].renderMode = modes[i];
                canvases[i].worldCamera = cameras[i];
            }
        }
    }
}
#endif

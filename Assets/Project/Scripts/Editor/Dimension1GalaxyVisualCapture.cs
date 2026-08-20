#if UNITY_EDITOR
using System.IO;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Dimension1GalaxyVisualCapture
{
    private const string ActiveKey = "QF.D1GalaxyCapture.Active";
    private const string FrameKey = "QF.D1GalaxyCapture.Frame";
    private const string FailureKey = "QF.D1GalaxyCapture.Failed";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const int SmoothSequenceFrameCount = 61;
    private const float SmoothSequenceFrameStep = 1f / 20f;
    private static readonly Dictionary<int, Vector3> StableLabelScales = new Dictionary<int, Vector3>();
    private static readonly Dictionary<int, string> StableSectorLabelText = new Dictionary<int, string>();
    private static readonly Dictionary<int, Vector2> StableOrbitPositions = new Dictionary<int, Vector2>();
    private static readonly Dictionary<int, Quaternion> StableOrbitRotations = new Dictionary<int, Quaternion>();
    private static readonly Dictionary<int, Vector2> StablePlanetPositions = new Dictionary<int, Vector2>();
    private static readonly Dictionary<int, Quaternion> StablePlanetRotations = new Dictionary<int, Quaternion>();
    private static readonly Dictionary<int, Vector2[]> StableHierarchyScreenCorners = new Dictionary<int, Vector2[]>();
    private static Vector2 stableBackgroundPosition;
    private static bool observedSelectedPlanetRotation;
    private static bool observedGalaxyAnimationTime;
    private static bool capturedGalaxyOnlyPair;
    private static bool selectionStarted;
    private static bool awaiting720Capture;
    private static int resolutionSwitchFrame;
    private static int sequenceFrameIndex;
    private static float stabilityBaselineTime;
    private static float stableGalaxyAnimationTime;
    private static float selectionStartTime;
    private static bool stabilityBaselineReady;
    private static string OutputDirectory => Path.GetFullPath("Logs/VisualQA/Dimension1/V9Anchored");
    private static string OutputPath => Path.Combine(OutputDirectory, "Galaxy_v9_selected_1080x1920.png");

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
        foreach (string oldCapture in Directory.GetFiles(OutputDirectory, "Galaxy_v9_*.png"))
            File.Delete(oldCapture);
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetBool(FailureKey, false);
        SessionState.SetInt(FrameKey, 0);
        StableLabelScales.Clear();
        StableSectorLabelText.Clear();
        StableOrbitPositions.Clear();
        StableOrbitRotations.Clear();
        StablePlanetPositions.Clear();
        StablePlanetRotations.Clear();
        StableHierarchyScreenCorners.Clear();
        observedSelectedPlanetRotation = false;
        observedGalaxyAnimationTime = false;
        capturedGalaxyOnlyPair = false;
        selectionStarted = false;
        awaiting720Capture = false;
        sequenceFrameIndex = 0;
        stabilityBaselineReady = false;
        SaveService.SuppressWritesForVisualQa = true;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        EditorApplication.isPlaying = true;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            // Domain reload clears static properties, so restore the QA write guard here.
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
            Debug.Log((failed ? "[D1 Galaxy Capture] FAIL | " : "[D1 Galaxy Capture] PASS | ") + OutputPath);
            EditorApplication.Exit(failed ? 1 : 0);
        }
    }

    private static void PreparePreviewState()
    {
        SaveService.SuppressWritesForVisualQa = true;
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
            panel.OnClickOpenGalaxyPanel();

        PrepareCanvasesForCapture();

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
        try
        {
            TickCore();
        }
        catch (System.Exception exception)
        {
            Debug.LogException(exception);
            SessionState.SetBool(FailureKey, true);
            EditorApplication.update -= Tick;
            EditorApplication.isPlaying = false;
        }
    }

    private static void TickCore()
    {
        int frame = SessionState.GetInt(FrameKey, 0) + 1;
        SessionState.SetInt(FrameKey, frame);
        if (awaiting720Capture)
        {
            if (frame < resolutionSwitchFrame + 5) return;
            ForceGalaxyVisible();
            RenderToPng(720, 1280, Path.Combine(OutputDirectory, "Galaxy_v9_selected_720x1280.png"));
            EditorApplication.update -= Tick;
            EditorApplication.isPlaying = false;
            return;
        }
        if (frame == 25) PreparePreviewState();
        if (frame >= 25 && frame <= 130)
        {
            ForceGalaxyVisible();
            PresentationReturnReportUI report = Object.FindFirstObjectByType<PresentationReturnReportUI>();
            if (report != null) report.gameObject.SetActive(false);
        }
        if (frame == 85)
        {
            CaptureStabilityBaseline();
            ValidateNeutralOpening();
            Dimension1GalaxyVisualUI visual = Object.FindFirstObjectByType<Dimension1GalaxyVisualUI>();
            if (visual == null)
                throw new System.InvalidOperationException("Dimension1GalaxyVisualUI no existe para fijar el tiempo de QA.");
            visual.SetGalaxyAnimationTimeForVisualQa(0f);
            RenderToPng(1080, 1920, Path.Combine(OutputDirectory, "Galaxy_v9_neutral_1080x1920.png"));
            RenderToPng(1080, 1920, Path.Combine(OutputDirectory, "Galaxy_v9_anchored_t0_1080x1920.png"));
        }
        if (frame > 85) ValidateStability();
        if (!stabilityBaselineReady) return;

        float elapsed = Time.unscaledTime - stabilityBaselineTime;
        if (!capturedGalaxyOnlyPair && elapsed >= 0.6f)
        {
            RenderToPng(1080, 1920, Path.Combine(OutputDirectory, "Galaxy_v9_anchored_t06_1080x1920.png"));
            capturedGalaxyOnlyPair = true;
            Dimension1PanelUI panel = Object.FindFirstObjectByType<Dimension1PanelUI>();
            if (panel != null) panel.OnClickPreviewGalaxySector3();
            selectionStarted = true;
            selectionStartTime = Time.unscaledTime;
        }
        float selectionElapsed = selectionStarted ? Time.unscaledTime - selectionStartTime : 0f;
        if (selectionStarted && sequenceFrameIndex < SmoothSequenceFrameCount)
        {
            Dimension1GalaxyVisualUI visual = Object.FindFirstObjectByType<Dimension1GalaxyVisualUI>();
            if (visual == null)
                throw new System.InvalidOperationException("Dimension1GalaxyVisualUI no existe durante la secuencia fluida.");
            float deterministicTime = sequenceFrameIndex * SmoothSequenceFrameStep;
            visual.SetGalaxyAnimationTimeForVisualQa(deterministicTime);
            string sequencePath = Path.Combine(
                OutputDirectory,
                $"Galaxy_v9_smooth_{sequenceFrameIndex:000}_t{deterministicTime:0.00}s_1080x1920.png");
            RenderToPng(1080, 1920, sequencePath);
            sequenceFrameIndex++;
            return;
        }
        if (!selectionStarted || sequenceFrameIndex < SmoothSequenceFrameCount) return;

        if (!observedSelectedPlanetRotation)
            throw new System.InvalidOperationException("El planeta seleccionado no registró rotación.");

        if (!observedGalaxyAnimationTime)
            throw new System.InvalidOperationException("El material de galaxia no avanzo su tiempo de animacion.");

        Dimension1GalaxyVisualUI finalVisual = Object.FindFirstObjectByType<Dimension1GalaxyVisualUI>();
        if (finalVisual == null)
            throw new System.InvalidOperationException("Dimension1GalaxyVisualUI no existe para la prueba prolongada.");
        finalVisual.SetGalaxyAnimationTimeForVisualQa(0f);
        RenderToPng(1080, 1920, Path.Combine(OutputDirectory, "Galaxy_v9_stability_t0_1080x1920.png"));
        finalVisual.SetGalaxyAnimationTimeForVisualQa(120f);
        RenderToPng(1080, 1920, Path.Combine(OutputDirectory, "Galaxy_v9_stability_t120_1080x1920.png"));
        finalVisual.SetGalaxyAnimationTimeForVisualQa(3f);
        RenderToPng(1080, 1920, OutputPath);
        Screen.SetResolution(720, 1280, false);
        awaiting720Capture = true;
        resolutionSwitchFrame = frame;
    }

    private static void CaptureStabilityBaseline()
    {
        Transform root = FindGalaxyVisualRoot();
        if (root == null) throw new System.InvalidOperationException("D1_GalaxyVisualRoot no existe para QA temporal.");

        Transform background = FindNamedChild(root, "StaticStarfield");
        if (background == null) throw new System.InvalidOperationException("StaticStarfield no existe para QA temporal.");
        stableBackgroundPosition = ((RectTransform)background).anchoredPosition;
        Canvas.ForceUpdateCanvases();
        RectMask2D mapMask = background.parent != null ? background.parent.GetComponent<RectMask2D>() : null;
        if (mapMask == null || background.parent.name != "FullBleedGalaxyMap")
            throw new System.InvalidOperationException("La galaxia no esta recortada dentro del area del mapa.");
        for (Transform current = background; current != null; current = current.parent)
        {
            RectTransform rect = current as RectTransform;
            if (rect != null)
                StableHierarchyScreenCorners[rect.GetInstanceID()] = GetScreenCorners(rect);
        }
        Transform galaxy = FindNamedChild(root, "GalaxyAnimated");
        if (galaxy == null) throw new System.InvalidOperationException("GalaxyAnimated no existe para QA temporal.");
        StableHierarchyScreenCorners[galaxy.GetInstanceID()] = GetScreenCorners((RectTransform)galaxy);
        Dimension1GalaxyVisualUI visual = root.GetComponent<Dimension1GalaxyVisualUI>();
        if (visual == null) throw new System.InvalidOperationException("Dimension1GalaxyVisualUI no existe para QA temporal.");
        stableGalaxyAnimationTime = visual.GalaxyAnimationTime;
        stabilityBaselineTime = Time.unscaledTime;
        if (FindNamedChild(root, "NebulaParallax") != null)
            throw new System.InvalidOperationException("NebulaParallax duplicada sigue presente.");

        foreach (TMP_Text label in root.GetComponentsInChildren<TMP_Text>(true))
        {
            if (label.gameObject.activeInHierarchy && label.color.a > 0.01f)
                StableLabelScales[label.GetInstanceID()] = label.transform.lossyScale;
            if (label.transform.parent != null && label.transform.parent.name == "LabelPlate")
                StableSectorLabelText[label.GetInstanceID()] = label.text;
        }

        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            RectTransform rect = child as RectTransform;
            if (rect == null) continue;
            if (child.name == "SelectionGlow" || child.name == "TechnicalOrbit")
                throw new System.InvalidOperationException("Persisten líneas o halos alrededor de un planeta: " + child.name);
            if (child.name == "TechnicalOrbit")
            {
                if (Vector2.Distance(rect.pivot, new Vector2(0.5f, 0.5f)) > 0.001f)
                    throw new System.InvalidOperationException("TechnicalOrbit conserva un pivote incorrecto.");
                StableOrbitPositions[rect.GetInstanceID()] = rect.anchoredPosition;
                StableOrbitRotations[rect.GetInstanceID()] = rect.localRotation;
            }
            else if (child.name == "PlanetBody")
            {
                if (Vector2.Distance(rect.pivot, new Vector2(0.5f, 0.5f)) > 0.001f)
                    throw new System.InvalidOperationException("PlanetBody no gira desde su centro.");
                StablePlanetPositions[rect.GetInstanceID()] = rect.anchoredPosition;
                StablePlanetRotations[rect.GetInstanceID()] = rect.localRotation;
            }
        }
        stabilityBaselineReady = true;
    }

    private static void ValidateStability()
    {
        if (!stabilityBaselineReady) return;
        Transform root = FindGalaxyVisualRoot();
        if (root == null) throw new System.InvalidOperationException("D1_GalaxyVisualRoot desapareció durante QA temporal.");
        Transform background = FindNamedChild(root, "StaticStarfield");
        Canvas.ForceUpdateCanvases();
        if (background == null || Vector2.Distance(((RectTransform)background).anchoredPosition, stableBackgroundPosition) > 0.001f)
            throw new System.InvalidOperationException("El fondo volvió a desplazarse durante la animación.");

        Transform galaxy = FindNamedChild(root, "GalaxyAnimated");
        if (galaxy == null) throw new System.InvalidOperationException("GalaxyAnimated desaparecio durante QA temporal.");
        if (StableHierarchyScreenCorners.TryGetValue(galaxy.GetInstanceID(), out Vector2[] galaxyBaselineCorners))
        {
            Vector2[] galaxyCurrentCorners = GetScreenCorners((RectTransform)galaxy);
            for (int i = 0; i < 4; i++)
                if (Vector2.Distance(galaxyCurrentCorners[i], galaxyBaselineCorners[i]) > 0.05f)
                    throw new System.InvalidOperationException("El RectTransform de la galaxia se movio o respiro.");
        }
        Dimension1GalaxyVisualUI visual = root.GetComponent<Dimension1GalaxyVisualUI>();
        if (visual != null && visual.GalaxyAnimationTime - stableGalaxyAnimationTime >= 0.5f)
            observedGalaxyAnimationTime = true;

        for (Transform current = background; current != null; current = current.parent)
        {
            RectTransform rect = current as RectTransform;
            if (rect == null || !StableHierarchyScreenCorners.TryGetValue(rect.GetInstanceID(), out Vector2[] baselineCorners))
                continue;
            Vector2[] currentCorners = GetScreenCorners(rect);
            for (int i = 0; i < 4; i++)
                if (Vector2.Distance(currentCorners[i], baselineCorners[i]) > 0.05f)
                    throw new System.InvalidOperationException("Saltó la jerarquía de Carta Galáctica: " + rect.name);
        }

        Transform secondaryNavigation = FindSceneTransform("SecondaryNavigationSlot");
        if (secondaryNavigation != null && secondaryNavigation.gameObject.activeSelf)
            throw new System.InvalidOperationException("SecondaryNavigationSlot se reactivó durante Carta Galáctica.");

        foreach (TMP_Text label in root.GetComponentsInChildren<TMP_Text>(true))
        {
            if (!StableLabelScales.TryGetValue(label.GetInstanceID(), out Vector3 baseline)) continue;
            if (Vector3.Distance(label.transform.lossyScale, baseline) > 0.001f)
                throw new System.InvalidOperationException("Una etiqueta TMP cambió de escala durante la animación: " + label.name);
            if (StableSectorLabelText.TryGetValue(label.GetInstanceID(), out string baselineText) && label.text != baselineText)
                throw new System.InvalidOperationException("Un rótulo de sector cambió de contenido durante el refresco: " + label.name);
        }

        foreach (Transform animatedChild in root.GetComponentsInChildren<Transform>(true))
        {
            RectTransform animatedRect = animatedChild as RectTransform;
            if (animatedRect == null) continue;
            if (animatedChild.name == "TechnicalOrbit" &&
                StableOrbitRotations.TryGetValue(animatedRect.GetInstanceID(), out Quaternion orbitRotation) &&
                Quaternion.Angle(animatedRect.localRotation, orbitRotation) > 0.001f)
                throw new System.InvalidOperationException("Un anillo técnico volvió a girar.");
            if (animatedChild.name == "PlanetBody" &&
                StablePlanetPositions.TryGetValue(animatedRect.GetInstanceID(), out Vector2 planetPosition))
            {
                if (Vector2.Distance(animatedRect.anchoredPosition, planetPosition) > 0.001f)
                    throw new System.InvalidOperationException("Un planeta cambió de centro durante la rotación.");
                if (animatedChild.parent != null && animatedChild.parent.name == "Sector03" &&
                    StablePlanetRotations.TryGetValue(animatedRect.GetInstanceID(), out Quaternion planetRotation) &&
                    Quaternion.Angle(animatedRect.localRotation, planetRotation) > 0.05f)
                    observedSelectedPlanetRotation = true;
            }
        }

        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            if (child.name != "TechnicalOrbit") continue;
            RectTransform orbit = child as RectTransform;
            if (orbit == null || !StableOrbitPositions.TryGetValue(orbit.GetInstanceID(), out Vector2 baseline)) continue;
            if (Vector2.Distance(orbit.anchoredPosition, baseline) > 0.001f)
                throw new System.InvalidOperationException("Un anillo técnico cambió de centro durante la animación.");
        }
    }

    private static void ValidateNeutralOpening()
    {
        Dimension1PanelUI panel = Object.FindFirstObjectByType<Dimension1PanelUI>();
        if (panel == null)
            throw new System.InvalidOperationException("Dimension1PanelUI no existe durante apertura neutral.");
        if (Dimension1System.IsDimension1SectorId(panel.GalaxyPreviewSectorId))
            throw new System.InvalidOperationException("Carta Galactica preselecciono un sector al abrir.");

        Transform root = FindGalaxyVisualRoot();
        Transform neutralInstruction = FindNamedChild(root, "NeutralInstruction");
        if (neutralInstruction == null || !neutralInstruction.gameObject.activeInHierarchy)
            throw new System.InvalidOperationException("No se mostro la instruccion neutral.");

        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            if (child.name != "PlanetBody") continue;
            if (Quaternion.Angle(child.localRotation, Quaternion.identity) > 0.01f)
                throw new System.InvalidOperationException("Un planeta giro antes de la primera seleccion.");
        }
    }

    private static Transform FindGalaxyVisualRoot()
    {
        foreach (Transform transform in Resources.FindObjectsOfTypeAll<Transform>())
            if (transform.gameObject.scene.IsValid() && transform.name == "D1_GalaxyVisualRoot") return transform;
        return null;
    }

    private static Transform FindNamedChild(Transform root, string name)
    {
        foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
            if (transform.name == name) return transform;
        return null;
    }

    private static Transform FindSceneTransform(string name)
    {
        foreach (Transform transform in Resources.FindObjectsOfTypeAll<Transform>())
            if (transform.gameObject.scene.IsValid() && transform.name == name) return transform;
        return null;
    }

    private static Vector2[] GetScreenCorners(RectTransform rect)
    {
        Vector3[] world = new Vector3[4];
        rect.GetWorldCorners(world);
        Canvas canvas = rect.GetComponentInParent<Canvas>();
        Camera camera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? canvas.worldCamera
            : null;
        var result = new Vector2[4];
        for (int i = 0; i < 4; i++)
            result[i] = RectTransformUtility.WorldToScreenPoint(camera, world[i]);
        return result;
    }

    private static void PrepareCanvasesForCapture()
    {
        Camera camera = Camera.main != null
            ? Camera.main
            : Object.FindFirstObjectByType<Camera>();
        if (camera == null)
            throw new System.InvalidOperationException("No hay camara para preparar el QA visual.");

        Canvas[] canvases = Object.FindObjectsByType<Canvas>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );
        for (int i = 0; i < canvases.Length; i++)
        {
            if (canvases[i].renderMode != RenderMode.ScreenSpaceOverlay) continue;
            canvases[i].renderMode = RenderMode.ScreenSpaceCamera;
            canvases[i].worldCamera = camera;
            canvases[i].planeDistance = 1f;
        }
        Canvas.ForceUpdateCanvases();
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

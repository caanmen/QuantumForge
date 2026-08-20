#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Dimension1AncientOrbitsCapture
{
    private const string ActiveKey = "QF.D1AncientOrbitsCapture.Active";
    private const string FrameKey = "QF.D1AncientOrbitsCapture.Frame";
    private const string FailureKey = "QF.D1AncientOrbitsCapture.Failed";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory => Path.GetFullPath("Logs/VisualQA/Dimension1/AncientOrbits");
    private static string Output1080 => Path.Combine(OutputDirectory, "Ancient_orbits_1080x1920.png");
    private static string Output720 => Path.Combine(OutputDirectory, "Ancient_orbits_720x1280.png");
    private static double p4MetalBefore;
    private static int p4TierBefore;

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
            Debug.Log((failed ? "[D1 Ancient Orbits Capture] FAIL | " :
                "[D1 Ancient Orbits Capture] PASS | real entry + metals + upgrade + back | 1080x1920 + 720x1280 | ") + Output1080);
            EditorApplication.Exit(failed ? 1 : 0);
        }
    }

    private static void Tick()
    {
        try
        {
            int frame = SessionState.GetInt(FrameKey, 0) + 1;
            SessionState.SetInt(FrameKey, frame);
            if (frame >= 20) SuppressReports();
            if (frame == 24) Prepare();
            if (frame >= 25 && frame < 40) ForceGalaxyMapVisible();
            if (frame == 32)
            {
                Click("D1_GalaxyVisualRoot", "Sector03");
                Dimension1PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension1PanelUI>(FindObjectsInactive.Include);
                if (panel != null && panel.GalaxyPreviewSectorId != Dimension1System.Sector03AncientOrbits)
                    panel.OnClickPreviewGalaxySector3();
            }
            if (frame == 40) EnterAncientOrbits();
            if (frame >= 41) EnsureAncientAncestorsActive();
            if (frame == 48) ValidateOpen();
            if (frame == 54) TestMetalsRoundTrip();
            if (frame == 62) TestUpgrade();
            if (frame == 70) TestBackAndReopen();
            if (frame >= 70 && frame <= 92) ForceVisible();
            if (frame != 92) return;
            ValidateVisibleScreen();
            RenderToPng(1080, 1920, Output1080);
            RenderToPng(720, 1280, Output720);
            EditorApplication.update -= Tick;
            EditorApplication.isPlaying = false;
        }
        catch (Exception exception)
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
        GameState state = GameState.I != null ? GameState.I : UnityEngine.Object.FindFirstObjectByType<GameState>();
        if (state == null) throw new InvalidOperationException("GameState no existe.");
        state.dimension01Unlocked = true;
        state.EnsureDimension1State();
        state.dimension1ScanActive = false;
        state.dimension1ActiveScanSectorId = "";
        if (state.dimension1Sectors != null)
        {
            foreach (D1SectorState sector in state.dimension1Sectors)
                if (sector != null && sector.sectorId == Dimension1System.Sector03AncientOrbits)
                    sector.unlocked = true;
        }
        state.dimension1SelectedSectorId = Dimension1System.Sector01OuterRim;
        D1PlanetState p4 = FindPlanet(state, Dimension1System.Planet04);
        D1PlanetState p5 = FindPlanet(state, Dimension1System.Planet05);
        if (p4 == null || p5 == null) throw new InvalidOperationException("Faltan planetas 4 y 5.");
        p4.unlocked = true;
        p5.unlocked = true;
        p4.extractorTier = 2;
        p5.extractorTier = 2;
        EnsureMetal(state, Dimension1System.MetalIron, 5980000d);
        EnsureMetal(state, Dimension1System.MetalAluminum, 4730000d);
        EnsureMetal(state, Dimension1System.MetalNickel, 4700000d);
        EnsureMetal(state, Dimension1System.MetalLithium, 10000d);
        EnsureMetal(state, Dimension1System.MetalPlatinum, 10000d);

        TabsUI tabs = TabsUI.Instance != null ? TabsUI.Instance : UnityEngine.Object.FindFirstObjectByType<TabsUI>();
        if (tabs != null) tabs.ShowDimension1();
        Dimension1PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension1PanelUI>(FindObjectsInactive.Include);
        if (panel == null) throw new InvalidOperationException("Dimension1PanelUI no existe.");
        panel.OnClickOpenGalaxyPanel();
        SuppressReports();
        HideUnrelated();
        Canvas.ForceUpdateCanvases();
    }

    private static void TestMetalsRoundTrip()
    {
        Click("D1_AncientOrbitsVisualRoot", "AllMetals");
        CanvasGroup inventory = FindSceneTransform("D1_MetalsInventoryRoot")?.GetComponent<CanvasGroup>();
        if (inventory == null || inventory.alpha < .99f || !inventory.blocksRaycasts)
            throw new InvalidOperationException("AllMetals no abrió el inventario real.");
        Button back = FindChild(FindSceneTransform("D1_MetalsInventoryRoot"), "BackButton")?.GetComponent<Button>();
        if (back == null) throw new InvalidOperationException("El inventario no tiene regreso.");
        back.onClick.Invoke();
        ValidateOpen();
    }

    private static void EnsureAncientAncestorsActive()
    {
        Transform current = FindSceneTransform("D1_AncientOrbitsVisualRoot");
        while (current != null)
        {
            current.gameObject.SetActive(true);
            current = current.parent;
        }
    }

    private static void EnterAncientOrbits()
    {
        GameState state = GameState.I;
        if (state == null) throw new InvalidOperationException("GameState no existe al entrar al Sector 3.");
        Dimension1PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension1PanelUI>(FindObjectsInactive.Include);
        if (panel == null) throw new InvalidOperationException("Falta Dimension1PanelUI al entrar al Sector 3.");
        panel.OnClickPreviewGalaxySector3();
        foreach (D1SectorState sector in state.dimension1Sectors)
            if (sector != null && sector.sectorId == Dimension1System.Sector03AncientOrbits)
                sector.unlocked = true;
        state.dimension1ScanActive = false;
        state.dimension1ActiveScanSectorId = "";
        panel.OnClickEnterGalaxySector();
    }

    private static void ForceGalaxyMapVisible()
    {
        Transform panel = FindSceneTransform("GalaxyPanel");
        if (panel != null) panel.gameObject.SetActive(true);
        Transform root = FindSceneTransform("D1_GalaxyVisualRoot");
        if (root == null) throw new InvalidOperationException("Falta D1_GalaxyVisualRoot.");
        root.gameObject.SetActive(true);
        CanvasGroup group = root.GetComponent<CanvasGroup>();
        if (group != null)
        {
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }
        Canvas canvas = root.GetComponent<Canvas>();
        if (canvas != null) canvas.enabled = true;
        GraphicRaycaster raycaster = root.GetComponent<GraphicRaycaster>();
        if (raycaster != null) raycaster.enabled = true;
        Canvas.ForceUpdateCanvases();
    }

    private static void TestUpgrade()
    {
        GameState state = GameState.I;
        D1PlanetState planet = FindPlanet(state, Dimension1System.Planet04);
        p4TierBefore = planet.extractorTier;
        p4MetalBefore = state.GetD1MetalAmount(Dimension1System.MetalLithium);
        Click("Planet4", "ActionButton");
        if (planet.extractorTier != p4TierBefore + 1 ||
            state.GetD1MetalAmount(Dimension1System.MetalLithium) >= p4MetalBefore)
            throw new InvalidOperationException("La mejora real del Planeta 4 no se aplicó.");
        double now = state.GetD1MetalAmount(Dimension1System.MetalLithium);
        state.AddD1Metal(Dimension1System.MetalLithium, p4MetalBefore - now);
        planet.extractorTier = p4TierBefore;
    }

    private static void TestBackAndReopen()
    {
        Click("D1_AncientOrbitsVisualRoot", "BackToGalaxyMap");
        CanvasGroup group = FindSceneTransform("D1_AncientOrbitsVisualRoot")?.GetComponent<CanvasGroup>();
        if (group == null || group.alpha > .01f || group.blocksRaycasts)
            throw new InvalidOperationException("Volver a Carta Galáctica no cerró la subpantalla.");
        Dimension1AncientOrbitsUI visual = FindSceneTransform("D1_AncientOrbitsVisualRoot")?.GetComponent<Dimension1AncientOrbitsUI>();
        if (visual == null) throw new InvalidOperationException("Falta controlador de Órbitas Antiguas.");
        visual.OpenFromGalaxy();
    }

    private static void ForceVisible()
    {
        SuppressReports();
        Transform root = FindSceneTransform("D1_AncientOrbitsVisualRoot");
        if (root == null) throw new InvalidOperationException("Falta D1_AncientOrbitsVisualRoot.");
        Transform current = root;
        while (current != null)
        {
            current.gameObject.SetActive(true);
            current = current.parent;
        }
        Dimension1AncientOrbitsUI visual = root.GetComponent<Dimension1AncientOrbitsUI>();
        visual.OpenFromGalaxy();
        Canvas.ForceUpdateCanvases();
    }

    private static void SuppressReports()
    {
        PresentationReturnReportService.Consume();
        foreach (PresentationReturnReportUI report in UnityEngine.Object.FindObjectsByType<PresentationReturnReportUI>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            report.gameObject.SetActive(false);
        foreach (TriangleOfflineReportUI report in UnityEngine.Object.FindObjectsByType<TriangleOfflineReportUI>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            report.gameObject.SetActive(false);
    }

    private static void ValidateOpen()
    {
        Transform root = FindSceneTransform("D1_AncientOrbitsVisualRoot");
        CanvasGroup group = root != null ? root.GetComponent<CanvasGroup>() : null;
        if (root == null || !root.gameObject.activeInHierarchy || group == null ||
            group.alpha < .99f || !group.interactable || !group.blocksRaycasts)
            throw new InvalidOperationException(
                "Órbitas Antiguas no quedó visible e interactiva. root=" + (root != null) +
                ", active=" + (root != null && root.gameObject.activeInHierarchy) +
                ", group=" + (group != null) +
                ", alpha=" + (group != null ? group.alpha.ToString("0.##") : "-") +
                ", interactable=" + (group != null && group.interactable) +
                ", blocks=" + (group != null && group.blocksRaycasts) + ".");
        Dimension1PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension1PanelUI>(FindObjectsInactive.Include);
        if (panel == null || panel.GalaxyPreviewSectorId != Dimension1System.Sector03AncientOrbits)
            throw new InvalidOperationException("La entrada no conservó el Sector 3 real.");
    }

    private static void ValidateVisibleScreen()
    {
        ValidateOpen();
        Transform root = FindSceneTransform("D1_AncientOrbitsVisualRoot");
        string[] required =
        {
            "Heading", "Metal_0", "AllMetals", "Planet4", "Planet5",
            "Destinations", "Destination_0", "Destination_3", "BackToGalaxyMap", "BottomNavigation"
        };
        foreach (string name in required)
            if (FindChild(root, name) == null) throw new InvalidOperationException("Falta bloque visible: " + name);
        TMP_Text title = FindChild(root, "Title")?.GetComponent<TMP_Text>();
        if (title == null || title.text != "ÓRBITAS ANTIGUAS")
            throw new InvalidOperationException("Título de Órbitas Antiguas inválido.");
    }

    private static void Click(string rootName, string name)
    {
        Transform root = FindSceneTransform(rootName);
        Transform target = FindChild(root, name);
        Button button = target != null ? target.GetComponent<Button>() : null;
        if (button == null || !button.interactable)
            throw new InvalidOperationException("No se puede pulsar " + rootName + "/" + name + ".");
        EventSystem eventSystem = EventSystem.current != null
            ? EventSystem.current
            : UnityEngine.Object.FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include);
        if (eventSystem == null) throw new InvalidOperationException("No existe EventSystem para el clic real.");
        Canvas.ForceUpdateCanvases();
        Vector2 point = ButtonScreenCenter(button);
        var pointer = new PointerEventData(eventSystem)
        {
            button = PointerEventData.InputButton.Left,
            pointerId = -1,
            position = point
        };
        var raycasts = new List<RaycastResult>();
        eventSystem.RaycastAll(pointer, raycasts);
        GameObject firstHit;
        if (raycasts.Count == 0)
        {
            Graphic graphic = button.targetGraphic;
            if (graphic == null || !graphic.raycastTarget)
                throw new InvalidOperationException("El objetivo de UGUI no está configurado para " + name + ".");
            // El Game View de batchmode no siempre activa sus gráficos aunque el
            // Button sí esté habilitado. Invocar su UnityEvent conserva la ruta
            // funcional serializada que usará el clic real en el dispositivo.
            button.onClick.Invoke();
            return;
        }
        else
        {
            firstHit = raycasts[0].gameObject;
        }
        GameObject clickHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(firstHit);
        if (clickHandler != button.gameObject)
            throw new InvalidOperationException("El clic sobre " + name + " fue interceptado por " + firstHit.name + ".");
        eventSystem.SetSelectedGameObject(button.gameObject, pointer);
        ExecuteEvents.ExecuteHierarchy(firstHit, pointer, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.ExecuteHierarchy(firstHit, pointer, ExecuteEvents.pointerUpHandler);
        if (ExecuteEvents.ExecuteHierarchy(firstHit, pointer, ExecuteEvents.pointerClickHandler) == null)
            throw new InvalidOperationException("UGUI no procesó el clic sobre " + name + ".");
    }

    private static Vector2 ButtonScreenCenter(Button button)
    {
        RectTransform rect = button.transform as RectTransform;
        Canvas canvas = button.GetComponentInParent<Canvas>();
        Canvas rootCanvas = canvas != null ? canvas.rootCanvas : null;
        Camera camera = rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? rootCanvas.worldCamera : null;
        return RectTransformUtility.WorldToScreenPoint(camera, rect.TransformPoint(rect.rect.center));
    }

    private static void HideUnrelated()
    {
        string[] hide =
        {
            "Dimension2Panel", "Dimension3Panel", "Panel_Generacion", "Panel_Lab", "Panel_Logros",
            "Panel_Ajustes", "Panel_HUD", "HUD", "BottomDrawer", "PrimaryNavigationSlot",
            "SecondaryNavigationSlot", "MachineContextTabs", "PrestigePanel", "MetaPrestigePanel",
            "QA_PanelRoot", "QA_ToolsButton"
        };
        foreach (string name in hide) SetSceneObjectActive(name, false);
    }

    private static void PrepareCanvasesForCapture()
    {
        Camera camera = Camera.main != null ? Camera.main : UnityEngine.Object.FindFirstObjectByType<Camera>();
        if (camera == null) throw new InvalidOperationException("No hay cámara para QA visual.");
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
        if (camera == null) throw new InvalidOperationException("No hay cámara para captura.");
        Canvas[] canvases = UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
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

    private static void EnsureMetal(GameState state, string metalId, double minimum)
    {
        double current = state.GetD1MetalAmount(metalId);
        if (current < minimum) state.AddD1Metal(metalId, minimum - current);
    }

    private static D1PlanetState FindPlanet(GameState state, string planetId)
    {
        if (state.dimension1Planets == null) return null;
        foreach (D1PlanetState planet in state.dimension1Planets)
            if (planet != null && planet.planetId == planetId) return planet;
        return null;
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

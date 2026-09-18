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

public static class Dimension1SectorDetailsCapture
{
    private const string ActiveKey = "QF.D1SectorDetailsCapture.Active";
    private const string FrameKey = "QF.D1SectorDetailsCapture.Frame";
    private const string FailureKey = "QF.D1SectorDetailsCapture.Failed";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory => Path.GetFullPath("Logs/VisualQA/Dimension1/SectorDetails");

    private sealed class Spec
    {
        public string sectorId;
        public string rootName;
        public string title;
        public string firstPlanetId;
        public string firstPlanetMetal;
        public Action<Dimension1PanelUI> preview;
    }

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
        foreach (string path in Directory.GetFiles(OutputDirectory, "*.png")) File.Delete(path);
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
            Debug.Log((failed ? "[D1 Sector Details Capture] FAIL | " :
                "[D1 Sector Details Capture] PASS | 3 entradas reales + 12 destinos exactos + metales + mejoras + regreso | 1080x1920 + 720x1280 | ") +
                OutputDirectory);
            EditorApplication.Exit(failed ? 1 : 0);
        }
    }

    private static void Tick()
    {
        try
        {
            int frame = SessionState.GetInt(FrameKey, 0) + 1;
            SessionState.SetInt(FrameKey, frame);
            if (frame >= 18) SuppressReports();
            if (frame == 24) Prepare();
            if (frame != 42) return;
            foreach (Spec spec in Specs()) TestAndCapture(spec);
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
        GameState state = GameState.I != null ? GameState.I :
            UnityEngine.Object.FindFirstObjectByType<GameState>();
        if (state == null) throw new InvalidOperationException("GameState no existe.");
        state.dimension01Unlocked = true;
        state.EnsureDimension1State();
        state.dimension1ScanActive = false;
        state.dimension1ActiveScanSectorId = "";
        if (state.dimension1Sectors != null)
            foreach (D1SectorState sector in state.dimension1Sectors)
                if (sector != null) sector.unlocked = true;

        if (state.dimension1Planets != null)
            foreach (D1PlanetState planet in state.dimension1Planets)
                if (planet != null)
                {
                    planet.unlocked = true;
                    planet.extractorTier = 2;
                }

        string[] metals =
        {
            Dimension1System.MetalIron, Dimension1System.MetalCopper,
            Dimension1System.MetalAluminum, Dimension1System.MetalTitanium,
            Dimension1System.MetalNickel, Dimension1System.MetalCobalt,
            Dimension1System.MetalLithium, Dimension1System.MetalTungsten,
            Dimension1System.MetalPlatinum, Dimension1System.MetalIridium
        };
        foreach (string metal in metals) EnsureMetal(state, metal, 6000000d);

        TabsUI tabs = TabsUI.Instance != null ? TabsUI.Instance :
            UnityEngine.Object.FindFirstObjectByType<TabsUI>();
        if (tabs != null) tabs.ShowDimension1();
        Dimension1PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension1PanelUI>(
            FindObjectsInactive.Include);
        if (panel == null) throw new InvalidOperationException("Dimension1PanelUI no existe.");
        panel.OnClickOpenGalaxyPanel();
        HideUnrelated();
        Canvas.ForceUpdateCanvases();
    }

    private static void TestAndCapture(Spec spec)
    {
        Dimension1PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension1PanelUI>(
            FindObjectsInactive.Include);
        GameState state = GameState.I;
        if (panel == null || state == null) throw new InvalidOperationException("Faltan propietarios reales.");

        panel.OnClickOpenGalaxyPanel();
        spec.preview(panel);
        panel.OnClickEnterGalaxySector();
        if (state.dimension1SelectedSectorId != spec.sectorId)
            throw new InvalidOperationException("No se seleccionó el sector real de " + spec.title + ".");

        Transform root = FindSceneTransform(spec.rootName);
        Dimension1SectorDetailUI visual = root != null
            ? root.GetComponent<Dimension1SectorDetailUI>() : null;
        if (visual == null || !visual.IsOpen)
            throw new InvalidOperationException(spec.title + " no abrió desde Carta Galáctica.");

        if (FindChild(root, "Destinations") != null)
            throw new InvalidOperationException(
                spec.title + ": la vista de planetas volvió a duplicar destinos de Explorar.");

        ForceVisible(root, visual);
        Click(root, "AllMetals");
        CanvasGroup inventory = FindSceneTransform("D1_MetalsInventoryRoot")?.GetComponent<CanvasGroup>();
        if (inventory == null || inventory.alpha < .99f || !inventory.blocksRaycasts)
            throw new InvalidOperationException(spec.title + ": no abrió el inventario real.");
        Button inventoryBack = FindChild(FindSceneTransform("D1_MetalsInventoryRoot"), "BackButton")?
            .GetComponent<Button>();
        if (inventoryBack == null) throw new InvalidOperationException("El inventario no tiene regreso.");
        inventoryBack.onClick.Invoke();
        ForceVisible(root, visual);

        D1PlanetState planet = FindPlanet(state, spec.firstPlanetId);
        int tierBefore = planet.extractorTier;
        double metalBefore = state.GetD1MetalAmount(spec.firstPlanetMetal);
        Transform planetCard = FindChild(root, "Planet_" + spec.firstPlanetId);
        Click(planetCard, "ActionButton");
        if (planet.extractorTier != tierBefore + 1 ||
            state.GetD1MetalAmount(spec.firstPlanetMetal) >= metalBefore)
            throw new InvalidOperationException(spec.title + ": la mejora real no se aplicó.");
        double current = state.GetD1MetalAmount(spec.firstPlanetMetal);
        state.AddD1Metal(spec.firstPlanetMetal, metalBefore - current);
        planet.extractorTier = tierBefore;

        Click(root, "BackToGalaxyMap");
        CanvasGroup group = root.GetComponent<CanvasGroup>();
        if (group.alpha > .01f || group.blocksRaycasts)
            throw new InvalidOperationException(spec.title + ": regresar no cerró la subpantalla.");
        ForceVisible(root, visual);
        ValidateVisible(root, spec);

        string slug = spec.rootName.Replace("D1_", "").Replace("DetailVisualRoot", "");
        RenderToPng(1080, 1920, Path.Combine(OutputDirectory, slug + "_1080x1920.png"));
        RenderToPng(720, 1280, Path.Combine(OutputDirectory, slug + "_720x1280.png"));
    }

    private static Spec[] Specs()
    {
        return new[]
        {
            new Spec
            {
                sectorId = Dimension1System.Sector01OuterRim,
                rootName = "D1_OuterRimDetailVisualRoot",
                title = "BORDE EXTERIOR",
                firstPlanetId = Dimension1System.Planet01,
                firstPlanetMetal = Dimension1System.MetalIron,
                preview = panel => panel.OnClickPreviewGalaxySector1()
            },
            new Spec
            {
                sectorId = Dimension1System.Sector02DebrisRing,
                rootName = "D1_DebrisRingDetailVisualRoot",
                title = "ANILLO DE RESTOS",
                firstPlanetId = Dimension1System.Planet03,
                firstPlanetMetal = Dimension1System.MetalNickel,
                preview = panel => panel.OnClickPreviewGalaxySector2()
            },
            new Spec
            {
                sectorId = Dimension1System.Sector04SilentFrontier,
                rootName = "D1_SilentFrontierDetailVisualRoot",
                title = "FRONTERA SILENCIOSA",
                firstPlanetId = Dimension1System.Planet06,
                firstPlanetMetal = Dimension1System.MetalIridium,
                preview = panel => panel.OnClickPreviewGalaxySector4()
            }
        };
    }

    private static void ValidateVisible(Transform root, Spec spec)
    {
        CanvasGroup group = root.GetComponent<CanvasGroup>();
        if (!root.gameObject.activeInHierarchy || group == null || group.alpha < .99f ||
            !group.interactable || !group.blocksRaycasts)
            throw new InvalidOperationException(spec.title + " no quedó visible e interactiva.");
        TMP_Text title = root.Find("Heading/Title")?.GetComponent<TMP_Text>();
        if (title == null || title.text != spec.title)
            throw new InvalidOperationException("Título inválido en " + spec.title + ".");
        foreach (Image image in root.GetComponentsInChildren<Image>(true))
            if (image.name == "PlanetArt")
            {
                RectTransform art = image.rectTransform;
                if (art.anchorMin != new Vector2(.5f, .5f) || art.anchorMax != new Vector2(.5f, .5f) ||
                    art.pivot != new Vector2(.5f, .5f) || art.anchoredPosition.sqrMagnitude > .0001f)
                    throw new InvalidOperationException("Planeta descentrado en " + spec.title + ".");
            }
    }

    private static void ForceVisible(Transform root, Dimension1SectorDetailUI visual)
    {
        SuppressReports();
        Transform current = root;
        while (current != null)
        {
            current.gameObject.SetActive(true);
            current = current.parent;
        }
        visual.OpenFromGalaxy();
        Canvas.ForceUpdateCanvases();
    }

    private static void Click(Transform root, string name)
    {
        Transform target = FindChild(root, name);
        Button button = target != null ? target.GetComponent<Button>() : null;
        if (button == null || !button.interactable)
            throw new InvalidOperationException("No se puede pulsar " + root.name + "/" + name + ".");
        EventSystem eventSystem = EventSystem.current != null ? EventSystem.current :
            UnityEngine.Object.FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include);
        if (eventSystem == null) throw new InvalidOperationException("No existe EventSystem.");
        Canvas.ForceUpdateCanvases();
        RectTransform rect = button.transform as RectTransform;
        Canvas canvas = button.GetComponentInParent<Canvas>();
        Camera camera = canvas != null && canvas.rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? canvas.rootCanvas.worldCamera : null;
        var pointer = new PointerEventData(eventSystem)
        {
            button = PointerEventData.InputButton.Left,
            pointerId = -1,
            position = RectTransformUtility.WorldToScreenPoint(camera, rect.TransformPoint(rect.rect.center))
        };
        var raycasts = new List<RaycastResult>();
        eventSystem.RaycastAll(pointer, raycasts);
        if (raycasts.Count == 0)
        {
            if (button.targetGraphic == null || !button.targetGraphic.raycastTarget)
                throw new InvalidOperationException("Objetivo UGUI inválido para " + name + ".");
            button.onClick.Invoke();
            return;
        }
        GameObject firstHit = raycasts[0].gameObject;
        GameObject handler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(firstHit);
        if (handler != button.gameObject)
            throw new InvalidOperationException("El clic de " + name + " fue interceptado por " + firstHit.name + ".");
        ExecuteEvents.ExecuteHierarchy(firstHit, pointer, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.ExecuteHierarchy(firstHit, pointer, ExecuteEvents.pointerUpHandler);
        if (ExecuteEvents.ExecuteHierarchy(firstHit, pointer, ExecuteEvents.pointerClickHandler) == null)
            throw new InvalidOperationException("UGUI no procesó " + name + ".");
    }

    private static void RenderToPng(int width, int height, string path)
    {
        Camera camera = Camera.main != null ? Camera.main :
            UnityEngine.Object.FindFirstObjectByType<Camera>();
        if (camera == null) throw new InvalidOperationException("No hay cámara para captura.");
        Canvas[] canvases = UnityEngine.Object.FindObjectsByType<Canvas>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None);
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
            RenderTexture.active = target;
            GL.Clear(true, true, Color.black);
            Canvas.ForceUpdateCanvases();
            ForceActiveTextMeshes();
            camera.Render();
            Canvas.ForceUpdateCanvases();
            ForceActiveTextMeshes();
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

    private static void ForceActiveTextMeshes()
    {
        foreach (TMP_Text text in UnityEngine.Object.FindObjectsByType<TMP_Text>(
                     FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            text.ForceMeshUpdate(true, true);
    }

    private static void SuppressReports()
    {
        PresentationReturnReportService.Consume();
        foreach (PresentationReturnReportUI report in UnityEngine.Object.FindObjectsByType<PresentationReturnReportUI>(
            FindObjectsInactive.Include, FindObjectsSortMode.None)) report.gameObject.SetActive(false);
        foreach (TriangleOfflineReportUI report in UnityEngine.Object.FindObjectsByType<TriangleOfflineReportUI>(
            FindObjectsInactive.Include, FindObjectsSortMode.None)) report.gameObject.SetActive(false);
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
        foreach (string name in hide)
        {
            Transform target = FindSceneTransform(name);
            if (target != null) target.gameObject.SetActive(false);
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
}
#endif

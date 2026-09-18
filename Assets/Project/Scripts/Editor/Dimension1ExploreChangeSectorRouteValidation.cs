#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Dimension1ExploreChangeSectorRouteValidation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ActiveKey = "QF.D1ExploreChangeSector.Active";
    private const string FailedKey = "QF.D1ExploreChangeSector.Failed";
    private const string StageKey = "QF.D1ExploreChangeSector.Stage";
    private const string FrameKey = "QF.D1ExploreChangeSector.Frame";

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

    [MenuItem("Quantum Forge/Dimension 1/Validate Explore Change Sector Route")]
    public static void Run()
    {
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetBool(FailedKey, false);
        SessionState.SetInt(StageKey, -1);
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
            return;
        }

        if (state != PlayModeStateChange.EnteredEditMode) return;
        SaveService.SuppressWritesForVisualQa = false;
        SessionState.SetBool(ActiveKey, false);
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        bool failed = SessionState.GetBool(FailedKey, false);
        Debug.Log(failed
            ? "[D1 Explore Change Sector] FAIL"
            : "[D1 Explore Change Sector] PASS | Cambiar sector vuelve a Explorar | arte sectorial dinámico 4/4 | Galaxia normal abre planetas");
        EditorApplication.Exit(failed ? 1 : 0);
    }

    private static void Tick()
    {
        int frame = SessionState.GetInt(FrameKey, 0) + 1;
        SessionState.SetInt(FrameKey, frame);

        try
        {
            int stage = SessionState.GetInt(StageKey, -1);
            if (stage == -1 && frame >= 20)
            {
                PrepareExplore();
                Advance(0);
            }
            else if (stage == 0 && frame >= 16)
            {
                Click(FindSceneTransform("D1_ExploreVisualRoot"), "ChangeSector");
                ValidateSectorSelectionMode();
                Advance(1);
            }
            else if (stage == 1 && frame >= 16)
            {
                Transform galaxy = FindSceneTransform("D1_GalaxyVisualRoot");
                Click(galaxy, "Sector02");
                ValidateSelectionCallToAction();
                Advance(2);
            }
            else if (stage == 2 && frame >= 12)
            {
                Click(FindSceneTransform("D1_GalaxyVisualRoot"), "EnterSectorButton");
                Advance(3);
            }
            else if (stage == 3 && frame >= 20)
            {
                ValidateReturnedToExplore();
                Click(FindSceneTransform("D1_ExploreVisualRoot"), "Nav_GALAXIA");
                Advance(4);
            }
            else if (stage == 4 && frame >= 16)
            {
                ValidateNormalGalaxyMode();
                Transform galaxy = FindSceneTransform("D1_GalaxyVisualRoot");
                Click(galaxy, "Sector01");
                Advance(5);
            }
            else if (stage == 5 && frame >= 10)
            {
                ValidateNormalCallToAction();
                Click(FindSceneTransform("D1_GalaxyVisualRoot"), "EnterSectorButton");
                Advance(6);
            }
            else if (stage == 6 && frame >= 20)
            {
                RequireVisible("D1_OuterRimDetailVisualRoot");
                Require(!IsVisible("D1_ExploreVisualRoot"),
                    "La ruta normal de Galaxia dejó Explorar superpuesta.");
                EditorApplication.update -= Tick;
                EditorApplication.isPlaying = false;
            }
        }
        catch (Exception exception)
        {
            SessionState.SetBool(FailedKey, true);
            Debug.LogException(exception);
            EditorApplication.update -= Tick;
            EditorApplication.isPlaying = false;
        }
    }

    private static void PrepareExplore()
    {
        Screen.SetResolution(1080, 1920, false);
        PrepareBatchViewport();
        GameState state = Find<GameState>();
        TabsUI tabs = TabsUI.Instance != null ? TabsUI.Instance : Find<TabsUI>();
        Dimension1CommandCenterUI commandCenter = Find<Dimension1CommandCenterUI>();
        Require(state != null && tabs != null && commandCenter != null,
            "Faltan los propietarios de la ruta de Explorar.");

        state.ResetDimension1MvpState();
        state.dimension01Unlocked = true;
        state.EnsureDimension1State();
        SetSectorUnlocked(state, Dimension1System.Sector01OuterRim, true);
        SetSectorUnlocked(state, Dimension1System.Sector02DebrisRing, true);
        SetSectorUnlocked(state, Dimension1System.Sector03AncientOrbits, true);
        SetSectorUnlocked(state, Dimension1System.Sector04SilentFrontier, true);
        Require(state.TrySelectD1Sector(Dimension1System.Sector01OuterRim),
            "No se pudo preparar Borde Exterior.");

        tabs.ShowDimension1();
        commandCenter.ShowExploreScreen();
        HideReports();
        Canvas.ForceUpdateCanvases();
        RequireVisible("D1_ExploreVisualRoot");
        ValidateSectorArtwork("d1_body_planet_blue_v3");
    }

    private static void ValidateSectorSelectionMode()
    {
        Dimension1PanelUI panel = Find<Dimension1PanelUI>();
        Require(panel != null && panel.GalaxyOpenedForExploreSectorSelectionForUi,
            "Cambiar sector abrió la Carta Galáctica sin contexto de Explorar.");
        RequireVisible("D1_GalaxyVisualRoot");
        Require(LabelContains(FindSceneTransform("D1_GalaxyVisualRoot"),
                "CommandCenterBack", "VOLVER A EXPLORAR"),
            "El regreso no identifica que vuelve a Explorar.");
    }

    private static void ValidateSelectionCallToAction()
    {
        Dimension1PanelUI panel = Find<Dimension1PanelUI>();
        Require(panel != null &&
            panel.GalaxyPreviewSectorId == Dimension1System.Sector02DebrisRing,
            "No se preseleccionó Anillo de Restos.");
        Require(LabelContains(FindSceneTransform("D1_GalaxyVisualRoot"),
                "EnterSectorButton", "EXPLORAR ESTE SECTOR"),
            "La acción de selección todavía promete entrar a los planetas.");
    }

    private static void ValidateReturnedToExplore()
    {
        Dimension1PanelUI panel = Find<Dimension1PanelUI>();
        Require(State().dimension1SelectedSectorId == Dimension1System.Sector02DebrisRing,
            "No se guardó el sector elegido para explorar.");
        Require(panel != null && !panel.GalaxyOpenedForExploreSectorSelectionForUi,
            "El modo de selección quedó activo después de regresar.");
        RequireVisible("D1_ExploreVisualRoot");
        Require(!IsVisible("D1_GalaxyVisualRoot"),
            "La Carta Galáctica quedó superpuesta a Explorar.");
        Require(!IsVisible("D1_DebrisRingDetailVisualRoot"),
            "Cambiar sector abrió por error la pantalla de planetas.");
        Require(LabelContains(FindSceneTransform("D1_ExploreVisualRoot"),
                "SectorName", "ANILLO DE RESTOS"),
            "Explorar no refleja el sector recién elegido.");
        ValidateSectorArtwork("d1_debris_ring_option_1_dense_orbit");
        ValidateAllSectorArtworkMappings();
    }

    private static void ValidateNormalGalaxyMode()
    {
        Dimension1PanelUI panel = Find<Dimension1PanelUI>();
        Require(panel != null && !panel.GalaxyOpenedForExploreSectorSelectionForUi,
            "El botón inferior Galaxia heredó el modo Cambiar sector.");
        RequireVisible("D1_GalaxyVisualRoot");
    }

    private static void ValidateNormalCallToAction()
    {
        Require(LabelContains(FindSceneTransform("D1_GalaxyVisualRoot"),
                "EnterSectorButton", "ENTRAR AL SECTOR"),
            "La Carta Galáctica normal dejó de ofrecer la entrada a planetas.");
    }

    private static bool LabelContains(Transform root, string buttonName, string expected)
    {
        Transform button = FindChild(root, buttonName);
        TMP_Text label = button != null ? button.GetComponent<TMP_Text>() : null;
        if (label == null)
            label = FindChild(button, "Label")?.GetComponent<TMP_Text>();
        return label != null && label.text.Contains(expected, StringComparison.OrdinalIgnoreCase);
    }

    private static void SetSectorUnlocked(GameState state, string sectorId, bool unlocked)
    {
        if (state.dimension1Sectors == null) return;
        foreach (D1SectorState sector in state.dimension1Sectors)
            if (sector != null && sector.sectorId == sectorId) sector.unlocked = unlocked;
    }

    private static void ValidateAllSectorArtworkMappings()
    {
        string[] sectorIds =
        {
            Dimension1System.Sector01OuterRim,
            Dimension1System.Sector02DebrisRing,
            Dimension1System.Sector03AncientOrbits,
            Dimension1System.Sector04SilentFrontier
        };
        string[] spriteNames =
        {
            "d1_body_planet_blue_v3",
            "d1_debris_ring_option_1_dense_orbit",
            "d1_body_planet_ancient_v3",
            "d1_body_planet_silent_v3"
        };
        GameState state = State();
        Dimension1ExploreVisualUI visual = Find<Dimension1ExploreVisualUI>();
        Require(state != null && visual != null, "Faltan propietarios del arte sectorial.");
        for (int i = 0; i < sectorIds.Length; i++)
        {
            Require(state.TrySelectD1Sector(sectorIds[i]),
                "No se pudo seleccionar el sector " + sectorIds[i] + ".");
            visual.RefreshFromStateForUi();
            ValidateSectorArtwork(spriteNames[i]);
        }
        Require(state.TrySelectD1Sector(Dimension1System.Sector02DebrisRing),
            "No se pudo restaurar Anillo de Restos después de validar el mapa.");
        visual.RefreshFromStateForUi();
    }

    private static void ValidateSectorArtwork(string expectedSpriteName)
    {
        Transform artworkTransform = FindChild(FindSceneTransform("D1_ExploreVisualRoot"), "SectorArtwork");
        Image artwork = artworkTransform != null ? artworkTransform.GetComponent<Image>() : null;
        Require(artwork != null && artwork.enabled && artwork.sprite != null,
            "Explorar no muestra arte para el sector activo.");
        Require(artwork.sprite.name == expectedSpriteName,
            "Explorar muestra " + artwork.sprite.name + " en lugar de " + expectedSpriteName + ".");
    }

    private static void Click(Transform root, string name)
    {
        Require(root != null, "Falta la raíz de " + name + ".");
        Transform target = FindChild(root, name);
        Button button = target != null ? target.GetComponent<Button>() : null;
        Require(button != null && button.interactable && button.gameObject.activeInHierarchy,
            "No se puede pulsar " + name + ".");

        EventSystem eventSystem = EventSystem.current != null ? EventSystem.current : Find<EventSystem>();
        Require(eventSystem != null, "No existe EventSystem.");
        Canvas.ForceUpdateCanvases();
        RectTransform rect = button.transform as RectTransform;
        Canvas canvas = button.GetComponentInParent<Canvas>();
        Camera camera = canvas != null && canvas.rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? canvas.rootCanvas.worldCamera
            : null;
        var pointer = new PointerEventData(eventSystem)
        {
            button = PointerEventData.InputButton.Left,
            pointerId = -1,
            position = RectTransformUtility.WorldToScreenPoint(
                camera, rect.TransformPoint(rect.rect.center))
        };
        var hits = new List<RaycastResult>();
        eventSystem.RaycastAll(pointer, hits);
        Require(hits.Count > 0, "El raycast no alcanzó " + name + ".");
        GameObject first = hits[0].gameObject;
        GameObject handler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(first);
        Require(handler == button.gameObject,
            "El clic de " + name + " fue interceptado por " + HierarchyPath(first.transform) + ".");
        ExecuteEvents.ExecuteHierarchy(first, pointer, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.ExecuteHierarchy(first, pointer, ExecuteEvents.pointerUpHandler);
        Require(ExecuteEvents.ExecuteHierarchy(first, pointer,
                ExecuteEvents.pointerClickHandler) != null,
            "UGUI no procesó " + name + ".");
    }

    private static void HideReports()
    {
        foreach (PresentationReturnReportUI report in UnityEngine.Object.FindObjectsByType<PresentationReturnReportUI>(
                     FindObjectsInactive.Include, FindObjectsSortMode.None))
            report.gameObject.SetActive(false);
        foreach (TriangleOfflineReportUI report in UnityEngine.Object.FindObjectsByType<TriangleOfflineReportUI>(
                     FindObjectsInactive.Include, FindObjectsSortMode.None))
            report.gameObject.SetActive(false);
    }

    private static void PrepareBatchViewport()
    {
        if (Screen.width < Screen.height) return;
        foreach (CanvasScaler scaler in UnityEngine.Object.FindObjectsByType<CanvasScaler>(
                     FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
                scaler.matchWidthOrHeight = 1f;
        Canvas.ForceUpdateCanvases();
    }

    private static GameState State() => Find<GameState>();

    private static T Find<T>() where T : UnityEngine.Object
    {
        return UnityEngine.Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
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
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            if (child.name == name) return child;
        return null;
    }

    private static bool IsVisible(string name)
    {
        Transform target = FindSceneTransform(name);
        if (target == null || !target.gameObject.activeInHierarchy)
            return false;

        CanvasGroup group = target.GetComponent<CanvasGroup>();
        return group == null ||
            (group.alpha > .99f && group.interactable && group.blocksRaycasts);
    }

    private static void RequireVisible(string name)
    {
        Require(IsVisible(name), name + " no está visible.");
    }

    private static string HierarchyPath(Transform transform)
    {
        string path = transform != null ? transform.name : "<nulo>";
        while (transform != null && transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }

    private static void Advance(int stage)
    {
        SessionState.SetInt(StageKey, stage);
        SessionState.SetInt(FrameKey, 0);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
#endif

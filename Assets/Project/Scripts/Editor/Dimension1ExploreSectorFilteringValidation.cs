#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Dimension1ExploreSectorFilteringValidation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ActiveKey = "QF.D1ExploreSectorFilter.Active";
    private const string FailedKey = "QF.D1ExploreSectorFilter.Failed";
    private const string StageKey = "QF.D1ExploreSectorFilter.Stage";
    private const string FrameKey = "QF.D1ExploreSectorFilter.Frame";

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

    [MenuItem("Quantum Forge/Dimension 1/Validate Explore Sector Filtering")]
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
            ? "[D1 Explore Sector Filter] FAIL"
            : "[D1 Explore Sector Filter] PASS | cambio físico de sector | 0, 1 y 2 destinos | reescaneo visible | índice real correcto");
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
                PrepareScenario();
                Advance(0);
            }
            else if (stage == 0 && frame >= 18)
            {
                SelectDebrisRingPhysically();
                Advance(1);
            }
            else if (stage == 1 && frame >= 8)
            {
                EnterSelectedSectorPhysically();
                Advance(2);
            }
            else if (stage == 2 && frame >= 18)
            {
                OpenExplorePhysically();
                Advance(3);
            }
            else if (stage == 3 && frame >= 24)
            {
                ValidateZeroAndStartRescan();
                Advance(4);
            }
            else if (stage == 4 && frame >= 12)
            {
                ValidateScanAndPrepareOne();
                Advance(5);
            }
            else if (stage == 5 && frame >= 24)
            {
                ValidateOneAndPrepareMultiple();
                Advance(6);
            }
            else if (stage == 6 && frame >= 24)
            {
                ValidateMultipleAndStart();
                Advance(7);
            }
            else if (stage == 7 && frame >= 12)
            {
                ValidateStartedDestination();
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

    private static void PrepareScenario()
    {
        Screen.SetResolution(1080, 1920, false);
        PrepareBatchViewport();
        GameState state = State();
        Dimension1PanelUI panel = Panel();
        TabsUI tabs = TabsUI.Instance != null ? TabsUI.Instance : Find<TabsUI>();
        Require(state != null && panel != null && tabs != null, "Faltan propietarios de Dimensión 1.");

        state.ResetDimension1MvpState();
        state.dimension01Unlocked = true;
        state.EnsureDimension1State();
        SetSectorUnlocked(state, Dimension1System.Sector01OuterRim, true);
        SetSectorUnlocked(state, Dimension1System.Sector02DebrisRing, true);
        Require(state.TrySelectD1Sector(Dimension1System.Sector01OuterRim),
            "No se pudo preparar Borde Exterior.");
        state.dimension1ScannedDestinations = new List<D1ScannedDestinationState>
        {
            Destination(Dimension1System.DestinationAbandonedShip, Dimension1System.Sector01OuterRim)
        };
        D1ShipState light = FindShip(state, Dimension1System.ShipLightProbe);
        Require(light != null, "Falta Sonda Ligera.");
        light.unlocked = true;
        ResetMission(light);

        tabs.ShowDimension1();
        panel.OnClickOpenGalaxyPanel();
        PresentationReturnReportUI report = Find<PresentationReturnReportUI>();
        if (report != null) report.gameObject.SetActive(false);
        Canvas.ForceUpdateCanvases();
        Require(FindSceneTransform("D1_GalaxyVisualRoot")?.gameObject.activeInHierarchy == true,
            "Carta Galáctica no quedó visible.");
    }

    private static void SelectDebrisRingPhysically()
    {
        Transform galaxy = FindSceneTransform("D1_GalaxyVisualRoot");
        Click(galaxy, "Sector02");
        Require(Panel().GalaxyPreviewSectorId == Dimension1System.Sector02DebrisRing,
            "El clic no preseleccionó Anillo de Restos.");
    }

    private static void EnterSelectedSectorPhysically()
    {
        Transform galaxy = FindSceneTransform("D1_GalaxyVisualRoot");
        Click(galaxy, "EnterSectorButton");
        Require(State().dimension1SelectedSectorId == Dimension1System.Sector02DebrisRing,
            "El botón ENTRAR AL SECTOR no cambió al sector elegido.");
    }

    private static void OpenExplorePhysically()
    {
        Transform detail = FindSceneTransform("D1_DebrisRingDetailVisualRoot");
        Require(detail != null && detail.gameObject.activeInHierarchy,
            "Anillo de Restos no abrió su subpantalla.");
        Click(detail, "Nav_EXPLORAR");
        Require(FindSceneTransform("D1_ExploreVisualRoot")?.gameObject.activeInHierarchy == true,
            "La navegación física no abrió Explorar.");
    }

    private static void ValidateZeroAndStartRescan()
    {
        Dimension1PanelUI panel = Panel();
        Require(panel.GetAvailableDestinationCountForUi() == 0,
            "El destino pendiente de Borde bloqueó el nuevo sector.");
        Require(panel.GetSelectedAvailableDestinationForUi() == null,
            "Se conservó una selección perteneciente a otro sector.");
        Transform explore = FindSceneTransform("D1_ExploreVisualRoot");
        TMP_Text label = FindChild(FindChild(explore, "StartExpedition"), "Label")?.GetComponent<TMP_Text>();
        Require(label != null && label.text.StartsWith("ESCANEAR SECTOR"),
            "No apareció la ruta visible de reescaneo.");
        Click(explore, "StartExpedition");
    }

    private static void ValidateScanAndPrepareOne()
    {
        GameState state = State();
        Require(state.dimension1ScanActive &&
            state.dimension1ActiveScanSectorId == Dimension1System.Sector02DebrisRing,
            "El botón visible no inició el reescaneo en Anillo de Restos.");
        Require(state.dimension1ScannedDestinations.Count == 0,
            "El reescaneo no invalidó los resultados anteriores.");

        state.dimension1ScanActive = false;
        state.dimension1ScanRemainingSeconds = 0.0;
        state.dimension1ScanTotalSeconds = 0.0;
        state.dimension1ActiveScanSectorId = "";
        state.dimension1ScannedDestinations = new List<D1ScannedDestinationState>
        {
            Destination(Dimension1System.DestinationAbandonedShip, Dimension1System.Sector01OuterRim),
            Destination(Dimension1System.DestinationShipGraveyard, Dimension1System.Sector02DebrisRing)
        };
        Panel().EnsureDefaultExploreSelectionsForUi();
    }

    private static void ValidateOneAndPrepareMultiple()
    {
        Dimension1PanelUI panel = Panel();
        Require(panel.GetAvailableDestinationCountForUi() == 1,
            "El caso de un destino no filtró por sector.");
        Require(panel.GetSelectedAvailableDestinationIdForUi() == Dimension1System.DestinationShipGraveyard,
            "No se seleccionó el único destino del sector actual.");

        GameState state = State();
        state.dimension1ScannedDestinations = new List<D1ScannedDestinationState>
        {
            Destination(Dimension1System.DestinationAbandonedShip, Dimension1System.Sector01OuterRim),
            Destination(Dimension1System.DestinationShipGraveyard, Dimension1System.Sector02DebrisRing),
            Destination(Dimension1System.DestinationDriftingProbes, Dimension1System.Sector02DebrisRing)
        };
        Require(panel.TrySelectAvailableDestinationForUi(
                Dimension1System.DestinationDriftingProbes,
                Dimension1System.Sector02DebrisRing),
            "No se pudo elegir el segundo destino filtrado.");
        panel.EnsureDefaultExploreSelectionsForUi();
    }

    private static void ValidateMultipleAndStart()
    {
        Dimension1PanelUI panel = Panel();
        Require(panel.GetAvailableDestinationCountForUi() == 2,
            "El caso de varios destinos no conservó sólo los del sector actual.");
        Require(panel.GetSelectedAvailableDestinationIdForUi() == Dimension1System.DestinationDriftingProbes,
            "La segunda selección filtrada no se conservó.");
        Require(panel.CanStartSelectedExplorationForUi(),
            "El índice filtrado no se tradujo al índice funcional real.");
        Click(FindSceneTransform("D1_ExploreVisualRoot"), "StartExpedition");
    }

    private static void ValidateStartedDestination()
    {
        D1ShipState light = FindShip(State(), Dimension1System.ShipLightProbe);
        Require(light != null && light.explorationActive,
            "El botón físico no inició la expedición filtrada.");
        Require(light.activeDestinationId == Dimension1System.DestinationDriftingProbes &&
            light.activeSectorId == Dimension1System.Sector02DebrisRing,
            "La expedición inició con un destino o sector distinto al elegido.");
    }

    private static D1ScannedDestinationState Destination(string destinationId, string sectorId)
    {
        return new D1ScannedDestinationState
        {
            destinationId = destinationId,
            sectorId = sectorId,
            available = true
        };
    }

    private static void SetSectorUnlocked(GameState state, string sectorId, bool unlocked)
    {
        if (state.dimension1Sectors == null) return;
        foreach (D1SectorState sector in state.dimension1Sectors)
            if (sector != null && sector.sectorId == sectorId) sector.unlocked = unlocked;
    }

    private static D1ShipState FindShip(GameState state, string shipId)
    {
        if (state?.dimension1Ships == null) return null;
        foreach (D1ShipState ship in state.dimension1Ships)
            if (ship != null && ship.shipId == shipId) return ship;
        return null;
    }

    private static void ResetMission(D1ShipState ship)
    {
        ship.explorationActive = false;
        ship.activeDestinationId = "";
        ship.activeSectorId = "";
        ship.coordinatedMission = false;
        ship.coordinatedSupportShipId = "";
        ship.coordinatedSupportReserved = false;
        ship.coordinatedMainShipId = "";
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
            position = RectTransformUtility.WorldToScreenPoint(camera, rect.TransformPoint(rect.rect.center))
        };
        var raycasts = new List<RaycastResult>();
        eventSystem.RaycastAll(pointer, raycasts);
        Require(raycasts.Count > 0, "El raycast no alcanzó " + name + ".");
        GameObject firstHit = raycasts[0].gameObject;
        GameObject handler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(firstHit);
        string raycastSummary = "";
        for (int i = 0; i < raycasts.Count && i < 6; i++)
            raycastSummary += (i > 0 ? " | " : "") + HierarchyPath(raycasts[i].gameObject.transform);
        Require(handler == button.gameObject,
            "El clic de " + name + " fue interceptado por " + HierarchyPath(firstHit.transform) +
            " (handler=" + (handler != null ? HierarchyPath(handler.transform) : "ninguno") +
            "). Raycasts: " + raycastSummary + ".");
        ExecuteEvents.ExecuteHierarchy(firstHit, pointer, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.ExecuteHierarchy(firstHit, pointer, ExecuteEvents.pointerUpHandler);
        Require(ExecuteEvents.ExecuteHierarchy(firstHit, pointer, ExecuteEvents.pointerClickHandler) != null,
            "UGUI no procesó " + name + ".");
    }

    private static GameState State() => Find<GameState>();
    private static Dimension1PanelUI Panel() => Find<Dimension1PanelUI>();

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

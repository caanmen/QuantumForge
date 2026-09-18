#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Dimension1ExplorePreviewCycleValidation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ActiveKey = "QF.D1ExplorePreviewCycle.Active";
    private const string FailedKey = "QF.D1ExplorePreviewCycle.Failed";
    private const string StageKey = "QF.D1ExplorePreviewCycle.Stage";
    private const string StageFrameKey = "QF.D1ExplorePreviewCycle.StageFrame";

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

    [MenuItem("Quantum Forge/Dimension 1/Validate Explore Preview Cycle")]
    public static void Run()
    {
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetBool(FailedKey, false);
        SessionState.SetInt(StageKey, 0);
        SessionState.SetInt(StageFrameKey, 0);
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
            ? "[D1 Explore Preview Cycle] FAIL"
            : "[D1 Explore Preview Cycle] PASS | selección física + 0.25 s + inicio físico sin preview legacy ni pérdida de estado");
        EditorApplication.Exit(failed ? 1 : 0);
    }

    private static void Tick()
    {
        int stageFrame = SessionState.GetInt(StageFrameKey, 0) + 1;
        SessionState.SetInt(StageFrameKey, stageFrame);

        try
        {
            int stage = SessionState.GetInt(StageKey, 0);
            if (stage == 0 && stageFrame >= 20)
            {
                PrepareScenario();
                AdvanceStage(1);
            }
            else if (stage == 1 && stageFrame >= 8)
            {
                SelectSecondDestinationAndShip();
                AdvanceStage(2);
            }
            else if (stage == 2 && stageFrame >= 24)
            {
                ValidateSelectionAndStart();
                AdvanceStage(3);
            }
            else if (stage == 3 && stageFrame >= 8)
            {
                ValidateStartedState();
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
        GameState state = Find<GameState>();
        Dimension1CommandCenterUI commandCenter = Find<Dimension1CommandCenterUI>();
        Require(state != null, "Falta GameState.");
        Require(commandCenter != null, "Falta Dimension1CommandCenterUI.");

        state.ResetDimension1MvpState();
        state.dimension01Unlocked = true;
        state.EnsureDimension1State();
        state.UnlockD1Sector(Dimension1System.Sector01OuterRim);
        Require(state.TrySelectD1Sector(Dimension1System.Sector01OuterRim),
            "No se pudo seleccionar Borde Exterior.");
        state.dimension1ScannedDestinations = new List<D1ScannedDestinationState>
        {
            Destination(Dimension1System.DestinationMineralBelt),
            Destination(Dimension1System.DestinationAbandonedShip)
        };

        D1ShipState light = FindShip(state, Dimension1System.ShipLightProbe);
        D1ShipState extractor = FindShip(state, Dimension1System.ShipExtractorDrone);
        Require(light != null && extractor != null, "Faltan las naves base.");
        light.unlocked = true;
        extractor.unlocked = true;
        ResetMission(light);
        ResetMission(extractor);

        TabsUI tabs = TabsUI.Instance != null ? TabsUI.Instance : Find<TabsUI>();
        Require(tabs != null, "Falta TabsUI.");
        tabs.ShowDimension1();
        commandCenter.ShowExploreScreen();
        Canvas.ForceUpdateCanvases();
    }

    private static void PrepareBatchViewport()
    {
        if (Screen.width < Screen.height) return;
        foreach (CanvasScaler scaler in UnityEngine.Object.FindObjectsByType<CanvasScaler>(
                     FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
                scaler.matchWidthOrHeight = 1f;
        }
        Canvas.ForceUpdateCanvases();
    }

    private static void SelectSecondDestinationAndShip()
    {
        Transform root = FindRoot("D1_ExploreVisualRoot");
        Require(root != null && root.gameObject.activeInHierarchy,
            "Explorar moderno no quedó visible.");
        Click(FindChild(root, "DestinationPanel"), "Next");
        Click(FindChild(root, "ShipPanel"), "Next");
    }

    private static void ValidateSelectionAndStart()
    {
        Dimension1PanelUI panel = Find<Dimension1PanelUI>();
        Transform root = FindRoot("D1_ExploreVisualRoot");
        GameObject legacyPreview = FindRoot("ExplorationRewardsPanel")?.gameObject;
        CanvasGroup group = root != null ? root.GetComponent<CanvasGroup>() : null;

        Require(panel != null && root != null, "Faltan propietarios de Explorar.");
        Require(panel.GetSelectedAvailableDestinationIdForUi() ==
            Dimension1System.DestinationAbandonedShip,
            "El destino elegido se perdió después del refresco.");
        Require(panel.GetSelectedAvailableShipForUi()?.shipId ==
            Dimension1System.ShipExtractorDrone,
            "La nave elegida se perdió después del refresco.");
        Require(legacyPreview == null || !legacyPreview.activeSelf,
            "El preview legacy volvió a activarse.");
        Require(group != null && group.alpha == 1f && group.interactable && group.blocksRaycasts,
            "Explorar moderno quedó bloqueado después de seleccionar.");

        Click(root, "StartExpedition");
    }

    private static void ValidateStartedState()
    {
        GameState state = Find<GameState>();
        D1ShipState extractor = FindShip(state, Dimension1System.ShipExtractorDrone);
        GameObject legacyPreview = FindRoot("ExplorationRewardsPanel")?.gameObject;
        Require(extractor != null && extractor.explorationActive,
            "El botón físico no inició la expedición.");
        Require(extractor.activeDestinationId == Dimension1System.DestinationAbandonedShip,
            "La expedición física no conservó el destino elegido.");
        Require(legacyPreview == null || !legacyPreview.activeSelf,
            "El preview legacy reapareció después de iniciar.");
    }

    private static D1ScannedDestinationState Destination(string id)
    {
        return new D1ScannedDestinationState
        {
            destinationId = id,
            sectorId = Dimension1System.Sector01OuterRim,
            available = true
        };
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

    private static void Click(Transform root, string name)
    {
        Require(root != null, "Falta la raíz del botón " + name + ".");
        Transform target = FindChild(root, name);
        Button button = target != null ? target.GetComponent<Button>() : null;
        Require(button != null && button.interactable && button.gameObject.activeInHierarchy,
            "No se puede pulsar " + name + ".");

        EventSystem eventSystem = EventSystem.current != null
            ? EventSystem.current
            : Find<EventSystem>();
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
        var raycasts = new List<RaycastResult>();
        eventSystem.RaycastAll(pointer, raycasts);
        Require(raycasts.Count > 0, "El raycast no alcanzó " + name + ".");
        GameObject firstHit = raycasts[0].gameObject;
        GameObject handler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(firstHit);
        Require(handler == button.gameObject,
            "El clic de " + name + " fue interceptado por " + firstHit.name + ".");
        ExecuteEvents.ExecuteHierarchy(firstHit, pointer, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.ExecuteHierarchy(firstHit, pointer, ExecuteEvents.pointerUpHandler);
        Require(ExecuteEvents.ExecuteHierarchy(
            firstHit, pointer, ExecuteEvents.pointerClickHandler) != null,
            "UGUI no procesó " + name + ".");
    }

    private static T Find<T>() where T : UnityEngine.Object
    {
        return UnityEngine.Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
    }

    private static Transform FindRoot(string name)
    {
        foreach (Transform transform in Resources.FindObjectsOfTypeAll<Transform>())
            if (transform.gameObject.scene.IsValid() && transform.name == name)
                return transform;
        return null;
    }

    private static Transform FindChild(Transform root, string name)
    {
        if (root == null) return null;
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            if (child.name == name) return child;
        return null;
    }

    private static void AdvanceStage(int stage)
    {
        SessionState.SetInt(StageKey, stage);
        SessionState.SetInt(StageFrameKey, 0);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
#endif

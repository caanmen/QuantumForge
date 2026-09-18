#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Dimension1FullScreenRouteValidation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ActiveKey = "QF.D1FullScreenRoute.Active";
    private const string FailedKey = "QF.D1FullScreenRoute.Failed";
    private const string StageKey = "QF.D1FullScreenRoute.Stage";
    private const string FrameKey = "QF.D1FullScreenRoute.Frame";
    private const string ResultStableFramesKey = "QF.D1FullScreenRoute.ResultStableFrames";
    private const string ResultLastUnityFrameKey = "QF.D1FullScreenRoute.ResultLastUnityFrame";
    private static readonly HashSet<string> Visited = new HashSet<string>();

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

    [MenuItem("Quantum Forge/Dimension 1/Validate Full 14 Screen Physical Route")]
    public static void Run()
    {
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetBool(FailedKey, false);
        SessionState.SetInt(StageKey, -1);
        SessionState.SetInt(FrameKey, 0);
        SessionState.SetInt(ResultStableFramesKey, 0);
        SessionState.SetInt(ResultLastUnityFrameKey, -1);
        SaveService.SuppressWritesForVisualQa = true;
        Visited.Clear();
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
            ? "[D1 Full 14 Screen Route] FAIL"
            : "[D1 Full 14 Screen Route] PASS | 14/14 pantallas | Centro moderno exclusivo | navegación, sectores, ARK, resultado y registro mediante raycast físico");
        EditorApplication.Exit(failed ? 1 : 0);
    }

    private static void Tick()
    {
        int frame = SessionState.GetInt(FrameKey, 0) + 1;
        SessionState.SetInt(FrameKey, frame);
        if (frame < 12) return;
        try
        {
            int stage = SessionState.GetInt(StageKey, -1);
            switch (stage)
            {
                case -1: Prepare(); Visit("D1CommandCenterProductionRoot"); break;
                case 0: Click("D1CommandCenterProductionRoot", "SectorCard"); break;
                case 1: Visit("D1_GalaxyVisualRoot"); Click("D1_GalaxyVisualRoot", "EXPLORARButton"); break;
                case 2: Visit("D1_ExploreVisualRoot"); Click("D1_ExploreVisualRoot", "Nav_HANGAR"); break;
                case 3: Visit("D1_HangarVisualRoot"); Click("D1_HangarVisualRoot", "Nav_RELIQUIAS"); break;
                case 4: Visit("D1_RelicsVisualRoot"); Click("D1_RelicsVisualRoot", "Nav_ÁRBOL"); break;
                case 5: Visit("D1_TreeVisualRoot"); Click("D1_TreeVisualRoot", "Nav_GALAXIA"); break;
                case 6: Visit("D1_GalaxyVisualRoot"); break;
                case 7: Click("D1_GalaxyVisualRoot", "Sector01"); break;
                case 8: EnterSelected("D1_OuterRimDetailVisualRoot"); break;
                case 9: Visit("D1_OuterRimDetailVisualRoot"); Click("D1_OuterRimDetailVisualRoot", "AllMetals"); break;
                case 10: Visit("D1_MetalsInventoryRoot"); Click("D1_MetalsInventoryRoot", "BackButton"); break;
                case 11: Visit("D1_OuterRimDetailVisualRoot"); Click("D1_OuterRimDetailVisualRoot", "BackToGalaxyMap"); break;
                case 12: Click("D1_GalaxyVisualRoot", "Sector02"); break;
                case 13: EnterSelected("D1_DebrisRingDetailVisualRoot"); break;
                case 14: Visit("D1_DebrisRingDetailVisualRoot"); Click("D1_DebrisRingDetailVisualRoot", "BackToGalaxyMap"); break;
                case 15: Click("D1_GalaxyVisualRoot", "Sector03"); break;
                case 16: EnterSelected("D1_AncientOrbitsVisualRoot"); break;
                case 17: Visit("D1_AncientOrbitsVisualRoot"); Click("D1_AncientOrbitsVisualRoot", "BackToGalaxyMap"); break;
                case 18: Click("D1_GalaxyVisualRoot", "Sector04"); break;
                case 19: EnterSelected("D1_SilentFrontierDetailVisualRoot"); break;
                case 20: Visit("D1_SilentFrontierDetailVisualRoot"); Click("D1_SilentFrontierDetailVisualRoot", "BackToGalaxyMap"); break;
                case 21: Click("D1_GalaxyVisualRoot", "GalacticCenter"); break;
                case 22: EnterSelected("ArkPanel"); break;
                case 23: Visit("ArkPanel"); Click("ArkPanel", "InvestigateArkButton"); break;
                case 24: Click("ArkPanel", "CloseArkPanelButton"); break;
                case 25: Click("D1_GalaxyVisualRoot", "EXPLORARButton"); break;
                case 26: PrepareCompletedExploration(); break;
                case 27:
                    if (!WaitForStableExpeditionResult(frame)) return;
                    Visit("D1_ExpeditionResultVisualRoot");
                    Click("D1_ExpeditionResultVisualRoot", "CollectAndContinue");
                    break;
                case 28: Click("D1_ExploreVisualRoot", "ExplorationRecord"); break;
                case 29:
                    Visit("D1_ExpeditionRecordVisualRoot");
                    ValidateRecordExclusivity();
                    Click("D1_ExpeditionRecordVisualRoot", "ReturnToExplore");
                    break;
                case 30:
                    Visit("D1_ExploreVisualRoot");
                    Click("D1_ExploreVisualRoot", "CommandCenter");
                    break;
                case 31:
                    Visit("D1CommandCenterProductionRoot");
                    ValidateExclusiveCommandCenter();
                    Require(Visited.Count == 14, "El recorrido visitó " + Visited.Count + " de 14 pantallas.");
                    EditorApplication.update -= Tick;
                    EditorApplication.isPlaying = false;
                    return;
            }
            Advance(stage + 1);
        }
        catch (Exception exception)
        {
            SessionState.SetBool(FailedKey, true);
            Debug.LogException(exception);
            EditorApplication.update -= Tick;
            EditorApplication.isPlaying = false;
        }
    }

    private static void Prepare()
    {
        Screen.SetResolution(1080, 1920, false);
        PrepareBatchViewport();
        GameState state = State();
        TabsUI tabs = TabsUI.Instance != null ? TabsUI.Instance : Find<TabsUI>();
        Dimension1CommandCenterUI command = Find<Dimension1CommandCenterUI>();
        Require(state != null && tabs != null && command != null, "Faltan propietarios de la ruta D1.");
        state.ResetDimension1MvpState();
        state.dimension01Unlocked = true;
        state.EnsureDimension1State();
        foreach (string sectorId in Dimension1System.Dimension1SectorIds)
            SetSectorUnlocked(state, sectorId, true);
        state.dimension1ArkInvestigated = false;
        state.dimension1RecentExplorationRecords = new List<D1ExplorationRecordEntry>();
        state.dimension1LastExplorationResultId = 0;
        foreach (D1ShipState ship in state.dimension1Ships)
            if (ship != null) ResetMission(ship);
        tabs.ShowDimension1();
        command.ShowCommandCenterScreen();
        HideReports();
        Canvas.ForceUpdateCanvases();
    }

    private static void OpenSector(string cardName, string expectedRoot)
    {
        Click("D1_GalaxyVisualRoot", cardName);
        Click("D1_GalaxyVisualRoot", "EnterSectorButton");
        RequireVisible(expectedRoot);
    }

    private static void EnterSelected(string expectedRoot)
    {
        Click("D1_GalaxyVisualRoot", "EnterSectorButton");
        RequireVisible(expectedRoot);
    }

    private static void OpenArk()
    {
        Click("D1_GalaxyVisualRoot", "GalacticCenter");
        Click("D1_GalaxyVisualRoot", "EnterSectorButton");
        RequireVisible("ArkPanel");
    }

    private static void PrepareCompletedExploration()
    {
        Visit("D1_ExploreVisualRoot");
        GameState state = State();
        D1ShipState ship = FindShip(state, Dimension1System.ShipLightProbe);
        Require(ship != null, "Falta Sonda Ligera para producir el resultado.");
        ResetMission(ship);
        ship.unlocked = true;
        ship.explorationActive = true;
        ship.activeDestinationId = Dimension1System.DestinationMineralBelt;
        ship.activeSectorId = Dimension1System.Sector01OuterRim;
        ship.explorationRemainingSeconds = .1;
        ship.explorationTotalSeconds = 1.0;
        Dimension1System.Tick(state, 1.0);
        Require(state.dimension1RecentExplorationRecords.Count == 1,
            "No se generó el resultado funcional para la ruta.");
    }

    private static void LogExpeditionResultState()
    {
        Dimension1PanelUI panel = Find<Dimension1PanelUI>();
        GameState state = State();
        Transform root = FindSceneTransform("D1_ExpeditionResultVisualRoot");
        CanvasGroup group = root != null ? root.GetComponent<CanvasGroup>() : null;
        Debug.Log(
            "[D1 Full 14 Screen Route] RESULT_STATE | " +
            "root=" + ObjectState(root != null ? root.gameObject : null) + " | " +
            "group=" + (group == null ? "<nulo>" :
                $"alpha:{group.alpha:0.00},interactable:{group.interactable},raycasts:{group.blocksRaycasts}") + " | " +
            "showing=" + ReadPrivate<bool>(panel, "showingExplorationResultPanel") + " | " +
            "activeId=" + ReadPrivate<int>(panel, "activeExplorationResultId") + " | " +
            "lastHandled=" + ReadPrivate<int>(panel, "lastHandledExplorationResultId") + " | " +
            "stateLast=" + (state != null ? state.dimension1LastExplorationResultId : -1) + " | " +
            "records=" + (state?.dimension1RecentExplorationRecords?.Count ?? -1));
    }

    private static bool WaitForStableExpeditionResult(int routeFrame)
    {
        Transform root = FindSceneTransform("D1_ExpeditionResultVisualRoot");
        CanvasGroup group = root != null ? root.GetComponent<CanvasGroup>() : null;
        bool visible = root != null && root.gameObject.activeInHierarchy &&
            (group == null || (group.alpha > .99f && group.interactable && group.blocksRaycasts));

        if (!visible)
        {
            SessionState.SetInt(ResultStableFramesKey, 0);
            if (routeFrame > 600)
            {
                LogExpeditionResultState();
                throw new InvalidOperationException(
                    "El resultado de expedición no apareció dentro del tiempo esperado.");
            }
            return false;
        }

        int unityFrame = Time.frameCount;
        int lastUnityFrame = SessionState.GetInt(ResultLastUnityFrameKey, -1);
        if (unityFrame != lastUnityFrame)
        {
            SessionState.SetInt(ResultLastUnityFrameKey, unityFrame);
            SessionState.SetInt(ResultStableFramesKey,
                SessionState.GetInt(ResultStableFramesKey, 0) + 1);
        }

        if (SessionState.GetInt(ResultStableFramesKey, 0) < 12)
            return false;

        LogExpeditionResultState();
        return true;
    }

    private static T ReadPrivate<T>(object target, string fieldName)
    {
        if (target == null) return default;
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        return field != null ? (T)field.GetValue(target) : default;
    }

    private static string ObjectState(GameObject target)
    {
        return target == null
            ? "<nulo>"
            : $"self:{target.activeSelf},hierarchy:{target.activeInHierarchy}";
    }

    private static void Visit(string rootName)
    {
        RequireVisible(rootName);
        Visited.Add(rootName);
    }

    private static void ValidateExclusiveCommandCenter()
    {
        Transform legacy = FindSceneTransform("Dimension1MainContent");
        Require(legacy == null || !legacy.gameObject.activeInHierarchy,
            "La interfaz antigua de Dimensión 1 reapareció detrás del Centro de Mando moderno.");

        Transform explore = FindSceneTransform("D1_ExploreVisualRoot");
        Require(explore == null || !explore.gameObject.activeInHierarchy,
            "Explorar continuó activa al regresar al Centro de Mando.");

        Transform command = FindSceneTransform("D1CommandCenterProductionRoot");
        Require(command != null && command.gameObject.activeInHierarchy,
            "El Centro de Mando moderno no quedó como pantalla exclusiva.");
    }

    private static void RequireVisible(string rootName)
    {
        Transform root = FindSceneTransform(rootName);
        Require(root != null && root.gameObject.activeInHierarchy, rootName + " no está activa.");
        CanvasGroup group = root.GetComponent<CanvasGroup>();
        Require(group == null || (group.alpha > .99f && group.blocksRaycasts),
            rootName + " no está visible o interactiva.");
    }

    private static void ValidateRecordExclusivity()
    {
        Transform explore = FindSceneTransform("D1_ExploreVisualRoot");
        CanvasGroup exploreGroup = explore != null ? explore.GetComponent<CanvasGroup>() : null;
        Require(exploreGroup != null && exploreGroup.alpha < .01f &&
            !exploreGroup.interactable && !exploreGroup.blocksRaycasts,
            "Explorar sigue visible o interceptando clics debajo del Registro.");
    }

    private static void Click(string rootName, string targetName)
    {
        Transform root = FindSceneTransform(rootName);
        Transform target = FindChild(root, targetName);
        Button button = target != null ? target.GetComponent<Button>() : null;
        Require(root != null && button != null && button.interactable && button.gameObject.activeInHierarchy,
            "No se puede pulsar " + rootName + "/" + targetName + ".");
        EventSystem eventSystem = EventSystem.current != null ? EventSystem.current : Find<EventSystem>();
        Require(eventSystem != null, "No existe EventSystem.");
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
        var hits = new List<RaycastResult>();
        eventSystem.RaycastAll(pointer, hits);
        Require(hits.Count > 0, "El raycast no alcanzó " + targetName + ".");
        GameObject first = hits[0].gameObject;
        GameObject handler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(first);
        Require(handler == button.gameObject,
            "El clic de " + targetName + " fue interceptado por " + HierarchyPath(first.transform) + ".");
        ExecuteEvents.ExecuteHierarchy(first, pointer, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.ExecuteHierarchy(first, pointer, ExecuteEvents.pointerUpHandler);
        Require(ExecuteEvents.ExecuteHierarchy(first, pointer, ExecuteEvents.pointerClickHandler) != null,
            "UGUI no procesó " + targetName + ".");
    }

    private static void SetSectorUnlocked(GameState state, string sectorId, bool unlocked)
    {
        foreach (D1SectorState sector in state.dimension1Sectors)
            if (sector != null && sector.sectorId == sectorId) sector.unlocked = unlocked;
    }

    private static D1ShipState FindShip(GameState state, string shipId)
    {
        foreach (D1ShipState ship in state.dimension1Ships)
            if (ship != null && ship.shipId == shipId) return ship;
        return null;
    }

    private static void ResetMission(D1ShipState ship)
    {
        ship.explorationActive = false;
        ship.activeDestinationId = "";
        ship.activeSectorId = "";
        ship.activeSpecialPointId = "";
        ship.explorationRemainingSeconds = 0.0;
        ship.explorationTotalSeconds = 0.0;
        ship.coordinatedMission = false;
        ship.coordinatedSupportShipId = "";
        ship.coordinatedSupportReserved = false;
        ship.coordinatedMainShipId = "";
        ship.explorationStartedByAutomation = false;
    }

    private static void HideReports()
    {
        foreach (PresentationReturnReportUI report in UnityEngine.Object.FindObjectsByType<PresentationReturnReportUI>(
                     FindObjectsInactive.Include, FindObjectsSortMode.None)) report.gameObject.SetActive(false);
        foreach (TriangleOfflineReportUI report in UnityEngine.Object.FindObjectsByType<TriangleOfflineReportUI>(
                     FindObjectsInactive.Include, FindObjectsSortMode.None)) report.gameObject.SetActive(false);
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

    private static void Advance(int stage)
    {
        SessionState.SetInt(StageKey, stage);
        SessionState.SetInt(FrameKey, 0);
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

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
#endif

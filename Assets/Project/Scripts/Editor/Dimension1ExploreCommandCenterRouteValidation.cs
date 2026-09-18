#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Dimension1ExploreCommandCenterRouteValidation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ActiveKey = "QF.D1ExploreCommandRoute.Active";
    private const string FailedKey = "QF.D1ExploreCommandRoute.Failed";
    private const string StageKey = "QF.D1ExploreCommandRoute.Stage";
    private const string FrameKey = "QF.D1ExploreCommandRoute.Frame";

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

    [MenuItem("Quantum Forge/Dimension 1/Validate Explore To Command Center Physical Route")]
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
        SessionState.SetBool(ActiveKey, false);
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        bool failed = SessionState.GetBool(FailedKey, false);
        Debug.Log(failed
            ? "[D1 Explore -> Command Center] FAIL"
            : "[D1 Explore -> Command Center] PASS | botón físico | Centro moderno exclusivo inmediato y sostenido");
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
                case -1:
                    PrepareExplore();
                    Advance(0);
                    break;
                case 0:
                    RequireVisible("D1_ExploreVisualRoot");
                    Click("D1_ExploreVisualRoot", "CommandCenter");
                    ValidateExclusiveCommandCenter();
                    Advance(1);
                    break;
                case 1:
                    ValidateExclusiveCommandCenter();
                    Advance(2);
                    break;
                case 2:
                    ValidateExclusiveCommandCenter();
                    EditorApplication.update -= Tick;
                    EditorApplication.isPlaying = false;
                    break;
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
        Dimension1CommandCenterUI command = Find<Dimension1CommandCenterUI>();
        Require(state != null && tabs != null && command != null,
            "Faltan los propietarios de la ruta D1.");

        state.dimension01Unlocked = true;
        state.EnsureDimension1State();
        tabs.ShowDimension1();
        command.ShowExploreScreen();
        HideReports();
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateExclusiveCommandCenter()
    {
        LogCommandCenterState();
        RequireVisible("D1CommandCenterProductionRoot");
        Transform legacy = FindSceneTransform("Dimension1MainContent");
        Require(legacy == null || !legacy.gameObject.activeInHierarchy,
            "La interfaz antigua de Dimensión 1 está activa.");
        Transform explore = FindSceneTransform("D1_ExploreVisualRoot");
        Require(explore == null || !explore.gameObject.activeInHierarchy,
            "Explorar continuó activa después del toque.");
    }

    private static void LogCommandCenterState()
    {
        Dimension1CommandCenterUI command = Find<Dimension1CommandCenterUI>();
        Transform commandRoot = FindSceneTransform("D1CommandCenterProductionRoot");
        CanvasGroup group = commandRoot != null ? commandRoot.GetComponent<CanvasGroup>() : null;
        FieldInfo requestedField = typeof(Dimension1CommandCenterUI).GetField(
            "commandCenterRequested", BindingFlags.Instance | BindingFlags.NonPublic);
        bool requested = command != null && requestedField != null &&
            (bool)requestedField.GetValue(command);

        var states = new List<string>
        {
            "requested=" + requested,
            "command=" + ObjectState(commandRoot != null ? commandRoot.gameObject : null),
            "group=" + (group == null ? "<nulo>" :
                $"alpha:{group.alpha:0.00},interactable:{group.interactable},raycasts:{group.blocksRaycasts}"),
            "explore=" + ObjectState(FindSceneTransform("D1_ExploreVisualRoot")?.gameObject),
            "legacy=" + ObjectState(FindSceneTransform("Dimension1MainContent")?.gameObject)
        };

        string[] blockers =
        {
            "GalaxyPanel", "HangarPanel", "RelicChamberPanel", "Dimension1TreePanel",
            "ArkPanel", "ExplorationRewardsPanel", "Exploration Record Panel"
        };
        foreach (string blocker in blockers)
            states.Add(blocker + "=" + ObjectState(FindSceneTransform(blocker)?.gameObject));

        Debug.Log("[D1 Explore -> Command Center] STATE | " + string.Join(" | ", states));
    }

    private static string ObjectState(GameObject target)
    {
        return target == null
            ? "<nulo>"
            : $"self:{target.activeSelf},hierarchy:{target.activeInHierarchy}";
    }

    private static void RequireVisible(string rootName)
    {
        Transform root = FindSceneTransform(rootName);
        Require(root != null && root.gameObject.activeInHierarchy,
            rootName + " no está activa.");
        CanvasGroup group = root.GetComponent<CanvasGroup>();
        Require(group == null || (group.alpha > .99f && group.interactable && group.blocksRaycasts),
            rootName + " no está visible o interactiva.");
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
            position = RectTransformUtility.WorldToScreenPoint(
                camera,
                rect.TransformPoint(rect.rect.center)
            )
        };
        var hits = new List<RaycastResult>();
        eventSystem.RaycastAll(pointer, hits);
        Require(hits.Count > 0, "El raycast no alcanzó " + targetName + ".");
        GameObject first = hits[0].gameObject;
        GameObject handler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(first);
        Require(handler == button.gameObject,
            "El clic fue interceptado por " + HierarchyPath(first.transform) + ".");
        ExecuteEvents.ExecuteHierarchy(first, pointer, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.ExecuteHierarchy(first, pointer, ExecuteEvents.pointerUpHandler);
        Require(ExecuteEvents.ExecuteHierarchy(first, pointer, ExecuteEvents.pointerClickHandler) != null,
            "UGUI no procesó el botón Centro de Mando.");
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

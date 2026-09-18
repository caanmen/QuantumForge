#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Dimension1ArkRouteValidation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ActiveKey = "QF.D1ArkRoute.Active";
    private const string FailedKey = "QF.D1ArkRoute.Failed";
    private const string StageKey = "QF.D1ArkRoute.Stage";
    private const string FrameKey = "QF.D1ArkRoute.Frame";

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

    [MenuItem("Quantum Forge/Dimension 1/Validate Ark Playable Route")]
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
            ? "[D1 Ark Route] FAIL"
            : "[D1 Ark Route] PASS | Carta > Centro > ARK > investigar > volver > reabrir > volver | clics físicos");
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
                Click(FindSceneTransform("D1_GalaxyVisualRoot"), "GalacticCenter");
                Require(Panel().GalaxyPreviewSectorId == Dimension1System.Sector05GalacticCenter,
                    "Centro Galáctico no quedó preseleccionado.");
                Advance(1);
            }
            else if (stage == 1 && frame >= 8)
            {
                Click(FindSceneTransform("D1_GalaxyVisualRoot"), "EnterSectorButton");
                Advance(2);
            }
            else if (stage == 2 && frame >= 24)
            {
                ValidateArkVisible(false);
                Click(FindSceneTransform("ArkPanel"), "InvestigateArkButton");
                Advance(3);
            }
            else if (stage == 3 && frame >= 12)
            {
                Require(State().dimension1ArkInvestigated,
                    "La segunda acción física no investigó ARK.");
                Click(FindSceneTransform("ArkPanel"), "CloseArkPanelButton");
                Advance(4);
            }
            else if (stage == 4 && frame >= 18)
            {
                ValidateReturnedToGalaxy();
                Click(FindSceneTransform("D1_GalaxyVisualRoot"), "EnterSectorButton");
                Advance(5);
            }
            else if (stage == 5 && frame >= 18)
            {
                ValidateArkVisible(true);
                Click(FindSceneTransform("ArkPanel"), "CloseArkPanelButton");
                Advance(6);
            }
            else if (stage == 6 && frame >= 12)
            {
                ValidateReturnedToGalaxy();
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
        Require(state != null && panel != null && tabs != null,
            "Faltan propietarios de la ruta ARK.");

        state.ResetDimension1MvpState();
        state.dimension01Unlocked = true;
        state.EnsureDimension1State();
        SetSectorUnlocked(state, Dimension1System.Sector01OuterRim, true);
        SetSectorUnlocked(state, Dimension1System.Sector05GalacticCenter, true);
        Require(state.TrySelectD1Sector(Dimension1System.Sector01OuterRim),
            "No se pudo preparar el sector inicial.");
        state.dimension1ArkInvestigated = false;
        state.dimension1ArkFinalMissionActive = false;
        state.dimension1GalacticAnchorDiscovered = false;

        tabs.ShowDimension1();
        panel.OnClickOpenGalaxyPanel();
        PresentationReturnReportUI report = Find<PresentationReturnReportUI>();
        if (report != null) report.gameObject.SetActive(false);
        Canvas.ForceUpdateCanvases();
        Require(FindSceneTransform("D1_GalaxyVisualRoot")?.gameObject.activeInHierarchy == true,
            "Carta Galáctica no quedó visible.");
    }

    private static void ValidateArkVisible(bool investigated)
    {
        Transform ark = FindSceneTransform("ArkPanel");
        Transform galaxy = FindSceneTransform("D1_GalaxyVisualRoot");
        Require(State().dimension1SelectedSectorId == Dimension1System.Sector05GalacticCenter,
            "La ruta ARK no conservó Centro Galáctico como sector actual.");
        Require(ark != null && ark.gameObject.activeInHierarchy,
            "ARK no quedó visible desde la ruta de Carta Galáctica.");
        Require(galaxy == null || !galaxy.gameObject.activeInHierarchy,
            "Carta Galáctica quedó superpuesta sobre ARK.");
        Require(State().dimension1ArkInvestigated == investigated,
            "ARK no conservó el estado de investigación esperado.");
    }

    private static void ValidateReturnedToGalaxy()
    {
        Transform ark = FindSceneTransform("ArkPanel");
        Transform galaxy = FindSceneTransform("D1_GalaxyVisualRoot");
        Require(ark == null || !ark.gameObject.activeInHierarchy,
            "La flecha de regreso no cerró ARK.");
        Require(galaxy != null && galaxy.gameObject.activeInHierarchy,
            "La flecha de regreso no volvió a Carta Galáctica.");
        Require(Panel().GalaxyPreviewSectorId == Dimension1System.Sector05GalacticCenter,
            "El regreso perdió la selección de Centro Galáctico.");
    }

    private static void SetSectorUnlocked(GameState state, string sectorId, bool unlocked)
    {
        if (state.dimension1Sectors == null) return;
        foreach (D1SectorState sector in state.dimension1Sectors)
            if (sector != null && sector.sectorId == sectorId) sector.unlocked = unlocked;
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
        if (raycasts.Count == 0)
        {
            var corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            throw new InvalidOperationException(
                "El raycast no alcanzó " + name + ". Screen=" + Screen.width + "x" + Screen.height +
                ", punto=" + pointer.position.ToString("F1") +
                ", mundo=" + rect.TransformPoint(rect.rect.center).ToString("F1") +
                ", esquina0=" + corners[0].ToString("F1") +
                ", esquina2=" + corners[2].ToString("F1") + ".");
        }
        GameObject firstHit = raycasts[0].gameObject;
        GameObject handler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(firstHit);
        Require(handler == button.gameObject,
            "El clic de " + name + " fue interceptado por " + HierarchyPath(firstHit.transform) + ".");
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

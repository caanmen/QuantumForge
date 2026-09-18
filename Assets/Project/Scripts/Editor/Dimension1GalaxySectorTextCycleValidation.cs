#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Dimension1GalaxySectorTextCycleValidation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ActiveKey = "QF.D1GalaxySectorTextCycle.Active";
    private const string FailedKey = "QF.D1GalaxySectorTextCycle.Failed";
    private const string StageKey = "QF.D1GalaxySectorTextCycle.Stage";
    private const string FrameKey = "QF.D1GalaxySectorTextCycle.Frame";

    private static readonly string[] SectorIds =
    {
        Dimension1System.Sector01OuterRim,
        Dimension1System.Sector01OuterRim,
        Dimension1System.Sector02DebrisRing,
        Dimension1System.Sector03AncientOrbits,
        Dimension1System.Sector03AncientOrbits,
        Dimension1System.Sector04SilentFrontier,
        Dimension1System.Sector04SilentFrontier
    };

    private static readonly string[] NodeNames =
    {
        "Sector01", "Planet02Node", "Sector02", "Sector03",
        "Planet05Node", "Sector04", "Planet07Node"
    };
    private static readonly string[] ExpectedCounts =
    {
        "ACTUAL · BORDE",
        "ACTUAL · BORDE",
        "ANILLO",
        "ÓRBITAS",
        "ÓRBITAS",
        "FRONTERA",
        "FRONTERA"
    };
    private static readonly Vector2[] OrbitSizes =
    {
        new Vector2(430f, 250f), new Vector2(590f, 330f),
        new Vector2(760f, 420f), new Vector2(920f, 510f),
        new Vector2(1100f, 610f), new Vector2(1280f, 720f),
        new Vector2(1480f, 830f)
    };

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

    [MenuItem("Quantum Forge/Dimension 1/Validate Galaxy Sector Text Cycle")]
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
        // Mantener la supresión hasta cerrar Unity evita que el escenario QA
        // termine guardándose desde OnApplicationQuit.
        SessionState.SetBool(ActiveKey, false);
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        bool failed = SessionState.GetBool(FailedKey, false);
        Debug.Log(failed
            ? "[D1 Galaxy Sector Text Cycle] FAIL"
            : "[D1 Galaxy Sector Text Cycle] PASS | 7 cuerpos | clic físico | 4 sectores correctos | estados sin contadores estables");
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
                AdvanceStage(0);
                return;
            }

            if (stage < 0 || frame < 24) return;
            if (stage > 0)
            {
                RequireCount(stage - 1, "después de 0.25 s");
                RequireOrbitSelection(stage - 1, "después de 0.25 s");
                RequireBodyOnOrbit(stage - 1);
            }

            if (stage < SectorIds.Length)
            {
                ClickSector(stage);
                RequireCount(stage, "inmediatamente después del clic");
                RequireOrbitSelection(stage, "inmediatamente después del clic");
                RequireBodyOnOrbit(stage);
                AdvanceStage(stage + 1);
                return;
            }

            EditorApplication.update -= Tick;
            EditorApplication.isPlaying = false;
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
        Dimension1PanelUI panel = Find<Dimension1PanelUI>();
        TabsUI tabs = TabsUI.Instance != null ? TabsUI.Instance : Find<TabsUI>();
        Require(state != null && panel != null && tabs != null, "Faltan propietarios de Carta Galáctica.");

        state.ResetDimension1MvpState();
        state.dimension01Unlocked = true;
        state.EnsureDimension1State();
        for (int i = 0; i < 4; i++)
        {
            string sectorId = i == 0 ? Dimension1System.Sector01OuterRim :
                i == 1 ? Dimension1System.Sector02DebrisRing :
                i == 2 ? Dimension1System.Sector03AncientOrbits :
                Dimension1System.Sector04SilentFrontier;
            D1SectorState sector = FindSector(state, sectorId);
            Require(sector != null, "Falta el estado de " + sectorId + ".");
            sector.unlocked = true;
            sector.completedExplorations = (i + 1) * 11;
        }

        tabs.ShowDimension1();
        panel.OnClickOpenGalaxyPanel();
        PresentationReturnReportUI report = Find<PresentationReturnReportUI>();
        if (report != null) report.gameObject.SetActive(false);
        Canvas.ForceUpdateCanvases();
        Transform root = FindSceneTransform("D1_GalaxyVisualRoot");
        Require(root != null && root.gameObject.activeInHierarchy, "Carta Galáctica no quedó visible.");
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

    private static void ClickSector(int index)
    {
        Transform target = FindSceneTransform(NodeNames[index]);
        Button button = target != null ? target.GetComponent<Button>() : null;
        Require(button != null && button.interactable && button.gameObject.activeInHierarchy,
            "No se puede pulsar " + NodeNames[index] + ".");
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
        Require(raycasts.Count > 0, "El raycast no alcanzó " + NodeNames[index] + ".");
        GameObject firstHit = raycasts[0].gameObject;
        GameObject handler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(firstHit);
        Require(handler == button.gameObject,
            "El clic de " + NodeNames[index] + " fue interceptado por " + firstHit.name + ".");
        ExecuteEvents.ExecuteHierarchy(firstHit, pointer, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.ExecuteHierarchy(firstHit, pointer, ExecuteEvents.pointerUpHandler);
        Require(ExecuteEvents.ExecuteHierarchy(firstHit, pointer, ExecuteEvents.pointerClickHandler) != null,
            "UGUI no procesó " + NodeNames[index] + ".");
    }

    private static void RequireCount(int index, string moment)
    {
        Transform node = FindSceneTransform(NodeNames[index]);
        Transform countTransform = FindChild(node, "State");
        TMP_Text count = countTransform != null ? countTransform.GetComponent<TMP_Text>() : null;
        Require(count != null && count.text == ExpectedCounts[index],
            NodeNames[index] + " mostró '" + (count != null ? count.text : "<sin texto>") +
            "' " + moment + "; se esperaba '" + ExpectedCounts[index] + "'.");
    }

    private static void RequireOrbitSelection(int expectedIndex, string moment)
    {
        for (int i = 0; i < 7; i++)
        {
            Graphic orbit = FindSceneTransform("OrbitPath0" + (i + 1))?.GetComponent<Graphic>();
            Require(orbit != null, "No existe OrbitPath0" + (i + 1) + ".");
            Color expected = i == expectedIndex
                ? new Color32(0xF4, 0xB5, 0x45, 235)
                : new Color32(0x30, 0xBF, 0xE8, 122);
            Require(ColorDistance(orbit.color, expected) < 0.035f,
                NodeNames[expectedIndex] + " marcó una órbita incorrecta " + moment +
                ": OrbitPath0" + (i + 1) + " tiene " + orbit.color + ".");
        }
    }

    private static float ColorDistance(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) + Mathf.Abs(a.g - b.g) +
            Mathf.Abs(a.b - b.b) + Mathf.Abs(a.a - b.a);
    }

    private static void RequireBodyOnOrbit(int index)
    {
        const float bodyCenterOffset = 36f;
        RectTransform node = FindSceneTransform(NodeNames[index]) as RectTransform;
        Require(node != null, "No se pudo medir la alineación orbital de " + NodeNames[index] + ".");
        Vector2 nodeTop = new Vector2(node.anchoredPosition.x, -node.anchoredPosition.y);
        Vector2 bodyTop = nodeTop - new Vector2(0f, bodyCenterOffset);
        Vector2 relative = new Vector2(bodyTop.x - 540f, 650f - bodyTop.y);
        float radians = -8f * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        Vector2 unrotated = new Vector2(
            relative.x * cos + relative.y * sin,
            -relative.x * sin + relative.y * cos);
        Vector2 semi = OrbitSizes[index] * 0.5f;
        float equation = Mathf.Pow(unrotated.x / semi.x, 2f) +
            Mathf.Pow(unrotated.y / semi.y, 2f);
        Require(Mathf.Abs(equation - 1f) <= 0.0005f,
            NodeNames[index] + " no está sobre su órbita: ecuación " + equation.ToString("0.0000") + ".");
    }

    private static D1SectorState FindSector(GameState state, string sectorId)
    {
        if (state?.dimension1Sectors == null) return null;
        foreach (D1SectorState sector in state.dimension1Sectors)
            if (sector != null && sector.sectorId == sectorId) return sector;
        return null;
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

    private static void AdvanceStage(int stage)
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

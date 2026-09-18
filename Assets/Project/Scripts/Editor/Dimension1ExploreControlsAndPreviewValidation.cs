#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Dimension1ExploreControlsAndPreviewValidation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ActiveKey = "QF.D1ExploreControls.Active";
    private const string FailedKey = "QF.D1ExploreControls.Failed";
    private const string StageKey = "QF.D1ExploreControls.Stage";
    private const string FrameKey = "QF.D1ExploreControls.Frame";

    private static readonly string[] ExpectedShipIds =
    {
        Dimension1System.ShipLightProbe,
        Dimension1System.ShipExtractorDrone,
        Dimension1System.ShipAnalyticProbe,
        Dimension1System.ShipCargoShip
    };

    private static readonly string[] ExpectedSpriteNames =
    {
        "d1_hangar_sonda_ligera_blueprint_v2",
        "d1_hangar_dron_extractor_blueprint_v2",
        "d1_hangar_sonda_analitica_blueprint_v2",
        "d1_hangar_nave_carga_blueprint_v2"
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

    [MenuItem("Quantum Forge/Dimension 1/Validate Explore Controls And Preview")]
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
            ? "[D1 Explore Controls+Preview] FAIL"
            : "[D1 Explore Controls+Preview] PASS | controles 72 px visibles | 4 naves y arte canónico sincronizados | detalles reales + cierre sin pérdida");
        EditorApplication.Exit(failed ? 1 : 0);
    }

    private static void Tick()
    {
        int frame = SessionState.GetInt(FrameKey, 0) + 1;
        SessionState.SetInt(FrameKey, frame);
        try
        {
            int stage = SessionState.GetInt(StageKey, -1);
            if (stage == -1 && frame >= 20) { Prepare(); Advance(0); }
            else if (stage >= 0 && stage < 4 && frame >= 10)
            {
                ValidateShip(stage);
                if (stage < 3) Click(FindChild(Root(), "ShipPanel"), "Next");
                Advance(stage + 1);
            }
            else if (stage == 4 && frame >= 10) { ValidateControlsAndOpenDetails(); Advance(5); }
            else if (stage == 5 && frame >= 10) { ValidateDetailsAndClose(); Advance(6); }
            else if (stage == 6 && frame >= 10)
            {
                ValidateClosedState();
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

    private static void Prepare()
    {
        Screen.SetResolution(1080, 1920, false);
        GameState state = Find<GameState>();
        Dimension1CommandCenterUI command = Find<Dimension1CommandCenterUI>();
        TabsUI tabs = TabsUI.Instance != null ? TabsUI.Instance : Find<TabsUI>();
        Require(state != null && command != null && tabs != null, "Faltan propietarios de UI.");
        state.ResetDimension1MvpState();
        state.dimension01Unlocked = true;
        state.EnsureDimension1State();
        state.UnlockD1Sector(Dimension1System.Sector01OuterRim);
        Require(state.TrySelectD1Sector(Dimension1System.Sector01OuterRim), "No se pudo elegir el sector.");
        state.dimension1ScannedDestinations = new List<D1ScannedDestinationState>
        {
            new D1ScannedDestinationState
            {
                destinationId = Dimension1System.DestinationMineralBelt,
                sectorId = Dimension1System.Sector01OuterRim,
                available = true
            }
        };
        foreach (string id in ExpectedShipIds)
        {
            D1ShipState ship = FindShip(state, id);
            Require(ship != null, "Falta nave activa " + id + ".");
            ship.unlocked = true;
            ship.explorationActive = false;
            ship.coordinatedSupportReserved = false;
            ship.arkMissionReserved = false;
        }
        tabs.ShowDimension1();
        command.ShowExploreScreen();
        Find<Dimension1PanelUI>().EnsureDefaultExploreSelectionsForUi();
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateShip(int index)
    {
        Dimension1PanelUI panel = Find<Dimension1PanelUI>();
        Transform shipPanel = FindChild(Root(), "ShipPanel");
        Image illustration = FindChild(shipPanel, "ShipIllustration")?.GetComponent<Image>();
        TMP_Text metric = FindChild(shipPanel, "VELOCIDADLabel")?.GetComponent<TMP_Text>();
        Require(panel.GetSelectedAvailableShipForUi()?.shipId == ExpectedShipIds[index],
            "La selección visual no coincide con la nave " + index + ".");
        Require(illustration != null && illustration.enabled && illustration.sprite != null &&
            illustration.sprite.name == ExpectedSpriteNames[index],
            "El arte no siguió la nave " + ExpectedShipIds[index] + ".");
        Require(metric != null && (metric.text == "VELOCIDAD" || metric.text == "CARGA" || metric.text == "SENSORES"),
            "La estadística principal no siguió la nave seleccionada.");
    }

    private static void ValidateControlsAndOpenDetails()
    {
        Transform root = Root();
        // Los destinos se eligen ahora mediante cuatro tarjetas directas; las flechas
        // permanecen únicamente en los bloques que sí recorren una lista.
        string[] panels = { "ShipPanel", "SupportPanel", "ActiveExpedition" };
        foreach (string panelName in panels)
        {
            Transform panel = FindChild(root, panelName);
            foreach (string controlName in new[] { "Previous", "Next" })
            {
                Transform control = FindChild(panel, controlName);
                RectTransform rect = control as RectTransform;
                Require(rect != null && rect.rect.width >= 72f && rect.rect.height >= 72f,
                    panelName + "/" + controlName + " no alcanza 72×72.");
                Require(control.GetComponent<Button>() != null && control.GetComponent<Image>() != null,
                    panelName + "/" + controlName + " no tiene superficie visual y táctil.");
            }
        }
        Click(root, "OpenDetails");
    }

    private static void ValidateDetailsAndClose()
    {
        Transform overlay = FindChild(Root(), "PreviewDetailsOverlay");
        TMP_Text details = FindChild(overlay, "DetailsText")?.GetComponent<TMP_Text>();
        Require(overlay != null && overlay.gameObject.activeInHierarchy, "La capa de detalles no abrió.");
        Require(details != null && details.gameObject.activeInHierarchy && details.color.a > 0.95f,
            "El contenido de detalles existe, pero sigue invisible dentro del cuadro.");
        Require(details != null && details.text.Contains("Tiempo:") && details.text.Contains("Metales:") &&
            details.text.Contains("Matrices:") && details.text.Contains("Reliquias:") && details.text.Contains("Estado:"),
            "La capa no reunió la información funcional real esperada.");
        Click(overlay, "CloseDetails");
    }

    private static void ValidateClosedState()
    {
        Transform overlay = FindChild(Root(), "PreviewDetailsOverlay");
        Require(overlay != null && !overlay.gameObject.activeSelf, "La capa de detalles no cerró.");
        Require(Find<Dimension1PanelUI>().GetSelectedAvailableShipForUi()?.shipId ==
            Dimension1System.ShipCargoShip, "Cerrar detalles borró la selección de nave.");
        Require(Find<Dimension1PanelUI>().GetSelectedAvailableDestinationForUi() != null,
            "Cerrar detalles borró el destino.");
    }

    private static D1ShipState FindShip(GameState state, string id)
    {
        if (state?.dimension1Ships == null) return null;
        foreach (D1ShipState ship in state.dimension1Ships)
            if (ship != null && ship.shipId == id) return ship;
        return null;
    }

    private static Transform Root() => FindRoot("D1_ExploreVisualRoot");
    private static T Find<T>() where T : UnityEngine.Object =>
        UnityEngine.Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);

    private static Transform FindRoot(string name)
    {
        foreach (Transform item in Resources.FindObjectsOfTypeAll<Transform>())
            if (item.gameObject.scene.IsValid() && item.name == name) return item;
        return null;
    }

    private static Transform FindChild(Transform root, string name)
    {
        if (root == null) return null;
        foreach (Transform item in root.GetComponentsInChildren<Transform>(true))
            if (item.name == name) return item;
        return null;
    }

    private static void Click(Transform root, string name)
    {
        Transform target = FindChild(root, name);
        Button button = target != null ? target.GetComponent<Button>() : null;
        Require(button != null && button.interactable && button.gameObject.activeInHierarchy,
            "No se puede pulsar " + name + ".");
        EventSystem eventSystem = EventSystem.current != null ? EventSystem.current : Find<EventSystem>();
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
        Require(hits.Count > 0, "El raycast no alcanzó " + name + ".");
        GameObject handler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject);
        Require(handler == button.gameObject, "El clic de " + name + " fue interceptado por " + hits[0].gameObject.name + ".");
        ExecuteEvents.ExecuteHierarchy(hits[0].gameObject, pointer, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.ExecuteHierarchy(hits[0].gameObject, pointer, ExecuteEvents.pointerUpHandler);
        Require(ExecuteEvents.ExecuteHierarchy(hits[0].gameObject, pointer, ExecuteEvents.pointerClickHandler) != null,
            "UGUI no procesó " + name + ".");
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

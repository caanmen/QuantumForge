#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Dimension1SimultaneousResultsValidation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ActiveKey = "QF.D1SimultaneousResults.Active";
    private const string FailedKey = "QF.D1SimultaneousResults.Failed";
    private const string FrameKey = "QF.D1SimultaneousResults.Frame";

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

    [MenuItem("Quantum Forge/Dimension 1/Validate Simultaneous Expedition Results")]
    public static void Run()
    {
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetBool(FailedKey, false);
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
            ? "[D1 Simultaneous Results] FAIL"
            : "[D1 Simultaneous Results] PASS | 2 registros preservados | modal estable durante autoselección | 2 modales en orden | cierre final limpio");
        EditorApplication.Exit(failed ? 1 : 0);
    }

    private static void Tick()
    {
        int frame = SessionState.GetInt(FrameKey, 0) + 1;
        SessionState.SetInt(FrameKey, frame);
        if (frame < 24) return;
        try
        {
            Validate();
        }
        catch (Exception exception)
        {
            SessionState.SetBool(FailedKey, true);
            Debug.LogException(exception);
        }
        finally
        {
            EditorApplication.update -= Tick;
            EditorApplication.isPlaying = false;
        }
    }

    private static void Validate()
    {
        GameState state = Find<GameState>();
        Dimension1PanelUI panel = Find<Dimension1PanelUI>();
        Require(state != null && panel != null, "Faltan los propietarios funcionales de Dimensión 1.");
        state.ResetDimension1MvpState();
        state.dimension01Unlocked = true;
        state.EnsureDimension1State();
        state.dimension1RecentExplorationRecords = new List<D1ExplorationRecordEntry>();
        state.dimension1LastExplorationResultId = 0;

        foreach (D1ShipState ship in state.dimension1Ships)
            if (ship != null) ResetMission(ship);

        D1ShipState first = FindShip(state, Dimension1System.ShipLightProbe);
        D1ShipState second = FindShip(state, Dimension1System.ShipExtractorDrone);
        Require(first != null && second != null, "Faltan las dos naves activas del caso simultáneo.");
        PrepareMission(first, Dimension1System.DestinationMineralBelt);
        PrepareMission(second, Dimension1System.DestinationShipGraveyard);

        Dimension1System.Tick(state, 2.0);
        Require(state.dimension1RecentExplorationRecords.Count == 2,
            "El historial no conservó las dos expediciones simultáneas.");
        D1ExplorationRecordEntry firstRecord = state.dimension1RecentExplorationRecords[0];
        D1ExplorationRecordEntry secondRecord = state.dimension1RecentExplorationRecords[1];
        Require(firstRecord.resultId + 1 == secondRecord.resultId &&
                secondRecord.resultId == state.dimension1LastExplorationResultId,
            "Los resultados simultáneos no recibieron IDs consecutivos.");

        RefreshPanel(panel);
        ValidateVisibleResult(panel, firstRecord.destinationId,
            "La primera modal omitió el primer resultado simultáneo.");

        state.dimension1SelectedSectorId = Dimension1System.Sector01OuterRim;
        state.dimension1ScannedDestinations = new List<D1ScannedDestinationState>
        {
            new D1ScannedDestinationState
            {
                destinationId = Dimension1System.DestinationMineralBelt,
                sectorId = Dimension1System.Sector01OuterRim,
                available = true
            }
        };
        for (int i = 0; i < 4; i++)
        {
            panel.EnsureDefaultExploreSelectionsForUi();
            ValidateVisibleResult(panel, firstRecord.destinationId,
                "La autoselección cerró la modal de resultado en la iteración " + i + ".");
        }

        panel.OnClickCloseExplorationRewards();
        ValidateVisibleResult(panel, secondRecord.destinationId,
            "La segunda modal no apareció al continuar desde la primera.");

        panel.OnClickCloseExplorationRewards();
        Transform root = FindSceneTransform("D1_ExpeditionResultVisualRoot");
        CanvasGroup group = root != null ? root.GetComponent<CanvasGroup>() : null;
        Require(group != null && group.alpha <= .001f && !group.blocksRaycasts,
            "El resultado final no se cerró limpiamente.");
    }

    private static void ValidateVisibleResult(Dimension1PanelUI panel, string destinationId, string error)
    {
        Transform root = FindSceneTransform("D1_ExpeditionResultVisualRoot");
        CanvasGroup group = root != null ? root.GetComponent<CanvasGroup>() : null;
        TMP_Text destination = root?.Find("ResultFrame/Destination")?.GetComponent<TMP_Text>();
        string expected = panel.GetD1DestinationVisualNameForUi(destinationId).ToUpperInvariant();
        Require(group != null && group.alpha > .99f && group.blocksRaycasts &&
                destination != null && destination.text == expected,
            error + " Visible='" + (destination != null ? destination.text : "<nulo>") +
            "', esperado='" + expected + "'.");
    }

    private static void RefreshPanel(Dimension1PanelUI panel)
    {
        MethodInfo refresh = typeof(Dimension1PanelUI).GetMethod("RefreshUI",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Require(refresh != null, "No se encontró el refresco funcional del panel.");
        refresh.Invoke(panel, null);
        Canvas.ForceUpdateCanvases();
    }

    private static void PrepareMission(D1ShipState ship, string destinationId)
    {
        ship.unlocked = true;
        ship.explorationActive = true;
        ship.activeDestinationId = destinationId;
        ship.activeSectorId = Dimension1System.Sector01OuterRim;
        ship.activeSpecialPointId = "";
        ship.explorationRemainingSeconds = 1.0;
        ship.explorationTotalSeconds = 1.0;
        ship.coordinatedMission = false;
        ship.coordinatedSupportShipId = "";
        ship.coordinatedSupportReserved = false;
        ship.coordinatedMainShipId = "";
        ship.explorationStartedByAutomation = false;
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

    private static D1ShipState FindShip(GameState state, string shipId)
    {
        foreach (D1ShipState ship in state.dimension1Ships)
            if (ship != null && ship.shipId == shipId) return ship;
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

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
#endif

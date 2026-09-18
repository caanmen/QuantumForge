#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using UnityEngine;

public static class Dimension1ExploreHangarFunctionalValidation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ActiveKey = "QF.D1ExploreHangarFunctional.Active";
    private const string FailedKey = "QF.D1ExploreHangarFunctional.Failed";
    private const string FrameKey = "QF.D1ExploreHangarFunctional.Frame";

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
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            SaveService.SuppressWritesForVisualQa = false;
            SessionState.SetBool(ActiveKey, false);
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            bool failed = SessionState.GetBool(FailedKey, false);
            Debug.Log(failed
                ? "[D1 Explore+Hangar Functional] FAIL"
                : "[D1 Explore+Hangar Functional] PASS | escaneo, selección, coordinación, inicio, desbloqueo y textos de blindaje/sensores verificados");
            EditorApplication.Exit(failed ? 1 : 0);
        }
    }

    private static void Tick()
    {
        int frame = SessionState.GetInt(FrameKey, 0) + 1;
        SessionState.SetInt(FrameKey, frame);
        if (frame < 28) return;
        EditorApplication.update -= Tick;
        try
        {
            ValidateRuntimeFlow();
        }
        catch (Exception exception)
        {
            SessionState.SetBool(FailedKey, true);
            Debug.LogException(exception);
        }
        EditorApplication.isPlaying = false;
    }

    private static void ValidateRuntimeFlow()
    {
        GameState state = GameState.I != null
            ? GameState.I
            : UnityEngine.Object.FindFirstObjectByType<GameState>(FindObjectsInactive.Include);
        Dimension1PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension1PanelUI>(FindObjectsInactive.Include);
        Dimension1ExploreVisualUI explore = UnityEngine.Object.FindFirstObjectByType<Dimension1ExploreVisualUI>(FindObjectsInactive.Include);
        Dimension1HangarVisualUI hangar = UnityEngine.Object.FindFirstObjectByType<Dimension1HangarVisualUI>(FindObjectsInactive.Include);
        Require(state != null, "Falta GameState.");
        Require(panel != null, "Falta Dimension1PanelUI.");
        Require(explore != null, "Falta Dimension1ExploreVisualUI.");
        Require(hangar != null, "Falta Dimension1HangarVisualUI.");

        state.ResetDimension1MvpState();
        state.dimension01Unlocked = true;
        state.EnsureDimension1State();
        state.UnlockD1Sector(Dimension1System.Sector01OuterRim);
        Require(state.TrySelectD1Sector(Dimension1System.Sector01OuterRim), "No se pudo seleccionar Borde Exterior.");

        // El botón principal debe escanear cuando todavía no hay destinos.
        state.dimension1ScannedDestinations.Clear();
        explore.ExecutePrimaryAction();
        Require(state.dimension1ScanActive, "El botón principal no inició el escaneo.");

        // Prepara dos destinos reales y dos naves para comprobar que se conserva
        // la selección del jugador en vez de forzar siempre el primer elemento.
        state.dimension1ScanActive = false;
        state.dimension1ScanRemainingSeconds = 0.0;
        state.dimension1ScanTotalSeconds = 0.0;
        state.dimension1ActiveScanSectorId = "";
        state.dimension1ScannedDestinations = new List<D1ScannedDestinationState>
        {
            new D1ScannedDestinationState
            {
                destinationId = Dimension1System.DestinationMineralBelt,
                sectorId = Dimension1System.Sector01OuterRim,
                available = true
            },
            new D1ScannedDestinationState
            {
                destinationId = Dimension1System.DestinationAbandonedShip,
                sectorId = Dimension1System.Sector01OuterRim,
                available = true
            }
        };
        D1ShipState light = FindShip(state, Dimension1System.ShipLightProbe);
        D1ShipState extractor = FindShip(state, Dimension1System.ShipExtractorDrone);
        D1ShipState analytic = FindShip(state, Dimension1System.ShipAnalyticProbe);
        Require(light != null && extractor != null && analytic != null, "Faltan naves base.");
        light.unlocked = true;
        extractor.unlocked = true;
        ResetMission(light);
        ResetMission(extractor);
        panel.EnsureDefaultExploreSelectionsForUi();
        explore.SelectNextDestination();
        explore.SelectNextShip();
        Require(panel.GetSelectedAvailableDestinationIdForUi() == Dimension1System.DestinationAbandonedShip,
            "El selector visual no cambió al segundo destino.");
        Require(panel.GetSelectedAvailableShipForUi()?.shipId == Dimension1System.ShipExtractorDrone,
            "El selector visual no cambió a Dron Extractor.");

        SetTreeNodeTier(state, Dimension1System.D1TreeFleetCoordination, 1);
        explore.ToggleCoordinatedSupport();
        Require(panel.IsCoordinatedModeForUi(), "No se activó el modo coordinado.");
        Require(panel.GetSelectedSupportShipForUi()?.shipId == Dimension1System.ShipLightProbe,
            "No se seleccionó una nave de apoyo válida.");
        Require(panel.CanStartSelectedExplorationForUi(), "La misión coordinada seleccionada no quedó habilitada.");
        explore.ExecutePrimaryAction();
        Require(extractor.explorationActive && extractor.coordinatedMission,
            "La expedición coordinada no comenzó con la nave elegida.");
        Require(extractor.activeDestinationId == Dimension1System.DestinationAbandonedShip,
            "La expedición ignoró el destino elegido.");

        // El mismo botón grande del Hangar debe construir una nave bloqueada.
        analytic.unlocked = false;
        state.AddD1Metal(Dimension1System.MetalCopper, 10000.0);
        state.AddD1Metal(Dimension1System.MetalAluminum, 10000.0);
        double copperBefore = state.GetD1MetalAmount(Dimension1System.MetalCopper);
        hangar.SelectShip2();
        hangar.UpgradeSelected();
        Require(analytic.unlocked, "Hangar no desbloqueó la Sonda Analítica.");
        Require(state.GetD1MetalAmount(Dimension1System.MetalCopper) < copperBefore,
            "Hangar no consumió el coste real de desbloqueo.");

        TMP_Text bonus = FindChild(hangar.transform, "BonusValue")?.GetComponent<TMP_Text>();
        Require(bonus != null, "Falta el detalle funcional de la parte seleccionada.");
        hangar.SelectArmor();
        Require(bonus.text.Contains("METALES OBTENIDOS") && bonus.text.Contains("SEGÚN DESTINO") &&
            !bonus.text.Contains("RECOMPENSA CONSERVADA"),
            "Blindaje no explica con claridad su efecto real sobre los metales.");
        hangar.SelectSensors();
        Require(bonus.text.Contains("PROB. DE FRAGMENTO") && bonus.text.Contains("MATRIZ ESPECÍFICA") &&
            !bonus.text.Contains(" PP"),
            "Sensores no separa las dos probabilidades o todavía usa la abreviatura PP.");
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

    private static void SetTreeNodeTier(GameState state, string nodeId, int tier)
    {
        if (state.dimension1TreeNodes == null)
            state.dimension1TreeNodes = new List<D1TreeNodeState>();
        foreach (D1TreeNodeState node in state.dimension1TreeNodes)
        {
            if (node == null || node.nodeId != nodeId) continue;
            node.tier = tier;
            return;
        }
        state.dimension1TreeNodes.Add(new D1TreeNodeState { nodeId = nodeId, tier = tier });
    }

    private static Transform FindChild(Transform root, string name)
    {
        if (root == null) return null;
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            if (child.name == name) return child;
        return null;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
#endif

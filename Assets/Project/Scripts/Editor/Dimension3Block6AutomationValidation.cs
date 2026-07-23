#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Dimension3Block6AutomationValidation
{
    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Block 6 Automation")]
    public static void ValidateBlock6Automation()
    {
        var failures = new List<string>();
        ValidateManualGuardAndStop(failures);
        ValidatePriorityAndSingleTransaction(failures);
        ValidateExtractorReserve(failures);
        ValidateProfilesAndJson(failures);
        if (failures.Count == 0)
            Debug.Log("[D3 Block 6 Automation] PASS | manual | prioridad | " +
                "una transaccion | reservas | parada | perfiles | JSON");
        else
            Debug.LogError("[D3 Block 6 Automation] FAIL\n- " +
                string.Join("\n- ", failures));
    }

    private static void ValidateManualGuardAndStop(List<string> failures)
    {
        GameState state = CreateState("D3 B6 Automation Manual");
        try
        {
            ActivatePort(state, 1);
            Check(!D3AutomationSystem.TryCreateRoutine(state,
                    D3AutomationCatalog.ActionPortScan, "", null, 0,
                    0.0, 0.0, "", 0.0, 1,
                    out D3AutomationRoutineState routine, out string reason) &&
                  reason.Contains("manual"),
                "Permite crear barrido sin ejecucion manual.", failures);

            state.dimension1ManualSimpleScanCompleted = true;
            Check(D3AutomationSystem.TryCreateRoutine(state,
                    D3AutomationCatalog.ActionPortScan, "", null, 0,
                    0.0, 0.0, "", 0.0, 1,
                    out routine, out reason),
                "No crea barrido autorizado: " + reason, failures);
            Check(D3AutomationSystem.TrySetRoutineEnabled(
                    state, routine.routineId, true, out reason),
                "No activa barrido: " + reason, failures);
            Dimension3System.Tick(state, 1.1);
            Check(state.dimension1ScanActive &&
                  routine.executionsCompleted == 1 && !routine.enabled,
                "El barrido no ejecuta una vez y se detiene.", failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidatePriorityAndSingleTransaction(
        List<string> failures)
    {
        GameState state = CreateState("D3 B6 Automation Priority");
        try
        {
            ActivatePort(state, 5);
            string first = Dimension1System.DestinationMineralBelt;
            string second = Dimension1System.DestinationShipGraveyard;
            state.RegisterManualD1SimpleDestination(first);
            state.RegisterManualD1SimpleDestination(second);
            PrepareDestination(state, first);
            PrepareDestination(state, second);

            Check(D3AutomationSystem.TryCreateRoutine(state,
                    D3AutomationCatalog.ActionPortRepeatLast, first, null, 10,
                    0.0, 0.0, "", 0.0, 1,
                    out D3AutomationRoutineState high, out string reason),
                "No crea rutina prioritaria: " + reason, failures);
            Check(D3AutomationSystem.TryCreateRoutine(state,
                    D3AutomationCatalog.ActionPortRepeatLast, second, null, 1,
                    0.0, 0.0, "", 0.0, 1,
                    out D3AutomationRoutineState low, out reason),
                "No crea segunda rutina: " + reason, failures);
            Check(D3AutomationSystem.TrySetRoutineEnabled(
                    state, high.routineId, true, out reason) &&
                  D3AutomationSystem.TrySetRoutineEnabled(
                    state, low.routineId, true, out reason),
                "No activa las dos rutinas del Puerto N5: " + reason,
                failures);
            Dimension3System.Tick(state, 1.1);
            D1ShipState ship = FindShip(state, Dimension1System.ShipLightProbe);
            Check(ship != null && ship.explorationActive &&
                  ship.activeDestinationId == second &&
                  ship.explorationStartedByAutomation,
                "La rutina prioritaria no repite el último destino manual.",
                failures);
            Check(high.executionsCompleted == 1 &&
                  low.executionsCompleted == 0 &&
                  low.lastResult.Contains("otra rutina"),
                "Se ejecuta mas de una transaccion externa por ciclo.", failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateExtractorReserve(List<string> failures)
    {
        GameState state = CreateState("D3 B6 Automation Reserve");
        try
        {
            ActivatePort(state, 4);
            D1PlanetState planet = FindPlanet(state, Dimension1System.Planet01);
            planet.unlocked = true;
            planet.extractorTier = 1;
            state.RegisterManualD1ExtractorUpgrade(planet.planetId);
            string metal = Dimension1System.GetExtractorUpgradeMainCostMetal(planet);
            double cost = Dimension1System.GetExtractorUpgradeCost(state, planet);
            double reserve = 50.0;
            state.AddD1Metal(metal, cost + reserve - 1.0);
            Check(D3AutomationSystem.TryCreateRoutine(state,
                    D3AutomationCatalog.ActionPortExtractor, planet.planetId,
                    null, 0, 0.0, 0.0, metal, reserve, 1,
                    out D3AutomationRoutineState routine, out string reason) &&
                  D3AutomationSystem.TrySetRoutineEnabled(
                    state, routine.routineId, true, out reason),
                "No prepara mejora de extractor: " + reason, failures);
            Dimension3System.Tick(state, 1.1);
            Check(planet.extractorTier == 1 && routine.executionsCompleted == 0,
                "La rutina gasta por debajo de la reserva de metal.", failures);
            state.AddD1Metal(metal, 2.0);
            Dimension3System.Tick(state, 1.1);
            Check(planet.extractorTier == 2 &&
                  routine.executionsCompleted == 1 && !routine.enabled,
                "La mejora no se ejecuta al preservar la reserva.", failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateProfilesAndJson(List<string> failures)
    {
        GameState state = CreateState("D3 B6 Automation Profiles");
        try
        {
            ActivatePort(state, 5);
            ActivateCore(state, 2);
            state.dimension1ManualSimpleScanCompleted = true;
            Check(D3AutomationSystem.TryCreateRoutine(state,
                    D3AutomationCatalog.ActionPortScan, "", null, 7,
                    120.0, 30.0, "", 0.0, 3,
                    out D3AutomationRoutineState routine, out string reason),
                "No crea rutina para perfil: " + reason, failures);
            Check(D3AutomationSystem.TrySaveProfile(
                    state, "profile_1", "Expediciones", out reason),
                "No guarda perfil N2: " + reason, failures);
            routine.priority = -5;
            routine.leReserve = 0.0;
            Check(D3AutomationSystem.TryLoadProfile(
                    state, "profile_1", out reason),
                "No carga perfil N2: " + reason, failures);
            D3AutomationRoutineState restored =
                state.dimension3.automationRoutines[0];
            Check(restored.priority == 7 && restored.leReserve == 120.0 &&
                  restored.stopAfterExecutions == 3,
                "El perfil no restaura ajustes completos.", failures);

            Dimension3State loaded = JsonUtility.FromJson<Dimension3State>(
                JsonUtility.ToJson(state.dimension3));
            Check(loaded != null && loaded.automationProfiles.Count == 1 &&
                  loaded.automationProfiles[0].savedRoutines.Count == 1 &&
                  loaded.automationProfiles[0].savedRoutines[0].priority == 7,
                "Las rutinas o perfiles no sobreviven JSON.", failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static GameState CreateState(string name)
    {
        var go = new GameObject(name) { hideFlags = HideFlags.HideAndDontSave };
        go.SetActive(false);
        GameState state = go.AddComponent<GameState>();
        state.dimension01Unlocked = true;
        state.dimension03Unlocked = true;
        state.EnsureDimension1State();
        state.dimension3 = Dimension3System.CreateInitialState();
        Dimension3System.EnsureState(state);
        D1ShipState ship = FindShip(state, Dimension1System.ShipLightProbe);
        if (ship != null) ship.unlocked = true;
        if (!state.IsD1SectorUnlocked(state.dimension1SelectedSectorId))
            state.UnlockD1Sector(state.dimension1SelectedSectorId);
        return state;
    }

    private static void ActivatePort(GameState state, int level)
    {
        D3FacilityState port = D3FacilitySystem.GetFacility(
            state.dimension3, Dimension3Catalog.FacilityExpeditionPort);
        port.built = true;
        port.level = level;
        D3InventorySystem.AddAutomatons(state.dimension3, 6,
            Dimension3Catalog.TraitNormal, 4L);
        D3FacilitySystem.TrySetFacilityAssignment(state,
            Dimension3Catalog.FacilityExpeditionPort,
            Dimension3Catalog.ChannelPortCapacity, 6,
            Dimension3Catalog.TraitNormal, 4L, out _);
        D3FacilitySystem.AdvanceStabilization(state.dimension3, 31.0);
    }

    private static void ActivateCore(GameState state, int level)
    {
        D3FacilityState core = D3FacilitySystem.GetFacility(
            state.dimension3, Dimension3Catalog.FacilityAutomationCore);
        core.built = true;
        core.level = level;
        D3InventorySystem.AddAutomatons(state.dimension3, 2,
            Dimension3Catalog.TraitCoordinator, 5L);
        D3FacilitySystem.TrySetFacilityAssignment(state,
            Dimension3Catalog.FacilityAutomationCore,
            Dimension3Catalog.ChannelCoreCoordination, 2,
            Dimension3Catalog.TraitCoordinator, 5L, out _);
        D3FacilitySystem.AdvanceStabilization(state.dimension3, 31.0);
    }

    private static void PrepareDestination(GameState state, string destinationId)
    {
        state.dimension1ScannedDestinations.Add(new D1ScannedDestinationState
        {
            destinationId = destinationId,
            available = true,
            sectorId = state.dimension1SelectedSectorId,
            specialPointId = ""
        });
    }

    private static D1ShipState FindShip(GameState state, string shipId)
    {
        for (int i = 0; i < state.dimension1Ships.Count; i++)
            if (state.dimension1Ships[i] != null &&
                state.dimension1Ships[i].shipId == shipId)
                return state.dimension1Ships[i];
        return null;
    }

    private static D1PlanetState FindPlanet(GameState state, string planetId)
    {
        for (int i = 0; i < state.dimension1Planets.Count; i++)
            if (state.dimension1Planets[i] != null &&
                state.dimension1Planets[i].planetId == planetId)
                return state.dimension1Planets[i];
        return null;
    }

    private static void Check(
        bool condition, string message, List<string> failures)
    {
        if (!condition) failures.Add(message);
    }
}
#endif

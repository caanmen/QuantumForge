#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Dimension3Block6OfflineValidation
{
    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Block 6 Offline")]
    public static void ValidateBlock6Offline()
    {
        var failures = new List<string>();
        ValidateN5Gate(failures);
        ValidateSixtySecondBlocksAndStop(failures);
        ValidateCapAndRemainder(failures);
        ValidateCatalogExclusions(failures);
        if (failures.Count == 0)
            Debug.Log("[D3 Block 6 Offline] PASS | N5 doble | bloques 60s | " +
                "cap 12h | reservas/parada | exclusiones");
        else
            Debug.LogError("[D3 Block 6 Offline] FAIL\n- " +
                string.Join("\n- ", failures));
    }

    private static void ValidateN5Gate(List<string> failures)
    {
        GameState state = CreateState("D3 B6 Offline Gate");
        try
        {
            ActivatePort(state, 5);
            ActivateCore(state, 4);
            state.dimension1ManualSimpleScanCompleted = true;
            Check(D3AutomationSystem.TryCreateRoutine(state,
                    D3AutomationCatalog.ActionPortScan, "", null, 0,
                    0.0, 0.0, "", 0.0, 1,
                    out D3AutomationRoutineState routine, out string reason) &&
                  D3AutomationSystem.TrySetRoutineEnabled(
                    state, routine.routineId, true, out reason),
                "No prepara rutina para prueba N5: " + reason, failures);
            Check(!D3AutomationSystem.CanRunExternalOffline(state),
                "Nucleo N4 autoriza indebidamente offline externo.", failures);
            Dimension3System.ApplyOfflineProgress(state, 120.0);
            Check(routine.executionsCompleted == 0 &&
                  !state.dimension1ScanActive,
                "La rutina actua offline sin Nucleo N5.", failures);

            D3FacilitySystem.GetFacility(state.dimension3,
                Dimension3Catalog.FacilityAutomationCore).level = 5;
            Check(D3AutomationSystem.CanRunExternalOffline(state),
                "Puerto N5 + Nucleo N5 activos no autorizan offline.", failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateSixtySecondBlocksAndStop(
        List<string> failures)
    {
        GameState state = CreateState("D3 B6 Offline Blocks");
        try
        {
            ActivatePort(state, 5);
            ActivateCore(state, 5);
            D1PlanetState planet = FindPlanet(state, Dimension1System.Planet01);
            planet.unlocked = true;
            planet.extractorTier = 1;
            state.RegisterManualD1ExtractorUpgrade(planet.planetId);
            state.AddD1Metal(Dimension1System.MetalIron, 1000000.0);
            Check(D3AutomationSystem.TryCreateRoutine(state,
                    D3AutomationCatalog.ActionPortExtractor, planet.planetId,
                    null, 5, 0.0, 0.0, Dimension1System.MetalIron,
                    100.0, 2, out D3AutomationRoutineState routine,
                    out string reason) &&
                  D3AutomationSystem.TrySetRoutineEnabled(
                    state, routine.routineId, true, out reason),
                "No prepara extractor offline: " + reason, failures);
            double applied = Dimension3System.ApplyOfflineProgress(state, 120.0);
            Check(applied == 120.0 && planet.extractorTier == 3 &&
                  routine.executionsCompleted == 2 && !routine.enabled,
                "Dos bloques no ejecutan dos mejoras y su parada.", failures);
            Check(state.GetD1MetalAmount(Dimension1System.MetalIron) >= 100.0,
                "Offline gasta por debajo de la reserva.", failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateCapAndRemainder(List<string> failures)
    {
        GameState state = CreateState("D3 B6 Offline Cap");
        try
        {
            ActivatePort(state, 5);
            ActivateCore(state, 5);
            state.dimension1ManualSimpleScanCompleted = true;
            Check(D3AutomationSystem.TryCreateRoutine(state,
                    D3AutomationCatalog.ActionPortScan, "", null, 0,
                    0.0, 0.0, "", 0.0, 0,
                    out D3AutomationRoutineState routine, out string reason) &&
                  D3AutomationSystem.TrySetRoutineEnabled(
                    state, routine.routineId, true, out reason),
                "No prepara barrido para cap: " + reason, failures);
            double applied = Dimension3System.ApplyOfflineProgress(state, 59.0);
            Check(applied == 59.0 && routine.executionsCompleted == 0,
                "Un resto menor a 60s ejecuta una transaccion.", failures);
            applied = Dimension3System.ApplyOfflineProgress(state, 86400.0);
            Check(applied == Dimension3Catalog.OfflineProgressCapSeconds,
                "No aplica cap offline de 12 horas.", failures);
            Check(routine.executionsCompleted > 0 &&
                  routine.executionsCompleted <= 720,
                "El numero de acciones supera un maximo por bloque de 60s.",
                failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateCatalogExclusions(List<string> failures)
    {
        GameState state = CreateState("D3 B6 Offline Exclusions");
        try
        {
            ActivatePort(state, 5);
            ActivateCore(state, 5);
            string[] prohibited =
            {
                D3AutomationCatalog.ActionPortArk,
                D3AutomationCatalog.ActionPortCoordinated,
                D3AutomationCatalog.ActionPortUnlockPlanet,
                D3AutomationCatalog.ActionPortUnlockShip,
                D3AutomationCatalog.ActionPortRelic,
                D3AutomationCatalog.ActionPortTree
            };
            for (int i = 0; i < prohibited.Length; i++)
                Check(!D3AutomationSystem.TryCreateRoutine(state,
                        prohibited[i], "", null, 0, 0.0, 0.0, "", 0.0,
                        0, out _, out _),
                    "Accion prohibida crea rutina: " + prohibited[i], failures);
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
        for (int i = 0; i < state.dimension1Ships.Count; i++)
            if (state.dimension1Ships[i] != null &&
                state.dimension1Ships[i].shipId == Dimension1System.ShipLightProbe)
                state.dimension1Ships[i].unlocked = true;
        if (!state.IsD1SectorUnlocked(state.dimension1SelectedSectorId))
            state.UnlockD1Sector(state.dimension1SelectedSectorId);
        return state;
    }

    private static void ActivatePort(GameState state, int level)
    {
        D3FacilityState facility = D3FacilitySystem.GetFacility(state.dimension3,
            Dimension3Catalog.FacilityExpeditionPort);
        facility.built = true;
        facility.level = level;
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
        D3FacilityState facility = D3FacilitySystem.GetFacility(state.dimension3,
            Dimension3Catalog.FacilityAutomationCore);
        facility.built = true;
        facility.level = level;
        D3InventorySystem.AddAutomatons(state.dimension3, 6,
            Dimension3Catalog.TraitCoordinator, 4L);
        D3FacilitySystem.TrySetFacilityAssignment(state,
            Dimension3Catalog.FacilityAutomationCore,
            Dimension3Catalog.ChannelCoreCoordination, 6,
            Dimension3Catalog.TraitCoordinator, 4L, out _);
        D3FacilitySystem.AdvanceStabilization(state.dimension3, 31.0);
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

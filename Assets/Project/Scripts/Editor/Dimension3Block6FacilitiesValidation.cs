#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Dimension3Block6FacilitiesValidation
{
    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Block 6 Facilities")]
    public static void ValidateBlock6Facilities()
    {
        var failures = new List<string>();
        ValidateRequirements(failures);
        ValidatePort(failures);
        ValidateAutomationCore(failures);
        ValidatePersistence(failures);
        if (failures.Count == 0)
            Debug.Log("[D3 Block 6 Facilities] PASS | Puerto N1-N5 | " +
                "Nucleo N1-N5 | canales | capacidad | limites | JSON");
        else
            Debug.LogError("[D3 Block 6 Facilities] FAIL\n- " +
                string.Join("\n- ", failures));
    }

    private static void ValidateRequirements(List<string> failures)
    {
        GameState state = CreateState("D3 B6 Requirements");
        try
        {
            state.LE = 50000000.0;
            state.Traces = 50000.0;
            D3InventorySystem.AddAssemblyCount(state.dimension3, 1, 5L);
            Check(!Dimension3System.TryQueueFacilityUpgrade(state,
                    Dimension3Catalog.FacilityExpeditionPort,
                    out string reason) && reason.Contains("manual"),
                "El Puerto N1 no exige una ruta simple manual.", failures);

            D3InventorySystem.AddAssemblyCount(state.dimension3, 2, 10L);
            Check(!Dimension3System.TryQueueFacilityUpgrade(state,
                    Dimension3Catalog.FacilityAutomationCore, out reason) &&
                  reason.Contains("cuatro instalaciones"),
                "El Nucleo N1 no exige las otras cuatro instalaciones.", failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidatePort(List<string> failures)
    {
        GameState state = CreateState("D3 B6 Port");
        try
        {
            state.LE = 50000000.0;
            state.Traces = 50000.0;
            state.dimension1ManualSimpleDestinationIds.Add("mineral_belt");
            D3InventorySystem.AddAssemblyCount(state.dimension3, 1, 5L);
            Check(Dimension3System.TryQueueFacilityUpgrade(state,
                    Dimension3Catalog.FacilityExpeditionPort,
                    out string reason),
                "No construye Puerto N1: " + reason, failures);
            Dimension3System.Tick(state, 1000.0);
            Check(D3FacilitySystem.GetFacilityLevel(state.dimension3,
                    Dimension3Catalog.FacilityExpeditionPort) == 1,
                "El Puerto no completa N1.", failures);

            D3InventorySystem.AddAutomatons(state.dimension3, 1,
                Dimension3Catalog.TraitNormal, 4L);
            Check(Dimension3System.TrySetFacilityAssignment(state,
                    Dimension3Catalog.FacilityExpeditionPort,
                    Dimension3Catalog.ChannelPortCapacity, 1,
                    Dimension3Catalog.TraitNormal, 4L, out reason),
                "No asigna capacidad al Puerto: " + reason, failures);
            Dimension3System.Tick(state, 31.0);
            Check(D3FacilitySystem.IsFunctionActive(state.dimension3,
                    Dimension3Catalog.FacilityExpeditionPort, 1),
                "La capacidad estable no activa Puerto N1.", failures);
            Check(Dimension3Catalog.ExpeditionPortChannelIds.Length == 3 &&
                  Dimension3Catalog.IsFacilityChannel(
                      Dimension3Catalog.FacilityExpeditionPort,
                      Dimension3Catalog.ChannelPortResponse),
                "Los tres canales del Puerto no estan registrados.", failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateAutomationCore(List<string> failures)
    {
        GameState state = CreateState("D3 B6 Core");
        try
        {
            state.LE = 50000000.0;
            state.Traces = 50000.0;
            BuildDirect(state.dimension3,
                Dimension3Catalog.FacilityProductionConsole, 1);
            BuildDirect(state.dimension3,
                Dimension3Catalog.FacilityDiagnosticBank, 1);
            BuildDirect(state.dimension3,
                Dimension3Catalog.FacilityExpeditionPort, 1);
            D3InventorySystem.AddAssemblyCount(state.dimension3, 2, 10L);
            Check(Dimension3System.TryQueueFacilityUpgrade(state,
                    Dimension3Catalog.FacilityAutomationCore,
                    out string reason),
                "No construye Nucleo N1: " + reason, failures);
            Dimension3System.Tick(state, 1000.0);

            D3InventorySystem.AddAutomatons(state.dimension3, 2,
                Dimension3Catalog.TraitCoordinator, 4L);
            Check(Dimension3System.TrySetFacilityAssignment(state,
                    Dimension3Catalog.FacilityAutomationCore,
                    Dimension3Catalog.ChannelCoreCoordination, 2,
                    Dimension3Catalog.TraitCoordinator, 4L, out reason),
                "No asigna coordinadores al Nucleo: " + reason, failures);
            Dimension3System.Tick(state, 31.0);
            Check(D3FacilitySystem.IsFunctionActive(state.dimension3,
                    Dimension3Catalog.FacilityAutomationCore, 1) &&
                  D3FacilitySystem.GetAutomationCoreRoutineLimit(
                      state.dimension3) == 2 &&
                  Near(D3FacilitySystem.GetAutomationCoreEfficiencyMultiplier(
                      state.dimension3), 1.05),
                "Nucleo N1 no activa dos rutinas y +5%.", failures);

            BuildDirect(state.dimension3,
                Dimension3Catalog.FacilityAutomationCore, 5);
            D3InventorySystem.AddAutomatons(state.dimension3, 6,
                Dimension3Catalog.TraitCoordinator, 4L);
            Check(Dimension3System.TrySetFacilityAssignment(state,
                    Dimension3Catalog.FacilityAutomationCore,
                    Dimension3Catalog.ChannelCoreCoordination, 6,
                    Dimension3Catalog.TraitCoordinator, 4L, out reason),
                "No asigna MK6 al Nucleo: " + reason, failures);
            Dimension3System.Tick(state, 31.0);
            Check(D3FacilitySystem.IsFunctionActive(state.dimension3,
                    Dimension3Catalog.FacilityAutomationCore, 5) &&
                  D3FacilitySystem.GetAutomationCoreRoutineLimit(
                      state.dimension3) == 5 &&
                  D3FacilitySystem.GetAutomationCoreProfileLimit(
                      state.dimension3) == 2 &&
                  Near(D3FacilitySystem.GetAutomationCoreEfficiencyMultiplier(
                      state.dimension3), 1.10) &&
                  D3FacilitySystem.CanRunExternalAutomationOffline(
                      state.dimension3),
                "Nucleo N5 no activa limites, eficiencia u offline.", failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidatePersistence(List<string> failures)
    {
        GameState state = CreateState("D3 B6 JSON");
        try
        {
            BuildDirect(state.dimension3,
                Dimension3Catalog.FacilityExpeditionPort, 3);
            BuildDirect(state.dimension3,
                Dimension3Catalog.FacilityAutomationCore, 4);
            Dimension3State loaded = JsonUtility.FromJson<Dimension3State>(
                JsonUtility.ToJson(state.dimension3));
            Check(loaded != null &&
                  D3FacilitySystem.GetFacilityLevel(loaded,
                      Dimension3Catalog.FacilityExpeditionPort) == 3 &&
                  D3FacilitySystem.GetFacilityLevel(loaded,
                      Dimension3Catalog.FacilityAutomationCore) == 4,
                "Puerto o Nucleo no sobreviven JSON.", failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void BuildDirect(
        Dimension3State state, string facilityId, int level)
    {
        D3FacilityState facility = D3FacilitySystem.GetFacility(state, facilityId);
        facility.built = level > 0;
        facility.level = level;
    }

    private static GameState CreateState(string name)
    {
        var go = new GameObject(name) { hideFlags = HideFlags.HideAndDontSave };
        go.SetActive(false);
        GameState state = go.AddComponent<GameState>();
        state.dimension03Unlocked = true;
        state.dimension3 = Dimension3System.CreateInitialState();
        Dimension3System.EnsureState(state);
        return state;
    }

    private static bool Near(double a, double b) =>
        System.Math.Abs(a - b) < 0.000001;

    private static void Check(
        bool condition, string message, List<string> failures)
    {
        if (!condition) failures.Add(message);
    }
}
#endif

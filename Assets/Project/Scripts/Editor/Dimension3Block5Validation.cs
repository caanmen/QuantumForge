#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public static class Dimension3Block5Validation
{
    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Block 5 Core")]
    public static void ValidateBlock5Core()
    {
        var failures = new List<string>();
        ValidateFacilityConstructionAndCapacity(failures);
        ValidatePersistentDiagnosticData(failures);
        if (failures.Count == 0)
            Debug.Log("[D3 Block 5] PASS | instalaciones N1-N5 | capacidad | " +
                "análisis persistente | reservas | recetas manuales");
        else Debug.LogError("[D3 Block 5] FAIL\n- " +
            string.Join("\n- ", failures));
    }

    private static void ValidateFacilityConstructionAndCapacity(
        List<string> failures)
    {
        GameState state = CreateState("D3 B5 Facilities");
        try
        {
            state.LE = 10000000.0;
            state.Traces = 10000.0;
            D3InventorySystem.AddAssemblyCount(state.dimension3, 1, 5L);
            D3InventorySystem.AddAutomatons(state.dimension3, 1,
                Dimension3Catalog.TraitNormal, 4L);
            Check(Dimension3System.TryQueueFacilityUpgrade(state,
                    Dimension3Catalog.FacilityProductionConsole,
                    out string reason),
                "No construye Consola N1: " + reason, failures);
            Dimension3System.Tick(state, 1000.0);
            Check(D3FacilitySystem.GetFacilityLevel(state.dimension3,
                    Dimension3Catalog.FacilityProductionConsole) == 1,
                "La Consola no completa nivel 1.", failures);
            Check(Dimension3System.TrySetFacilityAssignment(state,
                    Dimension3Catalog.FacilityProductionConsole,
                    Dimension3Catalog.ChannelConsoleCapacity, 1,
                    Dimension3Catalog.TraitNormal, 4L, out reason),
                "No asigna autómatas a la Consola: " + reason, failures);
            Dimension3System.Tick(state, 31.0);
            Check(D3FacilitySystem.IsFunctionActive(state.dimension3,
                    Dimension3Catalog.FacilityProductionConsole, 1),
                "La capacidad efectiva no activa la función N1.", failures);
            Check(!D3FacilitySystem.IsFunctionActive(state.dimension3,
                    Dimension3Catalog.FacilityProductionConsole, 2),
                "Una función no comprada quedó activa.", failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidatePersistentDiagnosticData(List<string> failures)
    {
        GameState state = CreateState("D3 B5 Persistence");
        try
        {
            D3DiagnosticSystem.RegisterManualFusionRecipe(state,
                ExperimentalFragmentType.Condensation,
                ExperimentalFragmentType.Confinement,
                ExperimentalCatalystType.Alpha);
            Check(state.dimension3.markedFusionRecipes.Count == 1 &&
                  state.dimension3.markedFusionRecipes[0].manuallyExecuted,
                "La receta ejecutada manualmente no queda registrada.", failures);
            string recipeId = state.dimension3.markedFusionRecipes[0].recipeId;
            Check(D3DiagnosticSystem.TrySetRecipeAutomationMark(
                    state, recipeId, true, out string reason),
                "No permite marcar una receta manual válida: " + reason, failures);

            state.dimension3.diagnosticSettings.autoAnalyzeEnabled = true;
            state.dimension3.diagnosticSettings.leReserve = 1234.0;
            string json = JsonUtility.ToJson(state.dimension3);
            Dimension3State loaded = JsonUtility.FromJson<Dimension3State>(json);
            Check(loaded != null && loaded.diagnosticSettings != null &&
                  loaded.diagnosticSettings.autoAnalyzeEnabled &&
                  loaded.diagnosticSettings.leReserve == 1234.0 &&
                  loaded.markedFusionRecipes.Count == 1 &&
                  loaded.markedFusionRecipes[0].automationMarked,
                "Ajustes o marcas del Diagnóstico no sobreviven JSON.", failures);

            var save = new SaveData
            {
                machineAnalysisNodeId = "node_test",
                machineAnalysisRemainingSeconds = 2.5
            };
            SaveData loadedSave = JsonUtility.FromJson<SaveData>(
                JsonUtility.ToJson(save));
            Check(loadedSave.machineAnalysisNodeId == "node_test" &&
                  loadedSave.machineAnalysisRemainingSeconds == 2.5,
                "El análisis activo de la Máquina no sobrevive JSON.", failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
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

    private static void Check(
        bool condition, string message, List<string> failures)
    {
        if (!condition) failures.Add(message);
    }
}
#endif

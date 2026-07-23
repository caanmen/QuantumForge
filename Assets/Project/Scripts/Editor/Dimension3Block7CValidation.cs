#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public static class Dimension3Block7CValidation
{
    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Block 7C")]
    public static void ValidateBlock7C()
    {
        var failures = new List<string>();
        ValidateN3Configuration(failures);
        ValidateRecipeSafetyAndService(failures);
        ValidateRoutineOfflineAndJson(failures);
        if (failures.Count == 0)
            Debug.Log("[D3 Block 7C] PASS | N3 prioridad/reservas | " +
                "N4 fusión manual marcada | N5 rutina/offline | negativos | JSON");
        else Debug.LogError("[D3 Block 7C] FAIL\n- " +
            string.Join("\n- ", failures));
    }

    private static void ValidateN3Configuration(List<string> failures)
    {
        GameState state = CreateState("D3 B7C N3");
        try
        {
            ActivateFacility(state, Dimension3Catalog.FacilityDiagnosticBank, 3);
            Check(D3DiagnosticSystem.TryConfigure(
                    state, 1, (int)MachineZoneType.FusionSector,
                    5000, 250, out string reason),
                "No guarda prioridad/reservas N3: " + reason, failures);
            Check(state.dimension3.diagnosticSettings.priorityMode == 1 &&
                  state.dimension3.diagnosticSettings.priorityZone ==
                    (int)MachineZoneType.FusionSector &&
                  state.dimension3.diagnosticSettings.leReserve == 5000 &&
                  state.dimension3.diagnosticSettings.tracesReserve == 250,
                "Los controles N3 no quedan persistidos.", failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateRecipeSafetyAndService(List<string> failures)
    {
        GameState state = CreateState("D3 B7C Fusion");
        try
        {
            ActivateFacility(state, Dimension3Catalog.FacilityDiagnosticBank, 4);
            string id = D3DiagnosticSystem.GetRecipeId(
                ExperimentalFragmentType.Condensation,
                ExperimentalFragmentType.Confinement,
                ExperimentalCatalystType.Alpha);
            Check(!D3DiagnosticSystem.TrySetRecipeAutomationMark(
                    state, id, true, out string reason) && reason.Contains("manual"),
                "Permite marcar una receta nunca ejecutada manualmente.", failures);
            D3DiagnosticSystem.RegisterManualFusionRecipe(state,
                ExperimentalFragmentType.Condensation,
                ExperimentalFragmentType.Confinement,
                ExperimentalCatalystType.Alpha);
            Check(D3DiagnosticSystem.TrySetRecipeAutomationMark(
                    state, id, true, out reason),
                "No marca una receta manual válida: " + reason, failures);
            Check(D3FusionService.TryParseRecipeId(id,
                    out ExperimentalFragmentType a,
                    out ExperimentalFragmentType b,
                    out ExperimentalCatalystType catalyst) &&
                  a == ExperimentalFragmentType.Condensation &&
                  b == ExperimentalFragmentType.Confinement &&
                  catalyst == ExperimentalCatalystType.Alpha,
                "El servicio no reconstruye la receta por ID estable.", failures);
            Check(!D3FusionService.TryExecuteMarkedSafeFusion(
                    state, "fusion:3:3:2", out _, out _),
                "El servicio ejecuta una receta no manual/no marcada.", failures);
            int fragmentsBefore = state.GetFragmentCount(
                ExperimentalFragmentType.Condensation) +
                state.GetFragmentCount(ExperimentalFragmentType.Confinement);
            Check(!D3DiagnosticSystem.TryRepeatNextMarkedFusion(
                    state, out reason) &&
                  state.GetFragmentCount(ExperimentalFragmentType.Condensation) +
                  state.GetFragmentCount(ExperimentalFragmentType.Confinement) ==
                    fragmentsBefore,
                "La autofusión atraviesa una condición segura o consume al fallar.",
                failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateRoutineOfflineAndJson(List<string> failures)
    {
        GameState state = CreateState("D3 B7C N5");
        try
        {
            ActivateFacility(state, Dimension3Catalog.FacilityDiagnosticBank, 5);
            state.dimension3.diagnosticSettings.autoAnalyzeEnabled = true;
            state.dimension3.diagnosticSettings.autoRepairEnabled = true;
            state.dimension3.diagnosticSettings.autoFusionEnabled = true;
            state.dimension3.diagnosticSettings.priorityMode = 1;
            state.dimension3.diagnosticSettings.priorityZone = 2;
            state.dimension3.diagnosticSettings.leReserve = 1234;
            state.dimension3.diagnosticSettings.tracesReserve = 321;
            D3DiagnosticSystem.RegisterManualFusionRecipe(state,
                ExperimentalFragmentType.Condensation,
                ExperimentalFragmentType.Condensation,
                ExperimentalCatalystType.Beta);
            string id = state.dimension3.markedFusionRecipes[0].recipeId;
            D3DiagnosticSystem.TrySetRecipeAutomationMark(state, id, true, out _);
            Check(D3DiagnosticSystem.TrySaveRoutine(state, out string reason),
                "No guarda la rutina N5: " + reason, failures);
            state.dimension3.diagnosticSettings.autoFusionEnabled = false;
            state.dimension3.diagnosticSettings.leReserve = 0;
            state.dimension3.markedFusionRecipes[0].automationMarked = false;
            Check(D3DiagnosticSystem.TryLoadRoutine(state, out reason) &&
                  state.dimension3.diagnosticSettings.autoFusionEnabled &&
                  state.dimension3.diagnosticSettings.leReserve == 1234 &&
                  state.dimension3.markedFusionRecipes[0].automationMarked,
                "La rutina N5 no restaura toda la configuración: " + reason,
                failures);
            Check(!D3DiagnosticSystem.CanRunOffline(state),
                "Diagnóstico corre offline sin Núcleo N5.", failures);
            ActivateFacility(state, Dimension3Catalog.FacilityAutomationCore, 5);
            Check(D3DiagnosticSystem.CanRunOffline(state),
                "Diagnóstico N5 + Núcleo N5 no habilitan offline.", failures);
            string json = JsonUtility.ToJson(state.dimension3);
            Dimension3State loaded = JsonUtility.FromJson<Dimension3State>(json);
            D3DiagnosticSystem.NormalizeSettings(loaded);
            Check(loaded != null && loaded.progressVersion ==
                    Dimension3System.ProgressVersion &&
                  loaded.diagnosticSettings.savedRoutine.saved &&
                  loaded.diagnosticSettings.savedRoutine.markedRecipeIds.Count == 1 &&
                  loaded.diagnosticSettings.autoFusionEnabled,
                "La rutina diagnóstica completa no sobrevive JSON.", failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ActivateFacility(
        GameState state, string facilityId, int level)
    {
        D3FacilityState facility = D3FacilitySystem.GetFacility(
            state.dimension3, facilityId);
        facility.built = true;
        facility.level = level;
        string channel = facilityId == Dimension3Catalog.FacilityAutomationCore
            ? Dimension3Catalog.ChannelCoreCoordination
            : Dimension3Catalog.ChannelDiagnosticCapacity;
        state.dimension3.assignments.Add(new D3AssignmentState
        {
            installationId = facilityId,
            channelId = channel,
            mk = 6,
            traitId = Dimension3Catalog.TraitNormal,
            amount = 100000,
            stabilizedAmount = 100000
        });
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

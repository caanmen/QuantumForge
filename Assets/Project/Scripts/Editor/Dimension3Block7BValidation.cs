#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Dimension3Block7BValidation
{
    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Block 7B")]
    public static void ValidateBlock7B()
    {
        var failures = new List<string>();
        ValidateCatalogAndManualGuard(failures);
        ValidatePolicyDiscountAndReserve(failures);
        ValidatePhaseAndTriangle(failures);
        ValidateProfilesOfflineAndJson(failures);
        if (failures.Count == 0)
            Debug.Log("[D3 Block 7B] PASS | N1 compras | N2 políticas/reservas | " +
                "N3 bloqueado | N4 fase | N5 Triángulo/perfiles/offline | JSON");
        else Debug.LogError("[D3 Block 7B] FAIL\n- " + string.Join("\n- ", failures));
    }

    private static void ValidateCatalogAndManualGuard(List<string> failures)
    {
        Check(D3AutomationCatalog.GetAction(
                D3AutomationCatalog.ActionConsoleBuyHiggs).status ==
              D3AutomationCatalogStatus.Authorized,
            "Higgs no quedó autorizado.", failures);
        Check(D3AutomationCatalog.GetAction(
                D3AutomationCatalog.ActionConsoleBasicUpgrade).status ==
              D3AutomationCatalogStatus.PendingDesign,
            "N3 no permanece bloqueado por diseño.", failures);
        Check(D3AutomationCatalog.GetAction(
                D3AutomationCatalog.ActionConsoleBuyModulator).status ==
              D3AutomationCatalogStatus.Prohibited,
            "La compra única del Modulador no quedó prohibida.", failures);

        GameState state = CreateState("D3 B7B Manual", 1);
        try
        {
            AddBuilding(state, D3ConsoleSystem.BuildingHiggs, 100.0, 2.0, 0);
            Check(!D3AutomationSystem.TryCreateRoutine(state,
                    D3AutomationCatalog.ActionConsoleBuyHiggs, "", null, 0,
                    0, 0, "", 0, 1, out _, out string reason) &&
                  reason.Contains("manual"),
                "Permite comprar Higgs sin historial manual.", failures);
            D3ConsoleSystem.RecordManualBuildingPurchase(
                state, D3ConsoleSystem.BuildingHiggs);
            Check(D3AutomationSystem.TryCreateRoutine(state,
                    D3AutomationCatalog.ActionConsoleBuyHiggs, "", null, 0,
                    0, 0, "", 0, 1,
                    out D3AutomationRoutineState routine, out reason) &&
                  D3AutomationSystem.TrySetRoutineEnabled(
                    state, routine.routineId, true, out reason),
                "No crea/activa compra autorizada: " + reason, failures);
            state.LE = 1000.0;
            Dimension3System.Tick(state, 1.1);
            Check(state.GetBuildingLevel(D3ConsoleSystem.BuildingHiggs) == 1 &&
                  routine.executionsCompleted == 1 && !routine.enabled,
                "La compra autorizada no se ejecuta una vez.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidatePolicyDiscountAndReserve(List<string> failures)
    {
        GameState state = CreateState("D3 B7B Policy", 2);
        try
        {
            BuildingState higgs = AddBuilding(
                state, D3ConsoleSystem.BuildingHiggs, 1000.0, 2.0, 1);
            AddBuilding(state, D3ConsoleSystem.BuildingTetraquark, 1000.0, 2.0, 1);
            D3ConsoleSystem.RecordManualBuildingPurchase(state,
                D3ConsoleSystem.BuildingHiggs);
            state.dimension3.assignments.Add(new D3AssignmentState
            {
                installationId = Dimension3Catalog.FacilityProductionConsole,
                channelId = Dimension3Catalog.ChannelConsoleCost,
                mk = 6, traitId = Dimension3Catalog.TraitEfficient,
                amount = 100, stabilizedAmount = 100
            });
            Check(D3ConsoleSystem.TrySetPolicyAndReserves(state,
                    D3ConsoleSystem.PolicyLE, 500.0, 20.0, out string reason),
                "No guarda política y reservas N2: " + reason, failures);
            double automaticCost = D3ConsoleSystem.GetAutomatedBuildingCost(state, higgs);
            Check(automaticCost < state.GetEffectiveBuildingCost(higgs),
                "Disciplina no reduce solo el costo automático.", failures);
            state.LE = automaticCost + 499.0;
            state.Traces = 20.0;
            Check(!D3ConsoleSystem.TryPurchaseRepeatableBuilding(state,
                    D3ConsoleSystem.BuildingHiggs, 0, 0, out reason) &&
                  state.GetBuildingLevel(D3ConsoleSystem.BuildingHiggs) == 1,
                "La compra atraviesa la reserva LE.", failures);
            state.LE += 2.0;
            Check(D3ConsoleSystem.TryPurchaseRepeatableBuilding(state,
                    D3ConsoleSystem.BuildingHiggs, 0, 0, out reason) &&
                  state.LE + 0.0001 >= 500.0,
                "La compra válida no conserva la reserva: " + reason, failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidatePhaseAndTriangle(List<string> failures)
    {
        GameState state = CreateState("D3 B7B Presets", 5);
        try
        {
            AddBuilding(state, D3ConsoleSystem.BuildingHiggs, 10, 2, 1);
            AddBuilding(state, D3ConsoleSystem.BuildingTetraquark, 10, 2, 1);
            AddBuilding(state, "fluctuation_antenna", 10, 2, 1);
            state.triangleSystemUnlocked = true;
            state.SetPhaseModulatorMode(PhaseModulatorMode.Expansion);
            D3ConsoleSystem.RecordManualModulatorMode(
                state, PhaseModulatorMode.Expansion);
            state.SetPhaseModulatorMode(PhaseModulatorMode.Conservation);
            Check(D3ConsoleSystem.TryApplyPreferredPhase(
                    state, PhaseModulatorMode.Expansion, out string reason) &&
                  state.phaseModulatorMode == PhaseModulatorMode.Expansion,
                "N4 no mantiene fase manual autorizada: " + reason, failures);

            state.AssignTriangleBuilding(TriangleSlotRole.Primary,
                D3ConsoleSystem.BuildingHiggs);
            state.AssignTriangleBuilding(TriangleSlotRole.Reinforcement,
                D3ConsoleSystem.BuildingTetraquark);
            state.AssignTriangleBuilding(TriangleSlotRole.Alteration,
                "fluctuation_antenna");
            D3TrianglePresetState preset =
                state.dimension3.consoleSettings.manualTrianglePresets[0];
            state.ClearTriangleConfiguration();
            state.AssignTriangleBuilding(TriangleSlotRole.Primary,
                "fluctuation_antenna", false);
            Check(D3ConsoleSystem.TryApplyTrianglePreset(
                    state, preset.presetId, out reason) &&
                  D3ConsoleSystem.IsTrianglePresetApplied(state, preset),
                "N5 no aplica preset manual básico: " + reason, failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateProfilesOfflineAndJson(List<string> failures)
    {
        GameState state = CreateState("D3 B7B Offline", 5);
        try
        {
            ActivateCore(state, 5);
            AddBuilding(state, D3ConsoleSystem.BuildingHiggs, 100, 2, 1);
            D3ConsoleSystem.RecordManualBuildingPurchase(
                state, D3ConsoleSystem.BuildingHiggs);
            D3ConsoleSystem.TrySetPolicyAndReserves(state,
                D3ConsoleSystem.PolicyLE, 50, 10, out _);
            state.LE = 10000;
            state.Traces = 100;
            Check(D3AutomationSystem.TryCreateRoutine(state,
                    D3AutomationCatalog.ActionConsoleBuyHiggs, "", null, 2,
                    0, 0, "", 0, 1,
                    out D3AutomationRoutineState routine, out string reason) &&
                  D3AutomationSystem.TrySetRoutineEnabled(
                    state, routine.routineId, true, out reason),
                "No prepara rutina offline de Consola: " + reason, failures);
            Check(D3AutomationSystem.CanRunAutomationOffline(state),
                "Consola N5 + Núcleo N5 no habilitan offline.", failures);
            Check(D3AutomationSystem.TrySaveProfile(
                    state, "profile_console", "Consola", out reason),
                "No guarda perfil de Consola: " + reason, failures);
            state.dimension3.consoleSettings.purchasePolicy =
                D3ConsoleSystem.PolicyTraces;
            Check(D3AutomationSystem.TryLoadProfile(
                    state, "profile_console", out reason) &&
                  state.dimension3.consoleSettings.purchasePolicy ==
                    D3ConsoleSystem.PolicyLE,
                "El perfil no restaura política de Consola: " + reason, failures);
            D3AutomationSystem.ApplyOfflineExternal(state, 60.0);
            Check(state.GetBuildingLevel(D3ConsoleSystem.BuildingHiggs) == 2,
                "La compra autorizada no funciona offline.", failures);
            string json = JsonUtility.ToJson(state.dimension3);
            Dimension3State loaded = JsonUtility.FromJson<Dimension3State>(json);
            Check(loaded != null && loaded.consoleSettings != null &&
                  loaded.consoleSettings.manuallyPurchasedBuildingIds.Contains(
                    D3ConsoleSystem.BuildingHiggs) &&
                  loaded.automationProfiles.Count == 1 &&
                  loaded.automationProfiles[0].savedConsoleSettings != null,
                "Historial, ajustes o perfil no sobreviven JSON.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static GameState CreateState(string name, int consoleLevel)
    {
        var go = new GameObject(name) { hideFlags = HideFlags.HideAndDontSave };
        go.SetActive(false);
        GameState state = go.AddComponent<GameState>();
        state.dimension03Unlocked = true;
        state.dimension3 = Dimension3System.CreateInitialState();
        Dimension3System.EnsureState(state);
        D3FacilityState console = D3FacilitySystem.GetFacility(
            state.dimension3, Dimension3Catalog.FacilityProductionConsole);
        console.built = true;
        console.level = consoleLevel;
        state.dimension3.assignments.Add(new D3AssignmentState
        {
            installationId = Dimension3Catalog.FacilityProductionConsole,
            channelId = Dimension3Catalog.ChannelConsoleCapacity,
            mk = 6, traitId = Dimension3Catalog.TraitNormal,
            amount = 5, stabilizedAmount = 5
        });
        return state;
    }

    private static BuildingState AddBuilding(
        GameState state, string id, double cost, double multiplier, int level)
    {
        var definition = new BuildingDef
            { id = id, baseCost = cost, costMult = multiplier };
        var building = new BuildingState { level = level };
        building.InitFromDef(definition);
        state.RegisterBuildingState(building);
        return building;
    }

    private static void ActivateCore(GameState state, int level)
    {
        D3FacilityState core = D3FacilitySystem.GetFacility(
            state.dimension3, Dimension3Catalog.FacilityAutomationCore);
        core.built = true;
        core.level = level;
        state.dimension3.assignments.Add(new D3AssignmentState
        {
            installationId = Dimension3Catalog.FacilityAutomationCore,
            channelId = Dimension3Catalog.ChannelCoreCoordination,
            mk = 6, traitId = Dimension3Catalog.TraitCoordinator,
            amount = 4, stabilizedAmount = 4
        });
    }

    private static void Check(bool condition, string message, List<string> failures)
    {
        if (!condition) failures.Add(message);
    }
}
#endif

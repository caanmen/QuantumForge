#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class F2ProgressionMigrationValidation
{
    [MenuItem("Quantum Forge/Validation/F2 Progression and Migration")]
    public static void Validate()
    {
        var failures = new List<string>();
        ValidateCatalog(failures);
        ValidateRuntimeRules(failures);
        if (failures.Count > 0) throw new InvalidOperationException("F2 progression FAIL\n- " + string.Join("\n- ", failures));
        Debug.Log("[F2ProgressionMigrationValidation] PASS · 7 mejoras · gating · 0/50/65/80 · migración idempotente.");
    }

    private static void ValidateCatalog(List<string> failures)
    {
        TextAsset json = Resources.Load<TextAsset>("Data/f2_upgrades");
        var list = json != null ? JsonUtility.FromJson<F2UpgradeDefList>(json.text) : null;
        Check(list?.upgrades != null, "No carga f2_upgrades.json.", failures);
        if (list?.upgrades == null) return;
        Check(list.upgrades.Count(x => !x.retired) == 7, "El catálogo público no contiene siete mejoras.", failures);
        Check(Find(list, "residual_analysis")?.retired == true && Find(list, "pattern_mapping")?.retired == true, "Residual/Patrones no están retiradas.", failures);
        Check(Find(list, "emission_focus")?.tiers?.Select(x => x.effectValue).SequenceEqual(new[] { 0.05, 0.15 }) == true, "Emisión no usa totales 5/15%.", failures);
        Check(Find(list, "containment_tuning")?.tiers?.Select(x => x.effectValue).SequenceEqual(new[] { 0.90, 0.80 }) == true, "Contención no usa ciclos 0.9/0.8 s.", failures);
        Check(Find(list, "tetraquark_stabilization")?.currency == F2UpgradeCurrency.LE, "Lectura Tetraquark no cuesta LE.", failures);
        Check(Find(list, "triangle_unlock_1")?.requiresAllTriangleVertices == true, "Acople no exige los tres vértices.", failures);
        Check(Find(list, "triangle_persistence_anchor")?.tiers?.Select(x => x.effectValue).SequenceEqual(new[] { 0.65, 0.80 }) == true, "Memoria no usa 65/80%.", failures);

        string visibilitySource = File.ReadAllText(
            "Assets/Project/Scripts/UI/F2UpgradeVisibilityController.cs");
        Check(visibilitySource.Contains("DontDestroyOnLoad"),
            "El controlador de visibilidad F2 no persiste al recargar Main/RESET.", failures);
    }

    private static F2UpgradeDef Find(F2UpgradeDefList list, string id) => list.upgrades.FirstOrDefault(x => x.id == id);

    private static void ValidateRuntimeRules(List<string> failures)
    {
        GameObject stateGo = new GameObject("F2 validation state") { hideFlags = HideFlags.HideAndDontSave };
        GameObject managerGo = new GameObject("F2 validation manager") { hideFlags = HideFlags.HideAndDontSave };
        GameState state = stateGo.AddComponent<GameState>();
        F2UpgradeManager manager = managerGo.AddComponent<F2UpgradeManager>();
        SetSingleton(typeof(GameState), state);
        SetSingleton(typeof(F2UpgradeManager), manager);
        typeof(F2UpgradeManager).GetMethod("LoadDefs", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(manager, null);
        try
        {
            BuildingState higgs = AddBuilding(state, "vacuum_observer", 0);
            BuildingState tetra = AddBuilding(state, "casimir_panel", 0);
            BuildingState mod = AddBuilding(state, "fluctuation_antenna", 0);
            state.LE = 100000;
            state.Traces = 100000;
            Check(!manager.CanBuy("tetraquark_stabilization"), "Lectura se compra sin Tetraquark.", failures);
            Check(!manager.TryBuy("triangle_unlock_1"), "Acople se compra sin vértices.", failures);
            Check(!manager.TryBuy("triangle_impulse_tuning"), "Circuito se compra antes de Acople.", failures);
            higgs.level = 1;
            Check(manager.ShouldBeVisible("emission_focus") && manager.ShouldBeVisible("containment_tuning"), "Fundamentos no aparecen con Higgs.", failures);
            tetra.level = 1;
            Check(manager.ShouldBeVisible("tetraquark_stabilization"), "Lectura no aparece con Tetraquark.", failures);
            mod.level = 1;
            Check(manager.ShouldBeVisible("triangle_unlock_1") && manager.TryBuy("triangle_unlock_1"), "Acople no se activa con los tres vértices.", failures);
            Check(state.triangleActiveCircuit == TriangleCircuitType.None && Mathf.Approximately(state.triangleSynchronization, 0f), "Acople selecciona o sincroniza un circuito oculto.", failures);
            Check(state.SetTriangleCircuit(TriangleCircuitType.Energy, false) && Mathf.Approximately(state.triangleSynchronization, 0f), "Primera elección no parte de 0%.", failures);
            state.triangleSynchronization = 1f;
            state.SetTriangleCircuit(TriangleCircuitType.Experimental, false);
            Check(Mathf.Approximately(state.triangleSynchronization, 0.50f), "Cambio base no parte de 50%.", failures);
            Check(Math.Abs(state.GetTriangleSynchronizationRemainingSeconds() - 90.0) < 0.001,
                "El cambio base no informa 90 s restantes reales.", failures);

            Check(manager.TryBuy("triangle_persistence_anchor"),
                "No se pudo comprar Memoria nivel 1 tras Acople.", failures);
            state.triangleSynchronization = 1f;
            state.SetTriangleCircuit(TriangleCircuitType.Energy, false);
            Check(Mathf.Approximately(state.triangleSynchronization, 0.65f),
                "Memoria nivel 1 no inicia al 65%.", failures);
            Check(Math.Abs(state.GetTriangleSynchronizationRemainingSeconds() - 90.0) < 0.001,
                "Memoria nivel 1 muestra un tiempo restante distinto de 90 s.", failures);

            Check(manager.TryBuy("triangle_persistence_anchor"),
                "No se pudo comprar Memoria nivel 2 tras Acople.", failures);
            state.triangleSynchronization = 1f;
            state.SetTriangleCircuit(TriangleCircuitType.Experimental, false);
            Check(Mathf.Approximately(state.triangleSynchronization, 0.80f),
                "Memoria nivel 2 no inicia al 80%.", failures);
            Check(Math.Abs(state.GetTriangleSynchronizationRemainingSeconds() - 90.0) < 0.001,
                "Memoria nivel 2 muestra un tiempo restante distinto de 90 s.", failures);

            manager.ApplyLoadedPurchasedTiers(new List<SavedF2UpgradeTier>
            {
                new() { id = "emission_focus", purchasedTiers = 3 },
                new() { id = "pattern_mapping", purchasedTiers = 2 },
                new() { id = "tetraquark_stabilization", purchasedTiers = 0 },
                new() { id = "residual_analysis", purchasedTiers = 2 },
                new() { id = "triangle_unlock_1", purchasedTiers = 1 },
                new() { id = "triangle_persistence_anchor", purchasedTiers = 1 }
            }, 0);
            Check(manager.GetPurchasedTierCount("emission_focus") == 2 && manager.GetPurchasedTierCount("tetraquark_stabilization") == 2, "Migración no fusiona o limita tiers.", failures);
            Check(manager.GetPurchasedTierCount("pattern_mapping") == 0 && manager.GetPurchasedTierCount("residual_analysis") == 0, "IDs retirados siguen activos.", failures);
            Check(Mathf.Approximately(manager.GetTriangleSwitchSynchronization(), 0.65f), "Memoria nivel 1 no conserva 65%.", failures);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(managerGo);
            UnityEngine.Object.DestroyImmediate(stateGo);
            SetSingleton(typeof(F2UpgradeManager), null);
            SetSingleton(typeof(GameState), null);
        }
    }

    private static void SetSingleton(Type type, UnityEngine.Object value) =>
        type.GetProperty("I", BindingFlags.Public | BindingFlags.Static)?.SetValue(null, value);

    private static BuildingState AddBuilding(GameState state, string id, int level)
    {
        var building = new BuildingState { level = level };
        building.InitFromDef(new BuildingDef { id = id, baseCost = 10, costMult = 2, tickInterval = 1, lePerTickBase = 1 });
        building.level = level;
        state.RegisterBuildingState(building);
        return building;
    }

    private static void Check(bool condition, string message, List<string> failures) { if (!condition) failures.Add(message); }
}
#endif

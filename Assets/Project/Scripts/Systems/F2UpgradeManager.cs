using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedF2UpgradeTier
{
    public string id;
    public int purchasedTiers;
}

public enum F2UpgradeLockReason
{
    None,
    Hidden,
    MissingBuilding,
    MissingUpgrade,
    MissingVertices,
    TriangleLocked,
    ResearchRequired,
    InsufficientFunds,
    Maxed
}

public class F2UpgradeManager : MonoBehaviour
{
    public const int ProgressionMigrationVersion = 1;
    public static F2UpgradeManager I { get; private set; }

    private readonly Dictionary<string, F2UpgradeDef> _defsById = new();
    private readonly Dictionary<string, int> _purchasedTiers = new();

    public int GetTriangleImpulseTuningTier() => GetPurchasedTierCount("triangle_impulse_tuning");
    public int GetTriangleSynergyResonanceTier() => GetPurchasedTierCount("triangle_synergy_resonance");
    public int GetTrianglePersistenceAnchorTier() => GetPurchasedTierCount("triangle_persistence_anchor");
    public int GetTriangleEnergyEfficiencyTier() => GetPurchasedTierCount("triangle_energy_efficiency");

    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        LoadDefs();
    }

    private void LoadDefs()
    {
        _defsById.Clear();
        TextAsset json = Resources.Load<TextAsset>("Data/f2_upgrades");
        if (json == null) { Debug.LogWarning("[F2UpgradeManager] Falta Data/f2_upgrades.json"); return; }
        var data = JsonUtility.FromJson<F2UpgradeDefList>(json.text);
        if (data?.upgrades == null) { Debug.LogWarning("[F2UpgradeManager] JSON inválido."); return; }
        foreach (var def in data.upgrades)
        {
            if (def == null || string.IsNullOrWhiteSpace(def.id)) continue;
            _defsById[def.id] = def;
            if (!_purchasedTiers.ContainsKey(def.id)) _purchasedTiers[def.id] = 0;
        }
    }

    public F2UpgradeDef GetDef(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        _defsById.TryGetValue(id, out var def);
        return def;
    }

    public int GetPurchasedTierCount(string id) =>
        !string.IsNullOrWhiteSpace(id) && _purchasedTiers.TryGetValue(id, out int value) ? value : 0;

    public bool IsMaxed(string id)
    {
        var def = GetDef(id);
        return def?.tiers == null || GetPurchasedTierCount(id) >= def.tiers.Count;
    }

    public double GetNextCost(string id)
    {
        var def = GetDef(id);
        int tier = GetPurchasedTierCount(id);
        return def?.tiers != null && tier >= 0 && tier < def.tiers.Count ? def.tiers[tier].cost : -1.0;
    }

    public bool MeetsPrerequisites(string id)
    {
        var def = GetDef(id);
        var state = GameState.I;
        if (def == null || def.retired || state == null) return false;
        if (!string.IsNullOrEmpty(def.requiredBuildingId) &&
            state.GetBuildingLevel(def.requiredBuildingId) < Mathf.Max(1, def.requiredBuildingLevel)) return false;
        if (!string.IsNullOrEmpty(def.requiredUpgradeId) &&
            GetPurchasedTierCount(def.requiredUpgradeId) < Mathf.Max(1, def.requiredUpgradeTier)) return false;
        if (def.requiresAllTriangleVertices && !state.HasAllTriangleVertices()) return false;
        if (def.requiresTriangleUnlocked && !state.CanUseTriangleCircuits()) return false;
        return true;
    }

    public bool ShouldBeVisible(string id)
    {
        var def = GetDef(id);
        GameState state = GameState.I;
        if (def == null || def.retired || state == null) return false;
        if (GetPurchasedTierCount(id) > 0) return true;
        if (UpgradeStudySystem.IsDiscovered(state, id))
            return MeetsPrerequisites(id);
        return UpgradeStudySystem.ShouldShowStudyOpportunity(state, id);
    }

    public F2UpgradeLockReason GetLockReason(string id)
    {
        var def = GetDef(id);
        var state = GameState.I;
        if (def == null || def.retired || state == null) return F2UpgradeLockReason.Hidden;
        if (IsMaxed(id)) return F2UpgradeLockReason.Maxed;
        if (!UpgradeStudySystem.IsDiscovered(state, id))
            return F2UpgradeLockReason.ResearchRequired;
        if (!string.IsNullOrEmpty(def.requiredBuildingId) && state.GetBuildingLevel(def.requiredBuildingId) < Mathf.Max(1, def.requiredBuildingLevel)) return F2UpgradeLockReason.MissingBuilding;
        if (!string.IsNullOrEmpty(def.requiredUpgradeId) && GetPurchasedTierCount(def.requiredUpgradeId) < Mathf.Max(1, def.requiredUpgradeTier)) return F2UpgradeLockReason.MissingUpgrade;
        if (def.requiresAllTriangleVertices && !state.HasAllTriangleVertices()) return F2UpgradeLockReason.MissingVertices;
        if (def.requiresTriangleUnlocked && !state.CanUseTriangleCircuits()) return F2UpgradeLockReason.TriangleLocked;
        double cost = GetNextCost(id);
        bool affordable = def.currency == F2UpgradeCurrency.LE ? state.LE >= cost : state.Traces >= cost;
        return affordable ? F2UpgradeLockReason.None : F2UpgradeLockReason.InsufficientFunds;
    }

    public bool CanBuy(string id) => GetLockReason(id) == F2UpgradeLockReason.None;

    public bool TryBuy(string id)
    {
        if (!CanBuy(id)) return false;
        var def = GetDef(id);
        double cost = GetNextCost(id);
        if (def.currency == F2UpgradeCurrency.LE) GameState.I.LE -= cost;
        else if (def.currency == F2UpgradeCurrency.Traces) GameState.I.Traces -= cost;
        else return false;
        _purchasedTiers[id] = GetPurchasedTierCount(id) + 1;
        if (id == "triangle_unlock_1")
        {
            GameState.I.triangleSystemUnlocked = true;
            GameState.I.PrepareNewTriangleActivation();
        }
        return true;
    }

    public List<SavedF2UpgradeTier> GetPurchasedTiersForSave()
    {
        var list = new List<SavedF2UpgradeTier>();
        foreach (var kv in _purchasedTiers) list.Add(new SavedF2UpgradeTier { id = kv.Key, purchasedTiers = kv.Value });
        return list;
    }

    public void ApplyLoadedPurchasedTiers(List<SavedF2UpgradeTier> loaded, int migrationVersion = 0)
    {
        bool legacyTriangleUnlocked = GameState.I != null && GameState.I.triangleSystemUnlocked;
        foreach (var key in new List<string>(_purchasedTiers.Keys)) _purchasedTiers[key] = 0;
        if (loaded != null)
        {
            foreach (var item in loaded)
            {
                var def = item == null ? null : GetDef(item.id);
                if (def?.tiers == null) continue;
                _purchasedTiers[item.id] = Mathf.Clamp(item.purchasedTiers, 0, def.tiers.Count);
            }
        }
        if (migrationVersion < ProgressionMigrationVersion)
        {
            MergeRetiredTier("residual_analysis", "tetraquark_stabilization");
            MergeRetiredTier("pattern_mapping", "emission_focus");
        }
        _purchasedTiers["residual_analysis"] = 0;
        _purchasedTiers["pattern_mapping"] = 0;
        if (legacyTriangleUnlocked && GetDef("triangle_unlock_1")?.tiers?.Count > 0)
            _purchasedTiers["triangle_unlock_1"] = 1;
        if (GameState.I != null)
        {
            GameState.I.triangleSystemUnlocked = GetPurchasedTierCount("triangle_unlock_1") > 0;
            GameState.I.SanitizeTriangleCircuit(false);
        }
    }

    private void MergeRetiredTier(string retiredId, string targetId)
    {
        int retired = GetPurchasedTierCount(retiredId);
        var target = GetDef(targetId);
        if (retired <= 0 || target?.tiers == null) return;
        _purchasedTiers[targetId] = Mathf.Clamp(Mathf.Max(GetPurchasedTierCount(targetId), retired), 0, target.tiers.Count);
    }

    private double GetCurrentTotalValue(string id, double fallback = 0.0)
    {
        var def = GetDef(id);
        int bought = GetPurchasedTierCount(id);
        return def?.tiers != null && bought > 0 && bought <= def.tiers.Count ? def.tiers[bought - 1].effectValue : fallback;
    }

    public double GetTotalGlobalLEMultBonus() => GetCurrentTotalValue("emission_focus");
    public double GetContainmentCycleMultiplier() => GetCurrentTotalValue("containment_tuning", 1.0);
    public double GetTetraquarkTraceBonus() => GetCurrentTotalValue("tetraquark_stabilization");
    public double GetTriangleEnergyLEBonus(double baseValue) =>
        GetCurrentTotalValue("triangle_impulse_tuning", baseValue);
    public double GetTriangleExperimentalTraceBonus(double baseValue) =>
        GetCurrentTotalValue("triangle_synergy_resonance", baseValue);
    public double GetTriangleEnergyProductionBonus() =>
        GetCurrentTotalValue("triangle_energy_efficiency");
    public double GetContainmentTuningBonus() => 0.0;
    public double GetTetraquarkStabilizationBonus() => 0.0;
    public double GetResidualAnalysisBonus() => GetTetraquarkTraceBonus();
    public double GetPatternMappingBonus() => 0.0;
    public double GetCycleCompressionBonus() => 0.0;
    public float GetTriangleSwitchSynchronization()
    {
        int tier = GetTrianglePersistenceAnchorTier();
        return tier >= 2 ? 0.80f : tier == 1 ? 0.65f : 0.50f;
    }

    public void DebugResetAllPurchases()
    {
        foreach (var key in new List<string>(_purchasedTiers.Keys)) _purchasedTiers[key] = 0;
        if (GameState.I != null)
        {
            GameState.I.triangleSystemUnlocked = false;
            GameState.I.triangleActiveCircuit = TriangleCircuitType.None;
            GameState.I.triangleSynchronization = 0f;
        }
        TabsUI tabs = FindFirstObjectByType<TabsUI>(FindObjectsInactive.Include);
        if (tabs != null) tabs.ShowGeneracion();
    }
}

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedF2UpgradeTier
{
    public string id;
    public int purchasedTiers;
}
public class F2UpgradeManager : MonoBehaviour
{
    public static F2UpgradeManager I { get; private set; }

    private readonly Dictionary<string, F2UpgradeDef> _defsById = new();
    private readonly Dictionary<string, int> _purchasedTiers = new();
    public int GetTriangleImpulseTuningTier()
    {
        return GetPurchasedTierCount("triangle_impulse_tuning");
    }

    public int GetTriangleSynergyResonanceTier()
    {
        return GetPurchasedTierCount("triangle_synergy_resonance");
    }

    public int GetTrianglePersistenceAnchorTier()
    {
        return GetPurchasedTierCount("triangle_persistence_anchor");
    }

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);
        LoadDefs();
    }

    private void LoadDefs()
    {
        _defsById.Clear();

        TextAsset json = Resources.Load<TextAsset>("Data/f2_upgrades");
        if (json == null)
        {
            Debug.LogWarning("[F2UpgradeManager] No se encontró Resources/Data/f2_upgrades.json");
            return;
        }

        var data = JsonUtility.FromJson<F2UpgradeDefList>(json.text);
        if (data == null || data.upgrades == null)
        {
            Debug.LogWarning("[F2UpgradeManager] JSON inválido o vacío.");
            return;
        }

        foreach (var def in data.upgrades)
        {
            if (def == null || string.IsNullOrWhiteSpace(def.id))
                continue;

            _defsById[def.id] = def;

            if (!_purchasedTiers.ContainsKey(def.id))
                _purchasedTiers[def.id] = 0;
        }

        Debug.Log($"[F2UpgradeManager] Cargadas {_defsById.Count} mejora(s) F2.");
    }

    public F2UpgradeDef GetDef(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        _defsById.TryGetValue(id, out var def);
        return def;
    }

    public int GetPurchasedTierCount(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return 0;
        return _purchasedTiers.TryGetValue(id, out int value) ? value : 0;
    }

    public bool IsMaxed(string id)
    {
        var def = GetDef(id);
        if (def == null || def.tiers == null) return true;

        return GetPurchasedTierCount(id) >= def.tiers.Count;
    }

    public double GetNextCost(string id)
    {
        var def = GetDef(id);
        if (def == null || def.tiers == null) return -1.0;

        int tierIndex = GetPurchasedTierCount(id);
        if (tierIndex < 0 || tierIndex >= def.tiers.Count) return -1.0;

        return def.tiers[tierIndex].cost;
    }

    public bool CanBuy(string id)
    {
        var def = GetDef(id);
        if (def == null || IsMaxed(id) || GameState.I == null) return false;

        double cost = GetNextCost(id);

        return def.currency switch
        {
            F2UpgradeCurrency.LE => GameState.I.LE >= cost,
            F2UpgradeCurrency.Traces => GameState.I.Traces >= cost,
            _ => false
        };
    }

    public bool TryBuy(string id)
    {
        var def = GetDef(id);
        if (def == null || IsMaxed(id) || GameState.I == null) return false;

        double cost = GetNextCost(id);
        if (cost < 0.0) return false;

        switch (def.currency)
        {
            case F2UpgradeCurrency.LE:
                if (GameState.I.LE < cost) return false;
                GameState.I.LE -= cost;
                break;

            case F2UpgradeCurrency.Traces:
                if (GameState.I.Traces < cost) return false;
                GameState.I.Traces -= cost;
                break;

            default:
                return false;
        }

        _purchasedTiers[id] = GetPurchasedTierCount(id) + 1;

        if (GameState.I != null)
        {
            GameState.I.triangleSystemUnlocked = GetPurchasedTierCount("triangle_unlock_1") > 0;
        }

        return true;
        }
        public List<SavedF2UpgradeTier> GetPurchasedTiersForSave()
        {
            var list = new List<SavedF2UpgradeTier>();

            foreach (var kv in _purchasedTiers)
            {
                list.Add(new SavedF2UpgradeTier
                {
                    id = kv.Key,
                    purchasedTiers = kv.Value
                });
            }

            return list;
        }

    public void ApplyLoadedPurchasedTiers(List<SavedF2UpgradeTier> loaded)
    {
        foreach (var key in new List<string>(_purchasedTiers.Keys))
        {
            _purchasedTiers[key] = 0;
        }

        if (loaded == null) return;

        foreach (var item in loaded)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.id))
                continue;

            if (_purchasedTiers.ContainsKey(item.id))
                _purchasedTiers[item.id] = Mathf.Max(0, item.purchasedTiers);
                
        }
            if (GameState.I != null)
{           GameState.I.triangleSystemUnlocked = GetPurchasedTierCount("triangle_unlock_1") > 0;
}
    }

    [ContextMenu("DEBUG: Buy Emission Focus")]
    private void DebugBuyEmissionFocus()
    {
        bool ok = TryBuy("emission_focus");
        Debug.Log($"[F2UpgradeManager] DEBUG Buy emission_focus => {ok}, tiers={GetPurchasedTierCount("emission_focus")}, bonus={GetTotalGlobalLEMultBonus():0.###}");
    }

    public double GetTotalGlobalLEMultBonus()
    {
        double total = 0.0;

        foreach (var kv in _defsById)
        {
            var def = kv.Value;
            if (def == null || def.tiers == null) continue;
            if (def.effectType != F2UpgradeEffectType.GlobalLEMult) continue;

            int bought = GetPurchasedTierCount(def.id);
            for (int i = 0; i < bought && i < def.tiers.Count; i++)
            {
                total += def.tiers[i].effectValue;
            }
        }

        total += GetPatternMappingBonus();

        return total;
    }
        public double GetContainmentTuningBonus()
    {
        var def = GetDef("containment_tuning");
        if (def == null || def.tiers == null) return 0.0;

        int bought = GetPurchasedTierCount("containment_tuning");
        double total = 0.0;

        for (int i = 0; i < bought && i < def.tiers.Count; i++)
        {
            total += def.tiers[i].effectValue;
        }

        return total;
    }

    public double GetTetraquarkStabilizationBonus()
    {
        var def = GetDef("tetraquark_stabilization");
        if (def == null || def.tiers == null) return 0.0;

        int bought = GetPurchasedTierCount("tetraquark_stabilization");
        double total = 0.0;

        for (int i = 0; i < bought && i < def.tiers.Count; i++)
        {
            total += def.tiers[i].effectValue;
        }

        return total;
    }

    public double GetCycleCompressionBonus()
    {
        var def = GetDef("cycle_compression");
        if (def == null || def.tiers == null) return 0.0;

        int bought = GetPurchasedTierCount("cycle_compression");
        double total = 0.0;

        for (int i = 0; i < bought && i < def.tiers.Count; i++)
        {
            total += def.tiers[i].effectValue;
        }

        return total;
    }


    public double GetResidualAnalysisBonus()
    {
        var def = GetDef("residual_analysis");
        if (def == null || def.tiers == null) return 0.0;

        int bought = GetPurchasedTierCount("residual_analysis");
        double total = 0.0;

        for (int i = 0; i < bought && i < def.tiers.Count; i++)
        {
            total += def.tiers[i].effectValue;
        }

        return total;
    }


    public double GetPatternMappingBonus()
    {
        var def = GetDef("pattern_mapping");
        if (def == null || def.tiers == null) return 0.0;

        int bought = GetPurchasedTierCount("pattern_mapping");
        double total = 0.0;

        for (int i = 0; i < bought && i < def.tiers.Count; i++)
        {
            total += def.tiers[i].effectValue;
        }

        int ownedArtifacts = 0;

        if (GameState.I != null)
        {
            if (GameState.I.GetBuildingLevel("vacuum_observer") > 0) ownedArtifacts++;
            if (GameState.I.GetBuildingLevel("casimir_panel") > 0) ownedArtifacts++;
            if (GameState.I.GetBuildingLevel("fluctuation_antenna") > 0) ownedArtifacts++;
        }

        int varietyFactor = Mathf.Max(ownedArtifacts - 1, 0);

        return total * varietyFactor;
    }

    
    public void DebugResetAllPurchases()
    {
        var keys = new List<string>(_purchasedTiers.Keys);
        foreach (var key in keys)
        {
            _purchasedTiers[key] = 0;
        }

        if (GameState.I != null)
        {
            GameState.I.triangleSystemUnlocked = false;
        }

        TabsUI tabsUI = FindFirstObjectByType<TabsUI>(FindObjectsInactive.Include);
        if (tabsUI != null)
        {
            tabsUI.ShowGeneracion();
        }

        Debug.Log("[F2UpgradeManager] DEBUG: compras F2 reseteadas.");
    }
}
using System.Collections.Generic;
using UnityEngine;

public class F2UpgradeManager : MonoBehaviour
{
    public static F2UpgradeManager I { get; private set; }

    private readonly Dictionary<string, F2UpgradeDef> _defsById = new();
    private readonly Dictionary<string, int> _purchasedTiers = new();

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
        return true;
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

        return total;
    }
}
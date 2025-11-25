using System;
using System.Collections.Generic;
using UnityEngine;

public enum ResearchEffectType
{
    None = 0,
    GlobalLEMult = 1,
    EMGenerationMult = 2
}

[Serializable]
public class ResearchDef
{
    public string id;
    public string name;
    public string description;
    public double costIP;
    public string prereqId;
    public ResearchEffectType effectType;
    public double effectValue;
}

[Serializable]
public class ResearchDefList
{
    public List<ResearchDef> research;
}

[Serializable]
public class ResearchState
{
    public string id;
    public bool purchased;
}

public class ResearchManager : MonoBehaviour
{
    public static ResearchManager I { get; private set; }

    [Header("Datos")]
    [Tooltip("Archivo JSON en Resources/Data/research.json")]
    public string jsonPath = "Data/research";

    public List<ResearchDef> defs = new List<ResearchDef>();
    public Dictionary<string, ResearchState> states = new Dictionary<string, ResearchState>();

    // Bonos agregados
    private double totalGlobalLEBonus = 0.0;
    private double totalEMGenBonus = 0.0;

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);

        LoadDefsFromJson();
        InitStatesFromDefs();
        RestorePurchasedFromSave();
        RecalculateBonuses();
    }

    private void LoadDefsFromJson()
    {
        try
        {
            TextAsset asset = Resources.Load<TextAsset>(jsonPath);
            if (asset == null)
            {
                Debug.LogError($"[ResearchManager] No se encontró research.json en Resources/{jsonPath}");
                return;
            }

            var list = JsonUtility.FromJson<ResearchDefList>(asset.text);
            if (list != null && list.research != null)
            {
                defs = list.research;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ResearchManager] Error cargando research.json: {ex.Message}");
        }
    }

    private void InitStatesFromDefs()
    {
        states.Clear();
        foreach (var def in defs)
        {
            if (string.IsNullOrEmpty(def.id)) continue;
            if (!states.ContainsKey(def.id))
            {
                states[def.id] = new ResearchState
                {
                    id = def.id,
                    purchased = false
                };
            }
        }
    }

    private void RestorePurchasedFromSave()
    {
        var saved = SaveService.LastLoadedResearchIds;
        if (saved == null) return;

        foreach (var id in saved)
        {
            if (states.TryGetValue(id, out var st))
            {
                st.purchased = true;
            }
        }
    }

    private void RecalculateBonuses()
    {
        totalGlobalLEBonus = 0.0;
        totalEMGenBonus = 0.0;

        foreach (var def in defs)
        {
            if (!states.TryGetValue(def.id, out var st) || !st.purchased)
                continue;

            switch (def.effectType)
            {
                case ResearchEffectType.GlobalLEMult:
                    totalGlobalLEBonus += def.effectValue;
                    break;
                case ResearchEffectType.EMGenerationMult:
                    totalEMGenBonus += def.effectValue;
                    break;
            }
        }

        // Aplicar al GameState
        if (GameState.I != null)
        {
            GameState.I.researchGlobalLEMult = 1.0 + totalGlobalLEBonus;
        }
    }

    /// <summary>
    /// Factor multiplicador para la generación de EM (1 + bonus).
    /// Lo usará GameState en CalculateEMps().
    /// </summary>
    public double GetEMGenerationFactor()
    {
        return 1.0 + totalEMGenBonus;
    }

    public bool IsPurchased(string id)
    {
        return states.TryGetValue(id, out var st) && st.purchased;
    }

    public bool CanPurchase(string id)
    {
        if (GameState.I == null) return false;

        var def = defs.Find(d => d.id == id);
        if (def == null) return false;

        // Ya comprada
        if (IsPurchased(id)) return false;

        // Requisito previo
        if (!string.IsNullOrEmpty(def.prereqId) && !IsPurchased(def.prereqId))
            return false;

        // IP suficiente
        return GameState.I.IP >= def.costIP;
    }

    public bool TryPurchase(string id)
    {
        if (!CanPurchase(id)) return false;
        if (GameState.I == null) return false;

        var def = defs.Find(d => d.id == id);
        if (def == null) return false;

        GameState.I.IP -= def.costIP;

        if (!states.TryGetValue(id, out var st))
        {
            st = new ResearchState { id = id, purchased = true };
            states[id] = st;
        }
        else
        {
            st.purchased = true;
        }

        RecalculateBonuses();
        return true;
    }

    public List<string> GetPurchasedIds()
    {
        var result = new List<string>();
        foreach (var kv in states)
        {
            if (kv.Value.purchased)
            {
                result.Add(kv.Key);
            }
        }
        return result;
    }

#if UNITY_EDITOR
    [ContextMenu("DEBUG: Dar 500 IP")]
    private void DebugGiveIP()
    {
        if (GameState.I != null)
        {
            GameState.I.IP += 500.0;
            Debug.Log("[ResearchManager] DEBUG: +500 IP");
        }
        else
        {
            Debug.LogWarning("[ResearchManager] DEBUG: GameState.I es null.");
        }
    }

    [ContextMenu("DEBUG: Comprar 'Estabilización EM I'")]
    private void DebugBuyEmStability1()
    {
        if (ResearchManager.I == null)
        {
            Debug.LogWarning("[ResearchManager] DEBUG: No hay instancia activa de ResearchManager.");
            return;
        }

        bool ok = ResearchManager.I.TryPurchase("em_stability_1");
        if (ok)
        {
            Debug.Log("[ResearchManager] DEBUG: Comprada 'em_stability_1'.");
        }
        else
        {
            Debug.LogWarning("[ResearchManager] DEBUG: NO se pudo comprar 'em_stability_1' (falta IP o prereq).");
        }
    }
#endif
}


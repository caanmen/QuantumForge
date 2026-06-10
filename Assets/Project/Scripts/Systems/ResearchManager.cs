using System;
using System.Collections.Generic;
using UnityEngine;

public enum ResearchEffectType
{
    None = 0,
    GlobalLEMult = 1,
    EMGenerationMult = 2,
    TriangleSystemUnlock = 3
}

[Serializable]
public class ResearchDef
{
    public string id;
    public string name;
    public string description;
    public double costLE;
    public double costTraces;
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

    public void ApplyLoadedResearch(List<string> ids)
    {
        if (ids == null) return;

        // Reset a false
        foreach (var kv in states)
        {
            kv.Value.purchased = false;
        }

        // Aplicar compras
        foreach (var id in ids)
        {
            if (states.TryGetValue(id, out var st))
            {
                st.purchased = true;
            }
        }

        RecalculateBonuses();
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
        if (GameState.I == null)
        {
            DebugLogCanPurchaseReason(id, "GameState nulo");
            return false;
        }

        var def = defs.Find(d => d.id == id);
        if (def == null)
        {
            DebugLogCanPurchaseReason(id, "Def no encontrada");
            return false;
        }

        // Ya comprada
        if (IsPurchased(id))
        {
            DebugLogCanPurchaseReason(id, "Ya estaba comprada");
            return false;
        }

        // Requisito previo
        if (!string.IsNullOrEmpty(def.prereqId) && !IsPurchased(def.prereqId))
        {
            DebugLogCanPurchaseReason(id, $"Falta prereq '{def.prereqId}'");
            return false;
        }

        // LE suficiente
        if (GameState.I.LE < def.costLE)
        {
            DebugLogCanPurchaseReason(
                id,
                $"Falta LE (tiene {GameState.I.LE}, requiere {def.costLE})"
            );
            return false;
        }

        // Trazas suficientes
        if (GameState.I.Traces < def.costTraces)
        {
            DebugLogCanPurchaseReason(
                id,
                $"Faltan Trazas (tiene {GameState.I.Traces}, requiere {def.costTraces})"
            );
            return false;
        }

        return true;
    }


    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void DebugLogCanPurchaseReason(string id, string reason)
    {
        // Logs desactivados (evita spam en consola)
    }



    public bool TryPurchase(string id)
    {
        if (!CanPurchase(id)) return false;
        if (GameState.I == null) return false;

        var def = defs.Find(d => d.id == id);
        if (def == null) return false;

        GameState.I.LE -= def.costLE;
        GameState.I.Traces -= def.costTraces;
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
        Debug.LogWarning("[ResearchManager] DEBUG: NO se pudo comprar 'em_stability_1' (faltan Trazas o prereq).");        }
    }

}


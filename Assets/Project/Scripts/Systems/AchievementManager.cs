using System;
using System.Collections.Generic;
using UnityEngine;

public enum AchievementConditionType
{
    None = 0,
    ReachLE = 1,
    ReachEM = 2,
    ReachIP = 3,
    PurchasedResearchCount = 4
}

[Serializable]
public class AchievementDef
{
    public string id;
    public string name;
    public string description;

    // Lo leemos como string desde el JSON ("ReachLE", "ReachEM"...)
    public string conditionType;

    public double threshold;
    public double rewardGlobalLEMult;
}

[Serializable]
public class AchievementDefList
{
    public List<AchievementDef> achievements;
}

[Serializable]
public class AchievementState
{
    public string id;
    public bool unlocked;
}

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager I { get; private set; }

    [Header("Datos")]
    [Tooltip("Archivo JSON en Resources/Data/achievements.json")]
    public string jsonPath = "Data/achievements";

    public List<AchievementDef> defs = new List<AchievementDef>();
    public Dictionary<string, AchievementState> states = new Dictionary<string, AchievementState>();

    // Bonus acumulado por logros
    private double totalGlobalLEBonus = 0.0;

    // Para no chequear cada frame
    private float checkTimer = 0f;
    [SerializeField] private float checkInterval = 1f; // cada 1 segundo

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

        // Intentar aplicar lo que haya cargado el SaveService
        ApplyLoadedAchievements(SaveService.LastLoadedAchievementIds);
    }

    private void Update()
    {
        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;
            CheckAchievements();
        }
    }

    private void LoadDefsFromJson()
    {
        try
        {
            TextAsset asset = Resources.Load<TextAsset>(jsonPath);
            if (asset == null)
            {
                Debug.LogError($"[AchievementManager] No se encontró achievements.json en Resources/{jsonPath}");
                return;
            }

            var list = JsonUtility.FromJson<AchievementDefList>(asset.text);
            if (list != null && list.achievements != null)
            {
                defs = list.achievements;
                Debug.Log($"[AchievementManager] Cargados {defs.Count} logros.");
            }
            else
            {
                Debug.LogError("[AchievementManager] Error al parsear achievements.json (lista nula).");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AchievementManager] Error cargando achievements.json: {ex.Message}");
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
                states[def.id] = new AchievementState
                {
                    id = def.id,
                    unlocked = false
                };
            }
        }
    }

    /// <summary>
    /// Aplica una lista de IDs desbloqueados (desde el SaveService).
    /// </summary>
    public void ApplyLoadedAchievements(List<string> saved)
    {
        if (saved == null) return;

        #if UNITY_EDITOR
        Debug.Log($"[AchievementManager] ApplyLoadedAchievements: {saved.Count} ids desde save.");
        #endif

        foreach (var id in saved)
        {
            if (states.TryGetValue(id, out var st))
            {
                st.unlocked = true;
            }
        }

        RecalculateBonuses();
    }

    private void CheckAchievements()
    {
        var gs = GameState.I;
        if (gs == null) return;

        foreach (var def in defs)
        {
            if (!states.TryGetValue(def.id, out var st)) continue;
            if (st.unlocked) continue; // ya desbloqueado

            if (IsConditionMet(def, gs))
            {
                st.unlocked = true;
                OnAchievementUnlocked(def);
            }
        }

        RecalculateBonuses();
    }

    private AchievementConditionType ParseCondition(string cond)
    {
        if (string.IsNullOrEmpty(cond)) return AchievementConditionType.None;

        switch (cond)
        {
            case "ReachLE": return AchievementConditionType.ReachLE;
            case "ReachEM": return AchievementConditionType.ReachEM;
            case "ReachIP": return AchievementConditionType.ReachIP;
            case "PurchasedResearchCount": return AchievementConditionType.PurchasedResearchCount;
            default: return AchievementConditionType.None;
        }
    }

    private bool IsConditionMet(AchievementDef def, GameState gs)
    {
        AchievementConditionType type = ParseCondition(def.conditionType);

        switch (type)
        {
            case AchievementConditionType.ReachLE:
                // Usar el MÁXIMO LE alcanzado en la run,
                // así no importa si luego baja o si se pasa rápido por 1000.
                return gs.maxLEAlcanzado >= def.threshold;

            case AchievementConditionType.ReachEM:
                return gs.EM >= def.threshold;

            case AchievementConditionType.ReachIP:
                return gs.IP >= def.threshold;

            case AchievementConditionType.PurchasedResearchCount:
                if (ResearchManager.I == null) return false;
                int count = 0;
                foreach (var kv in ResearchManager.I.states)
                {
                    if (kv.Value.purchased) count++;
                }
                return count >= def.threshold;

            default:
                return false;
        }
    }


    private void OnAchievementUnlocked(AchievementDef def)
    {
        Debug.Log($"[AchievementManager] Logro desbloqueado: {def.name} - {def.description}");

        // Popup visual
        if (AchievementPopupUI.I != null)
        {
            AchievementPopupUI.I.ShowPopup(def.name, def.description);
        }

        // 🔹 Nuevo: refrescar la lista de logros en pantalla si existe
        var listUI = FindFirstObjectByType<AchievementListUI>();
        if (listUI != null)
        {
            listUI.Refresh();
        }
    }



    private void RecalculateBonuses()
    {
        totalGlobalLEBonus = 0.0;

        foreach (var def in defs)
        {
            if (!states.TryGetValue(def.id, out var st) || !st.unlocked)
                continue;

            totalGlobalLEBonus += def.rewardGlobalLEMult;
        }

#if UNITY_EDITOR
        if (totalGlobalLEBonus > 0)
        {
            Debug.Log($"[AchievementManager] Bonus global LE por logros: +{totalGlobalLEBonus * 100:0.#}%");
        }
#endif
    }

    /// <summary>
    /// Factor multiplicador global de LE (1.0 = sin bonus).
    /// Lo usará GameState en CalculateTotalLEps().
    /// </summary>
    public double GetGlobalLEFactor()
    {
        return 1.0 + totalGlobalLEBonus;
    }

    public bool IsUnlocked(string id)
    {
        return states.TryGetValue(id, out var st) && st.unlocked;
    }

    public List<string> GetUnlockedIds()
    {
        var list = new List<string>();
        foreach (var kv in states)
        {
            if (kv.Value.unlocked)
                list.Add(kv.Key);
        }
        return list;
    }

        /// <summary>
    /// Resetea TODOS los logros a bloqueados.
    /// Usado por el debug "Reset Save (completo)".
    /// </summary>
    public void ResetAllAchievements()
    {
        // Poner todos los estados en bloqueado
        foreach (var kv in states)
        {
            kv.Value.unlocked = false;
        }

        // Recalcular el bonus global de LE (se pondrá a 0)
        RecalculateBonuses();

        // Si tienes una lista UI abierta, se refrescará cuando se vuelva a abrir.
        // (No hace falta nada más aquí)
    }

}

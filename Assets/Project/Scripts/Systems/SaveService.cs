using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public double LE;
    public double Traces;
    public double VP;

    // Mid-game: recurso EM
    public double EM;
    public double emMult;

    // F6.1: Moneda de prestigio
    public double ENT;

    // F6.1: Máximo LE alcanzado en el run
    public double maxLEAlcanzado;

    // F7: Recursos late-game
    public double ADP;
    public double WHF;

    // F7: Prestigio 2 (Lambda)
    public double Lambda;

    // F7: Estadísticas acumuladas para meta-prestigio
    public double totalENTAcumulada;
    public double totalADPGenerada;
    public double totalWHFGenerada;

    // Investigación
    public double IP;

    // Lista de investigaciones compradas (ids)
    public List<string> purchasedResearchIds;

    // Lista de logros desbloqueados (ids)
    public List<string> unlockedAchievementIds;

    public double baseLEps;
    public long lastUnix;

    // F6.4: Upgrades de prestigio
    public bool prestigeLeMult1Unlocked;
    public bool prestigeAutoBuyFirstUnlocked;
    public bool prestigeAutoBuyFirstEnabled;


    // F7.5: Meta-upgrades comprados con Λ
    public bool metaEntBoost1Bought;
    public bool metaEmBoost1Bought;

    // Niveles de edificios
    public List<SavedBuildingLevel> buildingLevels;
}

public class SaveService : MonoBehaviour
{
    public static List<string> LastLoadedResearchIds;
    public static List<string> LastLoadedAchievementIds;
    public static List<SavedBuildingLevel> LastLoadedBuildingLevels;



    public static SaveService I { get; private set; }

    private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    [Tooltip("Autosave cada N segundos.")]
    public int autosaveSeconds = 30;

    private void Awake()
    {
        // Versión simple: este objeto de la escena es SIEMPRE la instancia
        I = this;

#if UNITY_EDITOR
        Debug.Log("[SaveService] Awake()");
#endif
    }

    private void Start()
    {
#if UNITY_EDITOR
        Debug.Log("[SaveService] Start()");
#endif
        // OJO: el Load lo hará GameState.Start(), cuando GameState.I ya exista
        InvokeRepeating(nameof(Save), autosaveSeconds, autosaveSeconds);
    }

    private void OnApplicationQuit()
    {
#if UNITY_EDITOR
        Debug.Log("[SaveService] OnApplicationQuit -> Save()");
#endif
        Save();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
#if UNITY_EDITOR
            Debug.Log("[SaveService] OnApplicationPause(true) -> Save()");
#endif
            Save();
        }
    }

    public void Save()
    {
        if (GameState.I == null)
        {
        #if UNITY_EDITOR
            Debug.LogWarning("[SaveService] Save() cancelado: GameState.I es null.");
        #endif
            return;
        }

        var data = new SaveData
        {
        LE = GameState.I.LE,
        Traces = GameState.I.Traces,
        VP = GameState.I.VP,
        EM = GameState.I.EM,
        emMult = GameState.I.emMult,
        IP = GameState.I.IP,
        baseLEps = GameState.I.baseLEps,

        // F6.1: prestigio
        ENT = GameState.I.ENT,
        maxLEAlcanzado = GameState.I.maxLEAlcanzado,

        // F7: recursos late-game
        ADP = GameState.I.ADP,
        WHF = GameState.I.WHF,

        // F7: prestigio 2 (Lambda) + estadísticas
        Lambda = GameState.I.Lambda,
        totalENTAcumulada = GameState.I.totalENTAcumulada,
        totalADPGenerada = GameState.I.totalADPGenerada,
        totalWHFGenerada = GameState.I.totalWHFGenerada,

        // F6.4: upgrades de prestigio
        prestigeLeMult1Unlocked = GameState.I.prestigeLeMult1Unlocked,
        prestigeAutoBuyFirstUnlocked = GameState.I.prestigeAutoBuyFirstUnlocked,
        prestigeAutoBuyFirstEnabled = GameState.I.prestigeAutoBuyFirstEnabled,


        // F7.5: meta-upgrades
        metaEntBoost1Bought = GameState.I.metaEntBoost1Bought,
        metaEmBoost1Bought = GameState.I.metaEmBoost1Bought,

         // 🆕 Niveles de edificios
        buildingLevels = GameState.I.GetBuildingLevelsForSave(),


        purchasedResearchIds = (ResearchManager.I != null)
            ? ResearchManager.I.GetPurchasedIds()
            : LastLoadedResearchIds,
        unlockedAchievementIds = (AchievementManager.I != null)
            ? AchievementManager.I.GetUnlockedIds()
            : LastLoadedAchievementIds,
        lastUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
    };



        var json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(SavePath, json);

#if UNITY_EDITOR
        Debug.Log($"[SaveService] Saved: {SavePath}");
#endif
    }

    public void Load()
    {
#if UNITY_EDITOR
        Debug.Log($"[SaveService] Load() llamado. Existe archivo? {File.Exists(SavePath)} ({SavePath})");
#endif

        if (GameState.I == null) return;

        if (!File.Exists(SavePath))
        {
            InitNewGame();
            return;
        }

        var json = File.ReadAllText(SavePath);
        var data = JsonUtility.FromJson<SaveData>(json);

        bool noBuildings = (data.buildingLevels == null || data.buildingLevels.Count == 0);
        bool looksFresh = data.LE <= 0.0 && data.maxLEAlcanzado <= 0.0 && data.ENT <= 0.0 && data.Lambda <= 0.0 && noBuildings;

        if (looksFresh)
        {
            InitNewGame();
            return;
        }


        GameState.I.LE = data.LE;
        GameState.I.Traces = data.Traces;
        GameState.I.VP = data.VP;
        GameState.I.EM = data.EM;
        GameState.I.emMult = data.emMult;
        GameState.I.IP = data.IP;
        GameState.I.baseLEps = data.baseLEps;

        // F6.1: prestigio
        GameState.I.ENT = data.ENT;
        GameState.I.maxLEAlcanzado = data.maxLEAlcanzado;

        // F7: recursos late-game y Lambda
        GameState.I.ADP = data.ADP;
        GameState.I.WHF = data.WHF;
        GameState.I.Lambda = data.Lambda;
        GameState.I.totalENTAcumulada = data.totalENTAcumulada;
        GameState.I.totalADPGenerada = data.totalADPGenerada;
        GameState.I.totalWHFGenerada = data.totalWHFGenerada;

        // F6.4: upgrades de prestigio
        GameState.I.prestigeLeMult1Unlocked = data.prestigeLeMult1Unlocked;
        GameState.I.prestigeAutoBuyFirstUnlocked = data.prestigeAutoBuyFirstUnlocked;
        GameState.I.prestigeAutoBuyFirstEnabled  = data.prestigeAutoBuyFirstEnabled;


        // F7.5: meta-upgrades
        GameState.I.metaEntBoost1Bought = data.metaEntBoost1Bought;
        GameState.I.metaEmBoost1Bought = data.metaEmBoost1Bought;

        
        // 🆕 Guardar niveles de edificios para aplicarlos después
        SaveService.LastLoadedBuildingLevels = data.buildingLevels ?? new List<SavedBuildingLevel>();


        // Nos aseguramos de que el máximo quede coherente
        GameState.I.ActualizarMaxLE();

        LastLoadedResearchIds = data.purchasedResearchIds ?? new List<string>();
        LastLoadedAchievementIds = data.unlockedAchievementIds ?? new List<string>();

        if (ResearchManager.I != null)
        {
            ResearchManager.I.ApplyLoadedResearch(LastLoadedResearchIds);
        }


        if (AchievementManager.I != null)
        {
            AchievementManager.I.ApplyLoadedAchievements(LastLoadedAchievementIds);
        }

#if UNITY_EDITOR
        Debug.Log("[SaveService] Loaded.");
#endif
    }

    private void InitNewGame()
    {
        if (GameState.I == null) return;

        // ✅ Starter: lo mínimo para poder comprar el primer edificio (coste 10)
        GameState.I.LE = 10.0;
        GameState.I.VP = 0.0;

        GameState.I.EM = 0.0;
        GameState.I.emMult = 0.0;

        GameState.I.IP = 0.0;
        GameState.I.baseLEps = 0.0;

        GameState.I.ENT = 0.0;
        GameState.I.maxLEAlcanzado = 0.0;

        GameState.I.ADP = 0.0;
        GameState.I.WHF = 0.0;

        // Meta-progreso se mantiene en 0 para un save nuevo
        GameState.I.Lambda = 0.0;
        GameState.I.totalENTAcumulada = 0.0;
        GameState.I.totalADPGenerada = 0.0;
        GameState.I.totalWHFGenerada = 0.0;

        // Flags prestigio/meta-upgrades por defecto
        GameState.I.prestigeLeMult1Unlocked = false;
        GameState.I.prestigeAutoBuyFirstUnlocked = false;
        GameState.I.prestigeAutoBuyFirstEnabled = true;

        GameState.I.metaEntBoost1Bought = false;
        GameState.I.metaEmBoost1Bought = false;

        // Limpiar caches de load para evitar arrastrar algo raro
        LastLoadedResearchIds = new List<string>();
        LastLoadedAchievementIds = new List<string>();
        LastLoadedBuildingLevels = new List<SavedBuildingLevel>();

        // Aplica listas vacías si existen managers
        if (ResearchManager.I != null) ResearchManager.I.ApplyLoadedResearch(LastLoadedResearchIds);
        if (AchievementManager.I != null) AchievementManager.I.ApplyLoadedAchievements(LastLoadedAchievementIds);

        // Dejar max coherente
        GameState.I.ActualizarMaxLE();

        // ✅ Crear el save inmediatamente (en Android es clave)
        Save();
    }


    [ContextMenu("Reset Save (simple)")]
    public void ResetSave()
    {
        if (File.Exists(SavePath)) File.Delete(SavePath);

        if (GameState.I != null)
        {
            GameState.I.DebugResetRunState();
            GameState.I.Traces = 0.0;
        }

    #if UNITY_EDITOR
        Debug.Log("[SaveService] Save deleted.");
    #endif
    }

    #if UNITY_EDITOR
    [ContextMenu("DEBUG: Reset Save (completo)")]
    private void DebugResetSave()
    {
        try
        {
            // 1) Borrar el archivo de save en disco
            if (File.Exists(SavePath))
            {   
                File.Delete(SavePath);
                Debug.Log($"[SaveService] DEBUG: Save borrado en {SavePath}");
            }
            else
            {
                Debug.Log("[SaveService] DEBUG: No había archivo de save para borrar.");
            }

            // 2) Limpiar los buffers estáticos en memoria
            LastLoadedResearchIds    = null;
            LastLoadedAchievementIds = null;
            LastLoadedBuildingLevels = null;

            // 3) Resetear el GameState en memoria (LE, base, edificios, prestigio, etc.)
            if (GameState.I != null)
            {
                // Reset del run completo (LE, base, edificios, maxLE, etc.)
                GameState.I.DebugResetRunState();

                // Reset de monedas de prestigio y late-game
                GameState.I.ENT  = 0.0;
                GameState.I.Traces = 0.0;
                GameState.I.ADP  = 0.0;
                GameState.I.WHF  = 0.0;
                GameState.I.Lambda            = 0.0;
                GameState.I.totalENTAcumulada = 0.0;
                GameState.I.totalADPGenerada  = 0.0;
                GameState.I.totalWHFGenerada  = 0.0;

                // Reset de upgrades de prestigio y meta-upgrades
                GameState.I.prestigeLeMult1Unlocked      = false;
                GameState.I.prestigeAutoBuyFirstUnlocked = false;
                GameState.I.prestigeAutoBuyFirstEnabled  = true;
                GameState.I.metaEntBoost1Bought          = false;
                GameState.I.metaEmBoost1Bought           = false;
                
                // Resetear research en memoria
                if (ResearchManager.I != null)
                {
                    ResearchManager.I.ApplyLoadedResearch(new List<string>());
                }
            }

            // 4) Resetear logros en memoria
            if (AchievementManager.I != null)
            {
                // Marcar TODOS los logros como bloqueados
                foreach (var kv in AchievementManager.I.states)
                {
                    kv.Value.unlocked = false;
                }

                // Recalcular bonus global
                AchievementManager.I.SendMessage(
                    "RecalculateBonuses",
                    SendMessageOptions.DontRequireReceiver
                );

                // Refrescar la UI de la lista de logros si está abierta
                var listUI = FindFirstObjectByType<AchievementListUI>();
                if (listUI != null)
                {
                    listUI.Refresh();
                }
            }

            Debug.Log("[SaveService] DEBUG: Reset completo aplicado en memoria.");
        }
        catch (Exception ex)
        {
            Debug.LogError("[SaveService] DEBUG: Error al borrar el save: " + ex.Message);
        }
    }



    [ContextMenu("DEBUG: Save Now")]
    private void DebugSaveNow()
    {
        Save();
        Debug.Log("[SaveService] DEBUG: Save Now ejecutado.");
    }
#endif
}

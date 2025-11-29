using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public double LE;
    public double VP;

    // Mid-game: recurso EM
    public double EM;
    public double emMult;

    // F6.1: Moneda de prestigio
    public double ENT;

    // F6.1: Máximo LE alcanzado en el run
    public double maxLEAlcanzado;

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
}

public class SaveService : MonoBehaviour
{
    public static List<string> LastLoadedResearchIds;
    public static List<string> LastLoadedAchievementIds;

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
        VP = GameState.I.VP,
        EM = GameState.I.EM,
        emMult = GameState.I.emMult,
        IP = GameState.I.IP,
        baseLEps = GameState.I.baseLEps,

        // F6.1: prestigio
        ENT = GameState.I.ENT,
        maxLEAlcanzado = GameState.I.maxLEAlcanzado,

        // F6.4: upgrades de prestigio
            prestigeLeMult1Unlocked = GameState.I.prestigeLeMult1Unlocked,
            prestigeAutoBuyFirstUnlocked = GameState.I.prestigeAutoBuyFirstUnlocked,

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

        if (!File.Exists(SavePath) || GameState.I == null) return;

        var json = File.ReadAllText(SavePath);
        var data = JsonUtility.FromJson<SaveData>(json);

        GameState.I.LE = data.LE;
        GameState.I.VP = data.VP;
        GameState.I.EM = data.EM;
        GameState.I.emMult = data.emMult;
        GameState.I.IP = data.IP;
        GameState.I.baseLEps = data.baseLEps;

        // F6.1: prestigio
        GameState.I.ENT = data.ENT;
        GameState.I.maxLEAlcanzado = data.maxLEAlcanzado;

        // F6.4: upgrades de prestigio
        GameState.I.prestigeLeMult1Unlocked = data.prestigeLeMult1Unlocked;
        GameState.I.prestigeAutoBuyFirstUnlocked = data.prestigeAutoBuyFirstUnlocked;

        // Nos aseguramos de que el máximo quede coherente
        GameState.I.ActualizarMaxLE();

        LastLoadedResearchIds = data.purchasedResearchIds ?? new List<string>();
        LastLoadedAchievementIds = data.unlockedAchievementIds ?? new List<string>();



        if (AchievementManager.I != null)
        {
            AchievementManager.I.ApplyLoadedAchievements(LastLoadedAchievementIds);
        }

#if UNITY_EDITOR
        Debug.Log("[SaveService] Loaded.");
#endif
    }

    [ContextMenu("Reset Save (simple)")]
    public void ResetSave()
    {
        if (File.Exists(SavePath)) File.Delete(SavePath);
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
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                Debug.Log($"[SaveService] DEBUG: Save borrado en {SavePath}");
            }
            else
            {
                Debug.Log("[SaveService] DEBUG: No había archivo de save para borrar.");
            }

            LastLoadedResearchIds = null;
            LastLoadedAchievementIds = null;

            if (GameState.I != null)
            {
                GameState.I.LE = 0;
                GameState.I.VP = 0;
                GameState.I.EM = 0;
                GameState.I.emMult = 0;
                GameState.I.IP = 0;
            }
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

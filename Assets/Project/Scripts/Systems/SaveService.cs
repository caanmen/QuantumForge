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

    // Investigación
    public double IP;

    // Lista de investigaciones compradas (ids)
    public List<string> purchasedResearchIds;

    public double baseLEps;
    public long lastUnix;
}


public class SaveService : MonoBehaviour
{
    public static List<string> LastLoadedResearchIds;
    public static SaveService I { get; private set; }
    private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    [Tooltip("Autosave cada N segundos.")]
    public int autosaveSeconds = 30;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Load();
        InvokeRepeating(nameof(Save), autosaveSeconds, autosaveSeconds);
    }

    public void Save()
    {
        if (GameState.I == null) return;
        var data = new SaveData
{
    LE = GameState.I.LE,
    VP = GameState.I.VP,
    EM = GameState.I.EM,
    emMult = GameState.I.emMult,
    IP = GameState.I.IP,

    // si hay ResearchManager, tomamos lo que tenga; si no, dejamos lo último cargado
    purchasedResearchIds = (ResearchManager.I != null)
        ? ResearchManager.I.GetPurchasedIds()
        : LastLoadedResearchIds,

    baseLEps = GameState.I.baseLEps,
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
    if (!File.Exists(SavePath) || GameState.I == null) return;

    var json = File.ReadAllText(SavePath);
    var data = JsonUtility.FromJson<SaveData>(json);

    GameState.I.LE = data.LE;
    GameState.I.VP = data.VP;
    GameState.I.EM = data.EM;
    GameState.I.emMult = data.emMult;
    GameState.I.IP = data.IP;
    GameState.I.baseLEps = data.baseLEps;

    // Guardamos la lista para que ResearchManager la use en su Awake
    LastLoadedResearchIds = data.purchasedResearchIds ?? new List<string>();

#if UNITY_EDITOR
    Debug.Log("[SaveService] Loaded.");
#endif
}



    [ContextMenu("Reset Save")]
    public void ResetSave()
    {
        if (File.Exists(SavePath)) File.Delete(SavePath);
#if UNITY_EDITOR
        Debug.Log("[SaveService] Save deleted.");
#endif
    }
}

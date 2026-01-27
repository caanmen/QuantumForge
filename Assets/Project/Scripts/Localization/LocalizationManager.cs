using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager I { get; private set; }

    public enum Language { ES, EN }

    [SerializeField] private Language defaultLanguage = Language.ES;

    private readonly Dictionary<string, string> _dict = new Dictionary<string, string>();
    public Language CurrentLanguage { get; private set; }

    [Serializable]
    private class JsonDict
    {
        public List<Entry> entries = new List<Entry>();
    }

    [Serializable]
    private class Entry
    {
        public string key;
        public string value;
    }

    // Soporta JSON tipo: { "a":"b", "c":"d" } (como los tuyos)
    private static Dictionary<string, string> ParseSimpleJsonObject(string json)
{
    var result = new Dictionary<string, string>();
    json = json.Trim();

    // Quitar llaves externas
    if (json.StartsWith("{")) json = json.Substring(1);
    if (json.EndsWith("}")) json = json.Substring(0, json.Length - 1);

    int i = 0;
    while (i < json.Length)
    {
        // saltar espacios/commas
        while (i < json.Length && (char.IsWhiteSpace(json[i]) || json[i] == ',')) i++;
        if (i >= json.Length) break;

        // leer key "..."
        if (json[i] != '"') break;
        i++;
        int kStart = i;
        while (i < json.Length && json[i] != '"') i++;
        if (i >= json.Length) break;
        string key = json.Substring(kStart, i - kStart);
        i++; // cerrar "

        // saltar espacios y :
        while (i < json.Length && (char.IsWhiteSpace(json[i]) || json[i] == ':')) i++;
        if (i >= json.Length) break;

        // leer value "..."
        if (json[i] != '"') break;
        i++;
        int vStart = i;
        while (i < json.Length && json[i] != '"') i++;
        if (i >= json.Length) break;
        string value = json.Substring(vStart, i - vStart);
        i++; // cerrar "

        value = value.Replace("\\n", "\n").Replace("\\\"", "\"");
        result[key] = value;
    }

    return result;
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

        SetLanguage(defaultLanguage);
    }

    public void SetLanguage(Language lang)
    {

        CurrentLanguage = lang;
        _dict.Clear();

        string fileName = (lang == Language.EN) ? "lang_en" : "lang_es";
        TextAsset ta = Resources.Load<TextAsset>($"Localization/{fileName}");
        Debug.Log($"[Localization] RAW contains ui.cost_prefix? {(ta.text.Contains("ui.cost_prefix"))}");

        if (ta == null) return;

        var parsed = ParseSimpleJsonObject(ta.text);
        foreach (var kv in parsed)
            _dict[kv.Key] = kv.Value;



        Debug.Log($"Has ui.cost_prefix? {_dict.ContainsKey("ui.cost_prefix")}");

        Debug.Log($"[Localization] Idioma cargado: {lang} | keys: {_dict.Count} | has level_prefix: {_dict.ContainsKey("ui.level_prefix")}");

        LocalizedTMP.RefreshAll();

    }

    public string T(string key)
    {
        if (string.IsNullOrEmpty(key)) return "";
        if (_dict.TryGetValue(key, out var value)) return value;
        return key; // fallback: muestra la key si no existe
    }
}

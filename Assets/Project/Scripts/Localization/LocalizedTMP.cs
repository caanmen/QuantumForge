using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedTMP : MonoBehaviour
{
    public string key;

    private TextMeshProUGUI _tmp;
    private int _lastLang = -1;

    private static readonly HashSet<LocalizedTMP> _all = new HashSet<LocalizedTMP>();

    private void Awake()
    {
        _tmp = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        _all.Add(this);
        ForceRefresh();
    }

    private void OnDisable()
    {
        _all.Remove(this);
    }

    private void Update()
    {
        var lm = LocalizationManager.I;
        if (lm == null) return;

        int langNow = (int)lm.CurrentLanguage;
        if (langNow != _lastLang)
        {
            _lastLang = langNow;
            ForceRefresh();
        }
    }

    private void ForceRefresh()
    {
        if (_tmp == null) return;
        if (LocalizationManager.I == null) return;
        if (string.IsNullOrEmpty(key)) return;

        string k = key.Trim();   // <- CLAVE: limpia espacios invisibles
        key = k;                 // opcional: deja guardado limpio

        _lastLang = (int)LocalizationManager.I.CurrentLanguage;
        _tmp.SetText(LocalizationManager.I.T(k));
    }


    public void Refresh()
    {
        ForceRefresh();
    }

    public static void RefreshAll()
    {
        foreach (var lt in _all)
            if (lt != null) lt.Refresh();
    }
}

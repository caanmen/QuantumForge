using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedTMP : MonoBehaviour
{
    public string key;

    private TextMeshProUGUI _tmp;

    private static readonly HashSet<LocalizedTMP> _all = new HashSet<LocalizedTMP>();

    private void Awake()
    {
        _tmp = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        _all.Add(this);
        Refresh();
    }

    private void OnDisable()
    {
        _all.Remove(this);
    }

    public void Refresh()
    {
        if (_tmp == null) return;
        if (LocalizationManager.I == null) return;
        if (string.IsNullOrEmpty(key)) return;

        _tmp.SetText(LocalizationManager.I.T(key));
    }

    public static void RefreshAll()
    {
        foreach (var lt in _all)
            if (lt != null) lt.Refresh();
    }
}

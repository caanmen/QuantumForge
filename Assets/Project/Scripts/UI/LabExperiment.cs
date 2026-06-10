using UnityEngine;
using TMPro;
using System;

public class LabExperiment : MonoBehaviour
{
    [Header("Costos del experimento")]
    [Tooltip("LE que se gasta cada vez que se ejecuta el experimento.")]
    public double leCost = 100000.0;

    [Tooltip("Campo legado sin uso activo.")]
    public double ipReward = 10.0;

    [Header("UI")]
    [Tooltip("Texto que muestra la descripción del experimento.")]
    public TextMeshProUGUI infoText;

    private int _lastLang = -1;

    private void Start()
    {
        // Inicializa el idioma actual
        var lm = LocalizationManager.I;
        _lastLang = (lm != null) ? (int)lm.CurrentLanguage : -1;

        ActualizarTexto();
    }

    private void Update()
    {
        // Si cambia el idioma, refresca el texto
        var lm = LocalizationManager.I;
        if (lm == null) return;

        int langNow = (int)lm.CurrentLanguage;
        if (langNow != _lastLang)
        {
            _lastLang = langNow;
            ActualizarTexto();
        }
    }

    public void EjecutarExperimento()
    {
        return;
    }

    

    private string L(string key, string fallback)
    {
        var lm = LocalizationManager.I;
        if (lm == null) return fallback;

        var s = lm.T(key);

        // Si no existe, tu T() devuelve la misma key
        if (string.IsNullOrEmpty(s) || s == key) return fallback;

        return s;
    }

    private string LF(string key, string fallback, params object[] args)
    {
        string fmt = L(key, fallback);
        try { return string.Format(fmt, args); }
        catch { return fmt; }
    }

    private void ActualizarTexto()
    {
        if (infoText == null) return;

        infoText.text = "Experimento desactivado por ahora.";
    }
}

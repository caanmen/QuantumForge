using UnityEngine;
using TMPro;
using System;

public class LabExperiment : MonoBehaviour
{
    [Header("Costos del experimento")]
    [Tooltip("LE que se gasta cada vez que se ejecuta el experimento.")]
    public double leCost = 100000.0;

    [Tooltip("IP que se gana por cada experimento.")]
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
        var gs = GameState.I;
        if (gs == null) return;

        if (gs.LE < leCost)
        {
#if UNITY_EDITOR
            Debug.Log("[LabExperiment] No hay LE suficiente para el experimento.");
#endif
            return;
        }

        gs.LE -= leCost;
        gs.IP += ipReward;

        // Si en el futuro cambias leCost/ipReward en runtime,
        // esto asegura que el texto también se actualice.
        ActualizarTexto();
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

        infoText.text = LF(
            "lab.experiment_template",
            "Experimento:\nGasta {0:0} LE → Gana {1:0} IP",
            leCost, ipReward
        );
    }
}

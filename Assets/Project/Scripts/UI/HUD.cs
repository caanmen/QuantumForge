using System;
using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    [Header("Textos HUD")]
    public TextMeshProUGUI leText;        // LE y LE/s
    public TextMeshProUGUI tracesText;    // Trazas
    public TextMeshProUGUI energyText;    // Energía del Triángulo
    public TextMeshProUGUI vpText;        // VP
    public TextMeshProUGUI emText;        // EM
    public TextMeshProUGUI adpText;       // ADP
    public TextMeshProUGUI whfText;       // WHF
    public TextMeshProUGUI becText;       // BEC
    [Header("Rendimiento")]
    [SerializeField] private float uiRefreshInterval = 0.25f;
    private float _uiTimer;

    private void Update()
    {
        _uiTimer += Time.unscaledDeltaTime;
        if (_uiTimer < uiRefreshInterval) return;
        _uiTimer = 0f;

        RefreshNow();
    }

    public void RefreshNow()
    {
        var gs = GameState.I;
        if (gs == null) return;

        // LE y LE/s
        if (leText != null)
        {
            double leps = gs.GetTotalLEps();
            string leLabel = Localize("hud.le", "LE");

            leText.SetText($"{leLabel} {Compact(gs.LE)}\n+{CompactRate(leps)}/s");
        }

        // Trazas
        if (tracesText != null)
        {
            double tracesPs = gs.CalculateTracesPs();
            string tracesLabel = Localize("hud.traces", "TRAZAS");
            tracesText.SetText(
                $"{tracesLabel} {Compact(gs.Traces)}\n+{CompactRate(tracesPs)}/s");
        }

        if (energyText != null)
        {
            string energyLabel = Localize("hud.triangle_energy", "ENERGÍA");
            energyText.SetText(
                $"{energyLabel} {Compact(gs.triangleEnergy)}\n+{CompactRate(gs.CalculateTriangleEnergyPerSecond())}/s");
        }

        // VP
        if (vpText != null)
            vpText.SetText("VP: {0:0}", (float)gs.VP);

        // BEC
        if (becText != null)
            becText.SetText("BEC: {0:0}", (float)gs.BEC);

    }

    private static string Localize(string key, string fallback)
    {
        if (LocalizationManager.I == null)
            return fallback;
        string value = LocalizationManager.I.T(key);
        return string.IsNullOrEmpty(value) || value == key ? fallback : value;
    }

    private static string Compact(double value)
    {
        double absolute = Math.Abs(value);
        if (absolute >= 1_000_000_000_000.0)
            return (value / 1_000_000_000_000.0).ToString("0.##") + "T";
        if (absolute >= 1_000_000_000.0)
            return (value / 1_000_000_000.0).ToString("0.##") + "B";
        if (absolute >= 1_000_000.0)
            return (value / 1_000_000.0).ToString("0.##") + "M";
        if (absolute >= 1_000.0)
            return (value / 1_000.0).ToString("0.##") + "K";
        return value.ToString("0");
    }

    private static string CompactRate(double value)
    {
        return Math.Abs(value) >= 1000.0 ? Compact(value) : value.ToString("0.00");
    }
}

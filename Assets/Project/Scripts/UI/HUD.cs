using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    [Header("Textos HUD")]
    public TextMeshProUGUI leText;        // LE y LE/s
    public TextMeshProUGUI tracesText;    // Trazas
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
        var gs = GameState.I;
        if (gs == null) return;

        _uiTimer += Time.unscaledDeltaTime;
        if (_uiTimer < uiRefreshInterval) return;
        _uiTimer = 0f;

        // LE y LE/s
        if (leText != null)
        {
            double leps = gs.GetTotalLEps();
            string leLabel = Localize("hud.le", "LE");

            leText.SetText($"{leLabel} {(float)gs.LE:0}\n+{(float)leps:0.00}/s");
        }

        // Trazas
        if (tracesText != null)
        {
            double tracesPs = gs.CalculateTracesPs();
            string tracesLabel = Localize("hud.traces", "TRAZAS");
            tracesText.SetText(
                $"{tracesLabel} {(float)gs.Traces:0}\n+{(float)tracesPs:0.00}/s");
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
}

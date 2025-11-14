using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    [Header("Textos HUD")]
    public TextMeshProUGUI leText;   // LE y LE/s
    public TextMeshProUGUI vpText;   // VP
    public TextMeshProUGUI becText;  // BEC (placeholder recurso futuro)

    private void Update()
    {
        var gs = GameState.I;
        if (gs == null) return;

        // LE y LE/s SIN decimales
        if (leText != null)
        {
            double leps = gs.GetTotalLEps();
            leText.text = $"LE: {gs.LE:0}  (LE/s: {leps:0})";
        }

        // VP sin decimales
        if (vpText != null)
        {
            vpText.text = $"VP: {gs.VP:0}";
        }

        // BEC sin decimales
        if (becText != null)
        {
            becText.text = $"BEC: {gs.BEC:0}";
        }
    }
}

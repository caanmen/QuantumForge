using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    [Header("Textos HUD")]
    public TextMeshProUGUI leText;    // LE y LE/s
    public TextMeshProUGUI vpText;    // VP
    public TextMeshProUGUI emText;    // EM
    public TextMeshProUGUI adpText;   // ADP  <-- NUEVO
    public TextMeshProUGUI whfText;   // WHF  <-- NUEVO
    public TextMeshProUGUI becText;   // BEC
    public TextMeshProUGUI ipText;    // IP

    private void Update()
    {
        var gs = GameState.I;
    if (gs == null) return;

        // LE y LE/s
        if (leText != null)
        {
            double leps = gs.GetTotalLEps();

            // 1) Factor base por EM (por la cantidad de EM)
            double emBase = 1.0 + gs.emMult;

            // 2) Factor por investigaciones de Cosecha EM
            double emGenFactor = 1.0;
            if (ResearchManager.I != null)
            {
                emGenFactor = ResearchManager.I.GetEMGenerationFactor();
            }

            // 🔹 EMx total que queremos mostrar en pantalla
            double emxVisual = emBase * emGenFactor;

            // 3) Factor de laboratorio (Estabilización EM I/II/III)
            double labFactor = gs.researchGlobalLEMult;

            leText.text =
                $"LE: {gs.LE:0}  " +
                $"(LE/s: {leps:0.00}  EMx: {emxVisual:0.00}  Labx: {labFactor:0.00})";
        }


        // VP
        if (vpText != null)
        {
            vpText.text = $"VP: {gs.VP:0}";
        }

        // EM
        if (emText != null)
        {
            emText.text = $"EM: {gs.EM:0}";
        }

        // ADP
        if (adpText != null)
        {
            adpText.text = $"ADP: {gs.ADP:0.###}";
        }

        // WHF
        if (whfText != null)
        {
            whfText.text = $"WHF: {gs.WHF:0.###}";
        }

        // BEC
        if (becText != null)
        {
            becText.text = $"BEC: {gs.BEC:0}";
        }

        // IP
        if (ipText != null)
        {
            ipText.text = $"IP: {gs.IP:0}";
        }
    }
}

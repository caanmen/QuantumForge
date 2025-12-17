using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    [Header("Textos HUD")]
    public TextMeshProUGUI leText;    // LE y LE/s
    public TextMeshProUGUI vpText;    // VP
    public TextMeshProUGUI emText;    // EM
    public TextMeshProUGUI adpText;   // ADP
    public TextMeshProUGUI whfText;   // WHF
    public TextMeshProUGUI becText;   // BEC
    public TextMeshProUGUI ipText;    // IP

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

            // 1) Factor base por EM
            double emBase = 1.0 + gs.emMult;

            // 2) Factor por investigaciones de Cosecha EM
            double emGenFactor = 1.0;
            if (ResearchManager.I != null)
                emGenFactor = ResearchManager.I.GetEMGenerationFactor();

            // EMx visual
            double emxVisual = emBase * emGenFactor;

            // 3) Factor de laboratorio
            double labFactor = gs.researchGlobalLEMult;

            // TMP SetText (menos alloc)
            leText.SetText("LE: {0:0}  (LE/s: {1:0.00}  EMx: {2:0.00}  Labx: {3:0.00})",
                (float)gs.LE, (float)leps, (float)emxVisual, (float)labFactor);
        }

        // VP
        if (vpText != null)
            vpText.SetText("VP: {0:0}", (float)gs.VP);

        // EM
        if (emText != null)
            emText.SetText("EM: {0:0}", (float)gs.EM);

        // ADP
        if (adpText != null)
            adpText.SetText("ADP: {0:0.###}", (float)gs.ADP);

        // WHF
        if (whfText != null)
            whfText.SetText("WHF: {0:0.###}", (float)gs.WHF);

        // BEC
        if (becText != null)
            becText.SetText("BEC: {0:0}", (float)gs.BEC);

        // IP
        if (ipText != null)
            ipText.SetText("IP: {0:0}", (float)gs.IP);
    }
}

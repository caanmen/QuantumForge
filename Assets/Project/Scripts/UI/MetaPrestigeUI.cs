using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MetaPrestigeUI : MonoBehaviour
{
    [Header("Textos Λ")]
    public TextMeshProUGUI lambdaActualText;  // "Λ actual: 0"
    public TextMeshProUGUI lambdaGainText;    // "Si meta-prestigias ahora: +0 Λ"
    public TextMeshProUGUI metaWarningText;   // línea de advertencia

    [Header("Botón")]
    public Button metaPrestigeButton;         // botón "Forjar Λ"
    [Header("Rendimiento UI")]
    [SerializeField] private float uiRefreshInterval = 0.25f;
    private float _uiTimer = 0f;


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

    private void Start()
    {
        if (metaPrestigeButton != null)
            metaPrestigeButton.onClick.AddListener(OnClickMetaPrestige);
    }

    private void Update()
    {
        var gs = GameState.I;
        if (gs == null) return;

        _uiTimer += Time.unscaledDeltaTime;
        if (_uiTimer < uiRefreshInterval) return;
        _uiTimer = 0f;


        double lambdaPreview = gs.GetLambdaPreview();
        double lambda = gs.Lambda;

        if (lambdaActualText != null)
        {
            lambdaActualText.SetText(
            L("meta.lambda_current", "Λ actual: {0:0}"),
            (float)lambda
        );

        }

        if (lambdaGainText != null)
        {
            lambdaGainText.SetText(
            L("meta.lambda_gain", "Si meta-prestigias ahora: +{0:0} Λ"),
            (float)lambdaPreview
        );

        }

        // Estado de desbloqueo / advertencia
        bool unlocked = gs.IsMetaPrestigeUnlocked;
        bool canMeta = gs.CanMetaPrestige();

        if (metaWarningText != null)
        {
            if (!unlocked)
            {
                metaWarningText.text = L(
                    "meta.warn_locked",
                    "Aún no has desbloqueado el Meta-Prestigio.\nAcumula al menos 50 ENT totales para activarlo."
                );
            }
            else if (!canMeta)
            {
                metaWarningText.text = L(
                    "meta.warn_nogain",
                    "Progreso insuficiente para ganar Λ.\nSigue subiendo LE, ENT, ADP y WHF."
                );
            }
            else
            {
                metaWarningText.text = L(
                    "meta.warn_reset",
                    "Advertencia: esto reseteará LE, EM, IP, ADP, WHF,\ntodos los edificios, ENT y sus mejoras."
                );
            }
        }

        // Habilitar / deshabilitar el botón según si se puede meta-prestigiar
        if (metaPrestigeButton != null)
            metaPrestigeButton.interactable = canMeta;
    }

    private void OnClickMetaPrestige()
    {
        var gs = GameState.I;
        if (gs == null) return;

        double gained = gs.DoMetaPrestigeReset();
    }
}

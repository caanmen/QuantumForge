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

    private void Start()
    {
        if (metaPrestigeButton != null)
            metaPrestigeButton.onClick.AddListener(OnClickMetaPrestige);
    }

    private void Update()
    {
        var gs = GameState.I;
        if (gs == null) return;

        // Λ actual
        if (lambdaActualText != null)
        {
            lambdaActualText.text = $"Λ actual: {gs.Lambda:0}";
        }

        // Λ a ganar (preview)
        double lambdaPreview = gs.GetLambdaPreview();
        if (lambdaGainText != null)
        {
            lambdaGainText.text = $"Si meta-prestigias ahora: +{lambdaPreview:0} Λ";
        }

        // Estado de desbloqueo / advertencia
        bool unlocked = gs.IsMetaPrestigeUnlocked;
        bool canMeta = gs.CanMetaPrestige();

        if (metaWarningText != null)
        {
            if (!unlocked)
            {
                metaWarningText.text =
                    "Aún no has desbloqueado el Meta-Prestigio.\n" +
                    "Acumula al menos 50 ENT totales para activarlo.";
            }
            else if (!canMeta)
            {
                metaWarningText.text =
                    "Progreso insuficiente para ganar Λ.\n" +
                    "Sigue subiendo LE, ENT, ADP y WHF.";
            }
            else
            {
                metaWarningText.text =
                    "Advertencia: esto reseteará LE, EM, IP, ADP, WHF,\n" +
                    "todos los edificios, ENT y sus mejoras.";
            }
        }

        // Habilitar / deshabilitar el botón según si se puede meta-prestigiar
        if (metaPrestigeButton != null)
        {
            metaPrestigeButton.interactable = canMeta;
        }
    }

    private void OnClickMetaPrestige()
    {
        var gs = GameState.I;
        if (gs == null) return;

        double gained = gs.DoMetaPrestigeReset();
        Debug.Log($"[MetaPrestigeUI] Meta-prestigio ejecutado. Λ ganada: {gained}");
    }
}

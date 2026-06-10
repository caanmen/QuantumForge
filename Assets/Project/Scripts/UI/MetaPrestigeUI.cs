using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MetaPrestigeUI : MonoBehaviour
{
    [Header("Textos Λ")]
    public TextMeshProUGUI lambdaActualText;
    public TextMeshProUGUI lambdaGainText;
    public TextMeshProUGUI metaWarningText;

    [Header("Botón")]
    public Button metaPrestigeButton;

    private void Start()
    {
        if (metaPrestigeButton != null)
        {
            metaPrestigeButton.interactable = false;
        }

        RefreshDisabledState();
    }

    private void OnEnable()
    {
        RefreshDisabledState();
    }

    private void RefreshDisabledState()
    {
        GameState gs = GameState.I;

        if (lambdaActualText != null)
        {
            double lambda = gs != null ? gs.Lambda : 0.0;
            lambdaActualText.text = "Λ actual: " + lambda.ToString("0");
        }

        if (lambdaGainText != null)
        {
            lambdaGainText.text = "Prestigio 2 pendiente de rediseño.";
        }

        if (metaWarningText != null)
        {
            metaWarningText.text =
                "Este sistema fue desactivado temporalmente. Prestigio 2 se rediseñará más adelante.";
        }

        if (metaPrestigeButton != null)
        {
            metaPrestigeButton.interactable = false;
        }
    }
}
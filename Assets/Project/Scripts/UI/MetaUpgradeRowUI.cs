using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MetaUpgradeRowUI : MonoBehaviour
{
    public enum MetaUpgradeType
    {
        EmBoost1
    }

    [Header("Config")]
    public MetaUpgradeType upgradeType;
    public double costLambda = 1.0;

    [Header("Referencias UI")]
    public TextMeshProUGUI descriptionText;
    public Button buyButton;
    public TextMeshProUGUI buyButtonLabel;

    private void Start()
    {
        RefreshDisabledState();
    }

    private void OnEnable()
    {
        RefreshDisabledState();
    }

    private void RefreshDisabledState()
    {
        if (descriptionText != null)
        {
            descriptionText.text =
                "Mejora meta desactivada temporalmente. Prestigio 2 se rediseñará más adelante.";
        }

        if (buyButton != null)
        {
            buyButton.interactable = false;
        }

        if (buyButtonLabel != null)
        {
            buyButtonLabel.text = "Bloqueado";
        }
    }
}
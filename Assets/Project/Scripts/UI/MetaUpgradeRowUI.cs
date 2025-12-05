using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MetaUpgradeRowUI : MonoBehaviour
{
    public enum MetaUpgradeType
    {
        EntBoost1,
        EmBoost1
    }

    [Header("Config")]
    public MetaUpgradeType upgradeType;
    public double costLambda = 1.0;   // Coste en Λ (por defecto 1)

    [Header("Referencias UI")]
    public TextMeshProUGUI descriptionText;   // Texto largo del upgrade (opcional)
    public Button buyButton;                  // Botón "Comprar"
    public TextMeshProUGUI buyButtonLabel;    // Texto del botón

    private void Start()
    {
        if (buyButton != null)
            buyButton.onClick.AddListener(OnClickBuy);

        RefreshUI();
    }

    private void Update()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        var gs = GameState.I;
        if (gs == null || buyButton == null || buyButtonLabel == null)
            return;

        bool bought = GetBoughtFlag(gs);

        if (bought)
        {
            buyButton.interactable = false;
            buyButtonLabel.text = "Comprado";
        }
        else
        {
            bool hasLambda = gs.Lambda >= costLambda;
            buyButton.interactable = hasLambda;
            buyButtonLabel.text = hasLambda ? "Comprar" : "Sin Λ";
        }
    }

    private bool GetBoughtFlag(GameState gs)
    {
        switch (upgradeType)
        {
            case MetaUpgradeType.EntBoost1:
                return gs.metaEntBoost1Bought;
            case MetaUpgradeType.EmBoost1:
                return gs.metaEmBoost1Bought;
            default:
                return false;
        }
    }

    private void SetBoughtFlag(GameState gs, bool value)
    {
        switch (upgradeType)
        {
            case MetaUpgradeType.EntBoost1:
                gs.metaEntBoost1Bought = value;
                break;
            case MetaUpgradeType.EmBoost1:
                gs.metaEmBoost1Bought = value;
                break;
        }
    }

    private void OnClickBuy()
    {
        var gs = GameState.I;
        if (gs == null) return;

        // Ya comprado o sin Λ suficiente → no hacer nada
        if (GetBoughtFlag(gs)) return;
        if (gs.Lambda < costLambda) return;

        // Cobrar Λ
        gs.Lambda -= costLambda;

        // Marcar como comprado
        SetBoughtFlag(gs, true);

        // Actualizar UI
        RefreshUI();
    }
}

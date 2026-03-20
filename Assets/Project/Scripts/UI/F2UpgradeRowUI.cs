using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class F2UpgradeRowUI : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private string upgradeId = "emission_focus";

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI tierText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button buyButton;

    private void Awake()
    {
        if (buyButton != null)
        {
            buyButton.onClick.RemoveListener(OnBuyClicked);
            buyButton.onClick.AddListener(OnBuyClicked);
        }
    }

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (F2UpgradeManager.I == null) return;

        var def = F2UpgradeManager.I.GetDef(upgradeId);
        if (def == null) return;

        int bought = F2UpgradeManager.I.GetPurchasedTierCount(upgradeId);
        bool isMaxed = F2UpgradeManager.I.IsMaxed(upgradeId);
        double nextCost = F2UpgradeManager.I.GetNextCost(upgradeId);

        if (titleText != null)
            titleText.text = def.name;

        if (tierText != null)
            tierText.text = isMaxed ? $"Tier: MAX" : $"Tier: {bought}/{def.tiers.Count}";

        if (costText != null)
        {
            if (isMaxed)
                costText.text = "Coste: MAX";
            else
                costText.text = $"Coste: {nextCost:0.##} LE";
        }

        if (buyButton != null)
            buyButton.interactable = !isMaxed && F2UpgradeManager.I.CanBuy(upgradeId);
    }

    private void OnBuyClicked()
    {
        if (F2UpgradeManager.I == null) return;

        bool ok = F2UpgradeManager.I.TryBuy(upgradeId);
        if (ok)
            Refresh();
    }
}
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
    [SerializeField] private TextMeshProUGUI descriptionText;
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

        private string BuildDynamicDescription(F2UpgradeDef def, int purchasedTiers)
    {
        switch (def.id)
        {
            case "emission_focus":
                switch (purchasedTiers)
                {
                    case 0: return "Tier 1: +5% LE";
                    case 1: return "Tier 2: +10% LE";
                    case 2: return "Tier 3: +20% LE";
                    default: return "Máx: +20% LE";
                }

            case "containment_tuning":
                switch (purchasedTiers)
                {
                    case 0: return "Tier 1: +10% Higgs";
                    case 1: return "Tier 2: +20% Higgs";
                    case 2: return "Tier 3: +35% Higgs";
                    default: return "Máx: +35% Higgs";
                }

            case "tetraquark_stabilization":
                switch (purchasedTiers)
                {
                    case 0: return "Tier 1: +10% Tetra";
                    case 1: return "Tier 2: +20% Tetra";
                    case 2: return "Tier 3: +35% Tetra";
                    default: return "Máx: +35% Tetra";
                }

            case "residual_analysis":
                switch (purchasedTiers)
                {
                    case 0: return "Tier 1: +10% Trazas";
                    case 1: return "Tier 2: +20% Trazas";
                    default: return "Máx: +20% Trazas";
                }

            case "pattern_mapping":
                switch (purchasedTiers)
                {
                    case 0: return "Tier 1: +5% LE";
                    case 1: return "Tier 2: +10% LE";
                    default: return "Máx: +10% LE";
                }

            case "triangle_unlock_1":
                return purchasedTiers > 0 ? "Máx: Triángulo activo" : "Tier 1: Desbloquea el triángulo";

            case "triangle_impulse_tuning":
                switch (purchasedTiers)
                {
                    case 0: return "Tier 1: Impulso +16% LE";
                    case 1: return "Tier 2: Impulso +20% LE";
                    default: return "Máx: Impulso +20% LE";
                }

            case "triangle_synergy_resonance":
                switch (purchasedTiers)
                {
                    case 0: return "Tier 1: sinergia Higgs/Tetra +13%, Modulador +8%";
                    case 1: return "Tier 2: sinergia Higgs/Tetra +15%, Modulador +10%";
                    default: return "Máx: sinergia Higgs/Tetra +15%, Modulador +10%";
                }

        case "triangle_persistence_anchor":
            switch (purchasedTiers)
                {
                    case 0: return "Tier 1: carga mas rapida";
                    case 1: return "Tier 2: reserva máxima 4h";
                    default: return "Máx: 3h30m offline = 1h, reserva 4h";
                }

            default:
                return def.description;
        }
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
        {
            string titleKey = $"f2.upgrade.{def.id}.name";
            string localizedTitle = LocalizationManager.I != null ? LocalizationManager.I.T(titleKey) : titleKey;
            titleText.text = localizedTitle == titleKey ? def.name : localizedTitle;
        }

        if (descriptionText != null)
        {
            descriptionText.text = BuildDynamicDescription(def, bought);
        }

        if (tierText != null)
            tierText.text = isMaxed ? $"Tier: MAX" : $"Tier: {bought}/{def.tiers.Count}";

        if (costText != null)
        {
            if (isMaxed)
            {
                costText.text = "Coste: MAX";
            }
            else
            {
                string currencyLabel = def.currency switch
                {
                    F2UpgradeCurrency.LE => "LE",
                    F2UpgradeCurrency.Traces => "Trazas",
                    _ => "?"
                };

                costText.text = $"Coste: {nextCost:0.##} {currencyLabel}";
            }
        }

    }    
    private void OnBuyClicked()
    {
        if (F2UpgradeManager.I == null) return;

        bool ok = F2UpgradeManager.I.TryBuy(upgradeId);
        if (ok)
            Refresh();
    }
}
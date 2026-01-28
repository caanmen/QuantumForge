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
    public double costLambda = 1.0;

    [Header("Referencias UI")]
    public TextMeshProUGUI descriptionText;
    public Button buyButton;
    public TextMeshProUGUI buyButtonLabel;

    private int _lastLang = -1;
    private bool _lastBought;
    private bool _lastHasLambda;

    private void Start()
    {
        if (buyButton != null)
            buyButton.onClick.AddListener(OnClickBuy);

        ForceRefresh();
    }

    private void Update()
    {
        var gs = GameState.I;
        if (gs == null) return;

        var lm = LocalizationManager.I;
        int langNow = (lm != null) ? (int)lm.CurrentLanguage : -1;

        bool bought = GetBoughtFlag(gs);
        bool hasLambda = gs.Lambda >= costLambda;

        bool needsRefresh =
            (langNow != _lastLang) ||
            (bought != _lastBought) ||
            (hasLambda != _lastHasLambda);

        if (!needsRefresh) return;

        _lastLang = langNow;
        _lastBought = bought;
        _lastHasLambda = hasLambda;

        RefreshUI(gs, bought, hasLambda);
    }

    private void ForceRefresh()
    {
        var gs = GameState.I;
        if (gs == null) return;

        _lastLang = -1;
        _lastBought = !GetBoughtFlag(gs);  // fuerza cambio
        _lastHasLambda = !(gs.Lambda >= costLambda);
    }

    private void RefreshUI(GameState gs, bool bought, bool hasLambda)
    {
        // Texto largo (descripción)
        if (descriptionText != null)
        {
            switch (upgradeType)
            {
                case MetaUpgradeType.EntBoost1:
                    descriptionText.text = LF(
                        "meta.upg_ent_boost_desc",
                        "Boost ENT I: +20% ENT ganada permanentemente - Coste: {0:0} Λ",
                        costLambda
                    );
                    break;

                case MetaUpgradeType.EmBoost1:
                    descriptionText.text = LF(
                        "meta.upg_em_boost_desc",
                        "Boost EM I: +15% EM base permanente - Coste: {0:0} Λ",
                        costLambda
                    );
                    break;
            }
        }

        // Botón
        if (buyButton == null || buyButtonLabel == null) return;

        if (bought)
        {
            buyButton.interactable = false;
            buyButtonLabel.text = L("ui.bought", "Comprado");
        }
        else
        {
            buyButton.interactable = hasLambda;
            buyButtonLabel.text = hasLambda ? L("ui.buy", "Comprar") : L("ui.no_lambda", "Sin Λ");
        }
    }

    private bool GetBoughtFlag(GameState gs)
    {
        switch (upgradeType)
        {
            case MetaUpgradeType.EntBoost1: return gs.metaEntBoost1Bought;
            case MetaUpgradeType.EmBoost1:  return gs.metaEmBoost1Bought;
            default: return false;
        }
    }

    private void SetBoughtFlag(GameState gs, bool value)
    {
        switch (upgradeType)
        {
            case MetaUpgradeType.EntBoost1: gs.metaEntBoost1Bought = value; break;
            case MetaUpgradeType.EmBoost1:  gs.metaEmBoost1Bought  = value; break;
        }
    }

    private void OnClickBuy()
    {
        var gs = GameState.I;
        if (gs == null) return;

        if (GetBoughtFlag(gs)) return;
        if (gs.Lambda < costLambda) return;

        gs.Lambda -= costLambda;
        SetBoughtFlag(gs, true);

        ForceRefresh(); // fuerza refresco inmediato
    }

    private string L(string key, string fallback)
    {
        var lm = LocalizationManager.I;
        if (lm == null) return fallback;

        var s = lm.T(key);
        if (string.IsNullOrEmpty(s) || s == key) return fallback;

        return s;
    }

    private string LF(string key, string fallback, params object[] args)
    {
        string fmt = L(key, fallback);
        try { return string.Format(fmt, args); }
        catch { return fmt; }
    }
}

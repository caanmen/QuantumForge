using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResearchItemUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI costText;
    public Button buyButton;
    private string L(string key, string fallback)
    {
        var lm = LocalizationManager.I;
        if (lm == null) return fallback;

        var s = lm.T(key);

        // Si no existe, tu T() devuelve la key
        if (string.IsNullOrEmpty(s) || s == key) return fallback;

        return s;
    }


    private string researchId;
    private ResearchDef def;

    // ✅ Nuevo: refresco por localización y throttling
    private int _lastLocRevision = -1;
    private float _nextRefreshTime = 0f;
    private const float REFRESH_INTERVAL = 0.25f;

    // Llamado desde ResearchUI cuando instanciemos el item
public void Setup(ResearchDef researchDef)
{
    def = researchDef;
    researchId = (def != null) ? def.id : null;

    // Botón
    if (buyButton != null)
    {
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnClickBuy);
    }

    // Primer pintado + sincroniza revision actual
    _lastLocRevision = (LocalizationManager.I != null) ? LocalizationManager.I.Revision : -1;

    RefreshLocalizedTextsOnly();
    RefreshState();
}

    // Llamado desde ResearchUI cuando instanciemos el item
    private void RefreshLocalizedTextsOnly()
{
    if (def == null) return;

    // Nombre (fallback: def.name)
    if (nameText != null)
    {
        string key = $"res.{def.id}.name";
        string s = (LocalizationManager.I != null) ? LocalizationManager.I.T(key) : null;
        nameText.text = (!string.IsNullOrEmpty(s) && s != key) ? s : def.name;
    }

    // Descripción (fallback: def.description)
    if (descText != null)
    {
        string key = $"res.{def.id}.desc";
        string s = (LocalizationManager.I != null) ? LocalizationManager.I.T(key) : null;
        descText.text = (!string.IsNullOrEmpty(s) && s != key) ? s : def.description;
    }
}

private void SyncLocalizationRevisionIfNeeded()
{
    var lm = LocalizationManager.I;
    if (lm == null) return;

    int rev = lm.Revision;
    if (rev != _lastLocRevision)
    {
        _lastLocRevision = rev;

        RefreshLocalizedTextsOnly();
        RefreshState(); // para que “Coste/Cost” + estados cambien también
    }
}

    private void RefreshState()
{
    if (ResearchManager.I == null || def == null || buyButton == null) return;

    // Coste (se actualiza también al cambiar idioma)
    if (costText != null)
    {
        string prefix = L("ui.cost_prefix", "Coste:");
        costText.text = $"{prefix} {def.costIP:0} IP";
    }


    bool purchased = ResearchManager.I.IsPurchased(researchId);
    bool canBuy = ResearchManager.I.CanPurchase(researchId);
    var buttonText = buyButton.GetComponentInChildren<TMP_Text>();

    if (purchased)
    {
        buyButton.interactable = false;
        if (buttonText != null) buttonText.text = L("ui.bought", "Comprado");
        return;
    }

    // Aquí diferenciamos el por qué NO se puede comprar
    bool hasPrereq = true;
    if (!string.IsNullOrEmpty(def.prereqId))
    {
        hasPrereq = ResearchManager.I.IsPurchased(def.prereqId);
    }

    if (!hasPrereq)
    {
        buyButton.interactable = false;
        if (buttonText != null) buttonText.text = L("lab.locked", "Bloqueado");
    }
    else if (GameState.I.IP < def.costIP)
    {
        buyButton.interactable = false;
        if (buttonText != null) buttonText.text = L("lab.no_ip", "Sin IP");
    }
    else
    {
        buyButton.interactable = true;
        if (buttonText != null) buttonText.text = L("lab.buy", "Comprar");
    }

}


    private void OnClickBuy()
    {
        if (ResearchManager.I == null || def == null) return;

        if (ResearchManager.I.TryPurchase(researchId))
        {
            RefreshState();
        }
    }

    private void Update()
{
    if (Time.unscaledTime < _nextRefreshTime) return;
    _nextRefreshTime = Time.unscaledTime + REFRESH_INTERVAL;

    // Si cambió idioma, refresca textos + estados
    SyncLocalizationRevisionIfNeeded();

    // Aunque no cambie idioma, refresca estado (IP/prereq) en vivo
    RefreshState();
}

}

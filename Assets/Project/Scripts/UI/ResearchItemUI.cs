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

    private string researchId;
    private ResearchDef def;

    // Llamado desde ResearchUI cuando instanciemos el item
    public void Setup(ResearchDef def)
    {
        this.def = def;
        this.researchId = def.id;

        if (nameText != null) nameText.text = def.name;
        if (descText != null) descText.text = def.description;
        if (costText != null) costText.text = $"Coste: {def.costIP:0} IP";

        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnClickBuy);
        }

        RefreshState();
    }

    private void RefreshState()
{
    if (ResearchManager.I == null || def == null || buyButton == null) return;

    bool purchased = ResearchManager.I.IsPurchased(researchId);
    bool canBuy = ResearchManager.I.CanPurchase(researchId);
    var buttonText = buyButton.GetComponentInChildren<TMP_Text>();

    if (purchased)
    {
        buyButton.interactable = false;
        if (buttonText != null) buttonText.text = "Comprado";
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
        if (buttonText != null) buttonText.text = "Bloqueado";
    }
    else if (GameState.I.IP < def.costIP)
    {
        buyButton.interactable = false;
        if (buttonText != null) buttonText.text = "Sin IP";
    }
    else
    {
        buyButton.interactable = true;
        if (buttonText != null) buttonText.text = "Comprar";
    }
}


    private void Update()
{
    // Revisamos el estado cada frame para que el botón
    // cambie de Bloqueado/Comprar/Comprado cuando
    // cambian el IP o las investigaciones compradas.
    RefreshState();
}


    private void OnClickBuy()
    {
        if (ResearchManager.I == null || def == null) return;

        if (ResearchManager.I.TryPurchase(researchId))
        {
            RefreshState();
        }
    }
}

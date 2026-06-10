using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeycardPurchaseUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI buttonText;
    public Button buyButton;

    private void Awake()
    {
        AutoBindIfNeeded();
    }

    private void Update()
    {
        RefreshUI();
    }

    public void OnClickBuyKeycard()
    {
        if (GameState.I == null) return;

        bool ok = GameState.I.TryBuyExperimentalChamberKeycard();
        Debug.Log($"[F3] Comprar Keycard => {ok}");

        if (ok)
        {
            TabsUI.Instance?.RefreshRoom2ButtonVisibility();
            TabsUI.Instance?.ShowRoom2();
        }

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (GameState.I == null) return;

        bool unlocked = GameState.I.experimentalChamberUnlocked;
        bool hasReq = GameState.I.HasExperimentalChamberKeycardRequirements();
        bool canBuy = GameState.I.CanBuyExperimentalChamberKeycard();

        if (nameText != null)
            nameText.text = "Keycard de la Cámara Experimental";

        if (descText != null)
        {
            if (unlocked)
            {
                descText.text = "Acceso habilitado al Cuarto 2.";
            }
            else
            {
                descText.text = "Abre la puerta del Cuarto 2.";
            }
        }

        if (costText != null)
        {
            if (unlocked)
                costText.text = "Coste: completado";
            else
                costText.text = "Coste: 150000 LE + 250 Trazas";
        }

        if (buttonText != null)
        {
            if (unlocked)
                buttonText.text = "Abierto";
            else
                buttonText.text = "Comprar";
        }

        if (buyButton != null)
            buyButton.interactable = !unlocked && canBuy;
    }

    private void AutoBindIfNeeded()
    {
        if (nameText == null)
        {
            var t = transform.Find("Name_Text");
            if (t != null) nameText = t.GetComponent<TextMeshProUGUI>();
        }

        if (descText == null)
        {
            var t = transform.Find("Desc_Text");
            if (t != null) descText = t.GetComponent<TextMeshProUGUI>();
        }

        if (costText == null)
        {
            var t = transform.Find("Cost_Text");
            if (t != null) costText = t.GetComponent<TextMeshProUGUI>();
        }

        if (buyButton == null)
        {
            var t = transform.Find("Buy_Button");
            if (t != null) buyButton = t.GetComponent<Button>();
        }

        if (buttonText == null && buyButton != null)
        {
            var t = buyButton.transform.Find("Buy_Text");
            if (t != null) buttonText = t.GetComponent<TextMeshProUGUI>();
        }
    }
}
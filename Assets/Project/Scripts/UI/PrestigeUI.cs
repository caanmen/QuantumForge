using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PrestigeUI : MonoBehaviour
{
    [Header("Textos de transición")]
    public TextMeshProUGUI entActualText;
    public TextMeshProUGUI entGananciaText;

    [Header("Textos viejos / opcionales")]
    public TextMeshProUGUI leMult1Text;
    public TextMeshProUGUI autoBuy1Text;

    [Header("Botones viejos / opcionales")]
    public Button autoBuyToggleButton;
    public TextMeshProUGUI autoBuyToggleLabel;

    [Header("Rendimiento UI")]
    [SerializeField] private float uiRefreshInterval = 0.25f;
    private float _uiTimer = 0f;

    private void Update()
    {
        GameState gs = GameState.I;
        if (gs == null)
            return;

        _uiTimer += Time.unscaledDeltaTime;
        if (_uiTimer < uiRefreshInterval)
            return;

        _uiTimer = 0f;

        bool canPrestige1 = gs.CanDoPrestige1();

        if (entActualText != null)
        {
            entActualText.text =
                "Prestigios 1 realizados: " + gs.prestige1Count;
        }

        if (entGananciaText != null)
        {
            entGananciaText.text = gs.GetPrestige1StatusText();
        }

        if (leMult1Text != null)
        {
            leMult1Text.text =
                "Prestigio 1 reinicia el laboratorio base, pero conserva el conocimiento descubierto.";
        }

        if (autoBuy1Text != null)
        {
            autoBuy1Text.text = canPrestige1
                ? "La convergencia está lista."
                : "Repara la Máquina al 80% y activa el Canal de Convergencia.";
        }

        if (autoBuyToggleButton != null)
        {
            autoBuyToggleButton.gameObject.SetActive(false);
        }

        if (autoBuyToggleLabel != null)
        {
            autoBuyToggleLabel.text = "";
        }
    }

    public void OnClickPrestige()
    {
        GameState gs = GameState.I;
        if (gs == null)
            return;

        if (!gs.CanDoPrestige1())
        {
            Debug.Log("[PrestigeUI] Prestigio 1 todavía no disponible: " + gs.GetPrestige1StatusText());
            return;
        }

        bool ok = gs.DoPrestige1Reset();

        if (ok)
        {
            Debug.Log("[PrestigeUI] Prestigio 1 realizado correctamente.");

            if (TabsUI.Instance != null)
            {
                TabsUI.Instance.RefreshDimension1ButtonVisibility();
                TabsUI.Instance.RefreshDimension2ButtonVisibility();
                TabsUI.Instance.RefreshDimension3ButtonVisibility();
                TabsUI.Instance.RefreshPrestigeButtonVisibility();
            }
        }
    }

    // Métodos viejos: se dejan para no romper botones asignados en la escena.
    public void OnClickToggleAutoBuy()
    {
        Debug.Log("[PrestigeUI] Auto-buy viejo desactivado. ENT ya no se usa para Prestigio 1.");
    }

    public void OnClickBuyLeMult1()
    {
        Debug.Log("[PrestigeUI] Upgrade viejo de ENT desactivado.");
    }

    public void OnClickBuyAutoBuy1()
    {
        Debug.Log("[PrestigeUI] Upgrade viejo de auto-compra desactivado.");
    }
}

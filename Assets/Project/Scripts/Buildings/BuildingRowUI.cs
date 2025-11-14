using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla la fila de UI de un edificio:
/// - Muestra nombre, nivel y coste
/// - Gestiona el botón de compra
/// </summary>
public class BuildingRowUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI costText;
    public Button buyButton;

    // Estado del edificio que esta fila representa
    private BuildingState state;

    // Referencia al GameState para leer y gastar LE
    private GameState gameState;

    /// <summary>
    /// Inicializa la fila con un estado concreto y el GameState.
    /// La llamaremos desde el script que genere la lista.
    /// </summary>
    public void Init(BuildingState state, GameState gameState)
    {
        this.state = state;
        this.gameState = gameState;

        if (buyButton != null)
        {
            buyButton.onClick.RemoveListener(OnBuyClicked);
            buyButton.onClick.AddListener(OnBuyClicked);
        }

        Refresh();
    }

    /// <summary>
    /// Actualiza los textos de la UI según el estado actual.
    /// </summary>
    public void Refresh()
    {
        if (state == null || state.def == null)
            return;

        if (nameText != null)
            nameText.text = state.def.displayName;

        if (levelText != null)
            levelText.text = $"Nivel: {state.level}";

        if (costText != null)
            costText.text = $"Coste: {state.currentCost:0} LE";
    }

    private void Update()
    {
        // Cada frame revisamos si el jugador puede pagar
        if (buyButton != null && state != null && gameState != null)
        {
            buyButton.interactable = state.CanAfford(gameState.LE);
        }
    }

    private void OnBuyClicked()
    {
        if (state == null || gameState == null)
            return;

        // ¿Puede pagar?
        if (!state.CanAfford(gameState.LE))
            return;

        // Pagar coste
        gameState.LE -= state.currentCost;

        // Subir nivel y recalcular coste
        state.OnPurchased();

        // Actualizar textos
        Refresh();
    }
}

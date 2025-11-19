using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla la fila de UI de un edificio:
/// - Muestra nombre, nivel y coste
/// - Gestiona el botón de compra
/// - Respeta los requisitos de desbloqueo (gating suave)
/// </summary>
public class BuildingRowUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI costText;
    public Button buyButton;

    [Header("Estado visual")]
    [Tooltip("CanvasGroup de la fila, para manejar opacidad e interacción cuando está bloqueada.")]
    public CanvasGroup rowCanvasGroup;

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
    /// Actualiza los textos de la UI según el estado actual
    /// y si el edificio está desbloqueado o no.
    /// </summary>
    public void Refresh()
    {
        if (state == null || state.def == null || gameState == null)
            return;

        // Nombre siempre visible
        if (nameText != null)
            nameText.text = state.def.displayName;

        // ¿Está desbloqueado según el GameState?
        bool unlocked = BuildingUnlock.IsUnlocked(state.def);

        if (!unlocked)
    {
        // ---------- MODO BLOQUEADO ----------

        // Visual: fila apagada y sin interacción
        if (rowCanvasGroup != null)
        {
            rowCanvasGroup.alpha = 0.4f;
            rowCanvasGroup.interactable = false;
            rowCanvasGroup.blocksRaycasts = false;
        }

        // Botón desactivado
        if (buyButton != null)
            buyButton.interactable = false;

        // Texto de nivel
        if (levelText != null)
            levelText.text = "Bloqueado";

        // SOLO mostramos requisitos (LE y edificio), sin coste inicial
        if (costText != null)
        {
            string req = BuildRequirementsText(state.def);
            costText.text = req;
        }
    }

        else
        {
            // ---------- MODO DESBLOQUEADO ----------

            // Visual: fila normal
            if (rowCanvasGroup != null)
            {
                rowCanvasGroup.alpha = 1f;
                rowCanvasGroup.interactable = true;
                rowCanvasGroup.blocksRaycasts = true;
            }

            // Botón solo activo si puede pagar
            bool canAfford = state.CanAfford(gameState.LE);
            if (buyButton != null)
                buyButton.interactable = canAfford;

            // Texto de nivel
            if (levelText != null)
                levelText.text = $"Nivel: {state.level}";

            // Coste actual
            if (costText != null)
                costText.text = $"Coste: {state.currentCost:0} LE";
        }
    }

    /// <summary>
    /// Construye un texto corto de requisitos para que quepa mejor en la UI.
    /// Ejemplo: "Req: LE ≥ 5000 · LE/s ≥ 40 · Lab ≥ 3"
    /// </summary>
    private string BuildRequirementsText(BuildingDef def)
    {
        List<string> parts = new List<string>();

        // LE mínima actual
        if (def.unlockMinLE > 0.0)
        {
            parts.Add($"LE ≥ {def.unlockMinLE:0}");
        }

        // SOLO mostramos edificio requerido y nivel (no mostramos LE/s)
        if (!string.IsNullOrEmpty(def.unlockRequireId) && def.unlockRequireLevel > 0)
        {
            string shortName = GetShortReqName(def.unlockRequireId);
            parts.Add($"{shortName} ≥ {def.unlockRequireLevel}");
        }

        if (parts.Count == 0)
            return "Req: progreso";

        // Usamos " · " en vez de coma para que quepa mejor
        return "Req: " + string.Join(" · ", parts);
    }



    /// <summary>
    /// Devuelve un nombre corto para el edificio requerido,
    /// para que el texto no sea tan largo.
    /// </summary>
    private string GetShortReqName(string id)
    {
        switch (id)
        {
            case "vacuum_observer":     return "Obs.";
            case "fluctuation_antenna": return "Ant.";
            case "decoherence_lab":     return "Lab";
            case "vacuum_amplifier":    return "Amp.";
            default:                    return id;
        }
    }

    private void Update()
    {
        // Refrescamos cada frame para que, en cuanto se cumplan los requisitos,
        // la fila pase de "Bloqueado" a normal automáticamente.
        Refresh();
    }

    private void OnBuyClicked()
    {
        if (state == null || gameState == null) return;

        // Por seguridad, no dejar comprar si aún está bloqueado
        if (!BuildingUnlock.IsUnlocked(state.def))
        {
            return;
        }

        // ¿Puede pagar?
        if (!state.CanAfford(gameState.LE))
            return;

        // Pagar coste
        gameState.LE -= state.currentCost;

        // Subir nivel y recalcular coste
        state.OnPurchased();

        // Actualizar textos (nivel, coste, etc.)
        Refresh();
    }
}

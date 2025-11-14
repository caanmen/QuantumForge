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
        if (state == null || state.def == null) return;

        // Nombre siempre visible
        if (nameText != null)
            nameText.text = state.def.displayName;

        // ¿Está desbloqueado según el GameState?
        bool unlocked = BuildingUnlock.IsUnlocked(state.def);

        if (!unlocked)
        {
            // ---------- MODO BLOQUEADO ----------
            if (levelText != null)
                levelText.text = "Bloqueado";

            if (costText != null)
            {
                // Línea 1: coste inicial (para que el jugador sepa cuánto costará comprarlo)
                string lineCost = $"Coste inicial: {state.def.baseCost:0} LE";

                // Línea 2: requisitos comprimidos
                string req = BuildRequirementsText(state.def);

                costText.text = lineCost + "\n" + req;
            }
        }
        else
        {
            // ---------- MODO DESBLOQUEADO ----------
            if (levelText != null)
                levelText.text = $"Nivel: {state.level}";

            if (costText != null)
                costText.text = $"Coste: {state.currentCost:0} LE";
        }
    }

    /// <summary>
    /// Construye un texto corto de requisitos para que quepa mejor en la UI.
    /// Ejemplo: "Req: LE 5000, LE/s 40, Lab 3"
    /// </summary>
    private string BuildRequirementsText(BuildingDef def)
    {
        List<string> parts = new List<string>();

        if (def.unlockMinLE > 0.0)
        {
            parts.Add($"LE {def.unlockMinLE:0}");
        }

        if (def.unlockMinTotalLEps > 0.0)
        {
            parts.Add($"LE/s {def.unlockMinTotalLEps:0}");
        }

        if (!string.IsNullOrEmpty(def.unlockRequireId) && def.unlockRequireLevel > 0)
        {
            string shortName = GetShortReqName(def.unlockRequireId);
            parts.Add($"{shortName} {def.unlockRequireLevel}");
        }

        if (parts.Count == 0)
            return "Req: progreso";

        return "Req: " + string.Join(", ", parts);
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
        if (buyButton == null || state == null || gameState == null) return;

        bool unlocked = BuildingUnlock.IsUnlocked(state.def);

        if (!unlocked)
        {
            // Mientras esté bloqueado, el botón no se puede usar
            buyButton.interactable = false;
            return;
        }

        // Si está desbloqueado, el botón se habilita solo si puede pagar
        buyButton.interactable = state.CanAfford(gameState.LE);
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

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

    [Header("Rendimiento")]
    [SerializeField] private float refreshInterval = 0.25f;
    private float _t;

    // Estado del edificio que esta fila representa
    private BuildingState state;

    // Referencia al GameState para leer y gastar LE
    private GameState gameState;

    // Cache (para no re-asignar texto si no cambió)
    private bool _lastUnlocked;
    private int _lastLevel = -1;
    private double _lastCost = double.NaN;
    private string _lastReqText;

    // Cache de nombre (no cambia)
    private string _cachedName;

    /// <summary>
    /// Inicializa la fila con un estado concreto y el GameState.
    /// La llamaremos desde el script que genere la lista.
    /// </summary>
    public void Init(BuildingState state, GameState gameState)
    {
        this.state = state;
        this.gameState = gameState;

        _cachedName = (state != null && state.def != null) ? state.def.displayName : "";

        if (buyButton != null)
        {
            buyButton.onClick.RemoveListener(OnBuyClicked);
            buyButton.onClick.AddListener(OnBuyClicked);
        }

        // Forzar primer refresh
        _lastUnlocked = false;
        _lastLevel = -1;
        _lastCost = double.NaN;
        _lastReqText = null;

        Refresh();
    }

    private void Update()
    {
        _t += Time.unscaledDeltaTime;
        if (_t < refreshInterval) return;
        _t = 0f;

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

        // Nombre siempre visible (no cambia normalmente)
        if (nameText != null && nameText.text != _cachedName)
            nameText.SetText(_cachedName);

        bool unlocked = BuildingUnlock.IsUnlocked(state.def);

        // Si cambió el estado bloqueado/desbloqueado, ajusta visual una sola vez
        if (unlocked != _lastUnlocked)
        {
            _lastUnlocked = unlocked;

            if (!unlocked)
            {
                // ---------- MODO BLOQUEADO ----------
                if (rowCanvasGroup != null)
                {
                    rowCanvasGroup.alpha = 0.4f;
                    rowCanvasGroup.interactable = false;
                    rowCanvasGroup.blocksRaycasts = false;
                }

                if (buyButton != null)
                    buyButton.interactable = false;

                if (levelText != null)
                    levelText.SetText("Bloqueado");

                // Requisitos
                _lastReqText = BuildRequirementsText(state.def);
                if (costText != null)
                    costText.SetText(_lastReqText);

                return; // ya está todo para modo bloqueado
            }
            else
            {
                // ---------- MODO DESBLOQUEADO ----------
                if (rowCanvasGroup != null)
                {
                    rowCanvasGroup.alpha = 1f;
                    rowCanvasGroup.interactable = true;
                    rowCanvasGroup.blocksRaycasts = true;
                }
            }
        }

        if (!unlocked)
        {
            // Sigue bloqueado: por si cambian requisitos con el tiempo (raro, pero posible)
            string req = BuildRequirementsText(state.def);
            if (_lastReqText != req)
            {
                _lastReqText = req;
                if (costText != null) costText.SetText(req);
            }
            return;
        }

        // ---------- MODO DESBLOQUEADO ----------
        // Botón activo si puede pagar (esto sí cambia seguido)
        bool canAfford = state.CanAfford(gameState.LE);
        if (buyButton != null)
            buyButton.interactable = canAfford;

        // Nivel (solo si cambió)
        if (state.level != _lastLevel)
        {
            _lastLevel = state.level;
            if (levelText != null)
                levelText.SetText("Nivel: {0}", _lastLevel);
        }

        // Coste (solo si cambió)
        if (!NearlyEqual(state.currentCost, _lastCost))
        {
            _lastCost = state.currentCost;
            if (costText != null)
                costText.SetText("Coste: {0:0} LE", (float)_lastCost);
        }
    }

    private static bool NearlyEqual(double a, double b)
    {
        if (double.IsNaN(a) || double.IsNaN(b)) return false;
        return System.Math.Abs(a - b) < 0.0001;
    }

    /// <summary>
    /// Construye un texto corto de requisitos para que quepa mejor en la UI.
    /// Ejemplo: "Req: LE ≥ 5000 · Obs. ≥ 8"
    /// </summary>
    private string BuildRequirementsText(BuildingDef def)
    {
        List<string> parts = new List<string>();

        if (def.unlockMinLE > 0.0)
            parts.Add($"LE ≥ {def.unlockMinLE:0}");

        if (!string.IsNullOrEmpty(def.unlockRequireId) && def.unlockRequireLevel > 0)
        {
            string shortName = GetShortReqName(def.unlockRequireId);
            parts.Add($"{shortName} ≥ {def.unlockRequireLevel}");
        }

        if (parts.Count == 0)
            return "Req: progreso";

        return "Req: " + string.Join(" · ", parts);
    }

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

    private void OnBuyClicked()
    {
        if (state == null || gameState == null) return;

        // Por seguridad, no dejar comprar si aún está bloqueado
        if (!BuildingUnlock.IsUnlocked(state.def))
            return;

        // ¿Puede pagar?
        if (!state.CanAfford(gameState.LE))
            return;

        // Pagar coste
        gameState.LE -= state.currentCost;

        // Subir nivel y recalcular coste
        state.OnPurchased();

        // Forzar refresh de nivel/coste
        _lastLevel = -1;
        _lastCost = double.NaN;

        Refresh();
    }
}

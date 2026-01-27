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
    public TextMeshProUGUI statsText;
    public Image tickFill;



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

    // Cache stats (para no recalcular / reescribir si no cambió)
    private int _lastCasimirLevel = -1;
    private int _lastLang = -999;

    private double _lastStatsLeTick = double.NaN;
    private double _lastStatsEmTick = double.NaN;
    private double _lastStatsIpTick = double.NaN;
    private double _lastStatsInterval = double.NaN;
    private double _lastStatsWorld = double.NaN;


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
        _lastUnlocked = true;
        _lastLevel = -1;
        _lastCost = double.NaN;
        _lastReqText = null;
        _lastCasimirLevel = -1;
        _lastStatsLeTick = double.NaN;
        _lastStatsEmTick = double.NaN;
        _lastStatsIpTick = double.NaN;
        _lastStatsInterval = double.NaN;
        _lastStatsWorld = double.NaN;


        Refresh();
    }

    private void Update()
    {

        UpdateTickBar();

        _t += Time.unscaledDeltaTime;
        if (_t < refreshInterval) return;
        _t = 0f;

        Refresh();
    }
    
    private void UpdateTickBar()
    {
        if (tickFill == null || state == null || state.def == null) return;

            bool unlocked = BuildingUnlock.IsUnlocked(state.def);


        // Ocultar si no produce todavía
        if (state.level <= 0 || state.def.tickInterval <= 0.0)
        {
            tickFill.transform.parent.gameObject.SetActive(false);
            return;
        }

        tickFill.transform.parent.gameObject.SetActive(true);

        float interval = (float)state.def.tickInterval;
        float p = Mathf.Clamp01((float)(state.tickTimer / interval));
        tickFill.fillAmount = p;


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
                    {
                        levelText.SetText(LocalizationManager.I != null
                            ? LocalizationManager.I.T("ui.locked")
                            : "Locked");
                    }


                // Requisitos
                _lastReqText = BuildRequirementsText(state.def);
                if (costText != null)
                    costText.SetText(_lastReqText);
                
                if (statsText != null) statsText.gameObject.SetActive(false);


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
            if (statsText != null) statsText.gameObject.SetActive(false);


            return;
        }

        // ---------- MODO DESBLOQUEADO ----------
        // Botón activo si puede pagar (esto sí cambia seguido)
        bool canAfford = state.CanAfford(gameState.LE);
        if (buyButton != null)
            buyButton.interactable = canAfford;

        int langNow = (LocalizationManager.I != null) ? (int)LocalizationManager.I.CurrentLanguage : -1;
        bool langChanged = (langNow != _lastLang);
        if (langChanged) _lastLang = langNow;


        // Nivel (solo si cambió)
        // Nivel (actualiza si cambió el nivel o el idioma)
        if (state.level != _lastLevel || langChanged)
        {
            _lastLevel = state.level;

            if (levelText != null)
            {
                string lvl = (LocalizationManager.I != null)
                    ? LocalizationManager.I.T("ui.level_prefix")
                    : "Level:";

                levelText.SetText($"{lvl} {_lastLevel}");

            }
        }




        // Coste (solo si cambió)
        // Coste (actualiza si cambió el coste O cambió el idioma)
        if (!NearlyEqual(state.currentCost, _lastCost) || langChanged)
            {
                _lastCost = state.currentCost;

                if (costText != null)
                {
                    string costPrefix = (LocalizationManager.I != null)
                        ? LocalizationManager.I.T("ui.cost_prefix")
                        : "Cost:";

                    costText.SetText($"{costPrefix} {(float)_lastCost:0} LE");
                }
            }



        
        UpdateStatsText();
        UpdateTickBar();


    }

    private static bool NearlyEqual(double a, double b)
    {
        if (double.IsNaN(a) || double.IsNaN(b)) return false;
        return System.Math.Abs(a - b) < 0.0001;
    }

    private void UpdateStatsText()
    {
        if (statsText == null || state == null || state.def == null || gameState == null)
            return;

        var def = state.def;

        // Si el edificio no es de ticks, no mostramos nada (por ahora)
        if (def.tickInterval <= 0.0)
        {
            statsText.gameObject.SetActive(false);
            return;
        }

        statsText.gameObject.SetActive(true);

        double interval = def.tickInterval;

        // Multiplicadores globales (alineados con tu producción real)
        double achFactor = (AchievementManager.I != null) ? AchievementManager.I.GetGlobalLEFactor() : 1.0;
        double worldMult = (1.0 + gameState.emMult) * gameState.researchGlobalLEMult * achFactor;

        // Buff local (Casimir -> Vacuum Observer)
        int casimirLevel = 0;
        double localBuff = 1.0;
        if (def.id == "vacuum_observer")
        {
            casimirLevel = gameState.GetBuildingLevel("casimir_panel");
            if (casimirLevel > 0)
                localBuff *= (1.0 + 0.02 * casimirLevel);
        }

        // LE real por tick (base * nivel * buffs)
        double baseLeTick = def.lePerTickBase * state.level;
        double leTickReal = baseLeTick * localBuff * worldMult;

        // EM/IP por tick (si aplica)
        double emTick = 0.0;
        double ipTick = 0.0;

        if (def.emPerTickBase > 0.0)
        {
            double emGenFactor = 1.0;
            if (ResearchManager.I != null)
                emGenFactor *= ResearchManager.I.GetEMGenerationFactor();

            emGenFactor *= gameState.GetMetaEMGenerationMultiplier();

            emTick = def.emPerTickBase * state.level * emGenFactor;
            ipTick = emTick * 0.1;
        }

        // Si no cambió nada relevante, no reescribas texto
        if (NearlyEqual(interval, _lastStatsInterval) &&
            NearlyEqual(leTickReal, _lastStatsLeTick) &&
            NearlyEqual(emTick, _lastStatsEmTick) &&
            NearlyEqual(ipTick, _lastStatsIpTick) &&
            NearlyEqual(worldMult, _lastStatsWorld) &&
            casimirLevel == _lastCasimirLevel)
        {
            return;
        }

        _lastStatsInterval = interval;
        _lastStatsLeTick = leTickReal;
        _lastStatsEmTick = emTick;
        _lastStatsIpTick = ipTick;
        _lastStatsWorld = worldMult;
        _lastCasimirLevel = casimirLevel;

        // Texto final
        if (def.id == "vacuum_observer" && casimirLevel > 0)
        {
            // muestra el buff de Casimir
            if (def.emPerTickBase > 0.0)
                statsText.SetText(
                    "Tick: +{0:0.00} LE / {1:0.0}s (Casimir x{2:0.00})\n+{3:0.00} EM  +{4:0.00} IP",
                    (float)leTickReal, (float)interval, (float)localBuff, (float)emTick, (float)ipTick
                );
            else
                statsText.SetText(
                    "Tick: +{0:0.00} LE / {1:0.0}s (Casimir x{2:0.00})",
                    (float)leTickReal, (float)interval, (float)localBuff
                );
        }
        else
        {
            if (def.emPerTickBase > 0.0)
                statsText.SetText(
                    "Tick: +{0:0.00} LE / {1:0.0}s\n+{2:0.00} EM  +{3:0.00} IP",
                    (float)leTickReal, (float)interval, (float)emTick, (float)ipTick
                );
            else
                statsText.SetText(
                    "Tick: +{0:0.00} LE / {1:0.0}s",
                    (float)leTickReal, (float)interval
                );
        }

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
        _lastCasimirLevel = -1;
        _lastStatsLeTick = double.NaN;
        _lastStatsEmTick = double.NaN;
        _lastStatsIpTick = double.NaN;
        _lastStatsInterval = double.NaN;
        _lastStatsWorld = double.NaN;


        Refresh();
    }
}

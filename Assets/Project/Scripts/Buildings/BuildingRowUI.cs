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

    private string _cachedReqText = null;

    // Cache de nombre (no cambia)
    private string _cachedName;

    // Cache stats (para no recalcular / reescribir si no cambió)
    private int _lastLang = -999;

    private double _lastStatsLeTick = double.NaN;
    private double _lastStatsEmTick = double.NaN;
    private double _lastStatsInterval = double.NaN;
    private double _lastStatsWorld = double.NaN;
    private double _lastStatsTracesPs = double.NaN;
    private float _lastExpansionBonus = float.NaN;
    private float _lastModulatorCalibration = float.NaN;
    private int _lastModulatorMode = -999;
    private float _lastConservationDiscount = float.NaN;
    private int _lastStatsLang = -999;


    private string GetLocalizedBuildingName()
    {
        if (state == null || state.def == null) return "";

        string fallback = state.def.displayName ?? "";
        string key = $"bld.{state.def.id}.name";

        var lm = LocalizationManager.I;
        if (lm == null) return fallback;

        string s = lm.T(key);

        // Si no existe, tu T() devuelve la misma key
        if (string.IsNullOrEmpty(s) || s == key) return fallback;

        return s;
    }


    /// <summary>
    /// Inicializa la fila con un estado concreto y el GameState.
    /// La llamaremos desde el script que genere la lista.
    /// </summary>
    public void Init(BuildingState state, GameState gameState)
    {
        this.state = state;
        this.gameState = gameState;

        _cachedName = GetLocalizedBuildingName();

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
        _lastStatsLeTick = double.NaN;
        _lastStatsEmTick = double.NaN;
        _lastStatsInterval = double.NaN;
        _lastStatsTracesPs = double.NaN;
        _lastStatsWorld = double.NaN;
        _lastExpansionBonus = float.NaN;
        _lastModulatorCalibration = float.NaN;
        _lastModulatorMode = -999;
        _lastConservationDiscount = float.NaN;

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

        interval = Mathf.Max(0.0001f, interval);

        float p = Mathf.Clamp01((float)(state.tickTimer / interval));
        tickFill.fillAmount = p;


    }

    private void RequestLayoutRefresh()
    {
        RectTransform rt = transform as RectTransform;
        if (rt == null) return;

        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);

        RectTransform parentRt = transform.parent as RectTransform;
        if (parentRt != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentRt);
        }
    }


    /// <summary>
    /// Actualiza los textos de la UI según el estado actual
    /// y si el edificio está desbloqueado o no.
    /// </summary>
    public void Refresh()
    {
        if (state == null || state.def == null || gameState == null)
            return;

        int langNow = (LocalizationManager.I != null) ? (int)LocalizationManager.I.CurrentLanguage : -1;
        bool langChanged = (langNow != _lastLang);
        if (langChanged) _lastLang = langNow;

        if (langChanged)
        _cachedName = GetLocalizedBuildingName();

        if (langChanged) _cachedReqText = null; // para que "Req:" se regenere en el nuevo idioma


        // Nombre visible
        if (nameText != null)
        {
            string shownName = _cachedName;

            if (state != null && state.def != null)
            {
                if (state.def.id == "fluctuation_antenna" && gameState != null)
                {
                    int pct = Mathf.RoundToInt(gameState.triangleSynchronization * 100f);
                    shownName = $"{_cachedName} — {pct}%";
                }
                else
                {
                    shownName = $"{_cachedName} — Nv. {state.level}";
                }
            }

            if (nameText.text != shownName)
                nameText.SetText(shownName);
        }

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
                RequestLayoutRefresh();
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

            // Los requisitos NO cambian con el tiempo, solo con idioma (y ni eso todavía)
            if (_cachedReqText == null)
            {
                _cachedReqText = BuildRequirementsText(state.def);

                if (costText != null)
                    costText.SetText(_cachedReqText);
            }


            if (statsText != null) statsText.gameObject.SetActive(false);
            RequestLayoutRefresh();
            return;
        }


        // ---------- MODO DESBLOQUEADO ----------
        double effectiveCost = gameState.GetEffectiveBuildingCost(state);
        bool canAfford = !state.IsAtMaxLevel() && gameState.LE >= effectiveCost;

        if (buyButton != null)
        {
            if (state.def.id == "fluctuation_antenna" && state.level > 0)
                buyButton.interactable = false;
            else
                buyButton.interactable = canAfford;
        }

        RefreshBuyButtonLabel();


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




        if (!NearlyEqual(effectiveCost, _lastCost) || langChanged)
        {
            _lastCost = effectiveCost;

            if (costText != null)
            {
                // Se mantiene actualizado internamente, pero ya no lo mostramos visualmente
                costText.SetText(string.Empty);
            }
        }

       
        UpdateStatsText();
        UpdateTickBar();
        RequestLayoutRefresh();

    }

    private static bool NearlyEqual(double a, double b)
    {
        if (double.IsNaN(a) || double.IsNaN(b)) return false;
        return System.Math.Abs(a - b) < 0.0001;
    }

    private string L(string key, string fallback)
    {
        var lm = LocalizationManager.I;
        if (lm == null) return fallback;
        var s = lm.T(key);
        return string.IsNullOrEmpty(s) ? fallback : s;
    }

    private string LF(string key, string fallback, params object[] args)
    {
        string fmt = L(key, fallback);
        try { return string.Format(fmt, args); }
        catch { return fmt; }
    }

    private string GetPhaseModulatorModeLabel()
    {
        if (gameState == null) return "Sin circuito";

        switch (gameState.triangleActiveCircuit)
        {
            case TriangleCircuitType.Energy:
                return "Energía";
            case TriangleCircuitType.Experimental:
                return "Experimental";
            case TriangleCircuitType.Phase:
                return gameState.IsTrianglePhaseUnlocked() ? "Fase" : "Fase bloqueado";
            default:
                return "Sin circuito";
        }
    }

    private string GetPhaseModulatorDescription()
    {
        if (gameState == null) return "Sin circuito seleccionado.";

        switch (gameState.triangleActiveCircuit)
        {
            case TriangleCircuitType.Energy:
                return "Energía: prioriza LE.";
            case TriangleCircuitType.Experimental:
                return "Experimental: prioriza Trazas y fragmentos.";
            case TriangleCircuitType.Phase:
                return gameState.IsTrianglePhaseUnlocked()
                    ? "Fase: acelera análisis y rutinas compatibles."
                    : "Fase: bloqueado hasta desbloquear la Máquina.";
            default:
                return "Sin circuito seleccionado.";
        }
    }

        private string GetBuyButtonLabel()
    {
        if (state == null || state.def == null)
            return (LocalizationManager.I != null)
                ? LocalizationManager.I.T("ui.buy")
                : "Buy";

        bool modulatorBought =
            state.def.id == "fluctuation_antenna" &&
            state.level > 0;

        if (modulatorBought)
            return "Gestionar en el Triángulo";

        return (LocalizationManager.I != null)
            ? LocalizationManager.I.T("ui.buy")
            : "Buy";
    }

        private void RefreshBuyButtonLabel()
    {
        if (buyButton == null) return;

        var label = buyButton.GetComponentInChildren<TextMeshProUGUI>();
        if (label == null) return;

        string desired = GetBuyButtonLabel();
        if (label.text != desired)
            label.SetText(desired);
    }

        private double GetEffectiveShownInterval(double baseInterval)
    {
        if (gameState == null) return baseInterval;

        double effectiveInterval = baseInterval;

        float expansionBonus = gameState.GetPhaseModulatorExpansionTickBonus();
        if (expansionBonus > 0f)
        {
            effectiveInterval *= (1.0 - expansionBonus);
        }

        double devMult = (TickSystem.I != null) ? TickSystem.I.devMultiplier : 1.0;
        if (devMult <= 0.0) devMult = 1.0;

        return effectiveInterval / devMult;
    }

        private void UpdateStatsText()
{
    if (statsText == null || state == null || state.def == null || gameState == null)
        return;

    var def = state.def;

    string costPrefix = (LocalizationManager.I != null)
    ? LocalizationManager.I.T("ui.cost_prefix")
    : "Cost:";

    string costLine = $"{costPrefix} {(float)gameState.GetEffectiveBuildingCost(state):0} LE";

    if (def.tickInterval <= 0.0)
    {
        statsText.gameObject.SetActive(false);
        return;
    }

    statsText.gameObject.SetActive(true);

    double interval = def.tickInterval;
    double shownInterval = GetEffectiveShownInterval(interval);

    double achFactor = (AchievementManager.I != null)
        ? AchievementManager.I.GetGlobalLEFactor()
        : 1.0;

    double worldMult = (1.0 + gameState.emMult) * gameState.researchGlobalLEMult * achFactor;

    double baseLeTick = def.lePerTickBase * state.level;

    if (F2UpgradeManager.I != null)
    {
        // Ajuste de Contención -> Condensador de Higgs
        if (def.id == "vacuum_observer")
        {
            baseLeTick *= (1.0 + F2UpgradeManager.I.GetContainmentTuningBonus());
        }

        // Estabilización Tetraquark -> puente actual del Núcleo Tetraquark
        if (def.id == "casimir_panel")
        {
            baseLeTick *= (1.0 + F2UpgradeManager.I.GetTetraquarkStabilizationBonus());
        }
    }

    // Sinergia del triángulo para Higgs / Tetra
    baseLeTick *= gameState.GetTriangleSynergyBuildingMultiplier(def.id);

    // Persistencia nueva por reserva activa
    baseLeTick *= gameState.GetTrianglePersistenceReserveBuildingMultiplier(def.id);

    double leTickReal = baseLeTick * worldMult;
    leTickReal *= gameState.GetTriangleLEMultiplier();

    double emTick = 0.0;
    double tracesPs = 0.0;

    if (def.id == "casimir_panel")
    {
        double tracesPerTick = 0.03 * state.level;

        if (F2UpgradeManager.I != null)
        {
            tracesPerTick *= (1.0 + F2UpgradeManager.I.GetResidualAnalysisBonus());
        }

        tracesPerTick *= gameState.GetTriangleTracesMultiplier();
        tracesPs = shownInterval > 0.0 ? (tracesPerTick / shownInterval) : 0.0;
    }

    if (def.emPerTickBase > 0.0)
    {
        double emGenFactor = 1.0;
        if (ResearchManager.I != null)
            emGenFactor *= ResearchManager.I.GetEMGenerationFactor();

        emGenFactor *= gameState.GetMetaEMGenerationMultiplier();

        emTick = def.emPerTickBase * state.level * emGenFactor;
    }

        int langNow = (LocalizationManager.I != null) ? (int)LocalizationManager.I.CurrentLanguage : -1;
        float expansionBonus = (gameState != null) ? gameState.GetPhaseModulatorExpansionTickBonus() : 0f;
        float modulatorCalibration = (gameState != null) ? gameState.phaseModulatorCalibration : 0f;
        int modulatorMode = (gameState != null) ? (int)gameState.phaseModulatorMode : -1;
        float conservationDiscount = (gameState != null) ? gameState.GetPhaseModulatorConservationDiscount() : 0f;

        if (NearlyEqual(shownInterval, _lastStatsInterval) &&
            NearlyEqual(leTickReal, _lastStatsLeTick) &&
            NearlyEqual(emTick, _lastStatsEmTick) &&
            NearlyEqual(tracesPs, _lastStatsTracesPs) &&
            NearlyEqual(worldMult, _lastStatsWorld) &&
            Mathf.Abs(expansionBonus - _lastExpansionBonus) < 0.0001f &&
            Mathf.Abs(modulatorCalibration - _lastModulatorCalibration) < 0.0001f &&
            modulatorMode == _lastModulatorMode &&
            Mathf.Abs(conservationDiscount - _lastConservationDiscount) < 0.0001f &&
            langNow == _lastStatsLang)
        {
            return;
        }

        _lastStatsInterval = shownInterval;
        _lastStatsLeTick = leTickReal;
        _lastStatsEmTick = emTick;
        _lastStatsTracesPs = tracesPs;
        _lastStatsWorld = worldMult;
        _lastExpansionBonus = expansionBonus;
        _lastModulatorCalibration = modulatorCalibration;
        _lastModulatorMode = modulatorMode;
        _lastConservationDiscount = conservationDiscount;
        _lastStatsLang = langNow;


    if (def.emPerTickBase > 0.0)
    {
        statsText.SetText(
            $"{costLine}\nTick: +{(float)leTickReal:0.00} LE / {(float)shownInterval:0.0}s\n+{(float)emTick:0.00} EM"
        );
    }
        else if (def.id == "casimir_panel")
        {
            statsText.SetText(
                $"{costLine}\nTick: +{(float)leTickReal:0.00} LE / {(float)shownInterval:0.00}s\n+{(float)tracesPs:0.00} Trazas/s"
            );
        }
    else if (def.id == "fluctuation_antenna")
    {
        int pct = Mathf.RoundToInt(gameState.triangleSynchronization * 100f);

        string modeLabel = GetPhaseModulatorModeLabel();

        statsText.SetText(
            $"{costLine}\n" +
            $"Circuito: {modeLabel} | Sincronización: {pct}%"
        );
    }
    else
    {
        statsText.SetText(
            $"{costLine}\nTick: +{(float)leTickReal:0.00} LE / {(float)shownInterval:0.00}s"
        );
    }
}
    


    /// <summary>
    /// Construye un texto corto de requisitos para que quepa mejor en la UI.
    /// Ejemplo: "Req: LE ≥ 5000 · Obs. ≥ 8"
    /// </summary>
    private string BuildRequirementsText(BuildingDef def)
    {
        // Prefijos localizados
        string prefix = (LocalizationManager.I != null)
            ? LocalizationManager.I.T("ui.req_prefix")
            : "Req:";

        string progress = (LocalizationManager.I != null)
            ? LocalizationManager.I.T("ui.req_progress")
            : "progress";

        string leLabel = (LocalizationManager.I != null) ? LocalizationManager.I.T("ui.req_le_label") : "LE";
        string ge      = (LocalizationManager.I != null) ? LocalizationManager.I.T("ui.req_ge") : "≥";
        string sep     = (LocalizationManager.I != null) ? LocalizationManager.I.T("ui.req_sep") : " · ";


        // Asegurar espacio después del prefijo
        if (!prefix.EndsWith(" ")) prefix += " ";

        bool hasAny = false;
        string s = prefix;

        if (def.unlockMinLE > 0.0)
        {
            hasAny = true;
            s += leLabel + " " + ge + " " + def.unlockMinLE.ToString("0");
        }

        if (!string.IsNullOrEmpty(def.unlockRequireId) && def.unlockRequireLevel > 0)
        {
            string shortName = GetShortReqName(def.unlockRequireId);
            if (hasAny) s += sep;
            hasAny = true;
            s += shortName + " " + ge + " " + def.unlockRequireLevel;
        }


        if (!hasAny)
            return prefix + progress;

        return s;
    }


        private string GetShortReqName(string id)
    {
        switch (id)
        {
            case "vacuum_observer":     return "Higgs";
            case "casimir_panel":       return "Tetra";
            case "fluctuation_antenna": return "Modulador";
            default:                    return id;
        }
    }

        private void CyclePhaseModulatorMode()
    {
        if (gameState == null) return;
        if (!gameState.IsPhaseModulatorOwned()) return;

        PhaseModulatorMode nextMode = gameState.phaseModulatorMode;

        switch (gameState.phaseModulatorMode)
        {
            case PhaseModulatorMode.None:
                nextMode = PhaseModulatorMode.Expansion;
                break;
            case PhaseModulatorMode.Expansion:
                nextMode = PhaseModulatorMode.Conservation;
                break;
            case PhaseModulatorMode.Conservation:
                nextMode = gameState.IsAttunementUnlocked()
                    ? PhaseModulatorMode.Attunement
                    : PhaseModulatorMode.None;
                break;
            case PhaseModulatorMode.Attunement:
                nextMode = PhaseModulatorMode.None;
                break;
            default:
                nextMode = PhaseModulatorMode.None;
                break;
        }

        gameState.SetPhaseModulatorMode(nextMode);
        D3ConsoleSystem.RecordManualModulatorMode(gameState, nextMode);

        _lastStatsLeTick = double.NaN;
        _lastStatsEmTick = double.NaN;
        _lastStatsTracesPs = double.NaN;
        _lastStatsInterval = double.NaN;
        _lastStatsWorld = double.NaN;
        _lastExpansionBonus = float.NaN;
        _lastModulatorCalibration = float.NaN;
        _lastModulatorMode = -999;
        _lastConservationDiscount = float.NaN;

        Refresh();
    }

    private void OnBuyClicked()
    {
        if (state == null || gameState == null) return;

        // Por seguridad, no dejar comprar si aún está bloqueado
        if (!BuildingUnlock.IsUnlocked(state.def))
            return;

        // El Modulador es un vértice fijo; los circuitos se cambian en el Triángulo.
        if (state.def.id == "fluctuation_antenna" && state.level > 0)
        {
            return;
        }

        if (state.IsAtMaxLevel())
            return;

        double effectiveCost = gameState.GetEffectiveBuildingCost(state);

        // ¿Puede pagar?
        if (gameState.LE < effectiveCost)
            return;

        // Pagar coste efectivo
        gameState.LE -= effectiveCost;

        // Subir nivel y recalcular coste
        state.OnPurchased();
        D3ConsoleSystem.RecordManualBuildingPurchase(gameState, state.def.id);

        // Forzar refresh de nivel/coste
        _lastLevel = -1;
        _lastCost = double.NaN;
        _lastStatsLeTick = double.NaN;
        _lastStatsEmTick = double.NaN;
        _lastStatsTracesPs = double.NaN;
        _lastStatsInterval = double.NaN;
        _lastStatsWorld = double.NaN;

        Refresh();
    }

}

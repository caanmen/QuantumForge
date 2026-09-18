using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class F2UpgradeRowUI : MonoBehaviour
{
    private const float RefreshInterval = 0.20f;

    [Header("Config")]
    [SerializeField] private string upgradeId = "emission_focus";
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI tierText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button buyButton;

    public string UpgradeId => upgradeId;
    public TextMeshProUGUI TitleText => titleText;
    public TextMeshProUGUI TierText => tierText;
    public TextMeshProUGUI CostText => costText;
    public TextMeshProUGUI DescriptionText => descriptionText;
    public Button BuyButton => buyButton;

    private float _nextRefresh;
    private VerticalUiTheme _verticalTheme;

    private void Awake()
    {
        VerticalUiSkinRoot skin = GetComponentInParent<VerticalUiSkinRoot>(true);
        _verticalTheme = skin != null ? skin.theme : null;
        if (buyButton != null)
        {
            buyButton.onClick.RemoveListener(OnBuyClicked);
            buyButton.onClick.AddListener(OnBuyClicked);
            MobileQaFriendlyLayout.ConfigureButtonForMobile(buyButton);
        }
    }

    private void OnEnable()
    {
        _nextRefresh = 0f;
        RefreshNow();
    }

    private void Update()
    {
        if (Time.unscaledTime < _nextRefresh) return;
        _nextRefresh = Time.unscaledTime + RefreshInterval;
        RefreshNow();
    }

    public void Configure(
        string id,
        TextMeshProUGUI title,
        TextMeshProUGUI tier,
        TextMeshProUGUI cost,
        TextMeshProUGUI description,
        Button button)
    {
        upgradeId = id;
        titleText = title;
        tierText = tier;
        costText = cost;
        descriptionText = description;
        buyButton = button;
    }

    private string L(string key, string fallback)
    {
        if (LocalizationManager.I == null) return fallback;
        string value = LocalizationManager.I.T(key);
        return string.IsNullOrEmpty(value) || value == key ? fallback : value;
    }

    private string LF(string key, string fallback, params object[] args)
    {
        try { return string.Format(L(key, fallback), args); }
        catch { return fallback; }
    }

    private string B(string es, string en) =>
        LocalizationManager.I != null && LocalizationManager.I.CurrentLanguage == LocalizationManager.Language.EN ? en : es;

    private string Describe(int tier)
    {
        switch (upgradeId)
        {
            case "emission_focus": return tier == 0 ? B("Actual: +0% LE · Siguiente: +5%", "Current: +0% LE · Next: +5%") : tier == 1 ? B("Actual: +5% LE · Siguiente: +15%", "Current: +5% LE · Next: +15%") : B("Total: +15% LE", "Total: +15% LE");
            case "containment_tuning": return tier == 0 ? B("Higgs: ciclo 1,0 s → 0,9 s", "Higgs: cycle 1.0 s → 0.9 s") : tier == 1 ? B("Higgs: ciclo 0,9 s → 0,8 s", "Higgs: cycle 0.9 s → 0.8 s") : B("Higgs: ciclo 0,8 s", "Higgs: 0.8 s cycle");
            case "tetraquark_stabilization": return tier == 0 ? B("Actual: +0% Trazas · Siguiente: +15%", "Current: +0% Traces · Next: +15%") : tier == 1 ? B("Actual: +15% Trazas · Siguiente: +35%", "Current: +15% Traces · Next: +35%") : B("Total: +35% Trazas", "Total: +35% Traces");
            case "triangle_unlock_1": return tier > 0 ? B("Triángulo activo.", "Triangle active.") : B("Activa el Triángulo conectando sus tres vértices.", "Activate the Triangle by connecting its three vertices.");
            case "triangle_impulse_tuning": return tier == 0 ? B("LE: +12% → +16%; escala con la sincronización", "LE: +12% → +16%; scales with synchronization") : tier == 1 ? B("LE: +16% → +20%; escala con la sincronización", "LE: +16% → +20%; scales with synchronization") : B("LE: +20%; escala con la sincronización", "LE: +20%; scales with synchronization");
            case "triangle_synergy_resonance": return tier == 0 ? B("Trazas totales: +0% → +13%; siempre activa", "Total Traces: +0% → +13%; always active") : tier == 1 ? B("Trazas totales: +13% → +15%; siempre activa", "Total Traces: +13% → +15%; always active") : B("Trazas totales: +15%; siempre activa", "Total Traces: +15%; always active");
            case "triangle_persistence_anchor": return tier == 0 ? B("Próximos cambios: sincronización inicial 50% → 65%", "Future switches: starting synchronization 50% → 65%") : tier == 1 ? B("Próximos cambios: sincronización inicial 65% → 80%", "Future switches: starting synchronization 65% → 80%") : B("Próximos cambios: sincronización inicial 80%", "Future switches: starting synchronization 80%");
            case "triangle_energy_efficiency": return tier == 0 ? B("Energía: +0% → +20%", "Energy: +0% → +20%") : tier == 1 ? B("Energía: +20% → +40%", "Energy: +20% → +40%") : tier == 2 ? B("Energía: +40% → +65%", "Energy: +40% → +65%") : B("Energía total: +65%", "Total Energy: +65%");
            default: return F2UpgradeManager.I.GetDef(upgradeId)?.description ?? string.Empty;
        }
    }

    private string NextBenefitLabel(int bought)
    {
        switch (upgradeId)
        {
            case "emission_focus":
                return bought == 0 ? B("LE TOTAL +5%", "TOTAL LE +5%") : B("LE TOTAL +15%", "TOTAL LE +15%");
            case "containment_tuning":
                return bought == 0 ? B("CICLO HIGGS 0,9 S", "HIGGS CYCLE 0.9 S") : B("CICLO HIGGS 0,8 S", "HIGGS CYCLE 0.8 S");
            case "tetraquark_stabilization":
                return bought == 0 ? B("TRAZAS +15%", "TRACES +15%") : B("TRAZAS +35%", "TRACES +35%");
            case "triangle_unlock_1":
                return B("ACTIVA TRIÁNGULO", "ACTIVATE TRIANGLE");
            case "triangle_impulse_tuning":
                return bought == 0 ? B("LE DE CIRCUITO +16%", "CIRCUIT LE +16%") : B("LE DE CIRCUITO +20%", "CIRCUIT LE +20%");
            case "triangle_synergy_resonance":
                return bought == 0 ? B("TRAZAS TOTALES +13%", "TOTAL TRACES +13%") : B("TRAZAS TOTALES +15%", "TOTAL TRACES +15%");
            case "triangle_persistence_anchor":
                return bought == 0 ? B("INICIO DE SINCRONÍA 65%", "START SYNC 65%") : B("INICIO DE SINCRONÍA 80%", "START SYNC 80%");
            case "triangle_energy_efficiency":
                return bought == 0 ? B("ENERGÍA +20%", "ENERGY +20%") : bought == 1 ? B("ENERGÍA +40%", "ENERGY +40%") : B("ENERGÍA +65%", "ENERGY +65%");
            default:
                return B("APLICAR SIGUIENTE NIVEL", "APPLY NEXT LEVEL");
        }
    }

    public void RefreshNow()
    {
        var manager = F2UpgradeManager.I;
        var def = manager != null ? manager.GetDef(upgradeId) : null;
        if (def == null) return;
        int bought = manager.GetPurchasedTierCount(upgradeId);
        bool discovered = bought > 0 ||
            UpgradeStudySystem.IsDiscovered(GameState.I, upgradeId);
        if (!discovered)
        {
            RefreshStudyState();
            return;
        }

        bool maxed = manager.IsMaxed(upgradeId);
        double cost = manager.GetNextCost(upgradeId);
        string currency = def.currency == F2UpgradeCurrency.LE ? "LE" : L("ui.traces", "Trazas");

        if (titleText != null) titleText.text = L($"f2.upgrade.{upgradeId}.name", def.name);
        if (descriptionText != null) descriptionText.text = Describe(bought);
        if (tierText != null) tierText.text = maxed ? L("f2.action.completed", "Completado") : LF("f2.level", "Nivel {0}/{1}", bought, def.tiers.Count);
        if (costText != null) costText.text = maxed ? string.Empty : LF("f2.cost", "Coste: {0:0.##} {1}", cost, currency);

        F2UpgradeLockReason reason = manager.GetLockReason(upgradeId);
        if (buyButton != null)
        {
            buyButton.interactable = reason == F2UpgradeLockReason.None;
            ApplyButtonStyle(maxed);
            var label = buyButton.GetComponentInChildren<TextMeshProUGUI>(true);
            if (label != null)
                label.text = BuildButtonLabel(reason, cost, currency).ToUpperInvariant();
        }
    }

    private void RefreshStudyState()
    {
        UpgradeStudyDef study = UpgradeStudySystem.GetStudyForUnlock(upgradeId);
        if (study == null || GameState.I == null) return;

        bool active = UpgradeStudySystem.IsStudyActiveForUnlock(
            GameState.I, upgradeId);
        bool conclusion = active && UpgradeStudySystem.IsConclusionPending(GameState.I);
        double progress = active
            ? UpgradeStudySystem.GetActiveProgress01(GameState.I)
            : 0.0;
        double remaining = active
            ? UpgradeStudySystem.GetRemainingSeconds(GameState.I)
            : study.durationSeconds;

        if (titleText != null)
            titleText.text = L($"study.{study.id}.title", B("Estudio de artefacto", "Artifact study"));
        if (descriptionText != null)
        {
            string hint = L($"study.{study.id}.hint", B(
                "Analiza este fenómeno para descubrir una mejora.",
                "Analyze this phenomenon to discover an upgrade."));
            descriptionText.text = hint + "\n" + B(
                "Investiga, ajusta ambas señales y revela la conclusión.",
                "Research, tune both signals, then reveal the conclusion.");
        }

        if (tierText != null)
        {
            if (conclusion)
                tierText.text = L("study.status.conclusion", "Conclusión disponible");
            else if (active)
                tierText.text = L("study.status.active", "Investigando");
            else
                tierText.text = L("study.status.clue", "Indicio");
        }

        if (costText != null)
        {
            costText.text = active
                ? LF("study.progress", "Progreso: {0:0}% · {1}", progress * 100.0,
                    FormatDuration(remaining))
                : (study.energyCost > 0.000001
                    ? LF("study.energy_cost", "Inicio: {0:0.##} Energía", study.energyCost) + " · "
                    : string.Empty) +
                  LF("study.duration", "Duración: {0}", FormatDuration(remaining));
        }

        UpgradeStudyBlockReason reason = UpgradeStudySystem.GetStartBlockReason(
            GameState.I, study.id);
        if (buyButton != null)
        {
            buyButton.interactable = conclusion ||
                reason == UpgradeStudyBlockReason.None;
            ApplyButtonStyle(false);
            TextMeshProUGUI label = buyButton.GetComponentInChildren<TextMeshProUGUI>(true);
            if (label != null)
                label.text = BuildStudyButtonLabel(study, reason, conclusion)
                    .ToUpperInvariant();
        }
    }

    private string BuildStudyButtonLabel(
        UpgradeStudyDef study,
        UpgradeStudyBlockReason reason,
        bool conclusion)
    {
        if (conclusion) return L("study.action.reveal", "Revelar");
        if (reason == UpgradeStudyBlockReason.None)
            return L($"study.{study.id}.action", L("study.action.start", "Investigar"));
        if (reason == UpgradeStudyBlockReason.StudyAlreadyActive)
            return L("study.status.active", "Investigando");
        if (reason == UpgradeStudyBlockReason.AnotherStudyActive)
            return L("study.lock.another_active", "Otro estudio activo");
        if (reason == UpgradeStudyBlockReason.CircuitNotSynchronized)
            return L("study.lock.synchronize", "Sincroniza el circuito");
        if (reason == UpgradeStudyBlockReason.CircuitSwitchRequired)
            return L("study.lock.switch", "Cambia de circuito");
        if (reason == UpgradeStudyBlockReason.MissingVertices)
            return L("f2.lock.missing_vertices", "Faltan vértices");
        if (reason == UpgradeStudyBlockReason.TriangleLocked)
            return L("f2.lock.missing_triangle", "Requiere Acople");
        if (reason == UpgradeStudyBlockReason.MissingDiscovery)
            return L("study.lock.missing_discovery", "Faltan estudios");
        if (reason == UpgradeStudyBlockReason.InsufficientEnergy)
            return L("study.lock.energy", "Falta Energía");
        return L("study.action.start", "Investigar");
    }

    private static string FormatDuration(double seconds)
    {
        int total = Mathf.Max(0, Mathf.CeilToInt((float)seconds));
        int minutes = total / 60;
        int remainder = total % 60;
        return $"{minutes:0}:{remainder:00}";
    }

    private void ApplyButtonStyle(bool maxed)
    {
        if (_verticalTheme == null)
        {
            VerticalUiSkinRoot skin = GetComponentInParent<VerticalUiSkinRoot>(true);
            _verticalTheme = skin != null ? skin.theme : null;
        }

        if (_verticalTheme == null)
        {
            Dimension1DarkThemeRuntime.ApplyButtonColors(buyButton, maxed);
            return;
        }

        Image background = buyButton.targetGraphic as Image;
        if (background != null)
        {
            background.sprite = maxed
                ? _verticalTheme.selectedButtonFrame
                : _verticalTheme.buttonFrame;
            background.type = Image.Type.Sliced;
            background.color = Color.white;
        }

        Color accent = GetVerticalAccent(maxed);
        ColorBlock colors = buyButton.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.86f, 0.97f, 1f, 1f);
        colors.pressedColor = new Color(0.65f, 0.90f, 1f, 1f);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = new Color(0.34f, 0.43f, 0.50f, 0.72f);
        colors.colorMultiplier = 1f;
        buyButton.colors = colors;

        TextMeshProUGUI label = buyButton.GetComponentInChildren<TextMeshProUGUI>(true);
        if (label != null)
            label.color = buyButton.interactable || maxed
                ? accent
                : _verticalTheme.secondaryText;
    }

    private Color GetVerticalAccent(bool maxed)
    {
        if (maxed) return _verticalTheme.completed;
        if (upgradeId == "tetraquark_stabilization")
            return _verticalTheme.traces;
        if (upgradeId == "triangle_unlock_1" ||
            upgradeId == "triangle_impulse_tuning" ||
            upgradeId == "triangle_synergy_resonance" ||
            upgradeId == "triangle_persistence_anchor" ||
            upgradeId == "triangle_energy_efficiency")
            return _verticalTheme.triangle;
        return _verticalTheme.energy;
    }

    private string BuildButtonLabel(F2UpgradeLockReason reason, double cost, string currency)
    {
        if (reason == F2UpgradeLockReason.Maxed) return L("f2.action.completed", "Completado");
        if (reason == F2UpgradeLockReason.MissingVertices) return L("f2.lock.missing_vertices", "Faltan vértices");
        if (reason == F2UpgradeLockReason.TriangleLocked) return L("f2.lock.missing_triangle", "Requiere Acople");
        if (reason == F2UpgradeLockReason.ResearchRequired) return L("study.action.start", "Investigar");
        if (reason == F2UpgradeLockReason.InsufficientFunds)
        {
            double balance = F2UpgradeManager.I.GetDef(upgradeId).currency == F2UpgradeCurrency.LE ? GameState.I.LE : GameState.I.Traces;
            return LF("f2.lock.missing_funds", "Faltan {0:0.##} {1}", System.Math.Max(0.0, cost - balance), currency) +
                "\n" + NextBenefitLabel(F2UpgradeManager.I.GetPurchasedTierCount(upgradeId));
        }
        return (upgradeId == "triangle_unlock_1"
            ? L("f2.action.activate", "Activar")
            : L("f2.action.improve", "Mejorar")) + "\n" +
            NextBenefitLabel(F2UpgradeManager.I.GetPurchasedTierCount(upgradeId));
    }

    private void OnBuyClicked()
    {
        if (GameState.I == null) return;
        bool changed = false;
        bool triangleActivated = false;

        bool discovered = UpgradeStudySystem.IsDiscovered(GameState.I, upgradeId) ||
            (F2UpgradeManager.I != null &&
             F2UpgradeManager.I.GetPurchasedTierCount(upgradeId) > 0);
        if (!discovered)
        {
            UpgradeStudyDef study = UpgradeStudySystem.GetStudyForUnlock(upgradeId);
            if (study == null) return;
            if (UpgradeStudySystem.IsStudyActiveForUnlock(GameState.I, upgradeId) &&
                UpgradeStudySystem.IsConclusionPending(GameState.I))
            {
                changed = UpgradeStudySystem.TryRevealConclusion(GameState.I, out _);
            }
            else
            {
                changed = UpgradeStudySystem.TryStartStudy(GameState.I, study.id);
            }
            RefreshNow();
        }
        else if (F2UpgradeManager.I != null && F2UpgradeManager.I.TryBuy(upgradeId))
        {
            changed = true;
            triangleActivated = upgradeId == "triangle_unlock_1";
            RefreshNow();
        }

        if (changed) SaveService.I?.Save();

        if (triangleActivated)
        {
            TabsUI.Instance?.ShowGeneracion();
            TriangleActivationTutorialUI.ShowAfterUnlock();
        }

        var visibility = FindFirstObjectByType<F2UpgradeVisibilityController>();
        if (visibility != null) visibility.RefreshAll();
        HUD hud = FindFirstObjectByType<HUD>(FindObjectsInactive.Include);
        if (hud != null) hud.RefreshNow();
    }
}

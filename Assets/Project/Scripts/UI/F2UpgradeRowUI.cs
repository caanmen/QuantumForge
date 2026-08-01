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
        bool room2 = GameState.I != null && GameState.I.experimentalChamberUnlocked;
        switch (upgradeId)
        {
            case "emission_focus": return tier == 0 ? B("Actual: +0% LE · Siguiente: +5%", "Current: +0% LE · Next: +5%") : tier == 1 ? B("Actual: +5% LE · Siguiente: +15%", "Current: +5% LE · Next: +15%") : B("Total: +15% LE", "Total: +15% LE");
            case "containment_tuning": return tier == 0 ? B("Higgs: ciclo 1,0 s → 0,9 s", "Higgs: cycle 1.0 s → 0.9 s") : tier == 1 ? B("Higgs: ciclo 0,9 s → 0,8 s", "Higgs: cycle 0.9 s → 0.8 s") : B("Higgs: ciclo 0,8 s", "Higgs: 0.8 s cycle");
            case "tetraquark_stabilization": return tier == 0 ? B("Actual: +0% Trazas · Siguiente: +15%", "Current: +0% Traces · Next: +15%") : tier == 1 ? B("Actual: +15% Trazas · Siguiente: +35%", "Current: +15% Traces · Next: +35%") : B("Total: +35% Trazas", "Total: +35% Traces");
            case "triangle_unlock_1": return tier > 0 ? B("Conecta Higgs, Tetraquark y Modulador. Circuitos activos.", "Connects Higgs, Tetraquark and Modulator. Circuits active.") : B("Conecta Higgs, Tetraquark y Modulador. Activa los circuitos.", "Connects Higgs, Tetraquark and Modulator. Activates circuits.");
            case "triangle_impulse_tuning": return tier == 0 ? B("Energía: +12% → +16% LE", "Energy: +12% → +16% LE") : tier == 1 ? B("Energía: +16% → +20% LE", "Energy: +16% → +20% LE") : B("Energía: +20% LE", "Energy: +20% LE");
            case "triangle_synergy_resonance":
                if (!room2) return tier == 0 ? B("+10% Trazas · fragmentos al abrir Cuarto 2", "+10% Traces · fragments after opening Room 2") : tier == 1 ? B("+13% Trazas · fragmentos al abrir Cuarto 2", "+13% Traces · fragments after opening Room 2") : B("+15% Trazas · fragmentos al abrir Cuarto 2", "+15% Traces · fragments after opening Room 2");
                return tier == 0 ? B("+10% Trazas · +6% fragmentos", "+10% Traces · +6% fragments") : tier == 1 ? B("+13% Trazas · +8% fragmentos", "+13% Traces · +8% fragments") : B("+15% Trazas · +10% fragmentos", "+15% Traces · +10% fragments");
            case "triangle_persistence_anchor": return tier == 0 ? B("Cambio de circuito: inicia al 50% → 65%", "Circuit change: starts at 50% → 65%") : tier == 1 ? B("Cambio de circuito: inicia al 65% → 80%", "Circuit change: starts at 65% → 80%") : B("Cambio de circuito: inicia al 80%", "Circuit change: starts at 80%");
            default: return F2UpgradeManager.I.GetDef(upgradeId)?.description ?? string.Empty;
        }
    }

    public void RefreshNow()
    {
        var manager = F2UpgradeManager.I;
        var def = manager != null ? manager.GetDef(upgradeId) : null;
        if (def == null) return;
        int bought = manager.GetPurchasedTierCount(upgradeId);
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
            upgradeId == "triangle_persistence_anchor")
            return _verticalTheme.triangle;
        return _verticalTheme.energy;
    }

    private string BuildButtonLabel(F2UpgradeLockReason reason, double cost, string currency)
    {
        if (reason == F2UpgradeLockReason.Maxed) return L("f2.action.completed", "Completado");
        if (reason == F2UpgradeLockReason.MissingVertices) return L("f2.lock.missing_vertices", "Faltan vértices");
        if (reason == F2UpgradeLockReason.TriangleLocked) return L("f2.lock.missing_triangle", "Requiere Acople");
        if (reason == F2UpgradeLockReason.InsufficientFunds)
        {
            double balance = F2UpgradeManager.I.GetDef(upgradeId).currency == F2UpgradeCurrency.LE ? GameState.I.LE : GameState.I.Traces;
            return LF("f2.lock.missing_funds", "Faltan {0:0.##} {1}", System.Math.Max(0.0, cost - balance), currency);
        }
        return upgradeId == "triangle_unlock_1" ? L("f2.action.activate", "Activar") : L("f2.action.improve", "Mejorar");
    }

    private void OnBuyClicked()
    {
        if (F2UpgradeManager.I != null && F2UpgradeManager.I.TryBuy(upgradeId))
        {
            RefreshNow();
            var visibility = FindFirstObjectByType<F2UpgradeVisibilityController>();
            if (visibility != null) visibility.RefreshAll();
        }
    }
}

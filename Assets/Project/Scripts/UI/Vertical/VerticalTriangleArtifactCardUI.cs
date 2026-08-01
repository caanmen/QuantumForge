using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class VerticalTriangleArtifactCardUI : MonoBehaviour
{
    public string buildingId;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI stateText;
    public Image icon;
    public Button buyButton;
    public Sprite higgsIcon;
    public Sprite tetraIcon;
    public Sprite modulatorIcon;

    [SerializeField] private float refreshInterval = 0.25f;
    private float timer;

    private void Awake()
    {
        if (buyButton != null)
        {
            buyButton.onClick.RemoveListener(Buy);
            buyButton.onClick.AddListener(Buy);
            MobileQaFriendlyLayout.ConfigureButtonForMobile(buyButton);
        }
        Refresh(true);
    }

    private void Update()
    {
        timer += Time.unscaledDeltaTime;
        if (timer < refreshInterval)
            return;
        timer = 0f;
        Refresh(false);
    }

    private void Buy()
    {
        BuildingState state = GetState();
        if (BuildingPurchaseService.TryPurchase(GameState.I, state))
            Refresh(true);
    }

    private void Refresh(bool force)
    {
        BuildingState state = GetState();
        if (state == null || state.def == null || GameState.I == null)
            return;

        string displayName = buildingId == "vacuum_observer" ? "HIGGS" :
            buildingId == "casimir_panel" ? "TETRAQUARK" : "MODULADOR";
        string shownName = buildingId == "fluctuation_antenna" && state.level > 0
            ? displayName + " · " + Localize(
                "building.modulator.active", "Triangulo activo").ToUpperInvariant()
            : displayName + " · NV. " + state.level;
        SetIfChanged(nameText, shownName);

        bool modulatorOwned = buildingId == "fluctuation_antenna" && state.level > 0;
        double cost = GameState.I.GetEffectiveBuildingCost(state);
        string status = modulatorOwned
            ? Localize("building.modulator.active", "Triangulo activo")
            : Localize("ui.cost_prefix", "Coste:") + " " + cost.ToString("0.##") + " LE";
        SetIfChanged(stateText, status);

        if (buyButton != null)
        {
            buyButton.interactable = !modulatorOwned && !state.IsAtMaxLevel() &&
                BuildingUnlock.IsUnlocked(state.def) && GameState.I.LE >= cost;
            TextMeshProUGUI label = buyButton.GetComponentInChildren<TextMeshProUGUI>(true);
            if (label != null)
                SetIfChanged(label, modulatorOwned
                    ? "—"
                    : Localize("ui.buy", "Comprar").ToUpperInvariant());
        }

        if (icon != null && (force || icon.sprite == null))
        {
            if (buildingId == "vacuum_observer") icon.sprite = higgsIcon;
            else if (buildingId == "casimir_panel") icon.sprite = tetraIcon;
            else icon.sprite = modulatorIcon;
            icon.preserveAspect = true;
        }
    }

    private BuildingState GetState()
    {
        return GameState.I != null ? GameState.I.GetBuildingState(buildingId) : null;
    }

    private static string Localize(string key, string fallback)
    {
        if (LocalizationManager.I == null)
            return fallback;
        string value = LocalizationManager.I.T(key);
        return string.IsNullOrEmpty(value) || value == key ? fallback : value;
    }

    private static void SetIfChanged(TextMeshProUGUI label, string value)
    {
        if (label != null && label.text != value)
            label.SetText(value);
    }
}

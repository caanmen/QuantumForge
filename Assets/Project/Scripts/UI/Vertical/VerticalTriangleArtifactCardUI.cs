using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class VerticalTriangleArtifactCardUI : MonoBehaviour
{
    public string buildingId;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI stateText;
    public TextMeshProUGUI roleText;
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

        bool energyGenerator = buildingId == "fluctuation_antenna" && state.level > 0;
        string displayName = buildingId == "vacuum_observer" ? "HIGGS" :
            buildingId == "casimir_panel" ? "TETRAQUARK" : "MODULADOR";
        SetIfChanged(nameText, displayName + " \u00b7 NV. " + state.level);

        string role = buildingId == "vacuum_observer"
            ? "FLUCTUADOR\nDE CAMPO"
            : buildingId == "casimir_panel"
                ? "CONFINADOR\nCU\u00c1NTICO"
                : energyGenerator
                    ? "CAPTADOR\nDE ENERG\u00cdA"
                    : "MODULADOR\nDE FASE";
        SetIfChanged(roleText, role);

        double cost = GameState.I.GetEffectiveBuildingCost(state);
        double traceCost = energyGenerator
            ? GameState.I.GetTriangleEnergyGeneratorTraceCost()
            : 0.0;
        string status = Localize("ui.cost_prefix", "Coste:") + " " +
            cost.ToString("0.##") + " LE" +
            (traceCost > 0.0 ? " + " + traceCost.ToString("0.##") + " Trazas" : "");
        int nextMilestone = GameState.GetNextArtifactMilestoneLevel(
            state.def.id, state.level);
        if (nextMilestone > 0)
        {
            double multiplier = GameState.GetArtifactLevelMilestoneMultiplier(
                state.def.id, nextMilestone);
            status += "\nHITO NV. " + nextMilestone + " · PRODUCCIÓN x" +
                multiplier.ToString("0");
        }
        SetIfChanged(stateText, status);

        if (buyButton != null)
        {
            buyButton.interactable = !state.IsAtMaxLevel() &&
                BuildingUnlock.IsUnlocked(state.def) && GameState.I.LE >= cost &&
                GameState.I.Traces >= traceCost;
            TextMeshProUGUI label = buyButton.GetComponentInChildren<TextMeshProUGUI>(true);
            if (label != null)
                SetIfChanged(label, BuildPurchaseLabel(state));
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

    private static string BuildPurchaseLabel(BuildingState state)
    {
        if (GameState.I == null || state == null) return "COMPRAR";
        if (state.def.id == "fluctuation_antenna" && state.level == 0)
            return "COMPRAR\nDESBLOQUEA TRI\u00c1NGULO";

        double le = GameState.I.GetBuildingNextLevelLEPerSecond(state);
        double traces = GameState.I.GetBuildingNextLevelTracesPerSecond(state);
        double energy = GameState.I.GetBuildingNextLevelTriangleEnergyPerSecond(state);
        string gain = string.Empty;
        if (le > 0.000001) gain = "+" + le.ToString("0.##") + " LE/s";
        if (traces > 0.000001)
            gain += (gain.Length > 0 ? " \u00b7 " : "") + "+" + traces.ToString("0.###") + " T/s";
        if (energy > 0.000001)
            gain += (gain.Length > 0 ? " \u00b7 " : "") + "+" + energy.ToString("0.##") + " E/s";
        return Localize("ui.buy", "Comprar").ToUpperInvariant() +
            (gain.Length > 0 ? "\n" + gain : string.Empty);
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

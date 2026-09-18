using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controlador compartido por las subpantallas de detalle de los sectores 1, 2 y 4.
/// La escena conserva la composición; este componente sólo presenta datos reales y
/// enruta acciones usando identificadores estables.
/// </summary>
public sealed class Dimension1SectorDetailUI : MonoBehaviour
{
    [Serializable]
    public sealed class PlanetCardView
    {
        public string planetId;
        public string primaryMetalId;
        public string secondaryMetalId;
        public string tertiaryMetalId;
        public TMP_Text levelText;
        public TMP_Text productionText;
        public TMP_Text progressText;
        public Image progressFill;
        public TMP_Text actionText;
        public TMP_Text costText;
        public Button actionButton;
    }

    [SerializeField] private string sectorId;
    [SerializeField] private Dimension1PanelUI panel;
    [SerializeField] private Dimension1CommandCenterUI commandCenter;
    [SerializeField] private Dimension1MetalsInventoryUI metalsInventory;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button backButton;
    [SerializeField] private Button backToMapButton;
    [SerializeField] private Button allMetalsButton;
    [SerializeField] private TMP_Text[] metalAmounts;
    [SerializeField] private TMP_Text[] metalRates;
    [SerializeField] private PlanetCardView[] planets;
    [SerializeField] private string[] destinationIds;
    [SerializeField] private Button[] destinationButtons;
    [SerializeField] private TMP_Text[] destinationStatuses;
    [SerializeField] private Button[] navigationButtons;

    private bool visible;
    private float refreshTimer;

    public string SectorId => sectorId;
    public bool IsOpen => visible;

    public void Configure(
        string configuredSectorId,
        Dimension1PanelUI configuredPanel,
        Dimension1CommandCenterUI configuredCommandCenter,
        Dimension1MetalsInventoryUI configuredMetalsInventory,
        CanvasGroup configuredCanvasGroup,
        Button configuredBackButton,
        Button configuredBackToMapButton,
        Button configuredAllMetalsButton,
        TMP_Text[] configuredMetalAmounts,
        TMP_Text[] configuredMetalRates,
        PlanetCardView[] configuredPlanets,
        string[] configuredDestinationIds,
        Button[] configuredDestinationButtons,
        TMP_Text[] configuredDestinationStatuses,
        Button[] configuredNavigationButtons)
    {
        sectorId = configuredSectorId;
        panel = configuredPanel;
        commandCenter = configuredCommandCenter;
        metalsInventory = configuredMetalsInventory;
        canvasGroup = configuredCanvasGroup;
        backButton = configuredBackButton;
        backToMapButton = configuredBackToMapButton;
        allMetalsButton = configuredAllMetalsButton;
        metalAmounts = configuredMetalAmounts;
        metalRates = configuredMetalRates;
        planets = configuredPlanets;
        destinationIds = configuredDestinationIds;
        destinationButtons = configuredDestinationButtons;
        destinationStatuses = configuredDestinationStatuses;
        navigationButtons = configuredNavigationButtons;
    }

    private void Awake()
    {
        SetVisible(false);
    }

    private void Update()
    {
        if (!visible) return;
        refreshTimer -= Time.unscaledDeltaTime;
        if (refreshTimer > 0f) return;
        refreshTimer = .25f;
        RefreshData();
    }

    public void OpenFromGalaxy()
    {
        transform.SetAsLastSibling();
        SetVisible(true);
        refreshTimer = 0f;
        RefreshData();
    }

    public void CloseToGalaxy() { SetVisible(false); }
    public void CloseSilently() { SetVisible(false); }
    public void OpenMetals() { if (metalsInventory != null) metalsInventory.Open(); }

    public void ActOnPlanet0() { ActOnPlanet(0); }
    public void ActOnPlanet1() { ActOnPlanet(1); }
    public void OpenDestination0() { OpenDestination(0); }
    public void OpenDestination1() { OpenDestination(1); }
    public void OpenDestination2() { OpenDestination(2); }
    public void OpenDestination3() { OpenDestination(3); }

    public void OpenGalaxy() { CloseToGalaxy(); }

    public void OpenExplore()
    {
        CloseSilently();
        if (panel != null) panel.OnClickCloseGalaxyPanel();
        if (commandCenter != null) commandCenter.ShowExploreScreen();
    }

    public void OpenHangar()
    {
        CloseSilently();
        if (panel == null) return;
        panel.OnClickCloseGalaxyPanel();
        panel.OnClickOpenHangarPanel();
    }

    public void OpenRelics()
    {
        CloseSilently();
        if (panel == null) return;
        panel.OnClickCloseGalaxyPanel();
        panel.OnClickOpenRelicChamberPanel();
    }

    public void OpenTree()
    {
        CloseSilently();
        if (panel == null) return;
        panel.OnClickCloseGalaxyPanel();
        panel.OnClickOpenDimension1TreePanel();
    }

    private void OpenDestination(int index)
    {
        if (destinationIds == null || index < 0 || index >= destinationIds.Length || panel == null)
            return;

        string destinationId = destinationIds[index];
        if (FindAvailableDestinationDropdownIndex(GameState.I, destinationId) <= 0)
            return;

        OpenExplore();
        panel.TrySelectAvailableDestinationForUi(destinationId, sectorId);
    }

    private void ActOnPlanet(int index)
    {
        if (planets == null || index < 0 || index >= planets.Length) return;
        PlanetCardView view = planets[index];
        GameState state = GameState.I;
        if (state == null || view == null) return;
        state.EnsureDimension1State();
        D1PlanetState planet = FindPlanet(state, view.planetId);
        if (planet == null) return;

        bool changed = planet.unlocked
            ? Dimension1System.TryUpgradeExtractor(state, view.planetId)
            : Dimension1System.TryUnlockPlanet(state, view.planetId);

        if (changed && SaveService.I != null) SaveService.I.Save();
        RefreshData();
    }

    private void RefreshData()
    {
        GameState state = GameState.I;
        if (state == null) return;
        state.EnsureDimension1State();
        Dimension1HeaderMetalsUI.Refresh(transform, state, sectorId);

        if (planets != null)
            foreach (PlanetCardView planet in planets) RefreshPlanet(state, planet);

        if (destinationButtons == null || destinationIds == null) return;
        int count = Mathf.Min(destinationButtons.Length, destinationIds.Length);
        for (int i = 0; i < count; i++)
            if (destinationButtons[i] != null)
            {
                bool available = FindAvailableDestinationDropdownIndex(state, destinationIds[i]) > 0;
                destinationButtons[i].interactable = available;
                SetDestinationStatus(i, available
                    ? "LISTO PARA EXPLORAR"
                    : DestinationUnavailableStatus(state, destinationIds[i]));
            }
    }

    private void SetDestinationStatus(int index, string value)
    {
        if (destinationStatuses != null && index >= 0 && index < destinationStatuses.Length &&
            destinationStatuses[index] != null)
            destinationStatuses[index].text = value;
    }

    private string DestinationUnavailableStatus(GameState state, string destinationId)
    {
        if (state != null && state.dimension1ScanActive && state.dimension1ActiveScanSectorId == sectorId)
            return "BARRIDO EN CURSO";
        if (state?.dimension1ScannedDestinations != null)
            foreach (D1ScannedDestinationState destination in state.dimension1ScannedDestinations)
                if (destination != null && destination.destinationId == destinationId &&
                    destination.sectorId == sectorId)
                    return "NO DISPONIBLE";
        return "NO ESCANEADO";
    }

    private int FindAvailableDestinationDropdownIndex(GameState state, string destinationId)
    {
        if (state == null || state.dimension1ScannedDestinations == null) return -1;

        int dropdownIndex = 1;
        foreach (D1ScannedDestinationState destination in state.dimension1ScannedDestinations)
        {
            if (destination == null || !destination.available) continue;
            if (destination.destinationId == destinationId && destination.sectorId == sectorId)
                return dropdownIndex;
            dropdownIndex++;
        }
        return -1;
    }

    private static void RefreshPlanet(GameState state, PlanetCardView view)
    {
        if (view == null) return;
        D1PlanetState planet = FindPlanet(state, view.planetId);
        if (planet == null) return;

        bool unlocked = planet.unlocked;
        int tier = Mathf.Max(0, planet.extractorTier);
        if (view.levelText != null) view.levelText.text = unlocked ? tier.ToString() : "—";

        double production = 0d;
        if (unlocked)
        {
            production += EffectiveProduction(state, planet, view.primaryMetalId);
            production += EffectiveProduction(state, planet, view.secondaryMetalId);
            production += EffectiveProduction(state, planet, view.tertiaryMetalId);
        }
        if (view.productionText != null)
            view.productionText.text = unlocked ? "+" + FormatAmount(production) + "/s" : "BLOQUEADO";

        float progress = unlocked ? Mathf.Clamp01(tier / 10f) : 0f;
        if (view.progressText != null) view.progressText.text = Mathf.RoundToInt(progress * 100f) + "%";
        if (view.progressFill != null)
            view.progressFill.rectTransform.anchorMax = new Vector2(progress, 1f);

        if (unlocked)
        {
            if (view.actionText != null) view.actionText.text = "MEJORAR EXTRACTOR";
            double cost = Dimension1System.GetExtractorUpgradeCost(state, planet);
            string metal = Dimension1System.GetExtractorUpgradeMainCostMetal(planet);
            if (view.costText != null) view.costText.text = FormatAmount(cost) + " " + MetalName(metal);
            if (view.actionButton != null)
                view.actionButton.interactable = Dimension1System.CanUpgradeExtractor(state, view.planetId);
        }
        else
        {
            if (view.actionText != null) view.actionText.text = "DESBLOQUEAR PLANETA";
            if (view.costText != null) view.costText.text = UnlockCost(view.planetId);
            if (view.actionButton != null)
                view.actionButton.interactable = Dimension1System.CanUnlockPlanet(state, view.planetId);
        }
    }

    private static double EffectiveProduction(GameState state, D1PlanetState planet, string metalId)
    {
        return string.IsNullOrEmpty(metalId) ? 0d :
            Dimension1System.GetPlanetMetalEffectiveProductionPerSecond(state, planet, metalId);
    }

    private void SetVisible(bool value)
    {
        visible = value;
        if (canvasGroup == null) return;
        canvasGroup.alpha = value ? 1f : 0f;
        canvasGroup.interactable = value;
        canvasGroup.blocksRaycasts = value;
        panel?.RefreshGalaxyDetailOcclusion();
    }

    private static D1PlanetState FindPlanet(GameState state, string planetId)
    {
        if (state == null || state.dimension1Planets == null) return null;
        foreach (D1PlanetState planet in state.dimension1Planets)
            if (planet != null && planet.planetId == planetId) return planet;
        return null;
    }

    private static string UnlockCost(string planetId)
    {
        if (!Dimension1System.TryGetPlanetUnlockCost(
            planetId, out string m1, out double a1, out string m2, out double a2,
            out string m3, out double a3)) return "REQUISITOS PENDIENTES";
        string value = CostPart(m1, a1);
        if (!string.IsNullOrEmpty(m2) && a2 > 0d) value += "  ·  " + CostPart(m2, a2);
        if (!string.IsNullOrEmpty(m3) && a3 > 0d) value += "  ·  " + CostPart(m3, a3);
        return value;
    }

    private static string CostPart(string metalId, double amount)
    {
        return FormatAmount(amount) + " " + MetalName(metalId);
    }

    private static string MetalName(string metalId)
    {
        if (metalId == Dimension1System.MetalIron) return "HIERRO";
        if (metalId == Dimension1System.MetalCopper) return "COBRE";
        if (metalId == Dimension1System.MetalAluminum) return "ALUMINIO";
        if (metalId == Dimension1System.MetalTitanium) return "TITANIO";
        if (metalId == Dimension1System.MetalNickel) return "NÍQUEL";
        if (metalId == Dimension1System.MetalCobalt) return "COBALTO";
        if (metalId == Dimension1System.MetalLithium) return "LITIO";
        if (metalId == Dimension1System.MetalTungsten) return "TUNGSTENO";
        if (metalId == Dimension1System.MetalPlatinum) return "PLATINO";
        if (metalId == Dimension1System.MetalIridium) return "IRIDIO";
        return metalId ?? "";
    }

    private static void Set(TMP_Text[] values, int index, string text)
    {
        if (values != null && index < values.Length && values[index] != null)
            values[index].text = text;
    }

    private static string FormatAmount(double value)
    {
        value = Math.Max(0d, value);
        if (value >= 1000000000d) return (value / 1000000000d).ToString("0.##") + "B";
        if (value >= 1000000d) return (value / 1000000d).ToString("0.##") + "M";
        if (value >= 1000d) return (value / 1000d).ToString("0.##") + "K";
        if (value >= 100d) return value.ToString("0");
        if (value >= 10d) return value.ToString("0.#");
        return value.ToString("0.##");
    }
}

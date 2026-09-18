using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Subpantalla del Sector 3 de la Carta Galáctica. La composición permanece
/// estática; este controlador solo actualiza datos y enruta acciones reales.
/// </summary>
public sealed class Dimension1AncientOrbitsUI : MonoBehaviour
{
    [Serializable]
    public sealed class PlanetCardView
    {
        public string planetId;
        public string primaryMetalId;
        public string secondaryMetalId;
        public TMP_Text levelText;
        public TMP_Text productionText;
        public TMP_Text progressText;
        public Image progressFill;
        public TMP_Text actionText;
        public TMP_Text costText;
        public Button actionButton;
    }

    [SerializeField] private Dimension1PanelUI panel;
    [SerializeField] private Dimension1CommandCenterUI commandCenter;
    [SerializeField] private Dimension1MetalsInventoryUI metalsInventory;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button backButton;
    [SerializeField] private Button backToMapButton;
    [SerializeField] private Button allMetalsButton;
    [SerializeField] private TMP_Text[] metalAmounts;
    [SerializeField] private TMP_Text[] metalRates;
    [SerializeField] private PlanetCardView planet4;
    [SerializeField] private PlanetCardView planet5;
    [SerializeField] private Button[] destinationButtons;
    [SerializeField] private TMP_Text[] destinationStatuses;
    [SerializeField] private Button[] navigationButtons;

    private bool visible;
    private float refreshTimer;

    private static readonly string[] DestinationIds =
    {
        Dimension1System.DestinationAbandonedShip,
        Dimension1System.DestinationOrbitalRuin,
        Dimension1System.DestinationLaboratory,
        Dimension1System.DestinationAbandonedStation
    };

    public bool IsOpen => visible;

    public void Configure(
        Dimension1PanelUI configuredPanel,
        Dimension1CommandCenterUI configuredCommandCenter,
        Dimension1MetalsInventoryUI configuredMetalsInventory,
        CanvasGroup configuredCanvasGroup,
        Button configuredBackButton,
        Button configuredBackToMapButton,
        Button configuredAllMetalsButton,
        TMP_Text[] configuredMetalAmounts,
        TMP_Text[] configuredMetalRates,
        PlanetCardView configuredPlanet4,
        PlanetCardView configuredPlanet5,
        Button[] configuredDestinationButtons,
        TMP_Text[] configuredDestinationStatuses,
        Button[] configuredNavigationButtons)
    {
        panel = configuredPanel;
        commandCenter = configuredCommandCenter;
        metalsInventory = configuredMetalsInventory;
        canvasGroup = configuredCanvasGroup;
        backButton = configuredBackButton;
        backToMapButton = configuredBackToMapButton;
        allMetalsButton = configuredAllMetalsButton;
        metalAmounts = configuredMetalAmounts;
        metalRates = configuredMetalRates;
        planet4 = configuredPlanet4;
        planet5 = configuredPlanet5;
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

    public void CloseToGalaxy()
    {
        SetVisible(false);
    }

    public void CloseSilently()
    {
        SetVisible(false);
    }

    public void OpenMetals()
    {
        if (metalsInventory != null) metalsInventory.Open();
    }

    public void ActOnPlanet4() { ActOnPlanet(planet4); }
    public void ActOnPlanet5() { ActOnPlanet(planet5); }

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
        if (index < 0 || index >= DestinationIds.Length || panel == null)
            return;

        GameState state = GameState.I;
        if (FindAvailableDestinationDropdownIndex(state, DestinationIds[index]) <= 0)
            return;

        OpenExplore();
        panel.TrySelectAvailableDestinationForUi(
            DestinationIds[index], Dimension1System.Sector03AncientOrbits);
    }

    private void ActOnPlanet(PlanetCardView view)
    {
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

        Dimension1HeaderMetalsUI.Refresh(transform, state, Dimension1System.Sector03AncientOrbits);

        RefreshPlanet(state, planet4);
        RefreshPlanet(state, planet5);

        if (destinationButtons != null)
        {
            int count = Mathf.Min(destinationButtons.Length, DestinationIds.Length);
            for (int i = 0; i < count; i++)
            {
                if (destinationButtons[i] != null)
                {
                    bool available = FindAvailableDestinationDropdownIndex(state, DestinationIds[i]) > 0;
                    destinationButtons[i].interactable = available;
                    SetDestinationStatus(i, available
                        ? "LISTO PARA EXPLORAR"
                        : DestinationUnavailableStatus(state, DestinationIds[i]));
                }
            }
        }
    }

    private void SetDestinationStatus(int index, string value)
    {
        if (destinationStatuses != null && index >= 0 && index < destinationStatuses.Length &&
            destinationStatuses[index] != null)
            destinationStatuses[index].text = value;
    }

    private static string DestinationUnavailableStatus(GameState state, string destinationId)
    {
        if (state != null && state.dimension1ScanActive &&
            state.dimension1ActiveScanSectorId == Dimension1System.Sector03AncientOrbits)
            return "BARRIDO EN CURSO";
        if (state?.dimension1ScannedDestinations != null)
            foreach (D1ScannedDestinationState destination in state.dimension1ScannedDestinations)
                if (destination != null && destination.destinationId == destinationId &&
                    destination.sectorId == Dimension1System.Sector03AncientOrbits)
                    return "NO DISPONIBLE";
        return "NO ESCANEADO";
    }

    private static int FindAvailableDestinationDropdownIndex(GameState state, string destinationId)
    {
        if (state == null || state.dimension1ScannedDestinations == null)
            return -1;

        int dropdownIndex = 1;
        foreach (D1ScannedDestinationState destination in state.dimension1ScannedDestinations)
        {
            if (destination == null || !destination.available)
                continue;

            if (destination.destinationId == destinationId &&
                destination.sectorId == Dimension1System.Sector03AncientOrbits)
            {
                return dropdownIndex;
            }

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
            production += Dimension1System.GetPlanetMetalEffectiveProductionPerSecond(
                state, planet, view.primaryMetalId);
            production += Dimension1System.GetPlanetMetalEffectiveProductionPerSecond(
                state, planet, view.secondaryMetalId);
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
            if (view.costText != null)
                view.costText.text = FormatAmount(cost) + " " + MetalName(metal);
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

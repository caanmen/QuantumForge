using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class Dimension1ExploreVisualUI : MonoBehaviour
{
    [SerializeField] private Dimension1PanelUI panel;
    [SerializeField] private Dimension1CommandCenterUI commandCenter;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject[] blockingPanels;
    [SerializeField] private GameObject[] hideWhileOpen;
    [SerializeField] private TMP_Text[] metalAmounts;
    [SerializeField] private TMP_Text[] metalRates;
    [SerializeField] private TMP_Text scannerLevel;
    [SerializeField] private TMP_Text destinationName;
    [SerializeField] private TMP_Text destinationLevel;
    [SerializeField] private TMP_Text destinationDistance;
    [SerializeField] private TMP_Text destinationCount;
    [SerializeField] private TMP_Text shipName;
    [SerializeField] private TMP_Text shipStatus;
    [SerializeField] private TMP_Text shipSpeed;
    [SerializeField] private TMP_Text supportName;
    [SerializeField] private TMP_Text supportStatus;
    [SerializeField] private TMP_Text supportBonus;
    [SerializeField] private TMP_Text supportAvailability;
    [SerializeField] private TMP_Text activeShip;
    [SerializeField] private TMP_Text activeDestination;
    [SerializeField] private TMP_Text activeTimer;
    [SerializeField] private Button startButton;
    [SerializeField] private bool referencePreviewForVisualQa;

    private readonly string[] metalIds =
    {
        Dimension1System.MetalIron,
        Dimension1System.MetalAluminum,
        Dimension1System.MetalNickel
    };
    private float refreshTimer;
    private bool exclusiveNavigationPending;
    private bool[] hiddenPreviousStates;
    private VerticalNavigationUI verticalNavigation;

    public void SetReferencePreviewForVisualQa(bool enabled)
    {
        referencePreviewForVisualQa = enabled;
        Refresh();
    }

    public void OpenCommandCenter()
    {
        if (commandCenter != null)
            commandCenter.ShowCommandCenterScreen();
        else
            gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        exclusiveNavigationPending = true;
        if (verticalNavigation == null)
            verticalNavigation = FindFirstObjectByType<VerticalNavigationUI>(FindObjectsInactive.Include);
        if (verticalNavigation != null)
            verticalNavigation.SetNavigationSuppressed(true, this);
        refreshTimer = 0f;
        RefreshVisibility();
        Refresh();
    }

    private void OnDisable()
    {
        exclusiveNavigationPending = false;
        ApplyExclusiveNavigation(false);
        if (verticalNavigation != null)
            verticalNavigation.SetNavigationSuppressed(false, this);
    }

    private void Update()
    {
        if (exclusiveNavigationPending)
        {
            exclusiveNavigationPending = false;
            ApplyExclusiveNavigation(true);
        }

        if (hideWhileOpen != null)
            foreach (GameObject target in hideWhileOpen)
                if (target != null && target.activeSelf) target.SetActive(false);

        RefreshVisibility();
        refreshTimer -= Time.unscaledDeltaTime;
        if (refreshTimer > 0f) return;
        refreshTimer = 0.25f;
        Refresh();
    }

    private void RefreshVisibility()
    {
        if (canvasGroup == null) return;
        bool visible = true;
        if (blockingPanels != null)
            foreach (GameObject blocker in blockingPanels)
                if (blocker != null && blocker.activeSelf)
                {
                    visible = false;
                    break;
                }
        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }

    private void ApplyExclusiveNavigation(bool hide)
    {
        if (hideWhileOpen == null) return;
        if (hide)
        {
            hiddenPreviousStates = new bool[hideWhileOpen.Length];
            for (int i = 0; i < hideWhileOpen.Length; i++)
            {
                GameObject target = hideWhileOpen[i];
                if (target == null) continue;
                hiddenPreviousStates[i] = target.activeSelf;
                target.SetActive(false);
            }
            return;
        }

        if (hiddenPreviousStates == null) return;
        for (int i = 0; i < hideWhileOpen.Length && i < hiddenPreviousStates.Length; i++)
            if (hideWhileOpen[i] != null) hideWhileOpen[i].SetActive(hiddenPreviousStates[i]);
        hiddenPreviousStates = null;
    }

    private void Refresh()
    {
        if (referencePreviewForVisualQa)
        {
            ApplyReferencePreview();
            return;
        }

        GameState state = GameState.I;
        if (state == null) return;
        state.EnsureDimension1State();
        for (int i = 0; i < metalIds.Length; i++)
        {
            Set(metalAmounts, i, FormatAmount(state.GetD1MetalAmount(metalIds[i])));
            Set(metalRates, i, "+" + FormatAmount(Dimension1System.GetMetalProductionPerSecond(state, metalIds[i])) + "/s");
        }
        Set(scannerLevel, Mathf.Clamp(state.dimension1ScannerLevel, 1, 15) + "/15");

        D1ScannedDestinationState destination = FirstDestination(state, out int destinationTotal);
        Set(destinationCount, destinationTotal.ToString());
        Set(destinationName, destination == null ? "SIN SEÑALES" : DestinationName(destination.destinationId));
        Set(destinationLevel, destination == null ? "NO DISPONIBLE" : "SEÑAL DISPONIBLE");
        Set(destinationDistance, "—");

        D1ShipState availableShip = FirstAvailableShip(state);
        Set(shipName, availableShip == null ? "SIN NAVE" : ShipName(availableShip.shipId));
        Set(shipStatus, availableShip == null ? "NO DISPONIBLE" : "DISPONIBLE");
        Set(shipSpeed, availableShip == null ? "—" : "NIVEL " + Mathf.Max(0, availableShip.speedLevel));

        D1ShipState support = FindShip(state, Dimension1System.ShipExtractorDrone);
        Set(supportName, support == null ? "DRON EXTRACTOR" : ShipName(support.shipId));
        Set(supportStatus, support != null && support.unlocked ? "DISPONIBLE" : "BLOQUEADO");
        Set(supportBonus, "APOYO DE CARGA");
        Set(supportAvailability, support != null && support.unlocked ? "● DISPONIBLE" : "● NO DISPONIBLE");

        D1ShipState active = FirstActiveShip(state);
        Set(activeShip, active == null ? "SIN EXPEDICIÓN ACTIVA" : ShipName(active.shipId));
        Set(activeDestination, active == null ? "ESCANEA UNA SEÑAL PARA EMPEZAR" : DestinationName(active.activeDestinationId));
        Set(activeTimer, active == null ? "--:--:--" : FormatTimer(active.explorationRemainingSeconds));
        if (startButton != null)
            startButton.interactable = destination != null && availableShip != null;
    }

    private void ApplyReferencePreview()
    {
        string[] amounts = { "5.98M", "4.73M", "4.70M" };
        string[] rates = { "+0.30/s", "+0.08/s", "+0.03/s" };
        for (int i = 0; i < 3; i++)
        {
            Set(metalAmounts, i, amounts[i]);
            Set(metalRates, i, rates[i]);
        }
        Set(scannerLevel, "3/15");
        Set(destinationName, "SEÑAL DESCONOCIDA");
        Set(destinationLevel, "NIVEL 2");
        Set(destinationDistance, "2.41 UA");
        Set(destinationCount, "3");
        Set(shipName, "SONDA LIGERA");
        Set(shipStatus, "DISPONIBLE");
        Set(shipSpeed, "120 UA/s");
        Set(supportName, "DRON EXTRACTOR");
        Set(supportStatus, "DISPONIBLE");
        Set(supportBonus, "BONO: +10% ESCANEO");
        Set(supportAvailability, "● DISPONIBLE");
        Set(activeShip, "SONDA ANALÍTICA");
        Set(activeDestination, "SEÑAL DESCONOCIDA NIVEL 1");
        Set(activeTimer, "00:28:45");
        if (startButton != null) startButton.interactable = true;
    }

    private static D1ScannedDestinationState FirstDestination(GameState state, out int total)
    {
        total = 0;
        D1ScannedDestinationState first = null;
        if (state.dimension1ScannedDestinations == null) return null;
        foreach (D1ScannedDestinationState destination in state.dimension1ScannedDestinations)
        {
            if (destination == null || !destination.available) continue;
            total++;
            if (first == null) first = destination;
        }
        return first;
    }

    private static D1ShipState FirstAvailableShip(GameState state)
    {
        if (state.dimension1Ships == null) return null;
        foreach (D1ShipState ship in state.dimension1Ships)
            if (ship != null && ship.unlocked && Dimension1System.IsShipActiveInDimension1Base(ship.shipId) &&
                !Dimension1System.IsD1ShipBusy(ship)) return ship;
        return null;
    }

    private static D1ShipState FirstActiveShip(GameState state)
    {
        if (state.dimension1Ships == null) return null;
        foreach (D1ShipState ship in state.dimension1Ships)
            if (ship != null && ship.explorationActive) return ship;
        return null;
    }

    private static D1ShipState FindShip(GameState state, string id)
    {
        if (state.dimension1Ships == null) return null;
        foreach (D1ShipState ship in state.dimension1Ships)
            if (ship != null && ship.shipId == id) return ship;
        return null;
    }

    private static string ShipName(string id)
    {
        if (id == Dimension1System.ShipLightProbe) return "SONDA LIGERA";
        if (id == Dimension1System.ShipExtractorDrone) return "DRON EXTRACTOR";
        if (id == Dimension1System.ShipAnalyticProbe) return "SONDA ANALÍTICA";
        if (id == Dimension1System.ShipCargoShip) return "NAVE DE CARGA";
        return string.IsNullOrEmpty(id) ? "NAVE" : id.ToUpperInvariant();
    }

    private static string DestinationName(string id)
    {
        if (id == Dimension1System.DestinationMineralBelt) return "CINTURÓN MINERAL";
        if (id == Dimension1System.DestinationShipGraveyard) return "CEMENTERIO DE NAVES";
        if (id == Dimension1System.DestinationOrbitalRuin) return "RUINA ORBITAL";
        if (id == Dimension1System.DestinationDriftingProbes) return "SONDAS A LA DERIVA";
        if (id == Dimension1System.DestinationLaboratory) return "LABORATORIO";
        if (id == Dimension1System.DestinationAbandonedStation) return "ESTACIÓN ABANDONADA";
        if (id == Dimension1System.DestinationMinorAnomaly) return "ANOMALÍA MENOR";
        if (id == Dimension1System.DestinationAncientStructure) return "ESTRUCTURA ANTIGUA";
        if (id == Dimension1System.DestinationUnstableZone) return "ZONA INESTABLE";
        return string.IsNullOrEmpty(id) ? "SEÑAL DESCONOCIDA" : id.ToUpperInvariant();
    }

    private static string FormatTimer(double seconds)
    {
        int total = Mathf.Max(0, Mathf.CeilToInt((float)seconds));
        return string.Format("{0:00}:{1:00}:{2:00}", total / 3600, total / 60 % 60, total % 60);
    }

    private static string FormatAmount(double value)
    {
        value = System.Math.Max(0d, value);
        if (value >= 1000000000d) return (value / 1000000000d).ToString("0.##") + "B";
        if (value >= 1000000d) return (value / 1000000d).ToString("0.##") + "M";
        if (value >= 1000d) return (value / 1000d).ToString("0.##") + "K";
        return value.ToString("0.##");
    }

    private static void Set(TMP_Text target, string value) { if (target != null) target.text = value; }
    private static void Set(TMP_Text[] targets, int index, string value)
    {
        if (targets != null && index >= 0 && index < targets.Length && targets[index] != null)
            targets[index].text = value;
    }
}

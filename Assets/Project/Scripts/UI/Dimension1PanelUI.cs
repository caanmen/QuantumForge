using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class Dimension1PanelUI : MonoBehaviour
{
    [Header("Texto principal")]
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Botones de minería")]
    [SerializeField] private Button upgradePlanet1Button;
    [SerializeField] private Button unlockPlanet2Button;
    [SerializeField] private Button upgradePlanet2Button;
    [SerializeField] private Button unlockPlanet3Button;
    [SerializeField] private Button upgradePlanet3Button;
    [SerializeField] private Button unlockPlanet4Button;
    [SerializeField] private Button upgradePlanet4Button;
    [SerializeField] private Button unlockPlanet5Button;
    [SerializeField] private Button upgradePlanet5Button;
    [SerializeField] private Button unlockPlanet6Button;
    [SerializeField] private Button upgradePlanet6Button;
    [SerializeField] private Button unlockPlanet7Button;
    [SerializeField] private Button upgradePlanet7Button;

    [Header("Escáner")]
    [SerializeField] private Button scanButton;
    [SerializeField] private TMP_Dropdown destinationDropdown;
    [SerializeField] private TMP_Dropdown shipDropdown;
    [SerializeField] private Button exploreButton;
    [SerializeField] private Button upgradeScannerButton;

    [Header("Recompensas de exploración")]
    [SerializeField] private GameObject explorationRewardsPanel;
    [SerializeField] private TextMeshProUGUI explorationRewardsText;

    [Header("Hangar")]
    [SerializeField] private Button unlockExtractorDroneButton;
    [SerializeField] private Button unlockAnalyticProbeButton;

    [FormerlySerializedAs("unlockCargoShipButton")]
    [SerializeField] private Button unlockCargoShipButton;

    [SerializeField] private Button unlockRescueShipButton;
    [SerializeField] private Button unlockConvergenceShipButton;
    [SerializeField] private Button upgradeLightProbeSpeed1Button;
    [SerializeField] private Button upgradeExtractorDroneCargo1Button;
    [SerializeField] private Button upgradeExtractorDroneSpeed1Button;
    [SerializeField] private Button upgradeAnalyticProbeSensors1Button;
    [SerializeField] private Button upgradeCargoShipCargo1Button;
    [SerializeField] private Button upgradeRescueShipSpeed1Button;
    [SerializeField] private Button upgradeConvergenceShipSpeed1Button;

    [Header("Hangar provisional")]
    [SerializeField] private GameObject dimension1MainContentRoot;

    [FormerlySerializedAs("shipUpgradePanel")]
    [SerializeField] private GameObject hangarPanel;

    [FormerlySerializedAs("openShipUpgradePanelButton")]
    [SerializeField] private Button openHangarPanelButton;

    [FormerlySerializedAs("closeShipUpgradePanelButton")]
    [SerializeField] private Button closeHangarPanelButton;

    [FormerlySerializedAs("shipUpgradeDropdown")]
    [SerializeField] private TMP_Dropdown hangarShipDropdown;

    [FormerlySerializedAs("shipUpgradeInfoText")]
    [SerializeField] private TextMeshProUGUI hangarInfoText;

    [FormerlySerializedAs("upgradeSelectedCargoButton")]
    [SerializeField] private Button hangarUpgradeCargoButton;

    [FormerlySerializedAs("upgradeSelectedSpeedButton")]
    [SerializeField] private Button hangarUpgradeSpeedButton;

    [FormerlySerializedAs("upgradeSelectedArmorButton")]
    [SerializeField] private Button hangarUpgradeArmorButton;

    [FormerlySerializedAs("upgradeSelectedSensorsButton")]
    [SerializeField] private Button hangarUpgradeSensorsButton;

    [Header("Rendimiento")]
    [SerializeField] private float refreshInterval = 0.25f;

    private float refreshTimer;
    private int selectedDestinationIndex;
    private string destinationDropdownSignature = "";
    private bool isRefreshingDestinationDropdown;
    private int selectedShipIndex;
    private string shipDropdownSignature = "";
    private bool isRefreshingShipDropdown;
    private int selectedHangarShipIndex;
    private string hangarShipDropdownSignature = "";
    private bool isRefreshingHangarShipDropdown;
    private bool hangarPanelOpen;
    private int lastHandledExplorationResultId;
    private bool showingExplorationResultPanel;

        private static readonly string[] HangarShipIds =
    {
        Dimension1System.ShipLightProbe,
        Dimension1System.ShipExtractorDrone,
        Dimension1System.ShipAnalyticProbe,
        Dimension1System.ShipCargoShip,
        Dimension1System.ShipRescueShip,
        Dimension1System.ShipConvergenceShip
    };

    private void OnEnable()
    {
        PrimeExplorationResultPanelState();
        RefreshUI();
    }

    private void PrimeExplorationResultPanelState()
    {
        GameState gs = GameState.I;

        lastHandledExplorationResultId =
            gs != null
                ? gs.dimension1LastExplorationResultId
                : 0;

        showingExplorationResultPanel = false;
    }

    private void Update()
    {
        refreshTimer += Time.unscaledDeltaTime;

        if (refreshTimer < refreshInterval)
            return;

        refreshTimer = 0f;
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (statusText == null)
            return;

        GameState gs = GameState.I;

        if (gs == null)
        {
            statusText.text = "Dimensión 1 no disponible.";
            return;
        }

        if (!gs.dimension01Unlocked)
        {
            statusText.text = "Dimensión 1 bloqueada.";
            return;
        }

        gs.EnsureDimension1State();

        statusText.text =
            "DIMENSIÓN 1 - MVP\n\n" +
            BuildMetalsText(gs) +
            "\n\n" +
            BuildPlanetsText(gs) +
            "\n\n" +
            BuildScannerText(gs) +
            "\n\n" +
            BuildSelectedDestinationText(gs) +
            "\n\n" +
            BuildBlueprintArchiveText(gs) +
            "\n\n" +
            BuildHangarText(gs);


            RefreshDestinationDropdown(gs);
            RefreshShipDropdown(gs);
            RefreshButtons(gs);
            UpdateExplorationResultPanelState(gs);
            RefreshExplorationRewardsPanel(gs);
            RefreshHangarPanel(gs);

        }

        private void UpdateExplorationResultPanelState(GameState gs)
        {
            if (gs == null)
                return;

            if (gs.dimension1LastExplorationResultId <= lastHandledExplorationResultId)
                return;

            lastHandledExplorationResultId = gs.dimension1LastExplorationResultId;
            showingExplorationResultPanel = true;
        }

        private string BuildMetalsText(GameState gs)
        {
            return
                "Metales:\n" +
                BuildMetalLine(gs, Dimension1System.MetalIron, "Hierro") + "\n" +
                BuildMetalLine(gs, Dimension1System.MetalCopper, "Cobre") + "\n" +
                BuildMetalLine(gs, Dimension1System.MetalAluminum, "Aluminio") + "\n" +
                BuildMetalLine(gs, Dimension1System.MetalTitanium, "Titanio") + "\n" +
                BuildMetalLine(gs, Dimension1System.MetalNickel, "Níquel") + "\n" +
                BuildMetalLine(gs, Dimension1System.MetalCobalt, "Cobalto") + "\n" +
                BuildMetalLine(gs, Dimension1System.MetalLithium, "Litio") + "\n" +
                BuildMetalLine(gs, Dimension1System.MetalTungsten, "Tungsteno") + "\n" +
                BuildMetalLine(gs, Dimension1System.MetalPlatinum, "Platino") + "\n" +
                BuildMetalLine(gs, Dimension1System.MetalIridium, "Iridio");
        }

        private string BuildMetalLine(GameState gs, string metalId, string visualName)
        {
            double amount = gs.GetD1MetalAmount(metalId);
            double perSecond = Dimension1System.GetMetalProductionPerSecond(gs, metalId);

            return visualName +
                ": " +
                amount.ToString("0.00") +
                " (+" +
                perSecond.ToString("0.00") +
                "/s)";
        }


    private string BuildPlanetsText(GameState gs)
    {
        return
            "Planetas:\n" +
            BuildPlanetLine(gs, Dimension1System.Planet01, "Planeta 1", "Hierro", "Cobre") + "\n" +
            BuildPlanetLine(gs, Dimension1System.Planet02, "Planeta 2", "Aluminio", "Titanio") + "\n" +
            BuildPlanetLine(gs, Dimension1System.Planet03, "Planeta 3", "Níquel", "Cobalto") + "\n" +
            BuildPlanetLine(gs, Dimension1System.Planet04, "Planeta 4", "Litio", "Tungsteno") + "\n" +
            BuildPlanetLine(gs, Dimension1System.Planet05, "Planeta 5", "Platino", "Níquel") + "\n" +
            BuildPlanetLine(gs, Dimension1System.Planet06, "Planeta 6", "Iridio", "Cobalto") + "\n" +
            BuildPlanetLine(gs, Dimension1System.Planet07, "Planeta 7", "Tungsteno", "Platino", "Iridio");
    }

    private string BuildPlanetLine(
        GameState gs,
        string planetId,
        string visualName,
        string mainMetalName,
        string secondaryMetalName,
        string thirdMetalName = null
    )
    {
        D1PlanetState planet = FindPlanet(gs, planetId);

        if (planet == null || !planet.unlocked)
        {
            return visualName + ": bloqueado";
        }

        string activeMetals = mainMetalName;

        if (planet.extractorTier >= 10)
        {
            activeMetals += " / " + secondaryMetalName;
        }

        if (!string.IsNullOrEmpty(thirdMetalName) && planet.extractorTier >= 20)
        {
            activeMetals += " / " + thirdMetalName;
        }

        double upgradeCost = Dimension1System.GetExtractorUpgradeCost(planet);
        string costMetalId = Dimension1System.GetExtractorUpgradeMainCostMetal(planet);
        string costMetalName = GetMetalVisualName(costMetalId);

        return visualName +
            ": desbloqueado | Tier " +
            planet.extractorTier +
            " | Produce: " +
            activeMetals +
            " | Sig. tier: " +
            upgradeCost.ToString("0") +
            " " +
            costMetalName;
    }

    private string GetMetalVisualName(string metalId)
    {
        switch (metalId)
        {
            case Dimension1System.MetalIron:
                return "Hierro";

            case Dimension1System.MetalCopper:
                return "Cobre";

            case Dimension1System.MetalAluminum:
                return "Aluminio";

            case Dimension1System.MetalTitanium:
                return "Titanio";

            case Dimension1System.MetalNickel:
                return "Níquel";

            case Dimension1System.MetalCobalt:
                return "Cobalto";

            case Dimension1System.MetalLithium:
                return "Litio";

            case Dimension1System.MetalTungsten:
                return "Tungsteno";

            case Dimension1System.MetalPlatinum:
                return "Platino";

            case Dimension1System.MetalIridium:
                return "Iridio";    

            default:
                return metalId;
        }
    }

    private string BuildScannerText(GameState gs)
    {
        if (gs == null)
            return "Galaxia / Escáner:\nNo disponible.";

        string scannerHeader =
            "Galaxia / Escáner:\n" +
            "Escáner: Nivel " +
            Dimension1System.GetSimpleScannerLevel(gs) +
            "/" +
            Dimension1System.SimpleScannerMaxLevel +
            "\nCapacidad actual: " +
            Dimension1System.GetCurrentSimpleScanDestinationCount(gs) +
            "/" +
            Dimension1System.GetSimpleScanMaxDestinationCount() +
            " destinos\n";

        if (gs.dimension1ScanActive)
        {
            return
                scannerHeader +
                "Escaneando galaxia...\n" +
                "Barrido restante: " +
                FormatSeconds(gs.dimension1ScanRemainingSeconds);
        }

        int destinationCount = GetAvailableDestinationCount(gs);

        if (destinationCount <= 0)
        {
            return
                scannerHeader +
                "Ningún destino detectado.\n" +
                "Pulsa Escanear para hacer un barrido.";
        }

        return
            scannerHeader +
            "Destinos detectados: " +
            destinationCount +
            "\nSelecciona un destino en la lista para ver sus detalles.";
    }

    private string BuildSelectedDestinationText(GameState gs)
    {
        if (gs == null)
            return "Destino seleccionado:\nNo disponible.";

        if (gs.dimension1ScanActive)
        {
            return
                "Destino seleccionado:\n" +
                "Esperando resultado del barrido.";
        }

        D1ScannedDestinationState destination = GetSelectedAvailableDestination(gs);

        if (destination == null)
        {
            return
                "Destino seleccionado:\n" +
                "Ningún destino seleccionado.";
        }

        D1ShipState selectedShip = GetSelectedAvailableShip(gs);
        string selectedShipName = selectedShip != null
            ? GetShipVisualName(selectedShip.shipId)
            : "Ninguna";

        return
            "Destino seleccionado:\n" +
            "Nombre: " +
            GetDestinationVisualName(destination.destinationId) +
            "\nTiempo base: " +
            GetDestinationBaseDurationPreview(destination.destinationId) +
            "\nRecompensas posibles: " +
            GetDestinationRewardPreview(destination.destinationId) +
            "\nFragmentos blueprint base: " +
            GetDestinationBlueprintBaseChancePreview(destination.destinationId) +
            "\nNaves compatibles: " +
            BuildUnlockedShipsText(gs) +
            "\nNave seleccionada: " +
            selectedShipName +
            "\nBonus de nave: " +
            GetShipBonusPreview(selectedShip);
    }

    private void RefreshExplorationRewardsPanel(GameState gs)
    {
        bool shouldShowResult =
            showingExplorationResultPanel &&
            gs != null &&
            !string.IsNullOrEmpty(gs.dimension1LastExplorationDestinationId);

        bool shouldShowPreview =
            gs != null &&
            !gs.dimension1ScanActive &&
            GetSelectedAvailableDestination(gs) != null &&
            GetSelectedAvailableShip(gs) != null;

        bool shouldShow = shouldShowResult || shouldShowPreview;

        if (explorationRewardsPanel != null)
            explorationRewardsPanel.SetActive(shouldShow);

        if (explorationRewardsText == null)
            return;

        if (!shouldShow)
        {
            explorationRewardsText.text = "";
            return;
        }

        if (shouldShowResult)
        {
            explorationRewardsText.text = BuildExplorationResultText(gs);
            return;
        }

        explorationRewardsText.text = BuildExplorationRewardsText(gs);
    }

    private string BuildExplorationRewardsText(GameState gs)
    {
        D1ScannedDestinationState destination = GetSelectedAvailableDestination(gs);
        D1ShipState selectedShip = GetSelectedAvailableShip(gs);

        if (destination == null || selectedShip == null)
            return "";

        string metalRewards =
            GetDestinationRewardPreview(destination.destinationId)
                .Replace(" / ", "\n");

        string text =
            "Recompensas posibles\n\n" +
            "Tiempo de misión:\n" +
            GetDestinationDurationPreview(destination.destinationId, selectedShip) +
            "\n\nMetales:\n" +
            metalRewards;

        float fragmentChance =
            Dimension1System.GetSimpleBlueprintFragmentChance(
                destination.destinationId,
                selectedShip
            );

        text +=
            "\n\nFragmentos de blueprint:\n" +
            (fragmentChance * 100f).ToString("0") +
            "%";

        return text;
    }

    private string BuildExplorationResultText(GameState gs)
    {
        if (gs == null)
            return "";

        string text =
            "Exploración completada\n\n" +
            GetDestinationVisualName(gs.dimension1LastExplorationDestinationId) +
            "\n\n";

        bool hasAnyMetal = false;

        if (gs.dimension1LastExplorationRewards != null)
        {
            foreach (D1MetalAmount reward in gs.dimension1LastExplorationRewards)
            {
                if (reward == null)
                    continue;

                if (string.IsNullOrEmpty(reward.metalId))
                    continue;

                if (reward.amount <= 0.0)
                    continue;

                text +=
                    GetMetalVisualName(reward.metalId) +
                    " +" +
                    reward.amount.ToString("0.0") +
                    "\n";

                hasAnyMetal = true;
            }
        }

        if (!hasAnyMetal)
            text += "Sin metales\n";

        text +=
            "\nFragmentos blueprint +" +
            gs.dimension1LastExplorationBlueprintFragments;

        return text;
    }

    private string BuildLastExplorationText(GameState gs)
    {
        if (gs == null)
            return "Última exploración:\nNo disponible.";

        if (string.IsNullOrEmpty(gs.dimension1LastExplorationDestinationId))
            return "Última exploración:\nSin exploraciones completadas.";

        string text =
            "Última exploración:\n" +
            "Destino: " +
            GetDestinationVisualName(gs.dimension1LastExplorationDestinationId);

        if (gs.dimension1LastExplorationRewards == null || gs.dimension1LastExplorationRewards.Count == 0)
        {
            text += "\nMetales obtenidos: ninguno";
            return text;
        }

        text += "\nMetales obtenidos: ";

        bool hasAnyReward = false;

        foreach (D1MetalAmount reward in gs.dimension1LastExplorationRewards)
        {
            if (reward == null)
                continue;

            if (string.IsNullOrEmpty(reward.metalId))
                continue;

            if (reward.amount <= 0.0)
                continue;

            if (hasAnyReward)
                text += " / ";

            text +=
                reward.amount.ToString("0.0") +
                " " +
                GetMetalVisualName(reward.metalId);

            hasAnyReward = true;
        }

        if (!hasAnyReward)
            text += "ninguno";

        if (gs.dimension1LastExplorationBlueprintFragments > 0)
        {
            text +=
                "\nFragmentos de blueprint: +" +
                gs.dimension1LastExplorationBlueprintFragments;
        }
        else
        {
            text += "\nFragmentos de blueprint: +0";
        }

        return text;
    }

    private void RefreshDestinationDropdown(GameState gs)
    {
        if (destinationDropdown == null)
            return;

        int destinationCount = GetAvailableDestinationCount(gs);
        bool hasDestinations = destinationCount > 0;

        if (gs != null && gs.dimension1ScanActive)
        {
            destinationDropdown.gameObject.SetActive(false);
            return;
        }

        destinationDropdown.gameObject.SetActive(hasDestinations);
        destinationDropdown.interactable = hasDestinations && !IsLightProbeExploring(gs);

        if (!hasDestinations)
        {
            isRefreshingDestinationDropdown = true;

            destinationDropdown.ClearOptions();
            selectedDestinationIndex = 0;
            destinationDropdownSignature = "";

            isRefreshingDestinationDropdown = false;
            return;
        }

        List<string> options = new List<string>();
        options.Add("Elegir destino");

        int visibleIndex = 1;

        foreach (D1ScannedDestinationState destination in gs.dimension1ScannedDestinations)
        {
            if (destination == null || !destination.available)
                continue;

            options.Add(
                "Destino " +
                visibleIndex +
                ": " +
                GetDestinationVisualName(destination.destinationId)
            );

            visibleIndex++;
        }

        string newSignature = "";

        for (int i = 0; i < options.Count; i++)
        {
            if (i > 0)
                newSignature += "|";

            newSignature += options[i];
        }

        isRefreshingDestinationDropdown = true;

        if (newSignature != destinationDropdownSignature)
        {
            destinationDropdown.ClearOptions();
            destinationDropdown.AddOptions(options);
            destinationDropdownSignature = newSignature;
            selectedDestinationIndex = 0;
        }
        else
        {
            selectedDestinationIndex = destinationDropdown.value;
        }

        if (selectedDestinationIndex < 0 || selectedDestinationIndex >= options.Count)
            selectedDestinationIndex = 0;

        destinationDropdown.SetValueWithoutNotify(selectedDestinationIndex);
        destinationDropdown.RefreshShownValue();

        isRefreshingDestinationDropdown = false;
    }

    private void RefreshShipDropdown(GameState gs)
    {
        if (shipDropdown == null)
            return;

        bool scanActive = gs != null && gs.dimension1ScanActive;
        bool hasDestinations = GetAvailableDestinationCount(gs) > 0;
        int shipCount = GetAvailableShipCount(gs);
        bool hasShips = shipCount > 0;

        bool shouldShow = !scanActive && hasDestinations && hasShips;

        shipDropdown.gameObject.SetActive(shouldShow);
        shipDropdown.interactable = shouldShow && !IsSelectedShipExploring(gs);

        if (!shouldShow)
        {
            isRefreshingShipDropdown = true;

            shipDropdown.ClearOptions();
            selectedShipIndex = 0;
            shipDropdownSignature = "";

            isRefreshingShipDropdown = false;
            return;
        }

        List<string> options = new List<string>();
        options.Add("Elegir nave");

        foreach (D1ShipState ship in gs.dimension1Ships)
        {
            if (ship == null || !ship.unlocked)
                continue;

            options.Add(GetShipVisualName(ship.shipId));
        }

        string newSignature = "";

        for (int i = 0; i < options.Count; i++)
        {
            if (i > 0)
                newSignature += "|";

            newSignature += options[i];
        }

        isRefreshingShipDropdown = true;

        if (newSignature != shipDropdownSignature)
        {
            shipDropdown.ClearOptions();
            shipDropdown.AddOptions(options);
            shipDropdownSignature = newSignature;
            selectedShipIndex = 0;
        }
        else
        {
            selectedShipIndex = shipDropdown.value;
        }

        if (selectedShipIndex < 0 || selectedShipIndex >= options.Count)
            selectedShipIndex = 0;

        shipDropdown.SetValueWithoutNotify(selectedShipIndex);
        shipDropdown.RefreshShownValue();

        isRefreshingShipDropdown = false;
    }

    private int GetAvailableDestinationCount(GameState gs)
    {
        if (gs == null || gs.dimension1ScannedDestinations == null)
            return 0;

        int count = 0;

        foreach (D1ScannedDestinationState destination in gs.dimension1ScannedDestinations)
        {
            if (destination != null && destination.available)
                count++;
        }

        return count;
    }

    private D1ScannedDestinationState GetSelectedAvailableDestination(GameState gs)
    {
        if (gs == null || gs.dimension1ScannedDestinations == null)
            return null;

        int dropdownIndex = selectedDestinationIndex;

        if (destinationDropdown != null)
            dropdownIndex = destinationDropdown.value;

        if (dropdownIndex <= 0)
            return null;

        int targetIndex = dropdownIndex - 1;
        int currentIndex = 0;

        foreach (D1ScannedDestinationState destination in gs.dimension1ScannedDestinations)
        {
            if (destination == null || !destination.available)
                continue;

            if (currentIndex == targetIndex)
                return destination;

            currentIndex++;
        }

        return null;
    }

        private int GetSelectedAvailableDestinationRealIndex(GameState gs)
    {
        if (gs == null || gs.dimension1ScannedDestinations == null)
            return -1;

        int dropdownIndex = selectedDestinationIndex;

        if (destinationDropdown != null)
            dropdownIndex = destinationDropdown.value;

        if (dropdownIndex <= 0)
            return -1;

        return dropdownIndex - 1;
    }

    private int GetAvailableShipCount(GameState gs)
    {
        if (gs == null || gs.dimension1Ships == null)
            return 0;

        int count = 0;

        foreach (D1ShipState ship in gs.dimension1Ships)
        {
            if (ship != null && ship.unlocked)
                count++;
        }

        return count;
    }

    private D1ShipState GetSelectedAvailableShip(GameState gs)
    {
        if (gs == null || gs.dimension1Ships == null)
            return null;

        int dropdownIndex = selectedShipIndex;

        if (shipDropdown != null)
            dropdownIndex = shipDropdown.value;

        if (dropdownIndex <= 0)
            return null;

        int targetIndex = dropdownIndex - 1;
        int currentIndex = 0;

        foreach (D1ShipState ship in gs.dimension1Ships)
        {
            if (ship == null || !ship.unlocked)
                continue;

            if (currentIndex == targetIndex)
                return ship;

            currentIndex++;
        }

        return null;
    }

    private bool IsSelectedShipExploring(GameState gs)
    {
        D1ShipState selectedShip = GetSelectedAvailableShip(gs);
        return selectedShip != null && selectedShip.explorationActive;
    }

    private string GetShipVisualName(string shipId)
    {
        switch (shipId)
        {
            case Dimension1System.ShipLightProbe:
                return "Sonda Ligera";

            case Dimension1System.ShipExtractorDrone:
                return "Dron Extractor"; 

            case Dimension1System.ShipAnalyticProbe:
                return "Sonda Analítica";

            case Dimension1System.ShipCargoShip:
                return "Nave de Carga";  

            case Dimension1System.ShipRescueShip:
                return "Nave de Rescate";   

            case Dimension1System.ShipConvergenceShip:
                return "Nave de Convergencia"; 

            default:
                return shipId;
        }
    }

    private string GetShipBonusPreview(D1ShipState ship)
    {
        if (ship == null)
            return "Ninguno";

        switch (ship.shipId)
        {
            case Dimension1System.ShipExtractorDrone:
                if (ship.cargoLevel >= 6)
                    return "Carga VI: x2.00 metales de exploración";

                if (ship.cargoLevel >= 5)
                    return "Carga V: x1.80 metales de exploración";

                if (ship.cargoLevel >= 4)
                    return "Carga IV: x1.65 metales de exploración";

                if (ship.cargoLevel >= 3)
                    return "Carga III: x1.50 metales de exploración";

                if (ship.cargoLevel >= 2)
                    return "Carga II: x1.40 metales de exploración";

                if (ship.cargoLevel >= 1)
                    return "Carga I: x1.30 metales de exploración";

                return "Carga base: x1.20 metales de exploración";

            case Dimension1System.ShipAnalyticProbe:
                if (ship.sensorsLevel >= 6)
                    return "Sensores VI: +10% fragmentos blueprint y barrido 2.1s";

                if (ship.sensorsLevel >= 5)
                    return "Sensores V: +9% fragmentos blueprint y barrido 2.4s";

                if (ship.sensorsLevel >= 4)
                    return "Sensores IV: +7% fragmentos blueprint y barrido 2.7s";

                if (ship.sensorsLevel >= 3)
                    return "Sensores III: +6% fragmentos blueprint y barrido 3.0s";

                if (ship.sensorsLevel >= 2)
                    return "Sensores II: +4% fragmentos blueprint y barrido 3.5s";

                if (ship.sensorsLevel >= 1)
                    return "Sensores I: +3% fragmentos blueprint y barrido 4.0s";

                return "Sensores base: +2% fragmentos blueprint";

            case Dimension1System.ShipLightProbe:
                if (ship.speedLevel >= 6)
                    return "Velocidad VI: exploración x0.58 tiempo";

                if (ship.speedLevel >= 5)
                    return "Velocidad V: exploración x0.64 tiempo";

                if (ship.speedLevel >= 4)
                    return "Velocidad IV: exploración x0.70 tiempo";

                if (ship.speedLevel >= 3)
                    return "Velocidad III: exploración x0.76 tiempo";

                if (ship.speedLevel >= 2)
                    return "Velocidad II: exploración x0.82 tiempo";

                if (ship.speedLevel >= 1)
                    return "Velocidad I: exploración x0.90 tiempo";

                return "Velocidad base: exploración x1.00 tiempo";

                case Dimension1System.ShipCargoShip:
                    if (ship.cargoLevel >= 6)
                        return "Bodega Modular VI: x1.75 metales de exploración";

                    if (ship.cargoLevel >= 5)
                        return "Bodega Modular V: x1.60 metales de exploración";

                    if (ship.cargoLevel >= 4)
                        return "Bodega Modular IV: x1.47 metales de exploración";

                    if (ship.cargoLevel >= 3)
                        return "Bodega Modular III: x1.35 metales de exploración";

                    if (ship.cargoLevel >= 2)
                        return "Bodega Modular II: x1.26 metales de exploración";

                    if (ship.cargoLevel >= 1)
                        return "Bodega Modular I: x1.18 metales de exploración";

                        return "Bodega Modular base: x1.10 metales de exploración";

                    case Dimension1System.ShipRescueShip:
                        return
                            "Velocidad " +
                            FormatShipUpgradeLevel(ship.speedLevel) +
                            ": x" +
                            GetSpeedMultiplierText(ship.speedLevel) +
                            " tiempo en destinos de rescate/peligro";

                    case Dimension1System.ShipConvergenceShip:
                        return
                            "Velocidad " +
                            FormatShipUpgradeLevel(ship.speedLevel) +
                            ": x" +
                            GetSpeedMultiplierText(ship.speedLevel) +
                            " tiempo en anomalías y destinos avanzados";

            default:
                return "Ninguno";
        }
    }

    private string BuildUnlockedShipsText(GameState gs)
    {
        if (gs == null || gs.dimension1Ships == null)
            return "Ninguna";

        string text = "";
        bool hasAny = false;

        foreach (D1ShipState ship in gs.dimension1Ships)
        {
            if (ship == null || !ship.unlocked)
                continue;

            if (hasAny)
                text += " / ";

            text += GetShipVisualName(ship.shipId);
            hasAny = true;
        }

        if (!hasAny)
            return "Ninguna";

        return text;
    }

    private bool IsLightProbeExploring(GameState gs)
    {
        D1ShipState ship = FindShip(gs, Dimension1System.ShipLightProbe);
        return ship != null && ship.explorationActive;
    }

    private string GetDestinationVisualName(string destinationId)
    {
        switch (destinationId)
        {
            case Dimension1System.DestinationMineralBelt:
                return "Cinturón Mineral";

            case Dimension1System.DestinationShipGraveyard:
                return "Cementerio de Naves";

            case Dimension1System.DestinationAbandonedProbe:
            case Dimension1System.DestinationAbandonedShip:
                return "Nave Abandonada";

            case Dimension1System.DestinationOrbitalRuin:
                return "Ruina Orbital";

            case Dimension1System.DestinationDriftingProbes:
                return "Sondas a la Deriva";

            case Dimension1System.DestinationLaboratory:
                return "Laboratorio";

            case Dimension1System.DestinationAbandonedStation:
                return "Estación Abandonada";

            case Dimension1System.DestinationMinorAnomaly:
                return "Anomalía Menor";

            case Dimension1System.DestinationAncientStructure:
                return "Estructura Antigua";

            case Dimension1System.DestinationUnstableZone:
                return "Zona Inestable";

            case Dimension1System.DestinationCrystalDebris:
                return "Restos Cristalinos";

            case Dimension1System.DestinationOrbitalArmorWreckage:
                return "Restos de Blindaje Orbital";

            case Dimension1System.DestinationCollapsedReactor:
                return "Reactor Colapsado";

            default:
                return destinationId;
        }
    }

    private string GetDestinationRewardPreview(string destinationId)
    {
        GameState gs = GameState.I;

        List<string> rewards = new List<string>();

        switch (destinationId)
        {
            case Dimension1System.DestinationMineralBelt:
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalIron);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalCopper);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalAluminum);
                break;

            case Dimension1System.DestinationShipGraveyard:
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalIron);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalTitanium);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalNickel);
                break;

            case Dimension1System.DestinationAbandonedProbe:
            case Dimension1System.DestinationAbandonedShip:
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalCopper);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalTitanium);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalNickel);
                break;

            case Dimension1System.DestinationOrbitalRuin:
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalCopper);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalAluminum);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalLithium);
                break;

            case Dimension1System.DestinationDriftingProbes:
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalCopper);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalAluminum);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalLithium);
                break;

            case Dimension1System.DestinationLaboratory:
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalAluminum);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalTitanium);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalLithium);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalCobalt);
                break;

            case Dimension1System.DestinationAbandonedStation:
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalTitanium);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalNickel);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalCobalt);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalPlatinum);
                break;

            case Dimension1System.DestinationMinorAnomaly:
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalLithium);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalTungsten);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalCobalt);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalPlatinum);
                break;

            case Dimension1System.DestinationAncientStructure:
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalTungsten);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalPlatinum);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalIridium);
                break;

            case Dimension1System.DestinationUnstableZone:
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalTungsten);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalPlatinum);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalIridium);
                break;
        }

        if (rewards.Count == 0)
            return "Sin metales desbloqueados para este destino";

        return string.Join(" / ", rewards);
    }

    private void AddRewardPreviewIfUnlocked(GameState gs, List<string> rewards, string metalId)
    {
        if (!Dimension1System.IsMetalUnlockedForDimension1(gs, metalId))
            return;

        rewards.Add(GetMetalVisualName(metalId));
    }

    private string GetDestinationBaseDurationPreview(string destinationId)
    {
        double duration = Dimension1System.GetSimpleExplorationBaseDurationPreviewSeconds(
            destinationId
        );

        return duration.ToString("0.0") + "s";
    }

    private string GetDestinationBlueprintBaseChancePreview(string destinationId)
    {
        float chance = Dimension1System.GetSimpleBlueprintFragmentBaseChance(destinationId);

        return (chance * 100f).ToString("0") + "%";
    }

    private string GetDestinationDurationPreview(string destinationId, D1ShipState selectedShip)
    {
        double duration = Dimension1System.GetSimpleExplorationDurationPreviewSeconds(
            destinationId,
            selectedShip
        );

        return duration.ToString("0.0") + "s";
    }

    private string BuildBlueprintArchiveText(GameState gs)
    {
        if (gs == null)
            return "Archivo de Blueprints:\nNo disponible.";

        int completedBlueprints = Dimension1System.GetCompletedBlueprintCount(gs);
        int currentProgress = Dimension1System.GetBlueprintFragmentProgress(gs);

        string text =
            "Archivo de Blueprints:\n" +
            "Fragmentos: " +
            currentProgress +
            "/" +
            Dimension1System.BlueprintFragmentsPerBlueprint +
            "\nBlueprints disponibles: " +
            completedBlueprints +
            "\n\nNaves por blueprint:\n" +
            BuildBlueprintShipProgressLine(
                gs,
                Dimension1System.ShipCargoShip,
                "Nave de Carga",
                3,
                completedBlueprints
            ) +
            "\n" +
            BuildBlueprintShipProgressLine(
                gs,
                Dimension1System.ShipRescueShip,
                "Nave de Rescate",
                4,
                completedBlueprints
            ) +
            "\n" +
            BuildBlueprintShipProgressLine(
                gs,
                Dimension1System.ShipConvergenceShip,
                "Nave de Convergencia",
                4,
                completedBlueprints
            );

        return text;
    }

    private string BuildBlueprintShipProgressLine(
        GameState gs,
        string shipId,
        string shipName,
        int requiredBlueprints,
        int availableBlueprints
    )
    {
        D1ShipState ship = FindShip(gs, shipId);

        if (ship != null && ship.unlocked)
            return shipName + ": construida";

        return shipName +
            ": requiere " +
            requiredBlueprints +
            " blueprints genéricos";
    }

    private string BuildHangarText(GameState gs)
    {
        return
            "Hangar:\n" +
            BuildShipLine(gs, Dimension1System.ShipLightProbe, "Sonda Ligera") +
            "\n" +
            BuildShipLine(gs, Dimension1System.ShipExtractorDrone, "Dron Extractor") +
            "\n" +
            BuildShipLine(gs, Dimension1System.ShipAnalyticProbe, "Sonda Analítica")+
            "\n" +
            BuildShipLine(gs, Dimension1System.ShipCargoShip, "Nave de Carga")+
            "\n"+
            BuildShipLine(gs, Dimension1System.ShipRescueShip, "Nave de Rescate")+
            "\n"+
            BuildShipLine(gs, Dimension1System.ShipConvergenceShip, "Nave de Convergencia");
    }

    private string BuildShipLine(GameState gs, string shipId, string visualName)
    {
        D1ShipState ship = FindShip(gs, shipId);

        if (ship == null || !ship.unlocked)
        {
            return BuildLockedShipLine(shipId, visualName);
        }

        string upgradeText = BuildShipUpgradeLevelSummary(ship);

        if (ship.explorationActive)
        {
            return visualName +
                ": desbloqueada | " +
                upgradeText +
                " | Estado: explorando " +
                GetDestinationVisualName(ship.activeDestinationId) +
                " | Restante: " +
                FormatSeconds(ship.explorationRemainingSeconds);
        }

        return visualName +
            ": desbloqueada | " +
            upgradeText +
            " | Estado: disponible";
    }

    private string BuildLockedShipLine(string shipId, string visualName)
    {
        if (shipId == Dimension1System.ShipExtractorDrone)
        {
            return visualName +
                ": bloqueada | Requiere Hierro + Cobre";
        }

        if (shipId == Dimension1System.ShipAnalyticProbe)
        {
            return visualName +
                ": bloqueada | Requiere Cobre + Aluminio";
        }

        if (shipId == Dimension1System.ShipCargoShip)
        {
            return visualName +
                ": bloqueada | Requiere 3 Blueprints + Titanio + Níquel";
        }

        if (shipId == Dimension1System.ShipRescueShip)
        {
            return visualName +
                ": bloqueada | Requiere 4 Blueprints + Titanio + Níquel + Cobalto + Platino";
        }

        if (shipId == Dimension1System.ShipConvergenceShip)
        {
            return visualName +
                ": bloqueada | Requiere 4 Blueprints + Platino + Iridio + Cobalto + Tungsteno";
        }

        return visualName + ": bloqueada";
    }

    private string BuildShipUpgradeLevelSummary(D1ShipState ship)
    {
        if (ship == null)
            return "Mejora: -";

        switch (ship.shipId)
        {
            case Dimension1System.ShipLightProbe:
                return
                    "Velocidad: " +
                    FormatShipUpgradeLevel(ship.speedLevel) +
                    " | Exploración: x" +
                    GetLightProbeSpeedMultiplierText(ship.speedLevel) +
                    " tiempo";

            case Dimension1System.ShipExtractorDrone:
                return
                    "Carga: " +
                    FormatShipUpgradeLevel(ship.cargoLevel) +
                    " | Metales: x" +
                    GetExtractorDroneCargoMultiplierText(ship.cargoLevel);

            case Dimension1System.ShipAnalyticProbe:
                return
                    "Sensores: " +
                    FormatShipUpgradeLevel(ship.sensorsLevel) +
                    " | Barrido: " +
                    GetAnalyticProbeScanDurationText(ship.sensorsLevel);

            case Dimension1System.ShipCargoShip:
                return
                    "Carga: " +
                    FormatShipUpgradeLevel(ship.cargoLevel) +
                    " | Bodega Modular: x" +
                    GetCargoShipCargoMultiplierText(ship.cargoLevel);   


            case Dimension1System.ShipRescueShip:
                return
                    "Velocidad: " +
                    FormatShipUpgradeLevel(ship.speedLevel) +
                    " | Rescate/peligro: x" +
                    GetSpeedMultiplierText(ship.speedLevel) +
                    " tiempo";

            case Dimension1System.ShipConvergenceShip:
                return
                    "Velocidad: " +
                    FormatShipUpgradeLevel(ship.speedLevel) +
                    " | Anomalías/avanzado: x" +
                    GetSpeedMultiplierText(ship.speedLevel) +
                    " tiempo";

            default:
                return "Mejora: -";
        }
    }

    private string FormatShipUpgradeLevel(int level)
    {
        if (level <= 0)
            return "Base";

        return ToRomanNumber(level);
    }

    private string GetLightProbeSpeedMultiplierText(int level)
    {
        if (level >= 6)
            return "0.58";

        if (level >= 5)
            return "0.64";

        if (level >= 4)
            return "0.70";

        if (level >= 3)
            return "0.76";

        if (level >= 2)
            return "0.82";

        if (level >= 1)
            return "0.90";

        return "1.00";
    }

    private string GetSpeedMultiplierText(int level)
    {
        if (level >= 6)
            return "0.58";

        if (level >= 5)
            return "0.64";

        if (level >= 4)
            return "0.70";

        if (level >= 3)
            return "0.76";

        if (level >= 2)
            return "0.82";

        if (level >= 1)
            return "0.90";

        return "1.00";
    }

    private string GetExtractorDroneCargoMultiplierText(int level)
    {
        if (level >= 6)
            return "2.00";

        if (level >= 5)
            return "1.80";

        if (level >= 4)
            return "1.65";

        if (level >= 3)
            return "1.50";

        if (level >= 2)
            return "1.40";

        if (level >= 1)
            return "1.30";

        return "1.20";
    }

    private string GetCargoShipCargoMultiplierText(int level)
    {
        if (level >= 6)
            return "1.75";

        if (level >= 5)
            return "1.60";

        if (level >= 4)
            return "1.47";

        if (level >= 3)
            return "1.35";

        if (level >= 2)
            return "1.26";

        if (level >= 1)
            return "1.18";

        return "1.10";
    }

    private string GetAnalyticProbeScanDurationText(int level)
    {
        if (level >= 6)
            return "2.1s";

        if (level >= 5)
            return "2.4s";

        if (level >= 4)
            return "2.7s";

        if (level >= 3)
            return "3.0s";

        if (level >= 2)
            return "3.5s";

        if (level >= 1)
            return "4.0s";

        return "5.0s";
    }

    private string FormatSeconds(double seconds)
    {
        seconds = System.Math.Max(0.0, seconds);

        int totalSeconds = Mathf.CeilToInt((float)seconds);
        int minutes = totalSeconds / 60;
        int remainingSeconds = totalSeconds % 60;

        if (minutes > 0)
            return minutes + "m " + remainingSeconds + "s";

        return remainingSeconds + "s";
    }

    private D1PlanetState FindPlanet(GameState gs, string planetId)
    {
        if (gs == null || gs.dimension1Planets == null)
            return null;

        foreach (D1PlanetState planet in gs.dimension1Planets)
        {
            if (planet != null && planet.planetId == planetId)
                return planet;
        }

        return null;
    }

    private D1ShipState FindShip(GameState gs, string shipId)
    {
        if (gs == null || gs.dimension1Ships == null)
            return null;

        foreach (D1ShipState ship in gs.dimension1Ships)
        {
            if (ship != null && ship.shipId == shipId)
                return ship;
        }

        return null;
    }

    public void OnClickUpgradePlanet1()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        Dimension1System.TryUpgradeExtractor(
            gs,
            Dimension1System.Planet01
        );

        RefreshUI();
    }

        public void OnClickUnlockPlanet2()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        Dimension1System.TryUnlockPlanet(
            gs,
            Dimension1System.Planet02
        );

        RefreshUI();
    }

    public void OnClickUpgradePlanet2()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        Dimension1System.TryUpgradeExtractor(
            gs,
            Dimension1System.Planet02
        );

        RefreshUI();
    }

    public void OnClickUnlockPlanet3()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        Dimension1System.TryUnlockPlanet(
            gs,
            Dimension1System.Planet03
        );

        RefreshUI();
    }

    public void OnClickUpgradePlanet3()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        Dimension1System.TryUpgradeExtractor(
            gs,
            Dimension1System.Planet03
        );

        RefreshUI();
    }

    public void OnClickUnlockPlanet4()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        Dimension1System.TryUnlockPlanet(gs, Dimension1System.Planet04);

        RefreshUI();
    }

    public void OnClickUpgradePlanet4()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        Dimension1System.TryUpgradeExtractor(gs, Dimension1System.Planet04);

        RefreshUI();
    }

    public void OnClickUnlockPlanet5()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        Dimension1System.TryUnlockPlanet(gs, Dimension1System.Planet05);

        RefreshUI();
    }

    public void OnClickUpgradePlanet5()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        Dimension1System.TryUpgradeExtractor(gs, Dimension1System.Planet05);

        RefreshUI();
    }

    public void OnClickUnlockPlanet6()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        Dimension1System.TryUnlockPlanet(gs, Dimension1System.Planet06);

        RefreshUI();
    }

    public void OnClickUpgradePlanet6()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        Dimension1System.TryUpgradeExtractor(gs, Dimension1System.Planet06);

        RefreshUI();
    }

    public void OnClickUnlockPlanet7()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        Dimension1System.TryUnlockPlanet(gs, Dimension1System.Planet07);

        RefreshUI();
    }

    public void OnClickUpgradePlanet7()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        Dimension1System.TryUpgradeExtractor(gs, Dimension1System.Planet07);

        RefreshUI();
    }

    public void OnClickScanSimpleDestination()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        bool scanStarted = Dimension1System.TryScanSimpleDestination(gs);

        if (scanStarted)
        {
            selectedDestinationIndex = 0;
            selectedShipIndex = 0;

            if (destinationDropdown != null)
            {
                destinationDropdown.SetValueWithoutNotify(0);
                destinationDropdown.RefreshShownValue();
            }

            if (shipDropdown != null)
            {
                shipDropdown.SetValueWithoutNotify(0);
                shipDropdown.RefreshShownValue();
            }
        }

        RefreshUI();
    }

    public void OnClickUpgradeScanner()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        Dimension1System.TryUpgradeSimpleScanner(gs);

        RefreshUI();
    }

    public void OnClickStartLightProbeExploration()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        int destinationIndex = GetSelectedAvailableDestinationRealIndex(gs);

        if (destinationIndex < 0)
            return;

        D1ShipState selectedShip = GetSelectedAvailableShip(gs);

        if (selectedShip == null)
            return;

        bool explorationStarted =

        Dimension1System.TryStartExploration(gs, selectedShip.shipId, destinationIndex);

        if (!explorationStarted)
        {
            RefreshUI();
            return;
        }

        showingExplorationResultPanel = false;

        selectedDestinationIndex = 0;
        selectedShipIndex = 0;

        RefreshUI();
    }

        public void OnClickCloseExplorationRewards()
    {
        showingExplorationResultPanel = false;
        selectedDestinationIndex = 0;
        selectedShipIndex = 0;

        if (destinationDropdown != null)
        {
            destinationDropdown.SetValueWithoutNotify(0);
            destinationDropdown.RefreshShownValue();
        }

        if (shipDropdown != null)
        {
            shipDropdown.SetValueWithoutNotify(0);
            shipDropdown.RefreshShownValue();
        }

        RefreshUI();
    }

    public void OnClickUnlockExtractorDrone()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        Dimension1System.TryUnlockShip(
            gs,
            Dimension1System.ShipExtractorDrone
        );

        RefreshUI();
    }

    public void OnClickUnlockAnalyticProbe()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        Dimension1System.TryUnlockShip(
            gs,
            Dimension1System.ShipAnalyticProbe
        );

        RefreshUI();
    }

    public void OnClickUnlockCargoShip()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        Dimension1System.TryUnlockShip(
            gs,
            Dimension1System.ShipCargoShip
        );

        RefreshUI();
    }

        public void OnClickUnlockRescueShip()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        Dimension1System.TryUnlockShip(
            gs,
            Dimension1System.ShipRescueShip
        );

        RefreshUI();
    }

        public void OnClickUnlockConvergenceShip()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        Dimension1System.TryUnlockShip(
            gs,
            Dimension1System.ShipConvergenceShip
        );

        RefreshUI();
    }

    public void OnClickUpgradeLightProbeSpeed1()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        TryBuyAnyShipPartUpgrade(
            gs,
            Dimension1System.ShipLightProbe,
            Dimension1System.ShipPartSpeed
        );
        RefreshUI();
    }

    public void OnClickUpgradeExtractorDroneCargo1()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        TryBuyAnyShipPartUpgrade(
            gs,
            Dimension1System.ShipExtractorDrone,
            Dimension1System.ShipPartCargo
        );
        RefreshUI();
    }

        public void OnClickUpgradeExtractorDroneSpeed1()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        TryBuyAnyShipPartUpgrade(
            gs,
            Dimension1System.ShipExtractorDrone,
            Dimension1System.ShipPartSpeed
        );

        RefreshUI();
    }

    public void OnClickUpgradeAnalyticProbeSensors1()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        TryBuyAnyShipPartUpgrade(
            gs,
            Dimension1System.ShipAnalyticProbe,
            Dimension1System.ShipPartSensors
        );
        RefreshUI();
    }

    public void OnClickUpgradeCargoShipCargo1()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        TryBuyAnyShipPartUpgrade(
            gs,
            Dimension1System.ShipCargoShip,
            Dimension1System.ShipPartCargo
        );

        RefreshUI();
    }

    public void OnClickUpgradeRescueShipSpeed1()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        TryBuyAnyShipPartUpgrade(
            gs,
            Dimension1System.ShipRescueShip,
            Dimension1System.ShipPartSpeed
        );

        RefreshUI();
    }

    public void OnClickUpgradeConvergenceShipSpeed1()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        TryBuyAnyShipPartUpgrade(
            gs,
            Dimension1System.ShipConvergenceShip,
            Dimension1System.ShipPartSpeed
        );

        RefreshUI();
    }

    public void OnDestinationDropdownChanged(int index)
    {
        if (isRefreshingDestinationDropdown)
            return;

        selectedDestinationIndex = index;
        showingExplorationResultPanel = false;

        RefreshUI();
    }

    public void OnShipDropdownChanged(int index)
    {
        if (isRefreshingShipDropdown)
            return;

        selectedShipIndex = index;
        showingExplorationResultPanel = false;

        RefreshUI();
    }

    private void RefreshButtons(GameState gs)
    {
        if (gs == null)
            return;

        RefreshUpgradeButton(
            upgradePlanet1Button,
            gs,
            Dimension1System.Planet01,
            "Mejorar P1"
        );

        RefreshPlanet2Buttons(gs);
        RefreshPlanet3Buttons(gs);
        RefreshPlanet4Buttons(gs);
        RefreshPlanet5Buttons(gs);
        RefreshPlanet6Buttons(gs);
        RefreshPlanet7Buttons(gs);
        RefreshExtractorDroneButton(gs);
        RefreshShipUpgradeButtons(gs);
        RefreshAnalyticProbeButton(gs);
        RefreshCargoShipButton(gs);
        RefreshRescueShipButton(gs);
        RefreshConvergenceShipButton(gs);
        RefreshScannerUpgradeButton(gs);

        if (scanButton != null)
        {
            scanButton.interactable = Dimension1System.CanScanSimpleDestination(gs);

            if (gs.dimension1ScanActive)
                SetButtonText(scanButton, "Escaneando...");
            else
                SetButtonText(scanButton, "Escanear");
        }

        if (exploreButton != null)
        {
            int destinationIndex = GetSelectedAvailableDestinationRealIndex(gs);
            D1ShipState selectedShip = GetSelectedAvailableShip(gs);

            exploreButton.interactable =
                destinationIndex >= 0 &&
                selectedShip != null &&
                Dimension1System.CanStartExploration(gs, selectedShip.shipId, destinationIndex);

            SetButtonText(exploreButton, "Explorar");
        }
    }

    private void RefreshScannerUpgradeButton(GameState gs)
    {
        if (upgradeScannerButton == null)
            return;

        bool hasUpgrade =
            Dimension1System.TryGetNextSimpleScannerUpgradeCost(
                gs,
                out int nextLevel,
                out string metal1,
                out double amount1,
                out string metal2,
                out double amount2,
                out string metal3,
                out double amount3,
                out string metal4,
                out double amount4
            );

        upgradeScannerButton.gameObject.SetActive(true);

        if (!hasUpgrade)
        {
            SetButtonText(
                upgradeScannerButton,
                "Escáner máximo\n5/5 destinos"
            );

            upgradeScannerButton.interactable = false;
            return;
        }

        upgradeScannerButton.interactable =
            Dimension1System.CanUpgradeSimpleScanner(gs);

        SetButtonText(
            upgradeScannerButton,
            BuildScannerUpgradeButtonText(
                nextLevel,
                metal1,
                amount1,
                metal2,
                amount2,
                metal3,
                amount3,
                metal4,
                amount4
            )
        );
    }

    private bool HasStartedReceivingShipUnlockMetals(GameState gs, string shipId)
    {
        if (gs == null)
            return false;

        if (shipId == Dimension1System.ShipExtractorDrone)
        {
            return Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalIron) &&
                Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalCopper);
        }

        if (shipId == Dimension1System.ShipAnalyticProbe)
        {
            return Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalCopper) &&
                Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalAluminum);
        }

        if (shipId == Dimension1System.ShipCargoShip)
        {
            return Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalTitanium) &&
                Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalNickel);
        }

        if (shipId == Dimension1System.ShipRescueShip)
        {
            return Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalTitanium) &&
                Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalNickel) &&
                Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalCobalt) &&
                Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalPlatinum);
        }

        if (shipId == Dimension1System.ShipConvergenceShip)
        {
            return Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalPlatinum) &&
                Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalIridium) &&
                Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalCobalt) &&
                Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalTungsten);
        }

        return false;
    }

    private void RefreshExtractorDroneButton(GameState gs)
    {
        if (unlockExtractorDroneButton == null)
            return;

        D1ShipState drone = FindShip(gs, Dimension1System.ShipExtractorDrone);
        bool unlocked = drone != null && drone.unlocked;

        bool hasRequiredMetalsStarted =
        HasStartedReceivingShipUnlockMetals(gs, Dimension1System.ShipExtractorDrone);

        unlockExtractorDroneButton.gameObject.SetActive(!unlocked && hasRequiredMetalsStarted);
        unlockExtractorDroneButton.interactable =
            Dimension1System.CanUnlockShip(gs, Dimension1System.ShipExtractorDrone);

        SetButtonText(
            unlockExtractorDroneButton,
            "Desbloquear Dron\n300 Hierro + 80 Cobre"
        );
    }

    private void RefreshAnalyticProbeButton(GameState gs)
    {
        if (unlockAnalyticProbeButton == null)
            return;

        D1ShipState analyticProbe = FindShip(gs, Dimension1System.ShipAnalyticProbe);
        bool analyticUnlocked = analyticProbe != null && analyticProbe.unlocked;

        bool hasRequiredMetalsStarted =

        HasStartedReceivingShipUnlockMetals(gs, Dimension1System.ShipAnalyticProbe);

        unlockAnalyticProbeButton.gameObject.SetActive(!analyticUnlocked && hasRequiredMetalsStarted);

        unlockAnalyticProbeButton.interactable =
            Dimension1System.CanUnlockShip(gs, Dimension1System.ShipAnalyticProbe);

        SetButtonText(
            unlockAnalyticProbeButton,
            "Desbloquear Sonda Analítica\n250 Cobre + 120 Aluminio"
        );
    }

    private void RefreshCargoShipButton(GameState gs)
    {
        if (unlockCargoShipButton == null)
            return;

        D1ShipState analyticProbe = FindShip(gs, Dimension1System.ShipAnalyticProbe);
        bool analyticUnlocked = analyticProbe != null && analyticProbe.unlocked;

        D1ShipState cargoShip = FindShip(gs, Dimension1System.ShipCargoShip);
        bool cargoShipUnlocked = cargoShip != null && cargoShip.unlocked;

        bool hasRequiredMetalsStarted =
        
        HasStartedReceivingShipUnlockMetals(gs, Dimension1System.ShipCargoShip);

        unlockCargoShipButton.gameObject.SetActive(
            !cargoShipUnlocked &&
            hasRequiredMetalsStarted
        );

        unlockCargoShipButton.interactable =
            Dimension1System.CanUnlockShip(gs, Dimension1System.ShipCargoShip);

        SetButtonText(
            unlockCargoShipButton,
            "Construir Nave de Carga\n800 Titanio + 450 Níquel + 3 blueprints genéricos"
        );
    }

        private void RefreshRescueShipButton(GameState gs)
    {
        if (unlockRescueShipButton == null)
            return;

        D1ShipState rescueShip = FindShip(gs, Dimension1System.ShipRescueShip);
        bool rescueShipUnlocked = rescueShip != null && rescueShip.unlocked;

        bool hasRequiredMetalsStarted =
            HasStartedReceivingShipUnlockMetals(gs, Dimension1System.ShipRescueShip);

        unlockRescueShipButton.gameObject.SetActive(
            !rescueShipUnlocked &&
            hasRequiredMetalsStarted
        );

        unlockRescueShipButton.interactable =
            Dimension1System.CanUnlockShip(gs, Dimension1System.ShipRescueShip);

        SetButtonText(
            unlockRescueShipButton,
            "Construir Nave de Rescate\n1200 Titanio + 900 Níquel + 500 Cobalto + 250 Platino + 4 blueprints genéricos"
        );
    }

        private void RefreshConvergenceShipButton(GameState gs)
    {
        if (unlockConvergenceShipButton == null)
            return;

        D1ShipState convergenceShip = FindShip(gs, Dimension1System.ShipConvergenceShip);
        bool convergenceShipUnlocked = convergenceShip != null && convergenceShip.unlocked;

        bool hasRequiredMetalsStarted =
            HasStartedReceivingShipUnlockMetals(gs, Dimension1System.ShipConvergenceShip);

        unlockConvergenceShipButton.gameObject.SetActive(
            !convergenceShipUnlocked &&
            hasRequiredMetalsStarted
        );

        unlockConvergenceShipButton.interactable =
            Dimension1System.CanUnlockShip(gs, Dimension1System.ShipConvergenceShip);

        SetButtonText(
            unlockConvergenceShipButton,
            "Construir Nave de Convergencia\n900 Platino + 550 Iridio + 900 Cobalto + 1200 Tungsteno + 4 blueprints genéricos"
        );
    }

    private string BuildShipUpgradeButtonText(
        GameState gs,
        string shipId,
        string partId,
        string partVisualName,
        string shipShortName
    )
    {
        bool hasNextUpgrade = Dimension1System.TryGetNextShipPartUpgradeCost(
            gs,
            shipId,
            partId,
            out int nextLevel,
            out string metal1,
            out double amount1,
            out string metal2,
            out double amount2
        );

        if (hasNextUpgrade)
        {
            string normalText =
                partVisualName +
                " " +
                ToRomanNumber(nextLevel) +
                " " +
                shipShortName +
                "\n" +
                BuildShipUpgradeCostText(metal1, amount1, metal2, amount2);

            if (!Dimension1System.CanUpgradeShipPart(gs, shipId, partId))
                normalText += "\nRecursos insuficientes";

            return normalText;
        }

        bool hasAdvancedUpgrade = Dimension1System.TryGetNextAdvancedShipPartUpgradeCost(
            gs,
            shipId,
            partId,
            out int advancedNextLevel,
            out string advancedMetal1,
            out double advancedAmount1,
            out string advancedMetal2,
            out double advancedAmount2,
            out int blueprintCost
        );

        if (!hasAdvancedUpgrade)
            return partVisualName + " " + shipShortName + "\nMáximo";

        string advancedText =
            partVisualName +
            " " +
            ToRomanNumber(advancedNextLevel) +
            " " +
            shipShortName +
            "\n" +
            BuildShipAdvancedUpgradeCostText(
                advancedMetal1,
                advancedAmount1,
                advancedMetal2,
                advancedAmount2,
                blueprintCost
            );

        if (!Dimension1System.CanUpgradeAdvancedShipPart(gs, shipId, partId))
            advancedText += "\nRecursos insuficientes";

        return advancedText;
    }

    private string BuildShipUpgradeCostText(
        string metal1,
        double amount1,
        string metal2,
        double amount2
    )
    {
        string text = amount1.ToString("0") + " " + GetMetalVisualName(metal1);

        if (!string.IsNullOrEmpty(metal2) && amount2 > 0.0)
        {
            text += " + " + amount2.ToString("0") + " " + GetMetalVisualName(metal2);
        }

        return text;
    }

    private string BuildShipAdvancedUpgradeCostText(
        string metal1,
        double amount1,
        string metal2,
        double amount2,
        int blueprintCost
    )
    {
        string text = BuildShipUpgradeCostText(metal1, amount1, metal2, amount2);

        if (blueprintCost > 0)
        {
            text +=
                " + " +
                blueprintCost +
                " blueprint";

            if (blueprintCost > 1)
                text += "s";
        }

        return text;
    }

    private bool HasAnyShipPartUpgrade(GameState gs, string shipId, string partId)
    {
        bool hasNormalUpgrade = Dimension1System.TryGetNextShipPartUpgradeCost(
            gs,
            shipId,
            partId,
            out _,
            out _,
            out _,
            out _,
            out _
        );

        if (hasNormalUpgrade)
            return true;

        return Dimension1System.TryGetNextAdvancedShipPartUpgradeCost(
            gs,
            shipId,
            partId,
            out _,
            out _,
            out _,
            out _,
            out _,
            out _
        );
    }

    private bool CanBuyAnyShipPartUpgrade(GameState gs, string shipId, string partId)
    {
        if (Dimension1System.CanUpgradeShipPart(gs, shipId, partId))
            return true;

        return Dimension1System.CanUpgradeAdvancedShipPart(gs, shipId, partId);
    }

    private void TryBuyAnyShipPartUpgrade(GameState gs, string shipId, string partId)
    {
        if (Dimension1System.TryUpgradeShipPart(gs, shipId, partId))
            return;

        Dimension1System.TryUpgradeAdvancedShipPart(gs, shipId, partId);
    }

    private string BuildScannerUpgradeButtonText(
    int nextLevel,
    string metal1,
    double amount1,
    string metal2,
    double amount2,
    string metal3,
    double amount3,
    string metal4,
    double amount4
    )
    {
        string text =
            "Mejorar Escáner " +
            ToRomanNumber(nextLevel) +
            "\n";

        text += BuildMetalCostText(metal1, amount1);

        if (!string.IsNullOrEmpty(metal2) && amount2 > 0.0)
            text += " + " + BuildMetalCostText(metal2, amount2);

        if (!string.IsNullOrEmpty(metal3) && amount3 > 0.0)
            text += " + " + BuildMetalCostText(metal3, amount3);

        if (!string.IsNullOrEmpty(metal4) && amount4 > 0.0)
            text += " + " + BuildMetalCostText(metal4, amount4);

        return text;
    }

    private string BuildMetalCostText(string metalId, double amount)
    {
        if (string.IsNullOrEmpty(metalId) || amount <= 0.0)
            return "";

        return amount.ToString("0") + " " + GetMetalVisualName(metalId);
    }

    private string ToRomanNumber(int value)
    {
        switch (value)
        {
            case 1:
                return "I";

            case 2:
                return "II";

            case 3:
                return "III";

            case 4:
                return "IV";

            case 5:
                return "V";

            case 6:
                return "VI";

            default:
                return value.ToString();
        }
    }

    private void RefreshShipUpgradeButtons(GameState gs)
    {
        HideLegacyShipUpgradeButtons();
    }

    private void HideLegacyShipUpgradeButtons()
    {
        SetButtonActive(upgradeLightProbeSpeed1Button, false);
        SetButtonActive(upgradeExtractorDroneCargo1Button, false);
        SetButtonActive(upgradeAnalyticProbeSensors1Button, false);
        SetButtonActive(upgradeCargoShipCargo1Button, false);
        SetButtonActive(upgradeRescueShipSpeed1Button, false);
        SetButtonActive(upgradeConvergenceShipSpeed1Button, false);
    }

    private void SetButtonActive(Button button, bool active)
    {
        if (button == null)
            return;

        button.gameObject.SetActive(active);
    }

    private void RefreshLightProbeSpeed1Button(GameState gs)
    {
        if (upgradeLightProbeSpeed1Button == null)
            return;

        D1ShipState ship = FindShip(gs, Dimension1System.ShipLightProbe);
        bool unlocked = ship != null && ship.unlocked;

        bool hasNextUpgrade = HasAnyShipPartUpgrade(
            gs,
            Dimension1System.ShipLightProbe,
            Dimension1System.ShipPartSpeed
        );

        upgradeLightProbeSpeed1Button.gameObject.SetActive(unlocked && hasNextUpgrade);

        upgradeLightProbeSpeed1Button.interactable =
            CanBuyAnyShipPartUpgrade(
                gs,
                Dimension1System.ShipLightProbe,
                Dimension1System.ShipPartSpeed
            );

        SetButtonText(
            upgradeLightProbeSpeed1Button,
            BuildShipUpgradeButtonText(
                gs,
                Dimension1System.ShipLightProbe,
                Dimension1System.ShipPartSpeed,
                "Velocidad",
                "Sonda"
            )
        );
    }

    private void RefreshExtractorDroneCargo1Button(GameState gs)
    {
        if (upgradeExtractorDroneCargo1Button == null)
            return;

        D1ShipState ship = FindShip(gs, Dimension1System.ShipExtractorDrone);
        bool unlocked = ship != null && ship.unlocked;

        bool hasNextUpgrade = HasAnyShipPartUpgrade(
            gs,
            Dimension1System.ShipExtractorDrone,
            Dimension1System.ShipPartCargo
        );

        upgradeExtractorDroneCargo1Button.gameObject.SetActive(unlocked && hasNextUpgrade);

        upgradeExtractorDroneCargo1Button.interactable =
            CanBuyAnyShipPartUpgrade(
                gs,
                Dimension1System.ShipExtractorDrone,
                Dimension1System.ShipPartCargo
            );

        SetButtonText(
            upgradeExtractorDroneCargo1Button,
            BuildShipUpgradeButtonText(
                gs,
                Dimension1System.ShipExtractorDrone,
                Dimension1System.ShipPartCargo,
                "Carga",
                "Dron"
            )
        );
    }

        private void RefreshExtractorDroneSpeed1Button(GameState gs)
    {
        if (upgradeExtractorDroneSpeed1Button == null)
            return;

        D1ShipState ship = FindShip(gs, Dimension1System.ShipExtractorDrone);
        bool unlocked = ship != null && ship.unlocked;

        bool hasNextUpgrade = HasAnyShipPartUpgrade(
            gs,
            Dimension1System.ShipExtractorDrone,
            Dimension1System.ShipPartSpeed
        );

        upgradeExtractorDroneSpeed1Button.gameObject.SetActive(unlocked && hasNextUpgrade);

        upgradeExtractorDroneSpeed1Button.interactable =
            CanBuyAnyShipPartUpgrade(
                gs,
                Dimension1System.ShipExtractorDrone,
                Dimension1System.ShipPartSpeed
            );

        SetButtonText(
            upgradeExtractorDroneSpeed1Button,
            BuildShipUpgradeButtonText(
                gs,
                Dimension1System.ShipExtractorDrone,
                Dimension1System.ShipPartSpeed,
                "Velocidad",
                "Dron"
            )
        );
    }

    private void RefreshAnalyticProbeSensors1Button(GameState gs)
    {
        if (upgradeAnalyticProbeSensors1Button == null)
            return;

        D1ShipState ship = FindShip(gs, Dimension1System.ShipAnalyticProbe);
        bool unlocked = ship != null && ship.unlocked;

        bool hasNextUpgrade = HasAnyShipPartUpgrade(
            gs,
            Dimension1System.ShipAnalyticProbe,
            Dimension1System.ShipPartSensors
        );

        upgradeAnalyticProbeSensors1Button.gameObject.SetActive(unlocked && hasNextUpgrade);

        upgradeAnalyticProbeSensors1Button.interactable =
            CanBuyAnyShipPartUpgrade(
                gs,
                Dimension1System.ShipAnalyticProbe,
                Dimension1System.ShipPartSensors
            );

        SetButtonText(
            upgradeAnalyticProbeSensors1Button,
            BuildShipUpgradeButtonText(
                gs,
                Dimension1System.ShipAnalyticProbe,
                Dimension1System.ShipPartSensors,
                "Sensores",
                "Analítica"
            )
        );
    }

    private void RefreshCargoShipCargo1Button(GameState gs)
    {
        if (upgradeCargoShipCargo1Button == null)
            return;

        D1ShipState ship = FindShip(gs, Dimension1System.ShipCargoShip);
        bool unlocked = ship != null && ship.unlocked;

        bool hasNextUpgrade = HasAnyShipPartUpgrade(
            gs,
            Dimension1System.ShipCargoShip,
            Dimension1System.ShipPartCargo
        );

        upgradeCargoShipCargo1Button.gameObject.SetActive(unlocked && hasNextUpgrade);

        upgradeCargoShipCargo1Button.interactable =
            CanBuyAnyShipPartUpgrade(
                gs,
                Dimension1System.ShipCargoShip,
                Dimension1System.ShipPartCargo
            );

        SetButtonText(
            upgradeCargoShipCargo1Button,
            BuildShipUpgradeButtonText(
                gs,
                Dimension1System.ShipCargoShip,
                Dimension1System.ShipPartCargo,
                "Carga",
                "Nave Carga"
            )
        );
    }

    private void RefreshRescueShipSpeed1Button(GameState gs)
    {
        if (upgradeRescueShipSpeed1Button == null)
            return;

        D1ShipState ship = FindShip(gs, Dimension1System.ShipRescueShip);
        bool unlocked = ship != null && ship.unlocked;

        bool hasNextUpgrade = HasAnyShipPartUpgrade(
            gs,
            Dimension1System.ShipRescueShip,
            Dimension1System.ShipPartSpeed
        );

        upgradeRescueShipSpeed1Button.gameObject.SetActive(unlocked && hasNextUpgrade);

        upgradeRescueShipSpeed1Button.interactable =
            CanBuyAnyShipPartUpgrade(
                gs,
                Dimension1System.ShipRescueShip,
                Dimension1System.ShipPartSpeed
            );

        SetButtonText(
            upgradeRescueShipSpeed1Button,
            BuildShipUpgradeButtonText(
                gs,
                Dimension1System.ShipRescueShip,
                Dimension1System.ShipPartSpeed,
                "Velocidad",
                "Rescate"
            )
        );
    }

    private void RefreshConvergenceShipSpeed1Button(GameState gs)
    {
        if (upgradeConvergenceShipSpeed1Button == null)
            return;

        D1ShipState ship = FindShip(gs, Dimension1System.ShipConvergenceShip);
        bool unlocked = ship != null && ship.unlocked;

        bool hasNextUpgrade = HasAnyShipPartUpgrade(
            gs,
            Dimension1System.ShipConvergenceShip,
            Dimension1System.ShipPartSpeed
        );

        upgradeConvergenceShipSpeed1Button.gameObject.SetActive(unlocked && hasNextUpgrade);

        upgradeConvergenceShipSpeed1Button.interactable =
            CanBuyAnyShipPartUpgrade(
                gs,
                Dimension1System.ShipConvergenceShip,
                Dimension1System.ShipPartSpeed
            );

        SetButtonText(
            upgradeConvergenceShipSpeed1Button,
            BuildShipUpgradeButtonText(
                gs,
                Dimension1System.ShipConvergenceShip,
                Dimension1System.ShipPartSpeed,
                "Velocidad",
                "Convergencia"
            )
        );
    }

    private bool HasStartedReceivingPlanetUnlockMetals(GameState gs, string planetId)
    {
        if (gs == null)
            return false;

        if (planetId == Dimension1System.Planet02)
        {
            return Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalIron) &&
                Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalCopper);
        }

        if (planetId == Dimension1System.Planet03)
        {
            return Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalAluminum) &&
                Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalTitanium);
        }

        if (planetId == Dimension1System.Planet04)
        {
            return Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalNickel) &&
                Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalCobalt);
        }

        if (planetId == Dimension1System.Planet05)
        {
            return Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalLithium) &&
                Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalTungsten);
        }

        if (planetId == Dimension1System.Planet06)
        {
            return Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalPlatinum) &&
                Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalNickel);
        }

        if (planetId == Dimension1System.Planet07)
        {
            return Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalIridium) &&
                Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalCobalt) &&
                Dimension1System.IsMetalUnlockedForDimension1(gs, Dimension1System.MetalTungsten);
        }

        return false;
    }

    private void RefreshPlanet2Buttons(GameState gs)
    {
        D1PlanetState planet = FindPlanet(gs, Dimension1System.Planet02);
        bool unlocked = planet != null && planet.unlocked;

        if (unlockPlanet2Button != null)
        {
            bool hasRequiredMetalsStarted =
            HasStartedReceivingPlanetUnlockMetals(gs, Dimension1System.Planet02);
            unlockPlanet2Button.gameObject.SetActive(!unlocked && hasRequiredMetalsStarted);
            unlockPlanet2Button.interactable = Dimension1System.CanUnlockPlanet(gs, Dimension1System.Planet02);
            SetButtonText(unlockPlanet2Button, "Desbloquear P2\n500 Hierro + 120 Cobre");
        }

        if (upgradePlanet2Button != null)
        {
            upgradePlanet2Button.gameObject.SetActive(unlocked);
            RefreshUpgradeButton(
                upgradePlanet2Button,
                gs,
                Dimension1System.Planet02,
                "Mejorar P2"
            );
        }
    }

    private void RefreshPlanet3Buttons(GameState gs)
    {
        D1PlanetState planet = FindPlanet(gs, Dimension1System.Planet03);
        bool unlocked = planet != null && planet.unlocked;

        if (unlockPlanet3Button != null)
        {
            bool hasRequiredMetalsStarted =
            HasStartedReceivingPlanetUnlockMetals(gs, Dimension1System.Planet03);
            unlockPlanet3Button.gameObject.SetActive(!unlocked && hasRequiredMetalsStarted);
            unlockPlanet3Button.interactable = Dimension1System.CanUnlockPlanet(gs, Dimension1System.Planet03);
            SetButtonText(unlockPlanet3Button, "Desbloquear P3\n1500 Aluminio + 300 Titanio");
        }

        if (upgradePlanet3Button != null)
        {
            upgradePlanet3Button.gameObject.SetActive(unlocked);
            RefreshUpgradeButton(
                upgradePlanet3Button,
                gs,
                Dimension1System.Planet03,
                "Mejorar P3"
            );
        }
    }

    private void RefreshPlanet4Buttons(GameState gs)
    {
        D1PlanetState planet = FindPlanet(gs, Dimension1System.Planet04);
        bool unlocked = planet != null && planet.unlocked;

        if (unlockPlanet4Button != null)
        {
            bool hasRequiredMetalsStarted =
                HasStartedReceivingPlanetUnlockMetals(gs, Dimension1System.Planet04);

            unlockPlanet4Button.gameObject.SetActive(!unlocked && hasRequiredMetalsStarted);
            unlockPlanet4Button.interactable = Dimension1System.CanUnlockPlanet(gs, Dimension1System.Planet04);
            SetButtonText(unlockPlanet4Button, "Desbloquear P4\n1200 Níquel + 350 Cobalto");
        }

        if (upgradePlanet4Button != null)
        {
            upgradePlanet4Button.gameObject.SetActive(unlocked);
            RefreshUpgradeButton(
                upgradePlanet4Button,
                gs,
                Dimension1System.Planet04,
                "Mejorar P4"
            );
        }
    }

    private void RefreshPlanet5Buttons(GameState gs)
    {
        D1PlanetState planet = FindPlanet(gs, Dimension1System.Planet05);
        bool unlocked = planet != null && planet.unlocked;

        if (unlockPlanet5Button != null)
        {
            bool hasRequiredMetalsStarted =
                HasStartedReceivingPlanetUnlockMetals(gs, Dimension1System.Planet05);

            unlockPlanet5Button.gameObject.SetActive(!unlocked && hasRequiredMetalsStarted);
            unlockPlanet5Button.interactable = Dimension1System.CanUnlockPlanet(gs, Dimension1System.Planet05);
            SetButtonText(unlockPlanet5Button, "Desbloquear P5\n1400 Litio + 320 Tungsteno");
        }

        if (upgradePlanet5Button != null)
        {
            upgradePlanet5Button.gameObject.SetActive(unlocked);
            RefreshUpgradeButton(
                upgradePlanet5Button,
                gs,
                Dimension1System.Planet05,
                "Mejorar P5"
            );
        }
    }

    private void RefreshPlanet6Buttons(GameState gs)
    {
        D1PlanetState planet = FindPlanet(gs, Dimension1System.Planet06);
        bool unlocked = planet != null && planet.unlocked;

        if (unlockPlanet6Button != null)
        {
            bool hasRequiredMetalsStarted =
                HasStartedReceivingPlanetUnlockMetals(gs, Dimension1System.Planet06);

            unlockPlanet6Button.gameObject.SetActive(!unlocked && hasRequiredMetalsStarted);
            unlockPlanet6Button.interactable = Dimension1System.CanUnlockPlanet(gs, Dimension1System.Planet06);
            SetButtonText(unlockPlanet6Button, "Desbloquear P6\n1100 Platino + 1800 Níquel");
        }

        if (upgradePlanet6Button != null)
        {
            upgradePlanet6Button.gameObject.SetActive(unlocked);
            RefreshUpgradeButton(
                upgradePlanet6Button,
                gs,
                Dimension1System.Planet06,
                "Mejorar P6"
            );
        }
    }

    private void RefreshPlanet7Buttons(GameState gs)
    {
        D1PlanetState planet = FindPlanet(gs, Dimension1System.Planet07);
        bool unlocked = planet != null && planet.unlocked;

        if (unlockPlanet7Button != null)
        {
            bool hasRequiredMetalsStarted =
                HasStartedReceivingPlanetUnlockMetals(gs, Dimension1System.Planet07);

            unlockPlanet7Button.gameObject.SetActive(!unlocked && hasRequiredMetalsStarted);
            unlockPlanet7Button.interactable = Dimension1System.CanUnlockPlanet(gs, Dimension1System.Planet07);
            SetButtonText(unlockPlanet7Button, "Desbloquear P7\n800 Iridio + 1500 Cobalto + 900 Tungsteno");
        }

        if (upgradePlanet7Button != null)
        {
            upgradePlanet7Button.gameObject.SetActive(unlocked);
            RefreshUpgradeButton(
                upgradePlanet7Button,
                gs,
                Dimension1System.Planet07,
                "Mejorar P7"
            );
        }
    }

    private void RefreshUpgradeButton(
        Button button,
        GameState gs,
        string planetId,
        string baseText
    )
    {
        if (button == null)
            return;

        D1PlanetState planet = FindPlanet(gs, planetId);

        if (planet == null || !planet.unlocked)
        {
            button.interactable = false;
            SetButtonText(button, baseText);
            return;
        }

        double cost = Dimension1System.GetExtractorUpgradeCost(planet);
        string metalId = Dimension1System.GetExtractorUpgradeMainCostMetal(planet);
        string metalName = GetMetalVisualName(metalId);

        button.interactable = Dimension1System.CanUpgradeExtractor(gs, planetId);

        SetButtonText(
            button,
            baseText + "\n" + cost.ToString("0") + " " + metalName
        );
    }

        public void OnClickOpenHangarPanel()
    {
        hangarPanelOpen = true;
        RefreshUI();
    }

    public void OnClickCloseHangarPanel()
    {
        hangarPanelOpen = false;
        RefreshUI();
    }

    public void OnHangarShipDropdownChanged(int index)
    {
        if (isRefreshingHangarShipDropdown)
            return;

        selectedHangarShipIndex = index;
        RefreshUI();
    }

    public void OnClickUpgradeHangarShipCargo()
    {
        TryUpgradeSelectedHangarShipPart(Dimension1System.ShipPartCargo);
    }

    public void OnClickUpgradeHangarShipSpeed()
    {
        TryUpgradeSelectedHangarShipPart(Dimension1System.ShipPartSpeed);
    }

    public void OnClickUpgradeHangarShipArmor()
    {
        TryUpgradeSelectedHangarShipPart(Dimension1System.ShipPartArmor);
    }

    public void OnClickUpgradeHangarShipSensors()
    {
        TryUpgradeSelectedHangarShipPart(Dimension1System.ShipPartSensors);
    }

    private void TryUpgradeSelectedHangarShipPart(string partId)
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        string shipId = GetSelectedHangarShipId();

        if (string.IsNullOrEmpty(shipId))
            return;

        TryBuyAnyShipPartUpgrade(gs, shipId, partId);
        RefreshUI();
    }

    private void RefreshHangarPanel(GameState gs)
    {
        if (dimension1MainContentRoot != null)
            dimension1MainContentRoot.SetActive(!hangarPanelOpen);

        if (openHangarPanelButton != null)
        {
            openHangarPanelButton.gameObject.SetActive(!hangarPanelOpen);
            SetButtonText(openHangarPanelButton, "Hangar");
        }

        if (hangarPanel == null)
            return;

        hangarPanel.SetActive(hangarPanelOpen);

        if (!hangarPanelOpen)
            return;

        RefreshHangarShipDropdown(gs);

        string shipId = GetSelectedHangarShipId();

        if (hangarInfoText != null)
            hangarInfoText.text = BuildHangarInfoText(gs, shipId);

        RefreshHangarPartUpgradeButton(
            hangarUpgradeCargoButton,
            gs,
            shipId,
            Dimension1System.ShipPartCargo,
            "Carga"
        );

        RefreshHangarPartUpgradeButton(
            hangarUpgradeSpeedButton,
            gs,
            shipId,
            Dimension1System.ShipPartSpeed,
            "Velocidad"
        );

        RefreshHangarPartUpgradeButton(
            hangarUpgradeArmorButton,
            gs,
            shipId,
            Dimension1System.ShipPartArmor,
            "Blindaje"
        );

        RefreshHangarPartUpgradeButton(
            hangarUpgradeSensorsButton,
            gs,
            shipId,
            Dimension1System.ShipPartSensors,
            "Sensores"
        );

        if (closeHangarPanelButton != null)
            SetButtonText(closeHangarPanelButton, "Cerrar");
    }

    private void RefreshHangarShipDropdown(GameState gs)
    {
        if (hangarShipDropdown == null)
            return;

        List<string> options = new List<string>();

        for (int i = 0; i < HangarShipIds.Length; i++)
        {
            string shipId = HangarShipIds[i];
            D1ShipState ship = FindShip(gs, shipId);

            string optionText = GetShipVisualName(shipId);

            if (ship == null || !ship.unlocked)
                optionText += " (bloqueada)";

            options.Add(optionText);
        }

        string newSignature = "";

        for (int i = 0; i < options.Count; i++)
        {
            if (i > 0)
                newSignature += "|";

            newSignature += options[i];
        }

        isRefreshingHangarShipDropdown = true;

        if (newSignature != hangarShipDropdownSignature)
        {
            hangarShipDropdown.ClearOptions();
            hangarShipDropdown.AddOptions(options);
            hangarShipDropdownSignature = newSignature;
        }

        if (selectedHangarShipIndex < 0 || selectedHangarShipIndex >= options.Count)
            selectedHangarShipIndex = 0;

        hangarShipDropdown.SetValueWithoutNotify(selectedHangarShipIndex);
        hangarShipDropdown.RefreshShownValue();

        isRefreshingHangarShipDropdown = false;
    }

    private string GetSelectedHangarShipId()
    {
        int index = selectedHangarShipIndex;

        if (hangarShipDropdown != null)
            index = hangarShipDropdown.value;

        if (index < 0 || index >= HangarShipIds.Length)
            return HangarShipIds[0];

        return HangarShipIds[index];
    }

    private string BuildHangarInfoText(GameState gs, string shipId)
    {
        if (gs == null || string.IsNullOrEmpty(shipId))
            return "Hangar\nNo disponible.";

        D1ShipState ship = FindShip(gs, shipId);

        string status = "Bloqueada";

        if (ship != null && ship.unlocked)
        {
            status = ship.explorationActive
                ? "Explorando"
                : "Disponible";
        }

        return
            "Hangar\n\n" +
            "Nave: " +
            GetShipVisualName(shipId) +
            "\nEstado: " +
            status +
            "\nRol: " +
            GetShipRoleText(shipId) +
            "\n\nStats:\n" +
            "Carga: " +
            FormatShipUpgradeLevel(GetShipPartLevelForUI(ship, Dimension1System.ShipPartCargo)) +
            "/VI\n" +
            "Velocidad: " +
            FormatShipUpgradeLevel(GetShipPartLevelForUI(ship, Dimension1System.ShipPartSpeed)) +
            "/VI\n" +
            "Blindaje: " +
            FormatShipUpgradeLevel(GetShipPartLevelForUI(ship, Dimension1System.ShipPartArmor)) +
            "/VI\n" +
            "Sensores: " +
            FormatShipUpgradeLevel(GetShipPartLevelForUI(ship, Dimension1System.ShipPartSensors)) +
            "/VI";
    }

    private void RefreshHangarPartUpgradeButton(
        Button button,
        GameState gs,
        string shipId,
        string partId,
        string partVisualName
    )
    {
        if (button == null)
            return;

        button.gameObject.SetActive(true);

        D1ShipState ship = FindShip(gs, shipId);

        if (ship == null || !ship.unlocked)
        {
            button.interactable = false;
            SetButtonText(button, partVisualName + "\nNave bloqueada");
            return;
        }

        int currentLevel = GetShipPartLevelForUI(ship, partId);

        if (currentLevel >= 6)
        {
            button.interactable = false;
            SetButtonText(button, partVisualName + "\nMáximo");
            return;
        }

        bool hasNextUpgrade = HasAnyShipPartUpgrade(gs, shipId, partId);

        if (!hasNextUpgrade)
        {
            button.interactable = false;
            SetButtonText(button, partVisualName + "\nNo disponible todavía");
            return;
        }

        button.interactable = CanBuyAnyShipPartUpgrade(gs, shipId, partId);

        SetButtonText(
            button,
            BuildShipUpgradeButtonText(
                gs,
                shipId,
                partId,
                partVisualName,
                GetShipShortName(shipId)
            )
        );
    }

    private int GetShipPartLevelForUI(D1ShipState ship, string partId)
    {
        if (ship == null || string.IsNullOrEmpty(partId))
            return 0;

        switch (partId)
        {
            case Dimension1System.ShipPartCargo:
                return ship.cargoLevel;

            case Dimension1System.ShipPartSpeed:
                return ship.speedLevel;

            case Dimension1System.ShipPartArmor:
                return ship.armorLevel;

            case Dimension1System.ShipPartSensors:
                return ship.sensorsLevel;

            default:
                return 0;
        }
    }

    private string GetShipShortName(string shipId)
    {
        switch (shipId)
        {
            case Dimension1System.ShipLightProbe:
                return "Sonda";

            case Dimension1System.ShipExtractorDrone:
                return "Dron";

            case Dimension1System.ShipAnalyticProbe:
                return "Analítica";

            case Dimension1System.ShipCargoShip:
                return "Carga";

            case Dimension1System.ShipRescueShip:
                return "Rescate";

            case Dimension1System.ShipConvergenceShip:
                return "Convergencia";

            default:
                return "Nave";
        }
    }

    private string GetShipRoleText(string shipId)
    {
        switch (shipId)
        {
            case Dimension1System.ShipLightProbe:
                return "Nave inicial equilibrada.";

            case Dimension1System.ShipExtractorDrone:
                return "Especialista en recolección de materiales.";

            case Dimension1System.ShipAnalyticProbe:
                return "Especialista en análisis, blueprints y hallazgos.";

            case Dimension1System.ShipCargoShip:
                return "Transporte pesado y recompensas grandes.";

            case Dimension1System.ShipRescueShip:
                return "Recuperación segura y protección de hallazgos.";

            case Dimension1System.ShipConvergenceShip:
                return "Anomalías, destinos especiales y late game.";

            default:
                return "Sin rol definido.";
        }
    }

    private void SetButtonText(Button button, string text)
    {
        if (button == null)
            return;

        TextMeshProUGUI tmpLabel = button.GetComponentInChildren<TextMeshProUGUI>(true);

        if (tmpLabel != null)
        {
            tmpLabel.text = text;
            return;
        }

        UnityEngine.UI.Text legacyLabel = button.GetComponentInChildren<UnityEngine.UI.Text>(true);

        if (legacyLabel != null)
            legacyLabel.text = text;
    }
}

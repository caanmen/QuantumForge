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

    [Header("Escáner")]
    [SerializeField] private Button scanButton;
    [SerializeField] private TMP_Dropdown destinationDropdown;
    [SerializeField] private TMP_Dropdown shipDropdown;
    [SerializeField] private Button exploreButton;

    [Header("Hangar")]
    [SerializeField] private Button unlockExtractorDroneButton;
    [SerializeField] private Button unlockAnalyticProbeButton;

    [FormerlySerializedAs("unlockCargoShipButton")]
    [SerializeField] private Button unlockCargoShipButton;
    [SerializeField] private Button upgradeLightProbeSpeed1Button;
    [SerializeField] private Button upgradeExtractorDroneCargo1Button;
    [SerializeField] private Button upgradeAnalyticProbeSensors1Button;

    [Header("Rendimiento")]
    [SerializeField] private float refreshInterval = 0.25f;

    private float refreshTimer;
    private int selectedDestinationIndex;
    private string destinationDropdownSignature = "";
    private bool isRefreshingDestinationDropdown;
    private int selectedShipIndex;
    private string shipDropdownSignature = "";
    private bool isRefreshingShipDropdown;

    private void OnEnable()
    {
        RefreshUI();
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
            BuildLastExplorationText(gs) +
            "\n\n" +
            BuildBlueprintArchiveText(gs) +
            "\n\n" +
            BuildHangarText(gs);


        RefreshDestinationDropdown(gs);
        RefreshShipDropdown(gs);
        RefreshButtons(gs);

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
                BuildMetalLine(gs, Dimension1System.MetalCobalt, "Cobalto");
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
            BuildPlanetLine(gs, Dimension1System.Planet03, "Planeta 3", "Níquel", "Cobalto");
    }

    private string BuildPlanetLine(
        GameState gs,
        string planetId,
        string visualName,
        string mainMetalName,
        string secondaryMetalName
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

            default:
                return metalId;
        }
    }

    private string BuildScannerText(GameState gs)
    {
        if (gs == null)
            return "Galaxia / Escáner:\nNo disponible.";

        if (gs.dimension1ScanActive)
        {
            return
                "Galaxia / Escáner:\n" +
                "Escaneando galaxia...\n" +
                "Barrido restante: " +
                FormatSeconds(gs.dimension1ScanRemainingSeconds);
        }

        int destinationCount = GetAvailableDestinationCount(gs);

        if (destinationCount <= 0)
        {
            return
                "Galaxia / Escáner:\n" +
                "Ningún destino detectado.\n" +
                "Pulsa Escanear para hacer un barrido.";
        }

        return
            "Galaxia / Escáner:\n" +
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
            "\nDuración de exploración: " +
            GetDestinationDurationPreview(destination.destinationId, selectedShip) +
            "\nRecompensas posibles: " +
            GetDestinationRewardPreview(destination.destinationId) +
            "\nNaves compatibles: " +
            BuildUnlockedShipsText(gs) +
            "\nNave seleccionada: " +
            selectedShipName +
            "\nBonus de nave: " +
            GetShipBonusPreview(selectedShip);
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

        if (selectedDestinationIndex < 0 || selectedDestinationIndex >= destinationCount)
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

        if (selectedShipIndex < 0 || selectedShipIndex >= shipCount)
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

        int targetIndex = selectedDestinationIndex;

        if (destinationDropdown != null)
            targetIndex = destinationDropdown.value;

        if (targetIndex < 0)
            return null;

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

        int targetIndex = selectedShipIndex;

        if (shipDropdown != null)
            targetIndex = shipDropdown.value;

        if (targetIndex < 0)
            return null;

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
                    return "Sensores VI: 3 destinos y barrido 2.1s";

                if (ship.sensorsLevel >= 5)
                    return "Sensores V: 3 destinos y barrido 2.4s";

                if (ship.sensorsLevel >= 4)
                    return "Sensores IV: 3 destinos y barrido 2.7s";

                if (ship.sensorsLevel >= 3)
                    return "Sensores III: 3 destinos y barrido 3.0s";

                if (ship.sensorsLevel >= 2)
                    return "Sensores II: 3 destinos y barrido 3.5s";

                if (ship.sensorsLevel >= 1)
                    return "Sensores I: 3 destinos y barrido 4.0s";

                return "Sensores base: detecta 3 destinos";

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
                return "Bodega Modular base: x1.10 metales de exploración";

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
                return "Sonda Abandonada";

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
                break;

            case Dimension1System.DestinationShipGraveyard:
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalIron);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalCopper);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalAluminum);
                break;

            case Dimension1System.DestinationAbandonedProbe:
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalCopper);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalAluminum);
                break;

            case Dimension1System.DestinationCrystalDebris:
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalCopper);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalAluminum);
                break;    

            case Dimension1System.DestinationOrbitalArmorWreckage:
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalTitanium);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalNickel);
                break;    

            case Dimension1System.DestinationCollapsedReactor:
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalNickel);
                AddRewardPreviewIfUnlocked(gs, rewards, Dimension1System.MetalCobalt);
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

    private string GetDestinationDurationPreview(string destinationId, D1ShipState selectedShip)
    {
        double duration = 5.0;

        if (selectedShip != null &&
            selectedShip.shipId == Dimension1System.ShipLightProbe)
        {
            if (selectedShip.speedLevel >= 6)
                duration *= 0.58;
            else if (selectedShip.speedLevel >= 5)
                duration *= 0.64;
            else if (selectedShip.speedLevel >= 4)
                duration *= 0.70;
            else if (selectedShip.speedLevel >= 3)
                duration *= 0.76;
            else if (selectedShip.speedLevel >= 2)
                duration *= 0.82;
            else if (selectedShip.speedLevel >= 1)
                duration *= 0.90;
        }

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
            "\nBlueprints: " +
            completedBlueprints;

        return text;
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
            BuildShipLine(gs, Dimension1System.ShipCargoShip, "Nave de Carga");
    }

    private string BuildShipLine(GameState gs, string shipId, string visualName)
    {
        D1ShipState ship = FindShip(gs, shipId);

        if (ship == null || !ship.unlocked)
        {
            return visualName + ": bloqueada";
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
                return "Bodega Modular: Base | Metales: x1.10";      

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

    public void OnClickScanSimpleDestination()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        Dimension1System.TryScanSimpleDestination(gs);

        RefreshUI();
    }

    public void OnClickStartLightProbeExploration()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        int destinationIndex = selectedDestinationIndex;

        if (destinationDropdown != null)
            destinationIndex = destinationDropdown.value;

        D1ShipState selectedShip = GetSelectedAvailableShip(gs);

        if (selectedShip == null)
            return;

        Dimension1System.TryStartExploration(gs, selectedShip.shipId, destinationIndex);

        selectedDestinationIndex = 0;
        selectedShipIndex = 0;

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

    public void OnDestinationDropdownChanged(int index)
    {
        if (isRefreshingDestinationDropdown)
            return;

        selectedDestinationIndex = index;
    }

    public void OnShipDropdownChanged(int index)
    {
        if (isRefreshingShipDropdown)
            return;

        selectedShipIndex = index;
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
        RefreshExtractorDroneButton(gs);
        RefreshShipUpgradeButtons(gs);
        RefreshAnalyticProbeButton(gs);
        RefreshCargoShipButton(gs);

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
            int destinationIndex = selectedDestinationIndex;

            if (destinationDropdown != null)
                destinationIndex = destinationDropdown.value;

            D1ShipState selectedShip = GetSelectedAvailableShip(gs);

            exploreButton.interactable =
                selectedShip != null &&
                Dimension1System.CanStartExploration(gs, selectedShip.shipId, destinationIndex);

            SetButtonText(exploreButton, "Explorar");
        }
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
            "Desbloquear Nave de Carga\n800 Titanio + 450 Níquel + 3 Blueprints"
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
        RefreshLightProbeSpeed1Button(gs);
        RefreshExtractorDroneCargo1Button(gs);
        RefreshAnalyticProbeSensors1Button(gs);
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

    private void RefreshPlanet2Buttons(GameState gs)
    {
        D1PlanetState planet = FindPlanet(gs, Dimension1System.Planet02);
        bool unlocked = planet != null && planet.unlocked;

        if (unlockPlanet2Button != null)
        {
            unlockPlanet2Button.gameObject.SetActive(!unlocked);
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
            unlockPlanet3Button.gameObject.SetActive(!unlocked);
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

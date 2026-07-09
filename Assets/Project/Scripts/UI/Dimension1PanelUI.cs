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

    [Header("Registro de exploración")]
    [SerializeField] private Button openExplorationRecordButton;
    [SerializeField] private GameObject explorationRecordPanel;
    [SerializeField] private TextMeshProUGUI explorationRecordText;
    [SerializeField] private Button closeExplorationRecordButton;

    [Header("Hangar")]
    [SerializeField] private Button unlockExtractorDroneButton;
    [SerializeField] private Button unlockAnalyticProbeButton;

    [FormerlySerializedAs("unlockCargoShipButton")]
    [SerializeField] private Button unlockCargoShipButton;

    [SerializeField] private Button unlockRescueShipButton;
    [SerializeField] private Button unlockConvergenceShipButton;


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
    private bool explorationRecordPanelOpen;
    private int lastObservedDestinationDropdownValue = -1;
    private int lastObservedShipDropdownValue = -1;

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
        if (SyncMainExplorationDropdownChanges())
        {
            RefreshUI();
            return;
        }

        refreshTimer += Time.unscaledDeltaTime;

        if (refreshTimer < refreshInterval)
            return;

        refreshTimer = 0f;
        RefreshUI();
    }

    private bool SyncMainExplorationDropdownChanges()
    {
        bool changed = false;

        if (destinationDropdown != null && !isRefreshingDestinationDropdown)
        {
            int currentDestinationValue = destinationDropdown.value;

            if (lastObservedDestinationDropdownValue != currentDestinationValue)
            {
                lastObservedDestinationDropdownValue = currentDestinationValue;
                selectedDestinationIndex = currentDestinationValue;
                CloseExplorationOverlayPanels();
                changed = true;
            }
        }

        if (shipDropdown != null && !isRefreshingShipDropdown)
        {
            int currentShipValue = shipDropdown.value;

            if (lastObservedShipDropdownValue != currentShipValue)
            {
                lastObservedShipDropdownValue = currentShipValue;
                selectedShipIndex = currentShipValue;
                CloseExplorationOverlayPanels();
                changed = true;
            }
        }

        return changed;
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
            "DIMENSIÓN 1\n\n" +
            BuildMetalsText(gs) +
            "\n\n" +
            BuildPlanetsText(gs) +
            "\n\n" +
            BuildScannerText(gs) +
            "\n\n" +
            BuildActiveExplorationsText(gs) +
            "\n\n" +
            BuildSelectedDestinationText(gs);


        RefreshDestinationDropdown(gs);
        RefreshShipDropdown(gs);
        RefreshButtons(gs);
        UpdateExplorationResultPanelState(gs);
        RefreshExplorationRewardsPanel(gs);
        RefreshExplorationRecordPanel(gs);
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
        explorationRecordPanelOpen = false;
    }

    private string BuildMetalsText(GameState gs)
    {
        List<string> lines = new List<string>();

        AddMetalLineIfUnlocked(gs, lines, Dimension1System.MetalIron, "Hierro");
        AddMetalLineIfUnlocked(gs, lines, Dimension1System.MetalCopper, "Cobre");
        AddMetalLineIfUnlocked(gs, lines, Dimension1System.MetalAluminum, "Aluminio");
        AddMetalLineIfUnlocked(gs, lines, Dimension1System.MetalTitanium, "Titanio");
        AddMetalLineIfUnlocked(gs, lines, Dimension1System.MetalNickel, "Níquel");
        AddMetalLineIfUnlocked(gs, lines, Dimension1System.MetalCobalt, "Cobalto");
        AddMetalLineIfUnlocked(gs, lines, Dimension1System.MetalLithium, "Litio");
        AddMetalLineIfUnlocked(gs, lines, Dimension1System.MetalTungsten, "Tungsteno");
        AddMetalLineIfUnlocked(gs, lines, Dimension1System.MetalPlatinum, "Platino");
        AddMetalLineIfUnlocked(gs, lines, Dimension1System.MetalIridium, "Iridio");

        if (lines.Count == 0)
            return "Metales:\nSin metales desbloqueados.";

        return "Metales:\n" + string.Join("\n", lines);
    }

    private void AddMetalLineIfUnlocked(
        GameState gs,
        List<string> lines,
        string metalId,
        string visualName
    )
    {
        if (!Dimension1System.IsMetalUnlockedForDimension1(gs, metalId))
            return;

        lines.Add(BuildMetalLine(gs, metalId, visualName));
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

        double upgradeCost = Dimension1System.GetExtractorUpgradeCost(gs, planet);
        string costMetalId = Dimension1System.GetExtractorUpgradeMainCostMetal(planet);
        string costMetalName = GetMetalVisualName(costMetalId);

        return visualName +
            " | Tier " +
            planet.extractorTier +
            " " +
            activeMetals;
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

    private string GetRelicVisualName(string relicId)
    {
        switch (relicId)
        {
            case Dimension1System.RelicDriftCompass:
                return "Brújula de Deriva";

            case Dimension1System.RelicAncientCargoCore:
                return "Núcleo de Bodega Antigua";

            case Dimension1System.RelicLostNavigationRecord:
                return "Registro de Navegación Perdido";

            case Dimension1System.RelicExpeditionSeal:
                return "Sello de Expedición";

            case Dimension1System.RelicDormantEcho:
                return "Eco de Reliquia Dormida";

            case Dimension1System.RelicExplorerPlate:
                return "Placa de Explorador";

            case Dimension1System.RelicExtractionHook:
                return "Gancho de Extracción";

            case Dimension1System.RelicAnalyticCrystal:
                return "Cristal Analítico";

            case Dimension1System.RelicModularContainer:
                return "Contenedor Modular";

            case Dimension1System.RelicRescueBeacon:
                return "Baliza de Rescate";

            case Dimension1System.RelicAncientDrill:
                return "Taladro Antiguo";

            case Dimension1System.RelicRememberedAlloy:
                return "Aleación Recordada";

            case Dimension1System.RelicProspectingCore:
                return "Núcleo de Prospección";

            case Dimension1System.RelicExtractionSeal:
                return "Sello de Extracción";

            case Dimension1System.RelicMatrixArchive:
                return "Archivo de Matrices";

            case Dimension1System.RelicFracturedAntenna:
                return "Antena Fracturada";

            default:
                return relicId;
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

        float fracturedAntennaExtraChance =
            Dimension1System.GetFracturedAntennaExtraScanDestinationChancePreview(gs);

        if (fracturedAntennaExtraChance > 0.0f)
        {
            scannerHeader +=
                "Antena Fracturada: " +
                (fracturedAntennaExtraChance * 100f).ToString("0.#") +
                "% de +1 destino\n";
        }

        float scanMemoryReduction =
            Dimension1System.GetD1TreeScanMemoryRepetitionReduction(gs);

        if (scanMemoryReduction > 0.0f)
        {
            scannerHeader +=
                "Memoria de Escaneo: -" +
                (scanMemoryReduction * 100f).ToString("0.#") +
                "% peso de destinos repetidos\n";
        }

        float advancedCartographyChance =
            Dimension1System.GetD1TreeAdvancedCartographySpecialDestinationChance(gs);

        if (advancedCartographyChance > 0.0f)
        {
            scannerHeader +=
                "Cartografía Avanzada: " +
                (advancedCartographyChance * 100f).ToString("0.#") +
                "% de mejorar destino avanzado\n";
        }

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

    private string BuildActiveExplorationsText(GameState gs)
    {
        if (gs == null || gs.dimension1Ships == null)
            return "Exploraciones activas:\nNo disponible.";

        string text = "Exploraciones activas:\n";
        bool hasAny = false;

        foreach (D1ShipState ship in gs.dimension1Ships)
        {
            if (ship == null)
                continue;

            if (!ship.unlocked)
                continue;

            if (!ship.explorationActive)
                continue;

            if (hasAny)
                text += "\n";

            string destinationName = !string.IsNullOrEmpty(ship.activeDestinationId)
                ? GetDestinationVisualName(ship.activeDestinationId)
                : "Destino desconocido";

            text +=
                GetShipVisualName(ship.shipId) +
                " → " +
                destinationName +
                " (" +
                FormatSeconds(ship.explorationRemainingSeconds) +
                ")";

            hasAny = true;
        }

        if (!hasAny)
            return "Exploraciones activas:\nNinguna nave explorando.";

        return text;
    }

    private string BuildExplorationRecordText(GameState gs)
    {
        if (gs == null)
            return "Registro de exploración:\nNo disponible.";

        if (gs.dimension1RecentExplorationRecords == null || gs.dimension1RecentExplorationRecords.Count == 0)
            return "Registro de exploración:\nSin exploraciones completadas.";

        string text = "Registro de exploración:";

        int startIndex = Mathf.Max(0, gs.dimension1RecentExplorationRecords.Count - 20);

        for (int i = gs.dimension1RecentExplorationRecords.Count - 1; i >= startIndex; i--)
        {
            D1ExplorationRecordEntry entry = gs.dimension1RecentExplorationRecords[i];

            if (entry == null)
                continue;

            text +=
                "\n\n#" +
                entry.resultId +
                " — " +
                GetShipVisualName(entry.shipId) +
                " → " +
                GetDestinationVisualName(entry.destinationId) +
                "\n" +
                BuildExplorationRecordRewardSummaryText(entry);
        }

        return text;
    }

    private string BuildExplorationRecordRewardSummaryText(D1ExplorationRecordEntry entry)
    {
        if (entry == null)
            return "- Sin datos.";

        string text = "";

        text += BuildExplorationRecordMetalSummaryText(entry);
        text += "\n";
        text += BuildExplorationRecordMatrixSummaryText(entry);
        text += "\n";
        text += BuildExplorationRecordRelicSummaryText(entry);

        return text;
    }

    private string BuildExplorationRecordMetalSummaryText(D1ExplorationRecordEntry entry)
    {
        if (entry == null || entry.rewards == null || entry.rewards.Count == 0)
            return "- Metales: ninguno";

        string text = "- Metales: ";
        bool hasAnyMetal = false;

        foreach (D1MetalAmount reward in entry.rewards)
        {
            if (reward == null)
                continue;

            if (string.IsNullOrEmpty(reward.metalId))
                continue;

            if (reward.amount <= 0.0)
                continue;

            if (hasAnyMetal)
                text += " / ";

            text +=
                "+" +
                reward.amount.ToString("0.0") +
                " " +
                GetMetalVisualName(reward.metalId);

            hasAnyMetal = true;
        }

        if (!hasAnyMetal)
            return "- Metales: ninguno";

        return text;
    }

    private string BuildExplorationRecordMatrixSummaryText(D1ExplorationRecordEntry entry)
    {
        if (entry == null)
            return "- Matrices: ninguna";

        string text = "- Matrices: ";
        bool hasAnyMatrixReward = false;

        if (entry.blueprintFragments > 0)
        {
            text +=
                "+" +
                entry.blueprintFragments +
                " fragmentos de Matriz Adaptativa";

            hasAnyMatrixReward = true;
        }

        if (entry.specificBlueprintRewards != null)
        {
            foreach (D1BlueprintAmount matrixReward in entry.specificBlueprintRewards)
            {
                if (matrixReward == null)
                    continue;

                if (string.IsNullOrEmpty(matrixReward.blueprintId))
                    continue;

                if (matrixReward.amount <= 0)
                    continue;

                if (hasAnyMatrixReward)
                    text += " / ";

                text +=
                    "+" +
                    matrixReward.amount +
                    " " +
                    GetBlueprintVisualName(matrixReward.blueprintId);

                hasAnyMatrixReward = true;
            }
        }

        if (!hasAnyMatrixReward)
            return "- Matrices: ninguna";

        return text;
    }

    private string BuildExplorationRecordRelicSummaryText(D1ExplorationRecordEntry entry)
    {
        if (entry == null || entry.relicRewards == null || entry.relicRewards.Count == 0)
            return "- Reliquias: ninguna";

        string text = "- Reliquias: ";
        bool hasAnyRelic = false;

        foreach (D1RelicRewardEntry reward in entry.relicRewards)
        {
            if (reward == null)
                continue;

            if (string.IsNullOrEmpty(reward.relicId))
                continue;

            if (hasAnyRelic)
                text += " / ";

            if (reward.wasDuplicate)
            {
                text +=
                    GetRelicVisualName(reward.relicId) +
                    " repetida +" +
                    reward.duplicateMetalAmount.ToString("0") +
                    " " +
                    GetMetalVisualName(reward.duplicateMetalId);
            }
            else
            {
                text += GetRelicVisualName(reward.relicId) + " desbloqueada";
            }

            hasAnyRelic = true;
        }

        if (!hasAnyRelic)
            return "- Reliquias: ninguna";

        return text;
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

        bool shouldShow = !hangarPanelOpen && (shouldShowResult || shouldShowPreview);

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

    private void RefreshExplorationRecordPanel(GameState gs)
    {
        bool isMainDimension1View = !hangarPanelOpen;
        bool shouldShowRecordPanel = isMainDimension1View && explorationRecordPanelOpen;

        if (openExplorationRecordButton != null)
        {
            openExplorationRecordButton.gameObject.SetActive(isMainDimension1View && !explorationRecordPanelOpen);
            SetButtonText(openExplorationRecordButton, "abrir registro");
        }

        if (closeExplorationRecordButton != null)
        {
            closeExplorationRecordButton.gameObject.SetActive(shouldShowRecordPanel);
            SetButtonText(closeExplorationRecordButton, "cerrar registro");
        }

        if (explorationRecordPanel != null)
            explorationRecordPanel.SetActive(shouldShowRecordPanel);

        if (explorationRecordText != null)
            explorationRecordText.gameObject.SetActive(shouldShowRecordPanel);

        if (explorationRecordText == null)
            return;

        if (!shouldShowRecordPanel)
        {
            explorationRecordText.text = "";
            return;
        }

        explorationRecordText.text = BuildExplorationRecordText(gs);
        ResizeExplorationRecordScrollContent();
    }

    private void ResizeExplorationRecordScrollContent()
    {
        if (explorationRecordText == null)
            return;

        RectTransform textRect = explorationRecordText.rectTransform;

        if (textRect == null)
            return;

        RectTransform contentRect = textRect.parent as RectTransform;

        if (contentRect == null)
            return;

        float paddingX = 12f;
        float paddingTop = 10f;
        float paddingBottom = 16f;

        float contentWidth = contentRect.rect.width;

        if (contentWidth <= 0f)
            contentWidth = 420f;

        float textWidth = Mathf.Max(100f, contentWidth - paddingX * 2f);

        textRect.anchorMin = new Vector2(0f, 1f);
        textRect.anchorMax = new Vector2(0f, 1f);
        textRect.pivot = new Vector2(0f, 1f);
        textRect.anchoredPosition = new Vector2(paddingX, -paddingTop);
        textRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textWidth);

        explorationRecordText.ForceMeshUpdate();

        float preferredHeight = Mathf.Ceil(explorationRecordText.preferredHeight);
        float finalTextHeight = Mathf.Max(50f, preferredHeight + 4f);
        float finalContentHeight = finalTextHeight + paddingTop + paddingBottom;

        textRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, finalTextHeight);
        contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, finalContentHeight);
    }

    private string BuildExplorationRewardsText(GameState gs)
    {
        D1ScannedDestinationState destination = GetSelectedAvailableDestination(gs);
        D1ShipState selectedShip = GetSelectedAvailableShip(gs);

        if (destination == null || selectedShip == null)
            return "";

        string destinationId = destination.destinationId;

        string metalRewards =
            GetDestinationRewardPreview(destinationId)
                .Replace(" / ", " / ");

        float fragmentChance =
            Dimension1System.GetSimpleBlueprintFragmentChance(
                gs,
                destinationId,
                selectedShip
            );

        float specificMatrixChance =
            Dimension1System.GetSpecificBlueprintChancePreview(
                gs,
                destinationId,
                selectedShip
            );

        float relicChance =
            Dimension1System.GetExplorationRelicChancePreview(
                gs,
                destinationId,
                selectedShip
            );

        string text =
            "Recompensas posibles\n\n" +
            "Nave: " +
            GetShipVisualName(selectedShip.shipId) +
            "\n" +
            "Tiempo: " +
            GetDestinationDurationPreview(gs, destinationId, selectedShip) +
            "\n\n" +
            "Metales:\n" +
            metalRewards;

        Dimension1System.GetD1TreePartialRecoveryValues(
            gs,
            out float partialRecoveryChance,
            out float partialRecoveryAmount
            );

        if (partialRecoveryChance > 0.0f && partialRecoveryAmount > 0.0f)
        {
            text +=
                "\nRecuperación parcial: " +
                (partialRecoveryChance * 100f).ToString("0.#") +
                "% de +" +
                (partialRecoveryAmount * 100f).ToString("0.#") +
                "% metal";
        }

        if (Dimension1System.HasD1TreeDestinationReading(gs))
        {
            text +=
                "\n\nLectura:\n" +
                GetD1DestinationFocusText(destinationId) +
                " | Riesgo: " +
                GetD1DestinationRiskText(destinationId) +
                "\nHallazgo: " +
                GetD1DestinationExpectedFindText(destinationId) +
                "\nNave sugerida: " +
                GetD1DestinationRecommendedShipText(destinationId);
        }

        text +=
            "\n\nMatrices:\n" +
            "Adaptativa: " +
            (fragmentChance * 100f).ToString("0") +
            "% | Específica: " +
            (specificMatrixChance * 100f).ToString("0.#") +
            "%";

        text +=
            "\nPool matrices:\n" +
            BuildInlineSpecificMatrixPreviewPoolText(
                Dimension1System.GetSpecificBlueprintPoolPreview(gs, destinationId)
            );

        float blueprintPriorityBonus =
            Dimension1System.GetD1TreeBlueprintPriorityBonus(gs);

        float hiddenFindBonus =
            Dimension1System.GetD1TreeHiddenFindQualityBonus(gs);

        if (blueprintPriorityBonus > 0.0f || hiddenFindBonus > 0.0f)
        {
            text += "\n\nBonus árbol:";

            if (blueprintPriorityBonus > 0.0f)
            {
                text +=
                    "\nPrioridad matriz faltante: +" +
                    (blueprintPriorityBonus * 100f).ToString("0.#") +
                    "%";
            }

            if (hiddenFindBonus > 0.0f)
            {
                text +=
                    "\nRastreo oculto: +" +
                    (hiddenFindBonus * 100f).ToString("0.#") +
                    "%";
            }
        }

        text +=
            "\n\nReliquias:\n" +
            "Chance: " +
            (relicChance * 100f).ToString("0.#") +
            "%";

        text +=
            "\nPool reliquias:\n" +
            BuildInlineRelicPreviewPoolText(
                Dimension1System.GetExplorationRelicRewardPoolPreview(gs, destinationId)
            );

        return text;
    }

    private string BuildD1DestinationReadingText(string destinationId)
    {
        return
            "- Enfoque: " +
            GetD1DestinationFocusText(destinationId) +
            "\n- Riesgo: " +
            GetD1DestinationRiskText(destinationId) +
            "\n- Hallazgo esperado: " +
            GetD1DestinationExpectedFindText(destinationId) +
            "\n- Nave recomendada: " +
            GetD1DestinationRecommendedShipText(destinationId);
    }

    private string GetD1DestinationFocusText(string destinationId)
    {
        switch (destinationId)
        {
            case Dimension1System.DestinationMineralBelt:
                return "minerales y depósitos";

            case Dimension1System.DestinationShipGraveyard:
                return "restos de naves y matrices de carga";

            case Dimension1System.DestinationAbandonedProbe:
            case Dimension1System.DestinationDriftingProbes:
                return "sondas, sensores y lectura técnica";

            case Dimension1System.DestinationAbandonedShip:
                return "nave antigua, matrices y hallazgos recuperables";

            case Dimension1System.DestinationOrbitalRuin:
                return "ruinas, reliquias y señales antiguas";

            case Dimension1System.DestinationLaboratory:
                return "tecnología, matrices y reliquias técnicas";

            case Dimension1System.DestinationAbandonedStation:
                return "módulos de estación y recuperación";

            case Dimension1System.DestinationMinorAnomaly:
                return "anomalía menor y reliquias";

            case Dimension1System.DestinationAncientStructure:
                return "estructura avanzada y hallazgos raros";

            case Dimension1System.DestinationUnstableZone:
                return "zona avanzada de alto riesgo";

            default:
                return "desconocido";
        }
    }

    private string GetD1DestinationRiskText(string destinationId)
    {
        switch (destinationId)
        {
            case Dimension1System.DestinationMineralBelt:
            case Dimension1System.DestinationShipGraveyard:
            case Dimension1System.DestinationAbandonedProbe:
            case Dimension1System.DestinationDriftingProbes:
                return "bajo";

            case Dimension1System.DestinationAbandonedShip:
            case Dimension1System.DestinationOrbitalRuin:
            case Dimension1System.DestinationLaboratory:
            case Dimension1System.DestinationAbandonedStation:
            case Dimension1System.DestinationMinorAnomaly:
                return "medio";

            case Dimension1System.DestinationAncientStructure:
            case Dimension1System.DestinationUnstableZone:
                return "alto";

            default:
                return "desconocido";
        }
    }

    private string GetD1DestinationExpectedFindText(string destinationId)
    {
        switch (destinationId)
        {
            case Dimension1System.DestinationMineralBelt:
                return "metales";

            case Dimension1System.DestinationShipGraveyard:
                return "metales y matrices de Carga";

            case Dimension1System.DestinationAbandonedShip:
                return "matrices de nave y reliquias";

            case Dimension1System.DestinationOrbitalRuin:
                return "reliquias y matrices técnicas";

            case Dimension1System.DestinationDriftingProbes:
            case Dimension1System.DestinationAbandonedProbe:
                return "sensores, fragmentos y reliquias técnicas";

            case Dimension1System.DestinationLaboratory:
                return "matrices, reliquias y hallazgos técnicos";

            case Dimension1System.DestinationAbandonedStation:
                return "matrices de Rescate y estación";

            case Dimension1System.DestinationMinorAnomaly:
                return "reliquias y hallazgos ocultos";

            case Dimension1System.DestinationAncientStructure:
                return "reliquias raras y matrices avanzadas";

            case Dimension1System.DestinationUnstableZone:
                return "recompensas avanzadas y Convergencia";

            default:
                return "sin lectura";
        }
    }

    private string GetD1DestinationRecommendedShipText(string destinationId)
    {
        switch (destinationId)
        {
            case Dimension1System.DestinationMineralBelt:
                return "Dron Extractor / Nave de Carga";

            case Dimension1System.DestinationShipGraveyard:
                return "Dron Extractor / Nave de Carga";

            case Dimension1System.DestinationAbandonedProbe:
            case Dimension1System.DestinationDriftingProbes:
                return "Sonda Analítica";

            case Dimension1System.DestinationAbandonedShip:
                return "Sonda Analítica / Nave de Rescate";

            case Dimension1System.DestinationOrbitalRuin:
                return "Sonda Analítica";

            case Dimension1System.DestinationLaboratory:
                return "Sonda Analítica / Nave de Rescate";

            case Dimension1System.DestinationAbandonedStation:
                return "Nave de Carga / Nave de Rescate";

            case Dimension1System.DestinationMinorAnomaly:
                return "Sonda Analítica";

            case Dimension1System.DestinationAncientStructure:
                return "Sonda Analítica / Nave de Convergencia";

            case Dimension1System.DestinationUnstableZone:
                return "Nave de Rescate / Nave de Convergencia";

            default:
                return "Sonda Ligera";
        }
    }

    private string BuildSpecificMatrixPreviewPoolText(string[] matrixIds)
    {
        if (matrixIds == null || matrixIds.Length == 0)
            return "- Ninguna";

        string text = "";

        for (int i = 0; i < matrixIds.Length; i++)
        {
            if (string.IsNullOrEmpty(matrixIds[i]))
                continue;

            if (!string.IsNullOrEmpty(text))
                text += "\n";

            text += "- " + GetBlueprintVisualName(matrixIds[i]);
        }

        if (string.IsNullOrEmpty(text))
            return "- Ninguna";

        return text;
    }

    private string BuildInlineSpecificMatrixPreviewPoolText(string[] matrixIds)
    {
        if (matrixIds == null || matrixIds.Length == 0)
            return "Ninguna";

        string text = "";

        for (int i = 0; i < matrixIds.Length; i++)
        {
            if (string.IsNullOrEmpty(matrixIds[i]))
                continue;

            if (!string.IsNullOrEmpty(text))
                text += " / ";

            text += GetShortBlueprintVisualName(matrixIds[i]);
        }

        if (string.IsNullOrEmpty(text))
            return "Ninguna";

        return text;
    }

    private string BuildInlineRelicPreviewPoolText(string[] relicIds)
    {
        if (relicIds == null || relicIds.Length == 0)
            return "Ninguna";

        string text = "";

        for (int i = 0; i < relicIds.Length; i++)
        {
            if (string.IsNullOrEmpty(relicIds[i]))
                continue;

            if (!string.IsNullOrEmpty(text))
                text += " / ";

            text += GetRelicVisualName(relicIds[i]);
        }

        if (string.IsNullOrEmpty(text))
            return "Ninguna";

        return text;
    }

    private string GetShortBlueprintVisualName(string blueprintId)
    {
        switch (blueprintId)
        {
            case Dimension1System.BlueprintCargoFrame:
                return "Chasis Carga";

            case Dimension1System.BlueprintCargoHold:
                return "Bodega Carga";

            case Dimension1System.BlueprintCargoStabilizer:
                return "Estabilizador Carga";

            case Dimension1System.BlueprintRescueFrame:
                return "Chasis Rescate";

            case Dimension1System.BlueprintRescueBeacon:
                return "Baliza Rescate";

            case Dimension1System.BlueprintRescueRecoveryBay:
                return "Bahía Recuperación";

            case Dimension1System.BlueprintRescueProtectionMatrix:
                return "Protección Rescate";

            case Dimension1System.BlueprintConvergenceChassis:
                return "Chasis Convergencia";

            case Dimension1System.BlueprintConvergenceCore:
                return "Núcleo Convergencia";

            case Dimension1System.BlueprintConvergenceMatrix:
                return "Matriz Convergencia";

            case Dimension1System.BlueprintAnomalousArmor:
                return "Blindaje Anómalo";

            default:
                return GetBlueprintVisualName(blueprintId);
        }
    }

    private string BuildRelicPreviewPoolText(string[] relicIds)
    {
        if (relicIds == null || relicIds.Length == 0)
            return "- Ninguna";

        string text = "";

        for (int i = 0; i < relicIds.Length; i++)
        {
            if (string.IsNullOrEmpty(relicIds[i]))
                continue;

            if (!string.IsNullOrEmpty(text))
                text += "\n";

            text += "- " + GetRelicVisualName(relicIds[i]);
        }

        if (string.IsNullOrEmpty(text))
            return "- Ninguna";

        return text;
    }

    private string BuildExplorationResultText(GameState gs)
    {
        if (gs == null)
            return "";

        string text =
            "Exploración completada\n\n" +
            "Destino:\n" +
            GetDestinationVisualName(gs.dimension1LastExplorationDestinationId) +
            "\n\n" +
            BuildExplorationMetalResultText(gs) +
            "\n\n" +
            BuildExplorationMatrixResultText(gs) +
            "\n\n" +
            BuildExplorationRelicResultText(gs);

        return text;
    }

    private string BuildSpecificMatrixRewardsText(List<D1BlueprintAmount> rewards)
    {
        if (rewards == null || rewards.Count == 0)
            return "- Matrices específicas: ninguna";

        string text = "- Matrices específicas:\n";
        bool hasAny = false;

        foreach (D1BlueprintAmount reward in rewards)
        {
            if (reward == null)
                continue;

            if (string.IsNullOrEmpty(reward.blueprintId))
                continue;

            if (reward.amount <= 0)
                continue;

            text +=
                "  • " +
                GetBlueprintVisualName(reward.blueprintId) +
                " +" +
                reward.amount +
                "\n";

            hasAny = true;
        }

        if (!hasAny)
            return "- Matrices específicas: ninguna";

        return text.TrimEnd('\n');
    }

    private string BuildExplorationMatrixResultText(GameState gs)
    {
        if (gs == null)
            return "Matrices:\nNo disponible.";

        int gainedFragments = gs.dimension1LastExplorationBlueprintFragments;
        int currentProgress = Dimension1System.GetBlueprintFragmentProgress(gs);
        int completedBlueprints = Dimension1System.GetCompletedBlueprintCount(gs);

        string text =
            "Matrices obtenidas:\n";

        if (gainedFragments > 0)
        {
            text +=
                "- Fragmentos de Matriz Adaptativa: +" +
                gainedFragments +
                "\n";
        }
        else
        {
            text += "- Fragmentos de Matriz Adaptativa: +0\n";
        }

        text +=
            "- Progreso Matriz Adaptativa: " +
            currentProgress +
            "/" +
            Dimension1System.BlueprintFragmentsPerBlueprint +
            "\n" +
            "- Matrices Adaptativas disponibles: " +
            completedBlueprints +
            "\n";

        text += BuildSpecificMatrixResultText(gs.dimension1LastExplorationSpecificBlueprints);

        return text;
    }

    private string BuildExplorationRelicResultText(GameState gs)
    {
        if (gs == null)
            return "Reliquias obtenidas:\n- No disponible";

        if (gs.dimension1LastExplorationRelics == null ||
            gs.dimension1LastExplorationRelics.Count == 0)
        {
            return "Reliquias obtenidas:\n- Ninguna";
        }

        string text = "Reliquias obtenidas:\n";

        foreach (D1RelicRewardEntry reward in gs.dimension1LastExplorationRelics)
        {
            if (reward == null)
                continue;

            if (string.IsNullOrEmpty(reward.relicId))
                continue;

            if (reward.wasDuplicate)
            {
                text +=
                    "- " +
                    GetRelicVisualName(reward.relicId) +
                    " repetida → +" +
                    reward.duplicateMetalAmount.ToString("0") +
                    " " +
                    GetMetalVisualName(reward.duplicateMetalId) +
                    "\n";
            }
            else
            {
                text +=
                    "- " +
                    GetRelicVisualName(reward.relicId) +
                    " desbloqueada\n";
            }
        }

        return text.TrimEnd('\n');
    }

    private string BuildExplorationMetalResultText(GameState gs)
    {
        if (gs == null)
            return "Metales obtenidos:\n- No disponible";

        string text = "Metales obtenidos:\n";
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
                    "- " +
                    GetMetalVisualName(reward.metalId) +
                    ": +" +
                    reward.amount.ToString("0.0") +
                    "\n";

                hasAnyMetal = true;
            }
        }

        if (!hasAnyMetal)
            text += "- Ninguno\n";

        return text.TrimEnd('\n');
    }

    private string BuildSpecificMatrixResultText(List<D1BlueprintAmount> matrixRewards)
    {
        if (matrixRewards == null || matrixRewards.Count == 0)
            return "- Matriz específica: ninguna";

        string text = "";
        bool hasAnyMatrix = false;

        foreach (D1BlueprintAmount reward in matrixRewards)
        {
            if (reward == null)
                continue;

            if (string.IsNullOrEmpty(reward.blueprintId))
                continue;

            if (reward.amount <= 0)
                continue;

            if (!hasAnyMatrix)
                text += "- Matrices específicas:\n";

            text +=
                "  • " +
                GetBlueprintVisualName(reward.blueprintId) +
                ": +" +
                reward.amount +
                "\n";

            hasAnyMatrix = true;
        }

        if (!hasAnyMatrix)
            return "- Matriz específica: ninguna";

        return text.TrimEnd('\n');
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
                "\nFragmentos de Matriz Adaptativa: +" +
                gs.dimension1LastExplorationBlueprintFragments;
        }
        else
        {
            text += "\nFragmentos de Matriz Adaptativa: +0";
        }

        return text;
    }

    private void RefreshDestinationDropdown(GameState gs)
    {
        if (destinationDropdown == null)
            return;

        if (hangarPanelOpen)
        {
            destinationDropdown.gameObject.SetActive(false);
            return;
        }

        bool scanActive = gs != null && gs.dimension1ScanActive;
        int destinationCount = GetAvailableDestinationCount(gs);
        bool hasDestinations = destinationCount > 0;

        destinationDropdown.gameObject.SetActive(true);

        if (scanActive)
        {
            isRefreshingDestinationDropdown = true;

            destinationDropdown.ClearOptions();
            destinationDropdown.AddOptions(new List<string> { "Escaneando..." });
            destinationDropdown.SetValueWithoutNotify(0);
            destinationDropdown.RefreshShownValue();

            selectedDestinationIndex = 0;
            destinationDropdownSignature = "Escaneando...";

            destinationDropdown.interactable = false;

            isRefreshingDestinationDropdown = false;
            return;
        }

        if (!hasDestinations)
        {
            isRefreshingDestinationDropdown = true;

            destinationDropdown.ClearOptions();
            destinationDropdown.AddOptions(new List<string> { "Sin destinos disponibles" });
            destinationDropdown.SetValueWithoutNotify(0);
            destinationDropdown.RefreshShownValue();

            selectedDestinationIndex = 0;
            destinationDropdownSignature = "Sin destinos disponibles";

            destinationDropdown.interactable = false;

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

        destinationDropdown.interactable = true;

        isRefreshingDestinationDropdown = false;
    }

    private void RefreshShipDropdown(GameState gs)
    {
        if (shipDropdown == null)
            return;

        if (hangarPanelOpen)
        {
            shipDropdown.gameObject.SetActive(false);
            return;
        }

        bool scanActive = gs != null && gs.dimension1ScanActive;
        bool hasDestinations = GetAvailableDestinationCount(gs) > 0;
        int shipCount = GetAvailableShipCount(gs);
        bool hasShips = shipCount > 0;

        shipDropdown.gameObject.SetActive(true);

        if (scanActive)
        {
            isRefreshingShipDropdown = true;

            shipDropdown.ClearOptions();
            shipDropdown.AddOptions(new List<string> { "Escaneando..." });
            shipDropdown.SetValueWithoutNotify(0);
            shipDropdown.RefreshShownValue();

            selectedShipIndex = 0;
            shipDropdownSignature = "Escaneando...";

            shipDropdown.interactable = false;

            isRefreshingShipDropdown = false;
            return;
        }

        if (!hasDestinations)
        {
            isRefreshingShipDropdown = true;

            shipDropdown.ClearOptions();
            shipDropdown.AddOptions(new List<string> { "Sin destino disponible" });
            shipDropdown.SetValueWithoutNotify(0);
            shipDropdown.RefreshShownValue();

            selectedShipIndex = 0;
            shipDropdownSignature = "Sin destino disponible";

            shipDropdown.interactable = false;

            isRefreshingShipDropdown = false;
            return;
        }

        if (!hasShips)
        {
            isRefreshingShipDropdown = true;

            shipDropdown.ClearOptions();
            shipDropdown.AddOptions(new List<string> { "Sin naves disponibles" });
            shipDropdown.SetValueWithoutNotify(0);
            shipDropdown.RefreshShownValue();

            selectedShipIndex = 0;
            shipDropdownSignature = "Sin naves disponibles";

            shipDropdown.interactable = false;

            isRefreshingShipDropdown = false;
            return;
        }

        List<string> options = new List<string>();
        options.Add("Elegir nave");

        foreach (D1ShipState ship in gs.dimension1Ships)
        {
            if (ship == null || !ship.unlocked || ship.explorationActive)
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

        shipDropdown.interactable = true;

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
            if (ship != null && ship.unlocked && !ship.explorationActive)
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
            if (ship == null || !ship.unlocked || ship.explorationActive)
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
                    return "Sensores VI: +10% fragmentos de Matriz Adaptativa y barrido 2.1s";

                if (ship.sensorsLevel >= 5)
                    return "Sensores V: +9% fragmentos de Matriz Adaptativa y barrido 2.4s";

                if (ship.sensorsLevel >= 4)
                    return "Sensores IV: +7% fragmentos de Matriz Adaptativa y barrido 2.7s";

                if (ship.sensorsLevel >= 3)
                    return "Sensores III: +6% fragmentos de Matriz Adaptativa y barrido 3.0s";

                if (ship.sensorsLevel >= 2)
                    return "Sensores II: +4% fragmentos de Matriz Adaptativa y barrido 3.5s";

                if (ship.sensorsLevel >= 1)
                    return "Sensores I: +3% fragmentos de Matriz Adaptativa y barrido 4.0s";

                return "Sensores base: +2% fragmentos de Matriz Adaptativa";

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

        string[] metalIds =
            Dimension1System.GetExplorationMetalRewardPoolPreview(gs, destinationId);

        if (metalIds == null || metalIds.Length == 0)
            return "Sin metales produciendo para este destino";

        List<string> rewards = new List<string>();

        for (int i = 0; i < metalIds.Length; i++)
        {
            if (string.IsNullOrEmpty(metalIds[i]))
                continue;

            rewards.Add(GetMetalVisualName(metalIds[i]));
        }

        if (rewards.Count == 0)
            return "Sin metales produciendo para este destino";

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

    private string GetDestinationDurationPreview(
        GameState gs,
        string destinationId,
        D1ShipState selectedShip
    )
    {
        double duration = Dimension1System.GetSimpleExplorationDurationPreviewSeconds(
            gs,
            destinationId,
            selectedShip
        );

        return duration.ToString("0.0") + "s";
    }

    private string BuildBlueprintArchiveText(GameState gs)
    {
        if (gs == null)
            return "Archivo de Matrices:\nNo disponible.";

        int completedBlueprints = Dimension1System.GetCompletedBlueprintCount(gs);
        int currentProgress = Dimension1System.GetBlueprintFragmentProgress(gs);

        string text =
            "Archivo de Matrices:\n" +
            "Fragmentos de Matriz Adaptativa: " +
            currentProgress +
            "/" +
            Dimension1System.BlueprintFragmentsPerBlueprint +
            "\nMatrices Adaptativas de Nave disponibles: " +
            completedBlueprints +
            "\n\nNaves por Matrices:\n" +
                    BuildBlueprintShipProgressLine(
                gs,
                Dimension1System.ShipCargoShip,
                "Nave de Carga",
                completedBlueprints
            ) +
            "\n" +
            BuildBlueprintShipProgressLine(
                gs,
                Dimension1System.ShipRescueShip,
                "Nave de Rescate",
                completedBlueprints
            ) +
            "\n" +
            BuildBlueprintShipProgressLine(
                gs,
                Dimension1System.ShipConvergenceShip,
                "Nave de Convergencia",
                completedBlueprints
            ) +
            "\n\nMatrices específicas:\n" +
            BuildSpecificBlueprintArchiveText(gs);

        return text;
    }

    private string BuildSpecificBlueprintArchiveText(GameState gs)
    {
        if (gs == null)
            return "No disponible.";

        List<string> lines = new List<string>();

        foreach (string blueprintId in Dimension1System.Dimension1BlueprintIds)
        {
            int amount = gs.GetD1BlueprintAmount(blueprintId);

            lines.Add(
                "- " +
                GetBlueprintVisualName(blueprintId) +
                ": " +
                amount
            );
        }

        if (lines.Count == 0)
            return "Sin matrices específicas.";

        return string.Join("\n", lines.ToArray());
    }

    private string GetBlueprintVisualName(string blueprintId)
    {
        if (blueprintId == Dimension1System.BlueprintCargoFrame)
            return "Matriz de Chasis de Carga";

        if (blueprintId == Dimension1System.BlueprintCargoHold)
            return "Matriz de Bodega de Carga";

        if (blueprintId == Dimension1System.BlueprintCargoStabilizer)
            return "Matriz de Estabilizador de Carga";

        if (blueprintId == Dimension1System.BlueprintRescueFrame)
            return "Matriz de Chasis de Rescate";

        if (blueprintId == Dimension1System.BlueprintRescueBeacon)
            return "Matriz de Baliza de Rescate";

        if (blueprintId == Dimension1System.BlueprintRescueRecoveryBay)
            return "Matriz de Bahía de Recuperación";

        if (blueprintId == Dimension1System.BlueprintRescueProtectionMatrix)
            return "Matriz de Protección de Rescate";

        if (blueprintId == Dimension1System.BlueprintConvergenceChassis)
            return "Matriz de Chasis de Convergencia";

        if (blueprintId == Dimension1System.BlueprintConvergenceCore)
            return "Matriz de Núcleo de Convergencia";

        if (blueprintId == Dimension1System.BlueprintConvergenceMatrix)
            return "Matriz de Lectura de Convergencia";

        if (blueprintId == Dimension1System.BlueprintAnomalousArmor)
            return "Matriz de Blindaje Anómalo";

        return blueprintId;
    }

    private string BuildBlueprintShipProgressLine(
        GameState gs,
        string shipId,
        string shipName,
        int availableBlueprints
    )
    {
        D1ShipState ship = FindShip(gs, shipId);

        if (ship != null && ship.unlocked)
            return shipName + ": construida";

        if (Dimension1System.UsesSpecificShipMatricesForUnlock(shipId))
            return BuildSpecificShipMatrixProgressLine(gs, shipId, shipName);

        if (!Dimension1System.TryGetShipUnlockCost(
            shipId,
            out string metal1,
            out double amount1,
            out string metal2,
            out double amount2,
            out string metal3,
            out double amount3,
            out string metal4,
            out double amount4,
            out int requiredBlueprints
        ))
        {
            return shipName + ": requisitos no definidos";
        }

        if (requiredBlueprints <= 0)
            return shipName + ": no requiere Matrices";

        int displayedBlueprints = availableBlueprints > requiredBlueprints
            ? requiredBlueprints
            : availableBlueprints;

        string status = displayedBlueprints >= requiredBlueprints
            ? "Matrices Adaptativas listas"
            : "faltan Matrices Adaptativas";

        return
            shipName +
            ": " +
            displayedBlueprints +
            "/" +
            requiredBlueprints +
            " Matrices Adaptativas de Nave | " +
            status;
    }

    private string BuildSpecificShipMatrixProgressLine(
    GameState gs,
    string shipId,
    string shipName
)
    {
        if (!Dimension1System.TryGetRequiredShipBlueprintIds(shipId, out string[] matrixIds))
            return shipName + ": Matrices no definidas";

        int required = matrixIds.Length;
        int specificOwned = Dimension1System.GetOwnedRequiredSpecificShipMatrixCount(gs, shipId);
        int missing = Dimension1System.GetMissingRequiredSpecificShipMatrixCount(gs, shipId);
        int adaptiveAvailable = Dimension1System.GetCompletedBlueprintCount(gs);
        int adaptiveCovering = adaptiveAvailable > missing ? missing : adaptiveAvailable;
        int totalCovered = specificOwned + adaptiveCovering;

        string status = totalCovered >= required
            ? "Matrices listas"
            : "faltan Matrices";

        string adaptiveText = missing > 0
            ? " | Adaptativas cubriendo faltantes " + adaptiveCovering + "/" + missing
            : "";

        return
            shipName +
            ": " +
            totalCovered +
            "/" +
            required +
            " Matrices requeridas | específicas " +
            specificOwned +
            "/" +
            required +
            adaptiveText +
            " | " +
            status;
    }

    private string BuildHangarText(GameState gs)
    {
        string selectedShipId = GetSelectedHangarShipId();

        return
            "Resumen de flota:\n" +
            BuildCompactHangarShipLine(gs, Dimension1System.ShipLightProbe, "Sonda Ligera") +
            "\n" +
            BuildCompactHangarShipLine(gs, Dimension1System.ShipExtractorDrone, "Dron Extractor") +
            "\n" +
            BuildCompactHangarShipLine(gs, Dimension1System.ShipAnalyticProbe, "Sonda Analítica") +
            "\n" +
            BuildCompactHangarShipLine(gs, Dimension1System.ShipCargoShip, "Nave de Carga") +
            "\n" +
            BuildCompactHangarShipLine(gs, Dimension1System.ShipRescueShip, "Nave de Rescate") +
            "\n" +
            BuildCompactHangarShipLine(gs, Dimension1System.ShipConvergenceShip, "Nave de Convergencia") +
            "\n\n" +
            BuildSelectedShipMatricesText(gs, selectedShipId);
    }

    private string BuildCompactHangarShipLine(GameState gs, string shipId, string visualName)
    {
        D1ShipState ship = FindShip(gs, shipId);

        if (ship != null && ship.unlocked)
        {
            if (ship.explorationActive)
                return "- " + visualName + ": explorando";

            return "- " + visualName + ": construida";
        }

        if (Dimension1System.UsesSpecificShipMatricesForUnlock(shipId))
        {
            if (!Dimension1System.TryGetRequiredShipBlueprintIds(shipId, out string[] matrixIds))
                return "- " + visualName + ": bloqueada";

            int required = matrixIds.Length;
            int specificOwned = Dimension1System.GetOwnedRequiredSpecificShipMatrixCount(gs, shipId);
            int missing = Dimension1System.GetMissingRequiredSpecificShipMatrixCount(gs, shipId);
            int adaptiveAvailable = Dimension1System.GetCompletedBlueprintCount(gs);
            int adaptiveCovering = adaptiveAvailable > missing ? missing : adaptiveAvailable;
            int totalCovered = specificOwned + adaptiveCovering;

            return "- " + visualName + ": matrices " + totalCovered + "/" + required;
        }

        if (Dimension1System.TryGetShipUnlockCost(
            shipId,
            out string metal1,
            out double amount1,
            out string metal2,
            out double amount2,
            out string metal3,
            out double amount3,
            out string metal4,
            out double amount4,
            out int blueprintCost
        ))
        {
            if (blueprintCost > 0)
            {
                int adaptiveAvailable = Dimension1System.GetCompletedBlueprintCount(gs);
                int shown = adaptiveAvailable > blueprintCost ? blueprintCost : adaptiveAvailable;

                return "- " + visualName + ": adaptativas " + shown + "/" + blueprintCost;
            }
        }

        return "- " + visualName + ": bloqueada";
    }

    private string BuildSelectedShipMatricesText(GameState gs, string shipId)
    {
        if (gs == null || string.IsNullOrEmpty(shipId))
            return "Matrices:\nSin nave seleccionada.";

        if (!Dimension1System.TryGetRequiredShipBlueprintIds(shipId, out string[] matrixIds))
        {
            return
                "Matrices:\n" +
                "No requiere matrices para construcción.";
        }

        string text = "Matrices de construcción:\n";

        foreach (string matrixId in matrixIds)
        {
            text +=
                "- " +
                GetShortMatrixVisualName(matrixId) +
                ": " +
                gs.GetD1BlueprintAmount(matrixId) +
                "/1\n";
        }

        text +=
            "\nComodines:\n" +
            "- Matrices Adaptativas disponibles: " +
            Dimension1System.GetCompletedBlueprintCount(gs);

        return text;
    }

    private string GetShortMatrixVisualName(string matrixId)
    {
        string name = GetBlueprintVisualName(matrixId);

        name = name.Replace("Matriz de ", "");
        name = name.Replace(" de Carga", "");
        name = name.Replace(" de Rescate", "");
        name = name.Replace(" de Convergencia", "");

        return name;
    }

    private string BuildShipLine(GameState gs, string shipId, string visualName)
    {
        D1ShipState ship = FindShip(gs, shipId);

        if (ship == null || !ship.unlocked)
        {
            return BuildLockedShipLine(gs, shipId, visualName);
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

    private string BuildLockedShipLine(GameState gs, string shipId, string visualName)
    {
        if (shipId == Dimension1System.ShipLightProbe)
            return visualName + ": nave inicial";

        if (!Dimension1System.TryGetShipUnlockCost(
            shipId,
            out string metal1,
            out double amount1,
            out string metal2,
            out double amount2,
            out string metal3,
            out double amount3,
            out string metal4,
            out double amount4,
            out int blueprintCost
        ))
        {
            return visualName + ": bloqueada | requisitos no definidos";
        }

        bool usesSpecificMatrices = Dimension1System.UsesSpecificShipMatricesForUnlock(shipId);

        string requirements = BuildUnlockCostButtonText(
            gs,
            metal1,
            amount1,
            metal2,
            amount2,
            metal3,
            amount3,
            metal4,
            amount4,
            usesSpecificMatrices ? 0 : blueprintCost
        );

        if (usesSpecificMatrices)
        {
            requirements += " + " + BuildRequiredSpecificShipMatricesButtonText(gs, shipId);
        }

        return visualName + ": bloqueada | Requiere " + requirements;
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

    public void OnClickOpenExplorationRecord()
    {
        explorationRecordPanelOpen = true;
        RefreshUI();
    }

    public void OnClickCloseExplorationRecord()
    {
        explorationRecordPanelOpen = false;
        RefreshUI();
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

    private void CloseExplorationOverlayPanels()
    {
        showingExplorationResultPanel = false;
        explorationRecordPanelOpen = false;
    }

    private void ResetMainExplorationSelection()
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

    public void OnClickScanSimpleDestination()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        bool scanStarted = Dimension1System.TryScanSimpleDestination(gs);

        if (scanStarted)
        {
            CloseExplorationOverlayPanels();
            ResetMainExplorationSelection();

            if (SaveService.I != null)
                SaveService.I.Save();
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

        CloseExplorationOverlayPanels();
        ResetMainExplorationSelection();

        if (SaveService.I != null)
            SaveService.I.Save();

        RefreshUI();
    }

    public void OnClickCloseExplorationRewards()
    {
        showingExplorationResultPanel = false;
        ResetMainExplorationSelection();

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


    public void OnDestinationDropdownChanged(int index)
    {
        if (isRefreshingDestinationDropdown)
            return;

        selectedDestinationIndex = index;
        lastObservedDestinationDropdownValue = index;

        CloseExplorationOverlayPanels();

        RefreshUI();
    }

    public void OnShipDropdownChanged(int index)
    {
        if (isRefreshingShipDropdown)
            return;

        selectedShipIndex = index;
        lastObservedShipDropdownValue = index;

        CloseExplorationOverlayPanels();

        RefreshUI();
    }

    private void SetMainExplorationControlsVisible(bool visible)
    {
        if (scanButton != null)
            scanButton.gameObject.SetActive(visible);

        if (destinationDropdown != null)
            destinationDropdown.gameObject.SetActive(visible);

        if (shipDropdown != null)
            shipDropdown.gameObject.SetActive(visible);

        if (exploreButton != null)
            exploreButton.gameObject.SetActive(visible);

        if (upgradeScannerButton != null)
            upgradeScannerButton.gameObject.SetActive(visible);

        if (upgradePlanet1Button != null)
            upgradePlanet1Button.gameObject.SetActive(visible);

        if (unlockPlanet2Button != null)
            unlockPlanet2Button.gameObject.SetActive(visible);

        if (upgradePlanet2Button != null)
            upgradePlanet2Button.gameObject.SetActive(visible);

        if (unlockPlanet3Button != null)
            unlockPlanet3Button.gameObject.SetActive(visible);

        if (upgradePlanet3Button != null)
            upgradePlanet3Button.gameObject.SetActive(visible);

        if (unlockPlanet4Button != null)
            unlockPlanet4Button.gameObject.SetActive(visible);

        if (upgradePlanet4Button != null)
            upgradePlanet4Button.gameObject.SetActive(visible);

        if (unlockPlanet5Button != null)
            unlockPlanet5Button.gameObject.SetActive(visible);

        if (upgradePlanet5Button != null)
            upgradePlanet5Button.gameObject.SetActive(visible);

        if (unlockPlanet6Button != null)
            unlockPlanet6Button.gameObject.SetActive(visible);

        if (upgradePlanet6Button != null)
            upgradePlanet6Button.gameObject.SetActive(visible);

        if (unlockPlanet7Button != null)
            unlockPlanet7Button.gameObject.SetActive(visible);

        if (upgradePlanet7Button != null)
            upgradePlanet7Button.gameObject.SetActive(visible);

    }

    private void RefreshShipUnlockButtons(GameState gs)
    {
        RefreshExtractorDroneButton(gs);
        RefreshAnalyticProbeButton(gs);
        RefreshCargoShipButton(gs);
        RefreshRescueShipButton(gs);
        RefreshConvergenceShipButton(gs);
    }

    private void RefreshButtons(GameState gs)
    {
        if (gs == null)
            return;

        if (hangarPanelOpen)
        {
            SetMainExplorationControlsVisible(false);
            RefreshShipUnlockButtons(gs);
            return;
        }

        SetMainExplorationControlsVisible(true);

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
        RefreshShipUnlockButtons(gs);
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

        unlockExtractorDroneButton.gameObject.SetActive(!unlocked);
        unlockExtractorDroneButton.interactable =
            Dimension1System.CanUnlockShip(gs, Dimension1System.ShipExtractorDrone);

        SetButtonText(
            unlockExtractorDroneButton,
            BuildShipUnlockButtonText(
                gs,
                "Desbloquear Dron",
                Dimension1System.ShipExtractorDrone
            )
        );
    }

    private void RefreshAnalyticProbeButton(GameState gs)
    {
        if (unlockAnalyticProbeButton == null)
            return;

        D1ShipState analyticProbe = FindShip(gs, Dimension1System.ShipAnalyticProbe);
        bool analyticUnlocked = analyticProbe != null && analyticProbe.unlocked;

        unlockAnalyticProbeButton.gameObject.SetActive(!analyticUnlocked);
        unlockAnalyticProbeButton.interactable =
            Dimension1System.CanUnlockShip(gs, Dimension1System.ShipAnalyticProbe);

        SetButtonText(
            unlockAnalyticProbeButton,
            BuildShipUnlockButtonText(
                gs,
                "Desbloquear Sonda Analítica",
                Dimension1System.ShipAnalyticProbe
            )
        );
    }

    private void RefreshCargoShipButton(GameState gs)
    {
        if (unlockCargoShipButton == null)
            return;

        D1ShipState cargoShip = FindShip(gs, Dimension1System.ShipCargoShip);
        bool cargoShipUnlocked = cargoShip != null && cargoShip.unlocked;

        unlockCargoShipButton.gameObject.SetActive(!cargoShipUnlocked);
        unlockCargoShipButton.interactable =
            Dimension1System.CanUnlockShip(gs, Dimension1System.ShipCargoShip);

        SetButtonText(
            unlockCargoShipButton,
            BuildShipUnlockButtonText(
                gs,
                "Construir Nave de Carga",
                Dimension1System.ShipCargoShip
            )
        );
    }

    private void RefreshRescueShipButton(GameState gs)
    {
        if (unlockRescueShipButton == null)
            return;

        D1ShipState rescueShip = FindShip(gs, Dimension1System.ShipRescueShip);
        bool rescueShipUnlocked = rescueShip != null && rescueShip.unlocked;

        unlockRescueShipButton.gameObject.SetActive(!rescueShipUnlocked);
        unlockRescueShipButton.interactable =
            Dimension1System.CanUnlockShip(gs, Dimension1System.ShipRescueShip);

        SetButtonText(
            unlockRescueShipButton,
            BuildShipUnlockButtonText(
                gs,
                "Construir Nave de Rescate",
                Dimension1System.ShipRescueShip
            )
        );
    }

    private void RefreshConvergenceShipButton(GameState gs)
    {
        if (unlockConvergenceShipButton == null)
            return;

        D1ShipState convergenceShip = FindShip(gs, Dimension1System.ShipConvergenceShip);
        bool convergenceShipUnlocked = convergenceShip != null && convergenceShip.unlocked;

        unlockConvergenceShipButton.gameObject.SetActive(!convergenceShipUnlocked);
        unlockConvergenceShipButton.interactable =
            Dimension1System.CanUnlockShip(gs, Dimension1System.ShipConvergenceShip);

        SetButtonText(
            unlockConvergenceShipButton,
            BuildShipUnlockButtonText(
                gs,
                "Construir Nave de Convergencia",
                Dimension1System.ShipConvergenceShip
            )
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
                BuildShipUpgradeCostText(gs, metal1, amount1, metal2, amount2);

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
                gs,
                advancedMetal1,
                advancedAmount1,
                advancedMetal2,
                advancedAmount2,
                blueprintCost
            );

        int specificMatrixCost = Dimension1System.GetRequiredSpecificShipUpgradeMatrixCost(
            shipId,
            advancedNextLevel
        );

        if (specificMatrixCost > 0)
        {
            advancedText +=
                " + " +
                BuildRequiredSpecificShipUpgradeMatricesButtonText(
                    gs,
                    shipId,
                    specificMatrixCost
                );
        }

        if (!Dimension1System.CanUpgradeAdvancedShipPart(gs, shipId, partId))
            advancedText += "\nRecursos insuficientes";

        return advancedText;
    }

    private string BuildShipUpgradeCostText(
        GameState gs,
        string metal1,
        double amount1,
        string metal2,
        double amount2
    )
    {
        string text = BuildRequiredMetalButtonText(gs, metal1, amount1);

        if (!string.IsNullOrEmpty(metal2) && amount2 > 0.0)
        {
            text += " + " + BuildRequiredMetalButtonText(gs, metal2, amount2);
        }

        return text;
    }

    private string BuildShipAdvancedUpgradeCostText(
        GameState gs,
        string metal1,
        double amount1,
        string metal2,
        double amount2,
        int blueprintCost
    )
    {
        string text = BuildShipUpgradeCostText(gs, metal1, amount1, metal2, amount2);

        if (blueprintCost > 0)
        {
            text += " + " + BuildRequiredBlueprintButtonText(gs, blueprintCost);
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
            SetButtonText(
            unlockPlanet2Button,
            BuildPlanetUnlockButtonText(gs, "Desbloquear P2", Dimension1System.Planet02)
);
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
            SetButtonText(
            unlockPlanet3Button,
            BuildPlanetUnlockButtonText(gs, "Desbloquear P3", Dimension1System.Planet03)
);
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
            SetButtonText(
            unlockPlanet4Button,
            BuildPlanetUnlockButtonText(gs, "Desbloquear P4", Dimension1System.Planet04)
);
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
            SetButtonText(
            unlockPlanet5Button,
            BuildPlanetUnlockButtonText(gs, "Desbloquear P5", Dimension1System.Planet05)
);
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
            SetButtonText(
            unlockPlanet6Button,
            BuildPlanetUnlockButtonText(gs, "Desbloquear P6", Dimension1System.Planet06)
);
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
            SetButtonText(
            unlockPlanet7Button,
            BuildPlanetUnlockButtonText(gs, "Desbloquear P7", Dimension1System.Planet07)
);
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

        double cost = Dimension1System.GetExtractorUpgradeCost(gs, planet);
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
        explorationRecordPanelOpen = false;
        showingExplorationResultPanel = false;
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

        if (SaveService.I != null)
            SaveService.I.Save();

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
                : "Activa";
        }

        string text =
            GetShipVisualName(shipId) +
            "\n" +
            status +
            "\n\nRol:\n" +
            GetShipRoleText(shipId) +
            "\n\n" +
            BuildSelectedHangarShipStatsText(ship) +
            "\n\n" +
            BuildSelectedShipMatricesText(gs, shipId);

        if (ship == null || !ship.unlocked)
        {
            text +=
                "\n\nConstrucción:\n" +
                BuildHangarShipUnlockRequirementDetails(gs, shipId);
        }

        return text;
    }

    private string BuildSelectedHangarShipStatsText(D1ShipState ship)
    {
        return
            "Stats:\n" +
            "- Carga: " +
            FormatShipUpgradeLevel(GetShipPartLevelForUI(ship, Dimension1System.ShipPartCargo)) +
            "/VI\n" +
            "- Velocidad: " +
            FormatShipUpgradeLevel(GetShipPartLevelForUI(ship, Dimension1System.ShipPartSpeed)) +
            "/VI\n" +
            "- Blindaje: " +
            FormatShipUpgradeLevel(GetShipPartLevelForUI(ship, Dimension1System.ShipPartArmor)) +
            "/VI\n" +
            "- Sensores: " +
            FormatShipUpgradeLevel(GetShipPartLevelForUI(ship, Dimension1System.ShipPartSensors)) +
            "/VI";
    }

    private string BuildSelectedHangarShipUnlockText(GameState gs, string shipId, D1ShipState ship)
    {
        if (ship != null && ship.unlocked)
            return "";

        return
            "\n\nRequisitos para construir:\n" +
            BuildHangarShipUnlockRequirementDetails(gs, shipId);
    }

    private string BuildHangarShipUnlockRequirementDetails(GameState gs, string shipId)
    {
        if (shipId == Dimension1System.ShipLightProbe)
            return "Nave inicial.";

        if (!Dimension1System.TryGetShipUnlockCost(
            shipId,
            out string metal1,
            out double amount1,
            out string metal2,
            out double amount2,
            out string metal3,
            out double amount3,
            out string metal4,
            out double amount4,
            out int blueprintCost
        ))
        {
            return "Requisitos no definidos.";
        }

        List<string> lines = new List<string>();

        AddShipUnlockRequirementLine(gs, lines, metal1, amount1);
        AddShipUnlockRequirementLine(gs, lines, metal2, amount2);
        AddShipUnlockRequirementLine(gs, lines, metal3, amount3);
        AddShipUnlockRequirementLine(gs, lines, metal4, amount4);

        if (Dimension1System.UsesSpecificShipMatricesForUnlock(shipId))
        {
            lines.Add(BuildRequiredSpecificShipMatricesLine(gs, shipId));
        }
        else if (blueprintCost > 0)
        {
            lines.Add(BuildRequiredBlueprintLine(gs, blueprintCost));
        }

        if (lines.Count == 0)
            return "Sin requisitos.";

        return string.Join("\n", lines.ToArray());
    }

    private void AddShipUnlockRequirementLine(
        GameState gs,
        List<string> lines,
        string metalId,
        double amount
    )
    {
        if (string.IsNullOrEmpty(metalId))
            return;

        if (amount <= 0.0)
            return;

        lines.Add(BuildRequiredMetalLine(gs, metalId, amount));
    }

    private string BuildPlanetUnlockButtonText(GameState gs, string title, string planetId)
    {
        if (!Dimension1System.TryGetPlanetUnlockCost(
            planetId,
            out string metal1,
            out double amount1,
            out string metal2,
            out double amount2,
            out string metal3,
            out double amount3
        ))
        {
            return title + "\nSin costo definido";
        }

        return title +
            "\n" +
            BuildUnlockCostButtonText(
                gs,
                metal1,
                amount1,
                metal2,
                amount2,
                metal3,
                amount3,
                "",
                0.0,
                0
            );
    }

    private string BuildShipUnlockButtonText(GameState gs, string title, string shipId)
    {
        if (!Dimension1System.TryGetShipUnlockCost(
            shipId,
            out string metal1,
            out double amount1,
            out string metal2,
            out double amount2,
            out string metal3,
            out double amount3,
            out string metal4,
            out double amount4,
            out int blueprintCost
        ))
        {
            return title + "\nSin costo definido";
        }

        bool usesSpecificMatrices = Dimension1System.UsesSpecificShipMatricesForUnlock(shipId);

        string costText = BuildUnlockCostButtonText(
            gs,
            metal1,
            amount1,
            metal2,
            amount2,
            metal3,
            amount3,
            metal4,
            amount4,
            usesSpecificMatrices ? 0 : blueprintCost
        );

        if (usesSpecificMatrices)
        {
            costText += " + " + BuildRequiredSpecificShipMatricesButtonText(gs, shipId);
        }

        return title + "\n" + costText;
    }

    private string BuildUnlockCostButtonText(
        GameState gs,
        string metal1,
        double amount1,
        string metal2,
        double amount2,
        string metal3,
        double amount3,
        string metal4,
        double amount4,
        int blueprintCost
    )
    {
        List<string> parts = new List<string>();

        AddUnlockCostButtonPart(gs, parts, metal1, amount1);
        AddUnlockCostButtonPart(gs, parts, metal2, amount2);
        AddUnlockCostButtonPart(gs, parts, metal3, amount3);
        AddUnlockCostButtonPart(gs, parts, metal4, amount4);

        if (blueprintCost > 0)
            parts.Add(BuildRequiredBlueprintButtonText(gs, blueprintCost));

        if (parts.Count == 0)
            return "Sin costo";

        return string.Join(" + ", parts.ToArray());
    }

    private void AddUnlockCostButtonPart(
        GameState gs,
        List<string> parts,
        string metalId,
        double amount
    )
    {
        if (string.IsNullOrEmpty(metalId))
            return;

        if (amount <= 0.0)
            return;

        parts.Add(BuildRequiredMetalButtonText(gs, metalId, amount));
    }

    private string BuildRequiredMetalButtonText(GameState gs, string metalId, double requiredAmount)
    {
        double currentAmount = gs != null
            ? gs.GetD1MetalAmount(metalId)
            : 0.0;

        double displayedAmount = currentAmount > requiredAmount
            ? requiredAmount
            : currentAmount;

        return
            GetMetalVisualName(metalId) +
            " " +
            displayedAmount.ToString("0") +
            "/" +
            requiredAmount.ToString("0");
    }

    private string BuildRequiredSpecificShipMatricesButtonText(GameState gs, string shipId)
    {
        if (!Dimension1System.TryGetRequiredShipBlueprintIds(shipId, out string[] matrixIds))
            return "Matrices no definidas";

        int required = matrixIds.Length;
        int specificOwned = Dimension1System.GetOwnedRequiredSpecificShipMatrixCount(gs, shipId);
        int missing = Dimension1System.GetMissingRequiredSpecificShipMatrixCount(gs, shipId);
        int adaptiveAvailable = Dimension1System.GetCompletedBlueprintCount(gs);
        int adaptiveCovering = adaptiveAvailable > missing ? missing : adaptiveAvailable;
        int totalCovered = specificOwned + adaptiveCovering;

        return "Matrices " + totalCovered + "/" + required;
    }

    private string BuildRequiredSpecificShipUpgradeMatricesButtonText(
    GameState gs,
    string shipId,
    int requiredMatrices
)
    {
        int currentMatrices = Dimension1System.GetAvailableSpecificShipUpgradeMatrixCount(
            gs,
            shipId
        );

        int displayedMatrices = currentMatrices > requiredMatrices
            ? requiredMatrices
            : currentMatrices;

        return
            "Matrices sobrantes " +
            displayedMatrices +
            "/" +
            requiredMatrices;
    }

    private string BuildRequiredSpecificShipMatricesLine(GameState gs, string shipId)
    {
        if (!Dimension1System.TryGetRequiredShipBlueprintIds(shipId, out string[] matrixIds))
            return "- Matrices requeridas: no definidas";

        int required = matrixIds.Length;
        int specificOwned = Dimension1System.GetOwnedRequiredSpecificShipMatrixCount(gs, shipId);
        int missing = Dimension1System.GetMissingRequiredSpecificShipMatrixCount(gs, shipId);
        int adaptiveAvailable = Dimension1System.GetCompletedBlueprintCount(gs);
        int adaptiveCovering = adaptiveAvailable > missing ? missing : adaptiveAvailable;

        string text =
            "- Matrices específicas: " +
            specificOwned +
            "/" +
            required;

        if (missing > 0)
        {
            text +=
                "\n- Matrices Adaptativas cubriendo faltantes: " +
                adaptiveCovering +
                "/" +
                missing;
        }

        return text;
    }

    private string BuildRequiredBlueprintButtonText(GameState gs, int requiredBlueprints)
    {
        int currentBlueprints = Dimension1System.GetCompletedBlueprintCount(gs);

        int displayedBlueprints = currentBlueprints > requiredBlueprints
            ? requiredBlueprints
            : currentBlueprints;

        return
            "Matrices Adaptativas " +
            displayedBlueprints +
            "/" +
            requiredBlueprints;
    }



    private string BuildRequiredMetalLine(GameState gs, string metalId, double requiredAmount)
    {
        double currentAmount = gs != null
            ? gs.GetD1MetalAmount(metalId)
            : 0.0;

        double displayedAmount = currentAmount > requiredAmount
            ? requiredAmount
            : currentAmount;

        return
            "- " +
            GetMetalVisualName(metalId) +
            ": " +
            displayedAmount.ToString("0") +
            "/" +
            requiredAmount.ToString("0");
    }

    private string BuildRequiredBlueprintLine(GameState gs, int requiredBlueprints)
    {
        int currentBlueprints = Dimension1System.GetCompletedBlueprintCount(gs);

        int displayedBlueprints = currentBlueprints > requiredBlueprints
            ? requiredBlueprints
            : currentBlueprints;

        return
            "- Matrices Adaptativas de Nave: " +
            displayedBlueprints +
            "/" +
            requiredBlueprints;
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

        D1ShipState ship = FindShip(gs, shipId);

        if (ship == null || !ship.unlocked)
        {
            button.gameObject.SetActive(false);
            return;
        }

        button.gameObject.SetActive(true);

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

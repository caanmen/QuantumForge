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

    [Header("Camara de Reliquias")]
    [SerializeField] private GameObject relicChamberPanel;
    [SerializeField] private Button openRelicChamberPanelButton;
    [SerializeField] private Button closeRelicChamberPanelButton;
    [SerializeField] private TMP_Dropdown relicChamberDropdown;
    [SerializeField] private TextMeshProUGUI relicChamberInfoText;
    [SerializeField] private Button upgradeSelectedRelicButton;

    [Header("Arbol D1")]
    [SerializeField] private GameObject dimension1TreePanel;
    [SerializeField] private Button openDimension1TreePanelButton;
    [SerializeField] private Button closeDimension1TreePanelButton;
    [SerializeField] private TMP_Dropdown dimension1TreeNodeDropdown;
    [SerializeField] private TextMeshProUGUI dimension1TreeInfoText;
    [SerializeField] private Button buySelectedDimension1TreeNodeButton;

    [Header("Galaxia")]
    [SerializeField] private GameObject galaxyPanel;
    [SerializeField] private Button openGalaxyPanelButton;
    [SerializeField] private Button closeGalaxyPanelButton;

    [SerializeField] private TextMeshProUGUI galaxyTitleText;
    [SerializeField] private TextMeshProUGUI galaxySectorSummaryText;

    [SerializeField] private Button galaxySector1Button;
    [SerializeField] private Button galaxySector2Button;
    [SerializeField] private Button galaxySector3Button;
    [SerializeField] private Button galaxySector4Button;
    [SerializeField] private Button galaxyCenterButton;

    [SerializeField] private Button enterGalaxySectorButton;

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
    private int selectedRelicIndex;
    private string relicChamberDropdownSignature = "";
    private bool isRefreshingRelicChamberDropdown;
    private bool relicChamberPanelOpen;
    private bool dimension1TreePanelOpen;
    private int selectedDimension1TreeNodeIndex;
    private string dimension1TreeDropdownSignature = "";
    private bool isRefreshingDimension1TreeDropdown;
    private bool galaxyPanelOpen;
    private string galaxyPreviewSectorId =
        Dimension1System.Sector01OuterRim;
    private string galaxyFeedbackMessage = "";
    private int lastHandledExplorationResultId;
    private bool showingExplorationResultPanel;
    private bool explorationRecordPanelOpen;
    private int lastObservedDestinationDropdownValue = -1;
    private int lastObservedShipDropdownValue = -1;

    private static readonly string[] HangarShipIds =
        Dimension1System.Dimension1ActiveShipIds;

    private void OnEnable()
    {
        BindGalaxyButtonListeners();
        BindDimension1TreeListeners();
        HideFrozenPart2Controls();
        PrimeExplorationResultPanelState();
        RefreshUI();
    }

    private void OnDisable()
    {
        UnbindGalaxyButtonListeners();
        UnbindDimension1TreeListeners();
    }

    private void BindGalaxyButtonListeners()
    {
        UnbindGalaxyButtonListeners();

        AddGalaxyButtonListener(openGalaxyPanelButton, OnClickOpenGalaxyPanel);
        AddGalaxyButtonListener(closeGalaxyPanelButton, OnClickCloseGalaxyPanel);
        AddGalaxyButtonListener(
            galaxySector1Button,
            OnClickPreviewGalaxySector1
        );
        AddGalaxyButtonListener(
            galaxySector2Button,
            OnClickPreviewGalaxySector2
        );
        AddGalaxyButtonListener(
            galaxySector3Button,
            OnClickPreviewGalaxySector3
        );
        AddGalaxyButtonListener(
            galaxySector4Button,
            OnClickPreviewGalaxySector4
        );
        AddGalaxyButtonListener(
            galaxyCenterButton,
            OnClickPreviewGalaxyCenter
        );
        AddGalaxyButtonListener(
            enterGalaxySectorButton,
            OnClickEnterGalaxySector
        );
    }

    private void UnbindGalaxyButtonListeners()
    {
        RemoveGalaxyButtonListener(openGalaxyPanelButton, OnClickOpenGalaxyPanel);
        RemoveGalaxyButtonListener(closeGalaxyPanelButton, OnClickCloseGalaxyPanel);
        RemoveGalaxyButtonListener(
            galaxySector1Button,
            OnClickPreviewGalaxySector1
        );
        RemoveGalaxyButtonListener(
            galaxySector2Button,
            OnClickPreviewGalaxySector2
        );
        RemoveGalaxyButtonListener(
            galaxySector3Button,
            OnClickPreviewGalaxySector3
        );
        RemoveGalaxyButtonListener(
            galaxySector4Button,
            OnClickPreviewGalaxySector4
        );
        RemoveGalaxyButtonListener(
            galaxyCenterButton,
            OnClickPreviewGalaxyCenter
        );
        RemoveGalaxyButtonListener(
            enterGalaxySectorButton,
            OnClickEnterGalaxySector
        );
    }

    private static void AddGalaxyButtonListener(
        Button button,
        UnityEngine.Events.UnityAction action
    )
    {
        if (button != null)
            button.onClick.AddListener(action);
    }

    private static void RemoveGalaxyButtonListener(
        Button button,
        UnityEngine.Events.UnityAction action
    )
    {
        if (button != null)
            button.onClick.RemoveListener(action);
    }

    private void BindDimension1TreeListeners()
    {
        UnbindDimension1TreeListeners();

        AddGalaxyButtonListener(
            openDimension1TreePanelButton,
            OnClickOpenDimension1TreePanel
        );
        AddGalaxyButtonListener(
            closeDimension1TreePanelButton,
            OnClickCloseDimension1TreePanel
        );
        AddGalaxyButtonListener(
            buySelectedDimension1TreeNodeButton,
            OnClickBuySelectedDimension1TreeNode
        );

        if (dimension1TreeNodeDropdown != null)
        {
            dimension1TreeNodeDropdown.onValueChanged.AddListener(
                OnDimension1TreeNodeDropdownChanged
            );
        }
    }

    private void UnbindDimension1TreeListeners()
    {
        RemoveGalaxyButtonListener(
            openDimension1TreePanelButton,
            OnClickOpenDimension1TreePanel
        );
        RemoveGalaxyButtonListener(
            closeDimension1TreePanelButton,
            OnClickCloseDimension1TreePanel
        );
        RemoveGalaxyButtonListener(
            buySelectedDimension1TreeNodeButton,
            OnClickBuySelectedDimension1TreeNode
        );

        if (dimension1TreeNodeDropdown != null)
        {
            dimension1TreeNodeDropdown.onValueChanged.RemoveListener(
                OnDimension1TreeNodeDropdownChanged
            );
        }
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
        HideFrozenPart2Controls();

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
        RefreshRelicChamberPanel(gs);
        RefreshGalaxyPanel(gs);
        RefreshDimension1TreePanel(gs);

    }

    private void HideFrozenPart2Controls()
    {
        if (unlockRescueShipButton != null)
            unlockRescueShipButton.gameObject.SetActive(false);

        if (unlockConvergenceShipButton != null)
            unlockConvergenceShipButton.gameObject.SetActive(false);
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
        if (gs == null)
            return "Planetas:\nNo disponibles.";

        var lines = new List<string>();

        switch (gs.dimension1SelectedSectorId)
        {
            case Dimension1System.Sector01OuterRim:
                lines.Add(BuildPlanetLine(
                    gs,
                    Dimension1System.Planet01,
                    "Planeta 1",
                    "Hierro",
                    "Cobre"
                ));
                lines.Add(BuildPlanetLine(
                    gs,
                    Dimension1System.Planet02,
                    "Planeta 2",
                    "Aluminio",
                    "Titanio"
                ));
                break;

            case Dimension1System.Sector02DebrisRing:
                lines.Add(BuildPlanetLine(
                    gs,
                    Dimension1System.Planet03,
                    "Planeta 3",
                    "Níquel",
                    "Cobalto"
                ));
                break;

            case Dimension1System.Sector03AncientOrbits:
                lines.Add(BuildPlanetLine(
                    gs,
                    Dimension1System.Planet04,
                    "Planeta 4",
                    "Litio",
                    "Tungsteno"
                ));
                lines.Add(BuildPlanetLine(
                    gs,
                    Dimension1System.Planet05,
                    "Planeta 5",
                    "Platino",
                    "Níquel"
                ));
                break;

            case Dimension1System.Sector04SilentFrontier:
                lines.Add(BuildPlanetLine(
                    gs,
                    Dimension1System.Planet06,
                    "Planeta 6",
                    "Iridio",
                    "Cobalto"
                ));
                lines.Add(BuildPlanetLine(
                    gs,
                    Dimension1System.Planet07,
                    "Planeta 7",
                    "Tungsteno",
                    "Platino",
                    "Iridio"
                ));
                break;

            case Dimension1System.Sector05GalacticCenter:
                return "Planetas:\nEl Centro Galáctico no contiene planetas.";

            default:
                return "Planetas:\nSelecciona un sector.";
        }

        return "Planetas:\n" + string.Join("\n", lines);
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
        return Dimension1System.GetDimension1RelicVisualName(relicId);
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

        float specialPointChance =
            Dimension1System.GetD1SpecialPointScanChance(gs);

        if (specialPointChance > 0.0f)
        {
            scannerHeader +=
                "Puntos especiales: " +
                (specialPointChance * 100f).ToString("0.#") +
                "% de marcar 1 destino\n";
        }

        scannerHeader +=
            "Calidad de escaneo: +" +
            (Dimension1System.GetSimpleScannerQualityBonus(gs) * 100f).ToString("0.#") +
            "%\n";

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
            if (ship == null ||
                !Dimension1System.IsShipActiveInDimension1Base(ship.shipId))
            {
                continue;
            }

            if (!ship.unlocked)
                continue;

            if (!ship.explorationActive)
                continue;

            if (hasAny)
                text += "\n";

            string destinationName = !string.IsNullOrEmpty(ship.activeDestinationId)
                ? GetDestinationVisualName(ship.activeDestinationId)
                : "Destino desconocido";

            string specialPointMarker =
                !string.IsNullOrEmpty(ship.activeSpecialPointId)
                    ? " [P]"
                    : "";

            text +=
                GetShipVisualName(ship.shipId) +
                " → " +
                destinationName +
                specialPointMarker +
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
        bool hasVisibleEntries = false;

        int startIndex = Mathf.Max(0, gs.dimension1RecentExplorationRecords.Count - 20);

        for (int i = gs.dimension1RecentExplorationRecords.Count - 1; i >= startIndex; i--)
        {
            D1ExplorationRecordEntry entry = gs.dimension1RecentExplorationRecords[i];

            if (entry == null ||
                !Dimension1System.IsShipActiveInDimension1Base(entry.shipId))
            {
                continue;
            }

            hasVisibleEntries = true;

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

        if (!hasVisibleEntries)
            return "Registro de exploración:\nSin exploraciones completadas.";

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

                if (!Dimension1System.IsBlueprintActiveInDimension1Part1(matrixReward.blueprintId))
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

        bool shouldShow =
            !hangarPanelOpen &&
            !relicChamberPanelOpen &&
            !galaxyPanelOpen &&
            (shouldShowResult || shouldShowPreview);

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
        bool isMainDimension1View =
            !hangarPanelOpen &&
            !relicChamberPanelOpen &&
            !galaxyPanelOpen;
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

        if (Dimension1System.HasD1TreeDestinationReading(gs) &&
            !string.IsNullOrEmpty(destination.specialPointId))
        {
            text +=
                "\n\nPunto especial:\n" +
                Dimension1System.GetD1SpecialPointVisualName(destination.specialPointId) +
                "\n" +
                Dimension1System.GetD1SpecialPointPreviewDescription(destination.specialPointId);
        }

        if (partialRecoveryChance > 0.0f && partialRecoveryAmount > 0.0f)
        {
            text +=
                "\nRecuperación de Materiales: " +
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
                "\nHallazgo: " +
                GetD1DestinationExpectedFindText(destinationId);
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

        float singleShipEfficiencyBonus =
            Dimension1System.GetD1TreeSingleShipEfficiencyBonus(gs);

        float routeOptimizationBonus =
            Dimension1System.GetD1TreeRouteOptimizationDurationReduction(gs);

        float unstableZoneDurationReduction = 0.0f;
        float unstableZonePreservationBonus = 0.0f;

        if (destinationId == Dimension1System.DestinationUnstableZone)
        {
            unstableZoneDurationReduction =
                Dimension1System.GetD1TreeUnstableZoneDurationReduction(gs);
            unstableZonePreservationBonus =
                Dimension1System.GetD1TreeUnstableZoneRareRewardProtection(gs);
        }

        if (blueprintPriorityBonus > 0.0f ||
            hiddenFindBonus > 0.0f ||
            singleShipEfficiencyBonus > 0.0f ||
            routeOptimizationBonus > 0.0f ||
            unstableZoneDurationReduction > 0.0f ||
            unstableZonePreservationBonus > 0.0f)
        {
            text += "\n\nBonus árbol:";

            if (blueprintPriorityBonus > 0.0f)
            {
                text +=
                    "\nPrioridad matriz faltante: +" +
                    (blueprintPriorityBonus * 100f).ToString("0.#") +
                    " puntos porcentuales";
            }

            if (hiddenFindBonus > 0.0f)
            {
                text +=
                    "\nRastreo oculto: +" +
                    (hiddenFindBonus * 100f).ToString("0.#") +
                    " puntos porcentuales";
            }

            if (singleShipEfficiencyBonus > 0.0f)
            {
                text +=
                    "\nPreparación de Hangar: +" +
                    (singleShipEfficiencyBonus * 100f).ToString("0.#") +
                    "% materiales y -" +
                    (singleShipEfficiencyBonus * 100f).ToString("0.#") +
                    "% duración";
            }

            if (routeOptimizationBonus > 0.0f)
            {
                text +=
                    "\nOptimización de Ruta: -" +
                    (routeOptimizationBonus * 100f).ToString("0.#") +
                    "% duración";
            }

            if (unstableZoneDurationReduction > 0.0f ||
                unstableZonePreservationBonus > 0.0f)
            {
                text +=
                    "\nEstabilización Zona Inestable: -" +
                    (unstableZoneDurationReduction * 100f).ToString("0.#") +
                    "% duración y +" +
                    (unstableZonePreservationBonus * 100f).ToString("0.#") +
                    "% conservación";
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
            "\n- Hallazgo esperado: " +
            GetD1DestinationExpectedFindText(destinationId);
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
                return "matrices, estación y recompensas técnicas";

            case Dimension1System.DestinationMinorAnomaly:
                return "reliquias y hallazgos ocultos";

            case Dimension1System.DestinationAncientStructure:
                return "reliquias raras y matrices avanzadas";

            case Dimension1System.DestinationUnstableZone:
                return "recompensas avanzadas y lecturas inestables";

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
                return "Sonda Analítica / Nave de Carga";

            case Dimension1System.DestinationOrbitalRuin:
                return "Sonda Analítica";

            case Dimension1System.DestinationLaboratory:
                return "Sonda Analítica";

            case Dimension1System.DestinationAbandonedStation:
                return "Nave de Carga / Sonda Analítica";

            case Dimension1System.DestinationMinorAnomaly:
                return "Sonda Analítica";

            case Dimension1System.DestinationAncientStructure:
                return "Sonda Analítica / Nave de Carga";

            case Dimension1System.DestinationUnstableZone:
                return "Nave de Carga / Sonda Analítica";

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

            if (!Dimension1System.IsBlueprintActiveInDimension1Part1(reward.blueprintId))
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

            if (!Dimension1System.IsBlueprintActiveInDimension1Part1(reward.blueprintId))
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

        if (hangarPanelOpen || relicChamberPanelOpen || galaxyPanelOpen)
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

            string specialPointMarker = "";

            if (!string.IsNullOrEmpty(destination.specialPointId))
                specialPointMarker = " [P]";

            options.Add(
                "Destino " +
                visibleIndex +
                ": " +
                GetDestinationVisualName(destination.destinationId) +
                specialPointMarker
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

        if (hangarPanelOpen || relicChamberPanelOpen || galaxyPanelOpen)
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
            if (ship == null ||
                !Dimension1System.IsShipActiveInDimension1Base(ship.shipId) ||
                !ship.unlocked ||
                ship.explorationActive)
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
            if (ship != null &&
                Dimension1System.IsShipActiveInDimension1Base(ship.shipId) &&
                ship.unlocked &&
                !ship.explorationActive)
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
            if (ship == null ||
                !Dimension1System.IsShipActiveInDimension1Base(ship.shipId) ||
                !ship.unlocked ||
                ship.explorationActive)
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

        int cargoLevel = ClampShipPartLevelForD1Base(ship.cargoLevel);
        int speedLevel = ClampShipPartLevelForD1Base(ship.speedLevel);
        int sensorsLevel = ClampShipPartLevelForD1Base(ship.sensorsLevel);

        switch (ship.shipId)
        {
            case Dimension1System.ShipExtractorDrone:
                if (cargoLevel >= 6)
                    return "Carga VI: x2.00 metales de exploración";

                if (cargoLevel >= 5)
                    return "Carga V: x1.80 metales de exploración";

                if (cargoLevel >= 4)
                    return "Carga IV: x1.65 metales de exploración";

                if (cargoLevel >= 3)
                    return "Carga III: x1.50 metales de exploración";

                if (cargoLevel >= 2)
                    return "Carga II: x1.40 metales de exploración";

                if (cargoLevel >= 1)
                    return "Carga I: x1.30 metales de exploración";

                return "Carga base: x1.20 metales de exploración";

            case Dimension1System.ShipAnalyticProbe:
                if (sensorsLevel >= 6)
                    return "Sensores VI: +10% fragmentos de Matriz Adaptativa y barrido 2.1s";

                if (sensorsLevel >= 5)
                    return "Sensores V: +9% fragmentos de Matriz Adaptativa y barrido 2.4s";

                if (sensorsLevel >= 4)
                    return "Sensores IV: +7% fragmentos de Matriz Adaptativa y barrido 2.7s";

                if (sensorsLevel >= 3)
                    return "Sensores III: +6% fragmentos de Matriz Adaptativa y barrido 3.0s";

                if (sensorsLevel >= 2)
                    return "Sensores II: +4% fragmentos de Matriz Adaptativa y barrido 3.5s";

                if (sensorsLevel >= 1)
                    return "Sensores I: +3% fragmentos de Matriz Adaptativa y barrido 4.0s";

                return "Sensores base: +2% fragmentos de Matriz Adaptativa";

            case Dimension1System.ShipLightProbe:
                if (speedLevel >= 6)
                    return "Velocidad VI: exploración x0.58 tiempo";

                if (speedLevel >= 5)
                    return "Velocidad V: exploración x0.64 tiempo";

                if (speedLevel >= 4)
                    return "Velocidad IV: exploración x0.70 tiempo";

                if (speedLevel >= 3)
                    return "Velocidad III: exploración x0.76 tiempo";

                if (speedLevel >= 2)
                    return "Velocidad II: exploración x0.82 tiempo";

                if (speedLevel >= 1)
                    return "Velocidad I: exploración x0.90 tiempo";

                return "Velocidad base: exploración x1.00 tiempo";

            case Dimension1System.ShipCargoShip:
                if (cargoLevel >= 6)
                    return "Bodega Modular VI: x1.75 metales de exploración";

                if (cargoLevel >= 5)
                    return "Bodega Modular V: x1.60 metales de exploración";

                if (cargoLevel >= 4)
                    return "Bodega Modular IV: x1.47 metales de exploración";

                if (cargoLevel >= 3)
                    return "Bodega Modular III: x1.35 metales de exploración";

                if (cargoLevel >= 2)
                    return "Bodega Modular II: x1.26 metales de exploración";

                if (cargoLevel >= 1)
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
            if (ship == null ||
                !Dimension1System.IsShipActiveInDimension1Base(ship.shipId) ||
                !ship.unlocked)
            {
                continue;
            }

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
             "\n\nMatrices específicas:\n" +
             BuildSpecificBlueprintArchiveText(gs);

        return text;
    }

    private string BuildSpecificBlueprintArchiveText(GameState gs)
    {
        if (gs == null)
            return "No disponible.";

        List<string> lines = new List<string>();

        foreach (string blueprintId in Dimension1System.Dimension1Part1BlueprintIds)
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
        level = ClampShipPartLevelForD1Base(level);

        if (level <= 0)
            return "Base";

        return ToRomanNumber(level);
    }

    private int ClampShipPartLevelForD1Base(int level)
    {
        return Mathf.Clamp(level, 0, Dimension1System.Dimension1ShipPartMaxLevel);
    }

    private string GetLightProbeSpeedMultiplierText(int level)
    {
        level = ClampShipPartLevelForD1Base(level);

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
        level = ClampShipPartLevelForD1Base(level);

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
        level = ClampShipPartLevelForD1Base(level);

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
        level = ClampShipPartLevelForD1Base(level);

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
        level = ClampShipPartLevelForD1Base(level);

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
        hangarPanelOpen = false;
        relicChamberPanelOpen = false;
        galaxyPanelOpen = false;
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

        // Los botones futuros pueden seguir asignados en la escena, pero Parte 1
        // nunca debe volver a mostrarlos durante un refresco de la UI.
        if (unlockRescueShipButton != null)
            unlockRescueShipButton.gameObject.SetActive(false);

        if (unlockConvergenceShipButton != null)
            unlockConvergenceShipButton.gameObject.SetActive(false);
    }

    private void SetShipUnlockButtonsVisible(bool visible)
    {
        if (unlockExtractorDroneButton != null)
            unlockExtractorDroneButton.gameObject.SetActive(visible);

        if (unlockAnalyticProbeButton != null)
            unlockAnalyticProbeButton.gameObject.SetActive(visible);

        if (unlockCargoShipButton != null)
            unlockCargoShipButton.gameObject.SetActive(visible);

        if (unlockRescueShipButton != null)
            unlockRescueShipButton.gameObject.SetActive(false);

        if (unlockConvergenceShipButton != null)
            unlockConvergenceShipButton.gameObject.SetActive(false);
    }

    private void RefreshButtons(GameState gs)
    {
        if (gs == null)
            return;

        if (galaxyPanelOpen)
        {
            SetMainExplorationControlsVisible(false);
            SetShipUnlockButtonsVisible(false);
            return;
        }

        if (relicChamberPanelOpen)
        {
            SetMainExplorationControlsVisible(false);
            SetShipUnlockButtonsVisible(false);
            return;
        }

        if (hangarPanelOpen)
        {
            SetMainExplorationControlsVisible(false);
            RefreshShipUnlockButtons(gs);
            return;
        }

        SetMainExplorationControlsVisible(true);

        bool planet1Visible = Dimension1System.IsD1PlanetInSelectedSector(
            gs,
            Dimension1System.Planet01
        );

        if (upgradePlanet1Button != null)
            upgradePlanet1Button.gameObject.SetActive(planet1Visible);

        if (planet1Visible)
        {
            RefreshUpgradeButton(
                upgradePlanet1Button,
                gs,
                Dimension1System.Planet01,
                "Mejorar P1"
            );
        }

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
                "Escáner máximo\n" +
                Dimension1System.SimpleScannerMaxLevel +
                "/" +
                Dimension1System.SimpleScannerMaxLevel +
                " | " +
                Dimension1System.GetSimpleScanMaxDestinationCount() +
                "/" +
                Dimension1System.GetSimpleScanMaxDestinationCount() +
                " destinos"
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

            case 7:
                return "VII";

            case 8:
                return "VIII";

            case 9:
                return "IX";

            case 10:
                return "X";

            case 11:
                return "XI";

            case 12:
                return "XII";

            case 13:
                return "XIII";

            case 14:
                return "XIV";

            case 15:
                return "XV";

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
        bool inSelectedSector = Dimension1System.IsD1PlanetInSelectedSector(
            gs,
            Dimension1System.Planet02
        );

        if (unlockPlanet2Button != null)
        {
            bool hasRequiredMetalsStarted =
            HasStartedReceivingPlanetUnlockMetals(gs, Dimension1System.Planet02);
            unlockPlanet2Button.gameObject.SetActive(
                inSelectedSector && !unlocked && hasRequiredMetalsStarted
            );
            unlockPlanet2Button.interactable = Dimension1System.CanUnlockPlanet(gs, Dimension1System.Planet02);
            SetButtonText(
            unlockPlanet2Button,
            BuildPlanetUnlockButtonText(gs, "Desbloquear P2", Dimension1System.Planet02)
);
        }

        if (upgradePlanet2Button != null)
        {
            upgradePlanet2Button.gameObject.SetActive(inSelectedSector && unlocked);
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
        bool inSelectedSector = Dimension1System.IsD1PlanetInSelectedSector(
            gs,
            Dimension1System.Planet03
        );

        if (unlockPlanet3Button != null)
        {
            bool hasRequiredMetalsStarted =
            HasStartedReceivingPlanetUnlockMetals(gs, Dimension1System.Planet03);
            unlockPlanet3Button.gameObject.SetActive(
                inSelectedSector && !unlocked && hasRequiredMetalsStarted
            );
            unlockPlanet3Button.interactable = Dimension1System.CanUnlockPlanet(gs, Dimension1System.Planet03);
            SetButtonText(
            unlockPlanet3Button,
            BuildPlanetUnlockButtonText(gs, "Desbloquear P3", Dimension1System.Planet03)
);
        }

        if (upgradePlanet3Button != null)
        {
            upgradePlanet3Button.gameObject.SetActive(inSelectedSector && unlocked);
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
        bool inSelectedSector = Dimension1System.IsD1PlanetInSelectedSector(
            gs,
            Dimension1System.Planet04
        );

        if (unlockPlanet4Button != null)
        {
            bool hasRequiredMetalsStarted =
                HasStartedReceivingPlanetUnlockMetals(gs, Dimension1System.Planet04);

            unlockPlanet4Button.gameObject.SetActive(
                inSelectedSector && !unlocked && hasRequiredMetalsStarted
            );
            unlockPlanet4Button.interactable = Dimension1System.CanUnlockPlanet(gs, Dimension1System.Planet04);
            SetButtonText(
            unlockPlanet4Button,
            BuildPlanetUnlockButtonText(gs, "Desbloquear P4", Dimension1System.Planet04)
);
        }

        if (upgradePlanet4Button != null)
        {
            upgradePlanet4Button.gameObject.SetActive(inSelectedSector && unlocked);
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
        bool inSelectedSector = Dimension1System.IsD1PlanetInSelectedSector(
            gs,
            Dimension1System.Planet05
        );

        if (unlockPlanet5Button != null)
        {
            bool hasRequiredMetalsStarted =
                HasStartedReceivingPlanetUnlockMetals(gs, Dimension1System.Planet05);

            unlockPlanet5Button.gameObject.SetActive(
                inSelectedSector && !unlocked && hasRequiredMetalsStarted
            );
            unlockPlanet5Button.interactable = Dimension1System.CanUnlockPlanet(gs, Dimension1System.Planet05);
            SetButtonText(
            unlockPlanet5Button,
            BuildPlanetUnlockButtonText(gs, "Desbloquear P5", Dimension1System.Planet05)
);
        }

        if (upgradePlanet5Button != null)
        {
            upgradePlanet5Button.gameObject.SetActive(inSelectedSector && unlocked);
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
        bool inSelectedSector = Dimension1System.IsD1PlanetInSelectedSector(
            gs,
            Dimension1System.Planet06
        );

        if (unlockPlanet6Button != null)
        {
            bool hasRequiredMetalsStarted =
                HasStartedReceivingPlanetUnlockMetals(gs, Dimension1System.Planet06);

            unlockPlanet6Button.gameObject.SetActive(
                inSelectedSector && !unlocked && hasRequiredMetalsStarted
            );
            unlockPlanet6Button.interactable = Dimension1System.CanUnlockPlanet(gs, Dimension1System.Planet06);
            SetButtonText(
            unlockPlanet6Button,
            BuildPlanetUnlockButtonText(gs, "Desbloquear P6", Dimension1System.Planet06)
);
        }

        if (upgradePlanet6Button != null)
        {
            upgradePlanet6Button.gameObject.SetActive(inSelectedSector && unlocked);
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
        bool inSelectedSector = Dimension1System.IsD1PlanetInSelectedSector(
            gs,
            Dimension1System.Planet07
        );

        if (unlockPlanet7Button != null)
        {
            bool hasRequiredMetalsStarted =
                HasStartedReceivingPlanetUnlockMetals(gs, Dimension1System.Planet07);

            unlockPlanet7Button.gameObject.SetActive(
                inSelectedSector && !unlocked && hasRequiredMetalsStarted
            );
            unlockPlanet7Button.interactable = Dimension1System.CanUnlockPlanet(gs, Dimension1System.Planet07);
            SetButtonText(
            unlockPlanet7Button,
            BuildPlanetUnlockButtonText(gs, "Desbloquear P7", Dimension1System.Planet07)
);
        }

        if (upgradePlanet7Button != null)
        {
            upgradePlanet7Button.gameObject.SetActive(inSelectedSector && unlocked);
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

    public void OnClickOpenGalaxyPanel()
    {
        if (galaxyPanel == null)
            return;

        GameState gs = GameState.I;

        if (gs != null)
        {
            gs.EnsureDimension1State();
            galaxyPreviewSectorId = gs.dimension1SelectedSectorId;
        }

        galaxyFeedbackMessage = "";
        galaxyPanelOpen = true;
        hangarPanelOpen = false;
        relicChamberPanelOpen = false;
        dimension1TreePanelOpen = false;
        CloseExplorationOverlayPanels();

        RefreshUI();
    }

    public void OnClickCloseGalaxyPanel()
    {
        galaxyPanelOpen = false;
        galaxyFeedbackMessage = "";
        RefreshUI();
    }

    public void OnClickPreviewGalaxySector1()
    {
        PreviewGalaxySector(Dimension1System.Sector01OuterRim);
    }

    public void OnClickPreviewGalaxySector2()
    {
        PreviewGalaxySector(Dimension1System.Sector02DebrisRing);
    }

    public void OnClickPreviewGalaxySector3()
    {
        PreviewGalaxySector(Dimension1System.Sector03AncientOrbits);
    }

    public void OnClickPreviewGalaxySector4()
    {
        PreviewGalaxySector(Dimension1System.Sector04SilentFrontier);
    }

    public void OnClickPreviewGalaxyCenter()
    {
        PreviewGalaxySector(Dimension1System.Sector05GalacticCenter);
    }

    private void PreviewGalaxySector(string sectorId)
    {
        if (!Dimension1System.IsDimension1SectorId(sectorId))
            return;

        galaxyPreviewSectorId = sectorId;
        galaxyFeedbackMessage = "";
        RefreshUI();
    }

    public void OnClickEnterGalaxySector()
    {
        GameState gs = GameState.I;

        if (!CanEnterGalaxyPreviewSector(gs, out string blockedReason))
        {
            galaxyFeedbackMessage = blockedReason;
            RefreshUI();
            return;
        }

        if (!gs.TrySelectD1Sector(galaxyPreviewSectorId))
        {
            galaxyFeedbackMessage =
                "No fue posible entrar al sector seleccionado.";
            RefreshUI();
            return;
        }

        galaxyPanelOpen = false;
        galaxyFeedbackMessage = "";
        selectedDestinationIndex = 0;
        destinationDropdownSignature = "";
        CloseExplorationOverlayPanels();

        if (SaveService.I != null)
            SaveService.I.Save();

        RefreshUI();
    }

    private void RefreshGalaxyPanel(GameState gs)
    {
        bool secondaryPanelOpen =
            hangarPanelOpen ||
            relicChamberPanelOpen ||
            dimension1TreePanelOpen ||
            galaxyPanelOpen;

        if (dimension1MainContentRoot != null)
            dimension1MainContentRoot.SetActive(!secondaryPanelOpen);

        if (openGalaxyPanelButton != null)
        {
            openGalaxyPanelButton.gameObject.SetActive(
                galaxyPanel != null &&
                !secondaryPanelOpen
            );

            SetButtonText(openGalaxyPanelButton, "Galaxia");
        }

        if (galaxyPanel != null)
            galaxyPanel.SetActive(galaxyPanelOpen);

        RefreshGalaxySectorButton(
            galaxySector1Button,
            gs,
            Dimension1System.Sector01OuterRim
        );
        RefreshGalaxySectorButton(
            galaxySector2Button,
            gs,
            Dimension1System.Sector02DebrisRing
        );
        RefreshGalaxySectorButton(
            galaxySector3Button,
            gs,
            Dimension1System.Sector03AncientOrbits
        );
        RefreshGalaxySectorButton(
            galaxySector4Button,
            gs,
            Dimension1System.Sector04SilentFrontier
        );
        RefreshGalaxySectorButton(
            galaxyCenterButton,
            gs,
            Dimension1System.Sector05GalacticCenter
        );

        if (closeGalaxyPanelButton != null)
        {
            closeGalaxyPanelButton.gameObject.SetActive(galaxyPanelOpen);
            SetButtonText(closeGalaxyPanelButton, "Volver");
        }

        if (!galaxyPanelOpen)
        {
            if (enterGalaxySectorButton != null)
                enterGalaxySectorButton.gameObject.SetActive(false);

            return;
        }

        if (galaxyTitleText != null)
            galaxyTitleText.text = "GALAXIA";

        if (galaxySectorSummaryText != null)
            galaxySectorSummaryText.text = BuildGalaxySectorSummary(gs);

        RefreshEnterGalaxySectorButton(gs);
    }

    private void RefreshGalaxySectorButton(
        Button button,
        GameState gs,
        string sectorId
    )
    {
        if (button == null)
            return;

        button.gameObject.SetActive(galaxyPanelOpen);
        button.interactable = galaxyPanelOpen;

        if (!galaxyPanelOpen)
            return;

        bool current =
            gs != null &&
            gs.dimension1SelectedSectorId == sectorId;
        bool unlocked = IsGalaxySectorUnlocked(gs, sectorId);

        string stateText = current
            ? "ACTUAL"
            : unlocked
                ? "DESBLOQUEADO"
                : "BLOQUEADO";

        string previewMarker = galaxyPreviewSectorId == sectorId
            ? " >"
            : "";

        SetButtonText(
            button,
            Dimension1System.GetDimension1SectorVisualName(sectorId) +
            "\n[" + stateText + "]" +
            previewMarker
        );
    }

    private void RefreshEnterGalaxySectorButton(GameState gs)
    {
        if (enterGalaxySectorButton == null)
            return;

        enterGalaxySectorButton.gameObject.SetActive(galaxyPanelOpen);

        bool canEnter = CanEnterGalaxyPreviewSector(gs, out _);
        enterGalaxySectorButton.interactable = canEnter;

        bool current =
            gs != null &&
            gs.dimension1SelectedSectorId == galaxyPreviewSectorId;

        SetButtonText(
            enterGalaxySectorButton,
            current ? "Volver al sector" : "Entrar"
        );
    }

    private bool CanEnterGalaxyPreviewSector(
        GameState gs,
        out string blockedReason
    )
    {
        blockedReason = "";

        if (gs == null)
        {
            blockedReason = "Dimensión 1 no está disponible.";
            return false;
        }

        if (!Dimension1System.IsDimension1SectorId(galaxyPreviewSectorId))
        {
            blockedReason = "Selecciona un sector válido.";
            return false;
        }

        if (!IsGalaxySectorUnlocked(gs, galaxyPreviewSectorId))
        {
            blockedReason = "Este sector todavía está bloqueado.";
            return false;
        }

        if (gs.dimension1ScanActive &&
            gs.dimension1ActiveScanSectorId != galaxyPreviewSectorId)
        {
            blockedReason =
                "Hay un escaneo activo en otro sector. Espera a que termine.";
            return false;
        }

        return true;
    }

    private string BuildGalaxySectorSummary(GameState gs)
    {
        if (gs == null)
            return "Dimensión 1 no está disponible.";

        if (!Dimension1System.IsDimension1SectorId(galaxyPreviewSectorId))
            galaxyPreviewSectorId = gs.dimension1SelectedSectorId;

        string sectorId = galaxyPreviewSectorId;
        string sectorName =
            Dimension1System.GetDimension1SectorVisualName(sectorId);
        bool unlocked = IsGalaxySectorUnlocked(gs, sectorId);
        bool current = gs.dimension1SelectedSectorId == sectorId;

        var lines = new List<string>
        {
            sectorName,
            "",
            "Estado: " + (unlocked ? "Desbloqueado" : "Bloqueado"),
            "Sector visitado actualmente: " + (current ? "Sí" : "No"),
            "Exploraciones completadas: " +
                GetGalaxySectorExplorationCount(gs, sectorId),
            "",
            BuildGalaxySectorPlanetsText(sectorId),
            "",
            BuildGalaxySectorDestinationsText(sectorId)
        };

        string requirementsText = BuildGalaxySectorRequirementsText(gs, sectorId);

        if (!string.IsNullOrEmpty(requirementsText))
        {
            lines.Add("");
            lines.Add(requirementsText);
        }

        if (!string.IsNullOrEmpty(galaxyFeedbackMessage))
        {
            lines.Add("");
            lines.Add(galaxyFeedbackMessage);
        }
        else if (!CanEnterGalaxyPreviewSector(gs, out string blockedReason) &&
                 !string.IsNullOrEmpty(blockedReason))
        {
            lines.Add("");
            lines.Add(blockedReason);
        }

        return string.Join("\n", lines);
    }

    private string BuildGalaxySectorRequirementsText(
        GameState gs,
        string sectorId
    )
    {
        List<D1SectorRequirementStatus> requirements =
            Dimension1System.GetD1SectorUnlockRequirements(gs, sectorId);

        if (requirements.Count == 0)
            return "";

        var lines = new List<string>
        {
            "Requisitos de acceso:"
        };

        foreach (D1SectorRequirementStatus requirement in requirements)
        {
            if (requirement == null)
                continue;

            string line =
                (requirement.met ? "[OK] " : "[ ] ") +
                requirement.label;

            if (requirement.showProgress)
            {
                line +=
                    " (" +
                    Mathf.Min(requirement.currentValue, requirement.requiredValue) +
                    "/" +
                    requirement.requiredValue +
                    ")";
            }

            lines.Add(line);
        }

        return string.Join("\n", lines);
    }

    private bool IsGalaxySectorUnlocked(GameState gs, string sectorId)
    {
        if (gs == null ||
            gs.dimension1Sectors == null ||
            !Dimension1System.IsDimension1SectorId(sectorId))
        {
            return false;
        }

        foreach (D1SectorState sector in gs.dimension1Sectors)
        {
            if (sector != null && sector.sectorId == sectorId)
                return sector.unlocked;
        }

        return false;
    }

    private int GetGalaxySectorExplorationCount(
        GameState gs,
        string sectorId
    )
    {
        if (gs == null || gs.dimension1Sectors == null)
            return 0;

        foreach (D1SectorState sector in gs.dimension1Sectors)
        {
            if (sector != null && sector.sectorId == sectorId)
                return Mathf.Max(0, sector.completedExplorations);
        }

        return 0;
    }

    private string BuildGalaxySectorPlanetsText(string sectorId)
    {
        if (sectorId == Dimension1System.Sector05GalacticCenter)
            return "Planetas: ninguno.";

        var planetNames = new List<string>();

        foreach (string planetId in Dimension1System.StarterPlanets)
        {
            if (Dimension1System.GetDimension1PlanetSectorId(planetId) != sectorId)
                continue;

            planetNames.Add(GetGalaxyPlanetVisualName(planetId));
        }

        return planetNames.Count > 0
            ? "Planetas: " + string.Join(", ", planetNames)
            : "Planetas: ninguno.";
    }

    private string BuildGalaxySectorDestinationsText(string sectorId)
    {
        if (sectorId == Dimension1System.Sector05GalacticCenter)
        {
            return
                "Zona especial: hoyo negro y Ark.\n" +
                "No posee planetas ni escaneos normales.";
        }

        string[] destinationIds =
            Dimension1System.GetDimension1SectorDestinationIds(sectorId);
        var destinationNames = new List<string>();

        foreach (string destinationId in destinationIds)
            destinationNames.Add(GetDestinationVisualName(destinationId));

        return destinationNames.Count > 0
            ? "Destinos posibles: " + string.Join(", ", destinationNames)
            : "Destinos posibles: ninguno.";
    }

    private string GetGalaxyPlanetVisualName(string planetId)
    {
        switch (planetId)
        {
            case Dimension1System.Planet01:
                return "Planeta 1";
            case Dimension1System.Planet02:
                return "Planeta 2";
            case Dimension1System.Planet03:
                return "Planeta 3";
            case Dimension1System.Planet04:
                return "Planeta 4";
            case Dimension1System.Planet05:
                return "Planeta 5";
            case Dimension1System.Planet06:
                return "Planeta 6";
            case Dimension1System.Planet07:
                return "Planeta 7";
            default:
                return planetId ?? "";
        }
    }

    public void OnClickOpenHangarPanel()
    {
        hangarPanelOpen = true;
        relicChamberPanelOpen = false;
        dimension1TreePanelOpen = false;
        galaxyPanelOpen = false;
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

    public void OnClickOpenRelicChamberPanel()
    {
        if (relicChamberPanel == null)
            return;

        relicChamberPanelOpen = true;
        hangarPanelOpen = false;
        dimension1TreePanelOpen = false;
        galaxyPanelOpen = false;
        explorationRecordPanelOpen = false;
        showingExplorationResultPanel = false;
        RefreshUI();
    }

    public void OnClickCloseRelicChamberPanel()
    {
        relicChamberPanelOpen = false;
        RefreshUI();
    }

    public void OnRelicChamberDropdownChanged(int index)
    {
        if (isRefreshingRelicChamberDropdown)
            return;

        selectedRelicIndex = index;
        RefreshUI();
    }

    public void OnClickUpgradeSelectedRelic()
    {
        GameState gs = GameState.I;

        if (gs == null)
            return;

        string relicId = GetSelectedRelicChamberRelicId();

        if (string.IsNullOrEmpty(relicId))
            return;

        Dimension1System.TryUpgradeDimension1Relic(gs, relicId);

        if (SaveService.I != null)
            SaveService.I.Save();

        RefreshUI();
    }

    private void RefreshRelicChamberPanel(GameState gs)
    {
        if (dimension1MainContentRoot != null)
        {
            dimension1MainContentRoot.SetActive(
                !hangarPanelOpen &&
                !relicChamberPanelOpen &&
                !dimension1TreePanelOpen &&
                !galaxyPanelOpen
            );
        }

        if (openRelicChamberPanelButton != null)
        {
            openRelicChamberPanelButton.gameObject.SetActive(
                relicChamberPanel != null &&
                !hangarPanelOpen &&
                !relicChamberPanelOpen &&
                !dimension1TreePanelOpen &&
                !galaxyPanelOpen
            );
            SetButtonText(openRelicChamberPanelButton, "Reliquias");
        }

        if (relicChamberPanel != null)
            relicChamberPanel.SetActive(relicChamberPanelOpen);

        if (closeRelicChamberPanelButton != null)
        {
            closeRelicChamberPanelButton.gameObject.SetActive(relicChamberPanelOpen);
            SetButtonText(closeRelicChamberPanelButton, "Cerrar");
        }

        if (!relicChamberPanelOpen)
        {
            if (relicChamberInfoText != null)
                relicChamberInfoText.text = "";

            if (upgradeSelectedRelicButton != null)
                upgradeSelectedRelicButton.gameObject.SetActive(false);

            return;
        }

        RefreshRelicChamberDropdown(gs);

        string relicId = GetSelectedRelicChamberRelicId();

        if (relicChamberInfoText != null)
            relicChamberInfoText.text = BuildRelicChamberInfoText(gs, relicId);

        RefreshSelectedRelicUpgradeButton(gs, relicId);
    }

    private void RefreshRelicChamberDropdown(GameState gs)
    {
        if (relicChamberDropdown == null)
            return;

        List<string> options = new List<string>();
        options.Add("Elegir reliquia");

        string[] relicIds = Dimension1System.Dimension1RelicIds;

        for (int i = 0; i < relicIds.Length; i++)
        {
            string relicId = relicIds[i];

            if (!Dimension1System.IsRelicActiveInDimension1Base(relicId))
                continue;

            string optionText =
                GetRelicSectorShortLabel(relicId) +
                " · N" +
                Dimension1System.GetDimension1RelicTier(relicId) +
                " · " +
                GetRelicVisualName(relicId);

            if (gs == null || !gs.IsD1RelicUnlocked(relicId))
            {
                optionText += " (bloqueada)";
            }
            else
            {
                optionText +=
                    " Nv " +
                    gs.GetD1RelicLevel(relicId) +
                    "/" +
                    Dimension1System.Dimension1RelicMaxLevel;
            }

            options.Add(optionText);
        }

        string newSignature = "";

        for (int i = 0; i < options.Count; i++)
        {
            if (i > 0)
                newSignature += "|";

            newSignature += options[i];
        }

        // Conserva la selección hecha directamente en el TMP_Dropdown.
        // Esto también evita que el refresco periódico lo devuelva a 0
        // si el evento OnValueChanged todavía no alcanzó a actualizar el índice.
        if (!isRefreshingRelicChamberDropdown)
        {
            int currentDropdownValue = relicChamberDropdown.value;

            if (currentDropdownValue >= 0 && currentDropdownValue < options.Count)
                selectedRelicIndex = currentDropdownValue;
        }

        isRefreshingRelicChamberDropdown = true;

        if (newSignature != relicChamberDropdownSignature)
        {
            relicChamberDropdown.ClearOptions();
            relicChamberDropdown.AddOptions(options);
            relicChamberDropdownSignature = newSignature;

            if (selectedRelicIndex >= options.Count)
                selectedRelicIndex = 0;
        }

        if (selectedRelicIndex < 0 || selectedRelicIndex >= options.Count)
            selectedRelicIndex = 0;

        relicChamberDropdown.SetValueWithoutNotify(selectedRelicIndex);
        relicChamberDropdown.RefreshShownValue();
        relicChamberDropdown.interactable = options.Count > 1;

        isRefreshingRelicChamberDropdown = false;
    }

    private string GetRelicSectorShortLabel(string relicId)
    {
        switch (Dimension1System.GetDimension1RelicSectorId(relicId))
        {
            case Dimension1System.Sector01OuterRim:
                return "S1";
            case Dimension1System.Sector02DebrisRing:
                return "S2";
            case Dimension1System.Sector03AncientOrbits:
                return "S3";
            case Dimension1System.Sector04SilentFrontier:
                return "S4";
            default:
                return "D1";
        }
    }

    private string GetSelectedRelicChamberRelicId()
    {
        int index = selectedRelicIndex;

        if (relicChamberDropdown != null)
            index = relicChamberDropdown.value;

        if (index <= 0)
            return "";

        int activeIndex = 1;

        foreach (string relicId in Dimension1System.Dimension1RelicIds)
        {
            if (!Dimension1System.IsRelicActiveInDimension1Base(relicId))
                continue;

            if (activeIndex == index)
                return relicId;

            activeIndex++;
        }

        return "";
    }

    private string BuildRelicChamberInfoText(GameState gs, string relicId)
    {
        if (gs == null)
            return "Camara de Reliquias\nNo disponible.";

        if (string.IsNullOrEmpty(relicId))
            return BuildRelicChamberSummaryText(gs);

        bool unlocked = gs.IsD1RelicUnlocked(relicId);
        int level = unlocked
            ? Dimension1System.ClampDimension1RelicLevel(gs.GetD1RelicLevel(relicId))
            : 0;
        int unlockedCount = GetUnlockedDimension1RelicCount(gs, out int activeCount);
        int currentMilestone = Dimension1System.GetDimension1RelicMilestone(level);

        string text =
            "Camara de Reliquias\n" +
            "Coleccion: " +
            unlockedCount +
            "/" +
            activeCount +
            "\n\n" +
            GetRelicVisualName(relicId) +
            "\n" +
            "Sector: " +
            Dimension1System.GetDimension1SectorVisualName(
                Dimension1System.GetDimension1RelicSectorId(relicId)
            ) +
            "\nCategoria de descubrimiento: Nivel " +
            Dimension1System.GetDimension1RelicTier(relicId) +
            "\nDestinos: " +
            BuildRelicCompatibleDestinationsText(relicId) +
            "\n" +
            "Estado: " +
            (unlocked ? "Obtenida" : "Bloqueada") +
            "\nNivel de mejora: " +
            level +
            "/" +
            Dimension1System.Dimension1RelicMaxLevel +
            "\nHito actual: " +
            (currentMilestone > 0 ? currentMilestone.ToString() : "ninguno") +
            "\n\nEfecto actual:\n" +
            GetRelicEffectText(
                relicId,
                Dimension1System.GetDimension1RelicPrimaryBonusForLevel(relicId, level),
                Dimension1System.GetDimension1RelicSecondaryBonusForLevel(relicId, level)
            );

        if (level < Dimension1System.Dimension1RelicMaxLevel)
        {
            int nextMilestone =
                ((level / Dimension1System.Dimension1RelicMilestoneStep) + 1) *
                Dimension1System.Dimension1RelicMilestoneStep;

            if (Dimension1System.TryGetDimension1RelicMilestoneBonuses(
                relicId,
                nextMilestone,
                out double nextPrimaryBonus,
                out double nextSecondaryBonus
            ))
            {
                text +=
                    "\n\nProximo hito (nivel " +
                    nextMilestone +
                    "):\n" +
                    GetRelicEffectText(relicId, nextPrimaryBonus, nextSecondaryBonus);
            }
        }
        else
        {
            text += "\n\nHitos: todos completados.";
        }

        if (!unlocked)
        {
            text +=
                "\n\nRequisitos de descubrimiento:\n" +
                Dimension1System.GetDimension1RelicDiscoveryRequirementsText(
                    gs,
                    relicId
                );
            return text;
        }

        if (!Dimension1System.TryGetNextDimension1RelicUpgradeCost(
            gs,
            relicId,
            out int targetLevel,
            out double leCost,
            out double traceCost,
            out string metal1,
            out double metalAmount1,
            out string metal2,
            out double metalAmount2
        ))
        {
            text += "\n\nMejora:\nNivel maximo alcanzado.";
            return text;
        }

        text +=
            "\n\nMejora a nivel " +
            targetLevel +
            ":\n" +
            BuildRelicUpgradeCostText(gs, leCost, traceCost, metal1, metalAmount1, metal2, metalAmount2);

        return text;
    }

    private string BuildRelicChamberSummaryText(GameState gs)
    {
        int unlockedCount = GetUnlockedDimension1RelicCount(gs, out int activeCount);
        int tier1Count = GetUnlockedDimension1RelicCountByTier(gs, 1);
        int tier2Count = GetUnlockedDimension1RelicCountByTier(gs, 2);
        int tier3Count = GetUnlockedDimension1RelicCountByTier(gs, 3);
        int totalLevel = 0;

        foreach (string relicId in Dimension1System.Dimension1RelicIds)
        {
            if (!Dimension1System.IsRelicActiveInDimension1Base(relicId))
                continue;

            if (!gs.IsD1RelicUnlocked(relicId))
                continue;

            totalLevel += gs.GetD1RelicLevel(relicId);
        }

        return
            "Camara de Reliquias\n" +
            "Coleccion: " +
            unlockedCount +
            "/" +
            activeCount +
            "\nNivel 1: " +
            tier1Count +
            "/7 | Nivel 2: " +
            tier2Count +
            "/8 | Nivel 3: " +
            tier3Count +
            "/5" +
            "\nNiveles acumulados: " +
            totalLevel +
            "\nMaximo por reliquia: " +
            Dimension1System.Dimension1RelicMaxLevel +
            "\n\nSelecciona una reliquia para ver su efecto y coste de mejora.";
    }

    private int GetUnlockedDimension1RelicCount(GameState gs, out int activeCount)
    {
        int unlockedCount = 0;
        activeCount = 0;

        foreach (string relicId in Dimension1System.Dimension1RelicIds)
        {
            if (!Dimension1System.IsRelicActiveInDimension1Base(relicId))
                continue;

            activeCount++;

            if (gs != null && gs.IsD1RelicUnlocked(relicId))
                unlockedCount++;
        }

        return unlockedCount;
    }

    private int GetUnlockedDimension1RelicCountByTier(
        GameState gs,
        int relicTier
    )
    {
        if (gs == null)
            return 0;

        int count = 0;

        foreach (string relicId in Dimension1System.Dimension1RelicIds)
        {
            if (Dimension1System.GetDimension1RelicTier(relicId) != relicTier)
                continue;

            if (gs.IsD1RelicUnlocked(relicId))
                count++;
        }

        return count;
    }

    private string BuildRelicUpgradeCostText(
        GameState gs,
        double leCost,
        double traceCost,
        string metal1,
        double metalAmount1,
        string metal2,
        double metalAmount2
    )
    {
        List<string> parts = new List<string>();

        parts.Add("LE " + FormatOwnedRequired(gs.LE, leCost));
        parts.Add("Trazas " + FormatOwnedRequired(gs.Traces, traceCost));

        if (!string.IsNullOrEmpty(metal1) && metalAmount1 > 0.0)
            parts.Add(GetMetalVisualName(metal1) + " " + FormatOwnedRequired(gs.GetD1MetalAmount(metal1), metalAmount1));

        if (!string.IsNullOrEmpty(metal2) && metalAmount2 > 0.0)
            parts.Add(GetMetalVisualName(metal2) + " " + FormatOwnedRequired(gs.GetD1MetalAmount(metal2), metalAmount2));

        return string.Join(" + ", parts);
    }

    private string FormatOwnedRequired(double ownedAmount, double requiredAmount)
    {
        return
            ownedAmount.ToString("0") +
            "/" +
            requiredAmount.ToString("0");
    }

    private void RefreshSelectedRelicUpgradeButton(GameState gs, string relicId)
    {
        if (upgradeSelectedRelicButton == null)
            return;

        upgradeSelectedRelicButton.gameObject.SetActive(relicChamberPanelOpen);

        if (gs == null || string.IsNullOrEmpty(relicId))
        {
            upgradeSelectedRelicButton.interactable = false;
            SetButtonText(upgradeSelectedRelicButton, "Mejorar");
            return;
        }

        if (!gs.IsD1RelicUnlocked(relicId))
        {
            upgradeSelectedRelicButton.interactable = false;
            SetButtonText(upgradeSelectedRelicButton, "Bloqueada");
            return;
        }

        if (!Dimension1System.TryGetNextDimension1RelicUpgradeCost(
            gs,
            relicId,
            out _,
            out _,
            out _,
            out _,
            out _,
            out _,
            out _
        ))
        {
            upgradeSelectedRelicButton.interactable = false;
            SetButtonText(upgradeSelectedRelicButton, "Maximo");
            return;
        }

        upgradeSelectedRelicButton.interactable =
            Dimension1System.CanUpgradeDimension1Relic(gs, relicId);

        string text = "Mejorar";

        if (!upgradeSelectedRelicButton.interactable)
            text += "\nRecursos insuficientes";

        SetButtonText(upgradeSelectedRelicButton, text);
    }

    private string GetRelicEffectText(
        string relicId,
        double primaryBonus,
        double secondaryBonus
    )
    {
        switch (relicId)
        {
            case Dimension1System.RelicDriftCompass:
                return
                    "- Duracion de exploraciones: -" +
                    FormatRelicPercent(primaryBonus) +
                    "\n- Reduccion adicional en exploraciones largas: -" +
                    FormatRelicPercent(secondaryBonus);

            case Dimension1System.RelicAncientCargoCore:
                return
                    "- Metales obtenidos en exploracion: +" +
                    FormatRelicPercent(primaryBonus) +
                    "\n- Bonus adicional en destinos largos: +" +
                    FormatRelicPercent(secondaryBonus);

            case Dimension1System.RelicLostNavigationRecord:
                return
                    "- Probabilidad de destinos raros/especiales: +" +
                    FormatRelicPercentagePoints(primaryBonus) +
                    "\n- Peso de destinos comunes repetidos: -" +
                    FormatRelicPercent(secondaryBonus);

            case Dimension1System.RelicDormantEcho:
                return
                    "- Probabilidad de encontrar reliquias: +" +
                    FormatRelicPercentagePoints(primaryBonus) +
                    "\n- Prioridad de reliquias aun no obtenidas: +" +
                    FormatRelicPercentagePoints(secondaryBonus);

            case Dimension1System.RelicExplorerPlate:
                return
                    "- Materiales basicos compatibles con Sonda Ligera: +" +
                    FormatRelicPercent(primaryBonus) +
                    "\n- Conservacion en destinos medios: +" +
                    FormatRelicPercent(secondaryBonus);

            case Dimension1System.RelicExtractionHook:
                return
                    "- Materiales con Dron Extractor: +" +
                    FormatRelicPercent(primaryBonus) +
                    "\n- Probabilidad de recurso secundario compatible: +" +
                    FormatRelicPercentagePoints(secondaryBonus);

            case Dimension1System.RelicAnalyticCrystal:
                return
                    "- Fragmentos y matrices de Carga con Sonda Analitica: +" +
                    FormatRelicPercentagePoints(primaryBonus) +
                    "\n- Reliquias en destinos de investigacion: +" +
                    FormatRelicPercentagePoints(secondaryBonus);

            case Dimension1System.RelicModularContainer:
                return
                    "- Materiales con Nave de Carga: +" +
                    FormatRelicPercent(primaryBonus) +
                    "\n- Bonus adicional en destinos largos: +" +
                    FormatRelicPercent(secondaryBonus);

            case Dimension1System.RelicAncientDrill:
                return
                    "- Produccion minera general: +" +
                    FormatRelicPercent(primaryBonus) +
                    "\n- Produccion del metal principal: +" +
                    FormatRelicPercent(secondaryBonus);

            case Dimension1System.RelicRememberedAlloy:
                return
                    "- Reduccion de costos de extractores: -" +
                    FormatRelicPercent(primaryBonus) +
                    "\n- Reduccion adicional en tiers multiplos de 10: -" +
                    FormatRelicPercent(secondaryBonus);

            case Dimension1System.RelicProspectingCore:
                return
                    "- Produccion de recursos secundarios: +" +
                    FormatRelicPercent(primaryBonus) +
                    "\n- Bonus adicional en planetas avanzados: +" +
                    FormatRelicPercent(secondaryBonus);

            case Dimension1System.RelicExtractionSeal:
                return
                    "- Produccion con extractores tier 20+: +" +
                    FormatRelicPercent(primaryBonus) +
                    "\n- Produccion en planetas con varios recursos: +" +
                    FormatRelicPercent(secondaryBonus);

            case Dimension1System.RelicFracturedAntenna:
                return
                    "- Probabilidad de obtener un destino adicional al escanear: +" +
                    FormatRelicPercentagePoints(primaryBonus) +
                    "\n- Bonus secundario: pendiente de definicion.";

            case Dimension1System.RelicMatrixArchive:
                return
                    "- Probabilidad de Fragmento de Matriz Adaptativa: +" +
                    FormatRelicPercentagePoints(primaryBonus) +
                    "\n- Probabilidad de Matriz especifica compatible: +" +
                    FormatRelicPercentagePoints(secondaryBonus);

            case Dimension1System.RelicRoom1Echo:
                return
                    "- Produccion general de LE: +" +
                    FormatRelicPercent(primaryBonus) +
                    "\n- LE de artefactos del Cuarto 1: +" +
                    FormatRelicPercent(secondaryBonus);

            case Dimension1System.RelicIncompleteStarMap:
                return BuildPendingRelicEffectText(
                    "variedad de destinos"
                );

            case Dimension1System.RelicTracesResonator:
                return BuildPendingRelicEffectText("Trazas");

            case Dimension1System.RelicCalibrationFragment:
                return BuildPendingRelicEffectText(
                    "Modulador de Fase"
                );

            case Dimension1System.RelicRareFrequencySensor:
                return BuildPendingRelicEffectText(
                    "puntos especiales"
                );

            case Dimension1System.RelicTriangularSeal:
                return BuildPendingRelicEffectText("Triangulo");

            case Dimension1System.RelicMachineMemory:
                return BuildPendingRelicEffectText(
                    "Maquina / Cuarto 2"
                );

            default:
                return "Efecto no disponible.";
        }
    }

    private string BuildPendingRelicEffectText(string intendedImpact)
    {
        return
            "- Impacto previsto: " +
            intendedImpact +
            ".\n- Valores numericos pendientes de definicion; no aplica bonus por ahora.";
    }

    private string FormatRelicPercent(double value)
    {
        double percent = value * 100.0;

        return percent.ToString(
            percent >= 1.0 ? "0.##" : "0.###"
        ) + "%";
    }

    private string FormatRelicPercentagePoints(double value)
    {
        double points = value * 100.0;

        return points.ToString(
            points >= 1.0 ? "0.##" : "0.###"
        ) + " puntos porcentuales";
    }

    public void OnClickOpenDimension1TreePanel()
    {
        if (dimension1TreePanel == null)
            return;

        dimension1TreePanelOpen = true;
        hangarPanelOpen = false;
        relicChamberPanelOpen = false;
        galaxyPanelOpen = false;
        explorationRecordPanelOpen = false;
        showingExplorationResultPanel = false;
        RefreshUI();
    }

    public void OnClickCloseDimension1TreePanel()
    {
        dimension1TreePanelOpen = false;
        RefreshUI();
    }

    public void OnDimension1TreeNodeDropdownChanged(int index)
    {
        if (isRefreshingDimension1TreeDropdown)
            return;

        selectedDimension1TreeNodeIndex = index;
        RefreshUI();
    }

    public void OnClickBuySelectedDimension1TreeNode()
    {
        GameState gs = GameState.I;
        string nodeId = GetSelectedDimension1TreeNodeId();

        if (gs == null || string.IsNullOrEmpty(nodeId))
            return;

        Dimension1System.TryBuyDimension1TreeNode(gs, nodeId);

        if (SaveService.I != null)
            SaveService.I.Save();

        dimension1TreeDropdownSignature = "";
        RefreshUI();
    }

    private void RefreshDimension1TreePanel(GameState gs)
    {
        bool anotherPanelOpen =
            hangarPanelOpen ||
            relicChamberPanelOpen ||
            galaxyPanelOpen;

        if (dimension1MainContentRoot != null)
        {
            dimension1MainContentRoot.SetActive(
                !anotherPanelOpen &&
                !dimension1TreePanelOpen
            );
        }

        if (openDimension1TreePanelButton != null)
        {
            openDimension1TreePanelButton.gameObject.SetActive(
                dimension1TreePanel != null &&
                !anotherPanelOpen &&
                !dimension1TreePanelOpen
            );
            SetButtonText(openDimension1TreePanelButton, "Árbol D1");
        }

        if (dimension1TreePanel != null)
            dimension1TreePanel.SetActive(dimension1TreePanelOpen);

        if (closeDimension1TreePanelButton != null)
        {
            closeDimension1TreePanelButton.gameObject.SetActive(
                dimension1TreePanelOpen
            );
            SetButtonText(closeDimension1TreePanelButton, "Cerrar");
        }

        if (!dimension1TreePanelOpen)
        {
            if (dimension1TreeInfoText != null)
                dimension1TreeInfoText.text = "";

            if (buySelectedDimension1TreeNodeButton != null)
                buySelectedDimension1TreeNodeButton.gameObject.SetActive(false);

            return;
        }

        RefreshDimension1TreeDropdown(gs);

        string nodeId = GetSelectedDimension1TreeNodeId();

        if (dimension1TreeInfoText != null)
        {
            dimension1TreeInfoText.text =
                BuildDimension1TreeNodeInfoText(gs, nodeId);
        }

        RefreshDimension1TreeBuyButton(gs, nodeId);
    }

    private void RefreshDimension1TreeDropdown(GameState gs)
    {
        if (dimension1TreeNodeDropdown == null)
            return;

        var options = new List<string>
        {
            "Elegir nodo"
        };

        foreach (string nodeId in Dimension1System.Dimension1TreeNodeIds)
        {
            int tier = gs != null ? gs.GetD1TreeNodeTier(nodeId) : 0;
            int maxTier = Dimension1System.GetDimension1TreeNodeMaxTier(nodeId);
            string option = Dimension1System.GetDimension1TreeNodeVisualName(nodeId);

            option += tier > 0
                ? " " + tier + "/" + maxTier
                : " (sin comprar)";

            options.Add(option);
        }

        string signature = string.Join("|", options);

        if (signature != dimension1TreeDropdownSignature)
        {
            isRefreshingDimension1TreeDropdown = true;
            dimension1TreeNodeDropdown.ClearOptions();
            dimension1TreeNodeDropdown.AddOptions(options);
            dimension1TreeDropdownSignature = signature;
            isRefreshingDimension1TreeDropdown = false;
        }

        selectedDimension1TreeNodeIndex = Mathf.Clamp(
            selectedDimension1TreeNodeIndex,
            0,
            options.Count - 1
        );

        dimension1TreeNodeDropdown.SetValueWithoutNotify(
            selectedDimension1TreeNodeIndex
        );
        dimension1TreeNodeDropdown.RefreshShownValue();
    }

    private string GetSelectedDimension1TreeNodeId()
    {
        int nodeIndex = selectedDimension1TreeNodeIndex - 1;

        if (nodeIndex < 0 ||
            nodeIndex >= Dimension1System.Dimension1TreeNodeIds.Length)
        {
            return "";
        }

        return Dimension1System.Dimension1TreeNodeIds[nodeIndex];
    }

    private string BuildDimension1TreeNodeInfoText(
        GameState gs,
        string nodeId
    )
    {
        if (gs == null)
            return "Árbol D1 no disponible.";

        string header =
            "ÁRBOL D1\n" +
            "Puntos disponibles: " +
            gs.prestige1Points +
            "\nNodos comprados: " +
            Dimension1System.GetD1PurchasedTreeNodeCount(gs) +
            "/" +
            Dimension1System.Dimension1TreeNodeIds.Length;

        if (string.IsNullOrEmpty(nodeId))
            return header + "\n\nSelecciona un nodo para revisar sus efectos.";

        int tier = gs.GetD1TreeNodeTier(nodeId);
        int maxTier = Dimension1System.GetDimension1TreeNodeMaxTier(nodeId);
        int targetTier = Mathf.Min(tier + 1, maxTier);

        string text =
            header +
            "\n\n" +
            Dimension1System.GetDimension1TreeNodeVisualName(nodeId) +
            "\nNivel: " +
            tier +
            "/" +
            maxTier +
            "\n\n" +
            Dimension1System.GetDimension1TreeNodeDescription(nodeId);

        if (tier >= maxTier)
            return text + "\n\nEstado: máximo alcanzado.";

        int cost = Dimension1System.GetDimension1TreeNodeCost(
            nodeId,
            targetTier
        );

        text +=
            "\n\nPróximo nivel: " +
            targetTier +
            "\nCosto: " +
            cost +
            " Puntos de Prestigio 1" +
            "\n" +
            Dimension1System.GetDimension1TreeNodeUnlockSummary(
                gs,
                nodeId,
                targetTier
            );

        return text;
    }

    private string BuildRelicCompatibleDestinationsText(string relicId)
    {
        string sectorId =
            Dimension1System.GetDimension1RelicSectorId(relicId);
        string[] destinations =
            Dimension1System.GetDimension1SectorDestinationIds(sectorId);
        List<string> compatibleNames = new List<string>();

        foreach (string destinationId in destinations)
        {
            if (!Dimension1System.IsDimension1RelicCompatibleDestination(
                    relicId,
                    destinationId
                ))
            {
                continue;
            }

            string destinationName = GetDestinationVisualName(destinationId);

            if (Dimension1System.IsDimension1RelicStrongDestination(
                    relicId,
                    destinationId
                ))
            {
                destinationName += " (fuerte)";
            }

            compatibleNames.Add(destinationName);
        }

        return compatibleNames.Count > 0
            ? string.Join(", ", compatibleNames)
            : "Sin destinos compatibles";
    }

    private void RefreshDimension1TreeBuyButton(GameState gs, string nodeId)
    {
        if (buySelectedDimension1TreeNodeButton == null)
            return;

        buySelectedDimension1TreeNodeButton.gameObject.SetActive(
            dimension1TreePanelOpen
        );

        if (gs == null || string.IsNullOrEmpty(nodeId))
        {
            buySelectedDimension1TreeNodeButton.interactable = false;
            SetButtonText(buySelectedDimension1TreeNodeButton, "Comprar");
            return;
        }

        int tier = gs.GetD1TreeNodeTier(nodeId);
        int maxTier = Dimension1System.GetDimension1TreeNodeMaxTier(nodeId);

        if (tier >= maxTier)
        {
            buySelectedDimension1TreeNodeButton.interactable = false;
            SetButtonText(buySelectedDimension1TreeNodeButton, "Máximo");
            return;
        }

        int targetTier = tier + 1;
        int cost = Dimension1System.GetDimension1TreeNodeCost(
            nodeId,
            targetTier
        );
        bool canBuy = Dimension1System.CanBuyDimension1TreeNode(gs, nodeId);

        buySelectedDimension1TreeNodeButton.interactable = canBuy;

        if (canBuy)
        {
            SetButtonText(
                buySelectedDimension1TreeNodeButton,
                "Comprar nivel " + targetTier + "\nCosto: " + cost
            );
            return;
        }

        string buttonText = gs.prestige1Points < cost
            ? "Faltan puntos " + gs.prestige1Points + "/" + cost
            : "Requisitos pendientes";
        SetButtonText(buySelectedDimension1TreeNodeButton, buttonText);
    }

    private void RefreshHangarPanel(GameState gs)
    {
        if (dimension1MainContentRoot != null)
        {
            dimension1MainContentRoot.SetActive(
                !hangarPanelOpen &&
                !relicChamberPanelOpen &&
                !dimension1TreePanelOpen &&
                !galaxyPanelOpen
            );
        }

        if (openHangarPanelButton != null)
        {
            openHangarPanelButton.gameObject.SetActive(
                !hangarPanelOpen &&
                !relicChamberPanelOpen &&
                !dimension1TreePanelOpen &&
                !galaxyPanelOpen
            );
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
        string maxLevelText = ToRomanNumber(Dimension1System.Dimension1ShipPartMaxLevel);

        return
            "Stats:\n" +
            "- Carga: " +
            FormatShipUpgradeLevel(GetShipPartLevelForUI(ship, Dimension1System.ShipPartCargo)) +
            "/" +
            maxLevelText +
            "\n" +
            "- Velocidad: " +
            FormatShipUpgradeLevel(GetShipPartLevelForUI(ship, Dimension1System.ShipPartSpeed)) +
            "/" +
            maxLevelText +
            "\n" +
            "- Blindaje: " +
            FormatShipUpgradeLevel(GetShipPartLevelForUI(ship, Dimension1System.ShipPartArmor)) +
            "/" +
            maxLevelText +
            "\n" +
            "- Sensores: " +
            FormatShipUpgradeLevel(GetShipPartLevelForUI(ship, Dimension1System.ShipPartSensors)) +
            "/" +
            maxLevelText;
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

        if (currentLevel >= Dimension1System.Dimension1ShipPartMaxLevel)
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
                return ClampShipPartLevelForD1Base(ship.cargoLevel);

            case Dimension1System.ShipPartSpeed:
                return ClampShipPartLevelForD1Base(ship.speedLevel);

            case Dimension1System.ShipPartArmor:
                return ClampShipPartLevelForD1Base(ship.armorLevel);

            case Dimension1System.ShipPartSensors:
                return ClampShipPartLevelForD1Base(ship.sensorsLevel);

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

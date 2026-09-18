using System;
using System.Text;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Dimension3PanelUI : MonoBehaviour
{
    [Header("Vistas")]
    public GameObject firstEntryRoot;
    public GameObject factoryRoot;

    [Header("Navegación")]
    public Button continueFirstEntryButton;
    public Button closeDimension3Button;

    [Header("Estado")]
    public D3ProductionFloorSkinUI productionFloorSkin;
    public TMP_Text factoryStatusText;
    public TMP_Text inventoryText;
    public TMP_Text queueText;
    public TMP_Text noticeText;
    public TMP_Text assignmentText;
    public TMP_Text powerText;
    public TMP_Text costPreviewText;

    [Header("Presentación progresiva P2")]
    public PresentationObjectiveCardUI objectiveCard;
    public GameObject coachmarkRoot;
    public TMP_Text coachmarkText;
    public Button contextualHelpButton;
    public GameObject helpRoot;
    public TMP_Text helpTitleText;
    public TMP_Text helpBodyText;
    public Button closeHelpButton;
    public GameObject firstCycleCompleteRoot;
    public TMP_Text firstCycleCompleteText;
    public Button continueFirstCycleButton;
    public Button replayFirstCycleButton;
    public Button closeFirstCycleButton;

    [Header("Producción V1")]
    public TMP_Text productionTitleText;
    public Button produceChassisButton;
    public Button produceMotorButton;
    public Button produceToolButton;
    public Button produceControlButton;
    public Button produceRegulatorButton;
    public TMP_Dropdown productionVersionDropdown;
    public TMP_Dropdown productionQuantityDropdown;

    [Header("Ensamblaje")]
    public Button assembleMk1Button;
    public TMP_Dropdown assemblyMkDropdown;
    public TMP_Dropdown assemblyQuantityDropdown;
    public Button cancelPartJobButton;
    public Button cancelAssemblyJobButton;

    [Header("Banco de Procesos")]
    public TMP_Dropdown assignmentMkDropdown;
    public TMP_Dropdown assignmentTraitDropdown;
    public TMP_Dropdown assignmentChannelDropdown;
    public Button addAssignmentButton;
    public Button removeAssignmentButton;
    public Button upgradeProcessBankButton;
    public Button openCalibrationButton;
    public D3CalibrationPanelUI calibrationPanel;
    public Button openResearchButton;
    public D3ResearchPanelUI researchPanel;
    public Button openFacilitiesButton;
    public D3FacilitiesPanelUI facilitiesPanel;
    public D3AutomationPanelUI automationPanel;
    public Button openQueuesButton;
    public D3QueuesPanelUI queuesPanel;

    private const float RefreshIntervalSeconds = 0.2f;
    private const float ActiveCancelConfirmationSeconds = 5f;
    private float _refreshRemaining;
    private string _pendingCancelQueueId = "";
    private float _pendingCancelUntil;
    private readonly SafeDropdownOptionMap<int> _productionVersionOptions =
        new SafeDropdownOptionMap<int>();
    private readonly SafeDropdownOptionMap<int> _assemblyMkOptions =
        new SafeDropdownOptionMap<int>();
    private readonly SafeDropdownOptionMap<int> _assignmentMkOptions =
        new SafeDropdownOptionMap<int>();
    private readonly SafeDropdownOptionMap<string> _assignmentTraitOptions =
        new SafeDropdownOptionMap<string>(StringComparer.Ordinal);
    private readonly SafeDropdownOptionMap<string> _assignmentChannelOptions =
        new SafeDropdownOptionMap<string>(StringComparer.Ordinal);
    private int _lastRenderedOnboardingStage = -1;
    private int _configuredDropdownMode = -1;
    private bool _showingFirstCycleReview;

    private void Awake()
    {
        AddListener(continueFirstEntryButton, ContinueFirstEntry);
        AddListener(closeDimension3Button, CloseDimension3);
        AddListener(produceChassisButton,
            () => QueuePart(Dimension3Catalog.PartChassis));
        AddListener(produceMotorButton,
            () => QueuePart(Dimension3Catalog.PartMotor));
        AddListener(produceToolButton,
            () => QueuePart(Dimension3Catalog.PartTool));
        AddListener(produceControlButton,
            () => QueuePart(Dimension3Catalog.PartControl));
        AddListener(produceRegulatorButton,
            () => QueuePart(Dimension3Catalog.PartRegulator));
        AddListener(assembleMk1Button, QueueMk1Assembly);
        AddListener(cancelPartJobButton,
            () => CancelLastJob(Dimension3Catalog.QueuePartProduction));
        AddListener(cancelAssemblyJobButton,
            () => CancelLastJob(Dimension3Catalog.QueueAssembly));
        AddListener(addAssignmentButton, () => ChangeAssignment(1L));
        AddListener(removeAssignmentButton, () => ChangeAssignment(-1L));
        AddListener(upgradeProcessBankButton, UpgradeProcessBank);
        AddListener(openCalibrationButton, OpenCalibration);
        AddListener(openResearchButton, OpenResearch);
        AddListener(openFacilitiesButton, OpenFacilities);
        AddListener(openQueuesButton, OpenQueues);
        AddListener(contextualHelpButton, OpenContextualHelp);
        AddListener(closeHelpButton, CloseContextualHelp);
        AddListener(continueFirstCycleButton, ContinueAfterFirstCycle);
        AddListener(replayFirstCycleButton, OpenFirstCycleReview);
        AddListener(closeFirstCycleButton, CloseDimension3);
        ConfigureDropdowns();
    }

    private void OpenCalibration()
    {
        D3PresentationRouter.RememberScreen(GameState.I, PresentationFeatureIds.D3Calibration);
        if (calibrationPanel != null) calibrationPanel.Open();
    }

    private void OpenResearch()
    {
        D3PresentationRouter.RememberScreen(GameState.I, PresentationFeatureIds.D3Research);
        if (researchPanel != null) researchPanel.Open();
    }

    private void OpenFacilities()
    {
        D3PresentationRouter.RememberScreen(GameState.I, PresentationFeatureIds.D3Facilities);
        if (facilitiesPanel != null) facilitiesPanel.Open();
    }

    private void OpenQueues()
    {
        D3PresentationRouter.RememberScreen(GameState.I, PresentationFeatureIds.D3QueuesFull);
        if (queuesPanel != null) queuesPanel.Open();
    }

    private void OnEnable()
    {
        if (TabsUI.Instance != null && TabsUI.Instance.verticalNavigation != null)
            TabsUI.Instance.verticalNavigation.SetNavigationSuppressed(true, this);
        OpenFromTab();
    }

    private void OnDisable()
    {
        if (TabsUI.Instance != null && TabsUI.Instance.verticalNavigation != null)
            TabsUI.Instance.verticalNavigation.SetNavigationSuppressed(false, this);
    }

    private void Update()
    {
        _refreshRemaining -= Time.unscaledDeltaTime;
        if (_refreshRemaining > 0f)
            return;

        _refreshRemaining = RefreshIntervalSeconds;
        Refresh();
    }

    public void OpenFromTab()
    {
        if (!Dimension3System.CanAccessDimension3(GameState.I))
            return;

        GameState.I.EnsureDimension3State();
        if (calibrationPanel != null)
            calibrationPanel.gameObject.SetActive(false);
        if (researchPanel != null)
            researchPanel.gameObject.SetActive(false);
        if (facilitiesPanel != null)
            facilitiesPanel.gameObject.SetActive(false);
        if (automationPanel != null)
            automationPanel.gameObject.SetActive(false);
        if (queuesPanel != null)
            queuesPanel.gameObject.SetActive(false);
        if (facilitiesPanel != null && facilitiesPanel.consolePanel != null)
            facilitiesPanel.consolePanel.gameObject.SetActive(false);
        CloseContextualHelp();
        OpenResolvedScreen(D3PresentationRouter.ResolveInitialScreen(GameState.I));
        Refresh();
    }

    public void ContinueFirstEntry()
    {
        Dimension3System.MarkFirstEntrySeen(GameState.I);
        GameState.I.dimension3.presentation.onboardingStage = Math.Max(
            1, GameState.I.dimension3.presentation.onboardingStage);
        PresentationStateUtility.Acknowledge(
            GameState.I.dimension3.presentation, PresentationFeatureIds.D3Factory);
        D3PresentationRouter.RememberScreen(GameState.I, PresentationFeatureIds.D3Factory);
        SetView(false, true);
        SetNotice(T("d3.onboarding.notice.factory_ready"));
        Refresh();
    }

    private void ContinueAfterFirstCycle()
    {
        if (GameState.I == null || GameState.I.dimension3 == null) return;
        GameState.I.dimension3.presentation.onboardingStage =
            (int)D3OnboardingStage.Completed;
        PresentationStateUtility.Acknowledge(
            GameState.I.dimension3.presentation,
            PresentationFeatureIds.D3AssemblyNormal);
        D3PresentationRouter.RememberScreen(GameState.I, PresentationFeatureIds.D3Factory);
        CloseContextualHelp();
        SetNotice(T("d3.onboarding.notice.cycle_continue"));
        Refresh();
    }

    private void OpenFirstCycleReview()
    {
        _showingFirstCycleReview = true;
        if (helpTitleText != null)
            helpTitleText.text = T("d3.onboarding.help.review_title");
        if (helpBodyText != null)
            helpBodyText.text = T("d3.onboarding.help.review_body");
        if (helpRoot != null)
        {
            helpRoot.transform.SetAsLastSibling();
            helpRoot.SetActive(true);
        }
    }

    private void OpenContextualHelp()
    {
        if (GameState.I == null || GameState.I.dimension3 == null) return;
        D3OnboardingSnapshot snapshot = D3OnboardingRules.Synchronize(GameState.I);
        _showingFirstCycleReview = false;
        if (helpTitleText != null) helpTitleText.text = T("d3.onboarding.help.title");
        if (helpBodyText != null) helpBodyText.text = GetContextualHelp(snapshot.stage);
        if (helpRoot != null)
        {
            helpRoot.transform.SetAsLastSibling();
            helpRoot.SetActive(true);
        }
    }

    private void CloseContextualHelp()
    {
        _showingFirstCycleReview = false;
        if (helpRoot != null) helpRoot.SetActive(false);
    }

    public void CloseDimension3()
    {
        if (TabsUI.Instance != null)
            TabsUI.Instance.ShowGeneracion();
    }

    private void OpenResolvedScreen(string screenId)
    {
        bool firstEntry = screenId == D3PresentationRouter.FirstEntryScreenId;
        SetView(firstEntry, !firstEntry);
        if (firstEntry) return;

        D3PresentationRouter.RememberScreen(GameState.I, screenId);
        if (screenId == PresentationFeatureIds.D3Calibration)
        {
            if (calibrationPanel != null) calibrationPanel.Open();
        }
        else if (screenId == PresentationFeatureIds.D3Research)
        {
            if (researchPanel != null) researchPanel.Open();
        }
        else if (screenId == PresentationFeatureIds.D3Facilities ||
                 screenId == PresentationFeatureIds.D3FacilityConsole ||
                 screenId == PresentationFeatureIds.D3FacilityDiagnostic ||
                 screenId == PresentationFeatureIds.D3FacilityPort ||
                 screenId == PresentationFeatureIds.D3FacilityCore)
        {
            if (facilitiesPanel != null) facilitiesPanel.Open();
        }
        else if (screenId == PresentationFeatureIds.D3Automation)
        {
            if (automationPanel != null) automationPanel.Open();
        }
        else if (screenId == PresentationFeatureIds.D3QueuesCompact ||
                 screenId == PresentationFeatureIds.D3QueuesFull)
        {
            if (queuesPanel != null) queuesPanel.Open();
        }
    }

    public void Refresh()
    {
        GameState gameState = GameState.I;
        if (!Dimension3System.CanAccessDimension3(gameState))
            return;

        Dimension3System.EnsureState(gameState);
        D3OnboardingSnapshot onboarding = D3OnboardingRules.Synchronize(gameState);
        if (productionFloorSkin != null)
            productionFloorSkin.Refresh(gameState);
        RefreshStatus(gameState);
        RefreshInventory(gameState.dimension3);
        RefreshQueues(gameState.dimension3);
        RefreshAssignments(gameState.dimension3);
        RefreshPower(gameState.dimension3);
        RefreshCostPreview(gameState);
        RefreshButtons(gameState);
        RefreshOnboarding(gameState, onboarding);
        if (productionFloorSkin != null)
            productionFloorSkin.RefreshControls();

        if (!string.IsNullOrEmpty(_pendingCancelQueueId) &&
            Time.unscaledTime > _pendingCancelUntil)
        {
            _pendingCancelQueueId = "";
        }
    }

    private void QueuePart(string partId)
    {
        D3OnboardingSnapshot onboarding = D3OnboardingRules.Evaluate(GameState.I);
        bool tutorial = onboarding.stage < D3OnboardingStage.Completed;
        int version = tutorial ? 1 : GetSelectedProductionVersion();
        long quantity = tutorial ? 1L : GetSelectedProductionQuantity();
        string reason;
        if (Dimension3System.TryQueuePartProduction(
                GameState.I,
                partId,
                version,
                quantity,
                out reason))
        {
            PresentationStateUtility.Acknowledge(
                GameState.I.dimension3.presentation,
                PresentationFeatureIds.D3ProductionFirstPart);
            SetNotice(tutorial
                ? TF("d3.onboarding.notice.part_queued", GetPartDisplayName(partId))
                : GetPartDisplayName(partId) + " añadido a Producción.");
        }
        else
        {
            SetNotice(GetPartFailureMessage(partId, version, quantity));
        }

        Refresh();
    }

    private void QueueMk1Assembly()
    {
        D3OnboardingSnapshot onboarding = D3OnboardingRules.Evaluate(GameState.I);
        bool tutorial = onboarding.stage < D3OnboardingStage.Completed;
        string reason;
        int mk = tutorial ? 1 : GetSelectedAssemblyMk();
        long quantity = tutorial ? 1L : GetSelectedAssemblyQuantity();
        if (Dimension3System.TryQueueNormalAssembly(GameState.I, mk, quantity, out reason))
        {
            PresentationStateUtility.Acknowledge(
                GameState.I.dimension3.presentation,
                PresentationFeatureIds.D3AssemblyNormal);
            SetNotice(tutorial
                ? T("d3.onboarding.notice.assembly_started")
                : "MK" + mk + " Normal ×" + quantity +
                  " añadido a la cola de ensamblaje.");
        }
        else
            SetNotice(tutorial ? T("d3.onboarding.error.assembly") : reason);

        Refresh();
    }

    private void CancelLastJob(string queueId)
    {
        if (GameState.I == null || GameState.I.dimension3 == null)
            return;

        D3QueueState queue = D3JobQueueSystem.GetQueue(GameState.I.dimension3, queueId);
        if (queue == null || queue.jobs == null || queue.jobs.Count == 0)
        {
            SetNotice("No hay trabajos en esa cola.");
            return;
        }

        D3JobState job = queue.jobs[queue.jobs.Count - 1];
        if (job == null)
            return;

        if (job.started &&
            (_pendingCancelQueueId != queueId || Time.unscaledTime > _pendingCancelUntil))
        {
            _pendingCancelQueueId = queueId;
            _pendingCancelUntil = Time.unscaledTime + ActiveCancelConfirmationSeconds;
            SetNotice(
                "El trabajo está activo y no devolverá recursos. " +
                "Pulsa cancelar otra vez durante 5 segundos para confirmar."
            );
            return;
        }

        string reason;
        bool wasActive = job.started;
        if (Dimension3System.TryCancelJob(GameState.I, queueId, job.jobId, out reason))
        {
            SetNotice(wasActive
                ? "Trabajo activo cancelado sin devolución de consumibles."
                : "Trabajo pendiente cancelado con devolución completa.");
            _pendingCancelQueueId = "";
        }
        else
        {
            SetNotice(reason);
        }

        Refresh();
    }

    private void RefreshStatus(GameState gameState)
    {
        if (factoryStatusText == null)
            return;

        int bankLevel = D3FacilitySystem.GetProcessBankLevel(gameState.dimension3);
        factoryStatusText.text = productionFloorSkin != null
            ? "BANCO DE PROCESOS — NIVEL " + bankLevel
            : "BANCO DE PROCESOS — NIVEL " + bankLevel + "\n" +
              "LE: " + FormatNumber(gameState.LE) +
              "   |   Trazas: " + FormatNumber(gameState.Traces) +
              "\nEnsamblados: MK1 " + D3InventorySystem.GetAssemblyCount(gameState.dimension3, 1) +
              " | MK2 " + D3InventorySystem.GetAssemblyCount(gameState.dimension3, 2) +
              " | MK3 " + D3InventorySystem.GetAssemblyCount(gameState.dimension3, 3) + ".";
    }

    private void RefreshInventory(Dimension3State state)
    {
        if (inventoryText == null)
            return;

        int version = GetSelectedProductionVersion();
        if (productionFloorSkin != null)
        {
            inventoryText.text = "";
            productionFloorSkin.RefreshProductionInventory(state, version);
            return;
        }

        var builder = new StringBuilder();
        builder.AppendLine("INVENTARIO V" + version);
        for (int i = 0; i < Dimension3Catalog.PartIds.Length; i++)
        {
            string partId = Dimension3Catalog.PartIds[i];
            builder.Append(GetPartDisplayName(partId));
            builder.Append(": ");
            builder.AppendLine(D3InventorySystem.GetPartAmount(state, partId, version).ToString());
        }

        int mk = GetSelectedAssignmentMk();
        string traitId = GetSelectedTraitId();
        builder.Append("\nMK" + mk + " " + GetTraitDisplayName(traitId) + ": ");
        builder.Append(D3InventorySystem.GetAutomatonAmount(
            state,
            mk,
            traitId
        ));
        builder.Append(" (libres: ");
        builder.Append(D3InventorySystem.GetAvailableAutomatonAmount(state, mk, traitId));
        builder.Append(")");
        inventoryText.text = builder.ToString();
    }

    private void RefreshQueues(Dimension3State state)
    {
        if (queueText == null)
            return;

        if (productionFloorSkin != null)
        {
            queueText.text = "";
            return;
        }

        var builder = new StringBuilder();
        AppendQueue(builder, state, Dimension3Catalog.QueuePartProduction, "PRODUCCIÓN");
        builder.AppendLine();
        AppendQueue(builder, state, Dimension3Catalog.QueueAssembly, "ENSAMBLAJE");
        builder.AppendLine();
        AppendQueue(builder, state, Dimension3Catalog.QueueFacility, "MEJORAS");
        queueText.text = builder.ToString();
    }

    private void RefreshButtons(GameState gameState)
    {
        bool canUse = Dimension3System.CanAccessDimension3(gameState);
        int version = GetSelectedProductionVersion();
        int assemblyMk = GetSelectedAssemblyMk();
        long productionQuantity = GetSelectedProductionQuantity();
        long assemblyQuantity = GetSelectedAssemblyQuantity();
        D3CostTimeDefinition partDefinition = Dimension3Catalog.GetPartDefinition(version);
        if (partDefinition != null)
        {
            double partLE = D3PowerSystem.GetModifiedCost(
                gameState.dimension3, partDefinition.leCost * productionQuantity);
            double partTraces = D3PowerSystem.GetModifiedCost(
                gameState.dimension3, partDefinition.tracesCost * productionQuantity);
            string suffix = productionFloorSkin == null
                ? " ×" + productionQuantity + " — " + FormatNumber(partLE) +
                  " LE + " + FormatNumber(partTraces) + " T"
                : "";
            SetButtonLabel(produceChassisButton, "CHASIS" + suffix);
            SetButtonLabel(produceMotorButton, "SISTEMA MOTRIZ" + suffix);
            SetButtonLabel(produceToolButton, "HERRAMIENTA" + suffix);
            SetButtonLabel(produceControlButton, "MÓDULO DE CONTROL" + suffix);
            SetButtonLabel(produceRegulatorButton, "REGULADOR" + suffix);
            if (productionFloorSkin != null && productionTitleText != null)
                productionTitleText.text = FormatNumber(partLE) +
                    " LE + " + FormatNumber(partTraces) + " T";
        }
        D3CostTimeDefinition assemblyDefinition =
            Dimension3Catalog.GetNormalAssemblyDefinition(assemblyMk);
        if (assemblyDefinition != null)
        {
            double assemblyLE = D3PowerSystem.GetModifiedCost(
                gameState.dimension3, assemblyDefinition.leCost * assemblyQuantity);
            double assemblyTraces = D3PowerSystem.GetModifiedCost(
                gameState.dimension3, assemblyDefinition.tracesCost * assemblyQuantity);
            SetButtonLabel(assembleMk1Button, productionFloorSkin == null
                ? "ENSAMBLAR MK" + assemblyMk + " NORMAL — " +
                  FormatNumber(assemblyLE) + " LE + " +
                  FormatNumber(assemblyTraces) + " T ×" + assemblyQuantity
                : "ENSAMBLAR MK" + assemblyMk + " NORMAL");
            if (productionFloorSkin != null &&
                productionFloorSkin.assemblyCostText != null)
                productionFloorSkin.assemblyCostText.text =
                    "<color=#D8C8AA>COSTE</color>    <color=#3DB8B1>" +
                    FormatNumber(assemblyLE) + " LE + " +
                    FormatNumber(assemblyTraces) + " T ×" + assemblyQuantity +
                    "</color>";
        }
        bool validProductionBatch = productionQuantity != 50L ||
            D3FacilitySystem.GetProcessBankLevel(gameState.dimension3) >= 5;
        bool partQueueAvailable = canUse && validProductionBatch &&
            D3JobQueueSystem.CanAcceptJob(
            gameState.dimension3,
            Dimension3Catalog.QueuePartProduction
        );
        SetInteractable(produceChassisButton, partQueueAvailable && D3ProductionSystem.IsPartVersionUnlocked(gameState, Dimension3Catalog.PartChassis, version));
        SetInteractable(produceMotorButton, partQueueAvailable && D3ProductionSystem.IsPartVersionUnlocked(gameState, Dimension3Catalog.PartMotor, version));
        SetInteractable(produceToolButton, partQueueAvailable && D3ProductionSystem.IsPartVersionUnlocked(gameState, Dimension3Catalog.PartTool, version));
        SetInteractable(produceControlButton, partQueueAvailable && D3ProductionSystem.IsPartVersionUnlocked(gameState, Dimension3Catalog.PartControl, version));
        SetInteractable(produceRegulatorButton, partQueueAvailable && D3ProductionSystem.IsPartVersionUnlocked(gameState, Dimension3Catalog.PartRegulator, version));

        bool canAssemble = canUse &&
            D3JobQueueSystem.CanAcceptJob(
                gameState.dimension3,
                Dimension3Catalog.QueueAssembly) &&
            D3AssemblySystem.IsNormalMkUnlocked(gameState, assemblyMk) &&
            D3InventorySystem.HasCompletePartSet(
                gameState.dimension3, assemblyMk, GetSelectedAssemblyQuantity());
        SetInteractable(assembleMk1Button, canAssemble);
        SetInteractable(cancelPartJobButton,
            D3JobQueueSystem.GetJobCount(
                gameState.dimension3,
                Dimension3Catalog.QueuePartProduction) > 0);
        SetInteractable(cancelAssemblyJobButton,
            D3JobQueueSystem.GetJobCount(
                gameState.dimension3,
                Dimension3Catalog.QueueAssembly) > 0);
        SetInteractable(openQueuesButton, canUse);
        string channelId = GetSelectedChannelId();
        bool channelUnlocked = D3FacilitySystem.IsProcessBankChannelUnlocked(
            gameState.dimension3, channelId);
        SetInteractable(addAssignmentButton, canUse && channelUnlocked &&
            D3InventorySystem.GetAvailableAutomatonAmount(
                gameState.dimension3, GetSelectedAssignmentMk(),
                GetSelectedTraitId()) > 0L);
        D3AssignmentState assignment = D3FacilitySystem.GetAssignment(
            gameState.dimension3, Dimension3Catalog.FacilityProcessBank,
            channelId, GetSelectedAssignmentMk(), GetSelectedTraitId());
        SetInteractable(removeAssignmentButton, assignment != null && assignment.amount > 0L);
        SetInteractable(upgradeProcessBankButton,
            D3FacilitySystem.GetProcessBankLevel(gameState.dimension3) < 5 &&
            D3JobQueueSystem.GetJobCount(
                gameState.dimension3, Dimension3Catalog.QueueFacility) == 0);
        int targetLevel = D3FacilitySystem.GetProcessBankLevel(gameState.dimension3) + 1;
        D3FacilityLevelDefinition upgrade =
            Dimension3Catalog.GetProcessBankLevelDefinition(targetLevel);
        SetButtonLabel(upgradeProcessBankButton, upgrade == null
            ? "BANCO AL MÁXIMO DEL BLOQUE 2"
            : "SUBIR BANCO A N" + targetLevel + "\n" +
              FormatNumber(D3PowerSystem.GetModifiedCost(
                  gameState.dimension3, upgrade.leCost)) + " LE + " +
              FormatNumber(D3PowerSystem.GetModifiedCost(
                  gameState.dimension3, upgrade.tracesCost)) + " T");
    }

    private void ChangeAssignment(long delta)
    {
        if (GameState.I == null || GameState.I.dimension3 == null) return;
        string channelId = GetSelectedChannelId();
        int mk = GetSelectedAssignmentMk();
        string traitId = GetSelectedTraitId();
        D3AssignmentState assignment = D3FacilitySystem.GetAssignment(
            GameState.I.dimension3, Dimension3Catalog.FacilityProcessBank,
            channelId, mk, traitId);
        long current = assignment == null ? 0L : assignment.amount;
        long target = Math.Max(0L, current + delta);
        string reason;
        if (Dimension3System.TrySetProcessBankAssignment(
                GameState.I, channelId, mk, traitId, target, out reason))
        {
            if (delta > 0L)
            {
                PresentationStateUtility.Acknowledge(
                    GameState.I.dimension3.presentation,
                    PresentationFeatureIds.D3AssignmentBasic);
                SetNotice(T("d3.onboarding.notice.assigned"));
            }
            else
                SetNotice(T("d3.onboarding.notice.removed"));
        }
        else SetNotice(T("d3.onboarding.error.assignment"));
        Refresh();
    }

    private void UpgradeProcessBank()
    {
        string reason;
        if (Dimension3System.TryQueueProcessBankUpgrade(GameState.I, out reason))
            SetNotice(T("d3.onboarding.notice.upgrade_queued"));
        else SetNotice(T("d3.onboarding.error.upgrade"));
        Refresh();
    }

    private void RefreshAssignments(Dimension3State state)
    {
        if (assignmentText == null) return;
        string channelId = GetSelectedChannelId();
        int mk = GetSelectedAssignmentMk();
        string traitId = GetSelectedTraitId();
        D3AssignmentState assignment = D3FacilitySystem.GetAssignment(
            state, Dimension3Catalog.FacilityProcessBank, channelId, mk, traitId);
        long assigned = assignment == null ? 0L : assignment.amount;
        long stable = assignment == null ? 0L : assignment.stabilizedAmount;
        double remaining = assignment == null ? 0.0 :
            assignment.stabilizationRemainingSeconds;
        assignmentText.text =
            "ASIGNACIÓN SELECCIONADA\n" + GetChannelDisplayName(channelId) +
            " | MK" + mk + " " + GetTraitDisplayName(traitId) +
            "\nAsignados: " + assigned + " | Estables: " + stable +
            (remaining > 0.0 ? " | " + Math.Ceiling(remaining) + " s" : "");
    }

    private void RefreshPower(Dimension3State state)
    {
        if (powerText == null) return;
        D3ProcessModifiers modifiers = D3PowerSystem.GetProcessBankModifiers(state);
        if (productionFloorSkin != null)
        {
            powerText.text =
                "BONIFICACIONES DEL BANCO\n" +
                "Progreso +" + modifiers.progressBonusPercent.ToString("0.##") +
                "% · Tiempo " + modifiers.timeBonusRaw.ToString("0.##") +
                " · Costo " + modifiers.costBonusRaw.ToString("0.##") +
                "\nCoordinación +" + modifiers.coordinationPercent.ToString("0.##") + "%";
            return;
        }
        powerText.text =
            "BONIFICACIONES DEL BANCO\n" +
            "Progreso: +" + modifiers.progressBonusPercent.ToString("0.##") + "%" +
            " | Tiempo bruto: " + modifiers.timeBonusRaw.ToString("0.##") +
            " | Costo bruto: " + modifiers.costBonusRaw.ToString("0.##") +
            " | Coordinación: +" + modifiers.coordinationPercent.ToString("0.##") + "%";
    }

    private void RefreshCostPreview(GameState gameState)
    {
        if (costPreviewText == null) return;
        D3FactoryCostPreview preview = D3FactoryPreviewSystem.GetAssemblyPreview(
            gameState, GetSelectedAssemblyMk(), GetSelectedAssemblyQuantity());
        if (preview == null)
        {
            costPreviewText.text = "";
            return;
        }
        if (productionFloorSkin == null)
        {
            costPreviewText.text = preview.ToDisplayText();
            return;
        }
        if (productionFloorSkin.previewTitleText != null)
            productionFloorSkin.previewTitleText.text =
                "PREVISIÓN MK ×" + preview.quantity;
        costPreviewText.text =
            "<color=#D8C8AA>PIEZAS FALTANTES:</color> " +
            preview.missingPartsTotal +
            "\n<color=#D8C8AA>PIEZAS:</color> " +
            FormatNumber(preview.missingPartsLE) + " LE + " +
            FormatNumber(preview.missingPartsTraces) + " T" +
            "\n<color=#D8C8AA>ENSAMBLE:</color> " +
            FormatNumber(preview.assemblyLE) + " LE + " +
            FormatNumber(preview.assemblyTraces) + " T" +
            "\n\n<color=#D8C8AA>TOTAL:</color> " +
            FormatNumber(preview.TotalLE) + " LE + " +
            FormatNumber(preview.TotalTraces) + " T" +
            "\n<color=#D8C8AA>≈</color> " +
            Math.Ceiling(preview.estimatedSeconds) + " s";
    }

    private void RefreshOnboarding(
        GameState gameState,
        D3OnboardingSnapshot snapshot)
    {
        if (snapshot == null) return;
        D3OnboardingStage stage = snapshot.stage;
        bool advanced = stage == D3OnboardingStage.Completed &&
            D3OnboardingRules.IsAdvancedState(gameState.dimension3);
        EnsureDropdownMode(advanced);

        bool assigning = stage == D3OnboardingStage.AssignInitial ||
            stage == D3OnboardingStage.AssignNew;
        bool completingSet = stage == D3OnboardingStage.CompleteSet;
        bool assembling = stage == D3OnboardingStage.FirstAssembly;
        bool celebration = stage == D3OnboardingStage.Celebration;
        bool completed = stage == D3OnboardingStage.Completed;
        int productionJobs = D3JobQueueSystem.GetJobCount(
            gameState.dimension3, Dimension3Catalog.QueuePartProduction);
        int assemblyJobs = D3JobQueueSystem.GetJobCount(
            gameState.dimension3, Dimension3Catalog.QueueAssembly);
        int totalJobs = 0;
        for (int i = 0; i < Dimension3Catalog.QueueIds.Length; i++)
            totalJobs += D3JobQueueSystem.GetJobCount(
                gameState.dimension3, Dimension3Catalog.QueueIds[i]);

        SetVisible(firstCycleCompleteRoot, celebration);
        if (firstCycleCompleteText != null && celebration)
        {
            double required = D3FacilitySystem.GetRequiredEffectiveCapacity(2);
            firstCycleCompleteText.text = TF("d3.onboarding.celebration",
                Math.Max(0L, snapshot.totalMk1 - snapshot.manufacturedMk1),
                snapshot.manufacturedMk1, snapshot.freeMk1, snapshot.assignedMk1,
                D3FacilitySystem.GetEffectiveCapacity(
                    gameState.dimension3,
                    Dimension3Catalog.FacilityProcessBank).ToString("0.##"),
                required.ToString("0.##"));
        }
        SetVisible(contextualHelpButton, productionFloorSkin == null && !celebration &&
            stage != D3OnboardingStage.Discovery);
        SetVisible(coachmarkRoot,
            stage == D3OnboardingStage.AssignInitial &&
            gameState.dimension3.presentation.contextualHelpEnabled);
        if (coachmarkText != null)
        {
            coachmarkText.text = T("d3.onboarding.coachmark");
        }

        SetVisible(factoryStatusText, !celebration);
        SetVisible(inventoryText, productionFloorSkin == null && !celebration &&
            (assigning || completingSet || assembling || completed));
        SetVisible(assignmentText, !celebration &&
            (assigning || snapshot.assignedMk1 > 0L || completed));
        SetVisible(assignmentMkDropdown, advanced);
        SetVisible(assignmentTraitDropdown, advanced);
        SetVisible(assignmentChannelDropdown, assigning || advanced);
        SetVisible(addAssignmentButton, completed);
        SetVisible(removeAssignmentButton, !celebration &&
            snapshot.assignedMk1 > 0L && stage >= D3OnboardingStage.FirstPart);

        bool showParts = completingSet || assembling || completed;
        SetVisible(productionTitleText, advanced);
        SetVisible(produceChassisButton, showParts);
        SetVisible(produceMotorButton, showParts);
        SetVisible(produceToolButton, showParts);
        SetVisible(produceControlButton, showParts);
        SetVisible(produceRegulatorButton, showParts);
        SetVisible(productionVersionDropdown, advanced);
        SetVisible(productionQuantityDropdown, advanced);

        SetVisible(assembleMk1Button, completed);
        SetVisible(assemblyMkDropdown, advanced);
        SetVisible(assemblyQuantityDropdown, advanced);
        SetVisible(cancelPartJobButton, !celebration && productionJobs > 0);
        SetVisible(cancelAssemblyJobButton, !celebration && assemblyJobs > 0);

        bool showQueueSummary = !celebration &&
            (totalJobs > 0 || assembling || completed);
        SetVisible(queueText, showQueueSummary);
        FeaturePresentationState queuesState = D3PresentationRules.GetFeatureState(
            gameState, PresentationFeatureIds.D3QueuesFull);
        SetVisible(openQueuesButton, !celebration &&
            ((assembling && snapshot.assemblyActive) || totalJobs > 1 ||
             (completed && queuesState.CanOpen)));

        SetVisible(upgradeProcessBankButton, completed &&
            D3FacilitySystem.GetProcessBankLevel(gameState.dimension3) < 5);
        SetVisible(powerText, advanced && productionFloorSkin == null);
        SetVisible(costPreviewText, assembling || completed);
        FeaturePresentationState calibrationState =
            D3PresentationRules.GetFeatureState(
                gameState, PresentationFeatureIds.D3Calibration);
        FeaturePresentationState researchState = D3PresentationRules.GetFeatureState(
            gameState, PresentationFeatureIds.D3Research);
        FeaturePresentationState facilitiesState = D3PresentationRules.GetFeatureState(
            gameState, PresentationFeatureIds.D3Facilities);
        SetVisible(openCalibrationButton, completed && calibrationState.IsVisible);
        SetInteractable(openCalibrationButton, calibrationState.CanOpen);
        SetButtonLabel(openCalibrationButton, T("d3.onboarding.nav.calibration"));
        SetVisible(openResearchButton, completed && researchState.IsVisible);
        SetInteractable(openResearchButton, researchState.CanOpen);
        SetButtonLabel(openResearchButton, researchState.CanOpen
            ? T("d3.onboarding.nav.research_available")
            : T("d3.onboarding.nav.research_soon"));
        SetVisible(openFacilitiesButton, completed && facilitiesState.IsVisible);
        SetInteractable(openFacilitiesButton, facilitiesState.CanOpen);
        SetButtonLabel(openFacilitiesButton, facilitiesState.CanOpen
            ? T("d3.onboarding.nav.facilities_available")
            : T("d3.onboarding.nav.facilities_soon"));

        if (!advanced)
        {
            RefreshOnboardingStatus(gameState, snapshot);
            RefreshOnboardingInventory(gameState.dimension3, snapshot);
            RefreshOnboardingQueues(gameState.dimension3, snapshot);
            RefreshFirstSetButtons(gameState, snapshot);
        }

        if (helpRoot != null && helpRoot.activeSelf)
        {
            if (_showingFirstCycleReview)
            {
                if (helpTitleText != null)
                    helpTitleText.text = T("d3.onboarding.help.review_title");
                if (helpBodyText != null)
                    helpBodyText.text = T("d3.onboarding.help.review_body");
            }
            else
            {
                if (helpTitleText != null)
                    helpTitleText.text = T("d3.onboarding.help.title");
                if (helpBodyText != null)
                    helpBodyText.text = GetContextualHelp(stage);
            }
        }

        if (advanced && objectiveCard != null)
            objectiveCard.Render(null, null);
        else
            RenderObjective(gameState, snapshot);
        if (_lastRenderedOnboardingStage >= 0 &&
            (int)stage > _lastRenderedOnboardingStage)
            SetStageTransitionNotice(stage, snapshot);
        _lastRenderedOnboardingStage = (int)stage;
    }

    private void RenderObjective(
        GameState gameState,
        D3OnboardingSnapshot snapshot)
    {
        if (objectiveCard == null) return;
        PresentationObjective objective = null;
        UnityEngine.Events.UnityAction action = null;
        Dimension3State d3 = gameState.dimension3;

        switch (snapshot.stage)
        {
            case D3OnboardingStage.AssignInitial:
                objective = new PresentationObjective
                {
                    nowTitle = T("d3.onboarding.assign_initial.title"),
                    nowBody = T("d3.onboarding.assign_initial.body"),
                    progress = TF("d3.onboarding.assign_initial.progress", snapshot.freeMk1),
                    primaryAction = T("d3.onboarding.assign_initial.action"),
                    nextTitle = T("d3.onboarding.shared.next"),
                    nextBody = T("d3.onboarding.assign_initial.next")
                };
                action = () => ChangeAssignment(1L);
                break;
            case D3OnboardingStage.FirstPart:
                D3CostTimeDefinition part = Dimension3Catalog.GetPartDefinition(1);
                long chassisPlanned = D3InventorySystem.GetPartAmount(
                    d3, Dimension3Catalog.PartChassis, 1) +
                    D3OnboardingRules.GetQueuedPartAmount(
                        d3, Dimension3Catalog.PartChassis, 1);
                objective = new PresentationObjective
                {
                    nowTitle = T("d3.onboarding.first_part.title"),
                    nowBody = T("d3.onboarding.first_part.body"),
                    progress = part == null ? "" :
                        TF("d3.onboarding.progress.cost_time",
                            FormatNumber(D3PowerSystem.GetModifiedCost(d3, part.leCost)),
                            FormatNumber(D3PowerSystem.GetModifiedCost(d3, part.tracesCost)),
                            Math.Ceiling(D3PowerSystem.GetModifiedDuration(
                                d3, part.durationSeconds))),
                    primaryAction = chassisPlanned > 0L
                        ? T("d3.onboarding.first_part.action_active")
                        : T("d3.onboarding.first_part.action"),
                    nextTitle = T("d3.onboarding.shared.next"),
                    nextBody = T("d3.onboarding.first_part.next")
                };
                if (chassisPlanned <= 0L)
                    action = () => QueuePart(Dimension3Catalog.PartChassis);
                break;
            case D3OnboardingStage.CompleteSet:
                objective = new PresentationObjective
                {
                    nowTitle = T("d3.onboarding.complete_set.title"),
                    nowBody = T("d3.onboarding.complete_set.body"),
                    progress = TF("d3.onboarding.complete_set.progress",
                        snapshot.completedPartTypes, snapshot.plannedPartTypes),
                    primaryAction = string.IsNullOrEmpty(snapshot.nextMissingPartId)
                        ? T("d3.onboarding.action.waiting")
                        : TF("d3.onboarding.action.produce_part", GetPartDisplayName(
                            snapshot.nextMissingPartId).ToUpperInvariant()),
                    nextTitle = T("d3.onboarding.shared.next"),
                    nextBody = T("d3.onboarding.complete_set.next")
                };
                if (!string.IsNullOrEmpty(snapshot.nextMissingPartId))
                {
                    string nextPartId = snapshot.nextMissingPartId;
                    action = () => QueuePart(nextPartId);
                }
                break;
            case D3OnboardingStage.FirstAssembly:
                D3CostTimeDefinition assembly =
                    Dimension3Catalog.GetNormalAssemblyDefinition(1);
                bool completeSet = D3InventorySystem.HasCompletePartSet(d3, 1, 1L);
                objective = new PresentationObjective
                {
                    nowTitle = completeSet || snapshot.assemblyActive
                        ? T("d3.onboarding.first_assembly.title")
                        : T("d3.onboarding.first_assembly.title_restock"),
                    nowBody = completeSet || snapshot.assemblyActive
                        ? T("d3.onboarding.first_assembly.body")
                        : T("d3.onboarding.first_assembly.body_restock"),
                    progress = snapshot.assemblyActive
                        ? GetActiveJobProgress(d3, Dimension3Catalog.QueueAssembly)
                        : !completeSet
                            ? TF("d3.onboarding.first_assembly.progress_set",
                                snapshot.completedPartTypes)
                            : assembly == null ? "" :
                            TF("d3.onboarding.progress.cost_time",
                                FormatNumber(D3PowerSystem.GetModifiedCost(
                                    d3, assembly.leCost)),
                                FormatNumber(D3PowerSystem.GetModifiedCost(
                                    d3, assembly.tracesCost)),
                                Math.Ceiling(D3PowerSystem.GetModifiedDuration(
                                    d3, assembly.durationSeconds))),
                    primaryAction = snapshot.assemblyActive
                        ? T("d3.onboarding.first_assembly.action_active")
                        : !completeSet
                            ? (string.IsNullOrEmpty(snapshot.nextMissingPartId)
                                ? T("d3.onboarding.action.waiting")
                                : TF("d3.onboarding.action.produce_part",
                                    GetPartDisplayName(snapshot.nextMissingPartId)
                                        .ToUpperInvariant()))
                            : T("d3.onboarding.first_assembly.action"),
                    nextTitle = T("d3.onboarding.shared.next"),
                    nextBody = T("d3.onboarding.first_assembly.next")
                };
                if (!snapshot.assemblyActive && completeSet)
                    action = QueueMk1Assembly;
                else if (!snapshot.assemblyActive && !completeSet &&
                    !string.IsNullOrEmpty(snapshot.nextMissingPartId))
                {
                    string nextPartId = snapshot.nextMissingPartId;
                    action = () => QueuePart(nextPartId);
                }
                break;
            case D3OnboardingStage.AssignNew:
                objective = new PresentationObjective
                {
                    nowTitle = T("d3.onboarding.assign_new.title"),
                    nowBody = T("d3.onboarding.assign_new.body"),
                    progress = TF("d3.onboarding.assign_new.progress",
                        snapshot.manufacturedMk1, snapshot.freeMk1,
                        snapshot.assignedMk1),
                    primaryAction = T("d3.onboarding.assign_new.action"),
                    nextTitle = T("d3.onboarding.shared.next"),
                    nextBody = T("d3.onboarding.assign_new.next")
                };
                action = () => ChangeAssignment(1L);
                break;
            case D3OnboardingStage.Completed:
                double required = D3FacilitySystem.GetRequiredEffectiveCapacity(2);
                double current = D3FacilitySystem.GetEffectiveCapacity(
                    d3, Dimension3Catalog.FacilityProcessBank);
                objective = new PresentationObjective
                {
                    nowTitle = T("d3.onboarding.completed.title"),
                    nowBody = T("d3.onboarding.completed.body"),
                    progress = TF("d3.onboarding.completed.progress",
                        current.ToString("0.##"), required.ToString("0.##")),
                    primaryAction = T("d3.onboarding.completed.action"),
                    nextTitle = T("d3.onboarding.shared.next"),
                    nextBody = T("d3.onboarding.completed.next")
                };
                action = UpgradeProcessBank;
                break;
        }

        objectiveCard.Render(objective, action);
    }

    private void RefreshOnboardingStatus(
        GameState gameState,
        D3OnboardingSnapshot snapshot)
    {
        if (factoryStatusText == null) return;
        factoryStatusText.text = TF("d3.onboarding.status",
            D3FacilitySystem.GetProcessBankLevel(gameState.dimension3),
            FormatNumber(gameState.LE), FormatNumber(gameState.Traces),
            snapshot.freeMk1);
    }

    private void RefreshOnboardingInventory(
        Dimension3State d3,
        D3OnboardingSnapshot snapshot)
    {
        if (inventoryText == null) return;
        if (snapshot.stage == D3OnboardingStage.CompleteSet ||
            snapshot.stage == D3OnboardingStage.FirstAssembly)
        {
            var builder = new StringBuilder();
            builder.AppendLine(TF("d3.onboarding.inventory.set",
                snapshot.completedPartTypes));
            for (int i = 0; i < Dimension3Catalog.PartIds.Length; i++)
            {
                string partId = Dimension3Catalog.PartIds[i];
                long inventory = D3InventorySystem.GetPartAmount(d3, partId, 1);
                long queued = D3OnboardingRules.GetQueuedPartAmount(d3, partId, 1);
                builder.Append(GetPartDisplayName(partId));
                builder.Append(inventory > 0L
                    ? T("d3.onboarding.inventory.owned")
                    : queued > 0L
                        ? T("d3.onboarding.inventory.queued")
                        : T("d3.onboarding.inventory.missing"));
                builder.AppendLine();
            }
            inventoryText.text = builder.ToString();
            return;
        }

        long initialGranted = Math.Max(0L, snapshot.totalMk1 - snapshot.manufacturedMk1);
        inventoryText.text = TF("d3.onboarding.inventory.summary",
            initialGranted, snapshot.manufacturedMk1, snapshot.totalMk1,
            snapshot.freeMk1, snapshot.assignedMk1);
    }

    private void RefreshOnboardingQueues(
        Dimension3State d3,
        D3OnboardingSnapshot snapshot)
    {
        if (queueText == null) return;
        var builder = new StringBuilder();
        AppendQueue(builder, d3, Dimension3Catalog.QueuePartProduction,
            T("d3.onboarding.queue.production"));
        if (snapshot.stage >= D3OnboardingStage.FirstAssembly)
        {
            builder.AppendLine();
            AppendQueue(builder, d3, Dimension3Catalog.QueueAssembly,
                T("d3.onboarding.queue.assembly"));
        }
        queueText.text = builder.ToString();
    }

    private void RefreshFirstSetButtons(
        GameState gameState,
        D3OnboardingSnapshot snapshot)
    {
        if (snapshot.stage != D3OnboardingStage.CompleteSet &&
            snapshot.stage != D3OnboardingStage.FirstAssembly)
            return;

        SetFirstSetPartButton(gameState, produceChassisButton,
            Dimension3Catalog.PartChassis);
        SetFirstSetPartButton(gameState, produceMotorButton,
            Dimension3Catalog.PartMotor);
        SetFirstSetPartButton(gameState, produceToolButton,
            Dimension3Catalog.PartTool);
        SetFirstSetPartButton(gameState, produceControlButton,
            Dimension3Catalog.PartControl);
        SetFirstSetPartButton(gameState, produceRegulatorButton,
            Dimension3Catalog.PartRegulator);
    }

    private void SetFirstSetPartButton(
        GameState gameState,
        Button button,
        string partId)
    {
        if (button == null) return;
        long inventory = D3InventorySystem.GetPartAmount(
            gameState.dimension3, partId, 1);
        long queued = D3OnboardingRules.GetQueuedPartAmount(
            gameState.dimension3, partId, 1);
        D3CostTimeDefinition definition = Dimension3Catalog.GetPartDefinition(1);
        string status = inventory > 0L
            ? T("d3.onboarding.inventory.owned")
            : queued > 0L ? T("d3.onboarding.inventory.queued") : "";
        if (definition != null)
        {
            SetButtonLabel(button, TF("d3.onboarding.part_button",
                GetPartDisplayName(partId).ToUpperInvariant(),
                FormatNumber(D3PowerSystem.GetModifiedCost(
                    gameState.dimension3, definition.leCost)),
                FormatNumber(D3PowerSystem.GetModifiedCost(
                    gameState.dimension3, definition.tracesCost)),
                Math.Ceiling(D3PowerSystem.GetModifiedDuration(
                    gameState.dimension3, definition.durationSeconds)), status));
        }
        if (inventory + queued > 0L) button.interactable = false;
    }

    private void EnsureDropdownMode(bool advanced)
    {
        int mode = advanced ? 1 : 0;
        int[] levels = advanced && GameState.I != null
            ? D3ProgressivePresentationRules.GetUnlockedPartVersions(GameState.I)
            : new[] { 1 };
        int[] assemblyLevels = advanced && GameState.I != null
            ? D3ProgressivePresentationRules.GetUnlockedAssemblyMks(GameState.I)
            : new[] { 1 };
        for (int i = 0; i < levels.Length; i++) mode = mode * 7 + levels[i];
        for (int i = 0; i < assemblyLevels.Length; i++)
            mode = mode * 7 + assemblyLevels[i];
        if (_configuredDropdownMode == mode) return;
        int productionVersion = ResolvePreferredDropdownId(
            _productionVersionOptions, productionVersionDropdown, 1);
        int assemblyMk = ResolvePreferredDropdownId(
            _assemblyMkOptions, assemblyMkDropdown, 1);
        int assignmentMk = ResolvePreferredDropdownId(
            _assignmentMkOptions, assignmentMkDropdown, 1);
        string traitId = ResolvePreferredDropdownId(
            _assignmentTraitOptions, assignmentTraitDropdown,
            Dimension3Catalog.TraitNormal);
        string channelId = ResolvePreferredDropdownId(
            _assignmentChannelOptions, assignmentChannelDropdown,
            Dimension3Catalog.ChannelProcessPower);
        _configuredDropdownMode = mode;

        _productionVersionOptions.Rebuild(
            productionVersionDropdown, levels, value => "V" + value,
            productionVersion);
        _assemblyMkOptions.Rebuild(
            assemblyMkDropdown, assemblyLevels, value => "MK" + value,
            assemblyMk);
        _assignmentMkOptions.Rebuild(
            assignmentMkDropdown, assemblyLevels, value => "MK" + value,
            assignmentMk);
        string[] traits = advanced
            ? Dimension3Catalog.TraitIds
            : new[] { Dimension3Catalog.TraitNormal };
        string[] channels = advanced
            ? Dimension3Catalog.ProcessBankChannelIds
            : new[] { Dimension3Catalog.ChannelProcessPower };
        _assignmentTraitOptions.Rebuild(
            assignmentTraitDropdown, traits, GetTraitDisplayName,
            traitId);
        _assignmentChannelOptions.Rebuild(
            assignmentChannelDropdown, channels, GetChannelDisplayName,
            channelId);
    }

    public static T ResolvePreferredDropdownId<T>(
        SafeDropdownOptionMap<T> options, TMP_Dropdown dropdown, T fallback)
    {
        if (options == null) return fallback;
        int index = dropdown == null ? 0 : dropdown.value;
        return options.ResolveOrDefault(index, fallback);
    }

    private void SetStageTransitionNotice(
        D3OnboardingStage stage,
        D3OnboardingSnapshot snapshot)
    {
        switch (stage)
        {
            case D3OnboardingStage.CompleteSet:
                SetNotice(T("d3.onboarding.notice.chassis_received"));
                break;
            case D3OnboardingStage.FirstAssembly:
                SetNotice(T("d3.onboarding.notice.set_complete"));
                break;
            case D3OnboardingStage.AssignNew:
                SetNotice(TF("d3.onboarding.notice.mk1_built",
                    snapshot.totalMk1, snapshot.freeMk1));
                break;
            case D3OnboardingStage.Celebration:
                SetNotice(T("d3.onboarding.notice.cycle_complete"));
                break;
        }
    }

    private string GetPartFailureMessage(
        string partId,
        int version,
        long quantity)
    {
        string partName = GetPartDisplayName(partId);
        if (GameState.I == null || GameState.I.dimension3 == null)
            return TF("d3.onboarding.error.part", partName);
        D3CostTimeDefinition definition = Dimension3Catalog.GetPartDefinition(version);
        if (definition == null) return TF("d3.onboarding.error.part", partName);
        double leCost = D3PowerSystem.GetModifiedCost(
            GameState.I.dimension3, definition.leCost * quantity);
        double tracesCost = D3PowerSystem.GetModifiedCost(
            GameState.I.dimension3, definition.tracesCost * quantity);
        if (GameState.I.LE + 0.000001 < leCost)
            return TF("d3.onboarding.error.part_le",
                FormatNumber(leCost - GameState.I.LE), partName);
        if (GameState.I.Traces + 0.000001 < tracesCost)
            return TF("d3.onboarding.error.part_traces",
                FormatNumber(tracesCost - GameState.I.Traces), partName);
        return TF("d3.onboarding.error.part", partName);
    }

    private static string GetActiveJobProgress(Dimension3State d3, string queueId)
    {
        D3QueueState queue = D3JobQueueSystem.GetQueue(d3, queueId);
        if (queue == null || queue.jobs == null || queue.jobs.Count == 0 ||
            queue.jobs[0] == null) return "";
        return TF("d3.onboarding.progress.active_time",
            Math.Ceiling(queue.jobs[0].remainingSeconds));
    }

    private static string GetContextualHelp(D3OnboardingStage stage)
    {
        switch (stage)
        {
            case D3OnboardingStage.AssignInitial:
                return T("d3.onboarding.help.assign_initial");
            case D3OnboardingStage.FirstPart:
                return T("d3.onboarding.help.first_part");
            case D3OnboardingStage.CompleteSet:
                return T("d3.onboarding.help.complete_set");
            case D3OnboardingStage.FirstAssembly:
                return T("d3.onboarding.help.first_assembly");
            case D3OnboardingStage.AssignNew:
                return T("d3.onboarding.help.assign_new");
            default:
                return T("d3.onboarding.help.completed");
        }
    }

    private static void SetVisible(Component component, bool visible)
    {
        if (component != null) component.gameObject.SetActive(visible);
    }

    private static void SetVisible(GameObject target, bool visible)
    {
        if (target != null) target.SetActive(visible);
    }

    private void ConfigureDropdowns()
    {
        int productionVersion = GetSelectedProductionVersion();
        int assemblyMk = GetSelectedAssemblyMk();
        int assignmentMk = GetSelectedAssignmentMk();
        string traitId = GetSelectedTraitId();
        string channelId = GetSelectedChannelId();
        int[] levels = { 1, 2, 3, 4, 5, 6 };
        _productionVersionOptions.Rebuild(
            productionVersionDropdown, levels, value => "V" + value, productionVersion);
        SetOptions(productionQuantityDropdown,
            new[] { "Cantidad 1", "Cantidad 5", "Cantidad 10", "Cantidad 25", "Cantidad 50" });
        _assemblyMkOptions.Rebuild(
            assemblyMkDropdown, levels, value => "MK" + value, assemblyMk);
        SetOptions(assemblyQuantityDropdown,
            new[] { "Cantidad 1", "Cantidad 5", "Cantidad 10", "Cantidad 25" });
        _assignmentMkOptions.Rebuild(
            assignmentMkDropdown, levels, value => "MK" + value, assignmentMk);
        _assignmentTraitOptions.Rebuild(
            assignmentTraitDropdown, Dimension3Catalog.TraitIds,
            GetTraitDisplayName, traitId);
        _assignmentChannelOptions.Rebuild(
            assignmentChannelDropdown, Dimension3Catalog.ProcessBankChannelIds,
            GetChannelDisplayName, channelId);
    }

    private static void SetOptions(TMP_Dropdown dropdown, string[] labels)
    {
        if (dropdown == null) return;
        int selected = dropdown.value;
        var options = new List<string>(labels);
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        dropdown.value = Math.Min(selected, labels.Length - 1);
    }

    private int GetSelectedProductionVersion()
    {
        int index = productionVersionDropdown == null ? 0 : productionVersionDropdown.value;
        return _productionVersionOptions.ResolveOrDefault(index, 1);
    }

    private int GetSelectedAssemblyMk()
    {
        int index = assemblyMkDropdown == null ? 0 : assemblyMkDropdown.value;
        return _assemblyMkOptions.ResolveOrDefault(index, 1);
    }

    private long GetSelectedProductionQuantity()
    {
        long[] values = { 1L, 5L, 10L, 25L, 50L };
        int index = productionQuantityDropdown == null ? 0 : productionQuantityDropdown.value;
        return values[Math.Max(0, Math.Min(index, values.Length - 1))];
    }

    private long GetSelectedAssemblyQuantity()
    {
        long[] values = { 1L, 5L, 10L, 25L };
        int index = assemblyQuantityDropdown == null ? 0 : assemblyQuantityDropdown.value;
        return values[Math.Max(0, Math.Min(index, values.Length - 1))];
    }

    private int GetSelectedAssignmentMk()
    {
        int index = assignmentMkDropdown == null ? 0 : assignmentMkDropdown.value;
        return _assignmentMkOptions.ResolveOrDefault(index, 1);
    }

    private string GetSelectedTraitId()
    {
        int index = assignmentTraitDropdown == null ? 0 : assignmentTraitDropdown.value;
        return _assignmentTraitOptions.ResolveOrDefault(index, Dimension3Catalog.TraitNormal);
    }

    private string GetSelectedChannelId()
    {
        int index = assignmentChannelDropdown == null ? 0 : assignmentChannelDropdown.value;
        return _assignmentChannelOptions.ResolveOrDefault(
            index, Dimension3Catalog.ChannelProcessPower);
    }

    private static string GetTraitDisplayName(string traitId)
    {
        switch (traitId)
        {
            case Dimension3Catalog.TraitFast: return "Rápido";
            case Dimension3Catalog.TraitEfficient: return "Eficiente";
            case Dimension3Catalog.TraitCoordinator: return "Coordinador";
            default: return "Normal";
        }
    }

    private static string GetChannelDisplayName(string channelId)
    {
        switch (channelId)
        {
            case Dimension3Catalog.ChannelProcessTime: return "Ritmo Operativo";
            case Dimension3Catalog.ChannelProcessCost: return "Ahorro Energético";
            case Dimension3Catalog.ChannelProcessCoordination: return "Coordinación Interna";
            default: return "Potencia de Proceso";
        }
    }

    private static void AppendQueue(
        StringBuilder builder,
        Dimension3State state,
        string queueId,
        string title
    )
    {
        builder.AppendLine(title);
        D3QueueState queue = D3JobQueueSystem.GetQueue(state, queueId);
        if (queue == null || queue.jobs == null || queue.jobs.Count == 0)
        {
            builder.AppendLine(T("d3.onboarding.queue.empty"));
            return;
        }

        int visibleCount = Math.Min(queue.jobs.Count, 5);
        for (int i = 0; i < visibleCount; i++)
        {
            D3JobState job = queue.jobs[i];
            if (job == null)
                continue;

            builder.Append(i == 0 && job.started
                ? T("d3.onboarding.queue.active")
                : T("d3.onboarding.queue.pending"));
            if (job.jobType == Dimension3Catalog.JobPartProduction)
                builder.Append(TF("d3.onboarding.queue.part",
                    GetPartDisplayName(job.targetId), job.version));
            else if (job.jobType == Dimension3Catalog.JobFacilityUpgrade)
                builder.Append(TF("d3.onboarding.queue.facility", job.version));
            else
                builder.Append(TF("d3.onboarding.queue.automaton", job.mk));
            builder.AppendLine(TF("d3.onboarding.queue.job_suffix",
                job.quantity, Math.Ceiling(job.remainingSeconds)));
        }

        if (queue.jobs.Count > visibleCount)
            builder.AppendLine(TF("d3.onboarding.queue.more",
                queue.jobs.Count - visibleCount));
    }

    private static string GetPartDisplayName(string partId)
    {
        switch (partId)
        {
            case Dimension3Catalog.PartChassis:
                return T("d3.onboarding.part.chassis");
            case Dimension3Catalog.PartMotor:
                return T("d3.onboarding.part.motor");
            case Dimension3Catalog.PartTool:
                return T("d3.onboarding.part.tool");
            case Dimension3Catalog.PartControl:
                return T("d3.onboarding.part.control");
            case Dimension3Catalog.PartRegulator:
                return T("d3.onboarding.part.regulator");
            default: return partId;
        }
    }

    private static string T(string key)
    {
        return PresentationTextCatalog.Current(key);
    }

    private static string TF(string key, params object[] args)
    {
        return PresentationTextCatalog.CurrentFormat(key, args);
    }

    private static string FormatNumber(double value)
    {
        if (value >= 1000000000.0) return (value / 1000000000.0).ToString("0.##") + "B";
        if (value >= 1000000.0) return (value / 1000000.0).ToString("0.##") + "M";
        if (value >= 1000.0) return (value / 1000.0).ToString("0.##") + "K";
        return Math.Floor(Math.Max(0.0, value)).ToString("0");
    }

    private void SetNotice(string message)
    {
        if (noticeText != null)
            noticeText.text = message ?? "";
    }

    private void SetView(bool firstEntry, bool factory)
    {
        if (firstEntryRoot != null) firstEntryRoot.SetActive(firstEntry);
        if (factoryRoot != null) factoryRoot.SetActive(factory);
    }

    private static void AddListener(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null)
            button.onClick.AddListener(action);
    }

    private static void SetInteractable(Button button, bool interactable)
    {
        if (button != null)
            button.interactable = interactable;
    }

    private static void SetButtonLabel(Button button, string label)
    {
        if (button == null) return;
        TMP_Text text = button.GetComponentInChildren<TMP_Text>(true);
        if (text != null) text.text = label ?? "";
    }
}

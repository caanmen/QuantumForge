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
    public TMP_Text factoryStatusText;
    public TMP_Text inventoryText;
    public TMP_Text queueText;
    public TMP_Text noticeText;
    public TMP_Text assignmentText;
    public TMP_Text powerText;
    public TMP_Text costPreviewText;

    [Header("Producción V1")]
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
        ConfigureDropdowns();
    }

    private void OpenCalibration()
    {
        if (calibrationPanel != null) calibrationPanel.Open();
    }

    private void OpenResearch()
    {
        if (researchPanel != null) researchPanel.Open();
    }

    private void OpenFacilities()
    {
        if (facilitiesPanel != null) facilitiesPanel.Open();
    }

    private void OpenQueues()
    {
        if (queuesPanel != null) queuesPanel.Open();
    }

    private void OnEnable()
    {
        OpenFromTab();
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
        bool firstEntrySeen = GameState.I.dimension3.firstEntrySeen;
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
        SetView(!firstEntrySeen, firstEntrySeen);
        Refresh();
    }

    public void ContinueFirstEntry()
    {
        Dimension3System.MarkFirstEntrySeen(GameState.I);
        SetView(false, true);
        SetNotice("Banco de Procesos disponible. Producción V1 preparada.");
        Refresh();
    }

    public void CloseDimension3()
    {
        if (TabsUI.Instance != null)
            TabsUI.Instance.ShowGeneracion();
    }

    public void Refresh()
    {
        GameState gameState = GameState.I;
        if (!Dimension3System.CanAccessDimension3(gameState))
            return;

        Dimension3System.EnsureState(gameState);
        RefreshStatus(gameState);
        RefreshInventory(gameState.dimension3);
        RefreshQueues(gameState.dimension3);
        RefreshAssignments(gameState.dimension3);
        RefreshPower(gameState.dimension3);
        RefreshCostPreview(gameState);
        RefreshButtons(gameState);

        if (!string.IsNullOrEmpty(_pendingCancelQueueId) &&
            Time.unscaledTime > _pendingCancelUntil)
        {
            _pendingCancelQueueId = "";
        }
    }

    private void QueuePart(string partId)
    {
        string reason;
        if (Dimension3System.TryQueuePartProduction(
                GameState.I,
                partId,
                GetSelectedProductionVersion(),
                GetSelectedProductionQuantity(),
                out reason))
        {
            SetNotice("Lote añadido a la cola: " + GetPartDisplayName(partId) +
                " ×" + GetSelectedProductionQuantity() + ".");
        }
        else
        {
            SetNotice(reason);
        }

        Refresh();
    }

    private void QueueMk1Assembly()
    {
        string reason;
        int mk = GetSelectedAssemblyMk();
        long quantity = GetSelectedAssemblyQuantity();
        if (Dimension3System.TryQueueNormalAssembly(GameState.I, mk, quantity, out reason))
            SetNotice("MK" + mk + " Normal ×" + quantity +
                " añadido a la cola de ensamblaje.");
        else
            SetNotice(reason);

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

        factoryStatusText.text =
            "BANCO DE PROCESOS — NIVEL " +
            D3FacilitySystem.GetProcessBankLevel(gameState.dimension3) + "\n" +
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

        var builder = new StringBuilder();
        int version = GetSelectedProductionVersion();
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
            string suffix = " — " + FormatNumber(partLE) + " LE + " +
                FormatNumber(partTraces) + " T";
            suffix = " ×" + productionQuantity + suffix;
            SetButtonLabel(produceChassisButton, "CHASIS" + suffix);
            SetButtonLabel(produceMotorButton, "SISTEMA MOTRIZ" + suffix);
            SetButtonLabel(produceToolButton, "HERRAMIENTA" + suffix);
            SetButtonLabel(produceControlButton, "MÓDULO DE CONTROL" + suffix);
            SetButtonLabel(produceRegulatorButton, "REGULADOR" + suffix);
        }
        D3CostTimeDefinition assemblyDefinition =
            Dimension3Catalog.GetNormalAssemblyDefinition(assemblyMk);
        if (assemblyDefinition != null)
        {
            SetButtonLabel(assembleMk1Button,
                "ENSAMBLAR MK" + assemblyMk + " NORMAL — " +
                FormatNumber(D3PowerSystem.GetModifiedCost(
                    gameState.dimension3, assemblyDefinition.leCost * assemblyQuantity)) + " LE + " +
                FormatNumber(D3PowerSystem.GetModifiedCost(
                    gameState.dimension3, assemblyDefinition.tracesCost * assemblyQuantity)) +
                " T ×" + assemblyQuantity);
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
            SetNotice(delta > 0L
                ? "Asignación añadida; se estabilizará en 30 segundos."
                : "Autómata retirado y disponible inmediatamente.");
        }
        else SetNotice(reason);
        Refresh();
    }

    private void UpgradeProcessBank()
    {
        string reason;
        if (Dimension3System.TryQueueProcessBankUpgrade(GameState.I, out reason))
            SetNotice("Mejora del Banco de Procesos añadida a la cola.");
        else SetNotice(reason);
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
        costPreviewText.text = preview == null ? "" : preview.ToDisplayText();
    }

    private void ConfigureDropdowns()
    {
        SetOptions(productionVersionDropdown, new[] { "V1", "V2", "V3", "V4", "V5", "V6" });
        SetOptions(productionQuantityDropdown,
            new[] { "Cantidad 1", "Cantidad 5", "Cantidad 10", "Cantidad 25", "Cantidad 50" });
        SetOptions(assemblyMkDropdown, new[] { "MK1", "MK2", "MK3", "MK4", "MK5", "MK6" });
        SetOptions(assemblyQuantityDropdown,
            new[] { "Cantidad 1", "Cantidad 5", "Cantidad 10", "Cantidad 25" });
        SetOptions(assignmentMkDropdown,
            new[] { "MK1", "MK2", "MK3", "MK4", "MK5", "MK6" });
        SetOptions(assignmentTraitDropdown,
            new[] { "Normal", "Rápido", "Eficiente", "Coordinador" });
        SetOptions(assignmentChannelDropdown,
            new[] { "Potencia", "Ritmo", "Ahorro", "Coordinación" });
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
        return productionVersionDropdown == null ? 1 : productionVersionDropdown.value + 1;
    }

    private int GetSelectedAssemblyMk()
    {
        return assemblyMkDropdown == null ? 1 : assemblyMkDropdown.value + 1;
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
        return assignmentMkDropdown == null ? 1 : assignmentMkDropdown.value + 1;
    }

    private string GetSelectedTraitId()
    {
        int index = assignmentTraitDropdown == null ? 0 : assignmentTraitDropdown.value;
        return Dimension3Catalog.TraitIds[Math.Max(0, Math.Min(index,
            Dimension3Catalog.TraitIds.Length - 1))];
    }

    private string GetSelectedChannelId()
    {
        int index = assignmentChannelDropdown == null ? 0 : assignmentChannelDropdown.value;
        return Dimension3Catalog.ProcessBankChannelIds[Math.Max(0, Math.Min(index,
            Dimension3Catalog.ProcessBankChannelIds.Length - 1))];
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
            builder.AppendLine("Sin trabajos.");
            return;
        }

        int visibleCount = Math.Min(queue.jobs.Count, 5);
        for (int i = 0; i < visibleCount; i++)
        {
            D3JobState job = queue.jobs[i];
            if (job == null)
                continue;

            builder.Append(i == 0 && job.started ? "ACTIVO — " : "PENDIENTE — ");
            if (job.jobType == Dimension3Catalog.JobPartProduction)
                builder.Append(GetPartDisplayName(job.targetId) + " V" + job.version);
            else if (job.jobType == Dimension3Catalog.JobFacilityUpgrade)
                builder.Append("Banco de Procesos nivel " + job.version);
            else
                builder.Append("MK" + job.mk + " Normal");
            builder.Append(" ×");
            builder.Append(job.quantity);
            builder.Append(" — ");
            builder.Append(Math.Ceiling(job.remainingSeconds));
            builder.AppendLine(" s");
        }

        if (queue.jobs.Count > visibleCount)
            builder.AppendLine("… y " + (queue.jobs.Count - visibleCount) + " más.");
    }

    private static string GetPartDisplayName(string partId)
    {
        switch (partId)
        {
            case Dimension3Catalog.PartChassis: return "Chasis";
            case Dimension3Catalog.PartMotor: return "Sistema Motriz";
            case Dimension3Catalog.PartTool: return "Herramienta";
            case Dimension3Catalog.PartControl: return "Módulo de Control";
            case Dimension3Catalog.PartRegulator: return "Regulador";
            default: return partId;
        }
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

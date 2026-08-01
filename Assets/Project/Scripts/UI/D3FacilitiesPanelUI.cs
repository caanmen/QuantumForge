using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D3FacilitiesPanelUI : MonoBehaviour
{
    public GameObject factoryRoot;
    public Button backButton;
    public TMP_Dropdown facilityDropdown;
    public TMP_Dropdown channelDropdown;
    public TMP_Dropdown mkDropdown;
    public TMP_Dropdown traitDropdown;
    public TMP_Text statusText;
    public TMP_Text functionsText;
    public TMP_Text noticeText;
    public Button addAssignmentButton;
    public Button removeAssignmentButton;
    public Button upgradeButton;
    public Button toggleAutoAnalyzeButton;
    public Button toggleAutoRepairButton;
    public Button openAutomationButton;
    public Button integrateAutonomyCoreButton;
    public D3AutomationPanelUI automationPanel;
    public Button openConsoleButton;
    public D3ConsolePanelUI consolePanel;
    public Button openDiagnosticButton;
    public D3DiagnosticPanelUI diagnosticPanel;

    private float _refreshRemaining;
    private readonly SafeDropdownOptionMap<string> _facilityOptions =
        new SafeDropdownOptionMap<string>(StringComparer.Ordinal);
    private readonly SafeDropdownOptionMap<string> _channelOptions =
        new SafeDropdownOptionMap<string>(StringComparer.Ordinal);
    private readonly SafeDropdownOptionMap<int> _mkOptions =
        new SafeDropdownOptionMap<int>();
    private readonly SafeDropdownOptionMap<string> _traitOptions =
        new SafeDropdownOptionMap<string>(StringComparer.Ordinal);

    private void Awake()
    {
        Add(backButton, Close);
        Add(addAssignmentButton, () => ChangeAssignment(1L));
        Add(removeAssignmentButton, () => ChangeAssignment(-1L));
        Add(upgradeButton, Upgrade);
        Add(toggleAutoAnalyzeButton, ToggleAutoAnalyze);
        Add(toggleAutoRepairButton, ToggleAutoRepair);
        Add(openAutomationButton, OpenAutomation);
        Add(integrateAutonomyCoreButton, IntegrateAutonomyCore);
        Add(openConsoleButton, OpenConsole);
        Add(openDiagnosticButton, OpenDiagnostic);
        if (facilityDropdown != null)
            facilityDropdown.onValueChanged.AddListener(_ => RefreshFacilityOptions());
        ConfigureOptions();
    }

    private void Update()
    {
        _refreshRemaining -= Time.unscaledDeltaTime;
        if (_refreshRemaining > 0f) return;
        _refreshRemaining = 0.2f;
        Refresh();
    }

    public void Open()
    {
        if (factoryRoot != null) factoryRoot.SetActive(false);
        gameObject.SetActive(true);
        ConfigureOptions();
        Refresh();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        if (factoryRoot != null) factoryRoot.SetActive(true);
    }

    private void ConfigureOptions()
    {
        _facilityOptions.Rebuild(facilityDropdown, new[]
            {
                Dimension3Catalog.FacilityProductionConsole,
                Dimension3Catalog.FacilityDiagnosticBank,
                Dimension3Catalog.FacilityExpeditionPort,
                Dimension3Catalog.FacilityAutomationCore
            }, GetFacilityName, Dimension3Catalog.FacilityProductionConsole);
        _mkOptions.Rebuild(mkDropdown,
            D3ProgressivePresentationRules.GetUnlockedAssemblyMks(GameState.I),
            value => "MK" + value, 1);
        _traitOptions.Rebuild(traitDropdown, Dimension3Catalog.TraitIds,
            GetTraitName, Dimension3Catalog.TraitNormal);
        RefreshFacilityOptions();
    }

    private void RefreshFacilityOptions()
    {
        string facilityId = GetFacilityId();
        if (facilityId == Dimension3Catalog.FacilityProductionConsole)
            _channelOptions.Rebuild(channelDropdown,
                Dimension3Catalog.ProductionConsoleChannelIds,
                GetChannelName, Dimension3Catalog.ChannelConsoleCapacity);
        else if (facilityId == Dimension3Catalog.FacilityDiagnosticBank ||
                 facilityId == Dimension3Catalog.FacilityExpeditionPort)
            _channelOptions.Rebuild(channelDropdown,
                facilityId == Dimension3Catalog.FacilityDiagnosticBank
                    ? Dimension3Catalog.DiagnosticBankChannelIds
                    : Dimension3Catalog.ExpeditionPortChannelIds,
                GetChannelName, facilityId == Dimension3Catalog.FacilityDiagnosticBank
                    ? Dimension3Catalog.ChannelDiagnosticCapacity
                    : Dimension3Catalog.ChannelPortCapacity);
        else
            _channelOptions.Rebuild(channelDropdown,
                Dimension3Catalog.AutomationCoreChannelIds,
                GetChannelName, Dimension3Catalog.ChannelCoreCoordination);
        Refresh();
    }

    private void Refresh()
    {
        if (GameState.I == null || GameState.I.dimension3 == null) return;
        Dimension3State state = GameState.I.dimension3;
        string facilityId = GetFacilityId();
        int level = D3FacilitySystem.GetFacilityLevel(state, facilityId);
        int selectedMk = _mkOptions.ResolveOrDefault(
            mkDropdown == null ? 0 : mkDropdown.value, 1);
        _mkOptions.Rebuild(mkDropdown,
            D3ProgressivePresentationRules.GetUnlockedAssemblyMks(GameState.I),
            value => "MK" + value, selectedMk);
        double capacity = D3FacilitySystem.GetEffectiveCapacity(state, facilityId);
        int nextFunction = Math.Min(5, Math.Max(1, level));
        D3AssignmentState assignment = D3FacilitySystem.GetAssignment(
            state, facilityId, GetChannelId(), GetMk(), GetTraitId());
        long assigned = assignment == null ? 0L : assignment.amount;
        long stable = assignment == null ? 0L : assignment.stabilizedAmount;
        if (statusText != null)
        {
            D3FacilityLevelDefinition next =
                Dimension3Catalog.GetFacilityLevelDefinition(facilityId, level + 1);
            if (level == 0 && next != null)
                statusText.text = GetFacilityName(facilityId) +
                    "\nFUNCIÓN: " + GetHumanFunction(facilityId) +
                    "\nCosto: " + D3PowerSystem.GetModifiedCost(state, next.leCost).ToString("0") +
                    " LE + " + D3PowerSystem.GetModifiedCost(state, next.tracesCost).ToString("0") +
                    " T · " + Math.Ceiling(next.durationSeconds) + " s" +
                    "\nRequisito: " + next.requiredAssemblyAmount +
                    " ensamblajes MK" + next.requiredAssemblyMk;
            else
                statusText.text = GetFacilityName(facilityId) + " — NIVEL " + level +
                    "\nFunción actual: " + GetHumanFunction(facilityId) +
                    "\nCapacidad efectiva: " + capacity.ToString("0.##") +
                    " | Siguiente nivel: " + (level >= 5 ? "máximo" : "N" + (level + 1)) +
                    "\nCanal seleccionado: " + GetChannelName() +
                    " | Asignados: " + assigned + " | Estables: " + stable;
        }
        if (functionsText != null)
        {
            functionsText.text = level == 0
                ? "DETALLE\nSe mostrará el control operativo cuando termine la construcción."
                : "FUNCIÓN OPERATIVA\n" + GetCurrentFunction(facilityId, level);
            if (level > 0 && facilityId == Dimension3Catalog.FacilityAutomationCore)
                functionsText.text += "\n\nCapacidad actual: " +
                    D3FacilitySystem.GetAutomationCoreRoutineLimit(state) +
                    " rutinas | " +
                    D3FacilitySystem.GetAutomationCoreProfileLimit(state) +
                    " perfiles | x" +
                    D3FacilitySystem.GetAutomationCoreEfficiencyMultiplier(state)
                        .ToString("0.00");
        }
        bool built = level > 0;
        SetInteractable(addAssignmentButton, built &&
            D3InventorySystem.GetAvailableAutomatonAmount(
                state, GetMk(), GetTraitId()) > 0L);
        SetInteractable(removeAssignmentButton, assigned > 0L);
        SetInteractable(upgradeButton,
            level < 5 && D3JobQueueSystem.GetJobCount(
                state, Dimension3Catalog.QueueFacility) == 0);
        SetLabel(upgradeButton, level == 0 ? "CONSTRUIR NIVEL 1" :
            level >= 5 ? "NIVEL MÁXIMO" : "AMPLIAR A NIVEL " + (level + 1));
        bool diagnostic = facilityId == Dimension3Catalog.FacilityDiagnosticBank;
        if (toggleAutoAnalyzeButton != null)
            toggleAutoAnalyzeButton.gameObject.SetActive(false);
        if (toggleAutoRepairButton != null)
            toggleAutoRepairButton.gameObject.SetActive(false);
        D3DiagnosticSettingsState settings = state.diagnosticSettings;
        SetLabel(toggleAutoAnalyzeButton,
            "AUTOANÁLISIS: " + (settings.autoAnalyzeEnabled ? "ON" : "OFF"));
        SetLabel(toggleAutoRepairButton,
            "AUTORREPARACIÓN: " + (settings.autoRepairEnabled ? "ON" : "OFF"));
        SetInteractable(toggleAutoAnalyzeButton, diagnostic && level >= 1);
        SetInteractable(toggleAutoRepairButton, diagnostic && level >= 2);
        bool automation = facilityId == Dimension3Catalog.FacilityExpeditionPort ||
            facilityId == Dimension3Catalog.FacilityAutomationCore;
        if (openAutomationButton != null)
            openAutomationButton.gameObject.SetActive(automation);
        SetInteractable(openAutomationButton, automation && level >= 1);
        bool autonomyCore = facilityId == Dimension3Catalog.FacilityAutomationCore;
        if (integrateAutonomyCoreButton != null)
            integrateAutonomyCoreButton.gameObject.SetActive(autonomyCore);
        if (autonomyCore)
        {
            bool canIntegrate = D3AutonomyCoreSystem.CanIntegrate(GameState.I,
                out string integrationReason);
            SetInteractable(integrateAutonomyCoreButton, canIntegrate);
            SetLabel(integrateAutonomyCoreButton,
                state.autonomyCoreIntegrated
                    ? "NÚCLEO DE AUTONOMÍA INTEGRADO"
                    : "INTEGRAR NÚCLEO DE AUTONOMÍA");
            if (!state.autonomyCoreIntegrated && !canIntegrate && noticeText != null &&
                string.IsNullOrEmpty(noticeText.text))
                noticeText.text = integrationReason;
        }
        bool console = facilityId == Dimension3Catalog.FacilityProductionConsole;
        if (openConsoleButton != null) openConsoleButton.gameObject.SetActive(console);
        SetInteractable(openConsoleButton, console && level >= 1);
        if (openDiagnosticButton != null)
            openDiagnosticButton.gameObject.SetActive(diagnostic);
        SetInteractable(openDiagnosticButton, diagnostic && level >= 1);
    }

    private void ChangeAssignment(long delta)
    {
        Dimension3State state = GameState.I.dimension3;
        string facilityId = GetFacilityId();
        D3AssignmentState assignment = D3FacilitySystem.GetAssignment(
            state, facilityId, GetChannelId(), GetMk(), GetTraitId());
        long current = assignment == null ? 0L : assignment.amount;
        if (Dimension3System.TrySetFacilityAssignment(
                GameState.I, facilityId, GetChannelId(), GetMk(), GetTraitId(),
                Math.Max(0L, current + delta), out string reason))
            SetNotice(delta > 0 ? "Asignación añadida; estabiliza en 30 segundos." :
                "Autómata retirado.");
        else SetNotice(reason);
        Refresh();
    }

    private void Upgrade()
    {
        string facilityId = GetFacilityId();
        if (Dimension3System.TryQueueFacilityUpgrade(
                GameState.I, facilityId, out string reason))
            SetNotice("Construcción o ampliación añadida a la cola.");
        else SetNotice(reason);
        Refresh();
    }

    private void ToggleAutoAnalyze()
    {
        Dimension3State state = GameState.I.dimension3;
        bool enabled = !state.diagnosticSettings.autoAnalyzeEnabled;
        state.diagnosticSettings.autoAnalyzeEnabled = enabled;
        SetNotice(enabled ? "Autoanálisis activado." : "Autoanálisis pausado.");
        Refresh();
    }

    private void ToggleAutoRepair()
    {
        Dimension3State state = GameState.I.dimension3;
        bool enabled = !state.diagnosticSettings.autoRepairEnabled;
        state.diagnosticSettings.autoRepairEnabled = enabled;
        SetNotice(enabled ? "Autorreparación activada." : "Autorreparación pausada.");
        Refresh();
    }

    private void OpenAutomation()
    {
        if (automationPanel != null) automationPanel.Open();
    }

    private void IntegrateAutonomyCore()
    {
        D3AutonomyCoreSystem.TryIntegrate(GameState.I, out string reason);
        SetNotice(reason);
        Refresh();
    }

    private void OpenConsole()
    {
        if (consolePanel != null) consolePanel.Open();
    }

    private void OpenDiagnostic()
    {
        if (diagnosticPanel != null) diagnosticPanel.Open();
    }

    private string GetFacilityId()
    {
        int index = facilityDropdown == null ? 0 : facilityDropdown.value;
        return _facilityOptions.ResolveOrDefault(index,
            Dimension3Catalog.FacilityProductionConsole);
    }

    private string GetChannelId()
    {
        int index = channelDropdown == null ? 0 : channelDropdown.value;
        string facilityId = GetFacilityId();
        return _channelOptions.ResolveOrDefault(index,
            D3FacilitySystem.GetCoordinationChannel(facilityId));
    }

    private int GetMk() => _mkOptions.ResolveOrDefault(
        mkDropdown == null ? 0 : mkDropdown.value, 1);
    private string GetTraitId()
    {
        int index = traitDropdown == null ? 0 : traitDropdown.value;
        return _traitOptions.ResolveOrDefault(index, Dimension3Catalog.TraitNormal);
    }
    private string GetChannelName() => channelDropdown == null ||
        channelDropdown.options.Count == 0 ? "-" :
        channelDropdown.options[channelDropdown.value].text;
    private static string GetFacilityName(string id)
    {
        if (id == Dimension3Catalog.FacilityDiagnosticBank)
            return "BANCO DE DIAGNÓSTICO";
        if (id == Dimension3Catalog.FacilityExpeditionPort)
            return "PUERTO DE EXPEDICIONES";
        if (id == Dimension3Catalog.FacilityAutomationCore)
            return "NÚCLEO DE AUTOMATIZACIÓN";
        return "CONSOLA DE PRODUCCIÓN";
    }
    private static string GetHumanFunction(string id)
    {
        if (id == Dimension3Catalog.FacilityDiagnosticBank)
            return "analiza y repara sistemas ya autorizados";
        if (id == Dimension3Catalog.FacilityExpeditionPort)
            return "repite expediciones D1 conocidas manualmente";
        if (id == Dimension3Catalog.FacilityAutomationCore)
            return "coordina rutinas y automatización offline";
        return "compra producción base ya autorizada";
    }
    private static string GetCurrentFunction(string id, int level)
    {
        return GetHumanFunction(id) + " · función N" + level + " activa" +
            (level < 5 ? " · siguiente: N" + (level + 1) : "");
    }
    private static string GetTraitName(string id)
    {
        if (id == Dimension3Catalog.TraitFast) return "Rápido";
        if (id == Dimension3Catalog.TraitEfficient) return "Eficiente";
        if (id == Dimension3Catalog.TraitCoordinator) return "Coordinador";
        return "Normal";
    }
    private static string GetChannelName(string id)
    {
        if (id != null && id.IndexOf("capacity", StringComparison.Ordinal) >= 0) return "Capacidad";
        if (id != null && id.IndexOf("response", StringComparison.Ordinal) >= 0) return "Respuesta";
        if (id != null && id.IndexOf("cost", StringComparison.Ordinal) >= 0) return "Disciplina";
        return "Coordinación";
    }
    private void SetNotice(string value)
    {
        if (noticeText != null) noticeText.text = value ?? "";
    }
    private static void Add(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null) button.onClick.AddListener(action);
    }
    private static void SetOptions(TMP_Dropdown dropdown, string[] labels)
    {
        if (dropdown == null) return;
        int selected = dropdown.value;
        dropdown.ClearOptions();
        dropdown.AddOptions(new List<string>(labels));
        dropdown.value = Math.Min(selected, labels.Length - 1);
    }
    private static void SetInteractable(Button button, bool value)
    {
        if (button != null) button.interactable = value;
    }
    private static void SetLabel(Button button, string value)
    {
        if (button == null) return;
        TMP_Text text = button.GetComponentInChildren<TMP_Text>(true);
        if (text != null) text.text = value;
    }
}

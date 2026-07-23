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
    public D3AutomationPanelUI automationPanel;
    public Button openConsoleButton;
    public D3ConsolePanelUI consolePanel;
    public Button openDiagnosticButton;
    public D3DiagnosticPanelUI diagnosticPanel;

    private float _refreshRemaining;

    private void Awake()
    {
        Add(backButton, Close);
        Add(addAssignmentButton, () => ChangeAssignment(1L));
        Add(removeAssignmentButton, () => ChangeAssignment(-1L));
        Add(upgradeButton, Upgrade);
        Add(toggleAutoAnalyzeButton, ToggleAutoAnalyze);
        Add(toggleAutoRepairButton, ToggleAutoRepair);
        Add(openAutomationButton, OpenAutomation);
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
        SetOptions(facilityDropdown,
            new[]
            {
                "Consola de Producción",
                "Banco de Diagnóstico",
                "Puerto de Expediciones",
                "Núcleo de Automatización"
            });
        SetOptions(mkDropdown,
            new[] { "MK1", "MK2", "MK3", "MK4", "MK5", "MK6" });
        SetOptions(traitDropdown,
            new[] { "Normal", "Rápido", "Eficiente", "Coordinador" });
        RefreshFacilityOptions();
    }

    private void RefreshFacilityOptions()
    {
        string facilityId = GetFacilityId();
        if (facilityId == Dimension3Catalog.FacilityProductionConsole)
            SetOptions(channelDropdown,
                new[] { "Capacidad", "Respuesta", "Disciplina", "Coordinación" });
        else if (facilityId == Dimension3Catalog.FacilityDiagnosticBank ||
                 facilityId == Dimension3Catalog.FacilityExpeditionPort)
            SetOptions(channelDropdown,
                new[] { "Capacidad", "Respuesta", "Coordinación" });
        else
            SetOptions(channelDropdown, new[] { "Coordinación" });
        Refresh();
    }

    private void Refresh()
    {
        if (GameState.I == null || GameState.I.dimension3 == null) return;
        Dimension3State state = GameState.I.dimension3;
        string facilityId = GetFacilityId();
        int level = D3FacilitySystem.GetFacilityLevel(state, facilityId);
        double capacity = D3FacilitySystem.GetEffectiveCapacity(state, facilityId);
        int nextFunction = Math.Min(5, Math.Max(1, level));
        D3AssignmentState assignment = D3FacilitySystem.GetAssignment(
            state, facilityId, GetChannelId(), GetMk(), GetTraitId());
        long assigned = assignment == null ? 0L : assignment.amount;
        long stable = assignment == null ? 0L : assignment.stabilizedAmount;
        if (statusText != null)
        {
            statusText.text = GetFacilityName(facilityId) + " — NIVEL " + level +
                "\nCapacidad efectiva: " + capacity.ToString("0.##") +
                " | Requerida N" + nextFunction + ": " +
                D3FacilitySystem.GetRequiredEffectiveCapacity(nextFunction).ToString("0") +
                "\nCanal seleccionado: " + GetChannelName() +
                " | Asignados: " + assigned + " | Estables: " + stable;
        }
        if (functionsText != null)
        {
            functionsText.text = facilityId == Dimension3Catalog.FacilityProductionConsole
                ? "FUNCIONES\nN1 Compras simples ya usadas manualmente\n" +
                  "N2 Prioridad y reservas\nN3 Mejoras repetibles ya usadas\n" +
                  "N4 Fase preferida del Modulador\nN5 Configuración básica del Triángulo\n\n" +
                  "Control operativo disponible desde el nivel 1."
                : "FUNCIONES\nN1 Autoanálisis de nodos válidos\n" +
                  "N2 Autorreparación con reservas\nN3 Prioridad por zona\n" +
                  "N4 Recetas de fusión marcadas\nN5 Rutina offline con Núcleo N5";
            if (facilityId == Dimension3Catalog.FacilityExpeditionPort)
                functionsText.text = "FUNCIONES\nN1 Repetir la última ruta simple conocida\n" +
                    "N2 Prioridades entre rutas conocidas\n" +
                    "N3 Selección de ruta segura\nN4 Mejoras de extractores ya usadas\n" +
                    "N5 Expediciones externas offline con Núcleo N5";
            else if (facilityId == Dimension3Catalog.FacilityAutomationCore)
                functionsText.text = "FUNCIONES\nN1 Dos rutinas y +5% de eficiencia\n" +
                    "N2 Tres rutinas y un perfil\nN3 Cuatro rutinas y +10% de eficiencia\n" +
                    "N4 Cinco rutinas y dos perfiles\nN5 Automatización externa offline\n\n" +
                    "Límite activo: " +
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
        switch (index)
        {
            case 1: return Dimension3Catalog.FacilityDiagnosticBank;
            case 2: return Dimension3Catalog.FacilityExpeditionPort;
            case 3: return Dimension3Catalog.FacilityAutomationCore;
            default: return Dimension3Catalog.FacilityProductionConsole;
        }
    }

    private string GetChannelId()
    {
        int index = channelDropdown == null ? 0 : channelDropdown.value;
        string facilityId = GetFacilityId();
        string[] channels;
        if (facilityId == Dimension3Catalog.FacilityProductionConsole)
            channels = Dimension3Catalog.ProductionConsoleChannelIds;
        else if (facilityId == Dimension3Catalog.FacilityDiagnosticBank)
            channels = Dimension3Catalog.DiagnosticBankChannelIds;
        else if (facilityId == Dimension3Catalog.FacilityExpeditionPort)
            channels = Dimension3Catalog.ExpeditionPortChannelIds;
        else
            channels = Dimension3Catalog.AutomationCoreChannelIds;
        return channels[Math.Max(0, Math.Min(index, channels.Length - 1))];
    }

    private int GetMk() => mkDropdown == null ? 1 : mkDropdown.value + 1;
    private string GetTraitId()
    {
        int index = traitDropdown == null ? 0 : traitDropdown.value;
        return Dimension3Catalog.TraitIds[
            Math.Max(0, Math.Min(index, Dimension3Catalog.TraitIds.Length - 1))];
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

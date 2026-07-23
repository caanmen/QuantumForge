using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class D3ConsolePanelUI : MonoBehaviour
{
    public D3FacilitiesPanelUI facilitiesPanel;
    public D3AutomationPanelUI automationPanel;
    public Button backButton;
    public TMP_Dropdown policyDropdown;
    public TMP_Dropdown leReserveDropdown;
    public TMP_Dropdown tracesReserveDropdown;
    public TMP_Text statusText;
    public TMP_Text historyText;
    public TMP_Text noticeText;
    public Button savePolicyButton;
    public Button recordPhaseButton;
    public Button recordTriangleButton;
    public Button openRoutinesButton;

    private float _refreshRemaining;

    private void Awake()
    {
        Add(backButton, Close);
        Add(savePolicyButton, SavePolicy);
        Add(recordPhaseButton, RecordPhase);
        Add(recordTriangleButton, RecordTriangle);
        Add(openRoutinesButton, OpenRoutines);
        Configure();
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
        if (facilitiesPanel != null) facilitiesPanel.gameObject.SetActive(false);
        gameObject.SetActive(true);
        Configure();
        Refresh();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        if (facilitiesPanel != null) facilitiesPanel.gameObject.SetActive(true);
    }

    private void Configure()
    {
        SetOptions(policyDropdown, new[] { "Prioridad LE", "Prioridad Trazas", "Equilibrio" });
        SetOptions(leReserveDropdown,
            new[] { "Reserva LE 0", "Reserva LE 1K", "Reserva LE 10K", "Reserva LE 100K", "Reserva LE 1M" });
        SetOptions(tracesReserveDropdown,
            new[] { "Reserva T 0", "Reserva T 100", "Reserva T 1K", "Reserva T 10K" });
        if (GameState.I == null || GameState.I.dimension3 == null) return;
        D3ConsoleSettingsState settings = GameState.I.dimension3.consoleSettings;
        if (policyDropdown != null)
            policyDropdown.value = settings.purchasePolicy == D3ConsoleSystem.PolicyLE ? 0 :
                settings.purchasePolicy == D3ConsoleSystem.PolicyTraces ? 1 : 2;
        if (leReserveDropdown != null) leReserveDropdown.value = ReserveLEIndex(settings.leReserve);
        if (tracesReserveDropdown != null)
            tracesReserveDropdown.value = ReserveTracesIndex(settings.tracesReserve);
    }

    private void Refresh()
    {
        if (GameState.I == null || GameState.I.dimension3 == null) return;
        Dimension3State state = GameState.I.dimension3;
        D3ConsoleSettingsState settings = state.consoleSettings;
        int level = D3FacilitySystem.GetFacilityLevel(
            state, Dimension3Catalog.FacilityProductionConsole);
        if (statusText != null)
        {
            BuildingState higgs = GameState.I.GetBuildingState(D3ConsoleSystem.BuildingHiggs);
            BuildingState tetra = GameState.I.GetBuildingState(D3ConsoleSystem.BuildingTetraquark);
            statusText.text = "CONSOLA — NIVEL " + level +
                " | Política: " + PolicyName(settings.purchasePolicy) +
                "\nReserva: " + settings.leReserve.ToString("0") + " LE | " +
                settings.tracesReserve.ToString("0") + " T" +
                "\nPróxima compra automática: Higgs " + Cost(GameState.I, higgs) +
                " LE | Tetra " + Cost(GameState.I, tetra) + " LE";
        }
        if (historyText != null)
        {
            var b = new StringBuilder("AUTORIZACIONES MANUALES\n");
            b.Append("Higgs: ").Append(D3ConsoleSystem.HasManualBuildingAuthorization(
                state, D3ConsoleSystem.BuildingHiggs) ? "Sí" : "No");
            b.Append(" | Tetra: ").Append(D3ConsoleSystem.HasManualBuildingAuthorization(
                state, D3ConsoleSystem.BuildingTetraquark) ? "Sí" : "No");
            b.Append("\nFases registradas: ").Append(
                settings.manuallySelectedModulatorModes.Count);
            b.Append(" | Triángulos básicos: ").Append(settings.manualTrianglePresets.Count);
            b.Append("\nN3 MEJORAS BÁSICAS: BLOQUEADO — falta lista cerrada de diseño.");
            historyText.text = b.ToString();
        }
        SetInteractable(savePolicyButton,
            D3FacilitySystem.IsFunctionActive(state,
                Dimension3Catalog.FacilityProductionConsole, 2));
        SetInteractable(recordPhaseButton, level >= 4 &&
            GameState.I.phaseModulatorMode != PhaseModulatorMode.None);
        SetInteractable(recordTriangleButton, level >= 5 &&
            GameState.I.IsTriangleFullyConfiguredWithBaseArtifacts());
        SetInteractable(openRoutinesButton, level >= 1);
    }

    private void SavePolicy()
    {
        string policy = policyDropdown == null || policyDropdown.value == 2
            ? D3ConsoleSystem.PolicyBalanced
            : policyDropdown.value == 0 ? D3ConsoleSystem.PolicyLE
            : D3ConsoleSystem.PolicyTraces;
        if (D3ConsoleSystem.TrySetPolicyAndReserves(GameState.I, policy,
                GetLEReserve(), GetTracesReserve(), out string reason))
            Notice("Política y reservas guardadas.");
        else Notice(reason);
        Refresh();
    }

    private void RecordPhase()
    {
        D3ConsoleSystem.RecordManualModulatorMode(
            GameState.I, GameState.I.phaseModulatorMode);
        Notice("Fase actual registrada como elegida manualmente.");
        Refresh();
    }

    private void RecordTriangle()
    {
        D3ConsoleSystem.RecordManualTriangleConfiguration(GameState.I);
        Notice("Configuración básica actual registrada.");
        Refresh();
    }

    private void OpenRoutines()
    {
        gameObject.SetActive(false);
        if (automationPanel != null) automationPanel.Open();
    }

    private double GetLEReserve()
    {
        double[] values = { 0.0, 1000.0, 10000.0, 100000.0, 1000000.0 };
        int index = leReserveDropdown == null ? 0 : leReserveDropdown.value;
        return values[Math.Max(0, Math.Min(index, values.Length - 1))];
    }

    private double GetTracesReserve()
    {
        double[] values = { 0.0, 100.0, 1000.0, 10000.0 };
        int index = tracesReserveDropdown == null ? 0 : tracesReserveDropdown.value;
        return values[Math.Max(0, Math.Min(index, values.Length - 1))];
    }

    private static int ReserveLEIndex(double value) => value >= 1000000 ? 4 :
        value >= 100000 ? 3 : value >= 10000 ? 2 : value >= 1000 ? 1 : 0;
    private static int ReserveTracesIndex(double value) => value >= 10000 ? 3 :
        value >= 1000 ? 2 : value >= 100 ? 1 : 0;
    private static string PolicyName(string value) => value == D3ConsoleSystem.PolicyLE
        ? "LE" : value == D3ConsoleSystem.PolicyTraces ? "Trazas" : "Equilibrio";
    private static string Cost(GameState gameState, BuildingState building) =>
        building == null ? "—" :
        D3ConsoleSystem.GetAutomatedBuildingCost(gameState, building).ToString("0");
    private void Notice(string value) { if (noticeText != null) noticeText.text = value ?? ""; }
    private static void Add(Button button, UnityEngine.Events.UnityAction action)
    { if (button != null) button.onClick.AddListener(action); }
    private static void SetInteractable(Button button, bool value)
    { if (button != null) button.interactable = value; }
    private static void SetOptions(TMP_Dropdown dropdown, string[] labels)
    {
        if (dropdown == null) return;
        int selected = dropdown.value;
        dropdown.ClearOptions();
        dropdown.AddOptions(new List<string>(labels));
        dropdown.value = Math.Max(0, Math.Min(selected, labels.Length - 1));
    }
}

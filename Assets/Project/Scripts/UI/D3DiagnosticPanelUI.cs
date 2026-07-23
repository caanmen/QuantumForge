using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D3DiagnosticPanelUI : MonoBehaviour
{
    public D3FacilitiesPanelUI facilitiesPanel;
    public Button backButton;
    public TMP_Text statusText;
    public TMP_Text recipeText;
    public TMP_Text noticeText;
    public Button toggleAnalyzeButton;
    public Button toggleRepairButton;
    public Button toggleFusionButton;
    public TMP_Dropdown priorityModeDropdown;
    public TMP_Dropdown zoneDropdown;
    public TMP_Dropdown leReserveDropdown;
    public TMP_Dropdown tracesReserveDropdown;
    public Button saveSettingsButton;
    public TMP_Dropdown recipeDropdown;
    public Button toggleRecipeMarkButton;
    public Button saveRoutineButton;
    public Button loadRoutineButton;

    private readonly List<string> _recipeIds = new List<string>();
    private float _refreshRemaining;

    private void Awake()
    {
        Add(backButton, Close);
        Add(toggleAnalyzeButton, () => Toggle(1));
        Add(toggleRepairButton, () => Toggle(2));
        Add(toggleFusionButton, () => Toggle(4));
        Add(saveSettingsButton, SaveSettings);
        Add(toggleRecipeMarkButton, ToggleRecipeMark);
        Add(saveRoutineButton, SaveRoutine);
        Add(loadRoutineButton, LoadRoutine);
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
        SetOptions(priorityModeDropdown,
            new[] { "Orden ascendente", "Prioridad por zona" });
        SetOptions(zoneDropdown, new[]
        {
            "Sin zona", "Enlace Cuarto 1", "Sector de Fusión",
            "Soporte Interno", "Cámara Instantánea"
        });
        SetOptions(leReserveDropdown,
            new[] { "Reserva LE 0", "Reserva LE 1K", "Reserva LE 10K",
                "Reserva LE 100K", "Reserva LE 1M" });
        SetOptions(tracesReserveDropdown,
            new[] { "Reserva T 0", "Reserva T 100", "Reserva T 1K",
                "Reserva T 10K" });
        if (GameState.I == null || GameState.I.dimension3 == null) return;
        D3DiagnosticSettingsState settings = GameState.I.dimension3.diagnosticSettings;
        priorityModeDropdown.value = Math.Max(0, Math.Min(1, settings.priorityMode));
        zoneDropdown.value = Math.Max(0, Math.Min(4, settings.priorityZone));
        leReserveDropdown.value = ReserveLEIndex(settings.leReserve);
        tracesReserveDropdown.value = ReserveTracesIndex(settings.tracesReserve);
        RefreshRecipes();
    }

    private void Refresh()
    {
        if (GameState.I == null || GameState.I.dimension3 == null) return;
        Dimension3State state = GameState.I.dimension3;
        D3DiagnosticSettingsState settings = state.diagnosticSettings;
        int level = D3FacilitySystem.GetFacilityLevel(
            state, Dimension3Catalog.FacilityDiagnosticBank);
        SetLabel(toggleAnalyzeButton,
            "AUTOANÁLISIS: " + (settings.autoAnalyzeEnabled ? "ON" : "OFF"));
        SetLabel(toggleRepairButton,
            "AUTORREPARACIÓN: " + (settings.autoRepairEnabled ? "ON" : "OFF"));
        SetLabel(toggleFusionButton,
            "AUTOFUSIÓN: " + (settings.autoFusionEnabled ? "ON" : "OFF"));
        SetInteractable(toggleAnalyzeButton, level >= 1);
        SetInteractable(toggleRepairButton, level >= 2);
        SetInteractable(saveSettingsButton, level >= 3);
        SetInteractable(toggleFusionButton, level >= 4);
        SetInteractable(toggleRecipeMarkButton, level >= 4 && _recipeIds.Count > 0);
        SetInteractable(saveRoutineButton, level >= 5);
        SetInteractable(loadRoutineButton, level >= 5 &&
            settings.savedRoutine != null && settings.savedRoutine.saved);
        if (zoneDropdown != null)
            zoneDropdown.interactable = level >= 3 &&
                priorityModeDropdown != null && priorityModeDropdown.value == 1;
        if (statusText != null)
        {
            statusText.text = "BANCO DE DIAGNÓSTICO — NIVEL " + level +
                "\nOrden: " + (settings.priorityMode == 1
                    ? "zona " + ZoneName(settings.priorityZone)
                    : "ascendente global") +
                " | Reservas: " + settings.leReserve.ToString("0") +
                " LE / " + settings.tracesReserve.ToString("0") + " T" +
                "\nOffline: " + (D3DiagnosticSystem.CanRunOffline(GameState.I)
                    ? "habilitado" : "requiere Diagnóstico N5 + Núcleo N5");
        }
        RefreshRecipes();
    }

    private void RefreshRecipes()
    {
        string selected = SelectedRecipeId();
        _recipeIds.Clear();
        var labels = new List<string>();
        if (GameState.I != null && GameState.I.dimension3 != null)
        {
            List<D3MarkedFusionRecipeState> recipes =
                GameState.I.dimension3.markedFusionRecipes;
            for (int i = 0; i < recipes.Count; i++)
            {
                D3MarkedFusionRecipeState recipe = recipes[i];
                if (recipe == null || !recipe.manuallyExecuted) continue;
                _recipeIds.Add(recipe.recipeId);
                labels.Add(RecipeName(recipe.recipeId) +
                    (recipe.automationMarked ? " [MARCADA]" : ""));
            }
        }
        if (labels.Count == 0) labels.Add("Sin recetas manuales");
        if (recipeDropdown != null)
        {
            recipeDropdown.ClearOptions();
            recipeDropdown.AddOptions(labels);
            int index = _recipeIds.IndexOf(selected);
            recipeDropdown.value = index >= 0 ? index : 0;
        }
        if (recipeText != null)
        {
            var b = new StringBuilder("RECETAS DE FUSIÓN\n");
            b.Append(_recipeIds.Count).Append(" ejecutadas manualmente | ");
            int marked = 0;
            if (GameState.I != null && GameState.I.dimension3 != null)
                for (int i = 0; i < GameState.I.dimension3.markedFusionRecipes.Count; i++)
                    if (GameState.I.dimension3.markedFusionRecipes[i] != null &&
                        GameState.I.dimension3.markedFusionRecipes[i].automationMarked)
                        marked++;
            b.Append(marked).Append(" marcadas para repetir");
            recipeText.text = b.ToString();
        }
        D3MarkedFusionRecipeState selectedRecipe = GameState.I == null ? null :
            D3DiagnosticSystem.FindMarkedRecipe(GameState.I.dimension3,
                SelectedRecipeId());
        SetLabel(toggleRecipeMarkButton, selectedRecipe != null &&
            selectedRecipe.automationMarked ? "DESMARCAR RECETA" : "MARCAR RECETA");
    }

    private void Toggle(int level)
    {
        D3DiagnosticSettingsState settings = GameState.I.dimension3.diagnosticSettings;
        if (level == 1) settings.autoAnalyzeEnabled = !settings.autoAnalyzeEnabled;
        else if (level == 2) settings.autoRepairEnabled = !settings.autoRepairEnabled;
        else settings.autoFusionEnabled = !settings.autoFusionEnabled;
        Notice("Estado automático actualizado.");
        Refresh();
    }

    private void SaveSettings()
    {
        if (D3DiagnosticSystem.TryConfigure(GameState.I,
                priorityModeDropdown == null ? 0 : priorityModeDropdown.value,
                zoneDropdown == null ? 0 : zoneDropdown.value,
                GetLEReserve(), GetTracesReserve(), out string reason))
            Notice("Prioridad y reservas guardadas.");
        else Notice(reason);
        Refresh();
    }

    private void ToggleRecipeMark()
    {
        string id = SelectedRecipeId();
        D3MarkedFusionRecipeState recipe = D3DiagnosticSystem.FindMarkedRecipe(
            GameState.I.dimension3, id);
        bool mark = recipe != null && !recipe.automationMarked;
        if (D3DiagnosticSystem.TrySetRecipeAutomationMark(
                GameState.I, id, mark, out string reason))
            Notice(mark ? "Receta marcada para repetición." : "Receta desmarcada.");
        else Notice(reason);
        Refresh();
    }

    private void SaveRoutine()
    {
        if (D3DiagnosticSystem.TrySaveRoutine(GameState.I, out string reason))
            Notice("Rutina diagnóstica completa guardada.");
        else Notice(reason);
        Refresh();
    }

    private void LoadRoutine()
    {
        if (D3DiagnosticSystem.TryLoadRoutine(GameState.I, out string reason))
            Notice("Rutina diagnóstica cargada.");
        else Notice(reason);
        Configure();
        Refresh();
    }

    private string SelectedRecipeId()
    {
        if (recipeDropdown == null || _recipeIds.Count == 0) return "";
        int index = Math.Max(0, Math.Min(recipeDropdown.value, _recipeIds.Count - 1));
        return _recipeIds[index];
    }
    private double GetLEReserve()
    {
        double[] values = { 0, 1000, 10000, 100000, 1000000 };
        int index = leReserveDropdown == null ? 0 : leReserveDropdown.value;
        return values[Math.Max(0, Math.Min(index, values.Length - 1))];
    }
    private double GetTracesReserve()
    {
        double[] values = { 0, 100, 1000, 10000 };
        int index = tracesReserveDropdown == null ? 0 : tracesReserveDropdown.value;
        return values[Math.Max(0, Math.Min(index, values.Length - 1))];
    }
    private static string RecipeName(string id)
    {
        if (!D3FusionService.TryParseRecipeId(id, out ExperimentalFragmentType a,
                out ExperimentalFragmentType b, out ExperimentalCatalystType c))
            return id;
        return a + " + " + b + " / " + c;
    }
    private static string ZoneName(int value) => value == 1 ? "Enlace Cuarto 1" :
        value == 2 ? "Sector de Fusión" : value == 3 ? "Soporte Interno" :
        value == 4 ? "Cámara Instantánea" : "Sin zona";
    private static int ReserveLEIndex(double value) => value >= 1000000 ? 4 :
        value >= 100000 ? 3 : value >= 10000 ? 2 : value >= 1000 ? 1 : 0;
    private static int ReserveTracesIndex(double value) => value >= 10000 ? 3 :
        value >= 1000 ? 2 : value >= 100 ? 1 : 0;
    private void Notice(string value) { if (noticeText != null) noticeText.text = value ?? ""; }
    private static void Add(Button button, UnityEngine.Events.UnityAction action)
    { if (button != null) button.onClick.AddListener(action); }
    private static void SetInteractable(Button button, bool value)
    { if (button != null) button.interactable = value; }
    private static void SetLabel(Button button, string value)
    {
        if (button == null) return;
        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        if (label != null) label.text = value;
    }
    private static void SetOptions(TMP_Dropdown dropdown, string[] labels)
    {
        if (dropdown == null) return;
        int selected = dropdown.value;
        dropdown.ClearOptions();
        dropdown.AddOptions(new List<string>(labels));
        dropdown.value = Math.Max(0, Math.Min(selected, labels.Length - 1));
    }
}

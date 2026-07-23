using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class D3AutomationPanelUI : MonoBehaviour
{
    public D3FacilitiesPanelUI facilitiesPanel;
    public Button backButton;
    public TMP_Dropdown actionDropdown;
    public TMP_Dropdown targetDropdown;
    public TMP_Dropdown priorityDropdown;
    public TMP_Dropdown stopDropdown;
    public TMP_Dropdown reserveDropdown;
    public TMP_Dropdown profileDropdown;
    public TMP_Dropdown routineDropdown;
    public TMP_Text statusText;
    public TMP_Text routineText;
    public TMP_Text noticeText;
    public Button createButton;
    public Button toggleButton;
    public Button deleteButton;
    public Button saveProfileButton;
    public Button loadProfileButton;

    private string _knownRoutineSignature = "";
    private float _refreshRemaining;

    private static readonly string[] ActionIds =
    {
        D3AutomationCatalog.ActionPortScan,
        D3AutomationCatalog.ActionPortRepeatLast,
        D3AutomationCatalog.ActionPortPriorityRoutes,
        D3AutomationCatalog.ActionPortSafeRoute,
        D3AutomationCatalog.ActionPortExtractor,
        D3AutomationCatalog.ActionConsoleBuyHiggs,
        D3AutomationCatalog.ActionConsoleBuyTetraquark,
        D3AutomationCatalog.ActionConsoleModulator,
        D3AutomationCatalog.ActionConsoleTriangle
    };

    private void Awake()
    {
        Add(backButton, Close);
        Add(createButton, CreateRoutine);
        Add(toggleButton, ToggleRoutine);
        Add(deleteButton, DeleteRoutine);
        Add(saveProfileButton, SaveProfile);
        Add(loadProfileButton, LoadProfile);
        if (actionDropdown != null)
            actionDropdown.onValueChanged.AddListener(_ => RefreshTargets());
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
        if (facilitiesPanel != null) facilitiesPanel.gameObject.SetActive(false);
        gameObject.SetActive(true);
        ConfigureOptions();
        Refresh();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        if (facilitiesPanel != null) facilitiesPanel.gameObject.SetActive(true);
    }

    private void ConfigureOptions()
    {
        SetOptions(actionDropdown, new[]
        {
            "Barrido simple", "Repetir ruta", "Rutas prioritarias",
            "Ruta segura", "Mejorar extractor", "Comprar Higgs",
            "Comprar Tetraquark", "Mantener fase", "Aplicar Triángulo"
        });
        SetOptions(priorityDropdown,
            new[] { "Prioridad 0", "Prioridad 1", "Prioridad 2", "Prioridad 3" });
        SetOptions(stopDropdown,
            new[] { "Sin limite", "1 ejecucion", "5 ejecuciones", "10 ejecuciones" });
        SetOptions(reserveDropdown,
            new[] { "Reserva 0", "Reserva 50", "Reserva 100", "Reserva 1000" });
        SetOptions(profileDropdown, new[] { "Perfil 1", "Perfil 2" });
        RefreshTargets();
        _knownRoutineSignature = "";
    }

    private void RefreshTargets()
    {
        string actionId = GetActionId();
        if (actionId == D3AutomationCatalog.ActionConsoleModulator)
            SetOptions(targetDropdown,
                new[] { "Expansión", "Conservación", "Sintonía" });
        else if (actionId == D3AutomationCatalog.ActionConsoleTriangle)
        {
            var labels = new List<string>();
            D3ConsoleSettingsState settings = GameState.I == null ||
                GameState.I.dimension3 == null ? null :
                GameState.I.dimension3.consoleSettings;
            if (settings != null)
                for (int i = 0; i < settings.manualTrianglePresets.Count; i++)
                    labels.Add("Configuración básica " + (i + 1));
            if (labels.Count == 0) labels.Add("Sin configuración manual");
            SetOptions(targetDropdown, labels.ToArray());
        }
        else if (actionId == D3AutomationCatalog.ActionPortExtractor)
            SetOptions(targetDropdown, new[]
            {
                "Planeta 1", "Planeta 2", "Planeta 3", "Planeta 4",
                "Planeta 5", "Planeta 6", "Planeta 7"
            });
        else if (IsPortRouteAction(actionId))
            SetOptions(targetDropdown, new[]
            {
                "Cinturon mineral", "Cementerio de naves", "Sondas a la deriva",
                "Nave abandonada", "Ruina orbital", "Laboratorio",
                "Estacion abandonada", "Anomalia menor",
                "Estructura antigua", "Zona inestable"
            });
        Refresh();
    }

    private void Refresh()
    {
        if (GameState.I == null || GameState.I.dimension3 == null) return;
        Dimension3State state = GameState.I.dimension3;
        string signature = GetRoutineSignature(state);
        if (_knownRoutineSignature != signature)
        {
            int selected = routineDropdown == null ? 0 : routineDropdown.value;
            var labels = new List<string>();
            for (int i = 0; i < state.automationRoutines.Count; i++)
            {
                D3AutomationRoutineState item = state.automationRoutines[i];
                labels.Add(item == null ? "Rutina invalida" :
                    (item.enabled ? "[ON] " : "[OFF] ") +
                    GetActionName(item.actionId) + " #" + (i + 1));
            }
            if (labels.Count == 0) labels.Add("Sin rutinas");
            SetOptions(routineDropdown, labels.ToArray());
            if (routineDropdown != null)
                routineDropdown.value = Math.Min(selected, labels.Count - 1);
            _knownRoutineSignature = signature;
        }
        D3AutomationRoutineState routine = GetSelectedRoutine();
        if (statusText != null)
            statusText.text = "MOTOR ONLINE\nActivas: " +
                D3AutomationSystem.CountEnabled(state) + " / " +
                D3AutomationSystem.GetRoutineLimit(state) +
                " | Perfiles: " + state.automationProfiles.Count + " / " +
                D3FacilitySystem.GetAutomationCoreProfileLimit(state) +
                "\nOffline externo: " +
                (D3AutomationSystem.CanRunAutomationOffline(GameState.I)
                    ? "HABILITADO (bloques de 60 s, max. 12 h)"
                    : "BLOQUEADO (requiere instalación N5 + Núcleo N5 activos)") +
                "\nUna transaccion externa maxima por ciclo.";
        if (routineText != null)
            routineText.text = routine == null
                ? "RUTINA SELECCIONADA\nNo hay rutinas creadas."
                : "RUTINA SELECCIONADA\n" + GetActionName(routine.actionId) +
                  " | Prioridad " + routine.priority +
                  "\nEjecuciones: " + routine.executionsCompleted +
                  (routine.stopAfterExecutions > 0
                      ? " / " + routine.stopAfterExecutions : " / sin limite") +
                  "\nReserva LE: " + routine.leReserve.ToString("0") +
                  " | Trazas: " + routine.tracesReserve.ToString("0") +
                  " | Recurso: " + routine.resourceReserveAmount.ToString("0") +
                  "\nUltimo resultado: " + routine.lastResult;
        SetLabel(toggleButton,
            routine != null && routine.enabled ? "PAUSAR RUTINA" : "ACTIVAR RUTINA");
        SetInteractable(toggleButton, routine != null);
        SetInteractable(deleteButton, routine != null);
        SetInteractable(saveProfileButton,
            D3FacilitySystem.GetAutomationCoreProfileLimit(state) > 0);
        SetInteractable(loadProfileButton, state.automationProfiles.Count > 0 &&
            D3FacilitySystem.GetAutomationCoreProfileLimit(state) > 0);
        bool targetNeeded = GetActionId() != D3AutomationCatalog.ActionPortScan &&
            GetActionId() != D3AutomationCatalog.ActionPortSafeRoute &&
            GetActionId() != D3AutomationCatalog.ActionConsoleBuyHiggs &&
            GetActionId() != D3AutomationCatalog.ActionConsoleBuyTetraquark;
        if (targetDropdown != null)
            targetDropdown.gameObject.SetActive(targetNeeded);
    }

    private void CreateRoutine()
    {
        string actionId = GetActionId();
        string targetId = GetTargetId(actionId);
        List<string> priorities = null;
        if (actionId == D3AutomationCatalog.ActionPortPriorityRoutes)
        {
            priorities = new List<string>();
            if (!string.IsNullOrEmpty(targetId)) priorities.Add(targetId);
            for (int i = 0;
                 i < GameState.I.dimension1ManualSimpleDestinationIds.Count; i++)
            {
                string id = GameState.I.dimension1ManualSimpleDestinationIds[i];
                if (!priorities.Contains(id)) priorities.Add(id);
            }
        }
        double reserve = GetReserve();
        string resourceId = "";
        if (actionId == D3AutomationCatalog.ActionPortExtractor)
        {
            D1PlanetState planet = FindPlanet(targetId);
            if (planet != null)
                resourceId = Dimension1System.GetExtractorUpgradeMainCostMetal(planet);
        }
        if (D3AutomationSystem.TryCreateRoutine(GameState.I, actionId, targetId,
                priorities, priorityDropdown == null ? 0 : priorityDropdown.value,
                0.0, 0.0, resourceId, reserve, GetStopCount(),
                out _, out string reason))
            SetNotice("Rutina creada pausada. Actívala cuando quieras.");
        else SetNotice(reason);
        _knownRoutineSignature = "";
        Refresh();
    }

    private void ToggleRoutine()
    {
        D3AutomationRoutineState routine = GetSelectedRoutine();
        if (routine == null) return;
        if (D3AutomationSystem.TrySetRoutineEnabled(GameState.I,
                routine.routineId, !routine.enabled, out string reason))
            SetNotice(routine.enabled ? "Rutina activada." : "Rutina pausada.");
        else SetNotice(reason);
        _knownRoutineSignature = "";
        Refresh();
    }

    private void DeleteRoutine()
    {
        D3AutomationRoutineState routine = GetSelectedRoutine();
        if (routine == null) return;
        if (D3AutomationSystem.TryDeleteRoutine(GameState.I.dimension3,
                routine.routineId, out string reason))
            SetNotice("Rutina eliminada. Los gastos ya ejecutados no se devuelven.");
        else SetNotice(reason);
        _knownRoutineSignature = "";
        Refresh();
    }

    private void SaveProfile()
    {
        string profileId = GetProfileId();
        if (D3AutomationSystem.TrySaveProfile(GameState.I, profileId,
                "Perfil de automatizacion " + (GetProfileIndex() + 1),
                out string reason))
            SetNotice("Perfil guardado con todas las rutinas y reservas.");
        else SetNotice(reason);
        Refresh();
    }

    private void LoadProfile()
    {
        if (D3AutomationSystem.TryLoadProfile(GameState.I,
                GetProfileId(), out string reason))
            SetNotice("Perfil cargado.");
        else SetNotice(reason);
        _knownRoutineSignature = "";
        Refresh();
    }

    private string GetActionId()
    {
        int index = actionDropdown == null ? 0 : actionDropdown.value;
        return ActionIds[Math.Max(0, Math.Min(index, ActionIds.Length - 1))];
    }

    private string GetTargetId(string actionId)
    {
        int index = targetDropdown == null ? 0 : targetDropdown.value;
        if (actionId == D3AutomationCatalog.ActionPortExtractor)
        {
            string[] planets =
            {
                Dimension1System.Planet01, Dimension1System.Planet02,
                Dimension1System.Planet03, Dimension1System.Planet04,
                Dimension1System.Planet05, Dimension1System.Planet06,
                Dimension1System.Planet07
            };
            return planets[Math.Max(0, Math.Min(index, planets.Length - 1))];
        }
        if (actionId == D3AutomationCatalog.ActionConsoleModulator)
            return (Math.Max(0, Math.Min(index, 2)) + 1).ToString();
        if (actionId == D3AutomationCatalog.ActionConsoleTriangle)
        {
            D3ConsoleSettingsState settings = GameState.I.dimension3.consoleSettings;
            return settings.manualTrianglePresets.Count == 0 ? "" :
                settings.manualTrianglePresets[Math.Max(0,
                    Math.Min(index, settings.manualTrianglePresets.Count - 1))].presetId;
        }
        if (!IsPortRouteAction(actionId)) return "";
        return D3AutomationCatalog.Destinations[
            Math.Max(0, Math.Min(index,
                D3AutomationCatalog.Destinations.Length - 1))].destinationId;
    }

    private D3AutomationRoutineState GetSelectedRoutine()
    {
        Dimension3State state = GameState.I == null ? null : GameState.I.dimension3;
        if (state == null || state.automationRoutines.Count == 0) return null;
        int index = routineDropdown == null ? 0 : routineDropdown.value;
        return state.automationRoutines[
            Math.Max(0, Math.Min(index, state.automationRoutines.Count - 1))];
    }

    private int GetStopCount()
    {
        int index = stopDropdown == null ? 0 : stopDropdown.value;
        return index == 1 ? 1 : index == 2 ? 5 : index == 3 ? 10 : 0;
    }

    private double GetReserve()
    {
        int index = reserveDropdown == null ? 0 : reserveDropdown.value;
        return index == 1 ? 50.0 : index == 2 ? 100.0 :
            index == 3 ? 1000.0 : 0.0;
    }

    private int GetProfileIndex() =>
        profileDropdown == null ? 0 : profileDropdown.value;

    private string GetProfileId() =>
        "automation_profile_" + (GetProfileIndex() + 1);

    private static string GetActionName(string actionId)
    {
        if (actionId == D3AutomationCatalog.ActionPortScan) return "Barrido simple";
        if (actionId == D3AutomationCatalog.ActionPortRepeatLast) return "Repetir ruta";
        if (actionId == D3AutomationCatalog.ActionPortPriorityRoutes) return "Rutas prioritarias";
        if (actionId == D3AutomationCatalog.ActionPortSafeRoute) return "Ruta segura";
        if (actionId == D3AutomationCatalog.ActionPortExtractor) return "Mejorar extractor";
        if (actionId == D3AutomationCatalog.ActionConsoleBuyHiggs) return "Comprar Higgs";
        if (actionId == D3AutomationCatalog.ActionConsoleBuyTetraquark) return "Comprar Tetraquark";
        if (actionId == D3AutomationCatalog.ActionConsoleModulator) return "Mantener fase";
        if (actionId == D3AutomationCatalog.ActionConsoleTriangle) return "Aplicar Triángulo";
        return actionId;
    }

    private static bool IsPortRouteAction(string actionId)
    {
        return actionId == D3AutomationCatalog.ActionPortScan ||
            actionId == D3AutomationCatalog.ActionPortRepeatLast ||
            actionId == D3AutomationCatalog.ActionPortPriorityRoutes ||
            actionId == D3AutomationCatalog.ActionPortSafeRoute;
    }

    private static string GetRoutineSignature(Dimension3State state)
    {
        string value = state.automationRoutines.Count.ToString();
        for (int i = 0; i < state.automationRoutines.Count; i++)
        {
            D3AutomationRoutineState routine = state.automationRoutines[i];
            value += routine != null && routine.enabled ? "1" : "0";
        }
        return value;
    }

    private static D1PlanetState FindPlanet(string planetId)
    {
        if (GameState.I == null || GameState.I.dimension1Planets == null) return null;
        for (int i = 0; i < GameState.I.dimension1Planets.Count; i++)
        {
            D1PlanetState planet = GameState.I.dimension1Planets[i];
            if (planet != null && planet.planetId == planetId) return planet;
        }
        return null;
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
        dropdown.value = Math.Max(0, Math.Min(selected, labels.Length - 1));
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

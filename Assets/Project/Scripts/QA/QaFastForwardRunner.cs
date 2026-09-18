using System;
using System.Collections;
using UnityEngine;

public class QaFastForwardRunner : MonoBehaviour
{
    private const double StepSeconds = 1.0;
    private const int StepsPerYield = 50;
    private const double AggregateOfflineThresholdSeconds = 3600.0;

    public QaPanelUI panel;

    public bool IsRunning { get; private set; }
    public float Progress01 { get; private set; }
    public double ProcessedSecondsThisRun { get; private set; }
    public string LastError { get; private set; }

    private Coroutine activeRoutine;

    private void Awake()
    {
        ResolvePanel();
    }

    private void OnEnable()
    {
        ResolvePanel();
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public void RequestAdvance(double exactGameSeconds)
    {
        if (!QaRuntimeService.IsAvailable)
            return;

        if (IsRunning)
        {
            SetStatus("YA HAY UN AVANCE QA EN CURSO");
            return;
        }

        if (double.IsNaN(exactGameSeconds) ||
            double.IsInfinity(exactGameSeconds) || exactGameSeconds <= 0.0)
        {
            LastError = "La duración solicitada no es válida.";
            SetStatus("ERROR: " + LastError);
            return;
        }

        activeRoutine = StartCoroutine(RunAdvance(exactGameSeconds));
    }

    private IEnumerator RunAdvance(double exactGameSeconds)
    {
        IsRunning = true;
        Progress01 = 0f;
        ProcessedSecondsThisRun = 0.0;
        LastError = null;
        SetControlsInteractable(false);
        SetStatus(BuildProgressStatus(exactGameSeconds));

        try
        {
            if (GameState.I == null)
            {
                Fail("GameState no está disponible.");
                yield break;
            }

            if (!TrySave("antes de comenzar"))
                yield break;

            GameState state = GameState.I;
            MachineManager machine = MachineManager.I;
            Room2PanelUI room2 = UnityEngine.Object.FindFirstObjectByType<
                Room2PanelUI>(FindObjectsInactive.Include);
            double leBefore = state.LE;
            double tracesBefore = state.Traces;

            if (exactGameSeconds >= AggregateOfflineThresholdSeconds)
            {
                try
                {
                    ApplyAggregateOfflineProgress(
                        exactGameSeconds, state, machine, room2);
                }
                catch (Exception exception)
                {
                    Fail("Excepción durante el avance de ausencia: " +
                        exception.Message);
                    yield break;
                }

                ProcessedSecondsThisRun = exactGameSeconds;
                Progress01 = 1f;
                RefreshAfterAdvance();
                yield return null;
            }
            else
            {
                int stepsSinceYield = 0;
                while (ProcessedSecondsThisRun < exactGameSeconds)
                {
                    double remaining = exactGameSeconds - ProcessedSecondsThisRun;
                    double step = Math.Min(StepSeconds, remaining);

                    try
                    {
                        ProcessGameStep(step, state, machine, room2);
                    }
                    catch (Exception exception)
                    {
                        Fail("Excepción durante el avance: " + exception.Message);
                        yield break;
                    }

                    ProcessedSecondsThisRun += step;
                    Progress01 = Mathf.Clamp01(
                        (float)(ProcessedSecondsThisRun / exactGameSeconds));
                    stepsSinceYield++;

                    if (stepsSinceYield >= StepsPerYield &&
                        ProcessedSecondsThisRun < exactGameSeconds)
                    {
                        stepsSinceYield = 0;
                        SetStatus(BuildProgressStatus(exactGameSeconds));
                        yield return null;
                    }
                }
                RefreshAfterAdvance();
            }

            Progress01 = 1f;
            if (!TrySave("al finalizar"))
                yield break;

            double leGained = Math.Max(0.0, state.LE - leBefore);
            double tracesGained = Math.Max(0.0, state.Traces - tracesBefore);
            string mode = exactGameSeconds >= AggregateOfflineThresholdSeconds
                ? "AUSENCIA AGREGADA"
                : "SIMULACIÓN ACTIVA";
            SetStatus("COMPLETADO: +" + FormatDuration(exactGameSeconds) +
                " | LE +" + FormatResource(leGained) +
                " | TRAZAS +" + FormatResource(tracesGained));
            Debug.Log("[QA Fast Forward] COMPLETE | modo=" + mode +
                " | segundos=" + ProcessedSecondsThisRun.ToString("0.###") +
                " | LE+=" + leGained.ToString("0.###") +
                " | Trazas+=" + tracesGained.ToString("0.###"));
        }
        finally
        {
            IsRunning = false;
            activeRoutine = null;
            SetControlsInteractable(true);
        }
    }

    private static void ProcessGameStep(
        double step, GameState state, MachineManager machine,
        Room2PanelUI room2)
    {
        // El paso ya representa segundos simulados exactos: no aplicar QA aquí.
        state.Tick(step);
        if (machine != null)
            machine.AdvanceAnalysis(step);
        if (room2 != null)
            room2.AdvanceQaFusionCooldown(step);
    }

    private static TriangleOfflineReport ApplyAggregateOfflineProgress(
        double seconds, GameState state, MachineManager machine,
        Room2PanelUI room2)
    {
        // Esta ruta QA prioriza obtener recursos base con el cálculo agregado
        // ya usado al regresar de una ausencia. No recorre un tick por segundo
        // ni aplica el límite normal de 12 h: +24 H debe representar 24 h QA.
        TriangleOfflineReport report = state.ApplyOfflineBaseProgress(seconds);
        if (machine != null)
            machine.ApplyOfflineAnalysis(seconds);
        if (room2 != null)
            room2.AdvanceQaFusionCooldown(seconds);
        return report;
    }

    private static void RefreshAfterAdvance()
    {
        if (GameState.I != null)
            GameState.I.ActualizarMaxLE();

        HUD hud = UnityEngine.Object.FindFirstObjectByType<HUD>(
            FindObjectsInactive.Include);
        if (hud != null)
            hud.RefreshNow();

        MachinePanelUI machinePanel =
            UnityEngine.Object.FindFirstObjectByType<MachinePanelUI>(
                FindObjectsInactive.Include);
        if (machinePanel != null)
            machinePanel.Refresh();

        TabsUI tabs = UnityEngine.Object.FindFirstObjectByType<TabsUI>(
            FindObjectsInactive.Include);
        if (tabs != null)
            tabs.RefreshGenerationLayoutFromOutside();

        Canvas.ForceUpdateCanvases();
    }

    private bool TrySave(string stage)
    {
        if (SaveService.I == null)
        {
            Fail("SaveService no está disponible " + stage + ".");
            return false;
        }

        try
        {
            if (SaveService.I.TrySave(out string error))
                return true;

            Fail("No se pudo guardar " + stage + ": " + error);
            return false;
        }
        catch (Exception exception)
        {
            Fail("Excepción al guardar " + stage + ": " + exception.Message);
            return false;
        }
    }

    private void Fail(string error)
    {
        LastError = error;
        SetStatus("ERROR: " + error);
        Debug.LogError("[QA Fast Forward] " + error);
    }

    private string BuildProgressStatus(double totalSeconds)
    {
        double remaining = Math.Max(0.0,
            totalSeconds - ProcessedSecondsThisRun);
        return "AVANCE QA: " + (Progress01 * 100f).ToString("0.0") +
            "% | RESTAN " + FormatDuration(remaining);
    }

    private static string FormatDuration(double seconds)
    {
        if (seconds >= 3600.0 && seconds % 3600.0 == 0.0)
            return (seconds / 3600.0).ToString("0") + " H";
        if (seconds >= 60.0 && seconds % 60.0 == 0.0)
            return (seconds / 60.0).ToString("0") + " MIN";
        return seconds.ToString("0.#") + " S";
    }

    private static string FormatResource(double value)
    {
        double safe = Math.Max(0.0, value);
        if (safe >= 1000000000.0)
            return (safe / 1000000000.0).ToString("0.##") + "B";
        if (safe >= 1000000.0)
            return (safe / 1000000.0).ToString("0.##") + "M";
        if (safe >= 1000.0)
            return (safe / 1000.0).ToString("0.##") + "K";
        return safe.ToString("0.##");
    }

    private void SetStatus(string status)
    {
        ResolvePanel();
        if (panel != null)
            panel.SetOperationStatus(status);
    }

    private void SetControlsInteractable(bool interactable)
    {
        ResolvePanel();
        if (panel != null)
            panel.SetQaControlsInteractable(interactable);
    }

    private void ResolvePanel()
    {
        if (panel == null)
        {
            panel = UnityEngine.Object.FindFirstObjectByType<QaPanelUI>(
                FindObjectsInactive.Include);
        }
    }

    private void Subscribe()
    {
        if (panel == null)
            return;
        panel.AdvanceRequested -= RequestAdvance;
        panel.AdvanceRequested += RequestAdvance;
    }

    private void Unsubscribe()
    {
        if (panel != null)
            panel.AdvanceRequested -= RequestAdvance;
    }
}

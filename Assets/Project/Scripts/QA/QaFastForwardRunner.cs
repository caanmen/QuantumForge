using System;
using System.Collections;
using UnityEngine;

public class QaFastForwardRunner : MonoBehaviour
{
    private const double StepSeconds = 1.0;
    private const int StepsPerYield = 50;

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

            Progress01 = 1f;
            if (!TrySave("al finalizar"))
                yield break;

            SetStatus("COMPLETADO: +" + FormatDuration(exactGameSeconds));
            Debug.Log("[QA Fast Forward] COMPLETE | segundos exactos=" +
                ProcessedSecondsThisRun.ToString("0.###"));
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

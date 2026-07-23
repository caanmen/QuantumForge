#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Dimension3Block7AValidation
{
    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Block 7A")]
    public static void ValidateBlock7A()
    {
        var failures = new List<string>();
        ValidateDynamicSpeed(failures);
        ValidateLegacyJobs(failures);
        ValidateBatchesAndPreview(failures);
        ValidateEveryQueueCanCancelSelected(failures);
        if (failures.Count == 0)
            Debug.Log("[D3 Block 7A] PASS | velocidad dinámica | lotes | preview | 4 colas");
        else
            Debug.LogError("[D3 Block 7A] FAIL\n- " + string.Join("\n- ", failures));
    }

    private static void ValidateDynamicSpeed(List<string> failures)
    {
        GameState state = CreateState("D3 Block 7A Dynamic");
        try
        {
            state.LE = 1000000.0;
            state.Traces = 10000.0;
            string reason;
            Check(D3ProductionSystem.TryQueuePartProduction(state,
                Dimension3Catalog.PartChassis, 1, 1L, out reason),
                "No crea trabajo dinámico: " + reason, failures);
            D3JobState job = D3JobQueueSystem.GetQueue(state.dimension3,
                Dimension3Catalog.QueuePartProduction).jobs[0];
            Check(job.usesDynamicBankSpeed && Close(job.remainingSeconds, 10.0),
                "El trabajo nuevo no conserva 10 unidades de trabajo base.", failures);
            Dimension3System.Tick(state, 2.0);
            Check(Close(job.remainingSeconds, 8.0),
                "Sin potencia, dos segundos no consumen dos unidades.", failures);
            state.dimension3.assignments.Add(PowerAssignment());
            double rate = D3PowerSystem.GetDynamicWorkRate(state.dimension3);
            double before = job.remainingSeconds;
            Dimension3System.Tick(state, 1.0);
            Check(rate > 1.0 && Close(job.remainingSeconds, before - rate),
                "La potencia añadida no acelera inmediatamente el trabajo activo.", failures);
            state.dimension3.assignments.Clear();
            before = job.remainingSeconds;
            Dimension3System.Tick(state, 1.0);
            Check(Close(job.remainingSeconds, before - 1.0),
                "Retirar potencia no actualiza inmediatamente la velocidad.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateLegacyJobs(List<string> failures)
    {
        GameState state = CreateState("D3 Block 7A Legacy");
        try
        {
            state.dimension3.assignments.Add(PowerAssignment());
            var legacy = new D3JobState
            {
                jobType = Dimension3Catalog.JobPartProduction,
                targetId = Dimension3Catalog.PartChassis,
                baseDurationSeconds = 10.0,
                remainingSeconds = 10.0,
                quantity = 1L,
                usesDynamicBankSpeed = false
            };
            string reason;
            Check(D3JobQueueSystem.EnqueueCommittedJob(state.dimension3,
                Dimension3Catalog.QueuePartProduction, legacy, out reason),
                "No prepara trabajo legado: " + reason, failures);
            Dimension3System.Tick(state, 1.0);
            Check(Close(legacy.remainingSeconds, 9.0),
                "Una partida antigua cambia dos veces la duración ya modificada.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateBatchesAndPreview(List<string> failures)
    {
        GameState state = CreateState("D3 Block 7A Batch");
        try
        {
            state.LE = 1000000.0;
            state.Traces = 10000.0;
            string reason;
            Check(D3ProductionSystem.TryQueuePartProduction(state,
                Dimension3Catalog.PartChassis, 1, 25L, out reason),
                "No acepta lote de 25 piezas: " + reason, failures);
            D3JobState batch = D3JobQueueSystem.GetQueue(state.dimension3,
                Dimension3Catalog.QueuePartProduction).jobs[0];
            Check(batch.quantity == 25L && Close(batch.baseDurationSeconds, 250.0),
                "El lote no conserva cantidad y trabajo base correctos.", failures);
            D3InventorySystem.AddParts(state.dimension3,
                Dimension3Catalog.PartChassis, 1, 3L);
            D3FactoryCostPreview preview =
                D3FactoryPreviewSystem.GetAssemblyPreview(state, 1, 5L);
            Check(preview != null && preview.missingPartsTotal == 22L &&
                Close(preview.TotalLE, preview.missingPartsLE + preview.assemblyLE),
                "La previsión no cuenta faltantes y total correctamente.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateEveryQueueCanCancelSelected(List<string> failures)
    {
        GameState state = CreateState("D3 Block 7A Queues");
        try
        {
            for (int i = 0; i < Dimension3Catalog.QueueIds.Length; i++)
            {
                string queueId = Dimension3Catalog.QueueIds[i];
                var active = Job(queueId, 0.0, 0.0);
                var pending = Job(queueId, 5.0, 2.0);
                string reason;
                D3JobQueueSystem.EnqueueCommittedJob(
                    state.dimension3, queueId, active, out reason);
                D3JobQueueSystem.EnqueueCommittedJob(
                    state.dimension3, queueId, pending, out reason);
                double beforeLE = state.LE;
                Check(D3JobQueueSystem.TryCancelJob(state, queueId,
                    pending.jobId, out reason),
                    "No cancela seleccionado en " + queueId, failures);
                Check(Close(state.LE, beforeLE + 5.0),
                    "No devuelve pendiente seleccionado en " + queueId, failures);
            }
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static D3AssignmentState PowerAssignment()
    {
        return new D3AssignmentState
        {
            installationId = Dimension3Catalog.FacilityProcessBank,
            channelId = Dimension3Catalog.ChannelProcessPower,
            mk = 6,
            traitId = Dimension3Catalog.TraitNormal,
            amount = 100L,
            stabilizedAmount = 100L
        };
    }

    private static D3JobState Job(string queueId, double le, double traces)
    {
        string type = queueId == Dimension3Catalog.QueuePartProduction
            ? Dimension3Catalog.JobPartProduction
            : queueId == Dimension3Catalog.QueueAssembly
                ? Dimension3Catalog.JobAssembly
                : queueId == Dimension3Catalog.QueueResearch
                    ? Dimension3Catalog.JobResearch
                    : Dimension3Catalog.JobFacilityUpgrade;
        return new D3JobState
        {
            jobType = type, targetId = queueId, remainingSeconds = 10.0,
            baseDurationSeconds = 10.0, paidLE = le, paidTraces = traces
        };
    }

    private static GameState CreateState(string name)
    {
        var go = new GameObject(name) { hideFlags = HideFlags.HideAndDontSave };
        go.SetActive(false);
        GameState state = go.AddComponent<GameState>();
        state.dimension03Unlocked = true;
        state.dimension3 = Dimension3System.CreateInitialState();
        Dimension3System.EnsureState(state);
        return state;
    }

    private static bool Close(double a, double b)
    {
        return Math.Abs(a - b) <= 0.0001;
    }

    private static void Check(bool condition, string message, List<string> failures)
    {
        if (!condition) failures.Add(message);
    }
}
#endif

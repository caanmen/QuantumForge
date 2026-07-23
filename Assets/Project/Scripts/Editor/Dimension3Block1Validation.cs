using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public static class Dimension3Block1Validation
{
    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Block 1 Core")]
    public static void ValidateBlock1Core()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("[D3 Block 1] Ejecutar fuera de Play Mode.");
            return;
        }

        var failures = new List<string>();
        ValidateInitializationAndMigration(failures);
        ValidateProductionAssemblyAndCancellation(failures);
        ValidateOfflineCap(failures);
        ValidateSaveDataJson(failures);

        if (failures.Count == 0)
        {
            Debug.Log(
                "[D3 Block 1] PASS | inicialización | producción V1 | " +
                "ensamblaje MK1 | cancelación | offline 12 h | JSON"
            );
            return;
        }

        Debug.LogError("[D3 Block 1] FAIL\n- " + string.Join("\n- ", failures));
    }

    public static void ValidateBlock1CoreBatch()
    {
        ValidateBlock1Core();
    }

    private static void ValidateInitializationAndMigration(List<string> failures)
    {
        GameState state = CreateTestState("D3 Initialization Validation");

        try
        {
            state.dimension03Unlocked = true;
            state.dimension3 = null;
            Dimension3System.EnsureState(state);

            Check(state.dimension3 != null && state.dimension3.initialized,
                "No inicializa D3 desbloqueada.", failures);
            Check(D3InventorySystem.GetAutomatonAmount(
                    state.dimension3, 1, Dimension3Catalog.TraitNormal) == 1L,
                "No concede exactamente un MK1 Normal inicial.", failures);
            Check(state.dimension3.queues.Count == 4,
                "No prepara las cuatro colas.", failures);

            Dimension3System.EnsureState(state);
            Check(D3InventorySystem.GetAutomatonAmount(
                    state.dimension3, 1, Dimension3Catalog.TraitNormal) == 1L,
                "EnsureState duplica el MK1 inicial.", failures);

            state.dimension3.parts = null;
            state.dimension3.queues = null;
            state.dimension3.facilities = null;
            Dimension3System.EnsureState(state);

            Check(state.dimension3.parts != null && state.dimension3.queues.Count == 4,
                "No normaliza listas ausentes de una partida antigua.", failures);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(state.gameObject);
        }
    }

    private static void ValidateProductionAssemblyAndCancellation(List<string> failures)
    {
        GameState state = CreateTestState("D3 Production Validation");

        try
        {
            state.dimension03Unlocked = true;
            state.dimension3 = Dimension3System.CreateInitialState();
            state.LE = 100000.0;
            state.Traces = 1000.0;
            Dimension3System.EnsureState(state);

            string reason;
            Check(Dimension3System.TryQueuePartProduction(
                    state, Dimension3Catalog.PartChassis, 1, 1L, out reason),
                "No permite producir Chasis V1: " + reason, failures);
            Check(Dimension3System.TryQueuePartProduction(
                    state, Dimension3Catalog.PartChassis, 1, 1L, out reason),
                "No permite una segunda entrada pendiente: " + reason, failures);

            D3QueueState partQueue = D3JobQueueSystem.GetQueue(
                state.dimension3,
                Dimension3Catalog.QueuePartProduction
            );
            Check(partQueue.jobs.Count == 2 && partQueue.jobs[0].started &&
                  !partQueue.jobs[1].started,
                "No distingue trabajo activo de pendiente.", failures);

            double leAfterTwoOrders = state.LE;
            double tracesAfterTwoOrders = state.Traces;
            string pendingJobId = partQueue.jobs[1].jobId;
            Check(Dimension3System.TryCancelJob(
                    state,
                    Dimension3Catalog.QueuePartProduction,
                    pendingJobId,
                    out reason),
                "No cancela una entrada pendiente: " + reason, failures);
            Check(AreClose(state.LE, leAfterTwoOrders + 100.0) &&
                  AreClose(state.Traces, tracesAfterTwoOrders + 1.0),
                "Cancelar pendiente no devuelve el costo completo.", failures);

            Dimension3System.Tick(state, 10.0);
            Check(D3InventorySystem.GetPartAmount(
                    state.dimension3, Dimension3Catalog.PartChassis, 1) == 1L,
                "El trabajo V1 no entrega la pieza al terminar.", failures);

            string[] remainingParts =
            {
                Dimension3Catalog.PartMotor,
                Dimension3Catalog.PartTool,
                Dimension3Catalog.PartControl,
                Dimension3Catalog.PartRegulator
            };

            for (int i = 0; i < remainingParts.Length; i++)
            {
                Check(Dimension3System.TryQueuePartProduction(
                        state, remainingParts[i], 1, 1L, out reason),
                    "No encola " + remainingParts[i] + " V1: " + reason, failures);
            }

            Dimension3System.ApplyOfflineProgress(state, 40.0);
            Check(D3InventorySystem.HasCompletePartSet(state.dimension3, 1, 1L),
                "El progreso offline no completa las cinco piezas.", failures);

            Check(Dimension3System.TryQueueNormalAssembly(
                    state, 1, 1L, out reason),
                "No permite ensamblar MK1 Normal: " + reason, failures);
            Check(!D3InventorySystem.HasCompletePartSet(state.dimension3, 1, 1L),
                "El ensamblaje no consume las cinco piezas al confirmarse.", failures);

            Dimension3System.Tick(state, 30.0);
            Check(D3InventorySystem.GetAutomatonAmount(
                    state.dimension3, 1, Dimension3Catalog.TraitNormal) == 2L,
                "El ensamblaje no entrega el segundo MK1 Normal.", failures);

            Check(Dimension3System.TryQueuePartProduction(
                    state, Dimension3Catalog.PartChassis, 1, 1L, out reason),
                "No prepara prueba de cancelación activa: " + reason, failures);
            partQueue = D3JobQueueSystem.GetQueue(
                state.dimension3,
                Dimension3Catalog.QueuePartProduction
            );
            double leAfterActiveCharge = state.LE;
            double tracesAfterActiveCharge = state.Traces;
            string activeJobId = partQueue.jobs[0].jobId;
            Check(Dimension3System.TryCancelJob(
                    state,
                    Dimension3Catalog.QueuePartProduction,
                    activeJobId,
                    out reason),
                "No cancela trabajo activo: " + reason, failures);
            Check(AreClose(state.LE, leAfterActiveCharge) &&
                  AreClose(state.Traces, tracesAfterActiveCharge),
                "Cancelar trabajo activo devuelve consumibles.", failures);

            Check(Dimension3System.ValidateState(state, out reason),
                "El estado final no valida: " + reason, failures);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(state.gameObject);
        }
    }

    private static void ValidateOfflineCap(List<string> failures)
    {
        GameState state = CreateTestState("D3 Offline Validation");

        try
        {
            state.dimension03Unlocked = true;
            state.dimension3 = Dimension3System.CreateInitialState();
            Dimension3System.EnsureState(state);

            D3JobState longJob = new D3JobState
            {
                jobType = Dimension3Catalog.JobPartProduction,
                targetId = Dimension3Catalog.PartChassis,
                version = 1,
                quantity = 1L,
                baseDurationSeconds = 50000.0,
                remainingSeconds = 50000.0
            };

            string reason;
            Check(D3JobQueueSystem.EnqueueCommittedJob(
                    state.dimension3,
                    Dimension3Catalog.QueuePartProduction,
                    longJob,
                    out reason),
                "No prepara trabajo offline: " + reason, failures);

            double applied = Dimension3System.ApplyOfflineProgress(state, 999999.0);
            Check(AreClose(applied, Dimension3Catalog.OfflineProgressCapSeconds),
                "No limita el offline a 12 horas.", failures);
            Check(AreClose(longJob.remainingSeconds, 6800.0),
                "El cap offline avanza una cantidad incorrecta.", failures);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(state.gameObject);
        }
    }

    private static void ValidateSaveDataJson(List<string> failures)
    {
        var data = new SaveData
        {
            dimension03Unlocked = true,
            dimension3 = Dimension3System.CreateInitialState()
        };
        data.dimension3.initialized = true;
        D3InventorySystem.AddAutomatons(
            data.dimension3,
            1,
            Dimension3Catalog.TraitNormal,
            7L
        );

        string json = JsonUtility.ToJson(data);
        SaveData loaded = JsonUtility.FromJson<SaveData>(json);

        Check(loaded != null && loaded.dimension3 != null,
            "SaveData JSON pierde el estado raíz D3.", failures);
        if (loaded != null && loaded.dimension3 != null)
        {
            Check(D3InventorySystem.GetAutomatonAmount(
                    loaded.dimension3, 1, Dimension3Catalog.TraitNormal) == 7L,
                "SaveData JSON pierde el inventario de autómatas.", failures);
        }
    }

    private static GameState CreateTestState(string name)
    {
        var gameObject = new GameObject(name)
        {
            hideFlags = HideFlags.HideAndDontSave
        };
        gameObject.SetActive(false);
        return gameObject.AddComponent<GameState>();
    }

    private static bool AreClose(double left, double right)
    {
        return Math.Abs(left - right) <= 0.0001;
    }

    private static void Check(bool condition, string message, List<string> failures)
    {
        if (!condition)
            failures.Add(message);
    }
}

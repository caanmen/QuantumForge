#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public static class Dimension3Block2Validation
{
    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Block 2 Core")]
    public static void ValidateBlock2Core()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("[D3 Block 2] Ejecutar fuera de Play Mode.");
            return;
        }

        var failures = new List<string>();
        ValidateAssignmentsAndPower(failures);
        ValidateLevelsAndUnlocks(failures);
        ValidateSharedPreviewFormula(failures);

        if (failures.Count == 0)
        {
            Debug.Log(
                "[D3 Block 2] PASS | disponibilidad | asignaciones | " +
                "estabilización 30 s | potencia | niveles 2-3 | V2-V3 | previews"
            );
        }
        else
        {
            Debug.LogError("[D3 Block 2] FAIL\n- " + string.Join("\n- ", failures));
        }
    }

    private static void ValidateAssignmentsAndPower(List<string> failures)
    {
        GameState state = CreateState("D3 Block 2 Assignments");
        try
        {
            D3InventorySystem.AddAutomatons(
                state.dimension3, 1, Dimension3Catalog.TraitNormal, 9L);
            string reason;
            Check(!Dimension3System.TrySetProcessBankAssignment(
                    state, Dimension3Catalog.ChannelProcessPower, 1,
                    Dimension3Catalog.TraitNormal, 11L, out reason),
                "Permite asignar más autómatas de los disponibles.", failures);
            Check(Dimension3System.TrySetProcessBankAssignment(
                    state, Dimension3Catalog.ChannelProcessPower, 1,
                    Dimension3Catalog.TraitNormal, 10L, out reason),
                "No permite asignar los 10 MK1 disponibles: " + reason, failures);
            Check(D3InventorySystem.GetAvailableAutomatonAmount(
                    state.dimension3, 1, Dimension3Catalog.TraitNormal) == 0L,
                "Un autómata asignado continúa apareciendo libre.", failures);
            Check(AreClose(D3PowerSystem.GetRawChannelPower(
                    state.dimension3, Dimension3Catalog.FacilityProcessBank,
                    Dimension3Catalog.ChannelProcessPower), 0.0),
                "Una asignación aporta potencia antes de estabilizarse.", failures);

            Dimension3System.Tick(state, 29.0);
            Check(AreClose(D3PowerSystem.GetRawChannelPower(
                    state.dimension3, Dimension3Catalog.FacilityProcessBank,
                    Dimension3Catalog.ChannelProcessPower), 0.0),
                "La estabilización termina antes de 30 segundos.", failures);
            Dimension3System.Tick(state, 1.0);
            Check(AreClose(D3PowerSystem.GetRawChannelPower(
                    state.dimension3, Dimension3Catalog.FacilityProcessBank,
                    Dimension3Catalog.ChannelProcessPower), 12.5),
                "Potencia o bonificación de canal correcto incorrecta.", failures);
            Check(D3PowerSystem.GetProcessBankModifiers(
                    state.dimension3).progressMultiplier > 1.0,
                "La potencia estabilizada no mejora el progreso.", failures);

            Check(Dimension3System.TrySetProcessBankAssignment(
                    state, Dimension3Catalog.ChannelProcessPower, 1,
                    Dimension3Catalog.TraitNormal, 4L, out reason),
                "No permite retirar autómatas: " + reason, failures);
            Check(AreClose(D3PowerSystem.GetRawChannelPower(
                    state.dimension3, Dimension3Catalog.FacilityProcessBank,
                    Dimension3Catalog.ChannelProcessPower), 5.0),
                "La retirada no reduce la potencia de inmediato.", failures);
            Check(D3InventorySystem.GetAvailableAutomatonAmount(
                    state.dimension3, 1, Dimension3Catalog.TraitNormal) == 6L,
                "La retirada no libera autómatas de inmediato.", failures);
            Check(Dimension3System.ValidateState(state, out reason),
                "El estado de asignaciones no valida: " + reason, failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateLevelsAndUnlocks(List<string> failures)
    {
        GameState state = CreateState("D3 Block 2 Levels");
        try
        {
            state.LE = 1000000.0;
            state.Traces = 10000.0;
            string reason;
            Check(!Dimension3System.TryQueueProcessBankUpgrade(state, out reason),
                "Permite nivel 2 sin 10 MK1 ensamblados.", failures);
            D3InventorySystem.AddAssemblyCount(state.dimension3, 1, 10L);
            Check(Dimension3System.TryQueueProcessBankUpgrade(state, out reason),
                "No encola Banco nivel 2: " + reason, failures);
            Dimension3System.Tick(state, 10000.0);
            Check(D3FacilitySystem.GetProcessBankLevel(state.dimension3) == 2,
                "No completa Banco nivel 2.", failures);
            Check(D3FacilitySystem.IsProcessBankChannelUnlocked(
                    state.dimension3, Dimension3Catalog.ChannelProcessTime),
                "Nivel 2 no habilita Ritmo Operativo.", failures);
            Check(!D3ProductionSystem.IsPartVersionUnlocked(state, 2),
                "V2 se desbloquea antes de 25 MK1 ensamblados.", failures);
            D3InventorySystem.AddAssemblyCount(state.dimension3, 1, 15L);
            Check(D3ProductionSystem.IsPartVersionUnlocked(state, 2),
                "V2 no se desbloquea con nivel 2 y 25 MK1.", failures);

            D3InventorySystem.AddAssemblyCount(state.dimension3, 2, 10L);
            Check(Dimension3System.TryQueueProcessBankUpgrade(state, out reason),
                "No encola Banco nivel 3: " + reason, failures);
            Dimension3System.Tick(state, 10000.0);
            Check(D3FacilitySystem.GetProcessBankLevel(state.dimension3) == 3,
                "No completa Banco nivel 3.", failures);
            Check(D3FacilitySystem.IsProcessBankChannelUnlocked(
                    state.dimension3, Dimension3Catalog.ChannelProcessCost),
                "Nivel 3 no habilita Ahorro Energético.", failures);
            Check(!D3ProductionSystem.IsPartVersionUnlocked(state, 3),
                "V3 se desbloquea antes de 25 MK2 ensamblados.", failures);
            D3InventorySystem.AddAssemblyCount(state.dimension3, 2, 15L);
            Check(D3ProductionSystem.IsPartVersionUnlocked(state, 3),
                "V3 no se desbloquea con nivel 3 y 25 MK2.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateSharedPreviewFormula(List<string> failures)
    {
        GameState state = CreateState("D3 Block 2 Preview");
        try
        {
            D3FacilityState bank = D3FacilitySystem.GetFacility(
                state.dimension3, Dimension3Catalog.FacilityProcessBank);
            bank.level = 3;
            D3InventorySystem.AddAutomatons(
                state.dimension3, 3, Dimension3Catalog.TraitEfficient, 100L);
            string reason;
            Check(Dimension3System.TrySetProcessBankAssignment(
                    state, Dimension3Catalog.ChannelProcessCost, 3,
                    Dimension3Catalog.TraitEfficient, 100L, out reason),
                "No prepara canal de costo: " + reason, failures);
            Dimension3System.Tick(state, 30.0);
            state.LE = 1000000.0;
            state.Traces = 10000.0;

            double expectedLE = D3PowerSystem.GetModifiedCost(
                state.dimension3, 100.0);
            double expectedTraces = D3PowerSystem.GetModifiedCost(
                state.dimension3, 1.0);
            double expectedDuration = D3PowerSystem.GetModifiedDuration(
                state.dimension3, 10.0);
            Check(Dimension3System.TryQueuePartProduction(
                    state, Dimension3Catalog.PartChassis, 1, 1L, out reason),
                "No encola pieza para validar preview: " + reason, failures);
            D3JobState job = D3JobQueueSystem.GetQueue(
                state.dimension3, Dimension3Catalog.QueuePartProduction).jobs[0];
            Check(AreClose(job.paidLE, expectedLE) &&
                  AreClose(job.paidTraces, expectedTraces) &&
                  AreClose(job.baseDurationSeconds, 10.0) &&
                  job.usesDynamicBankSpeed &&
                  AreClose(job.baseDurationSeconds /
                      D3PowerSystem.GetDynamicWorkRate(state.dimension3),
                      expectedDuration),
                "El trabajo no usa las mismas fórmulas que el preview.", failures);
            Check(job.paidLE > 0.0 && job.paidTraces > 0.0 &&
                  job.baseDurationSeconds >= 0.1,
                "Costo o tiempo alcanza cero o un valor inválido.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static GameState CreateState(string name)
    {
        var gameObject = new GameObject(name) { hideFlags = HideFlags.HideAndDontSave };
        gameObject.SetActive(false);
        GameState state = gameObject.AddComponent<GameState>();
        state.dimension03Unlocked = true;
        state.dimension3 = Dimension3System.CreateInitialState();
        Dimension3System.EnsureState(state);
        return state;
    }

    private static bool AreClose(double left, double right)
    {
        return Math.Abs(left - right) <= 0.0001;
    }

    private static void Check(bool condition, string message, List<string> failures)
    {
        if (!condition) failures.Add(message);
    }
}
#endif

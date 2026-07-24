#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Prestige1ProgressionValidation
{
    [MenuItem("Tools/Quantum Forge/Convergence/Validate Prestige 1 Discovery")]
    public static void ValidatePrestige1Discovery()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError(
                "[Prestige 1 Discovery] Ejecutar fuera de Play Mode."
            );
            return;
        }

        var failures = new List<string>();
        int[][] orders =
        {
            new[] { 1, 2, 3 }, new[] { 1, 3, 2 },
            new[] { 2, 1, 3 }, new[] { 2, 3, 1 },
            new[] { 3, 1, 2 }, new[] { 3, 2, 1 }
        };

        foreach (int[] order in orders)
            ValidateDiscoveryOrder(order, failures);

        if (failures.Count == 0)
        {
            Debug.Log(
                "[Prestige 1 Discovery] PASS | 6 órdenes | " +
                "bloqueos | preservación | reset base | cierre"
            );
            return;
        }

        Debug.LogError(
            "[Prestige 1 Discovery] FAIL\n- " +
            string.Join("\n- ", failures)
        );
    }

    public static void ValidatePrestige1DiscoveryBatch()
    {
        ValidatePrestige1Discovery();
    }

    private static void ValidateDiscoveryOrder(
        int[] order,
        List<string> failures)
    {
        string orderName = string.Join("→", order);
        GameState state = CreateState("P1 Discovery " + orderName);
        GameObject machineObject = new GameObject("P1 Machine " + orderName)
        {
            hideFlags = HideFlags.HideAndDontSave
        };
        machineObject.SetActive(false);
        MachineManager machine = machineObject.AddComponent<MachineManager>();

        try
        {
            for (int step = 0; step < order.Length; step++)
            {
                int selectedDimension = order[step];
                PrepareMachineForPrestige(machine);

                Check(
                    state.CanDoPrestige1(machine),
                    orderName + ": el Prestigio " + (step + 1) +
                    " no se habilita con Máquina preparada.",
                    failures
                );

                Check(
                    state.DoPrestige1Reset(selectedDimension, machine),
                    orderName + ": no se pudo elegir D" + selectedDimension +
                    " en el Prestigio " + (step + 1) + ".",
                    failures
                );

                Check(
                    state.prestige1Count == step + 1 &&
                    state.prestige1CurrentDimensionId == selectedDimension,
                    orderName + ": contador o dimensión actual incorrectos " +
                    "tras el Prestigio " + (step + 1) + ".",
                    failures
                );

                ValidateUnlockedDimensions(
                    state,
                    order,
                    step + 1,
                    orderName,
                    failures
                );

                Check(
                    Near(state.LE, 10.0) && Near(state.Traces, 0.0) &&
                    !machine.MachineUnlocked,
                    orderName + ": el reset base o el reset operativo de " +
                    "Máquina es incorrecto en el paso " + (step + 1) + ".",
                    failures
                );

                if (step < order.Length - 1)
                {
                    Check(
                        !state.CanDoPrestige1(machine),
                        orderName + ": permite otro Prestigio sin completar " +
                        "el hito de D" + selectedDimension + ".",
                        failures
                    );

                    CompleteDimensionMilestone(state, selectedDimension);
                    Check(
                        state.HasDimensionMilestoneForNextPrestige1(),
                        orderName + ": no reconoce el hito de D" +
                        selectedDimension + ".",
                        failures
                    );
                }
                else
                {
                    CompleteDimensionMilestone(state, selectedDimension);
                }
            }

            Check(
                state.IsPrestige1CycleComplete() &&
                !state.HasAvailableDimensionForPrestige1Selection(),
                orderName + ": no cierra el ciclo tras los tres hitos.",
                failures
            );
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(machineObject);
            UnityEngine.Object.DestroyImmediate(state.gameObject);
        }
    }

    private static void ValidateUnlockedDimensions(
        GameState state,
        int[] order,
        int unlockedCount,
        string orderName,
        List<string> failures)
    {
        for (int dimensionId = 1; dimensionId <= 3; dimensionId++)
        {
            bool expectedUnlocked = false;
            for (int i = 0; i < unlockedCount; i++)
            {
                if (order[i] == dimensionId)
                {
                    expectedUnlocked = true;
                    break;
                }
            }

            Check(
                state.IsDimensionUnlockedAfterPrestige1(dimensionId) ==
                expectedUnlocked,
                orderName + ": D" + dimensionId + " tiene un desbloqueo " +
                "incorrecto tras " + unlockedCount + " elección(es).",
                failures
            );
        }
    }

    private static void CompleteDimensionMilestone(
        GameState state,
        int dimensionId)
    {
        switch (dimensionId)
        {
            case 1:
                state.dimension1GalacticAnchorDiscovered = true;
                break;
            case 2:
                Dimension2System.EnsureState(state);
                state.dimension2.civilization2.entityContained = true;
                state.dimension2.civilization2.majorPactPrepared = true;
                state.dimension2.civilization2.majorPactEstablished = true;
                break;
            case 3:
                Dimension3System.EnsureState(state);
                state.dimension3.autonomyCoreIntegrated = true;
                break;
        }
    }

    private static void PrepareMachineForPrestige(MachineManager machine)
    {
        List<MachineNodeDef> visibleNodes = machine.GetAllNodes(false);
        int requiredRepairCount = Mathf.CeilToInt(visibleNodes.Count * 0.80f);
        var repairedNodeIds = new List<string>();

        for (int i = 0; i < visibleNodes.Count &&
             repairedNodeIds.Count < requiredRepairCount; i++)
        {
            MachineNodeDef node = visibleNodes[i];
            if (node != null && node.id != "z3_convergence_channel")
                repairedNodeIds.Add(node.id);
        }

        repairedNodeIds.Add("z3_convergence_channel");
        machine.LoadProgressFromSave(new SaveData
        {
            machineIntroSeen = true,
            machineUnlocked = true,
            machineAllZonesUnlocked = true,
            machineRepairedNodeIds = repairedNodeIds
        });
    }

    private static GameState CreateState(string name)
    {
        var gameObject = new GameObject(name)
        {
            hideFlags = HideFlags.HideAndDontSave
        };
        gameObject.SetActive(false);
        return gameObject.AddComponent<GameState>();
    }

    private static bool Near(double a, double b)
    {
        return Math.Abs(a - b) < 0.000001;
    }

    private static void Check(
        bool condition,
        string message,
        List<string> failures)
    {
        if (!condition)
            failures.Add(message);
    }
}
#endif

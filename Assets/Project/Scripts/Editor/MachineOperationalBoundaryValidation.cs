#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class MachineOperationalBoundaryValidation
{
    [MenuItem("Tools/Quantum Forge/Convergence/Validate Machine Operational Boundary")]
    public static void ValidateMachineOperationalBoundary()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("[Machine Boundary] Ejecutar fuera de Play Mode.");
            return;
        }

        var failures = new List<string>();
        ValidateOperationalReset(failures);
        ValidatePermanentStateIsNotDuplicated(failures);

        if (failures.Count == 0)
        {
            Debug.Log("[Machine Boundary] PASS | operativo reiniciado | núcleo conservado");
            return;
        }

        Debug.LogError("[Machine Boundary] FAIL\n- " +
            string.Join("\n- ", failures));
    }

    public static void ValidateMachineOperationalBoundaryBatch()
    {
        ValidateMachineOperationalBoundary();
    }

    private static void ValidateOperationalReset(List<string> failures)
    {
        GameObject machineObject = new GameObject("Machine Operational Boundary");
        MachineManager machine = machineObject.AddComponent<MachineManager>();
        try
        {
            machine.MarkIntroSeenAndUnlockMachine();
            SaveData beforeReset = new SaveData();
            machine.WriteProgressToSave(beforeReset);

            machine.ResetOperationalProgress();
            SaveData afterReset = new SaveData();
            machine.WriteProgressToSave(afterReset);

            Check(beforeReset.machineUnlocked && beforeReset.machineIntroSeen,
                "La preparación de la Máquina no pudo establecerse para la prueba.",
                failures);
            Check(!afterReset.machineUnlocked && !afterReset.machineIntroSeen &&
                  !afterReset.machineFusionPanelUnlocked &&
                  !afterReset.machineAllZonesUnlocked &&
                  afterReset.machineRepairedNodeIds.Count == 0 &&
                  afterReset.machineAnalyzedNodeIds.Count == 0 &&
                  string.IsNullOrEmpty(afterReset.machineAnalysisNodeId) &&
                  afterReset.machineAnalysisRemainingSeconds == 0.0,
                "El reset operativo deja datos reconstruibles de la Máquina.",
                failures);
        }
        finally
        {
            Object.DestroyImmediate(machineObject);
        }
    }

    private static void ValidatePermanentStateIsNotDuplicated(
        List<string> failures)
    {
        ConvergenceState core = new ConvergenceState
        {
            progressVersion = ConvergenceSystem.ProgressVersion,
            phase = ConvergencePhase.ConfigurationPending,
            completedCycles = 2
        };
        SaveData save = new SaveData { convergence = core };
        SaveData roundTrip = JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(save));

        Check(roundTrip != null && roundTrip.convergence != null &&
              roundTrip.convergence.phase == ConvergencePhase.ConfigurationPending &&
              roundTrip.convergence.completedCycles == 2,
            "El Núcleo permanente no queda separado y persistente en ConvergenceState.",
            failures);
        Check(typeof(MachineManager).GetField("_repairedNodeIds",
                  System.Reflection.BindingFlags.NonPublic |
                  System.Reflection.BindingFlags.Instance) != null &&
              typeof(ConvergenceState).GetField("repairedNodeIds") == null &&
              typeof(ConvergenceState).GetField("analyzedNodeIds") == null,
            "ConvergenceState duplica el progreso operativo de MachineManager.",
            failures);
    }

    private static void Check(bool condition, string message, List<string> failures)
    {
        if (!condition)
            failures.Add(message);
    }
}
#endif

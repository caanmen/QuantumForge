#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ConvergenceEndpointValidation
{
    [MenuItem("Tools/Quantum Forge/Convergence/Validate C6 Endpoint")]
    public static void ValidateEndpoint()
    {
        var failures = new List<string>();
        var placements = new List<ConvergenceCircuitPlacement>
        {
            P("convergence_circuit_004_triaxial_distributor", 0, 1, 180),
            P("convergence_circuit_005_local_coupler", 1, 1, 270),
            P("convergence_circuit_006_closure_matrix", 2, 1, 0),
            P("convergence_circuit_003_resonant_elbow", 2, 0, 270),
            P("convergence_circuit_002_axial_link", 1, 0, 90),
            P(ConvergenceCircuitCatalog.StartupPulseCircuitId, -1, 0, 270)
        };
        ConvergenceBoardResolution valid = ConvergenceCircuitResolver.ResolveBoard(placements);
        Check(valid.structurallyValid && valid.activeCircuitIds.Count == 6, "C6 no resuelve 6/6 energizados.", failures);
        Check(Near(valid.candidateSnapshot.baseLEProductionMultiplier, 1.35) &&
              Near(valid.candidateSnapshot.baseTracesProductionMultiplier, 1.30) &&
              Near(valid.candidateSnapshot.stabilityGainMultiplier, 1.15),
            "El snapshot final C6 no coincide con los canales aprobados.", failures);
        placements.RemoveAt(5);
        ConvergenceBoardResolution invalid = ConvergenceCircuitResolver.ResolveBoard(placements);
        // Sin C1 quedan C4 (+0.12) y su amplificación C5 (+0.05). C6 sigue
        // energizado físicamente, pero su bono de cierre (+0.08) debe apagarse.
        Check(invalid.activeCircuitIds.Count < 6 && Near(invalid.candidateSnapshot.baseLEProductionMultiplier, 1.17),
            "C6 aplica su cierre sin seis circuitos energizados.", failures);

        var go = new GameObject("C6 Endpoint State") { hideFlags = HideFlags.HideAndDontSave };
        go.SetActive(false);
        GameState state = go.AddComponent<GameState>();
        state.convergence = ConvergenceSystem.CreateInitialState();
        state.convergence.completedCycles = 6;
        state.convergence.nextAwardOrdinal = 7;
        state.convergence.normalConvergenceCompleted = true;
        state.convergence.unknownSignalTriggered = true;
        Check(!ConvergenceCircuitSystem.HasNextDesignedCircuit(state) &&
              state.convergence.unknownSignalTriggered,
            "El endpoint permite C7 o pierde la señal desconocida.", failures);
        Object.DestroyImmediate(go);

        if (failures.Count == 0) Debug.Log("[Convergence Endpoint] PASS | C6 6/6 | snapshot | sin C7 | ???");
        else Debug.LogError("[Convergence Endpoint] FAIL\n- " + string.Join("\n- ", failures));
    }

    public static void ValidateEndpointBatch() { ValidateEndpoint(); }
    private static ConvergenceCircuitPlacement P(string id,int x,int y,int r) => new ConvergenceCircuitPlacement { circuitId=id,x=x,y=y,rotationDegrees=r };
    private static bool Near(double a,double b) => System.Math.Abs(a-b)<0.0001;
    private static void Check(bool condition,string message,List<string> failures){if(!condition)failures.Add(message);}
}
#endif

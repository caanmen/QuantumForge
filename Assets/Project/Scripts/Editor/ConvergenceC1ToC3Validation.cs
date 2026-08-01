#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public static class ConvergenceC1ToC3Validation
{
 [MenuItem("Tools/Quantum Forge/Convergence/Validate C1 to C3")]
 public static void Validate(){var f=new List<string>(); var c1=ConvergenceCircuitCatalog.StartupPulseCircuitId; var c2="convergence_circuit_002_axial_link"; var c3="convergence_circuit_003_resonant_elbow"; var r2=ConvergenceCircuitResolver.ResolveBoard(new List<ConvergenceCircuitPlacement>{new ConvergenceCircuitPlacement{circuitId=c2,x=0,y=1,rotationDegrees=0},new ConvergenceCircuitPlacement{circuitId=c1,x=0,y=2,rotationDegrees=0}}); if(System.Math.Abs(r2.candidateSnapshot.baseTracesProductionMultiplier-1.1)>.0001)f.Add("C2 no aporta Trazas."); var r3=ConvergenceCircuitResolver.ResolveBoard(new List<ConvergenceCircuitPlacement>{new ConvergenceCircuitPlacement{circuitId=c3,x=0,y=1,rotationDegrees=180}}); if(System.Math.Abs(r3.candidateSnapshot.stabilityGainMultiplier-1.1)>.0001)f.Add("C3 no aporta estabilidad."); if(f.Count==0)Debug.Log("[Convergence C1-C3] PASS | C2 Trazas | C3 estabilidad");else Debug.LogError("[Convergence C1-C3] FAIL\n- "+string.Join("\n- ",f));}
 public static void ValidateBatch(){Validate();}
}
#endif

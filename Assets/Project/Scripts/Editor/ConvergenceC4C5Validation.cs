#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public static class ConvergenceC4C5Validation
{
 [MenuItem("Tools/Quantum Forge/Convergence/Validate C4 C5")]
 public static void Validate(){var f=new List<string>();var p=new List<ConvergenceCircuitPlacement>{P("convergence_circuit_004_triaxial_distributor",0,1,180),P("convergence_circuit_005_local_coupler",1,1,270),P("convergence_circuit_006_closure_matrix",2,1,0),P("convergence_circuit_003_resonant_elbow",2,0,270),P("convergence_circuit_002_axial_link",1,0,90),P(ConvergenceCircuitCatalog.StartupPulseCircuitId,-1,0,270)};var amplified=ConvergenceCircuitResolver.ResolveBoard(p);p[1].rotationDegrees=90;var plain=ConvergenceCircuitResolver.ResolveBoard(p);if(System.Math.Abs(amplified.candidateSnapshot.baseLEProductionMultiplier-plain.candidateSnapshot.baseLEProductionMultiplier-.05)>.001)f.Add("C5 no amplifica una vez el canal LE primario de C4.");if(System.Math.Abs(amplified.candidateSnapshot.baseTracesProductionMultiplier-plain.candidateSnapshot.baseTracesProductionMultiplier)>.001)f.Add("C5 copia indebidamente el canal secundario de C4.");if(f.Count==0)Debug.Log("[Convergence C4-C5] PASS | bifurcación | acoplador independiente");else Debug.LogError("[Convergence C4-C5] FAIL\n- "+string.Join("\n- ",f));}
 private static ConvergenceCircuitPlacement P(string id,int x,int y,int r)=>new ConvergenceCircuitPlacement{circuitId=id,x=x,y=y,rotationDegrees=r};
 public static void ValidateBatch(){Validate();}
}
#endif

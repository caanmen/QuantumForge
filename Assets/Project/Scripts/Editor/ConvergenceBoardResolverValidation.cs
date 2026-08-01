#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public static class ConvergenceBoardResolverValidation
{
 [MenuItem("Tools/Quantum Forge/Convergence/Validate Board Resolver")]
 public static void ValidateBoardResolver(){var f=new List<string>(); var c1=ConvergenceCircuitCatalog.StartupPulseCircuitId; var c2="convergence_circuit_002_axial_link"; var a=ConvergenceCircuitResolver.ResolveBoard(new List<ConvergenceCircuitPlacement>{new ConvergenceCircuitPlacement{circuitId=c1,x=0,y=1,rotationDegrees=0}}); if(!a.activeCircuitIds.Contains(c1)||a.candidateSnapshot.baseLEProductionMultiplier!=1.1)f.Add("C1 conectado."); var b=ConvergenceCircuitResolver.ResolveBoard(new List<ConvergenceCircuitPlacement>{new ConvergenceCircuitPlacement{circuitId=c1,x=0,y=2,rotationDegrees=0}}); if(b.activeCircuitIds.Count!=0)f.Add("Desconexión."); var l=new List<ConvergenceCircuitPlacement>{new ConvergenceCircuitPlacement{circuitId=c2,x=0,y=1,rotationDegrees=0},new ConvergenceCircuitPlacement{circuitId=c1,x=0,y=2,rotationDegrees=0}}; var x=ConvergenceCircuitResolver.ResolveBoard(l); if(x.activeCircuitIds.Count!=2)f.Add("Línea."); l.Reverse(); if(x.layoutHash!=ConvergenceCircuitResolver.ResolveBoard(l).layoutHash)f.Add("Determinismo."); if(f.Count==0)Debug.Log("[Convergence Board] PASS | línea | desconexión | determinismo");else Debug.LogError("[Convergence Board] FAIL\n- "+string.Join("\n- ",f));}
 public static void ValidateBoardResolverBatch(){ValidateBoardResolver();}
}
#endif

using System;
using System.Collections.Generic;
using System.Linq;

public class ResolvedConvergenceCircuit
{
    public string circuitId; public int x; public int y; public bool placed; public int rotationDegrees;
    public int portMask; public bool energized; public int degree; public int distance = -1; public string inactiveReason;
}
public class ConvergenceBoardResolution
{
    public bool structurallyValid = true; public List<string> errors = new List<string>();
    public List<ResolvedConvergenceCircuit> circuits = new List<ResolvedConvergenceCircuit>();
    public List<string> activeCircuitIds = new List<string>(); public ConvergenceModifierSnapshot candidateSnapshot = new ConvergenceModifierSnapshot(); public string layoutHash = "";
}

public static class ConvergenceCircuitResolver
{
    private static readonly int[] Dx = { 0, 1, 0, -1 }, Dy = { 1, 0, -1, 0 }, Bit = { 1, 2, 4, 8 }, Opposite = { 4, 8, 1, 2 };
    public static ConvergenceBoardResolution ResolveBoard(List<ConvergenceCircuitPlacement> placements)
    {
        var result = new ConvergenceBoardResolution(); var cells = new Dictionary<string, ConvergenceCircuitPlacement>(); var circuitIds = new HashSet<string>();
        foreach (var p in (placements ?? new List<ConvergenceCircuitPlacement>()).OrderBy(p => p?.circuitId).ThenBy(p => p?.x).ThenBy(p => p?.y))
        {
            string key = p == null ? "" : Key(p.x, p.y);
            if (p == null || ConvergenceCircuitCatalog.Get(p.circuitId) == null || !ConvergenceCircuitCatalog.IsValidRotation(p.rotationDegrees) || !ConvergenceCircuitCatalog.IsValidBoardCoordinate(p.x,p.y) || (p.x == 0 && p.y == 0) || cells.ContainsKey(key) || !circuitIds.Add(p.circuitId)) { result.structurallyValid = false; result.errors.Add("Placement inválido, circuito duplicado o colisión."); continue; }
            cells[key] = p;
        }
        var resolved = new Dictionary<string, ResolvedConvergenceCircuit>();
        foreach (var pair in cells)
        {
            var p = pair.Value; var c = new ResolvedConvergenceCircuit { circuitId=p.circuitId, x=p.x, y=p.y, placed=true, rotationDegrees=p.rotationDegrees, portMask=ConvergenceCircuitCatalog.RotateMask(ConvergenceCircuitCatalog.Get(p.circuitId).basePortMask,p.rotationDegrees) };
            resolved[pair.Key] = c; result.circuits.Add(c);
        }
        var queue = new Queue<string>(); var distances = new Dictionary<string,int> { [Key(0,0)] = 0 }; queue.Enqueue(Key(0,0));
        while (queue.Count > 0)
        {
            string key = queue.Dequeue(); Parse(key, out int x, out int y); int mask = key == Key(0,0) ? 15 : resolved[key].portMask;
            for (int i=0;i<4;i++) if ((mask & Bit[i]) != 0)
            {
                string neighborKey=Key(x+Dx[i],y+Dy[i]);
                if (!resolved.TryGetValue(neighborKey,out var neighbor) || (neighbor.portMask & Opposite[i]) == 0) continue;
                if (distances.ContainsKey(neighborKey)) continue; distances[neighborKey]=distances[key]+1; queue.Enqueue(neighborKey);
            }
        }
        // Cada circuito cuenta sus enlaces recíprocos una sola vez. El recorrido BFS
        // visita ambos sentidos de una arista, por lo que incrementar allí duplicaba degree.
        foreach (var pair in resolved)
        {
            ResolvedConvergenceCircuit circuit = pair.Value;
            for (int i = 0; i < 4; i++)
            {
                if ((circuit.portMask & Bit[i]) == 0) continue;
                string neighborKey = Key(circuit.x + Dx[i], circuit.y + Dy[i]);
                if (neighborKey == Key(0, 0))
                {
                    circuit.degree++;
                    continue;
                }
                if (resolved.TryGetValue(neighborKey, out var neighbor) &&
                    (neighbor.portMask & Opposite[i]) != 0)
                    circuit.degree++;
            }
        }
        foreach (var pair in resolved.OrderBy(p=>p.Key)) { if (distances.TryGetValue(pair.Key,out int d)) { pair.Value.energized=true; pair.Value.distance=d; result.activeCircuitIds.Add(pair.Value.circuitId); } else pair.Value.inactiveReason="Sin ruta recíproca al Núcleo."; }
        double le=0, traces=0, stability=0;
        bool closureReady = result.structurallyValid && ConvergenceCircuitCatalog.Definitions.All(d => result.circuits.Any(c => c.circuitId == d.id && c.energized));
        foreach (var c in result.circuits.Where(c=>c.energized)) { var d=ConvergenceCircuitCatalog.Get(c.circuitId); if (d.requiresAllOwnedPowered && !closureReady) continue; le+=d.baseLEBonus; traces+=d.baseTracesBonus; stability+=d.stabilityBonus; }
        // C5: apunta al vecino en la dirección de su flecha (la misma rotación que su pieza).
        foreach (var c5 in result.circuits.Where(c=>c.energized && ConvergenceCircuitCatalog.Get(c.circuitId).isAmplifier))
        {
            ConvergenceCircuitDefinition c5Definition = ConvergenceCircuitCatalog.Get(c5.circuitId);
            int arrow = ConvergenceCircuitCatalog.RotateMask(c5Definition.baseTargetDirectionMask,c5.rotationDegrees); int i=Array.IndexOf(Bit,arrow); string targetKey=Key(c5.x+Dx[i],c5.y+Dy[i]);
            if (!resolved.TryGetValue(targetKey,out var target) || !target.energized) continue; var targetDef=ConvergenceCircuitCatalog.Get(target.circuitId); if (targetDef.isAmplifier || targetDef.id=="convergence_circuit_006_closure_matrix") continue;
            if (targetDef.primaryChannel==ConvergenceEffectChannel.BaseLE) le+=.05; else if(targetDef.primaryChannel==ConvergenceEffectChannel.BaseTraces) traces+=.05; else stability+=.05;
        }
        result.candidateSnapshot.baseLEProductionMultiplier=1+Math.Min(1,Math.Max(0,le)); result.candidateSnapshot.baseTracesProductionMultiplier=1+Math.Min(1,Math.Max(0,traces)); result.candidateSnapshot.stabilityGainMultiplier=1+Math.Min(.5,Math.Max(0,stability)); result.candidateSnapshot.activeCircuitIds=result.activeCircuitIds;
        result.layoutHash=string.Join("|", result.circuits.OrderBy(c=>c.circuitId).ThenBy(c=>c.x).ThenBy(c=>c.y).Select(c=>$"{c.circuitId}:{c.x}:{c.y}:{c.rotationDegrees}")); result.candidateSnapshot.layoutHash=result.layoutHash; return result;
    }
    private static string Key(int x,int y)=>x+":"+y; private static void Parse(string key,out int x,out int y){var p=key.Split(':');x=int.Parse(p[0]);y=int.Parse(p[1]);}
}

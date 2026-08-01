using System.Collections.Generic;

public enum ConvergenceEffectChannel { BaseLE, BaseTraces, StabilityGain }

public class ConvergenceCircuitDefinition
{
    public string id; public int awardOrdinal; public string displayName; public int basePortMask;
    public int baseTargetDirectionMask; public bool isAmplifier; public bool requiresAllOwnedPowered; public ConvergenceEffectChannel primaryChannel;
    public double baseLEBonus; public double baseTracesBonus; public double stabilityBonus;
}

public static class ConvergenceCircuitCatalog
{
    public const int CatalogRevision = 1;
    public const int North = 1, East = 2, South = 4, West = 8;
    public const int ExperimentalBoardRadius = 2;
    public const string StartupPulseCircuitId = "convergence_circuit_001_startup_pulse";
    public static double StartupPulseBaseLEProductionBonus = 0.10;
    public static readonly List<ConvergenceCircuitDefinition> Definitions = new List<ConvergenceCircuitDefinition>
    {
        new ConvergenceCircuitDefinition { id=StartupPulseCircuitId, awardOrdinal=1, displayName="Pulso de Arranque", basePortMask=South, primaryChannel=ConvergenceEffectChannel.BaseLE, baseLEBonus=.10 },
        new ConvergenceCircuitDefinition { id="convergence_circuit_002_axial_link", awardOrdinal=2, displayName="Enlace Axial", basePortMask=North|South, primaryChannel=ConvergenceEffectChannel.BaseTraces, baseTracesBonus=.10 },
        new ConvergenceCircuitDefinition { id="convergence_circuit_003_resonant_elbow", awardOrdinal=3, displayName="Codo Resonante", basePortMask=North|East, primaryChannel=ConvergenceEffectChannel.StabilityGain, stabilityBonus=.10 },
        new ConvergenceCircuitDefinition { id="convergence_circuit_004_triaxial_distributor", awardOrdinal=4, displayName="Distribuidor Triaxial", basePortMask=North|East|West, primaryChannel=ConvergenceEffectChannel.BaseLE, baseLEBonus=.12, baseTracesBonus=.12 },
        new ConvergenceCircuitDefinition { id="convergence_circuit_005_local_coupler", awardOrdinal=5, displayName="Acoplador Local", basePortMask=South, baseTargetDirectionMask=North, isAmplifier=true },
        new ConvergenceCircuitDefinition { id="convergence_circuit_006_closure_matrix", awardOrdinal=6, displayName="Matriz de Cierre", basePortMask=North|East|South|West, requiresAllOwnedPowered=true, primaryChannel=ConvergenceEffectChannel.BaseLE, baseLEBonus=.08, baseTracesBonus=.08, stabilityBonus=.05 }
    };
    public static bool IsKnownCircuit(string id) => Get(id) != null;
    public static ConvergenceCircuitDefinition Get(string id) => Definitions.Find(d => d.id == id);
    public static bool IsValidRotation(int degrees) => degrees == 0 || degrees == 90 || degrees == 180 || degrees == 270;
    public static bool IsValidBoardCoordinate(int x, int y) => x >= -2 && x <= 2 && y >= -2 && y <= 2;
    public static int RotateMask(int mask, int degrees) { int turns = ((degrees % 360) + 360) % 360 / 90; for (int i=0;i<turns;i++) mask = ((mask << 1) & 0xE) | ((mask & West) >> 3); return mask; }
}

using System;
using System.Collections.Generic;

public enum MachineZoneType
{
    None = 0,
    Room1Link = 1,
    FusionSector = 2,
    InternalSupport = 3,
    InstantChamber = 4
}

public enum MachineNodeEffectType
{
    None = 0,

    // Zona 1
    GlobalLEBonus = 1,
    TracesBonus = 2,
    TriangleBonus = 3,
    ArtifactBonus = 4,
    Room1GlobalBonus = 5,

    // Zona 2
    UnlockFusionSlot = 20,
    FusionFailureReduction = 21,
    FusionUsefulResultBonus = 22,
    FusionTimeReduction = 23,
    RevealFusionProbabilities = 24,
    CatalystTuning = 25,
    StableReactionChamber = 26,
    GuidedSynthesis = 27,
    SynthesisCore = 28,

    // Zona 3 — Soporte Interno
    UnlockDiagnostics = 40,
    RevealHiddenSubnodes = 41,
    CompensationCircuit = 42,
    UnlockMachineMemory = 43,
    ZoneProgressSyncBonus = 44,
    StructuralSupportBonus = 45,
    EnablePrestige1 = 46,
    DiagnosticReasonMarker = 47,
    InternalSupportBonus = 48,

    // Zona 4 — Cámara de Anclajes
    UnlockInstantChamber = 60,
    SeedReadingBonus = 61,
    ArchiveSlotBonus = 62,
    InstantInitialStabilityBonus = 63,
    SynchronizeStabilityBonus = 64,
    TensionContainmentBonus = 65,
    SafeRewindBonus = 66,
    SeedSlotBonus = 67,
    PureMaterialThresholdReduction = 68,

    // Legacy temporal: referencias antiguas de UI que ya no deben usarse en JSON.
    UnlockNodeAnalysis = 90,
}

[Serializable]
public class MachineNodeCostDef
{
    public double le;
    public double traces;

    public int hallazgo;
    public int muestra;
    public int lecturaIncompleta;
    public int compuestoUtil;

    public int pureInstant;
    public int stableInstant;
    public int forcedInstant;
}

[Serializable]
public class MachineNodeDef
{
    public string id;
    public string name;
    public string description;

    public MachineZoneType zone;
    public MachineNodeEffectType effectType;

    public double effectValue;

    public MachineNodeCostDef cost;
    public List<string> requiredNodeIds;
    public bool hidden;
    public bool damaged;

    // Si varios nodos comparten grupo, la UI solo muestra el siguiente tier pendiente.
    public string tierGroup;
    public int tierIndex;
}

[Serializable]
public class MachineNodeDefList
{
    public List<MachineNodeDef> nodes;
}
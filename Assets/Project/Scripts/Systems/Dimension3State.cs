using System;
using System.Collections.Generic;


[Serializable]
public class D3PartStackState
{
    public string partId = "";
    public int version = 1;
    public long amount;
}


[Serializable]
public class D3PartAmountState
{
    public string partId = "";
    public int version = 1;
    public long amount;
}


[Serializable]
public class D3AutomatonStackState
{
    public int mk = 1;
    public string traitId = Dimension3Catalog.TraitNormal;
    public long totalAmount;
}


[Serializable]
public class D3AssignmentState
{
    public string installationId = "";
    public string channelId = "";
    public int mk = 1;
    public string traitId = Dimension3Catalog.TraitNormal;
    public long amount;
    public long stabilizedAmount;
    public double stabilizationRemainingSeconds;
}


[Serializable]
public class D3ReservedAutomatonState
{
    public int mk = 1;
    public string traitId = Dimension3Catalog.TraitNormal;
    public long amount;
}


[Serializable]
public class D3CalibrationReadingState
{
    public string partId = "";
    public int reading;
}


[Serializable]
public class D3CalibrationControlState
{
    public string partId = "";
    public int valueA;
    public int valueB;
    public int valueC;
    public int optionA;
    public int optionB;
    public int optionC;
}


[Serializable]
public class D3JobState
{
    public string jobId = "";
    public string jobType = "";
    public string targetId = "";
    public int version = 1;
    public int mk = 1;
    public long quantity = 1;
    public string resultTraitId = Dimension3Catalog.TraitNormal;
    public double baseDurationSeconds;
    public double remainingSeconds;
    public double paidLE;
    public double paidTraces;
    public bool started;
    // Los trabajos creados antes del progreso v7 conservan su duracion ya
    // modificada. Los nuevos almacenan trabajo base y avanzan a velocidad dinamica.
    public bool usesDynamicBankSpeed;
    public List<D3PartAmountState> consumedParts = new List<D3PartAmountState>();
    public List<D3ReservedAutomatonState> reservedAutomatons =
        new List<D3ReservedAutomatonState>();
    public List<D3CalibrationReadingState> calibrationReadings =
        new List<D3CalibrationReadingState>();
}


[Serializable]
public class D3QueueState
{
    public string queueId = "";
    public List<D3JobState> jobs = new List<D3JobState>();
}


[Serializable]
public class D3ResearchState
{
    public string researchId = "";
    public bool completed;
}


[Serializable]
public class D3FacilityState
{
    public string facilityId = "";
    public bool built;
    public int level;
}


[Serializable]
public class D3CalibrationProfileState
{
    public string profileId = "";
    public string displayName = "";
    public List<D3CalibrationReadingState> readings =
        new List<D3CalibrationReadingState>();
    public List<D3CalibrationControlState> controls =
        new List<D3CalibrationControlState>();
}


[Serializable]
public class D3AutomationRoutineState
{
    public string routineId = "";
    public string actionId = "";
    public bool enabled;
    public int priority;
    public double leReserve;
    public double tracesReserve;
    public string resourceReserveId = "";
    public double resourceReserveAmount;
    public string targetId = "";
    public List<string> orderedTargetIds = new List<string>();
    public int stopAfterExecutions;
    public int executionsCompleted;
    public string lastResult = "";
    public double evaluationRemainingSeconds;
}


[Serializable]
public class D3AutomationProfileState
{
    public int snapshotVersion;
    public string profileId = "";
    public string displayName = "";
    public List<string> routineIds = new List<string>();
    public List<D3AutomationRoutineState> savedRoutines =
        new List<D3AutomationRoutineState>();
    public D3ConsoleSettingsState savedConsoleSettings =
        new D3ConsoleSettingsState();
    public D3DiagnosticSettingsState savedDiagnosticSettings =
        new D3DiagnosticSettingsState();
    public List<D3MarkedFusionRecipeState> savedMarkedFusionRecipes =
        new List<D3MarkedFusionRecipeState>();
    public List<D3CalibrationProfileState> savedCalibrationProfiles =
        new List<D3CalibrationProfileState>();
}


[Serializable]
public class D3TrianglePresetState
{
    public string presetId = "";
    public string primaryBuildingId = "";
    public string reinforcementBuildingId = "";
    public string alterationBuildingId = "";
}


[Serializable]
public class D3ConsoleSettingsState
{
    public string purchasePolicy = "balanced";
    public double leReserve;
    public double tracesReserve;
    public int preferredModulatorMode;
    public string preferredTrianglePresetId = "";
    public List<string> manuallyPurchasedBuildingIds = new List<string>();
    public List<int> manuallySelectedModulatorModes = new List<int>();
    public List<D3TrianglePresetState> manualTrianglePresets =
        new List<D3TrianglePresetState>();
}


[Serializable]
public class D3MarkedFusionRecipeState
{
    public string recipeId = "";
    public bool manuallyExecuted;
    public bool automationMarked;
}


[Serializable]
public class D3DiagnosticSettingsState
{
    public bool autoAnalyzeEnabled;
    public bool autoRepairEnabled;
    public bool autoFusionEnabled;
    // 0 = orden global ascendente; 1 = zona elegida primero.
    public int priorityMode;
    public int priorityZone;
    public double leReserve;
    public double tracesReserve;
    public double evaluationRemainingSeconds;
    public int nextFusionRecipeIndex;
    public D3DiagnosticRoutineState savedRoutine =
        new D3DiagnosticRoutineState();
}


[Serializable]
public class D3DiagnosticRoutineState
{
    public bool saved;
    public bool autoAnalyzeEnabled;
    public bool autoRepairEnabled;
    public bool autoFusionEnabled;
    public int priorityMode;
    public int priorityZone;
    public double leReserve;
    public double tracesReserve;
    public List<string> markedRecipeIds = new List<string>();
}


[Serializable]
public class D3MkAssemblyCountState
{
    public int mk = 1;
    public long amount;
}


[Serializable]
public class Dimension3State
{
    public int progressVersion = Dimension3System.ProgressVersion;
    public bool initialized;
    public bool firstEntrySeen;
    public string selectedInstallationId = Dimension3Catalog.FacilityProcessBank;
    public long nextJobSequence = 1;
    public long nextAutomationSequence = 1;

    public List<D3PartStackState> parts = new List<D3PartStackState>();
    public List<D3AutomatonStackState> automatons = new List<D3AutomatonStackState>();
    public List<D3AssignmentState> assignments = new List<D3AssignmentState>();
    public List<D3QueueState> queues = new List<D3QueueState>();
    public List<D3ResearchState> research = new List<D3ResearchState>();
    public List<D3FacilityState> facilities = new List<D3FacilityState>();
    public List<D3CalibrationProfileState> calibrationProfiles =
        new List<D3CalibrationProfileState>();
    public List<D3AutomationRoutineState> automationRoutines =
        new List<D3AutomationRoutineState>();
    public List<D3AutomationProfileState> automationProfiles =
        new List<D3AutomationProfileState>();
    public List<D3MarkedFusionRecipeState> markedFusionRecipes =
        new List<D3MarkedFusionRecipeState>();
    public D3DiagnosticSettingsState diagnosticSettings =
        new D3DiagnosticSettingsState();
    public D3ConsoleSettingsState consoleSettings =
        new D3ConsoleSettingsState();
    public List<D3MkAssemblyCountState> totalAssembledByMk =
        new List<D3MkAssemblyCountState>();
}

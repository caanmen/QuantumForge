using System;
using System.Collections.Generic;


public enum FeaturePresentationVisualState
{
    Hidden = 0,
    Teaser = 1,
    Available = 2,
    Active = 3,
    Completed = 4
}


[Serializable]
public class DimensionPresentationState
{
    public const int CurrentVersion = 6;

    public int presentationVersion;
    public int onboardingStage;
    public List<string> introducedFeatureIds = new List<string>();
    public List<string> acknowledgedFeatureIds = new List<string>();
    public string lastScreenId = "";
    public bool contextualHelpEnabled = true;
    public bool advancedControlsExpanded;

    public static DimensionPresentationState CreateCurrent()
    {
        return new DimensionPresentationState
        {
            presentationVersion = CurrentVersion,
            contextualHelpEnabled = true
        };
    }
}


public sealed class FeaturePresentationState
{
    public readonly string featureId;
    public readonly FeaturePresentationVisualState visualState;
    public readonly bool isNew;

    public FeaturePresentationState(
        string featureId,
        FeaturePresentationVisualState visualState,
        bool isNew)
    {
        this.featureId = featureId ?? "";
        this.visualState = visualState;
        this.isNew = isNew &&
            (visualState == FeaturePresentationVisualState.Available ||
             visualState == FeaturePresentationVisualState.Active);
    }

    public bool IsVisible => visualState != FeaturePresentationVisualState.Hidden;
    public bool CanOpen =>
        visualState == FeaturePresentationVisualState.Available ||
        visualState == FeaturePresentationVisualState.Active ||
        visualState == FeaturePresentationVisualState.Completed;
}


public static class PresentationFeatureIds
{
    public const string D2Map = "d2.map";
    public const string D2C1Refuge = "d2.c1.refuge";
    public const string D2C1Altars = "d2.c1.altars";
    public const string D2C1Pilgrimages = "d2.c1.pilgrimages";
    public const string D2C1Novitiate = "d2.c1.novitiate";
    public const string D2C1Rites = "d2.c1.rites";
    public const string D2C1Pacts = "d2.c1.pacts";
    public const string D2C1VeiledThreshold = "d2.c1.veiled_threshold";
    public const string D2C2Regions = "d2.c2.regions";
    public const string D2C2Operations = "d2.c2.operations";
    public const string D2C2Defense = "d2.c2.defense";
    public const string D2C2Resistance = "d2.c2.resistance";
    public const string D2C2Alert = "d2.c2.alert";
    public const string D2C2Containment = "d2.c2.containment";
    public const string D2C2MajorPact = "d2.c2.major_pact";
    public const string D2C3Archaeology = "d2.c3.archaeology";
    public const string D2C3Analysis = "d2.c3.analysis";
    public const string D2C3Archive = "d2.c3.archive";
    public const string D2C3Clues = "d2.c3.clues";
    public const string D2C3Anomalies = "d2.c3.anomalies";
    public const string D2C3EntityResearch = "d2.c3.entity_research";
    public const string D2C3EntityPact = "d2.c3.entity_pact";

    public const string D3Factory = "d3.factory";
    public const string D3AssignmentBasic = "d3.assignment_basic";
    public const string D3ProductionFirstPart = "d3.production_first_part";
    public const string D3ProductionSet = "d3.production_set";
    public const string D3AssemblyNormal = "d3.assembly_normal";
    public const string D3QueuesCompact = "d3.queues_compact";
    public const string D3QueuesFull = "d3.queues_full";
    public const string D3ProcessBank = "d3.process_bank";
    public const string D3Calibration = "d3.calibration";
    public const string D3Research = "d3.research";
    public const string D3Facilities = "d3.facilities";
    public const string D3FacilityConsole = "d3.facility.console";
    public const string D3FacilityDiagnostic = "d3.facility.diagnostic";
    public const string D3FacilityPort = "d3.facility.port";
    public const string D3FacilityCore = "d3.facility.core";
    public const string D3Automation = "d3.automation";
    public const string D3OfflineAutomation = "d3.offline_automation";

    public static readonly string[] D2All =
    {
        D2Map, D2C1Refuge, D2C1Altars, D2C1Pilgrimages, D2C1Novitiate,
        D2C1Rites, D2C1Pacts, D2C1VeiledThreshold, D2C2Regions,
        D2C2Operations, D2C2Defense, D2C2Resistance, D2C2Alert,
        D2C2Containment, D2C2MajorPact, D2C3Archaeology, D2C3Analysis,
        D2C3Archive, D2C3Clues, D2C3Anomalies, D2C3EntityResearch,
        D2C3EntityPact
    };

    public static readonly string[] D3All =
    {
        D3Factory, D3AssignmentBasic, D3ProductionFirstPart, D3ProductionSet,
        D3AssemblyNormal, D3QueuesCompact, D3QueuesFull, D3ProcessBank,
        D3Calibration, D3Research, D3Facilities, D3FacilityConsole,
        D3FacilityDiagnostic, D3FacilityPort, D3FacilityCore, D3Automation,
        D3OfflineAutomation
    };
}


public static class PresentationStateUtility
{
    public static void Normalize(DimensionPresentationState state)
    {
        if (state == null)
            return;

        if (state.introducedFeatureIds == null)
            state.introducedFeatureIds = new List<string>();
        if (state.acknowledgedFeatureIds == null)
            state.acknowledgedFeatureIds = new List<string>();

        NormalizeIds(state.introducedFeatureIds);
        NormalizeIds(state.acknowledgedFeatureIds);
        for (int i = 0; i < state.acknowledgedFeatureIds.Count; i++)
            AddUnique(state.introducedFeatureIds, state.acknowledgedFeatureIds[i]);

        state.onboardingStage = Math.Max(0, state.onboardingStage);
        if (state.lastScreenId == null)
            state.lastScreenId = "";
    }

    public static bool Contains(IList<string> ids, string id)
    {
        if (ids == null || string.IsNullOrEmpty(id))
            return false;
        for (int i = 0; i < ids.Count; i++)
        {
            if (string.Equals(ids[i], id, StringComparison.Ordinal))
                return true;
        }
        return false;
    }

    public static void Introduce(DimensionPresentationState state, string id)
    {
        if (state == null || string.IsNullOrEmpty(id))
            return;
        if (state.introducedFeatureIds == null)
            state.introducedFeatureIds = new List<string>();
        AddUnique(state.introducedFeatureIds, id);
    }

    public static void Acknowledge(DimensionPresentationState state, string id)
    {
        if (state == null || string.IsNullOrEmpty(id))
            return;
        Introduce(state, id);
        if (state.acknowledgedFeatureIds == null)
            state.acknowledgedFeatureIds = new List<string>();
        AddUnique(state.acknowledgedFeatureIds, id);
    }

    public static FeaturePresentationState Build(
        DimensionPresentationState state,
        string featureId,
        FeaturePresentationVisualState visualState)
    {
        bool introduced = state != null &&
            Contains(state.introducedFeatureIds, featureId);
        bool acknowledged = state != null &&
            Contains(state.acknowledgedFeatureIds, featureId);
        return new FeaturePresentationState(
            featureId,
            visualState,
            introduced && !acknowledged);
    }

    private static void NormalizeIds(List<string> ids)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        int write = 0;
        for (int read = 0; read < ids.Count; read++)
        {
            string id = ids[read];
            if (string.IsNullOrEmpty(id) || !seen.Add(id))
                continue;
            ids[write++] = id;
        }
        if (write < ids.Count)
            ids.RemoveRange(write, ids.Count - write);
    }

    private static void AddUnique(List<string> ids, string id)
    {
        if (!Contains(ids, id))
            ids.Add(id);
    }
}

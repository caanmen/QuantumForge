using System;
using System.Collections.Generic;


public static class D3PresentationRules
{
    public static void EnsurePresentationState(GameState gameState)
    {
        if (gameState == null || gameState.dimension3 == null)
            return;

        Dimension3State d3 = gameState.dimension3;
        bool requiresMigration = d3.presentation == null ||
            d3.presentation.presentationVersion < DimensionPresentationState.CurrentVersion;
        if (d3.presentation == null)
            d3.presentation = new DimensionPresentationState();

        PresentationStateUtility.Normalize(d3.presentation);
        if (requiresMigration)
        {
            InferIntroducedFeatures(d3);
            D3OnboardingRules.MigrateLegacyStage(d3);
        }
        IntroduceProgressiveFeatures(gameState);
        IntroduceActiveFeatures(d3);
        d3.presentation.presentationVersion = DimensionPresentationState.CurrentVersion;
        PresentationStateUtility.Normalize(d3.presentation);
    }

    public static FeaturePresentationState GetFeatureState(
        GameState gameState,
        string featureId)
    {
        if (gameState == null || gameState.dimension3 == null ||
            !IsKnownFeatureId(featureId))
        {
            return new FeaturePresentationState(
                featureId, FeaturePresentationVisualState.Hidden, false);
        }

        Dimension3State d3 = gameState.dimension3;
        DimensionPresentationState presentation = d3.presentation;
        FeaturePresentationVisualState visualState;
        if (IsActive(d3, featureId))
            visualState = FeaturePresentationVisualState.Active;
        else if (IsCompleted(d3, featureId))
            visualState = FeaturePresentationVisualState.Completed;
        else if (IsAvailable(d3, presentation, featureId))
            visualState = FeaturePresentationVisualState.Available;
        else if (IsTeaser(d3, featureId))
            visualState = FeaturePresentationVisualState.Teaser;
        else
            visualState = FeaturePresentationVisualState.Hidden;

        return PresentationStateUtility.Build(presentation, featureId, visualState);
    }

    public static bool IsKnownFeatureId(string featureId)
    {
        for (int i = 0; i < PresentationFeatureIds.D3All.Length; i++)
        {
            if (string.Equals(
                    PresentationFeatureIds.D3All[i], featureId,
                    StringComparison.Ordinal))
                return true;
        }
        return false;
    }

    public static bool IsActive(Dimension3State d3, string featureId)
    {
        if (d3 == null)
            return false;

        switch (featureId)
        {
            case PresentationFeatureIds.D3AssignmentBasic:
            case PresentationFeatureIds.D3ProcessBank:
                return HasStabilizingAssignments(d3);
            case PresentationFeatureIds.D3ProductionFirstPart:
            case PresentationFeatureIds.D3ProductionSet:
                return HasQueueJobs(d3, Dimension3Catalog.QueuePartProduction);
            case PresentationFeatureIds.D3AssemblyNormal:
                return HasQueueJobs(d3, Dimension3Catalog.QueueAssembly);
            case PresentationFeatureIds.D3QueuesCompact:
            case PresentationFeatureIds.D3QueuesFull:
                return GetTotalJobCount(d3) > 0;
            case PresentationFeatureIds.D3Calibration:
                return HasIncompleteCalibration(d3);
            case PresentationFeatureIds.D3Research:
                return HasQueueJobs(d3, Dimension3Catalog.QueueResearch);
            case PresentationFeatureIds.D3Facilities:
                return HasQueueJobs(d3, Dimension3Catalog.QueueFacility);
            case PresentationFeatureIds.D3FacilityConsole:
                return HasFacilityJob(d3, Dimension3Catalog.FacilityProductionConsole);
            case PresentationFeatureIds.D3FacilityDiagnostic:
                return HasFacilityJob(d3, Dimension3Catalog.FacilityDiagnosticBank);
            case PresentationFeatureIds.D3FacilityPort:
                return HasFacilityJob(d3, Dimension3Catalog.FacilityExpeditionPort);
            case PresentationFeatureIds.D3FacilityCore:
                return HasFacilityJob(d3, Dimension3Catalog.FacilityAutomationCore);
            case PresentationFeatureIds.D3Automation:
                return HasEnabledRoutine(d3);
            case PresentationFeatureIds.D3OfflineAutomation:
                return HasEnabledRoutine(d3) &&
                    D3FacilitySystem.CanRunExternalAutomationOffline(d3);
            default:
                return false;
        }
    }

    private static void InferIntroducedFeatures(Dimension3State d3)
    {
        DimensionPresentationState presentation = d3.presentation;
        if (d3.initialized)
        {
            Introduce(presentation, PresentationFeatureIds.D3Factory);
            Introduce(presentation, PresentationFeatureIds.D3ProcessBank);
        }
        if (HasAssignments(d3))
            Introduce(presentation, PresentationFeatureIds.D3AssignmentBasic);

        bool hasPartProgress = HasPartProgress(d3) ||
            HasQueueJobs(d3, Dimension3Catalog.QueuePartProduction);
        if (hasPartProgress)
            Introduce(presentation, PresentationFeatureIds.D3ProductionFirstPart);
        if (hasPartProgress)
            Introduce(presentation, PresentationFeatureIds.D3ProductionSet);

        if (HasAssemblyProgress(d3) || HasCompleteSet(d3))
            Introduce(presentation, PresentationFeatureIds.D3AssemblyNormal);

        int jobCount = GetTotalJobCount(d3);
        if (jobCount > 0)
            Introduce(presentation, PresentationFeatureIds.D3QueuesCompact);
        if (jobCount > 0 || HasAssemblyProgress(d3))
            Introduce(presentation, PresentationFeatureIds.D3QueuesFull);

        if (D3FacilitySystem.GetProcessBankLevel(d3) > 1 || HasAdvancedAssignments(d3))
            Introduce(presentation, PresentationFeatureIds.D3ProcessBank);
        if (HasCalibrationProgress(d3))
            Introduce(presentation, PresentationFeatureIds.D3Calibration);
        if (HasResearchProgress(d3) || HasAdvancedVersionProgress(d3))
            Introduce(presentation, PresentationFeatureIds.D3Research);

        if (HasAnyBuiltOptionalFacility(d3) || HasQueueJobs(d3, Dimension3Catalog.QueueFacility))
            Introduce(presentation, PresentationFeatureIds.D3Facilities);
        IntroduceFacilityIfBuilt(d3, Dimension3Catalog.FacilityProductionConsole,
            PresentationFeatureIds.D3FacilityConsole);
        IntroduceFacilityIfBuilt(d3, Dimension3Catalog.FacilityDiagnosticBank,
            PresentationFeatureIds.D3FacilityDiagnostic);
        IntroduceFacilityIfBuilt(d3, Dimension3Catalog.FacilityExpeditionPort,
            PresentationFeatureIds.D3FacilityPort);
        IntroduceFacilityIfBuilt(d3, Dimension3Catalog.FacilityAutomationCore,
            PresentationFeatureIds.D3FacilityCore);

        if (HasAutomationProgress(d3))
            Introduce(presentation, PresentationFeatureIds.D3Automation);
        if (HasAutomationProgress(d3) &&
            D3FacilitySystem.CanRunExternalAutomationOffline(d3))
            Introduce(presentation, PresentationFeatureIds.D3OfflineAutomation);

        presentation.onboardingStage = Math.Max(
            presentation.onboardingStage,
            InferMinimumOnboardingStage(d3));
    }

    private static void IntroduceActiveFeatures(Dimension3State d3)
    {
        for (int i = 0; i < PresentationFeatureIds.D3All.Length; i++)
        {
            string featureId = PresentationFeatureIds.D3All[i];
            if (IsActive(d3, featureId))
                Introduce(d3.presentation, featureId);
        }
    }

    private static void IntroduceProgressiveFeatures(GameState gameState)
    {
        Dimension3State d3 = gameState.dimension3;
        if (D3ProgressivePresentationRules.CanIntroduceCalibration(gameState))
            Introduce(d3.presentation, PresentationFeatureIds.D3Calibration);
        if (D3ProgressivePresentationRules.CanIntroduceResearch(gameState))
            Introduce(d3.presentation, PresentationFeatureIds.D3Research);
        if (D3ProgressivePresentationRules.CanIntroduceFacilities(gameState))
            Introduce(d3.presentation, PresentationFeatureIds.D3Facilities);
        if (D3ProgressivePresentationRules.GetLearnedAutomationActionIds(
                gameState).Count > 0)
            Introduce(d3.presentation, PresentationFeatureIds.D3Automation);
        if (HasAutomationProgress(d3) &&
            D3FacilitySystem.CanRunExternalAutomationOffline(d3))
            Introduce(d3.presentation, PresentationFeatureIds.D3OfflineAutomation);
    }

    private static bool IsAvailable(
        Dimension3State d3,
        DimensionPresentationState presentation,
        string featureId)
    {
        if (PresentationStateUtility.Contains(
                presentation == null ? null : presentation.introducedFeatureIds,
                featureId))
            return true;

        switch (featureId)
        {
            case PresentationFeatureIds.D3Factory:
            case PresentationFeatureIds.D3ProcessBank:
            case PresentationFeatureIds.D3AssignmentBasic:
            case PresentationFeatureIds.D3ProductionFirstPart:
                return d3.initialized;
            default:
                return false;
        }
    }

    private static bool IsCompleted(Dimension3State d3, string featureId)
    {
        if (featureId == PresentationFeatureIds.D3AssemblyNormal)
            return HasAssemblyProgress(d3) &&
                !HasQueueJobs(d3, Dimension3Catalog.QueueAssembly);
        if (featureId == PresentationFeatureIds.D3Research)
            return HasCompletedResearch(d3) &&
                !HasQueueJobs(d3, Dimension3Catalog.QueueResearch);
        return false;
    }

    private static bool IsTeaser(Dimension3State d3, string featureId)
    {
        if (featureId == PresentationFeatureIds.D3AssemblyNormal)
            return HasPartProgress(d3);
        if (featureId == PresentationFeatureIds.D3Facilities)
            return D3InventorySystem.GetAssemblyCount(d3, 1) > 0L;
        if (featureId == PresentationFeatureIds.D3Research)
            return D3FacilitySystem.GetProcessBankLevel(d3) >= 3 &&
                D3InventorySystem.GetAssemblyCount(d3, 3) > 0L;
        return false;
    }

    private static int InferMinimumOnboardingStage(Dimension3State d3)
    {
        if (HasAssemblyProgress(d3)) return 6;
        if (HasQueueJobs(d3, Dimension3Catalog.QueueAssembly)) return 5;
        if (HasCompleteSet(d3)) return 4;
        if (HasPartProgress(d3)) return 3;
        if (HasAssignments(d3)) return 2;
        if (d3.firstEntrySeen) return 1;
        return 0;
    }

    private static void IntroduceFacilityIfBuilt(
        Dimension3State d3,
        string facilityId,
        string featureId)
    {
        D3FacilityState facility = D3FacilitySystem.GetFacility(d3, facilityId);
        if (facility != null && facility.built)
            Introduce(d3.presentation, featureId);
    }

    private static bool HasAssignments(Dimension3State d3)
    {
        if (d3.assignments == null) return false;
        for (int i = 0; i < d3.assignments.Count; i++)
        {
            D3AssignmentState assignment = d3.assignments[i];
            if (assignment != null && assignment.amount > 0L) return true;
        }
        return false;
    }

    private static bool HasStabilizingAssignments(Dimension3State d3)
    {
        if (d3.assignments == null) return false;
        for (int i = 0; i < d3.assignments.Count; i++)
        {
            D3AssignmentState assignment = d3.assignments[i];
            if (assignment != null && assignment.amount > 0L &&
                assignment.stabilizationRemainingSeconds > 0.0) return true;
        }
        return false;
    }

    private static bool HasAdvancedAssignments(Dimension3State d3)
    {
        if (d3.assignments == null) return false;
        for (int i = 0; i < d3.assignments.Count; i++)
        {
            D3AssignmentState assignment = d3.assignments[i];
            if (assignment != null && assignment.amount > 0L &&
                (assignment.mk > 1 ||
                 assignment.channelId != Dimension3Catalog.ChannelProcessPower))
                return true;
        }
        return false;
    }

    private static bool HasPartProgress(Dimension3State d3)
    {
        if (d3.parts == null) return false;
        for (int i = 0; i < d3.parts.Count; i++)
            if (d3.parts[i] != null && d3.parts[i].amount > 0L) return true;
        return false;
    }

    private static bool HasCompleteSet(Dimension3State d3)
    {
        for (int version = 1; version <= 6; version++)
            if (D3InventorySystem.HasCompletePartSet(d3, version, 1L)) return true;
        return false;
    }

    private static bool HasAssemblyProgress(Dimension3State d3)
    {
        if (HasQueueJobs(d3, Dimension3Catalog.QueueAssembly)) return true;
        if (d3.totalAssembledByMk == null) return false;
        for (int i = 0; i < d3.totalAssembledByMk.Count; i++)
            if (d3.totalAssembledByMk[i] != null &&
                d3.totalAssembledByMk[i].amount > 0L) return true;
        return false;
    }

    private static bool HasCalibrationProgress(Dimension3State d3)
    {
        if (d3.calibrationProfiles == null) return false;
        for (int i = 0; i < d3.calibrationProfiles.Count; i++)
        {
            D3CalibrationProfileState profile = d3.calibrationProfiles[i];
            if (profile != null && ((profile.readings != null && profile.readings.Count > 0) ||
                (profile.controls != null && profile.controls.Count > 0))) return true;
        }
        return false;
    }

    private static bool HasIncompleteCalibration(Dimension3State d3)
    {
        if (d3.calibrationProfiles == null) return false;
        for (int i = 0; i < d3.calibrationProfiles.Count; i++)
        {
            D3CalibrationProfileState profile = d3.calibrationProfiles[i];
            if (profile == null || profile.readings == null) continue;
            var partIds = new HashSet<string>(StringComparer.Ordinal);
            for (int j = 0; j < profile.readings.Count; j++)
            {
                D3CalibrationReadingState reading = profile.readings[j];
                if (reading != null && !string.IsNullOrEmpty(reading.partId))
                    partIds.Add(reading.partId);
            }
            if (partIds.Count > 0 && partIds.Count < Dimension3Catalog.PartIds.Length)
                return true;
        }
        return false;
    }

    private static bool HasResearchProgress(Dimension3State d3)
    {
        return HasQueueJobs(d3, Dimension3Catalog.QueueResearch) ||
            HasCompletedResearch(d3);
    }

    private static bool HasCompletedResearch(Dimension3State d3)
    {
        if (d3.research == null) return false;
        for (int i = 0; i < d3.research.Count; i++)
            if (d3.research[i] != null && d3.research[i].completed) return true;
        return false;
    }

    private static bool HasAdvancedVersionProgress(Dimension3State d3)
    {
        if (d3.parts != null)
        {
            for (int i = 0; i < d3.parts.Count; i++)
                if (d3.parts[i] != null && d3.parts[i].amount > 0L &&
                    d3.parts[i].version >= 4) return true;
        }
        return D3InventorySystem.GetAssemblyCount(d3, 4) > 0L ||
            D3InventorySystem.GetAssemblyCount(d3, 5) > 0L ||
            D3InventorySystem.GetAssemblyCount(d3, 6) > 0L;
    }

    private static bool HasAnyBuiltOptionalFacility(Dimension3State d3)
    {
        if (d3.facilities == null) return false;
        for (int i = 0; i < d3.facilities.Count; i++)
        {
            D3FacilityState facility = d3.facilities[i];
            if (facility != null && facility.built &&
                facility.facilityId != Dimension3Catalog.FacilityProcessBank)
                return true;
        }
        return false;
    }

    private static bool HasAutomationProgress(Dimension3State d3)
    {
        return (d3.automationRoutines != null && d3.automationRoutines.Count > 0) ||
            (d3.automationProfiles != null && d3.automationProfiles.Count > 0);
    }

    private static bool HasEnabledRoutine(Dimension3State d3)
    {
        if (d3.automationRoutines == null) return false;
        for (int i = 0; i < d3.automationRoutines.Count; i++)
            if (d3.automationRoutines[i] != null &&
                d3.automationRoutines[i].enabled) return true;
        return false;
    }

    private static bool HasFacilityJob(Dimension3State d3, string facilityId)
    {
        D3QueueState queue = D3JobQueueSystem.GetQueue(
            d3, Dimension3Catalog.QueueFacility);
        if (queue == null || queue.jobs == null) return false;
        for (int i = 0; i < queue.jobs.Count; i++)
        {
            D3JobState job = queue.jobs[i];
            if (job != null && job.targetId == facilityId) return true;
        }
        return false;
    }

    private static bool HasQueueJobs(Dimension3State d3, string queueId)
    {
        return D3JobQueueSystem.GetJobCount(d3, queueId) > 0;
    }

    private static int GetTotalJobCount(Dimension3State d3)
    {
        int total = 0;
        for (int i = 0; i < Dimension3Catalog.QueueIds.Length; i++)
            total += D3JobQueueSystem.GetJobCount(d3, Dimension3Catalog.QueueIds[i]);
        return total;
    }

    private static void Introduce(DimensionPresentationState state, string id)
    {
        PresentationStateUtility.Introduce(state, id);
    }
}

using System;


public enum D3OnboardingStage
{
    Discovery = 0,
    AssignInitial = 1,
    FirstPart = 2,
    CompleteSet = 3,
    FirstAssembly = 4,
    AssignNew = 5,
    Celebration = 6,
    Completed = 7
}


public sealed class D3OnboardingSnapshot
{
    public D3OnboardingStage stage;
    public int completedPartTypes;
    public int plannedPartTypes;
    public long totalMk1;
    public long freeMk1;
    public long assignedMk1;
    public long manufacturedMk1;
    public bool productionActive;
    public bool assemblyActive;
    public string nextMissingPartId = "";
}


public static class D3OnboardingRules
{
    public static D3OnboardingSnapshot Synchronize(GameState gameState)
    {
        D3OnboardingSnapshot snapshot = Evaluate(gameState);
        if (gameState == null || gameState.dimension3 == null ||
            gameState.dimension3.presentation == null)
            return snapshot;

        DimensionPresentationState presentation = gameState.dimension3.presentation;
        int derived = (int)snapshot.stage;
        if (presentation.onboardingStage < derived)
            presentation.onboardingStage = derived;
        snapshot.stage = (D3OnboardingStage)Math.Max(
            derived, presentation.onboardingStage);
        IntroduceReachedFeatures(presentation, snapshot);
        return snapshot;
    }

    public static D3OnboardingSnapshot Evaluate(GameState gameState)
    {
        var snapshot = new D3OnboardingSnapshot();
        if (gameState == null || gameState.dimension3 == null)
            return snapshot;

        Dimension3State d3 = gameState.dimension3;
        snapshot.productionActive = D3JobQueueSystem.GetJobCount(
            d3, Dimension3Catalog.QueuePartProduction) > 0;
        snapshot.assemblyActive = D3JobQueueSystem.GetJobCount(
            d3, Dimension3Catalog.QueueAssembly) > 0;
        snapshot.manufacturedMk1 = D3InventorySystem.GetAssemblyCount(d3, 1);
        snapshot.totalMk1 = D3InventorySystem.GetAutomatonAmount(
            d3, 1, Dimension3Catalog.TraitNormal);
        snapshot.freeMk1 = D3InventorySystem.GetAvailableAutomatonAmount(
            d3, 1, Dimension3Catalog.TraitNormal);
        snapshot.assignedMk1 = D3InventorySystem.GetAssignedAutomatonAmount(
            d3, 1, Dimension3Catalog.TraitNormal);

        bool hasReceivedPart = false;
        bool hasPlannedPart = false;
        for (int i = 0; i < Dimension3Catalog.PartIds.Length; i++)
        {
            string partId = Dimension3Catalog.PartIds[i];
            long inventory = D3InventorySystem.GetPartAmount(d3, partId, 1);
            long queued = GetQueuedPartAmount(d3, partId, 1);
            if (inventory > 0L)
            {
                snapshot.completedPartTypes++;
                hasReceivedPart = true;
            }
            if (inventory + queued > 0L)
            {
                snapshot.plannedPartTypes++;
                hasPlannedPart = true;
            }
            else if (string.IsNullOrEmpty(snapshot.nextMissingPartId))
            {
                snapshot.nextMissingPartId = partId;
            }
        }

        int persistedStage = d3.presentation == null
            ? 0
            : Math.Max(0, d3.presentation.onboardingStage);
        if (IsAdvancedState(d3))
            snapshot.stage = D3OnboardingStage.Completed;
        else if (persistedStage >= (int)D3OnboardingStage.Completed)
            snapshot.stage = D3OnboardingStage.Completed;
        else if (snapshot.manufacturedMk1 > 0L)
            snapshot.stage = snapshot.assignedMk1 >= 2L || snapshot.freeMk1 <= 0L
                ? D3OnboardingStage.Celebration
                : D3OnboardingStage.AssignNew;
        else if (snapshot.assemblyActive ||
                 D3InventorySystem.HasCompletePartSet(d3, 1, 1L))
            snapshot.stage = D3OnboardingStage.FirstAssembly;
        else if (hasReceivedPart ||
                 (hasPlannedPart && snapshot.plannedPartTypes > 1))
            snapshot.stage = D3OnboardingStage.CompleteSet;
        else if (snapshot.productionActive || snapshot.assignedMk1 > 0L)
            snapshot.stage = D3OnboardingStage.FirstPart;
        else if (d3.firstEntrySeen)
            snapshot.stage = D3OnboardingStage.AssignInitial;
        else
            snapshot.stage = D3OnboardingStage.Discovery;

        return snapshot;
    }

    public static void MigrateLegacyStage(Dimension3State d3)
    {
        if (d3 == null || d3.presentation == null)
            return;

        if (IsAdvancedState(d3) ||
            D3InventorySystem.GetAssemblyCount(d3, 1) > 0L)
        {
            d3.presentation.onboardingStage = Math.Max(
                d3.presentation.onboardingStage,
                (int)D3OnboardingStage.Completed);
        }
    }

    public static long GetQueuedPartAmount(
        Dimension3State d3,
        string partId,
        int version)
    {
        D3QueueState queue = D3JobQueueSystem.GetQueue(
            d3, Dimension3Catalog.QueuePartProduction);
        if (queue == null || queue.jobs == null) return 0L;
        long total = 0L;
        for (int i = 0; i < queue.jobs.Count; i++)
        {
            D3JobState job = queue.jobs[i];
            if (job != null && job.jobType == Dimension3Catalog.JobPartProduction &&
                job.targetId == partId && job.version == version)
                total += Math.Max(0L, job.quantity);
        }
        return total;
    }

    public static bool IsFirstSetPartPlanned(Dimension3State d3, string partId)
    {
        return D3InventorySystem.GetPartAmount(d3, partId, 1) +
            GetQueuedPartAmount(d3, partId, 1) > 0L;
    }

    public static bool IsAdvancedState(Dimension3State d3)
    {
        if (D3FacilitySystem.GetProcessBankLevel(d3) > 1 ||
            D3JobQueueSystem.GetJobCount(
                d3, Dimension3Catalog.QueueResearch) > 0 ||
            D3JobQueueSystem.GetJobCount(
                d3, Dimension3Catalog.QueueFacility) > 0 ||
            (d3.research != null && d3.research.Count > 0) ||
            (d3.calibrationProfiles != null && d3.calibrationProfiles.Count > 0) ||
            (d3.automationRoutines != null && d3.automationRoutines.Count > 0) ||
            (d3.automationProfiles != null && d3.automationProfiles.Count > 0))
            return true;

        if (d3.facilities != null)
        {
            for (int i = 0; i < d3.facilities.Count; i++)
            {
                D3FacilityState facility = d3.facilities[i];
                if (facility != null && facility.built &&
                    facility.facilityId != Dimension3Catalog.FacilityProcessBank)
                    return true;
            }
        }
        if (d3.parts != null)
        {
            for (int i = 0; i < d3.parts.Count; i++)
                if (d3.parts[i] != null && d3.parts[i].version > 1 &&
                    d3.parts[i].amount > 0L) return true;
        }
        return false;
    }

    private static void IntroduceReachedFeatures(
        DimensionPresentationState presentation,
        D3OnboardingSnapshot snapshot)
    {
        PresentationStateUtility.Introduce(presentation, PresentationFeatureIds.D3Factory);
        if (snapshot.stage >= D3OnboardingStage.AssignInitial)
            PresentationStateUtility.Introduce(
                presentation, PresentationFeatureIds.D3AssignmentBasic);
        if (snapshot.stage >= D3OnboardingStage.FirstPart)
            PresentationStateUtility.Introduce(
                presentation, PresentationFeatureIds.D3ProductionFirstPart);
        if (snapshot.stage >= D3OnboardingStage.CompleteSet)
            PresentationStateUtility.Introduce(
                presentation, PresentationFeatureIds.D3ProductionSet);
        if (snapshot.stage >= D3OnboardingStage.FirstAssembly)
            PresentationStateUtility.Introduce(
                presentation, PresentationFeatureIds.D3AssemblyNormal);
        if (snapshot.assemblyActive)
        {
            PresentationStateUtility.Introduce(
                presentation, PresentationFeatureIds.D3QueuesCompact);
            PresentationStateUtility.Introduce(
                presentation, PresentationFeatureIds.D3QueuesFull);
        }
    }
}

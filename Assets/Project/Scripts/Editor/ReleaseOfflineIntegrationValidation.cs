#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Synthetic integration fixtures, not evidence of natural progression or executed failures.
/// No menu entry, file I/O, SaveService component, or runtime mutation outside isolated state.
/// Run from the coordinating release runner in Edit Mode. Suspected defects remain unconfirmed
/// until that runner executes these assertions against the current runtime.
/// </summary>
public static class ReleaseOfflineIntegrationValidation
{
    private const double Epsilon = 0.000001;

    public static void Run(Action<string, Action> run)
    {
        Run(run, AdvanceWithBaseCallback);
    }

    /// <summary>
    /// The coordinator can inject another full offline entry point here, without changing
    /// fixtures or oracles. It must advance base production AND dimensions exactly once, must
    /// not save, and receives the isolated GameState. Cases 1 and 2 always test D3 directly.
    /// </summary>
    public static void Run(Action<string, Action> run,
        Action<GameState, double> advanceIntegratedOffline)
    {
        if (run == null || advanceIntegratedOffline == null)
            throw new ArgumentNullException(run == null ? nameof(run) : nameof(advanceIntegratedOffline));
        run("OFFLINE_D3_StabilizationDoesNotCreditEarlierWork", () =>
            WithFixture(ValidateStabilization));
        run("OFFLINE_D3_DiagnosticN5DoesNotAnalyzeBeforeCompletion", () =>
            WithFixture(ValidateDiagnosticUnlock));
        run("OFFLINE_LoadOrder_ConsolePurchaseDoesNotProduceBeforePurchase", () =>
            WithFixture(f => ValidateConsolePurchase(f, advanceIntegratedOffline, new[] { 60.0 }, 1)));
        run("OFFLINE_LoadOrder_ConsoleTwoShortAbsencesProduceWithoutTransaction", () =>
            WithFixture(f => ValidateConsolePurchase(f, advanceIntegratedOffline, new[] { 30.0, 30.0 }, 1)));
        run("OFFLINE_LoadOrder_ConsoleTwoPurchasesHaveCausalProduction", () =>
            WithFixture(f => ValidateConsolePurchase(f, advanceIntegratedOffline, new[] { 120.0 }, 2)));
    }

    private static void ValidateStabilization(Fixture f)
    {
        GameState state = f.State;
        D3CostTimeDefinition definition = Dimension3Catalog.GetPartDefinition(1);
        Require(definition != null, "SETUP: missing real V1 part definition.");
        const long quantity = 10;
        double duration = definition.durationSeconds * quantity;
        double window = Dimension3Catalog.AssignmentStabilizationSeconds;
        Near(duration, 100.0, "SETUP: real V1 batch baseline");
        Near(window, 30.0, "SETUP: real stabilization baseline");
        Require(duration > window, "SETUP: batch must outlast stabilization.");
        state.LE = definition.leCost * quantity;
        state.Traces = definition.tracesCost * quantity;
        D3InventorySystem.AddAutomatons(state.dimension3, 1, Dimension3Catalog.TraitNormal, 80);
        Require(D3FacilitySystem.TrySetProcessBankAssignment(state,
            Dimension3Catalog.ChannelProcessPower, 1, Dimension3Catalog.TraitNormal,
            80, out string reason), "SETUP: assignment rejected: " + reason);
        Require(Dimension3System.TryQueuePartProduction(state,
            Dimension3Catalog.PartChassis, 1, quantity, out reason),
            "SETUP: real chassis batch rejected: " + reason);
        D3QueueState queue = D3JobQueueSystem.GetQueue(state.dimension3,
            Dimension3Catalog.QueuePartProduction);
        Require(queue != null && queue.jobs.Count == 1, "SETUP: expected one production job.");
        Near(queue.jobs[0].remainingSeconds, duration, "SETUP: initial work");
        Near(D3PowerSystem.GetDynamicWorkRate(state.dimension3), 1.0, "SETUP: initial work rate");
        D3AssignmentState assignment = D3FacilitySystem.GetAssignment(state.dimension3,
            Dimension3Catalog.FacilityProcessBank, Dimension3Catalog.ChannelProcessPower,
            1, Dimension3Catalog.TraitNormal);
        Require(assignment != null && assignment.stabilizedAmount == 0,
            "SETUP: assignment must still be inactive.");
        Near(assignment.stabilizationRemainingSeconds, window, "SETUP: stabilization deadline");
        long partsBefore = D3InventorySystem.GetPartAmount(state.dimension3,
            Dimension3Catalog.PartChassis, 1);

        Near(Dimension3System.ApplyOfflineProgress(state, window), window, "Applied D3 time");

        // Independent oracle: rate is 1 throughout [0, stabilization deadline).
        // The bonus becomes effective only at the endpoint; no post-event time exists.
        Require(queue.jobs.Count == 1, "Batch completed before sufficient work elapsed.");
        Near(queue.jobs[0].remainingSeconds, duration - window,
            "Stabilization credited work before activation (expected 70s; suspected old signature 68.5s)");
        Require(assignment.stabilizedAmount == 80, "Assignment did not stabilize at the endpoint.");
        Require(D3InventorySystem.GetPartAmount(state.dimension3,
            Dimension3Catalog.PartChassis, 1) == partsBefore, "Unfinished batch delivered parts.");
    }

    private static void ValidateDiagnosticUnlock(Fixture f)
    {
        string target = PrepareSingleRealAnalysisTarget(f);
        ActivateFacility(f.State, Dimension3Catalog.FacilityAutomationCore, 5);
        ActivateFacility(f.State, Dimension3Catalog.FacilityDiagnosticBank, 4);
        D3FacilityLevelDefinition definition = Dimension3Catalog.GetFacilityLevelDefinition(
            Dimension3Catalog.FacilityDiagnosticBank, 5);
        Require(definition != null, "SETUP: missing diagnostic N5 definition.");
        D3InventorySystem.AddAssemblyCount(f.State.dimension3,
            definition.requiredAssemblyMk, definition.requiredAssemblyAmount);
        f.State.LE = definition.leCost;
        f.State.Traces = definition.tracesCost;
        Require(Dimension3System.TryQueueFacilityUpgrade(f.State,
            Dimension3Catalog.FacilityDiagnosticBank, out string reason),
            "SETUP: diagnostic N5 upgrade rejected: " + reason);
        D3QueueState queue = D3JobQueueSystem.GetQueue(f.State.dimension3,
            Dimension3Catalog.QueueFacility);
        Require(queue != null && queue.jobs.Count == 1, "SETUP: expected one facility job.");
        const double window = 12.0;
        Require(queue.jobs[0].remainingSeconds > window, "SETUP: upgrade shorter than fixture window.");
        // Represent a saved, legitimately committed upgrade near completion; retain its real
        // target, cost, duration and prerequisites. This is not a natural-progression assertion.
        queue.jobs[0].remainingSeconds = window;
        Near(D3PowerSystem.GetDynamicWorkRate(f.State.dimension3), 1.0, "SETUP: upgrade work rate");
        var settings = f.State.dimension3.diagnosticSettings;
        settings.autoAnalyzeEnabled = true;
        settings.evaluationRemainingSeconds = 1.0;
        Require(!D3DiagnosticSystem.CanRunOffline(f.State), "SETUP: N4 unexpectedly authorizes offline.");
        Require(!f.Machine.IsAnalyzingNode && !f.Machine.IsNodeAnalyzed(target),
            "SETUP: target must be pending, without a running analysis.");

        Near(Dimension3System.ApplyOfflineProgress(f.State, window), window, "Applied D3 time");

        Require(queue.jobs.Count == 0 && D3DiagnosticSystem.CanRunOffline(f.State),
            "Upgrade did not enable N5 at the endpoint.");
        Require(!f.Machine.IsNodeAnalyzed(target),
            "Node " + target + " completed using time before diagnostic N5 existed.");
        // A scheduler may evaluate exactly at the boundary or on its next interval. Both
        // conventions are valid, but neither can grant elapsed analysis time at that boundary.
        if (f.Machine.IsAnalyzingNode)
            Near(f.Machine.AnalysisRemainingSeconds, MachineManager.BaseNodeAnalysisDurationSeconds,
                "Newly unlocked analysis received pre-unlock time: " + target);
        Dimension3System.ApplyOfflineProgress(f.State,
            MachineManager.BaseNodeAnalysisDurationSeconds + 2.0);
        Require(f.Machine.IsNodeAnalyzed(target),
            "Positive control: real target did not complete with sufficient post-unlock time: " + target);
    }

    private static void ValidateConsolePurchase(Fixture f, Action<GameState, double> advance,
        double[] windows, int stopAfter)
    {
        TextAsset json = Resources.Load<TextAsset>("Data/buildings");
        Require(json != null, "SETUP: missing real building catalog.");
        BuildingCollection catalog = JsonUtility.FromJson<BuildingCollection>(json.text);
        Require(catalog != null && catalog.buildings != null, "SETUP: invalid building catalog.");
        BuildingDef definition = Array.Find(catalog.buildings,
            b => b != null && b.id == D3ConsoleSystem.BuildingHiggs);
        Require(definition != null, "SETUP: real Higgs building missing.");
        Require(definition.tickInterval > 0 && definition.lePerTickBase > 0 &&
            definition.baseLEps == 0 && definition.bonusType == BuildingBonusType.None,
            "SETUP: oracle requires the catalog's plain tick-based Higgs producer.");
        Near(definition.tickInterval, 1.0, "SETUP: real Higgs tick interval");
        Near(definition.lePerTickBase, 1.0, "SETUP: real Higgs output per level");
        f.State.PrepareBuildingLevelsForOffline(new List<SavedBuildingLevel>
        {
            new SavedBuildingLevel { id = definition.id, level = 1 }
        }, catalog.buildings);
        BuildingState building = f.State.GetBuildingState(definition.id);
        Require(building != null && building.level == 1, "SETUP: saved building not restored.");
        Near(building.tickTimer, 0, "SETUP: initial building tick timer");
        ActivateFacility(f.State, Dimension3Catalog.FacilityAutomationCore, 5);
        ActivateFacility(f.State, Dimension3Catalog.FacilityProductionConsole, 5);
        D3ConsoleSystem.RecordManualBuildingPurchase(f.State, definition.id);
        Require(D3ConsoleSystem.TrySetPolicyAndReserves(f.State,
            D3ConsoleSystem.PolicyLE, 0, 0, out string reason), "SETUP: console policy: " + reason);
        Require(D3AutomationSystem.TryCreateRoutine(f.State,
            D3AutomationCatalog.ActionConsoleBuyHiggs, "", null, 0, 0, 0, "", 0, stopAfter,
            out D3AutomationRoutineState routine, out reason), "SETUP: routine: " + reason);
        Require(D3AutomationSystem.TrySetRoutineEnabled(f.State, routine.routineId, true, out reason),
            "SETUP: enable routine: " + reason);
        Require(D3AutomationSystem.CanRunAutomationOffline(f.State), "SETUP: offline routine unauthorized.");
        // Independent costs from the real catalog; no console cost/coordination assignments.
        double nextRawCost = definition.baseCost * definition.costMult; // saved level 1
        double firstCost = Math.Ceiling(nextRawCost);
        double secondCost = Math.Ceiling(nextRawCost * definition.costMult);
        Near(D3ConsoleSystem.GetAutomatedBuildingCost(f.State, building), firstCost,
            "SETUP: neutral console must charge catalog cost");
        f.State.LE = 2 * (firstCost + secondCost);
        double expectedBalance = f.State.LE;
        int expectedLevel = 1;
        int expectedPurchases = 0;
        double expectedTotalGain = 0;
        foreach (double window in windows)
        {
            // Existing contract: only COMPLETE 60s blocks of EACH absence transact.
            // 30+30 checks two short absences, not a newly invented persistent remainder.
            // Production is continuous across calls; the producer timer is never reset.
            double remaining = window;
            while (remaining >= 60.0)
            {
                double gain = 60.0 * definition.lePerTickBase * expectedLevel;
                expectedBalance += gain;
                expectedTotalGain += gain;
                if (expectedPurchases < stopAfter)
                {
                    expectedBalance -= Math.Ceiling(nextRawCost);
                    nextRawCost *= definition.costMult;
                    expectedLevel++;
                    expectedPurchases++;
                }
                remaining -= 60.0;
            }
            double remainderGain = remaining * definition.lePerTickBase * expectedLevel;
            expectedBalance += remainderGain;
            expectedTotalGain += remainderGain;

            advance(f.State, window);

            Require(building.level == expectedLevel && routine.executionsCompleted == expectedPurchases &&
                routine.enabled == (expectedPurchases < stopAfter),
                "Expected boundary purchases=" + expectedPurchases + "; level=" + building.level +
                ", executions=" + routine.executionsCompleted + ", result=" + routine.lastResult);
            Near(f.State.LE, expectedBalance,
                "Pre-purchase production or duplicate time; cumulative oracle gain=" + expectedTotalGain);
        }
    }

    private static void AdvanceWithBaseCallback(GameState state, double seconds)
    {
        // Current production API used by SaveService for an open D3. D1/D2 are locked in
        // these fixtures. No second base-production call: D3 owns the callback's time.
        Require(SaveService.I == null, "SaveService must remain isolated.");
        double callbackSeconds = 0;
        double applied = Dimension3System.ApplyOfflineProgress(state, seconds, step =>
        {
            Require(step >= 0 && !double.IsNaN(step) && !double.IsInfinity(step),
                "Invalid base-production callback interval.");
            callbackSeconds += step;
            state.ApplyOfflineBaseProgress(step);
        });
        double expectedSeconds = Math.Min(seconds, Dimension3Catalog.OfflineProgressCapSeconds);
        Near(applied, expectedSeconds, "Integrated offline applied time");
        Near(callbackSeconds, expectedSeconds, "Base callback must receive the window exactly once");
    }

    private static string PrepareSingleRealAnalysisTarget(Fixture f)
    {
        f.State.triangleEnergy = MachineManager.NodeAnalysisEnergyCost;
        List<MachineNodeDef> nodes = f.Machine.GetAllNodes(true);
        nodes.Sort((a, b) => string.CompareOrdinal(a.id, b.id));
        var progress = new SaveData
        {
            machineUnlocked = true, machineIntroSeen = true,
            machineRepairedNodeIds = new List<string>(), machineAnalyzedNodeIds = new List<string>()
        };
        foreach (MachineNodeDef node in nodes)
            if (node.effectType == MachineNodeEffectType.UnlockDiagnostics)
                progress.machineRepairedNodeIds.Add(node.id);
        Require(progress.machineRepairedNodeIds.Count > 0, "SETUP: no real diagnostic unlock node.");
        f.Machine.LoadProgressFromSave(progress);
        string target = null;
        foreach (MachineNodeDef node in nodes)
            if (f.Machine.CanAnalyzeNode(node.id, true, out _)) { target = node.id; break; }
        Require(target != null, "SETUP: no real automatable node available.");
        foreach (MachineNodeDef node in nodes)
            if (node.damaged && node.id != target) progress.machineAnalyzedNodeIds.Add(node.id);
        f.Machine.LoadProgressFromSave(progress);
        Require(f.Machine.CanAnalyzeNode(target, true, out string reason), "SETUP: target: " + reason);
        return target;
    }

    private static void ActivateFacility(GameState state, string id, int level)
    {
        D3FacilityState facility = D3FacilitySystem.GetFacility(state.dimension3, id);
        Require(facility != null, "SETUP: missing real facility " + id);
        facility.built = true;
        facility.level = level;
        // Five real MK6 normals provide sqrt(5*280) > 35 capacity, even without affinity.
        D3InventorySystem.AddAutomatons(state.dimension3, 6, Dimension3Catalog.TraitNormal, 5);
        string channel = D3FacilitySystem.GetCapacityChannel(id);
        Require(D3FacilitySystem.TrySetFacilityAssignment(state, id, channel, 6,
            Dimension3Catalog.TraitNormal, 5, out string reason), "SETUP: capacity: " + reason);
        D3FacilitySystem.AdvanceStabilization(state.dimension3, Dimension3Catalog.AssignmentStabilizationSeconds);
        Require(D3FacilitySystem.IsFunctionActive(state.dimension3, id, level), "SETUP: inactive facility " + id);
    }

    private static void WithFixture(Action<Fixture> body)
    {
        Require(!EditorApplication.isPlayingOrWillChangePlaymode, "Fixtures require Edit Mode.");
        Type[] types = { typeof(SaveService), typeof(GameState), typeof(MachineManager),
            typeof(BuildingDatabase), typeof(F2UpgradeManager), typeof(ResearchManager), typeof(AchievementManager) };
        var fields = new FieldInfo[types.Length];
        var previous = new object[types.Length];
        // Resolve and snapshot everything before changing any singleton.
        for (int i = 0; i < types.Length; i++)
        {
            fields[i] = types[i].GetField("<I>k__BackingField", BindingFlags.Static | BindingFlags.NonPublic);
            Require(fields[i] != null, "Missing singleton backing field: " + types[i].Name);
            previous[i] = fields[i].GetValue(null);
        }
        UnityEngine.Random.State randomBefore = UnityEngine.Random.state;
        GameObject root = null;
        try
        {
            for (int i = 0; i < fields.Length; i++) fields[i].SetValue(null, null);
            UnityEngine.Random.InitState(903120);
            root = new GameObject("Release Offline Integration Fixture")
                { hideFlags = HideFlags.HideAndDontSave };
            root.SetActive(false); // Prevent Awake, Update and application lifecycle participation.
            var f = new Fixture { State = root.AddComponent<GameState>(), Machine = root.AddComponent<MachineManager>() };
            fields[1].SetValue(null, f.State);
            fields[2].SetValue(null, f.Machine);
            f.State.dimension03Unlocked = true;
            f.State.dimension01Unlocked = false;
            f.State.dimension02Unlocked = false;
            f.State.baseLEps = 0;
            f.State.researchGlobalLEMult = 1;
            f.State.triangleSystemUnlocked = false;
            f.State.dimension3 = Dimension3System.CreateInitialState();
            Dimension3System.EnsureState(f.State);
            var settings = f.State.dimension3.diagnosticSettings;
            settings.autoAnalyzeEnabled = settings.autoRepairEnabled = settings.autoFusionEnabled = false;
            Require(SaveService.I == null, "SETUP: SaveService must remain null.");
            body(f);
            Require(SaveService.I == null, "Tested action changed SaveService singleton.");
        }
        finally
        {
            try
            {
                fields[0].SetValue(null, null);
                if (root != null) UnityEngine.Object.DestroyImmediate(root);
            }
            finally
            {
                // Restore SaveService last, including when fixture setup or teardown fails.
                try
                {
                    for (int i = fields.Length - 1; i >= 0; i--) fields[i].SetValue(null, previous[i]);
                }
                finally { UnityEngine.Random.state = randomBefore; }
            }
        }
    }

    private static void Near(double actual, double expected, string context)
    {
        Require(!double.IsNaN(actual) && !double.IsInfinity(actual) && Math.Abs(actual - expected) <= Epsilon,
            context + ": expected=" + expected.ToString("R") + ", observed=" + actual.ToString("R"));
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    [Serializable]
    private sealed class BuildingCollection { public BuildingDef[] buildings; }
    private sealed class Fixture { public GameState State; public MachineManager Machine; }
}
#endif

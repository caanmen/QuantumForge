#if UNITY_EDITOR
using System;
using UnityEngine;

/// <summary>Isolated synthetic timing checkpoints; invoked by the coordinator's runner.</summary>
public static class ReleaseD2TimingValidation
{
    public static void Run(Action<string, Action> run)
    {
        if (run == null) throw new ArgumentNullException(nameof(run));

        run("D2_Timing_EndRewardHasNoPast", () => WithState(game =>
        {
            PrepareAnalysis(game, D2Civilization3System.Zone1Id, 30.0);
            D2Civilization3System.Tick(game, 30.0);
            var state = game.dimension2.civilization3;
            Require(Near(state.entityResearchProgress, 0.0) &&
                Near(state.ancientKnowledge, 1.0) && state.entityResearchActive,
                "Knowledge arriving at t=30 cannot fund [0,30]. Research must keep waiting.");
            D2Civilization3System.Tick(game, 15.0);
            Require(Near(state.entityResearchProgress, 0.5) &&
                Near(state.ancientKnowledge, 0.5), "The next 15 seconds use half the reward.");
        }));

        run("D2_Timing_InitialFuelExhaustsBeforeReward", () => WithState(game =>
        {
            PrepareAnalysis(game, D2Civilization3System.Zone1Id, 15.0);
            game.dimension2.civilization3.ancientKnowledge = 0.25;
            D2Civilization3System.Tick(game, 30.0);
            var state = game.dimension2.civilization3;
            // Active [0,7.5], waiting [7.5,15], active [15,30].
            Require(Near(state.entityResearchProgress, 0.75) &&
                Near(state.ancientKnowledge, 0.5), "Only 22.5 seconds have fuel.");
        }));

        run("D2_Timing_MultipleIntervalsAndPartitions", () =>
        {
            foreach (double total in new[] { 7.5, 30.0, 60.0, 90.0 })
            foreach (double interval in new[] { 0.25, 1.0, 2.5, 7.0 })
            {
                WithState(whole => WithState(split =>
                {
                    PrepareMultiple(whole);
                    PrepareMultiple(split);
                    UnityEngine.Random.InitState(731);
                    D2Civilization3System.Tick(whole, total);
                    UnityEngine.Random.InitState(731);
                    double elapsed = 0.0;
                    while (elapsed < total)
                    {
                        double step = Math.Min(interval, total - elapsed);
                        D2Civilization3System.Tick(split, step);
                        elapsed += step;
                    }
                    RequireEquivalent(whole.dimension2.civilization3,
                        split.dimension2.civilization3);
                }));
            }
        });

        run("D2_Timing_SimultaneousMilestoneAndJobs", () => WithState(game =>
        {
            PrepareAnalysis(game, D2Civilization3System.Zone1Id, 30.0);
            PrepareAnalysis(game, D2Civilization3System.Zone2Id, 30.0);
            var state = game.dimension2.civilization3;
            state.entityResearchProgress = 29.0;
            state.ancientKnowledge = 1.0;
            var zone = D2Civilization3System.GetZone(state, D2Civilization3System.Zone1Id);
            zone.excavationActive = true;
            zone.excavationRemainingSeconds = 30.0;
            D2Civilization3System.Tick(game, 30.0);
            Require(Near(state.entityResearchProgress, 30.0) &&
                Near(state.ancientKnowledge, 2.0) && !state.entityResearchActive &&
                !state.entityResearchMilestone30Completed &&
                zone.totalExcavationsCompleted == 1 && zone.totalAnalysesCompleted == 1 &&
                D2Civilization3System.GetZone(state, D2Civilization3System.Zone2Id)
                    .totalAnalysesCompleted == 1,
                "Simultaneous jobs finish once; research stops at the unpaid milestone.");
            string completed = JsonUtility.ToJson(state);
            D2Civilization3System.Tick(game, 30.0);
            Require(completed == JsonUtility.ToJson(state), "Completed jobs cannot repeat.");
        }));

        run("D2_Timing_DueAtStartAndPositiveSubmicrosecond", () => WithState(game =>
        {
            PrepareAnalysis(game, D2Civilization3System.Zone1Id, 0.0);
            var state = game.dimension2.civilization3;
            var zone = D2Civilization3System.GetZone(state, D2Civilization3System.Zone1Id);
            zone.excavationActive = true;
            zone.excavationRemainingSeconds = 0.0;
            D2Civilization3System.Tick(game, 15.0);
            Require(Near(state.entityResearchProgress, 0.5) &&
                Near(state.ancientKnowledge, 0.5) && zone.totalExcavationsCompleted == 1,
                "Jobs due at t=0 resolve before the positive interval.");
            PrepareAnalysis(game, zone.zoneId, 0.0000005);
            zone.excavationActive = true;
            zone.excavationRemainingSeconds = 0.0000005;
            D2Civilization3System.Tick(game, 0.00000025);
            Require(zone.analysisActive && zone.excavationActive,
                "A positive remaining interval must not finish early via an epsilon.");
            D2Civilization3System.Tick(game, 0.00000025);
            Require(!zone.analysisActive && !zone.excavationActive &&
                zone.totalAnalysesCompleted == 2 && zone.totalExcavationsCompleted == 2,
                "Submicrosecond jobs finish at their actual endpoint.");
        }));

        run("D2_Timing_FiniteIntervalsAndFilters", () => WithState(game =>
        {
            PrepareMultiple(game);
            var state = game.dimension2.civilization3;
            string before = JsonUtility.ToJson(state);
            foreach (double invalid in new[] { 0.0, -1.0, double.NaN,
                double.PositiveInfinity, double.NegativeInfinity })
                D2Civilization3System.Tick(game, invalid);
            Require(before == JsonUtility.ToJson(state), "Invalid intervals must be no-ops.");
            game.dimension2.civilization3Unlocked = false;
            D2Civilization3System.Tick(game, 30.0);
            Require(before == JsonUtility.ToJson(state), "Locked civilization cannot advance.");
            game.dimension2.civilization3Unlocked = true;
            var zone = D2Civilization3System.GetZone(state, D2Civilization3System.Zone3Id);
            zone.unlocked = false;
            var invalidAnalysis = D2Civilization3System.GetZone(state, D2Civilization3System.Zone2Id);
            invalidAnalysis.analysisQualityId = "invalid";
            D2Civilization3System.Tick(game, double.MaxValue);
            Require(zone.totalAnalysesCompleted == 0 && zone.totalExcavationsCompleted == 0 &&
                invalidAnalysis.totalAnalysesCompleted == 0 &&
                Near(state.entityResearchProgress, 1.25) && Near(state.ancientKnowledge, 0.0),
                "Huge finite ticks terminate; locked zones and invalid analyses yield nothing.");
        }));

        run("D2_Archive_ApprovedRequirementsCostsAndEffects", () =>
        {
            CheckArchiveUpgrade(D2Civilization3System.AnomalousConcordanceUpgradeId,
                D2Civilization3System.Zone2Id, 3, 75.0, 35L);
            CheckArchiveUpgrade(D2Civilization3System.DeepExegesisUpgradeId,
                D2Civilization3System.Zone3Id, 4, 100.0, 50L);
            WithState(game =>
            {
                var state = game.dimension2.civilization3;
                state.archiveUnlocked = true;
                state.archiveLevel = 2;
                state.ancientKnowledge = 50.0;
                D2Civilization3System.GetZone(state, D2Civilization3System.Zone1Id)
                    .zoneResourceAmount = 25L;
                string id = D2Civilization3System.StratifiedCartographyUpgradeId;
                Require(D2Civilization3System.GetArchiveUpgradeEntityKnowledgeRequirement(id) == 1 &&
                    !D2Civilization3System.TryUnlockArchiveUpgrade(game, id),
                    "Cartography still needs one Entity Knowledge.");
                state.entityKnowledge = 1;
                Require(D2Civilization3System.TryUnlockArchiveUpgrade(game, id) &&
                    state.entityKnowledge == 1 && Near(state.ancientKnowledge, 0.0) &&
                    Near(D2Civilization3System.GetExcavationDuration(state), 28.5),
                    "Cartography retains its cost, non-spendable requirement and effect.");
            });
        });
    }

    private static void CheckArchiveUpgrade(string id, string zoneId, int level,
        double knowledgeCost, long resourceCost)
    {
        WithState(game =>
        {
            var state = game.dimension2.civilization3;
            var zone = D2Civilization3System.GetZone(state, zoneId);
            zone.unlocked = true;
            state.archiveUnlocked = true;
            state.archiveLevel = level - 1;
            state.ancientKnowledge = knowledgeCost;
            zone.zoneResourceAmount = resourceCost;
            Require(D2Civilization3System.GetArchiveUpgradeEntityKnowledgeRequirement(id) == 0 &&
                !D2Civilization3System.TryUnlockArchiveUpgrade(game, id),
                "Removing Entity Knowledge does not remove the Archive level requirement.");
            state.archiveLevel = level;
            state.ancientKnowledge = knowledgeCost - 1.0;
            Require(!D2Civilization3System.TryUnlockArchiveUpgrade(game, id), "Knowledge cost remains.");
            state.ancientKnowledge = knowledgeCost;
            zone.zoneResourceAmount = resourceCost - 1;
            Require(!D2Civilization3System.TryUnlockArchiveUpgrade(game, id), "Resource cost remains.");
            zone.zoneResourceAmount = resourceCost;
            Require(D2Civilization3System.TryUnlockArchiveUpgrade(game, id) &&
                state.entityKnowledge == 0 && Near(state.ancientKnowledge, 0.0) &&
                zone.zoneResourceAmount == 0 && !zone.anomalyRead &&
                !D2Civilization3System.TryUnlockArchiveUpgrade(game, id),
                "Upgrade is purchasable before reading anomalies, at the unchanged cost, once.");
            if (id == D2Civilization3System.AnomalousConcordanceUpgradeId)
            {
                state.anomalyClueDetectionUnlocked = true;
                zone.scholarHired = true;
                zone.scholarLevel = 1;
                zone.highQualityRemains = 1;
                Require(D2Civilization3System.TryStartAnalysis(game, zoneId,
                    D2Civilization3System.HighQualityId), "Start clue-producing analysis.");
                D2Civilization3System.Tick(game, 30.0);
                Require(Near(zone.anomalyClueProgress, 0.198), "Concordance keeps its 10% clue bonus.");
            }
            else
            {
                Require(Near(D2Civilization3System.GetEffectiveAnomalyKnowledgeCost(state, zoneId), 54.0) &&
                    D2Civilization3System.GetEffectiveAnomalyResourceCost(state, zoneId) == 32,
                    "Exegesis keeps its 10% reduction and resource rounding.");
            }
        });
    }

    private static void PrepareMultiple(GameState game)
    {
        PrepareAnalysis(game, D2Civilization3System.Zone1Id, 5.0);
        PrepareAnalysis(game, D2Civilization3System.Zone2Id, 15.0);
        PrepareAnalysis(game, D2Civilization3System.Zone3Id, 30.0);
        game.dimension2.civilization3.ancientKnowledge = 0.25;
        // Reverse excavation order to expose zone-order processing instead of time order.
        double[] deadlines = { 25.0, 10.0, 5.0 };
        for (int i = 0; i < 3; i++)
        {
            var zone = D2Civilization3System.GetZone(game.dimension2.civilization3,
                D2Civilization3System.ZoneIds[i]);
            zone.excavationActive = true;
            zone.excavationRemainingSeconds = deadlines[i];
        }
    }

    private static void PrepareAnalysis(GameState game, string zoneId, double remaining)
    {
        var state = game.dimension2.civilization3;
        var zone = D2Civilization3System.GetZone(state, zoneId);
        zone.unlocked = true;
        zone.scholarHired = true;
        zone.scholarLevel = 1;
        zone.lowQualityRemains = 1;
        Require(D2Civilization3System.TryStartAnalysis(game, zoneId,
            D2Civilization3System.LowQualityId), "Analysis must start via its public API.");
        zone.analysisRemainingSeconds = remaining; // Synthetic in-flight checkpoint.
        state.entityResearchUnlocked = true;
        if (!state.entityResearchActive)
            Require(D2Civilization3System.TryStartEntityResearch(game), "Research must start.");
    }

    private static void RequireEquivalent(D2Civilization3State a, D2Civilization3State b)
    {
        Require(Near(a.entityResearchProgress, b.entityResearchProgress) &&
            Near(a.ancientKnowledge, b.ancientKnowledge) &&
            a.entityResearchActive == b.entityResearchActive && a.lastResult == b.lastResult &&
            a.archiveLevel == b.archiveLevel, "Research, fuel and event order must be partition invariant.");
        foreach (string id in D2Civilization3System.ZoneIds)
        {
            var x = D2Civilization3System.GetZone(a, id);
            var y = D2Civilization3System.GetZone(b, id);
            Require(x.analysisActive == y.analysisActive && x.excavationActive == y.excavationActive &&
                Near(x.analysisRemainingSeconds, y.analysisRemainingSeconds) &&
                Near(x.excavationRemainingSeconds, y.excavationRemainingSeconds) &&
                x.totalAnalysesCompleted == y.totalAnalysesCompleted &&
                x.totalExcavationsCompleted == y.totalExcavationsCompleted &&
                x.lowQualityRemains == y.lowQualityRemains &&
                x.mediumQualityRemains == y.mediumQualityRemains &&
                x.highQualityRemains == y.highQualityRemains &&
                x.zoneResourceAmount == y.zoneResourceAmount &&
                Near(x.researchProgress, y.researchProgress), "Zone outcomes must be partition invariant: " + id);
        }
    }

    private static void WithState(Action<GameState> body)
    {
        var random = UnityEngine.Random.state;
        var root = new GameObject("Release D2 timing fixture");
        try
        {
            root.SetActive(false);
            root.hideFlags = HideFlags.HideAndDontSave;
            var game = root.AddComponent<GameState>();
            game.dimension02Unlocked = true;
            game.dimension2 = Dimension2System.CreateInitialState();
            game.dimension2.civilization3Unlocked = true;
            D2Civilization3System.EnsureState(game.dimension2.civilization3);
            body(game);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(root);
            UnityEngine.Random.state = random;
        }
    }

    private static bool Near(double a, double b)
    {
        return !double.IsNaN(a) && !double.IsInfinity(a) &&
            !double.IsNaN(b) && !double.IsInfinity(b) && Math.Abs(a - b) <= 1e-8;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
#endif

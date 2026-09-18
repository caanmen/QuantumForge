#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// D2 cases for the coordinator's runner. Synthetic checkpoints are QA isolation only,
/// not evidence of balance, natural progression or save migration. No global services.
/// Contract failures describe desired invariants; they do not prescribe mechanic fixes.
/// </summary>
public static class ReleaseD2RegressionCases
{
    public static void Run(Action<string, Action> run)
    {
        if (run == null)
            throw new InvalidOperationException("D2 requires a case runner.");

        run("D2_C3_EndFuel_TickPartitionInvariant", () =>
        {
            WithState(whole => WithState(split =>
            {
                PrepareEndFuel(whole);
                PrepareEndFuel(split);
                D2Civilization3System.Tick(whole, 30.0);
                for (int i = 0; i < 30; i++)
                    D2Civilization3System.Tick(split, 1.0);

                var a = whole.dimension2.civilization3;
                var b = split.dimension2.civilization3;
                Require(D2Civilization3System.GetZone(a, D2Civilization3System.Zone1Id)
                    .totalAnalysesCompleted == 1L &&
                    D2Civilization3System.GetZone(b, D2Civilization3System.Zone1Id)
                    .totalAnalysesCompleted == 1L, "Setup: both analyses must finish.");
                Require(Near(a.entityResearchProgress, b.entityResearchProgress) &&
                    Near(a.ancientKnowledge, b.ancientKnowledge),
                    "Fuel arriving at t=30 must not retroactively fund the full interval. " +
                    "Tick(30): research=" + a.entityResearchProgress + ", knowledge=" +
                    a.ancientKnowledge + "; Tick(1)*30: research=" + b.entityResearchProgress +
                    ", knowledge=" + b.ancientKnowledge + ".");
            }));
        });

        run("D2_C3_Concordance_BenefitOpportunityContract", () =>
        {
            WithState(state => RequireUpgradeOpportunity(state,
                D2Civilization3System.AnomalousConcordanceUpgradeId));
        });

        run("D2_C3_Exegesis_BenefitOpportunityContract", () =>
        {
            WithState(state => RequireUpgradeOpportunity(state,
                D2Civilization3System.DeepExegesisUpgradeId));
        });

        run("D2_Completion_ExistingValidatorFixture_NormalizationContract", () =>
        {
            // Exercise the actual existing factory, so repairing that factory
            // makes this regression pass without changing this test.
            var factory = typeof(DimensionCompletionValidation).GetMethod(
                "CreateCompletedState", BindingFlags.NonPublic | BindingFlags.Static);
            Require(factory != null, "Existing completion fixture factory not found.");
            var state = (GameState)factory.Invoke(null, null);
            try
            {
                // D2-only reproduction of DimensionCompletionValidation.CreateCompletedState:
                // that existing fixture sets majorPactEstablished without entityContained.
                // This is a FIXTURE contract failure, not a demand to preserve invalid saves
                // or weaken normalization. Do not invoke its all-dimension/Convergence runner.
                Require(DimensionCompletionService.IsDimensionCompleted(state, 2),
                    "Setup: the existing validator's D2 slice must initially report complete.");
                Dimension2System.EnsureState(state);
                Require(DimensionCompletionService.IsDimensionCompleted(state, 2),
                    "Fixture defect in DimensionCompletionValidation.CreateCompletedState: " +
                    "its D2 completed checkpoint loses completion after EnsureState. " +
                    "Review the fixture's containment prerequisites, not the normalizer.");
            }
            finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
        });

        run("D2_Completion_ValidPact_SurvivesNormalization_Control", () =>
        {
            WithState(state =>
            {
                // Authorized synthetic containment checkpoint; establish via the real API.
                state.dimension2.civilization2Unlocked = true;
                state.dimension2.civilization2.entityContained = true;
                D2Civilization2System.EnsureState(state.dimension2.civilization2);
                Require(D2Civilization2System.TryEstablishMajorPact(state),
                    "Setup: contained entity must allow establishing the Major Pact.");
                Dimension2System.EnsureState(state);
                Require(DimensionCompletionService.IsDimensionCompleted(state, 2),
                    "A valid established Major Pact must survive D2 normalization.");
            });
        });

        run("D2_Pilgrimage_NoDuplicateFinalization_Control", () =>
        {
            WithState(state =>
            {
                var c1 = state.dimension2.civilization1;
                string id = D2PilgrimageSystem.ShortId;
                c1.followersAvailable = D2PilgrimageSystem.GetFollowersRequired(id);
                var wax = D2AltarSystem.GetAltar(c1, D2AltarSystem.WaxAltarId);
                var bread = D2AltarSystem.GetAltar(c1, D2AltarSystem.RitualBreadAltarId);
                wax.offeringAmount = D2PilgrimageSystem.GetEffectiveWaxCost(c1, id);
                bread.offeringAmount = D2PilgrimageSystem.GetEffectiveBreadCost(c1, id);
                Require(D2PilgrimageSystem.TryStart(state, id),
                    "Setup: funded short pilgrimage must start.");
                D2PilgrimageSystem.Tick(state, D2PilgrimageSystem.GetDurationSeconds(id));
                Require(!c1.activePilgrimage.active && c1.totalPilgrimagesCompleted == 1L &&
                    c1.shortPilgrimagesCompleted == 1L &&
                    c1.followersAvailable == D2PilgrimageSystem.GetFollowersRequired(id),
                    "Short pilgrimage must finish once and return committed followers.");
                string completed = JsonUtility.ToJson(c1);
                D2PilgrimageSystem.Tick(state, D2PilgrimageSystem.GetDurationSeconds(id));
                Require(JsonUtility.ToJson(c1) == completed,
                    "A second Tick without restart must not duplicate completion or rewards.");
                // CompleteActive has an internal synchronization hook. Fresh incomplete D2
                // cannot synchronize; this case asserts only D2, never Convergence behavior.
            });
        });
    }

    private static void PrepareEndFuel(GameState state)
    {
        var c3 = state.dimension2.civilization3;
        state.dimension2.civilization3Unlocked = true;
        // Isolate timing: research is unlocked but has zero fuel. A single base-speed
        // analysis supplies the first knowledge at exactly the end of the 30-second window.
        c3.entityResearchUnlocked = true;
        c3.ancientKnowledge = 0.0;
        var zone = D2Civilization3System.GetZone(c3, D2Civilization3System.Zone1Id);
        zone.scholarHired = true;
        zone.scholarLevel = 1;
        zone.lowQualityRemains = 1L;
        Require(D2Civilization3System.TryStartAnalysis(state, zone.zoneId,
            D2Civilization3System.LowQualityId), "Setup: analysis must start.");
        Require(Near(zone.analysisRemainingSeconds, 30.0),
            "Setup: this boundary regression requires a 30-second catalog analysis.");
        Require(D2Civilization3System.TryStartEntityResearch(state),
            "Setup: unlocked entity research must wait for knowledge.");
    }

    private static void RequireUpgradeOpportunity(GameState state, string upgradeId)
    {
        var c3 = state.dimension2.civilization3;
        state.dimension2.civilization3Unlocked = true;
        // Synthetic ample ordinary funding and zone/archive access isolate the dependency
        // contract. Entity knowledge, anomaly reads and milestone rewards are NOT granted:
        // earn them using the catalog APIs. No new resource sources or repeatable reads.
        c3.archiveUnlocked = true;
        c3.archiveLevel = D2Civilization3System.GetArchiveUpgradeRequiredLevel(
            D2Civilization3System.DeepExegesisUpgradeId);
        c3.anomalyClueDetectionUnlocked = true;
        c3.ancientKnowledge = D2Civilization3System.GetArchiveUpgradeKnowledgeCost(upgradeId) +
            D2Civilization3System.EntityResearchMilestone85 *
            D2Civilization3System.EntityResearchKnowledgePerPercent;
        foreach (string zoneId in D2Civilization3System.ZoneIds)
        {
            var zone = D2Civilization3System.GetZone(c3, zoneId);
            zone.unlocked = true;
            zone.zoneResourceAmount = D2Civilization3System.GetAnomalyResourceCost(zoneId) +
                D2Civilization3System.EntityMilestone30ZoneResourceCost +
                D2Civilization3System.EntityMilestone60ZoneResourceCost +
                D2Civilization3System.EntityMilestone85ZoneResourceCost +
                D2Civilization3System.GetArchiveUpgradeResourceCost(upgradeId);
            c3.ancientKnowledge += D2Civilization3System.GetAnomalyKnowledgeCost(zoneId);
        }

        foreach (string zoneId in D2Civilization3System.ZoneIds)
        {
            if (TryPurchaseWithPendingBenefit(state, upgradeId)) return;
            var zone = D2Civilization3System.GetZone(c3, zoneId);
            // Skip only clue farming; thresholds are from the real catalog.
            zone.anomalyClues = D2Civilization3System.GetAnomalyClueRequirement(zoneId);
            D2Civilization3System.EnsureState(c3);
            if (TryPurchaseWithPendingBenefit(state, upgradeId)) return;
            Require(D2Civilization3System.TryReadAnomaly(state, zoneId),
                "Setup: funded revealed anomaly must be readable: " + zoneId);
        }

        double[] milestones = {
            D2Civilization3System.EntityResearchMilestone30,
            D2Civilization3System.EntityResearchMilestone60,
            D2Civilization3System.EntityResearchMilestone85
        };
        foreach (double milestone in milestones)
        {
            Require(D2Civilization3System.TryStartEntityResearch(state),
                "Setup: entity research must start before milestone " + milestone);
            D2Civilization3System.Tick(state, (milestone - c3.entityResearchProgress) *
                D2Civilization3System.EntityResearchSecondsPerPercent);
            Require(D2Civilization3System.TryPayEntityResearchMilestone(state),
                "Setup: funded entity milestone must pay: " + milestone);
            if (TryPurchaseWithPendingBenefit(state, upgradeId)) return;
            if (D2Civilization3System.CanUnlockArchiveUpgrade(state, upgradeId)) break;
        }

        Require(D2Civilization3System.CanUnlockArchiveUpgrade(state, upgradeId),
            "Setup: catalog milestone rewards must eventually make the upgrade affordable.");
        Require(false, "CONTRACT: " + D2Civilization3System.GetArchiveUpgradeName(upgradeId) +
            " becomes purchasable only after all anomaly targets have been consumed. " +
            "No useful target existed while purchase was available on this isolated route. " +
            "Review benefit timing/design; this case does not authorize new mechanics.");
    }

    private static bool TryPurchaseWithPendingBenefit(GameState state, string upgradeId)
    {
        if (!D2Civilization3System.CanUnlockArchiveUpgrade(state, upgradeId)) return false;
        foreach (string zoneId in D2Civilization3System.ZoneIds)
        {
            var zone = D2Civilization3System.GetZone(state.dimension2.civilization3, zoneId);
            bool pending = !zone.anomalyRead && (upgradeId ==
                D2Civilization3System.DeepExegesisUpgradeId || (!zone.anomalyRevealed &&
                zone.anomalyClues < D2Civilization3System.GetAnomalyClueRequirement(zoneId)));
            if (!zone.unlocked || !pending) continue;
            Require(D2Civilization3System.TryUnlockArchiveUpgrade(state, upgradeId),
                "Available upgrade must be purchasable while its benefit has a pending target.");
            return true;
        }
        return false;
    }

    private static void WithState(Action<GameState> body)
    {
        var root = new GameObject("Release D2 isolated regression fixture");
        try
        {
            root.SetActive(false); // Before AddComponent: never run GameState.Awake/Start.
            root.hideFlags = HideFlags.HideAndDontSave;
            var state = root.AddComponent<GameState>();
            state.dimension02Unlocked = true;
            state.dimension2 = Dimension2System.CreateInitialState();
            Dimension2System.EnsureState(state);
            body(state);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(root);
        }
    }

    private static bool Near(double a, double b)
    {
        return !double.IsNaN(a) && !double.IsNaN(b) &&
            !double.IsInfinity(a) && !double.IsInfinity(b) && Math.Abs(a - b) <= 1e-8;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
#endif

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Isolated, in-memory release regressions. Only the coordinating runner executes these.
/// The diagnostic regressions assert the desired behavior, not the current defects.
/// SaveService.Load and its offline ownership regression belong to the coordinating runner.
/// </summary>
public static class ReleaseD3RegressionCases
{
    private const double Epsilon = 0.000001;

    public static void Run(Action<string, Action> run)
    {
        if (run == null) throw new InvalidOperationException("D3 runner is required.");
        run("D3_DiagnosticOffline12hClearsEvaluationDebt", () =>
            WithFixture(f =>
            {
                D3DiagnosticSystem.Tick(f.State, 12 * 60 * 60, true);
                AssertEvaluationWindow(f.State, "After 12h offline");
            }));
        run("D3_DiagnosticOffline12hEvaluationPartitionInvariant", () =>
        {
            double whole = 0;
            double partitioned = 0;
            WithFixture(f =>
            {
                D3DiagnosticSystem.Tick(f.State, 12 * 60 * 60, true);
                whole = f.State.dimension3.diagnosticSettings.evaluationRemainingSeconds;
            });
            WithFixture(f =>
            {
                for (int i = 0; i < 12 * 60; i++)
                    D3DiagnosticSystem.Tick(f.State, 60, true);
                AssertEvaluationWindow(f.State, "Partitioned 12h offline control");
                partitioned = f.State.dimension3.diagnosticSettings.evaluationRemainingSeconds;
            });
            Require(Math.Abs(whole - partitioned) <= Epsilon,
                "12h evaluation partition mismatch: whole=" + whole +
                ", partitioned=" + partitioned + ". Offline debt must be consumed.");
        });
        run("D3_DiagnosticNewOfflineAnalysisPartitionInvariant", () =>
        {
            AnalysisSnapshot whole = SimulateNewAnalysis(1, 12);
            AnalysisSnapshot partitioned = SimulateNewAnalysis(48, 0.25);
            Require(partitioned.Completed && !partitioned.Running,
                "Partitioned positive control did not finish the real node analysis.");
            Require(whole.NodeId == partitioned.NodeId,
                "Fixtures selected different real nodes.");
            Require(whole.Completed == partitioned.Completed &&
                whole.Running == partitioned.Running &&
                Math.Abs(whole.Remaining - partitioned.Remaining) <= Epsilon &&
                whole.Executions == partitioned.Executions,
                "New offline analysis must consume time after its scheduled start. " +
                "Whole: completed=" + whole.Completed + ", remaining=" + whole.Remaining +
                "; partitioned: completed=" + partitioned.Completed +
                ", remaining=" + partitioned.Remaining + ". Node=" + whole.NodeId);
        });
        run("D3_AutonomyIntegrationGuardsAndRejectsRepeat", () =>
            WithFixture(f =>
            {
                var state = f.State.dimension3;
                var core = D3FacilitySystem.GetFacility(state,
                    Dimension3Catalog.FacilityAutomationCore);
                var coordinator = new D3AssignmentState
                {
                    installationId = Dimension3Catalog.FacilityAutomationCore,
                    channelId = Dimension3Catalog.ChannelCoreCoordination,
                    mk = D3AutonomyCoreSystem.RequiredMk,
                    traitId = Dimension3Catalog.TraitCoordinator,
                    amount = 1,
                    stabilizedAmount = 1
                };
                state.assignments.Add(coordinator);
                D3AutonomyCoreSystem.RecordSuccessfulAutomationExecution(state);
                f.State.dimension03Unlocked = false;
                AssertIntegrationRejected(f.State, "locked D3");
                f.State.dimension03Unlocked = true;
                core.level = 4;
                AssertIntegrationRejected(f.State, "core below N5");
                core.level = 5;
                coordinator.stabilizedAmount = 0;
                AssertIntegrationRejected(f.State, "unstabilized coordinator");
                coordinator.stabilizedAmount = 1;
                state.successfulAutomationExecutions = 0;
                AssertIntegrationRejected(f.State, "no successful automation");
                D3AutonomyCoreSystem.RecordSuccessfulAutomationExecution(state);
                Require(D3AutonomyCoreSystem.TryIntegrate(f.State, out string reason) &&
                    state.autonomyCoreIntegrated, "Valid integration rejected: " + reason);
                long executions = state.successfulAutomationExecutions;
                Require(!D3AutonomyCoreSystem.TryIntegrate(f.State, out reason) &&
                    !string.IsNullOrWhiteSpace(reason) && state.autonomyCoreIntegrated &&
                    state.successfulAutomationExecutions == executions,
                    "Repeated integration must be rejected without changing its proof.");
            }));
    }

    private static AnalysisSnapshot SimulateNewAnalysis(int steps, double seconds)
    {
        AnalysisSnapshot result = null;
        WithFixture(f =>
        {
            f.State.triangleEnergy = MachineManager.NodeAnalysisEnergyCost;
            // Keep one genuinely analyzable catalog node pending, so later automatic
            // starts cannot obscure whether this newly started analysis received time.
            List<MachineNodeDef> nodes = f.Machine.GetAllNodes(true);
            nodes.Sort((a, b) => string.CompareOrdinal(a.id, b.id));
            var progress = new SaveData
            {
                machineUnlocked = true,
                machineIntroSeen = true,
                machineRepairedNodeIds = new List<string>(),
                machineAnalyzedNodeIds = new List<string>()
            };
            foreach (MachineNodeDef node in nodes)
                if (node.effectType == MachineNodeEffectType.UnlockDiagnostics)
                    progress.machineRepairedNodeIds.Add(node.id);
            Require(progress.machineRepairedNodeIds.Count > 0,
                "Catalog has no real diagnostic unlock node.");
            f.Machine.LoadProgressFromSave(progress);
            string target = null;
            foreach (MachineNodeDef node in nodes)
                if (f.Machine.CanAnalyzeNode(node.id, true, out _))
                { target = node.id; break; }
            Require(target != null, "No real automatable damaged node is available.");
            foreach (MachineNodeDef node in nodes)
                if (node.damaged && node.id != target)
                    progress.machineAnalyzedNodeIds.Add(node.id);
            f.Machine.LoadProgressFromSave(progress);
            Require(f.Machine.CanAnalyzeNode(target, true, out string reason) &&
                !f.Machine.IsAnalyzingNode && !f.Machine.IsNodeAnalyzed(target),
                "New analysis fixture invalid: " + reason);
            f.State.dimension3.diagnosticSettings.autoAnalyzeEnabled = true;
            for (int i = 0; i < steps; i++)
                D3DiagnosticSystem.Tick(f.State, seconds, true);
            result = new AnalysisSnapshot
            {
                NodeId = target,
                Completed = f.Machine.IsNodeAnalyzed(target),
                Running = f.Machine.IsAnalyzingNode,
                Remaining = f.Machine.AnalysisRemainingSeconds,
                Executions = f.State.dimension3.successfulAutomationExecutions
            };
        });
        return result;
    }

    private static void AssertIntegrationRejected(GameState state, string guard)
    {
        Require(!D3AutonomyCoreSystem.TryIntegrate(state, out string reason) &&
            !string.IsNullOrWhiteSpace(reason) && !state.dimension3.autonomyCoreIntegrated,
            "Integration guard failed: " + guard);
    }

    private static void AssertEvaluationWindow(GameState state, string context)
    {
        double remaining = state.dimension3.diagnosticSettings.evaluationRemainingSeconds;
        double interval = D3DiagnosticSystem.GetEvaluationInterval(state.dimension3);
        Require(!double.IsNaN(remaining) && !double.IsInfinity(remaining) &&
            remaining > 0 && remaining <= interval + Epsilon,
            context + ": evaluation debt remains (remaining=" + remaining +
            ", interval=" + interval + ").");
    }

    private static void WithFixture(Action<Fixture> body)
    {
        Require(!EditorApplication.isPlayingOrWillChangePlaymode,
            "D3 regression fixtures require Edit Mode.");
        FieldInfo saveField = SingletonField(typeof(SaveService));
        FieldInfo stateField = SingletonField(typeof(GameState));
        FieldInfo machineField = SingletonField(typeof(MachineManager));
        object previousSave = saveField.GetValue(null);
        object previousState = stateField.GetValue(null);
        object previousMachine = machineField.GetValue(null);
        GameObject root = null;
        try
        {
            // Neither Awake nor Update may run. SaveService stays null for setup,
            // every tested action and teardown, including exceptional exits.
            saveField.SetValue(null, null);
            root = new GameObject("D3 Release Regression Fixture")
                { hideFlags = HideFlags.HideAndDontSave };
            root.SetActive(false);
            var f = new Fixture
            {
                State = root.AddComponent<GameState>(),
                Machine = root.AddComponent<MachineManager>()
            };
            stateField.SetValue(null, f.State);
            machineField.SetValue(null, f.Machine);
            f.State.dimension03Unlocked = true;
            f.State.dimension3 = Dimension3System.CreateInitialState();
            Dimension3System.EnsureState(f.State);
            ActivateFacility(f.State, Dimension3Catalog.FacilityDiagnosticBank);
            ActivateFacility(f.State, Dimension3Catalog.FacilityAutomationCore);
            var settings = f.State.dimension3.diagnosticSettings;
            settings.autoAnalyzeEnabled = false;
            settings.autoRepairEnabled = false;
            settings.autoFusionEnabled = false;
            settings.evaluationRemainingSeconds =
                D3DiagnosticSystem.GetEvaluationInterval(f.State.dimension3);
            Require(D3DiagnosticSystem.CanRunOffline(f.State), "N5 offline fixture invalid.");
            Require(SaveService.I == null, "Save singleton must stay null.");
            body(f);
            Require(SaveService.I == null, "A regression changed the save singleton.");
        }
        finally
        {
            try
            {
                saveField.SetValue(null, null);
                if (root != null) UnityEngine.Object.DestroyImmediate(root);
            }
            finally
            {
                stateField.SetValue(null, previousState);
                machineField.SetValue(null, previousMachine);
                saveField.SetValue(null, previousSave);
            }
        }
    }

    private static void ActivateFacility(GameState state, string facilityId)
    {
        D3FacilityState facility = D3FacilitySystem.GetFacility(state.dimension3, facilityId);
        Require(facility != null, "Missing real D3 facility: " + facilityId);
        facility.built = true;
        facility.level = 5;
        // Same stabilized MK6 capacity fixture as Dimension3Block7CValidation.
        state.dimension3.assignments.Add(new D3AssignmentState
        {
            installationId = facilityId,
            channelId = D3FacilitySystem.GetCapacityChannel(facilityId),
            mk = 6,
            traitId = Dimension3Catalog.TraitNormal,
            amount = 100000,
            stabilizedAmount = 100000
        });
    }

    private static FieldInfo SingletonField(Type type)
    {
        FieldInfo field = type.GetField("<I>k__BackingField",
            BindingFlags.Static | BindingFlags.NonPublic);
        Require(field != null, "Missing singleton backing field: " + type.Name);
        return field;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private sealed class Fixture
    {
        public GameState State;
        public MachineManager Machine;
    }

    private sealed class AnalysisSnapshot
    {
        public string NodeId;
        public bool Completed;
        public bool Running;
        public double Remaining;
        public long Executions;
    }
}
#endif

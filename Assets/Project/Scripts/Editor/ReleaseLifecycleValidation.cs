#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

// Synthetic Edit Mode checkpoints, not natural progression or pause/resume coverage.
// The coordinating runner owns execution and expected-error collection. No scene is opened.
public static class ReleaseLifecycleValidation
{
    public static void Run(Action<string, Action> run)
    {
        if (run == null) throw new ArgumentNullException(nameof(run));
        // SAVE_REJECT_ deliberately matches the runner's narrow rejected-load log filter.
        run("SAVE_REJECT_LIFECYCLE_CORRUPT_BLOCK_AND_RETRY", () => RejectedLoad(false));
        run("SAVE_REJECT_LIFECYCLE_FUTURE_BLOCK_AND_RETRY", () => RejectedLoad(true));
        run("LIFECYCLE_D1_D3_IN_FLIGHT_ROUNDTRIP_AND_COMPLETE_ONCE", ProcessRoundTrip);
        run("LIFECYCLE_QUIT_PERSISTS_LATEST_STATE_REPEATEDLY", QuitPersists);
        run("LIFECYCLE_FILESYSTEM_TEMP_OBSTRUCTION_AND_RETRY", TempObstruction);
        foreach (SaveFailureInjectionPoint point in new[] {
            SaveFailureInjectionPoint.BeforeTempWrite, SaveFailureInjectionPoint.AfterTempWrite,
            SaveFailureInjectionPoint.AfterTempValidation, SaveFailureInjectionPoint.BeforeReplace })
        {
            SaveFailureInjectionPoint captured = point;
            run("LIFECYCLE_ATOMIC_" + point, () => AtomicFailure(captured));
        }
    }

    private static void RejectedLoad(bool future)
    {
        using (var f = new Fixture())
        {
            string node = PrepareProcesses(f);
            string valid = f.Checkpoint("valid_retry.json");
            string rejected = future ? JsonUtility.ToJson(new SaveData {
                saveSchemaVersion = SaveService.CurrentSaveSchemaVersion + 1,
                LE = 123, lastUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            }) : "{truncated-lifecycle-save";
            // Poison every actual recovery candidate in this fixture only. A valid
            // .bak would test recovery rather than the HasLoadFailure branch.
            foreach (string path in f.Candidates())
                if (File.Exists(path) || path == f.Path) File.WriteAllText(path, rejected);
            f.Save.Load();
            Require(f.Save.HasLoadFailure, "Invalid input did not set HasLoadFailure.");
            string before = JsonUtility.ToJson(f.State);
            double analysis = f.Machine.AnalysisRemainingSeconds;
            for (int i = 0; i < 3; i++)
            {
                f.State.Tick(60.0);
                Invoke(f.Machine, "AdvanceOnlineFrame", 0.25f);
                f.Save.Save();
                Invoke(f.Save, "OnApplicationQuit");
                Require(!f.Save.TrySave(out string error) && !string.IsNullOrWhiteSpace(error),
                    "TrySave must reject a failed load with a reason.");
            }
            Require(before == JsonUtility.ToJson(f.State) &&
                analysis == f.Machine.AnalysisRemainingSeconds && !f.Machine.IsNodeAnalyzed(node),
                "Tick or the online machine frame advanced while load was rejected.");
            foreach (string path in f.Candidates())
                if (File.Exists(path)) Require(File.ReadAllText(path) == rejected,
                    "Rejected save candidate was overwritten: " + path);
            File.Copy(valid, f.Path, true);
            f.Save.Load();
            Require(!f.Save.HasLoadFailure, "Valid retry did not clear load failure.");
            double scan = f.State.dimension1ScanRemainingSeconds;
            double d2 = Zone(f).analysisRemainingSeconds;
            double d3 = Queue(f).jobs[0].remainingSeconds;
            analysis = f.Machine.AnalysisRemainingSeconds;
            f.State.Tick(0.25);
            Invoke(f.Machine, "AdvanceOnlineFrame", 0.25f);
            Require(f.State.dimension1ScanRemainingSeconds < scan &&
                Zone(f).analysisRemainingSeconds < d2 && Queue(f).jobs[0].remainingSeconds < d3 &&
                f.Machine.AnalysisRemainingSeconds < analysis,
                "Positive control: D1, D2, D3 and machine must resume after a valid retry.");
            Require(f.Save.TrySave(out string retryError), retryError);
        }
    }

    private static void ProcessRoundTrip()
    {
        using (var f = new Fixture())
        {
            string node = PrepareProcesses(f);
            f.State.Tick(0.25);
            Invoke(f.Machine, "AdvanceOnlineFrame", 0.25f);
            string expected = ProcessSnapshot(f);
            string checkpoint = f.Checkpoint("in_flight.json");
            // Destroy in-memory progress to ensure Load reconstructs it from disk.
            f.State.dimension1ScanActive = false;
            f.State.dimension1ScanRemainingSeconds = 0;
            f.State.dimension2 = Dimension2System.CreateInitialState();
            f.State.dimension3 = Dimension3System.CreateInitialState();
            f.Machine.ResetOperationalProgress();
            f.Reload(checkpoint);
            Require(expected == ProcessSnapshot(f), "In-flight roundtrip lost costs, jobs or remaining time.");

            long analyses = Zone(f).totalAnalysesCompleted;
            long parts = Parts(f);
            double knowledge = f.State.dimension2.civilization3.ancientKnowledge +
                D2Civilization3System.GetAncientKnowledgeReward(f.State.dimension2.civilization3,
                    Zone(f).analysisQualityId, Zone(f));
            double duration = Math.Max(f.State.dimension1ScanRemainingSeconds,
                Math.Max(Zone(f).analysisRemainingSeconds, Queue(f).jobs[0].remainingSeconds));
            Dimension1System.Tick(f.State, duration + 1);
            D2Civilization3System.Tick(f.State, duration + 1);
            Dimension3System.Tick(f.State, duration + 1);
            f.Machine.AdvanceAnalysis(f.Machine.AnalysisRemainingSeconds + 1);
            Require(!f.State.dimension1ScanActive && f.State.dimension1ScannedDestinations.Count > 0 &&
                !Zone(f).analysisActive && Zone(f).totalAnalysesCompleted == analyses + 1 &&
                Math.Abs(f.State.dimension2.civilization3.ancientKnowledge - knowledge) < 1e-8 &&
                Queue(f).jobs.Count == 0 && Parts(f) == parts + 1 && f.Machine.IsNodeAnalyzed(node),
                "Completion: scanActive=" + f.State.dimension1ScanActive +
                ", destinations=" + f.State.dimension1ScannedDestinations.Count +
                ", D2 active=" + Zone(f).analysisActive + ", analyses=" + Zone(f).totalAnalysesCompleted +
                "/" + (analyses + 1) + ", knowledge=" + f.State.dimension2.civilization3.ancientKnowledge +
                "/" + knowledge + ", D3 jobs=" + Queue(f).jobs.Count + ", parts=" + Parts(f) +
                "/" + (parts + 1) + ", machineAnalyzed=" + f.Machine.IsNodeAnalyzed(node));
            string destinations = JsonUtility.ToJson(new SaveData {
                dimension1ScannedDestinations = f.State.dimension1ScannedDestinations });
            string completed = ProcessSnapshot(f);
            checkpoint = f.Checkpoint("completed.json");
            for (int i = 0; i < 3; i++)
            {
                f.Reload(checkpoint);
                Dimension1System.Tick(f.State, duration + 1);
                D2Civilization3System.Tick(f.State, duration + 1);
                Dimension3System.Tick(f.State, duration + 1);
                f.Machine.AdvanceAnalysis(duration + 1);
                f.Machine.MarkNodeAnalyzed(node); // Existing public, idempotent completion API.
                Require(completed == ProcessSnapshot(f), "Repeated completion/load changed jobs, costs or rewards.");
                Require(destinations == JsonUtility.ToJson(new SaveData {
                    dimension1ScannedDestinations = f.State.dimension1ScannedDestinations }),
                    "Completed D1 scan rerolled its destinations.");
                checkpoint = f.Checkpoint("completed_repeat_" + i + ".json");
            }
        }
    }

    private static string PrepareProcesses(Fixture f)
    {
        var state = f.State;
        state.LE = 100000; // Synthetic budget, matching existing D3 production validation.
        state.Traces = 1000;
        state.triangleEnergy = MachineManager.NodeAnalysisEnergyCost;
        state.dimension01Unlocked = state.dimension02Unlocked = state.dimension03Unlocked = true;
        state.dimension1Ships.Single(ship => ship.shipId == Dimension1System.ShipLightProbe).unlocked = true;
        state.dimension1Planets.Single(planet => planet.planetId == Dimension1System.Planet01).unlocked = true;
        state.dimension1SelectedSectorId = Dimension1System.Sector01OuterRim;
        Require(Dimension1System.TryScanSimpleDestination(state), "Real D1 scan failed to start.");
        state.dimension2.civilization3Unlocked = true;
        var zone = Zone(f);
        zone.unlocked = zone.scholarHired = true;
        zone.scholarLevel = 1;
        zone.lowQualityRemains = 1;
        Require(D2Civilization3System.TryStartAnalysis(state, D2Civilization3System.Zone1Id,
            D2Civilization3System.LowQualityId), "Real D2 analysis failed to start.");
        Require(Dimension3System.TryQueuePartProduction(state, Dimension3Catalog.PartChassis,
            1, 1L, out string reason), "Real D3 production failed: " + reason);
        var progress = new SaveData { machineUnlocked = true, machineIntroSeen = true };
        var nodes = f.Machine.GetAllNodes(true);
        foreach (var def in nodes)
            if (def.effectType == MachineNodeEffectType.UnlockDiagnostics)
                progress.machineRepairedNodeIds.Add(def.id);
        Require(progress.machineRepairedNodeIds.Count > 0, "Catalog lacks diagnostics unlock.");
        f.Machine.LoadProgressFromSave(progress);
        var node = nodes.OrderBy(n => n.id, StringComparer.Ordinal)
            .FirstOrDefault(n => f.Machine.CanAnalyzeNode(n.id, false, out _));
        Require(node != null, "Catalog has no analyzable node for fixture.");
        double energyBeforeAnalysis = state.triangleEnergy;
        Require(f.Machine.TryStartNodeAnalysis(node.id, MachineManager.BaseNodeAnalysisDurationSeconds,
            false, out reason), "Real machine analysis failed: " + reason);
        Require(Math.Abs(state.triangleEnergy -
                    (energyBeforeAnalysis - MachineManager.NodeAnalysisEnergyCost)) < 0.000001,
            "Real machine analysis did not spend exactly 25 Energy.");
        return node.id;
    }

    private static void QuitPersists()
    {
        using (var f = new Fixture())
        {
            PrepareProcesses(f);
            f.State.Tick(0.25);
            string expected = ProcessSnapshot(f);
            for (int i = 0; i < 2; i++)
            {
                Invoke(f.Save, "OnApplicationQuit");
                Require(SaveService.TryReadSaveData(f.Path, out SaveData data), "Quit did not write a readable save.");
                Require(data.LE == f.State.LE && data.Traces == f.State.Traces &&
                    data.dimension1ScanRemainingSeconds == f.State.dimension1ScanRemainingSeconds,
                    "Quit did not commit the latest state.");
                string copy = f.CopyWithoutOffline("quit_" + i + ".json");
                f.Reload(copy);
                Require(ProcessSnapshot(f) == expected, "Quit/load lost or duplicated in-flight progress.");
            }
        }
    }

    private static void TempObstruction()
    {
        using (var f = new Fixture())
        {
            f.State.LE = 100;
            Require(f.Save.TrySave(out string error), error);
            string original = File.ReadAllText(f.Path);
            Directory.CreateDirectory(f.Path + ".tmp");
            f.State.LE = 200;
            Require(!f.Save.TrySave(out error) && !string.IsNullOrWhiteSpace(error),
                "A directory at .tmp must reject the write.");
            Require(File.ReadAllText(f.Path) == original && f.State.LE == 200,
                "Failed write altered the committed file or rolled back live progress.");
            Directory.Delete(f.Path + ".tmp"); // Exact empty directory owned by this fixture.
            Require(f.Save.TrySave(out error), error);
            Require(SaveService.TryReadSaveData(f.Path, out SaveData data) && data.LE == 200 &&
                File.ReadAllText(f.Path + ".bak") == original, "Retry lost latest state or previous backup.");
        }
    }

    private static void AtomicFailure(SaveFailureInjectionPoint point)
    {
        using (var f = new Fixture())
        {
            f.State.LE = 100;
            Require(f.Save.TrySave(out string error), error);
            f.State.LE = 200;
            Require(f.Save.TrySave(out error), error);
            string primary = File.ReadAllText(f.Path), backup = File.ReadAllText(f.Path + ".bak");
            f.State.LE = 300;
            SaveService.FailureInjectionPoint = point; // Existing hook; no runtime changes.
            Require(!f.Save.TrySave(out error) && !string.IsNullOrWhiteSpace(error), "Injected write unexpectedly succeeded.");
            Require(File.ReadAllText(f.Path) == primary && File.ReadAllText(f.Path + ".bak") == backup &&
                !File.Exists(f.Path + ".tmp"), "Atomic failure changed committed files or left a temporary file.");
            Require(SaveService.TryFindRecoverableSave(f.Path, out SaveData recovered, out string source, out error) &&
                recovered.LE == 200 && source == f.Path, "Last committed state was not recoverable.");
            Require(SaveService.FailureInjectionPoint == SaveFailureInjectionPoint.None, "Injection did not reset.");
            Invoke(f.Save, "OnApplicationQuit");
            Require(SaveService.TryReadSaveData(f.Path, out recovered) && recovered.LE == 300 &&
                File.ReadAllText(f.Path + ".bak") == primary, "Quit after failure did not commit latest live state.");
        }
    }

    private static D2C3ZoneState Zone(Fixture f) => D2Civilization3System.GetZone(
        f.State.dimension2.civilization3, D2Civilization3System.Zone1Id);
    private static D3QueueState Queue(Fixture f) => D3JobQueueSystem.GetQueue(
        f.State.dimension3, Dimension3Catalog.QueuePartProduction);
    private static long Parts(Fixture f) => D3InventorySystem.GetPartAmount(
        f.State.dimension3, Dimension3Catalog.PartChassis, 1);

    private static string ProcessSnapshot(Fixture f)
    {
        // Compare persisted process/economy fields, not UI/offline-report bookkeeping.
        var data = new SaveData {
            LE = f.State.LE, Traces = f.State.Traces,
            dimension1ScanActive = f.State.dimension1ScanActive,
            dimension1ScanRemainingSeconds = f.State.dimension1ScanRemainingSeconds,
            dimension1ScanTotalSeconds = f.State.dimension1ScanTotalSeconds,
            dimension1ActiveScanSectorId = f.State.dimension1ActiveScanSectorId
        };
        f.Machine.WriteProgressToSave(data);
        return JsonUtility.ToJson(data) + "\n" + JsonUtility.ToJson(Zone(f)) + "\n" +
            JsonUtility.ToJson(Queue(f)) + "\n" + Parts(f) + "\n" +
            f.State.dimension2.civilization3.ancientKnowledge.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
    }

    private static void Invoke(object instance, string name, params object[] args)
    {
        var method = instance.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
        Require(method != null, "Missing actual lifecycle method: " + name);
        method.Invoke(instance, args);
    }

    private sealed class Fixture : IDisposable
    {
        public GameState State;
        public MachineManager Machine;
        public SaveService Save;
        public string Path => Save.CurrentSavePath;
        private GameObject root;
        private readonly string previousDirectory = SaveService.EditorValidationSaveDirectory;
        private readonly List<string> research = SaveService.LastLoadedResearchIds;
        private readonly List<string> achievements = SaveService.LastLoadedAchievementIds;
        private readonly List<SavedBuildingLevel> buildings = SaveService.LastLoadedBuildingLevels;
        private readonly UnityEngine.Random.State random = UnityEngine.Random.state;
        private readonly object report = typeof(PresentationReturnReportService).GetProperty("PendingReport").GetValue(null);
        private readonly object prepared = typeof(PresentationReturnReportService).GetProperty("UnifiedReportPreparedThisLoad").GetValue(null);

        public Fixture()
        {
            Require(!EditorApplication.isPlayingOrWillChangePlaymode, "Lifecycle fixtures require Edit Mode.");
            Require(GameState.I == null && MachineManager.I == null && SaveService.I == null &&
                ResearchManager.I == null && AchievementManager.I == null && F2UpgradeManager.I == null &&
                BuildingDatabase.I == null && Object.FindFirstObjectByType<TabsUI>(FindObjectsInactive.Include) == null,
                "Refusing to run lifecycle fixtures alongside live state/managers/UI.");
            Require(!SaveService.SuppressWritesForVisualQa &&
                SaveService.FailureInjectionPoint == SaveFailureInjectionPoint.None, "Persistence instrumentation already active.");
            Require(!QaRuntimeService.IsAccelerated, "Lifecycle fixtures require the normal online clock.");
            try
            {
                string directory = System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath,
                    "../Logs/ReleaseD1D3/Lifecycle", Guid.NewGuid().ToString("N")));
                Directory.CreateDirectory(directory);
                SaveService.EditorValidationSaveDirectory = directory;
                root = new GameObject("Release lifecycle fixture") { hideFlags = HideFlags.HideAndDontSave };
                root.SetActive(false);
                State = root.AddComponent<GameState>();
                Machine = root.AddComponent<MachineManager>();
                Save = root.AddComponent<SaveService>();
                Singleton(typeof(GameState), State);
                Singleton(typeof(MachineManager), Machine);
                Singleton(typeof(SaveService), Save);
                Require(System.IO.Path.GetFullPath(Path) == System.IO.Path.Combine(directory, "save.json"),
                    "Save path escaped fixture directory.");
                State.EnsureDimension1State();
                State.EnsureDimension2State();
                State.EnsureDimension3State();
                State.EnsureConvergenceState();
            }
            catch { Dispose(); throw; }
        }

        public IEnumerable<string> Candidates()
        {
            yield return Path;
            yield return Path + ".bak";
            for (int i = 1; i <= SaveService.HistoricalBackupCount; i++)
                yield return SaveService.GetHistoricalSavePath(Path, i);
        }

        public string Checkpoint(string name)
        {
            Require(Save.TrySave(out string error), error);
            return CopyWithoutOffline(name);
        }

        public string CopyWithoutOffline(string name)
        {
            Require(SaveService.TryReadSaveData(Path, out SaveData data), "Checkpoint unreadable.");
            // Synthetic timestamp only on a copy: eliminate wall-clock second boundaries.
            // Offline credit is covered by the coordinator's dedicated tests.
            data.lastUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 3600;
            string copy = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), name);
            File.WriteAllText(copy, JsonUtility.ToJson(data, true));
            return copy;
        }

        public void Reload(string copy)
        {
            File.Copy(copy, Path, true);
            Save.Load();
            Require(!Save.HasLoadFailure, "Valid checkpoint was rejected.");
        }

        public void Dispose()
        {
            try
            {
                Singleton(typeof(SaveService), null);
                Singleton(typeof(MachineManager), null);
                Singleton(typeof(GameState), null);
                if (root != null) Object.DestroyImmediate(root);
            }
            finally
            {
                SaveService.EditorValidationSaveDirectory = previousDirectory;
                SaveService.LastLoadedResearchIds = research;
                SaveService.LastLoadedAchievementIds = achievements;
                SaveService.LastLoadedBuildingLevels = buildings;
                SaveService.FailureInjectionPoint = SaveFailureInjectionPoint.None;
                UnityEngine.Random.state = random;
                typeof(PresentationReturnReportService).GetProperty("PendingReport").SetValue(null, report);
                typeof(PresentationReturnReportService).GetProperty("UnifiedReportPreparedThisLoad").SetValue(null, prepared);
                // Retain only our GUID directory as reviewable test evidence.
            }
        }
    }

    private static void Singleton(Type type, object value) =>
        type.GetProperty("I", BindingFlags.Public | BindingFlags.Static).SetValue(null, value);
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
#endif

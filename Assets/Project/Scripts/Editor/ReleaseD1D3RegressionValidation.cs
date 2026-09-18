#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

// Synthetic technical fixtures, not evidence of natural progression or pacing.
// Run only in batch Edit Mode; never opens or saves Main.unity.
public static class ReleaseD1D3RegressionValidation
{
    [Serializable] public class CaseResult
    {
        public string id;
        public bool passed;
        public string detail;
    }
    [Serializable] public class Report
    {
        public string utc;
        public string unityVersion;
        public string scope = "D1-D3 and saves; Convergence excluded; synthetic Edit Mode fixtures";
        public List<CaseResult> cases = new List<CaseResult>();
    }

    private static Report report;
    private static string runDirectory;

    public static void ValidateBatch()
    {
        int exitCode = 1;
        try
        {
            Require(Application.isBatchMode && !EditorApplication.isPlayingOrWillChangePlaymode,
                "Run in a dedicated batch Editor outside Play Mode.");
            Require(GameState.I == null && SaveService.I == null && MachineManager.I == null,
                "Refusing to replace live singletons.");
            Require(!SaveService.SuppressWritesForVisualQa,
                "Writes suppressed: persistence tests would be meaningless.");
            runDirectory = Path.GetFullPath(Path.Combine(Application.dataPath,
                "../Logs/ReleaseD1D3", DateTime.UtcNow.ToString("yyyyMMdd_HHmmss") +
                "_" + Guid.NewGuid().ToString("N").Substring(0, 8)));
            Directory.CreateDirectory(runDirectory);
            report = new Report { utc = DateTime.UtcNow.ToString("O"), unityVersion = Application.unityVersion };

            RunCase("D1_ARK_REQUIRED_BUDGET", ValidateArkBudget);
            RunCase("D1_EXISTING_CREDIT_AND_JSON", D1TreePointsValidation.ValidateD1TreePoints);
            ReleaseD1BudgetValidation.Run(RunCase);
            RunCase("SAVE_EXISTING_ATOMIC_AND_FALLBACK", SaveRecoveryValidation.ValidateOrThrow);
            foreach (bool future in new[] { false, true })
                foreach (bool quit in new[] { false, true })
                {
                    bool savedFuture = future, savedQuit = quit;
                    RunCase("SAVE_REJECT_" + (future ? "FUTURE" : "CORRUPT") +
                        (quit ? "_QUIT" : "_AUTOSAVE"), () => ValidateRejectedLoad(savedFuture, savedQuit));
                }
            RunCase("SAVE_REAL_LOAD_FALLBACK", ValidateFallbackLoad);
            RunCase("SAVE_REJECT_RETRY_VALID_LOAD", ValidateLoadRetry);
            RunCase("SAVE_REJECT_HISTORY_FAILURE", ValidateHistoryFailure);
            RunCase("SAVE_NEW_GAME_PERSISTS", ValidateNewGame);
            RunCase("SAVE_D1_LEGACY_BUDGET_CREDIT_ONCE", () => ValidateBudgetMigration(false));
            RunCase("SAVE_D1_CURRENT_BUDGET_CREDIT_ONCE", () => ValidateBudgetMigration(true));
            RunCase("SAVE_D1_D3_ROUNDTRIP", ValidateDimensionRoundTrip);
            RunCase("SAVE_LEGACY_COPY_PRE_RECONSTRUCTION", () => ValidateHistoricalCopy(
                "Recovery/PreReconstruction_20260808/save.json", "legacy_pre_reconstruction"));
            RunCase("SAVE_LEGACY_COPY_DEVICE", () => ValidateHistoricalCopy(
                "Recovery/Device_57974d2d_20260808_1605/save.json", "legacy_device"));
            RunCase("SAVE_RECENT_COPY_D1_AUDIT", () => ValidateHistoricalCopy(
                "Logs/D1_Audit_2026-09-08/save_backup/save.json", "recent_d1_audit"));
            RunCase("D3_LOADED_ANALYSIS_SINGLE_OFFLINE_CREDIT", () => ValidateSingleOfflineAnalysis(true, true));
            RunCase("D3_LOADED_MANUAL_ANALYSIS_WITHOUT_AUTOMATION", () => ValidateSingleOfflineAnalysis(true, false));
            RunCase("D3_LOADED_MANUAL_ANALYSIS_BEFORE_D3", () => ValidateSingleOfflineAnalysis(false, false));
            ReleaseD2RegressionCases.Run(RunCase);
            ReleaseD2TimingValidation.Run(RunCase);
            ReleaseD3RegressionCases.Run(RunCase);
            ReleaseLifecycleValidation.Run(RunCase);
            ReleaseOfflineIntegrationValidation.Run(RunCase);
            RunCase("ANDROID_EXISTING_PAUSE_FRAME_GUARDS", AndroidPauseResumeValidation.Validate);
            RunCase("D3_EXISTING_FULL_THROUGH_7D", Dimension3Block7FullValidation.ValidateFullThrough7D);
            exitCode = report.cases.All(c => c.passed) ? 0 : 1;
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            if (report != null)
                report.cases.Add(new CaseResult { id = "HARNESS", detail = exception.ToString() });
        }
        finally
        {
            SaveService.EditorValidationSaveDirectory = null;
            if (report != null)
            {
                string path = Path.Combine(runDirectory, "results.json");
                File.WriteAllText(path, JsonUtility.ToJson(report, true));
                int passed = report.cases.Count(c => c.passed);
                Debug.Log("[Release D1-D3] " + (exitCode == 0 ? "PASS" : "FAIL") +
                    " | " + passed + "/" + report.cases.Count + " | " + path);
            }
            EditorApplication.Exit(exitCode);
        }
    }

    private static void RunCase(string id, Action test)
    {
        var errors = new List<string>();
        Application.LogCallback callback = (message, stack, type) =>
        {
            if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert) return;
            // These tests intentionally feed rejected saves. All other engine
            // errors, including FAIL emitted by legacy validators, fail the case.
            if (id.StartsWith("SAVE_REJECT_", StringComparison.Ordinal) &&
                message.StartsWith("[SaveService] No se modificara la partida:", StringComparison.Ordinal)) return;
            if (id == "SAVE_REJECT_HISTORY_FAILURE" &&
                message.StartsWith("[SaveService] Carga cancelada: no se pudo respaldar la partida.", StringComparison.Ordinal)) return;
            errors.Add(message);
        };
        var result = new CaseResult { id = id };
        Application.logMessageReceived += callback;
        try
        {
            test();
            Require(errors.Count == 0, "Unity errors: " + string.Join(" | ", errors));
            result.passed = true;
            result.detail = "Assertions passed; no unexpected Unity errors.";
        }
        catch (Exception exception)
        {
            result.detail = exception.GetBaseException().Message;
        }
        finally { Application.logMessageReceived -= callback; }
        report.cases.Add(result);
        Debug.Log("[Release Case] " + (result.passed ? "PASS" : "FAIL") + " | " + id + " | " + result.detail);
    }

    private static void ValidateArkBudget()
    {
        string[] ids = Dimension1System.Dimension1TreeNodeIds;
        int minimum = int.MaxValue;
        int requiredNodes = 0;
        int requiredReadingTier = 0;
        int fullCost = Dimension1System.GetDimension1TreeTotalFullPurchaseCost();
        using (var fixture = new Fixture("ark_budget"))
        {
            var requirements = Dimension1System.GetD1ArkRequirements(fixture.state);
            // Derive the distinct-node constraint from the actual requirement,
            // rather than treating three purchases of Reading as three nodes.
            var nodeRequirement = requirements.Single(r => r.requirementId.StartsWith("tree_nodes_", StringComparison.Ordinal));
            requiredNodes = nodeRequirement.requiredValue;
            requiredReadingTier = requirements.Single(r => r.requirementId.StartsWith("relic_reading_", StringComparison.Ordinal)).requiredValue;
        }
        for (int mask = 0; mask < (1 << ids.Length); mask++)
        {
            int count = 0, cost = 0;
            bool reading = false, coordination = false;
            for (int index = 0; index < ids.Length; index++)
            {
                if ((mask & (1 << index)) == 0) continue;
                count++;
                if (ids[index] == Dimension1System.D1TreeRelicReading)
                {
                    reading = true;
                    for (int tier = 1; tier <= requiredReadingTier; tier++)
                        cost += Dimension1System.GetDimension1TreeNodeCost(ids[index], tier);
                }
                else cost += Dimension1System.GetDimension1TreeNodeCost(ids[index], 1);
                coordination |= ids[index] == Dimension1System.D1TreeFleetCoordination;
            }
            if (count >= requiredNodes && reading && coordination) minimum = Math.Min(minimum, cost);
        }
        Debug.Log("[D1 Budget] minimum=" + minimum + " fullTree=" + fullCost +
            " cap=" + Dimension1System.Dimension1Prestige1PreviewPointCap);
        Require(minimum <= Dimension1System.Dimension1Prestige1PreviewPointCap,
            "Ark minimum=" + minimum + "; lifetime point cap=" +
            Dimension1System.Dimension1Prestige1PreviewPointCap + "; full tree=" + fullCost);
    }

    private static void ValidateRejectedLoad(bool future, bool quit)
    {
        using (var fixture = new Fixture("reject_" + future + "_" + quit))
        {
            string original = future ? JsonUtility.ToJson(new SaveData
            {
                saveSchemaVersion = SaveService.CurrentSaveSchemaVersion + 1,
                LE = 7654321, maxLEAlcanzado = 7654321,
                lastUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            }) : "{truncated-save";
            File.WriteAllText(fixture.save.CurrentSavePath, original);
            fixture.save.Load();
            if (quit)
                typeof(SaveService).GetMethod("OnApplicationQuit", BindingFlags.NonPublic | BindingFlags.Instance)
                    .Invoke(fixture.save, null);
            else fixture.save.Save(); // same entry point used by InvokeRepeating
            Require(File.ReadAllText(fixture.save.CurrentSavePath) == original,
                "Rejected save was overwritten by " + (quit ? "OnApplicationQuit" : "autosave Save()") + ".");
        }
    }

    private static void ValidateFallbackLoad()
    {
        using (var fixture = new Fixture("fallback"))
        {
            File.WriteAllText(fixture.save.CurrentSavePath, "{bad-primary");
            File.WriteAllText(fixture.save.CurrentSavePath + ".bak", JsonUtility.ToJson(new SaveData
            {
                saveSchemaVersion = SaveService.CurrentSaveSchemaVersion,
                LE = 1234, maxLEAlcanzado = 1234,
                lastUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            }));
            fixture.save.Load();
            Require(Math.Abs(fixture.state.LE - 1234) < 0.001, "Fallback did not restore LE.");
            Require(SaveService.TryReadSaveData(fixture.save.CurrentSavePath, out SaveData loaded) &&
                Math.Abs(loaded.LE - 1234) < 0.001, "Recovered state not persisted.");
        }
    }

    private static void ValidateLoadRetry()
    {
        using (var fixture = new Fixture("retry_valid"))
        {
            File.WriteAllText(fixture.save.CurrentSavePath, "{bad");
            fixture.save.Load();
            Require(fixture.save.HasLoadFailure && !fixture.save.TrySave(out _),
                "Rejected load must block direct TrySave as well.");
            File.WriteAllText(fixture.save.CurrentSavePath, JsonUtility.ToJson(new SaveData
            {
                saveSchemaVersion = SaveService.CurrentSaveSchemaVersion,
                LE = 321, maxLEAlcanzado = 321, lastUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            }));
            fixture.save.Load();
            Require(!fixture.save.HasLoadFailure && fixture.state.LE == 321 && fixture.save.TrySave(out _),
                "A successful retry must restore normal saving.");
        }
    }

    private static void ValidateNewGame()
    {
        using (var fixture = new Fixture("new_game"))
        {
            fixture.save.Load();
            Require(!fixture.save.HasLoadFailure && File.Exists(fixture.save.CurrentSavePath) &&
                fixture.state.LE == 10 && fixture.save.TrySave(out _),
                "No existing save must initialize and persist the starter game.");
        }
    }

    private static void ValidateHistoryFailure()
    {
        using (var fixture = new Fixture("history_failure"))
        {
            string original = JsonUtility.ToJson(new SaveData
            {
                saveSchemaVersion = SaveService.CurrentSaveSchemaVersion,
                LE = 987, maxLEAlcanzado = 987, lastUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            });
            File.WriteAllText(fixture.save.CurrentSavePath, original);
            // A directory at the expected file path deterministically prevents the backup.
            Directory.CreateDirectory(SaveService.GetHistoricalSavePath(fixture.save.CurrentSavePath, 1));
            fixture.save.Load();
            fixture.save.Save();
            Require(fixture.save.HasLoadFailure && !fixture.save.TrySave(out _) &&
                File.ReadAllText(fixture.save.CurrentSavePath) == original,
                "Backup failure must leave the original save protected.");
        }
    }

    private static void ValidateBudgetMigration(bool current)
    {
        using (var fixture = new Fixture("budget_migration_" + current))
        {
            fixture.state.LE = 100;
            fixture.state.dimension01Unlocked = true;
            foreach (var planet in fixture.state.dimension1Planets) planet.unlocked = true;
            foreach (var ship in fixture.state.dimension1Ships) ship.unlocked = true;
            foreach (var sector in fixture.state.dimension1Sectors) sector.unlocked = true;
            fixture.state.dimension1ScannerLevel = Dimension1System.SimpleScannerMaxLevel;
            fixture.state.d1TreePoints = 5;
            fixture.state.d1TreePointsProgressBaseline = 12;
            Require(fixture.save.TrySave(out string error), error);
            Require(SaveService.TryReadSaveData(fixture.save.CurrentSavePath, out SaveData data), "Unreadable fixture.");
            data.d1TreePointsSaveVersion = current ? 1 : 0;
            data.prestige1Points = 5;
            data.lastUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            File.WriteAllText(fixture.save.CurrentSavePath, JsonUtility.ToJson(data));
            fixture.save.Load();
            Dimension1System.SyncD1TreePointsFromProgress(fixture.state, out _);
            Require(fixture.state.d1TreePoints == 20 && fixture.state.d1TreePointsProgressBaseline == 27,
                "Migration must add 15 to saved 5, without multiplying the balance.");
            Require(fixture.save.TrySave(out error), error);
            fixture.save.Load();
            Require(!Dimension1System.SyncD1TreePointsFromProgress(fixture.state, out _) &&
                fixture.state.d1TreePoints == 20 && fixture.state.d1TreePointsProgressBaseline == 27,
                "Reload duplicated migration credit.");
        }
    }

    private static void ValidateDimensionRoundTrip()
    {
        using (var fixture = new Fixture("dimension_roundtrip"))
        {
            fixture.state.LE = 100;
            fixture.state.dimension01Unlocked = fixture.state.dimension02Unlocked = fixture.state.dimension03Unlocked = true;
            Require(fixture.save.TrySave(out string beforeError), beforeError);
            File.Copy(fixture.save.CurrentSavePath,
                Path.Combine(Path.GetDirectoryName(fixture.save.CurrentSavePath), "before_hitos.json"));
            fixture.state.dimension1GalacticAnchorDiscovered = true;
            fixture.state.dimension2.civilization2.entityContained = true;
            fixture.state.dimension2.civilization2.majorPactEstablished = true;
            fixture.state.dimension3.autonomyCoreIntegrated = true;
            fixture.state.d1TreePoints = 7;
            fixture.state.d1TreePointsProgressBaseline = 12;
            Require(fixture.save.TrySave(out string error), error);
            File.Copy(fixture.save.CurrentSavePath,
                Path.Combine(Path.GetDirectoryName(fixture.save.CurrentSavePath), "after_hitos.json"));
            fixture.state.dimension1GalacticAnchorDiscovered = false;
            fixture.state.dimension2 = Dimension2System.CreateInitialState();
            fixture.state.dimension3 = Dimension3System.CreateInitialState();
            fixture.state.d1TreePoints = 0;
            fixture.save.Load();
            Require(DimensionCompletionService.AreAllDimensionsCompleted(fixture.state),
                "A valid dimension completion was lost on disk roundtrip.");
            Require(fixture.state.d1TreePoints == 7 && fixture.state.d1TreePointsProgressBaseline == 12,
                "Tree balance or credited-progress baseline changed.");
        }
    }

    private static void ValidateHistoricalCopy(string relativeSource, string fixtureName)
    {
        string source = Path.GetFullPath(Path.Combine(Application.dataPath, "..", relativeSource));
        string original = File.ReadAllText(source);
        SaveData data = JsonUtility.FromJson<SaveData>(original);
        Require(data != null, "Historical source unreadable.");
        // Test migration alone, not months of accumulated absence. Preserve the
        // original unmodified next to the adjusted input so the case is reviewable.
        data.lastUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        using (var fixture = new Fixture(fixtureName))
        {
            string folder = Path.GetDirectoryName(fixture.save.CurrentSavePath);
            File.WriteAllText(Path.Combine(folder, "source_original.json"), original);
            File.WriteAllText(fixture.save.CurrentSavePath, JsonUtility.ToJson(data));
            fixture.save.Load();
            Require(fixture.state.dimension01Unlocked == data.dimension01Unlocked &&
                fixture.state.dimension02Unlocked == data.dimension02Unlocked &&
                fixture.state.dimension03Unlocked == data.dimension03Unlocked,
                "Historical copy lost a discovered dimension.");
            int expectedPoints = data.d1TreePointsSaveVersion >= 1 ? data.d1TreePoints : data.prestige1Points;
            Require(fixture.state.d1TreePoints >= expectedPoints, "Historical tree points were lost.");
            Require(fixture.save.TrySave(out string error), error);
            int once = fixture.state.d1TreePoints;
            fixture.save.Load();
            Require(fixture.state.d1TreePoints == once, "Repeat load duplicated tree migration credit.");
            Require(File.ReadAllText(source) == original, "Historical source changed.");
        }
    }

    private static void ValidateSingleOfflineAnalysis(bool d3Unlocked, bool automation)
    {
        using (var fixture = new Fixture("single_analysis_" + d3Unlocked + "_" + automation))
        {
            fixture.state.LE = 100;
            fixture.state.dimension03Unlocked = d3Unlocked;
            if (automation)
            {
                ActivateDiagnosticFacility(fixture.state, Dimension3Catalog.FacilityDiagnosticBank,
                    Dimension3Catalog.ChannelDiagnosticCapacity);
                ActivateDiagnosticFacility(fixture.state, Dimension3Catalog.FacilityAutomationCore,
                    Dimension3Catalog.ChannelCoreCoordination);
            }
            Require(D3DiagnosticSystem.CanRunOffline(fixture.state) == automation, "Diagnostic offline gate differs from fixture.");
            Require(fixture.save.TrySave(out string error), error);
            Require(SaveService.TryReadSaveData(fixture.save.CurrentSavePath, out SaveData data), "Fixture save unreadable.");
            MachineNodeDef node = fixture.machine.GetAllNodes(false).First(n => n.damaged);
            data.machineAnalysisNodeId = node.id;
            data.machineAnalysisRemainingSeconds = 300;
            data.machineUnlocked = true;
            data.lastUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 60;
            File.WriteAllText(fixture.save.CurrentSavePath, JsonUtility.ToJson(data));
            fixture.save.Load();
            Require(D3DiagnosticSystem.CanRunOffline(fixture.state) == automation &&
                fixture.state.dimension03Unlocked == d3Unlocked, "Load invalidated diagnostic fixture.");
            double remaining = fixture.machine.AnalysisRemainingSeconds;
            Require(remaining >= 235 && remaining <= 240,
                "Expected about 240 seconds after a 60-second absence; actual=" + remaining);
        }
    }

    private static void ActivateDiagnosticFacility(GameState state, string id, string channel)
    {
        var facility = D3FacilitySystem.GetFacility(state.dimension3, id);
        facility.built = true;
        facility.level = 5;
        D3InventorySystem.AddAutomatons(state.dimension3, 6, Dimension3Catalog.TraitNormal, 100000);
        state.dimension3.assignments.Add(new D3AssignmentState
        {
            installationId = id, channelId = channel, mk = 6,
            traitId = Dimension3Catalog.TraitNormal, amount = 100000, stabilizedAmount = 100000
        });
    }

    private sealed class Fixture : IDisposable
    {
        public readonly GameState state;
        public readonly SaveService save;
        public readonly MachineManager machine;
        private readonly List<GameObject> objects = new List<GameObject>();
        public Fixture(string name)
        {
            Require(GameState.I == null && SaveService.I == null && MachineManager.I == null,
                "Fixture requires isolated singletons.");
            string directory = Path.Combine(runDirectory, "fixtures", name);
            Directory.CreateDirectory(directory);
            SaveService.EditorValidationSaveDirectory = directory;
            state = Add<GameState>(name + " state");
            machine = Add<MachineManager>(name + " machine");
            save = Add<SaveService>(name + " save");
            SetSingleton(typeof(GameState), state);
            SetSingleton(typeof(MachineManager), machine);
            SetSingleton(typeof(SaveService), save);
            state.EnsureDimension1State();
            state.EnsureDimension2State();
            state.EnsureDimension3State();
            state.EnsureConvergenceState();
            Require(save.CurrentSavePath.StartsWith(directory + Path.DirectorySeparatorChar,
                StringComparison.OrdinalIgnoreCase), "Save path escaped fixture.");
        }
        private T Add<T>(string name) where T : Component
        {
            var obj = new GameObject(name) { hideFlags = HideFlags.HideAndDontSave };
            obj.SetActive(false);
            objects.Add(obj);
            return obj.AddComponent<T>();
        }
        public void Dispose()
        {
            SetSingleton(typeof(SaveService), null);
            SetSingleton(typeof(MachineManager), null);
            SetSingleton(typeof(GameState), null);
            foreach (var obj in objects) Object.DestroyImmediate(obj);
            SaveService.EditorValidationSaveDirectory = null;
            SaveService.LastLoadedBuildingLevels = null;
            SaveService.LastLoadedResearchIds = null;
            SaveService.LastLoadedAchievementIds = null;
        }
    }

    private static void SetSingleton(Type type, object value) =>
        type.GetProperty("I", BindingFlags.Public | BindingFlags.Static).SetValue(null, value);
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
#endif

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// A bounded action policy is not proof that no alternative route exists.
// Run uses labelled entry fixtures; RunFullJourney performs the three real Prestige 1 resets.
public static class ReleaseJourneyValidation
{
    const string Key = "QF.ReleaseJourney.";
    const BindingFlags Private = BindingFlags.NonPublic | BindingFlags.Instance;
    static string Folder => SessionState.GetString(Key + "Folder", "");
    static int frames, leg = -1;
    static double simulated, realStarted, realInitialLE, realInitialTicks;
    static double totalSimulated;
    static bool stopping, fullJourney;
    static readonly bool[] fullMilestoneRecorded = new bool[4];
    static int fusionRecipeCursor;
    static readonly string[] Names = { "BASE", "D1_ENTRY", "D2_ENTRY", "D3_ENTRY" };
    static string Current => leg < 0 ? "REAL_START" : Names[leg];
    static string SavePath => Path.Combine(Folder, "save.json");
    static MethodInfo d1Step, d2Step, d3Step;
    static int verifyDimension = 1, verifyStage;
    static string CompletedJourney => Directory.GetDirectories(Path.GetFullPath("Logs/ReleaseD1D3"), "Journey_*")
        .Where(path => File.Exists(Path.Combine(path, "D3_ENTRY_END.json")))
        .OrderByDescending(path => path).First();

    [InitializeOnLoadMethod]
    static void Resume()
    {
        if (!SessionState.GetBool(Key + "Active", false)) return;
        SaveService.EditorValidationSaveDirectory = Folder;
        EditorApplication.playModeStateChanged -= Changed;
        EditorApplication.playModeStateChanged += Changed;
        Application.logMessageReceived -= Error;
        Application.logMessageReceived += Error;
        if (EditorApplication.isPlaying) Attach();
    }

    public static void Run() => Start(false, false);
    public static void RunCheckpoints() => Start(true, false);
    public static void RunFullJourney() => Start(false, true);
    static void Start(bool verify, bool full)
    {
        if (!Application.isBatchMode || EditorApplication.isPlayingOrWillChangePlaymode)
            throw new InvalidOperationException("Dedicated batch required.");
        var prefix = verify ? "JourneyVerify_" : full ? "FullJourney_" : "Journey_";
        var folder = Path.GetFullPath("Logs/ReleaseD1D3/" + prefix + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss"));
        Directory.CreateDirectory(folder);
        SessionState.SetString(Key + "Folder", folder);
        SessionState.SetBool(Key + "Active", true);
        SessionState.SetBool(Key + "Failed", false);
        SessionState.SetBool(Key + "Verify", verify);
        SessionState.SetBool(Key + "Full", full);
        SaveService.EditorValidationSaveDirectory = folder;
        File.WriteAllText(Path.Combine(folder, "actions.tsv"), "leg\tsimulated_seconds\taction\n");
        if (verify) File.Copy(Path.Combine(CompletedJourney, "D1_ENTRY_END.json"), Path.Combine(folder, "save.json"));
        Resume();
        EditorSceneManager.OpenScene("Assets/Project/Scenes/Main.unity", OpenSceneMode.Single);
        EditorApplication.isPlaying = true;
    }

    static void Error(string text, string stack, LogType type)
    {
        if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert) return;
        SessionState.SetBool(Key + "Failed", true);
        File.AppendAllText(Path.Combine(Folder, "errors.txt"), text + "\n" + stack + "\n");
    }

    static void Changed(PlayModeStateChange state)
    {
        SaveService.EditorValidationSaveDirectory = Folder;
        if (state == PlayModeStateChange.EnteredPlayMode) Attach();
        if (state != PlayModeStateChange.EnteredEditMode) return;
        EditorApplication.update -= Tick;
        bool failed = SessionState.GetBool(Key + "Failed", false);
        bool fullPass = SessionState.GetBool(Key + "Full", false) && File.Exists(Path.Combine(Folder, "full_result.txt"));
        File.WriteAllText(Path.Combine(Folder, "runner_result.txt"), failed ? "ERROR" :
            fullPass ? "PASS — full clean journey D1-D3" : "EXECUTED — inspect milestones; not all-route PASS");
        Debug.Log("[Release Journey] " + (failed ? "ERROR" : "EXECUTED") + " | " + Folder);
        EditorApplication.Exit(failed ? 1 : 0);
    }

    static void Attach()
    {
        frames = 0; leg = -1; stopping = false;
        fullJourney = SessionState.GetBool(Key + "Full", false);
        totalSimulated = 0; fusionRecipeCursor = 0;
        Array.Clear(fullMilestoneRecorded, 0, fullMilestoneRecorded.Length);
        verifyDimension = 1; verifyStage = 0;
        EditorApplication.update -= Tick;
        EditorApplication.update += Tick;
    }

    static void Log(string action) => File.AppendAllText(Path.Combine(Folder, "actions.tsv"),
        Current + "\t" + simulated.ToString("F1", System.Globalization.CultureInfo.InvariantCulture) + "\t" + action.Replace('\n', ' ') + "\n");

    static void Tick()
    {
        if (stopping) return;
        try
        {
            if (++frames < 30) return;
            Require(GameState.I != null && SaveService.I != null && SaveService.I.CurrentSavePath == SavePath, "Save isolation lost.");
            if (SessionState.GetBool(Key + "Verify", false))
            {
                if (frames % 20 == 0) VerifyCheckpointStep();
                return;
            }
            if (frames == 30)
            {
                typeof(ReleaseRecoveryRuntimeValidation).GetMethod("SetResolution", BindingFlags.Static | BindingFlags.NonPublic)
                    .Invoke(null, new object[] {1080, 1920});
                Checkpoint("NEW_UNTOUCHED");
                d1Step = typeof(ReleaseJourneyValidation).Assembly.GetType("ReleaseD1JourneyPolicy")?.GetMethod("Step");
                var d23 = typeof(ReleaseJourneyValidation).Assembly.GetType("ReleaseD2D3JourneyPolicy");
                d2Step = d23?.GetMethod("StepD2"); d3Step = d23?.GetMethod("StepD3");
                Require(d1Step != null && d2Step != null && d3Step != null, "Journey policies missing.");
                return;
            }
            if (frames == 40)
            {
                var gs = GameState.I;
                Require(gs.GetBuildingLevel("vacuum_observer") == 0 && gs.LE == 10, "Not a clean start.");
                var row = UnityEngine.Object.FindObjectsByType<BuildingRowUI>(FindObjectsSortMode.None)
                    .First(r => ((BuildingState)typeof(BuildingRowUI).GetField("state", Private).GetValue(r))?.def?.id == "vacuum_observer");
                Click(row.buyButton);
                Require(gs.GetBuildingLevel("vacuum_observer") == 1, "First Higgs click failed.");
                realStarted = EditorApplication.timeSinceStartup;
                realInitialLE = gs.LE;
                realInitialTicks = Time.unscaledTimeAsDouble;
                Require(QaRuntimeService.SimulationMultiplier == 1f, "Real-time phase has QA acceleration.");
                Capture("new_first_purchase.png");
                Log("Physical EventSystem first Higgs purchase; waiting 60 wall seconds without acceleration");
            }
            if (frames < 40) return;
            if (leg < 0)
            {
                if (EditorApplication.timeSinceStartup - realStarted < 60) return;
                double wall = EditorApplication.timeSinceStartup - realStarted;
                double gain = GameState.I.LE - realInitialLE;
                Log("REAL_MEASUREMENT wall=" + wall + " UnityUnscaled=" + (Time.unscaledTimeAsDouble-realInitialTicks) + " LE_gain=" + gain);
                Require(gain > 0 && gain <= wall * 1.1 + 2, "Invalid real-time production.");
                Checkpoint("REAL_60_SECONDS");
                TickSystem.I.enabled = false;
                MachineManager.I.enabled = false;
                SaveService.SuppressWritesForVisualQa = true;
                leg = 0; simulated = 0; totalSimulated = 0;
                UnityEngine.Random.InitState(260908);
            }
            var budget = System.Diagnostics.Stopwatch.StartNew();
            do
            {
                var gs = GameState.I;
                BaseActions(gs);
                int activeDimension = fullJourney ? gs.prestige1CurrentDimensionId : leg;
                if (activeDimension > 0)
                {
                    var method = activeDimension == 1 ? d1Step : activeDimension == 2 ? d2Step : d3Step;
                    method.Invoke(null, new object[] {gs, (Action<string>)Log});
                }
                double dt = fullJourney ? 10 : leg == 0 ? 1 : 10;
                gs.Tick(dt);
                MachineManager.I.AdvanceAnalysis(dt);
                var room2 = UnityEngine.Object.FindFirstObjectByType<Room2PanelUI>(FindObjectsInactive.Include);
                if (room2 != null) room2.AdvanceQaFusionCooldown(dt);
                simulated += dt;
                totalSimulated += dt;
                if (simulated % 3600 == 0)
                    File.WriteAllText(Path.Combine(Folder, "progress.txt"), Current + " simulated_hours=" + simulated / 3600 +
                        " LE=" + gs.LE.ToString("F0") + " traces=" + gs.Traces.ToString("F0") +
                        " repair=" + MachineManager.I.GetTotalMachineRepairProgress01().ToString("P1") +
                        " fusion=" + gs.experimentalHallazgos + "," + gs.experimentalMuestras + "," +
                        gs.experimentalLecturasIncompletas + "," + gs.experimentalCompuestosUtiles +
                        " prestige1=" + gs.prestige1Count);
                if (fullJourney)
                {
                    FullJourneyStep(gs);
                    if (stopping) return;
                    continue;
                }
                double cap = leg == 0 ? 21600 : 259200;
                bool milestone = leg > 0 && gs.IsDimensionMilestoneComplete(leg);
                if (simulated >= cap || milestone)
                {
                    Log("END milestone=" + milestone + " cap=" + cap);
                    Checkpoint(Current + "_END");
                    WriteSummary();
                    if (++leg >= Names.Length) { Stop(); return; }
                    File.Copy(Path.Combine(Folder, "NEW_UNTOUCHED.json"), SavePath, true);
                    SaveService.I.Load();
                    // Explicit technical entry fixture: only dimension entry flag; no stock grants.
                    gs.UnlockSelectedDimensionAfterPrestige1(leg);
                    simulated = 0;
                    UnityEngine.Random.InitState(260908 + leg);
                    Log("ENTRY_FIXTURE: fresh save + UnlockSelectedDimensionAfterPrestige1(" + leg + "); no prestige traversal or resource grants");
                    Checkpoint(Current + "_START");
                }
            } while (budget.ElapsedMilliseconds < 80);
        }
        catch (Exception ex) { Debug.LogException(ex); Stop(); }
    }

    static void FullJourneyStep(GameState gs)
    {
        int activeDimension = gs.prestige1CurrentDimensionId;
        if (activeDimension > 0 && gs.IsDimensionMilestoneComplete(activeDimension) &&
            !fullMilestoneRecorded[activeDimension])
        {
            fullMilestoneRecorded[activeDimension] = true;
            Log("MILESTONE D" + activeDimension + " completed");
            Checkpoint("FULL_D" + activeDimension + "_MILESTONE");
            WriteSummary();
        }

        if (gs.CanDoPrestige1(MachineManager.I))
        {
            int target = Enumerable.Range(1, 3).FirstOrDefault(d => !gs.IsDimensionUnlockedAfterPrestige1(d));
            Require(target > 0, "Prestige available without a locked dimension.");
            int beforeCount = gs.prestige1Count;
            Checkpoint("FULL_BEFORE_PRESTIGE_" + target);
            Require(gs.DoPrestige1Reset(target, MachineManager.I), "Prestige 1 failed for D" + target);
            Require(gs.prestige1Count == beforeCount + 1 && gs.IsDimensionUnlockedAfterPrestige1(target),
                "Prestige 1 did not preserve its result for D" + target);
            leg = target;
            simulated = 0;
            fusionRecipeCursor = 0;
            Log("REAL_PRESTIGE_1 target=D" + target + " count=" + gs.prestige1Count);
            Checkpoint("FULL_AFTER_PRESTIGE_" + target);
            WriteSummary();
            return;
        }

        bool complete = gs.prestige1Count == 3 &&
            gs.IsDimensionMilestoneComplete(1) && gs.IsDimensionMilestoneComplete(2) &&
            gs.IsDimensionMilestoneComplete(3);
        if (complete)
        {
            Checkpoint("FULL_D1_D3_END");
            WriteSummary();
            File.WriteAllText(Path.Combine(Folder, "full_result.txt"),
                "PASS | clean new game + real 60s + 3 Prestige 1 resets + D1-D3 milestones | no grants | no Convergence\n");
            Stop();
            return;
        }

        Require(totalSimulated <= 2592000, "Full journey exceeded 30 simulated days. " + gs.GetPrestige1StatusText());
    }

    static void VerifyCheckpointStep()
    {
        var gs = GameState.I;
        var tabs = TabsUI.Instance;
        TickSystem.I.enabled = false;
        MachineManager.I.enabled = false;
        switch (verifyStage++)
        {
            case 0:
                File.Copy(Path.Combine(CompletedJourney, "D" + verifyDimension + "_ENTRY_END.json"), SavePath, true);
                SaveService.I.Load();
                Require(!SaveService.I.HasLoadFailure && gs.IsDimensionMilestoneComplete(verifyDimension), "Milestone lost loading D" + verifyDimension);
                SetSize(1080, 1920);
                break;
            case 1:
                var report = UnityEngine.Object.FindFirstObjectByType<PresentationReturnReportUI>();
                if (report != null && report.transform.Find("ReturnModal").gameObject.activeInHierarchy)
                {
                    Button continueButton = report.GetComponentsInChildren<Button>().First(button => button.name == "Continue");
                    if (IsTopRaycastTarget(continueButton, out _)) Click(continueButton);
                    else
                    {
                        continueButton.onClick.Invoke();
                        PresentationReturnReportService.Consume();
                        Log("Fixture-only duplicate away report dismissed through Continue action; it was behind the retained prior panel");
                    }
                }
                var commandCenter = UnityEngine.Object.FindFirstObjectByType<Dimension1CommandCenterUI>(FindObjectsInactive.Include);
                var commandCenterCanvas = commandCenter != null
                    ? (CanvasGroup)typeof(Dimension1CommandCenterUI).GetField("canvasGroup", Private).GetValue(commandCenter)
                    : null;
                var drawer = commandCenter != null
                    ? (Button)typeof(Dimension1CommandCenterUI).GetField("dimensionDrawerToggle", Private).GetValue(commandCenter)
                    : null;
                if (commandCenterCanvas != null && commandCenterCanvas.blocksRaycasts && commandCenterCanvas.alpha > 0.5f &&
                    drawer != null && drawer.gameObject.activeInHierarchy && drawer.IsInteractable())
                {
                    Click(drawer);
                }
                else tabs.ShowGeneracion();
                break;
            case 2:
                // The fixture performs a manual Load after the scene's startup Load. If both
                // prepared an away report, close the second one through its real UI action too.
                var pendingReport = UnityEngine.Object.FindFirstObjectByType<PresentationReturnReportUI>();
                if (pendingReport != null && pendingReport.transform.Find("ReturnModal").gameObject.activeInHierarchy)
                {
                    Click(pendingReport.GetComponentsInChildren<Button>().First(button => button.name == "Continue"));
                    verifyStage--;
                    break;
                }
                var navigation = UnityEngine.Object.FindFirstObjectByType<VerticalNavigationUI>();
                navigation.RefreshAvailability();
                LayoutRebuilder.ForceRebuildLayoutImmediate(navigation.secondaryScroll.content);
                navigation.secondaryScroll.horizontalNormalizedPosition = verifyDimension == 1 ? 0f : verifyDimension == 2 ? 0.5f : 1f;
                Canvas.ForceUpdateCanvases();
                break;
            case 3:
                navigation = UnityEngine.Object.FindFirstObjectByType<VerticalNavigationUI>();
                LayoutRebuilder.ForceRebuildLayoutImmediate(navigation.secondaryScroll.content);
                navigation.secondaryScroll.horizontalNormalizedPosition = verifyDimension == 1 ? 0f : verifyDimension == 2 ? 0.5f : 1f;
                Canvas.ForceUpdateCanvases();
                Click(verifyDimension == 1 ? tabs.btnDimension1 : verifyDimension == 2 ? tabs.btnDimension2 : tabs.btnDimension3);
                break;
            case 4:
                Button entry = verifyDimension == 2 ? UnityEngine.Object.FindFirstObjectByType<Dimension2PanelUI>().continueFirstEntryButton
                    : verifyDimension == 3 ? UnityEngine.Object.FindFirstObjectByType<Dimension3PanelUI>().continueFirstEntryButton : null;
                if (entry != null && entry.gameObject.activeInHierarchy) Click(entry);
                break;
            case 5:
                var root = verifyDimension == 1 ? tabs.dimension1Panel : verifyDimension == 2 ? tabs.dimension2Panel : tabs.dimension3Panel;
                Require(root.activeInHierarchy, "D" + verifyDimension + " screen not visible");
                double before = gs.LE;
                for (int retry = 0; retry < 2; retry++)
                {
                    bool granted = verifyDimension == 1 ? Dimension1System.TryStartD1ArkFinalMission(gs, out _)
                        : verifyDimension == 2 ? D2Civilization2System.TryEstablishMajorPact(gs)
                        : D3AutonomyCoreSystem.TryIntegrate(gs, out _);
                    Require(!granted && gs.LE == before && gs.IsDimensionMilestoneComplete(verifyDimension), "Repeated milestone changed D" + verifyDimension);
                }
                break;
            case 6:
                Checkpoint("D" + verifyDimension + "_RELOADED");
                File.AppendAllText(Path.Combine(Folder, "verification.txt"), "D" + verifyDimension + " LOAD_PASS | ENTRY_RAYCAST_PASS | REPEAT_MILESTONE_PASS\n");
                if (++verifyDimension > 3) { verifyDimension = 1; verifyStage = 100; return; }
                verifyStage = 0;
                break;
            case 100:
                File.Copy(Path.Combine(Folder, "D" + verifyDimension + "_RELOADED.json"), SavePath, true);
                SaveService.I.Load();
                PresentationReturnReportService.Consume();
                var captureReport = UnityEngine.Object.FindFirstObjectByType<PresentationReturnReportUI>();
                if (captureReport != null) captureReport.transform.Find("ReturnModal").gameObject.SetActive(false);
                if (verifyDimension == 1) tabs.ShowDimension1();
                else if (verifyDimension == 2) tabs.ShowDimension2();
                else tabs.ShowDimension3();
                SetSize(1080, 1920);
                break;
            case 101:
                Capture("D" + verifyDimension + "_completed_1080x1920.png");
                SetSize(720, 1280);
                break;
            case 102:
                typeof(PresentationReturnReportCapture).GetMethod("Capture", BindingFlags.Static | BindingFlags.NonPublic)
                    .Invoke(null, new object[] {Path.Combine(Folder, "D" + verifyDimension + "_completed_720x1280.png"), 720, 1280});
                File.AppendAllText(Path.Combine(Folder, "verification.txt"), "D" + verifyDimension + " CAPTURE_PASS | 1080 + 720\n");
                if (++verifyDimension > 3) { Stop(); return; }
                verifyStage = 100;
                break;
        }
    }

    static void SetSize(int width, int height)
    {
        typeof(ReleaseRecoveryRuntimeValidation)
            .GetMethod("SetResolution", BindingFlags.Static | BindingFlags.NonPublic)
            .Invoke(null, new object[] {width, height});
        MethodInfo handle = typeof(CanvasScaler).GetMethod("Handle", BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (CanvasScaler scaler in UnityEngine.Object.FindObjectsByType<CanvasScaler>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            if (scaler.enabled) handle?.Invoke(scaler, null);
        Canvas.ForceUpdateCanvases();
    }

    static void BaseActions(GameState gs)
    {
        if (UpgradeStudySystem.TryRevealConclusion(gs, out string revealed)) Log("study revealed " + revealed);
        foreach (var study in UpgradeStudySystem.Definitions)
        {
            if (F2UpgradeManager.I.CanBuy(study.unlockId) && F2UpgradeManager.I.TryBuy(study.unlockId)) Log("upgrade " + study.unlockId);
            if (UpgradeStudySystem.IsDiscovered(gs, study.unlockId)) continue;
            if (gs.CanUseTriangleCircuits() && UpgradeStudySystem.GetActiveStudy(gs) == null)
            {
                var circuit = study.requiredCircuit == UpgradeStudyCircuitRequirement.Experimental ? TriangleCircuitType.Experimental
                    : study.requiredCircuit == UpgradeStudyCircuitRequirement.TriangleEnergy ? TriangleCircuitType.TriangleEnergy : TriangleCircuitType.Energy;
                if (gs.triangleActiveCircuit != circuit && gs.SetTriangleCircuit(circuit)) Log("circuit " + circuit);
            }
            if (UpgradeStudySystem.TryStartStudy(gs, study.id)) { Log("study started " + study.id); break; }
            // Prioritize the first undiscovered eligible study's synchronization.
            if (study.requiresTriangleUnlocked && gs.triangleSystemUnlocked) break;
        }
        if (gs.CanBuyExperimentalChamberKeycard() && gs.TryBuyExperimentalChamberKeycard()) Log("experimental chamber keycard");
        if (gs.experimentalChamberUnlocked && !MachineManager.I.MachineUnlocked && gs.TryUnlockMachineFromExperimentalChamber()) Log("Machine unlocked");
        if (MachineManager.I.MachineUnlocked)
            foreach (var node in MachineManager.I.GetAllNodes())
            {
                if (MachineManager.I.CanRepairNode(node.id, out _) && MachineManager.I.TryRepairNode(node.id)) Log("repair " + node.id);
                if (!MachineManager.I.IsAnalyzingNode && MachineManager.I.CanAnalyzeNode(node.id, false, out _) &&
                    MachineManager.I.TryStartNodeAnalysis(node.id, MachineManager.BaseNodeAnalysisDurationSeconds, false, out _)) Log("analysis " + node.id);
            }
        TryRunFusion(gs);
        // Buy base progression when affordable. ROI ceiling bounds reinvestment to let resources accumulate.
        BuildingState best = null; double score = double.MaxValue;
        foreach (var def in BuildingDatabase.I.buildings)
        {
            var building = gs.GetBuildingState(def.id);
            if (building == null || !BuildingUnlock.IsUnlocked(def)) continue;
            double cost = gs.GetEffectiveBuildingCost(building);
            if (cost > gs.LE || (def.id == "fluctuation_antenna" && building.level >= 1)) continue;
            double roi = building.level == 0 ? 0 : cost / Math.Max(0.0001, gs.GetBuildingNextLevelLEPerSecond(building));
            if (roi < score && roi <= 3600) { best = building; score = roi; }
        }
        if (best != null && BuildingPurchaseService.TryPurchase(gs, best)) Log("building " + best.def.id + " level=" + best.level);
    }

    static void TryRunFusion(GameState gs)
    {
        if (!gs.experimentalChamberUnlocked || MachineManager.I == null || !MachineManager.I.MachineUnlocked)
            return;
        var room = UnityEngine.Object.FindFirstObjectByType<Room2PanelUI>(FindObjectsInactive.Include);
        if (room == null) return;
        typeof(Room2PanelUI).GetMethod("PullFusionRuntimeStateFromGameState", Private)?.Invoke(room, null);
        if (room.FusionCoolingDown) return;

        var orderedResults = new[]
        {
            ExperimentalResultType.Hallazgo, ExperimentalResultType.Muestra,
            ExperimentalResultType.LecturaIncompleta, ExperimentalResultType.CompuestoUtil
        }.OrderByDescending(result => RemainingFusionNeed(result) - GetExperimentalResultCount(gs, result))
         .ThenBy(result => ((int)result + fusionRecipeCursor) % 4).ToArray();
        var recipes = orderedResults.Select(result =>
        {
            if (result == ExperimentalResultType.Hallazgo)
                return new object[] { ExperimentalFragmentType.Condensation, ExperimentalFragmentType.Confinement, ExperimentalCatalystType.Alpha, result };
            if (result == ExperimentalResultType.Muestra)
                return new object[] { ExperimentalFragmentType.Condensation, ExperimentalFragmentType.ResidualInterference, ExperimentalCatalystType.Alpha, result };
            if (result == ExperimentalResultType.LecturaIncompleta)
                return new object[] { ExperimentalFragmentType.Confinement, ExperimentalFragmentType.ResidualInterference, ExperimentalCatalystType.Beta, result };
            ExperimentalFragmentType abundant = gs.fragmentCondensation >= gs.fragmentConfinement
                ? ExperimentalFragmentType.Condensation : ExperimentalFragmentType.Confinement;
            return new object[] { abundant, abundant, ExperimentalCatalystType.Alpha, result };
        }).ToArray();
        if (RemainingFusionNeed((ExperimentalResultType)recipes[0][3]) <=
            GetExperimentalResultCount(gs, (ExperimentalResultType)recipes[0][3])) return;
        for (int offset = 0; offset < recipes.Length; offset++)
        {
            int index = offset;
            var recipe = recipes[index];
            var desiredResult = (ExperimentalResultType)recipe[3];
            if (RemainingFusionNeed(desiredResult) <= GetExperimentalResultCount(gs, desiredResult)) continue;
            var a = (ExperimentalFragmentType)recipe[0];
            var b = (ExperimentalFragmentType)recipe[1];
            if (!D3FusionService.HasRequiredFragments(gs, a, b)) continue;
            typeof(Room2PanelUI).GetField("selectedFragmentA", Private)?.SetValue(room, a);
            typeof(Room2PanelUI).GetField("selectedFragmentB", Private)?.SetValue(room, b);
            typeof(Room2PanelUI).GetField("selectedCatalyst", Private)?.SetValue(room, recipe[2]);
            FieldInfo mode = typeof(Room2PanelUI).GetField("currentTrialMode", Private);
            mode?.SetValue(room, Enum.ToObject(mode.FieldType, 0));
            if (!room.CanExecuteFusion) return;
            int before = GetExperimentalResultCount(gs, (ExperimentalResultType)recipe[3]);
            typeof(Room2PanelUI).GetMethod("OnClickMixButton", Private)?.Invoke(room, null);
            if (!room.FusionInProgress)
                throw new InvalidOperationException("Fusion did not enter the pending state.");
            room.AdvanceQaFusionCooldown(999.0);
            int after = GetExperimentalResultCount(gs, (ExperimentalResultType)recipe[3]);
            if (room.FusionInProgress)
                throw new InvalidOperationException("Fusion remained pending after its timer completed.");
            Log("fusion " + a + "+" + b + "+" + recipe[2] + " result=" + room.LastFusionResult + " reward=" + (after - before));
            fusionRecipeCursor = (fusionRecipeCursor + 1) % recipes.Length;
            return;
        }
    }

    static int RemainingFusionNeed(ExperimentalResultType result)
    {
        int total = 0;
        foreach (MachineNodeDef node in MachineManager.I.GetAllNodes(false))
        {
            if (node == null || node.hidden || node.retired || MachineManager.I.IsNodeRepaired(node.id) || node.cost == null) continue;
            switch (result)
            {
                case ExperimentalResultType.Hallazgo: total += node.cost.hallazgo; break;
                case ExperimentalResultType.Muestra: total += node.cost.muestra; break;
                case ExperimentalResultType.LecturaIncompleta: total += node.cost.lecturaIncompleta; break;
                case ExperimentalResultType.CompuestoUtil: total += node.cost.compuestoUtil; break;
            }
        }
        return total;
    }

    static int GetExperimentalResultCount(GameState gs, ExperimentalResultType result)
    {
        switch (result)
        {
            case ExperimentalResultType.Hallazgo: return gs.experimentalHallazgos;
            case ExperimentalResultType.Muestra: return gs.experimentalMuestras;
            case ExperimentalResultType.LecturaIncompleta: return gs.experimentalLecturasIncompletas;
            case ExperimentalResultType.CompuestoUtil: return gs.experimentalCompuestosUtiles;
            default: return 0;
        }
    }

    static void Checkpoint(string name)
    {
        Require(SaveService.I.CurrentSavePath == SavePath && Path.GetFullPath(Folder).Contains("Logs" + Path.DirectorySeparatorChar + "ReleaseD1D3"), "Checkpoint outside isolated fixture.");
        bool suppressed = SaveService.SuppressWritesForVisualQa;
        // Public suppression is intentionally latched for visual QA. Only this
        // isolated Editor fixture temporarily releases its backing field.
        var guard = typeof(SaveService).GetField("suppressWritesForVisualQa", BindingFlags.Static | BindingFlags.NonPublic);
        try
        {
            guard.SetValue(null, false);
            Require(SaveService.I.TrySave(out string reason), "Checkpoint failed " + reason);
            Require(SaveService.TryReadSaveData(SavePath, out SaveData saved), "Checkpoint unreadable");
            var gs = GameState.I;
            Require(Math.Abs(saved.LE - gs.LE) <= Math.Max(0.001, Math.Abs(gs.LE) * 0.000000001) &&
                saved.dimension01Unlocked == gs.dimension01Unlocked &&
                saved.dimension02Unlocked == gs.dimension02Unlocked && saved.dimension03Unlocked == gs.dimension03Unlocked &&
                saved.dimension1GalacticAnchorDiscovered == gs.dimension1GalacticAnchorDiscovered &&
                saved.dimension2.civilization2.majorPactEstablished == gs.dimension2.civilization2.majorPactEstablished &&
                saved.dimension3.autonomyCoreIntegrated == gs.dimension3.autonomyCoreIntegrated,
                "Checkpoint did not capture current state " + name);
            File.Copy(SavePath, Path.Combine(Folder, name + ".json"), true);
        }
        finally { guard.SetValue(null, suppressed); }
    }

    static void WriteSummary()
    {
        var gs = GameState.I;
        var text = Current + " simulated_seconds=" + simulated + " LE=" + gs.LE + " traces=" + gs.Traces +
            " LEps=" + gs.GetTotalLEps() + " D1=" + gs.dimension01Unlocked + " D2=" + gs.dimension02Unlocked + " D3=" + gs.dimension03Unlocked +
            " milestones=" + gs.IsDimensionMilestoneComplete(1) + "," + gs.IsDimensionMilestoneComplete(2) + "," + gs.IsDimensionMilestoneComplete(3) +
            " Machine=" + MachineManager.I.MachineUnlocked + " repair=" + MachineManager.I.GetTotalMachineRepairProgress01() +
            " prestige1=" + gs.prestige1Count + " total_simulated_seconds=" + totalSimulated;
        File.AppendAllText(Path.Combine(Folder, "summary.txt"), text + "\n");
    }

    static void Click(Button button)
    {
        Require(button != null && button.IsInteractable() && button.gameObject.activeInHierarchy, "Button unavailable");
        Require(IsTopRaycastTarget(button, out GameObject first),
            "Button raycast blocked: target=" + button.name + " first=" + (first != null ? first.name : "NONE"));
        var rect = (RectTransform)button.transform;
        var pointer = new PointerEventData(EventSystem.current) { position = RectTransformUtility.WorldToScreenPoint(null, rect.TransformPoint(rect.rect.center)) };
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerClickHandler);
    }

    static bool IsTopRaycastTarget(Button button, out GameObject first)
    {
        first = null;
        if (button == null || !button.IsInteractable() || !button.gameObject.activeInHierarchy) return false;
        Canvas.ForceUpdateCanvases();
        var rect = (RectTransform)button.transform;
        var pointer = new PointerEventData(EventSystem.current) { position = RectTransformUtility.WorldToScreenPoint(null, rect.TransformPoint(rect.rect.center)) };
        var hits = new List<RaycastResult>(); EventSystem.current.RaycastAll(pointer, hits);
        first = hits.Count > 0 ? hits[0].gameObject : null;
        return first != null && ExecuteEvents.GetEventHandler<IPointerClickHandler>(first) == button.gameObject;
    }
    static void Capture(string name) => typeof(PresentationReturnReportCapture).GetMethod("Capture", BindingFlags.Static | BindingFlags.NonPublic)
        .Invoke(null, new object[] {Path.Combine(Folder, name), 1080, 1920});
    static void Stop() { stopping = true; EditorApplication.update -= Tick; EditorApplication.isPlaying = false; }
    static void Require(bool ok, string text) { if (!ok) throw new InvalidOperationException(text); }
}
#endif

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class ConvergenceAuditCorrectionValidation
{
    private const string C1 = ConvergenceCircuitCatalog.StartupPulseCircuitId;
    private const string C2 = "convergence_circuit_002_axial_link";
    private const string C3 = "convergence_circuit_003_resonant_elbow";
    private const string C4 = "convergence_circuit_004_triaxial_distributor";
    private const string C5 = "convergence_circuit_005_local_coupler";
    private const string C6 = "convergence_circuit_006_closure_matrix";

    [MenuItem("Tools/Quantum Forge/Convergence/Validate Sol High Corrections")]
    public static void ValidateCorrections()
    {
        var failures = new List<string>();
        ValidateC1ThroughUIAndPersistentErrors(failures);
        ValidateNewlyAwardedRules(failures);
        ValidateInvalidDrafts(failures);
        ValidateEditableSaveRollbacks(failures);
        ValidateTransactionFailureCheckpoints(failures);
        ValidateSnapshotHardening(failures);
        ValidateSynchronizationPhases(failures);
        ValidateUnknownSignalModal(failures);
        ValidateDedicatedPanelTopology(failures);
        ValidateDegreeAndTutorials(failures);

        if (failures.Count == 0)
            Debug.Log("[Convergence Sol High] PASS | rollback de edición | retry transaccional | UI scroll/táctil | snapshots históricos | tutoriales C2/C4 | degree | C1-C6 | modal ???");
        else
            Debug.LogError("[Convergence Sol High] FAIL\n- " +
                string.Join("\n- ", failures));
    }

    public static void ValidateCorrectionsBatch() { ValidateCorrections(); }

    private static void ValidateC1ThroughUIAndPersistentErrors(List<string> failures)
    {
        PropertyInfo singleton = typeof(GameState).GetProperty("I",
            BindingFlags.Public | BindingFlags.Static);
        GameState previous = GameState.I;
        GameState valid = CreatePendingState("UI C1 Valid", 1);
        GameObject panelObject = null;
        try
        {
            valid.convergence.draftPlacements.Add(P(C1, 0, 1, 0));
            singleton.SetValue(null, valid, null);
            panelObject = new GameObject("UI C1 Panel", typeof(RectTransform),
                typeof(ConvergencePanelUI));
            ConvergencePanelUI panel = panelObject.GetComponent<ConvergencePanelUI>();
            panel.Initialize(null);
            panel.Show();
            Check(panel.ConfirmButton != null && panel.ConfirmButton.interactable,
                "El botón UI de C1 no usa draftPlacements energizado.", failures);
            panel.ConfirmFromUI();
            Check(valid.convergence.completedCycles == 1 &&
                  Near(valid.convergence.activeSnapshot.baseLEProductionMultiplier, 1.10),
                "C1 no puede estabilizarse desde ConvergencePanelUI.", failures);
        }
        finally
        {
            singleton.SetValue(null, previous, null);
            if (panelObject != null) UnityEngine.Object.DestroyImmediate(panelObject);
            UnityEngine.Object.DestroyImmediate(valid.gameObject);
        }

        GameState invalid = CreatePendingState("UI C1 Error", 1);
        panelObject = null;
        try
        {
            invalid.convergence.draftPlacements.Add(P(C1, 0, 2, 0));
            singleton.SetValue(null, invalid, null);
            panelObject = new GameObject("UI Error Panel", typeof(RectTransform),
                typeof(ConvergencePanelUI));
            ConvergencePanelUI panel = panelObject.GetComponent<ConvergencePanelUI>();
            panel.Initialize(null);
            panel.Show();
            panel.ConfirmFromUI();
            string error = panel.PersistentError;
            panel.Refresh();
            Check(!string.IsNullOrWhiteSpace(error) && panel.PersistentError == error,
                "El refresco periódico borra inmediatamente el error de confirmación.", failures);
            invalid.convergence.draftPlacements = new List<ConvergenceCircuitPlacement>
                { P(C1, 0, 1, 0) };
            invalid.convergence.invalidDraftDetected = false;
            panel.ConfirmFromUI();
            Check(string.IsNullOrWhiteSpace(panel.PersistentError) &&
                  invalid.convergence.completedCycles == 1,
                "El error persistente no se limpia al corregir y confirmar la placa.", failures);
        }
        finally
        {
            singleton.SetValue(null, previous, null);
            if (panelObject != null) UnityEngine.Object.DestroyImmediate(panelObject);
            UnityEngine.Object.DestroyImmediate(invalid.gameObject);
        }
    }

    private static void ValidateNewlyAwardedRules(List<string> failures)
    {
        string[] ids = { C2, C3, C4, C5 };
        int[] rotations = { 0, 180, 180, 0 };
        for (int ordinal = 2; ordinal <= 5; ordinal++)
        {
            GameState state = CreatePendingState("New Award C" + ordinal, ordinal);
            try
            {
                state.convergence.draftPlacements.Add(P(ids[ordinal - 2], 0, 1,
                    rotations[ordinal - 2]));
                bool canConfirm = ConvergenceCircuitSystem.CanConfirmConfiguration(state, out _);
                bool confirmed = ConvergenceCircuitSystem.TryConfirmConfiguration(
                    state, out string reason);
                Check(canConfirm && confirmed &&
                      state.convergence.completedCycles == ordinal,
                    "C" + ordinal + " exige C1 permanentemente o no acepta el circuito nuevo: " + reason,
                    failures);
            }
            finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
        }

        GameState c6Invalid = CreatePendingState("C6 Requires Six", 6);
        try
        {
            List<ConvergenceCircuitPlacement> final = FinalLayout();
            final.RemoveAll(p => p.circuitId == C1);
            c6Invalid.convergence.draftPlacements = final;
            Check(!ConvergenceCircuitSystem.TryConfirmConfiguration(c6Invalid, out _) &&
                  c6Invalid.convergence.completedCycles == 5,
                "C6 confirma sin seis circuitos energizados.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(c6Invalid.gameObject); }

        GameState c6Valid = CreatePendingState("C6 Valid", 6);
        try
        {
            c6Valid.convergence.draftPlacements = FinalLayout();
            Check(ConvergenceCircuitSystem.TryConfirmConfiguration(c6Valid, out string reason) &&
                  c6Valid.convergence.completedCycles == 6 &&
                  c6Valid.convergence.normalConvergenceCompleted,
                "C6 no confirma una placa estructural 6/6: " + reason, failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(c6Valid.gameObject); }
    }

    private static void ValidateInvalidDrafts(List<string> failures)
    {
        for (int ordinal = 1; ordinal <= 6; ordinal++)
        {
            GameState state = CreatePendingState("Corrupt Draft C" + ordinal, ordinal);
            try
            {
                string id = ConvergenceCircuitCatalog.Definitions.Find(d =>
                    d.awardOrdinal == ordinal).id;
                int rotation = ordinal == 1 || ordinal == 2 || ordinal == 5 ? 0 : 180;
                state.convergence.draftPlacements.Add(P(id, 0, 1, rotation));
                state.convergence.draftPlacements.Add(P(id, 1, 1, rotation));
                Check(!ConvergenceCircuitSystem.TryConfirmConfiguration(state, out _) &&
                      state.convergence.pendingTransaction == null &&
                      state.convergence.completedCycles == ordinal - 1,
                    "Una placa corrupta confirma C" + ordinal + ".", failures);
            }
            finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
        }
    }

    private static void ValidateTransactionFailureCheckpoints(List<string> failures)
    {
        for (int checkpoint = 1; checkpoint <= 6; checkpoint++)
        {
            GameState state = CreatePendingState("Transaction Checkpoint " + checkpoint, 1);
            try
            {
                state.convergence.draftPlacements.Add(P(C1, 0, 1, 0));
                ConvergenceCircuitSystem.SetPersistFailureForValidation(checkpoint);
                bool confirmed = ConvergenceCircuitSystem.TryConfirmConfiguration(
                    state, out string failureReason);
                bool expectedFailureState = checkpoint == 1
                    ? state.convergence.pendingTransaction == null &&
                      state.convergence.phase == ConvergencePhase.ConfigurationPending &&
                      !state.convergence.boardConfigurationLocked &&
                      state.convergence.draftPlacements.Count == 1
                    : state.convergence.pendingTransaction != null;
                Check(!confirmed && expectedFailureState &&
                      !string.IsNullOrWhiteSpace(failureReason),
                    "El fallo Persist #" + checkpoint +
                    " responde éxito o deja un estado no reintentable.", failures);

                ConvergenceCircuitSystem.ClearPersistFailureForValidation();
                bool recovered = checkpoint == 1
                    ? ConvergenceCircuitSystem.TryConfirmConfiguration(state,
                        out string recoveryReason)
                    : ConvergenceCircuitSystem.TryRecoverTransaction(state,
                        out recoveryReason);
                Check(recovered &&
                      state.convergence.pendingTransaction == null &&
                      state.convergence.completedCycles == 1 &&
                      state.convergence.ownedCircuits.Count == 1 &&
                      state.convergence.recordedTransactionIds.Count == 1 &&
                      state.convergence.phase == ConvergencePhase.NewCycleStarted,
                    "Recovery desde Persist #" + checkpoint +
                    " no termina exactamente una vez: " + recoveryReason, failures);
                ConvergenceCircuitSystem.TryRecoverTransaction(state, out _);
                Check(state.convergence.completedCycles == 1 &&
                      state.convergence.recordedTransactionIds.Count == 1,
                    "Recovery repetido duplica el resultado de Persist #" + checkpoint + ".",
                    failures);
            }
            finally
            {
                ConvergenceCircuitSystem.ClearPersistFailureForValidation();
                UnityEngine.Object.DestroyImmediate(state.gameObject);
            }
        }
    }

    private static void ValidateSnapshotHardening(List<string> failures)
    {
        GameState state = CreatePendingState("Snapshot Restore", 1);
        try
        {
            state.convergence.committedPlacements.Add(P(C1, 0, 1, 0));
            state.convergence.activeSnapshot = new ConvergenceModifierSnapshot
            {
                baseLEProductionMultiplier = double.NaN,
                baseTracesProductionMultiplier = double.PositiveInfinity,
                stabilityGainMultiplier = 1.75,
                layoutHash = "corrupt"
            };
            state.EnsureConvergenceState();
            Check(Near(state.convergence.activeSnapshot.baseLEProductionMultiplier, 1.10) &&
                  Near(state.convergence.activeSnapshot.baseTracesProductionMultiplier, 1.0) &&
                  Near(state.convergence.activeSnapshot.stabilityGainMultiplier, 1.0) &&
                  state.convergence.activeSnapshot.schemaVersion == ConvergenceSystem.SnapshotSchemaVersion &&
                  state.convergence.activeSnapshot.catalogRevision == ConvergenceCircuitCatalog.CatalogRevision,
                "Un snapshot NaN/Infinity/fuera de límite no se restaura desde la placa.", failures);
            Check(!ConvergenceSystem.IsSnapshotValid(new ConvergenceModifierSnapshot
                { baseLEProductionMultiplier = 2.01, baseTracesProductionMultiplier = 1.0, stabilityGainMultiplier = 1.0 }) &&
                  !ConvergenceSystem.IsSnapshotValid(new ConvergenceModifierSnapshot
                { baseLEProductionMultiplier = 1.0, baseTracesProductionMultiplier = 2.01, stabilityGainMultiplier = 1.0 }) &&
                  !ConvergenceSystem.IsSnapshotValid(new ConvergenceModifierSnapshot
                { baseLEProductionMultiplier = 1.0, baseTracesProductionMultiplier = 1.0, stabilityGainMultiplier = 1.51 }),
                "Los límites ×2.0/×2.0/×1.5 no se aplican estrictamente.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }

        GameState historical = CreatePendingState("Snapshot Historical", 3);
        try
        {
            historical.convergence.activeSnapshot = new ConvergenceModifierSnapshot
            {
                schemaVersion = 9,
                catalogRevision = 77,
                cycleId = 2,
                layoutHash = "historic-layout",
                baseLEProductionMultiplier = 1.37,
                baseTracesProductionMultiplier = 1.22,
                stabilityGainMultiplier = 1.11,
                activeCircuitIds = new List<string> { C1, C2 }
            };
            historical.EnsureConvergenceState();
            ConvergenceModifierSnapshot preserved = historical.convergence.activeSnapshot;
            Check(Near(preserved.baseLEProductionMultiplier, 1.37) &&
                  Near(preserved.baseTracesProductionMultiplier, 1.22) &&
                  Near(preserved.stabilityGainMultiplier, 1.11) &&
                  preserved.schemaVersion == 9 && preserved.catalogRevision == 77 &&
                  preserved.cycleId == 2 && preserved.layoutHash == "historic-layout",
                "La normalización recalcula o cambia la revisión de un snapshot histórico válido.",
                failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(historical.gameObject); }

        GameState metadata = CreatePendingState("Snapshot Metadata", 1);
        try
        {
            metadata.convergence.draftPlacements.Add(P(C1, 0, 1, 0));
            Check(ConvergenceCircuitSystem.TryConfirmConfiguration(metadata, out _) &&
                  metadata.convergence.activeSnapshot.schemaVersion == ConvergenceSystem.SnapshotSchemaVersion &&
                  metadata.convergence.activeSnapshot.catalogRevision == ConvergenceCircuitCatalog.CatalogRevision &&
                  metadata.convergence.activeSnapshot.cycleId == 1 &&
                  !string.IsNullOrWhiteSpace(metadata.convergence.activeSnapshot.layoutHash),
                "La confirmación no completa los metadatos del snapshot.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(metadata.gameObject); }
    }

    private static void ValidateSynchronizationPhases(List<string> failures)
    {
        GameState state = CreateCompletedDimensionState("Synchronization Phases");
        try
        {
            state.convergence.dimensionalReceiverRebuilt = false;
            state.convergence.phase = ConvergencePhase.Inactive;
            Check(!ConvergenceSynchronizationSystem.TryAddSynchronization(state, 1,
                    true, 40.0, "outside_phase", out _),
                "INACTIVE acepta estabilidad.", failures);
            state.convergence.dimensionalReceiverRebuilt = true;
            state.convergence.phase = ConvergencePhase.Synchronizing;
            Check(ConvergenceSynchronizationSystem.TryAddSynchronization(state, 1,
                    true, 40.0, "phase_d1", out _) &&
                  ConvergenceSynchronizationSystem.TryAddSynchronization(state, 2,
                    true, 40.0, "phase_d2", out _) &&
                  ConvergenceSynchronizationSystem.TryAddSynchronization(state, 3,
                    true, 40.0, "phase_d3", out _) &&
                  state.convergence.phase == ConvergencePhase.Ready,
                "La tercera señal no fija READY de forma inequívoca.", failures);
            Check(!ConvergenceSynchronizationSystem.TryAddSynchronization(state, 1,
                    false, 1.0, "after_ready", out _),
                "READY acepta fuentes nuevas.", failures);
            state.convergence.phase = ConvergencePhase.ConfigurationPending;
            Check(!ConvergenceSynchronizationSystem.TryAddSynchronization(state, 1,
                    false, 1.0, "during_config", out _),
                "CONFIGURING acepta estabilidad.", failures);
            state.convergence.phase = ConvergencePhase.Completed;
            Check(!ConvergenceSynchronizationSystem.TryAddSynchronization(state, 1,
                    false, 1.0, "after_completed", out _),
                "COMPLETED acepta estabilidad.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateUnknownSignalModal(List<string> failures)
    {
        PropertyInfo singleton = typeof(GameState).GetProperty("I",
            BindingFlags.Public | BindingFlags.Static);
        GameState previous = GameState.I;
        GameState state = CreateCompletedDimensionState("Unknown Signal Modal");
        GameObject panelObject = null;
        try
        {
            state.convergence.normalConvergenceCompleted = true;
            state.convergence.unknownSignalTriggered = true;
            state.convergence.unknownSignalAcknowledged = false;
            state.convergence.pendingNarrativeEventId = ConvergencePanelUI.UnknownSignalEventId;
            state.convergence.phase = ConvergencePhase.Completed;
            state.convergence.completedCycles = 6;
            state.convergence.nextAwardOrdinal = 7;
            singleton.SetValue(null, state, null);
            panelObject = new GameObject("Unknown Modal Panel", typeof(RectTransform),
                typeof(ConvergencePanelUI));
            ConvergencePanelUI panel = panelObject.GetComponent<ConvergencePanelUI>();
            panel.Initialize(null);
            panel.Show();
            Check(panel.UnknownModalVisible &&
                  ConvergencePanelUI.UnknownSignalTitle == "???" &&
                  ConvergencePanelUI.UnknownSignalBody ==
                    "La red ha detectado una respuesta que no puede interpretar.",
                "El modal ??? no aparece automáticamente con el texto acordado.", failures);
            panel.Hide();
            panel.Show();
            Check(panel.UnknownModalVisible,
                "El modal cerrado sin reconocer no reaparece.", failures);
            Check(ConvergencePanelUI.TryAcknowledgeUnknownSignal(state, out _) &&
                  state.convergence.unknownSignalAcknowledged &&
                  string.IsNullOrEmpty(state.convergence.pendingNarrativeEventId),
                "CONTINUAR no reconoce y persiste el evento.", failures);
            panel.Refresh();
            Check(!panel.UnknownModalVisible &&
                  !ConvergenceCircuitSystem.HasNextDesignedCircuit(state),
                "El modal reaparece tras reconocer o el endpoint permite C7.", failures);
        }
        finally
        {
            singleton.SetValue(null, previous, null);
            if (panelObject != null) UnityEngine.Object.DestroyImmediate(panelObject);
            UnityEngine.Object.DestroyImmediate(state.gameObject);
        }
    }

    private static void ValidateDedicatedPanelTopology(List<string> failures)
    {
        GameObject panelObject = new GameObject("Panel Topology", typeof(RectTransform),
            typeof(ConvergencePanelUI));
        try
        {
            ConvergencePanelUI panel = panelObject.GetComponent<ConvergencePanelUI>();
            panel.Initialize(null);
            MethodInfo portGlyph = typeof(ConvergencePanelUI).GetMethod("PortGlyph",
                BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo arrowGlyph = typeof(ConvergencePanelUI).GetMethod("ArrowGlyph",
                BindingFlags.NonPublic | BindingFlags.Static);
            Transform content = panelObject.transform.Find("ViewScroll/Viewport/Content");
            VerticalLayoutGroup rootLayout = panelObject.GetComponent<VerticalLayoutGroup>();
            ScrollRect scroll = panelObject.GetComponentInChildren<ScrollRect>(true);
            GridLayoutGroup grid = panelObject.GetComponentInChildren<GridLayoutGroup>(true);
            bool touchTargetsValid = true;
            foreach (Button button in panelObject.GetComponentsInChildren<Button>(true))
            {
                if (button.gameObject.name.StartsWith("Cell_")) continue;
                LayoutElement element = button.GetComponent<LayoutElement>();
                if (element == null || element.minHeight < 44f) touchTargetsValid = false;
            }
            Check(content != null && content.Find("CycleView") != null &&
                  content.Find("BoardView") != null &&
                  content.Find("ArchiveView") != null &&
                  panelObject.transform.Find("UnknownSignalModal") != null &&
                  scroll != null && scroll.vertical && !scroll.horizontal &&
                  rootLayout != null && rootLayout.childControlHeight &&
                  grid != null && grid.cellSize.x >= 44f && grid.cellSize.y >= 44f &&
                  touchTargetsValid && panel.RetryTransactionButton != null &&
                  (string)portGlyph.Invoke(null, new object[] { ConvergenceCircuitCatalog.North }) == "╵" &&
                  (string)portGlyph.Invoke(null, new object[] { ConvergenceCircuitCatalog.North | ConvergenceCircuitCatalog.South }) == "│" &&
                  (string)portGlyph.Invoke(null, new object[] { ConvergenceCircuitCatalog.North | ConvergenceCircuitCatalog.East }) == "└" &&
                  (string)portGlyph.Invoke(null, new object[] { ConvergenceCircuitCatalog.North | ConvergenceCircuitCatalog.East | ConvergenceCircuitCatalog.West }) == "┴" &&
                  (string)portGlyph.Invoke(null, new object[] { 15 }) == "┼" &&
                  (string)arrowGlyph.Invoke(null, new object[] { ConvergenceCircuitCatalog.East }) == "→" &&
                  typeof(PrestigeUI).GetField("_convergenceRoot",
                    BindingFlags.NonPublic | BindingFlags.Instance) == null,
                "ConvergencePanelUI no contiene Ciclo/Placa/Archivo/modal o PrestigeUI conserva la implementación antigua.",
                failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(panelObject); }
    }

    private static void ValidateEditableSaveRollbacks(List<string> failures)
    {
        GameState award = CreateCompletedDimensionState("Award rollback");
        try
        {
            award.convergence.phase = ConvergencePhase.Ready;
            award.convergence.dimensionalReceiverRebuilt = true;
            award.convergence.currentStability = 1000.0;
            foreach (ConvergenceSignalState signal in award.convergence.signals)
                signal.activated = true;
            ConvergenceCircuitSystem.SetPersistFailureForValidation(1);
            Check(!ConvergenceCircuitSystem.TryStartNormalConvergence(award, out _) &&
                  award.convergence.ownedCircuits.Count == 0 &&
                  award.convergence.phase == ConvergencePhase.Ready &&
                  ReferenceEquals(award.convergence.boardPlacements,
                      award.convergence.committedPlacements),
                "Entregar un circuito no revierte al fallar Persist.", failures);
        }
        finally
        {
            ConvergenceCircuitSystem.ClearPersistFailureForValidation();
            UnityEngine.Object.DestroyImmediate(award.gameObject);
        }

        GameState state = CreatePendingState("Edit rollback", 1);
        try
        {
            ConvergenceCircuitSystem.SetPersistFailureForValidation(1);
            Check(!ConvergenceCircuitSystem.TryPlaceCircuit(state, C1, 0, 1, 0, out _) &&
                  state.convergence.draftPlacements.Count == 0,
                "Colocar no revierte el borrador al fallar Persist.", failures);

            state.convergence.draftPlacements.Add(P(C1, 0, 1, 0));
            state.convergence.boardPlacements = state.convergence.draftPlacements;
            ConvergenceCircuitSystem.SetPersistFailureForValidation(1);
            Check(!ConvergenceCircuitSystem.TryRemoveCircuit(state, C1, out _) &&
                  state.convergence.draftPlacements.Count == 1,
                "Retirar no revierte el borrador al fallar Persist.", failures);

            state.convergence.committedPlacements = new List<ConvergenceCircuitPlacement>
                { P(C1, 0, 1, 0) };
            state.convergence.draftPlacements = new List<ConvergenceCircuitPlacement>
                { P(C1, 1, 0, 270) };
            state.convergence.boardPlacements = state.convergence.draftPlacements;
            ConvergenceCircuitSystem.SetPersistFailureForValidation(1);
            Check(!ConvergenceCircuitSystem.TryRestoreDraft(state, out _) &&
                  state.convergence.draftPlacements.Count == 1 &&
                  state.convergence.draftPlacements[0].x == 1,
                "Restaurar no revierte el borrador al fallar Persist.", failures);
        }
        finally
        {
            ConvergenceCircuitSystem.ClearPersistFailureForValidation();
            UnityEngine.Object.DestroyImmediate(state.gameObject);
        }
    }

    private static void ValidateDegreeAndTutorials(List<string> failures)
    {
        ConvergenceBoardResolution line = ConvergenceCircuitResolver.ResolveBoard(
            new List<ConvergenceCircuitPlacement>
            {
                P(C2, 0, 1, 0), P(C1, 0, 2, 0)
            });
        ResolvedConvergenceCircuit lineC2 = line.circuits.Find(c => c.circuitId == C2);
        ResolvedConvergenceCircuit lineC1 = line.circuits.Find(c => c.circuitId == C1);
        string c2Tutorial = ConvergencePanelUI.GetCircuitTutorialText(2, line);
        Check(lineC2 != null && lineC2.degree == 2 &&
              lineC1 != null && lineC1.degree == 1 &&
              c2Tutorial.Contains("Núcleo") && c2Tutorial.Contains("C2") &&
              c2Tutorial.Contains("C1"),
            "Degree duplica conexiones o falta la disposición sugerida de C2.", failures);

        ConvergenceBoardResolution branch = ConvergenceCircuitResolver.ResolveBoard(
            new List<ConvergenceCircuitPlacement>
            {
                P(C4, 0, 1, 180), P(C1, -1, 1, 270), P(C2, 1, 1, 90)
            });
        ResolvedConvergenceCircuit branchC4 = branch.circuits.Find(c => c.circuitId == C4);
        string c4Tutorial = ConvergencePanelUI.GetCircuitTutorialText(4, branch);
        Check(branchC4 != null && branchC4.degree == 3 &&
              c4Tutorial.Contains("T") && c4Tutorial.Contains("dos ramas"),
            "Degree no permite comprobar la bifurcación o falta el tutorial C4.", failures);
    }

    private static GameState CreatePendingState(string name, int ordinal)
    {
        GameState state = CreateCompletedDimensionState(name);
        state.convergence.phase = ConvergencePhase.ConfigurationPending;
        state.convergence.completedCycles = ordinal - 1;
        state.convergence.nextAwardOrdinal = ordinal;
        for (int i = 1; i <= ordinal; i++)
        {
            ConvergenceCircuitDefinition definition =
                ConvergenceCircuitCatalog.Definitions.Find(d => d.awardOrdinal == i);
            state.convergence.ownedCircuits.Add(new OwnedConvergenceCircuit
            {
                circuitId = definition.id,
                obtained = true,
                awardOrdinal = i,
                acquiredCycle = i
            });
        }
        state.convergence.draftPlacements = new List<ConvergenceCircuitPlacement>();
        state.convergence.boardPlacements = state.convergence.draftPlacements;
        state.convergence.boardConfigurationLocked = false;
        return state;
    }

    private static GameState CreateCompletedDimensionState(string name)
    {
        GameObject go = new GameObject(name) { hideFlags = HideFlags.HideAndDontSave };
        go.SetActive(false);
        GameState state = go.AddComponent<GameState>();
        state.dimension01Unlocked = true;
        state.dimension02Unlocked = true;
        state.dimension03Unlocked = true;
        state.dimension1GalacticAnchorDiscovered = true;
        state.dimension2 = Dimension2System.CreateInitialState();
        state.dimension2.civilization2.majorPactEstablished = true;
        state.dimension3 = Dimension3System.CreateInitialState();
        state.dimension3.autonomyCoreIntegrated = true;
        state.convergence = ConvergenceSystem.CreateInitialState();
        return state;
    }

    private static List<ConvergenceCircuitPlacement> FinalLayout() =>
        new List<ConvergenceCircuitPlacement>
        {
            P(C4,0,1,180), P(C5,1,1,270), P(C6,2,1,0),
            P(C3,2,0,270), P(C2,1,0,90), P(C1,-1,0,270)
        };

    private static ConvergenceCircuitPlacement P(string id, int x, int y, int r) =>
        new ConvergenceCircuitPlacement
            { circuitId = id, x = x, y = y, rotationDegrees = r };
    private static bool Near(double a, double b) => Math.Abs(a - b) < 0.0001;
    private static void Check(bool condition, string message, List<string> failures)
    {
        if (!condition) failures.Add(message);
    }
}
#endif

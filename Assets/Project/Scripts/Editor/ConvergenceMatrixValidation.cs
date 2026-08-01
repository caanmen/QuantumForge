#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class ConvergenceMatrixValidation
{
    private const string C2 = "convergence_circuit_002_axial_link";
    private const string C3 = "convergence_circuit_003_resonant_elbow";
    private const string C4 = "convergence_circuit_004_triaxial_distributor";
    private const string C5 = "convergence_circuit_005_local_coupler";
    private const string C6 = "convergence_circuit_006_closure_matrix";

    [MenuItem("Tools/Quantum Forge/Convergence/Validate Mandatory Matrix")]
    public static void ValidateMandatoryMatrix()
    {
        var failures = new List<string>();
        ValidateCatalog(failures);
        ValidateBoardGuards(failures);
        ValidateAmplifierTargets(failures);
        ValidateActiveSnapshotDuringDraft(failures);
        ValidateAwardOrderAndEndpoint(failures);
        ValidateRecoveryIdempotence(failures);

        if (failures.Count == 0)
            Debug.Log("[Convergence Matrix] PASS | catálogo | tablero | C5 C1-C4 | orden C1-C6 | draft | recovery | endpoint");
        else
            Debug.LogError("[Convergence Matrix] FAIL\n- " + string.Join("\n- ", failures));
    }

    public static void ValidateMandatoryMatrixBatch() { ValidateMandatoryMatrix(); }

    private static void ValidateCatalog(List<string> failures)
    {
        List<ConvergenceCircuitDefinition> definitions = ConvergenceCircuitCatalog.Definitions;
        Check(definitions.Count == 6 && definitions.Select(d => d.id).Distinct().Count() == 6,
            "El catálogo no contiene exactamente seis IDs únicos.", failures);
        Check(definitions.Select(d => d.awardOrdinal).OrderBy(x => x).SequenceEqual(new[] { 1, 2, 3, 4, 5, 6 }),
            "Los ordinales del catálogo no son exactamente 1-6.", failures);
        Check(definitions.All(d => d != null && !string.IsNullOrWhiteSpace(d.id) &&
              d.basePortMask > 0 && d.basePortMask <= 15 &&
              FiniteInRange(d.baseLEBonus, 0.0, 1.0) &&
              FiniteInRange(d.baseTracesBonus, 0.0, 1.0) &&
              FiniteInRange(d.stabilityBonus, 0.0, 0.5)),
            "El catálogo contiene puertos o efectos inválidos.", failures);
        ConvergenceCircuitDefinition c5 = ConvergenceCircuitCatalog.Get(C5);
        Check(c5 != null && c5.isAmplifier && c5.baseTargetDirectionMask > 0 &&
              c5.baseTargetDirectionMask != c5.basePortMask,
            "C5 no separa el conector de alimentación de su flecha objetivo.", failures);
        Check(ConvergenceCircuitCatalog.Definitions.Find(d => d.awardOrdinal == 7) == null,
            "Existe una definición C7 no autorizada.", failures);
        Check(ConvergenceBalance.GetRequiredStabilityForNextCircuit(0) == 120.0 &&
              ConvergenceBalance.GetRequiredStabilityForNextCircuit(1) == 289.0 &&
              ConvergenceBalance.GetRequiredStabilityForNextCircuit(2) == 530.0 &&
              ConvergenceBalance.GetRequiredStabilityForNextCircuit(3) == 843.0 &&
              ConvergenceBalance.GetRequiredStabilityForNextCircuit(4) == 1229.0 &&
              ConvergenceBalance.GetRequiredStabilityForNextCircuit(5) == 1688.0,
            "Los seis requisitos de estabilidad no coinciden con la especificación.", failures);
    }

    private static void ValidateBoardGuards(List<string> failures)
    {
        int validCells = 0;
        for (int x = -2; x <= 2; x++)
            for (int y = -2; y <= 2; y++)
                if (ConvergenceCircuitCatalog.IsValidBoardCoordinate(x, y)) validCells++;
        Check(validCells == 25 && !ConvergenceCircuitCatalog.IsValidBoardCoordinate(3, 0),
            "La placa no reconoce exactamente el rango 5x5.", failures);

        var invalid = new List<ConvergenceCircuitPlacement>
        {
            P(ConvergenceCircuitCatalog.StartupPulseCircuitId, 3, 0, 0),
            P(C2, 0, 0, 45),
            P(C3, 1, 0, 0),
            P(C4, 1, 0, 0),
            P(C4, -1, 0, 0)
        };
        Check(!ConvergenceCircuitResolver.ResolveBoard(invalid).structurallyValid,
            "El resolvedor acepta coordenada, Núcleo, rotación, colisión o ID duplicado.", failures);
        Check(ConvergenceCircuitResolver.ResolveBoard(new List<ConvergenceCircuitPlacement>
            { P(ConvergenceCircuitCatalog.StartupPulseCircuitId, 0, 1, 180) }).activeCircuitIds.Count == 0,
            "Un conector unilateral energiza un circuito.", failures);

        var stateObject = new GameObject("Convergence Matrix Quarantine") { hideFlags = HideFlags.HideAndDontSave };
        stateObject.SetActive(false);
        try
        {
            GameState state = stateObject.AddComponent<GameState>();
            state.convergence = ConvergenceSystem.CreateInitialState();
            state.convergence.ownedCircuits.Add(new OwnedConvergenceCircuit
                { circuitId = ConvergenceCircuitCatalog.StartupPulseCircuitId, obtained = true, awardOrdinal = 1 });
            state.convergence.committedPlacements.Add(P(ConvergenceCircuitCatalog.StartupPulseCircuitId, 9, 0, 0));
            state.EnsureConvergenceState();
            Check(state.convergence.committedPlacements.Count == 0 && state.convergence.quarantinedPlacements.Count == 1,
                "Una coordenada corrupta no se pone en cuarentena al cargar.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(stateObject); }
    }

    private static void ValidateAmplifierTargets(List<string> failures)
    {
        CheckSnapshot(TargetC1(), 1.35, 1.30, 1.15, "C5 -> C1", failures);
        CheckSnapshot(TargetC2(), 1.30, 1.35, 1.15, "C5 -> C2", failures);
        CheckSnapshot(TargetC3(), 1.30, 1.30, 1.20, "C5 -> C3", failures);
        CheckSnapshot(TargetC4(), 1.35, 1.30, 1.15, "C5 -> C4", failures);
    }

    private static void ValidateActiveSnapshotDuringDraft(List<string> failures)
    {
        var go = new GameObject("Convergence Matrix Draft") { hideFlags = HideFlags.HideAndDontSave };
        go.SetActive(false);
        try
        {
            GameState state = go.AddComponent<GameState>();
            state.convergence = ConvergenceSystem.CreateInitialState();
            state.convergence.phase = ConvergencePhase.ConfigurationPending;
            state.convergence.boardConfigurationLocked = false;
            state.convergence.activeSnapshot = new ConvergenceModifierSnapshot
            {
                baseLEProductionMultiplier = 1.35,
                baseTracesProductionMultiplier = 1.30,
                stabilityGainMultiplier = 1.20
            };
            state.convergence.draftPlacements = new List<ConvergenceCircuitPlacement>();
            Check(Near(ConvergenceCircuitSystem.GetBaseLEProductionMultiplier(state), 1.35) &&
                  Near(ConvergenceCircuitSystem.GetBaseTracesProductionMultiplier(state), 1.30) &&
                  Near(ConvergenceCircuitSystem.GetStabilityGainMultiplier(state), 1.20),
                "Editar un borrador desactiva el snapshot confirmado.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(go); }
    }

    private static void ValidateAwardOrderAndEndpoint(List<string> failures)
    {
        var go = new GameObject("Convergence Matrix Progression") { hideFlags = HideFlags.HideAndDontSave };
        go.SetActive(false);
        try
        {
            GameState state = go.AddComponent<GameState>();
            state.convergence = ConvergenceSystem.CreateInitialState();
            for (int ordinal = 1; ordinal <= 6; ordinal++)
            {
                state.convergence.phase = ConvergencePhase.Ready;
                state.convergence.dimensionalReceiverRebuilt = true;
                foreach (ConvergenceSignalState signal in state.convergence.signals) signal.activated = true;
                state.convergence.currentStability = ConvergenceBalance.GetRequiredStabilityForNextCircuit(ordinal - 1);
                state.convergence.cycleStartedUnix = ConvergenceTelemetrySystem.GetCurrentUnixSeconds() - 100;
                state.convergence.receiverRebuiltUnix = ConvergenceTelemetrySystem.GetCurrentUnixSeconds() - 70;
                state.convergence.synchronizationReadyUnix = ConvergenceTelemetrySystem.GetCurrentUnixSeconds() - 20;

                Check(ConvergenceCircuitSystem.TryStartNormalConvergence(state, out string startReason),
                    "No inicia el premio ordinal " + ordinal + ": " + startReason, failures);
                Check(state.convergence.ownedCircuits.Count == ordinal &&
                      state.convergence.ownedCircuits[ordinal - 1].awardOrdinal == ordinal,
                    "El premio C" + ordinal + " se duplica o sale fuera de orden.", failures);

                state.convergence.draftPlacements.Clear();
                if (ordinal < 6)
                {
                    string id = ConvergenceCircuitCatalog.Definitions.Find(d => d.awardOrdinal == ordinal).id;
                    int rotation = ordinal == 1 || ordinal == 2 || ordinal == 5 ? 0 : 180;
                    Check(ConvergenceCircuitSystem.TryPlaceCircuit(state, id, 0, 1, rotation, out string placeReason),
                        "No coloca C" + ordinal + ": " + placeReason, failures);
                }
                else
                {
                    foreach (ConvergenceCircuitPlacement placement in TargetC4())
                        Check(ConvergenceCircuitSystem.TryPlaceCircuit(state, placement.circuitId,
                              placement.x, placement.y, placement.rotationDegrees, out string placeReason),
                            "No coloca la configuración final C6: " + placeReason, failures);
                }

                Check(ConvergenceCircuitSystem.TryConfirmConfiguration(state, out string confirmReason),
                    "No confirma C" + ordinal + ": " + confirmReason, failures);
                Check(state.convergence.completedCycles == ordinal &&
                      state.convergence.ownedCircuits.Count == ordinal &&
                      state.convergence.pendingTransaction == null &&
                      state.convergence.recordedTransactionIds.Count == ordinal,
                    "La transacción C" + ordinal + " no queda exactamente una vez.", failures);
                Check(!ConvergenceCircuitSystem.TryStartNormalConvergence(state, out _),
                    "Un doble clic reinicia o duplica C" + ordinal + ".", failures);
            }

            Check(state.convergence.normalConvergenceCompleted &&
                  state.convergence.unknownSignalTriggered &&
                  state.convergence.pendingNarrativeEventId == ConvergencePanelUI.UnknownSignalEventId &&
                  state.convergence.nextAwardOrdinal == 7 &&
                  !ConvergenceCircuitSystem.HasNextDesignedCircuit(state),
                "El endpoint C6 no dispara ??? o permite C7.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(go); }
    }

    private static void ValidateRecoveryIdempotence(List<string> failures)
    {
        var go = new GameObject("Convergence Matrix Recovery") { hideFlags = HideFlags.HideAndDontSave };
        go.SetActive(false);
        try
        {
            GameState state = go.AddComponent<GameState>();
            for (int completedSteps = 0; completedSteps <= 5; completedSteps++)
            {
                string transactionId = "matrix_recovery_" + completedSteps;
                var placement = P(ConvergenceCircuitCatalog.StartupPulseCircuitId, 0, 1, 0);
                ConvergenceModifierSnapshot snapshot = ConvergenceSystem.PrepareSnapshot(
                    ConvergenceCircuitResolver.ResolveBoard(
                        new List<ConvergenceCircuitPlacement> { placement }).candidateSnapshot, 1);
                state.convergence = ConvergenceSystem.CreateInitialState();
                state.convergence.ownedCircuits.Add(new OwnedConvergenceCircuit
                    { circuitId = ConvergenceCircuitCatalog.StartupPulseCircuitId, obtained = true, awardOrdinal = 1 });
                if (completedSteps >= 3)
                    state.convergence.recordedTransactionIds.Add(transactionId);
                if (completedSteps >= 5)
                {
                    state.convergence.completedCycles = 1;
                    state.convergence.committedPlacements.Add(placement);
                    state.convergence.activeSnapshot = snapshot;
                }
                state.convergence.pendingTransaction = new ConvergenceTransactionJournal
                {
                    transactionId = transactionId,
                    targetCompletedCycles = 1,
                    awardedCircuitId = ConvergenceCircuitCatalog.StartupPulseCircuitId,
                    stage = completedSteps,
                    candidatePlacements = new List<ConvergenceCircuitPlacement> { placement },
                    candidateSnapshot = snapshot,
                    baseResetApplied = completedSteps >= 1,
                    synchronizationResetApplied = completedSteps >= 2,
                    telemetryRecorded = completedSteps >= 3,
                    finalStatePromoted = completedSteps >= 5
                };
                ConvergenceCircuitSystem.RecoverTransaction(state);
                ConvergenceCircuitSystem.RecoverTransaction(state);
                Check(state.convergence.completedCycles == 1 &&
                      state.convergence.ownedCircuits.Count == 1 &&
                      state.convergence.recordedTransactionIds.Count == 1 &&
                      state.convergence.pendingTransaction == null,
                    "Recovery desde el paso " + completedSteps +
                    " duplica ciclo, premio o telemetría.", failures);
            }
        }
        finally { UnityEngine.Object.DestroyImmediate(go); }
    }

    private static List<ConvergenceCircuitPlacement> TargetC1() => new List<ConvergenceCircuitPlacement>
    {
        P(ConvergenceCircuitCatalog.StartupPulseCircuitId,0,1,0), P(C4,1,0,0), P(C6,2,0,0),
        P(C3,2,1,180), P(C5,1,1,270), P(C2,-1,0,90)
    };
    private static List<ConvergenceCircuitPlacement> TargetC2() => new List<ConvergenceCircuitPlacement>
    {
        P(C2,0,1,0), P(C5,1,1,270), P(C3,2,1,180), P(C4,2,0,0),
        P(C6,1,0,0), P(ConvergenceCircuitCatalog.StartupPulseCircuitId,-1,0,270)
    };
    private static List<ConvergenceCircuitPlacement> TargetC3() => new List<ConvergenceCircuitPlacement>
    {
        P(C3,0,1,90), P(C5,1,1,270), P(C6,2,1,0), P(C4,2,0,0),
        P(C2,1,0,90), P(ConvergenceCircuitCatalog.StartupPulseCircuitId,-1,0,270)
    };
    private static List<ConvergenceCircuitPlacement> TargetC4() => new List<ConvergenceCircuitPlacement>
    {
        P(C4,0,1,180), P(C5,1,1,270), P(C6,2,1,0), P(C3,2,0,270),
        P(C2,1,0,90), P(ConvergenceCircuitCatalog.StartupPulseCircuitId,-1,0,270)
    };

    private static void CheckSnapshot(List<ConvergenceCircuitPlacement> placements,
        double le, double traces, double stability, string label, List<string> failures)
    {
        ConvergenceBoardResolution result = ConvergenceCircuitResolver.ResolveBoard(placements);
        Check(result.structurallyValid && result.activeCircuitIds.Count == 6 &&
              Near(result.candidateSnapshot.baseLEProductionMultiplier, le) &&
              Near(result.candidateSnapshot.baseTracesProductionMultiplier, traces) &&
              Near(result.candidateSnapshot.stabilityGainMultiplier, stability) &&
              result.candidateSnapshot.layoutHash == result.layoutHash,
            label + " no produce el snapshot determinista esperado.", failures);
    }

    private static bool FiniteInRange(double value, double min, double max) =>
        !double.IsNaN(value) && !double.IsInfinity(value) && value >= min && value <= max;
    private static bool Near(double a, double b) => Math.Abs(a - b) < 0.0001;
    private static ConvergenceCircuitPlacement P(string id, int x, int y, int r) =>
        new ConvergenceCircuitPlacement { circuitId = id, x = x, y = y, rotationDegrees = r };
    private static void Check(bool condition, string message, List<string> failures)
    {
        if (!condition) failures.Add(message);
    }
}
#endif

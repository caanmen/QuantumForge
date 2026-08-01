using System.Collections.Generic;

public static class ConvergenceCircuitSystem
{
    public static int ValidationFailPersistOrdinal = -1;
    private static int _validationPersistCallCount;

    public static void SetPersistFailureForValidation(int persistOrdinal)
    {
        ValidationFailPersistOrdinal = persistOrdinal;
        _validationPersistCallCount = 0;
    }

    public static void ClearPersistFailureForValidation()
    {
        ValidationFailPersistOrdinal = -1;
        _validationPersistCallCount = 0;
    }
    public static bool IsConvergenceUnlocked(GameState gameState)
    {
        return DimensionCompletionService.AreAllDimensionsCompleted(gameState);
    }

    public static int GetOwnedCircuitCount(GameState gameState)
    {
        if (gameState == null) return 0;
        gameState.EnsureConvergenceState();
        return gameState.convergence.ownedCircuits.Count;
    }

    public static bool HasNextDesignedCircuit(GameState gameState)
    {
        if (gameState == null) return false;
        gameState.EnsureConvergenceState();
        return !gameState.convergence.normalConvergenceCompleted && gameState.convergence.nextAwardOrdinal >= 1 && gameState.convergence.nextAwardOrdinal <= 6;
    }

    public static bool TryStartNormalConvergence(GameState gameState, out string reason)
    {
        if (gameState == null)
        {
            reason = "No hay estado de juego disponible.";
            return false;
        }

        gameState.EnsureConvergenceState();
        ConvergenceState state = gameState.convergence;
        if (state.phase == ConvergencePhase.ConfigurationPending ||
            state.phase == ConvergencePhase.CircuitAwarded)
        {
            ConvergencePhase previousPhase = state.phase;
            List<ConvergenceCircuitPlacement> previousDraft =
                ConvergenceSystem.ClonePlacements(state.draftPlacements);
            bool previousInvalid = state.invalidDraftDetected;
            bool previousLocked = state.boardConfigurationLocked;
            state.phase = ConvergencePhase.ConfigurationPending;
            BeginDraft(state);
            if (!Persist(out string error))
            {
                RestoreDraftState(state, previousDraft, previousInvalid, previousLocked);
                state.phase = previousPhase;
                reason = "No se pudo guardar la apertura de la placa: " + error;
                return false;
            }
            reason = "La configuración del circuito está pendiente.";
            return true;
        }

        if (state.phase != ConvergencePhase.Ready)
        {
            reason = "La transacción de Convergencia actual no puede iniciarse de nuevo.";
            return false;
        }

        if (!ConvergenceSynchronizationSystem.IsSynchronizationReadyForNextConvergence(
                gameState, GetOwnedCircuitCount(gameState)))
        {
            reason = "Falta completar la sincronización dimensional requerida.";
            return false;
        }

        if (!HasNextDesignedCircuit(gameState))
        {
            reason = "No hay otro circuito diseñado disponible para esta prueba.";
            return false;
        }

        ConvergenceCircuitDefinition award = ConvergenceCircuitCatalog.Definitions.Find(d => d.awardOrdinal == state.nextAwardOrdinal);
        if (award == null) { reason = "No hay otro circuito disponible en este bloque."; return false; }
        int previousOwnedCount = state.ownedCircuits.Count;
        ConvergencePhase awardPreviousPhase = state.phase;
        List<ConvergenceCircuitPlacement> awardPreviousDraft =
            ConvergenceSystem.ClonePlacements(state.draftPlacements);
        bool awardPreviousInvalid = state.invalidDraftDetected;
        bool awardPreviousLocked = state.boardConfigurationLocked;
        long previousConfigurationStartedUnix = state.configurationStartedUnix;
        state.ownedCircuits.Add(new OwnedConvergenceCircuit
        {
            circuitId = award.id, obtained = true, awardOrdinal = award.awardOrdinal,
            acquiredCycle = state.completedCycles + 1
        });
        state.draftPlacements = ConvergenceSystem.ClonePlacements(state.committedPlacements);
        state.invalidDraftDetected = false;
        BeginDraft(state);
        state.phase = ConvergencePhase.ConfigurationPending;
        ConvergenceTelemetrySystem.RecordConfigurationStarted(gameState);
        if (!Persist(out string awardError))
        {
            while (state.ownedCircuits.Count > previousOwnedCount)
                state.ownedCircuits.RemoveAt(state.ownedCircuits.Count - 1);
            state.phase = awardPreviousPhase;
            RestoreDraftState(state, awardPreviousDraft, awardPreviousInvalid,
                awardPreviousLocked);
            state.configurationStartedUnix = previousConfigurationStartedUnix;
            reason = "No se pudo guardar el circuito otorgado: " + awardError;
            return false;
        }
        reason = award.displayName + " obtenido. Configura la placa.";
        return true;
    }

    public static bool TryPlaceCircuit(
        GameState gameState, string circuitId, int x, int y, int rotationDegrees,
        out string reason)
    {
        if (gameState == null)
        {
            reason = "No hay estado de juego disponible.";
            return false;
        }

        gameState.EnsureConvergenceState();
        ConvergenceState state = gameState.convergence;
        if (state.phase != ConvergencePhase.ConfigurationPending ||
            state.boardConfigurationLocked)
        {
            reason = "La placa no está disponible para configuración.";
            return false;
        }
        if (!HasOwnedCircuit(state, circuitId) ||
            !ConvergenceCircuitCatalog.IsValidRotation(rotationDegrees))
        {
            reason = "El circuito o la rotación no son válidos.";
            return false;
        }
        if (!ConvergenceCircuitCatalog.IsValidBoardCoordinate(x, y))
        {
            reason = "La posición está fuera de la placa experimental 5×5.";
            return false;
        }
        if (x == 0 && y == 0)
        {
            reason = "El Núcleo ocupa la posición central de la placa.";
            return false;
        }

        List<ConvergenceCircuitPlacement> previousDraft =
            ConvergenceSystem.ClonePlacements(state.draftPlacements);
        bool previousInvalid = state.invalidDraftDetected;
        bool previousLocked = state.boardConfigurationLocked;
        BeginDraft(state);
        ConvergenceCircuitPlacement current = FindPlacement(state, circuitId, true);
        foreach (ConvergenceCircuitPlacement placement in state.draftPlacements)
        {
            if (placement != current && placement.x == x && placement.y == y)
            {
                reason = "La posición de la placa ya está ocupada.";
                return false;
            }
        }

        if (current == null)
        {
            current = new ConvergenceCircuitPlacement { circuitId = circuitId };
            state.draftPlacements.Add(current);
        }
        current.x = x;
        current.y = y;
        current.rotationDegrees = rotationDegrees;
        state.invalidDraftDetected = !ConvergenceCircuitResolver.ResolveBoard(
            state.draftPlacements).structurallyValid;
        if (!Persist(out string persistError))
        {
            RestoreDraftState(state, previousDraft, previousInvalid, previousLocked);
            reason = "No se pudo guardar la colocación: " + persistError;
            return false;
        }
        reason = "Circuito colocado.";
        return true;
    }

    public static bool TryRemoveCircuit(GameState gameState, string circuitId, out string reason)
    {
        if (gameState == null || gameState.convergence == null ||
            gameState.convergence.phase != ConvergencePhase.ConfigurationPending ||
            gameState.convergence.boardConfigurationLocked)
        { reason = "La placa no está disponible para edición."; return false; }
        ConvergenceState state = gameState.convergence;
        List<ConvergenceCircuitPlacement> previousDraft =
            ConvergenceSystem.ClonePlacements(state.draftPlacements);
        bool previousInvalid = state.invalidDraftDetected;
        bool previousLocked = state.boardConfigurationLocked;
        BeginDraft(state);
        int removed = state.draftPlacements.RemoveAll(p => p != null && p.circuitId == circuitId);
        if (removed == 0) { reason = "El circuito seleccionado no está colocado."; return false; }
        state.invalidDraftDetected =
            !ConvergenceCircuitResolver.ResolveBoard(
                state.draftPlacements).structurallyValid;
        if (!Persist(out string persistError))
        {
            RestoreDraftState(state, previousDraft, previousInvalid, previousLocked);
            reason = "No se pudo guardar la retirada: " + persistError;
            return false;
        }
        reason = "Circuito retirado."; return true;
    }

    public static bool TryRestoreDraft(GameState gameState, out string reason)
    {
        if (gameState == null || gameState.convergence == null ||
            gameState.convergence.phase != ConvergencePhase.ConfigurationPending ||
            gameState.convergence.boardConfigurationLocked)
        { reason = "No hay un borrador que restaurar."; return false; }
        ConvergenceState state = gameState.convergence;
        List<ConvergenceCircuitPlacement> previousDraft =
            ConvergenceSystem.ClonePlacements(state.draftPlacements);
        bool previousInvalid = state.invalidDraftDetected;
        bool previousLocked = state.boardConfigurationLocked;
        state.draftPlacements = ConvergenceSystem.ClonePlacements(state.committedPlacements);
        state.boardPlacements = state.draftPlacements;
        state.invalidDraftDetected = false;
        if (!Persist(out string persistError))
        {
            RestoreDraftState(state, previousDraft, previousInvalid, previousLocked);
            reason = "No se pudo guardar la restauración: " + persistError;
            return false;
        }
        reason = "Configuración activa restaurada en el borrador."; return true;
    }

    public static bool TryConfirmConfiguration(GameState gameState, out string reason)
    {
        if (!TryResolveConfirmableDraft(gameState, out ConvergenceState state,
                out OwnedConvergenceCircuit awardedCircuit,
                out ConvergenceBoardResolution resolution,
                out ConvergenceModifierSnapshot candidateSnapshot, out reason))
            return false;

        string awardedCircuitId = awardedCircuit.circuitId;
        state.pendingTransaction = new ConvergenceTransactionJournal
        {
            transactionId = System.Guid.NewGuid().ToString("N"),
            targetCompletedCycles = state.completedCycles + 1,
            awardedCircuitId = awardedCircuitId,
            stage = (int)ConvergenceTransactionStage.CommitPrepared,
            candidatePlacements = ConvergenceSystem.ClonePlacements(state.draftPlacements),
            candidateSnapshot = ConvergenceSystem.CloneSnapshot(candidateSnapshot)
        };
        state.phase = ConvergencePhase.CommitPrepared;
        state.boardConfigurationLocked = true;
        if (!Persist(out string persistError))
        {
            state.pendingTransaction = null;
            state.phase = ConvergencePhase.ConfigurationPending;
            state.boardConfigurationLocked = false;
            state.boardPlacements = state.draftPlacements;
            reason = "No se pudo guardar la transacción preparada: " + persistError;
            return false;
        }
        if (!CompletePendingTransaction(gameState, out string completionError))
        {
            reason = completionError;
            return false;
        }
        reason = "Configuración estabilizada. Comenzó un nuevo ciclo.";
        return true;
    }

    public static bool CanConfirmConfiguration(GameState gameState, out string reason)
    {
        return TryResolveConfirmableDraft(gameState, out _, out _, out _, out _,
            out reason);
    }

    private static bool TryResolveConfirmableDraft(GameState gameState,
        out ConvergenceState state, out OwnedConvergenceCircuit awardedCircuit,
        out ConvergenceBoardResolution resolution,
        out ConvergenceModifierSnapshot candidateSnapshot, out string reason)
    {
        state = null;
        awardedCircuit = null;
        resolution = null;
        candidateSnapshot = null;
        if (gameState == null)
        { reason = "No hay estado de juego disponible."; return false; }
        gameState.EnsureConvergenceState();
        state = gameState.convergence;
        if (state.phase != ConvergencePhase.ConfigurationPending ||
            state.boardConfigurationLocked)
        { reason = "No hay una configuración pendiente para estabilizar."; return false; }
        BeginDraft(state);
        if (state.draftPlacements == null || state.draftPlacements.Count == 0)
        { reason = "Coloca al menos un circuito antes de estabilizar la placa."; return false; }
        if (state.invalidDraftDetected)
        { reason = "El borrador contenía placements inválidos; restáuralo o corrígelo antes de confirmar."; return false; }
        int awardOrdinal = state.nextAwardOrdinal;
        awardedCircuit = state.ownedCircuits.Find(c => c != null && c.obtained &&
            c.awardOrdinal == awardOrdinal);
        if (awardedCircuit == null)
        { reason = "No se pudo identificar el circuito recién otorgado."; return false; }
        resolution = ConvergenceCircuitResolver.ResolveBoard(state.draftPlacements);
        if (!resolution.structurallyValid)
        { reason = "La placa contiene posiciones, rotaciones, IDs o colisiones inválidas."; return false; }
        if (!resolution.activeCircuitIds.Contains(awardedCircuit.circuitId))
        {
            reason = awardedCircuit.awardOrdinal == 1
                ? "Pulso de Arranque sin energía. Energiza el circuito del borrador antes de estabilizar."
                : "El circuito recién otorgado debe estar energizado antes de estabilizar.";
            return false;
        }
        if (awardedCircuit.circuitId == "convergence_circuit_006_closure_matrix" &&
            resolution.activeCircuitIds.Count != 6)
        { reason = "Matriz de Cierre requiere los seis circuitos energizados."; return false; }
        candidateSnapshot = ConvergenceSystem.PrepareSnapshot(
            resolution.candidateSnapshot, state.completedCycles + 1);
        if (!ConvergenceSystem.IsSnapshotValid(candidateSnapshot) ||
            candidateSnapshot.layoutHash != resolution.layoutHash)
        { reason = "El snapshot calculado para la placa no es válido."; return false; }
        reason = "La configuración puede estabilizarse.";
        return true;
    }

    public static void RecoverTransaction(GameState gameState)
    {
        if (!TryRecoverTransaction(gameState, out string error) &&
            !string.IsNullOrWhiteSpace(error))
            UnityEngine.Debug.LogError("[Convergence Recovery] " + error);
    }

    public static bool TryRecoverTransaction(GameState gameState, out string reason)
    {
        reason = "";
        if (gameState == null) { reason = "No hay estado de juego para recuperar."; return false; }
        gameState.EnsureConvergenceState();
        if (ConvergenceSystem.IsFutureVersion(gameState.convergence))
        { reason = "La partida usa una versión futura de Convergencia."; return false; }
        if (gameState.convergence.pendingTransaction != null)
        {
            return CompletePendingTransaction(gameState, out reason);
        }
        if (gameState.convergence.phase == ConvergencePhase.CircuitAwarded)
        {
            gameState.convergence.phase = ConvergencePhase.ConfigurationPending;
            gameState.convergence.boardConfigurationLocked = false;
        }
        else if (gameState.convergence.phase == ConvergencePhase.CommitPrepared)
        { reason = "La fase de commit no contiene un journal recuperable."; return false; }
        return true;
    }

    public static double GetBaseLEProductionMultiplier(GameState gameState)
    {
        if (gameState == null) return 1.0;
        gameState.EnsureConvergenceState();
        ConvergenceState state = gameState.convergence;
        return state.activeSnapshot != null
            ? state.activeSnapshot.baseLEProductionMultiplier
            : 1.0;
    }

    public static double GetBaseTracesProductionMultiplier(GameState gameState)
    {
        if (gameState == null) return 1.0;
        gameState.EnsureConvergenceState();
        return gameState.convergence.activeSnapshot == null
            ? 1.0
            : gameState.convergence.activeSnapshot.baseTracesProductionMultiplier;
    }

    public static double GetStabilityGainMultiplier(GameState gameState)
    {
        if (gameState == null) return 1.0;
        gameState.EnsureConvergenceState();
        return gameState.convergence.activeSnapshot == null ? 1.0 : gameState.convergence.activeSnapshot.stabilityGainMultiplier;
    }

    public static bool IsCircuitPowered(GameState gameState, string circuitId)
    {
        if (gameState == null || !ConvergenceCircuitCatalog.IsKnownCircuit(circuitId))
            return false;
        gameState.EnsureConvergenceState();
        return ConvergenceCircuitResolver.ResolveBoard(gameState.convergence.committedPlacements)
            .activeCircuitIds.Contains(circuitId);
    }

    private static bool IsStartupPulsePowered(ConvergenceCircuitPlacement placement)
    {
        return (placement.x == 0 && placement.y == 1 &&
                placement.rotationDegrees == 0) ||
            (placement.x == 0 && placement.y == -1 &&
                placement.rotationDegrees == 180) ||
            (placement.x == -1 && placement.y == 0 &&
                placement.rotationDegrees == 90) ||
            (placement.x == 1 && placement.y == 0 &&
                placement.rotationDegrees == 270);
    }

    private static bool HasOwnedCircuit(ConvergenceState state, string circuitId)
    {
        return state.ownedCircuits.Exists(c => c != null && c.obtained &&
            c.circuitId == circuitId);
    }

    private static ConvergenceCircuitPlacement FindPlacement(
        ConvergenceState state, string circuitId, bool draft)
    {
        List<ConvergenceCircuitPlacement> placements = draft ? state.draftPlacements : state.committedPlacements;
        return placements.Find(p => p != null && p.circuitId == circuitId);
    }

    private static void BeginDraft(ConvergenceState state)
    {
        state.draftPlacements ??= ConvergenceSystem.ClonePlacements(state.committedPlacements);
        state.boardPlacements = state.draftPlacements;
        state.boardConfigurationLocked = false;
    }

    private static void RestoreDraftState(ConvergenceState state,
        List<ConvergenceCircuitPlacement> draft, bool invalid, bool locked)
    {
        state.draftPlacements = ConvergenceSystem.ClonePlacements(draft);
        state.invalidDraftDetected = invalid;
        state.boardConfigurationLocked = locked;
        state.boardPlacements = state.phase == ConvergencePhase.ConfigurationPending ||
            state.phase == ConvergencePhase.CircuitAwarded
            ? state.draftPlacements : state.committedPlacements;
    }

    private static bool CompletePendingTransaction(GameState gameState, out string reason)
    {
        reason = "";
        ConvergenceState state = gameState.convergence;
        ConvergenceTransactionJournal journal = state.pendingTransaction;
        if (journal == null) return true;

        ConvergenceBoardResolution candidateResolution =
            ConvergenceCircuitResolver.ResolveBoard(journal.candidatePlacements);
        if (journal.invalidCandidateDetected || !candidateResolution.structurallyValid ||
            !candidateResolution.activeCircuitIds.Contains(journal.awardedCircuitId) ||
            (journal.awardedCircuitId == "convergence_circuit_006_closure_matrix" &&
             candidateResolution.activeCircuitIds.Count != 6))
        {
            reason = "La transacción pendiente contiene una placa inválida y no puede promoverse.";
            return false;
        }
        ConvergenceModifierSnapshot expectedSnapshot = ConvergenceSystem.PrepareSnapshot(
            candidateResolution.candidateSnapshot, journal.targetCompletedCycles);
        if (!ConvergenceSystem.IsSnapshotValid(journal.candidateSnapshot) ||
            journal.candidateSnapshot.layoutHash != expectedSnapshot.layoutHash ||
            journal.candidateSnapshot.schemaVersion != ConvergenceSystem.SnapshotSchemaVersion ||
            journal.candidateSnapshot.catalogRevision != ConvergenceCircuitCatalog.CatalogRevision ||
            journal.candidateSnapshot.cycleId != journal.targetCompletedCycles)
        {
            reason = "La transacción pendiente contiene un snapshot inválido.";
            return false;
        }

        if (!journal.baseResetApplied)
        {
            gameState.ResetGameBaseForConvergence();
            journal.baseResetApplied = true;
            journal.stage = (int)ConvergenceTransactionStage.BaseResetApplied;
            if (!Persist(out string error))
            { reason = "Falló el checkpoint posterior al reset base: " + error; return false; }
        }
        if (!journal.synchronizationResetApplied)
        {
            ConvergenceSynchronizationSystem.ResetCycleSynchronization(gameState);
            journal.synchronizationResetApplied = true;
            journal.stage = (int)ConvergenceTransactionStage.SynchronizationResetApplied;
            if (!Persist(out string error))
            { reason = "Falló el checkpoint posterior al reset de sincronización: " + error; return false; }
        }
        if (!journal.telemetryRecorded)
        {
            ConvergenceTelemetrySystem.RecordConfigurationConfirmed(gameState, journal.transactionId);
            journal.telemetryRecorded = true;
            journal.stage = (int)ConvergenceTransactionStage.TelemetryRecorded;
            if (!Persist(out string error))
            { reason = "Falló el checkpoint posterior a telemetría: " + error; return false; }
        }
        if (!journal.finalStatePromoted)
        {
            state.committedPlacements = ConvergenceSystem.ClonePlacements(journal.candidatePlacements);
            state.draftPlacements = new List<ConvergenceCircuitPlacement>();
            state.boardPlacements = state.committedPlacements;
            state.activeSnapshot = ConvergenceSystem.CloneSnapshot(journal.candidateSnapshot);
            state.modifierSnapshot = state.activeSnapshot;
            state.boardConfigurationLocked = true;
            state.completedCycles = journal.targetCompletedCycles;
            state.nextAwardOrdinal = state.completedCycles + 1;
            if (journal.awardedCircuitId == "convergence_circuit_006_closure_matrix")
            {
                state.normalConvergenceCompleted = true;
                state.unknownSignalTriggered = true;
                state.pendingNarrativeEventId = ConvergenceSystem.UnknownSignalEventId;
                state.phase = ConvergencePhase.Completed;
            }
            else
            {
                state.phase = ConvergencePhase.NewCycleStarted;
            }
            journal.finalStatePromoted = true;
            journal.stage = (int)ConvergenceTransactionStage.FinalStatePromoted;
            if (!Persist(out string error))
            { reason = "Falló el checkpoint al promover el snapshot: " + error; return false; }
        }
        state.pendingTransaction = null;
        if (!Persist(out string finalError))
        {
            state.pendingTransaction = journal;
            reason = "Falló el guardado final de la transacción: " + finalError;
            return false;
        }
        return true;
    }

    private static bool Persist() => Persist(out _);

    private static bool Persist(out string error)
    {
        _validationPersistCallCount++;
        if (ValidationFailPersistOrdinal > 0 &&
            _validationPersistCallCount == ValidationFailPersistOrdinal)
        {
            error = "Fallo inyectado en Persist #" + _validationPersistCallCount + ".";
            return false;
        }
        if (SaveService.I == null) { error = ""; return true; }
        return SaveService.I.TrySave(out error);
    }
}

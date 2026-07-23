using System;
using System.Collections.Generic;


public static class D3AssemblySystem
{
    public static bool IsNormalMkUnlocked(GameState gameState, int mk)
    {
        if (!Dimension3System.CanAccessDimension3(gameState))
            return false;

        return mk >= 1 && mk <= 6 &&
            (mk <= 3
                ? D3ProductionSystem.IsPartVersionUnlocked(gameState, mk)
                : D3ResearchSystem.AreAllCompleted(gameState.dimension3, mk));
    }

    public static bool TryQueueNormalAssembly(
        GameState gameState,
        int mk,
        long quantity,
        out string reason
    )
    {
        reason = "";
        if (!Dimension3System.CanAccessDimension3(gameState))
        {
            reason = "Dimensión 3 está bloqueada.";
            return false;
        }

        Dimension3System.EnsureState(gameState);

        if (!IsNormalMkUnlocked(gameState, mk))
        {
            reason = "Ese MK todavía no está desbloqueado.";
            return false;
        }

        if (quantity <= 0L || quantity > 25L)
        {
            reason = "Cantidad de ensamblaje no permitida.";
            return false;
        }

        if (!D3JobQueueSystem.CanAcceptJob(
                gameState.dimension3,
                Dimension3Catalog.QueueAssembly))
        {
            reason = "La cola de ensamblaje está llena.";
            return false;
        }

        int version = mk;
        if (!D3InventorySystem.HasCompletePartSet(gameState.dimension3, version, quantity))
        {
            reason = "Falta al menos una de las cinco piezas requeridas.";
            return false;
        }

        D3CostTimeDefinition definition = Dimension3Catalog.GetNormalAssemblyDefinition(mk);
        if (definition == null)
        {
            reason = "No existe una definición para ese MK.";
            return false;
        }

        double leCost = D3PowerSystem.GetModifiedCost(
            gameState.dimension3, definition.leCost * quantity);
        double tracesCost = D3PowerSystem.GetModifiedCost(
            gameState.dimension3, definition.tracesCost * quantity);
        double duration = Math.Max(0.1, definition.durationSeconds * quantity);

        if (gameState.LE + 0.000001 < leCost)
        {
            reason = "LE insuficiente.";
            return false;
        }

        if (gameState.Traces + 0.000001 < tracesCost)
        {
            reason = "Trazas insuficientes.";
            return false;
        }

        D3JobState job = new D3JobState
        {
            jobType = Dimension3Catalog.JobAssembly,
            targetId = "automaton_mk" + mk,
            version = version,
            mk = mk,
            quantity = quantity,
            resultTraitId = Dimension3Catalog.TraitNormal,
            baseDurationSeconds = duration,
            remainingSeconds = duration,
            paidLE = leCost,
            paidTraces = tracesCost,
            usesDynamicBankSpeed = true
        };

        for (int i = 0; i < Dimension3Catalog.PartIds.Length; i++)
        {
            string partId = Dimension3Catalog.PartIds[i];
            job.consumedParts.Add(new D3PartAmountState
            {
                partId = partId,
                version = version,
                amount = quantity
            });
        }

        if (!TryReserveAssemblySupports(gameState.dimension3, mk, job, out reason))
            return false;

        gameState.LE -= leCost;
        gameState.Traces -= tracesCost;

        for (int i = 0; i < job.consumedParts.Count; i++)
        {
            D3PartAmountState part = job.consumedParts[i];
            if (!D3InventorySystem.TryConsumeParts(
                    gameState.dimension3,
                    part.partId,
                    part.version,
                    part.amount))
            {
                gameState.LE += Math.Max(0.0, job.paidLE);
                gameState.Traces += Math.Max(0.0, job.paidTraces);
                for (int refundIndex = 0; refundIndex < i; refundIndex++)
                {
                    D3PartAmountState consumed = job.consumedParts[refundIndex];
                    D3InventorySystem.AddParts(
                        gameState.dimension3,
                        consumed.partId,
                        consumed.version,
                        consumed.amount
                    );
                }
                reason = "El inventario cambió durante el ensamblaje.";
                return false;
            }
        }

        if (D3JobQueueSystem.EnqueueCommittedJob(
                gameState.dimension3,
                Dimension3Catalog.QueueAssembly,
                job,
                out reason))
        {
            return true;
        }

        RollbackCommit(gameState, job);
        return false;
    }

    public static bool TryQueueTraitAssembly(
        GameState gameState,
        int mk,
        IList<D3CalibrationReadingState> readings,
        out string reason)
    {
        return TryQueueTraitAssembly(gameState, mk, 1L, readings, out reason);
    }

    public static bool TryQueueTraitAssembly(
        GameState gameState,
        int mk,
        long quantity,
        IList<D3CalibrationReadingState> readings,
        out string reason)
    {
        reason = "";
        if (!Dimension3System.CanAccessDimension3(gameState))
        {
            reason = "Dimensión 3 está bloqueada.";
            return false;
        }
        Dimension3System.EnsureState(gameState);
        if (quantity <= 0L || quantity > 5L ||
            (quantity > 1L && D3FacilitySystem.GetProcessBankLevel(
                gameState.dimension3) < 5))
        {
            reason = "Los lotes con rasgo de 2–5 requieren Banco nivel 5.";
            return false;
        }
        if (!IsNormalMkUnlocked(gameState, mk))
        {
            reason = "Ese MK todavía no está desbloqueado.";
            return false;
        }
        if (!D3JobQueueSystem.CanAcceptJob(
                gameState.dimension3, Dimension3Catalog.QueueAssembly))
        {
            reason = "La cola de ensamblaje está llena.";
            return false;
        }
        if (!D3InventorySystem.HasCompletePartSet(gameState.dimension3, mk, quantity))
        {
            reason = "Falta al menos una de las cinco piezas requeridas.";
            return false;
        }

        D3CalibrationEvaluation evaluation = D3CalibrationSystem.Evaluate(readings);
        if (evaluation.calibratedCount != Dimension3Catalog.PartIds.Length)
        {
            reason = "Deben calibrarse las cinco piezas antes de confirmar.";
            return false;
        }
        D3CostTimeDefinition definition = Dimension3Catalog.GetNormalAssemblyDefinition(mk);
        if (definition == null)
        {
            reason = "No existe una definición para ese MK.";
            return false;
        }

        double leCost = D3PowerSystem.GetModifiedCost(
            gameState.dimension3, definition.leCost * 1.25 * quantity);
        double tracesCost = D3PowerSystem.GetModifiedCost(
            gameState.dimension3, Math.Ceiling(definition.tracesCost * 1.25) * quantity);
        double duration = Math.Max(
            0.1, definition.durationSeconds * 1.20 * quantity);
        if (gameState.LE + 0.000001 < leCost)
        {
            reason = "LE insuficiente.";
            return false;
        }
        if (gameState.Traces + 0.000001 < tracesCost)
        {
            reason = "Trazas insuficientes.";
            return false;
        }

        var job = new D3JobState
        {
            jobType = Dimension3Catalog.JobAssembly,
            targetId = "automaton_mk" + mk,
            version = mk,
            mk = mk,
            quantity = quantity,
            resultTraitId = evaluation.resultTraitId,
            baseDurationSeconds = duration,
            remainingSeconds = duration,
            paidLE = leCost,
            paidTraces = tracesCost,
            usesDynamicBankSpeed = true
        };
        for (int i = 0; i < Dimension3Catalog.PartIds.Length; i++)
        {
            string partId = Dimension3Catalog.PartIds[i];
            job.consumedParts.Add(new D3PartAmountState
                { partId = partId, version = mk, amount = quantity });
            D3CalibrationReadingState reading = FindReading(readings, partId);
            job.calibrationReadings.Add(new D3CalibrationReadingState
                { partId = partId, reading = reading.reading });
        }

        if (!TryReserveAssemblySupports(gameState.dimension3, mk, job, out reason))
            return false;

        gameState.LE -= leCost;
        gameState.Traces -= tracesCost;
        for (int i = 0; i < job.consumedParts.Count; i++)
        {
            D3PartAmountState part = job.consumedParts[i];
            if (!D3InventorySystem.TryConsumeParts(
                    gameState.dimension3, part.partId, part.version, part.amount))
            {
                gameState.LE += leCost;
                gameState.Traces += tracesCost;
                for (int refundIndex = 0; refundIndex < i; refundIndex++)
                {
                    D3PartAmountState consumed = job.consumedParts[refundIndex];
                    D3InventorySystem.AddParts(gameState.dimension3,
                        consumed.partId, consumed.version, consumed.amount);
                }
                reason = "El inventario cambió durante el ensamblaje.";
                return false;
            }
        }
        if (D3JobQueueSystem.EnqueueCommittedJob(
                gameState.dimension3, Dimension3Catalog.QueueAssembly,
                job, out reason)) return true;
        RollbackCommit(gameState, job);
        return false;
    }

    private static D3CalibrationReadingState FindReading(
        IList<D3CalibrationReadingState> readings, string partId)
    {
        for (int i = 0; i < readings.Count; i++)
            if (readings[i] != null && readings[i].partId == partId) return readings[i];
        return null;
    }

    private static void RollbackCommit(GameState gameState, D3JobState job)
    {
        gameState.LE += Math.Max(0.0, job.paidLE);
        gameState.Traces += Math.Max(0.0, job.paidTraces);
        D3InventorySystem.RefundConsumedParts(gameState.dimension3, job);
    }

    private static bool TryReserveAssemblySupports(
        Dimension3State state, int mk, D3JobState job, out string reason)
    {
        reason = "";
        int supportMk = mk == 5 ? 4 : mk == 6 ? 5 : 0;
        if (supportMk == 0) return true;
        long remaining = 5L * Math.Max(1L, job.quantity);
        for (int traitIndex = 0;
             traitIndex < Dimension3Catalog.TraitIds.Length && remaining > 0L;
             traitIndex++)
        {
            string traitId = Dimension3Catalog.TraitIds[traitIndex];
            long available = D3InventorySystem.GetAvailableAutomatonAmount(
                state, supportMk, traitId);
            long take = Math.Min(remaining, available);
            if (take <= 0L) continue;
            job.reservedAutomatons.Add(new D3ReservedAutomatonState
                { mk = supportMk, traitId = traitId, amount = take });
            remaining -= take;
        }
        if (remaining == 0L) return true;
        job.reservedAutomatons.Clear();
        reason = "Faltan cinco MK" + supportMk + " libres como apoyo.";
        return false;
    }
}

using System;


public static class D3ProductionSystem
{
    public static bool IsPartVersionUnlocked(GameState gameState, int version)
    {
        if (version <= 3) return IsPartVersionUnlocked(
            gameState, Dimension3Catalog.PartChassis, version);
        return Dimension3System.CanAccessDimension3(gameState) &&
            D3ResearchSystem.AreAllCompleted(gameState.dimension3, version);
    }

    public static bool IsPartVersionUnlocked(
        GameState gameState, string partId, int version)
    {
        if (!Dimension3System.CanAccessDimension3(gameState))
            return false;

        if (version == 1) return true;
        int bankLevel = D3FacilitySystem.GetProcessBankLevel(gameState.dimension3);
        if (version == 2)
            return bankLevel >= 2 &&
                D3InventorySystem.GetAssemblyCount(gameState.dimension3, 1) >= 25L;
        if (version == 3)
            return bankLevel >= 3 &&
                D3InventorySystem.GetAssemblyCount(gameState.dimension3, 2) >= 25L;
        if (version >= 4 && version <= 6)
            return D3ResearchSystem.IsCompleted(gameState.dimension3, partId, version);
        return false;
    }

    public static bool TryQueuePartProduction(
        GameState gameState,
        string partId,
        int version,
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

        if (!Dimension3Catalog.IsPartId(partId))
        {
            reason = "Tipo de pieza inválido.";
            return false;
        }

        if (!IsPartVersionUnlocked(gameState, partId, version))
        {
            reason = "La versión de la pieza no está desbloqueada.";
            return false;
        }

        if (!Dimension3Catalog.IsSupportedBatchSize(quantity) &&
            !(quantity == 50L &&
              D3FacilitySystem.GetProcessBankLevel(gameState.dimension3) >= 5))
        {
            reason = "Tamaño de lote no permitido.";
            return false;
        }

        if (!D3JobQueueSystem.CanAcceptJob(
                gameState.dimension3,
                Dimension3Catalog.QueuePartProduction))
        {
            reason = "La cola de producción está llena.";
            return false;
        }

        D3CostTimeDefinition definition = Dimension3Catalog.GetPartDefinition(version);
        if (definition == null)
        {
            reason = "No existe una definición para esa pieza.";
            return false;
        }

        double leCost = D3PowerSystem.GetModifiedCost(
            gameState.dimension3, definition.leCost * quantity);
        double tracesCost = D3PowerSystem.GetModifiedCost(
            gameState.dimension3, definition.tracesCost * quantity);
        double duration = Math.Max(0.1, definition.durationSeconds * quantity);

        if (!CanPay(gameState, leCost, tracesCost, out reason))
            return false;

        gameState.LE -= leCost;
        gameState.Traces -= tracesCost;

        D3JobState job = new D3JobState
        {
            jobType = Dimension3Catalog.JobPartProduction,
            targetId = partId,
            version = version,
            quantity = quantity,
            baseDurationSeconds = duration,
            remainingSeconds = duration,
            paidLE = leCost,
            paidTraces = tracesCost,
            usesDynamicBankSpeed = true,
            resultTraitId = Dimension3Catalog.TraitNormal
        };

        if (D3JobQueueSystem.EnqueueCommittedJob(
                gameState.dimension3,
                Dimension3Catalog.QueuePartProduction,
                job,
                out reason))
        {
            return true;
        }

        gameState.LE += leCost;
        gameState.Traces += tracesCost;
        return false;
    }

    private static bool CanPay(
        GameState gameState,
        double leCost,
        double tracesCost,
        out string reason
    )
    {
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

        reason = "";
        return true;
    }
}

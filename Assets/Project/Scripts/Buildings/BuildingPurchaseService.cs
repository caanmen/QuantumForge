public static class BuildingPurchaseService
{
    public static bool TryPurchase(GameState gameState, BuildingState state)
    {
        if (gameState == null || state == null || state.def == null)
            return false;
        if (!BuildingUnlock.IsUnlocked(state.def))
            return false;
        if (state.IsAtMaxLevel())
            return false;

        double effectiveCost = gameState.GetEffectiveBuildingCost(state);
        bool energyGeneratorLevel = state.def.id == "fluctuation_antenna" &&
            state.level > 0;
        double tracesCost = energyGeneratorLevel
            ? gameState.GetTriangleEnergyGeneratorTraceCost()
            : 0.0;
        if (gameState.LE < effectiveCost || gameState.Traces < tracesCost)
            return false;

        gameState.LE -= effectiveCost;
        gameState.Traces -= tracesCost;
        state.OnPurchased();
        if (state.def.id != "fluctuation_antenna")
            D3ConsoleSystem.RecordManualBuildingPurchase(gameState, state.def.id);
        return true;
    }
}

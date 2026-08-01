public static class BuildingPurchaseService
{
    public static bool TryPurchase(GameState gameState, BuildingState state)
    {
        if (gameState == null || state == null || state.def == null)
            return false;
        if (!BuildingUnlock.IsUnlocked(state.def))
            return false;
        if (state.def.id == "fluctuation_antenna" && state.level > 0)
            return false;
        if (state.IsAtMaxLevel())
            return false;

        double effectiveCost = gameState.GetEffectiveBuildingCost(state);
        if (gameState.LE < effectiveCost)
            return false;

        gameState.LE -= effectiveCost;
        state.OnPurchased();
        D3ConsoleSystem.RecordManualBuildingPurchase(gameState, state.def.id);
        return true;
    }
}

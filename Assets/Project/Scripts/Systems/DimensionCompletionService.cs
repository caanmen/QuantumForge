public static class DimensionCompletionService
{
    public static bool IsDimensionCompleted(GameState gameState, int dimensionId)
    {
        if (gameState == null ||
            !gameState.IsDimensionUnlockedAfterPrestige1(dimensionId))
        {
            return false;
        }

        switch (dimensionId)
        {
            case 1:
                return gameState.dimension1GalacticAnchorDiscovered;
            case 2:
                return gameState.dimension2 != null &&
                    gameState.dimension2.civilization2 != null &&
                    gameState.dimension2.civilization2.majorPactEstablished;
            case 3:
                return D3AutonomyCoreSystem.HasIntegrated(gameState.dimension3);
            default:
                return false;
        }
    }

    public static bool AreAllDimensionsCompleted(GameState gameState)
    {
        return IsDimensionCompleted(gameState, 1) &&
            IsDimensionCompleted(gameState, 2) &&
            IsDimensionCompleted(gameState, 3);
    }
}

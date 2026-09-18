using System;


public static class D2PresentationRouter
{
    public const string FirstEntryScreenId = "d2.entry";

    private static readonly string[] ActivePriority =
    {
        PresentationFeatureIds.D2C2Alert,
        PresentationFeatureIds.D2C2Containment,
        PresentationFeatureIds.D2C3EntityResearch,
        PresentationFeatureIds.D2C1Pilgrimages,
        PresentationFeatureIds.D2C1Novitiate,
        PresentationFeatureIds.D2C1Rites,
        PresentationFeatureIds.D2C2Operations,
        PresentationFeatureIds.D2C2Resistance,
        PresentationFeatureIds.D2C3Archaeology,
        PresentationFeatureIds.D2C3Analysis
    };

    public static string ResolveInitialScreen(GameState gameState)
    {
        if (gameState == null || gameState.dimension2 == null)
            return PresentationFeatureIds.D2Map;
        if (!Dimension2System.HasSeenCurrentFirstEntry(gameState))
            return FirstEntryScreenId;
        return ResolveSafeScreen(gameState, gameState.dimension2.presentation.lastScreenId);
    }

    public static string ResolveSafeScreen(GameState gameState, string requestedScreenId)
    {
        if (gameState == null || gameState.dimension2 == null)
            return PresentationFeatureIds.D2Map;

        string active = FindActiveScreen(gameState);
        if (!string.IsNullOrEmpty(active))
            return active;
        if (CanOpen(gameState, requestedScreenId))
            return requestedScreenId;

        string selected = ScreenForTerritory(gameState.dimension2.selectedTerritoryId);
        if (CanOpen(gameState, selected))
            return selected;
        if (CanOpen(gameState, PresentationFeatureIds.D2Map))
            return PresentationFeatureIds.D2Map;
        return PresentationFeatureIds.D2C1Refuge;
    }

    public static bool RememberScreen(GameState gameState, string screenId)
    {
        if (gameState == null || gameState.dimension2 == null ||
            gameState.dimension2.presentation == null ||
            (!CanOpen(gameState, screenId) && screenId != FirstEntryScreenId))
            return false;
        gameState.dimension2.presentation.lastScreenId = screenId;
        return true;
    }

    public static bool CanOpen(GameState gameState, string screenId)
    {
        if (!D2PresentationRules.IsKnownFeatureId(screenId))
            return false;
        return D2PresentationRules.GetFeatureState(gameState, screenId).CanOpen;
    }

    public static string ScreenForTerritory(string territoryId)
    {
        if (territoryId == Dimension2System.Civilization1TerritoryId)
            return PresentationFeatureIds.D2C1Refuge;
        if (territoryId == Dimension2System.Civilization2TerritoryId)
            return PresentationFeatureIds.D2C2Regions;
        if (territoryId == Dimension2System.Civilization3TerritoryId)
            return PresentationFeatureIds.D2C3Archaeology;
        return PresentationFeatureIds.D2Map;
    }

    private static string FindActiveScreen(GameState gameState)
    {
        for (int i = 0; i < ActivePriority.Length; i++)
        {
            FeaturePresentationState state =
                D2PresentationRules.GetFeatureState(gameState, ActivePriority[i]);
            if (state.visualState == FeaturePresentationVisualState.Active)
                return ActivePriority[i];
        }
        return "";
    }
}


public static class D3PresentationRouter
{
    public const string FirstEntryScreenId = "d3.entry";

    private static readonly string[] ActivePriority =
    {
        PresentationFeatureIds.D3Research,
        PresentationFeatureIds.D3AssemblyNormal,
        PresentationFeatureIds.D3ProductionFirstPart,
        PresentationFeatureIds.D3Facilities,
        PresentationFeatureIds.D3Calibration,
        PresentationFeatureIds.D3Automation,
        PresentationFeatureIds.D3QueuesFull,
        PresentationFeatureIds.D3AssignmentBasic
    };

    public static string ResolveInitialScreen(GameState gameState)
    {
        if (gameState == null || gameState.dimension3 == null)
            return PresentationFeatureIds.D3Factory;
        if (!gameState.dimension3.firstEntrySeen &&
            gameState.dimension3.presentation.onboardingStage == 0)
            return FirstEntryScreenId;
        return ResolveSafeScreen(gameState, gameState.dimension3.presentation.lastScreenId);
    }

    public static string ResolveSafeScreen(GameState gameState, string requestedScreenId)
    {
        if (gameState == null || gameState.dimension3 == null)
            return PresentationFeatureIds.D3Factory;

        string active = FindActiveScreen(gameState);
        if (!string.IsNullOrEmpty(active))
            return active;
        if (CanOpen(gameState, requestedScreenId))
            return requestedScreenId;
        if (CanOpen(gameState, PresentationFeatureIds.D3Factory))
            return PresentationFeatureIds.D3Factory;
        if (CanOpen(gameState, PresentationFeatureIds.D3ProductionFirstPart))
            return PresentationFeatureIds.D3ProductionFirstPart;
        return !gameState.dimension3.firstEntrySeen
            ? FirstEntryScreenId
            : PresentationFeatureIds.D3Factory;
    }

    public static bool RememberScreen(GameState gameState, string screenId)
    {
        if (gameState == null || gameState.dimension3 == null ||
            gameState.dimension3.presentation == null ||
            (!CanOpen(gameState, screenId) && screenId != FirstEntryScreenId))
            return false;
        gameState.dimension3.presentation.lastScreenId = screenId;
        return true;
    }

    public static bool CanOpen(GameState gameState, string screenId)
    {
        if (!D3PresentationRules.IsKnownFeatureId(screenId))
            return false;
        return D3PresentationRules.GetFeatureState(gameState, screenId).CanOpen;
    }

    private static string FindActiveScreen(GameState gameState)
    {
        for (int i = 0; i < ActivePriority.Length; i++)
        {
            FeaturePresentationState state =
                D3PresentationRules.GetFeatureState(gameState, ActivePriority[i]);
            if (state.visualState == FeaturePresentationVisualState.Active)
                return ActivePriority[i];
        }
        return "";
    }
}

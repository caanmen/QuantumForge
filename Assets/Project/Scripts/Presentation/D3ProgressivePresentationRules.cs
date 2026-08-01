using System;
using System.Collections.Generic;


public static class D3ProgressivePresentationRules
{
    public static int[] GetUnlockedPartVersions(GameState gameState)
    {
        var result = new List<int>();
        for (int version = 1; version <= 6; version++)
            if (D3ProductionSystem.IsPartVersionUnlocked(gameState, version))
                result.Add(version);
        if (result.Count == 0) result.Add(1);
        return result.ToArray();
    }

    public static int[] GetUnlockedAssemblyMks(GameState gameState)
    {
        var result = new List<int>();
        for (int mk = 1; mk <= 6; mk++)
            if (D3AssemblySystem.IsNormalMkUnlocked(gameState, mk))
                result.Add(mk);
        if (result.Count == 0) result.Add(1);
        return result.ToArray();
    }

    public static bool CanIntroduceCalibration(GameState gameState)
    {
        if (gameState == null || gameState.dimension3 == null) return false;
        Dimension3State state = gameState.dimension3;
        long total = 0L;
        for (int mk = 1; mk <= 6; mk++)
            for (int trait = 0; trait < Dimension3Catalog.TraitIds.Length; trait++)
                total += D3InventorySystem.GetAutomatonAmount(
                    state, mk, Dimension3Catalog.TraitIds[trait]);
        return D3InventorySystem.GetAssemblyCount(state, 1) > 0L || total >= 2L;
    }

    public static bool IsResearchNear(GameState gameState)
    {
        if (gameState == null || gameState.dimension3 == null) return false;
        Dimension3State state = gameState.dimension3;
        return D3FacilitySystem.GetProcessBankLevel(state) >= 3 &&
            D3InventorySystem.GetAssemblyCount(state, 3) > 0L;
    }

    public static bool CanIntroduceResearch(GameState gameState)
    {
        if (gameState == null || gameState.dimension3 == null) return false;
        for (int part = 0; part < Dimension3Catalog.PartIds.Length; part++)
            for (int version = 4; version <= 6; version++)
                if (!D3ResearchSystem.IsCompleted(gameState.dimension3,
                        Dimension3Catalog.PartIds[part], version) &&
                    D3ResearchSystem.ValidatePrerequisites(gameState,
                        Dimension3Catalog.PartIds[part], version, out _))
                    return true;
        return false;
    }

    public static bool IsFacilitiesNear(GameState gameState)
    {
        if (gameState == null || gameState.dimension3 == null) return false;
        return D3InventorySystem.GetAssemblyCount(gameState.dimension3, 1) > 0L;
    }

    public static bool CanIntroduceFacilities(GameState gameState)
    {
        if (gameState == null || gameState.dimension3 == null) return false;
        Dimension3State state = gameState.dimension3;
        if (D3JobQueueSystem.GetJobCount(state,
                Dimension3Catalog.QueueFacility) > 0) return true;
        string[] facilities =
        {
            Dimension3Catalog.FacilityProductionConsole,
            Dimension3Catalog.FacilityDiagnosticBank,
            Dimension3Catalog.FacilityExpeditionPort,
            Dimension3Catalog.FacilityAutomationCore
        };
        for (int i = 0; i < facilities.Length; i++)
        {
            if (D3FacilitySystem.GetFacilityLevel(state, facilities[i]) > 0)
                return true;
            D3FacilityLevelDefinition definition =
                Dimension3Catalog.GetFacilityLevelDefinition(facilities[i], 1);
            if (definition != null &&
                (definition.requiredAssemblyMk <= 0 ||
                 D3InventorySystem.GetAssemblyCount(state,
                     definition.requiredAssemblyMk) >= definition.requiredAssemblyAmount))
                return true;
        }
        return false;
    }

    public static List<string> GetLearnedAutomationActionIds(GameState gameState)
    {
        var result = new List<string>();
        if (gameState == null || gameState.dimension3 == null) return result;
        AddIfLearned(result, gameState, D3AutomationCatalog.ActionPortScan);
        AddIfLearned(result, gameState, D3AutomationCatalog.ActionPortRepeatLast);
        AddIfLearned(result, gameState, D3AutomationCatalog.ActionPortPriorityRoutes);
        AddIfLearned(result, gameState, D3AutomationCatalog.ActionPortSafeRoute);
        AddIfLearned(result, gameState, D3AutomationCatalog.ActionPortExtractor);
        AddIfLearned(result, gameState, D3AutomationCatalog.ActionConsoleBuyHiggs);
        AddIfLearned(result, gameState, D3AutomationCatalog.ActionConsoleBuyTetraquark);
        AddIfLearned(result, gameState, D3AutomationCatalog.ActionConsoleCircuit);
        return result;
    }

    public static bool IsPatternLearned(GameState gameState, string actionId)
    {
        if (gameState == null || gameState.dimension3 == null) return false;
        if (actionId == D3AutomationCatalog.ActionPortScan)
            return gameState.dimension1ManualSimpleScanCompleted;
        if (actionId == D3AutomationCatalog.ActionPortRepeatLast)
            return !string.IsNullOrWhiteSpace(
                    gameState.dimension1LastManualSimpleDestinationId) &&
                gameState.dimension1ManualSimpleDestinationIds.Contains(
                    gameState.dimension1LastManualSimpleDestinationId) &&
                D3AutomationCatalog.IsRepeatableSafeDestination(
                    gameState.dimension1LastManualSimpleDestinationId);
        if (actionId == D3AutomationCatalog.ActionPortPriorityRoutes)
            return gameState.dimension1ManualSimpleDestinationIds.Count > 0;
        if (actionId == D3AutomationCatalog.ActionPortSafeRoute)
        {
            for (int i = 0; i < gameState.dimension1ManualSimpleDestinationIds.Count; i++)
                if (D3AutomationCatalog.IsRepeatableSafeDestination(
                        gameState.dimension1ManualSimpleDestinationIds[i])) return true;
            return false;
        }
        if (actionId == D3AutomationCatalog.ActionPortExtractor)
        {
            string[] planets =
            {
                Dimension1System.Planet01, Dimension1System.Planet02,
                Dimension1System.Planet03, Dimension1System.Planet04,
                Dimension1System.Planet05, Dimension1System.Planet06,
                Dimension1System.Planet07
            };
            for (int i = 0; i < planets.Length; i++)
                if (gameState.dimension1ManualExtractorUpgradePlanetIds.Contains(
                        planets[i])) return true;
            return false;
        }
        if (actionId == D3AutomationCatalog.ActionConsoleBuyHiggs)
            return D3ConsoleSystem.HasManualBuildingAuthorization(
                gameState.dimension3, D3ConsoleSystem.BuildingHiggs);
        if (actionId == D3AutomationCatalog.ActionConsoleBuyTetraquark)
            return D3ConsoleSystem.HasManualBuildingAuthorization(
                gameState.dimension3, D3ConsoleSystem.BuildingTetraquark);
        if (actionId == D3AutomationCatalog.ActionConsoleCircuit)
            for (int circuit = 1; circuit <= 3; circuit++)
                if (D3ConsoleSystem.HasManualCircuitAuthorization(
                        gameState, (TriangleCircuitType)circuit)) return true;
        return false;
    }

    public static bool CanCreateRoutine(GameState gameState, string actionId)
    {
        D3AutomationActionDefinition definition =
            D3AutomationCatalog.GetAction(actionId);
        return definition != null && IsPatternLearned(gameState, actionId) &&
            D3FacilitySystem.GetFacilityLevel(
                gameState.dimension3, definition.facilityId) >=
            definition.requiredFacilityLevel;
    }

    private static void AddIfLearned(
        List<string> result, GameState gameState, string actionId)
    {
        if (IsPatternLearned(gameState, actionId)) result.Add(actionId);
    }
}

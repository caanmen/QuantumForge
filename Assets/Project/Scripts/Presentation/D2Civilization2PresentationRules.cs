using System;
using System.Collections.Generic;


public static class D2Civilization2PresentationRules
{
    public static bool HasRegionalAssignment(D2Civilization2State state)
    {
        if (state == null || state.regions == null) return false;
        for (int i = 0; i < state.regions.Count; i++)
            if (state.regions[i] != null && state.regions[i].membersAssigned > 0L)
                return true;
        return false;
    }

    public static bool HasThreat(D2Civilization2State state)
    {
        if (state == null || state.regions == null) return false;
        for (int i = 0; i < state.regions.Count; i++)
        {
            D2RegionState region = state.regions[i];
            if (region != null && region.threat > 0.0) return true;
        }
        return state.totalReprisals > 0L;
    }

    public static bool CanIntroduceResistance(D2Civilization2State state)
    {
        return state != null && (state.totalReprisals > 0L ||
            state.controlFragments > 0L || HasUpgradeOrPactProgress(state));
    }

    public static bool RequiresContainmentConfirmation(
        D2Civilization2State state)
    {
        return D2Civilization2System.CanAttemptContainment(state) &&
            D2Civilization2System.GetContainmentSuccessProbability(state) < 0.50;
    }

    public static string[] GetVisibleRegionIds(D2Civilization2State state)
    {
        var result = new List<string> { D2Civilization2System.Region1Id };
        D2RegionState region2 = D2Civilization2System.GetRegion(
            state, D2Civilization2System.Region2Id);
        D2RegionState region3 = D2Civilization2System.GetRegion(
            state, D2Civilization2System.Region3Id);
        double dominance = D2Civilization2System.GetTotalDominance(state);
        if ((region2 != null && region2.unlocked) ||
            dominance <= D2Civilization2System.Region2UnlockDominance + 10.0)
            result.Add(D2Civilization2System.Region2Id);
        if (region2 != null && region2.unlocked &&
            ((region3 != null && region3.unlocked) ||
             dominance <= D2Civilization2System.Region3UnlockDominance + 10.0))
            result.Add(D2Civilization2System.Region3Id);
        return result.ToArray();
    }

    public static string[] GetVisibleOperationIds(D2Civilization2State state)
    {
        var result = new List<string>
            { D2Civilization2System.RescueOperationId };
        if (HasOperationProgress(state,
                D2Civilization2System.RescueOperationId))
            result.Add(D2Civilization2System.ProtectionOperationId);
        if (HasThreat(state) || HasAnyOperationProgress(state))
            Add(result, D2Civilization2System.EspionageOperationId);
        if (state != null && state.totalReprisals > 0L)
            Add(result, D2Civilization2System.SabotageOperationId);
        AddActiveOperations(state, result);
        return result.ToArray();
    }

    public static string[] GetVisibleUpgradeIds(D2Civilization2State state)
    {
        var result = new List<string>();
        if (state == null) return result.ToArray();
        for (int i = 0; i < D2Civilization2System.UpgradeIds.Length; i++)
        {
            string id = D2Civilization2System.UpgradeIds[i];
            int level = D2Civilization2System.GetUpgradeLevel(state, id);
            if (level > 0 || state.controlFragments >=
                    D2Civilization2System.GetUpgradeCost(level + 1))
                result.Add(id);
        }
        if (result.Count == 0 && D2Civilization2System.UpgradeIds.Length > 0)
            result.Add(D2Civilization2System.UpgradeIds[0]);
        return result.ToArray();
    }

    public static string[] GetVisibleResistancePactIds(D2Civilization2State state)
    {
        if (HasResistancePactProgress(state))
            return D2Civilization2System.ResistancePactIds;
        return D2Civilization2System.ResistancePactIds.Length == 0
            ? new string[0]
            : new[] { D2Civilization2System.ResistancePactIds[0] };
    }

    private static bool HasAnyOperationProgress(D2Civilization2State state)
    {
        for (int i = 0; i < D2Civilization2System.OperationIds.Length; i++)
            if (HasOperationProgress(state,
                    D2Civilization2System.OperationIds[i])) return true;
        return false;
    }

    private static bool HasOperationProgress(
        D2Civilization2State state, string operationId)
    {
        if (state == null || state.regions == null) return false;
        for (int i = 0; i < state.regions.Count; i++)
        {
            D2OperationState operation = D2Civilization2System.GetOperation(
                state.regions[i], operationId);
            if (operation != null && operation.membersAssigned > 0L) return true;
        }
        return false;
    }

    private static void AddActiveOperations(
        D2Civilization2State state, List<string> result)
    {
        if (state == null || state.regions == null) return;
        for (int i = 0; i < state.regions.Count; i++)
        {
            D2RegionState region = state.regions[i];
            if (region == null || region.operations == null) continue;
            for (int j = 0; j < region.operations.Count; j++)
            {
                D2OperationState operation = region.operations[j];
                if (operation != null &&
                    D2Civilization2System.IsOperationActive(operation))
                    Add(result, operation.operationId);
            }
        }
    }

    private static bool HasUpgradeOrPactProgress(D2Civilization2State state)
    {
        if (state == null) return false;
        if (state.resistanceUpgrades != null)
            for (int i = 0; i < state.resistanceUpgrades.Count; i++)
                if (state.resistanceUpgrades[i] != null &&
                    state.resistanceUpgrades[i].level > 0) return true;
        return HasResistancePactProgress(state);
    }

    private static bool HasResistancePactProgress(D2Civilization2State state)
    {
        if (state == null || state.resistancePacts == null) return false;
        for (int i = 0; i < state.resistancePacts.Count; i++)
        {
            D2ResistancePactState pact = state.resistancePacts[i];
            if (pact != null && (pact.active || pact.membersAssigned > 0L))
                return true;
        }
        return false;
    }

    private static void Add(List<string> values, string id)
    {
        if (!values.Contains(id)) values.Add(id);
    }
}

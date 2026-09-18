using System;
using System.Collections.Generic;


public static class D2Civilization1PresentationRules
{
    public static bool HasLearnedRefuge(D2Civilization1State state)
    {
        return state != null && (state.followersAssignedToRefuge > 0L ||
            state.totalFollowersReceived > 5L);
    }

    public static bool CanIntroduceAltars(D2Civilization1State state)
    {
        return HasLearnedRefuge(state) || HasAnyAltarProgress(state);
    }

    public static bool CanIntroducePilgrimages(D2Civilization1State state)
    {
        if (state == null) return false;
        if (state.activePilgrimage != null && state.activePilgrimage.active)
            return true;
        D2AltarState wax = D2AltarSystem.GetAltar(
            state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(
            state, D2AltarSystem.RitualBreadAltarId);
        bool bothProduced = wax != null && bread != null &&
            wax.totalOfferingProduced > 0.0 && bread.totalOfferingProduced > 0.0;
        bool firstCostReady = wax != null && bread != null &&
            wax.offeringAmount >= D2PilgrimageSystem.GetEffectiveWaxCost(
                state, D2PilgrimageSystem.ShortId) &&
            bread.offeringAmount >= D2PilgrimageSystem.GetEffectiveBreadCost(
                state, D2PilgrimageSystem.ShortId);
        return bothProduced || firstCostReady || state.totalPilgrimagesCompleted > 0L;
    }

    public static bool CanIntroduceNovitiate(GameState gameState)
    {
        if (gameState == null || gameState.dimension2 == null) return false;
        D2Civilization1State state = gameState.dimension2.civilization1;
        if (state == null) return false;
        D2AltarState wax = D2AltarSystem.GetAltar(
            state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(
            state, D2AltarSystem.RitualBreadAltarId);
        bool trainingReachable = state.activeNovitiateTraining != null &&
            !state.activeNovitiateTraining.active &&
            !D2CivilizationPactSystem.IsPactActive(
                state, D2CivilizationPactSystem.InnerDoorId) &&
            state.followersAvailable >= D2NovitiateSystem.GetFollowerCost(
                state.novitiateLevel) + state.novitiateSupportFollowersSelected &&
            wax != null && bread != null &&
            wax.offeringAmount >= D2NovitiateSystem.GetOfferingCost(
                state.novitiateLevel) &&
            bread.offeringAmount >= D2NovitiateSystem.GetOfferingCost(
                state.novitiateLevel);
        return state.mediumPilgrimagesCompleted > 0L ||
            state.totalAcolytesCreated > 0L || state.novitiateBatchesCompleted > 0L ||
            (state.activeNovitiateTraining != null &&
             state.activeNovitiateTraining.active) ||
            trainingReachable;
    }

    public static bool CanIntroduceRites(D2Civilization1State state)
    {
        return state != null && (state.totalAcolytesCreated > 0L ||
            D2RiteSystem.GetActiveRiteCount(state) > 0);
    }

    public static bool IsPactsNear(D2Civilization1State state)
    {
        return state != null && (state.trust >= 150.0 ||
            (state.novitiateLevel >= 2 && state.trust >= 100.0));
    }

    public static bool CanIntroducePacts(D2Civilization1State state)
    {
        return state != null && (D2CivilizationPactSystem.ArePactsUnlocked(state) ||
            HasPactProgress(state));
    }

    public static string[] GetVisibleAltarIds(D2Civilization1State state)
    {
        // El catálogo canónico conserva la cuadrícula 3+2: los cinco espacios se
        // muestran desde el inicio y cada Altar decide por separado si es interactivo.
        return (string[])D2AltarSystem.AltarIds.Clone();
    }

    public static string[] GetVisibleRiteIds(D2Civilization1State state)
    {
        if (D2RiteSystem.AreRitesUnlocked(state))
            return (string[])D2RiteSystem.RiteIds.Clone();
        int baseCount = Math.Min(2, D2RiteSystem.RiteIds.Length);
        var ids = new List<string>(baseCount);
        for (int i = 0; i < baseCount; i++) ids.Add(D2RiteSystem.RiteIds[i]);
        return ids.ToArray();
    }

    public static string[] GetVisiblePactIds(D2Civilization1State state)
    {
        if (D2CivilizationPactSystem.PactIds.Length == 0) return new string[0];
        if (HasPactProgress(state)) return D2CivilizationPactSystem.PactIds;
        return new[] { D2CivilizationPactSystem.PactIds[0] };
    }

    private static bool HasAnyAltarProgress(D2Civilization1State state)
    {
        if (state == null || state.altars == null) return false;
        for (int i = 0; i < state.altars.Count; i++)
        {
            D2AltarState altar = state.altars[i];
            if (altar != null && (altar.followersAssigned > 0L ||
                altar.offeringAmount > 0.0 || altar.totalOfferingProduced > 0.0))
                return true;
        }
        return false;
    }

    private static bool HasPactProgress(D2Civilization1State state)
    {
        if (state == null || state.civilizationPacts == null) return false;
        for (int i = 0; i < state.civilizationPacts.Count; i++)
        {
            D2CivilizationPactState pact = state.civilizationPacts[i];
            if (pact != null && (pact.active || pact.suspended)) return true;
        }
        return false;
    }
}

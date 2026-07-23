using System;
using System.Collections.Generic;


public static class D2AltarSystem
{
    public const string WaxAltarId = "d2_c1_altar_wax";
    public const string RitualBreadAltarId = "d2_c1_altar_ritual_bread";
    public const string IncenseAltarId = "d2_c1_altar_incense";
    public const string SacredClothAltarId = "d2_c1_altar_sacred_cloth";
    public const string CarvedStoneAltarId = "d2_c1_altar_carved_stone";

    public const double BaseOfferingPerSecond = 0.05;
    public const double AssignedFollowerEfficiencyFactor = 0.35;

    public static readonly string[] AltarIds =
    {
        WaxAltarId,
        RitualBreadAltarId,
        IncenseAltarId,
        SacredClothAltarId,
        CarvedStoneAltarId
    };

    public static void EnsureState(D2Civilization1State civilization1)
    {
        if (civilization1 == null)
            return;

        if (civilization1.altars == null)
            civilization1.altars = new List<D2AltarState>();

        for (int i = civilization1.altars.Count - 1; i >= 0; i--)
        {
            D2AltarState altar = civilization1.altars[i];
            if (altar == null || !IsAltarId(altar.altarId) || HasEarlierDuplicate(civilization1, i))
                civilization1.altars.RemoveAt(i);
        }

        foreach (string altarId in AltarIds)
        {
            D2AltarState altar = FindAltar(civilization1, altarId);
            if (altar == null)
            {
                altar = new D2AltarState
                {
                    altarId = altarId,
                    unlocked = IsInitialAltar(altarId)
                };
                civilization1.altars.Add(altar);
            }

            if (IsInitialAltar(altarId) || IsAdvancedAltarUnlocked(civilization1, altarId))
                altar.unlocked = true;

            altar.offeringAmount = SanitizeNonNegative(altar.offeringAmount);
            altar.totalOfferingProduced = Math.Max(
                altar.offeringAmount,
                SanitizeNonNegative(altar.totalOfferingProduced)
            );
            altar.followersAssigned = Math.Max(0L, altar.followersAssigned);
        }
    }

    public static void Tick(D2Civilization1State civilization1, double seconds)
    {
        Tick(civilization1, seconds, 1.0);
    }

    public static void Tick(
        D2Civilization1State civilization1,
        double seconds,
        double externalMultiplier
    )
    {
        if (civilization1 == null || seconds <= 0.0 ||
            double.IsNaN(seconds) || double.IsInfinity(seconds))
        {
            return;
        }

        EnsureState(civilization1);
        foreach (D2AltarState altar in civilization1.altars)
        {
            if (altar == null || !altar.unlocked)
                continue;

            AddOffering(altar, GetOfferingPerSecond(civilization1, altar) *
                Math.Max(1.0, externalMultiplier) * seconds);
        }
    }

    public static D2AltarState GetAltar(D2Civilization1State civilization1, string altarId)
    {
        if (civilization1 == null || !IsAltarId(altarId))
            return null;

        EnsureState(civilization1);
        return FindAltar(civilization1, altarId);
    }

    public static bool IsAltarId(string altarId)
    {
        return altarId == WaxAltarId || altarId == RitualBreadAltarId ||
               altarId == IncenseAltarId || altarId == SacredClothAltarId ||
               altarId == CarvedStoneAltarId;
    }

    public static bool IsInitialAltar(string altarId)
    {
        return altarId == WaxAltarId || altarId == RitualBreadAltarId;
    }

    public static bool IsAdvancedAltarUnlocked(
        D2Civilization1State state,
        string altarId
    )
    {
        if (state == null)
            return false;

        switch (altarId)
        {
            case IncenseAltarId:
                return D2RiteSystem.AreRitesUnlocked(state);
            case SacredClothAltarId:
                return D2CivilizationPactSystem.ArePactsUnlocked(state);
            case CarvedStoneAltarId:
                return state.entityContactAvailable ||
                    state.trust >= D2VeiledThresholdSystem.UnlockTrustRequired;
            default:
                return false;
        }
    }

    public static string GetAltarName(string altarId)
    {
        switch (altarId)
        {
            case WaxAltarId: return "Altar de Cera";
            case RitualBreadAltarId: return "Altar de Pan Ritual";
            case IncenseAltarId: return "Altar de Incienso";
            case SacredClothAltarId: return "Altar de Tela Sagrada";
            case CarvedStoneAltarId: return "Altar de Piedra Tallada";
            default: return "Altar desconocido";
        }
    }

    public static string GetOfferingName(string altarId)
    {
        switch (altarId)
        {
            case WaxAltarId: return "Cera";
            case RitualBreadAltarId: return "Pan ritual";
            case IncenseAltarId: return "Incienso";
            case SacredClothAltarId: return "Tela sagrada";
            case CarvedStoneAltarId: return "Piedra tallada";
            default: return "Ofrenda desconocida";
        }
    }

    public static double GetOfferingPerSecond(D2AltarState altar)
    {
        return altar == null || !altar.unlocked
            ? 0.0
            : BaseOfferingPerSecond * GetAssignedFollowerMultiplier(altar);
    }

    public static double GetOfferingPerSecond(
        D2Civilization1State state,
        D2AltarState altar
    )
    {
        return GetOfferingPerSecond(altar) * (1.0 +
            D2RiteSystem.GetBonusFraction(state, D2RiteSystem.OfferingId) +
            D2BondSystem.GetSacredCraftBonus(state));
    }

    public static double GetAssignedFollowerMultiplier(D2AltarState altar)
    {
        if (altar == null)
            return 1.0;

        return 1.0 +
            (Math.Sqrt(Math.Max(0L, altar.followersAssigned)) *
             AssignedFollowerEfficiencyFactor);
    }

    public static long GetTotalFollowersAssignedToAltars(D2Civilization1State civilization1)
    {
        if (civilization1 == null)
            return 0L;

        EnsureState(civilization1);
        long total = 0L;
        foreach (D2AltarState altar in civilization1.altars)
        {
            if (altar != null)
                total = SaturatingAdd(total, altar.followersAssigned);
        }

        return total;
    }

    public static bool TryAssignFollowers(GameState gameState, string altarId, long amount)
    {
        if (!TryGetActiveAltar(gameState, altarId, out D2Civilization1State civilization1, out D2AltarState altar) ||
            amount <= 0L)
        {
            return false;
        }

        long assigned = Math.Min(amount, civilization1.followersAvailable);
        if (assigned <= 0L)
            return false;

        civilization1.followersAvailable -= assigned;
        altar.followersAssigned = SaturatingAdd(altar.followersAssigned, assigned);
        return true;
    }

    public static bool TryAssignAllFollowers(GameState gameState, string altarId)
    {
        if (!TryGetActiveAltar(gameState, altarId, out D2Civilization1State civilization1, out _))
            return false;

        return TryAssignFollowers(gameState, altarId, civilization1.followersAvailable);
    }

    public static bool TryReleaseFollowers(GameState gameState, string altarId, long amount)
    {
        if (!TryGetActiveAltar(gameState, altarId, out D2Civilization1State civilization1, out D2AltarState altar) ||
            amount <= 0L)
        {
            return false;
        }

        long released = Math.Min(amount, altar.followersAssigned);
        if (released <= 0L)
            return false;

        altar.followersAssigned -= released;
        civilization1.followersAvailable = SaturatingAdd(civilization1.followersAvailable, released);
        return true;
    }

    public static bool TryReleaseAllFollowers(GameState gameState, string altarId)
    {
        if (!TryGetActiveAltar(gameState, altarId, out _, out D2AltarState altar))
            return false;

        return TryReleaseFollowers(gameState, altarId, altar.followersAssigned);
    }

    public static bool ValidateState(D2Civilization1State civilization1, out string result)
    {
        if (civilization1 == null)
        {
            result = "Estado de Civilización 1 es null.";
            return false;
        }

        EnsureState(civilization1);
        if (civilization1.altars.Count != AltarIds.Length)
        {
            result = "El catálogo no contiene exactamente cinco Altares.";
            return false;
        }

        foreach (string altarId in AltarIds)
        {
            D2AltarState altar = FindAltar(civilization1, altarId);
            if (altar == null || (IsInitialAltar(altarId) && !altar.unlocked))
            {
                result = "Catálogo o desbloqueo inicial de Altares inválido.";
                return false;
            }

            if (altar.offeringAmount < 0.0 || altar.followersAssigned < 0L)
            {
                result = "Un Altar contiene valores negativos.";
                return false;
            }
        }

        result = "Estado de Altares y Ofrendas válido.";
        return true;
    }

    private static bool TryGetActiveAltar(
        GameState gameState,
        string altarId,
        out D2Civilization1State civilization1,
        out D2AltarState altar
    )
    {
        civilization1 = null;
        altar = null;
        if (!Dimension2System.CanAccessDimension2(gameState) || !IsAltarId(altarId))
            return false;

        Dimension2System.EnsureState(gameState);
        civilization1 = gameState.dimension2.civilization1;
        altar = GetAltar(civilization1, altarId);
        return altar != null && altar.unlocked;
    }

    private static D2AltarState FindAltar(D2Civilization1State civilization1, string altarId)
    {
        if (civilization1?.altars == null)
            return null;

        foreach (D2AltarState altar in civilization1.altars)
        {
            if (altar != null && altar.altarId == altarId)
                return altar;
        }

        return null;
    }

    private static bool HasEarlierDuplicate(D2Civilization1State civilization1, int index)
    {
        D2AltarState altar = civilization1.altars[index];
        for (int i = 0; i < index; i++)
        {
            D2AltarState previous = civilization1.altars[i];
            if (previous != null && previous.altarId == altar.altarId)
                return true;
        }

        return false;
    }

    private static void AddOffering(D2AltarState altar, double amount)
    {
        if (altar == null || amount <= 0.0 || double.IsNaN(amount))
            return;

        altar.offeringAmount = SaturatingAdd(altar.offeringAmount, amount);
        altar.totalOfferingProduced = SaturatingAdd(altar.totalOfferingProduced, amount);
    }

    private static double SanitizeNonNegative(double value)
    {
        return double.IsNaN(value) || value < 0.0 ? 0.0 : value;
    }

    private static double SaturatingAdd(double left, double right)
    {
        if (double.IsInfinity(right) || left >= double.MaxValue - right)
            return double.MaxValue;

        return left + right;
    }

    private static long SaturatingAdd(long left, long right)
    {
        left = Math.Max(0L, left);
        if (right <= 0L)
            return left;

        return left > long.MaxValue - right ? long.MaxValue : left + right;
    }
}

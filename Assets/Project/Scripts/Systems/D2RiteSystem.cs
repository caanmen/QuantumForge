using System;
using System.Collections.Generic;


public static class D2RiteSystem
{
    public const string WelcomeId = "d2_c1_rite_welcome";
    public const string OfferingId = "d2_c1_rite_offering";
    public const string PathId = "d2_c1_rite_path";
    public const string NovitiateId = "d2_c1_rite_novitiate";
    public const string RespectId = "d2_c1_rite_respect";

    public const int InitialActiveSlots = 2;
    public const int AdvancedActiveSlots = 3;
    public const double ThirdSlotTrustRequired = 250.0;
    public const int ThirdSlotNovitiateLevelRequired = 3;
    public const double ThirdSlotWaxCost = 150.0;
    public const double ThirdSlotBreadCost = 150.0;
    public const long ThirdSlotAcolyteCost = 5L;

    public const double FollowerPowerPercent = 2.5;
    public const double AcolytePowerPercent = 6.0;

    public static readonly string[] RiteIds =
    {
        WelcomeId,
        OfferingId,
        PathId,
        NovitiateId,
        RespectId
    };

    public static void EnsureState(D2Civilization1State state)
    {
        if (state == null)
            return;

        if (state.rites == null)
            state.rites = new List<D2RiteState>();

        var sanitized = new List<D2RiteState>(RiteIds.Length);
        foreach (string riteId in RiteIds)
        {
            D2RiteState rite = FindRite(state.rites, riteId) ?? new D2RiteState();
            rite.riteId = riteId;
            rite.followersAssigned = Math.Max(0L, rite.followersAssigned);
            rite.acolytesAssigned = Math.Max(0L, rite.acolytesAssigned);
            sanitized.Add(rite);
        }

        state.rites = sanitized;

        int slots = GetActiveSlotLimit(state);
        int active = 0;
        foreach (D2RiteState rite in state.rites)
        {
            if (!IsActive(rite))
                continue;

            active++;
            if (active <= slots)
                continue;

            state.followersAvailable = SaturatingAdd(
                state.followersAvailable,
                rite.followersAssigned
            );
            state.acolytesAvailable = SaturatingAdd(
                state.acolytesAvailable,
                rite.acolytesAssigned
            );
            rite.followersAssigned = 0L;
            rite.acolytesAssigned = 0L;
        }
    }

    public static bool AreRitesUnlocked(D2Civilization1State state)
    {
        return state != null && state.totalAcolytesCreated > 0L;
    }

    public static int GetActiveSlotLimit(D2Civilization1State state)
    {
        return state != null && state.thirdRiteSlotUnlocked
            ? AdvancedActiveSlots
            : InitialActiveSlots;
    }

    public static int GetActiveRiteCount(D2Civilization1State state)
    {
        if (state?.rites == null)
            return 0;

        int count = 0;
        foreach (D2RiteState rite in state.rites)
        {
            if (IsActive(rite))
                count++;
        }
        return count;
    }

    public static D2RiteState GetRite(D2Civilization1State state, string riteId)
    {
        if (state == null || !IsRiteId(riteId))
            return null;

        EnsureState(state);
        return FindRite(state.rites, riteId);
    }

    public static bool TryAssignFollowers(GameState gameState, string riteId, long amount)
    {
        if (!TryGetState(gameState, riteId, out D2Civilization1State state,
            out D2RiteState rite) || amount <= 0L || state.followersAvailable <= 0L)
        {
            return false;
        }

        if (!IsActive(rite) && GetActiveRiteCount(state) >= GetActiveSlotLimit(state))
            return false;

        long assigned = Math.Min(amount, state.followersAvailable);
        state.followersAvailable -= assigned;
        rite.followersAssigned = SaturatingAdd(rite.followersAssigned, assigned);
        return assigned > 0L;
    }

    public static bool TryAssignAcolytes(GameState gameState, string riteId, long amount)
    {
        if (!TryGetState(gameState, riteId, out D2Civilization1State state,
            out D2RiteState rite) || amount <= 0L || state.acolytesAvailable <= 0L)
        {
            return false;
        }

        if (!IsActive(rite) && GetActiveRiteCount(state) >= GetActiveSlotLimit(state))
            return false;

        long assigned = Math.Min(amount, state.acolytesAvailable);
        state.acolytesAvailable -= assigned;
        rite.acolytesAssigned = SaturatingAdd(rite.acolytesAssigned, assigned);
        return assigned > 0L;
    }

    public static bool TryReleaseFollowers(GameState gameState, string riteId, long amount)
    {
        if (!TryGetState(gameState, riteId, out D2Civilization1State state,
            out D2RiteState rite) || amount <= 0L || rite.followersAssigned <= 0L)
        {
            return false;
        }

        long released = Math.Min(amount, rite.followersAssigned);
        rite.followersAssigned -= released;
        state.followersAvailable = SaturatingAdd(state.followersAvailable, released);
        return released > 0L;
    }

    public static bool TryReleaseAcolytes(GameState gameState, string riteId, long amount)
    {
        if (!TryGetState(gameState, riteId, out D2Civilization1State state,
            out D2RiteState rite) || amount <= 0L || rite.acolytesAssigned <= 0L)
        {
            return false;
        }

        long released = Math.Min(amount, rite.acolytesAssigned);
        rite.acolytesAssigned -= released;
        state.acolytesAvailable = SaturatingAdd(state.acolytesAvailable, released);
        return released > 0L;
    }

    public static bool TryReleaseAll(GameState gameState, string riteId)
    {
        if (!TryGetState(gameState, riteId, out D2Civilization1State state,
            out D2RiteState rite) || !IsActive(rite))
        {
            return false;
        }

        state.followersAvailable = SaturatingAdd(
            state.followersAvailable,
            rite.followersAssigned
        );
        state.acolytesAvailable = SaturatingAdd(
            state.acolytesAvailable,
            rite.acolytesAssigned
        );
        rite.followersAssigned = 0L;
        rite.acolytesAssigned = 0L;
        return true;
    }

    public static bool CanUnlockThirdSlot(GameState gameState)
    {
        if (!Dimension2System.CanAccessDimension2(gameState))
            return false;

        Dimension2System.EnsureState(gameState);
        D2Civilization1State state = gameState.dimension2.civilization1;
        if (state.thirdRiteSlotUnlocked || !AreRitesUnlocked(state) ||
            state.trust < ThirdSlotTrustRequired ||
            state.novitiateLevel < ThirdSlotNovitiateLevelRequired ||
            state.acolytesAvailable < ThirdSlotAcolyteCost)
        {
            return false;
        }

        D2AltarState wax = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(
            state,
            D2AltarSystem.RitualBreadAltarId
        );
        return wax != null && wax.offeringAmount >= ThirdSlotWaxCost &&
            bread != null && bread.offeringAmount >= ThirdSlotBreadCost;
    }

    public static bool TryUnlockThirdSlot(GameState gameState)
    {
        if (!CanUnlockThirdSlot(gameState))
            return false;

        D2Civilization1State state = gameState.dimension2.civilization1;
        state.acolytesAvailable -= ThirdSlotAcolyteCost;
        D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId).offeringAmount -=
            ThirdSlotWaxCost;
        D2AltarSystem.GetAltar(state, D2AltarSystem.RitualBreadAltarId).offeringAmount -=
            ThirdSlotBreadCost;
        state.thirdRiteSlotUnlocked = true;
        return true;
    }

    public static double GetBonusFraction(D2Civilization1State state, string riteId)
    {
        D2RiteState rite = GetRite(state, riteId);
        if (!AreRitesUnlocked(state) || !IsActive(rite))
            return 0.0;

        double percent =
            (Math.Sqrt(rite.followersAssigned) * FollowerPowerPercent) +
            (Math.Sqrt(rite.acolytesAssigned) * AcolytePowerPercent);
        percent *= 1.0 + D2BondSystem.GetAcolyteOrderBonus(state);
        return Math.Min(GetBonusCapFraction(riteId), percent / 100.0);
    }

    public static double GetBonusCapFraction(string riteId)
    {
        switch (riteId)
        {
            case WelcomeId: return 0.50;
            case OfferingId: return 0.60;
            case PathId: return 0.40;
            case NovitiateId: return 0.35;
            case RespectId: return 0.35;
            default: return 0.0;
        }
    }

    public static string GetDisplayName(string riteId)
    {
        switch (riteId)
        {
            case WelcomeId: return "Rito de Recibimiento";
            case OfferingId: return "Rito de Ofrenda";
            case PathId: return "Rito del Camino";
            case NovitiateId: return "Rito de Noviciado";
            case RespectId: return "Rito de Respeto";
            default: return "Rito desconocido";
        }
    }

    public static string GetEffectDescription(string riteId)
    {
        switch (riteId)
        {
            case WelcomeId: return "Aumenta la llegada de Seguidores.";
            case OfferingId: return "Aumenta la producción de todos los Altares.";
            case PathId: return "Mejora las recompensas materiales de las Peregrinaciones.";
            case NovitiateId: return "Reduce la duración de la formación de Acólitos.";
            case RespectId: return "Aumenta la Confianza obtenida en Peregrinaciones.";
            default: return "Sin efecto.";
        }
    }

    public static long GetTotalFollowersAssigned(D2Civilization1State state)
    {
        if (state?.rites == null)
            return 0L;

        long total = 0L;
        foreach (D2RiteState rite in state.rites)
            total = SaturatingAdd(total, Math.Max(0L, rite?.followersAssigned ?? 0L));
        return total;
    }

    public static long GetTotalAcolytesAssigned(D2Civilization1State state)
    {
        if (state?.rites == null)
            return 0L;

        long total = 0L;
        foreach (D2RiteState rite in state.rites)
            total = SaturatingAdd(total, Math.Max(0L, rite?.acolytesAssigned ?? 0L));
        return total;
    }

    public static bool ValidateState(D2Civilization1State state, out string result)
    {
        if (state == null)
        {
            result = "Estado de Ritos ausente.";
            return false;
        }

        EnsureState(state);
        if (state.rites.Count != RiteIds.Length)
        {
            result = "Catálogo de Ritos incompleto.";
            return false;
        }

        if (GetActiveRiteCount(state) > GetActiveSlotLimit(state))
        {
            result = "Hay más Ritos activos que espacios disponibles.";
            return false;
        }

        foreach (string riteId in RiteIds)
        {
            D2RiteState rite = FindRite(state.rites, riteId);
            if (rite == null || rite.followersAssigned < 0L || rite.acolytesAssigned < 0L)
            {
                result = "Estado de Rito inválido: " + riteId + ".";
                return false;
            }
        }

        result = "Estado de Ritos válido.";
        return true;
    }

    public static bool IsRiteId(string riteId)
    {
        foreach (string id in RiteIds)
        {
            if (id == riteId)
                return true;
        }
        return false;
    }

    private static bool TryGetState(
        GameState gameState,
        string riteId,
        out D2Civilization1State state,
        out D2RiteState rite
    )
    {
        state = null;
        rite = null;
        if (!Dimension2System.CanAccessDimension2(gameState) || !IsRiteId(riteId))
            return false;

        Dimension2System.EnsureState(gameState);
        state = gameState.dimension2.civilization1;
        if (!AreRitesUnlocked(state))
            return false;

        rite = GetRite(state, riteId);
        return rite != null;
    }

    private static bool IsActive(D2RiteState rite)
    {
        return rite != null && (rite.followersAssigned > 0L || rite.acolytesAssigned > 0L);
    }

    private static D2RiteState FindRite(List<D2RiteState> rites, string riteId)
    {
        if (rites == null)
            return null;

        foreach (D2RiteState rite in rites)
        {
            if (rite != null && rite.riteId == riteId)
                return rite;
        }
        return null;
    }

    private static long SaturatingAdd(long left, long right)
    {
        left = Math.Max(0L, left);
        right = Math.Max(0L, right);
        return left > long.MaxValue - right ? long.MaxValue : left + right;
    }
}

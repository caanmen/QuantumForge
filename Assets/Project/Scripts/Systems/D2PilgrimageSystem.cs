using System;
using UnityEngine;


public static class D2PilgrimageSystem
{
    public const string ShortId = "d2_c1_pilgrimage_short";
    public const string MediumId = "d2_c1_pilgrimage_medium";
    public const string LongId = "d2_c1_pilgrimage_long";
    public const string GuidedLongId = "d2_c1_pilgrimage_guided_long";
    public const string SacredId = "d2_c1_pilgrimage_sacred";

    public const double MaxTrust = 500.0;
    public const double Civilization2UnlockTrust = 300.0;
    public const double EntityContactTrust = D2VeiledThresholdSystem.UnlockTrustRequired;
    public const float MediumFollowerRewardChance = 0.25f;
    public const long MaxSupportFollowers = 4L;
    public const double SupportRewardFactor = 0.20;
    public const double MaxSupportRewardBonus = 0.40;
    public const double MaxMaterialRewardMultiplier = 2.0;

    public static readonly string[] PilgrimageIds =
        { ShortId, MediumId, LongId, GuidedLongId, SacredId };

    public static void EnsureState(GameState gameState)
    {
        if (gameState?.dimension2?.civilization1 == null)
            return;

        D2Civilization1State state = gameState.dimension2.civilization1;
        if (state.activePilgrimage == null)
            state.activePilgrimage = new D2PilgrimageState();

        state.trust = SanitizeTrust(state.trust);
        state.totalPilgrimagesCompleted = Math.Max(0L, state.totalPilgrimagesCompleted);
        state.shortPilgrimagesCompleted = Math.Max(0L, state.shortPilgrimagesCompleted);
        state.mediumPilgrimagesCompleted = Math.Max(0L, state.mediumPilgrimagesCompleted);
        state.longPilgrimagesCompleted = Math.Max(0L, state.longPilgrimagesCompleted);
        if (state.lastPilgrimageResult == null)
            state.lastPilgrimageResult = "";
        state.pilgrimageSupportFollowersSelected = Math.Clamp(
            state.pilgrimageSupportFollowersSelected,
            0L,
            MaxSupportFollowers
        );

        D2PilgrimageState active = state.activePilgrimage;
        if (active.active && !IsPilgrimageId(active.pilgrimageId))
        {
            state.followersAvailable = SaturatingAdd(
                state.followersAvailable,
                SaturatingAdd(
                    Math.Max(0L, active.followersCommitted),
                    Math.Max(0L, active.supportFollowersCommitted)
                )
            );
            state.acolytesAvailable = SaturatingAdd(
                state.acolytesAvailable,
                Math.Max(0L, active.acolytesCommitted)
            );
            ClearActive(active);
        }

        if (!active.active)
            ClearActive(active);
        else
        {
            active.followersCommitted = Math.Max(0L, active.followersCommitted);
            active.supportFollowersCommitted = Math.Max(
                0L,
                active.supportFollowersCommitted
            );
            active.acolytesCommitted = Math.Max(0L, active.acolytesCommitted);
            if (double.IsNaN(active.remainingSeconds) ||
                double.IsInfinity(active.remainingSeconds) ||
                active.remainingSeconds < 0.0)
            {
                active.remainingSeconds = 0.0;
            }
        }

        if (state.trust >= Civilization2UnlockTrust)
            gameState.dimension2.civilization2Unlocked = true;

        D2VeiledThresholdSystem.EnsureState(state);
    }

    public static void Tick(GameState gameState, double seconds)
    {
        if (gameState?.dimension2?.civilization1 == null || seconds <= 0.0 ||
            double.IsNaN(seconds) || double.IsInfinity(seconds))
        {
            return;
        }

        EnsureState(gameState);
        D2Civilization1State state = gameState.dimension2.civilization1;
        D2PilgrimageState active = state.activePilgrimage;
        if (!active.active)
            return;

        active.remainingSeconds = Math.Max(
            0.0,
            active.remainingSeconds - seconds
        );
        if (active.remainingSeconds <= 0.0)
            CompleteActive(gameState);
    }

    public static bool CanStart(GameState gameState, string pilgrimageId)
    {
        if (!Dimension2System.CanAccessDimension2(gameState) ||
            !IsPilgrimageId(pilgrimageId))
        {
            return false;
        }

        Dimension2System.EnsureState(gameState);
        D2Civilization1State state = gameState.dimension2.civilization1;
        if (state.activePilgrimage.active ||
            state.followersAvailable < GetFollowersRequired(pilgrimageId) +
                state.pilgrimageSupportFollowersSelected ||
            state.acolytesAvailable < GetAcolytesRequired(pilgrimageId))
        {
            return false;
        }

        D2AltarState wax = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(
            state,
            D2AltarSystem.RitualBreadAltarId
        );
        return wax != null && bread != null &&
            wax.offeringAmount >= GetEffectiveWaxCost(state, pilgrimageId) &&
            bread.offeringAmount >= GetEffectiveBreadCost(state, pilgrimageId);
    }

    public static bool TryStart(GameState gameState, string pilgrimageId)
    {
        if (!CanStart(gameState, pilgrimageId))
            return false;

        D2Civilization1State state = gameState.dimension2.civilization1;
        D2AltarState wax = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(
            state,
            D2AltarSystem.RitualBreadAltarId
        );
        long followers = GetFollowersRequired(pilgrimageId);
        long supportFollowers = state.pilgrimageSupportFollowersSelected;
        state.followersAvailable -= followers + supportFollowers;
        long acolytes = GetAcolytesRequired(pilgrimageId);
        state.acolytesAvailable -= acolytes;
        wax.offeringAmount -= GetEffectiveWaxCost(state, pilgrimageId);
        bread.offeringAmount -= GetEffectiveBreadCost(state, pilgrimageId);

        state.activePilgrimage.active = true;
        state.activePilgrimage.pilgrimageId = pilgrimageId;
        state.activePilgrimage.remainingSeconds = GetDurationSeconds(pilgrimageId);
        state.activePilgrimage.followersCommitted = followers;
        state.activePilgrimage.supportFollowersCommitted = supportFollowers;
        state.activePilgrimage.acolytesCommitted = acolytes;
        state.activePilgrimage.preparedMediumFollowerReward =
            pilgrimageId == MediumId && UnityEngine.Random.value < MediumFollowerRewardChance;
        state.lastPilgrimageResult = "";
        return true;
    }

    public static bool TryCancel(GameState gameState)
    {
        if (!Dimension2System.CanAccessDimension2(gameState))
            return false;

        Dimension2System.EnsureState(gameState);
        D2Civilization1State state = gameState.dimension2.civilization1;
        D2PilgrimageState active = state.activePilgrimage;
        if (!active.active)
            return false;

        state.followersAvailable = SaturatingAdd(
            state.followersAvailable,
            SaturatingAdd(active.followersCommitted, active.supportFollowersCommitted)
        );
        state.acolytesAvailable = SaturatingAdd(
            state.acolytesAvailable,
            active.acolytesCommitted
        );
        state.lastPilgrimageResult =
            GetDisplayName(active.pilgrimageId) +
            " cancelada. Los Seguidores regresaron; las Ofrendas no se devolvieron.";
        ClearActive(active);
        return true;
    }

    public static bool ValidateState(GameState gameState, out string result)
    {
        if (gameState?.dimension2?.civilization1 == null)
        {
            result = "Estado de Peregrinaciones ausente.";
            return false;
        }

        EnsureState(gameState);
        D2Civilization1State state = gameState.dimension2.civilization1;
        if (state.trust < 0.0 || state.trust > MaxTrust)
        {
            result = "Confianza fuera del intervalo 0–500.";
            return false;
        }

        D2PilgrimageState active = state.activePilgrimage;
        if (active.active && (!IsPilgrimageId(active.pilgrimageId) ||
            active.followersCommitted != GetFollowersRequired(active.pilgrimageId) ||
            active.supportFollowersCommitted < 0L ||
            active.supportFollowersCommitted > MaxSupportFollowers ||
            active.acolytesCommitted != GetAcolytesRequired(active.pilgrimageId) ||
            active.remainingSeconds < 0.0))
        {
            result = "Peregrinación activa inválida.";
            return false;
        }

        result = "Estado de Peregrinaciones y Confianza válido.";
        return true;
    }

    public static bool TryChangeSupportFollowers(GameState gameState, long delta)
    {
        if (!Dimension2System.CanAccessDimension2(gameState) || delta == 0L)
            return false;

        Dimension2System.EnsureState(gameState);
        D2Civilization1State state = gameState.dimension2.civilization1;
        if (state.activePilgrimage.active)
            return false;

        long next = Math.Clamp(
            state.pilgrimageSupportFollowersSelected + delta,
            0L,
            Math.Min(MaxSupportFollowers, state.followersAvailable)
        );
        if (next == state.pilgrimageSupportFollowersSelected)
            return false;

        state.pilgrimageSupportFollowersSelected = next;
        return true;
    }

    public static double GetSupportRewardBonus(long supportFollowers)
    {
        return Math.Min(
            MaxSupportRewardBonus,
            Math.Sqrt(Math.Max(0L, supportFollowers)) * SupportRewardFactor
        );
    }

    public static bool IsPilgrimageId(string id)
    {
        return id == ShortId || id == MediumId || id == LongId ||
            id == GuidedLongId || id == SacredId;
    }

    public static string GetDisplayName(string id)
    {
        switch (id)
        {
            case ShortId: return "Peregrinación Corta";
            case MediumId: return "Peregrinación Media";
            case LongId: return "Peregrinación Larga";
            case GuidedLongId: return "Peregrinación Larga con Acólito";
            case SacredId: return "Peregrinación Sagrada";
            default: return "Peregrinación desconocida";
        }
    }

    public static double GetDurationSeconds(string id)
    {
        switch (id)
        {
            case ShortId: return 60.0;
            case MediumId: return 240.0;
            case LongId: return 600.0;
            case GuidedLongId: return 600.0;
            case SacredId: return 900.0;
            default: return 0.0;
        }
    }

    public static long GetFollowersRequired(string id)
    {
        switch (id)
        {
            case ShortId: return 1L;
            case MediumId: return 3L;
            case LongId: return 6L;
            case GuidedLongId: return 6L;
            case SacredId: return 8L;
            default: return 0L;
        }
    }

    public static long GetAcolytesRequired(string id)
    {
        switch (id)
        {
            case GuidedLongId: return 1L;
            case SacredId: return 2L;
            default: return 0L;
        }
    }

    public static double GetWaxCost(string id)
    {
        switch (id)
        {
            case ShortId: return 2.0;
            case MediumId: return 10.0;
            case LongId: return 25.0;
            case GuidedLongId: return 35.0;
            case SacredId: return 50.0;
            default: return 0.0;
        }
    }

    public static double GetBreadCost(string id)
    {
        return GetWaxCost(id);
    }

    public static double GetEffectiveWaxCost(D2Civilization1State state, string id)
    {
        double multiplier = D2CivilizationPactSystem.IsPactActive(
            state,
            D2CivilizationPactSystem.OpenPathId
        )
            ? 1.0 + D2CivilizationPactSystem.OpenPathOfferingCostIncrease
            : 1.0;
        return GetWaxCost(id) * multiplier;
    }

    public static double GetEffectiveBreadCost(D2Civilization1State state, string id)
    {
        return GetEffectiveWaxCost(state, id);
    }

    public static double GetTrustReward(string id)
    {
        switch (id)
        {
            case ShortId: return 1.0;
            case MediumId: return 5.0;
            case LongId: return 12.0;
            case GuidedLongId: return 16.0;
            case SacredId: return 25.0;
            default: return 0.0;
        }
    }

    public static double GetEffectiveTrustReward(
        D2Civilization1State state,
        string id
    )
    {
        double multiplier = 1.0 +
            D2RiteSystem.GetBonusFraction(state, D2RiteSystem.RespectId);
        if (D2CivilizationPactSystem.IsPactActive(
            state,
            D2CivilizationPactSystem.OpenPathId
        ))
        {
            multiplier += D2CivilizationPactSystem.OpenPathRewardBonus;
        }
        if (D2CivilizationPactSystem.IsPactActive(
            state,
            D2CivilizationPactSystem.SilentVowId
        ))
        {
            multiplier += D2CivilizationPactSystem.SilentVowTrustBonus;
        }
        multiplier = Math.Min(2.0, multiplier);
        return GetTrustReward(id) * multiplier;
    }

    public static double GetOfferingReward(string id)
    {
        switch (id)
        {
            case ShortId: return 1.0;
            case MediumId: return 3.0;
            case LongId: return 8.0;
            case GuidedLongId: return 10.0;
            case SacredId: return 15.0;
            default: return 0.0;
        }
    }

    private static void CompleteActive(GameState gameState)
    {
        D2Civilization1State state = gameState.dimension2.civilization1;
        D2PilgrimageState active = state.activePilgrimage;
        if (!active.active)
            return;

        string pilgrimageId = active.pilgrimageId;
        long baseFollowerReward = pilgrimageId == SacredId
            ? 2L
            : pilgrimageId == LongId || pilgrimageId == GuidedLongId ||
            (pilgrimageId == MediumId && active.preparedMediumFollowerReward)
            ? 1L
            : 0L;
        double pilgrimageRewardMultiplier = GetMaterialRewardMultiplier(
            state,
            active.supportFollowersCommitted
        );
        double accumulatedFollowerReward = state.pactPilgrimageFollowerRewardProgress +
            (baseFollowerReward * pilgrimageRewardMultiplier);
        long followerReward = SafeFloorToLong(accumulatedFollowerReward);
        state.pactPilgrimageFollowerRewardProgress =
            accumulatedFollowerReward - followerReward;
        state.followersAvailable = SaturatingAdd(
            state.followersAvailable,
            SaturatingAdd(
                SaturatingAdd(
                    active.followersCommitted,
                    active.supportFollowersCommitted
                ),
                followerReward
            )
        );
        state.totalFollowersReceived = SaturatingAdd(
            state.totalFollowersReceived,
            followerReward
        );
        state.acolytesAvailable = SaturatingAdd(
            state.acolytesAvailable,
            active.acolytesCommitted
        );

        double offeringReward = GetOfferingReward(pilgrimageId) *
            pilgrimageRewardMultiplier;
        AddOfferingReward(state, D2AltarSystem.WaxAltarId, offeringReward);
        AddOfferingReward(state, D2AltarSystem.RitualBreadAltarId, offeringReward);
        double trustReward = GetEffectiveTrustReward(state, pilgrimageId);
        state.trust = Math.Min(MaxTrust, state.trust + trustReward);
        state.totalPilgrimagesCompleted = SaturatingAdd(
            state.totalPilgrimagesCompleted,
            1L
        );
        IncrementCompletionCount(state, pilgrimageId);
        state.lastPilgrimageResult =
            GetDisplayName(pilgrimageId) + " completada: +" +
            trustReward.ToString("0.##") + " Confianza, +" +
            offeringReward.ToString("0") + " Cera, +" +
            offeringReward.ToString("0") + " Pan ritual" +
            (followerReward > 0L
                ? ", +" + followerReward.ToString("N0") +
                  (followerReward == 1L ? " Seguidor." : " Seguidores.")
                : ".");
        ClearActive(active);
        EnsureState(gameState);
    }

    public static double GetMaterialRewardMultiplier(
        D2Civilization1State state,
        long supportFollowers
    )
    {
        double multiplier = 1.0 + GetSupportRewardBonus(supportFollowers) +
            D2RiteSystem.GetBonusFraction(state, D2RiteSystem.PathId) +
            D2BondSystem.GetPilgrimPathBonus(state);
        if (D2CivilizationPactSystem.IsPactActive(
            state,
            D2CivilizationPactSystem.OpenPathId
        ))
        {
            multiplier += D2CivilizationPactSystem.OpenPathRewardBonus;
        }
        return Math.Min(MaxMaterialRewardMultiplier, multiplier);
    }

    private static void AddOfferingReward(
        D2Civilization1State state,
        string altarId,
        double amount
    )
    {
        D2AltarState altar = D2AltarSystem.GetAltar(state, altarId);
        if (altar == null || amount <= 0.0)
            return;

        altar.offeringAmount = SaturatingAdd(altar.offeringAmount, amount);
        altar.totalOfferingProduced = SaturatingAdd(
            altar.totalOfferingProduced,
            amount
        );
    }

    private static void IncrementCompletionCount(D2Civilization1State state, string id)
    {
        if (id == ShortId)
            state.shortPilgrimagesCompleted = SaturatingAdd(state.shortPilgrimagesCompleted, 1L);
        else if (id == MediumId)
            state.mediumPilgrimagesCompleted = SaturatingAdd(state.mediumPilgrimagesCompleted, 1L);
        else if (id == LongId)
            state.longPilgrimagesCompleted = SaturatingAdd(state.longPilgrimagesCompleted, 1L);
    }

    private static void ClearActive(D2PilgrimageState active)
    {
        active.active = false;
        active.pilgrimageId = "";
        active.remainingSeconds = 0.0;
        active.followersCommitted = 0L;
        active.supportFollowersCommitted = 0L;
        active.acolytesCommitted = 0L;
        active.preparedMediumFollowerReward = false;
    }

    private static double SanitizeTrust(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            return 0.0;

        return Math.Clamp(value, 0.0, MaxTrust);
    }

    private static long SaturatingAdd(long left, long right)
    {
        left = Math.Max(0L, left);
        right = Math.Max(0L, right);
        return left > long.MaxValue - right ? long.MaxValue : left + right;
    }

    private static double SaturatingAdd(double left, double right)
    {
        if (double.IsNaN(left) || left < 0.0)
            left = 0.0;
        if (double.IsNaN(right) || right <= 0.0)
            return left;
        return double.IsInfinity(right) || left >= double.MaxValue - right
            ? double.MaxValue
            : left + right;
    }

    private static long SafeFloorToLong(double value)
    {
        if (double.IsNaN(value) || value <= 0.0)
            return 0L;
        if (double.IsInfinity(value) || value >= long.MaxValue)
            return long.MaxValue;
        return (long)Math.Floor(value);
    }
}

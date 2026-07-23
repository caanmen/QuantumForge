using System;


public static class D2NovitiateSystem
{
    public const int MinLevel = 1;
    public const int MaxLevel = 5;
    public const long MaxSupportFollowers = 4L;
    public const double SupportDurationFactor = 0.18;
    public const double MaxSupportDurationReduction = 0.35;
    public const double MaxCombinedDurationReduction = 0.50;

    private static readonly double[] Durations = { 300.0, 360.0, 480.0, 600.0, 720.0 };
    private static readonly long[] AcolytesPerBatch = { 1L, 2L, 4L, 7L, 10L };
    private static readonly double[] OfferingCosts = { 12.0, 22.0, 40.0, 65.0, 90.0 };
    private static readonly long[] UpgradeFollowerCosts = { 25L, 60L, 140L, 300L };
    private static readonly double[] UpgradeOfferingCosts = { 30.0, 75.0, 160.0, 350.0 };

    public static void EnsureState(D2Civilization1State state)
    {
        if (state == null)
            return;

        state.novitiateLevel = Math.Clamp(state.novitiateLevel, MinLevel, MaxLevel);
        state.acolytesAvailable = Math.Max(0L, state.acolytesAvailable);
        state.totalAcolytesCreated = Math.Max(
            state.acolytesAvailable + GetAcolytesCommittedToPilgrimage(state),
            state.totalAcolytesCreated
        );
        state.novitiateBatchesCompleted = Math.Max(0L, state.novitiateBatchesCompleted);
        if (state.lastNovitiateResult == null)
            state.lastNovitiateResult = "";
        state.novitiateSupportFollowersSelected = Math.Clamp(
            state.novitiateSupportFollowersSelected,
            0L,
            MaxSupportFollowers
        );
        if (state.activeNovitiateTraining == null)
            state.activeNovitiateTraining = new D2NovitiateTrainingState();

        D2NovitiateTrainingState active = state.activeNovitiateTraining;
        if (!active.active)
        {
            ClearActive(active);
            return;
        }

        active.trainingLevel = Math.Clamp(active.trainingLevel, MinLevel, MaxLevel);
        active.followersCommitted = Math.Max(0L, active.followersCommitted);
        active.supportFollowersCommitted = Math.Max(
            0L,
            active.supportFollowersCommitted
        );
        active.acolytesToCreate = Math.Max(0L, active.acolytesToCreate);
        if (double.IsNaN(active.remainingSeconds) ||
            double.IsInfinity(active.remainingSeconds) || active.remainingSeconds < 0.0)
        {
            active.remainingSeconds = 0.0;
        }
    }

    public static void Tick(GameState gameState, double seconds)
    {
        if (gameState?.dimension2?.civilization1 == null || seconds <= 0.0 ||
            double.IsNaN(seconds) || double.IsInfinity(seconds))
        {
            return;
        }

        D2Civilization1State state = gameState.dimension2.civilization1;
        EnsureState(state);
        D2NovitiateTrainingState active = state.activeNovitiateTraining;
        if (!active.active)
            return;

        double durationReduction = GetCombinedDurationReduction(
            state,
            active.supportFollowersCommitted
        );
        double progressMultiplier = 1.0 / Math.Max(0.01, 1.0 - durationReduction);
        if (D2CivilizationPactSystem.IsPactActive(
            state,
            D2CivilizationPactSystem.ConsecrationId
        ))
        {
            progressMultiplier /= 1.0 +
                D2CivilizationPactSystem.ConsecrationDurationIncrease;
        }
        active.remainingSeconds = Math.Max(
            0.0,
            active.remainingSeconds - (seconds * progressMultiplier)
        );
        if (active.remainingSeconds <= 0.0)
            CompleteTraining(state, D2Civilization3System.GetSharedMemoryMultiplier(gameState));
    }

    public static bool CanStartTraining(GameState gameState)
    {
        if (!Dimension2System.CanAccessDimension2(gameState))
            return false;

        Dimension2System.EnsureState(gameState);
        D2Civilization1State state = gameState.dimension2.civilization1;
        if (state.activeNovitiateTraining.active ||
            D2CivilizationPactSystem.IsPactActive(
                state,
                D2CivilizationPactSystem.InnerDoorId
            ))
            return false;

        D2AltarState wax = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(state, D2AltarSystem.RitualBreadAltarId);
        return state.followersAvailable >= GetFollowerCost(state.novitiateLevel) +
                state.novitiateSupportFollowersSelected &&
            wax != null && wax.offeringAmount >= GetOfferingCost(state.novitiateLevel) &&
            bread != null && bread.offeringAmount >= GetOfferingCost(state.novitiateLevel);
    }

    public static bool TryStartTraining(GameState gameState)
    {
        if (!CanStartTraining(gameState))
            return false;

        D2Civilization1State state = gameState.dimension2.civilization1;
        int level = state.novitiateLevel;
        long followers = GetFollowerCost(level);
        long supportFollowers = state.novitiateSupportFollowersSelected;
        double offerings = GetOfferingCost(level);
        D2AltarState wax = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(state, D2AltarSystem.RitualBreadAltarId);
        state.followersAvailable -= followers + supportFollowers;
        wax.offeringAmount -= offerings;
        bread.offeringAmount -= offerings;
        state.activeNovitiateTraining.active = true;
        state.activeNovitiateTraining.trainingLevel = level;
        state.activeNovitiateTraining.remainingSeconds = GetDurationSeconds(level);
        state.activeNovitiateTraining.followersCommitted = followers;
        state.activeNovitiateTraining.supportFollowersCommitted = supportFollowers;
        state.activeNovitiateTraining.acolytesToCreate = GetAcolytesPerBatch(level);
        state.lastNovitiateResult = "";
        return true;
    }

    public static bool TryCancelTraining(GameState gameState)
    {
        if (!Dimension2System.CanAccessDimension2(gameState))
            return false;

        Dimension2System.EnsureState(gameState);
        D2Civilization1State state = gameState.dimension2.civilization1;
        D2NovitiateTrainingState active = state.activeNovitiateTraining;
        if (!active.active)
            return false;

        state.followersAvailable = SaturatingAdd(
            state.followersAvailable,
            SaturatingAdd(active.followersCommitted, active.supportFollowersCommitted)
        );
        state.lastNovitiateResult =
            "Formación cancelada. Los Seguidores regresaron; las Ofrendas no se devolvieron.";
        ClearActive(active);
        return true;
    }

    public static bool CanUpgrade(GameState gameState)
    {
        if (!Dimension2System.CanAccessDimension2(gameState))
            return false;

        Dimension2System.EnsureState(gameState);
        D2Civilization1State state = gameState.dimension2.civilization1;
        if (state.novitiateLevel >= MaxLevel || state.activeNovitiateTraining.active ||
            D2CivilizationPactSystem.IsPactActive(
                state,
                D2CivilizationPactSystem.InnerDoorId
            ))
            return false;

        D2AltarState wax = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(state, D2AltarSystem.RitualBreadAltarId);
        return state.followersAvailable >= GetUpgradeFollowerCost(state.novitiateLevel) &&
            wax != null && wax.offeringAmount >= GetUpgradeOfferingCost(state.novitiateLevel) &&
            bread != null && bread.offeringAmount >= GetUpgradeOfferingCost(state.novitiateLevel);
    }

    public static bool TryUpgrade(GameState gameState)
    {
        if (!CanUpgrade(gameState))
            return false;

        D2Civilization1State state = gameState.dimension2.civilization1;
        long followers = GetUpgradeFollowerCost(state.novitiateLevel);
        double offerings = GetUpgradeOfferingCost(state.novitiateLevel);
        state.followersAvailable -= followers;
        D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId).offeringAmount -= offerings;
        D2AltarSystem.GetAltar(state, D2AltarSystem.RitualBreadAltarId).offeringAmount -= offerings;
        state.novitiateLevel++;
        return true;
    }

    public static double GetDurationSeconds(int level) => Durations[ToIndex(level)];

    public static double GetCurrentDurationSeconds(D2Civilization1State state, int level)
    {
        double duration = GetDurationSeconds(level) * (1.0 -
            GetCombinedDurationReduction(
                state,
                state?.novitiateSupportFollowersSelected ?? 0L
            ));
        if (D2CivilizationPactSystem.IsPactActive(
            state,
            D2CivilizationPactSystem.ConsecrationId
        ))
        {
            duration *= 1.0 + D2CivilizationPactSystem.ConsecrationDurationIncrease;
        }
        return duration;
    }

    public static double GetDisplayedRemainingSeconds(D2Civilization1State state)
    {
        D2NovitiateTrainingState active = state?.activeNovitiateTraining;
        if (active == null || !active.active)
            return 0.0;

        double displayedSeconds = active.remainingSeconds * (1.0 -
            GetCombinedDurationReduction(state, active.supportFollowersCommitted));
        if (D2CivilizationPactSystem.IsPactActive(
            state,
            D2CivilizationPactSystem.ConsecrationId
        ))
        {
            displayedSeconds *= 1.0 +
                D2CivilizationPactSystem.ConsecrationDurationIncrease;
        }
        return Math.Max(0.0, displayedSeconds);
    }

    public static double GetCurrentAcolytesPerBatch(
        D2Civilization1State state,
        int level
    )
    {
        double multiplier = D2CivilizationPactSystem.IsPactActive(
            state,
            D2CivilizationPactSystem.ConsecrationId
        )
            ? 1.0 + D2CivilizationPactSystem.ConsecrationAcolyteBonus
            : 1.0;
        return GetAcolytesPerBatch(level) * multiplier;
    }

    public static bool TryChangeSupportFollowers(GameState gameState, long delta)
    {
        if (!Dimension2System.CanAccessDimension2(gameState) || delta == 0L)
            return false;

        Dimension2System.EnsureState(gameState);
        D2Civilization1State state = gameState.dimension2.civilization1;
        if (state.activeNovitiateTraining.active)
            return false;

        long next = Math.Clamp(
            state.novitiateSupportFollowersSelected + delta,
            0L,
            Math.Min(MaxSupportFollowers, state.followersAvailable)
        );
        if (next == state.novitiateSupportFollowersSelected)
            return false;

        state.novitiateSupportFollowersSelected = next;
        return true;
    }

    public static double GetSupportDurationReduction(long supportFollowers)
    {
        return Math.Min(
            MaxSupportDurationReduction,
            Math.Sqrt(Math.Max(0L, supportFollowers)) * SupportDurationFactor
        );
    }

    public static double GetCombinedDurationReduction(
        D2Civilization1State state,
        long supportFollowers
    )
    {
        return Math.Min(
            MaxCombinedDurationReduction,
            GetSupportDurationReduction(supportFollowers) +
                D2RiteSystem.GetBonusFraction(state, D2RiteSystem.NovitiateId) +
                D2BondSystem.GetAcolyteOrderBonus(state)
        );
    }
    public static long GetAcolytesPerBatch(int level) => AcolytesPerBatch[ToIndex(level)];
    public static long GetFollowerCost(int level) => 5L * GetAcolytesPerBatch(level);
    public static double GetOfferingCost(int level) => OfferingCosts[ToIndex(level)];

    public static long GetUpgradeFollowerCost(int currentLevel)
    {
        return currentLevel >= MaxLevel ? 0L : UpgradeFollowerCosts[ToIndex(currentLevel)];
    }

    public static double GetUpgradeOfferingCost(int currentLevel)
    {
        return currentLevel >= MaxLevel ? 0.0 : UpgradeOfferingCosts[ToIndex(currentLevel)];
    }

    public static long GetAcolytesCommittedToPilgrimage(D2Civilization1State state)
    {
        return state?.activePilgrimage != null
            ? Math.Max(0L, state.activePilgrimage.acolytesCommitted)
            : 0L;
    }

    public static long GetTotalAcolytes(D2Civilization1State state)
    {
        if (state == null)
            return 0L;
        return SaturatingAdd(
            Math.Max(0L, state.acolytesAvailable),
            SaturatingAdd(
                GetAcolytesCommittedToPilgrimage(state),
                SaturatingAdd(
                    D2RiteSystem.GetTotalAcolytesAssigned(state),
                    Math.Max(0L, state.acolytesAssignedToBond)
                )
            )
        );
    }

    public static bool ValidateState(D2Civilization1State state, out string result)
    {
        if (state == null)
        {
            result = "Estado de Noviciado ausente.";
            return false;
        }

        EnsureState(state);
        if (state.novitiateLevel < MinLevel || state.novitiateLevel > MaxLevel ||
            state.acolytesAvailable < 0L)
        {
            result = "Nivel o saldo de Acólitos inválido.";
            return false;
        }

        D2NovitiateTrainingState active = state.activeNovitiateTraining;
        if (active.active && (active.followersCommitted != GetFollowerCost(active.trainingLevel) ||
            active.supportFollowersCommitted < 0L ||
            active.supportFollowersCommitted > MaxSupportFollowers ||
            active.acolytesToCreate != GetAcolytesPerBatch(active.trainingLevel) ||
            active.remainingSeconds < 0.0))
        {
            result = "Tanda activa del Noviciado inválida.";
            return false;
        }

        result = "Estado de Noviciado y Acólitos válido.";
        return true;
    }

    private static void CompleteTraining(
        D2Civilization1State state,
        double externalMultiplier
    )
    {
        D2NovitiateTrainingState active = state.activeNovitiateTraining;
        double creationMultiplier = D2CivilizationPactSystem.IsPactActive(
            state,
            D2CivilizationPactSystem.ConsecrationId
        )
            ? 1.0 + D2CivilizationPactSystem.ConsecrationAcolyteBonus
            : 1.0;
        double accumulated = state.pactConsecrationAcolyteProgress +
            (active.acolytesToCreate * creationMultiplier *
             Math.Max(1.0, externalMultiplier));
        long created = SafeFloorToLong(accumulated);
        state.pactConsecrationAcolyteProgress = accumulated - created;
        state.followersAvailable = SaturatingAdd(
            state.followersAvailable,
            active.supportFollowersCommitted
        );
        state.acolytesAvailable = SaturatingAdd(state.acolytesAvailable, created);
        state.totalAcolytesCreated = SaturatingAdd(state.totalAcolytesCreated, created);
        state.novitiateBatchesCompleted = SaturatingAdd(state.novitiateBatchesCompleted, 1L);
        state.lastNovitiateResult =
            "Tanda completada: " + created.ToString("N0") +
            (created == 1L ? " Acólito formado." : " Acólitos formados.");
        ClearActive(active);
    }

    private static void ClearActive(D2NovitiateTrainingState active)
    {
        active.active = false;
        active.trainingLevel = 0;
        active.remainingSeconds = 0.0;
        active.followersCommitted = 0L;
        active.supportFollowersCommitted = 0L;
        active.acolytesToCreate = 0L;
    }

    private static int ToIndex(int level) => Math.Clamp(level, MinLevel, MaxLevel) - 1;

    private static long SaturatingAdd(long left, long right)
    {
        left = Math.Max(0L, left);
        right = Math.Max(0L, right);
        return left > long.MaxValue - right ? long.MaxValue : left + right;
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

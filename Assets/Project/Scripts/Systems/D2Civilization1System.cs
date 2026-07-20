using System;


public static class D2Civilization1System
{
    public const long InitialFollowers = 5L;
    public const int MinRefugeLevel = 1;
    public const int MaxRefugeLevel = 10;
    public const double BaseFollowerArrivalPerSecond = 0.05;
    public const double RefugeLevelArrivalBonus = 0.25;
    public const double AssignedFollowerEfficiencyFactor = 0.15;
    public const double RefugeUpgradeBaseCost = 12.0;
    public const double RefugeUpgradeCostGrowth = 1.85;

    public static void EnsureState(D2Civilization1State state)
    {
        if (state == null)
            return;

        if (!state.initialFollowersGranted)
        {
            state.followersAvailable = SaturatingAdd(
                Math.Max(0L, state.followersAvailable),
                InitialFollowers
            );
            state.totalFollowersReceived = SaturatingAdd(
                Math.Max(0L, state.totalFollowersReceived),
                InitialFollowers
            );
            state.initialFollowersGranted = true;
        }

        state.progressVersion = Dimension2System.Civilization1ProgressVersion;
        state.followersAvailable = Math.Max(0L, state.followersAvailable);
        state.followersAssignedToRefuge = Math.Max(0L, state.followersAssignedToRefuge);
        D2AltarSystem.EnsureState(state);
        D2NovitiateSystem.EnsureState(state);
        D2RiteSystem.EnsureState(state);
        D2CivilizationPactSystem.EnsureState(state);
        D2BondSystem.EnsureState(state);
        state.totalFollowersReceived = Math.Max(
            GetTotalFollowers(state),
            state.totalFollowersReceived
        );
        state.refugeLevel = Math.Clamp(
            state.refugeLevel,
            MinRefugeLevel,
            MaxRefugeLevel
        );

        if (double.IsNaN(state.followerArrivalProgress) ||
            double.IsInfinity(state.followerArrivalProgress) ||
            state.followerArrivalProgress < 0.0)
        {
            state.followerArrivalProgress = 0.0;
        }

        if (state.followerArrivalProgress >= 1.0)
        {
            long completedFollowers = SafeFloorToLong(state.followerArrivalProgress);
            AddFollowers(state, completedFollowers);
            state.followerArrivalProgress -= completedFollowers;
        }
    }

    public static void Tick(GameState gameState, double dt)
    {
        if (gameState == null || dt <= 0.0 || double.IsNaN(dt) || double.IsInfinity(dt))
            return;

        Dimension2System.EnsureState(gameState);
        D2Civilization1State civilization1 = gameState.dimension2.civilization1;
        D2AltarSystem.Tick(civilization1, dt);
        double hospitalitySeconds = D2CivilizationPactSystem
            .ConsumeHospitalityMaintenance(civilization1, dt);
        ProduceFollowers(civilization1, hospitalitySeconds, true);
        ProduceFollowers(civilization1, dt - hospitalitySeconds, false);
        D2PilgrimageSystem.Tick(gameState, dt);
        D2NovitiateSystem.Tick(gameState, dt);
        D2BondSystem.Tick(civilization1, dt);
    }

    public static void ApplyOfflineProgress(GameState gameState, double offlineSeconds)
    {
        if (gameState == null || offlineSeconds <= 0.0 ||
            double.IsNaN(offlineSeconds) || double.IsInfinity(offlineSeconds))
        {
            return;
        }

        Dimension2System.EnsureState(gameState);
        D2Civilization1State civilization1 = gameState.dimension2.civilization1;
        D2AltarSystem.Tick(civilization1, offlineSeconds);
        double hospitalitySeconds = D2CivilizationPactSystem
            .ConsumeHospitalityMaintenance(civilization1, offlineSeconds);
        ProduceFollowers(civilization1, hospitalitySeconds, true);
        ProduceFollowers(civilization1, offlineSeconds - hospitalitySeconds, false);
        D2PilgrimageSystem.Tick(gameState, offlineSeconds);
        D2NovitiateSystem.Tick(gameState, offlineSeconds);
        D2BondSystem.Tick(civilization1, offlineSeconds);
    }

    public static double GetFollowerArrivalPerSecond(D2Civilization1State state)
    {
        if (state == null)
            return 0.0;

        EnsureState(state);
        return GetFollowerArrivalPerSecond(
            state,
            D2CivilizationPactSystem.IsPactActive(
                state,
                D2CivilizationPactSystem.HospitalityId
            )
        );
    }

    private static double GetFollowerArrivalPerSecond(
        D2Civilization1State state,
        bool hospitalitySupported
    )
    {
        double levelMultiplier = 1.0 +
            ((state.refugeLevel - MinRefugeLevel) * RefugeLevelArrivalBonus);
        double assignmentMultiplier = 1.0 +
            (Math.Sqrt(state.followersAssignedToRefuge) * AssignedFollowerEfficiencyFactor);
        double riteMultiplier = 1.0 +
            D2RiteSystem.GetBonusFraction(state, D2RiteSystem.WelcomeId);
        riteMultiplier += D2BondSystem.GetPilgrimPathBonus(state);
        double pactMultiplier = 1.0;
        if (hospitalitySupported)
            pactMultiplier += D2CivilizationPactSystem.HospitalityFollowerBonus;
        if (D2CivilizationPactSystem.IsPactActive(
            state,
            D2CivilizationPactSystem.SilentVowId
        ))
        {
            pactMultiplier *= 1.0 - D2CivilizationPactSystem.SilentVowFollowerReduction;
        }
        return BaseFollowerArrivalPerSecond * levelMultiplier * assignmentMultiplier *
            riteMultiplier * pactMultiplier;
    }

    public static double GetAssignedFollowerMultiplier(D2Civilization1State state)
    {
        if (state == null)
            return 1.0;

        EnsureState(state);
        return 1.0 +
            (Math.Sqrt(state.followersAssignedToRefuge) * AssignedFollowerEfficiencyFactor);
    }

    public static long GetTotalFollowers(D2Civilization1State state)
    {
        if (state == null)
            return 0L;

        long refugeAndAvailable = SaturatingAdd(
            Math.Max(0L, state.followersAvailable),
            Math.Max(0L, state.followersAssignedToRefuge)
        );
        long followersWithPilgrimage = SaturatingAdd(
            SaturatingAdd(
                refugeAndAvailable,
                D2AltarSystem.GetTotalFollowersAssignedToAltars(state)
            ),
            state.activePilgrimage != null
                ? SaturatingAdd(
                    Math.Max(0L, state.activePilgrimage.followersCommitted),
                    Math.Max(0L, state.activePilgrimage.supportFollowersCommitted)
                )
                : 0L
        );
        long followersWithNovitiate = SaturatingAdd(
            followersWithPilgrimage,
            state.activeNovitiateTraining != null
                ? SaturatingAdd(
                    Math.Max(0L, state.activeNovitiateTraining.followersCommitted),
                    Math.Max(0L, state.activeNovitiateTraining.supportFollowersCommitted)
                )
                : 0L
        );
        return SaturatingAdd(
            followersWithNovitiate,
            D2RiteSystem.GetTotalFollowersAssigned(state)
        );
    }

    public static bool TryAssignFollowersToRefuge(GameState gameState, long amount)
    {
        if (!TryGetActiveState(gameState, out D2Civilization1State state) || amount <= 0L)
            return false;

        long assigned = Math.Min(amount, state.followersAvailable);
        if (assigned <= 0L)
            return false;

        state.followersAvailable -= assigned;
        state.followersAssignedToRefuge = SaturatingAdd(
            state.followersAssignedToRefuge,
            assigned
        );
        return true;
    }

    public static bool TryAssignAllFollowersToRefuge(GameState gameState)
    {
        if (!TryGetActiveState(gameState, out D2Civilization1State state))
            return false;

        return TryAssignFollowersToRefuge(gameState, state.followersAvailable);
    }

    public static bool TryReleaseFollowersFromRefuge(GameState gameState, long amount)
    {
        if (!TryGetActiveState(gameState, out D2Civilization1State state) || amount <= 0L)
            return false;

        long released = Math.Min(amount, state.followersAssignedToRefuge);
        if (released <= 0L)
            return false;

        state.followersAssignedToRefuge -= released;
        state.followersAvailable = SaturatingAdd(state.followersAvailable, released);
        return true;
    }

    public static bool TryReleaseAllFollowersFromRefuge(GameState gameState)
    {
        if (!TryGetActiveState(gameState, out D2Civilization1State state))
            return false;

        return TryReleaseFollowersFromRefuge(gameState, state.followersAssignedToRefuge);
    }

    public static long GetNextRefugeUpgradeCost(D2Civilization1State state)
    {
        if (state == null)
            return 0L;

        EnsureState(state);
        if (state.refugeLevel >= MaxRefugeLevel)
            return 0L;

        double rawCost = RefugeUpgradeBaseCost * Math.Pow(
            RefugeUpgradeCostGrowth,
            state.refugeLevel - MinRefugeLevel
        );
        return Math.Max(1L, SafeCeilingToLong(rawCost));
    }

    public static bool CanUpgradeRefuge(GameState gameState)
    {
        if (!TryGetActiveState(gameState, out D2Civilization1State state))
            return false;

        long cost = GetNextRefugeUpgradeCost(state);
        return cost > 0L && state.followersAvailable >= cost;
    }

    public static bool TryUpgradeRefuge(GameState gameState)
    {
        if (!TryGetActiveState(gameState, out D2Civilization1State state))
            return false;

        long cost = GetNextRefugeUpgradeCost(state);
        if (cost <= 0L || state.followersAvailable < cost)
            return false;

        state.followersAvailable -= cost;
        state.refugeLevel++;
        return true;
    }

    public static bool ValidateState(D2Civilization1State state, out string result)
    {
        if (state == null)
        {
            result = "Estado de Civilización 1 es null.";
            return false;
        }

        EnsureState(state);
        if (!state.initialFollowersGranted)
        {
            result = "El paquete inicial de Seguidores no fue registrado.";
            return false;
        }

        if (state.followersAvailable < 0L || state.followersAssignedToRefuge < 0L)
        {
            result = "Hay una cantidad negativa de Seguidores.";
            return false;
        }

        if (state.refugeLevel < MinRefugeLevel || state.refugeLevel > MaxRefugeLevel)
        {
            result = "Nivel de Refugio fuera de rango.";
            return false;
        }

        if (state.followerArrivalProgress < 0.0 || state.followerArrivalProgress >= 1.0)
        {
            result = "Progreso fraccional de Seguidores fuera de rango.";
            return false;
        }

        if (!D2AltarSystem.ValidateState(state, out result))
            return false;

        if (!D2NovitiateSystem.ValidateState(state, out result))
            return false;

        if (!D2RiteSystem.ValidateState(state, out result))
            return false;

        if (!D2CivilizationPactSystem.ValidateState(state, out result))
            return false;

        if (!D2BondSystem.ValidateState(state, out result))
            return false;

        result = "Estado de Refugio, Seguidores, Altares y Noviciado válido.";
        return true;
    }

    private static void ProduceFollowers(
        D2Civilization1State state,
        double seconds,
        bool hospitalitySupported
    )
    {
        if (state == null || seconds <= 0.0)
            return;

        EnsureState(state);
        double produced = GetFollowerArrivalPerSecond(state, hospitalitySupported) * seconds;
        if (double.IsNaN(produced) || produced <= 0.0)
            return;

        if (double.IsInfinity(produced) || produced >= long.MaxValue)
        {
            state.followersAvailable = long.MaxValue;
            state.totalFollowersReceived = long.MaxValue;
            state.followerArrivalProgress = 0.0;
            return;
        }

        double accumulated = state.followerArrivalProgress + produced;
        long completedFollowers = SafeFloorToLong(accumulated);
        state.followerArrivalProgress = accumulated - completedFollowers;
        AddFollowers(state, completedFollowers);
    }

    private static void AddFollowers(D2Civilization1State state, long amount)
    {
        if (state == null || amount <= 0L)
            return;

        state.followersAvailable = SaturatingAdd(state.followersAvailable, amount);
        state.totalFollowersReceived = SaturatingAdd(state.totalFollowersReceived, amount);
    }

    private static bool TryGetActiveState(
        GameState gameState,
        out D2Civilization1State civilization1
    )
    {
        civilization1 = null;
        if (!Dimension2System.CanAccessDimension2(gameState))
            return false;

        Dimension2System.EnsureState(gameState);
        if (!gameState.dimension2.civilization1Unlocked)
            return false;

        civilization1 = gameState.dimension2.civilization1;
        EnsureState(civilization1);
        return true;
    }

    private static long SaturatingAdd(long left, long right)
    {
        if (left < 0L)
            left = 0L;

        if (right <= 0L)
            return left;

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

    private static long SafeCeilingToLong(double value)
    {
        if (double.IsNaN(value) || value <= 0.0)
            return 0L;

        if (double.IsInfinity(value) || value >= long.MaxValue)
            return long.MaxValue;

        return (long)Math.Ceiling(value);
    }
}

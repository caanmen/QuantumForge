using System;
using System.Collections.Generic;


public static class D2Civilization2System
{
    public const string Region1Id = "d2_c2_region_1";
    public const string Region2Id = "d2_c2_region_2";
    public const string Region3Id = "d2_c2_region_3";
    public const string Region4Id = "d2_c2_region_4";

    public const string RescueOperationId = "d2_c2_operation_rescue";
    public const string ProtectionOperationId = "d2_c2_operation_protection";
    public const string EspionageOperationId = "d2_c2_operation_espionage";
    public const string SabotageOperationId = "d2_c2_operation_sabotage";

    public const long InitialMembers = 10L;
    public const double InitialDominance = 100.0;
    public const double InitialThreat = 0.0;
    public const double RescueMemberFactorPerMinute = 0.20;
    public const double ProtectionMemberFactorPerMinute = 0.08;
    public const double RescueDominancePerMinute = 0.05;
    public const double ProtectionDominancePerMinute = 0.02;
    public const double EspionageDominancePerMinute = 0.12;
    public const double SabotageDominancePerMinute = 0.30;
    public const double RescueThreatPerMinute = 0.20;
    public const double ProtectionThreatPerMinute = -0.35;
    public const double EspionageThreatPerMinute = 0.25;
    public const double SabotageThreatPerMinute = 0.60;
    public const double ProtectionCoverageFactorPerMinute = 0.25;
    public const double MaxCoverage = 60.0;
    public const double CoveragePerLossPoint = 10.0;
    public const double CoverageRetentionAfterReprisal = 0.50;
    public const double BaseReprisalLossFraction = 0.08;
    public const double MinimumReprisalLossFraction = 0.02;
    public const double EspionageReprisalReduction = 0.05;
    public const double WeakenedOperationMultiplier = 0.50;
    public const double WeakenedOperationDurationSeconds = 180.0;
    public const double ThreatAfterReprisal = 25.0;
    public const long ControlFragmentsPerReprisal = 3L;
    public const double Region2UnlockDominance = 80.0;
    public const double Region3UnlockDominance = 60.0;
    public const string RescueUpgradeId = "d2_c2_upgrade_rescue";
    public const string CoverageUpgradeId = "d2_c2_upgrade_coverage";
    public const string EspionageUpgradeId = "d2_c2_upgrade_espionage";
    public const string SabotageUpgradeId = "d2_c2_upgrade_sabotage";
    public const string HiddenSheltersPactId = "d2_c2_pact_hidden_shelters";
    public const string SilencedBellsPactId = "d2_c2_pact_silenced_bells";
    public const string KnivesPactId = "d2_c2_pact_knives";
    public const int MaxUpgradeLevel = 3;
    public const double UpgradeBonusPerLevel = 0.10;
    public const double EspionageUpgradeReductionPerLevel = 0.01;
    public const double ExhaustedRecoverySeconds = 300.0;
    public const double PactPenaltySeconds = 300.0;
    public const double HiddenSheltersLossMultiplier = 0.80;
    public const double SilencedBellsDurationMultiplier = 0.70;
    public const double KnivesSabotageMultiplier = 1.20;
    public const double HiddenSheltersBreachMultiplier = 1.25;
    public const double SilencedBellsBreachMultiplier = 1.40;
    public const double KnivesBreachThreatMultiplier = 1.30;
    public const double AlertDominanceThreshold = 30.0;
    public const double AlertThreatMultiplier = 1.50;
    public const long AlertControlFragmentsPerReprisal = 6L;
    public const double AlertMarkIntervalSeconds = 600.0;
    public const double AlertMarkExtraLossFraction = 0.03;
    public const double ContainmentFailureThreatIncrease = 20.0;
    public const double ContainmentFailureLossFraction = 0.05;
    public const double ContainmentCooldownDurationSeconds = 600.0;
    public const string ReconstitutedNetworkLineId = "d2_c2_major_reconstituted_network";
    public const string CoordinatedUprisingLineId = "d2_c2_major_coordinated_uprising";
    public const string LinkedDefenseLineId = "d2_c2_major_linked_defense";
    public const string ArtifactCustodyLineId = "d2_c2_major_artifact_custody";
    public const string ResistanceGeometryLineId = "d2_c2_major_resistance_geometry";
    public const int MaxMajorPactLineLevel = 3;
    public const double ContainmentStabilityPerMinuteFactor = 1.0;
    public const double MajorPactMemberBonusPerLevel = 0.05;
    public const double MajorPactDominanceBonusPerLevel = 0.05;
    public const double MajorPactCoverageBonusPerLevel = 0.05;
    public const double MajorPactReprisalLossReductionPerLevel = 0.01;
    public const double MajorPactArtifactBonusPerLevel = 0.02;
    public const double MajorPactTriangleBonusPerLevel = 0.02;
    private const double UnlockComparisonEpsilon = 0.000001;

    public static readonly string[] RegionIds =
    {
        Region1Id,
        Region2Id,
        Region3Id,
        Region4Id
    };

    public static readonly string[] OperationIds =
    {
        RescueOperationId,
        ProtectionOperationId,
        EspionageOperationId,
        SabotageOperationId
    };
    public static readonly string[] UpgradeIds =
        { RescueUpgradeId, CoverageUpgradeId, EspionageUpgradeId, SabotageUpgradeId };
    public static readonly string[] ResistancePactIds =
        { HiddenSheltersPactId, SilencedBellsPactId, KnivesPactId };
    public static readonly string[] MajorPactLineIds =
    {
        ReconstitutedNetworkLineId,
        CoordinatedUprisingLineId,
        LinkedDefenseLineId,
        ArtifactCustodyLineId,
        ResistanceGeometryLineId
    };

    public static void EnsureState(D2Civilization2State state)
    {
        if (state == null)
            return;

        if (!state.initialMembersGranted)
        {
            state.membersAvailable = SaturatingAdd(
                Math.Max(0L, state.membersAvailable),
                InitialMembers
            );
            state.totalMembersRecruited = Math.Max(
                state.totalMembersRecruited,
                state.membersAvailable
            );
            state.initialMembersGranted = true;
        }

        state.membersAvailable = Math.Max(0L, state.membersAvailable);
        state.controlFragments = Math.Max(0L, state.controlFragments);
        state.totalReprisals = Math.Max(0L, state.totalReprisals);
        state.totalMembersRecruited = Math.Max(
            GetTotalMembers(state),
            Math.Max(0L, state.totalMembersRecruited)
        );
        if (double.IsNaN(state.memberProductionProgress) ||
            double.IsInfinity(state.memberProductionProgress) ||
            state.memberProductionProgress < 0.0)
        {
            state.memberProductionProgress = 0.0;
        }

        if (state.lastResult == null)
            state.lastResult = "";
        if (state.lastAlertResult == null)
            state.lastAlertResult = "";
        if (state.lastContainmentResult == null)
            state.lastContainmentResult = "";
        if (state.lastMajorPactResult == null)
            state.lastMajorPactResult = "";

        EnsureRegions(state);
        EnsureResistanceProgress(state);
        UpdateRegionUnlocks(state);
        if (!IsSelectableRegion(state, state.selectedRegionId))
            state.selectedRegionId = Region1Id;
        state.totalMembersRecruited = Math.Max(
            GetTotalMembers(state),
            state.totalMembersRecruited
        );
        state.progressVersion = Dimension2System.Civilization2ProgressVersion;
    }

    public static bool SyncAlertUnlocks(GameState gameState)
    {
        if (gameState?.dimension2?.civilization2 == null)
            return false;

        D2Civilization2State state = gameState.dimension2.civilization2;
        if (!state.alertActive &&
            GetTotalDominance(state) <= AlertDominanceThreshold + UnlockComparisonEpsilon)
        {
            state.alertActive = true;
            state.containmentAvailable = true;
            state.alertMarkProgressSeconds = 0.0;
            state.lastAlertResult =
                "El Ente ha entrado en Alerta. Civilización 3 y Contención están disponibles.";
            state.lastResult = state.lastAlertResult;
        }

        if (!state.alertActive)
            return false;

        state.containmentAvailable = true;
        gameState.dimension2.civilization3Unlocked = true;
        return true;
    }

    public static void Tick(GameState gameState, double dt)
    {
        if (gameState?.dimension2?.civilization2 == null || dt <= 0.0 ||
            double.IsNaN(dt) || double.IsInfinity(dt))
        {
            return;
        }

        EnsureState(gameState.dimension2.civilization2);
        if (!gameState.dimension2.civilization2Unlocked)
            return;

        SyncAlertUnlocks(gameState);
        AdvanceCivilization(gameState, dt);
    }

    public static void ApplyOfflineProgress(GameState gameState, double seconds)
    {
        Tick(gameState, seconds);
    }

    public static D2RegionState GetRegion(D2Civilization2State state, string regionId)
    {
        if (state?.regions == null)
            return null;

        foreach (D2RegionState region in state.regions)
        {
            if (region != null && region.regionId == regionId)
                return region;
        }

        return null;
    }

    public static string GetRegionDisplayName(string regionId)
    {
        switch (regionId)
        {
            case Region1Id: return "Región 1";
            case Region2Id: return "Región 2";
            case Region3Id: return "Región 3";
            case Region4Id: return "Región 4";
            default: return "Región desconocida";
        }
    }

    public static D2RegionState GetSelectedRegion(D2Civilization2State state)
    {
        if (state == null)
            return null;

        D2RegionState selected = GetRegion(state, state.selectedRegionId);
        return selected != null && selected.unlocked && selected.regionId != Region4Id
            ? selected
            : GetRegion(state, Region1Id);
    }

    public static bool TrySelectRegion(GameState gameState, string regionId)
    {
        if (!TryGetState(gameState, out D2Civilization2State state) ||
            !IsSelectableRegion(state, regionId))
        {
            return false;
        }

        state.selectedRegionId = regionId;
        return true;
    }

    public static bool IsSelectableRegion(D2Civilization2State state, string regionId)
    {
        D2RegionState region = GetRegion(state, regionId);
        return region != null && region.unlocked && region.regionId != Region4Id;
    }

    public static bool UpdateRegionUnlocks(D2Civilization2State state)
    {
        if (state == null)
            return false;

        bool changed = false;
        D2RegionState region2 = GetRegion(state, Region2Id);
        D2RegionState region3 = GetRegion(state, Region3Id);
        if (region2 != null && !region2.unlocked &&
            GetTotalDominance(state) <= Region2UnlockDominance + UnlockComparisonEpsilon)
        {
            region2.unlocked = true;
            region2.dominance = InitialDominance;
            region2.threat = InitialThreat;
            state.lastResult =
                "Región 2 desbloqueada. Al comenzar en 100% de Dominio, " +
                "el promedio total puede aumentar.";
            changed = true;
        }

        if (region2 != null && region2.unlocked && region3 != null &&
            !region3.unlocked &&
            GetTotalDominance(state) <= Region3UnlockDominance + UnlockComparisonEpsilon)
        {
            region3.unlocked = true;
            region3.dominance = InitialDominance;
            region3.threat = InitialThreat;
            state.lastResult =
                "Región 3 desbloqueada. Al comenzar en 100% de Dominio, " +
                "el promedio total puede aumentar.";
            changed = true;
        }

        return changed;
    }

    public static D2OperationState GetOperation(D2RegionState region, string operationId)
    {
        if (region?.operations == null)
            return null;

        foreach (D2OperationState operation in region.operations)
        {
            if (operation != null && operation.operationId == operationId)
                return operation;
        }

        return null;
    }

    public static string GetOperationDisplayName(string operationId)
    {
        switch (operationId)
        {
            case RescueOperationId: return "Rescate";
            case ProtectionOperationId: return "Protección";
            case EspionageOperationId: return "Espionaje";
            case SabotageOperationId: return "Sabotaje";
            default: return "Operación desconocida";
        }
    }

    public static string GetOperationDescription(string operationId)
    {
        switch (operationId)
        {
            case RescueOperationId:
                return "Genera Miembros; reduce lentamente Dominio y aumenta Amenaza.";
            case ProtectionOperationId:
                return "Genera algunos Miembros, reduce Amenaza y prepara la defensa regional.";
            case EspionageOperationId:
                return "Reduce Dominio de forma más segura, con Amenaza moderada.";
            case SabotageOperationId:
                return "Reduce mucho el Dominio, pero aumenta bastante la Amenaza.";
            default:
                return "";
        }
    }

    public static long GetOperationRequirement(string operationId)
    {
        switch (operationId)
        {
            case RescueOperationId: return 5L;
            case ProtectionOperationId: return 5L;
            case EspionageOperationId: return 10L;
            case SabotageOperationId: return 20L;
            default: return long.MaxValue;
        }
    }

    public static bool IsOperationActive(D2OperationState operation)
    {
        return operation != null &&
            operation.membersAssigned >= GetOperationRequirement(operation.operationId);
    }

    public static long GetMembersAssignedToOperations(D2RegionState region)
    {
        if (region?.operations == null)
            return 0L;

        long total = 0L;
        foreach (D2OperationState operation in region.operations)
        {
            if (operation != null)
                total = SaturatingAdd(total, Math.Max(0L, operation.membersAssigned));
        }

        return total;
    }

    public static long GetRegionIdleMembers(D2RegionState region)
    {
        if (region == null)
            return 0L;

        return Math.Max(0L, region.membersAssigned - GetMembersAssignedToOperations(region));
    }

    public static long GetAssignedMembers(D2Civilization2State state)
    {
        if (state?.regions == null)
            return 0L;

        long total = 0L;
        foreach (D2RegionState region in state.regions)
        {
            if (region != null)
                total = SaturatingAdd(total, Math.Max(0L, region.membersAssigned));
        }

        return total;
    }

    public static long GetTotalMembers(D2Civilization2State state)
    {
        if (state == null)
            return 0L;

        long total = SaturatingAdd(
            Math.Max(0L, state.membersAvailable),
            GetAssignedMembers(state)
        );
        total = SaturatingAdd(total, GetMembersAssignedToPacts(state));
        total = SaturatingAdd(total, GetExhaustedMembers(state));
        return SaturatingAdd(total, Math.Max(0L, state.membersAssignedToContainment));
    }

    public static long GetMembersAssignedToPacts(D2Civilization2State state)
    {
        long total = 0L;
        if (state?.resistancePacts == null)
            return total;

        foreach (D2ResistancePactState pact in state.resistancePacts)
        {
            if (pact != null)
                total = SaturatingAdd(total, Math.Max(0L, pact.membersAssigned));
        }

        return total;
    }

    public static double GetTotalDominance(D2Civilization2State state)
    {
        if (state?.regions == null)
            return InitialDominance;

        double total = 0.0;
        int active = 0;
        foreach (D2RegionState region in state.regions)
        {
            if (region == null || !region.unlocked || region.regionId == Region4Id)
                continue;

            total += Math.Clamp(region.dominance, 0.0, 100.0);
            active++;
        }

        return active > 0 ? total / active : InitialDominance;
    }

    public static bool TryAssignMembers(
        GameState gameState,
        string regionId,
        long requestedAmount
    )
    {
        if (!TryGetState(gameState, out D2Civilization2State state) || requestedAmount <= 0L)
            return false;

        D2RegionState region = GetRegion(state, regionId);
        if (region == null || !region.unlocked || regionId == Region4Id)
            return false;

        long assigned = Math.Min(requestedAmount, state.membersAvailable);
        if (assigned <= 0L)
            return false;

        state.membersAvailable -= assigned;
        region.membersAssigned = SaturatingAdd(region.membersAssigned, assigned);
        state.lastResult = assigned.ToString("N0") + " Miembro(s) asignado(s) a " +
            GetRegionDisplayName(regionId) + ".";
        return true;
    }

    public static bool TryAssignAllMembers(GameState gameState, string regionId)
    {
        if (!TryGetState(gameState, out D2Civilization2State state))
            return false;

        return TryAssignMembers(gameState, regionId, state.membersAvailable);
    }

    public static bool TryReleaseMembers(
        GameState gameState,
        string regionId,
        long requestedAmount
    )
    {
        if (!TryGetState(gameState, out D2Civilization2State state) || requestedAmount <= 0L)
            return false;

        D2RegionState region = GetRegion(state, regionId);
        if (region == null)
            return false;

        long released = Math.Min(requestedAmount, GetRegionIdleMembers(region));
        if (released <= 0L)
            return false;

        region.membersAssigned -= released;
        state.membersAvailable = SaturatingAdd(state.membersAvailable, released);
        state.lastResult = released.ToString("N0") + " Miembro(s) retirado(s) de " +
            GetRegionDisplayName(regionId) + ".";
        return true;
    }

    public static bool TryReleaseAllMembers(GameState gameState, string regionId)
    {
        if (!TryGetState(gameState, out D2Civilization2State state))
            return false;

        D2RegionState region = GetRegion(state, regionId);
        return region != null && TryReleaseMembers(
            gameState,
            regionId,
            GetRegionIdleMembers(region)
        );
    }

    public static bool TryAssignMembersToOperation(
        GameState gameState,
        string regionId,
        string operationId,
        long requestedAmount
    )
    {
        if (!TryGetState(gameState, out D2Civilization2State state) ||
            requestedAmount <= 0L)
        {
            return false;
        }

        D2RegionState region = GetRegion(state, regionId);
        D2OperationState operation = GetOperation(region, operationId);
        if (region == null || !region.unlocked || operation == null)
            return false;

        long assigned = Math.Min(requestedAmount, GetRegionIdleMembers(region));
        if (assigned <= 0L)
            return false;

        operation.membersAssigned = SaturatingAdd(operation.membersAssigned, assigned);
        state.lastResult = assigned.ToString("N0") + " Miembro(s) destinado(s) a " +
            GetOperationDisplayName(operationId) + ".";
        return true;
    }

    public static bool TryAssignAllMembersToOperation(
        GameState gameState,
        string regionId,
        string operationId
    )
    {
        if (!TryGetState(gameState, out D2Civilization2State state))
            return false;

        D2RegionState region = GetRegion(state, regionId);
        return region != null && TryAssignMembersToOperation(
            gameState,
            regionId,
            operationId,
            GetRegionIdleMembers(region)
        );
    }

    public static bool TryReleaseMembersFromOperation(
        GameState gameState,
        string regionId,
        string operationId,
        long requestedAmount
    )
    {
        if (!TryGetState(gameState, out D2Civilization2State state) ||
            requestedAmount <= 0L)
        {
            return false;
        }

        D2RegionState region = GetRegion(state, regionId);
        D2OperationState operation = GetOperation(region, operationId);
        if (operation == null)
            return false;

        long released = Math.Min(requestedAmount, Math.Max(0L, operation.membersAssigned));
        if (released <= 0L)
            return false;

        operation.membersAssigned -= released;
        state.lastResult = released.ToString("N0") + " Miembro(s) retirado(s) de " +
            GetOperationDisplayName(operationId) + ".";
        return true;
    }

    public static bool TryReleaseAllMembersFromOperation(
        GameState gameState,
        string regionId,
        string operationId
    )
    {
        if (!TryGetState(gameState, out D2Civilization2State state))
            return false;

        D2OperationState operation = GetOperation(GetRegion(state, regionId), operationId);
        return operation != null && TryReleaseMembersFromOperation(
            gameState,
            regionId,
            operationId,
            operation.membersAssigned
        );
    }

    public static int GetUpgradeLevel(D2Civilization2State state, string id)
    {
        D2ResistanceUpgradeState upgrade = FindUpgrade(state, id);
        return upgrade != null ? upgrade.level : 0;
    }

    public static long GetUpgradeCost(int nextLevel)
    {
        switch (nextLevel) { case 1: return 3L; case 2: return 6L; case 3: return 9L; default: return long.MaxValue; }
    }

    public static string GetUpgradeName(string id)
    {
        switch (id) { case RescueUpgradeId: return "Romper Marca Menor"; case CoverageUpgradeId: return "Refugios Sellados"; case EspionageUpgradeId: return "Lectura de Símbolos"; case SabotageUpgradeId: return "Falla en la Cadena"; default: return "Mejora desconocida"; }
    }

    public static string GetUpgradeEffectDescription(string id)
    {
        switch (id)
        {
            case RescueUpgradeId: return "Rescate produce +10% Miembros por nivel.";
            case CoverageUpgradeId: return "Proteccion produce +10% Cobertura por nivel.";
            case EspionageUpgradeId: return "Espionaje reduce 1 punto adicional de perdida por nivel.";
            case SabotageUpgradeId: return "Sabotaje reduce +10% Dominio por nivel.";
            default: return "Mejora desconocida.";
        }
    }

    public static bool TryUpgradeResistance(GameState gameState, string id)
    {
        if (!TryGetState(gameState, out D2Civilization2State state)) return false;
        D2ResistanceUpgradeState upgrade = FindUpgrade(state, id);
        if (upgrade == null || upgrade.level >= MaxUpgradeLevel) return false;
        long cost = GetUpgradeCost(upgrade.level + 1);
        if (state.controlFragments < cost) return false;
        state.controlFragments -= cost; upgrade.level++;
        state.lastResult = GetUpgradeName(id) + " mejorada a nivel " + upgrade.level + ".";
        return true;
    }

    public static D2ResistancePactState GetResistancePact(D2Civilization2State state, string id)
    {
        if (state?.resistancePacts == null) return null;
        foreach (D2ResistancePactState pact in state.resistancePacts) if (pact != null && pact.pactId == id) return pact;
        return null;
    }

    public static string GetResistancePactName(string id)
    {
        switch (id) { case HiddenSheltersPactId: return "Refugios Ocultos"; case SilencedBellsPactId: return "Campanas Silenciadas"; case KnivesPactId: return "Cuchillos Bajo la Mesa"; default: return "Pacto desconocido"; }
    }

    public static long GetPactInitialRequirement(string id)
    {
        switch (id) { case HiddenSheltersPactId: return 30L; case SilencedBellsPactId: return 25L; case KnivesPactId: return 40L; default: return long.MaxValue; }
    }

    public static double GetPactWearInterval(string id) { return id == KnivesPactId ? 90.0 : 120.0; }

    public static string GetPactDescription(string id)
    {
        switch (id)
        {
            case HiddenSheltersPactId:
                return "30 Miembros | -20% perdidas en Represalias | desgaste: 1 cada 120 s.";
            case SilencedBellsPactId:
                return "25 Miembros | -30% duracion del debilitamiento | desgaste: 1 cada 120 s.";
            case KnivesPactId:
                return "40 Miembros | +20% eficacia de Sabotaje | desgaste: 1 cada 90 s.";
            default:
                return "Pacto desconocido.";
        }
    }

    public static bool TryActivateResistancePact(GameState gameState, string id)
    {
        if (!TryGetState(gameState, out D2Civilization2State state)) return false;
        D2ResistancePactState pact = GetResistancePact(state, id);
        long required = GetPactInitialRequirement(id);
        if (pact == null || pact.active || state.membersAvailable < required) return false;
        state.membersAvailable -= required; pact.membersAssigned = required; pact.active = true; pact.wearProgressSeconds = 0.0;
        state.lastResult = GetResistancePactName(id) + " activado."; return true;
    }

    public static bool TryReinforceResistancePact(GameState gameState, string id, long amount)
    {
        if (!TryGetState(gameState, out D2Civilization2State state) || amount <= 0L) return false;
        D2ResistancePactState pact = GetResistancePact(state, id);
        if (pact == null || !pact.active) return false;
        long assigned = Math.Min(amount, state.membersAvailable); if (assigned <= 0L) return false;
        state.membersAvailable -= assigned; pact.membersAssigned = SaturatingAdd(pact.membersAssigned, assigned); return true;
    }

    public static bool TryCancelResistancePact(GameState gameState, string id)
    {
        if (!TryGetState(gameState, out D2Civilization2State state)) return false;
        D2ResistancePactState pact = GetResistancePact(state, id); if (pact == null || !pact.active) return false;
        state.membersAvailable = SaturatingAdd(state.membersAvailable, pact.membersAssigned); pact.membersAssigned = 0L; pact.active = false; pact.wearProgressSeconds = 0.0;
        ApplyPactBreach(state, id); state.lastResult = GetResistancePactName(id) + " incumplido voluntariamente."; return true;
    }

    public static long GetExhaustedMembers(D2Civilization2State state)
    {
        long total = 0L; if (state?.exhaustedMemberBatches == null) return total;
        foreach (D2ExhaustedMemberBatch batch in state.exhaustedMemberBatches) if (batch != null) total = SaturatingAdd(total, Math.Max(0L, batch.amount));
        return total;
    }

    public static double GetExpectedReprisalLossFraction(D2Civilization2State state, D2RegionState region)
    {
        if (region == null)
            return BaseReprisalLossFraction;

        D2OperationState espionage = GetOperation(region, EspionageOperationId);
        double espionageBase = EspionageReprisalReduction + GetUpgradeLevel(state, EspionageUpgradeId) * EspionageUpgradeReductionPerLevel;
        double espionageReduction = IsOperationActive(espionage)
            ? espionageBase
            : region.nextReprisalEspionageReduction;
        double coverageReduction = Math.Floor(
            Math.Clamp(region.coverage, 0.0, MaxCoverage) / CoveragePerLossPoint
        ) * 0.01;
        double majorPactReduction = GetActiveMajorPactLineLevel(
            state, LinkedDefenseLineId) * MajorPactReprisalLossReductionPerLevel;
        double loss = BaseReprisalLossFraction - coverageReduction -
            espionageReduction - majorPactReduction;
        if (region.alertMarked)
            loss += AlertMarkExtraLossFraction;
        D2ResistancePactState shelters = GetResistancePact(state, HiddenSheltersPactId);
        if (shelters != null && shelters.active) loss *= HiddenSheltersLossMultiplier;
        if (state != null && state.hiddenSheltersPenaltySeconds > 0.0) loss *= HiddenSheltersBreachMultiplier;
        return Math.Max(MinimumReprisalLossFraction, loss);
    }

    public static bool IsProtectionActive(D2RegionState region)
    {
        return IsOperationActive(GetOperation(region, ProtectionOperationId));
    }

    public static double GetTimeUntilNextAlertMark(D2Civilization2State state)
    {
        if (state == null || !state.alertActive)
            return AlertMarkIntervalSeconds;
        return Math.Max(0.0, AlertMarkIntervalSeconds - state.alertMarkProgressSeconds);
    }

    public static double GetContainmentSuccessProbability(D2Civilization2State state)
    {
        double dominance = Math.Clamp(GetTotalDominance(state), 0.0, 30.0);
        if (dominance <= 10.0)
            return 1.0 - dominance * 0.03;
        return 0.70 - (dominance - 10.0) * 0.025;
    }

    public static bool CanAttemptContainment(D2Civilization2State state)
    {
        return state != null && state.alertActive && state.containmentAvailable &&
            !state.entityContained && state.containmentCooldownSeconds <= 0.0 &&
            GetTotalDominance(state) <= AlertDominanceThreshold + UnlockComparisonEpsilon;
    }

    public static bool TryAttemptContainment(GameState gameState)
    {
        return TryAttemptContainment(gameState, UnityEngine.Random.value);
    }

    public static bool TryAttemptContainment(GameState gameState, double roll)
    {
        if (!TryGetState(gameState, out D2Civilization2State state) ||
            !CanAttemptContainment(state))
        {
            return false;
        }

        roll = Math.Clamp(roll, 0.0, 0.999999999);
        state.totalContainmentAttempts = SaturatingAdd(state.totalContainmentAttempts, 1L);
        if (roll < GetContainmentSuccessProbability(state))
        {
            state.entityContained = true;
            state.majorPactPrepared = true;
            state.containmentCooldownSeconds = 0.0;
            state.alertMarkProgressSeconds = 0.0;
            foreach (D2RegionState region in state.regions)
                if (region != null) region.alertMarked = false;
            state.lastContainmentResult =
                "Contención exitosa. El pacto mayor de Civilización 2 está preparado.";
            state.lastResult = state.lastContainmentResult;
            return true;
        }

        state.totalContainmentFailures = SaturatingAdd(state.totalContainmentFailures, 1L);
        long totalLosses = 0L;
        foreach (D2RegionState region in state.regions)
        {
            if (region == null || !region.unlocked || region.regionId == Region4Id)
                continue;

            region.threat = Math.Clamp(
                region.threat + ContainmentFailureThreatIncrease,
                0.0,
                100.0
            );
            if (IsProtectionActive(region) || region.membersAssigned <= 0L)
                continue;

            long losses = Math.Max(
                1L,
                (long)Math.Ceiling(region.membersAssigned * ContainmentFailureLossFraction)
            );
            losses = Math.Min(losses, region.membersAssigned);
            RemoveRegionalMembersForReprisal(region, losses);
            totalLosses = SaturatingAdd(totalLosses, losses);
        }

        state.containmentCooldownSeconds = ContainmentCooldownDurationSeconds;
        state.lastContainmentResult =
            "Contención fallida: +20% Amenaza y " + totalLosses.ToString("N0") +
            " Miembro(s) no protegido(s) perdido(s). Reintento en 10 minutos.";
        state.lastResult = state.lastContainmentResult;
        return true;
    }

    public static bool TryAssignMembersToContainment(GameState gameState, long amount)
    {
        if (!TryGetState(gameState, out D2Civilization2State state) ||
            !state.entityContained || amount <= 0L)
        {
            return false;
        }

        long assigned = Math.Min(amount, state.membersAvailable);
        if (assigned <= 0L)
            return false;
        state.membersAvailable -= assigned;
        state.membersAssignedToContainment = SaturatingAdd(
            state.membersAssignedToContainment,
            assigned
        );
        state.lastContainmentResult = assigned.ToString("N0") +
            " Miembro(s) asignado(s) a sostener la Contención.";
        return true;
    }

    public static bool TryReleaseMembersFromContainment(GameState gameState, long amount)
    {
        if (!TryGetState(gameState, out D2Civilization2State state) || amount <= 0L)
            return false;

        long released = Math.Min(amount, state.membersAssignedToContainment);
        if (released <= 0L)
            return false;
        state.membersAssignedToContainment -= released;
        state.membersAvailable = SaturatingAdd(state.membersAvailable, released);
        state.lastContainmentResult = released.ToString("N0") +
            " Miembro(s) retirado(s) del sostenimiento.";
        return true;
    }

    public static bool CanEstablishMajorPact(GameState gameState)
    {
        return TryGetState(gameState, out D2Civilization2State state) &&
            state.entityContained && state.majorPactPrepared &&
            !state.majorPactEstablished;
    }

    public static bool TryEstablishMajorPact(GameState gameState)
    {
        if (!CanEstablishMajorPact(gameState))
            return false;
        D2Civilization2State state = gameState.dimension2.civilization2;
        state.majorPactEstablished = true;
        state.lastMajorPactResult = gameState.IsPrestige1CycleComplete()
            ? "Pacto mayor establecido. Las tres dimensiones están completas; la Convergencia puede prepararse en la Máquina."
            : "Pacto mayor establecido. La Contención ya puede generar Estabilidad.";
        state.lastContainmentResult = state.lastMajorPactResult;
        state.lastResult = state.lastMajorPactResult;
        return true;
    }

    public static int GetMajorPactLineLevel(D2Civilization2State state, string lineId)
    {
        return Math.Clamp(FindMajorPactLine(state, lineId)?.level ?? 0,
            0, MaxMajorPactLineLevel);
    }

    public static double GetMajorPactStabilityCost(int nextLevel)
    {
        return 20.0 * Math.Clamp(nextLevel, 1, MaxMajorPactLineLevel);
    }

    public static long GetMajorPactFragmentCost(int nextLevel)
    {
        return 3L * Math.Clamp(nextLevel, 1, MaxMajorPactLineLevel);
    }

    public static bool CanUpgradeMajorPactLine(GameState gameState, string lineId)
    {
        if (!TryGetState(gameState, out D2Civilization2State state) ||
            !state.majorPactEstablished || !IsMajorPactLineId(lineId))
        {
            return false;
        }
        int nextLevel = GetMajorPactLineLevel(state, lineId) + 1;
        return nextLevel <= MaxMajorPactLineLevel &&
            state.containmentStability >= GetMajorPactStabilityCost(nextLevel) &&
            state.controlFragments >= GetMajorPactFragmentCost(nextLevel);
    }

    public static bool TryUpgradeMajorPactLine(GameState gameState, string lineId)
    {
        if (!CanUpgradeMajorPactLine(gameState, lineId))
            return false;
        D2Civilization2State state = gameState.dimension2.civilization2;
        D2C2MajorPactLineState line = FindMajorPactLine(state, lineId);
        int nextLevel = line.level + 1;
        state.containmentStability -= GetMajorPactStabilityCost(nextLevel);
        state.controlFragments -= GetMajorPactFragmentCost(nextLevel);
        line.level = nextLevel;
        state.lastMajorPactResult = GetMajorPactLineName(lineId) +
            " mejorada a nivel " + nextLevel + ".";
        state.lastContainmentResult = state.lastMajorPactResult;
        state.lastResult = state.lastMajorPactResult;
        return true;
    }

    public static string GetMajorPactLineName(string lineId)
    {
        switch (lineId)
        {
            case ReconstitutedNetworkLineId: return "Red Reconstituida";
            case CoordinatedUprisingLineId: return "Levantamiento Coordinado";
            case LinkedDefenseLineId: return "Defensa Vinculada";
            case ArtifactCustodyLineId: return "Custodia de Artefactos";
            case ResistanceGeometryLineId: return "Geometría de Resistencia";
            default: return "Línea desconocida";
        }
    }

    public static string GetMajorPactLineDescription(string lineId)
    {
        switch (lineId)
        {
            case ReconstitutedNetworkLineId:
                return "+5% generación de Miembros por nivel";
            case CoordinatedUprisingLineId:
                return "+5% reducción de Dominio por nivel";
            case LinkedDefenseLineId:
                return "+5% Cobertura y -1 punto de pérdidas por Represalia por nivel";
            case ArtifactCustodyLineId:
                return "+2% producción de Artefactos por nivel";
            case ResistanceGeometryLineId:
                return "+2% efectividad positiva del Triángulo por nivel";
            default: return "Sin efecto";
        }
    }

    public static double GetMajorPactMemberMultiplier(D2Civilization2State state)
    {
        return 1.0 + GetActiveMajorPactLineLevel(state, ReconstitutedNetworkLineId) *
            MajorPactMemberBonusPerLevel;
    }

    public static double GetMajorPactDominanceMultiplier(D2Civilization2State state)
    {
        return 1.0 + GetActiveMajorPactLineLevel(state, CoordinatedUprisingLineId) *
            MajorPactDominanceBonusPerLevel;
    }

    public static double GetMajorPactCoverageMultiplier(D2Civilization2State state)
    {
        return 1.0 + GetActiveMajorPactLineLevel(state, LinkedDefenseLineId) *
            MajorPactCoverageBonusPerLevel;
    }

    public static double GetMajorPactArtifactMultiplier(D2Civilization2State state)
    {
        return 1.0 + GetActiveMajorPactLineLevel(state, ArtifactCustodyLineId) *
            MajorPactArtifactBonusPerLevel;
    }

    public static double GetMajorPactTriangleMultiplier(D2Civilization2State state)
    {
        return 1.0 + GetActiveMajorPactLineLevel(state, ResistanceGeometryLineId) *
            MajorPactTriangleBonusPerLevel;
    }

    public static bool ValidateState(D2Civilization2State state, out string result)
    {
        if (state == null)
        {
            result = "Estado de Civilización 2 ausente.";
            return false;
        }

        EnsureState(state);
        if (!state.initialMembersGranted || state.membersAvailable < 0L ||
            state.controlFragments < 0L || state.totalReprisals < 0L ||
            state.totalMembersRecruited < GetTotalMembers(state) ||
            !IsSelectableRegion(state, state.selectedRegionId) ||
            state.regions == null || state.regions.Count != RegionIds.Length)
        {
            result = "Estado base de Civilización 2 inválido.";
            return false;
        }

        for (int i = 0; i < RegionIds.Length; i++)
        {
            D2RegionState region = GetRegion(state, RegionIds[i]);
            if (region == null || region.membersAssigned < 0L ||
                region.dominance < 0.0 || region.dominance > 100.0 ||
                region.threat < 0.0 || region.threat > 100.0 ||
                region.coverage < 0.0 || region.coverage > MaxCoverage ||
                region.nextReprisalEspionageReduction < 0.0 ||
                region.nextReprisalEspionageReduction >
                    EspionageReprisalReduction + MaxUpgradeLevel * EspionageUpgradeReductionPerLevel ||
                region.weakenedOperationRemainingSeconds < 0.0 ||
                region.weakenedOperationRemainingSeconds >
                    WeakenedOperationDurationSeconds * SilencedBellsBreachMultiplier ||
                (!string.IsNullOrEmpty(region.weakenedOperationId) &&
                 !IsOperationId(region.weakenedOperationId)) ||
                region.operations == null || region.operations.Count != OperationIds.Length ||
                GetMembersAssignedToOperations(region) > region.membersAssigned)
            {
                result = "Estado regional de Civilización 2 inválido.";
                return false;
            }
        }

        if (state.resistanceUpgrades == null ||
            state.resistanceUpgrades.Count != UpgradeIds.Length ||
            state.resistancePacts == null ||
            state.resistancePacts.Count != ResistancePactIds.Length ||
            state.exhaustedMemberBatches == null ||
            state.hiddenSheltersPenaltySeconds < 0.0 ||
            state.hiddenSheltersPenaltySeconds > PactPenaltySeconds ||
            state.silencedBellsPenaltySeconds < 0.0 ||
            state.silencedBellsPenaltySeconds > PactPenaltySeconds ||
            state.knivesPenaltySeconds < 0.0 ||
            state.knivesPenaltySeconds > PactPenaltySeconds)
        {
            result = "Estado de la RED invalido.";
            return false;
        }


        if (state.alertMarkProgressSeconds < 0.0 ||
            state.alertMarkProgressSeconds > AlertMarkIntervalSeconds ||
            state.totalAlertMarks < 0L ||
            (state.alertActive && !state.containmentAvailable))
        {
            result = "Estado de Alerta invalido.";
            return false;
        }

        if (state.containmentCooldownSeconds < 0.0 ||
            state.containmentCooldownSeconds > ContainmentCooldownDurationSeconds ||
            state.membersAssignedToContainment < 0L ||
            state.totalContainmentAttempts < 0L ||
            state.totalContainmentFailures < 0L ||
            state.totalContainmentFailures > state.totalContainmentAttempts ||
            (state.entityContained && !state.majorPactPrepared) ||
            (!state.entityContained && state.majorPactPrepared) ||
            (state.majorPactEstablished && !state.majorPactPrepared) ||
            state.containmentStability < 0.0 ||
            state.majorPactLines == null ||
            state.majorPactLines.Count != MajorPactLineIds.Length)
        {
            result = "Estado de Contencion invalido.";
            return false;
        }

        foreach (string lineId in MajorPactLineIds)
        {
            D2C2MajorPactLineState line = FindMajorPactLine(state, lineId);
            if (line == null || line.level < 0 || line.level > MaxMajorPactLineLevel ||
                (!state.majorPactEstablished && line.level > 0))
            {
                result = "Estado del pacto mayor de Civilización 2 inválido.";
                return false;
            }
        }

        foreach (string id in UpgradeIds)
        {
            D2ResistanceUpgradeState upgrade = FindUpgrade(state, id);
            if (upgrade == null || upgrade.level < 0 || upgrade.level > MaxUpgradeLevel)
            {
                result = "Mejoras de la RED invalidas.";
                return false;
            }
        }

        foreach (string id in ResistancePactIds)
        {
            D2ResistancePactState pact = GetResistancePact(state, id);
            if (pact == null || pact.membersAssigned < 0L ||
                pact.wearProgressSeconds < 0.0 ||
                pact.wearProgressSeconds >= GetPactWearInterval(id) ||
                (!pact.active && pact.membersAssigned != 0L))
            {
                result = "Pactos de la RED invalidos.";
                return false;
            }
        }

        foreach (D2ExhaustedMemberBatch batch in state.exhaustedMemberBatches)
        {
            if (batch == null || batch.amount <= 0L || batch.remainingSeconds <= 0.0 ||
                batch.remainingSeconds > ExhaustedRecoverySeconds)
            {
                result = "Recuperacion de Miembros de la RED invalida.";
                return false;
            }
        }

        D2RegionState region1 = GetRegion(state, Region1Id);
        if (region1 == null || !region1.unlocked ||
            GetRegion(state, Region4Id).unlocked)
        {
            result = "Desbloqueos iniciales de regiones inválidos.";
            return false;
        }

        result = "Estado base de Civilización 2 válido.";
        return true;
    }

    private static void EnsureRegions(D2Civilization2State state)
    {
        if (state.regions == null)
            state.regions = new List<D2RegionState>();

        for (int i = state.regions.Count - 1; i >= 0; i--)
        {
            D2RegionState region = state.regions[i];
            if (region == null || !IsRegionId(region.regionId) || HasDuplicateBefore(state, i))
                state.regions.RemoveAt(i);
        }

        foreach (string regionId in RegionIds)
        {
            D2RegionState region = GetRegion(state, regionId);
            if (region == null)
            {
                region = new D2RegionState
                {
                    regionId = regionId,
                    unlocked = regionId == Region1Id,
                    dominance = InitialDominance,
                    threat = InitialThreat
                };
                state.regions.Add(region);
            }

            region.dominance = ClampPercent(region.dominance, InitialDominance);
            region.threat = ClampPercent(region.threat, InitialThreat);
            region.membersAssigned = Math.Max(0L, region.membersAssigned);
            region.coverage = ClampValue(region.coverage, 0.0, MaxCoverage, 0.0);
            region.nextReprisalEspionageReduction = ClampValue(
                region.nextReprisalEspionageReduction,
                0.0,
                EspionageReprisalReduction +
                    MaxUpgradeLevel * EspionageUpgradeReductionPerLevel,
                0.0
            );
            region.totalReprisals = Math.Max(0L, region.totalReprisals);
            if (region.weakenedOperationId == null ||
                (!string.IsNullOrEmpty(region.weakenedOperationId) &&
                 !IsOperationId(region.weakenedOperationId)))
            {
                region.weakenedOperationId = "";
            }
            region.weakenedOperationRemainingSeconds = ClampValue(
                region.weakenedOperationRemainingSeconds,
                0.0,
                WeakenedOperationDurationSeconds * SilencedBellsBreachMultiplier,
                0.0
            );
            if (region.weakenedOperationRemainingSeconds <= 0.0)
                region.weakenedOperationId = "";
            EnsureOperations(region);
            if (regionId == Region1Id)
                region.unlocked = true;
            else if (regionId == Region4Id)
                region.unlocked = false;
        }
    }

    private static void EnsureOperations(D2RegionState region)
    {
        if (region.operations == null)
            region.operations = new List<D2OperationState>();

        for (int i = region.operations.Count - 1; i >= 0; i--)
        {
            D2OperationState operation = region.operations[i];
            if (operation == null || !IsOperationId(operation.operationId) ||
                HasDuplicateOperationBefore(region, i))
            {
                region.operations.RemoveAt(i);
            }
        }

        foreach (string operationId in OperationIds)
        {
            D2OperationState operation = GetOperation(region, operationId);
            if (operation == null)
            {
                operation = new D2OperationState { operationId = operationId };
                region.operations.Add(operation);
            }

            operation.membersAssigned = Math.Max(0L, operation.membersAssigned);
        }
    }

    private static void EnsureResistanceProgress(D2Civilization2State state)
    {
        if (state.resistanceUpgrades == null) state.resistanceUpgrades = new List<D2ResistanceUpgradeState>();
        for (int i = state.resistanceUpgrades.Count - 1; i >= 0; i--)
        {
            D2ResistanceUpgradeState upgrade = state.resistanceUpgrades[i];
            if (upgrade == null || Array.IndexOf(UpgradeIds, upgrade.upgradeId) < 0 ||
                HasDuplicateUpgradeBefore(state, i))
            {
                state.resistanceUpgrades.RemoveAt(i);
            }
        }
        foreach (string id in UpgradeIds)
        {
            D2ResistanceUpgradeState upgrade = FindUpgrade(state, id);
            if (upgrade == null) { upgrade = new D2ResistanceUpgradeState { upgradeId = id }; state.resistanceUpgrades.Add(upgrade); }
            upgrade.level = Math.Clamp(upgrade.level, 0, MaxUpgradeLevel);
        }
        if (state.resistancePacts == null) state.resistancePacts = new List<D2ResistancePactState>();
        for (int i = state.resistancePacts.Count - 1; i >= 0; i--)
        {
            D2ResistancePactState pact = state.resistancePacts[i];
            if (pact == null || Array.IndexOf(ResistancePactIds, pact.pactId) < 0 ||
                HasDuplicatePactBefore(state, i))
            {
                state.resistancePacts.RemoveAt(i);
            }
        }
        foreach (string id in ResistancePactIds)
        {
            D2ResistancePactState pact = GetResistancePact(state, id);
            if (pact == null)
            {
                pact = new D2ResistancePactState { pactId = id };
                state.resistancePacts.Add(pact);
            }
            pact.membersAssigned = Math.Max(0L, pact.membersAssigned);
            if (!pact.active)
                pact.membersAssigned = 0L;
            pact.wearProgressSeconds = ClampValue(
                pact.wearProgressSeconds,
                0.0,
                Math.Max(0.0, GetPactWearInterval(id) - 0.000001),
                0.0
            );
        }
        if (state.exhaustedMemberBatches == null) state.exhaustedMemberBatches = new List<D2ExhaustedMemberBatch>();
        for (int i = state.exhaustedMemberBatches.Count - 1; i >= 0; i--)
        {
            D2ExhaustedMemberBatch batch = state.exhaustedMemberBatches[i];
            if (batch == null || batch.amount <= 0L || batch.remainingSeconds <= 0.0)
            {
                state.exhaustedMemberBatches.RemoveAt(i);
                continue;
            }
            batch.remainingSeconds = Math.Min(batch.remainingSeconds, ExhaustedRecoverySeconds);
        }
        state.hiddenSheltersPenaltySeconds = ClampValue(state.hiddenSheltersPenaltySeconds, 0.0, PactPenaltySeconds, 0.0);
        state.silencedBellsPenaltySeconds = ClampValue(state.silencedBellsPenaltySeconds, 0.0, PactPenaltySeconds, 0.0);
        state.knivesPenaltySeconds = ClampValue(state.knivesPenaltySeconds, 0.0, PactPenaltySeconds, 0.0);
        state.alertMarkProgressSeconds = ClampValue(
            state.alertMarkProgressSeconds,
            0.0,
            AlertMarkIntervalSeconds,
            0.0
        );
        state.totalAlertMarks = Math.Max(0L, state.totalAlertMarks);
        state.containmentCooldownSeconds = ClampValue(
            state.containmentCooldownSeconds,
            0.0,
            ContainmentCooldownDurationSeconds,
            0.0
        );
        state.membersAssignedToContainment = Math.Max(0L, state.membersAssignedToContainment);
        state.totalContainmentAttempts = Math.Max(0L, state.totalContainmentAttempts);
        state.totalContainmentFailures = Math.Clamp(
            state.totalContainmentFailures,
            0L,
            state.totalContainmentAttempts
        );
        state.containmentStability = ClampValue(
            state.containmentStability, 0.0, double.MaxValue, 0.0);
        EnsureMajorPactLines(state);
        if (state.entityContained)
        {
            state.majorPactPrepared = true;
            state.containmentCooldownSeconds = 0.0;
            state.alertMarkProgressSeconds = 0.0;
            foreach (D2RegionState region in state.regions)
                if (region != null) region.alertMarked = false;
        }
        else
        {
            state.majorPactPrepared = false;
            state.majorPactEstablished = false;
            state.containmentStability = 0.0;
            foreach (D2C2MajorPactLineState line in state.majorPactLines)
                line.level = 0;
            if (state.membersAssignedToContainment > 0L)
            {
                state.membersAvailable = SaturatingAdd(
                    state.membersAvailable,
                    state.membersAssignedToContainment
                );
                state.membersAssignedToContainment = 0L;
            }
        }
        if (state.alertActive)
            state.containmentAvailable = true;
        else
        {
            state.containmentAvailable = false;
            state.alertMarkProgressSeconds = 0.0;
            foreach (D2RegionState region in state.regions)
                if (region != null) region.alertMarked = false;
        }
    }

    private static void EnsureMajorPactLines(D2Civilization2State state)
    {
        if (state.majorPactLines == null)
            state.majorPactLines = new List<D2C2MajorPactLineState>();
        for (int i = state.majorPactLines.Count - 1; i >= 0; i--)
        {
            D2C2MajorPactLineState line = state.majorPactLines[i];
            if (line == null || !IsMajorPactLineId(line.lineId) ||
                HasEarlierMajorPactLineDuplicate(state, i))
            {
                state.majorPactLines.RemoveAt(i);
            }
        }
        foreach (string lineId in MajorPactLineIds)
        {
            D2C2MajorPactLineState line = FindMajorPactLine(state, lineId);
            if (line == null)
            {
                line = new D2C2MajorPactLineState { lineId = lineId };
                state.majorPactLines.Add(line);
            }
            line.level = Math.Clamp(line.level, 0, MaxMajorPactLineLevel);
            if (!state.majorPactEstablished)
                line.level = 0;
        }
    }

    private static bool IsMajorPactLineId(string lineId)
    {
        return Array.IndexOf(MajorPactLineIds, lineId) >= 0;
    }

    private static D2C2MajorPactLineState FindMajorPactLine(
        D2Civilization2State state,
        string lineId
    )
    {
        if (state?.majorPactLines == null)
            return null;
        foreach (D2C2MajorPactLineState line in state.majorPactLines)
            if (line != null && line.lineId == lineId) return line;
        return null;
    }

    private static bool HasEarlierMajorPactLineDuplicate(
        D2Civilization2State state,
        int index
    )
    {
        string lineId = state.majorPactLines[index].lineId;
        for (int i = 0; i < index; i++)
            if (state.majorPactLines[i] != null &&
                state.majorPactLines[i].lineId == lineId) return true;
        return false;
    }

    private static int GetActiveMajorPactLineLevel(
        D2Civilization2State state,
        string lineId
    )
    {
        return state != null && state.majorPactEstablished
            ? GetMajorPactLineLevel(state, lineId)
            : 0;
    }

    private static D2ResistanceUpgradeState FindUpgrade(D2Civilization2State state, string id)
    {
        if (state?.resistanceUpgrades == null) return null;
        foreach (D2ResistanceUpgradeState upgrade in state.resistanceUpgrades) if (upgrade != null && upgrade.upgradeId == id) return upgrade;
        return null;
    }

    private static bool HasDuplicateUpgradeBefore(D2Civilization2State state, int index)
    {
        string id = state.resistanceUpgrades[index].upgradeId;
        for (int i = 0; i < index; i++)
            if (state.resistanceUpgrades[i] != null && state.resistanceUpgrades[i].upgradeId == id)
                return true;
        return false;
    }

    private static bool HasDuplicatePactBefore(D2Civilization2State state, int index)
    {
        string id = state.resistancePacts[index].pactId;
        for (int i = 0; i < index; i++)
            if (state.resistancePacts[i] != null && state.resistancePacts[i].pactId == id)
                return true;
        return false;
    }

    private static void AdvanceCivilization(GameState gameState, double seconds)
    {
        D2Civilization2State state = gameState.dimension2.civilization2;
        double remaining = seconds;
        while (remaining > 0.000001)
        {
            double step = Math.Min(1.0, remaining);
            bool alertWasActive = state.alertActive;
            AdvancePactsAndRecovery(state, step);
            AdvanceMajorPact(state, step);
            AdvanceOperations(
                state,
                step,
                D2Civilization3System.GetSharedMemoryMultiplier(gameState)
            );
            SyncAlertUnlocks(gameState);
            if (alertWasActive && state.alertActive && !state.entityContained)
                AdvanceAlertMarks(state, step);
            remaining -= step;
        }
    }

    private static void AdvanceAlertMarks(D2Civilization2State state, double seconds)
    {
        state.alertMarkProgressSeconds += seconds;
        while (state.alertMarkProgressSeconds >= AlertMarkIntervalSeconds)
        {
            state.alertMarkProgressSeconds -= AlertMarkIntervalSeconds;
            ResolveAlertMark(state);
        }
    }

    private static void ResolveAlertMark(D2Civilization2State state)
    {
        List<D2RegionState> candidates = new List<D2RegionState>();
        foreach (D2RegionState region in state.regions)
        {
            if (region != null && region.unlocked && region.regionId != Region4Id)
                candidates.Add(region);
        }

        if (candidates.Count == 0)
            return;

        D2RegionState selected = candidates[UnityEngine.Random.Range(0, candidates.Count)];
        state.totalAlertMarks = SaturatingAdd(state.totalAlertMarks, 1L);
        if (IsProtectionActive(selected))
        {
            selected.alertMarked = false;
            state.lastAlertResult = GetRegionDisplayName(selected.regionId) +
                " fue elegida por el Ente, pero Protección mitigó la marca.";
        }
        else
        {
            selected.alertMarked = true;
            state.lastAlertResult = GetRegionDisplayName(selected.regionId) +
                " fue marcada: la próxima Represalia añade 3% de pérdidas.";
        }
        state.lastResult = state.lastAlertResult;
    }

    private static void AdvancePactsAndRecovery(D2Civilization2State state, double seconds)
    {
        state.hiddenSheltersPenaltySeconds = Math.Max(0.0, state.hiddenSheltersPenaltySeconds - seconds);
        state.silencedBellsPenaltySeconds = Math.Max(0.0, state.silencedBellsPenaltySeconds - seconds);
        state.knivesPenaltySeconds = Math.Max(0.0, state.knivesPenaltySeconds - seconds);
        state.containmentCooldownSeconds = Math.Max(
            0.0,
            state.containmentCooldownSeconds - seconds
        );
        for (int i = state.exhaustedMemberBatches.Count - 1; i >= 0; i--)
        {
            D2ExhaustedMemberBatch batch = state.exhaustedMemberBatches[i];
            if (batch == null) { state.exhaustedMemberBatches.RemoveAt(i); continue; }
            batch.remainingSeconds -= seconds;
            if (batch.remainingSeconds <= 0.0) { state.membersAvailable = SaturatingAdd(state.membersAvailable, batch.amount); state.exhaustedMemberBatches.RemoveAt(i); }
        }
        foreach (D2ResistancePactState pact in state.resistancePacts)
        {
            if (pact == null || !pact.active) continue;
            pact.wearProgressSeconds += seconds;
            double interval = GetPactWearInterval(pact.pactId);
            while (pact.wearProgressSeconds >= interval && pact.active)
            {
                pact.wearProgressSeconds -= interval;
                if (pact.membersAssigned > 0L)
                {
                    pact.membersAssigned--;
                    state.exhaustedMemberBatches.Add(new D2ExhaustedMemberBatch { amount = 1L, remainingSeconds = ExhaustedRecoverySeconds });
                }
                if (pact.membersAssigned <= 0L) { pact.active = false; pact.wearProgressSeconds = 0.0; ApplyPactBreach(state, pact.pactId); state.lastResult = GetResistancePactName(pact.pactId) + " incumplido por desgaste."; }
            }
        }
    }

    private static void ApplyPactBreach(D2Civilization2State state, string id)
    {
        if (id == HiddenSheltersPactId) state.hiddenSheltersPenaltySeconds = PactPenaltySeconds;
        else if (id == SilencedBellsPactId) state.silencedBellsPenaltySeconds = PactPenaltySeconds;
        else if (id == KnivesPactId) state.knivesPenaltySeconds = PactPenaltySeconds;
    }

    private static void AdvanceMajorPact(D2Civilization2State state, double seconds)
    {
        if (!state.majorPactEstablished || state.membersAssignedToContainment <= 0L ||
            seconds <= 0.0)
        {
            return;
        }
        double gained = Math.Sqrt(state.membersAssignedToContainment) *
            ContainmentStabilityPerMinuteFactor * (seconds / 60.0);
        if (double.IsInfinity(gained) || state.containmentStability >= double.MaxValue - gained)
            state.containmentStability = double.MaxValue;
        else
            state.containmentStability += gained;
    }

    private static void AdvanceOperations(
        D2Civilization2State state,
        double seconds,
        double positiveOutputMultiplier
    )
    {
        if (seconds <= 0.0 || double.IsNaN(seconds) || double.IsInfinity(seconds))
            return;

        foreach (D2RegionState region in state.regions)
        {
            if (region == null || !region.unlocked || region.regionId == Region4Id)
                continue;

            double remainingSeconds = seconds;
            int eventGuard = 0;
            while (remainingSeconds > 0.000001 && eventGuard++ < 512)
            {
                if (region.threat >= 100.0 - 0.000001)
                {
                    TriggerReprisal(state, region);
                    continue;
                }

                OperationRates rates = GetOperationRates(state, region);
                rates.dominanceReductionPerMinute *= positiveOutputMultiplier *
                    GetMajorPactDominanceMultiplier(state);
                rates.memberProductionPerMinute *= positiveOutputMultiplier *
                    GetMajorPactMemberMultiplier(state);
                rates.coverageProductionPerMinute *= positiveOutputMultiplier *
                    GetMajorPactCoverageMultiplier(state);
                double secondsToReprisal = double.PositiveInfinity;
                if (rates.threatChangePerMinute > 0.0)
                {
                    secondsToReprisal = Math.Max(
                        0.0,
                        (100.0 - region.threat) /
                        rates.threatChangePerMinute * 60.0
                    );
                }

                double stepSeconds = Math.Min(remainingSeconds, secondsToReprisal);
                if (region.weakenedOperationRemainingSeconds > 0.0)
                {
                    stepSeconds = Math.Min(
                        stepSeconds,
                        region.weakenedOperationRemainingSeconds
                    );
                }

                if (double.IsInfinity(stepSeconds))
                    stepSeconds = remainingSeconds;
                if (stepSeconds <= 0.000001)
                {
                    ClearExpiredWeakening(region);
                    if (secondsToReprisal <= 0.000001)
                        TriggerReprisal(state, region);
                    else
                        break;
                    continue;
                }

                ApplyContinuousProgress(state, region, rates, stepSeconds);
                UpdateRegionUnlocks(state);
                remainingSeconds -= stepSeconds;

                if (region.weakenedOperationRemainingSeconds > 0.0)
                {
                    region.weakenedOperationRemainingSeconds = Math.Max(
                        0.0,
                        region.weakenedOperationRemainingSeconds - stepSeconds
                    );
                    ClearExpiredWeakening(region);
                }

                if (region.threat >= 100.0 - 0.000001)
                    TriggerReprisal(state, region);
            }
        }

        CompleteProducedMembers(state);
    }

    private static OperationRates GetOperationRates(D2Civilization2State state, D2RegionState region)
    {
        OperationRates rates = new OperationRates();
        foreach (D2OperationState operation in region.operations)
        {
            if (!IsOperationActive(operation))
                continue;

            double multiplier = operation.operationId == region.weakenedOperationId &&
                region.weakenedOperationRemainingSeconds > 0.0
                ? WeakenedOperationMultiplier
                : 1.0;

            switch (operation.operationId)
            {
                case RescueOperationId:
                    rates.dominanceReductionPerMinute += RescueDominancePerMinute * multiplier;
                    rates.threatChangePerMinute += RescueThreatPerMinute * multiplier *
                        (state.alertActive && !state.entityContained ? AlertThreatMultiplier : 1.0);
                    rates.memberProductionPerMinute += Math.Sqrt(operation.membersAssigned) *
                        RescueMemberFactorPerMinute * multiplier * (1.0 + GetUpgradeLevel(state, RescueUpgradeId) * UpgradeBonusPerLevel);
                    break;
                case ProtectionOperationId:
                    rates.dominanceReductionPerMinute += ProtectionDominancePerMinute * multiplier;
                    rates.threatChangePerMinute += ProtectionThreatPerMinute * multiplier;
                    rates.memberProductionPerMinute += Math.Sqrt(operation.membersAssigned) *
                        ProtectionMemberFactorPerMinute * multiplier;
                    rates.coverageProductionPerMinute += Math.Sqrt(operation.membersAssigned) *
                        ProtectionCoverageFactorPerMinute * multiplier * (1.0 + GetUpgradeLevel(state, CoverageUpgradeId) * UpgradeBonusPerLevel);
                    break;
                case EspionageOperationId:
                    rates.dominanceReductionPerMinute += EspionageDominancePerMinute * multiplier;
                    rates.threatChangePerMinute += EspionageThreatPerMinute * multiplier *
                        (state.alertActive && !state.entityContained ? AlertThreatMultiplier : 1.0);
                    rates.espionageActive = true;
                    break;
                case SabotageOperationId:
                    D2ResistancePactState knives = GetResistancePact(state, KnivesPactId);
                    double sabotageMultiplier = 1.0 + GetUpgradeLevel(state, SabotageUpgradeId) * UpgradeBonusPerLevel;
                    if (knives != null && knives.active) sabotageMultiplier *= KnivesSabotageMultiplier;
                    rates.dominanceReductionPerMinute += SabotageDominancePerMinute * multiplier * sabotageMultiplier;
                    double threatMultiplier = state.knivesPenaltySeconds > 0.0 ? KnivesBreachThreatMultiplier : 1.0;
                    rates.threatChangePerMinute += SabotageThreatPerMinute * multiplier *
                        threatMultiplier *
                        (state.alertActive && !state.entityContained ? AlertThreatMultiplier : 1.0);
                    break;
            }
        }

        return rates;
    }

    private static void ApplyContinuousProgress(
        D2Civilization2State state,
        D2RegionState region,
        OperationRates rates,
        double seconds
    )
    {
        double minutes = seconds / 60.0;
        region.dominance = Math.Clamp(
            region.dominance - rates.dominanceReductionPerMinute * minutes,
            0.0,
            100.0
        );
        region.threat = Math.Clamp(
            region.threat + rates.threatChangePerMinute * minutes,
            0.0,
            100.0
        );
        region.coverage = Math.Clamp(
            region.coverage + rates.coverageProductionPerMinute * minutes,
            0.0,
            MaxCoverage
        );
        if (rates.espionageActive)
        {
            region.nextReprisalEspionageReduction = EspionageReprisalReduction +
                GetUpgradeLevel(state, EspionageUpgradeId) * EspionageUpgradeReductionPerLevel;
        }
        state.memberProductionProgress += rates.memberProductionPerMinute * minutes;
    }

    private static void CompleteProducedMembers(D2Civilization2State state)
    {
        long completedMembers = (long)Math.Floor(state.memberProductionProgress);
        if (completedMembers <= 0L)
            return;

        state.memberProductionProgress -= completedMembers;
        state.membersAvailable = SaturatingAdd(state.membersAvailable, completedMembers);
        state.totalMembersRecruited = SaturatingAdd(
            state.totalMembersRecruited,
            completedMembers
        );
    }

    private static void TriggerReprisal(
        D2Civilization2State state,
        D2RegionState region
    )
    {
        double lossFraction = GetExpectedReprisalLossFraction(state, region);
        if (state.hiddenSheltersPenaltySeconds > 0.0)
            state.hiddenSheltersPenaltySeconds = 0.0;
        long losses = region.membersAssigned > 0L
            ? Math.Max(1L, (long)Math.Ceiling(region.membersAssigned * lossFraction))
            : 0L;
        losses = Math.Min(losses, region.membersAssigned);
        region.alertMarked = false;

        RemoveRegionalMembersForReprisal(region, losses);
        region.coverage *= CoverageRetentionAfterReprisal;
        region.nextReprisalEspionageReduction = 0.0;
        region.threat = ThreatAfterReprisal;
        region.totalReprisals = SaturatingAdd(region.totalReprisals, 1L);
        state.totalReprisals = SaturatingAdd(state.totalReprisals, 1L);
        long fragmentsReward = state.alertActive
            ? AlertControlFragmentsPerReprisal
            : ControlFragmentsPerReprisal;
        state.controlFragments = SaturatingAdd(state.controlFragments, fragmentsReward);

        List<string> activeOperationIds = new List<string>();
        foreach (D2OperationState operation in region.operations)
        {
            if (IsOperationActive(operation))
                activeOperationIds.Add(operation.operationId);
        }

        if (activeOperationIds.Count > 0)
        {
            int selectedIndex = UnityEngine.Random.Range(0, activeOperationIds.Count);
            region.weakenedOperationId = activeOperationIds[selectedIndex];
            double duration = WeakenedOperationDurationSeconds;
            D2ResistancePactState bells = GetResistancePact(state, SilencedBellsPactId);
            if (bells != null && bells.active) duration *= SilencedBellsDurationMultiplier;
            if (state.silencedBellsPenaltySeconds > 0.0) { duration *= SilencedBellsBreachMultiplier; state.silencedBellsPenaltySeconds = 0.0; }
            region.weakenedOperationRemainingSeconds = duration;
        }
        else
        {
            region.weakenedOperationId = "";
            region.weakenedOperationRemainingSeconds = 0.0;
        }

        state.lastResult = "Represalia en " + GetRegionDisplayName(region.regionId) +
            ": " + losses.ToString("N0") + " Miembro(s) perdido(s), +" +
            fragmentsReward.ToString("N0") + " Fragmentos de Control.";
    }

    private static void RemoveRegionalMembersForReprisal(
        D2RegionState region,
        long losses
    )
    {
        if (region == null || losses <= 0L)
            return;

        long idleLosses = Math.Min(losses, GetRegionIdleMembers(region));
        long operationLosses = losses - idleLosses;
        if (operationLosses > 0L)
        {
            long operationTotal = GetMembersAssignedToOperations(region);
            long originalOperationLosses = operationLosses;
            foreach (D2OperationState operation in region.operations)
            {
                if (operation == null || operation.membersAssigned <= 0L || operationTotal <= 0L)
                    continue;

                long proportionalLoss = (long)Math.Floor(
                    (double)originalOperationLosses * operation.membersAssigned / operationTotal
                );
                proportionalLoss = Math.Min(proportionalLoss, operation.membersAssigned);
                operation.membersAssigned -= proportionalLoss;
                operationLosses -= proportionalLoss;
            }

            while (operationLosses > 0L)
            {
                D2OperationState largest = null;
                foreach (D2OperationState operation in region.operations)
                {
                    if (operation != null && operation.membersAssigned > 0L &&
                        (largest == null || operation.membersAssigned > largest.membersAssigned))
                    {
                        largest = operation;
                    }
                }

                if (largest == null)
                    break;

                long removed = Math.Min(operationLosses, largest.membersAssigned);
                largest.membersAssigned -= removed;
                operationLosses -= removed;
            }
        }

        region.membersAssigned = Math.Max(0L, region.membersAssigned - losses);
    }

    private static void ClearExpiredWeakening(D2RegionState region)
    {
        if (region.weakenedOperationRemainingSeconds > 0.000001)
            return;

        region.weakenedOperationRemainingSeconds = 0.0;
        region.weakenedOperationId = "";
    }

    private struct OperationRates
    {
        public double dominanceReductionPerMinute;
        public double threatChangePerMinute;
        public double memberProductionPerMinute;
        public double coverageProductionPerMinute;
        public bool espionageActive;
    }

    private static bool TryGetState(GameState gameState, out D2Civilization2State state)
    {
        state = null;
        if (gameState?.dimension2 == null || !gameState.dimension2.civilization2Unlocked)
            return false;

        state = gameState.dimension2.civilization2;
        EnsureState(state);
        return state != null;
    }

    private static bool IsRegionId(string regionId)
    {
        foreach (string id in RegionIds)
        {
            if (id == regionId)
                return true;
        }

        return false;
    }

    private static bool IsOperationId(string operationId)
    {
        foreach (string id in OperationIds)
        {
            if (id == operationId)
                return true;
        }

        return false;
    }

    private static bool HasDuplicateBefore(D2Civilization2State state, int index)
    {
        D2RegionState current = state.regions[index];
        for (int i = 0; i < index; i++)
        {
            if (state.regions[i]?.regionId == current.regionId)
                return true;
        }

        return false;
    }

    private static bool HasDuplicateOperationBefore(D2RegionState region, int index)
    {
        D2OperationState current = region.operations[index];
        for (int i = 0; i < index; i++)
        {
            if (region.operations[i]?.operationId == current.operationId)
                return true;
        }

        return false;
    }

    private static double ClampPercent(double value, double fallback)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            return fallback;
        return Math.Clamp(value, 0.0, 100.0);
    }

    private static double ClampValue(
        double value,
        double minimum,
        double maximum,
        double fallback
    )
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            return fallback;
        return Math.Clamp(value, minimum, maximum);
    }

    private static long SaturatingAdd(long left, long right)
    {
        if (right > 0L && left > long.MaxValue - right)
            return long.MaxValue;
        return left + right;
    }
}

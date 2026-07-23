using System;
using System.Collections.Generic;


public static class D3ConsoleSystem
{
    public const string PolicyLE = "le";
    public const string PolicyTraces = "traces";
    public const string PolicyBalanced = "balanced";
    public const string BuildingHiggs = "vacuum_observer";
    public const string BuildingTetraquark = "casimir_panel";

    public static void NormalizeSettings(D3ConsoleSettingsState settings)
    {
        if (settings == null) return;
        if (settings.purchasePolicy != PolicyLE &&
            settings.purchasePolicy != PolicyTraces &&
            settings.purchasePolicy != PolicyBalanced)
            settings.purchasePolicy = PolicyBalanced;
        settings.leReserve = Math.Max(0.0, settings.leReserve);
        settings.tracesReserve = Math.Max(0.0, settings.tracesReserve);
        if (settings.manuallyPurchasedBuildingIds == null)
            settings.manuallyPurchasedBuildingIds = new List<string>();
        if (settings.manuallySelectedModulatorModes == null)
            settings.manuallySelectedModulatorModes = new List<int>();
        if (settings.manualTrianglePresets == null)
            settings.manualTrianglePresets = new List<D3TrianglePresetState>();
        for (int i = settings.manualTrianglePresets.Count - 1; i >= 0; i--)
        {
            D3TrianglePresetState preset = settings.manualTrianglePresets[i];
            if (preset == null || string.IsNullOrWhiteSpace(preset.presetId))
                settings.manualTrianglePresets.RemoveAt(i);
        }
    }

    public static void RecordManualBuildingPurchase(
        GameState gameState, string buildingId)
    {
        if (gameState == null || !IsRepeatableBuilding(buildingId)) return;
        Dimension3System.EnsureState(gameState);
        List<string> history = gameState.dimension3.consoleSettings
            .manuallyPurchasedBuildingIds;
        if (!history.Contains(buildingId)) history.Add(buildingId);
    }

    public static void RecordManualModulatorMode(
        GameState gameState, PhaseModulatorMode mode)
    {
        if (gameState == null || mode == PhaseModulatorMode.None) return;
        if (mode == PhaseModulatorMode.Attunement && !gameState.IsAttunementUnlocked())
            return;
        Dimension3System.EnsureState(gameState);
        List<int> history = gameState.dimension3.consoleSettings
            .manuallySelectedModulatorModes;
        int value = (int)mode;
        if (!history.Contains(value)) history.Add(value);
    }

    public static void RecordManualTriangleConfiguration(GameState gameState)
    {
        if (gameState == null || !gameState.IsTriangleFullyConfiguredWithBaseArtifacts())
            return;
        Dimension3System.EnsureState(gameState);
        D3ConsoleSettingsState settings = gameState.dimension3.consoleSettings;
        string id = BuildPresetId(gameState.trianglePrimaryBuildingId,
            gameState.triangleReinforcementBuildingId,
            gameState.triangleAlterationBuildingId);
        if (GetPreset(settings, id) != null) return;
        settings.manualTrianglePresets.Add(new D3TrianglePresetState
        {
            presetId = id,
            primaryBuildingId = gameState.trianglePrimaryBuildingId,
            reinforcementBuildingId = gameState.triangleReinforcementBuildingId,
            alterationBuildingId = gameState.triangleAlterationBuildingId
        });
    }

    public static bool HasManualBuildingAuthorization(
        Dimension3State state, string buildingId)
    {
        return state != null && state.consoleSettings != null &&
            state.consoleSettings.manuallyPurchasedBuildingIds != null &&
            state.consoleSettings.manuallyPurchasedBuildingIds.Contains(buildingId);
    }

    public static bool HasManualPhaseAuthorization(
        GameState gameState, PhaseModulatorMode mode)
    {
        return gameState != null && gameState.dimension3 != null &&
            gameState.dimension3.consoleSettings != null &&
            gameState.dimension3.consoleSettings.manuallySelectedModulatorModes != null &&
            gameState.dimension3.consoleSettings.manuallySelectedModulatorModes
                .Contains((int)mode);
    }

    public static bool TrySetPolicyAndReserves(
        GameState gameState, string policy, double leReserve,
        double tracesReserve, out string reason)
    {
        reason = "";
        if (!CanUseFunction(gameState, 2, out reason)) return false;
        if ((policy != PolicyLE && policy != PolicyTraces &&
             policy != PolicyBalanced) || leReserve < 0.0 || tracesReserve < 0.0)
        {
            reason = "Política o reservas inválidas.";
            return false;
        }
        D3ConsoleSettingsState settings = gameState.dimension3.consoleSettings;
        settings.purchasePolicy = policy;
        settings.leReserve = leReserve;
        settings.tracesReserve = tracesReserve;
        return true;
    }

    public static bool TryPurchaseRepeatableBuilding(
        GameState gameState, string buildingId, double routineLEReserve,
        double routineTracesReserve, out string reason)
    {
        reason = "";
        if (!CanUseFunction(gameState, 1, out reason)) return false;
        if (!IsRepeatableBuilding(buildingId) ||
            !HasManualBuildingAuthorization(gameState.dimension3, buildingId))
        {
            reason = "La compra repetible no posee autorización manual.";
            return false;
        }
        if (!PolicyAllows(gameState, buildingId))
        {
            reason = "La compra espera por la política seleccionada.";
            return false;
        }
        BuildingState building = gameState.GetBuildingState(buildingId);
        if (building == null || building.def == null ||
            !IsBuildingUnlocked(gameState, building) || building.IsAtMaxLevel())
        {
            reason = "El artefacto repetible no está disponible.";
            return false;
        }
        D3ConsoleSettingsState settings = gameState.dimension3.consoleSettings;
        double leReserve = Math.Max(settings.leReserve, routineLEReserve);
        double tracesReserve = Math.Max(settings.tracesReserve, routineTracesReserve);
        double cost = GetAutomatedBuildingCost(gameState, building);
        if (gameState.LE + 0.000001 < cost + leReserve ||
            gameState.Traces + 0.000001 < tracesReserve)
        {
            reason = "La compra respetó las reservas mínimas.";
            return false;
        }
        gameState.LE -= cost;
        building.OnPurchased();
        reason = "Compra automática: " + buildingId + " nivel " + building.level +
            " por " + Math.Ceiling(cost) + " LE.";
        return true;
    }

    public static double GetAutomatedBuildingCost(
        GameState gameState, BuildingState building)
    {
        if (gameState == null || building == null) return double.PositiveInfinity;
        double baseCost = gameState.GetEffectiveBuildingCost(building);
        double rawCostPower = D3PowerSystem.GetRawChannelPower(
            gameState.dimension3, Dimension3Catalog.FacilityProductionConsole,
            Dimension3Catalog.ChannelConsoleCost);
        double rawCoordination = D3PowerSystem.GetRawChannelPower(
            gameState.dimension3, Dimension3Catalog.FacilityProductionConsole,
            Dimension3Catalog.ChannelConsoleCoordination);
        double coordination = 1.0 + Math.Sqrt(rawCoordination) * 0.25 / 100.0;
        double bonus = Math.Sqrt(rawCostPower) * 0.10 * coordination *
            D3FacilitySystem.GetAutomationCoreEfficiencyMultiplier(gameState.dimension3);
        return Math.Ceiling(Math.Max(0.0, baseCost) / (1.0 + bonus / 100.0));
    }

    public static bool TryApplyPreferredPhase(
        GameState gameState, PhaseModulatorMode mode, out string reason)
    {
        reason = "";
        if (!CanUseFunction(gameState, 4, out reason)) return false;
        if (mode == PhaseModulatorMode.None ||
            !HasManualPhaseAuthorization(gameState, mode) ||
            !gameState.IsPhaseModulatorOwned() ||
            (mode == PhaseModulatorMode.Attunement && !gameState.IsAttunementUnlocked()))
        {
            reason = "La fase no está desbloqueada o no tiene historial manual.";
            return false;
        }
        gameState.dimension3.consoleSettings.preferredModulatorMode = (int)mode;
        if (gameState.phaseModulatorMode != mode)
            gameState.SetPhaseModulatorMode(mode);
        reason = "Fase preferida mantenida: " + mode + ".";
        return true;
    }

    public static bool TryApplyTrianglePreset(
        GameState gameState, string presetId, out string reason)
    {
        reason = "";
        if (!CanUseFunction(gameState, 5, out reason)) return false;
        D3ConsoleSettingsState settings = gameState.dimension3.consoleSettings;
        D3TrianglePresetState preset = GetPreset(settings, presetId);
        if (preset == null || !gameState.triangleSystemUnlocked ||
            gameState.GetBuildingLevel(BuildingHiggs) <= 0 ||
            gameState.GetBuildingLevel(BuildingTetraquark) <= 0 ||
            gameState.GetBuildingLevel("fluctuation_antenna") <= 0)
        {
            reason = "La configuración básica no está disponible o no fue aplicada manualmente.";
            return false;
        }
        gameState.ClearTriangleConfiguration();
        bool applied = gameState.AssignTriangleBuilding(TriangleSlotRole.Primary,
                preset.primaryBuildingId, false) &&
            gameState.AssignTriangleBuilding(TriangleSlotRole.Reinforcement,
                preset.reinforcementBuildingId, false) &&
            gameState.AssignTriangleBuilding(TriangleSlotRole.Alteration,
                preset.alterationBuildingId, false);
        if (!applied || !gameState.IsTriangleFullyConfiguredWithBaseArtifacts())
        {
            reason = "No se pudo aplicar la configuración básica.";
            return false;
        }
        settings.preferredTrianglePresetId = presetId;
        reason = "Configuración básica del Triángulo aplicada.";
        return true;
    }

    public static D3TrianglePresetState GetPreset(
        D3ConsoleSettingsState settings, string presetId)
    {
        if (settings == null || settings.manualTrianglePresets == null) return null;
        for (int i = 0; i < settings.manualTrianglePresets.Count; i++)
        {
            D3TrianglePresetState preset = settings.manualTrianglePresets[i];
            if (preset != null && preset.presetId == presetId) return preset;
        }
        return null;
    }

    public static bool IsTrianglePresetApplied(
        GameState gameState, D3TrianglePresetState preset)
    {
        return gameState != null && preset != null &&
            gameState.trianglePrimaryBuildingId == preset.primaryBuildingId &&
            gameState.triangleReinforcementBuildingId == preset.reinforcementBuildingId &&
            gameState.triangleAlterationBuildingId == preset.alterationBuildingId;
    }

    public static D3ConsoleSettingsState CloneSettings(D3ConsoleSettingsState source)
    {
        var clone = new D3ConsoleSettingsState();
        if (source == null) return clone;
        clone.purchasePolicy = source.purchasePolicy;
        clone.leReserve = source.leReserve;
        clone.tracesReserve = source.tracesReserve;
        clone.preferredModulatorMode = source.preferredModulatorMode;
        clone.preferredTrianglePresetId = source.preferredTrianglePresetId ?? "";
        clone.manuallyPurchasedBuildingIds.AddRange(source.manuallyPurchasedBuildingIds);
        clone.manuallySelectedModulatorModes.AddRange(source.manuallySelectedModulatorModes);
        for (int i = 0; i < source.manualTrianglePresets.Count; i++)
        {
            D3TrianglePresetState item = source.manualTrianglePresets[i];
            if (item == null) continue;
            clone.manualTrianglePresets.Add(new D3TrianglePresetState
            {
                presetId = item.presetId,
                primaryBuildingId = item.primaryBuildingId,
                reinforcementBuildingId = item.reinforcementBuildingId,
                alterationBuildingId = item.alterationBuildingId
            });
        }
        return clone;
    }

    public static void MergeManualHistory(
        D3ConsoleSettingsState target, D3ConsoleSettingsState source)
    {
        if (target == null || source == null) return;
        NormalizeSettings(target);
        NormalizeSettings(source);
        for (int i = 0; i < source.manuallyPurchasedBuildingIds.Count; i++)
        {
            string id = source.manuallyPurchasedBuildingIds[i];
            if (!target.manuallyPurchasedBuildingIds.Contains(id))
                target.manuallyPurchasedBuildingIds.Add(id);
        }
        for (int i = 0; i < source.manuallySelectedModulatorModes.Count; i++)
        {
            int mode = source.manuallySelectedModulatorModes[i];
            if (!target.manuallySelectedModulatorModes.Contains(mode))
                target.manuallySelectedModulatorModes.Add(mode);
        }
        for (int i = 0; i < source.manualTrianglePresets.Count; i++)
        {
            D3TrianglePresetState preset = source.manualTrianglePresets[i];
            if (preset != null && GetPreset(target, preset.presetId) == null)
                target.manualTrianglePresets.Add(new D3TrianglePresetState
                {
                    presetId = preset.presetId,
                    primaryBuildingId = preset.primaryBuildingId,
                    reinforcementBuildingId = preset.reinforcementBuildingId,
                    alterationBuildingId = preset.alterationBuildingId
                });
        }
    }

    private static bool CanUseFunction(
        GameState gameState, int level, out string reason)
    {
        reason = "";
        if (!Dimension3System.CanAccessDimension3(gameState))
        {
            reason = "Dimensión 3 está bloqueada.";
            return false;
        }
        Dimension3System.EnsureState(gameState);
        if (!D3FacilitySystem.IsFunctionActive(gameState.dimension3,
                Dimension3Catalog.FacilityProductionConsole, level))
        {
            reason = "La Consola no posee nivel y capacidad activos para esta función.";
            return false;
        }
        return true;
    }

    private static bool PolicyAllows(GameState gameState, string buildingId)
    {
        string policy = gameState.dimension3.consoleSettings.purchasePolicy;
        if (policy == PolicyLE) return buildingId == BuildingHiggs;
        if (policy == PolicyTraces) return buildingId == BuildingTetraquark;
        int higgs = gameState.GetBuildingLevel(BuildingHiggs);
        int tetra = gameState.GetBuildingLevel(BuildingTetraquark);
        return buildingId == BuildingHiggs ? higgs <= tetra : tetra <= higgs;
    }

    private static bool IsRepeatableBuilding(string buildingId)
    {
        return buildingId == BuildingHiggs || buildingId == BuildingTetraquark;
    }

    private static bool IsBuildingUnlocked(
        GameState gameState, BuildingState building)
    {
        if (building.level > 0) return true;
        BuildingDef definition = building.def;
        if (!string.IsNullOrEmpty(definition.unlockRequireId) &&
            definition.unlockRequireLevel > 0)
            return gameState.GetBuildingLevel(definition.unlockRequireId) >=
                definition.unlockRequireLevel;
        if (definition.unlockMinLE > 0.0 && gameState.LE < definition.unlockMinLE)
            return false;
        return definition.unlockMinTotalLEps <= 0.0 ||
            gameState.GetTotalLEps() >= definition.unlockMinTotalLEps;
    }

    private static string BuildPresetId(
        string primary, string reinforcement, string alteration)
    {
        return "triangle_" + primary + "_" + reinforcement + "_" + alteration;
    }
}

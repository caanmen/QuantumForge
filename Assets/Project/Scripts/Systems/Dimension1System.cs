using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class D1MetalAmount
{
    public string metalId;
    public double amount;
}

[System.Serializable]
public class D1PlanetState
{
    public string planetId;
    public bool unlocked;
    public int extractorTier;
}

[System.Serializable]
public class D1ShipState
{
    public string shipId;
    public bool unlocked;

    // Preparado para más adelante:
    // exploración simple con 1 nave.
    public bool explorationActive;
    public string activeDestinationId;
    public double explorationRemainingSeconds;
    public double explorationTotalSeconds;
    public int cargoLevel;
    public int speedLevel;
    public int armorLevel;
    public int sensorsLevel;

}

[System.Serializable]
public class D1ScannedDestinationState
{
    public string destinationId;
    public bool available;
}

[System.Serializable]
public class D1ExplorationRecordEntry
{
    public int resultId;
    public string shipId;
    public string destinationId;
    public List<D1MetalAmount> rewards = new List<D1MetalAmount>();
    public int blueprintFragments;
    public List<D1BlueprintAmount> specificBlueprintRewards = new List<D1BlueprintAmount>();
}

[System.Serializable]
public class D1BlueprintAmount
{
    public string blueprintId;
    public int amount;
}

[System.Serializable]
public class D1RelicState
{
    public string relicId;
    public bool unlocked;
    public int level;
}

[System.Serializable]
public class D1TreeNodeState
{
    public string nodeId;
    public int tier;
}

public static class Dimension1System
{
    public const string DimensionId = "dimension_01";

    // Metales de Dimensión 1
    public const string MetalIron = "metal_iron";
    public const string MetalCopper = "metal_copper";
    public const string MetalAluminum = "metal_aluminum";
    public const string MetalTitanium = "metal_titanium";
    public const string MetalNickel = "metal_nickel";
    public const string MetalCobalt = "metal_cobalt";
    public const string MetalLithium = "metal_lithium";
    public const string MetalTungsten = "metal_tungsten";
    public const string MetalPlatinum = "metal_platinum";
    public const string MetalIridium = "metal_iridium";
    private const int MaxRecentExplorationRecords = 20;

    // Planetas de Dimensión 1
    public const string Planet01 = "planet_01";
    public const string Planet02 = "planet_02";
    public const string Planet03 = "planet_03";
    public const string Planet04 = "planet_04";
    public const string Planet05 = "planet_05";
    public const string Planet06 = "planet_06";
    public const string Planet07 = "planet_07";

    // Naves de Dimensión 1
    public const string ShipLightProbe = "ship_light_probe";
    public const string ShipExtractorDrone = "ship_extractor_drone";
    public const string ShipAnalyticProbe = "ship_analytic_probe";
    public const string ShipCargoShip = "ship_cargo_ship";
    public const string ShipRescueShip = "ship_rescue_ship";
    public const string ShipConvergenceShip = "ship_convergence_ship";

    // ID antiguo usado solo para migrar partidas viejas.
    public const string LegacyCargoShipId = "ship_survey_corvette";

    // Partes de nave
    public const string ShipPartCargo = "cargo";
    public const string ShipPartSpeed = "speed";
    public const string ShipPartArmor = "armor";
    public const string ShipPartSensors = "sensors";

    // Destinos normales oficiales de Dimensión 1
    public const string DestinationMineralBelt = "destination_mineral_belt";
    public const string DestinationShipGraveyard = "destination_ship_graveyard";
    public const string DestinationAbandonedShip = "destination_abandoned_ship";
    public const string DestinationOrbitalRuin = "destination_orbital_ruin";
    public const string DestinationDriftingProbes = "destination_drifting_probes";
    public const string DestinationLaboratory = "destination_laboratory";
    public const string DestinationAbandonedStation = "destination_abandoned_station";
    public const string DestinationMinorAnomaly = "destination_minor_anomaly";
    public const string DestinationAncientStructure = "destination_ancient_structure";
    public const string DestinationUnstableZone = "destination_unstable_zone";

    // IDs antiguos/provisionales. No se generan en nuevos escaneos.
    public const string DestinationAbandonedProbe = "destination_abandoned_probe";
    public const string DestinationCrystalDebris = "destination_crystal_debris";
    public const string DestinationOrbitalArmorWreckage = "destination_orbital_armor_wreckage";
    public const string DestinationCollapsedReactor = "destination_collapsed_reactor";

    // Offline inicial: 12 horas
    public const double DefaultOfflineCapSeconds = 43200.0;

    // Barrido base del escáner de Dimensión 1.
    public const double SimpleScanDurationSeconds = 5.0;
    public const int SimpleScanBaseDestinationCount = 2;
    public const int SimpleScanMaxDestinationCount = 5;
    public const int SimpleScannerMaxLevel = 3;
    public const int BlueprintFragmentsPerBlueprint = 10;

    public const int Dimension1RelicMaxLevel = 150;
    public const int Dimension1RelicMilestoneStep = 25;

    // Reliquias de Exploración
    public const string RelicDriftCompass = "relic_drift_compass";
    public const string RelicAncientCargoCore = "relic_ancient_cargo_core";
    public const string RelicLostNavigationRecord = "relic_lost_navigation_record";
    public const string RelicExpeditionSeal = "relic_expedition_seal";
    public const string RelicDormantEcho = "relic_dormant_echo";

    // Reliquias de Naves
    public const string RelicExplorerPlate = "relic_explorer_plate";
    public const string RelicExtractionHook = "relic_extraction_hook";
    public const string RelicAnalyticCrystal = "relic_analytic_crystal";
    public const string RelicModularContainer = "relic_modular_container";
    public const string RelicRescueBeacon = "relic_rescue_beacon";

    // Reliquias de Minería
    public const string RelicAncientDrill = "relic_ancient_drill";
    public const string RelicRememberedAlloy = "relic_remembered_alloy";
    public const string RelicProspectingCore = "relic_prospecting_core";
    public const string RelicExtractionSeal = "relic_extraction_seal";

    // Reliquias de Escáner / Estación
    public const string RelicFracturedAntenna = "relic_fractured_antenna";
    public const string RelicIncompleteStarMap = "relic_incomplete_starmap";
    public const string RelicRareFrequencySensor = "relic_rare_frequency_sensor";
    public const string RelicMatrixArchive = "relic_matrix_archive";

    // Reliquias de Cuarto 1
    public const string RelicRoom1Echo = "relic_room1_echo";
    public const string RelicTracesResonator = "relic_traces_resonator";
    public const string RelicCalibrationFragment = "relic_calibration_fragment";
    public const string RelicTriangularSeal = "relic_triangular_seal";

    // Reliquias de Cuarto 2
    public const string RelicBrokenMachineKey = "relic_broken_machine_key";
    public const string RelicSealedCatalyst = "relic_sealed_catalyst";
    public const string RelicChamberFragment = "relic_chamber_fragment";
    public const string RelicMachineMemory = "relic_machine_memory";

    public static readonly string[] Dimension1RelicIds =
    {
    RelicDriftCompass,
    RelicAncientCargoCore,
    RelicLostNavigationRecord,
    RelicExpeditionSeal,
    RelicDormantEcho,

    RelicExplorerPlate,
    RelicExtractionHook,
    RelicAnalyticCrystal,
    RelicModularContainer,
    RelicRescueBeacon,

    RelicAncientDrill,
    RelicRememberedAlloy,
    RelicProspectingCore,
    RelicExtractionSeal,

    RelicFracturedAntenna,
    RelicIncompleteStarMap,
    RelicRareFrequencySensor,
    RelicMatrixArchive,

    RelicRoom1Echo,
    RelicTracesResonator,
    RelicCalibrationFragment,
    RelicTriangularSeal,

    RelicBrokenMachineKey,
    RelicSealedCatalyst,
    RelicChamberFragment,
    RelicMachineMemory
};

    public const int Dimension1TreeTieredNodeMaxTier = 3;

    // Árbol D1 - Rama Exploración
    public const string D1TreeExplorationDestinationReading = "d1_tree_exploration_destination_reading";
    public const string D1TreeExplorationFilter = "d1_tree_exploration_filter";
    public const string D1TreeExplorationHiddenFindTracking = "d1_tree_exploration_hidden_find_tracking";
    public const string D1TreeExplorationContinuationDetected = "d1_tree_exploration_continuation_detected";
    public const string D1TreeExplorationScanMemory = "d1_tree_exploration_scan_memory";
    public const string D1TreeExplorationAdvancedCartography = "d1_tree_exploration_advanced_cartography";

    // Árbol D1 - Rama Flota
    public const string D1TreeFleetHangarPreparation = "d1_tree_fleet_hangar_preparation";
    public const string D1TreeFleetCoordination = "d1_tree_fleet_coordination";
    public const string D1TreeFleetSupportFormation = "d1_tree_fleet_support_formation";
    public const string D1TreeFleetSupportProtocols = "d1_tree_fleet_support_protocols";
    public const string D1TreeFleetRescueOperations = "d1_tree_fleet_rescue_operations";
    public const string D1TreeFleetConvergenceLink = "d1_tree_fleet_convergence_link";

    // Árbol D1 - Rama Recuperación
    public const string D1TreeRecoveryCopyRegistry = "d1_tree_recovery_copy_registry";
    public const string D1TreeRecoveryPartialRecovery = "d1_tree_recovery_partial_recovery";
    public const string D1TreeRecoveryBlueprintPriority = "d1_tree_recovery_blueprint_priority";
    public const string D1TreeRecoveryCargoConservation = "d1_tree_recovery_cargo_conservation";
    public const string D1TreeRecoveryFindProtection = "d1_tree_recovery_find_protection";
    public const string D1TreeRecoveryExpeditionArchive = "d1_tree_recovery_expedition_archive";

    // Árbol D1 - Rama Convergencia
    public const string D1TreeConvergenceAnomalousReading = "d1_tree_convergence_anomalous_reading";
    public const string D1TreeConvergenceSpecialDestinationReading = "d1_tree_convergence_special_destination_reading";
    public const string D1TreeConvergenceUnstableZoneStabilization = "d1_tree_convergence_unstable_zone_stabilization";
    public const string D1TreeConvergenceChain = "d1_tree_convergence_chain";
    public const string D1TreeConvergenceAdvancedBlueprintSignal = "d1_tree_convergence_advanced_blueprint_signal";
    public const string D1TreeConvergenceDimensionalCore = "d1_tree_convergence_dimensional_core";

    public static readonly string[] Dimension1TreeNodeIds =
    {
    D1TreeExplorationDestinationReading,
    D1TreeExplorationFilter,
    D1TreeExplorationHiddenFindTracking,
    D1TreeExplorationContinuationDetected,
    D1TreeExplorationScanMemory,
    D1TreeExplorationAdvancedCartography,

    D1TreeFleetHangarPreparation,
    D1TreeFleetCoordination,
    D1TreeFleetSupportFormation,
    D1TreeFleetSupportProtocols,
    D1TreeFleetRescueOperations,
    D1TreeFleetConvergenceLink,

    D1TreeRecoveryCopyRegistry,
    D1TreeRecoveryPartialRecovery,
    D1TreeRecoveryBlueprintPriority,
    D1TreeRecoveryCargoConservation,
    D1TreeRecoveryFindProtection,
    D1TreeRecoveryExpeditionArchive,

    D1TreeConvergenceAnomalousReading,
    D1TreeConvergenceSpecialDestinationReading,
    D1TreeConvergenceUnstableZoneStabilization,
    D1TreeConvergenceChain,
    D1TreeConvergenceAdvancedBlueprintSignal,
    D1TreeConvergenceDimensionalCore
};

    // Blueprints específicos de naves
    public const string BlueprintCargoFrame = "blueprint_cargo_frame";
    public const string BlueprintCargoHold = "blueprint_cargo_hold";
    public const string BlueprintCargoStabilizer = "blueprint_cargo_stabilizer";

    public const string BlueprintRescueFrame = "blueprint_rescue_frame";
    public const string BlueprintRescueBeacon = "blueprint_rescue_beacon";
    public const string BlueprintRescueRecoveryBay = "blueprint_rescue_recovery_bay";
    public const string BlueprintRescueProtectionMatrix = "blueprint_rescue_protection_matrix";

    public const string BlueprintConvergenceChassis = "blueprint_convergence_chassis";
    public const string BlueprintConvergenceCore = "blueprint_convergence_core";
    public const string BlueprintConvergenceMatrix = "blueprint_convergence_matrix";
    public const string BlueprintAnomalousArmor = "blueprint_anomalous_armor";

    public static bool TryGetRequiredShipBlueprintIds(
    string shipId,
    out string[] blueprintIds
)
    {
        blueprintIds = new string[0];

        if (shipId == ShipCargoShip)
        {
            blueprintIds = new string[]
            {
            BlueprintCargoFrame,
            BlueprintCargoHold,
            BlueprintCargoStabilizer
            };

            return true;
        }

        if (shipId == ShipRescueShip)
        {
            blueprintIds = new string[]
            {
            BlueprintRescueFrame,
            BlueprintRescueBeacon,
            BlueprintRescueRecoveryBay,
            BlueprintRescueProtectionMatrix
            };

            return true;
        }

        if (shipId == ShipConvergenceShip)
        {
            blueprintIds = new string[]
            {
            BlueprintConvergenceChassis,
            BlueprintConvergenceCore,
            BlueprintConvergenceMatrix,
            BlueprintAnomalousArmor
            };

            return true;
        }

        return false;
    }

    public static bool IsDimension1BlueprintId(string blueprintId)
    {
        if (string.IsNullOrEmpty(blueprintId))
            return false;

        foreach (string currentId in Dimension1BlueprintIds)
        {
            if (currentId == blueprintId)
                return true;
        }

        return false;
    }

    public static bool IsDimension1RelicId(string relicId)
    {
        if (string.IsNullOrEmpty(relicId))
            return false;

        foreach (string currentId in Dimension1RelicIds)
        {
            if (currentId == relicId)
                return true;
        }

        return false;
    }

    public static int ClampDimension1RelicLevel(int level)
    {
        return Mathf.Clamp(level, 0, Dimension1RelicMaxLevel);
    }

    public static int GetDimension1RelicMilestone(int level)
    {
        level = ClampDimension1RelicLevel(level);

        if (level <= 0)
            return 0;

        int milestone = level / Dimension1RelicMilestoneStep;
        return Mathf.Clamp(
            milestone * Dimension1RelicMilestoneStep,
            0,
            Dimension1RelicMaxLevel
        );
    }

    public static double GetDimension1RelicUpgradeCostMultiplier(int targetLevel)
    {
        targetLevel = ClampDimension1RelicLevel(targetLevel);

        if (targetLevel <= 1)
            return 1.0;

        double multiplier = 1.0;

        for (int level = 2; level <= targetLevel; level++)
        {
            if (level <= 25)
            {
                multiplier *= 1.01;
            }
            else if (level <= 100)
            {
                multiplier *= 1.015;
            }
            else
            {
                multiplier *= 1.02;
            }
        }

        return multiplier;
    }

    public static bool TryGetDimension1RelicUpgradeBaseCost(
        string relicId,
        out double baseLeCost,
        out double baseTraceCost,
        out string metal1,
        out double baseMetalAmount1,
        out string metal2,
        out double baseMetalAmount2
    )
    {
        baseLeCost = 0.0;
        baseTraceCost = 0.0;
        metal1 = "";
        baseMetalAmount1 = 0.0;
        metal2 = "";
        baseMetalAmount2 = 0.0;

        if (!IsDimension1RelicId(relicId))
            return false;

        // Exploración
        if (
            relicId == RelicDriftCompass ||
            relicId == RelicAncientCargoCore ||
            relicId == RelicLostNavigationRecord ||
            relicId == RelicExpeditionSeal ||
            relicId == RelicDormantEcho
        )
        {
            baseLeCost = 2500.0;
            baseTraceCost = 12.0;
            metal1 = MetalIron;
            baseMetalAmount1 = 80.0;
            metal2 = MetalCopper;
            baseMetalAmount2 = 40.0;
            return true;
        }

        // Naves
        if (
            relicId == RelicExplorerPlate ||
            relicId == RelicExtractionHook ||
            relicId == RelicAnalyticCrystal ||
            relicId == RelicModularContainer ||
            relicId == RelicRescueBeacon
        )
        {
            baseLeCost = 3500.0;
            baseTraceCost = 16.0;
            metal1 = MetalIron;
            baseMetalAmount1 = 100.0;
            metal2 = MetalTitanium;
            baseMetalAmount2 = 45.0;
            return true;
        }

        // Minería planetaria
        if (
            relicId == RelicAncientDrill ||
            relicId == RelicRememberedAlloy ||
            relicId == RelicProspectingCore ||
            relicId == RelicExtractionSeal
        )
        {
            baseLeCost = 2200.0;
            baseTraceCost = 10.0;
            metal1 = MetalIron;
            baseMetalAmount1 = 120.0;
            metal2 = MetalNickel;
            baseMetalAmount2 = 35.0;
            return true;
        }

        // Escáner / Estación
        if (
            relicId == RelicFracturedAntenna ||
            relicId == RelicIncompleteStarMap ||
            relicId == RelicRareFrequencySensor ||
            relicId == RelicMatrixArchive
        )
        {
            baseLeCost = 4000.0;
            baseTraceCost = 18.0;
            metal1 = MetalCopper;
            baseMetalAmount1 = 90.0;
            metal2 = MetalAluminum;
            baseMetalAmount2 = 60.0;
            return true;
        }

        // Cuarto 1
        if (
            relicId == RelicRoom1Echo ||
            relicId == RelicTracesResonator ||
            relicId == RelicCalibrationFragment ||
            relicId == RelicTriangularSeal
        )
        {
            baseLeCost = 6000.0;
            baseTraceCost = 25.0;
            return true;
        }

        // Cuarto 2
        if (
            relicId == RelicBrokenMachineKey ||
            relicId == RelicSealedCatalyst ||
            relicId == RelicChamberFragment ||
            relicId == RelicMachineMemory
        )
        {
            baseLeCost = 7000.0;
            baseTraceCost = 30.0;
            metal1 = MetalNickel;
            baseMetalAmount1 = 100.0;
            metal2 = MetalCobalt;
            baseMetalAmount2 = 60.0;
            return true;
        }

        return false;
    }

    public static bool TryGetNextDimension1RelicUpgradeCost(
        GameState state,
        string relicId,
        out int targetLevel,
        out double leCost,
        out double traceCost,
        out string metal1,
        out double metalAmount1,
        out string metal2,
        out double metalAmount2
    )
    {
        targetLevel = 0;
        leCost = 0.0;
        traceCost = 0.0;
        metal1 = "";
        metalAmount1 = 0.0;
        metal2 = "";
        metalAmount2 = 0.0;

        if (state == null)
            return false;

        if (!IsDimension1RelicId(relicId))
            return false;

        state.EnsureDimension1State();

        if (!state.IsD1RelicUnlocked(relicId))
            return false;

        int currentLevel = state.GetD1RelicLevel(relicId);

        if (currentLevel >= Dimension1RelicMaxLevel)
            return false;

        targetLevel = currentLevel + 1;

        if (!TryGetDimension1RelicUpgradeBaseCost(
            relicId,
            out double baseLeCost,
            out double baseTraceCost,
            out metal1,
            out double baseMetalAmount1,
            out metal2,
            out double baseMetalAmount2
        ))
        {
            return false;
        }

        double multiplier = GetDimension1RelicUpgradeCostMultiplier(targetLevel);

        leCost = System.Math.Ceiling(baseLeCost * multiplier);
        traceCost = System.Math.Ceiling(baseTraceCost * multiplier);

        if (!string.IsNullOrEmpty(metal1))
            metalAmount1 = System.Math.Ceiling(baseMetalAmount1 * multiplier);

        if (!string.IsNullOrEmpty(metal2))
            metalAmount2 = System.Math.Ceiling(baseMetalAmount2 * multiplier);

        return true;
    }

    public static bool CanUpgradeDimension1Relic(
        GameState state,
        string relicId
    )
    {
        if (!TryGetNextDimension1RelicUpgradeCost(
            state,
            relicId,
            out _,
            out double leCost,
            out double traceCost,
            out string metal1,
            out double metalAmount1,
            out string metal2,
            out double metalAmount2
        ))
        {
            return false;
        }

        if (state.LE < leCost)
            return false;

        if (state.Traces < traceCost)
            return false;

        if (!string.IsNullOrEmpty(metal1) && state.GetD1MetalAmount(metal1) < metalAmount1)
            return false;

        if (!string.IsNullOrEmpty(metal2) && state.GetD1MetalAmount(metal2) < metalAmount2)
            return false;

        return true;
    }

    public static bool TryUpgradeDimension1Relic(
        GameState state,
        string relicId
    )
    {
        if (!CanUpgradeDimension1Relic(state, relicId))
            return false;

        if (!TryGetNextDimension1RelicUpgradeCost(
            state,
            relicId,
            out int targetLevel,
            out double leCost,
            out double traceCost,
            out string metal1,
            out double metalAmount1,
            out string metal2,
            out double metalAmount2
        ))
        {
            return false;
        }

        state.LE -= leCost;
        state.Traces -= traceCost;

        if (!string.IsNullOrEmpty(metal1) && !state.SpendD1Metal(metal1, metalAmount1))
            return false;

        if (!string.IsNullOrEmpty(metal2) && !state.SpendD1Metal(metal2, metalAmount2))
            return false;

        return state.SetD1RelicLevel(relicId, targetLevel);
    }

    public static bool IsDimension1TreeNodeId(string nodeId)
    {
        if (string.IsNullOrEmpty(nodeId))
            return false;

        foreach (string currentId in Dimension1TreeNodeIds)
        {
            if (currentId == nodeId)
                return true;
        }

        return false;
    }

    public static int GetDimension1TreeNodeMaxTier(string nodeId)
    {
        switch (nodeId)
        {
            case D1TreeExplorationHiddenFindTracking:
            case D1TreeExplorationScanMemory:
            case D1TreeFleetSupportFormation:
            case D1TreeFleetSupportProtocols:
            case D1TreeRecoveryCopyRegistry:
            case D1TreeRecoveryPartialRecovery:
            case D1TreeConvergenceSpecialDestinationReading:
            case D1TreeConvergenceAdvancedBlueprintSignal:
                return Dimension1TreeTieredNodeMaxTier;

            default:
                return IsDimension1TreeNodeId(nodeId) ? 1 : 0;
        }
    }

    public static int ClampDimension1TreeNodeTier(string nodeId, int tier)
    {
        int maxTier = GetDimension1TreeNodeMaxTier(nodeId);

        if (maxTier <= 0)
            return 0;

        return Mathf.Clamp(tier, 0, maxTier);
    }

    public static int GetDimension1TreeNodeCost(string nodeId, int targetTier)
    {
        targetTier = ClampDimension1TreeNodeTier(nodeId, targetTier);

        if (targetTier <= 0)
            return 999999;

        switch (nodeId)
        {
            // Exploración
            case D1TreeExplorationDestinationReading:
                return 2;
            case D1TreeExplorationFilter:
                return 3;
            case D1TreeExplorationHiddenFindTracking:
                return targetTier == 1 ? 3 : targetTier == 2 ? 5 : 7;
            case D1TreeExplorationContinuationDetected:
                return 6;
            case D1TreeExplorationScanMemory:
                return targetTier == 1 ? 4 : targetTier == 2 ? 6 : 8;
            case D1TreeExplorationAdvancedCartography:
                return 10;

            // Flota
            case D1TreeFleetHangarPreparation:
                return 2;
            case D1TreeFleetCoordination:
                return 6;
            case D1TreeFleetSupportFormation:
                return targetTier == 1 ? 4 : targetTier == 2 ? 6 : 8;
            case D1TreeFleetSupportProtocols:
                return targetTier == 1 ? 5 : targetTier == 2 ? 8 : 10;
            case D1TreeFleetRescueOperations:
                return 12;
            case D1TreeFleetConvergenceLink:
                return 15;

            // Recuperación
            case D1TreeRecoveryCopyRegistry:
                return targetTier == 1 ? 4 : targetTier == 2 ? 6 : 8;
            case D1TreeRecoveryPartialRecovery:
                return targetTier == 1 ? 5 : targetTier == 2 ? 8 : 10;
            case D1TreeRecoveryBlueprintPriority:
                return 8;
            case D1TreeRecoveryCargoConservation:
                return 10;
            case D1TreeRecoveryFindProtection:
                return 12;
            case D1TreeRecoveryExpeditionArchive:
                return 15;

            // Convergencia
            case D1TreeConvergenceAnomalousReading:
                return 10;
            case D1TreeConvergenceSpecialDestinationReading:
                return targetTier == 1 ? 8 : targetTier == 2 ? 12 : 15;
            case D1TreeConvergenceUnstableZoneStabilization:
                return 14;
            case D1TreeConvergenceChain:
                return 18;
            case D1TreeConvergenceAdvancedBlueprintSignal:
                return targetTier == 1 ? 10 : targetTier == 2 ? 15 : 20;
            case D1TreeConvergenceDimensionalCore:
                return 25;

            default:
                return 999999;
        }
    }

    private static bool HasDimension1TreeNodePrerequisite(GameState state, string nodeId)
    {
        if (state == null)
            return false;

        switch (nodeId)
        {
            // Exploración
            case D1TreeExplorationDestinationReading:
                return true;
            case D1TreeExplorationFilter:
                return state.IsD1TreeNodeUnlocked(D1TreeExplorationDestinationReading);
            case D1TreeExplorationHiddenFindTracking:
                return state.IsD1TreeNodeUnlocked(D1TreeExplorationFilter);
            case D1TreeExplorationContinuationDetected:
                return state.IsD1TreeNodeUnlocked(D1TreeExplorationHiddenFindTracking);
            case D1TreeExplorationScanMemory:
                return state.IsD1TreeNodeUnlocked(D1TreeExplorationContinuationDetected);
            case D1TreeExplorationAdvancedCartography:
                return state.IsD1TreeNodeUnlocked(D1TreeExplorationScanMemory);

            // Flota
            case D1TreeFleetHangarPreparation:
                return true;
            case D1TreeFleetCoordination:
                return state.IsD1TreeNodeUnlocked(D1TreeFleetHangarPreparation);
            case D1TreeFleetSupportFormation:
                return state.IsD1TreeNodeUnlocked(D1TreeFleetCoordination);
            case D1TreeFleetSupportProtocols:
                return state.IsD1TreeNodeUnlocked(D1TreeFleetSupportFormation);
            case D1TreeFleetRescueOperations:
                return state.IsD1TreeNodeUnlocked(D1TreeFleetSupportProtocols);
            case D1TreeFleetConvergenceLink:
                return state.IsD1TreeNodeUnlocked(D1TreeFleetRescueOperations);

            // Recuperación
            case D1TreeRecoveryCopyRegistry:
                return true;
            case D1TreeRecoveryPartialRecovery:
                return state.IsD1TreeNodeUnlocked(D1TreeRecoveryCopyRegistry);
            case D1TreeRecoveryBlueprintPriority:
                return state.IsD1TreeNodeUnlocked(D1TreeRecoveryPartialRecovery);
            case D1TreeRecoveryCargoConservation:
                return state.IsD1TreeNodeUnlocked(D1TreeRecoveryBlueprintPriority);
            case D1TreeRecoveryFindProtection:
                return state.IsD1TreeNodeUnlocked(D1TreeRecoveryCargoConservation);
            case D1TreeRecoveryExpeditionArchive:
                return state.IsD1TreeNodeUnlocked(D1TreeRecoveryFindProtection);

            // Convergencia
            case D1TreeConvergenceAnomalousReading:
                return true;
            case D1TreeConvergenceSpecialDestinationReading:
                return state.IsD1TreeNodeUnlocked(D1TreeConvergenceAnomalousReading);
            case D1TreeConvergenceUnstableZoneStabilization:
                return state.IsD1TreeNodeUnlocked(D1TreeConvergenceSpecialDestinationReading);
            case D1TreeConvergenceChain:
                return state.IsD1TreeNodeUnlocked(D1TreeConvergenceUnstableZoneStabilization);
            case D1TreeConvergenceAdvancedBlueprintSignal:
                return state.IsD1TreeNodeUnlocked(D1TreeConvergenceChain);
            case D1TreeConvergenceDimensionalCore:
                return state.IsD1TreeNodeUnlocked(D1TreeConvergenceAdvancedBlueprintSignal);

            default:
                return false;
        }
    }

    public static bool CanBuyDimension1TreeNode(GameState state, string nodeId)
    {
        if (state == null)
            return false;

        if (!IsDimension1TreeNodeId(nodeId))
            return false;

        state.EnsureDimension1State();

        int currentTier = state.GetD1TreeNodeTier(nodeId);
        int maxTier = GetDimension1TreeNodeMaxTier(nodeId);

        if (currentTier >= maxTier)
            return false;

        if (currentTier <= 0 && !HasDimension1TreeNodePrerequisite(state, nodeId))
            return false;

        int targetTier = currentTier + 1;
        int cost = GetDimension1TreeNodeCost(nodeId, targetTier);

        return state.prestige1Points >= cost;
    }

    public static bool TryBuyDimension1TreeNode(GameState state, string nodeId)
    {
        if (!CanBuyDimension1TreeNode(state, nodeId))
            return false;

        int currentTier = state.GetD1TreeNodeTier(nodeId);
        int targetTier = currentTier + 1;
        int cost = GetDimension1TreeNodeCost(nodeId, targetTier);

        state.prestige1Points -= cost;

        return state.SetD1TreeNodeTier(nodeId, targetTier);
    }

    private static int GetDimension1TreeTierSafe(GameState state, string nodeId)
    {
        if (state == null)
            return 0;

        state.EnsureDimension1State();

        return state.GetD1TreeNodeTier(nodeId);
    }

    public static bool HasDimension1TreeNode(GameState state, string nodeId)
    {
        return GetDimension1TreeTierSafe(state, nodeId) > 0;
    }

    // Rama Exploración

    public static bool HasD1TreeDestinationReading(GameState state)
    {
        return HasDimension1TreeNode(state, D1TreeExplorationDestinationReading);
    }

    public static bool HasD1TreeExplorationFilter(GameState state)
    {
        return HasDimension1TreeNode(state, D1TreeExplorationFilter);
    }

    public static float GetD1TreeHiddenFindQualityBonus(GameState state)
    {
        int tier = GetDimension1TreeTierSafe(state, D1TreeExplorationHiddenFindTracking);

        switch (tier)
        {
            case 1:
                return 0.02f;
            case 2:
                return 0.04f;
            case 3:
                return 0.06f;
            default:
                return 0.0f;
        }
    }

    public static float GetD1TreeContinuationDetectionBonus(GameState state)
    {
        return HasDimension1TreeNode(state, D1TreeExplorationContinuationDetected)
            ? 0.03f
            : 0.0f;
    }

    public static float GetD1TreeScanMemoryRepetitionReduction(GameState state)
    {
        int tier = GetDimension1TreeTierSafe(state, D1TreeExplorationScanMemory);

        switch (tier)
        {
            case 1:
                return 0.03f;
            case 2:
                return 0.06f;
            case 3:
                return 0.10f;
            default:
                return 0.0f;
        }
    }

    public static float GetD1TreeAdvancedCartographySpecialDestinationChance(GameState state)
    {
        return HasDimension1TreeNode(state, D1TreeExplorationAdvancedCartography)
            ? 0.10f
            : 0.0f;
    }

    // Rama Flota

    public static float GetD1TreeSingleShipEfficiencyBonus(GameState state)
    {
        return HasDimension1TreeNode(state, D1TreeFleetHangarPreparation)
            ? 0.03f
            : 0.0f;
    }

    public static bool HasD1TreeFleetCoordination(GameState state)
    {
        return HasDimension1TreeNode(state, D1TreeFleetCoordination);
    }

    public static float GetD1TreeSupportFormationValue(GameState state)
    {
        int tier = GetDimension1TreeTierSafe(state, D1TreeFleetSupportFormation);

        switch (tier)
        {
            case 1:
                return 0.20f;
            case 2:
                return 0.30f;
            case 3:
                return 0.40f;
            default:
                return 0.0f;
        }
    }

    public static float GetD1TreeSupportProtocolsEfficiencyBonus(GameState state)
    {
        int tier = GetDimension1TreeTierSafe(state, D1TreeFleetSupportProtocols);

        switch (tier)
        {
            case 1:
                return 0.03f;
            case 2:
                return 0.06f;
            case 3:
                return 0.10f;
            default:
                return 0.0f;
        }
    }

    public static bool HasD1TreeRescueOperations(GameState state)
    {
        return HasDimension1TreeNode(state, D1TreeFleetRescueOperations);
    }

    public static bool HasD1TreeConvergenceLink(GameState state)
    {
        return HasDimension1TreeNode(state, D1TreeFleetConvergenceLink);
    }

    // Rama Recuperación

    public static float GetD1TreeDuplicateRelicConversionBonus(GameState state)
    {
        int tier = GetDimension1TreeTierSafe(state, D1TreeRecoveryCopyRegistry);

        switch (tier)
        {
            case 1:
                return 0.10f;
            case 2:
                return 0.20f;
            case 3:
                return 0.30f;
            default:
                return 0.0f;
        }
    }

    public static void GetD1TreePartialRecoveryValues(
        GameState state,
        out float chance,
        out float recoveredAmount
    )
    {
        int tier = GetDimension1TreeTierSafe(state, D1TreeRecoveryPartialRecovery);

        switch (tier)
        {
            case 1:
                chance = 0.15f;
                recoveredAmount = 0.10f;
                return;

            case 2:
                chance = 0.25f;
                recoveredAmount = 0.15f;
                return;

            case 3:
                chance = 0.35f;
                recoveredAmount = 0.20f;
                return;

            default:
                chance = 0.0f;
                recoveredAmount = 0.0f;
                return;
        }
    }

    public static float GetD1TreeBlueprintPriorityBonus(GameState state)
    {
        return HasDimension1TreeNode(state, D1TreeRecoveryBlueprintPriority)
            ? 0.03f
            : 0.0f;
    }

    public static float GetD1TreeRareCargoConservationChance(GameState state)
    {
        return HasDimension1TreeNode(state, D1TreeRecoveryCargoConservation)
            ? 0.10f
            : 0.0f;
    }

    public static float GetD1TreeDelicateFindProtectionBonus(GameState state)
    {
        return HasDimension1TreeNode(state, D1TreeRecoveryFindProtection)
            ? 0.05f
            : 0.0f;
    }

    public static float GetD1TreeExpeditionArchiveUsefulRewardBonus(GameState state)
    {
        return HasDimension1TreeNode(state, D1TreeRecoveryExpeditionArchive)
            ? 0.05f
            : 0.0f;
    }

    // Rama Convergencia

    public static float GetD1TreeAnomalousHiddenIdentificationBonus(GameState state)
    {
        return HasDimension1TreeNode(state, D1TreeConvergenceAnomalousReading)
            ? 0.10f
            : 0.0f;
    }

    public static float GetD1TreeSpecialDestinationDetectionBonus(GameState state)
    {
        int tier = GetDimension1TreeTierSafe(state, D1TreeConvergenceSpecialDestinationReading);

        switch (tier)
        {
            case 1:
                return 0.02f;
            case 2:
                return 0.04f;
            case 3:
                return 0.06f;
            default:
                return 0.0f;
        }
    }

    public static bool HasD1TreeUnstableZoneStabilization(GameState state)
    {
        return HasDimension1TreeNode(state, D1TreeConvergenceUnstableZoneStabilization);
    }

    public static float GetD1TreeUnstableZoneRiskReduction(GameState state)
    {
        return HasD1TreeUnstableZoneStabilization(state)
            ? 0.05f
            : 0.0f;
    }

    public static float GetD1TreeUnstableZoneDurationReduction(GameState state)
    {
        return HasD1TreeUnstableZoneStabilization(state)
            ? 0.05f
            : 0.0f;
    }

    public static float GetD1TreeUnstableZoneRareRewardProtection(GameState state)
    {
        return HasD1TreeUnstableZoneStabilization(state)
            ? 0.05f
            : 0.0f;
    }

    public static bool HasD1TreeConvergenceChain(GameState state)
    {
        return HasDimension1TreeNode(state, D1TreeConvergenceChain);
    }

    public static float GetD1TreeAdvancedBlueprintSignalBonus(GameState state)
    {
        int tier = GetDimension1TreeTierSafe(state, D1TreeConvergenceAdvancedBlueprintSignal);

        switch (tier)
        {
            case 1:
                return 0.01f;
            case 2:
                return 0.02f;
            case 3:
                return 0.03f;
            default:
                return 0.0f;
        }
    }

    public static bool HasD1TreeDimensionalCore(GameState state)
    {
        return HasDimension1TreeNode(state, D1TreeConvergenceDimensionalCore);
    }

    public static int CalculatePrestige1PointsFromBaseGame(GameState state)
    {
        if (state == null)
            return 0;

        double maxLe = System.Math.Max(state.maxLEAlcanzado, state.LE);

        if (maxLe >= 1000000000.0)
            return 16;

        if (maxLe >= 100000000.0)
            return 12;

        if (maxLe >= 10000000.0)
            return 8;

        if (maxLe >= 1000000.0)
            return 5;

        if (maxLe >= 100000.0)
            return 3;

        if (maxLe >= 10000.0)
            return 1;

        return 0;
    }

    public static int CalculatePrestige1PointsFromDimension1(GameState state)
    {
        if (state == null)
            return 0;

        int rawPoints =
            CalculatePrestige1PointsFromD1Planets(state) +
            CalculatePrestige1PointsFromD1Ships(state) +
            CalculatePrestige1PointsFromD1Relics(state) +
            CalculatePrestige1PointsFromD1Tree(state) +
            CalculatePrestige1PointsFromD1Scanner(state);

        return Mathf.Min(rawPoints, 12);
    }

    public static int CalculatePrestige1PointsPreview(GameState state)
    {
        if (state == null)
            return 0;

        return
            CalculatePrestige1PointsFromBaseGame(state) +
            CalculatePrestige1PointsFromDimension1(state);
    }

    public static int CalculateClaimablePrestige1Points(GameState state)
    {
        if (state == null)
            return 0;

        int previewPoints = CalculatePrestige1PointsPreview(state);
        int alreadyClaimed = Mathf.Max(0, state.prestige1BestClaimedPreviewPoints);

        return Mathf.Max(0, previewPoints - alreadyClaimed);
    }

    public static bool TryClaimPrestige1PreviewPoints(GameState state, out int claimedPoints)
    {
        claimedPoints = 0;

        if (state == null)
            return false;

        state.EnsureDimension1State();

        int previewPoints = CalculatePrestige1PointsPreview(state);
        int claimablePoints = CalculateClaimablePrestige1Points(state);

        if (claimablePoints <= 0)
            return false;

        state.prestige1Points += claimablePoints;
        state.prestige1BestClaimedPreviewPoints = Mathf.Max(
            state.prestige1BestClaimedPreviewPoints,
            previewPoints
        );

        claimedPoints = claimablePoints;
        return true;
    }

    public static int CalculatePrestige1PointsFromD1Planets(GameState state)
    {
        if (state == null)
            return 0;

        state.EnsureDimension1State();

        int unlockedPlanets = 0;

        foreach (D1PlanetState planet in state.dimension1Planets)
        {
            if (planet != null && planet.unlocked)
                unlockedPlanets++;
        }

        int points = 0;

        if (unlockedPlanets >= 1)
            points += 1;

        if (unlockedPlanets >= 3)
            points += 2;

        if (unlockedPlanets >= 5)
            points += 3;

        if (unlockedPlanets >= 7)
            points += 4;

        return points;
    }

    public static int CalculatePrestige1PointsFromD1Ships(GameState state)
    {
        if (state == null)
            return 0;

        state.EnsureDimension1State();

        int unlockedShips = 0;

        foreach (D1ShipState ship in state.dimension1Ships)
        {
            if (ship != null && ship.unlocked)
                unlockedShips++;
        }

        int points = 0;

        if (unlockedShips >= 1)
            points += 1;

        if (unlockedShips >= 3)
            points += 2;

        if (IsShipUnlockedForPrestigePreview(state, ShipCargoShip))
            points += 2;

        if (IsShipUnlockedForPrestigePreview(state, ShipRescueShip))
            points += 3;

        if (IsShipUnlockedForPrestigePreview(state, ShipConvergenceShip))
            points += 4;

        return points;
    }

    private static bool IsShipUnlockedForPrestigePreview(GameState state, string shipId)
    {
        if (state == null)
            return false;

        foreach (D1ShipState ship in state.dimension1Ships)
        {
            if (ship != null && ship.shipId == shipId)
                return ship.unlocked;
        }

        return false;
    }

    public static int CalculatePrestige1PointsFromD1Relics(GameState state)
    {
        if (state == null)
            return 0;

        state.EnsureDimension1State();

        int unlockedRelics = 0;
        int totalMilestones = 0;

        foreach (D1RelicState relic in state.dimension1Relics)
        {
            if (relic == null || !relic.unlocked)
                continue;

            unlockedRelics++;

            int level = ClampDimension1RelicLevel(relic.level);
            totalMilestones += level / Dimension1RelicMilestoneStep;
        }

        int pointsFromUnlocked = Mathf.Min(8, unlockedRelics / 3);
        int pointsFromMilestones = Mathf.Min(8, totalMilestones / 8);

        return pointsFromUnlocked + pointsFromMilestones;
    }

    public static int CalculatePrestige1PointsFromD1Tree(GameState state)
    {
        if (state == null)
            return 0;

        state.EnsureDimension1State();

        int totalPurchasedTiers = 0;

        foreach (D1TreeNodeState node in state.dimension1TreeNodes)
        {
            if (node == null)
                continue;

            totalPurchasedTiers += ClampDimension1TreeNodeTier(
                node.nodeId,
                node.tier
            );
        }

        return Mathf.Min(8, totalPurchasedTiers / 5);
    }

    public static int CalculatePrestige1PointsFromD1Scanner(GameState state)
    {
        if (state == null)
            return 0;

        return Mathf.Clamp(state.dimension1ScannerLevel, 0, 3);
    }

    private static double GetDimension1RelicProgress01(GameState state, string relicId)
    {
        if (state == null)
            return 0.0;

        if (!IsDimension1RelicId(relicId))
            return 0.0;

        if (!state.IsD1RelicUnlocked(relicId))
            return 0.0;

        int level = state.GetD1RelicLevel(relicId);

        if (level <= 0)
            return 0.0;

        return Mathf.Clamp01((float)level / Dimension1RelicMaxLevel);
    }

    private static double GetDimension1RelicScaledBonus(
        GameState state,
        string relicId,
        double maxBonus
    )
    {
        return maxBonus * GetDimension1RelicProgress01(state, relicId);
    }

    private static bool IsLongExplorationDestination(string destinationId)
    {
        return GetSimpleExplorationBaseDurationSeconds(destinationId) >= 5.0;
    }

    private static bool IsDangerousExplorationDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationAbandonedShip:
            case DestinationAbandonedStation:
            case DestinationMinorAnomaly:
            case DestinationAncientStructure:
            case DestinationUnstableZone:
                return true;

            default:
                return false;
        }
    }

    private static double GetRelicExplorationDurationMultiplier(
        GameState state,
        string destinationId
    )
    {
        double reduction = GetDimension1RelicScaledBonus(
            state,
            RelicDriftCompass,
            0.06
        );

        if (IsLongExplorationDestination(destinationId))
        {
            reduction += GetDimension1RelicScaledBonus(
                state,
                RelicDriftCompass,
                0.03
            );
        }

        return System.Math.Max(0.50, 1.0 - reduction);
    }

    private static double GetD1TreeExplorationDurationMultiplier(
    GameState state,
    string destinationId,
    D1ShipState ship
)
    {
        double reduction = 0.0;

        if (ship != null)
        {
            reduction += GetD1TreeSingleShipEfficiencyBonus(state);
        }

        if (destinationId == DestinationUnstableZone)
        {
            reduction += GetD1TreeUnstableZoneDurationReduction(state);
        }

        return System.Math.Max(0.50, 1.0 - reduction);
    }

    private static double GetD1TreeMaterialRewardMultiplier(
        GameState state,
        string destinationId,
        D1ShipState ship
    )
    {
        double bonus = 0.0;

        if (ship != null)
        {
            bonus += GetD1TreeSingleShipEfficiencyBonus(state);
        }

        return 1.0 + bonus;
    }

    private static double GetRelicMaterialRewardMultiplier(
        GameState state,
        string destinationId,
        D1ShipState ship
    )
    {
        double bonus = 0.0;

        bonus += GetDimension1RelicScaledBonus(
            state,
            RelicAncientCargoCore,
            0.06
        );

        if (IsLongExplorationDestination(destinationId))
        {
            bonus += GetDimension1RelicScaledBonus(
                state,
                RelicAncientCargoCore,
                0.03
            );
        }

        if (ship != null && ship.shipId == ShipExtractorDrone)
        {
            bonus += GetDimension1RelicScaledBonus(
                state,
                RelicExtractionHook,
                0.06
            );
        }

        if (ship != null && ship.shipId == ShipCargoShip)
        {
            bonus += GetDimension1RelicScaledBonus(
                state,
                RelicModularContainer,
                0.06
            );

            if (IsLongExplorationDestination(destinationId))
            {
                bonus += GetDimension1RelicScaledBonus(
                    state,
                    RelicModularContainer,
                    0.03
                );
            }
        }

        return 1.0 + bonus;
    }

    private static double GetRelicArmorPreservationMultiplier(
        GameState state,
        string destinationId,
        D1ShipState ship
    )
    {
        if (ship == null)
            return 1.0;

        double bonus = 0.0;

        if (ship.shipId == ShipRescueShip)
        {
            bonus += GetDimension1RelicScaledBonus(
                state,
                RelicRescueBeacon,
                0.06
            );

            if (IsDangerousExplorationDestination(destinationId))
            {
                bonus += GetDimension1RelicScaledBonus(
                    state,
                    RelicRescueBeacon,
                    0.03
                );
            }
        }

        return 1.0 + bonus;
    }

    private static float GetRelicBlueprintFragmentBonus(
        GameState state,
        string destinationId,
        D1ShipState ship
    )
    {
        if (ship == null)
            return 0.0f;

        if (ship.shipId != ShipAnalyticProbe)
            return 0.0f;

        double bonus = GetDimension1RelicScaledBonus(
            state,
            RelicAnalyticCrystal,
            0.03
        );

        return (float)bonus;
    }

    private static bool IsPlanetPrimaryMetal(D1PlanetState planet, string metalId)
    {
        if (planet == null || string.IsNullOrEmpty(metalId))
            return false;

        switch (planet.planetId)
        {
            case Planet01:
                return metalId == MetalIron;

            case Planet02:
                return metalId == MetalAluminum;

            case Planet03:
                return metalId == MetalNickel;

            case Planet04:
                return metalId == MetalLithium;

            case Planet05:
                return metalId == MetalPlatinum;

            case Planet06:
                return metalId == MetalIridium;

            case Planet07:
                return metalId == MetalTungsten;

            default:
                return false;
        }
    }

    private static bool IsPlanetSecondaryOrExtraMetal(D1PlanetState planet, string metalId)
    {
        if (planet == null || string.IsNullOrEmpty(metalId))
            return false;

        switch (planet.planetId)
        {
            case Planet01:
                return metalId == MetalCopper && planet.extractorTier >= 10;

            case Planet02:
                return metalId == MetalTitanium && planet.extractorTier >= 10;

            case Planet03:
                return metalId == MetalCobalt && planet.extractorTier >= 10;

            case Planet04:
                return metalId == MetalTungsten && planet.extractorTier >= 10;

            case Planet05:
                return metalId == MetalNickel && planet.extractorTier >= 10;

            case Planet06:
                return metalId == MetalCobalt && planet.extractorTier >= 10;

            case Planet07:
                return
                    metalId == MetalPlatinum && planet.extractorTier >= 10 ||
                    metalId == MetalIridium && planet.extractorTier >= 20;

            default:
                return false;
        }
    }

    private static bool IsAdvancedPlanet(D1PlanetState planet)
    {
        if (planet == null)
            return false;

        return
            planet.planetId == Planet04 ||
            planet.planetId == Planet05 ||
            planet.planetId == Planet06 ||
            planet.planetId == Planet07;
    }

    private static int GetUnlockedMetalCountForPlanet(D1PlanetState planet)
    {
        if (planet == null || !planet.unlocked || planet.extractorTier <= 0)
            return 0;

        switch (planet.planetId)
        {
            case Planet07:
                if (planet.extractorTier >= 20)
                    return 3;

                if (planet.extractorTier >= 10)
                    return 2;

                return 1;

            default:
                return planet.extractorTier >= 10 ? 2 : 1;
        }
    }

    private static double GetRelicMiningProductionMultiplier(
        GameState state,
        D1PlanetState planet,
        string metalId
    )
    {
        if (state == null || planet == null)
            return 1.0;

        double bonus = 0.0;

        // Taladro Antiguo: producción minera general.
        bonus += GetDimension1RelicScaledBonus(
            state,
            RelicAncientDrill,
            0.06
        );

        // Taladro Antiguo: extra al recurso principal del planeta.
        if (IsPlanetPrimaryMetal(planet, metalId))
        {
            bonus += GetDimension1RelicScaledBonus(
                state,
                RelicAncientDrill,
                0.03
            );
        }

        // Núcleo de Prospección: recursos secundarios.
        if (IsPlanetSecondaryOrExtraMetal(planet, metalId))
        {
            bonus += GetDimension1RelicScaledBonus(
                state,
                RelicProspectingCore,
                0.06
            );
        }

        // Núcleo de Prospección: recursos raros en planetas avanzados.
        if (IsAdvancedPlanet(planet) && IsPlanetSecondaryOrExtraMetal(planet, metalId))
        {
            bonus += GetDimension1RelicScaledBonus(
                state,
                RelicProspectingCore,
                0.03
            );
        }

        // Sello de Extracción: extractores avanzados.
        if (planet.extractorTier >= 20)
        {
            bonus += GetDimension1RelicScaledBonus(
                state,
                RelicExtractionSeal,
                0.045
            );
        }

        // Sello de Extracción: planetas con varios recursos desbloqueados.
        if (GetUnlockedMetalCountForPlanet(planet) >= 2)
        {
            bonus += GetDimension1RelicScaledBonus(
                state,
                RelicExtractionSeal,
                0.03
            );
        }

        return 1.0 + bonus;
    }

    private static double GetRelicExtractorUpgradeCostMultiplier(
        GameState state,
        D1PlanetState planet
    )
    {
        if (state == null || planet == null)
            return 1.0;

        double reduction = GetDimension1RelicScaledBonus(
            state,
            RelicRememberedAlloy,
            0.03
        );

        int targetTier = planet.extractorTier + 1;

        if (targetTier > 0 && targetTier % 10 == 0)
        {
            reduction += GetDimension1RelicScaledBonus(
                state,
                RelicRememberedAlloy,
                0.03
            );
        }

        return System.Math.Max(0.50, 1.0 - reduction);
    }

    public static bool UsesSpecificShipMatricesForUnlock(string shipId)
    {
        return
            shipId == ShipCargoShip ||
            shipId == ShipRescueShip ||
            shipId == ShipConvergenceShip;
    }

    public static int GetOwnedRequiredSpecificShipMatrixCount(GameState state, string shipId)
    {
        if (state == null)
            return 0;

        if (!TryGetRequiredShipBlueprintIds(shipId, out string[] blueprintIds))
            return 0;

        int owned = 0;

        foreach (string blueprintId in blueprintIds)
        {
            if (state.GetD1BlueprintAmount(blueprintId) > 0)
                owned++;
        }

        return owned;
    }

    public static int GetMissingRequiredSpecificShipMatrixCount(GameState state, string shipId)
    {
        if (state == null)
            return 0;

        if (!TryGetRequiredShipBlueprintIds(shipId, out string[] blueprintIds))
            return 0;

        int owned = GetOwnedRequiredSpecificShipMatrixCount(state, shipId);
        int missing = blueprintIds.Length - owned;

        return missing > 0 ? missing : 0;
    }

    public static bool CanCoverRequiredShipMatrices(GameState state, string shipId)
    {
        if (state == null)
            return false;

        if (!TryGetRequiredShipBlueprintIds(shipId, out string[] blueprintIds))
            return true;

        int missing = GetMissingRequiredSpecificShipMatrixCount(state, shipId);
        int adaptiveMatrices = GetCompletedBlueprintCount(state);

        return adaptiveMatrices >= missing;
    }

    public static bool TrySpendRequiredShipMatrices(GameState state, string shipId)
    {
        if (state == null)
            return false;

        if (!TryGetRequiredShipBlueprintIds(shipId, out string[] blueprintIds))
            return true;

        if (!CanCoverRequiredShipMatrices(state, shipId))
            return false;

        int adaptiveMatricesNeeded = 0;

        foreach (string blueprintId in blueprintIds)
        {
            if (state.GetD1BlueprintAmount(blueprintId) > 0)
            {
                if (!state.SpendD1Blueprint(blueprintId, 1))
                    return false;
            }
            else
            {
                adaptiveMatricesNeeded++;
            }
        }

        if (adaptiveMatricesNeeded > 0)
            return TrySpendCompletedBlueprints(state, adaptiveMatricesNeeded);

        return true;
    }

    public static int GetRequiredSpecificShipUpgradeMatrixCost(string shipId, int targetLevel)
    {
        if (!UsesSpecificShipMatricesForUnlock(shipId))
            return 0;

        if (targetLevel < 4 || targetLevel > 6)
            return 0;

        return 1;
    }

    public static int GetAvailableSpecificShipUpgradeMatrixCount(GameState state, string shipId)
    {
        if (state == null)
            return 0;

        if (!TryGetRequiredShipBlueprintIds(shipId, out string[] matrixIds))
            return 0;

        int total = 0;

        foreach (string matrixId in matrixIds)
        {
            total += state.GetD1BlueprintAmount(matrixId);
        }

        return total;
    }

    public static bool CanSpendSpecificShipUpgradeMatrices(GameState state, string shipId, int matrixCost)
    {
        if (matrixCost <= 0)
            return true;

        return GetAvailableSpecificShipUpgradeMatrixCount(state, shipId) >= matrixCost;
    }

    public static bool TrySpendSpecificShipUpgradeMatrices(GameState state, string shipId, int matrixCost)
    {
        if (!CanSpendSpecificShipUpgradeMatrices(state, shipId, matrixCost))
            return false;

        if (matrixCost <= 0)
            return true;

        if (!TryGetRequiredShipBlueprintIds(shipId, out string[] matrixIds))
            return false;

        int remaining = matrixCost;

        foreach (string matrixId in matrixIds)
        {
            while (remaining > 0 && state.GetD1BlueprintAmount(matrixId) > 0)
            {
                if (!state.SpendD1Blueprint(matrixId, 1))
                    return false;

                remaining--;
            }

            if (remaining <= 0)
                return true;
        }

        return remaining <= 0;
    }

    public static readonly string[] Dimension1BlueprintIds =
    {
    BlueprintCargoFrame,
    BlueprintCargoHold,
    BlueprintCargoStabilizer,

    BlueprintRescueFrame,
    BlueprintRescueBeacon,
    BlueprintRescueRecoveryBay,
    BlueprintRescueProtectionMatrix,

    BlueprintConvergenceChassis,
    BlueprintConvergenceCore,
    BlueprintConvergenceMatrix,
    BlueprintAnomalousArmor
};

    public static int GetCompletedBlueprintCount(GameState state)
    {
        if (state == null)
            return 0;

        int fragments = Mathf.Max(state.dimension1BlueprintFragments, 0);
        return fragments / BlueprintFragmentsPerBlueprint;
    }

    public static int GetBlueprintFragmentProgress(GameState state)
    {
        if (state == null)
            return 0;

        int fragments = Mathf.Max(state.dimension1BlueprintFragments, 0);
        return fragments % BlueprintFragmentsPerBlueprint;
    }

    public static int GetCurrentSimpleScanDestinationCount(GameState state)
    {
        return GetSimpleScanDestinationCount(state);
    }

    public static int GetSimpleScannerLevel(GameState state)
    {
        if (state == null)
            return 0;

        return Mathf.Clamp(
            state.dimension1ScannerLevel,
            0,
            SimpleScannerMaxLevel
        );
    }

    public static bool IsSimpleScannerMaxed(GameState state)
    {
        return GetSimpleScannerLevel(state) >= SimpleScannerMaxLevel;
    }

    public static bool TryGetNextSimpleScannerUpgradeCost(
    GameState state,
    out int nextLevel,
    out string metal1,
    out double amount1,
    out string metal2,
    out double amount2,
    out string metal3,
    out double amount3,
    out string metal4,
    out double amount4
    )
    {
        nextLevel = 0;
        metal1 = "";
        amount1 = 0.0;
        metal2 = "";
        amount2 = 0.0;
        metal3 = "";
        amount3 = 0.0;
        metal4 = "";
        amount4 = 0.0;

        if (state == null || !state.dimension01Unlocked)
            return false;

        int currentLevel = GetSimpleScannerLevel(state);
        nextLevel = currentLevel + 1;

        if (nextLevel > SimpleScannerMaxLevel)
            return false;

        if (nextLevel == 1)
        {
            metal1 = MetalCopper;
            amount1 = 300.0;
            metal2 = MetalAluminum;
            amount2 = 180.0;
            return true;
        }

        if (nextLevel == 2)
        {
            metal1 = MetalCopper;
            amount1 = 900.0;
            metal2 = MetalAluminum;
            amount2 = 500.0;
            metal3 = MetalTitanium;
            amount3 = 250.0;
            return true;
        }

        if (nextLevel == 3)
        {
            metal1 = MetalCopper;
            amount1 = 1800.0;
            metal2 = MetalAluminum;
            amount2 = 900.0;
            metal3 = MetalTitanium;
            amount3 = 600.0;
            metal4 = MetalNickel;
            amount4 = 300.0;
            return true;
        }

        return false;
    }

    public static bool CanUpgradeSimpleScanner(GameState state)
    {

        if (state == null)
            return false;

        if (state.dimension1ScanActive)
            return false;

        if (IsAnyShipExploring(state))
            return false;

        if (!TryGetNextSimpleScannerUpgradeCost(
            state,
            out _,
            out string metal1,
            out double amount1,
            out string metal2,
            out double amount2,
            out string metal3,
            out double amount3,
            out string metal4,
            out double amount4
        ))
        {
            return false;
        }

        if (!CanSpendD1MetalAmount(state, metal1, amount1))
            return false;

        if (!CanSpendD1MetalAmount(state, metal2, amount2))
            return false;

        if (!CanSpendD1MetalAmount(state, metal3, amount3))
            return false;

        if (!CanSpendD1MetalAmount(state, metal4, amount4))
            return false;

        return true;
    }

    public static bool TryUpgradeSimpleScanner(GameState state)
    {
        if (!CanUpgradeSimpleScanner(state))
            return false;

        if (!TryGetNextSimpleScannerUpgradeCost(
            state,
            out int nextLevel,
            out string metal1,
            out double amount1,
            out string metal2,
            out double amount2,
            out string metal3,
            out double amount3,
            out string metal4,
            out double amount4
        ))
        {
            return false;
        }

        SpendD1MetalAmount(state, metal1, amount1);
        SpendD1MetalAmount(state, metal2, amount2);
        SpendD1MetalAmount(state, metal3, amount3);
        SpendD1MetalAmount(state, metal4, amount4);

        state.dimension1ScannerLevel = Mathf.Clamp(
            nextLevel,
            0,
            SimpleScannerMaxLevel
        );

        return true;
    }

    private static bool CanSpendD1MetalAmount(GameState state, string metalId, double amount)
    {
        if (state == null)
            return false;

        if (string.IsNullOrEmpty(metalId) || amount <= 0.0)
            return true;

        return state.GetD1MetalAmount(metalId) >= amount;
    }

    private static void SpendD1MetalAmount(GameState state, string metalId, double amount)
    {
        if (state == null)
            return;

        if (string.IsNullOrEmpty(metalId) || amount <= 0.0)
            return;

        state.SpendD1Metal(metalId, amount);
    }

    public static int GetSimpleScanMaxDestinationCount()
    {
        return SimpleScanMaxDestinationCount;
    }

    public static double GetSimpleExplorationBaseDurationPreviewSeconds(string destinationId)
    {
        return GetSimpleExplorationBaseDurationSeconds(destinationId);
    }

    public static double GetSimpleExplorationDurationPreviewSeconds(string destinationId, D1ShipState ship)
    {
        return GetSimpleExplorationDurationSeconds(destinationId, ship);
    }


    public static int GetBlueprintFragmentCostForBlueprints(int blueprintCount)
    {
        if (blueprintCount <= 0)
            return 0;

        return blueprintCount * BlueprintFragmentsPerBlueprint;
    }

    public static bool CanSpendCompletedBlueprints(GameState state, int blueprintCount)
    {
        if (state == null)
            return false;

        if (blueprintCount <= 0)
            return true;

        int requiredFragments = GetBlueprintFragmentCostForBlueprints(blueprintCount);
        int currentFragments = Mathf.Max(state.dimension1BlueprintFragments, 0);

        return currentFragments >= requiredFragments;
    }

    public static bool TrySpendCompletedBlueprints(GameState state, int blueprintCount)
    {
        if (!CanSpendCompletedBlueprints(state, blueprintCount))
            return false;

        if (blueprintCount <= 0)
            return true;

        int requiredFragments = GetBlueprintFragmentCostForBlueprints(blueprintCount);

        state.dimension1BlueprintFragments = Mathf.Max(
            0,
            state.dimension1BlueprintFragments - requiredFragments
        );

        return true;
    }

    public static readonly string[] StarterMetals =
    {
        MetalIron,
        MetalCopper,
        MetalAluminum,
        MetalTitanium,
        MetalNickel,
        MetalCobalt,
        MetalLithium,
        MetalTungsten,
        MetalPlatinum,
        MetalIridium
    };

    public static readonly string[] StarterPlanets =
    {
        Planet01,
        Planet02,
        Planet03,
        Planet04,
        Planet05,
        Planet06,
        Planet07
    };

    public static readonly string[] Dimension1ShipIds =
    {
        ShipLightProbe,
        ShipExtractorDrone,
        ShipAnalyticProbe,
        ShipCargoShip,
        ShipRescueShip,
        ShipConvergenceShip
    };

    public static void Tick(GameState state, double dt)
    {
        if (state == null)
            return;

        if (dt <= 0.0)
            return;

        if (!state.dimension01Unlocked)
            return;

        state.EnsureDimension1State();

        foreach (D1PlanetState planet in state.dimension1Planets)
        {
            if (planet == null)
                continue;

            if (!planet.unlocked)
                continue;

            if (planet.extractorTier <= 0)
                continue;

            ProducePlanetResources(state, planet, dt);
        }

        UpdateActiveScan(state, dt);
        UpdateActiveExplorations(state, dt);

    }

    private static void ProducePlanetResources(GameState state, D1PlanetState planet, double dt)
    {
        if (planet == null)
            return;

        switch (planet.planetId)
        {
            case Planet01:
                AddMetalPerSecond(state, planet, MetalIron, 0.10, planet.extractorTier, dt);

                if (planet.extractorTier >= 10)
                    AddMetalPerSecond(state, planet, MetalCopper, 0.04, planet.extractorTier, dt);

                break;

            case Planet02:
                AddMetalPerSecond(state, planet, MetalAluminum, 0.045, planet.extractorTier, dt);

                if (planet.extractorTier >= 10)
                    AddMetalPerSecond(state, planet, MetalTitanium, 0.018, planet.extractorTier, dt);

                break;

            case Planet03:
                AddMetalPerSecond(state, planet, MetalNickel, 0.028, planet.extractorTier, dt);

                if (planet.extractorTier >= 10)
                    AddMetalPerSecond(state, planet, MetalCobalt, 0.011, planet.extractorTier, dt);

                break;

            case Planet04:
                AddMetalPerSecond(state, planet, MetalLithium, 0.018, planet.extractorTier, dt);

                if (planet.extractorTier >= 10)
                    AddMetalPerSecond(state, planet, MetalTungsten, 0.007, planet.extractorTier, dt);

                break;

            case Planet05:
                AddMetalPerSecond(state, planet, MetalPlatinum, 0.012, planet.extractorTier, dt);

                if (planet.extractorTier >= 10)
                    AddMetalPerSecond(state, planet, MetalNickel, 0.020, planet.extractorTier, dt);

                break;

            case Planet06:
                AddMetalPerSecond(state, planet, MetalIridium, 0.008, planet.extractorTier, dt);

                if (planet.extractorTier >= 10)
                    AddMetalPerSecond(state, planet, MetalCobalt, 0.014, planet.extractorTier, dt);

                break;

            case Planet07:
                AddMetalPerSecond(state, planet, MetalTungsten, 0.010, planet.extractorTier, dt);

                if (planet.extractorTier >= 10)
                    AddMetalPerSecond(state, planet, MetalPlatinum, 0.007, planet.extractorTier, dt);

                if (planet.extractorTier >= 20)
                    AddMetalPerSecond(state, planet, MetalIridium, 0.004, planet.extractorTier, dt);

                break;
        }
    }

    private static void ProducePlanet01(GameState state, int tier, double dt)
    {
        // Planeta 1:
        // Tier 1  -> Hierro
        // Tier 10 -> Cobre

        AddMetalPerSecond(state, MetalIron, 0.10, tier, dt);

        if (tier >= 10)
        {
            AddMetalPerSecond(state, MetalCopper, 0.04, tier, dt);
        }
    }

    private static void ProducePlanet02(GameState state, int tier, double dt)
    {
        // Planeta 2:
        // Tier 1  -> Aluminio
        // Tier 10 -> Titanio

        AddMetalPerSecond(state, MetalAluminum, 0.045, tier, dt);

        if (tier >= 10)
        {
            AddMetalPerSecond(state, MetalTitanium, 0.018, tier, dt);
        }
    }

    private static void ProducePlanet03(GameState state, int tier, double dt)
    {
        // Planeta 3:
        // Tier 1  -> Níquel
        // Tier 10 -> Cobalto

        AddMetalPerSecond(state, MetalNickel, 0.028, tier, dt);

        if (tier >= 10)
        {
            AddMetalPerSecond(state, MetalCobalt, 0.011, tier, dt);
        }
    }

    private static void ProducePlanet04(GameState state, int tier, double dt)
    {
        // Planeta 4:
        // Tier 1  -> Litio
        // Tier 10 -> Tungsteno

        AddMetalPerSecond(state, MetalLithium, 0.018, tier, dt);

        if (tier >= 10)
        {
            AddMetalPerSecond(state, MetalTungsten, 0.007, tier, dt);
        }
    }

    private static void ProducePlanet05(GameState state, int tier, double dt)
    {
        // Planeta 5:
        // Tier 1  -> Platino
        // Tier 10 -> Níquel

        AddMetalPerSecond(state, MetalPlatinum, 0.012, tier, dt);

        if (tier >= 10)
        {
            AddMetalPerSecond(state, MetalNickel, 0.020, tier, dt);
        }
    }

    private static void ProducePlanet06(GameState state, int tier, double dt)
    {
        // Planeta 6:
        // Tier 1  -> Iridio
        // Tier 10 -> Cobalto

        AddMetalPerSecond(state, MetalIridium, 0.008, tier, dt);

        if (tier >= 10)
        {
            AddMetalPerSecond(state, MetalCobalt, 0.014, tier, dt);
        }
    }

    private static void ProducePlanet07(GameState state, int tier, double dt)
    {
        // Planeta 7:
        // Tier 1  -> Tungsteno
        // Tier 10 -> Platino
        // Tier 20 -> Iridio

        AddMetalPerSecond(state, MetalTungsten, 0.010, tier, dt);

        if (tier >= 10)
        {
            AddMetalPerSecond(state, MetalPlatinum, 0.007, tier, dt);
        }

        if (tier >= 20)
        {
            AddMetalPerSecond(state, MetalIridium, 0.004, tier, dt);
        }
    }

    private static void AddMetalPerSecond(
        GameState state,
        string metalId,
        double productionPerSecondPerTier,
        int tier,
        double dt
    )
    {
        AddMetalPerSecond(
            state,
            null,
            metalId,
            productionPerSecondPerTier,
            tier,
            dt
        );
    }

    private static void AddMetalPerSecond(
        GameState state,
        D1PlanetState planet,
        string metalId,
        double productionPerSecondPerTier,
        int tier,
        double dt
    )
    {
        if (productionPerSecondPerTier <= 0.0)
            return;

        if (tier <= 0)
            return;

        double multiplier = planet != null
            ? GetRelicMiningProductionMultiplier(state, planet, metalId)
            : 1.0;

        double amount = productionPerSecondPerTier * tier * dt * multiplier;

        if (amount <= 0.0)
            return;

        state.AddD1Metal(metalId, amount);
    }

    public static bool IsMetalUnlockedForDimension1(GameState state, string metalId)
    {
        if (state == null)
            return false;

        if (!state.dimension01Unlocked)
            return false;

        state.EnsureDimension1State();

        D1PlanetState planet01 = FindPlanetState(state, Planet01);
        D1PlanetState planet02 = FindPlanetState(state, Planet02);
        D1PlanetState planet03 = FindPlanetState(state, Planet03);
        D1PlanetState planet04 = FindPlanetState(state, Planet04);
        D1PlanetState planet05 = FindPlanetState(state, Planet05);
        D1PlanetState planet06 = FindPlanetState(state, Planet06);
        D1PlanetState planet07 = FindPlanetState(state, Planet07);

        switch (metalId)
        {
            case MetalIron:
                return planet01 != null && planet01.unlocked && planet01.extractorTier >= 1;

            case MetalCopper:
                return planet01 != null && planet01.unlocked && planet01.extractorTier >= 10;

            case MetalAluminum:
                return planet02 != null && planet02.unlocked && planet02.extractorTier >= 1;

            case MetalTitanium:
                return planet02 != null && planet02.unlocked && planet02.extractorTier >= 10;

            case MetalNickel:
                return planet03 != null && planet03.unlocked && planet03.extractorTier >= 1;

            case MetalCobalt:
                return planet03 != null && planet03.unlocked && planet03.extractorTier >= 10;

            case MetalLithium:
                return planet04 != null && planet04.unlocked && planet04.extractorTier >= 1;

            case MetalTungsten:
                return
                    planet04 != null && planet04.unlocked && planet04.extractorTier >= 10 ||
                    planet07 != null && planet07.unlocked && planet07.extractorTier >= 1;

            case MetalPlatinum:
                return
                    planet05 != null && planet05.unlocked && planet05.extractorTier >= 1 ||
                    planet07 != null && planet07.unlocked && planet07.extractorTier >= 10;

            case MetalIridium:
                return
                    planet06 != null && planet06.unlocked && planet06.extractorTier >= 1 ||
                    planet07 != null && planet07.unlocked && planet07.extractorTier >= 20;

            default:
                return false;
        }
    }

    public static double GetMetalProductionPerSecond(GameState state, string metalId)
    {
        if (state == null)
            return 0.0;

        if (!state.dimension01Unlocked)
            return 0.0;

        state.EnsureDimension1State();

        if (state.dimension1Planets == null)
            return 0.0;

        double total = 0.0;

        foreach (D1PlanetState planet in state.dimension1Planets)
        {
            if (planet == null)
                continue;

            if (!planet.unlocked)
                continue;

            if (planet.extractorTier <= 0)
                continue;

            double baseProduction = GetPlanetMetalProductionPerSecond(planet, metalId);

            if (baseProduction <= 0.0)
                continue;

            total += baseProduction * GetRelicMiningProductionMultiplier(
                state,
                planet,
                metalId
            );
        }

        return total;
    }

    private static double GetBaseMetalProductionPerSecond(GameState state, string metalId)
    {
        if (state == null)
            return 0.0;

        if (!state.dimension01Unlocked)
            return 0.0;

        state.EnsureDimension1State();

        if (state.dimension1Planets == null)
            return 0.0;

        double total = 0.0;

        foreach (D1PlanetState planet in state.dimension1Planets)
        {
            if (planet == null)
                continue;

            if (!planet.unlocked)
                continue;

            if (planet.extractorTier <= 0)
                continue;

            total += GetPlanetMetalProductionPerSecond(planet, metalId);
        }

        return total;
    }

    public static double GetPlanetMetalProductionPerSecond(D1PlanetState planet, string metalId)
    {
        if (planet == null)
            return 0.0;

        switch (planet.planetId)
        {
            case Planet01:
                if (metalId == MetalIron)
                    return 0.10 * planet.extractorTier;

                if (metalId == MetalCopper && planet.extractorTier >= 10)
                    return 0.04 * planet.extractorTier;

                break;

            case Planet02:
                if (metalId == MetalAluminum)
                    return 0.045 * planet.extractorTier;

                if (metalId == MetalTitanium && planet.extractorTier >= 10)
                    return 0.018 * planet.extractorTier;

                break;

            case Planet03:
                if (metalId == MetalNickel)
                    return 0.028 * planet.extractorTier;

                if (metalId == MetalCobalt && planet.extractorTier >= 10)
                    return 0.011 * planet.extractorTier;

                break;

            case Planet04:
                if (metalId == MetalLithium)
                    return 0.018 * planet.extractorTier;

                if (metalId == MetalTungsten && planet.extractorTier >= 10)
                    return 0.007 * planet.extractorTier;

                break;

            case Planet05:
                if (metalId == MetalPlatinum)
                    return 0.012 * planet.extractorTier;

                if (metalId == MetalNickel && planet.extractorTier >= 10)
                    return 0.020 * planet.extractorTier;

                break;

            case Planet06:
                if (metalId == MetalIridium)
                    return 0.008 * planet.extractorTier;

                if (metalId == MetalCobalt && planet.extractorTier >= 10)
                    return 0.014 * planet.extractorTier;

                break;

            case Planet07:
                if (metalId == MetalTungsten)
                    return 0.010 * planet.extractorTier;

                if (metalId == MetalPlatinum && planet.extractorTier >= 10)
                    return 0.007 * planet.extractorTier;

                if (metalId == MetalIridium && planet.extractorTier >= 20)
                    return 0.004 * planet.extractorTier;

                break;
        }

        return 0.0;
    }

    public static double GetExtractorUpgradeCost(D1PlanetState planet)
    {
        if (planet == null)
            return 0.0;

        int currentTier = Mathf.Max(planet.extractorTier, 1);

        switch (planet.planetId)
        {
            case Planet01:
                return 25.0 * Mathf.Pow(1.14f, currentTier - 1);

            case Planet02:
                return 60.0 * Mathf.Pow(1.11f, currentTier - 1);

            case Planet03:
                return 80.0 * Mathf.Pow(1.10f, currentTier - 1);

            case Planet04:
                return 140.0 * Mathf.Pow(1.105f, currentTier - 1);

            case Planet05:
                return 220.0 * Mathf.Pow(1.10f, currentTier - 1);

            case Planet06:
                return 320.0 * Mathf.Pow(1.095f, currentTier - 1);

            case Planet07:
                return 500.0 * Mathf.Pow(1.09f, currentTier - 1);

            default:
                return 999999.0;
        }
    }

    public static double GetExtractorUpgradeCost(GameState state, D1PlanetState planet)
    {
        double baseCost = GetExtractorUpgradeCost(planet);

        if (state == null || planet == null)
            return baseCost;

        return baseCost * GetRelicExtractorUpgradeCostMultiplier(state, planet);
    }

    public static string GetExtractorUpgradeMainCostMetal(D1PlanetState planet)
    {
        if (planet == null)
            return MetalIron;

        switch (planet.planetId)
        {
            case Planet01:
                return MetalIron;

            case Planet02:
                return MetalAluminum;

            case Planet03:
                return MetalNickel;

            case Planet04:
                return MetalLithium;

            case Planet05:
                return MetalPlatinum;

            case Planet06:
                return MetalIridium;

            case Planet07:
                return MetalTungsten;

            default:
                return MetalIron;
        }
    }

    public static bool TryGetPlanetUnlockCost(
    string planetId,
    out string metal1,
    out double amount1,
    out string metal2,
    out double amount2,
    out string metal3,
    out double amount3
)
    {
        metal1 = "";
        amount1 = 0.0;
        metal2 = "";
        amount2 = 0.0;
        metal3 = "";
        amount3 = 0.0;

        if (planetId == Planet02)
        {
            metal1 = MetalIron;
            amount1 = 500.0;
            metal2 = MetalCopper;
            amount2 = 120.0;
            return true;
        }

        if (planetId == Planet03)
        {
            metal1 = MetalAluminum;
            amount1 = 1500.0;
            metal2 = MetalTitanium;
            amount2 = 300.0;
            return true;
        }

        if (planetId == Planet04)
        {
            metal1 = MetalNickel;
            amount1 = 1200.0;
            metal2 = MetalCobalt;
            amount2 = 350.0;
            return true;
        }

        if (planetId == Planet05)
        {
            metal1 = MetalLithium;
            amount1 = 1400.0;
            metal2 = MetalTungsten;
            amount2 = 320.0;
            return true;
        }

        if (planetId == Planet06)
        {
            metal1 = MetalPlatinum;
            amount1 = 1100.0;
            metal2 = MetalNickel;
            amount2 = 1800.0;
            return true;
        }

        if (planetId == Planet07)
        {
            metal1 = MetalIridium;
            amount1 = 800.0;
            metal2 = MetalCobalt;
            amount2 = 1500.0;
            metal3 = MetalTungsten;
            amount3 = 900.0;
            return true;
        }

        return false;
    }

    private static bool HasRequiredD1Metal(GameState state, string metalId, double requiredAmount)
    {
        if (state == null)
            return false;

        if (string.IsNullOrEmpty(metalId))
            return true;

        if (requiredAmount <= 0.0)
            return true;

        return state.GetD1MetalAmount(metalId) >= requiredAmount;
    }

    public static bool CanUpgradeExtractor(GameState state, string planetId)
    {
        if (state == null)
            return false;

        if (!state.dimension01Unlocked)
            return false;

        state.EnsureDimension1State();

        D1PlanetState planet = null;

        foreach (D1PlanetState candidate in state.dimension1Planets)
        {
            if (candidate != null && candidate.planetId == planetId)
            {
                planet = candidate;
                break;
            }
        }

        if (planet == null || !planet.unlocked)
            return false;

        double cost = GetExtractorUpgradeCost(state, planet);
        string costMetal = GetExtractorUpgradeMainCostMetal(planet);

        return state.GetD1MetalAmount(costMetal) >= cost;
    }

    public static bool CanUnlockPlanet(GameState state, string planetId)
    {
        if (state == null)
            return false;

        if (!state.dimension01Unlocked)
            return false;

        state.EnsureDimension1State();

        D1PlanetState planet = FindPlanetState(state, planetId);

        if (planet == null)
            return false;

        if (planet.unlocked)
            return false;

        if (!TryGetPlanetUnlockCost(
            planetId,
            out string metal1,
            out double amount1,
            out string metal2,
            out double amount2,
            out string metal3,
            out double amount3
        ))
        {
            return false;
        }

        return
            HasRequiredD1Metal(state, metal1, amount1) &&
            HasRequiredD1Metal(state, metal2, amount2) &&
            HasRequiredD1Metal(state, metal3, amount3);
    }



    public static bool TryUpgradeExtractor(GameState state, string planetId)
    {
        if (state == null)
            return false;

        if (!state.dimension01Unlocked)
            return false;

        state.EnsureDimension1State();

        D1PlanetState planet = null;

        foreach (D1PlanetState candidate in state.dimension1Planets)
        {
            if (candidate != null && candidate.planetId == planetId)
            {
                planet = candidate;
                break;
            }
        }

        if (planet == null)
            return false;

        if (!planet.unlocked)
            return false;

        double cost = GetExtractorUpgradeCost(state, planet);
         string costMetal = GetExtractorUpgradeMainCostMetal(planet);

        if (!state.SpendD1Metal(costMetal, cost))
            return false;

        planet.extractorTier += 1;
        return true;
    }

    public static double ApplyOfflineMining(GameState state, double offlineSeconds)
    {
        if (state == null)
            return 0.0;

        if (offlineSeconds <= 0.0)
            return 0.0;

        if (!state.dimension01Unlocked)
            return 0.0;

        state.EnsureDimension1State();

        double cappedSeconds = System.Math.Min(offlineSeconds, DefaultOfflineCapSeconds);

        foreach (D1PlanetState planet in state.dimension1Planets)
        {
            if (planet == null)
                continue;

            if (!planet.unlocked)
                continue;

            if (planet.extractorTier <= 0)
                continue;

            ProducePlanetResources(state, planet, cappedSeconds);
        }

        UpdateActiveScan(state, cappedSeconds);
        UpdateActiveExplorations(state, cappedSeconds);

        return cappedSeconds;
    }

    public static bool TryUnlockPlanet(GameState state, string planetId)
    {
        if (state == null)
            return false;

        if (!state.dimension01Unlocked)
            return false;

        state.EnsureDimension1State();

        D1PlanetState planet = FindPlanetState(state, planetId);

        if (planet == null)
            return false;

        if (planet.unlocked)
            return true;

        if (!CanUnlockPlanet(state, planetId))
            return false;

        if (!TryGetPlanetUnlockCost(
            planetId,
            out string metal1,
            out double amount1,
            out string metal2,
            out double amount2,
            out string metal3,
            out double amount3
        ))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(metal1) && !state.SpendD1Metal(metal1, amount1))
            return false;

        if (!string.IsNullOrEmpty(metal2) && !state.SpendD1Metal(metal2, amount2))
            return false;

        if (!string.IsNullOrEmpty(metal3) && !state.SpendD1Metal(metal3, amount3))
            return false;

        planet.unlocked = true;
        planet.extractorTier = 1;

        return true;
    }

    public static bool TryGetShipUnlockCost(
    string shipId,
    out string metal1,
    out double amount1,
    out string metal2,
    out double amount2,
    out string metal3,
    out double amount3,
    out string metal4,
    out double amount4,
    out int blueprintCost
)
    {
        metal1 = "";
        amount1 = 0.0;
        metal2 = "";
        amount2 = 0.0;
        metal3 = "";
        amount3 = 0.0;
        metal4 = "";
        amount4 = 0.0;
        blueprintCost = 0;

        if (shipId == ShipExtractorDrone)
        {
            metal1 = MetalIron;
            amount1 = 300.0;
            metal2 = MetalCopper;
            amount2 = 80.0;
            return true;
        }

        if (shipId == ShipAnalyticProbe)
        {
            metal1 = MetalCopper;
            amount1 = 250.0;
            metal2 = MetalAluminum;
            amount2 = 120.0;
            return true;
        }

        if (shipId == ShipCargoShip)
        {
            metal1 = MetalTitanium;
            amount1 = 800.0;
            metal2 = MetalNickel;
            amount2 = 450.0;
            blueprintCost = 3;
            return true;
        }

        if (shipId == ShipRescueShip)
        {
            metal1 = MetalTitanium;
            amount1 = 1200.0;
            metal2 = MetalNickel;
            amount2 = 900.0;
            metal3 = MetalCobalt;
            amount3 = 500.0;
            metal4 = MetalPlatinum;
            amount4 = 250.0;
            blueprintCost = 4;
            return true;
        }

        if (shipId == ShipConvergenceShip)
        {
            metal1 = MetalPlatinum;
            amount1 = 900.0;
            metal2 = MetalIridium;
            amount2 = 550.0;
            metal3 = MetalCobalt;
            amount3 = 900.0;
            metal4 = MetalTungsten;
            amount4 = 1200.0;
            blueprintCost = 4;
            return true;
        }

        return false;
    }

    public static bool CanUnlockShip(GameState state, string shipId)
    {
        if (state == null)
            return false;

        if (!state.dimension01Unlocked)
            return false;

        if (string.IsNullOrEmpty(shipId))
            return false;

        state.EnsureDimension1State();

        D1ShipState ship = FindShipState(state, shipId);

        if (ship == null)
            return false;

        if (ship.unlocked)
            return false;

        if (!TryGetShipUnlockCost(
            shipId,
            out string metal1,
            out double amount1,
            out string metal2,
            out double amount2,
            out string metal3,
            out double amount3,
            out string metal4,
            out double amount4,
            out int blueprintCost
        ))
        {
            return false;
        }

        bool hasRequiredMatrices = UsesSpecificShipMatricesForUnlock(shipId)
            ? CanCoverRequiredShipMatrices(state, shipId)
            : CanSpendCompletedBlueprints(state, blueprintCost);

        return
            HasRequiredD1Metal(state, metal1, amount1) &&
            HasRequiredD1Metal(state, metal2, amount2) &&
            HasRequiredD1Metal(state, metal3, amount3) &&
            HasRequiredD1Metal(state, metal4, amount4) &&
            hasRequiredMatrices;
    }

    public static bool TryUnlockShip(GameState state, string shipId)
    {
        if (!CanUnlockShip(state, shipId))
            return false;

        D1ShipState ship = FindShipState(state, shipId);

        if (ship == null)
            return false;

        if (!TryGetShipUnlockCost(
            shipId,
            out string metal1,
            out double amount1,
            out string metal2,
            out double amount2,
            out string metal3,
            out double amount3,
            out string metal4,
            out double amount4,
            out int blueprintCost
        ))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(metal1) && !state.SpendD1Metal(metal1, amount1))
            return false;

        if (!string.IsNullOrEmpty(metal2) && !state.SpendD1Metal(metal2, amount2))
            return false;

        if (!string.IsNullOrEmpty(metal3) && !state.SpendD1Metal(metal3, amount3))
            return false;

        if (!string.IsNullOrEmpty(metal4) && !state.SpendD1Metal(metal4, amount4))
            return false;
            
        bool spentMatrices = UsesSpecificShipMatricesForUnlock(shipId)
            ? TrySpendRequiredShipMatrices(state, shipId)
            : TrySpendCompletedBlueprints(state, blueprintCost);

        if (!spentMatrices)
            return false;

        ship.unlocked = true;
        return true;
    }

    public static bool CanUpgradeShipPart(GameState state, string shipId, string partId)
    {
        if (state == null || !state.dimension01Unlocked)
            return false;

        if (string.IsNullOrEmpty(shipId) || string.IsNullOrEmpty(partId))
            return false;

        state.EnsureDimension1State();

        D1ShipState ship = FindShipState(state, shipId);

        if (ship == null || !ship.unlocked)
            return false;

        int currentLevel = GetShipPartLevel(ship, partId);
        int targetLevel = currentLevel + 1;

        if (!TryGetShipPartUpgradeCost(
            shipId,
            partId,
            targetLevel,
            out string metal1,
            out double amount1,
            out string metal2,
            out double amount2
        ))
        {
            return false;
        }

        if (state.GetD1MetalAmount(metal1) < amount1)
            return false;

        if (!string.IsNullOrEmpty(metal2) && state.GetD1MetalAmount(metal2) < amount2)
            return false;

        return true;
    }

    public static bool TryUpgradeShipPart(GameState state, string shipId, string partId)
    {
        if (!CanUpgradeShipPart(state, shipId, partId))
            return false;

        D1ShipState ship = FindShipState(state, shipId);

        if (ship == null)
            return false;

        int currentLevel = GetShipPartLevel(ship, partId);
        int targetLevel = currentLevel + 1;

        if (!TryGetShipPartUpgradeCost(
            shipId,
            partId,
            targetLevel,
            out string metal1,
            out double amount1,
            out string metal2,
            out double amount2
        ))
        {
            return false;
        }

        if (!state.SpendD1Metal(metal1, amount1))
            return false;

        if (!string.IsNullOrEmpty(metal2) && !state.SpendD1Metal(metal2, amount2))
            return false;

        SetShipPartLevel(ship, partId, targetLevel);

        return true;
    }

    public static bool TryGetNextShipPartUpgradeCost(
        GameState state,
        string shipId,
        string partId,
        out int nextLevel,
        out string metal1,
        out double amount1,
        out string metal2,
        out double amount2
        )
    {
        nextLevel = 0;
        metal1 = "";
        amount1 = 0.0;
        metal2 = "";
        amount2 = 0.0;

        if (state == null || !state.dimension01Unlocked)
            return false;

        if (string.IsNullOrEmpty(shipId) || string.IsNullOrEmpty(partId))
            return false;

        state.EnsureDimension1State();

        D1ShipState ship = FindShipState(state, shipId);

        if (ship == null || !ship.unlocked)
            return false;

        int currentLevel = GetShipPartLevel(ship, partId);
        nextLevel = currentLevel + 1;

        return TryGetShipPartUpgradeCost(
            shipId,
            partId,
            nextLevel,
            out metal1,
            out amount1,
            out metal2,
            out amount2
        );
    }

    public static bool TryGetAdvancedShipPartUpgradeCost(
    string shipId,
    string partId,
    int targetLevel,
    out string metal1,
    out double amount1,
    out string metal2,
    out double amount2,
    out int blueprintCost
)
    {
        metal1 = "";
        amount1 = 0.0;
        metal2 = "";
        amount2 = 0.0;
        blueprintCost = 0;

        if (targetLevel < 4 || targetLevel > 6)
            return false;

        if (shipId == ShipLightProbe && partId == ShipPartArmor)
        {
            if (targetLevel == 4)
            {
                metal1 = MetalTitanium;
                amount1 = 520.0;
                metal2 = MetalNickel;
                amount2 = 260.0;
                blueprintCost = 1;
                return true;
            }

            if (targetLevel == 5)
            {
                metal1 = MetalTitanium;
                amount1 = 760.0;
                metal2 = MetalTungsten;
                amount2 = 220.0;
                blueprintCost = 2;
                return true;
            }

            if (targetLevel == 6)
            {
                metal1 = MetalTungsten;
                amount1 = 520.0;
                metal2 = MetalIridium;
                amount2 = 180.0;
                blueprintCost = 3;
                return true;
            }
        }

        if (shipId == ShipExtractorDrone && partId == ShipPartArmor)
        {
            if (targetLevel == 4)
            {
                metal1 = MetalTitanium;
                amount1 = 820.0;
                metal2 = MetalTungsten;
                amount2 = 320.0;
                blueprintCost = 1;
                return true;
            }

            if (targetLevel == 5)
            {
                metal1 = MetalTungsten;
                amount1 = 680.0;
                metal2 = MetalCobalt;
                amount2 = 520.0;
                blueprintCost = 2;
                return true;
            }

            if (targetLevel == 6)
            {
                metal1 = MetalTungsten;
                amount1 = 1050.0;
                metal2 = MetalIridium;
                amount2 = 380.0;
                blueprintCost = 3;
                return true;
            }
        }

        if (shipId == ShipAnalyticProbe && partId == ShipPartArmor)
        {
            if (targetLevel == 4)
            {
                metal1 = MetalTitanium;
                amount1 = 760.0;
                metal2 = MetalCobalt;
                amount2 = 420.0;
                blueprintCost = 1;
                return true;
            }

            if (targetLevel == 5)
            {
                metal1 = MetalCobalt;
                amount1 = 760.0;
                metal2 = MetalPlatinum;
                amount2 = 300.0;
                blueprintCost = 2;
                return true;
            }

            if (targetLevel == 6)
            {
                metal1 = MetalTungsten;
                amount1 = 900.0;
                metal2 = MetalIridium;
                amount2 = 320.0;
                blueprintCost = 3;
                return true;
            }
        }

        if (shipId == ShipCargoShip && partId == ShipPartArmor)
        {
            if (targetLevel == 4)
            {
                metal1 = MetalTitanium;
                amount1 = 1600.0;
                metal2 = MetalTungsten;
                amount2 = 760.0;
                blueprintCost = 1;
                return true;
            }

            if (targetLevel == 5)
            {
                metal1 = MetalTungsten;
                amount1 = 1300.0;
                metal2 = MetalPlatinum;
                amount2 = 680.0;
                blueprintCost = 2;
                return true;
            }

            if (targetLevel == 6)
            {
                metal1 = MetalTungsten;
                amount1 = 1900.0;
                metal2 = MetalIridium;
                amount2 = 760.0;
                blueprintCost = 3;
                return true;
            }
        }

        if (shipId == ShipRescueShip && partId == ShipPartArmor)
        {
            if (targetLevel == 4)
            {
                metal1 = MetalTitanium;
                amount1 = 1800.0;
                metal2 = MetalPlatinum;
                amount2 = 760.0;
                blueprintCost = 1;
                return true;
            }

            if (targetLevel == 5)
            {
                metal1 = MetalTungsten;
                amount1 = 1500.0;
                metal2 = MetalPlatinum;
                amount2 = 1050.0;
                blueprintCost = 2;
                return true;
            }

            if (targetLevel == 6)
            {
                metal1 = MetalTungsten;
                amount1 = 2300.0;
                metal2 = MetalIridium;
                amount2 = 1100.0;
                blueprintCost = 3;
                return true;
            }
        }

        if (shipId == ShipConvergenceShip && partId == ShipPartArmor)
        {
            if (targetLevel == 4)
            {
                metal1 = MetalTungsten;
                amount1 = 2100.0;
                metal2 = MetalIridium;
                amount2 = 900.0;
                blueprintCost = 1;
                return true;
            }

            if (targetLevel == 5)
            {
                metal1 = MetalPlatinum;
                amount1 = 1900.0;
                metal2 = MetalIridium;
                amount2 = 1250.0;
                blueprintCost = 2;
                return true;
            }

            if (targetLevel == 6)
            {
                metal1 = MetalTungsten;
                amount1 = 2800.0;
                metal2 = MetalIridium;
                amount2 = 1800.0;
                blueprintCost = 3;
                return true;
            }
        }

        if (shipId == ShipLightProbe && partId == ShipPartSensors)
        {
            if (targetLevel == 4)
            {
                metal1 = MetalCopper;
                amount1 = 620.0;
                metal2 = MetalAluminum;
                amount2 = 340.0;
                blueprintCost = 1;
                return true;
            }

            if (targetLevel == 5)
            {
                metal1 = MetalAluminum;
                amount1 = 820.0;
                metal2 = MetalTitanium;
                amount2 = 300.0;
                blueprintCost = 2;
                return true;
            }

            if (targetLevel == 6)
            {
                metal1 = MetalTitanium;
                amount1 = 740.0;
                metal2 = MetalNickel;
                amount2 = 360.0;
                blueprintCost = 3;
                return true;
            }
        }

        if (shipId == ShipExtractorDrone && partId == ShipPartSensors)
        {
            if (targetLevel == 4)
            {
                metal1 = MetalCopper;
                amount1 = 820.0;
                metal2 = MetalAluminum;
                amount2 = 500.0;
                blueprintCost = 1;
                return true;
            }

            if (targetLevel == 5)
            {
                metal1 = MetalAluminum;
                amount1 = 1050.0;
                metal2 = MetalTitanium;
                amount2 = 450.0;
                blueprintCost = 2;
                return true;
            }

            if (targetLevel == 6)
            {
                metal1 = MetalTitanium;
                amount1 = 980.0;
                metal2 = MetalNickel;
                amount2 = 520.0;
                blueprintCost = 3;
                return true;
            }
        }

        if (shipId == ShipCargoShip && partId == ShipPartSensors)
        {
            if (targetLevel == 4)
            {
                metal1 = MetalCopper;
                amount1 = 1200.0;
                metal2 = MetalNickel;
                amount2 = 950.0;
                blueprintCost = 1;
                return true;
            }

            if (targetLevel == 5)
            {
                metal1 = MetalTitanium;
                amount1 = 1350.0;
                metal2 = MetalCobalt;
                amount2 = 760.0;
                blueprintCost = 2;
                return true;
            }

            if (targetLevel == 6)
            {
                metal1 = MetalNickel;
                amount1 = 1500.0;
                metal2 = MetalPlatinum;
                amount2 = 680.0;
                blueprintCost = 3;
                return true;
            }
        }

        if (shipId == ShipRescueShip && partId == ShipPartSensors)
        {
            if (targetLevel == 4)
            {
                metal1 = MetalTitanium;
                amount1 = 1350.0;
                metal2 = MetalPlatinum;
                amount2 = 540.0;
                blueprintCost = 1;
                return true;
            }

            if (targetLevel == 5)
            {
                metal1 = MetalCobalt;
                amount1 = 1300.0;
                metal2 = MetalPlatinum;
                amount2 = 820.0;
                blueprintCost = 2;
                return true;
            }

            if (targetLevel == 6)
            {
                metal1 = MetalTitanium;
                amount1 = 1850.0;
                metal2 = MetalPlatinum;
                amount2 = 1200.0;
                blueprintCost = 3;
                return true;
            }
        }

        if (shipId == ShipConvergenceShip && partId == ShipPartSensors)
        {
            if (targetLevel == 4)
            {
                metal1 = MetalPlatinum;
                amount1 = 1500.0;
                metal2 = MetalIridium;
                amount2 = 780.0;
                blueprintCost = 1;
                return true;
            }

            if (targetLevel == 5)
            {
                metal1 = MetalTungsten;
                amount1 = 1850.0;
                metal2 = MetalIridium;
                amount2 = 1100.0;
                blueprintCost = 2;
                return true;
            }

            if (targetLevel == 6)
            {
                metal1 = MetalPlatinum;
                amount1 = 2200.0;
                metal2 = MetalIridium;
                amount2 = 1550.0;
                blueprintCost = 3;
                return true;
            }
        }

        if (shipId == ShipLightProbe && partId == ShipPartCargo)
        {
            if (targetLevel == 4)
            {
                metal1 = MetalCopper;
                amount1 = 620.0;
                metal2 = MetalAluminum;
                amount2 = 320.0;
                blueprintCost = 1;
                return true;
            }

            if (targetLevel == 5)
            {
                metal1 = MetalAluminum;
                amount1 = 820.0;
                metal2 = MetalTitanium;
                amount2 = 260.0;
                blueprintCost = 2;
                return true;
            }

            if (targetLevel == 6)
            {
                metal1 = MetalTitanium;
                amount1 = 720.0;
                metal2 = MetalNickel;
                amount2 = 320.0;
                blueprintCost = 3;
                return true;
            }
        }

        if (shipId == ShipAnalyticProbe && partId == ShipPartCargo)
        {
            if (targetLevel == 4)
            {
                metal1 = MetalAluminum;
                amount1 = 700.0;
                metal2 = MetalTitanium;
                amount2 = 300.0;
                blueprintCost = 1;
                return true;
            }

            if (targetLevel == 5)
            {
                metal1 = MetalTitanium;
                amount1 = 760.0;
                metal2 = MetalNickel;
                amount2 = 390.0;
                blueprintCost = 2;
                return true;
            }

            if (targetLevel == 6)
            {
                metal1 = MetalNickel;
                amount1 = 850.0;
                metal2 = MetalCobalt;
                amount2 = 380.0;
                blueprintCost = 3;
                return true;
            }
        }

        if (shipId == ShipRescueShip && partId == ShipPartCargo)
        {
            if (targetLevel == 4)
            {
                metal1 = MetalTitanium;
                amount1 = 1450.0;
                metal2 = MetalPlatinum;
                amount2 = 520.0;
                blueprintCost = 1;
                return true;
            }

            if (targetLevel == 5)
            {
                metal1 = MetalCobalt;
                amount1 = 1350.0;
                metal2 = MetalPlatinum;
                amount2 = 760.0;
                blueprintCost = 2;
                return true;
            }

            if (targetLevel == 6)
            {
                metal1 = MetalTitanium;
                amount1 = 1900.0;
                metal2 = MetalPlatinum;
                amount2 = 1100.0;
                blueprintCost = 3;
                return true;
            }
        }

        if (shipId == ShipConvergenceShip && partId == ShipPartCargo)
        {
            if (targetLevel == 4)
            {
                metal1 = MetalTungsten;
                amount1 = 1700.0;
                metal2 = MetalIridium;
                amount2 = 760.0;
                blueprintCost = 1;
                return true;
            }

            if (targetLevel == 5)
            {
                metal1 = MetalPlatinum;
                amount1 = 1600.0;
                metal2 = MetalIridium;
                amount2 = 1050.0;
                blueprintCost = 2;
                return true;
            }

            if (targetLevel == 6)
            {
                metal1 = MetalTungsten;
                amount1 = 2300.0;
                metal2 = MetalIridium;
                amount2 = 1450.0;
                blueprintCost = 3;
                return true;
            }
        }

        if (shipId == ShipLightProbe && partId == ShipPartSpeed)
        {
            if (targetLevel == 4)
            {
                metal1 = MetalCopper;
                amount1 = 700.0;
                metal2 = MetalAluminum;
                amount2 = 450.0;
                blueprintCost = 1;
                return true;
            }

            if (targetLevel == 5)
            {
                metal1 = MetalAluminum;
                amount1 = 950.0;
                metal2 = MetalTitanium;
                amount2 = 350.0;
                blueprintCost = 2;
                return true;
            }

            if (targetLevel == 6)
            {
                metal1 = MetalTitanium;
                amount1 = 850.0;
                metal2 = MetalNickel;
                amount2 = 400.0;
                blueprintCost = 3;
                return true;
            }
        }

        if (shipId == ShipExtractorDrone && partId == ShipPartCargo)
        {
            if (targetLevel == 4)
            {
                metal1 = MetalCopper;
                amount1 = 900.0;
                metal2 = MetalAluminum;
                amount2 = 550.0;
                blueprintCost = 1;
                return true;
            }

            if (targetLevel == 5)
            {
                metal1 = MetalAluminum;
                amount1 = 1200.0;
                metal2 = MetalTitanium;
                amount2 = 500.0;
                blueprintCost = 2;
                return true;
            }

            if (targetLevel == 6)
            {
                metal1 = MetalTitanium;
                amount1 = 1100.0;
                metal2 = MetalNickel;
                amount2 = 550.0;
                blueprintCost = 3;
                return true;
            }
        }

        if (shipId == ShipExtractorDrone && partId == ShipPartSpeed)
        {
            if (targetLevel == 4)
            {
                metal1 = MetalAluminum;
                amount1 = 700.0;
                metal2 = MetalLithium;
                amount2 = 350.0;
                blueprintCost = 1;
                return true;
            }

            if (targetLevel == 5)
            {
                metal1 = MetalLithium;
                amount1 = 600.0;
                metal2 = MetalCobalt;
                amount2 = 400.0;
                blueprintCost = 2;
                return true;
            }

            if (targetLevel == 6)
            {
                metal1 = MetalLithium;
                amount1 = 900.0;
                metal2 = MetalCobalt;
                amount2 = 650.0;
                blueprintCost = 3;
                return true;
            }
        }

        if (shipId == ShipAnalyticProbe && partId == ShipPartSensors)
        {
            if (targetLevel == 4)
            {
                metal1 = MetalAluminum;
                amount1 = 800.0;
                metal2 = MetalTitanium;
                amount2 = 350.0;
                blueprintCost = 1;
                return true;
            }

            if (targetLevel == 5)
            {
                metal1 = MetalTitanium;
                amount1 = 850.0;
                metal2 = MetalNickel;
                amount2 = 450.0;
                blueprintCost = 2;
                return true;
            }

            if (targetLevel == 6)
            {
                metal1 = MetalNickel;
                amount1 = 950.0;
                metal2 = MetalCobalt;
                amount2 = 420.0;
                blueprintCost = 3;
                return true;
            }
        }

        if (shipId == ShipAnalyticProbe && partId == ShipPartSpeed)
        {
            if (targetLevel == 4)
            {
                metal1 = MetalTitanium;
                amount1 = 750.0;
                metal2 = MetalLithium;
                amount2 = 350.0;
                blueprintCost = 1;
                return true;
            }

            if (targetLevel == 5)
            {
                metal1 = MetalLithium;
                amount1 = 650.0;
                metal2 = MetalCobalt;
                amount2 = 420.0;
                blueprintCost = 2;
                return true;
            }

            if (targetLevel == 6)
            {
                metal1 = MetalLithium;
                amount1 = 950.0;
                metal2 = MetalCobalt;
                amount2 = 700.0;
                blueprintCost = 3;
                return true;
            }
        }

        if (shipId == ShipCargoShip && partId == ShipPartCargo)
        {
            if (targetLevel == 4)
            {
                metal1 = MetalTitanium;
                amount1 = 1200.0;
                metal2 = MetalNickel;
                amount2 = 1300.0;
                blueprintCost = 1;
                return true;
            }

            if (targetLevel == 5)
            {
                metal1 = MetalNickel;
                amount1 = 1700.0;
                metal2 = MetalCobalt;
                amount2 = 900.0;
                blueprintCost = 2;
                return true;
            }

            if (targetLevel == 6)
            {
                metal1 = MetalTitanium;
                amount1 = 2200.0;
                metal2 = MetalCobalt;
                amount2 = 1400.0;
                blueprintCost = 3;
                return true;
            }
        }

        if (shipId == ShipCargoShip && partId == ShipPartSpeed)
        {
            if (targetLevel == 4)
            {
                metal1 = MetalTitanium;
                amount1 = 900.0;
                metal2 = MetalLithium;
                amount2 = 450.0;
                blueprintCost = 1;
                return true;
            }

            if (targetLevel == 5)
            {
                metal1 = MetalLithium;
                amount1 = 800.0;
                metal2 = MetalCobalt;
                amount2 = 550.0;
                blueprintCost = 2;
                return true;
            }

            if (targetLevel == 6)
            {
                metal1 = MetalLithium;
                amount1 = 1200.0;
                metal2 = MetalCobalt;
                amount2 = 850.0;
                blueprintCost = 3;
                return true;
            }
        }

        if (shipId == ShipCargoShip && partId == ShipPartSpeed)
        {
            if (targetLevel == 1)
            {
                metal1 = MetalAluminum;
                amount1 = 520.0;
                metal2 = MetalTitanium;
                amount2 = 260.0;
                return true;
            }

            if (targetLevel == 2)
            {
                metal1 = MetalAluminum;
                amount1 = 900.0;
                metal2 = MetalLithium;
                amount2 = 360.0;
                return true;
            }

            if (targetLevel == 3)
            {
                metal1 = MetalTitanium;
                amount1 = 1050.0;
                metal2 = MetalLithium;
                amount2 = 620.0;
                return true;
            }
        }

        if (shipId == ShipRescueShip && partId == ShipPartSpeed)
        {
            if (targetLevel == 4)
            {
                metal1 = MetalTitanium;
                amount1 = 1700.0;
                metal2 = MetalPlatinum;
                amount2 = 600.0;
                blueprintCost = 1;
                return true;
            }

            if (targetLevel == 5)
            {
                metal1 = MetalCobalt;
                amount1 = 1600.0;
                metal2 = MetalPlatinum;
                amount2 = 900.0;
                blueprintCost = 2;
                return true;
            }

            if (targetLevel == 6)
            {
                metal1 = MetalTitanium;
                amount1 = 2200.0;
                metal2 = MetalPlatinum;
                amount2 = 1300.0;
                blueprintCost = 3;
                return true;
            }
        }

        if (shipId == ShipConvergenceShip && partId == ShipPartSpeed)
        {
            if (targetLevel == 4)
            {
                metal1 = MetalTungsten;
                amount1 = 1900.0;
                metal2 = MetalIridium;
                amount2 = 900.0;
                blueprintCost = 1;
                return true;
            }

            if (targetLevel == 5)
            {
                metal1 = MetalPlatinum;
                amount1 = 1800.0;
                metal2 = MetalIridium;
                amount2 = 1250.0;
                blueprintCost = 2;
                return true;
            }

            if (targetLevel == 6)
            {
                metal1 = MetalTungsten;
                amount1 = 2600.0;
                metal2 = MetalIridium;
                amount2 = 1700.0;
                blueprintCost = 3;
                return true;
            }
        }

        return false;
    }

    public static bool TryGetNextAdvancedShipPartUpgradeCost(
        GameState state,
        string shipId,
        string partId,
        out int nextLevel,
        out string metal1,
        out double amount1,
        out string metal2,
        out double amount2,
        out int blueprintCost
    )
    {
        nextLevel = 0;
        metal1 = "";
        amount1 = 0.0;
        metal2 = "";
        amount2 = 0.0;
        blueprintCost = 0;

        if (state == null || !state.dimension01Unlocked)
            return false;

        if (string.IsNullOrEmpty(shipId) || string.IsNullOrEmpty(partId))
            return false;

        state.EnsureDimension1State();

        D1ShipState ship = FindShipState(state, shipId);

        if (ship == null || !ship.unlocked)
            return false;

        int currentLevel = GetShipPartLevel(ship, partId);
        nextLevel = currentLevel + 1;

        return TryGetAdvancedShipPartUpgradeCost(
            shipId,
            partId,
            nextLevel,
            out metal1,
            out amount1,
            out metal2,
            out amount2,
            out blueprintCost
        );
    }

    public static bool CanUpgradeAdvancedShipPart(
        GameState state,
        string shipId,
        string partId
    )
    {
        if (!TryGetNextAdvancedShipPartUpgradeCost(
            state,
            shipId,
            partId,
            out int nextLevel,
            out string metal1,
            out double amount1,
            out string metal2,
            out double amount2,
            out int blueprintCost
        ))
        {
            return false;
        }

        if (state.GetD1MetalAmount(metal1) < amount1)
            return false;

        if (!string.IsNullOrEmpty(metal2) && state.GetD1MetalAmount(metal2) < amount2)
            return false;

        if (!CanSpendCompletedBlueprints(state, blueprintCost))
            return false;

        int specificMatrixCost = GetRequiredSpecificShipUpgradeMatrixCost(shipId, nextLevel);

        if (!CanSpendSpecificShipUpgradeMatrices(state, shipId, specificMatrixCost))
            return false;

        return true;
    }

    public static bool TryUpgradeAdvancedShipPart(
        GameState state,
        string shipId,
        string partId
    )
    {
        if (!CanUpgradeAdvancedShipPart(state, shipId, partId))
            return false;

        D1ShipState ship = FindShipState(state, shipId);

        if (ship == null)
            return false;

        int currentLevel = GetShipPartLevel(ship, partId);
        int targetLevel = currentLevel + 1;

        if (!TryGetAdvancedShipPartUpgradeCost(
            shipId,
            partId,
            targetLevel,
            out string metal1,
            out double amount1,
            out string metal2,
            out double amount2,
            out int blueprintCost
        ))
        {
            return false;
        }

        if (!state.SpendD1Metal(metal1, amount1))
            return false;

        if (!string.IsNullOrEmpty(metal2) && !state.SpendD1Metal(metal2, amount2))
            return false;

        if (!TrySpendCompletedBlueprints(state, blueprintCost))
            return false;

        int specificMatrixCost = GetRequiredSpecificShipUpgradeMatrixCost(shipId, targetLevel);

        if (!TrySpendSpecificShipUpgradeMatrices(state, shipId, specificMatrixCost))
            return false;

        SetShipPartLevel(ship, partId, targetLevel);

        return true;
    }

    private static bool TryGetShipPartUpgradeCost(
        string shipId,
        string partId,
        int targetLevel,
        out string metal1,
        out double amount1,
        out string metal2,
        out double amount2
    )
    {
        metal1 = "";
        amount1 = 0.0;
        metal2 = "";
        amount2 = 0.0;

        if (targetLevel < 1 || targetLevel > 3)
            return false;

        if (shipId == ShipLightProbe && partId == ShipPartArmor)
        {
            if (targetLevel == 1)
            {
                metal1 = MetalIron;
                amount1 = 120.0;
                metal2 = MetalTitanium;
                amount2 = 45.0;
                return true;
            }

            if (targetLevel == 2)
            {
                metal1 = MetalIron;
                amount1 = 280.0;
                metal2 = MetalTitanium;
                amount2 = 110.0;
                return true;
            }

            if (targetLevel == 3)
            {
                metal1 = MetalAluminum;
                amount1 = 420.0;
                metal2 = MetalTitanium;
                amount2 = 260.0;
                return true;
            }
        }

        if (shipId == ShipExtractorDrone && partId == ShipPartArmor)
        {
            if (targetLevel == 1)
            {
                metal1 = MetalIron;
                amount1 = 180.0;
                metal2 = MetalTitanium;
                amount2 = 90.0;
                return true;
            }

            if (targetLevel == 2)
            {
                metal1 = MetalTitanium;
                amount1 = 260.0;
                metal2 = MetalNickel;
                amount2 = 140.0;
                return true;
            }

            if (targetLevel == 3)
            {
                metal1 = MetalTitanium;
                amount1 = 520.0;
                metal2 = MetalNickel;
                amount2 = 320.0;
                return true;
            }
        }

        if (shipId == ShipAnalyticProbe && partId == ShipPartArmor)
        {
            if (targetLevel == 1)
            {
                metal1 = MetalAluminum;
                amount1 = 180.0;
                metal2 = MetalTitanium;
                amount2 = 90.0;
                return true;
            }

            if (targetLevel == 2)
            {
                metal1 = MetalTitanium;
                amount1 = 280.0;
                metal2 = MetalNickel;
                amount2 = 150.0;
                return true;
            }

            if (targetLevel == 3)
            {
                metal1 = MetalTitanium;
                amount1 = 560.0;
                metal2 = MetalCobalt;
                amount2 = 260.0;
                return true;
            }
        }

        if (shipId == ShipCargoShip && partId == ShipPartArmor)
        {
            if (targetLevel == 1)
            {
                metal1 = MetalTitanium;
                amount1 = 650.0;
                metal2 = MetalNickel;
                amount2 = 360.0;
                return true;
            }

            if (targetLevel == 2)
            {
                metal1 = MetalTitanium;
                amount1 = 1050.0;
                metal2 = MetalCobalt;
                amount2 = 520.0;
                return true;
            }

            if (targetLevel == 3)
            {
                metal1 = MetalNickel;
                amount1 = 1250.0;
                metal2 = MetalTungsten;
                amount2 = 520.0;
                return true;
            }
        }

        if (shipId == ShipRescueShip && partId == ShipPartArmor)
        {
            if (targetLevel == 1)
            {
                metal1 = MetalTitanium;
                amount1 = 850.0;
                metal2 = MetalNickel;
                amount2 = 520.0;
                return true;
            }

            if (targetLevel == 2)
            {
                metal1 = MetalTitanium;
                amount1 = 1300.0;
                metal2 = MetalCobalt;
                amount2 = 760.0;
                return true;
            }

            if (targetLevel == 3)
            {
                metal1 = MetalTungsten;
                amount1 = 900.0;
                metal2 = MetalPlatinum;
                amount2 = 480.0;
                return true;
            }
        }

        if (shipId == ShipConvergenceShip && partId == ShipPartArmor)
        {
            if (targetLevel == 1)
            {
                metal1 = MetalTungsten;
                amount1 = 900.0;
                metal2 = MetalCobalt;
                amount2 = 760.0;
                return true;
            }

            if (targetLevel == 2)
            {
                metal1 = MetalTungsten;
                amount1 = 1350.0;
                metal2 = MetalPlatinum;
                amount2 = 760.0;
                return true;
            }

            if (targetLevel == 3)
            {
                metal1 = MetalTungsten;
                amount1 = 1800.0;
                metal2 = MetalIridium;
                amount2 = 720.0;
                return true;
            }
        }

        if (shipId == ShipLightProbe && partId == ShipPartSensors)
        {
            if (targetLevel == 1)
            {
                metal1 = MetalCopper;
                amount1 = 70.0;
                metal2 = MetalAluminum;
                amount2 = 35.0;
                return true;
            }

            if (targetLevel == 2)
            {
                metal1 = MetalCopper;
                amount1 = 170.0;
                metal2 = MetalAluminum;
                amount2 = 90.0;
                return true;
            }

            if (targetLevel == 3)
            {
                metal1 = MetalCopper;
                amount1 = 360.0;
                metal2 = MetalAluminum;
                amount2 = 210.0;
                return true;
            }
        }

        if (shipId == ShipExtractorDrone && partId == ShipPartSensors)
        {
            if (targetLevel == 1)
            {
                metal1 = MetalCopper;
                amount1 = 100.0;
                metal2 = MetalAluminum;
                amount2 = 50.0;
                return true;
            }

            if (targetLevel == 2)
            {
                metal1 = MetalCopper;
                amount1 = 240.0;
                metal2 = MetalAluminum;
                amount2 = 120.0;
                return true;
            }

            if (targetLevel == 3)
            {
                metal1 = MetalCopper;
                amount1 = 500.0;
                metal2 = MetalAluminum;
                amount2 = 280.0;
                return true;
            }
        }

        if (shipId == ShipCargoShip && partId == ShipPartSensors)
        {
            if (targetLevel == 1)
            {
                metal1 = MetalCopper;
                amount1 = 420.0;
                metal2 = MetalNickel;
                amount2 = 260.0;
                return true;
            }

            if (targetLevel == 2)
            {
                metal1 = MetalCopper;
                amount1 = 760.0;
                metal2 = MetalNickel;
                amount2 = 520.0;
                return true;
            }

            if (targetLevel == 3)
            {
                metal1 = MetalTitanium;
                amount1 = 820.0;
                metal2 = MetalNickel;
                amount2 = 900.0;
                return true;
            }
        }

        if (shipId == ShipRescueShip && partId == ShipPartSensors)
        {
            if (targetLevel == 1)
            {
                metal1 = MetalCopper;
                amount1 = 520.0;
                metal2 = MetalNickel;
                amount2 = 320.0;
                return true;
            }

            if (targetLevel == 2)
            {
                metal1 = MetalNickel;
                amount1 = 760.0;
                metal2 = MetalCobalt;
                amount2 = 430.0;
                return true;
            }

            if (targetLevel == 3)
            {
                metal1 = MetalTitanium;
                amount1 = 1050.0;
                metal2 = MetalCobalt;
                amount2 = 720.0;
                return true;
            }
        }

        if (shipId == ShipConvergenceShip && partId == ShipPartSensors)
        {
            if (targetLevel == 1)
            {
                metal1 = MetalCobalt;
                amount1 = 760.0;
                metal2 = MetalPlatinum;
                amount2 = 500.0;
                return true;
            }

            if (targetLevel == 2)
            {
                metal1 = MetalPlatinum;
                amount1 = 880.0;
                metal2 = MetalTungsten;
                amount2 = 900.0;
                return true;
            }

            if (targetLevel == 3)
            {
                metal1 = MetalPlatinum;
                amount1 = 1250.0;
                metal2 = MetalIridium;
                amount2 = 700.0;
                return true;
            }
        }

        if (shipId == ShipLightProbe && partId == ShipPartCargo)
        {
            if (targetLevel == 1)
            {
                metal1 = MetalIron;
                amount1 = 80.0;
                metal2 = MetalCopper;
                amount2 = 25.0;
                return true;
            }

            if (targetLevel == 2)
            {
                metal1 = MetalIron;
                amount1 = 220.0;
                metal2 = MetalCopper;
                amount2 = 75.0;
                return true;
            }

            if (targetLevel == 3)
            {
                metal1 = MetalIron;
                amount1 = 480.0;
                metal2 = MetalCopper;
                amount2 = 180.0;
                return true;
            }
        }

        if (shipId == ShipAnalyticProbe && partId == ShipPartCargo)
        {
            if (targetLevel == 1)
            {
                metal1 = MetalIron;
                amount1 = 120.0;
                metal2 = MetalCopper;
                amount2 = 80.0;
                return true;
            }

            if (targetLevel == 2)
            {
                metal1 = MetalIron;
                amount1 = 280.0;
                metal2 = MetalCopper;
                amount2 = 180.0;
                return true;
            }

            if (targetLevel == 3)
            {
                metal1 = MetalIron;
                amount1 = 620.0;
                metal2 = MetalAluminum;
                amount2 = 260.0;
                return true;
            }
        }

        if (shipId == ShipRescueShip && partId == ShipPartCargo)
        {
            if (targetLevel == 1)
            {
                metal1 = MetalTitanium;
                amount1 = 520.0;
                metal2 = MetalNickel;
                amount2 = 260.0;
                return true;
            }

            if (targetLevel == 2)
            {
                metal1 = MetalNickel;
                amount1 = 760.0;
                metal2 = MetalCobalt;
                amount2 = 420.0;
                return true;
            }

            if (targetLevel == 3)
            {
                metal1 = MetalTitanium;
                amount1 = 1100.0;
                metal2 = MetalCobalt;
                amount2 = 700.0;
                return true;
            }
        }

        if (shipId == ShipConvergenceShip && partId == ShipPartCargo)
        {
            if (targetLevel == 1)
            {
                metal1 = MetalCobalt;
                amount1 = 780.0;
                metal2 = MetalPlatinum;
                amount2 = 420.0;
                return true;
            }

            if (targetLevel == 2)
            {
                metal1 = MetalTungsten;
                amount1 = 1050.0;
                metal2 = MetalPlatinum;
                amount2 = 720.0;
                return true;
            }

            if (targetLevel == 3)
            {
                metal1 = MetalTungsten;
                amount1 = 1450.0;
                metal2 = MetalIridium;
                amount2 = 580.0;
                return true;
            }
        }

        if (shipId == ShipLightProbe && partId == ShipPartSpeed)
        {
            if (targetLevel == 1)
            {
                metal1 = MetalIron;
                amount1 = 100.0;
                metal2 = MetalCopper;
                amount2 = 30.0;
                return true;
            }

            if (targetLevel == 2)
            {
                metal1 = MetalIron;
                amount1 = 250.0;
                metal2 = MetalCopper;
                amount2 = 90.0;
                return true;
            }

            if (targetLevel == 3)
            {
                metal1 = MetalIron;
                amount1 = 550.0;
                metal2 = MetalCopper;
                amount2 = 220.0;
                return true;
            }
        }

        if (shipId == ShipExtractorDrone && partId == ShipPartCargo)
        {
            if (targetLevel == 1)
            {
                metal1 = MetalIron;
                amount1 = 160.0;
                metal2 = MetalCopper;
                amount2 = 60.0;
                return true;
            }

            if (targetLevel == 2)
            {
                metal1 = MetalIron;
                amount1 = 350.0;
                metal2 = MetalCopper;
                amount2 = 120.0;
                return true;
            }

            if (targetLevel == 3)
            {
                metal1 = MetalIron;
                amount1 = 750.0;
                metal2 = MetalCopper;
                amount2 = 280.0;
                return true;
            }
        }

        if (shipId == ShipExtractorDrone && partId == ShipPartSpeed)
        {
            if (targetLevel == 1)
            {
                metal1 = MetalCopper;
                amount1 = 120.0;
                metal2 = MetalAluminum;
                amount2 = 70.0;
                return true;
            }

            if (targetLevel == 2)
            {
                metal1 = MetalAluminum;
                amount1 = 280.0;
                metal2 = MetalTitanium;
                amount2 = 110.0;
                return true;
            }

            if (targetLevel == 3)
            {
                metal1 = MetalAluminum;
                amount1 = 620.0;
                metal2 = MetalTitanium;
                amount2 = 260.0;
                return true;
            }
        }

        if (shipId == ShipAnalyticProbe && partId == ShipPartSensors)
        {
            if (targetLevel == 1)
            {
                metal1 = MetalCopper;
                amount1 = 120.0;
                metal2 = MetalAluminum;
                amount2 = 60.0;
                return true;
            }

            if (targetLevel == 2)
            {
                metal1 = MetalCopper;
                amount1 = 260.0;
                metal2 = MetalAluminum;
                amount2 = 140.0;
                return true;
            }

            if (targetLevel == 3)
            {
                metal1 = MetalCopper;
                amount1 = 520.0;
                metal2 = MetalAluminum;
                amount2 = 320.0;
                return true;
            }
        }

        if (shipId == ShipAnalyticProbe && partId == ShipPartSpeed)
        {
            if (targetLevel == 1)
            {
                metal1 = MetalCopper;
                amount1 = 160.0;
                metal2 = MetalAluminum;
                amount2 = 90.0;
                return true;
            }

            if (targetLevel == 2)
            {
                metal1 = MetalAluminum;
                amount1 = 320.0;
                metal2 = MetalTitanium;
                amount2 = 140.0;
                return true;
            }

            if (targetLevel == 3)
            {
                metal1 = MetalAluminum;
                amount1 = 700.0;
                metal2 = MetalTitanium;
                amount2 = 320.0;
                return true;
            }
        }

        if (shipId == ShipCargoShip && partId == ShipPartCargo)
        {
            if (targetLevel == 1)
            {
                metal1 = MetalIron;
                amount1 = 900.0;
                metal2 = MetalNickel;
                amount2 = 300.0;
                return true;
            }

            if (targetLevel == 2)
            {
                metal1 = MetalIron;
                amount1 = 1800.0;
                metal2 = MetalNickel;
                amount2 = 650.0;
                return true;
            }

            if (targetLevel == 3)
            {
                metal1 = MetalTitanium;
                amount1 = 850.0;
                metal2 = MetalNickel;
                amount2 = 1100.0;
                return true;
            }
        }

        if (shipId == ShipCargoShip && partId == ShipPartSpeed)
        {
            if (targetLevel == 1)
            {
                metal1 = MetalAluminum;
                amount1 = 520.0;
                metal2 = MetalTitanium;
                amount2 = 260.0;
                return true;
            }

            if (targetLevel == 2)
            {
                metal1 = MetalAluminum;
                amount1 = 900.0;
                metal2 = MetalLithium;
                amount2 = 360.0;
                return true;
            }

            if (targetLevel == 3)
            {
                metal1 = MetalTitanium;
                amount1 = 1050.0;
                metal2 = MetalLithium;
                amount2 = 620.0;
                return true;
            }
        }

        if (shipId == ShipRescueShip && partId == ShipPartSpeed)
        {
            if (targetLevel == 1)
            {
                metal1 = MetalTitanium;
                amount1 = 650.0;
                metal2 = MetalNickel;
                amount2 = 420.0;
                return true;
            }

            if (targetLevel == 2)
            {
                metal1 = MetalNickel;
                amount1 = 900.0;
                metal2 = MetalCobalt;
                amount2 = 520.0;
                return true;
            }

            if (targetLevel == 3)
            {
                metal1 = MetalTitanium;
                amount1 = 1250.0;
                metal2 = MetalCobalt;
                amount2 = 780.0;
                return true;
            }
        }

        if (shipId == ShipConvergenceShip && partId == ShipPartSpeed)
        {
            if (targetLevel == 1)
            {
                metal1 = MetalCobalt;
                amount1 = 900.0;
                metal2 = MetalPlatinum;
                amount2 = 550.0;
                return true;
            }

            if (targetLevel == 2)
            {
                metal1 = MetalTungsten;
                amount1 = 1200.0;
                metal2 = MetalPlatinum;
                amount2 = 850.0;
                return true;
            }

            if (targetLevel == 3)
            {
                metal1 = MetalTungsten;
                amount1 = 1600.0;
                metal2 = MetalIridium;
                amount2 = 650.0;
                return true;
            }
        }

        return false;
    }

    private static int GetShipPartLevel(D1ShipState ship, string partId)
    {
        if (ship == null)
            return 0;

        switch (partId)
        {
            case ShipPartCargo:
                return ship.cargoLevel;

            case ShipPartSpeed:
                return ship.speedLevel;

            case ShipPartArmor:
                return ship.armorLevel;

            case ShipPartSensors:
                return ship.sensorsLevel;

            default:
                return 0;
        }
    }

    private static void SetShipPartLevel(D1ShipState ship, string partId, int level)
    {
        if (ship == null)
            return;

        switch (partId)
        {
            case ShipPartCargo:
                ship.cargoLevel = level;
                break;

            case ShipPartSpeed:
                ship.speedLevel = level;
                break;

            case ShipPartArmor:
                ship.armorLevel = level;
                break;

            case ShipPartSensors:
                ship.sensorsLevel = level;
                break;
        }
    }

    public static bool CanScanSimpleDestination(GameState state)
    {
        if (state == null)
            return false;

        if (!state.dimension01Unlocked)
            return false;

        state.EnsureDimension1State();

        if (state.dimension1ScanActive)
            return false;

        return true;
    }

    public static bool TryScanSimpleDestination(GameState state)
    {
        if (!CanScanSimpleDestination(state))
            return false;

        if (state.dimension1ScannedDestinations == null)
            state.dimension1ScannedDestinations = new List<D1ScannedDestinationState>();

        state.dimension1ScannedDestinations.Clear();

        double scanDuration = GetSimpleScanDurationSeconds(state);

        state.dimension1ScanActive = true;
        state.dimension1ScanTotalSeconds = scanDuration;
        state.dimension1ScanRemainingSeconds = scanDuration;

        return true;
    }

    public static bool CanStartLightProbeExploration(GameState state)
    {
        return CanStartLightProbeExploration(state, 0);
    }

    public static bool CanStartLightProbeExploration(GameState state, int destinationIndex)
    {
        return CanStartExploration(state, ShipLightProbe, destinationIndex);
    }

    public static bool CanStartExploration(GameState state, string shipId, int destinationIndex)
    {
        if (state == null)
            return false;

        if (!state.dimension01Unlocked)
            return false;

        if (string.IsNullOrEmpty(shipId))
            return false;

        state.EnsureDimension1State();

        if (state.dimension1ScanActive)
            return false;

        D1ShipState ship = FindShipState(state, shipId);

        if (ship == null || !ship.unlocked)
            return false;

        if (ship.explorationActive)
            return false;

        return GetAvailableScannedDestinationByIndex(state, destinationIndex) != null;
    }

    public static bool TryStartLightProbeExploration(GameState state)
    {
        return TryStartExploration(state, ShipLightProbe, 0);
    }

    public static bool TryStartLightProbeExploration(GameState state, int destinationIndex)
    {
        return TryStartExploration(state, ShipLightProbe, destinationIndex);
    }

    public static bool TryStartExploration(GameState state, string shipId, int destinationIndex)
    {
        if (!CanStartExploration(state, shipId, destinationIndex))
            return false;

        D1ShipState ship = FindShipState(state, shipId);
        D1ScannedDestinationState destination = GetAvailableScannedDestinationByIndex(state, destinationIndex);

        if (ship == null || destination == null)
            return false;

        if (string.IsNullOrEmpty(destination.destinationId))
            return false;

        double duration = GetSimpleExplorationDurationSeconds(state, destination.destinationId, ship);

        ship.explorationActive = true;
        ship.activeDestinationId = destination.destinationId;
        ship.explorationTotalSeconds = duration;
        ship.explorationRemainingSeconds = duration;

        destination.available = false;

        return true;
    }

    private static void UpdateActiveScan(GameState state, double dt)
    {
        if (state == null)
            return;

        if (!state.dimension1ScanActive)
            return;

        if (dt <= 0.0)
            return;

        state.dimension1ScanRemainingSeconds -= dt;

        if (state.dimension1ScanRemainingSeconds > 0.0)
            return;

        CompleteSimpleScan(state);
    }

    private static void CompleteSimpleScan(GameState state)
    {
        if (state == null)
            return;

        state.dimension1ScanActive = false;
        state.dimension1ScanRemainingSeconds = 0.0;
        state.dimension1ScanTotalSeconds = 0.0;

        GenerateSimpleScannedDestinations(state);
    }

    private static void GenerateSimpleScannedDestinations(GameState state)
    {
        if (state == null)
            return;

        if (state.dimension1ScannedDestinations == null)
            state.dimension1ScannedDestinations = new List<D1ScannedDestinationState>();

        state.dimension1ScannedDestinations.Clear();

        int destinationCount = GetSimpleScanDestinationCount(state);
        List<string> destinations = PickSimpleDestinations(state, destinationCount);

        foreach (string destinationId in destinations)
        {
            state.dimension1ScannedDestinations.Add(new D1ScannedDestinationState
            {
                destinationId = destinationId,
                available = true
            });
        }
    }

    private static int GetSimpleScanDestinationCount(GameState state)
    {
        int scannerLevel = GetSimpleScannerLevel(state);

        return Mathf.Clamp(
            SimpleScanBaseDestinationCount + scannerLevel,
            SimpleScanBaseDestinationCount,
            SimpleScanMaxDestinationCount
        );
    }

    private static void UpdateActiveExplorations(GameState state, double dt)
    {
        if (state == null)
            return;

        if (dt <= 0.0)
            return;

        if (state.dimension1Ships == null)
            return;

        foreach (D1ShipState ship in state.dimension1Ships)
        {
            if (ship == null)
                continue;

            if (!ship.unlocked)
                continue;

            if (!ship.explorationActive)
                continue;

            ship.explorationRemainingSeconds -= dt;

            if (ship.explorationRemainingSeconds > 0.0)
                continue;

            CompleteSimpleExploration(state, ship);
        }
    }

    private static void CompleteSimpleExploration(GameState state, D1ShipState ship)
    {
        if (state == null || ship == null)
            return;

        string destinationId = ship.activeDestinationId;

        if (!string.IsNullOrEmpty(destinationId))
        {
            GrantSimpleExplorationRewards(state, destinationId, ship);
        }

        ship.explorationActive = false;
        ship.activeDestinationId = "";
        ship.explorationRemainingSeconds = 0.0;
        ship.explorationTotalSeconds = 0.0;

        if (SaveService.I != null)
            SaveService.I.Save();
    }

    private static void GrantSimpleExplorationRewards(GameState state, string destinationId, D1ShipState ship)
    {
        if (state == null)
            return;

        List<D1MetalAmount> rewards = new List<D1MetalAmount>();
        double materialMultiplier = GetShipMaterialRewardMultiplier(state, destinationId, ship);
        materialMultiplier *= GetShipArmorRewardPreservationMultiplier(state, destinationId, ship);

        state.dimension1LastExplorationBlueprintFragments = 0;
        state.dimension1LastExplorationSpecificBlueprints = new List<D1BlueprintAmount>();

        string[] metalRewardPool = GetExplorationMetalRewardPool(destinationId);

        for (int i = 0; i < metalRewardPool.Length; i++)
        {
            AddExplorationTimedReward(
                state,
                rewards,
                metalRewardPool[i],
                materialMultiplier
            );
        }

        int blueprintFragments = RollSimpleBlueprintFragments(state, destinationId, ship);

        if (blueprintFragments > 0)
        {
            state.dimension1BlueprintFragments += blueprintFragments;
            state.dimension1LastExplorationBlueprintFragments = blueprintFragments;
        }

        TryGrantSpecificBlueprintReward(state, destinationId, ship);

        state.dimension1LastExplorationDestinationId = destinationId;
        state.dimension1LastExplorationRewards = rewards;
        state.dimension1LastExplorationResultId += 1;

        AddRecentExplorationRecord(
            state,
            ship != null ? ship.shipId : "",
            destinationId,
            rewards,
            blueprintFragments,
            state.dimension1LastExplorationSpecificBlueprints
        );
    }

    private static void AddRecentExplorationRecord(
        GameState state,
        string shipId,
        string destinationId,
        List<D1MetalAmount> rewards,
        int blueprintFragments,
        List<D1BlueprintAmount> specificBlueprintRewards
    )
    {
        if (state == null)
            return;

        if (state.dimension1RecentExplorationRecords == null)
            state.dimension1RecentExplorationRecords = new List<D1ExplorationRecordEntry>();

        D1ExplorationRecordEntry entry = new D1ExplorationRecordEntry
        {
            resultId = state.dimension1LastExplorationResultId,
            shipId = shipId,
            destinationId = destinationId,
            rewards = new List<D1MetalAmount>(),
            blueprintFragments = blueprintFragments,
            specificBlueprintRewards = CloneBlueprintRewards(specificBlueprintRewards)
        };

        if (rewards != null)
        {
            foreach (D1MetalAmount reward in rewards)
            {
                if (reward == null)
                    continue;

                entry.rewards.Add(new D1MetalAmount
                {
                    metalId = reward.metalId,
                    amount = reward.amount
                });
            }
        }

        state.dimension1RecentExplorationRecords.Add(entry);

        while (state.dimension1RecentExplorationRecords.Count > MaxRecentExplorationRecords)
        {
            state.dimension1RecentExplorationRecords.RemoveAt(0);
        }
    }

    private static List<D1BlueprintAmount> CloneBlueprintRewards(List<D1BlueprintAmount> rewards)
    {
        List<D1BlueprintAmount> result = new List<D1BlueprintAmount>();

        if (rewards == null)
            return result;

        foreach (D1BlueprintAmount reward in rewards)
        {
            if (reward == null)
                continue;

            if (string.IsNullOrEmpty(reward.blueprintId))
                continue;

            if (reward.amount <= 0)
                continue;

            result.Add(new D1BlueprintAmount
            {
                blueprintId = reward.blueprintId,
                amount = reward.amount
            });
        }

        return result;
    }

    public static float GetSimpleBlueprintFragmentChance(string destinationId, D1ShipState ship)
    {
        return GetSimpleBlueprintFragmentChance(null, destinationId, ship);
    }

    public static float GetSimpleBlueprintFragmentChance(
        GameState state,
        string destinationId,
        D1ShipState ship
    )
    {
        float chance = GetBaseSimpleBlueprintFragmentChance(destinationId);

        chance += GetShipSensorBlueprintFragmentBonus(destinationId, ship);
        chance += GetRelicBlueprintFragmentBonus(state, destinationId, ship);

        return Mathf.Clamp(chance, 0.0f, 0.60f);
    }

    public static float GetSimpleBlueprintFragmentBaseChance(string destinationId)
    {
        return Mathf.Clamp(
            GetBaseSimpleBlueprintFragmentChance(destinationId),
            0.0f,
            0.60f
        );
    }

    private static int RollSimpleBlueprintFragments(
        GameState state,
        string destinationId,
        D1ShipState ship
    )
    {
        float chance = GetSimpleBlueprintFragmentChance(state, destinationId, ship);

        if (chance <= 0.0f)
            return 0;

        if (Random.value > chance)
            return 0;

        if ((destinationId == DestinationAbandonedProbe || destinationId == DestinationAbandonedShip) &&
            Random.value < 0.25f)
        {
            return 2;
        }

        return 1;
    }

    private static bool TryGrantSpecificBlueprintReward(
        GameState state,
        string destinationId,
        D1ShipState ship
    )
    {
        if (state == null)
            return false;

        float chance = GetSpecificBlueprintChance(state, destinationId, ship);

        if (chance <= 0.0f)
            return false;

        if (Random.value > chance)
            return false;

        string blueprintId = GetRandomSpecificBlueprintForDestination(state, destinationId);

        if (string.IsNullOrEmpty(blueprintId))
            return false;

        state.AddD1Blueprint(blueprintId, 1);

        if (state.dimension1LastExplorationSpecificBlueprints == null)
            state.dimension1LastExplorationSpecificBlueprints = new List<D1BlueprintAmount>();

        state.dimension1LastExplorationSpecificBlueprints.Add(new D1BlueprintAmount
        {
            blueprintId = blueprintId,
            amount = 1
        });

        return true;
    }

    public static float GetSpecificBlueprintChancePreview(
    GameState state,
    string destinationId,
    D1ShipState ship
)
    {
        return GetSpecificBlueprintChance(state, destinationId, ship);
    }

    private static float GetSpecificBlueprintChance(
        GameState state,
        string destinationId,
        D1ShipState ship
    )
    {
        string[] availablePool = GetSpecificBlueprintPoolForDestination(state, destinationId);

        if (availablePool == null || availablePool.Length == 0)
            return 0.0f;

        float chance = GetBaseSpecificBlueprintChance(destinationId);

        if (chance <= 0.0f)
            return 0.0f;

        chance += GetShipSensorSpecificBlueprintBonus(destinationId, ship);

        return Mathf.Clamp(chance, 0.0f, 0.20f);
    }

    private static float GetShipSensorSpecificBlueprintBonus(
        string destinationId,
        D1ShipState ship
    )
    {
        if (ship == null)
            return 0.0f;

        // Las matrices específicas son más valiosas que los fragmentos,
        // así que usamos solo parte del bonus de sensores.
        return GetShipSensorBlueprintFragmentBonus(destinationId, ship) * 0.35f;
    }

    private static float GetBaseSpecificBlueprintChance(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationShipGraveyard:
                return 0.06f;

            case DestinationAbandonedShip:
                return 0.08f;

            case DestinationAbandonedStation:
                return 0.07f;

            case DestinationLaboratory:
                return 0.05f;

            case DestinationAncientStructure:
                return 0.06f;

            case DestinationUnstableZone:
                return 0.08f;

            default:
                return 0.0f;
        }
    }

    private static string GetRandomSpecificBlueprintForDestination(GameState state, string destinationId)
    {
        string[] pool = GetSpecificBlueprintPoolForDestination(state, destinationId);

        if (pool == null || pool.Length == 0)
            return "";

        return pool[Random.Range(0, pool.Length)];
    }

    private static string GetRandomSpecificBlueprintForDestination(string destinationId)
    {
        return GetRandomSpecificBlueprintForDestination(null, destinationId);
    }

    public static string[] GetSpecificBlueprintPoolPreview(GameState state, string destinationId)
    {
        string[] pool = GetSpecificBlueprintPoolForDestination(state, destinationId);

        if (pool == null || pool.Length == 0)
            return new string[0];

        string[] result = new string[pool.Length];

        for (int i = 0; i < pool.Length; i++)
            result[i] = pool[i];

        return result;
    }

    public static string[] GetSpecificBlueprintPoolPreview(string destinationId)
    {
        return GetSpecificBlueprintPoolPreview(null, destinationId);
    }

    private static string[] GetSpecificBlueprintPoolForDestination(GameState state, string destinationId)
    {
        string[] basePool = GetBaseSpecificBlueprintPoolForDestination(destinationId);

        if (basePool == null || basePool.Length == 0)
            return new string[0];

        if (state == null)
            return basePool;

        List<string> filteredPool = new List<string>();

        for (int i = 0; i < basePool.Length; i++)
        {
            string blueprintId = basePool[i];

            if (string.IsNullOrEmpty(blueprintId))
                continue;

            if (!IsSpecificBlueprintAllowedForCurrentD1Progress(state, blueprintId))
                continue;

            filteredPool.Add(blueprintId);
        }

        return filteredPool.ToArray();
    }

    private static string[] GetBaseSpecificBlueprintPoolForDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationShipGraveyard:
                return new string[]
                {
                BlueprintCargoFrame,
                BlueprintCargoHold,
                BlueprintCargoStabilizer
                };

            case DestinationAbandonedShip:
                return new string[]
                {
                BlueprintCargoFrame,
                BlueprintCargoHold,
                BlueprintCargoStabilizer,
                BlueprintRescueFrame,
                BlueprintRescueBeacon
                };

            case DestinationAbandonedStation:
                return new string[]
                {
                BlueprintRescueFrame,
                BlueprintRescueBeacon,
                BlueprintRescueRecoveryBay,
                BlueprintRescueProtectionMatrix
                };

            case DestinationLaboratory:
                return new string[]
                {
                BlueprintRescueRecoveryBay,
                BlueprintRescueProtectionMatrix,
                BlueprintConvergenceMatrix
                };

            case DestinationAncientStructure:
                return new string[]
                {
                BlueprintConvergenceChassis,
                BlueprintConvergenceMatrix,
                BlueprintAnomalousArmor
                };

            case DestinationUnstableZone:
                return new string[]
                {
                BlueprintConvergenceChassis,
                BlueprintConvergenceCore,
                BlueprintConvergenceMatrix,
                BlueprintAnomalousArmor
                };

            default:
                return new string[0];
        }
    }

    private static bool IsSpecificBlueprintAllowedForCurrentD1Progress(GameState state, string blueprintId)
    {
        if (IsCargoShipSpecificBlueprint(blueprintId))
            return true;

        if (IsRescueShipSpecificBlueprint(blueprintId))
            return IsD1ShipUnlockedForRewardProgress(state, ShipCargoShip);

        if (IsConvergenceShipSpecificBlueprint(blueprintId))
            return IsD1ShipUnlockedForRewardProgress(state, ShipRescueShip);

        return true;
    }

    private static bool IsCargoShipSpecificBlueprint(string blueprintId)
    {
        return
            blueprintId == BlueprintCargoFrame ||
            blueprintId == BlueprintCargoHold ||
            blueprintId == BlueprintCargoStabilizer;
    }

    private static bool IsRescueShipSpecificBlueprint(string blueprintId)
    {
        return
            blueprintId == BlueprintRescueFrame ||
            blueprintId == BlueprintRescueBeacon ||
            blueprintId == BlueprintRescueRecoveryBay ||
            blueprintId == BlueprintRescueProtectionMatrix;
    }

    private static bool IsConvergenceShipSpecificBlueprint(string blueprintId)
    {
        return
            blueprintId == BlueprintConvergenceChassis ||
            blueprintId == BlueprintConvergenceCore ||
            blueprintId == BlueprintConvergenceMatrix ||
            blueprintId == BlueprintAnomalousArmor;
    }

    private static bool IsD1ShipUnlockedForRewardProgress(GameState state, string shipId)
    {
        D1ShipState ship = FindShipState(state, shipId);

        return ship != null && ship.unlocked;
    }

    private static float GetBaseSimpleBlueprintFragmentChance(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationMineralBelt:
                return 0.03f;

            case DestinationShipGraveyard:
                return 0.06f;

            case DestinationAbandonedProbe:
            case DestinationAbandonedShip:
                return 0.08f;

            case DestinationOrbitalRuin:
                return 0.07f;

            case DestinationDriftingProbes:
                return 0.08f;

            case DestinationLaboratory:
                return 0.08f;

            case DestinationAbandonedStation:
                return 0.10f;

            case DestinationMinorAnomaly:
                return 0.09f;

            case DestinationAncientStructure:
                return 0.08f;

            case DestinationUnstableZone:
                return 0.10f;

            case DestinationCrystalDebris:
                return 0.03f;

            case DestinationOrbitalArmorWreckage:
                return 0.05f;

            case DestinationCollapsedReactor:
                return 0.06f;

            default:
                return 0.03f;
        }
    }

    private static float GetShipSensorBlueprintFragmentBonus(string destinationId, D1ShipState ship)
    {
        if (ship == null)
            return 0.0f;

        int sensorsLevel = Mathf.Clamp(ship.sensorsLevel, 0, 6);

        switch (ship.shipId)
        {
            case ShipLightProbe:
                return GetLightProbeSensorBlueprintBonus(destinationId, sensorsLevel);

            case ShipExtractorDrone:
                return GetExtractorDroneSensorBlueprintBonus(destinationId, sensorsLevel);

            case ShipAnalyticProbe:
                return GetAnalyticProbeSensorBlueprintBonus(destinationId, sensorsLevel);

            case ShipCargoShip:
                return GetCargoShipSensorBlueprintBonus(destinationId, sensorsLevel);

            case ShipRescueShip:
                return GetRescueShipSensorBlueprintBonus(destinationId, sensorsLevel);

            case ShipConvergenceShip:
                return GetConvergenceShipSensorBlueprintBonus(destinationId, sensorsLevel);

            default:
                return 0.0f;
        }
    }

    private static float GetLightProbeSensorBlueprintBonus(string destinationId, int sensorsLevel)
    {
        if (!IsLightProbeSensorDestination(destinationId))
            return GetSensorCurveBonus(sensorsLevel, 0.0f, 0.005f, 0.008f, 0.012f, 0.016f, 0.020f, 0.025f);

        return GetSensorCurveBonus(sensorsLevel, 0.0f, 0.010f, 0.015f, 0.020f, 0.026f, 0.032f, 0.040f);
    }

    private static float GetExtractorDroneSensorBlueprintBonus(string destinationId, int sensorsLevel)
    {
        if (!IsExtractorDroneSensorDestination(destinationId))
            return GetSensorCurveBonus(sensorsLevel, 0.0f, 0.003f, 0.006f, 0.009f, 0.012f, 0.015f, 0.020f);

        return GetSensorCurveBonus(sensorsLevel, 0.0f, 0.006f, 0.010f, 0.015f, 0.020f, 0.026f, 0.032f);
    }

    private static float GetAnalyticProbeSensorBlueprintBonus(string destinationId, int sensorsLevel)
    {
        if (!IsAnalyticProbeSensorDestination(destinationId))
            return GetSensorCurveBonus(sensorsLevel, 0.020f, 0.030f, 0.040f, 0.050f, 0.060f, 0.075f, 0.090f);

        return GetSensorCurveBonus(sensorsLevel, 0.020f, 0.040f, 0.055f, 0.070f, 0.085f, 0.100f, 0.120f);
    }

    private static float GetCargoShipSensorBlueprintBonus(string destinationId, int sensorsLevel)
    {
        if (!IsCargoShipSensorDestination(destinationId))
            return GetSensorCurveBonus(sensorsLevel, 0.0f, 0.004f, 0.008f, 0.012f, 0.016f, 0.020f, 0.025f);

        return GetSensorCurveBonus(sensorsLevel, 0.0f, 0.008f, 0.014f, 0.020f, 0.026f, 0.033f, 0.040f);
    }

    private static float GetRescueShipSensorBlueprintBonus(string destinationId, int sensorsLevel)
    {
        if (!IsRescueShipSensorDestination(destinationId))
            return GetSensorCurveBonus(sensorsLevel, 0.0f, 0.005f, 0.010f, 0.015f, 0.020f, 0.026f, 0.032f);

        return GetSensorCurveBonus(sensorsLevel, 0.0f, 0.012f, 0.020f, 0.028f, 0.038f, 0.050f, 0.060f);
    }

    private static float GetConvergenceShipSensorBlueprintBonus(string destinationId, int sensorsLevel)
    {
        if (!IsConvergenceShipSensorDestination(destinationId))
            return GetSensorCurveBonus(sensorsLevel, 0.0f, 0.004f, 0.008f, 0.012f, 0.018f, 0.024f, 0.030f);

        return GetSensorCurveBonus(sensorsLevel, 0.0f, 0.015f, 0.026f, 0.038f, 0.052f, 0.066f, 0.080f);
    }

    private static float GetSensorCurveBonus(
        int sensorsLevel,
        float level0,
        float level1,
        float level2,
        float level3,
        float level4,
        float level5,
        float level6
    )
    {
        if (sensorsLevel >= 6)
            return level6;

        if (sensorsLevel >= 5)
            return level5;

        if (sensorsLevel >= 4)
            return level4;

        if (sensorsLevel >= 3)
            return level3;

        if (sensorsLevel >= 2)
            return level2;

        if (sensorsLevel >= 1)
            return level1;

        return level0;
    }

    private static bool IsLightProbeSensorDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationMineralBelt:
            case DestinationShipGraveyard:
            case DestinationAbandonedProbe:
            case DestinationAbandonedShip:
            case DestinationDriftingProbes:
                return true;

            default:
                return false;
        }
    }

    private static bool IsExtractorDroneSensorDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationMineralBelt:
            case DestinationShipGraveyard:
            case DestinationAbandonedStation:
                return true;

            default:
                return false;
        }
    }

    private static bool IsAnalyticProbeSensorDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationOrbitalRuin:
            case DestinationDriftingProbes:
            case DestinationLaboratory:
            case DestinationMinorAnomaly:
            case DestinationAncientStructure:
                return true;

            default:
                return false;
        }
    }

    private static bool IsCargoShipSensorDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationShipGraveyard:
            case DestinationAbandonedShip:
            case DestinationAbandonedStation:
            case DestinationUnstableZone:
                return true;

            default:
                return false;
        }
    }

    private static bool IsRescueShipSensorDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationAbandonedShip:
            case DestinationAbandonedStation:
            case DestinationMinorAnomaly:
            case DestinationAncientStructure:
            case DestinationUnstableZone:
                return true;

            default:
                return false;
        }
    }

    private static bool IsConvergenceShipSensorDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationMinorAnomaly:
            case DestinationAncientStructure:
            case DestinationUnstableZone:
                return true;

            default:
                return false;
        }
    }

    private static double GetShipMaterialRewardMultiplier(
        GameState state,
        string destinationId,
        D1ShipState ship
    )
    {
        double shipMultiplier = 1.0;

        if (ship != null)
        {
            int cargoLevel = Mathf.Clamp(ship.cargoLevel, 0, 6);

            switch (ship.shipId)
            {
                case ShipLightProbe:
                    shipMultiplier = GetLightProbeCargoRewardMultiplier(destinationId, cargoLevel);
                    break;

                case ShipExtractorDrone:
                    shipMultiplier = GetExtractorDroneCargoRewardMultiplier(destinationId, cargoLevel);
                    break;

                case ShipAnalyticProbe:
                    shipMultiplier = GetAnalyticProbeCargoRewardMultiplier(destinationId, cargoLevel);
                    break;

                case ShipCargoShip:
                    shipMultiplier = GetCargoShipCargoRewardMultiplier(destinationId, cargoLevel);
                    break;

                case ShipRescueShip:
                    shipMultiplier = GetRescueShipCargoRewardMultiplier(destinationId, cargoLevel);
                    break;

                case ShipConvergenceShip:
                    shipMultiplier = GetConvergenceShipCargoRewardMultiplier(destinationId, cargoLevel);
                    break;
            }
        }

        return
            shipMultiplier *
            GetRelicMaterialRewardMultiplier(state, destinationId, ship) *
            GetD1TreeMaterialRewardMultiplier(state, destinationId, ship);
    }

    private static double GetShipArmorRewardPreservationMultiplier(
        GameState state,
        string destinationId,
        D1ShipState ship
    )
    {
        double shipMultiplier = 1.0;

        if (ship != null)
        {
            int armorLevel = Mathf.Clamp(ship.armorLevel, 0, 6);

            if (armorLevel > 0)
            {
                switch (ship.shipId)
                {
                    case ShipLightProbe:
                        shipMultiplier = GetLightProbeArmorMultiplier(destinationId, armorLevel);
                        break;

                    case ShipExtractorDrone:
                        shipMultiplier = GetExtractorDroneArmorMultiplier(destinationId, armorLevel);
                        break;

                    case ShipAnalyticProbe:
                        shipMultiplier = GetAnalyticProbeArmorMultiplier(destinationId, armorLevel);
                        break;

                    case ShipCargoShip:
                        shipMultiplier = GetCargoShipArmorMultiplier(destinationId, armorLevel);
                        break;

                    case ShipRescueShip:
                        shipMultiplier = GetRescueShipArmorMultiplier(destinationId, armorLevel);
                        break;

                    case ShipConvergenceShip:
                        shipMultiplier = GetConvergenceShipArmorMultiplier(destinationId, armorLevel);
                        break;
                }
            }
        }

        return shipMultiplier * GetRelicArmorPreservationMultiplier(state, destinationId, ship);
    }

    private static double GetLightProbeArmorMultiplier(string destinationId, int armorLevel)
    {
        if (!IsBasicArmorDestination(destinationId))
            return 1.0;

        return GetArmorCurveMultiplier(armorLevel, 1.0, 1.015, 1.030, 1.045, 1.065, 1.085, 1.10);
    }

    private static double GetExtractorDroneArmorMultiplier(string destinationId, int armorLevel)
    {
        if (!IsMaterialArmorDestination(destinationId))
            return 1.0;

        return GetArmorCurveMultiplier(armorLevel, 1.0, 1.020, 1.040, 1.065, 1.090, 1.120, 1.15);
    }

    private static double GetAnalyticProbeArmorMultiplier(string destinationId, int armorLevel)
    {
        if (!IsResearchArmorDestination(destinationId))
            return 1.0;

        return GetArmorCurveMultiplier(armorLevel, 1.0, 1.015, 1.030, 1.045, 1.065, 1.085, 1.10);
    }

    private static double GetCargoShipArmorMultiplier(string destinationId, int armorLevel)
    {
        if (!IsHeavyCargoArmorDestination(destinationId))
            return 1.0;

        return GetArmorCurveMultiplier(armorLevel, 1.0, 1.025, 1.050, 1.080, 1.110, 1.150, 1.20);
    }

    private static double GetRescueShipArmorMultiplier(string destinationId, int armorLevel)
    {
        if (!IsRecoveryArmorDestination(destinationId))
            return 1.0;

        return GetArmorCurveMultiplier(armorLevel, 1.0, 1.035, 1.070, 1.110, 1.160, 1.220, 1.30);
    }

    private static double GetConvergenceShipArmorMultiplier(string destinationId, int armorLevel)
    {
        if (!IsConvergenceArmorDestination(destinationId))
            return 1.0;

        return GetArmorCurveMultiplier(armorLevel, 1.0, 1.030, 1.060, 1.095, 1.140, 1.190, 1.25);
    }

    private static double GetArmorCurveMultiplier(
        int armorLevel,
        double level0,
        double level1,
        double level2,
        double level3,
        double level4,
        double level5,
        double level6
    )
    {
        if (armorLevel >= 6)
            return level6;

        if (armorLevel >= 5)
            return level5;

        if (armorLevel >= 4)
            return level4;

        if (armorLevel >= 3)
            return level3;

        if (armorLevel >= 2)
            return level2;

        if (armorLevel >= 1)
            return level1;

        return level0;
    }

    private static bool IsBasicArmorDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationAbandonedProbe:
            case DestinationAbandonedShip:
            case DestinationShipGraveyard:
                return true;

            default:
                return false;
        }
    }

    private static bool IsMaterialArmorDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationMineralBelt:
            case DestinationShipGraveyard:
            case DestinationAbandonedStation:
            case DestinationUnstableZone:
                return true;

            default:
                return false;
        }
    }

    private static bool IsResearchArmorDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationOrbitalRuin:
            case DestinationLaboratory:
            case DestinationMinorAnomaly:
            case DestinationAncientStructure:
                return true;

            default:
                return false;
        }
    }

    private static bool IsHeavyCargoArmorDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationShipGraveyard:
            case DestinationAbandonedShip:
            case DestinationAbandonedStation:
            case DestinationUnstableZone:
                return true;

            default:
                return false;
        }
    }

    private static bool IsRecoveryArmorDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationAbandonedProbe:
            case DestinationAbandonedShip:
            case DestinationAbandonedStation:
            case DestinationMinorAnomaly:
            case DestinationAncientStructure:
            case DestinationUnstableZone:
                return true;

            default:
                return false;
        }
    }

    private static bool IsConvergenceArmorDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationMinorAnomaly:
            case DestinationAncientStructure:
            case DestinationUnstableZone:
                return true;

            default:
                return false;
        }
    }

    private static double GetLightProbeCargoRewardMultiplier(string destinationId, int cargoLevel)
    {
        double bonusPerLevel = IsBasicExplorationDestination(destinationId) ? 0.05 : 0.025;
        return 1.0 + cargoLevel * bonusPerLevel;
    }

    private static double GetExtractorDroneCargoRewardMultiplier(string destinationId, int cargoLevel)
    {
        if (IsPrimaryMaterialDestination(destinationId))
            return GetCargoCurveMultiplier(cargoLevel, 1.20, 1.30, 1.40, 1.50, 1.65, 1.80, 2.00);

        if (IsSecondaryMaterialDestination(destinationId))
            return GetCargoCurveMultiplier(cargoLevel, 1.05, 1.12, 1.20, 1.28, 1.37, 1.47, 1.60);

        return GetCargoCurveMultiplier(cargoLevel, 1.0, 1.05, 1.09, 1.14, 1.20, 1.26, 1.33);
    }

    private static double GetAnalyticProbeCargoRewardMultiplier(string destinationId, int cargoLevel)
    {
        double bonusPerLevel = IsResearchDestination(destinationId) ? 0.03 : 0.015;
        return 1.0 + cargoLevel * bonusPerLevel;
    }

    private static double GetCargoShipCargoRewardMultiplier(string destinationId, int cargoLevel)
    {
        if (IsHeavyCargoDestination(destinationId))
            return GetCargoCurveMultiplier(cargoLevel, 1.10, 1.18, 1.26, 1.35, 1.47, 1.60, 1.75);

        if (IsSecondaryMaterialDestination(destinationId))
            return GetCargoCurveMultiplier(cargoLevel, 1.05, 1.12, 1.19, 1.27, 1.36, 1.46, 1.58);

        return GetCargoCurveMultiplier(cargoLevel, 1.0, 1.06, 1.12, 1.18, 1.25, 1.32, 1.40);
    }

    private static double GetRescueShipCargoRewardMultiplier(string destinationId, int cargoLevel)
    {
        if (IsRecoveryDestination(destinationId))
            return GetCargoCurveMultiplier(cargoLevel, 1.0, 1.08, 1.16, 1.25, 1.35, 1.46, 1.58);

        return GetCargoCurveMultiplier(cargoLevel, 1.0, 1.04, 1.08, 1.13, 1.18, 1.24, 1.30);
    }

    private static double GetConvergenceShipCargoRewardMultiplier(string destinationId, int cargoLevel)
    {
        if (IsConvergenceDestination(destinationId))
            return GetCargoCurveMultiplier(cargoLevel, 1.0, 1.08, 1.16, 1.25, 1.35, 1.46, 1.58);

        return GetCargoCurveMultiplier(cargoLevel, 1.0, 1.03, 1.06, 1.10, 1.15, 1.20, 1.25);
    }

    private static double GetCargoCurveMultiplier(
        int cargoLevel,
        double level0,
        double level1,
        double level2,
        double level3,
        double level4,
        double level5,
        double level6
    )
    {
        if (cargoLevel >= 6)
            return level6;

        if (cargoLevel >= 5)
            return level5;

        if (cargoLevel >= 4)
            return level4;

        if (cargoLevel >= 3)
            return level3;

        if (cargoLevel >= 2)
            return level2;

        if (cargoLevel >= 1)
            return level1;

        return level0;
    }

    private static bool IsBasicExplorationDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationMineralBelt:
            case DestinationShipGraveyard:
            case DestinationAbandonedShip:
            case DestinationAbandonedProbe:
            case DestinationDriftingProbes:
                return true;

            default:
                return false;
        }
    }

    private static bool IsPrimaryMaterialDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationMineralBelt:
            case DestinationShipGraveyard:
                return true;

            default:
                return false;
        }
    }

    private static bool IsSecondaryMaterialDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationAbandonedShip:
            case DestinationAbandonedProbe:
            case DestinationAbandonedStation:
            case DestinationUnstableZone:
                return true;

            default:
                return false;
        }
    }

    private static bool IsResearchDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationOrbitalRuin:
            case DestinationDriftingProbes:
            case DestinationLaboratory:
            case DestinationMinorAnomaly:
            case DestinationAncientStructure:
                return true;

            default:
                return false;
        }
    }

    private static bool IsHeavyCargoDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationMineralBelt:
            case DestinationShipGraveyard:
            case DestinationAbandonedStation:
            case DestinationUnstableZone:
                return true;

            default:
                return false;
        }
    }

    private static bool IsRecoveryDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationAbandonedShip:
            case DestinationAbandonedProbe:
            case DestinationAbandonedStation:
            case DestinationMinorAnomaly:
            case DestinationAncientStructure:
            case DestinationUnstableZone:
                return true;

            default:
                return false;
        }
    }

    private static bool IsConvergenceDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationMinorAnomaly:
            case DestinationAncientStructure:
            case DestinationUnstableZone:
                return true;

            default:
                return false;
        }
    }

    public static string[] GetExplorationMetalRewardPoolPreview(GameState state, string destinationId)
    {
        string[] pool = GetExplorationMetalRewardPool(destinationId);

        if (pool == null || pool.Length == 0)
            return new string[0];

        List<string> result = new List<string>();

        for (int i = 0; i < pool.Length; i++)
        {
            string metalId = pool[i];

            if (string.IsNullOrEmpty(metalId))
                continue;

            if (!IsMetalUnlockedForDimension1(state, metalId))
                continue;

            if (GetBaseMetalProductionPerSecond(state, metalId) <= 0.0)
                continue;

            result.Add(metalId);
        }

        return result.ToArray();
    }

    private static string[] GetExplorationMetalRewardPool(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationMineralBelt:
                return new string[]
                {
                MetalIron,
                MetalCopper,
                MetalAluminum
                };

            case DestinationShipGraveyard:
                return new string[]
                {
                MetalIron,
                MetalTitanium,
                MetalNickel
                };

            case DestinationAbandonedProbe:
            case DestinationAbandonedShip:
                return new string[]
                {
                MetalCopper,
                MetalTitanium,
                MetalNickel
                };

            case DestinationOrbitalRuin:
                return new string[]
                {
                MetalCopper,
                MetalAluminum,
                MetalLithium
                };

            case DestinationDriftingProbes:
                return new string[]
                {
                MetalCopper,
                MetalAluminum,
                MetalLithium
                };

            case DestinationLaboratory:
                return new string[]
                {
                MetalAluminum,
                MetalTitanium,
                MetalLithium,
                MetalCobalt
                };

            case DestinationAbandonedStation:
                return new string[]
                {
                MetalTitanium,
                MetalNickel,
                MetalCobalt,
                MetalPlatinum
                };

            case DestinationMinorAnomaly:
                return new string[]
                {
                MetalLithium,
                MetalTungsten,
                MetalCobalt,
                MetalPlatinum
                };

            case DestinationAncientStructure:
                return new string[]
                {
                MetalTungsten,
                MetalPlatinum,
                MetalIridium
                };

            case DestinationUnstableZone:
                return new string[]
                {
                MetalTungsten,
                MetalPlatinum,
                MetalIridium
                };

            default:
                return new string[0];
        }
    }

    private static void AddExplorationTimedReward(
    GameState state,
    List<D1MetalAmount> rewards,
    string metalId,
    double materialMultiplier
)
    {
        if (state == null || rewards == null)
            return;

        if (!IsMetalUnlockedForDimension1(state, metalId))
            return;

        double productionPerSecond = GetBaseMetalProductionPerSecond(state, metalId);

        if (productionPerSecond <= 0.0)
            return;

        double secondsReward = Random.Range(120f, 180f);
        double amount = productionPerSecond * secondsReward * materialMultiplier;

        AddExplorationReward(state, rewards, metalId, amount);
    }

    private static void AddExplorationReward(
        GameState state,
        List<D1MetalAmount> rewards,
        string metalId,
        double amount
    )
    {
        if (state == null || rewards == null)
            return;

        if (amount <= 0.0)
            return;

        if (!IsMetalUnlockedForDimension1(state, metalId))
            return;

        state.AddD1Metal(metalId, amount);

        rewards.Add(new D1MetalAmount
        {
            metalId = metalId,
            amount = amount
        });
    }

    private static double GetSimpleExplorationDurationSeconds(string destinationId, D1ShipState ship)
    {
        return GetSimpleExplorationDurationSeconds(null, destinationId, ship);
    }

    private static double GetSimpleExplorationDurationSeconds(
        GameState state,
        string destinationId,
        D1ShipState ship
    )
    {
        double baseDuration = GetSimpleExplorationBaseDurationSeconds(destinationId);

        if (ship == null)
            return baseDuration * GetRelicExplorationDurationMultiplier(state, destinationId);

        double shipDuration = baseDuration;

        if (ship.shipId == ShipLightProbe)
            shipDuration = baseDuration * GetSpeedMultiplierByLevel(ship.speedLevel);
        else if (ship.shipId == ShipExtractorDrone && IsExtractorDroneSpeedCompatibleDestination(destinationId))
            shipDuration = baseDuration * GetSpeedMultiplierByLevel(ship.speedLevel);
        else if (ship.shipId == ShipAnalyticProbe && IsAnalyticProbeSpeedCompatibleDestination(destinationId))
            shipDuration = baseDuration * GetSpeedMultiplierByLevel(ship.speedLevel);
        else if (ship.shipId == ShipCargoShip && IsCargoShipSpeedCompatibleDestination(destinationId))
            shipDuration = baseDuration * GetSpeedMultiplierByLevel(ship.speedLevel);
        else if (ship.shipId == ShipRescueShip && IsRescueSpeedCompatibleDestination(destinationId))
            shipDuration = baseDuration * GetSpeedMultiplierByLevel(ship.speedLevel);
        else if (ship.shipId == ShipConvergenceShip && IsConvergenceSpeedCompatibleDestination(destinationId))
            shipDuration = baseDuration * GetSpeedMultiplierByLevel(ship.speedLevel);

        return
            shipDuration *
            GetRelicExplorationDurationMultiplier(state, destinationId) *
            GetD1TreeExplorationDurationMultiplier(state, destinationId, ship);
    }

    private static double GetSimpleExplorationBaseDurationSeconds(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationMineralBelt:
                return 3.0;

            case DestinationShipGraveyard:
                return 3.0;

            case DestinationAbandonedShip:
            case DestinationAbandonedProbe:
                return 4.0;

            case DestinationOrbitalRuin:
                return 4.0;

            case DestinationDriftingProbes:
                return 4.0;

            case DestinationLaboratory:
                return 5.0;

            case DestinationAbandonedStation:
                return 5.0;

            case DestinationMinorAnomaly:
                return 6.0;

            case DestinationAncientStructure:
                return 7.0;

            case DestinationUnstableZone:
                return 8.0;

            case DestinationCrystalDebris:
                return 3.0;

            case DestinationOrbitalArmorWreckage:
                return 5.0;

            case DestinationCollapsedReactor:
                return 6.0;

            default:
                return 3.0;
        }
    }

    private static double GetSpeedMultiplierByLevel(int speedLevel)
    {
        if (speedLevel >= 6)
            return 0.58;

        if (speedLevel >= 5)
            return 0.64;

        if (speedLevel >= 4)
            return 0.70;

        if (speedLevel >= 3)
            return 0.76;

        if (speedLevel >= 2)
            return 0.82;

        if (speedLevel >= 1)
            return 0.90;

        return 1.0;
    }

    private static bool IsExtractorDroneSpeedCompatibleDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationMineralBelt:
            case DestinationShipGraveyard:
                return true;

            default:
                return false;
        }
    }

    private static bool IsAnalyticProbeSpeedCompatibleDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationOrbitalRuin:
            case DestinationDriftingProbes:
            case DestinationLaboratory:
            case DestinationMinorAnomaly:
            case DestinationAncientStructure:
                return true;

            default:
                return false;
        }
    }

    private static bool IsCargoShipSpeedCompatibleDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationMineralBelt:
            case DestinationShipGraveyard:
            case DestinationAbandonedStation:
                return true;

            default:
                return false;
        }
    }

    private static bool IsRescueSpeedCompatibleDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationAbandonedShip:
            case DestinationAbandonedStation:
            case DestinationMinorAnomaly:
            case DestinationAncientStructure:
            case DestinationUnstableZone:
                return true;

            default:
                return false;
        }
    }

    private static bool IsConvergenceSpeedCompatibleDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationMinorAnomaly:
            case DestinationAncientStructure:
            case DestinationUnstableZone:
                return true;

            default:
                return false;
        }
    }

    private static double GetSimpleScanDurationSeconds(GameState state)
    {
        D1ShipState analyticProbe = FindShipState(state, ShipAnalyticProbe);

        if (analyticProbe != null && analyticProbe.unlocked)
        {
            if (analyticProbe.sensorsLevel >= 6)
                return 2.1;

            if (analyticProbe.sensorsLevel >= 5)
                return 2.4;

            if (analyticProbe.sensorsLevel >= 4)
                return 2.7;

            if (analyticProbe.sensorsLevel >= 3)
                return 3.0;

            if (analyticProbe.sensorsLevel >= 2)
                return 3.5;

            if (analyticProbe.sensorsLevel >= 1)
                return 4.0;
        }

        return SimpleScanDurationSeconds;
    }

    private static D1PlanetState FindPlanetState(GameState state, string planetId)
    {
        if (state == null || state.dimension1Planets == null)
            return null;

        foreach (D1PlanetState planet in state.dimension1Planets)
        {
            if (planet != null && planet.planetId == planetId)
                return planet;
        }

        return null;
    }

    private static D1ShipState FindShipState(GameState state, string shipId)
    {
        if (state == null || state.dimension1Ships == null)
            return null;

        foreach (D1ShipState ship in state.dimension1Ships)
        {
            if (ship != null && ship.shipId == shipId)
                return ship;
        }

        return null;
    }

    private static D1ScannedDestinationState GetFirstAvailableScannedDestination(GameState state)
    {
        if (state == null || state.dimension1ScannedDestinations == null)
            return null;

        foreach (D1ScannedDestinationState destination in state.dimension1ScannedDestinations)
        {
            if (destination != null && destination.available)
                return destination;
        }

        return null;
    }

    private static D1ScannedDestinationState GetAvailableScannedDestinationByIndex(GameState state, int destinationIndex)
    {
        if (state == null || state.dimension1ScannedDestinations == null)
            return null;

        if (destinationIndex < 0)
            return null;

        int currentIndex = 0;

        foreach (D1ScannedDestinationState destination in state.dimension1ScannedDestinations)
        {
            if (destination == null || !destination.available)
                continue;

            if (currentIndex == destinationIndex)
                return destination;

            currentIndex++;
        }

        return null;
    }

    private static bool IsLightProbeExploring(GameState state)
    {
        D1ShipState ship = FindShipState(state, ShipLightProbe);
        return ship != null && ship.explorationActive;
    }

    private static bool IsAnyShipExploring(GameState state)
    {
        if (state == null || state.dimension1Ships == null)
            return false;

        foreach (D1ShipState ship in state.dimension1Ships)
        {
            if (ship != null && ship.unlocked && ship.explorationActive)
                return true;
        }

        return false;
    }

    private static List<string> PickSimpleDestinations(GameState state, int count)
    {
        List<string> pool = new List<string>
        {
            DestinationMineralBelt,
            DestinationShipGraveyard,
            DestinationAbandonedShip,
            DestinationOrbitalRuin,
            DestinationDriftingProbes,
            DestinationLaboratory,
            DestinationAbandonedStation,
            DestinationMinorAnomaly,
            DestinationAncientStructure,
            DestinationUnstableZone
        };

        List<string> validPool = new List<string>();

        foreach (string destinationId in pool)
        {
            if (CanDestinationAppearInScan(state, destinationId))
            {
                validPool.Add(destinationId);
            }
        }

        List<string> selected = new List<string>();

        while (selected.Count < count && validPool.Count > 0)
        {
            int index = Random.Range(0, validPool.Count);
            selected.Add(validPool[index]);
            validPool.RemoveAt(index);
        }

        return selected;
    }

    private static bool CanDestinationAppearInScan(GameState state, string destinationId)
    {
        if (state == null)
            return false;

        return
            HasDestinationAccessMetalUnlocked(state, destinationId) &&
            DestinationHasUnlockedMetalReward(state, destinationId);
    }

    private static bool HasDestinationAccessMetalUnlocked(GameState state, string destinationId)
    {
        if (state == null)
            return false;

        switch (destinationId)
        {
            case DestinationMineralBelt:
                return IsMetalUnlockedForDimension1(state, MetalIron);

            case DestinationShipGraveyard:
                return IsMetalUnlockedForDimension1(state, MetalIron);

            case DestinationAbandonedProbe:
            case DestinationAbandonedShip:
                return IsMetalUnlockedForDimension1(state, MetalCopper);

            case DestinationOrbitalRuin:
                return IsMetalUnlockedForDimension1(state, MetalAluminum);

            case DestinationDriftingProbes:
                return IsMetalUnlockedForDimension1(state, MetalAluminum);

            case DestinationLaboratory:
                return IsMetalUnlockedForDimension1(state, MetalTitanium);

            case DestinationAbandonedStation:
                return IsMetalUnlockedForDimension1(state, MetalCobalt);

            case DestinationMinorAnomaly:
                return IsMetalUnlockedForDimension1(state, MetalTungsten);

            case DestinationAncientStructure:
                return IsMetalUnlockedForDimension1(state, MetalPlatinum);

            case DestinationUnstableZone:
                return IsMetalUnlockedForDimension1(state, MetalIridium);

            default:
                return false;
        }
    }

    private static bool DestinationHasUnlockedMetalReward(GameState state, string destinationId)
    {
        string[] availableRewards = GetExplorationMetalRewardPoolPreview(state, destinationId);

        return availableRewards != null && availableRewards.Length > 0;
    }
}

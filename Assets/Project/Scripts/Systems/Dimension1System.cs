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
public class D1SectorState
{
    public string sectorId;
    public bool unlocked;
    public int completedExplorations;
}

[System.Serializable]
public class D1SectorRequirementStatus
{
    public string requirementId;
    public string label;
    public bool met;
    public bool evaluationAvailable;
    public bool showProgress;
    public int currentValue;
    public int requiredValue;
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
    public string activeSpecialPointId;
    public string activeSectorId;
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
    public string specialPointId;
    public string sectorId;
}

[System.Serializable]
public class D1ExplorationRecordEntry
{
    public int resultId;
    public string shipId;
    public string destinationId;
    public string sectorId;
    public List<D1MetalAmount> rewards = new List<D1MetalAmount>();
    public int blueprintFragments;
    public List<D1BlueprintAmount> specificBlueprintRewards = new List<D1BlueprintAmount>();
    public List<D1RelicRewardEntry> relicRewards = new List<D1RelicRewardEntry>();
}

[System.Serializable]
public class D1BlueprintAmount
{
    public string blueprintId;
    public int amount;
}

[System.Serializable]
public class D1RelicRewardEntry
{
    public string relicId;
    public bool wasDuplicate;
    public string duplicateMetalId;
    public double duplicateMetalAmount;
}

[System.Serializable]
public class D1RelicState
{
    public string relicId;
    public bool unlocked;
    public int level;
}

[System.Serializable]
public class D1RelicPityState
{
    public string sectorId;
    public int relicTier;
    public int points;
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

    // Sectores de Dimensión 1 - Parte 1
    public const string Sector01OuterRim = "d1_sector_01_outer_rim";
    public const string Sector02DebrisRing = "d1_sector_02_debris_ring";
    public const string Sector03AncientOrbits = "d1_sector_03_ancient_orbits";
    public const string Sector04SilentFrontier = "d1_sector_04_silent_frontier";
    public const string Sector05GalacticCenter = "d1_sector_05_galactic_center";

    public static readonly string[] Dimension1SectorIds =
    {
        Sector01OuterRim,
        Sector02DebrisRing,
        Sector03AncientOrbits,
        Sector04SilentFrontier,
        Sector05GalacticCenter
    };

    public static bool IsDimension1SectorId(string sectorId)
    {
        if (string.IsNullOrEmpty(sectorId))
            return false;

        foreach (string currentId in Dimension1SectorIds)
        {
            if (currentId == sectorId)
                return true;
        }

        return false;
    }

    public static bool IsDimension1ExplorationSectorId(string sectorId)
    {
        return
            sectorId == Sector01OuterRim ||
            sectorId == Sector02DebrisRing ||
            sectorId == Sector03AncientOrbits ||
            sectorId == Sector04SilentFrontier;
    }

    public static string GetDimension1SectorVisualName(string sectorId)
    {
        switch (sectorId)
        {
            case Sector01OuterRim:
                return "Sector 1 - Borde Exterior";
            case Sector02DebrisRing:
                return "Sector 2 - Anillo de Restos";
            case Sector03AncientOrbits:
                return "Sector 3 - Órbitas Antiguas";
            case Sector04SilentFrontier:
                return "Sector 4 - Frontera Silenciosa";
            case Sector05GalacticCenter:
                return "Centro Galáctico";
            default:
                return sectorId ?? "";
        }
    }

    public static List<D1SectorRequirementStatus> GetD1SectorUnlockRequirements(
        GameState state,
        string targetSectorId
    )
    {
        var result = new List<D1SectorRequirementStatus>();

        if (state == null)
            return result;

        switch (targetSectorId)
        {
            case Sector02DebrisRing:
                AddD1Requirement(
                    result,
                    "s1_explorations",
                    "50 exploraciones completadas en Sector 1",
                    GetD1SectorExplorationCountDirect(state, Sector01OuterRim),
                    50
                );
                AddD1BooleanRequirement(
                    result,
                    "planet_02",
                    "Planeta 2 desbloqueado",
                    IsD1PlanetUnlockedDirect(state, Planet02)
                );
                AddD1BooleanRequirement(
                    result,
                    "extractor_drone",
                    "Dron Extractor desbloqueado",
                    IsD1ShipUnlockedDirect(state, ShipExtractorDrone)
                );
                AddD1Requirement(
                    result,
                    "scanner_02",
                    "Escáner nivel 2",
                    GetSimpleScannerLevel(state),
                    2
                );
                break;

            case Sector03AncientOrbits:
                AddD1Requirement(
                    result,
                    "s2_explorations",
                    "100 exploraciones completadas en Sector 2",
                    GetD1SectorExplorationCountDirect(state, Sector02DebrisRing),
                    100
                );
                AddD1Requirement(
                    result,
                    "scanner_05",
                    "Escáner nivel 5",
                    GetSimpleScannerLevel(state),
                    5
                );
                AddD1Requirement(
                    result,
                    "relic_reading_1",
                    "Lectura de Reliquias Tier 1 comprada",
                    GetD1TreeTierDirect(state, D1TreeRelicReading),
                    1
                );
                AddD1Requirement(
                    result,
                    "tier_1_relic",
                    "Al menos 1 Reliquia Nivel 1 descubierta",
                    GetD1UnlockedRelicCountDirect(state, "", 1),
                    1
                );
                AddD1BooleanRequirement(
                    result,
                    "cargo_matrix",
                    "1 Matriz de Nave de Carga o 1 Fragmento Adaptativo",
                    HasD1CargoMatrixOrAdaptiveFragmentDirect(state)
                );
                break;

            case Sector04SilentFrontier:
                AddD1Requirement(
                    result,
                    "s3_explorations",
                    "200 exploraciones completadas en Sector 3",
                    GetD1SectorExplorationCountDirect(state, Sector03AncientOrbits),
                    200
                );
                AddD1BooleanRequirement(
                    result,
                    "cargo_ship",
                    "Nave de Carga desbloqueada",
                    IsD1ShipUnlockedDirect(state, ShipCargoShip)
                );
                AddD1Requirement(
                    result,
                    "scanner_10",
                    "Escáner nivel 10",
                    GetSimpleScannerLevel(state),
                    10
                );
                AddD1Requirement(
                    result,
                    "relic_reading_2",
                    "Lectura de Reliquias Tier 2 comprada",
                    GetD1TreeTierDirect(state, D1TreeRelicReading),
                    2
                );
                AddD1Requirement(
                    result,
                    "four_relics",
                    "4 Reliquias descubiertas",
                    GetD1UnlockedRelicCountDirect(state, "", 0),
                    4
                );
                AddD1Requirement(
                    result,
                    "two_tier_1_level_25",
                    "2 Reliquias Nivel 1 en nivel 25+",
                    GetD1RelicsAtOrAboveLevelDirect(state, "", 1, 25),
                    2
                );
                AddD1Requirement(
                    result,
                    "ship_upgrades_05",
                    "5 mejoras de nave compradas",
                    GetD1PurchasedShipUpgradeCountDirect(state),
                    5
                );
                break;

            case Sector05GalacticCenter:
                AddD1Requirement(
                    result,
                    "s4_explorations",
                    "300 exploraciones completadas en Sector 4",
                    GetD1SectorExplorationCountDirect(state, Sector04SilentFrontier),
                    300
                );
                AddD1Requirement(
                    result,
                    "scanner_15",
                    "Escáner nivel 15",
                    GetSimpleScannerLevel(state),
                    15
                );
                AddD1Requirement(
                    result,
                    "relic_reading_3",
                    "Lectura de Reliquias Tier 3 comprada",
                    GetD1TreeTierDirect(state, D1TreeRelicReading),
                    3
                );
                AddD1BooleanRequirement(
                    result,
                    "fleet_coordination",
                    "Coordinación de Flota comprada",
                    GetD1TreeTierDirect(state, D1TreeFleetCoordination) > 0
                );
                AddD1Requirement(
                    result,
                    "eight_relics",
                    "8 Reliquias descubiertas",
                    GetD1UnlockedRelicCountDirect(state, "", 0),
                    8
                );
                AddD1Requirement(
                    result,
                    "two_tier_2_relics",
                    "2 Reliquias Nivel 2 descubiertas",
                    GetD1UnlockedRelicCountDirect(state, "", 2),
                    2
                );
                AddD1Requirement(
                    result,
                    "three_relics_level_25",
                    "3 Reliquias en nivel 25+",
                    GetD1RelicsAtOrAboveLevelDirect(state, "", 0, 25),
                    3
                );
                AddD1Requirement(
                    result,
                    "ship_upgrades_07",
                    "7 mejoras de nave compradas",
                    GetD1PurchasedShipUpgradeCountDirect(state),
                    7
                );
                AddD1Requirement(
                    result,
                    "tree_nodes_05",
                    "5 nodos D1 comprados",
                    GetD1PurchasedTreeNodeCountDirect(state),
                    5
                );
                break;
        }

        return result;
    }

    public static bool AreD1SectorUnlockRequirementsMet(
        GameState state,
        string targetSectorId
    )
    {
        List<D1SectorRequirementStatus> requirements =
            GetD1SectorUnlockRequirements(state, targetSectorId);

        if (requirements.Count == 0)
            return targetSectorId == Sector01OuterRim;

        foreach (D1SectorRequirementStatus requirement in requirements)
        {
            if (requirement == null ||
                !requirement.evaluationAvailable ||
                !requirement.met)
            {
                return false;
            }
        }

        return IsPreviousD1SectorUnlockedDirect(state, targetSectorId);
    }

    private static void AddD1Requirement(
        List<D1SectorRequirementStatus> result,
        string requirementId,
        string label,
        int currentValue,
        int requiredValue
    )
    {
        currentValue = Mathf.Max(0, currentValue);
        requiredValue = Mathf.Max(0, requiredValue);

        result.Add(new D1SectorRequirementStatus
        {
            requirementId = requirementId,
            label = label,
            met = currentValue >= requiredValue,
            evaluationAvailable = true,
            showProgress = true,
            currentValue = currentValue,
            requiredValue = requiredValue
        });
    }

    private static void AddD1BooleanRequirement(
        List<D1SectorRequirementStatus> result,
        string requirementId,
        string label,
        bool met
    )
    {
        result.Add(new D1SectorRequirementStatus
        {
            requirementId = requirementId,
            label = label,
            met = met,
            evaluationAvailable = true,
            showProgress = false,
            currentValue = met ? 1 : 0,
            requiredValue = 1
        });
    }

    private static void AddD1PendingRequirement(
        List<D1SectorRequirementStatus> result,
        string requirementId,
        string label
    )
    {
        result.Add(new D1SectorRequirementStatus
        {
            requirementId = requirementId,
            label = label,
            met = false,
            evaluationAvailable = false,
            showProgress = false,
            currentValue = 0,
            requiredValue = 1
        });
    }

    private static int GetD1SectorExplorationCountDirect(
        GameState state,
        string sectorId
    )
    {
        if (state == null || state.dimension1Sectors == null)
            return 0;

        foreach (D1SectorState sector in state.dimension1Sectors)
        {
            if (sector != null && sector.sectorId == sectorId)
                return Mathf.Max(0, sector.completedExplorations);
        }

        return 0;
    }

    private static bool IsPreviousD1SectorUnlockedDirect(
        GameState state,
        string targetSectorId
    )
    {
        string previousSectorId = "";

        switch (targetSectorId)
        {
            case Sector02DebrisRing:
                previousSectorId = Sector01OuterRim;
                break;
            case Sector03AncientOrbits:
                previousSectorId = Sector02DebrisRing;
                break;
            case Sector04SilentFrontier:
                previousSectorId = Sector03AncientOrbits;
                break;
            case Sector05GalacticCenter:
                previousSectorId = Sector04SilentFrontier;
                break;
            default:
                return false;
        }

        return IsD1SectorUnlockedDirect(state, previousSectorId);
    }

    private static bool IsD1SectorUnlockedDirect(
        GameState state,
        string sectorId
    )
    {
        if (state == null || state.dimension1Sectors == null)
            return false;

        foreach (D1SectorState sector in state.dimension1Sectors)
        {
            if (sector != null && sector.sectorId == sectorId)
                return sector.unlocked;
        }

        return false;
    }

    private static bool IsD1PlanetUnlockedDirect(GameState state, string planetId)
    {
        if (state == null || state.dimension1Planets == null)
            return false;

        foreach (D1PlanetState planet in state.dimension1Planets)
        {
            if (planet != null && planet.planetId == planetId)
                return planet.unlocked;
        }

        return false;
    }

    private static bool IsD1ShipUnlockedDirect(GameState state, string shipId)
    {
        if (state == null || state.dimension1Ships == null)
            return false;

        foreach (D1ShipState ship in state.dimension1Ships)
        {
            if (ship != null && ship.shipId == shipId)
                return ship.unlocked;
        }

        return false;
    }

    private static int GetD1UnlockedRelicCountDirect(
        GameState state,
        string sectorId,
        int relicTier
    )
    {
        if (state == null || state.dimension1Relics == null)
            return 0;

        int count = 0;

        foreach (string relicId in Dimension1RelicIds)
        {
            if (!string.IsNullOrEmpty(sectorId) &&
                GetDimension1RelicSectorId(relicId) != sectorId)
            {
                continue;
            }

            if (relicTier > 0 && GetDimension1RelicTier(relicId) != relicTier)
                continue;

            foreach (D1RelicState relic in state.dimension1Relics)
            {
                if (relic == null || relic.relicId != relicId)
                    continue;

                if (relic.unlocked || relic.level > 0)
                    count++;

                break;
            }
        }

        return count;
    }

    private static int GetD1RelicCatalogCount(
        string sectorId,
        int relicTier
    )
    {
        int count = 0;

        foreach (string relicId in Dimension1RelicIds)
        {
            if (!string.IsNullOrEmpty(sectorId) &&
                GetDimension1RelicSectorId(relicId) != sectorId)
            {
                continue;
            }

            if (relicTier > 0 && GetDimension1RelicTier(relicId) != relicTier)
                continue;

            count++;
        }

        return count;
    }

    private static int GetD1RelicsAtOrAboveLevelDirect(
        GameState state,
        string sectorId,
        int relicTier,
        int minimumLevel
    )
    {
        if (state == null || state.dimension1Relics == null)
            return 0;

        int count = 0;

        foreach (string relicId in Dimension1RelicIds)
        {
            if (!string.IsNullOrEmpty(sectorId) &&
                GetDimension1RelicSectorId(relicId) != sectorId)
            {
                continue;
            }

            if (relicTier > 0 && GetDimension1RelicTier(relicId) != relicTier)
                continue;

            foreach (D1RelicState relic in state.dimension1Relics)
            {
                if (relic == null || relic.relicId != relicId)
                    continue;

                if ((relic.unlocked || relic.level > 0) &&
                    ClampDimension1RelicLevel(relic.level) >= minimumLevel)
                {
                    count++;
                }

                break;
            }
        }

        return count;
    }

    private static bool HasD1CargoMatrixOrAdaptiveFragmentDirect(GameState state)
    {
        if (state == null)
            return false;

        if (state.dimension1BlueprintFragments > 0)
            return true;

        if (state.dimension1Blueprints == null)
            return false;

        foreach (D1BlueprintAmount blueprint in state.dimension1Blueprints)
        {
            if (blueprint == null || blueprint.amount <= 0)
                continue;

            if (blueprint.blueprintId == BlueprintCargoFrame ||
                blueprint.blueprintId == BlueprintCargoHold ||
                blueprint.blueprintId == BlueprintCargoStabilizer)
            {
                return true;
            }
        }

        return false;
    }

    private static int GetD1PurchasedShipUpgradeCountDirect(GameState state)
    {
        if (state == null || state.dimension1Ships == null)
            return 0;

        int total = 0;

        foreach (D1ShipState ship in state.dimension1Ships)
        {
            if (ship == null ||
                !ship.unlocked ||
                !IsShipActiveInDimension1Base(ship.shipId))
            {
                continue;
            }

            total += Mathf.Max(0, ship.cargoLevel);
            total += Mathf.Max(0, ship.speedLevel);
            total += Mathf.Max(0, ship.armorLevel);
            total += Mathf.Max(0, ship.sensorsLevel);
        }

        return total;
    }

    private static int GetD1TreeTierDirect(GameState state, string nodeId)
    {
        if (state == null || state.dimension1TreeNodes == null)
            return 0;

        foreach (D1TreeNodeState node in state.dimension1TreeNodes)
        {
            if (node != null && node.nodeId == nodeId)
            {
                return ClampDimension1TreeNodeTier(nodeId, node.tier);
            }
        }

        return 0;
    }

    private static int GetD1PurchasedTreeNodeCountDirect(GameState state)
    {
        if (state == null)
            return 0;

        int count = 0;

        foreach (string nodeId in Dimension1TreeNodeIds)
        {
            if (GetD1TreeTierDirect(state, nodeId) > 0)
                count++;
        }

        return count;
    }

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
    public const int Dimension1RecentExplorationHistoryLimit = 20;

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

    public static string GetDimension1PlanetSectorId(string planetId)
    {
        switch (planetId)
        {
            case Planet01:
            case Planet02:
                return Sector01OuterRim;
            case Planet03:
                return Sector02DebrisRing;
            case Planet04:
            case Planet05:
                return Sector03AncientOrbits;
            case Planet06:
            case Planet07:
                return Sector04SilentFrontier;
            default:
                return "";
        }
    }

    public static bool IsDimension1PlanetInSector(
        string planetId,
        string sectorId
    )
    {
        return
            IsDimension1ExplorationSectorId(sectorId) &&
            GetDimension1PlanetSectorId(planetId) == sectorId;
    }

    public static string[] GetDimension1SectorDestinationIds(string sectorId)
    {
        switch (sectorId)
        {
            case Sector01OuterRim:
                return new string[]
                {
                    DestinationMineralBelt,
                    DestinationShipGraveyard,
                    DestinationDriftingProbes,
                    DestinationAbandonedShip
                };

            case Sector02DebrisRing:
                return new string[]
                {
                    DestinationShipGraveyard,
                    DestinationMineralBelt,
                    DestinationAbandonedShip,
                    DestinationDriftingProbes
                };

            case Sector03AncientOrbits:
                return new string[]
                {
                    DestinationAbandonedShip,
                    DestinationOrbitalRuin,
                    DestinationLaboratory,
                    DestinationAbandonedStation
                };

            case Sector04SilentFrontier:
                return new string[]
                {
                    DestinationOrbitalRuin,
                    DestinationMinorAnomaly,
                    DestinationAncientStructure,
                    DestinationUnstableZone
                };

            default:
                return new string[0];
        }
    }

    public static float GetDimension1SectorDestinationWeight(
        string sectorId,
        string destinationId
    )
    {
        switch (sectorId)
        {
            case Sector01OuterRim:
                if (destinationId == DestinationMineralBelt) return 0.40f;
                if (destinationId == DestinationShipGraveyard) return 0.30f;
                if (destinationId == DestinationDriftingProbes) return 0.20f;
                if (destinationId == DestinationAbandonedShip) return 0.10f;
                break;

            case Sector02DebrisRing:
                if (destinationId == DestinationShipGraveyard) return 0.30f;
                if (destinationId == DestinationMineralBelt) return 0.25f;
                if (destinationId == DestinationAbandonedShip) return 0.25f;
                if (destinationId == DestinationDriftingProbes) return 0.20f;
                break;

            case Sector03AncientOrbits:
                if (destinationId == DestinationAbandonedShip) return 0.25f;
                if (destinationId == DestinationOrbitalRuin) return 0.25f;
                if (destinationId == DestinationLaboratory) return 0.25f;
                if (destinationId == DestinationAbandonedStation) return 0.25f;
                break;

            case Sector04SilentFrontier:
                if (destinationId == DestinationOrbitalRuin) return 0.25f;
                if (destinationId == DestinationMinorAnomaly) return 0.25f;
                if (destinationId == DestinationAncientStructure) return 0.25f;
                if (destinationId == DestinationUnstableZone) return 0.25f;
                break;
        }

        return 0.0f;
    }

    public static bool IsDestinationInDimension1Sector(
        string destinationId,
        string sectorId
    )
    {
        return GetDimension1SectorDestinationWeight(
            sectorId,
            destinationId
        ) > 0.0f;
    }

    public static float GetDimension1SectorSpecialPointBaseChance(string sectorId)
    {
        switch (sectorId)
        {
            case Sector01OuterRim:
                return 0.03f;
            case Sector02DebrisRing:
                return 0.05f;
            case Sector03AncientOrbits:
                return 0.07f;
            case Sector04SilentFrontier:
                return 0.10f;
            default:
                return 0.0f;
        }
    }

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
    public const int SimpleScanMaxDestinationCount = 4;
    public const int SimpleScannerMinLevel = 1;
    public const int SimpleScannerMaxLevel = 15;
    public const int SimpleScannerProgressVersion = 1;
    public const float SimpleScannerRelicChancePerLevel = 0.0025f;
    public const float SimpleScannerSpecialPointChancePerLevel = 0.0025f;
    public const float SimpleScannerQualityPerLevel = 0.01f;
    public const int BlueprintFragmentsPerBlueprint = 10;
    public const int Dimension1ShipPartMaxLevel = 4;

    public const int Dimension1RelicMaxLevel = 100;
    public const int Dimension1RelicMilestoneStep = 25;
    public const int Dimension1RelicMinTier = 1;
    public const int Dimension1RelicMaxTier = 3;
    public const int Dimension1RelicPityTier = 1;
    public const int Dimension1RelicPityMaxPoints = 100;
    public const int Dimension1RelicPityPointsPerFailedAttempt = 2;
    public const int Dimension1RelicProgressVersion = 1;
    public const float Dimension1Tier1RelicBaseChance = 0.025f;
    public const float Dimension1Tier1RelicStrongDestinationBonus = 0.015f;
    public const float Dimension1Tier1RelicChanceCap = 0.065f;
    public const float Dimension1Tier2RelicBaseChance = 0.02f;
    public const float Dimension1Tier2RelicStrongDestinationBonus = 0.01f;
    public const float Dimension1Tier2AnalyticProbeBonus = 0.01f;
    public const float Dimension1Tier2RelicChanceCap = 0.065f;
    public const float Dimension1Tier3RelicBaseChance = 0.01f;
    public const float Dimension1Tier3RelicStrongDestinationBonus = 0.01f;
    public const float Dimension1Tier3AnalyticProbeBonus = 0.0075f;
    public const float Dimension1Tier3CargoShipBonus = 0.0075f;
    public const float Dimension1Tier3RelicChanceCap = 0.045f;
    public const float Dimension1RelicEchoChanceBonus = 0.01f;
    public const int Dimension1Prestige1PreviewPointCap = 12;

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
        // Sector 1 - Borde Exterior
        RelicDriftCompass,
        RelicExplorerPlate,
        RelicLostNavigationRecord,
        RelicIncompleteStarMap,
        RelicRoom1Echo,

        // Sector 2 - Anillo de Restos
        RelicExtractionHook,
        RelicAncientDrill,
        RelicAnalyticCrystal,
        RelicMatrixArchive,
        RelicTracesResonator,

        // Sector 3 - Orbitas Antiguas
        RelicRememberedAlloy,
        RelicFracturedAntenna,
        RelicAncientCargoCore,
        RelicModularContainer,
        RelicCalibrationFragment,

        // Sector 4 - Frontera Silenciosa
        RelicProspectingCore,
        RelicRareFrequencySensor,
        RelicExtractionSeal,
        RelicTriangularSeal,
        RelicMachineMemory
    };

    public static bool IsRelicActiveInDimension1Base(string relicId)
    {
        return IsDimension1RelicId(relicId);
    }

    public static int GetDimension1RelicTier(string relicId)
    {
        switch (relicId)
        {
            case RelicDriftCompass:
            case RelicExplorerPlate:
            case RelicExtractionHook:
            case RelicAncientDrill:
            case RelicRememberedAlloy:
            case RelicFracturedAntenna:
            case RelicProspectingCore:
                return 1;

            case RelicLostNavigationRecord:
            case RelicIncompleteStarMap:
            case RelicAnalyticCrystal:
            case RelicMatrixArchive:
            case RelicAncientCargoCore:
            case RelicModularContainer:
            case RelicRareFrequencySensor:
            case RelicExtractionSeal:
                return 2;

            case RelicRoom1Echo:
            case RelicTracesResonator:
            case RelicCalibrationFragment:
            case RelicTriangularSeal:
            case RelicMachineMemory:
                return 3;

            default:
                return 0;
        }
    }

    public static string GetDimension1RelicSectorId(string relicId)
    {
        switch (relicId)
        {
            case RelicDriftCompass:
            case RelicExplorerPlate:
            case RelicLostNavigationRecord:
            case RelicIncompleteStarMap:
            case RelicRoom1Echo:
                return Sector01OuterRim;

            case RelicExtractionHook:
            case RelicAncientDrill:
            case RelicAnalyticCrystal:
            case RelicMatrixArchive:
            case RelicTracesResonator:
                return Sector02DebrisRing;

            case RelicRememberedAlloy:
            case RelicFracturedAntenna:
            case RelicAncientCargoCore:
            case RelicModularContainer:
            case RelicCalibrationFragment:
                return Sector03AncientOrbits;

            case RelicProspectingCore:
            case RelicRareFrequencySensor:
            case RelicExtractionSeal:
            case RelicTriangularSeal:
            case RelicMachineMemory:
                return Sector04SilentFrontier;

            default:
                return "";
        }
    }

    public static string GetDimension1RelicVisualName(string relicId)
    {
        switch (relicId)
        {
            case RelicDriftCompass:
                return "Brújula de Deriva";
            case RelicExplorerPlate:
                return "Placa de Explorador";
            case RelicLostNavigationRecord:
                return "Registro de Navegación Perdido";
            case RelicIncompleteStarMap:
                return "Mapa Estelar Incompleto";
            case RelicRoom1Echo:
                return "Eco del Cuarto 1";
            case RelicExtractionHook:
                return "Gancho de Extracción";
            case RelicAncientDrill:
                return "Taladro Antiguo";
            case RelicAnalyticCrystal:
                return "Cristal Analítico";
            case RelicMatrixArchive:
                return "Archivo de Matrices";
            case RelicTracesResonator:
                return "Resonador de Trazas";
            case RelicRememberedAlloy:
                return "Aleación Recordada";
            case RelicFracturedAntenna:
                return "Antena Fracturada";
            case RelicAncientCargoCore:
                return "Núcleo de Bodega Antigua";
            case RelicModularContainer:
                return "Contenedor Modular";
            case RelicCalibrationFragment:
                return "Fragmento de Calibración";
            case RelicProspectingCore:
                return "Núcleo de Prospección";
            case RelicRareFrequencySensor:
                return "Sensor de Frecuencia Rara";
            case RelicExtractionSeal:
                return "Sello de Extracción";
            case RelicTriangularSeal:
                return "Sello Triangular";
            case RelicMachineMemory:
                return "Memoria de Máquina";
            default:
                return relicId ?? "";
        }
    }

    public static string GetDimension1RelicDiscoveryRequirementsText(
        GameState state,
        string relicId
    )
    {
        if (state == null || !IsDimension1RelicId(relicId))
            return "No disponible.";

        string sectorId = GetDimension1RelicSectorId(relicId);
        int relicTier = GetDimension1RelicTier(relicId);
        int scannerLevel = GetSimpleScannerLevel(state);
        int readingTier = GetD1TreeTierDirect(state, D1TreeRelicReading);
        string text = BuildD1RelicRequirementLine(
            IsD1SectorUnlockedDirect(state, sectorId),
            "Sector correspondiente desbloqueado"
        );

        if (relicTier == 1)
        {
            text += "\n" + BuildD1RelicRequirementLine(
                scannerLevel >= 5,
                "Escáner nivel 5 (" + scannerLevel + "/5)"
            );
            text += "\n" + BuildD1RelicRequirementLine(
                readingTier >= 1,
                "Lectura de Reliquias Tier 1 (" + readingTier + "/1)"
            );
            return text;
        }

        if (relicTier == 2)
        {
            int tier1Count = GetD1UnlockedRelicCountDirect(
                state,
                sectorId,
                1
            );
            text += "\n" + BuildD1RelicRequirementLine(
                scannerLevel >= 10,
                "Escáner nivel 10 (" + scannerLevel + "/10)"
            );
            text += "\n" + BuildD1RelicRequirementLine(
                readingTier >= 2,
                "Lectura de Reliquias Tier 2 (" + readingTier + "/2)"
            );
            text += "\n" + BuildD1RelicRequirementLine(
                tier1Count >= 1,
                "1 Reliquia Nivel 1 del sector (" + tier1Count + "/1)"
            );
            return text;
        }

        int lowerTierCount =
            GetD1UnlockedRelicCountDirect(state, sectorId, 1) +
            GetD1UnlockedRelicCountDirect(state, sectorId, 2);
        int requiredLowerTierCount =
            GetD1RelicCatalogCount(sectorId, 1) +
            GetD1RelicCatalogCount(sectorId, 2);
        int level25Count = GetD1RelicsAtOrAboveLevelDirect(
            state,
            sectorId,
            0,
            25
        );
        bool coordination =
            GetD1TreeTierDirect(state, D1TreeFleetCoordination) > 0;

        text += "\n" + BuildD1RelicRequirementLine(
            scannerLevel >= 15,
            "Escáner nivel 15 (" + scannerLevel + "/15)"
        );
        text += "\n" + BuildD1RelicRequirementLine(
            readingTier >= 3,
            "Lectura de Reliquias Tier 3 (" + readingTier + "/3)"
        );
        text += "\n" + BuildD1RelicRequirementLine(
            coordination,
            "Coordinación de Flota"
        );
        text += "\n" + BuildD1RelicRequirementLine(
            lowerTierCount >= requiredLowerTierCount,
            "Todas las Reliquias Nivel 1 y 2 del sector (" +
                lowerTierCount + "/" + requiredLowerTierCount + ")"
        );
        text += "\n" + BuildD1RelicRequirementLine(
            level25Count >= 2,
            "2 Reliquias del sector en nivel 25+ (" + level25Count + "/2)"
        );
        text += "\n[ ] Misión coordinada con 2 naves (aún no disponible)";
        return text;
    }

    private static string BuildD1RelicRequirementLine(bool met, string label)
    {
        return (met ? "[OK] " : "[ ] ") + label;
    }

    public static bool IsDimension1RelicCompatibleDestination(
        string relicId,
        string destinationId
    )
    {
        switch (relicId)
        {
            case RelicDriftCompass:
                return destinationId == DestinationMineralBelt ||
                    destinationId == DestinationDriftingProbes;
            case RelicExplorerPlate:
                return destinationId == DestinationShipGraveyard ||
                    destinationId == DestinationAbandonedShip;
            case RelicLostNavigationRecord:
                return destinationId == DestinationShipGraveyard ||
                    destinationId == DestinationDriftingProbes;
            case RelicIncompleteStarMap:
                return destinationId == DestinationMineralBelt ||
                    destinationId == DestinationShipGraveyard ||
                    destinationId == DestinationDriftingProbes ||
                    destinationId == DestinationAbandonedShip;
            case RelicRoom1Echo:
                return destinationId == DestinationDriftingProbes ||
                    destinationId == DestinationAbandonedShip;

            case RelicExtractionHook:
                return destinationId == DestinationMineralBelt ||
                    destinationId == DestinationShipGraveyard ||
                    destinationId == DestinationAbandonedShip;
            case RelicAncientDrill:
                return destinationId == DestinationMineralBelt ||
                    destinationId == DestinationShipGraveyard;
            case RelicAnalyticCrystal:
                return destinationId == DestinationDriftingProbes ||
                    destinationId == DestinationAbandonedShip;
            case RelicMatrixArchive:
                return destinationId == DestinationMineralBelt ||
                    destinationId == DestinationShipGraveyard ||
                    destinationId == DestinationDriftingProbes ||
                    destinationId == DestinationAbandonedShip;
            case RelicTracesResonator:
                return destinationId == DestinationShipGraveyard ||
                    destinationId == DestinationDriftingProbes ||
                    destinationId == DestinationAbandonedShip;

            case RelicRememberedAlloy:
                return destinationId == DestinationAbandonedShip ||
                    destinationId == DestinationAbandonedStation;
            case RelicFracturedAntenna:
                return destinationId == DestinationOrbitalRuin ||
                    destinationId == DestinationLaboratory ||
                    destinationId == DestinationAbandonedStation;
            case RelicAncientCargoCore:
                return destinationId == DestinationAbandonedShip ||
                    destinationId == DestinationOrbitalRuin ||
                    destinationId == DestinationLaboratory ||
                    destinationId == DestinationAbandonedStation;
            case RelicModularContainer:
                return destinationId == DestinationAbandonedShip ||
                    destinationId == DestinationAbandonedStation;
            case RelicCalibrationFragment:
                return destinationId == DestinationOrbitalRuin ||
                    destinationId == DestinationLaboratory ||
                    destinationId == DestinationAbandonedStation;

            case RelicProspectingCore:
                return destinationId == DestinationOrbitalRuin ||
                    destinationId == DestinationUnstableZone;
            case RelicRareFrequencySensor:
                return destinationId == DestinationOrbitalRuin ||
                    destinationId == DestinationMinorAnomaly ||
                    destinationId == DestinationAncientStructure;
            case RelicExtractionSeal:
                return destinationId == DestinationOrbitalRuin ||
                    destinationId == DestinationUnstableZone;
            case RelicTriangularSeal:
                return destinationId == DestinationMinorAnomaly ||
                    destinationId == DestinationAncientStructure ||
                    destinationId == DestinationUnstableZone;
            case RelicMachineMemory:
                return destinationId == DestinationMinorAnomaly ||
                    destinationId == DestinationAncientStructure ||
                    destinationId == DestinationUnstableZone;
            default:
                return false;
        }
    }

    public static bool IsDimension1RelicStrongDestination(
        string relicId,
        string destinationId
    )
    {
        switch (relicId)
        {
            case RelicDriftCompass:
            case RelicLostNavigationRecord:
                return destinationId == DestinationDriftingProbes;
            case RelicExplorerPlate:
            case RelicIncompleteStarMap:
                return destinationId == DestinationShipGraveyard;
            case RelicRoom1Echo:
                return destinationId == DestinationAbandonedShip;

            case RelicExtractionHook:
                return destinationId == DestinationShipGraveyard;
            case RelicAncientDrill:
                return destinationId == DestinationMineralBelt;
            case RelicAnalyticCrystal:
                return destinationId == DestinationDriftingProbes;
            case RelicMatrixArchive:
            case RelicTracesResonator:
                return destinationId == DestinationAbandonedShip;

            case RelicRememberedAlloy:
                return destinationId == DestinationAbandonedShip;
            case RelicFracturedAntenna:
            case RelicAncientCargoCore:
            case RelicModularContainer:
                return destinationId == DestinationAbandonedStation;
            case RelicCalibrationFragment:
                return destinationId == DestinationOrbitalRuin ||
                    destinationId == DestinationLaboratory;

            case RelicProspectingCore:
            case RelicExtractionSeal:
                return destinationId == DestinationUnstableZone;
            case RelicRareFrequencySensor:
                return destinationId == DestinationMinorAnomaly;
            case RelicTriangularSeal:
            case RelicMachineMemory:
                return destinationId == DestinationAncientStructure;
            default:
                return false;
        }
    }

    // IDs de puntos especiales
    public const int Dimension1TreeTieredNodeMaxTier = 3;
    public const int Dimension1TreeProgressVersion = 1;

    public const string D1SpecialPointRelicEcho = "d1_special_point_relic_echo";
    public const string D1SpecialPointMatrixTrace = "d1_special_point_matrix_trace";
    public const string D1SpecialPointMineralDeposit = "d1_special_point_mineral_deposit";
    public const string D1SpecialPointUnstableReading = "d1_special_point_unstable_reading";
    public const float Dimension1SpecialPointBaseScanChance = 0.03f;
    public const float Dimension1SpecialPointScanChanceCap = 0.25f;

    // Árbol D1 - Rama Exploración
    public const string D1TreeExplorationDestinationReading = "d1_tree_exploration_destination_reading";
    public const string D1TreeExplorationFilter = "d1_tree_exploration_filter";
    public const string D1TreeExplorationHiddenFindTracking = "d1_tree_exploration_hidden_find_tracking";
    public const string D1TreeExplorationContinuationDetected = "d1_tree_exploration_continuation_detected";
    public const string D1TreeExplorationScanMemory = "d1_tree_exploration_scan_memory";
    public const string D1TreeExplorationAdvancedCartography = "d1_tree_exploration_advanced_cartography";
    public const string D1TreeRelicReading = "d1_tree_relic_reading";

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
        D1TreeFleetHangarPreparation,
        D1TreeRecoveryCopyRegistry,
        D1TreeExplorationScanMemory,
        D1TreeRelicReading,
        D1TreeExplorationHiddenFindTracking,
        D1TreeFleetCoordination,
        D1TreeFleetSupportFormation,
        D1TreeExplorationAdvancedCartography,
        D1TreeConvergenceUnstableZoneStabilization
    };

    public static bool IsTreeNodeActiveInDimension1Base(string nodeId)
    {
        return
            nodeId == D1TreeExplorationDestinationReading ||
            nodeId == D1TreeFleetHangarPreparation ||
            nodeId == D1TreeRecoveryCopyRegistry ||
            nodeId == D1TreeExplorationScanMemory ||
            nodeId == D1TreeRelicReading ||
            nodeId == D1TreeExplorationHiddenFindTracking ||
            nodeId == D1TreeFleetCoordination ||
            nodeId == D1TreeFleetSupportFormation ||
            nodeId == D1TreeExplorationAdvancedCartography ||
            nodeId == D1TreeConvergenceUnstableZoneStabilization;
    }

    public static string GetDimension1TreeNodeVisualName(string nodeId)
    {
        switch (nodeId)
        {
            case D1TreeExplorationDestinationReading:
                return "Lectura de Destinos";
            case D1TreeExplorationHiddenFindTracking:
                return "Rastreo de Hallazgos Ocultos";
            case D1TreeExplorationScanMemory:
                return "Memoria de Escaneo";
            case D1TreeRelicReading:
                return "Lectura de Reliquias";
            case D1TreeFleetHangarPreparation:
                return "Preparación de Hangar";
            case D1TreeFleetCoordination:
                return "Coordinación de Flota";
            case D1TreeFleetSupportFormation:
                return "Optimización de Ruta";
            case D1TreeRecoveryCopyRegistry:
                return "Registro de Copias";
            case D1TreeExplorationAdvancedCartography:
                return "Cartografía Avanzada";
            case D1TreeConvergenceUnstableZoneStabilization:
                return "Estabilización de Zona Inestable";
            default:
                return nodeId ?? "";
        }
    }

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

        int relicTier = GetDimension1RelicTier(relicId);

        switch (relicTier)
        {
            case 1:
                baseLeCost = 2200.0;
                baseTraceCost = 10.0;
                baseMetalAmount1 = 80.0;
                baseMetalAmount2 = 40.0;
                break;
            case 2:
                baseLeCost = 4000.0;
                baseTraceCost = 18.0;
                baseMetalAmount1 = 100.0;
                baseMetalAmount2 = 60.0;
                break;
            case 3:
                baseLeCost = 6000.0;
                baseTraceCost = 25.0;
                baseMetalAmount1 = 120.0;
                baseMetalAmount2 = 80.0;
                break;
            default:
                return false;
        }

        switch (GetDimension1RelicSectorId(relicId))
        {
            case Sector01OuterRim:
                metal1 = MetalIron;
                metal2 = MetalAluminum;
                return true;
            case Sector02DebrisRing:
                metal1 = MetalAluminum;
                metal2 = MetalNickel;
                return true;
            case Sector03AncientOrbits:
                metal1 = MetalLithium;
                metal2 = MetalPlatinum;
                return true;
            case Sector04SilentFrontier:
                metal1 = MetalIridium;
                metal2 = MetalTungsten;
                return true;
            default:
                return false;
        }
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

        if (state.LE < leCost)
            return false;

        if (state.Traces < traceCost)
            return false;

        if (!string.IsNullOrEmpty(metal1) && state.GetD1MetalAmount(metal1) < metalAmount1)
            return false;

        if (!string.IsNullOrEmpty(metal2) && state.GetD1MetalAmount(metal2) < metalAmount2)
            return false;

        state.LE -= leCost;
        state.Traces -= traceCost;

        if (!string.IsNullOrEmpty(metal1))
            state.SpendD1Metal(metal1, metalAmount1);

        if (!string.IsNullOrEmpty(metal2))
            state.SpendD1Metal(metal2, metalAmount2);

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
        if (!IsDimension1TreeNodeId(nodeId))
            return 0;

        switch (nodeId)
        {
            case D1TreeExplorationHiddenFindTracking:
            case D1TreeExplorationScanMemory:
            case D1TreeRecoveryCopyRegistry:
            case D1TreeRelicReading:
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
            case D1TreeExplorationDestinationReading:
            case D1TreeFleetHangarPreparation:
                return 1;
            case D1TreeExplorationHiddenFindTracking:
            case D1TreeRecoveryCopyRegistry:
            case D1TreeExplorationScanMemory:
                return 1;
            case D1TreeRelicReading:
                return targetTier;
            case D1TreeFleetCoordination:
            case D1TreeFleetSupportFormation:
                return 2;
            case D1TreeExplorationAdvancedCartography:
                return 3;
            case D1TreeConvergenceUnstableZoneStabilization:
                return 3;

            default:
                return 999999;
        }
    }

    private static bool HasDimension1TreeNodePrerequisite(
        GameState state,
        string nodeId,
        int targetTier
    )
    {
        if (state == null)
            return false;

        switch (nodeId)
        {
            case D1TreeExplorationDestinationReading:
            case D1TreeFleetHangarPreparation:
                return IsD1SectorUnlockedDirect(state, Sector01OuterRim);

            case D1TreeRecoveryCopyRegistry:
            case D1TreeExplorationScanMemory:
                return IsD1SectorUnlockedDirect(state, Sector02DebrisRing);

            case D1TreeExplorationHiddenFindTracking:
                return targetTier <= 1
                    ? IsD1SectorUnlockedDirect(state, Sector02DebrisRing)
                    : IsD1SectorUnlockedDirect(state, Sector03AncientOrbits);

            case D1TreeRelicReading:
                if (targetTier == 1)
                {
                    return
                        IsD1SectorUnlockedDirect(state, Sector02DebrisRing) &&
                        GetSimpleScannerLevel(state) >= 5;
                }

                if (targetTier == 2)
                {
                    return
                        IsD1SectorUnlockedDirect(state, Sector03AncientOrbits) &&
                        GetSimpleScannerLevel(state) >= 10 &&
                        state.GetD1TreeNodeTier(D1TreeRelicReading) >= 1;
                }

                if (targetTier == 3)
                {
                    return
                        IsD1SectorUnlockedDirect(state, Sector04SilentFrontier) &&
                        GetSimpleScannerLevel(state) >= 15 &&
                        state.GetD1TreeNodeTier(D1TreeRelicReading) >= 2 &&
                        state.IsD1TreeNodeUnlocked(D1TreeFleetCoordination);
                }

                return false;

            case D1TreeFleetCoordination:
            case D1TreeFleetSupportFormation:
                return IsD1SectorUnlockedDirect(state, Sector03AncientOrbits);

            case D1TreeExplorationAdvancedCartography:
            case D1TreeConvergenceUnstableZoneStabilization:
                return IsD1SectorUnlockedDirect(state, Sector04SilentFrontier);

            default:
                return false;
        }
    }

    public static string GetDimension1TreeNodeDescription(string nodeId)
    {
        switch (nodeId)
        {
            case D1TreeExplorationDestinationReading:
                return "Mejora la vista previa de destinos y recompensas.";
            case D1TreeFleetHangarPreparation:
                return "+3% eficiencia en exploraciones realizadas con 1 nave.";
            case D1TreeRecoveryCopyRegistry:
                return "Mejora la conversión de Reliquias repetidas: +10% / +20% / +30%.";
            case D1TreeExplorationScanMemory:
                return "Reduce la repetición de destinos comunes: -3% / -6% / -10%.";
            case D1TreeRelicReading:
                return "Habilita la detección de Reliquias Nivel 1 / 2 / 3.";
            case D1TreeExplorationHiddenFindTracking:
                return "Mejora hallazgos ocultos compatibles: +2% / +4% / +6%.";
            case D1TreeFleetCoordination:
                return
                    "Permite enviar 2 naves a una misma exploración.\n\n" +
                    "Las exploraciones coordinadas duran más, pero entregan recompensas superiores:\n" +
                    "- Duración: ×2.5\n" +
                    "- Metales obtenidos: ×4.0\n" +
                    "- Fragmentos de Matriz Adaptativa: +150%\n" +
                    "- Matriz específica compatible: +15%\n" +
                    "- Probabilidad de Reliquia: +8%\n\n" +
                    "Algunas señales antiguas solo pueden ser recuperadas cuando una nave detecta y otra asegura el hallazgo.";
            case D1TreeFleetSupportFormation:
                return "Reduce 5% la duración de las exploraciones.";
            case D1TreeExplorationAdvancedCartography:
                return "+5% destinos raros compatibles y +2% probabilidad de punto especial.";
            case D1TreeConvergenceUnstableZoneStabilization:
                return "-5% duración peligrosa y +5% conservación de recompensa rara.";
            default:
                return "";
        }
    }

    public static string GetDimension1TreeNodeUnlockSummary(
        GameState state,
        string nodeId,
        int targetTier
    )
    {
        switch (nodeId)
        {
            case D1TreeExplorationDestinationReading:
            case D1TreeFleetHangarPreparation:
                return "Requiere Sector 1.";
            case D1TreeRecoveryCopyRegistry:
            case D1TreeExplorationScanMemory:
                return "Requiere Sector 2.";
            case D1TreeExplorationHiddenFindTracking:
                return targetTier <= 1
                    ? "Tier 1 requiere Sector 2."
                    : "Tier 2-3 requieren Sector 3.";
            case D1TreeRelicReading:
                if (targetTier <= 1)
                    return "Tier 1: Sector 2 y Escáner nivel 5.";
                if (targetTier == 2)
                    return "Tier 2: Sector 3, Escáner nivel 10 y Tier 1.";
                return "Tier 3: Sector 4, Escáner nivel 15, Tier 2 y Coordinación de Flota.";
            case D1TreeFleetCoordination:
            case D1TreeFleetSupportFormation:
                return "Requiere Sector 3.";
            case D1TreeExplorationAdvancedCartography:
            case D1TreeConvergenceUnstableZoneStabilization:
                return "Requiere Sector 4.";
            default:
                return "";
        }
    }

    public static int GetD1RelicReadingTier(GameState state)
    {
        return GetDimension1TreeTierSafe(state, D1TreeRelicReading);
    }

    public static int GetD1PurchasedTreeNodeCount(GameState state)
    {
        if (state == null)
            return 0;

        int count = 0;

        foreach (string nodeId in Dimension1TreeNodeIds)
        {
            if (GetDimension1TreeTierSafe(state, nodeId) > 0)
                count++;
        }

        return count;
    }

    public static int GetDimension1TreeTotalFullPurchaseCost()
    {
        int total = 0;

        foreach (string nodeId in Dimension1TreeNodeIds)
        {
            int maxTier = GetDimension1TreeNodeMaxTier(nodeId);

            for (int tier = 1; tier <= maxTier; tier++)
                total += GetDimension1TreeNodeCost(nodeId, tier);
        }

        return total;
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

        int targetTier = currentTier + 1;

        if (!HasDimension1TreeNodePrerequisite(state, nodeId, targetTier))
            return false;

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
        bool purchased = state.SetD1TreeNodeTier(nodeId, targetTier);

        if (purchased)
            state.RefreshD1SectorUnlocksFromProgress();

        return purchased;
    }

    private static int GetDimension1TreeTierSafe(GameState state, string nodeId)
    {
        if (state == null)
            return 0;

        if (!IsDimension1TreeNodeId(nodeId))
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
        float chance = 0.0f;

        if (HasDimension1TreeNode(state, D1TreeExplorationAdvancedCartography))
            chance += 0.05f;

        return Mathf.Clamp(chance, 0.0f, 0.25f);
    }

    public static float GetD1RareSpecialDestinationChance(GameState state)
    {
        float chance = 0.0f;

        chance += GetD1TreeAdvancedCartographySpecialDestinationChance(state);
        chance += GetLostNavigationRecordRareSpecialDestinationChance(state);

        return Mathf.Clamp(chance, 0.0f, 0.25f);
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

    public static float GetD1TreeRouteOptimizationDurationReduction(GameState state)
    {
        return HasDimension1TreeNode(state, D1TreeFleetSupportFormation)
            ? 0.05f
            : 0.0f;
    }

    // Nombre tecnico anterior conservado para debug y compatibilidad interna.
    public static float GetD1TreeSupportFormationValue(GameState state)
    {
        return GetD1TreeRouteOptimizationDurationReduction(state);
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

    public static float GetD1SpecialPointScanChance(GameState state)
    {
        string sectorId = state != null
            ? state.dimension1SelectedSectorId
            : Sector01OuterRim;

        float chance = GetDimension1SectorSpecialPointBaseChance(sectorId);

        chance += GetSimpleScannerSpecialPointChanceBonus(state);
        chance += GetD1TreeSpecialDestinationDetectionBonus(state);

        return Mathf.Clamp(chance, 0.0f, Dimension1SpecialPointScanChanceCap);
    }

    public static float GetD1TreeSpecialDestinationDetectionBonus(GameState state)
    {
        return HasDimension1TreeNode(state, D1TreeExplorationAdvancedCartography)
            ? 0.02f
            : 0.0f;
    }

    public static bool HasD1TreeUnstableZoneStabilization(GameState state)
    {
        return HasDimension1TreeNode(state, D1TreeConvergenceUnstableZoneStabilization);
    }

    public static float GetD1TreeUnstableZoneRiskReduction(GameState state)
    {
        // Parte 1 no tiene una variable real de riesgo o fallo.
        return 0.0f;
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
        if (state == null || !state.dimension01Unlocked)
            return 0;

        int rawPoints =
            CalculatePrestige1PointsFromD1Planets(state) +
            CalculatePrestige1PointsFromD1Ships(state) +
            CalculatePrestige1PointsFromD1Relics(state) +
            CalculatePrestige1PointsFromD1Tree(state) +
            CalculatePrestige1PointsFromD1Scanner(state);

        return Mathf.Min(rawPoints, Dimension1Prestige1PreviewPointCap);
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

        foreach (string planetId in StarterPlanets)
        {
            if (IsD1PlanetUnlockedForPrestigePreview(state, planetId))
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

    private static bool IsD1PlanetUnlockedForPrestigePreview(
        GameState state,
        string planetId
    )
    {
        if (state == null || string.IsNullOrEmpty(planetId))
            return false;

        foreach (D1PlanetState planet in state.dimension1Planets)
        {
            if (planet != null && planet.planetId == planetId && planet.unlocked)
                return true;
        }

        return false;
    }

    public static int CalculatePrestige1PointsFromD1Ships(GameState state)
    {
        if (state == null)
            return 0;

        state.EnsureDimension1State();

        int unlockedShips = 0;

        foreach (string shipId in Dimension1ActiveShipIds)
        {
            if (IsShipUnlockedForPrestigePreview(state, shipId))
                unlockedShips++;
        }

        int points = 0;

        if (unlockedShips >= 1)
            points += 1;

        if (unlockedShips >= 3)
            points += 2;

        if (IsShipUnlockedForPrestigePreview(state, ShipCargoShip))
            points += 2;

        return points;
    }

    private static bool IsShipUnlockedForPrestigePreview(GameState state, string shipId)
    {
        if (state == null || !IsShipActiveInDimension1Base(shipId))
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

        foreach (string relicId in Dimension1RelicIds)
        {
            int level = GetD1RelicProgressLevelForPrestigePreview(state, relicId);

            if (level <= 0)
                continue;

            unlockedRelics++;
            totalMilestones += level / Dimension1RelicMilestoneStep;
        }

        int pointsFromUnlocked = Mathf.Min(8, unlockedRelics / 3);
        int pointsFromMilestones = Mathf.Min(8, totalMilestones / 8);

        return pointsFromUnlocked + pointsFromMilestones;
    }

    private static int GetD1RelicProgressLevelForPrestigePreview(
        GameState state,
        string relicId
    )
    {
        if (state == null || !IsRelicActiveInDimension1Base(relicId))
            return 0;

        int highestLevel = 0;

        foreach (D1RelicState relic in state.dimension1Relics)
        {
            if (relic == null || relic.relicId != relicId || !relic.unlocked)
                continue;

            highestLevel = Mathf.Max(
                highestLevel,
                ClampDimension1RelicLevel(relic.level)
            );
        }

        return highestLevel;
    }

    public static int CalculatePrestige1PointsFromD1Tree(GameState state)
    {
        if (state == null)
            return 0;

        state.EnsureDimension1State();

        int totalPurchasedTiers = 0;

        foreach (string nodeId in Dimension1TreeNodeIds)
            totalPurchasedTiers += GetD1TreeTierForPrestigePreview(state, nodeId);

        return Mathf.Min(8, totalPurchasedTiers / 5);
    }

    private static int GetD1TreeTierForPrestigePreview(
        GameState state,
        string nodeId
    )
    {
        if (state == null || !IsTreeNodeActiveInDimension1Base(nodeId))
            return 0;

        int highestTier = 0;

        foreach (D1TreeNodeState node in state.dimension1TreeNodes)
        {
            if (node == null || node.nodeId != nodeId)
                continue;

            highestTier = Mathf.Max(
                highestTier,
                ClampDimension1TreeNodeTier(nodeId, node.tier)
            );
        }

        return highestTier;
    }

    public static int CalculatePrestige1PointsFromD1Scanner(GameState state)
    {
        if (state == null)
            return 0;

        // El nuevo nivel 1 equivale al antiguo nivel 0 para Prestigio 1.
        return Mathf.Clamp(GetSimpleScannerLevel(state) - SimpleScannerMinLevel, 0, 3);
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

    public static bool TryGetDimension1RelicMilestoneBonuses(
        string relicId,
        int milestoneLevel,
        out double primaryBonus,
        out double secondaryBonus
    )
    {
        primaryBonus = 0.0;
        secondaryBonus = 0.0;

        if (milestoneLevel != 25 &&
            milestoneLevel != 50 &&
            milestoneLevel != 75 &&
            milestoneLevel != 100)
        {
            return false;
        }

        double primaryPerMilestone;
        double secondaryPerMilestone;

        switch (relicId)
        {
            case RelicDriftCompass:
            case RelicAncientCargoCore:
            case RelicExplorerPlate:
            case RelicExtractionHook:
            case RelicModularContainer:
            case RelicAncientDrill:
                primaryPerMilestone = 0.01;
                secondaryPerMilestone = 0.005;
                break;

            case RelicLostNavigationRecord:
                primaryPerMilestone = 0.005;
                secondaryPerMilestone = 0.01;
                break;

            case RelicDormantEcho:
                primaryPerMilestone = 0.003;
                secondaryPerMilestone = 0.001;
                break;

            case RelicAnalyticCrystal:
                primaryPerMilestone = 0.005;
                secondaryPerMilestone = 0.002;
                break;

            case RelicRoom1Echo:
                primaryPerMilestone = 0.0075;
                secondaryPerMilestone = 0.005;
                break;

            case RelicRememberedAlloy:
                primaryPerMilestone = 0.0075;
                secondaryPerMilestone = 0.0075;
                break;

            case RelicProspectingCore:
                primaryPerMilestone = 0.015;
                secondaryPerMilestone = 0.0075;
                break;

            case RelicExtractionSeal:
                primaryPerMilestone = 0.01125;
                secondaryPerMilestone = 0.0075;
                break;

            case RelicFracturedAntenna:
                primaryPerMilestone = 0.00375;
                secondaryPerMilestone = 0.0;
                break;

            case RelicMatrixArchive:
                primaryPerMilestone = 0.0075;
                secondaryPerMilestone = 0.0075;
                break;

            default:
                return false;
        }

        double milestoneIndex = milestoneLevel / 25.0;
        primaryBonus = primaryPerMilestone * milestoneIndex;
        secondaryBonus = secondaryPerMilestone * milestoneIndex;
        return true;
    }

    public static double GetDimension1RelicPrimaryBonusForLevel(string relicId, int level)
    {
        return GetDimension1RelicInterpolatedBonus(relicId, level, false);
    }

    public static double GetDimension1RelicSecondaryBonusForLevel(string relicId, int level)
    {
        return GetDimension1RelicInterpolatedBonus(relicId, level, true);
    }

    public static double GetDimension1RelicPrimaryBonus(GameState state, string relicId)
    {
        if (!CanApplyDimension1RelicBonus(state, relicId))
            return 0.0;

        return GetDimension1RelicPrimaryBonusForLevel(
            relicId,
            state.GetD1RelicLevel(relicId)
        );
    }

    public static double GetDimension1RelicSecondaryBonus(GameState state, string relicId)
    {
        if (!CanApplyDimension1RelicBonus(state, relicId))
            return 0.0;

        return GetDimension1RelicSecondaryBonusForLevel(
            relicId,
            state.GetD1RelicLevel(relicId)
        );
    }

    private static bool CanApplyDimension1RelicBonus(GameState state, string relicId)
    {
        return
            state != null &&
            IsRelicActiveInDimension1Base(relicId) &&
            state.IsD1RelicUnlocked(relicId) &&
            state.GetD1RelicLevel(relicId) > 0;
    }

    private static double GetDimension1RelicInterpolatedBonus(
        string relicId,
        int level,
        bool secondary
    )
    {
        level = ClampDimension1RelicLevel(level);

        if (level <= 0 || !IsRelicActiveInDimension1Base(relicId))
            return 0.0;

        int lowerMilestone = (level / Dimension1RelicMilestoneStep) *
            Dimension1RelicMilestoneStep;

        if (lowerMilestone == level && lowerMilestone > 0)
        {
            if (!TryGetDimension1RelicMilestoneBonuses(
                relicId,
                lowerMilestone,
                out double exactPrimary,
                out double exactSecondary
            ))
            {
                return 0.0;
            }

            return secondary ? exactSecondary : exactPrimary;
        }

        int upperMilestone = Mathf.Min(
            lowerMilestone + Dimension1RelicMilestoneStep,
            Dimension1RelicMaxLevel
        );

        double lowerValue = 0.0;

        if (lowerMilestone > 0 &&
            TryGetDimension1RelicMilestoneBonuses(
                relicId,
                lowerMilestone,
                out double lowerPrimary,
                out double lowerSecondary
            ))
        {
            lowerValue = secondary ? lowerSecondary : lowerPrimary;
        }

        if (!TryGetDimension1RelicMilestoneBonuses(
            relicId,
            upperMilestone,
            out double upperPrimary,
            out double upperSecondary
        ))
        {
            return lowerValue;
        }

        double upperValue = secondary ? upperSecondary : upperPrimary;
        double range = upperMilestone - lowerMilestone;

        if (range <= 0.0)
            return upperValue;

        double interpolation = (level - lowerMilestone) / range;
        return lowerValue + ((upperValue - lowerValue) * interpolation);
    }

    public static float GetLostNavigationRecordRepetitionReduction(GameState state)
    {
        return (float)GetDimension1RelicSecondaryBonus(state, RelicLostNavigationRecord);
    }

    public static float GetLostNavigationRecordRareSpecialDestinationChance(GameState state)
    {
        return (float)GetDimension1RelicPrimaryBonus(state, RelicLostNavigationRecord);
    }

    public static double GetRoom1EchoGlobalLEBonus(GameState state)
    {
        return GetDimension1RelicPrimaryBonus(state, RelicRoom1Echo);
    }

    public static double GetRoom1EchoArtifactLEBonus(GameState state)
    {
        return GetDimension1RelicSecondaryBonus(state, RelicRoom1Echo);
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

    private static bool IsExplorerPlateBasicDestination(string destinationId)
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

    private static bool IsExplorerPlateMediumDestination(string destinationId)
    {
        switch (destinationId)
        {
            case DestinationOrbitalRuin:
            case DestinationLaboratory:
            case DestinationAbandonedStation:
            case DestinationMinorAnomaly:
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
        return GetRelicExplorationDurationMultiplier(state, destinationId, null);
    }

    private static double GetRelicExplorationDurationMultiplier(
        GameState state,
        string destinationId,
        D1ShipState ship
    )
    {
        double reduction = GetDimension1RelicPrimaryBonus(state, RelicDriftCompass);

        if (IsLongExplorationDestination(destinationId))
        {
            reduction += GetDimension1RelicSecondaryBonus(state, RelicDriftCompass);
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
            reduction += GetD1TreeRouteOptimizationDurationReduction(state);
        }

        if (destinationId == DestinationUnstableZone)
        {
            reduction += GetD1TreeUnstableZoneDurationReduction(state);
        }

        reduction += GetD1SpecialPointUnstableReadingDurationReduction(
            state,
            destinationId
        );

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

        bonus += GetDimension1RelicPrimaryBonus(state, RelicAncientCargoCore);

        if (IsLongExplorationDestination(destinationId))
        {
            bonus += GetDimension1RelicSecondaryBonus(state, RelicAncientCargoCore);
        }

        if (ship != null && ship.shipId == ShipLightProbe)
        {
            if (IsExplorerPlateBasicDestination(destinationId))
            {
                bonus += GetDimension1RelicPrimaryBonus(state, RelicExplorerPlate);
            }
        }

        if (ship != null && ship.shipId == ShipExtractorDrone)
        {
            bonus += GetDimension1RelicPrimaryBonus(state, RelicExtractionHook);
        }

        if (ship != null && ship.shipId == ShipCargoShip)
        {
            bonus += GetDimension1RelicPrimaryBonus(state, RelicModularContainer);

            if (IsLongExplorationDestination(destinationId))
            {
                bonus += GetDimension1RelicSecondaryBonus(state, RelicModularContainer);
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

        if (ship.shipId == ShipLightProbe &&
            IsExplorerPlateMediumDestination(destinationId))
        {
            bonus += GetDimension1RelicSecondaryBonus(state, RelicExplorerPlate);
        }

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
        double bonus = 0.0;

        bonus += GetDimension1RelicScaledBonus(
            state,
            RelicMatrixArchive,
            0.03
        );

        if (ship != null && ship.shipId == ShipAnalyticProbe)
        {
            bonus += GetDimension1RelicPrimaryBonus(state, RelicAnalyticCrystal);
        }

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
        bonus += GetDimension1RelicPrimaryBonus(state, RelicAncientDrill);

        // Taladro Antiguo: extra al recurso principal del planeta.
        if (IsPlanetPrimaryMetal(planet, metalId))
        {
            bonus += GetDimension1RelicSecondaryBonus(state, RelicAncientDrill);
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
        // En Parte 1, las matrices especificas sirven para construir Carga.
        // Las mejoras I-IV de todas las naves activas usan solo metales y,
        // para el nivel IV, una Matriz Adaptativa.
        if (IsShipActiveInDimension1Base(shipId))
            return 0;

        if (!UsesSpecificShipMatricesForUnlock(shipId))
            return 0;

        if (targetLevel < 4 || targetLevel > Dimension1ShipPartMaxLevel)
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
    // Catalogo completo conservado para guardados y debug de Parte 2.
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

    public static readonly string[] Dimension1Part1BlueprintIds =
    {
    BlueprintCargoFrame,
    BlueprintCargoHold,
    BlueprintCargoStabilizer
    };

    public static bool IsBlueprintActiveInDimension1Part1(string blueprintId)
    {
        return IsCargoShipSpecificBlueprint(blueprintId);
    }

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
            SimpleScannerMinLevel,
            SimpleScannerMaxLevel
        );
    }

    public static float GetSimpleScannerRelicChanceBonus(GameState state)
    {
        return GetSimpleScannerLevel(state) * SimpleScannerRelicChancePerLevel;
    }

    public static float GetSimpleScannerSpecialPointChanceBonus(GameState state)
    {
        return GetSimpleScannerLevel(state) * SimpleScannerSpecialPointChancePerLevel;
    }

    public static float GetSimpleScannerQualityBonus(GameState state)
    {
        return GetSimpleScannerLevel(state) * SimpleScannerQualityPerLevel;
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

        switch (nextLevel)
        {
            // Sector 1
            case 2:
                metal1 = MetalCopper;
                amount1 = 300.0;
                metal2 = MetalAluminum;
                amount2 = 180.0;
                return true;
            case 3:
                metal1 = MetalCopper;
                amount1 = 900.0;
                metal2 = MetalAluminum;
                amount2 = 500.0;
                metal3 = MetalTitanium;
                amount3 = 250.0;
                return true;

            // Sector 2
            case 4:
                metal1 = MetalNickel;
                amount1 = 300.0;
                metal2 = MetalCopper;
                amount2 = 1800.0;
                metal3 = MetalAluminum;
                amount3 = 900.0;
                metal4 = MetalTitanium;
                amount4 = 600.0;
                return true;
            case 5:
                metal1 = MetalNickel;
                amount1 = 900.0;
                metal2 = MetalCobalt;
                amount2 = 350.0;
                metal3 = MetalAluminum;
                amount3 = 1500.0;
                metal4 = MetalTitanium;
                amount4 = 900.0;
                return true;

            // Sector 3
            case 6:
                metal1 = MetalLithium;
                amount1 = 450.0;
                metal2 = MetalNickel;
                amount2 = 1200.0;
                metal3 = MetalCobalt;
                amount3 = 500.0;
                return true;
            case 7:
                metal1 = MetalLithium;
                amount1 = 900.0;
                metal2 = MetalPlatinum;
                amount2 = 250.0;
                metal3 = MetalNickel;
                amount3 = 1600.0;
                return true;
            case 8:
                metal1 = MetalLithium;
                amount1 = 1500.0;
                metal2 = MetalPlatinum;
                amount2 = 500.0;
                metal3 = MetalTungsten;
                amount3 = 250.0;
                return true;
            case 9:
                metal1 = MetalLithium;
                amount1 = 2400.0;
                metal2 = MetalPlatinum;
                amount2 = 850.0;
                metal3 = MetalTungsten;
                amount3 = 500.0;
                metal4 = MetalNickel;
                amount4 = 2400.0;
                return true;
            case 10:
                metal1 = MetalLithium;
                amount1 = 3600.0;
                metal2 = MetalPlatinum;
                amount2 = 1400.0;
                metal3 = MetalTungsten;
                amount3 = 900.0;
                metal4 = MetalNickel;
                amount4 = 3600.0;
                return true;

            // Sector 4
            case 11:
                metal1 = MetalIridium;
                amount1 = 300.0;
                metal2 = MetalTungsten;
                amount2 = 1200.0;
                metal3 = MetalCobalt;
                amount3 = 1600.0;
                return true;
            case 12:
                metal1 = MetalIridium;
                amount1 = 650.0;
                metal2 = MetalTungsten;
                amount2 = 1800.0;
                metal3 = MetalPlatinum;
                amount3 = 1600.0;
                return true;
            case 13:
                metal1 = MetalIridium;
                amount1 = 1100.0;
                metal2 = MetalTungsten;
                amount2 = 2600.0;
                metal3 = MetalCobalt;
                amount3 = 2600.0;
                metal4 = MetalPlatinum;
                amount4 = 2200.0;
                return true;
            case 14:
                metal1 = MetalIridium;
                amount1 = 1800.0;
                metal2 = MetalTungsten;
                amount2 = 3800.0;
                metal3 = MetalCobalt;
                amount3 = 3800.0;
                metal4 = MetalPlatinum;
                amount4 = 3200.0;
                return true;
            case 15:
                metal1 = MetalIridium;
                amount1 = 3000.0;
                metal2 = MetalTungsten;
                amount2 = 5500.0;
                metal3 = MetalCobalt;
                amount3 = 5200.0;
                metal4 = MetalPlatinum;
                amount4 = 4600.0;
                return true;
            default:
                return false;
        }
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
            SimpleScannerMinLevel,
            SimpleScannerMaxLevel
        );

        state.RefreshD1SectorUnlocksFromProgress();
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

    public static double GetSimpleExplorationDurationPreviewSeconds(
        GameState state,
        string destinationId,
        D1ShipState ship
    )
    {
        return GetSimpleExplorationDurationSeconds(state, destinationId, ship);
    }

    public static double GetSimpleExplorationDurationPreviewSeconds(string destinationId, D1ShipState ship)
    {
        return GetSimpleExplorationDurationPreviewSeconds(null, destinationId, ship);
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
    // Se conservan las seis para inicializar y migrar guardados de Parte 2.
    ShipLightProbe,
    ShipExtractorDrone,
    ShipAnalyticProbe,
    ShipCargoShip,
    ShipRescueShip,
    ShipConvergenceShip
    };

    public static readonly string[] Dimension1ActiveShipIds =
    {
        ShipLightProbe,
        ShipExtractorDrone,
        ShipAnalyticProbe,
        ShipCargoShip
    };

    public static bool IsShipActiveInDimension1Base(string shipId)
    {
        // Parte 1 oficial: Rescate y Convergencia permanecen guardadas,
        // pero no participan en el circuito normal hasta Parte 2.
        return
            shipId == ShipLightProbe ||
            shipId == ShipExtractorDrone ||
            shipId == ShipAnalyticProbe ||
            shipId == ShipCargoShip;
    }

    public static int ClampDimension1ShipPartLevel(int level)
    {
        return Mathf.Clamp(level, 0, Dimension1ShipPartMaxLevel);
    }

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

    public static bool IsD1PlanetInSelectedSector(
        GameState state,
        string planetId
    )
    {
        if (state == null)
            return false;

        return IsDimension1PlanetInSector(
            planetId,
            state.dimension1SelectedSectorId
        );
    }

    private static bool CanAccessD1PlanetInSelectedSector(
        GameState state,
        string planetId
    )
    {
        if (!IsD1PlanetInSelectedSector(state, planetId))
            return false;

        string sectorId = GetDimension1PlanetSectorId(planetId);
        return state.IsD1SectorUnlocked(sectorId);
    }

    public static bool CanUpgradeExtractor(GameState state, string planetId)
    {
        if (state == null)
            return false;

        if (!state.dimension01Unlocked)
            return false;

        state.EnsureDimension1State();

        if (!CanAccessD1PlanetInSelectedSector(state, planetId))
            return false;

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

        if (!CanAccessD1PlanetInSelectedSector(state, planetId))
            return false;

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

        if (!CanUpgradeExtractor(state, planetId))
            return false;

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

        state.RefreshD1SectorUnlocksFromProgress();
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

        if (!IsShipActiveInDimension1Base(shipId))
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
        state.RefreshD1SectorUnlocksFromProgress();
        return true;
    }

    public static bool CanUpgradeShipPart(GameState state, string shipId, string partId)
    {
        if (state == null || !state.dimension01Unlocked)
            return false;

        if (string.IsNullOrEmpty(shipId) || string.IsNullOrEmpty(partId))
            return false;

        if (!IsShipActiveInDimension1Base(shipId))
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

        if (!IsShipActiveInDimension1Base(shipId))
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

        if (targetLevel < 4 || targetLevel > Dimension1ShipPartMaxLevel)
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

        if (!IsShipActiveInDimension1Base(shipId))
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

        level = ClampDimension1ShipPartLevel(level);

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

        if (!IsDimension1ExplorationSectorId(state.dimension1SelectedSectorId))
            return false;

        if (!state.IsD1SectorUnlocked(state.dimension1SelectedSectorId))
            return false;

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

        state.dimension1PreviousScannedDestinationIds =
            GetCurrentScannedDestinationIds(state);

        state.dimension1ScannedDestinations.Clear();

        double scanDuration = GetSimpleScanDurationSeconds(state);

        state.dimension1ScanActive = true;
        state.dimension1ActiveScanSectorId = state.dimension1SelectedSectorId;
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

        if (!IsShipActiveInDimension1Base(shipId))
            return false;

        state.EnsureDimension1State();

        if (state.dimension1ScanActive)
            return false;

        D1ShipState ship = FindShipState(state, shipId);

        if (ship == null || !ship.unlocked)
            return false;

        if (ship.explorationActive)
            return false;

        D1ScannedDestinationState destination =
            GetAvailableScannedDestinationByIndex(state, destinationIndex);

        if (destination == null)
            return false;

        if (destination.sectorId != state.dimension1SelectedSectorId)
            return false;

        return !IsDestinationCurrentlyExplored(
            state,
            destination.destinationId,
            destination.sectorId
        );
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
        ship.activeSpecialPointId = string.IsNullOrEmpty(destination.specialPointId)
            ? ""
            : destination.specialPointId;
        ship.activeSectorId = destination.sectorId;
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
        state.dimension1ActiveScanSectorId = "";
    }

    private static void GenerateSimpleScannedDestinations(GameState state)
    {
        if (state == null)
            return;

        if (state.dimension1ScannedDestinations == null)
            state.dimension1ScannedDestinations = new List<D1ScannedDestinationState>();

        List<string> previousDestinationIds = state.dimension1PreviousScannedDestinationIds != null
            ? new List<string>(state.dimension1PreviousScannedDestinationIds)
            : new List<string>();

        state.dimension1ScannedDestinations.Clear();

        string scanSectorId = IsDimension1ExplorationSectorId(
            state.dimension1ActiveScanSectorId
        )
            ? state.dimension1ActiveScanSectorId
            : state.dimension1SelectedSectorId;

        int destinationCount = GetSimpleScanDestinationCountWithRelicRoll(state);
        List<string> destinations = PickSimpleDestinations(
            state,
            destinationCount,
            previousDestinationIds
        );

        destinations = ApplyRareSpecialDestinationRoll(
            state,
            destinations,
            previousDestinationIds
        );

        foreach (string destinationId in destinations)
        {
            state.dimension1ScannedDestinations.Add(new D1ScannedDestinationState
            {
                destinationId = destinationId,
                available = true,
                specialPointId = "",
                sectorId = scanSectorId
            });
        }

        TryAssignD1SpecialPointAfterScan(state);
        state.dimension1PreviousScannedDestinationIds.Clear();
    }

    private static void TryAssignD1SpecialPointAfterScan(GameState state)
    {
        if (state == null || state.dimension1ScannedDestinations == null)
            return;

        float chance = GetD1SpecialPointScanChance(state);

        if (chance <= 0.0f)
            return;

        if (Random.value > chance)
            return;

        TryForceD1SpecialPointOnFirstCompatibleScannedDestination(state);
    }

    public static bool TryForceD1SpecialPointOnFirstCompatibleScannedDestination(GameState state)
    {
        if (state == null || state.dimension1ScannedDestinations == null)
            return false;

        List<D1ScannedDestinationState> candidates = new List<D1ScannedDestinationState>();

        foreach (D1ScannedDestinationState destination in state.dimension1ScannedDestinations)
        {
            if (destination == null || !destination.available)
                continue;

            if (string.IsNullOrEmpty(destination.destinationId))
                continue;

            if (GetD1SpecialPointPoolForDestination(state, destination.destinationId).Length <= 0)
                continue;

            candidates.Add(destination);
        }

        if (candidates.Count <= 0)
            return false;

        D1ScannedDestinationState selected =
            candidates[Random.Range(0, candidates.Count)];

        string[] pool = GetD1SpecialPointPoolForDestination(
            state,
            selected.destinationId
        );

        if (pool == null || pool.Length == 0)
            return false;

        ClearAllD1SpecialPoints(state);
        selected.specialPointId = pool[Random.Range(0, pool.Length)];

        return true;
    }

    public static bool TryForceD1SpecialPointOnFirstCompatibleScannedDestination(
        GameState state,
        string specialPointId
    )
    {
        if (state == null ||
            state.dimension1ScannedDestinations == null ||
            string.IsNullOrEmpty(specialPointId))
        {
            return false;
        }

        D1ScannedDestinationState selected = null;

        foreach (D1ScannedDestinationState destination in state.dimension1ScannedDestinations)
        {
            if (destination == null || !destination.available)
                continue;

            if (string.IsNullOrEmpty(destination.destinationId))
                continue;

            string[] pool = GetD1SpecialPointPoolForDestination(
                state,
                destination.destinationId
            );

            if (!ContainsD1SpecialPoint(pool, specialPointId))
                continue;

            selected = destination;
            break;
        }

        if (selected == null)
            return false;

        ClearAllD1SpecialPoints(state);
        selected.specialPointId = specialPointId;
        return true;
    }

    private static void ClearAllD1SpecialPoints(GameState state)
    {
        if (state == null || state.dimension1ScannedDestinations == null)
            return;

        foreach (D1ScannedDestinationState destination in state.dimension1ScannedDestinations)
        {
            if (destination != null)
                destination.specialPointId = "";
        }
    }

    private static bool ContainsD1SpecialPoint(string[] pool, string specialPointId)
    {
        if (pool == null || string.IsNullOrEmpty(specialPointId))
            return false;

        for (int i = 0; i < pool.Length; i++)
        {
            if (pool[i] == specialPointId)
                return true;
        }

        return false;
    }

    private static string[] GetD1SpecialPointPoolForDestination(
        GameState state,
        string destinationId
    )
    {
        List<string> pool = new List<string>();

        if (GetExplorationRelicRewardPoolPreview(state, destinationId).Length > 0)
            pool.Add(D1SpecialPointRelicEcho);

        if (GetSpecificBlueprintPoolPreview(state, destinationId).Length > 0)
            pool.Add(D1SpecialPointMatrixTrace);

        if (GetExplorationMetalRewardPoolPreview(state, destinationId).Length > 0)
            pool.Add(D1SpecialPointMineralDeposit);

        if (IsD1UnstableSpecialPointDestination(destinationId))
            pool.Add(D1SpecialPointUnstableReading);

        return pool.ToArray();
    }

    private static bool IsD1UnstableSpecialPointDestination(string destinationId)
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

    public static string GetD1SpecialPointVisualName(string specialPointId)
    {
        switch (specialPointId)
        {
            case D1SpecialPointRelicEcho:
                return "Eco de Reliquia";

            case D1SpecialPointMatrixTrace:
                return "Rastro de Matriz";

            case D1SpecialPointMineralDeposit:
                return "Depósito Mineral";

            case D1SpecialPointUnstableReading:
                return "Lectura Inestable";

            default:
                return "Punto especial";
        }
    }

    public static string GetD1SpecialPointPreviewDescription(string specialPointId)
    {
        switch (specialPointId)
        {
            case D1SpecialPointRelicEcho:
                return "+1 punto porcentual de probabilidad de Reliquia en esta exploración.";

            case D1SpecialPointMatrixTrace:
                return "+5 puntos porcentuales de probabilidad de Matriz específica de Carga en esta exploración.";

            case D1SpecialPointMineralDeposit:
                return "+15% metales al completar esta exploración.";

            case D1SpecialPointUnstableReading:
                return "-5% duración en esta exploración.";

            default:
                return "Bonus especial activo en esta exploración.";
        }
    }

    private static string GetD1SpecialPointIdForDestination(
    GameState state,
    string destinationId
)
    {
        if (state == null || string.IsNullOrEmpty(destinationId))
        {
            return "";
        }

        if (state.dimension1Ships != null)
        {
            foreach (D1ShipState ship in state.dimension1Ships)
            {
                if (ship == null || !ship.explorationActive)
                    continue;

                if (ship.activeDestinationId != destinationId)
                    continue;

                if (!string.IsNullOrEmpty(ship.activeSpecialPointId))
                    return ship.activeSpecialPointId;
            }
        }

        if (state.dimension1ScannedDestinations == null)
            return "";

        foreach (D1ScannedDestinationState destination in state.dimension1ScannedDestinations)
        {
            if (destination == null)
                continue;

            if (destination.destinationId != destinationId)
                continue;

            return string.IsNullOrEmpty(destination.specialPointId)
                ? ""
                : destination.specialPointId;
        }

        return "";
    }

    private static float GetD1SpecialPointSpecificMatrixChanceBonus(
        GameState state,
        string destinationId
    )
    {
        string specialPointId = GetD1SpecialPointIdForDestination(
            state,
            destinationId
        );

        if (specialPointId != D1SpecialPointMatrixTrace)
            return 0.0f;

        if (GetSpecificBlueprintPoolPreview(state, destinationId).Length <= 0)
            return 0.0f;

        return 0.05f;
    }

    private static float GetD1SpecialPointRelicChanceBonus(
        GameState state,
        string destinationId
    )
    {
        string specialPointId = GetD1SpecialPointIdForDestination(
            state,
            destinationId
        );

        if (specialPointId != D1SpecialPointRelicEcho)
            return 0.0f;

        return Dimension1RelicEchoChanceBonus;
    }

    private static bool HasD1SpecialPoint(
        GameState state,
        string destinationId,
        string specialPointId
    )
    {
        return GetD1SpecialPointIdForDestination(state, destinationId) == specialPointId;
    }

    private static void ClearD1SpecialPointForDestination(
    GameState state,
    string destinationId
)
    {
        if (state == null ||
            state.dimension1ScannedDestinations == null ||
            string.IsNullOrEmpty(destinationId))
        {
            return;
        }

        foreach (D1ScannedDestinationState destination in state.dimension1ScannedDestinations)
        {
            if (destination == null)
                continue;

            if (destination.destinationId != destinationId)
                continue;

            destination.specialPointId = "";
        }
    }

    private static float GetD1SpecialPointUnstableReadingDurationReduction(
    GameState state,
    string destinationId
)
    {
        if (!HasD1SpecialPoint(state, destinationId, D1SpecialPointUnstableReading))
            return 0.0f;

        if (!IsD1UnstableSpecialPointDestination(destinationId))
            return 0.0f;

        return 0.05f;
    }

    private static void TryApplyD1SpecialPointMineralDepositBonus(
    GameState state,
    string destinationId,
    List<D1MetalAmount> rewards
)
    {
        if (state == null || rewards == null)
            return;

        if (!HasD1SpecialPoint(state, destinationId, D1SpecialPointMineralDeposit))
            return;

        if (rewards.Count <= 0)
            return;

        List<D1MetalAmount> snapshot = new List<D1MetalAmount>();

        foreach (D1MetalAmount reward in rewards)
        {
            if (reward == null)
                continue;

            if (string.IsNullOrEmpty(reward.metalId))
                continue;

            if (reward.amount <= 0.0)
                continue;

            snapshot.Add(new D1MetalAmount
            {
                metalId = reward.metalId,
                amount = reward.amount
            });
        }

        foreach (D1MetalAmount reward in snapshot)
        {
            double bonusAmount = reward.amount * 0.15;

            AddExplorationReward(
                state,
                rewards,
                reward.metalId,
                bonusAmount
            );
        }

        Debug.Log(
            "[D1 Special Point] Depósito Mineral aplicado en " +
            destinationId +
            " | Bonus: +15% metales"
        );
    }

    private static List<string> GetCurrentScannedDestinationIds(GameState state)
    {
        List<string> result = new List<string>();

        if (state == null || state.dimension1ScannedDestinations == null)
            return result;

        foreach (D1ScannedDestinationState destination in state.dimension1ScannedDestinations)
        {
            if (destination == null)
                continue;

            if (string.IsNullOrEmpty(destination.destinationId))
                continue;

            if (!result.Contains(destination.destinationId))
                result.Add(destination.destinationId);
        }

        return result;
    }

    private static int GetSimpleScanDestinationCount(GameState state)
    {
        int scannerLevel = GetSimpleScannerLevel(state);

        if (scannerLevel >= 10)
            return SimpleScanMaxDestinationCount;

        if (scannerLevel >= 2)
            return 3;

        return SimpleScanBaseDestinationCount;
    }

    private static int GetSimpleScanDestinationCountWithRelicRoll(GameState state)
    {
        int destinationCount = GetSimpleScanDestinationCount(state);

        if (destinationCount < SimpleScanMaxDestinationCount)
        {
            float extraDestinationChance = GetFracturedAntennaExtraScanDestinationChance(state);

            if (Random.value < extraDestinationChance)
                destinationCount += 1;
        }

        return Mathf.Clamp(
            destinationCount,
            SimpleScanBaseDestinationCount,
            SimpleScanMaxDestinationCount
        );
    }

    public static float GetFracturedAntennaExtraScanDestinationChancePreview(GameState state)
    {
        return GetFracturedAntennaExtraScanDestinationChance(state);
    }

    private static float GetFracturedAntennaExtraScanDestinationChance(GameState state)
    {
        double chance = GetDimension1RelicScaledBonus(
            state,
            RelicFracturedAntenna,
            0.015
        );

        return Mathf.Clamp((float)chance, 0.0f, 0.05f);
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

            // Las misiones antiguas de Parte 2 se conservan en el guardado,
            // pero permanecen pausadas mientras esas naves están congeladas.
            if (!IsShipActiveInDimension1Base(ship.shipId))
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
        string sectorId = ship.activeSectorId;

        if (!string.IsNullOrEmpty(destinationId))
        {
            GrantSimpleExplorationRewards(state, destinationId, ship);
            state.AddD1SectorExplorationCount(sectorId, 1);
        }

        ship.explorationActive = false;
        ship.activeDestinationId = "";
        ship.activeSpecialPointId = "";
        ship.activeSectorId = "";
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
        state.dimension1LastExplorationRelics = new List<D1RelicRewardEntry>();

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

        TryGrantExtractionHookSecondaryMetalReward(
            state,
            destinationId,
            ship,
            rewards,
            materialMultiplier
        );

        int blueprintFragments = RollSimpleBlueprintFragments(state, destinationId, ship);

        if (blueprintFragments > 0)
        {
            state.dimension1BlueprintFragments += blueprintFragments;
            state.dimension1LastExplorationBlueprintFragments = blueprintFragments;
        }

        TryGrantSpecificBlueprintReward(state, destinationId, ship);
        TryGrantExplorationRelicReward(state, destinationId, ship);

        TryApplyD1SpecialPointMineralDepositBonus(
            state,
            destinationId,
            rewards
        );

        state.dimension1LastExplorationDestinationId = destinationId;
        state.dimension1LastExplorationRewards = rewards;
        state.dimension1LastExplorationResultId += 1;

        AddRecentExplorationRecord(
            state,
            ship != null ? ship.shipId : "",
            destinationId,
            ship != null ? ship.activeSectorId : "",
            rewards,
            blueprintFragments,
            state.dimension1LastExplorationSpecificBlueprints,
            state.dimension1LastExplorationRelics
        );

        ClearD1SpecialPointForDestination(state, destinationId);
    }

    private static void AddRecentExplorationRecord(
        GameState state,
        string shipId,
        string destinationId,
        string sectorId,
        List<D1MetalAmount> rewards,
        int blueprintFragments,
        List<D1BlueprintAmount> specificBlueprintRewards,
        List<D1RelicRewardEntry> relicRewards
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
            sectorId = sectorId,
            rewards = new List<D1MetalAmount>(),
            blueprintFragments = blueprintFragments,
            specificBlueprintRewards = CloneBlueprintRewards(specificBlueprintRewards),
            relicRewards = CloneRelicRewards(relicRewards)
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

        while (state.dimension1RecentExplorationRecords.Count > Dimension1RecentExplorationHistoryLimit)
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

    private static List<D1RelicRewardEntry> CloneRelicRewards(List<D1RelicRewardEntry> rewards)
    {
        List<D1RelicRewardEntry> result = new List<D1RelicRewardEntry>();

        if (rewards == null)
            return result;

        foreach (D1RelicRewardEntry reward in rewards)
        {
            if (reward == null)
                continue;

            if (string.IsNullOrEmpty(reward.relicId))
                continue;

            result.Add(new D1RelicRewardEntry
            {
                relicId = reward.relicId,
                wasDuplicate = reward.wasDuplicate,
                duplicateMetalId = reward.duplicateMetalId,
                duplicateMetalAmount = reward.duplicateMetalAmount
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

    private static void TryGrantExplorationRelicReward(
        GameState state,
        string destinationId,
        D1ShipState ship
    )
    {
        if (state == null || string.IsNullOrEmpty(destinationId))
            return;

        string sectorId = ResolveD1RelicAttemptSectorId(
            state,
            destinationId,
            ship
        );

        bool validTier1Attempt = IsValidD1Tier1RelicPityAttempt(
            state,
            sectorId,
            destinationId
        );

        int currentPityPoints = validTier1Attempt
            ? state.GetD1RelicPityPoints(sectorId, Dimension1RelicPityTier)
            : 0;

        if (validTier1Attempt &&
            currentPityPoints + Dimension1RelicPityPointsPerFailedAttempt >=
            Dimension1RelicPityMaxPoints)
        {
            string guaranteedRelicId =
                PickMissingD1Tier1RelicForSector(state, sectorId);

            if (!string.IsNullOrEmpty(guaranteedRelicId))
            {
                GrantD1ExplorationRelicReward(state, guaranteedRelicId);
                state.SetD1RelicPityPoints(
                    sectorId,
                    Dimension1RelicPityTier,
                    0
                );
                return;
            }
        }

        string[] relicPool = GetExplorationRelicRewardPool(
            state,
            sectorId,
            destinationId,
            ship
        );

        bool discoveredNewTier1 = false;

        if (relicPool.Length > 0)
        {
            string relicId = relicPool[Random.Range(0, relicPool.Length)];
            float chance = GetExplorationRelicChanceForRelic(
                state,
                destinationId,
                ship,
                relicId
            );

            if (chance > 0.0f && Random.value <= chance)
            {
                bool wasDuplicate =
                    GrantD1ExplorationRelicReward(state, relicId);

                discoveredNewTier1 =
                    !wasDuplicate &&
                    GetDimension1RelicTier(relicId) ==
                        Dimension1RelicPityTier;
            }
        }

        if (!validTier1Attempt)
            return;

        state.SetD1RelicPityPoints(
            sectorId,
            Dimension1RelicPityTier,
            discoveredNewTier1
                ? 0
                : currentPityPoints +
                    Dimension1RelicPityPointsPerFailedAttempt
        );
    }

    private static bool GrantD1ExplorationRelicReward(
        GameState state,
        string relicId
    )
    {
        bool wasDuplicate = state.IsD1RelicUnlocked(relicId);

        D1RelicRewardEntry reward = new D1RelicRewardEntry
        {
            relicId = relicId,
            wasDuplicate = wasDuplicate,
            duplicateMetalId = "",
            duplicateMetalAmount = 0.0
        };

        if (wasDuplicate)
        {
            reward.duplicateMetalId = MetalIron;
            reward.duplicateMetalAmount =
                GetDuplicateRelicConversionAmount(state);

            state.AddD1Metal(
                reward.duplicateMetalId,
                reward.duplicateMetalAmount
            );
        }
        else
        {
            state.UnlockD1Relic(relicId);
        }

        if (state.dimension1LastExplorationRelics == null)
        {
            state.dimension1LastExplorationRelics =
                new List<D1RelicRewardEntry>();
        }

        state.dimension1LastExplorationRelics.Add(reward);
        return wasDuplicate;
    }

    public static float GetExplorationRelicChancePreview(
        GameState state,
        string destinationId,
        D1ShipState ship
    )
    {
        string sectorId = ResolveD1RelicAttemptSectorId(
            state,
            destinationId,
            ship
        );
        string[] pool = GetExplorationRelicRewardPool(
            state,
            sectorId,
            destinationId,
            ship
        );

        if (pool.Length == 0)
            return 0.0f;

        float totalChance = 0.0f;

        foreach (string relicId in pool)
        {
            totalChance += GetExplorationRelicChanceForRelic(
                state,
                destinationId,
                ship,
                relicId
            );
        }

        return totalChance / pool.Length;
    }

    public static float GetExplorationRelicTierChancePreview(
        GameState state,
        string destinationId,
        D1ShipState ship,
        int relicTier
    )
    {
        string sectorId = ResolveD1RelicAttemptSectorId(
            state,
            destinationId,
            ship
        );
        string[] pool = GetExplorationRelicRewardPool(
            state,
            sectorId,
            destinationId,
            ship
        );
        float totalChance = 0.0f;
        int compatibleCount = 0;

        foreach (string relicId in pool)
        {
            if (GetDimension1RelicTier(relicId) != relicTier)
                continue;

            totalChance += GetExplorationRelicChanceForRelic(
                state,
                destinationId,
                ship,
                relicId
            );
            compatibleCount++;
        }

        return compatibleCount > 0
            ? totalChance / compatibleCount
            : 0.0f;
    }

    public static string[] GetExplorationRelicRewardPoolPreview(
        GameState state,
        string destinationId
    )
    {
        string sectorId = ResolveD1RelicAttemptSectorId(
            state,
            destinationId,
            null
        );
        return GetExplorationRelicRewardPool(
            state,
            sectorId,
            destinationId,
            null
        );
    }

    public static bool IsD1Tier1RelicPityAttemptPreview(
        GameState state,
        string destinationId
    )
    {
        string sectorId = ResolveD1RelicAttemptSectorId(
            state,
            destinationId,
            null
        );

        return IsValidD1Tier1RelicPityAttempt(
            state,
            sectorId,
            destinationId
        );
    }

    public static bool IsD1RelicTierAvailableForSimpleExplorationPreview(
        GameState state,
        string sectorId,
        int relicTier
    )
    {
        return IsD1RelicTierAvailableForAttempt(
            state,
            sectorId,
            relicTier,
            null
        );
    }

    private static float GetExplorationRelicChanceForRelic(
        GameState state,
        string destinationId,
        D1ShipState ship,
        string relicId
    )
    {
        int relicTier = GetDimension1RelicTier(relicId);
        float chance;
        float cap;

        switch (relicTier)
        {
            case 1:
                chance = Dimension1Tier1RelicBaseChance;
                cap = Dimension1Tier1RelicChanceCap;

                if (IsDimension1RelicStrongDestination(relicId, destinationId))
                {
                    chance += Dimension1Tier1RelicStrongDestinationBonus;
                }
                break;

            case 2:
                chance = Dimension1Tier2RelicBaseChance;
                cap = Dimension1Tier2RelicChanceCap;

                if (IsDimension1RelicStrongDestination(relicId, destinationId))
                {
                    chance += Dimension1Tier2RelicStrongDestinationBonus;
                }

                if (ship != null && ship.shipId == ShipAnalyticProbe)
                    chance += Dimension1Tier2AnalyticProbeBonus;
                break;

            case 3:
                chance = Dimension1Tier3RelicBaseChance;
                cap = Dimension1Tier3RelicChanceCap;

                if (IsDimension1RelicStrongDestination(relicId, destinationId))
                {
                    chance += Dimension1Tier3RelicStrongDestinationBonus;
                }

                if (ship != null && ship.shipId == ShipAnalyticProbe)
                    chance += Dimension1Tier3AnalyticProbeBonus;

                if (ship != null && ship.shipId == ShipCargoShip)
                    chance += Dimension1Tier3CargoShipBonus;
                break;

            default:
                return 0.0f;
        }

        chance += GetSimpleScannerRelicChanceBonus(state);
        chance += GetD1RelicEchoChanceBonusForAttempt(
            state,
            destinationId,
            ship
        );
        return Mathf.Clamp(chance, 0.0f, cap);
    }

    private static float GetD1RelicEchoChanceBonusForAttempt(
        GameState state,
        string destinationId,
        D1ShipState ship
    )
    {
        if (ship != null &&
            ship.explorationActive &&
            ship.activeDestinationId == destinationId)
        {
            return ship.activeSpecialPointId == D1SpecialPointRelicEcho
                ? Dimension1RelicEchoChanceBonus
                : 0.0f;
        }

        return GetD1SpecialPointRelicChanceBonus(state, destinationId);
    }

    private static string[] GetExplorationRelicRewardPool(
        GameState state,
        string sectorId,
        string destinationId,
        D1ShipState ship
    )
    {
        if (state == null ||
            !IsDimension1ExplorationSectorId(sectorId) ||
            string.IsNullOrEmpty(destinationId))
        {
            return new string[0];
        }

        List<string> filteredPool = new List<string>();

        foreach (string relicId in Dimension1RelicIds)
        {
            if (GetDimension1RelicSectorId(relicId) != sectorId)
                continue;

            if (!IsDimension1RelicCompatibleDestination(
                    relicId,
                    destinationId
                ))
            {
                continue;
            }

            int relicTier = GetDimension1RelicTier(relicId);

            if (relicTier == 3 &&
                !IsDimension1RelicStrongDestination(
                    relicId,
                    destinationId
                ))
            {
                continue;
            }

            if (!IsD1RelicTierAvailableForAttempt(
                    state,
                    sectorId,
                    relicTier,
                    ship
                ))
            {
                continue;
            }

            filteredPool.Add(relicId);
        }

        return filteredPool.ToArray();
    }

    private static bool IsD1RelicTierAvailableForAttempt(
        GameState state,
        string sectorId,
        int relicTier,
        D1ShipState ship
    )
    {
        if (state == null ||
            !IsD1SectorUnlockedForRelicAttemptDirect(state, sectorId))
        {
            return false;
        }

        int scannerLevel = GetSimpleScannerLevel(state);
        int readingTier = GetD1TreeTierDirect(state, D1TreeRelicReading);

        if (relicTier == 1)
            return scannerLevel >= 5 && readingTier >= 1;

        if (relicTier == 2)
        {
            return
                scannerLevel >= 10 &&
                readingTier >= 2 &&
                GetD1UnlockedRelicCount(state, sectorId, 1) >= 1;
        }

        if (relicTier != 3)
            return false;

        // Las exploraciones coordinadas se implementan en el Bloque 5.
        // Una exploración simple nunca puede producir Reliquias Nivel 3.
        if (!IsD1CoordinatedRelicAttempt(ship))
            return false;

        return
            scannerLevel >= 15 &&
            readingTier >= 3 &&
            GetD1TreeTierDirect(state, D1TreeFleetCoordination) > 0 &&
            AreAllD1RelicsUnlockedInTierRange(state, sectorId, 1, 2) &&
            GetD1RelicsAtOrAboveLevel(state, sectorId, 25) >= 2;
    }

    private static bool IsD1CoordinatedRelicAttempt(D1ShipState ship)
    {
        return false;
    }

    private static int GetD1UnlockedRelicCount(
        GameState state,
        string sectorId,
        int relicTier
    )
    {
        int count = 0;

        foreach (string relicId in Dimension1RelicIds)
        {
            if (GetDimension1RelicSectorId(relicId) != sectorId ||
                GetDimension1RelicTier(relicId) != relicTier)
            {
                continue;
            }

            if (IsD1RelicUnlockedForAttemptDirect(state, relicId))
                count++;
        }

        return count;
    }

    private static int GetD1RelicsAtOrAboveLevel(
        GameState state,
        string sectorId,
        int minimumLevel
    )
    {
        int count = 0;

        foreach (string relicId in Dimension1RelicIds)
        {
            if (GetDimension1RelicSectorId(relicId) != sectorId)
                continue;

            if (GetD1RelicLevelForAttemptDirect(state, relicId) >= minimumLevel)
                count++;
        }

        return count;
    }

    private static bool AreAllD1RelicsUnlockedInTierRange(
        GameState state,
        string sectorId,
        int minimumTier,
        int maximumTier
    )
    {
        foreach (string relicId in Dimension1RelicIds)
        {
            if (GetDimension1RelicSectorId(relicId) != sectorId)
                continue;

            int relicTier = GetDimension1RelicTier(relicId);

            if (relicTier < minimumTier || relicTier > maximumTier)
                continue;

            if (!IsD1RelicUnlockedForAttemptDirect(state, relicId))
                return false;
        }

        return true;
    }

    private static bool IsValidD1Tier1RelicPityAttempt(
        GameState state,
        string sectorId,
        string destinationId
    )
    {
        if (!IsD1RelicTierAvailableForAttempt(
                state,
                sectorId,
                Dimension1RelicPityTier,
                null
            ))
        {
            return false;
        }

        bool hasMissingTier1 = false;
        bool hasCompatibleTier1 = false;

        foreach (string relicId in Dimension1RelicIds)
        {
            if (GetDimension1RelicSectorId(relicId) != sectorId ||
                GetDimension1RelicTier(relicId) != Dimension1RelicPityTier)
            {
                continue;
            }

            if (!IsD1RelicUnlockedForAttemptDirect(state, relicId))
                hasMissingTier1 = true;

            if (IsDimension1RelicCompatibleDestination(
                    relicId,
                    destinationId
                ))
            {
                hasCompatibleTier1 = true;
            }
        }

        return hasMissingTier1 && hasCompatibleTier1;
    }

    private static string PickMissingD1Tier1RelicForSector(
        GameState state,
        string sectorId
    )
    {
        List<string> missingRelics = new List<string>();

        foreach (string relicId in Dimension1RelicIds)
        {
            if (GetDimension1RelicSectorId(relicId) != sectorId ||
                GetDimension1RelicTier(relicId) != Dimension1RelicPityTier ||
                IsD1RelicUnlockedForAttemptDirect(state, relicId))
            {
                continue;
            }

            missingRelics.Add(relicId);
        }

        return missingRelics.Count > 0
            ? missingRelics[Random.Range(0, missingRelics.Count)]
            : "";
    }

    private static bool IsD1SectorUnlockedForRelicAttemptDirect(
        GameState state,
        string sectorId
    )
    {
        if (state == null || state.dimension1Sectors == null)
            return false;

        foreach (D1SectorState sector in state.dimension1Sectors)
        {
            if (sector != null && sector.sectorId == sectorId)
                return sector.unlocked;
        }

        return false;
    }

    private static bool IsD1RelicUnlockedForAttemptDirect(
        GameState state,
        string relicId
    )
    {
        if (state == null || state.dimension1Relics == null)
            return false;

        foreach (D1RelicState relic in state.dimension1Relics)
        {
            if (relic != null && relic.relicId == relicId)
                return relic.unlocked || relic.level > 0;
        }

        return false;
    }

    private static int GetD1RelicLevelForAttemptDirect(
        GameState state,
        string relicId
    )
    {
        if (state == null || state.dimension1Relics == null)
            return 0;

        foreach (D1RelicState relic in state.dimension1Relics)
        {
            if (relic != null && relic.relicId == relicId)
                return ClampDimension1RelicLevel(relic.level);
        }

        return 0;
    }

    private static string ResolveD1RelicAttemptSectorId(
        GameState state,
        string destinationId,
        D1ShipState ship
    )
    {
        if (ship != null &&
            IsDimension1ExplorationSectorId(ship.activeSectorId))
        {
            return ship.activeSectorId;
        }

        if (state != null &&
            IsDimension1ExplorationSectorId(state.dimension1SelectedSectorId))
        {
            return state.dimension1SelectedSectorId;
        }

        return "";
    }

    public static double GetD1DuplicateRelicConversionPreviewAmount(GameState state)
    {
        return GetDuplicateRelicConversionAmount(state);
    }

    private static double GetDuplicateRelicConversionAmount(GameState state)
    {
        double amount = 50.0;

        amount *= 1.0 + GetD1TreeDuplicateRelicConversionBonus(state);

        return System.Math.Round(amount, 0, System.MidpointRounding.AwayFromZero);
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
        chance += GetRelicSpecificBlueprintChanceBonus(state, destinationId, ship);
        chance += GetD1TreeHiddenFindSpecificMatrixBonus(state, destinationId);
        chance += GetD1SpecialPointSpecificMatrixChanceBonus(state, destinationId);

        return Mathf.Clamp(chance, 0.0f, 0.20f);
    }

    private static float GetD1TreeHiddenFindSpecificMatrixBonus(
    GameState state,
    string destinationId
)
    {
        float bonus = GetD1TreeHiddenFindQualityBonus(state);

        if (bonus <= 0.0f)
            return 0.0f;

        string[] availablePool = GetSpecificBlueprintPoolForDestination(state, destinationId);

        if (availablePool == null || availablePool.Length == 0)
            return 0.0f;

        return bonus;
    }

    private static float GetRelicSpecificBlueprintChanceBonus(
    GameState state,
    string destinationId,
    D1ShipState ship
)
    {
        double bonus = 0.0;

        bonus += GetDimension1RelicScaledBonus(
            state,
            RelicMatrixArchive,
            0.03
        );

        if (ship != null && ship.shipId == ShipAnalyticProbe)
        {
            bonus += GetDimension1RelicPrimaryBonus(state, RelicAnalyticCrystal);
        }

        return (float)bonus;
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

        string priorityBlueprintId = TryPickPrioritySpecificBlueprint(state, pool);

        if (!string.IsNullOrEmpty(priorityBlueprintId))
            return priorityBlueprintId;

        return pool[Random.Range(0, pool.Length)];
    }

    private static string TryPickPrioritySpecificBlueprint(GameState state, string[] pool)
    {
        if (state == null || pool == null || pool.Length == 0)
            return "";

        float priorityChance = GetD1TreeBlueprintPriorityBonus(state);

        if (priorityChance <= 0.0f)
            return "";

        if (Random.value > priorityChance)
            return "";

        List<string> missingPool = new List<string>();

        for (int i = 0; i < pool.Length; i++)
        {
            string blueprintId = pool[i];

            if (string.IsNullOrEmpty(blueprintId))
                continue;

            if (!IsSpecificBlueprintMissingForCurrentShipUnlock(state, blueprintId))
                continue;

            missingPool.Add(blueprintId);
        }

        if (missingPool.Count == 0)
            return "";

        return missingPool[Random.Range(0, missingPool.Count)];
    }

    private static bool IsSpecificBlueprintMissingForCurrentShipUnlock(GameState state, string blueprintId)
    {
        if (state == null || string.IsNullOrEmpty(blueprintId))
            return false;

        string targetShipId = GetShipIdForSpecificBlueprint(blueprintId);

        if (string.IsNullOrEmpty(targetShipId))
            return false;

        D1ShipState ship = FindShipState(state, targetShipId);

        if (ship != null && ship.unlocked)
            return false;

        return state.GetD1BlueprintAmount(blueprintId) <= 0;
    }

    private static string GetShipIdForSpecificBlueprint(string blueprintId)
    {
        if (IsCargoShipSpecificBlueprint(blueprintId))
            return ShipCargoShip;

        if (IsRescueShipSpecificBlueprint(blueprintId))
            return ShipRescueShip;

        if (IsConvergenceShipSpecificBlueprint(blueprintId))
            return ShipConvergenceShip;

        return "";
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
        if (!IsDimension1BlueprintId(blueprintId))
            return false;

        // Parte 1 solo entrega matrices especificas de Nave de Carga.
        // Las matrices futuras siguen siendo IDs validos para guardados/debug.
        return IsBlueprintActiveInDimension1Part1(blueprintId);
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

        int sensorsLevel = ClampDimension1ShipPartLevel(ship.sensorsLevel);

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
            int cargoLevel = ClampDimension1ShipPartLevel(ship.cargoLevel);

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
            int armorLevel = ClampDimension1ShipPartLevel(ship.armorLevel);

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

        double treePreservationMultiplier = 1.0;

        if (destinationId == DestinationUnstableZone)
        {
            treePreservationMultiplier +=
                GetD1TreeUnstableZoneRareRewardProtection(state);
        }

        return
            shipMultiplier *
            GetRelicArmorPreservationMultiplier(state, destinationId, ship) *
            treePreservationMultiplier;
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
        TryApplyPartialRecoveryMetalBonus(state, rewards, metalId, amount);
    }

    private static void TryGrantExtractionHookSecondaryMetalReward(
        GameState state,
        string destinationId,
        D1ShipState ship,
        List<D1MetalAmount> rewards,
        double materialMultiplier
    )
    {
        if (state == null || ship == null || rewards == null)
            return;

        if (ship.shipId != ShipExtractorDrone)
            return;

        double chance = GetDimension1RelicSecondaryBonus(state, RelicExtractionHook);

        if (chance <= 0.0 || Random.value > chance)
            return;

        string[] pool = GetExplorationMetalRewardPool(destinationId);

        if (pool == null || pool.Length <= 1)
            return;

        List<string> compatibleSecondaryMetals = new List<string>();

        // El primer metal del pool es el principal del destino. El resto solo
        // puede recibir esta recuperacion extra si ya esta desbloqueado.
        for (int i = 1; i < pool.Length; i++)
        {
            string metalId = pool[i];

            if (string.IsNullOrEmpty(metalId))
                continue;

            if (!IsMetalUnlockedForDimension1(state, metalId))
                continue;

            if (GetBaseMetalProductionPerSecond(state, metalId) <= 0.0)
                continue;

            compatibleSecondaryMetals.Add(metalId);
        }

        if (compatibleSecondaryMetals.Count == 0)
            return;

        string selectedMetal = compatibleSecondaryMetals[
            Random.Range(0, compatibleSecondaryMetals.Count)
        ];

        AddExplorationTimedReward(
            state,
            rewards,
            selectedMetal,
            materialMultiplier
        );
    }

    private static void TryApplyPartialRecoveryMetalBonus(
    GameState state,
    List<D1MetalAmount> rewards,
    string metalId,
    double baseAmount
)
    {
        if (state == null || rewards == null)
            return;

        if (string.IsNullOrEmpty(metalId))
            return;

        if (baseAmount <= 0.0)
            return;

        GetD1TreePartialRecoveryValues(
            state,
            out float chance,
            out float recoveredAmount
        );

        if (chance <= 0.0f || recoveredAmount <= 0.0f)
            return;

        if (Random.value > chance)
            return;

        double bonusAmount = baseAmount * recoveredAmount;

        AddExplorationReward(state, rewards, metalId, bonusAmount);

        Debug.Log(
            "[D1 Recovery] Recuperación de Materiales: +" +
            bonusAmount.ToString("0.0") +
            " " +
            metalId +
            " | Chance: " +
            (chance * 100f).ToString("0.#") +
            "% | Recuperado: +" +
            (recoveredAmount * 100f).ToString("0.#") +
            "%"
        );
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

        D1MetalAmount existingReward = FindExplorationRewardEntry(
            rewards,
            metalId
        );

        if (existingReward != null)
        {
            existingReward.amount += amount;
            return;
        }

        rewards.Add(new D1MetalAmount
        {
            metalId = metalId,
            amount = amount
        });
    }

    private static D1MetalAmount FindExplorationRewardEntry(
    List<D1MetalAmount> rewards,
    string metalId
    )
    {
        if (rewards == null || string.IsNullOrEmpty(metalId))
            return null;

        foreach (D1MetalAmount reward in rewards)
        {
            if (reward == null)
                continue;

            if (reward.metalId == metalId)
                return reward;
        }

        return null;
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
            return baseDuration * GetRelicExplorationDurationMultiplier(state, destinationId, ship);

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
            GetRelicExplorationDurationMultiplier(state, destinationId, ship) *
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
        speedLevel = ClampDimension1ShipPartLevel(speedLevel);

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
        double baseDuration = SimpleScanDurationSeconds;

        D1ShipState analyticProbe = FindShipState(state, ShipAnalyticProbe);

        if (analyticProbe != null && analyticProbe.unlocked)
        {
            int sensorsLevel = ClampDimension1ShipPartLevel(analyticProbe.sensorsLevel);

            if (sensorsLevel >= 6)
                baseDuration = 2.1;
            else if (sensorsLevel >= 5)
                baseDuration = 2.4;
            else if (sensorsLevel >= 4)
                baseDuration = 2.7;
            else if (sensorsLevel >= 3)
                baseDuration = 3.0;
            else if (sensorsLevel >= 2)
                baseDuration = 3.5;
            else if (sensorsLevel >= 1)
                baseDuration = 4.0;
        }

        return baseDuration * GetRelicScanDurationMultiplier(state);
    }

    private static double GetRelicScanDurationMultiplier(GameState state)
    {
        double reduction = GetDimension1RelicScaledBonus(
            state,
            RelicFracturedAntenna,
            0.06
        );

        return System.Math.Max(0.75, 1.0 - reduction);
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
        return PickSimpleDestinations(state, count, null);
    }

    private static List<string> PickSimpleDestinations(
        GameState state,
        int count,
        List<string> previousDestinationIds
    )
    {
        string sectorId = state != null
            ? state.dimension1SelectedSectorId
            : "";

        List<string> pool = new List<string>(
            GetDimension1SectorDestinationIds(sectorId)
        );

        List<string> validPool = new List<string>();

        foreach (string destinationId in pool)
        {
            if (CanDestinationAppearInScan(state, destinationId))
            {
                validPool.Add(destinationId);
            }
        }

        List<string> selected = new List<string>();
        float repetitionReduction = GetD1ScanRepetitionReduction(state);

        while (selected.Count < count && validPool.Count > 0)
        {
            int index = PickDestinationIndexWithScanMemory(
                sectorId,
                validPool,
                previousDestinationIds,
                repetitionReduction
            );

            selected.Add(validPool[index]);
            validPool.RemoveAt(index);
        }

        return selected;
    }

    private static int PickDestinationIndexWithScanMemory(
        string sectorId,
        List<string> validPool,
        List<string> previousDestinationIds,
        float repetitionReduction
    )
    {
        if (validPool == null || validPool.Count == 0)
            return 0;

        float totalWeight = 0.0f;

        for (int i = 0; i < validPool.Count; i++)
        {
            float sectorWeight = GetDimension1SectorDestinationWeight(
                sectorId,
                validPool[i]
            );
            float memoryWeight = GetScanMemoryDestinationWeight(
                validPool[i],
                previousDestinationIds,
                repetitionReduction
            );

            totalWeight += sectorWeight * memoryWeight;
        }

        if (totalWeight <= 0.0f)
            return Random.Range(0, validPool.Count);

        float roll = Random.value * totalWeight;
        float accumulated = 0.0f;

        for (int i = 0; i < validPool.Count; i++)
        {
            float sectorWeight = GetDimension1SectorDestinationWeight(
                sectorId,
                validPool[i]
            );
            float memoryWeight = GetScanMemoryDestinationWeight(
                validPool[i],
                previousDestinationIds,
                repetitionReduction
            );

            accumulated += sectorWeight * memoryWeight;

            if (roll <= accumulated)
                return i;
        }

        return validPool.Count - 1;
    }

    private static float GetD1ScanRepetitionReduction(GameState state)
    {
        float reduction = 0.0f;

        reduction += GetD1TreeScanMemoryRepetitionReduction(state);
        reduction += GetLostNavigationRecordRepetitionReduction(state);

        return Mathf.Clamp(reduction, 0.0f, 0.75f);
    }

    private static float GetScanMemoryDestinationWeight(
        string destinationId,
        List<string> previousDestinationIds,
        float repetitionReduction
    )
    {
        if (previousDestinationIds != null &&
            previousDestinationIds.Contains(destinationId))
        {
            return Mathf.Max(0.05f, 1.0f - repetitionReduction);
        }

        return 1.0f;
    }

    private static List<string> ApplyRareSpecialDestinationRoll(
    GameState state,
    List<string> destinations,
    List<string> previousDestinationIds
)
    {
        if (destinations == null)
            return new List<string>();

        float chance = GetD1RareSpecialDestinationChance(state);

        if (chance <= 0.0f)
            return destinations;

        if (Random.value > chance)
            return destinations;

        List<string> candidates = GetAdvancedCartographyCandidateDestinations(
            state,
            destinations
        );

        if (candidates.Count == 0)
            return destinations;

        string selectedRareSpecialDestination = candidates[Random.Range(0, candidates.Count)];

        if (string.IsNullOrEmpty(selectedRareSpecialDestination))
            return destinations;

        if (destinations.Count == 0)
        {
            destinations.Add(selectedRareSpecialDestination);
            return destinations;
        }

        int replaceIndex = PickAdvancedCartographyReplaceIndex(
            destinations,
            previousDestinationIds
        );

        destinations[replaceIndex] = selectedRareSpecialDestination;

        return destinations;
    }

    private static List<string> GetAdvancedCartographyCandidateDestinations(
        GameState state,
        List<string> currentDestinations
    )
    {
        List<string> candidates = new List<string>();

        AddAdvancedCartographyCandidate(
            state,
            currentDestinations,
            candidates,
            DestinationOrbitalRuin
        );

        AddAdvancedCartographyCandidate(
            state,
            currentDestinations,
            candidates,
            DestinationLaboratory
        );

        AddAdvancedCartographyCandidate(
            state,
            currentDestinations,
            candidates,
            DestinationMinorAnomaly
        );

        AddAdvancedCartographyCandidate(
            state,
            currentDestinations,
            candidates,
            DestinationAncientStructure
        );

        AddAdvancedCartographyCandidate(
            state,
            currentDestinations,
            candidates,
            DestinationUnstableZone
        );

        return candidates;
    }

    private static void AddAdvancedCartographyCandidate(
        GameState state,
        List<string> currentDestinations,
        List<string> candidates,
        string destinationId
    )
    {
        if (string.IsNullOrEmpty(destinationId))
            return;

        if (currentDestinations != null && currentDestinations.Contains(destinationId))
            return;

        if (!CanDestinationAppearInScan(state, destinationId))
            return;

        candidates.Add(destinationId);
    }

    private static int PickAdvancedCartographyReplaceIndex(
        List<string> destinations,
        List<string> previousDestinationIds
    )
    {
        if (destinations == null || destinations.Count == 0)
            return 0;

        for (int i = 0; i < destinations.Count; i++)
        {
            string destinationId = destinations[i];

            if (previousDestinationIds != null &&
                previousDestinationIds.Contains(destinationId))
            {
                return i;
            }
        }

        return destinations.Count - 1;
    }

    private static bool CanDestinationAppearInScan(GameState state, string destinationId)
    {
        if (state == null)
            return false;

        return
            IsDestinationInDimension1Sector(
                destinationId,
                state.dimension1SelectedSectorId
            ) &&
            !IsDestinationCurrentlyExplored(
                state,
                destinationId,
                state.dimension1SelectedSectorId
            ) &&
            HasDestinationAccessMetalUnlocked(state, destinationId) &&
            DestinationHasUnlockedMetalReward(state, destinationId);
    }

    private static bool IsDestinationCurrentlyExplored(
        GameState state,
        string destinationId,
        string sectorId
    )
    {
        if (state == null ||
            state.dimension1Ships == null ||
            string.IsNullOrEmpty(destinationId))
        {
            return false;
        }

        foreach (D1ShipState ship in state.dimension1Ships)
        {
            if (ship == null || !ship.explorationActive)
                continue;

            if (ship.activeDestinationId == destinationId &&
                ship.activeSectorId == sectorId)
            {
                return true;
            }
        }

        return false;
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

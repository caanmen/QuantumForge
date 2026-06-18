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

public static class Dimension1System
{
    public const string DimensionId = "dimension_01";

    // Metales MVP
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

    // Planetas MVP
    public const string Planet01 = "planet_01";
    public const string Planet02 = "planet_02";
    public const string Planet03 = "planet_03";
    public const string Planet04 = "planet_04";
    public const string Planet05 = "planet_05";
    public const string Planet06 = "planet_06";
    public const string Planet07 = "planet_07";

    // Nave MVP
    public const string ShipLightProbe = "ship_light_probe";
    public const string ShipExtractorDrone = "ship_extractor_drone";
    public const string ShipAnalyticProbe = "ship_analytic_probe";
    public const string ShipCargoShip = "ship_cargo_ship";
    public const string ShipRescueShip = "ship_rescue_ship";
    public const string ShipConvergenceShip = "ship_convergence_ship";

    // ID antiguo usado solo para migrar partidas viejas.
    public const string LegacyCargoShipId = "ship_survey_corvette";

    // Partes de nave MVP
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

    // Barrido temporal del escáner para pruebas MVP.
    public const double SimpleScanDurationSeconds = 5.0;
    public const int SimpleScanBaseDestinationCount = 2;
    public const int SimpleScanMaxDestinationCount = 5;
    public const int SimpleScannerMaxLevel = 3;
    public const int BlueprintFragmentsPerBlueprint = 10;

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
        switch (planet.planetId)
        {
            case Planet01:
                ProducePlanet01(state, planet.extractorTier, dt);
                break;

            case Planet02:
                ProducePlanet02(state, planet.extractorTier, dt);
                break;

            case Planet03:
                ProducePlanet03(state, planet.extractorTier, dt);
                break;

            case Planet04:
                ProducePlanet04(state, planet.extractorTier, dt);
                break;

            case Planet05:
                ProducePlanet05(state, planet.extractorTier, dt);
                break;

            case Planet06:
                ProducePlanet06(state, planet.extractorTier, dt);
                break;

            case Planet07:
                ProducePlanet07(state, planet.extractorTier, dt);
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
        if (productionPerSecondPerTier <= 0.0)
            return;

        if (tier <= 0)
            return;

        double amount = productionPerSecondPerTier * tier * dt;

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

        double cost = GetExtractorUpgradeCost(planet);
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

        if (planetId == Planet02)
        {
            return state.GetD1MetalAmount(MetalIron) >= 500.0 &&
                state.GetD1MetalAmount(MetalCopper) >= 120.0;
        }

        if (planetId == Planet03)
        {
            return state.GetD1MetalAmount(MetalAluminum) >= 1500.0 &&
                state.GetD1MetalAmount(MetalTitanium) >= 300.0;
        }

        if (planetId == Planet04)
        {
            return state.GetD1MetalAmount(MetalNickel) >= 1200.0 &&
                state.GetD1MetalAmount(MetalCobalt) >= 350.0;
        }

        if (planetId == Planet05)
        {
            return state.GetD1MetalAmount(MetalLithium) >= 1400.0 &&
                state.GetD1MetalAmount(MetalTungsten) >= 320.0;
        }

        if (planetId == Planet06)
        {
            return state.GetD1MetalAmount(MetalPlatinum) >= 1100.0 &&
                state.GetD1MetalAmount(MetalNickel) >= 1800.0;
        }

        if (planetId == Planet07)
        {
            return state.GetD1MetalAmount(MetalIridium) >= 800.0 &&
                state.GetD1MetalAmount(MetalCobalt) >= 1500.0 &&
                state.GetD1MetalAmount(MetalTungsten) >= 900.0;
        }

        return false;
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

        double cost = GetExtractorUpgradeCost(planet);
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

        if (planet.unlocked)
            return true;

        if (planetId == Planet02)
        {
            // Costos provisionales para probar el MVP.
            double ironCost = 500.0;
            double copperCost = 120.0;

            if (state.GetD1MetalAmount(MetalIron) < ironCost)
                return false;

            if (state.GetD1MetalAmount(MetalCopper) < copperCost)
                return false;

            state.SpendD1Metal(MetalIron, ironCost);
            state.SpendD1Metal(MetalCopper, copperCost);

            planet.unlocked = true;
            planet.extractorTier = 1;

            return true;
        }

        if (planetId == Planet03)
        {
            // Costos provisionales para probar el MVP.
            // La idea es que Planeta 3 requiera haber avanzado en Planeta 2
            // y haber desbloqueado Titanio.
            double aluminumCost = 1500.0;
            double titaniumCost = 300.0;

            if (state.GetD1MetalAmount(MetalAluminum) < aluminumCost)
                return false;

            if (state.GetD1MetalAmount(MetalTitanium) < titaniumCost)
                return false;

            state.SpendD1Metal(MetalAluminum, aluminumCost);
            state.SpendD1Metal(MetalTitanium, titaniumCost);

            planet.unlocked = true;
            planet.extractorTier = 1;

            return true;
        }

        if (planetId == Planet04)
    {
        double nickelCost = 1200.0;
        double cobaltCost = 350.0;

        if (state.GetD1MetalAmount(MetalNickel) < nickelCost)
            return false;

        if (state.GetD1MetalAmount(MetalCobalt) < cobaltCost)
            return false;

        state.SpendD1Metal(MetalNickel, nickelCost);
        state.SpendD1Metal(MetalCobalt, cobaltCost);

        planet.unlocked = true;
        planet.extractorTier = 1;

        return true;
    }

    if (planetId == Planet05)
    {
        double lithiumCost = 1400.0;
        double tungstenCost = 320.0;

        if (state.GetD1MetalAmount(MetalLithium) < lithiumCost)
            return false;

        if (state.GetD1MetalAmount(MetalTungsten) < tungstenCost)
            return false;

        state.SpendD1Metal(MetalLithium, lithiumCost);
        state.SpendD1Metal(MetalTungsten, tungstenCost);

        planet.unlocked = true;
        planet.extractorTier = 1;

        return true;
    }

    if (planetId == Planet06)
    {
        double platinumCost = 1100.0;
        double nickelCost = 1800.0;

        if (state.GetD1MetalAmount(MetalPlatinum) < platinumCost)
            return false;

        if (state.GetD1MetalAmount(MetalNickel) < nickelCost)
            return false;

        state.SpendD1Metal(MetalPlatinum, platinumCost);
        state.SpendD1Metal(MetalNickel, nickelCost);

        planet.unlocked = true;
        planet.extractorTier = 1;

        return true;
    }

    if (planetId == Planet07)
    {
        double iridiumCost = 800.0;
        double cobaltCost = 1500.0;
        double tungstenCost = 900.0;

        if (state.GetD1MetalAmount(MetalIridium) < iridiumCost)
            return false;

        if (state.GetD1MetalAmount(MetalCobalt) < cobaltCost)
            return false;

        if (state.GetD1MetalAmount(MetalTungsten) < tungstenCost)
            return false;

        state.SpendD1Metal(MetalIridium, iridiumCost);
        state.SpendD1Metal(MetalCobalt, cobaltCost);
        state.SpendD1Metal(MetalTungsten, tungstenCost);

        planet.unlocked = true;
        planet.extractorTier = 1;

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

        if (shipId == ShipExtractorDrone)
        {
            return state.GetD1MetalAmount(MetalIron) >= 300.0 &&
                state.GetD1MetalAmount(MetalCopper) >= 80.0;
        }

        if (shipId == ShipAnalyticProbe)
        {
            return state.GetD1MetalAmount(MetalCopper) >= 250.0 &&
                state.GetD1MetalAmount(MetalAluminum) >= 120.0;
        }

        if (shipId == ShipCargoShip)
        {
            return state.GetD1MetalAmount(MetalTitanium) >= 800.0 &&
                state.GetD1MetalAmount(MetalNickel) >= 450.0 &&
                CanSpendCompletedBlueprints(state, 3);
        }

        if (shipId == ShipRescueShip)
        {
            return state.GetD1MetalAmount(MetalTitanium) >= 1200.0 &&
                state.GetD1MetalAmount(MetalNickel) >= 900.0 &&
                state.GetD1MetalAmount(MetalCobalt) >= 500.0 &&
                state.GetD1MetalAmount(MetalPlatinum) >= 250.0 &&
                CanSpendCompletedBlueprints(state, 4);
        }

        if (shipId == ShipConvergenceShip)
        {
            return state.GetD1MetalAmount(MetalPlatinum) >= 900.0 &&
                state.GetD1MetalAmount(MetalIridium) >= 550.0 &&
                state.GetD1MetalAmount(MetalCobalt) >= 900.0 &&
                state.GetD1MetalAmount(MetalTungsten) >= 1200.0 &&
                CanSpendCompletedBlueprints(state, 4);
        }

        return false;
    }

    public static bool TryUnlockShip(GameState state, string shipId)
    {
        if (!CanUnlockShip(state, shipId))
            return false;

        D1ShipState ship = FindShipState(state, shipId);

        if (ship == null)
            return false;

        if (shipId == ShipExtractorDrone)
        {
            if (!state.SpendD1Metal(MetalIron, 300.0))
                return false;

            if (!state.SpendD1Metal(MetalCopper, 80.0))
                return false;

            ship.unlocked = true;
            return true;
        }

        if (shipId == ShipAnalyticProbe)
        {
            if (!state.SpendD1Metal(MetalCopper, 250.0))
                return false;

            if (!state.SpendD1Metal(MetalAluminum, 120.0))
                return false;

            ship.unlocked = true;
            return true;
        }

        if (shipId == ShipCargoShip)
        {
            if (!state.SpendD1Metal(MetalTitanium, 800.0))
                return false;

            if (!state.SpendD1Metal(MetalNickel, 450.0))
                return false;

            if (!TrySpendCompletedBlueprints(state, 3))
                return false;

            ship.unlocked = true;
            return true;
        }

        if (shipId == ShipRescueShip)
        {
            if (!state.SpendD1Metal(MetalTitanium, 1200.0))
                return false;

            if (!state.SpendD1Metal(MetalNickel, 900.0))
                return false;

            if (!state.SpendD1Metal(MetalCobalt, 500.0))
                return false;

            if (!state.SpendD1Metal(MetalPlatinum, 250.0))
                return false;

            if (!TrySpendCompletedBlueprints(state, 4))
                return false;

            ship.unlocked = true;
            return true;
        }

                if (shipId == ShipConvergenceShip)
        {
            if (!state.SpendD1Metal(MetalPlatinum, 900.0))
                return false;

            if (!state.SpendD1Metal(MetalIridium, 550.0))
                return false;

            if (!state.SpendD1Metal(MetalCobalt, 900.0))
                return false;

            if (!state.SpendD1Metal(MetalTungsten, 1200.0))
                return false;

            if (!TrySpendCompletedBlueprints(state, 4))
                return false;

            ship.unlocked = true;
            return true;
        }

        return false;
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

        if (IsAnyShipExploring(state))
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

        double duration = GetSimpleExplorationDurationSeconds(destination.destinationId, ship);

        ship.explorationActive = true;
        ship.activeDestinationId = destination.destinationId;
        ship.explorationTotalSeconds = duration;
        ship.explorationRemainingSeconds = duration;

        destination.available = false;
        state.dimension1ScannedDestinations.Clear();

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
        string shipId = ship.shipId;

        if (!string.IsNullOrEmpty(destinationId))
        {
            GrantSimpleExplorationRewards(state, destinationId, ship);
        }

        ship.explorationActive = false;
        ship.activeDestinationId = "";
        ship.explorationRemainingSeconds = 0.0;
        ship.explorationTotalSeconds = 0.0;
    }

    private static void GrantSimpleExplorationRewards(GameState state, string destinationId, D1ShipState ship)
    {
        if (state == null)
            return;

        List<D1MetalAmount> rewards = new List<D1MetalAmount>();
        double materialMultiplier = GetShipMaterialRewardMultiplier(ship);

        state.dimension1LastExplorationBlueprintFragments = 0;

        switch (destinationId)
        {
            case DestinationMineralBelt:
                AddExplorationTimedReward(state, rewards, MetalIron, materialMultiplier);
                AddExplorationTimedReward(state, rewards, MetalCopper, materialMultiplier);
                AddExplorationTimedReward(state, rewards, MetalAluminum, materialMultiplier);
                break;

            case DestinationShipGraveyard:
                AddExplorationTimedReward(state, rewards, MetalIron, materialMultiplier);
                AddExplorationTimedReward(state, rewards, MetalTitanium, materialMultiplier);
                AddExplorationTimedReward(state, rewards, MetalNickel, materialMultiplier);
                break;

            case DestinationAbandonedProbe:
            case DestinationAbandonedShip:
                AddExplorationTimedReward(state, rewards, MetalCopper, materialMultiplier);
                AddExplorationTimedReward(state, rewards, MetalTitanium, materialMultiplier);
                AddExplorationTimedReward(state, rewards, MetalNickel, materialMultiplier);
                break;

            case DestinationOrbitalRuin:
                AddExplorationTimedReward(state, rewards, MetalCopper, materialMultiplier);
                AddExplorationTimedReward(state, rewards, MetalAluminum, materialMultiplier);
                AddExplorationTimedReward(state, rewards, MetalLithium, materialMultiplier);
                break;

            case DestinationDriftingProbes:
                AddExplorationTimedReward(state, rewards, MetalCopper, materialMultiplier);
                AddExplorationTimedReward(state, rewards, MetalAluminum, materialMultiplier);
                AddExplorationTimedReward(state, rewards, MetalLithium, materialMultiplier);
                break;

            case DestinationLaboratory:
                AddExplorationTimedReward(state, rewards, MetalAluminum, materialMultiplier);
                AddExplorationTimedReward(state, rewards, MetalTitanium, materialMultiplier);
                AddExplorationTimedReward(state, rewards, MetalLithium, materialMultiplier);
                AddExplorationTimedReward(state, rewards, MetalCobalt, materialMultiplier);
                break;

            case DestinationAbandonedStation:
                AddExplorationTimedReward(state, rewards, MetalTitanium, materialMultiplier);
                AddExplorationTimedReward(state, rewards, MetalNickel, materialMultiplier);
                AddExplorationTimedReward(state, rewards, MetalCobalt, materialMultiplier);
                AddExplorationTimedReward(state, rewards, MetalPlatinum, materialMultiplier);
                break;

            case DestinationMinorAnomaly:
                AddExplorationTimedReward(state, rewards, MetalLithium, materialMultiplier);
                AddExplorationTimedReward(state, rewards, MetalTungsten, materialMultiplier);
                AddExplorationTimedReward(state, rewards, MetalCobalt, materialMultiplier);
                AddExplorationTimedReward(state, rewards, MetalPlatinum, materialMultiplier);
                break;

            case DestinationAncientStructure:
                AddExplorationTimedReward(state, rewards, MetalTungsten, materialMultiplier);
                AddExplorationTimedReward(state, rewards, MetalPlatinum, materialMultiplier);
                AddExplorationTimedReward(state, rewards, MetalIridium, materialMultiplier);
                break;

            case DestinationUnstableZone:
                AddExplorationTimedReward(state, rewards, MetalTungsten, materialMultiplier);
                AddExplorationTimedReward(state, rewards, MetalPlatinum, materialMultiplier);
                AddExplorationTimedReward(state, rewards, MetalIridium, materialMultiplier);
                break;
        }

        int blueprintFragments = RollSimpleBlueprintFragments(destinationId, ship);

        if (blueprintFragments > 0)
        {
            state.dimension1BlueprintFragments += blueprintFragments;
            state.dimension1LastExplorationBlueprintFragments = blueprintFragments;
        }

        state.dimension1LastExplorationDestinationId = destinationId;
        state.dimension1LastExplorationRewards = rewards;
        state.dimension1LastExplorationResultId += 1;
    }

    public static float GetSimpleBlueprintFragmentChance(string destinationId, D1ShipState ship)
    {
        float chance = GetBaseSimpleBlueprintFragmentChance(destinationId);

        if (ship != null && ship.shipId == ShipAnalyticProbe)
            chance += GetAnalyticProbeBlueprintFragmentBonus(ship);

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

    private static int RollSimpleBlueprintFragments(string destinationId, D1ShipState ship)
    {
        float chance = GetSimpleBlueprintFragmentChance(destinationId, ship);

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

    private static float GetAnalyticProbeBlueprintFragmentBonus(D1ShipState ship)
    {
        if (ship == null)
            return 0.0f;

        if (ship.sensorsLevel >= 6)
            return 0.10f;

        if (ship.sensorsLevel >= 5)
            return 0.09f;

        if (ship.sensorsLevel >= 4)
            return 0.07f;

        if (ship.sensorsLevel >= 3)
            return 0.06f;

        if (ship.sensorsLevel >= 2)
            return 0.04f;

        if (ship.sensorsLevel >= 1)
            return 0.03f;

        return 0.02f;
    }

    private static double GetShipMaterialRewardMultiplier(D1ShipState ship)
    {
        if (ship == null)
            return 1.0;

        if (ship.shipId == ShipExtractorDrone)
        {
            if (ship.cargoLevel >= 6)
                return 2.00;

            if (ship.cargoLevel >= 5)
                return 1.80;

            if (ship.cargoLevel >= 4)
                return 1.65;

            if (ship.cargoLevel >= 3)
                return 1.50;

            if (ship.cargoLevel >= 2)
                return 1.40;

            if (ship.cargoLevel >= 1)
                return 1.30;

            return 1.20;
        }

        if (ship.shipId == ShipCargoShip)
        {
            if (ship.cargoLevel >= 6)
                return 1.75;

            if (ship.cargoLevel >= 5)
                return 1.60;

            if (ship.cargoLevel >= 4)
                return 1.47;

            if (ship.cargoLevel >= 3)
                return 1.35;

            if (ship.cargoLevel >= 2)
                return 1.26;

            if (ship.cargoLevel >= 1)
                return 1.18;

            return 1.10;
        }

        return 1.0;
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

        double productionPerSecond = GetMetalProductionPerSecond(state, metalId);

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
        double baseDuration = GetSimpleExplorationBaseDurationSeconds(destinationId);

        if (ship == null)
            return baseDuration;

        if (ship.shipId == ShipLightProbe)
            return baseDuration * GetSpeedMultiplierByLevel(ship.speedLevel);

        if (ship.shipId == ShipRescueShip && IsRescueSpeedCompatibleDestination(destinationId))
            return baseDuration * GetSpeedMultiplierByLevel(ship.speedLevel);

        if (ship.shipId == ShipConvergenceShip && IsConvergenceSpeedCompatibleDestination(destinationId))
            return baseDuration * GetSpeedMultiplierByLevel(ship.speedLevel);

        return baseDuration;
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
        switch (destinationId)
        {
            case DestinationMineralBelt:
                return
                    IsMetalUnlockedForDimension1(state, MetalIron) ||
                    IsMetalUnlockedForDimension1(state, MetalCopper) ||
                    IsMetalUnlockedForDimension1(state, MetalAluminum);

            case DestinationShipGraveyard:
                return
                    IsMetalUnlockedForDimension1(state, MetalIron) ||
                    IsMetalUnlockedForDimension1(state, MetalTitanium) ||
                    IsMetalUnlockedForDimension1(state, MetalNickel);

            case DestinationAbandonedProbe:
            case DestinationAbandonedShip:
                return
                    IsMetalUnlockedForDimension1(state, MetalCopper) ||
                    IsMetalUnlockedForDimension1(state, MetalTitanium) ||
                    IsMetalUnlockedForDimension1(state, MetalNickel);

            case DestinationOrbitalRuin:
                return
                    IsMetalUnlockedForDimension1(state, MetalCopper) ||
                    IsMetalUnlockedForDimension1(state, MetalAluminum) ||
                    IsMetalUnlockedForDimension1(state, MetalLithium);

            case DestinationDriftingProbes:
                return
                    IsMetalUnlockedForDimension1(state, MetalCopper) ||
                    IsMetalUnlockedForDimension1(state, MetalAluminum) ||
                    IsMetalUnlockedForDimension1(state, MetalLithium);

            case DestinationLaboratory:
                return
                    IsMetalUnlockedForDimension1(state, MetalAluminum) ||
                    IsMetalUnlockedForDimension1(state, MetalTitanium) ||
                    IsMetalUnlockedForDimension1(state, MetalLithium) ||
                    IsMetalUnlockedForDimension1(state, MetalCobalt);

            case DestinationAbandonedStation:
                return
                    IsMetalUnlockedForDimension1(state, MetalTitanium) ||
                    IsMetalUnlockedForDimension1(state, MetalNickel) ||
                    IsMetalUnlockedForDimension1(state, MetalCobalt) ||
                    IsMetalUnlockedForDimension1(state, MetalPlatinum);

            case DestinationMinorAnomaly:
                return
                    IsMetalUnlockedForDimension1(state, MetalLithium) ||
                    IsMetalUnlockedForDimension1(state, MetalTungsten) ||
                    IsMetalUnlockedForDimension1(state, MetalCobalt) ||
                    IsMetalUnlockedForDimension1(state, MetalPlatinum);

            case DestinationAncientStructure:
                return
                    IsMetalUnlockedForDimension1(state, MetalTungsten) ||
                    IsMetalUnlockedForDimension1(state, MetalPlatinum) ||
                    IsMetalUnlockedForDimension1(state, MetalIridium);

            case DestinationUnstableZone:
                return
                    IsMetalUnlockedForDimension1(state, MetalTungsten) ||
                    IsMetalUnlockedForDimension1(state, MetalPlatinum) ||
                    IsMetalUnlockedForDimension1(state, MetalIridium);

            default:
                return false;
        }
    }
}

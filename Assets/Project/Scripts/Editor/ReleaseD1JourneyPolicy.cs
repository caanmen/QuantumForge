#if UNITY_EDITOR
using System;
using System.Linq;
using D1 = Dimension1System;

/// <summary>
/// Mechanical D1 journey, not a UI test or a balance guarantee. The caller owns real
/// prestige entry, GameState initialization, normal production/progress credits and time.
/// In particular, Dimension1System.Tick alone does not credit D1 tree points or produce
/// the inherited LE/Traces needed by relics. Never use debug entry/resources as evidence.
/// Call Step repeatedly between clock ticks, with an external iteration/time budget.
/// True means an action succeeded; false means wait, blocked, or already complete.
/// Inspect dimension1GalacticAnchorDiscovered separately to determine completion.
/// No persistent policy state: separate fixtures can reuse this class safely.
/// </summary>
public static class ReleaseD1JourneyPolicy
{
    // Eight distinct nodes, Reading III and Coordination: the canonical 15-point route.
    private static readonly string[] AdditionalTreeNodes =
    {
        D1.D1TreeExplorationDestinationReading, D1.D1TreeFleetHangarPreparation,
        D1.D1TreeRecoveryCopyRegistry, D1.D1TreeExplorationScanMemory,
        D1.D1TreeExplorationHiddenFindTracking, D1.D1TreeFleetSupportFormation
    };

    private static readonly string[] Parts =
    {
        D1.ShipPartSensors, D1.ShipPartSpeed, D1.ShipPartCargo, D1.ShipPartArmor
    };

    public static bool Step(GameState gs, Action<string> log)
    {
        if (gs == null || !gs.dimension01Unlocked || gs.dimension1GalacticAnchorDiscovered ||
            gs.dimension1ArkFinalMissionActive)
            return false;

        // Entry/normalization belongs to the caller, not this action policy.
        if (gs.dimension1Planets == null || gs.dimension1Ships == null ||
            gs.dimension1Relics == null || gs.dimension1Sectors == null ||
            gs.dimension1ScannedDestinations == null)
            return false;

        int frontier = 0;
        for (int i = 1; i < D1.Dimension1SectorIds.Length; i++)
            if (gs.IsD1SectorUnlocked(D1.Dimension1SectorIds[i])) frontier = i;

        if (frontier == 4 && !gs.dimension1ArkInvestigated && D1.TryInvestigateD1Ark(gs))
            return Done(log, "Investigated Ark");

        // Synchronies must overlap. Drain the entire required fleet before starting any,
        // then start all four in this call, regardless of the caller's tick interval.
        if (gs.dimension1ArkInvestigated && !gs.dimension1CentralAccessKeyObtained)
        {
            bool fleetOwned = D1.Dimension1ActiveShipIds.All(id =>
                gs.dimension1Ships.Any(s => s != null && s.shipId == id && s.unlocked));
            if (fleetOwned)
            {
                if (gs.dimension1Ships.Any(s => s != null &&
                    (s.explorationActive || s.coordinatedSupportReserved))) return false;
                if (D1.D1CentralSyncMissionIds.Any(id =>
                    { var m = D1.GetD1CentralSyncMission(gs, id); return m != null && m.active; }))
                    return false;
                // Validate the entire batch before mutating it.
                if (!D1.D1CentralSyncMissionIds.All(id =>
                    D1.CanStartD1CentralSyncMission(gs, id, out _))) return false;
                bool started = false;
                foreach (string id in D1.D1CentralSyncMissionIds)
                    if (D1.TryStartD1CentralSyncMission(gs, id, out _))
                        started |= Done(log, "Started central mission " + id);
                return started;
            }
        }

        if (gs.dimension1ArkInvestigated && gs.dimension1CentralAccessKeyObtained &&
            D1.GetD1ArkRequirements(gs).All(r => r.requirementId == "ships_available_2" || r.met))
        {
            // Stop dispatching so at least two ships become available naturally.
            if (D1.TryStartD1ArkFinalMission(gs, out _)) return Done(log, "Started Ark final mission");
            return false;
        }

        // Infrastructure is finite: unlock every accessible planet and stop extractors
        // at 10 (the secondary-metal threshold). No unbounded reinvestment loop.
        foreach (string id in D1.StarterPlanets)
        {
            D1PlanetState p = gs.dimension1Planets.FirstOrDefault(x => x != null && x.planetId == id);
            string sector = D1.GetDimension1PlanetSectorId(id);
            if (p == null || !gs.IsD1SectorUnlocked(sector)) continue;
            bool affordable;
            if (!p.unlocked)
                affordable = D1.TryGetPlanetUnlockCost(id, out string m1, out double a1,
                    out string m2, out double a2, out string m3, out double a3) &&
                    Has(gs, m1, a1) && Has(gs, m2, a2) && Has(gs, m3, a3);
            else
                affordable = p.extractorTier < 10 && Has(gs,
                    D1.GetExtractorUpgradeMainCostMetal(p), D1.GetExtractorUpgradeCost(gs, p));
            if (!affordable) continue;
            if (Select(gs, sector, log)) return true;
            if (gs.dimension1SelectedSectorId != sector) continue;
            if (!p.unlocked && D1.TryUnlockPlanet(gs, id)) return Done(log, "Unlocked planet " + id);
            if (p.unlocked && p.extractorTier < 10 && D1.TryUpgradeExtractor(gs, id))
                return Done(log, "Extractor " + id + " -> " + p.extractorTier);
        }

        foreach (string id in D1.Dimension1ActiveShipIds)
        {
            // Keep the cargo matrix/adaptive fragment until the Sector 3 gate is passed.
            if (id == D1.ShipCargoShip && frontier < 2) continue;
            if (D1.TryUnlockShip(gs, id)) return Done(log, "Unlocked ship " + id);
        }

        int scannerTarget = frontier == 0 ? 2 : frontier == 1 ? 5 : frontier == 2 ? 10 : 15;
        bool scannerReady = D1.GetSimpleScannerLevel(gs) < scannerTarget && ScannerAffordable(gs);
        if (scannerReady && D1.TryUpgradeSimpleScanner(gs))
            return Done(log, "Scanner -> " + D1.GetSimpleScannerLevel(gs));

        if (D1.TryBuyDimension1TreeNode(gs, D1.D1TreeRelicReading))
            return Done(log, "Relic Reading -> " + gs.GetD1TreeNodeTier(D1.D1TreeRelicReading));
        if (!gs.IsD1TreeNodeUnlocked(D1.D1TreeFleetCoordination) &&
            D1.TryBuyDimension1TreeNode(gs, D1.D1TreeFleetCoordination))
            return Done(log, "Bought Fleet Coordination");

        int reserve = gs.IsD1TreeNodeUnlocked(D1.D1TreeFleetCoordination) ? 0 :
            D1.GetDimension1TreeNodeCost(D1.D1TreeFleetCoordination, 1);
        for (int tier = gs.GetD1TreeNodeTier(D1.D1TreeRelicReading) + 1; tier <= 3; tier++)
            reserve += D1.GetDimension1TreeNodeCost(D1.D1TreeRelicReading, tier);
        foreach (string id in AdditionalTreeNodes)
            if (!gs.IsD1TreeNodeUnlocked(id) && gs.d1TreePoints >= reserve + D1.GetDimension1TreeNodeCost(id, 1) &&
                D1.TryBuyDimension1TreeNode(gs, id)) return Done(log, "Bought tree node " + id);

        // Two Tier-1 relics at 25 in each of S1-S3 also open those sectors' Tier-3
        // pools. One of these reaches 50 for Ark; never max every discovered relic.
        foreach (string id in D1.Dimension1RelicIds)
        {
            if (D1.GetDimension1RelicTier(id) != 1 ||
                D1.GetDimension1RelicSectorId(id) == D1.Sector04SilentFrontier) continue;
            int target = frontier == 4 && id == D1.RelicDriftCompass ? 50 : 25;
            if (gs.IsD1RelicUnlocked(id) && gs.GetD1RelicLevel(id) < target &&
                D1.TryUpgradeDimension1Relic(gs, id))
                return Done(log, "Relic " + id + " -> " + gs.GetD1RelicLevel(id));
        }

        int upgrades = gs.dimension1Ships.Where(s => s != null && s.unlocked &&
            D1.IsShipActiveInDimension1Base(s.shipId)).Sum(s =>
                s.cargoLevel + s.speedLevel + s.armorLevel + s.sensorsLevel);
        int upgradeTarget = frontier < 2 ? 0 : frontier == 2 ? 5 : frontier == 3 ? 7 : 10;
        if (upgrades < upgradeTarget)
            foreach (string part in Parts)
                foreach (string ship in D1.Dimension1ActiveShipIds)
                    if (D1.TryGetNextShipPartUpgradeCost(gs, ship, part, out int level,
                        out _, out _, out _, out _) && level <= 3 &&
                        D1.TryUpgradeShipPart(gs, ship, part))
                        return Done(log, "Ship " + ship + " " + part + " -> " + level);

        // Upgrading the scanner forbids any active exploration. Once affordable,
        // let existing work finish instead of perpetually keeping one ship busy.
        if (scannerReady) return false;

        string targetSector = D1.Dimension1SectorIds[Math.Min(frontier, 3)];
        string wantedRelic = MissingRelic(gs);
        bool needCargoMatrices = frontier >= 1 &&
            !gs.dimension1Ships.Any(s => s != null && s.shipId == D1.ShipCargoShip && s.unlocked) &&
            !D1.CanCoverRequiredShipMatrices(gs, D1.ShipCargoShip);
        if (wantedRelic == null && needCargoMatrices) targetSector = D1.Sector02DebrisRing;
        if (wantedRelic != null) targetSector = D1.GetDimension1RelicSectorId(wantedRelic);
        if (Select(gs, targetSector, log)) return true;
        if (gs.dimension1SelectedSectorId != targetSector || gs.dimension1ScanActive) return false;

        var idle = gs.dimension1Ships.Where(s => s != null && s.unlocked &&
            D1.IsShipActiveInDimension1Base(s.shipId) && !D1.IsD1ShipBusy(s)).ToArray();
        bool coordinated = wantedRelic != null && D1.GetDimension1RelicTier(wantedRelic) == 3;
        // Leave a lone free ship idle while its partner finishes; simple missions
        // can never yield Tier 3 and would starve the coordinated route.
        if (idle.Length < (coordinated ? 2 : 1)) return false;

        // API indices count AVAILABLE entries, not positions in the backing list.
        var available = gs.dimension1ScannedDestinations.Where(d => d != null && d.available).ToArray();
        var indices = Enumerable.Range(0, available.Length).OrderByDescending(i =>
            wantedRelic != null && D1.IsDimension1RelicStrongDestination(wantedRelic, available[i].destinationId));
        foreach (int index in indices)
        {
            var d = available[index];
            if (wantedRelic == null && needCargoMatrices &&
                !D1.GetSpecificBlueprintPoolPreview(gs, d.destinationId).Any(id =>
                    (id == D1.BlueprintCargoFrame || id == D1.BlueprintCargoHold ||
                     id == D1.BlueprintCargoStabilizer) && gs.GetD1BlueprintAmount(id) == 0)) continue;
            if (wantedRelic != null && !(coordinated
                ? D1.IsDimension1RelicStrongDestination(wantedRelic, d.destinationId)
                : D1.IsDimension1RelicCompatibleDestination(wantedRelic, d.destinationId))) continue;
            foreach (var main in idle)
            {
                if (coordinated)
                {
                    foreach (var support in idle)
                        if (D1.TryStartCoordinatedExploration(gs, main.shipId, support.shipId, index))
                            return Done(log, "Coordinated exploration " + main.shipId + " + " +
                                support.shipId + " -> " + targetSector + "/" + d.destinationId);
                }
                else if (D1.TryStartExploration(gs, main.shipId, index))
                    return Done(log, "Exploration " + main.shipId + " -> " + targetSector + "/" + d.destinationId);
            }
        }
        if (D1.TryScanSimpleDestination(gs)) return Done(log, "Started scan " + targetSector);
        return false;
    }

    private static string MissingRelic(GameState gs)
    {
        int reading = gs.GetD1TreeNodeTier(D1.D1TreeRelicReading);
        // Collect lower tiers first, revisiting sectors once Reading II opens them.
        // S1-S3 supply the three distinct Tier-3 relics needed by Ark; no Convergence.
        for (int tier = 1; tier <= reading; tier++)
            foreach (string id in D1.Dimension1RelicIds)
                if (D1.GetDimension1RelicTier(id) == tier && !gs.IsD1RelicUnlocked(id) &&
                    gs.IsD1SectorUnlocked(D1.GetDimension1RelicSectorId(id)) &&
                    (tier != 3 || D1.GetDimension1RelicSectorId(id) != D1.Sector04SilentFrontier))
                    return id;
        return null;
    }

    private static bool ScannerAffordable(GameState gs)
    {
        return D1.TryGetNextSimpleScannerUpgradeCost(gs, out _, out string m1, out double a1,
            out string m2, out double a2, out string m3, out double a3, out string m4, out double a4) &&
            Has(gs, m1, a1) && Has(gs, m2, a2) && Has(gs, m3, a3) && Has(gs, m4, a4);
    }

    private static bool Has(GameState gs, string metal, double amount)
    {
        return string.IsNullOrEmpty(metal) || amount <= 0 || gs.GetD1MetalAmount(metal) >= amount;
    }

    private static bool Select(GameState gs, string sector, Action<string> log)
    {
        return gs.dimension1SelectedSectorId != sector && gs.TrySelectD1Sector(sector) &&
            Done(log, "Selected sector " + sector);
    }

    private static bool Done(Action<string> log, string action)
    {
        log?.Invoke("[D1 journey] " + action);
        return true;
    }
}
#endif

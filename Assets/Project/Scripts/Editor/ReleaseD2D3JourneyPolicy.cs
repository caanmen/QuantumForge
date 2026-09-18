#if UNITY_EDITOR
using System;
using System.Collections.Generic;

/// <summary>
/// Action-only journey policy. The coordinator owns entry, base production, clocks,
/// save isolation and Unity execution. No grants, forced unlocks, ticks or Convergence.
/// True means one accepted action, NOT dimension completion; false means wait/blocked
/// or already complete. Read the runtime completion flags separately.
/// This is a bounded milestone route, not coverage of every optional D2/D3 feature.
/// All inventory/state changes go through normal gameplay APIs. Fresh entry must
/// already have been initialized by the runtime. There is no process-static progress.
/// </summary>
public static class ReleaseD2D3JourneyPolicy
{
    public static bool StepD2(GameState gs, Action<string> log)
    {
        if (!Dimension2System.CanAccessDimension2(gs) || gs.dimension2 == null ||
            gs.dimension2.civilization1 == null || gs.dimension2.civilization2 == null)
            return false;
        var c1 = gs.dimension2.civilization1;
        var c2 = gs.dimension2.civilization2;
        if (c2.majorPactEstablished) return false;
        if (c2.entityContained)
            return Done(D2Civilization2System.TryEstablishMajorPact(gs), log,
                "D2: Major Pact established.");

        if (!gs.dimension2.civilization2Unlocked)
        {
            // One short, one medium, then long pilgrimages until trust 300.
            // No acolytes/pacts/upgrades are required by this milestone route.
            if (c1.trust >= D2PilgrimageSystem.Civilization2UnlockTrust) return false;
            string pilgrimage = c1.shortPilgrimagesCompleted == 0
                ? D2PilgrimageSystem.ShortId : c1.mediumPilgrimagesCompleted == 0
                ? D2PilgrimageSystem.MediumId : D2PilgrimageSystem.LongId;
            if (D2PilgrimageSystem.CanStart(gs, pilgrimage))
                return Done(D2PilgrimageSystem.TryStart(gs, pilgrimage), log,
                    "D2: started " + pilgrimage + "; trust=" + c1.trust);

            // Optional bounded labor assignment; always preserve six available
            // pilgrims. Base altar production works even before labor is available.
            foreach (string altarId in new[] { D2AltarSystem.WaxAltarId,
                D2AltarSystem.RitualBreadAltarId })
            {
                var altar = D2AltarSystem.GetAltar(c1, altarId);
                long missing = altar == null ? 0 : 10L - altar.followersAssigned;
                if (missing > 0 && c1.followersAvailable >= missing + 6L)
                    return Done(D2AltarSystem.TryAssignFollowers(gs, altarId, missing),
                        log, "D2: assigned " + missing + " followers to " + altarId);
            }
            return false;
        }

        // At zero total dominance the REAL random-roll API has probability 1.
        // Do not use the deterministic-roll overload. True from an attempt is not
        // proof of success; inspect entityContained and report the actual outcome.
        if (D2Civilization2System.GetTotalDominance(c2) <= 0.0 &&
            D2Civilization2System.CanAttemptContainment(c2))
        {
            bool accepted = D2Civilization2System.TryAttemptContainment(gs);
            if (accepted && c2.entityContained)
                log?.Invoke("D2: entity contained at zero dominance.");
            return accepted;
        }

        // First establish self-recruiting protected teams in every unlocked region.
        // Extra members improve recruitment/coverage, not the fixed dominance rate.
        string[] regions = { D2Civilization2System.Region1Id,
            D2Civilization2System.Region2Id, D2Civilization2System.Region3Id };
        foreach (string region in regions)
        {
            if (FillOperation(gs, region, D2Civilization2System.RescueOperationId, 5, log))
                return true;
            if (FillOperation(gs, region, D2Civilization2System.ProtectionOperationId, 5, log))
                return true;
        }
        foreach (string region in regions)
        {
            if (FillOperation(gs, region, D2Civilization2System.RescueOperationId, 10, log))
                return true;
            if (FillOperation(gs, region, D2Civilization2System.ProtectionOperationId, 10, log))
                return true;
            if (FillOperation(gs, region, D2Civilization2System.EspionageOperationId, 15, log))
                return true;
            if (FillOperation(gs, region, D2Civilization2System.SabotageOperationId, 25, log))
                return true;
        }
        return false;
    }

    private static bool FillOperation(GameState gs, string regionId, string operationId,
        long target, Action<string> log)
    {
        var c2 = gs.dimension2.civilization2;
        var region = D2Civilization2System.GetRegion(c2, regionId);
        if (region == null || !region.unlocked) return false;
        var operation = D2Civilization2System.GetOperation(region, operationId);
        if (operation == null || operation.membersAssigned >= target) return false;
        long need = target - operation.membersAssigned;
        long idle = D2Civilization2System.GetRegionIdleMembers(region);
        if (idle > 0)
            return Done(D2Civilization2System.TryAssignMembersToOperation(gs,
                regionId, operationId, Math.Min(need, idle)), log,
                "D2: staffed " + regionId + "/" + operationId);
        // Reserve complete increments, avoiding one-member actions every frame.
        if (c2.membersAvailable < need) return false;
        return Done(D2Civilization2System.TryAssignMembers(gs, regionId, need), log,
            "D2: moved " + need + " members to " + regionId);
    }

    public static bool StepD3(GameState gs, Action<string> log)
    {
        if (!Dimension3System.CanAccessDimension3(gs) || gs.dimension3 == null ||
            !gs.dimension3.initialized || gs.dimension3.autonomyCoreIntegrated)
            return false;
        var state = gs.dimension3;
        if (D3AutonomyCoreSystem.CanIntegrate(gs, out _))
            return Done(D3AutonomyCoreSystem.TryIntegrate(gs, out _), log,
                "D3: Autonomy Core integrated.");

        // Serialize jobs deliberately: no duplicate purchases, queue overshoot,
        // team/support double reservation, cancellation refunds or clock ownership.
        if (HasAnyJob(state)) return false;
        int bank = D3FacilitySystem.GetProcessBankLevel(state);
        if (bank < 1) return false; // Runtime must grant the canonical entry bank.

        // Use only the canonical starter MK1 as a permanent bank worker.
        var starter = D3FacilitySystem.GetAssignment(state,
            Dimension3Catalog.FacilityProcessBank, Dimension3Catalog.ChannelProcessPower,
            1, Dimension3Catalog.TraitNormal);
        if (starter == null && D3InventorySystem.GetAvailableAutomatonAmount(
            state, 1, Dimension3Catalog.TraitNormal) > 0)
            return Done(D3FacilitySystem.TrySetProcessBankAssignment(gs,
                Dimension3Catalog.ChannelProcessPower, 1, Dimension3Catalog.TraitNormal,
                1, out _), log, "D3: assigned starter MK1 to process power.");

        long mk1Target = Math.Max(Dimension3Catalog.RequiredMk1AssembliesForV2,
            Dimension3Catalog.GetProcessBankLevelDefinition(2).requiredAssemblyAmount);
        if (D3InventorySystem.GetAssemblyCount(state, 1) < mk1Target)
            return AssembleToward(gs, 1, mk1Target, false, log);
        if (bank < 2) return Upgrade(gs, Dimension3Catalog.FacilityProcessBank, log);

        long mk2Target = Math.Max(Dimension3Catalog.RequiredMk2AssembliesForV3,
            Dimension3Catalog.GetProcessBankLevelDefinition(3).requiredAssemblyAmount);
        if (D3InventorySystem.GetAssemblyCount(state, 2) < mk2Target)
            return AssembleToward(gs, 2, mk2Target, false, log);
        if (bank < 3) return Upgrade(gs, Dimension3Catalog.FacilityProcessBank, log);
        if (D3InventorySystem.GetAssemblyCount(state, 3) < 10)
            return AssembleToward(gs, 3, 10, false, log);
        if (bank < 4) return Upgrade(gs, Dimension3Catalog.FacilityProcessBank, log);
        if (!D3ResearchSystem.AreAllCompleted(state, 4)) return Research(gs, 4, log);
        if (D3InventorySystem.GetAssemblyCount(state, 4) < 5)
            return AssembleToward(gs, 4, 5, false, log);
        if (bank < 5) return Upgrade(gs, Dimension3Catalog.FacilityProcessBank, log);
        if (!D3ResearchSystem.AreAllCompleted(state, 5)) return Research(gs, 5, log);
        if (D3InventorySystem.GetAssemblyCount(state, 5) < 5)
            return AssembleToward(gs, 5, 5, false, log);

        foreach (string facility in new[] { Dimension3Catalog.FacilityProductionConsole,
            Dimension3Catalog.FacilityDiagnosticBank, Dimension3Catalog.FacilityExpeditionPort })
            if (D3FacilitySystem.GetFacilityLevel(state, facility) < 1)
                return Upgrade(gs, facility, log);
        if (D3FacilitySystem.GetFacilityLevel(state,
            Dimension3Catalog.FacilityAutomationCore) < 4)
            return Upgrade(gs, Dimension3Catalog.FacilityAutomationCore, log);
        if (!D3ResearchSystem.AreAllCompleted(state, 6)) return Research(gs, 6, log);

        // A real five-piece calibration produces the required trait, not a direct
        // inventory write or an injected pattern reading. One MK6 also gates core N5.
        if (D3InventorySystem.GetAutomatonAmount(state, 6,
            Dimension3Catalog.TraitCoordinator) < 1)
            return AssembleToward(gs, 6,
                D3InventorySystem.GetAssemblyCount(state, 6) + 1, true, log);
        if (D3FacilitySystem.GetFacilityLevel(state,
            Dimension3Catalog.FacilityAutomationCore) < 5)
            return Upgrade(gs, Dimension3Catalog.FacilityAutomationCore, log);
        var coordinator = D3FacilitySystem.GetAssignment(state,
            Dimension3Catalog.FacilityAutomationCore, Dimension3Catalog.ChannelCoreCoordination,
            6, Dimension3Catalog.TraitCoordinator);
        if (coordinator == null || coordinator.amount < 1)
            return Done(D3FacilitySystem.TrySetFacilityAssignment(gs,
                Dimension3Catalog.FacilityAutomationCore, Dimension3Catalog.ChannelCoreCoordination,
                6, Dimension3Catalog.TraitCoordinator, 1, out _), log,
                "D3: assigned MK6 Coordinator to core; stabilization pending.");
        if (state.successfulAutomationExecutions < 1) return PrepareOneAutomation(gs, log);
        return false; // Wait for runtime stabilization, then CanIntegrate above.
    }

    private static bool AssembleToward(GameState gs, int mk, long targetCount,
        bool coordinator, Action<string> log)
    {
        long missing = targetCount - D3InventorySystem.GetAssemblyCount(gs.dimension3, mk);
        if (missing <= 0 || !D3AssemblySystem.IsNormalMkUnlocked(gs, mk)) return false;
        // MK5/6 need five FREE prior-MK supports PER unit; serial units reuse them.
        long quantity = mk >= 5 || coordinator ? 1 : Batch(missing);
        foreach (string part in Dimension3Catalog.PartIds)
        {
            long need = quantity - D3InventorySystem.GetPartAmount(gs.dimension3, part, mk);
            if (need > 0)
                return Done(D3ProductionSystem.TryQueuePartProduction(gs, part, mk,
                    Batch(need), out _), log, "D3: queued " + part + " V" + mk);
        }
        if (!coordinator)
            return Done(D3AssemblySystem.TryQueueNormalAssembly(gs, mk, quantity, out _),
                log, "D3: queued " + quantity + " normal MK" + mk);
        List<D3CalibrationReadingState> readings = CoordinatorReadings();
        if (readings == null || D3CalibrationSystem.Evaluate(readings).resultTraitId !=
            Dimension3Catalog.TraitCoordinator) return false;
        return Done(D3AssemblySystem.TryQueueTraitAssembly(gs, mk, readings, out _),
            log, "D3: queued calibrated MK6 Coordinator.");
    }

    private static List<D3CalibrationReadingState> CoordinatorReadings()
    {
        // Legal player controls. Control routing has discrete readings: this
        // permutation yields 83 (near the coordinator pattern's 85), not a fake 85.
        var controls = new[]
        {
            new D3CalibrationControlState { partId = Dimension3Catalog.PartChassis,
                valueA = 50, valueB = 50, valueC = 0 },
            new D3CalibrationControlState { partId = Dimension3Catalog.PartMotor,
                valueA = 45, valueB = 45, valueC = 45 },
            new D3CalibrationControlState { partId = Dimension3Catalog.PartTool,
                optionA = 3, valueA = 55 },
            new D3CalibrationControlState { partId = Dimension3Catalog.PartControl,
                valueA = 1, valueB = 2, valueC = 0,
                optionA = 1, optionB = 0, optionC = 2 },
            new D3CalibrationControlState { partId = Dimension3Catalog.PartRegulator,
                valueA = 55, valueB = 45 }
        };
        var readings = new List<D3CalibrationReadingState>();
        foreach (var control in controls)
        {
            if (!D3CalibrationSystem.TryCalculateReading(control, out int reading, out _))
                return null;
            readings.Add(new D3CalibrationReadingState { partId = control.partId,
                reading = reading });
        }
        return readings;
    }

    private static bool Research(GameState gs, int version, Action<string> log)
    {
        // Reserve existing free normal automatons only. The preceding chain gives
        // 10 MK3 for V4, 5 MK4 for V5, 5 MK5 for V6; no extras are manufactured here.
        var team = new List<D3ReservedAutomatonState>();
        for (int mk = version - 1; mk >= 1; mk--)
        {
            long free = D3InventorySystem.GetAvailableAutomatonAmount(gs.dimension3,
                mk, Dimension3Catalog.TraitNormal);
            if (free > 0) team.Add(new D3ReservedAutomatonState { mk = mk,
                traitId = Dimension3Catalog.TraitNormal, amount = free });
        }
        foreach (string part in Dimension3Catalog.PartIds)
            if (!D3ResearchSystem.IsCompleted(gs.dimension3, part, version))
                return Done(D3ResearchSystem.TryQueueResearch(gs, part, version, team,
                    out _), log, "D3: queued research " + part + " V" + version);
        return false;
    }

    private static bool PrepareOneAutomation(GameState gs, Action<string> log)
    {
        const string facility = Dimension3Catalog.FacilityProductionConsole;
        // N2 permits an explicit policy, so base-game purchases cannot strand a
        // Higgs routine behind the default balanced policy's changing level ratio.
        if (D3FacilitySystem.GetFacilityLevel(gs.dimension3, facility) < 2)
            return Upgrade(gs, facility, log);
        var assignment = D3FacilitySystem.GetAssignment(gs.dimension3, facility,
            Dimension3Catalog.ChannelConsoleCapacity, 2, Dimension3Catalog.TraitNormal);
        if (assignment == null || assignment.amount < 5)
            return Done(D3FacilitySystem.TrySetFacilityAssignment(gs, facility,
                Dimension3Catalog.ChannelConsoleCapacity, 2, Dimension3Catalog.TraitNormal,
                5, out _), log, "D3: assigned five MK2 to console capacity.");
        if (!D3FacilitySystem.IsFunctionActive(gs.dimension3, facility, 2)) return false;

        // At most one routine per action, stop after ONE successful real purchase.
        // Do not record manual history directly: buy through BuildingPurchaseService.
        string[] buildings = { D3ConsoleSystem.BuildingHiggs, D3ConsoleSystem.BuildingTetraquark };
        string[] actions = { D3AutomationCatalog.ActionConsoleBuyHiggs,
            D3AutomationCatalog.ActionConsoleBuyTetraquark };
        for (int i = 0; i < buildings.Length; i++)
        {
            var building = gs.GetBuildingState(buildings[i]);
            if (building == null || building.def == null || building.IsAtMaxLevel() ||
                !BuildingUnlock.IsUnlocked(building.def)) continue;
            var settings = gs.dimension3.consoleSettings;
            string policy = i == 0 ? D3ConsoleSystem.PolicyLE : D3ConsoleSystem.PolicyTraces;
            if (settings.purchasePolicy != policy)
                return Done(D3ConsoleSystem.TrySetPolicyAndReserves(gs, policy,
                    settings.leReserve, settings.tracesReserve, out _), log,
                    "D3: set console purchase policy " + policy);
            if (!D3ConsoleSystem.HasManualBuildingAuthorization(gs.dimension3, buildings[i]))
                return Done(BuildingPurchaseService.TryPurchase(gs, building), log,
                    "D3: manual purchase authorizes " + buildings[i]);
            D3AutomationRoutineState existing = null;
            foreach (var routine in gs.dimension3.automationRoutines)
                if (routine != null && routine.actionId == actions[i])
                { existing = routine; break; }
            if (existing != null)
            {
                if (existing.enabled || existing.executionsCompleted > 0) return false;
                return Done(D3AutomationSystem.TrySetRoutineEnabled(gs,
                    existing.routineId, true, out _), log,
                    "D3: enabled one-purchase routine " + existing.routineId);
            }
            return Done(D3AutomationSystem.TryCreateRoutine(gs, actions[i], buildings[i],
                null, 0, 0, 0, "", 0, 1, out _, out _), log,
                "D3: created one-purchase routine for " + buildings[i]);
        }
        return false; // Both repeatable buildings unavailable/capped: report to coordinator.
    }

    private static bool Upgrade(GameState gs, string facility, Action<string> log)
    {
        return Done(D3FacilitySystem.TryQueueFacilityUpgrade(gs, facility, out _), log,
            "D3: queued " + facility + " N" +
            (D3FacilitySystem.GetFacilityLevel(gs.dimension3, facility) + 1));
    }

    private static bool HasAnyJob(Dimension3State state)
    {
        if (state.queues == null) return false;
        foreach (var queue in state.queues)
            if (queue != null && queue.jobs != null && queue.jobs.Count > 0) return true;
        return false;
    }

    private static long Batch(long missing)
    {
        return missing >= 25 ? 25 : missing >= 10 ? 10 : missing >= 5 ? 5 : 1;
    }

    private static bool Done(bool success, Action<string> log, string message)
    {
        if (success) log?.Invoke(message);
        return success;
    }
}
#endif

using System;


public enum D3AutomationCatalogStatus
{
    Authorized = 0,
    PrepareApi = 1,
    PendingDesign = 2,
    Prohibited = 3,
    Internal = 4
}


public enum D3AutomationActionKind
{
    Repeatable = 0,
    StrategicPreset = 1,
    Unique = 2
}


public enum D3AutomationRisk
{
    None = 0,
    Low = 1,
    High = 2
}


public sealed class D3AutomationActionDefinition
{
    public readonly string actionId;
    public readonly string ownerSystemId;
    public readonly string facilityId;
    public readonly int requiredFacilityLevel;
    public readonly D3AutomationCatalogStatus status;
    public readonly D3AutomationActionKind kind;
    public readonly bool requiresManualExecution;
    public readonly bool offlineAllowedWithCore5;
    public readonly string executionApi;

    public D3AutomationActionDefinition(
        string actionId, string ownerSystemId, string facilityId,
        int requiredFacilityLevel, D3AutomationCatalogStatus status,
        D3AutomationActionKind kind, bool requiresManualExecution,
        bool offlineAllowedWithCore5, string executionApi)
    {
        this.actionId = actionId;
        this.ownerSystemId = ownerSystemId;
        this.facilityId = facilityId;
        this.requiredFacilityLevel = requiredFacilityLevel;
        this.status = status;
        this.kind = kind;
        this.requiresManualExecution = requiresManualExecution;
        this.offlineAllowedWithCore5 = offlineAllowedWithCore5;
        this.executionApi = executionApi;
    }
}


public sealed class D3AutomationDestinationDefinition
{
    public readonly string destinationId;
    public readonly D3AutomationRisk risk;
    public readonly bool repeatable;
    public readonly bool narrativeChain;
    public readonly bool requiresSpecialBlueprint;
    public readonly bool uniqueDecision;

    public D3AutomationDestinationDefinition(
        string destinationId, D3AutomationRisk risk, bool repeatable,
        bool narrativeChain, bool requiresSpecialBlueprint, bool uniqueDecision)
    {
        this.destinationId = destinationId;
        this.risk = risk;
        this.repeatable = repeatable;
        this.narrativeChain = narrativeChain;
        this.requiresSpecialBlueprint = requiresSpecialBlueprint;
        this.uniqueDecision = uniqueDecision;
    }
}


public static class D3AutomationCatalog
{
    public const string ActionFactoryProducePart = "d3.factory.produce_part";
    public const string ActionFactoryAssembleNormal = "d3.factory.assemble_normal";
    public const string ActionFactoryAssembleTrait = "d3.factory.assemble_trait";
    public const string ActionFactoryResearchPart = "d3.factory.research_part";
    public const string ActionFactoryUpgradeFacility = "d3.factory.upgrade_facility";
    public const string ActionDiagnosticAnalyze =
        "d3.diagnostic.analyze_line_node";
    public const string ActionDiagnosticRepair =
        "d3.diagnostic.repair_line_node";
    public const string ActionDiagnosticFusion =
        "d3.diagnostic.repeat_marked_fusion";
    public const string ActionDiagnosticPriority =
        "d3.diagnostic.set_zone_priority";
    public const string ActionDiagnosticCollectFragments =
        "d3.diagnostic.collect_fragments";
    public const string ActionDiagnosticUniqueNode =
        "d3.diagnostic.unique_or_hidden_node";
    public const string ActionConsoleBuyHiggs = "d3.console.buy_higgs";
    public const string ActionConsoleBuyTetraquark =
        "d3.console.buy_tetraquark";
    public const string ActionConsolePurchasePolicy =
        "d3.console.set_purchase_policy";
    public const string ActionConsoleBasicUpgrade =
        "d3.console.repeat_basic_upgrade";
    public const string ActionConsoleModulator =
        "d3.console.set_modulator_phase";
    public const string ActionConsoleTriangle =
        "d3.console.apply_basic_triangle";
    public const string ActionConsoleBuyModulator =
        "d3.console.buy_phase_modulator";
    public const string ActionPortScan = "d3.port.scan_simple_destinations";
    public const string ActionPortRepeatLast =
        "d3.port.repeat_last_simple_route";
    public const string ActionPortPriorityRoutes =
        "d3.port.priority_simple_routes";
    public const string ActionPortSafeRoute = "d3.port.repeat_safe_route";
    public const string ActionPortExtractor = "d3.port.upgrade_extractor";
    public const string ActionPortSecondRoutine =
        "d3.port.second_expedition_routine";
    public const string ActionPortUnlockPlanet = "d3.port.unlock_planet";
    public const string ActionPortUnlockShip = "d3.port.build_or_unlock_ship";
    public const string ActionPortCoordinated = "d3.port.coordinated_mission";
    public const string ActionPortArk = "d3.port.ark_or_chain_mission";
    public const string ActionPortRelic = "d3.port.upgrade_relic";
    public const string ActionPortTree = "d3.port.buy_tree_node";
    public const string ActionPortUpgradeShip = "d3.port.upgrade_ship";
    public const string ActionPortUpgradeScanner = "d3.port.upgrade_scanner";

    public static readonly D3AutomationActionDefinition[] Actions =
    {
        Def(ActionFactoryProducePart, "dimension3_factory",
            Dimension3Catalog.FacilityProcessBank, 1,
            D3AutomationCatalogStatus.Internal,
            D3AutomationActionKind.Repeatable, false, true,
            "D3ProductionSystem.TryQueuePartProduction"),
        Def(ActionFactoryAssembleNormal, "dimension3_factory",
            Dimension3Catalog.FacilityProcessBank, 1,
            D3AutomationCatalogStatus.Internal,
            D3AutomationActionKind.Repeatable, false, true,
            "D3AssemblySystem.TryQueueNormalAssembly"),
        Def(ActionFactoryAssembleTrait, "dimension3_factory",
            Dimension3Catalog.FacilityProcessBank, 1,
            D3AutomationCatalogStatus.Internal,
            D3AutomationActionKind.Repeatable, true, true,
            "D3AssemblySystem.TryQueueTraitAssembly"),
        Def(ActionFactoryResearchPart, "dimension3_factory",
            Dimension3Catalog.FacilityProcessBank, 4,
            D3AutomationCatalogStatus.Internal,
            D3AutomationActionKind.Repeatable, false, true,
            "D3ResearchSystem.TryQueueResearch"),
        Def(ActionFactoryUpgradeFacility, "dimension3_factory",
            Dimension3Catalog.FacilityProcessBank, 1,
            D3AutomationCatalogStatus.Internal,
            D3AutomationActionKind.Unique, false, true,
            "D3FacilitySystem.TryQueueFacilityUpgrade"),
        Def(ActionDiagnosticAnalyze, "machine",
            Dimension3Catalog.FacilityDiagnosticBank, 1,
            D3AutomationCatalogStatus.Authorized,
            D3AutomationActionKind.Repeatable, false, true,
            "MachineManager.TryStartNodeAnalysis"),
        Def(ActionDiagnosticRepair, "machine",
            Dimension3Catalog.FacilityDiagnosticBank, 2,
            D3AutomationCatalogStatus.Authorized,
            D3AutomationActionKind.Repeatable, false, true,
            "MachineManager.CanRepairNode/TryRepairNode"),
        Def(ActionDiagnosticFusion, "room2_fusion",
            Dimension3Catalog.FacilityDiagnosticBank, 4,
            D3AutomationCatalogStatus.Authorized,
            D3AutomationActionKind.Repeatable, true, true,
            "D3FusionService.TryExecuteMarkedSafeFusion"),
        Def(ActionDiagnosticPriority, "machine",
            Dimension3Catalog.FacilityDiagnosticBank, 3,
            D3AutomationCatalogStatus.Internal,
            D3AutomationActionKind.StrategicPreset, false, true,
            "D3DiagnosticSystem.TryConfigure"),
        Def(ActionDiagnosticCollectFragments, "room2_fusion",
            Dimension3Catalog.FacilityDiagnosticBank, 0,
            D3AutomationCatalogStatus.Prohibited,
            D3AutomationActionKind.Repeatable, false, false,
            "Prohibited: fragments are generated by Room 1 artifacts"),
        Def(ActionDiagnosticUniqueNode, "machine",
            Dimension3Catalog.FacilityDiagnosticBank, 0,
            D3AutomationCatalogStatus.Prohibited,
            D3AutomationActionKind.Unique, false, false,
            "Prohibited unique/hidden/key node"),
        Def(ActionConsoleBuyHiggs, "room1_buildings",
            Dimension3Catalog.FacilityProductionConsole, 1,
            D3AutomationCatalogStatus.Authorized,
            D3AutomationActionKind.Repeatable, true, true,
            "D3ConsoleSystem.TryPurchaseRepeatableBuilding"),
        Def(ActionConsoleBuyTetraquark, "room1_buildings",
            Dimension3Catalog.FacilityProductionConsole, 1,
            D3AutomationCatalogStatus.Authorized,
            D3AutomationActionKind.Repeatable, true, true,
            "D3ConsoleSystem.TryPurchaseRepeatableBuilding"),
        Def(ActionConsolePurchasePolicy, "room1_buildings",
            Dimension3Catalog.FacilityProductionConsole, 2,
            D3AutomationCatalogStatus.Internal,
            D3AutomationActionKind.StrategicPreset, false, true,
            "D3ConsoleSystem.TrySetPolicyAndReserves"),
        Def(ActionConsoleBasicUpgrade, "room1_upgrades",
            Dimension3Catalog.FacilityProductionConsole, 3,
            D3AutomationCatalogStatus.PendingDesign,
            D3AutomationActionKind.Repeatable, true, false,
            "Blocked until closed safe list exists"),
        Def(ActionConsoleModulator, "room1_modulator",
            Dimension3Catalog.FacilityProductionConsole, 4,
            D3AutomationCatalogStatus.Authorized,
            D3AutomationActionKind.StrategicPreset, true, true,
            "D3ConsoleSystem.TryApplyPreferredPhase"),
        Def(ActionConsoleTriangle, "room1_triangle",
            Dimension3Catalog.FacilityProductionConsole, 5,
            D3AutomationCatalogStatus.Authorized,
            D3AutomationActionKind.StrategicPreset, true, true,
            "D3ConsoleSystem.TryApplyTrianglePreset"),
        Def(ActionConsoleBuyModulator, "room1_buildings",
            Dimension3Catalog.FacilityProductionConsole, 0,
            D3AutomationCatalogStatus.Prohibited,
            D3AutomationActionKind.Unique, false, false,
            "Prohibited unique purchase"),
        Def(ActionPortScan, "dimension1",
            Dimension3Catalog.FacilityExpeditionPort, 1,
            D3AutomationCatalogStatus.Authorized,
            D3AutomationActionKind.Repeatable, true, true,
            "Dimension1System.TryScanSimpleDestinationAutomated"),
        Def(ActionPortRepeatLast, "dimension1",
            Dimension3Catalog.FacilityExpeditionPort, 1,
            D3AutomationCatalogStatus.Authorized,
            D3AutomationActionKind.Repeatable, true, true,
            "Dimension1System.TryStartExplorationByDestinationId"),
        Def(ActionPortPriorityRoutes, "dimension1",
            Dimension3Catalog.FacilityExpeditionPort, 2,
            D3AutomationCatalogStatus.Authorized,
            D3AutomationActionKind.StrategicPreset, true, true,
            "GameState.dimension1ManualSimpleDestinationIds"),
        Def(ActionPortSafeRoute, "dimension1",
            Dimension3Catalog.FacilityExpeditionPort, 3,
            D3AutomationCatalogStatus.Authorized,
            D3AutomationActionKind.Repeatable, true, true,
            "D3AutomationCatalog + TryStartExplorationByDestinationId"),
        Def(ActionPortExtractor, "dimension1",
            Dimension3Catalog.FacilityExpeditionPort, 4,
            D3AutomationCatalogStatus.Authorized,
            D3AutomationActionKind.Repeatable, true, true,
            "Dimension1System.TryUpgradeExtractorAutomated"),
        Def(ActionPortSecondRoutine, "dimension1",
            Dimension3Catalog.FacilityExpeditionPort, 5,
            D3AutomationCatalogStatus.Internal,
            D3AutomationActionKind.StrategicPreset, false, true,
            "D3AutomationSystem routine capacity"),
        Def(ActionPortUnlockPlanet, "dimension1",
            Dimension3Catalog.FacilityExpeditionPort, 0,
            D3AutomationCatalogStatus.Prohibited,
            D3AutomationActionKind.Unique, true, false,
            "Prohibited"),
        Def(ActionPortUnlockShip, "dimension1",
            Dimension3Catalog.FacilityExpeditionPort, 0,
            D3AutomationCatalogStatus.Prohibited,
            D3AutomationActionKind.Unique, true, false,
            "Prohibited"),
        Def(ActionPortCoordinated, "dimension1",
            Dimension3Catalog.FacilityExpeditionPort, 0,
            D3AutomationCatalogStatus.Prohibited,
            D3AutomationActionKind.Unique, true, false,
            "Prohibited"),
        Def(ActionPortArk, "dimension1",
            Dimension3Catalog.FacilityExpeditionPort, 0,
            D3AutomationCatalogStatus.Prohibited,
            D3AutomationActionKind.Unique, true, false,
            "Prohibited"),
        Def(ActionPortRelic, "dimension1",
            Dimension3Catalog.FacilityExpeditionPort, 0,
            D3AutomationCatalogStatus.Prohibited,
            D3AutomationActionKind.StrategicPreset, true, false,
            "Prohibited"),
        Def(ActionPortTree, "dimension1",
            Dimension3Catalog.FacilityExpeditionPort, 0,
            D3AutomationCatalogStatus.Prohibited,
            D3AutomationActionKind.StrategicPreset, true, false,
            "Prohibited"),
        Def(ActionPortUpgradeShip, "dimension1",
            Dimension3Catalog.FacilityExpeditionPort, 0,
            D3AutomationCatalogStatus.PendingDesign,
            D3AutomationActionKind.Repeatable, true, false,
            "Pending closed ship-upgrade list"),
        Def(ActionPortUpgradeScanner, "dimension1",
            Dimension3Catalog.FacilityExpeditionPort, 0,
            D3AutomationCatalogStatus.PendingDesign,
            D3AutomationActionKind.Repeatable, true, false,
            "Pending scanner-upgrade design")
    };

    public static readonly D3AutomationDestinationDefinition[] Destinations =
    {
        Destination(Dimension1System.DestinationMineralBelt),
        Destination(Dimension1System.DestinationShipGraveyard),
        Destination(Dimension1System.DestinationDriftingProbes),
        Destination(Dimension1System.DestinationAbandonedShip),
        Destination(Dimension1System.DestinationOrbitalRuin),
        Destination(Dimension1System.DestinationLaboratory),
        Destination(Dimension1System.DestinationAbandonedStation),
        Destination(Dimension1System.DestinationMinorAnomaly),
        Destination(Dimension1System.DestinationAncientStructure),
        Destination(Dimension1System.DestinationUnstableZone)
    };

    public static D3AutomationActionDefinition GetAction(string actionId)
    {
        for (int i = 0; i < Actions.Length; i++)
            if (Actions[i].actionId == actionId) return Actions[i];
        return null;
    }

    public static D3AutomationDestinationDefinition GetDestination(
        string destinationId)
    {
        for (int i = 0; i < Destinations.Length; i++)
            if (Destinations[i].destinationId == destinationId)
                return Destinations[i];
        return null;
    }

    public static bool IsRepeatableSafeDestination(string destinationId)
    {
        D3AutomationDestinationDefinition definition =
            GetDestination(destinationId);
        return definition != null && definition.repeatable &&
               !definition.narrativeChain &&
               !definition.requiresSpecialBlueprint &&
               !definition.uniqueDecision &&
               (definition.risk == D3AutomationRisk.None ||
                definition.risk == D3AutomationRisk.Low);
    }

    private static D3AutomationActionDefinition Def(
        string actionId, string ownerSystemId, string facilityId,
        int requiredFacilityLevel, D3AutomationCatalogStatus status,
        D3AutomationActionKind kind, bool requiresManualExecution,
        bool offlineAllowedWithCore5, string executionApi)
    {
        return new D3AutomationActionDefinition(
            actionId, ownerSystemId, facilityId, requiredFacilityLevel,
            status, kind, requiresManualExecution, offlineAllowedWithCore5,
            executionApi);
    }

    private static D3AutomationDestinationDefinition Destination(
        string destinationId)
    {
        return new D3AutomationDestinationDefinition(
            destinationId, D3AutomationRisk.None, true, false, false, false);
    }
}

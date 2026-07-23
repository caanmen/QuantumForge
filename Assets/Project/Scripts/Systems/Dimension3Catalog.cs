using System;


public sealed class D3CostTimeDefinition
{
    public readonly double leCost;
    public readonly double tracesCost;
    public readonly double durationSeconds;

    public D3CostTimeDefinition(double leCost, double tracesCost, double durationSeconds)
    {
        this.leCost = leCost;
        this.tracesCost = tracesCost;
        this.durationSeconds = durationSeconds;
    }
}


public sealed class D3FacilityLevelDefinition
{
    public readonly int level;
    public readonly double leCost;
    public readonly double tracesCost;
    public readonly double durationSeconds;
    public readonly int requiredAssemblyMk;
    public readonly long requiredAssemblyAmount;

    public D3FacilityLevelDefinition(
        int level,
        double leCost,
        double tracesCost,
        double durationSeconds,
        int requiredAssemblyMk,
        long requiredAssemblyAmount)
    {
        this.level = level;
        this.leCost = leCost;
        this.tracesCost = tracesCost;
        this.durationSeconds = durationSeconds;
        this.requiredAssemblyMk = requiredAssemblyMk;
        this.requiredAssemblyAmount = requiredAssemblyAmount;
    }
}


public sealed class D3ResearchDefinition
{
    public readonly string researchId;
    public readonly string partId;
    public readonly int version;
    public readonly double leCost;
    public readonly double tracesCost;
    public readonly double durationSeconds;
    public readonly double minimumPower;

    public D3ResearchDefinition(
        string researchId, string partId, int version, double leCost,
        double tracesCost, double durationSeconds, double minimumPower)
    {
        this.researchId = researchId;
        this.partId = partId;
        this.version = version;
        this.leCost = leCost;
        this.tracesCost = tracesCost;
        this.durationSeconds = durationSeconds;
        this.minimumPower = minimumPower;
    }
}


public static class Dimension3Catalog
{
    public const string PartChassis = "chassis";
    public const string PartMotor = "motor";
    public const string PartTool = "tool";
    public const string PartControl = "control";
    public const string PartRegulator = "regulator";

    public const string TraitNormal = "normal";
    public const string TraitFast = "fast";
    public const string TraitEfficient = "efficient";
    public const string TraitCoordinator = "coordinator";

    public const string ChannelProcessPower = "process_power";
    public const string ChannelProcessTime = "process_time";
    public const string ChannelProcessCost = "process_cost";
    public const string ChannelProcessCoordination = "process_coordination";

    public const string ChannelConsoleCapacity = "console_capacity";
    public const string ChannelConsoleResponse = "console_response";
    public const string ChannelConsoleCost = "console_cost";
    public const string ChannelConsoleCoordination = "console_coordination";
    public const string ChannelDiagnosticCapacity = "diagnostic_capacity";
    public const string ChannelDiagnosticResponse = "diagnostic_response";
    public const string ChannelDiagnosticCoordination = "diagnostic_coordination";
    public const string ChannelPortCapacity = "port_capacity";
    public const string ChannelPortResponse = "port_response";
    public const string ChannelPortCoordination = "port_coordination";
    public const string ChannelCoreCoordination = "core_coordination";

    public const string FacilityProcessBank = "process_bank";
    public const string FacilityProductionConsole = "production_console";
    public const string FacilityDiagnosticBank = "diagnostic_bank";
    public const string FacilityExpeditionPort = "expedition_port";
    public const string FacilityAutomationCore = "automation_core";

    public const string QueuePartProduction = "part_production";
    public const string QueueAssembly = "assembly";
    public const string QueueResearch = "research";
    public const string QueueFacility = "facility";

    public const string JobPartProduction = "part_production";
    public const string JobAssembly = "assembly";
    public const string JobFacilityUpgrade = "facility_upgrade";
    public const string JobResearch = "research";

    public const int MaxQueueEntries = 10;
    public const double AssignmentStabilizationSeconds = 30.0;
    public const double OfflineProgressCapSeconds = 12.0 * 60.0 * 60.0;

    public static readonly string[] PartIds =
    {
        PartChassis,
        PartMotor,
        PartTool,
        PartControl,
        PartRegulator
    };

    public static readonly string[] QueueIds =
    {
        QueuePartProduction,
        QueueAssembly,
        QueueResearch,
        QueueFacility
    };

    public static readonly string[] TraitIds =
    {
        TraitNormal,
        TraitFast,
        TraitEfficient,
        TraitCoordinator
    };

    public static readonly string[] ProcessBankChannelIds =
    {
        ChannelProcessPower,
        ChannelProcessTime,
        ChannelProcessCost,
        ChannelProcessCoordination
    };

    public static readonly string[] ProductionConsoleChannelIds =
    {
        ChannelConsoleCapacity,
        ChannelConsoleResponse,
        ChannelConsoleCost,
        ChannelConsoleCoordination
    };

    public static readonly string[] DiagnosticBankChannelIds =
    {
        ChannelDiagnosticCapacity,
        ChannelDiagnosticResponse,
        ChannelDiagnosticCoordination
    };

    public static readonly string[] ExpeditionPortChannelIds =
    {
        ChannelPortCapacity,
        ChannelPortResponse,
        ChannelPortCoordination
    };

    public static readonly string[] AutomationCoreChannelIds =
    {
        ChannelCoreCoordination
    };

    public static int GetTraitPatternReading(string traitId, string partId)
    {
        if (traitId == TraitFast)
        {
            if (partId == PartChassis) return 40;
            if (partId == PartMotor) return 80;
            if (partId == PartTool) return 65;
            if (partId == PartControl) return 35;
            if (partId == PartRegulator) return 70;
        }
        else if (traitId == TraitEfficient)
        {
            if (partId == PartChassis) return 65;
            if (partId == PartMotor) return 35;
            if (partId == PartTool) return 45;
            if (partId == PartControl) return 50;
            if (partId == PartRegulator) return 25;
        }
        else if (traitId == TraitCoordinator)
        {
            if (partId == PartChassis) return 50;
            if (partId == PartMotor) return 45;
            if (partId == PartTool) return 70;
            if (partId == PartControl) return 85;
            if (partId == PartRegulator) return 55;
        }
        return -1;
    }

    public static bool IsPartId(string partId)
    {
        if (string.IsNullOrWhiteSpace(partId))
            return false;

        for (int i = 0; i < PartIds.Length; i++)
        {
            if (PartIds[i] == partId)
                return true;
        }

        return false;
    }

    public static bool IsQueueId(string queueId)
    {
        if (string.IsNullOrWhiteSpace(queueId))
            return false;

        for (int i = 0; i < QueueIds.Length; i++)
        {
            if (QueueIds[i] == queueId)
                return true;
        }

        return false;
    }

    public static bool IsTraitId(string traitId)
    {
        for (int i = 0; i < TraitIds.Length; i++)
            if (TraitIds[i] == traitId) return true;
        return false;
    }

    public static bool IsProcessBankChannel(string channelId)
    {
        for (int i = 0; i < ProcessBankChannelIds.Length; i++)
            if (ProcessBankChannelIds[i] == channelId) return true;
        return false;
    }

    public static bool IsConnectedFacility(string facilityId)
    {
        return facilityId == FacilityProductionConsole ||
               facilityId == FacilityDiagnosticBank ||
               facilityId == FacilityExpeditionPort ||
               facilityId == FacilityAutomationCore;
    }

    public static bool IsFacilityChannel(string facilityId, string channelId)
    {
        string[] channels = facilityId == FacilityProcessBank
            ? ProcessBankChannelIds
            : facilityId == FacilityProductionConsole
                ? ProductionConsoleChannelIds
                : facilityId == FacilityDiagnosticBank
                    ? DiagnosticBankChannelIds
                    : facilityId == FacilityExpeditionPort
                        ? ExpeditionPortChannelIds
                        : facilityId == FacilityAutomationCore
                            ? AutomationCoreChannelIds
                            : null;
        if (channels == null) return false;
        for (int i = 0; i < channels.Length; i++)
            if (channels[i] == channelId) return true;
        return false;
    }

    public static string GetPreferredTraitForChannel(string channelId)
    {
        switch (channelId)
        {
            case ChannelProcessPower: return TraitNormal;
            case ChannelProcessTime: return TraitFast;
            case ChannelProcessCost: return TraitEfficient;
            case ChannelProcessCoordination: return TraitCoordinator;
            case ChannelConsoleCapacity: return TraitNormal;
            case ChannelConsoleResponse: return TraitFast;
            case ChannelConsoleCost: return TraitEfficient;
            case ChannelConsoleCoordination: return TraitCoordinator;
            case ChannelDiagnosticCapacity: return TraitNormal;
            case ChannelDiagnosticResponse: return TraitFast;
            case ChannelDiagnosticCoordination: return TraitCoordinator;
            case ChannelPortCapacity: return TraitNormal;
            case ChannelPortResponse: return TraitFast;
            case ChannelPortCoordination: return TraitCoordinator;
            case ChannelCoreCoordination: return TraitCoordinator;
            default: return "";
        }
    }

    public static bool IsSupportedBatchSize(long quantity)
    {
        return quantity == 1 || quantity == 5 || quantity == 10 || quantity == 25;
    }

    public static string GetResearchId(string partId, int version)
    {
        return "research_" + partId + "_v" + version;
    }

    public static D3ResearchDefinition GetResearchDefinition(string partId, int version)
    {
        if (!IsPartId(partId) || version < 4 || version > 6) return null;
        switch (version)
        {
            case 4: return new D3ResearchDefinition(
                GetResearchId(partId, version), partId, version,
                100000.0, 100.0, 1200.0, 50.0);
            case 5: return new D3ResearchDefinition(
                GetResearchId(partId, version), partId, version,
                450000.0, 350.0, 3600.0, 180.0);
            case 6: return new D3ResearchDefinition(
                GetResearchId(partId, version), partId, version,
                1500000.0, 1000.0, 10800.0, 500.0);
            default: return null;
        }
    }

    public static D3CostTimeDefinition GetPartDefinition(int version)
    {
        switch (version)
        {
            case 1: return new D3CostTimeDefinition(100.0, 1.0, 10.0);
            case 2: return new D3CostTimeDefinition(600.0, 3.0, 30.0);
            case 3: return new D3CostTimeDefinition(3000.0, 10.0, 90.0);
            case 4: return new D3CostTimeDefinition(12500.0, 30.0, 240.0);
            case 5: return new D3CostTimeDefinition(45000.0, 90.0, 600.0);
            case 6: return new D3CostTimeDefinition(150000.0, 250.0, 1500.0);
            default: return null;
        }
    }

    public static D3CostTimeDefinition GetNormalAssemblyDefinition(int mk)
    {
        switch (mk)
        {
            case 1: return new D3CostTimeDefinition(2000.0, 5.0, 30.0);
            case 2: return new D3CostTimeDefinition(12000.0, 20.0, 120.0);
            case 3: return new D3CostTimeDefinition(60000.0, 75.0, 360.0);
            case 4: return new D3CostTimeDefinition(250000.0, 220.0, 900.0);
            case 5: return new D3CostTimeDefinition(900000.0, 650.0, 2400.0);
            case 6: return new D3CostTimeDefinition(3000000.0, 1800.0, 5400.0);
            default: return null;
        }
    }

    public static int GetMkPower(int mk)
    {
        switch (mk)
        {
            case 1: return 1;
            case 2: return 4;
            case 3: return 12;
            case 4: return 35;
            case 5: return 100;
            case 6: return 280;
            default: return 0;
        }
    }

    public static D3FacilityLevelDefinition GetProcessBankLevelDefinition(int level)
    {
        switch (level)
        {
            case 2: return new D3FacilityLevelDefinition(2, 20000.0, 30.0, 120.0, 1, 10L);
            case 3: return new D3FacilityLevelDefinition(3, 100000.0, 100.0, 480.0, 2, 10L);
            case 4: return new D3FacilityLevelDefinition(4, 400000.0, 300.0, 1200.0, 3, 10L);
            case 5: return new D3FacilityLevelDefinition(5, 1500000.0, 900.0, 3600.0, 4, 5L);
            default: return null;
        }
    }

    public static D3FacilityLevelDefinition GetFacilityLevelDefinition(
        string facilityId, int level)
    {
        if (facilityId == FacilityProcessBank)
            return GetProcessBankLevelDefinition(level);

        if (facilityId == FacilityProductionConsole)
        {
            switch (level)
            {
                case 1: return new D3FacilityLevelDefinition(1, 25000.0, 40.0, 120.0, 1, 5L);
                case 2: return new D3FacilityLevelDefinition(2, 75000.0, 100.0, 300.0, 1, 15L);
                case 3: return new D3FacilityLevelDefinition(3, 250000.0, 250.0, 900.0, 2, 5L);
                case 4: return new D3FacilityLevelDefinition(4, 900000.0, 700.0, 2400.0, 3, 5L);
                case 5: return new D3FacilityLevelDefinition(5, 3000000.0, 1800.0, 5400.0, 5, 1L);
            }
        }

        if (facilityId == FacilityDiagnosticBank)
        {
            switch (level)
            {
                case 1: return new D3FacilityLevelDefinition(1, 50000.0, 75.0, 180.0, 1, 5L);
                case 2: return new D3FacilityLevelDefinition(2, 150000.0, 180.0, 480.0, 0, 0L);
                case 3: return new D3FacilityLevelDefinition(3, 500000.0, 500.0, 1200.0, 2, 5L);
                case 4: return new D3FacilityLevelDefinition(4, 1500000.0, 1200.0, 3000.0, 4, 1L);
                case 5: return new D3FacilityLevelDefinition(5, 4000000.0, 2500.0, 7200.0, 5, 1L);
            }
        }


        if (facilityId == FacilityExpeditionPort)
        {
            switch (level)
            {
                case 1: return new D3FacilityLevelDefinition(1, 50000.0, 75.0, 180.0, 1, 5L);
                case 2: return new D3FacilityLevelDefinition(2, 150000.0, 180.0, 480.0, 0, 0L);
                case 3: return new D3FacilityLevelDefinition(3, 500000.0, 500.0, 1200.0, 2, 5L);
                case 4: return new D3FacilityLevelDefinition(4, 1500000.0, 1200.0, 3000.0, 4, 1L);
                case 5: return new D3FacilityLevelDefinition(5, 4000000.0, 2500.0, 7200.0, 5, 1L);
            }
        }

        if (facilityId == FacilityAutomationCore)
        {
            switch (level)
            {
                case 1: return new D3FacilityLevelDefinition(1, 250000.0, 300.0, 600.0, 2, 10L);
                case 2: return new D3FacilityLevelDefinition(2, 750000.0, 700.0, 1200.0, 3, 5L);
                case 3: return new D3FacilityLevelDefinition(3, 2000000.0, 1500.0, 2700.0, 4, 1L);
                case 4: return new D3FacilityLevelDefinition(4, 5000000.0, 3500.0, 5400.0, 5, 1L);
                case 5: return new D3FacilityLevelDefinition(5, 12000000.0, 8000.0, 10800.0, 6, 1L);
            }
        }
        return null;
    }
}

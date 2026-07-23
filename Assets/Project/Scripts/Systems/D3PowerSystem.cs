using System;


public sealed class D3ProcessModifiers
{
    public double progressBonusPercent;
    public double timeBonusRaw;
    public double costBonusRaw;
    public double coordinationPercent;
    public double progressMultiplier = 1.0;
    public double timeMultiplier = 1.0;
    public double costMultiplier = 1.0;
}


public static class D3PowerSystem
{
    public static double GetRawChannelPower(
        Dimension3State state,
        string installationId,
        string channelId)
    {
        if (state == null || state.assignments == null)
            return 0.0;

        double power = 0.0;
        string preferredTrait = Dimension3Catalog.GetPreferredTraitForChannel(channelId);
        for (int i = 0; i < state.assignments.Count; i++)
        {
            D3AssignmentState assignment = state.assignments[i];
            if (assignment == null || assignment.installationId != installationId ||
                assignment.channelId != channelId || assignment.stabilizedAmount <= 0L)
            {
                continue;
            }

            double contribution = assignment.stabilizedAmount *
                Dimension3Catalog.GetMkPower(assignment.mk);
            if (assignment.traitId == preferredTrait)
                contribution *= 1.25;
            power += contribution;
        }
        return Math.Max(0.0, power);
    }

    public static D3ProcessModifiers GetProcessBankModifiers(Dimension3State state)
    {
        double coordinationPower = GetRawChannelPower(
            state,
            Dimension3Catalog.FacilityProcessBank,
            Dimension3Catalog.ChannelProcessCoordination
        );
        double coreMultiplier = D3FacilitySystem
            .GetAutomationCoreEfficiencyMultiplier(state);
        double coordinationPercent = Math.Sqrt(coordinationPower) * 0.35 *
            coreMultiplier;
        double coordinationMultiplier = 1.0 + coordinationPercent / 100.0;

        double progressBonus = Math.Sqrt(GetRawChannelPower(
            state, Dimension3Catalog.FacilityProcessBank,
            Dimension3Catalog.ChannelProcessPower)) * 0.50 *
            coordinationMultiplier * coreMultiplier;
        double timeBonus = Math.Sqrt(GetRawChannelPower(
            state, Dimension3Catalog.FacilityProcessBank,
            Dimension3Catalog.ChannelProcessTime)) * 0.25 *
            coordinationMultiplier * coreMultiplier;
        double costBonus = Math.Sqrt(GetRawChannelPower(
            state, Dimension3Catalog.FacilityProcessBank,
            Dimension3Catalog.ChannelProcessCost)) * 0.20 *
            coordinationMultiplier * coreMultiplier;

        return new D3ProcessModifiers
        {
            progressBonusPercent = progressBonus,
            timeBonusRaw = timeBonus,
            costBonusRaw = costBonus,
            coordinationPercent = coordinationPercent,
            progressMultiplier = 1.0 + progressBonus / 100.0,
            timeMultiplier = 1.0 / (1.0 + timeBonus / 100.0),
            costMultiplier = 1.0 / (1.0 + costBonus / 100.0)
        };
    }

    public static double GetModifiedDuration(Dimension3State state, double baseSeconds)
    {
        D3ProcessModifiers modifiers = GetProcessBankModifiers(state);
        double duration = Math.Max(0.0, baseSeconds) * modifiers.timeMultiplier /
            Math.Max(0.000001, modifiers.progressMultiplier);
        return Math.Max(0.1, duration);
    }

    public static double GetDynamicWorkRate(Dimension3State state)
    {
        D3ProcessModifiers modifiers = GetProcessBankModifiers(state);
        return Math.Max(0.000001, modifiers.progressMultiplier /
            Math.Max(0.000001, modifiers.timeMultiplier));
    }

    public static double GetModifiedCost(Dimension3State state, double baseCost)
    {
        D3ProcessModifiers modifiers = GetProcessBankModifiers(state);
        return Math.Ceiling(Math.Max(0.0, baseCost) * modifiers.costMultiplier);
    }
}

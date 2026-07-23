using System;
using System.Collections.Generic;


public static class D2BondSystem
{
    public const string PilgrimPathId = "d2_c1_bond_pilgrim_path";
    public const string SacredCraftId = "d2_c1_bond_sacred_craft";
    public const string AcolyteOrderId = "d2_c1_bond_acolyte_order";
    public const string SanctuaryEchoId = "d2_c1_bond_sanctuary_echo";
    public const string TraceLiturgyId = "d2_c1_bond_trace_liturgy";

    public const int MaxLineLevel = 3;
    public const double PrepareIncenseCost = 100.0;
    public const double PrepareSacredClothCost = 100.0;
    public const double PrepareCarvedStoneCost = 100.0;
    public const double BaseProgressPerMinuteFactor = 1.0;
    public const double PilgrimPathBonusPerLevel = 0.05;
    public const double SacredCraftBonusPerLevel = 0.075;
    public const double AcolyteOrderBonusPerLevel = 0.05;
    public const double SanctuaryEchoBonusPerLevel = 0.01;
    public const double TraceLiturgyBonusPerLevel = 0.01;

    public static readonly string[] LineIds =
    {
        PilgrimPathId,
        SacredCraftId,
        AcolyteOrderId,
        SanctuaryEchoId,
        TraceLiturgyId
    };

    public static void EnsureState(D2Civilization1State state)
    {
        if (state == null)
            return;

        if (state.bondLines == null)
            state.bondLines = new List<D2BondLineState>();
        for (int i = state.bondLines.Count - 1; i >= 0; i--)
        {
            D2BondLineState line = state.bondLines[i];
            if (line == null || !IsLineId(line.lineId) || HasEarlierDuplicate(state, i))
                state.bondLines.RemoveAt(i);
        }
        foreach (string lineId in LineIds)
        {
            D2BondLineState line = FindLine(state, lineId);
            if (line == null)
            {
                line = new D2BondLineState { lineId = lineId };
                state.bondLines.Add(line);
            }
            line.level = Math.Clamp(line.level, 0, MaxLineLevel);
        }

        state.acolytesAssignedToBond = Math.Max(0L, state.acolytesAssignedToBond);
        if (double.IsNaN(state.bondProgress) || double.IsInfinity(state.bondProgress) ||
            state.bondProgress < 0.0)
        {
            state.bondProgress = 0.0;
        }
        if (state.lastBondResult == null)
            state.lastBondResult = "";
        if (!state.entityContactAvailable)
            state.bondPlacePrepared = false;
    }

    public static void Tick(D2Civilization1State state, double seconds)
    {
        Tick(state, seconds, 1.0);
    }

    public static void Tick(
        D2Civilization1State state,
        double seconds,
        double externalMultiplier
    )
    {
        if (state == null || seconds <= 0.0 || double.IsNaN(seconds) ||
            double.IsInfinity(seconds))
        {
            return;
        }
        EnsureState(state);
        if (!state.bondPlacePrepared || state.acolytesAssignedToBond <= 0L)
            return;

        double multiplier = D2CivilizationPactSystem.IsPactActive(
            state,
            D2CivilizationPactSystem.InnerDoorId
        ) ? 1.0 + D2CivilizationPactSystem.InnerDoorBondProgressBonus : 1.0;
        state.bondProgress += Math.Sqrt(state.acolytesAssignedToBond) *
            BaseProgressPerMinuteFactor * multiplier * Math.Max(1.0, externalMultiplier) *
            (seconds / 60.0);
    }

    public static bool CanPrepare(GameState gameState)
    {
        if (!Dimension2System.CanAccessDimension2(gameState))
            return false;
        Dimension2System.EnsureState(gameState);
        D2Civilization1State state = gameState.dimension2.civilization1;
        if (!state.entityContactAvailable || state.bondPlacePrepared)
            return false;
        return HasOffering(state, D2AltarSystem.IncenseAltarId, PrepareIncenseCost) &&
            HasOffering(state, D2AltarSystem.SacredClothAltarId, PrepareSacredClothCost) &&
            HasOffering(state, D2AltarSystem.CarvedStoneAltarId, PrepareCarvedStoneCost);
    }

    public static bool IsMajorPactEstablished(D2Civilization1State state)
    {
        return state != null && state.entityContactAvailable && state.bondPlacePrepared;
    }

    public static bool TryPrepare(GameState gameState)
    {
        if (!CanPrepare(gameState))
            return false;
        D2Civilization1State state = gameState.dimension2.civilization1;
        SpendOffering(state, D2AltarSystem.IncenseAltarId, PrepareIncenseCost);
        SpendOffering(state, D2AltarSystem.SacredClothAltarId, PrepareSacredClothCost);
        SpendOffering(state, D2AltarSystem.CarvedStoneAltarId, PrepareCarvedStoneCost);
        state.bondPlacePrepared = true;
        state.lastBondResult = "Pacto mayor establecido en el Lugar de Vínculo.";
        return true;
    }

    public static bool TryAssignAcolytes(GameState gameState, long amount)
    {
        if (!Dimension2System.CanAccessDimension2(gameState) || amount <= 0L)
            return false;
        Dimension2System.EnsureState(gameState);
        D2Civilization1State state = gameState.dimension2.civilization1;
        if (!state.bondPlacePrepared)
            return false;
        long assigned = Math.Min(amount, state.acolytesAvailable);
        if (assigned <= 0L)
            return false;
        state.acolytesAvailable -= assigned;
        state.acolytesAssignedToBond = SaturatingAdd(state.acolytesAssignedToBond, assigned);
        return true;
    }

    public static bool TryReleaseAcolytes(GameState gameState, long amount)
    {
        if (!Dimension2System.CanAccessDimension2(gameState) || amount <= 0L)
            return false;
        Dimension2System.EnsureState(gameState);
        D2Civilization1State state = gameState.dimension2.civilization1;
        long released = Math.Min(amount, state.acolytesAssignedToBond);
        if (released <= 0L)
            return false;
        state.acolytesAssignedToBond -= released;
        state.acolytesAvailable = SaturatingAdd(state.acolytesAvailable, released);
        return true;
    }

    public static bool CanUpgrade(GameState gameState, string lineId)
    {
        if (!Dimension2System.CanAccessDimension2(gameState) || !IsLineId(lineId))
            return false;
        Dimension2System.EnsureState(gameState);
        D2Civilization1State state = gameState.dimension2.civilization1;
        D2BondLineState line = FindLine(state, lineId);
        if (!state.bondPlacePrepared || line == null || line.level >= MaxLineLevel)
            return false;
        int nextLevel = line.level + 1;
        double offeringCost = GetOfferingCost(nextLevel);
        return state.bondProgress >= GetProgressCost(nextLevel) &&
            HasOffering(state, D2AltarSystem.IncenseAltarId, offeringCost) &&
            HasOffering(state, D2AltarSystem.SacredClothAltarId, offeringCost) &&
            HasOffering(state, D2AltarSystem.CarvedStoneAltarId, offeringCost);
    }

    public static bool TryUpgrade(GameState gameState, string lineId)
    {
        if (!CanUpgrade(gameState, lineId))
            return false;
        D2Civilization1State state = gameState.dimension2.civilization1;
        D2BondLineState line = FindLine(state, lineId);
        int nextLevel = line.level + 1;
        double progressCost = GetProgressCost(nextLevel);
        double offeringCost = GetOfferingCost(nextLevel);
        state.bondProgress -= progressCost;
        SpendOffering(state, D2AltarSystem.IncenseAltarId, offeringCost);
        SpendOffering(state, D2AltarSystem.SacredClothAltarId, offeringCost);
        SpendOffering(state, D2AltarSystem.CarvedStoneAltarId, offeringCost);
        line.level = nextLevel;
        state.lastBondResult = GetDisplayName(lineId) + " mejorada a nivel " + nextLevel + ".";
        return true;
    }

    public static int GetLevel(D2Civilization1State state, string lineId)
    {
        return Math.Clamp(FindLine(state, lineId)?.level ?? 0, 0, MaxLineLevel);
    }

    public static double GetProgressCost(int nextLevel) => 20.0 * Math.Clamp(nextLevel, 1, MaxLineLevel);
    public static double GetOfferingCost(int nextLevel) => 25.0 * Math.Clamp(nextLevel, 1, MaxLineLevel);
    public static double GetPilgrimPathBonus(D2Civilization1State state) => GetLevel(state, PilgrimPathId) * PilgrimPathBonusPerLevel;
    public static double GetSacredCraftBonus(D2Civilization1State state) => GetLevel(state, SacredCraftId) * SacredCraftBonusPerLevel;
    public static double GetAcolyteOrderBonus(D2Civilization1State state) => GetLevel(state, AcolyteOrderId) * AcolyteOrderBonusPerLevel;
    public static double GetLuminousEssenceBonus(D2Civilization1State state) => GetLevel(state, SanctuaryEchoId) * SanctuaryEchoBonusPerLevel;
    public static double GetTraceBonus(D2Civilization1State state) => GetLevel(state, TraceLiturgyId) * TraceLiturgyBonusPerLevel;

    public static string GetDisplayName(string lineId)
    {
        switch (lineId)
        {
            case PilgrimPathId: return "Camino Peregrino";
            case SacredCraftId: return "Oficio Sagrado";
            case AcolyteOrderId: return "Orden de Acólitos";
            case SanctuaryEchoId: return "Eco del Santuario";
            case TraceLiturgyId: return "Liturgia de Trazas";
            default: return "Línea desconocida";
        }
    }

    public static string GetEffectDescription(string lineId)
    {
        switch (lineId)
        {
            case PilgrimPathId: return "+5% llegada y recompensas materiales por nivel";
            case SacredCraftId: return "+7.5% producción de Altares por nivel";
            case AcolyteOrderId: return "+5% formación y potencia de Ritos por nivel";
            case SanctuaryEchoId: return "+1% producción de LE por nivel";
            case TraceLiturgyId: return "+1% producción de Trazas por nivel";
            default: return "Sin efecto";
        }
    }

    public static bool ValidateState(D2Civilization1State state, out string result)
    {
        if (state == null)
        {
            result = "Estado del Lugar de Vínculo ausente.";
            return false;
        }
        EnsureState(state);
        if (state.bondLines.Count != LineIds.Length || state.acolytesAssignedToBond < 0L ||
            state.bondProgress < 0.0 || (!state.entityContactAvailable && state.bondPlacePrepared))
        {
            result = "Estado del Lugar de Vínculo inválido.";
            return false;
        }
        result = "Estado del Lugar de Vínculo válido.";
        return true;
    }

    private static bool IsLineId(string id)
    {
        foreach (string lineId in LineIds)
            if (lineId == id) return true;
        return false;
    }

    private static D2BondLineState FindLine(D2Civilization1State state, string id)
    {
        if (state?.bondLines == null) return null;
        foreach (D2BondLineState line in state.bondLines)
            if (line != null && line.lineId == id) return line;
        return null;
    }

    private static bool HasEarlierDuplicate(D2Civilization1State state, int index)
    {
        for (int i = 0; i < index; i++)
            if (state.bondLines[i]?.lineId == state.bondLines[index]?.lineId) return true;
        return false;
    }

    private static bool HasOffering(D2Civilization1State state, string altarId, double amount)
    {
        D2AltarState altar = D2AltarSystem.GetAltar(state, altarId);
        return altar != null && altar.unlocked && altar.offeringAmount >= amount;
    }

    private static void SpendOffering(D2Civilization1State state, string altarId, double amount)
    {
        D2AltarSystem.GetAltar(state, altarId).offeringAmount -= amount;
    }

    private static long SaturatingAdd(long left, long right)
    {
        left = Math.Max(0L, left);
        right = Math.Max(0L, right);
        return left > long.MaxValue - right ? long.MaxValue : left + right;
    }
}

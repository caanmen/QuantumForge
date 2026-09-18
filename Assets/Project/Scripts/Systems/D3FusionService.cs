using System;
using UnityEngine;


/// <summary>
/// API de fusión independiente de la UI. La automatización usa el modo seguro
/// normal: consume los mismos fragmentos y concede los mismos resultados base.
/// </summary>
public static class D3FusionService
{
    public const double StableReactionThreshold = 0.30;
    public const double StableFailureReductionRate = 0.20;
    public const double StableMinorConversionRate = 0.25;

    public static double ApplyStableReactionChamberReduction(
        double rawFailureChance, bool stableChamberActive)
    {
        double clamped = Math.Max(0.0, Math.Min(1.0, rawFailureChance));
        if (!stableChamberActive) return clamped;
        double excess = Math.Max(0.0, clamped - StableReactionThreshold);
        return Math.Max(0.0, Math.Min(
            1.0, clamped - excess * StableFailureReductionRate));
    }

    public static double GetStableMinorConversionChance(
        double rawFailureChance, double rolledFailureChance,
        bool stableChamberActive)
    {
        if (!stableChamberActive || rolledFailureChance <= 0.0) return 0.0;
        double clamped = Math.Max(0.0, Math.Min(1.0, rawFailureChance));
        double excess = Math.Max(0.0, clamped - StableReactionThreshold);
        double convertedProbability = excess * StableMinorConversionRate;
        return Math.Max(0.0, Math.Min(
            1.0, convertedProbability / rolledFailureChance));
    }

    public static bool TryParseRecipeId(
        string recipeId, out ExperimentalFragmentType fragmentA,
        out ExperimentalFragmentType fragmentB,
        out ExperimentalCatalystType catalyst)
    {
        fragmentA = ExperimentalFragmentType.None;
        fragmentB = ExperimentalFragmentType.None;
        catalyst = ExperimentalCatalystType.None;
        if (string.IsNullOrWhiteSpace(recipeId)) return false;
        string[] parts = recipeId.Split(':');
        if (parts.Length != 4 || parts[0] != "fusion" ||
            !int.TryParse(parts[1], out int a) ||
            !int.TryParse(parts[2], out int b) ||
            !int.TryParse(parts[3], out int c)) return false;
        if (!Enum.IsDefined(typeof(ExperimentalFragmentType), a) ||
            !Enum.IsDefined(typeof(ExperimentalFragmentType), b) ||
            !Enum.IsDefined(typeof(ExperimentalCatalystType), c)) return false;
        fragmentA = (ExperimentalFragmentType)a;
        fragmentB = (ExperimentalFragmentType)b;
        catalyst = (ExperimentalCatalystType)c;
        return fragmentA != ExperimentalFragmentType.None &&
               fragmentB != ExperimentalFragmentType.None &&
               catalyst != ExperimentalCatalystType.None;
    }

    public static bool HasRequiredFragments(
        GameState gameState, ExperimentalFragmentType fragmentA,
        ExperimentalFragmentType fragmentB)
    {
        if (gameState == null || fragmentA == ExperimentalFragmentType.None ||
            fragmentB == ExperimentalFragmentType.None) return false;
        if (fragmentA == fragmentB)
            return gameState.GetFragmentCount(fragmentA) >= 2;
        return gameState.GetFragmentCount(fragmentA) >= 1 &&
               gameState.GetFragmentCount(fragmentB) >= 1;
    }

    public static ExperimentalResultType ResolveRecipeResult(
        ExperimentalFragmentType fragmentA,
        ExperimentalFragmentType fragmentB,
        ExperimentalCatalystType catalyst)
    {
        if (fragmentA == ExperimentalFragmentType.None ||
            fragmentB == ExperimentalFragmentType.None ||
            catalyst == ExperimentalCatalystType.None)
            return ExperimentalResultType.None;
        int a = (int)fragmentA;
        int b = (int)fragmentB;
        if (a > b) { int swap = a; a = b; b = swap; }
        ExperimentalFragmentType left = (ExperimentalFragmentType)a;
        ExperimentalFragmentType right = (ExperimentalFragmentType)b;
        if (left == ExperimentalFragmentType.Condensation &&
            right == ExperimentalFragmentType.Condensation)
            return catalyst == ExperimentalCatalystType.Alpha
                ? ExperimentalResultType.CompuestoUtil
                : ExperimentalResultType.Muestra;
        if (left == ExperimentalFragmentType.Condensation &&
            right == ExperimentalFragmentType.Confinement)
            return catalyst == ExperimentalCatalystType.Alpha
                ? ExperimentalResultType.Hallazgo
                : ExperimentalResultType.LecturaIncompleta;
        if (left == ExperimentalFragmentType.Condensation &&
            right == ExperimentalFragmentType.ResidualInterference)
            return catalyst == ExperimentalCatalystType.Alpha
                ? ExperimentalResultType.Muestra
                : ExperimentalResultType.Hallazgo;
        if (left == ExperimentalFragmentType.Confinement &&
            right == ExperimentalFragmentType.Confinement)
            return catalyst == ExperimentalCatalystType.Alpha
                ? ExperimentalResultType.CompuestoUtil
                : ExperimentalResultType.Muestra;
        if (left == ExperimentalFragmentType.Confinement &&
            right == ExperimentalFragmentType.ResidualInterference)
            return catalyst == ExperimentalCatalystType.Alpha
                ? ExperimentalResultType.Hallazgo
                : ExperimentalResultType.LecturaIncompleta;
        if (left == ExperimentalFragmentType.ResidualInterference &&
            right == ExperimentalFragmentType.ResidualInterference)
            return catalyst == ExperimentalCatalystType.Alpha
                ? ExperimentalResultType.Muestra
                : ExperimentalResultType.LecturaIncompleta;
        return ExperimentalResultType.None;
    }

    public static bool TryExecuteMarkedSafeFusion(
        GameState gameState, string recipeId,
        out ExperimentalResultType result, out string reason)
    {
        result = ExperimentalResultType.None;
        reason = "";
        if (gameState == null || gameState.dimension3 == null)
        { reason = "Estado de juego no disponible."; return false; }
        D3MarkedFusionRecipeState mark =
            D3DiagnosticSystem.FindMarkedRecipe(gameState.dimension3, recipeId);
        if (mark == null || !mark.manuallyExecuted || !mark.automationMarked)
        { reason = "La receta no fue ejecutada y marcada manualmente."; return false; }
        if (!gameState.experimentalChamberUnlocked || MachineManager.I == null ||
            !MachineManager.I.MachineFusionPanelUnlocked ||
            MachineManager.I.GetUnlockedFusionSlotCount() <= 0)
        { reason = "El panel de fusión no está disponible."; return false; }
        if (!TryParseRecipeId(recipeId, out ExperimentalFragmentType a,
                out ExperimentalFragmentType b,
                out ExperimentalCatalystType catalyst))
        { reason = "ID de receta inválido."; return false; }
        if (!HasRequiredFragments(gameState, a, b))
        { reason = "No hay fragmentos suficientes."; return false; }

        result = ResolveRecipeResult(a, b, catalyst);
        double beforeStable = GetBaseFailureChance(a, b);
        double failureChance = beforeStable;
        failureChance -= MachineManager.I.GetTotalEffectValue(
            MachineNodeEffectType.FusionFailureReduction);
        if (MachineManager.I.GetTotalEffectValue(
                MachineNodeEffectType.CatalystTuning) > 0.0 &&
            catalyst == ExperimentalCatalystType.Beta)
            failureChance -= 0.05;
        failureChance = Math.Max(0.0, Math.Min(1.0, failureChance));
        beforeStable = failureChance;
        bool stable = MachineManager.I.GetTotalEffectValue(
            MachineNodeEffectType.StableReactionChamber) > 0.0;
        failureChance = ApplyStableReactionChamberReduction(
            failureChance, stable);
        bool hasCore = MachineManager.I.GetTotalEffectValue(
            MachineNodeEffectType.SynthesisCore) > 0.0;
        bool coreCharged = hasCore && gameState.synthesisCoreFusionCounter >= 10;
        if (coreCharged) failureChance *= 0.75;
        failureChance = Math.Max(0.03, Math.Min(1.0, failureChance));
        bool failed = UnityEngine.Random.value < failureChance;
        if (failed && UnityEngine.Random.value <
            GetStableMinorConversionChance(
                beforeStable, failureChance, stable))
        {
            result = ExperimentalResultType.LecturaIncompleta;
            failed = false;
        }
        if (failed) result = ExperimentalResultType.None;

        gameState.ConsumeFragment(a, 1);
        gameState.ConsumeFragment(b, 1);
        if (!failed && result != ExperimentalResultType.None)
        {
            int reward = 1;
            double bonusChance = MachineManager.I.GetTotalEffectValue(
                MachineNodeEffectType.FusionUsefulResultBonus) +
                MachineManager.I.GetZoneProgressSyncBonus(
                    MachineZoneType.FusionSector);
            if (MachineManager.I.GetTotalEffectValue(
                    MachineNodeEffectType.CatalystTuning) > 0.0 &&
                catalyst == ExperimentalCatalystType.Alpha)
                bonusChance += 0.05;
            if (coreCharged) bonusChance += 0.15;
            if (bonusChance > 0.0 && UnityEngine.Random.value < bonusChance)
                reward++;
            gameState.AddExperimentalResult(result, reward);
        }
        RegisterMixResult(gameState, a, b, catalyst, result);
        if (hasCore)
            gameState.synthesisCoreFusionCounter = coreCharged
                ? 0 : gameState.synthesisCoreFusionCounter + 1;
        reason = failed ? "Fusión automática inestable; insumos consumidos." :
            "Fusión automática completada.";
        return true;
    }

    public static void RegisterMixResult(
        GameState gameState, ExperimentalFragmentType fragmentA,
        ExperimentalFragmentType fragmentB,
        ExperimentalCatalystType catalyst, ExperimentalResultType result)
    {
        if (gameState == null) return;
        ExperimentalMixLogEntry entry = gameState.GetOrCreateExperimentalMixEntry(
            fragmentA, fragmentB, catalyst);
        entry.fragmentA = (int)fragmentA;
        entry.fragmentB = (int)fragmentB;
        entry.catalyst = (int)catalyst;
        entry.lastResult = (int)result;
        if ((int)result > entry.bestResult) entry.bestResult = (int)result;
        entry.timesExecuted += 1;
        bool newlyDiscovered = result != ExperimentalResultType.None && !entry.discovered;
        if (result != ExperimentalResultType.None) entry.discovered = true;
        if (newlyDiscovered) entry.unreadDiscovery = true;
    }

    private static double GetBaseFailureChance(
        ExperimentalFragmentType fragmentA, ExperimentalFragmentType fragmentB)
    {
        int a = (int)fragmentA;
        int b = (int)fragmentB;
        if (a > b) { int swap = a; a = b; b = swap; }
        ExperimentalFragmentType left = (ExperimentalFragmentType)a;
        ExperimentalFragmentType right = (ExperimentalFragmentType)b;
        if (left == ExperimentalFragmentType.ResidualInterference &&
            right == ExperimentalFragmentType.ResidualInterference) return 0.30;
        return left == right ? 0.05 : 0.15;
    }
}

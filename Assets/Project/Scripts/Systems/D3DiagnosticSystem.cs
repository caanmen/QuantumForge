using System;
using System.Collections.Generic;


public static class D3DiagnosticSystem
{
    private const double BaseEvaluationSeconds = 1.0;

    public static void NormalizeSettings(Dimension3State state)
    {
        if (state == null) return;
        if (state.diagnosticSettings == null)
            state.diagnosticSettings = new D3DiagnosticSettingsState();
        NormalizeSettings(state.diagnosticSettings);
    }

    public static void NormalizeSettings(D3DiagnosticSettingsState settings)
    {
        if (settings == null) return;
        settings.priorityMode = Math.Max(0, Math.Min(1, settings.priorityMode));
        settings.priorityZone = Math.Max(0, Math.Min(4, settings.priorityZone));
        settings.leReserve = Math.Max(0.0, settings.leReserve);
        settings.tracesReserve = Math.Max(0.0, settings.tracesReserve);
        settings.nextFusionRecipeIndex = Math.Max(0, settings.nextFusionRecipeIndex);
        if (settings.savedRoutine == null)
            settings.savedRoutine = new D3DiagnosticRoutineState();
        if (settings.savedRoutine.markedRecipeIds == null)
            settings.savedRoutine.markedRecipeIds = new List<string>();
    }

    public static string GetRecipeId(
        ExperimentalFragmentType fragmentA,
        ExperimentalFragmentType fragmentB,
        ExperimentalCatalystType catalyst)
    {
        return "fusion:" + (int)fragmentA + ":" + (int)fragmentB + ":" +
               (int)catalyst;
    }

    public static void RegisterManualFusionRecipe(
        GameState gameState, ExperimentalFragmentType fragmentA,
        ExperimentalFragmentType fragmentB, ExperimentalCatalystType catalyst)
    {
        if (gameState == null || fragmentA == ExperimentalFragmentType.None ||
            fragmentB == ExperimentalFragmentType.None ||
            catalyst == ExperimentalCatalystType.None) return;
        Dimension3System.EnsureState(gameState);
        string recipeId = GetRecipeId(fragmentA, fragmentB, catalyst);
        D3MarkedFusionRecipeState recipe = FindMarkedRecipe(
            gameState.dimension3, recipeId);
        if (recipe == null)
        {
            recipe = new D3MarkedFusionRecipeState { recipeId = recipeId };
            gameState.dimension3.markedFusionRecipes.Add(recipe);
        }
        recipe.manuallyExecuted = true;
    }

    public static bool TrySetRecipeAutomationMark(
        GameState gameState, string recipeId, bool marked, out string reason)
    {
        reason = "";
        if (gameState == null || string.IsNullOrWhiteSpace(recipeId))
        { reason = "Receta inválida."; return false; }
        Dimension3System.EnsureState(gameState);
        D3MarkedFusionRecipeState recipe = FindMarkedRecipe(
            gameState.dimension3, recipeId);
        if (recipe == null || !recipe.manuallyExecuted)
        { reason = "La receta debe ejecutarse manualmente al menos una vez."; return false; }
        recipe.automationMarked = marked;
        return true;
    }

    public static void Tick(GameState gameState, double seconds, bool offline)
    {
        if (gameState == null || gameState.dimension3 == null || seconds <= 0.0 ||
            double.IsNaN(seconds) || double.IsInfinity(seconds)) return;
        NormalizeSettings(gameState.dimension3);
        D3DiagnosticSettingsState settings = gameState.dimension3.diagnosticSettings;
        if (offline && !CanRunOffline(gameState))
        {
            settings.evaluationRemainingSeconds = GetEvaluationInterval(
                gameState.dimension3);
            return;
        }
        if (offline && MachineManager.I != null)
            MachineManager.I.ApplyOfflineAnalysis(seconds);
        settings.evaluationRemainingSeconds -= seconds;
        int safety = 0;
        while (settings.evaluationRemainingSeconds <= 0.0 && safety++ < 1000)
        {
            settings.evaluationRemainingSeconds += GetEvaluationInterval(
                gameState.dimension3);
            EvaluateOnce(gameState);
        }
    }

    public static double GetEvaluationInterval(Dimension3State state)
    {
        double coordination = Math.Sqrt(D3PowerSystem.GetRawChannelPower(
            state, Dimension3Catalog.FacilityDiagnosticBank,
            Dimension3Catalog.ChannelDiagnosticCoordination)) * 0.25;
        double coreMultiplier = D3FacilitySystem
            .GetAutomationCoreEfficiencyMultiplier(state);
        double coordinationMultiplier = 1.0 + coordination * coreMultiplier / 100.0;
        double response = Math.Sqrt(D3PowerSystem.GetRawChannelPower(
            state, Dimension3Catalog.FacilityDiagnosticBank,
            Dimension3Catalog.ChannelDiagnosticResponse)) * 0.15 *
            coordinationMultiplier * coreMultiplier;
        return Math.Max(0.2, BaseEvaluationSeconds / (1.0 + response / 100.0));
    }

    public static bool EvaluateOnce(GameState gameState)
    {
        if (gameState == null || gameState.dimension3 == null ||
            MachineManager.I == null) return false;
        D3DiagnosticSettingsState settings = gameState.dimension3.diagnosticSettings;
        if (settings.autoAnalyzeEnabled && D3FacilitySystem.IsFunctionActive(
                gameState.dimension3, Dimension3Catalog.FacilityDiagnosticBank, 1) &&
            TryStartNextAnalysis(gameState, out _)) return true;
        if (settings.autoRepairEnabled && D3FacilitySystem.IsFunctionActive(
                gameState.dimension3, Dimension3Catalog.FacilityDiagnosticBank, 2) &&
            TryRepairNextNode(gameState, out _)) return true;
        if (settings.autoFusionEnabled && D3FacilitySystem.IsFunctionActive(
                gameState.dimension3, Dimension3Catalog.FacilityDiagnosticBank, 4) &&
            TryRepeatNextMarkedFusion(gameState, out _)) return true;
        return false;
    }

    public static bool CanRunOffline(GameState gameState)
    {
        return gameState != null && gameState.dimension3 != null &&
            D3FacilitySystem.IsFunctionActive(gameState.dimension3,
                Dimension3Catalog.FacilityDiagnosticBank, 5) &&
            D3FacilitySystem.IsFunctionActive(gameState.dimension3,
                Dimension3Catalog.FacilityAutomationCore, 5);
    }

    public static bool TryConfigure(
        GameState gameState, int priorityMode, int priorityZone,
        double leReserve, double tracesReserve, out string reason)
    {
        reason = "";
        if (gameState == null || gameState.dimension3 == null)
        { reason = "Estado de Dimensión 3 no disponible."; return false; }
        if (!D3FacilitySystem.IsFunctionActive(gameState.dimension3,
                Dimension3Catalog.FacilityDiagnosticBank, 3))
        { reason = "El Banco de Diagnóstico N3 no está activo."; return false; }
        D3DiagnosticSettingsState settings = gameState.dimension3.diagnosticSettings;
        settings.priorityMode = Math.Max(0, Math.Min(1, priorityMode));
        settings.priorityZone = Math.Max(0, Math.Min(4, priorityZone));
        settings.leReserve = Math.Max(0.0, leReserve);
        settings.tracesReserve = Math.Max(0.0, tracesReserve);
        return true;
    }

    public static bool TryStartNextAnalysis(GameState gameState, out string reason)
    {
        reason = "No hay nodos válidos para analizar.";
        if (gameState == null || MachineManager.I == null) return false;
        List<MachineNodeDef> nodes = GetOrderedNodes(gameState);
        for (int i = 0; i < nodes.Count; i++)
        {
            MachineNodeDef node = nodes[i];
            if (MachineManager.I.TryStartNodeAnalysis(
                    node.id, MachineManager.BaseNodeAnalysisDurationSeconds,
                    true, out string candidateReason)) return true;
            reason = candidateReason;
        }
        return false;
    }

    public static bool TryRepairNextNode(GameState gameState, out string reason)
    {
        reason = "No hay nodos válidos para reparar.";
        if (gameState == null || gameState.dimension3 == null ||
            MachineManager.I == null) return false;
        D3DiagnosticSettingsState settings = gameState.dimension3.diagnosticSettings;
        if (gameState.LE < settings.leReserve ||
            gameState.Traces < settings.tracesReserve)
        { reason = "Las reservas configuradas impiden la reparación."; return false; }
        List<MachineNodeDef> nodes = GetOrderedNodes(gameState);
        for (int i = 0; i < nodes.Count; i++)
        {
            MachineNodeDef node = nodes[i];
            if (node.hidden || string.IsNullOrWhiteSpace(node.tierGroup)) continue;
            if (!MachineManager.I.CanRepairNode(node.id, out reason)) continue;
            MachineNodeCostDef cost = MachineManager.I.GetEffectiveNodeCost(node);
            if (cost == null || gameState.LE - cost.le < settings.leReserve ||
                gameState.Traces - cost.traces < settings.tracesReserve)
            { reason = "La reparación bajaría de las reservas configuradas."; continue; }
            return MachineManager.I.TryRepairNode(node.id);
        }
        return false;
    }

    public static bool TryRepeatNextMarkedFusion(
        GameState gameState, out string reason)
    {
        reason = "No hay recetas manuales marcadas con recursos disponibles.";
        if (gameState == null || gameState.dimension3 == null) return false;
        List<D3MarkedFusionRecipeState> recipes =
            gameState.dimension3.markedFusionRecipes;
        if (recipes == null || recipes.Count == 0) return false;
        D3DiagnosticSettingsState settings = gameState.dimension3.diagnosticSettings;
        int count = recipes.Count;
        int start = settings.nextFusionRecipeIndex % count;
        for (int offset = 0; offset < count; offset++)
        {
            int index = (start + offset) % count;
            D3MarkedFusionRecipeState recipe = recipes[index];
            if (recipe == null || !recipe.manuallyExecuted ||
                !recipe.automationMarked) continue;
            if (D3FusionService.TryExecuteMarkedSafeFusion(
                    gameState, recipe.recipeId, out _, out string candidateReason))
            {
                settings.nextFusionRecipeIndex = (index + 1) % count;
                reason = candidateReason;
                return true;
            }
            reason = candidateReason;
        }
        return false;
    }

    public static bool TrySaveRoutine(GameState gameState, out string reason)
    {
        reason = "";
        if (gameState == null || gameState.dimension3 == null ||
            !D3FacilitySystem.IsFunctionActive(gameState.dimension3,
                Dimension3Catalog.FacilityDiagnosticBank, 5))
        { reason = "El Banco de Diagnóstico N5 no está activo."; return false; }
        D3DiagnosticSettingsState settings = gameState.dimension3.diagnosticSettings;
        D3DiagnosticRoutineState saved = new D3DiagnosticRoutineState
        {
            saved = true,
            autoAnalyzeEnabled = settings.autoAnalyzeEnabled,
            autoRepairEnabled = settings.autoRepairEnabled,
            autoFusionEnabled = settings.autoFusionEnabled,
            priorityMode = settings.priorityMode,
            priorityZone = settings.priorityZone,
            leReserve = settings.leReserve,
            tracesReserve = settings.tracesReserve
        };
        for (int i = 0; i < gameState.dimension3.markedFusionRecipes.Count; i++)
        {
            D3MarkedFusionRecipeState recipe =
                gameState.dimension3.markedFusionRecipes[i];
            if (recipe != null && recipe.manuallyExecuted &&
                recipe.automationMarked)
                saved.markedRecipeIds.Add(recipe.recipeId);
        }
        settings.savedRoutine = saved;
        return true;
    }

    public static bool TryLoadRoutine(GameState gameState, out string reason)
    {
        reason = "";
        if (gameState == null || gameState.dimension3 == null)
        { reason = "Estado de Dimensión 3 no disponible."; return false; }
        D3DiagnosticSettingsState settings = gameState.dimension3.diagnosticSettings;
        D3DiagnosticRoutineState saved = settings.savedRoutine;
        if (saved == null || !saved.saved)
        { reason = "No hay una rutina diagnóstica guardada."; return false; }
        settings.autoAnalyzeEnabled = saved.autoAnalyzeEnabled;
        settings.autoRepairEnabled = saved.autoRepairEnabled;
        settings.autoFusionEnabled = saved.autoFusionEnabled;
        settings.priorityMode = saved.priorityMode;
        settings.priorityZone = saved.priorityZone;
        settings.leReserve = saved.leReserve;
        settings.tracesReserve = saved.tracesReserve;
        for (int i = 0; i < gameState.dimension3.markedFusionRecipes.Count; i++)
        {
            D3MarkedFusionRecipeState recipe =
                gameState.dimension3.markedFusionRecipes[i];
            if (recipe == null || !recipe.manuallyExecuted) continue;
            recipe.automationMarked = saved.markedRecipeIds != null &&
                saved.markedRecipeIds.Contains(recipe.recipeId);
        }
        NormalizeSettings(gameState.dimension3);
        return true;
    }

    private static List<MachineNodeDef> GetOrderedNodes(GameState gameState)
    {
        List<MachineNodeDef> nodes = MachineManager.I.GetAllNodes(false);
        D3DiagnosticSettingsState settings = gameState.dimension3.diagnosticSettings;
        int preferredZone = settings.priorityZone;
        nodes.Sort((a, b) =>
        {
            int aPriority = settings.priorityMode == 1 &&
                (int)a.zone == preferredZone ? 0 : 1;
            int bPriority = settings.priorityMode == 1 &&
                (int)b.zone == preferredZone ? 0 : 1;
            int comparison = aPriority.CompareTo(bPriority);
            if (comparison != 0) return comparison;
            comparison = ((int)a.zone).CompareTo((int)b.zone);
            if (comparison != 0) return comparison;
            comparison = a.tierIndex.CompareTo(b.tierIndex);
            return comparison != 0 ? comparison : string.CompareOrdinal(a.id, b.id);
        });
        return nodes;
    }

    public static D3MarkedFusionRecipeState FindMarkedRecipe(
        Dimension3State state, string recipeId)
    {
        if (state == null || state.markedFusionRecipes == null) return null;
        for (int i = 0; i < state.markedFusionRecipes.Count; i++)
        {
            D3MarkedFusionRecipeState recipe = state.markedFusionRecipes[i];
            if (recipe != null && recipe.recipeId == recipeId) return recipe;
        }
        return null;
    }
}

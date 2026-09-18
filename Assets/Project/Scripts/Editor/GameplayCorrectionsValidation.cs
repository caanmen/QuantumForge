#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class GameplayCorrectionsValidation
{
    [MenuItem("Tools/Quantum Forge/QA/Validate Gameplay Corrections Contract")]
    public static void Validate()
    {
        ValidateArtifactEconomy();
        ValidateMachineNodeProgression();
        ValidateFusionContracts();
        Debug.Log("[Gameplay Corrections] CONTRACT_PASS | economia | hitos | " +
            "ramas | analisis selectivo | mezclas");
    }

    public static void ValidateBatch()
    {
        try
        {
            Validate();
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    private static void ValidateArtifactEconomy()
    {
        TextAsset source = Resources.Load<TextAsset>("Data/buildings");
        Require(source != null, "Falta Data/buildings.");
        BuildingCollection catalog = JsonUtility.FromJson<BuildingCollection>(source.text);
        BuildingDef higgs = catalog?.buildings?.Find(value =>
            value != null && value.id == "vacuum_observer");
        BuildingDef tetra = catalog?.buildings?.Find(value =>
            value != null && value.id == "casimir_panel");
        Require(higgs != null && tetra != null,
            "Faltan los dos artefactos iniciales.");

        foreach (BuildingDef definition in new[] { higgs, tetra })
        {
            for (int level = 0; level <= 25; level++)
            {
                double legacy = definition.baseCost *
                    Math.Pow(definition.costMult, level);
                Near(BuildingState.CalculateCostForLevel(definition, level), legacy,
                    "La curva temprana cambió en " + definition.id +
                    " nivel " + level + ".");
            }
            double previous = 0.0;
            for (int level = 0; level <= 260; level++)
            {
                double current = BuildingState.CalculateCostForLevel(definition, level);
                Require(current >= previous,
                    "El coste dejó de ser monótono en " + definition.id + ".");
                previous = current;
            }
        }

        Require(GameState.GetArtifactLevelMilestoneMultiplier(higgs.id, 9) == 1.0 &&
                GameState.GetArtifactLevelMilestoneMultiplier(higgs.id, 10) == 2.0 &&
                GameState.GetArtifactLevelMilestoneMultiplier(higgs.id, 25) == 3.0 &&
                GameState.GetArtifactLevelMilestoneMultiplier(higgs.id, 50) == 4.0 &&
                GameState.GetArtifactLevelMilestoneMultiplier(higgs.id, 100) == 5.0 &&
                GameState.GetArtifactLevelMilestoneMultiplier(higgs.id, 200) == 6.0,
            "Los hitos de artefactos no conservan la progresión aprobada.");
        Require(GameState.GetArtifactLevelMilestoneMultiplier(
                    "fluctuation_antenna", 200) == 1.0,
            "El Modulador de Energía no debe recibir hitos de producción.");
        Near(GameState.TriangleEnergyPerGeneratorLevel, 0.05,
            "Cada nivel adicional del Modulador debe aportar +0.05 Energía/s.");
        Near(MachineManager.NodeAnalysisEnergyCost, 25.0,
            "El diagnóstico debe costar 25 Energía.");
        Near(UpgradeStudySystem.GetTuningBaseBoostFraction(0.90), 0.18,
            "La sintonización válida ordinaria debe aplicar 18%.");
        Near(UpgradeStudySystem.GetTuningBaseBoostFraction(0.9799), 0.18,
            "La sintonización por debajo de 98% no debe ser perfecta.");
        Near(UpgradeStudySystem.GetTuningBaseBoostFraction(0.98), 0.25,
            "La calibración perfecta debe comenzar en 98% y aplicar 25%.");

        int higgsLevel = 0;
        int tetraLevel = 0;
        double le = higgs.baseCost;
        double longestWait = 0.0;
        for (int purchase = 0; purchase < 260; purchase++)
        {
            double higgsCost = BuildingState.CalculateCostForLevel(higgs, higgsLevel);
            double tetraCost = higgsLevel > 0
                ? BuildingState.CalculateCostForLevel(tetra, tetraLevel)
                : double.PositiveInfinity;
            bool buyHiggs = higgsCost <= tetraCost;
            double cost = buyHiggs ? higgsCost : tetraCost;
            double production = higgs.lePerTickBase * higgsLevel *
                GameState.GetArtifactLevelMilestoneMultiplier(higgs.id, higgsLevel) +
                tetra.lePerTickBase * tetraLevel *
                GameState.GetArtifactLevelMilestoneMultiplier(tetra.id, tetraLevel);
            double wait = le >= cost ? 0.0 : (cost - le) / Math.Max(.000001, production);
            longestWait = Math.Max(longestWait, wait);
            le += production * wait - cost;
            if (buyHiggs) higgsLevel++;
            else tetraLevel++;
        }
        Require(longestWait >= 900.0 && longestWait < 1050.0,
            "La curva balanceada salió del intervalo intencional de 15 a " +
            "17.5 minutos: " +
            longestWait.ToString("0.0") + " s.");

        BuildingState immediate = new BuildingState();
        immediate.InitFromDef(higgs);
        double firstCost = immediate.currentCost;
        immediate.OnPurchased();
        Require(immediate.level == 1 && immediate.currentCost > firstCost,
            "Comprar un artefacto dejó de ser inmediato.");
    }

    private static void ValidateMachineNodeProgression()
    {
        TextAsset source = Resources.Load<TextAsset>("Data/machine_nodes");
        Require(source != null, "Falta Data/machine_nodes.");
        MachineNodeDefList catalog = JsonUtility.FromJson<MachineNodeDefList>(source.text);
        List<MachineNodeDef> active = catalog != null && catalog.nodes != null
            ? catalog.nodes.Where(node => node != null && !node.retired).ToList()
            : null;
        Require(active != null && active.Count > 0,
            "No se cargaron nodos activos de la Máquina.");

        string[] expectedAnalysisNodes =
        {
            "z1_protocol_reading", "z1_artifact_calibration",
            "z1_room1_synchronizer", "z2_composition_reading",
            "z2_stable_reaction_chamber", "z2_guided_synthesis",
            "z2_synthesis_core", "z3_machine_memory"
        };
        string[] actualAnalysisNodes = active
            .Where(node => !node.hidden && node.damaged)
            .Select(node => node.id).OrderBy(id => id).ToArray();
        Require(actualAnalysisNodes.SequenceEqual(expectedAnalysisNodes.OrderBy(id => id)),
            "El análisis no quedó limitado a los nodos significativos activos.");

        string[] freeBranchRoots =
        {
            "z1_triangle_anchor_1", "z2_mix_stabilizer_1",
            "z2_residual_catalyst_1", "z2_fusion_time_control_1"
        };
        foreach (string id in freeBranchRoots)
        {
            MachineNodeDef node = active.Find(value => value.id == id);
            Require(node != null && (node.requiredNodeIds == null ||
                    node.requiredNodeIds.Count == 0),
                id + " sigue bloqueado por otra rama opcional.");
        }

        foreach (IGrouping<string, MachineNodeDef> group in active
            .Where(node => !string.IsNullOrWhiteSpace(node.tierGroup))
            .GroupBy(node => node.tierGroup))
        {
            foreach (MachineNodeDef node in group.Where(node => node.tierIndex > 1))
            {
                MachineNodeDef previous = group.FirstOrDefault(candidate =>
                    candidate.tierIndex == node.tierIndex - 1);
                Require(previous != null && node.requiredNodeIds != null &&
                        node.requiredNodeIds.Contains(previous.id),
                    node.id + " rompió el orden de su cadena de tiers.");
            }
        }

        Require(Requires(active, "z2_fusion_slot_3", "z2_composition_reading") &&
                HasRequirements(active, "z2_synthesis_core") &&
                HasRequirements(active, "z3_structural_reinforcement"),
            "Un nodo estructural o final perdió su puerta de progresión.");
    }

    private static void ValidateFusionContracts()
    {
        Require(GameState.FusionInstabilityMax == 20,
            "La inestabilidad dejó de estar limitada a 20.");
        ExperimentalPendingFusionState pending = new ExperimentalPendingFusionState();
        Require(!pending.active,
            "Una mezcla nueva no debe empezar marcada como pendiente.");

        ExperimentalFragmentType[] fragments =
        {
            ExperimentalFragmentType.Condensation,
            ExperimentalFragmentType.Confinement,
            ExperimentalFragmentType.ResidualInterference
        };
        ExperimentalCatalystType[] catalysts =
        {
            ExperimentalCatalystType.Alpha,
            ExperimentalCatalystType.Beta
        };
        int recipeCount = 0;
        for (int left = 0; left < fragments.Length; left++)
        {
            for (int right = left; right < fragments.Length; right++)
            {
                foreach (ExperimentalCatalystType catalyst in catalysts)
                {
                    Require(D3FusionService.ResolveRecipeResult(
                            fragments[left], fragments[right], catalyst) !=
                            ExperimentalResultType.None,
                        "Una receta base no entrega un resultado visible.");
                    recipeCount++;
                }
            }
        }
        Require(recipeCount == 12,
            "El catálogo base no contiene las doce mezclas posibles.");

        ValidateStableChamberMonotonicity(1.0, "sin Núcleo");
        ValidateStableChamberMonotonicity(0.75, "con Núcleo cargado");
        Near(D3FusionService.GetStableMinorConversionChance(
                0.30, 0.30, true), 0.0,
            "Cámara Estable convierte fallos antes de superar 30%.");
        Require(D3FusionService.GetStableMinorConversionChance(
                    0.31, 0.308, true) > 0.0,
            "Cámara Estable no convierte una fracción progresiva después de 30%.");
        Near(D3FusionService.ApplyStableReactionChamberReduction(0.30, true),
            0.30, "Cámara Estable introduce un salto al llegar a 30%.");
        Require(D3FusionService.ApplyStableReactionChamberReduction(0.31, true) >
                D3FusionService.ApplyStableReactionChamberReduction(0.30, true),
            "El riesgo ajustado debe seguir creciendo por encima de 30%.");

        GameObject host = new GameObject("Gameplay Fusion Contract State");
        try
        {
            GameState state = host.AddComponent<GameState>();
            state.experimentalMixLog = new List<ExperimentalMixLogEntry>();
            state.AddExperimentalResult(
                ExperimentalResultType.LecturaIncompleta, 2);
            Require(state.experimentalLecturasIncompletas == 2,
                "Las lecturas incompletas no incrementan su contador.");

            D3FusionService.RegisterMixResult(state,
                ExperimentalFragmentType.Confinement,
                ExperimentalFragmentType.ResidualInterference,
                ExperimentalCatalystType.Beta,
                ExperimentalResultType.LecturaIncompleta);
            Require(state.GetUnreadExperimentalRecipeCount() == 1,
                "Una mezcla recién descubierta no activa el aviso del registro.");
            state.MarkExperimentalRecipesRead();
            Require(state.GetUnreadExperimentalRecipeCount() == 0,
                "Entrar al registro no limpia el aviso de mezcla nueva.");
            D3FusionService.RegisterMixResult(state,
                ExperimentalFragmentType.Confinement,
                ExperimentalFragmentType.ResidualInterference,
                ExperimentalCatalystType.Beta,
                ExperimentalResultType.LecturaIncompleta);
            Require(state.GetUnreadExperimentalRecipeCount() == 0,
                "Repetir una mezcla conocida vuelve a encender el aviso.");
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(host);
        }
    }

    private static void ValidateStableChamberMonotonicity(
        double finalFailureMultiplier, string context)
    {
        double previousEffectiveLoss = 0.0;
        for (int step = 0; step <= 1000; step++)
        {
            double raw = step / 1000.0;
            double rolled = Math.Max(0.03,
                D3FusionService.ApplyStableReactionChamberReduction(raw, true) *
                finalFailureMultiplier);
            double conversion = D3FusionService.GetStableMinorConversionChance(
                raw, rolled, true);
            Require(conversion >= 0.0 && conversion <= 1.0,
                "Probabilidad de conversión inválida " + context + ".");
            double effectiveLoss = rolled * (1.0 - conversion);
            Require(effectiveLoss + 1e-9 >= previousEffectiveLoss,
                "Cámara Estable vuelve no monótona la pérdida efectiva " +
                context + " en " + raw.ToString("0.000") + ".");
            previousEffectiveLoss = effectiveLoss;
        }
    }

    private static bool Requires(List<MachineNodeDef> nodes,
        string nodeId, string requirementId)
    {
        MachineNodeDef node = nodes.Find(value => value.id == nodeId);
        return node?.requiredNodeIds != null &&
            node.requiredNodeIds.Contains(requirementId);
    }

    private static bool HasRequirements(List<MachineNodeDef> nodes, string nodeId)
    {
        MachineNodeDef node = nodes.Find(value => value.id == nodeId);
        return node?.requiredNodeIds != null && node.requiredNodeIds.Count > 0;
    }

    private static void Near(double actual, double expected, string message)
    {
        double tolerance = Math.Max(1e-9, Math.Abs(expected) * 1e-10);
        Require(Math.Abs(actual - expected) <= tolerance, message);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
#endif

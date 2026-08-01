#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class PresentationBlockP3Validation
{
    [MenuItem("Tools/Quantum Forge/Presentation/Validate Block P3")]
    public static void ValidateBlockP3()
    {
        var failures = new List<string>();
        ValidateScene(failures);
        ValidateProgression(failures);
        ValidateAutomationLearning(failures);
        ValidateSafeMappings(failures);
        if (failures.Count == 0)
        {
            Debug.Log("[Presentation P3] PASS | D3-6 secuencia | D3-7 dropdown IDs | " +
                "D3-8 calibración gradual | D3-9 investigación contextual | " +
                "D3-10 instalaciones humanas | D3-11 manual antes de rutina");
            return;
        }
        Debug.LogError("[Presentation P3] FAIL\n- " + string.Join("\n- ", failures));
    }

    public static void ValidateP3AndRegressionsBatch()
    {
        EditorSceneManager.OpenScene(
            "Assets/Project/Scenes/Main.unity", OpenSceneMode.Single);
        ValidateBlockP3();
        PresentationBlockP2Validation.ValidateBlockP2();
        PresentationBlockP1Validation.ValidateBlockP1();
        Dimension3Block7FullValidation.ValidateFullThrough7D();
        Debug.Log("[Presentation P3 Full] FIN | revisar PASS/FAIL anteriores");
    }

    private static void ValidateScene(List<string> failures)
    {
        Dimension3PanelUI panel = UnityEngine.Object.FindFirstObjectByType<
            Dimension3PanelUI>(FindObjectsInactive.Include);
        Check(panel != null, "Main no contiene Dimension3PanelUI.", failures);
        if (panel == null) return;
        Check(panel.calibrationPanel != null && panel.researchPanel != null &&
            panel.facilitiesPanel != null && panel.queuesPanel != null,
            "Faltan referencias de paneles progresivos D3.", failures);
        Check(panel.productionVersionDropdown != null &&
            panel.assemblyMkDropdown != null && panel.assignmentMkDropdown != null,
            "Faltan dropdowns seguros de versión/MK.", failures);
    }

    private static void ValidateProgression(List<string> failures)
    {
        GameState state = CreateState("Presentation P3 Progression");
        try
        {
            int[] versions =
                D3ProgressivePresentationRules.GetUnlockedPartVersions(state);
            int[] mks = D3ProgressivePresentationRules.GetUnlockedAssemblyMks(state);
            Check(versions.Length == 1 && versions[0] == 1,
                "Una partida inicial expone versiones futuras.", failures);
            Check(mks.Length == 1 && mks[0] == 1,
                "Una partida inicial expone MK futuros.", failures);
            Check(!D3ProgressivePresentationRules.CanIntroduceCalibration(state),
                "Calibración aparece antes del primer MK1/segundo operador.", failures);

            D3InventorySystem.AddAutomatons(
                state.dimension3, 1, Dimension3Catalog.TraitNormal, 1L);
            D3PresentationRules.EnsurePresentationState(state);
            Check(D3PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D3Calibration).CanOpen,
                "Calibración no aparece tras el segundo operador.", failures);

            D3InventorySystem.AddAssemblyCount(state.dimension3, 1, 1L);
            FeaturePresentationState facilities = D3PresentationRules.GetFeatureState(
                state, PresentationFeatureIds.D3Facilities);
            Check(facilities.IsVisible,
                "Instalaciones no ofrece teaser tras varios progresos MK1.", failures);

            D3FacilityState bank = D3FacilitySystem.GetFacility(
                state.dimension3, Dimension3Catalog.FacilityProcessBank);
            bank.level = 4;
            bank.built = true;
            D3InventorySystem.AddAssemblyCount(state.dimension3, 3, 10L);
            D3PresentationRules.EnsurePresentationState(state);
            FeaturePresentationState research = D3PresentationRules.GetFeatureState(
                state, PresentationFeatureIds.D3Research);
            Check(research.CanOpen,
                "Investigación no pasa a Disponible con requisito real V4.", failures);

            D3QueueState queue = D3JobQueueSystem.GetQueue(
                state.dimension3, Dimension3Catalog.QueueResearch);
            queue.jobs.Add(new D3JobState
            {
                jobId = "p3_research_active",
                jobType = Dimension3Catalog.JobResearch,
                targetId = Dimension3Catalog.GetResearchId(
                    Dimension3Catalog.PartChassis, 4),
                version = 4,
                remainingSeconds = 5.0
            });
            Check(D3PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D3Research).visualState ==
                    FeaturePresentationVisualState.Active,
                "Investigación en curso no fuerza estado Active.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateAutomationLearning(List<string> failures)
    {
        GameState state = CreateState("Presentation P3 Automation");
        try
        {
            Check(D3ProgressivePresentationRules.GetLearnedAutomationActionIds(
                    state).Count == 0,
                "Automatización ofrece acciones antes del uso manual.", failures);
            state.dimension1ManualSimpleScanCompleted = true;
            List<string> learned =
                D3ProgressivePresentationRules.GetLearnedAutomationActionIds(state);
            Check(learned.Count == 1 &&
                    learned[0] == D3AutomationCatalog.ActionPortScan,
                "PATTERN LEARNED no filtra la acción manual exacta.", failures);
            Check(!D3ProgressivePresentationRules.CanCreateRoutine(
                    state, D3AutomationCatalog.ActionPortScan),
                "Permite crear rutina sin la instalación requerida.", failures);
            D3FacilityState port = D3FacilitySystem.GetFacility(
                state.dimension3, Dimension3Catalog.FacilityExpeditionPort);
            port.built = true;
            port.level = 1;
            Check(D3ProgressivePresentationRules.CanCreateRoutine(
                    state, D3AutomationCatalog.ActionPortScan),
                "CREATE ROUTINE no aparece con patrón e instalación válidos.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateSafeMappings(List<string> failures)
    {
        var map = new SafeDropdownOptionMap<string>(StringComparer.Ordinal);
        map.Rebuild(null, new[] { "known.a", "known.c" }, value => value, "known.c");
        Check(map.ResolveOrDefault(1, "fallback") == "known.c",
            "Dropdown visible no conserva ID estable.", failures);
        Check(map.ResolveOrDefault(99, "fallback") == "fallback",
            "Índice inválido no cae en fallback seguro.", failures);
    }

    private static GameState CreateState(string name)
    {
        var target = new GameObject(name) { hideFlags = HideFlags.HideAndDontSave };
        target.SetActive(false);
        GameState state = target.AddComponent<GameState>();
        state.dimension03Unlocked = true;
        state.dimension3 = Dimension3System.CreateInitialState();
        state.LE = 1000000000.0;
        state.Traces = 1000000000.0;
        Dimension3System.EnsureState(state);
        return state;
    }

    private static void Check(
        bool condition, string message, List<string> failures)
    {
        if (!condition) failures.Add(message);
    }
}
#endif

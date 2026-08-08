#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public static class PresentationBlockP7Validation
{
    public static void ValidateAwayLogBatch()
    {
        var failures = new List<string>();
        ValidateShortAndLongAbsence(failures);
        ValidateNewsLimitAndSingleAction(failures);
        ValidateReturnModalStructure(failures);
        ValidateLocalization(failures);
        if (failures.Count > 0)
            throw new System.InvalidOperationException(
                "[Away Log Validation] FAIL\n- " + string.Join("\n- ", failures));
        Debug.Log("[Away Log Validation] PASS | balance LE/Trazas/Energía | " +
            "tiempo base + D2/D3 | un solo Continuar | ES/EN");
    }

    [MenuItem("Tools/Quantum Forge/Presentation/Validate Block P7")]
    public static void ValidateBlockP7()
    {
        var failures = new List<string>();
        ValidateSceneHelp(failures);
        ValidateShortAndLongAbsence(failures);
        ValidateNewsLimitAndSingleAction(failures);
        ValidateReturnModalStructure(failures);
        ValidateAdvancedResume(failures);
        ValidateActiveWorkNavigation(failures);
        ValidateLocalization(failures);
        if (failures.Count == 0)
        {
            Debug.Log("[Presentation P7] PASS | bitácora offline | balance real | " +
                "un solo Continuar | ayuda | EN/ES | trabajos activos");
            return;
        }
        Debug.LogError("[Presentation P7] FAIL\n- " +
            string.Join("\n- ", failures));
    }

    public static void ValidateP7AndFullRegressionsBatch()
    {
        EditorSceneManager.OpenScene(
            "Assets/Project/Scenes/Main.unity", OpenSceneMode.Single);
        ValidateBlockP7();
        PresentationCorrectionsValidation.ValidateCorrections();
        PresentationBlockP6Validation.ValidateBlockP6();
        PresentationBlockP5Validation.ValidateBlockP5();
        PresentationBlockP4Validation.ValidateBlockP4();
        PresentationBlockP3Validation.ValidateBlockP3();
        PresentationBlockP2Validation.ValidateBlockP2();
        PresentationBlockP1Validation.ValidateBlockP1();
        ConvergenceFullValidation.ValidateFull();
        Debug.Log("[Presentation P7 Full] FIN | revisar PASS/FAIL anteriores");
    }

    private static void ValidateSceneHelp(List<string> failures)
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<
            Dimension2PanelUI>(FindObjectsInactive.Include);
        Check(panel != null && panel.contextualHelpButton != null &&
            panel.helpRoot != null && panel.helpTitleText != null &&
            panel.helpBodyText != null && panel.closeHelpButton != null,
            "Dimensión 2 no tiene AYUDA/REPASAR recuperable conectada.", failures);
    }

    private static void ValidateShortAndLongAbsence(List<string> failures)
    {
        GameState state = CreateState("Presentation P7 Absence");
        try
        {
            PresentationReturnSnapshot before =
                PresentationReturnReportService.Capture(state);
            PresentationReturnReport shortReport =
                PresentationReturnReportService.Build(before, state,
                    PresentationReturnReportService.SignificantAbsenceSeconds - 1.0,
                    120.0, 120.0);
            Check(shortReport == null,
                "Ausencia corta abre un informe obligatorio.", failures);
            PresentationReturnReport preparedShort =
                PresentationReturnReportService.Prepare(before, state,
                    PresentationReturnReportService.SignificantAbsenceSeconds - 1.0,
                    120.0, 120.0);
            Check(preparedShort == null &&
                    PresentationReturnReportService.UnifiedReportPreparedThisLoad &&
                    PresentationReturnReportService.Consume() == null,
                "Ausencia corta no suprime correctamente los modales legacy.",
                failures);

            PresentationReturnReport longReport =
                PresentationReturnReportService.Build(before, state,
                    3600.0, 3600.0, 3600.0);
            state.LE += 1250.0;
            state.Traces += 8.0;
            state.triangleEnergy += 42.0;
            longReport = PresentationReturnReportService.Build(before, state,
                3600.0, 3600.0, 3600.0, 3600.0);
            Check(longReport != null &&
                    longReport.newsKeys.Contains("return.d2") &&
                    longReport.newsKeys.Contains("return.d3") &&
                    System.Math.Abs(longReport.leDelta - 1250.0) < 0.0001 &&
                    System.Math.Abs(longReport.tracesDelta - 8.0) < 0.0001 &&
                    System.Math.Abs(longReport.triangleEnergyDelta - 42.0) < 0.0001 &&
                    System.Math.Abs(longReport.appliedSeconds - 3600.0) < 0.0001,
                "Ausencia larga no resume D2/D3 y monedas reales.", failures);

            double maximum = Dimension1System.DefaultOfflineCapSeconds;
            PresentationReturnReport cappedReport =
                PresentationReturnReportService.Build(before, state,
                    17.0 * 60.0 * 60.0, maximum, maximum, maximum);
            Check(cappedReport != null &&
                    System.Math.Abs(cappedReport.elapsedSeconds - 17.0 * 60.0 * 60.0) < 0.0001 &&
                    System.Math.Abs(cappedReport.appliedSeconds - maximum) < 0.0001,
                "El informe no diferencia 17 h de ausencia y el máximo de 12 h aplicado.",
                failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateNewsLimitAndSingleAction(List<string> failures)
    {
        GameState state = CreateState("Presentation P7 Queue");
        try
        {
            var before = new PresentationReturnSnapshot();
            PresentationReturnReport report =
                PresentationReturnReportService.Prepare(before, state,
                    7200.0, 7200.0, 7200.0);
            Check(report != null && report.newsKeys.Count <=
                    PresentationReturnReportService.MaxNewsItems,
                "Múltiples desbloqueos producen más de tres novedades.", failures);
            Check(PresentationReturnReportService.UnifiedReportPreparedThisLoad,
                "Informe unificado no suprime la cadena de modales offline.", failures);
            Check(PresentationReturnReportService.Consume() == report &&
                    PresentationReturnReportService.Consume() == null,
                "Una carga puede consumir más de un informe/modal de presentación.",
                failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateReturnModalStructure(List<string> failures)
    {
        GameObject root = new GameObject("Presentation P7 Modal Validation");
        try
        {
            PresentationReturnReportUI ui =
                root.AddComponent<PresentationReturnReportUI>();
            MethodInfo build = typeof(PresentationReturnReportUI).GetMethod(
                "Build", BindingFlags.Instance | BindingFlags.NonPublic);
            Check(build != null, "El informe no expone su constructor interno.", failures);
            build?.Invoke(ui, null);
            Button[] buttons = root.GetComponentsInChildren<Button>(true);
            Check(buttons.Length == 1 && buttons[0].name == "Continue",
                "El informe debe contener únicamente el botón Continuar.", failures);
            Check(root.transform.Find("ReturnModal/ReturnPanel/BalanceTitle") != null &&
                root.transform.Find("ReturnModal/ReturnPanel/LeBalance") != null &&
                root.transform.Find("ReturnModal/ReturnPanel/TracesBalance") != null &&
                root.transform.Find("ReturnModal/ReturnPanel/EnergyBalance") != null,
                "El informe no contiene el balance visual de las tres monedas.", failures);
        }
        catch (System.Exception exception)
        {
            failures.Add("No se pudo construir el informe: " +
                exception.GetBaseException().Message);
        }
        finally { UnityEngine.Object.DestroyImmediate(root); }
    }

    private static void ValidateAdvancedResume(List<string> failures)
    {
        GameState state = CreateState("Presentation P7 Migration");
        try
        {
            state.dimension2.civilization2Unlocked = true;
            state.dimension2.civilization3Unlocked = true;
            D2Civilization2State c2 = state.dimension2.civilization2;
            c2.totalReprisals = 2L;
            c2.alertActive = true;
            c2.containmentAvailable = true;
            D2Civilization3State c3 = state.dimension2.civilization3;
            D2C3ZoneState zone1 = D2Civilization3System.GetZone(
                c3, D2Civilization3System.Zone1Id);
            zone1.totalExcavationsCompleted = 2L;
            zone1.totalAnalysesCompleted = 1L;
            c3.archiveUnlocked = true;
            c3.archiveLevel = 1;
            state.dimension2.presentation.presentationVersion = 0;
            state.dimension2.presentation.introducedFeatureIds.Clear();
            D2PresentationRules.EnsurePresentationState(state);
            int introduced = state.dimension2.presentation.introducedFeatureIds.Count;
            D2PresentationRules.EnsurePresentationState(state);
            Check(introduced == state.dimension2.presentation.introducedFeatureIds.Count &&
                D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C2Alert).CanOpen &&
                D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C3Archive).CanOpen,
                "Save antiguo avanzado repite tutoriales o migra de forma no idempotente.",
                failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateActiveWorkNavigation(List<string> failures)
    {
        GameState state = CreateState("Presentation P7 Active");
        try
        {
            D2C3ZoneState zone1 = D2Civilization3System.GetZone(
                state.dimension2.civilization3,
                D2Civilization3System.Zone1Id);
            state.dimension2.civilization3Unlocked = true;
            zone1.excavationActive = true;
            zone1.excavationRemainingSeconds = 10.0;
            string d2Route = D2PresentationRouter.ResolveSafeScreen(
                state, "invalid.route");
            Check(d2Route == PresentationFeatureIds.D2C3Archaeology &&
                    D2PresentationRouter.CanOpen(state, d2Route),
                "Trabajo D2 activo queda oculto o provoca softlock.", failures);

            D3QueueState queue = state.dimension3.queues[0];
            queue.jobs.Add(new D3JobState
            {
                jobId = "presentation_p7_active",
                jobType = Dimension3Catalog.JobPartProduction,
                targetId = Dimension3Catalog.PartChassis,
                version = 1,
                quantity = 1L,
                remainingSeconds = 10.0,
                started = true
            });
            D3PresentationRules.EnsurePresentationState(state);
            string d3Route = D3PresentationRouter.ResolveSafeScreen(
                state, "invalid.route");
            Check(D3PresentationRouter.CanOpen(state, d3Route),
                "Trabajo D3 activo no conserva una ruta consultable.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateLocalization(List<string> failures)
    {
        foreach (string key in PresentationTextCatalog.RequiredKeys)
        {
            Check(PresentationTextCatalog.HasBothLanguages(key) &&
                PresentationTextCatalog.Get(key, false) != key &&
                PresentationTextCatalog.Get(key, true) != key,
                "Texto nuevo sin cobertura EN/ES: " + key, failures);
        }
    }

    private static GameState CreateState(string name)
    {
        var target = new GameObject(name) { hideFlags = HideFlags.HideAndDontSave };
        target.SetActive(false);
        GameState state = target.AddComponent<GameState>();
        state.dimension02Unlocked = true;
        state.dimension03Unlocked = true;
        state.dimension2 = Dimension2System.CreateInitialState();
        state.dimension3 = Dimension3System.CreateInitialState();
        state.dimension2.civilization3Unlocked = true;
        Dimension2System.EnsureState(state);
        Dimension3System.EnsureState(state);
        return state;
    }

    private static void Check(bool condition, string message,
        List<string> failures)
    {
        if (!condition) failures.Add(message);
    }
}
#endif

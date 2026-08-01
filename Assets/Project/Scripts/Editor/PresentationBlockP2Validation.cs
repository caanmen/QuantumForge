#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class PresentationBlockP2Validation
{
    [MenuItem("Tools/Quantum Forge/Presentation/Validate Block P2")]
    public static void ValidateBlockP2()
    {
        var failures = new List<string>();
        ValidateSceneConnections(failures);
        ValidateRecoverableFirstCycle(failures);
        ValidateLegacyMigration(failures);
        ValidatePresentationSafety(failures);

        if (failures.Count == 0)
        {
            Debug.Log(
                "[Presentation P2] PASS | descubrimiento | asignación | Chasis | " +
                "set 1/5–5/5 | Ensamblaje | MK1 offline | reanudación | ayuda | " +
                "navegación segura | sistemas avanzados ocultos");
            return;
        }
        Debug.LogError("[Presentation P2] FAIL\n- " + string.Join("\n- ", failures));
    }

    public static void ValidateBlockP2Batch()
    {
        EditorSceneManager.OpenScene(
            "Assets/Project/Scenes/Main.unity", OpenSceneMode.Single);
        ValidateBlockP2();
    }

    public static void ValidateP2AndRegressionsBatch()
    {
        ValidateBlockP2Batch();
        PresentationBlockP1Validation.ValidateBlockP1();
        Dimension3Block7FullValidation.ValidateFullThrough7D();
        Debug.Log("[Presentation P2 Full] FIN | revisar PASS/FAIL anteriores");
    }

    private static void ValidateSceneConnections(List<string> failures)
    {
        Dimension3PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension3PanelUI>(
            FindObjectsInactive.Include);
        Check(panel != null, "Main.unity no contiene Dimension3PanelUI.", failures);
        if (panel == null) return;

        Check(panel.objectiveCard != null,
            "La tarjeta común AHORA/DESPUÉS no está conectada.", failures);
        if (panel.objectiveCard != null)
        {
            Check(panel.objectiveCard.nowTitleText != null,
                "Tarjeta AHORA sin título.", failures);
            Check(panel.objectiveCard.nowBodyText != null,
                "Tarjeta AHORA sin cuerpo.", failures);
            Check(panel.objectiveCard.progressText != null,
                "Tarjeta AHORA sin progreso.", failures);
            Check(panel.objectiveCard.primaryActionButton != null,
                "Tarjeta AHORA sin CTA.", failures);
            Check(panel.objectiveCard.nextRoot != null &&
                panel.objectiveCard.nextTitleText != null &&
                panel.objectiveCard.nextBodyText != null,
                "Tarjeta DESPUÉS incompleta.", failures);
        }
        Check(panel.productionTitleText != null && panel.coachmarkRoot != null &&
            panel.coachmarkText != null,
            "Faltan referencias de presentación progresiva.", failures);
        Check(panel.contextualHelpButton != null && panel.helpRoot != null &&
            panel.helpTitleText != null && panel.helpBodyText != null &&
            panel.closeHelpButton != null,
            "AYUDA contextual no está conectada.", failures);
        Check(panel.firstCycleCompleteRoot != null &&
            panel.firstCycleCompleteText != null &&
            panel.continueFirstCycleButton != null &&
            panel.replayFirstCycleButton != null &&
            panel.closeFirstCycleButton != null,
            "Cierre del primer ciclo no está conectado.", failures);
        Check(panel.closeDimension3Button != null,
            "Cerrar D3 dejó de estar disponible.", failures);
        Check(panel.helpRoot != null && !panel.helpRoot.activeSelf &&
            panel.firstCycleCompleteRoot != null &&
            !panel.firstCycleCompleteRoot.activeSelf,
            "Ayuda/celebración deben iniciar cerradas.", failures);
    }

    private static void ValidateRecoverableFirstCycle(List<string> failures)
    {
        GameState state = CreateTestState("Presentation P2 Cycle");
        try
        {
            state.dimension03Unlocked = true;
            state.dimension3 = Dimension3System.CreateInitialState();
            state.LE = 1000000.0;
            state.Traces = 100000.0;
            Dimension3System.EnsureState(state);

            CheckStage(state, D3OnboardingStage.Discovery,
                "partida nueva", failures);
            CheckRoundTripStage(state, D3OnboardingStage.Discovery,
                "cerrar/cargar en descubrimiento", failures);

            Dimension3System.MarkFirstEntrySeen(state);
            CheckStage(state, D3OnboardingStage.AssignInitial,
                "activar Banco", failures);
            CheckRoundTripStage(state, D3OnboardingStage.AssignInitial,
                "cerrar/cargar antes de asignar", failures);

            string reason;
            Check(Dimension3System.TrySetProcessBankAssignment(
                    state, Dimension3Catalog.ChannelProcessPower, 1,
                    Dimension3Catalog.TraitNormal, 1L, out reason),
                "No asigna MK1 inicial: " + reason, failures);
            CheckStage(state, D3OnboardingStage.FirstPart,
                "asignación inicial", failures);
            CheckRoundTripStage(state, D3OnboardingStage.FirstPart,
                "cerrar/cargar antes de Chasis", failures);

            state.dimension3.presentation.contextualHelpEnabled = false;
            Check(Dimension3System.TryQueuePartProduction(
                    state, Dimension3Catalog.PartChassis, 1, 1L, out reason),
                "Ayuda desactivada bloquea Chasis: " + reason, failures);
            CheckStage(state, D3OnboardingStage.FirstPart,
                "Chasis en cola", failures);
            FeaturePresentationState productionActive =
                D3PresentationRules.GetFeatureState(
                    state, PresentationFeatureIds.D3ProductionFirstPart);
            Check(productionActive.visualState == FeaturePresentationVisualState.Active &&
                productionActive.IsVisible && productionActive.CanOpen,
                "Trabajo de Producción activo queda oculto.", failures);

            Dimension3System.ApplyOfflineProgress(state, 1000.0);
            Check(D3InventorySystem.GetPartAmount(
                    state.dimension3, Dimension3Catalog.PartChassis, 1) == 1L,
                "Chasis offline no se recibe.", failures);
            CheckStage(state, D3OnboardingStage.CompleteSet,
                "Chasis recibido", failures);

            string[] remaining =
            {
                Dimension3Catalog.PartMotor,
                Dimension3Catalog.PartTool,
                Dimension3Catalog.PartControl,
                Dimension3Catalog.PartRegulator
            };
            for (int i = 0; i < remaining.Length; i++)
            {
                Check(Dimension3System.TryQueuePartProduction(
                        state, remaining[i], 1, 1L, out reason),
                    "No encola " + remaining[i] + ": " + reason, failures);
            }
            D3OnboardingSnapshot planned = D3OnboardingRules.Synchronize(state);
            Check(planned.stage == D3OnboardingStage.CompleteSet &&
                planned.plannedPartTypes == 5,
                "Set en cola no conserva el paso 1/5–5/5.", failures);
            CheckRoundTripStage(state, D3OnboardingStage.CompleteSet,
                "cerrar/cargar con set en cola", failures);

            Dimension3System.ApplyOfflineProgress(state, 1000.0);
            CheckStage(state, D3OnboardingStage.FirstAssembly,
                "set completo", failures);
            Check(Dimension3System.TryQueueNormalAssembly(
                    state, 1, 1L, out reason),
                "No inicia primer Ensamblaje: " + reason, failures);
            CheckStage(state, D3OnboardingStage.FirstAssembly,
                "Ensamblaje activo", failures);
            FeaturePresentationState assemblyActive =
                D3PresentationRules.GetFeatureState(
                    state, PresentationFeatureIds.D3AssemblyNormal);
            Check(assemblyActive.visualState == FeaturePresentationVisualState.Active &&
                assemblyActive.IsVisible && assemblyActive.CanOpen,
                "Ensamblaje activo queda oculto.", failures);
            Check(D3PresentationRouter.ResolveSafeScreen(state, "invalid") ==
                    PresentationFeatureIds.D3AssemblyNormal,
                "Router no prioriza Ensamblaje activo.", failures);
            CheckRoundTripStage(state, D3OnboardingStage.FirstAssembly,
                "cerrar/cargar durante Ensamblaje", failures);

            Dimension3System.ApplyOfflineProgress(state, 1000.0);
            Check(D3InventorySystem.GetAssemblyCount(state.dimension3, 1) == 1L,
                "MK1 no termina offline.", failures);
            CheckStage(state, D3OnboardingStage.AssignNew,
                "MK1 terminado offline", failures);
            CheckRoundTripStage(state, D3OnboardingStage.AssignNew,
                "cerrar/cargar antes de asignar nuevo MK1", failures);

            Check(Dimension3System.TrySetProcessBankAssignment(
                    state, Dimension3Catalog.ChannelProcessPower, 1,
                    Dimension3Catalog.TraitNormal, 2L, out reason),
                "No asigna nuevo MK1: " + reason, failures);
            CheckStage(state, D3OnboardingStage.Celebration,
                "nuevo MK1 asignado", failures);
            state.dimension3.presentation.onboardingStage =
                (int)D3OnboardingStage.Completed;
            CheckStage(state, D3OnboardingStage.Completed,
                "continuar tras celebración", failures);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(state.gameObject);
        }
    }

    private static void ValidateLegacyMigration(List<string> failures)
    {
        GameState state = CreateTestState("Presentation P2 Migration");
        try
        {
            state.dimension03Unlocked = true;
            state.dimension3 = Dimension3System.CreateInitialState();
            Dimension3System.EnsureState(state);
            state.dimension3.firstEntrySeen = true;
            state.dimension3.presentation.presentationVersion = 1;
            D3InventorySystem.AddAutomatons(
                state.dimension3, 1, Dimension3Catalog.TraitNormal, 1L);
            state.dimension3.totalAssembledByMk.Add(new D3MkAssemblyCountState
            {
                mk = 1,
                amount = 1L
            });
            Dimension3System.EnsureState(state);
            Check(state.dimension3.presentation.presentationVersion ==
                    DimensionPresentationState.CurrentVersion,
                "Migración P2 no actualiza presentationVersion.", failures);
            Check(state.dimension3.presentation.onboardingStage >=
                    (int)D3OnboardingStage.Completed,
                "Save avanzado repite onboarding obsoleto.", failures);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(state.gameObject);
        }
    }

    private static void ValidatePresentationSafety(List<string> failures)
    {
        GameState state = CreateTestState("Presentation P2 Safety");
        try
        {
            state.dimension03Unlocked = true;
            state.dimension3 = Dimension3System.CreateInitialState();
            Dimension3System.EnsureState(state);
            Dimension3System.MarkFirstEntrySeen(state);
            D3OnboardingSnapshot snapshot = D3OnboardingRules.Synchronize(state);
            Check(snapshot.stage == D3OnboardingStage.AssignInitial,
                "Coachmark altera el paso real.", failures);
            state.dimension3.presentation.contextualHelpEnabled = false;
            Check(D3OnboardingRules.Synchronize(state).stage ==
                    D3OnboardingStage.AssignInitial,
                "Desactivar ayuda bloquea o avanza onboarding.", failures);
            Check(D3PresentationRouter.CanOpen(
                    state, PresentationFeatureIds.D3Factory),
                "Fábrica base no permanece accesible.", failures);
            Check(!D3PresentationRules.GetFeatureState(
                    state, PresentationFeatureIds.D3Calibration).CanOpen &&
                  !D3PresentationRules.GetFeatureState(
                    state, PresentationFeatureIds.D3Research).CanOpen &&
                  !D3PresentationRules.GetFeatureState(
                    state, PresentationFeatureIds.D3Facilities).CanOpen &&
                  !D3PresentationRules.GetFeatureState(
                    state, PresentationFeatureIds.D3Automation).CanOpen,
                "Sistema avanzado disponible durante primera sesión.", failures);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(state.gameObject);
        }
    }

    private static void CheckRoundTripStage(
        GameState state,
        D3OnboardingStage expected,
        string context,
        List<string> failures)
    {
        string json = JsonUtility.ToJson(state.dimension3);
        state.dimension3 = JsonUtility.FromJson<Dimension3State>(json);
        Dimension3System.EnsureState(state);
        CheckStage(state, expected, context, failures);
    }

    private static void CheckStage(
        GameState state,
        D3OnboardingStage expected,
        string context,
        List<string> failures)
    {
        D3OnboardingSnapshot snapshot = D3OnboardingRules.Synchronize(state);
        Check(snapshot.stage == expected,
            context + " restaura " + snapshot.stage + " en vez de " + expected + ".",
            failures);
    }

    private static void Check(bool condition, string message, List<string> failures)
    {
        if (!condition) failures.Add(message);
    }

    private static GameState CreateTestState(string name)
    {
        var gameObject = new GameObject(name) { hideFlags = HideFlags.HideAndDontSave };
        gameObject.SetActive(false);
        return gameObject.AddComponent<GameState>();
    }
}
#endif

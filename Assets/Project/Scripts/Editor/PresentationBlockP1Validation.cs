#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class PresentationBlockP1Validation
{
    [MenuItem("Tools/Quantum Forge/Presentation/Validate Block P1")]
    public static void ValidateBlockP1()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("[Presentation P1] Ejecutar fuera de Play Mode.");
            return;
        }

        var failures = new List<string>();
        ValidateFeatureIds(failures);
        ValidateNewAndLegacyMigration(failures);
        ValidateNormalizationAndJson(failures);
        ValidateRouterFallbacks(failures);
        ValidateActivePrecedence(failures);
        ValidateDropdownMapping(failures);

        if (failures.Count == 0)
        {
            Debug.Log(
                "[Presentation P1] PASS | estado D2/D3 | migración | IDs | " +
                "normalización | fallbacks | actividad activa | dropdown ID");
            return;
        }

        Debug.LogError("[Presentation P1] FAIL\n- " + string.Join("\n- ", failures));
    }

    public static void ValidateBlockP1Batch()
    {
        ValidateBlockP1();
    }

    public static void ValidateRelatedRegressionsBatch()
    {
        EditorSceneManager.OpenScene(
            "Assets/Project/Scenes/Main.unity", OpenSceneMode.Single);
        Dimension2Block1UISetup.ValidateBlock1();
        Dimension3Block7FullValidation.ValidateFullThrough7D();
        Debug.Log("[Presentation P1 Regressions] FIN | revisar PASS/FAIL anteriores");
    }

    private static void ValidateFeatureIds(List<string> failures)
    {
        CheckUnique(PresentationFeatureIds.D2All, "D2", failures);
        CheckUnique(PresentationFeatureIds.D3All, "D3", failures);
        for (int i = 0; i < PresentationFeatureIds.D2All.Length; i++)
            Check(D2PresentationRules.IsKnownFeatureId(PresentationFeatureIds.D2All[i]),
                "ID D2 no reconocido: " + PresentationFeatureIds.D2All[i], failures);
        for (int i = 0; i < PresentationFeatureIds.D3All.Length; i++)
            Check(D3PresentationRules.IsKnownFeatureId(PresentationFeatureIds.D3All[i]),
                "ID D3 no reconocido: " + PresentationFeatureIds.D3All[i], failures);
    }

    private static void ValidateNewAndLegacyMigration(List<string> failures)
    {
        GameState state = CreateTestState("Presentation P1 Migration");
        try
        {
            state.dimension02Unlocked = true;
            state.dimension03Unlocked = true;
            state.dimension2 = Dimension2System.CreateInitialState();
            state.dimension3 = Dimension3System.CreateInitialState();
            Dimension2System.EnsureState(state);
            Dimension3System.EnsureState(state);
            CheckCurrent(state.dimension2.presentation, "save nuevo D2", failures);
            CheckCurrent(state.dimension3.presentation, "save nuevo D3", failures);

            state.dimension2.presentation = null;
            state.dimension2.firstEntrySeen = true;
            state.dimension2.civilization2Unlocked = true;
            state.dimension2.civilization1.trust = 300.0;
            state.dimension2.civilization1.activePilgrimage.active = true;
            state.dimension2.civilization1.activePilgrimage.pilgrimageId =
                D2PilgrimageSystem.ShortId;
            Dimension2System.EnsureState(state);
            CheckCurrent(state.dimension2.presentation, "save antiguo D2", failures);
            Check(Contains(state.dimension2.presentation,
                    PresentationFeatureIds.D2C2Regions),
                "Migración D2 avanzada no infiere regiones.", failures);
            Check(Contains(state.dimension2.presentation,
                    PresentationFeatureIds.D2C1Pilgrimages),
                "Migración D2 no introduce actividad activa.", failures);

            state.dimension3.presentation = null;
            state.dimension3.initialized = true;
            state.dimension3.firstEntrySeen = true;
            state.dimension3.parts.Add(new D3PartStackState
            {
                partId = Dimension3Catalog.PartChassis,
                version = 1,
                amount = 1L
            });
            Dimension3System.EnsureState(state);
            CheckCurrent(state.dimension3.presentation, "save antiguo D3", failures);
            Check(Contains(state.dimension3.presentation,
                    PresentationFeatureIds.D3ProductionFirstPart),
                "Migración D3 avanzada no infiere producción.", failures);
            Check(state.dimension3.presentation.onboardingStage >= 3,
                "Migración D3 no infiere onboardingStage coherente.", failures);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(state.gameObject);
        }
    }

    private static void ValidateNormalizationAndJson(List<string> failures)
    {
        var presentation = new DimensionPresentationState
        {
            presentationVersion = 0,
            introducedFeatureIds = new List<string>
            {
                PresentationFeatureIds.D2Map,
                PresentationFeatureIds.D2Map,
                "future.unknown.id",
                ""
            },
            acknowledgedFeatureIds = new List<string>
            {
                PresentationFeatureIds.D2C1Refuge,
                PresentationFeatureIds.D2C1Refuge
            }
        };
        PresentationStateUtility.Normalize(presentation);
        Check(presentation.introducedFeatureIds.Count == 3,
            "Normalización no elimina duplicados/vacíos o no integra reconocidos.", failures);
        Check(Contains(presentation, "future.unknown.id"),
            "Normalización elimina IDs futuros desconocidos.", failures);

        var data = new SaveData
        {
            dimension2 = Dimension2System.CreateInitialState(),
            dimension3 = Dimension3System.CreateInitialState()
        };
        data.dimension2.presentation.onboardingStage = 4;
        PresentationStateUtility.Introduce(
            data.dimension3.presentation, PresentationFeatureIds.D3Calibration);
        string json = JsonUtility.ToJson(data);
        SaveData loaded = JsonUtility.FromJson<SaveData>(json);
        Check(loaded != null && loaded.dimension2 != null &&
            loaded.dimension2.presentation != null &&
            loaded.dimension2.presentation.onboardingStage == 4,
            "JSON pierde PresentationState D2.", failures);
        Check(loaded != null && loaded.dimension3 != null &&
            loaded.dimension3.presentation != null &&
            Contains(loaded.dimension3.presentation, PresentationFeatureIds.D3Calibration),
            "JSON pierde PresentationState D3.", failures);
    }

    private static void ValidateRouterFallbacks(List<string> failures)
    {
        GameState state = CreateTestState("Presentation P1 Router");
        try
        {
            state.dimension02Unlocked = true;
            state.dimension03Unlocked = true;
            state.dimension2 = Dimension2System.CreateInitialState();
            state.dimension3 = Dimension3System.CreateInitialState();
            Dimension2System.EnsureState(state);
            Dimension3System.EnsureState(state);
            state.dimension2.firstEntrySeen = true;
            state.dimension3.firstEntrySeen = true;
            state.dimension2.presentation.lastScreenId = "invalid.d2.screen";
            state.dimension3.presentation.lastScreenId = "invalid.d3.screen";

            Check(D2PresentationRouter.ResolveSafeScreen(
                    state, state.dimension2.presentation.lastScreenId) ==
                    PresentationFeatureIds.D2C1Refuge,
                "Fallback D2 no usa el territorio seleccionado seguro.", failures);
            Check(D3PresentationRouter.ResolveSafeScreen(
                    state, state.dimension3.presentation.lastScreenId) ==
                    PresentationFeatureIds.D3Factory,
                "Fallback D3 no vuelve a Fábrica.", failures);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(state.gameObject);
        }
    }

    private static void ValidateActivePrecedence(List<string> failures)
    {
        GameState state = CreateTestState("Presentation P1 Active");
        try
        {
            state.dimension02Unlocked = true;
            state.dimension03Unlocked = true;
            state.dimension2 = Dimension2System.CreateInitialState();
            state.dimension3 = Dimension3System.CreateInitialState();
            Dimension2System.EnsureState(state);
            Dimension3System.EnsureState(state);

            state.dimension2.civilization1.activePilgrimage.active = true;
            state.dimension2.civilization1.activePilgrimage.pilgrimageId =
                D2PilgrimageSystem.ShortId;
            state.dimension2.presentation.introducedFeatureIds.Clear();
            D2PresentationRules.EnsurePresentationState(state);
            FeaturePresentationState d2Feature = D2PresentationRules.GetFeatureState(
                state, PresentationFeatureIds.D2C1Pilgrimages);
            Check(d2Feature.visualState == FeaturePresentationVisualState.Active &&
                d2Feature.IsVisible && d2Feature.CanOpen,
                "Actividad D2 no prevalece como Active visible.", failures);

            D3QueueState queue = D3JobQueueSystem.GetQueue(
                state.dimension3, Dimension3Catalog.QueuePartProduction);
            queue.jobs.Add(new D3JobState
            {
                jobId = "p1_active_job",
                jobType = Dimension3Catalog.JobPartProduction,
                targetId = Dimension3Catalog.PartChassis,
                version = 1,
                quantity = 1L,
                remainingSeconds = 5.0,
                started = true
            });
            state.dimension3.presentation.introducedFeatureIds.Clear();
            D3PresentationRules.EnsurePresentationState(state);
            FeaturePresentationState d3Feature = D3PresentationRules.GetFeatureState(
                state, PresentationFeatureIds.D3ProductionFirstPart);
            Check(d3Feature.visualState == FeaturePresentationVisualState.Active &&
                d3Feature.IsVisible && d3Feature.CanOpen,
                "Trabajo D3 no prevalece como Active visible.", failures);
            Check(D3PresentationRouter.ResolveSafeScreen(state, "invalid") ==
                    PresentationFeatureIds.D3ProductionFirstPart,
                "Router D3 no prioriza el trabajo activo.", failures);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(state.gameObject);
        }
    }

    private static void ValidateDropdownMapping(List<string> failures)
    {
        var map = new SafeDropdownOptionMap<int>();
        int selected = map.Rebuild(null, new[] { 1, 3, 5 },
            value => "V" + value, 5);
        Check(selected == 2 && map.ResolveOrDefault(selected, -1) == 5,
            "Dropdown filtrado no conserva el ID seleccionado.", failures);
        selected = map.Rebuild(null, new[] { 1, 5 },
            value => "V" + value, 5);
        Check(selected == 1 && map.ResolveOrDefault(1, -1) == 5,
            "Dropdown interpreta índice visual como ID real.", failures);
        selected = map.Rebuild(null, new[] { 1, 3 },
            value => "V" + value, 5);
        Check(selected == 0 && map.ResolveOrDefault(0, -1) == 1,
            "Dropdown no cae a la primera opción válida.", failures);
    }

    private static void CheckCurrent(
        DimensionPresentationState state,
        string context,
        List<string> failures)
    {
        Check(state != null &&
            state.presentationVersion == DimensionPresentationState.CurrentVersion &&
            state.introducedFeatureIds != null &&
            state.acknowledgedFeatureIds != null,
            "No inicializa PresentationState actual en " + context + ".", failures);
    }

    private static void CheckUnique(
        string[] ids,
        string dimension,
        List<string> failures)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        for (int i = 0; i < ids.Length; i++)
            Check(!string.IsNullOrEmpty(ids[i]) && seen.Add(ids[i]),
                "ID vacío o duplicado en " + dimension + ": " + ids[i], failures);
    }

    private static bool Contains(DimensionPresentationState state, string id)
    {
        return state != null &&
            PresentationStateUtility.Contains(state.introducedFeatureIds, id);
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

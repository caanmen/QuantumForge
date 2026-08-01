#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class PresentationBlockP5Validation
{
    [MenuItem("Tools/Quantum Forge/Presentation/Validate Block P5")]
    public static void ValidateBlockP5()
    {
        var failures = new List<string>();
        ValidateScene(failures);
        ValidateCivilization2Sequence(failures);
        ValidateRegionOrder(failures);
        ValidateAlertContainmentAndPact(failures);
        ValidateAdvancedMigration(failures);
        ValidateFilteredCatalogs(failures);
        if (failures.Count == 0)
        {
            Debug.Log("[Presentation P5] PASS | D2-8 inicio Civ 2 | " +
                "D2-9 Operaciones/Defensa | D2-10 Resistencia/Regiones | " +
                "D2-11 Alerta/Contención/Pacto Mayor");
            return;
        }
        Debug.LogError("[Presentation P5] FAIL\n- " +
            string.Join("\n- ", failures));
    }

    public static void ValidateP5AndRegressionsBatch()
    {
        EditorSceneManager.OpenScene(
            "Assets/Project/Scenes/Main.unity", OpenSceneMode.Single);
        ValidateBlockP5();
        PresentationBlockP4Validation.ValidateBlockP4();
        PresentationBlockP3Validation.ValidateBlockP3();
        PresentationBlockP2Validation.ValidateBlockP2();
        PresentationBlockP1Validation.ValidateBlockP1();
        Dimension2Block1UISetup.ValidateBlock1();
        Debug.Log("[Presentation P5 Full] FIN | revisar PASS/FAIL anteriores");
    }

    private static void ValidateScene(List<string> failures)
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<
            Dimension2PanelUI>(FindObjectsInactive.Include);
        Check(panel != null, "Main no contiene Dimension2PanelUI.", failures);
        D2Civilization2PanelUI c2 = panel != null
            ? panel.civilization2PanelUI : null;
        Check(c2 != null && c2.objectiveText != null,
            "Civilización 2 no tiene objetivo AHORA/DESPUÉS conectado.", failures);
        if (c2 == null) return;
        Check(c2.operationsPanelUI != null && c2.defenseSectionRoot != null &&
            c2.reprisalsPanelUI != null && c2.resistancePanelUI != null &&
            c2.alertPanelUI != null && c2.containmentPanelUI != null,
            "Faltan paneles progresivos de Civilización 2.", failures);
        D2ContainmentPanelUI containment = c2.containmentPanelUI;
        Check(containment != null && containment.containmentAttemptRoot != null &&
            containment.majorPactRoot != null && containment.probabilityText != null &&
            containment.rulesText != null && containment.attemptButton != null &&
            containment.establishMajorPactButton != null,
            "Contención/Pacto Mayor no tiene sus controles informativos conectados.",
            failures);
    }

    private static void ValidateCivilization2Sequence(List<string> failures)
    {
        GameState state = CreateState("Presentation P5 Sequence");
        try
        {
            D2Civilization2State c2 = state.dimension2.civilization2;
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C2Regions).CanOpen,
                "Civ 2 inicial no presenta Región 1.", failures);
            Check(!D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C2Operations).IsVisible &&
                !D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C2Defense).IsVisible &&
                !D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C2Resistance).IsVisible &&
                !D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C2Alert).IsVisible &&
                !D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C2Containment).IsVisible,
                "Civ 2 inicial revela sistemas avanzados.", failures);

            D2RegionState region1 = D2Civilization2System.GetRegion(
                c2, D2Civilization2System.Region1Id);
            region1.membersAssigned = 1L;
            c2.membersAvailable = Math.Max(0L, c2.membersAvailable - 1L);
            D2PresentationRules.EnsurePresentationState(state);
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C2Operations).CanOpen,
                "Asignar a Región 1 no revela Operaciones.", failures);
            string[] operations =
                D2Civilization2PresentationRules.GetVisibleOperationIds(c2);
            Check(operations.Length == 1 &&
                    operations[0] == D2Civilization2System.RescueOperationId,
                "Primera Operación no es únicamente Rescate.", failures);

            D2OperationState rescue = D2Civilization2System.GetOperation(
                region1, D2Civilization2System.RescueOperationId);
            rescue.membersAssigned = 1L;
            operations = D2Civilization2PresentationRules.GetVisibleOperationIds(c2);
            Check(Array.IndexOf(operations,
                    D2Civilization2System.ProtectionOperationId) >= 0,
                "Aprender Rescate no revela Protección.", failures);

            region1.threat = 1.0;
            D2PresentationRules.EnsurePresentationState(state);
            Check(c2.totalReprisals == 0L &&
                D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C2Defense).CanOpen,
                "La primera Amenaza no revela Defensa antes de Represalias.", failures);
            operations = D2Civilization2PresentationRules.GetVisibleOperationIds(c2);
            Check(Array.IndexOf(operations,
                    D2Civilization2System.EspionageOperationId) >= 0,
                "Espionaje no aparece cuando ya existe Amenaza.", failures);

            c2.totalReprisals = 1L;
            c2.controlFragments = 1L;
            D2PresentationRules.EnsurePresentationState(state);
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C2Resistance).CanOpen,
                "Primera Represalia/Fragmento no revela Resistencia.", failures);
            operations = D2Civilization2PresentationRules.GetVisibleOperationIds(c2);
            Check(Array.IndexOf(operations,
                    D2Civilization2System.SabotageOperationId) >= 0,
                "Sabotaje no aparece después de la primera Represalia.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateRegionOrder(List<string> failures)
    {
        GameState state = CreateState("Presentation P5 Regions");
        try
        {
            D2Civilization2State c2 = state.dimension2.civilization2;
            string[] ids = D2Civilization2PresentationRules.GetVisibleRegionIds(c2);
            Check(ids.Length == 1 && ids[0] == D2Civilization2System.Region1Id,
                "Inicio de Civ 2 revela regiones futuras.", failures);
            D2Civilization2System.GetRegion(c2,
                D2Civilization2System.Region1Id).dominance = 90.0;
            ids = D2Civilization2PresentationRules.GetVisibleRegionIds(c2);
            Check(ids.Length == 2 && ids[1] == D2Civilization2System.Region2Id,
                "Región 2 no aparece como siguiente antes del umbral.", failures);
            D2RegionState region2 = D2Civilization2System.GetRegion(
                c2, D2Civilization2System.Region2Id);
            region2.unlocked = true;
            region2.dominance = 0.0;
            D2Civilization2System.GetRegion(c2,
                D2Civilization2System.Region1Id).dominance = 70.0;
            ids = D2Civilization2PresentationRules.GetVisibleRegionIds(c2);
            Check(ids.Length == 3 && ids[2] == D2Civilization2System.Region3Id &&
                    Array.IndexOf(ids, D2Civilization2System.Region4Id) < 0,
                "Región 3/4 no respeta el orden progresivo aprobado.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateAlertContainmentAndPact(List<string> failures)
    {
        GameState state = CreateState("Presentation P5 Closure");
        try
        {
            D2Civilization2State c2 = state.dimension2.civilization2;
            foreach (D2RegionState region in c2.regions)
                region.dominance = 0.0;
            D2Civilization2System.GetRegion(c2,
                D2Civilization2System.Region1Id).dominance = 30.0;
            Check(D2Civilization2System.SyncAlertUnlocks(state),
                "Umbral real no activa Alerta.", failures);
            D2PresentationRules.EnsurePresentationState(state);
            Check(c2.alertActive && c2.containmentAvailable &&
                    state.dimension2.civilization3Unlocked &&
                D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C2Alert).visualState ==
                    FeaturePresentationVisualState.Active &&
                D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C2Containment).CanOpen,
                "Alerta no libera simultáneamente marcas, Contención y Civ 3.",
                failures);
            Check(D2Civilization2PresentationRules.RequiresContainmentConfirmation(c2),
                "Probabilidad menor de 50% no exige confirmación de presentación.",
                failures);

            c2.entityContained = true;
            c2.majorPactPrepared = true;
            D2PresentationRules.EnsurePresentationState(state);
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C2MajorPact).CanOpen,
                "Contener el Ente no reemplaza controles por Pacto Mayor.", failures);
            c2.majorPactEstablished = true;
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C2MajorPact).visualState ==
                    FeaturePresentationVisualState.Completed,
                "Pacto Mayor establecido no se reconoce como hito de cierre.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateAdvancedMigration(List<string> failures)
    {
        GameState state = CreateState("Presentation P5 Migration");
        try
        {
            D2Civilization2State c2 = state.dimension2.civilization2;
            c2.totalReprisals = 3L;
            c2.controlFragments = 5L;
            c2.alertActive = true;
            c2.containmentAvailable = true;
            state.dimension2.civilization3Unlocked = true;
            state.dimension2.presentation.presentationVersion = 0;
            state.dimension2.presentation.introducedFeatureIds.Clear();
            D2PresentationRules.EnsurePresentationState(state);
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C2Defense).CanOpen &&
                D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C2Resistance).CanOpen &&
                D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C2Alert).CanOpen &&
                D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C2Containment).CanOpen,
                "Save avanzado de Civ 2 repetiría el tutorial progresivo.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateFilteredCatalogs(List<string> failures)
    {
        GameState state = CreateState("Presentation P5 Catalogs");
        try
        {
            D2Civilization2State c2 = state.dimension2.civilization2;
            Check(D2Civilization2PresentationRules.GetVisibleUpgradeIds(c2).Length == 1,
                "Resistencia inicial revela mejoras futuras.", failures);
            Check(D2Civilization2PresentationRules.GetVisibleResistancePactIds(c2).Length == 1,
                "Resistencia inicial revela todos los Pactos.", failures);
            var map = new SafeDropdownOptionMap<string>(StringComparer.Ordinal);
            map.Rebuild(null,
                D2Civilization2PresentationRules.GetVisibleOperationIds(c2),
                value => value, D2Civilization2System.RescueOperationId);
            Check(map.ResolveOrDefault(0, "fallback") ==
                    D2Civilization2System.RescueOperationId &&
                map.ResolveOrDefault(99, "fallback") == "fallback",
                "Dropdown filtrado de Operaciones no conserva ID seguro.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static GameState CreateState(string name)
    {
        var target = new GameObject(name) { hideFlags = HideFlags.HideAndDontSave };
        target.SetActive(false);
        GameState state = target.AddComponent<GameState>();
        state.dimension02Unlocked = true;
        state.dimension2 = Dimension2System.CreateInitialState();
        state.dimension2.civilization2Unlocked = true;
        Dimension2System.EnsureState(state);
        return state;
    }

    private static void Check(bool condition, string message,
        List<string> failures)
    {
        if (!condition) failures.Add(message);
    }
}
#endif

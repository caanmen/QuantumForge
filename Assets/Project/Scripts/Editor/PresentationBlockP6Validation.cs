#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class PresentationBlockP6Validation
{
    [MenuItem("Tools/Quantum Forge/Presentation/Validate Block P6")]
    public static void ValidateBlockP6()
    {
        var failures = new List<string>();
        ValidateScene(failures);
        ValidateArchaeologySequence(failures);
        ValidateProgressiveZones(failures);
        ValidateCluesAnomaliesAndEntity(failures);
        ValidateActiveSections(failures);
        ValidateAdvancedMigration(failures);
        if (failures.Count == 0)
        {
            Debug.Log("[Presentation P6] PASS | D2-12 Arqueología/Restos | " +
                "D2-13 Análisis/Archivo/Zonas | D2-14 Indicios/Anomalías/Ente");
            return;
        }
        Debug.LogError("[Presentation P6] FAIL\n- " +
            string.Join("\n- ", failures));
    }

    public static void ValidateP6AndRegressionsBatch()
    {
        EditorSceneManager.OpenScene(
            "Assets/Project/Scenes/Main.unity", OpenSceneMode.Single);
        ValidateBlockP6();
        PresentationBlockP5Validation.ValidateBlockP5();
        PresentationBlockP4Validation.ValidateBlockP4();
        PresentationBlockP3Validation.ValidateBlockP3();
        PresentationBlockP2Validation.ValidateBlockP2();
        PresentationBlockP1Validation.ValidateBlockP1();
        Dimension2Block1UISetup.ValidateBlock1();
        Debug.Log("[Presentation P6 Full] FIN | revisar PASS/FAIL anteriores");
    }

    private static void ValidateScene(List<string> failures)
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<
            Dimension2PanelUI>(FindObjectsInactive.Include);
        D2Civilization3PanelUI c3 = panel != null
            ? panel.civilization3PanelUI : null;
        Check(c3 != null && c3.objectiveText != null,
            "Civilización 3 no tiene objetivo AHORA/DESPUÉS conectado.", failures);
        if (c3 == null) return;
        Check(c3.archaeologySectionRoot != null && c3.archiveSectionRoot != null &&
            c3.entityResearchSectionRoot != null && c3.archivePanelUI != null &&
            c3.entityResearchPanelUI != null,
            "Faltan secciones progresivas de Civilización 3.", failures);
        Check(c3.zone1Button != null && c3.zone2Button != null &&
            c3.zone3Button != null && c3.excavateButton != null &&
            c3.inventoryText != null && c3.scholarText != null &&
            c3.analyzeLowButton != null && c3.analyzeMediumButton != null &&
            c3.analyzeHighButton != null && c3.showArchiveButton != null &&
            c3.showEntityResearchButton != null,
            "La escena no conserva controles canónicos de Civ 3.", failures);
    }

    private static void ValidateArchaeologySequence(List<string> failures)
    {
        GameState state = CreateState("Presentation P6 Archaeology");
        try
        {
            D2Civilization3State c3 = state.dimension2.civilization3;
            D2C3ZoneState zone1 = D2Civilization3System.GetZone(
                c3, D2Civilization3System.Zone1Id);
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C3Archaeology).CanOpen &&
                !D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C3Analysis).IsVisible &&
                !D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C3Archive).IsVisible &&
                !D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C3Clues).IsVisible &&
                !D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C3Anomalies).IsVisible &&
                !D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C3EntityResearch).IsVisible,
                "Primera entrada de Civ 3 revela contenido posterior a Excavar.", failures);
            string[] zones = D2Civilization3PresentationRules.GetVisibleZoneIds(c3);
            Check(zones.Length == 1 && zones[0] == D2Civilization3System.Zone1Id,
                "Primera entrada no muestra únicamente Zona 1.", failures);
            Check(D2Civilization3PresentationRules.GetPossessedQualityIds(zone1).Length == 0,
                "Primera entrada muestra calidades de Restos no poseídas.", failures);

            zone1.totalExcavationsCompleted = 1L;
            zone1.mediumQualityRemains = 1L;
            D2PresentationRules.EnsurePresentationState(state);
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C3Analysis).CanOpen,
                "Primera Excavación no revela Restos/Erudito/Análisis.", failures);
            string[] qualities =
                D2Civilization3PresentationRules.GetPossessedQualityIds(zone1);
            Check(qualities.Length == 1 &&
                    qualities[0] == D2Civilization3System.MediumQualityId,
                "Análisis revela calidades que el jugador no posee.", failures);

            zone1.totalAnalysesCompleted = 1L;
            zone1.researchProgress = 5.0;
            c3.ancientKnowledge = 1.0;
            c3.archiveUnlocked = true;
            c3.archiveLevel = 1;
            D2PresentationRules.EnsurePresentationState(state);
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C3Archive).CanOpen &&
                D2Civilization3PresentationRules.GetCurrentArchiveDiscovery(c3)
                    .Contains("40%"),
                "Primer Análisis no revela Investigación/Archivo y próximo hito.",
                failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateProgressiveZones(List<string> failures)
    {
        GameState state = CreateState("Presentation P6 Zones");
        try
        {
            D2Civilization3State c3 = state.dimension2.civilization3;
            D2C3ZoneState zone1 = D2Civilization3System.GetZone(
                c3, D2Civilization3System.Zone1Id);
            D2C3ZoneState zone2 = D2Civilization3System.GetZone(
                c3, D2Civilization3System.Zone2Id);
            zone1.researchProgress =
                D2Civilization3System.Zone2UnlockResearchRequirement - 10.0;
            string[] zones = D2Civilization3PresentationRules.GetVisibleZoneIds(c3);
            Check(zones.Length == 2 && zones[1] == D2Civilization3System.Zone2Id &&
                Array.IndexOf(zones, D2Civilization3System.Zone3Id) < 0,
                "Zona 2 no es la única próxima cuando se aproxima su requisito.",
                failures);
            zone2.unlocked = true;
            zone2.researchProgress = 0.0;
            zones = D2Civilization3PresentationRules.GetVisibleZoneIds(c3);
            Check(Array.IndexOf(zones, D2Civilization3System.Zone3Id) < 0,
                "Zona 3 aparece apenas se conoce Zona 2, antes de aproximarse.", failures);
            zone2.researchProgress =
                D2Civilization3System.Zone3UnlockResearchRequirement - 10.0;
            zones = D2Civilization3PresentationRules.GetVisibleZoneIds(c3);
            Check(zones.Length == 3 && zones[2] == D2Civilization3System.Zone3Id,
                "Zona 3 no aparece como siguiente al aproximarse su requisito.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateCluesAnomaliesAndEntity(List<string> failures)
    {
        GameState state = CreateState("Presentation P6 Entity");
        try
        {
            D2Civilization3State c3 = state.dimension2.civilization3;
            D2C3ZoneState zone1 = D2Civilization3System.GetZone(
                c3, D2Civilization3System.Zone1Id);
            c3.anomalyClueDetectionUnlocked = true;
            D2PresentationRules.EnsurePresentationState(state);
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C3Clues).CanOpen &&
                !D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C3Anomalies).IsVisible,
                "Detección de Indicios produce spoiler de Anomalías.", failures);
            zone1.anomalyClues = 1L;
            D2PresentationRules.EnsurePresentationState(state);
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C3Anomalies).CanOpen &&
                !D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C3EntityResearch).IsVisible,
                "Primer Indicio no revela Anomalías correctamente o adelanta el Ente.",
                failures);
            foreach (D2C3ZoneState zone in c3.zones)
                zone.anomalousData = 1L;
            c3.entityResearchUnlocked = true;
            D2PresentationRules.EnsurePresentationState(state);
            FeaturePresentationState entity = D2PresentationRules.GetFeatureState(
                state, PresentationFeatureIds.D2C3EntityResearch);
            Check(entity.CanOpen && entity.isNew,
                "Datos canónicos no presentan investigación del Ente como Nueva.",
                failures);
            c3.entityResearchMilestone100Completed = true;
            c3.entityPactAvailable = true;
            D2PresentationRules.EnsurePresentationState(state);
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C3EntityPact).CanOpen,
                "100% no reemplaza investigación por Pacto avanzado opcional.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateActiveSections(List<string> failures)
    {
        GameState state = CreateState("Presentation P6 Active");
        try
        {
            D2Civilization3State c3 = state.dimension2.civilization3;
            D2C3ZoneState zone1 = D2Civilization3System.GetZone(
                c3, D2Civilization3System.Zone1Id);
            zone1.excavationActive = true;
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C3Archaeology).visualState ==
                    FeaturePresentationVisualState.Active,
                "Excavación activa no fuerza Arqueología visible.", failures);
            zone1.excavationActive = false;
            zone1.analysisActive = true;
            zone1.analysisQualityId = D2Civilization3System.LowQualityId;
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C3Analysis).visualState ==
                    FeaturePresentationVisualState.Active,
                "Análisis activo no fuerza su sección visible.", failures);
            zone1.analysisActive = false;
            c3.entityResearchUnlocked = true;
            c3.entityResearchActive = true;
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C3EntityResearch).visualState ==
                    FeaturePresentationVisualState.Active,
                "Investigación del Ente activa no fuerza su sección visible.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateAdvancedMigration(List<string> failures)
    {
        GameState state = CreateState("Presentation P6 Migration");
        try
        {
            D2Civilization3State c3 = state.dimension2.civilization3;
            D2C3ZoneState zone1 = D2Civilization3System.GetZone(
                c3, D2Civilization3System.Zone1Id);
            zone1.totalExcavationsCompleted = 4L;
            zone1.totalAnalysesCompleted = 3L;
            zone1.researchProgress = 40.0;
            zone1.anomalyClues = 2L;
            c3.archiveUnlocked = true;
            c3.archiveLevel = 2;
            c3.anomalyClueDetectionUnlocked = true;
            c3.entityResearchUnlocked = true;
            state.dimension2.presentation.presentationVersion = 0;
            state.dimension2.presentation.introducedFeatureIds.Clear();
            D2PresentationRules.EnsurePresentationState(state);
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C3Analysis).CanOpen &&
                D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C3Archive).CanOpen &&
                D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C3Clues).CanOpen &&
                D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C3Anomalies).CanOpen &&
                D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C3EntityResearch).CanOpen,
                "Save avanzado de Civ 3 repetiría presentación inicial.", failures);
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
        state.dimension2.civilization3Unlocked = true;
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

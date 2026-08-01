#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class PresentationBlockP4Validation
{
    [MenuItem("Tools/Quantum Forge/Presentation/Validate Block P4")]
    public static void ValidateBlockP4()
    {
        var failures = new List<string>();
        ValidateScene(failures);
        ValidateCivilization1Sequence(failures);
        ValidateAdvancedMigration(failures);
        ValidateFilteredCatalogs(failures);
        if (failures.Count == 0)
        {
            Debug.Log("[Presentation P4] PASS | D2-0 mapa | D2-1 Refugio | " +
                "D2-2 Altares | D2-3 Peregrinación | D2-4 Noviciado | " +
                "D2-5 Ritos | D2-6 Pactos/Civ 2 | D2-7 Umbral");
            return;
        }
        Debug.LogError("[Presentation P4] FAIL\n- " +
            string.Join("\n- ", failures));
    }

    public static void ValidateP4AndRegressionsBatch()
    {
        EditorSceneManager.OpenScene(
            "Assets/Project/Scenes/Main.unity", OpenSceneMode.Single);
        ValidateBlockP4();
        PresentationBlockP3Validation.ValidateBlockP3();
        PresentationBlockP2Validation.ValidateBlockP2();
        PresentationBlockP1Validation.ValidateBlockP1();
        Dimension2Block1UISetup.ValidateBlock1();
        Debug.Log("[Presentation P4 Full] FIN | revisar PASS/FAIL anteriores");
    }

    private static void ValidateScene(List<string> failures)
    {
        Dimension2PanelUI panel = UnityEngine.Object.FindFirstObjectByType<
            Dimension2PanelUI>(FindObjectsInactive.Include);
        Check(panel != null, "Main no contiene Dimension2PanelUI.", failures);
        if (panel == null) return;
        Check(panel.firstEntryTitleText != null &&
            panel.firstEntryDescriptionText != null &&
            panel.continueFirstEntryButton != null,
            "Entrada D2 no tiene título, explicación y CTA conectados.", failures);
        Check(panel.civilization1PanelUI != null &&
            panel.civilization1PanelUI.objectiveText != null,
            "Civilización 1 no tiene objetivo AHORA/DESPUÉS conectado.", failures);
        D2Civilization1PanelUI c1 = panel.civilization1PanelUI;
        if (c1 == null) return;
        Check(c1.altarsPanelUI != null && c1.pilgrimagesPanelUI != null &&
            c1.novitiatePanelUI != null && c1.ritesPanelUI != null &&
            c1.pactsPanelUI != null && c1.veiledThresholdPanelUI != null,
            "Faltan paneles progresivos de Civilización 1.", failures);
    }

    private static void ValidateCivilization1Sequence(List<string> failures)
    {
        GameState state = CreateState("Presentation P4 Sequence");
        try
        {
            D2Civilization1State c1 = state.dimension2.civilization1;
            Check(!D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C1Altars).IsVisible,
                "Partida nueva revela Altares antes de aprender Refugio.", failures);
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C2Regions).visualState ==
                    FeaturePresentationVisualState.Teaser,
                "Mapa inicial no presenta Civ 2 como silueta próxima.", failures);
            Check(!D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C3Archaeology).IsVisible,
                "Mapa inicial revela contenido narrativo de Civ 3.", failures);

            c1.followersAssignedToRefuge = 1L;
            D2PresentationRules.EnsurePresentationState(state);
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C1Altars).CanOpen,
                "Primera asignación no revela Altares.", failures);

            D2AltarState wax = D2AltarSystem.GetAltar(
                c1, D2AltarSystem.WaxAltarId);
            D2AltarState bread = D2AltarSystem.GetAltar(
                c1, D2AltarSystem.RitualBreadAltarId);
            wax.totalOfferingProduced = 1.0;
            bread.totalOfferingProduced = 1.0;
            D2PresentationRules.EnsurePresentationState(state);
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C1Pilgrimages).CanOpen,
                "Producción de Cera y Pan no revela Peregrinaciones.", failures);

            c1.activePilgrimage.active = true;
            c1.activePilgrimage.pilgrimageId = D2PilgrimageSystem.ShortId;
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C1Pilgrimages).visualState ==
                    FeaturePresentationVisualState.Active,
                "Peregrinación activa no fuerza sección visible.", failures);
            c1.activePilgrimage.active = false;
            c1.shortPilgrimagesCompleted = 1L;
            c1.totalPilgrimagesCompleted = 1L;
            c1.mediumPilgrimagesCompleted = 1L;
            D2PresentationRules.EnsurePresentationState(state);
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C1Novitiate).CanOpen,
                "Peregrinación Media no revela Noviciado.", failures);

            c1.totalAcolytesCreated = 1L;
            c1.acolytesAvailable = 1L;
            D2PresentationRules.EnsurePresentationState(state);
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C1Rites).CanOpen,
                "Primer Acólito no revela Ritos.", failures);

            c1.trust = 150.0;
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C1Pacts).visualState ==
                    FeaturePresentationVisualState.Teaser,
                "Pactos no aparece como teaser cerca del umbral.", failures);
            c1.trust = D2CivilizationPactSystem.UnlockTrustRequired;
            c1.novitiateLevel = 2;
            D2PresentationRules.EnsurePresentationState(state);
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C1Pacts).CanOpen,
                "Pactos no usa su API real de disponibilidad.", failures);

            c1.trust = 300.0;
            Dimension2System.Tick(state, 0.1);
            Check(state.dimension2.civilization2Unlocked,
                "Civ 2 no se localiza al alcanzar 300 de Confianza.", failures);
            c1.trust = 499.0;
            c1.entityContactAvailable = false;
            Check(!D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C1VeiledThreshold).IsVisible,
                "Umbral existe visualmente antes de 500.", failures);
            c1.trust = D2VeiledThresholdSystem.UnlockTrustRequired;
            D2PresentationRules.EnsurePresentationState(state);
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C1VeiledThreshold).CanOpen,
                "Umbral no aparece tras el evento canónico de 500.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateAdvancedMigration(List<string> failures)
    {
        GameState state = CreateState("Presentation P4 Migration");
        try
        {
            D2Civilization1State c1 = state.dimension2.civilization1;
            c1.totalFollowersReceived = 100L;
            c1.totalPilgrimagesCompleted = 10L;
            c1.mediumPilgrimagesCompleted = 2L;
            c1.totalAcolytesCreated = 5L;
            c1.acolytesAvailable = 5L;
            c1.trust = 500.0;
            c1.entityContactAvailable = true;
            state.dimension2.presentation.presentationVersion = 0;
            state.dimension2.presentation.introducedFeatureIds.Clear();
            D2PresentationRules.EnsurePresentationState(state);
            Check(D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C1Altars).CanOpen &&
                D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C1Pilgrimages).CanOpen &&
                D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C1Novitiate).CanOpen &&
                D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C1Rites).CanOpen &&
                D2PresentationRules.GetFeatureState(state,
                    PresentationFeatureIds.D2C1VeiledThreshold).CanOpen,
                "Save avanzado no infiere secciones y repetiría tutorial.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateFilteredCatalogs(List<string> failures)
    {
        GameState state = CreateState("Presentation P4 Catalogs");
        try
        {
            D2Civilization1State c1 = state.dimension2.civilization1;
            Check(D2Civilization1PresentationRules.GetVisibleAltarIds(c1).Length == 2,
                "Altares iniciales revelan nombres futuros.", failures);
            Check(D2Civilization1PresentationRules.GetVisibleRiteIds(c1).Length == 2,
                "Ritos iniciales revelan el catálogo completo.", failures);
            Check(D2Civilization1PresentationRules.GetVisiblePactIds(c1).Length == 1,
                "Primera presentación de Pactos revela todo el catálogo.", failures);
            var map = new SafeDropdownOptionMap<string>(StringComparer.Ordinal);
            map.Rebuild(null,
                D2Civilization1PresentationRules.GetVisibleAltarIds(c1),
                value => value, D2AltarSystem.RitualBreadAltarId);
            Check(map.ResolveOrDefault(1, "fallback") ==
                    D2AltarSystem.RitualBreadAltarId &&
                map.ResolveOrDefault(99, "fallback") == "fallback",
                "Dropdown filtrado de Civ 1 no conserva ID seguro.", failures);
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

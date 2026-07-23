#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public static class Dimension3Block7DValidation
{
    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Block 7D")]
    public static void ValidateBlock7D()
    {
        var failures = new List<string>();
        ValidateCorePropagation(failures);
        ValidateCompleteProfiles(failures);
        ValidateCatalog(failures);
        ValidateLastRouteMigrationAndLocalization(failures);
        if (failures.Count == 0)
            Debug.Log("[D3 Block 7D] PASS | Núcleo | perfiles completos | " +
                "catálogo exhaustivo | última ruta | migración | EN/ES");
        else Debug.LogError("[D3 Block 7D] FAIL\n- " +
            string.Join("\n- ", failures));
    }

    private static void ValidateCorePropagation(List<string> failures)
    {
        GameState state = CreateState("D3 B7D Core");
        try
        {
            state.dimension3.assignments.Add(Assignment(
                Dimension3Catalog.FacilityProcessBank,
                Dimension3Catalog.ChannelProcessPower, 100));
            double withoutCore = D3PowerSystem.GetProcessBankModifiers(
                state.dimension3).progressBonusPercent;
            ActivateCore(state, 1);
            double withCore1 = D3PowerSystem.GetProcessBankModifiers(
                state.dimension3).progressBonusPercent;
            ActivateCore(state, 3);
            double withCore3 = D3PowerSystem.GetProcessBankModifiers(
                state.dimension3).progressBonusPercent;
            Check(withCore1 > withoutCore && withCore3 > withCore1,
                "El +5%/+10% del Núcleo no llega a otros bonus.", failures);
            Check(D3FacilitySystem.GetAutomationCoreEfficiencyMultiplier(
                    state.dimension3) == 1.10,
                "El N3 no reemplaza el +5% por +10%.", failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateCompleteProfiles(List<string> failures)
    {
        GameState state = CreateState("D3 B7D Profiles");
        try
        {
            ActivateCore(state, 4);
            state.dimension3.consoleSettings.purchasePolicy = D3ConsoleSystem.PolicyLE;
            state.dimension3.diagnosticSettings.autoFusionEnabled = true;
            state.dimension3.diagnosticSettings.priorityMode = 1;
            state.dimension3.diagnosticSettings.priorityZone = 2;
            D3DiagnosticSystem.RegisterManualFusionRecipe(state,
                ExperimentalFragmentType.Condensation,
                ExperimentalFragmentType.Confinement,
                ExperimentalCatalystType.Alpha);
            state.dimension3.markedFusionRecipes[0].automationMarked = true;
            state.dimension3.calibrationProfiles.Add(new D3CalibrationProfileState
            {
                profileId = "calibration_saved",
                displayName = "Calibración guardada"
            });
            Check(D3AutomationSystem.TrySaveProfile(
                    state, "profile_full", "Completo", out string reason),
                "No guarda perfil completo: " + reason, failures);
            D3AutomationProfileState profile = state.dimension3.automationProfiles[0];
            Check(profile.snapshotVersion == 1 &&
                  profile.savedDiagnosticSettings.autoFusionEnabled &&
                  profile.savedMarkedFusionRecipes.Count == 1 &&
                  profile.savedCalibrationProfiles.Count == 1,
                "El perfil no captura Diagnóstico/recetas/calibración.", failures);
            state.dimension3.consoleSettings.purchasePolicy =
                D3ConsoleSystem.PolicyTraces;
            state.dimension3.diagnosticSettings.autoFusionEnabled = false;
            state.dimension3.markedFusionRecipes[0].automationMarked = false;
            state.dimension3.calibrationProfiles.Clear();
            D3DiagnosticSystem.RegisterManualFusionRecipe(state,
                ExperimentalFragmentType.Confinement,
                ExperimentalFragmentType.Confinement,
                ExperimentalCatalystType.Beta);
            Check(D3AutomationSystem.TryLoadProfile(
                    state, "profile_full", out reason) &&
                  state.dimension3.consoleSettings.purchasePolicy ==
                    D3ConsoleSystem.PolicyLE &&
                  state.dimension3.diagnosticSettings.autoFusionEnabled &&
                  state.dimension3.markedFusionRecipes.Count == 2 &&
                  state.dimension3.markedFusionRecipes[0].automationMarked &&
                  !state.dimension3.markedFusionRecipes[1].automationMarked &&
                  state.dimension3.calibrationProfiles.Count == 1,
                "El perfil completo no restaura o borra historial manual nuevo: " +
                reason, failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateCatalog(List<string> failures)
    {
        Check(D3AutomationCatalog.GetAction(
                D3AutomationCatalog.ActionFactoryProducePart).status ==
              D3AutomationCatalogStatus.Internal,
            "Falta producción interna en catálogo.", failures);
        Check(D3AutomationCatalog.GetAction(
                D3AutomationCatalog.ActionDiagnosticFusion).status ==
              D3AutomationCatalogStatus.Authorized,
            "La fusión segura sigue sin autorizar tras extraer su API.", failures);
        Check(D3AutomationCatalog.GetAction(
                D3AutomationCatalog.ActionDiagnosticCollectFragments).status ==
              D3AutomationCatalogStatus.Prohibited &&
              D3AutomationCatalog.GetAction(
                D3AutomationCatalog.ActionDiagnosticUniqueNode).status ==
              D3AutomationCatalogStatus.Prohibited,
            "Faltan prohibiciones diagnósticas explícitas.", failures);
        Check(D3AutomationCatalog.GetAction(
                D3AutomationCatalog.ActionPortSecondRoutine).status ==
              D3AutomationCatalogStatus.Internal &&
              D3AutomationCatalog.GetAction(
                D3AutomationCatalog.ActionPortUpgradeShip).status ==
              D3AutomationCatalogStatus.PendingDesign &&
              D3AutomationCatalog.GetAction(
                D3AutomationCatalog.ActionPortUpgradeScanner).status ==
              D3AutomationCatalogStatus.PendingDesign,
            "El Puerto no representa todas sus acciones maestras.", failures);
    }

    private static void ValidateLastRouteMigrationAndLocalization(
        List<string> failures)
    {
        GameState state = CreateState("D3 B7D Migration");
        try
        {
            string first = Dimension1System.DestinationMineralBelt;
            string last = Dimension1System.DestinationShipGraveyard;
            state.RegisterManualD1SimpleDestination(first);
            state.RegisterManualD1SimpleDestination(last);
            Check(state.dimension1LastManualSimpleDestinationId == last,
                "No persiste semántica real de última ruta manual.", failures);

            Dimension3State old = JsonUtility.FromJson<Dimension3State>(
                "{\"initialized\":true,\"progressVersion\":6}");
            state.dimension3 = old;
            Dimension3System.EnsureState(state);
            Check(state.dimension3.progressVersion ==
                    Dimension3System.ProgressVersion &&
                  state.dimension3.diagnosticSettings != null &&
                  state.dimension3.diagnosticSettings.savedRoutine != null &&
                  state.dimension3.consoleSettings != null &&
                  state.dimension3.automationProfiles != null,
                "Una partida anterior no migra los campos nuevos.", failures);

            string english = D3RuntimeLocalizer.TranslateText(
                "CONTROL DE DIAGNÓSTICO — NIVEL 5", true);
            string spanish = D3RuntimeLocalizer.TranslateText(english, false);
            Check(english.Contains("DIAGNOSTIC CONTROL") &&
                  english.Contains("LEVEL 5") &&
                  spanish.Contains("CONTROL DE DIAGNÓSTICO") &&
                  spanish.Contains("NIVEL 5"),
                "La capa D3 no conmuta funcionalmente EN/ES.", failures);
        }
        finally { Object.DestroyImmediate(state.gameObject); }
    }

    private static D3AssignmentState Assignment(
        string facility, string channel, long amount)
    {
        return new D3AssignmentState
        {
            installationId = facility,
            channelId = channel,
            mk = 6,
            traitId = Dimension3Catalog.TraitNormal,
            amount = amount,
            stabilizedAmount = amount
        };
    }

    private static void ActivateCore(GameState state, int level)
    {
        D3FacilityState core = D3FacilitySystem.GetFacility(
            state.dimension3, Dimension3Catalog.FacilityAutomationCore);
        core.built = true;
        core.level = level;
        D3AssignmentState assignment = null;
        for (int i = 0; i < state.dimension3.assignments.Count; i++)
            if (state.dimension3.assignments[i].installationId ==
                Dimension3Catalog.FacilityAutomationCore)
                assignment = state.dimension3.assignments[i];
        if (assignment == null)
        {
            assignment = Assignment(Dimension3Catalog.FacilityAutomationCore,
                Dimension3Catalog.ChannelCoreCoordination, 100000);
            state.dimension3.assignments.Add(assignment);
        }
    }

    private static GameState CreateState(string name)
    {
        var go = new GameObject(name) { hideFlags = HideFlags.HideAndDontSave };
        go.SetActive(false);
        GameState state = go.AddComponent<GameState>();
        state.dimension03Unlocked = true;
        state.dimension3 = Dimension3System.CreateInitialState();
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

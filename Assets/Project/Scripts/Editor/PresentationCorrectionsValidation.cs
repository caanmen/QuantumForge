#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;


public static class PresentationCorrectionsValidation
{
    [MenuItem("Tools/Quantum Forge/Presentation/Validate Four Corrections")]
    public static void ValidateCorrections()
    {
        var failures = new List<string>();
        ValidateD2InternalRestorationAndRecognition(failures);
        ValidateD3OnboardingLocalization(failures);
        ValidateD3DropdownSelection(failures);
        if (failures.Count == 0)
        {
            Debug.Log("[Presentation Corrections] PASS | D2 sección interna | " +
                "NUEVO reconocido | D3 onboarding EN/ES | dropdown ID conservado");
            return;
        }
        Debug.LogError("[Presentation Corrections] FAIL\n- " +
            string.Join("\n- ", failures));
    }

    private static void ValidateD2InternalRestorationAndRecognition(
        List<string> failures)
    {
        GameState previous = GameState.I;
        GameObject target = new GameObject("Presentation Corrections D2")
            { hideFlags = HideFlags.HideAndDontSave };
        target.SetActive(false);
        try
        {
            GameState state = target.AddComponent<GameState>();
            state.dimension02Unlocked = true;
            state.dimension2 = Dimension2System.CreateInitialState();
            state.dimension2.firstEntrySeen = true;
            state.dimension2.civilization2Unlocked = true;
            state.dimension2.civilization3Unlocked = true;
            Dimension2System.EnsureState(state);
            SetGameStateSingleton(state);

            D2Civilization1PanelUI c1 = target.AddComponent<D2Civilization1PanelUI>();
            c1.refugeSectionRoot = Child(target, "C1 Refuge");
            c1.altarsSectionRoot = Child(target, "C1 Altars");
            c1.pilgrimagesSectionRoot = Child(target, "C1 Pilgrimages");
            c1.novitiateSectionRoot = Child(target, "C1 Novitiate");
            c1.ritesSectionRoot = Child(target, "C1 Rites");
            c1.pactsSectionRoot = Child(target, "C1 Pacts");
            c1.veiledThresholdSectionRoot = Child(target, "C1 Threshold");

            D2Civilization2PanelUI c2 = target.AddComponent<D2Civilization2PanelUI>();
            c2.regionSectionRoot = Child(target, "C2 Regions");
            c2.operationsSectionRoot = Child(target, "C2 Operations");
            c2.defenseSectionRoot = Child(target, "C2 Defense");
            c2.resistanceSectionRoot = Child(target, "C2 Resistance");
            c2.alertSectionRoot = Child(target, "C2 Alert");
            c2.containmentSectionRoot = Child(target, "C2 Containment");

            D2Civilization3PanelUI c3 = target.AddComponent<D2Civilization3PanelUI>();
            c3.archaeologySectionRoot = Child(target, "C3 Archaeology");
            c3.archiveSectionRoot = Child(target, "C3 Archive");
            c3.entityResearchSectionRoot = Child(target, "C3 Entity");

            Dimension2PanelUI panel = target.AddComponent<Dimension2PanelUI>();
            panel.civilization1PanelUI = c1;
            panel.civilization2PanelUI = c2;
            panel.civilization3PanelUI = c3;

            string[] c1Ids =
            {
                PresentationFeatureIds.D2C1Altars,
                PresentationFeatureIds.D2C1Pilgrimages,
                PresentationFeatureIds.D2C1Novitiate,
                PresentationFeatureIds.D2C1Rites
            };
            GameObject[] c1Roots =
            {
                c1.altarsSectionRoot, c1.pilgrimagesSectionRoot,
                c1.novitiateSectionRoot, c1.ritesSectionRoot
            };
            string[] c2Ids =
            {
                PresentationFeatureIds.D2C2Operations,
                PresentationFeatureIds.D2C2Defense,
                PresentationFeatureIds.D2C2Resistance,
                PresentationFeatureIds.D2C2Containment
            };
            GameObject[] c2Roots =
            {
                c2.operationsSectionRoot, c2.defenseSectionRoot,
                c2.resistanceSectionRoot, c2.containmentSectionRoot
            };
            foreach (string id in c1Ids)
                PresentationStateUtility.Introduce(state.dimension2.presentation, id);
            foreach (string id in c2Ids)
                PresentationStateUtility.Introduce(state.dimension2.presentation, id);
            PresentationStateUtility.Introduce(state.dimension2.presentation,
                PresentationFeatureIds.D2C2Alert);
            state.dimension2.civilization2.alertActive = true;
            state.dimension2.civilization2.containmentAvailable = true;

            for (int i = 0; i < c1Ids.Length; i++)
            {
                Check(D2PresentationRules.GetFeatureState(state, c1Ids[i]).isNew,
                    "Preparación del badge NUEVO C1 inválida: " + c1Ids[i], failures);
                panel.OpenCivilization1ResolvedSection(c1Ids[i]);
                Check(c1Roots[i].activeSelf &&
                    state.dimension2.presentation.lastScreenId == c1Ids[i] &&
                    !D2PresentationRules.GetFeatureState(state, c1Ids[i]).isNew,
                    "C1 no restaura/reconoce la sección " + c1Ids[i], failures);
            }
            for (int i = 0; i < c2Ids.Length; i++)
            {
                Check(D2PresentationRules.GetFeatureState(state, c2Ids[i]).isNew,
                    "Preparación del badge NUEVO C2 inválida: " + c2Ids[i], failures);
                panel.OpenCivilization2ResolvedSection(c2Ids[i]);
                Check(c2Roots[i].activeSelf &&
                    state.dimension2.presentation.lastScreenId == c2Ids[i] &&
                    !D2PresentationRules.GetFeatureState(state, c2Ids[i]).isNew,
                    "C2 no restaura/reconoce la sección " + c2Ids[i], failures);
            }

            state.dimension2.civilization2.alertActive = false;
            state.dimension2.civilization2.containmentAvailable = false;
            state.dimension2.civilization1.activePilgrimage.active = true;
            string activeRoute = D2PresentationRouter.ResolveSafeScreen(
                state, PresentationFeatureIds.D2C1Altars);
            panel.OpenCivilization1ResolvedSection(activeRoute);
            Check(activeRoute == PresentationFeatureIds.D2C1Pilgrimages &&
                    c1.pilgrimagesSectionRoot.activeSelf,
                "Prioridad de actividad D2 no abre su sección interna.", failures);
        }
        finally
        {
            SetGameStateSingleton(previous);
            UnityEngine.Object.DestroyImmediate(target);
        }
    }

    private static void ValidateD3OnboardingLocalization(List<string> failures)
    {
        foreach (string key in PresentationTextCatalog.D3OnboardingRequiredKeys)
        {
            Check(PresentationTextCatalog.HasBothLanguages(key),
                "Falta catálogo ES/EN para " + key, failures);
            ValidateFormattedCatalogEntry(key, false, failures);
            ValidateFormattedCatalogEntry(key, true, failures);
        }

        string[][] stages =
        {
            new[] { "Discovery", "d3.onboarding.notice.factory_ready" },
            new[] { "AssignInitial", "d3.onboarding.assign_initial.title",
                "d3.onboarding.assign_initial.body",
                "d3.onboarding.assign_initial.progress",
                "d3.onboarding.assign_initial.action",
                "d3.onboarding.assign_initial.next" },
            new[] { "FirstPart", "d3.onboarding.first_part.title",
                "d3.onboarding.first_part.body", "d3.onboarding.progress.cost_time",
                "d3.onboarding.first_part.action",
                "d3.onboarding.first_part.action_active",
                "d3.onboarding.first_part.next" },
            new[] { "CompleteSet", "d3.onboarding.complete_set.title",
                "d3.onboarding.complete_set.body",
                "d3.onboarding.complete_set.progress",
                "d3.onboarding.action.produce_part",
                "d3.onboarding.complete_set.next" },
            new[] { "FirstAssembly", "d3.onboarding.first_assembly.title",
                "d3.onboarding.first_assembly.title_restock",
                "d3.onboarding.first_assembly.body",
                "d3.onboarding.first_assembly.body_restock",
                "d3.onboarding.first_assembly.progress_set",
                "d3.onboarding.progress.active_time",
                "d3.onboarding.first_assembly.action",
                "d3.onboarding.first_assembly.action_active",
                "d3.onboarding.first_assembly.next" },
            new[] { "AssignNew", "d3.onboarding.assign_new.title",
                "d3.onboarding.assign_new.body", "d3.onboarding.assign_new.progress",
                "d3.onboarding.assign_new.action", "d3.onboarding.assign_new.next" },
            new[] { "Celebration", "d3.onboarding.celebration",
                "d3.onboarding.notice.cycle_complete" },
            new[] { "Completed", "d3.onboarding.completed.title",
                "d3.onboarding.completed.body", "d3.onboarding.completed.progress",
                "d3.onboarding.completed.action", "d3.onboarding.completed.next" }
        };
        foreach (string[] stage in stages)
        {
            for (int i = 1; i < stage.Length; i++)
                Check(PresentationTextCatalog.HasBothLanguages(stage[i]),
                    "Etapa " + stage[0] + " sin clave bilingüe " + stage[i], failures);
        }

        string[] helpKeys =
        {
            "d3.onboarding.help.title", "d3.onboarding.help.review_title",
            "d3.onboarding.help.review_body", "d3.onboarding.help.assign_initial",
            "d3.onboarding.help.first_part", "d3.onboarding.help.complete_set",
            "d3.onboarding.help.first_assembly", "d3.onboarding.help.assign_new",
            "d3.onboarding.help.completed"
        };
        string[] noticeKeys =
        {
            "d3.onboarding.notice.factory_ready",
            "d3.onboarding.notice.cycle_continue",
            "d3.onboarding.notice.part_queued",
            "d3.onboarding.notice.assembly_started",
            "d3.onboarding.notice.assigned", "d3.onboarding.notice.removed",
            "d3.onboarding.notice.upgrade_queued",
            "d3.onboarding.notice.chassis_received",
            "d3.onboarding.notice.set_complete",
            "d3.onboarding.notice.mk1_built",
            "d3.onboarding.notice.cycle_complete"
        };
        string[] errorKeys =
        {
            "d3.onboarding.error.part_le", "d3.onboarding.error.part_traces",
            "d3.onboarding.error.part", "d3.onboarding.error.assembly",
            "d3.onboarding.error.assignment", "d3.onboarding.error.upgrade"
        };
        ValidateCategory("ayuda", helpKeys, failures);
        ValidateCategory("aviso", noticeKeys, failures);
        ValidateCategory("error", errorKeys, failures);

        ValidateDistinctPair("d3.onboarding.assign_initial.title", "AHORA", "NOW",
            failures);
        ValidateDistinctPair("d3.onboarding.help.review_body", "Asigna", "Assign",
            failures);
        ValidateDistinctPair("d3.onboarding.notice.mk1_built", "construido", "built",
            failures);
        ValidateDistinctPair("d3.onboarding.error.part_traces", "Faltan", "need",
            failures);

        MonoScript panelScript = AssetDatabase.LoadAssetAtPath<MonoScript>(
            "Assets/Project/Scripts/UI/Dimension3PanelUI.cs");
        Check(panelScript != null &&
            !panelScript.text.Contains("D3RuntimeLocalizer.TranslateText"),
            "Dimension3PanelUI depende de D3RuntimeLocalizer.TranslateText.", failures);

        MonoScript localizerScript = AssetDatabase.LoadAssetAtPath<MonoScript>(
            "Assets/Project/Scripts/UI/D3RuntimeLocalizer.cs");
        string[] sourceLocalizedKeys =
        {
            "d3.onboarding.assign_initial.title",
            "d3.onboarding.first_part.title",
            "d3.onboarding.complete_set.title",
            "d3.onboarding.first_assembly.title",
            "d3.onboarding.assign_new.title",
            "d3.onboarding.completed.title",
            "d3.onboarding.help.review_body",
            "d3.onboarding.notice.cycle_complete",
            "d3.onboarding.error.assembly"
        };
        Check(localizerScript != null, "No se encontró D3RuntimeLocalizer.cs.", failures);
        if (localizerScript != null)
        {
            foreach (string key in sourceLocalizedKeys)
            {
                Check(!localizerScript.text.Contains(
                        PresentationTextCatalog.Get(key, false)) &&
                    !localizerScript.text.Contains(
                        PresentationTextCatalog.Get(key, true)),
                    "El texto nuevo sigue como reemplazo parcial en D3RuntimeLocalizer: " +
                    key, failures);
            }
        }
    }

    private static void ValidateFormattedCatalogEntry(
        string key, bool english, List<string> failures)
    {
        string value;
        try
        {
            value = PresentationTextCatalog.Format(key, english,
                "A", "B", "C", "D", "E", "F", "G", "H");
        }
        catch (FormatException exception)
        {
            failures.Add("Placeholder inválido en " + key + " (" +
                (english ? "EN" : "ES") + "): " + exception.Message);
            return;
        }
        Check(!string.IsNullOrEmpty(value) && value != key,
            "Salida vacía o clave visible para " + key + " (" +
            (english ? "EN" : "ES") + ")", failures);
        for (int i = 0; i < 8; i++)
            Check(!value.Contains("{" + i + "}"),
                "Placeholder sin resolver en " + key + " (" +
                (english ? "EN" : "ES") + ")", failures);
    }

    private static void ValidateCategory(
        string category, string[] keys, List<string> failures)
    {
        foreach (string key in keys)
            Check(PresentationTextCatalog.HasBothLanguages(key),
                "Falta clave bilingüe de " + category + ": " + key, failures);
    }

    private static void ValidateDistinctPair(
        string key, string spanishMarker, string englishMarker,
        List<string> failures)
    {
        string spanish = PresentationTextCatalog.Format(key, false, "3", "Chasis");
        string english = PresentationTextCatalog.Format(key, true, "3", "Chassis");
        Check(spanish != english && spanish.Contains(spanishMarker) &&
            english.Contains(englishMarker),
            "Par ES/EN no es semánticamente distinto para " + key, failures);
    }

    private static void ValidateD3DropdownSelection(List<string> failures)
    {
        GameObject target = new GameObject("Presentation Corrections Dropdown",
            typeof(RectTransform)) { hideFlags = HideFlags.HideAndDontSave };
        try
        {
            TMP_Dropdown dropdown = target.AddComponent<TMP_Dropdown>();
            var map = new SafeDropdownOptionMap<int>();
            map.Rebuild(dropdown, new[] { 1, 2 }, value => "V" + value, 2);
            int preferred = Dimension3PanelUI.ResolvePreferredDropdownId(
                map, dropdown, 1);
            map.Rebuild(dropdown, new[] { 1, 2, 3 },
                value => "V" + value, preferred);
            Check(map.ResolveOrDefault(dropdown.value, 1) == 2,
                "Desbloquear V3 reinicia la selección V2 a V1.", failures);

            map.Rebuild(dropdown, new[] { 1, 2, 3 },
                value => "MK" + value, 3);
            preferred = Dimension3PanelUI.ResolvePreferredDropdownId(
                map, dropdown, 1);
            map.Rebuild(dropdown, new[] { 1, 2, 3, 4 },
                value => "MK" + value, preferred);
            Check(map.ResolveOrDefault(dropdown.value, 1) == 3,
                "Desbloquear MK4 reinicia la selección MK3 a MK1.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(target); }
    }

    private static GameObject Child(GameObject parent, string name)
    {
        GameObject value = new GameObject(name);
        value.transform.SetParent(parent.transform, false);
        return value;
    }

    private static void SetGameStateSingleton(GameState state)
    {
        PropertyInfo property = typeof(GameState).GetProperty(
            "I", BindingFlags.Public | BindingFlags.Static);
        property?.SetValue(null, state, null);
    }

    private static void Check(bool condition, string message,
        List<string> failures)
    {
        if (!condition) failures.Add(message);
    }
}
#endif

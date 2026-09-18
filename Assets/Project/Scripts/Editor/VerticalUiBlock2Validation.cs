#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class VerticalUiBlock2Validation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";

    [MenuItem("Tools/Quantum Forge/Vertical UI/Validate Block 2 Navigation")]
    public static void Validate()
    {
        var failures = new List<string>();
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        TabsUI tabs = UnityEngine.Object.FindFirstObjectByType<TabsUI>(
            FindObjectsInactive.Include);
        VerticalNavigationUI navigation =
            UnityEngine.Object.FindFirstObjectByType<VerticalNavigationUI>(
                FindObjectsInactive.Include);
        VerticalSafeAreaLayout safeLayout =
            UnityEngine.Object.FindFirstObjectByType<VerticalSafeAreaLayout>(
                FindObjectsInactive.Include);

        Check(tabs != null, "Falta TabsUI.", failures);
        Check(navigation != null, "Falta VerticalNavigationUI.", failures);
        Check(safeLayout != null, "Falta VerticalSafeAreaLayout.", failures);
        if (tabs != null)
            ValidateTabs(tabs, navigation, failures);
        if (navigation != null)
            ValidateNavigation(navigation, safeLayout, failures);

        ValidatePanelSeparation(scene, tabs, failures);
        ValidateSettings(scene, tabs, failures);
        ValidateProgressiveContract(scene, navigation, failures);
        ValidateLocalization(failures);
        ValidateRuntimeNavigation(tabs, failures);
        ValidateNoDuplicates(scene, failures);
        Finish(failures);
    }

    private static void ValidateTabs(
        TabsUI tabs,
        VerticalNavigationUI navigation,
        List<string> failures)
    {
        CheckNamed(tabs.btnGeneracion, "Nav_Generation", "Generacion", failures);
        CheckNamed(tabs.btnMejoras, "Nav_Upgrades", "Mejoras", failures);
        CheckNamed(tabs.btnLab, "Nav_Research", "Investigacion", failures);
        CheckNamed(tabs.btnAjustes, "Nav_Settings", "Ajustes", failures);
        CheckNamed(tabs.btnQA, "Nav_QA", "QA", failures);
        CheckNamed(tabs.btnRoom2, "System_Room2", "Cuarto 2", failures);
        CheckNamed(tabs.btnDimension1, "System_Dimension1", "Dimension 1", failures);
        CheckNamed(tabs.btnDimension2, "System_Dimension2", "Dimension 2", failures);
        CheckNamed(tabs.btnDimension3, "System_Dimension3", "Dimension 3", failures);
        CheckNamed(tabs.btnPrestigio, "System_Prestige", "Prestigio", failures);
        Check(tabs.panelMejoras != null && tabs.panelMejoras.name == "Panel_Mejoras",
            "Falta Panel_Mejoras.", failures);
        Check(tabs.panelAjustes != null && tabs.panelAjustes.name == "Panel_Ajustes",
            "Falta Panel_Ajustes.", failures);
        Check(tabs.verticalNavigation == navigation,
            "TabsUI no referencia la navegacion vertical.", failures);
        Check(tabs.qaPanel != null && tabs.qaPanel.toolsButton == tabs.btnQA,
            "QA no reutiliza el acceso principal aprobado.", failures);
    }

    private static void ValidateNavigation(
        VerticalNavigationUI navigation,
        VerticalSafeAreaLayout safeLayout,
        List<string> failures)
    {
        Check(navigation.primaryNavigationRoot == safeLayout.primaryNavigationSlot,
            "La barra principal salio de su ranura segura.", failures);
        Check(navigation.secondaryNavigationRoot == safeLayout.secondaryNavigationSlot,
            "La barra secundaria salio de su ranura segura.", failures);
        Check(navigation.secondaryScroll != null &&
            navigation.secondaryScroll.horizontal &&
            !navigation.secondaryScroll.vertical,
            "La barra secundaria no tiene scroll horizontal.", failures);

        Button[] primary =
        {
            navigation.generationButton,
            navigation.upgradesButton,
            navigation.researchButton,
            navigation.settingsButton,
            navigation.qaButton
        };
        Check(primary.Length == 5, "La barra principal no contiene cinco accesos.", failures);
        for (int index = 0; index < primary.Length; index++)
        {
            Check(primary[index] != null,
                "Falta un acceso principal en indice " + index + ".", failures);
            if (primary[index] != null)
                Check(primary[index].transform.GetSiblingIndex() == index,
                    primary[index].name + " esta fuera de orden.", failures);
        }

        Check(navigation.room2Button != null && !navigation.room2Button.gameObject.activeSelf &&
            navigation.dimension1Button != null && !navigation.dimension1Button.gameObject.activeSelf &&
            navigation.dimension2Button != null && !navigation.dimension2Button.gameObject.activeSelf &&
            navigation.dimension3Button != null && !navigation.dimension3Button.gameObject.activeSelf &&
            navigation.prestigeButton != null && !navigation.prestigeButton.gameObject.activeSelf,
            "La escena adelanta sistemas progresivos antes de evaluar gates.", failures);
        Check(navigation.secondaryNavigationRoot != null &&
            !navigation.secondaryNavigationRoot.gameObject.activeSelf,
            "La barra secundaria debe iniciar visualmente ausente.", failures);
    }

    private static void ValidatePanelSeparation(
        Scene scene, TabsUI tabs, List<string> failures)
    {
        GameObject f2 = FindNamed(scene, "F2Panel");
        Check(f2 != null && tabs != null && tabs.panelMejoras != null &&
            f2.transform.parent == tabs.panelMejoras.transform,
            "F2Panel no esta separado dentro de Panel_Mejoras.", failures);
        Check(tabs != null && tabs.panelLab != null &&
            tabs.panelLab.transform.Find("VerticalResearchShell") != null,
            "Investigacion no conserva su host minimo.", failures);
        ResearchUI research = tabs != null && tabs.panelLab != null
            ? tabs.panelLab.GetComponent<ResearchUI>()
            : null;
        Check(research != null && research.enabled &&
            research.listContainer != null && research.itemPrefab != null,
            "ResearchUI no esta funcionalmente conectado.", failures);
    }

    private static void ValidateSettings(
        Scene scene, TabsUI tabs, List<string> failures)
    {
        VerticalSettingsPanelUI settings =
            UnityEngine.Object.FindFirstObjectByType<VerticalSettingsPanelUI>(
                FindObjectsInactive.Include);
        Check(settings != null && tabs != null &&
            settings.gameObject == tabs.panelAjustes,
            "El panel minimo de Ajustes no esta conectado.", failures);
        Check(settings != null && settings.spanishButton != null &&
            settings.englishButton != null && settings.currentLanguageText != null,
            "Ajustes no contiene ambos idiomas y su estado.", failures);

        GameObject legacyRoot = FindNamed(scene, "MobileSafeAreaRoot");
        Check(legacyRoot != null && !legacyRoot.activeSelf,
            "La navegacion horizontal heredada sigue visible.", failures);
        GameObject legacyQa = FindNamed(scene, "QA_ToolsButton");
        Check(legacyQa != null && !legacyQa.activeSelf,
            "El acceso QA heredado sigue duplicado.", failures);
    }

    private static void ValidateProgressiveContract(
        Scene scene,
        VerticalNavigationUI navigation,
        List<string> failures)
    {
        VerticalNavigationUI.EvaluateVisibility(null, null,
            out bool room2,
            out bool d1,
            out bool d2,
            out bool d3,
            out bool prestige);
        Check(!room2 && !d1 && !d2 && !d3 && !prestige,
            "Un estado nulo revela sistemas progresivos.", failures);

        GameState state = UnityEngine.Object.FindFirstObjectByType<GameState>(
            FindObjectsInactive.Include);
        if (state != null)
        {
            bool originalRoom2 = state.experimentalChamberUnlocked;
            bool originalD1 = state.dimension01Unlocked;
            try
            {
                state.experimentalChamberUnlocked = true;
                state.dimension01Unlocked = true;
                VerticalNavigationUI.EvaluateVisibility(state, MachineManager.I,
                    out room2, out d1, out d2, out d3, out prestige);
                Check(room2 && d1,
                    "Cuarto 2 o Dimension 1 no reutilizan sus flags reales.", failures);
            }
            finally
            {
                state.experimentalChamberUnlocked = originalRoom2;
                state.dimension01Unlocked = originalD1;
            }
        }

        string source = File.ReadAllText(
            "Assets/Project/Scripts/UI/Vertical/VerticalNavigationUI.cs");
        string machineSource = File.ReadAllText(
            "Assets/Project/Scripts/UI/MachinePanelUI.cs");
        Check(source.Contains("Dimension2System.CanAccessDimension2(state)") &&
            source.Contains("Dimension3System.CanAccessDimension3(state)") &&
            source.Contains("prestige = false;") &&
            machineSource.Contains("ShowPrestigeFromMachine()") &&
            source.Contains("QaRuntimeService.IsAvailable"),
            "La navegacion no reutiliza todos los gates canonicos.", failures);
    }

    private static void ValidateLocalization(List<string> failures)
    {
        string spanish = File.ReadAllText(
            "Assets/Project/Resources/Localization/lang_es.json");
        string english = File.ReadAllText(
            "Assets/Project/Resources/Localization/lang_en.json");
        string[] keys =
        {
            "nav.generation", "nav.upgrades", "nav.research", "nav.settings",
            "nav.qa", "nav.room2", "nav.dimension1", "nav.dimension2",
            "nav.dimension3", "nav.prestige", "nav.convergence",
            "settings.title", "settings.language.title",
            "settings.language.spanish", "settings.language.english",
            "settings.language.current.es", "settings.language.current.en",
            "research.title"
        };
        foreach (string key in keys)
            Check(spanish.Contains("\"" + key + "\"") &&
                english.Contains("\"" + key + "\""),
                "Falta localizacion ES/EN para " + key + ".", failures);
    }

    private static void ValidateRuntimeNavigation(
        TabsUI tabs, List<string> failures)
    {
        if (tabs == null)
            return;
        bool generationWasActive = tabs.panelGeneracion != null &&
            tabs.panelGeneracion.activeSelf;
        try
        {
            tabs.ShowMejoras();
            Check(tabs.panelMejoras != null && tabs.panelMejoras.activeSelf,
                "ShowMejoras no abre su panel.", failures);
            tabs.ShowLab();
            Check(tabs.panelLab != null && tabs.panelLab.activeSelf,
                "ShowLab no abre Investigacion.", failures);
            tabs.ShowAjustes();
            Check(tabs.panelAjustes != null && tabs.panelAjustes.activeSelf,
                "ShowAjustes no abre su panel.", failures);
            tabs.ShowQA();
            Check(tabs.qaPanel != null && tabs.qaPanel.IsOpen,
                "ShowQA no abre el overlay QA en Editor.", failures);
            if (tabs.qaPanel != null)
                tabs.qaPanel.ClosePanel();

            ValidateQaButtonTap(tabs, failures);
        }
        finally
        {
            if (tabs.panelMejoras != null) tabs.panelMejoras.SetActive(false);
            if (tabs.panelLab != null) tabs.panelLab.SetActive(false);
            if (tabs.panelAjustes != null) tabs.panelAjustes.SetActive(false);
            if (tabs.panelGeneracion != null)
                tabs.panelGeneracion.SetActive(generationWasActive);
        }
    }

    private static void ValidateQaButtonTap(
        TabsUI tabs, List<string> failures)
    {
        if (tabs.btnQA == null || tabs.qaPanel == null)
            return;

        FieldInfo availabilityOverride = typeof(QaRuntimeService).GetField(
            "availabilityOverrideForValidation",
            BindingFlags.NonPublic | BindingFlags.Static);
        try
        {
            if (availabilityOverride != null)
                availabilityOverride.SetValue(null, true);

            InvokePrivate(tabs.qaPanel, "Awake");
            InvokePrivate(tabs.qaPanel, "OnEnable");
            InvokePrivate(tabs, "Awake");

            tabs.qaPanel.ClosePanel();
            tabs.btnQA.onClick.Invoke();
            Check(tabs.qaPanel.IsOpen,
                "Un toque real en Nav_QA no deja abierto el panel.", failures);

            tabs.btnQA.onClick.Invoke();
            Check(!tabs.qaPanel.IsOpen,
                "El segundo toque real en Nav_QA no cierra el panel.", failures);
        }
        finally
        {
            InvokePrivate(tabs.qaPanel, "OnDisable");
            if (availabilityOverride != null)
                availabilityOverride.SetValue(null, null);
            tabs.qaPanel.ClosePanel();
        }
    }

    private static void InvokePrivate(object target, string methodName)
    {
        if (target == null)
            return;
        target.GetType().GetMethod(methodName,
            BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(target, null);
    }

    private static void ValidateNoDuplicates(Scene scene, List<string> failures)
    {
        string[] uniqueNames =
        {
            "Panel_Mejoras", "Panel_Ajustes", "VerticalResearchShell",
            "Nav_Generation", "Nav_Upgrades", "Nav_Research", "Nav_Settings",
            "Nav_QA", "SecondaryScroll", "System_Room2", "System_Dimension1",
            "System_Dimension2", "System_Dimension3", "System_Prestige"
        };
        foreach (string name in uniqueNames)
            Check(CountNamed(scene, name) == 1,
                name + " esta ausente o duplicado.", failures);
    }

    private static void CheckNamed(
        Button button, string expected, string label, List<string> failures)
    {
        Check(button != null && button.name == expected,
            "Falta el boton de " + label + ".", failures);
    }

    private static GameObject FindNamed(Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
            foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
                if (current.name == name)
                    return current.gameObject;
        return null;
    }

    private static int CountNamed(Scene scene, string name)
    {
        int count = 0;
        foreach (GameObject root in scene.GetRootGameObjects())
            foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
                if (current.name == name)
                    count++;
        return count;
    }

    private static void Check(bool condition, string message, List<string> failures)
    {
        if (!condition)
            failures.Add(message);
    }

    private static void Finish(List<string> failures)
    {
        if (failures.Count == 0)
        {
            Debug.Log("[Vertical UI Block 2] PASS | main 5 | progressive systems | " +
                "horizontal scroll | upgrades | research | settings | QA development only");
            return;
        }
        foreach (string failure in failures)
            Debug.LogError("[Vertical UI Block 2] " + failure);
        throw new InvalidOperationException(
            "Vertical UI Block 2 fallo con " + failures.Count + " error(es).");
    }
}
#endif

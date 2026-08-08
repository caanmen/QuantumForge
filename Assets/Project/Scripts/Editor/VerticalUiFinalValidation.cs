#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class VerticalUiFinalValidation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string GeneratedFolder = "Assets/Project/UI/Vertical/Generated";
    private const string ThemePath = GeneratedFolder + "/VerticalUiTheme.asset";

    private static readonly string[] UniqueNames =
    {
        "VerticalUIRoot",
        "VerticalSafeAreaRoot",
        "HeaderSlot",
        "ContentSlot",
        "SecondaryNavigationSlot",
        "PrimaryNavigationSlot",
        "Panel_Mejoras",
        "Panel_Ajustes",
        "VerticalGenerationHeader",
        "GenerationBeforeTriangleRoot",
        "GenerationTriangleRoot",
        "VerticalResearchShell",
        "VerticalUpgradesShell"
    };

    [MenuItem("Tools/Quantum Forge/Vertical UI/Validate Final Connections")]
    public static void Validate()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        var failures = new List<string>();

        VerticalUiSkinRoot skin = FindSingle<VerticalUiSkinRoot>(scene, failures);
        VerticalSafeAreaLayout safe = FindSingle<VerticalSafeAreaLayout>(scene, failures);
        VerticalNavigationUI navigation = FindSingle<VerticalNavigationUI>(scene, failures);
        TabsUI tabs = FindSingle<TabsUI>(scene, failures);
        HUD hud = FindSingle<HUD>(scene, failures);
        VerticalGenerationBeforeTriangleUI generation =
            FindSingle<VerticalGenerationBeforeTriangleUI>(scene, failures);
        VerticalTrianglePresentationUI triangle =
            FindSingle<VerticalTrianglePresentationUI>(scene, failures);
        VerticalUpgradesScreenUI upgrades =
            FindSingle<VerticalUpgradesScreenUI>(scene, failures);
        VerticalSettingsPanelUI settings =
            FindSingle<VerticalSettingsPanelUI>(scene, failures);

        ValidateUniqueHierarchy(scene, failures);
        ValidateSafeArea(safe, skin, failures);
        ValidateTabsAndNavigation(tabs, navigation, safe, failures);
        ValidateManagedPanels(scene, tabs, safe, failures);
        ValidateGeneration(tabs, generation, triangle, hud, failures);
        ValidateUpgrades(tabs, upgrades, failures);
        ValidateSettings(tabs, settings, failures);
        ValidateQa(tabs, navigation, failures);
        ValidateSerializedListeners(tabs, navigation, generation, upgrades,
            settings, failures);
        ValidateThemeAndGeneratedAssets(skin, navigation, failures);
        ValidateMissingComponents(scene, failures);
        Finish(failures);
    }

    private static void ValidateUniqueHierarchy(Scene scene, List<string> failures)
    {
        foreach (string name in UniqueNames)
            Check(CountNamed(scene, name) == 1,
                name + " falta o esta duplicado.", failures);

        Check(FindAll<VerticalUiSkinRoot>(scene).Count == 1,
            "VerticalUiSkinRoot falta o esta duplicado.", failures);
        Check(FindAll<VerticalSafeAreaLayout>(scene).Count == 1,
            "VerticalSafeAreaLayout falta o esta duplicado.", failures);
        Check(FindAll<VerticalNavigationUI>(scene).Count == 1,
            "VerticalNavigationUI falta o esta duplicado.", failures);
        Check(FindAll<VerticalGenerationBeforeTriangleUI>(scene).Count == 1,
            "VerticalGenerationBeforeTriangleUI falta o esta duplicado.", failures);
        Check(FindAll<VerticalUpgradesScreenUI>(scene).Count == 1,
            "VerticalUpgradesScreenUI falta o esta duplicado.", failures);
    }

    private static void ValidateSafeArea(
        VerticalSafeAreaLayout safe,
        VerticalUiSkinRoot skin,
        List<string> failures)
    {
        Check(skin != null && skin.theme != null,
            "VerticalUiSkinRoot no tiene tema.", failures);
        Check(safe != null && safe.safeAreaRoot != null && safe.headerSlot != null &&
            safe.contentSlot != null && safe.secondaryNavigationSlot != null &&
            safe.primaryNavigationSlot != null,
            "VerticalSafeAreaLayout tiene slots sin conectar.", failures);
        if (safe == null || skin == null) return;
        Check(safe.transform.IsChildOf(skin.transform) || safe.transform == skin.transform,
            "Safe Area no vive bajo el root vertical protegido.", failures);
        Check(safe.contentSlot.parent == safe.safeAreaRoot &&
            safe.headerSlot.parent == safe.safeAreaRoot &&
            safe.secondaryNavigationSlot.parent == safe.safeAreaRoot &&
            safe.primaryNavigationSlot.parent == safe.safeAreaRoot,
            "Los slots no viven directamente bajo VerticalSafeAreaRoot.", failures);
    }

    private static void ValidateTabsAndNavigation(
        TabsUI tabs,
        VerticalNavigationUI navigation,
        VerticalSafeAreaLayout safe,
        List<string> failures)
    {
        Check(tabs != null && navigation != null && safe != null,
            "Faltan TabsUI, navegacion o Safe Area.", failures);
        if (tabs == null || navigation == null || safe == null) return;

        Check(tabs.verticalNavigation == navigation && navigation.tabs == tabs,
            "TabsUI y VerticalNavigationUI no estan conectados entre si.", failures);
        Check(navigation.safeAreaLayout == safe &&
            navigation.primaryNavigationRoot == safe.primaryNavigationSlot &&
            navigation.secondaryNavigationRoot == safe.secondaryNavigationSlot,
            "La navegacion no usa los slots seguros.", failures);
        Check(navigation.secondaryScroll != null &&
            navigation.secondaryScroll.horizontal && !navigation.secondaryScroll.vertical,
            "La barra secundaria no conserva scroll horizontal.", failures);

        Button[] tabButtons =
        {
            tabs.btnGeneracion, tabs.btnMejoras, tabs.btnLab, tabs.btnAjustes,
            tabs.btnQA, tabs.btnRoom2, tabs.btnDimension1, tabs.btnDimension2,
            tabs.btnDimension3, tabs.btnPrestigio
        };
        Check(tabButtons.All(button => button != null),
            "TabsUI tiene botones verticales sin conectar.", failures);
        Check(tabs.btnGeneracion == navigation.generationButton &&
            tabs.btnMejoras == navigation.upgradesButton &&
            tabs.btnLab == navigation.researchButton &&
            tabs.btnAjustes == navigation.settingsButton &&
            tabs.btnQA == navigation.qaButton &&
            tabs.btnRoom2 == navigation.room2Button &&
            tabs.btnDimension1 == navigation.dimension1Button &&
            tabs.btnDimension2 == navigation.dimension2Button &&
            tabs.btnDimension3 == navigation.dimension3Button &&
            tabs.btnPrestigio == navigation.prestigeButton,
            "TabsUI no coincide con los botones de VerticalNavigationUI.", failures);

        Check(navigation.generationButton.transform.GetSiblingIndex() == 0 &&
            navigation.upgradesButton.transform.GetSiblingIndex() == 1 &&
            navigation.researchButton.transform.GetSiblingIndex() == 2 &&
            navigation.settingsButton.transform.GetSiblingIndex() == 3 &&
            navigation.qaButton.transform.GetSiblingIndex() == 4,
            "La navegacion principal no conserva el orden aprobado.", failures);
        Check(navigation.room2Button.transform.GetSiblingIndex() == 0 &&
            navigation.dimension1Button.transform.GetSiblingIndex() == 1 &&
            navigation.dimension2Button.transform.GetSiblingIndex() == 2 &&
            navigation.dimension3Button.transform.GetSiblingIndex() == 3 &&
            navigation.prestigeButton.transform.GetSiblingIndex() == 4,
            "La navegacion secundaria no conserva el orden progresivo.", failures);
    }

    private static void ValidateManagedPanels(
        Scene scene,
        TabsUI tabs,
        VerticalSafeAreaLayout safe,
        List<string> failures)
    {
        if (tabs == null || safe?.contentSlot == null) return;
        GameObject[] required =
        {
            tabs.panelGeneracion,
            tabs.panelMejoras,
            tabs.panelLab,
            tabs.panelAjustes,
            tabs.room2Panel,
            tabs.dimension1Panel,
            tabs.dimension2Panel,
            tabs.dimension3Panel,
            tabs.panelLogros,
            tabs.prestigePanel
        };
        foreach (GameObject panel in required)
        {
            Check(panel != null,
                "TabsUI conserva un panel gestionado sin conectar.", failures);
            if (panel != null)
                Check(panel.transform.IsChildOf(safe.contentSlot),
                    panel.name + " esta fuera de ContentSlot.", failures);
        }
        GameObject metaPrestige = FindNamed(scene, "MetaPrestigePanel");
        if (metaPrestige != null)
            Check(metaPrestige.transform.IsChildOf(safe.contentSlot),
                "MetaPrestigePanel esta fuera de ContentSlot.", failures);
    }

    private static void ValidateGeneration(
        TabsUI tabs,
        VerticalGenerationBeforeTriangleUI generation,
        VerticalTrianglePresentationUI triangle,
        HUD hud,
        List<string> failures)
    {
        Check(tabs != null && generation != null && triangle != null && hud != null,
            "Faltan conexiones de Generacion.", failures);
        if (tabs == null || generation == null || hud == null) return;
        Check(tabs.generationDefaultLayout == generation.beforeTriangleRoot &&
            tabs.generationTriangleLayout == generation.triangleRoot &&
            tabs.generationDefaultCanvasGroup != null,
            "Los dos estados de Generacion no coinciden con TabsUI.", failures);
        Check(generation.artifactScroll != null && generation.buildingList != null,
            "Generacion temprana tiene referencias rotas.", failures);
        Check(hud.leText != null && hud.tracesText != null &&
            hud.leText.transform.IsChildOf(tabs.panelGeneracion.transform) &&
            hud.tracesText.transform.IsChildOf(tabs.panelGeneracion.transform),
            "La cabecera comun no esta conectada al HUD real.", failures);
        Check(generation.beforeTriangleRoot.GetComponentsInChildren<F2UpgradeRowUI>(true).Length == 0 &&
            generation.triangleRoot.GetComponentsInChildren<F2UpgradeRowUI>(true).Length == 0,
            "Generacion contiene filas F2.", failures);
    }

    private static void ValidateUpgrades(
        TabsUI tabs,
        VerticalUpgradesScreenUI upgrades,
        List<string> failures)
    {
        Check(tabs?.panelMejoras != null && upgrades != null &&
            upgrades.transform.IsChildOf(tabs.panelMejoras.transform),
            "La pantalla independiente de Mejoras no esta conectada.", failures);
        if (upgrades == null) return;
        Check(upgrades.content != null && upgrades.productionSection != null &&
            upgrades.tracesSection != null && upgrades.triangleSection != null &&
            upgrades.rows != null && upgrades.rows.Length == 8 &&
            upgrades.rows.All(row => row != null),
            "VerticalUpgradesScreenUI tiene referencias incompletas.", failures);
    }

    private static void ValidateSettings(
        TabsUI tabs,
        VerticalSettingsPanelUI settings,
        List<string> failures)
    {
        Check(tabs?.panelAjustes != null && settings != null &&
            settings.transform.IsChildOf(tabs.panelAjustes.transform),
            "Ajustes no esta conectado a su panel vertical.", failures);
        Check(settings != null && settings.spanishButton != null &&
            settings.englishButton != null && settings.currentLanguageText != null,
            "Ajustes tiene referencias de idioma incompletas.", failures);
    }

    private static void ValidateQa(
        TabsUI tabs,
        VerticalNavigationUI navigation,
        List<string> failures)
    {
        Check(tabs != null && navigation != null && tabs.qaPanel != null &&
            tabs.qaPanel.toolsButton == navigation.qaButton,
            "QA no reutiliza el acceso vertical autorizado.", failures);
    }

    private static void ValidateSerializedListeners(
        TabsUI tabs,
        VerticalNavigationUI navigation,
        VerticalGenerationBeforeTriangleUI generation,
        VerticalUpgradesScreenUI upgrades,
        VerticalSettingsPanelUI settings,
        List<string> failures)
    {
        var buttons = new HashSet<Button>();
        AddButtons(buttons, navigation != null ? navigation.primaryNavigationRoot : null);
        AddButtons(buttons, navigation != null ? navigation.secondaryNavigationRoot : null);
        AddButtons(buttons, generation != null ? generation.triangleRoot?.transform : null);
        if (upgrades?.rows != null)
            foreach (F2UpgradeRowUI row in upgrades.rows)
                if (row?.BuyButton != null)
                    buttons.Add(row.BuyButton);
        if (settings != null)
        {
            if (settings.spanishButton != null) buttons.Add(settings.spanishButton);
            if (settings.englishButton != null) buttons.Add(settings.englishButton);
        }
        foreach (Button button in buttons)
        {
            int count = button.onClick.GetPersistentEventCount();
            if (count == 0) continue;
            var calls = new List<string>();
            for (int i = 0; i < count; i++)
            {
                UnityEngine.Object target = button.onClick.GetPersistentTarget(i);
                calls.Add((target != null ? target.GetType().Name : "null") + "." +
                    button.onClick.GetPersistentMethodName(i));
            }
            failures.Add(GetHierarchyPath(button.transform) +
                " conserva listeners serializados: " + string.Join(", ", calls));
        }
    }

    private static void ValidateThemeAndGeneratedAssets(
        VerticalUiSkinRoot skin,
        VerticalNavigationUI navigation,
        List<string> failures)
    {
        VerticalUiTheme theme = AssetDatabase.LoadAssetAtPath<VerticalUiTheme>(ThemePath);
        Check(theme != null && skin != null && skin.theme == theme &&
            navigation != null && navigation.theme == theme,
            "El tema vertical no esta compartido por skin y navegacion.", failures);
        if (theme != null)
        {
            UnityEngine.Object[] required =
            {
                theme.backgroundGrid, theme.panelFrame, theme.buttonFrame,
                theme.selectedButtonFrame, theme.softGlow,
                theme.generationIcon, theme.upgradesIcon, theme.researchIcon,
                theme.settingsIcon, theme.qaIcon, theme.room2Icon,
                theme.dimension1Icon, theme.dimension2Icon,
                theme.dimension3Icon, theme.prestigeIcon,
                theme.higgsArtifactIcon, theme.tetraArtifactIcon,
                theme.modulatorArtifactIcon, theme.primaryFont
            };
            Check(required.All(asset => asset != null),
                "VerticalUiTheme tiene recursos sin conectar.", failures);
        }

        string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D",
            new[] { GeneratedFolder });
        Check(textureGuids.Length >= 15,
            "Faltan texturas modulares generadas.", failures);
        foreach (string guid in textureGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            bool isTiledGrid = path.EndsWith("/qf_vertical_grid.png",
                StringComparison.OrdinalIgnoreCase);
            Check(importer != null && importer.textureType == TextureImporterType.Sprite &&
                !importer.mipmapEnabled && importer.wrapMode ==
                    (isTiledGrid ? TextureWrapMode.Repeat : TextureWrapMode.Clamp),
                path + " no usa importacion UI adecuada.", failures);
            if (importer == null) continue;
            TextureImporterPlatformSettings android =
                importer.GetPlatformTextureSettings("Android");
            Check(android.overridden && android.maxTextureSize <= 256 &&
                android.format == TextureImporterFormat.ASTC_4x4,
                path + " no usa compresion Android ASTC 4x4.", failures);
        }

        GameObject rowPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            GeneratedFolder + "/VerticalBuildingRow.prefab");
        Check(rowPrefab != null && rowPrefab.GetComponent<BuildingRowUI>() != null,
            "VerticalBuildingRow.prefab falta o no conserva BuildingRowUI.", failures);
        if (rowPrefab != null)
        {
            BuildingRowUI row = rowPrefab.GetComponent<BuildingRowUI>();
            Check(row != null && row.buyButton != null &&
                row.buyButton.onClick.GetPersistentEventCount() == 0,
                "VerticalBuildingRow.prefab conserva listeners serializados.", failures);
        }
    }

    private static void ValidateMissingComponents(Scene scene, List<string> failures)
    {
        int missing = 0;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
            {
                Component[] components = current.GetComponents<Component>();
                missing += components.Count(component => component == null);
            }
        }
        Check(missing == 0, "La escena contiene Missing Scripts: " + missing + ".", failures);
    }

    private static void AddButtons(HashSet<Button> buttons, Transform root)
    {
        if (root == null) return;
        foreach (Button button in root.GetComponentsInChildren<Button>(true))
            if (button != null)
                buttons.Add(button);
    }

    private static string GetHierarchyPath(Transform current)
    {
        if (current == null) return "<null>";
        string path = current.name;
        while (current.parent != null)
        {
            current = current.parent;
            path = current.name + "/" + path;
        }
        return path;
    }

    private static T FindSingle<T>(Scene scene, List<string> failures)
        where T : Component
    {
        List<T> all = FindAll<T>(scene);
        Check(all.Count == 1,
            typeof(T).Name + " debe existir exactamente una vez.", failures);
        return all.Count > 0 ? all[0] : null;
    }

    private static List<T> FindAll<T>(Scene scene) where T : Component
    {
        var result = new List<T>();
        foreach (GameObject root in scene.GetRootGameObjects())
            result.AddRange(root.GetComponentsInChildren<T>(true));
        return result;
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
        if (!condition) failures.Add(message);
    }

    private static void Finish(List<string> failures)
    {
        if (failures.Count == 0)
        {
            Debug.Log("[Vertical UI Final Validation] PASS | unique hierarchy | " +
                "safe content | Tabs/HUD/settings/QA connected | zero serialized listeners | " +
                "Android assets | Missing Scripts=0");
            return;
        }
        foreach (string failure in failures)
            Debug.LogError("[Vertical UI Final Validation] " + failure);
        throw new InvalidOperationException(
            "Vertical UI Final Validation fallo con " + failures.Count + " error(es).");
    }
}
#endif

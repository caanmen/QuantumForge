#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class VerticalUiBlock3Validation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string RowPrefabPath =
        "Assets/Project/UI/Vertical/Generated/VerticalBuildingRow.prefab";

    [MenuItem("Tools/Quantum Forge/Vertical UI/Validate Block 3 Generation Before Triangle")]
    public static void Validate()
    {
        var failures = new List<string>();
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        TabsUI tabs = UnityEngine.Object.FindFirstObjectByType<TabsUI>(
            FindObjectsInactive.Include);
        HUD hud = UnityEngine.Object.FindFirstObjectByType<HUD>(
            FindObjectsInactive.Include);
        VerticalGenerationBeforeTriangleUI generation =
            UnityEngine.Object.FindFirstObjectByType<VerticalGenerationBeforeTriangleUI>(
                FindObjectsInactive.Include);

        Check(tabs != null && tabs.panelGeneracion != null,
            "Falta Panel_Generacion o TabsUI.", failures);
        Check(generation != null, "Falta VerticalGenerationBeforeTriangleUI.", failures);
        if (generation != null)
            ValidateGenerationRoot(generation, tabs, failures);
        ValidateHeader(scene, hud, failures);
        ValidateBuildingPipeline(generation, failures);
        ValidateNoTriangleClues(generation, failures);
        ValidateStateSwitch(tabs, failures);
        ValidateLocalization(failures);
        ValidateNoDuplicates(scene, failures);
        Finish(failures);
    }

    private static void ValidateGenerationRoot(
        VerticalGenerationBeforeTriangleUI generation,
        TabsUI tabs,
        List<string> failures)
    {
        Check(generation.beforeTriangleRoot != null &&
            generation.beforeTriangleRoot.name == "GenerationBeforeTriangleRoot",
            "Falta GenerationBeforeTriangleRoot.", failures);
        Check(tabs != null && tabs.generationDefaultLayout == generation.beforeTriangleRoot,
            "TabsUI no referencia el estado temprano.", failures);
        Check(tabs != null && tabs.generationTriangleLayout == generation.triangleRoot,
            "TabsUI no conserva el estado alternativo del Triangulo.", failures);
        Check(generation.beforeTriangleRoot != null &&
            generation.beforeTriangleRoot.activeSelf,
            "El estado temprano no queda activo en la escena base.", failures);
        Check(generation.triangleRoot != null && !generation.triangleRoot.activeSelf,
            "La escena base revela el layout del Triangulo.", failures);
        Check(generation.artifactScroll != null && generation.artifactScroll.vertical &&
            !generation.artifactScroll.horizontal &&
            generation.artifactScroll.viewport != null &&
            generation.artifactScroll.content != null,
            "La lista de artefactos no usa ScrollRect vertical.", failures);
        Check(generation.beforeTriangleRoot != null &&
            generation.beforeTriangleRoot.transform.Find("GenerationTitle") != null,
            "Falta el titulo de Generacion.", failures);
        Check(generation.beforeTriangleRoot != null &&
            generation.beforeTriangleRoot.transform.Find("ArtifactsFrame/ArtifactsTitle") != null,
            "Falta la seccion Artefactos.", failures);
    }

    private static void ValidateHeader(
        Scene scene, HUD hud, List<string> failures)
    {
        GameObject header = FindNamed(scene, "VerticalGenerationHeader");
        Check(header != null, "Falta la cabecera vertical comun.", failures);
        Check(header != null && header.transform.Find("Resource_LE") != null &&
            header.transform.Find("Resource_Traces") != null,
            "La cabecera no contiene los dos recursos equilibrados.", failures);
        Check(hud != null && hud.leText != null && hud.tracesText != null &&
            hud.leText.transform.IsChildOf(header.transform) &&
            hud.tracesText.transform.IsChildOf(header.transform),
            "HUD no actualiza los textos de la nueva cabecera.", failures);
        GameObject legacy = FindNamed(scene, "Panel_HUD");
        Check(legacy != null && !legacy.activeSelf,
            "La cabecera heredada sigue visible o desaparecio.", failures);
    }

    private static void ValidateBuildingPipeline(
        VerticalGenerationBeforeTriangleUI generation,
        List<string> failures)
    {
        BuildingListUI list = generation != null ? generation.buildingList : null;
        Check(list != null && list.gameObject.name == "KnownArtifactsList",
            "No se reutiliza el listado real de edificios.", failures);
        Check(list != null && list.hideLockedRows,
            "Los artefactos desconocidos no se ocultan por completo.", failures);
        Check(list != null && list.rowsParent == list.transform,
            "El listado modular no instancia en su contenido vertical.", failures);
        Check(list != null && list.buildingRowPrefab != null &&
            AssetDatabase.GetAssetPath(list.buildingRowPrefab) == RowPrefabPath,
            "No se usa VerticalBuildingRow.prefab.", failures);
        Check(list != null && list.GetComponent<VerticalLayoutGroup>() != null &&
            list.GetComponent<ContentSizeFitter>() != null,
            "El listado no puede crecer verticalmente.", failures);

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(RowPrefabPath);
        BuildingRowUI row = prefab != null ? prefab.GetComponent<BuildingRowUI>() : null;
        Check(row != null && row.nameText != null && row.statsText != null &&
            row.buyButton != null && row.artifactIcon != null &&
            row.higgsIcon != null && row.tetraIcon != null && row.modulatorIcon != null,
            "El prefab vertical no esta completamente conectado.", failures);
        Check(row != null && row.GetComponent<LayoutElement>() != null &&
            row.GetComponent<LayoutElement>().preferredHeight >= 190f,
            "La fila vertical no tiene altura tactil suficiente.", failures);

        string listSource = File.ReadAllText(
            "Assets/Project/Scripts/Buildings/BuildingListUI.cs");
        string rowSource = File.ReadAllText(
            "Assets/Project/Scripts/Buildings/BuildingRowUI.cs");
        string purchaseSource = File.ReadAllText(
            "Assets/Project/Scripts/Buildings/BuildingPurchaseService.cs");
        Check(listSource.Contains("gs.RegisterBuildingState(state)") &&
            listSource.Contains("rowUI.Init(state, gs)") &&
            listSource.Contains("row.IsKnown"),
            "El listado dejo de registrar, enlazar o filtrar mediante el gate real.", failures);
        Check(rowSource.Contains("private void OnBuyClicked()") &&
            rowSource.Contains("BuildingPurchaseService.TryPurchase(gameState, state)") &&
            purchaseSource.Contains("gameState.LE -= effectiveCost") &&
            purchaseSource.Contains("state.OnPurchased()"),
            "La compra real de edificios fue sustituida.", failures);

        string data = File.ReadAllText(
            "Assets/Project/Resources/Data/buildings.json");
        Check(data.Contains("\"vacuum_observer\"") &&
            data.Contains("\"casimir_panel\"") &&
            data.Contains("\"fluctuation_antenna\"") &&
            data.Contains("\"baseCost\": 10.0") &&
            data.Contains("\"baseCost\": 700.0") &&
            data.Contains("\"baseCost\": 10000.0"),
            "El catalogo canonico de edificios cambio.", failures);
    }

    private static void ValidateNoTriangleClues(
        VerticalGenerationBeforeTriangleUI generation,
        List<string> failures)
    {
        if (generation == null || generation.beforeTriangleRoot == null)
            return;

        Transform root = generation.beforeTriangleRoot.transform;
        Check(root.GetComponentsInChildren<TrianglePanelUI>(true).Length == 0 &&
            root.GetComponentsInChildren<TriangleSlotUI>(true).Length == 0 &&
            root.GetComponentsInChildren<TriangleArtifactCardUI>(true).Length == 0 &&
            root.GetComponentsInChildren<TriangleSelectionUI>(true).Length == 0,
            "El estado temprano contiene componentes del Triangulo.", failures);

        string[] forbidden = { "triangle", "triangulo", "detectado", "???", "locked" };
        foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
        {
            if (current != root)
            {
                string objectName = Normalize(current.name);
                foreach (string token in forbidden)
                    Check(!objectName.Contains(token),
                        "Pista prohibida en el nombre " + current.name + ".", failures);
            }
            TextMeshProUGUI text = current.GetComponent<TextMeshProUGUI>();
            if (text == null)
                continue;
            string value = Normalize(text.text);
            foreach (string token in forbidden)
                Check(!value.Contains(token),
                    "Pista prohibida en el texto de " + current.name + ".", failures);
        }

        Check(root.GetComponentsInChildren<F2UpgradeRowUI>(true).Length == 0,
            "Mejoras F2 reaparecieron dentro de Generacion.", failures);
    }

    private static void ValidateStateSwitch(TabsUI tabs, List<string> failures)
    {
        string tabsSource = File.ReadAllText("Assets/Project/Scripts/UI/TabsUI.cs");
        string controllerSource = File.ReadAllText(
            "Assets/Project/Scripts/UI/Vertical/VerticalGenerationBeforeTriangleUI.cs");
        Check(tabsSource.Contains("GameState.I.triangleSystemUnlocked") &&
            tabsSource.Contains("generationTriangleLayout.SetActive(triangleUnlocked)") &&
            tabsSource.Contains("generationDefaultLayout.SetActive(!triangleUnlocked)"),
            "TabsUI no alterna los dos estados con triangleSystemUnlocked.", failures);
        Check(controllerSource.Contains("GameState.I.triangleSystemUnlocked") &&
            controllerSource.Contains("beforeTriangleRoot.SetActive(!unlocked)") &&
            controllerSource.Contains("triangleRoot.SetActive(unlocked)"),
            "El supervisor del estado temprano no usa el gate canonico.", failures);
    }

    private static void ValidateLocalization(List<string> failures)
    {
        string spanish = File.ReadAllText(
            "Assets/Project/Resources/Localization/lang_es.json");
        string english = File.ReadAllText(
            "Assets/Project/Resources/Localization/lang_en.json");
        string[] keys = { "hud.le", "hud.traces", "generation.title", "generation.artifacts" };
        foreach (string key in keys)
            Check(spanish.Contains("\"" + key + "\"") &&
                english.Contains("\"" + key + "\""),
                "Falta localizacion ES/EN para " + key + ".", failures);
    }

    private static void ValidateNoDuplicates(Scene scene, List<string> failures)
    {
        string[] unique =
        {
            "VerticalGenerationHeader", "Resource_LE", "Resource_Traces",
            "GenerationBeforeTriangleRoot", "ArtifactsFrame", "ArtifactsScroll",
            "KnownArtifactsList"
        };
        foreach (string name in unique)
            Check(CountNamed(scene, name) == 1,
                name + " esta ausente o duplicado.", failures);
    }

    private static string Normalize(string value)
    {
        return (value ?? string.Empty).ToLowerInvariant()
            .Replace("á", "a").Replace("é", "e").Replace("í", "i")
            .Replace("ó", "o").Replace("ú", "u");
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
            Debug.Log("[Vertical UI Block 3] PASS | common header | early generation | " +
                "known artifacts | real gates and purchases | vertical scroll | zero triangle clues");
            return;
        }

        foreach (string failure in failures)
            Debug.LogError("[Vertical UI Block 3] " + failure);
        throw new InvalidOperationException(
            "Vertical UI Block 3 fallo con " + failures.Count + " error(es).");
    }
}
#endif

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MobileNumericLegibilityPolish
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ResearchPrefabPath =
        "Assets/Project/Prefabs/UI/ResearchItem.prefab";

    [MenuItem("Tools/Quantum Forge/Vertical UI/Apply Mobile Numeric Legibility")]
    public static void ApplyAndValidate()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        GameObject machineRoot = FindNamed(scene, "MachineCubeVisualRoot");
        Require(machineRoot != null, "Falta MachineCubeVisualRoot.");

        StyleAll(machineRoot.transform, "LE", 28f, 21f);
        StyleAll(machineRoot.transform, "Traces", 28f, 21f);
        StyleAll(machineRoot.transform, "TotalProgress", 20f, 17f);
        StyleAll(machineRoot.transform, "Convergence", 20f, 17f);
        StyleAll(machineRoot.transform, "FaceIndex", 26f, 20f);
        StyleAll(machineRoot.transform, "Tier", 18f, 16f);
        StyleAll(machineRoot.transform, "NodeState", 22f, 18f);
        StyleAll(machineRoot.transform, "NodeEffect", 22f, 17f);
        StyleAll(machineRoot.transform, "NodeRequirements", 21f, 17f);
        StyleAll(machineRoot.transform, "NodeCost", 22f, 18f);
        StyleAll(machineRoot.transform, "FaceProgress", 19f, 17f);

        EditorSceneManager.MarkSceneDirty(scene);
        Require(EditorSceneManager.SaveScene(scene), "No se pudo guardar Main.unity.");

        StyleResearchCost();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        ValidateScene(scene);
        ValidateResearchPrefab();
        MachineCubeBlock1Validation.Validate();
        Debug.Log("[Mobile Numeric Legibility] PASS | recursos 28-29 | " +
            "costes y tasas 21-23 | microestados 18-20 | titulos intactos");
    }

    public static void ApplyAndValidateBatch()
    {
        try
        {
            ApplyAndValidate();
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    private static void StyleResearchCost()
    {
        GameObject root = PrefabUtility.LoadPrefabContents(ResearchPrefabPath);
        Require(root != null, "No se pudo abrir ResearchItem.prefab.");
        try
        {
            TextMeshProUGUI cost = root.transform.Find("Cost_Text")
                ?.GetComponent<TextMeshProUGUI>();
            Require(cost != null, "ResearchItem perdio Cost_Text.");
            cost.fontSize = 20f;
            cost.fontSizeMax = 20f;
            cost.fontSizeMin = 18f;
            cost.enableAutoSizing = true;
            cost.fontStyle = FontStyles.Bold;
            cost.overflowMode = TextOverflowModes.Truncate;

            RectTransform rect = cost.rectTransform;
            float rightEdge = rect.anchoredPosition.x + rect.sizeDelta.x *
                (1f - rect.pivot.x);
            rect.sizeDelta = new Vector2(340f, 36f);
            rect.anchoredPosition = new Vector2(
                rightEdge - rect.sizeDelta.x * (1f - rect.pivot.x), 29f);
            PrefabUtility.SaveAsPrefabAsset(root, ResearchPrefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ValidateScene(Scene scene)
    {
        GameObject machineRoot = FindNamed(scene, "MachineCubeVisualRoot");
        Require(machineRoot != null, "Falta la maquina durante validacion.");
        Require(AllMeet(machineRoot.transform, "LE", 28f, 21f),
            "El contador LE de la maquina sigue pequeno.");
        Require(AllMeet(machineRoot.transform, "Traces", 28f, 21f),
            "El contador de Trazas de la maquina sigue pequeno.");
        Require(AllMeet(machineRoot.transform, "NodeCost", 22f, 18f),
            "El coste del nodo sigue pequeno.");
        Require(AllMeet(machineRoot.transform, "FaceProgress", 19f, 17f),
            "El progreso del sector sigue pequeno.");
    }

    private static void ValidateResearchPrefab()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(ResearchPrefabPath);
        TextMeshProUGUI cost = prefab != null
            ? prefab.transform.Find("Cost_Text")?.GetComponent<TextMeshProUGUI>()
            : null;
        Require(cost != null && cost.fontSize >= 20f && cost.fontSizeMin >= 18f &&
            cost.rectTransform.sizeDelta.x >= 340f,
            "El coste de Investigacion sigue pequeno o sin espacio suficiente.");
    }

    private static void StyleAll(
        Transform root, string name, float size, float minimum)
    {
        int count = 0;
        foreach (TextMeshProUGUI text in root.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (text.name != name) continue;
            text.fontSize = size;
            text.fontSizeMax = size;
            text.fontSizeMin = minimum;
            text.enableAutoSizing = true;
            EditorUtility.SetDirty(text);
            count++;
        }
        Require(count > 0, "No se encontro texto " + name + " en la maquina.");
    }

    private static bool AllMeet(
        Transform root, string name, float size, float minimum)
    {
        var matches = new List<TextMeshProUGUI>();
        foreach (TextMeshProUGUI text in root.GetComponentsInChildren<TextMeshProUGUI>(true))
            if (text.name == name) matches.Add(text);
        return matches.Count > 0 && matches.TrueForAll(text =>
            text.fontSize >= size && text.fontSizeMin >= minimum);
    }

    private static GameObject FindNamed(Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
            foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
                if (current.name == name) return current.gameObject;
        return null;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
#endif

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class QaMainSceneIntegrityValidation
{
    private const string MainScenePath = "Assets/Project/Scenes/Main.unity";
    private const string RoundTripScenePath =
        "Assets/Project/Scenes/__QA_MainSceneRoundTrip.unity";

    [MenuItem("Tools/Quantum Forge/QA/Validate Main Scene Integrity")]
    public static void ValidateMainSceneIntegrity()
    {
        var failures = new List<string>();

        try
        {
            AssetDatabase.DeleteAsset(RoundTripScenePath);

            Scene main = EditorSceneManager.OpenScene(
                MainScenePath, OpenSceneMode.Single);
            ValidateLoadedScene(main, "apertura original", failures);

            bool saved = EditorSceneManager.SaveScene(
                main, RoundTripScenePath, true);
            Check(saved, "No fue posible guardar la copia temporal.", failures);

            if (saved)
            {
                Scene roundTrip = EditorSceneManager.OpenScene(
                    RoundTripScenePath, OpenSceneMode.Single);
                ValidateLoadedScene(roundTrip, "reapertura de copia", failures);
            }

            EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
        }
        catch (Exception exception)
        {
            failures.Add("Excepción al abrir/guardar/reabrir: " + exception.Message);
        }
        finally
        {
            AssetDatabase.DeleteAsset(RoundTripScenePath);
            AssetDatabase.Refresh();
        }

        if (failures.Count > 0)
        {
            Debug.LogError("[QA Main Scene Integrity] FAIL\n- " +
                string.Join("\n- ", failures));
            throw new InvalidOperationException(
                "Main.unity no superó la validación estructural.");
        }

        Debug.Log("[QA Main Scene Integrity] PASS | apertura limpia | " +
            "GameState=1 | Missing Scripts=0 | referencias locales rotas=0 | " +
            "guardado y reapertura correctos");
    }

    private static void ValidateLoadedScene(
        Scene scene, string stage, List<string> failures)
    {
        Check(scene.IsValid() && scene.isLoaded,
            stage + ": la escena no quedó cargada.", failures);
        if (!scene.IsValid() || !scene.isLoaded)
            return;

        GameState[] gameStates = UnityEngine.Object.FindObjectsByType<GameState>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        Check(gameStates.Length == 1,
            stage + ": GameState encontrados=" + gameStates.Length + ".",
            failures);

        int missingScripts = 0;
        int brokenLocalReferences = 0;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            Transform[] transforms = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform transform in transforms)
            {
                GameObject gameObject = transform.gameObject;
                missingScripts += GameObjectUtility
                    .GetMonoBehavioursWithMissingScriptCount(gameObject);
                brokenLocalReferences += CountBrokenLocalReferences(gameObject);
            }
        }

        Check(missingScripts == 0,
            stage + ": Missing Scripts=" + missingScripts + ".", failures);
        Check(brokenLocalReferences == 0,
            stage + ": referencias locales rotas=" + brokenLocalReferences + ".",
            failures);
    }

    private static int CountBrokenLocalReferences(GameObject gameObject)
    {
        int count = 0;
        Component[] components = gameObject.GetComponents<Component>();
        foreach (Component component in components)
        {
            if (component == null)
                continue;

            try
            {
                var serializedObject = new SerializedObject(component);
                SerializedProperty property = serializedObject.GetIterator();
                bool enterChildren = true;
                while (property.Next(enterChildren))
                {
                    enterChildren = true;
                    if (property.propertyType !=
                        SerializedPropertyType.ObjectReference)
                        continue;
                    if (property.objectReferenceValue == null &&
                        property.objectReferenceInstanceIDValue != 0)
                        count++;
                }
            }
            catch (Exception)
            {
                count++;
            }
        }
        return count;
    }

    private static void Check(
        bool condition, string failure, List<string> failures)
    {
        if (!condition)
            failures.Add(failure);
    }
}
#endif

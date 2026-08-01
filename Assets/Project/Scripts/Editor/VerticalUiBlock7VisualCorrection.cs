#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class VerticalUiBlock7VisualCorrection
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";

    public static void ApplyAndValidateBatch()
    {
        try
        {
            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            RectTransform artifacts = FindUnique(scene, "TriangleArtifactsTitle");
            RectTransform effect = FindUnique(scene, "CircuitEffect");

            artifacts.anchoredPosition = new Vector2(0f, -1124f);
            artifacts.sizeDelta = new Vector2(-56f, 64f);
            effect.anchoredPosition = new Vector2(0f, -390f);

            EditorUtility.SetDirty(artifacts);
            EditorUtility.SetDirty(effect);
            EditorSceneManager.MarkSceneDirty(scene);
            if (!EditorSceneManager.SaveScene(scene, ScenePath))
                throw new InvalidOperationException("No se pudo guardar la correccion visual del Bloque 7.");

            VerticalUiFinalValidation.Validate();
            Debug.Log("[Vertical UI Block 7 Visual Correction] PASS | " +
                "ARTEFACTOS inset | efecto dentro del panel | escena validada");
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    private static RectTransform FindUnique(Scene scene, string name)
    {
        RectTransform found = null;
        int count = 0;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (RectTransform current in
                root.GetComponentsInChildren<RectTransform>(true))
            {
                if (current.name != name)
                    continue;
                found = current;
                count++;
            }
        }

        if (count != 1 || found == null)
            throw new InvalidOperationException(
                name + " debe existir exactamente una vez; encontrados: " + count + ".");
        return found;
    }
}
#endif

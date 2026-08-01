#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Dimension1DarkThemeSetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";

    [MenuItem("Quantum Forge/Dimension 1/Apply Dark Theme")]
    public static void ApplyDarkTheme()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        GameObject host = GameObject.Find("Canvas");
        if (host == null) throw new System.InvalidOperationException("No se encontró Canvas en Main.");
        var theme = host.GetComponent<Dimension1DarkThemeRuntime>();
        if (theme == null) theme = host.AddComponent<Dimension1DarkThemeRuntime>();
        theme.ApplyTheme();
        EditorUtility.SetDirty(host);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[Dimension1DarkThemeSetup] Tema oscuro aplicado sin modificar layout.");
    }
}
#endif

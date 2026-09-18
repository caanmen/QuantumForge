#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public sealed class UIScreenWorkspaceWindow : EditorWindow
{
    private string dimension = "DIMENSION_1";
    private string screenName = "NUEVA_PANTALLA";
    private string displayName = "Nueva pantalla";

    [MenuItem("Quantum Forge/UI Production/Nueva carpeta de pantalla")]
    private static void Open()
    {
        GetWindow<UIScreenWorkspaceWindow>(true, "Nueva pantalla UI", true);
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Espacio documental de pantalla", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Crea ficha, contrato y registro de decisiones. No crea ni modifica la pantalla visual.",
            MessageType.Info);
        dimension = EditorGUILayout.TextField("Dimensión", dimension);
        screenName = EditorGUILayout.TextField("ID de carpeta", screenName);
        displayName = EditorGUILayout.TextField("Nombre visible", displayName);

        using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(dimension) ||
                                           string.IsNullOrWhiteSpace(screenName)))
        {
            if (GUILayout.Button("Crear carpeta documental"))
                CreateWorkspace(dimension, screenName, displayName);
        }
    }

    public static string CreateWorkspace(string dimensionId, string screenId, string visibleName)
    {
        string safeDimension = Sanitize(dimensionId);
        string safeScreen = Sanitize(screenId);
        string root = Directory.GetParent(Application.dataPath).FullName;
        string folder = Path.Combine(root, "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE", "10_PANTALLAS", safeDimension, safeScreen);
        Directory.CreateDirectory(folder);

        string templateRoot = Path.Combine(root, "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE", "02_PLANTILLAS");
        CopyOnce(
            Path.Combine(templateRoot, "FICHA_NUEVA_PANTALLA.txt"),
            Path.Combine(folder, "FICHA_PANTALLA.txt"));
        CopyOnce(
            Path.Combine(templateRoot, "CONTRATO_APROBACION_PANTALLA.txt"),
            Path.Combine(folder, "CONTRATO_APROBACION.txt"));

        string decisions = Path.Combine(folder, "DECISIONES.txt");
        if (!File.Exists(decisions))
        {
            File.WriteAllText(decisions,
                "DECISIONES ESPECÍFICAS: " + visibleName.ToUpperInvariant() + "\n" +
                "========================================\n\n" +
                "Registrar únicamente decisiones confirmadas por el usuario.\n",
                new UTF8Encoding(false));
        }

        AssetDatabase.Refresh();
        Debug.Log("[UI Production System] SCREEN_WORKSPACE_CREATED | " + folder);
        return folder;
    }

    private static void CopyOnce(string source, string destination)
    {
        if (!File.Exists(destination))
            File.Copy(source, destination, false);
    }

    private static string Sanitize(string value)
    {
        string result = value.Trim().ToUpperInvariant().Replace(' ', '_');
        foreach (char invalid in Path.GetInvalidFileNameChars())
            result = result.Replace(invalid.ToString(), string.Empty);
        result = result.Replace("..", string.Empty).Replace("/", string.Empty).Replace("\\", string.Empty);
        return string.IsNullOrWhiteSpace(result) ? "SIN_NOMBRE" : result;
    }
}
#endif

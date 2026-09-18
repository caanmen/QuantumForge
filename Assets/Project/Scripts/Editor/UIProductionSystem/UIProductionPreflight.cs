#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class UIProductionPreflight
{
    private const string SystemRoot = "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE";

    [MenuItem("Quantum Forge/UI Production/Ejecutar comprobación general")]
    public static void Run()
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        List<string> failures = new List<string>();
        List<string> warnings = new List<string>();
        List<string> checks = new List<string>();

        RequireFile(projectRoot, "AGENTS.md", 100, failures, checks);
        RequireFile(projectRoot, "REGLAS_DE_TRABAJO_Y_CONTINUIDAD.md", 100, failures, checks);
        RequireFile(projectRoot, SystemRoot + "/00_INICIO/INSTRUCCIONES_MAESTRAS_UI.md", 500, failures, checks);
        RequireFile(projectRoot, SystemRoot + "/01_GUIAS/GUIA_PREVENCION_ERRORES_PANTALLAS.txt", 1000, failures, checks);
        RequireFile(projectRoot, SystemRoot + "/01_GUIAS/GUIA_CREACION_CORRECTA_PANTALLAS.txt", 1000, failures, checks);
        RequireFile(projectRoot, SystemRoot + "/07_REGISTROS/PANTALLAS_UI.csv", 20, failures, checks);
        RequireFile(projectRoot, SystemRoot + "/08_PRUEBAS/MATRIZ_ESCENARIOS_QA.csv", 20, failures, checks);
        RequireFile(projectRoot, "Assets/Project/Scenes/Main.unity", 100, failures, checks);

        RequireDirectory(projectRoot, SystemRoot + "/05_REFERENCIAS", failures, checks);
        RequireDirectory(projectRoot, SystemRoot + "/06_CAPTURAS_APROBADAS", failures, checks);
        RequireDirectory(projectRoot, SystemRoot + "/10_PANTALLAS", failures, checks);

        VerticalUiTheme theme = AssetDatabase.LoadAssetAtPath<VerticalUiTheme>(
            "Assets/Project/UI/Vertical/Generated/VerticalUiTheme.asset");
        if (theme == null)
            failures.Add("No se pudo cargar VerticalUiTheme.asset.");
        else
            checks.Add("OK tema compartido: VerticalUiTheme.asset (schema " + theme.schemaVersion + ").");

        CheckRegistryForTemporaryPaths(projectRoot, failures, warnings, checks);
        CheckKnownReference(projectRoot,
            SystemRoot + "/05_REFERENCIAS/DIMENSION_1/CENTRO_DE_MANDO/referencia_centro_de_mando.png",
            failures, checks);
        CheckKnownReference(projectRoot,
            SystemRoot + "/05_REFERENCIAS/DIMENSION_1/CARTA_GALACTICA/referencia_carta_galactica.png",
            failures, checks);

        string logDirectory = Path.Combine(projectRoot, "Logs", "UIProductionSystem");
        Directory.CreateDirectory(logDirectory);
        string reportPath = Path.Combine(logDirectory, "preflight_report.txt");
        File.WriteAllText(reportPath, BuildReport(checks, warnings, failures), new UTF8Encoding(false));

        AssetDatabase.Refresh();
        if (failures.Count > 0)
        {
            string message = "[UI Production System] PREFLIGHT_FAIL | " + failures.Count +
                             " fallos. Informe: " + reportPath;
            Debug.LogError(message);
            throw new InvalidOperationException(message);
        }

        Debug.Log("[UI Production System] PREFLIGHT_PASS | " + checks.Count +
                  " comprobaciones, " + warnings.Count + " avisos. Informe: " + reportPath);
    }

    private static void RequireFile(
        string projectRoot,
        string relativePath,
        long minimumBytes,
        List<string> failures,
        List<string> checks)
    {
        string absolutePath = Combine(projectRoot, relativePath);
        if (!File.Exists(absolutePath))
        {
            failures.Add("Falta archivo: " + relativePath);
            return;
        }

        long length = new FileInfo(absolutePath).Length;
        if (length < minimumBytes)
        {
            failures.Add("Archivo vacío o incompleto: " + relativePath + " (" + length + " bytes).");
            return;
        }

        checks.Add("OK archivo: " + relativePath + " (" + length + " bytes).");
    }

    private static void RequireDirectory(
        string projectRoot,
        string relativePath,
        List<string> failures,
        List<string> checks)
    {
        string absolutePath = Combine(projectRoot, relativePath);
        if (!Directory.Exists(absolutePath))
        {
            failures.Add("Falta carpeta: " + relativePath);
            return;
        }

        checks.Add("OK carpeta: " + relativePath + ".");
    }

    private static void CheckRegistryForTemporaryPaths(
        string projectRoot,
        List<string> failures,
        List<string> warnings,
        List<string> checks)
    {
        string path = Combine(projectRoot, SystemRoot + "/07_REGISTROS/PANTALLAS_UI.csv");
        if (!File.Exists(path))
            return;

        string content = File.ReadAllText(path);
        if (content.IndexOf("AppData\\Local\\Temp", StringComparison.OrdinalIgnoreCase) >= 0 ||
            content.IndexOf("AppData/Local/Temp", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            failures.Add("El registro de pantallas contiene una ruta temporal.");
        }
        else
        {
            checks.Add("OK registro sin rutas temporales.");
        }

        if (content.IndexOf("95", StringComparison.OrdinalIgnoreCase) < 0)
            warnings.Add("El registro no menciona explícitamente la meta de 95 %; revisar sus columnas.");
    }

    private static void CheckKnownReference(
        string projectRoot,
        string relativePath,
        List<string> failures,
        List<string> checks)
    {
        string path = Combine(projectRoot, relativePath);
        if (!File.Exists(path))
        {
            failures.Add("Falta referencia permanente: " + relativePath);
            return;
        }

        byte[] bytes = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        try
        {
            if (!ImageConversion.LoadImage(texture, bytes, false))
            {
                failures.Add("No se pudo leer la imagen: " + relativePath);
                return;
            }
            checks.Add("OK referencia: " + relativePath + " (" + texture.width + "x" + texture.height + ").");
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(texture);
        }
    }

    private static string BuildReport(
        List<string> checks,
        List<string> warnings,
        List<string> failures)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("UI PRODUCTION SYSTEM - PREFLIGHT");
        builder.AppendLine("Fecha: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        builder.AppendLine();
        builder.AppendLine("COMPROBACIONES CORRECTAS (" + checks.Count + ")");
        foreach (string item in checks)
            builder.AppendLine("- " + item);
        builder.AppendLine();
        builder.AppendLine("AVISOS (" + warnings.Count + ")");
        foreach (string item in warnings)
            builder.AppendLine("- " + item);
        builder.AppendLine();
        builder.AppendLine("FALLOS (" + failures.Count + ")");
        foreach (string item in failures)
            builder.AppendLine("- " + item);
        return builder.ToString();
    }

    private static string Combine(string projectRoot, string relativePath)
    {
        return Path.Combine(projectRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));
    }
}
#endif

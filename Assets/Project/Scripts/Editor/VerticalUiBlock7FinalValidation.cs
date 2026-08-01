#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class VerticalUiBlock7FinalValidation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string CaptureDirectory =>
        Path.GetFullPath("Logs/VisualQA/VerticalUIBlock7");

    public static void ValidateBatch()
    {
        try
        {
            VerticalUiFinalSetup.RunValidations();
            QaMainSceneIntegrityValidation.ValidateMainSceneIntegrity();
            BuildingSceneReloadValidation.Validate();
            QaBlock1Validation.ValidateBlock1();
            SpanishReleaseReadinessValidation.Validate();
            VerticalGenerationVisualPolishValidation.Validate();
            VerticalUpgradesVisualPolishValidation.Validate();

            ValidateVisualCorrections();
            ValidateCaptures();
            Debug.Log("[Vertical UI Block 7 Final] PASS | regresiones 1-6 | " +
                "partida anterior | QA release/editor | escena round-trip | " +
                "7 capturas verificadas | pulido visual de Generacion y Mejoras estable");
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    private static void ValidateVisualCorrections()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        RectTransform artifacts = FindUnique(scene, "TriangleArtifactsTitle");
        RectTransform effect = FindUnique(scene, "CircuitEffect");
        RectTransform focus = FindUnique(scene, "TriangleFocus");
        RectTransform gauge = FindUnique(scene, "EnergyGauge");
        Require(!artifacts.gameObject.activeSelf,
            "El rotulo antiguo de ARTEFACTOS reaparecio sobre las filas compactas.");
        Require(Mathf.Approximately(effect.anchoredPosition.y, -120f),
            "CircuitEffect no conserva su separacion respecto a los selectores.");
        Require(Mathf.Approximately(focus.sizeDelta.y, 1090f) &&
            Mathf.Approximately(gauge.sizeDelta.x, 178f),
            "La composicion central o el medidor perdieron sus proporciones finales.");

        string spanish = File.ReadAllText(
            "Assets/Project/Resources/Localization/lang_es.json");
        Require(spanish.Contains("\"generation.title\": \"GENERACIÓN\""),
            "El titulo espanol de Generacion perdio la tilde.");
    }

    private static void ValidateCaptures()
    {
        var expected = new Dictionary<string, Vector2Int>
        {
            { "01_generation_initial_es_1080x1920.png", new Vector2Int(1080, 1920) },
            { "02_upgrades_early_es_1080x2340.png", new Vector2Int(1080, 2340) },
            { "03_upgrades_triangle_es_1080x2340.png", new Vector2Int(1080, 2340) },
            { "04_generation_energy_en_1080x2340.png", new Vector2Int(1080, 2340) },
            { "05_generation_phase_locked_es_1080x1920.png", new Vector2Int(1080, 1920) },
            { "06_secondary_navigation_es_720x1600.png", new Vector2Int(720, 1600) },
            { "07_safe_area_notch_es_1080x2340.png", new Vector2Int(1080, 2340) }
        };

        foreach (KeyValuePair<string, Vector2Int> item in expected)
        {
            string path = Path.Combine(CaptureDirectory, item.Key);
            Require(File.Exists(path) && new FileInfo(path).Length > 4096,
                "Falta una captura final valida: " + item.Key);
            ReadPngSize(path, out int width, out int height);
            Require(width == item.Value.x && height == item.Value.y,
                item.Key + " tiene dimension " + width + "x" + height +
                " en lugar de " + item.Value.x + "x" + item.Value.y + ".");
        }
    }

    private static void ReadPngSize(string path, out int width, out int height)
    {
        byte[] header = new byte[24];
        using (FileStream stream = File.OpenRead(path))
        {
            if (stream.Read(header, 0, header.Length) != header.Length)
                throw new InvalidOperationException("PNG incompleto: " + path);
        }

        byte[] signature = { 137, 80, 78, 71, 13, 10, 26, 10 };
        for (int i = 0; i < signature.Length; i++)
            Require(header[i] == signature[i], "Firma PNG invalida: " + path);
        width = ReadBigEndianInt32(header, 16);
        height = ReadBigEndianInt32(header, 20);
    }

    private static int ReadBigEndianInt32(byte[] bytes, int offset)
    {
        return (bytes[offset] << 24) |
            (bytes[offset + 1] << 16) |
            (bytes[offset + 2] << 8) |
            bytes[offset + 3];
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
        Require(count == 1 && found != null,
            name + " debe existir exactamente una vez.");
        return found;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
#endif

#if UNITY_EDITOR
using System;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

public static class VerticalUiFinalSetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";

    [MenuItem("Tools/Quantum Forge/Vertical UI/Configure All Blocks 1-5")]
    public static void ConfigureAll()
    {
        VerticalUiBlock1Setup.ConfigureBlock1Base();
        VerticalUiBlock2Setup.ConfigureBlock2Navigation();
        VerticalUiBlock3Setup.ConfigureBlock3Generation();
        VerticalUiBlock4Setup.ConfigureBlock4GenerationTriangle();
        VerticalUiBlock5Setup.ConfigureBlock5Upgrades();
        AssetDatabase.SaveAssets();
        Debug.Log("[Vertical UI Final Setup] CONFIGURED | blocks 1-5 in canonical order");
    }

    public static void ConfigureAllAndValidateBatch()
    {
        ConfigureAll();
        string firstSceneText = File.ReadAllText(ScenePath);
        string firstSceneHash = HashText(firstSceneText);

        ConfigureAll();
        string secondSceneText = File.ReadAllText(ScenePath);
        string secondSceneHash = HashText(secondSceneText);
        if (!string.Equals(firstSceneHash, secondSceneHash,
            StringComparison.OrdinalIgnoreCase))
        {
            LogFirstSceneDifferences(firstSceneText, secondSceneText);
            throw new InvalidOperationException(
                "La segunda ejecucion modifico Main.unity; la herramienta final no es idempotente.");
        }

        RunValidations();
        QaMainSceneIntegrityValidation.ValidateMainSceneIntegrity();
        VerticalUiFinalValidation.Validate();
        Debug.Log("[Vertical UI Final Setup] IDEMPOTENCE PASS | " +
            "scene hash stable | setup executed twice | no duplicate hierarchy or listeners");
    }

    [MenuItem("Tools/Quantum Forge/Vertical UI/Validate All Blocks 1-5")]
    public static void RunValidations()
    {
        VerticalUiBlock1Validation.Validate();
        VerticalUiBlock2Validation.Validate();
        VerticalUiBlock3Validation.Validate();
        VerticalUiBlock4Validation.Validate();
        VerticalUiBlock5Validation.Validate();
        VerticalUiFinalValidation.Validate();
        F2ProgressionMigrationValidation.Validate();
        TriangleRedesignValidation.Validate();
        MobileQaFriendlyLayoutValidation.Validate();
        MobileButtonLegibilityValidation.Validate();
    }

    private static string HashText(string text)
    {
        using SHA256 sha = SHA256.Create();
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text ?? string.Empty);
        return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", string.Empty);
    }

    private static void LogFirstSceneDifferences(string first, string second)
    {
        string[] left = first.Replace("\r\n", "\n").Split('\n');
        string[] right = second.Replace("\r\n", "\n").Split('\n');
        int limit = Math.Min(left.Length, right.Length);
        int logged = 0;
        for (int i = 0; i < limit && logged < 24; i++)
        {
            if (string.Equals(left[i], right[i], StringComparison.Ordinal))
                continue;
            Debug.LogError("[Vertical UI Final Setup] Scene diff line " + (i + 1) +
                " | first: " + left[i] + " | second: " + right[i]);
            logged++;
        }
        Debug.LogError("[Vertical UI Final Setup] Scene diff summary | first lines=" +
            left.Length + " | second lines=" + right.Length + " | logged=" + logged);
    }
}
#endif

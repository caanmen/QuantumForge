#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class QaAndroidDevelopmentBuild
{
    private const string OutputPath =
        "Builds/Android/QuantumForge-QA-Development.apk";

    [MenuItem("Tools/Quantum Forge/QA/Build Android Development")]
    public static void BuildAndroidDevelopment()
    {
        UIOrientation previousOrientation = PlayerSettings.defaultInterfaceOrientation;
        bool previousPortrait = PlayerSettings.allowedAutorotateToPortrait;
        bool previousPortraitUpsideDown =
            PlayerSettings.allowedAutorotateToPortraitUpsideDown;
        bool previousLandscapeLeft = PlayerSettings.allowedAutorotateToLandscapeLeft;
        bool previousLandscapeRight = PlayerSettings.allowedAutorotateToLandscapeRight;

        try
        {
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            BuildAndroidDevelopmentInternal();
        }
        finally
        {
            PlayerSettings.defaultInterfaceOrientation = previousOrientation;
            PlayerSettings.allowedAutorotateToPortrait = previousPortrait;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown =
                previousPortraitUpsideDown;
            PlayerSettings.allowedAutorotateToLandscapeLeft = previousLandscapeLeft;
            PlayerSettings.allowedAutorotateToLandscapeRight = previousLandscapeRight;
        }
    }

    private static void BuildAndroidDevelopmentInternal()
    {
        string[] scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();
        if (scenes.Length == 0 ||
            scenes[0] != "Assets/Project/Scenes/Main.unity")
            throw new InvalidOperationException(
                "Main.unity debe ser la primera escena habilitada.");

        string outputDirectory = Path.GetDirectoryName(OutputPath);
        if (!string.IsNullOrEmpty(outputDirectory))
            Directory.CreateDirectory(outputDirectory);

        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android &&
            !EditorUserBuildSettings.SwitchActiveBuildTarget(
                BuildTargetGroup.Android, BuildTarget.Android))
            throw new InvalidOperationException(
                "No se pudo activar el target Android.");

        BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = OutputPath,
            target = BuildTarget.Android,
            options = BuildOptions.Development | BuildOptions.AllowDebugging
        });

        BuildSummary summary = report.summary;
        if (summary.result != BuildResult.Succeeded)
            throw new InvalidOperationException(
                "Android Development Build fallo: " + summary.result +
                " | errores=" + summary.totalErrors);

        Debug.Log("[QA Android Development Build] PASS | " + OutputPath +
            " | bytes=" + summary.totalSize +
            " | warnings=" + summary.totalWarnings +
            " | errores=" + summary.totalErrors);
    }

    public static void BuildAndroidDevelopmentBatch()
    {
        try
        {
            BuildAndroidDevelopment();
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }
}
#endif

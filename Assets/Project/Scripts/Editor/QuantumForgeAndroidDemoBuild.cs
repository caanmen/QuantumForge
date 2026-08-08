#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class QuantumForgeAndroidDemoBuild
{
    private const string OutputPath =
        "Builds/Android/QuantumForge-QA-0.1.1-MixesFix-Universal.apk";
    private const string AndroidIdentifier = "com.nedfla.quantumforge";
    private const string QaBundleVersion = "0.1.1-qa";
    private const int QaVersionCode = 2;

    [MenuItem("Tools/Quantum Forge/Build/Build Android Demo APK")]
    public static void BuildAndroidDemo()
    {
        UIOrientation previousOrientation = PlayerSettings.defaultInterfaceOrientation;
        bool previousPortrait = PlayerSettings.allowedAutorotateToPortrait;
        bool previousPortraitUpsideDown =
            PlayerSettings.allowedAutorotateToPortraitUpsideDown;
        bool previousLandscapeLeft = PlayerSettings.allowedAutorotateToLandscapeLeft;
        bool previousLandscapeRight = PlayerSettings.allowedAutorotateToLandscapeRight;
        bool previousBuildAppBundle = EditorUserBuildSettings.buildAppBundle;
        ScriptingImplementation previousScriptingBackend =
            PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android);
        AndroidArchitecture previousArchitectures =
            PlayerSettings.Android.targetArchitectures;
        Il2CppCompilerConfiguration previousIl2CppConfiguration =
            PlayerSettings.GetIl2CppCompilerConfiguration(BuildTargetGroup.Android);
        string previousIdentifier =
            PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
        string previousProductName = PlayerSettings.productName;
        string previousBundleVersion = PlayerSettings.bundleVersion;
        int previousVersionCode = PlayerSettings.Android.bundleVersionCode;

        try
        {
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android,
                AndroidIdentifier);
            PlayerSettings.productName = "Quantum Forge";
            PlayerSettings.bundleVersion = QaBundleVersion;
            PlayerSettings.Android.bundleVersionCode = QaVersionCode;
            EditorUserBuildSettings.buildAppBundle = false;
            // APK universal local: incluye ARMv7 y ARM64 sin retirar contenido.
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android,
                ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures =
                AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
            // Reduce el consumo de clang; todas las opciones se restauran al terminar.
            PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.Android,
                Il2CppCompilerConfiguration.Debug);
            BuildInternal();
        }
        finally
        {
            PlayerSettings.defaultInterfaceOrientation = previousOrientation;
            PlayerSettings.allowedAutorotateToPortrait = previousPortrait;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown =
                previousPortraitUpsideDown;
            PlayerSettings.allowedAutorotateToLandscapeLeft = previousLandscapeLeft;
            PlayerSettings.allowedAutorotateToLandscapeRight = previousLandscapeRight;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android,
                previousIdentifier);
            PlayerSettings.productName = previousProductName;
            PlayerSettings.bundleVersion = previousBundleVersion;
            PlayerSettings.Android.bundleVersionCode = previousVersionCode;
            EditorUserBuildSettings.buildAppBundle = previousBuildAppBundle;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android,
                previousScriptingBackend);
            PlayerSettings.Android.targetArchitectures = previousArchitectures;
            PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.Android,
                previousIl2CppConfiguration);
        }
    }

    private static void BuildInternal()
    {
        string[] scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();
        if (scenes.Length == 0 ||
            scenes[0] != "Assets/Project/Scenes/Main.unity")
        {
            throw new InvalidOperationException(
                "Main.unity debe ser la primera escena habilitada.");
        }

        string outputDirectory = Path.GetDirectoryName(OutputPath);
        if (!string.IsNullOrWhiteSpace(outputDirectory))
            Directory.CreateDirectory(outputDirectory);

        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android &&
            !EditorUserBuildSettings.SwitchActiveBuildTarget(
                BuildTargetGroup.Android, BuildTarget.Android))
        {
            throw new InvalidOperationException(
                "No se pudo activar el target Android.");
        }

        BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = OutputPath,
            target = BuildTarget.Android,
            options = BuildOptions.None
        });

        BuildSummary summary = report.summary;
        if (summary.result != BuildResult.Succeeded)
        {
            throw new InvalidOperationException(
                "Android Demo APK fallo: " + summary.result +
                " | errores=" + summary.totalErrors);
        }

        Debug.Log("[Quantum Forge Android Demo] PASS | " + OutputPath +
            " | bytes=" + summary.totalSize +
            " | warnings=" + summary.totalWarnings +
            " | errores=" + summary.totalErrors);
    }

    public static void BuildAndroidDemoBatch()
    {
        try
        {
            BuildAndroidDemo();
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

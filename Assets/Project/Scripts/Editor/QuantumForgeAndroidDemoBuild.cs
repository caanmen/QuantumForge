#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class QuantumForgeAndroidDemoBuild
{
    private static readonly NamedBuildTarget AndroidBuildTarget = NamedBuildTarget.Android;
    private const string OutputPath =
        "Builds/Android/QuantumForge-QA-0.1.13-Repairs-ARM64.apk";
    private const string AndroidIdentifier = "com.nedfla.quantumforge";
    private const string QaBundleVersion = "0.1.13-qa-repairs";
    private const int QaVersionCode = 14;
    private const int PreviousHighestVersionCode = 13;
    private const string ExpectedCertificateSha256 =
        "4D704230994E826E0431BF360EC98BFCDF10AB816C985DA4EB2E0484260AE4B3";
    private const string SigningDirectoryName = "QuantumForge/Signing";
    private const string SigningConfigurationFileName = "android-signing.json";

    [Serializable]
    private sealed class SigningConfiguration
    {
        public string keystorePath;
        public string keyAlias;
        public string storePassword;
        public string keyPassword;
        public string expectedCertificateSha256;
    }

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
            PlayerSettings.GetScriptingBackend(AndroidBuildTarget);
        AndroidArchitecture previousArchitectures =
            PlayerSettings.Android.targetArchitectures;
        Il2CppCompilerConfiguration previousIl2CppConfiguration =
            PlayerSettings.GetIl2CppCompilerConfiguration(AndroidBuildTarget);
        string previousIdentifier =
            PlayerSettings.GetApplicationIdentifier(AndroidBuildTarget);
        string previousProductName = PlayerSettings.productName;
        string previousBundleVersion = PlayerSettings.bundleVersion;
        int previousVersionCode = PlayerSettings.Android.bundleVersionCode;
        bool previousUseCustomKeystore = PlayerSettings.Android.useCustomKeystore;
        string previousKeystoreName = PlayerSettings.Android.keystoreName;
        string previousKeyaliasName = PlayerSettings.Android.keyaliasName;
        string previousKeystorePass = PlayerSettings.Android.keystorePass;
        string previousKeyaliasPass = PlayerSettings.Android.keyaliasPass;

        try
        {
            if (QaVersionCode <= PreviousHighestVersionCode)
                throw new InvalidOperationException(
                    "El versionCode debe superar la ultima APK distribuida.");

            SaveRecoveryValidation.ValidateOrThrow();
            SigningConfiguration signing = LoadAndValidateSigningConfiguration();

            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
            PlayerSettings.SetApplicationIdentifier(AndroidBuildTarget,
                AndroidIdentifier);
            PlayerSettings.productName = "Quantum Forge";
            PlayerSettings.bundleVersion = QaBundleVersion;
            PlayerSettings.Android.bundleVersionCode = QaVersionCode;
            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = signing.keystorePath;
            PlayerSettings.Android.keyaliasName = signing.keyAlias;
            PlayerSettings.Android.keystorePass = signing.storePassword;
            PlayerSettings.Android.keyaliasPass = signing.keyPassword;
            EditorUserBuildSettings.buildAppBundle = false;
            // Una sola APK ARM64 sirve como actualizacion y como instalacion
            // nueva en telefonos Android modernos de 64 bits.
            PlayerSettings.SetScriptingBackend(AndroidBuildTarget,
                ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures =
                AndroidArchitecture.ARM64;
            // Reduce el consumo de clang; todas las opciones se restauran al terminar.
            PlayerSettings.SetIl2CppCompilerConfiguration(AndroidBuildTarget,
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
            PlayerSettings.SetApplicationIdentifier(AndroidBuildTarget,
                previousIdentifier);
            PlayerSettings.productName = previousProductName;
            PlayerSettings.bundleVersion = previousBundleVersion;
            PlayerSettings.Android.bundleVersionCode = previousVersionCode;
            PlayerSettings.Android.useCustomKeystore = previousUseCustomKeystore;
            PlayerSettings.Android.keystoreName = previousKeystoreName;
            PlayerSettings.Android.keyaliasName = previousKeyaliasName;
            PlayerSettings.Android.keystorePass = previousKeystorePass;
            PlayerSettings.Android.keyaliasPass = previousKeyaliasPass;
            EditorUserBuildSettings.buildAppBundle = previousBuildAppBundle;
            PlayerSettings.SetScriptingBackend(AndroidBuildTarget,
                previousScriptingBackend);
            PlayerSettings.Android.targetArchitectures = previousArchitectures;
            PlayerSettings.SetIl2CppCompilerConfiguration(AndroidBuildTarget,
                previousIl2CppConfiguration);
        }
    }

    private static void BuildInternal()
    {
        if (PlayerSettings.GetApplicationIdentifier(AndroidBuildTarget) !=
            AndroidIdentifier)
            throw new InvalidOperationException(
                "El paquete Android no coincide con la aplicacion instalada.");
        if (PlayerSettings.Android.bundleVersionCode != QaVersionCode)
            throw new InvalidOperationException(
                "El versionCode Android no coincide con el autorizado.");
        if (!PlayerSettings.Android.useCustomKeystore)
            throw new InvalidOperationException(
                "La build QA requiere la keystore permanente autorizada.");

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
            // QaRuntimeService habilita las herramientas solo cuando
            // Debug.isDebugBuild es verdadero. AllowDebugging permite además
            // respaldar save.json con adb antes de iniciar la aplicación.
            // CleanBuildCache evita que Gradle/Unity reutilicen bloques obsoletos
            // que pueden inflar la APK despues de una recompilacion incremental.
            options = BuildOptions.Development |
                BuildOptions.AllowDebugging |
                BuildOptions.CleanBuildCache
        });

        BuildSummary summary = report.summary;
        if (summary.result != BuildResult.Succeeded)
        {
            throw new InvalidOperationException(
                "Android Demo APK fallo: " + summary.result +
                " | errores=" + summary.totalErrors);
        }

        VerifyBuiltApk(OutputPath);

        Debug.Log("[Quantum Forge Android Demo] PASS | " + OutputPath +
            " | bytes=" + summary.totalSize +
            " | warnings=" + summary.totalWarnings +
            " | errores=" + summary.totalErrors);
    }

    private static SigningConfiguration LoadAndValidateSigningConfiguration()
    {
        string localData = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData);
        string configurationPath = Path.Combine(
            localData, SigningDirectoryName, SigningConfigurationFileName);
        if (!File.Exists(configurationPath))
            throw new FileNotFoundException(
                "Falta la configuracion local de firma Android.",
                configurationPath);

        SigningConfiguration configuration = JsonUtility.FromJson<SigningConfiguration>(
            File.ReadAllText(configurationPath));
        if (configuration == null ||
            string.IsNullOrWhiteSpace(configuration.keystorePath) ||
            string.IsNullOrWhiteSpace(configuration.keyAlias) ||
            string.IsNullOrWhiteSpace(configuration.storePassword) ||
            string.IsNullOrWhiteSpace(configuration.keyPassword))
        {
            throw new InvalidDataException(
                "La configuracion local de firma esta incompleta.");
        }

        string keystorePath = Path.GetFullPath(configuration.keystorePath);
        if (!File.Exists(keystorePath))
            throw new FileNotFoundException(
                "No se encontro la keystore QA permanente.", keystorePath);
        configuration.keystorePath = keystorePath;

        string configuredFingerprint = NormalizeFingerprint(
            configuration.expectedCertificateSha256);
        if (configuredFingerprint != ExpectedCertificateSha256)
            throw new InvalidOperationException(
                "La huella autorizada de la configuracion local fue modificada.");

        string actualFingerprint = ReadKeystoreFingerprint(configuration);
        if (actualFingerprint != ExpectedCertificateSha256)
            throw new InvalidOperationException(
                "La firma Android no coincide con la APK distribuida 0.1.3.");

        return configuration;
    }

    private static string ReadKeystoreFingerprint(
        SigningConfiguration configuration)
    {
        string keytool = Path.Combine(
            EditorApplication.applicationContentsPath,
            "PlaybackEngines", "AndroidPlayer", "OpenJDK", "bin",
            Application.platform == RuntimePlatform.WindowsEditor
                ? "keytool.exe" : "keytool");
        string output = RunProcess(keytool,
            "-list -v -keystore " + Quote(configuration.keystorePath) +
            " -storepass " + Quote(configuration.storePassword) +
            " -alias " + Quote(configuration.keyAlias) +
            " -keypass " + Quote(configuration.keyPassword));
        Match match = Regex.Match(output,
            @"SHA256:\s*([0-9A-Fa-f:]{64,95})",
            RegexOptions.CultureInvariant);
        if (!match.Success)
            throw new InvalidOperationException(
                "No se pudo leer la huella SHA-256 de la keystore.");
        return NormalizeFingerprint(match.Groups[1].Value);
    }

    private static void VerifyBuiltApk(string apkPath)
    {
        if (!File.Exists(apkPath))
            throw new FileNotFoundException("No se genero la APK esperada.", apkPath);

        string buildTools = Path.Combine(
            EditorApplication.applicationContentsPath,
            "PlaybackEngines", "AndroidPlayer", "SDK", "build-tools");
        string latestTools = Directory.GetDirectories(buildTools)
            .OrderByDescending(path => path, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();
        if (string.IsNullOrEmpty(latestTools))
            throw new DirectoryNotFoundException(
                "No se encontraron las herramientas Android de verificacion.");

        string suffix = Application.platform == RuntimePlatform.WindowsEditor
            ? ".bat" : string.Empty;
        string apksigner = Path.Combine(latestTools, "apksigner" + suffix);
        string aapt = Path.Combine(latestTools,
            Application.platform == RuntimePlatform.WindowsEditor
                ? "aapt.exe" : "aapt");

        string signature = RunProcess(apksigner,
            "verify --print-certs " + Quote(Path.GetFullPath(apkPath)));
        Match signatureMatch = Regex.Match(signature,
            @"certificate SHA-256 digest:\s*([0-9A-Fa-f]+)",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (!signatureMatch.Success ||
            NormalizeFingerprint(signatureMatch.Groups[1].Value) !=
                ExpectedCertificateSha256)
        {
            throw new InvalidOperationException(
                "La APK generada no conserva la firma autorizada.");
        }

        string badging = RunProcess(aapt,
            "dump badging " + Quote(Path.GetFullPath(apkPath)));
        string packagePattern = @"package: name='" +
            Regex.Escape(AndroidIdentifier) + @"' versionCode='" +
            QaVersionCode + @"' versionName='" +
            Regex.Escape(QaBundleVersion) + @"'";
        if (!Regex.IsMatch(badging, packagePattern,
                RegexOptions.CultureInvariant))
        {
            throw new InvalidOperationException(
                "La APK generada no conserva paquete, versionCode o versionName.");
        }
        if (!Regex.IsMatch(badging,
                @"native-code:.*'arm64-v8a'",
                RegexOptions.CultureInvariant))
        {
            throw new InvalidOperationException(
                "La APK generada no contiene la arquitectura ARM64 requerida.");
        }
    }

    private static string RunProcess(string executable, string arguments)
    {
        if (!File.Exists(executable))
            throw new FileNotFoundException(
                "No se encontro una herramienta requerida.", executable);

        string processExecutable = executable;
        string processArguments = arguments;
        string extension = Path.GetExtension(executable);
        if (Application.platform == RuntimePlatform.WindowsEditor &&
            (string.Equals(extension, ".bat", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(extension, ".cmd", StringComparison.OrdinalIgnoreCase)))
        {
            processExecutable = Environment.GetEnvironmentVariable("ComSpec");
            if (string.IsNullOrWhiteSpace(processExecutable))
                processExecutable = Path.Combine(
                    Environment.SystemDirectory, "cmd.exe");
            processArguments = "/d /s /c \"\"" + executable + "\" " +
                arguments + "\"";
        }

        var output = new StringBuilder();
        using (var process = new Process())
        {
            process.StartInfo = new ProcessStartInfo
            {
                FileName = processExecutable,
                Arguments = processArguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            process.OutputDataReceived += (_, eventArgs) =>
            {
                if (eventArgs.Data != null) output.AppendLine(eventArgs.Data);
            };
            process.ErrorDataReceived += (_, eventArgs) =>
            {
                if (eventArgs.Data != null) output.AppendLine(eventArgs.Data);
            };
            if (!process.Start())
                throw new InvalidOperationException(
                    "No se pudo iniciar " + executable);
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            if (process.ExitCode != 0)
                throw new InvalidOperationException(
                    Path.GetFileName(executable) + " fallo (" +
                    process.ExitCode + "). " + output);
        }
        return output.ToString();
    }

    private static string Quote(string value)
    {
        return "\"" + (value ?? string.Empty).Replace("\"", "\\\"") + "\"";
    }

    private static string NormalizeFingerprint(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        var normalized = new StringBuilder(value.Length);
        foreach (char character in value)
            if (Uri.IsHexDigit(character)) normalized.Append(character);
        return normalized.ToString().ToUpperInvariant();
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

    public static void ValidateBuildConfigurationBatch()
    {
        try
        {
            if (OutputPath !=
                "Builds/Android/QuantumForge-QA-0.1.13-Repairs-ARM64.apk")
                throw new InvalidOperationException("La ruta de salida QA no coincide.");
            if (QaBundleVersion != "0.1.13-qa-repairs")
                throw new InvalidOperationException("La versión QA no coincide.");
            if (QaVersionCode != 14 || QaVersionCode <= PreviousHighestVersionCode)
                throw new InvalidOperationException("El versionCode QA no es incremental.");
            if (AndroidIdentifier != "com.nedfla.quantumforge")
                throw new InvalidOperationException("El identificador Android no coincide.");

            string[] scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();
            if (scenes.Length == 0 || scenes[0] != "Assets/Project/Scenes/Main.unity")
                throw new InvalidOperationException("Main.unity no es la primera escena.");

            SaveRecoveryValidation.ValidateOrThrow();
            LoadAndValidateSigningConfiguration();

            Debug.Log("[Quantum Forge Android Config] PASS | " +
                QaBundleVersion + " | versionCode=" + QaVersionCode +
                " | package=" + AndroidIdentifier + " | ARM64 | firma autorizada");
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

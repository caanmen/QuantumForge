#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class QaBlock5Validation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const BindingFlags PrivateStatic =
        BindingFlags.NonPublic | BindingFlags.Static;

    [MenuItem("Tools/Quantum Forge/QA/Validate Block 5")]
    public static void ValidateBlock5()
    {
        var failures = new List<string>();
        string testRoot = Path.Combine("Library", "QaBlock5Validation");
        try
        {
            if (Directory.Exists(testRoot))
                Directory.Delete(testRoot, true);
            Directory.CreateDirectory(testRoot);

            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            ValidateSceneContract(failures);
            ValidateSourceContract(failures);
            ValidateCheckpointRoundTrip(testRoot, failures);
            ValidateCorruptCheckpoint(testRoot, failures);
            ValidateLegacyCheckpoint(testRoot, failures);
        }
        finally
        {
            SaveService.FailureInjectionPoint = SaveFailureInjectionPoint.None;
            if (Directory.Exists(testRoot))
                Directory.Delete(testRoot, true);
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        }

        Finish(failures);
    }

    private static void ValidateSceneContract(List<string> failures)
    {
        QaCheckpointService[] services =
            UnityEngine.Object.FindObjectsByType<QaCheckpointService>(
                FindObjectsInactive.Include, FindObjectsSortMode.None);
        Check(services.Length == 1,
            "QaCheckpointService está ausente o duplicado.", failures);
        if (services.Length == 1)
        {
            Check(services[0].panel != null &&
                services[0].panel.checkpointSaveButtons.Length == 3 &&
                services[0].panel.checkpointLoadButtons.Length == 3,
                "El servicio no está conectado a los tres slots del panel.",
                failures);
        }
    }

    private static void ValidateSourceContract(List<string> failures)
    {
        string source = File.ReadAllText(
            "Assets/Project/Scripts/QA/QaCheckpointService.cs");
        string saveSource = File.ReadAllText(
            "Assets/Project/Scripts/Systems/SaveService.cs");
        string panelSource = File.ReadAllText(
            "Assets/Project/Scripts/QA/QaPanelUI.cs");

        Check(source.Contains("qa_checkpoints") &&
            source.Contains("checkpoint_A") == false &&
            source.Contains("checkpoint_\" + slot") &&
            source.Contains("checkpoint_pre_restore.json"),
            "Las rutas QA o los slots no siguen el contrato.", failures);
        Check(source.Contains("SaveService.I.TrySave") &&
            source.Contains("SaveService.TryReadSaveData") &&
            source.Contains("SaveService.TryWriteAtomicJson"),
            "El servicio no reutiliza guardado, validación y escritura atómica.",
            failures);
        Check(source.Contains("data.lastUnix = nowUnix") &&
            source.Contains("SceneManager.LoadScene(\"Main\")"),
            "La restauración no neutraliza offline o no recarga Main.", failures);
        Check(source.Contains("QaRuntimeService.IsAvailable") &&
            panelSource.Contains("safeAreaRoot.gameObject.SetActive(false)") &&
            !source.Contains("Resources/") &&
            !source.Contains("StreamingAssets"),
            "Los checkpoints podrían exponerse o incluirse en una build pública.",
            failures);
        Check(saveSource.Contains("public string CurrentSavePath => SavePath") &&
            !saveSource.Contains("QaCheckpointMetadata"),
            "Los metadatos QA contaminaron SaveData o falta acceso controlado.",
            failures);
        Check(source.Contains("createdUnix") &&
            source.Contains("dimension01Unlocked") &&
            source.Contains("dimension02Unlocked") &&
            source.Contains("dimension03Unlocked") &&
            source.Contains("convergencePhase") &&
            source.Contains("qaSimulationMultiplier"),
            "Faltan metadatos QA requeridos.", failures);
    }

    private static void ValidateCheckpointRoundTrip(
        string root, List<string> failures)
    {
        string main = Path.Combine(root, "save.json");
        string slot = Path.Combine(root, "checkpoint_A.json");
        string pre = Path.Combine(root, "checkpoint_pre_restore.json");
        WriteSave(main, new SaveData
        {
            LE = 123.5,
            baseLEps = 7.0,
            dimensionDiscoverySaveVersion = 1,
            lastUnix = 10
        }, failures);

        var metadata = new QaCheckpointMetadata
        {
            createdUnix = 20,
            dimension01Unlocked = true,
            convergencePhase = ConvergencePhase.Ready.ToString(),
            qaSimulationMultiplier = 10f
        };
        bool created = InvokeBool(
            "TryCreateCheckpointFromSave",
            new object[] { main, slot, metadata, null }, out string createError);
        Check(created && string.IsNullOrEmpty(createError) &&
            File.Exists(slot) && File.Exists(slot + ".meta.json"),
            "Guardar A no creó save y metadatos atómicos: " + createError,
            failures);

        WriteSave(main, new SaveData
        {
            LE = 999.0,
            baseLEps = 20.0,
            lastUnix = 30
        }, failures);
        bool restored = InvokeBool(
            "TryRestoreCheckpointFiles",
            new object[] { slot, pre, main, 424242L, null },
            out string restoreError);
        SaveService.TryReadSaveData(main, out SaveData loaded);
        SaveService.TryReadSaveData(pre, out SaveData preRestore);
        Check(restored && string.IsNullOrEmpty(restoreError) &&
            loaded != null && Math.Abs(loaded.LE - 123.5) < 0.000001 &&
            Math.Abs(loaded.baseLEps - 7.0) < 0.000001,
            "Cargar A no recuperó el estado real guardado: " + restoreError,
            failures);
        Check(loaded != null && loaded.lastUnix == 424242L,
            "Cargar A no actualizó lastUnix al reloj real.", failures);
        Check(preRestore != null && Math.Abs(preRestore.LE - 999.0) < 0.000001,
            "pre_restore no conserva el estado anterior.", failures);
    }

    private static void ValidateCorruptCheckpoint(
        string root, List<string> failures)
    {
        string main = Path.Combine(root, "corrupt_main.json");
        string slot = Path.Combine(root, "corrupt_slot.json");
        string pre = Path.Combine(root, "corrupt_pre.json");
        WriteSave(main, new SaveData { LE = 777.0, lastUnix = 50 }, failures);
        File.WriteAllText(slot, "{ JSON CORRUPTO");
        string before = File.ReadAllText(main);
        bool restored = InvokeBool(
            "TryRestoreCheckpointFiles",
            new object[] { slot, pre, main, 60L, null }, out _);
        string after = File.ReadAllText(main);
        SaveService.TryReadSaveData(pre, out SaveData preRestore);
        Check(!restored && before == after && preRestore != null &&
            Math.Abs(preRestore.LE - 777.0) < 0.000001,
            "Un JSON corrupto reemplazó el save o perdió pre_restore.", failures);
    }

    private static void ValidateLegacyCheckpoint(
        string root, List<string> failures)
    {
        string main = Path.Combine(root, "legacy_main.json");
        string legacy = Path.Combine(root, "legacy_slot.json");
        File.WriteAllText(legacy,
            "{\"LE\":321.0,\"EM\":9.0,\"ADP\":8.0,\"WHF\":7.0," +
            "\"lastUnix\":1}");
        WriteSave(main, new SaveData { LE = 1.0, lastUnix = 2 }, failures);
        bool restored = InvokeBool(
            "TryReplaceMainSave",
            new object[] { legacy, main, 987654L, null }, out string error);
        SaveService.TryReadSaveData(main, out SaveData loaded);
        Check(restored && loaded != null &&
            Math.Abs(loaded.LE - 321.0) < 0.000001 &&
            loaded.lastUnix == 987654L,
            "Un checkpoint antiguo no fue aceptado para la migración normal: " +
            error, failures);
    }

    private static bool InvokeBool(
        string methodName, object[] arguments, out string error)
    {
        error = null;
        MethodInfo method = typeof(QaCheckpointService).GetMethod(
            methodName, PrivateStatic);
        if (method == null)
        {
            error = "Método no encontrado: " + methodName;
            return false;
        }

        bool result = (bool)method.Invoke(null, arguments);
        error = arguments[arguments.Length - 1] as string;
        return result;
    }

    private static void WriteSave(
        string path, SaveData data, List<string> failures)
    {
        bool written = SaveService.TryWriteAtomicJson(
            path, JsonUtility.ToJson(data, true), out string error);
        Check(written, "No se pudo preparar fixture: " + error, failures);
    }

    private static void Finish(List<string> failures)
    {
        if (failures.Count == 0)
        {
            Debug.Log("[QA Block 5] PASS | A/B/C reales | restauración exacta | " +
                "lastUnix actual | JSON corrupto seguro | pre_restore | " +
                "migración normal | oculto en build pública");
            return;
        }

        Debug.LogError("[QA Block 5] FAIL\n- " + string.Join("\n- ", failures));
        throw new InvalidOperationException(
            "El Bloque 5 no superó su validación.");
    }

    private static void Check(
        bool condition, string failure, List<string> failures)
    {
        if (!condition)
            failures.Add(failure);
    }
}
#endif

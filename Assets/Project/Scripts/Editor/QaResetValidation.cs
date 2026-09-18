#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class QaResetValidation
{
    [MenuItem("Tools/Quantum Forge/QA/Validate Reset And Retired Cube Quality")]
    public static void ValidateBatch()
    {
        var failures = new List<string>();
        ValidateResetPersistence(failures);
        ValidateCheckpointResetRestoreLifecycle(failures);
        ValidateQaIntegrationContract(failures);
        ValidateRetiredCubeQuality(failures);
        ValidateMonolithTransitionContract(failures);

        if (failures.Count > 0)
        {
            Debug.LogError("[QA Reset] FAIL\n- " + string.Join("\n- ", failures));
            throw new InvalidOperationException(
                "El reinicio QA no superó su validación.");
        }

        Debug.Log("[QA Reset] PASS | confirmación desacoplada | partida nueva " +
            "canónica | checkpoint intacto tras reset | activación en memoria | " +
            "autoguardado + carga fría | calidad del cubo ausente | " +
            "transición 2D optimizada preservada");
    }

    private static void ValidateCheckpointResetRestoreLifecycle(
        List<string> failures)
    {
        if (GameState.I != null || SaveService.I != null || MachineManager.I != null)
        {
            failures.Add("La prueba checkpoint/reset requiere singletons aislados.");
            return;
        }

        string previousDirectory = SaveService.EditorValidationSaveDirectory;
        List<string> previousResearch = SaveService.LastLoadedResearchIds;
        List<string> previousAchievements = SaveService.LastLoadedAchievementIds;
        List<SavedBuildingLevel> previousBuildings =
            SaveService.LastLoadedBuildingLevels;
        GameObject root = null;
        string directory = null;

        try
        {
            directory = Path.GetFullPath(Path.Combine(
                Application.dataPath,
                "../Logs/ReleaseD1D3/QaCheckpointResetRestore",
                Guid.NewGuid().ToString("N")));
            Directory.CreateDirectory(directory);
            SaveService.EditorValidationSaveDirectory = directory;

            root = CreateSaveFixture(
                "QA checkpoint reset/restore fixture",
                out GameState state, out SaveService save);
            state.EnsureDimension1State();
            state.EnsureDimension2State();
            state.EnsureDimension3State();
            state.EnsureConvergenceState();
            state.LE = 24680.0;
            state.Traces = 1357.0;
            state.dimension01Unlocked = true;
            state.prestige1Count = 2;
            state.hasDonePrestige1 = true;

            Check(save.TrySave(out string initialError),
                "No se pudo preparar el progreso del checkpoint: " +
                initialError, failures);

            string checkpointDirectory = Path.Combine(
                directory, "qa_checkpoints");
            Directory.CreateDirectory(checkpointDirectory);
            string checkpointPath = Path.Combine(
                checkpointDirectory, "checkpoint_A.json");
            string preRestorePath = Path.Combine(
                checkpointDirectory, "checkpoint_pre_restore.json");
            var metadata = new QaCheckpointMetadata
            {
                createdUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                dimension01Unlocked = true,
                convergencePhase = ConvergencePhase.Inactive.ToString(),
                qaSimulationMultiplier = 1f
            };
            bool checkpointCreated = InvokeCheckpointBool(
                "TryCreateCheckpointFromSave",
                new object[]
                {
                    save.CurrentSavePath, checkpointPath, metadata, null
                }, out string checkpointError);
            Check(checkpointCreated,
                "No se pudo crear A mediante el servicio real: " +
                checkpointError, failures);
            if (!checkpointCreated)
                return;

            string checkpointBytes = Convert.ToBase64String(
                File.ReadAllBytes(checkpointPath));
            string metadataBytes = Convert.ToBase64String(
                File.ReadAllBytes(checkpointPath + ".meta.json"));

            Check(save.TryResetToNewGame(out string resetError),
                "El reset previo a restaurar falló: " + resetError, failures);
            Check(Math.Abs(state.LE - 10.0) < 0.000001 &&
                Math.Abs(state.Traces) < 0.000001,
                "La preparación no alcanzó el estado reset.", failures);
            Check(checkpointBytes == Convert.ToBase64String(
                    File.ReadAllBytes(checkpointPath)) &&
                metadataBytes == Convert.ToBase64String(
                    File.ReadAllBytes(checkpointPath + ".meta.json")),
                "El reset modificó A o su metadata.", failures);

            bool restored = InvokeCheckpointBool(
                "TryRestoreAndActivateCheckpointFiles",
                new object[]
                {
                    checkpointPath, preRestorePath, save.CurrentSavePath,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds(), save, null
                }, out string restoreError);
            Check(restored && string.IsNullOrEmpty(restoreError),
                "La restauración no activó A: " + restoreError, failures);
            Check(Math.Abs(state.LE - 24680.0) < 0.000001 &&
                Math.Abs(state.Traces - 1357.0) < 0.000001 &&
                state.dimension01Unlocked && state.prestige1Count == 2,
                "A quedó en disco, pero no reemplazó el GameState reset.",
                failures);

            Check(save.TrySave(out string autosaveError),
                "El autoguardado posterior falló: " + autosaveError, failures);
            Check(SaveService.TryReadSaveData(
                    save.CurrentSavePath, out SaveData afterAutosave) &&
                Math.Abs(afterAutosave.LE - 24680.0) < 0.000001 &&
                Math.Abs(afterAutosave.Traces - 1357.0) < 0.000001 &&
                afterAutosave.dimension01Unlocked &&
                afterAutosave.prestige1Count == 2,
                "El autoguardado volvió a imponer el reset.", failures);

            SetSingleton(typeof(SaveService), null);
            SetSingleton(typeof(MachineManager), null);
            SetSingleton(typeof(GameState), null);
            UnityEngine.Object.DestroyImmediate(root);
            root = CreateSaveFixture(
                "QA checkpoint cold reload fixture",
                out GameState reloadedState, out SaveService reloadedSave);
            reloadedSave.Load();
            Check(!reloadedSave.HasLoadFailure &&
                Math.Abs(reloadedState.LE - 24680.0) < 0.000001 &&
                Math.Abs(reloadedState.Traces - 1357.0) < 0.000001 &&
                reloadedState.dimension01Unlocked &&
                reloadedState.prestige1Count == 2,
                "Una instancia nueva no recuperó el checkpoint confirmado.",
                failures);
            Check(checkpointBytes == Convert.ToBase64String(
                    File.ReadAllBytes(checkpointPath)) &&
                metadataBytes == Convert.ToBase64String(
                    File.ReadAllBytes(checkpointPath + ".meta.json")),
                "La restauración o recarga alteró A o su metadata.", failures);
        }
        catch (Exception exception)
        {
            failures.Add("Excepción en checkpoint/reset/autosave: " +
                exception.Message);
        }
        finally
        {
            SetSingleton(typeof(SaveService), null);
            SetSingleton(typeof(MachineManager), null);
            SetSingleton(typeof(GameState), null);
            if (root != null)
                UnityEngine.Object.DestroyImmediate(root);
            SaveService.EditorValidationSaveDirectory = previousDirectory;
            SaveService.LastLoadedResearchIds = previousResearch;
            SaveService.LastLoadedAchievementIds = previousAchievements;
            SaveService.LastLoadedBuildingLevels = previousBuildings;
            if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
                Directory.Delete(directory, true);
        }
    }

    private static GameObject CreateSaveFixture(
        string name, out GameState state, out SaveService save)
    {
        GameObject root = new GameObject(name)
            { hideFlags = HideFlags.HideAndDontSave };
        root.SetActive(false);
        state = root.AddComponent<GameState>();
        MachineManager machine = root.AddComponent<MachineManager>();
        save = root.AddComponent<SaveService>();
        SetSingleton(typeof(GameState), state);
        SetSingleton(typeof(MachineManager), machine);
        SetSingleton(typeof(SaveService), save);
        return root;
    }

    private static bool InvokeCheckpointBool(
        string methodName, object[] arguments, out string error)
    {
        error = null;
        MethodInfo method = typeof(QaCheckpointService).GetMethod(
            methodName, BindingFlags.NonPublic | BindingFlags.Static);
        if (method == null)
        {
            error = "Método no encontrado: " + methodName;
            return false;
        }

        bool result = (bool)method.Invoke(null, arguments);
        error = arguments[arguments.Length - 1] as string;
        return result;
    }

    private static void ValidateResetPersistence(List<string> failures)
    {
        if (GameState.I != null || SaveService.I != null || MachineManager.I != null)
        {
            failures.Add("La prueba de persistencia requiere singletons aislados.");
            return;
        }

        string previousDirectory = SaveService.EditorValidationSaveDirectory;
        List<string> previousResearch = SaveService.LastLoadedResearchIds;
        List<string> previousAchievements = SaveService.LastLoadedAchievementIds;
        List<SavedBuildingLevel> previousBuildings =
            SaveService.LastLoadedBuildingLevels;
        GameObject root = null;
        string directory = null;

        try
        {
            directory = Path.GetFullPath(Path.Combine(
                Application.dataPath, "../Logs/ReleaseD1D3/QaReset",
                Guid.NewGuid().ToString("N")));
            Directory.CreateDirectory(directory);
            SaveService.EditorValidationSaveDirectory = directory;

            root = new GameObject("QA reset validation fixture")
                { hideFlags = HideFlags.HideAndDontSave };
            root.SetActive(false);
            GameState state = root.AddComponent<GameState>();
            MachineManager machine = root.AddComponent<MachineManager>();
            SaveService save = root.AddComponent<SaveService>();
            SetSingleton(typeof(GameState), state);
            SetSingleton(typeof(MachineManager), machine);
            SetSingleton(typeof(SaveService), save);

            state.EnsureDimension1State();
            state.EnsureDimension2State();
            state.EnsureDimension3State();
            state.EnsureConvergenceState();
            state.LE = 987654.0;
            state.Traces = 54321.0;
            state.dimension01Unlocked = true;
            state.dimension02Unlocked = true;
            state.dimension03Unlocked = true;
            state.prestige1Count = 4;
            state.hasDonePrestige1 = true;
            state.experimentalHallazgos = 17;

            Check(save.TrySave(out string initialError),
                "No se pudo preparar el save contaminado: " + initialError,
                failures);
            string savePath = save.CurrentSavePath;
            if (!File.Exists(savePath))
                return;

            File.Copy(savePath, savePath + ".bak", true);
            for (int index = 1; index <= SaveService.HistoricalBackupCount; index++)
            {
                File.Copy(savePath,
                    SaveService.GetHistoricalSavePath(savePath, index), true);
            }
            File.WriteAllText(savePath + ".tmp", "stale reset candidate");

            Check(save.TryResetToNewGame(out string resetError),
                "TryResetToNewGame falló: " + resetError, failures);
            Check(Math.Abs(state.LE - 10.0) < 0.000001 &&
                Math.Abs(state.Traces) < 0.000001 &&
                !state.dimension01Unlocked && !state.dimension02Unlocked &&
                !state.dimension03Unlocked && state.prestige1Count == 0 &&
                !state.hasDonePrestige1 && state.experimentalHallazgos == 0,
                "El estado en memoria no coincide con una partida nueva.", failures);
            Check(SaveService.TryReadSaveData(savePath, out SaveData data) &&
                data != null && Math.Abs(data.LE - 10.0) < 0.000001 &&
                Math.Abs(data.Traces) < 0.000001 &&
                !data.dimension01Unlocked && !data.dimension02Unlocked &&
                !data.dimension03Unlocked && data.prestige1Count == 0,
                "El save nuevo no es legible o conserva progreso anterior.", failures);
            Check(!File.Exists(savePath + ".bak") &&
                !File.Exists(savePath + ".tmp"),
                "Quedó un backup o temporal del save anterior.", failures);
            for (int index = 1; index <= SaveService.HistoricalBackupCount; index++)
            {
                Check(!File.Exists(SaveService.GetHistoricalSavePath(savePath, index)),
                    "Quedó el histórico " + index + " del save anterior.", failures);
            }

            state.LE = 444.0;
            Check(save.TryResetToNewGame(out string repeatedError) &&
                Math.Abs(state.LE - 10.0) < 0.000001,
                "El reinicio no es repetible: " + repeatedError, failures);
        }
        catch (Exception exception)
        {
            failures.Add("Excepción en la prueba aislada de reset: " +
                exception.Message);
        }
        finally
        {
            SetSingleton(typeof(SaveService), null);
            SetSingleton(typeof(MachineManager), null);
            SetSingleton(typeof(GameState), null);
            if (root != null)
                UnityEngine.Object.DestroyImmediate(root);
            SaveService.EditorValidationSaveDirectory = previousDirectory;
            SaveService.LastLoadedResearchIds = previousResearch;
            SaveService.LastLoadedAchievementIds = previousAchievements;
            SaveService.LastLoadedBuildingLevels = previousBuildings;
            if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
                Directory.Delete(directory, true);
        }
    }

    private static void ValidateQaIntegrationContract(List<string> failures)
    {
        string panel = File.ReadAllText(
            "Assets/Project/Scripts/QA/QaPanelUI.cs");
        string service = File.ReadAllText(
            "Assets/Project/Scripts/QA/QaCheckpointService.cs");
        Check(panel.Contains("ResetSaveRequested") &&
            panel.Contains("PendingConfirmation.ResetSave") &&
            panel.Contains("no se puede deshacer") &&
            panel.Contains("Los checkpoints QA A/B/C se conservarán") &&
            !panel.Contains("TryResetToNewGame"),
            "QaPanelUI no mantiene confirmación y persistencia desacopladas.",
            failures);
        Check(service.Contains("panel.ResetSaveRequested += OnResetSaveRequested") &&
            service.Contains("SaveService.I.TryResetToNewGame") &&
            service.Contains("TryRestoreAndActivateCheckpointFiles") &&
            service.Contains("ReloadMainScene()"),
            "QaCheckpointService no conecta reset, guardado y recarga.", failures);
    }

    private static void ValidateRetiredCubeQuality(List<string> failures)
    {
        string settings = File.ReadAllText(
            "Assets/Project/Scripts/UI/Vertical/VerticalSettingsPanelUI.cs");
        string scene = File.ReadAllText("Assets/Project/Scenes/Main.unity");
        Check(settings.IndexOf("cube", StringComparison.OrdinalIgnoreCase) < 0 &&
            settings.IndexOf("cubo", StringComparison.OrdinalIgnoreCase) < 0 &&
            settings.IndexOf("quality", StringComparison.OrdinalIgnoreCase) < 0 &&
            settings.IndexOf("calidad", StringComparison.OrdinalIgnoreCase) < 0,
            "Ajustes todavía contiene lógica de calidad del cubo.", failures);
        Check(scene.IndexOf("MachineCube3DQuality", StringComparison.OrdinalIgnoreCase) < 0 &&
            scene.IndexOf("calidad del cubo", StringComparison.OrdinalIgnoreCase) < 0 &&
            scene.IndexOf("cube quality", StringComparison.OrdinalIgnoreCase) < 0,
            "La escena todavía serializa la opción de calidad del cubo.", failures);
    }

    private static void ValidateMonolithTransitionContract(List<string> failures)
    {
        string source = File.ReadAllText(
            "Assets/Project/Scripts/UI/MachineMonolith2DVisualUI.cs");
        Check(source.Contains("SectorApproachDuration = .24f") &&
            source.Contains("SectorTransitionDuration = 1.12f") &&
            source.Contains("SectorApproachScale = 1.06f") &&
            source.Contains("Time.realtimeSinceStartupAsDouble") &&
            source.Contains("approachRect.anchoredPosition = approachStartPosition") &&
            source.Contains("artRect.anchoredPosition = overviewRestPosition") &&
            source.Contains("labRect.localScale = labStartScale") &&
            source.Contains("supportRect.localScale = supportStartScale"),
            "La transicion del Monolito perdio el acercamiento frontal con entorno estable.",
            failures);
    }

    private static void SetSingleton(Type type, object value)
    {
        type.GetProperty("I", BindingFlags.Public | BindingFlags.Static)?
            .SetValue(null, value);
    }

    private static void Check(
        bool condition, string failure, List<string> failures)
    {
        if (!condition)
            failures.Add(failure);
    }
}
#endif

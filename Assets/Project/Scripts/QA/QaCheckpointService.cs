using System;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public sealed class QaCheckpointMetadata
{
    public long createdUnix;
    public bool dimension01Unlocked;
    public bool dimension02Unlocked;
    public bool dimension03Unlocked;
    public string convergencePhase;
    public float qaSimulationMultiplier;
}

public class QaCheckpointService : MonoBehaviour
{
    private const string CheckpointDirectoryName = "qa_checkpoints";
    private const string PreRestoreName = "checkpoint_pre_restore.json";

    public QaPanelUI panel;

    public bool IsBusy { get; private set; }
    public string LastError { get; private set; }

    private string CheckpointDirectory => Path.Combine(
        Application.persistentDataPath, CheckpointDirectoryName);

    private void Awake()
    {
        ResolvePanel();
    }

    private void OnEnable()
    {
        ResolvePanel();
        Subscribe();
        RefreshSlotStatuses();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public bool TrySaveCheckpoint(char slot, out string error)
    {
        error = null;
        if (!CanBegin(slot, out error))
            return false;

        BeginOperation("GUARDANDO CHECKPOINT " + slot + "...");
        try
        {
            if (!SaveService.I.TrySave(out error))
                return Fail(error);

            string savePath = SaveService.I.CurrentSavePath;
            string slotPath = GetSlotPath(CheckpointDirectory, slot);
            QaCheckpointMetadata metadata = CaptureMetadata();
            if (!TryCreateCheckpointFromSave(
                savePath, slotPath, metadata, out error))
            {
                return Fail(error);
            }

            SetStatus("CHECKPOINT " + slot + " GUARDADO");
            return true;
        }
        finally
        {
            EndOperation();
        }
    }

    public bool TryRestoreCheckpoint(char slot, out string error)
    {
        error = null;
        if (!CanBegin(slot, out error))
            return false;

        BeginOperation("RESTAURANDO CHECKPOINT " + slot + "...");
        try
        {
            if (!SaveService.I.TrySave(out error))
                return Fail(error);

            string savePath = SaveService.I.CurrentSavePath;
            string slotPath = GetSlotPath(CheckpointDirectory, slot);
            string preRestorePath = Path.Combine(
                CheckpointDirectory, PreRestoreName);
            if (!TryRestoreAndActivateCheckpointFiles(
                slotPath, preRestorePath, savePath,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                SaveService.I, out error))
            {
                return Fail(error);
            }

            SetStatus("CHECKPOINT " + slot + " RESTAURADO");
            ReloadMainScene();
            return true;
        }
        finally
        {
            EndOperation();
        }
    }

    public bool TryRestorePreRestore(out string error)
    {
        error = null;
        if (!QaRuntimeService.IsAvailable)
        {
            error = "Los checkpoints QA no están disponibles en esta build.";
            return false;
        }
        if (IsBusy || SaveService.I == null)
        {
            error = IsBusy
                ? "Ya hay una operación QA en curso."
                : "SaveService no está disponible.";
            return false;
        }

        BeginOperation("RESTAURANDO PRE_RESTORE...");
        try
        {
            string preRestorePath = Path.Combine(
                CheckpointDirectory, PreRestoreName);
            if (!TryReplaceMainSave(
                preRestorePath, SaveService.I.CurrentSavePath,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds(), out error))
            {
                return Fail(error);
            }

            if (!TryLoadAndConfirmMainSave(
                SaveService.I, SaveService.I.CurrentSavePath,
                null, DateTimeOffset.UtcNow.ToUnixTimeSeconds(), out error))
            {
                return Fail(error);
            }

            SetStatus("PRE_RESTORE RESTAURADO");
            ReloadMainScene();
            return true;
        }
        finally
        {
            EndOperation();
        }
    }

    private bool CanBegin(char slot, out string error)
    {
        error = null;
        if (!QaRuntimeService.IsAvailable)
        {
            error = "Los checkpoints QA no están disponibles en esta build.";
            return false;
        }
        if (slot != 'A' && slot != 'B' && slot != 'C')
        {
            error = "Slot QA inválido.";
            return false;
        }
        if (IsBusy)
        {
            error = "Ya hay una operación QA en curso.";
            return false;
        }
        if (SaveService.I == null)
        {
            error = "SaveService no está disponible.";
            return false;
        }
        return true;
    }

    private void BeginOperation(string status)
    {
        IsBusy = true;
        LastError = null;
        SetControlsInteractable(false);
        SetStatus(status);
    }

    private void EndOperation()
    {
        IsBusy = false;
        SetControlsInteractable(true);
        RefreshSlotStatuses();
    }

    private bool Fail(string error)
    {
        LastError = string.IsNullOrEmpty(error)
            ? "Error desconocido de checkpoint."
            : error;
        SetStatus("ERROR: " + LastError);
        Debug.LogError("[QA Checkpoint] " + LastError);
        return false;
    }

    private static bool TryCreateCheckpointFromSave(
        string savePath, string checkpointPath,
        QaCheckpointMetadata metadata, out string error)
    {
        error = null;
        if (!SaveService.TryReadSaveData(savePath, out _))
        {
            error = "El save principal no es legible.";
            return false;
        }

        string saveJson;
        try
        {
            saveJson = File.ReadAllText(savePath);
        }
        catch (Exception exception)
        {
            error = exception.Message;
            return false;
        }

        if (!SaveService.TryWriteAtomicJson(
            checkpointPath, saveJson, out error))
        {
            return false;
        }

        string metadataJson = JsonUtility.ToJson(metadata, true);
        return TryWriteAtomicText(
            GetMetadataPath(checkpointPath), metadataJson, out error);
    }

    private static bool TryRestoreCheckpointFiles(
        string checkpointPath, string preRestorePath,
        string mainSavePath, long nowUnix, out string error)
    {
        error = null;
        if (!SaveService.TryReadSaveData(mainSavePath, out _))
        {
            error = "El save principal no es legible; no se reemplazó.";
            return false;
        }

        string currentJson;
        try
        {
            currentJson = File.ReadAllText(mainSavePath);
        }
        catch (Exception exception)
        {
            error = exception.Message;
            return false;
        }

        if (!SaveService.TryWriteAtomicJson(
            preRestorePath, currentJson, out error))
        {
            return false;
        }

        if (!SaveService.TryReadSaveData(checkpointPath, out _))
        {
            error = "El checkpoint seleccionado no es legible.";
            return false;
        }

        return TryReplaceMainSave(
            checkpointPath, mainSavePath, nowUnix, out error);
    }

    private static bool TryRestoreAndActivateCheckpointFiles(
        string checkpointPath, string preRestorePath,
        string mainSavePath, long nowUnix,
        SaveService saveService, out string error)
    {
        if (!TryRestoreCheckpointFiles(
            checkpointPath, preRestorePath, mainSavePath,
            nowUnix, out error))
        {
            return false;
        }

        return TryLoadAndConfirmMainSave(
            saveService, mainSavePath, preRestorePath,
            nowUnix, out error);
    }

    private static bool TryLoadAndConfirmMainSave(
        SaveService saveService, string mainSavePath,
        string rollbackPath, long nowUnix, out string error)
    {
        error = null;
        string activationError = null;
        try
        {
            if (saveService == null)
            {
                activationError = "SaveService no está disponible.";
            }
            else
            {
                // GameState sobrevive a LoadScene(Main). Sin esta carga explícita,
                // el estado anterior queda en memoria y el siguiente autoguardado
                // puede volver a escribirlo sobre el checkpoint restaurado.
                saveService.Load();
                if (saveService.HasLoadFailure)
                {
                    activationError =
                        "El checkpoint se escribió, pero no pudo activarse en memoria.";
                }
                else if (!saveService.TrySave(out activationError))
                {
                    activationError = "El checkpoint se cargó, pero no pudo " +
                        "confirmarse antes de recargar: " + activationError;
                }
                else
                {
                    return true;
                }
            }
        }
        catch (Exception exception)
        {
            activationError = "No se pudo activar el checkpoint: " +
                exception.Message;
        }

        string rollbackError = null;
        if (!string.IsNullOrEmpty(rollbackPath) &&
            TryReplaceMainSave(
                rollbackPath, mainSavePath, nowUnix, out rollbackError))
        {
            try
            {
                saveService?.Load();
                if (saveService != null && !saveService.HasLoadFailure)
                    saveService.TrySave(out _);
            }
            catch (Exception exception)
            {
                rollbackError = exception.Message;
            }

            error = activationError + " Se restauró el estado anterior.";
            return false;
        }

        error = activationError;
        if (!string.IsNullOrEmpty(rollbackPath))
        {
            error += " Tampoco se pudo recuperar pre_restore: " +
                (string.IsNullOrEmpty(rollbackError)
                    ? "error desconocido."
                    : rollbackError);
        }
        return false;
    }

    private static bool TryReplaceMainSave(
        string sourcePath, string mainSavePath,
        long nowUnix, out string error)
    {
        error = null;
        if (!SaveService.TryReadSaveData(sourcePath, out SaveData data))
        {
            error = "El checkpoint no es legible.";
            return false;
        }

        data.lastUnix = nowUnix;
        string restoredJson = JsonUtility.ToJson(data, true);
        return SaveService.TryWriteAtomicJson(
            mainSavePath, restoredJson, out error);
    }

    private static bool TryWriteAtomicText(
        string targetPath, string text, out string error)
    {
        error = null;
        string tempPath = targetPath + ".tmp";
        string backupPath = targetPath + ".bak";
        try
        {
            string directory = Path.GetDirectoryName(targetPath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);
            File.WriteAllText(tempPath, text);
            if (File.Exists(targetPath))
                File.Replace(tempPath, targetPath, backupPath, true);
            else
                File.Move(tempPath, targetPath);
            return true;
        }
        catch (Exception exception)
        {
            error = exception.Message;
            return false;
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }

    private static string GetSlotPath(string directory, char slot)
    {
        return Path.Combine(directory, "checkpoint_" + slot + ".json");
    }

    private static string GetMetadataPath(string checkpointPath)
    {
        return checkpointPath + ".meta.json";
    }

    public void RefreshSlotStatuses()
    {
        ResolvePanel();
        if (panel == null)
            return;

        for (int index = 0; index < 3; index++)
        {
            char slot = (char)('A' + index);
            string checkpointPath = GetSlotPath(CheckpointDirectory, slot);
            if (!File.Exists(checkpointPath))
            {
                panel.SetCheckpointSlotStatus(index, "VACÍO", false);
                continue;
            }

            if (!SaveService.TryReadSaveData(checkpointPath, out _))
            {
                panel.SetCheckpointSlotStatus(index, "NO LEGIBLE", false);
                continue;
            }

            panel.SetCheckpointSlotStatus(
                index, "GUARDADO\n" + GetCheckpointTimestamp(checkpointPath), true);
        }
    }

    private static string GetCheckpointTimestamp(string checkpointPath)
    {
        long createdUnix = 0L;
        string metadataPath = GetMetadataPath(checkpointPath);
        try
        {
            if (File.Exists(metadataPath))
            {
                QaCheckpointMetadata metadata = JsonUtility.FromJson<QaCheckpointMetadata>(
                    File.ReadAllText(metadataPath));
                if (metadata != null)
                    createdUnix = metadata.createdUnix;
            }

            DateTime localTime = createdUnix > 0L
                ? DateTimeOffset.FromUnixTimeSeconds(createdUnix).LocalDateTime
                : File.GetLastWriteTime(checkpointPath);
            return localTime.ToString("dd/MM HH:mm", CultureInfo.InvariantCulture);
        }
        catch (Exception)
        {
            return "FECHA NO DISPONIBLE";
        }
    }

    private static QaCheckpointMetadata CaptureMetadata()
    {
        GameState state = GameState.I;
        return new QaCheckpointMetadata
        {
            createdUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            dimension01Unlocked = state != null && state.dimension01Unlocked,
            dimension02Unlocked = state != null && state.dimension02Unlocked,
            dimension03Unlocked = state != null && state.dimension03Unlocked,
            convergencePhase = state != null && state.convergence != null
                ? state.convergence.phase.ToString()
                : ConvergencePhase.Inactive.ToString(),
            qaSimulationMultiplier = QaRuntimeService.SimulationMultiplier
        };
    }

    private void ReloadMainScene()
    {
        SceneManager.LoadScene("Main");
    }

    private void ResolvePanel()
    {
        if (panel == null)
        {
            panel = UnityEngine.Object.FindFirstObjectByType<QaPanelUI>(
                FindObjectsInactive.Include);
        }
    }

    private void Subscribe()
    {
        if (panel == null)
            return;
        panel.SaveCheckpointRequested -= OnSaveRequested;
        panel.SaveCheckpointRequested += OnSaveRequested;
        panel.LoadCheckpointRequested -= OnLoadRequested;
        panel.LoadCheckpointRequested += OnLoadRequested;
        panel.ResetSaveRequested -= OnResetSaveRequested;
        panel.ResetSaveRequested += OnResetSaveRequested;
    }

    private void Unsubscribe()
    {
        if (panel == null)
            return;
        panel.SaveCheckpointRequested -= OnSaveRequested;
        panel.LoadCheckpointRequested -= OnLoadRequested;
        panel.ResetSaveRequested -= OnResetSaveRequested;
    }

    private void OnSaveRequested(char slot)
    {
        TrySaveCheckpoint(slot, out _);
    }

    private void OnLoadRequested(char slot)
    {
        TryRestoreCheckpoint(slot, out _);
    }

    private void OnResetSaveRequested()
    {
        TryResetGame(out _);
    }

    public bool TryResetGame(out string error)
    {
        error = null;
        if (!QaRuntimeService.IsAvailable)
        {
            error = "El reinicio de partida sólo está disponible en QA.";
            return false;
        }
        if (IsBusy || SaveService.I == null)
        {
            error = IsBusy
                ? "Ya hay una operación QA en curso."
                : "SaveService no está disponible.";
            return false;
        }

        BeginOperation("REINICIANDO PARTIDA...");
        try
        {
            if (!SaveService.I.TryResetToNewGame(out error))
                return Fail(error);

            // El único PlayerPref histórico pertenece a la calidad del cubo 3D
            // retirado. No debe sobrevivir a una partida realmente nueva.
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            QaRuntimeService.ResetToNormalSpeed();
            TickSystem.I?.ResetAccumulator();
            SetStatus("PARTIDA REINICIADA");
            ReloadMainScene();
            return true;
        }
        finally
        {
            EndOperation();
        }
    }

    private void SetStatus(string status)
    {
        ResolvePanel();
        if (panel != null)
            panel.SetOperationStatus(status);
    }

    private void SetControlsInteractable(bool interactable)
    {
        ResolvePanel();
        if (panel != null)
            panel.SetQaControlsInteractable(interactable);
    }
}

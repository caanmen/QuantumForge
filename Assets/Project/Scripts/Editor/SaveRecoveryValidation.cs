#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class SaveRecoveryValidation
{
    [MenuItem("Tools/Quantum Forge/QA/Validate Save Recovery")]
    public static void Validate()
    {
        ValidateOrThrow();
        Debug.Log("[Save Recovery Validation] PASS | schema | .bak | " +
            "3 historicos | corrupcion | save futuro");
    }

    public static void ValidateOrThrow()
    {
        var failures = new List<string>();
        string root = Path.Combine(Path.GetTempPath(),
            "QuantumForgeSaveRecovery_" + Guid.NewGuid().ToString("N"));
        string savePath = Path.Combine(root, "save.json");

        try
        {
            Directory.CreateDirectory(root);
            Write(savePath, 10.0, SaveService.CurrentSaveSchemaVersion);
            Check(SaveService.TryCreateHistoricalSnapshot(
                savePath, savePath, out _),
                "No creo el primer historico.", failures);

            Write(savePath, 20.0, SaveService.CurrentSaveSchemaVersion);
            Check(SaveService.TryCreateHistoricalSnapshot(
                savePath, savePath, out _),
                "No roto el segundo historico.", failures);

            Write(savePath, 30.0, SaveService.CurrentSaveSchemaVersion);
            Check(SaveService.TryCreateHistoricalSnapshot(
                savePath, savePath, out _),
                "No roto el tercer historico.", failures);

            Check(ReadLe(SaveService.GetHistoricalSavePath(savePath, 1)) == 30.0 &&
                  ReadLe(SaveService.GetHistoricalSavePath(savePath, 2)) == 20.0 &&
                  ReadLe(SaveService.GetHistoricalSavePath(savePath, 3)) == 10.0,
                "La rotacion no conserva tres partidas ordenadas.", failures);

            File.WriteAllText(savePath, "{}");
            File.WriteAllText(savePath + ".bak", "save corrupto");
            Check(SaveService.TryFindRecoverableSave(savePath,
                    out SaveData recovered, out string source, out _) &&
                  recovered.LE == 30.0 &&
                  source == SaveService.GetHistoricalSavePath(savePath, 1),
                "No recupera el historico tras corromper principal y .bak.",
                failures);

            File.Delete(savePath);
            Write(savePath + ".bak", 25.0,
                SaveService.CurrentSaveSchemaVersion);
            Check(SaveService.TryFindRecoverableSave(savePath,
                    out recovered, out source, out _) &&
                  recovered.LE == 25.0 && source == savePath + ".bak",
                "No recupera .bak cuando falta save.json.", failures);

            Write(savePath, 99.0, SaveService.CurrentSaveSchemaVersion + 1);
            Check(!SaveService.TryFindRecoverableSave(savePath,
                    out _, out _, out string futureError) &&
                  !string.IsNullOrEmpty(futureError),
                "Una build antigua acepta y puede sobrescribir un schema futuro.",
                failures);
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }

        if (failures.Count > 0)
            throw new InvalidOperationException(
                "[Save Recovery Validation] FAIL\n- " +
                string.Join("\n- ", failures));
    }

    private static void Write(string path, double le, int schemaVersion)
    {
        var data = new SaveData
        {
            saveSchemaVersion = schemaVersion,
            LE = le,
            lastUnix = 1L
        };
        File.WriteAllText(path, JsonUtility.ToJson(data, true));
    }

    private static double ReadLe(string path)
    {
        return SaveService.TryReadSaveData(path, out SaveData data)
            ? data.LE : double.NaN;
    }

    private static void Check(
        bool condition, string message, List<string> failures)
    {
        if (!condition) failures.Add(message);
    }
}
#endif

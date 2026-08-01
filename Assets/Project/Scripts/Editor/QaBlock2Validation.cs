#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class QaBlock2Validation
{
    private const string ScriptsRoot = "Assets/Project/Scripts/";

    [MenuItem("Tools/Quantum Forge/QA/Validate Block 2")]
    public static void ValidateBlock2()
    {
        var failures = new List<string>();
        float initialTimeScale = Time.timeScale;
        FieldInfo availabilityOverride = typeof(QaRuntimeService).GetField(
            "availabilityOverrideForValidation",
            BindingFlags.NonPublic | BindingFlags.Static);

        try
        {
            Check(availabilityOverride != null,
                "No se puede simular disponibilidad pública.", failures);
            if (availabilityOverride == null)
                Finish(failures);

            availabilityOverride.SetValue(null, true);
            QaRuntimeService.ResetToNormalSpeed();

            ValidateRuntimeButton(availabilityOverride, failures);
            ValidateOnlineWiring(failures);
            ValidateSingleScalingBoundary(failures);
            ValidateRealTimeBoundaries(failures);

            QaRuntimeService.TrySetSpeed(10f);
            Check(Math.Abs(QaRuntimeService.ScaleOnlineSeconds(60.0) - 600.0) <
                0.000001,
                "60 s reales a x10 no producen 600 s simulados.", failures);
            Check(Time.timeScale == initialTimeScale && Time.timeScale == 1f,
                "El Bloque 2 alteró Time.timeScale.", failures);
        }
        finally
        {
            if (availabilityOverride != null)
                availabilityOverride.SetValue(null, null);
            QaRuntimeService.ResetToNormalSpeed();
        }

        Finish(failures);
    }

    private static void ValidateRuntimeButton(
        FieldInfo availabilityOverride, List<string> failures)
    {
        GameObject root = null;
        try
        {
            root = new GameObject("QA Block 2 Button",
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Image),
                typeof(Button));
            GameObject labelObject = new GameObject("Label",
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            labelObject.transform.SetParent(root.transform, false);

            Button button = root.GetComponent<Button>();
            Text label = labelObject.GetComponent<Text>();
            DevMultiplierButton controller =
                root.AddComponent<DevMultiplierButton>();
            InvokePrivate(controller, "Awake");
            InvokePrivate(controller, "OnEnable");

            Check(controller != null && root.activeSelf && label.text == "QA x1",
                "El botón no inicia visible como QA x1.", failures);

            float[] expected = { 5f, 10f, 20f, 1f };
            foreach (float multiplier in expected)
            {
                button.onClick.Invoke();
                Check(QaRuntimeService.SimulationMultiplier == multiplier &&
                    label.text == "QA x" + multiplier.ToString("0"),
                    "Texto y velocidad del botón se desincronizaron.", failures);
            }

            InvokePrivate(controller, "OnDisable");
            root.SetActive(false);
            root.SetActive(true);
            InvokePrivate(controller, "OnEnable");
            button.onClick.Invoke();
            Check(QaRuntimeService.SimulationMultiplier == 5f &&
                label.text == "QA x5",
                "Reactivar el botón duplicó listeners.", failures);

            QaRuntimeService.TrySetSpeed(10f);
            Check(label.text == "QA x10",
                "SpeedChanged no actualiza el texto real.", failures);

            availabilityOverride.SetValue(null, false);
            MethodInfo refresh = typeof(DevMultiplierButton).GetMethod(
                "ApplyAvailabilityAndRefresh",
                BindingFlags.NonPublic | BindingFlags.Instance);
            refresh?.Invoke(controller, null);
            Check(!root.activeSelf &&
                QaRuntimeService.SimulationMultiplier == 1f,
                "La build pública no oculta el botón o no fuerza x1.", failures);
        }
        finally
        {
            availabilityOverride.SetValue(null, true);
            if (root != null)
            {
                DevMultiplierButton controller =
                    root.GetComponent<DevMultiplierButton>();
                if (controller != null)
                    InvokePrivate(controller, "OnDisable");
                UnityEngine.Object.DestroyImmediate(root);
            }
            QaRuntimeService.ResetToNormalSpeed();
        }
    }

    private static void ValidateOnlineWiring(List<string> failures)
    {
        string tick = Read("Systems/TickSystem.cs");
        Check(tick.Contains("QaRuntimeService.ScaleOnlineSeconds(_step)") &&
            !tick.Contains("Tick(_step * devMultiplier)"),
            "TickSystem no usa exclusivamente la autoridad QA.", failures);

        string button = Read("UI/DevMultiplierButton.cs");
        Check(button.Contains("QaRuntimeService.CycleSpeed()") &&
            button.Contains("QaRuntimeService.SpeedChanged") &&
            !button.Contains("bool _on") &&
            !button.Contains("DEV x5: ON"),
            "DevMultiplierButton conserva estado o texto DEV heredado.", failures);

        string building = Read("Buildings/BuildingRowUI.cs");
        Check(building.Contains("QaRuntimeService.SimulationMultiplier") &&
            !building.Contains("TickSystem.I.devMultiplier"),
            "BuildingRowUI no muestra el intervalo QA real.", failures);

        string machine = Read("Systems/MachineManager.cs");
        Check(machine.Contains("QaRuntimeService.ScaleOnlineSeconds(") &&
            machine.Contains(
                "Mathf.Min(unscaledDeltaTime, MaxAcceptedFrameDeltaSeconds)") &&
            Count(machine, "QaRuntimeService.ScaleOnlineSeconds") == 1,
            "El análisis de Máquina no limita el delta o no aplica QA " +
            "exactamente una vez.", failures);

        string room2 = Read("UI/Room2PanelUI.cs");
        Check(room2.Contains("QaRuntimeService.ScaleOnlineSeconds(") &&
            room2.Contains("Time.unscaledDeltaTime") &&
            !room2.Contains("currentFusionCooldownSeconds -= Time.deltaTime") &&
            Count(room2, "QaRuntimeService.ScaleOnlineSeconds") == 1,
            "El cooldown del Cuarto 2 no aplica QA exactamente una vez.", failures);
    }

    private static void ValidateSingleScalingBoundary(List<string> failures)
    {
        string[] alreadyCovered =
        {
            "Systems/Dimension1System.cs",
            "Systems/Dimension2System.cs",
            "Systems/Dimension3System.cs",
            "Systems/D3JobQueueSystem.cs",
            "Systems/D3AutomationSystem.cs",
            "Core/GameState.cs"
        };

        foreach (string relativePath in alreadyCovered)
        {
            Check(!Read(relativePath).Contains("QaRuntimeService"),
                relativePath + " aplica un segundo multiplicador QA.", failures);
        }
    }

    private static void ValidateRealTimeBoundaries(List<string> failures)
    {
        string saveService = Read("Systems/SaveService.cs");
        Check(!saveService.Contains("QaRuntimeService") &&
            saveService.Contains("lastUnix") &&
            saveService.Contains("DateTimeOffset.UtcNow"),
            "El cálculo offline dejó de usar exclusivamente tiempo real.", failures);

        string qaService = Read("QA/QaRuntimeService.cs");
        Check(!qaService.Contains("Time.timeScale ="),
            "QaRuntimeService modifica Time.timeScale.", failures);

        string building = Read("Buildings/BuildingRowUI.cs");
        Check(building.Contains("_t += Time.unscaledDeltaTime"),
            "El refresco visual de edificios fue acelerado.", failures);
    }

    private static string Read(string relativePath)
    {
        return File.ReadAllText(ScriptsRoot + relativePath);
    }

    private static void InvokePrivate(object target, string methodName)
    {
        target.GetType().GetMethod(methodName,
            BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(target, null);
    }

    private static int Count(string source, string value)
    {
        int count = 0;
        int index = 0;
        while ((index = source.IndexOf(value, index,
            StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += value.Length;
        }
        return count;
    }

    private static void Finish(List<string> failures)
    {
        if (failures.Count == 0)
        {
            Debug.Log("[QA Block 2] PASS | botón QA x1/x5/x10/x20 | " +
                "Tick global | Máquina | Cuarto 2 | sin doble escala | " +
                "offline/UI en tiempo real | build pública x1");
            return;
        }

        Debug.LogError("[QA Block 2] FAIL\n- " +
            string.Join("\n- ", failures));
        throw new InvalidOperationException(
            "El Bloque 2 no superó su validación.");
    }

    private static void Check(
        bool condition, string failure, List<string> failures)
    {
        if (!condition)
            failures.Add(failure);
    }
}
#endif

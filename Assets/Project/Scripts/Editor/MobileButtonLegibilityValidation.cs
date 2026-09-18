#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class MobileButtonLegibilityValidation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string BuildingRowPath =
        "Assets/Project/Prefabs/BuildingRow.prefab";

    [MenuItem("Tools/Quantum Forge/QA/Validate Mobile Button Legibility")]
    public static void Validate()
    {
        var failures = new List<string>();
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        ValidateLoadedButtons(failures);
        ValidateBuildingButton(failures);
        ValidateLocalizedCompactLabel(failures);
        ValidateTriangleSynchronizationContract(failures);

        if (failures.Count == 0)
        {
            Debug.Log("[Mobile Button Legibility] PASS | botones cargados | " +
                "ajuste automático | Modulador compacto | 120x50 sin recorte | " +
                "Triángulo 0/50% en 90 s");
            return;
        }

        foreach (string failure in failures)
            Debug.LogError("[Mobile Button Legibility] " + failure);
        throw new InvalidOperationException(
            "Mobile Button Legibility falló con " + failures.Count +
            " error(es).");
    }

    public static void ValidateWithRegressions()
    {
        Validate();
        MobileQaFriendlyLayoutValidation.Validate();
        TriangleRedesignValidation.Validate();
        SpanishReleaseReadinessValidation.Validate();
        Debug.Log("[Mobile Button Legibility Regressions] PASS | legibilidad | " +
            "layout móvil | Triángulo | español");
    }

    private static void ValidateLoadedButtons(List<string> failures)
    {
        Button[] buttons = UnityEngine.Object.FindObjectsByType<Button>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        Check(buttons.Length > 0, "Main no contiene botones.", failures);

        MobileQaFriendlyLayout layout =
            UnityEngine.Object.FindFirstObjectByType<MobileQaFriendlyLayout>(
                FindObjectsInactive.Include);
        if (layout != null)
            layout.ApplyLayout();
        Canvas.ForceUpdateCanvases();

        int labels = 0;
        foreach (Button button in buttons)
        {
            MobileQaFriendlyLayout.ConfigureButtonForMobile(button);
            foreach (TMP_Text label in
                button.GetComponentsInChildren<TMP_Text>(true))
            {
                labels++;
                Check(label.enableAutoSizing,
                    button.name + " no activa auto-size.", failures);
                Check(label.fontSizeMin + .01f >=
                        Mathf.Min(14f, label.fontSizeMax) &&
                      label.fontSizeMax <= 28f,
                    button.name + " conserva un rango de fuente inseguro.",
                    failures);
                Check(label.textWrappingMode == TextWrappingModes.Normal,
                    button.name + " no permite ajustar palabras.", failures);
                Check(label.margin.x >= 4f && label.margin.y >= 4f &&
                    label.margin.z >= 4f && label.margin.w >= 4f,
                    button.name + " no conserva margen interior.", failures);

                Rect labelRect = label.rectTransform.rect;
                if (!string.IsNullOrWhiteSpace(label.text) &&
                    labelRect.width >= 10f && labelRect.height >= 10f)
                {
                    label.ForceMeshUpdate(true, true);
                    Check(!label.isTextOverflowing,
                        button.name + " todavía recorta '" + label.text + "'.",
                        failures);
                }
            }
        }

        Check(labels > 0, "No se encontraron etiquetas TMP en botones.", failures);
    }

    private static void ValidateBuildingButton(List<string> failures)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            BuildingRowPath);
        Check(prefab != null, "Falta BuildingRow.prefab.", failures);
        if (prefab == null)
            return;

        GameObject canvasObject = null;
        try
        {
            canvasObject = new GameObject("Mobile Button Validation Canvas",
                typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            GameObject instance = UnityEngine.Object.Instantiate(
                prefab, canvasObject.transform);
            BuildingRowUI row = instance.GetComponent<BuildingRowUI>();
            Check(row != null && row.buyButton != null,
                "BuildingRow no conserva su botón de compra.", failures);
            if (row == null || row.buyButton == null)
                return;

            RectTransform buttonRect =
                row.buyButton.transform as RectTransform;
            TMP_Text label =
                row.buyButton.GetComponentInChildren<TMP_Text>(true);
            Check(buttonRect != null && buttonRect.rect.width >= 120f &&
                buttonRect.rect.height >= 50f,
                "El botón de edificio es menor de 120x50.", failures);
            Check(label != null, "El botón de edificio no tiene texto TMP.",
                failures);
            if (label == null)
                return;

            label.SetText("Triángulo activo");
            MobileQaFriendlyLayout.ConfigureButtonForMobile(row.buyButton);
            Canvas.ForceUpdateCanvases();
            label.ForceMeshUpdate(true, true);
            Check(!label.isTextOverflowing,
                "'Triángulo activo' todavía se recorta en 120x50.", failures);

            label.SetText("Gestionar en el Triángulo");
            Canvas.ForceUpdateCanvases();
            label.ForceMeshUpdate(true, true);
            Check(!label.isTextOverflowing,
                "El ajuste general no resuelve una etiqueta larga de prueba.",
                failures);
        }
        finally
        {
            if (canvasObject != null)
                UnityEngine.Object.DestroyImmediate(canvasObject);
        }
    }

    private static void ValidateLocalizedCompactLabel(List<string> failures)
    {
        string spanish = File.ReadAllText(
            "Assets/Project/Resources/Localization/lang_es.json");
        string english = File.ReadAllText(
            "Assets/Project/Resources/Localization/lang_en.json");
        string rowSource = File.ReadAllText(
            "Assets/Project/Scripts/Buildings/BuildingRowUI.cs");

        Check(spanish.Contains("\"building.modulator.ready\": \"Vértice listo\"") &&
              spanish.Contains("\"building.modulator.active\": \"Triángulo activo\""),
            "Faltan estados compactos del Modulador en español.", failures);
        Check(english.Contains("\"building.modulator.ready\": \"Vertex ready\"") &&
              english.Contains("\"building.modulator.active\": \"Triangle active\""),
            "Faltan estados compactos del Modulador en inglés.", failures);
        Check(rowSource.Contains("building.modulator.ready") && rowSource.Contains("building.modulator.active"),
            "BuildingRow no usa los nuevos estados del Modulador.", failures);
    }

    private static void ValidateTriangleSynchronizationContract(
        List<string> failures)
    {
        Check(Mathf.Approximately(
                GameState.TriangleInitialSynchronization, 0f),
            "La primera activación del Triángulo no comienza en 0%.",
            failures);
        Check(Mathf.Approximately(
                GameState.TriangleSwitchSynchronization, 0.50f),
            "Los cambios de circuito no comienzan en 50%.", failures);
        Check(Math.Abs(GameState.TriangleSynchronizationRecoverySeconds -
                90.0) < 0.0001,
            "La recuperación no dura un minuto y medio.", failures);
    }

    private static void Check(
        bool condition, string message, List<string> failures)
    {
        if (!condition)
            failures.Add(message);
    }
}
#endif

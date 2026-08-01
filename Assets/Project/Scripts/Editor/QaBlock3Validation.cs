#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class QaBlock3Validation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";

    [MenuItem("Tools/Quantum Forge/QA/Validate Block 3")]
    public static void ValidateBlock3()
    {
        var failures = new List<string>();
        FieldInfo availabilityOverride = typeof(QaRuntimeService).GetField(
            "availabilityOverrideForValidation",
            BindingFlags.NonPublic | BindingFlags.Static);
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        QaPanelUI panel = UnityEngine.Object.FindFirstObjectByType<QaPanelUI>(
            FindObjectsInactive.Include);

        try
        {
            Check(CountNamed(scene, "QA_SafeArea") == 1,
                "El Safe Area QA está ausente o duplicado.", failures);
            Check(CountNamed(scene, "QA_ToolsButton") == 1,
                "El acceso HERRAMIENTAS QA está ausente o duplicado.", failures);
            Check(CountNamed(scene, "QA_PanelRoot") == 1,
                "El panel QA está ausente o duplicado.", failures);
            Check(panel != null, "Falta QaPanelUI.", failures);
            if (panel == null)
                Finish(failures);

            ValidateReferencesAndLayout(panel, failures);
            ValidateRuntimeInteraction(panel, availabilityOverride, failures);
            ValidateFutureBoundaries(failures);
        }
        finally
        {
            if (availabilityOverride != null)
                availabilityOverride.SetValue(null, null);
            QaRuntimeService.ResetToNormalSpeed();
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        }

        Finish(failures);
    }

    private static void ValidateReferencesAndLayout(
        QaPanelUI panel, List<string> failures)
    {
        Check(panel.safeAreaRoot != null && panel.toolsButton != null &&
            panel.panelRoot != null && panel.scrollRect != null &&
            panel.speedStatusText != null && panel.closeButton != null,
            "Faltan referencias principales del panel.", failures);
        Check(panel.speedButtons != null && panel.speedButtons.Length == 4 &&
            panel.advanceButtons != null && panel.advanceButtons.Length == 3 &&
            panel.checkpointSaveButtons != null &&
            panel.checkpointSaveButtons.Length == 3 &&
            panel.checkpointLoadButtons != null &&
            panel.checkpointLoadButtons.Length == 3,
            "La cantidad de controles QA no coincide con la especificación.",
            failures);
        Check(panel.confirmationRoot != null && panel.confirmationText != null &&
            panel.confirmationAcceptButton != null &&
            panel.confirmationCancelButton != null,
            "Falta el diálogo de confirmación.", failures);
        Check(panel.scrollRect != null && panel.scrollRect.vertical &&
            !panel.scrollRect.horizontal && panel.scrollRect.viewport != null &&
            panel.scrollRect.content != null &&
            panel.scrollRect.viewport.GetComponent<RectMask2D>() != null &&
            panel.scrollRect.content.GetComponent<ContentSizeFitter>() != null,
            "El contenido no usa un scroll vertical recortado.", failures);
        Check(panel.panelRoot != null && !panel.panelRoot.activeSelf,
            "El panel no inicia cerrado.", failures);

        GameObject speedObject = FindNamed(panel.gameObject.scene,
            "BtnDevMultiplier");
        bool toolsInLegacySafeArea = panel.toolsButton != null &&
            panel.toolsButton.transform.parent == panel.safeAreaRoot;
        bool toolsInVerticalSafeArea = panel.toolsButton != null &&
            panel.toolsButton.name == "Nav_QA" &&
            panel.toolsButton.GetComponentInParent<VerticalSafeAreaLayout>(true) != null;
        Check(speedObject != null &&
            speedObject.transform.parent == panel.safeAreaRoot &&
            (toolsInLegacySafeArea || toolsInVerticalSafeArea),
            "Los accesos QA no están dentro de un Safe Area aprobado.", failures);

        if (speedObject != null)
            ValidateTouchHeight(speedObject.GetComponent<Button>(), failures);
        ValidateTouchHeight(panel.toolsButton, failures);
        ValidateTouchHeight(panel.closeButton, failures);
        ValidateButtons(panel.speedButtons, failures);
        ValidateButtons(panel.advanceButtons, failures);
        ValidateButtons(panel.checkpointSaveButtons, failures);
        ValidateButtons(panel.checkpointLoadButtons, failures);

        TMP_Text title = FindNamed(panel.gameObject.scene, "QA_Title")?
            .GetComponent<TMP_Text>();
        Check(title != null && title.text ==
            "MODO QA - NO REPRESENTA EL BALANCE FINAL",
            "El título de advertencia QA no es exacto.", failures);
    }

    private static void ValidateRuntimeInteraction(
        QaPanelUI panel, FieldInfo availabilityOverride,
        List<string> failures)
    {
        Check(availabilityOverride != null,
            "No se puede validar la build pública.", failures);
        if (availabilityOverride == null)
            return;

        availabilityOverride.SetValue(null, true);
        QaRuntimeService.ResetToNormalSpeed();
        InvokePrivate(panel, "Awake");
        InvokePrivate(panel, "OnEnable");

        panel.toolsButton.onClick.Invoke();
        Check(panel.IsOpen, "El panel no abre con toque.", failures);
        panel.closeButton.onClick.Invoke();
        Check(!panel.IsOpen && !panel.panelRoot.activeInHierarchy,
            "El panel no cierra o conserva raycasts activos.", failures);

        InvokePrivate(panel, "OnDisable");
        InvokePrivate(panel, "OnEnable");
        InvokePrivate(panel, "OnEnable");
        panel.toolsButton.onClick.Invoke();
        Check(panel.IsOpen,
            "Reactivar el panel duplicó listeners del acceso.", failures);

        float[] speeds = { 1f, 5f, 10f, 20f };
        for (int index = 0; index < speeds.Length; index++)
        {
            panel.speedButtons[index].onClick.Invoke();
            Check(QaRuntimeService.SimulationMultiplier == speeds[index] &&
                panel.speedStatusText.text == "VELOCIDAD ACTUAL: QA x" +
                    speeds[index].ToString("0"),
                "Un botón de velocidad no recibe el toque correctamente.",
                failures);
        }

        var advances = new List<double>();
        panel.AdvanceRequested += seconds => advances.Add(seconds);
        panel.advanceButtons[0].onClick.Invoke();
        panel.advanceButtons[1].onClick.Invoke();
        Check(advances.Count == 2 && advances[0] == 300.0 &&
            advances[1] == 1800.0,
            "Los botones +5 MIN/+30 MIN no emiten solicitudes exactas.", failures);
        panel.advanceButtons[2].onClick.Invoke();
        Check(panel.confirmationRoot.activeSelf && advances.Count == 2,
            "+1 H no solicita confirmación previa.", failures);
        panel.confirmationAcceptButton.onClick.Invoke();
        Check(advances.Count == 3 && advances[2] == 3600.0,
            "La confirmación +1 H no emite 3600 s.", failures);

        var saved = new List<char>();
        var loaded = new List<char>();
        panel.SaveCheckpointRequested += slot => saved.Add(slot);
        panel.LoadCheckpointRequested += slot => loaded.Add(slot);
        for (int index = 0; index < 3; index++)
        {
            char expected = (char)('A' + index);
            panel.checkpointSaveButtons[index].onClick.Invoke();
            panel.checkpointLoadButtons[index].onClick.Invoke();
            Check(panel.confirmationRoot.activeSelf,
                "Cargar slot " + expected + " no pide confirmación.", failures);
            panel.confirmationAcceptButton.onClick.Invoke();
        }
        Check(saved.Count == 3 && loaded.Count == 3 &&
            saved[0] == 'A' && saved[1] == 'B' && saved[2] == 'C' &&
            loaded[0] == 'A' && loaded[1] == 'B' && loaded[2] == 'C',
            "Los slots A/B/C no reciben todos los toques.", failures);

        panel.ClosePanel();
        availabilityOverride.SetValue(null, false);
        InvokePrivate(panel, "ApplyAvailability");
        Check(!panel.safeAreaRoot.gameObject.activeSelf && !panel.IsOpen,
            "Los controles QA existen visualmente en build pública.", failures);
        availabilityOverride.SetValue(null, true);
        InvokePrivate(panel, "OnDisable");
    }

    private static void ValidateFutureBoundaries(List<string> failures)
    {
        string source = File.ReadAllText(
            "Assets/Project/Scripts/QA/QaPanelUI.cs");
        Check(!source.Contains("GameState.I.Tick") &&
            !source.Contains("ApplyOfflineProgress") &&
            !source.Contains("persistentDataPath") &&
            !source.Contains("SaveService"),
            "El Bloque 3 adelantó lógica de los Bloques 4 o 5.", failures);
    }

    private static void ValidateButtons(Button[] buttons, List<string> failures)
    {
        if (buttons == null)
            return;
        foreach (Button button in buttons)
            ValidateTouchHeight(button, failures);
    }

    private static void ValidateTouchHeight(Button button, List<string> failures)
    {
        if (button == null)
        {
            failures.Add("Existe un botón QA nulo.");
            return;
        }

        RectTransform rect = button.transform as RectTransform;
        LayoutElement layout = button.GetComponent<LayoutElement>();
        float minimum = layout != null
            ? Math.Max(layout.minHeight, layout.preferredHeight)
            : rect != null ? rect.sizeDelta.y : 0f;
        Check(minimum >= 52f,
            button.name + " tiene altura táctil menor a 52 px.", failures);
    }

    private static int CountNamed(Scene scene, string name)
    {
        int count = 0;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Transform current in
                root.GetComponentsInChildren<Transform>(true))
            {
                if (current.name == name)
                    count++;
            }
        }
        return count;
    }

    private static GameObject FindNamed(Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Transform current in
                root.GetComponentsInChildren<Transform>(true))
            {
                if (current.name == name)
                    return current.gameObject;
            }
        }
        return null;
    }

    private static void InvokePrivate(object target, string methodName)
    {
        target.GetType().GetMethod(methodName,
            BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(target, null);
    }

    private static void Finish(List<string> failures)
    {
        if (failures.Count == 0)
        {
            Debug.Log("[QA Block 3] PASS | Safe Area | acceso táctil | " +
                "scroll vertical/horizontal | panel abre/cierra | confirmaciones | " +
                "slots A/B/C | build pública oculta | setup idempotente");
            return;
        }

        Debug.LogError("[QA Block 3] FAIL\n- " +
            string.Join("\n- ", failures));
        throw new InvalidOperationException(
            "El Bloque 3 no superó su validación.");
    }

    private static void Check(
        bool condition, string failure, List<string> failures)
    {
        if (!condition)
            failures.Add(failure);
    }
}
#endif

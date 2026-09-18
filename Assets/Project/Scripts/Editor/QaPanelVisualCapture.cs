#if UNITY_EDITOR
using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class QaPanelVisualCapture
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory =>
        Path.GetFullPath("Logs/VisualQA/QaReset");

    [MenuItem("Tools/Quantum Forge/QA/Capture Reset Panel")]
    public static void RunBatch()
    {
        Directory.CreateDirectory(OutputDirectory);
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        QaPanelUI panel = UnityEngine.Object.FindFirstObjectByType<QaPanelUI>(
            FindObjectsInactive.Include);
        Require(panel != null && panel.panelRoot != null &&
            panel.resetSaveButton != null && panel.confirmationRoot != null &&
            panel.confirmationText != null && panel.scrollRect != null &&
            panel.checkpointStatusTexts != null &&
            panel.checkpointStatusTexts.Length == 3 &&
            panel.checkpointLoadButtons != null &&
            panel.checkpointLoadButtons.Length == 3,
            "Faltan referencias del reset QA.");

        bool safeWasActive = panel.safeAreaRoot.gameObject.activeSelf;
        bool panelWasActive = panel.panelRoot.activeSelf;
        bool confirmationWasActive = panel.confirmationRoot.activeSelf;
        float previousScroll = panel.scrollRect.verticalNormalizedPosition;
        string previousMessage = panel.confirmationText.text;
        string[] previousStatuses = new string[panel.checkpointStatusTexts.Length];
        Color[] previousStatusColors = new Color[panel.checkpointStatusTexts.Length];
        bool[] previousLoadState = new bool[panel.checkpointLoadButtons.Length];
        for (int index = 0; index < previousStatuses.Length; index++)
        {
            previousStatuses[index] = panel.checkpointStatusTexts[index].text;
            previousStatusColors[index] = panel.checkpointStatusTexts[index].color;
            previousLoadState[index] = panel.checkpointLoadButtons[index].interactable;
        }
        try
        {
            panel.safeAreaRoot.gameObject.SetActive(true);
            panel.panelRoot.SetActive(true);
            panel.confirmationRoot.SetActive(false);
            panel.scrollRect.verticalNormalizedPosition = 0f;
            panel.SetCheckpointSlotStatus(0,
                "GUARDADO\n12/09 10:18", true);
            panel.SetCheckpointSlotStatus(1, "VACÍO", false);
            panel.SetCheckpointSlotStatus(2,
                "GUARDADO\n12/09 10:42", true);
            Canvas.ForceUpdateCanvases();
            Canvas.ForceUpdateCanvases();

            Capture("01_qa_reset_button_1080x1920.png", 1080, 1920);
            Capture("01_qa_reset_button_720x1280.png", 720, 1280);

            panel.confirmationText.SetText(
                "¿BORRAR TODA LA PARTIDA Y EMPEZAR DE NUEVO? " +
                "Esta acción no se puede deshacer. " +
                "Los checkpoints QA A/B/C se conservarán.");
            panel.confirmationRoot.SetActive(true);
            Canvas.ForceUpdateCanvases();
            Capture("02_qa_reset_confirmation_1080x1920.png", 1080, 1920);
            Capture("02_qa_reset_confirmation_720x1280.png", 720, 1280);

            Require(CountNamed(scene, "QA_ResetSave") == 1 &&
                CountNamed(scene, "BtnDevReset") == 0,
                "La captura encontró reset duplicado o acceso legacy.");
            Debug.Log("[QA Reset Capture] PASS | botón + confirmación | " +
                "1080x1920 + 720x1280 | escena no guardada");
        }
        finally
        {
            for (int index = 0; index < previousStatuses.Length; index++)
            {
                panel.checkpointStatusTexts[index].SetText(previousStatuses[index]);
                panel.checkpointStatusTexts[index].color = previousStatusColors[index];
                panel.checkpointLoadButtons[index].interactable =
                    previousLoadState[index];
            }
            panel.confirmationText.SetText(previousMessage);
            panel.scrollRect.verticalNormalizedPosition = previousScroll;
            panel.confirmationRoot.SetActive(confirmationWasActive);
            panel.panelRoot.SetActive(panelWasActive);
            panel.safeAreaRoot.gameObject.SetActive(safeWasActive);
        }
    }

    private static void Capture(string fileName, int width, int height)
    {
        string path = Path.Combine(OutputDirectory, fileName);
        Camera camera = Camera.main != null
            ? Camera.main
            : UnityEngine.Object.FindFirstObjectByType<Camera>(
                FindObjectsInactive.Include);
        Require(camera != null, "No hay cámara para capturar QA.");

        Canvas[] canvases = UnityEngine.Object.FindObjectsByType<Canvas>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        RenderMode[] modes = new RenderMode[canvases.Length];
        Camera[] cameras = new Camera[canvases.Length];
        float[] distances = new float[canvases.Length];
        RenderTexture target = new RenderTexture(width, height, 24,
            RenderTextureFormat.ARGB32);
        RenderTexture previousTarget = camera.targetTexture;
        RenderTexture previousActive = RenderTexture.active;
        try
        {
            target.Create();
            camera.targetTexture = target;
            for (int index = 0; index < canvases.Length; index++)
            {
                modes[index] = canvases[index].renderMode;
                cameras[index] = canvases[index].worldCamera;
                distances[index] = canvases[index].planeDistance;
                if (modes[index] == RenderMode.ScreenSpaceOverlay)
                {
                    canvases[index].renderMode = RenderMode.ScreenSpaceCamera;
                    canvases[index].worldCamera = camera;
                    canvases[index].planeDistance = 1f;
                }
            }
            RecalculateCanvasScalers(canvases);
            Canvas.ForceUpdateCanvases();
            camera.Render();
            RenderTexture.active = target;
            Texture2D image = new Texture2D(
                width, height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
            image.Apply(false, false);
            File.WriteAllBytes(path, image.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(image);
        }
        finally
        {
            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            target.Release();
            UnityEngine.Object.DestroyImmediate(target);
            for (int index = 0; index < canvases.Length; index++)
            {
                canvases[index].renderMode = modes[index];
                canvases[index].worldCamera = cameras[index];
                canvases[index].planeDistance = distances[index];
            }
        }

        Require(File.Exists(path) && new FileInfo(path).Length > 4096,
            "No se generó " + fileName);
        Debug.Log("[QA Reset Capture] PNG | " + path);
    }

    private static void RecalculateCanvasScalers(Canvas[] canvases)
    {
        MethodInfo handle = typeof(CanvasScaler).GetMethod(
            "Handle", BindingFlags.Instance | BindingFlags.NonPublic);
        if (handle == null)
            return;
        foreach (Canvas canvas in canvases)
        {
            CanvasScaler scaler = canvas != null
                ? canvas.GetComponent<CanvasScaler>()
                : null;
            if (scaler != null && scaler.enabled)
                handle.Invoke(scaler, null);
        }
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

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
#endif

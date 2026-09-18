#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Main real en Play Mode; todos los guardados se desvían antes de Awake/Start.
public static class ReleaseRecoveryRuntimeValidation
{
    private const string Key = "QF.ReleaseRecovery.";
    private static int frames;
    private static double frozenLE;
    private static double startedAt;
    private static string Folder => SessionState.GetString(Key + "Folder", "");
    private static string SavePath => Path.Combine(Folder, "save.json");
    private const BindingFlags PrivateInstance = BindingFlags.NonPublic | BindingFlags.Instance;

    [InitializeOnLoadMethod]
    private static void Resume()
    {
        if (!SessionState.GetBool(Key + "Active", false)) return;
        SaveService.EditorValidationSaveDirectory = Folder;
        Application.logMessageReceived -= CaptureError;
        Application.logMessageReceived += CaptureError;
        EditorApplication.playModeStateChanged -= Changed;
        EditorApplication.playModeStateChanged += Changed;
        if (EditorApplication.isPlaying) StartTicks();
    }

    public static void Run()
    {
        if (!Application.isBatchMode || EditorApplication.isPlayingOrWillChangePlaymode)
            throw new InvalidOperationException("Dedicated batch Editor required.");
        string folder = Path.GetFullPath(Path.Combine("Logs/ReleaseD1D3",
            "Runtime_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss")));
        Directory.CreateDirectory(folder);
        SessionState.SetString(Key + "Folder", folder);
        SessionState.SetBool(Key + "Active", true);
        SessionState.SetBool(Key + "Failed", false);
        SessionState.SetInt(Key + "Stage", 0);
        SaveService.EditorValidationSaveDirectory = folder;
        File.WriteAllText(SavePath, "{deliberately-truncated-fixture");
        Resume();
        EditorSceneManager.OpenScene("Assets/Project/Scenes/Main.unity", OpenSceneMode.Single);
        EditorApplication.isPlaying = true;
    }

    private static void CaptureError(string message, string stack, LogType type)
    {
        if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert) return;
        if (message.StartsWith("[SaveService] No se modificara la partida:", StringComparison.Ordinal)) return;
        SessionState.SetBool(Key + "Failed", true);
    }

    private static void Changed(PlayModeStateChange change)
    {
        SaveService.EditorValidationSaveDirectory = Folder;
        if (change == PlayModeStateChange.EnteredPlayMode) StartTicks();
        if (change != PlayModeStateChange.EnteredEditMode) return;
        EditorApplication.update -= Tick;
        bool failed = SessionState.GetBool(Key + "Failed", false);
        Debug.Log("[Release Recovery Runtime] " + (failed ? "FAIL" : "PASS") +
            " | Main Play Mode | carga rechazada | bloqueo | raycast y reintento | pausa/reanudacion | cierre/carga | " + Folder);
        File.WriteAllText(Path.Combine(Folder, "result.txt"), failed ? "FAIL" : "PASS");
        // Mantener ruta aislada también durante el cierre del proceso y sus callbacks.
        EditorApplication.Exit(failed ? 1 : 0);
    }

    private static void StartTicks()
    {
        frames = 0;
        startedAt = EditorApplication.timeSinceStartup;
        EditorApplication.update -= Tick;
        EditorApplication.update += Tick;
    }

    private static void Tick()
    {
        try
        {
            if (EditorApplication.timeSinceStartup - startedAt > 120)
                throw new TimeoutException("Runtime stage exceeded 120 seconds.");
            if (++frames < 20) return;
            frames = 0;
            Require(SaveService.I != null && GameState.I != null, "Main did not initialize.");
            Require(SaveService.I.CurrentSavePath == SavePath, "Save isolation lost after domain reload.");
            var ui = UnityEngine.Object.FindFirstObjectByType<PresentationReturnReportUI>();
            Require(ui != null, "Return report UI missing.");
            int stage = SessionState.GetInt(Key + "Stage", 0);
            switch (stage)
            {
                case 0:
                    Require(SaveService.I.HasLoadFailure, "Corrupt save accepted.");
                    SetResolution(1080, 1920);
                    frozenLE = GameState.I.LE;
                    break;
                case 1:
                    Require(GameState.I.LE == frozenLE, "Game progressed after failed load.");
                    CheckErrorUi(ui);
                    Capture("load_failed_1080x1920.png", 1080, 1920);
                    ClickRetry(ui);
                    ClickRetry(ui);
                    break;
                case 2:
                    Require(SaveService.I.HasLoadFailure && File.ReadAllText(SavePath).StartsWith("{deliberately"),
                        "Repeated rejected retry replaced the save.");
                    SetResolution(720, 1280);
                    break;
                case 3:
                    CheckErrorUi(ui);
                    Capture("load_failed_720x1280.png", 720, 1280);
                    string source = "Logs/ReleaseD1D3/20260908_163135_72478c12/fixtures/dimension_roundtrip/before_hitos.json";
                    var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(source));
                    data.LE = data.maxLEAlcanzado = 1000;
                    data.baseLEps = 2;
                    data.lastUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    File.WriteAllText(SavePath, JsonUtility.ToJson(data));
                    ClickRetry(ui);
                    break;
                case 4:
                    Require(!SaveService.I.HasLoadFailure && GameState.I.LE >= 1000, "Valid retry failed.");
                    Require(!ui.transform.Find("ReturnModal").gameObject.activeSelf,
                        "Recovery overlay still blocks successful load.");
                    Pause(true);
                    Require(SaveService.TryReadSaveData(SavePath, out SaveData paused), "Pause did not save.");
                    paused.lastUnix -= 60;
                    File.WriteAllText(SavePath, JsonUtility.ToJson(paused));
                    Pause(false);
                    double resumed = GameState.I.LE;
                    Require(resumed >= paused.LE + 119 && resumed <= paused.LE + 130,
                        "Resume did not apply approximately 60 seconds once: delta=" + (resumed - paused.LE));
                    string once = File.ReadAllText(SavePath);
                    Pause(false);
                    Require(GameState.I.LE == resumed && File.ReadAllText(SavePath) == once,
                        "Duplicate resume granted progress or saved again.");
                    typeof(SaveService).GetMethod("OnApplicationQuit", PrivateInstance).Invoke(SaveService.I, null);
                    GameState.I.LE = -100;
                    SaveService.I.Load();
                    Require(GameState.I.LE >= resumed && GameState.I.LE < resumed + 10,
                        "Close/reload lost or duplicated progress.");
                    break;
                case 5:
                    SetResolution(1080, 1920);
                    ArchiveValidation("PrepareFixture");
                    break;
                case 6:
                    ArchiveValidation("ValidatePhysicalRoutesAndRestore");
                    CheckArchiveThresholds();
                    ArchiveValidation("ValidateVisibleState");
                    Capture("archive_1080x1920.png", 1080, 1920);
                    SetResolution(720, 1280);
                    break;
                case 7:
                    CheckArchiveThresholds();
                    ArchiveValidation("ValidateVisibleState");
                    Capture("archive_720x1280.png", 720, 1280);
                    CompareArchive("1080x1920");
                    CompareArchive("720x1280");
                    break;
                default:
                    Require(!SaveService.I.HasLoadFailure, "Game returned to failed state.");
                    EditorApplication.update -= Tick;
                    EditorApplication.isPlaying = false;
                    return;
            }
            SessionState.SetInt(Key + "Stage", stage + 1);
            startedAt = EditorApplication.timeSinceStartup;
        }
        catch (Exception exception)
        {
            SessionState.SetBool(Key + "Failed", true);
            Debug.LogException(exception);
            EditorApplication.update -= Tick;
            EditorApplication.isPlaying = false;
        }
    }

    private static void ArchiveValidation(string method) => typeof(Dimension2ArchiveCapture)
        .GetMethod(method, BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);

    private static void CompareArchive(string resolution)
    {
        var result = UIReferenceComparison.CompareFiles(
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/DIMENSION_2/CORREGIDAS_V4_2026-08-21/18_Archivo_Mejoras_Permanentes_Corregido_V4.png",
            Path.Combine(Folder, "archive_" + resolution + ".png"), Path.Combine(Folder, "comparison_" + resolution));
        Debug.Log("[Release Archive] " + resolution + " perceptual=" + result.perceptualSimilarity);
        Require(result.perceptualSimilarity >= 0.95f, "Archive visual gate below 95%.");
    }

    private static void CheckArchiveThresholds()
    {
        var archive = UnityEngine.Object.FindFirstObjectByType<D2ArchivePanelUI>();
        Require(archive != null, "Archive not visible.");
        archive.Refresh();
        for (int index = 0; index < 3; index++)
        {
            var label = archive.transform.Find("ArchiveEntityThreshold" + index).GetComponent<TMP_Text>();
            Require(index == 0 ? label.gameObject.activeSelf && label.text == "UMBRAL DEL ENTE: 1"
                : !label.gameObject.activeSelf && label.text == string.Empty, "Stale entity threshold " + index);
        }
    }

    private static void CheckErrorUi(PresentationReturnReportUI ui)
    {
        Require(ui.transform.Find("ReturnModal").gameObject.activeInHierarchy, "Error overlay missing.");
        Require(ui.GetComponent<Canvas>().sortingOrder == 32760, "Error overlay not above game.");
        Require(ui.GetComponentsInChildren<TMP_Text>().Length > 0, "Error has no readable text.");
        Canvas.ForceUpdateCanvases();
        var pointer = new PointerEventData(EventSystem.current) { position = new Vector2(2, 2) };
        var hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, hits);
        Require(hits.Count > 0 && hits[0].gameObject.transform.IsChildOf(ui.transform),
            "Covered gameplay still receives raycasts.");
    }

    private static void ClickRetry(PresentationReturnReportUI ui)
    {
        Button button = ui.GetComponentInChildren<Button>();
        Require(button != null && button.interactable, "Retry missing or disabled.");
        var rect = (RectTransform)button.transform;
        Vector2 point = RectTransformUtility.WorldToScreenPoint(null, rect.TransformPoint(rect.rect.center));
        Require(point.x >= 0 && point.x <= Screen.width && point.y >= 0 && point.y <= Screen.height,
            "Retry outside viewport " + Screen.width + "x" + Screen.height);
        var pointer = new PointerEventData(EventSystem.current) { position = point, button = PointerEventData.InputButton.Left };
        var hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, hits);
        Require(hits.Count > 0 && ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject) == button.gameObject,
            "Raycast does not reach Retry.");
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerClickHandler);
    }

    private static void Pause(bool value) => typeof(SaveService).GetMethod("OnApplicationPause", PrivateInstance)
        .Invoke(SaveService.I, new object[] { value });

    private static void Capture(string name, int width, int height)
    {
        typeof(PresentationReturnReportCapture).GetMethod("Capture", BindingFlags.Static | BindingFlags.NonPublic)
            .Invoke(null, new object[] { Path.Combine(Folder, name), width, height });
    }

    private static void SetResolution(int width, int height)
    {
        Assembly assembly = typeof(Editor).Assembly;
        Type sizesType = assembly.GetType("UnityEditor.GameViewSizes");
        Type singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        object sizes = singleton.GetProperty("instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).GetValue(null);
        object groupType = sizesType.GetProperty("currentGroupType").GetValue(sizes);
        object group = sizesType.GetMethod("GetGroup").Invoke(sizes, new[] { groupType });
        Type sizeType = assembly.GetType("UnityEditor.GameViewSize");
        Type sizeKind = assembly.GetType("UnityEditor.GameViewSizeType");
        object size = Activator.CreateInstance(sizeType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null,
            new[] { Enum.Parse(sizeKind, "FixedResolution"), (object)width, height, "Release Recovery " + width }, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        int count = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        EditorWindow view = EditorWindow.GetWindow(assembly.GetType("UnityEditor.GameView"));
        view.GetType().GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(view, count - 1);
        Screen.SetResolution(width, height, false);
        Canvas.ForceUpdateCanvases();
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
#endif

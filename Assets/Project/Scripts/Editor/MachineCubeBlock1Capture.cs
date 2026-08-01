#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MachineCubeBlock1Capture
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ActiveKey = "QF.MachineCubeCapture.Active";
    private const string StageKey = "QF.MachineCubeCapture.Stage";
    private const string FramesKey = "QF.MachineCubeCapture.Frames";
    private const string FailedKey = "QF.MachineCubeCapture.Failed";
    private const string FaceReadyKey = "QF.MachineCubeCapture.FaceReady";
    private const string SingleFaceKey = "QF.MachineCubeCapture.SingleFace";
    private const string SingleRepairedKey = "QF.MachineCubeCapture.SingleRepaired";
    private const string RotationPrototypeKey = "QF.MachineCubeCapture.RotationPrototype";
    private const string True3DPrototypeKey = "QF.MachineCubeCapture.True3DPrototype";
    private const string CompletedKey = "QF.MachineCubeCapture.Completed";
    private const string SaveExistedKey = "QF.MachineCubeCapture.SaveExisted";
    private const string BackupExistedKey = "QF.MachineCubeCapture.BackupExisted";
    private const string QualityBeforeKey = "QF.MachineCubeCapture.QualityBefore";

    private static Camera _captureCamera;
    private static RenderTexture _captureTarget;
    private static RenderTexture _previousCameraTarget;
    private static Canvas[] _captureCanvases;
    private static RenderMode[] _previousCanvasModes;
    private static Camera[] _previousCanvasCameras;
    private static float[] _previousPlaneDistances;

    private static string OutputDirectory =>
        Path.GetFullPath("Logs/VisualQA/MachineBlock1");
    private static string SavePath =>
        Path.Combine(Application.persistentDataPath, "save.json");
    private static string SaveBackupPath => SavePath + ".bak";
    private static string SessionSave =>
        Path.Combine(OutputDirectory, ".save.session-backup");
    private static string SessionBackup =>
        Path.Combine(OutputDirectory, ".save.bak.session-backup");
    private static string SessionSaveMissing =>
        Path.Combine(OutputDirectory, ".save.session-missing");
    private static string SessionBackupMissing =>
        Path.Combine(OutputDirectory, ".save.bak.session-missing");

    [InitializeOnLoadMethod]
    private static void ResumeAfterReload()
    {
        if (!SessionState.GetBool(ActiveKey, false))
            return;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        if (EditorApplication.isPlaying)
        {
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
        }
    }

    [MenuItem("Tools/Quantum Forge/Machine/Capture Block 1 Evidence")]
    public static void Run()
    {
        RunInternal(-1, false);
    }

    public static void RunFace3InitialBatch()
    {
        RunInternal(2, false);
    }

    public static void RunFace2InitialBatch()
    {
        RunInternal(1, false);
    }

    public static void RunFace4InitialBatch()
    {
        RunInternal(3, false);
    }

    public static void RunFace2RepairedBatch()
    {
        RunInternal(1, true);
    }

    public static void RunFace4RepairedBatch()
    {
        RunInternal(3, true);
    }

    [MenuItem("Tools/Quantum Forge/Machine/Capture Physical Rotation Prototype")]
    public static void RunRotationPrototypeBatch()
    {
        RunInternal(-1, false, true);
    }

    [MenuItem("Tools/Quantum Forge/Machine/Capture True 3D Prototype")]
    public static void RunTrue3DPrototypeBatch()
    {
        RunInternal(-1, false, false, true);
    }

    private static void RunInternal(int singleFace, bool singleRepaired,
        bool rotationPrototype = false, bool true3DPrototype = false)
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            throw new InvalidOperationException("La captura debe comenzar fuera de Play Mode.");

        Directory.CreateDirectory(OutputDirectory);
        RecoverStaleBackup();
        if (singleFace < 0 && !rotationPrototype && !true3DPrototype)
        {
            foreach (string path in Directory.GetFiles(OutputDirectory, "*.png"))
                File.Delete(path);
        }
        else if (rotationPrototype)
        {
            foreach (string path in Directory.GetFiles(OutputDirectory,
                "rotation_*.png"))
                File.Delete(path);
        }
        else if (true3DPrototype)
        {
            foreach (string path in Directory.GetFiles(OutputDirectory,
                "true3d_*.png"))
                File.Delete(path);
        }
        BackupUserSave();
        try
        {
            MachineCubeBlock1Setup.ConfigureBlock1Cube();
            MachineCubeBlock1Validation.Validate();
            if (true3DPrototype)
            {
                MachineCube3DPrototypeSetup.ConfigurePrototype();
                MachineCube3DPrototypeSetup.ValidatePrototype();
            }
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            SessionState.SetBool(ActiveKey, true);
            SessionState.SetBool(FailedKey, false);
            SessionState.SetBool(CompletedKey, false);
            SessionState.SetBool(FaceReadyKey, false);
            SessionState.SetInt(SingleFaceKey, singleFace);
            SessionState.SetBool(SingleRepairedKey, singleRepaired);
            SessionState.SetBool(RotationPrototypeKey, rotationPrototype);
            SessionState.SetBool(True3DPrototypeKey, true3DPrototype);
            SessionState.SetInt(QualityBeforeKey, (int)MachineCube3DQuality.Current);
            SessionState.SetInt(StageKey, 0);
            SessionState.SetInt(FramesKey, 0);
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            EditorApplication.isPlaying = true;
        }
        catch
        {
            SessionState.SetBool(ActiveKey, false);
            RestoreUserSave();
            throw;
        }
    }

    public static void RunBatch()
    {
        Run();
    }

    private static void OnPlayModeChanged(PlayModeStateChange change)
    {
        if (change == PlayModeStateChange.EnteredPlayMode)
        {
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
            return;
        }
        if (change != PlayModeStateChange.EnteredEditMode)
            return;

        EditorApplication.update -= Tick;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EndCaptureRendering();
        bool failed = SessionState.GetBool(FailedKey, false);
        failed |= !SessionState.GetBool(CompletedKey, false);
        try
        {
            RestoreUserSave();
        }
        catch (Exception exception)
        {
            failed = true;
            Debug.LogException(exception);
        }
        finally
        {
            MachineCube3DQuality.Set((MachineCube3DQualityLevel)
                SessionState.GetInt(QualityBeforeKey,
                    (int)MachineCube3DQualityLevel.High));
            SessionState.SetBool(ActiveKey, false);
            SessionState.SetBool(FailedKey, false);
        }

        if (!failed)
        {
            if (SessionState.GetBool(True3DPrototypeKey, false))
            {
                Debug.Log("[Machine Cube True 3D Capture] PASS | damaged + repaired | " +
                    "rest + 45deg + 90deg | physical Transform both directions | " +
                    "Low/Balanced/High runtime profiles | 1080x1920 | save restored");
            }
            else if (SessionState.GetBool(RotationPrototypeKey, false))
            {
                Debug.Log("[Machine Cube Rotation Capture] PASS | start + midpoint + end | physical coroutine both directions | interaction restored | 1080x1920 | save restored");
            }
            else
            {
            int singleFace = SessionState.GetInt(SingleFaceKey, -1);
            if (singleFace >= 0)
            {
                bool repaired = SessionState.GetBool(SingleRepairedKey, false);
                Debug.Log($"[Machine Cube Capture] PASS | face {singleFace + 1} {(repaired ? "repaired" : "initial")} | 1080x1920 | save restored");
            }
            else
            {
                Debug.Log("[Machine Cube Capture] PASS | 4 initial + 4 repaired | 1080x1920 | save restored");
            }
            }
        }
        if (Application.isBatchMode)
            EditorApplication.Exit(failed ? 1 : 0);
    }

    private static void Tick()
    {
        int frames = SessionState.GetInt(FramesKey, 0) + 1;
        SessionState.SetInt(FramesKey, frames);
        if (frames < 28)
            return;

        try
        {
            int stage = SessionState.GetInt(StageKey, 0);
            int singleFace = SessionState.GetInt(SingleFaceKey, -1);
            if (SessionState.GetBool(True3DPrototypeKey, false))
            {
                CaptureTrue3DPrototype(stage);
                return;
            }
            if (SessionState.GetBool(RotationPrototypeKey, false))
            {
                CaptureRotationPrototype(stage);
                return;
            }
            if (singleFace >= 0)
            {
                bool repaired = SessionState.GetBool(SingleRepairedKey, false);
                if (stage == 0)
                {
                    PrepareMachine(repaired);
                    ShowFace(singleFace);
                    BeginCaptureRendering(1080, 1920);
                    Advance(1);
                    return;
                }

                int prefix = repaired ? 6 + singleFace : 1 + singleFace;
                string state = repaired ? "repaired" : "initial";
                Capture($"{prefix:00}_face_{singleFace + 1}_{state}_1080x1920.png");
                EditorApplication.update -= Tick;
                EndCaptureRendering();
                SessionState.SetBool(CompletedKey, true);
                EditorApplication.isPlaying = false;
                return;
            }

            if (stage == 0)
            {
                PrepareMachine(false);
                Advance(1);
                return;
            }
            if (stage >= 1 && stage <= 4)
            {
                int face = stage - 1;
                if (!SessionState.GetBool(FaceReadyKey, false))
                {
                    EndCaptureRendering();
                    PrepareMachine(false);
                    ShowFace(face);
                    BeginCaptureRendering(1080, 1920);
                    SessionState.SetBool(FaceReadyKey, true);
                    SessionState.SetInt(FramesKey, 0);
                    return;
                }
                Capture($"{stage:00}_face_{face + 1}_initial_1080x1920.png");
                SessionState.SetBool(FaceReadyKey, false);
                Advance(stage + 1);
                return;
            }
            if (stage == 5)
            {
                EndCaptureRendering();
                PrepareMachine(true);
                Advance(6);
                return;
            }
            if (stage >= 6 && stage <= 9)
            {
                int face = stage - 6;
                if (!SessionState.GetBool(FaceReadyKey, false))
                {
                    EndCaptureRendering();
                    PrepareMachine(true);
                    ShowFace(face);
                    BeginCaptureRendering(1080, 1920);
                    SessionState.SetBool(FaceReadyKey, true);
                    SessionState.SetInt(FramesKey, 0);
                    return;
                }
                Capture($"{stage:00}_face_{face + 1}_repaired_1080x1920.png");
                SessionState.SetBool(FaceReadyKey, false);
                Advance(stage + 1);
                return;
            }

            EditorApplication.update -= Tick;
            EndCaptureRendering();
            SessionState.SetBool(CompletedKey, true);
            EditorApplication.isPlaying = false;
        }
        catch (Exception exception)
        {
            SessionState.SetBool(FailedKey, true);
            Debug.LogException(exception);
            EditorApplication.update -= Tick;
            EndCaptureRendering();
            EditorApplication.isPlaying = false;
        }
    }

    private static void CaptureRotationPrototype(int stage)
    {
        MachineCubeVisualUI visual = UnityEngine.Object.FindFirstObjectByType<MachineCubeVisualUI>(
            FindObjectsInactive.Include);

        if (stage == 0)
        {
            PrepareMachine(false);
            ShowFace(0);
            BeginCaptureRendering(1080, 1920);
            Advance(1);
            return;
        }

        Require(visual != null, "MachineCubeVisualUI no disponible para capturar el giro.");
        if (stage == 1)
        {
            Require(visual.ShowRotationPrototypePose(1, 1, 0f),
                "No se pudo preparar el inicio del giro fÃ­sico.");
            Capture("rotation_01_start_face_1_to_2.png");
            Advance(2);
            return;
        }
        if (stage == 2)
        {
            Require(visual.ShowRotationPrototypePose(1, 1, 0.5f),
                "No se pudo preparar la mitad del giro fÃ­sico.");
            Capture("rotation_02_mid_face_1_to_2.png");
            Advance(3);
            return;
        }
        if (stage == 3)
        {
            visual.CompleteRotationPrototypePose(1);
            Canvas.ForceUpdateCanvases();
            Capture("rotation_03_end_face_1_to_2.png");
            visual.RotateBy(-1);
            Require(visual.IsRotating,
                "El giro real Cara 2 a Cara 1 no comenzÃ³.");
            CanvasGroup interaction = FindMachineInteractionGroup();
            Require(interaction != null && !interaction.interactable &&
                !interaction.blocksRaycasts,
                "La interacciÃ³n no se bloqueÃ³ durante el giro real.");
            Advance(4);
            return;
        }
        if (stage == 4)
        {
            if (visual.IsRotating)
                return;
            MachinePanelUI panel = UnityEngine.Object.FindFirstObjectByType<MachinePanelUI>(
                FindObjectsInactive.Include);
            Require(panel != null && panel.CurrentZone == MachineZoneType.Room1Link,
                "El giro real inverso no terminÃ³ en Cara 1.");
            CanvasGroup interaction = FindMachineInteractionGroup();
            Require(interaction != null && interaction.interactable &&
                interaction.blocksRaycasts,
                "La interacciÃ³n no se restaurÃ³ tras el giro inverso.");
            visual.RotateBy(1);
            Require(visual.IsRotating,
                "El giro real Cara 1 a Cara 2 no comenzÃ³.");
            Advance(5);
            return;
        }
        if (stage == 5)
        {
            if (visual.IsRotating)
                return;
            MachinePanelUI panel = UnityEngine.Object.FindFirstObjectByType<MachinePanelUI>(
                FindObjectsInactive.Include);
            Require(panel != null && panel.CurrentZone == MachineZoneType.FusionSector,
                "El giro real directo no terminÃ³ en Cara 2.");
            CanvasGroup interaction = FindMachineInteractionGroup();
            Require(interaction != null && interaction.interactable &&
                interaction.blocksRaycasts,
                "La interacciÃ³n no se restaurÃ³ tras el giro directo.");
            EditorApplication.update -= Tick;
            EndCaptureRendering();
            SessionState.SetBool(CompletedKey, true);
            EditorApplication.isPlaying = false;
        }
    }

    private static CanvasGroup FindMachineInteractionGroup()
    {
        MachinePanelUI panel = UnityEngine.Object.FindFirstObjectByType<MachinePanelUI>(
            FindObjectsInactive.Include);
        Transform root = panel != null
            ? panel.transform.Find("MachineCubeVisualRoot")
            : null;
        return root != null ? root.GetComponent<CanvasGroup>() : null;
    }

    private static void CaptureTrue3DPrototype(int stage)
    {
        MachineCube3DPrototypeController controller =
            UnityEngine.Object.FindFirstObjectByType<MachineCube3DPrototypeController>(
                FindObjectsInactive.Include);

        if (stage == 0)
        {
            PrepareMachine(false);
            ShowFace(0);
            Require(controller != null, "MachineCube3DPrototypeController no disponible.");
            ValidateQualityProfiles(controller);
            MachineCube3DVisualStateController initialVisualState =
                UnityEngine.Object.FindFirstObjectByType<MachineCube3DVisualStateController>(
                    FindObjectsInactive.Include);
            Require(initialVisualState != null && initialVisualState.RefreshNow(true),
                "Los estados visuales 3D no respondieron al estado inicial.");
            Require(CountActiveNamed("DamageDetails") > 0 &&
                CountActiveNamed("RepairEmitter") == 0,
                "El cubo inicial no muestra dano operativo correctamente.");
            Require(controller.IsVisible && controller.CurrentFaceIndex == 0,
                "El cubo 3D no se activÃ³ automÃ¡ticamente en el flujo normal.");
            SetSelectionGuideVisible(false);
            BeginCaptureRendering(1080, 1920);
            Advance(1);
            return;
        }

        Require(controller != null, "MachineCube3DPrototypeController no disponible.");
        if (stage == 1)
        {
            controller.SetRotationDegrees(0f);
            SetSelectionGuideVisible(false);
            Capture("true3d_01_face_1_rest.png");
            Advance(2);
            return;
        }
        if (stage == 2)
        {
            controller.SetRotationDegrees(-45f);
            SetSelectionGuideVisible(false);
            Capture("true3d_02_connected_faces_45deg.png");
            Advance(3);
            return;
        }
        if (stage == 3)
        {
            controller.SetRotationDegrees(-90f);
            ShowFace(1);
            SetSelectionGuideVisible(false);
            controller.RenderNow();
            Capture("true3d_03_face_2_front_90deg.png");
            ShowFace(0);
            MachineCubeVisualUI visual = UnityEngine.Object.FindFirstObjectByType<MachineCubeVisualUI>(
                FindObjectsInactive.Include);
            Require(visual != null && controller.IsVisible &&
                controller.CurrentFaceIndex == 0,
                "La integraciÃ³n normal no regresÃ³ a Cara 1.");
            visual.RotateBy(1);
            Require(visual.IsRotating && controller.IsRotating,
                "La UI normal no iniciÃ³ el giro fÃ­sico Cara 1 a Cara 2.");
            Advance(4);
            return;
        }
        if (stage == 4)
        {
            MachineCubeVisualUI visual = UnityEngine.Object.FindFirstObjectByType<MachineCubeVisualUI>(
                FindObjectsInactive.Include);
            if (controller.IsRotating || (visual != null && visual.IsRotating))
                return;
            MachinePanelUI panel = UnityEngine.Object.FindFirstObjectByType<MachinePanelUI>(
                FindObjectsInactive.Include);
            Require(visual != null && panel != null && controller.CurrentFaceIndex == 1 &&
                panel.CurrentZone == MachineZoneType.FusionSector,
                "La UI normal no terminÃ³ sincronizada en Cara 2.");
            visual.RotateBy(-1);
            Require(visual.IsRotating && controller.IsRotating,
                "La UI normal no iniciÃ³ el giro fÃ­sico Cara 2 a Cara 1.");
            Advance(5);
            return;
        }
        if (stage == 5)
        {
            MachineCubeVisualUI visual = UnityEngine.Object.FindFirstObjectByType<MachineCubeVisualUI>(
                FindObjectsInactive.Include);
            if (controller.IsRotating || (visual != null && visual.IsRotating))
                return;
            MachinePanelUI panel = UnityEngine.Object.FindFirstObjectByType<MachinePanelUI>(
                FindObjectsInactive.Include);
            Require(panel != null && controller.CurrentFaceIndex == 0 &&
                panel.CurrentZone == MachineZoneType.Room1Link && controller.IsVisible,
                "La UI normal no regresÃ³ sincronizada y visible a Cara 1.");
            PrepareMachine(true);
            ShowFace(0);
            MachineCube3DVisualStateController repairedVisualState =
                UnityEngine.Object.FindFirstObjectByType<MachineCube3DVisualStateController>(
                    FindObjectsInactive.Include);
            Require(repairedVisualState != null,
                "Falta el controlador de estados visuales.");
            repairedVisualState.RefreshNow(true);
            controller.SetFaceImmediate(0);
            SetSelectionGuideVisible(false);
            Require(CountActiveNamed("DamageDetails") == 0 &&
                CountActiveNamed("RepairEmitter") > 0,
                "La reparacion completa no transforma los nodos 3D.");
            Capture("true3d_04_face_1_repaired.png");
            Advance(6);
            return;
        }
        if (stage == 6)
        {
            ShowFace(1);
            MachineCube3DVisualStateController repairedVisualState =
                UnityEngine.Object.FindFirstObjectByType<MachineCube3DVisualStateController>(
                    FindObjectsInactive.Include);
            repairedVisualState?.RefreshNow(true);
            controller.SetFaceImmediate(1);
            SetSelectionGuideVisible(false);
            Capture("true3d_05_face_2_repaired.png");
            Require(TabsUI.Instance != null, "TabsUI no disponible para Ajustes.");
            TabsUI.Instance.ShowAjustes();
            Canvas.ForceUpdateCanvases();
            Advance(7);
            return;
        }
        if (stage == 7)
        {
            VerticalSettingsPanelUI settings =
                UnityEngine.Object.FindFirstObjectByType<VerticalSettingsPanelUI>(
                    FindObjectsInactive.Include);
            Require(settings != null && settings.gameObject.activeInHierarchy,
                "El panel de Ajustes no se abrio para validar calidad 3D.");
            settings.RefreshQualityState();
            Capture("true3d_06_graphics_settings.png");
            EditorApplication.update -= Tick;
            EndCaptureRendering();
            SessionState.SetBool(CompletedKey, true);
            EditorApplication.isPlaying = false;
        }
    }

    private static void ValidateQualityProfiles(
        MachineCube3DPrototypeController controller)
    {
        MachineCube3DQualityLevel previous = MachineCube3DQuality.Current;
        VerticalSettingsPanelUI settings =
            UnityEngine.Object.FindFirstObjectByType<VerticalSettingsPanelUI>(
                FindObjectsInactive.Include);
        Require(settings != null && settings.lowQualityButton != null &&
            settings.balancedQualityButton != null &&
            settings.highQualityButton != null && settings.currentQualityText != null,
            "Los controles de calidad 3D no estan conectados.");

        settings.SetLow();
        Require(controller.CurrentRenderSize == 512 &&
            !HasActiveCubeShadows(controller),
            "El perfil BAJA no aplico 512px y sombras desactivadas.");
        settings.SetBalanced();
        Require(controller.CurrentRenderSize == 768 &&
            !HasActiveCubeShadows(controller),
            "El perfil EQUILIBRADA no aplico 768px y sombras desactivadas.");
        settings.SetHigh();
        Require(controller.CurrentRenderSize == 1024 &&
            HasActiveCubeShadows(controller),
            "El perfil ALTA no aplico 1024px y sombras activas.");
        MachineCube3DQuality.Set(previous);
    }

    private static bool HasActiveCubeShadows(
        MachineCube3DPrototypeController controller)
    {
        return controller.GetComponentsInChildren<MeshRenderer>(true)
            .Any(renderer => renderer.shadowCastingMode !=
                UnityEngine.Rendering.ShadowCastingMode.Off);
    }

    private static int CountActiveNamed(string objectName)
    {
        return Resources.FindObjectsOfTypeAll<GameObject>()
            .Count(gameObject => gameObject.name == objectName &&
                gameObject.scene.IsValid() && gameObject.activeInHierarchy);
    }

    private static void SetSelectionGuideVisible(bool visible)
    {
        MachinePanelUI panel = UnityEngine.Object.FindFirstObjectByType<MachinePanelUI>(
            FindObjectsInactive.Include);
        Transform guide = panel != null
            ? panel.transform.Find("MachineCubeVisualRoot/SelectionGuide")
            : null;
        if (guide != null)
            guide.gameObject.SetActive(visible);
    }

    private static void PrepareMachine(bool repaired)
    {
        Require(GameState.I != null, "GameState no disponible.");
        Require(MachineManager.I != null, "MachineManager no disponible.");
        GameState.I.experimentalChamberUnlocked = true;
        GameState.I.LE = 1e15;
        GameState.I.Traces = 1e12;
        GameState.I.experimentalHallazgos = 9999;
        GameState.I.experimentalMuestras = 9999;
        GameState.I.experimentalLecturasIncompletas = 9999;
        GameState.I.experimentalCompuestosUtiles = 9999;
        GameState.I.chronalPureInstants = 9999;
        GameState.I.chronalStableInstants = 9999;
        GameState.I.chronalForcedInstants = 9999;

        List<string> repairedIds = repaired
            ? MachineManager.I.GetAllNodes(true)
                .Where(node => node != null && !node.hidden)
                .Select(node => node.id).ToList()
            : new List<string>();
        SaveData state = new SaveData
        {
            machineIntroSeen = true,
            machineUnlocked = true,
            machineAllZonesUnlocked = true,
            machineFusionPanelUnlocked = repaired,
            machineSelectedFaceIndex = 0,
            machineRepairedNodeIds = repairedIds,
            machineAnalyzedNodeIds = new List<string>()
        };
        MachineManager.I.LoadProgressFromSave(state);
        TabsUI.Instance?.ShowRoom2();

        MachinePanelUI panel = UnityEngine.Object.FindFirstObjectByType<MachinePanelUI>(
            FindObjectsInactive.Include);
        Require(panel != null, "MachinePanelUI no disponible.");
        ActivateAncestors(panel.transform);
        panel.SelectZoneFromCube(MachineZoneType.Room1Link);
        panel.Refresh();
        HideTransientOverlays();
        Canvas.ForceUpdateCanvases();
    }

    private static void ShowFace(int faceIndex)
    {
        MachinePanelUI panel = UnityEngine.Object.FindFirstObjectByType<MachinePanelUI>(
            FindObjectsInactive.Include);
        Require(panel != null, "MachinePanelUI no disponible.");
        MachineZoneType zone = (MachineZoneType)(faceIndex + 1);
        panel.SelectZoneFromCube(zone);
        MachineNodeDef preferred = MachineManager.I.GetDisplayNodesByZone(zone)
            .FirstOrDefault(node => node != null &&
                (!node.damaged || MachineManager.I.IsNodeRepaired(node.id)));
        if (preferred != null)
            panel.SelectNodeFromCube(preferred.id);
        panel.Refresh();
        HideTransientOverlays();
        Canvas.ForceUpdateCanvases();
    }

    private static void HideTransientOverlays()
    {
        PresentationReturnReportService.Consume();
        PresentationReturnReportUI[] reports =
            UnityEngine.Object.FindObjectsByType<PresentationReturnReportUI>(
                FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (PresentationReturnReportUI report in reports)
        {
            if (report != null)
                report.gameObject.SetActive(false);
        }
    }

    private static void Capture(string filename)
    {
        string path = Path.Combine(OutputDirectory, filename);
        RenderToPng(1080, 1920, path);
        Require(File.Exists(path) && new FileInfo(path).Length > 10000,
            "Captura vacía: " + filename);
    }

    private static void RenderToPng(int width, int height, string path)
    {
        BeginCaptureRendering(width, height);
        RenderTexture previousActive = RenderTexture.active;
        try
        {
            RefreshCaptureGeometry();
            // Batch rendering can upload UI/TMP geometry over several render passes.
            // Warm the target repeatedly before the final evidence readback.
            for (int pass = 0; pass < 4; pass++)
            {
                _captureCamera.Render();
                RefreshCaptureGeometry();
            }
            _captureCamera.Render();
            RenderTexture.active = _captureTarget;
            Texture2D image = new Texture2D(width, height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            image.Apply();
            File.WriteAllBytes(path, image.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(image);
        }
        finally
        {
            RenderTexture.active = previousActive;
        }
    }

    private static void BeginCaptureRendering(int width, int height)
    {
        if (_captureTarget != null && _captureCamera != null)
            return;

        _captureCamera = Camera.main != null
            ? Camera.main
            : UnityEngine.Object.FindFirstObjectByType<Camera>();
        Require(_captureCamera != null, "No camera available for machine capture.");
        _captureCanvases = UnityEngine.Object.FindObjectsByType<Canvas>(
                FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .Where(canvas => canvas != null && canvas.isRootCanvas)
            .OrderBy(canvas => canvas.sortingOrder)
            .ThenBy(canvas => canvas.transform.GetSiblingIndex())
            .ToArray();
        _previousCanvasModes = new RenderMode[_captureCanvases.Length];
        _previousCanvasCameras = new Camera[_captureCanvases.Length];
        _previousPlaneDistances = new float[_captureCanvases.Length];
        _captureTarget = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        _previousCameraTarget = _captureCamera.targetTexture;
        _captureCamera.targetTexture = _captureTarget;

        for (int i = 0; i < _captureCanvases.Length; i++)
        {
            Canvas canvas = _captureCanvases[i];
            _previousCanvasModes[i] = canvas.renderMode;
            _previousCanvasCameras[i] = canvas.worldCamera;
            _previousPlaneDistances[i] = canvas.planeDistance;
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = _captureCamera;
                canvas.planeDistance = 1f + (_captureCanvases.Length - i) * 0.05f;
            }
        }
        RefreshCaptureGeometry();
    }

    private static void RefreshCaptureGeometry()
    {
        Canvas.ForceUpdateCanvases();
        MachineCubeVisualUI[] machineVisuals =
            UnityEngine.Object.FindObjectsByType<MachineCubeVisualUI>(
                FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (MachineCubeVisualUI visual in machineVisuals)
            visual.RefreshNow();
        if (SessionState.GetBool(True3DPrototypeKey, false))
            SetSelectionGuideVisible(false);
        Canvas.ForceUpdateCanvases();
        RectTransform[] rects = UnityEngine.Object.FindObjectsByType<RectTransform>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (RectTransform rect in rects)
            rect.ForceUpdateRectTransforms();
        TMP_Text[] texts = UnityEngine.Object.FindObjectsByType<TMP_Text>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (TMP_Text label in texts)
            label.ForceMeshUpdate(false, true);
        Canvas.ForceUpdateCanvases();
    }

    private static void EndCaptureRendering()
    {
        if (_captureCanvases != null)
        {
            for (int i = 0; i < _captureCanvases.Length; i++)
            {
                Canvas canvas = _captureCanvases[i];
                if (canvas == null)
                    continue;
                canvas.renderMode = _previousCanvasModes[i];
                canvas.worldCamera = _previousCanvasCameras[i];
                canvas.planeDistance = _previousPlaneDistances[i];
            }
        }
        if (_captureCamera != null)
            _captureCamera.targetTexture = _previousCameraTarget;
        if (_captureTarget != null)
        {
            _captureTarget.Release();
            UnityEngine.Object.DestroyImmediate(_captureTarget);
        }
        _captureCamera = null;
        _captureTarget = null;
        _previousCameraTarget = null;
        _captureCanvases = null;
        _previousCanvasModes = null;
        _previousCanvasCameras = null;
        _previousPlaneDistances = null;
    }

    private static void RenderToPngLegacy(int width, int height, string path)
    {
        Camera camera = Camera.main != null
            ? Camera.main
            : UnityEngine.Object.FindFirstObjectByType<Camera>();
        Require(camera != null, "No hay cámara para captura.");

        Canvas[] canvases = UnityEngine.Object.FindObjectsByType<Canvas>(
                FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .Where(canvas => canvas != null && canvas.isRootCanvas)
            .OrderBy(canvas => canvas.sortingOrder)
            .ThenBy(canvas => canvas.transform.GetSiblingIndex())
            .ToArray();
        RenderMode[] modes = new RenderMode[canvases.Length];
        Camera[] cameras = new Camera[canvases.Length];
        float[] planeDistances = new float[canvases.Length];
        RenderTexture target = new RenderTexture(width, height, 24,
            RenderTextureFormat.ARGB32);
        RenderTexture previousTarget = camera.targetTexture;
        RenderTexture previousActive = RenderTexture.active;
        try
        {
            // The camera must know the target dimensions before Overlay canvases
            // become camera-space; otherwise some TMP meshes keep the batch window's
            // stale geometry and appear cropped or displaced in the PNG.
            camera.targetTexture = target;
            for (int i = 0; i < canvases.Length; i++)
            {
                modes[i] = canvases[i].renderMode;
                cameras[i] = canvases[i].worldCamera;
                planeDistances[i] = canvases[i].planeDistance;
                if (modes[i] == RenderMode.ScreenSpaceOverlay)
                {
                    canvases[i].renderMode = RenderMode.ScreenSpaceCamera;
                    canvases[i].worldCamera = camera;
                    // Separate root canvases in depth while retaining their sorting
                    // order. Sharing one plane caused nondeterministic z-fighting in
                    // batch captures (missing text, cards and navigation fragments).
                    canvases[i].planeDistance = 1f + (canvases.Length - i) * 0.05f;
                }
            }
            Canvas.ForceUpdateCanvases();
            RectTransform[] rects = UnityEngine.Object.FindObjectsByType<RectTransform>(
                FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (RectTransform rect in rects)
                rect.ForceUpdateRectTransforms();
            TMP_Text[] texts = UnityEngine.Object.FindObjectsByType<TMP_Text>(
                FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (TMP_Text label in texts)
                label.ForceMeshUpdate(false, true);
            Canvas.ForceUpdateCanvases();
            camera.Render();
            Canvas.ForceUpdateCanvases();
            camera.Render();
            RenderTexture.active = target;
            Texture2D image = new Texture2D(width, height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            image.Apply();
            File.WriteAllBytes(path, image.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(image);
        }
        finally
        {
            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            target.Release();
            UnityEngine.Object.DestroyImmediate(target);
            for (int i = 0; i < canvases.Length; i++)
            {
                canvases[i].renderMode = modes[i];
                canvases[i].worldCamera = cameras[i];
                canvases[i].planeDistance = planeDistances[i];
            }
        }
    }

    private static void ReloadScene()
    {
        Scene active = SceneManager.GetActiveScene();
        Require(active.IsValid(), "No hay escena activa para reiniciar la captura.");
        SceneManager.LoadScene(active.buildIndex);
    }

    private static void BackupUserSave()
    {
        bool saveExists = File.Exists(SavePath);
        bool backupExists = File.Exists(SaveBackupPath);
        SessionState.SetBool(SaveExistedKey, saveExists);
        SessionState.SetBool(BackupExistedKey, backupExists);
        DeleteIfPresent(SessionSaveMissing);
        DeleteIfPresent(SessionBackupMissing);
        if (saveExists) File.Copy(SavePath, SessionSave, true);
        else File.WriteAllText(SessionSaveMissing, string.Empty);
        if (backupExists) File.Copy(SaveBackupPath, SessionBackup, true);
        else File.WriteAllText(SessionBackupMissing, string.Empty);
    }

    private static void RestoreUserSave()
    {
        RestoreOne(SavePath, SessionSave, SessionSaveMissing,
            SessionState.GetBool(SaveExistedKey, false));
        RestoreOne(SaveBackupPath, SessionBackup, SessionBackupMissing,
            SessionState.GetBool(BackupExistedKey, false));
    }

    private static void RestoreOne(string destination, string sessionCopy,
        string missingMarker, bool existed)
    {
        if (existed)
        {
            Require(File.Exists(sessionCopy), "Falta respaldo de sesión: " + sessionCopy);
            File.Copy(sessionCopy, destination, true);
        }
        else if (File.Exists(destination))
        {
            File.Delete(destination);
        }
        if (File.Exists(sessionCopy))
            File.Delete(sessionCopy);
        DeleteIfPresent(missingMarker);
    }

    private static void RecoverStaleBackup()
    {
        RecoverOne(SavePath, SessionSave, SessionSaveMissing);
        RecoverOne(SaveBackupPath, SessionBackup, SessionBackupMissing);
    }

    private static void RecoverOne(string destination, string sessionCopy,
        string missingMarker)
    {
        if (File.Exists(sessionCopy))
            File.Copy(sessionCopy, destination, true);
        else if (File.Exists(missingMarker))
            DeleteIfPresent(destination);
        DeleteIfPresent(sessionCopy);
        DeleteIfPresent(missingMarker);
    }

    private static void DeleteIfPresent(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
    }

    private static void ActivateAncestors(Transform transform)
    {
        for (Transform current = transform; current != null; current = current.parent)
            current.gameObject.SetActive(true);
    }

    private static void Advance(int stage)
    {
        SessionState.SetInt(StageKey, stage);
        SessionState.SetInt(FramesKey, 0);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
#endif

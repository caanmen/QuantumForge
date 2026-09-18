#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class MachineMonolith2DCapture
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ActiveKey = "QF.Monolith2DCapture.Active";
    private const string FramesKey = "QF.Monolith2DCapture.Frames";
    private const string FailedKey = "QF.Monolith2DCapture.Failed";
    private const string CompletedKey = "QF.Monolith2DCapture.Completed";
    private const string SupportOnlyKey = "QF.Monolith2DCapture.SupportOnly";
    private const string SaveExistedKey = "QF.Monolith2DCapture.SaveExisted";
    private const string BackupExistedKey = "QF.Monolith2DCapture.BackupExisted";

    private static Camera captureCamera;
    private static RenderTexture captureTarget;
    private static RenderTexture previousCameraTarget;
    private static Canvas[] captureCanvases;
    private static RenderMode[] previousCanvasModes;
    private static Camera[] previousCanvasCameras;
    private static float[] previousPlaneDistances;
    private static int animationProbeStage;
    private static double animationProbeStartedAt;
    private static float previousOverviewAlpha;
    private static float previousSectorAlpha;
    private static float previousCloseLabAlpha;
    private static Vector2 transitionArtRestPosition;
    private static Vector3 transitionArtRestScale;
    private static Vector2 transitionApproachRestPosition;
    private static Vector3 transitionApproachRestScale;
    private static Vector3 transitionArtBottomCenterWorld;
    private static Vector2 transitionLabRestPosition;
    private static Vector3 transitionLabRestScale;
    private static Vector2 transitionSupportBackRestPosition;
    private static Vector3 transitionSupportBackRestScale;
    private static Vector2 transitionSupportFrontRestPosition;
    private static Vector3 transitionSupportFrontRestScale;
    private static int timedEntryIndex;
    private static double timedEntryStarted;
    private static readonly Dictionary<string, Texture2D> surfaceTextures = new();
    private static readonly HashSet<string> surfaceCases = new();

    private static readonly float[] TransitionCheckpoints =
        { .10f, .20f, .25f, .50f, .55f, .62f, .75f };
    private const float SolidSurfaceAlphaThreshold = .98f;

    private static string OutputDirectory =>
        Path.GetFullPath("Logs/VisualQA/MachineMonolith2D_V59");
    private static string SavePath =>
        Path.Combine(Application.persistentDataPath, "save.json");
    private static string SaveBackupPath => SavePath + ".bak";
    private static string SessionSave =>
        Path.Combine(OutputDirectory, ".save.session-backup");
    private static string SessionBackup =>
        Path.Combine(OutputDirectory, ".save.bak.session-backup");

    [InitializeOnLoadMethod]
    private static void ResumeAfterReload()
    {
        if (!SessionState.GetBool(ActiveKey, false))
            return;
        SaveService.SuppressWritesForVisualQa = true;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        if (EditorApplication.isPlaying)
        {
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
        }
    }

    [MenuItem("Tools/Quantum Forge/Machine/Capture Monolith 2D Evidence")]
    public static void Run()
    {
        RunInternal(false);
    }

    [MenuItem("Tools/Quantum Forge/Machine/Capture Approved V39 Overview Pedestal Evidence")]
    public static void RunOverviewSupport()
    {
        RunInternal(true);
    }

    private static void RunInternal(bool supportOnly, bool rebuild = true)
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            throw new InvalidOperationException("La captura debe comenzar fuera de Play Mode.");

        Directory.CreateDirectory(OutputDirectory);
        foreach (string path in Directory.GetFiles(OutputDirectory, "*.png"))
            File.Delete(path);
        BackupUserSave();
        SaveService.SuppressWritesForVisualQa = true;

        try
        {
            if (supportOnly && rebuild)
                MachineMonolith2DSetup.ConfigureOverviewSupportBatch();
            else if (rebuild)
                MachineMonolith2DSetup.ConfigureBatch();
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            SessionState.SetBool(ActiveKey, true);
            SessionState.SetBool(SupportOnlyKey, supportOnly);
            SessionState.SetBool(FailedKey, false);
            SessionState.SetBool(CompletedKey, false);
            SessionState.SetInt(FramesKey, 0);
            animationProbeStage = 0;
            animationProbeStartedAt = 0d;
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            EditorApplication.isPlaying = true;
        }
        catch
        {
            SessionState.SetBool(ActiveKey, false);
            SessionState.SetBool(SupportOnlyKey, false);
            RestoreUserSave();
            throw;
        }
    }

    public static void RunBatch()
    {
        Run();
    }

    public static void RunCurrentSceneBatch()
    {
        // Layout coordinates are owned by runtime, so iteration needs no scene rebuild.
        RunInternal(false, false);
    }

    public static void RunTransitionCurrentSceneBatch()
    {
        // Targeted evidence for the overview-to-face motion, without rebuilding
        // the approved scene or recapturing every repair surface.
        RunInternal(true, false);
    }

    public static void RunOverviewSupportBatch()
    {
        RunOverviewSupport();
    }

    private static void OnPlayModeChanged(PlayModeStateChange change)
    {
        if (change == PlayModeStateChange.EnteredPlayMode)
        {
            SaveService.SuppressWritesForVisualQa = true;
            surfaceCases.Clear();
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
            return;
        }
        if (change != PlayModeStateChange.EnteredEditMode)
            return;

        EditorApplication.update -= Tick;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EndCaptureRendering();
        bool supportOnly = SessionState.GetBool(SupportOnlyKey, false);
        bool failed = SessionState.GetBool(FailedKey, false) ||
            !SessionState.GetBool(CompletedKey, false);
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
            SessionState.SetBool(ActiveKey, false);
            SessionState.SetBool(FailedKey, false);
            SessionState.SetBool(SupportOnlyKey, false);
        }

        if (!failed)
        {
            if (supportOnly)
                Debug.Log("[Machine Monolith 2D Capture] V59 TRANSITION PASS | pedestal preserved | no pre-zoom | canonical overview + close lab synchronized with the selected face | complementary alpha without empty frames | checkpoints 0/10/20/25/50/55/62/75/100 | 1080x1920 + 720x1280 | save restored");
            else
                Debug.Log("[Machine Monolith 2D Capture] 8-8-8 PROGRESSIVE-SURFACE PASS | 3/5/7/8 signatures per face across 4 global repair stages x 2 resolutions | faces 3 and 4 use dedicated flat-top lower geometry | transformed full symbol bounds + 4 local px clearance inside stone and current tile alpha | 32 px symbols + 80 px touch targets | navigation and transition checked | 1080x1920 + 720x1280 | save restored | visual review pending");
        }
        if (Application.isBatchMode)
            EditorApplication.Exit(failed ? 1 : 0);
    }

    private static void Tick()
    {
        if (animationProbeStage > 0)
        {
            TickAnimationProbe();
            return;
        }

        int frames = SessionState.GetInt(FramesKey, 0) + 1;
        SessionState.SetInt(FramesKey, frames);
        if (frames < 30)
            return;

        try
        {
            if (SessionState.GetBool(SupportOnlyKey, false))
                CaptureOverviewSupportStates();
            else
            {
                CaptureAllStates();
                Require(surfaceCases.Count == 24,
                    "Faltan casos de superficie: se requieren 3 caras x 4 etapas x 2 resoluciones.");
                WriteFaceReviewSheets();
            }
            StartAnimationProbe();
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

    private static void StartAnimationProbe()
    {
        PrepareMachine(false);
        MachineMonolith2DVisualUI visual = FindVisual();
        visual.ShowOverviewImmediate();
        ForceUiGeometry();
        Button sector = visual.GetComponentsInChildren<Button>(true)
            .FirstOrDefault(candidate => candidate.name == "SectorTouch_1");
        Require(sector != null && sector.interactable,
            "La cara 1 no está disponible para probar el acercamiento animado.");
        RectTransform art = visual.GetComponentsInChildren<RectTransform>(true)
            .FirstOrDefault(candidate => candidate.name == "OverviewArtwork");
        RectTransform lab = visual.GetComponentsInChildren<RectTransform>(true)
            .FirstOrDefault(candidate => candidate.name == "HumanLabOverviewBackground");
        RectTransform supportBack = visual.GetComponentsInChildren<RectTransform>(true)
            .FirstOrDefault(candidate => candidate.name == "OverviewFittedSupportBack");
        RectTransform supportFront = visual.GetComponentsInChildren<RectTransform>(true)
            .FirstOrDefault(candidate => candidate.name == "OverviewFittedSupportFront");
        CanvasGroup overviewGroup = visual.GetComponentsInChildren<CanvasGroup>(true)
            .FirstOrDefault(candidate => candidate.name == "MonolithOverview");
        RectTransform approach = overviewGroup != null
            ? overviewGroup.transform as RectTransform : null;
        Require(art != null && lab != null && supportBack != null &&
            supportFront != null && approach != null,
            "Faltan capas para registrar el reposo de la transición.");
        transitionArtRestPosition = art.anchoredPosition;
        transitionArtRestScale = art.localScale;
        transitionApproachRestPosition = approach.anchoredPosition;
        transitionApproachRestScale = approach.localScale;
        transitionArtBottomCenterWorld = GetBottomCenterWorld(art);
        transitionLabRestPosition = lab.anchoredPosition;
        transitionLabRestScale = lab.localScale;
        transitionSupportBackRestPosition = supportBack.anchoredPosition;
        transitionSupportBackRestScale = supportBack.localScale;
        transitionSupportFrontRestPosition = supportFront.anchoredPosition;
        transitionSupportFrontRestScale = supportFront.localScale;
        CapturePair("12_transition_v59_00");
        previousOverviewAlpha = 1f;
        previousSectorAlpha = 0f;
        previousCloseLabAlpha = 0f;
        visual.EditorTransitionProgressOverride = TransitionCheckpoints[0];
        sector.onClick.Invoke();
        animationProbeStage = 1;
        animationProbeStartedAt = EditorApplication.timeSinceStartup;
    }

    private static void TickAnimationProbe()
    {
        try
        {
            if (animationProbeStage == 100)
            {
                TickTimedEntry();
                return;
            }
            double elapsed = EditorApplication.timeSinceStartup - animationProbeStartedAt;
            MachineMonolith2DVisualUI visual = FindVisual();
            CanvasGroup veil = visual.GetComponentsInChildren<CanvasGroup>(true)
                .FirstOrDefault(candidate => candidate.name == "AlienTransitionVeil");
            RectTransform art = visual.GetComponentsInChildren<RectTransform>(true)
                .FirstOrDefault(candidate => candidate.name == "OverviewArtwork");
            RectTransform lab = visual.GetComponentsInChildren<RectTransform>(true)
                .FirstOrDefault(candidate => candidate.name == "HumanLabOverviewBackground");
            RectTransform supportBack = visual.GetComponentsInChildren<RectTransform>(true)
                .FirstOrDefault(candidate => candidate.name == "OverviewFittedSupportBack");
            RectTransform supportFront = visual.GetComponentsInChildren<RectTransform>(true)
                .FirstOrDefault(candidate => candidate.name == "OverviewFittedSupportFront");
            CanvasGroup overview = visual.GetComponentsInChildren<CanvasGroup>(true)
                .FirstOrDefault(candidate => candidate.name == "MonolithOverview");
            CanvasGroup closeLabGroup = visual.GetComponentsInChildren<CanvasGroup>(true)
                .FirstOrDefault(candidate => candidate.name == "HumanLabCloseBackground");
            CanvasGroup sectorGroup = visual.GetComponentsInChildren<CanvasGroup>(true)
                .FirstOrDefault(candidate => candidate.name == "MonolithSectorCloseup");
            CanvasGroup overviewLabGroup = visual.GetComponentsInChildren<CanvasGroup>(true)
                .FirstOrDefault(candidate => candidate.name == "HumanLabOverviewBackground");

            if (animationProbeStage >= 1 && animationProbeStage <=
                TransitionCheckpoints.Length)
            {
                float checkpoint = TransitionCheckpoints[animationProbeStage - 1];
                if (visual.TransitionProgress >= checkpoint)
                {
                    CapturePair($"{12 + animationProbeStage:00}_transition_v59_{checkpoint * 100f:00}");
                    ValidateTransitionFrame(visual, art, lab, supportBack, supportFront, overview,
                        sectorGroup, overviewLabGroup, closeLabGroup, veil,
                        checkpoint);
                    animationProbeStage++;
                    visual.EditorTransitionProgressOverride =
                        animationProbeStage <= TransitionCheckpoints.Length
                            ? TransitionCheckpoints[animationProbeStage - 1] : 1f;
                    return;
                }
            }

            if (animationProbeStage == TransitionCheckpoints.Length + 1 &&
                visual.IsShowingSector && visual.TransitionProgress >= .999f)
            {
                GameObject closeLab = closeLabGroup != null
                    ? closeLabGroup.gameObject : null;
                GameObject overviewLab = overviewLabGroup != null
                    ? overviewLabGroup.gameObject : null;
                GameObject supportBackObject = supportBack != null
                    ? supportBack.gameObject : null;
                GameObject supportFrontObject = supportFront != null
                    ? supportFront.gameObject : null;
                Require(veil != null && veil.alpha < .01f,
                    "El velo no terminó totalmente retirado.");
                Require(overview != null && !overview.gameObject.activeInHierarchy,
                    "La vista general siguió activa al terminar la entrada.");
                Require(sectorGroup != null && sectorGroup.gameObject.activeInHierarchy &&
                    sectorGroup.alpha > .99f,
                    "La cara cercana no quedó estable al terminar la entrada.");
                Require(closeLab != null && closeLab.activeInHierarchy &&
                    overviewLab != null && !overviewLab.activeInHierarchy &&
                    supportBackObject != null && !supportBackObject.activeInHierarchy &&
                    supportFrontObject != null && !supportFrontObject.activeInHierarchy,
                    "El estado final no dejó exclusivamente el fondo cercano V27.");
                CapturePair("20_transition_v59_100");
                visual.EditorTransitionProgressOverride = null;
                timedEntryIndex = 0;
                animationProbeStage = 100;
                BeginTimedEntry();
                return;
            }

            // Capturing both validation resolutions at every checkpoint can exceed three
            // seconds on slower machines even though the one-second transition completed.
            if (elapsed > 8d)
                throw new InvalidOperationException(
                    "La transición no completó sus controles 0/10/20/25/50/55/62/75/100.");
        }
        catch (Exception exception)
        {
            SessionState.SetBool(FailedKey, true);
            animationProbeStage = 0;
            Debug.LogException(exception);
            EditorApplication.update -= Tick;
            EndCaptureRendering();
            EditorApplication.isPlaying = false;
        }
    }

    private static void BeginTimedEntry()
    {
        MachineMonolith2DVisualUI visual = FindVisual();
        visual.ShowOverviewImmediate();
        ForceUiGeometry();
        int sectorIndex = timedEntryIndex % 4;
        Button button = visual.GetComponentsInChildren<Button>(true)
            .First(candidate => candidate.name == "SectorTouch_" + (sectorIndex + 1));
        timedEntryStarted = EditorApplication.timeSinceStartup;
        button.onClick.Invoke();
    }

    private static void TickTimedEntry()
    {
        MachineMonolith2DVisualUI visual = FindVisual();
        double elapsed = EditorApplication.timeSinceStartup - timedEntryStarted;
        if (visual.IsShowingSector)
        {
            Require(elapsed >= MachineMonolith2DVisualUI.SectorTransitionDuration &&
                elapsed < MachineMonolith2DVisualUI.SectorTransitionDuration + .5d,
                "La entrada se alarga bajo fotogramas espaciados: " + elapsed);
            Debug.Log($"[Monolith V54] TIMING PASS | face={timedEntryIndex % 4 + 1} | " +
                $"frame delay={(timedEntryIndex < 4 ? 33 : 67)}ms | wall={elapsed:F3}s");
            if (++timedEntryIndex < 8)
                BeginTimedEntry();
            else
            {
                SessionState.SetBool(CompletedKey, true);
                animationProbeStage = 0;
                EditorApplication.update -= Tick;
                EndCaptureRendering();
                EditorApplication.isPlaying = false;
            }
            return;
        }
        Require(elapsed < 2d, "La transición no finaliza.");
        // Simulate a constrained frame cadence, without injecting the capture clock.
        System.Threading.Thread.Sleep(timedEntryIndex < 4 ? 33 : 67);
    }

    private static void CaptureOverviewSupportStates()
    {
        PrepareMachine(false);
        MachineMonolith2DVisualUI visual = FindVisual();
        visual.ShowOverviewImmediate();
        ForceUiGeometry();
        Transform viewport = visual.transform.Find(
            "MachineMonolith2DRoot/MachinePrimaryContent/MonolithViewport");
        RectTransform overview = viewport?.Find(
            "MonolithOverview/OverviewArtwork") as RectTransform;
        GameObject supportBack = viewport?.Find("OverviewFittedSupportBack")?.gameObject;
        GameObject supportFront = viewport?.Find("OverviewFittedSupportFront")?.gameObject;
        Require(overview != null && supportBack != null && supportFront != null,
            "Faltan las tres capas de la composición V39.");
        Require(Mathf.Abs(overview.localScale.x - .72f) < .01f &&
                Vector2.Distance(overview.anchoredPosition,
                    new Vector2(0f, 34f)) < 1f,
            "La captura V39 no conserva escala 0,72 y posición (0,34).");
        Require(UsesFootprintStage(supportBack, 0f) &&
                UsesFootprintStage(supportFront, 0f),
            "El pedestal inicial V39 no usa la etapa 0.");
        CapturePair("01_overview_initial_v39");

        PrepareMachine(true);
        visual = FindVisual();
        visual.ShowOverviewImmediate();
        ForceUiGeometry();
        viewport = visual.transform.Find(
            "MachineMonolith2DRoot/MachinePrimaryContent/MonolithViewport");
        supportBack = viewport?.Find("OverviewFittedSupportBack")?.gameObject;
        supportFront = viewport?.Find("OverviewFittedSupportFront")?.gameObject;
        Require(UsesFootprintStage(supportBack, 3f) &&
                UsesFootprintStage(supportFront, 3f),
            "El pedestal reparado V39 no usa la etapa 3.");
        CapturePair("02_overview_repaired_v39");
    }

    private static void CaptureAllStates()
    {
        PrepareMachine(false);
        MachineMonolith2DVisualUI visual = FindVisual();
        visual.ShowOverviewImmediate();
        Transform overviewArtwork = visual.transform.Find(
            "MachineMonolith2DRoot/MachinePrimaryContent/MonolithViewport/MonolithOverview/OverviewArtwork");
        Require(overviewArtwork != null,
            "No se encontró el arte de la vista general.");
        RectTransform overviewRect = overviewArtwork as RectTransform;
        Require(overviewRect != null &&
            Mathf.Abs(overviewRect.localScale.x - .72f) < .01f &&
            Vector2.Distance(overviewRect.anchoredPosition,
                new Vector2(0f, 34f)) < 1f,
            "La vista general no conserva el tamaño y apoyo V26 bloqueados.");
        Require(Enumerable.Range(1, 4).All(index =>
                overviewArtwork.Find("FaceEntryLight_" + index)?.gameObject.activeInHierarchy == true),
            "La vista general debe mostrar cuatro luces de entrada.");
        for (int index = 0; index < 4; index++)
        {
            RectTransform light = overviewArtwork.Find(
                "FaceEntryLight_" + (index + 1)) as RectTransform;
            Vector2 expected =
                MachineMonolith2DVisualUI.GetOverviewEntryLightAnchor(index, 0);
            Require(light != null && light.parent == overviewArtwork &&
                Vector2.Distance(light.anchorMin, expected) < .002f &&
                Vector2.Distance(light.anchorMax, expected) < .002f &&
                light.anchoredPosition.sqrMagnitude < .01f &&
                Vector3.Distance(light.localScale, Vector3.one) < .001f,
                "La luz de entrada " + (index + 1) +
                " no está integrada en la hendidura canónica del daño.");
        }
        Require(overviewArtwork.Find("ReservedFaceMarker") == null,
            "El ??? debe existir sólo dentro de la cuarta cara.");
        Require(overviewArtwork.GetComponentsInChildren<RawImage>(true)
                .All(image => !image.name.Contains("AlienSignature")),
            "La vista general no debe mostrar firmas de nodo.");
        Transform viewport = visual.transform.Find(
            "MachineMonolith2DRoot/MachinePrimaryContent/MonolithViewport");
        GameObject overviewLab = viewport?.Find("HumanLabOverviewBackground")?.gameObject;
        GameObject closeLab = viewport?.Find("HumanLabCloseBackground")?.gameObject;
        GameObject supportBackObject = viewport?.Find(
            "OverviewFittedSupportBack")?.gameObject;
        GameObject supportFrontObject = viewport?.Find(
            "OverviewFittedSupportFront")?.gameObject;
        Require(overviewLab != null && closeLab != null &&
            supportBackObject != null && supportFrontObject != null &&
            overviewLab.activeInHierarchy && supportBackObject.activeInHierarchy &&
            supportFrontObject.activeInHierarchy && !closeLab.activeInHierarchy,
            "La vista general debe usar el soporte V35 derivado de la huella del Monolito.");
        Require(UsesFootprintStage(supportBackObject, 0f) &&
                UsesFootprintStage(supportFrontObject, 0f),
            "El receptáculo inicial no deriva la huella del estado inicial visible.");
        CapturePair("01_overview_initial");

        Texture canonicalSymbolTexture = null;
        for (int sector = 0; sector < 3; sector++)
        {
            visual.ShowSectorImmediate(sector);
            Require(closeLab.activeInHierarchy && !overviewLab.activeInHierarchy &&
                !supportBackObject.activeInHierarchy &&
                !supportFrontObject.activeInHierarchy,
                "Cada sector funcional debe usar el fondo cercano canónico V27.");
            RectTransform functionalArt = viewport.Find(
                "MonolithSectorCloseup/SectorArtwork") as RectTransform;
            float expectedFloorOffset = viewport.GetComponent<RectTransform>().rect.height *
                MachineMonolith2DVisualUI.SectorFloorAlignmentRatio;
            float expectedWidthScale =
                MachineMonolith2DVisualUI.GetSectorCloseWidthScale(sector);
            float expectedHorizontalOffset = functionalArt != null
                ? functionalArt.rect.width *
                    expectedWidthScale *
                    MachineMonolith2DVisualUI.GetSectorStageHorizontalOffset(sector, 0)
                : 0f;
            Require(functionalArt != null &&
                Mathf.Abs(functionalArt.localScale.x -
                    expectedWidthScale) < .01f &&
                Mathf.Abs(functionalArt.localScale.y -
                    MachineMonolith2DVisualUI.SectorCloseScale) < .01f &&
                Mathf.Abs(functionalArt.anchoredPosition.x -
                    expectedHorizontalOffset) < 1f &&
                Mathf.Abs(functionalArt.anchoredPosition.y - expectedFloorOffset) < 1f,
                "Las caras funcionales no quedan apoyadas dentro del marco del suelo.");
            Button firstVisibleNode = visual.GetComponentsInChildren<Button>(true)
                .FirstOrDefault(button => button.name == "NodeTouch_01" &&
                    button.gameObject.activeInHierarchy);
            Require(firstVisibleNode != null,
                "No existe un primer nodo visible para demostrar la selección de la cara.");
            firstVisibleNode.onClick.Invoke();
            visual.RefreshNow();
            ForceUiGeometry();
            RawImage[] visibleSymbols = visual.GetComponentsInChildren<RawImage>(true)
                .Where(image => image.name == "AlienSignature" &&
                    image.gameObject.activeInHierarchy)
                .ToArray();
            int expectedSymbols =
                MachineMonolith2DVisualUI.GetFaceVisibleNodeCount(sector, 0);
            Require(visibleSymbols.Length == expectedSymbols,
                "Cantidad incorrecta de firmas visibles en la cara " + (sector + 1));
            if (sector == 0 && visibleSymbols.Length > 0)
                canonicalSymbolTexture = visibleSymbols[0].texture;
            Require(canonicalSymbolTexture != null &&
                    visibleSymbols.All(symbol => symbol.texture == canonicalSymbolTexture),
                "S1, S2 y S3 no reutilizan la misma familia canónica de firmas ramificadas.");
            Require(visibleSymbols.All(symbol =>
                    Mathf.Abs(symbol.rectTransform.rect.width -
                        MachineMonolith2DVisualUI.NodeSymbolDisplaySize) < 1f &&
                    Mathf.Abs(symbol.uvRect.width -
                        MachineMonolith2DVisualUI.NodeSymbolUvWidth * .5f) < .001f &&
                    Mathf.Abs(symbol.uvRect.height -
                        MachineMonolith2DVisualUI.NodeSymbolUvHeight * .5f) < .001f &&
                    symbol.color.a >= MachineMonolith2DVisualUI.DamagedNodeAlpha - .01f &&
                    symbol.color.a <= MachineMonolith2DVisualUI.SelectedDamagedNodeAlpha + .01f),
                "Las firmas dañadas no respetan el encuadre ampliado y contraste V49.");
            for (int symbolIndex = 0; symbolIndex < visibleSymbols.Length; symbolIndex++)
            {
                bool usesCanonicalCrop = Enumerable.Range(0, 7).Any(canonicalIndex =>
                    RectApproximately(visibleSymbols[symbolIndex].uvRect,
                        MachineMonolith2DVisualUI.GetCanonicalSymbolUvRect(
                            0, canonicalIndex)));
                Require(usesCanonicalCrop,
                    "La firma " + (symbolIndex + 1) + " del sector " + (sector + 1) +
                    " no recorta uno de los símbolos canónicos de la cara superior izquierda.");
            }
            Require(visibleSymbols.Any(symbol =>
                    Mathf.Abs(symbol.color.a -
                        MachineMonolith2DVisualUI.DamagedNodeAlpha) < .01f) &&
                visibleSymbols.Any(symbol =>
                    Mathf.Abs(symbol.color.a -
                        MachineMonolith2DVisualUI.SelectedDamagedNodeAlpha) < .01f),
                "La captura dañada debe demostrar luz tenue y selección intermedia.");
            if (sector == 0)
            {
                MachinePanelUI costPanel = UnityEngine.Object.FindFirstObjectByType<
                    MachinePanelUI>(FindObjectsInactive.Include);
                MachineNodeDef affordableHintNode = MachineManager.I
                    .GetDisplayNodesByZone(
                        MachineMonolith2DVisualUI.GetContentZoneForFace(sector))
                    .FirstOrDefault(node => node != null && !node.damaged &&
                        node.cost != null &&
                        (node.cost.le > 0.0 || node.cost.traces > 0.0 ||
                         node.cost.hallazgo > 0 || node.cost.muestra > 0 ||
                         node.cost.lecturaIncompleta > 0 ||
                         node.cost.compuestoUtil > 0));
                Require(costPanel != null && affordableHintNode != null,
                    "No existe un nodo visible para validar el coste faltante.");
                double savedLe = GameState.I.LE;
                double savedTraces = GameState.I.Traces;
                int savedFindings = GameState.I.experimentalHallazgos;
                int savedSamples = GameState.I.experimentalMuestras;
                int savedReadings = GameState.I.experimentalLecturasIncompletas;
                int savedCompounds = GameState.I.experimentalCompuestosUtiles;
                GameState.I.LE = 0.0;
                GameState.I.Traces = 0.0;
                GameState.I.experimentalHallazgos = 0;
                GameState.I.experimentalMuestras = 0;
                GameState.I.experimentalLecturasIncompletas = 0;
                GameState.I.experimentalCompuestosUtiles = 0;
                costPanel.SelectNodeFromCube(affordableHintNode.id);
                visual.RefreshNow();
                ForceUiGeometry();
                TMP_Text costText = visual.GetComponentsInChildren<TMP_Text>(true)
                    .FirstOrDefault(text => text.name == "NodeCost");
                Require(costText != null &&
                        costText.text.Contains("<color=#FF6B6B>"),
                    "Los recursos insuficientes del nodo no se muestran en rojo.");
                GameState.I.LE = savedLe;
                GameState.I.Traces = savedTraces;
                GameState.I.experimentalHallazgos = savedFindings;
                GameState.I.experimentalMuestras = savedSamples;
                GameState.I.experimentalLecturasIncompletas = savedReadings;
                GameState.I.experimentalCompuestosUtiles = savedCompounds;
                visual.RefreshNow();
            }
            List<string> unsafeFootprints = new List<string>();
            for (int symbolIndex = 0; symbolIndex < visibleSymbols.Length; symbolIndex++)
            {
                RawImage symbol = visibleSymbols[symbolIndex];
                RectTransform hit = symbol.transform.parent as RectTransform;
                RectTransform art = hit != null ? hit.parent as RectTransform : null;
                if (hit == null || art == null || art.rect.width <= 0f ||
                    art.rect.height <= 0f)
                {
                    unsafeFootprints.Add($"#{symbolIndex + 1}: jerarquía o rectángulo inválido");
                    continue;
                }
                Vector2 halfExtent = new Vector2(
                    symbol.rectTransform.rect.width * .5f / art.rect.width,
                    symbol.rectTransform.rect.height * .5f / art.rect.height);
                if (!MachineMonolith2DVisualUI.IsNodeFootprintInsideDarkSurface(
                    sector, hit.anchorMin, halfExtent))
                {
                    unsafeFootprints.Add($"#{symbolIndex + 1}: centro " +
                        $"({hit.anchorMin.x:F3},{hit.anchorMin.y:F3}), mitad " +
                        $"({halfExtent.x:F3},{halfExtent.y:F3})");
                }
            }
            Require(unsafeFootprints.Count == 0,
                "Una firma del sector " + (sector + 1) +
                " toca daño, bisel, riel, base o esquina con parte de su superficie: " +
                string.Join("; ", unsafeFootprints));
            Vector2[] symbolCenters = visibleSymbols
                .Select(symbol => ((RectTransform)symbol.transform.parent).anchorMin)
                .ToArray();
            Require(MachineMonolith2DVisualUI.AreNodePlacementsIrregular(symbolCenters),
                "Las firmas del sector " + (sector + 1) +
                " forman filas/columnas o no respetan la separación irregular V49.");
            CapturePair($"{sector + 2:00}_sector_{sector + 1}_initial");
        }

        visual.ShowOverviewImmediate();
        Button reserved = visual.GetComponentsInChildren<Button>(true)
            .FirstOrDefault(candidate => candidate.name == "SectorTouch_4");
        Require(reserved != null && reserved.interactable,
            "La cuarta cara reservada no está disponible para entrar.");
        visual.ShowSectorImmediate(3);
        Require(closeLab.activeInHierarchy && !overviewLab.activeInHierarchy,
            "La cuarta cara debe compartir el mismo fondo cercano físico.");
        Require(visual.GetComponentsInChildren<Button>(true)
                .Where(candidate => candidate.name.StartsWith("NodeTouch_"))
                .All(candidate => !candidate.gameObject.activeInHierarchy),
            "La cuarta cara no debe mostrar nodos táctiles.");
        TMP_Text[] activeReservedMarkers = visual.GetComponentsInChildren<TMP_Text>(true)
            .Where(candidate => candidate.gameObject.activeInHierarchy &&
                candidate.text == "???" && candidate.name == "ReservedFaceMarker")
            .ToArray();
        Require(activeReservedMarkers.Length == 1 &&
                activeReservedMarkers[0].transform.parent.name == "SectorArtwork",
            "Debe existir un único ??? como capa separada dentro de la cuarta cara.");
        RectTransform reservedArt = viewport.Find(
            "MonolithSectorCloseup/SectorArtwork") as RectTransform;
        float expectedReservedFloorOffset = viewport.GetComponent<RectTransform>().rect.height *
            MachineMonolith2DVisualUI.SectorFloorAlignmentRatio;
        float expectedReservedHorizontalOffset = reservedArt != null
            ? reservedArt.rect.width *
                MachineMonolith2DVisualUI.GetSectorCloseWidthScale(3) *
                MachineMonolith2DVisualUI.GetSectorStageHorizontalOffset(3, 0)
            : 0f;
        Require(reservedArt != null &&
            Mathf.Abs(reservedArt.localScale.x -
                MachineMonolith2DVisualUI.GetSectorCloseWidthScale(3)) < .01f &&
            Mathf.Abs(reservedArt.localScale.y -
                MachineMonolith2DVisualUI.SectorCloseScale) < .01f &&
            Mathf.Abs(reservedArt.anchoredPosition.x -
                expectedReservedHorizontalOffset) < 1f &&
            Mathf.Abs(reservedArt.anchoredPosition.y - expectedReservedFloorOffset) < 1f,
            "La cuarta cara no queda apoyada dentro del marco del suelo.");
        CapturePair("05_sector_4_reserved");

        ValidateRepairStageBoundaries();

        // Exercise the first attainable state at every repair threshold, not a
        // texture override. With 36 public nodes those states are 8/15/22.
        for (int stage = 1; stage <= 3; stage++)
        {
            PrepareMachine(false, stage);
            visual = FindVisual();
            for (int sector = 0; sector < 3; sector++)
            {
                visual.ShowSectorImmediate(sector);
                RawImage partialArt = visual.GetComponentsInChildren<RawImage>(true)
                    .First(image => image.name == "SectorArtwork");
                Rect expectedTile = stage switch
                {
                    1 => new Rect(.5f, .5f, .5f, .5f),
                    2 => new Rect(0f, 0f, .5f, .5f),
                    _ => new Rect(.5f, 0f, .5f, .5f)
                };
                Require(RectApproximately(partialArt.uvRect, expectedTile),
                    $"La preparación QA no alcanzó la etapa {stage} de cara {sector + 1}.");
                int expectedSymbols =
                    MachineMonolith2DVisualUI.GetFaceVisibleNodeCount(sector, stage);
                int actualSymbols = visual.GetComponentsInChildren<RawImage>(true)
                    .Count(image => image.name == "AlienSignature" &&
                        image.gameObject.activeInHierarchy);
                Require(actualSymbols == expectedSymbols,
                    $"La etapa {stage} de la cara {sector + 1} debe mostrar " +
                    $"{expectedSymbols} firmas y muestra {actualSymbols}.");
                CapturePair($"stage_{stage}_sector_{sector + 1}_partial");
            }
        }

        PrepareMachine(true);
        visual = FindVisual();
        visual.ShowOverviewImmediate();
        viewport = visual.transform.Find(
            "MachineMonolith2DRoot/MachinePrimaryContent/MonolithViewport");
        supportBackObject = viewport?.Find("OverviewFittedSupportBack")?.gameObject;
        supportFrontObject = viewport?.Find("OverviewFittedSupportFront")?.gameObject;
        Require(UsesFootprintStage(supportBackObject, 3f) &&
                UsesFootprintStage(supportFrontObject, 3f),
            "El receptáculo reparado no deriva la huella del estado reparado visible.");
        CapturePair("06_overview_repaired");
        for (int sector = 0; sector < 3; sector++)
        {
            visual.ShowSectorImmediate(sector);
            RectTransform repairedArt = viewport.Find(
                "MonolithSectorCloseup/SectorArtwork") as RectTransform;
            float expectedRepairedHorizontalOffset = repairedArt != null
                ? repairedArt.rect.width *
                    MachineMonolith2DVisualUI.GetSectorCloseWidthScale(sector) *
                    MachineMonolith2DVisualUI.GetSectorStageHorizontalOffset(sector, 3)
                : 0f;
            Require(repairedArt != null && Mathf.Abs(
                    repairedArt.anchoredPosition.x -
                    expectedRepairedHorizontalOffset) < 1f,
                "La silueta reparada del sector " + (sector + 1) +
                " no conserva su centro perceptual.");
            RawImage[] repairedSymbols = visual.GetComponentsInChildren<RawImage>(true)
                .Where(image => image.name == "AlienSignature" &&
                    image.gameObject.activeInHierarchy)
                .ToArray();
            string[] invalidRepairedSymbols = repairedSymbols
                .Where(image => Mathf.Abs(image.color.a -
                        MachineMonolith2DVisualUI.RepairedNodeAlpha) >= .01f ||
                    Mathf.Abs(image.rectTransform.rect.width -
                        MachineMonolith2DVisualUI.RepairedNodeDisplaySize) >= 1f ||
                    image.color.g <= image.color.r)
                .Select((image, index) => $"#{index + 1} " +
                    $"a={image.color.a:F2} w={image.rectTransform.rect.width:F1} " +
                    $"rgb=({image.color.r:F2},{image.color.g:F2},{image.color.b:F2})")
                .ToArray();
            Require(repairedSymbols.Length ==
                    MachineMonolith2DVisualUI.GetFaceDisplayNodeCount(sector) &&
                    invalidRepairedSymbols.Length == 0,
                "Las firmas reparadas del sector " + (sector + 1) +
                " no alcanzan el brillo y realce completos: " +
                string.Join("; ", invalidRepairedSymbols));
            CapturePair($"{sector + 7:00}_sector_{sector + 1}_repaired");
        }

        visual.ShowOverviewImmediate();
        MachinePanelUI panel = UnityEngine.Object.FindFirstObjectByType<MachinePanelUI>(
            FindObjectsInactive.Include);
        Require(panel != null, "MachinePanelUI no disponible para probar Mezclas.");
        Button mixes = panel.transform.Find(
            "MachineMonolith2DRoot/MachineContextTabs/MixesTab")?.GetComponent<Button>();
        Require(mixes != null && mixes.interactable,
            "La pestaña Mezclas no está disponible para el recorrido físico.");
        mixes.onClick.Invoke();
        ForceUiGeometry();
        Require(panel.FusionPanelVisible,
            "La pestaña Mezclas no abrió el panel de fusión.");
        Require(panel.transform.Find("MachineMonolith2DRoot/MachineHeader")?.gameObject.activeInHierarchy == true,
            "La cabecera de recursos desapareció al entrar a Mezclas.");
        CapturePair("10_fusion_with_shared_header");

        Room2PanelUI room = UnityEngine.Object.FindFirstObjectByType<Room2PanelUI>(
            FindObjectsInactive.Include);
        Button back = room != null
            ? room.transform.Find("LegacyFusionPanel/FusionBackToNodes")?.GetComponent<Button>()
            : null;
        Require(back != null && back.gameObject.activeInHierarchy && back.interactable,
            "Mezclas no presenta una salida visible y táctil.");
        back.onClick.Invoke();
        ForceUiGeometry();
        Require(!panel.FusionPanelVisible,
            "El botón Volver no regresó desde Mezclas a Nodos.");
        Require(panel.transform.Find("MachineMonolith2DRoot/MachinePrimaryContent")?.gameObject.activeInHierarchy == true,
            "El contenido del Monolito no reapareció al volver desde Mezclas.");
        CapturePair("11_return_from_fusion");
    }

    private static void ValidateTransitionFrame(MachineMonolith2DVisualUI visual,
        RectTransform art, RectTransform lab, RectTransform supportBack,
        RectTransform supportFront, CanvasGroup overview,
        CanvasGroup sector, CanvasGroup overviewLab, CanvasGroup closeLab,
        CanvasGroup veil, float checkpoint)
    {
        Require(art != null && lab != null && supportBack != null &&
            supportFront != null && overview != null && sector != null &&
            overviewLab != null && closeLab != null && veil != null,
            "Faltan componentes para validar la transición V40.");
        Require(overview.gameObject.activeInHierarchy &&
            sector.gameObject.activeInHierarchy &&
            overviewLab.gameObject.activeInHierarchy &&
            closeLab.gameObject.activeInHierarchy,
            "Origen y destino deben permanecer preparados durante el cruce.");
        Require(Mathf.Max(overview.alpha, sector.alpha) >= .50f,
            "Hay un fotograma visualmente vacío entre el Monolito y la cara.");
        Require(Mathf.Abs(overview.alpha + sector.alpha - 1f) < .025f,
            "El Monolito y la cara no mantienen una mezcla complementaria.");
        Require(Mathf.Abs(closeLab.alpha - sector.alpha) < .025f,
            "El fondo cercano se separó del avance de la cara.");
        Require(overviewLab.alpha > .99f,
            "El laboratorio general V06 se atenuó antes de que V27 cubriera el fondo.");
        RectTransform approach = overview.transform as RectTransform;
        Require(approach != null &&
            Vector2.Distance(approach.anchoredPosition,
                transitionApproachRestPosition) < .1f,
            "El contenedor de acercamiento se desvió del eje frontal.");
        Require(Vector3.Distance(approach.localScale,
                transitionApproachRestScale) < .003f,
            "El contenedor del Monolito cambio de escala antes del cruce.");
        if (checkpoint <= .10f)
        {
            Require(Vector3.Distance(GetBottomCenterWorld(art),
                    transitionArtBottomCenterWorld) < 1.5f,
                "La base visible del Monolito se separo del pedestal antes del cruce.");
        }
        Require(Vector2.Distance(art.anchoredPosition,
                transitionArtRestPosition) < .1f,
            "El acercamiento desplazó el Monolito: reposo=" +
            transitionArtRestPosition + " actual=" + art.anchoredPosition + ".");
        Require(Vector3.Distance(art.localScale,
                transitionArtRestScale) < .003f,
            "El arte interno cambió de escala durante el acercamiento: reposo=" +
            transitionArtRestScale + " actual=" + art.localScale + ".");
        Require(Vector2.Distance(lab.anchoredPosition,
                transitionLabRestPosition) < .1f &&
            Vector3.Distance(lab.localScale, transitionLabRestScale) < .002f,
            "El laboratorio cambió durante el acercamiento: reposo=" +
            transitionLabRestPosition + "/" + transitionLabRestScale +
            " actual=" + lab.anchoredPosition + "/" + lab.localScale + ".");
        Require(Vector2.Distance(supportBack.anchoredPosition,
                transitionSupportBackRestPosition) < 2.1f &&
            Vector3.Distance(supportBack.localScale,
                transitionSupportBackRestScale) < .002f,
            "El soporte posterior cambió durante el acercamiento: reposo=" +
            transitionSupportBackRestPosition + "/" +
            transitionSupportBackRestScale + " actual=" +
            supportBack.anchoredPosition + "/" + supportBack.localScale + ".");
        Require(Vector2.Distance(supportFront.anchoredPosition,
                transitionSupportFrontRestPosition) < 2.1f &&
            Vector3.Distance(supportFront.localScale,
                transitionSupportFrontRestScale) < .002f,
            "El soporte frontal cambió durante el acercamiento: reposo=" +
            transitionSupportFrontRestPosition + "/" +
            transitionSupportFrontRestScale + " actual=" +
            supportFront.anchoredPosition + "/" + supportFront.localScale + ".");
        for (int index = 0; index < 4; index++)
        {
            RectTransform light = art.Find("FaceEntryLight_" + (index + 1))
                as RectTransform;
            Vector2 expected =
                MachineMonolith2DVisualUI.GetOverviewEntryLightAnchor(index, 0);
            Require(light != null && light.parent == art &&
                Vector2.Distance(light.anchorMin, expected) < .002f &&
                Vector2.Distance(light.anchorMax, expected) < .002f &&
                light.anchoredPosition.sqrMagnitude < .01f &&
                Vector3.Distance(light.localScale, Vector3.one) < .001f,
                "La luz " + (index + 1) +
                " se desprendió del Monolito durante la transición.");
        }
        Require(overview.alpha <= previousOverviewAlpha + .02f &&
            sector.alpha + .02f >= previousSectorAlpha &&
            closeLab.alpha + .02f >= previousCloseLabAlpha,
            "La transición invirtió una opacidad y produjo un salto visible.");
        Require(veil.alpha >= 0f && veil.alpha <= .181f,
            "El velo excede el pulso sutil permitido.");
        previousOverviewAlpha = overview.alpha;
        previousSectorAlpha = sector.alpha;
        previousCloseLabAlpha = closeLab.alpha;
    }

    private static Vector3 GetBottomCenterWorld(RectTransform rect)
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);
        return (corners[0] + corners[3]) * .5f;
    }

    private static bool UsesFootprintStage(GameObject target, float expectedStage)
    {
        RawImage image = target != null ? target.GetComponent<RawImage>() : null;
        return image != null && image.material != null &&
            image.material.HasProperty("_FootprintStage") &&
            Mathf.Abs(image.material.GetFloat("_FootprintStage") - expectedStage) < .01f;
    }

    private static MachineMonolith2DVisualUI FindVisual()
    {
        MachineMonolith2DVisualUI visual =
            UnityEngine.Object.FindFirstObjectByType<MachineMonolith2DVisualUI>(
                FindObjectsInactive.Include);
        Require(visual != null, "MachineMonolith2DVisualUI no está disponible.");
        return visual;
    }

    private static void PrepareMachine(bool repaired, int partialStage = 0)
    {
        SaveService.SuppressWritesForVisualQa = true;
        Require(GameState.I != null, "GameState no disponible.");
        Require(MachineManager.I != null, "MachineManager no disponible.");
        GameState.I.experimentalChamberUnlocked = true;
        GameState.I.LE = 1e15;
        GameState.I.Traces = 1e12;

        List<string> repairedIds = repaired
            ? MachineManager.I.GetAllNodes(true)
                .Where(node => node != null)
                .Select(node => node.id)
                .ToList()
            : new List<string>();
        if (!repaired && partialStage > 0)
        {
            List<MachineNodeDef> visibleNodes = MachineManager.I.GetAllNodes(false)
                .Where(node => node != null)
                .ToList();
            double targetProgress = partialStage switch
            {
                1 => .20,
                2 => .40,
                _ => .60
            };
            int targetCount = Mathf.CeilToInt(
                (float)(visibleNodes.Count * targetProgress));
            repairedIds.AddRange(visibleNodes.Take(targetCount)
                .Select(node => node.id));
        }
        SaveData state = new SaveData
        {
            machineIntroSeen = true,
            machineUnlocked = true,
            machineAllZonesUnlocked = true,
            machineFusionPanelUnlocked = true,
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
        ForceUiGeometry();
    }

    private static void ActivateAncestors(Transform target)
    {
        Transform current = target;
        while (current != null)
        {
            current.gameObject.SetActive(true);
            current = current.parent;
        }
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

    private static void CapturePair(string name)
    {
        Capture(1080, 1920, name + "_1080x1920.png");
        Capture(720, 1280, name + "_720x1280.png");
    }

    private static void Capture(int width, int height, string filename)
    {
        string path = Path.Combine(OutputDirectory, filename);
        RenderToPng(width, height, path);
        Require(File.Exists(path) && new FileInfo(path).Length > 10000,
            "Captura vacía: " + filename);
    }

    private static void RenderToPng(int width, int height, string path)
    {
        BeginCaptureRendering(width, height);
        RenderTexture previousActive = RenderTexture.active;
        try
        {
            for (int pass = 0; pass < 5; pass++)
            {
                ForceUiGeometry();
                captureCamera.Render();
            }
            RenderTexture.active = captureTarget;
            Texture2D image = new Texture2D(width, height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            image.Apply();
            File.WriteAllBytes(path, image.EncodeToPNG());
            // Unmodified pixels from this same render, including the entire damaged edge.
            if (Path.GetFileName(path).Contains("_sector_") &&
                !Path.GetFileName(path).Contains("_4_"))
            {
                float scale = width / 1080f;
                int x = Mathf.RoundToInt(350f * scale);
                int y = Mathf.RoundToInt((1920f - 1120f) * scale);
                int cropWidth = Mathf.RoundToInt(450f * scale);
                int cropHeight = Mathf.RoundToInt(770f * scale);
                Texture2D detail = new Texture2D(cropWidth, cropHeight,
                    TextureFormat.RGB24, false);
                detail.SetPixels(image.GetPixels(x, y, cropWidth, cropHeight));
                detail.Apply();
                File.WriteAllBytes(Path.Combine(OutputDirectory,
                    "detail_" + Path.GetFileName(path)), detail.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(detail);
            }
            UnityEngine.Object.DestroyImmediate(image);
            ValidateRenderedSurface(width, height);
        }
        finally
        {
            RenderTexture.active = previousActive;
            EndCaptureTarget();
        }
    }

    private static void ValidateRenderedSurface(int width, int height)
    {
        MachineMonolith2DVisualUI visual = FindVisual();
        if (!visual.IsShowingSector || visual.CurrentSectorIndex >= 3 ||
            visual.TransitionProgress < .999f)
            return;
        int face = visual.CurrentSectorIndex;
        RawImage art = visual.GetComponentsInChildren<RawImage>(true)
            .FirstOrDefault(image => image.name == "SectorArtwork" &&
                image.gameObject.activeInHierarchy);
        if (art == null)
            return; // An auxiliary view, e.g. Mixes, owns the screen.
        Rect rect = art.rectTransform.rect;
        RawImage[] symbols = visual.GetComponentsInChildren<RawImage>(true)
            .Where(image => image.name == "AlienSignature" &&
                image.gameObject.activeInHierarchy).ToArray();
        int stage = MachineMonolith2DVisualUI.GetRepairStage(
            MachineManager.I != null
                ? MachineManager.I.GetTotalMachineRepairProgress01()
                : 0.0);
        Require(symbols.Length ==
                MachineMonolith2DVisualUI.GetFaceVisibleNodeCount(face, stage),
            "Cantidad incorrecta en la validación renderizada de cara " + (face + 1));
        ValidateActiveHitboxCentersAreUnambiguous(visual, art.rectTransform,
            face, width, height);
        string assetPath = AssetDatabase.GetAssetPath(art.texture);
        if (!surfaceTextures.TryGetValue(assetPath, out Texture2D surface))
        {
            surface = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            Require(surface.LoadImage(File.ReadAllBytes(assetPath)),
                "No se pudo leer el alfa del arte canónico: " + assetPath);
            surfaceTextures.Add(assetPath, surface);
        }
        // Control negativo: un centro sobre el riel y otro fuera del zócalo
        // deben seguir siendo rechazados aunque la zona útil se haya ampliado.
        Vector2 oldHalf = new Vector2(20f / rect.width, 20f / rect.height);
        Require(!MachineMonolith2DVisualUI.IsNodeFootprintInsideDarkSurface(0,
                new Vector2(.720f, .500f), oldHalf) &&
            !MachineMonolith2DVisualUI.IsNodeFootprintInsideDarkSurface(0,
                new Vector2(.180f, .150f), oldHalf),
            "La prueba dejó de detectar rieles y zonas externas como inseguras.");

        Vector3[] corners = new Vector3[4];
        for (int index = 0; index < symbols.Length; index++)
        {
            symbols[index].rectTransform.GetWorldCorners(corners);
            Vector2 min = Vector2.positiveInfinity;
            Vector2 max = Vector2.negativeInfinity;
            foreach (Vector3 corner in corners)
            {
                Vector3 local = art.rectTransform.InverseTransformPoint(corner);
                Vector2 uv = new Vector2((local.x - rect.xMin) / rect.width,
                    (local.y - rect.yMin) / rect.height);
                min = Vector2.Min(min, uv);
                max = Vector2.Max(max, uv);
            }
            Vector2 clearance = new Vector2(4f / rect.width, 4f / rect.height);
            min -= clearance;
            max += clearance;
            string context = $"cara {face + 1}, UV etapa {art.uvRect}, " +
                $"{width}x{height}, firma {index + 1}, límites {min:F4}–{max:F4}";
            Require(MachineMonolith2DVisualUI.IsNodeFootprintInsideDarkSurface(
                face, (min + max) * .5f, (max - min) * .5f),
                "La firma invade el perímetro dañado, riel o base: " + context);
            for (int row = 0; row <= 6; row++)
                for (int column = 0; column <= 6; column++)
                {
                    float u = Mathf.Lerp(min.x, max.x, column / 6f);
                    float v = Mathf.Lerp(min.y, max.y, row / 6f);
                    Color pixel = surface.GetPixelBilinear(
                        art.uvRect.x + u * art.uvRect.width,
                        art.uvRect.y + v * art.uvRect.height);
                    // Generated stone contains a few nearly opaque antialias pixels
                    // (252/255). Real gaps and damaged voids remain far below 98%.
                    Require(pixel.a >= SolidSurfaceAlphaThreshold,
                        "La firma alcanza transparencia en la etapa visible: " + context);
                }
        }
        Debug.Log($"[Monolith surface] face={face + 1} tile={art.uvRect} " +
            $"resolution={width}x{height} signatures={symbols.Length} PASS");
        float sourceLeft = Mathf.Min(art.uvRect.x,
            art.uvRect.x + art.uvRect.width);
        int tile = (art.uvRect.y > .25f ? 0 : 2) +
            (sourceLeft > .25f ? 1 : 0);
        surfaceCases.Add($"{face}/{tile}/{width}/{height}");
    }

    private static void ValidateRepairStageBoundaries()
    {
        Require(MachineMonolith2DVisualUI.GetRepairStage(.1999) == 0 &&
                MachineMonolith2DVisualUI.GetRepairStage(.20) == 1 &&
                MachineMonolith2DVisualUI.GetRepairStage(.3999) == 1 &&
                MachineMonolith2DVisualUI.GetRepairStage(.40) == 2 &&
                MachineMonolith2DVisualUI.GetRepairStage(.5999) == 2 &&
                MachineMonolith2DVisualUI.GetRepairStage(.60) == 3,
            "Los límites 20/40/60 % de reparación no son exactos.");
    }

    private static void ValidateActiveHitboxCentersAreUnambiguous(
        MachineMonolith2DVisualUI visual, RectTransform art, int face,
        int width, int height)
    {
        Button[] hits = visual.GetComponentsInChildren<Button>(true)
            .Where(button => button.gameObject.activeInHierarchy &&
                button.name.StartsWith("NodeTouch_", StringComparison.Ordinal))
            .OrderBy(button => button.name)
            .ToArray();
        Vector3[] leftCorners = new Vector3[4];
        Vector3[] rightCorners = new Vector3[4];
        for (int left = 0; left < hits.Length; left++)
        {
            RectTransform leftRect = hits[left].transform as RectTransform;
            Require(leftRect != null && hits[left].interactable,
                $"Hitbox inválida o no interactiva en cara {face + 1}: " +
                hits[left].name);
            leftRect.GetWorldCorners(leftCorners);
            for (int right = left + 1; right < hits.Length; right++)
            {
                RectTransform rightRect = hits[right].transform as RectTransform;
                Require(rightRect != null,
                    $"Hitbox inválida en cara {face + 1}: " + hits[right].name);
                rightRect.GetWorldCorners(rightCorners);
                Vector2 leftMin = Vector2.positiveInfinity;
                Vector2 leftMax = Vector2.negativeInfinity;
                Vector2 rightMin = Vector2.positiveInfinity;
                Vector2 rightMax = Vector2.negativeInfinity;
                foreach (Vector3 corner in leftCorners)
                {
                    Vector3 local = art.InverseTransformPoint(corner);
                    leftMin = Vector2.Min(leftMin, local);
                    leftMax = Vector2.Max(leftMax, local);
                }
                foreach (Vector3 corner in rightCorners)
                {
                    Vector3 local = art.InverseTransformPoint(corner);
                    rightMin = Vector2.Min(rightMin, local);
                    rightMax = Vector2.Max(rightMax, local);
                }
                Vector2 leftCenter = (leftMin + leftMax) * .5f;
                Vector2 rightCenter = (rightMin + rightMax) * .5f;
                bool rightCenterInsideLeft = rightCenter.x > leftMin.x &&
                    rightCenter.x < leftMax.x && rightCenter.y > leftMin.y &&
                    rightCenter.y < leftMax.y;
                bool leftCenterInsideRight = leftCenter.x > rightMin.x &&
                    leftCenter.x < rightMax.x && leftCenter.y > rightMin.y &&
                    leftCenter.y < rightMax.y;
                Require(!rightCenterInsideLeft && !leftCenterInsideRight,
                    $"El centro visible de un nodo cae dentro del toque de otro " +
                    $"en cara {face + 1}, {width}x{height}: " +
                    $"{hits[left].name} y {hits[right].name}.");
            }
        }
        Debug.Log($"[Monolith hitboxes] face={face + 1} resolution=" +
            $"{width}x{height} count={hits.Length} center-safe PASS");
    }

    private static void WriteFaceReviewSheets()
    {
        foreach (bool repaired in new[] { false, true })
        {
            Texture2D sheet = null;
            for (int face = 1; face <= 3; face++)
            {
                string state = repaired ? "repaired" : "initial";
                int order = repaired ? face + 6 : face + 1;
                Texture2D crop = new Texture2D(2, 2, TextureFormat.RGB24, false);
                crop.LoadImage(File.ReadAllBytes(Path.Combine(OutputDirectory,
                    $"detail_{order:00}_sector_{face}_{state}_1080x1920.png")));
                if (sheet == null)
                    sheet = new Texture2D(crop.width * 3, crop.height,
                        TextureFormat.RGB24, false);
                sheet.SetPixels((face - 1) * crop.width, 0, crop.width,
                    crop.height, crop.GetPixels());
                UnityEngine.Object.DestroyImmediate(crop);
            }
            sheet.Apply();
            File.WriteAllBytes(Path.Combine(OutputDirectory, repaired
                ? "review_faces_1_2_3_repaired.png" : "review_faces_1_2_3_damaged.png"),
                sheet.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(sheet);
        }
    }

    private static void BeginCaptureRendering(int width, int height)
    {
        if (captureCamera == null)
        {
            captureCamera = Camera.main != null
                ? Camera.main
                : UnityEngine.Object.FindFirstObjectByType<Camera>();
            Require(captureCamera != null, "No hay cámara disponible para la captura.");
            captureCanvases = UnityEngine.Object.FindObjectsByType<Canvas>(
                    FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .Where(canvas => canvas != null && canvas.isRootCanvas)
                .OrderBy(canvas => canvas.sortingOrder)
                .ThenBy(canvas => canvas.transform.GetSiblingIndex())
                .ToArray();
            previousCanvasModes = new RenderMode[captureCanvases.Length];
            previousCanvasCameras = new Camera[captureCanvases.Length];
            previousPlaneDistances = new float[captureCanvases.Length];
            previousCameraTarget = captureCamera.targetTexture;

            for (int i = 0; i < captureCanvases.Length; i++)
            {
                Canvas canvas = captureCanvases[i];
                previousCanvasModes[i] = canvas.renderMode;
                previousCanvasCameras[i] = canvas.worldCamera;
                previousPlaneDistances[i] = canvas.planeDistance;
                if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    canvas.worldCamera = captureCamera;
                    canvas.planeDistance = 1f + (captureCanvases.Length - i) * .05f;
                }
            }
        }

        EndCaptureTarget();
        captureTarget = new RenderTexture(width, height, 24,
            RenderTextureFormat.ARGB32);
        captureCamera.targetTexture = captureTarget;
        RecalculateCanvasScalers();
        Canvas.ForceUpdateCanvases();
    }

    private static void RecalculateCanvasScalers()
    {
        MethodInfo handle = typeof(CanvasScaler).GetMethod(
            "Handle", BindingFlags.Instance | BindingFlags.NonPublic);
        if (handle == null || captureCanvases == null)
            return;
        foreach (Canvas canvas in captureCanvases)
        {
            CanvasScaler scaler = canvas != null
                ? canvas.GetComponent<CanvasScaler>() : null;
            if (scaler != null && scaler.enabled)
                handle.Invoke(scaler, null);
        }
    }

    private static void ForceUiGeometry()
    {
        Canvas.ForceUpdateCanvases();
        MachineMonolith2DVisualUI[] visuals =
            UnityEngine.Object.FindObjectsByType<MachineMonolith2DVisualUI>(
                FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (MachineMonolith2DVisualUI visual in visuals)
            visual.RefreshNow();
        RectTransform[] rects = UnityEngine.Object.FindObjectsByType<RectTransform>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (RectTransform rect in rects)
            rect.ForceUpdateRectTransforms();
        TMP_Text[] texts = UnityEngine.Object.FindObjectsByType<TMP_Text>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (TMP_Text text in texts)
            text.ForceMeshUpdate(false, true);
        Canvas.ForceUpdateCanvases();
    }

    private static void EndCaptureRendering()
    {
        EndCaptureTarget();
        if (captureCanvases != null)
        {
            for (int i = 0; i < captureCanvases.Length; i++)
            {
                Canvas canvas = captureCanvases[i];
                if (canvas == null)
                    continue;
                canvas.renderMode = previousCanvasModes[i];
                canvas.worldCamera = previousCanvasCameras[i];
                canvas.planeDistance = previousPlaneDistances[i];
            }
        }
        if (captureCamera != null)
            captureCamera.targetTexture = previousCameraTarget;
        captureCamera = null;
        previousCameraTarget = null;
        captureCanvases = null;
        previousCanvasModes = null;
        previousCanvasCameras = null;
        previousPlaneDistances = null;
    }

    private static void EndCaptureTarget()
    {
        if (captureCamera != null && captureCamera.targetTexture == captureTarget)
            captureCamera.targetTexture = previousCameraTarget;
        if (captureTarget != null)
        {
            captureTarget.Release();
            UnityEngine.Object.DestroyImmediate(captureTarget);
        }
        captureTarget = null;
    }

    private static void BackupUserSave()
    {
        SessionState.SetBool(SaveExistedKey, File.Exists(SavePath));
        SessionState.SetBool(BackupExistedKey, File.Exists(SaveBackupPath));
        DeleteIfPresent(SessionSave);
        DeleteIfPresent(SessionBackup);
        if (File.Exists(SavePath))
            File.Copy(SavePath, SessionSave, true);
        if (File.Exists(SaveBackupPath))
            File.Copy(SaveBackupPath, SessionBackup, true);
    }

    private static void RestoreUserSave()
    {
        if (SessionState.GetBool(SaveExistedKey, false))
        {
            Require(File.Exists(SessionSave), "Falta la copia temporal de save.json.");
            File.Copy(SessionSave, SavePath, true);
        }
        else
        {
            DeleteIfPresent(SavePath);
        }

        if (SessionState.GetBool(BackupExistedKey, false))
        {
            Require(File.Exists(SessionBackup), "Falta la copia temporal de save.json.bak.");
            File.Copy(SessionBackup, SaveBackupPath, true);
        }
        else
        {
            DeleteIfPresent(SaveBackupPath);
        }
        DeleteIfPresent(SessionSave);
        DeleteIfPresent(SessionBackup);
    }

    private static void DeleteIfPresent(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
    }

    private static bool RectApproximately(Rect value, Rect expected)
    {
        return Mathf.Abs(value.x - expected.x) < .001f &&
            Mathf.Abs(value.y - expected.y) < .001f &&
            Mathf.Abs(value.width - expected.width) < .001f &&
            Mathf.Abs(value.height - expected.height) < .001f;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
#endif

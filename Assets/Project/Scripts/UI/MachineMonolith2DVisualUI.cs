using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class MachineMonolith2DVisualUI : MonoBehaviour
{
    public const float SectorCloseScale = .87f;
    public const float SectorCloseWidthScale = 1.01f;
    public const float SectorFloorAlignmentRatio = .11f;
    public const float OverviewDisplayScale = .72f;
    public const float SectorTransitionDuration = .88f;
    public const float NodeSymbolDisplaySize = 32f;
    public const float NodeTouchDisplaySize = 80f;
    public const float NodeMinimumNormalizedSeparation = .068f;
    public const float NodeSymbolUvWidth = .112f;
    public const float NodeSymbolUvHeight = .063f;
    public const float DamagedNodeAlpha = .64f;
    public const float SelectedDamagedNodeAlpha = .84f;
    public const float RepairedNodeAlpha = 1f;
    public const float RepairedNodeDisplaySize = 32f;
    public static readonly Vector2 OverviewDisplayPosition = new Vector2(0f, 34f);

    [Header("Integración")]
    [SerializeField] private MachinePanelUI machinePanel;
    [SerializeField] private GameObject visualContentRoot;
    [SerializeField] private GameObject primaryContentRoot;
    [SerializeField] private GameObject overviewRoot;
    [SerializeField] private GameObject sectorRoot;
    [SerializeField] private GameObject nodeCardRoot;
    [SerializeField] private CanvasGroup nodeCardCanvasGroup;
    [SerializeField] private RectTransform viewportRect;
    [SerializeField] private GameObject overviewLabBackgroundRoot;
    [SerializeField] private GameObject closeLabBackgroundRoot;
    [SerializeField] private RectTransform overviewLabBackgroundRect;
    [SerializeField] private Vector2 overviewLabBackgroundRestPosition;
    [SerializeField] private Vector3 overviewLabBackgroundRestScale = Vector3.one;
    [SerializeField] private CanvasGroup overviewLabBackgroundGroup;
    [SerializeField] private CanvasGroup closeLabBackgroundGroup;
    [SerializeField] private GameObject overviewSupportBackgroundRoot;
    [SerializeField] private RectTransform overviewSupportBackgroundRect;
    [SerializeField] private Vector2 overviewSupportBackgroundRestPosition;
    [SerializeField] private Vector3 overviewSupportBackgroundRestScale = Vector3.one;
    [SerializeField] private CanvasGroup overviewSupportBackgroundGroup;
    [SerializeField] private GameObject overviewSupportForegroundRoot;
    [SerializeField] private RectTransform overviewSupportForegroundRect;
    [SerializeField] private Vector2 overviewSupportForegroundRestPosition;
    [SerializeField] private Vector3 overviewSupportForegroundRestScale = Vector3.one;
    [SerializeField] private CanvasGroup overviewSupportForegroundGroup;

    [Header("Arte 2D aprobado")]
    [SerializeField] private RawImage overviewArtwork;
    [SerializeField] private Vector2 overviewRestPosition;
    [SerializeField] private Vector3 overviewRestScale = Vector3.one;
    [SerializeField] private RawImage sectorArtwork;
    [SerializeField] private CanvasGroup overviewCanvasGroup;
    [SerializeField] private CanvasGroup sectorCanvasGroup;
    [SerializeField] private Texture2D overviewProgressionSheet;
    [SerializeField] private Texture2D[] sectorProgressionSheets;
    [SerializeField] private Texture2D[] sectorSymbolSheets;
    [SerializeField] private CanvasGroup transitionVeilGroup;

    [Header("Navegación")]
    [SerializeField] private Button[] sectorButtons;
    [SerializeField] private GameObject[] overviewEntryLights;
    [SerializeField] private Button backToOverviewButton;
    [SerializeField] private Button[] nodeButtons;
    [SerializeField] private RawImage[] nodeSymbolImages;
    [SerializeField] private TextMeshProUGUI reservedFaceMarker;

    [Header("Cabecera")]
    [SerializeField] private TextMeshProUGUI viewTitleText;
    [SerializeField] private TextMeshProUGUI viewIndexText;
    [SerializeField] private TextMeshProUGUI leResourceText;
    [SerializeField] private TextMeshProUGUI tracesResourceText;
    [SerializeField] private TextMeshProUGUI globalProgressText;
    [SerializeField] private TextMeshProUGUI convergenceText;
    [SerializeField] private Image globalProgressFill;

    [Header("Ficha completa de nodo")]
    [SerializeField] private Image selectedIconImage;
    [SerializeField] private TextMeshProUGUI selectedIconText;
    [SerializeField] private TextMeshProUGUI selectedNameText;
    [SerializeField] private TextMeshProUGUI selectedStateText;
    [SerializeField] private TextMeshProUGUI selectedDescriptionText;
    [SerializeField] private TextMeshProUGUI selectedEffectText;
    [SerializeField] private TextMeshProUGUI selectedRequirementsText;
    [SerializeField] private TextMeshProUGUI selectedCostText;
    [SerializeField] private TextMeshProUGUI selectedSectorProgressText;
    [SerializeField] private Sprite iconLe;
    [SerializeField] private Sprite iconTraces;
    [SerializeField] private Sprite iconTriangle;
    [SerializeField] private Sprite iconArtifact;
    [SerializeField] private Sprite iconFusion;
    [SerializeField] private Sprite iconDiagnostic;
    [SerializeField] private Sprite iconStructure;
    [SerializeField] private Sprite iconConvergence;
    [SerializeField] private Sprite iconAnchor;
    [SerializeField] private Sprite iconSynthesis;

    private bool _showingSector;
    private bool _transitioning;
    private int _currentSectorIndex = 1;
    private Coroutine _transitionRoutine;
    private float _transitionProgress;

    private static readonly Rect[] ProgressionUvRects =
    {
        new Rect(0f, .5f, .5f, .5f),
        new Rect(.5f, .5f, .5f, .5f),
        new Rect(0f, 0f, .5f, .5f),
        new Rect(.5f, 0f, .5f, .5f)
    };

    private static readonly Rect[] OverviewUvRects =
    {
        new Rect(0f, .5f, .5f, .5f),
        new Rect(.5f, .5f, .5f, .5f),
        new Rect(0f, 0f, .5f, .5f),
        new Rect(.5f, 0f, .5f, .5f)
    };

    private static readonly Vector2[][] NodePositions =
    {
        new[]
        {
            new Vector2(.4756f, .1844f), new Vector2(.2803f, .3332f),
            new Vector2(.6111f, .3443f), new Vector2(.3269f, .5028f),
            new Vector2(.4993f, .6083f), new Vector2(.3764f, .7560f),
            new Vector2(.6314f, .7570f)
        },
        new[]
        {
            new Vector2(.7826f, .2474f), new Vector2(.5206f, .2637f),
            new Vector2(.6612f, .3597f), new Vector2(.4758f, .4090f),
            new Vector2(.7255f, .4816f), new Vector2(.5855f, .5084f),
            new Vector2(.4084f, .5701f), new Vector2(.6463f, .6529f),
            new Vector2(.4768f, .6839f), new Vector2(.5850f, .7947f),
            new Vector2(.4165f, .8157f)
        },
        new[]
        {
            new Vector2(.6149f, .2562f), new Vector2(.6582f, .3792f),
            new Vector2(.4209f, .4699f), new Vector2(.6129f, .5074f),
            new Vector2(.4291f, .6017f), new Vector2(.6148f, .6670f),
            new Vector2(.4777f, .7595f)
        },
        Array.Empty<Vector2>()
    };

    private static readonly Vector2[][] OverviewEntryLightAnchors =
    {
        new[] { new Vector2(.365f, .708f), new Vector2(.370f, .704f), new Vector2(.374f, .700f), new Vector2(.378f, .696f) },
        new[] { new Vector2(.666f, .704f), new Vector2(.670f, .700f), new Vector2(.674f, .696f), new Vector2(.678f, .692f) },
        new[] { new Vector2(.704f, .326f), new Vector2(.700f, .330f), new Vector2(.696f, .334f), new Vector2(.692f, .338f) },
        new[] { new Vector2(.344f, .326f), new Vector2(.348f, .330f), new Vector2(.352f, .334f), new Vector2(.356f, .338f) }
    };

    private static readonly float[] OverviewEntryLightRotations =
        { -18f, 20f, 14f, -13f };
    private static readonly Vector2[] OverviewEntryLightSizes =
        { new Vector2(46f, 118f), new Vector2(44f, 110f),
          new Vector2(42f, 102f), new Vector2(40f, 96f) };
    private static readonly float[] OverviewEntryLightAlphas =
        { .78f, .72f, .66f, .60f };

    private static readonly int[][] CanonicalSymbolIndices =
    {
        new[] { 0, 1, 2, 3, 4, 5, 6 },
        new[] { 3, 0, 5, 2, 6, 1, 4, 0, 2, 5, 1 },
        new[] { 4, 1, 6, 0, 5, 2, 3 },
        Array.Empty<int>()
    };

    private static readonly Vector2[] Face1DisplayPositions =
    {
        new Vector2(.596223f, .681511f), new Vector2(.590667f, .439111f),
        new Vector2(.431556f, .150222f), new Vector2(.532000f, .583111f),
        new Vector2(.472444f, .323555f), new Vector2(.462667f, .472889f),
        new Vector2(.576444f, .261333f), new Vector2(.549460f, .802069f)
    };

    private static readonly Vector2[] Face2DisplayPositions =
    {
        new Vector2(.552158f, .798470f), new Vector2(.554856f, .488079f),
        new Vector2(.656475f, .120108f), new Vector2(.572842f, .632029f),
        new Vector2(.557554f, .298246f), new Vector2(.631295f, .419703f),
        new Vector2(.682554f, .215475f), new Vector2(.562050f, .206478f)
    };

    private static readonly Vector2[] Face3DisplayPositions =
    {
        new Vector2(.440647f, .734593f), new Vector2(.473921f, .431399f),
        new Vector2(.609712f, .186685f), new Vector2(.529676f, .573549f),
        new Vector2(.589029f, .316239f), new Vector2(.578237f, .757085f),
        new Vector2(.625899f, .439496f), new Vector2(.680755f, .615834f)
    };

    // Las caras son una presentación estable, no las zonas persistentes. Cada
    // reparación global revela 3/5/7/8 ramas por cara; una firma ya revelada
    // conserva siempre su posición. Los nodos ocultos no ocupan espacio visual.
    private static readonly string[][] FaceBranchKeys =
    {
        new[]
        {
            "z1_energy_coupling", "z2_fusion_table", "z3_internal_diagnostics",
            "z1_triangle_anchor", "z2_composition_reading", "z3_machine_memory",
            "z1_room1_synchronizer", "z2_fusion_time_control"
        },
        new[]
        {
            "z1_traces_channel", "z2_fusion_slot_2", "z3_auxiliary_conduits",
            "z1_artifact_calibration", "z2_fusion_slot_3", "z3_sync_core",
            "z2_stable_reaction_chamber", "z2_guided_synthesis"
        },
        new[]
        {
            "z1_protocol_reading", "z2_mix_stabilizer", "z3_compensation_circuit",
            "z1_flow_distributor", "z2_residual_catalyst", "z3_structural_reinforcement",
            "z2_catalyst_tuning", "z2_synthesis_core"
        },
        Array.Empty<string>()
    };

    // V56: structural envelope of the final dark stone, excluding rails and the
    // base. Runtime/capture validation also samples the alpha of the CURRENT
    // repair tile, so a newly revealed signature can use newly repaired surface
    // without ever appearing over a gap in an earlier stage.
    private static readonly Vector2[][] FaceDarkSurfacePolygons =
    {
        new[]
        {
            new Vector2(.340f, .920f), new Vector2(.660f, .920f),
            new Vector2(.645f, .180f), new Vector2(.500f, .070f),
            new Vector2(.260f, .070f)
        },
        new[]
        {
            new Vector2(.495f, .930f), new Vector2(.620f, .900f),
            new Vector2(.790f, .070f), new Vector2(.621f, .055f),
            new Vector2(.475f, .145f)
        },
        new[]
        {
            new Vector2(.350f, .900f), new Vector2(.700f, .900f),
            new Vector2(.860f, .100f), new Vector2(.350f, .100f)
        }
    };

    private static readonly MachineZoneType[] FaceContentZones =
    {
        MachineZoneType.FusionSector,
        MachineZoneType.Room1Link,
        MachineZoneType.InternalSupport,
        MachineZoneType.InstantChamber
    };

    private static readonly float[] SectorWidthMultipliers =
    {
        1.12f, 1.12f, 1.12f, 1.12f
    };

    private static readonly float[][] SectorStageHorizontalOffsets =
    {
        new[] { 0f, 0f, 0f, 0f },
        new[] { 0f, 0f, 0f, 0f },
        new[] { 0f, 0f, 0f, 0f },
        new[] { 0f, 0f, 0f, 0f }
    };

    public bool IsShowingSector => _showingSector;
    public int CurrentSectorIndex => _currentSectorIndex;
    public float TransitionProgress => _transitionProgress;
#if UNITY_EDITOR
    // Deterministic rendered checkpoints; never compiled into the player.
    public float? EditorTransitionProgressOverride { get; set; }
#endif

    private void Awake()
    {
        if (nodeCardRoot != null && nodeCardCanvasGroup == null)
        {
            nodeCardCanvasGroup = nodeCardRoot.GetComponent<CanvasGroup>();
            if (nodeCardCanvasGroup == null)
                nodeCardCanvasGroup = nodeCardRoot.AddComponent<CanvasGroup>();
        }
        if (sectorButtons != null)
        {
            for (int i = 0; i < sectorButtons.Length; i++)
            {
                int capturedIndex = i;
                if (sectorButtons[i] != null && i < 4)
                    sectorButtons[i].onClick.AddListener(() => ShowSector(capturedIndex));
            }
        }

        if (backToOverviewButton != null)
            backToOverviewButton.onClick.AddListener(ShowOverview);

        if (nodeButtons != null)
        {
            for (int i = 0; i < nodeButtons.Length; i++)
            {
                int capturedIndex = i;
                if (nodeButtons[i] != null)
                    nodeButtons[i].onClick.AddListener(() => SelectDisplayNode(capturedIndex));
            }
        }
    }

    private void OnEnable()
    {
        RefreshNow();
    }

    private void OnDisable()
    {
        if (_transitionRoutine != null)
            StopCoroutine(_transitionRoutine);
        _transitionRoutine = null;
        _transitioning = false;
        ResetTransitionVisuals();
    }

    public void RefreshNow()
    {
        // The coroutine owns geometry until landing. No repair can occur in this interval.
        if (_transitioning && _transitionProgress > 0f)
            return;
        bool available = MachineManager.I != null && MachineManager.I.MachineUnlocked;
        if (visualContentRoot != null)
            visualContentRoot.SetActive(available);
        if (!available || machinePanel == null)
            return;

        RefreshHeader();
        bool showPrimary = !machinePanel.HasAuxiliaryViewOpen;
        if (primaryContentRoot != null)
            primaryContentRoot.SetActive(showPrimary);
        if (!showPrimary)
        {
            RefreshOverviewEntryLights();
            return;
        }

        RefreshArtwork();
        RefreshNodeHotspots();
        RefreshSelectedCard();
    }

    public void ShowOverviewImmediate()
    {
        _showingSector = false;
        _transitionProgress = 0f;
        ResetTransitionVisuals();
        RefreshNow();
    }

    public void ShowSectorImmediate(int sectorIndex)
    {
        _currentSectorIndex = Mathf.Clamp(sectorIndex, 0, 3);
        _showingSector = true;
        _transitionProgress = 1f;
        ResetTransitionVisuals();
        if (machinePanel != null)
            machinePanel.SelectZoneFromCube(GetContentZoneForFace(_currentSectorIndex));
        else
            RefreshNow();
    }

    private void ShowOverview()
    {
        if (_transitioning)
            return;
        _showingSector = false;
        _transitionProgress = 0f;
        ResetTransitionVisuals();
        RefreshNow();
    }

    private void ShowSector(int sectorIndex)
    {
        if (_transitioning || sectorIndex < 0 || sectorIndex > 3)
            return;
        _transitionRoutine = StartCoroutine(OpenSectorAnimated(sectorIndex));
    }

    private IEnumerator OpenSectorAnimated(int sectorIndex)
    {
        _transitioning = true;
        _transitionProgress = 0f;
        _currentSectorIndex = sectorIndex;
        SetSectorButtonsInteractable(false);

        // Preserve the exact overview pose before preparing the destination
        // layout. The shared viewport changes size for the close face and can
        // otherwise rewrite the artwork's anchored position before the first
        // animated frame.
        RectTransform artRect = overviewArtwork != null
            ? overviewArtwork.rectTransform
            : null;
        AspectRatioFitter overviewFitter = artRect != null
            ? artRect.GetComponent<AspectRatioFitter>() : null;
        if (overviewFitter != null)
            overviewFitter.enabled = false;
        if (artRect != null)
        {
            artRect.anchoredPosition = overviewRestPosition;
            artRect.localScale = overviewRestScale;
        }
        RectTransform approachRect = overviewRoot != null
            ? overviewRoot.transform as RectTransform : null;
        Vector2 approachStartPosition = approachRect != null
            ? approachRect.anchoredPosition : Vector2.zero;
        Vector3 approachStartScale = approachRect != null
            ? approachRect.localScale : Vector3.one;
        RectTransform labRect = overviewLabBackgroundRect;
        Vector2 labStartPosition = labRect != null
            ? labRect.anchoredPosition : Vector2.zero;
        Vector3 labStartScale = labRect != null
            ? labRect.localScale : Vector3.one;
        RectTransform supportRect = overviewSupportForegroundRect;
        AspectRatioFitter supportFitter = supportRect != null
            ? supportRect.GetComponent<AspectRatioFitter>() : null;
        if (supportFitter != null)
            supportFitter.enabled = false;
        Vector2 supportStartPosition = supportRect != null
            ? supportRect.anchoredPosition : Vector2.zero;
        Vector3 supportStartScale = supportRect != null
            ? supportRect.localScale : Vector3.one;
        RectTransform supportBackRect = overviewSupportBackgroundRect;
        AspectRatioFitter supportBackFitter = supportBackRect != null
            ? supportBackRect.GetComponent<AspectRatioFitter>() : null;
        if (supportBackFitter != null)
            supportBackFitter.enabled = false;
        Vector2 supportBackStartPosition = supportBackRect != null
            ? supportBackRect.anchoredPosition : Vector2.zero;
        Vector3 supportBackStartScale = supportBackRect != null
            ? supportBackRect.localScale : Vector3.one;

        if (machinePanel != null)
            machinePanel.SelectZoneFromCube(GetContentZoneForFace(_currentSectorIndex));
        else
            RefreshNow();

        if (overviewRoot != null)
            overviewRoot.SetActive(true);
        if (sectorRoot != null)
            sectorRoot.SetActive(true);
        if (overviewLabBackgroundRoot != null)
            overviewLabBackgroundRoot.SetActive(true);
        if (closeLabBackgroundRoot != null)
            closeLabBackgroundRoot.SetActive(true);
        if (overviewCanvasGroup != null)
            overviewCanvasGroup.alpha = 1f;
        if (sectorCanvasGroup != null)
            sectorCanvasGroup.alpha = 0f;
        if (nodeCardCanvasGroup != null)
            nodeCardCanvasGroup.alpha = 0f;
        if (overviewLabBackgroundGroup != null)
            overviewLabBackgroundGroup.alpha = 1f;
        if (closeLabBackgroundGroup != null)
            closeLabBackgroundGroup.alpha = 0f;
        if (transitionVeilGroup != null)
            transitionVeilGroup.alpha = 0f;
        if (overviewSupportForegroundRoot != null)
            overviewSupportForegroundRoot.SetActive(true);
        if (overviewSupportBackgroundRoot != null)
            overviewSupportBackgroundRoot.SetActive(true);
        if (overviewSupportForegroundGroup != null)
            overviewSupportForegroundGroup.alpha = 1f;
        if (overviewSupportBackgroundGroup != null)
            overviewSupportBackgroundGroup.alpha = 1f;

        // La geometria final queda resuelta antes del primer fotograma visible.
        // Asi la cara, el fondo y la ficha inferior comparten una sola llegada
        // y no hay un segundo cambio de tamaño al terminar la interpolacion.
        float viewportTargetMinY = sectorIndex == 3 ? .035f : .225f;
        float viewportStartMinY = viewportRect != null
            ? viewportRect.anchorMin.y : viewportTargetMinY;
        if (nodeCardRoot != null)
            nodeCardRoot.SetActive(sectorIndex != 3);

        Canvas.ForceUpdateCanvases();
        yield return null;

        RectTransform sectorRect = sectorArtwork != null
            ? sectorArtwork.rectTransform : null;
        Vector3 sectorEndScale = sectorRect != null
            ? sectorRect.localScale : Vector3.one;
        Vector3 sectorStartScale = new Vector3(
            sectorEndScale.x * .92f, sectorEndScale.y * .92f, 1f);
        float stageHorizontalOffset = GetSectorStageHorizontalOffset(
            sectorIndex, GetSectorStage(sectorIndex));
        RectTransform viewportParent = viewportRect != null
            ? viewportRect.parent as RectTransform : null;
        float targetViewportHeight = viewportRect != null && viewportParent != null
            ? viewportParent.rect.height *
                (viewportRect.anchorMax.y - viewportTargetMinY)
            : (viewportRect != null ? viewportRect.rect.height : 0f);
        float targetFloorOffset = targetViewportHeight *
            SectorFloorAlignmentRatio;
        float targetHorizontalOffset = sectorRect != null
            ? sectorRect.rect.width * sectorEndScale.x * stageHorizontalOffset
            : 0f;
        Vector2 sectorEndPosition = new Vector2(
            targetHorizontalOffset, targetFloorOffset);
        Vector2 sectorStartPosition = sectorEndPosition + new Vector2(
            0f, -targetViewportHeight * .018f);
        if (sectorRect != null)
        {
            sectorRect.localScale = sectorStartScale;
            sectorRect.anchoredPosition = sectorStartPosition;
        }
        RectTransform closeLabRect = closeLabBackgroundRoot != null
            ? closeLabBackgroundRoot.GetComponent<RectTransform>() : null;
        Vector2 closeLabEndPosition = closeLabRect != null
            ? closeLabRect.anchoredPosition : Vector2.zero;
        Vector3 closeLabEndScale = closeLabRect != null
            ? closeLabRect.localScale : Vector3.one;
        Vector2 closeLabStartPosition = closeLabEndPosition;
        Vector3 closeLabStartScale = closeLabEndScale;
        if (closeLabRect != null)
        {
            closeLabRect.anchoredPosition = closeLabStartPosition;
            closeLabRect.localScale = closeLabStartScale;
        }
        double startedAt = Time.realtimeSinceStartupAsDouble;
        float elapsed = 0f;
        while (elapsed < SectorTransitionDuration)
        {
            // Wall time must not slow down when frames are dropped on a mobile device.
            elapsed = (float)(Time.realtimeSinceStartupAsDouble - startedAt);
            float progress = Mathf.Clamp01(elapsed / SectorTransitionDuration);
#if UNITY_EDITOR
            if (EditorTransitionProgressOverride.HasValue)
            {
                progress = Mathf.Clamp01(EditorTransitionProgressOverride.Value);
                elapsed = progress * SectorTransitionDuration;
            }
#endif
            float handoff = progress;
            float viewBlend = Smooth01(
                Mathf.InverseLerp(.12f, .82f, handoff));
            float sectorMotion = Smooth01(
                Mathf.InverseLerp(.08f, .92f, handoff));
            float closeLabMotion = Smooth01(
                Mathf.InverseLerp(.08f, .92f, handoff));
            float viewportMotion = Smooth01(
                Mathf.InverseLerp(.12f, .92f, handoff));
            _transitionProgress = progress;
            if (viewportRect != null)
            {
                Vector2 minimum = viewportRect.anchorMin;
                minimum.y = Mathf.LerpUnclamped(
                    viewportStartMinY, viewportTargetMinY, viewportMotion);
                viewportRect.anchorMin = minimum;
                viewportRect.offsetMin = Vector2.zero;
            }
            if (approachRect != null)
            {
                approachRect.localScale = approachStartScale;
                approachRect.anchoredPosition = approachStartPosition;
            }
            if (artRect != null)
            {
                artRect.localScale = overviewRestScale;
                artRect.anchoredPosition = overviewRestPosition;
            }
            if (labRect != null)
            {
                labRect.localScale = labStartScale;
                labRect.anchoredPosition = labStartPosition;
            }
            if (supportRect != null)
            {
                supportRect.localScale = supportStartScale;
                supportRect.anchoredPosition = supportStartPosition;
            }
            if (supportBackRect != null)
            {
                supportBackRect.localScale = supportBackStartScale;
                supportBackRect.anchoredPosition = supportBackStartPosition;
            }
            if (sectorRect != null)
            {
                sectorRect.localScale = Vector3.LerpUnclamped(
                    sectorStartScale, sectorEndScale, sectorMotion);
                sectorRect.anchoredPosition = Vector2.LerpUnclamped(
                    sectorStartPosition, sectorEndPosition, sectorMotion);
            }
            if (closeLabRect != null)
            {
                closeLabRect.localScale = Vector3.LerpUnclamped(
                    closeLabStartScale, closeLabEndScale, closeLabMotion);
                closeLabRect.anchoredPosition = Vector2.LerpUnclamped(
                    closeLabStartPosition, closeLabEndPosition, closeLabMotion);
            }
            if (overviewCanvasGroup != null)
                overviewCanvasGroup.alpha = 1f - viewBlend;
            if (overviewLabBackgroundGroup != null)
                overviewLabBackgroundGroup.alpha = 1f;
            if (closeLabBackgroundGroup != null)
                closeLabBackgroundGroup.alpha = viewBlend;
            if (sectorCanvasGroup != null)
                sectorCanvasGroup.alpha = viewBlend;
            if (nodeCardCanvasGroup != null)
                nodeCardCanvasGroup.alpha = Smooth01(
                    Mathf.InverseLerp(.52f, .92f, handoff));
            if (overviewSupportForegroundGroup != null)
                overviewSupportForegroundGroup.alpha = 1f -
                    Smooth01(Mathf.InverseLerp(.35f, .62f, handoff));
            if (overviewSupportBackgroundGroup != null)
                overviewSupportBackgroundGroup.alpha = 1f -
                    Smooth01(Mathf.InverseLerp(.35f, .62f, handoff));
            if (transitionVeilGroup != null)
                transitionVeilGroup.alpha = Mathf.Sin(handoff * Mathf.PI) * .08f;
            yield return null;
        }

        _showingSector = true;
        _transitionProgress = 1f;
        _transitioning = false;
        if (overviewFitter != null)
            overviewFitter.enabled = true;
        if (supportFitter != null)
            supportFitter.enabled = true;
        if (supportBackFitter != null)
            supportBackFitter.enabled = true;
        ResetTransitionVisuals();
        RefreshNow();
        if (nodeCardCanvasGroup != null && nodeCardRoot != null &&
            nodeCardRoot.activeSelf)
            nodeCardCanvasGroup.alpha = 1f;
        _transitionRoutine = null;
        SetSectorButtonsInteractable(true);
    }

    private void SelectDisplayNode(int displayIndex)
    {
        if (!_showingSector || _currentSectorIndex == 3 || MachineManager.I == null ||
            machinePanel == null)
            return;

        List<MachineNodeDef> nodes = GetDisplayNodesForFace(
            _currentSectorIndex, false);
        if (nodes == null || displayIndex < 0 || displayIndex >= nodes.Count ||
            nodes[displayIndex] == null)
            return;

        MachineNodeDef selectedNode = nodes[displayIndex];
        machinePanel.SelectZoneFromCube(selectedNode.zone);
        machinePanel.SelectNodeFromCube(selectedNode.id);
    }

    private void RefreshHeader()
    {
        double total = MachineManager.I.GetTotalMachineRepairProgress01();
        if (GameState.I != null)
        {
            SetText(leResourceText, "LE  " + FormatNumber(GameState.I.LE));
            SetText(tracesResourceText, "TRAZAS  " + FormatNumber(GameState.I.Traces));
        }

        SetText(globalProgressText, $"REPARACIÓN TOTAL  {total * 100.0:0}%  /  80%");
        if (convergenceText != null)
            convergenceText.gameObject.SetActive(false);

        if (globalProgressFill != null)
            globalProgressFill.fillAmount = Mathf.Clamp01((float)(total / .8));

        if (!_showingSector)
        {
            SetText(viewTitleText, "MONOLITO");
            SetText(viewIndexText, string.Empty);
            return;
        }

        SetText(viewTitleText, GetSectorTitle(_currentSectorIndex));
        SetText(viewIndexText, $"CARA {_currentSectorIndex + 1} / 4");
    }

    private void RefreshArtwork()
    {
        int sectorStage = GetSectorStage(_currentSectorIndex);
        int overviewStage = GetOverviewStage();
        SetSupportFootprintStage(overviewStage);
        bool showApprovedCloseBackground = _showingSector || _transitioning;
        bool showOverviewBackground = !_showingSector || _transitioning;
        if (overviewLabBackgroundRoot != null)
            overviewLabBackgroundRoot.SetActive(showOverviewBackground);
        if (closeLabBackgroundRoot != null)
            closeLabBackgroundRoot.SetActive(showApprovedCloseBackground);
        if (overviewSupportForegroundRoot != null)
            overviewSupportForegroundRoot.SetActive(showOverviewBackground);
        if (overviewSupportBackgroundRoot != null)
            overviewSupportBackgroundRoot.SetActive(showOverviewBackground);

        if (overviewRoot != null)
            overviewRoot.SetActive(!_showingSector || _transitioning);
        if (sectorRoot != null)
            sectorRoot.SetActive(_showingSector || _transitioning);
        if (backToOverviewButton != null)
            backToOverviewButton.gameObject.SetActive(_showingSector && !_transitioning);
        if (nodeCardRoot != null)
            nodeCardRoot.SetActive(_showingSector && !_transitioning &&
                _currentSectorIndex != 3);
        if (viewportRect != null && !_transitioning)
        {
            Vector2 minimum = viewportRect.anchorMin;
            minimum.y = _showingSector && _currentSectorIndex != 3 ? .225f : .035f;
            viewportRect.anchorMin = minimum;
            viewportRect.offsetMin = Vector2.zero;
        }
        if (sectorArtwork != null &&
            (!_transitioning || _transitionProgress <= 0f))
        {
            RectTransform sectorRect = sectorArtwork.rectTransform;
            float sectorWidthScale = GetSectorCloseWidthScale(_currentSectorIndex);
            sectorRect.localScale = new Vector3(sectorWidthScale,
                SectorCloseScale, 1f);
            float floorOffset = viewportRect != null
                ? viewportRect.rect.height * SectorFloorAlignmentRatio
                : 0f;
            float horizontalOffset = sectorRect.rect.width *
                sectorWidthScale * GetSectorStageHorizontalOffset(
                    _currentSectorIndex, sectorStage);
            sectorRect.anchoredPosition = new Vector2(horizontalOffset, floorOffset);
        }

        if (!_transitioning)
        {
            if (overviewCanvasGroup != null)
                overviewCanvasGroup.alpha = 1f;
            if (sectorCanvasGroup != null)
                sectorCanvasGroup.alpha = 1f;
            if (overviewLabBackgroundGroup != null)
                overviewLabBackgroundGroup.alpha = 1f;
            if (closeLabBackgroundGroup != null)
                closeLabBackgroundGroup.alpha = 1f;
            if (overviewSupportForegroundGroup != null)
                overviewSupportForegroundGroup.alpha = 1f;
            if (overviewSupportBackgroundGroup != null)
                overviewSupportBackgroundGroup.alpha = 1f;
            if (nodeCardCanvasGroup != null && _transitionRoutine == null)
                nodeCardCanvasGroup.alpha = _showingSector ? 1f : 0f;
        }

        RefreshOverviewEntryLights();

        if (!_showingSector && !_transitioning)
        {
            if (overviewArtwork != null)
            {
                overviewArtwork.texture = overviewProgressionSheet;
                overviewArtwork.uvRect = OverviewUvRects[overviewStage];
            }
            return;
        }

        if (sectorArtwork == null || sectorProgressionSheets == null ||
            _currentSectorIndex >= sectorProgressionSheets.Length)
            return;

        sectorArtwork.texture = sectorProgressionSheets[_currentSectorIndex];
        Rect artworkUv = ProgressionUvRects[sectorStage];
        sectorArtwork.uvRect = artworkUv;
        sectorArtwork.color = _currentSectorIndex == 3
            ? new Color(.68f, .71f, .74f, 1f)
            : Color.white;
        if (reservedFaceMarker != null)
            reservedFaceMarker.gameObject.SetActive(_currentSectorIndex == 3);
    }

    private void RefreshNodeHotspots()
    {
        if (nodeButtons == null)
            return;

        List<MachineNodeDef> nodes = null;
        bool sectorContentVisible = (_showingSector || _transitioning) &&
            _currentSectorIndex != 3;
        if (sectorContentVisible && MachineManager.I != null)
            nodes = GetDisplayNodesForFace(_currentSectorIndex, false);

        MachineNodeDef selected = machinePanel != null && MachineManager.I != null
            ? MachineManager.I.GetDef(machinePanel.SelectedNodeId)
            : null;

        for (int i = 0; i < nodeButtons.Length; i++)
        {
            Vector2[] positions = GetFaceDisplayPositions(_currentSectorIndex);
            bool visible = nodes != null && i < nodes.Count && i < positions.Length &&
                nodes[i] != null;
            if (nodeButtons[i] != null)
            {
                nodeButtons[i].gameObject.SetActive(visible);
                if (visible && nodeButtons[i].transform is RectTransform rect)
                {
                    Vector2 displayPosition = GetNodeDisplayPosition(
                        _currentSectorIndex, i, positions[i]);
                    rect.anchorMin = displayPosition;
                    rect.anchorMax = displayPosition;
                    rect.anchoredPosition = Vector2.zero;
                    nodeButtons[i].interactable = _showingSector && !_transitioning;
                }
            }

            if (nodeSymbolImages == null || i >= nodeSymbolImages.Length ||
                nodeSymbolImages[i] == null)
                continue;

            RawImage symbol = nodeSymbolImages[i];
            symbol.gameObject.SetActive(visible);
            if (!visible)
                continue;

            bool repaired = MachineManager.I.IsNodeRepaired(nodes[i].id);
            bool selectedVisual = IsSameVisualBranch(nodes[i], selected);
            symbol.rectTransform.sizeDelta = Vector2.one *
                (repaired ? RepairedNodeDisplaySize : NodeSymbolDisplaySize);
            symbol.texture = sectorSymbolSheets != null &&
                sectorSymbolSheets.Length > 0 && sectorSymbolSheets[0] != null
                ? sectorSymbolSheets[0]
                : sectorProgressionSheets[0];
            symbol.uvRect = GetCanonicalSymbolUvRect(nodes[i]);
            float alpha = repaired
                ? RepairedNodeAlpha
                : selectedVisual ? SelectedDamagedNodeAlpha : DamagedNodeAlpha;
            symbol.color = repaired
                ? new Color(.78f, 1f, 1f, alpha)
                : new Color(1f, 1f, 1f, alpha);
        }
    }

    private void RefreshSelectedCard()
    {
        MachineNodeDef node = machinePanel != null && MachineManager.I != null
            ? MachineManager.I.GetDef(machinePanel.SelectedNodeId)
            : null;
        if (node == null)
        {
            SetText(selectedIconText, "--");
            if (selectedIconImage != null)
                selectedIconImage.enabled = false;
            SetText(selectedNameText, "SELECCIONA UN NODO");
            SetText(selectedStateText, "ESTADO  •  DESCONOCIDO");
            SetText(selectedDescriptionText, "Explora las firmas de la cara actual.");
            SetText(selectedEffectText, "EFECTO  —");
            SetText(selectedRequirementsText, "REQUISITOS  —");
            SetText(selectedCostText, "COSTE  —");
            SetText(selectedSectorProgressText, "PROGRESO DEL SECTOR  —");
            return;
        }

        bool repaired = MachineManager.I.IsNodeRepaired(node.id);
        bool analyzed = MachineManager.I.IsNodeAnalyzed(node.id);
        bool analyzing = MachineManager.I.IsAnalyzingNode &&
            (MachineManager.I.AnalysisNodeId == node.id ||
             (!string.IsNullOrWhiteSpace(node.tierGroup) &&
              MachineManager.I.AnalysisNodeId == "tierGroup:" + node.tierGroup));
        bool concealed = node.damaged && !repaired && !analyzed;

        string glyph = MachineCubeVisualUI.GetEffectGlyph(node);
        SetText(selectedIconText, concealed ? "?" : glyph);
        if (selectedIconImage != null)
        {
            selectedIconImage.sprite = concealed ? iconSynthesis : ResolveEffectIcon(glyph);
            selectedIconImage.enabled = selectedIconImage.sprite != null;
            selectedIconImage.color = concealed
                ? new Color(.48f, .62f, .68f, .72f)
                : Color.white;
        }

        SetText(selectedNameText, concealed ? "NODO DAÑADO" : node.name.ToUpperInvariant());
        SetText(selectedStateText, "NODO SELECCIONADO  •  " +
            (repaired ? "REPARADO" : analyzing ? "ANALIZANDO" :
             concealed ? "DAÑADO" : analyzed ? "ANALIZADO" : "PENDIENTE"));

        if (concealed)
        {
            SetText(selectedDescriptionText,
                "Nodo dañado. Pulsa ANALIZAR para revelar su función y sus requisitos de reparación.");
            SetText(selectedEffectText, "EFECTO  —");
            SetText(selectedRequirementsText, "REQUISITOS  —");
            SetText(selectedCostText, "COSTE  —");
        }
        else
        {
            SetText(selectedDescriptionText, node.description);
            SetText(selectedEffectText, "EFECTO  " + FormatEffect(node));
            SetText(selectedRequirementsText, "REQUISITOS  " + FormatRequirements(node));
            SetText(selectedCostText,
                "COSTE  " + FormatCostWithAvailability(
                    MachineManager.I.GetEffectiveNodeCost(node)));
        }

        List<MachineNodeDef> sectorNodes = GetDisplayNodesForFace(
            _currentSectorIndex, true);
        int repairedCount = 0;
        int visualTotal = sectorNodes != null
            ? sectorNodes.Count
            : GetFaceDisplayNodeCount(_currentSectorIndex);
        if (sectorNodes != null)
        {
            for (int i = 0; i < sectorNodes.Count && i < visualTotal; i++)
                if (sectorNodes[i] != null &&
                    MachineManager.I.IsNodeRepaired(sectorNodes[i].id))
                    repairedCount++;
        }
        SetText(selectedSectorProgressText,
            $"PROGRESO DE LA CARA  {repairedCount}/{visualTotal}");
    }

    private void RefreshOverviewEntryLights()
    {
        if (overviewEntryLights == null)
            return;
        int stage = GetOverviewStage();
        for (int i = 0; i < overviewEntryLights.Length; i++)
        {
            if (overviewEntryLights[i] == null)
                continue;
            GameObject light = overviewEntryLights[i];
            if (overviewArtwork != null && light.transform is RectTransform rect)
            {
                RectTransform expectedParent = overviewArtwork.rectTransform;
                if (rect.parent != expectedParent)
                    rect.SetParent(expectedParent, false);
                Vector2 canonicalCenter = GetOverviewEntryLightAnchor(i, stage);
                rect.anchorMin = canonicalCenter;
                rect.anchorMax = canonicalCenter;
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = OverviewEntryLightSizes[stage];
                rect.localRotation = Quaternion.Euler(
                    0f, 0f, OverviewEntryLightRotations[Mathf.Clamp(i, 0, 3)]);
                rect.localScale = Vector3.one;
            }
            RawImage[] emitters = light.GetComponentsInChildren<RawImage>(true);
            for (int emitterIndex = 0; emitterIndex < emitters.Length;
                emitterIndex++)
            {
                float strength = emitterIndex == 1 ? 1f : .78f;
                emitters[emitterIndex].color = new Color(.68f, .92f, 1f,
                    OverviewEntryLightAlphas[stage] * strength);
            }
            light.SetActive(!_showingSector && i < 4);
        }
    }

    private void SetSectorButtonsInteractable(bool enabled)
    {
        if (sectorButtons == null)
            return;
        for (int i = 0; i < sectorButtons.Length; i++)
        {
            if (sectorButtons[i] != null)
                sectorButtons[i].interactable = enabled && i < 4;
        }
    }

    private void ResetTransitionVisuals()
    {
        if (overviewRoot != null &&
            overviewRoot.transform is RectTransform overviewRect)
        {
            overviewRect.localScale = Vector3.one;
            overviewRect.anchoredPosition = Vector2.zero;
        }
        if (overviewArtwork != null)
        {
            AspectRatioFitter fitter =
                overviewArtwork.GetComponent<AspectRatioFitter>();
            if (fitter != null)
                fitter.enabled = true;
            overviewArtwork.rectTransform.localScale = overviewRestScale;
            overviewArtwork.rectTransform.anchoredPosition = overviewRestPosition;
        }
        if (overviewLabBackgroundRect != null)
        {
            overviewLabBackgroundRect.localScale = overviewLabBackgroundRestScale;
            overviewLabBackgroundRect.anchoredPosition =
                overviewLabBackgroundRestPosition;
        }
        if (closeLabBackgroundRoot != null &&
            closeLabBackgroundRoot.transform is RectTransform closeLabRect)
        {
            closeLabRect.localScale = Vector3.one;
            closeLabRect.anchoredPosition = Vector2.zero;
        }
        if (overviewSupportForegroundRect != null)
        {
            AspectRatioFitter fitter =
                overviewSupportForegroundRect.GetComponent<AspectRatioFitter>();
            if (fitter != null)
                fitter.enabled = true;
            overviewSupportForegroundRect.localScale =
                overviewSupportForegroundRestScale;
            overviewSupportForegroundRect.anchoredPosition =
                overviewSupportForegroundRestPosition;
        }
        if (overviewSupportBackgroundRect != null)
        {
            AspectRatioFitter fitter =
                overviewSupportBackgroundRect.GetComponent<AspectRatioFitter>();
            if (fitter != null)
                fitter.enabled = true;
            overviewSupportBackgroundRect.localScale =
                overviewSupportBackgroundRestScale;
            overviewSupportBackgroundRect.anchoredPosition =
                overviewSupportBackgroundRestPosition;
        }
        if (overviewCanvasGroup != null)
            overviewCanvasGroup.alpha = 1f;
        if (sectorCanvasGroup != null)
            sectorCanvasGroup.alpha = 1f;
        if (overviewLabBackgroundGroup != null)
            overviewLabBackgroundGroup.alpha = 1f;
        if (closeLabBackgroundGroup != null)
            closeLabBackgroundGroup.alpha = 1f;
        if (overviewSupportForegroundGroup != null)
            overviewSupportForegroundGroup.alpha = 1f;
        if (overviewSupportBackgroundGroup != null)
            overviewSupportBackgroundGroup.alpha = 1f;
        if (transitionVeilGroup != null)
            transitionVeilGroup.alpha = 0f;
        if (overviewLabBackgroundRoot != null)
            overviewLabBackgroundRoot.SetActive(!_showingSector);
        if (closeLabBackgroundRoot != null)
            closeLabBackgroundRoot.SetActive(_showingSector);
        if (overviewRoot != null)
            overviewRoot.SetActive(!_showingSector);
        if (sectorRoot != null)
            sectorRoot.SetActive(_showingSector);
        if (overviewSupportForegroundRoot != null)
            overviewSupportForegroundRoot.SetActive(!_showingSector);
        if (overviewSupportBackgroundRoot != null)
            overviewSupportBackgroundRoot.SetActive(!_showingSector);
    }

    public static Vector2 GetNodeDisplayPosition(int sectorIndex, int nodeIndex,
        Vector2 sourcePosition)
    {
        Vector2[] positions = GetFaceDisplayPositions(sectorIndex);
        if (nodeIndex >= 0 && nodeIndex < positions.Length)
            return positions[nodeIndex];
        return sourcePosition;
    }

    public static MachineZoneType GetContentZoneForFace(int faceIndex)
    {
        faceIndex = Mathf.Clamp(faceIndex, 0, FaceContentZones.Length - 1);
        return FaceContentZones[faceIndex];
    }

    public static int GetFaceDisplayNodeCount(int faceIndex)
    {
        if (faceIndex < 0 || faceIndex >= FaceBranchKeys.Length)
            return 0;
        return FaceBranchKeys[faceIndex].Length;
    }

    public static int GetFaceVisibleNodeCount(int faceIndex, int repairStage)
    {
        int total = GetFaceDisplayNodeCount(faceIndex);
        if (total <= 0)
            return 0;
        int[] stagedCounts = { 3, 5, 7, 8 };
        repairStage = Mathf.Clamp(repairStage, 0, stagedCounts.Length - 1);
        return Math.Min(total, stagedCounts[repairStage]);
    }

    public static bool IsBranchVisibleAtStage(string branchKey, int repairStage)
    {
        if (string.IsNullOrWhiteSpace(branchKey))
            return false;
        for (int face = 0; face < 3; face++)
        {
            int visible = GetFaceVisibleNodeCount(face, repairStage);
            for (int index = 0; index < visible; index++)
                if (FaceBranchKeys[face][index] == branchKey)
                    return true;
        }
        return false;
    }

    private static List<MachineNodeDef> GetDisplayNodesForFace(int faceIndex,
        bool includeAll)
    {
        var result = new List<MachineNodeDef>();
        if (MachineManager.I == null || faceIndex < 0 ||
            faceIndex >= FaceBranchKeys.Length)
            return result;

        var byBranch = new Dictionary<string, MachineNodeDef>();
        foreach (MachineZoneType zone in new[]
        {
            MachineZoneType.Room1Link,
            MachineZoneType.FusionSector,
            MachineZoneType.InternalSupport
        })
        {
            List<MachineNodeDef> zoneNodes = MachineManager.I.GetDisplayNodesByZone(zone);
            if (zoneNodes == null) continue;
            foreach (MachineNodeDef node in zoneNodes)
            {
                if (node == null) continue;
                string key = string.IsNullOrWhiteSpace(node.tierGroup)
                    ? node.id
                    : node.tierGroup;
                byBranch[key] = node;
            }
        }

        int visibleCount = includeAll
            ? GetFaceDisplayNodeCount(faceIndex)
            : GetFaceVisibleNodeCount(faceIndex,
                MachineManager.I != null
                    ? GetRepairStage(MachineManager.I.GetTotalMachineRepairProgress01())
                    : 0);
        for (int index = 0; index < visibleCount &&
            index < FaceBranchKeys[faceIndex].Length; index++)
        {
            string key = FaceBranchKeys[faceIndex][index];
            if (byBranch.TryGetValue(key, out MachineNodeDef node))
                result.Add(node);
        }
        return result;
    }

    private static Vector2[] GetFaceDisplayPositions(int faceIndex)
    {
        return faceIndex switch
        {
            0 => Face1DisplayPositions,
            1 => Face2DisplayPositions,
            2 => Face3DisplayPositions,
            _ => Array.Empty<Vector2>()
        };
    }

    public static bool IsNodeCenterInsideDarkSurface(int sectorIndex, Vector2 center)
    {
        if (center.y < .04f || center.y > .90f)
            return false;

        if (sectorIndex < 0 || sectorIndex >= FaceDarkSurfacePolygons.Length)
            return false;

        Vector2[] polygon = FaceDarkSurfacePolygons[sectorIndex];
        bool? positiveWinding = null;
        for (int i = 0; i < polygon.Length; i++)
        {
            Vector2 edge = polygon[(i + 1) % polygon.Length] - polygon[i];
            Vector2 relative = center - polygon[i];
            float cross = edge.x * relative.y - edge.y * relative.x;
            if (Mathf.Abs(cross) <= .0001f)
                continue;

            bool positive = cross > 0f;
            if (positiveWinding.HasValue && positiveWinding.Value != positive)
                return false;
            positiveWinding = positive;
        }

        return positiveWinding.HasValue;
    }

    public static bool IsNodeFootprintInsideDarkSurface(int sectorIndex,
        Vector2 center, Vector2 halfExtent)
    {
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                Vector2 sample = center + new Vector2(
                    halfExtent.x * x, halfExtent.y * y);
                if (!IsNodeCenterInsideDarkSurface(sectorIndex, sample))
                    return false;
            }
        }

        return true;
    }

    public static bool AreNodeCentersSeparated(Vector2[] centers)
    {
        if (centers == null)
            return false;

        for (int i = 0; i < centers.Length; i++)
        {
            for (int j = i + 1; j < centers.Length; j++)
            {
                if (Vector2.Distance(centers[i], centers[j]) <
                    NodeMinimumNormalizedSeparation)
                    return false;
            }
        }

        return true;
    }

    public static bool AreNodePlacementsIrregular(Vector2[] centers)
    {
        if (!AreNodeCentersSeparated(centers))
            return false;

        const float obviousAxisTolerance = .006f;
        for (int i = 0; i < centers.Length; i++)
        {
            for (int j = i + 1; j < centers.Length; j++)
            {
                for (int k = j + 1; k < centers.Length; k++)
                {
                    float minX = Mathf.Min(centers[i].x,
                        Mathf.Min(centers[j].x, centers[k].x));
                    float maxX = Mathf.Max(centers[i].x,
                        Mathf.Max(centers[j].x, centers[k].x));
                    float minY = Mathf.Min(centers[i].y,
                        Mathf.Min(centers[j].y, centers[k].y));
                    float maxY = Mathf.Max(centers[i].y,
                        Mathf.Max(centers[j].y, centers[k].y));
                    if (maxX - minX <= obviousAxisTolerance ||
                        maxY - minY <= obviousAxisTolerance)
                        return false;
                }
            }
        }

        return true;
    }

    public static Rect GetCanonicalSymbolUvRect(int sectorIndex, int nodeIndex)
    {
        MachineZoneType zone = GetContentZoneForFace(sectorIndex);
        int contentIndex = Mathf.Clamp((int)zone - 1, 0,
            CanonicalSymbolIndices.Length - 1);
        int[] indices = CanonicalSymbolIndices[contentIndex];
        if (indices == null || indices.Length == 0 || NodePositions[0].Length == 0)
            return Rect.zero;

        nodeIndex = Mathf.Clamp(nodeIndex, 0, indices.Length - 1);
        int canonicalIndex = Mathf.Clamp(indices[nodeIndex], 0,
            NodePositions[0].Length - 1);
        return GetFinalSymbolUvRect(NodePositions[0][canonicalIndex]);
    }

    private static Rect GetCanonicalSymbolUvRect(MachineNodeDef node)
    {
        if (node == null || MachineManager.I == null)
            return Rect.zero;
        List<MachineNodeDef> zoneNodes = MachineManager.I.GetDisplayNodesByZone(node.zone);
        int localIndex = zoneNodes != null
            ? zoneNodes.FindIndex(candidate => candidate != null &&
                (candidate.id == node.id ||
                 (!string.IsNullOrWhiteSpace(candidate.tierGroup) &&
                  candidate.tierGroup == node.tierGroup)))
            : -1;
        int contentIndex = Mathf.Clamp((int)node.zone - 1, 0,
            CanonicalSymbolIndices.Length - 1);
        int[] indices = CanonicalSymbolIndices[contentIndex];
        if (localIndex < 0 || indices == null || indices.Length == 0 ||
            NodePositions[0].Length == 0)
            return Rect.zero;
        int canonicalIndex = Mathf.Clamp(indices[localIndex % indices.Length], 0,
            NodePositions[0].Length - 1);
        return GetFinalSymbolUvRect(NodePositions[0][canonicalIndex]);
    }

    private void SetSupportFootprintStage(int stage)
    {
        SetSupportFootprintStage(overviewSupportBackgroundRoot, stage);
        SetSupportFootprintStage(overviewSupportForegroundRoot, stage);
    }

    private static void SetSupportFootprintStage(GameObject root, int stage)
    {
        RawImage image = root != null ? root.GetComponent<RawImage>() : null;
        if (image != null && image.material != null &&
            image.material.HasProperty("_FootprintStage"))
            image.material.SetFloat("_FootprintStage", Mathf.Clamp(stage, 0, 3));
    }

    private static Rect GetFinalSymbolUvRect(Vector2 position)
    {
        const float normalizedWidth = NodeSymbolUvWidth;
        const float normalizedHeight = NodeSymbolUvHeight;
        return new Rect(
            .5f + (position.x - normalizedWidth * .5f) * .5f,
            (position.y - normalizedHeight * .5f) * .5f,
            normalizedWidth * .5f,
            normalizedHeight * .5f);
    }

    public static Vector2 GetOverviewFaceCenter(int sectorIndex)
    {
        return sectorIndex switch
        {
            0 => new Vector2(.390f, .710f),
            1 => new Vector2(.690f, .710f),
            2 => new Vector2(.725f, .360f),
            _ => new Vector2(.325f, .360f)
        };
    }

    public static Vector2 GetOverviewEntryLightAnchor(int faceIndex, int stage)
    {
        faceIndex = Mathf.Clamp(faceIndex, 0, OverviewEntryLightAnchors.Length - 1);
        stage = Mathf.Clamp(stage, 0, OverviewEntryLightAnchors[faceIndex].Length - 1);
        return OverviewEntryLightAnchors[faceIndex][stage];
    }

    public static Vector2 GetOverviewEntryLightSize(int stage)
    {
        return OverviewEntryLightSizes[Mathf.Clamp(stage, 0,
            OverviewEntryLightSizes.Length - 1)];
    }

    public static float GetOverviewEntryLightRotation(int faceIndex)
    {
        return OverviewEntryLightRotations[Mathf.Clamp(faceIndex, 0,
            OverviewEntryLightRotations.Length - 1)];
    }

    public static float GetSectorStageHorizontalOffset(int sectorIndex, int stage)
    {
        sectorIndex = Mathf.Clamp(sectorIndex, 0,
            SectorStageHorizontalOffsets.Length - 1);
        float[] offsets = SectorStageHorizontalOffsets[sectorIndex];
        stage = Mathf.Clamp(stage, 0, offsets.Length - 1);
        return offsets[stage];
    }

    public static float GetSectorCloseWidthScale(int sectorIndex)
    {
        sectorIndex = Mathf.Clamp(sectorIndex, 0, SectorWidthMultipliers.Length - 1);
        return SectorCloseWidthScale * SectorWidthMultipliers[sectorIndex];
    }

    private static float Smooth01(float value)
    {
        value = Mathf.Clamp01(value);
        return value * value * (3f - 2f * value);
    }

    private int GetOverviewStage()
    {
        double progress = MachineManager.I != null
            ? MachineManager.I.GetTotalMachineRepairProgress01()
            : 0.0;
        return GetRepairStage(progress);
    }

    public static int GetRepairStage(double progress)
    {
        if (progress < .20)
            return 0;
        if (progress < .40)
            return 1;
        if (progress < .60)
            return 2;
        return 3;
    }

    private int GetSectorStage(int sectorIndex)
    {
        return GetOverviewStage();
    }

    private static bool IsSameVisualBranch(MachineNodeDef shown, MachineNodeDef selected)
    {
        if (shown == null || selected == null)
            return false;
        if (shown.id == selected.id)
            return true;
        return !string.IsNullOrWhiteSpace(shown.tierGroup) &&
            shown.tierGroup == selected.tierGroup;
    }

    private static string GetSectorTitle(int index)
    {
        return index switch
        {
            0 => "CARA 1",
            1 => "CARA 2",
            2 => "CARA 3",
            3 => string.Empty,
            _ => "MONOLITO"
        };
    }

    private static string FormatNumber(double value)
    {
        double absolute = Math.Abs(value);
        if (absolute >= 1e12)
            return (value / 1e12).ToString("0.##") + " T";
        if (absolute >= 1e9)
            return (value / 1e9).ToString("0.##") + " B";
        if (absolute >= 1e6)
            return (value / 1e6).ToString("0.##") + " M";
        if (absolute >= 1e3)
            return (value / 1e3).ToString("0.##") + " K";
        return value.ToString("0.##");
    }

    private Sprite ResolveEffectIcon(string glyph)
    {
        return glyph switch
        {
            "LE" => iconLe,
            "TR" => iconTraces,
            "TI" => iconTriangle,
            "EN" => iconTriangle,
            "AR" => iconArtifact,
            "FU" => iconFusion,
            "DX" => iconDiagnostic,
            "ST" => iconStructure,
            "CV" => iconConvergence,
            "AN" => iconAnchor,
            _ => iconSynthesis
        };
    }

    private static string FormatRequirements(MachineNodeDef node)
    {
        if (node.requiredNodeIds == null || node.requiredNodeIds.Count == 0)
            return "NINGUNO";

        StringBuilder builder = new StringBuilder();
        foreach (string id in node.requiredNodeIds)
        {
            if (builder.Length > 0)
                builder.Append("  ·  ");
            MachineNodeDef required = MachineManager.I.GetDef(id);
            bool repaired = MachineManager.I.IsNodeRepaired(id);
            if (!repaired)
                builder.Append("<color=#FF6B6B>");
            builder.Append(repaired ? "[OK] " : "[FALTA] ");
            builder.Append(required != null ? required.name : id);
            if (!repaired)
                builder.Append("</color>");
        }
        return builder.ToString().ToUpperInvariant();
    }

    private static string FormatCostWithAvailability(MachineNodeCostDef cost)
    {
        if (cost == null)
            return "SIN COSTE";

        GameState state = GameState.I;
        StringBuilder builder = new StringBuilder();
        AppendAvailableCost(builder, cost.le, state != null ? state.LE : 0.0,
            "LE", "LE");
        AppendAvailableCost(builder, cost.traces, state != null ? state.Traces : 0.0,
            "TRAZA", "TRAZAS");
        AppendAvailableCost(builder, cost.hallazgo,
            state != null ? state.experimentalHallazgos : 0,
            "ANOMALÍA", "ANOMALÍAS");
        AppendAvailableCost(builder, cost.muestra,
            state != null ? state.experimentalMuestras : 0,
            "CONDENSADO", "CONDENSADOS");
        AppendAvailableCost(builder, cost.lecturaIncompleta,
            state != null ? state.experimentalLecturasIncompletas : 0,
            "VESTIGIO", "VESTIGIOS");
        AppendAvailableCost(builder, cost.compuestoUtil,
            state != null ? state.experimentalCompuestosUtiles : 0,
            "COMPUESTO", "COMPUESTOS");
        AppendAvailableCost(builder, cost.pureInstant,
            state != null ? state.chronalPureInstants : 0,
            "ANCLAJE PURO", "ANCLAJES PUROS");
        AppendAvailableCost(builder, cost.stableInstant,
            state != null ? state.chronalStableInstants : 0,
            "ANCLAJE ESTABLE", "ANCLAJES ESTABLES");
        AppendAvailableCost(builder, cost.forcedInstant,
            state != null ? state.chronalForcedInstants : 0,
            "ANCLAJE FORZADO", "ANCLAJES FORZADOS");
        return builder.Length == 0 ? "SIN COSTE" : builder.ToString();
    }

    private static void AppendAvailableCost(StringBuilder builder, double required,
        double available, string singular, string plural)
    {
        if (required <= 0.0)
            return;
        if (builder.Length > 0)
            builder.Append("  ·  ");
        bool missing = available + .000001 < required;
        if (missing)
            builder.Append("<color=#FF6B6B>");
        builder.Append(FormatNumber(required)).Append(' ')
            .Append(Math.Abs(required - 1.0) < .000001 ? singular : plural);
        if (missing)
            builder.Append("</color>");
    }

    private static string FormatCost(MachineNodeCostDef cost)
    {
        if (cost == null)
            return "SIN COSTE";

        StringBuilder builder = new StringBuilder();
        AppendCost(builder, cost.le, "LE", "LE");
        AppendCost(builder, cost.traces, "TRAZA", "TRAZAS");
        AppendCost(builder, cost.hallazgo, "ANOMALÍA", "ANOMALÍAS");
        AppendCost(builder, cost.muestra, "CONDENSADO", "CONDENSADOS");
        AppendCost(builder, cost.lecturaIncompleta,
            "VESTIGIO", "VESTIGIOS");
        AppendCost(builder, cost.compuestoUtil,
            "COMPUESTO", "COMPUESTOS");
        AppendCost(builder, cost.pureInstant, "ANCLAJE PURO", "ANCLAJES PUROS");
        AppendCost(builder, cost.stableInstant, "ANCLAJE ESTABLE", "ANCLAJES ESTABLES");
        AppendCost(builder, cost.forcedInstant, "ANCLAJE FORZADO", "ANCLAJES FORZADOS");
        return builder.Length == 0 ? "SIN COSTE" : builder.ToString();
    }

    private static void AppendCost(StringBuilder builder, double value,
        string singular, string plural)
    {
        if (value <= 0.0)
            return;
        if (builder.Length > 0)
            builder.Append("  ·  ");
        builder.Append(FormatNumber(value)).Append(' ')
            .Append(Math.Abs(value - 1.0) < .000001 ? singular : plural);
    }

    private static string FormatEffect(MachineNodeDef node)
    {
        string value = node.effectValue > 0.0
            ? "  +" + node.effectValue.ToString("0.##") : "";
        string percent = node.effectValue > 0.0
            ? "  +" + (node.effectValue * 100.0).ToString("0.##") + "%" : "";
        string reduction = node.effectValue > 0.0
            ? "  -" + (node.effectValue * 100.0).ToString("0.##") + "%" : "";
        return node.effectType switch
        {
            MachineNodeEffectType.GlobalLEBonus => "PRODUCCIÓN GLOBAL DE LE" + percent,
            MachineNodeEffectType.TracesBonus => "GENERACIÓN DE TRAZAS" + percent,
            MachineNodeEffectType.TriangleBonus => "SINCRONIZACIÓN TRIANGULAR" + percent,
            MachineNodeEffectType.ArtifactBonus => "CALIBRACIÓN DE ARTEFACTOS" + percent,
            MachineNodeEffectType.Room1GlobalBonus => "SINCRONIZACIÓN DEL CUARTO 1" + percent,
            MachineNodeEffectType.TriangleEnergyBaseBonus =>
                "ENERGÍA BASE DEL TRIÁNGULO  +" + node.effectValue.ToString("0.##") + "/S",
            MachineNodeEffectType.UnlockFusionSlot =>
                "RANURAS DE FUSIÓN  " + node.effectValue.ToString("0"),
            MachineNodeEffectType.FusionFailureReduction => "RIESGO DE FALLO" + reduction,
            MachineNodeEffectType.FusionUsefulResultBonus =>
                "PROBABILIDAD DE RESULTADO ÚTIL" + percent,
            MachineNodeEffectType.FusionTimeReduction => "DURACIÓN DE FUSIÓN" + reduction,
            MachineNodeEffectType.RevealFusionProbabilities => "LECTURA DE COMPOSICIÓN",
            MachineNodeEffectType.CatalystTuning => "AFINACIÓN DE CATALIZADORES",
            MachineNodeEffectType.StableReactionChamber => "CÁMARA DE REACCIÓN ESTABLE",
            MachineNodeEffectType.GuidedSynthesis => "SÍNTESIS GUIADA",
            MachineNodeEffectType.SynthesisCore => "NÚCLEO DE SÍNTESIS",
            MachineNodeEffectType.UnlockDiagnostics => "DIAGNÓSTICO INTERNO",
            MachineNodeEffectType.RevealHiddenSubnodes => "REVELAR NODOS SECRETOS",
            MachineNodeEffectType.CompensationCircuit => "CIRCUITO DE COMPENSACIÓN",
            MachineNodeEffectType.UnlockMachineMemory => "MEMORIA DE LA MÁQUINA",
            MachineNodeEffectType.ZoneProgressSyncBonus => "SINCRONIZACIÓN POR SECTOR",
            MachineNodeEffectType.StructuralSupportBonus => "REFUERZO ESTRUCTURAL",
            MachineNodeEffectType.EnablePrestige1 => "UMBRAL ESTRUCTURAL",
            MachineNodeEffectType.DiagnosticReasonMarker => "DIAGNÓSTICO DE FALLAS",
            MachineNodeEffectType.InternalSupportBonus => "SOPORTE INTERNO" + percent,
            MachineNodeEffectType.UnlockInstantChamber => "CÁMARA DE ANCLAJES",
            MachineNodeEffectType.SeedReadingBonus => "LECTURA DE MADURACIÓN",
            MachineNodeEffectType.ArchiveSlotBonus => "ESPACIOS DE ARCHIVO" + value,
            MachineNodeEffectType.InstantInitialStabilityBonus => "ESTABILIDAD INICIAL" + percent,
            MachineNodeEffectType.SynchronizeStabilityBonus =>
                "ESTABILIDAD POR SINCRONIZACIÓN" + value,
            MachineNodeEffectType.TensionContainmentBonus =>
                "TENSIÓN POR ESTABILIZACIÓN  -" + node.effectValue.ToString("0.##"),
            MachineNodeEffectType.SafeRewindBonus =>
                "PÉRDIDA AL COMPENSAR  -" + node.effectValue.ToString("0.##"),
            MachineNodeEffectType.SeedSlotBonus => "ESPACIOS DE SEMILLA" + value,
            MachineNodeEffectType.PureMaterialThresholdReduction =>
                "UMBRAL DE ANCLAJE PURO" + reduction,
            _ => node.effectType.ToString().ToUpperInvariant().Replace('_', ' ') + value
        };
    }

    private static void SetText(TextMeshProUGUI label, string value)
    {
        if (label != null)
            label.text = value;
    }
}

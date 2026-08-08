using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class PrestigeDimensionTransitionUI : MonoBehaviour
{
    public const int RequiredConfirmationPresses = 3;

    [Header("Arte dimensional")]
    [SerializeField] private Texture2D cardsTexture;
    [SerializeField] private Texture2D portraitReference;
    [SerializeField] private Texture2D laboratoryBackground;
    [SerializeField] private Texture2D cube2DTexture;
    [SerializeField] private Texture2D characterTexture;
    [SerializeField] private Texture2D portalRingTexture;

    [Header("Tiempos")]
    [SerializeField, Min(6f)] private float transitionDuration = 8.6f;
    [SerializeField, Min(2f)] private float confirmationResetSeconds = 5f;

    private static readonly Color Background = new Color(0.008f, 0.014f, 0.027f, 1f);
    private static readonly Color Cyan = new Color(0.12f, 0.78f, 1f, 1f);
    private static readonly Color Violet = new Color(0.72f, 0.28f, 1f, 1f);
    private static readonly Color Orange = new Color(1f, 0.42f, 0.12f, 1f);

    private PrestigeUI _owner;
    private GameObject _root;
    private CanvasGroup _rootGroup;
    private GameObject _transitionStage;
    private GameObject _selectionStage;
    private CanvasGroup _selectionGroup;
    private RectTransform _cubeFrame;
    private RawImage _cubeImage;
    private RawImage _laboratoryBackgroundImage;
    private Image _laboratoryDarkening;
    private RectTransform _characterRect;
    private RawImage _characterImage;
    private readonly RectTransform[] _portalRects = new RectTransform[3];
    private readonly RawImage[] _portalOuterRings = new RawImage[3];
    private readonly RawImage[] _portalInnerRings = new RawImage[3];
    private readonly PrestigePortal2DGraphic[] _portalCores =
        new PrestigePortal2DGraphic[3];
    private Image _flash;

    private RawImage _leftCard;
    private RawImage _centerCard;
    private RawImage _rightCard;
    private RectTransform _leftCardRect;
    private RectTransform _centerCardRect;
    private RectTransform _rightCardRect;
    private TextMeshProUGUI _selectedTitle;
    private TextMeshProUGUI _selectedStatus;
    private TextMeshProUGUI _confirmationNotice;
    private TextMeshProUGUI _tuneLabel;
    private TextMeshProUGUI[] _dots;
    private Button _leftButton;
    private Button _rightButton;
    private Button _backButton;
    private Button _tuneButton;
    private Image _tuneButtonImage;

    private readonly List<AmbientParticle> _particles = new List<AmbientParticle>();
    private readonly List<CinematicParticle> _cinematicParticles =
        new List<CinematicParticle>();
    private Coroutine _flowRoutine;
    private Coroutine _buttonPulseRoutine;
    private int _selectedDimensionId = 2;
    private int _confirmationPresses;
    private float _lastConfirmationTime;
    private bool _isTuning;
    private bool _previewMode;

    public Texture2D CardsTexture => cardsTexture;
    public Texture2D PortraitReference => portraitReference;
    public Texture2D LaboratoryBackground => laboratoryBackground;
    public Texture2D Cube2DTexture => cube2DTexture;
    public Texture2D CharacterTexture => characterTexture;
    public Texture2D PortalRingTexture => portalRingTexture;
    public int ConfirmationPresses => _confirmationPresses;
    public int SelectedDimensionId => _selectedDimensionId;
    public bool IsVisible => _root != null && _root.activeSelf;
    public bool IsSelectionVisible => _selectionStage != null && _selectionStage.activeSelf;
    public float ConfirmationResetSeconds => confirmationResetSeconds;

    private sealed class AmbientParticle
    {
        public RectTransform rect;
        public Image image;
        public float phase;
        public float radius;
        public float speed;
    }

    private sealed class CinematicParticle
    {
        public RectTransform rect;
        public RawImage image;
        public int portalIndex;
        public float phase;
    }

    private void Awake()
    {
        Initialize(GetComponent<PrestigeUI>());
    }

    private void OnDisable()
    {
        if (_root != null && _root.activeSelf)
            CancelFlow(false);
    }

    private void Update()
    {
        if (_root == null || !_root.activeSelf)
            return;

        if (_confirmationPresses > 0 && !_isTuning &&
            Time.unscaledTime - _lastConfirmationTime > confirmationResetSeconds)
        {
            ResetConfirmation("La confirmación expiró. Pulsa tres veces para sintonizar.");
        }

        AnimateAmbientDetails();
    }

    public void Initialize(PrestigeUI owner)
    {
        if (owner != null)
            _owner = owner;
        if (_root == null)
            BuildInterface();
    }

    public bool BeginTransition()
    {
        GameState state = GameState.I;
        if (state == null || !state.CanDoPrestige1())
            return false;

        return BeginInternal(false);
    }

    public void HideAfterCommit()
    {
        _isTuning = false;
        _previewMode = false;
        if (_root != null)
            _root.SetActive(false);
    }

    public void Cancel()
    {
        if (_isTuning)
            return;
        CancelFlow(true);
    }

    public void MoveSelection(int direction)
    {
        if (_isTuning || direction == 0)
            return;

        int next = _selectedDimensionId + (direction > 0 ? 1 : -1);
        if (next < 1) next = 3;
        if (next > 3) next = 1;
        if (next == _selectedDimensionId)
            return;

        _selectedDimensionId = next;
        ResetConfirmation("La firma cambió. Confirma la nueva selección con tres pulsaciones.");
        RefreshCards(true);
    }

    public void PressTune()
    {
        if (_isTuning || !IsDimensionAvailable(_selectedDimensionId))
            return;

        _confirmationPresses = Mathf.Clamp(
            _confirmationPresses + 1, 0, RequiredConfirmationPresses);
        _lastConfirmationTime = Time.unscaledTime;
        RefreshConfirmationUi();

        if (_confirmationPresses >= RequiredConfirmationPresses)
        {
            if (_buttonPulseRoutine != null)
                StopCoroutine(_buttonPulseRoutine);
            _buttonPulseRoutine = null;
            if (_flowRoutine != null)
                StopCoroutine(_flowRoutine);
            _flowRoutine = StartCoroutine(CommitSelectionRoutine());
        }
        else
        {
            if (_buttonPulseRoutine != null)
                StopCoroutine(_buttonPulseRoutine);
            _buttonPulseRoutine = StartCoroutine(PulseTuneButton());
        }
    }

    public static string GetConfirmationInstruction(int presses)
    {
        switch (Mathf.Clamp(presses, 0, RequiredConfirmationPresses))
        {
            case 1: return "CONFIRMACIÓN 1/3  •  Pulsa dos veces más";
            case 2: return "CONFIRMACIÓN 2/3  •  Pulsa una vez más";
            case 3: return "CONFIRMACIÓN 3/3  •  FIRMA BLOQUEADA";
            default: return "SEGURIDAD: SINTONIZAR REQUIERE 3 PULSACIONES";
        }
    }

#if UNITY_EDITOR
    public void PreviewTransitionForCapture()
    {
        BeginInternal(true);
    }

    public void PreviewSelectionForCapture(int dimensionId, int confirmationPresses)
    {
        _previewMode = true;
        BuildInterface();
        if (_flowRoutine != null)
            StopCoroutine(_flowRoutine);
        _selectedDimensionId = Mathf.Clamp(dimensionId, 1, 3);
        _confirmationPresses = Mathf.Clamp(
            confirmationPresses, 0, RequiredConfirmationPresses - 1);
        _lastConfirmationTime = Time.unscaledTime;
        _root.SetActive(true);
        _root.transform.SetAsLastSibling();
        _rootGroup.alpha = 1f;
        _transitionStage.SetActive(false);
        _selectionStage.SetActive(true);
        _selectionGroup.alpha = 1f;
        RefreshCards(false);
        RefreshConfirmationUi();
    }
#endif

    private bool BeginInternal(bool preview)
    {
        BuildInterface();
        if (_root == null || _isTuning)
            return false;

        _previewMode = preview;
        _selectedDimensionId = FindInitialDimension();
        ResetConfirmation();
        RefreshCards(false);
        ApplyQualityProfile();

        if (_flowRoutine != null)
            StopCoroutine(_flowRoutine);
        _flowRoutine = StartCoroutine(TransitionRoutine());
        return true;
    }

    private void BuildInterface()
    {
        if (_root != null)
            return;

        LoadResourceConfiguration();

        Canvas parentCanvas = GetComponentInParent<Canvas>();
        Transform parent = parentCanvas != null ? parentCanvas.transform : transform;
        TMP_FontAsset font = _owner != null ? _owner.InterfaceFont : null;

        _root = CreateRectObject("PrestigeDimensionalFlow", parent,
            typeof(Image), typeof(CanvasGroup), typeof(Canvas),
            typeof(GraphicRaycaster));
        Stretch(_root.GetComponent<RectTransform>());
        Canvas overlayCanvas = _root.GetComponent<Canvas>();
        overlayCanvas.overrideSorting = true;
        overlayCanvas.sortingOrder = 6000;
        Image blocker = _root.GetComponent<Image>();
        blocker.color = Background;
        blocker.raycastTarget = true;
        _rootGroup = _root.GetComponent<CanvasGroup>();

        BuildBackground(font);
        BuildTransitionStage(font);
        BuildSelectionStage(font);

        _root.SetActive(false);
    }

    private void LoadResourceConfiguration()
    {
        if (cardsTexture != null && portraitReference != null &&
            laboratoryBackground != null && cube2DTexture != null &&
            characterTexture != null && portalRingTexture != null)
            return;

        PrestigeDimensionTransitionConfig config =
            Resources.Load<PrestigeDimensionTransitionConfig>(
                "Prestige/PrestigeDimensionTransitionConfig");
        if (config == null)
            return;
        if (cardsTexture == null)
            cardsTexture = config.cardsTexture;
        if (portraitReference == null)
            portraitReference = config.portraitReference;
        if (laboratoryBackground == null)
            laboratoryBackground = config.laboratoryBackground;
        if (cube2DTexture == null)
            cube2DTexture = config.cube2DTexture;
        if (characterTexture == null)
            characterTexture = config.characterTexture;
        if (portalRingTexture == null)
            portalRingTexture = config.portalRingTexture;
    }

    private void BuildBackground(TMP_FontAsset font)
    {
        GameObject vignette = CreateRectObject("BackgroundVignette", _root.transform,
            typeof(Image));
        SetRect(vignette.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero,
            Vector2.zero, new Vector2(1080f, 1920f));
        vignette.GetComponent<Image>().color = new Color(0.015f, 0.025f, 0.05f, 1f);
        vignette.GetComponent<Image>().raycastTarget = false;

        for (int i = 0; i < 14; i++)
        {
            GameObject line = CreateRectObject("CircuitLine_" + i, vignette.transform,
                typeof(Image));
            RectTransform rect = line.GetComponent<RectTransform>();
            float side = i % 2 == 0 ? -1f : 1f;
            float y = -780f + (i / 2) * 245f;
            SetRect(rect, Vector2.zero, Vector2.zero,
                new Vector2(side * (320f + (i % 3) * 55f), y),
                new Vector2(360f, 2f + (i % 2)));
            rect.localRotation = Quaternion.Euler(0f, 0f, side * (i % 3) * 12f);
            Image image = line.GetComponent<Image>();
            image.color = new Color(0.08f, 0.45f, 0.64f, 0.22f);
            image.raycastTarget = false;
        }

        for (int i = 0; i < 18; i++)
        {
            GameObject particle = CreateRectObject("DimensionalParticle_" + i,
                vignette.transform, typeof(Image));
            RectTransform rect = particle.GetComponent<RectTransform>();
            float radius = 180f + (i % 6) * 105f;
            SetRect(rect, Vector2.zero, Vector2.zero, Vector2.zero,
                new Vector2(5f + (i % 3) * 3f, 5f + (i % 3) * 3f));
            Image image = particle.GetComponent<Image>();
            image.color = i % 3 == 1
                ? new Color(Violet.r, Violet.g, Violet.b, 0.65f)
                : new Color(Cyan.r, Cyan.g, Cyan.b, 0.55f);
            image.raycastTarget = false;
            _particles.Add(new AmbientParticle
            {
                rect = rect,
                image = image,
                phase = i * 0.91f,
                radius = radius,
                speed = 0.12f + (i % 5) * 0.035f
            });
        }
    }

    private void BuildTransitionStage(TMP_FontAsset font)
    {
        _transitionStage = CreateRectObject("CubeTransitionStage", _root.transform);
        Stretch(_transitionStage.GetComponent<RectTransform>());

        GameObject laboratory = CreateRectObject("LaboratoryBackground",
            _transitionStage.transform, typeof(RawImage));
        Stretch(laboratory.GetComponent<RectTransform>());
        _laboratoryBackgroundImage = laboratory.GetComponent<RawImage>();
        _laboratoryBackgroundImage.texture = laboratoryBackground;
        _laboratoryBackgroundImage.color = Color.white;
        _laboratoryBackgroundImage.raycastTarget = false;

        GameObject darkening = CreateRectObject("LaboratoryDarkening",
            _transitionStage.transform, typeof(Image));
        Stretch(darkening.GetComponent<RectTransform>());
        _laboratoryDarkening = darkening.GetComponent<Image>();
        _laboratoryDarkening.color = new Color(0.005f, 0.008f, 0.015f, 0.08f);
        _laboratoryDarkening.raycastTarget = false;

        for (int i = 0; i < 3; i++)
        {
            GameObject portal = CreateRectObject("Portal2D_" + (i + 1),
                _transitionStage.transform);
            _portalRects[i] = portal.GetComponent<RectTransform>();
            Vector2 portalSize = i == 1
                ? new Vector2(270f, 390f)
                : new Vector2(260f, 500f);
            SetRect(_portalRects[i], Vector2.zero, Vector2.zero,
                PortalPosition(i), portalSize);
            _portalRects[i].localScale = Vector3.zero;

            GameObject core = CreateRectObject("PortalCore", portal.transform,
                typeof(PrestigePortal2DGraphic));
            Stretch(core.GetComponent<RectTransform>(), 12f, 18f);
            _portalCores[i] = core.GetComponent<PrestigePortal2DGraphic>();
            Color portalColor = ColorForDimension(i + 1);
            _portalCores[i].Configure(portalColor);
            _portalCores[i].SetIntensity(0f);

            _portalOuterRings[i] = CreateRawImage(portal.transform, "OuterRing");
            Stretch(_portalOuterRings[i].rectTransform);
            _portalOuterRings[i].texture = portalRingTexture;
            _portalOuterRings[i].color = new Color(
                portalColor.r, portalColor.g, portalColor.b, 0f);
            _portalOuterRings[i].raycastTarget = false;

            _portalInnerRings[i] = CreateRawImage(portal.transform, "InnerRing");
            Stretch(_portalInnerRings[i].rectTransform, 20f, 28f);
            _portalInnerRings[i].texture = portalRingTexture;
            _portalInnerRings[i].color = new Color(1f, 1f, 1f, 0f);
            _portalInnerRings[i].raycastTarget = false;
        }

        for (int i = 0; i < 18; i++)
        {
            GameObject spark = CreateRectObject("PortalSpark_" + i,
                _transitionStage.transform, typeof(RawImage));
            RectTransform sparkRect = spark.GetComponent<RectTransform>();
            SetRect(sparkRect, Vector2.zero, Vector2.zero, Vector2.zero,
                new Vector2(54f + (i % 3) * 8f, 18f + (i % 2) * 5f));
            RawImage sparkImage = spark.GetComponent<RawImage>();
            sparkImage.texture = portalRingTexture;
            sparkImage.uvRect = i % 2 == 0
                ? new Rect(0f, 0f, 0.5f, 1f)
                : new Rect(0.5f, 0f, 0.5f, 1f);
            Color sparkColor = ColorForDimension(i % 3 + 1);
            sparkImage.color = new Color(
                sparkColor.r, sparkColor.g, sparkColor.b, 0f);
            sparkImage.raycastTarget = false;
            _cinematicParticles.Add(new CinematicParticle
            {
                rect = sparkRect,
                image = sparkImage,
                portalIndex = i % 3,
                phase = (i / 3) / 6f
            });
        }

        GameObject frame = CreateRectObject("Cube2DHero", _transitionStage.transform);
        _cubeFrame = frame.GetComponent<RectTransform>();
        SetRect(_cubeFrame, Vector2.zero, Vector2.zero,
            new Vector2(0f, 80f), new Vector2(660f, 660f));

        GameObject cube = CreateRectObject("IllustratedMachineCube", frame.transform,
            typeof(RawImage));
        SetRect(cube.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero,
            Vector2.zero, new Vector2(650f, 650f));
        _cubeImage = cube.GetComponent<RawImage>();
        _cubeImage.texture = cube2DTexture;
        _cubeImage.color = new Color(1f, 1f, 1f, 0f);
        _cubeImage.raycastTarget = false;

        GameObject character = CreateRectObject("AnonymousCharacter2D",
            _transitionStage.transform, typeof(RawImage));
        _characterRect = character.GetComponent<RectTransform>();
        SetRect(_characterRect, Vector2.zero, Vector2.zero,
            new Vector2(0f, -560f), new Vector2(570f, 855f));
        _characterImage = character.GetComponent<RawImage>();
        _characterImage.texture = characterTexture;
        _characterImage.color = new Color(1f, 1f, 1f, 0f);
        _characterImage.raycastTarget = false;

        GameObject flash = CreateRectObject("DimensionalFlash", _root.transform,
            typeof(Image));
        Stretch(flash.GetComponent<RectTransform>());
        _flash = flash.GetComponent<Image>();
        _flash.color = new Color(0.72f, 0.92f, 1f, 0f);
        _flash.raycastTarget = false;
    }

    private void BuildSelectionStage(TMP_FontAsset font)
    {
        _selectionStage = CreateRectObject("DimensionSelectionStage", _root.transform,
            typeof(CanvasGroup));
        Stretch(_selectionStage.GetComponent<RectTransform>());
        _selectionGroup = _selectionStage.GetComponent<CanvasGroup>();

        TextMeshProUGUI title = CreateText(_selectionStage.transform,
            "SELECCIÓN DIMENSIONAL", font, 48f, FontStyles.Bold,
            new Color(0.9f, 0.92f, 1f, 1f));
        SetRect(title.rectTransform, Vector2.zero, Vector2.zero,
            new Vector2(0f, 830f), new Vector2(760f, 90f));

        TextMeshProUGUI subtitle = CreateText(_selectionStage.transform,
            "DESLIZA O TOCA LOS LATERALES PARA CAMBIAR LA FIRMA", font, 18f,
            FontStyles.Normal, new Color(0.36f, 0.74f, 0.94f, 0.9f));
        SetRect(subtitle.rectTransform, Vector2.zero, Vector2.zero,
            new Vector2(0f, 766f), new Vector2(920f, 48f));

        _backButton = CreateButton(_selectionStage.transform, "BackButton", "VOLVER",
            font, new Vector2(-430f, 835f), new Vector2(150f, 64f),
            new Color(0.07f, 0.14f, 0.22f, 0.95f), Cancel, out _);

        GameObject cardArea = CreateRectObject("CarouselSwipeArea",
            _selectionStage.transform, typeof(Image), typeof(PrestigeDimensionSwipeSurface));
        SetRect(cardArea.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero,
            new Vector2(0f, 80f), new Vector2(1080f, 1320f));
        Image swipeBlock = cardArea.GetComponent<Image>();
        swipeBlock.color = new Color(0f, 0f, 0f, 0.001f);
        swipeBlock.raycastTarget = true;
        cardArea.GetComponent<PrestigeDimensionSwipeSurface>().Initialize(this);

        _leftCard = CreateCard(cardArea.transform, "LeftPreview",
            new Vector2(-455f, 60f), new Vector2(340f, 900f), out _leftCardRect);
        _rightCard = CreateCard(cardArea.transform, "RightPreview",
            new Vector2(455f, 60f), new Vector2(340f, 900f), out _rightCardRect);

        _leftButton = CreateInvisibleButton(cardArea.transform, "PreviousDimension",
            new Vector2(-445f, 60f), new Vector2(330f, 1020f), () => MoveSelection(-1));
        _rightButton = CreateInvisibleButton(cardArea.transform, "NextDimension",
            new Vector2(445f, 60f), new Vector2(330f, 1020f), () => MoveSelection(1));

        _centerCard = CreateCard(cardArea.transform, "SelectedDimension",
            new Vector2(0f, 60f), new Vector2(590f, 1160f), out _centerCardRect);

        TextMeshProUGUI leftArrow = CreateText(cardArea.transform, "‹", font, 80f,
            FontStyles.Bold, new Color(0.55f, 0.9f, 1f, 0.9f));
        SetRect(leftArrow.rectTransform, Vector2.zero, Vector2.zero,
            new Vector2(-490f, 60f), new Vector2(80f, 100f));
        TextMeshProUGUI rightArrow = CreateText(cardArea.transform, "›", font, 80f,
            FontStyles.Bold, new Color(0.55f, 0.9f, 1f, 0.9f));
        SetRect(rightArrow.rectTransform, Vector2.zero, Vector2.zero,
            new Vector2(490f, 60f), new Vector2(80f, 100f));

        _selectedTitle = CreateText(_selectionStage.transform, "", font, 29f,
            FontStyles.Bold, Color.white);
        SetRect(_selectedTitle.rectTransform, Vector2.zero, Vector2.zero,
            new Vector2(0f, -515f), new Vector2(880f, 60f));
        _selectedStatus = CreateText(_selectionStage.transform, "", font, 19f,
            FontStyles.Normal, new Color(0.7f, 0.8f, 0.9f, 1f));
        SetRect(_selectedStatus.rectTransform, Vector2.zero, Vector2.zero,
            new Vector2(0f, -565f), new Vector2(900f, 50f));

        _dots = new TextMeshProUGUI[3];
        for (int i = 0; i < _dots.Length; i++)
        {
            _dots[i] = CreateText(_selectionStage.transform, "●", font, 28f,
                FontStyles.Normal, new Color(0.16f, 0.45f, 0.62f, 0.8f));
            SetRect(_dots[i].rectTransform, Vector2.zero, Vector2.zero,
                new Vector2((i - 1) * 58f, -615f), new Vector2(44f, 44f));
        }

        GameObject noticePanel = CreateRectObject("SafetyConfirmationNotice",
            _selectionStage.transform, typeof(Image));
        SetRect(noticePanel.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero,
            new Vector2(0f, -680f), new Vector2(880f, 78f));
        noticePanel.GetComponent<Image>().color = new Color(0.045f, 0.09f, 0.15f, 0.96f);
        _confirmationNotice = CreateText(noticePanel.transform,
            GetConfirmationInstruction(0), font, 18f, FontStyles.Bold,
            new Color(0.65f, 0.86f, 1f, 1f));
        Stretch(_confirmationNotice.rectTransform, 20f, 10f);

        _tuneButton = CreateButton(_selectionStage.transform, "TuneButton",
            "SINTONIZAR", font, new Vector2(0f, -790f), new Vector2(720f, 126f),
            new Color(0.25f, 0.09f, 0.36f, 1f), PressTune, out _tuneLabel);
        _tuneButtonImage = _tuneButton.targetGraphic as Image;

        TextMeshProUGUI footer = CreateText(_selectionStage.transform,
            "El Prestigio se ejecutará solamente después de la tercera pulsación.",
            font, 17f, FontStyles.Normal, new Color(0.55f, 0.62f, 0.72f, 0.9f));
        SetRect(footer.rectTransform, Vector2.zero, Vector2.zero,
            new Vector2(0f, -880f), new Vector2(920f, 55f));

        _selectionStage.SetActive(false);
    }

    private IEnumerator TransitionRoutine()
    {
        _root.SetActive(true);
        _root.transform.SetAsLastSibling();
        _transitionStage.SetActive(true);
        _selectionStage.SetActive(false);
        _rootGroup.alpha = 0f;
        _flash.color = new Color(0.72f, 0.92f, 1f, 0f);
        if (_laboratoryBackgroundImage != null)
            _laboratoryBackgroundImage.color = Color.white;
        if (_laboratoryDarkening != null)
            _laboratoryDarkening.color = new Color(0.005f, 0.008f, 0.015f, 0.08f);

        _characterRect.anchoredPosition = new Vector2(0f, -680f);
        _characterImage.color = new Color(1f, 1f, 1f, 0f);
        _cubeFrame.localScale = Vector3.one;
        _cubeFrame.localRotation = Quaternion.identity;
        _cubeImage.color = new Color(1f, 1f, 1f, 0f);
        for (int i = 0; i < 3; i++)
        {
            _portalRects[i].localScale = Vector3.zero;
            _portalOuterRings[i].color = Color.clear;
            _portalInnerRings[i].color = Color.clear;
            _portalCores[i].SetIntensity(0f);
        }
        for (int i = 0; i < _cinematicParticles.Count; i++)
            _cinematicParticles[i].image.color = Color.clear;

        float elapsed = 0f;
        float duration = Mathf.Max(6f, transitionDuration);
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            _rootGroup.alpha = Mathf.Clamp01(t / 0.08f);

            float characterIn = Mathf.SmoothStep(0f, 1f,
                Mathf.InverseLerp(0.02f, 0.16f, t));
            float characterOut = 1f - Mathf.SmoothStep(0f, 1f,
                Mathf.InverseLerp(0.70f, 0.82f, t));
            float characterAlpha = characterIn * characterOut;
            _characterImage.color = new Color(1f, 1f, 1f, characterAlpha);
            _characterRect.anchoredPosition = new Vector2(0f,
                Mathf.Lerp(-680f, -560f, characterIn));

            float cubeIn = Mathf.SmoothStep(0f, 1f,
                Mathf.InverseLerp(0.10f, 0.25f, t));
            _cubeFrame.localScale = Vector3.one;
            _cubeFrame.localRotation = Quaternion.identity;
            _cubeImage.color = new Color(1f, 1f, 1f, cubeIn);

            for (int i = 0; i < 3; i++)
            {
                float portalStart = 0.35f + i * 0.055f;
                float portalOpen = Mathf.SmoothStep(0f, 1f,
                    Mathf.InverseLerp(portalStart, portalStart + 0.12f, t));
                float portalFade = 1f - Mathf.SmoothStep(0f, 1f,
                    Mathf.InverseLerp(0.70f, 0.82f, t));
                float portalAlpha = portalOpen * portalFade;
                float portalScale = portalOpen *
                    (1f + Mathf.Sin(t * 11f + i) * 0.018f);
                _portalRects[i].localScale = Vector3.one * portalScale;
                _portalOuterRings[i].rectTransform.localRotation =
                    Quaternion.Euler(0f, 0f,
                        Mathf.Sin(t * 7f + i * 0.8f) * 2.2f);
                _portalInnerRings[i].rectTransform.localRotation =
                    Quaternion.Euler(0f, 0f,
                        -Mathf.Sin(t * 8.5f + i * 0.65f) * 3.4f);
                float innerBreath = 0.94f +
                    Mathf.Sin(t * 12f + i) * 0.012f;
                _portalInnerRings[i].rectTransform.localScale =
                    Vector3.one * innerBreath;
                Color portalColor = ColorForDimension(i + 1);
                _portalOuterRings[i].color = new Color(
                    portalColor.r, portalColor.g, portalColor.b,
                    portalAlpha * 0.95f);
                _portalInnerRings[i].color = new Color(
                    Mathf.Lerp(portalColor.r, 1f, 0.28f),
                    Mathf.Lerp(portalColor.g, 1f, 0.28f),
                    Mathf.Lerp(portalColor.b, 1f, 0.28f),
                    portalAlpha * 0.38f);
                _portalCores[i].SetIntensity(portalAlpha);
            }

            for (int i = 0; i < _cinematicParticles.Count; i++)
            {
                CinematicParticle particle = _cinematicParticles[i];
                Color particleColor = ColorForDimension(
                    particle.portalIndex + 1);
                float launchStart = 0.22f + particle.portalIndex * 0.055f +
                    particle.phase * 0.055f;
                float launchEnd = launchStart + 0.18f;
                if (t >= launchStart && t <= launchEnd)
                {
                    float travel = Mathf.SmoothStep(0f, 1f,
                        Mathf.InverseLerp(launchStart, launchEnd, t));
                    Vector2 start = new Vector2(0f, 80f);
                    Vector2 target = PortalPosition(particle.portalIndex);
                    Vector2 direction = target - start;
                    particle.rect.anchoredPosition = Vector2.Lerp(
                        start, target, travel);
                    particle.rect.localRotation = Quaternion.Euler(0f, 0f,
                        Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
                    particle.image.color = new Color(
                        particleColor.r, particleColor.g, particleColor.b,
                        Mathf.Sin(travel * Mathf.PI) * 0.92f);
                }
                else
                {
                    particle.image.color = Color.clear;
                }
            }

            // El laboratorio desaparece gradualmente mientras el cubo concentra
            // la energía que abre la selección dimensional.
            if (_laboratoryDarkening != null)
            {
                float fade = Mathf.SmoothStep(0.08f, 0.76f,
                    Mathf.InverseLerp(0.62f, 0.90f, t));
                _laboratoryDarkening.color = new Color(
                    0.005f, 0.008f, 0.015f, fade);
            }

            if (t > 0.88f)
            {
                float flash = Mathf.InverseLerp(0.88f, 1f, t);
                _flash.color = new Color(0.72f, 0.92f, 1f,
                    Mathf.SmoothStep(0f, 1f, flash));
            }
            yield return null;
        }

        _transitionStage.SetActive(false);
        _selectionStage.SetActive(true);
        _selectionGroup.alpha = 0f;
        RefreshCards(false);

        float reveal = 0f;
        Vector2 centerTarget = new Vector2(0f, 60f);
        while (reveal < 0.65f)
        {
            reveal += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(reveal / 0.65f);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            _selectionGroup.alpha = eased;
            _flash.color = new Color(0.72f, 0.92f, 1f, 1f - eased);
            _centerCardRect.anchoredPosition = centerTarget +
                new Vector2(0f, -90f * (1f - eased));
            _centerCardRect.localScale = Vector3.one * Mathf.Lerp(0.9f, 1f, eased);
            yield return null;
        }

        _selectionGroup.alpha = 1f;
        _flash.color = new Color(0f, 0f, 0f, 0f);
        _centerCardRect.anchoredPosition = centerTarget;
        _centerCardRect.localScale = Vector3.one;
        _flowRoutine = null;
    }

    private IEnumerator CommitSelectionRoutine()
    {
        _isTuning = true;
        SetControlsInteractable(false);
        _confirmationNotice.text = GetConfirmationInstruction(3);
        _confirmationNotice.color = ColorForDimension(_selectedDimensionId);
        _tuneLabel.text = "SINTONIZANDO...";

        Color signature = ColorForDimension(_selectedDimensionId);
        _flash.transform.SetAsLastSibling();
        float elapsed = 0f;
        const float duration = 1.25f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float pulse = Mathf.Sin(t * Mathf.PI);
            _centerCardRect.localScale = Vector3.one * (1f + pulse * 0.045f);
            _tuneButtonImage.color = Color.Lerp(
                new Color(0.25f, 0.09f, 0.36f, 1f), signature, pulse * 0.65f);
            _flash.color = new Color(signature.r, signature.g, signature.b,
                t > 0.72f ? Mathf.InverseLerp(0.72f, 1f, t) * 0.65f : 0f);
            yield return null;
        }

        if (_previewMode)
        {
            _isTuning = false;
            _confirmationPresses = RequiredConfirmationPresses - 1;
            RefreshConfirmationUi();
            SetControlsInteractable(true);
            yield break;
        }

        bool committed = _owner != null &&
            _owner.CommitDimensionForPrestige1(_selectedDimensionId);
        if (!committed)
        {
            _isTuning = false;
            _centerCardRect.localScale = Vector3.one;
            _flash.color = new Color(0f, 0f, 0f, 0f);
            SetControlsInteractable(true);
            ResetConfirmation("No fue posible completar el Prestigio. No se modificó la partida.");
        }
        _flowRoutine = null;
    }

    private IEnumerator PulseTuneButton()
    {
        if (_tuneButtonImage == null)
            yield break;
        Color baseColor = new Color(0.25f, 0.09f, 0.36f, 1f);
        Color pulseColor = ColorForDimension(_selectedDimensionId);
        float elapsed = 0f;
        while (elapsed < 0.28f)
        {
            elapsed += Time.unscaledDeltaTime;
            float pulse = Mathf.Sin(Mathf.Clamp01(elapsed / 0.28f) * Mathf.PI);
            _tuneButtonImage.color = Color.Lerp(baseColor, pulseColor, pulse * 0.55f);
            yield return null;
        }
        _tuneButtonImage.color = baseColor;
        _buttonPulseRoutine = null;
    }

    private void ApplyQualityProfile()
    {
        int activeParticles = MachineCube3DQuality.Current == MachineCube3DQualityLevel.High
            ? 18
            : MachineCube3DQuality.Current == MachineCube3DQualityLevel.Balanced ? 10 : 6;
        for (int i = 0; i < _particles.Count; i++)
            _particles[i].rect.gameObject.SetActive(i < activeParticles);

        int activeCinematicParticles =
            MachineCube3DQuality.Current == MachineCube3DQualityLevel.High
                ? 18
                : MachineCube3DQuality.Current == MachineCube3DQualityLevel.Balanced
                    ? 12
                    : 6;
        for (int i = 0; i < _cinematicParticles.Count; i++)
            _cinematicParticles[i].rect.gameObject.SetActive(
                i < activeCinematicParticles);

        bool useSecondPortalLayer = MachineCube3DQuality.Current !=
            MachineCube3DQualityLevel.Low;
        for (int i = 0; i < _portalInnerRings.Length; i++)
        {
            if (_portalInnerRings[i] != null)
                _portalInnerRings[i].gameObject.SetActive(useSecondPortalLayer);
            if (_portalCores[i] != null)
                _portalCores[i].SetLowQuality(
                    MachineCube3DQuality.Current == MachineCube3DQualityLevel.Low);
        }
    }

    private void AnimateAmbientDetails()
    {
        float time = Time.unscaledTime;
        bool transition = _transitionStage != null && _transitionStage.activeSelf;
        for (int i = 0; i < _particles.Count; i++)
        {
            AmbientParticle particle = _particles[i];
            if (!particle.rect.gameObject.activeSelf)
                continue;
            float angle = particle.phase + time * particle.speed;
            float radius = particle.radius;
            if (transition)
                radius *= 0.78f + 0.22f * Mathf.Sin(time * 0.9f + particle.phase);
            particle.rect.anchoredPosition = new Vector2(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius * 1.55f);
            Color color = particle.image.color;
            color.a = 0.22f + 0.42f *
                Mathf.Pow(Mathf.Sin(time * 0.7f + particle.phase), 2f);
            particle.image.color = color;
        }

        if (!transition && _centerCardRect != null && !_isTuning &&
            MachineCube3DQuality.Current != MachineCube3DQualityLevel.Low)
        {
            float scale = 1f + Mathf.Sin(time * 1.15f) * 0.004f;
            _centerCardRect.localScale = Vector3.one * scale;
        }
    }

    private void RefreshCards(bool animate)
    {
        if (_centerCard == null)
            return;

        int previous = _selectedDimensionId - 1;
        if (previous < 1) previous = 3;
        int next = _selectedDimensionId + 1;
        if (next > 3) next = 1;

        SetCard(_leftCard, previous, false);
        SetCard(_centerCard, _selectedDimensionId, true);
        SetCard(_rightCard, next, false);

        bool available = IsDimensionAvailable(_selectedDimensionId);
        _selectedTitle.text = Roman(_selectedDimensionId) + "  •  " +
            PrestigeUI.GetDimensionSignatureName(_selectedDimensionId);
        _selectedTitle.color = ColorForDimension(_selectedDimensionId);
        _selectedStatus.text = available
            ? GetDimensionTeaser(_selectedDimensionId)
            : "FIRMA YA SINTONIZADA  •  Selecciona otra dimensión";

        for (int i = 0; i < _dots.Length; i++)
        {
            bool selected = i + 1 == _selectedDimensionId;
            _dots[i].color = selected
                ? ColorForDimension(_selectedDimensionId)
                : new Color(0.15f, 0.38f, 0.52f, 0.75f);
            _dots[i].fontSize = selected ? 34f : 25f;
        }

        _tuneButton.interactable = available && !_isTuning;
        if (!available)
        {
            _tuneLabel.text = "FIRMA YA SINTONIZADA";
            _confirmationNotice.text = "Esta ruta ya fue revelada. Elige otra firma dimensional.";
            _confirmationNotice.color = new Color(0.55f, 0.64f, 0.72f, 1f);
        }
        else
        {
            RefreshConfirmationUi();
        }

        if (animate && isActiveAndEnabled)
            StartCoroutine(PulseSelectedCard());
    }

    private IEnumerator PulseSelectedCard()
    {
        float elapsed = 0f;
        while (elapsed < 0.24f)
        {
            elapsed += Time.unscaledDeltaTime;
            float pulse = Mathf.Sin(Mathf.Clamp01(elapsed / 0.24f) * Mathf.PI);
            _centerCardRect.localScale = Vector3.one * (1f + pulse * 0.025f);
            yield return null;
        }
        _centerCardRect.localScale = Vector3.one;
    }

    private void SetCard(RawImage image, int dimensionId, bool selected)
    {
        image.texture = cardsTexture;
        image.uvRect = GetCardUv(dimensionId);
        bool available = IsDimensionAvailable(dimensionId);
        float alpha = selected ? 1f : 0.62f;
        if (!available)
            alpha *= 0.48f;
        image.color = new Color(1f, 1f, 1f, alpha);
    }

    private bool IsDimensionAvailable(int dimensionId)
    {
        if (_previewMode || GameState.I == null)
            return true;
        return !GameState.I.IsDimensionUnlockedAfterPrestige1(dimensionId);
    }

    private int FindInitialDimension()
    {
        int[] preferredOrder = { 2, 1, 3 };
        for (int i = 0; i < preferredOrder.Length; i++)
            if (IsDimensionAvailable(preferredOrder[i]))
                return preferredOrder[i];
        return 1;
    }

    private void ResetConfirmation(string notice = null)
    {
        _confirmationPresses = 0;
        _lastConfirmationTime = 0f;
        if (_confirmationNotice != null && !string.IsNullOrEmpty(notice))
        {
            _confirmationNotice.text = notice;
            _confirmationNotice.color = new Color(0.65f, 0.86f, 1f, 1f);
        }
        else
        {
            RefreshConfirmationUi();
        }
    }

    private void RefreshConfirmationUi()
    {
        if (_confirmationNotice == null || _tuneLabel == null)
            return;
        _confirmationNotice.text = GetConfirmationInstruction(_confirmationPresses);
        _confirmationNotice.color = _confirmationPresses > 0
            ? ColorForDimension(_selectedDimensionId)
            : new Color(0.65f, 0.86f, 1f, 1f);
        _tuneLabel.text = _confirmationPresses <= 0
            ? "SINTONIZAR  •  3 PULSACIONES"
            : "CONFIRMAR  " + _confirmationPresses + "/3";
    }

    private void SetControlsInteractable(bool interactable)
    {
        if (_leftButton != null) _leftButton.interactable = interactable;
        if (_rightButton != null) _rightButton.interactable = interactable;
        if (_backButton != null) _backButton.interactable = interactable;
        if (_tuneButton != null)
            _tuneButton.interactable = interactable &&
                IsDimensionAvailable(_selectedDimensionId);
    }

    private void CancelFlow(bool userRequested)
    {
        if (_flowRoutine != null)
            StopCoroutine(_flowRoutine);
        _flowRoutine = null;
        if (_buttonPulseRoutine != null)
            StopCoroutine(_buttonPulseRoutine);
        _buttonPulseRoutine = null;
        _isTuning = false;
        _previewMode = false;
        ResetConfirmation();
        if (_root != null)
            _root.SetActive(false);
        if (userRequested)
            Debug.Log("[Prestige Dimension] Selección cancelada sin modificar la partida.");
    }

    private RawImage CreateCard(Transform parent, string name, Vector2 position,
        Vector2 size, out RectTransform rect)
    {
        GameObject frame = CreateRectObject(name + "Frame", parent, typeof(Image));
        rect = frame.GetComponent<RectTransform>();
        SetRect(rect, Vector2.zero, Vector2.zero, position,
            size + new Vector2(16f, 16f));
        frame.GetComponent<Image>().color = new Color(0.08f, 0.32f, 0.5f, 0.72f);
        frame.GetComponent<Image>().raycastTarget = false;

        GameObject imageObject = CreateRectObject(name, frame.transform, typeof(RawImage));
        RectTransform imageRect = imageObject.GetComponent<RectTransform>();
        Stretch(imageRect, 8f, 8f);
        RawImage image = imageObject.GetComponent<RawImage>();
        image.texture = cardsTexture;
        image.raycastTarget = false;
        return image;
    }

    private static Rect GetCardUv(int dimensionId)
    {
        switch (dimensionId)
        {
            case 1: return new Rect(0.032f, 0.055f, 0.296f, 0.89f);
            case 2: return new Rect(0.352f, 0.055f, 0.296f, 0.89f);
            default: return new Rect(0.674f, 0.055f, 0.296f, 0.89f);
        }
    }

    private static string Roman(int dimensionId)
    {
        return dimensionId == 1 ? "DIMENSIÓN I" :
            dimensionId == 2 ? "DIMENSIÓN II" : "DIMENSIÓN III";
    }

    private static string GetDimensionTeaser(int dimensionId)
    {
        switch (dimensionId)
        {
            case 1: return "Ruta galáctica  •  Descubre el Ancla Galáctica";
            case 2: return "Ruta del eclipse  •  Establece el Pacto Mayor";
            default: return "Ruta industrial  •  Integra el Núcleo de Autonomía";
        }
    }

    private static Color ColorForDimension(int dimensionId)
    {
        switch (dimensionId)
        {
            case 1: return Cyan;
            case 2: return Violet;
            default: return Orange;
        }
    }

    private static Vector2 PortalPosition(int portalIndex)
    {
        switch (portalIndex)
        {
            case 0: return new Vector2(-360f, 150f);
            case 1: return new Vector2(0f, 590f);
            default: return new Vector2(360f, 150f);
        }
    }

    private static RawImage CreateRawImage(Transform parent, string name)
    {
        return CreateRectObject(name, parent, typeof(RawImage)).GetComponent<RawImage>();
    }

    private static Button CreateInvisibleButton(Transform parent, string name,
        Vector2 position, Vector2 size, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = CreateRectObject(name, parent,
            typeof(Image), typeof(Button));
        SetRect(buttonObject.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero,
            position, size);
        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.001f);
        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);
        return button;
    }

    private static Button CreateButton(Transform parent, string name, string label,
        TMP_FontAsset font, Vector2 position, Vector2 size, Color color,
        UnityEngine.Events.UnityAction action, out TextMeshProUGUI labelText)
    {
        GameObject buttonObject = CreateRectObject(name, parent,
            typeof(Image), typeof(Button));
        SetRect(buttonObject.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero,
            position, size);
        Image image = buttonObject.GetComponent<Image>();
        image.color = color;
        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.12f, 1.12f, 1.12f, 1f);
        colors.pressedColor = new Color(0.72f, 0.82f, 0.92f, 1f);
        colors.disabledColor = new Color(0.32f, 0.36f, 0.42f, 0.72f);
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        labelText = CreateText(buttonObject.transform, label, font, 25f,
            FontStyles.Bold, Color.white);
        Stretch(labelText.rectTransform, 14f, 8f);
        return button;
    }

    private static TextMeshProUGUI CreateText(Transform parent, string content,
        TMP_FontAsset font, float size, FontStyles style, Color color)
    {
        GameObject textObject = CreateRectObject("Text", parent,
            typeof(TextMeshProUGUI));
        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = content;
        text.font = font;
        text.fontSize = size;
        text.fontStyle = style;
        text.color = color;
        text.alignment = TextAlignmentOptions.Center;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.raycastTarget = false;
        return text;
    }

    private static GameObject CreateRectObject(string name, Transform parent,
        params System.Type[] components)
    {
        var types = new List<System.Type> { typeof(RectTransform) };
        if (components != null)
            types.AddRange(components);
        GameObject gameObject = new GameObject(name, types.ToArray());
        gameObject.layer = 5;
        gameObject.transform.SetParent(parent, false);
        return gameObject;
    }

    private static void Stretch(RectTransform rect, float horizontal = 0f,
        float vertical = 0f)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(horizontal, vertical);
        rect.offsetMax = new Vector2(-horizontal, -vertical);
        rect.anchoredPosition = Vector2.zero;
    }

    private static void SetRect(RectTransform rect, Vector2 anchorMin,
        Vector2 anchorMax, Vector2 position, Vector2 size)
    {
        rect.anchorMin = anchorMin == Vector2.zero && anchorMax == Vector2.zero
            ? new Vector2(0.5f, 0.5f)
            : anchorMin;
        rect.anchorMax = anchorMin == Vector2.zero && anchorMax == Vector2.zero
            ? new Vector2(0.5f, 0.5f)
            : anchorMax;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        rect.localScale = Vector3.one;
    }
}

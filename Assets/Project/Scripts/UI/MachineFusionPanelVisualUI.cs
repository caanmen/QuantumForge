using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Presentación dinámica del panel de mezclas. La lógica de inventario,
/// recetas y recompensas continúa viviendo en Room2PanelUI.
/// </summary>
public sealed class MachineFusionPanelVisualUI : MonoBehaviour
{
    private static readonly Color Cyan = Hex("00C9FF");
    private static readonly Color Violet = Hex("B55CFF");
    private static readonly Color Amber = Hex("F0A018");
    private static readonly Color Red = Hex("FF5364");
    private static readonly Color Green = Hex("74E346");
    private static readonly Color Inactive = Hex("18242C");
    private static readonly Color Muted = Hex("66727C");

    [Header("Estado")]
    [SerializeField] private Room2PanelUI roomPanel;
    [SerializeField] private Outline fragmentAOutline;
    [SerializeField] private Outline fragmentBOutline;
    [SerializeField] private Outline catalystOutline;
    [SerializeField] private Image[] riskSegments;
    [SerializeField] private Image[] instabilitySegments;

    [Header("Reactor")]
    [SerializeField] private Image cyanStream;
    [SerializeField] private Image violetStream;
    [SerializeField] private Image coreGlow;
    [SerializeField] private RectTransform coreTransform;
    [SerializeField] private Image[] reactorParticles;
    [SerializeField] private FusionEnergyWaveGraphic energyWave;

    [Header("Controles")]
    [SerializeField] private Button coolButton;
    [SerializeField] private TextMeshProUGUI coolButtonText;
    [SerializeField] private Button mixButton;
    [SerializeField] private TextMeshProUGUI mixButtonText;
    [SerializeField] private GameObject logBadgeRoot;
    [SerializeField] private TextMeshProUGUI logBadgeText;

    [Header("Resultado")]
    [SerializeField] private Image resultIcon;
    [SerializeField] private TextMeshProUGUI resultGlyph;
    [SerializeField] private TextMeshProUGUI resultTitle;
    [SerializeField] private TextMeshProUGUI resultDetail;
    [SerializeField] private TextMeshProUGUI resultMeta;
    [SerializeField] private TextMeshProUGUI diagnosticText;
    [SerializeField] private Image[] diagnosticBars;

    private Color _cyanBase;
    private Color _violetBase;
    private Color _coreBase;

    private void Awake()
    {
        CacheBaseColors();
    }

    private void OnEnable()
    {
        CacheBaseColors();
        ApplyPulse(0f);
        RefreshState();
    }

    private void Update()
    {
        float pulse = 0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * 4.2f);
        ApplyPulse(pulse);
        AnimateDiagnostics();
    }

    private void LateUpdate()
    {
        RefreshState();
    }

    private void CacheBaseColors()
    {
        if (cyanStream != null) _cyanBase = cyanStream.color;
        if (violetStream != null) _violetBase = violetStream.color;
        if (coreGlow != null) _coreBase = coreGlow.color;
    }

    private void RefreshState()
    {
        if (roomPanel == null)
            return;

        SetSelectionOutline(fragmentAOutline, roomPanel.FragmentASelected, Cyan);
        SetSelectionOutline(fragmentBOutline, roomPanel.FragmentBSelected, Violet);
        SetSelectionOutline(catalystOutline, roomPanel.CatalystSelected, Cyan);

        RefreshSegmentBar(riskSegments, roomPanel.CurrentFusionRisk01, Amber, Red);
        RefreshSegmentBar(instabilitySegments,
            Mathf.Clamp01(roomPanel.CurrentInstability / 20f),
            roomPanel.CurrentInstability > 12 ? Red : Violet,
            Amber);

        RefreshButtons();
        RefreshLogBadge();
        RefreshResult();
        RefreshDiagnostics();
    }

    private void RefreshButtons()
    {
        bool cooling = roomPanel.FusionCoolingDown;
        bool english = roomPanel.IsEnglishUi;
        if (mixButton != null)
            mixButton.interactable = roomPanel.CanExecuteFusion;
        if (mixButtonText != null)
        {
            if (!roomPanel.HasUnlockedFusionSlot)
                mixButtonText.text = english ? "REPAIR FUSION TABLE" : "REPARA MESA DE FUSIÓN";
            else if (cooling)
                mixButtonText.text = (english ? "STABILIZING " : "ESTABILIZANDO ") +
                    roomPanel.FusionCooldownRemaining.ToString("0.0") + " S";
            else if (!roomPanel.FusionSelectionComplete)
                mixButtonText.text = english ? "SELECT COMPONENTS" : "SELECCIONA COMPONENTES";
            else if (!roomPanel.HasRequiredFragmentsForSelection)
                mixButtonText.text = english ? "MISSING FRAGMENTS" : "FALTAN FRAGMENTOS";
            else
                mixButtonText.text = english ? "FUSE" : "FUSIONAR";
            mixButtonText.color = roomPanel.CanExecuteFusion ? Color.white : Muted;
        }

        if (coolButton != null)
            coolButton.interactable = roomPanel.CanCoolCurrentInstability;
        if (coolButtonText != null)
        {
            if (roomPanel.CanCoolCurrentInstability)
                coolButtonText.text = english ? "COOL · 30 TRACES" : "ENFRIAR · 30 TRAZAS";
            else if (roomPanel.CurrentInstability <= 0)
                coolButtonText.text = english ? "COOL · SYSTEM STABLE" : "ENFRIAR · SISTEMA ESTABLE";
            else
                coolButtonText.text = english ? "COOL · MISSING TRACES" : "ENFRIAR · FALTAN TRAZAS";
            coolButtonText.color = roomPanel.CanCoolCurrentInstability
                ? Color.white
                : Muted;
        }
    }

    private void RefreshLogBadge()
    {
        int count = roomPanel.ExperimentalLogEntryCount;
        if (logBadgeRoot != null)
            logBadgeRoot.SetActive(count > 0);
        if (logBadgeText != null)
            logBadgeText.text = Mathf.Min(count, 99).ToString();
    }

    private void RefreshResult()
    {
        bool completed = roomPanel.HasCompletedFusion;
        ExperimentalResultType result = roomPanel.LastFusionResult;
        bool success = completed && result != ExperimentalResultType.None;
        bool english = roomPanel.IsEnglishUi;

        if (resultIcon != null)
            resultIcon.color = success ? Violet : completed ? Red : Muted;
        if (resultGlyph != null)
        {
            resultGlyph.text = success ? "+" : completed ? "!" : "R";
            resultGlyph.color = success ? Color.white : completed ? Red : Violet;
        }

        if (resultTitle != null)
        {
            if (!completed)
            {
                resultTitle.text = roomPanel.FusionStatusTitle;
                resultTitle.color = roomPanel.CanExecuteFusion ? Green : Amber;
            }
            else if (!success)
            {
                resultTitle.text = english
                    ? "RESULT: UNSTABLE FUSION"
                    : "RESULTADO: FUSIÓN INESTABLE";
                resultTitle.color = Red;
            }
            else
            {
                resultTitle.text = (english ? "RESULT: " : "RESULTADO: ") +
                    roomPanel.LastFusionResultDisplayName.ToUpperInvariant();
                resultTitle.color = Green;
            }
        }

        if (resultDetail != null)
        {
            resultDetail.text = !completed
                ? roomPanel.FusionGuidanceMessage
                : success
                    ? roomPanel.LastFusionRewardDisplayText.ToUpperInvariant()
                    : english ? "NO RECOVERABLE RESULT" : "SIN RESULTADO RECUPERABLE";
            resultDetail.color = completed ? Color.white : Muted;
        }

        if (resultMeta != null)
        {
            resultMeta.text = completed
                ? "+" + roomPanel.LastFusionInstabilityGain +
                  (english ? " INSTABILITY     ·     CORE " : " INESTABILIDAD     ·     NÚCLEO ") +
                  roomPanel.SynthesisCoreCounter + "/10"
                : english
                    ? "RISK AND REWARD UPDATE IN REAL TIME"
                    : "RIESGO Y RECOMPENSA SE ACTUALIZAN EN TIEMPO REAL";
            resultMeta.color = completed ? Violet : Muted;
        }
    }

    private void RefreshDiagnostics()
    {
        if (diagnosticText == null)
            return;

        int risk = Mathf.RoundToInt(roomPanel.CurrentFusionRisk01 * 100f);
        bool english = roomPanel.IsEnglishUi;
        string flow = roomPanel.FusionCoolingDown
            ? english ? "CONVERGENCE STABILIZING" : "CONVERGENCIA EN ESTABILIZACIÓN"
            : roomPanel.FusionSelectionComplete
                ? english ? "FLOW CONVERGENCE: OPTIMAL" : "CONVERGENCIA DE FLUJOS: ÓPTIMA"
                : english ? "FLOW CONVERGENCE: WAITING" : "CONVERGENCIA DE FLUJOS: EN ESPERA";
        diagnosticText.text =
            (english ? "CORE DIAGNOSTIC     ·     RISK " : "DIAGNÓSTICO DEL NÚCLEO     ·     RIESGO ") + risk + "%" +
            "     ·     " + roomPanel.InstabilityStateDisplayName.ToUpperInvariant() +
            "\n" + flow + (english ? "     ·     CORE " : "     ·     NÚCLEO ") +
            roomPanel.SynthesisCoreCounter + "/10";
    }

    private void ApplyPulse(float pulse)
    {
        float activity = roomPanel != null && roomPanel.FusionSelectionComplete
            ? roomPanel.FusionCoolingDown ? 1f : 0.78f
            : 0.22f;
        if (energyWave != null)
            energyWave.SetState(activity,
                roomPanel != null &&
                (roomPanel.FusionCoolingDown || roomPanel.HasCompletedFusion));
        if (cyanStream != null)
            cyanStream.color = WithAlpha(_cyanBase,
                Mathf.Lerp(0.28f, 1f, pulse) * activity);
        if (violetStream != null)
            violetStream.color = WithAlpha(_violetBase,
                Mathf.Lerp(0.28f, 1f, 1f - pulse) * activity);
        if (coreGlow != null)
            coreGlow.color = WithAlpha(_coreBase,
                Mathf.Lerp(0.42f, 1f, pulse) * Mathf.Lerp(0.45f, 1f, activity));
        if (coreTransform != null)
            coreTransform.localScale = Vector3.one *
                Mathf.Lerp(0.92f, 1.10f, pulse * activity);

        if (reactorParticles == null)
            return;
        for (int i = 0; i < reactorParticles.Length; i++)
        {
            Image particle = reactorParticles[i];
            if (particle == null) continue;
            float wave = 0.5f + 0.5f * Mathf.Sin(
                Time.unscaledTime * (4.5f + i * 0.18f) + i * 0.72f);
            Color baseColor = i % 2 == 0 ? Cyan : Violet;
            particle.color = WithAlpha(baseColor, wave * activity * 0.9f);
            particle.rectTransform.localScale = Vector3.one * Mathf.Lerp(0.5f, 1.35f, wave);
        }
    }

    private void AnimateDiagnostics()
    {
        if (diagnosticBars == null)
            return;
        for (int i = 0; i < diagnosticBars.Length; i++)
        {
            Image bar = diagnosticBars[i];
            if (bar == null) continue;
            float wave = 0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * 2.8f + i * 0.83f);
            RectTransform rect = bar.rectTransform;
            Vector2 max = rect.anchorMax;
            max.y = Mathf.Lerp(0.28f, 0.92f, wave);
            rect.anchorMax = max;
            bar.color = i % 3 == 0 ? Cyan : Violet;
        }
    }

    private static void SetSelectionOutline(Outline outline, bool selected, Color accent)
    {
        if (outline == null)
            return;
        outline.enabled = selected;
        outline.effectColor = WithAlpha(accent, 0.92f);
        outline.effectDistance = selected ? new Vector2(2f, -2f) : Vector2.zero;
    }

    private static void RefreshSegmentBar(Image[] segments, float normalized,
        Color activeLow, Color activeHigh)
    {
        if (segments == null || segments.Length == 0)
            return;
        int active = Mathf.Clamp(Mathf.CeilToInt(normalized * segments.Length),
            0, segments.Length);
        Color activeColor = Color.Lerp(activeLow, activeHigh,
            Mathf.Clamp01(normalized * 1.35f));
        for (int i = 0; i < segments.Length; i++)
        {
            if (segments[i] != null)
                segments[i].color = i < active ? activeColor : Inactive;
        }
    }

    private static Color WithAlpha(Color color, float alphaMultiplier)
    {
        color.a *= alphaMultiplier;
        return color;
    }

    private static Color Hex(string value)
    {
        ColorUtility.TryParseHtmlString("#" + value, out Color color);
        return color;
    }
}

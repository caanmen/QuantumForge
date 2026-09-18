using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Presentación operativa de Semillas y Anclajes. La simulación y las
/// transacciones continúan viviendo en GameState y MachinePanelUI.
/// </summary>
public sealed class MachineSeedsPanelVisualUI : MonoBehaviour
{
    private const float StateRefreshInterval = 0.1f;

    private static readonly Color Cyan = Hex("00D5D0");
    private static readonly Color CyanSoft = Hex("69EAD1");
    private static readonly Color Amber = Hex("F0A018");
    private static readonly Color Green = Hex("55DF9A");
    private static readonly Color Muted = Hex("6F7A82");
    private static readonly Color Track = Hex("152128");

    [Header("Resumen")]
    [SerializeField] private TextMeshProUGUI matureSeedsText;
    [SerializeField] private TextMeshProUGUI archiveUsageText;
    [SerializeField] private TextMeshProUGUI activeAnchorText;

    [Header("Incubación")]
    [SerializeField] private GameObject[] seedSlotRoots;
    [SerializeField] private TextMeshProUGUI[] seedSlotStateTexts;
    [SerializeField] private TextMeshProUGUI[] seedSlotMetaTexts;
    [SerializeField] private Image[] seedSlotProgressFills;

    [Header("Anclaje")]
    [SerializeField] private ChronalLatticeGraphic lattice;
    [SerializeField] private Image stabilityGauge;
    [SerializeField] private Image tensionGauge;
    [SerializeField] private TextMeshProUGUI stabilityValueText;
    [SerializeField] private TextMeshProUGUI tensionValueText;
    [SerializeField] private TextMeshProUGUI anchorStateText;
    [SerializeField] private TextMeshProUGUI thresholdText;
    [SerializeField] private TextMeshProUGUI forecastText;

    [Header("Controles")]
    [SerializeField] private Slider intensitySlider;
    [SerializeField] private TextMeshProUGUI intensityValueText;
    [SerializeField] private TextMeshProUGUI stabilizationEffectText;
    [SerializeField] private TextMeshProUGUI compensateEffectText;
    [SerializeField] private Button createSeedButton;
    [SerializeField] private Button formAnchorButton;
    [SerializeField] private Button stabilizeButton;
    [SerializeField] private Button compensateButton;
    [SerializeField] private Button materializeButton;
    [SerializeField] private Button discardButton;

    [Header("Archivo")]
    [SerializeField] private TextMeshProUGUI pureCountText;
    [SerializeField] private TextMeshProUGUI stableCountText;
    [SerializeField] private TextMeshProUGUI forcedCountText;
    [SerializeField] private Image archiveFill;
    private float nextStateRefreshTime;

    private void OnEnable()
    {
        nextStateRefreshTime = 0f;
        RefreshState();
    }

    private void LateUpdate()
    {
        if (Time.unscaledTime < nextStateRefreshTime)
            return;
        nextStateRefreshTime = Time.unscaledTime + StateRefreshInterval;
        RefreshState();
    }

    private void RefreshState()
    {
        GameState state = GameState.I;
        if (state == null)
            return;

        state.EnsureChronalSeedSlots();
        bool hasAnchor = state.chronalInstant != null &&
            state.chronalInstant.hasInstant;
        int used = state.GetChronalArchiveUsed();
        int capacity = state.GetChronalArchiveCapacity();

        SetText(matureSeedsText, "SEMILLAS MADURAS  " +
            state.chronalMatureSeedsStored);
        SetText(archiveUsageText, "ARCHIVO  " + used + " / " + capacity);
        SetText(activeAnchorText, hasAnchor
            ? "ANCLAJE ACTIVO"
            : "CÁMARA DISPONIBLE");

        RefreshSeedSlots(state);
        RefreshAnchor(state, hasAnchor);
        RefreshControls(state, hasAnchor);
        RefreshArchive(state, used, capacity);
    }

    private void RefreshSeedSlots(GameState state)
    {
        int unlocked = state.chronalSeedSlots != null
            ? state.chronalSeedSlots.Count
            : 0;
        int readingLevel = state.GetChronalSeedReadingLevel();
        int count = seedSlotRoots != null ? seedSlotRoots.Length : 0;
        for (int i = 0; i < count; i++)
        {
            bool available = i < unlocked;
            if (seedSlotRoots[i] != null)
                seedSlotRoots[i].SetActive(true);

            ChronalSeedSlotState slot = available
                ? state.chronalSeedSlots[i]
                : null;
            bool growing = slot != null && slot.hasSeed;
            float progress = growing && state.chronalSeedDurationSeconds > 0.0
                ? Mathf.Clamp01((float)(slot.progressSeconds /
                    state.chronalSeedDurationSeconds))
                : 0f;

            string stateLabel;
            string meta;
            Color accent;
            if (!available)
            {
                stateLabel = "SLOT " + (i + 1) + "  ·  BLOQUEADO";
                meta = "REPARA EL MÓDULO DE INCUBACIÓN";
                accent = Muted;
            }
            else if (!growing)
            {
                stateLabel = "SLOT " + (i + 1) + "  ·  VACÍO";
                meta = "LISTO PARA NUEVA SEMILLA";
                accent = Muted;
            }
            else if (readingLevel <= 0)
            {
                stateLabel = "SLOT " + (i + 1) + "  ·  MADURANDO";
                meta = "LECTURA NO CALIBRADA";
                accent = Cyan;
                progress = 0.18f + 0.10f *
                    (0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * 2.4f + i));
            }
            else
            {
                int percent = Mathf.FloorToInt(progress * 100f);
                stateLabel = "SLOT " + (i + 1) + "  ·  MADURANDO " +
                    percent + "%";
                if (readingLevel >= 2)
                {
                    double remaining = System.Math.Max(0.0,
                        state.chronalSeedDurationSeconds - slot.progressSeconds);
                    meta = remaining.ToString("0.0") + " S RESTANTES";
                }
                else
                {
                    meta = "LECTURA DE FASE ACTIVA";
                }
                accent = CyanSoft;
            }

            SetText(Get(seedSlotStateTexts, i), stateLabel);
            SetText(Get(seedSlotMetaTexts, i), meta);
            TextMeshProUGUI stateText = Get(seedSlotStateTexts, i);
            if (stateText != null)
                stateText.color = accent;
            Image fill = Get(seedSlotProgressFills, i);
            if (fill != null)
            {
                fill.fillAmount = progress;
                fill.color = growing ? accent : Track;
            }
        }
    }

    private void RefreshAnchor(GameState state, bool hasAnchor)
    {
        double stability = hasAnchor ? state.chronalInstant.stability : 0.0;
        double tension = hasAnchor ? state.chronalInstant.tension : 0.0;
        double threshold = state.GetChronalMaterializationThreshold();

        if (stabilityGauge != null)
        {
            stabilityGauge.fillAmount = (float)(stability / 100.0);
            stabilityGauge.color = stability >= threshold ? Green : Cyan;
        }
        if (tensionGauge != null)
        {
            tensionGauge.fillAmount = (float)(tension / 100.0);
            tensionGauge.color = tension > 30.0 ? Hex("FF5E57") : Amber;
        }

        SetText(stabilityValueText, stability.ToString("0") + "%");
        SetText(tensionValueText, tension.ToString("0") + "%");
        SetText(anchorStateText, hasAnchor
            ? "MEMBRANA CRONAL CONTENIDA"
            : "SIN ANCLAJE EN CÁMARA");
        SetText(thresholdText, "UMBRAL DE FIJACIÓN  " +
            threshold.ToString("0") + "%");

        string forecast;
        Color forecastColor;
        if (!hasAnchor)
        {
            forecast = "FORMA UN ANCLAJE PARA INICIAR LA LECTURA";
            forecastColor = Muted;
        }
        else if (stability < threshold)
        {
            forecast = "FIJACIÓN PENDIENTE  ·  FALTA " +
                (threshold - stability).ToString("0") + "% ESTABILIDAD";
            forecastColor = Amber;
        }
        else
        {
            forecast = "RESULTADO PREVISTO  ·  " +
                state.GetChronalMaterializationQualityName(tension).ToUpperInvariant();
            forecastColor = tension <= 15.0 ? CyanSoft :
                tension <= 30.0 ? Green : Amber;
        }
        SetText(forecastText, forecast);
        if (forecastText != null)
            forecastText.color = forecastColor;
        if (lattice != null)
            lattice.SetState(hasAnchor ? 1f : 0.08f, (float)(tension / 100.0));
    }

    private void RefreshControls(GameState state, bool hasAnchor)
    {
        int intensity = intensitySlider != null
            ? state.NormalizeChronalStabilizationIntensity(intensitySlider.value)
            : 50;
        SetText(intensityValueText, intensity + "%");
        SetText(stabilizationEffectText,
            "+" + state.GetChronalStabilityGainForIntensity(intensity).ToString("0") +
            "% ESTABILIDAD  ·  +" +
            state.GetChronalTensionGainForIntensity(intensity).ToString("0") +
            "% TENSIÓN");
        SetText(compensateEffectText,
            "COMPENSAR  −" + state.GetChronalRewindTensionReduction().ToString("0") +
            "% TENSIÓN  ·  −" +
            state.GetChronalRewindStabilityLoss().ToString("0") + "% ESTABILIDAD");

        SetButtonLabel(createSeedButton, HasFreeSeedSlot(state)
            ? "CREAR SEMILLA"
            : "INCUBADOR LLENO");
        SetButtonLabel(formAnchorButton, hasAnchor
            ? "ANCLAJE EN CÁMARA"
            : state.chronalMatureSeedsStored > 0
                ? "FORMAR ANCLAJE"
                : "SIN SEMILLAS MADURAS");
        SetButtonLabel(materializeButton,
            hasAnchor && state.chronalInstant.stability >=
                state.GetChronalMaterializationThreshold()
                ? "FIJAR"
                : "FIJAR");
    }

    private void RefreshArchive(GameState state, int used, int capacity)
    {
        SetText(pureCountText, "PUROS  " + state.chronalPureInstants);
        SetText(stableCountText, "ESTABLES  " + state.chronalStableInstants);
        SetText(forcedCountText, "FORZADOS  " + state.chronalForcedInstants);
        if (archiveFill != null)
        {
            archiveFill.fillAmount = capacity > 0
                ? Mathf.Clamp01((float)used / capacity)
                : 0f;
            archiveFill.color = state.IsChronalArchiveFull() ? Amber : Cyan;
        }
    }

    private static bool HasFreeSeedSlot(GameState state)
    {
        if (state.chronalSeedSlots == null)
            return false;
        for (int i = 0; i < state.chronalSeedSlots.Count; i++)
        {
            ChronalSeedSlotState slot = state.chronalSeedSlots[i];
            if (slot == null || !slot.hasSeed)
                return true;
        }
        return false;
    }

    private static T Get<T>(T[] array, int index) where T : Object
    {
        return array != null && index >= 0 && index < array.Length
            ? array[index]
            : null;
    }

    private static void SetButtonLabel(Button button, string value)
    {
        if (button == null)
            return;
        TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>();
        SetText(label, value);
    }

    private static void SetText(TextMeshProUGUI target, string value)
    {
        if (target != null)
            target.text = value;
    }

    private static Color Hex(string value)
    {
        ColorUtility.TryParseHtmlString("#" + value, out Color parsed);
        return parsed;
    }
}

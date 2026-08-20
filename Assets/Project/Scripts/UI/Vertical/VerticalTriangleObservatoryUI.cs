using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class VerticalTriangleObservatoryUI : MonoBehaviour
{
    private static readonly string[] TriangleStudyIds =
    {
        "study_triangle_impulse_tuning",
        "study_triangle_synergy_resonance",
        "study_triangle_persistence_anchor",
        "study_experimental_chamber_keycard"
    };

    public TMP_Text titleText;
    public TMP_Text statusText;
    public TMP_Text detailText;
    public Slider progressSlider;
    public Button conclusionButton;
    public TMP_Text conclusionButtonText;
    public Button[] opportunityButtons;
    public TMP_Text[] opportunityLabels;
    public GameObject tuningRoot;
    public Slider amplitudeSlider;
    public Slider frequencySlider;
    public TMP_Text tuningHintText;
    public Button tuneButton;
    public TMP_Text tuneButtonText;

    private const float RefreshInterval = 0.15f;
    private float nextRefresh;

    private void Awake()
    {
        BindOpportunityButtons();
        BindTuningControls();
    }

    private void OnEnable()
    {
        BindOpportunityButtons();
        BindTuningControls();
        nextRefresh = 0f;
        RefreshNow();
    }

    private void Update()
    {
        if (Time.unscaledTime < nextRefresh) return;
        nextRefresh = Time.unscaledTime + RefreshInterval;
        RefreshNow();
    }

    public void OnClickConclusion()
    {
        GameState state = GameState.I;
        if (state == null || !UpgradeStudySystem.IsConclusionPending(state)) return;
        if (UpgradeStudySystem.TryRevealConclusion(state, out _))
            SaveService.I?.Save();
        RefreshNow();
    }

    public void StartOpportunity(int index)
    {
        if (index < 0 || index >= TriangleStudyIds.Length || GameState.I == null)
            return;
        if (UpgradeStudySystem.TryStartStudy(GameState.I, TriangleStudyIds[index]))
            SaveService.I?.Save();
        RefreshNow();
    }

    public void OnTuningChanged(float _)
    {
        if (GameState.I == null || amplitudeSlider == null || frequencySlider == null)
            return;
        UpgradeStudySystem.SetTuningValues(
            GameState.I, amplitudeSlider.value, frequencySlider.value);
        RefreshTuning(GameState.I);
    }

    public void OnClickTune()
    {
        if (GameState.I == null) return;
        UpgradeStudySystem.SetTuningValues(
            GameState.I,
            amplitudeSlider != null ? amplitudeSlider.value : 0.5f,
            frequencySlider != null ? frequencySlider.value : 0.5f);
        UpgradeStudySystem.TryApplyActiveTuning(
            GameState.I, out _, out _);
        SaveService.I?.Save();
        RefreshNow();
    }

    [ContextMenu("Refresh observatory")]
    public void RefreshNow()
    {
        GameState state = GameState.I;
        if (state == null) return;

        if (titleText != null)
            titleText.SetText(L("study.console.title", "SINTONIZACIÓN EXPERIMENTAL"));

        UpgradeStudyDef active = UpgradeStudySystem.GetActiveStudy(state);
        bool conclusion = active != null && UpgradeStudySystem.IsConclusionPending(state);
        if (progressSlider != null)
        {
            progressSlider.gameObject.SetActive(active != null);
            progressSlider.value = active == null
                ? 0f
                : (float)UpgradeStudySystem.GetActiveProgress01(state);
        }

        if (conclusionButton != null)
        {
            conclusionButton.gameObject.SetActive(conclusion);
            conclusionButton.interactable = conclusion;
        }
        if (conclusionButtonText != null)
            conclusionButtonText.SetText(L("study.action.reveal", "Revelar"));

        if (active != null)
            RefreshActive(state, active, conclusion);
        else
            RefreshAvailable(state);
    }

    private void RefreshActive(GameState state, UpgradeStudyDef active, bool conclusion)
    {
        SetConsoleHeight(conclusion ? 360f : 600f);
        if (statusText != null)
            statusText.SetText(conclusion
                ? L("study.status.conclusion", "Conclusión disponible")
                : UpgradeStudySystem.IsActiveStudyProgressing(state)
                    ? L("study.status.active", "Investigando")
                    : L("observatory.paused", "Estudio en pausa"));

        if (detailText != null)
        {
            string title = L($"study.{active.id}.title", active.id);
            string remaining = FormatDuration(
                UpgradeStudySystem.GetRemainingSeconds(state));
            detailText.SetText(conclusion
                ? title
                : $"{title}\n" + LF("study.progress", "Progreso: {0:0}% · {1}",
                    UpgradeStudySystem.GetActiveProgress01(state) * 100.0,
                    remaining));
        }

        SetAllOpportunitiesHidden();
        RefreshTuning(state);
    }

    private void RefreshAvailable(GameState state)
    {
        SetConsoleHeight(300f);
        if (statusText != null)
            statusText.SetText(L("observatory.available", "Consola disponible"));
        SetAllOpportunitiesHidden();
        if (detailText != null)
            detailText.SetText(L("observatory.choose_upgrade",
                "Selecciona una mejora sin descubrir para iniciar su estudio."));
        if (tuningRoot != null) tuningRoot.SetActive(false);
        if (tuneButton != null) tuneButton.gameObject.SetActive(false);
    }

    private void BindOpportunityButtons()
    {
        if (opportunityButtons == null) return;
        for (int index = 0; index < opportunityButtons.Length; index++)
        {
            Button button = opportunityButtons[index];
            if (button == null) continue;
            int captured = index;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => StartOpportunity(captured));
        }
        if (conclusionButton != null)
        {
            conclusionButton.onClick.RemoveListener(OnClickConclusion);
            conclusionButton.onClick.AddListener(OnClickConclusion);
        }
    }

    private void BindTuningControls()
    {
        if (amplitudeSlider != null)
        {
            amplitudeSlider.onValueChanged.RemoveListener(OnTuningChanged);
            amplitudeSlider.onValueChanged.AddListener(OnTuningChanged);
        }
        if (frequencySlider != null)
        {
            frequencySlider.onValueChanged.RemoveListener(OnTuningChanged);
            frequencySlider.onValueChanged.AddListener(OnTuningChanged);
        }
        if (tuneButton != null)
        {
            tuneButton.onClick.RemoveListener(OnClickTune);
            tuneButton.onClick.AddListener(OnClickTune);
        }
    }

    private void RefreshTuning(GameState state)
    {
        UpgradeStudyDef active = UpgradeStudySystem.GetActiveStudy(state);
        int stage = UpgradeStudySystem.GetTuningStage(state);
        int stageCount = UpgradeStudySystem.GetTuningStageCount(state);
        bool visible = active != null && !UpgradeStudySystem.IsConclusionPending(state) &&
            stage < stageCount;
        if (tuningRoot != null) tuningRoot.SetActive(visible);
        if (tuneButton != null) tuneButton.gameObject.SetActive(visible);
        if (!visible) return;

        UpgradeStudyState progress = UpgradeStudySystem.EnsureState(state);
        if (amplitudeSlider != null)
            amplitudeSlider.SetValueWithoutNotify(progress.tuningAmplitude);
        if (frequencySlider != null)
            frequencySlider.SetValueWithoutNotify(progress.tuningFrequency);

        UpgradeStudySystem.GetTuningTarget(
            state, out float targetAmplitude, out float targetFrequency);
        double accuracy = UpgradeStudySystem.GetTuningAccuracy(state);
        if (tuningHintText != null)
        {
            tuningHintText.SetText(
                $"Firma {stage + 1}/{stageCount} · " +
                $"Amplitud {Band(targetAmplitude)} · Frecuencia {Band(targetFrequency)}\n" +
                $"Coincidencia: {accuracy * 100.0:0}%");
        }
        if (tuneButton != null) tuneButton.interactable = accuracy >= 0.86;
        if (tuneButtonText != null)
            tuneButtonText.SetText(accuracy >= 0.86
                ? L("study.tuning.apply", "ESTABILIZAR")
                : L("study.tuning.adjust", "AJUSTA LA SEÑAL"));
    }

    private static string Band(float value)
    {
        if (value < 0.38f) return "BAJA";
        if (value > 0.62f) return "ALTA";
        return "MEDIA";
    }

    private TMP_Text GetOpportunityLabel(int index)
    {
        if (opportunityLabels != null && index < opportunityLabels.Length &&
            opportunityLabels[index] != null)
            return opportunityLabels[index];
        if (opportunityButtons == null || index >= opportunityButtons.Length ||
            opportunityButtons[index] == null)
            return null;
        return opportunityButtons[index].GetComponentInChildren<TMP_Text>(true);
    }

    private void SetAllOpportunitiesHidden()
    {
        if (opportunityButtons == null) return;
        for (int index = 0; index < opportunityButtons.Length; index++)
            SetOpportunityVisible(index, false);
    }

    private void SetOpportunityVisible(int index, bool visible)
    {
        if (opportunityButtons == null || index < 0 ||
            index >= opportunityButtons.Length || opportunityButtons[index] == null)
            return;
        opportunityButtons[index].gameObject.SetActive(visible);
    }

    private static string FormatDuration(double seconds)
    {
        int total = Mathf.Max(0, Mathf.CeilToInt((float)seconds));
        return $"{total / 60}:{total % 60:00}";
    }

    private void SetConsoleHeight(float height)
    {
        LayoutElement layout = GetComponent<LayoutElement>();
        if (layout == null) return;
        layout.preferredHeight = height;
        layout.minHeight = Mathf.Min(height, 230f);
    }

    private static string L(string key, string fallback)
    {
        LocalizationManager lm = LocalizationManager.I;
        if (lm == null) return fallback;
        string value = lm.T(key);
        return string.IsNullOrEmpty(value) || value == key ? fallback : value;
    }

    private static string LF(string key, string fallback, params object[] args)
    {
        string format = L(key, fallback);
        try { return string.Format(format, args); }
        catch { return format; }
    }
}

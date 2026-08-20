using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(120)]
public sealed class GenerationLabQuarantineUI : MonoBehaviour
{
    [Header("Banner")]
    public TextMeshProUGUI quarantineBannerText;

    [Header("Circuito real")]
    public TextMeshProUGUI activeSectionTitle;
    public TextMeshProUGUI activeCircuitValue;
    public TextMeshProUGUI synchronizationTitle;
    public TextMeshProUGUI synchronizationValue;
    public TextMeshProUGUI synchronizationEta;
    public Image synchronizationFill;
    public Image activeAccent;

    [Header("Efecto real")]
    public TextMeshProUGUI effectSectionTitle;
    public TextMeshProUGUI activeEffectValue;
    public TextMeshProUGUI benefitTitle;
    public TextMeshProUGUI benefitValue;
    public TextMeshProUGUI sacrificeTitle;
    public TextMeshProUGUI sacrificeValue;
    public Image effectAccent;

    [Header("Paleta")]
    public Color energyColor = new(0.01f, 0.30f, 0.76f, 1f);
    public Color experimentalColor = new(0.80f, 0.33f, 1f, 1f);
    public Color phaseColor = new(1f, 0.58f, 0.10f, 1f);
    public Color inactiveColor = new(0.34f, 0.47f, 0.56f, 1f);

    private float refreshTimer;
    private int lastLanguage = -1;

    private void OnEnable()
    {
        refreshTimer = 0f;
        Refresh(true);
    }

    private void Update()
    {
        refreshTimer -= Time.unscaledDeltaTime;
        int language = LocalizationManager.I != null
            ? (int)LocalizationManager.I.CurrentLanguage
            : -1;
        if (refreshTimer <= 0f || language != lastLanguage)
        {
            refreshTimer = 0.15f;
            lastLanguage = language;
            Refresh(false);
        }

        float pulse = 0.82f + 0.18f * Mathf.Sin(Time.unscaledTime * 2.6f);
        PulseAccent(activeAccent, pulse);
        PulseAccent(effectAccent, pulse * 0.92f);
    }

    private void Refresh(bool force)
    {
        bool english = LocalizationManager.I != null &&
            LocalizationManager.I.CurrentLanguage == LocalizationManager.Language.EN;
        SetText(activeSectionTitle,
            english ? "ACTIVE CIRCUIT" : "CIRCUITO ACTIVO", force);
        SetText(synchronizationTitle,
            english ? "SYNCHRONIZATION" : "SINCRONIZACIÓN", force);
        SetText(effectSectionTitle,
            english ? "CURRENT EFFECT" : "EFECTO ACTUAL", force);
        SetText(benefitTitle,
            english ? "BONUS" : "BONIFICACIÓN", force);
        SetText(sacrificeTitle,
            english ? "PENALTY" : "PENALIZACIÓN", force);

        GameState state = GameState.I;
        bool available = state != null && state.CanUseTriangleCircuits();
        TriangleCircuitType circuit = available
            ? state.triangleActiveCircuit
            : TriangleCircuitType.None;
        Color accent = GetAccent(circuit);
        SetText(activeEffectValue,
            circuit == TriangleCircuitType.None
                ? "--"
                : CircuitName(circuit, english), force);

        if (!available)
        {
            SetText(activeCircuitValue,
                english ? "NO COUPLING" : "SIN ACOPLE", force);
            SetText(activeEffectValue, "--", force);
            SetText(synchronizationValue, "--", force);
            SetText(synchronizationEta,
                english ? "Install all three vertices" : "Instala los tres vértices",
                force);
            SetText(benefitValue, english ? "No effect" : "Sin efecto", force);
            SetText(sacrificeValue, "--", force);
            SetFill(0f, inactiveColor);
            ApplyAccent(inactiveColor);
            return;
        }

        if (circuit == TriangleCircuitType.None)
        {
            SetText(activeCircuitValue,
                english ? "CHOOSE" : "ELIGE", force);
            SetText(activeEffectValue, "--", force);
            SetText(synchronizationValue, "0%", force);
            SetText(synchronizationEta,
                english ? "Select one circuit" : "Selecciona un circuito", force);
            SetText(benefitValue, english ? "No effect" : "Sin efecto", force);
            SetText(sacrificeValue, "--", force);
            SetFill(0f, inactiveColor);
            ApplyAccent(inactiveColor);
            return;
        }

        int percentage = Mathf.RoundToInt(state.triangleSynchronization * 100f);
        int remaining = Mathf.CeilToInt(
            (float)state.GetTriangleSynchronizationRemainingSeconds());
        SetText(activeCircuitValue, CircuitName(circuit, english), force);
        SetText(synchronizationValue, percentage + "%", force);
        SetText(synchronizationEta,
            remaining > 0
                ? (english ? "Stable in " : "Estable en ") + remaining + " s"
                : (english ? "STABLE" : "ESTABLE"),
            force);
        SetText(benefitValue, BuildBenefit(state, circuit, english), force);
        SetText(sacrificeValue, BuildSacrifice(circuit, english), force);
        SetFill(Mathf.Clamp01(state.triangleSynchronization), accent);
        ApplyAccent(accent);
    }

    private string BuildBenefit(
        GameState state,
        TriangleCircuitType circuit,
        bool english)
    {
        if (circuit == TriangleCircuitType.Energy)
        {
            double bonus = (state.GetTriangleLEMultiplier() - 1.0) * 100.0;
            return "+" + bonus.ToString("0.#") + "%\nLE";
        }

        if (circuit == TriangleCircuitType.Experimental)
        {
            double traces = (state.GetTriangleTracesMultiplier() - 1.0) * 100.0;
            if (!state.experimentalChamberUnlocked)
                return "+" + traces.ToString("0.#") + "%\n" +
                    (english ? "TRACES" : "TRAZAS");
            double fragments = (state.GetTriangleFragmentMultiplier() - 1.0) * 100.0;
            return "+" + traces.ToString("0.#") + "%\n" +
                (english ? "TRACES" : "TRAZAS") + "\n+" +
                fragments.ToString("0.#") + "%\n" +
                (english ? "FRAGMENTS" : "FRAGMENTOS");
        }

        double energy = (state.GetTriangleEnergyFocusMultiplier() - 1.0) * 100.0;
        return "+" + energy.ToString("0.#") + "%\n" +
                (english ? "ENERGY" : "ENERGÍA");
    }

    private static string BuildSacrifice(
        TriangleCircuitType circuit,
        bool english)
    {
        if (circuit == TriangleCircuitType.Energy)
            return "-10%\n" + (english ? "TRACES" : "TRAZAS");
        if (circuit == TriangleCircuitType.Experimental)
            return "-10% LE";
        return "-10%\nLE / " + (english ? "TRACES" : "TRAZAS");
    }

    private static string CircuitName(
        TriangleCircuitType circuit,
        bool english)
    {
        if (circuit == TriangleCircuitType.Energy) return "LE";
        if (circuit == TriangleCircuitType.Experimental)
            return english ? "TRACES" : "TRAZAS";
        if (circuit == TriangleCircuitType.Phase)
            return english ? "ENERGY" : "ENERG\u00cdA";
        return "--";
    }

    private Color GetAccent(TriangleCircuitType circuit)
    {
        if (circuit == TriangleCircuitType.Energy) return energyColor;
        if (circuit == TriangleCircuitType.Experimental) return experimentalColor;
        if (circuit == TriangleCircuitType.Phase) return phaseColor;
        return inactiveColor;
    }

    private void SetFill(float value, Color color)
    {
        if (synchronizationFill == null) return;
        synchronizationFill.fillAmount = value;
        synchronizationFill.color = color;
    }

    private void ApplyAccent(Color color)
    {
        if (activeAccent != null) activeAccent.color = color;
        if (effectAccent != null) effectAccent.color = color;
        if (activeCircuitValue != null) activeCircuitValue.color = color;
        if (synchronizationValue != null) synchronizationValue.color = color;
        if (benefitValue != null) benefitValue.color = color;
        if (activeEffectValue != null) activeEffectValue.color = color;
    }

    private static void PulseAccent(Image image, float pulse)
    {
        if (image == null) return;
        Color color = image.color;
        color.a = Mathf.Lerp(0.62f, 0.96f, pulse);
        image.color = color;
    }

    private static void SetText(
        TextMeshProUGUI label,
        string value,
        bool force)
    {
        if (label != null && (force || label.text != value))
            label.SetText(value);
    }
}

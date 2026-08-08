using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(100)]
public sealed class VerticalGenerationPolishUI : MonoBehaviour
{
    [Header("Medidor")]
    public Image gaugeFrame;
    public Image gaugeGlow;
    public Image gaugeProgressRing;
    public TextMeshProUGUI gaugeCircuitText;
    public TextMeshProUGUI gaugeProgressText;

    [Header("Jerarquia textual")]
    public TextMeshProUGUI synchronizationText;
    public TextMeshProUGUI effectText;

    [Header("Haces")]
    public Image energyBeamRoot;
    public Image experimentalBeamRoot;
    public Image phaseBeamRoot;
    public Image energyBeamGlow;
    public Image experimentalBeamGlow;
    public Image phaseBeamGlow;
    public Image energyBeamCore;
    public Image experimentalBeamCore;
    public Image phaseBeamCore;
    public Image energyBeamHotCore;
    public Image experimentalBeamHotCore;
    public Image phaseBeamHotCore;
    public RawImage energyChevrons;
    public RawImage experimentalChevrons;
    public RawImage phaseChevrons;
    public Image energyStartConnector;
    public Image energyEndConnector;
    public Image experimentalStartConnector;
    public Image experimentalEndConnector;
    public Image phaseStartConnector;
    public Image phaseEndConnector;
    public TextMeshProUGUI energyFlow;
    public TextMeshProUGUI experimentalFlow;
    public TextMeshProUGUI phaseFlow;

    [Header("Selectores")]
    public Image energyCircuitBorder;
    public Image experimentalCircuitBorder;
    public Image phaseCircuitBorder;

    [Header("Nodos")]
    public RectTransform higgsNode;
    public RectTransform tetraNode;
    public RectTransform modulatorNode;

    public Color energyColor = new(0f, 0.84f, 1f, 1f);
    public Color experimentalColor = new(0.73f, 0.31f, 0.93f, 1f);
    public Color phaseColor = new(1f, 0.60f, 0.13f, 1f);
    public Color inactiveBorderColor = new(0.06f, 0.22f, 0.32f, 0.9f);

    private Vector3 higgsBaseScale = Vector3.one;
    private Vector3 tetraBaseScale = Vector3.one;
    private Vector3 modulatorBaseScale = Vector3.one;

    private void OnEnable()
    {
        if (higgsNode != null) higgsBaseScale = higgsNode.localScale;
        if (tetraNode != null) tetraBaseScale = tetraNode.localScale;
        if (modulatorNode != null) modulatorBaseScale = modulatorNode.localScale;
        RefreshVisuals();
    }

    private void Update()
    {
        RefreshVisuals();
    }

    private void RefreshVisuals()
    {
        GameState state = GameState.I;
        TriangleCircuitType active = state != null
            ? state.triangleActiveCircuit
            : TriangleCircuitType.None;
        bool available = state != null && state.triangleSystemUnlocked &&
            state.AreTriangleVerticesAvailable();
        bool phaseLocked = false;
        float pulse = 0.84f + 0.16f * Mathf.Sin(Time.unscaledTime * 3.4f);

        Color accent = GetAccent(active);
        SetGauge(state, active, available, accent, pulse);
        float synchronization = state != null
            ? Mathf.Clamp01(state.triangleSynchronization)
            : 0f;
        SetBeam(energyBeamRoot, energyBeamGlow, energyBeamCore, energyBeamHotCore,
            energyFlow, energyChevrons, energyStartConnector, energyEndConnector,
            available && active == TriangleCircuitType.Energy, false,
            energyColor, pulse, synchronization);
        SetBeam(experimentalBeamRoot, experimentalBeamGlow,
            experimentalBeamCore, experimentalBeamHotCore,
            experimentalFlow, experimentalChevrons,
            experimentalStartConnector, experimentalEndConnector,
            available && active == TriangleCircuitType.Experimental, false,
            experimentalColor, pulse, synchronization);
        SetBeam(phaseBeamRoot, phaseBeamGlow, phaseBeamCore, phaseBeamHotCore,
            phaseFlow, phaseChevrons, phaseStartConnector, phaseEndConnector,
            available && active == TriangleCircuitType.Phase, phaseLocked,
            phaseColor, pulse, synchronization);

        SetBorder(energyCircuitBorder,
            available && active == TriangleCircuitType.Energy, !available,
            energyColor, pulse);
        SetBorder(experimentalCircuitBorder,
            available && active == TriangleCircuitType.Experimental, !available,
            experimentalColor, pulse);
        SetBorder(phaseCircuitBorder,
            available && active == TriangleCircuitType.Phase,
            !available || phaseLocked, phaseColor, pulse);

        bool higgsActive = active == TriangleCircuitType.Energy ||
            active == TriangleCircuitType.Experimental;
        bool tetraActive = active == TriangleCircuitType.Experimental ||
            active == TriangleCircuitType.Phase;
        bool modulatorActive = active == TriangleCircuitType.Energy ||
            active == TriangleCircuitType.Phase;
        PulseNode(higgsNode, higgsBaseScale, available && higgsActive, pulse);
        PulseNode(tetraNode, tetraBaseScale, available && tetraActive, pulse);
        PulseNode(modulatorNode, modulatorBaseScale,
            available && modulatorActive, pulse);
        ApplyTextHierarchy(accent);
    }

    private void SetGauge(
        GameState state,
        TriangleCircuitType active,
        bool available,
        Color accent,
        float pulse)
    {
        if (gaugeFrame != null)
        {
            Color tint = active == TriangleCircuitType.None
                ? new Color(0.35f, 0.50f, 0.62f, 0.82f)
                : Color.Lerp(Color.white, accent, 0.22f);
            tint.a = available ? 0.92f + 0.08f * pulse : 0.55f;
            gaugeFrame.color = tint;
        }
        if (gaugeGlow != null)
        {
            Color glow = active == TriangleCircuitType.None
                ? inactiveBorderColor
                : accent;
            glow.a = available ? 0.16f + 0.10f * pulse : 0.07f;
            gaugeGlow.color = glow;
        }
        if (gaugeProgressRing != null)
        {
            gaugeProgressRing.fillAmount = available && state != null
                ? Mathf.Clamp01(state.triangleSynchronization)
                : 0f;
            Color ring = active == TriangleCircuitType.None
                ? inactiveBorderColor
                : accent;
            ring.a = available ? 0.90f + 0.10f * pulse : 0.25f;
            gaugeProgressRing.color = ring;
        }

        if (gaugeCircuitText != null)
            gaugeCircuitText.SetText(GetCircuitName(active));
        if (gaugeProgressText != null)
        {
            if (!available || active == TriangleCircuitType.None || state == null)
            {
                gaugeProgressText.SetText("--");
                gaugeProgressText.color = new Color(0.55f, 0.64f, 0.72f, 1f);
            }
            else
            {
                int percent = Mathf.RoundToInt(state.triangleSynchronization * 100f);
                int remaining = Mathf.CeilToInt(
                    (float)state.GetTriangleSynchronizationRemainingSeconds());
                gaugeProgressText.SetText(remaining > 0
                    ? $"{percent}% · {remaining} s"
                    : $"{percent}%");
                gaugeProgressText.color = accent;
            }
        }
    }

    private void SetBeam(
        Image root,
        Image glow,
        Image core,
        Image hotCore,
        TextMeshProUGUI flow,
        RawImage chevrons,
        Image startConnector,
        Image endConnector,
        bool active,
        bool locked,
        Color accent,
        float pulse,
        float synchronization)
    {
        if (root != null)
            root.color = locked
                ? new Color(0.018f, 0.032f, 0.045f, 0.42f)
                : new Color(0.018f, 0.050f, 0.068f, 0.96f);

        float activeStrength = Mathf.Lerp(0.35f, 0.95f, synchronization);
        activeStrength *= 0.92f + 0.08f * pulse;
        if (glow != null)
        {
            Color glowColor = accent;
            glowColor.a = locked ? 0.025f : active
                ? Mathf.Lerp(0.12f, 0.24f, synchronization)
                : 0.075f;
            glow.color = glowColor;
        }
        if (core != null)
        {
            Color color = accent;
            color.a = locked ? 0.08f : active ? activeStrength : 0.28f;
            core.color = color;
        }
        if (hotCore != null)
        {
            Color hot = Color.Lerp(Color.white, accent, 0.30f);
            hot.a = locked ? 0f : active
                ? Mathf.Lerp(0.25f, 0.82f, synchronization)
                : 0.06f;
            hotCore.color = hot;
        }

        if (chevrons != null)
        {
            chevrons.gameObject.SetActive(active && !locked);
            Color flowColor = Color.Lerp(Color.white, accent, 0.42f);
            flowColor.a = Mathf.Lerp(0.55f, 0.78f, synchronization);
            chevrons.color = flowColor;
            Rect uv = chevrons.uvRect;
            uv.x = Mathf.Repeat(Time.unscaledTime *
                Mathf.Lerp(0.20f, 0.48f, synchronization), 1f);
            chevrons.uvRect = uv;
        }
        if (flow != null)
        {
            flow.gameObject.SetActive(active && !locked);
            Color flowTextColor = Color.Lerp(Color.white, accent, 0.34f);
            flowTextColor.a = Mathf.Lerp(0.50f, 0.76f, synchronization);
            flow.color = flowTextColor;
            if (flow.gameObject.activeSelf)
            {
                RectTransform flowRect = flow.rectTransform;
                flowRect.anchoredPosition = new Vector2(
                    Mathf.Repeat(Time.unscaledTime *
                        Mathf.Lerp(24f, 42f, synchronization), 36f) - 18f,
                    0f);
            }
        }

        SetConnector(startConnector, accent, active, locked,
            activeStrength, pulse);
        SetConnector(endConnector, accent, active, locked,
            activeStrength, pulse);
    }

    private static void SetConnector(
        Image connector,
        Color accent,
        bool active,
        bool locked,
        float activeStrength,
        float pulse)
    {
        if (connector == null) return;
        Color color = accent;
        color.a = locked ? 0.10f : active
            ? Mathf.Clamp01(0.60f + activeStrength * 0.36f)
            : 0.28f;
        connector.color = color;
        connector.rectTransform.localScale = active
            ? Vector3.one * (0.96f + 0.05f * pulse)
            : Vector3.one;
    }

    private void SetBorder(
        Image border,
        bool active,
        bool locked,
        Color accent,
        float pulse)
    {
        if (border == null) return;
        Color color = locked
            ? new Color(0.22f, 0.28f, 0.34f, 0.38f)
            : active
                ? accent
                : inactiveBorderColor;
        if (active) color.a = 0.82f + 0.18f * pulse;
        border.color = color;
    }

    private static void PulseNode(
        RectTransform node,
        Vector3 baseScale,
        bool active,
        float pulse)
    {
        if (node == null) return;
        float scale = active ? 1f + 0.022f * pulse : 1f;
        node.localScale = baseScale * scale;
    }

    private Color GetAccent(TriangleCircuitType circuit)
    {
        if (circuit == TriangleCircuitType.Energy) return energyColor;
        if (circuit == TriangleCircuitType.Experimental) return experimentalColor;
        if (circuit == TriangleCircuitType.Phase) return phaseColor;
        return inactiveBorderColor;
    }

    private void ApplyTextHierarchy(Color accent)
    {
        if (synchronizationText != null)
        {
            string raw = StripRichText(synchronizationText.text);
            int separator = raw.LastIndexOf(':');
            if (separator >= 0)
            {
                string label = raw.Substring(0, separator + 1);
                string value = raw.Substring(separator + 1).Trim();
                synchronizationText.SetText(
                    $"<color=#EDF5FF>{label}</color> <color=#{ColorHex(accent)}>{value}</color>");
            }
        }

        if (effectText != null)
        {
            string raw = StripRichText(effectText.text);
            int separator = raw.IndexOf(':');
            string styled = separator >= 0
                ? $"<color=#DCE8F2>{raw.Substring(0, separator + 1)}</color>{raw.Substring(separator + 1)}"
                : raw;
            styled = Regex.Replace(styled,
                @"([+-][0-9.,]+%?\s*LE)", "<color=#00D5FF>$1</color>",
                RegexOptions.IgnoreCase);
            styled = Regex.Replace(styled,
                @"([+-][0-9.,]+%?\s*Trazas)", "<color=#D55CFF>$1</color>",
                RegexOptions.IgnoreCase);
            styled = Regex.Replace(styled,
                @"([+-][0-9.,]+%?\s*Energ[ií]a)", "<color=#FF9A21>$1</color>",
                RegexOptions.IgnoreCase);
            effectText.SetText(styled);
        }
    }

    private static string StripRichText(string value)
    {
        return Regex.Replace(value ?? string.Empty, "<[^>]+>", string.Empty);
    }

    private static string ColorHex(Color color)
    {
        return ColorUtility.ToHtmlStringRGB(color);
    }

    private static string GetCircuitName(TriangleCircuitType circuit)
    {
        LocalizationManager localization = LocalizationManager.I;
        if (circuit == TriangleCircuitType.Energy)
            return Localize(localization, "triangle.circuit.energy", "LE");
        if (circuit == TriangleCircuitType.Experimental)
            return Localize(localization, "triangle.circuit.experimental", "TRAZAS");
        if (circuit == TriangleCircuitType.Phase)
            return Localize(localization, "triangle.circuit.phase", "ENERGÍA");
        return "TRIÁNGULO";
    }

    private static string Localize(
        LocalizationManager localization,
        string key,
        string fallback)
    {
        if (localization == null) return fallback;
        string value = localization.T(key);
        if (string.IsNullOrEmpty(value) || value == key) return fallback;
        return value.ToUpperInvariant();
    }
}

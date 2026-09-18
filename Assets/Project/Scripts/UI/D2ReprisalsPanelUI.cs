using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D2ReprisalsPanelUI : MonoBehaviour
{
    public D2Civilization2PanelUI civilization2PanelUI;

    [Header("Cabecera V4")]
    public TMP_Text availableMembersText;
    public TMP_Text fragmentsText;
    public TMP_Text reprisalsCountText;
    public TMP_Text regionNameText;
    public Button regionSelectorButton;

    [Header("Medidores V4")]
    public TMP_Text threatText;
    public Slider threatSlider;
    public D2SegmentedGaugeGraphic threatGauge;
    public TMP_Text coverageText;
    public Slider coverageSlider;
    public D2SegmentedGaugeGraphic coverageGauge;

    [Header("Estado de la próxima represalia")]
    public TMP_Text estimatedLossText;
    public TMP_Text protectionText;
    public TMP_Text weakeningText;
    public TMP_Text weakeningDurationText;
    public TMP_Text rulesText;
    public TMP_Text lastResultText;

    private void Awake()
    {
        if (regionSelectorButton != null)
            regionSelectorButton.onClick.AddListener(CycleRegion);
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        GameState gameState = GameState.I;
        if (gameState?.dimension2?.civilization2 == null)
            return;

        gameState.EnsureDimension2State();
        D2Civilization2State state = gameState.dimension2.civilization2;
        string regionId = civilization2PanelUI != null
            ? civilization2PanelUI.GetSelectedRegionId()
            : D2Civilization2System.Region1Id;
        D2RegionState region = D2Civilization2System.GetRegion(state, regionId);
        if (region == null)
            return;

        SetText(availableMembersText, FormatInteger(state.membersAvailable));
        SetText(fragmentsText, FormatInteger(state.controlFragments));
        SetText(reprisalsCountText, FormatInteger(state.totalReprisals));
        SetText(regionNameText,
            D2Civilization2System.GetRegionDisplayName(regionId).ToUpperInvariant());

        SetText(threatText, FormatPercent(region.threat));
        SetSlider(threatSlider, region.threat,
            D2Civilization2System.ReprisalThreatThreshold);
        SetGauge(threatGauge, region.threat,
            D2Civilization2System.ReprisalThreatThreshold);

        SetText(coverageText,
            region.coverage.ToString("0.##", CultureInfo.InvariantCulture) + " / " +
            D2Civilization2System.MaxCoverage.ToString("0", CultureInfo.InvariantCulture));
        SetSlider(coverageSlider, region.coverage, D2Civilization2System.MaxCoverage);
        SetGauge(coverageGauge, region.coverage, D2Civilization2System.MaxCoverage);

        double expectedLoss = D2Civilization2System.GetExpectedReprisalLossFraction(
            state, region);
        SetText(estimatedLossText, FormatPercent(expectedLoss * 100.0));

        double espionageReduction =
            D2Civilization2System.GetPreparedEspionageReprisalReduction(state, region);
        SetText(protectionText, espionageReduction > 0.0
            ? "LISTO (-" + FormatPercent(espionageReduction * 100.0) + ")"
            : "NO PREPARADO");

        bool weakened = !string.IsNullOrEmpty(region.weakenedOperationId) &&
            region.weakenedOperationRemainingSeconds > 0.0;
        if (weakened)
        {
            SetText(weakeningText,
                D2Civilization2System.GetOperationDisplayName(
                    region.weakenedOperationId).ToUpperInvariant() + " AL " +
                FormatPercent(D2Civilization2System.WeakenedOperationMultiplier * 100.0));
            SetText(weakeningDurationText,
                FormatDuration(region.weakenedOperationRemainingSeconds));
        }
        else
        {
            SetText(weakeningText, "NINGUNA OPERACIÓN");
            SetText(weakeningDurationText, "—");
        }

        long fragmentsReward = state.alertActive
            ? D2Civilization2System.AlertControlFragmentsPerReprisal
            : D2Civilization2System.ControlFragmentsPerReprisal;
        SetText(rulesText,
            "AL LLEGAR A " +
            FormatPercent(D2Civilization2System.ReprisalThreatThreshold) +
            " OCURRE UNA REPRESALIA\n" +
            "LA AMENAZA VUELVE A " +
            FormatPercent(D2Civilization2System.ThreatAfterReprisal) + "\n" +
            BuildCoverageRetentionRule() + "\n" +
            "RECOMPENSA " + FormatInteger(fragmentsReward) + " FRAGMENTOS");

        SetText(lastResultText, region.hasLastReprisalResult
            ? FormatInteger(region.lastReprisalMemberLosses) + " MIEMBROS PERDIDOS"
            : "SIN REGISTRO");
    }

    private void CycleRegion()
    {
        if (civilization2PanelUI != null)
            civilization2PanelUI.CycleSelectedRegion(1);
    }

    private static string BuildCoverageRetentionRule()
    {
        if (Math.Abs(D2Civilization2System.CoverageRetentionAfterReprisal - 0.5) <
            0.000001)
            return "LA COBERTURA CONSERVA LA MITAD";
        return "LA COBERTURA CONSERVA EL " +
            FormatPercent(
                D2Civilization2System.CoverageRetentionAfterReprisal * 100.0);
    }

    private static string FormatDuration(double seconds)
    {
        int totalSeconds = Mathf.Max(0, Mathf.CeilToInt((float)seconds));
        return (totalSeconds / 60).ToString("00", CultureInfo.InvariantCulture) + ":" +
            (totalSeconds % 60).ToString("00", CultureInfo.InvariantCulture);
    }

    private static string FormatInteger(long value)
    {
        return value.ToString("N0", CultureInfo.InvariantCulture);
    }

    private static string FormatPercent(double value)
    {
        return value.ToString("0.##", CultureInfo.InvariantCulture) + "%";
    }

    private static void SetSlider(Slider slider, double value, double maximum)
    {
        if (slider == null)
            return;
        slider.minValue = 0f;
        slider.maxValue = (float)maximum;
        slider.value = (float)Math.Clamp(value, 0.0, maximum);
    }

    private static void SetGauge(
        D2SegmentedGaugeGraphic gauge,
        double value,
        double maximum)
    {
        if (gauge == null)
            return;
        gauge.SetProgress(maximum <= 0.0
            ? 0f
            : (float)Math.Clamp(value / maximum, 0.0, 1.0));
    }

    private static void SetText(TMP_Text text, string value)
    {
        if (text != null)
            text.text = value;
    }
}

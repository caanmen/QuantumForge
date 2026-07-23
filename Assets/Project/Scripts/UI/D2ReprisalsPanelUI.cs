using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D2ReprisalsPanelUI : MonoBehaviour
{
    public D2Civilization2PanelUI civilization2PanelUI;
    public TMP_Text threatText;
    public Slider threatSlider;
    public TMP_Text coverageText;
    public Slider coverageSlider;
    public TMP_Text protectionText;
    public TMP_Text fragmentsText;
    public TMP_Text weakeningText;
    public TMP_Text rulesText;
    public TMP_Text lastResultText;

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

        SetText(
            threatText,
            D2Civilization2System.GetRegionDisplayName(regionId).ToUpperInvariant() +
            " — AMENAZA: " + region.threat.ToString("0.##") + "%"
        );
        SetSlider(threatSlider, region.threat, 100.0);
        SetText(
            coverageText,
            "COBERTURA: " + region.coverage.ToString("0.##") + " / " +
            D2Civilization2System.MaxCoverage.ToString("0")
        );
        SetSlider(coverageSlider, region.coverage, D2Civilization2System.MaxCoverage);

        bool espionagePrepared = region.nextReprisalEspionageReduction > 0.0 ||
            D2Civilization2System.IsOperationActive(
                D2Civilization2System.GetOperation(
                    region,
                    D2Civilization2System.EspionageOperationId
                )
            );
        SetText(
            protectionText,
            "Pérdida estimada en próxima Represalia: " +
            (D2Civilization2System.GetExpectedReprisalLossFraction(state, region) * 100.0)
                .ToString("0.##") + "%\n" +
            "Preparación de Espionaje: " + (espionagePrepared ? "LISTA (-5%)" : "NO PREPARADA")
        );
        SetText(
            fragmentsText,
            "FRAGMENTOS DE CONTROL: " + state.controlFragments.ToString("N0") +
            " | Represalias resistidas: " + state.totalReprisals.ToString("N0")
        );

        string weakening = "Ninguna operación debilitada.";
        if (!string.IsNullOrEmpty(region.weakenedOperationId) &&
            region.weakenedOperationRemainingSeconds > 0.0)
        {
            weakening = D2Civilization2System.GetOperationDisplayName(
                region.weakenedOperationId
            ) + " funciona al 50% durante " +
                FormatDuration(region.weakenedOperationRemainingSeconds) + ".";
        }
        SetText(weakeningText, weakening);
        SetText(
            rulesText,
            "Al llegar a 100% de Amenaza ocurre una Represalia. La Amenaza vuelve " +
            "a 25%, la Cobertura conserva la mitad y se obtienen " +
            (state.alertActive ? "6" : "3") + " Fragmentos." +
            (region.alertMarked ? " La marca añade 3% de pérdidas y se consumirá." : "")
        );
        SetText(
            lastResultText,
            string.IsNullOrEmpty(state.lastResult)
                ? "Aún no se ha producido ninguna Represalia."
                : state.lastResult
        );
    }

    private static string FormatDuration(double seconds)
    {
        int totalSeconds = Mathf.Max(0, Mathf.CeilToInt((float)seconds));
        return (totalSeconds / 60).ToString("00") + ":" +
            (totalSeconds % 60).ToString("00");
    }

    private static void SetSlider(Slider slider, double value, double maximum)
    {
        if (slider == null)
            return;
        slider.minValue = 0f;
        slider.maxValue = (float)maximum;
        slider.value = (float)value;
    }

    private static void SetText(TMP_Text text, string value)
    {
        if (text != null)
            text.text = value;
    }
}

using TMPro;
using UnityEngine;


public class D2AlertPanelUI : MonoBehaviour
{
    public TMP_Text stateText;
    public TMP_Text dominanceText;
    public TMP_Text timerText;
    public TMP_Text effectsText;
    public TMP_Text regionsText;
    public TMP_Text unlocksText;
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
        double dominance = D2Civilization2System.GetTotalDominance(state);
        SetText(
            stateText,
            state.entityContained
                ? "ALERTA FINALIZADA — ENTE CONTENIDO"
                : state.alertActive
                ? "FASE DE ALERTA — ACTIVA"
                : "FASE DE ALERTA — INACTIVA"
        );
        SetText(
            dominanceText,
            "Dominio total: " + dominance.ToString("0.##") +
            "% | Se activa permanentemente al llegar a 30%."
        );
        SetText(
            timerText,
            state.entityContained
                ? "La Contención detuvo las nuevas marcas del Ente."
                : state.alertActive
                ? "Próxima elección de región: " +
                    FormatDuration(D2Civilization2System.GetTimeUntilNextAlertMark(state)) +
                    " | Elecciones realizadas: " + state.totalAlertMarks.ToString("N0")
                : "El Ente todavía no está marcando regiones."
        );
        SetText(
            effectsText,
            state.entityContained
                ? "El aumento de Amenaza de Alerta ha terminado. Las Represalias conservan 6 Fragmentos."
                : state.alertActive
                ? "Rescate, Espionaje y Sabotaje generan +50% Amenaza. " +
                    "Cada Represalia entrega 6 Fragmentos."
                : "Antes de Alerta se mantienen la Amenaza normal y 3 Fragmentos por Represalia."
        );
        SetText(regionsText, BuildRegionStatus(state));
        SetText(
            unlocksText,
            state.entityContained
                ? "Civilización 3: DESBLOQUEADA | Pacto mayor: PREPARADO"
                : state.alertActive
                ? "Civilización 3: DESBLOQUEADA | Contención: PREPARADA PARA 3G"
                : "Civilización 3 y Contención permanecen bloqueadas."
        );
        SetText(
            lastResultText,
            string.IsNullOrEmpty(state.lastAlertResult)
                ? "No se han producido ataques de Alerta."
                : state.lastAlertResult
        );
    }

    private static string BuildRegionStatus(D2Civilization2State state)
    {
        string result = "ESTADO DE LAS REGIONES\n";
        bool first = true;
        foreach (string regionId in D2Civilization2System.RegionIds)
        {
            D2RegionState region = D2Civilization2System.GetRegion(state, regionId);
            if (region == null || !region.unlocked || regionId == D2Civilization2System.Region4Id)
                continue;

            if (!first)
                result += "\n";
            first = false;
            result += D2Civilization2System.GetRegionDisplayName(regionId) + ": " +
                (region.alertMarked ? "MARCADA (+3% próxima Represalia)" : "sin marca") +
                " | Protección: " +
                (D2Civilization2System.IsProtectionActive(region) ? "ACTIVA" : "inactiva");
        }
        return result;
    }

    private static string FormatDuration(double seconds)
    {
        int totalSeconds = Mathf.Max(0, Mathf.CeilToInt((float)seconds));
        return (totalSeconds / 60).ToString("00") + ":" +
            (totalSeconds % 60).ToString("00");
    }

    private static void SetText(TMP_Text target, string value)
    {
        if (target != null)
            target.text = value;
    }
}

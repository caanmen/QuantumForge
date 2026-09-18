using TMPro;
using UnityEngine;


public class D2AlertPanelUI : MonoBehaviour
{
    public TMP_Text stateText;
    public TMP_Text dominanceText;
    public TMP_Text timerText;
    public TMP_Text marksCountText;
    public TMP_Text effectsText;
    public TMP_Text fragmentsEffectText;
    public TMP_Text regionsText;
    public TMP_Text[] regionNameTexts;
    public TMP_Text[] regionMarkTexts;
    public TMP_Text[] regionProtectionTexts;
    public GameObject[] regionProtectionActiveIcons;
    public GameObject[] regionProtectionInactiveIcons;
    public TMP_Text unlocksText;
    public TMP_Text unlockDetailText;
    public TMP_Text lastResultText;
    public TMP_Text lastResultDetailText;
    public D2SegmentedGaugeGraphic dominanceGauge;

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
                ? "ALERTA FINALIZADA · ENTE CONTENIDO"
                : state.alertActive
                ? "EL ENTE HA ENTRADO EN ALERTA"
                : "FASE DE ALERTA · INACTIVA"
        );
        SetText(dominanceText, dominance.ToString("0.##") + "%");
        if (dominanceGauge != null)
            dominanceGauge.SetProgress((float)(dominance / 100.0));
        SetText(timerText, state.entityContained
            ? "—:—"
            : FormatDuration(D2Civilization2System.GetTimeUntilNextAlertMark(state)));
        SetText(marksCountText, state.totalAlertMarks.ToString("N0"));
        SetText(
            effectsText,
            state.entityContained
                ? "AUMENTO DE AMENAZA\nFINALIZADO"
                : state.alertActive
                ? "RESCATE, ESPIONAJE Y\nSABOTAJE GENERAN\n+50% AMENAZA"
                : "AMENAZA NORMAL\nANTES DE ALERTA"
        );
        SetText(fragmentsEffectText, state.alertActive || state.entityContained
            ? "CADA REPRESALIA\nENTREGA 6 FRAGMENTOS"
            : "CADA REPRESALIA\nENTREGA 3 FRAGMENTOS");
        RefreshRegions(state);
        SetText(
            unlocksText,
            state.entityContained
                ? "CIVILIZACIÓN 3 · DESBLOQUEADA"
                : state.alertActive
                ? "CIVILIZACIÓN 3 · NUEVA"
                : "CIVILIZACIÓN 3 · BLOQUEADA"
        );
        SetText(unlockDetailText, state.alertActive || state.entityContained
            ? "CIVILIZACIÓN 3 Y CONTENCIÓN\nDESBLOQUEADAS AL LLEGAR A 30% DE DOMINIO."
            : "REDUCE EL DOMINIO TOTAL A 30%\nPARA DESBLOQUEAR CIVILIZACIÓN 3 Y CONTENCIÓN.");
        RefreshLastResult(state);
    }

    private void RefreshRegions(D2Civilization2State state)
    {
        string[] ids =
        {
            D2Civilization2System.Region1Id,
            D2Civilization2System.Region2Id,
            D2Civilization2System.Region3Id
        };
        for (int index = 0; index < ids.Length; index++)
        {
            D2RegionState region = D2Civilization2System.GetRegion(state, ids[index]);
            bool unlocked = region != null && region.unlocked;
            bool marked = unlocked && region.alertMarked;
            bool protectedRegion = unlocked && D2Civilization2System.IsProtectionActive(region);
            SetArrayText(regionNameTexts, index,
                D2Civilization2System.GetRegionDisplayName(ids[index]).ToUpperInvariant());
            SetArrayText(regionMarkTexts, index,
                !unlocked ? "BLOQUEADA" : marked
                    ? "MARCADA\n<size=68%>(+3% PRÓXIMA REPRESALIA)</size>"
                    : "SIN MARCA");
            SetArrayColor(regionMarkTexts, index,
                marked ? new Color32(229, 83, 57, 255) : new Color32(165, 141, 114, 255));
            SetArrayText(regionProtectionTexts, index,
                "PROTECCIÓN\n" + (protectedRegion ? "ACTIVA" : "INACTIVA"));
            SetArrayColor(regionProtectionTexts, index,
                protectedRegion
                    ? new Color32(99, 179, 166, 255)
                    : new Color32(165, 141, 114, 255));
            SetArrayActive(regionProtectionActiveIcons, index, protectedRegion);
            SetArrayActive(regionProtectionInactiveIcons, index, !protectedRegion);
        }
        SetText(regionsText, "ESTADO DE LAS REGIONES");
    }

    private void RefreshLastResult(D2Civilization2State state)
    {
        string value = state.lastAlertResult ?? "";
        if (state.entityContained)
        {
            SetText(lastResultText, "EL ENTE FUE CONTENIDO");
            SetText(lastResultDetailText, "NO SE PRODUCIRÁN NUEVAS MARCAS");
            return;
        }
        int markedIndex = value.IndexOf(" fue marcada:", System.StringComparison.OrdinalIgnoreCase);
        if (markedIndex >= 0)
        {
            SetText(lastResultText,
                value.Substring(0, markedIndex).ToUpperInvariant() + " FUE MARCADA");
            SetText(lastResultDetailText,
                "LA PRÓXIMA REPRESALIA AÑADE 3% DE PÉRDIDAS");
            return;
        }
        int protectedIndex = value.IndexOf(
            " fue elegida por el Ente", System.StringComparison.OrdinalIgnoreCase);
        if (protectedIndex >= 0)
        {
            SetText(lastResultText,
                value.Substring(0, protectedIndex).ToUpperInvariant() + " FUE PROTEGIDA");
            SetText(lastResultDetailText, "PROTECCIÓN MITIGÓ LA MARCA DEL ENTE");
            return;
        }
        if (string.IsNullOrEmpty(value))
        {
            SetText(lastResultText, "SIN RESULTADOS RECIENTES");
            SetText(lastResultDetailText, "EL ENTE TODAVÍA NO HA ELEGIDO UNA REGIÓN");
            return;
        }
        SetText(lastResultText, "EL ENTE HA ENTRADO EN ALERTA");
        SetText(lastResultDetailText, value.ToUpperInvariant());
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

    private static void SetArrayText(TMP_Text[] targets, int index, string value)
    {
        if (targets != null && index >= 0 && index < targets.Length)
            SetText(targets[index], value);
    }

    private static void SetArrayActive(GameObject[] targets, int index, bool active)
    {
        if (targets != null && index >= 0 && index < targets.Length && targets[index] != null)
            targets[index].SetActive(active);
    }

    private static void SetArrayColor(TMP_Text[] targets, int index, Color color)
    {
        if (targets != null && index >= 0 && index < targets.Length && targets[index] != null)
            targets[index].color = color;
    }
}

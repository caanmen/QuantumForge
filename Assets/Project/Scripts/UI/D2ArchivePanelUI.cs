using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D2ArchivePanelUI : MonoBehaviour
{
    public D2Civilization3PanelUI civilization3PanelUI;
    public TMP_Text stateText;
    public TMP_Text resourcesText;
    public TMP_Text cartographyText;
    public TMP_Text concordanceText;
    public TMP_Text exegesisText;
    public TMP_Text lastResultText;
    public Button cartographyButton;
    public Button concordanceButton;
    public Button exegesisButton;
    public Button backToArchaeologyButton;
    public Button backToMapButton;

    private void Awake()
    {
        if (cartographyButton != null)
            cartographyButton.onClick.AddListener(() => Unlock(
                D2Civilization3System.StratifiedCartographyUpgradeId));
        if (concordanceButton != null)
            concordanceButton.onClick.AddListener(() => Unlock(
                D2Civilization3System.AnomalousConcordanceUpgradeId));
        if (exegesisButton != null)
            exegesisButton.onClick.AddListener(() => Unlock(
                D2Civilization3System.DeepExegesisUpgradeId));
        if (backToArchaeologyButton != null)
            backToArchaeologyButton.onClick.AddListener(BackToArchaeology);
        if (backToMapButton != null)
            backToMapButton.onClick.AddListener(BackToMap);
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        GameState gameState = GameState.I;
        if (gameState?.dimension2?.civilization3 == null)
            return;
        gameState.EnsureDimension2State();
        D2Civilization3State state = gameState.dimension2.civilization3;
        D2C3ZoneState zone1 = D2Civilization3System.GetZone(
            state, D2Civilization3System.Zone1Id);
        D2C3ZoneState zone2 = D2Civilization3System.GetZone(
            state, D2Civilization3System.Zone2Id);
        D2C3ZoneState zone3 = D2Civilization3System.GetZone(
            state, D2Civilization3System.Zone3Id);

        SetText(stateText, state.archiveUnlocked
            ? "ARCHIVO " + ToRoman(state.archiveLevel) + " — MEJORAS PERMANENTES"
            : "ARCHIVO DE INTERPRETACIÓN — BLOQUEADO");
        SetText(resourcesText,
            "Conocimiento Antiguo: " + state.ancientKnowledge.ToString("0.##") +
            " | Conocimiento del Ente: " + state.entityKnowledge.ToString("N0") + "/6\n" +
            "Fragmentos: " + zone1.zoneResourceAmount.ToString("N0") +
            " | Inscripciones: " + zone2.zoneResourceAmount.ToString("N0") +
            " | Sellos: " + zone3.zoneResourceAmount.ToString("N0"));

        RefreshUpgrade(gameState, state,
            D2Civilization3System.StratifiedCartographyUpgradeId,
            cartographyText, cartographyButton);
        RefreshUpgrade(gameState, state,
            D2Civilization3System.AnomalousConcordanceUpgradeId,
            concordanceText, concordanceButton);
        RefreshUpgrade(gameState, state,
            D2Civilization3System.DeepExegesisUpgradeId,
            exegesisText, exegesisButton);
        SetText(lastResultText, string.IsNullOrEmpty(state.lastResult)
            ? "El Archivo aguarda nuevos hallazgos."
            : state.lastResult);
    }

    private static void RefreshUpgrade(
        GameState gameState,
        D2Civilization3State state,
        string upgradeId,
        TMP_Text text,
        Button button
    )
    {
        bool unlocked = D2Civilization3System.IsArchiveUpgradeUnlocked(state, upgradeId);
        string zoneId = D2Civilization3System.GetArchiveUpgradeZoneId(upgradeId);
        SetText(text,
            D2Civilization3System.GetArchiveUpgradeName(upgradeId).ToUpperInvariant() +
            (unlocked ? " — DESBLOQUEADA" : " — BLOQUEADA") + "\n" +
            D2Civilization3System.GetArchiveUpgradeDescription(upgradeId) + "\n" +
            "Requiere Archivo " +
            ToRoman(D2Civilization3System.GetArchiveUpgradeRequiredLevel(upgradeId)) +
            " | " + D2Civilization3System.GetArchiveUpgradeKnowledgeCost(upgradeId).ToString("0") +
            " Conocimiento + " +
            D2Civilization3System.GetArchiveUpgradeResourceCost(upgradeId).ToString("N0") +
            " " + D2Civilization3System.GetZoneResourceName(zoneId) +
            " | Umbral: " +
            D2Civilization3System.GetArchiveUpgradeEntityKnowledgeRequirement(upgradeId));
        if (button != null)
        {
            button.gameObject.SetActive(!unlocked);
            button.interactable = D2Civilization3System.CanUnlockArchiveUpgrade(
                gameState, upgradeId);
        }
    }

    private void Unlock(string upgradeId)
    {
        D2Civilization3System.TryUnlockArchiveUpgrade(GameState.I, upgradeId);
        Refresh();
    }

    private void BackToArchaeology()
    {
        if (civilization3PanelUI != null)
            civilization3PanelUI.ShowArchaeology();
    }

    private void BackToMap()
    {
        if (civilization3PanelUI != null)
            civilization3PanelUI.BackToMapFromChild();
    }

    private static string ToRoman(int level)
    {
        if (level >= 4) return "IV";
        if (level == 3) return "III";
        if (level == 2) return "II";
        return "I";
    }

    private static void SetText(TMP_Text target, string value)
    {
        if (target != null)
            target.text = value;
    }
}

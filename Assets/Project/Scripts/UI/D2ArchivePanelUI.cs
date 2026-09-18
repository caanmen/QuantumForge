using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D2ArchivePanelUI : MonoBehaviour
{
    public D2Civilization3PanelUI civilization3PanelUI;

    // Compatibility references retained for validation and existing scene bindings.
    public TMP_Text stateText;
    public TMP_Text resourcesText;
    public TMP_Text cartographyText;
    public TMP_Text concordanceText;
    public TMP_Text exegesisText;
    public TMP_Text lastResultText;

    public TMP_Text levelText;
    public TMP_Text ancientKnowledgeValueText;
    public TMP_Text entityKnowledgeValueText;
    public TMP_Text fragmentsValueText;
    public TMP_Text inscriptionsValueText;
    public TMP_Text sealsValueText;
    public TMP_Text[] actionTexts;
    public TMP_Text[] upgradeStateTexts;
    public Image[] actionHighlightImages;
    public GameObject[] lockIconRoots;

    public Button cartographyButton;
    public Button concordanceButton;
    public Button exegesisButton;
    public Button backToArchaeologyButton;
    public Button excavateNavigationButton;
    public Button analyzeNavigationButton;
    public Button entityNavigationButton;

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
            backToArchaeologyButton.onClick.AddListener(ShowArchaeology);
        if (excavateNavigationButton != null)
            excavateNavigationButton.onClick.AddListener(ShowArchaeology);
        if (analyzeNavigationButton != null)
            analyzeNavigationButton.onClick.AddListener(ShowAnalysis);
        if (entityNavigationButton != null)
            entityNavigationButton.onClick.AddListener(ShowEntityResearch);
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

        string romanLevel = ToRoman(state.archiveLevel);
        SetText(stateText, state.archiveUnlocked
            ? "ARCHIVO " + romanLevel + " — MEJORAS PERMANENTES"
            : "ARCHIVO DE INTERPRETACIÓN — BLOQUEADO");
        SetText(levelText, state.archiveUnlocked ? romanLevel : "—");
        SetText(ancientKnowledgeValueText, state.ancientKnowledge.ToString("0.##"));
        SetText(entityKnowledgeValueText, state.entityKnowledge.ToString("N0") + " / 6");
        SetText(fragmentsValueText, zone1?.zoneResourceAmount.ToString("N0") ?? "0");
        SetText(inscriptionsValueText, zone2?.zoneResourceAmount.ToString("N0") ?? "0");
        SetText(sealsValueText, zone3?.zoneResourceAmount.ToString("N0") ?? "0");

        RefreshUpgrade(gameState, state, 0,
            D2Civilization3System.StratifiedCartographyUpgradeId,
            cartographyButton);
        RefreshUpgrade(gameState, state, 1,
            D2Civilization3System.AnomalousConcordanceUpgradeId,
            concordanceButton);
        RefreshUpgrade(gameState, state, 2,
            D2Civilization3System.DeepExegesisUpgradeId,
            exegesisButton);

        SetText(lastResultText, string.IsNullOrEmpty(state.lastResult)
            ? "EL ARCHIVO AGUARDA NUEVOS HALLAZGOS"
            : state.lastResult.ToUpperInvariant());
    }

    private void RefreshUpgrade(
        GameState gameState,
        D2Civilization3State state,
        int index,
        string upgradeId,
        Button button)
    {
        // Resolve existing scene labels as well as freshly rebuilt UI.
        Transform threshold = transform.Find("ArchiveEntityThreshold" + index);
        if (threshold != null)
            RefreshEntityThreshold(threshold.GetComponent<TMP_Text>(), upgradeId);

        bool unlocked = D2Civilization3System.IsArchiveUpgradeUnlocked(state, upgradeId);
        bool available = !unlocked &&
            D2Civilization3System.CanUnlockArchiveUpgrade(gameState, upgradeId);

        TMP_Text actionText = Get(actionTexts, index);
        TMP_Text upgradeStateText = Get(upgradeStateTexts, index);
        SetText(actionText, unlocked ? "ADQUIRIDA" : available ? "DESBLOQUEAR" : string.Empty);
        SetText(upgradeStateText,
            unlocked ? "DESBLOQUEADA" : available ? "DISPONIBLE" : "BLOQUEADA");
        if (actionText != null)
            actionText.color = unlocked ? Hex("6E6861") : Hex("D8C09A");
        if (upgradeStateText != null)
            upgradeStateText.color = unlocked || available
                ? Hex("65C5C5") : Hex("B286C1");

        Image highlight = Get(actionHighlightImages, index);
        if (highlight != null)
            highlight.gameObject.SetActive(available);
        GameObject lockRoot = Get(lockIconRoots, index);
        if (lockRoot != null)
            lockRoot.SetActive(!unlocked && !available);

        if (button != null)
        {
            button.gameObject.SetActive(true);
            button.interactable = available;
        }
    }

    public static void RefreshEntityThreshold(TMP_Text label, string upgradeId)
    {
        if (label == null) return;
        double requirement = D2Civilization3System.GetArchiveUpgradeEntityKnowledgeRequirement(upgradeId);
        label.text = requirement > 0 ? "UMBRAL DEL ENTE: " + requirement.ToString("0") : string.Empty;
        label.gameObject.SetActive(requirement > 0);
    }

    private void Unlock(string upgradeId)
    {
        D2Civilization3System.TryUnlockArchiveUpgrade(GameState.I, upgradeId);
        Refresh();
    }

    private void ShowArchaeology()
    {
        if (civilization3PanelUI != null)
            civilization3PanelUI.ShowArchaeology();
    }

    private void ShowAnalysis()
    {
        if (civilization3PanelUI != null)
            civilization3PanelUI.ShowAnalysis();
    }

    private void ShowEntityResearch()
    {
        if (civilization3PanelUI != null)
            civilization3PanelUI.ShowEntityResearch();
    }

    private static T Get<T>(T[] items, int index) where T : class
    {
        return items != null && index >= 0 && index < items.Length ? items[index] : null;
    }

    private static string ToRoman(int level)
    {
        if (level >= 4) return "IV";
        if (level == 3) return "III";
        if (level == 2) return "II";
        return "I";
    }

    private static Color Hex(string rgb)
    {
        ColorUtility.TryParseHtmlString("#" + rgb, out Color color);
        return color;
    }

    private static void SetText(TMP_Text target, string value)
    {
        if (target != null)
            target.text = value;
    }
}

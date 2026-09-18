using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D2EntityResearchPanelUI : MonoBehaviour
{
    private float _refreshTimer;
    private string _selectedPactLineId = D2Civilization3System.ResonantExpeditionLineId;

    public D2Civilization3PanelUI civilization3PanelUI;

    [Header("Compatibilidad y fase de pacto")]
    public GameObject researchPhaseVisualRoot;
    public GameObject pactPhaseVisualRoot;
    public TMP_Text unlockText;
    public TMP_Text statusText;
    public TMP_Text progressText;
    public Slider progressSlider;
    public TMP_Text milestoneText;
    public TMP_Text resourcesText;
    public TMP_Text entityKnowledgeText;
    public TMP_Text lastResultText;
    public Button resonantExpeditionButton;
    public Button endlessArchiveButton;
    public Button sharedMemoryButton;
    public Button modulatorResonanceButton;
    public Button firstThresholdChronicleButton;
    public Button backToMapButton;

    [Header("Investigación del Ente — composición V5")]
    public TMP_Text unlockDetailText;
    public TMP_Text progressGaugeText;
    public TMP_Text ancientKnowledgeValueText;
    public D2SegmentedGaugeGraphic progressRadialGraphic;
    public TMP_Text milestoneGaugeText;
    public TMP_Text milestoneCostText;
    public TMP_Text milestoneRewardText;
    public GameObject[] milestoneSelectionRoots;
    public TMP_Text[] milestoneNodeTexts;
    public TMP_Text[] milestoneCardTitleTexts;
    public TMP_Text fragmentsValueText;
    public TMP_Text inscriptionsValueText;
    public TMP_Text sealsValueText;
    public TMP_Text basicDataValueText;
    public TMP_Text symbolicDataValueText;
    public TMP_Text deepDataValueText;
    public TMP_Text entityKnowledgeValueText;
    public GameObject[] entityKnowledgeFillRoots;
    public TMP_Text objectiveText;
    public TMP_Text startPauseActionText;
    public TMP_Text milestoneActionText;
    public Image startPauseHighlightImage;
    public Image milestoneHighlightImage;

    [Header("Pacto con el Ente — composición V5")]
    public TMP_Text pactStatusText;
    public TMP_Text pactProgressText;
    public TMP_Text pactAncientKnowledgeText;
    public TMP_Text[] pactCardTitleTexts;
    public TMP_Text[] pactLevelTexts;
    public GameObject[] pactSelectionRoots;
    public GameObject[] pactDetailIconRoots;
    public TMP_Text pactDetailTitleText;
    public TMP_Text pactDetailEffectText;
    public TMP_Text pactDetailCostText;
    public TMP_Text pactThresholdText;
    public TMP_Text pactFragmentsValueText;
    public TMP_Text pactInscriptionsValueText;
    public TMP_Text pactSealsValueText;
    public TMP_Text pactEntityKnowledgeValueText;
    public TMP_Text pactEstablishActionText;
    public TMP_Text pactUpgradeActionText;
    public TMP_Text pactLastResultText;
    public TMP_Text pactNavigationLabelText;
    public Image pactEstablishHighlightImage;
    public Image pactUpgradeHighlightImage;
    public GameObject pactNavigationSelectionRoot;

    [Header("Acciones y navegación")]
    public Button startPauseButton;
    public Button completeMilestoneButton;
    public Button backToArchaeologyButton;
    public Button excavateNavigationButton;
    public Button analyzeNavigationButton;
    public Button archiveNavigationButton;
    public Button pactEstablishButton;
    public Button pactUpgradeButton;
    public Button pactBackToArchaeologyButton;
    public Button pactExcavateNavigationButton;
    public Button pactAnalyzeNavigationButton;
    public Button pactArchiveNavigationButton;

    private void Awake()
    {
        if (startPauseButton != null)
            startPauseButton.onClick.AddListener(StartOrPause);
        if (completeMilestoneButton != null)
            completeMilestoneButton.onClick.AddListener(CompleteMilestone);
        if (resonantExpeditionButton != null)
            resonantExpeditionButton.onClick.AddListener(() => SelectPactLine(D2Civilization3System.ResonantExpeditionLineId));
        if (endlessArchiveButton != null)
            endlessArchiveButton.onClick.AddListener(() => SelectPactLine(D2Civilization3System.EndlessArchiveLineId));
        if (sharedMemoryButton != null)
            sharedMemoryButton.onClick.AddListener(() => SelectPactLine(D2Civilization3System.SharedMemoryLineId));
        if (modulatorResonanceButton != null)
            modulatorResonanceButton.onClick.AddListener(() => SelectPactLine(D2Civilization3System.ModulatorResonanceLineId));
        if (firstThresholdChronicleButton != null)
            firstThresholdChronicleButton.onClick.AddListener(() => SelectPactLine(D2Civilization3System.FirstThresholdChronicleLineId));
        if (backToArchaeologyButton != null)
            backToArchaeologyButton.onClick.AddListener(ShowArchaeology);
        if (excavateNavigationButton != null)
            excavateNavigationButton.onClick.AddListener(ShowArchaeology);
        if (analyzeNavigationButton != null)
            analyzeNavigationButton.onClick.AddListener(ShowAnalysis);
        if (archiveNavigationButton != null)
            archiveNavigationButton.onClick.AddListener(ShowArchive);
        if (pactEstablishButton != null)
            pactEstablishButton.onClick.AddListener(EstablishPact);
        if (pactUpgradeButton != null)
            pactUpgradeButton.onClick.AddListener(UpgradeSelectedPactLine);
        if (pactBackToArchaeologyButton != null)
            pactBackToArchaeologyButton.onClick.AddListener(ShowArchaeology);
        if (pactExcavateNavigationButton != null)
            pactExcavateNavigationButton.onClick.AddListener(ShowArchaeology);
        if (pactAnalyzeNavigationButton != null)
            pactAnalyzeNavigationButton.onClick.AddListener(ShowAnalysis);
        if (pactArchiveNavigationButton != null)
            pactArchiveNavigationButton.onClick.AddListener(ShowArchive);
        if (backToMapButton != null)
            backToMapButton.onClick.AddListener(BackToMap);
    }

    private void OnEnable()
    {
        _refreshTimer = 0f;
        Refresh();
    }

    private void Update()
    {
        _refreshTimer -= Time.unscaledDeltaTime;
        if (_refreshTimer > 0f)
            return;
        _refreshTimer = 0.2f;
        Refresh();
    }

    public void Refresh()
    {
        GameState gameState = GameState.I;
        if (gameState?.dimension2?.civilization3 == null)
            return;
        gameState.EnsureDimension2State();
        D2Civilization3State state = gameState.dimension2.civilization3;
        D2C3ZoneState zone1 = D2Civilization3System.GetZone(state, D2Civilization3System.Zone1Id);
        D2C3ZoneState zone2 = D2Civilization3System.GetZone(state, D2Civilization3System.Zone2Id);
        D2C3ZoneState zone3 = D2Civilization3System.GetZone(state, D2Civilization3System.Zone3Id);

        bool pactPhase = state.entityResearchMilestone100Completed;
        SetActive(researchPhaseVisualRoot, !pactPhase);
        SetActive(pactPhaseVisualRoot, pactPhase);
        if (pactPhase)
        {
            RefreshPactPhase(gameState, state, zone1, zone2, zone3);
            return;
        }

        SetText(unlockText, state.entityResearchUnlocked ? "INVESTIGACIÓN DESBLOQUEADA" : "INVESTIGACIÓN BLOQUEADA");
        SetText(unlockDetailText, state.entityResearchUnlocked
            ? "LOS TRES DATOS ANÓMALOS HAN TRAZADO UN PATRÓN"
            : "REQUIERE 1 DATO BÁSICO, 1 SIMBÓLICO Y 1 PROFUNDO");

        string status = state.entityResearchActive
            ? state.ancientKnowledge > 0.0 ? "EN CURSO" : "ESPERANDO"
            : "PAUSADA";
        string progressValue = state.entityResearchProgress.ToString("0.##");
        SetText(statusText, status);
        SetText(progressText, "PROGRESO " + progressValue + "%");
        SetText(progressGaugeText, progressValue + "%");
        SetText(ancientKnowledgeValueText, "CONOCIMIENTO ANTIGUO " + state.ancientKnowledge.ToString("0.##"));
        if (progressRadialGraphic != null)
            progressRadialGraphic.SetProgress((float)(state.entityResearchProgress / 100.0));
        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = 100f;
            progressSlider.value = (float)state.entityResearchProgress;
        }

        double pendingMilestone = D2Civilization3System.GetPendingEntityResearchMilestone(state);
        int pendingIndex = GetMilestoneIndex(pendingMilestone);
        SetText(milestoneText, "PRÓXIMO HITO " + pendingMilestone.ToString("0") + "%");
        SetText(milestoneGaugeText, pendingMilestone.ToString("0") + "%");
        SetText(milestoneCostText, GetMilestoneCost(pendingMilestone));
        SetText(milestoneRewardText, GetMilestoneReward(pendingMilestone));
        if (milestoneSelectionRoots != null)
        {
            for (int i = 0; i < milestoneSelectionRoots.Length; i++)
                SetActive(milestoneSelectionRoots[i], i == pendingIndex);
        }
        Color bronze = new Color32(177, 138, 92, 255);
        Color purple = new Color32(178, 134, 193, 255);
        if (milestoneNodeTexts != null)
        {
            for (int i = 0; i < milestoneNodeTexts.Length; i++)
                if (milestoneNodeTexts[i] != null)
                    milestoneNodeTexts[i].color = i == pendingIndex ? purple : bronze;
        }
        if (milestoneCardTitleTexts != null)
        {
            for (int i = 0; i < milestoneCardTitleTexts.Length; i++)
                if (milestoneCardTitleTexts[i] != null)
                    milestoneCardTitleTexts[i].color = i == pendingIndex ? purple : bronze;
        }

        SetText(fragmentsValueText, zone1.zoneResourceAmount.ToString("N0"));
        SetText(inscriptionsValueText, zone2.zoneResourceAmount.ToString("N0"));
        SetText(sealsValueText, zone3.zoneResourceAmount.ToString("N0"));
        SetText(basicDataValueText, zone1.anomalousData.ToString("N0"));
        SetText(symbolicDataValueText, zone2.anomalousData.ToString("N0"));
        SetText(deepDataValueText, zone3.anomalousData.ToString("N0"));
        SetText(resourcesText, zone1.zoneResourceAmount.ToString("N0"));
        SetText(entityKnowledgeValueText, state.entityKnowledge.ToString("N0") + " / 6");
        SetText(entityKnowledgeText, state.entityKnowledge.ToString("N0") + " / 6");
        if (entityKnowledgeFillRoots != null)
        {
            for (int i = 0; i < entityKnowledgeFillRoots.Length; i++)
                SetActive(entityKnowledgeFillRoots[i], i < state.entityKnowledge);
        }

        SetText(lastResultText, FormatResearchResult(state.lastResult));
        SetText(objectiveText, "OBJETIVO ACTUAL — REVISA EL HITO ACTUAL DEL ENTE.");

        string startLabel = state.entityResearchActive ? "PAUSAR INVESTIGACIÓN" : "INICIAR INVESTIGACIÓN";
        SetText(startPauseActionText, startLabel);
        SetButtonLabel(startPauseButton, startLabel);
        bool canStartOrPause = state.entityResearchActive || D2Civilization3System.CanStartEntityResearch(gameState);
        SetInteractable(startPauseButton, canStartOrPause);
        SetActive(startPauseHighlightImage, canStartOrPause);

        bool milestoneReached = state.entityResearchUnlocked && state.entityResearchProgress >= pendingMilestone - 0.000001;
        string milestoneLabel = pendingMilestone >= 100.0 ? "PREPARAR PACTO" : "APORTAR RECURSOS";
        SetText(milestoneActionText, milestoneLabel);
        SetButtonLabel(completeMilestoneButton, milestoneLabel);
        if (completeMilestoneButton != null)
        {
            completeMilestoneButton.gameObject.SetActive(true);
            completeMilestoneButton.interactable = milestoneReached && D2Civilization3System.CanPayEntityResearchMilestone(gameState);
        }
        if (milestoneActionText != null)
            milestoneActionText.color = completeMilestoneButton != null && completeMilestoneButton.interactable
                ? new Color32(216, 192, 154, 255)
                : new Color32(92, 86, 80, 255);
        SetActive(milestoneHighlightImage, completeMilestoneButton != null && completeMilestoneButton.interactable);
        SetPactButtonsActive(false);
    }

    private void RefreshPactPhase(GameState gameState, D2Civilization3State state,
        D2C3ZoneState zone1, D2C3ZoneState zone2, D2C3ZoneState zone3)
    {
        string pactStatus = state.entityPactEstablished
            ? "PACTO CON EL ENTE — ESTABLECIDO"
            : "PACTO PREPARADO — PENDIENTE DE ESTABLECER";
        SetText(unlockText, "INVESTIGACIÓN COMPLETADA");
        SetText(statusText, pactStatus);
        SetText(progressText, "PROGRESO 100%");
        SetText(pactStatusText, pactStatus);
        SetText(pactProgressText, "PROGRESO 100%");
        SetText(pactAncientKnowledgeText,
            "CONOCIMIENTO ANTIGUO " + state.ancientKnowledge.ToString("0.##"));
        if (progressSlider != null)
            progressSlider.value = 100f;

        string[] visibleLineIds =
        {
            D2Civilization3System.ResonantExpeditionLineId,
            D2Civilization3System.EndlessArchiveLineId,
            D2Civilization3System.SharedMemoryLineId,
            D2Civilization3System.ModulatorResonanceLineId
        };
        int selectedIndex = System.Array.IndexOf(visibleLineIds, _selectedPactLineId);
        if (selectedIndex < 0)
        {
            selectedIndex = 0;
            _selectedPactLineId = visibleLineIds[0];
        }
        for (int i = 0; i < visibleLineIds.Length; i++)
        {
            int cardLevel = D2Civilization3System.GetEntityPactLineLevel(
                state, visibleLineIds[i]);
            if (pactCardTitleTexts != null && i < pactCardTitleTexts.Length &&
                pactCardTitleTexts[i] != null)
            {
                pactCardTitleTexts[i].color = i == selectedIndex
                    ? new Color32(178, 134, 193, 255)
                    : new Color32(177, 138, 92, 255);
            }
            if (pactLevelTexts != null && i < pactLevelTexts.Length)
            {
                SetText(pactLevelTexts[i], "NIVEL " + cardLevel + "/3");
                if (pactLevelTexts[i] != null)
                    pactLevelTexts[i].color = i == selectedIndex
                        ? new Color32(178, 134, 193, 255)
                        : new Color32(177, 138, 92, 255);
            }
            if (pactSelectionRoots != null && i < pactSelectionRoots.Length)
                SetActive(pactSelectionRoots[i], i == selectedIndex);
            if (pactDetailIconRoots != null && i < pactDetailIconRoots.Length)
                SetActive(pactDetailIconRoots[i], i == selectedIndex);
        }

        int level = D2Civilization3System.GetEntityPactLineLevel(state, _selectedPactLineId);
        int nextLevel = Mathf.Min(level + 1, D2Civilization3System.MaxEntityPactLineLevel);
        string selectedName = D2Civilization3System.GetEntityPactLineName(_selectedPactLineId);
        string description = D2Civilization3System.GetEntityPactLineDescription(_selectedPactLineId).TrimEnd('.');
        SetText(milestoneText,
            "PACTO — " + selectedName +
            " | NIVEL " + level + "/3\n" + D2Civilization3System.GetEntityPactLineDescription(_selectedPactLineId) +
            (level < D2Civilization3System.MaxEntityPactLineLevel
                ? "\nSIGUIENTE: " + D2Civilization3System.GetEntityPactAncientKnowledgeCost(nextLevel).ToString("0") +
                  " CONOCIMIENTO + " + D2Civilization3System.GetEntityPactZoneResourceCost(nextLevel).ToString("N0") + " DE CADA RECURSO"
                : "\nNIVEL MÁXIMO"));
        SetText(pactDetailTitleText,
            "PACTO — " + selectedName.ToUpperInvariant() + " | NIVEL " + level + "/3");
        SetText(pactDetailEffectText, FormatPactEffect(description));
        if (level < D2Civilization3System.MaxEntityPactLineLevel)
        {
            SetText(pactDetailCostText,
                "SIGUIENTE: " + D2Civilization3System.GetEntityPactAncientKnowledgeCost(nextLevel).ToString("0") +
                " CONOCIMIENTO + " + D2Civilization3System.GetEntityPactZoneResourceCost(nextLevel).ToString("N0") +
                " DE CADA RECURSO |");
            SetText(pactThresholdText,
                "UMBRAL DEL ENTE: " + D2Civilization3System.GetEntityPactKnowledgeRequirement(nextLevel).ToString("N0"));
        }
        else
        {
            SetText(pactDetailCostText, "NIVEL MÁXIMO");
            SetText(pactThresholdText, "");
        }
        SetText(resourcesText, zone1.zoneResourceAmount.ToString("N0") + " · " + zone2.zoneResourceAmount.ToString("N0") + " · " + zone3.zoneResourceAmount.ToString("N0"));
        SetText(entityKnowledgeText, state.entityKnowledge.ToString("N0") + " / 6");
        SetText(pactFragmentsValueText, zone1.zoneResourceAmount.ToString("N0"));
        SetText(pactInscriptionsValueText, zone2.zoneResourceAmount.ToString("N0"));
        SetText(pactSealsValueText, zone3.zoneResourceAmount.ToString("N0"));
        SetText(pactEntityKnowledgeValueText, state.entityKnowledge.ToString("N0") + "/6");
        string pactResult = string.IsNullOrEmpty(state.lastEntityPactResult)
            ? string.IsNullOrEmpty(state.lastResult)
                ? "EL PACTO AGUARDA UNA DECISIÓN."
                : state.lastResult
            : state.lastEntityPactResult;
        SetText(lastResultText, pactResult.ToUpperInvariant());
        SetText(pactLastResultText, pactResult);
        SetText(pactNavigationLabelText,
            state.entityPactEstablished ? "PACTO\nOPCIONAL" : "ENTE");

        string establishLabel = state.entityPactEstablished
            ? "PACTO ESTABLECIDO"
            : "ESTABLECER PACTO";
        bool canEstablish = D2Civilization3System.CanEstablishEntityPact(gameState);
        bool canUpgrade = D2Civilization3System.CanUpgradeEntityPactLine(
            gameState, _selectedPactLineId);
        SetText(pactEstablishActionText, establishLabel);
        SetButtonLabel(pactEstablishButton, establishLabel);
        SetInteractable(pactEstablishButton, canEstablish);
        SetActive(pactEstablishHighlightImage, canEstablish);
        if (pactEstablishActionText != null)
            pactEstablishActionText.color = canEstablish
                ? new Color32(216, 192, 154, 255)
                : new Color32(92, 86, 80, 255);
        SetText(pactUpgradeActionText, "MEJORAR LÍNEA");
        SetButtonLabel(pactUpgradeButton, "MEJORAR LÍNEA");
        SetInteractable(pactUpgradeButton, canUpgrade);
        SetActive(pactUpgradeHighlightImage, canUpgrade);
        if (pactUpgradeActionText != null)
            pactUpgradeActionText.color = canUpgrade
                ? new Color32(216, 192, 154, 255)
                : new Color32(92, 86, 80, 255);

        SetButtonLabel(startPauseButton, establishLabel);
        SetInteractable(startPauseButton, canEstablish);
        if (completeMilestoneButton != null)
        {
            completeMilestoneButton.gameObject.SetActive(state.entityPactEstablished);
            SetButtonLabel(completeMilestoneButton, "MEJORAR LÍNEA");
            SetInteractable(completeMilestoneButton, canUpgrade);
        }
        SetPactButtonsActive(true);
    }

    private static int GetMilestoneIndex(double milestone)
    {
        if (milestone <= 30.0) return 0;
        if (milestone <= 60.0) return 1;
        if (milestone <= 85.0) return 2;
        return 3;
    }

    private static string GetMilestoneCost(double milestone)
    {
        if (milestone <= 30.0) return "25 FRAGMENTOS BASE + 1 DATO BÁSICO";
        if (milestone <= 60.0) return "35 INSCRIPCIONES + 1 DATO SIMBÓLICO";
        if (milestone <= 85.0) return "45 SELLOS + 1 DATO PROFUNDO";
        return "50 FRAGMENTOS + 50 INSCRIPCIONES + 50 SELLOS";
    }

    private static string GetMilestoneReward(double milestone)
    {
        if (milestone <= 30.0) return "+1 CONOCIMIENTO DEL ENTE";
        if (milestone <= 60.0) return "+2 CONOCIMIENTO DEL ENTE";
        if (milestone <= 85.0) return "+3 CONOCIMIENTO DEL ENTE";
        return "PREPARAR PACTO OPCIONAL";
    }

    private static string FormatResearchResult(string result)
    {
        if (string.IsNullOrEmpty(result))
            return "ÚLTIMO RESULTADO — LA INVESTIGACIÓN TODAVÍA NO HA COMENZADO.";
        string normalized = result.Trim().TrimEnd('.');
        if (normalized.Equals("Investigación del Ente iniciada", System.StringComparison.OrdinalIgnoreCase) ||
            normalized.Equals("La investigación ha comenzado", System.StringComparison.OrdinalIgnoreCase))
            normalized = "LA INVESTIGACIÓN HA COMENZADO";
        else
            normalized = normalized.ToUpperInvariant();
        return "ÚLTIMO RESULTADO — " + normalized + ".";
    }

    private static string FormatPactEffect(string description)
    {
        string value = string.IsNullOrWhiteSpace(description)
            ? "Sin efecto"
            : description.Trim().TrimEnd('.');
        int split = value.IndexOf(' ');
        if (split <= 0)
            return value + ".";
        return "<color=#65C5C5>" + value.Substring(0, split) + "</color>" +
            value.Substring(split) + ".";
    }

    private void StartOrPause()
    {
        D2Civilization3State state = GameState.I?.dimension2?.civilization3;
        if (state != null && state.entityResearchMilestone100Completed)
            D2Civilization3System.TryEstablishEntityPact(GameState.I);
        else if (state != null && state.entityResearchActive)
            D2Civilization3System.TryPauseEntityResearch(GameState.I);
        else
            D2Civilization3System.TryStartEntityResearch(GameState.I);
        Refresh();
    }

    private void CompleteMilestone()
    {
        D2Civilization3State state = GameState.I?.dimension2?.civilization3;
        if (state != null && state.entityResearchMilestone100Completed)
            D2Civilization3System.TryUpgradeEntityPactLine(GameState.I, _selectedPactLineId);
        else
            D2Civilization3System.TryPayEntityResearchMilestone(GameState.I);
        Refresh();
    }

    private void SelectPactLine(string lineId)
    {
        _selectedPactLineId = lineId;
        Refresh();
    }

    private void EstablishPact()
    {
        D2Civilization3System.TryEstablishEntityPact(GameState.I);
        Refresh();
    }

    private void UpgradeSelectedPactLine()
    {
        D2Civilization3System.TryUpgradeEntityPactLine(GameState.I, _selectedPactLineId);
        Refresh();
    }

    private void SetPactButtonsActive(bool active)
    {
        SetActive(resonantExpeditionButton, active);
        SetActive(endlessArchiveButton, active);
        SetActive(sharedMemoryButton, active);
        SetActive(modulatorResonanceButton, active);
        SetActive(firstThresholdChronicleButton, false);
    }

    private void ShowArchaeology() { if (civilization3PanelUI != null) civilization3PanelUI.ShowArchaeology(); }
    private void ShowAnalysis() { if (civilization3PanelUI != null) civilization3PanelUI.ShowAnalysis(); }
    private void ShowArchive() { if (civilization3PanelUI != null) civilization3PanelUI.ShowArchive(); }
    private void BackToMap() { if (civilization3PanelUI != null) civilization3PanelUI.BackToMapFromChild(); }

    private static void SetText(TMP_Text target, string value) { if (target != null) target.text = value; }
    private static void SetInteractable(Button target, bool value) { if (target != null) target.interactable = value; }
    private static void SetActive(Component target, bool active) { if (target != null) target.gameObject.SetActive(active); }
    private static void SetActive(GameObject target, bool active) { if (target != null) target.SetActive(active); }

    private static void SetButtonLabel(Button target, string value)
    {
        if (target == null) return;
        TMP_Text label = target.GetComponentInChildren<TMP_Text>(true);
        if (label != null) label.text = value;
    }
}

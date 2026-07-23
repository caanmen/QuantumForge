using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D2EntityResearchPanelUI : MonoBehaviour
{
    private float _refreshTimer;
    private string _selectedPactLineId = D2Civilization3System.ResonantExpeditionLineId;

    public D2Civilization3PanelUI civilization3PanelUI;
    public TMP_Text unlockText;
    public TMP_Text statusText;
    public TMP_Text progressText;
    public Slider progressSlider;
    public TMP_Text milestoneText;
    public TMP_Text resourcesText;
    public TMP_Text entityKnowledgeText;
    public TMP_Text lastResultText;
    public Button startPauseButton;
    public Button completeMilestoneButton;
    public Button resonantExpeditionButton;
    public Button endlessArchiveButton;
    public Button sharedMemoryButton;
    public Button modulatorResonanceButton;
    public Button firstThresholdChronicleButton;
    public Button backToArchaeologyButton;
    public Button backToMapButton;

    private void Awake()
    {
        if (startPauseButton != null)
            startPauseButton.onClick.AddListener(StartOrPause);
        if (completeMilestoneButton != null)
            completeMilestoneButton.onClick.AddListener(CompleteMilestone);
        if (resonantExpeditionButton != null)
            resonantExpeditionButton.onClick.AddListener(() => SelectPactLine(
                D2Civilization3System.ResonantExpeditionLineId));
        if (endlessArchiveButton != null)
            endlessArchiveButton.onClick.AddListener(() => SelectPactLine(
                D2Civilization3System.EndlessArchiveLineId));
        if (sharedMemoryButton != null)
            sharedMemoryButton.onClick.AddListener(() => SelectPactLine(
                D2Civilization3System.SharedMemoryLineId));
        if (modulatorResonanceButton != null)
            modulatorResonanceButton.onClick.AddListener(() => SelectPactLine(
                D2Civilization3System.ModulatorResonanceLineId));
        if (firstThresholdChronicleButton != null)
            firstThresholdChronicleButton.onClick.AddListener(() => SelectPactLine(
                D2Civilization3System.FirstThresholdChronicleLineId));
        if (backToArchaeologyButton != null)
            backToArchaeologyButton.onClick.AddListener(BackToArchaeology);
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
        D2C3ZoneState zone1 = D2Civilization3System.GetZone(
            state, D2Civilization3System.Zone1Id);
        D2C3ZoneState zone2 = D2Civilization3System.GetZone(
            state, D2Civilization3System.Zone2Id);
        D2C3ZoneState zone3 = D2Civilization3System.GetZone(
            state, D2Civilization3System.Zone3Id);

        if (state.entityResearchMilestone100Completed)
        {
            RefreshPactPhase(gameState, state, zone1, zone2, zone3);
            return;
        }

        SetText(
            unlockText,
            state.entityResearchUnlocked
                ? "INVESTIGACIÓN DESBLOQUEADA — los tres Datos Anómalos han trazado un patrón"
                : "BLOQUEADA — requiere 1 Dato Básico, 1 Simbólico y 1 Profundo"
        );
        string status = state.entityResearchMilestone100Completed
            ? "COMPLETADA — Pacto con el Ente preparado"
            : state.entityResearchActive
                ? state.ancientKnowledge > 0.0
                    ? "EN CURSO — consume 1 Conocimiento por cada 1%"
                    : "ESPERANDO — falta Conocimiento Antiguo"
                : "PAUSADA";
        SetText(statusText, status);
        SetText(
            progressText,
            "PROGRESO: " + state.entityResearchProgress.ToString("0.##") +
            "% | Conocimiento Antiguo: " + state.ancientKnowledge.ToString("0.##")
        );
        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = 100f;
            progressSlider.value = (float)state.entityResearchProgress;
        }

        double pendingMilestone = D2Civilization3System.GetPendingEntityResearchMilestone(state);
        SetText(milestoneText, GetMilestoneDescription(state, pendingMilestone));
        SetText(
            resourcesText,
            "RECURSOS — Fragmentos: " + zone1.zoneResourceAmount.ToString("N0") +
            " | Inscripciones: " + zone2.zoneResourceAmount.ToString("N0") +
            " | Sellos: " + zone3.zoneResourceAmount.ToString("N0") +
            "\nDATOS — Básicos: " + zone1.anomalousData.ToString("N0") +
            " | Simbólicos: " + zone2.anomalousData.ToString("N0") +
            " | Profundos: " + zone3.anomalousData.ToString("N0")
        );
        SetText(
            entityKnowledgeText,
            "CONOCIMIENTO DEL ENTE: " + state.entityKnowledge.ToString("N0") +
            "/6 — permanente y no gastable durante 4F"
        );
        SetText(
            lastResultText,
            string.IsNullOrEmpty(state.lastResult)
                ? "La investigación todavía no ha comenzado."
                : state.lastResult
        );

        SetButtonLabel(
            startPauseButton,
            state.entityResearchActive ? "PAUSAR INVESTIGACIÓN" : "INICIAR INVESTIGACIÓN"
        );
        SetInteractable(
            startPauseButton,
            state.entityResearchActive || D2Civilization3System.CanStartEntityResearch(gameState)
        );
        bool milestoneReached = state.entityResearchUnlocked &&
            !state.entityResearchMilestone100Completed &&
            state.entityResearchProgress >= pendingMilestone - 0.000001;
        if (completeMilestoneButton != null)
        {
            completeMilestoneButton.gameObject.SetActive(milestoneReached);
            SetButtonLabel(
                completeMilestoneButton,
                pendingMilestone >= 100.0 ? "PREPARAR PACTO" : "COMPLETAR HITO"
            );
            SetInteractable(
                completeMilestoneButton,
                D2Civilization3System.CanPayEntityResearchMilestone(gameState)
            );
        }
        SetPactButtonsActive(false);
    }

    private void RefreshPactPhase(
        GameState gameState,
        D2Civilization3State state,
        D2C3ZoneState zone1,
        D2C3ZoneState zone2,
        D2C3ZoneState zone3
    )
    {
        SetText(unlockText, "INVESTIGACIÓN COMPLETADA — Pacto con el Ente disponible");
        SetText(statusText, state.entityPactEstablished
            ? "PACTO CON EL ENTE — ESTABLECIDO"
            : "PACTO PREPARADO — pendiente de establecer");
        SetText(progressText, "PROGRESO: 100% | Conocimiento Antiguo: " +
            state.ancientKnowledge.ToString("0.##"));
        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = 100f;
            progressSlider.value = 100f;
        }
        int level = D2Civilization3System.GetEntityPactLineLevel(
            state, _selectedPactLineId);
        int nextLevel = Mathf.Min(level + 1,
            D2Civilization3System.MaxEntityPactLineLevel);
        SetText(milestoneText,
            "PACTO — " + D2Civilization3System.GetEntityPactLineName(_selectedPactLineId) +
            " | Nivel " + level + "/3\n" +
            D2Civilization3System.GetEntityPactLineDescription(_selectedPactLineId) +
            (level < D2Civilization3System.MaxEntityPactLineLevel
                ? "\nSiguiente: " +
                  D2Civilization3System.GetEntityPactAncientKnowledgeCost(nextLevel).ToString("0") +
                  " Conocimiento + " +
                  D2Civilization3System.GetEntityPactZoneResourceCost(nextLevel).ToString("N0") +
                  " de cada recurso | Umbral del Ente: " +
                  D2Civilization3System.GetEntityPactKnowledgeRequirement(nextLevel).ToString("N0")
                : "\nNIVEL MÁXIMO"));
        SetText(resourcesText,
            "RECURSOS — Fragmentos: " + zone1.zoneResourceAmount.ToString("N0") +
            " | Inscripciones: " + zone2.zoneResourceAmount.ToString("N0") +
            " | Sellos: " + zone3.zoneResourceAmount.ToString("N0"));
        SetText(entityKnowledgeText,
            "CONOCIMIENTO DEL ENTE: " + state.entityKnowledge.ToString("N0") +
            "/6 — permanente y no gastable");
        SetText(lastResultText, string.IsNullOrEmpty(state.lastResult)
            ? "El Pacto aguarda una decisión."
            : state.lastResult);
        SetButtonLabel(startPauseButton, state.entityPactEstablished
            ? "PACTO ESTABLECIDO"
            : "ESTABLECER PACTO");
        SetInteractable(startPauseButton,
            D2Civilization3System.CanEstablishEntityPact(gameState));
        if (completeMilestoneButton != null)
        {
            completeMilestoneButton.gameObject.SetActive(state.entityPactEstablished);
            SetButtonLabel(completeMilestoneButton, "MEJORAR LÍNEA");
            SetInteractable(completeMilestoneButton,
                D2Civilization3System.CanUpgradeEntityPactLine(
                    gameState, _selectedPactLineId));
        }
        SetPactButtonsActive(true);
    }

    private static string GetMilestoneDescription(
        D2Civilization3State state,
        double milestone
    )
    {
        if (state.entityResearchMilestone100Completed)
            return "HITOS 30/60/85/100% COMPLETADOS";
        if (milestone <= 30.0)
            return "PRÓXIMO HITO 30% — 25 Fragmentos Base + 1 Dato Básico → +1 Conocimiento del Ente";
        if (milestone <= 60.0)
            return "PRÓXIMO HITO 60% — 35 Inscripciones + 1 Dato Simbólico → +2 Conocimiento del Ente";
        if (milestone <= 85.0)
            return "PRÓXIMO HITO 85% — 45 Sellos + 1 Dato Profundo → +3 Conocimiento del Ente";
        return "HITO 100% — 50 Fragmentos + 50 Inscripciones + 50 Sellos → preparar Pacto";
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
            D2Civilization3System.TryUpgradeEntityPactLine(
                GameState.I, _selectedPactLineId);
        else
            D2Civilization3System.TryPayEntityResearchMilestone(GameState.I);
        Refresh();
    }

    private void SelectPactLine(string lineId)
    {
        _selectedPactLineId = lineId;
        Refresh();
    }

    private void SetPactButtonsActive(bool active)
    {
        if (resonantExpeditionButton != null)
            resonantExpeditionButton.gameObject.SetActive(active);
        if (endlessArchiveButton != null)
            endlessArchiveButton.gameObject.SetActive(active);
        if (sharedMemoryButton != null)
            sharedMemoryButton.gameObject.SetActive(active);
        if (modulatorResonanceButton != null)
            modulatorResonanceButton.gameObject.SetActive(active);
        if (firstThresholdChronicleButton != null)
            firstThresholdChronicleButton.gameObject.SetActive(active);
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

    private static void SetText(TMP_Text target, string value)
    {
        if (target != null)
            target.text = value;
    }

    private static void SetInteractable(Button target, bool value)
    {
        if (target != null)
            target.interactable = value;
    }

    private static void SetButtonLabel(Button target, string value)
    {
        if (target == null)
            return;
        TMP_Text label = target.GetComponentInChildren<TMP_Text>(true);
        if (label != null)
            label.text = value;
    }
}

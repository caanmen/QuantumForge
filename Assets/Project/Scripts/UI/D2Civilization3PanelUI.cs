using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D2Civilization3PanelUI : MonoBehaviour
{
    private float _refreshTimer;

    public Dimension2PanelUI dimension2PanelUI;
    public GameObject archaeologySectionRoot;
    public GameObject archiveSectionRoot;
    public GameObject entityResearchSectionRoot;
    public D2ArchivePanelUI archivePanelUI;
    public D2EntityResearchPanelUI entityResearchPanelUI;
    public Button showEntityResearchButton;
    public Button showArchiveButton;
    public TMP_Text zoneText;
    public TMP_Text lockedZonesText;
    public TMP_Text excavationText;
    public Slider excavationSlider;
    public TMP_Text inventoryText;
    public TMP_Text analysisText;
    public Slider analysisSlider;
    public TMP_Text researchText;
    public TMP_Text archiveText;
    public TMP_Text cluesText;
    public TMP_Text anomalyText;
    public TMP_Text scholarText;
    public TMP_Text civilization1ResourcesText;
    public TMP_Text lastResultText;
    public Button excavateButton;
    public Button zone1Button;
    public Button zone2Button;
    public Button zone3Button;
    public Button unlockZone2Button;
    public Button unlockZone3Button;
    public Button analyzeLowButton;
    public Button analyzeMediumButton;
    public Button analyzeHighButton;
    public Button hireScholarButton;
    public Button readAnomalyButton;
    public Button backToMapButton;

    private void Awake()
    {
        if (showEntityResearchButton != null)
            showEntityResearchButton.onClick.AddListener(ShowEntityResearch);
        if (showArchiveButton != null)
            showArchiveButton.onClick.AddListener(ShowArchive);
        if (excavateButton != null)
            excavateButton.onClick.AddListener(StartExcavation);
        if (zone1Button != null)
            zone1Button.onClick.AddListener(() => SelectZone(D2Civilization3System.Zone1Id));
        if (zone2Button != null)
            zone2Button.onClick.AddListener(() => SelectZone(D2Civilization3System.Zone2Id));
        if (zone3Button != null)
            zone3Button.onClick.AddListener(() => SelectZone(D2Civilization3System.Zone3Id));
        if (unlockZone2Button != null)
            unlockZone2Button.onClick.AddListener(UnlockZone2);
        if (unlockZone3Button != null)
            unlockZone3Button.onClick.AddListener(UnlockZone3);
        if (analyzeLowButton != null)
            analyzeLowButton.onClick.AddListener(() => StartAnalysis(D2Civilization3System.LowQualityId));
        if (analyzeMediumButton != null)
            analyzeMediumButton.onClick.AddListener(() => StartAnalysis(D2Civilization3System.MediumQualityId));
        if (analyzeHighButton != null)
            analyzeHighButton.onClick.AddListener(() => StartAnalysis(D2Civilization3System.HighQualityId));
        if (hireScholarButton != null)
            hireScholarButton.onClick.AddListener(HireScholar);
        if (readAnomalyButton != null)
            readAnomalyButton.onClick.AddListener(ReadAnomaly);
        if (backToMapButton != null)
            backToMapButton.onClick.AddListener(BackToMap);
    }

    private void OnEnable()
    {
        ShowArchaeology();
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
        D2C3ZoneState zone = D2Civilization3System.GetSelectedZone(state);
        if (zone == null)
            return;
        D2C3ZoneState zone2 = D2Civilization3System.GetZone(state, D2Civilization3System.Zone2Id);
        D2C3ZoneState zone3 = D2Civilization3System.GetZone(state, D2Civilization3System.Zone3Id);
        SetInteractable(showEntityResearchButton, state.entityResearchUnlocked);
        SetInteractable(showArchiveButton, state.archiveUnlocked);
        SetButtonLabel(showEntityResearchButton,
            state.entityPactEstablished ? "PACTO" : "ENTE");

        SetText(
            zoneText,
            "ZONA " + GetZoneNumber(zone.zoneId) + " — " +
            D2Civilization3System.GetZoneName(zone.zoneId).ToUpperInvariant() +
            "\nRecurso propio: " + D2Civilization3System.GetZoneResourceName(zone.zoneId)
        );
        SetText(
            lockedZonesText,
            (zone2 != null && zone2.unlocked
                ? "ZONA 2 — GALERÍA DE INSCRIPCIONES: DISPONIBLE\n"
                : "ZONA 2 — BLOQUEADA: Zona 1 al 60% + 25 Incienso + 25 Tela Sagrada\n") +
            (zone3 != null && zone3.unlocked
                ? "ZONA 3 — SANTUARIO SELLADO: DISPONIBLE"
                : "ZONA 3 — BLOQUEADA: Zona 2 al 60% + 50 Incienso/Tela/Piedra")
        );
        double excavationDuration = D2Civilization3System.GetExcavationDuration(state);
        SetText(
            excavationText,
            zone.excavationActive
                ? "EXCAVACIÓN EN CURSO — " + FormatDuration(zone.excavationRemainingSeconds)
                : "EXCAVACIÓN DISPONIBLE — duración " +
                  FormatDuration(excavationDuration)
        );
        if (excavationSlider != null)
        {
            excavationSlider.minValue = 0f;
            excavationSlider.maxValue = (float)excavationDuration;
            excavationSlider.value = (float)(excavationDuration -
                zone.excavationRemainingSeconds);
        }
        SetText(
            inventoryText,
            "RESTOS ARQUEOLÓGICOS — Baja: " + zone.lowQualityRemains.ToString("N0") +
            " | Media: " + zone.mediumQualityRemains.ToString("N0") +
            " | Alta: " + zone.highQualityRemains.ToString("N0") +
            " | Total: " + D2Civilization3System.GetTotalRemains(zone).ToString("N0")
        );
        SetText(
            analysisText,
            zone.analysisActive
                ? "ANÁLISIS EN CURSO — calidad " +
                  D2Civilization3System.GetQualityName(zone.analysisQualityId) + " — " +
                  FormatDuration(zone.analysisRemainingSeconds)
                : zone.scholarHired
                    ? "ANÁLISIS DISPONIBLE — selecciona una calidad"
                    : "ANÁLISIS BLOQUEADO — requiere " +
                      D2Civilization3System.GetScholarName(zone.zoneId)
        );
        if (analysisSlider != null)
        {
            float duration = (float)D2Civilization3System.GetAnalysisDuration(state, zone);
            analysisSlider.minValue = 0f;
            analysisSlider.maxValue = duration;
            analysisSlider.value = duration - (float)zone.analysisRemainingSeconds;
        }
        SetText(
            researchText,
            "INVESTIGACIÓN DE " +
            "ZONA " + GetZoneNumber(zone.zoneId) +
            ": " + zone.researchProgress.ToString("0.##") +
            "% | Conocimiento Antiguo: " + state.ancientKnowledge.ToString("0.##") +
            " | " + D2Civilization3System.GetZoneResourceName(zone.zoneId) + ": " +
            zone.zoneResourceAmount.ToString("N0")
        );
        SetText(
            archiveText,
            state.archiveUnlocked
                ? "ARCHIVO " + (state.archiveLevel >= 4 ? "IV" :
                    state.archiveLevel >= 3 ? "III" :
                    state.archiveLevel >= 2 ? "II" : "I") +
                  " — DESBLOQUEADO | Hitos: 20% restos +5% · 40% análisis +5% · " +
                  "60% Zona 2 · 80% Conocimiento +10%"
                : "ARCHIVO DE INTERPRETACIÓN — se desbloquea tras el primer análisis"
        );
        SetText(
            cluesText,
            state.anomalyClueDetectionUnlocked
                ? D2Civilization3System.GetClueName(zone.zoneId).ToUpperInvariant() + ": " +
                  zone.anomalyClues.ToString("N0") + " | Acumulación: " +
                  (zone.anomalyClueProgress * 100.0).ToString("0.##") + "%"
                : "INDICIOS ANÓMALOS — se habilitan con Zona 2 al 20%"
        );
        long clueRequirement = D2Civilization3System.GetAnomalyClueRequirement(zone.zoneId);
        string anomalyStatus;
        if (!state.anomalyClueDetectionUnlocked)
        {
            anomalyStatus = "ANOMALÍAS — la detección todavía no está disponible";
        }
        else if (!zone.anomalyRevealed)
        {
            anomalyStatus = D2Civilization3System.GetAnomalyName(zone.zoneId).ToUpperInvariant() +
                " — SIN REVELAR | Indicios: " + zone.anomalyClues.ToString("N0") +
                "/" + clueRequirement.ToString("N0");
        }
        else if (zone.anomalyRead)
        {
            anomalyStatus = D2Civilization3System.GetAnomalyName(zone.zoneId).ToUpperInvariant() +
                " — LEÍDA Y ARCHIVADA | " +
                D2Civilization3System.GetAnomalousDataName(zone.zoneId) + ": " +
                zone.anomalousData.ToString("N0");
        }
        else if (state.archiveLevel <
            (zone.zoneId == D2Civilization3System.Zone3Id ? 4 : 3))
        {
            anomalyStatus = D2Civilization3System.GetAnomalyName(zone.zoneId).ToUpperInvariant() +
                (zone.zoneId == D2Civilization3System.Zone3Id
                    ? " — REVELADA | Lectura requiere Archivo IV (Zona 3 al 30%)"
                    : " — REVELADA | Lectura requiere Archivo III (Zona 2 al 40%)");
        }
        else
        {
            anomalyStatus = D2Civilization3System.GetAnomalyName(zone.zoneId).ToUpperInvariant() +
                " — REVELADA | Coste: " +
                D2Civilization3System.GetEffectiveAnomalyKnowledgeCost(
                    state, zone.zoneId).ToString("0.##") +
                " Conocimiento + " +
                D2Civilization3System.GetEffectiveAnomalyResourceCost(
                    state, zone.zoneId).ToString("N0") +
                " " + D2Civilization3System.GetZoneResourceName(zone.zoneId);
        }
        SetText(anomalyText, anomalyStatus);
        SetText(
            scholarText,
            zone.scholarHired
                ? D2Civilization3System.GetScholarName(zone.zoneId).ToUpperInvariant() +
                  " — NIVEL " + zone.scholarLevel + "/3" +
                  (zone.scholarLevel < D2Civilization3System.MaxScholarLevel
                      ? "\nSiguiente: " +
                        D2Civilization3System.GetScholarUpgradeKnowledgeCost(
                            zone.zoneId, zone.scholarLevel + 1).ToString("0") +
                        " Conocimiento + " +
                        D2Civilization3System.GetScholarUpgradeResourceCost(
                            zone.zoneId, zone.scholarLevel + 1).ToString("N0") + " " +
                        D2Civilization3System.GetZoneResourceName(zone.zoneId) +
                        " | Umbral: " +
                        D2Civilization3System.GetScholarUpgradeEntityKnowledgeRequirement(
                            zone.scholarLevel + 1).ToString("N0")
                      : " — NIVEL MÁXIMO")
                : D2Civilization3System.GetScholarName(zone.zoneId).ToUpperInvariant() +
                  " — NO CONTRATADO\nCoste: " +
                  (zone.zoneId == D2Civilization3System.Zone3Id ? "30" :
                    zone.zoneId == D2Civilization3System.Zone2Id ? "20" : "10") +
                  " Cera + " +
                  (zone.zoneId == D2Civilization3System.Zone3Id ? "30" :
                    zone.zoneId == D2Civilization3System.Zone2Id ? "20" : "10") +
                  " Pan ritual."
        );

        D2AltarState wax = D2AltarSystem.GetAltar(
            gameState.dimension2.civilization1,
            D2AltarSystem.WaxAltarId
        );
        D2AltarState bread = D2AltarSystem.GetAltar(
            gameState.dimension2.civilization1,
            D2AltarSystem.RitualBreadAltarId
        );
        D2AltarState incense = D2AltarSystem.GetAltar(
            gameState.dimension2.civilization1,
            D2AltarSystem.IncenseAltarId
        );
        D2AltarState cloth = D2AltarSystem.GetAltar(
            gameState.dimension2.civilization1,
            D2AltarSystem.SacredClothAltarId
        );
        D2AltarState stone = D2AltarSystem.GetAltar(
            gameState.dimension2.civilization1,
            D2AltarSystem.CarvedStoneAltarId
        );
        SetText(
            civilization1ResourcesText,
            "RECURSOS DE CIVILIZACIÓN 1 — Cera: " +
            (wax != null ? wax.offeringAmount : 0.0).ToString("0.##") +
            " | Pan ritual: " +
            (bread != null ? bread.offeringAmount : 0.0).ToString("0.##") +
            " | Incienso: " +
            (incense != null ? incense.offeringAmount : 0.0).ToString("0.##") +
            " | Tela: " +
            (cloth != null ? cloth.offeringAmount : 0.0).ToString("0.##") +
            " | Piedra: " +
            (stone != null ? stone.offeringAmount : 0.0).ToString("0.##")
        );
        SetText(
            lastResultText,
            string.IsNullOrEmpty(state.lastResult)
                ? "Las ruinas aguardan la primera excavación."
                : state.lastResult
        );
        SetInteractable(excavateButton, !zone.excavationActive);
        SetInteractable(zone1Button, true);
        SetInteractable(zone2Button, zone2 != null && zone2.unlocked);
        SetInteractable(zone3Button, zone3 != null && zone3.unlocked);
        if (unlockZone2Button != null)
        {
            unlockZone2Button.gameObject.SetActive(zone2 != null && !zone2.unlocked);
            SetInteractable(
                unlockZone2Button,
                D2Civilization3System.CanUnlockZone2(gameState)
            );
        }
        if (unlockZone3Button != null)
        {
            unlockZone3Button.gameObject.SetActive(
                zone2 != null && zone2.unlocked && zone3 != null && !zone3.unlocked
            );
            SetInteractable(
                unlockZone3Button,
                D2Civilization3System.CanUnlockZone3(gameState)
            );
        }
        SetInteractable(
            analyzeLowButton,
            D2Civilization3System.CanStartAnalysis(
                gameState, zone.zoneId, D2Civilization3System.LowQualityId)
        );
        SetInteractable(
            analyzeMediumButton,
            D2Civilization3System.CanStartAnalysis(
                gameState, zone.zoneId, D2Civilization3System.MediumQualityId)
        );
        SetInteractable(
            analyzeHighButton,
            D2Civilization3System.CanStartAnalysis(
                gameState, zone.zoneId, D2Civilization3System.HighQualityId)
        );
        SetButtonLabel(hireScholarButton,
            !zone.scholarHired ? "CONTRATAR ERUDITO" :
            zone.scholarLevel < D2Civilization3System.MaxScholarLevel
                ? "MEJORAR ERUDITO"
                : "ERUDITO MÁXIMO");
        SetInteractable(hireScholarButton,
            !zone.scholarHired
                ? D2Civilization3System.CanHireScholar(gameState, zone.zoneId)
                : D2Civilization3System.CanUpgradeScholar(gameState, zone.zoneId));
        if (readAnomalyButton != null)
        {
            readAnomalyButton.gameObject.SetActive(
                zone.anomalyRevealed && !zone.anomalyRead
            );
            SetInteractable(
                readAnomalyButton,
                D2Civilization3System.CanReadAnomaly(gameState, zone.zoneId)
            );
        }
        if (entityResearchPanelUI != null)
            entityResearchPanelUI.Refresh();
        if (archivePanelUI != null)
            archivePanelUI.Refresh();
    }

    private void StartExcavation()
    {
        D2Civilization3System.TryStartExcavation(
            GameState.I,
            GetSelectedZoneId()
        );
        Refresh();
    }

    private void StartAnalysis(string qualityId)
    {
        D2Civilization3System.TryStartAnalysis(
            GameState.I,
            GetSelectedZoneId(),
            qualityId
        );
        Refresh();
    }

    private void HireScholar()
    {
        D2Civilization3State state = GameState.I?.dimension2?.civilization3;
        D2C3ZoneState zone = D2Civilization3System.GetSelectedZone(state);
        if (zone != null && zone.scholarHired)
            D2Civilization3System.TryUpgradeScholar(GameState.I, zone.zoneId);
        else
            D2Civilization3System.TryHireScholar(GameState.I, GetSelectedZoneId());
        Refresh();
    }

    private void ReadAnomaly()
    {
        D2Civilization3System.TryReadAnomaly(GameState.I, GetSelectedZoneId());
        Refresh();
    }

    private void SelectZone(string zoneId)
    {
        D2Civilization3System.TrySelectZone(GameState.I, zoneId);
        Refresh();
    }

    private void UnlockZone2()
    {
        D2Civilization3System.TryUnlockZone2(GameState.I);
        Refresh();
    }

    private void UnlockZone3()
    {
        D2Civilization3System.TryUnlockZone3(GameState.I);
        Refresh();
    }

    private string GetSelectedZoneId()
    {
        D2Civilization3State state = GameState.I?.dimension2?.civilization3;
        D2C3ZoneState zone = D2Civilization3System.GetSelectedZone(state);
        return zone != null ? zone.zoneId : D2Civilization3System.Zone1Id;
    }

    private void BackToMap()
    {
        BackToMapFromChild();
    }

    public void ShowArchaeology()
    {
        if (archaeologySectionRoot != null)
            archaeologySectionRoot.SetActive(true);
        if (entityResearchSectionRoot != null)
            entityResearchSectionRoot.SetActive(false);
        if (archiveSectionRoot != null)
            archiveSectionRoot.SetActive(false);
        Refresh();
    }

    public void ShowArchive()
    {
        if (archaeologySectionRoot != null)
            archaeologySectionRoot.SetActive(false);
        if (archiveSectionRoot != null)
            archiveSectionRoot.SetActive(true);
        if (entityResearchSectionRoot != null)
            entityResearchSectionRoot.SetActive(false);
        if (archivePanelUI != null)
            archivePanelUI.Refresh();
    }

    public void ShowEntityResearch()
    {
        D2Civilization3State state = GameState.I?.dimension2?.civilization3;
        if (state == null || !state.entityResearchUnlocked)
            return;
        if (archaeologySectionRoot != null)
            archaeologySectionRoot.SetActive(false);
        if (archiveSectionRoot != null)
            archiveSectionRoot.SetActive(false);
        if (entityResearchSectionRoot != null)
            entityResearchSectionRoot.SetActive(true);
        if (entityResearchPanelUI != null)
            entityResearchPanelUI.Refresh();
    }

    public void BackToMapFromChild()
    {
        if (dimension2PanelUI != null)
            dimension2PanelUI.ShowMap();
    }

    private static string FormatDuration(double seconds)
    {
        int totalSeconds = Mathf.Max(0, Mathf.CeilToInt((float)seconds));
        return (totalSeconds / 60).ToString("00") + ":" +
            (totalSeconds % 60).ToString("00");
    }

    private static string GetZoneNumber(string zoneId)
    {
        if (zoneId == D2Civilization3System.Zone3Id) return "3";
        return zoneId == D2Civilization3System.Zone2Id ? "2" : "1";
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

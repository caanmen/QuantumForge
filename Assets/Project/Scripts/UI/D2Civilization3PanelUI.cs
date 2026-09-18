using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D2Civilization3PanelUI : MonoBehaviour
{
    private float _refreshTimer;

    public Dimension2PanelUI dimension2PanelUI;
    public GameObject archaeologySectionRoot;
    public GameObject analysisSectionRoot;
    public GameObject archiveSectionRoot;
    public GameObject entityResearchSectionRoot;
    public D2ArchivePanelUI archivePanelUI;
    public D2EntityResearchPanelUI entityResearchPanelUI;
    public Button showEntityResearchButton;
    public Button showArchiveButton;
    public Button showAnalysisButton;
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
    public TMP_Text objectiveText;
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

    [Header("Ruinas Sepultadas — composición V4")]
    public TMP_Text ancientKnowledgeHeaderText;
    public TMP_Text ancientKnowledgeHeaderValueText;
    public TMP_Text archiveHeaderText;
    public TMP_Text entityKnowledgeHeaderText;
    public TMP_Text entityKnowledgeHeaderValueText;
    public TMP_Text[] zoneNumberTexts;
    public TMP_Text[] zoneNameTexts;
    public TMP_Text[] zoneResearchValueTexts;
    public TMP_Text[] zoneResourceNameTexts;
    public TMP_Text[] zoneResourceValueTexts;
    public Image[] zoneResearchFillImages;
    public GameObject[] zoneSelectionOverlays;
    public GameObject[] zoneRailMarkers;
    public TMP_Text detailTitleText;
    public TMP_Text detailExcavationText;
    public TMP_Text remainsLowText;
    public TMP_Text remainsMediumText;
    public TMP_Text remainsHighText;
    public TMP_Text detailAnalysisText;
    public TMP_Text clueTitleText;
    public TMP_Text cluePatternText;
    public TMP_Text clueAccumulationText;
    public TMP_Text excavationActionText;
    public Image excavationActionHighlight;

    [Header("Analizar Restos — composición V4")]
    public Button analysisBackButton;
    public Button analysisExcavateButton;
    public Button analysisArchiveButton;
    public Button analysisEntityButton;
    public Button[] analysisQualityButtons;
    public Button analysisActionButton;
    public GameObject[] analysisQualitySelectionOverlays;
    public TMP_Text analysisZoneTitleText;
    public TMP_Text analysisAncientKnowledgeValueText;
    public TMP_Text analysisResourceNameText;
    public TMP_Text analysisResourceValueText;
    public TMP_Text[] analysisQualityNameTexts;
    public TMP_Text[] analysisQualityCountTexts;
    public TMP_Text analysisTotalRemainsText;
    public TMP_Text analysisAvailabilityText;
    public TMP_Text analysisSelectedQualityText;
    public TMP_Text analysisDurationText;
    public TMP_Text analysisRewardsText;
    public TMP_Text[] analysisRewardLineTexts;
    public TMP_Text analysisActionText;
    public TMP_Text analysisScholarTitleText;
    public TMP_Text analysisScholarNextText;
    public TMP_Text analysisScholarUpgradeText;
    public TMP_Text analysisResearchTitleText;
    public TMP_Text analysisResearchValueText;
    public Image analysisResearchFillImage;
    public TMP_Text[] analysisCivilization1ResourceValueTexts;

    private string _selectedAnalysisQualityId = D2Civilization3System.MediumQualityId;

    private void Awake()
    {
        if (showEntityResearchButton != null)
            showEntityResearchButton.onClick.AddListener(ShowEntityResearch);
        if (showArchiveButton != null)
            showArchiveButton.onClick.AddListener(ShowArchive);
        if (showAnalysisButton != null)
            showAnalysisButton.onClick.AddListener(ShowAnalysis);
        if (analysisBackButton != null)
            analysisBackButton.onClick.AddListener(ShowArchaeology);
        if (analysisExcavateButton != null)
            analysisExcavateButton.onClick.AddListener(ShowArchaeology);
        if (analysisArchiveButton != null)
            analysisArchiveButton.onClick.AddListener(ShowArchive);
        if (analysisEntityButton != null)
            analysisEntityButton.onClick.AddListener(ShowEntityResearch);
        if (analysisQualityButtons != null)
        {
            for (int i = 0; i < analysisQualityButtons.Length; i++)
            {
                int qualityIndex = i;
                if (analysisQualityButtons[i] != null)
                    analysisQualityButtons[i].onClick.AddListener(
                        () => SelectAnalysisQuality(qualityIndex));
            }
        }
        if (analysisActionButton != null)
            analysisActionButton.onClick.AddListener(StartSelectedAnalysis);
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
        FeaturePresentationState analysisFeature = D2PresentationRules.GetFeatureState(
            gameState, PresentationFeatureIds.D2C3Analysis);
        FeaturePresentationState archiveFeature = D2PresentationRules.GetFeatureState(
            gameState, PresentationFeatureIds.D2C3Archive);
        FeaturePresentationState cluesFeature = D2PresentationRules.GetFeatureState(
            gameState, PresentationFeatureIds.D2C3Clues);
        FeaturePresentationState anomaliesFeature = D2PresentationRules.GetFeatureState(
            gameState, PresentationFeatureIds.D2C3Anomalies);
        FeaturePresentationState entityFeature = D2PresentationRules.GetFeatureState(
            gameState, PresentationFeatureIds.D2C3EntityResearch);
        string[] visibleZones =
            D2Civilization3PresentationRules.GetVisibleZoneIds(state);
        string[] possessedQualities =
            D2Civilization3PresentationRules.GetPossessedQualityIds(zone);
        bool hasExcavated =
            D2Civilization3PresentationRules.HasExcavationProgress(state);
        bool hasAnalyzed =
            D2Civilization3PresentationRules.HasAnalysisProgress(state);
        // Las cuatro pestañas forman parte permanente del marco de navegación.
        // Su disponibilidad se expresa con interactabilidad, no ocultando la placa.
        SetActive(showEntityResearchButton, true);
        SetActive(showArchiveButton, true);
        SetInteractable(showEntityResearchButton, entityFeature.CanOpen);
        SetInteractable(showArchiveButton, archiveFeature.CanOpen);
        SetButtonLabel(showEntityResearchButton,
            state.entityPactEstablished ? "PACTO OPCIONAL" : "ENTE",
            entityFeature.isNew);

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
        string nextZoneText = "";
        if (D2Civilization3PresentationRules.Contains(
                visibleZones, D2Civilization3System.Zone3Id))
            nextZoneText = zone3 != null && zone3.unlocked
                ? "ZONA 3 — SANTUARIO SELLADO: DISPONIBLE"
                : "PRÓXIMA: ZONA 3 — requiere Zona 2 al 60% + 50 Incienso/Tela/Piedra";
        else if (D2Civilization3PresentationRules.Contains(
                visibleZones, D2Civilization3System.Zone2Id))
            nextZoneText = zone2 != null && zone2.unlocked
                ? "ZONA 2 — GALERÍA DE INSCRIPCIONES: DISPONIBLE"
                : "PRÓXIMA: ZONA 2 — requiere Zona 1 al 60% + 25 Incienso + 25 Tela Sagrada";
        SetText(lockedZonesText, nextZoneText);

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
        SetText(archiveText,
            "ARCHIVO " + (state.archiveLevel >= 4 ? "IV" :
                state.archiveLevel >= 3 ? "III" :
                state.archiveLevel >= 2 ? "II" : "I") + " — " +
            D2Civilization3PresentationRules.GetCurrentArchiveDiscovery(state));
        SetText(
            cluesText,
            state.anomalyClueDetectionUnlocked
                ? D2Civilization3System.GetClueName(zone.zoneId).ToUpperInvariant() + ": " +
                  zone.anomalyClues.ToString("N0") + " | Acumulación: " +
                  (zone.anomalyClueProgress * 100.0).ToString("0.##") + "%"
                : "INDICIOS ANÓMALOS — se habilitan con Zona 2 al 20%"
        );
        long clueRequirement = D2Civilization3System.GetAnomalyClueRequirement(zone.zoneId);
        SetText(cluesText,
            D2Civilization3System.GetClueName(zone.zoneId).ToUpperInvariant() +
            " — PATRÓN INCOMPLETO " + zone.anomalyClues.ToString("N0") + "/" +
            clueRequirement.ToString("N0") + " | Acumulación: " +
            (zone.anomalyClueProgress * 100.0).ToString("0.##") + "%");
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
        SetText(objectiveText, GetObjective(state, zone));
        SetInteractable(excavateButton, !zone.excavationActive);
        SetInteractable(zone1Button, true);
        SetInteractable(zone2Button, zone2 != null && zone2.unlocked);
        SetInteractable(zone3Button, zone3 != null && zone3.unlocked);
        SetActive(zone2Button, D2Civilization3PresentationRules.Contains(
            visibleZones, D2Civilization3System.Zone2Id));
        SetActive(zone3Button, D2Civilization3PresentationRules.Contains(
            visibleZones, D2Civilization3System.Zone3Id));
        SetActive(lockedZonesText, !string.IsNullOrEmpty(nextZoneText));
        if (unlockZone2Button != null)
        {
            unlockZone2Button.gameObject.SetActive(
                D2Civilization3PresentationRules.Contains(
                    visibleZones, D2Civilization3System.Zone2Id) &&
                zone2 != null && !zone2.unlocked);
            SetInteractable(
                unlockZone2Button,
                D2Civilization3System.CanUnlockZone2(gameState)
            );
        }
        if (unlockZone3Button != null)
        {
            unlockZone3Button.gameObject.SetActive(
                D2Civilization3PresentationRules.Contains(
                    visibleZones, D2Civilization3System.Zone3Id) &&
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
        SetActive(inventoryText, hasExcavated);
        SetActive(analysisText, analysisFeature.IsVisible);
        SetActive(analysisSlider, analysisFeature.IsVisible && zone.analysisActive);
        SetActive(analyzeLowButton, analysisFeature.IsVisible &&
            D2Civilization3PresentationRules.Contains(
                possessedQualities, D2Civilization3System.LowQualityId));
        SetActive(analyzeMediumButton, analysisFeature.IsVisible &&
            D2Civilization3PresentationRules.Contains(
                possessedQualities, D2Civilization3System.MediumQualityId));
        SetActive(analyzeHighButton, analysisFeature.IsVisible &&
            D2Civilization3PresentationRules.Contains(
                possessedQualities, D2Civilization3System.HighQualityId));
        SetActive(scholarText, hasExcavated);
        SetActive(hireScholarButton, hasExcavated);
        SetActive(civilization1ResourcesText, hasExcavated);
        SetActive(researchText, hasAnalyzed);
        SetActive(archiveText, archiveFeature.IsVisible);
        SetActive(cluesText, cluesFeature.IsVisible);
        SetActive(anomalyText, anomaliesFeature.IsVisible);
        if (readAnomalyButton != null)
        {
            readAnomalyButton.gameObject.SetActive(
                anomaliesFeature.IsVisible && zone.anomalyRevealed && !zone.anomalyRead
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

        RefreshRuinsReferenceVisual(state, zone);
        RefreshAnalyzeReferenceVisual(gameState, state, zone);
    }

    private void RefreshRuinsReferenceVisual(
        D2Civilization3State state, D2C3ZoneState selectedZone)
    {
        SetText(ancientKnowledgeHeaderText, "CONOCIMIENTO ANTIGUO");
        SetText(ancientKnowledgeHeaderValueText,
            state.ancientKnowledge.ToString("0.##"));
        SetText(archiveHeaderText, "ARCHIVO " + ToRomanArchiveLevel(state.archiveLevel));
        SetText(entityKnowledgeHeaderText, "CONOCIMIENTO DEL ENTE");
        SetText(entityKnowledgeHeaderValueText,
            state.entityKnowledge.ToString("0.##") + " / 6");

        string[] zoneIds =
        {
            D2Civilization3System.Zone1Id,
            D2Civilization3System.Zone2Id,
            D2Civilization3System.Zone3Id
        };
        Color bronze = Hex("B18A5C");
        Color teal = Hex("65C5C5");
        Color purple = Hex("B286C1");

        for (int i = 0; i < zoneIds.Length; i++)
        {
            D2C3ZoneState current = D2Civilization3System.GetZone(state, zoneIds[i]);
            if (current == null)
                continue;

            bool selected = current.zoneId == selectedZone.zoneId;
            Color accent = selected ? purple : bronze;
            SetTextAt(zoneNumberTexts, i, (i + 1).ToString());
            SetTextAt(zoneNameTexts, i,
                "ZONA " + (i + 1) + " · " +
                D2Civilization3System.GetZoneName(current.zoneId).ToUpperInvariant());
            SetTextAt(zoneResearchValueTexts, i,
                current.researchProgress.ToString("0.##") + "%");
            SetTextAt(zoneResourceNameTexts, i,
                D2Civilization3System.GetZoneResourceName(current.zoneId).ToUpperInvariant());
            SetTextAt(zoneResourceValueTexts, i,
                current.zoneResourceAmount.ToString("N0"));

            SetColorAt(zoneNumberTexts, i, accent);
            SetColorAt(zoneNameTexts, i, accent);
            SetActiveAt(zoneSelectionOverlays, i, selected);
            SetActiveAt(zoneRailMarkers, i, selected);
            if (zoneResearchFillImages != null && i < zoneResearchFillImages.Length &&
                zoneResearchFillImages[i] != null)
            {
                float progress = Mathf.Clamp01((float)(current.researchProgress / 100.0));
                zoneResearchFillImages[i].fillAmount = progress;
                RectTransform fillRect = zoneResearchFillImages[i].rectTransform;
                Vector2 fillSize = fillRect.sizeDelta;
                fillSize.x = 289f / 941f * 1080f * progress;
                fillRect.sizeDelta = fillSize;
                zoneResearchFillImages[i].color = selected ? purple : teal;
            }
        }

        SetText(detailTitleText,
            D2Civilization3System.GetZoneName(selectedZone.zoneId).ToUpperInvariant());
        SetText(detailExcavationText,
            selectedZone.excavationActive
                ? "EXCAVACIÓN EN CURSO · " +
                  FormatDuration(selectedZone.excavationRemainingSeconds)
                : "EXCAVACIÓN DISPONIBLE · DURACIÓN " +
                  FormatDuration(D2Civilization3System.GetExcavationDuration(state)));
        SetText(remainsLowText, selectedZone.lowQualityRemains.ToString("N0"));
        SetText(remainsMediumText, selectedZone.mediumQualityRemains.ToString("N0"));
        SetText(remainsHighText, selectedZone.highQualityRemains.ToString("N0"));
        SetText(detailAnalysisText,
            selectedZone.scholarHired
                ? "ANÁLISIS DISPONIBLE · " +
                  D2Civilization3System.GetScholarName(selectedZone.zoneId).ToUpperInvariant() +
                  " NIVEL " + selectedZone.scholarLevel + "/3"
                : "ANÁLISIS BLOQUEADO · REQUIERE " +
                  D2Civilization3System.GetScholarName(selectedZone.zoneId).ToUpperInvariant());
        SetText(clueTitleText,
            D2Civilization3System.GetClueName(selectedZone.zoneId).ToUpperInvariant());
        long clueRequirement =
            D2Civilization3System.GetAnomalyClueRequirement(selectedZone.zoneId);
        SetText(cluePatternText,
            "PATRÓN        " + selectedZone.anomalyClues.ToString("N0") + " / " +
            clueRequirement.ToString("N0"));
        SetText(clueAccumulationText,
            "ACUMULACIÓN        " +
            (selectedZone.anomalyClueProgress * 100.0).ToString("0.##") + "%");
        SetText(excavationActionText,
            selectedZone.excavationActive ? "EXCAVACIÓN EN CURSO" : "INICIAR EXCAVACIÓN");
        if (excavationActionHighlight != null)
            excavationActionHighlight.gameObject.SetActive(!selectedZone.excavationActive);
    }

    private void RefreshAnalyzeReferenceVisual(
        GameState gameState,
        D2Civilization3State state,
        D2C3ZoneState zone)
    {
        if (analysisSectionRoot == null)
            return;

        string[] qualityIds =
        {
            D2Civilization3System.LowQualityId,
            D2Civilization3System.MediumQualityId,
            D2Civilization3System.HighQualityId
        };
        long[] counts =
        {
            zone.lowQualityRemains,
            zone.mediumQualityRemains,
            zone.highQualityRemains
        };
        Color bronze = Hex("B18A5C");
        Color purple = Hex("B286C1");

        SetText(analysisZoneTitleText,
            "ZONA " + GetZoneNumber(zone.zoneId) + " — " +
            D2Civilization3System.GetZoneName(zone.zoneId).ToUpperInvariant());
        SetText(analysisAncientKnowledgeValueText,
            state.ancientKnowledge.ToString("0.##"));
        SetText(analysisResourceNameText,
            "Recurso Propio:\n" +
            D2Civilization3System.GetZoneResourceName(zone.zoneId));
        SetText(analysisResourceValueText, zone.zoneResourceAmount.ToString("N0"));

        for (int i = 0; i < qualityIds.Length; i++)
        {
            bool selected = qualityIds[i] == _selectedAnalysisQualityId;
            SetTextAt(analysisQualityNameTexts, i,
                D2Civilization3System.GetQualityName(qualityIds[i]).ToUpperInvariant());
            SetTextAt(analysisQualityCountTexts, i, counts[i].ToString("N0"));
            SetColorAt(analysisQualityNameTexts, i, selected ? purple : bronze);
            SetColorAt(analysisQualityCountTexts, i, selected ? purple : bronze);
            SetActiveAt(analysisQualitySelectionOverlays, i, selected);
            if (analysisQualityButtons != null && i < analysisQualityButtons.Length &&
                analysisQualityButtons[i] != null)
            {
                analysisQualityButtons[i].interactable = counts[i] > 0L &&
                    zone.scholarHired && !zone.analysisActive;
            }
        }

        SetText(analysisTotalRemainsText,
            "TOTAL " + D2Civilization3System.GetTotalRemains(zone).ToString("N0"));
        SetText(analysisAvailabilityText,
            zone.analysisActive
                ? "ANÁLISIS EN CURSO — " +
                  D2Civilization3System.GetQualityName(zone.analysisQualityId).ToUpperInvariant() +
                  " " + FormatDuration(zone.analysisRemainingSeconds)
                : zone.scholarHired
                    ? "ANÁLISIS DISPONIBLE — SELECCIONA UNA CALIDAD"
                    : "ANÁLISIS BLOQUEADO — REQUIERE ERUDITO");

        string qualityName =
            D2Civilization3System.GetQualityName(_selectedAnalysisQualityId).ToUpperInvariant();
        SetText(analysisSelectedQualityText, "CALIDAD " + qualityName);
        SetText(analysisDurationText,
            "DURACIÓN\n" + FormatDuration(D2Civilization3System.GetAnalysisDuration(state, zone)));
        SetTextAt(analysisRewardLineTexts, 0,
            D2Civilization3System.GetAncientKnowledgeReward(
                state, _selectedAnalysisQualityId, zone).ToString("0.##") +
            "  CONOCIMIENTO ANTIGUO");
        SetTextAt(analysisRewardLineTexts, 1,
            D2Civilization3System.GetZoneResourceReward(
                _selectedAnalysisQualityId).ToString("N0") + "  " +
            D2Civilization3System.GetZoneResourceName(zone.zoneId).ToUpperInvariant());
        SetTextAt(analysisRewardLineTexts, 2,
            "+" + D2Civilization3System.GetResearchReward(
                _selectedAnalysisQualityId).ToString("0.##") + "%  INVESTIGACIÓN");
        SetTextAt(analysisRewardLineTexts, 3,
            "+" + (D2Civilization3System.GetClueProgressReward(
                _selectedAnalysisQualityId) * 100.0).ToString("0.##") +
            "%  ACUMULACIÓN DE INDICIOS");
        SetText(analysisActionText, "ANALIZAR\n" + qualityName);
        SetInteractable(analysisActionButton,
            D2Civilization3System.CanStartAnalysis(
                gameState, zone.zoneId, _selectedAnalysisQualityId));

        SetText(analysisScholarTitleText,
            D2Civilization3System.GetScholarName(zone.zoneId).ToUpperInvariant() +
            " — NIVEL " + (zone.scholarHired ? zone.scholarLevel : 0) + "/3");
        if (zone.scholarHired && zone.scholarLevel < D2Civilization3System.MaxScholarLevel)
        {
            int nextLevel = zone.scholarLevel + 1;
            SetText(analysisScholarNextText,
                "SIGUIENTE\n" +
                D2Civilization3System.GetScholarUpgradeKnowledgeCost(
                    zone.zoneId, nextLevel).ToString("0.##") +
                "  CONOCIMIENTO\n" +
                D2Civilization3System.GetScholarUpgradeResourceCost(
                    zone.zoneId, nextLevel).ToString("N0") + "  " +
                D2Civilization3System.GetZoneResourceName(zone.zoneId).ToUpperInvariant() +
                "\nUMBRAL DEL ENTE: " +
                D2Civilization3System.GetScholarUpgradeEntityKnowledgeRequirement(
                    nextLevel).ToString("N0"));
        }
        else
        {
            SetText(analysisScholarNextText,
                zone.scholarHired ? "NIVEL MÁXIMO" : "ERUDITO NO CONTRATADO");
        }
        bool canChangeScholar = zone.scholarHired
            ? D2Civilization3System.CanUpgradeScholar(gameState, zone.zoneId)
            : D2Civilization3System.CanHireScholar(gameState, zone.zoneId);
        SetInteractable(hireScholarButton, canChangeScholar);
        SetText(analysisScholarUpgradeText,
            zone.scholarHired ? "MEJORAR ERUDITO" : "CONTRATAR ERUDITO");
        if (analysisScholarUpgradeText != null)
            analysisScholarUpgradeText.color = canChangeScholar
                ? bronze : Hex("6E6861");

        SetText(analysisResearchTitleText,
            "INVESTIGACIÓN DE ZONA " + GetZoneNumber(zone.zoneId));
        SetText(analysisResearchValueText, zone.researchProgress.ToString("0.##") + "%");
        if (analysisResearchFillImage != null)
        {
            float progress = Mathf.Clamp01((float)(zone.researchProgress / 100.0));
            Vector2 fillSize = analysisResearchFillImage.rectTransform.sizeDelta;
            fillSize.x = 698f / 941f * 1080f * progress;
            analysisResearchFillImage.rectTransform.sizeDelta = fillSize;
        }

        D2AltarState wax = D2AltarSystem.GetAltar(
            gameState.dimension2.civilization1, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(
            gameState.dimension2.civilization1, D2AltarSystem.RitualBreadAltarId);
        D2AltarState incense = D2AltarSystem.GetAltar(
            gameState.dimension2.civilization1, D2AltarSystem.IncenseAltarId);
        D2AltarState cloth = D2AltarSystem.GetAltar(
            gameState.dimension2.civilization1, D2AltarSystem.SacredClothAltarId);
        D2AltarState stone = D2AltarSystem.GetAltar(
            gameState.dimension2.civilization1, D2AltarSystem.CarvedStoneAltarId);
        double[] values =
        {
            wax != null ? wax.offeringAmount : 0.0,
            bread != null ? bread.offeringAmount : 0.0,
            incense != null ? incense.offeringAmount : 0.0,
            cloth != null ? cloth.offeringAmount : 0.0,
            stone != null ? stone.offeringAmount : 0.0
        };
        for (int i = 0; i < values.Length; i++)
            SetTextAt(analysisCivilization1ResourceValueTexts, i,
                values[i].ToString("N0"));

        SetInteractable(showAnalysisButton,
            D2PresentationRules.GetFeatureState(
                gameState, PresentationFeatureIds.D2C3Analysis).IsVisible);
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

    private void StartSelectedAnalysis()
    {
        StartAnalysis(_selectedAnalysisQualityId);
    }

    private void SelectAnalysisQuality(int qualityIndex)
    {
        _selectedAnalysisQualityId = qualityIndex == 0
            ? D2Civilization3System.LowQualityId
            : qualityIndex == 2
                ? D2Civilization3System.HighQualityId
                : D2Civilization3System.MediumQualityId;
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
        RecognizeSection(PresentationFeatureIds.D2C3Archaeology);
        if (archaeologySectionRoot != null)
            archaeologySectionRoot.SetActive(true);
        if (analysisSectionRoot != null)
            analysisSectionRoot.SetActive(false);
        if (entityResearchSectionRoot != null)
            entityResearchSectionRoot.SetActive(false);
        if (archiveSectionRoot != null)
            archiveSectionRoot.SetActive(false);
        Refresh();
    }

    public void ShowArchive()
    {
        RecognizeSection(PresentationFeatureIds.D2C3Archive);
        if (archaeologySectionRoot != null)
            archaeologySectionRoot.SetActive(false);
        if (analysisSectionRoot != null)
            analysisSectionRoot.SetActive(false);
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
        RecognizeSection(PresentationFeatureIds.D2C3EntityResearch);
        if (archaeologySectionRoot != null)
            archaeologySectionRoot.SetActive(false);
        if (analysisSectionRoot != null)
            analysisSectionRoot.SetActive(false);
        if (archiveSectionRoot != null)
            archiveSectionRoot.SetActive(false);
        if (entityResearchSectionRoot != null)
            entityResearchSectionRoot.SetActive(true);
        if (entityResearchPanelUI != null)
            entityResearchPanelUI.Refresh();
    }

    public void ShowAnalysis()
    {
        RecognizeSection(PresentationFeatureIds.D2C3Analysis);
        if (archaeologySectionRoot != null)
            archaeologySectionRoot.SetActive(false);
        if (analysisSectionRoot != null)
            analysisSectionRoot.SetActive(true);
        if (archiveSectionRoot != null)
            archiveSectionRoot.SetActive(false);
        if (entityResearchSectionRoot != null)
            entityResearchSectionRoot.SetActive(false);
        Refresh();
    }

    private static void RecognizeSection(string featureId)
    {
        GameState gameState = GameState.I;
        if (gameState?.dimension2?.presentation == null) return;
        D2PresentationRouter.RememberScreen(gameState, featureId);
        PresentationStateUtility.Acknowledge(
            gameState.dimension2.presentation, featureId);
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

    private static string ToRomanArchiveLevel(int level)
    {
        if (level >= 4) return "IV";
        if (level == 3) return "III";
        return level == 2 ? "II" : "I";
    }

    private static Color Hex(string rgb)
    {
        ColorUtility.TryParseHtmlString("#" + rgb, out Color color);
        return color;
    }

    private static void SetTextAt(TMP_Text[] targets, int index, string value)
    {
        if (targets != null && index >= 0 && index < targets.Length)
            SetText(targets[index], value);
    }

    private static void SetColorAt(TMP_Text[] targets, int index, Color color)
    {
        if (targets != null && index >= 0 && index < targets.Length &&
            targets[index] != null)
            targets[index].color = color;
    }

    private static void SetActiveAt(GameObject[] targets, int index, bool active)
    {
        if (targets != null && index >= 0 && index < targets.Length &&
            targets[index] != null)
            targets[index].SetActive(active);
    }

    private static string GetObjective(
        D2Civilization3State state, D2C3ZoneState zone)
    {
        if (!D2Civilization3PresentationRules.HasExcavationProgress(state))
            return "AHORA: realiza una Excavación.\nDESPUÉS: examina los Restos recuperados.";
        if (!zone.scholarHired)
            return "AHORA: contrata al Erudito requerido.\nDESPUÉS: analiza una calidad de Restos poseída.";
        if (!D2Civilization3PresentationRules.HasAnalysisProgress(state))
            return "AHORA: analiza un Resto disponible.\nDESPUÉS: investiga la Zona y consulta el Archivo.";
        if (state.entityResearchUnlocked && !state.entityResearchMilestone100Completed)
            return "AHORA: revisa el hito actual del Ente.\nCIERRE PRINCIPAL: Pacto Mayor de Civilización 2.";
        if (state.entityPactAvailable || state.entityPactEstablished)
            return "CONTENIDO AVANZADO OPCIONAL: Pacto con el Ente.\nCIERRE PRINCIPAL: Pacto Mayor de Civilización 2.";
        return "AHORA: continúa el ciclo excavar → analizar → investigar.";
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

    private static void SetButtonLabel(
        Button target, string value, bool isNew = false)
    {
        if (target == null)
            return;
        TMP_Text label = target.GetComponentInChildren<TMP_Text>(true);
        if (label != null)
            label.text = value + (isNew ? " · NUEVO" : "");
    }

    private static void SetActive(Component target, bool active)
    {
        if (target != null)
            target.gameObject.SetActive(active);
    }
}

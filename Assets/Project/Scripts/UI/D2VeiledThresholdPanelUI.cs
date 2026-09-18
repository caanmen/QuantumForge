using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D2VeiledThresholdPanelUI : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text revelationText;
    public TMP_Text placeText;
    public TMP_Text pendingText;
    public TMP_Text resourcesText;
    public TMP_Text acolytesText;
    public TMP_Dropdown lineDropdown;
    public TMP_Text lineText;
    public Button prepareButton;
    public Button assignAcolyteButton;
    public Button releaseAcolyteButton;
    public Button upgradeLineButton;

    public TMP_Text incenseValueText;
    public TMP_Text sacredClothValueText;
    public TMP_Text carvedStoneValueText;
    public TMP_Text progressValueText;
    public TMP_Text[] lineNameTexts;
    public TMP_Text[] lineLevelTexts;
    public Button[] lineButtons;
    public TMP_Text centerStateText;
    public TMP_Text detailTitleText;
    public TMP_Text detailLevelText;
    public TMP_Text effectText;
    public TMP_Text progressCostText;
    public TMP_Text incenseCostText;
    public TMP_Text sacredClothCostText;
    public TMP_Text carvedStoneCostText;
    public TMP_Text availableAcolytesValueText;
    public TMP_Text assignedAcolytesValueText;
    public TMP_Text actionButtonText;
    public TMP_Text thresholdTabLabelText;
    public Button backButton;
    public Button refugeNavButton;
    public Button altarsNavButton;
    public Button pilgrimagesNavButton;
    public Button novitiateNavButton;
    public Button ritesNavButton;
    public Button pactsNavButton;

    private float _refreshTimer;

    public string SelectedLineId => GetSelectedLineId();

    private void Awake()
    {
        ConfigureDropdown();
        Add(prepareButton, Prepare);
        Add(assignAcolyteButton, AssignAcolyte);
        Add(releaseAcolyteButton, ReleaseAcolyte);
        Add(upgradeLineButton, UpgradeLine);
        if (lineDropdown != null)
            lineDropdown.onValueChanged.AddListener(_ => Refresh());
        if (lineButtons != null)
        {
            for (int index = 0; index < lineButtons.Length && index < D2BondSystem.LineIds.Length; index++)
            {
                string lineId = D2BondSystem.LineIds[index];
                Add(lineButtons[index], () => SelectLine(lineId));
            }
        }
        Add(backButton, ShowMap);
        Add(refugeNavButton, () => Parent()?.ShowRefugeSection());
        Add(altarsNavButton, () => Parent()?.ShowAltarsSection());
        Add(pilgrimagesNavButton, () => Parent()?.ShowPilgrimagesSection());
        Add(novitiateNavButton, () => Parent()?.ShowNovitiateSection());
        Add(ritesNavButton, () => Parent()?.ShowRitesSection());
        Add(pactsNavButton, () => Parent()?.ShowPactsSection());
    }

    private void OnEnable()
    {
        ConfigureDropdown();
        _refreshTimer = 0f;
        Refresh();
    }

    private void Update()
    {
        _refreshTimer -= Time.unscaledDeltaTime;
        if (_refreshTimer > 0f) return;
        _refreshTimer = 0.2f;
        Refresh();
    }

    public void ConfigureDropdown()
    {
        if (lineDropdown == null) return;
        int selected = Mathf.Clamp(lineDropdown.value, 0, D2BondSystem.LineIds.Length - 1);
        lineDropdown.ClearOptions();
        var options = new List<string>();
        foreach (string id in D2BondSystem.LineIds)
            options.Add(D2BondSystem.GetDisplayName(id));
        lineDropdown.AddOptions(options);
        lineDropdown.SetValueWithoutNotify(selected);
    }

    public void SelectLine(string lineId)
    {
        int index = System.Array.IndexOf(D2BondSystem.LineIds, lineId);
        if (index < 0 || lineDropdown == null) return;
        lineDropdown.SetValueWithoutNotify(index);
        Refresh();
    }

    public void Refresh()
    {
        GameState gameState = GameState.I;
        if (!Dimension2System.CanAccessDimension2(gameState)) return;

        gameState.EnsureDimension2State();
        D2Civilization1State state = gameState.dimension2.civilization1;
        D2BondSystem.EnsureState(state);
        bool unlocked = D2VeiledThresholdSystem.IsUnlocked(state);
        bool prepared = D2BondSystem.IsMajorPactEstablished(state);

        Set(titleText, unlocked
            ? prepared ? "PACTO — LUGAR DE VÍNCULO" : "UMBRAL — PREPARAR LUGAR DE VÍNCULO"
            : "ALGO PERMANECE EN SILENCIO");
        Set(thresholdTabLabelText, prepared ? "PACTO" : "UMBRAL");
        Set(centerStateText, prepared ? "PACTO\nESTABLECIDO" : "UMBRAL\nVELADO");

        D2AltarState incense = D2AltarSystem.GetAltar(state, D2AltarSystem.IncenseAltarId);
        D2AltarState cloth = D2AltarSystem.GetAltar(state, D2AltarSystem.SacredClothAltarId);
        D2AltarState stone = D2AltarSystem.GetAltar(state, D2AltarSystem.CarvedStoneAltarId);
        Set(incenseValueText, Number(incense?.offeringAmount ?? 0d));
        Set(sacredClothValueText, Number(cloth?.offeringAmount ?? 0d));
        Set(carvedStoneValueText, Number(stone?.offeringAmount ?? 0d));
        Set(progressValueText, Number(state.bondProgress));

        string selectedLineId = GetSelectedLineId();
        int selectedIndex = System.Array.IndexOf(D2BondSystem.LineIds, selectedLineId);
        for (int index = 0; index < D2BondSystem.LineIds.Length; index++)
        {
            int level = D2BondSystem.GetLevel(state, D2BondSystem.LineIds[index]);
            if (lineNameTexts != null && index < lineNameTexts.Length)
            {
                Set(lineNameTexts[index], GetCardDisplayName(D2BondSystem.LineIds[index]));
                lineNameTexts[index].color = index == selectedIndex
                    ? new Color32(226, 185, 121, 255)
                    : new Color32(194, 148, 91, 255);
            }
            if (lineLevelTexts != null && index < lineLevelTexts.Length)
                Set(lineLevelTexts[index], "NIVEL " + level + " / " + D2BondSystem.MaxLineLevel);
            if (lineButtons != null && index < lineButtons.Length)
                SetInteractable(lineButtons[index], prepared);
        }

        int selectedLevel = D2BondSystem.GetLevel(state, selectedLineId);
        int nextLevel = Mathf.Min(selectedLevel + 1, D2BondSystem.MaxLineLevel);
        Set(detailTitleText, D2BondSystem.GetDisplayName(selectedLineId).ToUpperInvariant());
        Set(detailLevelText, "NIVEL " + selectedLevel + " / " + D2BondSystem.MaxLineLevel);
        Set(effectText, GetEffectDisplayText(selectedLineId));
        bool atMaximum = selectedLevel >= D2BondSystem.MaxLineLevel;
        Set(progressCostText, atMaximum ? "MÁX." : Number(D2BondSystem.GetProgressCost(nextLevel)) + " PROGRESO");
        Set(incenseCostText, atMaximum ? "—" : Number(D2BondSystem.GetOfferingCost(nextLevel)) + " INCIENSO");
        Set(sacredClothCostText, atMaximum ? "—" : Number(D2BondSystem.GetOfferingCost(nextLevel)) + " TELA SAGRADA");
        Set(carvedStoneCostText, atMaximum ? "—" : Number(D2BondSystem.GetOfferingCost(nextLevel)) + " PIEDRA TALLADA");
        Set(availableAcolytesValueText, state.acolytesAvailable.ToString("N0", CultureInfo.InvariantCulture));
        Set(assignedAcolytesValueText, state.acolytesAssignedToBond.ToString("N0", CultureInfo.InvariantCulture));

        Set(revelationText, prepared ? "PACTO ESTABLECIDO" : unlocked ? "EL ENTE RESPONDE" : "UMBRAL VELADO");
        Set(placeText, prepared ? "PREPARACIÓN COMPLETADA"
            : unlocked ? "LA CIVILIZACIÓN PERMITE PREPARAR EL LUGAR DE VÍNCULO"
            : "EL LUGAR DE VÍNCULO TODAVÍA NO SE HA MANIFESTADO");
        Set(actionButtonText, prepared ? "MEJORAR LÍNEA" : "PREPARAR LUGAR");

        SetActive(prepareButton, unlocked && !prepared);
        SetActive(upgradeLineButton, prepared);
        SetInteractable(prepareButton, D2BondSystem.CanPrepare(gameState));
        SetInteractable(assignAcolyteButton, prepared && state.acolytesAvailable > 0L);
        SetInteractable(releaseAcolyteButton, prepared && state.acolytesAssignedToBond > 0L);
        SetInteractable(upgradeLineButton, prepared && D2BondSystem.CanUpgrade(gameState, selectedLineId));
    }

    private string GetSelectedLineId()
    {
        int index = lineDropdown != null ? lineDropdown.value : 0;
        return D2BondSystem.LineIds[Mathf.Clamp(index, 0, D2BondSystem.LineIds.Length - 1)];
    }

    private static string GetCardDisplayName(string lineId)
    {
        switch (lineId)
        {
            case D2BondSystem.PilgrimPathId: return "CAMINO\nPEREGRINO";
            case D2BondSystem.SacredCraftId: return "OFICIO\nSAGRADO";
            case D2BondSystem.AcolyteOrderId: return "ORDEN\nDE ACÓLITOS";
            case D2BondSystem.SanctuaryEchoId: return "ECO DEL\nSANTUARIO";
            case D2BondSystem.TraceLiturgyId: return "LITURGIA\nDE TRAZAS";
            default: return D2BondSystem.GetDisplayName(lineId).ToUpperInvariant();
        }
    }

    private static string GetEffectDisplayText(string lineId)
    {
        switch (lineId)
        {
            case D2BondSystem.PilgrimPathId:
                return "+5% LLEGADA Y RECOMPENSAS\nMATERIALES POR NIVEL";
            case D2BondSystem.SacredCraftId:
                return "+7.5% PRODUCCIÓN DE ALTARES\nPOR NIVEL";
            case D2BondSystem.AcolyteOrderId:
                return "+5% FORMACIÓN Y POTENCIA\nDE RITOS POR NIVEL";
            case D2BondSystem.SanctuaryEchoId:
                return "+1% PRODUCCIÓN DE LE\nPOR NIVEL";
            case D2BondSystem.TraceLiturgyId:
                return "+1% PRODUCCIÓN DE TRAZAS\nPOR NIVEL";
            default:
                return D2BondSystem.GetEffectDescription(lineId).ToUpperInvariant();
        }
    }

    private void Prepare() { D2BondSystem.TryPrepare(GameState.I); RefreshAll(); }
    private void AssignAcolyte() { D2BondSystem.TryAssignAcolytes(GameState.I, 1L); RefreshAll(); }
    private void ReleaseAcolyte() { D2BondSystem.TryReleaseAcolytes(GameState.I, 1L); RefreshAll(); }
    private void UpgradeLine() { D2BondSystem.TryUpgrade(GameState.I, GetSelectedLineId()); RefreshAll(); }
    private D2Civilization1PanelUI Parent() => GetComponentInParent<D2Civilization1PanelUI>(true);
    private void RefreshAll() { D2Civilization1PanelUI parent = Parent(); if (parent != null) parent.Refresh(); else Refresh(); }
    private void ShowMap() { Dimension2PanelUI panel = GetComponentInParent<Dimension2PanelUI>(true); if (panel != null) panel.ShowMap(); }
    private static string Number(double value) => value.ToString("N0", CultureInfo.InvariantCulture);
    private static void Set(TMP_Text text, string value) { if (text != null) text.text = value; }
    private static void SetInteractable(Button button, bool value) { if (button != null) button.interactable = value; }
    private static void SetActive(Component component, bool value) { if (component != null) component.gameObject.SetActive(value); }
    private static void Add(Button button, UnityEngine.Events.UnityAction action) { if (button != null) button.onClick.AddListener(action); }
}

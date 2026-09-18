using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D2ResistancePanelUI : MonoBehaviour
{
    [Header("Compatibilidad con la presentación anterior")]
    public TMP_Dropdown upgradeDropdown;
    public TMP_Text upgradeText;
    public Button upgradeButton;
    public TMP_Dropdown pactDropdown;
    public TMP_Text pactText;

    [Header("Cabecera")]
    public TMP_Text availableMembersText;
    public TMP_Text fragmentsText;
    public TMP_Text exhaustedText;

    [Header("Mejoras — orden canónico por ID")]
    public GameObject[] upgradeCardRoots;
    public Button[] upgradeCardButtons;
    public TMP_Text[] upgradeLevelTexts;
    public TMP_Text[] upgradeCostTexts;
    public GameObject[] upgradeCostNormalRoots;
    public TMP_Text[] upgradeMaxTexts;

    [Header("Pactos — orden canónico por ID")]
    public GameObject[] pactCardRoots;
    public Button[] pactCardButtons;
    public GameObject[] pactSelectionRoots;
    public TMP_Text[] pactStateTexts;
    public TMP_Text[] pactMembersOrRequirementTexts;
    public TMP_Text[] pactEffectTexts;
    public TMP_Text[] pactWearTexts;
    public GameObject[] pactActiveMemberIcons;
    public GameObject[] pactInactiveLockIcons;
    public GameObject[] pactEffectIcons;
    public GameObject[] pactWearIcons;

    [Header("Detalle del pacto seleccionado")]
    public TMP_Text pactDetailTitleText;
    public TMP_Text pactMembersText;
    public TMP_Text pactWearDetailText;
    public TMP_Text penaltiesText;
    public Button activateButton;
    public Button reinforceOneButton;
    public Button reinforceTenButton;
    public Button cancelButton;
    public GameObject pactAdjustRoot;
    public GameObject noPenaltyIconRoot;

    private readonly SafeDropdownOptionMap<string> _upgradeOptions =
        new SafeDropdownOptionMap<string>(System.StringComparer.Ordinal);
    private readonly SafeDropdownOptionMap<string> _pactOptions =
        new SafeDropdownOptionMap<string>(System.StringComparer.Ordinal);
    private string _selectedUpgradeId = D2Civilization2System.RescueUpgradeId;
    private string _selectedPactId = D2Civilization2System.HiddenSheltersPactId;

    private void Awake()
    {
        PopulateDropdowns();
        if (upgradeDropdown != null)
            upgradeDropdown.onValueChanged.AddListener(_ => SelectLegacyUpgrade());
        if (pactDropdown != null)
            pactDropdown.onValueChanged.AddListener(_ => SelectLegacyPact());
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(UpgradeSelected);
        WireUpgradeCards();
        WirePactCards();
        if (activateButton != null)
            activateButton.onClick.AddListener(ActivateSelectedPact);
        if (reinforceOneButton != null)
            reinforceOneButton.onClick.AddListener(() => ReinforceSelectedPact(1L));
        if (reinforceTenButton != null)
            reinforceTenButton.onClick.AddListener(() => ReinforceSelectedPact(10L));
        if (cancelButton != null)
            cancelButton.onClick.AddListener(CancelSelectedPact);
    }

    private void OnEnable()
    {
        PopulateDropdowns();
        Refresh();
    }

    public void Refresh()
    {
        GameState gameState = GameState.I;
        if (gameState?.dimension2?.civilization2 == null)
            return;

        D2Civilization2State state = gameState.dimension2.civilization2;
        PopulateDropdowns();
        string[] visibleUpgradeIds =
            D2Civilization2PresentationRules.GetVisibleUpgradeIds(state);
        string[] visiblePactIds =
            D2Civilization2PresentationRules.GetVisibleResistancePactIds(state);
        _selectedUpgradeId = ResolveVisibleId(
            _selectedUpgradeId, visibleUpgradeIds,
            D2Civilization2System.RescueUpgradeId);
        _selectedPactId = ResolveVisibleId(
            _selectedPactId, visiblePactIds,
            D2Civilization2System.HiddenSheltersPactId);

        SetText(availableMembersText, state.membersAvailable.ToString("N0"));
        SetText(fragmentsText, state.controlFragments.ToString("N0"));
        SetText(exhaustedText,
            D2Civilization2System.GetExhaustedMembers(state).ToString("N0"));

        RefreshUpgradeCards(state, visibleUpgradeIds);
        RefreshPactCards(state, visiblePactIds);
        RefreshSelectedPactDetail(state);
        RefreshLegacySummary(state);
    }

    private void RefreshUpgradeCards(
        D2Civilization2State state, string[] visibleIds)
    {
        for (int i = 0; i < D2Civilization2System.UpgradeIds.Length; i++)
        {
            string id = D2Civilization2System.UpgradeIds[i];
            bool visible = Contains(visibleIds, id);
            SetActive(Get(upgradeCardRoots, i), visible);
            int level = D2Civilization2System.GetUpgradeLevel(state, id);
            long cost = D2Civilization2System.GetUpgradeCost(level + 1);
            SetText(Get(upgradeLevelTexts, i),
                "NIVEL " + level + "/" + D2Civilization2System.MaxUpgradeLevel);
            bool maximum = level >= D2Civilization2System.MaxUpgradeLevel;
            SetText(Get(upgradeCostTexts, i), cost.ToString("N0"));
            SetActive(Get(upgradeCostNormalRoots, i), visible && !maximum);
            TMP_Text maximumText = Get(upgradeMaxTexts, i);
            SetActive(maximumText != null ? maximumText.gameObject : null,
                visible && maximum);
            SetInteractable(Get(upgradeCardButtons, i),
                visible && level < D2Civilization2System.MaxUpgradeLevel &&
                state.controlFragments >= cost);
        }
    }

    private void RefreshPactCards(
        D2Civilization2State state, string[] visibleIds)
    {
        for (int i = 0; i < D2Civilization2System.ResistancePactIds.Length; i++)
        {
            string id = D2Civilization2System.ResistancePactIds[i];
            bool visible = Contains(visibleIds, id);
            SetActive(Get(pactCardRoots, i), visible);
            SetActive(Get(pactSelectionRoots, i), visible && id == _selectedPactId);
            SetInteractable(Get(pactCardButtons, i), visible);

            D2ResistancePactState pact =
                D2Civilization2System.GetResistancePact(state, id);
            bool active = pact != null && pact.active;
            SetActive(Get(pactActiveMemberIcons, i), visible && active);
            SetActive(Get(pactInactiveLockIcons, i), visible && !active);
            SetActive(Get(pactWearIcons, i), visible && active);
            SetText(Get(pactStateTexts, i), active ? "ACTIVO" : "INACTIVO");
            SetText(Get(pactMembersOrRequirementTexts, i), active
                ? pact.membersAssigned.ToString("N0") + " MIEMBROS"
                : "REQUIERE " +
                    D2Civilization2System.GetPactInitialRequirement(id).ToString("N0") +
                    "\nMIEMBROS");
            SetText(Get(pactEffectTexts, i), GetPactEffect(id));
            float memberRowOpticalOffset = i == 0 ? 9f : (i == 1 ? 4f : -17f);
            float effectRowOpticalOffset = i == 0 ? 9f : (i == 1 ? 8f : -16f);
            CenterInlinePactRow(
                Get(pactMembersOrRequirementTexts, i),
                active ? Get(pactActiveMemberIcons, i) :
                    Get(pactInactiveLockIcons, i),
                10f, memberRowOpticalOffset);
            CenterInlinePactRow(
                Get(pactEffectTexts, i), Get(pactEffectIcons, i),
                10f, effectRowOpticalOffset);
            SetText(Get(pactWearTexts, i), active
                ? "PRÓXIMO DESGASTE   " +
                    "     " +
                    Mathf.CeilToInt((float)(
                        D2Civilization2System.GetPactWearInterval(id) -
                        pact.wearProgressSeconds)) + " S"
                : string.Empty);
        }
    }

    private void RefreshSelectedPactDetail(D2Civilization2State state)
    {
        D2ResistancePactState pact =
            D2Civilization2System.GetResistancePact(state, _selectedPactId);
        bool active = pact != null && pact.active;
        long assigned = pact != null ? pact.membersAssigned : 0L;
        SetText(pactDetailTitleText,
            "DETALLES DEL PACTO: " +
            D2Civilization2System.GetResistancePactName(
                _selectedPactId).ToUpperInvariant());
        SetText(pactMembersText, assigned.ToString("N0"));
        SetText(pactWearDetailText,
            "1 CADA " + Mathf.RoundToInt((float)
                D2Civilization2System.GetPactWearInterval(_selectedPactId)) + " S");
        string penalties = FormatPenalties(state);
        SetText(penaltiesText, penalties);
        SetActive(noPenaltyIconRoot, penalties == "NINGUNA");

        long requirement =
            D2Civilization2System.GetPactInitialRequirement(_selectedPactId);
        SetActive(activateButton != null ? activateButton.gameObject : null, !active);
        SetActive(cancelButton != null ? cancelButton.gameObject : null, active);
        SetActive(reinforceOneButton != null ? reinforceOneButton.gameObject : null, active);
        SetActive(reinforceTenButton != null ? reinforceTenButton.gameObject : null, active);
        SetActive(pactAdjustRoot, active);
        SetInteractable(activateButton, !active && state.membersAvailable >= requirement);
        SetInteractable(reinforceOneButton, active && state.membersAvailable >= 1L);
        SetInteractable(reinforceTenButton, active && state.membersAvailable >= 1L);
        SetInteractable(cancelButton, active);
    }

    private void RefreshLegacySummary(D2Civilization2State state)
    {
        int level = D2Civilization2System.GetUpgradeLevel(
            state, _selectedUpgradeId);
        long cost = D2Civilization2System.GetUpgradeCost(level + 1);
        SetText(upgradeText,
            D2Civilization2System.GetUpgradeName(_selectedUpgradeId).ToUpperInvariant() +
            " — NIVEL " + level + "/" + D2Civilization2System.MaxUpgradeLevel);
        SetInteractable(upgradeButton,
            level < D2Civilization2System.MaxUpgradeLevel &&
            state.controlFragments >= cost);

        D2ResistancePactState pact = D2Civilization2System.GetResistancePact(
            state, _selectedPactId);
        SetText(pactText,
            D2Civilization2System.GetResistancePactName(
                _selectedPactId).ToUpperInvariant() + " — " +
            (pact != null && pact.active ? "ACTIVO" : "INACTIVO"));
    }

    private void PopulateDropdowns()
    {
        if (upgradeDropdown != null)
        {
            string selected = _upgradeOptions.ResolveOrDefault(
                upgradeDropdown.value, _selectedUpgradeId);
            D2Civilization2State state = CurrentState();
            _upgradeOptions.Rebuild(upgradeDropdown,
                D2Civilization2PresentationRules.GetVisibleUpgradeIds(state),
                D2Civilization2System.GetUpgradeName, selected);
        }

        if (pactDropdown != null)
        {
            string selected = _pactOptions.ResolveOrDefault(
                pactDropdown.value, _selectedPactId);
            D2Civilization2State state = CurrentState();
            _pactOptions.Rebuild(pactDropdown,
                D2Civilization2PresentationRules.GetVisibleResistancePactIds(state),
                D2Civilization2System.GetResistancePactName, selected);
        }
    }

    private void WireUpgradeCards()
    {
        if (upgradeCardButtons == null) return;
        for (int i = 0; i < upgradeCardButtons.Length; i++)
        {
            int captured = i;
            if (upgradeCardButtons[i] != null)
                upgradeCardButtons[i].onClick.AddListener(
                    () => UpgradeCard(captured));
        }
    }

    private void WirePactCards()
    {
        if (pactCardButtons == null) return;
        for (int i = 0; i < pactCardButtons.Length; i++)
        {
            int captured = i;
            if (pactCardButtons[i] != null)
                pactCardButtons[i].onClick.AddListener(
                    () => SelectPactCard(captured));
        }
    }

    private void UpgradeCard(int index)
    {
        if (index < 0 || index >= D2Civilization2System.UpgradeIds.Length) return;
        _selectedUpgradeId = D2Civilization2System.UpgradeIds[index];
        UpgradeSelected();
    }

    private void SelectPactCard(int index)
    {
        if (index < 0 || index >= D2Civilization2System.ResistancePactIds.Length)
            return;
        _selectedPactId = D2Civilization2System.ResistancePactIds[index];
        Refresh();
    }

    private void SelectLegacyUpgrade()
    {
        _selectedUpgradeId = _upgradeOptions.ResolveOrDefault(
            upgradeDropdown != null ? upgradeDropdown.value : 0,
            D2Civilization2System.RescueUpgradeId);
        Refresh();
    }

    private void SelectLegacyPact()
    {
        _selectedPactId = _pactOptions.ResolveOrDefault(
            pactDropdown != null ? pactDropdown.value : 0,
            D2Civilization2System.HiddenSheltersPactId);
        Refresh();
    }

    private void UpgradeSelected()
    {
        D2Civilization2System.TryUpgradeResistance(
            GameState.I, _selectedUpgradeId);
        Refresh();
    }

    private void ActivateSelectedPact()
    {
        D2Civilization2System.TryActivateResistancePact(
            GameState.I, _selectedPactId);
        Refresh();
    }

    private void ReinforceSelectedPact(long amount)
    {
        D2Civilization2System.TryReinforceResistancePact(
            GameState.I, _selectedPactId, amount);
        Refresh();
    }

    private void CancelSelectedPact()
    {
        D2Civilization2System.TryCancelResistancePact(
            GameState.I, _selectedPactId);
        Refresh();
    }

    private static string GetPactEffect(string id)
    {
        if (id == D2Civilization2System.HiddenSheltersPactId)
            return "−20% PÉRDIDAS\nEN REPRESALIAS";
        if (id == D2Civilization2System.SilencedBellsPactId)
            return "−30% DURACIÓN\nDEL DEBILITAMIENTO";
        if (id == D2Civilization2System.KnivesPactId)
            return "+20% EFICACIA\nDE SABOTAJE";
        return string.Empty;
    }

    private static string FormatPenalties(D2Civilization2State state)
    {
        var active = new List<string>();
        AddPenalty(active, "REFUGIOS", state.hiddenSheltersPenaltySeconds);
        AddPenalty(active, "CAMPANAS", state.silencedBellsPenaltySeconds);
        AddPenalty(active, "CUCHILLOS", state.knivesPenaltySeconds);
        return active.Count == 0 ? "NINGUNA" : string.Join("   |   ", active);
    }

    private static void AddPenalty(
        List<string> values, string name, double seconds)
    {
        if (seconds > 0.0)
            values.Add(name + " " + Mathf.CeilToInt((float)seconds) + " S");
    }

    private static string ResolveVisibleId(
        string selected, string[] visible, string fallback)
    {
        if (Contains(visible, selected)) return selected;
        return visible != null && visible.Length > 0 ? visible[0] : fallback;
    }

    private static bool Contains(string[] values, string value)
    {
        if (values == null) return false;
        for (int i = 0; i < values.Length; i++)
            if (values[i] == value) return true;
        return false;
    }

    private static D2Civilization2State CurrentState()
    {
        return GameState.I != null && GameState.I.dimension2 != null
            ? GameState.I.dimension2.civilization2 : null;
    }

    private static T Get<T>(T[] values, int index) where T : class
    {
        return values != null && index >= 0 && index < values.Length
            ? values[index] : null;
    }

    private static void SetText(TMP_Text target, string value)
    {
        if (target != null) target.text = value;
    }

    private static void CenterInlinePactRow(
        TMP_Text text, GameObject icon, float gap, float opticalOffsetX)
    {
        if (text == null || icon == null) return;

        RectTransform textRect = text.rectTransform;
        RectTransform iconRect = icon.GetComponent<RectTransform>();
        RectTransform cardRect = textRect.parent as RectTransform;
        if (iconRect == null || cardRect == null) return;

        float cardWidth = cardRect.rect.width > 0f
            ? cardRect.rect.width : cardRect.sizeDelta.x;
        float iconWidth = iconRect.rect.width > 0f
            ? iconRect.rect.width : iconRect.sizeDelta.x;
        float maximumTextWidth = Mathf.Max(
            1f, cardWidth - iconWidth - gap - 16f);
        float textWidth = Mathf.Min(
            maximumTextWidth,
            Mathf.Ceil(text.GetPreferredValues(
                text.text, maximumTextWidth, textRect.sizeDelta.y).x) + 4f);
        float startX =
            (cardWidth - iconWidth - gap - textWidth) * 0.5f + opticalOffsetX;

        iconRect.anchoredPosition = new Vector2(
            startX, iconRect.anchoredPosition.y);
        textRect.anchoredPosition = new Vector2(
            startX + iconWidth + gap, textRect.anchoredPosition.y);
        textRect.sizeDelta = new Vector2(textWidth, textRect.sizeDelta.y);
    }

    private static void SetInteractable(Button target, bool value)
    {
        if (target != null) target.interactable = value;
    }

    private static void SetActive(GameObject target, bool value)
    {
        if (target != null && target.activeSelf != value)
            target.SetActive(value);
    }
}

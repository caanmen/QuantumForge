using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class D2CivilizationPactsPanelUI : MonoBehaviour
{
    public TMP_Text headerTrustText, headerAcolytesText, headerWaxText, headerBreadText;
    public TMP_Text slotsText, detailTitleText, benefitValueText, commitmentValueText;
    public TMP_Text stateValueText, warningText;
    public Button hospitalityCardButton, openPathCardButton, consecrationCardButton;
    public Button silentVowCardButton, innerDoorCardButton;
    public GameObject hospitalityNeutralOverlay, openPathSelectionOverlay;
    public GameObject consecrationSelectionOverlay, silentVowSelectionOverlay;
    public GameObject innerDoorSelectionOverlay, hospitalitySlotNeutralOverlay;
    public GameObject secondSlotUnlockedOverlay, lockRequirementIconsOverlay;
    public RawImage firstActivePactIcon, secondActivePactIcon;
    public Texture[] pactMedallionTextures;
    public Button activateButton, cancelButton, unlockSecondSlotButton;
    public TMP_Text activateButtonText, unlockSecondSlotButtonText, secondSlotRequirementsText;
    public Button backButton, refugeNavButton, altarsNavButton, pilgrimagesNavButton;
    public Button novitiateNavButton, ritesNavButton, thresholdNavButton;

    // Compatibilidad con escenas anteriores; el constructor canónico los mantiene ocultos/aliasados.
    public TMP_Dropdown pactDropdown;
    public TMP_Text pactStateText, benefitText, commitmentText, resourcesText, lastResultText;

    private float _refreshTimer;
    private string _selectedPactId = D2CivilizationPactSystem.HospitalityId;
    private readonly SafeDropdownOptionMap<string> _pactOptions =
        new SafeDropdownOptionMap<string>(System.StringComparer.Ordinal);
    private static readonly CultureInfo StableCulture = CultureInfo.InvariantCulture;
    private bool _inspectedThisVisit;

    public string SelectedPactId => _selectedPactId;

    private void Awake()
    {
        ConfigureDropdown();
        Add(hospitalityCardButton, () => SelectPact(D2CivilizationPactSystem.HospitalityId));
        Add(openPathCardButton, () => SelectPact(D2CivilizationPactSystem.OpenPathId));
        Add(consecrationCardButton, () => SelectPact(D2CivilizationPactSystem.ConsecrationId));
        Add(silentVowCardButton, () => SelectPact(D2CivilizationPactSystem.SilentVowId));
        Add(innerDoorCardButton, () => SelectPact(D2CivilizationPactSystem.InnerDoorId));
        Add(activateButton, ActivateSelected);
        Add(cancelButton, CancelSelected);
        Add(unlockSecondSlotButton, UnlockSecondSlot);
        Add(backButton, ShowMap);
        Add(refugeNavButton, () => Parent()?.ShowRefugeSection());
        Add(altarsNavButton, () => Parent()?.ShowAltarsSection());
        Add(pilgrimagesNavButton, () => Parent()?.ShowPilgrimagesSection());
        Add(novitiateNavButton, () => Parent()?.ShowNovitiateSection());
        Add(ritesNavButton, () => Parent()?.ShowRitesSection());
        Add(thresholdNavButton, () => Parent()?.ShowVeiledThresholdSection());
    }

    private void OnEnable()
    {
        ConfigureDropdown();
        _inspectedThisVisit = true;
        _refreshTimer = 0f;
        Refresh();
    }

    private void OnDisable()
    {
        if (_inspectedThisVisit && GameState.I?.dimension2 != null)
            PresentationStateUtility.Acknowledge(
                GameState.I.dimension2.presentation,
                PresentationFeatureIds.D2C1Pacts);
        _inspectedThisVisit = false;
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
        D2Civilization1State state = GameState.I?.dimension2?.civilization1;
        string[] visible = D2CivilizationPactSystem.ArePactsUnlocked(state)
            ? D2CivilizationPactSystem.PactIds
            : D2Civilization1PresentationRules.GetVisiblePactIds(state);
        if (!Contains(visible, _selectedPactId))
            _selectedPactId = D2CivilizationPactSystem.HospitalityId;
        _pactOptions.Rebuild(pactDropdown, visible,
            D2CivilizationPactSystem.GetDisplayName, _selectedPactId);
    }

    public void SelectPact(string pactId)
    {
        if (!D2CivilizationPactSystem.IsPactId(pactId)) return;
        D2Civilization1State state = GameState.I?.dimension2?.civilization1;
        string[] visible = D2CivilizationPactSystem.ArePactsUnlocked(state)
            ? D2CivilizationPactSystem.PactIds
            : D2Civilization1PresentationRules.GetVisiblePactIds(state);
        if (!Contains(visible, pactId)) return;
        _selectedPactId = pactId;
        Refresh();
    }

    public void Refresh()
    {
        GameState gameState = GameState.I;
        if (!Dimension2System.CanAccessDimension2(gameState)) return;
        Dimension2System.EnsureState(gameState);
        D2Civilization1State state = gameState.dimension2.civilization1;
        ConfigureDropdown();
        D2CivilizationPactState selected = D2CivilizationPactSystem.GetPact(state, _selectedPactId);
        if (selected == null) return;

        D2AltarState wax = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(state, D2AltarSystem.RitualBreadAltarId);
        int activeCount = D2CivilizationPactSystem.GetActivePactCount(state);
        SetText(headerTrustText, state.trust.ToString("N0", StableCulture) + " / 500");
        SetText(headerAcolytesText, state.acolytesAvailable.ToString("N0", StableCulture));
        SetText(headerWaxText, (wax?.offeringAmount ?? 0.0).ToString("N0", StableCulture));
        SetText(headerBreadText, (bread?.offeringAmount ?? 0.0).ToString("N0", StableCulture));
        SetText(slotsText, "PACTOS ACTIVOS " + activeCount.ToString(StableCulture) + "/" +
            D2CivilizationPactSystem.AdvancedActiveSlots.ToString(StableCulture));
        SetText(detailTitleText, D2CivilizationPactSystem.GetDisplayName(_selectedPactId).ToUpperInvariant() +
            (selected.active ? " – " + (selected.suspended ? "SUSPENDIDO" : "ACTIVO") : ""));
        SetText(benefitValueText,
            D2CivilizationPactSystem.GetBenefitDescription(_selectedPactId).ToUpperInvariant());
        SetText(commitmentValueText,
            D2CivilizationPactSystem.GetCommitmentDescription(_selectedPactId).ToUpperInvariant());
        SetText(stateValueText, selected.active
            ? selected.suspended ? "SUSPENDIDO" : "ACTIVO"
            : "INACTIVO");
        SetText(warningText, "CANCELAR NO DEVUELVE EL COSTE DE ACTIVACIÓN.");
        SetText(activateButtonText, selected.active ? "PACTO ACTIVO" : "ACTIVAR PACTO");
        SetText(unlockSecondSlotButtonText, state.secondCivilizationPactSlotUnlocked
            ? "SEGUNDO ESPACIO\nDESBLOQUEADO"
            : "SEGUNDO ESPACIO");
        SetText(secondSlotRequirementsText, state.secondCivilizationPactSlotUnlocked
            ? ""
            : "400 CONFIANZA\nNOVICIADO 4\n10 ACÓLITOS\n300 CERA\n300 PAN RITUAL");

        bool hospitalitySelected = _selectedPactId == D2CivilizationPactSystem.HospitalityId;
        SetActive(hospitalityNeutralOverlay, !hospitalitySelected);
        SetActive(openPathSelectionOverlay, _selectedPactId == D2CivilizationPactSystem.OpenPathId);
        SetActive(consecrationSelectionOverlay, _selectedPactId == D2CivilizationPactSystem.ConsecrationId);
        SetActive(silentVowSelectionOverlay, _selectedPactId == D2CivilizationPactSystem.SilentVowId);
        SetActive(innerDoorSelectionOverlay, _selectedPactId == D2CivilizationPactSystem.InnerDoorId);

        SetActive(secondSlotUnlockedOverlay, state.secondCivilizationPactSlotUnlocked);
        SetActive(lockRequirementIconsOverlay, !state.secondCivilizationPactSlotUnlocked);
        SetInteractable(activateButton, D2CivilizationPactSystem.CanActivate(gameState, _selectedPactId));
        SetInteractable(cancelButton, selected.active);
        SetInteractable(unlockSecondSlotButton,
            D2CivilizationPactSystem.CanUnlockSecondSlot(gameState));
        RefreshActiveSlots(state);

        pactStateText = detailTitleText;
        benefitText = benefitValueText;
        commitmentText = commitmentValueText;
        resourcesText = headerTrustText;
        lastResultText = warningText;
    }

    private void RefreshActiveSlots(D2Civilization1State state)
    {
        string firstId = null;
        string secondId = null;
        for (int i = 0; i < D2CivilizationPactSystem.PactIds.Length; i++)
        {
            string id = D2CivilizationPactSystem.PactIds[i];
            D2CivilizationPactState pact = D2CivilizationPactSystem.GetPact(state, id);
            if (pact == null || !pact.active) continue;
            if (firstId == null) firstId = id;
            else if (secondId == null) secondId = id;
        }

        bool bakedHospitality = firstId == D2CivilizationPactSystem.HospitalityId;
        SetActive(hospitalitySlotNeutralOverlay, !bakedHospitality);
        SetPactIcon(firstActivePactIcon, bakedHospitality ? null : firstId);
        SetPactIcon(secondActivePactIcon, secondId);
    }

    private void SetPactIcon(RawImage target, string pactId)
    {
        if (target == null) return;
        int index = System.Array.IndexOf(D2CivilizationPactSystem.PactIds, pactId);
        bool valid = index >= 0 && pactMedallionTextures != null && index < pactMedallionTextures.Length &&
            pactMedallionTextures[index] != null;
        target.texture = valid ? pactMedallionTextures[index] : null;
        target.gameObject.SetActive(valid);
    }

    public void ActivateSelected()
    {
        D2CivilizationPactSystem.TryActivate(GameState.I, _selectedPactId);
        RefreshAll();
    }

    public void CancelSelected()
    {
        D2CivilizationPactSystem.TryCancel(GameState.I, _selectedPactId);
        RefreshAll();
    }

    public void UnlockSecondSlot()
    {
        D2CivilizationPactSystem.TryUnlockSecondSlot(GameState.I);
        RefreshAll();
    }

    private void ShowMap()
    {
        Dimension2PanelUI panel = GetComponentInParent<Dimension2PanelUI>(true);
        if (panel != null) panel.ShowMap();
    }

    private D2Civilization1PanelUI Parent() =>
        GetComponentInParent<D2Civilization1PanelUI>(true);
    private void RefreshAll()
    {
        D2Civilization1PanelUI parent = Parent();
        if (parent != null) parent.Refresh(); else Refresh();
    }
    private static bool Contains(string[] ids, string id)
    {
        if (ids == null) return false;
        for (int i = 0; i < ids.Length; i++) if (ids[i] == id) return true;
        return false;
    }
    private static void Add(Button button, UnityEngine.Events.UnityAction action)
    { if (button != null) button.onClick.AddListener(action); }
    private static void SetText(TMP_Text target, string value)
    { if (target != null) target.text = value; }
    private static void SetInteractable(Button button, bool value)
    { if (button != null) button.interactable = value; }
    private static void SetActive(GameObject target, bool value)
    { if (target != null) target.SetActive(value); }
}

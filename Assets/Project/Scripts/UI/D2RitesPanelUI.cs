using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class D2RitesPanelUI : MonoBehaviour
{
    public TMP_Text headerFollowersText, headerAcolytesText, headerWaxText, headerBreadText;
    public TMP_Text slotsText, detailTitleText, descriptionText, currentEffectText, limitText;
    public TMP_Text followersAssignedText, acolytesAssignedText;
    public Button welcomeCardButton, offeringCardButton, pathCardButton, novitiateCardButton, respectCardButton;
    public GameObject welcomeNeutralOverlay, offeringSelectionOverlay, pathSelectionOverlay;
    public GameObject novitiateSelectionOverlay, respectSelectionOverlay;
    public Button assignFollowerOneButton, assignFollowerTenButton, releaseFollowerOneButton;
    public Button assignAcolyteOneButton, assignAcolyteFiveButton, releaseAcolyteOneButton;
    public Button releaseAllButton, unlockThirdSlotButton;
    public TMP_Text unlockThirdSlotButtonText;
    public Button backButton, refugeNavButton, altarsNavButton, pilgrimagesNavButton;
    public Button novitiateNavButton, pactsNavButton, thresholdNavButton;

    // Compatibilidad con escenas anteriores; el constructor canónico lo mantiene oculto.
    public TMP_Dropdown riteDropdown;
    public TMP_Text effectText, resourcesText, assignmentText;

    private float _refreshTimer;
    private string _selectedRiteId = D2RiteSystem.WelcomeId;
    private readonly SafeDropdownOptionMap<string> _riteOptions =
        new SafeDropdownOptionMap<string>(System.StringComparer.Ordinal);
    private static readonly CultureInfo StableCulture = CultureInfo.InvariantCulture;

    public string SelectedRiteId => _selectedRiteId;

    private void Awake()
    {
        ConfigureDropdown();
        Add(welcomeCardButton, () => SelectRite(D2RiteSystem.WelcomeId));
        Add(offeringCardButton, () => SelectRite(D2RiteSystem.OfferingId));
        Add(pathCardButton, () => SelectRite(D2RiteSystem.PathId));
        Add(novitiateCardButton, () => SelectRite(D2RiteSystem.NovitiateId));
        Add(respectCardButton, () => SelectRite(D2RiteSystem.RespectId));
        Add(assignFollowerOneButton, AssignFollowerOne);
        Add(assignFollowerTenButton, AssignFollowerTen);
        Add(releaseFollowerOneButton, ReleaseFollowerOne);
        Add(assignAcolyteOneButton, AssignAcolyteOne);
        Add(assignAcolyteFiveButton, AssignAcolyteFive);
        Add(releaseAcolyteOneButton, ReleaseAcolyteOne);
        Add(releaseAllButton, ReleaseAll);
        Add(unlockThirdSlotButton, UnlockThirdSlot);
        Add(backButton, ShowMap);
        Add(refugeNavButton, () => Parent()?.ShowRefugeSection());
        Add(altarsNavButton, () => Parent()?.ShowAltarsSection());
        Add(pilgrimagesNavButton, () => Parent()?.ShowPilgrimagesSection());
        Add(novitiateNavButton, () => Parent()?.ShowNovitiateSection());
        Add(pactsNavButton, () => Parent()?.ShowPactsSection());
        Add(thresholdNavButton, () => Parent()?.ShowVeiledThresholdSection());
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
        D2Civilization1State state = GameState.I?.dimension2?.civilization1;
        string[] visible = D2Civilization1PresentationRules.GetVisibleRiteIds(state);
        bool selectedVisible = false;
        for (int i = 0; i < visible.Length; i++)
            selectedVisible |= visible[i] == _selectedRiteId;
        if (!selectedVisible) _selectedRiteId = D2RiteSystem.WelcomeId;
        _riteOptions.Rebuild(riteDropdown, visible, D2RiteSystem.GetDisplayName, _selectedRiteId);
    }

    public void SelectRite(string riteId)
    {
        if (!D2RiteSystem.IsRiteId(riteId)) return;
        string[] visible = D2Civilization1PresentationRules.GetVisibleRiteIds(
            GameState.I?.dimension2?.civilization1);
        for (int i = 0; i < visible.Length; i++)
        {
            if (visible[i] != riteId) continue;
            _selectedRiteId = riteId;
            Refresh();
            return;
        }
    }

    public void Refresh()
    {
        GameState gameState = GameState.I;
        if (!Dimension2System.CanAccessDimension2(gameState)) return;
        Dimension2System.EnsureState(gameState);
        D2Civilization1State state = gameState.dimension2.civilization1;
        ConfigureDropdown();
        D2RiteState rite = D2RiteSystem.GetRite(state, _selectedRiteId);
        if (rite == null) return;

        bool unlocked = D2RiteSystem.AreRitesUnlocked(state);
        int activeCount = D2RiteSystem.GetActiveRiteCount(state);
        int slotLimit = D2RiteSystem.GetActiveSlotLimit(state);
        bool selectedActive = rite.followersAssigned > 0L || rite.acolytesAssigned > 0L;
        bool canOccupySlot = selectedActive || activeCount < slotLimit;
        double bonus = D2RiteSystem.GetBonusFraction(state, _selectedRiteId);
        double cap = D2RiteSystem.GetBonusCapFraction(_selectedRiteId);
        D2AltarState wax = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(state, D2AltarSystem.RitualBreadAltarId);
        string sign = _selectedRiteId == D2RiteSystem.NovitiateId ? "−" : "+";

        SetText(headerFollowersText, state.followersAvailable.ToString("N0", StableCulture));
        SetText(headerAcolytesText, state.acolytesAvailable.ToString("N0", StableCulture));
        SetText(headerWaxText, (wax?.offeringAmount ?? 0.0).ToString("N0", StableCulture));
        SetText(headerBreadText, (bread?.offeringAmount ?? 0.0).ToString("N0", StableCulture));
        SetText(slotsText, "RITOS ACTIVOS " + activeCount.ToString(StableCulture) + " / " +
            slotLimit.ToString(StableCulture));
        SetText(detailTitleText, D2RiteSystem.GetDisplayName(_selectedRiteId).ToUpperInvariant());
        SetText(descriptionText, D2RiteSystem.GetEffectDescription(_selectedRiteId)
            .TrimEnd('.').ToUpperInvariant());
        SetText(currentEffectText, sign + (bonus * 100.0).ToString("0.##", StableCulture) + "%");
        SetText(limitText, sign + (cap * 100.0).ToString("0", StableCulture) + "%");
        SetText(followersAssignedText, rite.followersAssigned.ToString("N0", StableCulture));
        SetText(acolytesAssignedText, rite.acolytesAssigned.ToString("N0", StableCulture));

        string[] visible = D2Civilization1PresentationRules.GetVisibleRiteIds(state);
        SetActive(welcomeCardButton, Contains(visible, D2RiteSystem.WelcomeId));
        SetActive(offeringCardButton, Contains(visible, D2RiteSystem.OfferingId));
        SetActive(pathCardButton, Contains(visible, D2RiteSystem.PathId));
        SetActive(novitiateCardButton, Contains(visible, D2RiteSystem.NovitiateId));
        SetActive(respectCardButton, Contains(visible, D2RiteSystem.RespectId));
        bool welcomeSelected = _selectedRiteId == D2RiteSystem.WelcomeId;
        SetActive(welcomeNeutralOverlay, !welcomeSelected);
        SetActive(offeringSelectionOverlay, _selectedRiteId == D2RiteSystem.OfferingId);
        SetActive(pathSelectionOverlay, _selectedRiteId == D2RiteSystem.PathId);
        SetActive(novitiateSelectionOverlay, _selectedRiteId == D2RiteSystem.NovitiateId);
        SetActive(respectSelectionOverlay, _selectedRiteId == D2RiteSystem.RespectId);

        SetInteractable(assignFollowerOneButton, unlocked && canOccupySlot && state.followersAvailable > 0L);
        SetInteractable(assignFollowerTenButton, unlocked && canOccupySlot && state.followersAvailable > 0L);
        SetInteractable(releaseFollowerOneButton, rite.followersAssigned > 0L);
        SetInteractable(assignAcolyteOneButton, unlocked && canOccupySlot && state.acolytesAvailable > 0L);
        SetInteractable(assignAcolyteFiveButton, unlocked && canOccupySlot && state.acolytesAvailable > 0L);
        SetInteractable(releaseAcolyteOneButton, rite.acolytesAssigned > 0L);
        SetInteractable(releaseAllButton, selectedActive);
        SetInteractable(unlockThirdSlotButton, D2RiteSystem.CanUnlockThirdSlot(gameState));
        SetText(unlockThirdSlotButtonText, state.thirdRiteSlotUnlocked
            ? "TERCER ESPACIO DESBLOQUEADO"
            : "DESBLOQUEAR TERCER ESPACIO\n250 CONFIANZA · NOVICIADO 3 · 5 ACÓLITOS · 150 CERA · 150 PAN RITUAL");
        SetActive(unlockThirdSlotButton, true);
    }

    public void AssignFollowerOne() => AssignFollowers(1L);
    public void AssignFollowerTen() => AssignFollowers(10L);
    public void AssignAcolyteOne() => AssignAcolytes(1L);
    public void AssignAcolyteFive() => AssignAcolytes(5L);
    public void ReleaseFollowerOne() { D2RiteSystem.TryReleaseFollowers(GameState.I, _selectedRiteId, 1L); RefreshAll(); }
    public void ReleaseAcolyteOne() { D2RiteSystem.TryReleaseAcolytes(GameState.I, _selectedRiteId, 1L); RefreshAll(); }
    public void ReleaseAll() { D2RiteSystem.TryReleaseAll(GameState.I, _selectedRiteId); RefreshAll(); }
    public void UnlockThirdSlot() { D2RiteSystem.TryUnlockThirdSlot(GameState.I); RefreshAll(); }
    private void AssignFollowers(long amount) { D2RiteSystem.TryAssignFollowers(GameState.I, _selectedRiteId, amount); RefreshAll(); }
    private void AssignAcolytes(long amount) { D2RiteSystem.TryAssignAcolytes(GameState.I, _selectedRiteId, amount); RefreshAll(); }

    private void ShowMap()
    {
        Dimension2PanelUI panel = GetComponentInParent<Dimension2PanelUI>(true);
        if (panel != null) panel.ShowMap();
    }
    private D2Civilization1PanelUI Parent() => GetComponentInParent<D2Civilization1PanelUI>(true);
    private void RefreshAll() { D2Civilization1PanelUI parent = Parent(); if (parent != null) parent.Refresh(); else Refresh(); }
    private static bool Contains(string[] ids, string id) { for (int i = 0; i < ids.Length; i++) if (ids[i] == id) return true; return false; }
    private static void Add(Button button, UnityEngine.Events.UnityAction action) { if (button != null) button.onClick.AddListener(action); }
    private static void SetText(TMP_Text target, string value) { if (target != null) target.text = value; }
    private static void SetInteractable(Button button, bool value) { if (button != null) button.interactable = value; }
    private static void SetActive(Component component, bool value) { if (component != null) component.gameObject.SetActive(value); }
    private static void SetActive(GameObject target, bool value) { if (target != null) target.SetActive(value); }
}

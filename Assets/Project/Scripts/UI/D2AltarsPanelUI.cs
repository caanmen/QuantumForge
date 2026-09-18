using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class D2AltarsPanelUI : MonoBehaviour
{
    public TMP_Dropdown altarDropdown;
    public TMP_Text headerFollowersText;
    public TMP_Text headerWaxText;
    public TMP_Text headerBreadText;
    public TMP_Text selectedCardStateText;
    public TMP_Text detailTitleText;
    public TMP_Text offeringNameText;
    public TMP_Text offeringValueText;
    public TMP_Text productionPerSecondText;
    public TMP_Text productionPerMinuteText;
    public TMP_Text followersAvailableText;
    public TMP_Text followersAssignedText;
    public TMP_Text multiplierText;
    public GameObject waxSelectionOverlay;
    public Button waxCardButton;
    public Button breadCardButton;
    public Button incenseCardButton;
    public Button clothCardButton;
    public Button stoneCardButton;
    public Button backButton;
    public Button refugeNavButton;
    public Button pilgrimagesNavButton;
    public Button novitiateNavButton;
    public Button ritesNavButton;
    public Button pactsNavButton;
    public Button thresholdNavButton;

    // Compatibilidad con escenas anteriores al diseño definitivo.
    public TMP_Text altarStateText;
    public TMP_Text offeringText;
    public TMP_Text productionText;
    public TMP_Text assignmentText;
    public Button assignOneButton;
    public Button assignTenButton;
    public Button assignAllButton;
    public Button releaseOneButton;
    public Button releaseAllButton;

    private float _refreshTimer;
    private readonly SafeDropdownOptionMap<string> _altarOptions =
        new SafeDropdownOptionMap<string>(System.StringComparer.Ordinal);

    private void Awake()
    {
        ConfigureDropdown();
        if (altarDropdown != null) altarDropdown.onValueChanged.AddListener(OnAltarSelectionChanged);
        Add(assignOneButton, AssignOne);
        Add(assignTenButton, AssignTen);
        Add(assignAllButton, AssignAll);
        Add(releaseOneButton, ReleaseOne);
        Add(releaseAllButton, ReleaseAll);
        Add(waxCardButton, () => SelectAltar(D2AltarSystem.WaxAltarId));
        Add(breadCardButton, () => SelectAltar(D2AltarSystem.RitualBreadAltarId));
        Add(incenseCardButton, () => SelectAltar(D2AltarSystem.IncenseAltarId));
        Add(clothCardButton, () => SelectAltar(D2AltarSystem.SacredClothAltarId));
        Add(stoneCardButton, () => SelectAltar(D2AltarSystem.CarvedStoneAltarId));
        Add(backButton, ShowMap);
        Add(refugeNavButton, () => Parent()?.ShowRefugeSection());
        Add(pilgrimagesNavButton, () => Parent()?.ShowPilgrimagesSection());
        Add(novitiateNavButton, () => Parent()?.ShowNovitiateSection());
        Add(ritesNavButton, () => Parent()?.ShowRitesSection());
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
        if (altarDropdown == null) return;
        string selectedId = _altarOptions.ResolveOrDefault(
            altarDropdown.value, D2AltarSystem.WaxAltarId);
        _altarOptions.Rebuild(altarDropdown, D2AltarSystem.AltarIds,
            D2AltarSystem.GetAltarName, selectedId);
    }

    public void SelectAltar(string altarId)
    {
        GameState gameState = GameState.I;
        if (!Dimension2System.CanAccessDimension2(gameState)) return;
        gameState.EnsureDimension2State();
        D2AltarState altar = D2AltarSystem.GetAltar(
            gameState.dimension2.civilization1, altarId);
        int index = _altarOptions.IndexOf(altarId);
        if (altar == null || !altar.unlocked || index < 0 || altarDropdown == null) return;
        altarDropdown.SetValueWithoutNotify(index);
        Refresh();
    }

    public string SelectedAltarId => GetSelectedAltarId();

    public void Refresh()
    {
        GameState gameState = GameState.I;
        if (!Dimension2System.CanAccessDimension2(gameState)) return;
        gameState.EnsureDimension2State();
        D2Civilization1State state = gameState.dimension2.civilization1;
        D2AltarState wax = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(state, D2AltarSystem.RitualBreadAltarId);
        string altarId = GetSelectedAltarId();
        D2AltarState altar = D2AltarSystem.GetAltar(state, altarId);
        if (altar == null) return;

        string altarName = D2AltarSystem.GetAltarName(altarId).ToUpperInvariant();
        string offeringName = D2AltarSystem.GetOfferingName(altarId).ToUpperInvariant();
        double perSecond = D2AltarSystem.GetOfferingPerSecond(state, altar);
        Set(headerFollowersText, state.followersAvailable.ToString("N0", CultureInfo.InvariantCulture));
        Set(headerWaxText, (wax?.offeringAmount ?? 0d).ToString("N0", CultureInfo.InvariantCulture));
        Set(headerBreadText, (bread?.offeringAmount ?? 0d).ToString("N0", CultureInfo.InvariantCulture));
        Set(selectedCardStateText, altar.unlocked ? "DISPONIBLE" : "BLOQUEADO");
        Set(detailTitleText, altarName);
        Set(offeringNameText, "OFRENDA  ·  " + offeringName);
        Set(offeringValueText, altar.offeringAmount.ToString("N2", CultureInfo.InvariantCulture));
        Set(productionPerSecondText, perSecond.ToString("0.000", CultureInfo.InvariantCulture) + " / S");
        Set(productionPerMinuteText, "(" + (perSecond * 60d).ToString("0.00", CultureInfo.InvariantCulture) + " / MIN)");
        Set(followersAvailableText, state.followersAvailable.ToString("N0", CultureInfo.InvariantCulture));
        Set(followersAssignedText, altar.followersAssigned.ToString("N0", CultureInfo.InvariantCulture));
        Set(multiplierText, "×" + D2AltarSystem.GetAssignedFollowerMultiplier(altar).ToString("0.000", CultureInfo.InvariantCulture));
        Set(altarStateText, altarName);
        Set(offeringText, offeringName);
        Set(productionText, perSecond.ToString("0.000", CultureInfo.InvariantCulture));
        Set(assignmentText, altar.followersAssigned.ToString("N0", CultureInfo.InvariantCulture));
        if (waxSelectionOverlay != null) waxSelectionOverlay.SetActive(altarId == D2AltarSystem.WaxAltarId);

        SetInteractable(waxCardButton, wax != null && wax.unlocked);
        SetInteractable(breadCardButton, bread != null && bread.unlocked);
        SetInteractable(incenseCardButton, IsUnlocked(state, D2AltarSystem.IncenseAltarId));
        SetInteractable(clothCardButton, IsUnlocked(state, D2AltarSystem.SacredClothAltarId));
        SetInteractable(stoneCardButton, IsUnlocked(state, D2AltarSystem.CarvedStoneAltarId));
        SetInteractable(assignOneButton, altar.unlocked && state.followersAvailable >= 1L);
        SetInteractable(assignTenButton, altar.unlocked && state.followersAvailable >= 1L);
        SetInteractable(assignAllButton, altar.unlocked && state.followersAvailable >= 1L);
        SetInteractable(releaseOneButton, altar.unlocked && altar.followersAssigned >= 1L);
        SetInteractable(releaseAllButton, altar.unlocked && altar.followersAssigned >= 1L);
    }

    public void AssignOne() { D2AltarSystem.TryAssignFollowers(GameState.I, GetSelectedAltarId(), 1L); RefreshAll(); }
    public void AssignTen() { D2AltarSystem.TryAssignFollowers(GameState.I, GetSelectedAltarId(), 10L); RefreshAll(); }
    public void AssignAll() { D2AltarSystem.TryAssignAllFollowers(GameState.I, GetSelectedAltarId()); RefreshAll(); }
    public void ReleaseOne() { D2AltarSystem.TryReleaseFollowers(GameState.I, GetSelectedAltarId(), 1L); RefreshAll(); }
    public void ReleaseAll() { D2AltarSystem.TryReleaseAllFollowers(GameState.I, GetSelectedAltarId()); RefreshAll(); }

    private void OnAltarSelectionChanged(int _) => Refresh();
    private string GetSelectedAltarId() => _altarOptions.ResolveOrDefault(
        altarDropdown != null ? altarDropdown.value : 0, D2AltarSystem.WaxAltarId);
    private D2Civilization1PanelUI Parent() => GetComponentInParent<D2Civilization1PanelUI>(true);
    private void RefreshAll() { D2Civilization1PanelUI parent = Parent(); if (parent != null) parent.Refresh(); else Refresh(); }
    private void ShowMap() { Dimension2PanelUI panel = GetComponentInParent<Dimension2PanelUI>(true); if (panel != null) panel.ShowMap(); }
    private static bool IsUnlocked(D2Civilization1State state, string id) { D2AltarState altar = D2AltarSystem.GetAltar(state, id); return altar != null && altar.unlocked; }
    private static void Set(TMP_Text text, string value) { if (text != null) text.text = value; }
    private static void SetInteractable(Button button, bool value) { if (button != null) button.interactable = value; }
    private static void Add(Button button, UnityEngine.Events.UnityAction action) { if (button != null) button.onClick.AddListener(action); }
}

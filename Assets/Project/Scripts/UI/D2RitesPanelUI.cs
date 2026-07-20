using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D2RitesPanelUI : MonoBehaviour
{
    public TMP_Dropdown riteDropdown;
    public TMP_Text slotsText;
    public TMP_Text effectText;
    public TMP_Text resourcesText;
    public TMP_Text assignmentText;
    public Button assignFollowerOneButton;
    public Button assignFollowerTenButton;
    public Button releaseFollowerOneButton;
    public Button assignAcolyteOneButton;
    public Button assignAcolyteFiveButton;
    public Button releaseAcolyteOneButton;
    public Button releaseAllButton;
    public Button unlockThirdSlotButton;
    public TMP_Text unlockThirdSlotButtonText;

    private float _refreshTimer;

    private void Awake()
    {
        ConfigureDropdown();
        if (riteDropdown != null)
            riteDropdown.onValueChanged.AddListener(OnRiteSelectionChanged);
        if (assignFollowerOneButton != null)
            assignFollowerOneButton.onClick.AddListener(AssignFollowerOne);
        if (assignFollowerTenButton != null)
            assignFollowerTenButton.onClick.AddListener(AssignFollowerTen);
        if (releaseFollowerOneButton != null)
            releaseFollowerOneButton.onClick.AddListener(ReleaseFollowerOne);
        if (assignAcolyteOneButton != null)
            assignAcolyteOneButton.onClick.AddListener(AssignAcolyteOne);
        if (assignAcolyteFiveButton != null)
            assignAcolyteFiveButton.onClick.AddListener(AssignAcolyteFive);
        if (releaseAcolyteOneButton != null)
            releaseAcolyteOneButton.onClick.AddListener(ReleaseAcolyteOne);
        if (releaseAllButton != null)
            releaseAllButton.onClick.AddListener(ReleaseAll);
        if (unlockThirdSlotButton != null)
            unlockThirdSlotButton.onClick.AddListener(UnlockThirdSlot);
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
        if (_refreshTimer > 0f)
            return;

        _refreshTimer = 0.2f;
        Refresh();
    }

    public void ConfigureDropdown()
    {
        if (riteDropdown == null)
            return;

        int selected = Mathf.Clamp(riteDropdown.value, 0, D2RiteSystem.RiteIds.Length - 1);
        var options = new List<TMP_Dropdown.OptionData>();
        foreach (string riteId in D2RiteSystem.RiteIds)
            options.Add(new TMP_Dropdown.OptionData(D2RiteSystem.GetDisplayName(riteId)));

        riteDropdown.ClearOptions();
        riteDropdown.AddOptions(options);
        riteDropdown.SetValueWithoutNotify(selected);
        riteDropdown.RefreshShownValue();
    }

    public void Refresh()
    {
        GameState gameState = GameState.I;
        if (!Dimension2System.CanAccessDimension2(gameState))
            return;

        Dimension2System.EnsureState(gameState);
        D2Civilization1State state = gameState.dimension2.civilization1;
        string riteId = GetSelectedRiteId();
        D2RiteState rite = D2RiteSystem.GetRite(state, riteId);
        if (rite == null)
            return;

        bool unlocked = D2RiteSystem.AreRitesUnlocked(state);
        int activeCount = D2RiteSystem.GetActiveRiteCount(state);
        int slotLimit = D2RiteSystem.GetActiveSlotLimit(state);
        bool selectedActive = rite.followersAssigned > 0L || rite.acolytesAssigned > 0L;
        bool canOccupySlot = selectedActive || activeCount < slotLimit;
        double bonus = D2RiteSystem.GetBonusFraction(state, riteId);
        double cap = D2RiteSystem.GetBonusCapFraction(riteId);
        D2AltarState wax = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(
            state,
            D2AltarSystem.RitualBreadAltarId
        );

        SetText(
            slotsText,
            unlocked
                ? "RITOS ACTIVOS: " + activeCount + "/" + slotLimit +
                  (state.thirdRiteSlotUnlocked ? " — TERCER ESPACIO DESBLOQUEADO" : "")
                : "RITOS BLOQUEADOS — Forma tu primer Acólito para habilitarlos."
        );
        string sign = riteId == D2RiteSystem.PathId || riteId == D2RiteSystem.NovitiateId
            ? "−"
            : "+";
        SetText(
            effectText,
            D2RiteSystem.GetDisplayName(riteId) + " — " +
            D2RiteSystem.GetEffectDescription(riteId) + "\nEfecto actual: " + sign +
            (bonus * 100.0).ToString("0.##") + "%   |   Límite: " + sign +
            (cap * 100.0).ToString("0") + "%"
        );
        SetText(
            resourcesText,
            "Disponibles — Seguidores: " + state.followersAvailable.ToString("N0") +
            "   |   Acólitos: " + state.acolytesAvailable.ToString("N0") +
            "   |   Cera: " + (wax?.offeringAmount ?? 0.0).ToString("N2") +
            "   |   Pan: " + (bread?.offeringAmount ?? 0.0).ToString("N2")
        );
        SetText(
            assignmentText,
            "Asignados a este Rito — Seguidores: " +
            rite.followersAssigned.ToString("N0") + "   |   Acólitos: " +
            rite.acolytesAssigned.ToString("N0") +
            (canOccupySlot ? "" : "   |   Libera otro Rito para usarlo")
        );

        SetInteractable(assignFollowerOneButton,
            unlocked && canOccupySlot && state.followersAvailable > 0L);
        SetInteractable(assignFollowerTenButton,
            unlocked && canOccupySlot && state.followersAvailable > 0L);
        SetInteractable(releaseFollowerOneButton, rite.followersAssigned > 0L);
        SetInteractable(assignAcolyteOneButton,
            unlocked && canOccupySlot && state.acolytesAvailable > 0L);
        SetInteractable(assignAcolyteFiveButton,
            unlocked && canOccupySlot && state.acolytesAvailable > 0L);
        SetInteractable(releaseAcolyteOneButton, rite.acolytesAssigned > 0L);
        SetInteractable(releaseAllButton, selectedActive);
        SetInteractable(unlockThirdSlotButton, D2RiteSystem.CanUnlockThirdSlot(gameState));
        SetText(
            unlockThirdSlotButtonText,
            state.thirdRiteSlotUnlocked
                ? "TERCER ESPACIO DESBLOQUEADO"
                : "DESBLOQUEAR TERCER ESPACIO\n250 Confianza · Noviciado 3 · " +
                  "5 Acólitos · 150 Cera · 150 Pan"
        );
    }

    public void AssignFollowerOne() => AssignFollowers(1L);
    public void AssignFollowerTen() => AssignFollowers(10L);
    public void AssignAcolyteOne() => AssignAcolytes(1L);
    public void AssignAcolyteFive() => AssignAcolytes(5L);

    public void ReleaseFollowerOne()
    {
        D2RiteSystem.TryReleaseFollowers(GameState.I, GetSelectedRiteId(), 1L);
        RefreshAll();
    }

    public void ReleaseAcolyteOne()
    {
        D2RiteSystem.TryReleaseAcolytes(GameState.I, GetSelectedRiteId(), 1L);
        RefreshAll();
    }

    public void ReleaseAll()
    {
        D2RiteSystem.TryReleaseAll(GameState.I, GetSelectedRiteId());
        RefreshAll();
    }

    public void UnlockThirdSlot()
    {
        D2RiteSystem.TryUnlockThirdSlot(GameState.I);
        RefreshAll();
    }

    private void AssignFollowers(long amount)
    {
        D2RiteSystem.TryAssignFollowers(GameState.I, GetSelectedRiteId(), amount);
        RefreshAll();
    }

    private void AssignAcolytes(long amount)
    {
        D2RiteSystem.TryAssignAcolytes(GameState.I, GetSelectedRiteId(), amount);
        RefreshAll();
    }

    private void OnRiteSelectionChanged(int _)
    {
        Refresh();
    }

    private string GetSelectedRiteId()
    {
        int index = riteDropdown != null ? riteDropdown.value : 0;
        index = Mathf.Clamp(index, 0, D2RiteSystem.RiteIds.Length - 1);
        return D2RiteSystem.RiteIds[index];
    }

    private void RefreshAll()
    {
        D2Civilization1PanelUI parent = GetComponentInParent<D2Civilization1PanelUI>(true);
        if (parent != null)
            parent.Refresh();
        else
            Refresh();
    }

    private static void SetText(TMP_Text target, string value)
    {
        if (target != null)
            target.text = value;
    }

    private static void SetInteractable(Button button, bool interactable)
    {
        if (button != null)
            button.interactable = interactable;
    }
}

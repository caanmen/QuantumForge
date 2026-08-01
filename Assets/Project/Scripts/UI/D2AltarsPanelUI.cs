using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D2AltarsPanelUI : MonoBehaviour
{
    public TMP_Dropdown altarDropdown;
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

        if (altarDropdown != null)
            altarDropdown.onValueChanged.AddListener(OnAltarSelectionChanged);

        if (assignOneButton != null)
            assignOneButton.onClick.AddListener(AssignOne);

        if (assignTenButton != null)
            assignTenButton.onClick.AddListener(AssignTen);

        if (assignAllButton != null)
            assignAllButton.onClick.AddListener(AssignAll);

        if (releaseOneButton != null)
            releaseOneButton.onClick.AddListener(ReleaseOne);

        if (releaseAllButton != null)
            releaseAllButton.onClick.AddListener(ReleaseAll);
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
        if (altarDropdown == null)
            return;

        string selectedId = _altarOptions.ResolveOrDefault(
            altarDropdown.value, D2AltarSystem.WaxAltarId);
        D2Civilization1State state = GameState.I != null &&
            GameState.I.dimension2 != null
            ? GameState.I.dimension2.civilization1 : null;
        _altarOptions.Rebuild(altarDropdown,
            D2Civilization1PresentationRules.GetVisibleAltarIds(state),
            D2AltarSystem.GetAltarName, selectedId);
    }

    public void Refresh()
    {
        GameState gameState = GameState.I;
        if (!Dimension2System.CanAccessDimension2(gameState))
            return;

        gameState.EnsureDimension2State();
        D2Civilization1State civilization1 = gameState.dimension2.civilization1;
        string altarId = GetSelectedAltarId();
        D2AltarState altar = D2AltarSystem.GetAltar(civilization1, altarId);
        if (altar == null)
            return;

        bool unlocked = altar.unlocked;
        if (altarStateText != null)
        {
            altarStateText.text = unlocked
                ? D2AltarSystem.GetAltarName(altarId) + " — DISPONIBLE" +
                  (_altarOptions.VisibleOptionIds.Count == 2
                      ? "\nOtros espacios del Santuario permanecen inertes." : "")
                : D2AltarSystem.GetAltarName(altarId) +
                  " — BLOQUEADO (requisito pendiente de su bloque de diseño)";
        }

        if (offeringText != null)
        {
            offeringText.text =
                D2AltarSystem.GetOfferingName(altarId) + ": " +
                altar.offeringAmount.ToString("N2");
        }

        if (productionText != null)
        {
            double perSecond = D2AltarSystem.GetOfferingPerSecond(civilization1, altar);
            productionText.text = unlocked
                ? "Producción: " + perSecond.ToString("0.000") + "/s" +
                  "   (" + (perSecond * 60.0).ToString("0.00") + "/min)"
                : "Producción detenida mientras el Altar esté bloqueado";
        }

        if (assignmentText != null)
        {
            assignmentText.text =
                "Seguidores disponibles: " + civilization1.followersAvailable.ToString("N0") +
                "   |   Asignados al Altar: " + altar.followersAssigned.ToString("N0") +
                "   |   Multiplicador: ×" +
                D2AltarSystem.GetAssignedFollowerMultiplier(altar).ToString("0.000");
        }

        SetInteractable(assignOneButton, unlocked && civilization1.followersAvailable >= 1L);
        SetInteractable(assignTenButton, unlocked && civilization1.followersAvailable >= 1L);
        SetInteractable(assignAllButton, unlocked && civilization1.followersAvailable >= 1L);
        SetInteractable(releaseOneButton, unlocked && altar.followersAssigned >= 1L);
        SetInteractable(releaseAllButton, unlocked && altar.followersAssigned >= 1L);
        bool learned = altar.followersAssigned > 0L ||
            altar.totalOfferingProduced > 0.0;
        SetActive(assignTenButton, learned);
        SetActive(assignAllButton, learned);
        SetActive(releaseAllButton, learned);
    }

    public void AssignOne()
    {
        D2AltarSystem.TryAssignFollowers(GameState.I, GetSelectedAltarId(), 1L);
        RefreshAllCivilization1UI();
    }

    public void AssignTen()
    {
        D2AltarSystem.TryAssignFollowers(GameState.I, GetSelectedAltarId(), 10L);
        RefreshAllCivilization1UI();
    }

    public void AssignAll()
    {
        D2AltarSystem.TryAssignAllFollowers(GameState.I, GetSelectedAltarId());
        RefreshAllCivilization1UI();
    }

    public void ReleaseOne()
    {
        D2AltarSystem.TryReleaseFollowers(GameState.I, GetSelectedAltarId(), 1L);
        RefreshAllCivilization1UI();
    }

    public void ReleaseAll()
    {
        D2AltarSystem.TryReleaseAllFollowers(GameState.I, GetSelectedAltarId());
        RefreshAllCivilization1UI();
    }

    private void OnAltarSelectionChanged(int _)
    {
        Refresh();
    }

    private string GetSelectedAltarId()
    {
        int index = altarDropdown != null ? altarDropdown.value : 0;
        return _altarOptions.ResolveOrDefault(index, D2AltarSystem.WaxAltarId);
    }

    private void RefreshAllCivilization1UI()
    {
        D2Civilization1PanelUI civilization1UI =
            GetComponentInParent<D2Civilization1PanelUI>(true);
        if (civilization1UI != null)
            civilization1UI.Refresh();
        else
            Refresh();
    }

    private static void SetInteractable(Button button, bool interactable)
    {
        if (button != null)
            button.interactable = interactable;
    }

    private static void SetActive(Component component, bool active)
    {
        if (component != null) component.gameObject.SetActive(active);
    }
}

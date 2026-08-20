using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class VerticalNavigationUI : MonoBehaviour
{
    public enum PrimarySection
    {
        Generation,
        Upgrades,
        Research,
        Settings
    }

    public TabsUI tabs;
    public VerticalSafeAreaLayout safeAreaLayout;
    public VerticalUiTheme theme;
    public RectTransform primaryNavigationRoot;
    public RectTransform secondaryNavigationRoot;
    public ScrollRect secondaryScroll;

    [Header("Navegacion principal")]
    public Button generationButton;
    public Button upgradesButton;
    public Button researchButton;
    public Button settingsButton;
    public Button qaButton;

    [Header("Sistemas progresivos")]
    public Button room2Button;
    public Button dimension1Button;
    public Button dimension2Button;
    public Button dimension3Button;
    public Button prestigeButton;

    private const float RefreshInterval = 0.35f;
    private float nextRefreshTime;
    private int lastLocalizationRevision = -1;
    private PrimarySection selectedPrimary = PrimarySection.Generation;
    private Button selectedSecondary;
    private readonly HashSet<int> navigationSuppressionOwners = new HashSet<int>();
    private bool legacyNavigationSuppressed;
    private bool navigationVisibilityApplyPending;
    private int navigationVisibilityApplyFrame;
    private bool commandCenterDrawerMode;
    private bool commandCenterDrawerExpanded;

    public bool CommandCenterDrawerExpanded => commandCenterDrawerExpanded;
    public bool NavigationSuppressed =>
        legacyNavigationSuppressed || navigationSuppressionOwners.Count > 0;

    private void Awake()
    {
        RefreshAvailability();
        RefreshLabels();
        RefreshVisuals();
    }

    private void OnEnable()
    {
        RefreshAvailability();
        RefreshLabels();
        RefreshVisuals();
    }

    private void Update()
    {
        if (navigationVisibilityApplyPending &&
            Time.frameCount >= navigationVisibilityApplyFrame)
        {
            navigationVisibilityApplyPending = false;
            RefreshAvailability();
        }

        if (Time.unscaledTime < nextRefreshTime)
            return;
        nextRefreshTime = Time.unscaledTime + RefreshInterval;
        RefreshAvailability();

        int revision = LocalizationManager.I != null
            ? LocalizationManager.I.Revision
            : -1;
        if (revision != lastLocalizationRevision)
            RefreshLabels();
    }

    public void SetPrimarySelection(PrimarySection section)
    {
        selectedPrimary = section;
        selectedSecondary = null;
        RefreshVisuals();
    }

    public void SetSecondarySelection(Button button)
    {
        selectedSecondary = button;
        RefreshVisuals();
    }

    [ContextMenu("Refresh vertical navigation")]
    public void RefreshAvailability()
    {
        // Una solicitud iniciada por un Button se materializa como mínimo en el
        // fotograma siguiente. Así UGUI termina primero su ciclo de PointerClick.
        if (navigationVisibilityApplyPending &&
            Time.frameCount < navigationVisibilityApplyFrame)
        {
            return;
        }

        // Full-screen feature panels own navigation while open. Without this
        // guard the 0.35 s refresh reactivated SecondaryNavigationSlot, while
        // the feature panel disabled it again on the next frame. Safe-area
        // layout then alternated ContentSlot by 120 px and made the whole view jump.
        if (NavigationSuppressed)
        {
            bool hiddenChanged = SetActive(primaryNavigationRoot, false) |
                SetActive(secondaryNavigationRoot, false);
            if (hiddenChanged && safeAreaLayout != null)
                safeAreaLayout.ApplyLayout();
            return;
        }

        EvaluateVisibility(GameState.I, MachineManager.I,
            out bool room2,
            out bool dimension1,
            out bool dimension2,
            out bool dimension3,
            out bool prestige);

        bool changed = SetActive(primaryNavigationRoot, !commandCenterDrawerMode) |
            SetActive(researchButton, false) |
            SetActive(room2Button, room2) |
            SetActive(dimension1Button, dimension1) |
            SetActive(dimension2Button, dimension2) |
            SetActive(dimension3Button, dimension3) |
            SetActive(prestigeButton, prestige);

        bool anySecondary = room2 || dimension1 || dimension2 ||
            dimension3 || prestige;
        bool showSecondary = anySecondary &&
            (!commandCenterDrawerMode || commandCenterDrawerExpanded);
        if (secondaryNavigationRoot != null &&
            secondaryNavigationRoot.gameObject.activeSelf != showSecondary)
        {
            secondaryNavigationRoot.gameObject.SetActive(showSecondary);
            changed = true;
        }

        SetActive(qaButton, QaRuntimeService.IsAvailable);
        if (changed && safeAreaLayout != null)
            safeAreaLayout.ApplyLayout();
        ApplyCommandCenterDrawerPosition();
        RefreshPrestigeLabel();
    }

    public void SetCommandCenterDrawerMode(bool active)
    {
        if (commandCenterDrawerMode == active)
            return;

        commandCenterDrawerMode = active;
        commandCenterDrawerExpanded = false;
        RefreshAvailability();
        if (safeAreaLayout != null)
            safeAreaLayout.ApplyLayout();
        ApplyCommandCenterDrawerPosition();
    }

    public void SetCommandCenterDrawerExpanded(bool expanded)
    {
        if (!commandCenterDrawerMode || commandCenterDrawerExpanded == expanded)
            return;

        commandCenterDrawerExpanded = expanded;
        RefreshAvailability();
        if (safeAreaLayout != null)
            safeAreaLayout.ApplyLayout();
        ApplyCommandCenterDrawerPosition();
    }

    public void SetNavigationSuppressed(bool suppressed)
    {
        bool previous = NavigationSuppressed;
        if (legacyNavigationSuppressed == suppressed)
            return;

        legacyNavigationSuppressed = suppressed;
        ScheduleNavigationVisibilityApply(previous);
    }

    public void SetNavigationSuppressed(bool suppressed, Object owner)
    {
        if (owner == null)
        {
            SetNavigationSuppressed(suppressed);
            return;
        }

        bool previous = NavigationSuppressed;
        int ownerId = owner.GetInstanceID();
        bool changed = suppressed
            ? navigationSuppressionOwners.Add(ownerId)
            : navigationSuppressionOwners.Remove(ownerId);
        if (!changed)
            return;

        ScheduleNavigationVisibilityApply(previous);
    }

    private void ScheduleNavigationVisibilityApply(bool previous)
    {
        if (previous == NavigationSuppressed)
            return;

        navigationVisibilityApplyPending = true;
        navigationVisibilityApplyFrame = Time.frameCount + 1;
    }

    public static void EvaluateVisibility(
        GameState state,
        MachineManager machine,
        out bool room2,
        out bool dimension1,
        out bool dimension2,
        out bool dimension3,
        out bool prestige)
    {
        room2 = state != null && state.experimentalChamberUnlocked;
        dimension1 = state != null && state.dimension01Unlocked;
        dimension2 = Dimension2System.CanAccessDimension2(state);
        dimension3 = Dimension3System.CanAccessDimension3(state);
        prestige = TabsUI.ShouldShowPrestige1Button(state, machine);
    }

    private void RefreshLabels()
    {
        lastLocalizationRevision = LocalizationManager.I != null
            ? LocalizationManager.I.Revision
            : -1;
        RefreshPrestigeLabel();
    }

    private void RefreshPrestigeLabel()
    {
        if (prestigeButton == null)
            return;
        TMP_Text label = prestigeButton.GetComponentInChildren<TMP_Text>(true);
        if (label == null)
            return;

        bool convergence = GameState.I != null &&
            ConvergenceCircuitSystem.IsConvergenceUnlocked(GameState.I);
        string key = convergence ? "nav.convergence" : "nav.prestige";
        label.SetText(LocalizationManager.I != null
            ? LocalizationManager.I.T(key)
            : convergence ? "CONVERGENCIA" : "PRESTIGIO");
    }

    private void RefreshVisuals()
    {
        StylePrimary(generationButton,
            selectedPrimary == PrimarySection.Generation);
        StylePrimary(upgradesButton,
            selectedPrimary == PrimarySection.Upgrades);
        StylePrimary(researchButton,
            selectedPrimary == PrimarySection.Research);
        StylePrimary(settingsButton,
            selectedPrimary == PrimarySection.Settings);
        StyleQa(qaButton);

        StyleSecondary(room2Button, selectedSecondary == room2Button);
        StyleSecondary(dimension1Button, selectedSecondary == dimension1Button);
        StyleSecondary(dimension2Button, selectedSecondary == dimension2Button);
        StyleSecondary(dimension3Button, selectedSecondary == dimension3Button);
        StyleSecondary(prestigeButton, selectedSecondary == prestigeButton);
    }

    private void StylePrimary(Button button, bool selected)
    {
        if (button == null || theme == null)
            return;
        Image background = button.targetGraphic as Image;
        if (background != null)
        {
            background.sprite = selected
                ? theme.selectedButtonFrame
                : theme.buttonFrame;
            background.type = Image.Type.Sliced;
            background.color = Color.white;
        }
        TintContents(button, selected ? theme.energy : theme.primaryText);
    }

    private void StyleSecondary(Button button, bool selected)
    {
        if (button == null || theme == null)
            return;
        Image background = button.targetGraphic as Image;
        if (background != null)
        {
            background.sprite = selected
                ? theme.selectedButtonFrame
                : theme.buttonFrame;
            background.type = Image.Type.Sliced;
            background.color = Color.white;
        }
        TintContents(button, selected ? theme.energy : theme.secondaryText);
    }

    private void StyleQa(Button button)
    {
        if (button == null || theme == null)
            return;
        Image background = button.targetGraphic as Image;
        if (background != null)
        {
            background.sprite = theme.buttonFrame;
            background.type = Image.Type.Sliced;
            background.color = Color.white;
        }
        TintContents(button, theme.triangle);
    }

    private static void TintContents(Button button, Color color)
    {
        foreach (TMP_Text text in button.GetComponentsInChildren<TMP_Text>(true))
            text.color = color;
        foreach (Image image in button.GetComponentsInChildren<Image>(true))
            if (image != button.targetGraphic)
                image.color = color;
    }

    private static bool SetActive(Button button, bool active)
    {
        if (button == null || button.gameObject.activeSelf == active)
            return false;
        button.gameObject.SetActive(active);
        return true;
    }

    private static bool SetActive(RectTransform root, bool active)
    {
        if (root == null || root.gameObject.activeSelf == active)
            return false;
        root.gameObject.SetActive(active);
        return true;
    }

    private void ApplyCommandCenterDrawerPosition()
    {
        if (!commandCenterDrawerMode || !commandCenterDrawerExpanded ||
            secondaryNavigationRoot == null)
        {
            return;
        }

        float bottom = safeAreaLayout != null
            ? safeAreaLayout.verticalGap
            : 16f;
        secondaryNavigationRoot.anchoredPosition = new Vector2(
            secondaryNavigationRoot.anchoredPosition.x, bottom);
    }
}

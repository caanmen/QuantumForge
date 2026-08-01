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
        EvaluateVisibility(GameState.I, MachineManager.I,
            out bool room2,
            out bool dimension1,
            out bool dimension2,
            out bool dimension3,
            out bool prestige);

        bool changed = SetActive(room2Button, room2) |
            SetActive(dimension1Button, dimension1) |
            SetActive(dimension2Button, dimension2) |
            SetActive(dimension3Button, dimension3) |
            SetActive(prestigeButton, prestige);

        bool anySecondary = room2 || dimension1 || dimension2 ||
            dimension3 || prestige;
        if (secondaryNavigationRoot != null &&
            secondaryNavigationRoot.gameObject.activeSelf != anySecondary)
        {
            secondaryNavigationRoot.gameObject.SetActive(anySecondary);
            changed = true;
        }

        SetActive(qaButton, QaRuntimeService.IsAvailable);
        if (changed && safeAreaLayout != null)
            safeAreaLayout.ApplyLayout();
        RefreshPrestigeLabel();
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
}

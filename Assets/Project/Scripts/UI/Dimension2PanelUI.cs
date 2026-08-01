using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Dimension2PanelUI : MonoBehaviour
{
    [Header("Vistas")]
    public GameObject firstEntryRoot;
    public GameObject mapRoot;
    public GameObject civilization1Root;
    public GameObject civilization2Root;
    public GameObject civilization3Root;
    public Button closeDimension2Button;
    public Button contextualHelpButton;
    public GameObject helpRoot;
    public TMP_Text helpTitleText;
    public TMP_Text helpBodyText;
    public Button closeHelpButton;

    [Header("Entrada")]
    public TMP_Text firstEntryTitleText;
    public TMP_Text firstEntryDescriptionText;
    public Button continueFirstEntryButton;

    [Header("Mapa")]
    public TMP_Text mapStatusText;
    public Button civilization1Button;
    public Button civilization2Button;
    public Button civilization3Button;
    public TMP_Text civilization1StateText;
    public TMP_Text civilization2StateText;
    public TMP_Text civilization3StateText;

    [Header("Civilización 1")]
    public TMP_Text civilization1PlaceholderText;
    public Button backToMapButton;
    public D2Civilization1PanelUI civilization1PanelUI;

    [Header("Civilización 2")]
    public D2Civilization2PanelUI civilization2PanelUI;

    [Header("Civilización 3")]
    public D2Civilization3PanelUI civilization3PanelUI;

    private void Awake()
    {
        if (continueFirstEntryButton != null)
            continueFirstEntryButton.onClick.AddListener(ContinueFirstEntry);

        if (civilization1Button != null)
            civilization1Button.onClick.AddListener(OpenCivilization1);

        if (civilization2Button != null)
            civilization2Button.onClick.AddListener(OpenCivilization2);

        if (civilization3Button != null)
            civilization3Button.onClick.AddListener(OpenCivilization3);

        if (backToMapButton != null)
            backToMapButton.onClick.AddListener(ShowMap);

        if (closeDimension2Button != null)
            closeDimension2Button.onClick.AddListener(CloseDimension2);
        if (contextualHelpButton != null)
            contextualHelpButton.onClick.AddListener(OpenContextualHelp);
        if (closeHelpButton != null)
            closeHelpButton.onClick.AddListener(CloseContextualHelp);
    }

    private void OnEnable()
    {
        OpenFromTab();
    }

    public void OpenFromTab()
    {
        if (!Dimension2System.CanAccessDimension2(GameState.I))
            return;

        GameState.I.EnsureDimension2State();
        OpenResolvedScreen(D2PresentationRouter.ResolveInitialScreen(GameState.I));
    }

    public void ShowFirstEntry()
    {
        D2PresentationRouter.RememberScreen(GameState.I, D2PresentationRouter.FirstEntryScreenId);
        SetView(firstEntry: true, map: false, civilization1: false, civilization2: false, civilization3: false);
        Refresh();
    }

    public void ContinueFirstEntry()
    {
        Dimension2System.MarkFirstEntrySeen(GameState.I);
        GameState.I.dimension2.presentation.onboardingStage = Mathf.Max(
            1, GameState.I.dimension2.presentation.onboardingStage);
        PresentationStateUtility.Acknowledge(
            GameState.I.dimension2.presentation, PresentationFeatureIds.D2Map);
        ShowMap();
    }

    public void ShowMap()
    {
        if (!Dimension2System.CanAccessDimension2(GameState.I))
            return;

        D2PresentationRouter.RememberScreen(GameState.I, PresentationFeatureIds.D2Map);
        SetView(firstEntry: false, map: true, civilization1: false, civilization2: false, civilization3: false);
        Refresh();
    }

    public void OpenCivilization1()
    {
        OpenTerritory(Dimension2System.Civilization1TerritoryId);
    }

    public void OpenCivilization2()
    {
        OpenTerritory(Dimension2System.Civilization2TerritoryId);
    }

    public void OpenCivilization3()
    {
        OpenTerritory(Dimension2System.Civilization3TerritoryId);
    }

    public void CloseDimension2()
    {
        if (TabsUI.Instance != null)
            TabsUI.Instance.ShowGeneracion();
    }

    public void Refresh()
    {
        GameState state = GameState.I;
        if (!Dimension2System.CanAccessDimension2(state))
            return;

        state.EnsureDimension2State();
        SetButtonLabel(contextualHelpButton,
            PresentationTextCatalog.Current("help.review"));
        if (helpRoot != null && helpRoot.activeSelf)
        {
            SetText(helpTitleText,
                PresentationTextCatalog.Current("help.d2.title"));
            SetText(helpBodyText,
                PresentationTextCatalog.Current("help.d2.body"));
            SetButtonLabel(closeHelpButton,
                PresentationTextCatalog.Current("help.close"));
        }

        bool civilization1Unlocked = Dimension2System.IsTerritoryUnlocked(
            state,
            Dimension2System.Civilization1TerritoryId
        );
        bool civilization2Unlocked = Dimension2System.IsTerritoryUnlocked(
            state,
            Dimension2System.Civilization2TerritoryId
        );
        bool civilization3Unlocked = Dimension2System.IsTerritoryUnlocked(
            state,
            Dimension2System.Civilization3TerritoryId
        );

        SetButtonState(civilization1Button, civilization1Unlocked);
        SetButtonState(civilization2Button, civilization2Unlocked);
        SetButtonState(civilization3Button, civilization3Unlocked);

        SetText(firstEntryTitleText, "DIMENSIÓN 2 · PACTOS");
        SetText(firstEntryDescriptionText,
            "La influencia se gana acogiendo, guiando y cumpliendo compromisos.");
        SetButtonLabel(continueFirstEntryButton, "ABRIR MAPA");
        SetButtonLabel(civilization1Button, "ENTRAR AL SANTUARIO");
        SetButtonLabel(civilization2Button, civilization2Unlocked
            ? "CIVILIZACIÓN 2\nTERRITORIO LOCALIZADO"
            : "TERRITORIO PRÓXIMO");
        if (civilization3Button != null)
            civilization3Button.gameObject.SetActive(civilization3Unlocked);

        SetText(civilization1StateText, civilization1Unlocked ? "DISPONIBLE" : "BLOQUEADA");
        SetText(civilization2StateText, civilization2Unlocked
            ? "NUEVA · DISPONIBLE"
            : "SILUETA · Confianza " +
              state.dimension2.civilization1.trust.ToString("0") + "/300");
        SetText(civilization3StateText, civilization3Unlocked ? "NUEVA · DISPONIBLE" : "");

        if (mapStatusText != null)
        {
            mapStatusText.text =
                "DIMENSIÓN 2 · PACTOS\n" +
                (civilization2Unlocked
                    ? "Se ha localizado otro territorio. Puedes visitarlo cuando quieras."
                    : "El Santuario está disponible. Otro territorio se perfila con la Confianza.");
        }

        if (civilization1PlaceholderText != null)
        {
            civilization1PlaceholderText.text =
                "CIVILIZACIÓN 1 — SANTUARIO DE PEREGRINOS";
        }

        if (civilization1PanelUI != null)
            civilization1PanelUI.Refresh();

        if (civilization2PanelUI != null)
            civilization2PanelUI.Refresh();
        if (civilization3PanelUI != null)
            civilization3PanelUI.Refresh();
    }

    private void OpenTerritory(string territoryId)
    {
        if (!Dimension2System.TrySelectTerritory(GameState.I, territoryId))
        {
            Refresh();
            return;
        }

        if (territoryId == Dimension2System.Civilization1TerritoryId)
        {
            D2PresentationRouter.RememberScreen(GameState.I, PresentationFeatureIds.D2C1Refuge);
            SetView(firstEntry: false, map: false, civilization1: true, civilization2: false, civilization3: false);
            Refresh();
        }
        else if (territoryId == Dimension2System.Civilization2TerritoryId)
        {
            D2PresentationRouter.RememberScreen(GameState.I, PresentationFeatureIds.D2C2Regions);
            SetView(firstEntry: false, map: false, civilization1: false, civilization2: true, civilization3: false);
            Refresh();
        }
        else if (territoryId == Dimension2System.Civilization3TerritoryId)
        {
            D2PresentationRouter.RememberScreen(GameState.I, PresentationFeatureIds.D2C3Archaeology);
            SetView(firstEntry: false, map: false, civilization1: false, civilization2: false, civilization3: true);
            Refresh();
        }
    }

    public void OpenContextualHelp()
    {
        if (helpRoot == null) return;
        helpRoot.SetActive(true);
        SetText(helpTitleText,
            PresentationTextCatalog.Current("help.d2.title"));
        SetText(helpBodyText,
            PresentationTextCatalog.Current("help.d2.body"));
        SetButtonLabel(closeHelpButton,
            PresentationTextCatalog.Current("help.close"));
    }

    public void CloseContextualHelp()
    {
        if (helpRoot != null) helpRoot.SetActive(false);
    }

    private void OpenResolvedScreen(string screenId)
    {
        if (screenId == D2PresentationRouter.FirstEntryScreenId)
        {
            ShowFirstEntry();
            return;
        }
        if (screenId == PresentationFeatureIds.D2Map)
        {
            ShowMap();
            return;
        }
        if (screenId != null && screenId.StartsWith("d2.c1."))
        {
            OpenTerritory(Dimension2System.Civilization1TerritoryId);
            OpenCivilization1ResolvedSection(screenId);
            return;
        }
        if (screenId != null && screenId.StartsWith("d2.c2."))
        {
            OpenTerritory(Dimension2System.Civilization2TerritoryId);
            OpenCivilization2ResolvedSection(screenId);
            return;
        }
        if (screenId != null && screenId.StartsWith("d2.c3."))
        {
            OpenTerritory(Dimension2System.Civilization3TerritoryId);
            OpenCivilization3ResolvedSection(screenId);
            return;
        }
        ShowMap();
    }

    public void OpenCivilization1ResolvedSection(string screenId)
    {
        if (civilization1PanelUI == null) return;
        switch (screenId)
        {
            case PresentationFeatureIds.D2C1Altars:
                civilization1PanelUI.ShowAltarsSection(); break;
            case PresentationFeatureIds.D2C1Pilgrimages:
                civilization1PanelUI.ShowPilgrimagesSection(); break;
            case PresentationFeatureIds.D2C1Novitiate:
                civilization1PanelUI.ShowNovitiateSection(); break;
            case PresentationFeatureIds.D2C1Rites:
                civilization1PanelUI.ShowRitesSection(); break;
            case PresentationFeatureIds.D2C1Pacts:
                civilization1PanelUI.ShowPactsSection(); break;
            case PresentationFeatureIds.D2C1VeiledThreshold:
                civilization1PanelUI.ShowVeiledThresholdSection(); break;
            default:
                civilization1PanelUI.ShowRefugeSection(); break;
        }
        RememberResolvedSection(screenId);
    }

    public void OpenCivilization2ResolvedSection(string screenId)
    {
        if (civilization2PanelUI == null) return;
        switch (screenId)
        {
            case PresentationFeatureIds.D2C2Operations:
                civilization2PanelUI.ShowOperations(); break;
            case PresentationFeatureIds.D2C2Defense:
                civilization2PanelUI.ShowDefense(); break;
            case PresentationFeatureIds.D2C2Resistance:
                civilization2PanelUI.ShowResistance(); break;
            case PresentationFeatureIds.D2C2Alert:
                civilization2PanelUI.ShowAlert(); break;
            case PresentationFeatureIds.D2C2Containment:
            case PresentationFeatureIds.D2C2MajorPact:
                civilization2PanelUI.ShowContainment(); break;
            default:
                civilization2PanelUI.ShowRegions(); break;
        }
        RememberResolvedSection(screenId);
    }

    public void OpenCivilization3ResolvedSection(string screenId)
    {
        if (civilization3PanelUI == null) return;
        switch (screenId)
        {
            case PresentationFeatureIds.D2C3Archive:
                civilization3PanelUI.ShowArchive(); break;
            case PresentationFeatureIds.D2C3EntityResearch:
            case PresentationFeatureIds.D2C3EntityPact:
                civilization3PanelUI.ShowEntityResearch(); break;
            default:
                civilization3PanelUI.ShowArchaeology(); break;
        }
        RememberResolvedSection(screenId);
    }

    private static void RememberResolvedSection(string screenId)
    {
        GameState gameState = GameState.I;
        if (!D2PresentationRouter.RememberScreen(gameState, screenId) ||
            gameState?.dimension2?.presentation == null) return;
        PresentationStateUtility.Acknowledge(
            gameState.dimension2.presentation, screenId);
    }

    private void SetView(
        bool firstEntry,
        bool map,
        bool civilization1,
        bool civilization2,
        bool civilization3
    )
    {
        if (firstEntryRoot != null)
            firstEntryRoot.SetActive(firstEntry);

        if (mapRoot != null)
            mapRoot.SetActive(map);

        if (civilization1Root != null)
            civilization1Root.SetActive(civilization1);

        if (civilization2Root != null)
            civilization2Root.SetActive(civilization2);

        if (civilization3Root != null)
            civilization3Root.SetActive(civilization3);
    }

    private static void SetButtonState(Button button, bool unlocked)
    {
        if (button != null)
            button.interactable = unlocked;
    }

    private static void SetText(TMP_Text text, string value)
    {
        if (text != null)
            text.text = value;
    }

    private static void SetButtonLabel(Button button, string value)
    {
        if (button == null) return;
        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        if (label != null) label.text = value;
    }
}

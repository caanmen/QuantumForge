using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Dimension2PanelUI : MonoBehaviour
{
    [Header("Vistas")]
    public GameObject firstEntryRoot;
    public GameObject mapRoot;
    public GameObject civilization1Root;
    public Button closeDimension2Button;

    [Header("Entrada")]
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

        if (!GameState.I.dimension2.firstEntrySeen)
            ShowFirstEntry();
        else
            ShowMap();
    }

    public void ShowFirstEntry()
    {
        SetView(firstEntry: true, map: false, civilization1: false);
        Refresh();
    }

    public void ContinueFirstEntry()
    {
        Dimension2System.MarkFirstEntrySeen(GameState.I);
        ShowMap();
    }

    public void ShowMap()
    {
        if (!Dimension2System.CanAccessDimension2(GameState.I))
            return;

        SetView(firstEntry: false, map: true, civilization1: false);
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

        SetText(civilization1StateText, civilization1Unlocked ? "DISPONIBLE" : "BLOQUEADA");
        SetText(civilization2StateText, civilization2Unlocked ? "DISPONIBLE" : "BLOQUEADA — Requiere 300 de Confianza en Civ 1");
        SetText(civilization3StateText, civilization3Unlocked ? "DISPONIBLE" : "BLOQUEADA — Requiere progreso de Civ 2");

        if (mapStatusText != null)
        {
            mapStatusText.text =
                "DIMENSIÓN DE LOS PACTOS\n" +
                "Tres territorios guardan respuestas distintas. " +
                "Civilización 1 es el único punto de entrada disponible.";
        }

        if (civilization1PlaceholderText != null)
        {
            civilization1PlaceholderText.text =
                "CIVILIZACIÓN 1 — SANTUARIO DE PEREGRINOS";
        }

        if (civilization1PanelUI != null)
            civilization1PanelUI.Refresh();
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
            SetView(firstEntry: false, map: false, civilization1: true);
            Refresh();
        }
        else
        {
            ShowMap();
            if (mapStatusText != null)
            {
                mapStatusText.text =
                    Dimension2System.GetTerritoryDisplayName(territoryId) +
                    "\nDESBLOQUEADA — su contenido se implementará en el bloque correspondiente.";
            }
        }
    }

    private void SetView(bool firstEntry, bool map, bool civilization1)
    {
        if (firstEntryRoot != null)
            firstEntryRoot.SetActive(firstEntry);

        if (mapRoot != null)
            mapRoot.SetActive(map);

        if (civilization1Root != null)
            civilization1Root.SetActive(civilization1);
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
}

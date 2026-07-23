using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;


public class TabsUI : MonoBehaviour

{
    public static TabsUI Instance { get; private set; }

    [Header("Botones de pestañas")]
    public Button btnGeneracion;
    public Button btnLab;
    public Button btnRoom2;
    public Button btnDimension1;
    public Button btnDimension2;
    public Button btnDimension3;
    public Button btnLogros;
    public Button btnPrestigio;   // 🔹 nuevo
    

    [Header("Paneles")]
    public GameObject panelGeneracion;
    public GameObject panelLab;
    public GameObject room2Panel;
    public GameObject dimension1Panel;
    public GameObject dimension2Panel;
    public GameObject dimension3Panel;
    public GameObject panelLogros;
    public GameObject prestigePanel;      // 🔹 nuevo

    [Header("Layouts de Generación")]
    public GameObject generationDefaultLayout;
    public GameObject generationTriangleLayout;
    public CanvasGroup generationDefaultCanvasGroup;

    private void Awake()
    {
        Instance = this;

        if (btnGeneracion != null)
            btnGeneracion.onClick.AddListener(ShowGeneracion);

        if (btnLab != null)
            btnLab.onClick.AddListener(ShowLab);

        if (btnRoom2 != null)
            btnRoom2.onClick.AddListener(ShowRoom2);

        if (btnDimension1 != null)
            btnDimension1.onClick.AddListener(ShowDimension1);

        if (btnDimension2 != null)
            btnDimension2.onClick.AddListener(ShowDimension2);

        if (btnDimension3 != null)
            btnDimension3.onClick.AddListener(ShowDimension3);

        if (btnLogros != null)
            btnLogros.onClick.AddListener(ShowLogros);

        if (btnPrestigio != null)
            btnPrestigio.onClick.AddListener(ShowPrestigio);

    }

    private void Start()
    {
        RefreshPrestigeButtonVisibility();
        HideMetaNavigationButton();
        RefreshRoom2ButtonVisibility();
        RefreshDimension1ButtonVisibility();
        RefreshDimension2ButtonVisibility();
        RefreshDimension3ButtonVisibility();

        ShowGeneracion();   // pestaña por defecto
        StartCoroutine(RefreshGenerationLayoutNextFrame());
    }

    private IEnumerator RefreshGenerationLayoutNextFrame()
    {
        yield return null;

        if (panelGeneracion != null && panelGeneracion.activeSelf)
        {
            RefreshGenerationLayoutState();
        }
    }

    private void HideAll()
    {
        if (panelGeneracion != null)    panelGeneracion.SetActive(false);
        if (panelLab != null)          panelLab.SetActive(false);
        if (room2Panel != null)         room2Panel.SetActive(false);
        if (dimension1Panel != null)   dimension1Panel.SetActive(false);
        if (dimension2Panel != null)   dimension2Panel.SetActive(false);
        if (dimension3Panel != null)   dimension3Panel.SetActive(false);
        if (panelLogros != null)       panelLogros.SetActive(false);
        if (prestigePanel != null)     prestigePanel.SetActive(false);
    }

    private void RefreshGenerationLayoutState()
    {
        // El triángulo queda siempre visible en escena.
        // Su bloqueo/desbloqueo se maneja por lógica interna (slots, overlay futuro, etc).
        if (generationTriangleLayout != null)
        {
            generationTriangleLayout.SetActive(true);
        }

        // El layout default se mantiene vivo también por estabilidad,
        // pero no depende ya del unlock del triángulo.
        if (generationDefaultLayout != null)
        {
            generationDefaultLayout.SetActive(true);

            if (generationDefaultCanvasGroup != null)
            {
                generationDefaultCanvasGroup.alpha = 1f;
                generationDefaultCanvasGroup.interactable = true;
                generationDefaultCanvasGroup.blocksRaycasts = true;
            }
        }
    }

    public void RefreshGenerationLayoutFromOutside()
    {
        RefreshGenerationLayoutState();
    }

    
    public void RefreshRoom2ButtonVisibility()
    {
        if (btnRoom2 == null)
            return;

        bool unlocked = GameState.I != null && GameState.I.experimentalChamberUnlocked;
        btnRoom2.gameObject.SetActive(unlocked);
    }

    public void RefreshDimension1ButtonVisibility()
    {
        if (btnDimension1 == null)
            return;

        bool unlocked = GameState.I != null && GameState.I.dimension01Unlocked;
        btnDimension1.gameObject.SetActive(unlocked);
    }

    public void RefreshPrestigeButtonVisibility()
    {
        if (btnPrestigio != null)
        {
            bool prestige1Completed = GameState.I != null &&
                GameState.I.hasDonePrestige1;
            btnPrestigio.gameObject.SetActive(!prestige1Completed);
        }
    }

    private void HideMetaNavigationButton()
    {
        Transform navigationParent = btnGeneracion != null
            ? btnGeneracion.transform.parent
            : null;
        if (navigationParent == null)
            return;

        Button[] navigationButtons = navigationParent.GetComponentsInChildren<Button>(true);
        for (int i = 0; i < navigationButtons.Length; i++)
        {
            Button button = navigationButtons[i];
            if (button == null) continue;
            TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
            if (label == null) continue;
            string text = label.text == null ? "" : label.text.ToLowerInvariant();
            if (text.Contains("meta"))
                button.gameObject.SetActive(false);
        }
    }

    public void RefreshDimension2ButtonVisibility()
    {
        if (btnDimension2 == null)
            return;

        bool unlocked = Dimension2System.CanAccessDimension2(GameState.I);
        btnDimension2.gameObject.SetActive(unlocked);
    }

    public void RefreshDimension3ButtonVisibility()
    {
        if (btnDimension3 == null)
            return;

        bool unlocked = Dimension3System.CanAccessDimension3(GameState.I);
        btnDimension3.gameObject.SetActive(unlocked);
    }


    public void ShowGeneracion()
    {
        HideAll();

        if (panelGeneracion != null)
            panelGeneracion.SetActive(true);

        RefreshRoom2ButtonVisibility();
        RefreshDimension1ButtonVisibility();
        RefreshDimension2ButtonVisibility();
        RefreshDimension3ButtonVisibility();
        RefreshPrestigeButtonVisibility();
        RefreshGenerationLayoutState();
    }

    private void ShowLab()
    {
        HideAll();
        if (panelLab != null) panelLab.SetActive(true);
    }

    public void ShowRoom2()
    {
        if (GameState.I == null || !GameState.I.experimentalChamberUnlocked)
            return;

        HideAll();

        if (room2Panel != null)
            room2Panel.SetActive(true);
    }

    public void ShowDimension1()
    {
        if (GameState.I == null || !GameState.I.dimension01Unlocked)
            return;

        HideAll();

        if (dimension1Panel != null)
            dimension1Panel.SetActive(true);

        RefreshRoom2ButtonVisibility();
        RefreshDimension1ButtonVisibility();
        RefreshDimension2ButtonVisibility();
        RefreshDimension3ButtonVisibility();
    }

    public void ShowDimension2()
    {
        if (!Dimension2System.CanAccessDimension2(GameState.I))
            return;

        HideAll();

        if (dimension2Panel != null)
        {
            dimension2Panel.SetActive(true);

            Dimension2PanelUI panel = dimension2Panel.GetComponent<Dimension2PanelUI>();
            if (panel != null)
                panel.OpenFromTab();
        }

        RefreshRoom2ButtonVisibility();
        RefreshDimension1ButtonVisibility();
        RefreshDimension2ButtonVisibility();
        RefreshDimension3ButtonVisibility();
    }

    public void ShowDimension3()
    {
        if (!Dimension3System.CanAccessDimension3(GameState.I))
            return;

        HideAll();

        if (dimension3Panel != null)
        {
            dimension3Panel.SetActive(true);
            Dimension3PanelUI panel = dimension3Panel.GetComponent<Dimension3PanelUI>();
            if (panel != null)
                panel.OpenFromTab();
        }

        RefreshRoom2ButtonVisibility();
        RefreshDimension1ButtonVisibility();
        RefreshDimension2ButtonVisibility();
        RefreshDimension3ButtonVisibility();
    }
    private void ShowLogros()
    {
        HideAll();
        if (panelLogros != null) panelLogros.SetActive(true);
    }

    private void ShowPrestigio()
    {
        HideAll();
        if (prestigePanel != null) prestigePanel.SetActive(true);
    }

}


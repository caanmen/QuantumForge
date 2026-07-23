using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PrestigeUI : MonoBehaviour
{
    [Header("Textos de transición")]
    public TextMeshProUGUI entActualText;
    public TextMeshProUGUI entGananciaText;

    [Header("Textos viejos / opcionales")]
    public TextMeshProUGUI leMult1Text;
    public TextMeshProUGUI autoBuy1Text;

    [Header("Botones viejos / opcionales")]
    public Button autoBuyToggleButton;
    public TextMeshProUGUI autoBuyToggleLabel;

    [Header("Rendimiento UI")]
    [SerializeField] private float uiRefreshInterval = 0.25f;
    private float _uiTimer = 0f;
    private GameObject _dimensionSelectionRoot;
    private Button[] _dimensionSelectionButtons;
    private GameObject _dimensionObjectiveRoot;
    private TextMeshProUGUI _dimensionObjectiveText;
    private int _revealedDimensionId;

    private void Awake()
    {
        BuildDimensionSelectionUI();
        BuildDimensionObjectiveUI();
    }

    private void Update()
    {
        GameState gs = GameState.I;
        if (gs == null)
            return;

        _uiTimer += Time.unscaledDeltaTime;
        if (_uiTimer < uiRefreshInterval)
            return;

        _uiTimer = 0f;

        bool canPrestige1 = gs.CanDoPrestige1();

        if (entActualText != null)
        {
            entActualText.text =
                "Prestigios 1 realizados: " + gs.prestige1Count;
        }

        if (entGananciaText != null)
        {
            entGananciaText.text = gs.GetPrestige1StatusText();
        }

        if (leMult1Text != null)
        {
            leMult1Text.text =
                "Prestigio 1 reinicia el laboratorio base, pero conserva el conocimiento descubierto.";
        }

        if (autoBuy1Text != null)
        {
            autoBuy1Text.text = canPrestige1
                ? "La convergencia está lista."
                : "Repara la Máquina al 80% y activa el Canal de Convergencia.";
        }

        if (autoBuyToggleButton != null)
        {
            autoBuyToggleButton.gameObject.SetActive(false);
        }

        if (autoBuyToggleLabel != null)
        {
            autoBuyToggleLabel.text = "";
        }
    }

    public void OnClickPrestige()
    {
        GameState gs = GameState.I;
        if (gs == null)
            return;

        if (!gs.CanDoPrestige1())
        {
            Debug.Log("[PrestigeUI] Prestigio 1 todavía no disponible: " + gs.GetPrestige1StatusText());
            return;
        }

        ShowDimensionSelection();
    }

    private void ShowDimensionSelection()
    {
        if (_dimensionSelectionRoot == null)
            BuildDimensionSelectionUI();

        RefreshDimensionSelectionButtons();
        if (_dimensionSelectionRoot != null)
            _dimensionSelectionRoot.SetActive(true);
    }

    private void HideDimensionSelection()
    {
        if (_dimensionSelectionRoot != null)
            _dimensionSelectionRoot.SetActive(false);
    }

    private void SelectDimensionForPrestige1(int dimensionId)
    {
        GameState gs = GameState.I;
        if (gs == null || !gs.DoPrestige1Reset(dimensionId))
            return;

        HideDimensionSelection();
        ShowDimensionObjective(dimensionId);
        Debug.Log("[PrestigeUI] Prestigio 1 realizado con Dimensión " + dimensionId + ".");

        if (TabsUI.Instance != null)
        {
            TabsUI.Instance.RefreshDimension1ButtonVisibility();
            TabsUI.Instance.RefreshDimension2ButtonVisibility();
            TabsUI.Instance.RefreshDimension3ButtonVisibility();
            TabsUI.Instance.RefreshPrestigeButtonVisibility();
        }
    }

    private void BuildDimensionSelectionUI()
    {
        if (_dimensionSelectionRoot != null)
            return;

        var root = new GameObject("PrestigeDimensionSelection", typeof(RectTransform), typeof(Image));
        root.transform.SetParent(transform, false);
        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0.08f, 0.12f);
        rootRect.anchorMax = new Vector2(0.92f, 0.88f);
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;
        root.GetComponent<Image>().color = new Color(0.035f, 0.055f, 0.11f, 0.98f);

        var layout = root.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(32, 32, 28, 28);
        layout.spacing = 14f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = false;

        CreateSelectionText(root.transform,
            "Elige una dimensión", 30f, FontStyles.Bold, 54f);
        CreateSelectionText(root.transform,
            "Solo una dimensión será revelada. El juego base se reiniciará; el progreso dimensional se conservará.",
            18f, FontStyles.Normal, 64f);

        _dimensionSelectionButtons = new Button[3];
        for (int i = 0; i < 3; i++)
        {
            int dimensionId = i + 1;
            _dimensionSelectionButtons[i] = CreateSelectionButton(
                root.transform, "Dimensión " + dimensionId, dimensionId);
        }

        Button cancelButton = CreateSelectionButton(root.transform, "Volver", 0);
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(HideDimensionSelection);

        _dimensionSelectionRoot = root;
        root.SetActive(false);
    }

    private void BuildDimensionObjectiveUI()
    {
        if (_dimensionObjectiveRoot != null)
            return;

        var root = new GameObject("PrestigeDimensionObjective",
            typeof(RectTransform), typeof(Image));
        root.transform.SetParent(transform, false);
        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0.10f, 0.18f);
        rootRect.anchorMax = new Vector2(0.90f, 0.82f);
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;
        root.GetComponent<Image>().color = new Color(0.035f, 0.055f, 0.11f, 0.98f);

        var layout = root.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(32, 32, 28, 28);
        layout.spacing = 16f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = false;

        CreateSelectionText(root.transform, "Dimensión revelada", 30f,
            FontStyles.Bold, 54f);
        _dimensionObjectiveText = CreateSelectionText(root.transform, "", 19f,
            FontStyles.Normal, 120f);

        Button enterButton = CreateSelectionButton(root.transform,
            "ENTRAR A LA DIMENSIÓN", -1);
        enterButton.onClick.RemoveAllListeners();
        enterButton.onClick.AddListener(EnterRevealedDimension);
        Button laterButton = CreateSelectionButton(root.transform,
            "VOLVER AL JUEGO BASE", -1);
        laterButton.onClick.RemoveAllListeners();
        laterButton.onClick.AddListener(HideDimensionObjective);

        _dimensionObjectiveRoot = root;
        root.SetActive(false);
    }

    private void ShowDimensionObjective(int dimensionId)
    {
        if (_dimensionObjectiveRoot == null)
            BuildDimensionObjectiveUI();

        _revealedDimensionId = dimensionId;
        if (_dimensionObjectiveText != null)
            _dimensionObjectiveText.text = GetDimensionObjectiveText(dimensionId);
        if (_dimensionObjectiveRoot != null)
            _dimensionObjectiveRoot.SetActive(true);
    }

    private void HideDimensionObjective()
    {
        if (_dimensionObjectiveRoot != null)
            _dimensionObjectiveRoot.SetActive(false);
    }

    private void EnterRevealedDimension()
    {
        HideDimensionObjective();
        if (TabsUI.Instance == null)
            return;

        switch (_revealedDimensionId)
        {
            case 1: TabsUI.Instance.ShowDimension1(); break;
            case 2: TabsUI.Instance.ShowDimension2(); break;
            case 3: TabsUI.Instance.ShowDimension3(); break;
        }
    }

    private static string GetDimensionObjectiveText(int dimensionId)
    {
        bool isFinalReveal = GameState.I != null &&
            !GameState.I.HasAvailableDimensionForPrestige1Selection();
        string nextStep = isFinalReveal
            ? "Al conseguirlo cerrarás el ciclo de Prestigio 1. Prestigio 2 llegará en una expansión futura."
            : "Al conseguirlo podrás completar otra vuelta de la Máquina y revelar una dimensión restante.";

        switch (dimensionId)
        {
            case 1:
                return "La Dimensión 1 ha sido revelada.\n\n" +
                    "Objetivo de esta ruta: descubre el Ancla Galáctica.\n\n" +
                    nextStep;
            case 2:
                return "La Dimensión 2 ha sido revelada.\n\n" +
                    "Objetivo de esta ruta: establece el Pacto Mayor.\n\n" +
                    nextStep;
            case 3:
                return "La Dimensión 3 ha sido revelada.\n\n" +
                    "Objetivo de esta ruta: integra el Núcleo de Autonomía.\n\n" +
                    nextStep;
            default:
                return "Explora la dimensión revelada para descubrir su hito único.";
        }
    }

    private void RefreshDimensionSelectionButtons()
    {
        GameState gs = GameState.I;
        if (gs == null || _dimensionSelectionButtons == null)
            return;

        for (int i = 0; i < _dimensionSelectionButtons.Length; i++)
        {
            int dimensionId = i + 1;
            Button button = _dimensionSelectionButtons[i];
            bool available = !gs.IsDimensionUnlockedAfterPrestige1(dimensionId);
            button.interactable = available;
            TMP_Text label = button.GetComponentInChildren<TMP_Text>();
            if (label != null)
                label.text = available
                    ? "Dimensión " + dimensionId
                    : "Dimensión " + dimensionId + " — ya revelada";
        }
    }

    private TextMeshProUGUI CreateSelectionText(
        Transform parent, string content, float fontSize, FontStyles style, float height)
    {
        var textObject = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);
        var layout = textObject.AddComponent<LayoutElement>();
        layout.preferredHeight = height;
        var text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = content;
        text.font = entActualText != null ? entActualText.font : null;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.alignment = TextAlignmentOptions.Center;
        text.enableWordWrapping = true;
        text.color = Color.white;
        return text;
    }

    private Button CreateSelectionButton(Transform parent, string label, int dimensionId)
    {
        var buttonObject = new GameObject("DimensionButton", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);
        var layout = buttonObject.AddComponent<LayoutElement>();
        layout.preferredHeight = 56f;
        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.13f, 0.31f, 0.52f, 1f);
        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(() => SelectDimensionForPrestige1(dimensionId));

        var textObject = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(buttonObject.transform, false);
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        var text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = label;
        text.font = entActualText != null ? entActualText.font : null;
        text.fontSize = 22f;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        return button;
    }

    // Métodos viejos: se dejan para no romper botones asignados en la escena.
    public void OnClickToggleAutoBuy()
    {
        Debug.Log("[PrestigeUI] Auto-buy viejo desactivado. ENT ya no se usa para Prestigio 1.");
    }

    public void OnClickBuyLeMult1()
    {
        Debug.Log("[PrestigeUI] Upgrade viejo de ENT desactivado.");
    }

    public void OnClickBuyAutoBuy1()
    {
        Debug.Log("[PrestigeUI] Upgrade viejo de auto-compra desactivado.");
    }
}

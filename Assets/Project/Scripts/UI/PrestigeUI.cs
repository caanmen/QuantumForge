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
    private GameObject _convergenceRoot;
    private TextMeshProUGUI _convergenceStatusText;
    private Button _rebuildReceiverButton;
    private Button _startConvergenceButton;
    private Button _rotateCircuitButton;
    private Button _confirmConfigurationButton;
    private Button[,] _convergenceBoardButtons;

    private void Awake()
    {
        BuildDimensionSelectionUI();
        BuildDimensionObjectiveUI();
        BuildConvergenceUI();
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

        RefreshConvergenceUI();
    }

    public void OnClickPrestige()
    {
        GameState gs = GameState.I;
        if (gs == null)
            return;

        if (!gs.CanDoPrestige1())
        {
            if (ConvergenceCircuitSystem.IsConvergenceUnlocked(gs))
            {
                ShowConvergenceUI();
                return;
            }
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
        Debug.Log(
            "[PrestigeUI] Prestigio 1 realizado al sintonizar " +
            GetDimensionSignatureName(dimensionId) + "."
        );

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
            "Sintoniza una Firma Dimensional", 30f, FontStyles.Bold, 54f);
        CreateSelectionText(root.transform,
            "La Máquina detectó tres firmas desconocidas. Solo una ruta será revelada; el juego base se reiniciará y el progreso dimensional se conservará.",
            18f, FontStyles.Normal, 64f);

        _dimensionSelectionButtons = new Button[3];
        for (int i = 0; i < 3; i++)
        {
            int dimensionId = i + 1;
            _dimensionSelectionButtons[i] = CreateSelectionButton(
                root.transform, GetDimensionSelectionLabel(dimensionId, true), dimensionId);
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

        CreateSelectionText(root.transform, "Firma sintonizada", 30f,
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

    private void BuildConvergenceUI()
    {
        if (_convergenceRoot != null)
            return;

        var root = new GameObject("ConvergencePanel",
            typeof(RectTransform), typeof(Image));
        root.transform.SetParent(transform, false);
        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0.10f, 0.12f);
        rootRect.anchorMax = new Vector2(0.90f, 0.88f);
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;
        root.GetComponent<Image>().color = new Color(0.035f, 0.055f, 0.11f, 0.98f);

        var layout = root.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(32, 32, 28, 28);
        layout.spacing = 12f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        CreateSelectionText(root.transform, "Convergencia", 30f,
            FontStyles.Bold, 54f);
        _convergenceStatusText = CreateSelectionText(root.transform, "", 18f,
            FontStyles.Normal, 112f);
        _rebuildReceiverButton = CreateActionButton(root.transform,
            "RECONSTRUIR RECEPTOR DIMENSIONAL", RebuildReceiver);
        _startConvergenceButton = CreateActionButton(root.transform,
            "INICIAR CONVERGENCIA", StartConvergence);
        BuildConvergenceBoard(root.transform);
        _rotateCircuitButton = CreateActionButton(root.transform,
            "ROTAR PULSO 90°", RotateStartupPulse);
        _confirmConfigurationButton = CreateActionButton(root.transform,
            "ESTABILIZAR CONFIGURACIÓN", ConfirmConfiguration);
        CreateActionButton(root.transform, "VOLVER", HideConvergenceUI);
        _convergenceRoot = root;
        root.SetActive(false);
    }

    private void ShowConvergenceUI()
    {
        if (_convergenceRoot == null)
            BuildConvergenceUI();
        _convergenceRoot.SetActive(true);
        RefreshConvergenceUI();
    }

    private void HideConvergenceUI()
    {
        if (_convergenceRoot != null)
            _convergenceRoot.SetActive(false);
    }

    private void RefreshConvergenceUI()
    {
        GameState state = GameState.I;
        if (state == null || _convergenceRoot == null || !_convergenceRoot.activeSelf)
            return;

        state.EnsureConvergenceState();
        int owned = ConvergenceCircuitSystem.GetOwnedCircuitCount(state);
        double required = ConvergenceBalance.GetRequiredStabilityForNextCircuit(owned);
        bool ready = ConvergenceSynchronizationSystem.IsSynchronizationReadyForNextConvergence(
            state, owned);
        bool configuring = state.convergence.phase == ConvergencePhase.ConfigurationPending;
        _convergenceStatusText.text = "Receptor Dimensional: " +
            (state.convergence.dimensionalReceiverRebuilt ? "reconstruido" : "pendiente") + "\n" +
            "Señales: D1 " + GetSignalStatus(state, 1) + " · D2 " +
            GetSignalStatus(state, 2) + " · D3 " + GetSignalStatus(state, 3) + "\n" +
            "Estabilidad: " + state.convergence.currentStability.ToString("0.#") +
            " / " + required.ToString("0.#") + "\n" +
            "Circuitos poseídos: " + owned +
            (ready ? "\nLa Convergencia está preparada." : "");
        _rebuildReceiverButton.interactable = !state.convergence.dimensionalReceiverRebuilt;
        _startConvergenceButton.interactable = ready && !configuring;
        _rotateCircuitButton.interactable = configuring && FindStartupPulse(state) != null;
        _confirmConfigurationButton.interactable = configuring;
        RefreshConvergenceBoard(state, configuring);
    }

    private void RefreshConvergenceBoard(GameState state, bool configuring)
    {
        if (_convergenceBoardButtons == null)
            return;

        ConvergenceCircuitPlacement placement = FindStartupPulse(state);
        for (int y = -2; y <= 2; y++)
        {
            for (int x = -2; x <= 2; x++)
            {
                Button cell = _convergenceBoardButtons[x + 2, y + 2];
                bool core = x == 0 && y == 0;
                bool hasPulse = placement != null && placement.x == x &&
                    placement.y == y;
                TMP_Text label = cell.GetComponentInChildren<TMP_Text>();
                label.text = core ? "N" : hasPulse
                    ? (placement.rotationDegrees == 0 ||
                       placement.rotationDegrees == 180 ? "│" : "─")
                    : "·";
                cell.interactable = configuring && !core;
            }
        }
    }

    private static string GetSignalStatus(GameState state, int dimensionId)
    {
        return ConvergenceSynchronizationSystem.IsSignalActivated(state, dimensionId)
            ? "activa" : "pendiente";
    }

    private void RebuildReceiver()
    {
        string reason;
        if (!ConvergenceSynchronizationSystem.TryRebuildReceiver(GameState.I, out reason))
            Debug.Log("[Convergence UI] " + reason);
        else if (SaveService.I != null)
            SaveService.I.Save();
        RefreshConvergenceUI();
    }

    private void StartConvergence()
    {
        string reason;
        if (!ConvergenceCircuitSystem.TryStartNormalConvergence(GameState.I, out reason))
            Debug.Log("[Convergence UI] " + reason);
        RefreshConvergenceUI();
    }

    private void BuildConvergenceBoard(Transform parent)
    {
        GameObject board = new GameObject("ConvergenceBoard", typeof(RectTransform),
            typeof(GridLayoutGroup));
        board.transform.SetParent(parent, false);
        board.AddComponent<LayoutElement>().preferredHeight = 244f;
        GridLayoutGroup grid = board.GetComponent<GridLayoutGroup>();
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 5;
        grid.cellSize = new Vector2(42f, 42f);
        grid.spacing = new Vector2(4f, 4f);
        grid.childAlignment = TextAnchor.MiddleCenter;
        _convergenceBoardButtons = new Button[5, 5];
        for (int y = 2; y >= -2; y--)
        {
            for (int x = -2; x <= 2; x++)
            {
                int cellX = x;
                int cellY = y;
                Button cell = CreateBoardCell(board.transform, cellX, cellY);
                cell.onClick.AddListener(() => PlaceStartupPulse(cellX, cellY));
                _convergenceBoardButtons[cellX + 2, cellY + 2] = cell;
            }
        }
    }

    private void PlaceStartupPulse(int x, int y)
    {
        ConvergenceCircuitPlacement current = FindStartupPulse(GameState.I);
        int rotation = current != null ? current.rotationDegrees : 0;
        string reason;
        if (!ConvergenceCircuitSystem.TryPlaceCircuit(GameState.I,
                ConvergenceCircuitCatalog.StartupPulseCircuitId, x, y, rotation,
                out reason))
        {
            Debug.Log("[Convergence UI] " + reason);
        }
        RefreshConvergenceUI();
    }

    private Button CreateBoardCell(Transform parent, int x, int y)
    {
        var cellObject = new GameObject("Cell_" + x + "_" + y,
            typeof(RectTransform), typeof(Image), typeof(Button));
        cellObject.transform.SetParent(parent, false);
        Image image = cellObject.GetComponent<Image>();
        image.color = x == 0 && y == 0
            ? new Color(0.8f, 0.52f, 0.15f, 1f)
            : new Color(0.11f, 0.22f, 0.34f, 1f);
        Button button = cellObject.GetComponent<Button>();
        button.targetGraphic = image;
        var labelObject = new GameObject("Label", typeof(RectTransform),
            typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(cellObject.transform, false);
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        TMP_Text label = labelObject.GetComponent<TMP_Text>();
        label.font = entActualText != null ? entActualText.font : null;
        label.fontSize = 25f;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;
        return button;
    }

    private void RotateStartupPulse()
    {
        ConvergenceCircuitPlacement current = FindStartupPulse(GameState.I);
        if (current == null)
            return;
        string reason;
        if (!ConvergenceCircuitSystem.TryPlaceCircuit(GameState.I, current.circuitId,
                current.x, current.y, (current.rotationDegrees + 90) % 360,
                out reason))
        {
            Debug.Log("[Convergence UI] " + reason);
        }
        RefreshConvergenceUI();
    }

    private void ConfirmConfiguration()
    {
        string reason;
        if (!ConvergenceCircuitSystem.TryConfirmConfiguration(GameState.I, out reason))
            Debug.Log("[Convergence UI] " + reason);
        RefreshConvergenceUI();
    }

    private static ConvergenceCircuitPlacement FindStartupPulse(GameState state)
    {
        return state != null && state.convergence != null &&
            state.convergence.boardPlacements != null
            ? state.convergence.boardPlacements.Find(placement => placement != null &&
                placement.circuitId == ConvergenceCircuitCatalog.StartupPulseCircuitId)
            : null;
    }

    private Button CreateActionButton(Transform parent, string label,
        UnityEngine.Events.UnityAction action)
    {
        var buttonObject = new GameObject("ConvergenceAction", typeof(RectTransform),
            typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);
        buttonObject.AddComponent<LayoutElement>().preferredHeight = 48f;
        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.13f, 0.31f, 0.52f, 1f);
        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);
        var textObject = new GameObject("Label", typeof(RectTransform),
            typeof(TextMeshProUGUI));
        textObject.transform.SetParent(buttonObject.transform, false);
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        TMP_Text text = textObject.GetComponent<TMP_Text>();
        text.text = label;
        text.font = entActualText != null ? entActualText.font : null;
        text.fontSize = 18f;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        return button;
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
                return GetDimensionSignatureName(dimensionId) +
                    " ha revelado la Dimensión 1.\n\n" +
                    "Objetivo de esta ruta: descubre el Ancla Galáctica.\n\n" +
                    nextStep;
            case 2:
                return GetDimensionSignatureName(dimensionId) +
                    " ha revelado la Dimensión 2.\n\n" +
                    "Objetivo de esta ruta: establece el Pacto Mayor.\n\n" +
                    nextStep;
            case 3:
                return GetDimensionSignatureName(dimensionId) +
                    " ha revelado la Dimensión 3.\n\n" +
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
                label.text = GetDimensionSelectionLabel(dimensionId, available);
        }
    }

    public static string GetDimensionSignatureName(int dimensionId)
    {
        switch (dimensionId)
        {
            case 1: return "Firma Dimensional Alfa";
            case 2: return "Firma Dimensional Beta";
            case 3: return "Firma Dimensional Gamma";
            default: return "Firma Dimensional desconocida";
        }
    }

    public static string GetDimensionSelectionLabel(int dimensionId, bool available)
    {
        string signature = GetDimensionSignatureName(dimensionId);
        return available ? signature : signature + " — ya sintonizada";
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

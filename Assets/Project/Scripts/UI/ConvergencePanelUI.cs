using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConvergencePanelUI : MonoBehaviour
{
    public const string UnknownSignalEventId = ConvergenceSystem.UnknownSignalEventId;
    public const string UnknownSignalTitle = "???";
    public const string UnknownSignalBody =
        "La red ha detectado una respuesta que no puede interpretar.";

    private TMP_FontAsset _font;
    private GameObject _cycleView;
    private GameObject _boardView;
    private GameObject _archiveView;
    private GameObject _unknownModal;
    private Transform _viewContainer;
    private TextMeshProUGUI _cycleStatus;
    private TextMeshProUGUI _boardStatus;
    private TextMeshProUGUI _archiveStatus;
    private TextMeshProUGUI _errorText;
    private TextMeshProUGUI _activeConfigurationText;
    private TextMeshProUGUI _draftConfigurationText;
    private Button _rebuildButton;
    private Button _startButton;
    private Button _confirmButton;
    private Button _rotateButton;
    private Button _removeButton;
    private Button _restoreButton;
    private Button _retryTransactionButton;
    private readonly List<Button> _inventoryButtons = new List<Button>();
    private Button[,] _boardButtons;
    private string _selectedCircuitId;
    private string _persistentError = "";
    private bool _initialized;
    private bool _unknownModalDismissedThisOpen;

    public Button ConfirmButton => _confirmButton;
    public Button RetryTransactionButton => _retryTransactionButton;
    public string PersistentError => _persistentError;
    public bool UnknownModalVisible => _unknownModal != null && _unknownModal.activeSelf;

    public void Initialize(TMP_FontAsset font)
    {
        if (_initialized) return;
        _initialized = true;
        _font = font;

        RectTransform rootRect = GetComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0.06f, 0.06f);
        rootRect.anchorMax = new Vector2(0.94f, 0.94f);
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;
        Image background = gameObject.AddComponent<Image>();
        background.color = new Color(0.025f, 0.045f, 0.09f, 0.99f);
        VerticalLayoutGroup layout = gameObject.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(22, 22, 18, 18);
        layout.spacing = 8f;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childAlignment = TextAnchor.UpperCenter;

        CreateText(transform, "CONVERGENCIA", 28f, FontStyles.Bold, 42f);
        BuildViewTabs();
        _errorText = CreateText(transform, "", 16f, FontStyles.Bold, 38f);
        _errorText.color = new Color(1f, 0.42f, 0.32f, 1f);
        BuildContentScroll();
        BuildCycleView();
        BuildBoardView();
        BuildArchiveView();
        CreateButton(transform, "VOLVER", Hide, 40f);
        BuildUnknownModal();
        ShowView(_cycleView);
        gameObject.SetActive(false);
    }

    public void Show()
    {
        _unknownModalDismissedThisOpen = false;
        gameObject.SetActive(true);
        Refresh();
    }

    public void Hide()
    {
        _unknownModalDismissedThisOpen = false;
        if (_unknownModal != null) _unknownModal.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Refresh()
    {
        if (!_initialized || !gameObject.activeSelf || GameState.I == null) return;
        GameState state = GameState.I;
        state.EnsureConvergenceState();
        ConvergenceState convergence = state.convergence;
        int owned = ConvergenceCircuitSystem.GetOwnedCircuitCount(state);
        bool configuring = convergence.phase == ConvergencePhase.ConfigurationPending;
        bool hasNext = ConvergenceCircuitSystem.HasNextDesignedCircuit(state);
        bool ready = ConvergenceSynchronizationSystem.IsSynchronizationReadyForNextConvergence(
            state, owned);
        bool canRebuild = ConvergenceSynchronizationSystem.CanRebuildReceiver(
            state, out string rebuildReason);

        _errorText.text = string.IsNullOrWhiteSpace(_persistentError)
            ? "" : "ERROR: " + _persistentError;
        _cycleStatus.text = "Fase: " + convergence.phase + "\n" +
            "Receptor: " + (convergence.dimensionalReceiverRebuilt ? "reconstruido" : "pendiente") +
            "\nSeñales: D1 " + Signal(state, 1) + " · D2 " + Signal(state, 2) +
            " · D3 " + Signal(state, 3) + "\nCircuitos: " + owned + "/6";
        if (convergence.phase == ConvergencePhase.Synchronizing)
            _cycleStatus.text += "\nEstabilidad: " + convergence.currentStability.ToString("0.#") +
                " / " + ConvergenceBalance.GetRequiredStabilityForNextCircuit(owned).ToString("0.#");
        if (!canRebuild && !convergence.dimensionalReceiverRebuilt &&
            convergence.phase != ConvergencePhase.Completed)
            _cycleStatus.text += "\n" + rebuildReason;
        if (ready) _cycleStatus.text += "\nREADY: la entrada de nuevas fuentes está congelada.";
        if (convergence.pendingTransaction != null)
            _cycleStatus.text += "\nTransacción pendiente: checkpoint " +
                convergence.pendingTransaction.stage + ".";
        if (!hasNext && convergence.normalConvergenceCompleted)
            _cycleStatus.text += "\nConvergencia normal completada. No existe C7.";

        _rebuildButton.interactable = canRebuild && hasNext &&
            (convergence.phase == ConvergencePhase.Inactive ||
             convergence.phase == ConvergencePhase.NewCycleStarted);
        _startButton.interactable = ready && hasNext;
        _retryTransactionButton.gameObject.SetActive(
            convergence.pendingTransaction != null);
        _retryTransactionButton.interactable = convergence.pendingTransaction != null;

        ConvergenceBoardResolution active = ConvergenceCircuitResolver.ResolveBoard(
            convergence.committedPlacements);
        ConvergenceBoardResolution draft = ConvergenceCircuitResolver.ResolveBoard(
            convergence.draftPlacements);
        _activeConfigurationText.text = "ACTIVA · " + SnapshotText(convergence.activeSnapshot) +
            " · energizados " + active.activeCircuitIds.Count + "/" + owned;
        _draftConfigurationText.text = configuring
            ? "BORRADOR · " + SnapshotText(draft.candidateSnapshot) + " · energizados " +
              draft.activeCircuitIds.Count + "/" + owned +
              (draft.structurallyValid ? "" : " · PLACA INVÁLIDA")
            : "BORRADOR · no hay edición pendiente";
        _boardStatus.text = configuring
            ? "Selecciona un circuito y una celda. Verde = ruta energizada; gris = inactivo."
            : "La Placa confirmada permanece consultable.";
        if (configuring)
            _boardStatus.text += GetCircuitTutorialText(
                convergence.nextAwardOrdinal, draft);

        ConvergenceCircuitPlacement selected = FindPlacement(
            configuring ? convergence.draftPlacements : convergence.committedPlacements,
            _selectedCircuitId);
        _rotateButton.interactable = configuring && selected != null;
        _removeButton.interactable = configuring && selected != null;
        _restoreButton.interactable = configuring;
        _confirmButton.interactable = ConvergenceCircuitSystem.CanConfirmConfiguration(
            state, out _);
        for (int i = 0; i < _inventoryButtons.Count; i++)
            _inventoryButtons[i].interactable = i < owned;
        RefreshBoard(convergence, configuring, draft);
        RefreshArchive(convergence);
        RefreshUnknownModal(convergence);
    }

    public void ConfirmFromUI()
    {
        if (ConvergenceCircuitSystem.TryConfirmConfiguration(GameState.I, out string reason))
            ClearError();
        else
            SetError(reason);
        Refresh();
    }

    public static bool TryAcknowledgeUnknownSignal(GameState state, out string reason)
    {
        if (state == null) { reason = "No hay estado de juego."; return false; }
        state.EnsureConvergenceState();
        if (state.convergence.pendingNarrativeEventId != UnknownSignalEventId ||
            state.convergence.unknownSignalAcknowledged)
        { reason = "No hay una señal desconocida pendiente."; return false; }
        state.convergence.unknownSignalAcknowledged = true;
        state.convergence.pendingNarrativeEventId = "";
        if (SaveService.I != null && !SaveService.I.TrySave(out string error))
        {
            state.convergence.unknownSignalAcknowledged = false;
            state.convergence.pendingNarrativeEventId = UnknownSignalEventId;
            reason = "No se pudo guardar el reconocimiento: " + error;
            return false;
        }
        reason = "Señal reconocida.";
        return true;
    }

    private void BuildViewTabs()
    {
        GameObject tabs = CreateHorizontal(transform, "ConvergenceViews", 40f);
        CreateButton(tabs.transform, "CICLO", () => ShowView(_cycleView), 38f);
        CreateButton(tabs.transform, "PLACA", () => ShowView(_boardView), 38f);
        CreateButton(tabs.transform, "ARCHIVO", () => ShowView(_archiveView), 38f);
    }

    private void BuildContentScroll()
    {
        GameObject scrollObject = new GameObject("ViewScroll", typeof(RectTransform),
            typeof(Image), typeof(ScrollRect), typeof(LayoutElement));
        scrollObject.transform.SetParent(transform, false);
        LayoutElement scrollLayout = scrollObject.GetComponent<LayoutElement>();
        scrollLayout.minHeight = 240f;
        scrollLayout.flexibleHeight = 1f;
        scrollObject.GetComponent<Image>().color = new Color(0.02f, 0.055f, 0.1f, 0.8f);

        GameObject viewport = new GameObject("Viewport", typeof(RectTransform),
            typeof(Image), typeof(Mask));
        viewport.transform.SetParent(scrollObject.transform, false);
        RectTransform viewportRect = viewport.GetComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = new Vector2(6f, 6f);
        viewportRect.offsetMax = new Vector2(-6f, -6f);
        viewport.GetComponent<Image>().color = Color.white;
        viewport.GetComponent<Mask>().showMaskGraphic = false;

        GameObject content = new GameObject("Content", typeof(RectTransform),
            typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        content.transform.SetParent(viewport.transform, false);
        RectTransform contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;
        VerticalLayoutGroup contentLayout = content.GetComponent<VerticalLayoutGroup>();
        contentLayout.spacing = 6f;
        contentLayout.childControlHeight = true;
        contentLayout.childControlWidth = true;
        contentLayout.childForceExpandHeight = false;
        content.GetComponent<ContentSizeFitter>().verticalFit =
            ContentSizeFitter.FitMode.PreferredSize;

        ScrollRect scroll = scrollObject.GetComponent<ScrollRect>();
        scroll.viewport = viewportRect;
        scroll.content = contentRect;
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        _viewContainer = content.transform;
    }

    private void BuildCycleView()
    {
        _cycleView = CreateVertical(_viewContainer, "CycleView", 240f);
        _cycleStatus = CreateText(_cycleView.transform, "", 17f, FontStyles.Normal, 120f);
        _rebuildButton = CreateButton(_cycleView.transform,
            "RECONSTRUIR RECEPTOR DIMENSIONAL", RebuildReceiver, 42f);
        _startButton = CreateButton(_cycleView.transform,
            "INICIAR CONVERGENCIA", StartConvergence, 42f);
        _retryTransactionButton = CreateButton(_cycleView.transform,
            "REINTENTAR TRANSACCIÓN", RetryPendingTransaction, 48f);
    }

    private void BuildBoardView()
    {
        _boardView = CreateVertical(_viewContainer, "BoardView", 520f);
        _boardStatus = CreateText(_boardView.transform, "", 15f, FontStyles.Normal, 42f);
        _activeConfigurationText = CreateText(_boardView.transform, "", 14f, FontStyles.Normal, 28f);
        _draftConfigurationText = CreateText(_boardView.transform, "", 14f, FontStyles.Normal, 28f);
        GameObject inventory = CreateHorizontal(_boardView.transform, "CircuitInventory", 38f);
        foreach (ConvergenceCircuitDefinition definition in ConvergenceCircuitCatalog.Definitions)
        {
            string id = definition.id;
            Button button = CreateButton(inventory.transform, "C" + definition.awardOrdinal,
                () => { _selectedCircuitId = id; Refresh(); }, 36f);
            button.gameObject.name = "Select_C" + definition.awardOrdinal;
            _inventoryButtons.Add(button);
        }
        BuildBoardGrid();
        GameObject actions = CreateHorizontal(_boardView.transform, "BoardActions", 40f);
        _rotateButton = CreateButton(actions.transform, "ROTAR", RotateSelected, 38f);
        _removeButton = CreateButton(actions.transform, "RETIRAR", RemoveSelected, 38f);
        _restoreButton = CreateButton(actions.transform, "RESTAURAR", RestoreDraft, 38f);
        _confirmButton = CreateButton(_boardView.transform,
            "ESTABILIZAR CONFIGURACIÓN", ConfirmFromUI, 44f);
        _confirmButton.gameObject.name = "ConfirmConvergenceConfiguration";
    }

    private void BuildArchiveView()
    {
        _archiveView = CreateVertical(_viewContainer, "ArchiveView", 320f);
        _archiveStatus = CreateText(_archiveView.transform, "", 16f, FontStyles.Normal, 220f);
    }

    private void BuildBoardGrid()
    {
        GameObject board = new GameObject("ConvergenceBoard", typeof(RectTransform),
            typeof(GridLayoutGroup), typeof(LayoutElement));
        board.transform.SetParent(_boardView.transform, false);
        board.GetComponent<LayoutElement>().preferredHeight = 236f;
        GridLayoutGroup grid = board.GetComponent<GridLayoutGroup>();
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 5;
        grid.cellSize = new Vector2(44f, 44f);
        grid.spacing = new Vector2(4f, 4f);
        grid.childAlignment = TextAnchor.MiddleCenter;
        _boardButtons = new Button[5, 5];
        for (int y = 2; y >= -2; y--)
        {
            for (int x = -2; x <= 2; x++)
            {
                int cellX = x;
                int cellY = y;
                Button cell = CreateBoardCell(board.transform, x, y);
                cell.onClick.AddListener(() => PlaceSelected(cellX, cellY));
                _boardButtons[x + 2, y + 2] = cell;
            }
        }
    }

    private void BuildUnknownModal()
    {
        _unknownModal = new GameObject("UnknownSignalModal", typeof(RectTransform),
            typeof(Image), typeof(LayoutElement));
        _unknownModal.transform.SetParent(transform, false);
        RectTransform rect = _unknownModal.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.16f, 0.25f);
        rect.anchorMax = new Vector2(0.84f, 0.75f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        _unknownModal.GetComponent<Image>().color = new Color(0.015f, 0.02f, 0.04f, 0.995f);
        _unknownModal.GetComponent<LayoutElement>().ignoreLayout = true;
        VerticalLayoutGroup layout = _unknownModal.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(28, 28, 28, 28);
        layout.spacing = 16f;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        CreateText(_unknownModal.transform, UnknownSignalTitle, 36f, FontStyles.Bold, 58f);
        CreateText(_unknownModal.transform, UnknownSignalBody, 20f, FontStyles.Normal, 100f);
        CreateButton(_unknownModal.transform, "CONTINUAR", AcknowledgeUnknown, 48f);
        CreateButton(_unknownModal.transform, "CERRAR", CloseUnknownWithoutAcknowledgement, 40f);
        _unknownModal.SetActive(false);
    }

    private void RebuildReceiver()
    {
        if (!ConvergenceSynchronizationSystem.TryRebuildReceiver(GameState.I, out string reason))
            SetError(reason);
        else
            ClearError();
        Refresh();
    }

    private void StartConvergence()
    {
        if (!ConvergenceCircuitSystem.TryStartNormalConvergence(GameState.I, out string reason))
            SetError(reason);
        else
        {
            ClearError();
            ConvergenceState state = GameState.I.convergence;
            OwnedConvergenceCircuit awarded = state.ownedCircuits.Find(c =>
                c != null && c.awardOrdinal == state.nextAwardOrdinal);
            if (awarded != null) _selectedCircuitId = awarded.circuitId;
            ShowView(_boardView);
        }
        Refresh();
    }

    private void PlaceSelected(int x, int y)
    {
        if (string.IsNullOrWhiteSpace(_selectedCircuitId))
        { SetError("Selecciona un circuito antes de colocarlo."); Refresh(); return; }
        ConvergenceCircuitPlacement current = FindPlacement(
            GameState.I?.convergence?.draftPlacements, _selectedCircuitId);
        int rotation = current == null ? 0 : current.rotationDegrees;
        if (!ConvergenceCircuitSystem.TryPlaceCircuit(GameState.I, _selectedCircuitId,
                x, y, rotation, out string reason))
            SetError(reason);
        else RefreshBoardErrorAfterCorrection();
        Refresh();
    }

    private void RotateSelected()
    {
        ConvergenceCircuitPlacement current = FindPlacement(
            GameState.I?.convergence?.draftPlacements, _selectedCircuitId);
        if (current == null)
        { SetError("El circuito seleccionado no está colocado."); Refresh(); return; }
        if (!ConvergenceCircuitSystem.TryPlaceCircuit(GameState.I, current.circuitId,
                current.x, current.y, (current.rotationDegrees + 90) % 360, out string reason))
            SetError(reason);
        else RefreshBoardErrorAfterCorrection();
        Refresh();
    }

    private void RemoveSelected()
    {
        if (!ConvergenceCircuitSystem.TryRemoveCircuit(GameState.I, _selectedCircuitId,
                out string reason)) SetError(reason); else RefreshBoardErrorAfterCorrection();
        Refresh();
    }

    private void RestoreDraft()
    {
        if (!ConvergenceCircuitSystem.TryRestoreDraft(GameState.I, out string reason))
            SetError(reason); else RefreshBoardErrorAfterCorrection();
        Refresh();
    }

    private void RetryPendingTransaction()
    {
        if (!ConvergenceCircuitSystem.TryRecoverTransaction(GameState.I,
                out string reason))
            SetError(reason);
        else
            ClearError();
        Refresh();
    }

    private void AcknowledgeUnknown()
    {
        if (!TryAcknowledgeUnknownSignal(GameState.I, out string reason))
            SetError(reason);
        else
        {
            ClearError();
            _unknownModal.SetActive(false);
        }
        Refresh();
    }

    private void CloseUnknownWithoutAcknowledgement()
    {
        _unknownModalDismissedThisOpen = true;
        _unknownModal.SetActive(false);
    }

    private void RefreshUnknownModal(ConvergenceState state)
    {
        bool pending = state.pendingNarrativeEventId == UnknownSignalEventId &&
            !state.unknownSignalAcknowledged;
        if (_unknownModal != null)
        {
            _unknownModal.SetActive(pending && !_unknownModalDismissedThisOpen);
            if (_unknownModal.activeSelf) _unknownModal.transform.SetAsLastSibling();
        }
    }

    private void RefreshArchive(ConvergenceState state)
    {
        string text = "CIRCUITOS ARCHIVADOS\n";
        foreach (ConvergenceCircuitDefinition definition in ConvergenceCircuitCatalog.Definitions)
        {
            bool owned = state.ownedCircuits.Exists(c => c != null && c.obtained &&
                c.circuitId == definition.id);
            bool active = state.activeSnapshot != null &&
                state.activeSnapshot.activeCircuitIds.Contains(definition.id);
            text += "\nC" + definition.awardOrdinal + " · " + definition.displayName +
                " · " + (owned ? "obtenido" : "no obtenido") +
                (owned ? (active ? " · activo" : " · inactivo") : "");
        }
        if (state.normalConvergenceCompleted)
            text += "\n\nConvergencia normal completada. Placa y Archivo permanecen disponibles. No existe C7.";
        _archiveStatus.text = text;
    }

    private void RefreshBoard(ConvergenceState state, bool configuring,
        ConvergenceBoardResolution draftResolution)
    {
        List<ConvergenceCircuitPlacement> placements = configuring
            ? state.draftPlacements : state.committedPlacements;
        ConvergenceBoardResolution resolution = configuring
            ? draftResolution : ConvergenceCircuitResolver.ResolveBoard(placements);
        for (int y = -2; y <= 2; y++)
        {
            for (int x = -2; x <= 2; x++)
            {
                Button cell = _boardButtons[x + 2, y + 2];
                bool core = x == 0 && y == 0;
                ConvergenceCircuitPlacement placement = placements.Find(p =>
                    p != null && p.x == x && p.y == y);
                TMP_Text label = cell.GetComponentInChildren<TMP_Text>();
                Image image = cell.GetComponent<Image>();
                if (core)
                {
                    label.text = "◆";
                    image.color = new Color(0.82f, 0.54f, 0.16f, 1f);
                }
                else if (placement == null)
                {
                    label.text = "·";
                    image.color = new Color(0.08f, 0.14f, 0.22f, 1f);
                }
                else
                {
                    ConvergenceCircuitDefinition definition =
                        ConvergenceCircuitCatalog.Get(placement.circuitId);
                    int mask = ConvergenceCircuitCatalog.RotateMask(
                        definition.basePortMask, placement.rotationDegrees);
                    string glyph = PortGlyph(mask);
                    if (definition.isAmplifier)
                    {
                        int arrow = ConvergenceCircuitCatalog.RotateMask(
                            definition.baseTargetDirectionMask, placement.rotationDegrees);
                        glyph += ArrowGlyph(arrow);
                    }
                    bool active = resolution.activeCircuitIds.Contains(placement.circuitId);
                    label.text = glyph;
                    image.color = active
                        ? new Color(0.08f, 0.46f, 0.34f, 1f)
                        : new Color(0.26f, 0.29f, 0.34f, 1f);
                }
                cell.interactable = configuring && !core;
            }
        }
    }

    private void ShowView(GameObject view)
    {
        if (_cycleView != null) _cycleView.SetActive(view == _cycleView);
        if (_boardView != null) _boardView.SetActive(view == _boardView);
        if (_archiveView != null) _archiveView.SetActive(view == _archiveView);
        Refresh();
    }

    private void SetError(string reason)
    {
        _persistentError = reason ?? "Error desconocido.";
        Debug.LogWarning("[Convergence UI] " + _persistentError);
    }

    private void ClearError() { _persistentError = ""; }

    private void RefreshBoardErrorAfterCorrection()
    {
        if (string.IsNullOrWhiteSpace(_persistentError)) return;
        if (ConvergenceCircuitSystem.CanConfirmConfiguration(GameState.I,
                out string remainingReason))
            ClearError();
        else
            _persistentError = remainingReason;
    }

    private Button CreateBoardCell(Transform parent, int x, int y)
    {
        GameObject cell = new GameObject("Cell_" + x + "_" + y,
            typeof(RectTransform), typeof(Image), typeof(Button));
        cell.transform.SetParent(parent, false);
        Button button = cell.GetComponent<Button>();
        button.targetGraphic = cell.GetComponent<Image>();
        CreateText(cell.transform, "", 22f, FontStyles.Normal, 44f, false);
        return button;
    }

    private GameObject CreateVertical(Transform parent, string name,
        float preferredHeight)
    {
        GameObject result = new GameObject(name, typeof(RectTransform),
            typeof(VerticalLayoutGroup), typeof(LayoutElement));
        result.transform.SetParent(parent, false);
        result.GetComponent<LayoutElement>().preferredHeight = preferredHeight;
        VerticalLayoutGroup layout = result.GetComponent<VerticalLayoutGroup>();
        layout.spacing = 6f;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        return result;
    }

    private GameObject CreateHorizontal(Transform parent, string name, float height)
    {
        GameObject result = new GameObject(name, typeof(RectTransform),
            typeof(HorizontalLayoutGroup), typeof(LayoutElement));
        result.transform.SetParent(parent, false);
        float touchHeight = Mathf.Max(44f, height);
        LayoutElement element = result.GetComponent<LayoutElement>();
        element.minHeight = touchHeight;
        element.preferredHeight = touchHeight;
        HorizontalLayoutGroup layout = result.GetComponent<HorizontalLayoutGroup>();
        layout.spacing = 6f;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandWidth = true;
        return result;
    }

    private Button CreateButton(Transform parent, string label, UnityAction action,
        float height)
    {
        GameObject buttonObject = new GameObject(label, typeof(RectTransform),
            typeof(Image), typeof(Button), typeof(LayoutElement));
        buttonObject.transform.SetParent(parent, false);
        float touchHeight = Mathf.Max(44f, height);
        LayoutElement element = buttonObject.GetComponent<LayoutElement>();
        element.minHeight = touchHeight;
        element.preferredHeight = touchHeight;
        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.11f, 0.29f, 0.48f, 1f);
        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);
        CreateText(buttonObject.transform, label, 15f, FontStyles.Bold,
            touchHeight, false);
        return button;
    }

    public static string GetCircuitTutorialText(int awardOrdinal,
        ConvergenceBoardResolution resolution)
    {
        if (awardOrdinal == 2)
        {
            bool complete = resolution != null &&
                resolution.activeCircuitIds.Contains("convergence_circuit_001_startup_pulse") &&
                resolution.circuits.Exists(c => c != null &&
                    c.circuitId == "convergence_circuit_002_axial_link" &&
                    c.energized && c.degree >= 2);
            return "\nTutorial C2 sugerido: Núcleo → C2 Enlace Axial → C1 Pulso de Arranque. " +
                (complete ? "Disposición completada." : "Conecta C2 entre el Núcleo y C1.");
        }
        if (awardOrdinal == 4)
        {
            bool complete = resolution != null && resolution.circuits.Exists(c =>
                c != null && c.circuitId == "convergence_circuit_004_triaxial_distributor" &&
                c.energized && c.degree >= 3);
            return "\nTutorial C4 sugerido: usa la T del Distribuidor para mantener dos ramas " +
                "energizadas. " + (complete ? "Bifurcación completada." :
                    "Conecta el Núcleo y dos circuitos a la T.");
        }
        return "";
    }

    private TextMeshProUGUI CreateText(Transform parent, string text, float size,
        FontStyles style, float height, bool layoutElement = true)
    {
        GameObject textObject = new GameObject("Label", typeof(RectTransform),
            typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        if (layoutElement)
            textObject.AddComponent<LayoutElement>().preferredHeight = height;
        TextMeshProUGUI label = textObject.GetComponent<TextMeshProUGUI>();
        label.text = text;
        label.font = _font;
        label.fontSize = size;
        label.fontStyle = style;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;
        return label;
    }

    private static string SnapshotText(ConvergenceModifierSnapshot snapshot)
    {
        snapshot ??= ConvergenceSystem.CreateDefaultSnapshot();
        return "LE ×" + snapshot.baseLEProductionMultiplier.ToString("0.00") +
            " · Trazas ×" + snapshot.baseTracesProductionMultiplier.ToString("0.00") +
            " · Estabilidad ×" + snapshot.stabilityGainMultiplier.ToString("0.00");
    }

    private static string Signal(GameState state, int dimensionId) =>
        ConvergenceSynchronizationSystem.IsSignalActivated(state, dimensionId)
            ? "activa" : "pendiente";

    private static ConvergenceCircuitPlacement FindPlacement(
        List<ConvergenceCircuitPlacement> placements, string circuitId) =>
        placements == null || string.IsNullOrWhiteSpace(circuitId)
            ? null : placements.Find(p => p != null && p.circuitId == circuitId);

    private static string ArrowGlyph(int direction)
    {
        if (direction == ConvergenceCircuitCatalog.North) return "↑";
        if (direction == ConvergenceCircuitCatalog.East) return "→";
        if (direction == ConvergenceCircuitCatalog.South) return "↓";
        return "←";
    }

    private static string PortGlyph(int mask)
    {
        switch (mask)
        {
            case ConvergenceCircuitCatalog.North: return "╵";
            case ConvergenceCircuitCatalog.East: return "╴";
            case ConvergenceCircuitCatalog.South: return "╷";
            case ConvergenceCircuitCatalog.West: return "╶";
            case ConvergenceCircuitCatalog.North | ConvergenceCircuitCatalog.South: return "│";
            case ConvergenceCircuitCatalog.East | ConvergenceCircuitCatalog.West: return "─";
            case ConvergenceCircuitCatalog.North | ConvergenceCircuitCatalog.East: return "└";
            case ConvergenceCircuitCatalog.East | ConvergenceCircuitCatalog.South: return "┌";
            case ConvergenceCircuitCatalog.South | ConvergenceCircuitCatalog.West: return "┐";
            case ConvergenceCircuitCatalog.West | ConvergenceCircuitCatalog.North: return "┘";
            case ConvergenceCircuitCatalog.North | ConvergenceCircuitCatalog.East | ConvergenceCircuitCatalog.West: return "┴";
            case ConvergenceCircuitCatalog.East | ConvergenceCircuitCatalog.South | ConvergenceCircuitCatalog.West: return "┬";
            case ConvergenceCircuitCatalog.North | ConvergenceCircuitCatalog.South | ConvergenceCircuitCatalog.West: return "┤";
            case ConvergenceCircuitCatalog.North | ConvergenceCircuitCatalog.East | ConvergenceCircuitCatalog.South: return "├";
            case ConvergenceCircuitCatalog.North | ConvergenceCircuitCatalog.East | ConvergenceCircuitCatalog.South | ConvergenceCircuitCatalog.West: return "┼";
            default: return "?";
        }
    }
}

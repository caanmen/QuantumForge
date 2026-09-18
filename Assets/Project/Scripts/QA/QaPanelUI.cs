using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QaPanelUI : MonoBehaviour
{
    public RectTransform safeAreaRoot;
    public Button toolsButton;
    public GameObject panelRoot;
    public ScrollRect scrollRect;
    public TMP_Text speedStatusText;
    public TMP_Text operationStatusText;
    public Button[] speedButtons;
    public Button[] advanceButtons;
    public Button[] checkpointSaveButtons;
    public Button[] checkpointLoadButtons;
    public TMP_Text[] checkpointStatusTexts;
    public Button resetSaveButton;
    public Button closeButton;

    public GameObject confirmationRoot;
    public TMP_Text confirmationText;
    public Button confirmationAcceptButton;
    public Button confirmationCancelButton;

    public event Action<double> AdvanceRequested;
    public event Action<char> SaveCheckpointRequested;
    public event Action<char> LoadCheckpointRequested;
    public event Action ResetSaveRequested;

    public bool IsOpen => panelRoot != null && panelRoot.activeSelf;

    private enum PendingConfirmation
    {
        None,
        AdvanceTime,
        LoadCheckpoint,
        ResetSave
    }

    private PendingConfirmation pendingConfirmation;
    private double pendingAdvanceSeconds;
    private char pendingCheckpointSlot;
    private int lastScreenWidth = -1;
    private int lastScreenHeight = -1;
    private Rect lastSafeArea;
    private bool qaControlsInteractable = true;
    private readonly bool[] checkpointLoadAvailable = { true, true, true };

    private void Awake()
    {
        WireListeners();
        if (panelRoot != null)
            panelRoot.SetActive(false);
        HideConfirmation();
        ApplyAvailability();
        RefreshSpeed(QaRuntimeService.SimulationMultiplier);
        ApplySafeArea();
    }

    private void OnEnable()
    {
        QaRuntimeService.SpeedChanged -= OnSpeedChanged;
        QaRuntimeService.SpeedChanged += OnSpeedChanged;
        WireListeners();
        ApplyAvailability();
        RefreshSpeed(QaRuntimeService.SimulationMultiplier);
        ApplySafeArea();
    }

    private void OnDisable()
    {
        QaRuntimeService.SpeedChanged -= OnSpeedChanged;
    }

    private void OnDestroy()
    {
        QaRuntimeService.SpeedChanged -= OnSpeedChanged;
        UnwireListeners();
    }

    private void Update()
    {
        if (lastScreenWidth != Screen.width ||
            lastScreenHeight != Screen.height ||
            lastSafeArea != Screen.safeArea)
        {
            ApplySafeArea();
        }

        Keyboard keyboard = Keyboard.current;
        HandleBackRequest(keyboard != null &&
            keyboard.escapeKey.wasPressedThisFrame);
    }

    public void TogglePanel()
    {
        if (!QaRuntimeService.IsAvailable)
            return;

        if (IsOpen)
            ClosePanel();
        else
            OpenPanel();
    }

    public void OpenPanel()
    {
        if (!QaRuntimeService.IsAvailable || panelRoot == null)
            return;

        panelRoot.SetActive(true);
        HideConfirmation();
        RefreshSpeed(QaRuntimeService.SimulationMultiplier);
        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
    }

    public void ClosePanel()
    {
        HideConfirmation();
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    private void HandleBackRequest(bool backRequested)
    {
        if (!backRequested || !IsOpen)
            return;

        if (confirmationRoot != null && confirmationRoot.activeSelf)
            HideConfirmation();
        else
            ClosePanel();
    }

    public void SetQaControlsInteractable(bool interactable)
    {
        qaControlsInteractable = interactable;
        if (toolsButton != null)
            toolsButton.interactable = interactable;
        SetButtonsInteractable(speedButtons, interactable);
        SetButtonsInteractable(advanceButtons, interactable);
        SetButtonsInteractable(checkpointSaveButtons, interactable);
        ApplyCheckpointLoadAvailability();
        if (resetSaveButton != null)
            resetSaveButton.interactable = interactable;
        // CERRAR es navegación, no una mutación del estado. Debe seguir
        // disponible aunque un avance QA esté guardando o calculando recursos.
        if (closeButton != null)
            closeButton.interactable = true;
    }

    public void SetOperationStatus(string status)
    {
        if (operationStatusText != null)
            operationStatusText.SetText(status ?? "");
    }

    public void SetCheckpointSlotStatus(
        int index, string status, bool loadAvailable)
    {
        if (index < 0 || index >= checkpointLoadAvailable.Length)
            return;

        checkpointLoadAvailable[index] = loadAvailable;
        TMP_Text label = Get(checkpointStatusTexts, index);
        if (label != null)
        {
            string visibleStatus = string.IsNullOrWhiteSpace(status)
                ? "VACÍO"
                : status;
            label.SetText(visibleStatus);
            label.color = visibleStatus.StartsWith(
                    "GUARDADO", StringComparison.OrdinalIgnoreCase)
                ? new Color(0.45f, 0.90f, 0.64f)
                : visibleStatus.StartsWith(
                    "NO LEGIBLE", StringComparison.OrdinalIgnoreCase)
                    ? new Color(1f, 0.45f, 0.42f)
                    : new Color(0.60f, 0.68f, 0.76f);
        }

        ApplyCheckpointLoadAvailability();
    }

    public void ApplySafeArea()
    {
        if (safeAreaRoot == null || Screen.width <= 0 || Screen.height <= 0)
            return;

        Rect safeArea = Screen.safeArea;
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        safeAreaRoot.anchorMin = anchorMin;
        safeAreaRoot.anchorMax = anchorMax;
        safeAreaRoot.offsetMin = Vector2.zero;
        safeAreaRoot.offsetMax = Vector2.zero;

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
        lastSafeArea = safeArea;
    }

    private void ApplyAvailability()
    {
        if (safeAreaRoot == null)
            return;

        bool available = QaRuntimeService.IsAvailable;
        if (!available)
        {
            ClosePanel();
            safeAreaRoot.gameObject.SetActive(false);
        }
    }

    private void WireListeners()
    {
        Wire(toolsButton, TogglePanel);
        Wire(closeButton, ClosePanel);
        Wire(confirmationAcceptButton, AcceptConfirmation);
        Wire(confirmationCancelButton, HideConfirmation);

        Wire(Get(speedButtons, 0), SetSpeed1);
        Wire(Get(speedButtons, 1), SetSpeed5);
        Wire(Get(speedButtons, 2), SetSpeed10);
        Wire(Get(speedButtons, 3), SetSpeed20);

        Wire(Get(advanceButtons, 0), RequestAdvanceFiveMinutes);
        Wire(Get(advanceButtons, 1), RequestAdvanceThirtyMinutes);
        Wire(Get(advanceButtons, 2), RequestAdvanceOneHour);
        Wire(Get(advanceButtons, 3), RequestAdvanceEightHours);
        Wire(Get(advanceButtons, 4), RequestAdvanceTwelveHours);
        Wire(Get(advanceButtons, 5), RequestAdvanceTwentyFourHours);

        Wire(Get(checkpointSaveButtons, 0), SaveCheckpointA);
        Wire(Get(checkpointSaveButtons, 1), SaveCheckpointB);
        Wire(Get(checkpointSaveButtons, 2), SaveCheckpointC);
        Wire(Get(checkpointLoadButtons, 0), LoadCheckpointA);
        Wire(Get(checkpointLoadButtons, 1), LoadCheckpointB);
        Wire(Get(checkpointLoadButtons, 2), LoadCheckpointC);
        Wire(resetSaveButton, RequestResetSave);
    }

    private void UnwireListeners()
    {
        Unwire(toolsButton, TogglePanel);
        Unwire(closeButton, ClosePanel);
        Unwire(confirmationAcceptButton, AcceptConfirmation);
        Unwire(confirmationCancelButton, HideConfirmation);

        Unwire(Get(speedButtons, 0), SetSpeed1);
        Unwire(Get(speedButtons, 1), SetSpeed5);
        Unwire(Get(speedButtons, 2), SetSpeed10);
        Unwire(Get(speedButtons, 3), SetSpeed20);

        Unwire(Get(advanceButtons, 0), RequestAdvanceFiveMinutes);
        Unwire(Get(advanceButtons, 1), RequestAdvanceThirtyMinutes);
        Unwire(Get(advanceButtons, 2), RequestAdvanceOneHour);
        Unwire(Get(advanceButtons, 3), RequestAdvanceEightHours);
        Unwire(Get(advanceButtons, 4), RequestAdvanceTwelveHours);
        Unwire(Get(advanceButtons, 5), RequestAdvanceTwentyFourHours);

        Unwire(Get(checkpointSaveButtons, 0), SaveCheckpointA);
        Unwire(Get(checkpointSaveButtons, 1), SaveCheckpointB);
        Unwire(Get(checkpointSaveButtons, 2), SaveCheckpointC);
        Unwire(Get(checkpointLoadButtons, 0), LoadCheckpointA);
        Unwire(Get(checkpointLoadButtons, 1), LoadCheckpointB);
        Unwire(Get(checkpointLoadButtons, 2), LoadCheckpointC);
        Unwire(resetSaveButton, RequestResetSave);
    }

    private void SetSpeed1() => QaRuntimeService.TrySetSpeed(1f);
    private void SetSpeed5() => QaRuntimeService.TrySetSpeed(5f);
    private void SetSpeed10() => QaRuntimeService.TrySetSpeed(10f);
    private void SetSpeed20() => QaRuntimeService.TrySetSpeed(20f);

    private void RequestAdvanceFiveMinutes() => AdvanceRequested?.Invoke(300.0);
    private void RequestAdvanceThirtyMinutes() => AdvanceRequested?.Invoke(1800.0);

    private void RequestAdvanceOneHour() => RequestConfirmedAdvance(3600.0);
    private void RequestAdvanceEightHours() => RequestConfirmedAdvance(28800.0);
    private void RequestAdvanceTwelveHours() => RequestConfirmedAdvance(43200.0);
    private void RequestAdvanceTwentyFourHours() => RequestConfirmedAdvance(86400.0);

    private void RequestConfirmedAdvance(double seconds)
    {
        pendingConfirmation = PendingConfirmation.AdvanceTime;
        pendingAdvanceSeconds = seconds;
        ShowConfirmation("¿Aplicar +" + FormatDuration(seconds) +
            " mediante progreso de ausencia QA?");
    }

    private void SaveCheckpointA() => SaveCheckpointRequested?.Invoke('A');
    private void SaveCheckpointB() => SaveCheckpointRequested?.Invoke('B');
    private void SaveCheckpointC() => SaveCheckpointRequested?.Invoke('C');
    private void LoadCheckpointA() => RequestLoadCheckpoint('A');
    private void LoadCheckpointB() => RequestLoadCheckpoint('B');
    private void LoadCheckpointC() => RequestLoadCheckpoint('C');

    private void RequestLoadCheckpoint(char slot)
    {
        pendingConfirmation = PendingConfirmation.LoadCheckpoint;
        pendingCheckpointSlot = slot;
        ShowConfirmation("¿Cargar el checkpoint " + slot +
            "? El estado actual será reemplazado.");
    }

    private void RequestResetSave()
    {
        pendingConfirmation = PendingConfirmation.ResetSave;
        ShowConfirmation("¿BORRAR TODA LA PARTIDA Y EMPEZAR DE NUEVO? " +
            "Esta acción no se puede deshacer. " +
            "Los checkpoints QA A/B/C se conservarán.");
    }

    private void AcceptConfirmation()
    {
        PendingConfirmation accepted = pendingConfirmation;
        char slot = pendingCheckpointSlot;
        double advanceSeconds = pendingAdvanceSeconds;
        HideConfirmation();

        if (accepted == PendingConfirmation.AdvanceTime)
            AdvanceRequested?.Invoke(advanceSeconds);
        else if (accepted == PendingConfirmation.LoadCheckpoint)
            LoadCheckpointRequested?.Invoke(slot);
        else if (accepted == PendingConfirmation.ResetSave)
            ResetSaveRequested?.Invoke();
    }

    private void ShowConfirmation(string message)
    {
        if (confirmationText != null)
            confirmationText.SetText(message);
        if (confirmationRoot != null)
            confirmationRoot.SetActive(true);
    }

    private void HideConfirmation()
    {
        pendingConfirmation = PendingConfirmation.None;
        pendingCheckpointSlot = '\0';
        pendingAdvanceSeconds = 0.0;
        if (confirmationRoot != null)
            confirmationRoot.SetActive(false);
    }

    private static string FormatDuration(double seconds)
    {
        if (seconds >= 3600.0 && seconds % 3600.0 == 0.0)
            return (seconds / 3600.0).ToString("0") + " H";
        if (seconds >= 60.0 && seconds % 60.0 == 0.0)
            return (seconds / 60.0).ToString("0") + " MIN";
        return seconds.ToString("0.#") + " S";
    }

    private void OnSpeedChanged(float multiplier)
    {
        RefreshSpeed(multiplier);
    }

    private void RefreshSpeed(float multiplier)
    {
        if (speedStatusText != null)
            speedStatusText.SetText("VELOCIDAD ACTUAL: QA x" +
                multiplier.ToString("0"));
    }

    private static Button Get(Button[] buttons, int index)
    {
        return buttons != null && index >= 0 && index < buttons.Length
            ? buttons[index]
            : null;
    }

    private static TMP_Text Get(TMP_Text[] labels, int index)
    {
        return labels != null && index >= 0 && index < labels.Length
            ? labels[index]
            : null;
    }

    private void ApplyCheckpointLoadAvailability()
    {
        if (checkpointLoadButtons == null)
            return;

        for (int index = 0; index < checkpointLoadButtons.Length; index++)
        {
            Button button = checkpointLoadButtons[index];
            if (button == null)
                continue;
            bool slotAvailable = index < checkpointLoadAvailable.Length &&
                checkpointLoadAvailable[index];
            button.interactable = qaControlsInteractable && slotAvailable;
        }
    }

    private static void Wire(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button == null)
            return;
        button.onClick.RemoveListener(action);
        button.onClick.AddListener(action);
    }

    private static void Unwire(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null)
            button.onClick.RemoveListener(action);
    }

    private static void SetButtonsInteractable(Button[] buttons, bool interactable)
    {
        if (buttons == null)
            return;
        foreach (Button button in buttons)
        {
            if (button != null)
                button.interactable = interactable;
        }
    }
}

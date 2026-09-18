using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class PrestigeMachineScreenUI : MonoBehaviour
{
    private const float RefreshInterval = .25f;

    [Header("Integración")]
    [SerializeField] private PrestigeUI prestigeUI;
    [SerializeField] private MachinePanelUI machinePanel;

    [Header("Navegación")]
    [SerializeField] private Button nodesButton;
    [SerializeField] private Button mixesButton;
    [SerializeField] private Button prestigeButton;
    [SerializeField] private Button actionButton;

    [Header("Cabecera")]
    [SerializeField] private TextMeshProUGUI leText;
    [SerializeField] private TextMeshProUGUI tracesText;
    [SerializeField] private TextMeshProUGUI totalProgressText;
    [SerializeField] private TextMeshProUGUI channelText;
    [SerializeField] private Image totalProgressFill;
    [SerializeField] private TextMeshProUGUI statusBadgeText;

    [Header("Estado de Prestigio")]
    [SerializeField] private TextMeshProUGUI repairValueText;
    [SerializeField] private Image repairProgressFill;
    [SerializeField] private TextMeshProUGUI repairRequirementText;
    [SerializeField] private Graphic repairRequirementPanel;
    [SerializeField] private TextMeshProUGUI repairRequirementStateText;
    [SerializeField] private TextMeshProUGUI channelRequirementText;
    [SerializeField] private Graphic channelRequirementPanel;
    [SerializeField] private TextMeshProUGUI channelRequirementStateText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI actionText;
    [SerializeField] private RawImage overviewArtwork;

    private float _nextRefresh;

    private static readonly Color Cyan = Hex("5ADFFF");
    private static readonly Color Purple = Hex("A94CFF");
    private static readonly Color ReadyPanel = Hex("123740", 245);
    private static readonly Color LockedPanel = Hex("161F26", 245);
    private static readonly Color ReadyText = Hex("E8EAEC");
    private static readonly Color LockedText = Hex("87929B");
    private static readonly Color ActionReadyPanel = Hex("472461", 248);

    private static readonly Rect[] OverviewUvRects =
    {
        new Rect(0f, .5f, .5f, .5f),
        new Rect(.5f, .5f, .5f, .5f),
        new Rect(0f, 0f, .5f, .5f),
        new Rect(.5f, 0f, .5f, .5f)
    };

    private void Awake()
    {
        if (prestigeUI == null)
            prestigeUI = GetComponentInParent<PrestigeUI>(true);
        if (machinePanel == null)
            machinePanel = FindFirstObjectByType<MachinePanelUI>(FindObjectsInactive.Include);

        nodesButton?.onClick.AddListener(ShowNodes);
        mixesButton?.onClick.AddListener(ShowMixes);
        actionButton?.onClick.AddListener(ActivatePrestige);
    }

    private void OnEnable()
    {
        _nextRefresh = 0f;
        RefreshNow();
    }

    private void Update()
    {
        if (Time.unscaledTime < _nextRefresh)
            return;

        _nextRefresh = Time.unscaledTime + RefreshInterval;
        RefreshNow();
    }

    public void RefreshNow()
    {
        GameState state = GameState.I;
        MachineManager machine = MachineManager.I;
        if (state == null || machine == null)
            return;

        float repair = Mathf.Clamp01((float)machine.GetTotalMachineRepairProgress01());
        bool repairReady = machine.HasEnoughRepairForPrestige1();
        bool canPrestige = state.CanDoPrestige1(machine);
        bool canOpenConvergence = ConvergenceCircuitSystem.IsConvergenceUnlocked(state);

        SetText(leText, "LE  " + FormatNumber(state.LE));
        SetText(tracesText, "TRAZAS  " + FormatNumber(state.Traces));
        SetText(totalProgressText, $"REPARACIÓN TOTAL  {repair * 100f:0}%  /  80%");
        if (channelText != null)
            channelText.gameObject.SetActive(false);
        if (totalProgressFill != null)
            totalProgressFill.fillAmount = Mathf.Clamp01(repair / .8f);

        SetText(statusBadgeText,
            canOpenConvergence ? "CONVERGENCIA" : canPrestige ? "LISTO" : "BLOQUEADO");
        if (statusBadgeText != null)
        {
            statusBadgeText.color = canPrestige || canOpenConvergence ? ReadyText : LockedText;
            SetBadgeStyle(canPrestige || canOpenConvergence);
        }

        SetText(repairValueText, $"{repair * 100f:0}%");
        if (repairProgressFill != null)
            repairProgressFill.fillAmount = repair;
        SetRequirement(repairRequirementPanel, repairRequirementText,
            repairRequirementStateText, repairReady, "REPARACIÓN AL 80%",
            "LISTO", "PENDIENTE");
        if (channelRequirementPanel != null)
            channelRequirementPanel.gameObject.SetActive(false);
        SetText(statusText, state.GetPrestige1StatusText());

        bool actionAvailable = canPrestige || canOpenConvergence;
        if (actionButton != null)
        {
            actionButton.interactable = actionAvailable;
            if (actionButton.targetGraphic is PrestigeChamferedPanelGraphic actionPanel)
            {
                actionPanel.FillColor = actionAvailable ? ActionReadyPanel : LockedPanel;
                actionPanel.BorderColor = actionAvailable ? Purple : LockedText;
            }
            else if (actionButton.targetGraphic is Image actionImage)
                actionImage.color = actionAvailable ? ActionReadyPanel : LockedPanel;
        }
        SetText(actionText, canOpenConvergence
            ? "ABRIR CONVERGENCIA"
            : canPrestige ? "INICIAR PRESTIGIO" : "REQUISITOS PENDIENTES");

        if (overviewArtwork != null)
            overviewArtwork.uvRect = OverviewUvRects[GetOverviewStage(repair)];

        if (prestigeButton != null)
            prestigeButton.interactable = false;
    }

#if UNITY_EDITOR
    public void PreviewForCapture(float repair, bool channelReady, bool canPrestige)
    {
        _ = channelReady;
        repair = Mathf.Clamp01(repair);
        SetText(leText, "LE  1000 T");
        SetText(tracesText, "TRAZAS  1 T");
        SetText(totalProgressText, $"REPARACIÓN TOTAL  {repair * 100f:0}%  /  80%");
        if (channelText != null)
            channelText.gameObject.SetActive(false);
        if (totalProgressFill != null)
            totalProgressFill.fillAmount = Mathf.Clamp01(repair / .8f);
        SetText(statusBadgeText, canPrestige ? "LISTO" : "BLOQUEADO");
        if (statusBadgeText != null)
        {
            statusBadgeText.color = canPrestige ? ReadyText : LockedText;
            SetBadgeStyle(canPrestige);
        }
        SetText(repairValueText, $"{repair * 100f:0}%");
        if (repairProgressFill != null)
            repairProgressFill.fillAmount = repair;
        SetRequirement(repairRequirementPanel, repairRequirementText,
            repairRequirementStateText, repair >= .8f, "REPARACIÓN AL 80%",
            "LISTO", "PENDIENTE");
        if (channelRequirementPanel != null)
            channelRequirementPanel.gameObject.SetActive(false);
        SetText(statusText, canPrestige
            ? "Prestigio 1 disponible. Elige una Firma Dimensional."
            : $"Reparación de Máquina: {repair * 100f:0}% / 80%");
        if (actionButton != null)
        {
            actionButton.interactable = canPrestige;
            if (actionButton.targetGraphic is PrestigeChamferedPanelGraphic actionPanel)
            {
                actionPanel.FillColor = canPrestige ? ActionReadyPanel : LockedPanel;
                actionPanel.BorderColor = canPrestige ? Purple : LockedText;
            }
            else if (actionButton.targetGraphic is Image actionImage)
                actionImage.color = canPrestige ? ActionReadyPanel : LockedPanel;
        }
        SetText(actionText, canPrestige ? "INICIAR PRESTIGIO" : "REQUISITOS PENDIENTES");
        if (overviewArtwork != null)
            overviewArtwork.uvRect = OverviewUvRects[GetOverviewStage(repair)];
    }
#endif

    private void ShowNodes()
    {
        machinePanel?.ShowNodesFromPrestige();
    }

    private void ShowMixes()
    {
        machinePanel?.ShowMixesFromPrestige();
    }

    private void ActivatePrestige()
    {
        prestigeUI?.OnClickPrestige();
    }

    private static void SetRequirement(Graphic panel,
        TextMeshProUGUI label, TextMeshProUGUI stateLabel, bool complete,
        string value, string completedState, string pendingState)
    {
        if (panel != null)
        {
            if (panel is PrestigeChamferedPanelGraphic chamfered)
            {
                chamfered.FillColor = complete ? ReadyPanel : LockedPanel;
                chamfered.BorderColor = complete ? Cyan : Hex("223C4B");
            }
            else
                panel.color = complete ? ReadyPanel : LockedPanel;
            Transform dot = panel.transform.Find("StateDot");
            Graphic dotGraphic = dot != null ? dot.GetComponent<Graphic>() : null;
            if (dotGraphic != null)
                dotGraphic.color = complete ? Cyan : Hex("EBA03C");
        }
        SetText(label, value);
        if (label != null)
            label.color = ReadyText;
        SetText(stateLabel, complete ? completedState : pendingState);
        if (stateLabel != null)
            stateLabel.color = complete ? Cyan : Hex("EFA64C");
    }

    private void SetBadgeStyle(bool ready)
    {
        if (statusBadgeText == null)
            return;
        Graphic panel = statusBadgeText.transform.parent.GetComponent<Graphic>();
        if (panel == null)
            return;
        if (panel is PrestigeChamferedPanelGraphic chamfered)
        {
            chamfered.FillColor = ready ? Hex("211331", 242) : Hex("271710", 235);
            chamfered.BorderColor = ready ? Purple : Hex("E99936");
        }
        else
            panel.color = ready ? Hex("4A2670", 242) : Hex("5A351B", 242);
    }

    private static int GetOverviewStage(float progress)
    {
        if (progress < .20f) return 0;
        if (progress < .40f) return 1;
        if (progress < .60f) return 2;
        return 3;
    }

    private static string FormatNumber(double value)
    {
        if (value >= 1e12) return (value / 1e12).ToString("0.##") + "T";
        if (value >= 1e9) return (value / 1e9).ToString("0.##") + "B";
        if (value >= 1e6) return (value / 1e6).ToString("0.##") + "M";
        if (value >= 1e3) return (value / 1e3).ToString("0.##") + "K";
        return value.ToString("0.##");
    }

    private static void SetText(TMP_Text target, string value)
    {
        if (target != null)
            target.text = value;
    }

    private static Color Hex(string rgb, byte alpha = 255)
    {
        ColorUtility.TryParseHtmlString("#" + rgb, out Color color);
        color.a = alpha / 255f;
        return color;
    }
}

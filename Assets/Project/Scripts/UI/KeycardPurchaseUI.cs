using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeycardPurchaseUI : MonoBehaviour
{
    private const float RefreshInterval = 0.25f;

    [Header("Referencias UI")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI buttonText;
    public Button buyButton;
    private float nextRefreshTime;

    private void Awake()
    {
        AutoBindIfNeeded();
    }

    private void Update()
    {
        if (Time.unscaledTime < nextRefreshTime)
            return;
        nextRefreshTime = Time.unscaledTime + RefreshInterval;
        RefreshUI();
    }

    public void OnClickBuyKeycard()
    {
        GameState state = GameState.I;
        if (state == null) return;

        if (!UpgradeStudySystem.IsKeycardProjectDiscovered(state))
        {
            UpgradeStudyDef study = UpgradeStudySystem.GetStudyForUnlock(
                UpgradeStudySystem.KeycardProjectUnlockId);
            bool changed = UpgradeStudySystem.IsConclusionPending(state) &&
                UpgradeStudySystem.IsStudyActiveForUnlock(state,
                    UpgradeStudySystem.KeycardProjectUnlockId)
                ? UpgradeStudySystem.TryRevealConclusion(state, out _)
                : study != null && UpgradeStudySystem.TryStartStudy(state, study.id);
            if (changed) SaveService.I?.Save();
            RefreshUI();
            return;
        }

        bool bought = state.TryBuyExperimentalChamberKeycard();
        Debug.Log($"[F3] Comprar Keycard => {bought}");

        if (bought)
        {
            TabsUI.Instance?.RefreshRoom2ButtonVisibility();
            TabsUI.Instance?.ShowRoom2();
        }

        RefreshUI();
    }

    private void RefreshUI()
    {
        GameState state = GameState.I;
        if (state == null) return;

        bool unlocked = state.experimentalChamberUnlocked;
        bool hasArtifacts = state.HasExperimentalChamberArtifactRequirements();
        bool hasTriangle = state.HasExperimentalChamberTriangleRequirement();
        bool canBuy = state.CanBuyExperimentalChamberKeycard();
        bool discovered = UpgradeStudySystem.IsKeycardProjectDiscovered(state);
        bool active = UpgradeStudySystem.IsStudyActiveForUnlock(state,
            UpgradeStudySystem.KeycardProjectUnlockId);
        bool conclusion = active && UpgradeStudySystem.IsConclusionPending(state);
        UpgradeStudyDef study = UpgradeStudySystem.GetStudyForUnlock(
            UpgradeStudySystem.KeycardProjectUnlockId);

        if (nameText != null)
            nameText.text = discovered
                ? L("keycard.name", "Keycard de la Cámara Experimental")
                : L("study.study_experimental_chamber_keycard.title",
                    "Proyecto de acceso experimental");

        if (descText != null)
        {
            if (!discovered)
            {
                if (active)
                {
                    double progress = UpgradeStudySystem.GetActiveProgress01(state);
                    string remaining = FormatDuration(
                        UpgradeStudySystem.GetRemainingSeconds(state));
                    descText.text = conclusion
                        ? L("study.status.conclusion", "Conclusión disponible")
                        : LF("study.progress", "Progreso: {0:0}% · {1}",
                            progress * 100.0, remaining);
                }
                else
                {
                    descText.text = L("study.study_experimental_chamber_keycard.hint",
                        "Compara los resultados de Energía y Experimental para descubrir la keycard.");
                }
            }
            else if (unlocked)
            {
                descText.text = L("keycard.open_desc",
                    "Acceso habilitado al Cuarto 2.");
            }
            else if (!hasArtifacts)
            {
                descText.text = L("keycard.needs_artifacts",
                    "Requiere los tres artefactos.");
            }
            else if (!hasTriangle)
            {
                descText.text = L("keycard.needs_triangle",
                    "Activa un circuito del Triángulo para estabilizar el acceso.");
            }
            else
            {
                descText.text = L("keycard.ready",
                    "Proyecto descubierto. La keycard puede construirse.");
            }
        }

        if (costText != null)
        {
            if (!discovered)
                costText.text = study == null ? string.Empty :
                    LF("study.energy_cost", "Inicio: {0:0.##} Energía", study.energyCost) +
                    " · " + LF("study.duration", "Duración: {0}",
                        FormatDuration(study.durationSeconds));
            else if (unlocked)
                costText.text = L("keycard.cost_complete", "Coste: completado");
            else
                costText.text = LF("keycard.cost",
                    "Coste: {0:0} LE + {1:0} Trazas",
                    state.experimentalChamberKeycardLeCost,
                    state.experimentalChamberKeycardTraceCost);
        }

        if (buttonText != null)
        {
            if (!discovered)
            {
                UpgradeStudyBlockReason reason = study == null
                    ? UpgradeStudyBlockReason.UnknownStudy
                    : UpgradeStudySystem.GetStartBlockReason(state, study.id);
                buttonText.text = conclusion
                    ? L("study.action.reveal", "Revelar")
                    : active
                        ? L("study.status.active", "Investigando")
                        : reason == UpgradeStudyBlockReason.InsufficientEnergy
                            ? L("study.lock.energy", "Falta Energía")
                            : L("study.action.start", "Investigar");
            }
            else if (unlocked)
                buttonText.text = L("keycard.open", "Abierto");
            else
                buttonText.text = L("ui.buy", "Comprar");
        }

        if (buyButton != null)
        {
            if (!discovered)
            {
                bool canStart = study != null &&
                    UpgradeStudySystem.GetStartBlockReason(state, study.id) ==
                        UpgradeStudyBlockReason.None;
                buyButton.interactable = conclusion || (!active && canStart);
            }
            else
            {
                buyButton.interactable = !unlocked && canBuy;
            }
        }
    }

    private static string FormatDuration(double seconds)
    {
        int total = Mathf.Max(0, Mathf.CeilToInt((float)seconds));
        return total >= 60
            ? $"{total / 60}:{total % 60:00}"
            : $"0:{total:00}";
    }

    private static string L(string key, string fallback)
    {
        LocalizationManager lm = LocalizationManager.I;
        if (lm == null) return fallback;
        string value = lm.T(key);
        return string.IsNullOrEmpty(value) || value == key ? fallback : value;
    }

    private static string LF(string key, string fallback, params object[] args)
    {
        string format = L(key, fallback);
        try { return string.Format(format, args); }
        catch { return format; }
    }

    private void AutoBindIfNeeded()
    {
        if (nameText == null)
        {
            Transform target = transform.Find("Name_Text");
            if (target != null) nameText = target.GetComponent<TextMeshProUGUI>();
        }

        if (descText == null)
        {
            Transform target = transform.Find("Desc_Text");
            if (target != null) descText = target.GetComponent<TextMeshProUGUI>();
        }

        if (costText == null)
        {
            Transform target = transform.Find("Cost_Text");
            if (target != null) costText = target.GetComponent<TextMeshProUGUI>();
        }

        if (buyButton == null)
        {
            Transform target = transform.Find("Buy_Button");
            if (target != null) buyButton = target.GetComponent<Button>();
        }

        if (buttonText == null && buyButton != null)
        {
            Transform target = buyButton.transform.Find("Buy_Text");
            if (target != null) buttonText = target.GetComponent<TextMeshProUGUI>();
        }
    }
}

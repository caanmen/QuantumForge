using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DevMultiplierButton : MonoBehaviour
{
    [Header("Refs")]
    public TickSystem tickSystem;
    public Button button;

    [Header("Dev (compatibilidad serializada)")]
    public float devMult = 5f;

    private static readonly Color NormalColor = new Color(0.65f, 0.65f, 0.65f);
    private static readonly Color AcceleratedColor = new Color(1f, 0.65f, 0.15f);
    private TMP_Text tmpLabel;
    private Text legacyLabel;

    void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        tmpLabel = GetComponentInChildren<TMP_Text>(true);
        legacyLabel = GetComponentInChildren<Text>(true);

        if (button != null)
        {
            button.onClick.RemoveListener(CycleSpeed);
            button.onClick.AddListener(CycleSpeed);
        }

        ApplyAvailabilityAndRefresh();
    }

    void OnEnable()
    {
        QaRuntimeService.SpeedChanged -= OnSpeedChanged;
        QaRuntimeService.SpeedChanged += OnSpeedChanged;
        ApplyAvailabilityAndRefresh();
    }

    void OnDisable()
    {
        QaRuntimeService.SpeedChanged -= OnSpeedChanged;
    }

    void OnDestroy()
    {
        QaRuntimeService.SpeedChanged -= OnSpeedChanged;
        if (button != null)
            button.onClick.RemoveListener(CycleSpeed);
    }

    private void CycleSpeed()
    {
        QaRuntimeService.CycleSpeed();
    }

    private void OnSpeedChanged(float multiplier)
    {
        RefreshVisuals(multiplier);
    }

    private void ApplyAvailabilityAndRefresh()
    {
        if (!QaRuntimeService.IsAvailable)
        {
            gameObject.SetActive(false);
            return;
        }

        RefreshVisuals(QaRuntimeService.SimulationMultiplier);
    }

    private void RefreshVisuals(float multiplier)
    {
        string label = "QA x" + multiplier.ToString("0");
        if (tmpLabel != null)
            tmpLabel.SetText(label);
        if (legacyLabel != null)
            legacyLabel.text = label;

        if (button != null && button.image != null)
        {
            button.image.color = multiplier > 1f
                ? AcceleratedColor
                : NormalColor;
        }
    }
}

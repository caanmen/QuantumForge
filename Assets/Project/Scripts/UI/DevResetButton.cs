using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DevResetButton : MonoBehaviour
{
    private const float ConfirmationWindowSeconds = 3f;
    private const string ConfirmationLabel = "TOCA OTRA VEZ";

    public Button button;

    private TMP_Text tmpLabel;
    private Text legacyLabel;
    private string originalTmpLabel;
    private string originalLegacyLabel;
    private bool awaitingConfirmation;
    private float confirmationDeadline;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        tmpLabel = GetComponentInChildren<TMP_Text>(true);
        legacyLabel = GetComponentInChildren<Text>(true);
        originalTmpLabel = tmpLabel != null ? tmpLabel.text : null;
        originalLegacyLabel = legacyLabel != null ? legacyLabel.text : null;

        if (!QaRuntimeService.IsAvailable)
        {
            gameObject.SetActive(false);
            return;
        }

        if (button != null)
        {
            button.onClick.RemoveListener(DoReset);
            button.onClick.AddListener(DoReset);
        }
    }

    private void OnEnable()
    {
        if (!QaRuntimeService.IsAvailable)
        {
            gameObject.SetActive(false);
            return;
        }

        CancelConfirmation();
    }

    private void Update()
    {
        if (awaitingConfirmation &&
            Time.unscaledTime >= confirmationDeadline)
        {
            CancelConfirmation();
        }
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(DoReset);
    }

    public void DoReset()
    {
        if (!QaRuntimeService.IsAvailable)
            return;

        if (!awaitingConfirmation ||
            Time.unscaledTime >= confirmationDeadline)
        {
            awaitingConfirmation = true;
            confirmationDeadline =
                Time.unscaledTime + ConfirmationWindowSeconds;
            SetLabel(ConfirmationLabel);
            return;
        }

        awaitingConfirmation = false;
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        var save = FindFirstObjectByType<SaveService>();
        if (save != null)
        {
            try { save.ResetSave(); } catch { }
        }

        var scene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scene);
    }

    private void CancelConfirmation()
    {
        awaitingConfirmation = false;
        confirmationDeadline = 0f;

        if (tmpLabel != null && originalTmpLabel != null)
            tmpLabel.SetText(originalTmpLabel);
        if (legacyLabel != null && originalLegacyLabel != null)
            legacyLabel.text = originalLegacyLabel;
    }

    private void SetLabel(string value)
    {
        if (tmpLabel != null)
            tmpLabel.SetText(value);
        if (legacyLabel != null)
            legacyLabel.text = value;
    }
}

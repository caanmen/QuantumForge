using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class VerticalSettingsPanelUI : MonoBehaviour
{
    public Button spanishButton;
    public Button englishButton;
    public TMP_Text currentLanguageText;

    [Header("Machine Cube 3D Quality")]
    public Button lowQualityButton;
    public Button balancedQualityButton;
    public Button highQualityButton;
    public TMP_Text currentQualityText;

    private int lastRevision = -1;

    private void Awake()
    {
        WireListeners();
        RefreshLanguageState();
    }

    private void OnEnable()
    {
        MachineCube3DQuality.Changed -= RefreshQualityState;
        MachineCube3DQuality.Changed += RefreshQualityState;
        WireListeners();
        RefreshLanguageState();
    }

    private void OnDisable()
    {
        MachineCube3DQuality.Changed -= RefreshQualityState;
    }

    private void Update()
    {
        int revision = LocalizationManager.I != null
            ? LocalizationManager.I.Revision
            : -1;
        if (revision != lastRevision)
            RefreshLanguageState();
    }

    private void OnDestroy()
    {
        MachineCube3DQuality.Changed -= RefreshQualityState;
        if (spanishButton != null)
            spanishButton.onClick.RemoveListener(SetSpanish);
        if (englishButton != null)
            englishButton.onClick.RemoveListener(SetEnglish);
        if (lowQualityButton != null)
            lowQualityButton.onClick.RemoveListener(SetLow);
        if (balancedQualityButton != null)
            balancedQualityButton.onClick.RemoveListener(SetBalanced);
        if (highQualityButton != null)
            highQualityButton.onClick.RemoveListener(SetHigh);
    }

    public void SetSpanish()
    {
        if (LocalizationManager.I != null)
            LocalizationManager.I.SetLanguage(LocalizationManager.Language.ES);
        RefreshLanguageState();
    }

    public void SetEnglish()
    {
        if (LocalizationManager.I != null)
            LocalizationManager.I.SetLanguage(LocalizationManager.Language.EN);
        RefreshLanguageState();
    }

    public void SetLow()
    {
        MachineCube3DQuality.Set(MachineCube3DQualityLevel.Low);
        RefreshQualityState();
    }

    public void SetBalanced()
    {
        MachineCube3DQuality.Set(MachineCube3DQualityLevel.Balanced);
        RefreshQualityState();
    }

    public void SetHigh()
    {
        MachineCube3DQuality.Set(MachineCube3DQualityLevel.High);
        RefreshQualityState();
    }

    public void RefreshLanguageState()
    {
        lastRevision = LocalizationManager.I != null
            ? LocalizationManager.I.Revision
            : -1;
        bool english = LocalizationManager.I != null &&
            LocalizationManager.I.CurrentLanguage == LocalizationManager.Language.EN;
        if (currentLanguageText != null)
        {
            string key = english
                ? "settings.language.current.en"
                : "settings.language.current.es";
            currentLanguageText.SetText(LocalizationManager.I != null
                ? LocalizationManager.I.T(key)
                : english ? "Language: English" : "Idioma: Espanol");
        }

        RefreshQualityState();
    }

    public void RefreshQualityState()
    {
        MachineCube3DQualityLevel quality = MachineCube3DQuality.Current;

        if (lowQualityButton != null)
            lowQualityButton.interactable = quality != MachineCube3DQualityLevel.Low;
        if (balancedQualityButton != null)
            balancedQualityButton.interactable = quality != MachineCube3DQualityLevel.Balanced;
        if (highQualityButton != null)
            highQualityButton.interactable = quality != MachineCube3DQualityLevel.High;

        if (currentQualityText == null)
            return;

        bool english = LocalizationManager.I != null &&
            LocalizationManager.I.CurrentLanguage == LocalizationManager.Language.EN;
        string qualityName = quality switch
        {
            MachineCube3DQualityLevel.Low => english ? "Low" : "Baja",
            MachineCube3DQualityLevel.Balanced => english ? "Balanced" : "Equilibrada",
            _ => english ? "High" : "Alta"
        };
        currentQualityText.SetText(english
            ? $"Cube quality: {qualityName}"
            : $"Calidad del cubo: {qualityName}");
    }

    private void WireListeners()
    {
        if (spanishButton != null)
        {
            spanishButton.onClick.RemoveListener(SetSpanish);
            spanishButton.onClick.AddListener(SetSpanish);
        }
        if (englishButton != null)
        {
            englishButton.onClick.RemoveListener(SetEnglish);
            englishButton.onClick.AddListener(SetEnglish);
        }
        if (lowQualityButton != null)
        {
            lowQualityButton.onClick.RemoveListener(SetLow);
            lowQualityButton.onClick.AddListener(SetLow);
        }
        if (balancedQualityButton != null)
        {
            balancedQualityButton.onClick.RemoveListener(SetBalanced);
            balancedQualityButton.onClick.AddListener(SetBalanced);
        }
        if (highQualityButton != null)
        {
            highQualityButton.onClick.RemoveListener(SetHigh);
            highQualityButton.onClick.AddListener(SetHigh);
        }
    }
}

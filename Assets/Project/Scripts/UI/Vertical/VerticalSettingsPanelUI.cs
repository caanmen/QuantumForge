using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class VerticalSettingsPanelUI : MonoBehaviour
{
    public Button spanishButton;
    public Button englishButton;
    public TMP_Text currentLanguageText;

    private int lastRevision = -1;

    private void Awake()
    {
        WireListeners();
        RefreshLanguageState();
    }

    private void OnEnable()
    {
        WireListeners();
        RefreshLanguageState();
    }

    private void OnDisable()
    {
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
        if (spanishButton != null)
            spanishButton.onClick.RemoveListener(SetSpanish);
        if (englishButton != null)
            englishButton.onClick.RemoveListener(SetEnglish);
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
    }
}

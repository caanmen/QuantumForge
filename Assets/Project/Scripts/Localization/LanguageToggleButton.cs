using UnityEngine;

public class LanguageToggleButton : MonoBehaviour
{
    public void ToggleLanguage()
    {
        if (LocalizationManager.I == null) return;

        var current = LocalizationManager.I.CurrentLanguage;
        var next = (current == LocalizationManager.Language.EN)
            ? LocalizationManager.Language.ES
            : LocalizationManager.Language.EN;

        LocalizationManager.I.SetLanguage(next);
    }

    public void SetSpanish()
    {
        if (LocalizationManager.I == null) return;
        LocalizationManager.I.SetLanguage(LocalizationManager.Language.ES);
    }

    public void SetEnglish()
    {
        if (LocalizationManager.I == null) return;
        LocalizationManager.I.SetLanguage(LocalizationManager.Language.EN);
    }
}

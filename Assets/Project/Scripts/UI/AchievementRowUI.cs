using TMPro;
using UnityEngine;

public class AchievementRowUI : MonoBehaviour
{
    [Header("Refs")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI statusText;

    private string L(string key, string fallback)
    {
        var lm = LocalizationManager.I;
        if (lm == null) return fallback;

        var s = lm.T(key);

        // IMPORTANTE: tu T() devuelve la key si no existe.
        // Si s == key, consideramos que NO hay traducción y usamos fallback.
        if (string.IsNullOrEmpty(s) || s == key) return fallback;

        return s;
    }


    public void SetData(AchievementDef def, bool unlocked)
    {
        // Nombre/Descripción: intenta por key y si no existe usa el JSON actual (fallback)
        // Keys sugeridas: achv.<id>.name / achv.<id>.desc
        if (nameText != null)
            nameText.text = L($"achv.{def.id}.name", def.name);

        if (descText != null)
            descText.text = L($"achv.{def.id}.desc", def.description);

        // Estado: siempre localizado
        if (statusText != null)
        {
            statusText.text = unlocked
                ? L("achv.status.unlocked", "Desbloqueado")
                : L("achv.status.locked", "Bloqueado");

            statusText.color = unlocked
                ? new Color(0.3f, 1f, 0.3f)
                : new Color(1f, 0.4f, 0.4f);
        }
    }
}

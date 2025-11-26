using TMPro;
using UnityEngine;

public class AchievementRowUI : MonoBehaviour
{
    [Header("Refs")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI statusText;

    /// <summary>
    /// Configura los textos y colores de la fila según el logro.
    /// </summary>
    public void SetData(AchievementDef def, bool unlocked)
    {
        if (nameText != null)
            nameText.text = def.name;

        if (descText != null)
            descText.text = def.description;

        if (statusText != null)
        {
            statusText.text = unlocked ? "Desbloqueado" : "Bloqueado";
            statusText.color = unlocked
                ? new Color(0.3f, 1f, 0.3f)  // verde suave
                : new Color(1f, 0.4f, 0.4f); // rojo suave
        }
    }
}

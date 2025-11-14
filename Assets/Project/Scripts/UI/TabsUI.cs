using UnityEngine;

/// <summary>
/// Control sencillo de pestañas:
/// - Pestaña de Generación (edificios LE)
/// - Pestaña de Laboratorio (cosas futuras: BEC, ZPE, upgrades, etc.)
/// </summary>
public class TabsUI : MonoBehaviour
{
    [Header("Paneles de pestañas")]
    public GameObject generationPanel; // panel donde están tus edificios actuales
    public GameObject labPanel;        // panel para laboratorio / futuros sistemas

    private void Awake()
    {
        // Al iniciar, mostramos la pestaña de generación por defecto
        ShowGenerationTab();
    }

    /// <summary>
    /// Muestra la pestaña de generación y oculta la de laboratorio.
    /// La puedes conectar a un botón "Generación".
    /// </summary>
    public void ShowGenerationTab()
    {
        if (generationPanel != null)
            generationPanel.SetActive(true);

        if (labPanel != null)
            labPanel.SetActive(false);
    }

    /// <summary>
    /// Muestra la pestaña de laboratorio y oculta la de generación.
    /// La puedes conectar a un botón "Laboratorio".
    /// </summary>
    public void ShowLabTab()
    {
        if (generationPanel != null)
            generationPanel.SetActive(false);

        if (labPanel != null)
            labPanel.SetActive(true);
    }
}

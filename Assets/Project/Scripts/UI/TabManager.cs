using UnityEngine;

public class TabManager : MonoBehaviour
{
    [Header("Paneles de contenido")]
    public GameObject panelGeneracion;   // Panel_Generacion
    public GameObject panelLab;          // Panel_Lab
    public GameObject panelLogros;       // Panel_Logros
    public GameObject panelPrestigio;    // PrestigePanel

    private void Start()
    {
        ShowGeneracion();   // pestaña por defecto
    }

    public void ShowGeneracion()
    {
        ActivatePanel(panelGeneracion);
    }

    public void ShowLab()
    {
        ActivatePanel(panelLab);
    }

    public void ShowLogros()
    {
        ActivatePanel(panelLogros);
    }

    public void ShowPrestigio()
    {
        ActivatePanel(panelPrestigio);
    }

    private void ActivatePanel(GameObject panel)
    {
        // Ojo: NO tocamos Panel_HUD aquí.
        if (panelGeneracion != null) panelGeneracion.SetActive(false);
        if (panelLab        != null) panelLab.SetActive(false);
        if (panelLogros     != null) panelLogros.SetActive(false);
        if (panelPrestigio  != null) panelPrestigio.SetActive(false);

        if (panel != null)
            panel.SetActive(true);
    }
}

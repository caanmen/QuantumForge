using UnityEngine;
using UnityEngine.UI;

public class TabsUI : MonoBehaviour
{
    [Header("Botones de pestañas")]
    public Button btnGeneracion;
    public Button btnLab;
    public Button btnLogros;
    public Button btnPrestigio;   // 🔹 nuevo
    public Button btnLambda;      // 🔹 nuevo

    [Header("Paneles")]
    public GameObject panelGeneracion;
    public GameObject panelLab;
    public GameObject panelLogros;
    public GameObject prestigePanel;      // 🔹 nuevo
    public GameObject metaPrestigePanel;  // 🔹 nuevo (Λ)

    private void Awake()
    {
        if (btnGeneracion != null)
            btnGeneracion.onClick.AddListener(ShowGeneracion);

        if (btnLab != null)
            btnLab.onClick.AddListener(ShowLab);

        if (btnLogros != null)
            btnLogros.onClick.AddListener(ShowLogros);

        if (btnPrestigio != null)
            btnPrestigio.onClick.AddListener(ShowPrestigio);

        if (btnLambda != null)
            btnLambda.onClick.AddListener(ShowMetaPrestigio);
    }

    private void Start()
    {
        ShowGeneracion();   // pestaña por defecto
    }

    private void HideAll()
    {
        if (panelGeneracion != null)    panelGeneracion.SetActive(false);
        if (panelLab != null)          panelLab.SetActive(false);
        if (panelLogros != null)       panelLogros.SetActive(false);
        if (prestigePanel != null)     prestigePanel.SetActive(false);
        if (metaPrestigePanel != null) metaPrestigePanel.SetActive(false);
    }

    private void ShowGeneracion()
    {
        HideAll();
        if (panelGeneracion != null) panelGeneracion.SetActive(true);
    }

    private void ShowLab()
    {
        HideAll();
        if (panelLab != null) panelLab.SetActive(true);
    }

    private void ShowLogros()
    {
        HideAll();
        if (panelLogros != null) panelLogros.SetActive(true);
    }

    private void ShowPrestigio()
    {
        HideAll();
        if (prestigePanel != null) prestigePanel.SetActive(true);
    }

    private void ShowMetaPrestigio()
    {
        HideAll();
        if (metaPrestigePanel != null) metaPrestigePanel.SetActive(true);
    }
}

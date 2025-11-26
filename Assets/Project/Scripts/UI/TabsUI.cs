using UnityEngine;
using UnityEngine.UI;

public class TabsUI : MonoBehaviour
{
    [Header("Botones de pestañas")]
    public Button btnGeneracion;
    public Button btnLab;
    public Button btnLogros;

    [Header("Paneles")]
    public GameObject panelGeneracion;
    public GameObject panelLab;
    public GameObject panelLogros;

    private void Awake()
    {
        if (btnGeneracion != null)
            btnGeneracion.onClick.AddListener(ShowGeneracion);

        if (btnLab != null)
            btnLab.onClick.AddListener(ShowLab);

        if (btnLogros != null)
            btnLogros.onClick.AddListener(ShowLogros);
    }

    private void Start()
    {
        ShowGeneracion();   // pestaña por defecto
    }

    private void ShowGeneracion()
    {
        Debug.Log("[Tabs] Mostrar GENERACIÓN");
        SetPanels(true, false, false);
    }

    private void ShowLab()
    {
        Debug.Log("[Tabs] Mostrar LABORATORIO");
        SetPanels(false, true, false);
    }

    private void ShowLogros()
    {
        Debug.Log("[Tabs] Mostrar LOGROS");
        SetPanels(false, false, true);
    }

    private void SetPanels(bool gen, bool lab, bool logros)
    {
        if (panelGeneracion != null) panelGeneracion.SetActive(gen);
        if (panelLab != null)        panelLab.SetActive(lab);
        if (panelLogros != null)     panelLogros.SetActive(logros);
    }
}

using UnityEngine;
using UnityEngine.UI;

public sealed class Dimension1TreeNavigationUI : MonoBehaviour
{
    [SerializeField] private Dimension1PanelUI panel;
    [SerializeField] private Dimension1CommandCenterUI commandCenter;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject[] hideWhileOpen;

    private bool[] hiddenPreviousStates;
    private VerticalNavigationUI verticalNavigation;
    private bool exclusiveNavigationPending;

    public void OpenCommandCenter()
    {
        CloseTree();
        if (commandCenter != null) commandCenter.ShowCommandCenterScreen();
    }

    public void OpenGalaxy()
    {
        if (panel == null) return;
        CloseTree();
        panel.OnClickOpenGalaxyPanel();
    }

    public void OpenExplore()
    {
        CloseTree();
        if (commandCenter != null) commandCenter.ShowExploreScreen();
    }

    public void OpenHangar()
    {
        if (panel == null) return;
        CloseTree();
        panel.OnClickOpenHangarPanel();
    }

    public void OpenRelics()
    {
        if (panel == null) return;
        CloseTree();
        panel.OnClickOpenRelicChamberPanel();
    }

    private void OnEnable()
    {
        // El Árbol puede abrirse dentro del callback de un Button. Desactivar otros
        // Selectable en este mismo OnEnable corrompe el registro global de UGUI y deja
        // la interfaz inerte aunque el PlayerLoop continúe. Se difiere un fotograma,
        // igual que en Galaxia y Explorar.
        exclusiveNavigationPending = true;
        if (verticalNavigation == null)
            verticalNavigation = FindFirstObjectByType<VerticalNavigationUI>(FindObjectsInactive.Include);
        if (verticalNavigation != null) verticalNavigation.SetNavigationSuppressed(true, this);
        SetInteraction(true);
    }

    private void OnDisable()
    {
        exclusiveNavigationPending = false;
        ApplyExclusiveNavigation(false);
        if (verticalNavigation != null) verticalNavigation.SetNavigationSuppressed(false, this);
    }

    private void Update()
    {
        if (exclusiveNavigationPending)
        {
            exclusiveNavigationPending = false;
            ApplyExclusiveNavigation(true);
        }

        if (hideWhileOpen == null) return;
        foreach (GameObject target in hideWhileOpen)
            if (target != null && target.activeSelf) target.SetActive(false);
    }

    private void CloseTree()
    {
        if (panel != null) panel.OnClickCloseDimension1TreePanel();
        else gameObject.SetActive(false);
    }

    private void SetInteraction(bool enabled)
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = enabled;
        canvasGroup.blocksRaycasts = enabled;
    }

    private void ApplyExclusiveNavigation(bool hide)
    {
        if (hideWhileOpen == null) return;
        if (hide)
        {
            hiddenPreviousStates = new bool[hideWhileOpen.Length];
            for (int i = 0; i < hideWhileOpen.Length; i++)
            {
                GameObject target = hideWhileOpen[i];
                if (target == null) continue;
                hiddenPreviousStates[i] = target.activeSelf;
                target.SetActive(false);
            }
            return;
        }

        if (hiddenPreviousStates == null) return;
        for (int i = 0; i < hideWhileOpen.Length && i < hiddenPreviousStates.Length; i++)
            if (hideWhileOpen[i] != null) hideWhileOpen[i].SetActive(hiddenPreviousStates[i]);
        hiddenPreviousStates = null;
    }
}

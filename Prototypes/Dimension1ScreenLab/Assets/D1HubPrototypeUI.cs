using UnityEngine;

public sealed class D1HubPrototypeUI : MonoBehaviour
{
    [SerializeField] private GameObject metalsDrawer;
    [SerializeField] private RectTransform crystal;
    [SerializeField] private CanvasGroup crystalGlow;

    private float time;

    public void Configure(GameObject drawer, RectTransform crystalRoot, CanvasGroup glow)
    {
        metalsDrawer = drawer;
        crystal = crystalRoot;
        crystalGlow = glow;
    }

    public void ToggleMetalsDrawer()
    {
        if (metalsDrawer != null)
            metalsDrawer.SetActive(!metalsDrawer.activeSelf);
    }

    public void SelectModule(string moduleName)
    {
        Debug.Log("[D1 Screen Lab] Modulo seleccionado: " + moduleName);
    }

    private void Update()
    {
        time += Time.unscaledDeltaTime;
        if (crystal != null)
        {
            float scale = 1f + Mathf.Sin(time * 1.35f) * 0.018f;
            crystal.localScale = new Vector3(scale, scale, 1f);
        }

        if (crystalGlow != null)
            crystalGlow.alpha = 0.32f + (Mathf.Sin(time * 1.35f) + 1f) * 0.08f;
    }
}

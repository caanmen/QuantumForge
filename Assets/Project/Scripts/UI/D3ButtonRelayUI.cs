using UnityEngine;
using UnityEngine.UI;

public sealed class D3ButtonRelayUI : MonoBehaviour
{
    public Button sourceButton;
    public Button targetButton;

    private void OnEnable()
    {
        if (sourceButton != null)
            sourceButton.onClick.AddListener(Relay);
    }

    private void OnDisable()
    {
        if (sourceButton != null)
            sourceButton.onClick.RemoveListener(Relay);
    }

    private void Relay()
    {
        if (targetButton != null && targetButton.interactable)
            targetButton.onClick.Invoke();
    }
}

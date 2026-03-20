using UnityEngine;
using UnityEngine.UI;

public class DevMultiplierButton : MonoBehaviour
{
    [Header("Refs")]
    public TickSystem tickSystem;   // arrastra tu TickSystem aquí
    public Button button;           // arrastra el Button aquí

    [Header("Dev")]
    public float devMult = 5f;
    bool _on;

    void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        if (button != null) button.onClick.AddListener(Toggle);
    }

    void Toggle()
    {
        _on = !_on;
        if (tickSystem != null)
            tickSystem.devMultiplier = _on ? devMult : 1f;
    }
}
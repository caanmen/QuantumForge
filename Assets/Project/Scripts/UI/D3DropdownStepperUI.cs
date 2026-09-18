using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class D3DropdownStepperUI : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public Button minusButton;
    public Button plusButton;
    public TMP_Text valueText;

    private void OnEnable()
    {
        if (minusButton != null)
            minusButton.onClick.AddListener(Decrease);
        if (plusButton != null)
            plusButton.onClick.AddListener(Increase);
        if (dropdown != null)
            dropdown.onValueChanged.AddListener(OnDropdownChanged);
        Refresh();
    }

    private void OnDisable()
    {
        if (minusButton != null)
            minusButton.onClick.RemoveListener(Decrease);
        if (plusButton != null)
            plusButton.onClick.RemoveListener(Increase);
        if (dropdown != null)
            dropdown.onValueChanged.RemoveListener(OnDropdownChanged);
    }

    private void Decrease()
    {
        Step(-1);
    }

    private void Increase()
    {
        Step(1);
    }

    private void Step(int delta)
    {
        if (dropdown == null || dropdown.options == null || dropdown.options.Count == 0)
            return;
        dropdown.value = Mathf.Clamp(
            dropdown.value + delta, 0, dropdown.options.Count - 1);
        dropdown.RefreshShownValue();
        Refresh();
    }

    private void OnDropdownChanged(int _)
    {
        Refresh();
    }

    public void Refresh()
    {
        int count = dropdown == null || dropdown.options == null
            ? 0
            : dropdown.options.Count;
        int value = dropdown == null ? 0 : dropdown.value;
        if (minusButton != null)
            minusButton.interactable = count > 0 && value > 0;
        if (plusButton != null)
            plusButton.interactable = count > 0 && value < count - 1;
        if (valueText == null)
            return;

        string display = count > 0
            ? dropdown.options[Mathf.Clamp(value, 0, count - 1)].text
            : "—";
        display = display.Replace("Cantidad", "").Trim();
        valueText.text = string.IsNullOrEmpty(display) ? "—" : display;
    }
}

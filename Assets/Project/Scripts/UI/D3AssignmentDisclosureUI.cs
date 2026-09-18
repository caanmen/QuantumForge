using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class D3AssignmentDisclosureUI : MonoBehaviour
{
    public Button toggleButton;
    public TMP_Text label;
    public TMP_Text statusText;
    public TMP_Dropdown mkDropdown;
    public TMP_Dropdown traitDropdown;
    public TMP_Dropdown channelDropdown;
    public Button addButton;
    public Button removeButton;

    private bool expanded;

    private void OnEnable()
    {
        if (toggleButton != null)
            toggleButton.onClick.AddListener(Toggle);
        Subscribe(mkDropdown, true);
        Subscribe(traitDropdown, true);
        Subscribe(channelDropdown, true);
        expanded = false;
        RefreshControls();
    }

    private void OnDisable()
    {
        if (toggleButton != null)
            toggleButton.onClick.RemoveListener(Toggle);
        Subscribe(mkDropdown, false);
        Subscribe(traitDropdown, false);
        Subscribe(channelDropdown, false);
    }

    private void Toggle()
    {
        expanded = !expanded;
        RefreshControls();
    }

    private void Subscribe(TMP_Dropdown dropdown, bool add)
    {
        if (dropdown == null)
            return;
        if (add)
            dropdown.onValueChanged.AddListener(OnSelectionChanged);
        else
            dropdown.onValueChanged.RemoveListener(OnSelectionChanged);
    }

    private void OnSelectionChanged(int _)
    {
        RefreshLabel();
    }

    public void RefreshControls()
    {
        SetVisible(mkDropdown, expanded);
        SetVisible(traitDropdown, expanded);
        SetVisible(channelDropdown, expanded);
        if (addButton != null)
            addButton.gameObject.SetActive(!expanded);
        if (removeButton != null)
            removeButton.gameObject.SetActive(!expanded);
        if (statusText != null)
            statusText.gameObject.SetActive(false);
        RefreshLabel();
    }

    private void RefreshLabel()
    {
        if (label == null)
            return;
        string mk = Current(mkDropdown, "MK1");
        string trait = Current(traitDropdown, "NORMAL");
        label.text = mk.ToUpperInvariant() + " " + trait.ToUpperInvariant();
    }

    private static string Current(TMP_Dropdown dropdown, string fallback)
    {
        if (dropdown == null || dropdown.options == null ||
            dropdown.options.Count == 0)
            return fallback;
        int index = Mathf.Clamp(dropdown.value, 0, dropdown.options.Count - 1);
        string value = dropdown.options[index].text;
        return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
    }

    private static void SetVisible(TMP_Dropdown dropdown, bool visible)
    {
        if (dropdown != null)
            dropdown.gameObject.SetActive(visible);
    }
}

using TMPro;
using UnityEngine;


[ExecuteAlways]
public sealed class D3CenteredIconLabelUI : MonoBehaviour
{
    public RectTransform icon;
    public TMP_Text label;
    public float gap = 10f;
    public float horizontalOffset;
    public float verticalOffset;

    private string _lastText;
    private Vector2 _lastSize;

    public void Configure(
        RectTransform iconRect, TMP_Text text, float spacing,
        float contentVerticalOffset = 0f,
        float contentHorizontalOffset = 0f)
    {
        icon = iconRect;
        label = text;
        gap = spacing;
        verticalOffset = contentVerticalOffset;
        horizontalOffset = contentHorizontalOffset;
        Layout();
    }

    private void OnEnable()
    {
        Layout();
    }

    private void LateUpdate()
    {
        RectTransform container = transform as RectTransform;
        if (container == null || label == null || icon == null)
            return;
        if (_lastText != label.text || _lastSize != container.rect.size)
            Layout();
    }

    public void Layout()
    {
        RectTransform container = transform as RectTransform;
        if (container == null || label == null || icon == null)
            return;

        float width = container.rect.width;
        float height = container.rect.height;
        float iconWidth = icon.rect.width;
        float iconHeight = icon.rect.height;
        float availableText = Mathf.Max(1f, width - iconWidth - gap - 8f);
        float textWidth = Mathf.Min(
            label.GetPreferredValues(label.text).x, availableText);
        float groupWidth = iconWidth + gap + textWidth;
        float start = Mathf.Max(
            4f, (width - groupWidth) * 0.5f + horizontalOffset);

        SetRect(icon, start, (height - iconHeight) * 0.5f + verticalOffset,
            iconWidth, iconHeight);
        SetRect(label.rectTransform, start + iconWidth + gap, verticalOffset,
            textWidth, height);
        label.alignment = TextAlignmentOptions.Center;
        label.textWrappingMode = TextWrappingModes.NoWrap;

        _lastText = label.text;
        _lastSize = container.rect.size;
    }

    private static void SetRect(
        RectTransform rect, float x, float y, float width, float height)
    {
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(x, -y);
        rect.sizeDelta = new Vector2(width, height);
        rect.localScale = Vector3.one;
    }
}

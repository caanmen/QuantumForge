using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public sealed class D3FacilitiesStaticSkinUI : MonoBehaviour
{
    public RawImage referencePlate;
    public Texture consoleReference;
    public Texture nucleusReference;
    public Texture improvementReference;
    public Texture portReference;
    public TMP_Dropdown facilityDropdown;
    public RectTransform backRect;
    public RectTransform facilityRect;
    public RectTransform channelRect;
    public RectTransform mkRect;
    public RectTransform traitRect;
    public RectTransform addRect;
    public RectTransform removeRect;
    public RectTransform upgradeRect;
    public RectTransform automationRect;
    public RectTransform autonomyRect;
    public RectTransform consoleRect;

    private const float ScaleY = 1890f / 1920f;
    private int _lastVariant = int.MinValue;
    private bool _improvementVariant;

    private void OnEnable()
    {
        ApplyCurrentVariant(true);
    }

    private void LateUpdate()
    {
        ApplyCurrentVariant(false);
    }

    public void ApplyCurrentVariant(bool force)
    {
        string label = facilityDropdown == null || facilityDropdown.options.Count == 0
            ? ""
            : facilityDropdown.options[Mathf.Clamp(
                facilityDropdown.value, 0, facilityDropdown.options.Count - 1)].text;
        string upper = (label ?? "").ToUpperInvariant();
        int variant = upper.Contains("NÚCLEO") || upper.Contains("NUCLEO")
            ? 1
            : upper.Contains("PUERTO") ? 3
            : upper.Contains("CONSOLA") ? (_improvementVariant ? 2 : 0) : -1;
        if (!force && variant == _lastVariant)
            return;
        _lastVariant = variant;

        if (referencePlate == null)
            return;
        referencePlate.enabled = variant >= 0;
        if (variant < 0)
            return;
        referencePlate.texture = variant == 1
            ? nucleusReference
            : variant == 2 ? improvementReference
            : variant == 3 ? portReference : consoleReference;
        if (variant == 1)
            ApplyNucleusLayout();
        else if (variant == 2)
            ApplyImprovementLayout();
        else if (variant == 3)
            ApplyPortLayout();
        else
            ApplyConsoleLayout();
    }

    public void SetImprovementVariant(bool enabled)
    {
        _improvementVariant = enabled;
        ApplyCurrentVariant(true);
    }

    private void ApplyNucleusLayout()
    {
        Set(facilityRect, 59, 669, 347, 82);
        Set(mkRect, 414, 669, 165, 82);
        Set(traitRect, 586, 669, 197, 82);
        Set(channelRect, 792, 669, 231, 82);
        Set(addRect, 78, 1405, 226, 103);
        Set(removeRect, 315, 1405, 226, 103);
        Set(upgradeRect, 545, 1405, 466, 103);
        Set(automationRect, 78, 1518, 464, 81);
        Set(autonomyRect, 545, 1518, 466, 81);
        Set(backRect, 280, 1634, 520, 116);
    }

    private void ApplyConsoleLayout()
    {
        Set(facilityRect, 218, 994, 393, 67);
        Set(channelRect, 727, 994, 304, 67);
        Set(mkRect, 218, 1090, 335, 67);
        Set(traitRect, 607, 1090, 385, 67);
        Set(addRect, 62, 1391, 468, 104);
        Set(removeRect, 550, 1391, 469, 104);
        Set(upgradeRect, 62, 1502, 468, 94);
        Set(consoleRect, 550, 1502, 469, 94);
        Set(backRect, 154, 1614, 774, 103);
    }

    private void ApplyImprovementLayout()
    {
        Set(facilityRect, 47, 1525, 463, 61);
        Set(channelRect, 48, 1586, 185, 57);
        Set(mkRect, 239, 1586, 124, 57);
        Set(traitRect, 366, 1586, 145, 57);
        Set(addRect, 42, 1644, 293, 64);
        Set(removeRect, 339, 1644, 232, 64);
        Set(upgradeRect, 574, 1644, 462, 64);
        Set(consoleRect, 42, 1712, 530, 67);
        Set(backRect, 574, 1712, 462, 67);
    }

    private void ApplyPortLayout()
    {
        Set(facilityRect, 56, 1002, 365, 79);
        Set(channelRect, 430, 1002, 249, 79);
        Set(mkRect, 686, 1002, 158, 79);
        Set(traitRect, 850, 1002, 184, 79);
        Set(addRect, 58, 1268, 224, 104);
        Set(removeRect, 292, 1268, 232, 104);
        Set(upgradeRect, 532, 1268, 250, 104);
        Set(automationRect, 791, 1268, 241, 104);
        Set(backRect, 302, 1664, 481, 87);
    }

    private static void Set(RectTransform rect,
        float x, float y, float width, float height)
    {
        if (rect == null)
            return;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(x, -y * ScaleY);
        rect.sizeDelta = new Vector2(width, height * ScaleY);
        rect.localScale = Vector3.one;
    }
}

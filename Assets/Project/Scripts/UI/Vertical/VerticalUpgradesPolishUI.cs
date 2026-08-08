using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(200)]
[DisallowMultipleComponent]
public sealed class VerticalUpgradesPolishUI : MonoBehaviour
{
    [Serializable]
    public sealed class SectionBinding
    {
        public Image railGlow;
        public Image railCore;
        public Image iconGlow;
        public RectTransform icon;
        public Color accent = Color.cyan;
    }

    [Serializable]
    public sealed class RowBinding
    {
        public F2UpgradeRowUI row;
        public Image frame;
        public Image innerFrame;
        public Image accentBar;
        public Image iconGlow;
        public Image icon;
        public Image buttonFrame;
        public Color accent = Color.cyan;
    }

    [Header("Cabecera compartida visualmente")]
    public TextMeshProUGUI sourceLeText;
    public TextMeshProUGUI sourceTracesText;
    public TextMeshProUGUI sourceEnergyText;
    public TextMeshProUGUI leText;
    public TextMeshProUGUI tracesText;
    public TextMeshProUGUI energyText;

    [Header("Tema")]
    public VerticalUiTheme theme;

    [Header("Animacion decorativa")]
    public SectionBinding[] sections;
    public RowBinding[] rows;

    private const float StateRefreshInterval = 0.20f;
    private float nextStateRefresh;

    private void OnEnable()
    {
        nextStateRefresh = 0f;
        CopyResourceText();
        RefreshRowVisuals();
    }

    private void Update()
    {
        CopyResourceText();
        AnimateSections();

        if (Time.unscaledTime < nextStateRefresh) return;
        nextStateRefresh = Time.unscaledTime + StateRefreshInterval;
        RefreshRowVisuals();
    }

    private void CopyResourceText()
    {
        if (leText != null && sourceLeText != null && leText.text != sourceLeText.text)
            leText.text = sourceLeText.text;
        if (tracesText != null && sourceTracesText != null &&
            tracesText.text != sourceTracesText.text)
            tracesText.text = sourceTracesText.text;
        if (energyText != null && sourceEnergyText != null &&
            energyText.text != sourceEnergyText.text)
            energyText.text = sourceEnergyText.text;
    }

    private void AnimateSections()
    {
        if (sections == null) return;
        float wave = 0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * 2.8f);
        foreach (SectionBinding section in sections)
        {
            if (section == null) continue;
            if (section.railGlow != null)
            {
                Color color = section.accent;
                color.a = 0.10f + 0.08f * wave;
                section.railGlow.color = color;
            }
            if (section.railCore != null)
            {
                Color color = section.accent;
                color.a = 0.78f + 0.22f * wave;
                section.railCore.color = color;
            }
            if (section.iconGlow != null)
            {
                Color color = section.accent;
                color.a = 0.10f + 0.06f * wave;
                section.iconGlow.color = color;
            }
            if (section.icon != null)
                section.icon.localScale = Vector3.one * (1f + 0.012f * wave);
        }
    }

    private void RefreshRowVisuals()
    {
        if (rows == null) return;
        F2UpgradeManager manager = F2UpgradeManager.I;
        foreach (RowBinding binding in rows)
        {
            if (binding?.row == null) continue;
            bool maxed = manager != null && manager.IsMaxed(binding.row.UpgradeId);
            Color accent = binding.accent;

            SetAlphaColor(binding.frame, accent, 0.82f);
            SetAlphaColor(binding.innerFrame, accent, 0.20f);
            SetAlphaColor(binding.accentBar, accent, 0.92f);
            SetAlphaColor(binding.iconGlow, accent, 0.14f);
            if (binding.icon != null) binding.icon.color = Color.white;

            if (binding.row.TitleText != null)
                binding.row.TitleText.color = theme != null
                    ? theme.primaryText
                    : Color.white;
            if (binding.row.DescriptionText != null)
                binding.row.DescriptionText.color = theme != null
                    ? theme.secondaryText
                    : new Color(0.62f, 0.72f, 0.82f, 1f);
            if (binding.row.CostText != null)
                binding.row.CostText.color = accent;
            if (binding.row.TierText != null)
                binding.row.TierText.color = accent;

            Button button = binding.row.BuyButton;
            if (button == null) continue;
            Color buttonAccent = maxed && theme != null ? theme.completed : accent;
            SetAlphaColor(binding.buttonFrame, buttonAccent, 1f);
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.white;
            colors.pressedColor = new Color(0.82f, 0.90f, 0.96f, 1f);
            colors.selectedColor = Color.white;
            colors.disabledColor = Color.white;
            colors.colorMultiplier = 1f;
            button.colors = colors;

            TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>(true);
            if (label != null) label.color = buttonAccent;
        }
    }

    private static void SetAlphaColor(Image image, Color color, float alpha)
    {
        if (image == null) return;
        color.a = alpha;
        image.color = color;
    }
}

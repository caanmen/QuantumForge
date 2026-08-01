using TMPro;
using UnityEngine;

[CreateAssetMenu(
    fileName = "VerticalUiTheme",
    menuName = "Quantum Forge/UI/Vertical UI Theme")]
public sealed class VerticalUiTheme : ScriptableObject
{
    public int schemaVersion = 1;

    [Header("Sprites modulares")]
    public Sprite backgroundGrid;
    public Sprite panelFrame;
    public Sprite buttonFrame;
    public Sprite selectedButtonFrame;
    public Sprite softGlow;

    [Header("Iconos de navegacion")]
    public Sprite generationIcon;
    public Sprite upgradesIcon;
    public Sprite researchIcon;
    public Sprite settingsIcon;
    public Sprite qaIcon;
    public Sprite room2Icon;
    public Sprite dimension1Icon;
    public Sprite dimension2Icon;
    public Sprite dimension3Icon;
    public Sprite prestigeIcon;

    [Header("Iconos de artefactos")]
    public Sprite higgsArtifactIcon;
    public Sprite tetraArtifactIcon;
    public Sprite modulatorArtifactIcon;

    [Header("Tipografia")]
    public TMP_FontAsset primaryFont;

    [Header("Paleta")]
    public Color background = new(0.008f, 0.027f, 0.043f, 1f);
    public Color deepSurface = new(0.012f, 0.047f, 0.071f, 0.98f);
    public Color panel = new(0.018f, 0.071f, 0.102f, 0.98f);
    public Color border = new(0.075f, 0.365f, 0.560f, 0.92f);
    public Color primaryText = new(0.925f, 0.957f, 1f, 1f);
    public Color secondaryText = new(0.600f, 0.710f, 0.820f, 1f);
    public Color energy = new(0f, 0.835f, 1f, 1f);
    public Color traces = new(0.725f, 0.310f, 0.925f, 1f);
    public Color triangle = new(1f, 0.595f, 0.035f, 1f);
    public Color completed = new(0.310f, 0.870f, 0.265f, 1f);

    public void ResetToApprovedDefaults()
    {
        schemaVersion = 1;
        background = Hex("02070B");
        deepSurface = Hex("030C12", 250);
        panel = Hex("05121A", 250);
        border = Hex("0A354D", 220);
        primaryText = Hex("ECF4FF");
        secondaryText = Hex("99B5D1");
        energy = Hex("00D5FF");
        traces = Hex("B94FED");
        triangle = Hex("FF9820");
        completed = Hex("4FDE43");
    }

    private static Color Hex(string rgb, byte alpha = 255)
    {
        ColorUtility.TryParseHtmlString("#" + rgb, out Color color);
        color.a = alpha / 255f;
        return color;
    }
}

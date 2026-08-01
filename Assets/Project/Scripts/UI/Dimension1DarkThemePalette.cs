using UnityEngine;

public static class Dimension1DarkThemePalette
{
    public static readonly Color Background = Hex("090A13");
    public static readonly Color DeepSurface = Hex("141A2B");
    public static readonly Color Panel = Hex("242E4D", 250);
    public static readonly Color Elevated = Hex("2D3C63");
    public static readonly Color Border = Hex("46597D", 220);
    public static readonly Color PrimaryText = Hex("E6EDFF");
    public static readonly Color SecondaryText = Hex("B8C5DC");
    public static readonly Color MutedText = Hex("8F9DB5");
    public static readonly Color Accent = Hex("6BADEB");
    public static readonly Color Active = Hex("43C6DB");
    public static readonly Color Success = Hex("63D69A");
    public static readonly Color Warning = Hex("F2C66D");
    public static readonly Color Error = Hex("E06C75");
    public static readonly Color ButtonNormal = Hex("244D73");
    public static readonly Color ButtonHighlighted = Hex("2D6696");
    public static readonly Color ButtonPressed = Hex("193B59");
    public static readonly Color ButtonSelected = Hex("237B86");
    public static readonly Color ButtonDisabled = Hex("283247");
    public static readonly Color Completed = Hex("275E4A");
    public static readonly Color ResetNormal = Hex("5A2931");
    public static readonly Color ResetHighlighted = Hex("783943");
    public static readonly Color QaNormal = Hex("3A334F");
    public static readonly Color QaActive = Hex("9A5B22");

    public static Color Hex(string rgb, byte alpha = 255)
    {
        ColorUtility.TryParseHtmlString("#" + rgb, out Color color);
        color.a = alpha / 255f;
        return color;
    }
}

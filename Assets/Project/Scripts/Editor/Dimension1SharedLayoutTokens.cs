#if UNITY_EDITOR
using UnityEngine;

/// <summary>
/// Único propietario de las medidas estructurales compartidas por las pantallas
/// verticales de Dimensión 1. El contenido interno de cada pantalla sigue siendo
/// independiente; sólo se comparten lienzo, encabezado, marco y navegación local.
/// </summary>
public static class Dimension1SharedLayoutTokens
{
    public const float Width = 1080f;
    public const float Height = 1920f;

    public const float RootOffsetY = -82f;

    public const float OuterFrameX = 10f;
    public const float OuterFrameY = 12f;
    public const float OuterFrameWidth = 1060f;
    public const float OuterFrameHeight = 1892f;

    // Encabezado canónico aprobado: Hangar D1.
    public const float HeaderTitleX = 285f;
    public const float HeaderTitleY = 22f;
    public const float HeaderTitleWidth = 510f;
    public const float HeaderTitleHeight = 58f;
    public const float HeaderTitleFontSize = 42f;

    public const float CommandCenterX = 28f;
    public const float CommandCenterY = 58f;
    public const float CommandCenterWidth = 168f;
    public const float CommandCenterHeight = 76f;

    public const float MetalCardX = 208f;
    public const float MetalCardY = 86f;
    public const float MetalCardWidth = 188f;
    public const float MetalCardHeight = 100f;
    public const float MetalCardStep = 197f;

    public const float AllMetalsX = 799f;
    public const float AllMetalsY = 86f;
    public const float AllMetalsWidth = 245f;
    public const float AllMetalsHeight = 100f;

    public const float NavigationX = 24f;
    public const float NavigationY = 1704f;
    public const float NavigationWidth = 1032f;
    public const float NavigationHeight = 180f;

    public const float NavigationCardY = 2f;
    public const float NavigationCardWidth = 198f;
    public const float NavigationCardHeight = 176f;
    public const float NavigationCardStep = 207f;
    public const float NavigationLabelX = 10f;
    public const float NavigationLabelY = 118f;
    public const float NavigationLabelWidth = 178f;
    public const float NavigationLabelHeight = 34f;
    public const float NavigationLabelFontSize = 22f;

    public static Vector2 RootOffset => new Vector2(0f, RootOffsetY);
    public static float NavigationCardX(int index) => index * NavigationCardStep;
}
#endif

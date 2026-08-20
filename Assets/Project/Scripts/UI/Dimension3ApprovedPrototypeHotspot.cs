using UnityEngine;

/// <summary>
/// Semantic marker used by the isolated approved Dimension 3 prototype.
/// It does not call game logic yet; it documents the future binding target.
/// </summary>
public sealed class Dimension3ApprovedPrototypeHotspot : MonoBehaviour
{
    public string semanticId;

    [TextArea]
    public string futureBinding;

    public Rect sourcePixelRect;
}

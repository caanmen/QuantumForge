using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public sealed class MachineCubeSideGraphic : MaskableGraphic
{
    [SerializeField] private Color outerColor = new Color(0.015f, 0.03f, 0.04f, 1f);
    [SerializeField, Range(0f, 0.2f)] private float taper = 0.085f;

    public void Configure(Color inner, Color outer)
    {
        color = inner;
        outerColor = outer;
        raycastTarget = false;
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        Rect r = GetPixelAdjustedRect();
        float inset = r.height * taper;

        AddVertex(vh, new Vector2(r.xMin, r.yMin), color);
        AddVertex(vh, new Vector2(r.xMin, r.yMax), color);
        AddVertex(vh, new Vector2(r.xMax, r.yMax - inset), outerColor);
        AddVertex(vh, new Vector2(r.xMax, r.yMin + inset), outerColor);
        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(0, 2, 3);
    }

    private static void AddVertex(VertexHelper vh, Vector2 position, Color tint)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.position = position;
        vertex.color = tint;
        vh.AddVert(vertex);
    }
}

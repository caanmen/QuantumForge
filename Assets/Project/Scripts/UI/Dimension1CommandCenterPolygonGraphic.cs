using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public sealed class Dimension1CommandCenterPolygonGraphic : MaskableGraphic
{
    [SerializeField] private List<Vector2> points = new List<Vector2>();

    public void SetPolygon(IList<Vector2> value)
    {
        points = value == null ? new List<Vector2>() : new List<Vector2>(value);
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        if (points == null || points.Count < 3) return;

        for (int i = 0; i < points.Count; i++)
        {
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;
            vertex.position = points[i];
            vh.AddVert(vertex);
        }

        for (int i = 1; i < points.Count - 1; i++)
            vh.AddTriangle(0, i, i + 1);
    }
}

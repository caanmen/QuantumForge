using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public sealed class D1HubLineGraphic : MaskableGraphic
{
    [SerializeField] private List<Vector2> points = new List<Vector2>();
    [SerializeField] private float thickness = 2f;
    [SerializeField] private bool closed;

    public void SetLine(IList<Vector2> value, float width, bool close = false)
    {
        points = new List<Vector2>(value);
        thickness = width;
        closed = close;
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        if (points == null || points.Count < 2) return;

        int segmentCount = closed ? points.Count : points.Count - 1;
        for (int i = 0; i < segmentCount; i++)
        {
            Vector2 a = points[i];
            Vector2 b = points[(i + 1) % points.Count];
            Vector2 direction = (b - a).normalized;
            Vector2 normal = new Vector2(-direction.y, direction.x) * thickness * 0.5f;
            int start = vh.currentVertCount;
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;
            vertex.position = a - normal; vh.AddVert(vertex);
            vertex.position = a + normal; vh.AddVert(vertex);
            vertex.position = b + normal; vh.AddVert(vertex);
            vertex.position = b - normal; vh.AddVert(vertex);
            vh.AddTriangle(start, start + 1, start + 2);
            vh.AddTriangle(start, start + 2, start + 3);
        }
    }
}

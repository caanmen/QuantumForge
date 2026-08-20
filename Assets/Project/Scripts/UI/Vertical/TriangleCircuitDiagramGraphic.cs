using UnityEngine;
using UnityEngine.UI;

public sealed class TriangleCircuitDiagramGraphic : MaskableGraphic
{
    [Min(1f)] public float lineThickness = 4f;
    [Min(1f)] public float glowThickness = 7f;
    [Min(2f)] public float nodeRadius = 8f;
    [Min(1f)] public float nodeThickness = 3f;
    [Min(0f)] public float coreRadius = 2.4f;
    [Range(6, 32)] public int nodeSegments = 16;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        Rect rect = GetPixelAdjustedRect();
        Vector2 center = rect.center;
        float width = rect.width * 0.30f;
        float height = rect.height * 0.29f;
        Vector2 top = center + new Vector2(0f, height);
        Vector2 left = center + new Vector2(-width, -height * 0.78f);
        Vector2 right = center + new Vector2(width, -height * 0.78f);

        AddFilledTriangle(vh, top, left, right,
            new Color(0.005f, 0.007f, 0.009f, 0.88f));
        Color glow = color;
        glow.a *= 0.18f;
        AddLine(vh, top, left, glowThickness, glow);
        AddLine(vh, left, right, glowThickness, glow);
        AddLine(vh, right, top, glowThickness, glow);
        AddLine(vh, top, left, lineThickness, color);
        AddLine(vh, left, right, lineThickness, color);
        AddLine(vh, right, top, lineThickness, color);
        AddRing(vh, top, nodeRadius + 3.5f, 2f, glow);
        AddRing(vh, left, nodeRadius + 3.5f, 2f, glow);
        AddRing(vh, right, nodeRadius + 3.5f, 2f, glow);
        AddRing(vh, top, nodeRadius, nodeThickness, color);
        AddRing(vh, left, nodeRadius, nodeThickness, color);
        AddRing(vh, right, nodeRadius, nodeThickness, color);
        Color core = Color.Lerp(color, Color.white, 0.52f);
        AddDisc(vh, top, coreRadius, core);
        AddDisc(vh, left, coreRadius, core);
        AddDisc(vh, right, coreRadius, core);
    }

    private static void AddFilledTriangle(
        VertexHelper vh,
        Vector2 top,
        Vector2 left,
        Vector2 right,
        Color tint)
    {
        int index = vh.currentVertCount;
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = tint;
        vertex.position = top;
        vh.AddVert(vertex);
        vertex.position = left;
        vh.AddVert(vertex);
        vertex.position = right;
        vh.AddVert(vertex);
        vh.AddTriangle(index, index + 1, index + 2);
    }

    private static void AddLine(
        VertexHelper vh, Vector2 start, Vector2 end, float thickness, Color tint)
    {
        Vector2 direction = (end - start).normalized;
        Vector2 normal = new Vector2(-direction.y, direction.x) * thickness * 0.5f;
        int index = vh.currentVertCount;
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = tint;
        vertex.position = start - normal; vh.AddVert(vertex);
        vertex.position = start + normal; vh.AddVert(vertex);
        vertex.position = end + normal; vh.AddVert(vertex);
        vertex.position = end - normal; vh.AddVert(vertex);
        vh.AddTriangle(index, index + 1, index + 2);
        vh.AddTriangle(index, index + 2, index + 3);
    }

    private void AddRing(
        VertexHelper vh, Vector2 center, float radius, float thickness, Color tint)
    {
        float innerRadius = Mathf.Max(1f, radius - thickness);
        int start = vh.currentVertCount;
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = tint;

        for (int i = 0; i <= nodeSegments; i++)
        {
            float angle = Mathf.PI * 2f * i / nodeSegments;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            vertex.position = center + direction * radius;
            vh.AddVert(vertex);
            vertex.position = center + direction * innerRadius;
            vh.AddVert(vertex);
        }

        for (int i = 0; i < nodeSegments; i++)
        {
            int outer = start + i * 2;
            int inner = outer + 1;
            int nextOuter = outer + 2;
            int nextInner = outer + 3;
            vh.AddTriangle(outer, nextOuter, nextInner);
            vh.AddTriangle(outer, nextInner, inner);
        }
    }

    private void AddDisc(VertexHelper vh, Vector2 center, float radius, Color tint)
    {
        if (radius <= 0f) return;
        int centerIndex = vh.currentVertCount;
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = tint;
        vertex.position = center;
        vh.AddVert(vertex);
        for (int i = 0; i <= nodeSegments; i++)
        {
            float angle = Mathf.PI * 2f * i / nodeSegments;
            vertex.position = center +
                new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            vh.AddVert(vertex);
        }
        for (int i = 0; i < nodeSegments; i++)
            vh.AddTriangle(centerIndex, centerIndex + i + 1, centerIndex + i + 2);
    }
}

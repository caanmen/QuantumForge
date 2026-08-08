using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class PrestigePortal2DGraphic : MaskableGraphic
{
    [SerializeField] private Color portalColor = Color.cyan;
    [SerializeField, Range(0f, 1f)] private float intensity;
    [SerializeField, Range(20, 64)] private int segments = 48;

    public void Configure(Color value)
    {
        portalColor = value;
        raycastTarget = false;
        SetVerticesDirty();
    }

    public void SetIntensity(float value)
    {
        value = Mathf.Clamp01(value);
        if (Mathf.Approximately(intensity, value))
            return;
        intensity = value;
        SetVerticesDirty();
    }

    public void SetLowQuality(bool lowQuality)
    {
        int next = lowQuality ? 24 : 48;
        if (segments == next)
            return;
        segments = next;
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vertexHelper)
    {
        vertexHelper.Clear();
        if (intensity <= 0.001f)
            return;

        Rect rect = GetPixelAdjustedRect();
        Vector2 center = rect.center;
        Vector2 radius = rect.size * 0.5f;

        AddDisc(vertexHelper, center, radius * 0.76f,
            WithAlpha(portalColor * 0.68f, intensity * 0.25f),
            WithAlpha(portalColor, intensity * 0.065f));
        AddRing(vertexHelper, center, radius * 0.78f, radius * 0.98f,
            WithAlpha(portalColor, intensity * 0.34f),
            WithAlpha(portalColor, 0f));
    }

    private void AddDisc(VertexHelper vertexHelper, Vector2 center,
        Vector2 radius, Color centerColor, Color edgeColor)
    {
        int centerIndex = vertexHelper.currentVertCount;
        AddVertex(vertexHelper, center, centerColor);
        for (int i = 0; i <= segments; i++)
        {
            float angle = i / (float)segments * Mathf.PI * 2f;
            AddVertex(vertexHelper, center + new Vector2(
                Mathf.Cos(angle) * radius.x,
                Mathf.Sin(angle) * radius.y), edgeColor);
        }

        for (int i = 0; i < segments; i++)
            vertexHelper.AddTriangle(centerIndex, centerIndex + i + 1,
                centerIndex + i + 2);
    }

    private void AddRing(VertexHelper vertexHelper, Vector2 center,
        Vector2 innerRadius, Vector2 outerRadius, Color innerColor,
        Color outerColor)
    {
        int start = vertexHelper.currentVertCount;
        for (int i = 0; i <= segments; i++)
        {
            float angle = i / (float)segments * Mathf.PI * 2f;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            AddVertex(vertexHelper, center + new Vector2(
                direction.x * innerRadius.x, direction.y * innerRadius.y),
                innerColor);
            AddVertex(vertexHelper, center + new Vector2(
                direction.x * outerRadius.x, direction.y * outerRadius.y),
                outerColor);
        }

        for (int i = 0; i < segments; i++)
        {
            int index = start + i * 2;
            vertexHelper.AddTriangle(index, index + 1, index + 2);
            vertexHelper.AddTriangle(index + 2, index + 1, index + 3);
        }
    }

    private static void AddVertex(VertexHelper vertexHelper, Vector2 position,
        Color color)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.position = position;
        vertex.color = color;
        vertexHelper.AddVert(vertex);
    }

    private static Color WithAlpha(Color value, float alpha)
    {
        value.a = Mathf.Clamp01(alpha);
        return value;
    }
}

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Dibuja la membrana dimensional del anclaje sin depender de una textura fija.
/// El estado operativo lo controla MachineSeedsPanelVisualUI.
/// </summary>
public sealed class ChronalLatticeGraphic : MaskableGraphic
{
    private const float MeshRefreshInterval = 1f / 20f;

    private static readonly Vector2[] Nodes =
    {
        new(0.14f, 0.22f), new(0.25f, 0.68f), new(0.37f, 0.38f),
        new(0.47f, 0.78f), new(0.58f, 0.24f), new(0.69f, 0.58f),
        new(0.83f, 0.34f), new(0.78f, 0.82f), new(0.42f, 0.12f),
        new(0.18f, 0.48f), new(0.55f, 0.52f), new(0.88f, 0.62f)
    };

    private static readonly Vector2Int[] Edges =
    {
        new(0, 2), new(0, 9), new(1, 2), new(1, 3), new(1, 9),
        new(2, 3), new(2, 4), new(2, 10), new(3, 5), new(3, 7),
        new(3, 10), new(4, 6), new(4, 8), new(4, 10), new(5, 6),
        new(5, 7), new(5, 10), new(5, 11), new(6, 11), new(7, 11),
        new(8, 2), new(9, 3), new(10, 6)
    };

    [SerializeField, Range(0f, 1f)] private float activity;
    [SerializeField, Range(0f, 1f)] private float tension;
    private float meshRefreshTimer;

    private static readonly Color Cyan = Hex("38E5C1");
    private static readonly Color Amber = Hex("F0A018");

    public void SetState(float activity01, float tension01)
    {
        float nextActivity = Mathf.Clamp01(activity01);
        float nextTension = Mathf.Clamp01(tension01);
        if (Mathf.Approximately(activity, nextActivity) &&
            Mathf.Approximately(tension, nextTension))
            return;
        activity = nextActivity;
        tension = nextTension;
        SetVerticesDirty();
    }

    private void Update()
    {
        if (activity <= 0.001f)
            return;

        meshRefreshTimer += Time.unscaledDeltaTime;
        if (meshRefreshTimer < MeshRefreshInterval)
            return;

        meshRefreshTimer = 0f;
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        Rect bounds = rectTransform.rect;
        if (bounds.width <= 1f || bounds.height <= 1f)
            return;

        float pulse = 0.72f + 0.28f * Mathf.Sin(Time.unscaledTime * 3.1f);
        Color activeColor = Color.Lerp(Cyan, Amber, tension * 0.72f);
        activeColor.a = Mathf.Lerp(0.08f, 0.92f, activity) * pulse;
        Color dimColor = activeColor;
        dimColor.a *= 0.42f;

        for (int i = 0; i < Edges.Length; i++)
        {
            Vector2 a = ToLocal(Nodes[Edges[i].x], bounds);
            Vector2 b = ToLocal(Nodes[Edges[i].y], bounds);
            AddLine(vh, a, b, Mathf.Lerp(1.1f, 2.6f, activity),
                i % 3 == 0 ? activeColor : dimColor);
        }

        for (int i = 0; i < Nodes.Length; i++)
        {
            Vector2 center = ToLocal(Nodes[i], bounds);
            float nodePulse = 0.72f + 0.28f * Mathf.Sin(
                Time.unscaledTime * 4.0f + i * 0.81f);
            AddDiamond(vh, center, Mathf.Lerp(2.4f, 5.4f, activity) * nodePulse,
                i % 4 == 0 ? activeColor : dimColor);
        }
    }

    private static Vector2 ToLocal(Vector2 normalized, Rect bounds)
    {
        return new Vector2(
            Mathf.Lerp(bounds.xMin, bounds.xMax, normalized.x),
            Mathf.Lerp(bounds.yMin, bounds.yMax, normalized.y));
    }

    private static void AddLine(VertexHelper vh, Vector2 a, Vector2 b,
        float thickness, Color lineColor)
    {
        Vector2 direction = (b - a).normalized;
        Vector2 normal = new Vector2(-direction.y, direction.x) * thickness;
        AddQuad(vh, a - normal, a + normal, b + normal, b - normal, lineColor);
    }

    private static void AddDiamond(VertexHelper vh, Vector2 center, float radius,
        Color nodeColor)
    {
        AddQuad(vh,
            center + Vector2.left * radius,
            center + Vector2.up * radius,
            center + Vector2.right * radius,
            center + Vector2.down * radius,
            nodeColor);
    }

    private static void AddQuad(VertexHelper vh, Vector2 a, Vector2 b,
        Vector2 c, Vector2 d, Color quadColor)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = quadColor;
        int start = vh.currentVertCount;
        vertex.position = a; vh.AddVert(vertex);
        vertex.position = b; vh.AddVert(vertex);
        vertex.position = c; vh.AddVert(vertex);
        vertex.position = d; vh.AddVert(vertex);
        vh.AddTriangle(start, start + 1, start + 2);
        vh.AddTriangle(start, start + 2, start + 3);
    }

    private static Color Hex(string value)
    {
        ColorUtility.TryParseHtmlString("#" + value, out Color parsed);
        return parsed;
    }
}

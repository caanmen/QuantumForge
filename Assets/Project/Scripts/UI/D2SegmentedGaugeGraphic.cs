using UnityEngine;
using UnityEngine.UI;


[ExecuteAlways]
public sealed class D2SegmentedGaugeGraphic : MaskableGraphic
{
    [SerializeField, Range(0f, 1f)] private float progress;
    [SerializeField, Min(1)] private int segmentCount = 14;
    [SerializeField, Range(0.05f, 0.49f)] private float innerRadius = 0.39f;
    [SerializeField, Range(0.06f, 0.50f)] private float outerRadius = 0.49f;
    [SerializeField, Range(0f, 12f)] private float gapDegrees = 3.2f;
    [SerializeField] private float startAngleDegrees = 90f;
    [SerializeField] private bool clockwise = true;

    public float Progress => progress;

    public void Configure(
        int segments,
        float inner,
        float outer,
        float gap,
        float startAngle,
        bool drawClockwise)
    {
        segmentCount = Mathf.Max(1, segments);
        innerRadius = Mathf.Clamp(inner, 0.05f, 0.49f);
        outerRadius = Mathf.Clamp(outer, innerRadius + 0.01f, 0.50f);
        gapDegrees = Mathf.Clamp(gap, 0f, 12f);
        startAngleDegrees = startAngle;
        clockwise = drawClockwise;
        SetVerticesDirty();
    }

    public void SetProgress(float normalizedValue)
    {
        float next = Mathf.Clamp01(normalizedValue);
        if (Mathf.Approximately(progress, next))
            return;
        progress = next;
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vertexHelper)
    {
        vertexHelper.Clear();
        if (segmentCount <= 0 || progress <= 0f)
            return;

        Rect rect = rectTransform.rect;
        float diameter = Mathf.Min(rect.width, rect.height);
        float inner = diameter * innerRadius;
        float outer = diameter * outerRadius;
        float sweep = 360f / segmentCount;
        int visibleSegments = Mathf.Clamp(
            Mathf.CeilToInt(progress * segmentCount), 0, segmentCount);
        float direction = clockwise ? -1f : 1f;
        Vector2 center = rect.center;
        Color32 vertexColor = color;

        for (int index = 0; index < visibleSegments; index++)
        {
            float segmentStart = startAngleDegrees + direction * index * sweep;
            float segmentEnd = startAngleDegrees + direction * (index + 1) * sweep;
            float inset = gapDegrees * 0.5f;
            segmentStart += direction * inset;
            segmentEnd -= direction * inset;
            AddSegment(vertexHelper, center, inner, outer,
                segmentStart * Mathf.Deg2Rad, segmentEnd * Mathf.Deg2Rad,
                vertexColor);
        }
    }

    private static void AddSegment(
        VertexHelper vertexHelper,
        Vector2 center,
        float inner,
        float outer,
        float start,
        float end,
        Color32 color)
    {
        Vector2 startDirection = new Vector2(Mathf.Cos(start), Mathf.Sin(start));
        Vector2 endDirection = new Vector2(Mathf.Cos(end), Mathf.Sin(end));
        int first = vertexHelper.currentVertCount;
        vertexHelper.AddVert(center + startDirection * inner, color, Vector2.zero);
        vertexHelper.AddVert(center + startDirection * outer, color, Vector2.up);
        vertexHelper.AddVert(center + endDirection * outer, color, Vector2.one);
        vertexHelper.AddVert(center + endDirection * inner, color, Vector2.right);
        vertexHelper.AddTriangle(first, first + 1, first + 2);
        vertexHelper.AddTriangle(first, first + 2, first + 3);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        segmentCount = Mathf.Max(1, segmentCount);
        outerRadius = Mathf.Clamp(outerRadius, innerRadius + 0.01f, 0.50f);
        SetVerticesDirty();
    }
#endif
}

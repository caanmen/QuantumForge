using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public sealed class PrestigeChamferedPanelGraphic : MaskableGraphic
{
    [SerializeField] private Color fillColor = new Color(.02f, .04f, .08f, .96f);
    [SerializeField] private Color borderColor = new Color(.05f, .65f, .82f, .9f);
    [SerializeField, Min(0f)] private float borderWidth = 2f;
    [SerializeField, Min(0f)] private float cutSize = 12f;

    public Color FillColor
    {
        get => fillColor;
        set { fillColor = value; SetVerticesDirty(); }
    }

    public Color BorderColor
    {
        get => borderColor;
        set { borderColor = value; SetVerticesDirty(); }
    }

    public void SetStyle(Color fill, Color border, float width, float cut)
    {
        fillColor = fill;
        borderColor = border;
        borderWidth = Mathf.Max(0f, width);
        cutSize = Mathf.Max(0f, cut);
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        Rect rect = GetPixelAdjustedRect();
        float cut = Mathf.Min(cutSize, Mathf.Min(rect.width, rect.height) * .45f);
        Vector2[] outer = BuildPoints(rect, 0f, cut);
        float inset = Mathf.Min(borderWidth, Mathf.Min(rect.width, rect.height) * .24f);
        Vector2[] inner = BuildPoints(rect, inset, Mathf.Max(0f, cut - inset * .42f));

        AddFan(vh, inner, fillColor);
        if (inset <= .01f)
            return;

        for (int i = 0; i < outer.Length; i++)
        {
            int next = (i + 1) % outer.Length;
            AddQuad(vh, outer[i], outer[next], inner[next], inner[i], borderColor);
        }
    }

    private static Vector2[] BuildPoints(Rect rect, float inset, float cut)
    {
        float left = rect.xMin + inset;
        float right = rect.xMax - inset;
        float bottom = rect.yMin + inset;
        float top = rect.yMax - inset;
        return new[]
        {
            new Vector2(left + cut, top), new Vector2(right - cut, top),
            new Vector2(right, top - cut), new Vector2(right, bottom + cut),
            new Vector2(right - cut, bottom), new Vector2(left + cut, bottom),
            new Vector2(left, bottom + cut), new Vector2(left, top - cut)
        };
    }

    private static void AddFan(VertexHelper vh, Vector2[] points, Color color)
    {
        Vector2 center = Vector2.zero;
        foreach (Vector2 point in points) center += point;
        center /= points.Length;
        int centerIndex = AddVertex(vh, center, color);
        int first = vh.currentVertCount;
        foreach (Vector2 point in points) AddVertex(vh, point, color);
        for (int i = 0; i < points.Length; i++)
            vh.AddTriangle(centerIndex, first + ((i + 1) % points.Length), first + i);
    }

    private static void AddQuad(VertexHelper vh, Vector2 a, Vector2 b, Vector2 c,
        Vector2 d, Color color)
    {
        int start = vh.currentVertCount;
        AddVertex(vh, a, color);
        AddVertex(vh, b, color);
        AddVertex(vh, c, color);
        AddVertex(vh, d, color);
        vh.AddTriangle(start, start + 2, start + 1);
        vh.AddTriangle(start, start + 3, start + 2);
    }

    private static int AddVertex(VertexHelper vh, Vector2 position, Color color)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.position = position;
        vertex.color = color;
        vh.AddVert(vertex);
        return vh.currentVertCount - 1;
    }
}

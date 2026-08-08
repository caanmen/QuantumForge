using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Malla UI procedimental para el campo eléctrico de la cámara de fusión.
/// No usa partículas de mundo ni texturas pesadas y responde al estado real.
/// </summary>
[RequireComponent(typeof(CanvasRenderer))]
public sealed class FusionEnergyWaveGraphic : MaskableGraphic
{
    [SerializeField, Range(24, 96)] private int segments = 64;
    [SerializeField] private Color cyan = new Color(0f, 0.79f, 1f, 1f);
    [SerializeField] private Color blue = new Color(0.08f, 0.30f, 1f, 1f);
    [SerializeField] private Color violet = new Color(0.72f, 0.36f, 1f, 1f);

    private float _phase;
    private float _activity = 0.18f;
    private bool _reacting;

    public void SetState(float activity, bool reacting)
    {
        activity = Mathf.Clamp01(activity);
        if (Mathf.Abs(_activity - activity) < 0.005f && _reacting == reacting)
            return;
        _activity = activity;
        _reacting = reacting;
        SetVerticesDirty();
    }

    protected override void Awake()
    {
        base.Awake();
        raycastTarget = false;
    }

    private void Update()
    {
        float speed = Mathf.Lerp(0.7f, 4.8f, _activity);
        if (_reacting) speed *= 1.45f;
        _phase += Time.unscaledDeltaTime * speed;
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        Rect rect = GetPixelAdjustedRect();
        if (rect.width <= 1f || rect.height <= 1f)
            return;

        float centerX = rect.center.x;
        float centerY = rect.center.y;
        float baseAmplitude = rect.height * Mathf.Lerp(0.035f, 0.13f, _activity);
        float opacity = Mathf.Lerp(0.14f, 1f, _activity);

        // Tres capas dan la sensación de resplandor sin usar un shader aditivo.
        DrawWaveLayer(vh, rect, centerY, baseAmplitude, 12f,
            opacity * 0.08f, 0.55f);
        DrawWaveLayer(vh, rect, centerY, baseAmplitude, 5f,
            opacity * 0.24f, 0.85f);
        DrawWaveLayer(vh, rect, centerY, baseAmplitude, 1.7f,
            opacity * 0.92f, 1f);

        // Haz vertical del catalizador y núcleo de convergencia.
        DrawVerticalBeam(vh, rect, centerX, opacity);
        DrawCoreDiamond(vh, centerX, centerY, rect.height, opacity);
    }

    private void DrawWaveLayer(VertexHelper vh, Rect rect, float centerY,
        float amplitude, float width, float alpha, float frequencyScale)
    {
        const int waveCount = 5;
        for (int wave = 0; wave < waveCount; wave++)
        {
            float offset = (wave - (waveCount - 1) * 0.5f) * rect.height * 0.035f;
            float wavePhase = _phase * (1f + wave * 0.085f) + wave * 1.37f;
            Vector2 previous = Vector2.zero;
            Color previousColor = Color.white;

            for (int i = 0; i <= segments; i++)
            {
                float t = i / (float)segments;
                float x = Mathf.Lerp(rect.xMin, rect.xMax, t);
                float convergence = Mathf.Lerp(0.72f, 1.2f,
                    1f - Mathf.Abs(t - 0.5f) * 2f);
                float fineNoise = Mathf.Sin(t * 57f + wavePhase * 1.7f) *
                    amplitude * 0.18f * _activity;
                float y = centerY + offset +
                    Mathf.Sin(t * Mathf.PI * (7.5f + wave * 0.55f) *
                        frequencyScale + wavePhase) * amplitude * convergence +
                    fineNoise;

                Color waveColor = Color.Lerp(blue, cyan,
                    Mathf.Clamp01(0.35f + t * 0.85f));
                float centerGlow = 1f - Mathf.Clamp01(Mathf.Abs(t - 0.5f) * 3.5f);
                waveColor = Color.Lerp(waveColor, Color.white, centerGlow * 0.82f);
                waveColor.a = alpha * Mathf.Lerp(0.45f, 1f, centerGlow);

                Vector2 current = new Vector2(x, y);
                if (i > 0)
                    AddSegment(vh, previous, current, width,
                        previousColor, waveColor);
                previous = current;
                previousColor = waveColor;
            }
        }
    }

    private void DrawVerticalBeam(VertexHelper vh, Rect rect, float centerX,
        float opacity)
    {
        float pulse = 0.78f + 0.22f * Mathf.Sin(_phase * 3.1f);
        AddQuad(vh,
            new Vector2(centerX - 13f, rect.yMin),
            new Vector2(centerX + 13f, rect.yMax),
            WithAlpha(violet, opacity * 0.07f * pulse));
        AddQuad(vh,
            new Vector2(centerX - 5f, rect.yMin),
            new Vector2(centerX + 5f, rect.yMax),
            WithAlpha(violet, opacity * 0.22f * pulse));
        AddQuad(vh,
            new Vector2(centerX - 1.3f, rect.yMin),
            new Vector2(centerX + 1.3f, rect.yMax),
            WithAlpha(Color.white, opacity * 0.85f * pulse));
    }

    private void DrawCoreDiamond(VertexHelper vh, float centerX, float centerY,
        float height, float opacity)
    {
        float radius = height * Mathf.Lerp(0.06f, 0.12f, _activity);
        Color glow = WithAlpha(Color.white, opacity * 0.9f);
        int start = vh.currentVertCount;
        AddVert(vh, new Vector2(centerX, centerY + radius), glow);
        AddVert(vh, new Vector2(centerX + radius, centerY), glow);
        AddVert(vh, new Vector2(centerX, centerY - radius), glow);
        AddVert(vh, new Vector2(centerX - radius, centerY), glow);
        vh.AddTriangle(start, start + 1, start + 2);
        vh.AddTriangle(start, start + 2, start + 3);
    }

    private static void AddSegment(VertexHelper vh, Vector2 a, Vector2 b,
        float width, Color colorA, Color colorB)
    {
        Vector2 direction = b - a;
        if (direction.sqrMagnitude < 0.001f)
            return;
        Vector2 normal = new Vector2(-direction.y, direction.x).normalized *
            (width * 0.5f);
        int start = vh.currentVertCount;
        AddVert(vh, a - normal, colorA);
        AddVert(vh, a + normal, colorA);
        AddVert(vh, b + normal, colorB);
        AddVert(vh, b - normal, colorB);
        vh.AddTriangle(start, start + 1, start + 2);
        vh.AddTriangle(start, start + 2, start + 3);
    }

    private static void AddQuad(VertexHelper vh, Vector2 min, Vector2 max,
        Color color)
    {
        int start = vh.currentVertCount;
        AddVert(vh, new Vector2(min.x, min.y), color);
        AddVert(vh, new Vector2(min.x, max.y), color);
        AddVert(vh, new Vector2(max.x, max.y), color);
        AddVert(vh, new Vector2(max.x, min.y), color);
        vh.AddTriangle(start, start + 1, start + 2);
        vh.AddTriangle(start, start + 2, start + 3);
    }

    private static void AddVert(VertexHelper vh, Vector2 position, Color color)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.position = position;
        vertex.color = color;
        vh.AddVert(vertex);
    }

    private static Color WithAlpha(Color color, float alpha)
    {
        color.a = Mathf.Clamp01(alpha);
        return color;
    }
}

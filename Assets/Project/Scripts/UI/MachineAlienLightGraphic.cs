using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public sealed class MachineAlienLightGraphic : MaskableGraphic
{
    private const float AlphaRefreshInterval = 1f / 20f;

    [SerializeField] private int signatureSeed = 1;
    [SerializeField] private bool compactFaceLight;
    [SerializeField] private float pulseSpeed = 1.35f;

    private float _baseAlpha = 1f;
    private float _nextAlphaRefreshTime;

    public void Configure(int seed, bool compact)
    {
        signatureSeed = Mathf.Max(1, seed);
        compactFaceLight = compact;
        SetVerticesDirty();
    }

    protected override void Awake()
    {
        base.Awake();
        raycastTarget = false;
        _baseAlpha = color.a;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _baseAlpha = color.a;
        SetVerticesDirty();
    }

    private void Update()
    {
        if (!Application.isPlaying)
            return;

        if (Time.unscaledTime < _nextAlphaRefreshTime)
            return;
        _nextAlphaRefreshTime = Time.unscaledTime + AlphaRefreshInterval;

        float phase = signatureSeed * 0.73f;
        float pulse = .78f + .22f * Mathf.Sin(Time.unscaledTime * pulseSpeed + phase);
        canvasRenderer.SetAlpha(_baseAlpha * pulse);
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        Rect rect = GetPixelAdjustedRect();
        float scale = Mathf.Min(rect.width, rect.height);
        Vector2 center = rect.center;

        Random.State previous = Random.state;
        Random.InitState(signatureSeed * 7919);

        Vector2[] spine = compactFaceLight
            ? BuildFaceSeam(center, scale)
            : BuildSignature(center, scale);

        AddPolyline(vh, spine, scale * .20f,
            new Color(.20f, .92f, 1f, .055f));
        AddPolyline(vh, spine, scale * .095f,
            new Color(.28f, .94f, 1f, .14f));
        AddPolyline(vh, spine, scale * .026f,
            new Color(.72f, .98f, 1f, .96f));

        if (!compactFaceLight)
        {
            for (int i = 1; i < spine.Length - 1; i++)
            {
                float side = i % 2 == 0 ? 1f : -1f;
                float length = scale * Random.Range(.16f, .27f);
                Vector2 direction = new Vector2(side * Random.Range(.62f, .92f),
                    Random.Range(-.48f, .48f)).normalized;
                Vector2[] branch =
                {
                    spine[i],
                    spine[i] + direction * length * .58f,
                    spine[i] + direction * length
                };
                AddPolyline(vh, branch, scale * .105f,
                    new Color(.20f, .92f, 1f, .045f));
                AddPolyline(vh, branch, scale * .050f,
                    new Color(.28f, .94f, 1f, .13f));
                AddPolyline(vh, branch, scale * .018f,
                    new Color(.72f, .98f, 1f, .90f));
            }
        }

        Random.state = previous;
    }

    private static Vector2[] BuildFaceSeam(Vector2 center, float scale)
    {
        float lean = Random.Range(-.10f, .10f) * scale;
        return new[]
        {
            center + new Vector2(-.04f * scale, -.33f * scale),
            center + new Vector2(.04f * scale + lean, -.14f * scale),
            center + new Vector2(-.05f * scale + lean, .05f * scale),
            center + new Vector2(.03f * scale, .30f * scale)
        };
    }

    private static Vector2[] BuildSignature(Vector2 center, float scale)
    {
        return new[]
        {
            center + new Vector2(Random.Range(-.08f, .03f), -.34f) * scale,
            center + new Vector2(Random.Range(-.03f, .08f), -.15f) * scale,
            center + new Vector2(Random.Range(-.08f, .08f), .03f) * scale,
            center + new Vector2(Random.Range(-.04f, .08f), .20f) * scale,
            center + new Vector2(Random.Range(-.08f, .03f), .35f) * scale
        };
    }

    private static void AddPolyline(VertexHelper vh, Vector2[] points,
        float width, Color tint)
    {
        if (points == null || points.Length < 2 || width <= 0f)
            return;

        for (int i = 0; i < points.Length - 1; i++)
            AddSegment(vh, points[i], points[i + 1], width, tint);
    }

    private static void AddSegment(VertexHelper vh, Vector2 a, Vector2 b,
        float width, Color tint)
    {
        Vector2 direction = b - a;
        if (direction.sqrMagnitude < .001f)
            return;

        Vector2 normal = new Vector2(-direction.y, direction.x).normalized * width * .5f;
        int start = vh.currentVertCount;
        vh.AddVert(a - normal, tint, Vector2.zero);
        vh.AddVert(a + normal, tint, Vector2.up);
        vh.AddVert(b + normal, tint, Vector2.one);
        vh.AddVert(b - normal, tint, Vector2.right);
        vh.AddTriangle(start, start + 1, start + 2);
        vh.AddTriangle(start, start + 2, start + 3);
    }
}

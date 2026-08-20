using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public sealed class GalaxyParticleFieldUI : MaskableGraphic
{
    private const int MaxParticleCount = 36;
    private const float UpdateStep = 1f / 30f;

    [SerializeField] private Vector2 orbitCenter = new Vector2(0f, 5f);
    [SerializeField] private float axisRotation = 0f;
    [SerializeField, Range(0.35f, 1f)] private float axisRatio = 0.68f;
    [SerializeField, Range(20, MaxParticleCount)] private int activeCount = 36;
    [SerializeField] private Color innerColor = new Color(1f, 0.79f, 0.45f, 0.72f);
    [SerializeField] private Color outerColor = new Color(0.25f, 0.82f, 1f, 0.68f);
    [SerializeField] private Texture particleTexture;

    private readonly float[] phases = new float[MaxParticleCount];
    private readonly float[] radii = new float[MaxParticleCount];
    private readonly float[] speeds = new float[MaxParticleCount];
    private readonly float[] sizes = new float[MaxParticleCount];
    private readonly float[] alphaScales = new float[MaxParticleCount];
    private readonly float[] armJitters = new float[MaxParticleCount];
    private float animationTime;
    private float dirtyTimer;
    private Vector2 renderedParticle0;
    private Vector2 renderedParticle12;
    private bool hasRenderedFrame;

    public override Texture mainTexture => particleTexture != null ? particleTexture : s_WhiteTexture;

    public void Configure(Texture softParticleTexture)
    {
        particleTexture = softParticleTexture;
        SetMaterialDirty();
        SetVerticesDirty();
    }

    public void ConfigureLayout(Vector2 center, float rotationDegrees, float projectedAxisRatio, bool highQuality)
    {
        orbitCenter = center;
        axisRotation = rotationDegrees;
        axisRatio = Mathf.Clamp(projectedAxisRatio, 0.35f, 1f);
        activeCount = highQuality ? MaxParticleCount : 24;
        InitializeParticles();
        SetVerticesDirty();
    }

    public Vector2 SampleParticlePosition(int index)
    {
        if (index < 0 || index >= activeCount) return orbitCenter;
        return CalculatePosition(index, animationTime);
    }

    public bool TryGetRenderedParticlePositions(out Vector2 particle0, out Vector2 particle12)
    {
        particle0 = renderedParticle0;
        particle12 = renderedParticle12;
        return hasRenderedFrame;
    }

    protected override void Awake()
    {
        base.Awake();
        raycastTarget = false;
        color = Color.white;
        InitializeParticles();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        animationTime = 0f;
        dirtyTimer = 0f;
        SetVerticesDirty();
    }

    private void InitializeParticles()
    {
        for (int i = 0; i < MaxParticleCount; i++)
        {
            float sequence = Hash01(i * 13 + 7);
            int arm = i & 1;
            float normalizedRadius = (i + 0.5f) / MaxParticleCount;
            radii[i] = Mathf.Lerp(92f, 350f, normalizedRadius) + Mathf.Lerp(-12f, 12f, sequence);
            phases[i] = arm * Mathf.PI + Mathf.Lerp(-0.08f, 0.08f, Hash01(i * 17 + 3));
            speeds[i] = Mathf.Deg2Rad * Mathf.Lerp(2.2f, 5.4f, 1f - normalizedRadius);
            sizes[i] = Mathf.Lerp(4f, 10f, Hash01(i * 29 + 11));
            alphaScales[i] = Mathf.Lerp(0.32f, 0.86f, Hash01(i * 31 + 19));
            armJitters[i] = Mathf.Lerp(-0.10f, 0.10f, Hash01(i * 37 + 23));
        }
    }

    private void Update()
    {
        animationTime += Time.unscaledDeltaTime;
        dirtyTimer += Time.unscaledDeltaTime;
        if (dirtyTimer < UpdateStep) return;
        dirtyTimer -= UpdateStep;
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        for (int i = 0; i < activeCount; i++)
        {
            Vector2 position = CalculatePosition(i, animationTime);
            if (i == 0) renderedParticle0 = position;
            else if (i == 12) renderedParticle12 = position;
            float depth = (Mathf.Sin(phases[i] + animationTime * speeds[i]) + 1f) * 0.5f;
            float size = sizes[i] * Mathf.Lerp(0.72f, 1.18f, depth);
            Color particleColor = Color.Lerp(innerColor, outerColor, radii[i] / 350f);
            particleColor.a *= alphaScales[i] * Mathf.Lerp(0.56f, 1f, depth);
            AddQuad(vh, position, size, particleColor);
        }
        hasRenderedFrame = true;
    }

    private Vector2 CalculatePosition(int index, float time)
    {
        float radius = radii[index];
        float winding = 2.05f * Mathf.Log(Mathf.Max(radius, 1f) / 92f);
        float angle = phases[index] + winding + time * speeds[index] + armJitters[index];
        float armWidth = Mathf.Sin(index * 1.73f + time * 0.31f) * 5f;
        float x = Mathf.Cos(angle) * (radius + armWidth);
        float y = Mathf.Sin(angle) * (radius + armWidth) * axisRatio;
        float radians = axisRotation * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        return orbitCenter + new Vector2(x * cos - y * sin, x * sin + y * cos);
    }

    private static void AddQuad(VertexHelper vh, Vector2 center, float size, Color color)
    {
        int start = vh.currentVertCount;
        float half = size * 0.5f;
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = new Vector3(center.x - half, center.y - half);
        vertex.uv0 = new Vector2(0f, 0f);
        vh.AddVert(vertex);
        vertex.position = new Vector3(center.x - half, center.y + half);
        vertex.uv0 = new Vector2(0f, 1f);
        vh.AddVert(vertex);
        vertex.position = new Vector3(center.x + half, center.y + half);
        vertex.uv0 = new Vector2(1f, 1f);
        vh.AddVert(vertex);
        vertex.position = new Vector3(center.x + half, center.y - half);
        vertex.uv0 = new Vector2(1f, 0f);
        vh.AddVert(vertex);

        vh.AddTriangle(start, start + 1, start + 2);
        vh.AddTriangle(start, start + 2, start + 3);
    }

    private static float Hash01(int value)
    {
        uint x = (uint)value;
        x ^= x >> 16;
        x *= 0x7feb352d;
        x ^= x >> 15;
        x *= 0x846ca68b;
        x ^= x >> 16;
        return (x & 0x00ffffff) / 16777215f;
    }
}

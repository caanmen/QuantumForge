using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public sealed class MachineCubePerspectiveFaceGraphic : MaskableGraphic
{
    public enum CubePlane
    {
        Front,
        Right,
        Left
    }

    private const int GridSubdivisions = 10;

    [SerializeField] private Texture sourceTexture;
    [SerializeField] private CubePlane plane = CubePlane.Front;
    [SerializeField] private float angleDegrees;
    [SerializeField] private float cameraDistance = 3.4f;
    [SerializeField] private float framingScale = 1f;
    [SerializeField] private Rect uvRect = new Rect(0f, 0f, 1f, 1f);

    public override Texture mainTexture => sourceTexture != null
        ? sourceTexture
        : s_WhiteTexture;

    public void SetFace(Texture texture, CubePlane cubePlane)
    {
        sourceTexture = texture;
        plane = cubePlane;
        raycastTarget = false;
        SetMaterialDirty();
        SetVerticesDirty();
    }

    public void SetPose(float angle, float distance, float scale)
    {
        angleDegrees = angle;
        cameraDistance = Mathf.Max(1.25f, distance);
        framingScale = Mathf.Clamp(scale, 0.7f, 1f);
        SetVerticesDirty();
    }

    public void SetUvRect(Rect sourceRect)
    {
        uvRect = sourceRect;
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        Rect rect = GetPixelAdjustedRect();
        float radians = angleDegrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
        Color32 tint = EvaluateLighting(sin, cos);

        int rowLength = GridSubdivisions + 1;
        for (int y = 0; y <= GridSubdivisions; y++)
        {
            float v = y / (float)GridSubdivisions;
            for (int x = 0; x <= GridSubdivisions; x++)
            {
                float u = x / (float)GridSubdivisions;
                Vector3 point = EvaluateCubePoint(u, v);
                float rotatedX = point.x * cos + point.z * sin;
                float rotatedZ = -point.x * sin + point.z * cos;
                float perspective = (cameraDistance - 0.5f) /
                    Mathf.Max(0.2f, cameraDistance - rotatedZ);

                UIVertex vertex = UIVertex.simpleVert;
                vertex.position = new Vector3(
                    rect.center.x + rotatedX * perspective * rect.width * framingScale,
                    rect.center.y + point.y * perspective * rect.height * framingScale,
                    0f);
                vertex.uv0 = new Vector2(
                    Mathf.Lerp(uvRect.xMin, uvRect.xMax, u),
                    Mathf.Lerp(uvRect.yMin, uvRect.yMax, v));
                vertex.color = tint;
                vh.AddVert(vertex);
            }
        }

        for (int y = 0; y < GridSubdivisions; y++)
        {
            for (int x = 0; x < GridSubdivisions; x++)
            {
                int bottomLeft = y * rowLength + x;
                int bottomRight = bottomLeft + 1;
                int topLeft = bottomLeft + rowLength;
                int topRight = topLeft + 1;
                vh.AddTriangle(bottomLeft, topLeft, topRight);
                vh.AddTriangle(bottomLeft, topRight, bottomRight);
            }
        }
    }

    private Vector3 EvaluateCubePoint(float u, float v)
    {
        float y = Mathf.Lerp(-0.5f, 0.5f, v);
        return plane switch
        {
            CubePlane.Right => new Vector3(0.5f, y, Mathf.Lerp(0.5f, -0.5f, u)),
            CubePlane.Left => new Vector3(-0.5f, y, Mathf.Lerp(-0.5f, 0.5f, u)),
            _ => new Vector3(Mathf.Lerp(-0.5f, 0.5f, u), y, 0.5f)
        };
    }

    private Color32 EvaluateLighting(float sin, float cos)
    {
        Vector3 normal = plane switch
        {
            CubePlane.Right => Vector3.right,
            CubePlane.Left => Vector3.left,
            _ => Vector3.forward
        };
        float facingCamera = Mathf.Clamp01(-normal.x * sin + normal.z * cos);
        float brightness = Mathf.Lerp(0.48f, 1f, Mathf.Sqrt(facingCamera));
        return new Color(color.r * brightness, color.g * brightness,
            color.b * brightness, color.a);
    }
}

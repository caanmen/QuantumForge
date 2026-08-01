using System;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class MachineCube3DVisualStateController : MonoBehaviour
{
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    [SerializeField] private MachineCube3DPrototypeController prototypeController;
    [SerializeField, Min(0.05f)] private float refreshInterval = 0.20f;
    [SerializeField] private Color damagedPlateColor =
        new Color(0.48f, 0.50f, 0.52f, 1f);
    [SerializeField] private Color repairedPlateColor =
        new Color(0.90f, 0.94f, 0.96f, 1f);
    [SerializeField] private Color warningColor =
        new Color(1.00f, 0.20f, 0.025f, 1f);
    [SerializeField] private Color repairColor =
        new Color(0.16f, 1.00f, 0.64f, 1f);

    private MachineCube3DNode[] _nodes = Array.Empty<MachineCube3DNode>();
    private FaceVisualState[] _faces = Array.Empty<FaceVisualState>();
    private MaterialPropertyBlock _propertyBlock;
    private float _nextRefreshTime;

    private void Awake()
    {
        if (prototypeController == null)
            prototypeController = GetComponentInParent<MachineCube3DPrototypeController>();
        if (prototypeController == null)
            prototypeController = GetComponentInChildren<MachineCube3DPrototypeController>(true);
        RebuildCache();
    }

    private void OnEnable()
    {
        _nextRefreshTime = 0f;
    }

    private void Update()
    {
        if (Time.unscaledTime < _nextRefreshTime)
            return;
        _nextRefreshTime = Time.unscaledTime + refreshInterval;
        RefreshNow();
    }

    /// <summary>
    /// Rescans the generated physical hierarchy. Call after rebuilding its art.
    /// </summary>
    public void RebuildCache()
    {
        _nodes = GetComponentsInChildren<MachineCube3DNode>(true);
        for (int i = 0; i < _nodes.Length; i++)
            _nodes[i]?.InvalidateVisualReferences();

        Transform face1 = FindDeepChild(transform, "PhysicalFace_1");
        Transform face2 = FindDeepChild(transform, "PhysicalFace_2");
        _faces = new[]
        {
            BuildFaceState(face1, MachineZoneType.Room1Link),
            BuildFaceState(face2, MachineZoneType.FusionSector)
        };
        _propertyBlock ??= new MaterialPropertyBlock();
        RefreshNow(true);
    }

    /// <summary>
    /// Applies repaired-node and per-zone progress. Rendering is requested only
    /// if a renderer/property/active visual state changed.
    /// </summary>
    public bool RefreshNow(bool force = false)
    {
        MachineManager manager = MachineManager.I;
        if (manager == null)
            return false;

        bool changed = false;
        for (int i = 0; i < _nodes.Length; i++)
        {
            if (_nodes[i] != null)
                changed |= _nodes[i].RefreshVisualState(manager, force);
        }

        for (int i = 0; i < _faces.Length; i++)
            changed |= RefreshFace(_faces[i], manager, force);

        if (changed)
            prototypeController?.RenderNow();
        return changed;
    }

    private FaceVisualState BuildFaceState(Transform face, MachineZoneType zone)
    {
        if (face == null)
            return new FaceVisualState(zone);
        return new FaceVisualState(zone)
        {
            plate = FindDeepChild(face, "TexturedMetalPlate")?.GetComponent<Renderer>(),
            damageDetails = FindDeepChild(face, "FaceDamageDetails"),
            warningLights = FindDeepChild(face, "FaceWarningLights"),
            repairLights = FindDeepChild(face, "FaceRepairLights")
        };
    }

    private bool RefreshFace(FaceVisualState face, MachineManager manager, bool force)
    {
        if (face == null)
            return false;
        float progress = Mathf.Clamp01((float)manager.GetZoneRepairProgress01(face.zone));
        int damageStep = GetActiveStepCount(face.damageDetails, 1f - progress);
        int warningStep = GetActiveStepCount(face.warningLights, 1f - progress);
        int repairStep = GetActiveStepCount(face.repairLights, progress);
        if (!force && face.hasState && damageStep == face.damageStep &&
            warningStep == face.warningStep && repairStep == face.repairStep &&
            Mathf.Abs(progress - face.progress) < 0.0001f)
        {
            return false;
        }

        face.hasState = true;
        face.damageStep = damageStep;
        face.warningStep = warningStep;
        face.repairStep = repairStep;
        face.progress = progress;
        SetSequentialChildren(face.damageDetails, damageStep, false);
        SetSequentialChildren(face.warningLights, warningStep, false);
        SetSequentialChildren(face.repairLights, repairStep, true);

        Color plateColor = Color.Lerp(damagedPlateColor, repairedPlateColor, progress);
        SetRendererColor(face.plate, plateColor, false);
        SetGroupColor(face.warningLights, warningColor * 2.5f, true);
        SetGroupColor(face.repairLights, repairColor * 2.2f, true);
        return true;
    }

    private static int GetActiveStepCount(Transform group, float normalized)
    {
        if (group == null || group.childCount == 0)
            return 0;
        if (normalized <= 0f)
            return 0;
        return Mathf.Clamp(Mathf.CeilToInt(normalized * group.childCount),
            1, group.childCount);
    }

    private static void SetSequentialChildren(Transform group, int activeCount,
        bool fromStart)
    {
        if (group == null)
            return;
        int count = group.childCount;
        for (int i = 0; i < count; i++)
        {
            bool active = fromStart ? i < activeCount : i >= count - activeCount;
            GameObject child = group.GetChild(i).gameObject;
            if (child.activeSelf != active)
                child.SetActive(active);
        }
    }

    private void SetGroupColor(Transform group, Color color, bool emissive)
    {
        if (group == null)
            return;
        Renderer[] renderers = group.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
            SetRendererColor(renderers[i], color, emissive);
    }

    private void SetRendererColor(Renderer target, Color color, bool emissive)
    {
        if (target == null)
            return;
        _propertyBlock ??= new MaterialPropertyBlock();
        target.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor(BaseColorId, color);
        _propertyBlock.SetColor(ColorId, color);
        if (emissive)
            _propertyBlock.SetColor(EmissionColorId, color);
        target.SetPropertyBlock(_propertyBlock);
        _propertyBlock.Clear();
    }

    private static Transform FindDeepChild(Transform root, string childName)
    {
        if (root == null)
            return null;
        if (root.name == childName)
            return root;
        for (int i = 0; i < root.childCount; i++)
        {
            Transform result = FindDeepChild(root.GetChild(i), childName);
            if (result != null)
                return result;
        }
        return null;
    }

    [Serializable]
    private sealed class FaceVisualState
    {
        public readonly MachineZoneType zone;
        public Renderer plate;
        public Transform damageDetails;
        public Transform warningLights;
        public Transform repairLights;
        public bool hasState;
        public int damageStep = -1;
        public int warningStep = -1;
        public int repairStep = -1;
        public float progress = -1f;

        public FaceVisualState(MachineZoneType zone)
        {
            this.zone = zone;
        }
    }
}

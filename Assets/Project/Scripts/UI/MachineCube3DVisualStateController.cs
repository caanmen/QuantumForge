using System;
using System.Collections.Generic;
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
        new Color(0.70f, 0.68f, 0.64f, 1f);
    [SerializeField] private Color repairedPlateColor =
        new Color(0.84f, 0.83f, 0.80f, 1f);
    [SerializeField] private Color warningColor =
        new Color(1.00f, 0.025f, 0.055f, 1f);
    [SerializeField] private Color repairColor =
        new Color(0.025f, 0.30f, 0.40f, 1f);
    [Header("Selection")]
    [SerializeField, Min(0.6f)] private float selectionPulseDuration = 1.25f;

    private MachineCube3DNode[] _nodes = Array.Empty<MachineCube3DNode>();
    private FaceVisualState[] _faces = Array.Empty<FaceVisualState>();
    private MaterialPropertyBlock _propertyBlock;
    private float _nextRefreshTime;
    private float _nextSelectionRenderTime;
    private float _nextDamageFlickerRenderTime;
    private string _selectedNodeId = "";

    public string SelectedNodeId => _selectedNodeId;

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
        _nextSelectionRenderTime = 0f;
        _nextDamageFlickerRenderTime = 0f;
    }

    private void Update()
    {
        bool stateRendered = false;
        if (Time.unscaledTime >= _nextRefreshTime)
        {
            _nextRefreshTime = Time.unscaledTime + refreshInterval;
            stateRendered = RefreshNow();
        }

        bool damageChanged = false;
        if (prototypeController != null && prototypeController.IsVisible &&
            Time.unscaledTime >= _nextDamageFlickerRenderTime)
        {
            float damageRefreshRate = Mathf.Max(2f,
                MachineCube3DQuality.Profile.idleRefreshRate);
            _nextDamageFlickerRenderTime = Time.unscaledTime +
                1f / damageRefreshRate;
            for (int i = 0; i < _nodes.Length; i++)
            {
                if (_nodes[i] != null)
                    damageChanged |= _nodes[i].ApplyDamageFlicker(
                        CalculateDamageFlicker(i));
            }
        }

        if (string.IsNullOrEmpty(_selectedNodeId) ||
            prototypeController == null || !prototypeController.IsVisible ||
            Time.unscaledTime < _nextSelectionRenderTime)
        {
            if (damageChanged && !stateRendered)
                prototypeController?.RenderNow();
            return;
        }

        float refreshRate = GetSelectionRefreshRate();
        _nextSelectionRenderTime = Time.unscaledTime + 1f / refreshRate;
        float cycle = Mathf.Repeat(Time.unscaledTime /
            Mathf.Max(0.6f, selectionPulseDuration), 1f);
        float pulse = 0.5f - 0.5f * Mathf.Cos(cycle * Mathf.PI * 2f);
        bool selectionChanged = false;
        for (int i = 0; i < _nodes.Length; i++)
        {
            if (_nodes[i] != null && _nodes[i].IsSelected)
                selectionChanged |= _nodes[i].ApplySelectionPulse(pulse);
        }
        if ((selectionChanged || damageChanged) && !stateRendered)
            prototypeController.RenderNow();
    }

    private static float CalculateDamageFlicker(int nodeIndex)
    {
        float phase = Mathf.Repeat(Time.unscaledTime * 0.38f +
            nodeIndex * 0.173f, 1f);
        // Short, unsynchronised drop-outs sell an unstable circuit without
        // turning the warning light into a rapid accessibility-hostile strobe.
        if (phase < 0.045f)
            return 0.025f;
        if (phase > 0.135f && phase < 0.205f)
            return 0.09f;
        // One isolated surge per cycle, followed by a low irregular tremor.
        if (phase > 0.50f && phase < 0.545f)
            return 1f;
        float tremor = 0.62f +
            0.16f * Mathf.Sin((phase * 11f + nodeIndex * 0.91f) *
                Mathf.PI * 2f) +
            0.08f * Mathf.Sin((phase * 23f + nodeIndex * 0.37f) *
                Mathf.PI * 2f);
        return Mathf.Clamp01(tremor);
    }

    /// <summary>
    /// Rescans the generated physical hierarchy. Call after rebuilding its art.
    /// </summary>
    public void RebuildCache()
    {
        MachineCube3DFace[] modularFaces =
            GetComponentsInChildren<MachineCube3DFace>(true);
        Array.Sort(modularFaces, (a, b) => a.FaceIndex.CompareTo(b.FaceIndex));
        for (int i = 0; i < modularFaces.Length; i++)
            modularFaces[i]?.RebuildCache();

        _nodes = GetComponentsInChildren<MachineCube3DNode>(true);
        for (int i = 0; i < _nodes.Length; i++)
            _nodes[i]?.InvalidateVisualReferences();

        if (modularFaces.Length > 0)
        {
            _faces = new FaceVisualState[modularFaces.Length];
            for (int i = 0; i < modularFaces.Length; i++)
                _faces[i] = BuildFaceState(modularFaces[i]);
        }
        else
        {
            Transform face1 = FindDeepChild(transform, "PhysicalFace_1");
            Transform face2 = FindDeepChild(transform, "PhysicalFace_2");
            _faces = new[]
            {
                BuildFaceState(face1, MachineZoneType.Room1Link),
                BuildFaceState(face2, MachineZoneType.FusionSector)
            };
        }
        _propertyBlock ??= new MaterialPropertyBlock();
        ApplySelectedNode(true);
        RefreshNow(true);
    }

    /// <summary>
    /// Selects a physical node by its stable game-data id. This is safe to call
    /// from clicks, keyboard navigation or a restored MachinePanel selection.
    /// </summary>
    public bool SetSelectedNode(string nodeId, bool force = false)
    {
        string normalized = string.IsNullOrWhiteSpace(nodeId)
            ? ""
            : nodeId.Trim();
        if (!force && string.Equals(_selectedNodeId, normalized,
                StringComparison.Ordinal))
        {
            return false;
        }

        _selectedNodeId = normalized;
        bool changed = ApplySelectedNode(true);
        _nextSelectionRenderTime = 0f;
        if (changed)
            prototypeController?.RenderNow();
        return changed;
    }

    public bool ClearSelectedNode() => SetSelectedNode("");

    /// <summary>
    /// Applies repaired-node and per-zone progress. Rendering is requested only
    /// if a renderer/property/active visual state changed.
    /// </summary>
    public bool RefreshNow(bool force = false)
    {
        MachineManager manager = MachineManager.I;
        if (manager == null)
            return false;

        bool bindingsChanged = RefreshNodeBindings(manager);
        bool changed = bindingsChanged;
        if (bindingsChanged)
            changed |= ApplySelectedNode(true);
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

    private bool RefreshNodeBindings(MachineManager manager)
    {
        var nodesByZone = new Dictionary<MachineZoneType, List<MachineNodeDef>>();
        bool changed = false;
        for (int i = 0; i < _nodes.Length; i++)
        {
            MachineCube3DNode node = _nodes[i];
            if (node == null)
                continue;
            MachineZoneType zone = (MachineZoneType)(
                Mathf.Clamp(node.FaceIndex, 0, 3) + 1);
            if (!nodesByZone.TryGetValue(zone, out List<MachineNodeDef> source))
            {
                source = manager.GetDisplayNodesByZone(zone);
                nodesByZone.Add(zone, source);
            }
            MachineNodeDef definition = source != null && node.SlotIndex >= 0 &&
                node.SlotIndex < source.Count
                ? source[node.SlotIndex]
                : null;
            changed |= node.BindDisplayNode(definition);
        }
        return changed;
    }

    private bool ApplySelectedNode(bool force)
    {
        bool changed = false;
        for (int i = 0; i < _nodes.Length; i++)
        {
            MachineCube3DNode node = _nodes[i];
            if (node == null)
                continue;
            bool selected = !string.IsNullOrEmpty(_selectedNodeId) &&
                string.Equals(node.NodeId, _selectedNodeId,
                    StringComparison.Ordinal);
            changed |= node.SetSelected(selected, force);
        }
        return changed;
    }

    private static float GetSelectionRefreshRate()
    {
        return MachineCube3DQuality.Current switch
        {
            MachineCube3DQualityLevel.Low => 8f,
            MachineCube3DQualityLevel.Balanced => 15f,
            _ => 24f
        };
    }

    private FaceVisualState BuildFaceState(Transform face, MachineZoneType zone)
    {
        if (face == null)
            return new FaceVisualState(zone);
        return new FaceVisualState(zone)
        {
            plate = FindDeepChild(face, "TexturedMetalPlate")?.GetComponent<Renderer>(),
            damageDetails = FindDeepChild(face, "FaceDamageDetails"),
            repairPatches = FindDeepChild(face, "FaceRepairPatches"),
            warningLights = FindDeepChild(face, "FaceWarningLights"),
            repairLights = FindDeepChild(face, "FaceRepairLights"),
            blenderDamage = FindDeepChild(face, "DAMAGE_STATES"),
            blenderRepairs = FindDeepChild(face, "REPAIR_STATES")
        };
    }

    private static FaceVisualState BuildFaceState(MachineCube3DFace face)
    {
        if (face == null)
            return null;
        return new FaceVisualState(face.Zone)
        {
            plate = face.SurfaceRenderer,
            damageDetails = face.DamageDetails,
            repairPatches = face.RepairPatches,
            warningLights = face.WarningLights,
            repairLights = face.RepairLights,
            blenderDamage = FindDeepChild(face.transform, "DAMAGE_STATES"),
            blenderRepairs = FindDeepChild(face.transform, "REPAIR_STATES")
        };
    }

    private bool RefreshFace(FaceVisualState face, MachineManager manager, bool force)
    {
        if (face == null)
            return false;
        float progress = Mathf.Clamp01((float)manager.GetZoneRepairProgress01(face.zone));
        int patchStep = GetRepairStageCount(face.repairPatches, progress);
        int damageStep = face.repairPatches != null
            ? Mathf.Max(0, face.damageDetails.childCount - patchStep)
            : GetActiveStepCount(face.damageDetails, 1f - progress);
        int warningStep = GetActiveStepCount(face.warningLights, 1f - progress);
        int repairStep = GetActiveStepCount(face.repairLights, progress);
        if (!force && face.hasState && damageStep == face.damageStep &&
            patchStep == face.patchStep && warningStep == face.warningStep &&
            repairStep == face.repairStep &&
            Mathf.Abs(progress - face.progress) < 0.0001f)
        {
            return false;
        }

        face.hasState = true;
        face.damageStep = damageStep;
        face.patchStep = patchStep;
        face.warningStep = warningStep;
        face.repairStep = repairStep;
        face.progress = progress;
        SetSequentialChildren(face.damageDetails, damageStep, false);
        SetSequentialChildren(face.repairPatches, patchStep, true);
        SetSequentialChildren(face.warningLights, warningStep, false);
        SetSequentialChildren(face.repairLights, repairStep, true);
        if (face.blenderDamage != null && face.blenderRepairs != null)
        {
            int blenderPatchStep = Mathf.Clamp(patchStep, 0,
                face.blenderRepairs.childCount);
            int blenderDamageStep = Mathf.Max(0,
                face.blenderDamage.childCount - blenderPatchStep);
            SetSequentialChildren(face.blenderDamage, blenderDamageStep, false);
            SetSequentialChildren(face.blenderRepairs, blenderPatchStep, true);
        }

        bool pbrPreview = face.plate != null &&
            face.plate.sharedMaterial != null &&
            face.plate.sharedMaterial.name.StartsWith("M3D_PBR_",
                StringComparison.Ordinal);
        Color plateColor = pbrPreview
            ? Color.Lerp(new Color(0.35f, 0.33f, 0.30f, 1f),
                new Color(0.39f, 0.37f, 0.34f, 1f), progress)
            : Color.Lerp(damagedPlateColor, repairedPlateColor, progress);
        SetRendererColor(face.plate, plateColor, false, 0);
        SetGroupColor(face.warningLights, warningColor * 1.35f, true);
        SetGroupColor(face.repairLights, repairColor * 0.90f, true);
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

    private static int GetRepairStageCount(Transform group, float progress)
    {
        if (group == null || group.childCount == 0)
            return 0;
        if (group.childCount == 3)
        {
            if (progress >= 0.90f)
                return 3;
            if (progress >= 0.62f)
                return 2;
            if (progress >= 0.28f)
                return 1;
            return 0;
        }
        return Mathf.Clamp(Mathf.FloorToInt(progress * group.childCount),
            0, group.childCount);
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

    private void SetRendererColor(Renderer target, Color color, bool emissive,
        int materialIndex = -1)
    {
        if (target == null)
            return;
        _propertyBlock ??= new MaterialPropertyBlock();
        if (materialIndex >= 0)
            target.GetPropertyBlock(_propertyBlock, materialIndex);
        else
            target.GetPropertyBlock(_propertyBlock);
        Color baseColor = emissive ? color * 0.16f : color;
        baseColor.a = color.a;
        _propertyBlock.SetColor(BaseColorId, baseColor);
        _propertyBlock.SetColor(ColorId, baseColor);
        if (emissive)
            _propertyBlock.SetColor(EmissionColorId, color);
        if (materialIndex >= 0)
            target.SetPropertyBlock(_propertyBlock, materialIndex);
        else
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
        public Transform repairPatches;
        public Transform warningLights;
        public Transform repairLights;
        public Transform blenderDamage;
        public Transform blenderRepairs;
        public bool hasState;
        public int damageStep = -1;
        public int patchStep = -1;
        public int warningStep = -1;
        public int repairStep = -1;
        public float progress = -1f;

        public FaceVisualState(MachineZoneType zone)
        {
            this.zone = zone;
        }
    }
}

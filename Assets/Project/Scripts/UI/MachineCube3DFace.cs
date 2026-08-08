using System;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class MachineCube3DFace : MonoBehaviour
{
    [SerializeField, Range(0, 3)] private int faceIndex;
    [SerializeField] private MachineZoneType zone = MachineZoneType.Room1Link;
    [SerializeField] private Transform structureRoot;
    [SerializeField] private Transform nodeRoot;
    [SerializeField] private Transform stateRoot;

    private MachineCube3DModule[] _modules = Array.Empty<MachineCube3DModule>();
    private MachineCube3DNode[] _nodes = Array.Empty<MachineCube3DNode>();
    private Renderer[] _renderers = Array.Empty<Renderer>();
    private Collider[] _nodeColliders = Array.Empty<Collider>();
    private bool _renderVisibilityInitialized;
    private bool _renderingVisible = true;

    public int FaceIndex => faceIndex;
    public MachineZoneType Zone => zone;
    public Transform StructureRoot => structureRoot;
    public Transform NodeRoot => nodeRoot;
    public Transform StateRoot => stateRoot;
    public MachineCube3DNode[] Nodes => _nodes;
    public bool IsRenderingVisible => _renderingVisible;
    public Renderer SurfaceRenderer { get; private set; }
    public Transform DamageDetails { get; private set; }
    public Transform RepairPatches { get; private set; }
    public Transform WarningLights { get; private set; }
    public Transform RepairLights { get; private set; }

    public void Configure(int index, MachineZoneType machineZone,
        Transform structure, Transform nodes, Transform state)
    {
        faceIndex = Mathf.Clamp(index, 0, 3);
        zone = machineZone;
        structureRoot = structure;
        nodeRoot = nodes;
        stateRoot = state;
        RebuildCache();
    }

    public void RebuildCache()
    {
        _modules = GetComponentsInChildren<MachineCube3DModule>(true);
        Transform authoredAnchors = FindDeepChild(transform, "NODE_ANCHORS");
        Transform activeNodeRoot = authoredAnchors != null
            ? authoredAnchors
            : (nodeRoot != null ? nodeRoot : transform);
        _nodes = activeNodeRoot
            .GetComponentsInChildren<MachineCube3DNode>(true);
        Transform authoredVisual = transform.Find("BlenderVisualRoot");
        _renderers = authoredVisual != null
            ? authoredVisual.GetComponentsInChildren<Renderer>(true)
            : GetComponentsInChildren<Renderer>(true);
        _nodeColliders = activeNodeRoot
            .GetComponentsInChildren<Collider>(true);
        Transform structure = structureRoot != null ? structureRoot : transform;
        Transform surface = FindDeepChild(structure, "PhysicalMetalBackplate") ??
            FindDeepChild(structure, "TexturedMetalPlate");
        SurfaceRenderer = surface?.GetComponent<Renderer>();
        Transform state = stateRoot != null ? stateRoot : transform;
        DamageDetails = FindDeepChild(state, "FaceDamageDetails");
        RepairPatches = FindDeepChild(state, "FaceRepairPatches");
        WarningLights = FindDeepChild(state, "FaceWarningLights");
        RepairLights = FindDeepChild(state, "FaceRepairLights");
    }

    public bool ApplyQuality(MachineCube3DQualityLevel quality)
    {
        bool changed = false;
        for (int i = 0; i < _modules.Length; i++)
            if (_modules[i] != null)
                changed |= _modules[i].ApplyQuality(quality);
        return changed;
    }

    public bool SetRenderVisibility(bool visible)
    {
        bool changed = !_renderVisibilityInitialized || _renderingVisible != visible;
        _renderVisibilityInitialized = true;
        _renderingVisible = visible;
        if (changed)
        {
            for (int i = 0; i < _renderers.Length; i++)
                if (_renderers[i] != null)
                    _renderers[i].enabled = visible;
        }
        for (int i = 0; i < _nodeColliders.Length; i++)
        {
            Collider nodeCollider = _nodeColliders[i];
            if (nodeCollider != null)
                nodeCollider.enabled = visible && nodeCollider.gameObject.activeSelf;
        }
        return changed;
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
}

using UnityEngine;

[DisallowMultipleComponent]
public sealed class MachineCube3DNode : MonoBehaviour
{
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    [SerializeField] private string nodeId;
    [SerializeField] private int faceIndex;
    [Header("Visual state")]
    [SerializeField] private Color damagedSurfaceColor =
        new Color(0.36f, 0.38f, 0.40f, 1f);
    [SerializeField] private Color repairedSurfaceColor =
        new Color(0.82f, 0.88f, 0.90f, 1f);
    [SerializeField] private Color warningColor =
        new Color(1.00f, 0.24f, 0.035f, 1f);
    [SerializeField] private Color repairColor =
        new Color(0.20f, 1.00f, 0.68f, 1f);

    private Renderer _referenceSurface;
    private GameObject _operationalEmitter;
    private GameObject _damageDetails;
    private GameObject _damageEmitter;
    private GameObject _repairEmitter;
    private MaterialPropertyBlock _propertyBlock;
    private bool _referencesCached;
    private bool _lastRepaired;
    private bool _lastDefinitionDamaged;
    private bool _hasVisualState;

    public string NodeId => nodeId;
    public int FaceIndex => faceIndex;

    /// <summary>
    /// Refreshes this physical node without cloning any renderer material.
    /// Returns true only when the visible state actually changed.
    /// </summary>
    public bool RefreshVisualState(MachineManager manager, bool force = false)
    {
        if (manager == null || string.IsNullOrWhiteSpace(nodeId))
            return false;

        EnsureVisualReferences();
        bool repaired = manager.IsNodeRepaired(nodeId);
        MachineNodeDef definition = manager.GetDef(nodeId);
        bool definitionDamaged = definition != null && definition.damaged;
        if (!force && _hasVisualState && repaired == _lastRepaired &&
            definitionDamaged == _lastDefinitionDamaged)
        {
            return false;
        }

        _lastRepaired = repaired;
        _lastDefinitionDamaged = definitionDamaged;
        _hasVisualState = true;

        SetActiveIfDifferent(_operationalEmitter, true);
        SetActiveIfDifferent(_damageDetails, !repaired);
        SetActiveIfDifferent(_damageEmitter, !repaired);
        SetActiveIfDifferent(_repairEmitter, repaired);

        SetRendererColor(_referenceSurface,
            repaired ? repairedSurfaceColor : damagedSurfaceColor, false);

        // The node remains operational while damaged: its normal face-color
        // emitter stays on. Damage receives a separate warning channel.
        Color operational = faceIndex == 1
            ? new Color(0.66f, 0.18f, 1.00f, 1f)
            : new Color(0.00f, 0.72f, 1.00f, 1f);
        SetGroupColor(_operationalEmitter, operational,
            repaired ? 2.25f : 1.35f);
        SetGroupColor(_damageEmitter, warningColor,
            definitionDamaged ? 3.0f : 1.9f);
        SetGroupColor(_damageDetails, warningColor,
            definitionDamaged ? 0.75f : 0.45f);
        SetGroupColor(_repairEmitter, repairColor, 2.6f);
        return true;
    }

    public void InvalidateVisualReferences()
    {
        _referencesCached = false;
        _hasVisualState = false;
    }

    private void EnsureVisualReferences()
    {
        if (_referencesCached)
            return;

        _referenceSurface = FindDeepChild(transform, "ReferenceSurface")
            ?.GetComponent<Renderer>();
        _operationalEmitter = FindDeepChild(transform, "OperationalEmitter")
            ?.gameObject;
        _damageDetails = FindDeepChild(transform, "DamageDetails")?.gameObject;
        _damageEmitter = FindDeepChild(transform, "DamageEmitter")?.gameObject;
        _repairEmitter = FindDeepChild(transform, "RepairEmitter")?.gameObject;
        _propertyBlock ??= new MaterialPropertyBlock();
        _referencesCached = true;
    }

    private void SetGroupColor(GameObject group, Color color, float emission)
    {
        if (group == null)
            return;
        Renderer[] renderers = group.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
            SetRendererColor(renderers[i], color * emission, true);
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

    private static void SetActiveIfDifferent(GameObject target, bool active)
    {
        if (target != null && target.activeSelf != active)
            target.SetActive(active);
    }

    private static Transform FindDeepChild(Transform root, string childName)
    {
        if (root == null)
            return null;
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child.name == childName)
                return child;
            Transform nested = FindDeepChild(child, childName);
            if (nested != null)
                return nested;
        }
        return null;
    }
}

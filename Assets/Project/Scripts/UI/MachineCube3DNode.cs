using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class MachineCube3DNode : MonoBehaviour
{
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    [SerializeField] private string nodeId;
    [SerializeField] private int faceIndex;
    [SerializeField, Min(0)] private int slotIndex;
    [Header("Visual state")]
    [SerializeField] private Color damagedSurfaceColor =
        new Color(0.22f, 0.23f, 0.24f, 1f);
    [SerializeField] private Color repairedSurfaceColor =
        new Color(0.56f, 0.59f, 0.60f, 1f);
    [SerializeField] private Color warningColor =
        new Color(1.00f, 0.025f, 0.055f, 1f);
    [SerializeField] private Color repairColor =
        new Color(0.20f, 1.00f, 0.68f, 1f);
    [Header("Selection feedback")]
    [SerializeField, Min(0f)] private float selectedEmission = 2.20f;

    private Renderer _referenceSurface;
    private GameObject _operationalEmitter;
    private GameObject _damageDetails;
    private GameObject _damageEmitter;
    private GameObject _repairEmitter;
    private Transform _selectionFeedback;
    private GameObject _authoredDamagedState;
    private GameObject _authoredRepairedState;
    private Renderer[] _selectionRenderers = System.Array.Empty<Renderer>();
    private RendererMaterialSlot[] _damagedAccentSlots =
        System.Array.Empty<RendererMaterialSlot>();
    private Light[] _damageLights = System.Array.Empty<Light>();
    private GameObject[] _damageRemovedParts = System.Array.Empty<GameObject>();
    private MaterialPropertyBlock _propertyBlock;
    private bool _referencesCached;
    private bool _lastRepaired;
    private bool _lastDefinitionDamaged;
    private bool _hasVisualState;
    private bool _selected;
    private float _lastSelectionPulse = -1f;
    private bool _visiblyDamaged;
    private float _lastDamageFlicker = -1f;
    private Vector3 _restingLocalPosition;
    private bool _transformReferenceCached;

    public string NodeId => nodeId;
    public int FaceIndex => faceIndex;
    public int SlotIndex => slotIndex;
    public bool IsSelected => _selected;

    public bool BindDisplayNode(MachineNodeDef definition)
    {
        string nextId = definition != null ? definition.id : "";
        bool visible = definition != null;
        bool changed = !string.Equals(nodeId, nextId,
            System.StringComparison.Ordinal) || gameObject.activeSelf != visible;
        if (!changed)
            return false;

        if (!visible && _selected)
            SetSelected(false, true);
        nodeId = nextId;
        _hasVisualState = false;
        Collider hitCollider = GetComponent<Collider>();
        if (hitCollider != null)
            hitCollider.enabled = visible;
        if (gameObject.activeSelf != visible)
            gameObject.SetActive(visible);
        return true;
    }

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

        bool visiblyDamaged = definitionDamaged && !repaired;
        bool visiblyRepaired = definitionDamaged && repaired;
        _visiblyDamaged = visiblyDamaged;
        _lastDamageFlicker = -1f;

        bool authoredStates = _authoredDamagedState != null ||
            _authoredRepairedState != null;
        if (authoredStates)
        {
            SetActiveIfDifferent(_authoredDamagedState, visiblyDamaged);
            SetActiveIfDifferent(_authoredRepairedState, !visiblyDamaged);
        }
        else
        {
            // Procedural fallback for faces without an approved authored
            // Blender template.
            SetActiveIfDifferent(_operationalEmitter, !visiblyDamaged);
            SetActiveIfDifferent(_damageDetails, visiblyDamaged);
            SetActiveIfDifferent(_damageEmitter, visiblyDamaged);
            SetActiveIfDifferent(_repairEmitter, visiblyRepaired);
        }
        for (int i = 0; i < _damageRemovedParts.Length; i++)
            SetActiveIfDifferent(_damageRemovedParts[i], !visiblyDamaged);

        SetRendererColor(_referenceSurface,
            repaired ? repairedSurfaceColor : damagedSurfaceColor, false);

        // Healthy and repaired lights keep the identity colour of their face.
        // Damage uses a dedicated warning channel so it remains immediately
        // readable even when several face palettes are visible at once.
        Color operational = GetFaceAccent();
        SetGroupColor(_operationalEmitter, operational,
            repaired ? 1.35f : 0.88f);
        SetGroupColor(_damageEmitter, warningColor, 1.35f);
        SetGroupColor(_repairEmitter, operational, 2.2f);
        if (visiblyDamaged)
            ApplyDamageFlicker(0.72f, true);
        return true;
    }

    /// <summary>
    /// Applies a low-cost irregular flicker only to the light geometry of a
    /// damaged node. Geometry, selection and the face palette are unchanged.
    /// </summary>
    public bool ApplyDamageFlicker(float normalizedFlicker, bool force = false)
    {
        if (!_visiblyDamaged)
            return false;

        EnsureVisualReferences();
        float flicker = Mathf.Clamp01(normalizedFlicker);
        if (!force && Mathf.Abs(flicker - _lastDamageFlicker) < 0.025f)
            return false;
        _lastDamageFlicker = flicker;

        Color accent = warningColor;
        float emission = Mathf.Lerp(0.20f, 2.8f, flicker);
        for (int i = 0; i < _damagedAccentSlots.Length; i++)
        {
            RendererMaterialSlot slot = _damagedAccentSlots[i];
            SetRendererSlotColors(slot.renderer, slot.materialIndex,
                accent * 0.018f, accent * emission, true);
        }
        for (int i = 0; i < _damageLights.Length; i++)
        {
            if (_damageLights[i] != null)
                _damageLights[i].intensity = Mathf.Lerp(0.10f, 1.55f, flicker);
        }
        return _damagedAccentSlots.Length > 0;
    }

    /// <summary>
    /// Enables the single integrated under-light. Selection never draws a
    /// frame, halo or outline and never moves the physical node.
    /// </summary>
    public bool SetSelected(bool selected, bool force = false)
    {
        EnsureVisualReferences();
        if (!force && _selected == selected)
            return false;

        _selected = selected;
        _lastSelectionPulse = -1f;
        SetActiveIfDifferent(_selectionFeedback?.gameObject, selected);
        if (selected)
        {
            ApplySelectionPulse(0.5f, true);
        }
        else
        {
            transform.localPosition = _restingLocalPosition;

            // Older generated prototypes do not have SelectionFeedback yet.
            // Restore their operational strip after using it as a fallback.
            Color operational = GetFaceAccent();
            SetGroupColor(_operationalEmitter, operational,
                _lastRepaired ? 1.35f : 0.88f);
        }
        return true;
    }

    /// <summary>
    /// Updates the low-cost pulse. The caller throttles this method per quality
    /// profile, so it never requires post-processing or per-frame allocations.
    /// </summary>
    public bool ApplySelectionPulse(float normalizedPulse, bool force = false)
    {
        if (!_selected)
            return false;

        EnsureVisualReferences();
        float pulse = Mathf.Clamp01(normalizedPulse);
        if (!force && Mathf.Abs(pulse - _lastSelectionPulse) < 0.005f)
            return false;
        _lastSelectionPulse = pulse;

        if (_selectionFeedback != null)
        {
            Color accent = GetFaceAccent();
            float emission = selectedEmission * Mathf.Lerp(0.72f, 1f, pulse);
            for (int i = 0; i < _selectionRenderers.Length; i++)
                SetRendererColors(_selectionRenderers[i], accent * 0.16f,
                    accent * emission, true);
        }
        else
        {
            // Graceful fallback until the setup regenerates the dedicated ring.
            SetGroupColor(_operationalEmitter, GetFaceAccent(),
                selectedEmission * Mathf.Lerp(0.72f, 1f, pulse));
        }
        return true;
    }

    public void InvalidateVisualReferences()
    {
        if (_transformReferenceCached)
            transform.localPosition = _restingLocalPosition;
        _referencesCached = false;
        _hasVisualState = false;
        _lastDamageFlicker = -1f;
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
        _authoredDamagedState = FindDeepChildStartingWith(transform,
            "DAMAGED_STATE_")?.gameObject;
        _authoredRepairedState = FindDeepChildStartingWith(transform,
            "REPAIRED_STATE_")?.gameObject;
        _selectionFeedback = FindDeepChild(transform, "SelectionFeedback") ??
            FindDeepChildStartingWith(transform, "SELECTION_FEEDBACK_");
        _selectionRenderers = _selectionFeedback != null
            ? _selectionFeedback.GetComponentsInChildren<Renderer>(true)
            : System.Array.Empty<Renderer>();
        _damagedAccentSlots = FindDamagedAccentSlots();
        Transform damageLightSource = _authoredDamagedState != null
            ? _authoredDamagedState.transform
            : _damageEmitter != null ? _damageEmitter.transform : null;
        _damageLights = damageLightSource != null
            ? damageLightSource.GetComponentsInChildren<Light>(true)
            : System.Array.Empty<Light>();
        string[] removablePartNames =
        {
            // Remove a few asymmetric pieces but retain the main silhouette.
            // The earlier set removed the top and right rails completely,
            // causing damaged sockets to read as unidentifiable debris.
            "HousingCornerTR", "HousingCornerBL", "SocketInnerRimRight",
            "CircularHousing_3", "CircularHousing_7"
        };
        var removableParts = new List<GameObject>();
        for (int i = 0; i < removablePartNames.Length; i++)
        {
            Transform part = FindDeepChild(transform, removablePartNames[i]);
            if (part != null)
                removableParts.Add(part.gameObject);
        }
        _damageRemovedParts = removableParts.ToArray();
        if (!_transformReferenceCached)
        {
            _restingLocalPosition = transform.localPosition;
            _transformReferenceCached = true;
        }
        if (_selectionFeedback != null)
            _selectionFeedback.gameObject.SetActive(_selected);
        _propertyBlock ??= new MaterialPropertyBlock();
        _referencesCached = true;
    }

    private RendererMaterialSlot[] FindDamagedAccentSlots()
    {
        Transform source = _authoredDamagedState != null
            ? _authoredDamagedState.transform
            : _damageEmitter != null ? _damageEmitter.transform : null;
        if (source == null)
            return System.Array.Empty<RendererMaterialSlot>();

        Renderer[] candidates = source.GetComponentsInChildren<Renderer>(true);
        var result = new List<RendererMaterialSlot>();
        for (int i = 0; i < candidates.Length; i++)
        {
            Material[] materials = candidates[i].sharedMaterials;
            for (int slot = 0; slot < materials.Length; slot++)
            {
                Material material = materials[slot];
                bool animated = _authoredDamagedState == null ||
                    (material != null && (material.IsKeywordEnabled("_EMISSION") ||
                    material.name.IndexOf("Cyan",
                        System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                    material.name.IndexOf("Purple",
                        System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                    material.name.IndexOf("Amber",
                        System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                    material.name.IndexOf("Warning",
                        System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                    material.name.IndexOf("DamageFracture",
                        System.StringComparison.OrdinalIgnoreCase) >= 0));
                if (animated)
                    result.Add(new RendererMaterialSlot(candidates[i], slot));
            }
        }
        return result.ToArray();
    }

    private Color GetFaceAccent()
    {
        return faceIndex switch
        {
            1 => new Color(0.48f, 0.08f, 0.78f, 1f),
            2 => new Color(0.98f, 0.22f, 0.010f, 1f),
            3 => new Color(0.02f, 0.62f, 0.50f, 1f),
            _ => new Color(0.00f, 0.48f, 0.78f, 1f)
        };
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
        SetRendererColors(target, emissive ? color * 0.16f : color,
            color, emissive);
    }

    private void SetRendererColors(Renderer target, Color baseColor,
        Color emissionColor, bool emissive)
    {
        if (target == null)
            return;
        _propertyBlock ??= new MaterialPropertyBlock();
        target.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor(BaseColorId, baseColor);
        _propertyBlock.SetColor(ColorId, baseColor);
        if (emissive)
            _propertyBlock.SetColor(EmissionColorId, emissionColor);
        target.SetPropertyBlock(_propertyBlock);
        _propertyBlock.Clear();
    }

    private void SetRendererSlotColors(Renderer target, int materialIndex,
        Color baseColor, Color emissionColor, bool emissive)
    {
        if (target == null || materialIndex < 0 ||
            materialIndex >= target.sharedMaterials.Length)
            return;
        _propertyBlock ??= new MaterialPropertyBlock();
        target.GetPropertyBlock(_propertyBlock, materialIndex);
        _propertyBlock.SetColor(BaseColorId, baseColor);
        _propertyBlock.SetColor(ColorId, baseColor);
        if (emissive)
            _propertyBlock.SetColor(EmissionColorId, emissionColor);
        target.SetPropertyBlock(_propertyBlock, materialIndex);
        _propertyBlock.Clear();
    }

    private readonly struct RendererMaterialSlot
    {
        public readonly Renderer renderer;
        public readonly int materialIndex;

        public RendererMaterialSlot(Renderer renderer, int materialIndex)
        {
            this.renderer = renderer;
            this.materialIndex = materialIndex;
        }
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

    private static Transform FindDeepChildStartingWith(Transform root,
        string prefix)
    {
        if (root == null)
            return null;
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child.name.StartsWith(prefix, System.StringComparison.Ordinal))
                return child;
            Transform nested = FindDeepChildStartingWith(child, prefix);
            if (nested != null)
                return nested;
        }
        return null;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class MachineCube3DPrototypeController : MonoBehaviour
{
    [SerializeField] private Transform cubeRoot;
    [SerializeField] private Camera prototypeCamera;
    [SerializeField] private RenderTexture targetTexture;
    [SerializeField] private RawImage display;
    [SerializeField] private MachineCube3DVisualStateController visualStateController;
    [SerializeField] private float rotationDuration = 0.72f;
    [SerializeField] private float restingYawOffsetDegrees = 14f;

    private Coroutine _rotationRoutine;
    private int _currentFaceIndex;
    private bool _visible;
    private float _nextIdleRenderTime;
    private MachineCube3DFace[] _faces = System.Array.Empty<MachineCube3DFace>();

    public bool IsRotating => _rotationRoutine != null;
    public bool IsVisible => _visible && display != null &&
        display.gameObject.activeInHierarchy && prototypeCamera != null;
    public int CurrentFaceIndex => _currentFaceIndex;
    public float CurrentYawDegrees => cubeRoot != null
        ? Mathf.DeltaAngle(0f, cubeRoot.localEulerAngles.y)
        : 0f;
    public Camera PrototypeCamera => prototypeCamera;
    public Texture OutputTexture => targetTexture;
    public int CurrentRenderSize => targetTexture != null ? targetTexture.width : 0;
    public string SelectedNodeId => visualStateController != null
        ? visualStateController.SelectedNodeId
        : "";
    public int AvailableFaceCount => _faces.Length;
    public float GetFaceRotationDegrees(int faceIndex)
    {
        int clamped = Mathf.Clamp(faceIndex, 0,
            Mathf.Max(0, AvailableFaceCount - 1));
        // Every face rests with the same right-hand three-quarter view. Using
        // opposite signs on alternating faces made the visible side flip after
        // some rotations even though the selected face was correct.
        return -90f * clamped + restingYawOffsetDegrees;
    }

    private void Awake()
    {
        RebuildFaceCache();
        if (visualStateController == null)
            visualStateController = GetComponent<MachineCube3DVisualStateController>();
        if (visualStateController == null)
            visualStateController =
                GetComponentInChildren<MachineCube3DVisualStateController>(true);
        MachineCube3DQuality.Changed += ApplyQualityProfile;
        if (prototypeCamera != null)
        {
            prototypeCamera.targetTexture = targetTexture;
            prototypeCamera.enabled = false;
        }
        if (display != null)
        {
            display.texture = targetTexture;
            display.raycastTarget = true;
            display.gameObject.SetActive(false);
        }
        ApplyQualityProfile();
    }

    private void Start()
    {
        if (_visible)
            ShowPrototype(true);
    }

    private void Update()
    {
        if (!_visible || IsRotating)
            return;
        float refreshRate = MachineCube3DQuality.Profile.idleRefreshRate;
        if (refreshRate <= 0f || Time.unscaledTime < _nextIdleRenderTime)
            return;
        _nextIdleRenderTime = Time.unscaledTime + 1f / refreshRate;
        RenderNow();
    }

    private void OnDestroy()
    {
        MachineCube3DQuality.Changed -= ApplyQualityProfile;
    }

    private void OnDisable()
    {
        if (_rotationRoutine != null)
            StopCoroutine(_rotationRoutine);
        _rotationRoutine = null;
        if (prototypeCamera != null)
            prototypeCamera.enabled = false;
        if (display != null)
            display.gameObject.SetActive(false);
        _visible = false;
    }

    public void ShowPrototype(bool visible)
    {
        _visible = visible;
        if (display != null)
            display.gameObject.SetActive(visible);
        if (prototypeCamera != null)
        {
            prototypeCamera.targetTexture = targetTexture;
            prototypeCamera.enabled = false;
        }
        if (visible)
        {
            _nextIdleRenderTime = Time.unscaledTime;
            RenderNow();
        }
    }

    public void SetFaceImmediate(int faceIndex)
    {
        RebuildFaceCache();
        _currentFaceIndex = Mathf.Clamp(faceIndex, 0,
            Mathf.Max(0, AvailableFaceCount - 1));
        SetRotationDegrees(GetFaceRotationDegrees(_currentFaceIndex));
    }

    public bool SupportsFace(int faceIndex)
    {
        if (_faces == null || _faces.Length == 0)
            RebuildFaceCache();
        for (int i = 0; i < _faces.Length; i++)
            if (_faces[i] != null && _faces[i].FaceIndex == faceIndex)
                return true;
        return false;
    }

    public void SetRotationDegrees(float degrees)
    {
        if (cubeRoot != null)
            cubeRoot.localRotation = Quaternion.Euler(0f, degrees, 0f);
        RenderNow();
    }

    public bool RotateBy(int direction)
    {
        if (direction == 0 || IsRotating)
            return false;

        int target = _currentFaceIndex + (direction > 0 ? 1 : -1);
        if (!SupportsFace(target))
            return false;

        _rotationRoutine = StartCoroutine(RotateRoutine(target));
        return true;
    }

    public void RenderNow()
    {
        if (!_visible || prototypeCamera == null || targetTexture == null)
            return;
        UpdateFaceRenderVisibility();
        prototypeCamera.targetTexture = targetTexture;
        prototypeCamera.Render();
    }

    /// <summary>
    /// Public selection bridge for the UI. The visual-state component handles
    /// only the integrated under-light pulse without creating material instances.
    /// </summary>
    public bool SetSelectedNode(string nodeId)
    {
        if (visualStateController == null)
            visualStateController =
                GetComponentInChildren<MachineCube3DVisualStateController>(true);
        return visualStateController != null &&
            visualStateController.SetSelectedNode(nodeId);
    }

    public bool ClearSelectedNode()
    {
        return visualStateController != null &&
            visualStateController.ClearSelectedNode();
    }

    public void ApplyQualityProfile()
    {
        MachineCube3DQualityProfile profile = MachineCube3DQuality.Profile;
        RebuildFaceCache();
        for (int i = 0; i < _faces.Length; i++)
            _faces[i]?.ApplyQuality(MachineCube3DQuality.Current);
        if (cubeRoot != null)
            SetQualityDetailsActive(cubeRoot, MachineCube3DQuality.Current);
        if (targetTexture != null &&
            (targetTexture.width != profile.renderTextureSize ||
             targetTexture.height != profile.renderTextureSize ||
             targetTexture.antiAliasing != profile.antiAliasing))
        {
            targetTexture.Release();
            targetTexture.width = profile.renderTextureSize;
            targetTexture.height = profile.renderTextureSize;
            targetTexture.antiAliasing = profile.antiAliasing;
            targetTexture.Create();
        }

        if (prototypeCamera != null)
        {
            prototypeCamera.targetTexture = targetTexture;
            prototypeCamera.allowMSAA = profile.antiAliasing > 1;
            prototypeCamera.enabled = false;
        }

        if (cubeRoot != null)
        {
            foreach (MeshRenderer renderer in
                cubeRoot.GetComponentsInChildren<MeshRenderer>(true))
            {
                renderer.shadowCastingMode = profile.shadows
                    ? ShadowCastingMode.On
                    : ShadowCastingMode.Off;
                renderer.receiveShadows = profile.shadows;
            }
        }

        foreach (Light light in GetComponentsInChildren<Light>(true))
        {
            if (light.name == "IndustrialFillLight")
                light.enabled = MachineCube3DQuality.Current >=
                    MachineCube3DQualityLevel.Balanced;
            else if (light.name == "IndustrialRimLight")
                light.enabled = MachineCube3DQuality.Current ==
                    MachineCube3DQualityLevel.High;
            else
                light.enabled = true;
            light.shadows = profile.shadows && light.name == "IndustrialKeyLight"
                ? LightShadows.Soft
                : LightShadows.None;
        }
        if (_visible)
            RenderNow();
    }

    private void RebuildFaceCache()
    {
        if (cubeRoot == null)
        {
            _faces = GetComponentsInChildren<MachineCube3DFace>(true);
        }
        else
        {
            _faces = cubeRoot.GetComponentsInChildren<MachineCube3DFace>(true);
        }
        System.Array.Sort(_faces, (a, b) => a.FaceIndex.CompareTo(b.FaceIndex));
        for (int i = 0; i < _faces.Length; i++)
            _faces[i]?.RebuildCache();
    }

    private void UpdateFaceRenderVisibility()
    {
        if (prototypeCamera == null || _faces == null)
            return;
        Vector3 cameraPosition = prototypeCamera.transform.position;
        for (int i = 0; i < _faces.Length; i++)
        {
            MachineCube3DFace face = _faces[i];
            if (face == null)
                continue;
            Vector3 toCamera = (cameraPosition - face.transform.position).normalized;
            // A small negative margin keeps the thin adjacent edge visible at
            // rest and swaps faces cleanly while rotating through 45 degrees.
            bool visible = Vector3.Dot(face.transform.forward, toCamera) > -0.08f;
            face.SetRenderVisibility(visible);
        }
    }

    private static void SetQualityDetailsActive(Transform root,
        MachineCube3DQualityLevel quality)
    {
        if (root == null)
            return;
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child.name == "OptionalDetailGeometry" ||
                child.name == "OptionalNodeDetailGeometry" ||
                child.name == "DETAIL_BALANCED")
            {
                bool active = quality >= MachineCube3DQualityLevel.Balanced;
                if (child.gameObject.activeSelf != active)
                    child.gameObject.SetActive(active);
                continue;
            }
            if (child.name == "DETAIL_HIGH")
            {
                bool active = quality == MachineCube3DQualityLevel.High;
                if (child.gameObject.activeSelf != active)
                    child.gameObject.SetActive(active);
                continue;
            }
            SetQualityDetailsActive(child, quality);
        }
    }

    private IEnumerator RotateRoutine(int targetFaceIndex)
    {
        float from = GetFaceRotationDegrees(_currentFaceIndex);
        float to = GetFaceRotationDegrees(targetFaceIndex);
        float elapsed = 0f;
        while (elapsed < rotationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / rotationDuration);
            float eased = t * t * (3f - 2f * t);
            SetRotationDegrees(Mathf.Lerp(from, to, eased));
            yield return null;
        }

        SetRotationDegrees(to);
        _currentFaceIndex = targetFaceIndex;
        _rotationRoutine = null;
    }
}

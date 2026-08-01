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
    [SerializeField] private float rotationDuration = 0.72f;

    private Coroutine _rotationRoutine;
    private int _currentFaceIndex;
    private bool _visible;
    private float _nextIdleRenderTime;

    public bool IsRotating => _rotationRoutine != null;
    public bool IsVisible => _visible && display != null &&
        display.gameObject.activeInHierarchy && prototypeCamera != null;
    public int CurrentFaceIndex => _currentFaceIndex;
    public Camera PrototypeCamera => prototypeCamera;
    public int CurrentRenderSize => targetTexture != null ? targetTexture.width : 0;

    private void Awake()
    {
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
        _currentFaceIndex = Mathf.Clamp(faceIndex, 0, 1);
        SetRotationDegrees(-90f * _currentFaceIndex);
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
        if (target < 0 || target > 1)
            return false;

        _rotationRoutine = StartCoroutine(RotateRoutine(target));
        return true;
    }

    public void RenderNow()
    {
        if (!_visible || prototypeCamera == null || targetTexture == null)
            return;
        prototypeCamera.targetTexture = targetTexture;
        prototypeCamera.Render();
    }

    public void ApplyQualityProfile()
    {
        MachineCube3DQualityProfile profile = MachineCube3DQuality.Profile;
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
            light.shadows = profile.shadows && light.name == "IndustrialKeyLight"
                ? LightShadows.Soft
                : LightShadows.None;
        }
        if (_visible)
            RenderNow();
    }

    private IEnumerator RotateRoutine(int targetFaceIndex)
    {
        float from = -90f * _currentFaceIndex;
        float to = -90f * targetFaceIndex;
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

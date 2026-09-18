using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Mantiene un presupuesto de renderizado razonable en teléfonos sin alterar
/// la frecuencia de simulación ni los multiplicadores de QA.
/// </summary>
[DisallowMultipleComponent]
public sealed class MobilePerformanceGovernor : MonoBehaviour
{
    public const int ActiveFrameRate = 30;
    public const int IdleFrameRate = 15;
    public const float IdleDelaySeconds = 20f;

    private static MobilePerformanceGovernor instance;
    private float idleSeconds;
    private int appliedFrameRate = -1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Install()
    {
        if (instance != null)
            return;

        GameObject root = new GameObject(nameof(MobilePerformanceGovernor));
        root.hideFlags = HideFlags.HideInHierarchy;
        instance = root.AddComponent<MobilePerformanceGovernor>();
        DontDestroyOnLoad(root);
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
        Application.runInBackground = false;
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        ApplyFrameRate(ActiveFrameRate);
#endif
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
        if (HasUserActivity())
        {
            idleSeconds = 0f;
            ApplyFrameRate(ActiveFrameRate);
            return;
        }

        idleSeconds += Time.unscaledDeltaTime;
        if (idleSeconds >= IdleDelaySeconds)
            ApplyFrameRate(IdleFrameRate);
#endif
    }

    private void OnApplicationFocus(bool hasFocus)
    {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
        idleSeconds = 0f;
        ApplyFrameRate(hasFocus ? ActiveFrameRate : IdleFrameRate);
#endif
    }

    private void OnApplicationPause(bool pause)
    {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
        idleSeconds = 0f;
        ApplyFrameRate(pause ? IdleFrameRate : ActiveFrameRate);
#endif
    }

    private static bool HasUserActivity()
    {
        Pointer pointer = Pointer.current;
        if (pointer != null &&
            (pointer.press.isPressed || pointer.delta.ReadValue().sqrMagnitude > 0.01f))
        {
            return true;
        }

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.anyKey.isPressed)
            return true;

        Gamepad gamepad = Gamepad.current;
        return gamepad != null && gamepad.wasUpdatedThisFrame;
    }

    private void ApplyFrameRate(int targetFrameRate)
    {
        if (appliedFrameRate == targetFrameRate &&
            Application.targetFrameRate == targetFrameRate)
            return;

        appliedFrameRate = targetFrameRate;
        Application.targetFrameRate = targetFrameRate;
    }
}

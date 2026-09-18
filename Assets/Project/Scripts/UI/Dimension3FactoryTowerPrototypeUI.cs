using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Animation-only controller for the isolated Dimension 3 visual prototype.
/// It is intentionally disconnected from game state and production systems.
/// </summary>
public sealed class Dimension3FactoryTowerPrototypeUI : MonoBehaviour
{
    private const float AnimationRefreshInterval = 1f / 20f;

    [Header("Moving factory parts")]
    public RectTransform pressHead;
    public RectTransform liftCar;
    public RectTransform fanRotor;
    public RectTransform[] conveyorMarkers;

    [Header("Signals")]
    public Image runningLamp;
    public Image queueProgress;
    public CanvasGroup steam;

    private Vector2 pressOrigin;
    private Vector2 liftOrigin;
    private Vector2[] markerOrigins;
    private float nextAnimationRefreshTime;

    private void Awake()
    {
        CacheOrigins();
    }

    private void OnEnable()
    {
        CacheOrigins();
    }

    private void Update()
    {
        if (Time.unscaledTime < nextAnimationRefreshTime)
            return;
        nextAnimationRefreshTime = Time.unscaledTime + AnimationRefreshInterval;
        ApplyPose(Time.unscaledTime);
    }

    public void ApplyPreviewPose(float time)
    {
        CacheOrigins();
        ApplyPose(time);
    }

    private void CacheOrigins()
    {
        if (pressHead != null) pressOrigin = pressHead.anchoredPosition;
        if (liftCar != null) liftOrigin = liftCar.anchoredPosition;

        if (conveyorMarkers == null) return;
        markerOrigins = new Vector2[conveyorMarkers.Length];
        for (int i = 0; i < conveyorMarkers.Length; i++)
        {
            if (conveyorMarkers[i] != null)
                markerOrigins[i] = conveyorMarkers[i].anchoredPosition;
        }
    }

    private void ApplyPose(float time)
    {
        float pulse = (Mathf.Sin(time * 2.8f) + 1f) * .5f;

        if (pressHead != null)
            pressHead.anchoredPosition = pressOrigin + Vector2.down * Mathf.SmoothStep(0f, 34f, pulse);

        if (liftCar != null)
            liftCar.anchoredPosition = liftOrigin + Vector2.up * (Mathf.Sin(time * .55f) * 42f);

        if (fanRotor != null)
            fanRotor.localRotation = Quaternion.Euler(0f, 0f, time * -80f);

        if (conveyorMarkers != null && markerOrigins != null)
        {
            for (int i = 0; i < conveyorMarkers.Length; i++)
            {
                if (conveyorMarkers[i] == null) continue;
                float travel = Mathf.Repeat(time * 48f + i * 74f, 260f);
                conveyorMarkers[i].anchoredPosition = markerOrigins[i] + Vector2.right * travel;
            }
        }

        if (runningLamp != null)
            runningLamp.color = Color.Lerp(new Color(.30f, .62f, .42f, 1f), new Color(.46f, 1f, .62f, 1f), pulse);

        if (queueProgress != null)
            queueProgress.fillAmount = .42f + pulse * .08f;

        if (steam != null)
            steam.alpha = .2f + pulse * .45f;
    }
}

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Small presentation-only animation layer for the isolated Dimension 2 pact scene.
/// It intentionally contains no game-state or economy logic.
/// </summary>
public sealed class Dimension2PactPrototypeUI : MonoBehaviour
{
    private const float AnimationRefreshInterval = 1f / 20f;

    [SerializeField] private CanvasGroup content;
    [SerializeField] private RectTransform selectedMedallion;
    [SerializeField] private Image selectedAura;

    private float elapsed;
    private float nextAnimationRefreshTime;

    private void OnEnable()
    {
        elapsed = 0f;
        if (content != null)
            content.alpha = 0f;
    }

    private void Update()
    {
        elapsed += Time.unscaledDeltaTime;
        if (Time.unscaledTime < nextAnimationRefreshTime)
            return;
        nextAnimationRefreshTime = Time.unscaledTime + AnimationRefreshInterval;

        if (content != null)
            content.alpha = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / .38f));

        float pulse = .5f + .5f * Mathf.Sin(elapsed * 2.15f);
        if (selectedMedallion != null)
            selectedMedallion.localScale = Vector3.one * Mathf.Lerp(.992f, 1.018f, pulse);

        if (selectedAura != null)
        {
            Color color = selectedAura.color;
            color.a = Mathf.Lerp(.06f, .18f, pulse);
            selectedAura.color = color;
        }
    }
}

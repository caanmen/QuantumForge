using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class MachineCubeCircuitLineUI : MonoBehaviour
{
    private Image _energy;
    private bool _lit;
    private Coroutine _routine;

    public static MachineCubeCircuitLineUI Create(RectTransform parent,
        Vector2 start, Vector2 end)
    {
        GameObject root = new GameObject("ValidatedCircuit", typeof(RectTransform));
        root.layer = parent.gameObject.layer;
        RectTransform rect = root.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);

        Vector2 delta = end - start;
        rect.anchoredPosition = (start + end) * 0.5f;
        rect.sizeDelta = new Vector2(delta.magnitude, 6f);
        rect.localRotation = Quaternion.Euler(0f, 0f,
            Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);

        Image groove = root.AddComponent<Image>();
        groove.color = new Color(0.005f, 0.012f, 0.016f, 0.10f);
        groove.raycastTarget = false;

        GameObject energyObject = new GameObject("Energy", typeof(RectTransform));
        energyObject.layer = root.layer;
        RectTransform energyRect = energyObject.GetComponent<RectTransform>();
        energyRect.SetParent(rect, false);
        energyRect.anchorMin = Vector2.zero;
        energyRect.anchorMax = Vector2.one;
        energyRect.offsetMin = new Vector2(0f, 1.25f);
        energyRect.offsetMax = new Vector2(0f, -1.25f);
        Image energy = energyObject.AddComponent<Image>();
        energy.type = Image.Type.Filled;
        energy.fillMethod = Image.FillMethod.Horizontal;
        energy.fillOrigin = 0;
        energy.fillAmount = 0f;
        energy.raycastTarget = false;

        MachineCubeCircuitLineUI line = root.AddComponent<MachineCubeCircuitLineUI>();
        line._energy = energy;
        return line;
    }

    public void SetState(bool lit, Color accent, bool animate)
    {
        if (_energy == null)
            return;
        _energy.color = Color.Lerp(accent, Color.white, 0.34f);
        if (_lit == lit)
        {
            if (_routine == null)
                _energy.fillAmount = lit ? 1f : 0f;
            return;
        }

        _lit = lit;
        if (_routine != null)
            StopCoroutine(_routine);
        if (lit && animate && isActiveAndEnabled)
            _routine = StartCoroutine(Propagate());
        else
            _energy.fillAmount = lit ? 1f : 0f;
    }

    private void OnDisable()
    {
        if (_routine != null)
            StopCoroutine(_routine);
        _routine = null;
        if (_energy != null)
            _energy.fillAmount = _lit ? 1f : 0f;
    }

    private IEnumerator Propagate()
    {
        const float duration = 0.28f;
        float elapsed = 0f;
        _energy.fillAmount = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            _energy.fillAmount = 1f - Mathf.Pow(1f - t, 2f);
            yield return null;
        }
        _energy.fillAmount = 1f;
        _routine = null;
    }
}

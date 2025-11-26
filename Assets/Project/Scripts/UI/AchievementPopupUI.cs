using System.Collections;
using TMPro;
using UnityEngine;

public class AchievementPopupUI : MonoBehaviour
{
    public static AchievementPopupUI I { get; private set; }

    [Header("Refs")]
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descText;

    [Header("Tiempos")]
    [Tooltip("Tiempo que tarda en aparecer (fade in).")]
    public float fadeInTime = 0.2f;

    [Tooltip("Tiempo visible antes de desvanecerse.")]
    public float stayTime = 2.0f;

    [Tooltip("Tiempo que tarda en desaparecer (fade out).")]
    public float fadeOutTime = 0.4f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    /// <summary>
    /// Muestra el popup con el nombre y descripción del logro.
    /// </summary>
    public void ShowPopup(string title, string description)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }
        currentRoutine = StartCoroutine(Co_ShowPopup(title, description));
    }

    private IEnumerator Co_ShowPopup(string title, string description)
    {
        if (titleText != null) titleText.text = title;
        if (descText != null)  descText.text = description;

        if (canvasGroup == null)
            yield break;

        // Fade in
        canvasGroup.gameObject.SetActive(true);
        float t = 0f;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeInTime);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Espera visible
        yield return new WaitForSeconds(stayTime);

        // Fade out
        t = 0f;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeOutTime);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        canvasGroup.gameObject.SetActive(false);

        currentRoutine = null;
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeCanvasUI : MonoBehaviour
{
    public CanvasGroup canvasGroup; // Assign the UI CanvasGroup in the inspector
    public float fadeDuration = 0.5f;

    private void Awake()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.gameObject.SetActive(false);
        }
    }

    public void ShowUI()
    {
        if (canvasGroup == null) return;

        canvasGroup.gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float time = 0f;
        canvasGroup.alpha = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }
}

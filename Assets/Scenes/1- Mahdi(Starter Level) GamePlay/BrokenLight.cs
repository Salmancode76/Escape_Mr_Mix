using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenLight : MonoBehaviour
{
    public Light targetLight;            // Assign the Light in the Inspector
    public float minIntensity = 0.1f;
    public float maxIntensity = 0.5f;
    public float flickerSpeed = 0.1f;    // Time between flickers

    private void Start()
    {
        StartCoroutine(FlickerLoop());
    }

    private IEnumerator FlickerLoop()
    {
        while (true)
        {
            float nextIntensity = Random.Range(minIntensity, maxIntensity);
            float duration = Random.Range(0.05f, 0.3f); // Random speed for more realism

            yield return StartCoroutine(FadeTo(nextIntensity, duration));

            yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
        }
    }

    private IEnumerator FadeTo(float target, float duration)
    {
        float start = targetLight.intensity;
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            targetLight.intensity = Mathf.Lerp(start, target, t);
            yield return null;
        }
    }
}

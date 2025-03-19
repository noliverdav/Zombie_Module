using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CrosshairManager : MonoBehaviour
{
    public Image crosshair;
    public bool hideWhenAiming = true;
    public float fadeDuration = 0.2f;

    private Coroutine fadeCoroutine;

    void Update()
    {
        if (hideWhenAiming)
        {
            if (Input.GetMouseButton(1))
                StartFade(0f);
            else
                StartFade(1f);
        }
    }

    void StartFade(float targetAlpha)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeCrosshair(targetAlpha));
    }

    IEnumerator FadeCrosshair(float targetAlpha)
    {
        float startAlpha = crosshair.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            crosshair.color = new Color(crosshair.color.r, crosshair.color.g, crosshair.color.b, newAlpha);
            yield return null;
        }

        crosshair.color = new Color(crosshair.color.r, crosshair.color.g, crosshair.color.b, targetAlpha);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

public class ImageFade : MonoBehaviourPun
{
    private float fadeDuration = 1.0f; // ���̵� ��/�ƿ� ���� �ð�

    private bool isFading = false; // ���̵� ������ Ȯ���ϴ� ����

    public void FadeInAndOut(float fadeTime, CanvasGroup image)
    {
        if (!isFading)
        {
            isFading = true;
            fadeDuration = fadeTime;

            StartCoroutine(FadeImage(0.0f, 1.0f, image, () =>
            {
                StartCoroutine(FadeImage(1.0f, 0.0f, image, () =>
                {
                    isFading = false;
                }));
            }));
        }
    }
    public void FadeIn(float fadeTime, CanvasGroup image)
    {
        if (!isFading)
        {
            isFading = true;
            fadeDuration = fadeTime;

            StartCoroutine(FadeImage(0.0f, 1.0f, image, () =>
            {
                isFading = false;
            }));
        }
    }

    // ���̵� ��/�ƿ� ó���ϴ� �ڷ�ƾ
    private System.Collections.IEnumerator FadeImage(float startAlpha, float targetAlpha, CanvasGroup image, System.Action onComplete)
    {
        float currentTime = 0.0f;
        CanvasGroup imageColor = image;

        float startValue = startAlpha;
        float targetValue = targetAlpha;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startValue, targetValue, currentTime / fadeDuration);
            imageColor.alpha = alpha;
            image.alpha = imageColor.alpha;
            yield return null;
        }

        onComplete?.Invoke();
    }
}

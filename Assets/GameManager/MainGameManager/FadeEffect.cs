using System.Collections;
using UnityEngine;

public class FadeEffect : MonoBehaviour
{
    public GameObject fadeImage;
    private CanvasGroup fadeCanvasGroup;
    private Coroutine coroutine;

    private void Awake()
    {
        fadeCanvasGroup = fadeImage.GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        fadeImage.gameObject.SetActive(false);
    }


    public void InvokeFadeIn()
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = StartCoroutine(FadeIn());
    }
    private IEnumerator FadeIn()
    {
        fadeImage.gameObject.SetActive(true);
        fadeCanvasGroup.blocksRaycasts = true;
        fadeCanvasGroup.interactable = true;

        float duration = 0.5f;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / duration);

            yield return null;
        }

        fadeCanvasGroup.alpha = 1;
        coroutine = null;
    }

    public void InvokeFadeOut()
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = StartCoroutine(FadeOut());
    }
    private IEnumerator FadeOut()
    {
        float duration = 0.5f;
        for(float t=0f; t< duration; t += Time.deltaTime)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / duration);

            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
        fadeCanvasGroup.interactable = false;
        fadeImage.gameObject.SetActive(false);
        coroutine = null;
    }
    

}

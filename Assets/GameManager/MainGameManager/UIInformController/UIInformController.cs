
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class UIInformController : MonoBehaviour
{
    public static UIInformController instance;
    private CanvasGroup canvasGroup;

    public GameObject midInformPanel;
    public Text midInformtext;
    public Coroutine midInformCoroutine;

    private void Awake()
    {
        if(instance  == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
    }

    private void Start()
    {
        midInformPanel.gameObject.SetActive(false);
    }

    public void PostMidInformPanel(string text, float duration)
    {
        if(midInformCoroutine != null)
        {
            StopCoroutine(midInformCoroutine);
        }
        midInformCoroutine = StartCoroutine(MidInformCoroutine(text, duration));
    }
    private IEnumerator MidInformCoroutine(string text, float duration)
    {
        midInformPanel.gameObject.SetActive(true);
        midInformtext.text = text;

        yield return new WaitForSeconds(duration);

        midInformPanel.gameObject.SetActive(false);
    }
    
}

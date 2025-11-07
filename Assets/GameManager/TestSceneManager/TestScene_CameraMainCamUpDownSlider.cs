using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestScene_CameraMainCamUpDownSlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private TestScene_CameraManager cameraManager;
    private Slider slider;
    private Coroutine currentCoroutine;

    private void Awake()
    {
        cameraManager = GetComponentInParent<TestScene_CameraManager>();
        slider = GetComponent<Slider>();
    }
    private void Start()
    {
        slider.minValue = -1f;
        slider.maxValue = 1f;
        slider.value = 0;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(HoldSlider());
    }
    private IEnumerator HoldSlider()
    {
        while (true)
        {
            cameraManager.UpDownSliderUpdate(slider.value);

            yield return null;
        }
    }
    public void OnPointerUp( PointerEventData eventData)
    {
        slider.value = 0;
        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
    }
}

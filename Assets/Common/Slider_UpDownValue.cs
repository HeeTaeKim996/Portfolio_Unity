using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Slider_UpDownValue : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    private Slider slider;
    private Coroutine currentCoroutine;
    public delegate void ValueChangedEvnetHandler(object sender, float newValue);

    [HideInInspector]
    public event ValueChangedEvnetHandler GetSliderValue;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }
    private void Start()
    {
        slider.value = 0;
        slider.minValue = -1;
        slider.maxValue = 1;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(PointerDownCoroutine());
    }
    private IEnumerator PointerDownCoroutine()  
    {
        while (true)
        {
            GetSliderValue?.Invoke(this, slider.value);
            yield return null;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        slider.value = 0;
        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
    }
}

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
    /* object 는 var과 같은 다양한 변수를 담을 수 있다. object가 var보다 더 담을 수 있는 타입이 많음.. 
       event Action 에서 Action은 delegate의 한 종류이다. Action은 매개변수? 를 지정하면 매개변수의 타입을 바꿀 수 없지만, 위처럼 object, var 같은 타입을 매개변수값으로 지정해야 할 때, 위처럼 delegate를 수동으로 만들어서, event delegate(1) 로 사용할 수 있다.
       또한 delegate를 수동으로 지정해서, object 변수를 이벤트로 캐스트할 경우, 구독한 클래스에서는 그 object 변수를, SpecificType(타입) A = object as SpecificType(타입); 처럼 타입들 다시 구체적으로 정의해야 한다.
       이 클래스에서는 사실 delegate를 따로 지정할 필요 없이, public event Action<Slider_UpDownValue, float> EventName; 으로 Action으로도 처리가 가능하지만,
       delegate를 직접 만들고, object라는 포괄적인 변수를 사용하며, GizmoController클래스에서 구독해서 받은 이 object를 as 를 사용해서 다시 구체적인 타입으로 지정하고 처리하는 작업을 배우기 좋다 생각되서 delegate를 직접 만들어 사용함 */
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForTestController_ArbitraryValueController : MonoBehaviour
{
    private ForTest_ArbitraryValueController testValueController;
    
    private Slider slider;
    [HideInInspector]
    public float inspertorMin;
    [HideInInspector]
    public float inspectorMax;
    [HideInInspector]
    public float insperctorStart;


    private void Awake()
    {
        testValueController = GetComponentInParent<ForTest_ArbitraryValueController>();
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnSliderValueChanged); // 유니티 자체 이벤트로, 슬라이더 값 변경시, AddLister(MethodA); 로 methodA가 발동되도록함
    }

    public void SetMInMaxValue(float newMinValue, float newMaxValue, float newCurrentValue)
    {
        slider.minValue = newMinValue;
        slider.maxValue = newMaxValue;
        slider.value = newCurrentValue;
    }


    public void OnSliderValueChanged(float value)
    {
        testValueController.OnArbiraryValueChanged(value);
    }
}

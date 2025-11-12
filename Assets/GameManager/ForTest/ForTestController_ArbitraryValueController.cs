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
        slider.onValueChanged.AddListener(OnSliderValueChanged);
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

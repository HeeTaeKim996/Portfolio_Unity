using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MinMaxValue
{
    public string CurrentUsingClass;
    [HideInInspector]
    public float value;
    public float minValue;
    public float maxValue;
    public float startValue;
}


public class ForTest_ArbitraryValueController : MonoBehaviour
{
    public static ForTest_ArbitraryValueController instance;

    private ForTestController_ArbitraryValueController valueSlider;

    public List<MinMaxValue> testValues = new List<MinMaxValue>();

    private int currentIndex = 0;
    [HideInInspector]
    public float value;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        valueSlider = GetComponentInChildren< ForTestController_ArbitraryValueController>();

        for(int i = 0; i < testValues.Count; i++)
        {
            testValues[i].value = testValues[i].startValue;
        }
    }
    private void Start()
    {
        SetArbitraryValue();
    }


    private void SetArbitraryValue()
    {
        float minValue = testValues[currentIndex].minValue;
        float maxValue = Mathf.Max(0, testValues[currentIndex].maxValue);
        float currentValue = testValues[currentIndex].value;

        valueSlider.SetMInMaxValue(minValue, maxValue, currentValue);

        Debug.Log($"ArbitraryValueName : {testValues[currentIndex].CurrentUsingClass} \nMinValue : {minValue}, MaxValue : {maxValue}, CurrentValue : {currentValue}");
    }

    public void OnArbiraryValueChanged(float newValue)
    {
        testValues[currentIndex].value = newValue;
        Debug.Log($"Value : {testValues[currentIndex].value}");
    }


    public void ChangeCurrentIndex()
    {
        if(currentIndex < testValues.Count - 1)
        {
            currentIndex++;
        }
        else
        {
            currentIndex = 0;
        }
        SetArbitraryValue();
    }

}

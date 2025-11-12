using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestScene_SliderManager : MonoBehaviour
{
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.wholeNumbers = true;
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public void OnEnable()
    {
        slider.minValue = 0;
        slider.maxValue = TestScene_ReplayManager.instance.totalFrame;
    }

    public void OnSliderValueChanged(float value)
    {
        int framdIndex = Mathf.RoundToInt(value);
        TestScene_ReplayManager.instance.ApplyFrame(framdIndex);
    }

    public void UpdateSliderValue(int currentFramdIndex)
    {
        slider.value = currentFramdIndex;
    }

}

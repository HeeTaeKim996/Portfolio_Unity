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
        slider.wholeNumbers = true; // 슬라이더에서 정수값만 허용
        slider.onValueChanged.AddListener(OnSliderValueChanged); // 유니티 자체 이벤트로, 슬라이더 값 변경시, AddLister(MethodA); 로 methodA가 발동되도록함
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


using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class GameManager_TimeScaleControllSlider : MonoBehaviour, IPointerUpHandler
{
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }
    private void Start()
    {
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1;
    }
    private void OnEnable()
    {
        slider.value = 1;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        Time.timeScale = slider.value;
        Debug.Log($"TimeScale : {Mathf.Round(slider.value * 10000) / 10000}");
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestScene_ReplayPauseButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private bool isPressed = false;
    private bool isOnButton = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isOnButton = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOnButton = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPressed && isOnButton)
        {
            TestScene_ReplayManager.instance.PauseReplay();
        }

        isPressed = false;
    }
}
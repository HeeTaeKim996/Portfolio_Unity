using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_PointerUpDown : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private bool isPressed = false;
    private bool isOnButton = false;
    [HideInInspector]
    public event Action<Button_PointerUpDown> OnPressUpButtonEvent;

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
            OnPressUpButton();
        }

        isPressed = false;
    }

    public void OnPressUpButton()
    {
        OnPressUpButtonEvent?.Invoke(this);
    }
}
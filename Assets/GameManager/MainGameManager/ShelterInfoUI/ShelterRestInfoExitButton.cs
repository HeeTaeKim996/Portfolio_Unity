using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShelterRestInfoExitButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private ShelterRestInfo shelterRestInfo;

    private bool isPressed = false;
    private bool isOnButton = false;


    private void Awake()
    {
        shelterRestInfo = GetComponentInParent<ShelterRestInfo>();
    }

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
            shelterRestInfo.DeactiveInfoChoosePanel();
        }

        isPressed = false;
    }
}
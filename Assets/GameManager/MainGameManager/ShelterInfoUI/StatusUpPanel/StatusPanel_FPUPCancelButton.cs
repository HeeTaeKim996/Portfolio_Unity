using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatusPanel_FPUPCancelButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private ShelterInfo_StatusUpPanel statusPanel;

    private bool isPressed = false;
    private bool isOnButton = false;


    private void Awake()
    {
        statusPanel = GetComponentInParent<ShelterInfo_StatusUpPanel>();
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
            statusPanel.OnPressedFPUpCancelButton();
        }

        isPressed = false;
    }
}
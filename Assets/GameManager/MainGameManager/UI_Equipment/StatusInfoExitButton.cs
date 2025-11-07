// #StatusInfoExitButton

using UnityEngine;
using UnityEngine.EventSystems;

public class StatusInfoExitButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private StatusInfoPanel statusInfoPanel;

    private bool isPressed = false;
    private bool isOnButton = false;


    private void Awake()
    {
        statusInfoPanel = GetComponentInParent<StatusInfoPanel>();
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
            statusInfoPanel.gameObject.SetActive(false);
        }

        isPressed = false;
    }
}

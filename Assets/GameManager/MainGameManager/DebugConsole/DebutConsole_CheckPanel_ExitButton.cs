
using UnityEngine;
using UnityEngine.EventSystems;

public class DebutConsole_CheckPanel_ExitButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private DebugConsole debugConsole;

    private bool isPressed = false;
    private bool isOnButton = false;


    private void Awake()
    {
        debugConsole = GetComponentInParent<DebugConsole>();
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
            debugConsole.checkPanel.gameObject.SetActive(false);
        }

        isPressed = false;
    }
}
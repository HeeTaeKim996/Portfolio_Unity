
using UnityEngine;
using UnityEngine.EventSystems;

public class MainGameChooseButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private MainMenuController mainmenuController;

    private bool isPressed = false;
    private bool isOnButton = false;


    private void Awake()
    {
        mainmenuController = GetComponentInParent<MainMenuController>();
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
            mainmenuController.MainGameStart();
        }

        isPressed = false;
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponDiscardButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private EquipmentController equipmentController;

    private bool isPressed = false;
    private bool isOnButton = false;


    private void Awake()
    {
        equipmentController = GetComponentInParent<EquipmentController>();
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
            equipmentController.DiscardCurrentShowedItem();
        }

        isPressed = false;
    }
}

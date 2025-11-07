// #StatusInfoStartButton

using UnityEngine;
using UnityEngine.EventSystems;

public class StatusInfoStartButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private EquipmentController equipmentController;
    private PlayerStatusManager playerStatusManager;

    private bool isPressed = false;
    private bool isOnButton = false;


    private void Awake()
    {
        equipmentController = GetComponentInParent<EquipmentController>();
        playerStatusManager = FindObjectOfType<PlayerStatusManager>();
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
            equipmentController.PostStatusInfoPanel();
            playerStatusManager.UpdateTotalData();
            Debug.Log($"From StatusInfoStartButton \n MovementSpeed : {playerStatusManager.movementSpeed} \n MinusMovementRate : {playerStatusManager.minusMovementRate} \n PlusStaminRate : {playerStatusManager.plusStaminaRate}");
        }

        isPressed = false;
    }
}

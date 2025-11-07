
using UnityEngine;
using UnityEngine.EventSystems;

public class ForEquipmentScene_PlayerRotator : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ForEquipmentScene_PlayerEquipController equipmentPlayer;


    public void GetEquipmentPlayer(ForEquipmentScene_PlayerEquipController newEquipmentPlayer)
    {
        equipmentPlayer = newEquipmentPlayer;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

    }
    public void OnDrag(PointerEventData eventData)
    {
        float rotationY = eventData.delta.x * 0.35f;

        if(equipmentPlayer != null)
        {
            equipmentPlayer.transform.Rotate(0, -rotationY, 0);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

}

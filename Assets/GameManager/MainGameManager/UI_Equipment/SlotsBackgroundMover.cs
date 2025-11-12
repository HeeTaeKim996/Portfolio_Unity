using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotsBackgroundMover : MonoBehaviour, IPointerDownHandler,IDragHandler, IPointerUpHandler, IBeginDragHandler
{
    private EquipmentController equipmentController;
    private RectTransform rectTransform;
    private bool didItDragged = false;

    public RectTransform mainBackground;
    public RectTransform exitButton;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        equipmentController = GetComponentInParent<EquipmentController>();

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        didItDragged = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        didItDragged = true;
    }

    public void OnDrag(PointerEventData eventData) 
    {
        Vector2 newPosition = rectTransform.anchoredPosition + new Vector2(0, eventData.delta.y);

        float minY = 5f;
        float maxY = mainBackground.rect.height - exitButton.rect.height - 5f;

        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        rectTransform.anchoredPosition = newPosition;
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        if (!didItDragged)
        {
            equipmentController.ShutDownItemInfo();
        }
    }
}

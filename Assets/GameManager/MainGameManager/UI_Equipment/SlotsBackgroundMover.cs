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
    /* !!! 드래가블옵젝이 드래그 중이라면, 이 스크립트의 OnDrag가 작동하지 않아서, 물체를 옮길 때 배경화면이 드래그 되지 않는다. 이렇게 작동할 수 있는 이유는, 이벤트 버블링 시스템으로, 
           IDragHanler 이벤트는 전체 옵젝 중 하나에서만 작동하며, 드래가블 옵젝들이 부모 관계라면, 자식 옵젝의 드래가블이 최우선으로 발동한다는 것. 따라서 자식옵제의 드래가블 이벤트가 발동하면, 부모 옵젝의 드래가블은 발동하지 않는다. */

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!didItDragged)
        {
            equipmentController.ShutDownItemInfo();
        }
    }
}

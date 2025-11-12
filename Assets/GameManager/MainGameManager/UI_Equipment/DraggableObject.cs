// #DraggableObject


using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour, IPointerUpHandler, IBeginDragHandler, IDragHandler, IPointerDownHandler
{
    private EquipmentController equipmentController;
    private RectTransform rectTransform;

    private Vector2 originalPosition;

    private bool isReorderMode;
    private bool isImmediateDrag;

    private Coroutine longPressCoroutine;
    private Coroutine parentDragCoroutine;

    private RectTransform equipSpace
    {
        get
        {
            if(itemCategory == ItemCategory.category_Helmet)
            {
                return equipmentController.equipHelmetSpace;
            }
            else if(itemCategory == ItemCategory.category_Weapon)
            {
                return equipmentController.equipWeaponSpace;
            }
            else if (itemCategory == ItemCategory.category_SecondaryWeapon)
            {
                return equipmentController.equipSecondaryWeaponSpace;
            }
            else if(itemCategory == ItemCategory.category_UpperClothe)
            {
                return equipmentController.equipUpperClotheSpace;
            }
            else if (itemCategory == ItemCategory.category_UnderClothe)
            {
                return equipmentController.equipUnderClotheSpace;
            }
            else if(itemCategory == ItemCategory.category_Shoes)
            {
                return equipmentController.equipShoesSpace;
            }
            else if(itemCategory == ItemCategory.category_Gloves)
            {
                return equipmentController.equipGlovesSpace;
            }
            else
            {
                return null;
            }
        }
    }

    private ItemCategory itemCategory;
    
    private void Awake()
    {
        equipmentController = GetComponentInParent<EquipmentController>();
        rectTransform = GetComponent<RectTransform>();

    }

    private void OnEnable()
    {
        IItemCategory item = GetComponent<IItemCategory>();
        if(item != null)
        {
            itemCategory = item.Category;
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        longPressCoroutine = StartCoroutine(LongPressDetection());
        originalPosition = rectTransform.anchoredPosition;
        isImmediateDrag = false;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (longPressCoroutine != null)
        {
            StopCoroutine(longPressCoroutine);
        }

        if (!isReorderMode)
        {
            isImmediateDrag = true;
            PassDragToParent(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {

        if (isImmediateDrag) return;

        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isImmediateDrag)
        {
            StopParentDragCoroutine();
            return;
        }

        if (!isReorderMode)
        {
            equipmentController.ShowItemInfo(gameObject, itemCategory);

            if (longPressCoroutine != null)
            {
                StopCoroutine(longPressCoroutine);
            }
        }

        else if(isReorderMode)
        {
            Transform thisNowParent = rectTransform.parent;

            if (thisNowParent == equipSpace)
            {
                foreach (RectTransform slot in equipmentController.blankSpaces)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(slot, eventData.position, null))
                    {
                        Transform existingItem = slot.GetComponentInChildren<DraggableObject>()?.transform;

                        if (existingItem != null && existingItem != this.transform)
                        {
                            IItemCategory item = existingItem.GetComponent<IItemCategory>();

                            if (item != null)
                            {
                                if(item.Category == itemCategory)
                                {
                                    existingItem.SetParent(rectTransform.parent);
                                    existingItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                                    rectTransform.SetParent(slot);
                                    rectTransform.anchoredPosition = Vector2.zero;

                                    EquipOtherItem(existingItem);

                                    rectTransform.localScale = Vector3.one;
                                    isReorderMode = false;
                                    equipmentController.ShutDownItemInfo();
                                    return;
                                }
                            }
                        }
                        else
                        {
                            rectTransform.SetParent(slot);
                            rectTransform.anchoredPosition = Vector2.zero;

                            EquipNull();


                            rectTransform.localScale = Vector3.one;
                            isReorderMode = false;
                            equipmentController.ShutDownItemInfo();
                            return;
                        }
                    }
                }
            }
            else
            {

                foreach (RectTransform slot in equipmentController.blankSpaces)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(slot, eventData.position, null))
                    {
                        Transform existingItem = slot.GetComponentInChildren<DraggableObject>()?.transform;

                        if (existingItem != null && existingItem != this.transform)
                        {
                            existingItem.SetParent(rectTransform.parent);
                            existingItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                            rectTransform.SetParent(slot);
                            rectTransform.anchoredPosition = Vector2.zero;


                            rectTransform.localScale = Vector3.one;
                            isReorderMode = false;
                            equipmentController.ShutDownItemInfo();
                            return;
                        }
                        else
                        {
                            rectTransform.SetParent(slot);
                            rectTransform.anchoredPosition = Vector2.zero;

                            rectTransform.localScale = Vector3.one;
                            isReorderMode = false;
                            equipmentController.ShutDownItemInfo();

                            return;
                        }
                    }
                }

                if (RectTransformUtility.RectangleContainsScreenPoint(equipSpace, eventData.position, null))
                {
                    Transform existingItem = equipSpace.GetComponentInChildren<DraggableObject>()?.transform;

                    if (existingItem != null && existingItem != this.transform)
                    {
                        existingItem.SetParent(rectTransform.parent);
                        existingItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                        rectTransform.SetParent(equipSpace);
                        rectTransform.anchoredPosition = Vector2.zero;
                    }
                    else
                    {
                        rectTransform.SetParent(equipSpace);
                        rectTransform.anchoredPosition = Vector2.zero;
                    }


                    EquipItem();

                    rectTransform.localScale = Vector3.one;
                    isReorderMode = false;
                    equipmentController.ShutDownItemInfo();
                    return;
                }
            }
        }

        rectTransform.localScale = Vector3.one;
        isReorderMode = false;

        rectTransform.anchoredPosition = originalPosition; 
    }



    private void EquipOtherItem(Transform otherRect)
    {
        if (itemCategory == ItemCategory.category_Weapon)
        {
            WeaponItem otherWeaponItem = otherRect.GetComponent<WeaponItem>();
            equipmentController.EquipWeapon(otherRect.gameObject, otherWeaponItem.weaponData);
        }
        else if(itemCategory == ItemCategory.category_SecondaryWeapon)
        {
            SecondaryWeaponItem otherSecondaryWeaponItem = otherRect.GetComponent<SecondaryWeaponItem>();
            equipmentController.EquipSecondaryWeapon(otherRect.gameObject, otherSecondaryWeaponItem.secondaryWeaponData);
        }
        else if (itemCategory == ItemCategory.category_UpperClothe)
        {
            UpperClotheItem otherUpperClotheItem = otherRect.GetComponent<UpperClotheItem>();
            equipmentController.EquipUpperClothe(otherRect.gameObject, otherUpperClotheItem.upperBodyClothingSet);
        }
        else if (itemCategory == ItemCategory.category_UnderClothe)
        {
            UnderClotheItem otherUnderClotheItem = otherRect.GetComponent<UnderClotheItem>();
            equipmentController.EquipUnderClothe(otherRect.gameObject, otherUnderClotheItem.underClotheData);
        }
        else if(itemCategory == ItemCategory.category_Helmet)
        {
            HelmetItem otherHelmetItem = otherRect.GetComponent<HelmetItem>();
            equipmentController.EquipHelmet(otherRect.gameObject, otherHelmetItem.helmetData);
        }
        else if(itemCategory == ItemCategory.category_Shoes)
        {
            ShoesItem otherShpesItem = otherRect.GetComponent<ShoesItem>();
            equipmentController.EquipShoes(otherRect.gameObject, otherShpesItem.shoesData);
        }
        else if(itemCategory == ItemCategory.category_Gloves)
        {
            GlovesItem otherGlovesItem = otherRect.GetComponent<GlovesItem>();
            equipmentController.EquipGloves(otherRect.gameObject, otherGlovesItem.glovesData);
        }
    }
    private void EquipItem()
    {
        if(itemCategory == ItemCategory.category_Weapon)
        {
            WeaponItem weaponItem = GetComponent<WeaponItem>();
            equipmentController.EquipWeapon(gameObject, weaponItem.weaponData);
        }
        else if(itemCategory == ItemCategory.category_SecondaryWeapon)
        {
            SecondaryWeaponItem secondaryWeaponItem = GetComponent<SecondaryWeaponItem>();
            equipmentController.EquipSecondaryWeapon(gameObject, secondaryWeaponItem.secondaryWeaponData);
        }
        else if(itemCategory == ItemCategory.category_UpperClothe)
        {
            UpperClotheItem upperClotheItem = GetComponent<UpperClotheItem>();
            equipmentController.EquipUpperClothe(gameObject, upperClotheItem.upperBodyClothingSet);
        }
        else if (itemCategory == ItemCategory.category_UnderClothe)
        {
            UnderClotheItem underClotheItem = GetComponent<UnderClotheItem>();
            equipmentController.EquipUnderClothe(gameObject, underClotheItem.underClotheData);
        }
        else if(itemCategory == ItemCategory.category_Helmet)
        {
            HelmetItem helmetItem = GetComponent<HelmetItem>();
            equipmentController.EquipHelmet(gameObject, helmetItem.helmetData);
        }
        else if(itemCategory == ItemCategory.category_Shoes)
        {
            ShoesItem shoesItem = GetComponent<ShoesItem>();
            equipmentController.EquipShoes(gameObject, shoesItem.shoesData);
        }
        else if(itemCategory == ItemCategory.category_Gloves)
        {
            GlovesItem glovesItem = GetComponent<GlovesItem>();
            equipmentController.EquipGloves(gameObject, glovesItem.glovesData);
        }
    }
    private void EquipNull()
    {
        if (itemCategory == ItemCategory.category_Weapon)
        {
            equipmentController.EquipWeapon(null, equipmentController.weaponData_Null);
        }
        else if(itemCategory == ItemCategory.category_SecondaryWeapon)
        {
            equipmentController.EquipSecondaryWeapon(null, null);
        }
        else if (itemCategory == ItemCategory.category_UpperClothe)
        {
            equipmentController.EquipUpperClothe(null, null);
        }
        else if (itemCategory == ItemCategory.category_UnderClothe)
        {
            equipmentController.EquipUnderClothe(null, null);
        }
        else if(itemCategory == ItemCategory.category_Helmet)
        {
            equipmentController.EquipHelmet(null, null);
        }
        else if(itemCategory == ItemCategory.category_Shoes)
        {
            equipmentController.EquipShoes(null, null);
        }
        else if(itemCategory == ItemCategory.category_Gloves)
        {
            equipmentController.EquipGloves(null, null);
        }
    }


    private IEnumerator LongPressDetection()
    {
        yield return new WaitForSeconds(0.2f);
        isReorderMode = true;
        rectTransform.localScale = Vector3.one * 0.8f;
    }

    private void PassDragToParent(PointerEventData eventData)
    {
        SlotsBackgroundMover parentDragHanler = GetComponentInParent<SlotsBackgroundMover>();

        if(parentDragHanler != null)
        {
            ExecuteEvents.Execute(parentDragHanler.gameObject, eventData, ExecuteEvents.beginDragHandler);


            parentDragCoroutine = StartCoroutine(TransferDragToParent(parentDragHanler, eventData));
        }
    }

    private IEnumerator TransferDragToParent(SlotsBackgroundMover parentDragHandler1, PointerEventData eventData1)
    {
        while (true)
        {
            ExecuteEvents.Execute(parentDragHandler1.gameObject, eventData1, ExecuteEvents.dragHandler);
            yield return null;
        }
    }

    private void StopParentDragCoroutine()
    {
        if(parentDragCoroutine != null)
        {
            StopCoroutine(parentDragCoroutine);
            parentDragCoroutine = null;
        }
    }
}

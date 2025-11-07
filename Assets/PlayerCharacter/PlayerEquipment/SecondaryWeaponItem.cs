using UnityEngine;
using UnityEngine.UI;

public class SecondaryWeaponItem : Item, IItemCategory
{
    public SecondaryWeaponData secondaryWeaponData;

    public ItemCategory Category => ItemCategory.category_SecondaryWeapon;

    public Text secondaryWeaponNameText;

    public void Initialize(SecondaryWeaponData newSecondaryWeaponData)
    {
        secondaryWeaponData = newSecondaryWeaponData;
        if (secondaryWeaponNameText != null)
        {
            secondaryWeaponNameText.text = secondaryWeaponData.secondaryWeaponName;
        }
    }
}

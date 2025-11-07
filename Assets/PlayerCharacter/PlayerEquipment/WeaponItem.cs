using UnityEngine;
using UnityEngine.UI;

public class WeaponItem : Item, IItemCategory
{
    public WeaponData weaponData;

    public ItemCategory Category => ItemCategory.category_Weapon;

    public Text weaponNameText;

    public void Initialize(WeaponData newWeaponData)
    {
        weaponData = newWeaponData;
        if(weaponNameText != null)
        {
            weaponNameText.text = weaponData.weaponName;
        }
    }
}

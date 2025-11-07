
using UnityEngine;
using UnityEngine.UI;

public class ShoesItem : Item, IItemCategory
{
    public ShoesData shoesData;

    public ItemCategory Category => ItemCategory.category_Shoes;

    public Text shoesNameText;


    public void Initialize(ShoesData newShoesData)
    {
        shoesData = newShoesData;
        if (shoesNameText != null)
        {
            shoesNameText.text = shoesData.shoesName;
        }
    }

}
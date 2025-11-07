using UnityEngine;
using UnityEngine.UI;

public class GlovesItem : Item, IItemCategory
{
    public GlovesData glovesData;

    public ItemCategory Category => ItemCategory.category_Gloves;

    public Text glovesNameText;


    public void Initialize(GlovesData newGlovesData)
    {
        glovesData = newGlovesData;
        if (glovesNameText != null)
        {
            glovesNameText.text = glovesData.glovesName;
        }
    }
}

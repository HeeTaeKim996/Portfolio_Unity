using UnityEngine;
using UnityEngine.UI;

public class UpperClotheItem : Item, IItemCategory
{
    
    public UpperBodyClothingSet upperBodyClothingSet;

    public ItemCategory Category => ItemCategory.category_UpperClothe;



    public Text upperClothingNameText;



    public void Initialize(UpperBodyClothingSet newUpperBodyClothingSet)
    {
        upperBodyClothingSet = newUpperBodyClothingSet;
        if (upperClothingNameText != null)
        {
            upperClothingNameText.text = upperBodyClothingSet.upperClotheName;
        }
    }
}

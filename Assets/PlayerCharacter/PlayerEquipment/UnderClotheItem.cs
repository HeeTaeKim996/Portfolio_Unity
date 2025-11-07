using UnityEngine;
using UnityEngine.UI;

public class UnderClotheItem : Item, IItemCategory
{

    public UnderClotheData underClotheData;

    public ItemCategory Category => ItemCategory.category_UnderClothe;



    public Text underClotheNameText;



    public void Initialize(UnderClotheData underClotheData1)
    {
        underClotheData = underClotheData1;
        if (underClotheNameText != null)
        {
            underClotheNameText.text = underClotheData.underClotheName;
        }
    }
}

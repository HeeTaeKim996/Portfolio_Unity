
using UnityEngine;
using UnityEngine.UI;

public class HelmetItem : Item, IItemCategory
{
    public HelmetData helmetData;
    
    public ItemCategory Category => ItemCategory.category_Helmet;

    public Text helmetNameText;


    public void Initialize(HelmetData newHelmetData)
    {
        helmetData = newHelmetData;
        if(helmetNameText != null)
        {
            helmetNameText.text = helmetData.helmetName;
        }
    }
}

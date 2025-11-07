
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPanel : MonoBehaviour
{
    public GameObject[] Infos;
    public GameObject infoHelmet;
    public GameObject InfoWeapon;
    public GameObject InfoSecondaryWeapon;
    public GameObject InfoUpperClothe;
    public GameObject InfoUnderClothe;
    public GameObject infoGloves;
    public GameObject infoShoes;

    public Text weaponNameText;
    public Text lightAttackDamageText;
    public Text heavyAttackDamageText;

    public Text secondaryWeaponNameText;

    public Text upperClotheNameText;

    public Text underClotheNameText;

    public Text helmetNameText;
    public Text helmetWeightText;
    public Text helmetDefenseText;

    public Text shoesNameText;
    public Text shoesWeightText;
    public Text shoesDefenseText;

    public Text glovesNameText;
    public Text glovesWeightText;
    public Text glovesDefenseText;



    private void Awake()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1;
        }

        Infos = new GameObject[] { infoHelmet,InfoWeapon, InfoSecondaryWeapon, InfoUpperClothe, InfoUnderClothe, infoGloves, infoShoes };
    }


    public void SetInfoWeapon(WeaponData weaponData)
    {
        foreach(GameObject info in Infos)
        {
            info.SetActive(false);
        }
        InfoWeapon.SetActive(true);

        weaponNameText.text = weaponData.weaponName;
        lightAttackDamageText.text = $"LightAttackDamage : {weaponData.lightAttack_1_Damage}";
        heavyAttackDamageText.text = $"HeavyAttackDamage : {weaponData.heavyAttack_Damage}";
    }
    public void SetInfoSecondaryWeapon(SecondaryWeaponData secondaryWeaponData)
    {
        foreach (GameObject info in Infos)
        {
            info.SetActive(false);
        }
        InfoSecondaryWeapon.SetActive(true);

        secondaryWeaponNameText.text = secondaryWeaponData.secondaryWeaponName;
    }
    public void SetInfoUpperClothe(UpperBodyClothingSet upperBodyData1)
    {
        foreach (GameObject info in Infos)
        {
            info.SetActive(false);
        }
        InfoUpperClothe.SetActive(true);

        upperClotheNameText.text = upperBodyData1.upperClotheName;
    }
    public void SetInfoUnderClothe(UnderClotheData underClotheData1)
    {
        foreach (GameObject info in Infos)
        {
            info.SetActive(false);
        }
        InfoUnderClothe.SetActive(true);

        underClotheNameText.text = underClotheData1.underClotheName;
    }

    public void SetInfoHelmet(HelmetData helmetData)
    {
        foreach (GameObject info in Infos)
        {
            info.SetActive(false);
        }
        infoHelmet.SetActive(true);

        helmetNameText.text = helmetData.helmetName;
        helmetWeightText.text = $"Weight : {helmetData.weight}";
        helmetDefenseText.text = $"Defense : {helmetData.defense}";
    }

    public void SetInfoSHoes(ShoesData shoesData)
    {
        foreach(GameObject info in Infos)
        {
            info.SetActive(false);
        }
        infoShoes.SetActive(true);

        shoesNameText.text = shoesData.shoesName;
        shoesWeightText.text = $"Weight : {shoesData.weight}";
        shoesDefenseText.text = $"Defense : {shoesData.defense}";
    }
    public void SEtInfoGloves(GlovesData glovesData)
    {
        foreach(GameObject info in Infos)
        {
            info.SetActive(false);
        }
        infoGloves.SetActive(true);

        glovesNameText.text = glovesData.glovesName;
        glovesWeightText.text = $"Weight : {glovesData.weight}";
        glovesDefenseText.text = $"Defense : {glovesData.defense}";
    }
}

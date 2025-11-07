using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ForEquipmentScene_PlayerEquipController : MonoBehaviour
{
    private Animator playerAnimator;

    public SkinnedMeshRenderer helmetMesh;
    public SkinnedMeshRenderer upperBodySkinnedMeshRenderer;
    public SkinnedMeshRenderer underBoduSkinnedMeshRenderer;
    public SkinnedMeshRenderer shoesMesh;
    public SkinnedMeshRenderer glovesMesh;

    public Transform rightHandTransform;
    public Transform leftHandTransform;

    private GameObject currentWeapon;
    private GameObject currentSecondaryWeapon;


    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        playerAnimator.Play("Idle", 0);
    }

    public void EquipHelmet(HelmetData newHelmetData) 
    { 
        helmetMesh.sharedMesh = newHelmetData.helmetMesh;
        helmetMesh.materials = newHelmetData.helmetMaterials;
    }

    public void EquipWeapon(WeaponData newWeaponData)
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        GameObject weaponPrefab = Resources.Load<GameObject>(newWeaponData.modelPath);
        currentWeapon = Instantiate(weaponPrefab, rightHandTransform);
        currentWeapon.transform.localPosition = newWeaponData.positionOffset;
        currentWeapon.transform.localRotation = Quaternion.Euler(newWeaponData.rotationOffset);
        currentWeapon.transform.localScale = newWeaponData.scaleOffset;

        HitBox_Player hitBox = currentWeapon.GetComponentInChildren<HitBox_Player>();
        if (hitBox != null)
        {
            hitBox.gameObject.SetActive(false);
        }

        WeaponTrailer weaponTrailer = currentWeapon.GetComponentInChildren<WeaponTrailer>();
        if (weaponTrailer != null)
        {
            weaponTrailer.gameObject.SetActive(false);
        }

    }
    public void EquipSecondaryWeapon(SecondaryWeaponData newSecondaryWeaponData)
    {
        if(currentSecondaryWeapon != null)
        {
            Destroy(currentSecondaryWeapon);
        }

        if(newSecondaryWeaponData != null)
        {
            GameObject secondaryWeaponPrefab = Resources.Load<GameObject>(newSecondaryWeaponData.modelPath);
            currentSecondaryWeapon = Instantiate(secondaryWeaponPrefab, leftHandTransform);
            currentSecondaryWeapon.transform.localPosition = newSecondaryWeaponData.positionOffset;
            currentSecondaryWeapon.transform.localRotation = Quaternion.Euler(newSecondaryWeaponData.rotationOffset);
            currentSecondaryWeapon.transform.localScale = newSecondaryWeaponData.scaleOffset;
        }      
    }
    public void EquipUpperClothe(UpperBodyClothingSet newUpperClotheData)
    {
        upperBodySkinnedMeshRenderer.sharedMesh = newUpperClotheData.upperBodyMesh;
        upperBodySkinnedMeshRenderer.materials = newUpperClotheData.upperbodyMaterals;     
    }
    public void EquipUnderClothe(UnderClotheData newUnderClotheData)
    {
        underBoduSkinnedMeshRenderer.sharedMesh = newUnderClotheData.underClotheMesh;
        underBoduSkinnedMeshRenderer.materials = newUnderClotheData.underClotheMaterals;       
    }
    public void EquipShoes(ShoesData newShoesData)
    {
        shoesMesh.sharedMesh = newShoesData.shoesMesh;
        shoesMesh.materials = newShoesData.shoesMaterials;
    }
    public void EquipGloves(GlovesData newGlovesData)
    {
        glovesMesh.sharedMesh = newGlovesData.glovesMesh;
        glovesMesh.materials = newGlovesData.glovesMaterials;
    }
}

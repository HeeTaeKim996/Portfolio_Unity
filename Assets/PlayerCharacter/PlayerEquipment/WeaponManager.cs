using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    private PlayerStatusManager playerStatusManager;
    private PlayerAttack playerAttack;
    private PlayerMovement playerMovement;

    public Transform rightHandBone;
    [HideInInspector]
    public GameObject currentWeapon;
    public Transform leftHandBone;
    [HideInInspector]
    public GameObject currentSecondaryWeapon;


    public GameObject potionPrefab;
    [HideInInspector]
    public GameObject potionObject;
    public ItemData potionTransformData;

    public SecondaryWeaponData secondaryWeaponData_Null;

    private void Awake()
    {
        playerStatusManager = GetComponent<PlayerStatusManager>();
        playerAttack = GetComponent<PlayerAttack>();
        playerMovement = GetComponent<PlayerMovement>();
        InstantiatePotionObject();

    }

    private void OnEnable()
    {


    }


    public void EquipWeapon(WeaponData weaponData)
    {
        if (weaponData == null) Debug.LogWarning("WeaponManager : ���������͸� ã�� �� ����");

        if(currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        GameObject weaponPrefab = Resources.Load<GameObject>(weaponData.modelPath);
        currentWeapon = Instantiate(weaponPrefab, rightHandBone);
        currentWeapon.transform.localPosition = weaponData.positionOffset;
        currentWeapon.transform.localRotation = Quaternion.Euler(weaponData.rotationOffset);
        currentWeapon.transform.localScale = weaponData.scaleOffset;


        playerStatusManager.SetUpAttackData(weaponData);

        HitBox_Player hitBox = currentWeapon.GetComponentInChildren<HitBox_Player>();
        if(hitBox != null)
        {
            hitBox.Initialize(playerAttack);
            playerMovement.SetHitBox(hitBox.gameObject);
        }
        else
        {
            Debug.LogWarning("WeaponManager : ��Ʈ�ڽ� ��ũ��Ʈ �Ҵ����");
        }

        WeaponTrailer weaponTrailer = currentWeapon.GetComponentInChildren<WeaponTrailer>();
        if(weaponTrailer != null)
        {
            playerMovement.SetWeaponTrailer(weaponTrailer.gameObject);
        }
    }

    public void EquipSecondaryWeapon(SecondaryWeaponData secondaryWeaponData)
    {        
        Destroy(currentSecondaryWeapon);

        if (secondaryWeaponData == null)
        {
            playerStatusManager.SetUpSecondaryWeaponData(secondaryWeaponData_Null);
        }
        else
        {
            GameObject secondaryWeaponPrefab = Resources.Load<GameObject>(secondaryWeaponData.modelPath);
            currentSecondaryWeapon = Instantiate(secondaryWeaponPrefab, leftHandBone);
            currentSecondaryWeapon.transform.localPosition = secondaryWeaponData.positionOffset;
            currentSecondaryWeapon.transform.localRotation = Quaternion.Euler(secondaryWeaponData.rotationOffset);
            currentSecondaryWeapon.transform.localScale = secondaryWeaponData.scaleOffset;

            playerStatusManager.SetUpSecondaryWeaponData(secondaryWeaponData);
        }
    }


    public void InstantiatePotionObject()
    {
        potionObject = Instantiate(potionPrefab, rightHandBone);

        potionObject.transform.localPosition = potionTransformData.positionOffset;
        potionObject.transform.localRotation = Quaternion.Euler(potionTransformData.rotationOffset);
        potionObject.transform.localScale = potionTransformData.scaleOffset;

        potionObject.SetActive(false);
    }
}

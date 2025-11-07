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


    // PotionItem TransformData => 추후 포션은 아이템 데이터로 옮겨도 될듯..
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
        if (weaponData == null) Debug.LogWarning("WeaponManager : 웨폰데이터를 찾을 수 없음");

        if(currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        GameObject weaponPrefab = Resources.Load<GameObject>(weaponData.modelPath);
        currentWeapon = Instantiate(weaponPrefab, rightHandBone);
        currentWeapon.transform.localPosition = weaponData.positionOffset;
        currentWeapon.transform.localRotation = Quaternion.Euler(weaponData.rotationOffset);
        currentWeapon.transform.localScale = weaponData.scaleOffset;
        /* 
        ※장비를 새로 만들어서 추가할 때 주의사항
          - 장비를 블렌더에서 만든 후에 FBX(애니매이션파일과동일)을 가져온 후에, 필드에 놓고, 필드에 놓은 걸 폴더 Resources내부에 위치한 폴더에 프리팹으로 만들고, 스크립타블을 작성하는 등의 과정에서,, Scale이 꼬이는 경우가 잦다
            따라서 이와 관련해서 루틴을 만들어 처리하자
        1. Fbx파일을 필드에 배치한다. 필드에 배치한 옵젝을 플레이어의 손 등에 위차하여 localPosition,localRotation,localSacle을 확인하고, 스크립타블에 확인한 값들을 입력한다.(이 때 반드시 값들은 부모본(주로rightHand)의 자식에 위치한 상태에서의
           local 값들이어야 한다)
        2. 반드시!! 다시 프리팹을 일반 필드로, 부모없는 옵젝으로 반드시 다시 만든 후에, Resources에 PreFab으로 저장한다.
        3. 이후 추후 수정사항 등을 관리하자. 히트박스의 크기 등.. 히트박스는 켜놓은 상태로 PreFab으로 저장해야 한다. 안그러면 Equip할 때, 히트박스를 못찾음..
        4. 모두 처리한 프리팹은, 필드에 있을시 삭제하자 
        */

        playerStatusManager.SetUpAttackData(weaponData);

        HitBox_Player hitBox = currentWeapon.GetComponentInChildren<HitBox_Player>();
        if(hitBox != null)
        {
            hitBox.Initialize(playerAttack);
            playerMovement.SetHitBox(hitBox.gameObject);
        }
        else
        {
            Debug.LogWarning("WeaponManager : 히트박스 스크립트 할당안함");
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




    // 추후 포션은 아이템 매니저로 옮기는게 좋을듯
    public void InstantiatePotionObject()
    {
        potionObject = Instantiate(potionPrefab, rightHandBone);

        potionObject.transform.localPosition = potionTransformData.positionOffset;
        potionObject.transform.localRotation = Quaternion.Euler(potionTransformData.rotationOffset);
        potionObject.transform.localScale = potionTransformData.scaleOffset;

        potionObject.SetActive(false);
    }
}

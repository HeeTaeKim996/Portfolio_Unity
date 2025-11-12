
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [HideInInspector]
    public PlayerMovement playerMovement;
    public PlayerHealth playerHealth;


    private Dictionary<string,DamageInfo> damageInfoDictionary;
    private DamageInfo currentAttackDamage;

    private float heavyAttackDamage;
    private float heavyAttackAdditionalDamage;

    private AudioSource impactAudio;
    private AudioClip impactAudioClip;



    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        impactAudio = gameObject.AddComponent<AudioSource>();
    }
 
    public void ChangeAttackData(WeaponData weaponData, PlayerAttackDamagesInfo playerAttackDamagesInfo)
    {
        damageInfoDictionary = new Dictionary<string, DamageInfo>
        {
            {"lightAttack_1", new DamageInfo(playerAttackDamagesInfo.lightAttack_1_Damage, playerAttackDamagesInfo.lightAttack_1_Power, Quaternion.Euler(weaponData.lightAttack_1_AttackDirection), weaponData.lightAttack_1_DirectionType, weaponData.lightAttack_1_AttackType, weaponData.lightAttack_1_Attribute) },
            {"lightAttack_2", new DamageInfo(playerAttackDamagesInfo.lightAttack_2_Damage, playerAttackDamagesInfo.lightAttack_2_Power, Quaternion.Euler(weaponData.lightAttack_2_AttackDirection), weaponData.lightAttack_2_DirectionType, weaponData.lightAttack_2_AttackType, weaponData.lightAttack_2_Attribute) },
            {"lightAttack_3", new DamageInfo(playerAttackDamagesInfo.lightAttack_3_Damage, playerAttackDamagesInfo.lightAttack_3_Power, Quaternion.Euler(weaponData.lightAttack_3_AttackDirection), weaponData.lightAttack_3_DirectionType, weaponData.lightAttack_3_AttackType, weaponData.lightAttack_3_Attribute) },
            {"lightAttack_4", new DamageInfo(playerAttackDamagesInfo.lightAttack_4_Damage, playerAttackDamagesInfo.lightAttack_4_Power, Quaternion.Euler(weaponData.lightAttack_4_AttackDirection), weaponData.lightAttack_4_DirectionType, weaponData.lightAttack_4_AttackType, weaponData.lightAttack_4_Attribute) },
            {"lightAttack_5", new DamageInfo(playerAttackDamagesInfo.lightAttack_5_Damage, playerAttackDamagesInfo.lightAttack_5_Power, Quaternion.Euler(weaponData.lightAttack_5_AttackDirection), weaponData.lightAttack_5_DirectionType, weaponData.lightAttack_5_AttackType, weaponData.lightAttack_5_Attribute) },
            {"heavyAttack", new DamageInfo(0, playerAttackDamagesInfo.heavyAttack_Power, Quaternion.Euler(weaponData.heavyAttack_AttackDirection), weaponData.heavyAttack_DirectionType, weaponData.heavyAttack_AttackType, weaponData.heavyAttack_Attribute) },
            {"runningAttack", new DamageInfo(playerAttackDamagesInfo.runningAttack_Damage, playerAttackDamagesInfo.runningAttack_Power, Quaternion.Euler(weaponData.runningAttack_AttackDirection), weaponData.runningAttack_DirectionType, weaponData.runningAttack_AttackType, weaponData.runningAttack_Attribute) }
        };

        heavyAttackDamage = playerAttackDamagesInfo.heavyAttack_Damage;
        heavyAttackAdditionalDamage = playerAttackDamagesInfo.heavyAttackAdditionalDamage;

    }



    public void SetCurrentAttackByName(string attackName)
    {
        if (string.IsNullOrEmpty(attackName))
        {
            currentAttackDamage = null;
            return;
        }

        if (damageInfoDictionary.ContainsKey(attackName))
        {
            if(attackName == "heavyAttack")
            {
                SetHeavyAttackDamage(playerMovement.heavyAttackChargeTime);
            }
            currentAttackDamage = damageInfoDictionary[attackName];
        }
        else
        {
            Debug.LogWarning("�������� �ʴ� ���� �̸�");
        }
    }


    public DamageInfo GetCurrentAttackDamage()
    {
        return currentAttackDamage;
    }

    private void SetHeavyAttackDamage(float heavyAttackChargeTime1)
    {
        float baseDamage = heavyAttackDamage;
        float additionalDamage = (heavyAttackChargeTime1 / playerMovement.heavyAttackCharging_maxChargeTime) * heavyAttackAdditionalDamage;



        damageInfoDictionary["heavyAttack"].Damage = baseDamage + additionalDamage;
    }

    public void SetImpactAudio(AudioClip newAudioClip)
    {
        impactAudioClip = newAudioClip;
    }

    public void InvokeImpactSound()
    {
        CommonMethods.AudioPlay(impactAudio, impactAudioClip, 0.7f, 0);
    }
}

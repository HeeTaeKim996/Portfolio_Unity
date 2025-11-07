

using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerStatusManager : MonoBehaviour
{
    private PlayerAttack playerAttack;
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;
    private StatusInfoPanel statusInfoPanel;

    public PlayerStatData playerStatData_Scriptable;
    private PlayerStatData playerStatData;
    private WeaponData weaponData;
    private HelmetData helmetData;
    private SecondaryWeaponData secondaryWeaponData;
    private UpperBodyClothingSet upperBodyClothingSet;
    private UnderClotheData underClotheData;
    private ShelterRestInfo shelterRestInfo;
    private ShoesData shoesData;
    private GlovesData glovesData;

    // Level
    [HideInInspector]
    public int level;
    [HideInInspector]
    public int hpLevel;
    [HideInInspector]
    public int fpLevel;
    [HideInInspector]
    public int dmLevel;
    [HideInInspector]
    public int shard;


    // Health
    [HideInInspector]
    public float maxHealth;
    private float baseHealth;

    private int healthPotionStartCount;
    private float healthPotionHealingAmount;


    // Stamina
    [HideInInspector]
    public float stamina;
    private float maxStamina;
    private float baseStamina;
    [HideInInspector]
    public float staminaRegen;
    private float dodgeStamina;
    private float runningStamina;

    // Mana
    private float mana;
    private float maxMana;
    private float baseMana;

    // Speed
    [HideInInspector]
    public float movementSpeed;


    // Weight
    private float weight;
    [HideInInspector]
    public float minusMovementRate;
    [HideInInspector]
    public float plusStaminaRate;

    // defense
    private float defense;
    private float defenseRate;

    // Staggered
    private int staggered;

    private int knockedDown;
    private int normalKnockedDown;

    private int shieldPower;

    //Damage
    [HideInInspector]
    public float damage;

    // Amount
    private float lightAttack_1_Damage;
    private float lightAttack_1_Power;
    private float lightAttack_1_Stamina;


    private float lightAttack_2_Damage;
    private float lightAttack_2_Power;
    private float lightAttack_2_Stamina;


    private float lightAttack_3_Damage;
    private float lightAttack_3_Power;
    private float lightAttack_3_Stamina;

 
    private float lightAttack_4_Damage;
    private float lightAttack_4_Power;
    private float lightAttack_4_Stamina;
  

    private float lightAttack_5_Damage;
    private float lightAttack_5_Power;
    private float lightAttack_5_Stamina;


    private float heavyAttack_Damage;
    private float heavyAttack_Power;
    private float heavyAttack_Stamina;
    private float heavyAttackAdditionalDamage;


    private float runningAttack_Damage;
    private float runningAttack_Power;
    private float runningAttack_Stamina;




    [HideInInspector]
    public int tentativeHpUpLevelPlus;
    [HideInInspector]
    public int tentativeFpUpLevelPlus;
    [HideInInspector]
    public int tentativeDmUpLevelPlus;
    [HideInInspector]
    public int tentativeShard;
    private int tenTativeLevelUpPlus;

    private void Awake()
    {
        PlayerJsonSaveData jsonSaveData = SaveSystem.LoadPlayerJsonData();

        level = jsonSaveData.level + 1;
        hpLevel = jsonSaveData.hpLevel + 1;
        fpLevel = jsonSaveData.fpLevel + 1;
        dmLevel = jsonSaveData.dmLevel + 1;
        shard = jsonSaveData.shard;

        playerAttack = GetComponent<PlayerAttack>();
        playerMovement = GetComponent<PlayerMovement>();
        playerHealth = GetComponent<PlayerHealth>();
        statusInfoPanel = FindObjectOfType<StatusInfoPanel>();
        shelterRestInfo = UIManager.instance.GetComponentInChildren<ShelterRestInfo>();

        /* 지금은 UpdateTotalData에 PlayerStatData_Scriptable도 넣었지만, 이건 초기화작업(Awake_Start) 때 입력된 후에는 변동하지 않기에, 여기 하단 Awake에 값들을 모두 넣는게 나을 듯 하다.
           이렇게 하지 않고, 밑의 UpdateTotalData에 이 데이터들을 넣으면, 순환논리에 의해 코드가 작동하지 않고, 오류가 매우 빈번히 일어난다.. 
           따라서 base 값들은, UpdateTotalData 에 넣지 말자.. 추후 PlayerDataa_Scriptable 값들 모두 Awake에 처리하는 것으로 바꾸기*/
    }

    private void OnEnable()
    {
        SetupPlayerStatData(playerStatData_Scriptable);
        UIManager.instance.UpdateShardText(shard, 0);
    }

    public void SetupPlayerStatData(PlayerStatData givenPlayerStatData)
    {
       playerStatData = givenPlayerStatData;
       UpdateTotalData();
    }
    public void SetUpAttackData(WeaponData weaponData1)
    {
        weaponData = weaponData1;
        UpdateTotalData();
    }

    public void SetUpHelmetData(HelmetData newHelmetData)
    {
        helmetData = newHelmetData;
        UpdateTotalData();
    }


    public void SetUpSecondaryWeaponData(SecondaryWeaponData newSecondaryWeaponData)
    {
        secondaryWeaponData = newSecondaryWeaponData;
        UpdateTotalData();
    }
    public void SetUpUpperClotheData(UpperBodyClothingSet newUpperBodyClothingSet)
    {
        upperBodyClothingSet = newUpperBodyClothingSet;
        UpdateTotalData();
    }
    public void SetUpUnderClotheData(UnderClotheData newUnderClotheData)
    {
        underClotheData = newUnderClotheData;
        UpdateTotalData();
    }
    public void SetUpShoesData(ShoesData newShoesData)
    {
        shoesData = newShoesData;
        UpdateTotalData();
    }
    public void SetUpGlovesData(GlovesData newGlovesData)
    {
        glovesData = newGlovesData;
        UpdateTotalData();
    }

    // @@ From StatusPanel

    public void UpdateTentativeData(int intForTentativeShard) // int가 1일시 tentative Shard 를 더하고, 0 일시 그대로두고, -1일시 빼는 것으로..
    {
        tenTativeLevelUpPlus = tentativeHpUpLevelPlus + tentativeFpUpLevelPlus + tentativeDmUpLevelPlus;

        int tenTativeHpLevel = hpLevel + tentativeHpUpLevelPlus;
        int tenTativeFpLevel = fpLevel + tentativeFpUpLevelPlus;
        int tenTativeDmLevel = dmLevel + tentativeDmUpLevelPlus;

        int tenTativeLevel = level + tenTativeLevelUpPlus;
        float tentativeHP = GetMaxHealth(tenTativeHpLevel);
        float tentativeFP = GetMaxStamina(tenTativeFpLevel);
        float tentativeDM = GetDamage(tenTativeDmLevel);




        if(intForTentativeShard == -1)
        {
            tentativeShard -= GetNeededShardToLevelUp(tenTativeLevel - 1);
        }
        else if(intForTentativeShard == 1)
        {
            tentativeShard += GetNeededShardToLevelUp(tenTativeLevel);
        }

        StatusPanel_Data statusPanelData = new StatusPanel_Data
        {
            level = tenTativeLevel,
            hpLevel = tenTativeHpLevel,
            fpLevel = tenTativeFpLevel,
            dmLevel = tenTativeDmLevel,

            shard = tentativeShard,
            shardToLevelUp = GetNeededShardToLevelUp(tenTativeLevel),

            hp = tentativeHP,
            fp = tentativeFP,
            dm = tentativeDM
        };
        shelterRestInfo.statusUpPanel.UpdatePanelInfoData(statusPanelData);
    }


    public void ApplyTenTativeData()
    {
        level += tenTativeLevelUpPlus;
        hpLevel += tentativeHpUpLevelPlus;
        fpLevel += tentativeFpUpLevelPlus;
        dmLevel += tentativeDmUpLevelPlus;
        shard = tentativeShard;

        tentativeHpUpLevelPlus = 0;
        tentativeFpUpLevelPlus = 0;
        tentativeDmUpLevelPlus = 0;
        tentativeShard = shard;
        UpdateTotalData();
    }

    private float GetMaxHealth(int newHpLevel)
    {
        float healthIncrease = 30f;
        return playerStatData.baseHealth + newHpLevel * healthIncrease;
    }
    private float GetMaxStamina(int newFpLevel)
    {
        float staminaIncrease = 30f;
        return playerStatData.baseStamina + newFpLevel * staminaIncrease;
    }
    private float GetDamage(int newDmLevel)
    {
        float damageIncrese = 40f;
        return playerStatData.damage + newDmLevel * damageIncrese + weaponData.damage;
    }
    private int GetNeededShardToLevelUp(int newLevel)
    {
        int increasedShard = 500;

        return newLevel * increasedShard;
    }

    public void UpdateTotalData()
    {
        if (playerStatData == null || weaponData == null || secondaryWeaponData == null || upperBodyClothingSet == null || underClotheData == null || helmetData == null || shoesData == null || glovesData == null) return;

        float staminaRegenIncrease = 0.05f;

        maxHealth = GetMaxHealth(hpLevel);
        maxStamina = GetMaxStamina(fpLevel);
        staminaRegen = playerStatData.staminaRegen + fpLevel * staminaRegenIncrease;
        damage = GetDamage(dmLevel);

        defense = upperBodyClothingSet.defense + underClotheData.defense + helmetData.defense + shoesData.defense + glovesData.defense;
        defenseRate = (float)defense / 100;

        weight = weaponData.weight + secondaryWeaponData.Weight + upperBodyClothingSet.weight + underClotheData.weight + helmetData.weight + shoesData.weight + glovesData.weight;
        minusMovementRate = (float)weight / 100;
        plusStaminaRate = (float)weight / 100;



        healthPotionStartCount = playerStatData.healthPotionStartCount;
        healthPotionHealingAmount = playerStatData.healthPotionHealingAmount;

        maxMana = playerStatData.baseMana;


        dodgeStamina = playerStatData.dodgeStamina * (1 + plusStaminaRate);
        runningStamina = playerStatData.runningStamina;

        staggered = playerStatData.staggered;
        normalKnockedDown = playerStatData.knockedDown;
        shieldPower = secondaryWeaponData.shieldPower;  // 

        // Giving PlayerMovement data
        movementSpeed = (1-minusMovementRate) * playerStatData.movementSpeed;



        // Amount
        lightAttack_1_Damage = damage * (float)(weaponData.lightAttack_1_Damage/ 100);
        lightAttack_2_Damage = damage * (float)(weaponData.lightAttack_2_Damage / 100);
        lightAttack_3_Damage = damage * (float)(weaponData.lightAttack_3_Damage / 100);
        lightAttack_4_Damage = damage * (float)(weaponData.lightAttack_4_Damage / 100);
        lightAttack_5_Damage = damage * (float)(weaponData.lightAttack_5_Damage / 100);
        heavyAttack_Damage = damage * (float)(weaponData.heavyAttack_Damage / 100);
        heavyAttackAdditionalDamage = damage * (float)(weaponData.heavyAttackAdditionalDamage / 100);
        runningAttack_Damage = damage * (float)(weaponData.runningAttack_Damage / 100);


        lightAttack_1_Stamina = weaponData.lightAttack_1_Stamina * (1 + plusStaminaRate);
        lightAttack_2_Stamina = weaponData.lightAttack_2_Stamina * (1 + plusStaminaRate);
        lightAttack_3_Stamina = weaponData.lightAttack_3_Stamina * (1 + plusStaminaRate);
        lightAttack_4_Stamina = weaponData.lightAttack_4_Stamina * (1 + plusStaminaRate);
        lightAttack_5_Stamina = weaponData.lightAttack_5_Stamina * (1 + plusStaminaRate);
        heavyAttack_Stamina = weaponData.heavyAttack_Stamina * (1 + plusStaminaRate);
        runningAttack_Stamina = weaponData.runningAttack_Stamina * (1 + plusStaminaRate);



        lightAttack_1_Power = weaponData.lightAttack_1_Power;
        lightAttack_2_Power = weaponData.lightAttack_2_Power;
        lightAttack_3_Power = weaponData.lightAttack_3_Power;
        lightAttack_4_Power = weaponData.lightAttack_4_Power;
        lightAttack_5_Power = weaponData.lightAttack_5_Power;
        heavyAttack_Power = weaponData.heavyAttack_Power;
        runningAttack_Power = weaponData.runningAttack_Power;


        ApplyTotalData();
        if(statusInfoPanel != null)
        {
            InformStatusData newInformStatusData = new InformStatusData
            {
                level = level,
                shard = shard,
                health = maxHealth,
                stamina = maxStamina,
                damage = damage,
                weight = weight,
                defense = defense,
            };
            statusInfoPanel.UpdateStatusInfo(newInformStatusData);
        }
    }

    private void ApplyTotalData()
    {

        playerMovement.SetUpMovementSpeed(movementSpeed);

        PlayerHealthData newPlayerHealthData = new PlayerHealthData
        {
            maxHealth = maxHealth,
            healthPotionStartCount = healthPotionStartCount,
            healthPotionHealingAmount = healthPotionHealingAmount,

            maxMana = maxMana,

            maxStamina = maxStamina,
            staminaRegen = staminaRegen,

            dodgeStamina = dodgeStamina,
            runningStamina = runningStamina,

            staggered = staggered,
            normalKnockedDown = normalKnockedDown,
            shieldPower = shieldPower,

            defenseRate = defenseRate


        };
        playerHealth.SetUpHealthData(newPlayerHealthData);


        PlayerAttackStaminaInfo newPlayerAttackStaminaInfo = new PlayerAttackStaminaInfo
        {
            lightAttack_1_Stamina = lightAttack_1_Stamina,
            lightAttack_2_Stamina = lightAttack_2_Stamina,
            lightAttack_3_Stamina = lightAttack_3_Stamina,
            lightAttack_4_Stamina = lightAttack_4_Stamina,
            lightAttack_5_Stamina = lightAttack_5_Stamina,
            heavyAttack_Stamina = heavyAttack_Stamina,
            runningAttack_Stamina = runningAttack_Stamina
        };
        playerMovement.ChangeAnimationData(weaponData, newPlayerAttackStaminaInfo);

        
        PlayerAttackDamagesInfo newPlayerAttackDamagesInfo = new PlayerAttackDamagesInfo
        {
            lightAttack_1_Damage = lightAttack_1_Damage,
            lightAttack_1_Power = lightAttack_1_Power,

            lightAttack_2_Damage = lightAttack_2_Damage,
            lightAttack_2_Power = lightAttack_2_Power,

            lightAttack_3_Damage = lightAttack_3_Damage,
            lightAttack_3_Power = lightAttack_3_Power,

            lightAttack_4_Damage = lightAttack_4_Damage,
            lightAttack_4_Power = lightAttack_4_Power,

            lightAttack_5_Damage = lightAttack_5_Damage,
            lightAttack_5_Power = lightAttack_5_Power,

            heavyAttack_Damage = heavyAttack_Damage,
            heavyAttack_Power = heavyAttack_Power,
            heavyAttackAdditionalDamage = heavyAttackAdditionalDamage,

            runningAttack_Damage = runningAttack_Damage,
            runningAttack_Power = runningAttack_Power,
        };
        playerAttack.ChangeAttackData(weaponData, newPlayerAttackDamagesInfo);
    }


    // @@

    public void PlusShard(int plusShard)
    {
        shard += plusShard;
        UIManager.instance.UpdateShardText(shard, plusShard);
    }
    public void SaveJsonData()
    {
        PlayerJsonSaveData jsonSaveData = new PlayerJsonSaveData
        {
            level = level - 1,
            hpLevel = hpLevel - 1,
            fpLevel = fpLevel - 1,
            dmLevel = dmLevel - 1,
            shard = shard
        };
        SaveSystem.SavePlayerJsonData(jsonSaveData);
    }
}

using System.Collections.Generic;
using UnityEngine;

public class FieldEnemy_Mob1 : FieldEnemy
{

    [HideInInspector]
    public FieldEnemy_Mob1_Movement mob1Movement;
    public EnemyStatData enemyStatData;


    //ParticleEffect
    public GameObject bloodEffectPrefab;

    protected override void Awake()
    {
        base.Awake();
        Setup(enemyStatData);
        mob1Movement = GetComponent<FieldEnemy_Mob1_Movement>();
    }
    protected override void Start()
    {
        base.Start();
        mob1Movement.enabled = true;
    }

    protected override void OnReviveReset()
    {
        base.OnReviveReset();

        mob1Movement.enabled = true;
        mob1Movement.OnReviveReset();
    }
    protected override void OnOnfieldReset()
    {
        base.OnOnfieldReset();
        mob1Movement.OnOnfieldReset();
    }

    public override void TurnOffOBject()
    {
        base.TurnOffOBject();
        mob1Movement.TurnOffObject();
        mob1Movement.enabled = false;
    }

    public override void SetUpToReplayMode()
    {
        base.SetUpToReplayMode();
        mob1Movement.enabled = false;
    }
    public override void SetUpToGameMode()
    {
        base.SetUpToGameMode();
        mob1Movement.enabled = true;       
    }

    protected virtual void Setup(EnemyStatData enemyStatData)
    {
        maxHealth = enemyStatData.maxHealth;

        enemyCommonData.walkingSpeed = enemyStatData.walkingSpeed;
        enemyCommonData.runningSpeed = enemyStatData.runningSpeed;
        enemyCommonData.staggered = enemyStatData.staggered;
        enemyCommonData.knockedDown = enemyStatData.knockedDown;
        enemyCommonData.detectRange = enemyStatData.detectRange;

        fieldEnemyData.giveUpRange = enemyStatData.giveUpRange;
        fieldEnemyData.giveUpTime = enemyStatData.giveUpTime;
        fieldEnemyData.shard = enemyStatData.shard;

        damageInfoDictionary = new Dictionary<string, DamageInfo>
        {
            {"Attack_Chain1_Right1" ,new DamageInfo(enemyStatData.attack_Chain1_Right1_Damage, enemyStatData.attack_Chain1_Right1_Power, Quaternion.Euler(0,-90,0), DirectionType.Normal, AttackType.Normal, Attribute.Normal) },
            {"Attack_Chain1_Left2", new DamageInfo(enemyStatData.attack_Chain1_Left2_Damage, enemyStatData.attack_Chain1_Left2_Power, Quaternion.Euler(0,90,0), DirectionType.Normal, AttackType.Normal, Attribute.Normal) },
            {"Attack_Chain1_Right3", new DamageInfo(enemyStatData.attack_Chain1_Right3_Damage, enemyStatData.attack_Chain1_Right3_Power, Quaternion.Euler(0,0,0), DirectionType.TopDown, AttackType.Normal, Attribute.Normal) },
            { "Attack_LeftHand", new DamageInfo(enemyStatData.attack_LeftThrust_Damage, enemyStatData.attack_LeftThrust_Power, Quaternion.Euler(0,0,0), DirectionType.Normal, AttackType.Normal, Attribute.Normal) },
            {"Attack_CloseLeftThrust", new DamageInfo(enemyStatData.attack_CloseLefThrust_Damage, enemyStatData.attack_CloseLeftThrust_Power, Quaternion.Euler(0,0,0), DirectionType.Normal, AttackType.Normal, Attribute.Normal) },
            {"Attack_Grab", new DamageInfo(enemyStatData.attack_Grab_Damage, enemyStatData.attack_Grab_Power, Quaternion.Euler(0,0,0), DirectionType.Normal, AttackType.NotShieldable, Attribute.Normal)}
            
        };
    }

    public override void Die()
    {
        base.Die();
        mob1Movement.InvokeDieAction();
    }

    public override void OnDamage(float damage, float power, DirectionType directionType, Vector3 fromNormal, Quaternion attackDirection, AttackType attackType, Vector3 hitPoint, Vector3 hitDirection, Attribute attribute)
    {
        base.OnDamage(damage, power, directionType, fromNormal, attackDirection, attackType, hitPoint, hitDirection, attribute);
        health -= damage;
        if (health <= 0 && !dead)
        {
            Die();
            GameManager.instance.playerStatusManager.PlusShard(fieldEnemyData.shard);

        }
        else if (health > 0)
        {
            if (power >= enemyCommonData.staggered && power < enemyCommonData.knockedDown)
            {
                mob1Movement.InvokeStaggered(directionType, fromNormal, attackDirection);
            }
            else if (power >= enemyCommonData.knockedDown)
            {
                mob1Movement.InvokeKnockedDown(directionType, fromNormal);
            }
        }

        // hitEffect
        float effectDuration = 0.5f;

        GameObject bloodEffect = Instantiate(bloodEffectPrefab, hitPoint, Quaternion.LookRotation(hitDirection));
        Destroy(bloodEffect, effectDuration);
        if(TestScene_ReplayManager.instance != null)
        {
            if (TestScene_ReplayManager.instance.isRecording)
            {
                TestScene_ReplayManager.instance.RecordEffect(bloodEffectPrefab, hitPoint, Quaternion.LookRotation(hitDirection), effectDuration);
            }
        }
    }



    public override void InvokeShielderedBack(Vector3 targetPosition)
    {
        mob1Movement.ShielderedBack(targetPosition);
    }


    public override void InvokeGrappedAction(string grapAnimationName, float animationDuration, float grappedTime, GameObject grappingPositionObject)
    {
        mob1Movement.InvokeGrappedAction(grapAnimationName, animationDuration, grappedTime, grappingPositionObject);
    }
    public override void GetGrappingTargetInformation(LivingEntity grappingTarget)
    {
        Rigidbody grapTargetRigidbody = grappingTarget.GetComponent<Rigidbody>();
        mob1Movement.GetGrabSucced(grappingTarget, grapTargetRigidbody);
    }
}

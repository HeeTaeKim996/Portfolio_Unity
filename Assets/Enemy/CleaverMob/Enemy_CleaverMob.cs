using System.Collections.Generic;
using UnityEngine;

public class Enemy_CleaverMob : FieldEnemy
{ 
    //@@Private
    [HideInInspector]
    public Enemy_CleaverMobMovement cleaveMovement;
    public EnemyRegularStatData enemyStatData;
    public enum CleaveMobType
    {
        Normal,
        Shield
    }
    public CleaveMobType cleaveMobType;

    //ParticleEffect
    public GameObject bloodEffectPrefab;

    protected override void Awake()
    {
        base.Awake();
        cleaveMovement = GetComponent<Enemy_CleaverMobMovement>();
        Setup(enemyStatData);
    }
    protected override void Start()
    {
        base.Start();
        cleaveMovement.enabled = true;
    }

    protected override void OnReviveReset()
    {
        base.OnReviveReset();

        cleaveMovement.enabled = true;
        cleaveMovement.OnReviveReset();
    }
    protected override void OnOnfieldReset()
    {
        base.OnOnfieldReset();
        cleaveMovement.OnOnfieldReset();
    }

    public override void TurnOffOBject()
    {
        base.TurnOffOBject();
        cleaveMovement.TurnOffObject();
        cleaveMovement.enabled = false;
    }

    public override void SetUpToReplayMode()
    {
        base.SetUpToReplayMode();
        cleaveMovement.enabled = false;
    }
    public override void SetUpToGameMode()
    {
        base.SetUpToGameMode();
        cleaveMovement.enabled = true;
    }

    protected virtual void Setup(EnemyRegularStatData enemyStatData)
    {
        //@@Common
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
            {"Attack_SwingCross" ,new DamageInfo(enemyStatData.attackDatas[0].attackDamage, enemyStatData.attackDatas[0].attackPower, Quaternion.Euler(0,-90,0), DirectionType.Normal, AttackType.Normal, Attribute.Normal) },
            {"Attack_Stab", new DamageInfo(enemyStatData.attackDatas[1].attackDamage, enemyStatData.attackDatas[1].attackPower, Quaternion.Euler(0,0,0), DirectionType.Normal, AttackType.Normal, Attribute.Normal) }
        };

        //@@Private
        shieldPower = enemyStatData.shieldPower;
    }

    public override void Die()
    {
        base.Die();
        cleaveMovement.InvokeDieAction();
    }

    public override void OnDamage(float damage, float power, DirectionType directionType, Vector3 fromNormal, Quaternion attackDirection, AttackType attackType, Vector3 hitPoint, Vector3 hitDirection, Attribute attribute)
    {
            //@@Private
            if (isShielding)
            {
                Vector3 refinedFrom = new Vector3(fromNormal.x, 0, fromNormal.z).normalized;

                float diffAngle = Vector3.Angle(transform.forward, refinedFrom);

                Vector3 rotatedVector = attackDirection * refinedFrom;
                float rotatedDiffAngle = Vector3.Angle(transform.forward, rotatedVector);
                float crossY = Vector3.Cross(transform.forward, rotatedVector).y;

                if (diffAngle < 80f)
                {
                    // 추후 쉴드백모션도 처리!
                    return;
                }

            }


            //@@Common
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
                cleaveMovement.InvokeStaggered(directionType, fromNormal, attackDirection);
            }
            else if (power >= enemyCommonData.knockedDown)
            {
                cleaveMovement.InvokeKnockedDown(directionType, fromNormal);
            }
        }


        // hitEffect
        float effectDuration = 0.5f;

        GameObject bloodEffect = Instantiate(bloodEffectPrefab, hitPoint, Quaternion.LookRotation(hitDirection));
        Destroy(bloodEffect, effectDuration);
        if (TestScene_ReplayManager.instance != null)
        {
            if (TestScene_ReplayManager.instance.isRecording)
            {
                TestScene_ReplayManager.instance.RecordEffect(bloodEffectPrefab, hitPoint, Quaternion.LookRotation(hitDirection), effectDuration);
            }
        }   
    }



    public override void InvokeShielderedBack(Vector3 targetPosition)
    {
        cleaveMovement.ShielderedBack(targetPosition);
    }



}

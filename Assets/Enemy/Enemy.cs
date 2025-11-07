

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCommonStatData
{
    public float walkingSpeed;
    public float runningSpeed;
    public int staggered;
    public int knockedDown;
    public float detectRange;
}


public class Enemy : LivingEntity
{
    protected Dictionary<string, DamageInfo> damageInfoDictionary;
    public List<String> stringsOfAttackTarget;

    private AudioSource impactSoundAudio;

    protected CapsuleCollider enemyCollider;

    [HideInInspector]
    public EnemyCommonStatData enemyCommonData;


    protected virtual void Awake()
    {
        impactSoundAudio = gameObject.AddComponent<AudioSource>();
        enemyCollider = GetComponent<CapsuleCollider>();
        enemyCommonData = new EnemyCommonStatData();
    }

    public override void Die()
    {
        base.Die();
    }


    public override void OnDamage(float damage, float power, DirectionType directionType, Vector3 fromNormal, Quaternion attackDirection, AttackType attackType, Vector3 hitPoint, Vector3 hitDirection, Attribute attribute)
    {
        // 자식 클래스에서 구현
    }

    public virtual void SetUpToReplayMode()
    {

    }
    public virtual void SetUpToGameMode()
    {

    }


    public void InvokeImpactSound(AudioClip newAudioClip, float newVolume)
    {
        CommonMethods.AudioPlayOneShot(impactSoundAudio, newAudioClip, newVolume);
    }

    public virtual void TurnOffOBject()
    {

    }

    public void SetCurrentAttackByName(string attackName, HitBox_Enemy hitBox)
    {
        if (damageInfoDictionary.ContainsKey(attackName))
        {
            hitBox.damageInfo = damageInfoDictionary[attackName];
        }
        else
        {
            Debug.LogWarning("Enemy: 존재하지 않는 공격 이름");
        }
    }
}

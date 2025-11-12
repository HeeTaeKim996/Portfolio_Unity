// #HitBox_Player

using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;
using static UnityEngine.EventSystems.EventTrigger;

public class HitBox_Player : MonoBehaviour
{   
    private PlayerAttack playerAttack;
    private Rigidbody rb;
    public GameObject lineStart;
    public GameObject lineEnd;


    private Vector3 nowLineStartPosition;
    private Vector3 nowLineEndPosition;

    private Vector3 previousLineStartPosition;
    private Vector3 previousLineEndPosition;

    private HashSet<LivingEntity> subscribedEntities = new HashSet<LivingEntity>();
    private HashSet<LivingEntity> hitList = new HashSet<LivingEntity>();

    private LayerMask targetLayers;

    private void Start()
    {
        targetLayers = CommonMethods.GetStringsToLayerMask(playerAttack.playerHealth.stringsOfAttackTarget);
        rb = GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.isKinematic = true;
        }

    }

    private void OnEnable()
    {
        hitList.Clear();
    }


    public void Initialize(PlayerAttack playerAttack)
    {
        this.playerAttack = playerAttack;   
    }


    private void FixedUpdate()
    {


        previousLineStartPosition = nowLineStartPosition;
        previousLineEndPosition = nowLineEndPosition;

        nowLineStartPosition = lineStart.transform.position;
        nowLineEndPosition = lineEnd.transform.position;
    }


    private void OnTriggerEnter(Collider other)
    {
        if ((targetLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            LivingEntity entity = other.GetComponent<LivingEntity>();
            if(entity != null)
            {
                if (entity.state != State.Invulnerable && !entity.dead && !hitList.Contains(entity))
                {
                    ProcesSHit(entity);
                }
                else
                {
                    entity.OnStateChanged += HandleEnemyStateChanged;
                    subscribedEntities.Add(entity);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((targetLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            LivingEntity entity = other.GetComponent<LivingEntity>();
            if (entity != null)
            {
                entity.OnStateChanged -= HandleEnemyStateChanged;
                subscribedEntities.Remove(entity);
            }
        }
    }



    private void HandleEnemyStateChanged(LivingEntity entity)
    {
        if(entity.state != State.Invulnerable && !entity.dead && !hitList.Contains(entity))
        {
            oneFrameWaitHit(entity);
            entity.OnStateChanged -= HandleEnemyStateChanged;
            subscribedEntities.Remove(entity);
        }
    }
    private IEnumerator oneFrameWaitHit(LivingEntity entity)
    {
        yield return null;

        ProcesSHit(entity);
    }

    private void OnDisable()
    {
        foreach(LivingEntity entity in subscribedEntities)
        {
            entity.OnStateChanged -= HandleEnemyStateChanged;
        }
        subscribedEntities.Clear();
    }

    private void ProcesSHit(LivingEntity entity)
    {
        hitList.Add(entity);

        Vector3 middlePoint = Vector3.Lerp(entity.hitHighPoint.position, entity.hitLowPoint.position, 0.5f);

        DamageInfo damageInfo = playerAttack.GetCurrentAttackDamage();
        float damage = damageInfo.Damage;
        float power = damageInfo.Power;
        DirectionType directionType = damageInfo.DirectionType;
        AttackType attackType = damageInfo.AttackType;
        Quaternion attackDirection = damageInfo.AttackDirection;
        Attribute attirbute = damageInfo.Attribute;

        Vector3 hitPivot = CommonMethods.ClosestPointOnLine(previousLineStartPosition, previousLineEndPosition, middlePoint);
        Vector3 hitCenterPoint = CommonMethods.ClosestPointOnLine(entity.hitLowPoint.position, entity.hitHighPoint.position, hitPivot);

        Vector3 hitDirection = hitPivot - hitCenterPoint;
        Vector3 hitPoint = hitCenterPoint + hitDirection * Mathf.Clamp(hitDirection.magnitude, 0, entity.hitPointFromCenterMagnitude);

        Vector3 fromNormal = (playerAttack.transform.position - entity.transform.position).normalized;

        if (entity.isShielding)
        {
            if (entity.shieldPower > power && Vector3.Angle(playerAttack.transform.forward, -entity.transform.forward) < 80)
            {
                playerAttack.playerHealth.InvokeShielderedBack(entity.transform.position);
            }
        }


        entity.OnDamage(damage, power, directionType, fromNormal, attackDirection, attackType, hitPoint, hitDirection, attirbute);
        playerAttack.InvokeImpactSound();
    }
    

}

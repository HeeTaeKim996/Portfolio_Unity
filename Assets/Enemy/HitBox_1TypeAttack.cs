
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class HitBox_1TypeAttack : HitBox_Enemy
{
    public Transform lineStartTransform;
    public Transform lineEndTransform;

    private Vector3 nowLineStartPosition;
    private Vector3 nowLineEndPosition;

    private Vector3 previousLineStartPosition;
    private Vector3 previousLineEndPosition;

    private HashSet<LivingEntity> hitList = new HashSet<LivingEntity>();
    private HashSet<LivingEntity> subscribedEntity = new HashSet<LivingEntity>();


    protected override void Start()
    {
        base.Start();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        foreach (LivingEntity entity in subscribedEntity)
        {
            entity.OnStateChanged -= OnStateChanged;
        }
        subscribedEntity.Clear();
        hitList.Clear();
    }


    private void Update()
    {
        previousLineStartPosition = nowLineStartPosition;
        previousLineEndPosition = nowLineEndPosition;

        nowLineStartPosition = lineStartTransform.position;
        nowLineEndPosition = lineEndTransform.position;
    }


    private void OnTriggerEnter(Collider other)
    {
        if ((targetLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            LivingEntity entity = other.GetComponent<LivingEntity>();
            if (entity != null)
            {
                if (entity.state != State.Invulnerable && entity.state != State.InvulDodgable && !entity.dead && !hitList.Contains(entity))
                {
                    ProcessHit(entity);
                }
                else
                {
                    entity.OnStateChanged += OnStateChanged;
                    subscribedEntity.Add(entity);
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
                entity.OnStateChanged -= OnStateChanged;
                subscribedEntity.Remove(entity);
            }
        }
    }

    private void OnStateChanged(LivingEntity entity)
    {
        if (entity.state != State.Invulnerable && entity.state != State.InvulDodgable && !entity.dead && !hitList.Contains(entity))
        {
            StartCoroutine(oneFrameWaitHit(entity));

            entity.OnStateChanged -= OnStateChanged;
            subscribedEntity.Remove(entity);
        }
    }
    private IEnumerator oneFrameWaitHit(LivingEntity entity)
    {
        yield return null;

        ProcessHit(entity);
    }

    private void ProcessHit(LivingEntity entity)
    {
        hitList.Add(entity);

        Vector3 middlePoint = (entity.hitLowPoint.position + entity.hitHighPoint.position) / 2;

        float damage = damageInfo.Damage;
        float power = damageInfo.Power;
        DirectionType directionType = damageInfo.DirectionType;
        AttackType attackType = damageInfo.AttackType;
        Quaternion attackDirection = damageInfo.AttackDirection;
        Attribute attribute = damageInfo.Attribute;

        Vector3 hitPivot = CommonMethods.ClosestPointOnLine(previousLineStartPosition, previousLineEndPosition, middlePoint);
        Vector3 hitCenterPoint = CommonMethods.ClosestPointOnLine(entity.hitLowPoint.position, entity.hitHighPoint.position, hitPivot);

        Vector3 hitDirection = hitPivot - hitCenterPoint;
        Vector3 hitPoint = hitCenterPoint + hitDirection * Mathf.Clamp(hitDirection.magnitude, 0, entity.hitPointFromCenterMagnitude);


        Vector3 fromNormal = (enemy.transform.position - entity.transform.position).normalized;

        if (entity.isShielding)
        {
            if (entity.shieldPower > power && Vector3.Angle(enemy.transform.forward, -entity.transform.forward) < 80)
            {
                enemy.InvokeShielderedBack(entity.transform.position);
            }
        }

        entity.OnDamage(damage, power, directionType, fromNormal, attackDirection, attackType, hitPoint, hitDirection, attribute);
        enemy.InvokeImpactSound(hitAudioClip, hitAudioVolume);
    }
}

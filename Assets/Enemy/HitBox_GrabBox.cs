
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class HitBox_GrabBox : HitBox_Enemy
{

    private HashSet<LivingEntity> subscribedEntity = new HashSet<LivingEntity>();

    protected override void Start()
    {
        base.Start();
    }


    private void OnTriggerEnter(Collider other)
    {
        if ((targetLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            LivingEntity entity = other.GetComponent<LivingEntity>();
            if (entity != null && !entity.dead)
            {
                if (entity.state != State.Invulnerable && entity.state != State.InvulDodgable)
                {
                    enemy.GetGrappingTargetInformation(entity);
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
    protected override void OnDisable()
    {
        foreach (LivingEntity entity in subscribedEntity)
        {
            entity.OnStateChanged -= OnStateChanged;
        }
        subscribedEntity.Clear();
    }

    private void OnStateChanged(LivingEntity entity)
    {
        if (entity.state != State.Invulnerable && entity.state != State.InvulDodgable && !entity.dead)
        {
            OneFrameWaitGrab(entity);
            entity.OnStateChanged -= OnStateChanged;
            subscribedEntity.Remove(entity);
        }
    }
    private IEnumerator OneFrameWaitGrab(LivingEntity entity)
    {
        yield return null;
        enemy.GetGrappingTargetInformation(entity);
    }
    
}

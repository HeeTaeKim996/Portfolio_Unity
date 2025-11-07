using System;
using UnityEngine;


public class LivingEntity : MonoBehaviour, IDamageable
{
    public float maxHealth { get; protected set; }
    [HideInInspector]
    public float health;
    public bool dead { get; protected set; }

    public Transform hitHighPoint;
    public Transform hitLowPoint;
    public float hitPointFromCenterMagnitude;

    public event Action onDeath;

    [HideInInspector]
    public event Action<LivingEntity> OnStateChanged;

    [HideInInspector]
    public virtual bool isShielding { get; set; }
    [HideInInspector]
    public int shieldPower;

    [HideInInspector]
    public Rigidbody rigidbody1;
    [HideInInspector]
    public Collider collider1;

    [HideInInspector]
    public virtual State state { get; set; }

    protected virtual void OnEnable()
    {
        dead = false;
        health = maxHealth;
    }

    public virtual void OnDamage(float damage, float power, DirectionType directionType, Vector3 fromNormal, Quaternion attackDirection, AttackType attackType, Vector3 hitPoint, Vector3 hitDirection, Attribute attribute)
    {
        health -= damage;
        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        if (onDeath != null)
        {
            onDeath();
        }
        dead = true;
    }

    public virtual void RestoreHealth(float newHealth)
    {
        if (dead)
        {
            return;
        }
        health = Mathf.Min(health + newHealth, maxHealth);
    }

    public virtual void SetState(State newState)
    {
        if (state != newState)
        {
            state = newState;
            OnStateChanged?.Invoke(this);
        }
    }

    public virtual void GetGrappingTargetInformation(LivingEntity grappingTarget)
    {

    }
    public virtual void InvokeGrappedAction(string grapAnimationName, float animationDuration, float grappedTime, GameObject grappingPositionObject)
    {

    }
    public virtual void GetFixedDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {

    }
    public virtual void InvokeShielderedBack(Vector3 targetPosition)
    {

    }
}

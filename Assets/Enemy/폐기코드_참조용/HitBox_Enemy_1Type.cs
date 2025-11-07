// #HitBox_Enemy

using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class HitBox_Enemy_1Type : HitBox_Enemy
{
    /*
    public Enemy enemy;

    private void Start()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null && !playerHealth.dead)
        {
            if (playerHealth.playerMovement.state != PlayerMovement.State.Invulnerable && playerHealth.playerMovement.state != PlayerMovement.State.InvulDodgable)
            {
                DamageInfo damageInfo = enemy.GetCurrentAttackDamage_1Type();
                float damage = damageInfo.Damage;
                float power = damageInfo.Power;
                DirectionType directionType = damageInfo.DirectionType;
                AttackType attackType = damageInfo.AttackType;
                Quaternion attackDirection = damageInfo.AttackDirection;
                Attribute attribute = damageInfo.Attribute;

                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 fromNormal = (enemy.transform.position - other.transform.position).normalized;

                if (playerHealth.playerMovement.isShielding)
                {
                    if(playerHealth.shieldPower > power && Vector3.Angle(enemy.transform.forward, -playerHealth.transform.forward) < 80)
                    {
                        enemy.enemyMovement.ShielderedBack(other.transform.position);
                    }
                }

                playerHealth.OnDamage(damage, power, directionType, fromNormal, attackDirection, attackType, hitPoint, attribute);
            }
        }
    }
    */

}

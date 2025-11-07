using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox_Enemy_2Type : HitBox_Enemy
{
    /*
    private Enemy enemy;

    public void Initialize(Enemy enemy1)
    {
        this.enemy = enemy1;
    }


    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null && !playerHealth.dead)
        {

            float damageThreshold = 3f;

            if (playerHealth.playerMovement.state != PlayerMovement.State.Invulnerable && playerHealth.playerMovement.state != PlayerMovement.State.InvulDodgable)
            {

                Dictionary<string, DamageInfo> damageInfoDict = enemy.GetCurrentAttackDamage_2Type();

                Vector3 hitPoint = other.ClosestPoint(transform.position);
                float distnace = Vector3.Distance(transform.position, hitPoint);

                DamageInfo selectedDamageInfo = distnace <= damageThreshold ? damageInfoDict["Second"] : damageInfoDict["Third"];



                float damage = selectedDamageInfo.Damage;
                float power = selectedDamageInfo.Power;
                DirectionType directionType = selectedDamageInfo.DirectionType;
                AttackType attackType = selectedDamageInfo.AttackType;
                Quaternion attackDirection = selectedDamageInfo.AttackDirection;
                Attribute attribute = selectedDamageInfo.Attribute;

                Vector3 fromNormal = (transform.position - other.transform.position).normalized;

                playerHealth.OnDamage(damage, power, directionType, fromNormal, attackDirection, attackType, hitPoint, attribute);

            }
        }
    }
    
    */

}

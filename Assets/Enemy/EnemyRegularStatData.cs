
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyAttackData
{
    public string attackName;
    public float attackDamage;
    public int attackPower;
}

[CreateAssetMenu(fileName = "EnemyRegularStatData", menuName = "Scriptable/EnemyRegularStatData")]
public class EnemyRegularStatData : ScriptableObject
{
    [Header("BaseStat")]
    public int maxHealth;
    public float runningSpeed;
    public float walkingSpeed;

    public float detectRange;
    public float giveUpRange;
    public int giveUpTime;

    public int staggered;
    public int knockedDown;

    public List<EnemyAttackData> attackDatas;


    [Header("ETC")]
    public int shard;

    [Header("Plus")]
    public int shieldPower;
}

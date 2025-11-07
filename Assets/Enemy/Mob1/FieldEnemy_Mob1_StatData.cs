using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatData", menuName = "Scriptable/EnemyStat")]
public class EnemyStatData : ScriptableObject
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

    public float attack_Chain1_Right1_Damage;
    public int attack_Chain1_Right1_Power;

    public float attack_Chain1_Left2_Damage;
    public int attack_Chain1_Left2_Power;

    public float attack_Chain1_Right3_Damage;
    public float attack_Chain1_Right3_Power;

    public float attack_LeftThrust_Damage;
    public int attack_LeftThrust_Power;

    public float attack_CloseLefThrust_Damage;
    public int attack_CloseLeftThrust_Power;

    public float attack_Grab_Damage;
    public int attack_Grab_Power;



    [Header("ETC")]
    public int shard;
    
}

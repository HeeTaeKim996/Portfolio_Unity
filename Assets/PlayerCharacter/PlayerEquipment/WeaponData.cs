using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Path")]
    public string scriptablePath;

    [Header("Transform")]
    public string weaponName;
    public string modelPath;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;
    public Vector3 scaleOffset;
    public float damage;
    public float weight;

    [Header("Animation")]
    public string lightAttack_1_Animation;
    public float lightAttack_1_Duration;
    public float lightAttack_1_HitBoxOnTime;
    public float lightAttack_1_HitBoxOffTime;
    public float lightAttack_1_swingSoundTime;
    public DirectionType lightAttack_1_DirectionType;
    public Vector3 lightAttack_1_AttackDirection;
    public AttackType lightAttack_1_AttackType;
    public Attribute lightAttack_1_Attribute;
    public AudioClip lightAttack_1_SwingSound;
    public AudioClip lightAttack_1_ImpactSound;

    [Space(10)]

    public string lightAttack_2_Animation;
    public float lightAttack_2_Duration;
    public float lightAttack_2_HitBoxOnTime;
    public float lightAttack_2_HitBoxOffTime;
    public float lightAttack_2_swingSoundTime;
    public DirectionType lightAttack_2_DirectionType;
    public Vector3 lightAttack_2_AttackDirection;
    public AttackType lightAttack_2_AttackType;
    public Attribute lightAttack_2_Attribute;
    public AudioClip lightAttack_2_SwingSound;
    public AudioClip lightAttack_2_ImpactSound;


    [Space(10)]

    public string lightAttack_3_Animation;
    public float lightAttack_3_Duration;
    public float lightAttack_3_HitBoxOnTime;
    public float lightAttack_3_HitBoxOffTime;
    public float lightAttack_3_swingSoundTime;
    public DirectionType lightAttack_3_DirectionType;
    public Vector3 lightAttack_3_AttackDirection;
    public AttackType lightAttack_3_AttackType;
    public Attribute lightAttack_3_Attribute;
    public AudioClip lightAttack_3_SwingSound;
    public AudioClip lightAttack_3_ImpactSound;

    [Space(10)]

    public string lightAttack_4_Animation;
    public float lightAttack_4_Duration;
    public float lightAttack_4_HitBoxOnTime;
    public float lightAttack_4_HitBoxOffTime;
    public float lightAttack_4_swingSoundTime;
    public DirectionType lightAttack_4_DirectionType;
    public Vector3 lightAttack_4_AttackDirection;
    public AttackType lightAttack_4_AttackType;
    public Attribute lightAttack_4_Attribute;
    public AudioClip lightAttack_4_SwingSound;
    public AudioClip lightAttack_4_ImpactSound;

    [Space(10)]

    public string lightAttack_5_Animation;
    public float lightAttack_5_Duration;
    public float lightAttack_5_HitBoxOnTime;
    public float lightAttack_5_HitBoxOffTime;
    public float lightAttack_5_swingSoundTime;
    public DirectionType lightAttack_5_DirectionType;
    public Vector3 lightAttack_5_AttackDirection;
    public AttackType lightAttack_5_AttackType;
    public Attribute lightAttack_5_Attribute;
    public AudioClip lightAttack_5_SwingSound;
    public AudioClip lightAttack_5_ImpactSound;

    [Space(10)]

    public string heavyAttackCharging_Animation;
    public float heavyAttackCharging_minChargeTime;
    public float heavyAttackCharging_maxChargeTime;

    [Space(10)]

    public string heavyAttack_Animation;
    public float heavyAttack_Duration;
    public float heavyAttack_HitBoxOnTime;
    public float heavyAttack_HitBoxOffTime;
    public float heavyAttack_swingSoundTime;
    public DirectionType heavyAttack_DirectionType;
    public Vector3 heavyAttack_AttackDirection;
    public AttackType heavyAttack_AttackType;
    public Attribute heavyAttack_Attribute;
    public AudioClip heavyAttack_SwingSound;
    public AudioClip heavyAttack_ImpactSound;

    [Space(10)]

    public string runningAttack_Animation;
    public float runningAttack_Duration;
    public float runningAttack_HitBoxOnTime;
    public float runningAttack_HitBoxOffTime;
    public float runningAttack_swingSoundTime;
    public DirectionType runningAttack_DirectionType;
    public Vector3 runningAttack_AttackDirection;
    public AttackType runningAttack_AttackType;
    public Attribute runningAttack_Attribute;
    public AudioClip runningAttack_SwingSound;
    public AudioClip runningAttack_ImpactSound;

    [Space(10)]

    [Header("Amount")]
    public float lightAttack_1_Damage;
    public float lightAttack_1_Power;
    public float lightAttack_1_Stamina;

    [Space(10)]

    public float lightAttack_2_Damage;
    public float lightAttack_2_Power;
    public float lightAttack_2_Stamina;

    [Space(10)]

    public float lightAttack_3_Damage;
    public float lightAttack_3_Power;
    public float lightAttack_3_Stamina;

    [Space(10)]

    public float lightAttack_4_Damage;
    public float lightAttack_4_Power;
    public float lightAttack_4_Stamina;

    [Space(10)]

    public float lightAttack_5_Damage;
    public float lightAttack_5_Power;
    public float lightAttack_5_Stamina;

    [Space(10)]

    public float heavyAttack_Damage;
    public float heavyAttack_Power;
    public float heavyAttack_Stamina;
    public float heavyAttackAdditionalDamage;

    [Space(10)]

    public float runningAttack_Damage;
    public float runningAttack_Power;
    public float runningAttack_Stamina;
}

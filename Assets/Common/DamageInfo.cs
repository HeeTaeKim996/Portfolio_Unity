// #DamageInfo

public class DamageInfo
{
    public float Damage { get; set; }
    public float Power { get; set; }

    public UnityEngine.Quaternion AttackDirection { get; set; }

    public DirectionType DirectionType{get;set;}

    public AttackType AttackType { get; set; }

    public Attribute Attribute { get; set; }

    public DamageInfo(float damage, float power, UnityEngine.Quaternion attackDirection, DirectionType directionType, AttackType attackType, Attribute attribute)
    {
        Damage = damage;
        Power = power;
        AttackDirection = attackDirection;
        DirectionType = directionType;
        AttackType = attackType;
        Attribute = attribute;
    }
}

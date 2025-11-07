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

    /* Class를 다른 스크립트 내부에, public class DamageInfo(){... 이렇게 해도 된다. 하지만 이렇게 새로운 스크립트에 using없이 class자체를 생성하면,
       여러 스크립트들에서 공통으로 참조하기 편한 이점이 있다(각 스크립트에서 class를 정의할 필요도 없음.
       또한 이렇게 여러 타입으로 구성된 2열 이상의 데이터는, Class 와 Structure라는 것으로 관리할 수 있는데, 
       Class의 경우에는 변동가능한 변수에 적합하고, Structure는 변동이 없는 상수에 적합하여, 후자는 간단한 데이터 관리에 적합하다 */
}

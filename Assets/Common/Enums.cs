// #Enums

public enum State
{
    Normal,
    Invulnerable,
    InvulDodgable,
    Vulnerable,
    VulMovable
}
public enum DirectionType
{
    Normal,
    TopDown,
    BottomUp
}

public enum AttackType
{
    Normal,
    NotShieldable // ���� �߰�
}

public enum Attribute
{
    Normal,
    Fire
}

public enum ItemCategory
{
    category_Helmet,
    category_Weapon,
    category_SecondaryWeapon,
    category_UpperClothe,
    category_UnderClothe,
    category_Shoes,
    category_Gloves
}

public enum GameObjectType
{
    None,
    Trailer,
    HitBox
}

public enum ABCEnum{
    A,
    B,
    C
}

public enum RecordSoundDataType
{
    Play,
    PlayOneShot
}
public enum ResetState
{
    Normal,
    Revive,
    OnField
}
public enum SuicidableState
{
    Suicidable,
    NotSuicidable
}
public enum PolygonType
{
    sphere,
    box
}

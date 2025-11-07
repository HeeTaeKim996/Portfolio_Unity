using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatData", menuName = "Scriptable/PlayerStat")]
public class PlayerStatData : ScriptableObject
{
    [Header("Health")]
    public int baseHealth;
    public int baseMana;
    public int healthPotionStartCount;
    public float healthPotionHealingAmount;


    [Header("Stamina")]
    public int baseStamina;
    public int staminaRegen;

    public int dodgeStamina;
    public int runningStamina;
    public float regenCoolDownTime;


    [Header("AboutPower")]
    public int staggered;
    public int knockedDown;


    [Header("Damage")]
    public float damage;

    [Header("ETC")]
    public float movementSpeed;



}

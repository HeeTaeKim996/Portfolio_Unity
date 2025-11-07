using UnityEngine;

public class PlayerHealthData
{
    [Header("Health")]
    public float maxHealth;

    public int healthPotionStartCount;
    public float healthPotionHealingAmount;

    [Header("Mana")]
    public float maxMana;

    [Header("Stamina")]
    public float maxStamina;
    public float staminaRegen;

    public float dodgeStamina;
    public float runningStamina;

    [Header("AboutPower")]
    public int staggered;
    public int normalKnockedDown;
    public int shieldPower;


    // defense
    public float defenseRate;


}
